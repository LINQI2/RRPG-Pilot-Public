# 远征、战斗背景真实流程及环境层升级

2026-11-07，Target Branch：`ai/rrpg-r3b-population-pilot`。持续任务记录，未提交、未推送，不关闭全游戏美术升级。

## 本轮完成

真实入口：MainMenuUI.OnContinue → TownUI.EnterProductExpedition → 现有可用队伍与补给校验 → btnConfirm → Journey；再从实际可用Battle节点调用原HandleNodeClick → BattleUI正式战斗准备初始化。没有强制激活背景Canvas来冒充产品验证，没有新建测试人物、注入敌人或修改资源。

- 主背景已在真实Journey与Battle准备画面显示；战斗备用图仅完成Editor导入/回读，正常路径没有消费备用图。
- Journey首轮截图暴露旧蓝色山形、绿色条带遮住新背景。内置image_gen生成透明低植被与草丛层，原位替换journey_env_far_placeholder.png及journey_env_near_placeholder.png，保留原Image、布局与滚动宿主。
- 第二次从相同真实入口出征，Journey可见、实际队伍2人，两层均为1774×887、raycastTarget=false；截图确认几何占位已消失、中央不再被实心山形遮住。
- 未修改Unity C#、场景、Prefab或玩法规则；PNG通过Editor导入，精确旧新SHA及Meta检查在04_Integrated/cross-scene-background-replacements.json。原图备份保留。

新增素材在 `02_Candidates/Journey/Terrain/journey_far_foliage_r1.png`、`journey_near_grass_r1.png`。提示词为 `Prompts/PROMPTS-JOURNEY-OVERLAYS-R1.md`，原始透明PNG与生成缓存均保留。

## 验证与限制

- journey-background-product-entry.json：真实出征可见，队伍2人。
- battle-background-product-entry.json：真实可用Battle节点进入成功，Battle initialized=true。
- journey-layers-product-verification.json：叠层替换后再走真实出征，通过。
- cross-scene-product-console.json：排除已归档的历史工具/夹具消息后，无其他Error/Exception。
- 原存档先备份到被忽略的Unity Temp并记SHA；两次验证后文件SHA始终为29600FBC09239F6BC3C8AC423B1C586996D679AF2618E47F30EF047A8F17EF10。没有执行保存、新游戏、返程保存；最终退出Play，丢弃内存测试远征。

实际GameView：journey-background-product-r1.png（暴露旧层）、journey-layers-product-r2.png（新层），battle-background-product-r1.png（真实战斗准备）。这是背景/环境宿主验证，**不是完整战斗、返程、动画或整模块首轮验收**。

## 下一批缺口

远征旧角色仍偏像素/平视，脚底落点与新道路带仍需配合角色视图统一校准；Journey HUD仍是旧样式。战斗有旧单位卡、HUD/数值占位与图标。继续这些基础缺口及其他未覆盖界面，不返回村庄精修，不追加本批背景装饰；动画仍暂缓，之后采用骨骼方案。
