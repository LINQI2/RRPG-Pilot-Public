# R3-B 当前 15 例管理 Play 集成预检（2026-10-27，阶段 1/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 本阶段性质：**只读核对 + 只新增本文件 + 交宿主整类 Play 15 例**。未改代码 / 测试 / Prefab / Scene / 存档 / manifest / 基线；未 `AssetDatabase.Refresh`、未 `SaveAssets / SaveChanges / SaveDirtyScenes`、未 `SetDirty / ClearDirty`、未 `rql-mutate`、未 PrefabStage、未字体测量（`GetPreferredValues` / `ForceMeshUpdate`）、未写 Preferences、未 Shell / Git 写、未启停 Editor、未构建 Player。**模型未运行 `tests-run`**；15 例整类由宿主 Studio 阶段 check 串行执行。
- 日期口径：当前 **2026-10-27**；2026-10-23 / 10-24 / 10-25 / 10-26 的既有产物只读引用，不改写、不伪改历史。
- 状态口径：R3-B 仍 **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；交付 contract 仍 **非 Completed / 非 awaiting_acceptance**（父子 running 非终态）。本文件须由主开发专业核验，不代表制作人批准。
- 上游已终态、本轮**不重派、不重复、不重跑**：主开发已原始核 `33b8271a-79c1-45d1-b6c3-c9fa6261de63`（新双槽单例 **1/1 Passed、0 skipped、2.6883936 s、scope-restored**）、`46d65625`（旧 14 例整类 **14/14**）、`99761145-ddc9-43d9-a3eb-db6706e59acf`（管理 Edit 整类 **19/19**、0 skipped、1.1578404 s、scope-restored）。本文件**不重跑、不重算、不改写**这三条回执。
- 核对时刻：**2026-10-27T02:37:03Z**（本机 `10:37 +08:00`，China Standard Time）。

---

## 1. 当前身份与安全基线（只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，可交宿主 |
| 场景 | `SampleScene`、`isDirty=false`、`roots=5`、`loadedScenes=1`、`PrefabStage=null` | 干净，无游离 PrefabStage |
| Game View 分辨率 | **1920×1080**（桌面 `Screen.currentResolution=2560×1600`，非工程渲染设置） | 保持；**1280 已由制作人 `01a53eb8` 取消，本轮不再询问、不设门禁、不由 1920 外推** |
| 34 项磁盘基线 | `total=34 / problems=0`（逐项 bytes + sha256 与 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 全一致，本轮只读重算） | 全一致 |
| 受保护 TMP 字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、mtime `2026-10-10T13:23:55.9520319Z` | 未变 |
| 受保护 TMP 字体（内存） | `LoadAllAssetsAtPath` 9 个子资产、`EditorUtility.IsDirty` **dirty=0** | 无内存脏写；未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、mtime `2026-10-27T02:34:05.9419665Z`、`.tmp=False` | 内容为**原始单槽**（mtime 由 CASE-E 收尾恢复回写产生，非内容变化） |
| guard 目录 | `dirs=260`：`protective-snapshot=1`（`20261024T052704Z`，`readbackVerified=true`）+ `restored=259`；`noManifest=0 / other=0`；最新 restored = **`20261027T023404Z`**（`readbackVerified=true`，manifest `sourceSha256=4b2b56f6…d6aa`，`sourceBytes=13405`） | 无 unfinished / 无待人工取证项 |
| Console | 产品源码 `Error/Exception` **0**；历史 Error 条目仅为早前只读探针自身的 `script-execute` 编译失败（`2026-10-27 10:31:20` 本机 `snippet.cs(6,92) CS0234 'UnityTestAttribute'`，不涉及产品 / 测试源码） | 产品侧干净；探针失败如实留痕 |

> 与 2026-10-27T02:21Z 索引相比，guard 由 `259 = 258 + 1` 变为 **`260 = 259 + 1`**：多出的一条即 `20261027T023404Z`（CASE-E 单例运行收尾恢复），属**新增实测**，不是历史改写。

---

## 2. 代码 / 测试 / DLL 新鲜度与 15 例发现（只读）

