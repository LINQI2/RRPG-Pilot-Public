# R3-B 专项：劳动撤职绝对/日内到场时间口径修复

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，本任务唯一授权目录）。
- 分支：`ai/rrpg-r3b-population-pilot`；`.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`，该 ref = `054b00d3e38fe2e2e142bfdb2502de90f7991030`（只读两个文件，未执行 Git 命令）。
- Unity 工程：`UnityProject\RRPG1.0game`（当前连接的 Pilot Editor）。
- 阶段：**1/1 —— 修正劳动撤职专项绝对到场时间口径**。本文件不越过阶段验证门禁；通过回执必须由 Studio 阶段 check 产生。
- 本包实际写入（仅 2 个文件）：
  - `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  - `Development/StudioRuns/R3B-UI-20261024/labor-removal-absolute-time-fix.md`（本文件）
- 本包未做：未运行任何测试（未 `tests-run`、未反射启动 TestRunner、未忙等结果）、未 `rql-mutate`、未 Shell/`Process.Start`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未改场景/Prefab/资产/配置、未读写用户存档或存档备份/历史档、未改 34 项基线/14 项保护、未 Git 写（commit/push/stage/切分支）、未启动/关闭 Editor。安全刷新只调用 `AssetDatabase.Refresh()`。
- 本包未改产品 `WorkClock` / `Work` / `Clock` / API / DTO / Authority；未新增测试钩子；未削弱、跳过或删除任何既有强断言；未写死 day 10 作为期望。

---

## 0. 前置核对（本轮只读实核）

| 项 | 实际值 | 来源 |
|---|---|---|
| Pilot 身份 | `.git/HEAD` → `refs/heads/ai/rrpg-r3b-population-pilot`；ref = `054b00d3e38fe2e2e142bfdb2502de90f7991030` | `read` 两个文件 |
| Editor 空闲（改前/改后） | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | `rql-query editor`（改前、Refresh 后各一次） |
| 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | `rql-query scenes`（改前、改后各一次） |
| 34 项 SHA 基线（改前/改后） | **total=34 match=34 problems=0 all-match** | 只读 `script-execute` 按 `workspace-baseline.json` 内 `bytes+sha256` 逐项重算（未写文件） |
| 受保护 TMP 字体（改前/改后） | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 同上，前后同值 |
| 未完成 save run | `R3B-SaveProtection-20261024/` 共 **24** 个 `manifest.json`：23 个 `status="restored"`、1 个 `20261024T052704Z` 为 `protective-snapshot`；**无 captured / restore-failed / absent / no-manifest** | 只读 `glob` + `grep`（未读、未写、未移动存档与备份） |
| 上次 7 组 Restore | 最近一轮 `20261024T062512Z`～`20261024T062518Z` 共 **7 组均 `restored`** | 同上 |
| 上次保护 post | 证据日志末次 `FONT-POST` 同值 `unchanged=True`，`BASELINE-POST total=34 same=34 changed=0 problems=0 all-match` | 只读 `grep` 证据日志 |
| Play 测试发现 | `ManagementShellPlayTests` = **7 条 PlayMode**（assembly `BattleStage1.PlayModeTests`） | `rql-query tests` |
| 原测试回执（当前 Editor `testResults`） | **Passed=6 / Failed=1**（无 skipped 行） | `rql-query testResults | group-by status` |

结论：身份、空闲、场景、34 项基线、字体、存档保护 run 均满足前置，才继续本阶段修复。

---

## 1. 原失败回执与真实栈

任务给定真实回执 `c875296a`：**7 总 / 6 过 / 1 败 / 0 skip**；`CASE-B` 扩展计量 `Passed`。当前 Editor `testResults` 与之一致：Passed=6、Failed=1。

唯一失败用例：

`RRPG.ManagementShell.Tests.ManagementShellPlayTests.WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes`

完整栈（当前 `testResults` 原样读取）：

```text
at RRPG.ManagementShell.Tests.ManagementShellPlayTests+<WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes>d__108.MoveNext ()
  [0x00536] in D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game\Assets\Tests\PlayMode\ManagementShellPlayTests.cs:1841
