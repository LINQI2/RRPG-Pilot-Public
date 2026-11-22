# R3-B 当前 14 例管理 Play 集成预检（2026-10-27，阶段 1/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 本阶段性质：**只读核对 + 只新增本文件**。未改代码 / 测试 / Prefab / Scene / 存档 / manifest / 34 项基线；未 `AssetDatabase.Refresh`、未 `AssetDatabase.SaveAssets / SaveChanges / SaveDirtyScenes`、未 `SetDirty / ClearDirty`、未 `rql-mutate`、未 PrefabStage、未字体测量、未写 Preferences、未 Shell / Git 写、未启停 Editor、未构建 Player。**模型未运行 `tests-run`**。
- 日期口径：当前 **2026-10-27**；2026-10-26 的既有产物（`R3B-UI-20261026/*`、`R3B-UI-20261023/play-evidence/*.log`）**只读引用，不改写、不伪改历史**。
- 状态口径：R3-B 仍 **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；交付 contract 仍 **非 Completed / 非 awaiting_acceptance**。本文件须由主开发专业核验，不代表制作人批准。
- 上游已终态、本轮**不重派、不重复**：最近 `3026` / `b676` 执行已终态；主开发已原始核 `99761145-ddc9-43d9-a3eb-db6706e59acf`（管理 Edit 整类 **19/19 Passed、0 skipped、1.1578404 s、scope-restored**）与 `df6c6645-56ce-4b85-92d0-3e4bf8aac552`（输入单例 **1/1 Passed、0 skipped、2.6441609 s、scope-restored**）。本文件**不重跑、不重算、不改写**这两条回执。
- 核对时刻：**2026-10-27T02:18:43Z**（本机 `10:18 +08:00`，China Standard Time）。

---

## 1. 当前身份与安全基线（只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，可交宿主 |
| 场景 | `SampleScene`、`isDirty=false`、`roots=5`、`PrefabStage=null` | 干净，无游离 PrefabStage |
| 分辨率 | `1920x1080`（未改 1280；1280 由制作人 `01a53eb8` 取消，取消 ≠ 通过） | 保持 |
| 34 项磁盘基线 | `total=34 / problems=0`（逐项 bytes + sha256 与 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 全一致） | 全一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、mtime `2026-10-10T13:23:55.9520319Z` | 未变 |
| 字体内存态 | `LoadAllAssetsAtPath` 9 个子资产、`EditorUtility.IsDirty` **dirty=0** | 无内存脏写，未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、mtime `2026-10-26T11:08:10.0458894Z`、`.tmp=False` | 与既有单槽一致 |
| guard 目录 | `dirs=245 / restored=244 / protective-snapshot=1（20261024T052704Z）/ other=0 / noManifest=0`；最新 restored = `20261026T110808Z`（`readbackVerified=true`，manifest 内 `sourceSha256=4b2b56f6…d6aa`） | 无 unfinished / 无待人工取证项 |
| Console | 产品源码 `Error/Exception` **0**；Error 条目均来自本阶段只读探针自身的 `script-execute` 编译失败（`2026-10-27 10:18:39` 本机 `snippet.cs(26,71) CS0119 'TestRunnerApi' is a type`；写后哈希探针 `CS0201` 语句表达式），不涉及任何产品 / 测试源码；每个探针修正后只读通过 | 产品侧干净；探针失败如实留痕 |

> 与 2026-10-26 索引相比，guard 由 `244 = 243 + 1` 变为 `245 = 244 + 1`：多出的一条即输入单例运行 `20261026T110808Z` 的 restored 记录，属**新增实测**，不是历史改写。

---

## 2. 代码 / 测试 / DLL 新鲜度与 14 例发现（只读）

