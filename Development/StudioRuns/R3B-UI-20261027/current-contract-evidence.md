# R3-B 当前交付 contract `requiredChecks` 证据与缺口（2026-10-27 ESC 采样修后 16 例）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 本文件性质：**只读核对 + 文档证据**。按父 contract 的 `requiredChecks` **原 id / 原类型**（`execution(real)`、`regression(real)`、`design-audit(analysis)`）逐项核覆盖，**不新增门槛、不降低门槛、不 mark Completed / awaiting_acceptance**。
- 版本口径：**2026-10-27（双槽 15 例集成后）**。本文正文即当前口径；2026-10-27 早段（14 例）与 10-26 及更早的数字一律标“历史”，不删改。
- 核对时刻：**2026-10-27T02:42Z 前后**（本机 `10:42 +08:00`，China Standard Time）。
- R3-B 状态不变：**Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；父子交付保持 running 非终态。

---

## 0. 2026-10-27 当前口径（ESC 采样修后 16 例；覆盖 §1–§8 的 15 例当时口径）

> 本节为**当前结论**；§1–§8 保留 2026-10-27 双槽 15 例时的原文，不改写。冲突时以本节为准。

| # | requiredCheck id | 类型 | 当前条件结论 | 依据 / 仍缺 |
|---|---|---|---|---|
| 1 | `execution` | `real` | **已覆盖（条件：16 类 run 级元数据待主开发补）** | 管理 Play **16/16 Passed、逐条 `message=null`**（method 合计 **14 634 ms**；日志 L4139–L4374：16 组 `[ENTRY]` / 16 组 `[SAVE-RESTORE] matches-before=True` / 4 组 `[SCHEDULE-RESTORE]` / `[FONT-POST]` / `[BASELINE-POST] 34/34`）+ 管理 Edit **19/19、0 skipped、1.1578404 s**（`99761145…`）+ ESC 单例 **1/1、0 skipped、2.0916182 s**（`cc3adfc5…`）。**runId / skipped / run 时长本层不可见 → 待主开发补** |
| 2 | `regression` | `real` | **已按“实际改动的必要回归”覆盖**（不等于最新全集重跑） | 本轮产品改动仅 `HomeProductShell.LateUpdate` ESC 采样（+ Editor-only internal seam）；受影响面为管理 Play 帧路径 / 管理 UI / 命令 / 双槽 / SaveLoad，已由 16 Play + 19 Edit + 保护面覆盖。264 / 1513 / 122 未重跑且**不作门槛**；历史回执仍真实但属前阶段 |
| 3 | `design-audit` | `analysis` | **已覆盖（分析）+ 真实缺口列明** | ESC 已从“仅 public 回调”升级为**真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁**（证据与边界见 `final-professional-handoff.md` §6）；**OS 物理键 / `Input.GetKeyDown` 引擎实现 / 物理鼠标手感 / IME / DPI / Player 面 / guard 进程级 / UI 文案像素仍未覆盖**，不冒称 |

**当前被取代的旧表述（§1–§8 原文保留）**：

- §1 第 1 行 `execution` 的“15 Play 15/15”：当前为 **16/16**；§2.1 的 15 例汇总与 §2.2 的 15 条明细为 15 例当时口径。
- §1 第 3 行 / §4.3 的“真实键盘轮询未覆盖”：当前已由第 16 例覆盖到**产品层真实 `LateUpdate` 采样路由**；仍不覆盖 OS 物理键与 Input 引擎实现。
- §3.3 第 1 项“真实键盘轮询”仍作为**物理 / 引擎边界**保留；其“未覆盖”升级为“产品层机制已覆盖、OS / 引擎未覆盖”。
- §4.4 的“最小技术方案（未做）”：已按该方案实施（实例级 `internal` 一次性注入 + 1 个 Play 用例），**不再列为待授权方案**；实施证据见 `escape-frame-routing-review.md` 与 `current-sixteen-preflight.md`。
- §2.3 / §4.2 的 15 例证据行：**历史**；第 16 例整类证据 L4231–L4239 为当前。

---

