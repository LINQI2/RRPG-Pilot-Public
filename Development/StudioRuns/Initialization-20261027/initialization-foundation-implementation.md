# Initial World Configuration 第一垂直层 · 实施记录

- 仓库/唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`
- 阶段：**1/1 —— 正式初始化配置与真实实体创建底层**（本文件只覆盖这一层，不推导整包完成）
- 范围来源：**制作人 `69d2bd2e` 的确认来自本次任务消息（conversation message），不是仓库原有设计文档**；本文件不把消息来源冒充为既有 Authority。
- 执行方式：仅使用文件读写、Glob/Grep/Read、`rql-query` 只读查询、`script-execute`（只读 + 一次安全 `AssetDatabase.Refresh` + 一次非测试运行态冒烟）、本任务授权目录内写文件。
  **未使用** Shell / Process / Git、未 `tests-run`、未 `rql-mutate`、未 `SaveAssets/SaveChanges/SaveDirtyScenes/SetDirty/ClearDirty`、未进 PrefabStage、未测字模、未改 Prefs / 权限 / Packages / 组件 / 计费、未启停 Editor、未创建子 Agent / Workflow / Goal。
- 验证状态：**未预填通过**。本文件不含任何测试 PASS/FAIL；`InitialWorld.Tests` 整类必须由宿主 Studio 阶段 check 运行，结果以宿主回执为准。

---

## 1. 首项前置核验（真实值，符合后才写入）

| 项 | 实际值 | 结论 |
|---|---|---|
| 当前 cwd | `D:\AI_Agent_Lab\rrpg_pilot\repo` | ✅ 与授权一致 |
| workspace 身份 | project | ✅ |
| Unity 工程 | `D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game`（productName `RRPG_AI_Pilot`） | ✅ 已手动打开的本任务工程 |
| Unity 版本 | `2022.3.62t9` | ✅ 与任务声明一致，未启停 Editor |
| 播放/编译/导入 | `isPlaying=false`、`isCompiling=false`、`isUpdating=false` | ✅ 空闲 |
| 原生 TestRunner | 反射 `TestRunnerApi.IsRunActive` = `false` | ✅ 空闲，无未知执行 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`）`isDirty=false` | ✅ 未保存/未污染场景 |
| 字体 disk | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与既有保护值一致 |
| 字体 IsDirty | 9 个 sub-asset 全部 `clean`（`fontDirty=false`） | ✅ 无 pending 字体编辑；**未测字模** |
| 存档 guard | `Development/StudioRuns/R3B-SaveProtection-20261024`：310 个 run，`protective-snapshot=1`、`restored=309`、**unfinished=NONE** | ✅ 后续测试不被误阻断 |
| 用户存档当前实际值 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，**exists=true、13134 bytes、SHA256=`adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`** | ✅ 按实际值记录，**未硬套 13438 / 13405** |
| 当日存档内容只读核对 | `SaveVersion=1`、`GameVersion=0.2.3`、Characters=5、Heroes=1、Buildings=0（next=1）、Ingredients=0 / Food=0、Assignments=0 | ✅ 与后续默认提取一致（只读，不写档） |

**基线（不使用 Git 能力，不伪造 HEAD）**：宿主开工状态报告 HEAD=`054b00d3e38fe2e2e142bfdb2502de90f7991030`（来源：主开发开工 status，非本次独立 Git 读取）。本包用只读文件哈希复算既有 R3B 34 项基线：`entries=34, changed=0, missing=0`，结果持久化为
`Development/StudioRuns/Initialization-20261027/baseline-r3b-20261022-current.json`。既有 dirty / 原 34 项 / R3B 最新 17 例修复文件均未被本包修改或覆盖。

---

## 2. 实现清单（真实落盘）

### 2.1 新增文件

