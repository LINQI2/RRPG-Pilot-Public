# 施工到场边界夹具修复报告（R3-B 同任务小包 · 2026-10-24）

范围：仅 Pilot `D:/AI_Agent_Lab/rrpg_pilot/repo`，分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`。
性质：**仅修 Play 测试时间线夹具**；不改生产 Work/Clock 核心、不改 Bridge 全局 API、不改 Save DTO Authority、不改玩法/活动规则、不放宽断言、不 skip、不反射伪造到场。
结论状态：**代码已编译通过，待 Studio 阶段宿主串行运行 PlayMode 复验；本报告不预填 PASS**。

## 1. 原始失败事实（沿用宿主只读回执，不是本次执行）

- 宿主记录 `3016ddd9-eaa5-45be-a2ab-cdc0d027fc89`：`ManagementShellPlayTests` 5 项，4 Passed / 1 Failed / 0 skip，4.1753459 秒。
- 唯一失败 `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`：
  `Expected: 495 But was: 494`，失败点是用例内 `ReadyMinute` 业务断言（前置的
  `Destination == "construction:"+a` 断言已通过），即真实登记目的地正确，只有到场分钟假设错误。
- 同次 `resourceScopeEnd=scope-restored|font|0|632`；旧失败回执保持 failed，不改写为 PASS。

## 2. 实际根因调用链（读生产源码 + 真实回执共同证明）

旧夹具（以及 `R3B-UI-20261023/construction-arrival-fix-report.md` 的假设）认为：
“`TryAddConstructionWorker` 不触发 `RefreshWorkIntent`，所以 480 工作边界才登记，`ReadyMinute = MorningStart(480) + 15 = 495`”。
真实调用链证伪了这一点：

1. `ManagementCommandGateway.AddConstructionWorker` → `VillageRuntimeBridge.TryAddConstructionWorker`
   （`Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs:49`）。
2. `TryAddConstructionWorker` → `TryAddProjectParticipant`
   （`VillageRuntimeBridge.Buildings.cs:65`；`VillageRuntimeBridge.cs:1118`）。
3. `TryAddProjectParticipant` → `AddProjectParticipantCore`，成功后**立即**
   `RefreshWorkIntent()`（`VillageRuntimeBridge.cs:1290`）。
4. `RefreshWorkIntent` 在通勤窗口内（`MinuteOfDay >= MorningStart - CommuteMinutes` 且 `< WorkEnd`）
   以**当前真实分钟**调用 `ArrivedWorkers(Time.CurrentInstant.Minute, true)`
   （`Assets/Scripts/Village/VillageRuntimeBridge.Work.cs:173-177`）。
5. `ArrivedWorkers` → `VillageIntradayWork.Arrived(id, destination, minute)`；目的地变化时
   `ReadyMinute = now + Schedule.CommuteMinutes`（`Assets/Scripts/Village/Runtime/VillageIntradayWork.cs:104-110`）。

本夹具在 479 分钟（`AdvanceTo(479)` 后 Clock 保持 Paused）通过真实管理按钮加入施工：
`479 + 15 = 494`。480 工作边界 `CompleteWorkBoundary` 只对“目的地未变”的登记执行
`Arrived(...,480)`，不会重新计时（`Arrived` 仅在 `a.Destination != destination` 时才重算），
所以 494 保持。真实回执中的 494 正是此登记，不是快照或读取时序问题。

**对历史假设的纠正（保留历史）**：`R3B-UI-20261023/construction-arrival-fix-report.md` 第 26-32 行
“加入施工不调用 RefreshWorkIntent、480 边界登记 → 495”的结论已被真实回执证伪；该报告保留为历史，
不修改成“当时已通过”。本次按“真实登记时刻 + 生产 CommuteMinutes”修正夹具，不把 495 硬改成 494。

## 3. 修正后的精确时间线（全部由生产值/真实 Clock 推导）

登记时刻来自运行中真实 `Clock.MinuteOfDay`（加入前后都捕获并断言暂停不推进），
`CommuteMinutes` 来自真实 `IntradayWork.Schedule`，不写死 15/480：

| 分钟 | 事件 | 断言依据 |
| --- | --- | --- |
| 479 | `TryAssignPerson(p1,"ws_farm_01")` → `RefreshWorkIntent` 登记 `ws_farm_01`，ready=494 | 前置 Primary Assignment 保留 |
| 479 | p1 真实按钮加入施工 → `AddProjectParticipantCore` → `RefreshWorkIntent` 登记 `construction:<a>`，ready=479+15=494 | `Destination` + `ReadyMinute == 真实登记分钟 + CommuteMinutes` |
| 479 | p2 真实按钮加入施工，同一真实分钟登记，ready=494 | 同上，并断言两人登记分钟相同 |
| 480/481 | 仍在通勤：`PersonPlace=null`、非“项目劳动”、`LaborRemainder=0`、`Progress=0` | 未到场 0 贡献（1 分钟通勤不被计入） |
| 494 | `HasArrived` 成立：真实到场 `construction:<a>`、“项目劳动”；当分钟 `LaborRemainder=0`、`Progress=0` | 到场当分钟不凭空产生劳动 |
| 495 | 区间 [494,495) 真实劳动 1 分钟 × 2 人 × 1 单位 = 2 人分钟 → `LaborRemainder>0`、`Progress=0` | 不足一个进度单位 |
| 796 | 上午 [494,720)=226 分钟 ×2=452，下午 [780,796)=16 分钟 ×2=32，合计 484 人分钟 → `Progress=1`、`LaborRemainder=4` | `prj_test_01` 为 1 人/日、target=10（480 人分钟/单位），项目仍 Active |

随后保留原强断言：真实按钮撤出 p1（参与者=1、`ws_farm_01` 占用=1、Primary Assignment 不变、
`WorkExecution != "项目劳动"`），再真实按钮重新加入（参与者=2、Primary Assignment 仍 `ws_farm_01`）。

## 4. 精确 diff（唯一改动文件）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

- Hunk 1（L1043-1048，新增只读生产时间读取，移到加入施工之前）：
  `Schedule` / `MorningStart` / `CommuteMinutes` 从真实 `IntradayWork.Schedule` 读取，
  断言 `CommuteMinutes > 0`。
- Hunk 2（L1074-1100，p1 加入段）：加入前后捕获真实 `Clock.MinuteOfDay` 并断言暂停不推进；
  新增只读 `WorkArrival(p1)` 取证：`Destination == "construction:"+a`、
  `ReadyMinute == p1RegistrationMinute + commuteMinutes`；并断言登记分钟落在生产通勤窗口
  `[MorningStart-CommuteMinutes, MorningStart)`（本夹具 479<480，故不能用 `MorningStart+通勤`）。
- Hunk 3（L1104-1121，p2 加入段）：同样捕获真实登记分钟、断言同分钟、登记目的地与
  `ReadyMinute == p2RegistrationMinute + commuteMinutes`。
- Hunk 4（L1128-1145）：删除“480 边界才登记 → `onsiteMinute = MorningStart + CommuteMinutes`”的旧假设，
  改为 `onsiteMinute = p1RegistrationMinute + commuteMinutes`（并与 p2 同边界交叉校验）；
  保留 480/481 通勤零贡献、494 到场、495 劳动、796 进度全部原强断言。
- 未新增/未删除任何测试用例；未改生产 `.cs`、活动规则、成长数据、基线 JSON、字体资产、旧备份。

哈希（本模型实际计算）：
- 修改后测试文件：`4bdf388941dddd7de8f9688772b8ddbc71490e2a8cccf7a7c89c33ba7ec79ee6`，68385 字节。
- 修改前测试文件 SHA（任务给定）：`1749fb86d223eb680e22b6ff236982d77ca4cdd2a30fe7fc79cbf94217c348cd`。
- `workspace-baseline.json`：`bc7216f125aa0046d116ad23d71e33bd15d9e31d861ce3c317e614429540394e`，6866 字节（只读记录，未改）。
- 受保护字体：14961033 字节，`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，
  与保护基线一致（只读复核，未写入/未重新导入）。

