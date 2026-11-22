# Activity claim 持有者加入施工：现契约证据与最小 EditMode 用例（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`）。
- 本包范围：**只读 Authority / 生产调用链核验 + 新增 1 个 EditMode 用例 + 旧报告 §5.2 概念更正**。不改 Work / Clock / Activity / API / DTO / 配置，不新增 busy 拒绝，不跑测试、不 `rql-mutate`、不 Shell、不全局 Save、不 Git 写、不碰正式工程。
- 结论前置：**当前生产契约允许 Activity claim 持有者合法加入施工**；claim 只约束 Activity 机会与劳动判定，不写 Temporary Occupancy / Assignment / Project。因此本包不造 `PersonBusy` 拒绝，只把现状锁进测试。

---

## 1. 现场前置（实测，2026-10-26）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` | 空闲 |
| 场景 | 仅 `SampleScene`，`isDirty=false`、`rootCount=5` | 干净 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage()==null` | 未打开 |
| 34 项 workspace 基线 | 按 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 逐文件 bytes+SHA256：**total=34 / same=34 / diff=0 / missing=0**（改前、Refresh 后各一次） | 全一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` | 未变 |
| guard 根 | **176** 目录：`restored=175`、`protective-snapshot=1`、无 manifest 缺失、无其它状态 | 无未知 / 未完成 run |
| 编译 | `AssetDatabase.Refresh` 后 `scriptCompilationFailed=False`；`VillageStage1.Tests.dll` mtime `2026-10-26T02:59:29.0494597Z`；`Assembly-CSharp.dll` 仍为 `2026-10-24T11:47:08.6030118Z` | 测试程序集新鲜，产品程序集未重编 |

补充：Console 当前有 **9 条工具层 `Script / Execute` 错误**（MCP snippet；其中本包一次只读哈希 snippet 用了不允许的表达式语句，其余为此前已存在的工具调用错误，含 `EditorApplication.inPlayMode` / 运行时 NRE），**均不是项目源码编译错误**。判据：`scriptCompilationFailed=False`、测试程序集新鲜且可反射出新用例；不据此宣称项目零错误之外的结论。

---

## 2. 读到的 Authority 与生产契约

### 2.1 Authority 没有要求“claim → busy 拒绝”

- R3-P 规格 §4：施工只调用 `TryAddConstructionWorker` / `TryRemoveConstructionWorker`；§7 的 “busy” 指世界推进 / 返程结算，不是 Activity claim；规格同时明确“未新增 busy 拒绝会改变现有规则语义，需制作人裁决”。
- R3-B 执行记录 §4/§7：在 Gateway 增加“Activity claim → busy 拒绝”会改变交互规则 / 公共契约，本包**不实施**。
- `Docs/项目状态快照.md` 当前口径：Activity claim 不改变 Primary Assignment、不写 Temporary Occupancy；claim 期间 `ArrivedWorkers` 跳过登记、投影显示“自主活动进行中”；**未新增 busy 拒绝**。
- `Docs/Architecture/技术架构总览.md`：`ActivityClaimRegistry` 是活动互斥占用 Authority，独立于 Village Temporary Occupancy；不写 Expedition / Unavailable、Active Project 或 Primary Assignment。

**判定：没有“claim 必须拒绝施工加入”的 Authority；现实现与 Authority 不冲突。** 若未来产品要求 busy 拒绝，那是对公共契约的规则变更，需单独裁决，不能用本测试或实现反推。

### 2.2 真实调用链（按源码逐段读）

1. `ManagementCommandGateway.AddConstructionWorker`（`Assets/Scripts/Village/ProductHome/ManagementCommandGateway.cs:74-91`）
   → `CheckPerson`（`:130-159`）：只查来源存在、`VillagePersonAvailability` 可用性、`CanSaveWorld`；**不查 Activity claim**。
   → 建筑存在且 `UnderConstruction`，否则 `TargetAlreadyCompleted`。
   → `VillageRuntimeBridge.TryAddConstructionWorker`。
