# R3-B 当前 16 例管理 Play 最终集成预检（2026-10-27，阶段 1/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 本阶段性质：**只读核对 + 只新增本文件 + 交宿主整类 Play 16 例**。未改代码 / 测试 / Prefab / Scene / 存档 / manifest / 基线；未 `AssetDatabase.Refresh`、未 `SaveAssets / SaveChanges / SaveDirtyScenes`、未 `SetDirty / ClearDirty`、未 `rql-mutate`、未 PrefabStage 编辑、未字体测量（`GetPreferredValues` / `ForceMeshUpdate`）、未写 Preferences、未 Shell / Git 写、未启停 Editor、未构建 Player。**模型未运行 `tests-run`**；16 例整类由宿主 Studio 阶段 check 串行执行。
- 日期口径：当前 **2026-10-27**；2026-10-22 / 10-23 / 10-24 / 10-25 / 10-26 的既有产物只读引用，不改写、不伪改历史。
- 状态口径：R3-B 仍 **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；交付 contract 仍 **非 Completed / 非 awaiting_acceptance**（父子 running 非终态）。本文件须由主开发专业核验，不代表制作人批准。
- 上游已终态、本轮**不重派、不重复、不重跑**：主开发已原始核 ESC 采样修后单例 `cc3adfc5-7ede-4a1f-959a-2cdb9581220c`（**1/1 Passed、0 skipped、2.0916182 s、scope-restored**）；本文件**不重跑、不重算、不改写**该回执。旧 15 例整类 `33b8271a…`（单例）/ `46d65625`（14 例）/ `99761145-ddc9-43d9-a3eb-db6706e59acf`（19 Edit）亦保持上游原文。
- 核对时刻：**2026-10-27T07:06:16.6985257Z**（本机 `15:06 +08:00`，China Standard Time）。

---

