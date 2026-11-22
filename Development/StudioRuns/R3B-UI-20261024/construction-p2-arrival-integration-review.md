# R3B-UI-20261024 · 管理整类 1 败定位与修复（CASE-B 施工第二人 p2 到场登记 null）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一操作目录）；Unity 仅连接用户已打开的 Pilot Editor，**未启停 Unity**。
- 阶段：**1/1**（本阶段：定位施工第二人登记失败并修复集成专项）。后续由 Studio 阶段 check 继续，本包不越过门禁。
- 结论（先行摘要）：**根因不是通勤边界、不是命令失败、不是人物不可用，而是夹具选人前置非法**——420 分钟的真实自主活动机会若选中第二人，该人持有 Activity claim；生产 `RefreshWorkIntent → ArrivedWorkers` 会跳过被 claim 的人物（活动优先于工作到场登记），因此“加入施工成功”不等于“必登记到场计划”。已按**最小夹具前置修复**（真实 `QueryClaim` 只读筛选施工参与者 + 精确前置断言 + 可区分 Evidence），未改任何产品/核心/公共 API/DTO/Authority。

---

## 1. 本包实际做过 / 未做过（边界声明）

**做过（全部有工具真实回执）**

1. 只读 RQL/文件：Editor 空闲、场景干净、Console Error=0、`testResults` 6 条、6 个最新存档保护 run 的 manifest、34 项基线 JSON 头部、证据日志。
2. 只读源码：`ManagementShellPlayTests` 全段（CASE-B / SetUp / TearDown / SaveSlotGuard）、`ManagementView` 命令与按钮可用性、`ManagementCommandGateway`、`VillageRuntimeBridge`（Projects/Work/Buildings/Activities/Intraday/Reset）、`VillageIntradayWork`、`VillageProjectSystem`、`ActivityRuntimeBridge`/`ActivityLifecycleService`/`ActivityClaimRegistry`、`IntradayActivitySchedule`、`DecisionPlanner`/`FairArbitration`/`DecisionCoordinator`、`ManagementReadAdapter`/`ProductHomeReadAdapter`。
3. 编修：**只改** `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（测试夹具 + Evidence 助手，见 §5）。
4. 编译安全刷新：`script-execute` 仅调用 `AssetDatabase.Refresh()` 一次；随后回读无编译错误、无常量提示残留（§6）。
5. 写入本文件（`Development/StudioRuns/...` 文档，不在 34 项基线内）。

**未做过**

- 未运行任何测试（未 `tests-run`、未反射启动 TestRunner、未启动/重放模型、未忙等）。
- 未 `rql-mutate`；未 Shell / `Process.Start`；未 Git 写（commit/push/stage/切分支）。
- 未做全局 `SaveAssets / SaveChanges / SaveDirtyScenes`；`AssetDatabase.Refresh` 之外无任何保存。
- 未改场景 / Prefab / 资产 / ProjectSettings / Packages / 产品源码 / 静态配置 / 存档文件 / 备份 / manifest / 34 项基线 / 字体 / 证据日志。
- 未启停 Unity、未安装组件、未改权限、未创建任何子 Agent / Workflow / Goal。
- 未撤销 `NewGame` 的 `Restore(null)` 资源重置修复，未移除 CASE-C 的独立 Authority 断言，未读回旧库存当 expected。

---

## 2. 运行回执事实（Studio 宿主的整类 1 败）

`testResults`（本包只读回读，非本包运行）：

| 用例 | 状态 |
|---|---|
| `CloseButtonAndEscape_…` | Passed |
| `ExplicitSaveContinueAndNewGame_…`（CASE-C） | Passed |
| `Management_ProductEntry_…` | Passed |
| `**ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor**`（CASE-B） | **Failed** |
| `ManagementModal_RaycastBlocksWorldAndNavigation_…` | Passed |
| `ManagementWorkplaceCommands_…` | Passed |

失败原文（`testResults.message`）：

```text
p2 加入施工后必须登记真实到场计划
  Expected: not null
  But was:  null