## 5. 保护与编译核验（本模型实际执行）

- 未使用 `rql-mutate`；刷新仅 `script-execute` 调用 `UnityEditor.AssetDatabase.Refresh()`，
  未调用 `SaveAssets` / `SaveChanges` / `SaveDirtyScenes`，未做全局保存。
- 刷新后 Editor 状态：`isPlaying=false isPaused=false isCompiling=false isUpdating=false`。
- Console `Error`/`Exception` 查询：0 条（一次读查询因编译域重载断线，未重发变更类请求，随后重新只读查询为 0 条）。
- 编译产物只读反射核验：`RRPG.ManagementShell.Tests.ManagementShellPlayTests` 位于
  `BattleStage1.PlayModeTests`；目标测试方法与新增 `WorkArrival` 只读 helper 均存在。
  编译时间核对：源码 mtime `2026-10-24T05:07:39.8892411Z`，`Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`
  mtime `2026-10-24T05:07:48.4669930Z`，汇编晚于源码，确认本次改动已实际编译。
- 测试自身仍在 `OneTimeSetUp/OneTimeTearDown` 逐项校验 34 项基线与受保护字体 SHA，并写
  `R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`；单槽存档仍由 `SaveSlotGuard`
  在每例前后按真实字节/SHA 捕获与 finally 恢复。
- 未启动/关闭 Unity，未安装组件，未改权限，未 commit/push/stage/切分支。

## 6. 宿主待验证与未测风险（不声称通过）

- 需由 Studio 阶段 check 用短类名 `ManagementShellPlayTests` 串行运行 PlayMode，持久化原始回执；
  通过后才继续原整包剩余 Save/Continue/NewGame、Simulation/Return gating、最小长期回归清理与
  Compiler/完整 Edit/Play 串行及日期化文档自审。
- 796 分钟 `Progress>0` 依赖生产规则“2 名真实到场工人 × 480 人分钟/单位”；本次已把两人登记时刻
  真实化，未放宽。若宿主回执显示某参与者被 420 活动机会 claim 而延迟到场，应按实际回执继续定位，
  不用延长多天或降低断言求绿。
- `Assert.Less(registrationMinute, MorningStart)` 是本夹具“479 加入”的合法性前置；若生产时间标定
  改变，应重新按真实登记时刻派生夹具，而不是放宽业务断言。
- 物理鼠标手感、UI 视觉体验仍不在本用例范围；旧 f6ae 回执与历史报告均保持原状。
