# RRPG FP-V0.1 正常玩家路径核验：G6/G7

实际执行日期：**2026-11-06**（制作人时区 Asia/Shanghai）。工程 `D:/AI_Agent_Lab/rrpg_pilot/repo`；分支 `ai/rrpg-r3b-population-pilot`；开工/结束 HEAD 均为 `223a08a3d1c45cc5498a7b453cd1b9f4675908d4`，暂存区空。本轮不做 Git 提交。

## 结论

**FP-V0.1 未取得通过证据；G7 的规定链未走通，G6 的重复循环未验证。V17 保持 LOCAL DEVELOPMENT CLOSED。**

主开发已亲自通过真实鼠标点击实际 Unity GameView 完成：

```text
正式 MainMenu → 新游戏 → 选择英雄 → 确认 → Home
Home → 远征 → 正式准备页 → 开始冒险不可用（补给不足）
准备页 → 返回领地 → 设置 → 保存并返回主菜单 → Continue → Home
```

**G7 首个偏离点**：确认英雄后直接进 Home，没有经过 Tutorial Expedition 和第一次 Return。**独立出征分支的首个阻塞**：默认新游戏 Food=0，实际 Carry=0、第一步预计成本=2，开始冒险不可用；点击后仍停在准备页。没有注入食物或绕过校验继续。

这不证明游戏永久不能出征：正常 Farm/Kitchen 生产后是否可完成循环，本轮未运行。不能把初始未备粮误报为 Supply 功能错误，不能据此直接要求增加初始食物。没有形成“第一次 Return→第二次 Expedition→第二次 Return→再次 Prep”的本轮证据。

## 依据与状态分层

阅读入口：[产品设计与证据整合](RRPG_FP-V0.1_Product-Design-and-Evidence-Integration_2026-11-06.md)、[Evidence Index](RRPG_FP-V0.1_Evidence-Index_2026-11-06.md)；Gate 原文为 `Docs/DesignCheckpoints/2026-09-07_RRPG_Functional-Prototype-V0.1_Concept-Checkpoint.md` §3，十四项为 `GameSpec/Village/Village-GameSpec-V0.1.md` §6.8。Person/Battle/Expedition/Village Resource/Save Current Authority 决定身份、补给、返程和持久化语义；不由历史候选或测试夹具定义新规则。

G6 原要求：第一次 Return 后继续 Prep、再次 Expedition、第二次 Return 后仍可继续、无需人工改数据。G7 原要求：New Game→Tutorial Expedition→Return Home→Formal Prep→Expedition→Return→Save/Load→Continue；不是仅“能保存继续”或好玩验收。

V17 [关闭回执](../DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closure-Receipt.md)及最新 Handoff 保持有效。整合文档原有 V17 Blocked 行为历史结论，已由后续关闭回执取代；不因此把 FP Gate 顺带勾选。

## 实际操作与证据

本次有效运行由 Codex Computer Use 直接点击：没有调用私有处理器、没有直接 NewGame/EnterTown/FinalizeReturn、没有 Mock、TickDay、伤害/资源/HP/Node 注入，也未使用调试/对照按钮。只读源码追踪和测试槽 JSON 检查属于诊断证据，不能替代下表正常路径。

| 步骤 | 正常操作 | 预期 | 实际与证据 |
| --- | --- | --- | --- |
| N0 | Unity 正式 SampleScene 的 Play 按钮 | 出现正式主菜单 | 主菜单出现，新游戏可用，Continue 因隔离空槽灰色。先关闭了插件更新提示，未安装或升级。 |
| N1 | 点击“新游戏” | 进入既有起始流程 | 显示英雄选择，点击英雄卡，确认选择由灰变可用。 |
| N2 | 点击“确认选择” | G7：Tutorial Expedition→首次返程 | **实际直接 Home**，见 [01](../../Development/StudioRuns/FP-NormalPath-20261106/screenshots/01-newgame-direct-home.png)。新游戏/村庄建立成功，但不是首次 Return。 |
| N3 | Home 底栏点击“远征↗” | 正式准备页 | 页面出现角色卡和选中人物、英雄及 Supply 提示；当前 Food=0，预计 Carry=4，实际0，第一步成本2，见 [02](../../Development/StudioRuns/FP-NormalPath-20261106/screenshots/02-prep-food-zero.png)。这是独立分支，不填补 N2 缺失。 |
| N4 | 点击灰色“开始冒险” | 满足条件才出发 | 仍在准备页，见 [03](../../Development/StudioRuns/FP-NormalPath-20261106/screenshots/03-start-blocked.png)。未创建/推进一次有效 Expedition；此处停止该分支。 |
| N5 | 点击“返回领地” | 取消准备回 Home | Home 可见；**不是 Expedition Return**，不能用来证明 Return 结算。 |
| N6 | Home 设置→保存并返回主菜单 | 隔离测试世界保存后回菜单 | 返回主菜单，Continue 可用，见 [04](../../Development/StudioRuns/FP-NormalPath-20261106/screenshots/04-saved-to-main-menu.png)。 |
| N7 | 点击 Continue | 正式 Load→Home | Home 重新出现，建筑/人物显示、Food0；见 [05](../../Development/StudioRuns/FP-NormalPath-20261106/screenshots/05-continue-home.png)。是保存继续子路径，不是完整 G7。 |
| N8 | 点击 Editor Play 退出 | 结束隔离运行，不保存资产 | 返回非 Play 的编辑场景；测试槽移出，用户原文件原子归位，字体门禁收尾通过。 |

