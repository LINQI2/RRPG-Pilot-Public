$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$runDir = Join-Path $repo 'Development\StudioRuns\FixedPlace-Tavern-20261105'
$art = Join-Path $runDir 'artifacts'
$srcDir = Join-Path $repo 'Development\SourceAssets\Village\A-Tier'
$bak = Join-Path $runDir 'before'
$log = Join-Path $art 'tavern-build.log'
New-Item -ItemType Directory -Force -Path $art, $srcDir, $bak | Out-Null
if (Test-Path $log) { Remove-Item $log -Force }
function Log([string]$m) { $line = ('[' + (Get-Date -Format 'HH:mm:ss') + '] ' + $m); Write-Output $line; Add-Content -LiteralPath $log -Value $line -Encoding UTF8 }

try {
  # ---- 1. A 档酒馆白模：小屋 + 门 + 悬挂招牌 ----
  Add-Type -AssemblyName System.Drawing
  $bmp = New-Object System.Drawing.Bitmap(300, 200)
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $g.Clear([System.Drawing.Color]::Transparent)
  $wall = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 226, 205, 170))
  $roof = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 152, 78, 62))
  $door = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 62, 44, 34))
  $win  = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 108, 146, 172))
  $sign = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 214, 176, 74))
  $pole = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 78, 58, 40))
  $pen  = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(255, 40, 30, 24), 3)
  # body
  $g.FillRectangle($wall, 62, 96, 176, 84)
  # roof (wide, overhanging)
  $pts = New-Object 'System.Drawing.Point[]' 3
  $pts[0] = New-Object System.Drawing.Point(40, 96)
  $pts[1] = New-Object System.Drawing.Point(150, 42)
  $pts[2] = New-Object System.Drawing.Point(260, 96)
  $g.FillPolygon($roof, $pts)
  # door + window
  $g.FillRectangle($door, 126, 128, 42, 52)
  $g.FillRectangle($win, 78, 120, 36, 30)
  $g.FillRectangle($win, 186, 120, 36, 30)
  # hanging sign post on the right
  $g.FillRectangle($pole, 246, 96, 8, 62)
  $g.FillRectangle($pole, 246, 96, 40, 7)
  $g.FillRectangle($sign, 262, 104, 34, 26)
  $g.DrawRectangle($pen, 262, 104, 34, 26)
  $bmp.Save((Join-Path $srcDir 'tavern-a.png'), [System.Drawing.Imaging.ImageFormat]::Png)
  $bmp.Dispose()
  Log ('PNG tavern-a.png bytes=' + (Get-Item (Join-Path $srcDir 'tavern-a.png')).Length)

  # ---- 2. 备份并编辑两本工作簿 ----
  foreach ($n in @('RRPG_VisualAssets.xlsx','RRPG_VillagePresentation.xlsx')) {
    Copy-Item -LiteralPath (Join-Path $repo ('Development\Config\' + $n)) -Destination (Join-Path $bak $n) -Force
  }
  Log 'workbook backups copied'
  $xl = New-Object -ComObject Excel.Application
  $xl.Visible = $false; $xl.DisplayAlerts = $false
  try {
    $wb = $xl.Workbooks.Open((Join-Path $repo 'Development\Config\RRPG_VisualAssets.xlsx'))
    $ws = $wb.Worksheets.Item('VisualAsset')
    $last = $ws.Cells($ws.Rows.Count, 1).End(-4162).Row
    $map = @{}
    for ($c = 1; $c -le 8; $c++) { $h = [string]$ws.Cells(1, $c).Text; if ($h) { $map[$h] = $c } }
    $r = $last + 1
    $ws.Cells($r, $map['VisualAssetId']).Value2 = 'va_village_tavern_a'
    $ws.Cells($r, $map['SourceFile']).Value2 = 'Village/A-Tier/tavern-a.png'
    $ws.Cells($r, $map['ImportPreset']).Value2 = 'sprite_point_100'
    $wb.Save(); $wb.Close($false)
    Log ('VisualAsset append row ' + $r)

    $wb2 = $xl.Workbooks.Open((Join-Path $repo 'Development\Config\RRPG_VillagePresentation.xlsx'))
    $ws2 = $wb2.Worksheets.Item('VillagePresentation')
    $last2 = $ws2.Cells($ws2.Rows.Count, 1).End(-4162).Row
    $map2 = @{}
    for ($c = 1; $c -le 8; $c++) { $h = [string]$ws2.Cells(1, $c).Text; if ($h) { $map2[$h] = $c } }
    $r2 = $last2 + 1
    $ws2.Cells($r2, $map2['PresentationKey']).Value2 = 'village.place.fixed.Tavern'
    $ws2.Cells($r2, $map2['VisualAssetId']).Value2 = 'va_village_tavern_a'
    $r2++
    $ws2.Cells($r2, $map2['PresentationKey']).Value2 = 'village.place.r2.fixed.Tavern'
    $ws2.Cells($r2, $map2['VisualAssetId']).Value2 = 'va_village_r2_workshop'
    $wb2.Save(); $wb2.Close($false)
    Log ('VillagePresentation append rows ' + ($r2-1) + '..' + $r2)
  }
  finally {
    $xl.Quit(); [System.Runtime.InteropServices.Marshal]::ReleaseComObject($xl) | Out-Null
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
  }

  # ---- 3. Compiler build + validate ----
  $exe = Join-Path $repo 'Tools\RRPG.ConfigCompiler\src\RRPG.ConfigCompiler\bin\Release\net10.0\rrpg-config.exe'
  $out = & $exe build --root $repo 2>&1
  $code = $LASTEXITCODE
  foreach ($l in $out) { if ($l -match 'result=PASS|errors=|updated .*Village|updated .*VisualAsset') { Log ('  build| ' + $l) } }
  Log ('build exit=' + $code)
  if ($code -ne 0) { throw 'compiler build failed' }
  $val = & $exe validate --root $repo 2>&1
  $vcode = $LASTEXITCODE
  foreach ($l in $val) { if ($l -match 'PASS|errors=') { Log ('  validate| ' + $l) } }
  Log ('validate exit=' + $vcode)
  Log 'DONE'
  exit 0
}
catch {
  Log ('FATAL ' + $_.Exception.GetType().Name + ': ' + $_.Exception.Message)
  exit 1
}
