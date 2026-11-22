# R3-B 原规格逐项证据与剩余实施矩阵（只读专业覆盖审计）

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一授权目录）；分支 `ai/rrpg-r3b-population-pilot`；HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`（读 `.git/HEAD` + `.git/refs/heads/ai/rrpg-r3b-population-pilot`，未执行任何 git 命令）。
- Unity：用户已开的 Pilot `2022.3.62t9`（`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`），本包只做只读 `rql-query`，**未启动/关闭 Editor**。
- 阶段：**1/1 —— 原规格逐项证据与剩余实施矩阵**。后续由 Studio 自动继续；本文件不越过阶段验证门禁。
- 本包实际执行：`read` / `grep` / `glob` 读取 R3-P 规格、R3-A 执行记录、GameSpec Authority、当前实现与测试源码、`Development/StudioRuns/**` 既有回执；只读 `rql-query` 核对 Editor 空闲、场景干净、测试发现与最近结果。
- 本包明确未做：未运行任何测试（未 `tests-run`、未反射启动 TestRunner）、未 `AssetDatabase.Refresh`、未 `rql-mutate`、未 Shell/`Process.Start`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未改源码/场景/Prefab/资产/配置/测试、未读写用户存档、未改 34 项基线 / 14 项保护 / 备份 / manifest / 证据日志、未 Git 写（commit/push/stage/切分支）。
- **本文件是覆盖审计与剩余矩阵，不是 R3-B 完成声明，也不写正式项目状态收尾。**
- **2026-10-24 更新说明**：同一 Pilot 的管理命令 Edit 补测包只更正 §0.1 的宿主回执口径，并更新 B-01/B-02/B-05/D-01/I-02 等与本包新增用例直接相关的“未测/已补测试待宿主”状态；其余历史结论保持原样。新增证据见 `command-edit-gap-review.md`。

---

## 0. 本轮可复核的真实回执（只读）

| 项 | 实际值 | 来源 |
|---|---|---|
| Editor 状态 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | 本包 `rql-query` `editor` |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false` | 本包 `rql-query` `scenes` |
| Play 测试发现 | `ManagementShellPlayTests` = **6 条 PlayMode**，assembly `BattleStage1.PlayModeTests` | 本包 `rql-query` `tests` |
| 最近 Play 结果（当前 Editor 保留） | **6/6 `Passed`**（994 / 1556 / 585 / 684 / 566 / 834 ms），无 Skipped/Ignored | 本包 `rql-query` `testResults` |
| EditMode 测试发现 | `ManagementProjectionTests` = **13 条 EditMode**（本包补测后为 **17**，见 §0.1） | 本包 `rql-query` `tests` |
| Console | `Warning` 含 "Management" = 0 条；`Error` 1 条 `sequence 29620`，是**本包自己的无效查询**（`count` 函数不存在），不是项目错误（补测刷新触发 domain reload 后 Error 为 0，见 `command-edit-gap-review.md`） | 本包 `rql-query` `console` |

**回执等级说明**

- **E1 本轮可核**：仓库内文件、当前 Editor 只读状态、当前 `testResults`。
- **E2 任务给定但仓库不可核**：宿主执行记录 `775bbc05`、`resourceScopeEnd=scope-restored|font|0|632` 属 Studio 私有 runtime / 宿主侧字段，本包未访问、未读取，按任务给定登记，不以本文件冒充复核；原 `dsh:009683…` 的 `studio-test-record/0f7051f6-…json`（Edit 13/13）同属此类，由主开发核验，见 §0.1。
- **E3 历史版本限制**：R3-A 的 `Compiler264/264、EditMode1495/1495、PlayMode111/111`（2026-10-10）只对应 R3-A 交付版本，**不能当 R3-B 回执**；`ManagementProjectionTests` 的**旧失败历史**为 **12/13（1 Failed，WindowConflict）**（`labor-gate-last-result.json` 记录本地时间 `2026-10-23T13:32:23+08:00`）。**最新宿主回执**由主开发实际读取原 `dsh:009683ce04cc92aa46f40e3b0b7bfbfc2c7071f3a7dfe117021a3114d4ff1b09` 会话的 `studio-test-record/0f7051f6-a119-4b3b-9b25-040554837ff0.json`：`ManagementProjectionTests` **13/13 Passed、0 skipped**。本执行者没有该 Studio 私有 runtime 的访问能力，**该宿主记录由主开发核验，本文件不冒充复核**；旧 12/13 失败历史保留，**不得再说“最新仍是 12/13”**。
- **E4 推断**：CASE-B 旧失败（`p2Arrival == null`）的根因（420 活动 claim 跳过到场登记）由“生产调用链 + 失败断言行 + 两轮日志差异”推出，属推断，未在当轮直接观测失败者的 claim。

### 0.1 宿主 Edit 回执更正（2026-10-24，本包登记）

| 项 | 实际登记 | 来源与边界 |
|---|---|---|
| 会话 | `dsh:009683ce04cc92aa46f40e3b0b7bfbfc2c7071f3a7dfe117021a3114d4ff1b09` | 任务给定；本执行者无 Studio 私有 runtime / 该目录访问能力 |
| 记录 | `studio-test-record/0f7051f6-a119-4b3b-9b25-040554837ff0.json` | 主开发已实际读取并核验；**本包未读取该文件** |
| 结果 | `ManagementProjectionTests` **13/13 Passed、0 skipped** | 覆盖的是本包补测前的既有 13 例；不是本包新增用例的回执 |
| 旧失败历史 | 2026-10-23 `12/13`（1 Failed，WindowConflict） | 保留，不删除、不改写；不得据此再说“最新仍是 12/13” |

本包只据此更正覆盖矩阵的证据口径；**不声称新增 4 例已通过**。新增用例必须由宿主按短类名 `ManagementProjectionTests` 整类 EditMode 串行 check 后，才能写新的通过回执。

**本轮 Play 6/6 与证据日志的一致性**：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 最后一轮（`2026-10-24T05:58Z`）有 6 次 `ENTRY`、6 组 `SAVE-BACKUP/SAVE-CAPTURE` + `SAVE-RESTORE … matches-before=True`，`BASELINE-PRE/POST total=34 all-match`、`FONT-POST 3c758e16… unchanged=True`；与当前 `testResults` 6/6 一致。该日志**不包含** `resourceScopeEnd`（该字段由 Studio 测试框架产生）。

---

## 1. 证据/回执口径

1. **只有真实执行回执算 PASS**：本文件引用 `testResults`、证据日志、既有 run manifest；不把“编译通过”“测试被发现”“报告写了断言”当作运行通过。
2. **阶段/夹具前置不得当业务通过**：`Claim==null` 选人、UI 按钮禁用、把活动机会排到区间外等，都是夹具/前置，不是被测业务规则已通过。
3. **历史失败保持原样**：`05:51` 的 1 败（CASE-B）、`12/13` 的 EditMode 1 败、`15:53` 存档漏恢复，均保留失败事实，不改写为 PASS；其中 EditMode 1 败之后已有主开发核验的 13/13 宿主回执（见 E3 与 §0.1），两者是“历史失败 → 后续复跑通过”的关系，不互相抹除。
4. **当前实现与规格冲突**：规则以 GameSpec / R3-P 为准，实现冲突只记录，不在本包裁决或改写。

---

## 2. 逐项矩阵（规格条款 → 实现 → 测试/断言 → 回执 → 未测 → 后续最小动作）

> 通用限制（下表不再逐行重复）：Play 侧唯一当前回执是 `ManagementShellPlayTests` **6/6 Passed**（E1）；Edit 侧既有 `ManagementProjectionTests` 已有主开发核验的 **13/13 Passed、0 skipped** 宿主回执（E2，见 §0.1）。下表凡写“Edit 12/13 轮 Passed”的，是**保留的历史轮次**，不是最新口径。本包另新增 4 例后整类为 **17 例**，新增用例**尚未运行**，必须由宿主整类串行 check 后才能写通过回执。

### 2.1 人口 / 投影 / 选择

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| A-01 | R3-P §4「人口」：同一 Roster；姓名/职业/在村/职责/现时执行/临时承诺/饥饿可比较；按名称与状态筛选；过滤排序只属会话；选人不自动派工 | `ManagementReadAdapter.cs`（`Read` 60–70、`ReadPersons` 73–122）；`ManagementView.cs`（`nameSearch`/`statusFilterButtons`/`populationSummary`） | Edit `SameNameDifferentPersonIdsStayIndependentAndFilterIsSessionOnly`：3 个同名不同 PersonId；`Filter("林")=3`、不存在=0、`InVillage=3`、`NotInVillage=0`；`Read().Persons.Count` 仍 3（筛选不改来源）；派工后仅 alpha 有职责。Play `Management_ProductEntry_…`：人口行数 == 真实快照人数且克隆行 active；姓名过滤 0 行 + 空态 + 汇总变化；清空恢复 | Play 6/6（E1）；历史 Edit 该项在 12/13 轮 Passed（E3） | 状态筛选按钮（全部/在村/不在村）Play 未点；排序（DisplayName+PersonId）无断言；“选人不自动派工”无显式断言；职业/承诺/饥饿列 UI 文案未逐项断言 | Play 小断言包：状态筛选三态、选中不产生职责、关键列文案 |
| A-02 | R3-P §4/§5：读模型不初始化/写 Authority；同一数据快照 | `ManagementReadAdapter.cs`（只读 Bridge 查询）；`ManagementSnapshot.cs` | Edit `ReadProjectionDoesNotTouchWorldClockOrAuthority`：20 次 `Read()` 后 `BuildSaveData` JSON、Places、Assignment Records、ActiveProjects、Clock 分钟全不变 | 历史 Edit 12/13 轮 Passed（E3） | 未比较 Social / Activity / 资源 Save 区块；未验证 `QueryClaim` 读取无副作用（只读实现） | 可选：把对比面扩到 Social/Activity/Resource 的 SaveData JSON |
| A-03 | R3-P §4/§5：跨页/跨表面同一 Selection；岗位/施工候选只属会话；来源替换清选择 | `ManagementView.cs`（`SelectedPersonId` 派生、`_workplaceCandidate`/`_constructionCandidate`）；`HomeProductShell.cs`（`PersonDetailRequested`） | Play `Management_ProductEntry_…`：点人口行→Person Surface，`Selection.Id`==该 PersonId；关闭→World 且 Selection 保留；重开管理 `SelectedPersonId` 同一人。Play CASE-C：Continue/NewGame 后候选/检索/选择为空 | Play 6/6（E1） | **世界选中 Building/Place → 管理行定位**（`LocateSelectionIfNeeded`）只有源码核对，无运行断言 | Play 断言“世界选建筑→管理切到职责页并高亮对应实例行” |

### 2.2 职责与场所 / 派工撤职命令

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| B-01 | R3-P §4「职责与场所」+ Building GameSpec §17/18/19：已完成实例按 FunctionBindings 枚举 slot，实例+slot 独立，不按同名合并、不只取 FirstOrDefault；一行一 WorkplaceId | `ManagementReadAdapter.cs`（145 起，Completed 才枚举 bindings）；`VillageRuntimeBridge.BuildingWorkplaceId`；`ManagementCommandGateway.cs` 22–58 | Edit `SameDefinitionBuildingsAndRegisteredSlotsStayIndependent`：两 Farm 岗位 ID 不同、分别映射实例/slot/profile、`Capacity=2`、绑定人独立；注入第二 slot 后投影出现 `aux`，容量/绑定独立、slot 间名额独立。Play CASE-A：两 Farm `wa != wb` 且各含实例；p1→A 占 1、p2→A 占 2、B 为 0；CASE-C 动态 Farm/Kitchen 岗位 ID 绑定各自实例 | Play 6/6（E1）；历史 Edit 12/13 轮 Passed（E3） | **第二 slot 的“命令”路径未测**：Edit 只把自定义 registry 传给读投影，派工走 `TryAssignPerson` 直连；`Gateway` 校验使用真实 `StaticConfigRegistry`，故“多 slot 命令 + Capacity/CanAssign”无回执。Play 只有 main slot | **本包已只读评估**：生成资产 `Assets/Generated/StaticConfig/Databases/BuildingConfigDatabase.asset` 对 `building_farm/kitchen/workshop` 只登记 `main`；`StaticConfigRegistry` 无公共注入/注册 API，测试侧只能构造另一个 `BuildingRegistry` 给读投影。**不存在不改配置 Authority 的合法第二 slot Gateway 入口**；本包**不伪造**、不改生成配置，精确标为未测（详见 `command-edit-gap-review.md`） |
| B-02 | R3-P §4：未完工建筑不出可派工岗位；命令拒绝且零写入；0 worker 保留进度、不写 Pause 进度字段 | `Gateway` 36–38；`ManagementReadAdapter`（仅 Completed）；`ManagementView` 施工区 | Edit `UnderConstructionBuildingHasNoAssignableWorkplaceAndCommandRefuses`：投影无该岗位；施工行 `Progress=0/Target=10/WorkerCount=0/ExecutionPaused=true`；`Command=TargetNotCompleted`；`Assignments.Records=0`。Play CASE-B：0 worker 推进到 479 仍 `UnderConstruction`、无 Active Project、行 `Progress=0/WorkerCount=0` | Play 6/6（E1）；既有 Edit 13/13（E2，历史 12/13 亦保留） | 完工后 `AddConstructionWorker` 的 `TargetAlreadyCompleted` 拒绝原先无断言；Play 未尝试点“未完工的派工行”（行本来不出现） | **本包已补** Edit `CompletedBuildingRejectsConstructionWorkerWithoutWrite`（`TargetAlreadyCompleted` + SaveData/Projects/Resources/Clock 前后比对），待宿主整类 check |
| B-03 | R3-P §4：满员由领域返回；AlreadyAssigned 先撤职再派新岗；不自动 Remove→Assign、不重建原记录 | `Gateway` 54–57、`AssignMessage` 183–198；`VillageAssignmentSystem` | Edit `CapacityFullAndAlreadyAssignedPreserveDomainDiagnosticsWithoutWrite`：满员 `DomainRejected/CapacityFull`、gamma 无职责、记录 2；已有职责派 B → `AlreadyAssigned`、仍在 A、记录 2、B 占 0。Play CASE-A：满员目标 `assignButton.interactable=false`，对禁用按钮真实 Raycast 点击→p3 无职责、占用不变；已有职责再派 A→状态含“已有主要职责”、仍在 B、占用不变 | Play 6/6（E1）；历史 Edit 12/13 轮 Passed（E3） | **Play 从未真正进入 `CapacityFull` 域诊断**：UI 先禁用，证据行自述 `FULL-AT-UI disabled=true … CapacityFull 域名诊断未经按钮进入，见报告未测`（证据日志 05:58:12.791） | 域诊断已由 Edit 覆盖；若要求 Play 回执，需要一条**产品允许的可点但满员**路径——这属交互规则决定，先不改 |
| B-04 | R3-P §4：静态兼容岗位仍可见/可撤，但不作正式新增派工目标 | `ManagementReadAdapter`（LegacyAssignments）；`ManagementView`（兼容提示）；`Gateway` 27–29 | Edit `LegacyStaticAssignmentIsVisibleRemovableButNotANewTarget`：`Kind=LegacyCompatibility`、Capacity=2、绑定可见、`CanAssign=false`、派工 `StaticTargetNotAssignable`、撤职后占用 0。Play：只用 `ws_farm_01` 作 Primary Assignment 验证施工保留（CASE-B），**未点静态兼容行** | 历史 Edit 12/13 轮 Passed（E3）；Play 6/6 但未覆盖该 UI | Play 中静态兼容行的显示与“撤职”按钮真实点击未测 | Play 小断言：静态行可见、派工按钮禁用、撤职可用且占用归零 |
| B-05 | R3-P §4：撤职移除该 Assignment 记录及其累计工作统计，不包装成无损 Undo，不自动重建 | `Gateway.RemoveAssignment` 61–71 → `VillageRuntimeBridge.RemovePersonAssignment`（`VillageRuntimeBridge.cs` 1015–1020）→ `Assignments.RemoveAssignment` | Play CASE-A：撤 p1 → 无职责、wa 2→1、wb 0、状态“已移除”；再派 B 成功。Edit `LegacyStaticAssignment…`：撤职后静态记录消失、占用 0 | Play 6/6（E1）；既有 Edit 13/13（E2，历史 12/13 亦保留） | **“累计工作统计随撤职移除”原先无断言**（`TotalTestContribution` / 执行历史在撤职后的值未检查）；撤职与“仍在施工项目参与”叠加未测 | **本包已补** Edit `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution`：真实跨两个午夜累计后撤职，记录/累计统计删除、村庄级已结算贡献不回滚、下一工作日旧岗位不新增；待宿主整类 check。Play 侧叠加仍未测 |
| B-06 | R3-P §7 风险：`RemovePersonAssignment` 没有 `RefreshWorkIntent`；“劳动中撤职→后续贡献/旧意图”需专项 | `VillageRuntimeBridge.cs` 1015–1020（**确认无 RefreshWorkIntent**）；`Work.cs` `WorkDestination` 19–23、`ArrivedWorkers` 56–69、`CompleteWorkInterval` 109–151、`RefreshWorkIntent` 173–177 | Edit `RemovingAssignmentDuringLaborStopsOldWorkplaceEvidence`：481 已“岗位劳动”、证据>0；撤职后 `GetWorkExecutionLabel=="空闲"`、`BoundCount=0`；`Advance(483)` 后旧岗位证据**不再增加**、`Arrivals()` 不再含该人 | 历史 Edit 12/13 轮 Passed（E3） | 见 §3 D-01：**“立即”只有边界/下一区间级证据**；到场登记清除发生在下一边界 `ArrivedWorkers(prepare)`；无 Play 实时断言；无“已结算分钟不可追回”的说明性断言 | 先只读确认意图清理时机，再补“同一分钟不新增证据 + 下一边界 arrival 清除 + 已结算分钟不追回”的断言；若要求真正即时清意图→领域变更，需制作人授权（见 D-01 选项） |

### 2.3 施工命令 / 劳动口径

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| C-01 | R3-P §4：施工只走 `TryAddConstructionWorker`/`TryRemoveConstructionWorker`；原 Primary Assignment 保留；名额≠施工人数；真实到场才贡献 | `VillageRuntimeBridge.Buildings.cs` 49–80；`Gateway` 74–111；`Work.cs` 109–151 | Edit `ConstructionWorkerKeepsPrimaryAssignment`：加入/撤出都不改 `ws_farm_01`，项目参与者正确、施工行工人数正确。Edit `ConstructionLaborDoesNotCountTowardPrimaryWorkplace`：施工时标签“项目劳动”、原岗位 `BoundCount=1`/`LaborCount=0`、项目 Labor 增长、原岗位证据不增。Play CASE-B：479 加入→480/481 通勤零贡献→494 到场零贡献→495 劳动>0→796 `Progress>0`；撤出→participants 1、primary 不变、`execution != 项目劳动`；再加入→2 | Play 6/6（E1）；历史 Edit 两项在 12/13 轮 Passed（E3） | 同一建筑**完工后仍是同一实例**、完工后施工行/项目结束无断言；两栋建筑同时施工/双项目并行未测；撤出后“停止贡献”见 C-02 | 扩展 CASE-B 到完工边界（或改用 Edit 完成路径）断言同一实例完工与职能岗位激活；双施工项目另立小用例 |
| C-02 | R3-P §4：撤出施工只移除参与、不改 Primary；R3-P §8「劳动与资源」：加入/撤施工立即影响合法后续意图 | `VillageRuntimeBridge.Buildings.cs` 71–80（撤出时 `IntradayWork.Leave` + `RefreshWorkIntent`） | Play CASE-B：撤出当下 participants 1、primary 不变、`execution="通勤"`（证据日志 05:58:11.460：`p1 removed at minute 796; participants=1; primary=ws_farm_01; execution=通勤`）；再加入 participants 2 | Play 6/6（E1） | **撤出后未推进边界断言项目 `LaborRemainder/Progress` 不再增加**；也未断言重新加入后必须重新到场（证据行自述 `next-boundary re-arrival not claimed`） | Play 在同一用例推进 1–2 个边界，断言撤出后项目劳动/进度不再增；再断言重新加入后的到场边界 |
| C-03 | Village §2.3 / R3-P §2：Activity Commitment/Participation Claim 是 Window 内短期占用，不写入 Temporary Occupancy；工作到场对 claim 者跳过 | `Work.cs` 56–69（`ArrivedWorkers` 中 `QueryClaim != null` 即 `continue`）；`Activities.cs` 46–96（`CompleteActivityBoundary` **没有** `RefreshWorkIntent`） | 现行 Play 夹具：`activity claims at minute 479: …` 记录每人 claim；只从 `claim==null && 无既有职责` 的人里选 p1/p2，并加前置断言（`ManagementShellPlayTests.cs` 1415–1434、1457–1458、1493–1494）。旧失败轮 05:51 的 `p2Arrival==null` 是 E4 推断根因 | 新夹具后 05:58 轮 6/6（E1）；**旧失败者 claim 从未当轮直接观测**（E4） | **“claim 持有者加入施工”的真实产品行为没有断言**，现绿只证明“无占用者”路径；活动结束后不补登记的工作意图缺口未测 | 新增专项（不改产品）：让真实 claim 持有者加入施工，断言命令结果、是否有到场登记、活动结束后是否/何时补登记；结果交主开发决定是否产品包（见 §4 与 §5） |
| C-04 | R3-P §4：施工加入/撤出成功后更新视图 | `ManagementView`（命令后 `RefreshNow`） | Play CASE-B 撤出/再加入后施工行参与者数随真实项目变化（同用例断言 participants 1→2） | Play 6/6（E1） | 未单独断言行 `WorkerCount/Progress` 在撤出后立即刷新（只断言项目状态） | Play 小断言施工行 WorkerCount 同步 |

### 2.4 人物不可用 / 世界忙 / 保存

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| D-01 | R3-P §7：产品动作必须从当前源重查；远征/死亡/消失/busy 不提交 | `Gateway.CheckPerson` 130–159（存在 / `VillagePersonAvailability` / `CanSaveWorld`）；`VillageAvailability.cs`（仅 Expedition / IsDead） | Edit `MissingOrAwayPersonIsRejectedWithoutWrite`：`person_missing`→`PersonMissing`、记录 0；`SetExpeditionPersonIds(["person_beta"])`→`PersonNotInVillage`、无职责、记录 0；投影 `Expedition`/“远征中（保留职责）” | 既有 Edit 13/13（E2，历史 12/13 亦保留） | **原先未测 Unavailable（死亡）**；**原先未测命令级 WorldBusy**（只有 `SaveWorld` 的 WorldBusy Edit 回执）；未测“世界推进中”命令禁用 | **本包已补**两个 Edit 用例（待宿主整类 check）：`DeadUnavailablePersonIsRejectedByEveryManagementCommandWithoutWrite`（真实 `IsDead` → 派工/加入施工/撤职/撤出均 `PersonNotInVillage` 零写入）；`WorldBusyRejectsManagementCommandsWithoutWrite`（既有公共远征入口使 `CanSaveWorld=false` → 四类命令均 `WorldBusy` 零写入，不反射私有锁） |
| D-02 | R3-P §4：显式保存调用 `SaveSystem.Save(source)` 并显示 bool；失败不称已保存、不回滚、可重试；busy 时禁用 | `Gateway.SaveWorld` 114–127；`ManagementView.Save` 218–230 | Edit `SaveFeedbackUsesExistingBoolAndRespectsWorldLock`：强制失败→`SaveRejected/SaveReturnedFalse`、文案含“未成功”、职责保留；远征→`WorldBusy`。Play CASE-C：成功文案“已保存当前世界。”+ `ManagementView.Save()==true` + 单槽存在 + `"SaveVersion": 1` + `"Ingredients":` + UI 候选/检索不入档；强制失败→文案含“未保存/内存状态保留”、文件 hash 不变、资源/批次/证据/历史/进度/Clock/职责/施工参与逐项不变；复位后再成功 | Play 6/6（E1）；历史 Edit 12/13 轮 Passed（E3） | “保存失败后 UI 列表与快照一致性”只由再次成功保存间接覆盖；未测磁盘写入异常的其它形态 | 已足够；如需更严，可补“失败后刷新管理页仍与内存一致”的断言 |

### 2.5 资源 / 饥饿

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| E-01 | R3-P §4「资源」：当前 Ingredients/Food、在村供餐对象、已有 Hungry 人数；列出 Farm/Kitchen 实例、名额与实际劳动人数；无写按钮/无预测 | `ManagementReadAdapter.cs` 60–70、`ReadFulfillment`、`ReadResources`；`ManagementView` 资源页 | Edit `ResourcesAndNutritionFactsComeFromExistingState`：`AddIngredients(4)/AddFood(7)` → 投影 4/7、`InVillageCount=3`、`InVillagePersonIds` 三人、`HungryPersonIds=[person_beta]`；Farm 行 `RoleLabel=Farm`、`WorkplaceId`、Capacity 2；人物行 `HasNutritionFacts/Hungry/ConsecutiveUnderfedDays=2` | 历史 Edit 12/13 轮 Passed（E3） | Play 只断言资源汇总含“食材 N/食物 N”，**未断言在村供餐数、Hungry 数、Farm/Kitchen 行与“导航到职责”**；批次/供餐明细无投影（规格允许后置） | Play 小断言资源页文案与行导航；批次/供餐薄只读投影另立包（需新 Query 契约，不伪造） |
| E-02 | R3-P §4：管理页不 Add/Consume 库存、不手动加工 | `ManagementView`（无资源写按钮） | Play CASE-C：`RefreshNow` 多次后 Ingredients/Food 不增减；`ManagementView` 无资源写入口（39 引用中无对应按钮） | Play 6/6（E1） | 无“确认没有资源写按钮”的显式负向断言（按字段清单间接证明） | 可选：断言资源页不存在写按钮（按字段/事件） |

### 2.6 保存 / Continue / NewGame

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| F-01 | Save GameSpec：单槽 `SaveVersion=1`；Continue/Load 恢复；无 Load Tick/Drain/Join/生产/历史重复 | `SaveSystem.Save/Load`；`VillageRuntimeBridge.BuildSaveData/Restore…` | Play CASE-C 段 5：`SceneManager.LoadScene("SampleScene")` → `MainMenuUI.RefreshContinueButton` → 真实 `btnContinue.onClick`（`OnContinue→Load→EnterTown`）；新 host/shell InstanceID 不同；换源且 `TownUI.PlayerData` 即新源；Clock `Paused` 且分钟=存档；存档 hash 不被 Load 改写；动态岗位/占用/施工项目 ID/参与者/Progress/LaborRemainder/p1 到场 Destination+ReadyMinute/资源/批次/证据/执行历史/管理施工行/资源投影逐项连续；UI 候选/检索/Selection 为空 | Play 6/6（E1）；证据行 `CONTINUE-FACTS …` | 只覆盖单一世界、单槽、无远征 Return / 事件 pending 的 Continue | 现有覆盖已足；Return/pending 的 Continue 另立远征 gating 包 |
| F-02 | R3-P §4：NewGame 清旧世界、无旧实例/职责/候选泄露；Hero 选择 gating 不被绕过 | `SaveSystem.NewGame` 225–240（**资源重置已落盘**：`if (VillageRuntimeBridge.Resources != null) VillageRuntimeBridge.Resources.Restore(null);` 第 238 行）；`HeroSelectionUI.OnConfirm` | Play CASE-C 段 6：`DataManager.InitialPersonRoster.Count` 动态（不写死）；未选英雄确认按钮禁用→真实卡片按钮启用→真实确认按钮；`SaveSystem.HasSave()` 证明产品真写；Clock=0；新 host/shell；换源；合法 PersonId 集合=（配置角色 + 真实英雄）且与旧源无交集；旧人物无职责；管理人口投影逐 PersonId 等价；候选/检索/选择为空；旧建筑不在岗位/施工行；资源==`VillageResourceState` 初始 Authority、批次/证据/历史归零、刷新不再增减 | Play 6/6（E1）；05:58 证据 `NEWGAME roster=6 … resources=0/0 authorityInitial=0/0 batches=0 evidence=0 completed=0 clock=0 newIngredientsWereAuthority=True newFoodWasAuthority=True` | 未覆盖 NewGame 时远征/事件 pending 未结算、Activity/Social/Return 区块清理；资源重置只覆盖 Ingredients/Food | NewGame 组合边界专项（先由制作人确认范围）；**不得撤销现有 `Restore(null)` 与 CASE-C 断言** |
| F-03 | R3-P §4/§8：保存连续性、无重复生产/历史；保存失败明确提示 | 同 F-01/F-02 | 同 F-01/F-02 | Play 6/6（E1） | 见 F-01/F-02 | 同 F-01/F-02 |

### 2.7 UI / Prefab / 模态 / 输入

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| G-01 | R3-P §5：固定 UI 通过 Editor 可编辑 Prefab；Runtime 只生成重复行；原白模退出 | `Assets/Resources/UI/ManagementView.prefab`（282097B / `ade4c7e1…`）；`HomeProductShell.cs` 72–81；`ManagementView.cs` | Play `Management_ProductEntry_…`：`Shell.Management` 是 authored Prefab 实例、GameObject 名 `ManagementView`、`IsVisible`、`SurfaceModalActive`/`IsPersonModalOpen` true；白模 `HomeInternalWhitebox` inactive；三页签 active；**39 个序列化对象引用、0 null**；人口行数==快照人数且行 active；页签切换 | Play 6/6（E1）；Prefab 结构另有 `prefab-binding-report.md` 静态 35 字段/39 引用回读 | 4 个行模板中 **workplace/construction/resource 行的运行时克隆未单独断言**（CASE-C 通过 `FindRow` 间接使用）；Prefab 的 Editor 可编辑性只有资产存在与静态回读，无“在 Editor 中打开修改”证据 | Play 断言三页签行数与模板克隆；Editor 侧另做一次只读绑定回读（不写资产） |
| G-02 | R3-P §5：全屏 Raycast Blocker + World 输入阻断；TopHUD 可操作；ESC 与关闭同一回调；旧 Town 输入恢复 | `ManagementView.prefab`（`ModalBlocker` 顶部 88px 让位）；`HomeProductShell.cs`（`SurfaceModalActive`、`SetTownInputBlocked`、`HandleEscape`）；`ProductHomeWorldHost.cs`（`IsPersonModalOpen` 早退） | Play `ManagementModal_…`：背景点 Raycast top hit 属 Blocker/面板、无 `IPointerClickHandler`；`HandleWorldClick` 不改 Surface/Selection；底部“建造”被遮罩、`Build.IsOpen==false`；TopHUD Pause/Speed 真实可点并改变 Clock；手动 Pause 跨切页/关闭/重开保持；`IsSimulationActive` 始终 true。Play `CloseButtonAndEscape_…`：`TownUI.canvasRoot` CanvasGroup 禁用/恢复；关闭与 ESC 同一 World Surface/同一 Selection | Play 6/6（E1） | **镜头拖拽/滚轮未注入验证**；键盘输入（除 ESC）穿透未测；管理页内嵌 Growth pending 时的 ESC 未测（只有源码路径） | Play 输入包：镜头 pan/zoom、键盘、嵌套 Growth pending；不改产品 |
| G-03 | R3-P §5：滚动长列表可读；中文字体；多分辨率（1920×1080 / 1280×720）；模态内 TopHUD 可见 | `ManagementView.prefab`（四个 ScrollRect + Viewport/Mask + Content VLG/Fitter；CanvasScaler 1920×1080 match 0.5；TMP `RRPG_SourceHanSansCN_Regular_TMP` dynamic） | 无可执行断言。静态证据：`prefab-binding-report.md` §6 记录 4 个滚动区结构、1920 参考布局与 1280 等比缩放推算、14 个字符未预烘；Play 只验证行/页签/按钮，**未做滚动拖拽、未切分辨率、未做字体渲染断言** | 静态 Prefab 回读（E1 级资产事实）；运行期无回执 | **滚动实际拖动/滚轮**、**1920/1280 实际画面与文字裁切**、**14 个缺字在运行期是否补入**、**物理鼠标/拖拽手感**全部未测 | 视觉/输入验收包（Play 截图 + 分辨率切换 + 滚动拖拽 + 物理鼠标人工体验）；不改 UI 结构、不改字体资产 |
| G-04 | R3-P §5：内部页不调用 `SetSimulationActive(false)`/Pause/改 Speed；手动 Pause 保持 | `HomeProductShell.cs`（`RefreshNow`/`CloseSurface` 不触碰 Clock）；`ManagementView`（无 Clock 调用） | Play `ManagementModal_…`：打开管理 `IsSimulationActive==true`；手动 Pause 后切页/关闭/重开仍 `Paused`；TopHUD 点击可改变 Clock 且不关闭管理 | Play 6/6（E1） | 管理页打开期间“不自动暂停”在**真实自然时间流逝**下的表现未测（夹具从 Paused 开始） | 可选：Play 以 Normal 速度打开管理，断言 Clock 继续推进且未改 Speed |

### 2.8 Simulation 对照 / 远征 gating

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| H-01 | R3-P §8「Simulation 对照」：同一测试世界同一 seed/目标分钟，World、Person、Management 及成长模态无操作时实际 Clock/Work/Activity/Social/Resource 结果一致；Pause 保持；正常/加速/Jump/Return 共用唯一推进 | 无“表面专属 Clock”；`HomeProductShell`/`ManagementView`/`PersonDetailView` 只读；`VillageRuntimeBridge` 唯一推进 | 无对应测试。相近证据：Edit `IntradayPathEquivalenceTests`（normal/fast/jump/Return 到同一目标分钟，比较 Village `BuildSaveData` + ProductHome Snapshot + Social facts + Selection/Feedback SaveData 完全一致）；Edit `ReadProjectionDoesNotTouchWorldClockOrAuthority`（读投影不改 Clock/Authority）；Play 模态用例只断言 `IsSimulationActive` 与 Pause 保持 | 上述两项 Edit 在各自轮次 Passed（E3 级，非本轮）；**没有 World/Person/Management 三表面同 seed 同分钟对比** | **规格§8 的 Simulation 对照行实质未实现**：没有在三个表面分别停留后逐项比较 Work/Activity/Social/Resource；也没有“成长模态打开”参与对照 | 新 Play 包（新文件，串行）：同 seed 推进到目标分钟，分别在 World / Person / Management 停留并读取同一组真实查询逐项等值；不改产品 |
| H-02 | R3-P §5/§8「远征/事件 gating」：Home 既有入口→准备→远征/Return→Home；busy/pending 保持既有优先级；关闭内部页不解锁 Return 或启动第二 Clock | `VillageRuntimeBridge.CanSaveWorld`；`HomeProductShell.Navigate` 的 `SurfaceModalActive` 早退；`Expedition` gating 链（本次未改） | Play：管理打开时通过真实 Raycast 实测“建造”导航被遮罩且 `Build.IsOpen==false`；其余导航项（含远征 index==5）由 `Navigate` 的 `SurfaceModalActive` 早退（源码级，未逐项实测）。Edit `SaveFeedback…`：设置远征集合后 `SaveWorld` → `WorldBusy`。**没有真实远征准备/出征/Return 的 Play** | 历史 Edit 12/13 轮 Passed；Play 6/6（E1） | **远征/Return gating 无 Play 回执**：busy/pending 时命令与保存禁用、Return 不被内部页解锁、关闭内部页不启动第二 Clock、事件 PauseOnOpen 优先级均未实测 | Play 远征/Return gating 包（新文件，串行，复用既有 Expedition V4F 产品夹具） |

### 2.9 编译 / 全量回归 / 保护

| # | 规格条款 | 实现文件 | 具体测试方法与业务断言 | 真实回执 / 历史版本限制 | 未测 | 后续最小动作 |
|---|---|---|---|---|---|---|
| I-01 | R3-P §8：实施验证按 Compile/Console→专项 Edit→专项 Play→受影响完整 Edit/Play 串行 | 全部 | 当前 Console：项目错误 0（唯一 Error 是本包无效查询）；`isCompiling=false`；CASE-B 夹具修复后曾 `AssetDatabase.Refresh` 编译通过、`BattleStage1.PlayModeTests.dll` 晚于源码（`construction-p2-arrival-integration-review.md` §6、`construction-boundary-fixture-report.md` §5） | 编译级通过（E1/E3）；**无 R3-B 的完整 Compiler 回归** | R3-B 全量 Compiler 未跑；R3-A `264/264` 是历史（E3） | 串行：Compiler → 全 EditMode → 全 PlayMode |
| I-02 | R3-P §8：完整 Edit / Play 串行；不并行 Test Runner、不把 0 发现算 PASS | 全部 | `ManagementShellPlayTests` 6/6 已由宿主串行跑过（当前 `testResults` 可核）；`ManagementProjectionTests` 的既有 13 例已有主开发核验的 **13/13 Passed、0 skipped** 宿主记录（E2，§0.1）；历史 `12/13`（1 Failed WindowConflict）保留 | Play 专项 6/6（E1）；Edit 既有 13 例 **13/13**（E2）；本包新增 4 例**待宿主** | **完整 EditMode / PlayMode 全量回归未跑**；新增 4 例未跑；R3-A `1495/111` 不可用 | 串行：`ManagementProjectionTests` 整类（新增后 **17** 例）→ 全 EditMode → 全 PlayMode；每轮前后核 34/字体/存档 |
| I-03 | R3-P §8：单槽档测试前后字节备份/恢复；不得用历史备份覆盖用户新进展 | `ManagementShellPlayTests.SaveSlotGuard`（Capture/Restore/BlockOnUnfinishedRuns/OneTime 兜底） | 05:58 轮：6 组 `SAVE-BACKUP`+`SAVE-CAPTURE` 与 6 组 `SAVE-RESTORE … matches-before=True`，全部 `13405B / 4b2b56f6…d2d6aa`；`BASELINE-PRE diff=0`、`BASELINE-POST problems=0`、`FONT-PRE=FONT-POST 3c758e16… unchanged=True` | E1（证据日志 + 当前 testResults） | 保护只在 TearDown（原生 abort 不恢复）；历史 `2026-10-23T15:53` 一轮 5 次 Capture 无 Restore 的根因未定；`21075B / 650c45c4…` 原档在授权范围内未找到备份 | 历史档恢复需制作人单独授权取证；本包不读档、不恢复、不覆盖 |

---

## 3. 专项审计结论（逐条）

### D-01 劳动中撤职：旧 WorkIntent 与贡献“立即停止”

**事实（只读源码）**

- `TryAssignPerson` 成功后调用 `RefreshWorkIntent()`（`VillageRuntimeBridge.cs` 1010）；**`RemovePersonAssignment` 没有任何 `RefreshWorkIntent`**（1015–1020）。
- `WorkDestination` 直接派生自“Active Project → Primary Assignment”（`Work.cs` 19–23），所以撤职后下一帧起 `GetWorkExecutionLabel` 立即变为“空闲/下班”，这是**标签立即变化**，不等于意图记录已清。
- 贡献在 `CompleteWorkInterval(from,to)` 用 `from` 分钟的 `ArrivedWorkers(prepare:true)` 与本刻 `Assignments` 计算（`Work.cs` 109–151）；撤职后下一区间已排除该人，因此**下一区间不再累计**。
- 到场登记行（`IntradayWork.Arrivals`）只在 `ArrivedWorkers(prepare:true)` 时清理；Edit 用例在 `Advance(483)` 后断言其已移除，说明清理发生在**下一边界/下一区间**，不是撤职当刻。

**证据边界**：Edit `RemovingAssignmentDuringLaborStopsOldWorkplaceEvidence` 只证明“撤职后旧岗位不再新增证据 + 下一边界清意图”，**不能冒充“立即停止”**；没有同一分钟/Play 实时断言，也没有“已结算分钟不可追回”的断言。

**后续最小动作（先测试，不改核心）**：① 只读时序评估（确认 `RemovePersonAssignment` 是否应补 `RefreshWorkIntent`）；② Edit/Play 增补断言：撤职当刻前后同一分钟不新增证据、下一边界 arrival 清除、已结算区间不回滚；③ 若产品要求“真正即时清意图/追回”，属 Work/Clock 核心语义变更，**需制作人裁决**，不在本包改。

### D-02 多实例 / 多 slot / full / AlreadyAssigned / 未完工

- **已覆盖**：两实例独立、实例+slot 绑定、未完工不派工与命令拒绝、满员 `CapacityFull` 域诊断（Edit）、AlreadyAssigned 不自动换岗（Edit + Play）、UI 满员前置禁用（Play）。
- **未覆盖**：① 第二 slot 的**命令**路径（Edit 只把自定义 registry 给读投影，派工用直连 `TryAssignPerson`；Gateway 用真实 registry）；② 完工后 `AddConstructionWorker` 的 `TargetAlreadyCompleted` 拒绝；③ Play 真正进入 `CapacityFull` 域诊断（证据自述未经按钮）。
- **结论**：核心命令语义（满员/已有职责/未完工）已有 Edit 级证据；**多 slot 命令**与**完工后施工拒绝**仍空白。

### D-03 away / unavailable / busy / missing 不写

| 情形 | 现有拒绝点 | 证据 | 缺口 |
|---|---|---|---|
| missing | `Gateway.CheckPerson` → `PersonMissing` | Edit `MissingOrAwayPersonIsRejectedWithoutWrite` | 无 |
| away（远征） | `VillagePersonAvailability` → `PersonNotInVillage` | 同 Edit 用例；Edit `SaveFeedback…` 的 `WorldBusy` | 无（命令级） |
| unavailable（死亡） | `VillagePersonAvailability`（`IsDead` → `Unavailable`）→ `PersonNotInVillage` | **无测试** | Edit/Play 均未断言 |
| busy（世界推进/返程） | `Gateway.CheckPerson` 的 `CanSaveWorld` → `WorldBusy` | 只有 `SaveWorld` 的 `WorldBusy`（Edit） | 派工/施工命令的 `WorldBusy` 分支未测 |
| busy（人物被 Activity claim / 临时占用） | **无任何拒绝**（见 §4 与 §5） | 无 | 产品行为未定义/未测 |

### D-04 施工撤出后停止贡献

- `TryRemoveConstructionWorker` 成功且确为参与者时调用 `IntradayWork.Leave(personId)` + `RefreshWorkIntent()`（`Buildings.cs` 71–80），即**意图立即切换**；Play 证据 `execution=通勤` 与之一致。
- 但 Play 在撤出后**没有推进任何边界**就重新加入，未断言 `LaborRemainder/Progress` 停止增长，也未断言重新加入后重新到场。**“撤出后停止贡献”目前只有意图级证据，没有计量级证据。**

### D-05 Simulation World/Person/Management 同 seed 同目标分钟连续

- **未实现**（见 H-01）。现有最近似证据是 Edit `IntradayPathEquivalenceTests`（normal/fast/jump/Return 持久世界完全一致）与读投影无副作用；**没有任何用例在 World / Person / Management 三个表面分别停留后比较 Work/Activity/Social/Resource**。
- 结论：规格§8 该行是**明确未测**，不能由“打开管理不暂停”“读投影不改权威”替代。

### D-06 Pause / Return gating 远征

- 已证：管理打开不 `SetSimulationActive(false)`、手动 Pause 保持、TopHUD 可操作、管理模态阻断底部导航（Play 实测“建造”被遮罩；其余导航靠 `Navigate` 的 `SurfaceModalActive` 源码早退）、`SaveWorld` 在远征集合存在时 `WorldBusy`。
- 未证：真实“远征准备→出征→Return→回村”链、busy/pending 事件下命令与保存禁用、关闭内部页不解锁 Return、不启动第二 Clock。
- 结论：**远征 gating 只有源码/旁证，无 Play 回执**。

### D-07 资源现状 / 饥饿

- 已证（Edit）：`Ingredients/Food` 真实值、在村供餐对象集合、Hungry 集合、人物 `Hungry/ConsecutiveUnderfedDays`、Farm 行容量。
- 未证（Play）：资源页“在村供餐人数/其中 Hungry”文案、Farm/Kitchen 行导航、批次/供餐只读投影（规格允许后置）。
- 结论：**数据层已覆盖，UI 层只覆盖汇总数字**。

### D-08 保存 / Continue / NewGame 无重复

- 已证（Play CASE-C，当前 6/6）：成功保存文案与 bool、单槽 JSON `SaveVersion=1`/`Ingredients`、UI session 不入档；失败保存不动盘、不回滚、可恢复；Continue 换源/新 host/Clock/岗位/施工/资源/批次/证据/历史/到场逐项连续且不重复；NewGame Hero gating、旧世界不泄漏、资源回 `VillageResourceState` 初始 Authority（`Restore(null)`）、批次/证据/历史归零。
- **保留项**：`SaveSystem.NewGame` 的 `Restore(null)` 与 CASE-C 独立 Authority 断言必须保留；历史 `650` 存档缺口与 Return/pending 的 Continue 不在本轮范围。
- 结论：**规格§8 Save/Continue/NewGame 行在单一世界/单槽范围内已真实通过**；跨远征 Return / 事件 pending 仍缺。

### D-09 Prefab 编辑绑定 / 滚动 / 多分辨率 / 物理体验

- **绑定**：Play 断言 39 引用 0 null、authored 实例、三页签；Prefab 静态 35 字段回读（`prefab-binding-report.md`）——**已证**。
- **滚动**：只有静态结构（4 个 ScrollRect + Viewport/Mask + Content VLG/Fitter），**无运行期拖拽/滚轮断言**。
- **多分辨率**：只有 1920 参考布局与 1280 等比缩放推算，**无运行期画面断言**。
- **物理体验**：Play 全部是 `EventSystem.RaycastAll + ExecuteEvents.PointerClick` 程序化点击，报告多处明写“不是物理鼠标人工体验”，**物理手感/拖拽/滚轮未测**。
- **中文字体**：受保护 TMP 动态字体，14 个字符未预烘（静态记录），运行期补字未断言。

### D-10 Compiler 及全 Edit/Play 未跑项

- **已跑**：`ManagementShellPlayTests` 6/6（当前 `testResults` 可核）；CASE-B 夹具修复后的编译。
- **未跑**：R3-B 全量 Compiler 回归；修复后的 `ManagementProjectionTests` 13 项；完整 EditMode；完整 PlayMode；远征/Return gating；Simulation 对照。
- **不能引用**：R3-A 的 `264/264、1495/1495、111/111`（历史版本，E3）。
- **结论**：R3-B **不具备“整包回归完成”条件**。

---

## 4. CASE-B 自由候选规避 claim：能证明什么、不能证明什么

**事实链（仓库内可核）**

1. 旧实现下 `ManagementConstructionCommands_…` 曾在整类串行中 1 败：`p2 加入施工后必须登记真实到场计划 Expected: not null But was: null`（`construction-p2-arrival-integration-review.md` §2，来自宿主回执）。
2. 该报告的根因定位是**推断**：`ArrivedWorkers` 对 `QueryClaim != null` 的人 `continue`（`Work.cs` 63），夹具随机选到 420 活动被选中的第二人；报告 §9.2 明写“尚未在真实运行中直接观测到 claim”。
3. 修复方式：**只从 `claim==null && 无既有职责` 的人中选 p1/p2**，并新增 claim 前置断言与诊断证据（`ManagementShellPlayTests.cs` 1415–1434、1457–1458、1493–1494）。没有改产品、没有放宽业务断言、没有清 claim。
4. 修复后 05:58 轮实测：`activity claims at minute 479: person_3dd5…=<null>; person_e9b6…=<null>; person_d6e7…=<null>; person_676f…=village.intraday|420|free|39:person_676f…|70:15:prototype.drink…|activity; person_319d…=<null>; person_92c6…=<null>;`（证据日志 05:58:11.268）——**当轮确实直接观测到 479 有 `prototype.drink` 的活动 claim**，且被选中的 p1/p2 均无 claim。
5. 同一轮 p1/p2 加入施工、登记 `ready=494`、480/481 通勤零贡献、494 到场、495 劳动、796 `Progress=1`，6/6 通过。

**边界（必须保留）**

- 该绿只证明：**“两名未被活动占用的人加入施工 → 到场/劳动/进度”这条路径通过**。
- **不证明**：claim 持有者加入施工时产品行为正确；也**不证明**旧失败者的 claim 当时是什么（05:51 那轮 p2 的 claim 没有当轮直接观测）。
- **根因措辞保留推断**：`construction-p2-arrival-integration-review.md` 的因果结论是“生产代码路径 + 失败断言行 + 两轮日志差异”的推断；新证据只证明“479 存在 claim 现象”可复现，**没有把旧失败者的 claim 变成直接观测**。不得把本文件或该报告改写成“已证实该人当时被 claim”。

---

## 5. Gateway 对 Activity claim / `GetPersonOccupancyReason` 的现有规则核对（不改）

### 5.1 现状（只读证据）

| 检查点 | 实际规则 | 证据 |
|---|---|---|
| `ManagementCommandGateway.CheckPerson` | 只校验：人物存在 → `VillagePersonAvailability.Resolve(...)`（远征/死亡）→ `CanSaveWorld` | `ManagementCommandGateway.cs` 130–159 |
| Gateway 是否调用 `GetPersonOccupancyReason` | **没有** | 全文件无该调用（grep 仅 `ManagementReadAdapter.cs:104`、`ProductHomeReadAdapter.cs:22` 用于投影） |
| Gateway 是否调用 `ActivityRuntimeBridge.QueryClaim` | **没有** | 同上 |
| `GetPersonOccupancyReason` 覆盖 | 远征 / `_unavailablePersonIds` / “Active Project 且 `IsExecutingWork`” | `VillageRuntimeBridge.cs` 1333–1355 |
| `GetPersonOccupancyReason` 是否覆盖 Activity claim / Rest | **不覆盖** | 同函数；Activity claim 不在 Temporary Occupancy Authority（Village §2.3 明确“不写入现有 Temporary Occupancy Authority”） |
| Activity claim 的唯一产品效果 | `ArrivedWorkers` 跳过 → 不登记工作到场；`IsExecutingWork` 为 false；管理投影显示“自主活动进行中：…” | `Work.cs` 56–69、24–27；`ManagementReadAdapter.cs` 104–109 |
| 活动结束是否补登记 | `CompleteActivityBoundary` 不调用 `RefreshWorkIntent`；补登记只能等下一个工作边界（`480/465/780`） | `Activities.cs` 46–96；`Work.cs` 155–157、173–177 |

### 5.2 判定

- **Gateway 目前没有任何“人物 busy（Activity claim / 临时占用）拒绝”规则**；它只拒绝“不在村/远征/死亡”和“世界忙”。
- 这不是“实现漏了一行”那么简单：按 Village §4.2/§4.3 与 §2.3，Primary Assignment 是长期安排、Activity claim 不写入 Temporary Occupancy，**在 claim 期间禁止派工/加入施工会改变现有规则语义**。因此不能由本包自行加“busy 拒绝”。
- 可以确认的真实产品现象候选：claim 持有者加入施工**命令成功**，但因 `ArrivedWorkers` 跳过而不登记到场；活动结束后没有立即补登记，当天上午的施工劳动可能丢失（05:51 失败即此形态的推断）。这与 R3-P §7“busy 不提交”的期望存在差距。

### 5.3 窄修选项（只列，不在本包实施）

| 选项 | 范围 | 是否改规则/核心 | 备注 |
|---|---|---|---|
| N-0 只加测试记录现状 | 仅测试（新文件） | 否 | 先用 Play 记录“claim 者加入施工”的真实命令结果与到场/补登记，作为裁决输入。**推荐先做** |
| N-1 仅 UI 提示/禁用 | `ManagementView` + `actionHint`（读 `QueryClaim`） | 是（把自主活动变成派工禁用，属交互规则） | 不改 Gateway/领域，但需要制作人确认“活动期间禁派工” |
| N-2 Gateway 增加只读前置 | `ManagementCommandGateway.CheckPerson`（新增 outcome 或复用 `PersonNotInVillage`/`WorldBusy`） | 是（公共契约/错误语义） | 影响所有调用方；需单独契约审计，不自行改 |
| N-3 活动结束补登记 | `VillageRuntimeBridge.Activities.cs`（`CompleteActivityBoundary` 后 `RefreshWorkIntent`）或 Work 边界语义 | 是（Work/Clock 核心） | 超出本阶段授权，按 Stop Condition 交制作人 |
| N-4 改 `GetPersonOccupancyReason` 纳入 claim | `VillageRuntimeBridge.cs` 1333 | 是（Temporary Occupancy Authority，与 Village §2.3 冲突） | 不建议；会建立第二套占用语义 |

**结论**：本包只记录证据与选项，**不修改 WorkClock 核心、不新增规则、不改 Gateway**。

---

## 6. 后续包建议（依赖顺序 / 文件所有权 / 可并行性）

> 原则：先纯测试补证据，再决定是否改产品；Unity 测试一律**串行**，不并行 Test Runner；纯文档/只读包可与测试包并行。

| 包 | 性质 | 依赖 | 文件所有权（唯一写者） | 内容 |
|---|---|---|---|---|
| P1（本包） | 只读 + 文档 | — | `Development/StudioRuns/R3B-UI-20261024/requirement-evidence-gap-matrix.md` | 覆盖矩阵与缺口（已完成） |
| P2 EditMode 补测 | 纯测试，串行 | 无产品改动 | `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`（或新增 `ManagementCommandGuardTests.cs`，不与他包共用） | dead / 命令级 `WorldBusy` 零写入；撤职后累计统计清除；完工后 `AddConstructionWorker` 拒绝；必要时补“多 slot 命令”前置评估 |
| P3 Play 断言补测 | 纯测试，串行 | P2 后（避免同 Unity 会话与同文件冲突） | `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 撤出后停止贡献（推进 1–2 边界）；claim 持有者加入施工（N-0）；静态兼容行撤职点击；资源页 Hungry/供餐文案；状态筛选；施工行 WorkerCount 刷新 |
| P4 Simulation 对照 | 新 Play 文件，串行 | P3 后 | 新文件 `Assets/Tests/PlayMode/ManagementSimulationEquivalencePlayTests.cs` | 同 seed/目标分钟，World/Person/Management 三表面 Work/Activity/Social/Resource 逐项等值 |
| P5 远征/Return gating | 新 Play 文件，串行 | P4 后 | 新文件 `Assets/Tests/PlayMode/ManagementExpeditionGatingPlayTests.cs` | 准备→出征→Return→Home；busy/pending 命令/保存禁用；内部页不解锁 Return、不启第二 Clock |
| P6 视觉/输入验收 | Play + 人工，串行 | P3 后（可与 P4/P5 排期相邻但不并行 Runner） | 测试文件若新增则独立；截图/产物放忽略临时目录 | 1920/1280 画面、滚动拖拽、物理鼠标、14 缺字、镜头 pan/zoom |
| P7 产品决策包 | 领域变更，需制作人 | 必须在 P3/N-0 结果之后 | `ManagementCommandGateway.cs` 或 `Work/Activities`（届时由主开发指定） | claim 期间派工/施工规则、活动结束补登记、撤职是否即时清意图 |
| P8 历史档取证 | 只读，需授权 | 独立 | 无仓库写入 | 历史 `21075B/650c45c4…` 存档缺口的恢复/取证；不得覆盖用户新进展 |

**串行顺序建议**：`P2 → P3 → P4 → P5 →（P6 可选）→ 全 Edit → 全 Play → 专业核验`。P7 在 P3 结果出来后由制作人裁决，未裁决前不改核心。

---

## 7. 保护面与不得越界的事实

- **34 项基线 / 14 项保护 / 字体**：05:58 轮测试硬断言 `BASELINE-PRE total=34 diff=0 all-match`、`BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`、`FONT-PRE=FONT-POST 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854 unchanged=True`。本包未重算、未修改 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`。
- **字体历史**：`e40b6d77…/14968490` 变化已由维护方备份并定向恢复为 `3c758e16…/14961033`；直接写入路径为原执行器 `rql-mutate` 普通 call 后的全局 `SaveChanges`（见 `R3B-Font-Recovery-20261023/maintenance-handoff.md`）。本包遵守：不用 `rql-mutate`、不全局保存、刷新只用 `AssetDatabase.Refresh`（本次未刷新）。
- **存档**：当前单槽 `13405B / 4b2b56f6…d2d6aa`（来自 6 个 `restored` manifest 与 `SAVE-RESTORE matches-before=True`）；**`21075B / 650c45c4…` 是历史缺口**，`2026-10-23T15:53Z` 一轮 5 次 Capture 无 Restore，授权范围内未找到原始备份。本包未读取用户档、未恢复、未覆盖。
- **GameSpec Authority 不变**：Village §2.3、§4.2/4.3/4.4/4.5、§12.4；Product Home §21/§23；Building §17/§18/§19；Save §1；R3-P 仍是当前管理开发规格。没有新增 Person / Assignment / Occupancy / Work / Activity / Save Authority。
- **文档滞后事实（不掩盖）**：当前 `Docs/项目状态快照.md` / `Docs/文档地图.md` 首页仍是 V17-R3-A 的“R3-B 未启动”，因为 R3-B 只在 Pilot 分支推进、尚未发布；这属已知的“文档滞后”，**不表示 R3-B 已完成，也不表示其已发布**。本包不修改这些状态文档。

---

### 7.1 本包读取入口（Bootstrap / Authority / 证据）

按根 `AGENTS.md` 顺序实际读取：

1. `README.md`；2. `AI_CONTEXT.md`；3. `Docs/文档地图.md`；4. `.clinerules.md`；5. `GameDesign/README.md`；6. `GameSpec/README.md`；7. `Docs/项目状态快照.md`（首页状态段）。

任务指定/对应 Authority 与证据：

- `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md` §2、§4–§8（本矩阵主规格）。
- `Docs/DevelopmentSpecs/2026-10-10_RRPG_V17-R3-A_Person-Detail_Execution-Record.md`（R3-A 真实交付；验收前 24 路径 / Final Closeout 22 路径；模态/选择/Growth 接缝）。
- `GameSpec/Village/Village-GameSpec-V0.1.md` §2.3、§4.2–4.5、§7、§12；`GameSpec/Village/Product-Home-World-GameSpec-V0.1.md` §21/§23；`GameSpec/Village/Village-Building-World-GameSpec-V0.1.md` §17–§19；`GameSpec/Save/Save-GameSpec-V0.1.md` §1。
- 实现：`Assets/Scripts/Village/ProductHome/{ManagementCommandGateway,ManagementReadAdapter,ManagementSnapshot,ManagementView,ManagementRowView,HomeProductShell,ProductHomeWorldHost}.cs`；`Assets/Scripts/Village/{VillageRuntimeBridge,VillageRuntimeBridge.Work,VillageRuntimeBridge.Buildings,VillageRuntimeBridge.Activities,VillageRuntimeBridge.Intraday}.cs`；`Assets/Scripts/Village/Runtime/VillageAvailability.cs`；`Assets/Scripts/Save/SaveSystem.cs`。
- 测试：`Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`；`Assets/Tests/PlayMode/ManagementShellPlayTests.cs`；参照 `Assets/Tests/EditMode/AutonomousLife/Selection/IntradayPathEquivalenceTests.cs`。
- 既有 StudioRuns 回执：`R3B-tools-20261022/capability-check.md`；`R3B-Recovery-20261023/{labor-projection-review,activity-window-test-fix,window-conflict-fixture-fix,ManagementProjectionTests.json,labor-gate-last-result.json}`；`R3B-UI-20261023/{controller-report,prefab-binding-report,shell-integration-report,shell-play-test-report,management-actions-play-report,font-protection-incident-review,construction-arrival-fix-report}` 与 `play-evidence/management-shell-play-evidence.log`；`R3B-UI-20261024/{construction-boundary-fixture-report,construction-p2-arrival-integration-review,save-continuity-play-report,newgame-resource-reset-review,continue-roster-count-fix,save-single-case-preflight,save-single-case-resume-preflight,management-six-case-preflight}.md`；`R3B-SaveProtection-20261024/{save-guard-review.md,*/manifest.json}`；`R3B-Font-Recovery-20261023/maintenance-handoff.md`；`R3B-20261022/workspace-baseline.json`。

---

## 8. 本包产物与不声称

- **唯一写入**：本文件 `Development/StudioRuns/R3B-UI-20261024/requirement-evidence-gap-matrix.md`。
- 未新增/修改/删除：源码、测试、场景、Prefab、资产、配置、成长数据、字体、存档、run 目录、manifest、备份、基线、证据日志。
- **不声称**：R3-B 整包完成、全量回归通过、远征 gating 通过、Simulation 对照通过、滚动/多分辨率/物理体验通过、claim 场景通过、历史存档缺口已修复。
- 本文件的完成报告须由主开发专业核验，**不能代制作人批准**；后续由 Studio 自动继续，不越过阶段验证门禁。