| 文件 | bytes | SHA256 | mtimeUtc | 判定 |
|---|---|---|---|---|
| `Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | 5077 | `39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` | `2026-10-26T11:04:08.7201777Z` | 与 2026-10-26 一致，未改 |
| `Assets/Scripts/StaticConfig/StaticConfigRegistry.cs`（测试缝宿主） | 29248 | `1c0c62fa77678e5bd25a5dec09ec164d9a2ccbad7054fbb72267838b8d366e75` | `2026-10-26T10:48:55.6554791Z` | 未改 |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **446994** | `05d6de111fd64054cee3ff50deb615a938f4a2a85d98c0c2633d045d03dd1d8a` | `2026-10-27T02:32:36.4434403Z` | 本轮**新增第 15 例**后的当前版本 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 547840 | `b8dae7339260189f68a1880cac5c122c4f3779b0f8e6aa92ccb0d53c7cdcdb1d` | `2026-10-27T02:32:40.3038091Z` | **晚于测试源码 3.86 s，新鲜** |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 649216 | `f80f8c804f10d9d84248a1f1e7ea717c3ab5ca676521cd2af9f8946a6877c852` | `2026-10-26T11:05:07.9226979Z` | **晚于 `HomeWorldCamera.cs` 59.20 s，新鲜** |

15 例发现用三条独立只读面交叉核对，结果一致：

1. 源码正则计数：`[UnityTest]` 标注 **15 处**（第 1169 行 `EnterTownThroughProductEntry` 是 `[UnitySetUp]` 辅助入口，不是用例，不计入）；
2. 运行时反射 `BattleStage1.PlayModeTests.dll` 中该 fixture 的 `[UnityTest]` 方法数 → **15**；
3. `tests | where fullName starts-with "RRPG.ManagementShell.Tests.ManagementShellPlayTests."` → **15 条**，`mode` 全部 `PlayMode`。

| # | 用例 | 性质 |
|---|---|---|
| 1 | `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` | 既有 |
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
| 14 | `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState` | 2026-10-26 新增（输入门禁单例） |
| 15 | `ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts` | **本轮新增（双槽真实 UI/SaveLoad + 建造态 ESC 回调，已单例通过）** |

Edit 侧 `ManagementProjectionTests` 反射 `[Test]` 数仍为 **19**（含第 19 个双 slot Gateway 用例 `ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`）；本阶段**未改、未重跑**，只用于说明 15 例 Play 与 19 例 Edit 是两个独立集合，不互为替代。

---

## 3. 测试缝覆盖状态（只读实测：registry 与相机）

| 检查 | 实测 | 判定 |
|---|---|---|
| 默认建筑 registry 覆盖 | `StaticConfigRegistry.GetBuildingsOverrideForTests()` → **null** | 覆盖已清空，默认 Root 路径生效 |
| 默认 registry 内容 | `TryGetBuildings` 成功；`FunctionBindings` = `building_farm:main:Workplace:ws_farm_01;building_kitchen:main:Workplace:ws_kitchen_01;building_workshop:main:Workplace:ws_test_01`（**3 条，全 `main`，无 `aux`**） | **默认 defs 无 aux**；第 15 例的 aux 只存在于内存覆盖，finally 必恢复 |
| `SetBuildingsOverrideForTests` / `GetBuildingsOverrideForTests` | 存在、`internal`（`#if UNITY_EDITOR`），非 public API | 未新增公共契约 |
| `HomeWorldCamera` 实例 | `Resources.FindObjectsOfTypeAll` → **0 个** | 无宿主实例残留，故无“实例覆盖未清”的载体 |
| `_inputSampleForTests` | 字段存在、`Nullable<InputSnapshot>`、`private=True`、`static=False` | 实例级、无全局单例 |
| `SetInputSampleForTests` / `ClearInputSampleForTests` | 均存在、`IsAssembly`（internal）；public 版本不存在 | 未新增 public API |
| `ReadInput` | public、参数 **2**（`bool overUI, bool suppressRightPan=false`） | 公共签名未变 |
| 键盘路由（产品侧） | `HomeProductShell.cs:95 Input.GetKeyDown(KeyCode.Escape) → HandleEscape()`；`HomeBuildingPlacement.cs:102 ESC/右键 → Cancel()`；`CharacterGrowthUI.cs:125 ESC`；**产品无键盘镜头控制** | 键盘输入面只有 ESC 轮询一行；见 §9 边界 |

