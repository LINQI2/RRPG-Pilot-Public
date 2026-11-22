# Initial World 第二层 · 正式配置生产链与读取接线（实施记录）

- 仓库 / 唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`
- 阶段：**1/1 —— 正式 Authoring → Compiler → Mirror → Builder → Registry → Catalog 配置框架与读取接线**（本文件只覆盖这一层，不推导整包完成）
- 工作区：workspace project（`RRPG_AI_Pilot`，Unity `2022.3.62t9`，当前场景 `Assets/Scenes/SampleScene.scene`）
- 结论口径：**本包是 artifact 交付 + 宿主待执行生成**。文件存在性、哈希、编译发现均为本轮实测；**未运行任何 Unity 测试**，不预填 PASS/FAIL。

---

## 0. 前置核验（真实值）

| 项 | 实际值 | 结论 |
|---|---|---|
| CWD | `D:\AI_Agent_Lab\rrpg_pilot\repo` | ✅ 授权目录 |
| Unity 工程 | `D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game`，productName `RRPG_AI_Pilot` | ✅ 本任务工程；未读写正式工程 |
| 编辑器状态 | `isPlaying=false` / `isCompiling=false` / `isUpdating=false` | ✅ 开工与收尾均空闲 |
| 场景 | `SampleScene`，`isDirty=false` | ✅ 未保存 / 未污染 |
| 字体 | `14961033 B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；9 个 sub-asset `dirty=0` | ✅ 与既有保护值一致；未做字模测量 |
| 用户存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，`13134 B` / `adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08` | ✅ 未写档 |
| Initialization guard | `Development/StudioRuns/Initialization-20261027/save-guard`：20 run = **20 restored**，0 unfinished / 0 unknown | ✅ |
| R3B guard | `Development/StudioRuns/R3B-SaveProtection-20261024`：310 run = **309 restored + 1 protective-snapshot**，0 unfinished | ✅ 与第一层记录一致 |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0`（开工与收尾各一次） | ✅ 未改既有 dirty / 基线文件 |
| 现有生成状态 | `Development/Config/RRPG_InitialWorld.xlsx` = false；`Development/Generated/StaticConfig/InitialWorld*.json` = 0；`InitialWorldConfigDatabase.asset` = false | ✅ 本包**没有**手写 / 伪造生成物 |

实现方式：文件读写 + `rql-query` 只读 + `script-execute` 安全 `AssetDatabase.Refresh(ForceSynchronousImport)` 编译；
**本 Agent 未调用** Shell / Process / Git / `tests-run` / `rql-mutate` / `SaveAssets` / `SaveChanges` / `SetDirty` / `ClearDirty` / PrefabStage / Prefs / 启停 Editor / 字体测量 / 子 Agent。Builder 源码内部沿用既有 `EditorUtility.SetDirty`（只标记本领域生成资产，见 §2.9）。

---

## 1. 真实落盘（新增 / 修改 + 字节 + SHA256）

### 1.1 Compiler / 测试（Tools）

| 文件 | 状态 | 字节 | SHA256 |
|---|---|---|---|
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/InitialWorldValidator.cs` | 新增 | 12163 | `b2552ddd1e57579283e018c746c2f014b58073c07d39714726b270a34cdb13ef` |
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/InitialWorldWorkbookSeed.cs` | 新增 | 15313 | `a22d2b123a63c4ec7b3164ff7e88fc4416e8c3491f76e0128313d245c6f14016` |
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/DiagnosticCodes.cs` | 修改 | 10719 | `3233f327896c53377c877e810f5c0cd565aa332428d9cd4bfa83b192d0d1da43` |
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/EntityRequirements.cs` | 修改 | 5797 | `a23ea471e0d09d4d1420eb87190446853d4f4ccfd1afd2432f316592c50b13ed` |
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/CompilerRunner.cs` | 修改 | 11419 | `8ea44da54cceec177e2c0e613489aac9184c0b12258c5c6c4033d7eb701e8821` |
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/PrimaryErrorSummary.cs` | 修改（新增 5 条修复建议） | 9354 | `e75149314275d816dde67e7367b72f186eaf456b9c22712741e888095de27187` |
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Program.cs` | 修改 | 12280 | `1a7b13088565e096f9f457de578e5fffad30349cf441aa54c629ac7653c16085` |
| `Tools/RRPG.ConfigCompiler/tests/RRPG.ConfigCompiler.Tests/InitialWorldConfigTests.cs` | 新增 | 13673 | `1bbfd8995d3cf9da6185e1ccd32c458d022c74e288f37dc126290dcc352afc3c` |

