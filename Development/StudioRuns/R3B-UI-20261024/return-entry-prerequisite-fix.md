# R3-B Pilot 出征确认前置专项｜开发执行记录

生成时间：2026-10-24（本机）
状态：**已完成只读定位与测试夹具修复；安全编译通过，PlayMode 尚未由执行层启动，等待宿主串行运行 `ManagementShellPlayTests` 整类。本文件不声称 Return 用例通过。**

---

## 1. 本轮先决状态（只读核对）

| 项目 | 实际值 / 证据 |
|---|---|
| Unity 版本 | `2022.3.62t9`，`projectPath=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` |
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` |
| 场景干净 | 唯一打开场景 `Assets/Scenes/SampleScene.scene`，`isDirty=false`、`rootCount=5` |
| 测试发现 | `ManagementShellPlayTests` 共 **9** 个 PlayMode 测试，assembly=`BattleStage1.PlayModeTests`，名称与原 8 例保持不变 |
| 上轮结果 | 9 run：**8 Passed / 1 Failed / 0 Skipped**；仅 `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement` 失败 |
| 上轮失败栈 | `ManagementShellPlayTests.cs:2588`（修复前行号），断言“补给足够时真实确认按钮必须可用”，`Expected: True But was: False` |
| 上轮保存保护 | `R3B-SaveProtection-20261024` 下最新 9 份 manifest（`...111448Z`～`...111455Z`）全部 `status=restored`、`restored=true`，无未完成 run |
| 基线 / 字体 | 本层只读重算：`baselineEntries=34 mismatch=0 missing=0`；字体 SHA=`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致）。上轮宿主日志同为 `BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`、`FONT-POST … unchanged=True` |
| 34 项 / 备份历史档 | 本阶段未触碰 `R3B-20261022/workspace-baseline.json` 所列 34 项、字体资产、Save 备份与历史 manifest |

---

## 2. 失败真实根因（先证因果，不预设“只缺补给”）

### 2.1 产品确认按钮的真实更新链

1. `TownUI.EnterProductExpedition → OnExpedition → ExpeditionUI.ShowExpeditionFromTown`
   （`TownUI.cs:173-189`、`ExpeditionUI.cs:1297-1319`）。
2. `ShowExpeditionFromTown` 创建 `ExpeditionSystem`，自动选英雄，调用 `ShowExpeditionUI`。
3. `ShowExpeditionUI` 依次执行 `ShowHeroSelection → RefreshCardPanel → 自动选中第一张可出征角色卡
   → RefreshPrepSupplyAndPreflight`（`ExpeditionUI.cs:190-215`）。
4. 角色卡选择走 `OnCharacterCardClick → UpdateSummary → RefreshPrepSupplyAndPreflight`
   （`ExpeditionUI.cs:399-460`）。
5. `RefreshPrepSupplyAndPreflight` 在**调用当时**读取：
   - `VillageRuntimeBridge.Resources.Food`（Village Food 唯一 Authority）
   - `VillageRuntimeBridge.ResourceCarryPerPerson`
   - `VillageRuntimeBridge.ResourcePerPersonDailyConsumptionRate`
   - `LegacyNodeElapsedDaysTestPolicy.BattleNodeElapsedDays`
   算出 `_preflightCarryOk`，并设置
   `btnConfirm.interactable = _expedition.IsReady && _preflightCarryOk`
   （`ExpeditionUI.cs:1046-1074`）。
6. `OnConfirmExpedition` 点击时还会调用 `GuardWorldWrite()` 并重新执行 preflight
   （`ExpeditionUI.cs:470-492`）；但 UI 层 `interactable=false` 时真实玩家点击不会进入该回调。

### 2.2 上轮失败的实际时序

修复前用例顺序是：

1. 经真实导航 `ProductNavigation/Navigate5` 打开准备界面；
2. 自动选中角色卡，`ShowExpeditionUI / OnCharacterCardClick` 用**当时** Village Food 求值；
3. 之后才 `Call(ResourcesState(), "AddFood", 100)`；
4. 随后立即断言 `btnConfirm.interactable == true`。

上轮日志已证 NewGame 初始资源为 `0/0`（`[CASE-C] NEWGAME … resources=0/0`），且本用例在进入准备前没有补食物。于是：

- 第 2 步 preflight 读到 `villageFood=0`，`actualCarry=0 < firstStepCost`，按钮被真实置为 `false`；
- 第 3 步 `AddFood` 只写 Village Resource Authority，**不会**重跑准备界面的 preflight；
- 第 4 步读到的是**上一步真实 UI 求值后的陈旧状态**，而不是此刻补给不足。

因此失败根因是**测试夹具在真实 preflight 求值之后才配置补给**，不是出征业务规则失败，也不是 `IsReady`、WorldBusy、Return pending 或成长 pending 造成的禁用。

### 2.3 其余必要条件的实际状态

修复版本在断言前逐项读取并留存证据：

