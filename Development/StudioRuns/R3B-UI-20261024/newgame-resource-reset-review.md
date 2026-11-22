# R3B-UI-20261024 · NewGame 资源连续缺口核对与最小修复

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一操作目录）
- 阶段：1/1 —— 核对 CASE-C 暴露的 NewGame 资源缺口，区分产品未重置 / 夹具流程 / 只读投影，并在最窄组合层修复 + 加强测试断言
- 本包写入：
  - `UnityProject/RRPG1.0game/Assets/Scripts/Save/SaveSystem.cs`（NewGame 组合层，1 行重置）
  - `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅 CASE-C expected / 断言 / 证据）
  - 本报告
- 本包未做：未运行任何测试（未 `tests-run`、未反射启动、未忙等）、未改 Save DTO / SaveVersion / Authority / 公共 API / 场景 / Prefab / 资产 / 经济初值、未改 `SaveSlotGuard`、维护时间函数、已有 5 例、未使用 `rql-mutate`、未 Shell、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未读写用户单槽存档、未动 14/34 保护与历史备份、未 commit/push/stage。
- **结论：既有契约要求 NewGame 从资源初始 Authority 开始；真实缺口是产品 `SaveSystem.NewGame` 漏用既有 `VillageResourceState.Restore(null)` 重置入口。已在最窄组合层修复，并把 CASE-C 的 expected 改为独立从资源 Authority 计算。CASE-C 尚未执行，本报告不预填 PASS。**

---

## 1. 原绿测为什么不足

最近一次 CASE-C 单跑为 1/1 Passed（`f06dbc01`），`resourceScopeEnd=scope-restored|font|0|632`，但该通过不能证明 NewGame 资源正确：

1. **资源断言只做“自一致”，没有独立 expected**。测试先记录 `initialIngredients/initialFood`，NewGame 后再断言“投影 == 当前 Resources”，并且只比较 `newIngredients == initialIngredients`。当初始值本身已被旧世界污染时，`2/127` 也能与投影自洽。
2. **初始基线来自被污染的世界**。05:43:38 证据为：
   `[CASE-C] START ... ingredients=0 food=180 ...`
   而同一轮 `SAVED-FACTS ... ingredients=2 food=127`、`NEWGAME ... resources=2/127 ...`。
   0/180 不是资源初始 Authority，而是旧单槽在夹具准备阶段被 `TownUI.Start → SaveSystem.Load` 读入后的库存。
3. **测试代码当时把缺口当成允许边界**。旧注释写明“当前 SaveSystem.NewGame 不重置 VillageRuntimeBridge.Resources，归零预期不在本包越界改写核心”，因此把“新世界沿用旧库存”误当成不可改规则。该注释已随本次修复删除/改写。
4. 旧断言没有在 NewGame 后检查 `batches / evidence / completedProjects` 的归零，也没有把资源与旧世界保存值做可辨别性核对；`oldLeak=false` 只是固定字符串，不是资源无泄漏的证明。

因此：**不能把 0/180 当作“新世界正确值”，也不能把 2/127 当作允许结果。**

---

## 2. 真实调用链与 Reset / Load 顺序

### 2.1 产品 NewGame 链

```text
MainMenuUI.OnNewGame
→ HeroSelectionUI.OnConfirm
→ SaveSystem.DeleteSave()
→ SaveSystem.NewGame()
→ PlayerCollection.InitializeDefaultCollection()
→ Social / Activity / AutonomousDecision / SocialFeedback ResetForNewGame()
→ VillageRuntimeBridge.ResetTimeForNewGame()
      → EnsureInitialized(null)（缺实例时 new VillageResourceState()，初始 0/0）
      → Time.ResetForNewGame() + IntradayWork.Restore(null) + Return/Intraday 宿主复位
→ VillageRuntimeBridge.ResetBuildingsForNewGame()
      → Buildings / Assignments / Projects / World 重建，IntradayWork.Restore(null)
→ （修复前：Resources 未重置，保留上一世界库存）
→ SaveSystem.Save(collection)
      → VillageRuntimeBridge.BuildSaveData() 把当前 Resources 镜像写入 Village.Resources
→ TownUI.Initialize(collection)
      → VillageRuntimeBridge.EnsureInitialized(null)（已存在实例，不覆盖）