2. `VillagePersonAvailability.Resolve`（`Assets/Scripts/Village/Runtime/VillageAvailability.cs:43-79`）：只有 `Expedition`、`IsDead` 两种非在村原因；claim 不参与。
3. `CanSaveWorld`（`VillageRuntimeBridge.Intraday.cs:33`）：`!IsWorldWriteLocked && _expeditionPersonIds.Count == 0`；claim 不参与。
4. `TryAddConstructionWorker`（`VillageRuntimeBridge.Buildings.cs:49-70`）：`GuardWorldWrite && CanSaveWorld` → 建筑必须 `UnderConstruction` → `PresentWorkPersons().Contains(personId)` → 建筑 Function 定义校验 → 复用/创建 `prj_test_01` → `TryAddProjectParticipant`；**全程没有 `QueryClaim`**。`PresentWorkPersons`（`Work.cs:11-18`）也只过滤远征 / `_unavailablePersonIds`。
5. 加入施工只写 Project 参与者与 ConstructionPlace，不改 `Assignments`；`RemoveConstructionWorker` 也只移除 Project 参与，不改 Primary Assignment。

### 2.3 claim 如何影响“到场 / 劳动”

- `ActivityClaimRegistry`（`Assets/Scripts/AutonomousLife/Activity/Runtime/ActivityClaimRegistry.cs:5-14`）只保存 `personId → activityInstanceId`；写入口是 `ActivityLifecycleService.Commit`（`:45-67`），且只在 Activity 提交时检查 `QueryClaim` 冲突；施工路径不调用它。
- `IsExecutingWork`（`Work.cs:24-27`）要求 `ActivityRuntimeBridge.QueryClaim(personId)==null`、`HasArrived(...)` 且处于工作时段。
- `ArrivedWorkers`（`Work.cs:56-69`）在 **prepare 与查询两条路径**都先 `if (QueryClaim(id)!=null) continue;`；`CompleteWorkInterval`（`:109-151`）与 `NextWorkBoundary`（`:70-108`）都只使用该结果，因此 claim 期间**不会登记 Arrival、不会计入项目劳动、不会推进 Project**。
- 到达语义：`VillageIntradayWork.Arrived`（`VillageIntradayWork.cs:104-110`）在首次登记 / 目的地变化时写 `ReadyMinute = now + Schedule.CommuteMinutes`；默认 `CommuteMinutes=15`（`:9-18`）。
- 活动结束：`ActivityLifecycleService.Complete`（`:93-112`）释放 claim；`ActivityRuntimeBridge.TryComplete`（`:40-44`）只额外广播 `CompletedOccurrence`（当前唯一订阅者是 `HomeProductProjection.HomeVillageFeed`），**没有调用 `RefreshWorkIntent`**。
- `RefreshWorkIntent`（`Work.cs:173-177`）只在撤出施工、项目完成等路径被调用；活动完成不会立即补登记到场。到场登记只会在下一次 `CompleteWorkInterval` 的 `prepare=true`（或工作边界 / 显式刷新）发生。

---

## 3. 新增 EditMode 用例

文件：`UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`

新增方法：`ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability`（第 18 例；原 17 例断言未改、未删除）。

### 3.1 夹具与合法性

- 复用既有 `CommitRestActivity` helper：真实 `OpenDailyWindow` + `TryCommitActivity`（公开 Activity API），**不私有写 claim、不伪造 `QueryClaim` 字符串、不随机选活动**。
- 用既有公开注入点 `ConfigureActivitySchedule` 把机会点排到推进区间外（`{1020,1200}`），避免生产默认 420 机会点抢占当日唯一窗口；`TearDown` 已恢复原日程。
- 用真实 `Place` + `Complete` 建立一座完工建筑与一座施工中建筑；Primary Assignment 通过 `Gateway.AssignToWorkplace` 建立；施工加入通过 `Gateway.AddConstructionWorker`。
- 活动结束复用既有 FSM：`TryBeginExecution` → `TryComplete`（公开入口，反射处理 out 参数），走 `Committed→Executing→Completed` 并释放 claim。

