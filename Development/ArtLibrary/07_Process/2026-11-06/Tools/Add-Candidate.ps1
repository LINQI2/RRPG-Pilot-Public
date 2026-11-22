param(
    [Parameter(Mandatory=$true)][string]$Name,
    [Parameter(Mandatory=$true)][ValidateSet('Buildings','Farms','Places','Persons','Terrain','Backgrounds','Icons')][string]$Category,
    [Parameter(Mandatory=$true)][string]$Purpose,
    [Parameter(Mandatory=$true)][string]$Reason,
    [Parameter(Mandatory=$true)][string]$GeneratorCachePath,
    [Parameter(Mandatory=$true)][string]$PromptRecord,
    [ValidateSet('Village','Journey','Battle')][string]$Surface='Village'
)
$ErrorActionPreference='Stop'
$library=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../../../../..')).Path
if([IO.Path]::GetFileName($Name) -ne $Name -or [IO.Path]::GetExtension($Name) -ne '.png'){throw 'Expected PNG leaf filename'}
$source=(Resolve-Path -LiteralPath (Join-Path $library "00_Inbox/$Name")).Path
$destination=[IO.Path]::GetFullPath((Join-Path $library "02_Candidates/$Surface/$Category/$Name"))
if(-not $source.StartsWith($library+'\',[StringComparison]::OrdinalIgnoreCase) -or -not $destination.StartsWith($library+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Path escaped library'}
if(Test-Path -LiteralPath $destination){throw 'Destination exists'}
if(-not(Test-Path -LiteralPath (Join-Path $library $PromptRecord))){throw 'Missing actual prompt record'}
$sha=(Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
if((Get-FileHash -LiteralPath $GeneratorCachePath -Algorithm SHA256).Hash -ne $sha){throw 'Cache/source SHA mismatch'}
$rows=@(Get-Content -LiteralPath (Join-Path $library 'catalog.json') -Raw | ConvertFrom-Json)
if(@($rows|Where-Object File -eq $Name).Count){throw 'Asset already cataloged'}
New-Item -ItemType Directory -Path (Split-Path $destination -Parent) -Force | Out-Null
Move-Item -LiteralPath $source -Destination $destination
if((Get-FileHash -LiteralPath $destination -Algorithm SHA256).Hash -ne $sha){throw 'Moved art changed'}
$row=[pscustomobject]@{File=$Name;Status='candidate';Purpose=$Purpose;Reason=$Reason;CurrentPath=$destination.Substring($repo.Length+1).Replace('\','/');OriginalPath=$source.Substring($repo.Length+1).Replace('\','/');SHA256=$sha;Bytes=(Get-Item -LiteralPath $destination).Length;Generator='built-in image_gen';GeneratorCachePath=$GeneratorCachePath;StatusLabel='待接入候选';ReadyForIntegration=$false;PromptOrRecipe=$PromptRecord}
@($rows)+@($row) | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $library 'catalog.json') -Encoding utf8
Write-Output "Cataloged $Name"