## 1. 逐项结论（按父 contract 原 id / 原类型）

| # | requiredCheck id | 类型 | 当前结论 | 一句话依据 |
|---|---|---|---|---|
| 1 | `execution` | `real` | **已覆盖（真实宿主执行）** | 15 Play（**15/15 Passed、`message=null`**，方法合计 13 996 ms）+ 19 Edit（`99761145…` 19/19、0 skipped、1.1578404 s）+ 双槽单例（`33b8271a…` 1/1、0 skipped、2.6883936 s）+ 输入单例（`df6c6645…` 1/1、0 skipped、2.6441609 s） |
| 2 | `regression` | `real` | **已按“实际改动的必要回归”覆盖**（充分性逐项说明见 §3）；**不等于**“最新版本全集已重跑” | 本次改动的影响面被完整枚举并全部取得当前版本真实回执：管理 Play 15/15（每个用例帧循环都经改动后的 `HomeWorldCamera` 采样路径）、管理 Edit 19/19（管理命令 / 投影 / 双槽 Gateway）、保护面 34 / 字体 / 单槽 / guard 全一致。**264 / 1513 / 122 未重跑，且不构成本次完成门槛**（理由见 §3.2–§3.3） |
| 3 | `design-audit` | `analysis` | **已覆盖（分析）+ 具体缺口列明**（见 §4） | 规格 §4 / §5 / §8 逐项对照；双槽真实 Gateway / UI / SaveLoad、`CapacityFull` 领域诊断、建造态 ESC 公开回调现已覆盖；**真实键盘轮询、物理鼠标 / 焦点 / IME、Player 构建内行为仍未覆盖**，明确不冒称 |

> 结论不是 `Completed`：`design-audit` 仍有**真实未覆盖项**（§4.3），且 15 类 runId / skipped / run 级时长本层不可见（待主开发补）。因此**不写** `Completed` / `awaiting_acceptance`，交付 Evidence 若需对外只能标 **partial**。

---

## 2. `execution(real)`：当前真实执行回执

### 2.1 汇总

| 执行 | 结果 | 时长 | skipped | 证据来源 |
|---|---|---|---|---|
| **管理 Play 整类（当前，15 例）** | **15/15 Passed**、逐条 `message=null`、无 Failed | method 合计 **13 996 ms**（范围 567–1554 ms）；日志段 `02:38:32.155Z–02:38:46.580Z`（≈14.43 s，**纯日志行跨度，不是 run 时长**） | **本层不可见 → 待主开发补** | 本机 `testResults` 15 条；主证据日志 `management-shell-play-evidence.log` **L3886–L4108**；guard 新增 15 条 restored |
| 双槽单例（上一轮，仍有效） | **1/1 Passed、0 skipped、2.6883936 s、scope-restored** | 2.6883936 s（run 级） | **0** | 主开发原始核 `33b8271a-79c1-45d1-b6c3-c9fa6261de63`；本机日志 L3861–L3885 |
| 管理 Edit 整类（当前，19 例） | **19/19 Passed**、`message=null` | run 级 **1.1578404 s**；method 合计 96 ms | **0** | 主开发原始核 `99761145-ddc9-43d9-a3eb-db6706e59acf`；本机 `testResults` 19 条；scope-restored |
| 输入门禁单例（上一轮，仍有效） | **1/1 Passed** | run 级 **2.6441609 s** | **0** | 主开发原始核 `df6c6645-56ce-4b85-92d0-3e4bf8aac552`；日志 `02:34` 前段 |
| 管理 Play 整类（14 例，历史） | **14/14** | — | — | 主开发已核 `46d65625`；**历史**：测试源码 `2026-10-27T02:32:36Z` 之后新增第 15 例，14/14 不再代表当前版本 |

**运行级信息缺口（不编造）**：15 例整类的 runId / skipped / run 级时长在本层 `testResults`（字段仅 `name / status / durationMs / message / stackTrace`）与日志中都读不到；`tests-run` 工具调用级耗时也未被本层记录。**统一写“待主开发补”**，不把 method 合计或日志跨度写成 run 时长。

