# A deterministic geometry diagram for image generation, not game art or runtime placement.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$bitmap = [Drawing.Bitmap]::new(1800,1100)
$g = [Drawing.Graphics]::FromImage($bitmap)
$g.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([Drawing.Color]::FromArgb(249,247,238))
$gridPen = [Drawing.Pen]::new([Drawing.Color]::FromArgb(210,213,205),1)
$edgePen = [Drawing.Pen]::new([Drawing.Color]::FromArgb(48,71,71),4)
$font = [Drawing.Font]::new('Arial',21)
function Project([double]$u,[double]$v) {
    [Drawing.PointF]::new([single](720+55*$u-55*$v),[single](65+29*$u+29*$v))
}
try {
    for ($u=0; $u -le 18; $u++) { $g.DrawLine($gridPen,(Project $u 0),(Project $u 11)) }
    for ($v=0; $v -le 11; $v++) { $g.DrawLine($gridPen,(Project 0 $v),(Project 18 $v)) }
    $items = @(
        @{Label='H1';U=1;V=1;W=2;D=2},@{Label='H2';U=4;V=1;W=2;D=2},
        @{Label='H3';U=7;V=1;W=2;D=2},@{Label='H4';U=10;V=1;W=2;D=2},
        @{Label='H5';U=13;V=1;W=2;D=2},@{Label='FARM';U=2;V=7;W=3;D=2},
        @{Label='KITCHEN';U=8;V=7;W=2;D=2},@{Label='TAVERN';U=13;V=7;W=3;D=2}
    )
    foreach ($item in $items) {
        $points = [Drawing.PointF[]]@((Project $item.U $item.V),(Project ($item.U+$item.W) $item.V),(Project ($item.U+$item.W) ($item.V+$item.D)),(Project $item.U ($item.V+$item.D)))
        $brush = [Drawing.SolidBrush]::new([Drawing.Color]::FromArgb(235,202,150))
        if ($item.Label -eq 'FARM') { $brush.Color = [Drawing.Color]::FromArgb(183,207,151) }
        $g.FillPolygon($brush,$points); $g.DrawPolygon($edgePen,$points)
        $center = Project ($item.U+$item.W/2) ($item.V+$item.D/2)
        $g.DrawString($item.Label,$font,[Drawing.Brushes]::Black,$center.X-38,$center.Y-15)
        $brush.Dispose()
    }
    $g.DrawString('PROJECTION GUIDE ONLY / u=(55,29), v=(-55,29)',$font,[Drawing.Brushes]::Black,32,985)
    $g.DrawString('Tavern size and all positions are illustrative; not runtime rules.',$font,[Drawing.Brushes]::Black,32,1023)
    $bitmap.Save((Join-Path $PSScriptRoot '../../../01_Direction/GeometryGuides/r5-grid-guide.png'),[Drawing.Imaging.ImageFormat]::Png)
} finally { $font.Dispose(); $gridPen.Dispose(); $edgePen.Dispose(); $g.Dispose(); $bitmap.Dispose() }