Home UI 仿真时间观察为 00:02→取消准备后的00:25→Continue后的00:54；测试槽保存 `CurrentWorldMinute=52`。表明本段时间在推进/恢复，但没有跨日或人物生活长期连续性验证。测试档记录 5 Character+1 Hero、7 个完成建筑（Farm1/Kitchen1/House5）、Food/Ingredients均0。人物与建筑 ID 可在 [隔离测试槽](../../Development/StudioRuns/FP-NormalPath-20261106/normalpath-test-save.json) 查阅；本轮未对 Continue 前后全部运行时 ID 作独立双快照对账，不能夸大成完整 Person 连续性证明。

## G1–G7 本轮判定

| Gate | 本轮证据 | 结论 |
| --- | --- | --- |
| G1 Battle Core | 没有到 Battle；历史 E3 有 Debug 强伤害/第二战抽牌检查边界 | **未验证**，不重做已实现战斗，也不把历史作为本轮通过。 |
| G2 Expedition Core | 正式 Prep 可达，补给提示/出发限制实际生效 | **部分证据**；Node/Branch/Time/Supply消耗、SafeReturn/Retreat/Failure未跑。 |
| G3 Person Continuity | 正常新游戏人物可见、准备页读取人物、Continue可回Home | **部分证据**；Battle/Return同一Person与长期结果未验证。 |
| G4 Home Exists | 正常Home含人物、建筑/固定场所、时间与导航；V17验收可限定复用 | **Home可达有本轮证据**，不是该Gate的全部世界理解/行为要求已通过。 |
| G5 Coupling | Prep读取Village Food并限制Carry | **部分证据**；没有实际出发、返程资源/人物结果回流。 |
| G6 Sustainability | 无第一次有效Return、无第二次远征或Return；独立Prep在Food0停住 | **未验证/本路径受阻，不记PASS**。不能由取消准备/再次开准备推导循环可重复。 |
| G7 End-to-End | NewGame跳过Tutorial直达Home；后段Save/Continue单独跑通 | **规定链未走通**；首缺口为Tutorial入口，后续完整链未测，不能按分段拼成PASS。 |

## O1–O14 原操作清单覆盖

| O | 原要求短名 | 本轮覆盖与边界 |
| --- | --- | --- |
| 1 | Village时间与人物持续运行 | 时间推进可见；完整持续人物行为未测。 |
| 2 | 安排人物 | 本轮未安排；复用V17对应范围验收，不冒充新操作。 |
| 3 | 至少一个Assignment/Project | 本轮未新增或完成，初始Completed建筑不算玩家Project。 |
| 4 | 时间推进 | 见N2/N5/N7；无手动Tick，无跨日长跑证明。 |
| 5 | 事实经Pattern/Recognition形成回应 | 未触发；历史E1专项有限复用。 |
| 6 | Village Event出现 | 未触发；编辑态旧测试面板不算正常游戏事件。 |
| 7 | Choice可见后果 | 未执行。 |
| 8 | 合法选人/Project互斥 | 准备页可见选人和默认选中；本轮未测试占用互斥或完成出发。 |
| 9 | Existing Battle/Run | 未进入。 |
| 10 | SafeReturn | 未执行；取消准备不能代替。 |
| 11 | HP/Exp/Pending收益回流 | 未执行。 |
| 12 | Village状态变化 | 时间变化和保存继续可见；Away历史汇合未执行。 |
| 13 | Capability改变Pool/未来可能 | 未发生，历史E2含测试Project和注入边界。 |
| 14 | 下一次远征读取变化 | 未执行；本轮只到准备，不能替代下一Run。 |

历史 E1–E6 的适用范围保留于整合文档；E2有手动Tick/HP注入且Return为Retreat，E3有9999伤害，不并入本轮正常证据。

## 首阻塞、只读原因和缺口分类

