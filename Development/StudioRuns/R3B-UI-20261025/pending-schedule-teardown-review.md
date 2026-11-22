# R3-B 单例清理链修复：TearDown 活动日程“同一实例”恢复顺序核对

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 日期：**2026-10-25**（Asia/Shanghai；UTC+8）。
- 阶段：**1/1**，只核对并修复活动日程恢复清理链，使单例可被宿主复验；不重做整包、不预填通过。
- 结论状态：**根因已定位（TearDown 恢复顺序 × `GuardWorldWrite`，不是复制/默认化，也不是用例中段重建）；测试夹具修复已写入并完成安全编译/发现实核；单例 PlayMode 仍由宿主执行，本执行层未运行、不声称通过。**
- 边界：未 `rql-mutate`；未 Shell / `Process.Start`；未 Git 写；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`（编译仅 `AssetDatabase.Refresh`）；未改产品源码 / Clock / 活动规则 / API / DTO / Scene / Prefab / 字体 / Packages / ProjectSettings / 34 项基线 / 存档本体 / `SaveSlotGuard`；未改备份历史 run；未跑 `tests-run`、未忙等测试；未创建额外付费子 Agent。

---

## 1. 开工前只读实核（当前连接）

| 项 | 实测 | 证据 |
|---|---|---|
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | `rql-query editor`（改前 / 两次安全编译后各一次） |
| 场景 | 仅 `SampleScene`，`isDirty=false`、`isLoaded=true`、`rootCount=5` | `rql-query scenes` |
| TestRunner | `testResults` **仅 1 条**（即失败单例），无 running / pending / 未知投递；`tests` 中目标方法全局唯一 1 条 `PlayMode` | `rql-query testResults` / `tests` |
| 34 项 workspace baseline | **34/34 same、0 diff、0 missing** | `script-execute` 只读 bytes+SHA256（改前、改后各一次） |
| 受保护 TMP 字体 | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 同上 |
| 单槽存档 | **13405B / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`** | 只读 `SaveSystem.GetSavePath()` + SHA256 |
| SaveSlotGuard 全部 run | **150**：`restored=149` + `protective-snapshot=1`；`other=0`、`noManifest=0`（无 captured/restore-failed 未知状态） | 只读遍历 150 个 `manifest.json` |
| 最新 run | `20261025T073336Z`：`status=restored`、`sourceBytes=backupBytes=13405`、`backupSha256=4b2b56f6…d6aa`、`readbackVerified=true`、`lastError=null` | 该 run manifest |

> 未完成 / 未知 run 为 0，因此无需（也**未**）自动恢复任何档；历史 run 与备份未改动。

### 1.1 失败单例的真实回执与 TearDown 栈（宿主原样）

```
testResults（全局仅此 1 条）:
Failed  "TearDown :   活动日程必须恢复为用例前捕获的同一实例\r\n  Expected: True\r\n  But was:  False\r\n"
stackTrace:
  at RRPG.ManagementShell.Tests.ManagementShellPlayTests.RestoreActivityScheduleAfterFixture ()
      .../ManagementShellPlayTests.cs:449          ← 修复前源码的 ReferenceEquals 断言行
  at RRPG.ManagementShell.Tests.ManagementShellPlayTests.TearDown ()
      .../ManagementShellPlayTests.cs:1184         ← 修复前 TearDown 先行恢复处
```

- **用例主体没有失败消息**，且该 run 的日志尾部有 `[EVENT-GATE] END return catchup once clock=73500 phase=CatchUpComplete days=1 history=2 … repeat=AlreadyCaughtUp projection=consistent`（sequence 53529），说明主体跑到方法最后一行后进入 TearDown 才失败。
- 归档的 `CASE-B-SCHEDULE`（sequence 53517）：`captured=IntradayActivitySchedule original-points=[420,1020,1200] original-durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90} commute=15 repeat=30 isolated-points=[]`。
- 该 run **没有** `SCHEDULE-RESTORE` 日志（断言在打证据前抛出）；同一 run 的 `SAVE-RESTORE`（sequence 53530）却已执行：`matches-before=True`、`sha256=4b2b56f6…d6aa`。

### 1.2 “TearDown 异常是否仍执行 finally 存档恢复”实核结论

**是，已真实发生且未被短路。** 失败 run 的 manifest 为 `restored`、单槽回读与备份同 hash；console 53530 证明 `SaveSlotGuard.Restore()` 在活动日程断言抛出之后仍由 `finally` 执行成功。本次顺序调整继续把 `SaveSlotGuard.Restore()` 放在活动日程恢复之前，并把活动日程核对放在**最外层 finally**，因此：

