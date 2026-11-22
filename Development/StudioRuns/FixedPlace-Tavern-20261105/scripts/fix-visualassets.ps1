$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$path = Join-Path $repo 'Development\Config\RRPG_VisualAssets.xlsx'
$need = @(
  @{ id = 'va_village_kitchen_a'; f = 'Village/A-Tier/kitchen-a.png' },
  @{ id = 'va_village_house_a';   f = 'Village/A-Tier/house-a.png' },
  @{ id = 'va_village_person_a';  f = 'Village/A-Tier/person-a.png' },
  @{ id = 'va_village_tavern_a';  f = 'Village/A-Tier/tavern-a.png' }
)
$xl = New-Object -ComObject Excel.Application
$xl.Visible = $false; $xl.DisplayAlerts = $false
try {
  $wb = $xl.Workbooks.Open($path)
  $ws = $wb.Worksheets.Item('VisualAsset')
  $map = @{}
  for ($c = 1; $c -le 8; $c++) { $h = [string]$ws.Cells(1, $c).Text; if ($h) { $map[$h] = $c } }
  $last = $ws.Cells($ws.Rows.Count, 1).End(-4162).Row
  $have = @{}
  for ($r = 5; $r -le $last; $r++) { $k = [string]$ws.Cells($r, $map['VisualAssetId']).Text; if ($k) { $have[$k] = $r } }
  Write-Output ('LAST=' + $last + ' existing=' + $have.Count)
  $r2 = $last + 1
  foreach ($n in $need) {
    if ($have.ContainsKey($n.id)) { continue }
    $ws.Cells($r2, $map['VisualAssetId']).Value2 = $n.id
    $ws.Cells($r2, $map['SourceFile']).Value2 = $n.f
    $ws.Cells($r2, $map['ImportPreset']).Value2 = 'sprite_point_100'
    Write-Output ('  added ' + $n.id + ' at ' + $r2)
    $r2++
  }
  $wb.Save(); $wb.Close($false)
}
finally {
  $xl.Quit(); [System.Runtime.InteropServices.Marshal]::ReleaseComObject($xl) | Out-Null
  [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}
Start-Sleep -Seconds 1
$exe = Join-Path $repo 'Tools\RRPG.ConfigCompiler\src\RRPG.ConfigCompiler\bin\Release\net10.0\rrpg-config.exe'
$out = & $exe build --root $repo 2>&1
$code = $LASTEXITCODE
$out | Select-String -Pattern 'result=|errors=|updated .*Village|updated .*VisualAsset|ERROR' | Select-Object -First 8 | ForEach-Object { $_.Line }
'BUILD_EXIT=' + $code
if ($code -ne 0) { exit 1 }
$v = & $exe validate --root $repo 2>&1
'VALIDATE_EXIT=' + $LASTEXITCODE
$v | Select-String -Pattern 'PASS|errors=' | Select-Object -Last 2 | ForEach-Object { $_.Line }
Select-String -LiteralPath (Join-Path $repo 'Development\Generated\StaticConfig\VillagePresentation.json') -Pattern 'fixed.Tavern' | ForEach-Object { '  mirror: ' + $_.Line.Trim() }
Select-String -LiteralPath (Join-Path $repo 'Development\Generated\StaticConfig\VisualAsset.json') -Pattern 'tavern_a|kitchen_a' | ForEach-Object { '  asset: ' + $_.Line.Trim() }