| 文件 | 行数 | 说明 |
|---|---|---|
| `Assets/Scripts/Village/Runtime/InitialWorldConfiguration.cs` | 156 | 统一配置数据模型（人口 / 金币 / Ingredients / Food / 建筑布局）+ `InitialWorldConfigurationValidator` 纯结构校验。位于既有 `Village.Runtime` asmdef（无 Unity 依赖、无 Authoring 副作用） |
| `Assets/Scripts/Initialization/InitialWorldConfigurationCatalog.cs` | 130 | Host 层唯一读取 / 内容校验入口：从当前实际 Authority 提取默认配置；校验 PersonDefinition / Hero / BuildingDefinition 引用与初始占地（复用 `BuildingPlacementValidator`）。Assembly-CSharp |
| `Assets/Tests/EditMode/Initialization/InitialWorld.Tests.asmdef` | 25 | 新测试程序集，引用 `Village.Runtime` / `Person.Runtime` / `Assembly-CSharp-Editor`，`Editor` 平台 + `UNITY_INCLUDE_TESTS` |
| `Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` | 672 | 独立 `InitialWorldConfigurationTests`（10 个 Edit 测试，见 §6） |

### 2.2 修改文件（仅必要接线，未改 SaveVersion / DTO / 既有断言）

| 文件 | 改动摘要 |
|---|---|
| `Assets/Scripts/Village/BuildingRuntime.cs` | 新增 `internal bool TryAddInitialCompleted(...)`：只在**未提交的候选** BuildingRuntime 上分配 `Completed` 实例，复用既有 `Preview`（边界 / 重叠 / 定义）与同一 allocator。施工路径 `TryPlace`（UnderConstruction）未改 |
| `Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs` | 新增 `NewGameWorldPlan` + `internal TryPrepareNewGameWorld` + `internal CommitNewGameWorld`。准备阶段纯内存：按配置创建 Completed 实例，复用既有 `FunctionWorkplaces` / `FunctionResourceWorkplaces` 注册实例岗位与资源岗位，保留既有静态岗位；任一失败 → 零提交。提交阶段一次性替换 Buildings / Assignments / Projects / World / IntradayWork / 资源岗位派生表，并把资源写回唯一 `VillageResourceState` |
| `Assets/Scripts/Core/PlayerCollection.cs` | `InitializeDefaultCollection` 改为从 `InitialWorldConfigurationCatalog.CreateDefault()` 走同一条初始化路径；新增 `public bool InitializeFromConfiguration(config, out diagnostic)`（显式纯参数，非可变 setter）。人物创建仍复用 `PersonBootstrapper → CharacterInstance → ResolveOrCreateInitialCard → BindCharacterCardOwner`；默认路径保持既有宽松容错，显式配置 strict（失败整体丢弃） |
| `Assets/Scripts/Save/SaveSystem.cs` | `NewGame()` 改为读取默认配置后调用 `NewGame(config)`；新增 `public static PlayerCollection NewGame(InitialWorldConfiguration config)`：守卫 → 全量校验 → 候选世界准备 → PlayerCollection 创建 → 既有重置序列 → 一次性提交。**Continue / Load 路径未改** |
| `Assets/Scripts/**`（其余） | 未改。`Assets/Generated/**`、`Assets/Resources/**`、场景 / Prefab / 字体 / ProjectSettings / Packages / Tools / Development/Config / Development/Generated 均未改 |

新增 `.meta` 由 Unity 安全 `AssetDatabase.Refresh(ForceSynchronousImport)` 自动生成，未手写生成资产。

### 2.3 文件指纹（本包实测，只读哈希）

| 文件 | 字节 | SHA256 |
|---|---|---|
| `Assets/Scripts/Village/Runtime/InitialWorldConfiguration.cs` | 6455 | `92954c19a4c60bd46722ff8f8344fe6984f8c5a07682babfef8d96deda0db0cd` |
| `Assets/Scripts/Initialization/InitialWorldConfigurationCatalog.cs` | 6368 | `eeb4e55d4942a7052867a8195e76e3251da0ca19f26c9534c279f34e90a1da2e` |
| `Assets/Scripts/Village/BuildingRuntime.cs` | 6609 | `7f82e113b8e8506bd8aa8d6aada2fade9bfe74007133a8b76e96682a2aea8c08` |
| `Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs` | 18866 | `f18a8bbd9c69e57059509ac492c9df82fd9e2ed93a5cc19eb8b63d44e8814b2b` |
| `Assets/Scripts/Core/PlayerCollection.cs` | 24025 | `4160818b0a9a8ce10a20a9457b398c2d9c42b48714468303e7db3f0895858ff6` |
| `Assets/Scripts/Save/SaveSystem.cs` | 47363 | `bc9fea4f6d64c900a1b25098b2636df9dffb7cd86becf96683d4e962db21bffb` |
| `Assets/Tests/EditMode/Initialization/InitialWorld.Tests.asmdef` | 565 | `654415a10fa2fd5d7537544757f663ad507dc61ac10f05e414af95028da8bf06` |
| `Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` | 36657 | `7032f8b62502e8465529f75a61b6f5818b3de26ce440fc2ddc1b6abaef85c251` |
| `Development/StudioRuns/Initialization-20261027/baseline-r3b-20261022-current.json` | 6618 | `8419d4549272a2fc048a8cf2739ae8dcf94ebea42e05ba1aa053c227fde164e3` |