> **2026-10-27 当前（16 例）**：本节与 §2.2 的 15 例数字为 15 例当时口径；当前为 **16/16 Passed**，方法合计 **14 634 ms**，日志 L4139–L4374（§0）。16 类 runId / skipped / run 时长同样**待主开发补**。

### 2.2 15 例逐条（`testResults` 当前可见）

| # | 用例（`RRPG.ManagementShell.Tests.ManagementShellPlayTests.*`） | status | durationMs |
|---|---|---|---|
| 1 | `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput` | Passed | 939 |
| 2 | `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement` | Passed | 969 |
| 3 | `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` | Passed | 1554 |
| 4 | `FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt` | Passed | 617 |
| 5 | `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` | Passed | 630 |
| 6 | `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` | Passed | 773 |
| 7 | `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable` | Passed | 929 |
| 8 | `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState` | Passed | 847 |
| 9 | `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` | Passed | 567 |
| 10 | `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput` | Passed | 1242 |
| 11 | **`ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts`** | Passed | 1312 |
| 12 | `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite` | Passed | 802 |
| 13 | `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` | Passed | 598 |
| 14 | `SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous` | Passed | 1416 |
| 15 | `WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes` | Passed | 801 |

- 15 条全 `message=null`；合计 **13 996 ms**。
- 日志段结构（L3886–L4108）：**15 组 `[ENTRY]`**、**15 组 `[SAVE-RESTORE] matches-before=True`**（`20261027T023832Z` … `20261027T023845Z`，含 `-2` 目录）、**4 组 `[SCHEDULE-RESTORE] restored=true sameInstance=True`**（CASE-B / REAL-SCROLL / **CASE-E** / EVENT-GATE）、`[FONT-POST] unchanged=True`、`[BASELINE-POST] total=34 same=34 changed=0 problems=0`。
- 与 14 例段相比，本轮多出的一组 `[SCHEDULE-RESTORE]` 即 CASE-E（第 15 例）自身捕获 / 恢复，说明临时覆盖与日程隔离在同一 fixture 内**双向干净**。

### 2.3 第 15 例（双槽）在整类中的真实证据（L4018–L4035）

| 证据行 | 关键值 |
|---|---|
| `CASE-E START` | `baselineRegistry=263chars override=<null>`（开工无残留覆盖） |
| `CASE-E OVERRIDE` | 真实目录 + 末尾追加 `building_farm:aux:Workplace:ws_test_02`（仅内存） |
| `CASE-E SLOTS` | `wMain=workplace:building_instance_1:main`、`wAux=…:aux`、`main.capacity=2 aux.capacity=1` |
| `CASE-E FULL` | 两槽 `CapacityFull=True`、占用 `1 / 2` 不变、`no-write=True` |
| `CASE-E LABOR` | `main.bound=2 labor=2 onsite=2`、`aux.bound=1 labor=1 onsite=1`、`40/40/40`、`cross=0`、`clock=16360` |
| `CASE-E SAVE / CONTINUE` | `clock=16360 ingredients=14 food=0 batches=1 evidence=3 completed=3`；两槽身份与绑定人集合逐项一致 |
| `CASE-E ESC-BUILD / ESC-CLOSE` | `Build.IsOpen` 时 `HandleEscape` 不关建造、不进 CloseSurface；公开 `Build.Cancel` 后才关当前 Surface |
| `CASE-E END` + `[SAVE-RESTORE]` + `[SCHEDULE-RESTORE]` | `override=<null>`、`defaultRegistryRestored=True`、单槽 `matches-before=True`、日程 `sameInstance=True` |

边界：这些是**断言通过后**写出的摘要行；判定以源码断言为准。ESC 仍是 **public 回调 / 取消顺序机制**，不是物理键，也不是 `Input.GetKeyDown` 轮询路径。

---

## 3. `regression(real)`：按实际改动的必要回归（逐变更影响说明）

### 3.1 本次改动面与影响枚举

