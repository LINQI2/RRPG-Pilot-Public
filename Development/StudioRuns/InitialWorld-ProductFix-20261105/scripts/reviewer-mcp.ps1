param([string]$Tool,[string]$ArgumentsFile,[string]$OutputFile)
$ErrorActionPreference='Stop'
$headers=@{Accept='application/json, text/event-stream'}
$init=@{jsonrpc='2.0';id=1;method='initialize';params=@{protocolVersion='2024-11-05';capabilities=@{};clientInfo=@{name='codex-productfix-review';version='1'}}}|ConvertTo-Json -Depth 8 -Compress
$r=Invoke-WebRequest 'http://localhost:21867/mcp' -Method Post -Headers $headers -ContentType 'application/json' -Body $init -UseBasicParsing
if($r.Headers['Mcp-Session-Id']){$headers['Mcp-Session-Id']=[string](@($r.Headers['Mcp-Session-Id'])[0])}
$args=Get-Content -LiteralPath $ArgumentsFile -Raw -Encoding UTF8|ConvertFrom-Json
$body=@{jsonrpc='2.0';id=2;method='tools/call';params=@{name=$Tool;arguments=$args}}|ConvertTo-Json -Depth 12 -Compress
$response=Invoke-WebRequest 'http://localhost:21867/mcp' -Method Post -Headers $headers -ContentType 'application/json' -Body $body -TimeoutSec 180 -UseBasicParsing
[IO.File]::WriteAllText((Join-Path (Get-Location) $OutputFile),$response.Content,[Text.UTF8Encoding]::new($false))
Write-Output $response.Content
$line=($response.Content -split '\r?\n'|Where-Object {$_ -like 'data: *'}|Select-Object -Last 1)
if($line){$parsed=$line.Substring(6)|ConvertFrom-Json;if($parsed.error -or $parsed.result.isError){exit 1}; if($parsed.result.structuredContent.result.Summary.FailedTests -gt 0){exit 2}}