采样缝只替换“输入快照数据”，不绕过 `overUI` / `suppressRightPan` 门禁；`ProductHomeWorldHost.Update` 仍以 `Shell.IsPersonModalOpen || PointerOverUI(Input.mousePosition)` 计算 `overUI` 并调用同一个 `ReadInput`。

---

## 4. CASE-E 恢复日志逐行核对（直接证据）

来源：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，**L3861–L3885**（文件共 3885 行，之后无新增行）——即 `33b8271a` 新单例运行段：

| 行 | 时间（UTC） | 证据行（截取原值） |
|---|---|---|
| 3861 | `02:34:03.8469590Z` | `[BASELINE-PRE] total=34 diff=0 all-match` |
| 3862 | `02:34:03.9821984Z` | `[FONT-PRE] sha256=3c758e16…c404854` |
| 3863–3864 | `02:34:04.058Z` | `[SAVE-BACKUP]/[SAVE-CAPTURE] runId=20261027T023404Z … bytes=13405 sha256=4b2b56f6…d6aa readback-verified=True` |
| 3865 | `02:34:04.9075981Z` | `[ENTRY] Product entry reached World; clock=Paused; saveGuard=active` |
| 3866 | `02:34:04.9372754Z` | `[CASE-E] START baselineRegistry=263chars override=<null>` |
| 3867 | `02:34:04.9397650Z` | `[CASE-E] OVERRIDE twoSlot=building_farm/农田/Production/3x2;building_house/房屋/Housing/2x2;building_kitchen/厨房/Production/2x2;building_workshop/工坊/Production/3x2\|\|building_farm:main:Workplace:ws_farm_01;building_kitchen:main:Workplace:ws_kitchen_01;building_workshop:main:Workplace:ws_test_01;building_farm:aux:Workplace:ws_test_02` |
| 3868 | `02:34:04.9435299Z` | `[CASE-B-SCHEDULE] CASE-E two-slot ui/save fixture: … isolated-points=[] … window=minute-of-day>=0 (仅本用例时间跨度；其它 9 例仍跑真实活动机会)` |
| 3869 | `02:34:04.9572861Z` | `[BUILD] definition=building_farm instance=building_instance_1` |
| 3870 | `02:34:04.9618800Z` | `[CASE-E] PLACE farm=building_instance_1 workerA=…9c0d77b0 workerB=…ba6d41e5 mainOwner=…bb44e848 auxOwner=…5a57dd96 lateJoiner=…ff570dfb` |
| 3871 | `02:34:05.1610174Z` | `[ADVANCE] targetMinute=14400 outcome=Completed` |
| 3872 | `02:34:05.1902789Z` | `[CASE-E] SLOTS wMain=workplace:building_instance_1:main wAux=workplace:building_instance_1:aux main.capacity=2 aux.capacity=1` |
| 3873 | `02:34:05.3095344Z` | `[CASE-E] ASSIGN main=…bb44e848→…:main aux=…5a57dd96→…:aux main.occupants=1 aux.occupants=1` |
| 3874 | `02:34:05.3477770Z` | `[CASE-E] FULL aux.CapacityFull=True main.CapacityFull=True aux.occupants=1 main.occupants=2 no-write=True` |
| 3875 | `02:34:05.4082823Z` | `[ADVANCE] targetMinute=16360 outcome=Completed` |
| 3876 | `02:34:05.4461832Z` | `[CASE-E] LABOR main.bound=2 labor=2 onsite=2 aux.bound=1 labor=1 onsite=1 evidence mainOwner/main=40 lateJoiner/main=40 auxOwner/aux=40 cross=0 clock=16360` |
| 3877 | `02:34:05.4603333Z` | `[CASE-E] SAVE clock=16360 ingredients=14 food=0 batches=1 evidence=3 completed=3 hashBefore=e8fa0011…41ed3 hashAfter=402fd3ec…5ee236fb` |
| 3878 | `02:34:05.9125445Z` | `[CASE-E] CONTINUE main=…:main aux=…:aux bound main=[…bb44e848,…ff570dfb] aux=[…5a57dd96] clock=16360 resources=14/0 batches=1 evidence=3 completed=3` |
| 3879 | `02:34:05.9181560Z` | `[CASE-E] ESC-BUILD buildOpen=True candidate=building_farm ghost=True surface=World managementActive=False (回调先判 Build.IsOpen)` |
| 3880 | `02:34:05.9363426Z` | `[CASE-E] ESC-CLOSE buildOpen=False surface=World managementActive=False (公开 Cancel 后 HandleEscape 才关闭当前 Surface)` |
| 3881 | `02:34:05.9374075Z` | `[CASE-E] END override=<null> defaultRegistryRestored=True farm=building_instance_1 main=…:main aux=…:aux clock=16360 resources=14/0` |
| 3882 | `02:34:05.9434732Z` | `[SAVE-RESTORE] runId=20261027T023404Z … sha256=4b2b56f6…d6aa matches-before=True` |
| 3883 | `02:34:05.9457811Z` | `[SCHEDULE-RESTORE] restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}（只读校验）` |
| 3884 | `02:34:06.0823590Z` | `[FONT-POST] sha256=3c758e16…c404854 unchanged=True` |
| 3885 | `02:34:06.2287457Z` | `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match` |