| 文件 | bytes | SHA256 | mtimeUtc | 判定 |
|---|---|---|---|---|
| `Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | **5077** | `39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` | `2026-10-26T11:04:08.7201777Z` | 与任务书一致 |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **408932** | `0025be07393e97462c1f57e429416fb2b9dcb888f06e8871a1448e6dd47332f1` | `2026-10-26T11:06:12.7673634Z` | 与任务书一致 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 525312 | `bd131c42ac05aa71c86e924051824b9f7ac559c1cccb17f48c13c901789ca3a4` | `2026-10-26T11:06:18.8461968Z` | **晚于源码 6.08 s，新鲜** |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 649216 | `f80f8c804f10d9d84248a1f1e7ea717c3ab5ca676521cd2af9f8946a6877c852` | `2026-10-26T11:05:07.9226979Z` | **晚于`HomeWorldCamera.cs` 59.20 s，新鲜** |

14 例发现用两条独立只读面交叉核对，结果一致：

1. `tests | where fullName starts-with "RRPG.ManagementShell.Tests.ManagementShellPlayTests."` → **14 条**，`mode` 全部 `PlayMode`；
2. 运行时反射该 fixture 的 `[UnityTest]` 方法数 → **14**。

| # | 用例 | 性质 |
|---|---|---|
| 1 | `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` | 既有 13 例（源码未改） |
| 2 | `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable` | 既有 |
| 3 | `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput` | 既有 |
| 4 | `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` | 既有 |
| 5 | `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput` | 既有 |
| 6 | `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite` | 既有 |
| 7 | `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` | 既有 |
| 8 | `WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes` | 既有 |
| 9 | `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` | 既有 |
| 10 | `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement` | 既有 |
| 11 | `FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt` | 既有 |
| 12 | `SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous` | 既有 |
| 13 | `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` | 既有 |
| 14 | `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState` | **本轮新增（输入门禁单例，已单独通过）** |

Edit 侧 `ManagementProjectionTests` 反射 `[Test]` 数仍为 **19**，`tests` 查询同为 19；本阶段**未改、未重跑**，只用于确认 14 例管理 Play 与 19 例 Edit 是两个独立集合。

---

## 3. 输入采样缝与 Editor 覆盖残留（只读核对）

| 检查 | 实测 | 判定 |
|---|---|---|
| `HomeWorldCamera._inputSampleForTests` | 字段存在、类型 `Nullable<InputSnapshot>`、`private=True`、**`static=False`** | 实例级、无全局单例 |
| 当前 Editor 中的 `HomeWorldCamera` 实例 | `Resources.FindObjectsOfTypeAll(HomeWorldCamera)` → **0 个** | 无宿主实例残留，故无“实例覆盖未清”的载体 |
| 既有实例采样值 | 无实例可读；若有实例，字段按源码为 `null=恢复生产采样`；上一轮运行日志已记录 `INPUT-GATE-CLEANUP` | 无残留证据缺口 |
| `SetInputSampleForTests` / `ClearInputSampleForTests` | 均存在、`IsAssembly`（internal）；public 版本不存在（第 14 例内断言） | 未新增 public API |
| `ReadInput` | public、参数 **2**（`bool overUI, bool suppressRightPan=false`） | 公共签名未变 |
| 默认建筑 registry 覆盖 | `StaticConfigRegistry.GetBuildingsOverrideForTests()` → **null**；`SetBuildingsOverrideForTests` 存在（internal） | 覆盖已清空，默认 Root 路径生效 |
| Player 面 | 字段 / 方法 / 分支全部位于 `#if UNITY_EDITOR`（源码 L53–L100） | **仅静态保证**，本轮未构建 Player 实证 |

采样缝只替换“输入快照数据”，不替换 `overUI` / `suppressRightPan` 门禁；`ProductHomeWorldHost.Update` 仍以 `Shell.IsPersonModalOpen || PointerOverUI(Input.mousePosition)` 计算 `overUI` 并调用同一个 `ReadInput`。生产路径唯一的语义差异是：快照统一在 `SampleInput()` 提前读取 legacy Input（只读、无副作用），门禁分支逐句沿用。