| 变更 | 影响面（谁会被改到） | 当前版本证据 | 为什么足够 |
|---|---|---|---|
| `ManagementShellPlayTests.cs` 新增第 15 例（测试源码 `446994B / 05d6de11…d1d8a`） | 该 fixture 全部 15 例的运行环境（static registry 覆盖、Scene 重载 + Continue 重建 host、日程隔离、存档 guard 链） | **15/15 Passed**（§2） | 第 15 例会改写同 fixture 的静态 registry 与场景引用，**只有整类串行**能证明前序用例不污染它、它不污染后继用例；单独跑不构成该证明 |
| `HomeWorldCamera.cs`（`5077B / 39caba4b…`，2026-10-26 引入采样缝） | `ProductHomeWorldHost.Update` **每帧**调用 `ReadInput` → 所有进入真实 Home 世界的 Play 用例 | 管理 Play 15/15；生产路径为 `_inputSampleForTests==null` 时回落到**逐句相同的 legacy Input 采样**（`Input.mousePosition` / `GetMouseButton(1|2)` / `GetMouseButtonDown(1|2)` / `mouseScrollDelta.y`） | 改动是**纯取数重构 + Editor-only 条件分支**；门禁（`overUI` / `suppressRightPan`）与 `Pan/Zoom` 调用未变，且被 15 例的模态 / 输入门禁 / 阳性对照逐帧覆盖 |
| `ManagementView.cs`（`39037B / f8316e43…`）+ `ManagementView.prefab`（`282332B / 31bf3e8c…`，2026-10-26 行明细 / 行高） | 管理三子页渲染与行内容 | 管理 Play 15/15（含布局例、滚动例、双槽行）；管理 Edit 19/19（投影 / 命令，不渲染） | 渲染改动只被管理页消费；对应 Play 用例逐字符断言 + 真实滚动 + 双槽两行并存 |
| `StaticConfigRegistry` Editor-only `Set/GetBuildingsOverrideForTests`（`29248B / 1c0c62fa…`） | 仅 Editor 编译；只影响显式调用者 | 管理 Edit 19/19（第 19 例）+ 管理 Play 15/15（第 15 例） | 默认无覆盖（本轮实测 `override=null`、默认 `FunctionBindings` 3 条全 `main`、**无 aux**）；两处调用都在 finally / TearDown 恢复 |
| 产品编译产物 | `Assembly-CSharp.dll` `649216B / f80f8c80…`（晚于相机源码 59.20 s）、`BattleStage1.PlayModeTests.dll` `547840B / b8dae733…`（晚于测试源码 3.86 s） | 本轮 15/15 + 19/19 真实运行即证明**当前源码可编译、可加载、可执行** | 编译有效性不需另跑编译门禁来证明 |

### 3.2 为什么**不**把 264 / 1513 / 122 重跑当作完成门槛（不臆造要求）

| 门禁 | 它实际测什么 | 与本次改动的关系 | 结论 |
|---|---|---|---|
| Compiler **264** | 宿主 `.NET` 工具链门禁（`dotnet build/test` + `rrpg-config validate`），不是 Unity 产品脚本单测 | 本包未改该工具链 / 配置 / manifest | **非必要回归**；如需重跑属“工具链门禁复核”，另立包 |
| 完整 Edit **1513** | 全工程 EditMode | 本次改动不触及 Person / Growth / Social / History / Battle / Encounter 等领域；管理相关 Edit 只有 `ManagementProjectionTests` 一个类（已 19/19 覆盖） | **非必要**重跑全集；受影响编辑面已被 19/19 覆盖 |
| 完整 Play **122** | 全工程 PlayMode | 本次新增 / 改动都落在管理 Play fixture 与其运行环境；其它 Play 类不经管理页 | **非必要**重跑全集；管理面已被 15/15 覆盖（可选加强项见 §3.3） |

> 上述 264 / 1513 / 122 的**历史回执仍为真实**（`5eb0ad4c` / `f116b56e` / 宿主普通用户 264），但它们是**前阶段版本**，本文件既不把它们写成当前版本通过，也不把它们设成本次的完成门槛。

### 3.3 若审阅人仍要求更宽的运行时回归：具体受影响类（可选，非必要）

