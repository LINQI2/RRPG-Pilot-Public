# R3-B 管理命令 Edit 边界证据补测（command-edit-gap-review）

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一授权目录）；Unity 工程：`UnityProject\RRPG1.0game`。
- 分支 / HEAD：`ai/rrpg-r3b-population-pilot` / `054b00d3e38fe2e2e142bfdb2502de90f7991030`（取自 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`；本包未执行任何 git 命令）。
- 阶段：**1/1 —— 补齐管理命令 Edit 边界证据**。只完成本阶段；后续由 Studio 自动继续，不越过阶段验证门禁。
- 本包**代码唯一写入**：`UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`。
- 本包**文档写入**：本文件；并按任务要求更正 `requirement-evidence-gap-matrix.md` 的宿主回执口径与相关“未测/已补测试待宿主”状态。
- 本包明确未做：未改产品源码 / 公共 API / DTO / WorkClock / 领域规则 / 配置 Authority；未 `rql-mutate`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 Shell / `Process.Start`；未 Git 写；未读写用户存档；未改 34 项基线 / 14 项保护 / 备份 / manifest / 证据日志 / 字体 / 场景 / Prefab。
- **本包未自行启动任何测试**（未 `tests-run`、未反射启动 TestRunner、未忙等）。新增用例的通过与否必须由宿主整类串行 check 决定；本文件不预填通过。

---

## 1. 运行前只读核对（本包实际回读）

| # | 检查项 | 实际值 | 判定 |
|---|---|---|---|
| 1 | Pilot 身份 | `unity=2022.3.62t9`；`project=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 通过 |
| 2 | 基线身份 | `branch=ai/rrpg-r3b-population-pilot`、`head=054b00d3e38fe2e2e142bfdb2502de90f7991030` | 与任务给定 Pilot 一致 |
| 3 | Editor 空闲 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | 通过 |
| 4 | 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`rootCount=5` | 通过 |
| 5 | 34 项保护 SHA | `workspace-baseline.json` 解析 34 项；逐项重算 `bytes+sha256`：`total=34 matched=34 problems=0` | 通过 |
| 6 | 受保护字体 SHA | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，与基线条目一致 | 通过 |
| 7 | 存档保护未完成 run | `R3B-SaveProtection-20261024` 共 16 个 manifest：15 个 `restored`、1 个 `protective-snapshot`（`pendingRestore=false`）；**未完成 run=0**；当前单槽均为 `13405B / 4b2b56f6…2d6aa` | 通过 |
| 8 | 测试发现（改前） | `ManagementProjectionTests` = 13 条 EditMode、无 ignored / parameterized | 通过 |

> 34 项与字体 SHA 由本包用只读 C# 代码逐文件重算，不引用历史口头结论；未写入基线文件。
> 存档结论来自 manifest 的 `status/restored/pendingRestore` 与 source 字节/SHA 字段；本包未读取用户存档本体。

---

## 2. 宿主 Edit 回执口径更正（保留旧失败历史）

- 主开发实际读取原 `dsh:009683ce04cc92aa46f40e3b0b7bfbfc2c7071f3a7dfe117021a3114d4ff1b09` 会话的 `studio-test-record/0f7051f6-a119-4b3b-9b25-040554837ff0.json`，真实结果为 `ManagementProjectionTests` **13/13 Passed、0 skipped**。
- 本执行者没有 Studio 私有 runtime / 该 `studio-test-record` 目录的访问能力，**未读取该记录**；上条按主开发已核验的宿主证据登记，不由本文件冒充复核。
- 旧失败历史保留：2026-10-23 `12/13`（1 Failed，WindowConflict，`labor-gate-last-result.json` 本地时间 `2026-10-23T13:32:23+08:00`）。该历史与后续 13/13 是“旧失败 → 后续复跑通过”的关系，均不删除、不改写。
- 因此：**不得再说“最新仍为 12/13”**。上述 13/13 只覆盖本包补测前的既有 13 例；**不覆盖本包新增的 4 例**。

---

## 3. 本包新增的 Edit 用例（只改测试，不改产品）

文件：`UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`（改后 751 行、41449 bytes、`sha256=f20a120ead170e9796b06fb9c59e3edcffc64c33c943e6b9f5ad79f036629ec5`）。
既有 13 例的测试名、断言与活动 schedule 恢复逻辑保持不变；新增 4 例，整类发现数由 13 变为 17。

### 3.1 `DeadUnavailablePersonIsRejectedByEveryManagementCommandWithoutWrite`