---

## 3. 配置默认与未冻结项（从当前实际数据提取，不猜测）

| 字段 | 当前默认（Test / Not Frozen） | 提取来源 / 边界 |
|---|---|---|
| `InitialHeroId` | `hero_001` | `PlayerCollection` 既有默认；**未冻结英雄内容** |
| `Gold` | `100` | `InitializeDefaultCollection` 既有实际值 |
| `InitialIngredients` | `0` | `VillageResourceState` 既有默认（`Restore(null)` = 0） |
| `InitialFood` | `0` | 同上 |
| `People` | 当前 `DataManager.InitialPersonRoster` 的 5 个 `PersonDefinitionId`（顺序保持） | **不写死人数 / 名单**；当前实测 roster=5，且 5 条均通过 `PersonDefinitionValidator` |
| `Buildings` | **空列表（0）** | 当前产品 NewGame 的实际值就是“无初始建筑”（`ResetBuildingsForNewGame`）；也是既有 `BuildingConstructionProductPlayTests` 的断言（NewGame 后 `Save.Buildings.Instances.Count == 0`）。**本包不猜测初始布局 / 住房比例** |

未冻结声明：人口数量、房价/住房比例、Farm/Kitchen 数量与位置、资源初值、金币、英雄选择均**未冻结**；默认只表达“现在实际是什么”，可通过显式配置参数 / 未来的工作簿内容修改。默认提取路径不读写任何生成 Mirror，也不创建第二套配置链。

---

## 4. 权威与兼容决定

1. **单一权威链**：初始配置数据模型只消费既有 Authority —— PersonDefinition 初始名册 / `DataManager`、`VillageResourceState`、`BuildingRegistry`（`StaticConfigRegistry.TryGetBuildings`）、既有 `VillageAssignmentSystem` / `VillageWorldRegistry` / `VillageProjectSystem`。未新建并行配置链。
2. **真实实体创建复用既有 Building / Workplace 语义**：`BuildingRuntime.TryAddInitialCompleted` 复用 `BuildingPlacementValidator` 与同一实例分配器；实例岗位 / 资源岗位复用 `FunctionWorkplaces` / `FunctionResourceWorkplaces`，Capacity 仍由 `VillageWorkplaceDef` 单一权威；Housing 定义没有 `BuildingFunctionBinding`，因此**自然不生成岗位**。
3. **NewGame 单次 / 事务式**：只有 `SaveSystem.NewGame(config)` 提交初始世界；`Continue` 仍走既有 `Load`（恢复存档实例 / 职责 / 资源），**不重跑配置**；`Town` / `EnsureInitialized` / `ResetForTests` 不创建初始建筑或产品内容（新增测试覆盖）。
4. **零半状态**：结构校验 → 内容 / 占地校验 → 候选世界准备（纯内存）全部成功后才创建 PlayerCollection 并提交；任一失败返回 `null` + 明确 `Debug.LogWarning`，当前世界逐字节不变。
5. **SaveVersion / DTO 未变**：初始配置不进入 `SaveData` / `VillageSaveData` / 任何 DTO 字段；`CurrentSaveVersion = 1` 与旧档兼容路径不变。
6. **英雄选择时序**：`NewGame` 仍只创建一次默认英雄；`HeroSelectionUI.OnConfirm` 的既有“清空默认英雄 → 加入所选英雄”时序未改。新增测试按同一时序验证：替换后仅 1 个英雄、新 PersonId 非空、其它人物 PersonId 不变，Save→Continue 后英雄 PersonId 不丢失。
7. **旧静态岗位 / 旧档兼容保留**：`ws_farm_01` / `ws_kitchen_01` / `ws_test_*` 静态岗位仍在默认目录；旧无 `Buildings` 档不补建初始建筑，静态职责仍可恢复（新增测试覆盖）。旧 `ws_*` 继续同时作为 profile 与 Legacy assignment 身份保留，**本包未删定义、未解禁旧静态派工、未改 Registry 的 workshop 旧档**。
8. **住房表现**：当前产品住房表现仍来自 Presence 派生的每 Person `Home:*` 锚点（数量 = 当前人物数量），本包**未改展示层**；显式配置创建的 House 实例只进入 Building 权威，不与 `Home:*` 合并、不复制为第二套住房状态。正式默认无 House 实例，因此当前产品表现零变化；展示去重 / 与实例同权威属整包后续必须项（§7）。
9. **未降低既有断言**：默认 NewGame 仍为 5 人 / hero_001 / Gold 100 / 资源 0/0 / 0 建筑 / 旧静态岗位；未修改任何既有测试文件或其断言。

