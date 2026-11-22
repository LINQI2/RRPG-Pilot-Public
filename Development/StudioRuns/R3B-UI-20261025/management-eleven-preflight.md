# R3-B 管理十一例集成复验前置核查（只读）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 日期：**2026-10-25**（Asia/Shanghai；UTC+8；日志时间为 UTC）。
- 阶段：**1/1**，只读核对恢复证据与串行复验前置，不重做、不改代码、不运行测试。后续整类 PlayMode 由宿主 Studio 阶段 check 执行。
- 结论状态：**前置全部成立**（Editor 空闲 / 场景干净 / 测试源码与 DLL 一致且 11 例可发现 / 34 项与字体一致 / 单槽存档 `13405B / 4b2b56f6…d6aa` 与最近合法恢复一致 / 无未完成 `SaveSlotGuard` run / 最新单例恢复证据完整）。**本文件不含任何十一例 PASS 结论**，本执行层未运行 `tests-run`、未启动或忙等 TestRunner。
- 边界（本阶段实际未做）：未 `rql-mutate`；未 Shell / `Process.Start`；未 Git 写；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 `AssetDatabase.Refresh`；未改产品源码 / 测试源码 / 断言 / Scene / Prefab / 资产 / 静态配置 / 字体 / Packages / ProjectSettings / 34 项基线 / 存档本体 / `SaveSlotGuard` / 备份历史 run / 已有证据日志；未恢复任何存档、未清任何锁；未安装组件、未改权限、未创建额外付费子 Agent。全部读取仅用 `rql-query` 与只读 C# 片段（无文件写入）。

---

## 1. 开工前只读实核（当前连接）

| 项 | 实测 | 证据来源 |
|---|---|---|
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | `rql-query editor` |
| 场景 | 仅 `SampleScene`，`isLoaded=true`、**`isDirty=false`**、`rootCount=5` | `rql-query scenes` |
| TestRunner | `testResults` **仅 1 条**（即主开发记录的 `ba019b1c` 单例，`Passed`、`durationMs=1177`、`message=null`），无 running / pending / 未知投递记录；本执行层未投递任何测试请求 | `rql-query testResults` |
| 测试发现 | `tests where fullName contains "ManagementShellPlayTests"` = **11 条，全部 PlayMode**，`ignored=false`、`explicit=false`、`parameterized=false` | `rql-query tests` |
| 34 项 workspace baseline | **entries=34 / same=34 / diff=0 / missing=0**（逐项重读字节 + SHA256，按 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 的 `bytes+sha256`） | 只读 `script-execute` |
| 受保护 TMP 字体 | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**（与基线同值） | 同上 |
| 单槽存档 | **`1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：13405B / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`** | 只读 `SaveSystem.GetSavePath()` + SHA256 |
| SaveSlotGuard 全部 run | **151**：`restored=150` + `protective-snapshot=1`；**unfinished/未知=0**（无 `captured`、无 `restore-failed`、无 no-manifest） | 只读遍历 151 个 `manifest.json` 的 `status` |
| 最新 run | `20261025T073850Z`：`status=restored`、`sourceBytes=backupBytes=13405`、`backupSha256=4b2b56f6…d6aa`、`readbackVerified=true`、`lastError=null` | 该 run `manifest.json` |

- 单槽当前值与最新合法恢复 `backupSha256` **同值**，说明该 run 恢复后未被后续写盘污染；本阶段**未新增 run、未恢复、未改任何历史 run/备份**。
- 未完成 / 未知 run 为 0，故不存在“需要恢复的未知档”，也不存在需要清理的锁。

---

## 2. 最新单例恢复证据摘录（宿主 `ba019b1c` 那一次运行产生，来源为同一 Editor 的 console 与证据日志）

> 下列为**历史运行回执**，由主开发核验；本执行层只读摘录，未重跑、未补跑。

console（sequence 54593–54614）与 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 末段（2026-10-25T07:38:50Z–07:38:52Z）一致：

```
[BASELINE-PRE] total=34 diff=0 all-match
[FONT-PRE] sha256=3c758e16…c404854
[CASE-B-SCHEDULE] captured=IntradayActivitySchedule original-points=[420,1020,1200]
  original-durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90} commute=15 repeat=30
  isolated-points=[] …
[EVENT-GATE] SPEED open paused=Paused clock=72000 session=WaitingForInteraction
[EVENT-GATE] SPEED resume=Normal clock=72000 then playerPause=Paused
[EVENT-GATE] direct advance resumed once clock=72060 …
[EVENT-GATE] END return catchup once clock=73500 phase=CatchUpComplete days=1 history=2 … repeat=AlreadyCaughtUp projection=consistent
[SAVE-RESTORE] runId=20261025T073850Z existed=True sha256=4b2b56f6…d6aa matches-before=True
[SCHEDULE-RESTORE] restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}
[FONT-POST] sha256=3c758e16…c404854 unchanged=True
[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match
```

要点：

1. **`EVENT-GATE END`**：Return catch-up 只结算一次（`once`），`phase=CatchUpComplete`、`days=1`、`history=2`、`repeat=AlreadyCaughtUp`、`projection=consistent`。
2. **`SAVE-RESTORE`**：`matches-before=True`，回读 hash 与本次备份相同（`4b2b56f6…d6aa`），manifest 终态 `restored`、`readbackVerified=true`。
3. **`SCHEDULE-RESTORE`**：`sameInstance=True`，机会点 `[420,1020,1200]`、`commute=15`、`repeat=30`、五项时长 `Rest/Drink/Talk=60`、`VisitSick/Help=90` **全部**与用例前捕获值一致。
4. **恢复顺序已生效**：证据日志 2297→2298 与 console 54611→54612 显示 `SAVE-RESTORE`（07:38:52.034）先于 `SCHEDULE-RESTORE`（07:38:52.0368），即“存档兜底恢复 → `ResetForTests` 释放世界写锁 → 最后恢复活动日程”的实际执行次序；日程断言失败不会短路存档恢复。
5. `BASELINE-PRE/POST` 双端 34 项 all-match、`FONT-PRE/POST` 同 hash `unchanged=True`。