### 4.1 逐项结论（严格按日志能证明的范围）

| 要求 | 日志直接证明 | 诚实边界 |
|---|---|---|
| **FULL：main / aux 双路 `CapacityFull` 且零写** | L3874：两槽 `CapacityFull=True`、`aux.occupants=1 / main.occupants=2` 保持不变、`no-write=True`（aux 满员的 `workerB` 与 main 满员的 `workerB` 均未写入） | 该行的 CapacityFull 文本是**断言之后**写出的摘要；真正的判定是源码 L6324–6352 对 `CommandOutcome=="DomainRejected"`、`DomainDiagnostic=="CapacityFull"`、`PersonWorkplace(workerB)==null`、占用不变的断言，且同一运行 Passed。日志本身不是断言执行器 |
| **LABOR：main 2 / aux 1 且交叉 0** | L3876：`main.bound=2 labor=2 onsite=2`、`aux.bound=1 labor=1 onsite=1`；逐人证据 `mainOwner/main=40`、`lateJoiner/main=40`、`auxOwner/aux=40`；`cross=0`（mainOwner、auxOwner 在对方槽位的证据分钟均为 0） | 证据分钟是**真实推进到 16360 后**由产品计算出的 `WorkplaceEvidenceMinutes`，不是测试构造值 |
| **SAVE / CONTINUE 全事实等值** | L3877 记录保存前事实（`clock=16360 ingredients=14 food=0 batches=1 evidence=3 completed=3`）；L3878 记录 Load 后同口径事实**逐项相同**，且两槽身份 `…:main / …:aux`、绑定人集合 `main=[bb44e848, ff570dfb]`、`aux=[5a57dd96]` 一致 | 两行的“相等”由源码 L6503–6518 的 `Assert.AreEqual(savedX, X())` 断言判定；日志是同一次运行中这些断言通过后的摘要。`hashBefore≠hashAfter` 是**保存前后文件字节**（单槽被真实写入），与“世界事实等值”不矛盾；收尾 L3882 已把单槽恢复为 `4b2b56f6…d6aa` |
| **ESC-BUILD / ESC-CLOSE 回调顺序** | L3879：`Build.IsOpen=true`、候选 `building_farm`、ghost 可见、`surface=World`、`managementActive=False`（`HandleEscape` 未关建造、未进 CloseSurface 分支）；L3880：公开 `Build.Cancel` 后 `buildOpen=False`，再次 `HandleEscape` 才关闭当前 Surface | **只是回调 / 取消顺序机制**：`OpenCatalog` / `Choose` / `Cancel` 与 `Shell.HandleEscape` 均为公开方法调用，**不是物理 ESC 键**，也不是 `Input.GetKeyDown` 轮询路径 |
| **恢复** | L3881：`override=<null>`、`defaultRegistryRestored=True`；L3882 单槽 `matches-before=True`；L3883 日程 `sameInstance=True`；L3884 字体 `unchanged=True`；L3885 34 项 `same=34 changed=0 problems=0` | 与 §1 本轮独立只读实测（guard 最新 `20261027T023404Z`、单槽 hash、字体 hash、34 项）一致 |

