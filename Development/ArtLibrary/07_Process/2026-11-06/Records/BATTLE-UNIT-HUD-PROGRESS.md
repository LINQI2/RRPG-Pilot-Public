# 战斗单位HUD首轮统一 — 2026-11-08

Target Branch: `ai/rrpg-r3b-population-pilot`。本批接入验证完成，整体美术升级仍进行中；未最终美术验收，未commit/push。

## Existing Host Map 与本批范围
- 原 `Assets/Prefabs/UI/UnitCard.prefab` 的 HPSlider / Background / Fill Area / Fill / Text_HP / Image（攻击数字底板）及 Text_ATK，继续由 BattleUI / BattleValuePresenter 刷新。
- 原 SampleScene / BattleUI 的 BtnParty、BtnSettings，保留原Button/targetGraphic与事件，只替换显示。
- 先通过Editor读取实际组件与资源，备份当前已含其他修改的Prefab和Scene，保留它们改前已有的TMP改动。所有.prefab/.scene/.meta均经Editor/MCP产生，没有手改YAML。
- 已使用uGUI skill；增量编辑已有宿主，不重建整个UI。Unity Insight同步成功，日志目录EPERM不影响索引完成。
- 领地人物本轮仅审计，未修改该模块代码、配置或资源。

## 已落地
1. UnitCard血条：移除旧条纹贴图，保留原Slider/fillRect，使用原生矩形木色底、暗红填充；142×20，中心(0,65)。HP文字同位置142×24、16号粗体奶油色；原current/max仍由真实BattleUnit读取。
2. HP Slider改只读，子图形不截获射线；原Slider数值刷新保留。原禁用Handle Slide Area保持禁用，不删除历史资源。
3. 攻击数字：旧蓝色圆图替换为原生暖纸矩形与细木色Outline；原Text_ATK保留、26号粗体墨色。姓名/AP文字同墨色。
4. 旗帜与设置：built-in image_gen各生成一张透明图，旧Button Image用纸色底，新增非交互子Icon+木色Outline。最终64×64，右上锚定，中心(-132,-54)/(-54,-54)，边缘22px、间距14px。未新增设置或队伍功能；这批仅验证图标显示与界面边界。
5. 没有改.cs、玩法、数值、人物身份、牌库、敌人配置或存档格式；没有新增动画。

## 真实产品验证
- EditMode/SceneClean/Console基线0。
- Continue → Town.EnterProductExpedition → 原确认出发 → 自动战斗遭遇 → 原BtnOption_0 → 原拖拽布阵 → 原StartBattle。
- 第一轮真实PlayerTurn：我方30/30、35/35，敌方5/5，Slider均1；所有单位显示新HP与攻击底板。
- 原EndTurn → 敌方行动 → 下一PlayerTurn：林恩HP **26/30**，Slider **0.8666667**；其他仍满血。证明真实受伤及数值填充同步，不是人为设置测试数值。
- 首轮截图发现原Button113.5px范围与新不透明底板造成顶边裁切，之后通过Editor将两按钮尺寸/位置修正。
- 最终另跑一次相同真实入口至PlayerTurn。1920×1080实际屏幕坐标：BtnSettings min(1834,994) max(1898,1058)；BtnParty min(1756,994) max(1820,1058)，均完整、互不重叠。最终血条Prefab未再变更，第二次实际满血显示通过；受伤状态证据来自第一轮同一最终Prefab。
- Console最终0 Error/Exception。Play切换中一次只读Console查询连接中断；随后只读Editor核对确认Play已进入，再继续流程，没有重发启动写操作。后续Console读取恢复正常。
- 未执行出牌、真实结算或返程存盘；本批不宣称这些流程已测。
- 两轮退出Play出现同样5处自动Layout坐标漂移。每轮先通过Editor保存Temp诊断副本，与正式磁盘Scene比对仅5处坐标后，Editor重载正式Scene，丢弃布局漂移。最终Play=False、Compiling=False、Dirty=False。
- 磁盘存档SHA256始终 `2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF`，SaveUnchanged=True。当前基线备份于忽略Temp，未恢复更早存档。

## 归档与证据
- `battle-unit-hud-full-r1.png` / `battle-unit-hud-damage-r1.png`：真实满血、受伤及回合变化；首版按钮尺寸失败也保留，不能作为最终按钮布局。
- `battle-unit-hud-final-r2.png`：最终尺寸按钮与单位HUD实际显示。
- 提示词：[BATTLE-UNIT-HUD-PROMPTS.md](BATTLE-UNIT-HUD-PROMPTS.md)，原缓存保留，候选与Unity项目均有PNG。
- [图标接入清单](../../../04_Integrated/status-icons-integration-r1.json)记录PNG/meta哈希和验证范围；[原生宿主清单](../../../04_Integrated/battle-unit-hud-r1.json)记录Prefab/Scene修改前后哈希及备份。
- Catalog已加入图标PNG/meta校验、按钮专用状态标签。60条＝58张ImageGen+2导引，hash校验通过，Ready/FinalArtAccepted仍0。
- 回滚备份在08_ExistingProject/UnitCard-before-unit-hud-20261108.prefab.backup与SampleScene-before-unit-hud-20261108.scene.backup。序列化内容恢复仍通过Editor；不覆盖之前其他任务的修改。

## 领地人物审计与下一组
现有HomePresentationResolver在B样板统一返回 village.person.r2，HomeWorldEntityView另读 village.person.r2.ne 作背面，并水平镜像形成四向。共享VisualAsset/VillagePresentation配置链是当前正式资产Authority。HomePersonSnapshot有Runtime PersonId，没有AppearanceKey；已有PlayerCollection能提供具体人物绑定。不能把人物名字/职业或战斗模板当作领地身份映射依据。

下一组按现有宿主推进：核对具体PersonId→AppearanceKey；成套准备6人物背面静帧，正面复用已接身体；共享配置表加入对应前/后资产与表现键，再用现有Host/Resolver读取；测试B样板方向、A/B切换、选择、遮挡及真实移动，保持模拟到达/占用规则不变。无需加行走动画或第二套人物系统。该组尚未生成背面、修改配置或实现绑定。

其他未覆盖：黄色射程提示及与邻排底板重叠、部分红色提醒标、护盾/特殊图标真实触发、卡牌专属插画、普通事件/岔路真实触发和真实结算。继续全局首轮，暂不单点精修。