## 1. 当前身份与安全恢复基线（只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，可交宿主 |
| 场景 / PrefabStage | `SampleScene` loaded、`isDirty=false`、`roots=5`；`PrefabStage==null` | 干净，无游离 PrefabStage |
| Game View 分辨率 | **1920×1080**（`Screen=1920×1080`） | 保持；**1280 已由制作人 `01a53eb8` 取消，本轮不再询问、不设门禁、不由 1920 外推** |
| 34 项磁盘基线 | `total=34 / problems=0`（逐项 bytes + sha256 对比 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`，本轮只读重算） | 全一致 |
| 受保护 TMP 字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、mtime `2026-10-10T13:23:55.9520319Z` | 未变 |
| 受保护 TMP 字体（内存） | `LoadAllAssetsAtPath` 9 个子资产、`EditorUtility.IsDirty` **dirty=0** | 无内存脏写；未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、mtime `2026-10-27T07:03:26.9159133Z`、`.tmp=False` | 内容为**原始单槽**；mtime 由 `cc3adfc5` 单例收尾恢复回写产生，非内容变化 |
| guard 目录 | `dirs=277`：`protective-snapshot=1`（`20261024T052704Z`，`readbackVerified=true`）+ `restored=276`；`noManifest=0 / pendingRestore=0 / unverified=0`；最新 restored = **`20261027T070325Z`**（`readbackVerified=true`、`sourceSha256=4b2b56f6…d6aa`、`sourceBytes=13405`） | 无 unfinished / 无待人工取证项 |
| Console | 产品源码 `Error/Exception` **0** | 产品侧干净 |

> guard 计数口径纠正：按 manifest 的**首个 `status` 字段**解析为 `277 = 276 restored + 1 protective-snapshot`。部分旧 manifest 的 `statusHistory` 数组内含 `protective-snapshot` 历史项（如 `20261024T143611Z`、`20261025T074636Z`），不得把历史状态计为当前 protective；已用 `grep` 逐文件复核，当前真正 `status=protective-snapshot` 只有 `20261024T052704Z` 一条。

---

## 2. 代码 / 测试 / DLL 新鲜度与 16 例发现（只读）

| 文件 | bytes | SHA256 | mtimeUtc | 判定 |
|---|---|---|---|---|
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs`（ESC 采样缝宿主） | 17549 | `fbcb85949f6876557ee1aed56e5261763f30d74b6d0e12e672ce6a813f573878` | `2026-10-27T02:45:50.6363386Z` | 与专项 §5 一致，本阶段未改 |
| `Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | 5077 | `39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` | `2026-10-26T11:04:08.7201777Z` | 未改 |
| `Assets/Scripts/Village/ProductHome/ProductHomeWorldHost.cs` | 14911 | `8e5dc3b1bd8a47b62f1a2bc184138af237d4be910162467efec08b4b20ceab16` | `2026-10-10T12:10:57.7976717Z` | 未改 |
| `Assets/Scripts/Village/ProductHome/HomeBuildingPlacement.cs` | 11222 | `d650c1f4cca4f1d065da7fc5e57841a7c73cdfb6636f8a45ff4128e8de489335` | `2026-10-10T12:10:57.7885344Z` | 未改 |
| `Assets/Scripts/StaticConfig/StaticConfigRegistry.cs`（测试缝宿主） | 29248 | `1c0c62fa77678e5bd25a5dec09ec164d9a2ccbad7054fbb72267838b8d366e75` | `2026-10-26T10:48:55.6554791Z` | 未改 |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **477356** | `5820d5308e5edadc9eefd53617cb91e14c01564bd8e915429324eeb5fc3c1033` | `2026-10-27T07:02:34.1971111Z` | 夹具修正后最终版，本阶段未改 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 565248 | `0801d17f2dc576207cc2cfda81b3493dd865dd00798b61e625b22f7eac8d4200` | `2026-10-27T07:02:38.5104572Z` | **晚于测试源码 4.31 s，新鲜** |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 649216 | `1fb4e9e38038f0a4100c5af619e5c0973e33a2d245faf4d9f2b6dbb4b9d2f151` | `2026-10-27T02:46:38.6043467Z` | **晚于 `HomeProductShell.cs` 47.97 s，新鲜** |

16 例发现用三条独立只读面交叉核对，结果一致：

1. 源码正则计数：`[UnityTest]` 标注 **16 处**（`[UnitySetUp]` 辅助入口不计）；
2. 运行时反射当前 `BattleStage1.PlayModeTests.dll` 中该 fixture 的 `[UnityTest]` 方法数 → **16**；
3. `tests | where fullName starts-with "RRPG.ManagementShell.Tests.ManagementShellPlayTests."` → **16 条**，`mode` 全部 `PlayMode`。

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
| 14 | `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState` | 既有 |
| 15 | `ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts` | 既有 |
| 16 | **`ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate`** | **本轮新增（真实 LateUpdate 采样 / 一次消费 / 阴性帧 / 建造门禁，已单例通过）** |

Edit 侧 `ManagementProjectionTests` 反射 `[Test]` 数仍为 **19**；本阶段**未改、未重跑**，只说明 16 例 Play 与 19 例 Edit 是两个独立集合，不互为替代。

---

## 3. 测试缝与生产路由状态（只读实测）

| 检查 | 实测 | 判定 |
|---|---|---|
| 默认建筑 registry 覆盖 | `StaticConfigRegistry.GetBuildingsOverrideForTests()` → **null** | 覆盖已清空，默认 Root 路径生效 |
| `SetBuildingsOverrideForTests` / `GetBuildingsOverrideForTests` | 存在、`internal`（`#if UNITY_EDITOR`），非 public API | 未新增公共契约 |
| `HomeWorldCamera` 实例 | `Resources.FindObjectsOfTypeAll` → **0 个**；`_inputSampleForTests` 字段存在 | 无宿主实例残留，故无“实例覆盖未清”的载体 |
| `HomeProductShell` 实例 | **0 个**；`_escapePressPendingForTests` 字段存在（默认 `false`） | 无宿主实例残留；pending 无非实例载体 |
| `SampleEscapePressed` | `private bool`、0 参数、1 处 `LateUpdate` 调用、1 处 `Input.GetKeyDown(KeyCode.Escape)` 回落 | 生产采样路径未变 |
| `SetEscapePressForTests` / `ClearEscapePressForTests` | 均 `internal`（`IsAssembly=True`）、0 参数、`public=False` | 无 public API |
| public ESC 测试缝计数 | 方法名同时含 `Escape` 与 `ForTests` 的 public 方法 = **0** | 未污染公共契约 |
| `LateUpdate` | `private`；`SampleEscapePressed` 调用点仍在其内 | 真实 Unity 消息路径 |
| 公开 API | `HandleEscape()` / `CloseSurface()` 等公开方法与属性签名与修前一致；无新增 public 成员 | 默认生产路由 / 公共 API / Authority 未改 |
| 产品文件范围 | 本阶段未改任何产品文件；`HomeProductShell.cs` 仍是专项批准的 **17549B / `fbcb8594…573878`** | 除已审 ESC 采样缝外零产品改动 |

