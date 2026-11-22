# R3-B 小修复包：ManagementProjectionTests 活动窗口夹具 WindowConflict

- 日期：2026-10-23
- 授权范围：仅 `D:\AI_Agent_Lab\rrpg_pilot\repo`，分支 `ai/rrpg-r3b-population-pilot`（HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`）
- 唯一改动文件：`UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`
- Unity：Pilot 2022.3.62t9（用户手动已开，本阶段未启动 / 关闭进程）
- 未做：未 `commit/push/stage/切分支/改远程`；未 `Process.Start`；未改 ACL/标签；未安装组件；未改 Packages/字体/插件；未改 Work/Clock/Bridge/Activity/SaveAuthority/投影/网关模型；未删/跳过/忽略/放宽任何断言；未反射改写私有状态伪造 claim；**未自行调用 tests-run**。
- 旧证据原样保留：`activity-window-test-fix.md`、`labor-gate-last-result.json`、`ManagementProjectionTests.json`（本文件为新增，未覆盖任何旧文件）。

## 1. 本轮失败事实（沿用主开发已回读的宿主记录，不夸大）

`studio-test-record/10a41539-e129-41f1-aff8-052e75e034fb.json` 与 `b2888ea0-d6ac-4bd4-817b-8d17e767bfdd.json`：两次均 13 总 / 12 通过 / 1 失败 / 0 跳过。
唯一失败 `ManagementProjectionTests.WorkplaceLaborOnlyCountsActualWorkplaceExecution`：`Expected "Success"`，实际 `WindowConflict`（不再是上一轮的 `InvalidWindow`，说明 3 参窗口修正已生效，失败点后移到服务内的同日窗口判定）。

## 2. 真实根因（领域契约定位，非缺陷）

1. 生产默认日程机会点为 `{420, 1020, 1200}`：`Assets/Scripts/ActivityHost/IntradayActivitySchedule.cs:15`。
2. 推进期间的机会点由 AdvanceSink 开窗：`VillageRuntimeBridge.Intraday.cs:169` → `OpenIntradayOpportunity(boundary)`（`VillageRuntimeBridge.Activities.cs:104-126`），它是**当日窗口的唯一建立者**。
3. 本用例先 `Advance(470)`：`0 → 470` 跨过 420，于是 `OpenOpportunity` 成功建立**当日的 intraday 窗口**（`OpportunityId="village.intraday|420|free"`）。
4. 测试末尾 `CommitRestActivity` 再用 3 参日窗口调 `OpenDailyWindow`：`ActivityLifecycleService.cs:15-16`

```csharp
if (world.Window != null && world.Window.DayStamp == window.DayStamp)
    return world.Window.SameSnapshot(window) ? ActivityResult.Duplicate : ActivityResult.WindowConflict;
```

DayStamp 相同、快照不同（`OpportunityId` 不同）→ 返回 `WindowConflict`。

5. 排除另一条 `WindowConflict` 分支：`ActivityLifecycleService.cs:14` 要求存在 `IsScheduled` 活动实例；本夹具从不调用 `TryCommitScheduled*`，自始至终没有 scheduled 实例，因此命中点必然是第 16 行。
6. 契约正确性旁证（**隔离域对象**，不触碰产品运行时、不建立/不推进任何测试）：一次只读契约验证得到
   `freshDaily=Success opportunity=Success sameDayDaily=WindowConflict`
   即：无窗口时开日窗口成功；先开当日机会窗口后，再开同日不同快照日窗口按契约返回 `WindowConflict`。

结论：**这是夹具的重复开窗 / 共享当日窗口状态遗漏，不是领域真实缺陷**。领域"同一 DayStamp 只允许一个窗口快照"是既有正确规则，本用例目标（真实 claim 期间岗位劳动为 0）与窗口冲突无关。

## 3. 精确差异（仅测试文件，3 处）

改动后文件 `bytes=30086`，`sha256=0a020ac751bcd19a9f173c962eb81c4ef1d5048d25a9275bc5d08ff1140b36e6`（改动前 28226 / `c2545824…aed883`）。

a) 新增日程类型反射句柄与夹具字段（测试装配只引用 `Village.Runtime`，`IntradayActivitySchedule` 属 `Assembly-CSharp`，故只能经既有反射通道）：

```csharp
private static readonly Type ActivitySchedule = Type.GetType("IntradayActivitySchedule, Assembly-CSharp", true);
private object _scheduleBeforeTest;
```

b) `Setup` 记录原日程、`TearDown` 用既有 `ConfigureActivitySchedule` 注入点还原，避免跨用例泄漏：

```csharp
_scheduleBeforeTest = Static("ActivitySchedule");                        // Setup，在 ResetForTests 之后
if (_scheduleBeforeTest != null) Call("ConfigureActivitySchedule", _scheduleBeforeTest);  // TearDown
```

c) 用例 13 只把机会点排到本用例推进区间（0→497）之外，使夹具成为当日窗口的唯一建立者；断言零改动：

```csharp
Call("ConfigureActivitySchedule",
    Activator.CreateInstance(ActivitySchedule, new object[] { new[] { 1020, 1200 }, 15, 30, null }));
