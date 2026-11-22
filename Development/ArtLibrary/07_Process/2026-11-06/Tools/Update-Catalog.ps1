$ErrorActionPreference='Stop'
$library=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../../../../..')).Path
$prefix='07_Process/2026-11-06/Prompts/'
$promptMap=@{
    'visual-target-01.png'='PROMPTS.md';'visual-target-02.png'='PROMPTS.md';'visual-target-03.png'='PROMPTS.md'
    'home-spatial-r2.png'='PROMPTS-CAMERA-CORRECTION.md';'home-spatial-r3-topdown-control.png'='PROMPTS-CAMERA-CORRECTION.md'
    'home-spatial-r4.png'='PROMPTS-CAMERA-CORRECTION-2.md';'home-spatial-r5-grid.png'='PROMPTS-R5-GRID.md'
    'house_01.png'='PROMPTS-BUILDINGS-01.md';'kitchen_01.png'='PROMPTS-BUILDINGS-01.md'
    'house_r2_high.png'='PROMPTS-CAMERA-CORRECTION.md';'kitchen_r2_high.png'='PROMPTS-CAMERA-CORRECTION-2.md'
    'house_r3.png'='PROMPTS-R4-BUILDINGS.md';'kitchen_r3.png'='PROMPTS-R4-BUILDINGS.md'
    'farm_r5_square_rejected.png'='PROMPTS-R5-FARM.md';'farm_r5_background_rejected.png'='PROMPTS-R5-FARM.md'
    'farm_r5_candidate.png'='PROMPTS-R5-FARM-CUTOUT.md'
}
$labels=@{style_selected='已选画风';camera_selected='已认可视角';alignment_proposal='当前对齐方案';candidate='待接入候选';control_only='仅实验对照';not_selected='未选方向';rejected='已放弃/失败';superseded='已被替代';geometry_guide='几何导引'}
$rows=@(Get-Content -LiteralPath (Join-Path $library 'catalog.json') -Raw | ConvertFrom-Json)
$integrationPath=Join-Path $library '04_Integrated/home-prototype-integration.json'
$integrations=if(Test-Path -LiteralPath $integrationPath){@(Get-Content -LiteralPath $integrationPath -Raw|ConvertFrom-Json)}else{@()}
$nativePath=Join-Path $library '04_Integrated/cross-scene-background-replacements.json'
$native=if(Test-Path -LiteralPath $nativePath){@(Get-Content -LiteralPath $nativePath -Raw|ConvertFrom-Json)}else{@()}
$personPath=Join-Path $library '04_Integrated/person-surface-integration-r1.json'
$personAssets=if(Test-Path -LiteralPath $personPath){@(Get-Content -LiteralPath $personPath -Raw|ConvertFrom-Json)}else{@()}
$homePersonPath=Join-Path $library '04_Integrated/home-persons-integration-r1.json'
$homePersonAssets=if(Test-Path -LiteralPath $homePersonPath){@(Get-Content -LiteralPath $homePersonPath -Raw|ConvertFrom-Json)}else{@()}
foreach($entry in $homePersonAssets){
    if((Get-FileHash -LiteralPath (Join-Path $repo $entry.SourcePath)).Hash -ne $entry.SHA256 -or (Get-FileHash -LiteralPath (Join-Path $repo $entry.GeneratedPath)).Hash -ne $entry.SHA256 -or (Get-FileHash -LiteralPath (Join-Path $repo ($entry.GeneratedPath+'.meta'))).Hash -ne $entry.MetaSHA256){throw 'Home person source/managed asset changed without a record'}
}
$eventPath=Join-Path $library '04_Integrated/event-illustrations-r1.json'
$eventAssets=if(Test-Path -LiteralPath $eventPath){@(Get-Content -LiteralPath $eventPath -Raw|ConvertFrom-Json)}else{@()}
foreach($entry in $eventAssets){
    $target=Join-Path $repo $entry.TargetPath
    if((Get-FileHash -LiteralPath $target).Hash -ne $entry.SHA256 -or (Get-FileHash -LiteralPath ($target+'.meta')).Hash -ne $entry.MetaSHA256){throw 'Event illustration/import settings changed without a record'}
}
$portraitPath=Join-Path $library '04_Integrated/portrait-integration-r1.json'
if(Test-Path -LiteralPath $portraitPath){$personAssets+=@(Get-Content -LiteralPath $portraitPath -Raw|ConvertFrom-Json)}
$iconsPath=Join-Path $library '04_Integrated/status-icons-integration-r1.json'
$iconAssets=if(Test-Path -LiteralPath $iconsPath){@(Get-Content -LiteralPath $iconsPath -Raw|ConvertFrom-Json)}else{@()}
foreach($entry in $iconAssets){
    $target=Join-Path $repo $entry.TargetPath
    if((Get-FileHash -LiteralPath $target).Hash -ne $entry.SHA256 -or (Get-FileHash -LiteralPath ($target+'.meta')).Hash -ne $entry.MetaSHA256){throw 'Icon asset/import settings changed without a record'}
}
$enemyPath=Join-Path $library '04_Integrated/enemy-first-pass-integration.json'
$enemyAssets=if(Test-Path -LiteralPath $enemyPath){@(Get-Content -LiteralPath $enemyPath -Raw|ConvertFrom-Json)}else{@()}
$uiPath=Join-Path $library '04_Integrated/battle-ui-art-integration.json'
$uiAssets=if(Test-Path -LiteralPath $uiPath){@(Get-Content -LiteralPath $uiPath -Raw|ConvertFrom-Json)}else{@()}
foreach($entry in $personAssets){
    $target=Join-Path $repo $entry.TargetPath
    if((Get-FileHash -LiteralPath $target).Hash -ne $entry.SHA256 -or (Get-FileHash -LiteralPath ($target+'.meta')).Hash -ne $entry.MetaSHA256){throw 'Person asset/import settings changed without a record'}
}
foreach($row in $rows){
    $path=Join-Path $repo $row.CurrentPath
    if((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $row.SHA256){throw "Art bytes changed: $($row.File)"}
    $record=if($row.PSObject.Properties.Name -contains 'PromptOrRecipe' -and $row.PromptOrRecipe){$row.PromptOrRecipe}elseif($promptMap.ContainsKey($row.File)){$prefix+$promptMap[$row.File]}elseif($row.Status -eq 'geometry_guide'){'07_Process/2026-11-06/Tools/'+$(if($row.File -like 'farm-*'){'Make-FarmGuide.ps1'}else{'Make-GridGuide.ps1'})}else{throw "New asset requires its actual prompt record: $($row.File)"}
    if(-not(Test-Path -LiteralPath (Join-Path $library $record))){throw 'Missing prompt or geometry recipe'}
    $row | Add-Member StatusLabel $labels[$row.Status] -Force
    $row | Add-Member ReadyForIntegration $false -Force
    $row | Add-Member PromptOrRecipe $record -Force
    $integrated=@($integrations|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath})
    $row | Add-Member PrototypeIntegrated ($integrated.Count -gt 0) -Force
    $row | Add-Member VisualAssetId $(if($integrated.Count){$integrated[0].VisualAssetId}else{''}) -Force
    if($integrated.Count){$row|Add-Member StatusLabel '候选（已接入B实机样板）' -Force}
    $nativeMatched=@($native|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath -and $_.ProductFlowVerified})
    $row|Add-Member NativeHostProductVerified ($nativeMatched.Count -gt 0) -Force
    if($nativeMatched.Count){$row|Add-Member StatusLabel '跨场景背景宿主已验证（模块未完成）' -Force}
    $personMatched=@($personAssets|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath})
    $eventMatched=@($eventAssets|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath})
    if($eventMatched.Count){$row|Add-Member StatusLabel $(if($eventMatched[0].ProductFlowVerified){'事件插图已接真实遭遇（首轮候选）'}else{'事件插图已接类别配置（待真实触发）'}) -Force}
    if($personMatched.Count){
        $row|Add-Member StatusLabel $(if($personMatched[0].ProductFlowVerified){'人物已接Journey/战斗准备实机（模块未完成）'}else{'人物已接配置（待产品显示验证）'}) -Force
    }
    $homePersonMatched=@($homePersonAssets|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath})
    $row|Add-Member HomePersonProductVerified ($homePersonMatched.Count -gt 0 -and $homePersonMatched[0].ProductFlowVerified) -Force
    if($homePersonMatched.Count){$row|Add-Member StatusLabel $(if($homePersonMatched[0].Direction -eq 'NE'){'已停用多视角扩展（历史归档）'}elseif($homePersonMatched[0].ProductFlowVerified){'人物身体已接领地/远征实机（首轮候选）'}else{'人物单视角配置（待复验）'}) -Force}
    $enemyMatched=@($enemyAssets|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath})
    if($enemyMatched.Count){$row|Add-Member StatusLabel $(if($enemyMatched[0].ProductFlowVerified){'敌人已接实机准备画面（首轮候选）'}else{'敌人已构建并解析（待出场验证）'}) -Force}
    if(@($uiAssets|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath}).Count){$row|Add-Member StatusLabel '卡牌通用回退图已接实机（非专属插画）' -Force}
    $iconMatched=@($iconAssets|Where-Object {('Development/ArtLibrary/'+$_.Candidate) -eq $row.CurrentPath})
    if($iconMatched.Count){$row|Add-Member StatusLabel $(if($iconMatched[0].ProductFlowVerified){'状态图标实机已显示'}else{'状态图标已接配置（待触发验证）'}) -Force}
    if($iconMatched.Count -and $row.File -in @('icon_party_flag_r1.png','icon_settings_gear_r1.png')){$row|Add-Member StatusLabel $(if($iconMatched[0].ProductFlowVerified){'战斗按钮图标实机已显示'}else{'战斗按钮图标待最终实机复验'}) -Force}
    if($row.File -like 'portrait_*' -and $personMatched.Count){$row|Add-Member StatusLabel $(if($personMatched[0].ProductFlowVerified){'卡牌头像实机已显示'}else{'卡牌头像已接配置（待出场验证）'}) -Force}
}
$rows | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $library 'catalog.json') -Encoding utf8
$rows | Export-Csv -LiteralPath (Join-Path $library 'catalog.csv') -NoTypeInformation -Encoding utf8
$imageCount=@($rows|Where-Object Generator -eq 'built-in image_gen').Count
$guideCount=@($rows|Where-Object Status -eq 'geometry_guide').Count
$md=@('# 美术资源分类索引','',"$imageCount 张ImageGen图片与 $guideCount 张几何导引已归档；候选不等于可正式接入。新旧路径、原缓存及SHA见catalog.json/csv。",'','| 文件 | 状态 | 用途与边界 | 提示词/制图脚本 |','|---|---|---|---|')
foreach($row in $rows){$rel=$row.CurrentPath.Substring('Development/ArtLibrary/'.Length);$md+="| [$($row.File)]($rel) | $($row.StatusLabel) | $($row.Purpose)：$($row.Reason) | [记录]($($row.PromptOrRecipe)) |"}
$md | Set-Content -LiteralPath (Join-Path $library 'CATALOG.md') -Encoding utf8
$checkFiles=@(Get-ChildItem -LiteralPath $library -Recurse -File -Filter 'README.md')+@(Get-Item -LiteralPath (Join-Path $library 'CATALOG.md'))+@(Get-Item -LiteralPath (Join-Path $repo 'Development/ArtUpgrade/2026-11-06/README.md'),(Join-Path $repo 'Development/SourceAssets/Village/ArtUpgrade01/README.md'))
$links=0
foreach($file in $checkFiles){
    # Archived compiler snapshots retain their original relative links; they are not live navigation.
    if($file.FullName -like '*\BeforeHomePersons-20261108\Generated\*'){continue}
    $content=Get-Content -LiteralPath $file.FullName -Raw
    foreach($match in [regex]::Matches($content,'\]\(([^)]+)\)')){
        $target=$match.Groups[1].Value
        if($target -match '^https?://|^#'){continue}
        $target=$target.Split('#')[0]
        $resolved=[IO.Path]::GetFullPath((Join-Path $file.DirectoryName $target))
        if(-not(Test-Path -LiteralPath $resolved)){throw "Broken navigation: $($file.FullName) -> $target"}
        $links++
    }
}
$legacy=Import-Csv -LiteralPath (Join-Path $PSScriptRoot '../Evidence/raster-and-model-inventory.csv')
$current=Import-Csv -LiteralPath (Join-Path $library '08_ExistingProject/existing-project-inventory.csv')
$lookup=@{};foreach($row in $current){$lookup[$row.Path]=$row.SHA256}
$approved=@{};foreach($entry in $integrations){
    $approved['Development/SourceAssets/'+$entry.SourceFile]=$entry
    $approved['UnityProject/RRPG1.0game/Assets/Generated/StaticConfig/Assets/'+$entry.SourceFile]=$entry
}
$nativePath=Join-Path $library '04_Integrated/cross-scene-background-replacements.json'
$native=if(Test-Path -LiteralPath $nativePath){@(Get-Content -LiteralPath $nativePath -Raw|ConvertFrom-Json)}else{@()}
$nativeTargets=@{};foreach($entry in $native){$approved[$entry.TargetPath]=$entry;$nativeTargets[$entry.TargetPath]=$true}
$changed=0;$nativeChanged=0
$enemyChanged=0
foreach($entry in $enemyAssets){$approved[$entry.SourcePath]=$entry;$approved[$entry.GeneratedPath]=$entry}
foreach($entry in $uiAssets){$approved[$entry.TargetPath]=$entry}
foreach($row in $legacy){if($lookup[$row.Path] -ne $row.SHA256){
    if(!$approved.ContainsKey($row.Path) -or $lookup[$row.Path] -ne $approved[$row.Path].AfterSHA256 -or $row.SHA256 -ne $approved[$row.Path].BeforeSHA256){throw "Unrecorded existing project art change: $($row.Path)"}
    $changed++
    if($nativeTargets.ContainsKey($row.Path)){$nativeChanged++}
    if($row.Path -match '/Enemy/enemy_mon_'){$enemyChanged++}
}}
$receipt=[ordered]@{Date='2026-11-06';ArtEntries=$rows.Count;ImageGenImages=@($rows|Where-Object Generator -eq 'built-in image_gen').Count;GeometryGuides=$guideCount;AllArtHashesMatch=$true;NavigationLinksChecked=$links;ExistingProjectFilesUnchanged=($legacy.Count-$changed);RecordedPrototypeReplacementFiles=($changed-$nativeChanged-$enemyChanged-$uiAssets.Count);NativeUIArtReplacements=$uiAssets.Count;EnemyReplacementFiles=$enemyChanged;NativeBackgroundReplacementFiles=$nativeChanged;NativeBackgroundHostTargetsProductVerified=@($native|Where-Object ProductFlowVerified).Count;NativeBackgroundReplacementsPendingProductVerification=@($native|Where-Object {-not $_.ProductFlowVerified}).Count;ReadyToIntegrate=0;IntegratedPrototypeThisUpgrade=$integrations.Count;FinalArtAccepted=0;HistoricalPromptsRetainOriginalPaths=$true;StatusCounts=@($rows|Group-Object StatusLabel|Select-Object Name,Count)}
$receipt | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $PSScriptRoot '../Evidence/organization-verification.json') -Encoding utf8
$receipt | ConvertTo-Json -Depth 6



