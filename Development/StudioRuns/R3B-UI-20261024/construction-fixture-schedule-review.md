# R3-B 专项：施工计量夹具（CASE-B）随机活动干扰隔离

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，本任务唯一授权目录）。
- Pilot 身份（只读两个文件，未执行任何 Git 命令）：`.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`；`.git/refs/heads/ai/rrpg-r3b-population-pilot` = `054b00d3e38fe2e2e142bfdb2502de90f7991030`。
- Unity 工程：`UnityProject\RRPG1.0game`（当前连接的 Pilot Editor）；`Application.dataPath` 推出的仓库根 = 本目录，与 34 项基线 `root` 一致。
- 阶段：**1/1 —— 隔离施工计量夹具的随机活动干扰，只修 `ManagementShellPlayTests` 的 CASE-B 合法活动前置**。本文件不越过阶段验证门禁；通过回执必须由 Studio 阶段 check 产生。
- 本包实际写入（仅 2 个文件）：
  - `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  - `Development/StudioRuns/R3B-UI-20261024/construction-fixture-schedule-review.md`（本文件）
- 本包未做：未 `tests-run`、未反射启动 TestRunner、未忙等结果（按阶段约定由宿主整类跑）；未 `rql-mutate`；未 Shell/`Process.Start`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未改场景/Prefab/资产/静态配置；未读写用户存档与存档备份/历史档；未改 34 项基线/14 项保护；未 Git 写（commit/push/stage/切分支）；未安装组件、未改权限。编译/刷新只调用 `AssetDatabase.Refresh()`。
- 本包未改任何产品源码、API、DTO、Authority、活动规则；未新增测试钩子；未 weakening/skip/删除任何既有断言；未降低候选人数、未清 claim、未造人。

---

## 0. 前置核对（本轮只读实核）

| 项 | 实际值 | 来源 |
|---|---|---|
| Pilot 身份 | 分支 `ai/rrpg-r3b-population-pilot`，ref = `054b00d3e38fe2e2e142bfdb2502de90f7991030` | `read` `.git/HEAD` + ref 文件 |
| 34 项基线 `root` | `D:\AI_Agent_Lab\rrpg_pilot\repo`，与本工程一致 | `read` `Development/StudioRuns/R3B-20261022/workspace-baseline.json` |
| Editor 空闲（改前） | `isPlaying=false / isPaused=false / isCompiling=false / selection=[]` | `rql-query editor` |
| Editor 空闲（编译后） | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | 同上，重连后复查 |
| 场景干净（改前/改后） | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | `rql-query scenes` 两次 |
| 34 项 SHA 基线（改后） | **entries=34 problems=0 all-match**（doc-root-match=True） | 只读 `script-execute` 按基线 `bytes+sha256` 逐项重算（未写文件） |
| 受保护 TMP 字体（改后） | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（14961033 bytes）= 基线值 | 同上 |
| 上次运行基线/字体 | 证据日志末次 `BASELINE-PRE total=34 diff=0 all-match`、`FONT-PRE/FONT-POST` 同值 `unchanged=True`、`BASELINE-POST total=34 same=34 changed=0 problems=0 all-match` | 只读 `grep` `R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`（line 1664–1787） |
| 未完成 save run | `R3B-SaveProtection-20261024/` 共 115 个 `manifest.json`；对 `captured / absent / restore-failed / unreadable` 的只读检索**无匹配**；最新 10 组（`20261024T141851Z`…`20261024T141859Z`）逐个为 `status="restored"`、`restored=true`、`lastError=null`、`readbackVerified=true` | 只读 `glob` + `grep` + `read` 两个 manifest（`141851Z`、`141859Z`） |
| Play 测试发现 | `ManagementShellPlayTests` = **10 条 PlayMode**（改前、编译后各一次） | `rql-query tests` |
| 最后一批回执 | **10 总 / 9 过 / 1 败 / 0 skip**；唯一失败 = `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`（Failed, 534ms），`FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt` = **Passed**（597ms） | 只读 `rql-query testResults`（当前 Editor 最后一次运行结果，逐条 status） |

结论：身份、空闲、场景干净、34 项 SHA 与受保护字体一致、存档保护 run 无未完成，才继续本阶段。

---

## 1. 失败证据：本次直接 claim 证据 vs. 历史旧失败的推断

### 1.1 直接证据（本轮实读，可用于根因判定）

任务给定回执 `1088aa8e`（10 总 / 9 过 / 1 败，FirstNavigation Passed）。该 run id **在仓库内无任何文件记录**（对 `Development/`、`UnityProject/` 全文检索无匹配），因此本包**不把“当前 `testResults` 就是 1088aa8e”当作已验证事实**；能把两者对应起来的只有时间与条数的一致性（见 1.2），属**推断**。

直接可核事实（只读 `rql-query testResults`，当前 Editor 最后一次运行）：10 条 PlayMode，9 `Passed`，唯一 `Failed` = `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`；`FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt` = `Passed`。

失败断言原文（`testResults` message 原样摘录，`Expected: greater than or equal to 2 / But was: 1`）：

```text
  本夹具需要至少 2 名未被自主活动占用且无既有职责的真实人物；实际 1；claims=
  person_14d81565e0ea4fc590d513497f2f0f47=<null>;
  person_7a10d1a465e04e1aaf39f201f3d00727=village.intraday|420|free|...|prototype.talk...|activity;
  person_07826cd718b844f4b70a2b29fc7c1484=village.intraday|420|free|...|prototype.rest...|activity;
  person_2d9b60bafdc64edc83506b2b58d13a3d=village.intraday|420|free|...|prototype.talk...|activity;
  person_023a036163f24dfc945254d37e490f22=village.intraday|420|free|...|prototype.drink...|activity;
  person_bdc7f55ad2ed46ecb98523a2c7828bee=village.intraday|420|free|...|prototype.drink...|activity;
  Expected: greater than or equal to 2
  But was:  1
