# Explicit, bounded migration of this art-upgrade session's unregistered files.
$ErrorActionPreference='Stop'
$repo=(Resolve-Path -LiteralPath 'D:/AI_Agent_Lab/rrpg_pilot/repo').Path
$library=[IO.Path]::GetFullPath((Join-Path $repo 'Development/ArtLibrary'))
$oldRecords=Join-Path $repo 'Development/ArtUpgrade/2026-11-06'
$oldSources=Join-Path $repo 'Development/SourceAssets/Village/ArtUpgrade01'
$process='07_Process/2026-11-06'
$art=@(
    @('visual-target-01.png','01_Direction/Selected','style_selected','风格参考','采用温暖插画色彩/材质；原镜头与假UI未采用'),
    @('home-spatial-r4.png','01_Direction/Selected','camera_selected','镜头参考','视角已认可；原朝向与布局需修正'),
    @('home-spatial-r5-grid.png','01_Direction/Working','alignment_proposal','网格校准','当前统一斜向网格方案；非运行截图/保存布局'),
    @('house_r3.png','02_Candidates/Village/Buildings','candidate','住宅源图','待统一网格与实机尺寸/遮挡/点击验证'),
    @('kitchen_r3.png','02_Candidates/Village/Buildings','candidate','厨房源图','无火烟底图；待网格与实机验证'),
    @('farm_r5_candidate.png','02_Candidates/Village/Farms','candidate','农田源图','接近3×2投影；Alpha边缘与实机占地待验'),
    @('home-spatial-r3-topdown-control.png','05_Experiments/CameraControls','control_only','镜头对照','极端顶视上限，非当前量产方向'),
    @('house_r2_high.png','05_Experiments/CameraControls','control_only','住宅对照','更高俯视，与当前R4/R5镜头不混用'),
    @('kitchen_r2_high.png','05_Experiments/CameraControls','control_only','厨房对照','更高俯视，与当前R4/R5镜头不混用'),
    @('visual-target-02.png','06_Abandoned/NotSelected','not_selected','未选画风','制作人选择01；02留作历史，不是技术失败'),
    @('visual-target-03.png','06_Abandoned/NotSelected','not_selected','未选画风','制作人选择01；03留作历史，不是技术失败'),
    @('home-spatial-r2.png','06_Abandoned/WrongCamera','rejected','失败空间图','立面仍高且只生成4栋住宅'),
    @('house_01.png','06_Abandoned/WrongCamera','rejected','失败住宅','制作人否决平视倾向，不用于游戏'),
    @('kitchen_01.png','06_Abandoned/WrongCamera','rejected','失败厨房','平视倾向且火焰烘焙，不用于游戏'),
    @('farm_r5_square_rejected.png','06_Abandoned/InvalidFootprint','rejected','失败农田','近似投影正方形，不符合3×2'),
    @('farm_r5_background_rejected.png','06_Abandoned/AlphaBackground','superseded','旧农田版本','外围低Alpha底雾多，已由candidate替代'),
    @('r5-grid-guide.png','01_Direction/GeometryGuides','geometry_guide','网格导引','代码生成的几何图，不是游戏美术'),
    @('farm-3x2-grid-guide.png','01_Direction/GeometryGuides','geometry_guide','占地导引','代码生成3×2投影图，不是游戏美术')
)
$plan=[Collections.Generic.List[object]]::new()
foreach($row in $art) {
    $matches=@(Get-ChildItem -LiteralPath $oldRecords,$oldSources -File | Where-Object Name -eq $row[0])
    if($matches.Count -ne 1){throw "Expected one source: $($row[0])"}
    $plan.Add([pscustomobject]@{Source=$matches[0].FullName; Destination=(Join-Path $library ($row[1]+'/'+$row[0])); Kind='art'; Status=$row[2]; Purpose=$row[3]; Reason=$row[4]})
}
foreach($file in Get-ChildItem -LiteralPath $oldRecords -File | Where-Object Extension -ne '.png') {
    $category=if($file.Name -like 'PROMPTS*'){'Prompts'}elseif($file.Extension -eq '.ps1'){'Tools'}elseif($file.Extension -in '.json','.csv'){'Evidence'}else{'Records'}
    $plan.Add([pscustomobject]@{Source=$file.FullName;Destination=(Join-Path $library "$process/$category/$($file.Name)");Kind='process';Status='historical_record';Purpose=$category;Reason='保留原始提示词、路径与当时结论，不改写历史'})
}
$plan.Add([pscustomobject]@{Source=(Join-Path $oldSources 'README.md');Destination=(Join-Path $library "$process/Records/SourceAssets-README-before-organization.md");Kind='process';Status='historical_record';Purpose='Records';Reason='原源图目录说明，历史归档'})

