$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$xl = New-Object -ComObject Excel.Application
$xl.Visible = $false; $xl.DisplayAlerts = $false
try {
  $wb = $xl.Workbooks.Open((Join-Path $repo 'Development\Config\RRPG_VillagePresentation.xlsx'))
  $ws = $wb.Worksheets.Item('VillagePresentation')
  $last = $ws.Cells($ws.Rows.Count, 1).End(-4162).Row
  $map = @{}
  for ($c = 1; $c -le 8; $c++) { $h = [string]$ws.Cells(1, $c).Text; if ($h) { $map[$h] = $c } }
  $existing = @{}
  for ($r = 5; $r -le $last; $r++) { $k = [string]$ws.Cells($r, $map['PresentationKey']).Text; if ($k) { $existing[$k] = $r } }
  $r = $last + 1
  foreach ($n in @(@{k='village.place.fixed.Tavern';a='va_village_tavern_a'}, @{k='village.place.r2.fixed.Tavern';a='va_village_r2_workshop'})) {
    if ($existing.ContainsKey($n.k)) { continue }
    $ws.Cells($r, $map['PresentationKey']).Value2 = $n.k
    $ws.Cells($r, $map['VisualAssetId']).Value2 = $n.a
    Write-Output ('  added ' + $n.k + ' row ' + $r)
    $r++
  }
  $wb.Save(); $wb.Close($false)
  Write-Output ('LAST_AFTER=' + $last + ' -> ' + ($r-1))

  $wb2 = $xl.Workbooks.Open((Join-Path $repo 'Development\Config\RRPG_VisualAssets.xlsx'))
  $ws2 = $wb2.Worksheets.Item('VisualAsset')
  $last2 = $ws2.Cells($ws2.Rows.Count, 1).End(-4162).Row
  $map2 = @{}
  for ($c = 1; $c -le 8; $c++) { $h = [string]$ws2.Cells(1, $c).Text; if ($h) { $map2[$h] = $c } }
  $has = $false
  for ($r = 5; $r -le $last2; $r++) { if ([string]$ws2.Cells($r, $map2['VisualAssetId']).Text -eq 'va_village_tavern_a') { $has = $true } }
  if (-not $has) {
    $ws2.Cells($last2+1, $map2['VisualAssetId']).Value2 = 'va_village_tavern_a'
    $ws2.Cells($last2+1, $map2['SourceFile']).Value2 = 'Village/A-Tier/tavern-a.png'
    $ws2.Cells($last2+1, $map2['ImportPreset']).Value2 = 'sprite_point_100'
    Write-Output ('  VisualAsset added at ' + ($last2+1))
  } else { Write-Output '  VisualAsset already has tavern' }
  $wb2.Save(); $wb2.Close($false)
}
finally {
  $xl.Quit(); [System.Runtime.InteropServices.Marshal]::ReleaseComObject($xl) | Out-Null
  [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}
$exe = Join-Path $repo 'Tools\RRPG.ConfigCompiler\src\RRPG.ConfigCompiler\bin\Release\net10.0\rrpg-config.exe'
$out = & $exe build --root $repo 2>&1
$code = $LASTEXITCODE
$out | Select-String -Pattern 'result=|errors=|updated .*Village|updated .*VisualAsset' | ForEach-Object { $_.Line }
'BUILD_EXIT=' + $code
if ($code -ne 0) { exit 1 }
$v = & $exe validate --root $repo 2>&1
$vc = $LASTEXITCODE
$v | Select-String -Pattern 'PASS|errors=|ERROR' | Select-Object -First 6 | ForEach-Object { $_.Line }
'VALIDATE_EXIT=' + $vc
Select-String -LiteralPath (Join-Path $repo 'Development\Generated\StaticConfig\VillagePresentation.json') -Pattern 'fixed.Tavern' | ForEach-Object { '  mirror: ' + $_.Line.Trim() }
