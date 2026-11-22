# 六类敌人基础美术替换（2026-11-07）

Target Branch：ai/rrpg-r3b-population-pilot。未提交/推送，不代表全游戏首轮或最终美术验收。

## 本批结果
mon_001史莱姆、mon_002骷髅弓手、mon_003暗狼、mon_004重甲骑士、mon_005森林施法者、mon_006骨面巨兽均已通过既有SourceAssets→Compiler→Unity Builder接入。保留MonsterId、VisualAssetId、XLSX绑定、数值和玩法C#。初版骑士朝向错误，保存在06_Abandoned；使用修正朝向的r2接入，不作动画。

人工源图6份原位替换，旧图在08_ExistingProject/BeforeEnemyFirstPass；精确前后SHA与候选路径见04_Integrated/enemy-first-pass-integration.json。候选与原缓存、提示词完整保留，提示词为PROMPTS-ENEMIES-R1.md和PROMPTS-ENEMIES-COMPLETE-R1.md。Builder生成副本6份哈希逐项与源图一致，未手改生成资产或meta。

## 生产链验证
改前Compiler validate通过（errors=0、warnings=9）。改后build通过，44份Mirror中仅AssetManifest.json变化，其他Mirror与改前快照哈希相同；未改工作簿。Unity Builder Success=true、NoOp=false，明确更新6份Enemy PNG，其他16项托管图不变，保留6条绑定。六个MonsterId经既有EnemyPresentationResolver.TryResolve均true、diagnostic空。

真实产品Continue→Town→Expedition→Confirm→实际可用Battle节点HandleNodeClick=true、initialized=true。截图Evidence/enemy-first-pass-product-r1.png显示mon_001、002、005的新图；其余003/004/006本次未抽中，只完成构建/解析验证，不冒称真实出场通过。截图中三类外观与方向基本协调，史莱姆在草地上的对比仍偏低，后续整批细化时复核。未验证攻击、结算、返程和多分辨率。

## 用户存档与Editor
本轮接入前发现用户正在Play战斗且存档已更新；用户明确允许退出Play、保留磁盘存档并丢弃未保存运行状态。先备份当前存档到忽略的Unity Temp/ArtEnemySave-<SHA>.original，采用新的基线：
2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF。
未恢复旧29600...测试存档。新图产品验证后SaveCheck=true，退出Play未调用Save/Return。最终Play=false、SceneDirty=false。Console基线已有InsufficientSupply拒绝节点消息，本轮最终排除该既有消息后Error/Exception=0；不称Console绝对空。

## 范围与下一步
本批涉及6份SourceAssets、Compiler生成AssetManifest、Builder生成根（ignored）、ArtLibrary素材/目录/证据/记录。不改Scene、Prefab、产品C#或游戏规则；保留所有其他工作区变化。资源SHA与目录导航校验通过。
接下来优先继续顶部HUD、卡牌占位插画与状态图标，再补主菜单/选人/管理子页；尚未出场的三种敌人随真实节点补验证。村庄精修和动画继续后移，全部模块首轮统一后再集中细化。
