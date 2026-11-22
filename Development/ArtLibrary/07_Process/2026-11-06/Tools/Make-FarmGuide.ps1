# Deterministic 3x2 projection diagram; no game art is edited.
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$b=[Drawing.Bitmap]::new(1400,1100)
$g=[Drawing.Graphics]::FromImage($b)
$g.SmoothingMode=[Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([Drawing.Color]::Ivory)
$pen=[Drawing.Pen]::new([Drawing.Color]::DarkSlateGray,5)
function FarmPoint([double]$u,[double]$v) { [Drawing.PointF]::new([single](560+220*$u-220*$v),[single](200+116*$u+116*$v)) }
try {
    $colors=@([Drawing.Color]::Goldenrod,[Drawing.Color]::DarkSeaGreen,[Drawing.Color]::OliveDrab)
    for($i=0;$i -lt 3;$i++) {
        $v0=$i*2.0/3; $v1=($i+1)*2.0/3
        $pts=[Drawing.PointF[]]@((FarmPoint 0 $v0),(FarmPoint 3 $v0),(FarmPoint 3 $v1),(FarmPoint 0 $v1))
        $brush=[Drawing.SolidBrush]::new($colors[$i]); $g.FillPolygon($brush,$pts); $g.DrawPolygon($pen,$pts); $brush.Dispose()
    }
    $b.Save((Join-Path $PSScriptRoot '../../../01_Direction/GeometryGuides/farm-3x2-grid-guide.png'),[Drawing.Imaging.ImageFormat]::Png)
} finally { $pen.Dispose();$g.Dispose();$b.Dispose() }
