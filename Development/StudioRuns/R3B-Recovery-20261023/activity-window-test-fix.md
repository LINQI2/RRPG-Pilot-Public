# R3-B 修复包：专项测试的合法活动窗口前置

- 日期：2026-10-23
- 范围：仅 `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`
- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（未访问其他工程 / 认证目录 / 用户档）
- Unity：2022.3.62t9（用户已开 Pilot 工程 `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`，未启动 / 关闭）
- 未做：未 `commit/push/stage/切分支`、未建远程、未安装组件、未改权限、未改 Packages / 字体 / 插件 / 14 项保护、未改 Work / Clock / Bridge / Activity 规则 / 公共核心 API / Save DTO、未删失败断言 / 未跳过 / 未忽略 / 未反射伪造 claim、未自行调用 `tests-run`。

## 1. 失败事实（沿用主开发已回读的转录证据，不夸大）

`Development/StudioRuns/R3B-Recovery-20261023/labor-gate-last-result.json`（由 MCP `rql-query` 的 `source:testResults` 逐行转录，非磁盘原始 XML/JSON）记录：13 项中 12 Passed / 1 Failed。
唯一失败 `WorkplaceLaborOnlyCountsActualWorkplaceExecution`：

- 期望 `"Success"`，实得 `"InvalidWindow"`；
- 抛出点 `CommitRestActivity` 第 416 行，调用点测试体第 398 行；
- `ConstructionLaborDoesNotCountTowardPrimaryWorkplace` 及其余 11 项 Passed。

本文件与 `ManagementProjectionTests.json`、`labor-projection-review.md` 均原样保留，未覆盖。

## 2. 根因（真实契约定位）

`ActivityRuntimeBridge.OpenDailyWindow(window)` → `ActivityLifecycleService.OpenDailyWindow`：

```csharp
if (window == null || !window.DayStamp.IsValid) return ActivityResult.InvalidWindow;   // 第 12 行
if (window.IsIntraday) return ActivityResult.InvalidWindow;                            // 第 13 行
```

`ActivityDailyWindow.IsIntraday => OpportunityId != null`（`ActivityDailyWindow.cs:11`）。

测试 helper 原来用 **5 参**构造窗口：

```csharp
Activator.CreateInstance(ActivityWindow, day, actors, participants, id, Clock.CurrentInstant.Minute);
```

第 5 参 `opportunityId` 非空 → `IsIntraday == true` → 服务在入口直接返回 `InvalidWindow`，窗口从未建立，后续 `TryCommitActivity` 也就无从成功，第 416 行的 `Assert.AreEqual("Success", ...)` 必然失败。这与转录中“第 416 行 / InvalidWindow”完全一致。

Rest 在本测试中是**非排程的普通活动**（`TryCommitActivity` 路径，`opportunity == null`），不需要日内 Opportunity 身份；日内机会只在 `OpenOpportunity` + `TryCommitScheduled*` 路径使用（见 `ActivityLifecycleService.cs:20-33,36-44`）。

## 3. 合法契约依据（既有成功测试同构）

- `UnityProject/.../Tests/EditMode/Village/HomeProductWhiteboxTests.cs:40-43`：用 **3 参** `ActivityDailyWindow(day, actors, participants)` → `OpenDailyWindow` 得 `Success` → `TryCommitActivity` 得 `Success`。
- `ActivityLifecycleTests.cs:19-22`、`ActivityIntegrationTestKit.cs:43-48`、`ActivityPersistenceTests.cs:12-14`、`SelectionIntegrationTestKit.cs:49-53`：均以 3 参日窗口 + `TryCommitActivity` 建立真实 claim。
- `VillageRuntimeBridge.Work.cs:24-27`：`IsExecutingWork` 明确要求 `ActivityRuntimeBridge.QueryClaim(personId) == null`，因此授予真实 claim 后 `ManagementReadAdapter.Labor` 必然不再把该人物计为岗位劳动——即被测断言（活动期间 `LaborCount == 0`）具有判别力，而不是“无活动误当成功”。

