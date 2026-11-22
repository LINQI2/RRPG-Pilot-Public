# R3-B Pilot｜合法回村后管理页真实导航复核与最窄修复

生成时间：2026-10-24（本机）
阶段：1/1（核对回村后管理页真实导航）
状态：**仅修改测试夹具的时序与证据；未改产品代码。安全编译通过；PlayMode 未由本执行层启动，等待宿主以短类名 `ManagementShellPlayTests` 整类串行执行。本文件不声称用例已通过。**

---

## 1. 本轮先决状态（只读核对）

| 项目 | 实际值 / 证据 |
|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` |
| 场景干净 | 唯一打开场景 `Assets/Scenes/SampleScene.scene`，`isDirty=false`、`rootCount=5` |
| Console | 编译后只读查询 `logType == Error/Exception`：0 条 |
| 34 项基线 | 本层只读重算 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`：`total=34 same=34 missing=0 problems=0 all-match` |
| 受保护字体 | `RRPG_SourceHanSansCN_Regular_TMP.asset` 实测 `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致） |
| Save 保护历史档 | `R3B-SaveProtection-20261024/*/manifest.json` 最新一轮（`20261024T111850Z`～`20261024T111857Z`）全部 `status=restored`、`restored=true`、`readbackVerified=true`；无未完成 run |
| 测试发现 | `fullName contains "ManagementShellPlayTests"` 共 **9** 例，`mode=PlayMode`、`assembly=BattleStage1.PlayModeTests`；原 8 例名称不变 |
| 上轮结果 | 9 run：**8 Passed / 1 Failed / 0 Skipped**，唯一失败为本用例 |

---

## 2. 失败原始证据（不推断 Return 未执行）

### 2.1 宿主给出的失败行与消息

- 断言：`ManagementShellPlayTests.cs`（修复前行号 **2833**；插入证据后当前行号 **2886**）
  `Assert.IsTrue((bool)Get(management, "IsVisible"), "合法回村后真实管理页必须可见");`
- 结果：`Expected: True But was: False`。
- **原始完整栈文本本执行层未持有**（宿主 Test Runner 的 `testResults.stackTrace` 未落入仓库；本轮遵守“测试仅宿主 check”，不启动 Test Runner，因此不伪造栈文本）。下面 2.2 给出证据日志的实际断点，2.3 给出源码级真实调用链，两者可互相复核。

### 2.2 证据日志的实际断点

`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`（`2026-10-24T11:18:52Z`）：

1. `[EXP-RETURN] confirm-preflight … isReady=True affordable=True … button=True`
2. `[EXP-RETURN] departure clock=14400 … shellVisible=false simulationActive=false canSave=false`
3. `[EXP-RETURN] pending clock=14400 phase=Active … commands=PersonNotInVillage/WorldBusy save=WorldBusy zero-write=true shellVisible=false surfaceOpenBlocked=true manualPauseNoUnlock=true`
4. `[EXP-RETURN] returned clock=15840 days=1 ingredients=0 food=96 batches=0 evidence=0 history=0 duty=workplace:building_instance_1:main occupancy=1 project=… progress=1 farmB=UnderConstruction save=8c8c62ff…`
5. 下一行即 `[SAVE-RESTORE] runId=20261024T111851Z … matches-before=True`（TearDown 恢复），随后进入下一用例。

结论：Return 的 Settlement / Catch-up / Carry / Consequence / Clear / Final Save 与 `Load` 断言段均真实跑过并留下了 `returned` 行；用例是在第 5 段“真实管理导航后可见性”处中止的，**没有** `END legal return unlocked` 行。因此不是“Return 未跑”，也不是确认按钮/补给/WorldBusy/Return pending 门禁问题。

### 2.3 失败前的对象与状态（源码级确证，非运行时伪造）

| 检查项 | 真实结论 |
|---|---|
| `SaveSystem.Load` 是否替换世界 | 否。`LoadCore`（`SaveSystem.cs:118-216`）把存档恢复到既有 `VillageRuntimeBridge` 各 Authority，并 `return` 一个**新的 `PlayerCollection` 实例**；不重建场景、不重建 `ProductHomeWorldHost` / `HomeProductShell` / `ManagementView`。 |
| 测试是否拿到旧实例 | 否。`_host` / `_shell` / `management` 仍是同一个场景实例（Shell 由 `ProductHomeWorldHost.Awake` 创建一次）；Load 后 `management = Management()` 仍是同一 authored Prefab 实例。 |
| Canvas / Shell 是否被 Load 关掉 | 否。Load 不改 `IsSimulationActive`；`ShellVisible => _canvas!=null && _canvas.enabled`，`_canvas.enabled` 由 `HomeProductShell.LateUpdate` 按 `_host.WorldVisible && IsSimulationActive` 维持；`WorldVisible` 仅依赖 `IsSimulationActive && SimulationPersonSource!=null && Time!=null`，Load 后三者均成立。 |
| Router.Surface 是否被 Load 改动 | 否，仍为 `World`（用例在 Load 前已断言，Load 不触碰 `HomeProductRouter`）。 |
| 是否已有模态 / Growth pending | 否。管理页只在 `Router.Surface==Management` 时显示；`SurfaceModalActive` 依赖 Person/Management/Growth 三态，此时三者均未打开；`CharacterGrowthUI` 的 `_pendingNode` 只在显式选牌流程中被置位（`CharacterGrowthUI.cs:534`），Return/Load 不打开成长页。 |
| 导航命中与 OnClick | 命中真实 `ProductNavigation/Navigate2`（Raycast top hit + `ExecuteEvents.pointerClickHandler` 均通过，否则失败消息会是“top hit 不是目标/无 handler”）。 |

---

## 3. 确切根因：真实点击被 Shell 的“来源切换复位”吞掉

### 3.1 产品代码事实

`UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeProductShell.cs`：

- `RefreshNow()`（第 107-138 行）在**世界来源**变化时先做会话复位：

```csharp
if(!ReferenceEquals(_source,VillageRuntimeBridge.SimulationPersonSource))
{ _source=VillageRuntimeBridge.SimulationPersonSource;Feed.ResetSession();Router.Close(); … }
…
bool show=Router.Surface==HomeSurface.Management && ShellVisible;
if(Management.gameObject.activeSelf!=show) Management.gameObject.SetActive(show);
```

- `OpenSurface(HomeSurface surface)`（第 139-144 行）先打开路由，再在同一次调用内刷新：

```csharp
Build?.Cancel();Router.Open(surface);RefreshNow();
```

- `RefreshNow` 平时由 `LateUpdate` 以 **0.2 秒节流**调用（第 96 行）。

### 3.2 失败时序

1. `SaveSystem.Load()` 返回新 `PlayerCollection`（`loaded`），世界事实恢复到既有 Authority；
2. 用例经产品公共入口 `VillageRuntimeBridge.SetSimulationPersonSource(loaded)` 重新绑定长期人物真相源（`VillageRuntimeBridge.cs:833-842`，与 `TownUI.OnReturnFromExpedition` 同义）；
3. 此时 `HomeProductShell._source` 仍是旧实例，而 Shell 的 0.2 秒周期刷新尚未到来（Load 后只 `yield return null` 一帧）；
4. 用例点击真实“管理”导航 → `Navigate(2)` → `OpenSurface(Management)` → **先** `Router.Open(Management)`，**再** `RefreshNow()`；
5. 这次 `RefreshNow()` 首次观察到来源变化 → `Router.Close()`（Surface 复位为 `World`）→ 同一方法后段 `show=false` → `Management` 仍为 inactive；
6. 一帧后断言 `IsVisible`：`true` 期望、`false` 实际 → `Expected: True But was: False`。

即：**管理页并没有被管理规则禁用，也没有拿旧实例或 World Surface 已改变；是测试在 Shell 尚未消费“Load 后来源切换复位”时发出了首次真实导航，产品内部同一次刷新的复位把刚打开的表面关掉。** 这解释了为什么失败点恰好在 Load 之后、且只有这一条用例（其它 8 例都在没有“先 Load 再立即导航”的时序下打开管理页）。

---

## 4. 最窄修复（只改测试时序与证据，不改产品）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

1. 在 `Load → SetSimulationPersonSource(loaded) → 事实等值断言` 之后、真实导航之前，先用 Shell 的**公开周期刷新入口** `HomeProductShell.RefreshNow()` 让产品自己完成这次来源切换复位；随后断言：
   - `ReferenceEquals(Get(_shell,"_source"), loaded)`：Shell 已真实观察新来源；
   - `Surface()=="World"`、`Management.IsVisible==false`：复位后仍在 World，未被提前激活。
   这不是写 Router、不是 `SetActive` 管理页、不是反射私有状态；它与 `LateUpdate` 每 0.2 秒调用的是同一个产品方法。
2. 若进入时已有合法表面模态（目前实测路径为“无”，但防御性保留）：仅当 `CharacterGrowthUI.IsOpen` 时经产品公开 `TryHide()` 关闭；若 `HasPendingChoice` 为真则**断言失败并留证**，不绕过成长门禁。
3. 真实导航仍完全保留：`OpenManagementThroughNavigation → Raycast 命中 → ExecuteEvents.PointerClick → Navigate2 → OnClick`；点击后新增两条强断言：
   - `Surface()=="Management"`（真实导航确实进入管理表面）；
   - `SurfaceModalActive==true`（管理计入统一表面模态）；
   然后才断言 `Management.IsVisible==true`。
4. 新增四行 `[EXP-RETURN]` 证据（不含存档内容）：
   - `reentry-pre-sync`：shell / host / management 实例 ID、Shell 是否已观察新来源、Bridge 来源是否即 `loaded`、ShellVisible、Canvas enabled/active、Surface、SurfaceModalActive、Growth open/pending、管理页 active；
   - `reentry-world-synced`：同步后同一组实例与来源事实；
   - `reentry-nav-committed`：点击后的 Surface / modal / 管理页 active / ShellVisible / Canvas enabled，并标注 `clicked=ProductNavigation/Navigate2 realPointerClick`；
   - 原 `END legal return unlocked` 证据保留。
5. 未删除、未放宽任何原有断言：Return pending 的 `PersonNotInVillage/WorldBusy/zero-write`、手动 Pause 不解锁、合法结算 `Completed/DaysSimulated=1/occupancy 清除/CanSaveWorld/SimulationActive/Clock+1440`、Load 后逐项等值（Clock/Speed/Ingredients/Food/批次/证据/历史/职责/占用/施工/建筑生命周期）、显式保存不推进 Clock/不重复批次与历史，全部原样保留。

### 为什么没有改产品代码

- 产品在“世界来源切换”时复位表面，是既有且必要的语义（避免管理页继续显示旧世界投影）；
- 本次失败是测试在来源切换尚未被 Shell 周期刷新消费的窗口内立即导航，属**测试夹具时序**问题；
- 若在产品侧让 `OpenSurface` 吞掉来源复位（先同步再打开），会改变所有来源切换场景的表面语义，且本执行层未持有 PlayMode 运行证据证明其必要性，不符合“产品改动先确证因果”；
- 因此按产品契约选择最窄的测试时序修复，产品行为完全不变。

---

## 5. 编译与执行证据

- 安全编译：仅 `script-execute` 调用 `AssetDatabase.Refresh()`；之后只读查询 `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false`；Console `Error/Exception` 0 条。
- 测试发现：`ManagementShellPlayTests` 仍为 **9** 例，原 8 例名称不变（见第 1 节查询结果）。
- 本执行层**未启动 Test Runner**、未等待/忙等模型测试；PlayMode 由宿主以短类名 `ManagementShellPlayTests` 整类串行执行。
- 新增证据行与 Play 结果以宿主实际写入 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 为准。

---

## 6. 保护范围

- 未修改 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 所列 34 项与受保护 TMP 字体；
- 未修改/删除 `R3B-SaveProtection-20261024/*` 备份、manifest 与历史档；未删除或覆盖任何 Save；
- 未新增产品代码、Prefab、Scene、Generated 资产；未改 WorkClock / API / DTO / 玩法规则；
- 未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未安装组件、未改权限、未创建额外付费子 Agent；
- 未访问未授权工程、认证目录或其他工作区；
- 改动文件仅：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  2. `Development/StudioRuns/R3B-UI-20261024/return-management-reentry-fix.md`（本文件）

---

## 7. 未测 / 留给宿主的验证

1. **PlayMode 未由本执行层运行**：新增的 `reentry-*` 证据行、导航后 `Surface==Management`/`SurfaceModalActive`/`IsVisible` 三条断言、以及后续保存/撤职按钮与显式保存断言，均以宿主实际 `testResults` 为准；本文件不声称通过。
2. **原始完整栈**：宿主上轮的 `stackTrace` 文本未落仓库，本执行层只保留宿主给出的失败行（修复前 2833）与消息，以及源码级调用链；不伪造栈帧。
3. **产品级同帧竞态未修**：若真实玩家在“世界来源刚切换、Shell 尚未刷新”的同一窗口内点击导航，产品仍可能吞掉这次打开（Surface 复位为 World）。本轮按最小范围只修测试时序，未把该产品级窗口作为缺陷修改；如需产品修复，应另立包并先取得真实运行证据。
4. **物理输入**：仍是程序化 `EventSystem` 点击真实 Button，不等价人工鼠标手感/拖拽验收。
5. **34 项 SHA 与字体 SHA**：本层已只读重算一致；宿主 `OneTimeSetUp/OneTimeTearDown` 会再次重算并强断言。
