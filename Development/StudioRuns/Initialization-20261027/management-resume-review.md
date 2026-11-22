# 管理 17 例续接准备与宿主交接（阶段 1/1 · 恢复后仅补管理集成）

- 唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`，本轮实测）。
- 本阶段范围：**只续接缺失的 `ManagementShellPlayTests` 17 例宿主 check 准备**；不重做初始化整包、不重跑已通过的
  Compiler 280 / Edit 9 / 产品 1/1（`0231ecde`）/ Home 3/3（`909a6321`）。
- 本轮**未修改任何产品源码 / 测试源码 / 断言 / 场景 / Prefab / 字体 / 存档 / guard 历史 / 基线**；
  已有夹具只读审查，未重写。
- 执行方式：文件 Read / Glob / Grep、`rql-query` 只读、`script-execute` 只读状态测量与程序集内容核对。
- **未使用**：Shell / `Process` / Git / `tests-run`（含单方法 / 整类）/ `rql-mutate` / 全局 `SaveAssets` / `SaveChanges` /
  `SaveDirtyScenes` / `ClearDirty` / 字体或场景 `SetDirty` / PrefabStage / Preferences / 启停 Editor /
  Editor script `NewGame` / 派工 / UI / 字体探针 / 子 Agent / Workflow / Goal。
- 本文件是**待宿主实测**的准备与交接记录，**不预填 PASS、不代表制作人批准、不表示初始化整包完成**。

---

## 1. 恢复依据（原执行与安全续接）

| 项 | 值 |
|---|---|
| 原执行 ID | `01a75064` |
| 原执行状态 | `unknown`，**无原生终态**；本轮未把它当完成、未改写 |
| recoveryReview | `safeToContinue=true` at `2026-10-29T09:57:10.769Z` |
| recoveryReview.source | `maintenance-host-process-and-native-test-review` |
| recoveryReview.note | 全机无 SDK、无宿主测试投递记录、原生 runner 空闲 |
| Unity 状态 | `released / pending` 空、`testPending=false` |
| snapshotHash | `31fc764135b8b944c19732ba07af6f0be9f8ac49f697a693d7a3cc57145d7e05` |
| 制作人确认 | `2b6f25cf` 确认安全续接 |

- 前置要求是**保留**此前的 `recoveryReviewError` 旧错误与 `unknown` 历史，不删除、不改写、不以新状态覆盖历史。
- 本轮在授权 `repo` 内**未发现** `studio-execution-status.json` 文件，因此无法二次读取该 JSON；
  上表为交接值，**当前环境状态一律以下节本轮实测为准**，不以昨日摘要代替实测。
- 本轮**未新建 / 改写** `studio-execution-status.json`：没有新的 TRANSPORT 或终止性错误需要登记；
  若后续出现相同 TRANSPORT，应停止反复模型调用并按约定用新的状态文件保留错误。

---

## 2. 当前现场实测（2026-10-29T10:03Z 左右，真实只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| CWD | `D:\AI_Agent_Lab\rrpg_pilot\repo` | ✅ 授权目录 |
| Unity 工程 / 版本 | `project=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`、`unity=2022.3.62t9` | ✅ Pilot |
| Editor | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | ✅ 空闲 |
| PrefabStage | `PrefabStageUtility.GetCurrentPrefabStage()` → `null` | ✅ 未进 Prefab 编辑 |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()`（`Static\|NonPublic` 反射）→ **`False`** | ✅ 无运行中 run |
| 宿主 pending 可见范围 | `Tool_Tests.HasPendingTestRun()` → **`False`**；`testResults` 源 **0 条**（本 Editor 会话未执行测试） | ✅ 无未完成投递 |
| 字体资源隔离 scope | `SessionState "Studio.Pilot.FontTestScope"=False`；`Studio.Pilot.TestRunPending=False`；`Studio.Pilot.PrefabStage=False` | ✅ 上一轮 scope 已正常 End |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`）`isDirty=false / rootCount=5` | ✅ 未污染 |
| Console | 开工读取 `Error / Exception` → **0 条**；收尾为 **6 条**，全部来自本轮两个**失败的只读探针**（见下方说明），无产品 / 测试 / 编译错误 | ✅ 编译干净；探针日志如实登记、不清除 |
| 受保护字体（磁盘） | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与保护值一致 |
| 字体子资产 | `9` 个，`EditorUtility.IsDirty` → **0 dirty** | ✅ 无 pending 写 |
| 34 项 workspace 基线 | `Development/StudioRuns/R3B-20261022/workspace-baseline.json`：`entries=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |
| 当前用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`、`.tmp=False` | ✅ 按当前实际值记录 |
| Initialization guard | `Development/StudioRuns/Initialization-20261027/save-guard`：`dirs=67 / restored=67 / protective=0 / failed=0 / noManifest=0 / other=0` | ✅ 无未完成 run |
| R3B guard | `Development/StudioRuns/R3B-SaveProtection-20261024`：`dirs=310 / restored=309 / protective=1 / failed=0 / noManifest=0 / other=0` | ✅ 与既有记录一致 |
| 测试源码 | `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：`500853 B / 0293c882cf1928088e4213ad224339dd00c266e515c3f5bd1354c8b04dd8ca6e`（7371 行） | ✅ 与上次快照逐字节一致 |

