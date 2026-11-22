# 主菜单与英雄选择基础统一（2026-11-07）

Target Branch：ai/rrpg-r3b-population-pilot。持续升级，未提交/推送。管理子页尚未在本批修改。

## 现有宿主
复用MainMenuUI.MainMenuCanvas、HeroSelectionUI.HeroSelectionCanvas与HeroSelectCard.prefab；按钮/回调、HeroId和Save流程保留。背景复用已接入的高俯视Journey插画，只在菜单装饰Image上压暗；不新增PNG或第二套Canvas/入口。

## 改动
- 主菜单增加非交互背景、520×660纸色面板与RRPG标题；原Continue/NewGame/Delete按钮重新分区着色，原InfoText保留用于反馈但清理初始“New Text”占位。
- 选择页增加同背景及1200×760纸色面板；原标题与确认按钮统一字体/颜色。HeroSelectCard调整300×420，顶部136×136肖像，下方姓名/HP/ATK/技能分区，原Root Button仍负责选择。
- 原hero_001的HeroSelectSpriteOverride复用已制作红披肩头像，其余绑定不变。
- HeroSelectionUI.cs仅表现修改：绿色选中改暖金色、取消选中恢复Paper；横向布局按现有英雄总数围绕startPosition居中。Scene起点(0,0)、间距340。没有改变选择数据、存档或Hero逻辑。
- 新增装饰raycastTarget=false；没有执行删除/确认新游戏操作。

## 验证
Asset Refresh后编译正常、Console Error/Exception=0。真实Play启动主菜单可见，Continue可用；截图main-menu-art-r1.png。
真实主菜单NewGame按钮进入既有HeroSelection.Show。当前实际可选hero_001一名；原位卡片点击前Confirm=false、点击后true，EventSystem首命中HeroCard_hero_001，卡位置(0,0)，4个文字域均overflow=false。hero-select-art-r1.png保留为左偏首版，r2.png为修正后最终截图。
另一次真实启动点击Continue：菜单隐藏、TownActive=true，未以SetActive假装初始化。复验只证明选择/进入流程与基本视觉，不执行OnConfirm中新建并替换存档的步骤，未验证删除存档或无存档状态，也未验证多分辨率/多英雄布局。
原有存档先备份至忽略Temp/ArtMenuSave-<SHA>.original；三次测试末尾SaveCheck均true，SHA保持2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF。最终Play=false、SceneDirty=false、Compiling=false、Console无Error/Exception。证据menu-selection-product-check.json。

## 文件与交接
产品修改：HeroSelectionUI.cs、SampleScene.scene、HeroSelectCard.prefab、PersonVisualBindingCatalog.asset。后3者只经Editor/MCP修改，未手改YAML。Scene/Prefab/绑定修改前只读备份分别为08_ExistingProject/menu-scene-before-r1.txt、hero-select-card-before-r1.txt、person-binding-before-hero-select-r1.txt。保留其他已有工作区变化。
Editor制图辅助脚本07_Process/2026-11-06/Tools/MenuArtApply.cs不进入Unity Assets，不成为新的产品流程。本批没有生成新位图；背景与头像保持原资产来源及SHA。HeroSelectionUI.cs的git diff --check通过。

下一步优先管理/人物/建筑等尚未覆盖子页，再补准备/结果/事件卡残留。先全模块基础统一，再整体细化；动画继续后移。