```

`testResults.stackTrace` 精确指向 `ManagementShellPlayTests.cs:1418`（**修改前行号**，即“p2Arrival 不是 null”这一条断言）。因此：
- 行 1415 `参与者必须为 2` 已通过 → **p2 确实被真实命令加入施工**，命令本身成功；
- 失败点只在“p2 的真实到场登记（`IntradayWork.ToSaveData().Arrivals`）为 null”。
- 证据日志 `05:51:12` CASE-B 尾部同样停在 `p1 joined … participants=1; arrival=construction:building_instance_1 ready=494`，之后**无新证据**（与“断言前无后续证据”的回执一致），故不能从日志推断 p2 的 claim/availability，必须靠代码路径定位（§4）。

---

## 3. 只读前置核对（本包实际值）

| # | 检查项 | 实际值 | 判定 |
|---|---|---|---|
| 1 | Editor 空闲/非播放 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` | 通过 |
| 2 | 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`rootCount=5` | 通过 |
| 3 | Console 错误 | `console \| where logType=="Error"` = **0 条**（刷新前后各查一次） | 通过 |
| 4 | 最新 6 个存档保护 run | `055108Z`/`055109Z`/`055111Z`/`055111Z-2`/`055112Z`/`055113Z` 全部 `status=restored`、`restored=true`、`lastError=null`、`sourceBytes=backupBytes=13405`、`sourceSha256=backupSha256=4b2b56f6…d2d6aa`；未完成 run = 0 | 通过（无保护异常，未停） |
| 5 | 34 项基线文件本身 | `Development/StudioRuns/R3B-20261022/workspace-baseline.json`：`root=D:\AI_Agent_Lab\rrpg_pilot\repo`、`complete:true`、`head=054b00d3…`；本包未修改 | 通过（只读） |
| 6 | 受保护字体 | 基线内该条目 `bytes=14961033`、`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；本包未写入该文件 | 通过（只读；逐项重算由测试 `BASELINE-PRE/POST` 与 `FONT-PRE/POST` 硬断言权威执行） |
| 7 | 单例 vs 整类 | 同一 CASE-B 在 `2026-10-24T05:08:59Z`（单例/较早轮）**完整跑完**（`p2 joined … arrival ready=494`），在 `05:51:12Z`（整类串行）**在 p2 处败**；两次之间 **CASE-B 的断言与产品代码未变**（改动只有夹具选手 PersonId 与新增 CASE-C/资源重置） | 关键线索：⇒ 随机 PersonId → 活动选中结果不同 ⇒ 夹具不确定前置（§4） |

> 说明：本包**不**用旧回执代替本次证据，也**不**宣称“5 例旧版本通过即当前正确”。

---

## 4. 因果定位（先证实，再最小修改）

### 4.1 生产调用链（真实产品路径，逐段只读核对）

```text
ManagementView.AddSelectedPersonToConstruction()                     (ManagementView.cs:265)
  → ManagementCommandGateway.AddConstructionWorker(buildingId, personId)  (Gateway:74)
      · CheckPerson：人物存在 / Availability==InVillage / CanSaveWorld      → 通过（否则命令会返回失败）
  → VillageRuntimeBridge.TryAddConstructionWorker(bid, pid)           (Buildings.cs:49)
      · PresentWorkPersons().Contains(pid)                            → 通过（否则返回 PersonCurrentlyUnavailable）
      · Projects.TryCreateProject / 复用 b.ConstructionProjectId
  → TryAddProjectParticipant(projectId, pid)                          (VillageRuntimeBridge.cs:1118)
  → AddProjectParticipantCore → Projects.TryAddParticipant            (VillageProjectSystem.cs:84)
      · 参与者确实写入 → 测试“参与者必须为 2”通过（回执已证）
      · 成功 → RefreshWorkIntent()                                    (VillageRuntimeBridge.cs:1290)
  → RefreshWorkIntent()                                               (VillageRuntimeBridge.Work.cs:173)
      · 仅在 [MorningStart-Commute, WorkEnd) 窗口执行 → 479 在窗口内（MorningStart 480 / Commute 15）
  → ArrivedWorkers(Time.CurrentInstant.Minute, prepare:true)          (Work.cs:56)
      · present = PresentWorkPersons()：只由 Expedition / IsDead / _unavailable 决定
      · if (ActivityRuntimeBridge.QueryClaim(id) != null) continue;   ← 唯一“人在活动则不登记工作到场”规则
      · destination = Projects.GetActiveProjectOfPerson(id) → ProjectDestination(projectId)
                      → 该实例 ConstructionProjectId 已绑定 ⇒ 目的地必为 "construction:<id>"，非空
      · IntradayWork.Arrived(id, destination, minute)  ← 只有能走到这里才会写入 Arrivals
