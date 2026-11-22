# 累计劳动专项：活动时间异常（Invalid activity timing）定位与修复

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一授权目录）
- Unity 工程：`UnityProject\RRPG1.0game`
- 阶段：**1/1 —— 定位累计劳动专项活动时间异常**。本文件不越过阶段验证门禁；通过回执必须由宿主整类 EditMode 产生后才可登记。
- 本包实际写入：仅 `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs` 与本文档。
- 本包未做：未运行任何测试（未 `tests-run`、未反射启动 TestRunner、未忙等测试结果）、未 `rql-mutate`、未 Shell/`Process.Start`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未改源码/场景/Prefab/资产/配置、未读写用户存档与存档备份/历史档、未改 34 项基线或 14 项保护、未 Git 写。安全刷新只调用 `AssetDatabase.Refresh`。

---

## 0. 前置核对（E1，本轮只读实核）

| 项 | 实际值 | 来源 |
|---|---|---|
| Pilot 身份 | `.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`；`refs/heads/ai/rrpg-r3b-population-pilot` = `054b00d3e38fe2e2e142bfdb2502de90f7991030` | `read` 两个文件（未执行任何 git 命令） |
| Editor 空闲 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | `rql-query editor`（改前 + 改后各一次） |
| 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | `rql-query scenes` |
| 34 项 SHA 基线 | `Development/StudioRuns/R3B-20261022/workspace-baseline.json` → **total=34 ok=34 problems=0 all-match**（改前读一次、改后复核一次） | 只读 C# 片段按 JSON 内 `bytes+sha256` 逐项重算 |
| 受保护 TMP 字体 | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，与基线一致（改前/改后都不变） | 同上（该字体是 34 项之一） |
| 未完成 save run | `Development/StudioRuns/R3B-SaveProtection-20261024/` 共 16 个 run：15 个 `status="restored"`、1 个 `20261024T052704Z` 为 `protective-snapshot`；**无 captured/restore-failed/no-manifest** | 只读 `grep "status"` 全部 manifest（不读、不写、不移动存档与备份） |

结论：身份、空闲、场景、34 项基线、字体、存档保护 run 全部满足前置，才继续本阶段写操作。

---

## 1. 原始失败回执（E1，当前 Editor `testResults`）

`testResults` 当前保存的正是任务所述 `0447ba69` 那轮：**17 条、16 Passed、1 Failed、0 Skipped**。

| 方法 | 状态 | 耗时 |
|---|---|---|
| `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution` | **Failed** | **4 ms** |
| 其余 16 条（含 `WorkplaceLaborOnlyCountsActualWorkplaceExecution`、`ConstructionLaborDoesNotCountTowardPrimaryWorkplace` 等同夹具写入用例） | Passed | 1–79 ms |

失败 message：

```
System.Reflection.TargetInvocationException : Exception has been thrown by the target of an invocation.
  ----> System.ArgumentException : Invalid activity timing
```

失败 stackTrace（原文，`testResults.stackTrace`）：

```
  at System.Reflection.RuntimeConstructorInfo.InternalInvoke (System.Object obj, System.Object[] parameters, System.Boolean wrapExceptions) [0x0001a] ...
  at System.Reflection.RuntimeConstructorInfo.DoInvoke (...)
  at System.Reflection.RuntimeConstructorInfo.Invoke (System.Object obj, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] args, System.Globalization.CultureInfo culture) ...
  at System.RuntimeType.CreateInstanceImpl (...)
  at System.Activator.CreateInstance (System.Type type, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Object[] args, ...)
  at System.Activator.CreateInstance (System.Type type, System.Object[] args) ...
  at RRPG.Management.Tests.ManagementProjectionTests.RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution () [0x00000] in D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game\Assets\Tests\EditMode\Village\ManagementProjectionTests.cs:560
  at (wrapper managed-to-native) System.Reflection.RuntimeMethodInfo.InternalInvoke(...)
  at System.Reflection.RuntimeMethodInfo.Invoke(...)
--ArgumentException
  at IntradayActivitySchedule..ctor (System.Collections.Generic.IEnumerable`1[T] opportunities, System.Int32 commuteMinutes, System.Int32 repeatDelay, System.Collections.Generic.IEnumerable`1[T] activityDurations) [0x0018a] in ...\Assets\Scripts\ActivityHost\IntradayActivitySchedule.cs:28
  at (wrapper managed-to-native) System.Reflection.RuntimeConstructorInfo.InternalInvoke(...)
```