**口径纠正（不洗白历史）**：日志 L3797（旧 14 例运行的 CASE-A 段）仍写 `(CapacityFull 域名诊断未经按钮进入，见报告未测)`。该括注是**CASE-A 单例的当时结论**，对 CASE-A 仍成立（CASE-A 只做 UI 满员禁用），但**不再是整包当前缺口**——`CapacityFull` 领域诊断已由 CASE-E 的 L3874 与对应断言覆盖。整类 15 例运行时 CASE-A 会再次打印同一括注，读日志时不得据此判“CapacityFull 仍未测”。

---

## 5. 为什么本轮必须宿主运行“管理 Play 整类 15 例”

1. **测试源码在 14 例回执之后变更**：`ManagementShellPlayTests.cs` mtime `2026-10-27T02:32:36Z`，晚于 `46d65625` 的 14/14 运行段（日志 `02:20:37–02:20:50Z`）。因此 **14/14 是“上一版源码 + 上一版 DLL”的历史回执**，不能外推为当前版本的 15/15。
2. **第 15 例改写同一 fixture 的运行环境**，逐项都需要同 fixture 集成证明：
   - **临时 registry 覆盖**：`SetBuildingsOverrideForTests` 是 **static** 内存覆盖（`StaticConfigRegistry`）。本用例开工断言“无残留覆盖”、finally 只恢复本次覆盖、收尾断言默认 registry 指纹与开工完全相同且不含 `building_farm:aux`。只有把整类放在同一进程串行跑，才能证明**前序用例不污染它、它不污染后继用例**。
   - **真实 Continue 重建 host**：用例内 `SceneManager.LoadScene("SampleScene")` → 真实 `MainMenuUI.btnContinue.onClick` → `SaveSystem.Load`，断言旧 host instanceId ≠ 新 host instanceId、`TownUI.PlayerData` 成为新来源、`_town/_host/_shell` 重新绑定。这与 `[UnitySetUp]` 的入口链同源，必须在既有 setUp/tearDown 链下验证不会互相打断。
   - **活动日程隔离**：`IsolateActivityOpportunitiesForFixture` 复用既有公开注入点，`TearDown` 在 `ResetForTests` 之后按同一实例恢复；单独跑无法证明“前 14 例仍跑真实活动机会、且本用例不改产品规则”。
   - **存档 guard 链**：每例 `SaveSlotGuard.Capture/Restore` + `OneTimeTearDown` 兜底；本用例含**显式真实保存**，是最重的存档写路径，必须与整类串行的 guard 链一起验证。
3. 因此“整类 15 例”是**必要受影响集成**，既不是重复已过的 19 Edit（Edit 不经过 `Host.Update` 帧循环，也不含 Continue 重建 host），也不是重复已过的单例（单例不含跨用例污染证据），更不是重跑全集（264 Compiler / 1513 Edit / 122 Play 均不重跑）。
4. 本轮**不改代码、不 Refresh**：源码与 DLL 已确认新鲜，宿主可直接执行；若宿主发现编译或测试失败，保留完整 message / 堆栈 / 证据行交回，本预检不改断言、不削范围。

---

## 6. 交宿主执行口径（阶段 2 由 Studio 自动继续）

```text
testMode: PlayMode
testClass: RRPG.ManagementShell.Tests.ManagementShellPlayTests
```