以下 Play 类会在真实帧里进入 `ProductHomeWorldHost` 并因此**每帧经 `ReadInput`**，是相机采样重构除管理面之外的唯一受影响集合。若要把“纯取数重构”从代码论证升级为运行时证据，**最小额外回归**就是这 6 个类（属原 122 全集子集，不是新全集）：

`ProductHomeFoundationPlayTests`、`VillageVisualPrototypePlayTests`、`BuildingConstructionProductPlayTests`、`BuildingResourceProductPlayTests`、`BuildingVisualReplacementPlayTests`、`R3AGrowthTransactionPlayTests`

- 这些都是**既有原测试类**，不改断言、不新增用例；成本为一次串行 Play 子集运行，仍远小于 122 全集。
- 本包**未运行**它们（约束：模型不 `tests-run`；且任务明确不为换数值重跑全集）。**因此当前 regression 的“必要性”结论建立在“必要影响面已全覆盖 + 采样重构可论证等价”上，而非“全工程最新版本均通过”。**

---

## 4. `design-audit(analysis)`：规格对照与仍未证

### 4.1 已覆盖（当前版本真实证据）

| 规格 / 要求 | 结论 | 证据 |
|---|---|---|
| §4 职责与场所：一行一 `WorkplaceId` + `BuildingInstanceId` + `FunctionSlotId`，容量 / 绑定 / 劳动 / 现场分列 | 已覆盖，**含同一实例 main + aux 两行** | 第 15 例 `SLOTS` / `LABOR`；行模板 132 / `1072×76` |
| §4 施工只走 `TryAddConstructionWorker` / `TryRemoveConstructionWorker` | 已覆盖 | CASE-B 与第 15 例（真实“加入施工”按钮 → 完工自动注册） |
| §4 名额（长期绑定）与现场 / 实际劳动独立 | 已覆盖，**按 slot 独立** | 第 15 例 `main.bound=2 labor=2 onsite=2` / `aux.1/1/1` / `cross=0` |
| §4 满员语义 | 已覆盖：**领域诊断 `CapacityFull` + 原 UI 满员禁用** | 第 15 例 `FULL` 行 + 源码断言（`DomainRejected` / `Diagnostic=="CapacityFull"` / 无写入 / 按钮 `interactable=false`） |
| §4 管理命令成功后显式保存并显示 bool | 已覆盖 | CASE-C 与第 15 例 `SAVE`（真实按钮 + `SaveSystem.HasSave` + `SaveVersion` JSON） |
| §5 关闭返回 World 且保留有效选择；ESC 与关闭同一回调；嵌套不抢底层 | 已覆盖（**回调语义**） | 关闭例、输入门禁例、第 15 例 `ESC-BUILD` / `ESC-CLOSE` |
| §5 全屏 Raycast Blocker + World 输入阻断 | 已覆盖（**程序化 Raycast**） | 模态例 / 输入门禁例 |
| §5 TopHUD 在模态允许区可操作 | 已覆盖 | 模态例 |
| §5 内部页面不 `SetSimulationActive(false)` / 不 Pause / 不改 Speed | 已覆盖（三表面等价 + 多例） | `SurfaceEquivalentSimulation`、各例 `simulationActive=True` |
| §8 滚轮 / 拖拽不穿透 + 长列表可读 + 末行可达 | 已覆盖（**机制级**：合成 legacy Input 经真实 `Host.Update` + 真实 `IScrollHandler` / 多轮拖拽） | 输入门禁例 `POSITIVE/MODAL/AFTER-CLOSE/CLEANUP`；滚动例 |
| §8 1920×1080 可读 | 已覆盖 | 布局例（1280 已取消） |

### 4.2 本轮由“未覆盖”转为“已覆盖”的项（历史结论明确标历史）

