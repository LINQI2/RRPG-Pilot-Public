# R3-B Pilot 下一有界 Return gating Play 专项｜开发执行记录

生成时间：2026-10-24（本机）
状态：**测试代码已写入并通过安全编译；PlayMode 测试尚未由本执行层启动，等待宿主串行运行 `ManagementShellPlayTests` 整类。**

---

## 1. 本阶段范围与边界

- 只补一条最小代表的远征 / Return gating UnityTest，追加到既有
  `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`。
- 复用该类既有 `SaveSlotGuard`（单槽字节备份/恢复、34 项 workspace baseline、字体 SHA、TearDown 恢复）。
- 未新增产品规则 / 公共 API / DTO / Save 字段；未改 `Work` / `Clock` 核心；未反射写任何私有锁或私有状态。
- 未使用 `rql-mutate` / Shell / 全局 Save / Git 写；只使用 `AssetDatabase.Refresh` 触发安全编译与只读 `rql-query`。
- 保留原 8 个用例的强断言与 NewGame 修复不变；本类用例数 8 → 9。

写入文件（仅此两项）：

1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
2. `Development/StudioRuns/R3B-UI-20261024/expedition-return-gating-review.md`（本文件）

---

## 2. 先决状态核对（只读）

| 项目 | 实际值 / 证据 |
|---|---|
| Unity 版本 | `2022.3.62t9`（`editor.unityVersion` 只读查询） |
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` |
| 场景干净 | 唯一打开场景 `Assets/Scenes/SampleScene.scene`，`isDirty=false`，`rootCount=5` |
| Console | 编译后只读查询 `logType == Error/Exception`：0 条 |
| 测试发现 | `tests` 查询到新用例 `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement`，`mode=PlayMode`，`assembly=BattleStage1.PlayModeTests` |
| 34 项基线 | `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 为 34 项固定只读 Git SHA 基线；`OneTimeSetUp` 会在宿主运行时逐项重算并断言，`OneTimeTearDown` 再核一次。本执行层未重算 SHA（无 Shell/哈希工具），**不以“文件存在”冒充 SHA 一致**。 |
| 字体 | 基线中受保护字体 `RRPG_SourceHanSansCN_Regular_TMP.asset` 期望 `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；未在本阶段改写。宿主用例 `FONT-PRE/FONT-POST` 会重算并断言 unchanged。 |
| Save protection manifests | `Development/StudioRuns/R3B-SaveProtection-20261024/*/manifest.json` 共 38 份，`status` 全部为 `restored` 或 `protective-snapshot`；不存在未完成 run，`SaveSlotGuard.BlockOnUnfinishedRuns` 不会阻断。 |

> 说明：34 SHA 与字体 SHA 的最终一致结论只能由宿主实际运行 `OneTimeSetUp/OneTimeTearDown` 得出；本文件只记录可复核的只读前置事实。

---

## 3. 真实调用层级（不把 SetExpeditionPersonIds 冒充完整远征）

### 3.1 出征与离村

1. 真实世界基座：既有公开建造 / 施工 / Clock.AdvanceTo 完成 Farm A（Completed），再保留 Farm B（UnderConstruction）。
2. **真实产品入口**：`HomeProductShell.ProductNavigation/Navigate5`（底部导航“远征 ↗”）经程序化 `EventSystem.RaycastAll + ExecuteEvents.PointerClick` 命中真实 Button，走 `TownUI.EnterProductExpedition → ExpeditionUI.ShowExpeditionFromTown`。
3. 真实准备宿主自动选中第一张可出征角色卡（`ExpeditionUI.ShowExpeditionUI → FindFirstDeployableCharacterCard → OnCharacterCardClick`），本用例从 `ExpeditionSystem.SelectedCharacters` / `SelectedHero` 读取真实 PersonId 名单。
4. 出征前职责/施工前置：
   - `VillageRuntimeBridge.TryAssignPerson(partyPerson, workplace:farmA:…)`：既有公共派工 Authority，建立真实动态职责；
   - `VillageRuntimeBridge.TryAddConstructionWorker(farmB, constructionPerson)`：既有公开施工入口，留村人物成为真实施工参与者；
   - `VillageResourceState.AddFood(100)`：既有公开资源写入口，满足真实 Carry preflight，不伪造 Carry。
5. **真实确认按钮**：`ExpeditionUI.btnConfirm.onClick` → `OnConfirmExpedition`：
   - 真实创建 `AdventureManager`；
   - `SetSimulationActive(false)`；
   - `SetExpeditionPersonIds(adventure.GetExpeditionPersonIds())` 登记离村人物；
   - 显示 Journey；隐藏 Town Canvas。

### 3.2 Return 推进与合法结算

- 为了在“Return 进行中”这个**相位中间**做 gating 断言（Product `OnReturnToTown` 是同步跑完整事务，无法在中间插入断言），使用既有公共 Return 入口：
  `VillageRuntimeBridge.BeginExpeditionReturn(1 day, Retreat)`
  → 仅建立一次 `ExpeditionReturnPhase.Active` 会话（`ActiveReturnRecord`），不推进 Clock、不结算。
- 随后仍由**真实产品入口** `ExpeditionUI.OnReturnToTown()` → `FinalizeReturn(Retreat)` 按既有相位续跑：
  `Settlement（真实 _currentAdventure.SettleReturnDetailed）`
  → `MarkExpeditionReturnSettled`
  → `RunReturnCatchUp`（真实 `VillageTime.AdvanceTo(target, WorldAdvanceOrigin.Return, identity)` 逐日推进 1 天）
  → `ReturnRemainingCarryStage`
  → `VillageReturnPersonConsequence.TryCommit`
  → `ClearExpeditionPersonIds`（真实 `Time.HoldForFinalSave`）
  → `SaveSystem.SaveReturnFinal`（Final Return Save）
  → `CompleteExpeditionReturnSave(true)` → `FinishedCompletedReturnPresentation → TownUI.ShowAfterCompletedExpedition`。

> elapsed = 1 day 是既有 `Test / Not Frozen` 输入，通过公共 Return 入口建立；**不是**用 `Clock.AdvanceTo` 冒充 Return，也没有用 `SetExpeditionPersonIds` 冒充注册。

---

## 4. 用例逐项断言清单

用例：`ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement`

### A. 出征 / 离村（真实产品入口）

- `CanSaveWorld == false`；
- `IsSimulationActive == false` 由真实出征入口设置；
- `ExpeditionPersonIds` 数量与真实出征名单一致，且逐 PersonId 包含；
- Home `ShellVisible == false`，`TownUI.canvasRoot.activeSelf == false`；
- 共享 Router 仍为 `World`；
- 管理只读投影把出征人物标为 `Expedition`；
- 出征前真实动态职责 `workplaceA` 与岗位占用 1 保持。

### B. Return 进行中（Active pending）

- `ActiveReturnRecord != null`、`Phase == Active`、`Completed == false`、`CanSaveWorld == false`；
- 管理 Gateway 防御（逐条）：
  - 对远征人物派工 → `PersonNotInVillage`；
  - 对远征人物撤职 → `PersonNotInVillage`；
  - 对留村人物派工 → `WorldBusy`；
  - 对留村人物加入施工 → `WorldBusy`；
  - 对留村人物撤出施工 → `WorldBusy`；
  - `SaveWorld()` → `WorldBusy`；
- 全部被拒命令后零写：职责、岗位占用、施工参与者/进度/劳动、远征登记、Clock、存档字节 SHA 均不变；
- 真实 Management UI 绑定：`saveButton.interactable == false`、对远征人物 `removeButton.interactable == false`；真实 `RemoveSelectedPersonAssignment()` / `Save()` 返回 false；状态文案含“远征 / 未保存 / 世界正在推进”；保存失败不写盘；
- 表面开关 / ESC：直接产品回调 `OpenSurface(Management)` 后管理页仍 `IsVisible == false`（Shell 隐藏，真实 UI 不可达）；`HandleEscape` 回 `World`，`CanSaveWorld` 仍 false；
- 手动 Pause：真实 `TogglePause` 把 Clock 置 `Normal` 后，连续 3 个真实帧 Clock 绝对分钟不变、`IsSimulationActive` 仍 false、Return 仍 `Active`、`CanSaveWorld` 仍 false；随后恢复 `Paused`。

### C. 合法结算回村

- `ReturnRecord.Completed == true`、`Phase == Completed`、`DaysSimulated == 1`；
- `ExpeditionPersonIds.Count == 0`，`CanSaveWorld == true`；
- `IsSimulationActive == true`，Clock 仍 `Paused`；
- Clock 绝对分钟 == 基座 14400 + 1440（恰好 1 天，不得重复推进/第二 Clock）；
- Home Shell 恢复可见，Router `World`；
- 出征人物恢复 `InVillage`，原动态职责 `workplaceA` 与占用 1 保持；
- `ProductionBatchCount == 0`、`AssignmentContributionHistory == 0`：离村人物职责在远征占用期间不执行，Catch-up 不得重复生产/历史；
- Farm B 仍 `UnderConstruction`，原施工参与者保持；
- `SaveSystem.HasSave == true`（Final Return Save 已真实落盘）。

### D. 保存可恢复 / 不重复

- 最终 Return Save 后记录 Clock / Ingredients / Food / 生产批次 / 工作证据 / 执行历史 / 动态职责 / 岗位占用 / 施工真实事实 / Farm B 生命周期；
- 真实 `SaveSystem.Load()` 后逐项等值；`SetSimulationPersonSource(loaded)` 后再次 `RefreshNow`；
- Load 后 `CanSaveWorld == true`；
- 合法回村后才解锁：真实底部导航进入管理，管理页可见，`saveButton` / 原职责人物的 `removeButton` 均可交互；真实点击保存 → 状态“已保存当前世界。”，Clock/批次/历史不变。

---

## 5. 未测 / 风险与明确未覆盖

1. **事件 pending 的独立构造未测**：本用例覆盖 Return 进行中的 Active 相位与真实 1 天 Catch-up；未单独构造“统一事件 pending 正在阻断 Advance”的 UI 会话。若宿主运行时 Catch-up 因事件 pending 返回 `WaitingForInteraction`，本用例会失败并留下 Console 证据，不会静默通过。
2. **管理 UI 在远征/Return 期间实体不可达**：因产品 `SetSimulationActive(false)` 使 Home Shell `enabled=false`，本用例没有强行点击不可达按钮；改以真实禁用绑定状态 + Gateway 防御双重断言。物理鼠标/键盘输入不在本用例。
3. **Return 相位中间点由公共 `BeginExpeditionReturn` 预置**：Settlement / Catch-up / Carry / Consequence / Clear / Final Save 均由真实产品 `OnReturnToTown → FinalizeReturn` 执行；但“Begin 由测试先调用”这一点已如实标注，不宣称是单次物理点击的完整连续 UI 会话。
4. **elapsed = 1 day 为 Test / Not Frozen 输入**：未从真实 Node 战斗推进推导 elapsed；本用例目标是 gating 与 exactly-once 连续性，不是远征进度规则验收。
5. **34 项 SHA 与字体 SHA 未由本执行层重算**：只核对了基线定义、受保护路径与 Save manifest 状态；最终结论以宿主 `OneTimeSetUp/OneTimeTearDown` 实跑为准。
6. **未执行 PlayMode 测试**：遵守“宿主随后短类名整类串行、不模型启动测试或忙等”的约束。本文件不声称用例通过；任何 PASS/FAIL 以宿主实际 `testResults` 与证据日志为准。

---

## 6. 编译与执行证据

- 安全编译：`AssetDatabase.Refresh()` 后只读查询 `isCompiling=false / isUpdating=false / isPlaying=false`；
- Console：`logType == Error/Exception` 0 条；
- 测试发现：新用例已在 `BattleStage1.PlayModeTests` 中可发现；
- 本类当前 9 个 PlayMode 测试，原 8 个名称保持不变；
- 宿主待执行：短类名 `ManagementShellPlayTests` 整类 Play 串行（不并行 Test Runner）。

## 7. 受保护范围

- 未触碰 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 所列 34 项（含 TMP 字体）；
- 未触碰 Save 备份历史档与 `R3B-SaveProtection-20261024/*`；
- 未新增产品代码 / Prefab / Scene / Generated 资产；
- 未执行 Git 写操作；未使用 `rql-mutate`；未安装组件、未改权限、未创建付费子 Agent。