- 期望发现：**15** 个 `[UnityTest]`（名单见 §2），不得多跑 / 少跑；不接受“只跑第 15 例”替代整类。
- 通过标准：**15/15 Passed、0 skipped**，并取得真实 runId / run 级时长 / skipped；逐条 `message=null`。
- 执行后必须核对：每例 `[SAVE-RESTORE] matches-before=True`、`BASELINE-PRE/POST 34/34`、`FONT-PRE/POST unchanged=True`、单槽 hash 仍 `4b2b56f6…d6aa`、guard 新增 restored 且无 unfinished / 无 `noManifest`、场景 `dirty=False`、`PrefabStage=null`。
- 第 15 例重点证据行：`CASE-E START / OVERRIDE / PLACE / SLOTS / ASSIGN / FULL / LABOR / SAVE / CONTINUE / ESC-BUILD / ESC-CLOSE / END`。
- 若失败：保留完整 message / 堆栈 / 证据行，区分“环境前置失败（如覆盖残留、真实指针压在 UI 上）”与产品断言失败；不得删改用例或降低断言。
- 若宿主回执在本层不可见：按“待主开发补”记录，**不写 Passed**，不以 14/14 + 单例外推 15/15。

---

## 7. 当前可见结果面（不编造）

| 项 | 现状 |
|---|---|
| `testResults` | 当前 **1 条**：`ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts` **Passed / 1962 ms**，即 `33b8271a` 单例（run 级 2.6883936 s 来自主开发原始核，本层不复制为该 run 的 run 级时长） |
| 旧 14 例 / 19 Edit | 以上游原始回执 `46d65625`（14/14）、`99761145…`（19/19）为唯一权威；本文件不复制、不改写、不升级 |
| 证据日志 | 最后一条为 `2026-10-27T02:34:06.2287457Z`（L3885 `[BASELINE-POST]`）；**之后无任何新增行**，证明本阶段未发生新运行 |
| 15 例整类 | **尚无本轮真实回执**；交宿主运行，结果以宿主回执为准；不得由 14/14 + 单例外推 15/15 |

---

## 8. 本轮由已知覆盖推进的项（与历史口径对照）

| 项 | 历史口径 | 当前实测（`33b8271a` + CASE-E 日志） |
|---|---|---|
| 双 FunctionSlot 真实 Gateway 命令路径 | 2026-10-26 结论：配置只登记 `main`，第二 slot 未覆盖 | **已覆盖（Editor-only 内存覆盖，不改生产配置）**：L3872 两槽独立、L3874 双路 `CapacityFull` 零写；默认 defs 仍无 aux（§3 实测） |
| 双槽真实 UI 派工 / 名额 | 未覆盖 | **已覆盖**：真实行点击 + 真实 `assignButton` 派工，`main.occupants/aux.occupants` 互不影响 |
| 双槽 SaveLoad / Continue | 未覆盖 | **已覆盖**：真实保存按钮 + `SaveSystem.HasSave` + 真实 `btnContinue.onClick` 换源，L3878 逐项连续 |
| `CapacityFull` 域内诊断 | 2026-10-26 结论：按钮禁用已断言、诊断文案未单测 | **已覆盖（领域诊断值，非 UI 文案像素）**：`DomainDiagnostic=="CapacityFull"` + 原 UI 满员禁用断言 |
| 建造态 ESC 路由 | 2026-10-26 结论：未构造“建造打开 + ESC” | **已覆盖（公开回调 / 取消顺序机制）**：L3879–L3880；**不含物理 ESC 键** |

> 上述“已覆盖”均以**单例回执 + 日志行**为据；本轮不新增、不放大为整类通过。整类 15 例仍待宿主回执（§6）。

---

## 9. 剩余项：范围准确、不冒称