### 1.2 Unity 运行时 / Builder / 测试

| 文件 | 状态 | 字节 | SHA256 |
|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/StaticConfig/InitialWorldConfigDatabase.cs` | 新增 | 15632 | `ade5f27ca0c9f1891c9b028890775b03f421dd77835b76d0d37606a5a389f32a` |
| `UnityProject/RRPG1.0game/Assets/Scripts/StaticConfig/RRPGStaticConfigRoot.cs` | 修改 | 5997 | `61ae31f4c67088679698c5a1a8eb9b4fe2cf376d75219ddee72d6f263b77a18a` |
| `UnityProject/RRPG1.0game/Assets/Scripts/StaticConfig/StaticConfigRegistry.cs` | 修改 | 33717 | `a69cc2981874003238920a75dc7033856ca9f8c6642a554995027e607999d25f` |
| `UnityProject/RRPG1.0game/Assets/Scripts/Initialization/InitialWorldConfigurationCatalog.cs` | 修改 | 7974 | `2bb7a00dcbd94b1431c65f787dfc8302cb4ef46b4911f5583abd278f09be8381` |
| `UnityProject/RRPG1.0game/Assets/Scripts/Core/PlayerCollection.cs` | 修改 | 24414 | `e21688bf00670bae08ecf318fe4dc10895afda636aa84c0ebd644d3a81e8b222` |
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/Runtime/InitialWorldConfiguration.cs` | 修改（仅注释口径） | 6279 | `8b568303afe1848cb4983f9fde9f4bc659aca8f25c87f12d6e950721fbae08d6` |
| `UnityProject/RRPG1.0game/Assets/Editor/StaticConfig/InitialWorldMirrorReader.cs` | 新增 | 7106 | `0722d342b8ac56a237b8e25ffd3e27376b588f69fad9fae74cf0e47a2e1373b7` |
| `UnityProject/RRPG1.0game/Assets/Editor/StaticConfig/StaticConfigBuilder.cs` | 修改 | 62215 | `596203d264208513dc74663e0bd2be9c503da331bd9e11f83c92576cff265f89` |
| `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Initialization/InitialWorld.Tests.asmdef` | 修改（+`StaticConfig.Runtime`） | 597 | `3064b0454dbdc6fa570f4703c95e6df0853a298b480049b72140214a82feda42` |
| `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` | 修改（#9 改来源一致性） | 43378 | `f89ca9b89f1dd1b79e69b715e452eb131b166f5c4f1d713ec68c7539783c7bf0` |
| `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Initialization/InitialWorldConfigPipelineTests.cs` | 新增 | 20247 | `4129ce54c71199cffa08a3f427a210710147b9b1ccef87fb0fef3fc51acf7baa` |

新增 `.meta` 由 Unity Refresh 自动生成（3 个新代码文件 + 1 个测试文件），未手写生成资产。
文档同步：`Development/Config/README.md`（22465 B / `6e7dbf67…`）、`Development/Generated/StaticConfig/README.md`（7912 B / `3cefa96d…`）。

---

## 2. 实际接线的数据链

```text
Development/Config/RRPG_InitialWorld.xlsx（人工 Authority；本包提供一次性 seed 命令，尚未执行）
  → RRPG.ConfigCompiler（通用 Schema/Data/CrossReference + InitialWorldValidator）
  → Development/Generated/StaticConfig/{InitialWorld,InitialPopulation,InitialBuilding}.json（Canonical Mirror，禁止手写）
  → Unity Builder（InitialWorldMirrorReader → InitialWorldConfigDatabase）
  → SelectedConfigRoot.InitialWorldDatabase
  → StaticConfigRegistry.TryGetInitialWorld
  → InitialWorldConfigurationCatalog.TryReadDefault / TryBuildFromGenerated（深拷贝 + 跨域校验）
  → SaveSystem.NewGame
```

