$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$runDir = Join-Path $repo 'Development\StudioRuns\Initialization-20261105'
$artDir = Join-Path $runDir 'artifacts'
$bakDir = Join-Path $runDir 'before-a-tier'
$srcDir = Join-Path $repo 'Development\SourceAssets\Village\A-Tier'
$logPath = Join-Path $artDir 'a-tier-build.log'
New-Item -ItemType Directory -Force -Path $artDir, $bakDir, $srcDir | Out-Null
if (Test-Path -LiteralPath $logPath) { Remove-Item -LiteralPath $logPath -Force }
function Log([string]$m) { $line = ('[' + (Get-Date -Format 'HH:mm:ss') + '] ' + $m); Write-Output $line; Add-Content -LiteralPath $logPath -Value $line -Encoding UTF8 }

try {
  # ---------- 1. A 档白模图 ----------
  Add-Type -AssemblyName System.Drawing
  function Save-Png($bmp, $path) { $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png); $bmp.Dispose() }
  function New-Canvas {
    $bmp = New-Object System.Drawing.Bitmap(300, 200)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::Transparent)
    return @($bmp, $g)
  }
  function Brush($r, $g, $b) { return (New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, $r, $g, $b))) }
  function Poly($g, $brush, $pts) {
    $arr = New-Object 'System.Drawing.Point[]' $pts.Count
    for ($i = 0; $i -lt $pts.Count; $i++) { $arr[$i] = New-Object System.Drawing.Point($pts[$i][0], $pts[$i][1]) }
    $g.FillPolygon($brush, $arr)
  }

  # Kitchen: ochre roof + ivory wall + chimney + dark doorway
  $c = New-Canvas; $bmp = $c[0]; $g = $c[1]
  $wall = Brush 238 230 214; $roof = Brush 196 138 74; $chim = Brush 160 108 56; $door = Brush 70 52 40
  $g.FillRectangle($chim, 212, 52, 26, 46)
  $g.FillRectangle($wall, 62, 96, 176, 84)
  Poly $g $roof @(@(46, 96), @(150, 44), @(254, 96))
  $g.FillRectangle($door, 132, 126, 38, 54)
  Save-Png $bmp (Join-Path $srcDir 'kitchen-a.png')
  Log 'PNG kitchen-a.png written'

  # House: terracotta roof + ivory wall + two windows + door
  $c = New-Canvas; $bmp = $c[0]; $g = $c[1]
  $wall2 = Brush 240 233 220; $roof2 = Brush 178 96 78; $win = Brush 110 150 180; $door2 = Brush 78 58 44
  $g.FillRectangle($wall2, 58, 92, 184, 88)
  Poly $g $roof2 @(@(40, 92), @(150, 40), @(260, 92))
  $g.FillRectangle($win, 84, 116, 40, 34)
  $g.FillRectangle($win, 176, 116, 40, 34)
  $g.FillRectangle($door2, 136, 132, 28, 48)
  Save-Png $bmp (Join-Path $srcDir 'house-a.png')
  Log 'PNG house-a.png written'

  # Person: head + tunic torso + two legs
  $c = New-Canvas; $bmp = $c[0]; $g = $c[1]
  $skin = Brush 236 220 200; $tunic = Brush 88 116 168; $leg = Brush 62 74 96
  $g.FillRectangle($leg, 133, 140, 14, 40)
  $g.FillRectangle($leg, 153, 140, 14, 40)
  Poly $g $tunic @(@(118, 82), @(182, 82), @(192, 146), @(108, 146))
  $g.FillEllipse($skin, 132, 40, 36, 36)
  Save-Png $bmp (Join-Path $srcDir 'person-a.png')
  Log 'PNG person-a.png written'

  # ---------- 2. 备份配置表 ----------
  foreach ($n in @('RRPG_VisualAssets.xlsx', 'RRPG_VillagePresentation.xlsx')) {
    Copy-Item -LiteralPath (Join-Path $repo ('Development\Config\' + $n)) -Destination (Join-Path $bakDir $n) -Force
  }
  Log 'workbook backups copied'

  # ---------- 3. Excel 编辑 ----------
  $xl = New-Object -ComObject Excel.Application
  $xl.Visible = $false
  $xl.DisplayAlerts = $false
  try {
    # 3a VisualAsset
    $wb = $xl.Workbooks.Open((Join-Path $repo 'Development\Config\RRPG_VisualAssets.xlsx'))
    $ws = $wb.Worksheets.Item('VisualAsset')
    $last = $ws.Cells($ws.Rows.Count, 1).End(-4162).Row
    $map = @{}
    for ($c2 = 1; $c2 -le 10; $c2++) { $h = [string]$ws.Cells(1, $c2).Text; if ($h) { $map[$h] = $c2 } }
    Log ('VisualAsset last row=' + $last + ' cols=' + (($map.Keys | Sort-Object) -join ','))
    $rows = @(
      @{ id = 'va_village_kitchen_a'; file = 'Village/A-Tier/kitchen-a.png' },
      @{ id = 'va_village_house_a'; file = 'Village/A-Tier/house-a.png' },
      @{ id = 'va_village_person_a'; file = 'Village/A-Tier/person-a.png' }
    )
    $r = $last + 1
    foreach ($row in $rows) {
      $ws.Cells($r, $map['VisualAssetId']).Value2 = $row.id
      $ws.Cells($r, $map['SourceFile']).Value2 = $row.file
      $ws.Cells($r, $map['ImportPreset']).Value2 = 'sprite_point_100'
      Log ('VisualAsset append row ' + $r + ' = ' + $row.id)
      $r++
    }
    $wb.Save()
    $wb.Close($false)
    Log 'VisualAsset saved'

    # 3b VillagePresentation + BuildingPresentationBinding
    $wb2 = $xl.Workbooks.Open((Join-Path $repo 'Development\Config\RRPG_VillagePresentation.xlsx'))
    $ws2 = $wb2.Worksheets.Item('VillagePresentation')
    $last2 = $ws2.Cells($ws2.Rows.Count, 1).End(-4162).Row
    $map2 = @{}
    for ($c3 = 1; $c3 -le 10; $c3++) { $h2 = [string]$ws2.Cells(1, $c3).Text; if ($h2) { $map2[$h2] = $c3 } }
    Log ('VillagePresentation last row=' + $last2 + ' cols=' + (($map2.Keys | Sort-Object) -join ','))
    $vp = @(
      @{ key = 'village.building.kitchen.basic'; asset = 'va_village_kitchen_a' },
      @{ key = 'village.building.house.basic'; asset = 'va_village_house_a' },
      @{ key = 'village.person.onsite'; asset = 'va_village_person_a' },
      @{ key = 'village.person.away'; asset = 'va_village_person_a' },
      @{ key = 'village.person.traveling'; asset = 'va_village_person_a' }
    )
    $r2 = $last2 + 1
    foreach ($row in $vp) {
      $ws2.Cells($r2, $map2['PresentationKey']).Value2 = $row.key
      $ws2.Cells($r2, $map2['VisualAssetId']).Value2 = $row.asset
      Log ('VillagePresentation append row ' + $r2 + ' = ' + $row.key + ' -> ' + $row.asset)
      $r2++
    }

    $ws3 = $wb2.Worksheets.Item('BuildingPresentationBinding')
    $last3 = $ws3.Cells($ws3.Rows.Count, 1).End(-4162).Row
    $map3 = @{}
    for ($c4 = 1; $c4 -le 10; $c4++) { $h3 = [string]$ws3.Cells(1, $c4).Text; if ($h3) { $map3[$h3] = $c4 } }
    Log ('BuildingPresentationBinding last row=' + $last3 + ' cols=' + (($map3.Keys | Sort-Object) -join ','))
    for ($i = 5; $i -le $last3; $i++) {
      $defId = [string]$ws3.Cells($i, $map3['BuildingDefinitionId']).Text
      $life = [string]$ws3.Cells($i, $map3['Lifecycle']).Text
      if ($defId -eq 'building_kitchen') { $ws3.Cells($i, $map3['PresentationKey']).Value2 = 'village.building.kitchen.basic'; Log ('binding row ' + $i + ' kitchen/' + $life + ' -> kitchen.basic') }
      elseif ($defId -eq 'building_house') { $ws3.Cells($i, $map3['PresentationKey']).Value2 = 'village.building.house.basic'; Log ('binding row ' + $i + ' house/' + $life + ' -> house.basic') }
    }
    $wb2.Save()
    $wb2.Close($false)
    Log 'VillagePresentation saved'
  }
  finally {
    $xl.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($xl) | Out-Null
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
    Log 'excel quit'
  }

  # ---------- 4. Compiler build ----------
  $exe = Join-Path $repo 'Tools\RRPG.ConfigCompiler\src\RRPG.ConfigCompiler\bin\Release\net10.0\rrpg-config.exe'
  Log ('compiler exists=' + (Test-Path -LiteralPath $exe))
  $out = & $exe build --root $repo 2>&1
  $code = $LASTEXITCODE
  foreach ($l in $out) { Log ('  compiler| ' + $l) }
  Log ('compiler exit=' + $code)
  if ($code -ne 0) { throw ('compiler build failed exit=' + $code) }

  $val = & $exe validate --root $repo 2>&1
  $vcode = $LASTEXITCODE
  foreach ($l in $val) { Log ('  validate| ' + $l) }
  Log ('validate exit=' + $vcode)
  Log 'DONE'
  exit 0
}
catch {
  Log ('FATAL ' + $_.Exception.GetType().Name + ': ' + $_.Exception.Message)
  Log ('AT ' + $_.InvocationInfo.PositionMessage)
  exit 1
}