---

## 4. 最近直接日志（输入单例运行段，逐行引用）

来源：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，**L3643–L3658**（文件尾，之后无新增行）：

| 行 | 时间（UTC） | 证据行（截取原值） |
|---|---|---|
| 3643 | `2026-10-26T11:08:08.3209526Z` | `[BASELINE-PRE] total=34 diff=0 all-match` |
| 3644 | `2026-10-26T11:08:08.4645862Z` | `[FONT-PRE] sha256=3c758e16…c404854` |
| 3645–3646 | `11:08:08.550–551Z` | `[SAVE-BACKUP]/[SAVE-CAPTURE] runId=20261026T110808Z … bytes=13405 sha256=4b2b56f6…d6aa readback-verified=True` |
| 3647 | `11:08:09.6381279Z` | `[ENTRY] Product entry reached World; clock=Paused; saveGuard=active` |
| 3648 | `11:08:09.6959035Z` | `[INPUT-GATE-POINTER] 非模态阳性对照前 pointer=(44023.14, -42042.32) overUI=False hits=（无 UI 命中）` |
| 3649 | `11:08:09.7121622Z` | `[INPUT-GATE-POSITIVE] before=(10000.00, 0.00, -10.00) size=7.5 after=(9998.61, -0.56, -10.00) size=6.4` |
| 3650 | `11:08:09.9095995Z` | `[INPUT-GATE-MODAL] management camera=(9998.61100, -0.55556, -10.00000) size=6.40000 selection=person_adec172f8ac24ab4bbee222327a687e0 buildOpen=False speed=Paused` |
| 3651 | `11:08:09.9903275Z` | `[INPUT-GATE-PERSON] person camera=(9998.61100, -0.55556, -10.00000) size=6.40000 selection=person_adec172f8ac24ab4bbee222327a687e0 speed=Paused` |
| 3652 | `11:08:10.0231799Z` | `[INPUT-GATE-REOPEN] repeatOpenClose=2 townInteractable=True townBlocksRaycasts=True` |
| 3653 | `11:08:10.0231799Z` | `[INPUT-GATE-POINTER] 关闭模态后再次注入前 pointer=(44023.14, -42042.32) overUI=False hits=（无 UI 命中）` |
| 3654 | `11:08:10.0400615Z` | `[INPUT-GATE-AFTER-CLOSE] before=(9998.61, -0.56, -10.00) size=6.4 after=(9996.24, -2.45, -10.00) size=5.299999` |
| 3655 | `11:08:10.0412889Z` | `[INPUT-GATE-CLEANUP] camera sample cleared; no physical mouse/keyboard was simulated` |
| 3656 | `11:08:10.0474961Z` | `[SAVE-RESTORE] runId=20261026T110808Z … matches-before=True` |
| 3657 | `11:08:10.2176446Z` | `[FONT-POST] sha256=3c758e16…c404854 unchanged=True` |
| 3658 | `11:08:10.3953252Z` | `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match` |

**机制实证边界（严格区分）：**

- **已证（机制级）**：非模态时，合成的右键拖动 + 滚轮经**真实 `ProductHomeWorldHost.Update` → 同一 `ReadInput`** 产生**非零**相机位移与缩放（`size 7.5→6.4`，位置 `10000,0 → 9998.61,-0.56`），不是直接调 `Pan/Zoom`，也不是“无输入不变”的弱证据；清除采样后相机稳定；管理 / 人物模态下同一输入序列连续真实帧相机与 Selection / Clock / 建造候选不变；关闭模态后同一输入再次生效（可逆）；`finally` 清理并写 CLEANUP。
- **未证（物理面）**：物理鼠标 / 滚轮 / 键盘、真实窗口焦点 / DPI / UI 命中顺序、手感与速度；`overUI` 在模态段由 `Shell.IsPersonModalOpen` 决定，不依赖真实指针，但**不等价**物理设备。
- **ESC**：日志与用例走 `HandleEscape` **public 回调**（`Call(_shell,"HandleEscape")`），属回调机制证据，**不是物理 ESC 键**。
- 该段墙体跨度 `11:08:08.32→11:08:10.40`（约 2.08 s，纯日志行）；上游 run `df6c6645…` 报 **1/1 Passed、0 skipped、2.6441609 s、scope-restored**，两者不互相冒充（run 时长 ≠ 日志行跨度）。