| 项 | 历史口径（保留原文） | 当前 |
|---|---|---|
| 第二 FunctionSlot 真实 Gateway | 2026-10-26：“配置只登记 `main`，第二 slot 未覆盖” | **已覆盖**：Editor-only 内存覆盖 + Edit 第 19 例 19/19 + Play 第 15 例（`CapacityFull` 双路零写） |
| UI 双档位 / Save-Load 双 slot | 2026-10-26：“UI / 存档双档位未覆盖” | **已覆盖**：真实行点击 + 真实派工按钮 + 真实保存 + 真实 Continue 换源，逐项等值 |
| `CapacityFull` 域内诊断 | 2026-10-26：“按钮禁用已断言，诊断文案未单测” | **已覆盖（领域诊断值）**；UI 侧仍只断言按钮禁用，**UI 文案像素级未单测**（不冒称文案已验） |
| 建造态 ESC 路由 | 2026-10-26：“未构造建造打开 + ESC” | **已覆盖（公开回调 / 取消顺序）**；**物理 ESC 键仍未覆盖** |
| 日志里 `CapacityFull 域名诊断未经按钮进入`（CASE-A 段） | 旧 13/14 例报告据此判“未测” | 该括注**只对 CASE-A 成立**（CASE-A 只做 UI 禁用）；整类运行时它会再次打印，**不得据此判仍未测** |

### 4.3 仍未覆盖（真实存在，不列为“需批准”，也不写成已验）

| 项 | 规格 / 源码依据 | 现状 | 性质 |
|---|---|---|---|
| **真实键盘轮询**（`HomeProductShell.cs:95 Input.GetKeyDown(KeyCode.Escape) → HandleEscape()`；`HomeBuildingPlacement.cs:102` ESC/右键取消；`CharacterGrowthUI.cs:125`） | 规格 §8 含“键盘不穿透” | **产品层机制已覆盖（2026-10-27 第 16 例）**：`LateUpdate → SampleEscapePressed → HandleEscape` 由真实帧执行、注入一次消费一次、3 个阴性帧不重放、`Build.IsOpen` 让路；**仍未覆盖**：OS 物理 ESC、`Input.GetKeyDown` 引擎实现、按住重复；建造自身 ESC / 右键未注入 | 产品层路由已有运行证据；物理 / 引擎边界真实存在，不冒称 |
| 物理鼠标 / 滚轮 / 拖拽手感、DPI / 焦点 / 多指针时序 | 规格 §8 只要求“不穿透”，**未把“手感”列为验收项** | 未覆盖；合成快照 + `EventSystem` 只到机制层 | **不得作为原规格新增强制验收**；如需体验评估属制作人人工 |
| Player 构建内行为 | 采样缝、registry 缝全部 `#if UNITY_EDITOR` | 仅静态保证；双槽用例依赖 Editor-only 缝，Player 内不存在 | 未构建；如需要须独立 Player 构建授权 |
| UI 满员文案像素 / `CapacityFull` 展示文本 | 规格只要求给已有诊断 | 按钮禁用已断言，文案未单测 | 低优先、普通可测 |
| `SaveSlotGuard` 进程级 abort | 现有 guard 只在用例收尾 / `OneTimeTearDown` | 未覆盖进程级崩溃 | 需单独授权设计 out-of-process 兜底 |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过；不由 1920 外推 | 已决定，不再询问 |
| 历史 650 存档（`21075B / 650c45c4…`） | 2026-10-25 记录 | 授权内无备份，未恢复、未伪造 | 保持缺口 |
| 两次历史边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`） | 既有文档原文 | 原文留存 | 不洗白、不回滚 |

### 4.4 若“真实键盘轮询”被视为必须实证：可自主补的最小技术方案（供主开发决定，本包未做）

1. **最小代码方案（与现有采样缝同风格）**：在 `HomeProductShell.Update` 的 ESC 轮询处加一个 `#if UNITY_EDITOR` 的实例级 `internal` 可注入布尔（`escapePressedForTests`），`Update` 内改为 `if (Input.GetKeyDown(KeyCode.Escape) || ConsumeEscapeForTests()) HandleEscape();`；Player 编译内该字段 / 方法 / 分支全部不存在，生产语义不变。
2. **配套用例**：一个 Play 用例注入“ESC 按下一次”，断言 `HandleEscape` 被调用一次、管理模态只关顶层、`Build.IsOpen` 时不清候选、关闭后不再重复触发。成本约 1 处 seam + 1 个用例。
3. **替代方案（不改产品代码）**：由宿主普通用户在 Editor / Player 窗口用 OS 级按键注入做一次性实证并留日志；但这是宿主 / 人工动作，不属于自动化回归。
4. 两种方案都**需要另行授权**（方案 1 属代码修改）。本包**未实施**，也未用回调证据冒充它们。