- 队伍：`ExpeditionSystem.IsReady` 必须为 true（真实准备宿主自动选中至少 1 张角色卡）；
- 补给：`actualCarry = min(Village Food, partySize × carryPerPerson)` 必须 `>= firstStepCost = partySize × battleDays × dailyRate`；
- 世界写锁：`CanSaveWorld=true`、`IsWorldWriteLocked=false`、`ActiveReturnRecord=null`（无 Active Advance、无未完成 Return）；
- 成长 pending：`CharacterGrowthUI.HasPendingChoice=false`（不得被成长选牌门禁掩盖）；
- 区域选择：当前产品 `ExpeditionUI` **不存在**出征前区域/目的地选择字段或门禁；地图/路线在确认后才由 Adventure/Progression 生成。本专项不虚构区域门禁。

---

## 3. 修复内容（仅测试夹具，未改产品规则）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

1. 将真实补给夹具前移：在真实导航 `Navigate5` **之前**，通过既有公开入口
   `VillageRuntimeBridge.Resources.AddFood(100)` 写入 Village Food Authority；
   注释明确“不在打开后直接改 `btnConfirm.interactable`，那会绕过真实 UI 更新链”。
2. 删除打开准备界面之后才 `AddFood` 的旧顺序。
3. 在点击确认前新增真实前置断言与证据：
   - `ExpeditionSystem.IsReady`；
   - 真实补给来源与计算结果（partySize / food / carryPolicy / dailyRate / battleDays / requestedCarry / actualCarry / firstStepCost）；
   - `CanSaveWorld`、`IsWorldWriteLocked`、`ActiveReturnRecord`；
   - `CharacterGrowthUI.HasPendingChoice`；
   - 新增 `[EXP-RETURN] confirm-preflight …` 证据行，记录按钮状态与禁用原因相关输入，避免输出存档内容。
4. 保留原调用链：真实导航 `Navigate5 → ExpeditionUI 选择 → 真实 btnConfirm.onClick.Invoke()`；
   未强置 `interactable`、未反射写私有核心字段、未直接调用 `Confirm` 绕过禁用、未用
   `SetExpeditionPersonIds` 冒充出征、未吞任何断言。
5. 原 8 例、NewGame 资源修复、`SaveSlotGuard`、Return 全链路强断言均保持。

### 为什么没有改产品代码

- 当前真实产品路径在远征准备期间会 `SetSimulationActive(false)`，准备界面内没有生产/进食等资源变更路径；
  补给 preflight 在每次真实选人/打开准备时都会重算。
- 既有真实远征 Play 基座（`ExpeditionV4FReturnProductPlayTests`、
  `ExpeditionReturnIntegrityProductPlayTests`）同样是在确认前用公开 `AddFood` 配置真实 Food。
- 本次失败来自测试在 preflight 之后补资源，而不是产品漏刷新真实可达状态；
  加 UI 轮询会扩大产品行为，不符合“最窄既有 UI 组合修复”，故不改经济/玩法/API/DTO/WorkClock Authority。

---

## 4. 编译与静态执行证据

- 仅通过 `script-execute` 调用 `AssetDatabase.Refresh()` 触发安全编译；未使用 Shell、`rql-mutate`、全局 Save 或 Git 写。
- Refresh 后只读查询：`isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false`。
- Console 只读过滤 `Error/Exception`：0 条。
- 测试发现：`ManagementShellPlayTests` 仍为 9 例，原 8 例名称不变。
- 本执行层未启动 Test Runner、未忙等；PlayMode 由宿主以短类名 `ManagementShellPlayTests` 整类串行执行。

---

## 5. 保护范围

- 未修改 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 所列 34 项（含 TMP 字体）。
- 未修改 `R3B-SaveProtection-20261024/*` 备份、manifest 或历史档；未删除/覆盖任何 Save。
- 未新增产品代码、Prefab、Scene、Generated 资产；未安装组件、未改权限、未创建额外付费子 Agent。
- 未访问未授权工程、认证目录或其他工作区。

---

## 6. 未测 / 待宿主验证

1. **PlayMode 未由本执行层运行**：新增的 `confirm-preflight` 证据行、Return 后续 gating / 结算 / Load 断言，均以宿主实际结果为准；本文件不声称通过。
2. **34 项 SHA 与字体 SHA**：本层已用只读脚本按基线逐项重算（34/34 一致、0 missing、字体一致）；宿主 `OneTimeSetUp/OneTimeTearDown` 仍会在测试前后再次重算并强断言。
3. **区域选择**：当前准备产品无出征前区域选择门禁，未构造不存在的产品输入。
4. **物理输入**：仍使用程序化 `EventSystem` 点击真实 Button，不等价人工鼠标手感/拖拽验收。
5. **既有 V4 远征测试**：它们直接 `onClick.Invoke()`，不以 `interactable` 为断言；本用例补上了真实按钮可用性断言，但不替代 V4 规则验收。