采样缝只替换“一帧按下”的采样值，且由真实 `LateUpdate` 消费一次后回落为 `Input.GetKeyDown`；不绕过 `ShellVisible`、`Build.IsOpen`、`SurfaceModalActive` 等门禁。

---

## 4. ESC 单例直接证据（日志逐行核对）

来源：`Development/StudioRuns/R3B-20261023/play-evidence/management-shell-play-evidence.log`，**L4122–L4138**（文件共 4138 行，L4138 之后无新增行）——即 `cc3adfc5` 单例运行段。

| 行 | 时间（UTC） | 证据行（截取原值） |
|---|---|---|
| 4122 | `07:03:25.4433849Z` | `[BASELINE-PRE] total=34 diff=0 all-match` |
| 4123 | `07:03:25.5863075Z` | `[FONT-PRE] sha256=3c758e16…c404854` |
| 4124 | `07:03:25.6296941Z` | `[SAVE-BACKUP] runId=20261027T070325Z … bytes=13405 sha256=4b2b56f6…d6aa readback-verified=True` |
| 4125 | `07:03:25.6307357Z` | `[SAVE-CAPTURE] runId=20261027T070325Z … existed=True bytes=13405 sha256=4b2b56f6…d6aa` |
| 4126 | `07:03:26.6181695Z` | `[ENTRY] Product entry reached World; clock=Paused; saveGuard=active` |
| 4127 | `07:03:26.6505699Z` | `[ESC-FRAME] PRE selected=person_c989b941… clock=0 speed=Paused multiplier=0 townInteractable=True townBlocks=True buildOpen=False` |
| 4128 | `07:03:26.7574343Z` | `[ESC-FRAME] MANAGEMENT-CLOSED surface=World selection=person_c989b941… clock=0 speed=Paused townInteractable=True townBlocks=True` |
| 4129 | `07:03:26.7957746Z` | `[ESC-FRAME] NEGATIVE-FRAMES=3 surface=Management modal=True (no injection; press not replayed)` |
| 4130 | `07:03:26.8462740Z` | `[ESC-FRAME] PERSON-CLOSED surface=World selection=person_c989b941… clock=0 townInteractable=True` |
| 4131 | `07:03:26.8681468Z` | `[ESC-FRAME] BUILD-POINTER-NEGATIVE pointer=(44778.36, -42545.02, 0.00) overUI=False manual=(0.00, 4.00) candidate=(608.59, -598.40) pointerDerived=(608.59, -598.40) hits=（无 UI 命中）` |
| 4132 | `07:03:26.8691589Z` | `[ESC-FRAME] BUILD-NEGATIVE buildOpen=True definition=building_farm ghost=True surface=World pointer=(44778.36, -42545.02, 0.00) overUI=False candidate=(608.59, -598.40) buildCount=0 selection=person_c989b941… clock=0` |
| 4133 | `07:03:26.8813232Z` | `[ESC-FRAME] BUILD-GATE buildOpen=True definition=building_farm ghost=True surface=World managementActive=False candidate=(608.46, -598.57) pointerDerived=(608.46, -598.57) pointer=(44769.08, -42556.95, 0.00) pointerStable=False cameraStable=True camera=(10000.00000, 0.00000, -10.00000) size=7.50000 overUI=False buildCount=0 selection=person_c989b941… clock=0 speed=Paused` |
| 4134 | `07:03:26.9089016Z` | `[ESC-FRAME] END surface=World selection=person_c989b941… clock=0 speed=Paused townInteractable=True townBlocks=True` |
| 4135 | `07:03:26.9099586Z` | `[ESC-FRAME] CLEANUP injection cleared; no OS physical ESC / Unity Input engine internals were simulated` |
| 4136 | `07:03:26.9159133Z` | `[SAVE-RESTORE] runId=20261027T070325Z existed=True sha256=4b2b56f6…d6aa matches-before=True` |
| 4137 | `07:03:27.0585036Z` | `[FONT-POST] sha256=3c758e16…c404854 unchanged=True` |
| 4138 | `07:03:27.2108177Z` | `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match` |