- 前置：完成 1 座 Farm 并把 `person_alpha` 派到 `main`；另放 1 座施工中 Farm 并让 `person_alpha` 真实加入施工项目。
- 通过既有长期人物真相源把 `person_alpha` 的 `CharacterInstance.IsDead` 置为 true（与 `BuildPersonInputs` 的真实读取链一致；未反射造私有锁、未新增钩子）。
- 依次执行：`AssignToWorkplace` / `AddConstructionWorker` / `RemoveConstructionWorker` / `RemoveAssignment`。
- 独立业务断言：
  - 四个命令均返回 `PersonNotInVillage`、`Succeeded=false`，说明文案含“不可用”；
  - `BuildSaveData` JSON 前后完全一致；
  - Clock 分钟不变；
  - 原 Primary Assignment 仍在原岗位，Assignment 记录数不变；
  - Active Project 数不变，项目参与者集合不变；
  - Ingredients / Food 不变；
  - 管理投影人物行 `Availability=Unavailable`、`AvailabilityLabel="不可用"`。
- 不只比较结果码：同时比较了 Assignments / Projects / Resources / Clock 与 SaveData 快照。

### 3.2 `WorldBusyRejectsManagementCommandsWithoutWrite`

- 前置：完成 1 座 Farm 并派工 `person_alpha`；另放 1 座施工中 Farm 并让 `person_alpha` 真实加入施工项目。
- **只调用既有公共远征入口** `VillageRuntimeBridge.SetExpeditionPersonIds(["person_beta"])`，使真实 `CanSaveWorld=false`；未反射私有锁、未新增测试钩子、未走 Return 私有状态。
- 依次执行四类管理命令（派工 / 加入施工 / 撤出施工 / 撤职）。
- 独立业务断言：
  - 四个命令均返回 `WorldBusy`、`Succeeded=false`；
  - `BuildSaveData` JSON 前后完全一致；
  - Clock 分钟不变；
  - 原职责、Assignment 记录数、Active Project 数、项目参与者集合、Ingredients / Food 均不变；
  - `CanSaveWorld` 仍为 false（命令未自行解除世界忙）。
- 局限：本用例证明的是“既有远征公共入口造成的 WorldBusy”；未单独制造“Advance 会话进行中 / Return 结算中”的私有锁，因为任务禁止反射私有锁。该两类状态仍由既有产品门禁与宿主阶段负责，不在本用例宣称已覆盖。

### 3.3 `CompletedBuildingRejectsConstructionWorkerWithoutWrite`

- 前置：用既有 `Complete` 路径完成 1 座 Farm（同一实例、`ConstructionProjectId == null`）。
- 执行 `AddConstructionWorker(completedBuildingId, "person_alpha")`。
- 独立业务断言：
  - 返回 `TargetAlreadyCompleted`、`Succeeded=false`，文案含“已完工”；
  - `BuildSaveData` JSON 前后完全一致；
  - Clock 分钟不变；
  - 完工实例的 `ConstructionProjectId` 仍为 null（拒绝后不得新建施工项目）；
  - Assignment 记录数、Active Project 数、Completed Project 数、Ingredients / Food 均不变；
  - 完工实例不重新进入施工投影 `Read().Construction`。
- 未改 `TryAddConstructionWorker` / `ManagementCommandGateway` 的任何规则。

### 3.4 `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution`

- 通过既有公开注入点 `ConfigureActivitySchedule` 把机会点排到推进区间之外（`{4000}`），避免真实 Activity claim 干扰累计统计前置；`TearDown` 恢复本测试开始前日程。未改产品规则、未清 claim。
- 完成 Farm → 派工 `person_alpha` → `Advance(2881)` 跨两个午夜，让真实劳动证据经 `HandleDayTick` → `Assignments.CommitLabor` 结算进 Assignment 累计统计。
- 前置硬断言：`record.WorkedMinutes > 480`、`record.TotalExecutedDays >= 1`、`Assignments.TotalTestContribution > 0`。
- 执行 `RemoveAssignment` 后断言：
  - 原 `VillageAssignmentRecord` 被删除（`GetRecord == null`），岗位占用归零；
  - 村庄级 `TotalTestContribution` 与撤职前一致（**已结算证据不回滚、不追回**）；
  - `Assignments.ToSaveData().TotalTestContribution` 同样不变。
- 再推进到下一个工作日 `Advance(3361)` 后断言：
  - 旧岗位不重建 Assignment；
  - 村庄级累计贡献不因旧岗位增加；
  - 旧岗位 Work Evidence 不再增加；
  - 管理投影旧岗位 `LaborCount=0`。
- 说明：`TotalTestContribution` 是既有 Village 级聚合；单条 Assignment 的 `WorkedMinutes/TotalExecutedDays/ContributionRemainder` 随记录删除，聚合值不回滚——这是当前领域契约，不是本包新增规则。

---

## 4. 第二 slot 的 Gateway 路径：精确标为未测，不改配置 Authority

**只读核对结论**