**说明**：Initialization guard 由旧报告记录的 `63`（阶段 2/3 记录为 `64`）增至 `67`，新增 4 个目录时间戳为
`2026-10-28T11:46:12Z` / `11:47:56Z` / `11:47:57Z` / `11:47:58Z`，状态全部为 `restored`；
R3B guard 最新目录仍为 `2026-10-27T10:34:10Z`。即原中断阶段没有留下未完成 / 未知捕获。

**Console 6 条的真实来源（如实登记，不删除、不冒充干净）**：本轮为确认只读 API 形态先后执行过两个
`script-execute` 探针——第一次按错误签名调用 `TryReadDefault`，抛 `TargetParameterCountException`；
第二次在 `StaticConfigRegistry` 类型解析失败后继续调用，抛 `NullReferenceException`。
两处各被工具记录 3 条（McpToolManager / ToolRunnerCollection / Exception），合计 6 条，时间
`18:01:19` / `18:02:40`（本地），栈均落在 `__CodeExecSnippet.__Run`，**不是产品、测试或编译错误**。
两条探针随后均已按真实签名重写并成功取得只读结果；本包**未清除**这些日志。

---

## 3. 原中断阶段的副作用核对

| 可能副作用 | 实测 | 结论 |
|---|---|---|
| 未完成存档保护 run | Initialization `67/67 restored`；R3B `309 restored + 1 protective`；`noManifest=0 / other=0` | ✅ 无未知 `captured` 需自动恢复 |
| 用户档被半写入 | 当前档 `13134 B / adcf5d8b…8ceb08`，无 `.tmp`；与旧报告阶段 1/3 结束值一致 | ✅ 未漂移，本轮**不覆盖**该档 |
| 场景 / Prefab 编辑残留 | `SampleScene isDirty=false`；PrefabStage `null` | ✅ |
| 未保存 dirty 资产 | 字体子资产 `9 / dirty=0`；Editor `isUpdating=false` | ✅ |
| 编译 / Console 错误 | 无编译错误；`isCompiling=false`。Console 收尾的 6 条均为本轮只读探针自身异常（见 §2 说明），不含产品 / 测试 / 编译错误 | ✅ |
| 原生测试仍在跑 | `IsRunActive=False`；`HasPendingTestRun=False`；`testResults` 空 | ✅ runner 空闲 |
| 管理测试已跑过 | `R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 最后一条为 `2026-10-27T10:34:11Z`，**无 2026-10-28 / 10-29 记录** | ✅ 当前 17 例（含阶段 2 夹具更新）**尚未被宿主执行** |

---

## 4. 源码与程序集现状（管理 17 例）

### 4.1 源码

- `ManagementShellPlayTests.cs`：`500853 B / 0293c882…8ca6e`，7371 行，与上次快照**完全相同**（本轮无写入）。
- 类：`RRPG.ManagementShell.Tests.ManagementShellPlayTests`（有命名空间，`testClass` 正则可用）。
- 源码内 `[UnityTest]` 方法 **17 个**，方法名逐一保留：

  1. `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip`
  2. `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`
  3. `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock`
  4. `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`
  5. `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState`
  6. `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite`
  7. `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`
  8. `WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes`
  9. `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak`
  10. `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement`
  11. `FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt`
  12. `SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous`
  13. `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`
  14. `ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts`
  15. `ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate`
  16. `Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry`
  17. `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`

### 4.2 发现（`tests` 源，非源码推断）

| 项 | 实测 |
|---|---|
| 条数 | **17** |
| mode / assembly | 17 条全部 `PlayMode` / `BattleStage1.PlayModeTests` |
| fixture | 17 条均为 `ManagementShellPlayTests` |
| ignored / explicit | 17 条均 `false / false`（无 skip / ignore） |

### 4.3 程序集内容级新鲜度核对（不用 mtime 冒充）

| 项 | 实测 |
|---|---|
| `BattleStage1.PlayModeTests.dll` | `607232 B / 0cdabfb9a03f92d7e0277e27493d8c7dd83133c8bbbb6a947871425b0332efdb`、mtime `2026-10-28T11:39:08Z` |
| mtime 事实 | 源码写入时刻晚于 DLL（旧报告 §11.4 已披露），因此**不**以 mtime 证明新鲜 |
| 程序集反射 | 类型 `RRPG.ManagementShell.Tests.ManagementShellPlayTests` 存在且 `public=True`；`[UnityTest]` 方法 **17** 个，名称与源码逐一一致 |
| 内容级字符串核对 | 在当前 DLL 中按 UTF-16 全字节偏移检索本源码**断言 / 证据字面量**均命中，例如：`默认实例岗位 + 新增 6 座 Completed Farm 各 1 条真实功能岗位行（不得有测试注入的额外行）`、`正式默认世界必须已有真实实例岗位`、`显式追加的静态兼容岗位必须真实注册`、`正式实例世界必须有可派工的 Farm 实例岗位`、`本夹具必须通过真实建造入口生成 7 座 Farm`、`静态兼容岗位不是正式新增派工目标`、`正式默认世界必须存在 Completed Farm 实例`、`默认实例资源行 + 6 座已完工 Farm 必须产生真实 Farm 资源行（非空业务行，不是空态）` |

结论：当前加载的 `BattleStage1.PlayModeTests` 与当前管理测试源码内容一致；三处（源码、程序集反射、`tests` 发现）均实测成立。

---

## 5. 已有夹具只读审查（不重做）

### 5.1 正式默认世界仍为 7 实体

只读调用 `InitialWorldConfigurationCatalog.CreateDefault()`（内存构造，不 NewGame、不写档）：

- `Buildings=7`：`building_farm(-6,-1.2)`、`building_kitchen(-2,-1.2)`、5 座 `building_house(y=4.2)`；
- `People=5`（配置人口；运行时候选人含英雄共 6 人，夹具按真实快照计数，不硬编码 5/6）；
- 只读 `StaticConfigRegistry.TryGetBuildings`：Farm / Kitchen 各 1 个 `main` Workplace 槽（`ws_farm_01` / `ws_kitchen_01`），
  Housing 无岗位；因此正式默认基线岗位 / 资源行均为 **2**，**不是 0 建筑世界**。

### 5.2 四处阶段 2 夹具更新的只读复核

| 夹具位置 | 现状 | 判定 |
|---|---|---|
| `ManagementConstructionCommands...`（CASE-B） | 通过既有公开兼容通道 `EnsureInitializedWithExtraWorkplaces(null, [ws_farm_01])` **显式追加**静态兼容岗位；`EnsureCore(null, …)` 只补齐空单例、不重置正式实例世界；断言 `Kind=LegacyCompatibility / 派工禁用 / 撤职可用 / 加入施工不改写 Primary Assignment` 全部保留 | ✅ 旧静态测试初始化与正式世界显式分开，正式默认仍为实例世界 |
| `FirstNavigationAfterSourceReplacement...` | 旧候选岗位通过 `FirstInstanceFarmWorkplace(out _)` 从真实 `Snapshot.Buildings` 取第一座 Completed Farm 的实例岗位，不写死 `ws_farm_01` | ✅ 按真实基线派生 |
| `ManagementScroll_RealBuildingRows...` | 开工先读 `ReadManagementProjection()` 得 `baselineWorkplaces / baselineResources`，再断言 `baseline + 6`；岗位行要求全部来自 Completed Building Function；不假设空世界 | ✅ 滚动计数按真实基线派生 |
| `ManagementEscape_FramePolling...` | 建造候选手工起点 `(0,-6)`，先断言合法再以真实帧阴性对照；不冻结候选求绿 | ✅ 坐标与默认布局不重叠（见 5.3） |

### 5.3 坐标合法性（按真实定义 / 校验器复核）

- `BuildingPlacementValidator`：范围 `X[-12,12] Y[-10,8]`；越界条件为 `< / >`，重叠条件为**严格小于**半宽和。
- `building_farm` 实际 `FootprintWidth=3 / FootprintDepth=2`。
- 滚动用例 7 座 Farm：`y=-4`，`x=-10.5…7.5`，相邻 `Δx=3`（等于半宽和，不构成重叠），最左 `-12` 恰在边界（非越界），
  与默认 Farm/Kitchen（`y=-1.2`，`Δy=2.8 > 2`）及 Housing（`y=4.2`）均不重叠；
- `CASE-B / CASE-C / CASE-E` 的 `x=0/4/8, y=-4` 同样不越界、不与默认实例重叠；
- Escape 候选 `(0,-6)`：Farm 半高 1 → `y∈[-7,-5]` 在 `[-10,8]` 内，且不与默认实例重叠。

### 5.4 保留的强断言（未削弱）

- **SaveContinue**：`ExplicitSaveContinueAndNewGame_...` 仍验证真实管理 `saveButton` 成功文案、`SaveSystem.HasSave`、
  `SaveVersion: 1`、资源 / 批次 / 证据 / Clock / 施工参与者逐项等值、Continue 不重复结算、NewGame 不泄漏旧世界。
- **持久 SaveSlotGuard**：`Capture()` 先 `BlockOnUnfinishedRuns()`（只接受 `restored / protective-snapshot`，
  其它 / 无 manifest 一律阻断），`CreateNew` 备份并回读 SHA256；`Restore()` 只用本 run 备份写回并回读校验；
  失败保留 backup / manifest 并抛异常，不吞异常、不自动恢复 unknown。
- **日程恢复**：`IsolateActivityOpportunitiesForFixture` 捕获原 `IntradayActivitySchedule` 引用；
  `TearDown` 的**最外层 finally** 调 `RestoreActivityScheduleAfterFixture`，断言恢复为**同一实例**且机会点 /
  通勤 / 重复延迟 / 五项时长逐项等值；恢复失败保留捕获并报错。
- **计量 / 输入**：岗位容量、绑定 / 劳动 / 现场、施工进度、劳务证据分钟、活动 claim、真实 Raycast / PointerClick 等断言保留。

本轮**未新增 / 删除 / 放宽任何断言**；未发现需要窄修的实际源码错误（无编译错误、DLL 内容一致、夹具与真实默认配置一致；
Console 仅本轮只读探针日志）。
若宿主出现真实失败，再按异常栈定位：夹具问题改夹具，产品栈才做窄修，保留失败 message / stack。

---

## 6. 本轮实际写入与边界

| 项 | 实际 |
|---|---|
| 写入文件 | 本文件 `Development/StudioRuns/Initialization-20261027/management-resume-review.md`（新建）；`product-play-integration-review.md` 末尾追加交接指针 |
| Unity 侧动作 | 无（只读 `rql-query` / 只读 `script-execute` 状态与程序集核对；未 `Refresh` / `ImportAsset` / 重编译 / 进 Play） |
| 未做 | 未 `tests-run`、未启动或忙等 TestRunner、未 Play、未 `rql-mutate`、未 Shell / `Process.Start`、未 Git、未全局保存 / 场景保存 / `SetDirty` / `ClearDirty`、未写 Preferences、未 PrefabStage、未启停 Editor、未改字体 / 场景 / 存档 / guard 历史 / 34 项基线、未改产品与测试源码 / 断言、未访问未授权工程 / 认证目录 / 其他工作区、未安装组件 / 改权限 / 创建额外付费子 Agent |
| 未知 captured | 未发现；未自动恢复任何未知 run；用户档按当前实际值记录，未用历史 hash 覆盖 |

---

## 7. 宿主串行整类交接口径（唯一通过依据）

- **执行**：`testMode=PlayMode` + **`testClass=ManagementShellPlayTests`**（整类 17 例，串行，不拆方法、不并行）。
- **发现前置已实测**：该类 17/17 命中（类有命名空间 `RRPG.ManagementShell.Tests`，工具类过滤正则可匹配）。
- **通过标准**：17 例真实回执中 **Passed=17 / Failed=0 / Skipped=0**；**0 项 / skip 一律不算通过**。
- **失败处理**：保留真实 `message` / `stack trace` / 保护面；按栈做最小夹具或产品窄修，不得削弱任何已冻结 / 保留断言。
- **不重跑**：Compiler 280、Edit 9、产品 1/1（`0231ecde`）、Home 3/3（`909a6321`）已通过，本阶段不重复执行。
- **预期副作用**：宿主真实执行会新增 17 个 R3B guard run 目录（每个用例一次 `Capture/Restore`），全部应回到
  `restored`；用户单槽应恢复到本轮实测值 `13134 B / adcf5d8b…8ceb08`。若有失败，backup / manifest 保留供取证，
  不以“已恢复”掩盖失败。
- 本文件**不预填 PASS**；阶段门禁只认宿主统一 Studio 阶段 check 的真实回执。

---

## 9. 附记（2026-10-29 本轮 SetUp 反射歧义 + 本 run 存档 guard 收尾）

> 本节为**追加附记**，不改写 §1–§8 的历史结论；仍不预填任何测试 PASS。
> 本轮只修改 `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` 与本附记；
> 未改产品源码 / 产品 API / Save DTO / 场景 / Prefab / 字体 / 34 项基线 / guard 白名单。

### 9.1 首失败真实 stack（`testResults` / Console 实测）

宿主上一轮 `ManagementShellPlayTests` 17 例结果（真实回执）：

| 项 | 实测 |
|---|---|
| 总数 / 通过 / 失败 / 跳过 | `17 / 0 / 17 / 0`，总时长 `2.2060279s` |
| 首例 | `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`，`Failed 775ms` |
| 首例 message | `SetUp : System.Reflection.AmbiguousMatchException : Ambiguous match found.` |
| 首例 stack | `System.RuntimeType.GetMethodImplCommon` → `System.Type.GetMethod(String, BindingFlags)` → `ManagementShellPlayTests.Static(...) : line 113` → `<EnterTownThroughProductEntry>d__112.MoveNext() : line 1256` |
| 其余 16 例 | 均在 SetUp 被 `20261029T100458Z(captured)` 阻断，message 为 `存在未完成的存档保护 run … Expected: 0 But was: 1`；**不是 16 个业务缺陷** |

- 旧行号（113 / 1256）对应本轮修改前源码；修改后行号已变化。
- 首个 SetUp 内 `Capture()` 已成功，`Entry` 证据**未出现**，说明失败发生在产品入口完成前；该 run 的 `SAVE-RESTORE` 直到 `OneTimeTearDown` 才由 `SAVE-RESTORE-ONETIME` 兜底执行（见 9.3）。
- 结论：首例是**测试反射选择重载**造成，不是 `SaveSystem.NewGame` 产品语义缺陷；16 例是唯一 captured run 的连锁阻断，不能当作业务失败统计。

### 9.2 根因与最小修复（不削弱断言、不改产品 API）

`SaveSystem` 当前真实签名（只读反射枚举）：

```text
NewGame()
NewGame(InitialWorldConfiguration)
```

旧 `Static(typeName, method, args)` 使用 `Type.GetMethod(method, flags)`（不传参数类型）。当 `method="NewGame"` 时 .NET 无法在零参 / 一参重载间唯一选择，抛 `AmbiguousMatchException`。因此：

1. 新增 `ResolveMethod(Type, owner, method, args, requireStatic)`：按名称 + 实际参数个数 → 参数类型兼容性 → 精确运行时类型逐级唯一化；无法唯一确定时带候选签名 `Assert.Fail`，**不臆选**。
2. `Static(...)` 与 `Call(...)` 全部改走 `ResolveMethod`，覆盖本文件所有反射调用。
3. 直接 `GetMethod(name, flags)` 的位置改为显式签名：
   - `HomeWorldCamera.SetInputSampleForTests(Vector3, bool, bool, bool, bool, float)` / `ReadInput(bool, bool)`；
   - `HomeProductShell.SetEscapePressForTests()` / `ClearEscapePressForTests()` / `SampleEscapePressed()` / `LateUpdate()` / `HandleEscape()`；
   - `StaticConfigRegistry.TryGetBuildings(out BuildingRegistry, out string)` / `GetBuildingsOverrideForTests()` / `SetBuildingsOverrideForTests(BuildingRegistry)`；
   - `ManagementReadAdapter.Read()` 原本已是 `Type.EmptyTypes` 显式签名。
4. `StaticByArgs`（名称 + 参数个数）保留原语义，未改产品重载，未删除任何 `NewGame` 重载。
5. 只读复核修复后的选择结果：

```text
RESOLVE SaveSystem.NewGame() => NewGame() static=True
NEWGAME overloads=NewGame(); NewGame(InitialWorldConfiguration)
```

即零参调用明确选中 `NewGame()`，不再歧义；一参调用仍可按 `InitialWorldConfiguration` 精确定位。

**保留断言**：本文件 17 例原业务断言、`SaveSlotGuard` 白名单（仅 `restored` / `protective-snapshot` 放行）、
34 项基线 / 字体保护、活动日程同一实例恢复断言均未删除或放宽。

### 9.3 SetUp 先 Capture 后失败的清理漏洞与修复

上一轮真实证据（`management-shell-play-evidence.log`，2026-10-29）：

```text
10:04:58.7368580Z [BASELINE-PRE] total=34 diff=0 all-match
10:04:58.8740784Z [FONT-PRE] sha256=3c758e16...
10:04:58.9240257Z [SAVE-BACKUP] runId=20261029T100458Z ... readback-verified=True
10:04:58.9258469Z [SAVE-CAPTURE] runId=20261029T100458Z ... bytes=13134 sha256=adcf5d8b...
（无 ENTRY）
10:05:00.3124154Z [SAVE-RESTORE] runId=20261029T100458Z ... matches-before=True
10:05:00.3135635Z [SAVE-RESTORE-ONETIME] ok=True note=OneTimeTearDown 兜底恢复
10:05:00.4495301Z [FONT-POST] ... unchanged=True
10:05:00.6010572Z [BASELINE-POST] ... all-match
```

- 事实：`Capture` 成功后 SetUp 抛异常，普通 `[TearDown]` **没有**执行恢复；恢复靠 `OneTimeTearDown` 兜底，证明存在“先 Capture 后初始化失败”的清理缺口。
- 修复：`[UnitySetUp] EnterTownThroughProductEntry()` 在成功 `Capture` 后使用 `try/finally` + `entryCompleted` 哨兵；
  Setup 内任何异常（含反射选择失败、产品入口断言失败、后续帧异常）都会进入 `RestoreCapturedSaveAfterFailedSetUp()`：
  - 只用**本 fixture 已成功捕获的本 runID** `original-save.bin` 调用幂等 `Restore()`；
  - 写 `SAVE-RESTORE-SETUP-FAILURE` 证据（成功 / 失败均留痕）；
  - 恢复自身失败只记录证据 + 告警，**不掩盖原始 SetUp 异常**，backup / manifest 保留；
  - 未新增任何自动发现 / 自动恢复其他 run 的路径。
- 选择 `try/finally` 的原因：C# 不允许在带 `catch` 的 `try` 内 `yield return`；而 `try/finally` 在迭代器 `MoveNext()` 抛出时按语言语义执行 `finally`，不依赖 UnitySetUp 失败后普通 TearDown 是否调度。
- `[TearDown]` / `[OneTimeTearDown]` 的既有幂等 `Restore()` 与兜底逻辑保持不变；OneTime 链中的 `LoadBaseline` / 字体核对不会被前置 `ResetForTests` 反射异常短路（Reset 已在 TearDown 内单独 catch；Setup 异常路径先恢复存档再向外抛）。

### 9.4 本 run `20261029T100458Z` 保护收尾（独立复核，非引用旧报告）

| 项 | 本轮独立实测 |
|---|---|
| run 目录 | `Development/StudioRuns/R3B-SaveProtection-20261024/20261029T100458Z` |
| manifest | `status=restored`、`restored=true`、`captureUtc=2026-10-29T10:04:58.9208140Z`、`updatedUtc=2026-10-29T10:05:00.3124154Z` |
| backup | `original-save.bin`，`13134 B`，SHA256 `adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，mtime `2026-10-29T10:04:58.9230287Z` |
| 当前用户单槽 | 真实 `SaveSystem.GetSavePath()` 归一化后严格等于 manifest `sourcePath`；`13134 B`；SHA256 同上；mtime `2026-10-29T10:05:00.3124154Z` |
| 路径归一化 | manifest 的 JSON 转义先还原，再 `Path.GetFullPath(...).TrimEnd('\','/')`：`strictNormalizedEqual=True` |
| 字节比较 | `bytesEqual=True`、`shaEqual=True`（当前档与 backup 逐字节相同） |
| 时间窗 | Capture `10:04:58.920` → 兜底 Restore `10:05:00.312`；首例 `775ms` 失败、整轮 `2.2060279s`，均落在该窗口内；无 `ENTRY`，与“SetUp 在产品入口前失败”一致 |
| 源码顺序 | `Capture` → `_lastGuard` → `ResetForTests` → `LoadScene` → `NewGame`（旧行 1256）→ `Save`（旧行 1258） |
| 失败前是否写档 | `NewGame` 反射在 `Invoke` 前即抛 `AmbiguousMatchException`，未执行 `NewGame`；更未到达 `Save`。当前档与 backup 逐字节相同，独立证明没有本轮测试残留写入 |

