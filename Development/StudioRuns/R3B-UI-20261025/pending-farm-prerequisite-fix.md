# R3-B pending 专项 Farm A 前置失败定位与最小夹具修复

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 日期：**2026-10-25**（Asia/Shanghai；UTC+8）。
- 阶段：1/1，仅定位 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` 的 Farm A 前置并做最小测试夹具修复。
- 结论状态：**代码与编译已实核；单例 PlayMode 未由本执行层运行，交宿主只跑该完整 testMethod 串行判定。**
- 未做：未改产品代码 / 公共 API / DTO / WorkClock / Authority / 经济规则 / Scene / Prefab / 字体 / Packages / ProjectSettings / 基线 / 存档本体 / SaveSlotGuard；未 `rql-mutate`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 Shell / `Process.Start`；未 Git 写；未运行 `tests-run`；未忙等测试；未创建额外付费子 Agent。

---

## 1. 开工前只读实核（当前连接，非历史报告）

| 项 | 实测 | 证据 |
|---|---|---|
| 连接工程 | `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`，`2022.3.62t9` | `rql-query editor` |
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 同上（改前/改后各一次） |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`isLoaded=true`、`rootCount=5` | `rql-query scenes`（改前） |
| TestRunner | `testResults` 为 0 条；`tests` 源中本类 11 例可发现，目标方法全局唯一 1 条 PlayMode；Editor 非 Play / 非编译，无活跃测试运行或未知投递迹象 | `rql-query testResults` / `rql-query tests` |
| 34 项 workspace baseline | **34/34 same、0 diff、0 missing**（逐项 bytes + SHA256） | `script-execute` 只读哈希（改前） |
| 受保护 TMP 字体 | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**（= 基线） | 同上 |
| SaveSlotGuard 全部 run | **148**：`restored=147` + `protective-snapshot=1`；无其它 status；manifest 指向的 `original-save.bin` 逐项 SHA256 与 manifest 一致 | 只读遍历 `Development/StudioRuns/R3B-SaveProtection-20261024/**/manifest.json` + bin 哈希 |
| 最新 run | `20261024T150425Z`，`status=restored`，`backupBytes=13405`，`backupSha256=4b2b56f6…d6aa`，`readbackVerified=true` | 该 run manifest |
| 当前单槽 | **13405B / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**，与最新合法恢复证据一致 | `SaveSystem.GetSavePath()` 只读 + 文件 SHA256 |

> 无未完成 run，未自动恢复任何未知档；历史存档 / 备份未被改动。

---

## 2. 宿主失败事实与证据时间线

宿主回执（任务给定）：`2f8b6084`，共 11、10 过、1 败、0 skip；唯一失败：

```
PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice
前置：Farm A 必须在真实推进后完工
Expected: Completed
Actual:   UnderConstruction
```

失败时刻对应的真实证据日志（`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，UTC）：

- `2026-10-24T15:04:24.127Z [CASE-B-SCHEDULE] 统一事件 pending 门禁 … isolated-points=[]`：本用例开始，活动机会已按既有公开注入点局部隔离。
- `15:04:24.133Z [BUILD] definition=building_farm instance=building_instance_1`：Farm A 真实建造。
- `15:04:24.137Z [ADVANCE] targetMinute=14400 outcome=Completed`：真实 `Clock.AdvanceTo` 已完成（不是失败/未知）。
- `15:04:24.142Z [SCHEDULE-RESTORE] restored=true …`：推进后 5ms 即进入失败收尾，说明首败就是 Farm A 状态断言；没有后续施工 / 出场 / pending 证据。

**关键事实：推进本身 `Completed`；失败发生在读取建筑状态时。** 失败行改前为：

```csharp
AdvanceTo(10L * Day);
Assert.AreEqual("Completed", BuildingState(farmA), "Farm A 必须在真实推进后完工");
```

`BuildingState` 读的是 `_host.Snapshot`：

```csharp
private string BuildingState(string buildingId)
{
    object building = Call(Get(_host, "Snapshot"), "Building", buildingId);
    return Get(building, "LifecycleState").ToString();
}
```

而 `_host.Snapshot` 只在 `ProductHomeWorldHost.RefreshNow()`（`Update` 中按 `unscaledTime` 0.1 秒限流，或显式调用）时更新。`AdvanceTo` 是同一帧内同步调用，不经过 `Update`；本用例最后一次刷新是 `PlaceBuilding` 内（加入施工之前），因此读到的是 **Farm A 仍处 UnderConstruction 的旧投影**。

---

## 3. 根因（排除工人 / 到场 / 推进跨度 / pending）

只读核对产品链后，排除题面列出的其它可能：

