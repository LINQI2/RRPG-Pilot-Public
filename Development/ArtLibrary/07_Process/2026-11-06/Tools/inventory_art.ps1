$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../../../../..')).Path
$outDir = (Resolve-Path (Join-Path $PSScriptRoot '../../../08_ExistingProject')).Path
$roots = @((Join-Path $repo 'Development/SourceAssets'), (Join-Path $repo 'UnityProject/RRPG1.0game/Assets'))
$rows = @(foreach ($root in $roots) {
    foreach ($file in Get-ChildItem -LiteralPath $root -Recurse -File | Where-Object Extension -in '.png','.jpg','.jpeg','.psd','.tga','.fbx','.spine') {
        $width = ''; $height = ''
        if ($file.Extension -eq '.png') {
            $stream = [IO.File]::OpenRead($file.FullName)
            try {
                $header = [byte[]]::new(24)
                if ($stream.Read($header,0,24) -eq 24 -and $header[0] -eq 137 -and $header[1] -eq 80) {
                    $width = [uint32]$header[16]*16777216 + [uint32]$header[17]*65536 + [uint32]$header[18]*256 + $header[19]
                    $height = [uint32]$header[20]*16777216 + [uint32]$header[21]*65536 + [uint32]$header[22]*256 + $header[23]
                }
            } finally { $stream.Dispose() }
        }
        $relative = $file.FullName.Substring($repo.Length+1).Replace('\','/')
        [pscustomobject]@{Path=$relative; Width=$width; Height=$height; Bytes=$file.Length; SHA256=(Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash; BindingStatus='Not verified in live Editor'}
    }
})
$rows | Sort-Object Path | Export-Csv -LiteralPath (Join-Path $outDir 'existing-project-inventory.csv') -Encoding utf8 -NoTypeInformation
$summary = [ordered]@{TotalFiles=$rows.Count; UniqueContent=@($rows.SHA256 | Sort-Object -Unique).Count; Note='File inventory only, not live usage or completeness. PNG dimensions only. Fonts/packages/library excluded.'; Directories=@($rows | Group-Object {Split-Path $_.Path -Parent} | Select-Object Name,Count)}
$summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $outDir 'inventory-summary.json') -Encoding utf8
$summary | ConvertTo-Json -Depth 5