- 存档恢复仍先于日程核对执行，日程断言失败**不可能**短路存档恢复；
- `ForceSaveFailureForTests` 清理、`SaveSlotGuard.Restore()`、`ResetForTests` 任一抛异常，最外层 `finally` 仍执行日程核对；
- 恢复失败不写“已恢复”证据、不吞异常（见 §4）。

---

## 2. 根因（源码级，含失败 run 的直接证据）

### 2.1 失败点不是“复制/默认化”，而是被产品写守卫拒绝的空操作

- 既有公开注入点：`VillageRuntimeBridge.Activities.cs:7-13`
  ```csharp
  public static IntradayActivitySchedule ActivitySchedule { get; private set; } = new IntradayActivitySchedule();
  public static void ConfigureActivitySchedule(IntradayActivitySchedule schedule)
  {
      if(!GuardWorldWrite()) return;          // ← 世界写锁有效时静默拒绝
      ActivitySchedule=schedule ?? new IntradayActivitySchedule();   // ← 接受时是直接赋同一引用
  }
  ```
  即：**接受路径不复制、不重建、不默认化**（同一引用）；**拒绝路径直接 `return`**。
- 守卫：`VillageRuntimeBridge.Intraday.cs:31-43`
  ```csharp
  public static bool IsWorldWriteLocked => (Time != null && Time.HasActiveAdvance)
      || (_returnRecord != null && !_returnRecord.Completed);
  public static bool GuardWorldWrite()
  {
      if (!IsWorldWriteLocked || _executingBlockingOption) return true;
      ...
      return false;
  }
  ```
- 失败用例的末段事实（源码 4391 附近，now 行号偏移后仍在原位）：Return Catch-up 已完成（`Phase=CatchUpComplete`）但**最终 Save 之前不得解锁**，用例显式断言 `CanSaveWorld == false` 后结束。故 TearDown 进入时 `_returnRecord != null && !Completed` → `IsWorldWriteLocked == true` → `ConfigureActivitySchedule(before)` **被契约拒绝**。
- 于是 `restored` 仍是隔离实例 `[]`，`ReferenceEquals(restored, before)` 为 False —— 与宿主回执的“Expected True / But was False”逐字吻合；且此时绝不会出现“复制成新默认实例”的另一种原因（那会得到值相等但引用不等；而这里的 `restored` 仍是本次隔离实例）。

### 2.2 旧顺序本身还有第二重错误

旧 `TearDown` 先 `RestoreActivityScheduleAfterFixture()`、后 `ResetForTests()`。即使当时没有写锁、恢复成功，`ResetForTests → ResetIntradayHost`（`VillageRuntimeBridge.Intraday.cs:142`）也会 `ActivitySchedule=new IntradayActivitySchedule()`，把刚恢复的同一实例再次替换。也就是说旧顺序对“保留用例前实例”这一目标在结构上就不可能成立。

### 2.3 排除“用例中段 Authority 重置 / Load / NewGame 造成重建”

- `ResetIntradayHost()` 的全部调用方只有：`ResetTimeForNewGame()`（`Intraday.cs:28`）、`ResetForTests()`（`VillageRuntimeBridge.cs:1384`）；`ResetTimeForNewGame` 的唯一产品调用是 `SaveSystem.NewGame()`（`SaveSystem.cs:234`）。
- 该用例主体**没有** `VillageRuntimeBridge.ResetForTests`、`SaveSystem.Load`、`SaveSystem.NewGame`；只调用了 `EventRuntimeBridge.Restore(null)`（仅事件区块，不触碰活动日程）与既有 Advance/Return/UI/管理入口。
- 因此用例期间活动日程始终是 `IsolateActivityOpportunitiesForFixture` 写入的隔离实例；直到 TearDown 失败时也没有被“重建”，失败发生在**恢复写入被守卫拒绝**。

**根因判定：恢复顺序错误（在未释放世界写锁时调用带守卫的公开写入口），不是配置入口复制/默认化，也不是用例中段 Authority 重置。**

---

## 3. 唯一文件改动（仅测试夹具，`Assets/Tests/PlayMode/ManagementShellPlayTests.cs`）

### 3.1 `TearDown` 恢复顺序（源码 1193-1224）

```csharp
[TearDown]
public void TearDown()
{
    try
    {
        try { /* 清 ForceSaveFailureForTests */ }
        finally
        {
            try { if (_saveGuard != null) _saveGuard.Restore(); }
            finally
            {
                try { Static("VillageRuntimeBridge", "ResetForTests"); }
                catch (Exception ex) { Debug.LogWarning(...); }
            }
        }
    }
    finally
    {
        // ResetForTests 释放世界写锁（Time=null、_returnRecord=null）之后才恢复日程；
        // 放最外层 finally：存档兜底恢复 / Reset 抛异常也必须核对，且日程失败不短路存档恢复。
        RestoreActivityScheduleAfterFixture();
    }
}
```