| 项 | 依据（原规格 / 当前源码） | 现状 | 处置 |
|---|---|---|---|
| **物理鼠标 / 滚轮手感、拖拽速度** | R3-P 规格 §8 只要求“点击 / 拖拽 / 滚轮 / 键盘**不穿透**”，**未要求“手感”**作为验收项 | 未覆盖；程序化 `EventSystem` + 合成 legacy Input 快照只证明机制 | **不得作为原规格新增强制验收**；如需体验评估属制作人人工，不写进 `requiredChecks` |
| **物理 ESC / 真实键盘轮询** | 规格 §8 含“键盘不穿透”；产品侧唯一键盘路由为 `HomeProductShell.cs:95 Input.GetKeyDown(KeyCode.Escape)`（另两处为建造取消与成长关闭）；产品**无键盘镜头控制** | **真实未覆盖**：所有 ESC 证据均走 `Call(_shell,"HandleEscape")` 公开回调，**绕过了 `Input.GetKeyDown` 轮询** | 诚实标注“回调机制已覆盖、真实键盘轮询未覆盖”；普通测试无法注入 legacy Input 键盘事件，不冒称 |
| **OS 焦点 / IME / 多指针时序** | 规格未列具体条目 | 未覆盖 | 需物理设备 / 人工；不冒称 |
| **Player 构建内测试缝不存在** | 源码 `#if UNITY_EDITOR`（`HomeWorldCamera.cs` L53–L100、`StaticConfigRegistry` ForTests） | 仅**静态保证**，未构建 Player | 如需实证须独立 Player 构建授权与包；双槽依赖 Editor-only 缝，Player 内不存在 |
| UI 双档位渲染（1920 已测） | 1280 由制作人 `01a53eb8` **取消** | 取消 ≠ 通过；不由 1920 外推 | 不再询问；不设门禁 |
| `SaveSlotGuard` 进程级缺口 | 只在用例收尾 / `OneTimeTearDown` 兜底恢复 | 未覆盖进程级 abort | 需单独授权设计 out-of-process 兜底 |
| **历史 650 存档缺口** | `21075B / 650c45c4…`（2026-10-25 记录） | 授权范围内**无备份、未恢复、未伪造** | 保持缺口记录，不在本包处理 |
| **历史违规留痕** | 旧 `ClearDirty`、详情修复包 `SaveAssets` 各 1 次 | 原文留存于既有文档 | **不洗白、不回滚、不重写**；本包未触碰任何资产 / 场景保存 |

---

## 10. 本阶段边界与不做

- 只新增本文件；未改 2026-10-23 / 10-24 / 10-25 / 10-26 任何文件，未改代码 / 测试 / Prefab / Scene / 字体 / 存档 / manifest / 基线。
- 未运行 `tests-run`（15 例由宿主执行）；未 Refresh；未 `rql-mutate`；未 PrefabStage；未字体测量；未 `SaveAssets / SaveChanges / SaveDirtyScenes / ClearDirty / SetDirty`；未 Shell / Git / Preferences / 正式工程；未启停 Editor；未构建 Player。
- 只读手段：`rql-query`、只读 `read` / `grep` / `glob`、只读 C#（哈希 / 反射 / 目录与 manifest 统计）。只读 C# 未写任何资产；Console 中历史探针编译失败已如实保留。
- 未创建额外付费子 Agent；未安装组件、未改权限、未改计费渠道。
- 本文件与整包结论须由**主开发专业核验**；R3-B 状态不变，`requiredChecks` 不因本预检升级，父 contract 保持 running 非终态。

---

## 11. 写后只读复核（2026-10-27T02:38Z 前后）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / problems=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、`.tmp=False` | 未变 |
| guard | `260 = 259 restored + 1 protective-snapshot`、`noManifest=0 / other=0` | 仍无 unfinished |
| Editor / 场景 | `playing=False`、`compiling=False`、`SampleScene dirty=False roots=5`、`PrefabStage=null`、`1920×1080` | 仍干净空闲 |
| 源码 / 测试 / DLL | 与 §2 完全一致（未改、未 Refresh） | 新鲜度保持 |
| 本文件 | 追加本行之前的版本：**26014B / `dbf4327a88c462944b51314e4d1c603eec6d1a030630f00b8f06544b92d3ffa5`**、mtime `2026-10-27T02:37:53.1525828Z` | 只新增文档；追加本行后字节 / 哈希会变化，**最终以目录实际文件为准** |

> 写后复核未运行测试、未 Refresh、未保存任何资产 / 场景。15 例整类结果仍待宿主真实回执；19 Edit / 14 例 / 单例仍以上游 `99761145…` / `46d65625` / `33b8271a…` 为权威。