结论：本 run 的**恢复终态已经是真实 `restored`**，manifest 与当前档一致，无需改写 manifest / 不需要新建保护快照 / 不需要以 13134 历史值覆盖当前档。
保留 `original-save.bin` + `manifest.json` 作为本 run 残留备份与历史证据；未删除、未移动、未改写。未扫描或恢复其他历史 run。

### 9.5 保护面 / 前置门禁（本轮实测）

| 项 | 实测 |
|---|---|
| Pilot | `project=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`，Editor `2022.3.62t9` |
| Editor | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` |
| 原生 runner | `UnityEditor.TestRunner.IsRunActive=False` |
| 宿主 pending | `com.IvanMurzak.Unity.MCP.Editor.API.Tool_Tests.HasPendingTestRun=False` |
| 字体 scope | `Studio.Pilot.FontTestScope=False`、`Studio.Pilot.TestRunPending=False`、`Studio.Pilot.PrefabStage=False` |
| 场景 | `SampleScene` `isDirty=false / rootCount=5` |
| 受保护字体 | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |
| 字体子资产 | `9` 个，`EditorUtility.IsDirty` → **0 dirty** |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` |
| R3B guard | `dirs=311 / restored=310 / protective=1 / other=0 / noManifest=0 / bad=[]` |
| Initialization guard | `dirs=67 / restored=67 / protective=0 / other=0 / noManifest=0 / bad=[]` |
| 测试发现 | `ManagementShellPlayTests` 17 条，全部 `PlayMode / BattleStage1.PlayModeTests`，`ignored=false / explicit=false` |