at UnityEngine.TestTools.TestEnumerator+<Execute>d__7.MoveNext () [0x0003a] in .\Library\PackageCache\com.unity.test-framework@1.1.33\UnityEngine.TestRunner\NUnitExtensions\Attributes\TestEnumerator.cs:46
```

失败断言（原第 1841 行）：

```text
到场分钟必须等于真实派工时刻 + Schedule.CommuteMinutes
Expected: 496
But was:  14896
```

核对：`14896 = 10 × 1440 + 496 = 14400 + 496`。这不是产品把到场分钟算错，而是测试把“日内分钟”与“跨日绝对分钟”混比。

---

## 2. 契约核对（只读源码）

| 契约 | 实际语义 | 来源 |
|---|---|---|
| `VillageTime.CurrentInstant` | `WorldInstant`，`Minute` 为跨日绝对分钟 | `VillageTime.cs` |
| `VillageTime.MinuteOfDay` | `CurrentInstant.MinuteOfDay` = `(int)(Minute % 1440)`，仅日内偏移 | `VillageTime.cs`、`WorldAdvance.cs` |
| `VillageIntradayWork.Arrived(personId, destination, now)` | `now` 为绝对分钟；新登记时 `ReadyMinute = now + Schedule.CommuteMinutes` | `VillageIntradayWork.cs:104-109` |
| `HasArrived` / `IsExecutingWork` | 以 `Time.CurrentInstant.Minute`（绝对分钟）比较 `ReadyMinute` | `VillageRuntimeBridge.Work.cs:24-27,111-115` |
| `WorldInstant.NextBoundary` | 以 `from.Minute - from.MinuteOfDay` 取当日基点，再叠加日内 schedule 偏移 | `VillageIntradayWork.cs:117-125` |
| `VillageTime.AdvanceTo(WorldInstant)` | 目标是绝对 `WorldInstant`，且 `target.Minute < CurrentInstant.Minute` 返回 `InvalidTarget`（不能回拨） | `VillageTime.cs:106-123` |

因此：

- `IntradayWork.Arrival.ReadyMinute` 的口径是**绝对分钟**；
- `Clock.MinuteOfDay` 的口径是**日内分钟**；
- 本用例前置已推进到 `14400 + 481 = 14881`（绝对分钟），此时真实派工分钟是 `14881`，`ReadyMinute` 正确为 `14881 + 15 = 14896`；
- 旧测试却用 `ClockMinuteOfDay()` 得到 `481` 作为派工分钟，期望 `481 + 15 = 496`，与真实 `14896` 比较。`14896 - 496 = 14400`，正好是 10 天绝对偏移。

**真实根因：测试侧绝对/日内语义混比，不是 `WorkClock`、`Arrived`、生产时钟或派工行为失败。**

---

## 3. 精确修改（仅 `ManagementShellPlayTests.cs`）

统一原则：CASE-D 内所有与 `IntradayWork.Arrival.ReadyMinute`、`AdvanceTo`、真实跨日派工/撤职时刻相关的 expected，均从**真实 `Clock.CurrentInstant.Minute` 绝对分钟 + 真实 `Schedule.CommuteMinutes/Lunch`** 派生；`AdvanceTo` 按其真实语义接收绝对目标分钟，跨日由绝对分钟自然表达。

### 3.1 派工分钟改为绝对分钟

修改前：

```csharp
long assignMinute = ClockMinuteOfDay();
ClickButtonField(management, "assignButton", "派工·劳动中撤职人物");
yield return null;
Assert.AreEqual(assignMinute, ClockMinuteOfDay(), "Clock 暂停时真实派工不得自行推进时间");
```

修改后：

```csharp
// Clock.CurrentInstant.Minute 是跨日绝对分钟；VillageIntradayWork.Arrived 也以该绝对分钟登记
// ReadyMinute。CASE-D 在 14400 分钟后运行，不能用 MinuteOfDay 与 arrival 的绝对分钟混比。
long assignMinute = ClockAbsoluteMinute();
ClickButtonField(management, "assignButton", "派工·劳动中撤职人物");
yield return null;
Assert.AreEqual(assignMinute, ClockAbsoluteMinute(), "Clock 暂停时真实派工不得自行推进时间");
```

`readyMinute = assignMinute + commuteMinutes` 公式保持不变；因 `assignMinute` 现在是绝对分钟，`readyMinute` 自动成为绝对到场分钟，`AdvanceTo(readyMinute)` 也按真实绝对目标推进，不再出现“回拨到 496”的隐含错误。

### 3.2 撤职分钟改为绝对分钟

修改前：

```csharp
long dismissalMinute = ClockMinuteOfDay();
Assert.AreEqual(readyMinute + 1, dismissalMinute, "撤职必须发生在真实到场后的下一工作分钟");
```

修改后：

```csharp
long dismissalMinute = ClockAbsoluteMinute();
Assert.AreEqual(readyMinute + 1, dismissalMinute, "撤职必须发生在真实到场后的下一工作分钟");
```

### 3.3 下一工作区间边界仍由真实绝对日基 + Schedule.Lunch 派生

原实现：

```csharp
long dayBase = dismissalMinute - (dismissalMinute % 1440L);
long nextBoundaryMinute = dayBase + lunch;
```

该式在 `dismissalMinute` 改为绝对分钟后自然正确：本夹具得到 `dayBase = 14400`、`nextBoundaryMinute = 14400 + lunch`。**没有新增任何 `10 * 1440` / `14896` / `day 10` 常量作为 expected**；`1440` 仅表示 `WorldInstant.MinutesPerDay` 的日长语义，不是写死“第 10 天”。

### 3.4 保留不变

- 真实按钮派工、真实按钮撤职、真实 `AdvanceTo` 推进；
- 非零劳动证据前置 `evidenceBefore > 0`；
- 撤职当刻“空闲”标签、`IsExecutingWork == false`、绑定 0、占用 0、状态含“已移除”；
- 已结算 `WorkplaceEvidenceMinutes` 不追回、不回滚；
- 旧 `arrival` 行残留事实（dest/ready 保持原值）且不得复活劳动；
- 撤职后下一 interval 无新增岗位证据、旧 arrival 在下一工作区间完成边界被清理；
- 重开管理复核 `BoundCount=0 / LaborCount=0 / evidence` 不变；
- 所有既有断言未删除、未 skip、未放宽。

未修改：`ManagementShellPlayTests.cs` 之外的任何产品源码、场景、Prefab、资产、配置、WorkClock/API/DTO/Authority。

---

## 4. 编译 / 保护回执（本轮实核）

| 项 | 实际值 |
|---|---|
| 刷新方式 | `script-execute` 内仅 `AssetDatabase.Refresh()`（未用 `rql-mutate`、未全局保存） |
| 刷新后 Editor | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` |
| 刷新后场景 | `SampleScene`，`isDirty=false`，`rootCount=5` |
| 编译结果 | 无编译错误；`ManagementShellPlayTests` 仍被发现为 **7 条 PlayMode** |
| Console Error/Exception | `rql-query console` 过滤 Error/Exception 为 **0** 条 |
| 34 项基线（改后） | **34/34 all-match，problems=0** |
| 受保护 TMP 字体（改后） | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与改前同值） |
| 存档 / 备份 / 历史档 | 未读、未写、未移动、未恢复；本轮未产生新 save run |
| 未完成 save run（改后） | 仍为 24 run：23 restored + 1 protective-snapshot，无异常状态 |
| 产品代码 / API / DTO / Authority | 未改；仅测试文件与本文档 |
| Git 写 | 无 commit/push/stage/切分支 |