```

（构造第 2/3/4 参显式写回生产默认 `commuteMinutes=15`、`repeatDelay=30`、`activityDurations=null`，仅 `OpportunityMinutes` 不同；未新增任何日程参数语义。）

d) `CommitRestActivity` 增加一条**前置不变量**（加强而非放宽）：开窗前断言当前无任何当日窗口，并补注释说明同日窗口契约：

```csharp
Assert.IsNull(ActivityCall("QueryCurrentWindow"),
    "夹具推进区间内不得已有当日活动窗口（否则同日开窗按契约返回 WindowConflict）");
```

`3 参真实日窗口 + TryCommitActivity` 建立 claim 的路径保持不变（未伪造 claim 字符串、未镜像劳动判断）。

## 4. 强断言保留核对（逐条对照）

用例 13 语义完全未变：通勤窗口 `LaborCount==0`；`Advance(481)` 后 `"岗位劳动"` 且 `LaborCount==1`、证据 `>0`；`Work.Leave` 后 `"通勤"` 且 `LaborCount==0`；`Advance(496)+Advance(497)` 后 `"岗位劳动"`、`LaborCount==1`、证据递增；活动期间 `QueryClaim` 非空且 `LaborCount==0`。未新增 `Ignore`/`Explicit`/`Assert.Pass`，未删除分支，未把 `WindowConflict` 当成功。

## 5. 编译 / Console / 保护核验（实际执行证据）

1. `editor | call RefreshAssets()` → `RqlEditorApi@1|RefreshAssets()|null`。
2. 编译结束：`editor | select isCompiling isUpdating isPlaying` → `false | false | false`。
3. Console 回读：`console | where logType == "Error" or logType == "Exception" | sort sequence desc | first 10` → 仅 1 条历史错误（sequence 14061，即上一版直接引用 `IntradayActivitySchedule` 的 CS0246，已修复）；最新 12 条日志均为 UI 字体字形 Warning（`TitleText`/`Label`，与本改动无关），**无新编译错误 / 异常**。
4. 编译新鲜度：源码 mtime `2026-10-23 19:57:46.321`；`Library/ScriptAssemblies/VillageStage1.Tests.dll` 写入 `2026-10-23 19:57:49.042` → `fresh=True`（改动已被真实编译）。
5. 34 项受控基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，逐项重算 SHA256）：
   `entries=34 same=34 diff=0 missing=0`。包含 14 项既有修改（字体/Packages/插件/ProjectSettings 等）全部与基线一致，未覆盖 HEAD 旧备份，未删除 `Assets/Editor/StudioRecoveryBackups/2026-10-23T11-49-56-028Z` 及其 meta。
6. 场景状态（宿主 tests-run 前置）：`scenes | select name path isDirty` → `SampleScene | Assets/Scenes/SampleScene.scene | false`（干净，可运行测试）。
7. 只读身份核对：`editor | select unityVersion projectPath` → `2022.3.62t9 | D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`（即唯一授权 Pilot 工程）。

## 6. 未测 / 交宿主门禁（不预填 PASS）

- 按授权**未**自行调用 `tests-run`，也未用 script-execute/反射启动测试绕过归属。`WorkplaceLaborOnlyCountsActualWorkplaceExecution` 与整类 `ManagementProjectionTests` 的最终通过状态须由宿主 Studio 阶段以短类名 `ManagementProjectionTests` 串行复验。
- 未跑全量回归、未验证其他测试类、未做 PlayMode。
- 若宿主复验仍失败，须连同 Console 原文与失败栈回读后重新判断，不能以本文件的静态推理替代实跑结论；若失败点变成 `QueryCurrentWindow` 前置断言或 `ActorIneligible`，说明生产日程/窗口语义再次变化，需回到契约核对而不是放宽断言。
