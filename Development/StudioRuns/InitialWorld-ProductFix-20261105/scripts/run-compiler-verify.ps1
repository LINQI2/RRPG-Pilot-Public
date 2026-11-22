$ErrorActionPreference = 'Continue'
$repo = 'D:\AI_Agent_Lab\rrpg_pilot\repo'
$runDir = Join-Path $repo 'Development\StudioRuns\InitialWorld-ProductFix-20261105'
$art = Join-Path $runDir 'artifacts'
New-Item -ItemType Directory -Force -Path $art | Out-Null
$log = Join-Path $art 'compiler-verify.log'
"=== compiler verify $(Get-Date -Format o) ===" | Out-File -FilePath $log -Encoding utf8

$exe = Join-Path $repo 'Tools\RRPG.ConfigCompiler\src\RRPG.ConfigCompiler\bin\Release\net10.0\rrpg-config.exe'
$out = & $exe validate --root $repo 2>&1
$code = $LASTEXITCODE
$out | Out-File -FilePath $log -Append -Encoding utf8
"VALIDATE_EXIT=$code" | Out-File -FilePath $log -Append -Encoding utf8

$proj = Join-Path $repo 'Tools\RRPG.ConfigCompiler\tests\RRPG.ConfigCompiler.Tests\RRPG.ConfigCompiler.Tests.csproj'
$out2 = & dotnet test $proj --no-restore -v q 2>&1
$code2 = $LASTEXITCODE
$out2 | Out-File -FilePath $log -Append -Encoding utf8
"DOTNET_TEST_EXIT=$code2" | Out-File -FilePath $log -Append -Encoding utf8

Get-Content -LiteralPath $log | Select-Object -Last 14 | ForEach-Object { $_ }
"VALIDATE_EXIT=$code" 
"DOTNET_TEST_EXIT=$code2"