```

`WorkArrival(personId)` 直接读 `IntradayWork.ToSaveData().Arrivals`（同样走 `ToSaveData/Copy`，不做过滤），因此 **p2Arrival==null ⟺ ArrivedWorkers 跳过了 p2**。

### 4.2 排除法（为什么只能是 Activity claim）

| 候选原因 | 判定 | 依据 |
|---|---|---|
| 命令被域拒绝 / 未写入参与者 | **排除** | 回执显示 `参与者必须为 2` 断言已通过；失败在更后面的 p2Arrival |
| p2 不在村 / 不可用 | **排除** | `TryAddConstructionWorker` 的 `PresentWorkPersons` 与 Gateway `CheckPerson` 均通过；`VillagePersonAvailability.Resolve` 只看 Expedition / IsDead，与 claim/饥饿无关 |
| 目的地为空（无项目、无岗位） | **排除** | 加入成功后 `Projects.GetActiveProjectOfPerson(p2)` 必非空；`ProjectDestination` 经 `BuildingForProject` 返回 `construction:<instance>` |
| 通勤边界/时间重新计时（回执曾怀疑方向） | **排除** | 失败发生在**加入当刻**（Clock 暂停 479，`Assert.AreEqual(joinMinuteBeforeP2, p2RegistrationMinute)` 已通过）；不是 480/494 之后的时间断言 |
| `IntradayWork` 被替换 / Arrivals 被清 | **排除** | 同一调用栈内 `AddProjectParticipantCore` 先写入后立即 `RefreshWorkIntent`；无重建路径；p1 的登记行仍在（同一份 Arrivals） |
| **Activity claim（自主活动占用）** | **唯一剩余且自洽** | `ArrivedWorkers` 的第二条 continue 分支 |

### 4.3 claim 为什么会在这个用例里真实出现

- `IntradayActivitySchedule` 默认机会分钟 = **{420, 1020, 1200}**（`IntradayActivitySchedule.cs:15`），通勤 15 分钟，活动时长 60/90 分钟。
- 用例先 `AdvanceTo(479)`（真实 `Clock.AdvanceTo`），推进过程中在 **420** 命中活动边界：
  `DailyAdvanceSink.CompleteBoundary → CompleteActivityBoundary → CompleteWorkBoundary → OpenIntradayOpportunity(420)`（`VillageRuntimeBridge.Intraday.cs:169`、`VillageRuntimeBridge.Activities.cs:104`）。
- 420 时无人工作/无人持有 claim，`PresentWorkPersons()` 全部合格；`AutonomousSelectionRuntimeBridge.RunOpportunity` 产出提案后 `DecisionCoordinator` 对**非冲突提案全部授予 Winner**（`DecisionPlanning.cs:84-94`），
  `ScheduledGateway.Commit` → `TryCommitScheduledAtPlace` 在 **commit 当刻 `world.Claims.Acquire(claimParticipants, id)`**（`ActivityLifecycleService.cs:65`），
  到场 435、完成 495（60 分钟）或 510（90 分钟）。
- 因此 **420 被选中的任何人在 435–495/510 之间都持有 claim**，479 恰好落在这个区间。
- 结果：479 分钟“加入施工”的命令成功，但 `RefreshWorkIntent` 会跳过该人 ⇒ 没有到场登记 ⇒ p2Arrival==null。

### 4.4 与两份真实日志一致（不是猜测）

| 轮次 | 场景 | 结果 |
|---|---|---|
| `2026-10-24T05:08:59Z` | 较早轮（CASE-B 基本单独/较早执行），p1=`person_8223…`、p2=`person_bb7f…` | **完整通过**：`p2 joined construction at minute 479; arrival ready=494`，后续 480/481/494/796 全链断言通过 |
| `2026-10-24T05:51:12Z` | 整类 6 例串行，p1=`person_c950…`、p2=`person_0e95…` | **p2 处败**：p1 登记 479→494，p2 无登记 |

两次之间产品代码、CASE-B 断言、时间线（479/480/481/494/796）完全相同；**唯一变量是每轮 NewGame 生成的随机 PersonId**，而 420 活动选中谁是这些 PersonId 的确定性函数。⇒ 夹具对“第二人是否被活动占用”没有前置保证，属**夹具不确定前置**，不是产品回归。

> 备注（不越界、不修改）：当前产品在活动结束后（495/510）并不会为“已被加入施工但当时被 claim 跳过”的人补登记到场，因此该人当天可能一直不产生项目劳动。这是**产品行为问题候选**，涉及 WorkClock/工作意图核心与调度语义，按本阶段授权**不改**，留在 §9 风险表交主开发判定。

---

## 5. 修复（最小、只在测试夹具内）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（2000 → 2091 行）。
**未改**任何产品源码、场景、资产、DTO、公共 API、Authority、静态配置、存档格式。

### 5.1 新增只读助手（284–340 行）

| 助手 | 作用 |
|---|---|
| `ActivityClaim(personId)`（284–290） | `Static("ActivityRuntimeBridge","QueryClaim", id)` 只读真实 claim；不创建、不清除、不改相位 |
| `HostPersonIds()`（292–299） | 当前 World Snapshot 的真实 PersonId 顺序列表（不重排、不合成） |
| `PersonDiagnostic(personId)`（301–313） | 只读人物诊断：`availability`（快照投影）、`claim`、`primary`、`execution` |
| `ArrivalDiagnostic()`（315–329） | 只读真实到场登记全集 `PersonId→Destination@ReadyMinute`（直读 `IntradayWork.ToSaveData()`） |
| `ProjectDiagnostic()`（331–340） | 只读施工项目：`ProjectId / participants / progress / labor` |

所有助手只读真实产品查询；`PersonDiagnostic` 使用既有 `ProductHomeSnapshot` 投影（只读组合），**不调用** `GetPersonAvailability` 等可能触发运行时同步的入口，避免诊断副作用。

### 5.2 CASE-B 夹具前置（合法化，不放宽断言）

修改前：`p1 = 快照第 0 人、p2 = 快照第 1 人`，隐含假设“前两人在 479 分钟都不在自主活动中”。
修改后（1415–1434）：

```text
在 AdvanceTo(479) 之后、任何写操作之前：
  枚举 HostPersonIds()，逐个读取真实 ActivityClaim
  freePersons = { claim==null 且无既有 Primary Assignment }（保持快照顺序）
  Assert.GreaterOrEqual(freePersons.Count, 2, "…实际 N；claims=…")   ← 前置不成立就明确失败，不猜、不跳过
  p1 = freePersons[0]; p2 = freePersons[1]
  Evidence("CASE-B","activity claims at minute 479: <每人 id=claim>")
  Evidence("CASE-B","participants p1=… p2=…; arrivals=[…]")