> 说明：刷新后首次 `rql-query editor` 因插件连接短暂断开返回一次连接错误；未重发该命令，随后重新只读查询得到的 Editor 状态与场景状态如上。该连接错误不影响文件与编译结果。

---

## 5. 未测 / 交接

1. **本包未运行任何测试**，因此不能声称该用例或整类已通过。当前 `testResults` 仍是修复前的 `6 Passed / 1 Failed` 回执，仅用于证明根因，不是本包结果。
2. 请 Studio 阶段以短类名 `ManagementShellPlayTests` 做 **PlayMode 整类串行** check（7 例）。若通过，通过回执才可登记。
3. 修复只统一了 CASE-D 的绝对/日内口径；CASE-D 在失败点之后仍有“撤职当刻绑定/证据/旧 arrival”与“下一午休边界清理”等强断言未在本包运行验证。若宿主运行暴露新的业务失败，应按真实回执区分“测试推导错误 / 产品行为差异”，不得通过削弱断言、改生产时钟或伪造 arrival 求绿。
4. 本包未覆盖/未重测：跨日 Day Tick、下班边界、第二天上午补登记、多 Slot、真实远征 Return gating 等；这些仍属既有边界，不用旁证冒充通过。
5. 本文件不构成通过声明，也不代制作人批准；完成报告须由主开发专业核验后交 Studio 阶段继续。