关键语义：

1. **三表齐全**：`InitialWorld` / `InitialPopulation` / `InitialBuilding` 只存在一部分 → `DAT048_INITIAL_WORLD_DOMAIN_SHAPE` Hard Error；Mirror 0 更新。
2. **默认 profile 唯一**：`InitialWorld` 必须恰好 1 行 `IsDefault=TRUE`，否则 `DAT049`；`ProfileId` / `HeroId` 非空。
3. **稳定序号 / 人物重复**：`StableIndex` 由通用 `key` 保证唯一且 `>=0`；另校验 `PersonDefinitionId` 不重复（`DAT050`）。
4. **未知建筑引用**：`InitialBuilding.BuildingDefinitionId` 使用 `ref:BuildingDefinition`，未知引用由既有 `DAT008` 拒绝。
5. **不合法范围**：负数金币 / Ingredients / Food（`DAT052`），负数序号 / 非有限坐标（`DAT051`）。这些检查不依赖 Row 3 的表格规则，是第二道防线。
6. **跨域 PersonDefinition**：Compiler 不伪造引用表；`InitialWorldConfigurationCatalog.TryValidate` 用 `DataManager` 校验真实人物 / 英雄，用 `BuildingPlacementValidator` 校验布局。
7. **不静默 0 建筑**：Mirror 缺失 → Builder 生成空容器；Registry 返回 false + `InitialWorldConfigEmptyDiagnostic`；Catalog 返回 false + 明确诊断；`CreateDefault()` 改为 fail-closed 抛出。
8. **防外部变更副本**：`InitialWorldConfigDatabase` 只暴露 `CopyProfiles / CopyPopulation / CopyBuildings / TryCopyDefaultProfile`，全部深拷贝；Catalog 每次返回独立模型。
9. **Builder 保存收窄**：旧的全局 `AssetDatabase.SaveAssets()` 已替换为对本次生成资产的 `SaveAssetIfDirty`（Root / Monster / Presentation / Catalog / Encounter / Event / AutonomousLife / Social / Building / VillagePresentation / InitialWorld）；`Exit()` 不再全局保存。Builder 内部沿用既有 `EditorUtility.SetDirty` 仅标记本领域生成资产（与其它领域同一模式）；本包**没有**对任何非生成资产 / 场景 / 用户档调用 SetDirty。

---

## 3. 默认内容（Test / Not Frozen；由一次性 seed 命令写入 XLSX）

| 项 | 内容 | 来源 |
|---|---|---|
| 默认 profile | `iw_default_test`（`IsDefault=TRUE`） | 新工作簿 Test 内容 |
| 初始英雄 | `hero_001`（青鹿） | 当前 `DataManager.GetHeroById` 实际值 |
| 金币 / Ingredients / Food | `100 / 0 / 0` | 当前实际默认值 |
| 初始人口 | `person_lin_en`、`person_su_wan`、`person_han_zhao`、`person_qiu_shuang`、`person_wei_an`（StableIndex 1–5） | 当前 `DataManager.InitialPersonRoster` 实际名册 |
| Farm / Kitchen | `building_farm @ (-6,-1.2)`、`building_kitchen @ (-2,-1.2)` | 当前 Home 展示中 `ws_farm_01` / `ws_kitchen_01` 的非住房锚点（`ProductHomeWorldHost` prototype 槽位） |
| 住房 | `building_house` × 5，`y=4.2`，`x=-8.5,-5.1,-1.7,1.7,5.1` | 当前 Home 展示的 5 个住房锚点；**数量与位置保留现表现**，不新增容量规则 / 比例 |
| 不含 | `building_workshop` / 铁匠铺 | 按本包要求，workshop 不进入默认 / 正式入口 |

以上数量、坐标、资源、住房均 **Test / Not Frozen**，后续直接改 XLSX 即可，不需要制作人另定数量。

---

## 4. Compiler 测试（新增，尚未执行）

`Tools/RRPG.ConfigCompiler/tests/RRPG.ConfigCompiler.Tests/InitialWorldConfigTests.cs`：9 个测试方法（14 个用例），通过公开 `CompilerRunner` 行为验证，不复制实现：