| ID / 分类 | 已确认事实、依据和影响 | 不是已确认的结论 |
| --- | --- | --- |
| P1 **入口未接** | `HeroSelectionUI.OnConfirm`（约179–207行）创建/保存新游戏后直接 `townUI.Initialize`；`MainMenuUI.OnNewGame`无英雄UI的兼容分支也直入Town。与N2截图吻合。G7 Tutorial→首次Return缺失；Battle-first方向已有设计依据。 | 未证明全部教程内容/所有其它入口都不存在；具体教程剧情/内容仍未冻结。 |
| P2 **前置条件未满足；已有功能缺完整路径证据** | `InitialWorld.json` 默认 InitialFood/Ingredients=0；`ExpeditionUI.RefreshPrepSupplyAndPreflight`（约1046行）令 actualCarry=min(Food,requested)，不足firstStepCost就禁用确认；`OnConfirmExpedition`约470行有相同守卫。N3/N4实际0<2。影响本条“新游戏直接出征”分支。 | 没有证明Guard有Bug、Farm/Kitchen不能生产，或必须增加初始食物；源码明确引导先生产。需要正常派工/生产后的后续运行证据。 |
| P3 **已有功能缺证据** | Battle/事件、真实返程、第二Run、人物变化、Capability和社会回应链均未在本次正常路径验证。 | 不把未执行写成Missing，不安排重建已验证底层。 |
| P4 **表现风险，待确认** | 本次GameView中准备页左标题/卡面文字有重叠/裁切，见02/03。 | 尚未排除Editor固定分辨率/预览缩放与Prefab表现影响，不能直接定为阻断性功能缺陷。 |
| P5 **尚未冻结的设计问题** | 双教学职责已采用；具体初始Tutorial内容、第一/第二轮正式内容选择、准备教学/供给节奏尚需确定。 | 不重新讨论Battle-first是否采用，不自行冻结人物、剧情、奖励/成本数值。 |

本轮没有确认需要改代码的运行异常。P2不是“初始内容已证明配置错误”；P1的精确实现选择应在规格中约束，不能因此擅自生成Tutorial系统。

## 最小后续顺序与建议范围（本轮未实施）

1. **先完成已有Home→生产→出征的正常证据链**：通过现有管理UI合法派工、等待实际到场/生产并积累食物，再重走Prep/Battle/Return/第二Run。复用现有存档隔离，不加粮、不改速度规则、只用玩家已有速度按钮。若走通，P2只补证据而非开发需求。
2. **为P1确定最薄Tutorial/首次Return接线规格**：范围从MainMenu/HeroSelection既有宿主到既有Expedition/Return接缝，复用当前Battle/Expedition规则；制作人/设计顾问先选代表内容与成功边界，不另建第二套Run/Save或写V18 GameSpec。
3. **在代表内容和正常准备条件具备后复测G6/G7**：同一具体Person、实际战斗/事件、实际Return、Save/Load→Continue→再次Prep/Run/Return；逐项留证，不能拼用取消准备或历史注入结果。
4. **仅修复新发现的真实阻断**：必要时检查准备页可读性；先正常分辨率复现再决定是否改Prefab/UI。社会回应/Capability等从现有E1/E2系统选择正常内容，未验证前不扩展完整人格/叙事/经济。

需要制作人裁决的仅是Tutorial具体内容与边界，以及正式准备教学/供给的目标体验；是否沿用“先派工生产”的首轮节奏应由证据与设计决定。本轮不请求增加库存或更改平衡。其它普通技术核验由开发处理。

## 保护、执行限制与交付状态

- 原档：28,994B，SHA `89625eef3ce43cb02d55dcc5d93b74c1e46acf0b30a49ceda903cb3e1c424948`。运行前完整备份及SHA核验，同目录原子改名封存；运行后先移动测试档，再原子归位原档。**原档没有被游戏写入，没有以覆盖方式恢复**；归位SHA/字节相同，封存和.tmp残留均无。记录见本目录外保护回执及项目证据 `protection.json`。
- 主开发原文件读取最初被沙箱拒绝；仅针对已授权的存档保护/归位命令使用受审查提升，未改ACL或扩大持久权限。未读取私有配置/凭据。
- 字体Label `fp-codex-normalpath-01`，begin/end=0，end PASS，SHA维持 `7b5717b976bac0d90dce70ba65b4dda0995bf78e8b3fd317ea9918058109c016`。旧incident不删；未SaveAssets/场景，不恢复字体。
- 两次DSH派发均真实受理并结束，但均以预算窗口为由未运行，原 `progress.md`/连接回执是**未执行记录**；有效截图和正常运行是主开发Computer Use完成，不能归为DSH已通过。没有擅自修改连接器或扩展工具链。
- 开工4783项文件SHA基线与范围外核验见 `D:/AI_Agent_Lab/audit-results/fp-normalpath-20261106/`；必要新增报告/状态引用与证据单列。Git HEAD/index不变，未Commit/Push/Merge，原dirty成果保留。
- 未执行：真正首次Return、第二次远征、Battle/事件/结算、SocialResponse/Capability链、充分供给后的G6、全量测试/Builder/编译重跑、Player和市场/乐趣验收。没有以只读源码或诊断替代正常路径。

本报告供制作人评审；V17维持Closed，FP整体不判通过。本次完成后停止，不实施建议修复。
