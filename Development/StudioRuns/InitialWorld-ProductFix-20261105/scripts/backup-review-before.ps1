$ErrorActionPreference = 'Stop'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$runDir = Join-Path $repo 'Development\StudioRuns\InitialWorld-ProductFix-20261105'
$bakDir = Join-Path $runDir 'review-before'
New-Item -ItemType Directory -Force -Path $bakDir | Out-Null
$files = @(
  'UnityProject\RRPG1.0game\Assets\Scripts\Village\Runtime\ProductHomeSnapshot.cs',
  'UnityProject\RRPG1.0game\Assets\Scripts\Village\ProductHome\ProductHomeReadAdapter.cs',
  'UnityProject\RRPG1.0game\Assets\Scripts\Village\ProductHome\ProductHomeWorldHost.cs',
  'UnityProject\RRPG1.0game\Assets\Scripts\Village\ProductHome\ManagementView.cs',
  'UnityProject\RRPG1.0game\Assets\Scripts\Village\ProductHome\HomeProductShell.cs',
  'UnityProject\RRPG1.0game\Assets\Scripts\UI\MainMenuUI.cs',
  'UnityProject\RRPG1.0game\Assets\Tests\PlayMode\Village\VillageVisualPrototypePlayTests.cs'
)
$rows = @()
foreach ($rel in $files) {
  $src = Join-Path $repo $rel
  $name = Split-Path $src -Leaf
  $dst = Join-Path $bakDir $name
  Copy-Item -LiteralPath $src -Destination $dst -Force
  $h = (Get-FileHash -LiteralPath $dst -Algorithm SHA256).Hash.ToLower()
  $srcH = (Get-FileHash -LiteralPath $src -Algorithm SHA256).Hash.ToLower()
  if ($h -ne $srcH) { throw ('BACKUP_MISMATCH ' + $rel) }
  $rows += [pscustomobject]@{ path = $rel; backup = ('review-before/' + $name); bytes = (Get-Item $dst).Length; sha256 = $h }
}
$manifest = [ordered]@{ schema = 'rrpg-review-before/1'; createdUtc = (Get-Date).ToUniversalTime().ToString('o'); files = $rows }
[IO.File]::WriteAllText((Join-Path $bakDir 'manifest.json'), ($manifest | ConvertTo-Json -Depth 6), (New-Object System.Text.UTF8Encoding($false)))
$rows | ForEach-Object { $_.sha256.Substring(0,12) + '  ' + $_.bytes + '  ' + $_.backup }
'BACKUP_OK count=' + $rows.Count
