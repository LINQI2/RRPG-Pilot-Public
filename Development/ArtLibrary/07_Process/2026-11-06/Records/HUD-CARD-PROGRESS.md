# 战斗HUD与卡牌通用回退图（2026-11-07）

Target Branch：ai/rrpg-r3b-population-pilot。持续美术任务，本批非全游戏首轮完成；未提交/推送。

## 原位修改
- SampleScene现有BattleHudPresenter.turnHost/drawPileHost/discardPileHost保留原只读绑定。TurnRoot/DrawPileRoot/DiscardPileRoot的旧装饰Image关闭，新增非交互HudPaper；现有文字使用CurrentUIFontAuthority.Default、深棕字、明确中文计数标签。
- 回合上中300×64，牌堆左右下角200×64；没有新增第二套HUD数据源。三处实查均无Button，不隐藏交互。顶部旧英雄框及999装饰数值不在本批改动范围，仍待核对和统一。
- 原card_art_placeholder.png改为冒险日志/地图/指南针暖插画；复用BattleCardView与原GUID/导入设置。仅是未配置专属ArtSprite时的通用回退图，不声称每张牌有专属效果插画。
- 修改前Scene与PNG备份见08_ExistingProject/battle-hud-before-r2.txt、card-art-placeholder-before-r1.png；接入SHA见04_Integrated/battle-ui-art-integration.json。
- 新图使用内置image_gen；完整PNG及原缓存保留，候选02_Candidates/Battle/Backgrounds/card_adventure_fallback_r1.png，提示词PROMPTS-CARD-FALLBACK-R1.md。

## 实机证据
真实Continue→Town→Expedition→Confirm→可用Battle节点成功；原拖拽处理部署两人并开战。截图Evidence/battle-hud-card-r1.png显示新面板与通用插画。实际两张牌为切配与分食，均读取原card_art_placeholder路径新内容。
开战后显示回合1、抽牌堆2、弃牌堆0；结束回合后回合显示刷新至3，Phase=PlayerTurn。没有伪造牌堆值，不宣称验证攻击、出牌、结算或返程。
本场真实遭遇显示mon_006骨面巨兽，补齐其出场验证；此前未出场的mon_003/004仍待实际显示。
原有InsufficientSupply拒绝消息作为基线保留；最终排除该消息后Error/Exception=0。最终Play=false、SceneDirty=false。

## 保存与边界
当前存档测试前备份至忽略的Temp/ArtHudSave-<SHA>.original，结束前SaveCheck=true，SHA为2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF；退出Play未保存/返程，没有恢复旧存档。模块证据见Evidence/hud-card-product-check.json。
本批仅Scene表现布局、单张直接Unity PNG及ArtLibrary记录，无产品C#、Prefab、Gameplay数值变更。SHA与148处目录链接校验通过。
下一步继续旧英雄框、卡牌头像/费用/状态图标与专属插画，并覆盖主菜单/选人/管理子页。维持先全模块基础统一再集中细化，动画暂缓。
