param([Parameter(Mandatory=$true)][string[]]$Paths)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$records = @(foreach ($path in $Paths) {
    $resolved = (Resolve-Path -LiteralPath $path).Path
    $bitmap = [System.Drawing.Bitmap]::new($resolved)
    try {
        $transparent = 0; $partial = 0; $opaque = 0; $alphaHistogram = @{}
        $minX = $bitmap.Width; $minY = $bitmap.Height; $maxX = -1; $maxY = -1
        for ($y=0; $y -lt $bitmap.Height; $y+=4) {
            for ($x=0; $x -lt $bitmap.Width; $x+=4) {
                $alpha = $bitmap.GetPixel($x,$y).A
                if (-not $alphaHistogram.ContainsKey([int]$alpha)) { $alphaHistogram[[int]$alpha] = 0 }
                $alphaHistogram[[int]$alpha]++
                if ($alpha -eq 0) { $transparent++ }
                elseif ($alpha -eq 255) { $opaque++ }
                else { $partial++ }
                if ($alpha -gt 8) {
                    $minX = [Math]::Min($minX,$x); $maxX = [Math]::Max($maxX,$x)
                    $minY = [Math]::Min($minY,$y); $maxY = [Math]::Max($maxY,$y)
                }
            }
        }
        [pscustomobject]@{Path=$resolved; Width=$bitmap.Width; Height=$bitmap.Height; PixelFormat=$bitmap.PixelFormat.ToString(); SHA256=(Get-FileHash -LiteralPath $resolved -Algorithm SHA256).Hash; SamplingStride=4; FullyTransparentSamples=$transparent; PartialAlphaSamples=$partial; OpaqueSamples=$opaque; MostFrequentAlpha=@($alphaHistogram.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 8 @{n='Alpha';e={$_.Key}},@{n='Samples';e={$_.Value}}); ApproxBoundsAlphaAbove8=@($minX,$minY,$maxX,$maxY); Note='Sampled alpha/content bounds, not a full pixel scan. No image modified. Runtime scale/pivot not validated.'}
    } finally { $bitmap.Dispose() }
})
$records | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $PSScriptRoot '../Evidence/source-art-inspection-current.json') -Encoding utf8
$records | ConvertTo-Json -Depth 4