## 4. 精确改动（唯一文件，纯测试 helper）

`ManagementProjectionTests.cs`（改后 533 行，`bytes=28226`，`sha256=c25458246613323f06e8c653b8c26028ff67e57617d188c0e88d073541aed883`）：

- `CommitRestActivity` 的窗口构造由 5 参改为 3 参：

```csharp
// 旧
object window = Activator.CreateInstance(ActivityWindow, day, new[] { personId }, new[] { personId },
    id, Clock.CurrentInstant.Minute);
// 新
object window = Activator.CreateInstance(ActivityWindow, day, new[] { personId }, new[] { personId });
```

- 同步在该 helper 的 XML 注释中写明上述契约约束（`OpenDailyWindow` 拒绝 intraday 窗口；Rest 为普通活动，使用真实日窗口）。
- 测试体与断言**零改动**：通勤 `LaborCount==0`、真实岗位劳动 `LaborCount==1` 且证据 > 0、真实到岗后继续计数与证据增长、活动期间 `LaborCount==0` 且 `QueryClaim` 非空，全部保留；未新增跳过 / 断言放宽 / 反射伪造。
- 其余任何文件（投影修正、Work / Clock / Bridge / Activity 规则、公共核心 API、Save DTO）未触碰。

## 5. 执行证据

1. **Asset Refresh**：`editor | call RefreshAssets()` → 返回 `RqlEditorApi@1|RefreshAssets()|null`。
2. **编译结束回读**：`editor | select isCompiling isUpdating unityVersion projectPath` → `isCompiling=false, isUpdating=false, 2022.3.62t9, ...\UnityProject\RRPG1.0game`。
3. **编译新鲜度（受控反射读取，只读）**：source mtime `2026-10-23 13:46:14.673`；测试程序集 `VillageStage1.Tests` 写入时间 `2026-10-23 13:46:31.043` → `fresh=True`（程序集晚于源码，说明改动已被编译）。
4. **Console 回读**：`console | where logType == "Error" or logType == "Exception" | sort sequence desc | first 20` → **0 条**（无编译错误 / 异常）。
5. **活动契约旁证（隔离域对象，不触碰产品运行时）**：对独立 `ActivityWorldState` + `ActivityLifecycleService` 实例做一次契约验证 →
   `3argWindow.open=Success 5argWindow.open=InvalidWindow commit=Success claim=management:rest:person_alpha`。
   即：3 参日窗口可开、原 5 参窗口确实返回 `InvalidWindow`、修正后 `TryCommitActivity` 能真实授予 claim。
6. **34 项受控 SHA256 对比**（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，逐项重算）：
   `entries=34 same=34 diff=0 missing=0`。基线包含 14 项保护相关文件（字体 / Packages / 插件 / ScriptableObjects / ProjectSettings 等）与工具链产物，全部无差异；`ManagementProjectionTests.cs` 不在该 34 项基线内，本次唯一改动不触及受保护集合。未恢复 HEAD 旧备份。
7. **保留证据**：`labor-gate-last-result.json`、`ManagementProjectionTests.json`、`labor-projection-review.md` 均未修改或覆盖。

## 6. 未测 / 待宿主核验（不预填 PASS）

- 本阶段**未**自行调用 `tests-run`（按授权要求）。该测试及整类 `ManagementProjectionTests` 的最终通过状态，须由宿主阶段以短类名 `ManagementProjectionTests` 串行复验，较长测试亦由宿主执行。
- 未做全量回归、未加 UI、未验证其他测试类；上一轮 12 项 Passed 的结论仍来自既存转录证据，本次未重复运行。
- 若宿主复验出现新的失败，须连同 `Console` 与失败栈原文回读后再判断，不能以本文件的推理替代实跑结论。
