param([Parameter(Mandatory=$true)][string]$RequestPath,[string]$OutputPath)
$ErrorActionPreference='Stop'
$e=Get-Content -LiteralPath (Join-Path $PSScriptRoot '../Evidence/unity-mcp-reconnection.json') -Raw | ConvertFrom-Json
$headers=@{Accept='application/json, text/event-stream';'MCP-Protocol-Version'='2024-11-05'}
if($e.sessionId){$headers['Mcp-Session-Id']=$e.sessionId}
$request=Get-Content -LiteralPath $RequestPath -Raw
$response=Invoke-WebRequest -Uri $e.endpoint -Method Post -Headers $headers -ContentType 'application/json' -Body ([System.Text.Encoding]::UTF8.GetBytes($request)) -TimeoutSec 55
$parsed=@([regex]::Matches($response.Content,'(?m)^data: (.+)$') | ForEach-Object {$_.Groups[1].Value | ConvertFrom-Json})
if(!$parsed.Count){$parsed=@($response.Content|ConvertFrom-Json)}
$json=$parsed|ConvertTo-Json -Depth 45
if($OutputPath){$json|Set-Content -LiteralPath $OutputPath -Encoding utf8}
$json