- 合法三表 → 三份 Mirror、字节确定性、`validate` 通过；
- 缺任一表 → `DAT048`，Mirror 0 更新；
- 0 / 2 个默认 profile → `DAT049`；
- 负资源（故意去掉 Row3 `>=0`）→ `DAT052`；
- 重复人物定义 → `DAT050`；负稳定序号 → `DAT051`；
- 空人口 / 空建筑表 → 明确失败；
- 未知 `BuildingDefinition` → 既有 `DAT008`；
- 没有本领域表 → 其它工作簿不受影响、无本领域 Mirror。

**未运行**：本机没有 Shell / Process，且编译器测试属于 `dotnet test` 宿主执行；不得用 Unity 进程替代。
因此 `Tools/` 下的新增 Compiler 代码在本轮**只做静态落盘与人工复核，未经 dotnet 编译验证**；Unity 侧代码已由 Editor 实际编译通过。

---

## 5. Unity 测试（新增 / 修改，尚未执行）

新增 `Assets/Tests/EditMode/Initialization/InitialWorldConfigPipelineTests.cs`（5 例）：

1. `GeneratedContainerReadbackIsDeepCopiedAndOrdered`：容器读回、稳定序号排序、深拷贝防污染；
2. `IllegalGeneratedContentIsRejectedBeforePublishing`：空 / 多默认 / 负资源 / 重复序号 / 重复人物 / NaN 坐标均拒绝，`ApplyGeneratedContent` 抛明确异常；
3. `EmptyGeneratedContainerIsRejectedByCatalogAndRegistry`：空容器 → Registry / Catalog fail-closed，诊断含「为空」，**不**返回 0 建筑成功；
4. `DefaultReadComesFromGeneratedContainerAndIsIndependentPerCall`：默认读取的 hero/gold/资源/人口/建筑全部来自生成容器；两次读取互不影响；
5. `ExplicitConfigurationDoesNotReadGeneratedDefault`：把默认容器设为完全不同的内容，`NewGame(explicitConfig)` 仍只使用显式人口 / 金币 / 资源 / 建筑。

修改 `InitialWorldConfigurationTests` 第 9 例：保留 Reset / EnsureInitialized 不生成产品内容的断言；默认部分改为**来源一致性验证**（Registry/生成容器 vs Catalog 返回的模型），断言默认建筑非 0、包含 farm/kitchen/house、不含 workshop，不再固定人数 / 金币 / 0 建筑。

当前 Unity 编译后测试发现：**15 条 EditMode**（10 旧 + 5 新），见 §6。**未运行** `tests-run`。

---

## 6. 编译发现（真实执行）

| 检查 | 真实结果 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ForceSynchronousImport)` → `refresh-requested`（两次） |
| 编译状态 | `isCompiling=false` / `isUpdating=false` / `isPlaying=false` |
| Console 新增错误 | `sequence > 84271` 的 `Error` = **0 条**；既有 3 条仅是本包开始前只读 snippet 的 `CS1002`（未清洗） |
| 测试发现 | `tests | where fullName contains "InitialWorld"` → **15 条 EditMode**（`InitialWorld.Tests`） |
| 场景 | `SampleScene isDirty=false` |
| 受保护值复核 | 字体 / 存档 / 20+310 guard / 34 项基线在收尾时仍全部一致（见 §0） |

---

## 7. 已生成 / 待生成清单（不虚称）

| 项 | 状态 |
|---|---|
| Compiler 代码 / 领域校验 / 一次性 seed 命令 | ✅ 已落盘 |
| 编译器测试代码 | ✅ 已落盘（未执行） |
| Unity 生成容器 / Builder 接线 / Registry / Catalog | ✅ 已落盘，Unity 编译通过 |
| Unity 管道测试代码 | ✅ 已落盘（未执行） |
| `Development/Config/RRPG_InitialWorld.xlsx` | ❌ **未生成**（本机无 Shell / Process；由宿主执行 seed 命令） |
| `Development/Generated/StaticConfig/Initial*.json` | ❌ **未生成**（必须由 Compiler build 产生，禁止手写） |
| `Assets/Generated/StaticConfig/Databases/InitialWorldConfigDatabase.asset` | ❌ **未生成**（必须由 Unity Builder 产生） |

因此本包结束状态下：**默认 `SaveSystem.NewGame()` 会 fail-closed（明确诊断），不会静默恢复旧 hero/gold/空建筑，也还不是可玩状态**。这是刻意 gate，不是完成声明。

### 宿主执行步骤（按顺序）

```text
cd D:\AI_Agent_Lab\rrpg_pilot\repo