- **不是未加入真实 worker**：`TryAddConstructionWorker` 返回 `Success` 才会走到推进；首败前无该断言失败。
- **不是未到场 / 推进跨度不足**：工作区间由 `CompleteWorkBoundary(465/480/780)` 登记到场，`VillageIntradayWork.Arrived` 在 465 登记 ReadyMinute=480；10 天 × 480 工作分钟 × 1 单位/人分钟 = 4800 单位 = `TestProgressTarget 10`，按真实区间恰好完成；`OBSERVE` 在 `CompleteWorkInterval` 内由 `Projects.OnWorkInterval` → `ObserveProjectCompletions` → `FinalizeConstruction` 把 `BuildingRuntime` 置为 `Completed`。
- **不是反射返回值用错**：`BuildingRuntime` / `Projects` 由产品自身结算；测试读的旧 `ProductHomeSnapshot` 才是错误来源。
- **不是已有 pending 阻断**：本用例在 Farm A 之前尚未建立任何 pending（`EventRuntimeBridge.Restore(null)` 后断言为空；首次临时承诺也尚未发生）。
- **对照证据**：同一基座在 `ExpeditionReturn_…` 用例中推进到 14400 后**先** `Call(_host, "RefreshNow")`，再断言 Farm A `Completed`；CASE-D 亦按刷新后读取。两者通过，说明差异只在读取同步。

因此确切根因是：**测试夹具缺少确定性推进后的产品只读投影刷新，导致断言读到推进前的旧 Snapshot，而不是真实建筑状态未完成。**

---

## 4. 精确修改（唯一代码改动）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（第 4124–4130 行）

```diff
         AdvanceTo(10L * Day);
+        // 读取同步：AdvanceTo 在本帧内同步完成（不经过 Update 的 0.1 秒周期刷新），
+        // 若直接读 _host.Snapshot 会得到加入施工后、推进前的旧投影（UnderConstruction），
+        // 而真实 BuildingRuntime 已在工作区间完成结算为 Completed。这里只刷新既有产品只读投影，
+        // 不改建筑 / 项目 / 劳动状态；与 ExpeditionReturn 用例的读取方式一致。
+        Call(_host, "RefreshNow");
         Assert.AreEqual("Completed", BuildingState(farmA), "Farm A 必须在真实推进后完工");
```

- 仅新增一次既有公开 `ProductHomeWorldHost.RefreshNow()` 调用与说明注释；不设置 `Completed` / `Progress` / 贡献，不改任何产品门禁或后续事件强断言。
- 保留其余 10 例、NewGame / Shell / schedule 恢复 / SaveSlotGuard 全部逻辑；未整理 9 份文档。
- 未修改：`ManagementView` / `Gateway` / `Shell` / `BuildingRuntime` / `VillageProjectSystem` / `WorkClock` / 经济规则 / `SaveSystem` / 基线 / 字体 / 存档。

修改后文件：**289629B / `75f8c00d49e838ad7bb03adc0ca6b78b3792c9e2e0c4714eb74458fdf4e204dd`**。

---

## 5. 编译保护与发现核验（本执行层实际执行）

- 编译入口：仅 `script-execute` 调用 `UnityEditor.AssetDatabase.Refresh()`；**未**调用 `SaveAssets` / `SaveChanges` / `SaveDirtyScenes`，未 `rql-mutate`。
- 刷新后 `editor.isCompiling` 由 true 回到 false；项目 Console 无新增 `error CS`（当前 Error/Exception 仅为本执行层两次只读 snippet 自身的编译/路径错误，未触及项目源码与测试程序集）。
- DLL 新鲜：`Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` mtime **`2026-10-25T07:27:43.0509733Z`**。
- 反射实核：类内 `UnityTest` 属性方法 **11** 个；目标方法 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` **存在（True）**。
- 发现唯一性：`tests | where fullName contains "PendingEvent_BlocksAdvanceAndReturn" | count` = **1**，模式 `PlayMode`。

---

## 6. 未测 / 交宿主

- 本执行层**没有运行** `tests-run`，也没有启动 / 忙等 TestRunner；不声称该用例通过或整类通过。
- 宿主请仅以完整方法名、单例、PlayMode 串行执行：
  `RRPG.ManagementShell.Tests.ManagementShellPlayTests.PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`
- 若单例在 Farm A 之后暴露新的真实失败，以宿主原始回执再定位；本阶段不预填后续结果，不削弱门禁断言。
- Farm A 之后的 40 天临时承诺、pending 建立、Direct / Return 门禁、保存零写与投影一致等断言仍全部未由本执行层验证。
- `ExpeditionReturn` 等其余 10 例未在本阶段运行；既有宿主回执保持历史事实，不并入本阶段结论。