### 3.2 逐项断言（全部对应当前生产契约）

| 阶段 | 断言 |
|---|---|
| claim 建立后加入施工 | Gateway 结果 `Success`、`Succeeded=true`、`DomainDiagnostic=""`、消息含“原主要职责保留”；**不是 WorldBusy / PersonBusy** |
| claim 身份 | 加入前后 `QueryClaim` 均为同一 `management:rest:person_alpha`；实例状态 `Committed` |
| 长期绑定 | `Assignments.GetWorkplaceId == workplace`；`ManagementSnapshot.Person.PrimaryWorkplaceId` 不变；`Availability == InVillage` |
| 短期占用 | 投影 `TemporaryCommitment == "自主活动进行中：" + claimId` |
| 参与者集 | Project `ParticipantPersonIds` 精确等于 `{person_alpha}`；施工行 `WorkerCount=1` 且 `WorkerPersonIds` 精确 |
| claim 期间（Advance 481） | 无 Arrival 行；`HasArrived=false`；`GetWorkExecutionLabel="通勤"`；`Project.Progress=0`、`LaborRemainder=0`、`ExecutionPaused=true`；原岗位 `LaborCount=0`、`EvidenceMinutes=0` |
| 合法结束瞬间 | `QueryClaim=null`、实例 `Completed`；仍无 Arrival、`LaborRemainder=0`、label 仍“通勤” → **不预设“立即登记”** |
| 下一工作区间（Advance 482） | 出现 Arrival，目的地为 `construction:<buildingId>`，`ReadyMinute=496`（481+15）；`HasArrived=false`、无劳动 |
| 到场就绪后（Advance 497） | `HasArrived=true`、label `"项目劳动"`、`LaborRemainder=1`、`Progress=0`；Primary 仍为原岗位；参与者集不变；原岗位证据仍为 0 |

### 3.3 编译 / 发现（本执行层实做）

- 测试文件：`52584B / 9370af5ba2bbd6e2f79a27e729e7a5b51b2f554119c195192d7bf84a82585691`（mtime `2026-10-26T02:59:18.9346533Z`）。
- `AssetDatabase.Refresh()` 后 `scriptCompilationFailed=False`。
- 反射 `VillageStage1.Tests`：`ManagementProjectionTests` 的 `[Test]` 总数为 **18**，新方法可发现；`tests` 查询该方法的 mode 为 `EditMode`。
- `testResults` 为 **0** 条（本包未运行测试）。
- **未运行** `tests-run` / TestRunner；PASS/FAIL 由宿主单方法 EditMode 产出，本报告不预填通过。

---

## 4. 长期绑定与执行占用的区分

- **长期绑定**：`Assignments` 的 Primary Assignment / 岗位名额，属于长期安排；加入/撤出施工都不改它（测试直接断言）。
- **执行占用**：Project 参与者名单是施工的临时承诺；Activity claim 是另一条短期互斥占用。两者都不等于“当前正在劳动”。
- **真实劳动**：需要工作时段 + 已到场（`ReadyMinute` 已过）+ `QueryClaim==null`，由 `CompleteWorkInterval` 结算；只有到场工人才推进 Project。
- 因此“claim 持有者加入施工成功”只说明**长期/名单层面允许加入**，不说明 claim 期间有到场或劳动；测试把这两层分开断言。

---

## 5. 更正旧报告 §5.2：第二 slot 是建筑 FunctionSlotId，不是 SaveSystem 存档槽

已在 `resume-status-and-safe-closeout.md` §5.2 更正，并保留错误原文作为“更正附记”：

- **第二 slot = `BuildingFunctionBinding.FunctionSlotId`**（例如 `main` / `aux`），属于建筑功能岗位配置。
- 当前 `BuildingConfigDatabase` 只登记 `FunctionBindings: main`；因此真实产品 Gateway 没有合法注入第二 FunctionSlot 的路径。这是**配置 / Authority 缺口**，不是测试缺陷。
- `SaveSystem` 是**单槽存档**（唯一 save 文件、guard 只保护该槽）；它**不能解释** Gateway 的 FunctionSlot 限制，二者不是同一概念。
- 本包未改配置、未伪造第二 FunctionSlot、未扩断言。

