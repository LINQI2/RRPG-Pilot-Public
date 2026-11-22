# R3-B pending 事件关闭速度恢复契约核对与单例复验（夹具时间线最小修复）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 日期：**2026-10-25**（Asia/Shanghai；UTC+8）。
- 阶段：**1/1**，仅核对“事件关闭后速度恢复”契约，并做单例复验所需的**夹具时间线最小修复**；不重做整包。
- 结论状态：**根因已定位（夹具时间线，不是产品丢失玩家 Pause）；修复已写入，编译/发现已实核；单例 PlayMode 由宿主执行，本执行层未运行、不预填结果。**
- 边界：未 `rql-mutate`；未 Shell / `Process.Start`；未 Git 写；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`（编译仅 `AssetDatabase.Refresh`）；未改产品源码 / 公共 API / DTO / WorkClock / 规则 / Scene / Prefab / 字体 / Packages / ProjectSettings / 基线 / 存档本体 / `SaveSlotGuard`；未改 34 项基线文件与备份历史 run；未运行 `tests-run`、未忙等测试；未创建额外付费子 Agent。

---

## 1. 开工前与改后只读实核（当前连接，非历史报告）

| 项 | 改前 | 改后 | 证据 |
|---|---|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 同（刷新编译结束后再核一次 `isCompiling=false`） | `rql-query editor` |
| 场景 | 仅 `SampleScene`，`isDirty=false`、`isLoaded=true`、`rootCount=5` | 同 | `rql-query scenes` |
| TestRunner | `testResults` **仅 1 条**（即 88d9273b 的单例败例），无 running / pending / 未知投递；`tests` 中目标方法**全局唯一 1 条 PlayMode** | 刷新后唯一性复核仍为 1 | `rql-query testResults` / `tests` |
| 34 项 workspace baseline | **34/34 same、0 diff、0 missing**（逐项 bytes + SHA256） | 同（改后复测） | `script-execute` 只读哈希 |
| 受保护 TMP 字体 | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 同 | 同上 |
| 单槽存档 | **13405B / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`** | 同 | 只读 `SaveSystem.GetSavePath()` + SHA256 |
| SaveSlotGuard 全部 run | **149**：`restored=148` + `protective-snapshot=1`；`other=0`、`noManifest=0` | 同 | 只读遍历 149 个 `manifest.json` |
| 最新 run | `20261025T072847Z`（= 88d9273b 单例运行产生）`status=restored`、`backupBytes=13405`、`backupSha256=4b2b56f6…d6aa`、`readbackVerified=true`、单槽回读与 backup 同 hash | 同 | 该 run manifest |

> 无未完成 / 未知 run（`other=0`、`noManifest=0`），本阶段未自动恢复任何档，历史 run 与备份未改动。

---

## 2. 宿主失败回执（88d9273b）完整文本

```
1 total / 0 passed / 1 failed / 0 skipped
RRPG.ManagementShell.Tests.ManagementShellPlayTests.PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice
Failed  durationMs=1222
  事件关闭后必须保持玩家 Pause
  String lengths are both 6. Strings differ at index 0.
  Expected: "Paused"
  But was:  "Normal"
  -----------^
```

- `testResults` 对该例只记录上述断言消息（无额外堆栈帧回传）；失败点唯一定位到用例结束前的 `ClockSpeed()` 断言（现第 4297 行，失败时为 4267 行）。
- Farm A 前置（上一包 `RefreshNow` 修复）已越过：本次失败前无 Farm A 断言失败，符合上一包结论。

---

## 3. 真实调用链与动作时间线（逐速度步骤，源码级）

### 3.1 相关产品链（未改动，只读核对）

- 时间权威：`VillageTime.SetSpeed`（`VillageTime.cs:98-104`）——`WaitingForInteraction` 时**允许**改速但不推进；`Pause()` / `Resume()` 都只是 `SetSpeed`。
- 事件模态：`VillageSocialEventUI`：
  - `ShowEventPending`（`VillageSocialEventUI.cs:253-301`）在 `PauseOnOpen && ShouldPauseOnOpen` 时调用 `PauseForOpen()`。
  - `PauseForOpen`（`:528-540`）**记录 `_speedBeforePause = time.Speed`**，若已 Paused 则不重复 Pause；`_pausedByOpen=true` 后再次打开早退（`:530`）。
  - `OnCloseClicked`（`:493-504`）→ `RestoreAfterClose`（`:543-551`）→ `time.SetSpeed(_speedBeforePause)`；`SetVisible(false)` 也会兜底调用一次（幂等）。
  - 因此产品契约是：**关闭时恢复“事件打开那一刻”的 Speed**（组件注释 `:511`、`:524-527` 明示“含打开前本就是 Paused，不误恢复”）。
- 首次 Present 的触发点：`VillageTimeDriver.Update`（`VillageTimeDriver.cs:21-24`）在 `HasActiveAdvance` 时调用 `PumpWorldAdvance` → `PresentAdvanceInteraction`（`VillageRuntimeBridge.Intraday.cs:113-120`）→ `ShowAdvancePending` → `ShowEventPending`。**它只在真实帧运行**。
- 事件配置：`EventPresentation.json` 中 `evt_test_village_temp_draft` = `PauseOnOpen=true` / `Urgency=Timely`（`Development/Generated/StaticConfig/EventPresentation.json:86-92`）。

