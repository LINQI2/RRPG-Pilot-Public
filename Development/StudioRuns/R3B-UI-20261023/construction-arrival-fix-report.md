# 施工到场失败根因修复报告（R3-B 同任务小包）

日期：2026-10-23
范围：仅 Pilot `D:/AI_Agent_Lab/rrpg_pilot/repo`，分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`。
性质：夹具/测试时间线修正 + 编译与保护自检；**不改生产业务代码、不改成长数据、不改活动规则、不放宽业务断言**。
结论状态：**待 Studio 阶段宿主串行运行 PlayMode 复验，不预填 PASS**。

## 1. 原始失败事实（沿用宿主只读回执，不是本次执行）

- 原宿主记录 `273ef5ef-8f38-4d92-a03e-279ba52d8bcc`：5 个用例 4 Passed / 1 Failed / 0 skip，唯一失败
  `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`：
  `Expected: "construction:building_instance_1" But was: null`（p1 的 `Snapshot.Person(p1).CurrentPlaceId`）。
- 该失败已推进到真实业务断言，不是字体/TypeLoad/Token/unknown 阻塞。
- 对照组 `BuildingConstructionProductPlayTests` 真实通过：同一施工到场逻辑在“更早加入施工”的时间线下 481 分钟可到场。

## 2. 根因：真实生产契约的时间线，不是断言写法

只读代码链路（未修改）：

- `VillageRuntimeBridge.Work.cs` `CompleteWorkBoundary`：仅在工作边界
  `MorningStart-CommuteMinutes(465)`、`MorningStart(480)`、`AfternoonStart(780)` 调用 `ArrivedWorkers(boundary,true)`。
- `VillageIntradayWork.Arrived(personId,destination,now)`：新目的地登记时
  `ReadyMinute = now + Schedule.CommuteMinutes`（默认 15）；`HasArrived` 只有 `now >= ReadyMinute` 才为真。
- `VillageRuntimeBridge.Reception.cs` `QueryIntradayPresence`：工作时间内未到场且目的地非空 → `PlaceId = null`、状态“通勤”。
- `VillageRuntimeBridge.Work.cs` `IsExecutingWork` / `GetWorkExecutionLabel`：未到场 → 非“项目劳动”。
- `VillageRuntimeBridge.Buildings.cs` `TryAddConstructionWorker`：加入施工**不**调用 `RefreshWorkIntent`，
  所以 479 分钟通过管理按钮加入时不会立即登记到场，只会在下一个工作边界 480 登记。

因此原用例的静态假设只在“465 通勤起点边界之前已加入施工”时成立：
465 登记 → ReadyMinute=480 → 481 分钟已到场（这正是 `BuildingConstructionProductPlayTests` 通过的原因）。
本用例先在 0 worker 状态推进到 479，再用真实管理按钮加入 p1/p2：480 边界才登记，
`ReadyMinute = 480 + 15 = 495`，481 分钟必然仍在通勤 → `CurrentPlaceId = null`。
这是夹具时间线断言落后于真实通勤规则，不是生产集成 bug，也不是 `PersonPlace` 读取过时快照
（`Call(_host,"RefreshNow")` 后仍读到“通勤”，与上面对 481 分钟的推演一致）。

## 3. 修正后的合法时间线（纯夹具/只读查询，不改规则）

1. 0 worker 推进到 479：施工状态 `UnderConstruction`、无活动项目、行内 Progress/WorkerCount 均为 0（原断言保留）。
2. 真实管理按钮加入 p1、p2；p1 原 Primary Assignment `ws_farm_01` 不被改写（原断言保留）。
3. 推进 480、481：读取真实 `IntradayWork` 到场登记，断言
   `Destination == "construction:"+a`、`ReadyMinute == MorningStart + Schedule.CommuteMinutes`（生产派生，不写死 15）、
   `ReadyMinute > 481`；同时断言 `PersonPlace == null`、非“项目劳动”、`LaborRemainder == 0`、`Progress == 0`
   —— 明确检查“通勤期间不推进、未到场 0 贡献”。
4. 推进到生产到场分钟 495：断言真实到场 `construction:<同一实例>`、`GetWorkExecutionLabel == "项目劳动"`，
   且当分钟 `LaborRemainder == 0`、`Progress == 0`（到场当分钟不得凭空产生进度）。
5. 推进到 496（到场后 1 个真实工作分钟）：`LaborRemainder > 0`、`Progress == 0`（真实劳动前置，未满进度单位）。
6. 同一工作日内推进到 796（生产边界：495–720 上午 + 780–796 下午，2 名真实到场工人 × 1 单位/480 人分钟）：
   `Progress == 1 > 0`、`LaborRemainder > 0`。不是反复多天自动完工。
7. 关闭/重开管理走真实按钮撤出 p1：Participant 只剩 p2、`ws_farm_01` 占用仍 1、
   Primary Assignment 不变、`WorkExecution(p1) != "项目劳动"`（真实劳动停止）。
8. 再用真实按钮重新加入 p1：参与者 2、Primary Assignment 仍 `ws_farm_01`（合法返还名额）。

新增帮助器 `WorkArrival(personId)` 只读 `VillageRuntimeBridge.IntradayWork.ToSaveData().Arrivals`，
用于把“通勤事实/到场分钟”作为可核验证据，不创建、不改写任何生产状态。

## 4. 精确 diff（唯一改动文件）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

- Hunk 1（约 L261-271，新增只读帮助器，插入在 `PersonPlace` 之后）：

```csharp
    /// <summary>读取当前真实工作到场登记（只读生产状态；不创建、不改写）。</summary>
    private static object WorkArrival(string personId)
    {
        object work = StaticField("VillageRuntimeBridge", "IntradayWork");
        Assert.IsNotNull(work, "VillageRuntimeBridge.IntradayWork 必须存在");
        object data = Call(work, "ToSaveData");
        Assert.IsNotNull(data, "IntradayWork.ToSaveData 必须返回真实存档投影");
        return ((IEnumerable)Get(data, "Arrivals")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "PersonId"), personId, StringComparison.Ordinal));
    }