---

## 6. 直接证据与推断分界

**直接实测**：现场状态、34 项 / 字体 / 单槽 / guard、`scriptCompilationFailed=False`、测试程序集 mtime 与 18 例反射发现；源码逐段阅读到的调用链与条件。

**基于源码的确定行为**（不是猜测，但本包未运行该用例）：claim 期间 `ArrivedWorkers` 跳过、结束不立即补登记、`ReadyMinute=now+15`、到场后下一分钟劳动。若宿主单方法运行失败，以宿主回执为准，不回写本报告为 PASS。

**未验证**：真实产品 PlayMode 入口下的同样路径；第二 FunctionSlot 的真实 Gateway 路径；claim 与其他短期占用叠加；跨午夜 / 跨日活动结束边界。

---

## 7. 本包边界与实际副作用

- 唯一写入：
  1. `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`（仅新增第 18 例与一个 helper）；
  2. 本文件；
  3. `resume-status-and-safe-closeout.md` §5.2 更正与更正附记。
- 未改产品源码 / Prefab / Scene / 字体 / 34 项基线 / 存档本体 / guard manifest / 配置 / Authority；测试源码不在 34 项基线内。
- 未 `rql-mutate`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未保存场景 / Prefab、未写用户 Preferences、未 Shell / `Process.Start`、未启停 Editor、未 Git 写。
- 唯一 Editor 操作：`AssetDatabase.Refresh()` 触发测试程序集编译；场景保持 `isDirty=false`。
- 未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。

---

## 8. 移交 Studio

1. 请宿主以**单方法 EditMode** 运行 `RRPG.Management.Tests.ManagementProjectionTests.ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability`。
2. 期望按 §3.2 断言；任一失败请保留真实回执与日志，不以夹具放宽掩盖。
3. 本报告及结论需主开发专业核验，不代表制作人批准；R3-B 仍为 Implemented / Verification PARTIAL / Not Committed / Not Published。

---

## 附记（2026-10-26，claim 生命周期断言修复包；本节追加，不改写上文历史结论）

### A. 历史失败回执（宿主实跑，原样保留）

| 项 | 实测 |
|---|---|
| 宿主 run | `24c81ccf-e9c9-492e-843d-09a93757bc9d` |
| 统计 | **1 总 / 0 过 / 1 败 / 0 skip** |
| 用例 | `RRPG.Management.Tests.ManagementProjectionTests.ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability` |
| 状态 / 耗时 | `Failed / 135ms` |
| message | `claim 持续期间身份不变` / `Expected: "management:rest:person_alpha"` / `But was: null` |
| 失败位置 | 原用例 `Advance(481)` 后的 `Assert.AreEqual(claimId, QueryClaim(...))`（现文件已改；原逻辑行在旧版本 673） |
| scope | `scope-restored`（宿主已恢复作用域） |

失败不是产品异常，也不是 `AddConstructionWorker` / `RefreshWorkIntent` 抢占：是夹具把断言时间点放在了生产**合法的 Activity 终止之后**。原 §3.2 中 “claim 期间 Advance 481 / 下一区间 482 / ReadyMinute 496 / 497 到场” 是按错误前提写下的历史期望，**已被本节取代**；旧文保留不改写。

### B. 直接时间线与释放因果（源码逐段核对）

真实 `WorkSchedule`：`MorningStart=480`、`CommuteMinutes=15`、`Lunch=720`、`AfternoonStart=780`、`WorkEnd=1020`（`VillageIntradayWork.cs:9-18`）。因此：

