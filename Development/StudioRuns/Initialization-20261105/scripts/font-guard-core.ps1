# RRPG font-guard core: TMP Dynamic FontAsset drift classification (pure functions).
# Reads two files and returns a verdict; never writes and never updates the baseline.

function Get-Blocks([string]$text) {
  $rx = [regex]'(?ms)^--- !u!(\d+) &(-?\d+)(.*?)(?=^--- !u!|\z)'
  $list = New-Object System.Collections.Generic.List[object]
  foreach ($m in $rx.Matches($text)) {
    $body = $m.Groups[3].Value
    $kind = 'other'
    if ($body -match '(?m)^  m_AtlasTextures:') { $kind = 'font' }
    elseif ($body -match '(?m)^Material:') { $kind = 'material' }
    elseif ($body -match '(?m)^Texture2D:') { $kind = 'texture' }
    $list.Add([pscustomobject]@{ Type = [int]$m.Groups[1].Value; FileId = $m.Groups[2].Value; Kind = $kind; Body = $body })
  }
  return $list
}

function Get-Chars([string]$fontBody) {
  $rx = [regex]'(?m)^\s+- m_ElementType: 1\s*?\r?\n\s+m_Unicode: (\d+)\s*?\r?\n\s+m_GlyphIndex: (\d+)'
  $list = New-Object System.Collections.Generic.List[object]
  foreach ($m in $rx.Matches($fontBody)) {
    $list.Add([pscustomobject]@{ Unicode = [int]$m.Groups[1].Value; GlyphIndex = [int]$m.Groups[2].Value })
  }
  return $list
}

function Get-Glyphs([string]$fontBody) {
  $rx = [regex]'(?ms)^\s+- m_Index: (\d+)(.*?)(?=^\s+- m_Index:|^\s+m_CharacterTable:|\z)'
  $list = New-Object System.Collections.Generic.List[object]
  foreach ($m in $rx.Matches($fontBody)) {
    $entry = $m.Groups[2].Value
    $rect = [regex]::Match($entry, '(?m)m_GlyphRect:\s*?\r?\n\s+m_X: (-?\d+)\s*?\r?\n\s+m_Y: (-?\d+)\s*?\r?\n\s+m_Width: (-?\d+)\s*?\r?\n\s+m_Height: (-?\d+)')
    $atlas = [regex]::Match($entry, '(?m)m_AtlasIndex: (-?\d+)')
    if (-not $rect.Success -or -not $atlas.Success) { return $null }
    $list.Add([pscustomobject]@{
      Index = [int]$m.Groups[1].Value
      X = [int]$rect.Groups[1].Value
      Y = [int]$rect.Groups[2].Value
      W = [int]$rect.Groups[3].Value
      H = [int]$rect.Groups[4].Value
      Atlas = [int]$atlas.Groups[1].Value
      Raw = $m.Groups[0].Value
    })
  }
  return $list
}

function Get-FaceInfoText([string]$fontBody) {
  $m = [regex]::Match($fontBody, '(?sm)^  m_FaceInfo:(.*?)^  m_[A-Za-z]')
  if (-not $m.Success) { return $null }
  return $m.Groups[1].Value
}

function Get-AtlasRefs([string]$fontBody) {
  $m = [regex]::Match($fontBody, '(?ms)^  m_AtlasTextures:\s*?\r?\n((?:\s+- \{fileID: -?\d+\}\s*?\r?\n)+)')
  if (-not $m.Success) { return $null }
  $list = New-Object System.Collections.Generic.List[string]
  foreach ($r in [regex]::Matches($m.Groups[1].Value, '\{fileID: (-?\d+)\}')) { $list.Add($r.Groups[1].Value) }
  return $list
}

function Get-AtlasMeta([string]$textureBody) {
  $w = [regex]::Match($textureBody, '(?m)^  m_Width: (\d+)')
  $h = [regex]::Match($textureBody, '(?m)^  m_Height: (\d+)')
  $fmt = [regex]::Match($textureBody, '(?m)^  m_TextureFormat: (\d+)')
  $data = [regex]::Match($textureBody, '(?m)^  _typelessdata: ([0-9a-fA-F\r\n]+)')
  if (-not $w.Success -or -not $h.Success -or -not $fmt.Success) { return $null }
  $hex = ''
  if ($data.Success) { $hex = ($data.Groups[1].Value -replace '\s', '') }
  return [pscustomobject]@{ Width = [int]$w.Groups[1].Value; Height = [int]$h.Groups[1].Value; Format = [int]$fmt.Groups[1].Value; Hex = $hex }
}