```

- Hunk 2（约 L1086-1149，原位替换原“AdvanceTo(480)/AdvanceTo(481)/到场断言”段落）：

```csharp
        // 生产契约(VillageIntradayWork.Arrived / CompleteWorkBoundary)：
        // 第一个工作边界 MorningStart=480 才登记新施工目的地，
        // 登记时 ReadyMinute = 边界分钟 + Schedule.CommuteMinutes，因此 480/481 仍在通勤途中。
        object schedule = Get(StaticField("VillageRuntimeBridge", "IntradayWork"), "Schedule");
        Assert.IsNotNull(schedule, "IntradayWork.Schedule 必须存在");
        long morningStart = Convert.ToInt64(Get(schedule, "MorningStart"));
        long commuteMinutes = Convert.ToInt64(Get(schedule, "CommuteMinutes"));
        long onsiteMinute = morningStart + commuteMinutes;
        Assert.Greater(commuteMinutes, 0L, "真实通勤分钟数必须非零（480/481 不能直接到场）");

        AdvanceTo(480);
        AdvanceTo(481);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "推进后施工项目必须仍活动");
        object arrival = WorkArrival(p1);
        Assert.IsNotNull(arrival, "施工参与者必须已登记真实到场计划（通勤事实）");
        Assert.AreEqual("construction:" + a, (string)Get(arrival, "Destination"), "通勤目的地必须是同一施工实例");
        Assert.AreEqual(onsiteMinute, Convert.ToInt64(Get(arrival, "ReadyMinute")),
            "生产到场分钟必须等于工作边界 + 通勤分钟");
        Assert.Greater(onsiteMinute, 481L, "生产通勤边界必须晚于 481");
        Assert.IsNull(PersonPlace(p1), "通勤期间不得伪造到场");
        Assert.AreNotEqual("项目劳动", WorkExecution(p1), "通勤期间不得计为项目劳动");
        Assert.AreEqual(0, Convert.ToInt64(Get(project, "LaborRemainder")), "通勤期间不得产生劳动剩余");
        Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "通勤期间不得推进施工进度");
        ...
        AdvanceTo(onsiteMinute);        // 495
        ... 到场 + "项目劳动"，LaborRemainder/Progress 仍为 0
        AdvanceTo(onsiteMinute + 1);    // 496
        ... LaborRemainder > 0，Progress 仍为 0
        AdvanceTo(796);                 // 同日上午 + 下午真实劳动
        ... PersonPlace=construction:<a>、项目劳动、LaborRemainder > 0、Progress > 0