恢复条件因此是明确的：`ConfigureActivitySchedule` 需要 `GuardWorldWrite()==true`，即 `IsWorldWriteLocked==false`；`ResetForTests()` 置 `Time=null`、`_returnRecord=null`、`_expeditionPersonIds.Clear()`，使 `IsWorldWriteLocked==false`，写入必然被接受。

### 3.2 `RestoreActivityScheduleAfterFixture` 校验（源码 431-471）

- 先按需调用既有公开入口 `ConfigureActivitySchedule(before)`，再**独立**读取当前值校验：
  1. `ReferenceEquals(restored, before)`：证明“守卫写入口真的接受并赋了同一引用”（失败消息附带 `IsWorldWriteLocked`，便于区分锁未释放）；
  2. `OpportunityMinutes` 逐项相等；
  3. `CommuteMinutes` 相等；
  4. `RepeatOpportunityDelay` 相等；
  5. 五项 `activityDurations`（Rest/Drink/Talk/VisitSick/Help）完整相等；
- **只有同一实例 + 全部值通过后才清空 `_activityScheduleBefore`** 并写 `SCHEDULE-RESTORE`；任一项失败则保留捕获引用并抛错。下一个用例的 `IsolateActivityOpportunitiesForFixture` 会因 `_activityScheduleBefore != null` 以“同一用例不得重复捕获活动日程（上次未恢复会掩盖泄漏）”直接失败——这是“captured / restore 未确定”的停止信号，不自动覆盖、不清记录。
- 不反射写私有状态：只读 `ActivitySchedule` 公开 getter 与 `IntradayActivitySchedule` 的公开成员，写操作只走既有公开注入点。

### 3.3 为什么“引用身份”是契约而不是巧合

`ConfigureActivitySchedule` 的既有实现是 `ActivitySchedule = schedule`（直接赋值），没有拷贝构造、没有从值重建；因此“传进去的实例就是 getter 返回的实例”是该公开入口**当前契约本身**的一部分。`ReferenceEquals` 不是对私有实现的白盒假设，而是检查“带 `GuardWorldWrite` 的写入口是否真正生效”（旧失败正是它被拒绝）。同时保留 §3.2 的 5 组值比对：即使将来产品改成“值等价但不保身份”，这些断言仍能独立证明机会点/通勤/重复延迟/时长没有漂移，且失败消息会直接指出身份契约变化，而不是靠删断言求绿。

### 3.4 非测试默认日程未受影响（实核 + 源码）

- 生产默认机会点/时长只在 `IntradayActivitySchedule` 构造器（`IntradayActivitySchedule.cs:15,19-21`）：`420/1020/1200`、`commute=15`、`repeat=30`、`Rest/Drink/Talk=60`、`VisitSick/Help=90` —— 本阶段未改（文件不在改动列表）。
- 全仓 `ConfigureActivitySchedule` 的调用方只有测试文件（生产代码无调用点），因此生产默认日程不会被测试期注入污染；测试恢复的 `before` 本身也是 `ResetForTests → new IntradayActivitySchedule()` 的产品默认实例（失败 run 的 `CASE-B-SCHEDULE` 证据：`original-points=[420,1020,1200] … commute=15 repeat=30`），恢复它不引入任何非默认值。
- 恢复发生在 `ResetForTests` 之后、下一用例 `[UnitySetUp]`（再次 `ResetForTests`）之前；下一用例仍从产品默认日程开始，其余 10 例继续跑真实活动机会（未全局禁用）。

### 3.5 未削弱的既有断言 / 未改的产品契约

- 事件用例全部 pending 业务断言、`PauseOnOpen` / `RestoreAfterClose`“恢复打开前 Speed”断言、`ClockSpeed()==speedBeforeEventOpen`、`CanSaveWorld` 门禁、零写盘、单次结算 / `AlreadyCaughtUp`、管理投影一致性等**原样保留**（DLL 字面量实核存在：`关闭必须恢复事件打开前记录的速度`、`事件关闭后必须保持玩家 Pause`）。
- Farm A 的 `RefreshNow` 读取同步修复仍在位。
- 其余 10 例用例代码未改；`OneTimeTearDown` 的 `_lastGuard` 兜底、34 项 baseline、字体校验未改。
- 未改产品 Clock / 活动规则 / 公共 API / DTO / `SaveSlotGuard` / Scene / Prefab / 存档。

---

## 4. 编译与发现核验（本执行层实际执行）