# Validate every absolute source and target before the first move; no recursive moves.
$allOld=@(Get-ChildItem -LiteralPath $oldRecords,$oldSources -File)
if($allOld.Count -ne $plan.Count){throw 'Unexpected source files; stop before moving.'}
foreach($entry in $plan) {
    $entry.Source=(Resolve-Path -LiteralPath $entry.Source).Path
    $entry.Destination=[IO.Path]::GetFullPath($entry.Destination)
    if(-not $entry.Source.StartsWith($repo+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Source escaped repo'}
    if(-not $entry.Destination.StartsWith($library+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Destination escaped library'}
    if(Test-Path -LiteralPath $entry.Destination){throw "Destination exists: $($entry.Destination)"}
    $entry | Add-Member SHA256 (Get-FileHash -LiteralPath $entry.Source -Algorithm SHA256).Hash
    $entry | Add-Member Bytes (Get-Item -LiteralPath $entry.Source).Length
}
$runtimeManifest=Get-Content -LiteralPath (Join-Path $repo 'Development/Generated/StaticConfig/AssetManifest.json') -Raw | ConvertFrom-Json
if(@($runtimeManifest.assets | Where-Object SourceFile -like '*ArtUpgrade01*').Count){throw 'Source assets have runtime bindings; do not move.'}
$cache='1PUBLIC_USER/.codex/generated_images/01a1afb3-f692-7413-ae2f-eec22d1433f9'
$cacheItems=@(Get-ChildItem -LiteralPath $cache -File -Filter '*.png' | ForEach-Object { [pscustomobject]@{Path=$_.FullName;SHA256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash} })
$generated=@($plan | Where-Object { $_.Kind -eq 'art' -and $_.Status -ne 'geometry_guide' })
if($cacheItems.Count -ne $generated.Count){throw 'ImageGen cache count does not match project image count.'}
foreach($image in $cacheItems){if(-not ($generated.SHA256 -contains $image.SHA256)){throw 'Unmapped generated image; stop.'}}
$evidence=Join-Path $library "$process/Evidence"
New-Item -ItemType Directory -Path $evidence -Force | Out-Null
$plan | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidence 'organization-before.json') -Encoding utf8
foreach($entry in $plan) {
    New-Item -ItemType Directory -Path (Split-Path $entry.Destination -Parent) -Force | Out-Null
    Move-Item -LiteralPath $entry.Source -Destination $entry.Destination
}
foreach($entry in $plan){if((Get-FileHash -LiteralPath $entry.Destination -Algorithm SHA256).Hash -ne $entry.SHA256){throw 'Moved file hash mismatch'}}
$plan | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $evidence 'organization-move-receipt.json') -Encoding utf8
$catalog=@($plan | Where-Object Kind -eq 'art' | ForEach-Object {
    $entry=$_; $cached=@($cacheItems | Where-Object SHA256 -eq $entry.SHA256)
    [pscustomobject]@{File=[IO.Path]::GetFileName($entry.Destination);Status=$entry.Status;Purpose=$entry.Purpose;Reason=$entry.Reason;CurrentPath=$entry.Destination.Substring($repo.Length+1).Replace('\','/');OriginalPath=$entry.Source.Substring($repo.Length+1).Replace('\','/');SHA256=$entry.SHA256;Bytes=$entry.Bytes;Generator=if($entry.Status -eq 'geometry_guide'){'PowerShell/System.Drawing geometry diagram'}else{'built-in image_gen'};GeneratorCachePath=if($cached.Count){$cached[0].Path}else{''}}
})
$catalog | Export-Csv -LiteralPath (Join-Path $library 'catalog.csv') -NoTypeInformation -Encoding utf8
$catalog | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $library 'catalog.json') -Encoding utf8
$md=@('# 美术资源分类索引','','本次升级全部16张ImageGen图片与2张几何导引；所有运行接线状态均未通过。本文件路径以资源库根为基准。','','| 文件 | 状态 | 用途与边界 |','|---|---|---|')
foreach($row in $catalog){$rel=$row.CurrentPath.Substring('Development/ArtLibrary/'.Length);$md+="| [$($row.File)]($rel) | $($row.Status) | $($row.Purpose)：$($row.Reason) |"}
$md | Set-Content -LiteralPath (Join-Path $library 'CATALOG.md') -Encoding utf8
[pscustomobject]@{MovedFiles=$plan.Count;ImageGenImages=$generated.Count;GeometryGuides=2;CacheOriginalsUntouched=$true;MovedHashesVerified=$true} | ConvertTo-Json