---

## 5. 机制实证 vs 物理设备（严格分栏，不合并）

| 结论 | 可以证明 | 不能证明 |
|---|---|---|
| 非模态阳性对照 | 合成右键拖动 + 滚轮经真实 `Host.Update` → 同一 `ReadInput` 产生**非零** `Pan` / `Zoom`（`size 7.5→6.4`） | 物理设备输入、手感、灵敏度 |
| 管理 / 人物模态阻断 | 相同序列在真实帧内不改变相机 / Selection / Clock / 建造候选 | OS 级穿透、焦点 / DPI / IME |
| 关闭后可逆 | 同一输入在关闭后再次生效 | 快速连点 / 拖拽中断等真实时序 |
| ESC / 建造 ESC | `HandleEscape` 回调在 `Build.IsOpen` 时不关建造、不进 CloseSurface；`Build.Cancel` 后只关顶层 | 物理 ESC 键、`Input.GetKeyDown` 轮询、按住重复 |
| 双槽 UI / SaveLoad | 真实 Button.onClick 路径 + 真实 `SaveSystem` 读写 + 换源重建 host | 物理鼠标点击手感 |
| Player 面 | 源码 `#if UNITY_EDITOR` 静态排除 | 实际 Player 构建运行结果 |

---

## 6. 运行后保护面（2026-10-27T02:41Z 前后只读实测，15 例运行后）

| 项 | 实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / problems=0` | 全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、`.tmp=False` | 内容未变（mtime 由恢复回写产生） |
| guard | `275 = 274 restored + 1 protective-snapshot`；最新 restored `20261027T023845Z`；`readbackVerified=true` 全覆盖；`noManifest=0 / other=0` | 合法终态，无 unfinished |
| 场景 / Editor | `SampleScene dirty=False`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080` | 干净空闲 |
| Console | 产品 `Error/Exception` **0** | 无产品异常新增证据 |
| 源码 / DLL | `ManagementShellPlayTests.cs 446994 / 05d6de11…`；`BattleStage1.PlayModeTests.dll 547840 / b8dae733…`（晚 3.86 s）；`Assembly-CSharp.dll 649216 / f80f8c80…`（晚相机源码 59.20 s） | 未改、未 Refresh |

---

## 7. 边界与诚信声明

- **模型未运行 `tests-run`**：15 例由宿主 Studio 阶段 check 实际运行，本层只读 `testResults` / 日志 / guard / 保护面；未 Refresh、未 `rql-mutate`、未 PrefabStage、未字体测量、未 `SaveAssets / SaveChanges / SaveDirtyScenes / ClearDirty / SetDirty`、未 Shell / Git / Preferences / 正式工程、未构建 Player。
- 未重跑 264 / 1513 / 122，也未把“重跑全集”写成完成门槛（§3.2）。
- 不把机制证据写成物理设备证据；不把工具调用 / method 合计 / 日志跨度写成 run 时长；不把历史回执写成当前版本回归。
- 历史违规与 650 缺口原文保留，不洗白（§4.3）。
- `requiredChecks` 结论：`execution(real)` 已覆盖、`regression(real)` 按实际改动已覆盖（全集未重跑，且不作门槛）、`design-audit(analysis)` 已覆盖但**真实缺口列明**。**不 mark Completed / awaiting_acceptance**。
- 本文件须由**主开发专业核验**，不代表制作人批准。

---

## 8. 写后只读复核

见 `Development/StudioRuns/R3B-UI-20261027/final-scope-and-contract-review.md` §8（本轮全部文档的统一写后复核：34 项 / 字体 / 单槽 / guard / 场景 / 源码指纹与各文档字节 + SHA256）。