:: 1) 构建编译器（会编译新增 validator / seed / tests）
dotnet build Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj -c Release
dotnet test  Tools/RRPG.ConfigCompiler/tests/RRPG.ConfigCompiler.Tests/RRPG.ConfigCompiler.Tests.csproj -c Release --filter FullyQualifiedName~InitialWorldConfigTests

:: 2) 一次性生成 authoring 工作簿（目标已存在会拒绝，绝不覆盖）
dotnet run --project Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj -c Release -- initialworld-seed --root D:\AI_Agent_Lab\rrpg_pilot\repo

:: 3) 只读校验（不写 Mirror）
RRPG_检查配置.cmd
:: 或 dotnet run ... -- validate --root D:\AI_Agent_Lab\rrpg_pilot\repo

:: 4) 生成 Canonical Mirror + Unity 生成根（需要 Unity 关闭；先备份当前生成根）
RRPG_一键导表.cmd
:: Unity 开着时：菜单 RRPG/Static Config/M0 · Build Generated Root（等价 Builder）
```

保护要求：seed 只在目标不存在时执行；若维护者已有 `RRPG_InitialWorld.xlsx`，**不要**运行 seed，直接进入 validate/build。`RRPG_一键导表.cmd` 前应记录当前 `Development/Generated/StaticConfig` 与 `Assets/Generated/StaticConfig` 的文件哈希，生成后核对只新增本领域三份 Mirror + 一个数据库，其它 Mirror 哈希不变。

### 生成后回归

1. `InitialWorldConfigPipelineTests`（5 例）与 `InitialWorldConfigurationTests`（10 例）整类；共 15 例，预期全部通过；**以宿主回执为准**。
2. 既有 `StaticConfigBuilderTests`（Builder 签名 / no-op / rebuild / ownership）与既有 Village / Save / PlayMode 回归。
3. 真实 NewGame / Continue UI，确认默认 5 人、hero_001、100 金币、0/0 资源、farm/kitchen/5 house；旧档 Continue 不重建。
4. 字体、存档 guard、34 项基线复核。

---

## 8. 接线未完项（下一包必须做，不在本包虚称）

1. **住房展示同一权威**：当前产品住房表现仍来自每人 `Home:<personId>` 展示锚点；本包只生成 5 个 House 配置实例（与当前锚点同位置），尚未把 `Home:*` 锚点收敛到真实 Housing 实例。若直接进入展示，House 实例会与锚点并存 → 去重 / 合并属下一包。
2. **Farm / Kitchen 展示去重**：静态 `ws_farm_01` / `ws_kitchen_01` 展示锚点与新建实例同位置，需按同一权威合并；本包未改展示层。
3. **旧静态岗位退出默认 / workshop 正式下线**：本包默认内容已不含 workshop，但静态 `ws_*` 仍保留用于旧档兼容；正式入口隔离与旧静态岗位退出属后续。
4. **PersonDefinition 进同一 Compiler 语料**（如未来需要 ref 校验）需另开领域迁移；本包按授权做 Runtime 跨域校验，不伪造引用表。
5. **真实可玩验收**：本包只到「配置框架 + 读取接线 + 生成命令」，未做 PlayMode / UI 验收。

---

## 9. 边界声明

- 未运行 `tests-run`；§4 / §5 的测试代码只是 artifact，结果以宿主 Studio 阶段回执为准。
- 未手写 `Generated/Mirror`、未手写 `.asset`、未用占位冒充生成成功。
- 未改 SaveVersion / Save DTO；Continue 仍只恢复存档。
- 未新增住房容量规则 / 比例；未把数值标成正式平衡。
- 本文件与实现需由主开发专业核验；**不代表制作人批准**，也不表示新初始化整包完成。
