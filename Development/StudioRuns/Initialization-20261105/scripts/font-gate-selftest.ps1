# Gate self-test: only temporary copies; never touches the real font or baseline.
param([string]$RepoRoot = 'D:\AI_Agent_Lab\rrpg_pilot\repo')

$ErrorActionPreference = 'Stop'
. (Join-Path $RepoRoot 'Development\StudioRuns\Initialization-20261105\scripts\font-guard-core.ps1')

$runDir    = Join-Path $RepoRoot 'Development\StudioRuns\Initialization-20261105'
$current   = Join-Path $RepoRoot 'UnityProject\RRPG1.0game\Assets\Art\Fonts\SourceHanSansCN\RRPG_SourceHanSansCN_Regular_TMP.asset'
$sample648 = Join-Path $runDir 'review-backups\font-after-dsh.asset'
$sample654 = Join-Path $runDir 'font-protection\baseline\RRPG_SourceHanSansCN_Regular_TMP.asset'

$tmp = Join-Path $env:TEMP ('font-gate-selftest-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $tmp | Out-Null
$results = New-Object System.Collections.Generic.List[object]
function Record([string]$name, [string]$expect, [string]$actual, [string]$note) {
  $ok = ($expect -eq $actual)
  $results.Add([pscustomobject]@{ Case = $name; Expect = $expect; Actual = $actual; Pass = $ok; Note = $note })
}

try {
  $v1 = Get-FontDriftVerdict $sample648 $sample654
  Record 'growth-648-to-654' 'LEGITIMATE_GROWTH' $v1.Verdict (($v1.Detail['addedCharacters'] | ForEach-Object { $_.char }) -join '')
  $v2 = Get-FontDriftVerdict $sample654 $current
  Record 'growth-654-to-656' 'LEGITIMATE_GROWTH' $v2.Verdict (($v2.Detail['addedCharacters'] | ForEach-Object { $_.char }) -join '')

  $baseText = [IO.File]::ReadAllText($current)

  $t = [regex]::Replace($baseText, '(?m)^\s+- m_ElementType: 1\s*?\r?\n\s+m_Unicode: \d+\s*?\r?\n\s+m_GlyphIndex: \d+\s*?\r?\n\s+m_Scale: 1\s*?\r?\n', '', 1)
  $p = Join-Path $tmp 'bad-char-removed.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-remove-old-character' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $t = [regex]::Replace($baseText, '(?ms)^\s+- m_Index: \d+\s*?\r?\n.*?(?=^\s+- m_Index:|\z)', '', 1)
  $p = Join-Path $tmp 'bad-glyph-removed.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-remove-old-glyph' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $m = [regex]::Match($baseText, '(?ms)^\s+- m_Index: \d+\s*?\r?\n.*?m_GlyphRect:\s*?\r?\n\s+m_X: (\d+)')
  if (-not $m.Success) { throw 'glyph rect regex must match' }
  $pos = $m.Groups[1].Index
  $t = $baseText.Substring(0, $pos) + '999' + $baseText.Substring($pos + $m.Groups[1].Length)
  $p = Join-Path $tmp 'bad-glyph-modified.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-modify-old-glyph' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $m = [regex]::Match($baseText, '(?m)^  _typelessdata: ([0-9a-fA-F]{2})')
  $pos = $m.Groups[1].Index
  $orig = $m.Groups[1].Value
  if ($orig -eq '00') { $flip = 'ff' } else { $flip = '00' }
  $t = $baseText.Substring(0, $pos) + $flip + $baseText.Substring($pos + 2)
  $p = Join-Path $tmp 'bad-atlas-corrupted.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-corrupt-old-atlas-region' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $t = [regex]::Replace($baseText, '(?m)^(  m_Shader: \{fileID: \d+, guid: )([0-9a-f]{32})', { param($mm) $mm.Groups[1].Value + 'ffffffffffffffffffffffffffffffff' }, 1)
  $p = Join-Path $tmp 'bad-material.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-break-material-ref' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $t = [regex]::Replace($baseText, '(?ms)(^  m_AtlasTextures:\s*?\r?\n(?:  - \{fileID: -?\d+\}\s*?\r?\n)+)', { param($mm)
    $block = $mm.Groups[1].Value
    $lines = $block -split "`n"
    $idx = -1
    for ($i = 0; $i -lt $lines.Count; $i++) { if ($lines[$i] -match '\{fileID: ([1-9]\d*|-?[1-9]\d*)\}') { $idx = $i } }
    if ($idx -ge 0) { $lines[$idx] = '  - {fileID: 0}' }
    return ($lines -join "`n")
  }, 1)
  $p = Join-Path $tmp 'bad-atlas-page-removed.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-remove-atlas-page-ref' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $t = [regex]::Replace($baseText, '(?m)^(\s+m_PointSize: )\d+', '${1}80', 1)
  $p = Join-Path $tmp 'bad-faceinfo.asset'; [IO.File]::WriteAllText($p, $t)
  $v = Get-FontDriftVerdict $sample654 $p
  Record 'bad-modify-faceinfo' 'INCIDENT' $v.Verdict $v.Detail['reason']

  $failed = @($results | Where-Object { -not $_.Pass })
  $results | ForEach-Object { "{0,-32} expect={1,-18} actual={2,-18} {3} {4}" -f $_.Case, $_.Expect, $_.Actual, $(if ($_.Pass) { 'OK' } else { 'FAIL' }), $_.Note }
  ""
  "TOTAL=$($results.Count) PASSED=$(@($results | Where-Object { $_.Pass }).Count) FAILED=$($failed.Count)"
  if ($failed.Count -gt 0) { exit 1 } else { exit 0 }
}
finally {
  Remove-Item -LiteralPath $tmp -Recurse -Force -ErrorAction SilentlyContinue
}