function ConvertFrom-HexBytes([string]$hex) {
  if ([string]::IsNullOrEmpty($hex)) { return [byte[]]@() }
  if (($hex.Length % 2) -ne 0) { return $null }
  try { return [System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary]::Parse($hex).Value }
  catch { return $null }
}

function Test-RectsWithin([int]$w, [int]$h, $rects) {
  foreach ($r in $rects) {
    if ($r.W -le 0 -or $r.H -le 0) { return $false }
    if ($r.X -lt 0 -or $r.Y -lt 0) { return $false }
    if (($r.X + $r.W) -gt $w -or ($r.Y + $r.H) -gt $h) { return $false }
  }
  return $true
}

function Test-Overlap($a, $b) {
  return -not (($a.X + $a.W) -le $b.X -or ($b.X + $b.W) -le $a.X -or ($a.Y + $a.H) -le $b.Y -or ($b.Y + $b.H) -le $a.Y)
}

# Verdict: PASS | LEGITIMATE_GROWTH | INCIDENT | NEEDS_REVIEW
function Get-FontDriftVerdict([string]$oldPath, [string]$newPath) {
  $oldBytes = [System.IO.File]::ReadAllBytes($oldPath)
  $newBytes = [System.IO.File]::ReadAllBytes($newPath)
  $detail = [ordered]@{}
  if ([System.Linq.Enumerable]::SequenceEqual($oldBytes, $newBytes)) {
    return [pscustomobject]@{ Verdict = 'PASS'; Detail = $detail }
  }
  $oldText = [System.IO.File]::ReadAllText($oldPath)
  $newText = [System.IO.File]::ReadAllText($newPath)
  $oldBlocks = Get-Blocks $oldText
  $newBlocks = Get-Blocks $newText
  $detail['oldBlocks'] = $oldBlocks.Count
  $detail['newBlocks'] = $newBlocks.Count

  $oldFont = $oldBlocks | Where-Object { $_.Kind -eq 'font' } | Select-Object -First 1
  $newFont = $newBlocks | Where-Object { $_.Kind -eq 'font' } | Select-Object -First 1
  if ($null -eq $oldFont -or $null -eq $newFont) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'font block missing' } }
  }

  $oldChars = Get-Chars $oldFont.Body
  $newChars = Get-Chars $newFont.Body
  $oldGlyphs = Get-Glyphs $oldFont.Body
  $newGlyphs = Get-Glyphs $newFont.Body
  if ($null -eq $oldGlyphs -or $null -eq $newGlyphs) {
    return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = 'glyph parse failed' } }
  }
  $detail['oldChars'] = $oldChars.Count
  $detail['newChars'] = $newChars.Count
  $detail['oldGlyphs'] = $oldGlyphs.Count
  $detail['newGlyphs'] = $newGlyphs.Count

  if ($newChars.Count -lt $oldChars.Count) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'character table shrank' } }
  }
  if ($newGlyphs.Count -lt $oldGlyphs.Count) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'glyph table shrank' } }
  }
  if ($newChars.Count -ne $newGlyphs.Count) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'char/glyph count mismatch' } }
  }

  for ($i = 0; $i -lt $oldChars.Count; $i++) {
    if ($oldChars[$i].Unicode -ne $newChars[$i].Unicode -or $oldChars[$i].GlyphIndex -ne $newChars[$i].GlyphIndex) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('existing character modified or removed at ' + $i) } }
    }
  }

  for ($i = 0; $i -lt $oldGlyphs.Count; $i++) {
    if ($oldGlyphs[$i].Raw -ne $newGlyphs[$i].Raw) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('existing glyph modified at ' + $i); glyph = $oldGlyphs[$i].Index } }
    }
  }

  $addedChars = @()
  for ($i = $oldChars.Count; $i -lt $newChars.Count; $i++) { $addedChars += $newChars[$i] }
  $addedGlyphs = @()
  for ($i = $oldGlyphs.Count; $i -lt $newGlyphs.Count; $i++) { $addedGlyphs += $newGlyphs[$i] }
  if ($addedChars.Count -ne $addedGlyphs.Count) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'added char/glyph count mismatch' } }
  }

  $addedGlyphIds = New-Object System.Collections.Generic.HashSet[int]
  foreach ($g in $addedGlyphs) { [void]$addedGlyphIds.Add($g.Index) }
  foreach ($c in $addedChars) {
    if (-not $addedGlyphIds.Contains($c.GlyphIndex)) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('added character has no new glyph: U+' + $c.Unicode) } }
    }
  }

  $oldMat = $oldBlocks | Where-Object { $_.Kind -eq 'material' } | Select-Object -First 1
  $newMat = $newBlocks | Where-Object { $_.Kind -eq 'material' } | Select-Object -First 1
  if ($null -eq $oldMat -or $null -eq $newMat) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'material block missing' } }
  }
  if ($oldMat.Body -ne $newMat.Body) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'material changed' } }
  }

  $oldFace = Get-FaceInfoText $oldFont.Body
  $newFace = Get-FaceInfoText $newFont.Body
  if ($null -eq $oldFace -or $null -eq $newFace) {
    return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = 'faceinfo parse failed' } }
  }
  if ($oldFace -ne $newFace) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'faceinfo changed' } }
  }

  $oldSrc = [regex]::Match($oldFont.Body, '(?m)^  m_SourceFontFileGUID: (\S+)').Groups[1].Value
  $newSrc = [regex]::Match($newFont.Body, '(?m)^  m_SourceFontFileGUID: (\S+)').Groups[1].Value
  if ($oldSrc -ne $newSrc) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'source font guid changed' } }
  }
  $oldMode = [regex]::Match($oldFont.Body, '(?m)^  m_AtlasPopulationMode: (\d+)').Groups[1].Value
  $newMode = [regex]::Match($newFont.Body, '(?m)^  m_AtlasPopulationMode: (\d+)').Groups[1].Value
  if ($oldMode -ne '1' -or $newMode -ne '1') {
    return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = 'atlas population mode is not Dynamic' } }
  }

  $oldRefs = Get-AtlasRefs $oldFont.Body
  $newRefs = Get-AtlasRefs $newFont.Body
  if ($null -eq $oldRefs -or $null -eq $newRefs) {
    return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = 'atlas refs parse failed' } }
  }
  $oldNonNull = @($oldRefs | Where-Object { $_ -ne '0' })
  $newNonNull = @($newRefs | Where-Object { $_ -ne '0' })
  if ($newNonNull.Count -lt $oldNonNull.Count) {
    return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'atlas page removed' } }
  }
  for ($i = 0; $i -lt $oldNonNull.Count; $i++) {
    if ($oldNonNull[$i] -ne $newNonNull[$i]) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('atlas ref changed at ' + $i) } }
    }
  }
  $detail['oldPages'] = $oldNonNull.Count
  $detail['newPages'] = $newNonNull.Count

  $oldTex = @{}
  foreach ($b in ($oldBlocks | Where-Object { $_.Kind -eq 'texture' })) { $oldTex[$b.FileId] = $b }
  $newTex = @{}
  foreach ($b in ($newBlocks | Where-Object { $_.Kind -eq 'texture' })) { $newTex[$b.FileId] = $b }

  $newRectsByPage = @{}
  foreach ($g in $addedGlyphs) {
    $pg = [string]$g.Atlas
    if (-not $newRectsByPage.ContainsKey($pg)) { $newRectsByPage[$pg] = New-Object System.Collections.Generic.List[object] }
    $newRectsByPage[$pg].Add($g)
  }
  $oldRectsByPage = @{}
  foreach ($g in $oldGlyphs) {
    $pg = [string]$g.Atlas
    if (-not $oldRectsByPage.ContainsKey($pg)) { $oldRectsByPage[$pg] = New-Object System.Collections.Generic.List[object] }
    $oldRectsByPage[$pg].Add($g)
  }

  foreach ($pg in $newRectsByPage.Keys) {
    if (-not $oldRectsByPage.ContainsKey($pg)) { continue }
    foreach ($nr in $newRectsByPage[$pg]) {
      foreach ($or in $oldRectsByPage[$pg]) {
        if (Test-Overlap $nr $or) {
          return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('new glyph rect overlaps existing glyph on page ' + $pg) } }
        }
      }
    }
  }

  $pagesAdded = 0
  $pagesChanged = 0
  for ($page = 0; $page -lt $newNonNull.Count; $page++) {
    $fid = $newNonNull[$page]
    $newBlock = $newTex[$fid]
    if ($null -eq $newBlock) {
      return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = ('atlas page block missing: ' + $fid) } }
    }
    $meta = Get-AtlasMeta $newBlock.Body
    if ($null -eq $meta) {
      return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = 'atlas meta parse failed' } }
    }
    $rects = New-Object System.Collections.Generic.List[object]
    if ($newRectsByPage.ContainsKey([string]$page)) { $rects = $newRectsByPage[[string]$page] }
    if (-not (Test-RectsWithin $meta.Width $meta.Height $rects)) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('glyph rect outside atlas page ' + $page) } }
    }

    if (-not $oldTex.ContainsKey($fid)) { $pagesAdded++; continue }
    if ($oldTex[$fid].Body -eq $newBlock.Body) { continue }
    $pagesChanged++
    if ($page -ge $oldNonNull.Count -and $rects.Count -eq 0) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = 'atlas page changed without new glyphs' } }
    }
    if ($meta.Format -ne 1) {
      return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = ('unsupported atlas texture format ' + $meta.Format) } }
    }
    $oldMeta = Get-AtlasMeta $oldTex[$fid].Body
    if ($null -eq $oldMeta -or $oldMeta.Width -ne $meta.Width -or $oldMeta.Height -ne $meta.Height -or $oldMeta.Format -ne $meta.Format) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('atlas page shape changed: ' + $page) } }
    }
    $a = ConvertFrom-HexBytes $oldMeta.Hex
    $b = ConvertFrom-HexBytes $meta.Hex
    if ($null -eq $a -or $null -eq $b -or $a.Length -ne ($meta.Width * $meta.Height) -or $b.Length -ne $a.Length) {
      return [pscustomobject]@{ Verdict = 'NEEDS_REVIEW'; Detail = @{ reason = ('atlas pixel decode mismatch: ' + $page) } }
    }

    $mask = New-Object 'byte[]' $a.Length
    $margin = 12
    foreach ($r in $rects) {
      $x0 = [Math]::Max(0, $r.X - $margin); $x1 = [Math]::Min($meta.Width - 1, $r.X + $r.W + $margin)
      $y0 = [Math]::Max(0, $r.Y - $margin); $y1 = [Math]::Min($meta.Height - 1, $r.Y + $r.H + $margin)
      for ($yy = $y0; $yy -le $y1; $yy++) {
        $rowStart = $yy * $meta.Width
        for ($xx = $x0; $xx -le $x1; $xx++) { $mask[$rowStart + $xx] = 1 }
      }
    }
    $outside = 0
    for ($i = 0; $i -lt $a.Length; $i++) {
      if ($a[$i] -ne $b[$i] -and $mask[$i] -eq 0) {
        $outside++
        if ($outside -gt 64) { break }
      }
    }
    if ($outside -gt 0) {
      return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('atlas pixels changed outside new glyph rects on page ' + $page); outsidePixels = $outside } }
    }
    foreach ($r in $rects) {
      $nonzero = $false
      for ($yy = $r.Y; $yy -lt ($r.Y + $r.H) -and -not $nonzero; $yy++) {
        $rowStart = $yy * $meta.Width
        for ($xx = $r.X; $xx -lt ($r.X + $r.W); $xx++) {
          if ($b[$rowStart + $xx] -ne 0) { $nonzero = $true; break }
        }
      }
      if (-not $nonzero) {
        return [pscustomobject]@{ Verdict = 'INCIDENT'; Detail = @{ reason = ('new glyph rect is empty on page ' + $page) } }
      }
    }
  }

  $detail['pagesAdded'] = $pagesAdded
  $detail['pagesChanged'] = $pagesChanged
  $added = @()
  foreach ($c in $addedChars) {
    $added += [pscustomobject]@{ unicode = $c.Unicode; char = [string][char]$c.Unicode; glyphIndex = $c.GlyphIndex }
  }
  $detail['addedCharacters'] = $added
  $detail['addedGlyphCount'] = $addedGlyphs.Count
  $detail['charDelta'] = $newChars.Count - $oldChars.Count
  return [pscustomobject]@{ Verdict = 'LEGITIMATE_GROWTH'; Detail = $detail }
}
