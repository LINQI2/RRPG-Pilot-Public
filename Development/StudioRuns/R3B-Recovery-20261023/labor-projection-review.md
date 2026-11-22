# R3-B Management 实际劳动人数口径核对（2026-10-23）

## 范围与身份

- 工作目录：`D:/AI_Agent_Lab/rrpg_pilot/repo`
- 分支：`ai/rrpg-r3b-population-pilot`
- HEAD：`054b00d3e38fe2e2e142bfdb2502de90f7991030`
- 本工作包只核对并修正 Management 岗位“实际劳动人数”口径，补最小专项；未做 UI、Play、全量测试或最终 R3-B 验收。
- 未新建任务，未 commit / push / stage / 切分支，未改正式工程、权限、ACL、Packages、字体或计费渠道；未启动或关闭 Unity，仅连接手动已开的 Pilot 2022.3.62t9。

## 缺陷是否确认

**确认存在真实缺陷。**

`ManagementReadAdapter.Labor(bound)` 修正前只判断：

```csharp
if (VillageRuntimeBridge.IsExecutingWork(id)) result.Add(id);
```

`IsExecutingWork` 的真实目标是 `WorkDestination`：若人物有 Active Project，目标为项目工地；否则为 Primary Assignment 的 Workplace。因此人物保留原岗位 Primary Assignment、实际投入另一施工项目时，`IsExecutingWork` 为 true，旧实现会把这个人物同时计入原岗位 `LaborCount`。长期绑定数（`BoundCount`）本身仍应保留，但不能替代“实际在该 Workplace 劳动”。

## 真实查询依据与最小变化

沿用现有只读权威投影，不新增第二套状态、不改 Work / Clock / Bridge 全局契约、不改 Save DTO 或 Authority：

- `VillageRuntimeBridge.IsExecutingWork(id)`：只确认当前确实在执行工作（上班时段、在村、无活动 claim、已到真实目的地）。
- `VillageRuntimeBridge.GetPersonPresence(id)` → `VillageWorldPresence.Resolve`：现有 Presence 派生，优先级 `Away → ProjectSite → Workplace → Free`；`Presence.State == Workplace` 且 `PlaceId == 当前 WorkplaceId` 才说明真实目标就是该岗位。施工参与时是 `ProjectSite`，不会回流原岗位。

`ManagementReadAdapter.cs` 的最小变化：

- `Labor(bound)` 改为 `Labor(bound, workplaceId)`，同时校验 `IsExecutingWork` 与 `GetPersonPresence` 的真实目标匹配。
- 建筑功能岗位与静态兼容岗位两个调用点均传入各自的真实 `WorkplaceId`。
- `ManagementSnapshot.cs` 只补 `LaborPersonIds` 注释，说明按真实目标匹配，绑定数不替代；数据模型结构未变。

未改：

- `VillageRuntimeBridge.Work.cs`（修正前后 SHA256 均为 `79b306a1afd325049f60da7ce1095b1a82cf8ea51ce548f5a4ce556400905690`）
- `VillageWorldPresence.cs`（修正前后 SHA256 均为 `98ddfd169acae8659e431d5f43b84612c723882f626fe8e4424e57eededb2720`）
- Assignment / Project / Resource / Save 契约与 DTO。

## 改动文件与落盘哈希（SHA256）

| 文件 | 修正前 | 修正后 |
| --- | --- | --- |
| `Assets/Scripts/Village/ProductHome/ManagementReadAdapter.cs` | `73a6990a47337a7c5226b69f4ce6ecfae8dc91d573c14ac2bffcec2dcb408506` | `752c6a2d8ebb0579ad30792fab93f4e8b70b56f2d5d933ae0f23cf0457f6fea7` |
| `Assets/Scripts/Village/Runtime/ManagementSnapshot.cs` | `fca9156092333fb1ada354482cc44b6900dd5ab77332436b43d7265435c34850` | `1184f80307e8ef07563193f27433a6983077b7d79a312b1df57b53ae5ade1ce6` |
| `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs` | `e5857e3fd7b01c5a4bb12a19070efbd46309c029ba4eb6c2c2a20c0df2dfe98f` | `98e239ea99a4a18d10d3f6db58772a1d160e16a85c981178e5b4198693fea07f` |