---

## 5. 当前可见结果面（不编造）

| 项 | 现状 |
|---|---|
| `testResults` | 当前 **0 条**（本机只读查询为空）。因此**不能**从本层重建 19 Edit / 输入单例 / 14 例的逐条结果 |
| 19 Edit / 输入单例 | 以上游原始回执 `99761145…`、`df6c6645…` 为唯一权威；本文件不复制、不改写、不升级 |
| 证据日志 | 最后一条为 `2026-10-26T11:08:10.3953252Z`；**之后无任何新增行**，证明本阶段未发生新运行 |
| 14 例整类 | **尚无本轮真实回执**；按任务口径交宿主运行，结果以宿主回执为准；若宿主回执在本层不可见，写“待主开发补”，**不得由 13/13 + 单例外推为 14/14** |

---

## 6. 为什么宿主本轮必须跑“管理 Play 整类 14 例”

1. 输入门禁包改的是**产品输入采样路径本身**：`HomeWorldCamera` 新增私有 `SampleInput()` 与 `InputSnapshot`，`ReadInput` 的生产实现改由它取数。该 `ReadInput` 被 `ProductHomeWorldHost.Update` 在**每一个真实帧**调用，因此管理 / 人物 / 建造 / 滚动 / 存档 / 事件等**所有**管理 Play 用例的帧循环都经过这条被改动的路径。
2. 新增的第 14 例只覆盖“门禁 + 阳性对照”一个场景；它**不能**代表另外 13 例在新采样路径下的回归（例如关闭后真实帧节奏、模态期间各页签行刷新、TopHUD 暂停/速度、返回门禁、FirstNavigation、Save/Continue/NewGame）。
3. 因此“当前 14 例管理 Play 整类”是**必要受影响集成**，既不是重复已过的 19 Edit（Edit 不经过 `Host.Update` 帧循环），也不是重复已过的输入单例（只 1 例），更不是重跑全集（264 Compiler / 1513 Edit / 122 Play 均不重跑）。
4. 本轮**不改代码、不 Refresh**：源码与 DLL 已在上表确认新鲜，宿主可直接执行；若宿主发现编译或测试失败，应保留完整 message 交回，不在本预检内改断言或削弱范围。

---

## 7. 交宿主执行口径（阶段 2 由 Studio 自动继续）

```text
testMode: PlayMode
testClass: RRPG.ManagementShell.Tests.ManagementShellPlayTests
```

- 期望发现：**14** 个 `[UnityTest]`（名单见 §2），不得多跑 / 少跑；不接受“只跑第 14 例”替代整类。
- 通过标准：**14/14 Passed、0 skipped**，并取得真实 **runId / run 级时长 / skipped**；逐条 `message=null`。
- 执行后必须核对：`SAVE-RESTORE matches-before=True`（每个用例）、`BASELINE-PRE/POST 34/34`、`FONT-PRE/POST unchanged=True`、单槽 hash 仍 `4b2b56f6…d6aa`、guard 新增 restored 且无 unfinished、场景 `dirty=False`、`PrefabStage=null`。
- 第 14 例重点证据行：`INPUT-GATE-POINTER / POSITIVE / MODAL / PERSON / REOPEN / AFTER-CLOSE / CLEANUP`。
- 若失败：保留完整 message / 堆栈 / 证据行，区分“环境前置失败（真实指针压在 UI 上）”与产品断言失败；不得删改用例或降低断言。
- 若宿主回执在本层不可见：按“待主开发补”记录，不写 Passed。