（`console` 侧 `Error/Exception` 查询当前为 0 条，与宿主回执一致：失败异常被 NUnit 收在测试结果里，不是编辑器日志错误。）

---

## 2. 定位链（只读源码，逐段核对）

1. 失败行 `ManagementProjectionTests.cs:560`（旧编号）= 用例内唯一一次 `ConfigureActivitySchedule(...)`，参数为 `new[] { 4000 }, 15, 30, null`。
2. `Activator.CreateInstance(ActivitySchedule, ...)` 直接构造 `IntradayActivitySchedule`；该类型定义在 `Assets/Scripts/ActivityHost/IntradayActivitySchedule.cs`。
3. 构造器（`IntradayActivitySchedule.cs` 第 15–16 行）：

   ```csharp
   var points=(opportunities ?? new[]{420,1020,1200}).Distinct().OrderBy(x=>x).ToArray();
   if(points.Any(x=>x<0 || x>=1440) || commuteMinutes<1 || repeatDelay<1) throw new ArgumentException("Invalid activity timing");
   ```

   即：机会点必须是**日内分钟 0..1439**；`4000` 越界 → 构造器按既有契约抛 `ArgumentException("Invalid activity timing")`，反射调用包成 `TargetInvocationException`。
4. `commuteMinutes=15 / repeatDelay=30` 合法；活动时长参数 `null` 走默认值，与异常无关；`4000` 是唯一触发点。
5. 同一文件内同类用例的门控写法：
   - 用例 13 `WorkplaceLaborOnlyCountsActualWorkplaceExecution`（第 382–383 行）用 `{ 1020, 1200 }`（**合法**日内分钟，且都在推进区间 0→497 之外），该轮 Passed；
   - 其他既有 Edit/Play 用例（`IntradaySelectionTests`、`IntradayActivityProductPlayTests`、`IntradayReceptionIntegrationTests`、`IntradayOnsiteProductPlayTests`）统一用 `new int[0]`（空机会点）作为公开注入配置。
6. 用例 17 的推进区间是 `Advance(2881)` → `Advance(3361)`，跨两个午夜；任何 0..1439 的机会点都会落在推进区间内（第 0/1 天全覆盖），因此「把机会点排到推进区间之外」的旧注释在**合法分钟域内不可能成立**——这正是夹具参数写法不自洽的地方，而不是生产调度问题。

**根因（确定，非推断）**：新增用例把非法机会分钟 `4000` 传给 `IntradayActivitySchedule` 构造器，触发既有输入校验。属**夹具输入非法**，不是生产核心契约缺陷：`IntradayActivitySchedule`、`ActivityRuntimeBridge`、`VillageRuntimeBridge.Activities`、`Work`/Clock 核心均无改动必要，也未改动。

---

## 3. 精确修复（仅测试文件 1 处）

`ManagementProjectionTests.cs` 用例 17 开头：

```diff
-        // 把机会点排到推进区间之外，避免真实活动 claim 干扰“累计工作统计”前置；
-        // 这是既有公开注入点，TearDown 恢复原日程，不改产品规则。
+        // 夹具隔离：本用例不需要任何真实活动 claim，只需把机会点清空（既有公开注入点 + TearDown 恢复）。
+        // 机会点必须是合法日内分钟（0..1439）：旧写法 { 4000 } 会被 IntradayActivitySchedule 构造器
+        // 按既有契约判为 ArgumentException("Invalid activity timing")——那是非法测试输入，不是生产缺陷。
+        // 空机会点配置已被其他 Edit/Play 用例使用，不改变任何领域规则，断言保持原样。
         Call("ConfigureActivitySchedule",
-            Activator.CreateInstance(ActivitySchedule, new object[] { new[] { 4000 }, 15, 30, null }));
+            Activator.CreateInstance(ActivitySchedule, new object[] { new int[0], 15, 30, null }));
```

边界说明：