- `Assets/Generated/StaticConfig/Databases/BuildingConfigDatabase.asset` 中 `building_farm` / `building_kitchen` / `building_workshop` 的 `FunctionBindings` 只有 `FunctionSlotId: main`，没有 `aux` 等第二 slot。
- `StaticConfigRegistry.TryGetBuildings` 每次从生成的 `BuildingConfigDatabase` 创建 `BuildingRegistry`；`StaticConfigRegistry` **没有公共注入 / 注册 Building 定义或 FunctionBinding 的 API**。
- `ManagementCommandGateway.AssignToWorkplace` 同时要求：
  1. `StaticConfigRegistry.TryGetBuildings` 返回的 `FunctionBindings` 存在目标 slot；
  2. `VillageRuntimeBridge.Assignments.GetWorkplaceDef(workplaceId)` 已真实注册。
- 既有 Edit `SameDefinitionBuildingsAndRegisteredSlotsStayIndependent` 里的 `RegistryWithSecondSlot()` 只把自定义 `BuildingRegistry` 传给**只读投影** `Read(definitions)`；派工走 `TryAssignPerson` 直连。它不能证明 Gateway 的第二 slot 命令路径。
- 若只 `Assignments.TryRegisterWorkplace(aux)` 而不改生成配置，Gateway 仍会在 `FunctionBindings` 检查处返回 `TargetNotRegistered`；这不是“第二 slot 命令成功路径”。

**结论**

- 在“只改测试、不改产品 / 公共 API / 配置 Authority、不用私有反射注入”的边界内，**不存在合法的第二 slot Gateway 路径注入点**。
- 因此本包**没有为第二 slot Gateway 路径新增测试**，也没有修改 `BuildingConfigDatabase.asset` 或任何配置 Authority；该路径在覆盖矩阵中精确标为**未测**。若后续要补，需要配置 Authority 的所有者先合法增加第二 slot，再由新包补 Gateway 命令测试。

---

## 5. 安全编译与发现（本包实际执行）

- 唯一刷新动作：通过 `script-execute` 调用 `AssetDatabase.Refresh()`；未使用 `rql-mutate`、未全局保存。
- 刷新后：`isCompiling=false / isUpdating=false / isPlaying=false / isPaused=false`。
- Console Error：刷新前唯一 Error 是 `sequence 29620` 的旧无效 RQL 查询（`unknown function 'count'`，非项目错误）；刷新触发 domain reload 后 `console | where logType == "Error"` = **0 条**。未发现 `error CS` 编译错误。
- 测试发现：`tests | where fullName starts-with "RRPG.Management.Tests.ManagementProjectionTests."` = **17 条 EditMode**，assembly `VillageStage1.Tests`，无 `ignored`；新增 4 个方法名均被发现。
- 本包**未运行**这 17 条测试；发现数不等于通过数。

---

## 6. 交宿主的唯一执行方式（本包不执行）

```text
测试模式：EditMode（Studio 阶段 check）
短类名：ManagementProjectionTests
串行：单次运行；不并行、不 split、不追加其它 filter
```

运行前：确认 Editor 空闲、打开场景已保存（当前 `SampleScene.isDirty=false`）。
运行后必须回读：

1. 结果集条数 = **17**，全部 `Passed`；任何 `Skipped / Ignored / Inconclusive / 0 项 / 部分执行` 都不 PASS。
2. 4 个新增方法名必须真实出现在结果中：
   - `DeadUnavailablePersonIsRejectedByEveryManagementCommandWithoutWrite`
   - `WorldBusyRejectsManagementCommandsWithoutWrite`
   - `CompletedBuildingRejectsConstructionWorkerWithoutWrite`
   - `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution`
3. 保护面：34 项基线、字体 `3c758e16…`、场景 `isDirty=false`、存档无未完成 run；出现异常按原样保留证据并停下，不改档、不重试覆盖。
4. 若新增用例失败：保留失败断言原文与上下文，交主开发核验；**不得为通过而放宽断言、改产品规则或改公共 API**。

---

## 7. 边界与未声称

- 未声称新增 4 例已通过；本包只完成测试编写、安全刷新、编译发现与只读前置核对。
- 未声称第二 slot Gateway 路径已覆盖；它被精确标为未测，且未改配置 Authority。
- 未声称 `WorldBusy` 已覆盖“Advance 会话中 / Return 结算中”的私有锁；本用例只覆盖既有远征公共入口造成的真实 `CanSaveWorld=false`。
- 未对“claim 期间长期职责安排是否可改”作新产品裁决；本包只记录现有契约，后续须用真实测试核对，不能把测试结论直接当产品 Gate。
- 未改 34 项基线 / 14 项保护 / 字体 / 备份 / manifest / 证据日志 / 用户存档 / 场景 / Prefab；未 Git 写。
- 本文件的完成报告须由主开发专业核验，**不能代制作人批准**；后续由 Studio 自动继续。