- `NextWorkBoundary` 把 `MorningStart - CommuteMinutes = 465` 作为真实通勤工作边界（`VillageRuntimeBridge.Work.cs:70-108` + `VillageIntradayWork.NextBoundary` `:117-125`）。
- `VillageTime.ResumeAdvance` 到达 465 时先 `CompleteInterval(460→465)`，再 `CompleteBoundary(465)`（`VillageTime.cs:176-188`）。
- `DailyAdvanceSink.CompleteBoundary` 的顺序是 `CompleteActivityBoundary` → `CompleteWorkBoundary` → `OpenIntradayOpportunity`（`VillageRuntimeBridge.Intraday.cs:168-169`）。
- `CompleteActivityBoundary` 在 `point.MinuteOfDay == MorningStart-CommuteMinutes` 时置 `workPriority=true`；claim 参与者 alpha 虽在村，但其 `WorkDestination` 非空（施工项目目标），故被判定 `interrupted`（`VillageRuntimeBridge.Activities.cs:46-65`）。
- 该 Rest 是**未排程**活动（`TryCommitActivity` 提交，`OpportunityId=null`、`IsScheduled=false`、`CompletionMinute=0`），不会按完成分钟自动完成；它在 465 走 `FinishOrInterruptReception` → `ActivityRuntimeBridge.TryInterrupt`（`VillageRuntimeBridge.Reception.cs:60-79`），由 `ActivityLifecycleService.EndWithoutSuccess` 置 `Interrupted` 并 `Claims.Release`（`ActivityLifecycleService.cs:113-123`）。**这是 Activity Authority 的合法终态释放，不是管理命令。**
- 同一边界紧接着 `CompleteWorkBoundary(465)`：此时 claim 已空，`ArrivedWorkers(465,true)` 为新目的地登记 Arrival，`ReadyMinute = 465 + 15 = 480`（`VillageRuntimeBridge.Work.cs:56-69,152-160` + `VillageIntradayWork.Arrived` `:104-110`）。
- 465→480 不满足 `IsWorking`，`CompleteWorkInterval` 直接返回，不结算劳动（`VillageRuntimeBridge.Work.cs:109-113`）；480 到场但 `LaborRemainder=0`；480→481 才是工作时段第一分钟，`OnWorkInterval(1, [alpha])` 产生 `LaborRemainder=1`（`VillageProjectSystem.cs:211-232`；`prj_test_01` 每人每日 1、目标 10）。

直接时间线（绝对分钟，全部由真实 Schedule 派生）：

| 分钟 | 事件 | claim | 施工贡献 | Arrival |
|---|---|---|---|---|
| 464 | 提交真实 Rest（Committed）并成功 `AddConstructionWorker`；通勤边界前 | 同 `claimId` | `Progress=0`、`LaborRemainder=0`、原岗位劳动/证据 0 | 无 |
| 465 | 真实通勤工作边界；工作优先中断 Rest（Committed→Interrupted） | **null（合法释放）** | 仍 `Progress=0`、`LaborRemainder=0` | 登记，`ReadyMinute=480`，尚未到场 |
| 480 | WorkStart；`HasArrived=true` | null | 仍 0（465→480 非工作时段） | 已到场 |
| 481 | 工作时段第一分钟 | null | `LaborRemainder=1`、`Progress=0`、`ExecutionPaused=false` | 已到场并劳动 |

**判定：不存在 Authority 冲突。** claim 未完成却被“清除”的原因是 Clock 推进到真实工作边界时，生产按既有工作优先规则终止未排程 Rest；`AddConstructionWorker` 回执 Success、加入前后 claim 不变，边界前后没有任何管理命令，未调用 `RefreshWorkIntent`，也没有清私有状态。

### C. 本次最小修复（只改第 18 例与专项 helper）

文件：`UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`

