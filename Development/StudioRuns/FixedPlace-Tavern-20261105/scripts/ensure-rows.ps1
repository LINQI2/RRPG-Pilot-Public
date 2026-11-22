$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$path = Join-Path $repo 'Development\Config\RRPG_VillagePresentation.xlsx'
$need = @(
  @{ k = 'village.building.kitchen.basic'; a = 'va_village_kitchen_a' },
  @{ k = 'village.building.house.basic';   a = 'va_village_house_a' },
  @{ k = 'village.person.onsite';          a = 'va_village_person_a' },
  @{ k = 'village.person.away';            a = 'va_village_person_a' },
  @{ k = 'village.person.traveling';       a = 'va_village_person_a' },
  @{ k = 'village.place.fixed.Tavern';     a = 'va_village_tavern_a' },
  @{ k = 'village.place.r2.fixed.Tavern';  a = 'va_village_r2_workshop' }
)
function Run-Pass {
  $xl = New-Object -ComObject Excel.Application
  $xl.Visible = $false; $xl.DisplayAlerts = $false
  try {
    $wb = $xl.Workbooks.Open($path)
    $ws = $wb.Worksheets.Item('VillagePresentation')
    $map = @{}
    for ($c = 1; $c -le 8; $c++) { $h = [string]$ws.Cells(1, $c).Text; if ($h) { $map[$h] = $c } }
    $last = $ws.Cells($ws.Rows.Count, 1).End(-4162).Row
    $have = @{}
    for ($r = 5; $r -le $last; $r++) { $k = [string]$ws.Cells($r, $map['PresentationKey']).Text; if ($k) { $have[$k] = $r } }
    $r2 = $last + 1
    foreach ($n in $need) {
      if ($have.ContainsKey($n.k)) { continue }
      $ws.Cells($r2, $map['PresentationKey']).Value2 = $n.k
      $ws.Cells($r2, $map['VisualAssetId']).Value2 = $n.a
      Write-Output ('  appended ' + $n.k + ' at ' + $r2)
      $r2++
    }
    $wb.Save()
    $wb.Close($false)
  }
  finally {
    $xl.Quit(); [System.Runtime.InteropServices.Marshal]::ReleaseComObject($xl) | Out-Null
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
  }
}
function Count-Keys {
  $xl = New-Object -ComObject Excel.Application
  $xl.Visible = $false; $xl.DisplayAlerts = $false
  try {
    $wb = $xl.Workbooks.Open($path)
    $ws = $wb.Worksheets.Item('VillagePresentation')
    $last = $ws.Cells($ws.Rows.Count, 1).End(-4162).Row
    $keys = @()
    for ($r = 5; $r -le $last; $r++) { $k = [string]$ws.Cells($r, 1).Text; if ($k) { $keys += $k } }
    $wb.Close($false)
    return $keys
  }
  finally {
    $xl.Quit(); [System.Runtime.InteropServices.Marshal]::ReleaseComObject($xl) | Out-Null
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
  }
}
Run-Pass
Start-Sleep -Seconds 1
$keys = Count-Keys
Write-Output ('KEY_COUNT=' + $keys.Count)
foreach ($n in $need) { Write-Output ('  present ' + $n.k + ' = ' + ($keys -contains $n.k)) }
$missing = $need | Where-Object { $keys -notcontains $_.k }
if ($missing.Count -gt 0) { Write-Output 'PERSIST_FAILED'; exit 2 }

$exe = Join-Path $repo 'Tools\RRPG.ConfigCompiler\src\RRPG.ConfigCompiler\bin\Release\net10.0\rrpg-config.exe'
$out = & $exe build --root $repo 2>&1
$code = $LASTEXITCODE
$out | Select-String -Pattern 'result=|errors=' | ForEach-Object { $_.Line }
'BUILD_EXIT=' + $code
if ($code -ne 0) { exit 1 }
$v = & $exe validate --root $repo 2>&1
$vc = $LASTEXITCODE
$v | Select-String -Pattern 'PASS|errors=|ERROR' | Select-Object -First 5 | ForEach-Object { $_.Line }
'VALIDATE_EXIT=' + $vc
Select-String -LiteralPath (Join-Path $repo 'Development\Generated\StaticConfig\VillagePresentation.json') -Pattern 'fixed.Tavern' | ForEach-Object { '  mirror: ' + $_.Line.Trim() }