- 无未知 captured / 额外 active run / pending；R3B 唯一 `protective-snapshot` 为既有 `20261024T052704Z`，未改动。
- 未修改 guard 白名单，未删除任何历史 run / evidence / manifest。

### 9.6 编译与文件证据

| 项 | 实测 |
|---|---|
| 安全 Refresh | `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` 两次（仅必要编译） |
| 最终编译 | `EditorUtility.scriptCompilationFailed=False`；`isCompiling=false / isUpdating=false`；测试程序集 `BattleStage1.PlayModeTests.dll` mtime `2026-10-29T10:11:13.9588564Z` |
| 程序集内容 | 反射确认新方法 `RestoreCapturedSaveAfterFailedSetUp` / `ResolveMethod` 已进入当前测试程序集 |
| 修改文件 | `ManagementShellPlayTests.cs`：`508853 B` / `4b900beff1d5122e683e4802dbeb9948fe54d6dce4f184962c2fb7110d00927a` / `7520` 行 |
| 修改文件 2 | 本附记 `management-resume-review.md`（仅追加本节） |
| 编译历史日志 | 第一次编译因测试类内已有 `Nullable(object)` 方法名与 `System.Nullable` 冲突报 2 条 `error CS0119`；已改为 `System.Nullable.GetUnderlyingType`，最终编译成功。旧错误日志按规则**未清除**，如实保留 |