### 3.2 失败运行时序（改前夹具）

| 步骤 | 用例动作 | 真实 Speed | Session / Clock |
|---|---|---|---|
| 4110 | 夹具断言从玩家 Pause 开始（`EnterTownThroughProductEntry` 已 `Pause`） | `Paused` | 无 AdvanceSession，0 分钟 |
| 4160 | 第 3 次真实临时承诺 → 产品 Recognition 建立 `PendingVillageEvent`（**同步，无帧**） | `Paused` | 仍无 Session |
| 4170 | `AttemptDirectAdvance(+60)` → `WaitingForInteraction`（**不触发 Present**） | `Paused` | Session=Waiting/Direct，Clock 不前进 |
| 4188–4212 | 管理导航 / 保存 / 撤职 / ESC（同步） | `Paused` | Waiting 不变 |
| **4214** | 尝试“手动 Resume 门禁”：`_shell.TogglePause()`（Paused→Resume） | **`Normal`** | Waiting 不变 |
| **4215–4217** | `yield return null` ×3；`VillageTimeDriver.Update` 首次 `PumpWorldAdvance` → `PresentAdvanceInteraction` → `ShowEventPending` → **`PauseForOpen` 记录 `_speedBeforePause = Normal`**（并未真 Pause，因为此刻已是 Normal） | `Normal` | Waiting，Clock 仍 0 前进 |
| 4226 | 夹具 `Clock.Pause()`（采样回到玩家 Pause） | `Paused` | Waiting 不变 |
| 4252 | 再次 `PresentAdvanceInteraction` → `PauseForOpen` 因 `_pausedByOpen=true` 早退，**采样值仍是 Normal** | `Paused` | Waiting 不变 |
| 4255 | `OnEventOptionClicked`（不改速） | `Paused` | Pending 清除，仍 Waiting |
| 4261 | `OnCloseClicked` → `RestoreAfterClose` → `SetSpeed(_speedBeforePause=Normal)` → 随后 acknowledge=Yielded | **`Normal`** | Yielded |
| 4264 | `PumpWorldAdvance` → `ResumeAdvance` 完成到 `directTarget`，Session 释放（不改速） | `Normal` | 完成，无 Session |
| 4267 | 断言 `Paused`，实际 `Normal` → **唯一失败点** | `Normal` | — |

**关键事实**：改前夹具里“事件打开时刻”被真实帧推到了 `TogglePause()`（Resume→Normal）之后；产品忠实地按“打开前 Speed”恢复 `Normal`，与“玩家最后合法速度”在这一时间线下本来就是 `Normal`。

---

## 4. 根因判定（Normal 归属）与 Authority

1. **Normal 是“既有恢复正常契约”的结果，不是产品丢失玩家 Pause。** 失败时间线里玩家显式 Pause 发生在事件打开**之后**（4226），而 `RestoreAfterClose` 的语义是恢复**打开前** Speed（组件注释与 `EventPresentation.PauseOnOpen` 一致；Village GameSpec §5.5 只规定“可暂停”，恢复语义由既有实现承载）。
2. **夹具缺陷**：旧夹具在事件首次 Present 之前先 `TogglePause()`，把“打开前速度”留在了 Resume 之后的 Normal；随后又期望关闭后为 Paused——这是**把恢复采样点当作可被之后 Pause 覆盖**的错误假设，属操作用例时间线问题。
3. **不是 Farm A / pending / Advance / Return / WorldBusy 链问题**：失败前所有门禁断言（Waiting、Clock 不前进、`CanSaveWorld=false`、`WorldBusy` 零写、Pending 仍在）均已通过；失败只发生在关闭后速度。
4. 因此**不采用“把 expected 改成 Normal”** 求绿（那会把“打开前 Paused 必须恢复 Paused”的契约一起删掉）；按既有契约验证，并让夹具把“事件打开”固定在玩家 Pause 时刻。

---

## 5. 唯一代码改动（夹具时间线，不削弱断言）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（方法 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`）

- 在“停止交互 / ESC 不得解锁”之后、`TogglePause` 之前，**新增一次既有公开入口 `VillageRuntimeBridge.PresentAdvanceInteraction()`**，使事件打开时刻确定在玩家 Pause 上；并断言：
  - 打开前 Speed 必须是 `Paused`（`speedBeforeEventOpen`）；
  - Present 后既有事件 UI `root.activeSelf == true`，Session 仍 `WaitingForInteraction`；
  - `PauseOnOpen` 对“打开前已 Paused”**不得重复暂停 / 改速**（Speed 仍等于 `speedBeforeEventOpen`）。