---

## 8. 剩余项：普通可测 vs 必须物理设备（不全部转人工）

| 项 | 现状 | 处置 |
|---|---|---|
| **建造态 ESC 路由**（`Build.IsOpen` 时 `HandleEscape` 不关 Surface、先取消建造） | 第 14 例只断言“ESC 关闭模态时建造未打开”，**未构造建造打开 + ESC** → **当前未证** | **普通可测**：现有 public 入口（导航打开建造 → `HandleEscape` → 断言 `IsOpen=false` 且 Surface 不变）即可补，不需物理设备；列入后续可补包，不在本阶段扩范围 |
| Player 中采样缝不存在 | 仅 `#if UNITY_EDITOR` 静态证据 | 未构建 Player；如需要须独立 Player 构建授权与包 |
| UI 双档位渲染 / Save-Load 双 slot | 未覆盖（EditMode 真实 Gateway 已 19/19 覆盖） | 可另立包，普通可测 |
| 物理鼠标 / 滚轮 / 键盘 ESC / 焦点 / IME / 手感 | 未覆盖 | 必须物理设备 + 制作人人工体验；不得用程序化事件冒充 |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过；不由 1920 外推 |
| `CapacityFull` 诊断文案 | 按钮禁用已断言，文案未单测 | 普通可测，低优先 |
| `SaveSlotGuard` 进程级缺口 | 只在用例收尾恢复 | 需单独授权设计 out-of-process 兜底 |
| Chevron `44×28` / 字号 24 | 未修未测 | 非必要指示，保持 |

明确保留：**ESC 仍是 public 回调证据，不是物理键**；阳性对照的 `overUI` 依赖真实指针位置，属环境前置，不是产品缺陷；`Player` 编译面无运行时证据。

---

## 9. 本阶段边界与不做

- 只新增本文件；未改 2026-10-26 任何文件，未改代码 / 测试 / Prefab / Scene / 字体 / 存档 / manifest / 基线。
- 未运行 `tests-run`；未 Refresh；未 `rql-mutate`；未 PrefabStage；未字体测量（`GetPreferredValues` / `ForceMeshUpdate`）；未 Save / ClearDirty / SetDirty；未 Shell / Git / Preferences / 正式工程。
- 只读手段：`rql-query`、只读 `read` / `grep` / `glob`、只读 C#（哈希 / 反射 / 目录与 manifest 统计 / PNG 无需）。只读 C# 曾有一次探针编译失败（Console 已如实记录），修正后通过，未触碰资产。
- 未创建额外付费子 Agent；未安装组件、未改权限。
- 本文件与整包结论须由**主开发专业核验**；R3-B 状态不变，`requiredChecks` 不因本预检升级。

---

## 10. 写后只读复核（2026-10-27T02:20Z 前后）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / problems=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0`、mtime 仍 `2026-10-10T13:23:55.952Z` | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、mtime `2026-10-26T11:08:10.0458894Z`、`.tmp=False` | 未变 |
| guard | `dirs=245 / restored=244 / protective=1 / other=0 / noManifest=0` | 仍无 unfinished |
| Editor / 场景 | `playing=False`、`compiling=False`、`SampleScene dirty=False roots=5`、`PrefabStage=null` | 仍干净空闲 |
| 本文件（§1–§9 初版） | **17077B / `fc9537ae1ab2d0df79369c980ab7a58a86a4d57be78a9b9660f39f183f2ddd9f`**、mtime `2026-10-27T02:19:53.2676557Z` | §10 追加后自身字节会变化；**最终以目录实际文件为准** |
| 源码 / 测试 / DLL | 与 §2 完全一致（未改、未 Refresh） | 新鲜度保持 |

> 写后复核未运行测试、未 Refresh、未保存任何资产 / 场景。14 例整类结果仍待宿主真实回执；19 Edit / 输入单例仍以上游 `99761145…` / `df6c6645…` 为权威。
