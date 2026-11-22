# 战斗标题、卡牌头像/费用与状态图标（2026-11-07）

Target Branch：ai/rrpg-r3b-population-pilot。持续任务，本批已落盘，无commit/push。

## 原位替换与边界
旧DecorationImage是一张画有“英雄名称/999”的静态装饰图，不是实时英雄HUD。Editor中关闭其Image，保留原物体；同HudDecorationRoot增加非交互纸色标题“远征战斗”，不伪造英雄事实。
BattleCard.prefab保留原BattleCardView、Button和规则绑定。关闭两张烘焙粉发头像/蓝色费用圆图的装饰Image；现有ownerPortraitImage重挂到同一卡根并定为112×112、preserveAspect，避免受Text_Owner文字节点影响；费用数字保留原ActualCost来源，独立100×100纸底、62号字。未改UnitCard.prefab已有工作区变化。
3张近景头像沿旧身体图发色/服饰生成，用原4条PersonVisualBinding记录的CardPortraitSpriteOverride配置；不变更PersonId、其他表面或默认图。各自UI近景构图不强套世界镜头；头向略有不同，当前作为首轮候选。
3张独立透明图标：木盾用于Shield/Defend，剑用于Attack，紫色星芒用于Special，接到原BattleUI公开表现字段，显隐与意图语义仍由原代码掌管。没有修改玩法C#、数值、卡牌规则或泄露敌方目标。

## 验证
真实Continue→Town→Expedition→Confirm→可用Battle节点→原拖拽部署→Start完成；最终截图Evidence/battle-portraits-icons-r1.png显示新标题、牌主头像、费用标记和攻击图标。旧“999”与固定粉发头像消失。
红披肩与绿兜帽实际卡牌分别读portrait_red_cape_r1和portrait_green_hood_r1；两张实际费用均1且不溢出。紫帽仅资源导入与绑定完成，本场未出场。攻击图标实际在3个敌方Intent宿主显示；护盾/防御/特殊本场未触发，不冒称已显示验收。
原结束回合回调执行后Phase=PlayerTurn。本批未完成选牌/取消/出牌回归、攻击结算、返程及多分辨率检查，后续继续补。
首轮截图battle-portrait-cost-r1.png另补mon_004重甲骑士真实出场；当前只剩mon_003暗狼未在已记录真实遭遇中出场。
Console包含此前InsufficientSupply拒绝节点基线和一次只读探测脚本误写Resources.FindObjectsOfType的工具编译异常；脚本已改Object.FindObjectsOfType。排除这两种已知消息后无其他Error/Exception，不宣称Console绝对空。

## 中断与保存
接口曾从工具列表移除；恢复时读到Play=false、Dirty=false。随后为图标最终验证重新走真实入口，不用旧截图冒充新图标接入。
当前存档先备份至忽略Temp/ArtIconSave-<SHA>.original；末尾SaveCheck=true，SHA保持2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF。未恢复旧存档或保存/返程，最终Play=false、SceneDirty=false。Evidence/portraits-icons-product-check.json记录实机与保存核验。

## 文件/归档
新增6份内置image_gen原始PNG，完整提示词PROMPTS-PORTRAITS-R1.md、PROMPTS-BATTLE-ICONS-R1.md，缓存原件保留。候选在02_Candidates/Battle/Persons与Icons，运行副本Assets/Art/Characters/ArtUpgrade01和Assets/Art/UI/ArtUpgrade01。精确目标/SHA/meta/产品范围见04_Integrated/portrait-integration-r1.json及status-icons-integration-r1.json。
Scene、BattleCard.prefab及人物绑定的修改前只读备份在08_ExistingProject对应before-portrait/header/icons文件。序列化资源均通过Editor/MCP修改，没有手改YAML。

下一步优先转向主菜单、选人/管理等未覆盖子页，并补选牌交互回归；HP/攻击数值旧圆图、按钮装饰、专属卡牌插画等残留继续列入首轮清单。全体基础覆盖后再统一细化，动画继续暂缓。