- 本轮未运行任何测试（未 `tests-run`、未单方法、未整类、未启动 / 忙等 TestRunner），因此没有新增 `SAVE-CAPTURE` / `SAVE-RESTORE`，
  也不预填 17 例结果。
- 本轮未使用 Shell / `Process` / Git / `rql-mutate` / 全局 `SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `ClearDirty` /
  字体或场景 `SetDirty` / PrefabStage / Preferences / 启停 Editor；未改正式工程、Packages、权限、认证目录或其他工作区。
- 只读探针自身产生的 Console 异常（含本轮首个 `VerifyState` 的 `NullReferenceException`）按“不删除历史日志”原则保留，不代表产品 / 测试 / 编译失败。

### 9.7 交接给宿主（不预填结果）

- 修复后仍需由宿主串行执行 **`testMode=PlayMode` + `testClass=ManagementShellPlayTests` 17 例**；通过标准仍为
  `Passed=17 / Failed=0 / Skipped=0`，0 pass / skip 不算通过。
- 预期副作用：每个用例新增一个 R3B guard run，成功路径 `captured → restored`；用户单槽应回到
  `13134 B / adcf5d8b…8ceb08`。若 SetUp 再失败，新 `SAVE-RESTORE-SETUP-FAILURE` 证据会直接显示本 run 是否已恢复；
  恢复失败则保留 backup / manifest，不自动掩盖。
- 本附记只证明：歧义调用已按显式签名修复、SetUp 失败清理缺口已补、本 run 终态与当前档经独立字节 / 路径核验一致。
  **不代表 17 例已通过，不代表制作人批准。**

---

## 10. 附记（2026-10-29 三例目标行真实滚动命中修复 · 阶段 1/1）

> 本节为**追加附记**，不改写 §1–§9 的历史结论；仍不预填任何测试 PASS。
> 本轮只修改 `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` 与本附记；
> **未改产品源码 / 产品 API / Prefab / 场景 / 字体 / Save DTO / 34 项基线 / guard 白名单 / 正式默认 0 建筑规则**。
> 本轮**未运行** `tests-run`（不单方法、不整类、不启动 TestRunner），17 例结果留给宿主串行验证。

### 10.1 宿主最新真实失败（只处理 3 例，14 例通过保持）

宿主 `a5b3061b-45d0-4980-a6fe-a01e9a5059a4` 的 17 例回执为 `17 总 / 14 pass / 3 fail / 0 skip / 14.7285278s`，
失败均为**真实业务行 top-hit 被挡**，不是此前的 SetUp 反射歧义 / 未完成 guard 连锁：

| 用例 | 真实 message（截取） | 真实 stack 落点 |
|---|---|---|
| `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` | `施工行·A（p1）：top hit 不是目标（玩家点到的不是它）`；命中 `0. ModalBlocker` | `AssertTopHitBelongs` line 395 → `RealPointerClick` line 410 → `ClickConstructionRow` line 857 → 用例 line 4319 |
| `ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts` | `岗位行·main：top hit 不是目标`；命中 `0. WorkplacePage / 1. ModalPanel / 2. ModalBlocker` | 同上 → `ClickWorkplaceRow` line 839 → 用例 line 6569 |
| `SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous` | `W0·施工行 A（p1）：top hit 不是目标`；命中 `0. ModalBlocker` | 同上 → `ClickConstructionRow` line 857 → 用例 line 5955 |

- 三处命中列表里**没有** `Viewport`、也没有任何业务行；说明 `ScreenCenter(target)` 已经落在
  `workplaceScroll` 视口之外，被页面 / 面板 / 模态背景接住，而不是按钮层级或 PointerClick handler 缺失。
- 栈行号对应**上一版**源码；本轮在 825 行附近新增 helper 后行号已变化，但失败的方法名与三个用例调用点不变。
- 前 14 例真实通过；本轮未重跑 Compiler 280 / Edit 9 / 产品 1/1 / Home 3/3，也未手动恢复任何历史 run。

### 10.2 目标行排序 / Rect / viewport / Canvas 时机的只读取证

- 正式默认世界仍为 `7` 座建筑、`2` 个真实实例岗位（Farm `main`、Kitchen `main`）；各例在真实建造 / 完工后追加
  完整建筑功能行。三例失败前的工作页真实行序为：
  - `CASE-C` / `W0`：2 条默认岗位 + Farm B `main` + Kitchen C `main`，其后才是施工段；
  - `CASE-E`：2 条默认岗位 + 新 Farm `main` + `aux`（同一实例两槽），目标 `main` 位于 4 条岗位行内。
- 行高由 `ManagementRowView.Bind(..., 132f)` 固定；岗位行容器与施工行容器都在**同一个**外层
  `workplaceScroll` 内容下，前面还有 `WorkplaceHeading` / 施工 `Heading` 等固定块。
  工作页 viewport 高度为 `650`（既有布局证据 `SCROLL ... viewportHeight=650`），
  4 条岗位行 + 两段标题 + 施工行区域已超过视口；施工行位于业务列表最后一段。
- 旧 `ClickWorkplaceRow` / `ClickConstructionRow` 只做 `Canvas.ForceUpdateCanvases()` +
  `LayoutRebuilder.ForceRebuildLayoutImmediate(content)`，随后直接
  `content.GetChild(index)` + `RealPointerClick`；**没有先滚动到目标行**。
  既有 `RealScrollAndDragToEnd` 只把列表滚到“末行”并验证末行可达，未服务这三例的“具体目标行点击”。
- 因此三例失败是“目标行业务上真实存在、但当前滚动位置下视口外不可点”，不能先归因为夹具行缺失，
  也不能用 `onClick` / 私有回调绕过。若宿主在本轮真实滚动后仍出现 top-hit 被挡，则按真实命中栈转产品布局窄修。

### 10.3 最小测试侧修复（不削弱断言、不绕过输入）

在 `ManagementShellPlayTests.cs` 新增**仅测试专用**目标行滚动 helper，并让两个既有点击 helper 统一走它：

- `ResolveDomainRow(...)`：每次按**真实领域 ID**（`WorkplaceId` / `BuildingInstanceId`）从当前
  `Management.CurrentSnapshot` 找行序，再从当前 `workplaceContent` / `constructionContent` 取行；
  强制一次布局并保留原“行数 == 真实快照业务行数 / 行 active”断言。滚动每一步后都重新解析，**不点陈旧行对象**。
- `ClickDomainRow(...)`：
  1. 只读记录滚动前 `Clock` / `Selection` / `Surface`、`content.anchoredPosition`、`verticalNormalizedPosition`；
  2. 目标行完整落在 viewport 内且真实 top-hit 属于目标行时，直接进入原 `RealPointerClick`；
  3. 否则只用真实 `EventSystem` 的 `IScrollHandler` 事件滚动：目标在视口下方发负 `scrollDelta`、在上方发正
     `scrollDelta`，固定 `scrollSensitivity` 步长、最多 `240` 步；**不直接赋 normalizedPosition / anchoredPosition**；
  4. 每步打印 `REAL-SCROLL-TARGET-STEP`：目标 ID、行序、`rowRect`、`viewport`、`content.anchoredPosition`、
     `verticalNormalizedPosition`、是否完整在视口内、真实 top-hit；开始 / 就绪各一条 `REAL-SCROLL-TARGET-START/READY`；
  5. 滚动结束断言 `Clock` / `Selection` / `Surface` 与滚动前相同（滚动不得推进业务事实），
     再按同一领域 ID 重新解析行，调用**原有** `RealPointerClick`（top-hit + `IPointerClickHandler` + `PointerClick`），
     最后断言 `SelectedWorkplaceId` / `SelectedConstructionBuildingId` 等于目标 ID。
- 未关闭遮罩、未缩小 viewport、未改行高、未克隆假行、未改正式默认 0 建筑、未放开可见 / 命中断言；
  原有 SaveLoad / 两 slot / Primary / 资源 / 计量 / guard 断言全部保留。

### 10.4 本轮实现后实测前置与文件证据

| 项 | 本轮实测 |
|---|---|
| Pilot | `D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game`，Editor `2022.3.62t9` |
| Editor | `playing=False / paused=False / compiling=False / updating=False` |
| 原生 runner | `TestRunnerApi.IsRunActive=False`（反射只读） |
| 宿主 pending | `Tool_Tests.HasPendingTestRun=False` |
| 测试 scope | `FontTestScope=False`、`TestRunPending=False`、`PrefabStage=False` |
| 场景 | `SampleScene` `dirty=False / roots=5`；PrefabStage `null` |
| 受保护字体 | `14961033 B / 3c758e16…c404854`；`LoadAllAssetsAtPath` = `9`，`EditorUtility.IsDirty` 计数 `0` |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` |
| Initialization guard | `dirs=67 / restored=67 / protective=0 / failed=0 / other=0 / noManifest=0` |
| R3B guard | `dirs=328 / restored=327 / protective=1 / failed=0 / other=0 / noManifest=0`（唯一 protective 为既有 `20261024T052704Z`） |
| 宿主上轮 17 run | `dirs=17 / restored=17`（`20261029T101213Z`…`T101226Z`，含 3 个 `-2`）；无未知 captured |
| 当前用户单槽 | `13134 B / adcf5d8b…8ceb08`，未覆盖 |
| 测试源码 | `516209 B / 7649 行 / ae58d6bf0d268d143aafb3e567b43c3c6d3e86176a41d9f7a64460a0afef4496` |
| 测试程序集 | `BattleStage1.PlayModeTests.dll` `613888 B`，mtime `2026-10-29T10:25:42.1664973Z` |
| 编译 | 安全 `AssetDatabase.Refresh(ForceSynchronousImport)` 后 `scriptCompilationFailed=False`、`isCompiling=False` |
| 发现 | 反射确认 `[UnityTest]` 仍为 `17`；新 helper `ResolveDomainRow / TopHitBelongs / TopHitLabel / ClickDomainRow` 已进入当前测试程序集 |