> 准确读法说明：任务表述中的首段 `START` 在本用例实际日志中不存在，产品/夹具写出的首段标记是 **`PRE`**（源码 `Evidence("ESC-FRAME","PRE selected=…")`）。本文件按真实 `PRE` 记录，不臆造 `START` 行。

### 4.1 逐项结论（严格按日志能证明的范围）

| 要求 | 日志直接证明 | 诚实边界 |
|---|---|---|
| **真实 LateUpdate 采样** | L4127→L4128：注入一帧 press 后，管理表面由真实帧关闭到 `World`；L4135 明确 `injection cleared`；L4129 证明无注入的 3 个真实帧不关闭（press 未被重放） | 注入的是**合成“按下”**；不证明 OS 物理 ESC，也不证明 `Input.GetKeyDown` 引擎内部实现；只证明产品同一 `LateUpdate → SampleEscapePressed → HandleEscape` 路径 |
| **一次消费** | L4129：连续 3 个无注入真实帧保持 `Management` 模态；说明第一次注入只消费一次、未残留重复关闭 | 用例内的私有 pending 字段断言（cleanup 后为 false）已由同 run `Passed` 判定；日志本身不是断言执行器 |
| **阴性帧** | L4129 `NEGATIVE-FRAMES=3 surface=Management modal=True` | 依赖运行期间无物理 ESC 的环境前置；不等于 OS 键盘不穿透 |
| **管理 / Person 只关当前 Surface** | L4128 管理关闭、L4130 Person 关闭，两次都回 `World` 且 Selection / Clock / Speed 不漂移 | 程序化真实导航；非物理鼠标点击 |
| **建造门禁** | L4132 `buildOpen=True / definition=building_farm / ghost=True / surface=World / buildCount=0`；L4133 `candidate=pointerDerived`（`608.46,-598.57` 与同帧指针换算完全相同）、`pointerStable=False`（指针从 `44778.36` 移到 `44769.08`）、`cameraStable=True`、`managementActive=False`、`buildCount=0` | 候选变化来自**正常 `UpdatePointer` 指针更新**（指针在两次采样间移动），不是 ESC 改写；建造自身物理 ESC / 右键轮询未被注入，本用例只证明 Shell 路由让路 |
| **恢复** | L4136 单槽 `matches-before=True`；L4137 字体 `unchanged=True`；L4138 34 项 `same=34 changed=0 problems=0`；L4122 PRE 同样 34/34 | 与本轮 §1 独立只读复核一致 |

单例回执：主开发原始核 `cc3adfc5-7ede-4a1f-959a-2cdb9581220c` = **1/1 Passed、0 skipped、2.0916182 s、scope-restored**；本层 `testResults` 同步可见 1 条 **Passed / 1331 ms / message=null**（method 级时长，**不当作** run 级 2.0916182 s 的替代或重算）。