```

### 2.2 资源 Authority 与既有重置入口

- `VillageResourceState` 是 `Ingredients / Food` 的唯一长期 Authority；构造函数初始值为 `0/0`。
- `VillageResourceState.Restore(null)` 是既有重置入口：将两个值恢复为 `0/0`（与“旧档缺 Resources → 默认 0/0”同一语义）。
- `SaveSystem.Load` 会调用 `VillageRuntimeBridge.RestoreVillageWithoutTime → EnsureInitialized(save) → Resources.Restore(save.Resources)`，即 **Load 才应恢复旧库存**。
- `VillageRuntimeBridge.ResetForTests` 会把 `Resources = null`（下一次 Ensure 重建为 0/0），但那是测试入口，不是产品 NewGame 组合。
- 因此正确顺序应为：**NewGame 重置到初始 0/0 → Save 写新世界 DTO → Continue/Load 恢复该新世界值**。修复前 NewGame 跳到“Save 旧库存”，违反该顺序。

### 2.3 夹具初始 0/180 的来源（区分产品 / 夹具 / 投影）

- `ManagementShellPlayTests` 的 `[UnitySetUp]` 先 `ResetForTests()`（Resources=null），再重建场景。
- 场景加载时 `TownUI.Start` 会执行 `_playerData == null` 分支：存在用户单槽 → `SaveSystem.Load()`，把旧档 `Village.Resources` 恢复到内存；随后 MainMenu 前台化并不回滚该内存值。
- 随后夹具调用 `SaveSystem.NewGame()`。修复前 NewGame 未重置 Resources，于是旧档库存（证据中的 0/180）被保留，`Save`/`Continue` 后成为 CASE-C 的“初始基线”。
- 结论：**0/180 是夹具加载旧档后的内存泄漏，不是资源初始 Authority；产品 NewGame 也确有未重置缺口。二者叠加，但根因修复点在产品组合层。** 本次测试改为独立计算 Authority expected，正是为了不再依赖该夹具副产物。

> 注：本包未读取用户单槽文件内容（在授权目录外），因此“旧档=0/180”是由调用顺序与证据值推得，不宣称逐字节核对。

---

## 3. 最小修复（产品，最窄组合层）

文件：`UnityProject/RRPG1.0game/Assets/Scripts/Save/SaveSystem.cs`，`NewGame()` 内：

```csharp
VillageRuntimeBridge.ResetTimeForNewGame();
// VillageResourceState 是 Ingredients / Food 的唯一长期 Authority；NewGame 必须回到其
// 默认初始状态（既有 Restore(null) = 0/0），不得把上一世界的库存带入新世界。
// 这里只复用既有重置入口，不新增 Authority / DTO / 公共 API。
if (VillageRuntimeBridge.Resources != null) VillageRuntimeBridge.Resources.Restore(null);
VillageRuntimeBridge.ResetBuildingsForNewGame();
```

边界说明：

- 只复用既有 `Restore(null)`，没有新增 `ResetResourcesForNewGame` 等公共 API，没有新 Authority / DTO / SaveVersion / 经济初值。
- 未改动 `VillageResourceState` 本身的语义、结算、工作、Clock。
- 未改动 `ResetForTests`、`Load`、`Save`、`SaveSlotGuard` 或维护时间辅助函数。
- `SaveSystem.NewGame` 是所有产品 NewGame（MainMenu 直入、HeroSelection 确认）的共用组合点，修复一处即可覆盖真实入口。

---

## 4. CASE-C 断言加强（仅测试）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

### 4.1 新增独立 Authority 计算

- `FreshResourceAuthority()`：只借用当前 `VillageRuntimeBridge.Resources` 的运行时类型，`Activator.CreateInstance` 新建一个全新 `VillageResourceState`；**不读取 Singleton 当前库存，不依赖 Management 投影，也不引用旧档**。
- `InitialResourceIngredients()` / `InitialResourceFood()`：从该全新实例读取初始 Authority 值（当前实现为 0/0）。

### 4.2 初始段（段 0）

- 断言 `initialIngredients == 初始 Authority Ingredients`、`initialFood == 初始 Authority Food`。若夹具仍把旧档库存当初始值，本断言会先失败并指出来源错误。
- 保留 `batches / evidence / completedProjects == 0`。
- `START` 证据新增 `authorityInitial=...`。

### 4.3 NewGame 段（段 6）

新增/加强：

- `expectedNewIngredients/expectedNewFood` 再次独立取自资源 Authority。
- 先断言夹具的旧世界保存值与该 Authority 可辨别（否则测试无法证明“无泄漏”）：
  `savedIngredients != authorityInitialIngredients`、`savedFood != authorityInitialFood`。
- 断言 `NewGame` 后 `Ingredients / Food` **等于初始 Authority**，不得等于旧世界库存。
- 断言 `batches / evidence / completedProjects` 回到初始值（0），不得保留旧世界生产与执行历史。
- 保留原有：旧 PersonId 不残留、旧人物岗位职责不泄漏、旧建筑实例不出现在岗位/施工行、施工目标为空、Clock=0、人物投影逐 PersonId 等价、候选/检索/选择为空。
- `NEWGAME` 证据新增 `authorityInitial=...`、`batches/evidence/completed` 与 `newIngredientsWereAuthority/newFoodWasAuthority`。

### 4.4 未改内容

- 原 5 例断言未删除、未放宽、未 skip。
- `SaveSlotGuard`（备份/恢复/未完成 run 阻断）、34 项基线校验、字体哈希、维护时间辅助函数均未改。
- 未新增测试用例；发现数仍为 6。

---

## 5. 编译与现场核对（本包实际执行）

1. 修改前 Editor 状态：`isPlaying=false, isPaused=false, isCompiling=false, isUpdating=false`；`SampleScene.isDirty=false`，Console Error=0。
2. 仅执行安全刷新：`AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` → 返回 `refresh-requested`。
3. 编译状态：`isCompiling` 由 `true → false`（未启动/关闭 Unity）。
4. 编译后 Console 只读回读：`logType == "Error"` 或 `message contains "error CS"` = **0 条**。
5. 测试发现（只读）：`ManagementShellPlayTests` = **6 条 PlayMode**，目标方法仍在列。
6. 刷新/编译后 `SampleScene.isDirty=false`；未使用 `rql-mutate`，未触发无关资产/场景保存。

---

## 6. 存档保护与保护基线

- `20261024T054336Z/manifest.json`：`status=restored`、`restored=true`、`sourceBytes=backupBytes=13405`、`sourceSha256=backupSha256=4b2b56f6…`、`readbackVerified=true`、`lastError=null`。
- `20261024T053959Z/manifest.json`：同样 `restored=true`、13405B / `4b2b56f6…`。
- `20261024T052704Z/manifest.json`：`status=protective-snapshot`、`pendingRestore=false`，按既有白名单不阻断后续测试，也不作为待恢复 run。
- 因此当前保护目录无 `captured` / `restore-failed` / 未完成 run；本包未写任何 run 目录、未覆盖备份、未恢复历史档。
- 34 项 `workspace-baseline.json`、14 项指定保护与 TMP 字体本包未触碰；未重算哈希，不以历史 34/34 冒充本轮结果。下一次宿主 CASE-C 仍由测试自身输出 `BASELINE-PRE/POST` 与 `FONT-PRE/POST`。
- 历史 21075B / `650c45c4…` 缺口仍按原记录保留，本包未处理、未恢复。

---

## 7. 未测 / 交宿主

- 本包**未运行任何测试**，不声明 CASE-C 已通过、不声明完整 6 例回归或全量回归通过。
- 运行态仍未验证：NewGame 后资源是否为 0/0、批次/证据/历史是否归零、旧世界资源是否不再进入新档；这些必须由宿主下一次单方法 check 的实际回执决定。
- 建议宿主仅串行执行同一门禁：

```text
PlayMode
RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak
```

- 运行后应核对：
  1. `[CASE-C] START ... ingredients=0 food=0 authorityInitial=0/0`（若仍非 0/0，停止并保留证据，说明仍有未识别入口）；
  2. `SAVE-CAPTURE/SAVE-RESTORE` 成对、新 run manifest 为 `restored=true` 且 hash/字节匹配；
  3. `BASELINE-PRE/POST all-match`、`FONT-POST unchanged=True`、`resourceScopeEnd=scope-restored|font|…`；
  4. `[CASE-C] NEWGAME ... resources=0/0 ... batches=0 evidence=0 completed=0 clock=0 oldLeak=false`；
  5. 场景结束仍 `isDirty=false`。
- 本包不自行重跑、不轮询等待宿主结果。

---

## 8. 风险与未扩展范围

- 本次只处理 `Ingredients / Food` 资源 Authority。NewGame 是否还应对其它 Village / Event 长期区块做同等隔离，不在本包范围；未据此宣称全世界 NewGame 事务已完成。
- 未改 Save Trigger Ownership、SaveVersion、DTO、公共核心接口；若后续要求“新游戏初始资源可配置/非 0”，应先取得正式 Authority 决策，不能由测试或本修复自行改写经济初值。
- 若宿主运行后 CASE-C 仍在资源断言处失败，应以其实际回执为准，停止并交主开发复核，不重放旧模型、不放宽断言、不改旧资源绕过。