- 本轮没有新增 / 删除 / 放宽任何业务断言；未改产品布局源码，等待宿主真实滚动命中结果决定是否需产品窄修。
- 未使用 Shell / `Process` / Git / `rql-mutate` / `tests-run` / 全局 `SaveAssets` / `SaveChanges` / `SaveDirtyScenes` /
  `ClearDirty` / 字体或场景 `SetDirty` / PrefabStage / Preferences / 启停 Editor；未访问未授权工程 / 认证目录 /
  其他工作区；未安装组件 / 改权限 / 创建额外付费子 Agent。

### 10.5 交给宿主的唯一验证口径

- 仍执行 `testMode=PlayMode` + **`testClass=ManagementShellPlayTests`** 整类 17 例串行；通过标准
  `Passed=17 / Failed=0 / Skipped=0`，**0 项 / skip 不算通过**。
- 宿主真实运行后，新增 `REAL-SCROLL-TARGET-STEP` 证据会直接给出三例目标行的真实
  `rowRect / viewport / contentPos / norm / topHit / steps`：
  - 若三例均在本轮滚动后 top-hit 命中目标行并通过，则保留 17/17 回执与证据；
  - 若滚动到端点后仍被挡，按该证据中的真实差异判定产品布局 / 滚动窄修，而不是继续放宽测试或直调回调。
- 本附记不预填 PASS；不代表 17 例已通过，不代表制作人批准。