- 编译入口仅 `script-execute` → `UnityEditor.AssetDatabase.Refresh()`（两次：代码改动后、注释补充后）；结束后 `rql-query editor` 均为 `isCompiling=false`、`isUpdating=false`、`isPlaying=false`。
- 项目 Console 无非本次 snippet 自身的新增 `error CS`：两次刷新后 `logType ∈ {Error,Exception}` 且 `sequence > 上次基线` 的查询均为 0 条（仅诊断期间我自己写错 `using UnityEngine` 的 snippet 编译错误，属工具自身记录，已排除）。
- DLL 新鲜：`Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` mtime **`2026-10-25T07:37:20.1539053Z`** > 源文件 mtime **`2026-10-25T07:37:17.3737039Z`**。
- 新语义确实进入程序集：DLL 内 UTF-16 字面量实测 `活动日程必须恢复为用例前捕获的同一实例（既有 ConfigureActivitySchedule 直接赋值同一引用；`=True、`当前 IsWorldWriteLocked=`=True、`活动日程重复机会延迟必须恢复到用例前原值`=True、`活动日程五项活动时长必须恢复到用例前原值`=True。
- 反射实核：类内 `[UnityTest]` 方法 **11** 个；目标方法与 `TearDown` / `RestoreActivityScheduleAfterFixture` 均存在。
- 发现唯一性：`tests | where fullName contains "PendingEvent_BlocksAdvanceAndReturn"` = **1**，模式 `PlayMode`。

### 4.1 改后文件与未改文件（实测）

| 文件 | 实测 | 说明 |
|---|---|---|
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **294535B / `cc55cb405216f43cb4b090d068f43446d4ccbfecf130c5274f4791388f80715a`** | 唯一改动（TearDown 顺序 + 恢复校验/文档注释；改前 292352B / `e2a98f04…`） |
| `Assets/Scripts/Village/VillageRuntimeBridge.Activities.cs` | 10593B / `0438670637d7488c02c65fee2422cd228d14157e120b286915c2916b72c20418` | 未改（`ConfigureActivitySchedule` 守卫语义原样） |
| `Assets/Scripts/Village/VillageRuntimeBridge.Intraday.cs` | 10394B / `e0abf3d79e138b37cb1a0eed401f740132aa68003e658181aed936bfaf5104cb` | 未改（`GuardWorldWrite` / `ResetIntradayHost` 原样） |
| `Assets/Scripts/Village/UI/VillageSocialEventUI.cs` | 28380B / `54bdb9fa…382c48` | 未改（Pause/恢复打开前速度契约原样） |
| `Assets/Scripts/Village/Runtime/VillageTime.cs` | 17082B / `8563bd6d…04723d` | 未改（Clock 核心未动） |
| 受保护 TMP 字体 | 14961033B / `3c758e16…c404854` | 未改 |
| 34 项 baseline | 34/34 same | 未改 |
| 单槽存档 | 13405B / `4b2b56f6…d6aa` | 未改 |

---

## 5. 存档保护当前状态

- 150 个 run 全部为终态：`restored=149` + `protective-snapshot=1`；`other=0`、`noManifest=0`，无 `captured` / `restore-failed` 未知状态。
- 最新 run `20261025T073336Z` 为 `restored`，备份与单槽回读均 `13405B / 4b2b56f6…d6aa`；本阶段未新增 run（未跑测试），未自动恢复、未改动任何历史 run/备份。
- 若后续宿主运行出现 `restore-failed` 或 `captured`：按现有守卫会阻止新测试；恢复条件是“仅用该 runID 的 `original-save.bin`，回读 SHA256 与 manifest `backupSha256` 一致后才允许写回”，否则停止并人工取证（本阶段未触发）。

---

## 6. 未测（交宿主）

- 本执行层**没有运行** `tests-run`，没有启动 / 忙等 TestRunner，**不声称该单例通过**，也不预填整类 / 整包结论。
- 宿主请以**完整 testMethod、单例、PlayMode 串行**执行：
  `RRPG.ManagementShell.Tests.ManagementShellPlayTests.PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`
- 预期观察点（通过时）：日志出现 `SCHEDULE-RESTORE restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}`，且用例主体既有 `EVENT-GATE` / 门禁断言全部保持；`SaveSlotGuard` manifest 仍为 `restored`。若仍失败，TearDown 消息会带上 `IsWorldWriteLocked=` 实际值，可直接区分“锁未释放”与“身份契约变化”，不需要重跑猜测。
- 未实测：真实 UI 下“事件模态打开期间顶部时间控制是否仍可点”（既有契约字面行为，本阶段未改产品；如制作人要求模态内手动改速优先，需另立产品包裁决）。

## 7. 与任务要求的差异 / 停止条件

- 未发现产品契约越界：`GuardWorldWrite` 拒绝写是既有产品规则，本次**只调整测试夹具的恢复时机**（先释放锁再恢复），没有放宽守卫、没有改产品 API/规则。
- 未采用“删 `ReferenceEquals` 断言 / 改期望值”求绿；引用身份与 5 组值比对同时保留。
- 未发现需要停止写入的产品契约冲突；其余待制作人裁决项（模态内手动改速优先）仍留原状。