- 采用**既有合法公共 schedule 配置**（空机会点，`ConfigureActivitySchedule` 注入，`TearDown` 里 `_scheduleBeforeTest` 恢复），不新增测试钩子、不改产品规则。
- 未删除、未放宽任何断言：`WorkedMinutes > 480`、`TotalExecutedDays >= 1`、`TotalTestContribution > 0`（撤职前后）、`GetRecord==null`、`GetWorkplaceId==null`、`GetOccupantCount==0`、`ToSaveData().TotalTestContribution` 不变、`evidenceAfterRemoval` 不变、`LaborCount==0` 全部原样保留。
- 未吞异常、未改生产经济/Clock/Work 核心/API/DTO/Authority、未删除活动相关验证。
- 逐项静态核对（供宿主复核）：`WorkplaceLabor…` 的 `CommitRestActivity` 仍用真实 Activity Runtime 建立 claim，未受本改动影响；用例 17 只需「无 claim 干扰」的确定性推进，空机会点满足；`ResetForTests` 会把 `ActivitySchedule` 复位为默认值，`Setup` 捕获、`TearDown` 恢复，跨用例无残留。
- 累计口径静态核对：`VillageAssignmentSystem.OnDayTick` 对「有 Primary Assignment + 岗位有效 + 未被占用」每人每次 `WorkedMinutes += 480`、`TotalTestContribution += def.TestContributionPerDay`（`ws_farm_01` = 1），`Advance(2881)` 跨两个午夜 ⇒ `WorkedMinutes ≥ 960 > 480`、`TotalTestContribution > 0`；撤职后记录删除，`Advance(3361)` 的 Day Tick 不再产生该人贡献，`Labor()` 只遍历当前绑定（已为空）⇒ `LaborCount=0`。该推导只说明断言与既有实现自洽，**不替代宿主运行回执**。

---

## 4. 编译与测试发现回执（E1，本轮实核）

| 项 | 实际值 |
|---|---|
| 刷新方式 | `script-execute` 内 `AssetDatabase.Refresh()`（唯一安全刷新；未用 `rql-mutate`、未全局保存） |
| 刷新副作用 | 触发域重载 / MCP 连接瞬时断开一次，重连后确认完成；刷新前场景 `isDirty=false`，无 dirty 资产被自动保存 |
| 编译结果 | `isCompiling=false / isUpdating=false`，`console` 中 `Error` = **0 条**（无编译错误） |
| 测试发现 | `tests | where fullName contains "ManagementProjectionTests"` → **17 条**，全部 `EditMode`，assembly `VillageStage1.Tests` |
| 是否运行测试 | **未运行**（按要求交宿主整类串行 check；未 `tests-run`、未忙等） |

---

## 5. 保护回执

| 项 | 改前 | 改后 |
|---|---|---|
| 34 项基线 | 34/34 all-match | 34/34 all-match（problems=0） |
| 受保护 TMP 字体 SHA256 | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 同值，unchanged |
| 场景 | `SampleScene` `isDirty=false` | 同（未改场景/资产/Prefab） |
| 存档 / 存档备份 / 历史档 | 未读、未写、未移动、未恢复 | 同 |
| 未完成 save run | 16 run 全为 restored / protective-snapshot | 同（本包未产生新 run） |
| 改动文件面 | — | 仅 `ManagementProjectionTests.cs` + 本文件 |

---

## 6. 未测 / 未证（必须由宿主补）

1. **修复后 17 例的真实运行结果**：本包未运行测试，`16 Passed / 1 Failed` 是修复**前**那一轮的回执。修复后的通过数**尚无回执**，不得写成已通过。
2. 用例 17 修复后除「构造器不抛异常」之外的逐条断言（`WorkedMinutes > 480`、贡献非零与不回滚、后续零增长、`LaborCount=0`）**未在真实执行中观测**，仅有第 3 节的源码级推导。
3. 本轮未跑完整 EditMode / PlayMode 全量回归；未跑 Compiler 全量；Play 6/6 属既有历史回执，不是本轮结果。
4. 存档保护 run 的字节级 Capture/Restore 属 Play 侧 `SaveSlotGuard`，本包没有触发也没有替代验证。

---

## 7. 矩阵状态补记（对 `requirement-evidence-gap-matrix.md` 的最小增量，不改该文件）

| 项 | 补记 |
|---|---|
| B-05（撤职删除记录与累计统计） | 新增用例已从「构造器即失败（非法机会分钟 4000）」修为合法公共 schedule 配置；**仍为待宿主整类 check**，不得登记通过。 |
| I-02（整类串行 check） | 修复后 `ManagementProjectionTests` 仍为 **17 例**（发现级 E1）；通过数待宿主。 |
| 证据口径 | 04…回执的 `16 过 / 1 败 / 0 跳` 保留为**失败历史**；根因定性为**夹具非法输入**，不登记为生产缺陷，也不据此改任何 Authority。 |

---

## 8. 交接

请宿主以短类名 `ManagementProjectionTests` 整类 **EditMode** 串行 check（17 例），并以新回执替换 §1 的失败口径。本文件不构成通过声明，也不代制作人批准。
