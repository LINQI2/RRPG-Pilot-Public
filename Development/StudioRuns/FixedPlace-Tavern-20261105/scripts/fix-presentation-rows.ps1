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
  Write-Output ('LAST=' + $last + ' KEYS=' + (($existing.Keys | Sort-Object) -join ','))
  $need = @(
    @{ k = 'village.building.kitchen.basic'; a = 'va_village_kitchen_a' },
    @{ k = 'village.building.house.basic';  a = 'va_village_house_a' },
    @{ k = 'village.person.onsite';         a = 'va_village_person_a' },
    @{ k = 'village.person.away';           a = 'va_village_person_a' },
    @{ k = 'village.person.traveling';      a = 'va_village_person_a' }
  )
  $r = $last + 1
  foreach ($n in $need) {
    if ($existing.ContainsKey($n.k)) { continue }
    $ws.Cells($r, $map['PresentationKey']).Value2 = $n.k
    $ws.Cells($r, $map['VisualAssetId']).Value2 = $n.a
    Write-Output ('  restored ' + $n.k + ' at row ' + $r)
    $r++
  }
  $wb.Save(); $wb.Close($false)
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