```

- 之所以是**合法前置**而不是“削弱断言”：claim 是真实产品状态（人在自主活动中），产品**本来就**不为被 claim 的人登记工作到场（`ArrivedWorkers` 既有规则、`ManagementReadAdapter` 也把 claim 显示为“自主活动进行中”）。在本用例的契约（“登记分钟 + CommuteMinutes”）下，必须选“当前可登记”的人，才不会把另一条产品规则的合法表现误判成本用例失败。
- **未**直接制造 arrival、**未**清任何私有状态（未 `ResetForTests`、未 `Restore(null)`、未 `Leave()`、未改 claim/activity 实例）、**未**改 Clock、**未**跳过任何用例。

### 5.3 新增精确前置断言（只加不减）

```text
Assert.IsNull(ActivityClaim(p1), "p1 加入施工前不得处于自主活动占用（真实 claim 会跳过到场登记）：" + PersonDiagnostic(p1));   // 1457–1458
Assert.IsNull(ActivityClaim(p2), "p2 加入施工前不得处于自主活动占用（真实 claim 会跳过到场登记）：" + PersonDiagnostic(p2));   // 1493–1494
```

### 5.4 Evidence 增强（能区分“命令失败 / 未登记 / 边界错”）

| 位置 | 新增/扩写内容 |
|---|---|
| 479 前置（1428/1433–1434） | 全人物 claim 清单（含 `<null>`）、选定 p1/p2 的 availability/claim/primary/execution、当前 arrivals |
| p1 命令前（1455–1456） | `ProjectDiagnostic()`（此时应为 `project=<null>`）、p1 诊断、arrivals |
| p1 命令后（1483–1486） | 保留原字段 + `after=` 项目参与者/进度/劳动、`farmOccupancy=` 岗位占用、`arrivals=` 全量 |
| p2 命令前（1491–1492） | 参与者=1 的真实项目状态、p2 诊断、arrivals |
| p2 断言消息（1506–1507） | 失败时同时给出 `p2 诊断 / 项目参与者 / arrivals` |
| p2 命令后（1511–1514） | `arrival ready=` + `after=` + `farmOccupancy=` + `arrivals=` |

### 5.5 明确**未**改动（保持原强断言与既有修复）

- CASE-B 其余全部断言逐字保留：`ws_farm_01` 不变、参与者计数、`participation==1/2`、`ReadyMinute == 登记分钟 + CommuteMinutes`、`onsiteMinute > 481`、480/481 通勤期 `place=null`/非“项目劳动”/`labor=0`/`progress=0`、`onsiteMinute` 到场、`onsiteMinute+1` 劳动剩余>0 且 Progress=0、796 分钟 Progress>0、撤出/再加入与占用不变。
- CASE-C 全部断言（含独立 `FreshResourceAuthority()` 的 `authorityInitial` 断言、`Restore(null)` 资源重置、旧库存可辨别校验、Continue/NewGame 逐项连续）**未动**。
- `SaveSlotGuard`（Capture/Restore/BlockOnUnfinishedRuns）、`OneTimeSetUp/TearDown` 的 34 项 + 字体哈希断言、`[UnitySetUp]` 真实产品入口**未动**。
- 无 `[Ignore]`、无 `Explicit`、无分类过滤、无断言放宽、无 try/catch 吞失败。

---

## 6. 编译与刷新（唯一一次，安全刷新）

| 项 | 实际值 |
|---|---|
| 刷新方式 | `script-execute` → `AssetDatabase.Refresh()`（唯一写入 Editor 的操作，无其它保存 API） |
| 刷新后 Editor | `isCompiling=false`、`isUpdating=false`、`isPlaying=false` |
| Console Error | **0 条** |
| 测试程序集重编译 | `BattleStage1.PlayModeTests` 程序集写入时间 `2026-10-24T05:56:20.5601576Z`；源文件写入 `2026-10-24T05:56:15.9910566Z`（`sourceAfterAssembly=True`）⇒ 本次改动**已被编译** |
| 新助手可见性 | 反射确认 `ManagementShellPlayTests.ActivityClaim` 存在（返回 `String`） |
| 运行时依赖可见性 | `Type.GetType("ActivityRuntimeBridge, Assembly-CSharp")` 解析成功，`QueryClaim` 返回 `String` |
| 场景 | 刷新后仍 `SampleScene.isDirty=false`、`rootCount=5`（未被自动保存改写） |

---

## 7. 保护面现状（只读事实与权威来源）

| 面 | 事实 | 权威判定方 |
|---|---|---|
| 34 项基线 | 本包只读；`workspace-baseline.json` 未被修改（`root`/`complete` 已核对）。测试文件与本包文档都**不在** 34 项内 | 下一次整类运行的 `BASELINE-PRE diff=0` / `BASELINE-POST problems=0`（硬断言） |
| 受保护 TMP 字体 | 基线条目 `3c758e16…4854`、14961033 字节；本包未写入 | 下一次运行 `FONT-PRE == FONT-POST`、`unchanged=True`（硬断言） |
| 存档单槽 | 最新 6 个 run 均 `restored`，`sourceSha256 == backupSha256 == 4b2b56f6…`、13405 字节、`lastError=null` | 下一次运行新增的 `SAVE-BACKUP`+`SAVE-CAPTURE` 与 `SAVE-RESTORE matches-before=True`、新 manifest |
| 资源隔离 / Authority | 本包未改 `SaveSystem.NewGame`、`VillageResourceState`、`StudioTestResourceScope` | CASE-C 的 `authorityInitial` 与 `resourceScopeEnd` 断言 |

> 本包**未**写入任何 run 目录、manifest、备份、证据日志；上述状态由下一次整类运行继续产生。

---

## 8. 交宿主的执行方式（本包不执行）

```text
测试模式：PlayMode（Studio 阶段 check）
filter.testClass：ManagementShellPlayTests（短类名，串行一次，6 例）
前提：所有打开场景已保存（当前 SampleScene.isDirty=false）
```

**必须回读的本次证据（任一不满足即停、交人工）**

1. `testResults` 含 **6** 条 `ManagementShellPlayTests` 结果且全部 `Passed`（含 CASE-B 与 CASE-C）；条数 ≠ 6、出现 Skipped/Ignored/Inconclusive 一律不算通过。
2. 证据日志出现完整 CASE-B 段：
   - `activity claims at minute 479: …`（应能看到至少一个非 null claim，证明本包定位的现象真实存在）；
   - `participants p1=… claim=<null> … p2=… claim=<null> …`；
   - `p1 joined … ready=494`、`p2 joined … ready=494`，且 `arrivals=[…]` 同时包含两人，Destination 均为 `construction:building_instance_1`；
   - 若出现 `本夹具需要至少 2 名未被自主活动占用…` 失败 → **前置不足**（不是产品回归），需人工决定是否把该用例的活动机会窗口显式对齐（本包不改）。
3. 保护：成对 `SAVE-BACKUP` + `SAVE-CAPTURE runId=…` 与 `SAVE-RESTORE runId=… matches-before=True`；新 manifest `status=restored`、`restored=true`、`sourceSha256=backupSha256=4b2b56f6…`、`sourceBytes=13405`。出现 `captured`/`restore-failed`/hash 不匹配 → 停止，不清记录、不重试。
4. 保护面：`BASELINE-PRE total=34 diff=0 all-match`、`BASELINE-POST total=34 problems=0`、`FONT-PRE=FONT-POST 3c758e16… unchanged=True`、`resourceScopeEnd=scope-restored`。
5. 收尾：场景仍 `isDirty=false`；CASE-C `NEWGAME … authorityInitial=0/0 newIngredientsWereAuthority=True newFoodWasAuthority=True oldLeak=false` 不得退化。

---

## 9. 未测 / 风险 / 需主开发判定

1. **本包未运行测试**：CASE-B 是否在修复后通过、以及 6 例整类是否 5+1→6，全部以宿主阶段 check 回执为准；本文件不预填通过。
2. **尚未在真实运行中直接观测到 claim**：本包结论由“生产代码路径 + 失败断言行 + 两次真实日志差异”三重证据推出（§4.2 排除法已排除其余分支），并未在本包内读到 05:51 那轮 p2 的 claim 值。宿主下一次运行新增的 `activity claims at minute 479:` 行将**首次给出该直接证据**；若该行显示 p2 候选 claim 全为 `<null>` 却仍失败，则本文结论需推翻重查。
3. **前置容量风险（低）**：若某轮 420 机会把几乎所有人物都选中（claim 覆盖到只剩 <2 名未占用者），新前置断言会以明确文案失败而非误绿。这是夹具能力边界，不是产品断言放宽；是否要为该用例显式对齐活动机会窗口/时序，需主开发与设计确认后再动。
4. **产品行为问题候选（本包未改，越界）**：被 claim 的人加入施工后，活动结束（495/510）时没有任何 `RefreshWorkIntent` 补登记，导致该人当天可能一直“加入施工但不到场劳动”。修它需要改 Work 意图/调度核心或 `CompleteActivityBoundary` 的边界语义，属 WorkClock 核心，超出本阶段授权 → **留下证据，交主开发判定**。
5. **单例 vs 整类**：本缺陷是“随 PersonId 随机化的夹具前置”，因此单跑可能一直绿、整类串行才暴露（05:08 绿 / 05:51 败即实例）。修复后两者应一致，但“单例绿”不再作为整类正确的证据。
6. 本包未核对 34 项逐文件 SHA256（只读工具对超长单行 JSON 有截断），逐项等值判定仍以下一次运行的 `BASELINE-PRE/POST` 硬断言为准。

---

## 10. 本包产物

- 修改：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅 CASE-B 夹具前置、5 个只读诊断助手与 CASE-B 的 Evidence；+91 行，无删除既有断言）。
- 新增：本文件 `Development/StudioRuns/R3B-UI-20261024/construction-p2-arrival-integration-review.md`。
- 未新增/修改/删除：任何产品源码、场景、Prefab、资产、静态配置、存档、run 目录、manifest、备份、34 项基线、字体、证据日志。