- `TogglePause` 后**新增** `Speed == Normal` 断言（证明 Resume 真实生效、门禁不靠“顶部控制不可达”），跨 3 个真实帧后再断言 Speed 仍为 `Normal`、UI 仍打开；
- 保持原有强断言不变：Clock 不前进、Session 仍 Waiting、`CanSaveWorld=false`、`PendingVillageEvent` 仍存在、随后 `Clock.Pause()` 回到玩家 Pause；
- 第二次 Present 处新增：UI 仍打开、Speed 仍等于 `speedBeforeEventOpen`（关闭恢复目标未被重采样改写）；
- 关闭后保留 `Paused` 断言，并**新增独立断言** `ClockSpeed() == speedBeforeEventOpen`（“恢复玩家最后合法速度”，而不是改成 Normal）；
- 新增两条 `[EVENT-GATE] SPEED ...` 证据行（打开 / Resume / 玩家 Pause 的 Speed 与 Clock、Session 状态）。

未改：其余 10 例、Farm A 的公开 `RefreshNow` 读取同步修复（仍在位）、SaveSlotGuard、NewGame / Shell / schedule 恢复、全部门禁与“只结算一次”强断言。

文件实测：**292352B / `e2a98f0410cb8c1fe3139128d477605e5ba3b54695901c1c162b74c2fc28a47a`**（改前 289629B / `75f8c00d…`）。

---

## 6. 编译与发现核验（本执行层实际执行）

- 编译入口仅 `script-execute` → `UnityEditor.AssetDatabase.Refresh()`；刷新期间 `isCompiling=True`，结束后 `isCompiling=false`（`rql-query editor`）。
- 项目 Console 无非本次 snippet 自身的新增 `error CS`（本次刷新后 `logType ∈ {Error, Exception}` 且 `sequence>52440` 的查询结果为 0 条）。
- DLL 新鲜：`Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` mtime **`2026-10-25T07:32:01.5054528Z`** > 源文件 mtime `2026-10-25T07:31:40.7663579Z`。
- 新断言确实进入程序集：DLL 内 UTF-16 字面量实测存在 `事件打开前必须是玩家显式 Pause`、`手动 Resume 必须真实改速`、`事件跨真实帧不得改写玩家手动选择的 Normal`、`关闭必须恢复事件打开前记录的速度`、`事件关闭后必须保持玩家 Pause` = **True**。
- 反射实核：类内 `[UnityTest]` 方法 **11** 个；目标方法存在（`RRPG.ManagementShell.Tests.ManagementShellPlayTests.PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`）。
- 发现唯一性：`tests | where fullName contains "PendingEvent_BlocksAdvanceAndReturn"` = **1**，模式 `PlayMode`。

---

## 7. 保护与未改文件（实测哈希）

| 文件 | 实测 | 说明 |
|---|---|---|
| `Assets/Scripts/Village/UI/VillageSocialEventUI.cs` | 28380B / `54bdb9fae49b679732fc01435200c0ab7a56d0f58ae7f4cd39ca664004382c48` | 未改（PauseOnOpen / RestoreAfterClose 契约原样） |
| `Assets/Scripts/Village/Runtime/VillageTime.cs` | 17082B / `8563bd6d8ce8a533075dfff598cc07fa259311aaaa7744ec8e3b89196804723d` | 未改（Clock 核心未动） |
| `Assets/Scripts/Village/VillageRuntimeBridge.Intraday.cs` | 10394B / `e0abf3d79e138b37cb1a0eed401f740132aa68003e658181aed936bfaf5104cb` | 未改（PresentAdvanceInteraction 原样） |
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | 16082B / `7da8fbb06aac044e37acbd21377ff7e32f4aa1b99659e4f18bdf66bbbd8f2f66` | 未改（与固定快照一致） |
| 受保护 TMP 字体 | 14961033B / `3c758e16…c404854` | 未改 |
| 34 项 baseline | 34/34 same | 未改 |

---

## 8. 未测（交宿主）

- 本执行层**没有运行** `tests-run`，也没有启动 / 忙等 TestRunner，不声称该用例通过。
- 宿主请仅以**完整 testMethod、单例、PlayMode 串行**执行：
  `RRPG.ManagementShell.Tests.ManagementShellPlayTests.PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`
- 若通过，只能登记该单例回执；本文件不预填整类 / 整包结论（其余 10 例与完整回归仍按各自历史事实，不并入）。
- 未实测：真实 UI 下“事件模态打开期间顶部时间控制是否仍可点”（若可点，`RestoreAfterClose` 会按“打开前 Speed”覆盖模态内的手动改速——这是既有契约的字面行为，本阶段**未改**产品、未新增规则；如制作人要求“模态内手动改速优先”，需另立产品包裁决）。

## 9. 与任务要求的差异

- 任务描述 Option 为“夹具自己 Pause→Resume 已选择 Normal，却之后期待 Paused”：实测时间线为 **Pause→（先 TogglePause→Normal）→真实帧内事件才首次打开→再 Pause→关闭恢复 Normal**。即“最后合法速度”与“打开前速度”在旧夹具里被 Resume 拆开；本阶段按“事件规则明确恢复打开前 Speed”的契约修正打开时刻，而不是改期望值。
- 未做产品修复：证据不支持“产品忽略打开前的明确手动 Pause”；产品对打开前 Pause 的恢复符合契约，无需改 Clock 核心 / 公共 API / DTO / 新规则。