- 原 17 例断言与 helper 未改；只重写第 18 例的生命周期断言与去除仅被第 18 例使用的 `CompleteCommittedActivity` helper（原 `Committed→Executing→Completed` 前提不成立；`TryInterrupt` 才是真实终态路径）。
- 不再写死 `460/481/482/496/497`：由 `Work.Schedule.MorningStart` 与 `CommuteMinutes` 派生 `commuteBoundary`、`workStart`、`firstLaborMinute`，`ReadyMinute` 断言为 `commuteBoundary + commute`，劳动断言按 `(firstLaborMinute - workStart) × TestProgressPerParticipantPerDay` 计算。
- 强断言分界（全部保留业务语义）：
  1. **完成前**：通勤边界前一分钟 `QueryClaim` 仍为同一 `claimId`、实例 `Committed`；无 Arrival、未到场、`Progress=0`、`LaborRemainder=0`、原岗位 `LaborCount=0`/证据 0、施工名单仍 1 人。
  2. **合法终止**：真实边界后实例 `Interrupted`、`QueryClaim=null`；参与者集仍 `{person_alpha}`、施工名单仍 1 人（释放 claim 不等于撤出施工）。
  3. **到场分界**：同一边界登记 Arrival，`Destination=construction:<id>`、`ReadyMinute=边界+CommuteMinutes`、`HasArrived=false`；活动终止瞬间仍 `Progress=0/LaborRemainder=0`、原岗位证据 0。
  4. **劳动分界**：`WorkStart` 时 `HasArrived=true` 但 `LaborRemainder=0`；工作时段第一分钟才 `LaborRemainder=1`、`Progress=0`、`ExecutionPaused=false`；Primary Assignment、参与者集、原岗位名额与 0 证据全部保持。
- 未缩短产品活动、未改 Work/Clock/Activity、未清私有状态、未移除业务断言、未新增 busy 拒绝、未伪造 claim/Arrival。

### D. 编译与发现（本执行层实做）

- 仅通过 `script-execute` 调用 `AssetDatabase.Refresh(ForceUpdate)`；`EditorUtility.scriptCompilationFailed=False`、`EditorApplication.isCompiling=False`。
- 测试程序集 `VillageStage1.Tests.dll` 重编：mtime `2026-10-26T03:08:50.4871113Z`（晚于测试文件最终修改时间 `03:08:46.6725349Z`）。
- 反射 `RRPG.Management.Tests.ManagementProjectionTests`：`[Test]` 总数 **18**；第 18 例 FQN 唯一且可发现：
  `RRPG.Management.Tests.ManagementProjectionTests.ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability`（命中 1 次）；原 17 例方法均在。
- 测试文件：`54062B / f48f8f7d0cb9bfe299c3c8cbc1a6814540bd178495e632f907eedf21e9f16e0b`（最终修改后）。
- 本节写就时 `testResults` 仍为 §A 的历史 `Failed`；**本包未运行 `tests-run` / Test Runner**，不预填 PASS。

### E. 安全复核（修复后只读实测）

| 项 | 实测 | 判定 |
|---|---|---|
| Editor / 场景 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`；`SampleScene` 仅 1 个、`isDirty=false`、`rootCount=5` | 空闲、干净 |
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0` | 全一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` | 未变 |
| guard 根 | **176** 目录：`restored=175`、`protective-snapshot=1`、无 manifest 缺失、无其它状态 | 无未完成 / 未知 run |

未改产品源码 / Prefab / Scene / 字体 / 34 项基线 / 存档 / guard manifest / 配置 / Authority；测试源码不在 34 项基线内。未 `tests-run`、未全集、未 Shell / `Process.Start`、未 `rql-mutate`、未全局 Save / SaveChanges / SaveDirtyScenes、未写用户 Preferences、未 Git 写、未启停 Editor、未访问未授权工程 / 认证目录 / 其他工作区、未安装组件 / 改权限 / 创建额外付费子 Agent。制作人已取消 1280 测试，本包未派双尺寸、未等待人工。

### F. 未验证 / 交宿主

- 本执行层**未运行**该用例；修复后的 PASS 只能由宿主以**单方法 EditMode** 串行运行产出。
- 请宿主运行上述唯一 FQN，并保留 `[CLAIM-CONSTRUCTION]` 前缀日志（before work boundary / legal end / at work start / first labor minute）与真实回执；若仍失败，以宿主回执为准，不回写本节为 PASS。
- 本报告与修复需主开发专业核验，不代表制作人批准；R3-B 仍为 Implemented / Verification PARTIAL / Not Committed / Not Published。