`ManagementReadAdapter.cs` / `ManagementSnapshot.cs` / `ManagementProjectionTests.cs` 均为任务授权文件；.cs 已存在，无需新增 Editor meta。

## 测试专项（已写入，未由本执行者运行）

在现有 `ManagementProjectionTests` 增加 2 个最小专项，未改冻结测试、未删既有 11 项：

1. `ConstructionLaborDoesNotCountTowardPrimaryWorkplace`
   - 先让 `person_alpha` 在岗位真实劳动到 481 分，断言 `LaborCount == 1` 且岗位证据分钟数 > 0。
   - 再加入另一施工项目，推进到 500 分；断言 `GetWorkExecutionLabel == "项目劳动"`、原岗位 `BoundCount == 1` 但 `LaborCount == 0`、施工行 `WorkerCount == 1`、项目 `LaborRemainder` 独立增加、原岗位证据不增加。
   - 该场景在旧实现下会把施工人物计入原岗位劳动，具备回归判别力。

2. `WorkplaceLaborOnlyCountsActualWorkplaceExecution`
   - 上班前（470 分）`LaborCount == 0`。
   - 正常岗位劳动（481 分）`LaborCount == 1` 且真实证据 > 0，作为非零前置。
   - 通过真实 `VillageIntradayWork.Leave/Arrived` 制造通勤未到岗，标签为“通勤”，`LaborCount == 0`。
   - 真实到岗后恢复“岗位劳动”并继续产生证据。
   - 通过真实 `ActivityRuntimeBridge.OpenDailyWindow/TryCommitActivity` 建立活动 claim（未伪造字符串），活动期间 `LaborCount == 0`。

测试使用现有隔离 `Advance` 基座和运行时 Work / Activity 真实状态，未读写真实用户存档，未 mock 字符串或镜像实现。

## 保护核验（受控 SHA256）

- 以 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 为唯一现有基线：登记 34 项（26 项 ` M` + 8 项 `??`），修正前后两次逐文件 SHA256 比对均为 **34/34 一致，0 差异**。
- 未执行 HEAD / 旧备份恢复，未写回任何受保护文件。
- 现有真实恢复测试回执 `Development/StudioRuns/R3B-Recovery-20261023/ManagementProjectionTests.json` 未被覆盖或改写，仍为 11 总 / 11 通过 / 0 失败跳过。
- 既有 `RemovingAssignmentDuringLaborStopsOldWorkplaceEvidence` 结论范围保持不变：只证明撤职后旧岗位无新增证据、下一边界清意图，不夸大为即时意图清除或 Play 通过。

## 编译与 Console 回读

- 已执行 `AssetDatabase.Refresh(ForceUpdate)`，等待 `EditorApplication.isCompiling == false`。
- Console 回读：无 `ManagementReadAdapter` / `ManagementSnapshot` / `ManagementProjectionTests` 编译错误；无新增项目 `error CS`。反射确认 `ManagementReadAdapter` 与新增两个测试方法已编译进对应程序集。
- 实际失败保留：本执行者第一次用 `Convert.ToHexString` / `SHA256.HashData` 写 SHA 校验片段时，因 Unity 2022.3 运行时 API 较旧而编译失败（`CS0117`，仅影响该临时 snippet）；改用 `SHA256.Create()` + `BitConverter` 后完成核验，未影响产品代码。

## 未测与后续门禁

- **本执行者未运行 `tests-run`**；专项结果留给宿主串行门禁。建议宿主使用 `ManagementProjectionTests` 短类名过滤，不要多重转义 RQL。
- 未做最终产品集成、PlayMode、全量测试、UI 接入验证；因此本文件不构成 R3-B 验收或关闭。
- UI 接入注意：
  - `LaborCount` 只表示真实目标为当前 Workplace 的执行人数；`BoundCount` 是长期 Assignment 名额占用，二者不可互相替代。
  - 施工工人行仍按项目参与者事实展示；项目参与者在施工时不应出现在原岗位 `LaborCount`。
  - 资源页 Farm / Kitchen 行复用同一 `LaborCount` 口径，会随本次修正自动排除施工回流。
  - 活动、通勤、下班/休息、远征/不可用均不计入岗位 `LaborCount`；这些状态仍通过人物行 `CurrentExecution` / `TemporaryCommitment` 等现有字段展示。
- 若宿主测试出现失败，应按真实失败保留并回报；本文件未预填 PASS。