---

## 5. 为什么本轮必须宿主运行“管理 Play 整类 16 例”

1. **第 16 例已真实进入 fixture 且单例已过**，但单例不能证明同 fixture 串行污染关系：第 16 例注入 `HomeProductShell._escapePressPendingForTests`、操作真实建造候选、切换管理 / Person Surface；只有整类串行才能证明它不污染前 15 例、前 15 例也不污染它。
2. **第 16 例的 `finally` 清理必须被整类链验证**：`ClearEscapePressForTests` + `SaveSlotGuard` + `ResetForTests` + 活动日程恢复链在 16 例串行中都须逐例收尾；单例只覆盖它自己。
3. **第 15 例仍改写同 fixture 的静态 registry 与场景 / Continue 重建 host**：整类运行继续提供跨用例隔离证据；这不是 19 Edit（Edit 不经 `Host.Update` 帧循环，也不走真实 `LateUpdate` ESC 采样），也不是全集重跑（264 / 1513 / 122 均不重跑）。
4. 因此“整类 16 例”是**必要受影响集成**，不是重复已过的单例，也不是重复已过的 19 Edit。

---

## 6. 交宿主执行口径（阶段 2 由 Studio 自动继续）

```text
testMode: PlayMode
testClass: RRPG.ManagementShell.Tests.ManagementShellPlayTests
```

- 期望发现：**16** 个 `[UnityTest]`（名单见 §2），不得多跑 / 少跑；不接受“只跑第 16 例”替代整类。
- 通过标准：**16/16 Passed、0 skipped**，并取得真实 runId / run 级时长 / skipped；逐条 `message=null`。
- 执行后必须核对：每例 `[SAVE-RESTORE] matches-before=True`、`BASELINE-PRE/POST 34/34`、`FONT-PRE/POST unchanged=True`、单槽 hash 仍 `4b2b56f6…d6aa`、guard 新增 restored 且无 unfinished / 无 `noManifest` / 无 `pendingRestore`、场景 `dirty=False`、`PrefabStage=null`、Editor 回到空闲。
- 第 16 例重点证据行：`[ESC-FRAME] PRE / MANAGEMENT-CLOSED / NEGATIVE-FRAMES=3 / PERSON-CLOSED / BUILD-POINTER-NEGATIVE / BUILD-NEGATIVE / BUILD-GATE / END / CLEANUP`。
- 若失败：保留完整 message / 堆栈 / 证据行，区分“环境前置失败（如真实指针压在 UI、指针换算恰好等于手动候选、OS 指针在采样间移动）”与产品断言失败；不得删改用例、降低断言或把失败改写成通过。
- 若宿主回执在本层不可见：按“待主开发补”记录，**不写 Passed**，不以“单例 + 旧 15 例”外推 16/16。

---

## 7. 当前可见结果面（不编造）

| 项 | 现状 |
|---|---|
| `testResults` | 当前 **1 条**：`ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate` **Passed / 1331 ms / message=null**，即 `cc3adfc5` 单例（run 级 2.0916182 s 来自主开发原始核，本层不复制为该 run 的 run 级时长） |
| 16 例整类 | **尚无本轮真实回执**；交宿主运行，结果以宿主回执为准；不得由单例 + 旧 15 例外推 16/16 |
| 证据日志 | 最后一条为 `2026-10-27T07:03:27.2108177Z`（L4138 `[BASELINE-POST]`）；**之后无任何新增行**，证明本阶段未发生新运行 |
| 19 Edit / 旧 15 例 / 输入单例 | 以上游原始回执 `99761145…` / `46d65625` / `33b8271a…` / `df6c6645…` 为唯一权威；本文件不复制、不改写、不升级 |

---

## 8. 仍存在的诚实边界（不冒称、不新增门槛）