```

栈（原样）：`...<ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor>d__119.MoveNext () ... ManagementShellPlayTests.cs:1646`（修复前行号，即前置 `freePersons` 断言）。

同一次运行的证据日志（`R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` line 1664–1787）给出可交叉核对的直接序列：

- `14:18:51Z` `BASELINE-PRE total=34 diff=0 all-match`；`14:19:00Z` `FONT-POST unchanged=True`、`BASELINE-POST problems=0 all-match`；
- `14:18:51Z`→`14:18:59Z` 恰好 **10 个** `SaveSlotGuard` run，全部 `SAVE-RESTORE matches-before=True`（即 10 例串行、无中断）；
- `14:18:55Z` `[FIRST-NAV]` 从 `periodic-reset`→`pre-click`→`same-frame-committed`→`next-frame-before-tab-click`→`population-tab-clicked`→`first-click-committed` 完整走完（无失败栈），与 `testResults` 的 `FirstNavigation ... Passed` 一致；
- `14:18:56Z` `[CASE-B] START` → `[ADVANCE] targetMinute=479` → `[CASE-B] activity claims at minute 479: …`（6 人里 **5 人**带 `village.intraday|420|free|…` claim，仅 1 人 `<null>`）→ 立刻 `SAVE-RESTORE`（无后续 CASE-B 日志），即失败发生在该前置断言处。

因此：**“唯一失败是 CASE-B 的‘自由候选仅 1’”是直接证据；“这次运行就是 1088aa8e”是推断（时间/条数一致，run id 未落盘）。**

### 1.2 随机性直接证据（同一前置在不同运行中取值不同）

同一日志中该前置的多次真实取值（同一类、同一用例）：

| 时间（日志） | 6 人中被 420 活动占用 | 自由候选 | 结果 |
|---|---|---|---|
| `2026-10-24T05:58:11Z` | 1 | 5 | 继续执行 |
| `2026-10-24T06:25:16Z` | 4 | 2 | 继续执行（恰达下界） |
| `2026-10-24T06:28:16Z` | 2 | 4 | 继续执行 |
| `2026-10-24T11:05:27Z` | 2 | 4 | 继续执行 |
| `2026-10-24T11:14:52Z` | 2 | 4 | 继续执行 |
| `2026-10-24T11:18:54Z` | 1 | 5 | 继续执行 |
| `2026-10-24T11:42:43Z` | 1 | 5 | 继续执行 |
| `2026-10-24T11:49:35Z` | 4 | 2 | 继续执行（恰达下界） |
| `2026-10-24T11:58:14Z` | 1 | 5 | 继续执行 |
| `2026-10-24T14:02:33Z` | 2 | 4 | 继续执行 |
| `2026-10-24T14:08:56Z` | 2 | 4 | 继续执行 |
| `2026-10-24T14:18:56Z` | **5** | **1** | **失败（前置不满足）** |

结论：这是夹具前置的**随机活动干扰**，不是施工业务断言失败——失败点在 `AddProjectParticipantCore`/通勤/到场/劳动/撤出/再加入等业务断言**之前**，业务断言从未执行到。

### 1.3 历史旧失败（本次未复核，不得当作已修）

`Development/StudioRuns/R3B-UI-20261024/` 下的既有报告（如 `labor-removal-absolute-time-fix.md`、`construction-p2-arrival-integration-review.md` 等）记录过更早的 CASE-B/CASE-D 失败。本包**没有重跑**这些历史失败点，只依据当前 `testResults` 证明它们在本批运行中为 `Passed`；历史报告里的旧根因描述属历史结论，不是本包的验证结果。

---

## 2. 根因（只读源码核对）

| 契约 | 实际语义 | 来源 |
|---|---|---|
| `IntradayActivitySchedule` 构造器 | `opportunities` 默认 `{420,1020,1200}`；只拒绝 `x<0 \|\| x>=1440`、`commute<1`、`repeat<1`；空集合合法（`Distinct().OrderBy()` 得空数组） | `Assets/Scripts/ActivityHost/IntradayActivitySchedule.cs:12-28` |
| 活动机会点消费 | `NextActivityBoundary` / `IsActivityOpportunity` 只遍历 `ActivitySchedule.OpportunityMinutes`（全仓仅这两处引用） | `VillageRuntimeBridge.Activities.cs:18,40` |
| 机会点开窗 | `OpenIntradayOpportunity` 在机会点按“在场、无 claim、非执行工作”筛选合格者并交给 AutonomousSelection 决策 | `VillageRuntimeBridge.Activities.cs:104-133` |
| claim 优先 | 被 claim 者会跳过 `RefreshWorkIntent → ArrivedWorkers` 的到场登记（`PresentWorkPersons` 等在生产侧排除被占用者） | 测试文件原注释 + `OpenIntradayOpportunity` 的合格者筛选 |
| 既有合法注入点 | `public static void ConfigureActivitySchedule(IntradayActivitySchedule schedule)`，`GuardWorldWrite()` 通过后替换 `ActivitySchedule`；无新增 API | `VillageRuntimeBridge.Activities.cs:7-13` |

同理测试已有先例（同库合法用法，只读参考）：`Assets/Tests/EditMode/Village/ManagementProjectionTests.cs:41`（`SetUp` 捕获 `ActivitySchedule`）、`:49`（`TearDown` 用 `ConfigureActivitySchedule` 还原）、`:375-383`（在推进区间外排除机会点，注释明确“不改领域规则”）。

根因：CASE-B 在 `Clock` 暂停下把世界从当日 0 分钟推进到 479，途中 `420` 机会点会为当时在场人物建立 claim；claim 数随选择/配对随机变化，直接决定“无 claim 且无既有职责”的自由候选数。夹具要求 ≥2，随机落到 1 时前置失败。

---

## 3. 精确修改（仅 `ManagementShellPlayTests.cs`）

### 3.1 新增：活动日程捕获 / 隔离 / 恢复（只走既有公开注入点）

新增字段：

```csharp
/// <summary>施工计量夹具隔离前捕获的真实活动日程（TearDown 恢复后置空）。</summary>
private object _activityScheduleBefore;
```

新增只读辅助：`ActivitySchedule()`（读 `VillageRuntimeBridge.ActivitySchedule` 公开 getter，不反射私有状态）、`OpportunityPoints(schedule)`、`ActivityDurationSnapshot(schedule)`（只读记录 `Rest/Drink/Talk/VisitSick/Help` 时长，用于前后对照）。

新增 `IsolateActivityOpportunitiesForFixture(string what)`（CASE-B 开头调用，**在第一次 `AdvanceTo(479)` 之前**）：

1. 捕获当前真实日程对象引用；
2. 读取 `CommuteMinutes` / `RepeatOpportunityDelay` 并原值复用；
3. 只保留“当日当前分钟之前”的机会点（已过去的点不会再次触发），排除本例推进跨度内的全部机会点（本例从当日 0 分钟推进到约 816 分钟，故保留集为空数组；**空机会点集是构造器合法输入，未使用 >=1440 的非法分钟**）；
4. 用 `Activator.CreateInstance(IntradayActivitySchedule, { points, commute, repeat, null })` 构造隔离日程，`activityDurations=null` 即产品默认时长（与生产默认日程一致，已在证据中前后对照记录）；
5. 经既有 `ConfigureActivitySchedule` 注入，并断言注入后的实例是刚构造实例、且注入后不残留本例跨度内的机会点；
6. 写 `CASE-B-SCHEDULE` 证据（原机会点、原时长、commute/repeat、隔离后机会点与时长、窗口起点）。

新增 `RestoreActivityScheduleAfterFixture()`（`TearDown` 调用）：

- 无捕获时为幂等空操作；
- 若当前不是捕获实例，则用同一公开注入点写回**捕获的同一实例**；
- **只读校验**：`ReferenceEquals` 同一实例、机会点逐项 `CollectionAssert.AreEqual`、`CommuteMinutes` 相同，并写 `SCHEDULE-RESTORE` 证据。

### 3.2 `TearDown` 接入（try/finally 保证存档兜底不被阻断）

```csharp
try
{
    // 先恢复本例可能注入的活动日程并只读校验；恢复失败不掩盖，也不阻止存档兜底恢复。
    RestoreActivityScheduleAfterFixture();
}
finally
{
    try { /* ForceSaveFailureForTests 复位 */ }
    finally { try { _saveGuard.Restore(); } finally { ResetForTests(); } }
}
```

即：日程恢复放在最外层 `try`，无论它是否抛异常，原有“ForceSaveFailure 复位 → 单槽存档兜底恢复 → `ResetForTests`”链路都照旧执行；异常不会被吞掉。域重载 / 进程 abort 不会执行 `TearDown`，但静态日程随域重建，不会泄漏到下一夹具（本包不宣称进程级保证）。

### 3.3 CASE-B 调用点与注释

```csharp
public IEnumerator ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor()
{
    // 夹具窗口所有权隔离（既有公开注入点 ConfigureActivitySchedule，未改产品规则）：
    // 420 分钟自主活动机会随机占用在场人物（真实回执：6 人里 5 人被占，只剩 1 名自由候选）…
    IsolateActivityOpportunitiesForFixture("CASE-B construction measurement fixture");
    string a = PlaceBuilding("building_farm", 4f);
    ...
```

同时更新了“真实活动占用前置”注释：说明本例只在自身时间跨度内排除机会点，真实 `QueryClaim` 扫描与断言本身不做任何放松。

### 3.4 保持不变（全部强断言）

- 自由候选的真实 `ActivityQueryClaim` 扫描与诊断（`claims=…`、`Assert.GreaterOrEqual(freePersons.Count, 2)`、p1/p2 加入前 `Assert.IsNull(ActivityClaim(...))` 全部保留）；
- 0 worker 推进 479 后 `Progress=0 / WorkerCount=0 / 无活动项目`；
- 真实管理按钮（导航、页签、施工行、加入/撤出/再加入）与真实 PointerClick；
- Primary Assignment（`TryAssignPerson(p1, "ws_farm_01")`）不被施工改写；静态兼容岗位 UI 事实；
- 通勤 `+Schedule.CommuteMinutes`（本例 494）、到场与项目劳动、撤出单人计量（1 人×区间）、再加入双人计量（2 人×区间）、`AssertProjectInterval` 的精确 `Progress/LaborRemainder` 推导；
- 未清 claim、未造人、未降候选人数、未 skip、未改产品活动规则。

未修改：`ManagementShellPlayTests.cs` 之外的任何产品源码、场景、Prefab、资产、配置、API、DTO、Authority。

---

## 4. 编译 / 保护回执（本轮实核）

| 项 | 实际值 |
|---|---|
| 刷新方式 | `script-execute` 内仅 `AssetDatabase.Refresh()`（未用 `rql-mutate`、未全局保存） |
| 刷新后 Editor | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` |
| 刷新后场景 | `SampleScene`，`isDirty=false`，`rootCount=5` |
| 编译结果 | 无项目编译错误；`ManagementShellPlayTests` 仍被发现为 **10 条 PlayMode** |
| Console Error/Exception | 最近 3 条 Error/Exception 全部是 `22:17:32` 一次**临时脚本片段**自身编译失败（`snippet.cs(28,27) GetCustomAttributes`），早于本次修改且与项目无关；本次编辑后无新增项目编译错误 |
| 34 项基线（改后） | `entries=34 problems=0 all-match`，doc-root-match=True |
| 受保护 TMP 字体（改后） | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（14961033 bytes，与基线同值） |
| 隔离日程构造路径只读验证 | `script-execute` 只读构造：`type=IntradayActivitySchedule`、空机会点集合法、`commute=15 / repeat=30 / Rest=60`；线上实测 `ActivitySchedule` = `[420,1020,1200]`、`commute=15`（未写任何状态） |
| 存档 / 备份 / 历史档 | 未读、未写、未移动、未恢复；本轮未产生新 save run |
| 未完成 save run（改后） | 仍无 `captured/absent/restore-failed/unreadable` |
| 产品代码 / API / DTO / Authority | 未改；仅测试文件与本文档 |
| Git 写 | 无 commit/push/stage/切分支 |

> 说明：`AssetDatabase.Refresh()` 触发域重载时，插件连接曾断开一次（`RunCallTool` 报 disconnected），未重发该命令；重连后只读查询得到上述 Editor/场景状态。该连接错误不影响文件与编译结果。

---

## 5. 未测 / 风险 / 交接

1. **本包未运行任何测试**，因此**不能声称** CASE-B 或整类已通过。当前 `testResults` 仍是修复前的 9 过 / 1 败回执，只用于证明根因，不是本包结果。
2. 请 Studio 阶段以短类名 `ManagementShellPlayTests` 做 **PlayMode 整类串行** check（10 例），并核对证据日志新增行：
   - `CASE-B-SCHEDULE … original-points=[420,1020,1200] … isolated-points=[] …`；
   - `CASE-B activity claims at minute 479: …` 应为 6 人全 `<null>`；
   - `CASE-B` 后续 `p1/p2 joined construction at minute 479 … ready=494` 等计量行；
   - `SCHEDULE-RESTORE restored=true sameInstance=True points=[420,1020,1200]`（每个用例都有，说明恢复确实执行）。
3. **未测边界（不得旁证冒充）**：
   - 其它 9 例“仍跑真实活动机会”本包只用“恢复同一实例 + 只读校验”的设计保证，未实际观测其活动机会是否触发；`SurfaceEquivalentSimulation` 未改一行，其 `Advance 14400→16360` 区间本身不跨 1020/1200 类机会点，真实活动机会是否在其中产生 claim 未在本包观测。
   - 隔离日程的 `activityDurations=null`（产品默认时长）与捕获日程的时长一致性只做了**只读记录对照**（`CASE-B-SCHEDULE` 中两组 `durations`），未对“自定义时长日程”场景做断言；本例窗口内不触发任何活动边界，时长不参与计算，且原实例已被恢复。
   - 域重载 / 进程 abort 时 `TearDown` 不执行，日程静态值随域重建；本包不宣称进程级恢复保证（与既有存档保护口径一致）。
   - 未复核 `Development/StudioRuns/R3B-UI-20261024/` 下历史报告中记录过的旧失败点（本批显示 Passed，但本次未重跑单点验证）。
4. 若宿主运行暴露新的业务失败，应按真实回执区分“测试前置/推导错误”与“产品行为差异”，不得通过削弱断言、跳过用例、清 claim、减少候选人数或关闭全局活动求绿。
5. 本文件不构成通过声明，也不代制作人批准；完成报告须由主开发专业核验后交 Studio 阶段继续。