---

## 5. 编译发现（真实执行）

| 检查 | 真实结果 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ForceSynchronousImport)` → `refresh-requested` |
| 编译状态 | `isCompiling=false`、`isUpdating=false`、`isPlaying=false`（刷新后复核） |
| Console `error CS` | **0 条** |
| 本包相关编译警告 / 错误 | Console 中 `InitialWorld` / `PlayerCollection` / `VillageRuntimeBridge` 相关告警 = **0 条** |
| 测试发现 | `tests \| where fullName contains "InitialWorldConfigurationTests"` → **10 条 EditMode**（`InitialWorld.Tests`） |
| 场景污染 | `SampleScene isDirty=false` |
| 字体 / 存档复核 | 字体 Hash 仍 `3c758e16…`、9 个子资产 clean；用户存档仍 `13134B / adcf5d8b…`（本包未写档） |
| 既有 34 项复核（写入 / Refresh / 冒烟之后） | 再次只读复算：`same=34, changed=0, missing=0`（04:56Z） |

**非测试运行态冒烟（`script-execute`，非 `tests-run`；完成后 `finally ResetForTests`）**，实测输出：

```text
default-validate=True: people=5 buildings=0 gold=100
default-newgame=True chars=5 heroes=1 gold=100 buildings=0 ing=0 food=0
explicit-validate=True:
explicit-newgame=True chars=3 gold=77 ing=3 food=2
building building_instance_1 building_farm @-4,-6 Completed project=null
building building_instance_2 building_kitchen @0,-6 Completed project=null
building building_instance_3 building_house @4,-6 Completed project=null
farmDef=2 kitchenDef=1 houseDef=null
assignFarm=Success assignKitchen=Success assignHouse=UnknownWorkplace
worldPlaces=6 workplacePlaces=6
invalid-newgame-null=True worldUnchanged=True
```

该冒烟只证明“当前源码在 Editor 中可按上述路径运行”，**不替代**宿主对新 Edit 整类的正式 check，也不构成 PlayMode / 真实入口验收。
冒烟中故意提交的重叠布局产生且仅产生一条预期告警（`[SaveSystem] NewGame rejected: … 占地与已有建筑重叠`），无异常、无 `error CS`；随后 `finally ResetForTests` 已复位运行态。

---

## 6. 新增测试类覆盖（`RRPG.Initialization.Tests.InitialWorldConfigurationTests`）

| # | 测试 | 覆盖要求 |
|---|---|---|
| 1 | `PureConfigurationValidationRejectsStructurallyInvalidInputs` | 纯配置验证：null / 负金币 / 负资源 / 空英雄 / null 人口 / 重复人口 / null 建筑 / 空定义 / NaN / Infinity |
| 2 | `ContentAndLayoutValidationRejectUnknownOrIllegalReferences` | 内容引用：未知 PersonDefinition / Hero / BuildingDefinition；初始布局重叠 / 越界；合法布局通过 |
| 3 | `NewGameCreatesConfiguredPopulationGoldAndResources` | 初始人口 / 英雄 / 金币 / Ingredients / Food 由配置创建；PersonId 真实且唯一 |
| 4 | `NewGameCreatesCompletedFarmKitchenWorkplacesAndHousingWithoutWorkplace` | 真实 Completed Farm / Kitchen / House；实例 Workplace 容量；Farm/Kitchen 可派工；House 无岗位且派工被拒；资源岗位 Role 正确 |
| 5 | `NewGameIsSingleShotAndRepeatableWithoutDuplicateInstances` | NewGame 单次 / 重入：不累加建筑 / 人口 / 资源，实例分配器从 1 重新开始，不残留第三次岗位 |
| 6 | `ContinuePreservesInstanceIdentityPlacementAssignmentAndResources` | NewGame → 派工 → Save → 另一配置扰动 → Load：实例 ID / 定义 / 位置 / Completed / 职责 / 资源 / PersonId 保持 |
| 7 | `LegacySaveWithoutBuildingsDoesNotRebuildInitialWorldAndKeepsStaticAssignments` | 旧档移除 `Buildings` 后 Continue：不补建初始建筑；静态 `ws_farm_01` 职责保留 |
| 8 | `InvalidConfigurationLeavesCurrentWorldUnchangedAndReportsFailure` | 重叠 / 未知人口 / 越界均返回 null；世界 JSON 逐字节不变；既有建筑 / 职责保持 |
| 9 | `TestResetAndEnsureInitializedDoNotSeedProductWorld` | 测试重置 / `EnsureInitialized` 不种产品内容；默认提取 = 0 建筑、人口数 = 当前名册、Gold 100、资源 0/0 |
| 10 | `HeroReplacementAfterNewGameKeepsSingleHeroAndPersonIds` | Hero 替换时序：不重复初始化、只 1 个英雄、新 PersonId、其它人物 PersonId 不变，Continue 后不丢失 |

**存档保护**：测试类内实现 `SaveFileGuard` —— 每例开始把用户单槽**真实字节持久快照**到
`Development/StudioRuns/Initialization-20261027/save-guard/<UTC>/original-save.bin` + `manifest.json`（`status=captured`），
`TearDown` 的 `finally` 写回并回读 SHA256 校验后置 `status=restored`；发现任何非 `restored` 的未完成 run 即阻断新测试并要求人工核对，不自动清记录、不覆盖他人快照。
**本轮 2026-10-28 已把 `Capture` 加强为 CreateNew 唯一目录 + 备份回读字节/SHA 校验后才 `captured`，详见 §10。**
既有 `SaveSlotGuard` 是 `ManagementShellPlayTests`（PlayMode 程序集）内的私有实现，跨程序集不可直接复用；本类按其既有语义（持久备份 + 回读校验 + 未完成 run 阻断 + `finally` 恢复）实现等价 guard，不新建第二套存档写入路径。
**最初实现轮未运行该测试类**；宿主随后已实际运行并产生 10 个 `restored` guard run（当前无 `captured` / unfinished，见 §10）。

---

## 7. 待生成项 / 整包后续必须项（本包不省略、不冒充完成）

1. **正式 Authoring（待生成，未做）**：新增初始世界工作簿（建议 `Development/Config/RRPG_InitialWorld.xlsx`：`InitialWorld` / `InitialPopulation` / `InitialBuilding` 三张表，按现有行 1–4 表头规则），并由 `RRPG.ConfigCompiler` 生成同名 `Development/Generated/StaticConfig/*.json`、Unity Builder 生成数据库、`StaticConfigRegistry` 暴露只读入口；**禁止手写 Mirror**，也**未**在 `Development/Generated/StaticConfig` 放任何假装生成的 JSON。本包受控工具无法安全编辑 XLSX / 运行 Compiler，故只完成代码 / 测试与同一模型的默认提取。
2. **默认初始布局内容**：当前默认 0 建筑是“当前实际值 + 保护既有断言”；正式 Farm / Kitchen / Housing 初始布局需要制作人在工作簿中给出（数量 / 位置 / 住房关系均 **Not Frozen**），本包不猜测、不冻结。
3. **住房表现去重与同一权威**：把每 Person `Home:*` 展示锚点收敛到真实 Housing 实例（保持当前数量、不生成双份），需要专门后续包；本包只保留现状并记录余项。
4. **正式入口 workshop 下线 / 旧静态岗位退出默认**：`ws_*` 静态岗位仍为默认（旧档兼容），正式 `Catalog` / 生产 `Choose` 入口后续必须拒绝未确认类型；本包不删定义、不解禁旧静态派工，也未把这些后续项算作完成。
5. **展示接线最小必须部分**：本包未改 `ProductHomeReadAdapter` / `HomeWorld*`；显式配置创建的 House 若直接进入产品展示，会与 `Home:*` 锚点并存（仅测试配置场景；生产默认无 House），去重属第 3 项。
6. **宿主验证**：`InitialWorld.Tests` 整类、既有 EditMode / PlayMode 回归、真实 NewGame / Continue UI 链路由宿主 Studio 阶段 check 执行；本文件不作通过声明。

---

## 8. 边界与未验证项（明确记录）

- 最初实现轮**未运行**任何 Unity Test Runner 用例（含新类），**未**做 PlayMode / 真实 UI 入口验收；宿主随后已运行并给出 9/1 回执（见 §10），所有运行结论仍以宿主回执为准。
- 本包**未**读写用户存档（只读捕获，未写；测试类运行时会由 guard 保护），**未**恢复任何历史档。
- 本包**未**使用 Git 能力；HEAD 为宿主提供值，基线为只读文件哈希，不声称原子快照。
- 本包**未**修改任何既有测试文件，未降低断言；既有原 34 项基线文件哈希全部未变。
- 本包**未**新增正式内容 / 数值冻结；所有新增默认值均 `Test / Not Frozen`。
- 本包**不**代表制作人批准；实施与证据须由主开发专业核验。

---

## 9. Artifact 清单

| 路径 | 说明 |
|---|---|
| `Assets/Scripts/Village/Runtime/InitialWorldConfiguration.cs` | 统一配置模型 + 纯结构校验 |
| `Assets/Scripts/Initialization/InitialWorldConfigurationCatalog.cs` | 默认提取 + 内容 / 布局校验（唯一读取入口） |
| `Assets/Scripts/Village/BuildingRuntime.cs` | internal Completed 初始实例窄入口 |
| `Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs` | 候选世界准备 / 一次性提交 |
| `Assets/Scripts/Core/PlayerCollection.cs` | 配置驱动人口 / 金币初始化 |
| `Assets/Scripts/Save/SaveSystem.cs` | `NewGame()` 默认 + `NewGame(config)` 正式路径 |
| `Assets/Tests/EditMode/Initialization/InitialWorld.Tests.asmdef` | 新测试程序集 |
| `Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` | 10 例 Edit 测试 + 存档 guard |
| `Development/StudioRuns/Initialization-20261027/baseline-r3b-20261022-current.json` | 34 项只读基线复算 |
| `Development/StudioRuns/Initialization-20261027/initialization-foundation-implementation.md` | 本文件 |

本文件及实现需由主开发专业核验；**不代表制作人批准**，也不表示新初始化整包完成。

---

## 10. 2026-10-28 第一层唯一失败与存档 guard 修复附记（本轮，短）

### 10.1 口径更正（优先于上文旧句）

- 上文 §5 / §6 / §8 的“本包未运行该测试类 / 未运行任何 Test Runner”只描述最初实现轮；**宿主此后已实际运行**，不是“未执行”。
- 宿主原回执：`708ba490-a0ba-461e-a46c-1d70128dc095`，EditMode `InitialWorldConfigurationTests` **10 总 / 9 Passed / 1 Failed / 0 skipped**，run 级 `1.5051377 s`，`scope-restored|font|0|632`。
- 失败例 = `TestResetAndEnsureInitializedDoNotSeedProductWorld`，`NullReferenceException`；本机只读 `testResults` 当前保留一条同方法失败记录：`Failed / 8 ms / System.NullReferenceException : Object reference not set to an instance of an object`，stack 指向修复前测试第 **344** 行 `Assignments().WorkplaceDefs`。该记录是否即宿主同一 run 无法由本层证明（Studio runId 不可读），但栈已足以定位；宿主原回执“stack 字段为空”与本机保留记录不互相冒充。

```text
System.NullReferenceException : Object reference not set to an instance of an object
  at RRPG.Initialization.Tests.InitialWorldConfigurationTests.TestResetAndEnsureInitializedDoNotSeedProductWorld ()
     [0x00029] in .../Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs:344
  at (wrapper managed-to-native) System.Reflection.RuntimeMethodInfo.InternalInvoke(...)
```

- 默认 **0 建筑是当前未完成的过渡值**，不是正式设计，也不是“保护旧断言”的理由；正式人口 / 建筑 / 资源数量可在测试配置中修改，**无需制作人另定数量**。`XLSX / Compiler 链、住房同权威、旧静态岗位退出默认、workshop 正式下线`仍是整包必须项，**不需要新授权**，本包不把它们标成完成或另需授权。

### 10.2 根因与修复（只改失败根因，不改产品默认内容）

| 项 | 结论 / 改动 |
|---|---|
| 真实契约 | `VillageRuntimeBridge.ResetForTests()` 明确 `Time/State/Assignments/Projects/Resources/World = null`（源码第 1389–1401 行）。Reset 后就是**未初始化**，对 `Assignments` 解引用必然 NRE，不是产品缺省内容 bug。 |
| 测试修复 | 失败例先断言 Reset 状态：`Assignments == null`、`Resources == null`、Buildings 为空；随后 `EnsureInitialized()` 才断言 `Assignments != null`、无 `workplace:` 实例岗位、资源 0/0。保留“不生成产品内容”的强断言与原 9 例通过预期。 |
| `SetUp` 顺序 | 改为 **先 `SaveFileGuard.Capture()`，再 `ResetRuntime()`**；任何可能写档的初始化之前已持有用户单槽快照。 |
| guard 差异（旧→新） | 旧 `Capture` 写 `original-save.bin` 后**未回读**即写 `status=captured`；新实现使用唯一目录（时间戳 + GUID 短码），原档备份 `FileMode.CreateNew`，**回读字节逐字节 + SHA256 双校验成功后才写 `captured`**；失败写 `capture-failed`（保留原 manifest 字段、不清洗、不删除本次 run）并上抛阻断。`finally` 恢复与“未完成 / 未知 status 阻断新测试”逻辑完整保留；未知状态不自动恢复、不自动删除。 |
| 未改 | 未改 SaveVersion / DTO / 正式人口·建筑·资源默认 / 产品 NewGame 内容；未改既有 9 例断言。 |

### 10.3 本轮只读核验（真实值）

| 检查 | 实际结果 |
|---|---|
| 用户当前档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，`13134 B`，`adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`（与修复前参考值一致，未被本轮写改） |
| Initialization guard | 10 个 run，全部 `restored`，无 unfinished；10 个 `original-save.bin` 均为 `13134 B / adcf…`，与当前用户档 **match=True** |
| 既有 R3B guard | 310 run = **309 restored + 1 protective-snapshot**，无 unfinished / 未知状态被改动 |
| 字体 | `14961033 B / 3c758e16…c404854`，9 个 sub-asset `dirty=False`（未做字模测量） |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` |
| 编译 / 发现 | 安全 `AssetDatabase.Refresh(ForceSynchronousImport)` 后 `isCompiling=false / isUpdating=false / isPlaying=false`；Console 现有 **3 条 Error/Exception 全部为本轮只读哈希 snippet 自身的 `CS1002` 编译错误**（`sequence 84269–84271`，非项目 / 测试程序集错误，保留不清洗）；`InitialWorld.Tests.dll`（`31744 B / 2026-10-28T05:00:21Z`）晚于测试源码（`40624 B / 05:00:18Z`，fresh=True）；`tests` 发现 **10 条 EditMode**（`InitialWorld.Tests`） |
| 本轮测试文件指纹 | `InitialWorldConfigurationTests.cs`：`40624 B / 42ae36d9d9698c379b4db2a4902712ceb754dd3f844396f34e8b8a11d944e160` |

### 10.4 明确待宿主

- 本轮**未运行** `tests-run`，上述“10 条发现”只是编译后测试发现，不是通过声明；宿主需重新运行**整类** `RRPG.Initialization.Tests.InitialWorldConfigurationTests`（10 例）并给出新回执。
- 旧 `testResults` 中 `9 Passed / 1 Failed` 仍是修复前记录；在新宿主回执前，不得把本附记当作该失败例已通过。
- 本附记只覆盖第一层唯一失败根因与 guard 可靠性；整包其余必须项按 §7 留待后续包，本包不重做、不越界。