```

- Hunk 3（L1167）：撤出证据中写死的 `minute 481` 改为读取真实 `Clock.MinuteOfDay`，只改证据文本。

未改动：任何生产 `.cs`、活动规则、成长数据、`README`/规范、保护基线 JSON、字体资产、旧备份。
`ManagementView/Gateway/Shell` 无需改动：加入/撤出真实按钮链路已在前次执行中验证到项目与参与者状态，
唯一失败点只是时间线。

## 5. 编译与保护自检（本模型实际执行）

- 刷新：`script-execute` 仅调用 `UnityEditor.AssetDatabase.Refresh()`，未调用 `SaveAssets/SaveChanges/SaveDirtyScenes`。
- 编译：刷新后 `editor.isCompiling` 由 true → false；Console 中 `Error/Exception` 查询为 0 条。
- 编译产物核验（反射，不启动测试）：`BattleStage1.PlayModeTests`（该目录 asmdef 实际编译归属）中
  `RRPG.ManagementShell.Tests.ManagementShellPlayTests` 存在，目标测试方法与新增 `WorkArrival` 均存在
  （`compiled-method:True arrival-helper:True`）。
- 本次未调用 `rql-mutate`，未做全局保存，未启动/关闭 Unity，未触碰字体与备份。
- 刷新后字体只读复核（未写入）：14961033 字节，
  SHA256 `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，与保护基线一致。
- 34 项基线/字体：由用例自身 `OneTimeSetUp/OneTimeTearDown` 逐项 SHA256 校验并写
  `BASELINE-PRE/POST`、`FONT-PRE/POST`；本模型未改基线文件，也未把任何结果预填为通过。
  模型侧未找到可读取 SDK `globalMutationSaveBlocked` 状态的接口（MCP resources/tools 均不暴露），
  故仅能证明本任务写路径未触发全局保存；该 guard 激活状态仍应由 Studio 宿主回执核验。

## 6. 未测 / 风险（交由宿主真实 PlayMode 判定）

- 宿主 PlayMode 尚未为本修正运行，不能声称通过。
- 796 分钟处 `Progress>0` 依赖生产规则“2 名到场工人 × 480 人分钟/单位”，已在静态推演中给出
  （495–720 共 225 分钟 ×2 = 450，780–796 共 16 分钟 ×2 = 32，合计 482 → Progress 1、剩余 2）；
  若真实运行中活动 claim/午餐/其他边界改变了可用劳动分钟，应在宿主回执中按实际失败点继续定位，
  不通过延长多天或放宽断言求绿。
- 上午活动机会点 420 已在 0-worker 推进中经过；本修正未注入活动夹具，若真实运行出现 claim 阻断，
  将按“既有合法生命周期或既有测试注入”处理，不改活动规则。
- 物理鼠标手感、UI 视觉体验仍不在本用例范围。

## 7. 保护/恢复证据位置

- 34 项基线与字体 SHA256：测试运行会追加到
  `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`
  （`BASELINE-PRE/POST`、`FONT-PRE/POST`、`SAVE-CAPTURE/SAVE-RESTORE`）。
- 单槽存档：用例 `SaveSlotGuard` 每例前按真实 `SaveSystem.GetSavePath()` 捕获原始字节与 SHA256，
  TearDown 按原字节写回并断言哈希一致；不使用历史档覆盖用户档。
- 字体恢复原始字节与恢复历史保留在 `Development/StudioRuns/R3B-Font-Recovery-20261023/`
  与 `Assets/Editor/StudioRecoveryBackups`，本任务未覆盖、未删除。

## 8. 下一步（Studio 阶段，不属本报告结论）

宿主需以短类名 `ManagementShellPlayTests` 串行运行 PlayMode 专项并持久化原始回执；
通过后才继续 Continue/NewGame 保存、Simulation/Return gating、完整回归与日期化文档自审。
维护/部分通过不等于整 R3-B 通过。