---

## 3. 与主开发宿主记录的区别（不冒认执行）

- 主开发本轮宿主记录：**`ba019b1c` 单例 1/1 Passed、0 skip、`scope-restored`**；该记录由主开发核验。
- 本执行层**没有运行**该单例，也没有运行任何测试；`testResults` 中的 1 条 `Passed` 是该次历史运行留在当前 Editor 的结果，**不是本层产生**。本层只读观察它与 `tests` 的 11 条发现一致，不据此宣称整类或任一新用例通过。
- 十一例整类串行结果只能由宿主 Studio 阶段 check 产出；本文件不预填。

---

## 4. 测试源码 / DLL / 发现（未改代码）

| 项 | 实测 | 说明 |
|---|---|---|
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **294535B / `cc55cb405216f43cb4b090d068f43446d4ccbfecf130c5274f4791388f80715a`** | 与上一包修复后记录**逐字节一致**，本阶段未改 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | **451584B / mtime `2026-10-25T07:37:20.1539053Z`** | 晚于源文件 mtime `2026-10-25T07:37:17.3737039Z`（DLL 新鲜） |
| DLL 语义抽检（UTF-16LE 字节精确搜索，各命中 1 次） | `活动日程必须恢复为用例前捕获的同一实例`、`SCHEDULE-RESTORE`、`sameInstance=True`、`IsWorldWriteLocked=`、`活动日程五项活动时长必须恢复到用例前原值` | 新断言/证据确实进入程序集 |
| 反射 | `[UnityTest]` 方法 **11** 个；`TearDown`、`RestoreActivityScheduleAfterFixture` 均存在 | 与 `tests` 查询一致 |
| 运行中程序集 | `asm.Location` = `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`，字节与磁盘 DLL 完全相同 | 无第二份旧程序集参与 |

关键源码核对（未改动）：

- `TearDown`（`ManagementShellPlayTests.cs:1194-1225`）：`ForceSaveFailureForTests` 清理 → `finally` 内 `_saveGuard.Restore()` → `finally` 内 `ResetForTests()`（释放 `Time` / `_returnRecord`，即释放世界写锁）→ **最外层 finally** `RestoreActivityScheduleAfterFixture()`。
- `RestoreActivityScheduleAfterFixture`（`:441-472`）：先经既有公开入口 `ConfigureActivitySchedule(before)`，再独立只读校验“同一实例 + 机会点逐项 + `CommuteMinutes` + `RepeatOpportunityDelay` + 五项时长”，全部通过后才清空捕获并写 `SCHEDULE-RESTORE`；失败保留捕获并抛错（下一例捕获时会直接失败，作为不确定状态停止信号）。
- 捕获日程的用例共 2 例（`:1752` 建造/工地计量、`:4124` 统一事件 pending 门禁），二者都走同一 TearDown 恢复路径；**其余 9 例无捕获时该恢复为空操作**，但 `TearDown` 的存档恢复与 `ResetForTests` 对 11 例**每例都执行**。

---

## 5. 存档保护与停止条件

- 151 个 run 全部终态（`restored=150` + `protective-snapshot=1`），`unfinished/未知=0`；最新 run 与单槽同 hash。本阶段未新增 run。
- 若宿主整类运行出现 `restore-failed` / `captured` / 未知 manifest：按现有守卫会阻止后续用例；处理条件仅为“用**该 runID** 的 `original-save.bin`，回读 SHA256 与 manifest `backupSha256` 一致后才允许写回”，否则停止并保留现场取证。**不恢复未知档、不清锁、不重试覆盖。**
- 任何异常（编译错误、场景 dirty、保护断言失败、恢复断言失败）都按原样保留证据并停止受影响部分，不继续下一例制造“部分通过”。

---

## 6. 交宿主（本阶段不做的事）

- 宿主随后以**短类名 `ManagementShellPlayTests` 整类、PlayMode、串行**运行 11 例；本层不启动测试、不忙等。
- 验收口径：**11 例全部 Passed、0 skipped、0 项未执行**；每例 `TearDown` 的存档恢复、`ResetForTests`、日程恢复核对都必须真实执行（不是 skip / 早退），任一失败或 0 项/skip 均不算通过。
- 通过时每例都会产生 `SAVE-*` 与 `TearDown` 证据；`SCHEDULE-RESTORE` 只在捕获过日程的 2 例（建造计量、统一事件 pending 门禁）出现，其余 9 例为空操作——宿主不应把“未出现该行”误判为未执行。
- 运行结束后仍以测试自身 `BASELINE-POST total=34 all-match`、`FONT-POST unchanged=True`、`SaveSlotGuard` manifest 终态 `restored` 与单槽 hash 为保护面回执。

## 7. 本阶段实际未测 / 未做

- 未运行 `tests-run`、未反射启动 TestRunner、未忙等；不含任何十一例 PASS/FAIL。
- 未 `AssetDatabase.Refresh`（无编译动作）、未改任何代码或断言、未改 34 项 / 字体 / 存档 / run / 证据日志。
- 唯一写入是本文件（`Development/StudioRuns/R3B-UI-20261025/management-eleven-preflight.md`，不在 34 项基线内）。