| 项 | 依据（原规格 / 当前源码） | 现状 |
|---|---|---|
| **OS 物理 ESC / `Input.GetKeyDown` 引擎实现** | 规格 §8 只要求“键盘不穿透” | 单例只证明产品层同一采样 / 路由 / 门禁路径被真实 `LateUpdate` 执行；**不证明** OS 物理键、按住重复、IME 焦点、Input 引擎内部 |
| 物理鼠标 / 滚轮 / 拖拽手感、DPI / 多指针时序 | 规格 §8 只要求“不穿透”，**未把“手感”列为验收项** | 未覆盖；合成快照 + `EventSystem` 只到机制层；**不得作为原规格新增强制验收** |
| 建造面板自身物理 ESC / 右键 | `HomeBuildingPlacement.ReadInput` 的 ESC / 右键分支 | 本用例**不注入**；只证明 Shell 路由在 `Build.IsOpen` 时让路 |
| Player 构建内测试缝不存在 | `HomeProductShell` / `HomeWorldCamera` / `StaticConfigRegistry` 测试缝均 `#if UNITY_EDITOR` | 仅**静态保证**，未构建 Player 实证 |
| 双档位渲染 1280 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过；不由 1920 外推；不再询问 |
| 历史 650 存档缺口 / 两次历史边界违规 | 既有文档原文 | 本包**未处理、未洗白、未回滚**，原文保留 |
| `SaveSlotGuard` 进程级 abort | 只在用例收尾 / `OneTimeTearDown` 兜底恢复 | 未覆盖进程级崩溃；需单独授权设计 out-of-process 兜底 |

> 本轮不把“物理设备 / IME / DPI / Player 构建”写成已验，也不把它们新增为 `requiredChecks`；Player 未构建是**测试边界**，不是原规格新验收门槛。

---

## 9. 本阶段边界与不做

- 只新增本文件；未改 2026-10-22～10-26 任何文件，未改代码 / 测试 / Prefab / Scene / 字体 / 存档 / manifest / 基线。
- 未运行 `tests-run`（16 例由宿主执行）；未 Refresh；未 `rql-mutate`；未 PrefabStage 编辑；未字体测量；未 `SaveAssets / SaveChanges / SaveDirtyScenes / ClearDirty / SetDirty`；未 Shell / Git / Preferences / 正式工程；未启停 Editor；未构建 Player。
- 只读手段：`rql-query`、只读 `read` / `grep` / `glob`、只读 C#（哈希 / 反射 / 目录与 manifest 统计）。只读 C# 未写任何资产；本轮第一次只读探针因两处编译错误（`CS8361` 字符串插值、`CS0234` 误用 `RRPG.Village` 命名空间）失败一次，错误如实留在 Console 历史，已按真实命名空间 / 语法修正后重跑，不涉及产品 / 测试源码。
- 未创建额外付费子 Agent；未安装组件、未改权限、未改计费渠道。
- 本文件与整包结论须由**主开发专业核验**；R3-B 状态不变，`requiredChecks` 不因本预检升级，父 contract 保持 running 非终态。

---

## 10. 写后只读复核（本文件写入后）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / problems=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、`.tmp=False` | 未变 |
| guard | `277 = 276 restored + 1 protective`、`pendingRestore=0 / noManifest=0 / unverified=0` | 仍无 unfinished |
| Editor / 场景 | `playing=false`、`compiling=false`、`SampleScene dirty=False roots=5`、`PrefabStage=null`、`1920×1080` | 仍干净空闲 |
| 源码 / 测试 / DLL | 与 §2 完全一致（未改、未 Refresh） | 新鲜度保持 |
| 本文件 | 追加本行之前的版本：**22119B / `851e269c4026e381ca4c8396d130486710d18810cfeb7e6d15129154a1d8429e`**、mtime `2026-10-27T07:06:58.9518394Z` | 只新增文档；追加本行后字节 / 哈希会变化，**最终以目录实际文件为准** |

> 写后复核未运行测试、未 Refresh、未保存任何资产 / 场景。16 例整类结果仍待宿主真实回执；单例仍以 `cc3adfc5` 为权威。
