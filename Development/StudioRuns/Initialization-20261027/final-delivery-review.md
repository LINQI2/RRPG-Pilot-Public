# Initial World 初始化整包 · 最终实质自审与统一交付记录

- 唯一授权目录 / 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`，本轮实测）。
- 阶段：**1/1 —— 初始化交付实质自审 + Home Edit 必要回归**。
- **2026-10-29 增量（阶段 1/3，上一轮）**：厨房真实 UI 派工 / 加工 / 保存 / 继续专项的宿主交付前置——方法唯一发现、当前用户单槽实际捕获、恢复链与保护面复核、`UnitySetUp` 异常恢复窄修；并静态给出视觉绑定修复精确提案（§16–§17）。
- **2026-10-29 增量（阶段 2/3，本轮）**：Kitchen 专项**宿主回执 1/1 Passed（1845 ms）**；发现并窄修 Edit 九例的**跨守卫 manifest 格式不兼容**（源码级必然阻断）；交 `InitialWorldProductWiringTests` 九例宿主口径（§18）。
- **2026-10-29 增量（阶段 3/3，本轮）**：受影响旧 Play 六方法按新正式实体语义修订完成并编译；`BuildingConstructionProductPlayTests` 整类两例交宿主门禁，其余三类写准确 class / fullName 待后续精准门禁；另如实登记源码级新发现的同因受影响方法（§19）。
- 不另建任务、不重做已过实现、不进入后续阶段。
- 本文档口径：**Verification PARTIAL（尚缺宿主验证）/ 不 Completed / 未 Commit / 未 Push**。
  本文只登记本轮真实读取、真实落盘与已存在的宿主回执；**本轮没有运行任何测试**，新增 / 修改后的测试一律以宿主 Studio 阶段回执为准，**不预填 PASS**。
- 执行方式：文件 Read/Write/Edit、Glob/Grep、`rql-query` 只读、`script-execute`（仅只读测量 + 2 次安全 `AssetDatabase.Refresh(ForceSynchronousImport)`）、`read_image` 读取既有截图。
- **未使用**：Shell / `Process` / `tests-run`（含单方法、整类）/ `rql-mutate` / `AssetDatabase.SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `ClearDirty` / 字体或场景 `SetDirty` / 全局保存 / Preferences / PrefabStage / 正式工程 / 启停 Editor / 业务冒烟（NewGame、派工、UI 探针、渲染探针、字体测量）/ 额外付费子 Agent / Workflow / Goal。
- 保留：既有 dirty / untracked、34 项基线、字体、用户单槽、guard 历史、历史违规与失败记录一律不删不改不洗白；**未执行任何 Git 写操作**。

---

## 0. 制作人确认范围归档（不是独立 GameSpec）

### 0.1 来源事实（如实归档，不冒充 Authority）

| 项 | 事实 |
|---|---|
| 确认编号 | `69d2bd2e` —— **来自对话消息（conversation message），不是仓库既有设计文档** |
| 仓库内唯一既有记录 | `Development/StudioRuns/Initialization-20261027/initialization-foundation-implementation.md` 第 5 行：「范围来源：制作人 `69d2bd2e` 的确认来自本次任务消息（conversation message），不是仓库原有设计文档；本文件不把消息来源冒充为既有 Authority。」 |
| 独立 GameSpec | **仓库中不存在** Initial World 的独立 GameSpec / Authority 文档；**本记录不声称存在**，也不以实施报告代替规则 Authority |
| 原始消息正文 | **未在本工作区留存**，本层无法逐字引用；可归档的是已落盘的实施转述与当前委托原文 |

**结论**：任何把「初始化整包」当成 `GameSpec` 或「已冻结规则」的表述都是错误的。玩家长期规则仍只从 [GameSpec/README.md](../../../GameSpec/README.md) 定位。

### 0.2 已落盘的范围转述（各阶段报告原文位置）

| 阶段报告 | 归档的范围口径（位置） |
|---|---|
| `initialization-foundation-implementation.md` | §2.1–§2.2（配置模型 / Catalog / NewGame 事务式提交）、§3（默认值 Test/Not Frozen，人口与住房数量不冻结）、§7（Authoring XLSX / Compiler 链、住房同权威、旧静态岗位退出、展示接线属整包必须项） |
| `config-pipeline-implementation.md` | §2（唯一数据链 `XLSX → Compiler → Canonical Mirror → Builder → Registry → Catalog → NewGame`）、§3（默认内容 `iw_default_test`：hero_001 / Gold 100 / 0-0 资源 / 5 人口 / Farm+Kitchen+5 House，**不含 workshop**）、§7（禁止手写 Mirror） |
| `unity-generated-pipeline-review.md` | §1（主开发 Compiler 宿主回执 `compiler-host:8e7758c9…`：280/280、0 skip）、§3–§5（Builder 生成真实数据库与变动面）、§4（Profile / 人口 / 建筑稳定顺序） |
| `product-entity-wiring-review.md` | §3（正式 NewGame / Continue-Legacy 分流）、§4（显示同一实体去重 + 工坊正式下线）、§5（5 例 Edit）、§8（未验证项） |
| `product-play-integration-review.md` | §3（Load 目录只从存档可证明来源）、§4（住房显示别名契约与局限）、§5/§10/§12（产品入口 Play 专项、Home 3 例夹具、证据） |
| `management-resume-review.md` | §2–§5（现场恢复、管理 17 例源码 / 发现 / 夹具只读复核）、§7（宿主整类口径） |

### 0.3 本轮委托原文（操作性授权，逐字归档）

> 「进入原初始化整包最终自审与统一交付，不另建任务不重做已过实现。」
> 「本包先做实质自审而非只复制绿表：读取制作人69d2bd2e确认范围、报告与实际实现逐项映射」
> 「真实farm/kitchen/house能否显示正确已有美术、无重复旧Place/假workshop，不接受只有实体数据绿但默认全中性白模却宣称保留住房表现」
> 「XLSX需固定Compiler生成不得手写Mirror，无法该包安全生成则明确待项不可伪完成。」
> 「对原授权农田厨房都可用的要求…若缺则增加1个聚焦InitializationProductPlayTests厨房真实UI专项」
> 「禁Shell/Process/tests-run/rql-mutate/SaveAssets/SaveChanges/ClearDirty/字体SetDirty/全局保存/Prefs/PrefabStage/正式工程/启停Editor，业务动作与字体/UI测量只能宿主Scope测试，禁止无保护Editor冒烟。必要安全Refresh允许。」

---

## 1. 逐规格证据矩阵（实质自审，不只复制绿表）

状态列含义：
- **宿主已验**：有宿主真实回执（本层只读回看，未重跑，见 §7）。
- **本轮只读实测**：本轮用只读文件 / 资产 / 日志 / `rql-query` 实测。
- **本轮新增待宿主**：本轮写入，尚未运行，不预填结果。
- **未完成 / 待项**：实现或内容仍缺，明确不标完成。

| # | 规格项（来自 69d2bd2e 范围转述） | 实际实现锚点 | 证据 | 状态 |
|---|---|---|---|---|
| R1 | 统一初始配置模型 + 纯结构校验 | `Assets/Scripts/Village/Runtime/InitialWorldConfiguration.cs`（6279 B / `8b568303…ae08d6`） | `initialization-foundation-implementation.md` §2.1；本轮复算 SHA | 宿主已验（`ff7dc4eb` 10/10） |
| R2 | 唯一默认读取 / 内容校验入口（fail-closed） | `Assets/Scripts/Initialization/InitialWorldConfigurationCatalog.cs`（7974 B / `2bb7a00d…be8381`） | 生成容器缺失 → Registry / Catalog 明确诊断，不静默 0 建筑 | 宿主已验（`ff7dc4eb` + `ec5d598a`） |
| R3 | 正式生成链 XLSX→Compiler→Mirror→Builder→Registry→Catalog | `Development/Config/RRPG_InitialWorld.xlsx`、3 份 Mirror、`InitialWorldConfigDatabase.asset` | Compiler 宿主回执 280/280；Mirror 头部 `*Sha256` 与 Builder 读回；本轮文件 SHA 见 §4 | 宿主已验（Compiler 280 + `ec5d598a` 5/5） |
| R4 | NewGame 事务式提交、零半状态 | `SaveSystem.NewGame(InitialWorldConfiguration)`、`VillageRuntimeBridge.TryPrepareNewGameWorld/CommitNewGameWorld` | 非法配置返回 null 且世界 JSON 逐字节不变（Edit 例）；`VillageRuntimeBridge.Buildings.cs` 23335 B / `b2fdb4c8…169d0` | 宿主已验（10/10） |
| R5 | 真实 Completed Farm / Kitchen / House；住房无岗位 | `BuildingRuntime.TryAddInitialCompleted`、`FunctionWorkplaces/FunctionResourceWorkplaces` | Edit：实例岗位可派工、Housing `UnknownWorkplace`；本轮只读 `Farm/Kitchen/Housing` 绑定见 §3 | 宿主已验（`d10f7d0d` 9/9） |
| R6 | 正式默认无静态 `ws_*`、无 workshop | `TryPrepareNewGameWorld` 空静态基座、`BuildingRegistry.IsFormallyBuildable` | Edit + 产品 Play 均断言 4 个静态岗位定义 / Place / 资源职责为空；工坊 Catalog / Choose / TryPlaceBuilding 拒绝 | 宿主已验（`d10f7d0d`、`0231ecde`） |
| R7 | 旧无 Buildings 档 Continue 不补建、静态兼容保留 | `TryPrepareBuildingWorld` 按存档真实引用恢复 | Edit `LegacySaveWithoutBuildings…`；本轮复看 §5 | 宿主已验（`d10f7d0d`） |
| R8 | 工坊正式下线、旧实例仍可 Load | `IsFormallyBuildable` + `HomeBuildingPlacement` | Edit `WorkshopIsOffline…`；产品 Play `WORKSHOP-REJECT` 日志行 | 宿主已验 |
| R9 | Load 目录只从存档可证明来源（冷/热会话等值） | `VillageRuntimeBridge.Buildings.cs`（静态基座 = 存档真实引用） | Edit 冷/热快照逐项等值 | 宿主已验（`d10f7d0d`） |
| R10 | 住房显示别名：只用 Completed、稳定数值序号、`FNV-1a(PersonId)` 模映射、`DrawsWorldEntity=false` | `ProductHomeReadAdapter.cs`（10629 B / `8d1f849a…c5a39d6`） | Edit `HousingAliases…`；Home Play 3 例；本轮截图（§2） | 宿主已验（`d10f7d0d`、`909a6321`） |
| R11 | 建筑 / 实例岗位同一实体，不再画第二套 Place | `ProductHomeSnapshot` + `ProductHomeWorldHost` | Edit 同实例去重断言；Home Play `homePlaceEntity=False`；截图 7 实体无重复 | 宿主已验 |
| R12 | 真实产品入口端到端（NewGame→管理→Farm 派工→劳动→Save→Continue→不重种→工坊拒绝） | `InitializationProductPlayTests`（原 1 例） | `0231ecde` 1/1 + 证据日志 / 唯一截图（§2） | 宿主已验 |
| R13 | **默认生成 Kitchen 的真实 UI 派工 + 输入转换** | 本轮新增 `DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` | 源码 / 程序集 / 发现已实测（§10）；**未运行** | 本轮新增待宿主 |
| R14 | 管理 17 例（含真实 Kitchen 点击 / 派工 / 转换） | `ManagementShellPlayTests`（516209 B / `ae58d6bf…0afef4496`） | 宿主 `802085c9…` 17/17；日志 10:27:59–10:28:10Z 17 组 ENTRY / 17 组 SAVE-RESTORE / 5 组 SCHEDULE-RESTORE | 宿主已验（本轮只读回看） |
| R15 | `ProductHomeFoundationTests`（Edit）类名与方法发现 | `RRPG.ProductHome.Tests.ProductHomeFoundationTests`（4 例） | 本轮 `tests` 源 + 程序集反射 + `testClass` 正则复算（§10.2） | 本轮修复待宿主 |
| R16 | Home 产品 Play 3 例 | `RRPG.ProductHome.Tests.ProductHomeFoundationPlayTests` | `909a6321` 3/3；证据日志 §2.3 | 宿主已验 |
| R17 | 保护面（guard / 用户档 / 字体 / 34 项 / 场景 / 空闲） | 见 §9 | 本轮只读实测 | 本轮只读实测 |
| R18 | 真实美术表现（farm / kitchen / house） | 见 §3 | 截图 + 生成绑定 + VisualAsset Mirror | **未达成「显示正确已有美术」**（待项，见 §3.4） |
| R19 | 文档同步（快照 / 日志 / 地图 / 交接 / DevelopmentSpecs） | 见 §13 | 本轮实际写入 | 本轮完成写入（文档，不等于验证通过） |

---

## 2. 真实可玩入口与目视审查（只用既有证据，未运行渲染探针）

### 2.1 唯一产品专项证据（只读）

| 项 | 实测 |
|---|---|
| 证据日志 | `Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-play-evidence.log`（3078 B / `3d93e78b…2b091`） |
| 日志结构 | 15 行：`[FONT-PRE] → [SAVE-CAPTURE] → [CONFIG buildings=7 people=5] → [ENTRY] → [WIRING farmWorkplace=workplace:building_instance_1:main] → [MANAGEMENT 已绑定] → [LABOR 0→521 ingredients 0→1 evidenceMinutes=41] → [SAVE] → [CONTINUE sameBuildings=True 逐项等值] → [SHOT] → [WORKSHOP-REJECT] → [END] → [SAVE-RESTORE matches-before=True] → [SCHEDULE-RESTORE sameInstance=True] → [FONT-POST unchanged=True]` |
| 唯一截图 | `play-evidence/initialization-product-save-continue-20261028T114614Z.png`，1920×1080，76821 B / `2ad2089b…4fd4b` |
| 截图 SHA | 与日志 `[SHOT]` 行声明值一致（`2ad2089b01d2b880…aff4fd4b`） |

### 2.2 目视结论（本轮 `read_image` 实测，不代替美术验收）

截图内容（场景为 Continue 后、时钟 08:41、`R2 对照：A 白模` 标签可见）：

1. **7 个建筑实体，与默认配置数量一致**：上排 5 个方形（住房 ×5，`y=4.2`），下排 1 个绿色卡片（Farm）+ 1 个白色圆形（Kitchen）。
2. **Farm 显示的是 `Village/farm-proof.png`**（卡面文字 `FARM A / TECH PROOF` / `NOT FINAL ART`）——即 `va_village_farm_basic`，是**技术证明图，不是最终美术**。
3. **Kitchen 与 5 栋 House 显示的是 `neutral_marker.png` 中性白模**（白色圆 + 金色占位框），**不是** `R2Prototype/kitchen.png` / `R2Prototype/home.png`。
4. **没有重复旧 Place**：画面中不存在 `ws_farm_01 / ws_kitchen_01 / ws_test_*` 的第二个装饰实体（同实例岗位 / 住房别名 `DrawsWorldEntity=false`）。
5. **没有假 / 旧 workshop**：画面无工坊；正式 Catalog / Choose / Place 均已拒绝（日志 `WORKSHOP-REJECT`，catalog=farm,house,kitchen）。

**因此必须如实记录**：实体数据（7 个 Completed 实例、实例岗位、住房无岗位、Save/Continue 等值）已绿，但**默认 A 白模下 Kitchen / House / Person 并没有显示「正确已有美术」**；本记录**不声称住房美术已保留为正式表现**。详见 §3。

### 2.3 Home 3 例真实证据（只读回看）

`play-evidence/product-home-foundation-play-evidence.log`（3 例，每例各 1 组 `[SAVE-CAPTURE]` / `[SAVE-RESTORE] matches-before=True`）：
`[SETUP] farmWorkplace=workplace:building_instance_1:main kitchenWorkplace=workplace:building_instance_2:main buildings=7`；
`[STABLE] personViewStable=True farmViewStable=True houseViewStable=True homePlaceEntity=False`；
`[TRAVEL] kitchenPlace=workplace:building_instance_2:main … arrivedAt520=True domainStable=True`。
即：Home Play 已使用**默认生成实例**的 Farm / Kitchen 岗位与住房实例，但 Kitchen 派工走的是产品 API（`TryAssignPerson`），**不是真实 UI 点击**。

---

## 3. `HomePresentationResolver` / `BuildingPresentationBinding` 实际映射与视觉缺口

### 3.1 生成绑定（本轮只读实测，运行时资产读回）

`Assets/Generated/StaticConfig/Databases/VillagePresentationDatabase.asset` 实际 `buildings` 列表（8 条）：

```text
building_farm/Completed           -> village.building.farm.basic
building_farm/UnderConstruction   -> village.building.farm.construction
building_house/Completed          -> village.building.neutral
building_house/UnderConstruction  -> village.building.neutral
building_kitchen/Completed        -> village.building.neutral
building_kitchen/UnderConstruction-> village.building.neutral
building_workshop/Completed       -> village.building.neutral
building_workshop/UnderConstruction->village.building.neutral
```

对应 Mirror `Development/Generated/StaticConfig/BuildingPresentationBinding.json`（`rowCount=8`）与 `VillagePresentation.json`：
`village.building.neutral → va_village_neutral_marker`（`Village/neutral_marker.png`）；
`village.building.farm.basic → va_village_farm_basic`（`Village/farm-proof.png`）；
`village.building.r2.building_kitchen → va_village_r2_kitchen`（`Village/R2Prototype/kitchen.png`）；
`village.building.r2.building_house → va_village_r2_home`（`Village/R2Prototype/home.png`）。

### 3.2 解析路径（代码实测）

`HomePresentationResolver`（3175 B / `a0089ed3…41a18a`）：

- `PrototypeEnabled=true`（B 原型）→ `village.building.r2.<DefinitionId>`，即 Farm / Kitchen / House **都能解析到 R2 真图**。
- `PrototypeEnabled=false`（**默认 A 白模**，`VillageVisualPrototypePlayTests` 断言「A whitebox remains the default」）→ 走生成绑定：只有 Farm 拿到真实图，Kitchen / House 拿到 `neutral`。
- Person 在 A 下使用 `village.person.<PresentationIntent>`；生成 presentations 中**只有** `village.person.r2`，因此 A 下人物也回落到 `va_village_neutral_marker`（截图中的米色小椭圆即该图着色缩放）。

### 3.3 结论（不是代码 bug，是内容 / 默认档待项）

- 实体接线（同一权威、无重复 Place、无假 workshop、住房别名）**正确**；**A 白模档美术不正确**。
- 该缺口**不是** `HomePresentationResolver` 的解析错误：它忠实消费了生成的 `BuildingPresentationBinding`；A 档的绑定就是 `neutral`。
- 现有 R2 真图（`home.png` / `kitchen.png`）只在 **B 原型**档可达，未进入正式 A 档。

### 3.4 待项（明确不伪完成；不可由本包安全生成）

要修「正式 A 档显示正确已有美术」，唯一正确链路是**改 Authoring XLSX 后由 Compiler 重新生成 Mirror**（禁止手写 Mirror），至少需要：

| 表 | Key | 现值 | 建议值（Test / Not Frozen，仍需制作人/主开发确认） |
|---|---|---|---|
| `BuildingPresentationBinding` | `building_house` / `Completed` | `village.building.neutral` | `village.building.r2.building_house` |
| `BuildingPresentationBinding` | `building_kitchen` / `Completed` | `village.building.neutral` | `village.building.r2.building_kitchen` |
| `BuildingPresentationBinding` | （可选）`building_farm` / `Completed` | `village.building.farm.basic` | 保持或切到 `village.building.r2.building_farm`（属美术决策） |

重新生成命令（宿主执行，本包禁止 / 无法执行 Compiler）：

```text
dotnet run --project Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj -c Release -- validate --root D:\AI_Agent_Lab\rrpg_pilot\repo
RRPG_一键导表.cmd            :: 或 Unity 菜单 RRPG/Static Config/M0 · Build Generated Root
```

**未做原因（边界，不是偷懒）**：本包禁止 Shell / Process、禁止手写 Mirror / 生成资产、禁止业务与渲染冒烟，也不应擅自改变 R2「默认 A 白模」这一已验收的默认档；因此本项**明确为待项**，不写「已修复」。

**另一处如实记录的既有边界**：A 档 Place 键为 `village.place.<Kind>`，而生成 presentations 只含 `village.place.r2.<Kind>`；因此 A 档下若有需要独立绘制的 Place（如活动地点），会落到中性 fallback 并带 Missing 诊断。默认世界当前没有这类实体（建筑 / 岗位 / 住房均 `DrawsWorldEntity=false`），本轮未改该行为，仅记录。

### 3.5 住房别名映射的显示局限（按任务要求如实记录，不新增规则）

`Home:<PersonId>` → 住房实例 = `FNV-1a(PersonId) % Completed 住房数`（稳定数值序号排序）。因此：

- **唯一迁移来源**：新增 / 完成一栋住房会改变模空间，部分人物的显示别名可能迁移；未新增入住 / 容量 / 人口比例规则。
- 当前 SaveData **没有**住房—人物持久绑定；要彻底消除迁移必须改 Save DTO（本包未改，SaveVersion 仍 1）。
- 6 人 / 5 房时允许「一人暂无独立房屋锚点共用」；断言只要求 `usedHousing > 0` 且 `≤ housingIds.Count`（不复制房屋实体）。

---

## 4. 代码 / 配置 SHA 与生成源链（本轮只读复算）

### 4.1 生成源链

```text
Development/Config/RRPG_InitialWorld.xlsx（人工 Authority，11207 B）
  → RRPG.ConfigCompiler（InitialWorldValidator / 三表 Schema / 跨引用；Compiler 宿主 280/280）
  → Development/Generated/StaticConfig/{InitialWorld,InitialPopulation,InitialBuilding}.json（Canonical Mirror，禁止手写）
  → Unity Builder（InitialWorldMirrorReader → InitialWorldConfigDatabase）
  → RRPGStaticConfigRoot.InitialWorldDatabase
  → StaticConfigRegistry.TryGetInitialWorld
  → InitialWorldConfigurationCatalog.TryReadDefault / TryBuildFromGenerated
  → SaveSystem.NewGame()
```

### 4.2 关键文件指纹（本轮实测）

| 文件 | 字节 | SHA256 |
|---|---:|---|
| `Development/Config/RRPG_InitialWorld.xlsx` | 11207 | `a9ac696d6c5a8e638177bf89509b95866d86558dcf344f8f719320afb3a10e4d` |
| `Development/Generated/StaticConfig/InitialWorld.json` | 1569 | `5af02464bbf359c62568525cec1f8c4c11d7fec8a4ce91852a776399c8da321b` |
| `.../InitialPopulation.json` | 1377 | `ae0fe5456266ff6658bd2d2829d987631e74e35ebc0301c3af1514d87e6bcc65` |
| `.../InitialBuilding.json` | 2209 | `32abb64be5c5fbe715b985fae1d56e6301fda516d008d85aa6217b3ff64f0015` |
| `Assets/Generated/StaticConfig/Databases/InitialWorldConfigDatabase.asset` | 1828 | `5859e59b67a8a83072bf1a7e21bffbe34b09677f5744c12abb48ff813e9c35b4` |
| `Assets/Scripts/Initialization/InitialWorldConfigurationCatalog.cs` | 7974 | `2bb7a00dcbd94b1431c65f787dfc8302cb4ef46b4911f5583abd278f09be8381` |
| `Assets/Scripts/Village/Runtime/InitialWorldConfiguration.cs` | 6279 | `8b568303afe1848cb4983f9fde9f4bc659aca8f25c87f12d6e950721fbae08d6` |
| `Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs` | 23335 | `b2fdb4c86cab0b4874f31fa079f571002262c0a6fd0238a6366afbc3a87169d0` |
| `Assets/Scripts/Village/BuildingRuntime.cs` | 6609 | `7f82e113b8e8506bd8aa8d6aada2fade9bfe74007133a8b76e96682a2aea8c08` |
| `Assets/Scripts/Core/PlayerCollection.cs` | 24414 | `e21688bf00670bae08ecf318fe4dc10895afda636aa84c0ebd644d3a81e8b222` |
| `Assets/Scripts/Save/SaveSystem.cs` | 47363 | `bc9fea4f6d64c900a1b25098b2636df9dffb7cd86becf96683d4e962db21bffb` |
| `Assets/Scripts/Village/ProductHome/ProductHomeReadAdapter.cs` | 10629 | `8d1f849a09e4e1653051dd9988edbeef465adba7b18c624faa7261c5205a39d6` |
| `Assets/Scripts/Village/ProductHome/ProductHomeWorldHost.cs` | 15351 | `67c373945226824be22737e33f8b402f9154d0690557ef506ab4100f98d853e9` |
| `Assets/Scripts/Village/ProductHome/HomePresentationResolver.cs` | 3175 | `a0089ed3d732b2def6e99f79cbceb70eddfea4340b312472752df9ccb341a18a` |
| `Assets/Scripts/StaticConfig/VillagePresentationDatabase.cs` | 1910 | `87693ffa501929c65374d353bc5cbfaf086e765f8896f9506b80543cfc77cb48` |

### 4.3 生成物来源声明

- XLSX / 三份 Mirror / 数据库均由同一 Compiler + Builder 链产生（Compiler 宿主回执 `compiler-host:8e7758c9290a8d750aae31b24d71a167219d2ba4906509fe7ec9e08a4f476b4a`，280/280、0 skip、`seed/build/validate` 退出 0）；**本包未手写 Mirror、未手写 `.asset`**，本轮也未运行 Compiler / Builder。
- 生成当时变动面：新增 `InitialWorldConfigDatabase.asset(+.meta)`、Root 内容更新；其余生成资产内容 SHA 未变（`generated-pre/post-snapshot.json`）。

---

## 5. 旧无 Buildings / 旧 workshop / 静态岗位边界

| 场景 | 实际语义 | 证据 |
|---|---|---|
| 正式 NewGame → Save → Load（7 实例） | 岗位 / 资源目录只从存档真实引用 + 实例派生；不重种 `ws_*` | Edit `FormalSaveLoads…` / `FormalSaveRestoresOnly…`；产品 Play `CONTINUE` 断言 |
| 旧静态世界（`EnsureInitialized`）派工 → Save（0 建筑） | 完整历史静态兼容目录 + 默认资源职责保留；**不补建**初始建筑 | Edit `LegacySave…`；产品 Play `ws_*` 断言 |
| 正式实例档 + 存档真实引用一个旧静态岗位 | 只恢复被引用者（定义 / 职责 / 资源角色 / Place），未引用者不凭空出现 | Edit `FormalSaveRestoresOnlyLegacyWorkplacesItActuallyReferences` |
| 同会话先 NewGame 再 Load 旧档 | 与冷启动结果一致（同一兼容目录） | Edit 冷/热等值用例 |
| 工坊 / 铁匠铺 | 历史定义、旧实例 Load 保留；正式 Catalog / Choose / Confirm / `TryPlaceBuilding` 全部拒绝（诊断含“下线”） | Edit `WorkshopIsOffline…`；产品 Play `[WORKSHOP-REJECT]` |
| 旧静态岗位 `ws_*` | 仍在默认兼容目录，**不是**正式新增派工目标；正式默认世界不注册 | Edit + 管理 CASE-B 显式兼容注入 |

---

## 6. Test / Not Frozen 与数量口径

| 字段 | 当前值（Test / Not Frozen） | 来源 / 边界 |
|---|---|---|
| Profile | `iw_default_test`（`IsDefault=TRUE`） | XLSX 行；**未冻结** |
| Hero | `hero_001` | 既有 `DataManager` 默认 |
| Gold / Ingredients / Food | 100 / 0 / 0 | 既有实际默认 |
| 人口 | 5（`person_lin_en / su_wan / han_zhao / qiu_shuang / wei_an`），运行时候选含英雄 = 6 | 生成配置 + `DataManager.InitialPersonRoster`；**不冻结人数** |
| 建筑 | 7（1 Farm `(-6,-1.2)`、1 Kitchen `(-2,-1.2)`、5 House `y=4.2`） | 当前 Test 内容；**不冻结数量 / 位置 / 住房比例** |
| 容量 | Farm 2 / Kitchen 1（来自既有 `VillageWorkplaceDef` profile） | **测试改为从 profile 派生**，见 §10.1 |
| 不含 | `building_workshop` / `building_smithy` | 正式默认与正式 Catalog 均不含 |
| 住房 | 无 FunctionBinding → 无岗位、无容量/入住规则 | 既有语义，未新增 |

---

## 7. 当前与历史测试清单（宿主回执为唯一通过依据）

### 7.1 当前有效回执（本层只读回看，未重跑）

| 范围 | 回执 / 结果 | 说明 |
|---|---|---|
| 管理 Play `ManagementShellPlayTests` | 宿主 `802085c9-d9a9-47fa-8fe6-b28267963190`：**17/17 Passed、0 fail、0 skip、16.4111984 s、`scope-restored\|font\|0\|632`**；本轮 `testResults` 只读复核 17 条全 `Passed`、`message=null` | 最新 |
| 初始化 ×3 Edit 类 | `ff7dc4eb` 基本 **10/10**；`ec5d598a` 配置链 **5/5**；`d10f7d0d` Edit 新接线 **9/9** | 前阶段真实通过 |
| 产品专项 Play | `0231ecde` **1/1** | 前阶段真实通过；本轮新增第 2 例**已由宿主运行通过**（见下行） |
| Kitchen 真实 UI 专项 | `testResults` 当前唯一回执：`InitializationProductPlayTests.DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` → **Passed / 1845 ms / message=null**；证据见 §18.1（guard run `20261029T110032Z` `restored`） | 2026-10-29T11:00Z 宿主真实运行 |
| Home Play | `909a6321` **3/3** | 前阶段真实通过 |
| Compiler | 宿主回执 `compiler-host:8e7758c9…`：**280/280、0 skip**，`seed/build/validate` 退出 0 | 前阶段真实通过 |
| 完整 Edit / Play 全量 | 1513 / 122 为**更早阶段**真实通过，**不作本包门槛，本包未重跑** | 口径保留 |

### 7.2 管理 17 例保护面日志（本轮只读精确统计）

日志 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，窗口 `2026-10-29T10:27:54Z–10:28:10Z`（252 行）：

```text
[ENTRY]=17  [SAVE-CAPTURE]=17  [SAVE-RESTORE matches-before=True]=17
[SCHEDULE-RESTORE sameInstance=True]=5  [FONT-POST unchanged=True]=1  [BASELINE-POST 34/34 all-match]=1
[SAVE-RESTORE-SETUP-FAILURE]=0
```

即：**17 例真实 ENTRY、17 次先捕获后恢复且逐字节匹配、5 例日程同一实例恢复、字体与 34 项收尾一致、无 SetUp 清理失败**。

### 7.3 历史失败 / 拒绝 / unknown（原文保留，不洗白）

| 记录 | 内容 | 处理 |
|---|---|---|
| `708ba490` | `InitialWorldConfigurationTests` 10 总 / 9 Passed / 1 Failed（`TestResetAndEnsureInitialized…` NRE，stack 指向旧第 344 行 `Assignments().WorkplaceDefs`） | 根因 = `ResetForTests` 后 `Assignments=null`；已修测试断言顺序；后续 `ff7dc4eb` 10/10 |
| `568f51ff` | `InitialWorldProductWiringTests` 9 总 / 7 Passed / 2 Failed（NRE，测试辅助 `Work()` 把静态属性当字段） | 改测试反射形态；后续 `d10f7d0d` 9/9 |
| `0483b34b` | `InitializationProductPlayTests` **0 匹配**（工具 `testClass` 正则要求类名前有命名空间段，全局命名空间类必然 0 匹配） | 不改 namespace；改口径为 `testMethod=…FullName`；后续 `0231ecde` 1/1 |
| `a5b3061b` | 管理 17 例 14 pass / 3 fail（目标业务行在视口外，top-hit 被 `ModalBlocker` 接住） | 新增真实 `IScrollHandler` 目标行滚动 helper；后续 `802085c9` 17/17 |
| 2026-10-29 首次管理 17 | 17/17 Failed：首例 `AmbiguousMatchException`（`SaveSystem.NewGame()` 新增重载后 `GetMethod(name)` 歧义），其余 16 例被唯一 `captured` guard run 连锁阻断 | 显式签名 / 参数个数解析 + SetUp `try/finally` 兜底恢复；该 run 终态经独立字节核验为 `restored` |
| `01a75064` | 执行状态 `unknown`、无原生终态 | `recoveryReview.safeToContinue=true`（2026-10-29T09:57:10Z），制作人 `2b6f25cf` 确认安全续接；旧错误历史保留 |
| 历史边界违规 | 旧 `4fbf7754` 轮 `ClearDirty`、详情修复包 `AssetDatabase.SaveAssets()` 越界 | 原文保留，不删除不辩解 |

---

## 8. 本轮实际变更（窄修，均为最小面）

| 文件 | 变更 | 变更前 | 变更后 |
|---|---|---|---|
| `Assets/Tests/EditMode/Village/ProductHomeFoundationTests.cs` | `Call(...)` 改为按「方法名 + 实参个数」唯一定位静态方法（修复 `SaveSystem.NewGame` 重载歧义，SetUp 不再抛 `AmbiguousMatchException`）；断言未改、方法名未改 | 4232 B / `a6720cf7…c209820e` | **4944 B / `2062db0d1f4d8fa1cbd0c11b55f73e0bb63bfe17ff0cf869721bca7a79f29829`** |
| `Assets/Tests/PlayMode/InitializationProductPlayTests.cs` | 新增 1 个聚焦用例 `DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` + 4 个只读辅助（`FirstCompletedKitchen` / `WorkBatchWorkplaceIds` / `ClickPopulationRow` / `ClickWorkshopRow` 等价 `ClickWorkplaceRow` / `ShowManagementTab`）；原用例与断言未改 | 48236 B / `13f1b3c5…c8dd60fec9` | **69371 B / `35b16d7caf0c6e7c9372741e4cb04b9daea33b26436e53656a866d4bbc6fa0f2`** |
| `Assets/Tests/EditMode/Initialization/InitialWorldProductWiringTests.cs` | 解冻数量口径：删除硬编码 `7 / 1 / 1 / 5` 与容量 `2 / 1`，改为由**当前生成配置**与**既有 FunctionProfile** 派生；新增 `FunctionProfile` / `ProfileCapacity` 只读辅助；保留非空 / 类型范围 / 无 workshop / Completed / 唯一 InstanceId / 不重复初始化强语义 | 40921 B / `96890cb8…0cc61fb9e16` | **45206 B / `9049decfbc1ccac36e2053cbeac1e11422091e2288c5a7fcf00f8e42e292e81c`** |

**没有改动**：`Assets/Scripts/**`（本轮 0 产品代码改动）、`Assets/Generated/**`、XLSX、Canonical Mirror、Save DTO / SaveVersion、Scene / Prefab / 字体 / ProjectSettings / Packages、34 项基线、guard 历史、用户单槽。

### 8.1 新增 Kitchen 专项覆盖（不绕过 UI）

`DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue`：

1. 真实主菜单 `btnNewGame` → HeroSelection 英雄卡 + 确认（真实 `EventSystem` + top-hit 断言）；
2. 读取默认生成配置 → 运行时建筑数量 / 定义同源；取**默认生成的** Kitchen / Farm 实例与其 `workplace:<InstanceId>:main`；
3. 管理页：人口行真实点击选人 A → 职责页真实点击 **默认 Kitchen 实例岗位行**（断言 `SelectedWorkplaceId`）→ 真实“派工”按钮 → 断言写入 Kitchen 职责且状态含“已绑定”；
4. 再选人物 B → 默认 Farm 实例岗位行 → 派工（为 Kitchen 提供真实原料）；
5. 关闭管理，隔离活动机会窗口后真实 `Clock.AdvanceTo`：先断言 Farm 真实劳务证据 > 0，再在有界窗口内推进直到 **Food 真实增加**，断言 Kitchen 劳务证据 > 0 且两个实例都留下资源批次（**全程不调用 `AddIngredients`，不手写 Work / Resources**）；
6. 管理页真实“保存”按钮 → 场景重建 → `MainMenu` 真实 Continue → 建筑 / 双方职责 / Food / Ingredients / 批次 / 证据（含 Kitchen 分钟）逐项等值，且**不重种** 4 个静态 `ws_*`；
7. 存档保护沿用本类既有持久 `SaveSlotGuard`（先捕获、`finally` 恢复、未完成 run 阻断），日程恢复沿用 `RestoreActivitySchedule`（同一实例断言），字体由 `[OneTimeSetUp]/[OneTimeTearDown]` 比对。

**运行状态（2026-10-29 更新）**：该用例已由宿主真实执行 → `testResults` **Passed / 1845 ms**，证据见 §18.1（此处的设计描述仍按写入时口径保留为历史）。

### 8.2 解冻改动的精确范围

- `InitialWorldProductWiringTests.DefaultGeneratedNewGameCreatesSevenRealBuildingsWithoutDuplicateInitialization`：**方法名保留**（历史发现契约），注释说明 “Seven” 只是当时数量；断言全部改为配置派生。
- `InitialWorldProductWiringTests.DefaultNewGameAssignsInstanceFarmKitchenAndHousingHasNoWorkplace`：容量断言改为 `ProfileCapacity(FunctionProfile(...))`，不写死 2/1。
- `InitialWorldProductWiringTests.HomeSnapshotUsesSameBuildingIdentityWithoutDuplicateHousingOrInstanceWorkplaces`：`housingIds.Count` 由默认配置 Housing 数量派生（且 > 0），不再固定 5。
- 其余 6 例未改。

### 8.3 现有 Kitchen 证据审查结论（为何仍新增专项）

| 现有证据 | 是否真实 UI | Kitchen 来源 | 结论 |
|---|---|---|---|
| 产品专项 `InitializationProductPlayTests`（`0231ecde` 1/1） | ✅ 真实 UI（人口行 / 岗位行 / 派工按钮 top-hit） | 只用**默认 Farm** `workplace:building_instance_1:main` | **没有 Kitchen 点击 / 派工** |
| 管理 `ManagementShellPlayTests.CASE-C`（`ExplicitSaveContinueAndNewGame…`） | ✅ 真实 UI：`ClickWorkplaceRow(wKitchen)` → `assignButton` → 断言 `PersonWorkplace(p5)==wKitchen` | **测试内新建** `kitchenC=building_instance_9`（日志 2026-10-29T10:27:56Z）；原料走既有公开 `AddIngredients(2)`；真实推进后 `food=7` | 有真实 Kitchen UI 派工 + 输入转换，但**不是默认生成 Kitchen 实例** |
| 管理 `SurfaceEquivalentSimulation…`（W0） | ✅ 真实 UI：Kitchen C 岗位行 + 派工 + `AddIngredients(2)` → 真实 `Advance` 后 `resources=2/7` | 测试内新建 Kitchen | 同上 |
| Home Play 3 例（`909a6321` 3/3） | ❌ 走产品 API `TryAssignPerson` | **默认实例** `workplace:building_instance_2:main`（日志 `[SETUP] kitchenWorkplace=…`） | 默认 Kitchen 有 API 派工与通勤证据，**无 UI 派工** |
| Home Edit 9 例（`d10f7d0d` 9/9） | ❌ Edit 直接调用 | 默认实例 | 只证明领域可派工，不是 UI |

**结论**：「默认生成 Kitchen 实例 + 真实 UI 派工 + 真实输入转换 + SaveContinue 同事实」在既有证据中**不完整**，因此本轮新增 §8.1 的聚焦专项；没有重复已有的 Edit / API 断言，也没有把 API 调用冒充 UI。

### 8.4 2026-10-29 增量窄修（阶段 1/3，仅测试夹具）

| 文件 | 变更 | 变更前 | 变更后 |
|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/InitializationProductPlayTests.cs` | `SaveSlotGuard` 新增只读 `RunId`；`[UnitySetUp] SetUp()` 改为 `Capture` 后 `try/finally`：`ResetForTests` / `LoadScene` / 等待帧任一步抛异常时，用**本 runID 已验备份**恢复用户单槽并写 `SAVE-RESTORE-SETUP-FAILURE` 证据；新增 `RestoreCapturedSaveAfterFailedSetUp()`（恢复失败只告警 + 证据，不掩盖原始异常）。**原有断言、方法名、两个用例体、TearDown 顺序均未改** | 69371 B / `35b16d7c…fa0f2` | **71084 B / `56cca5e4cd8913cc1e1a9770366c6a79fab2a6ab8b38112c86b401f551c0af39`** |

理由（不是风格偏好）：`ManagementShellPlayTests` 已实测过「UnitySetUp 失败时 NUnit 不保证执行普通 `[TearDown]`」，当时曾出现 Capture 后 NewGame 抛 `AmbiguousMatchException`、`SAVE-RESTORE` 直到 `OneTimeTearDown` 才发生。新 Kitchen 专项与旧 Farm 单例共用本类 SetUp，因此必须先补异常路径，否则任何 SetUp 期异常都会把用户单槽留在 `captured` 未完成态（虽然不写坏档，但会阻断后续 run 并要求人工核对）。

**本轮仍未**：改断言强度、改产品代码、改 XLSX / Mirror / 生成资产、加新用例、跑测试。

### 8.5 2026-10-29 增量窄修（阶段 2/3，仅测试夹具）

| 文件 | 变更 | 变更前 | 变更后 |
|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` | `SaveFileGuard.BlockOnUnfinishedRuns()` 的 manifest 状态判定改为**去空白后**匹配 `"status":"restored"`（修复与 `SaveSlotGuard` 缩进写 manifest 的跨格式不兼容）；注释写明两种真实格式与“不放松语义”（`captured` / `capture-failed` / `restore-failed` / 缺失仍阻断） | 判定为 `File.ReadAllText(manifest).Contains("\"status\":\"restored\"")`（只认紧凑格式；该文件本轮前未单独记录字节数，不补造） | **44666 B / `cd126162b80573d8098889a388329778dc42723e254adcf0030a9a11de5da289`**（mtime `2026-10-29T11:01:43Z`） |

理由见 §18.2：保护根现有 5 个由 `SaveSlotGuard` 缩进写出的 `restored` manifest，旧判定会把它们全部当作 unfinished，导致九例 / 十例在 `SetUp` 阶段 `Assert.Fail`，**断言根本没有机会执行**。

---

## 9. 保护面（本轮只读实测，写文件 / 2 次安全 Refresh 之后）

| 项 | 实测值 | 判定 |
|---|---|---|
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| PrefabStage | `null` | ✅ 未进入 Prefab 编辑 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`）`isDirty=False / rootCount=5` | ✅ 未污染 |
| 用户单槽 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，`13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`（存在、未写） | ✅ 逐字节恢复态 |
| Initialization guard | `dirs=67 / restored=67 / protective=0 / other=0 / noManifest=0` | ✅ 无未完成 / 无未知 |
| R3B guard | `dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective 为既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体（磁盘） | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与保护值一致 |
| 字体子资产 | `9` 个，`EditorUtility.IsDirty` → **0 dirty** | ✅ 无 pending 保存（**未做字模测量**） |
| 34 项 workspace 基线 | `total=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |
| Console | 本轮安全 Refresh 后 `sequence > 96542` 的 `Error/Exception` = **0**；既有 3 条（`sequence 96540–96542`，`TestRunnerApi.IsRunActive` 探针编译失败）非本轮产生，**保留不清洗** | ✅ 无项目 / 测试程序集错误 |

### 9.1 2026-10-29 增量只读实测（阶段 1/3，写入 + 安全 Refresh 之后）

| 项 | 实测值（本轮） | 判定 |
|---|---|---|
| Pilot 身份 | `Application.productName=RRPG_AI_Pilot`、`companyName=DefaultCompany`、`unityVersion=2022.3.62t9` | ✅ 授权工程 |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False / selectionCount=0` | ✅ 空闲 |
| 原生 TestRunner | `Tool_Tests.HasPendingTestRun()=False`；`EditModeLauncher.IsRunning=False`、`PlaymodeLauncher.IsRunning=False`（静态字段反射） | ✅ 无 running / 无待投递（本版本 `TestRunnerApi` 无 `IsRunActive` 成员，已如实记录，不据此推断队列） |
| PrefabStage | `PrefabStageUtility.GetCurrentPrefabStage()=null` | ✅ 未进入 Prefab 编辑 |
| 场景 | `SampleScene` `isDirty=False / rootCount=5` | ✅ 未污染 |
| 用户单槽（**本轮实际捕获，不套用历史字节数**） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，`exists=True / 13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`；同目录 `*.tmp` = 0 | ✅ 与历史值恰好一致，但以本轮实测为准；本轮**未写、未删、未改**该文件 |
| Initialization guard | `dirs=67 / restored=67 / protective=0 / other=0 / noManifest=0` | ✅ 无未完成 / 无未知（`BlockOnUnfinishedRuns` 不阻断） |
| R3B guard | `dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective = 既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体（磁盘） | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与保护值一致 |
| 字体子资产 | `AssetDatabase.LoadAllAssetsAtPath` = **9** 个，`EditorUtility.IsDirty` → **0 dirty** | ✅ 无 pending 保存（**未做字模测量**） |
| 34 项 workspace 基线 | `total=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |
| Console | 本轮 `console` 过滤 `Error/Exception` **只有 3 条**：`sequence 98681–98683`，全部是 §10.4 记录的**只读探针** snippet 编译失败（`TestRunnerApi` 在 `1.1.33` 无 `IsRunActive` 成员），与项目 / 测试程序集无关；§9 旧记录的 `96540–96542` 已不在当前缓冲区，**不据此改写历史** | ✅ 项目与测试程序集 0 错误 |

---

## 10. 编译与发现证据（真实执行）

### 10.1 安全刷新

- `script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` ×2（改 Play 测试后一次、改 Edit 测试后一次）；刷新后 `isCompiling=true → false / isUpdating=false / isPlaying=false`。
- 未 `SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `SetDirty` / `ClearDirty`；未保存场景；未启停 Editor。

### 10.2 `ProductHomeFoundationTests`（Edit）实际类名与方法发现

| 项 | 实测 |
|---|---|
| 类全名 | `RRPG.ProductHome.Tests.ProductHomeFoundationTests`（命名空间 `RRPG.ProductHome.Tests`，`sealed`） |
| 程序集 | `VillageStage1.Tests`（`VillageStage1.Tests.dll`，161792 B / `7aee7fed…d458ea`，mtime `2026-10-29T10:48:00Z`） |
| `tests` 源发现 | **4 条** EditMode、`ignored=false / explicit=false` |
| 方法名 | `IdentityAndNameAreFromLongTermSource`、`AssignmentCapacityAndReservedOccupancyUseExistingAuthority`、`SnapshotAndSelectionDoNotWriteRuntimeOrRegistry`、`CollectionsCannotBeMutatedThroughSnapshot` |
| 类过滤可用性 | 有命名空间 → 宿主同一正则 `^.*\.<class>\.[^\.]+$` 可命中（`testClass=ProductHomeFoundationTests` 可用） |
| 修复后 DLL 内容 | 新断言串「静态方法必须唯一：」UTF-16 命中；即修复已真实编译进当前程序集 |

### 10.3 `InitializationProductPlayTests` 发现与程序集内容

| 项 | 实测 |
|---|---|
| `tests` 源发现 | **2 条** PlayMode（`BattleStage1.PlayModeTests`）：`DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` + 原 `DefaultProductEntry_…` |
| 程序集反射 | `InitializationProductPlayTests` 存在；`[UnityTest]` 方法 **2** 个、名称与源码一致 |
| DLL | `BattleStage1.PlayModeTests.dll` 624640 B / `fa2a36387250f818dc6f7c40c6bd6a45e48776838ae58ae8b807c1357582a06f`，mtime `2026-10-29T10:48:01Z` |
| 内容级核对（UTF-16 字面量） | 「真实 Kitchen 必须把 Farm 产出的 Ingredients 转换（消耗输入）为 Food；未消耗原料不得凭空产出」、「真实点击岗位行后 Management 选中候选必须是默认 Kitchen 实例岗位」、「必须为 Kitchen / Farm 选择两个不同真实人物」、「Continue 必须逐项恢复 Kitchen 劳务证据」、「真实派工按钮必须写入默认 Kitchen 实例职责」、「正式实例档 Continue 不得重种静态资源职责：」**全部命中**；方法名（UTF-8）亦命中 |
| 类过滤限制（沿用已记录事实） | 该类在**全局命名空间**，`testClass=InitializationProductPlayTests` 必然 0 匹配（工具正则要求类名前有命名空间段）；**必须用 `testMethod=FullName`** |
| `InitialWorldProductWiringTests` | 9 条 EditMode 仍可发现；`InitialWorld.Tests.dll` 71168 B / `eb97fe6d…d305b5e`，新字面量（`_testWorkplaces` 权威、配置派生住房数、正式可建造类型、容量继承 Profile）**全部命中** |

### 10.4 2026-10-29 增量（阶段 1/3）安全刷新与发现复核

- 安全刷新：`script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` ×1（**仅夹具改动后**）；刷新后 `isCompiling=False / isUpdating=False / isPlaying=False`。未 `SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `SetDirty` / `ClearDirty`；未保存场景；未启停 Editor。
- 只读探针 `TestRunnerApi.IsRunActive` 在 `com.unity.test-framework@1.1.33` **不存在**，导致 1 次 snippet 编译失败（`sequence 98681–98683`）；随后改用 `Tool_Tests.HasPendingTestRun()` + `EditModeLauncher/PlaymodeLauncher.IsRunning` 静态字段完成同一核对。该失败**只发生在 MCP 代码片段沙箱**，未触碰项目 / 测试程序集，已如实登记。

| 项 | 本轮实测（2026-10-29T10:56–10:58Z） |
|---|---|
| `tests` 源目标方法 | **1 条**：`InitializationProductPlayTests.DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue`；`mode=PlayMode`、`assembly=BattleStage1.PlayModeTests`、`explicit=false / ignored=false / parameterized=false / namespace=""`（全局命名空间） |
| 类内用例数 | `tests` 源该前缀共 **2 条**；程序集 `[UnityTest]` 方法 **2** 个、名称与源码逐字一致 |
| 类过滤限制 | 全局命名空间 → `testClass=InitializationProductPlayTests` 仍会 **0 匹配**；宿主必须用 `testMethod=<FullName>` |
| 源码 | `Assets/Tests/PlayMode/InitializationProductPlayTests.cs`，**71084 B / `56cca5e4cd8913cc1e1a9770366c6a79fab2a6ab8b38112c86b401f551c0af39`**，mtime `2026-10-29T10:56:56Z` |
| DLL | `BattleStage1.PlayModeTests.dll` **625152 B / `55743eeb7a3b53f4680a5cea71cbf93b1ed89329de9b4f5fdab71271e71315dd`**，mtime `2026-10-29T10:57:06Z`（旧 `624640 / fa2a3638…` 为增量前） |
| 编译后形态核对 | `SetUp` 带 `[UnitySetUp]` 特性；`RestoreCapturedSaveAfterFailedSetUp` 方法存在；`SaveSlotGuard.RunId` 属性存在 |

---

## 11. 交给宿主的准确验证口径（不预填结果）

### 11.1 受影响既有测试（实际源码审查，不声称「无交集」）

初始化包把**默认 `SaveSystem.NewGame()`** 从「0 建筑 + 静态 `ws_*` 兼容目录」改成「生成配置的 7 个 Completed 实例、静态基座为空」，因此凡是在**默认 NewGame 世界**里引用静态 `ws_*` 或断言 0 建筑的既有测试都会真实失败。逐份源码审查结论：

**已确认（源码级必然失败，需后续窄包更新，不能当作已通过）**

| 类 / 方法 | 真实原因 |
|---|---|
| `RRPG.Building.Tests.BuildingConstructionProductPlayTests.HomeWorkersActualLaborTwoIndependentFunctionsAndContinue` | 默认世界派工写死 `ws_farm_01` / `ws_kitchen_01`（现 `UnknownWorkplace`）；并断言 `Snapshot.Buildings.Count == 2`（现在默认 7 + 新建） |
| `RRPG.Building.Tests.BuildingConstructionProductPlayTests.ZeroWorkersAndLateLoadFailureRetainsWholeConstructionWorld` | 断言 `NewGame` 后 `Save.Buildings.Instances.Count == 0` 且无 `workplace:` 定义（现在分别 7 / 2） |
| `VillageCommitmentProductIntegrationPlayTests.VillageTMP_LabelsRender_AndKitchenRowsRemainStableAcrossRefresh` | 期待 `WorkplaceRow_ws_kitchen_01` UI 行（正式世界无此静态岗位） |
| `VillageCommitmentProductIntegrationPlayTests.IntradayWork_RealTownAssignment_CommutingLaborLunchVisible` | 期待并点击 `WorkplaceRow_ws_farm_01`（同上） |
| `VillageV4ResourceSurplusPlayTests.FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry` | 默认 NewGame 后 `TryAssignPerson(..., "ws_farm_01"/"ws_kitchen_01")`（现 `UnknownWorkplace`） |
| `RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests.ActivityPlacesCannotReflowRegisteredWorkplaces` | 直接索引 `PlaceViews["ws_farm_01"…"ws_test_02"]`（正式世界不再创建这些 Place） |

**源码审查后判断不受本次默认改动影响（依据写在右列，仍建议宿主全量回归确认）**

| 类 | 判断依据 |
|---|---|
| `RRPG.Building.Tests.BuildingPlacementProductPlayTests` | 只做相对存档 / 拒绝 Load 比较，不依赖建筑数量绝对值；新放置点 `(5,-4)/(8,-4)` 与默认布局不重叠 |
| `RRPG.Building.Tests.BuildingResourceProductPlayTests` | 只用新建实例岗位 + 真实生产；未派工的默认 Farm/Kitchen 不产出，`479 分钟 0/0` 仍成立 |
| `RRPG.Building.Tests.BuildingVisualReplacementPlayTests` | 仅新旧实例视觉替换，不依赖静态岗位 |
| `VillageWorldProductIntegrationPlayTests` | 世界 Place 计数用相对值；存档只持久化 ProjectSite（Workplace 派生不落档） |
| `HomeProductWhiteboxTests`（Edit） | 只用 NewGame 建人物源 + Attention / 存档 JSON 自等值，不读建筑数量 |
| `PersonDetailProjectionTests` / `BuildingConstructionTests` / `BuildingResourceTests` / `VillageResourceBridgeTickTests` / `ManagementProjectionTests` / `VillageResourceGate4Tests` 等（Edit） | 走 `EnsureInitialized(null)` 的**旧静态兼容世界**，静态 `ws_*` 仍注册；不受默认 NewGame 改动影响 |
| Battle / Expedition / PersonVisual / Encounter / Event 等只把 `NewGame()` 当人物源的套件 | 不检查村庄建筑 / 静态岗位；本层未逐方法复核，保留为宿主全量回归项 |

**结论**：本包**不声称**「其它测试无交集」；上述 6 个方法是**真实缺口**，属于初始化整包的后续窄包 / 宿主回归范围，本阶段未改写它们（避免超出「Home Edit 必要回归 + 本范围窄修」授权），也不会把它们算作通过。

> **2026-10-29 阶段 3/3 更正与补充（原文保留）**
> 1. 上表第 4 行 `VillageCommitmentProductIntegrationPlayTests.IntradayWork_…` 的**类归属正确**，但源码在**另一个 partial 文件** `Assets/Tests/PlayMode/Village/IntradayWorkProductPlayTests.cs`；宿主按 `testMethod` 执行不受影响。
> 2. 源码逐行复核发现**上表漏判 3 个同因方法**（都引用正式世界已不存在的静态岗位）：
>    - `VillageCommitmentProductIntegrationPlayTests.CommitmentUI_RealTown_AssignAndProject_ClosedLoop_PanelDoesNotPauseSimulation`（`OnAssignClicked("ws_test_01")`）
>    - `VillageCommitmentProductIntegrationPlayTests.CommitmentUI_ReflectsRuntimeAfterSaveLoad_MidProject`（同上）
>    - `VillageWorldProductIntegrationPlayTests.Presence_DerivedAndWorldVisible_WithoutAnyManagementPanel_And_SurvivesSimToggle`（`TryAssignPerson(pid,"ws_test_01")`，**与上表「VillageWorldProductIntegrationPlayTests 不受影响」的判断相反**）
> 3. 前两个已随本阶段同类适配修订（同一类、同一原因、同一改法）；第三个**本阶段未修订**，明确登记为待办缺口（§19.6），不得当作通过。

### 11.2 宿主执行参数

| 目标 | 参数 | 通过标准 |
|---|---|---|
| 新增 Kitchen UI 专项 | `testMode=PlayMode` + `testMethod=InitializationProductPlayTests.DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` | 1/1 Passed、0 skipped；0 项 / skip 不算通过 |
| 原产品专项（回归） | `testMode=PlayMode` + `testMethod=InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed` | 1/1 |
| Home Edit 必要回归 | `testMode=EditMode` + `testClass=ProductHomeFoundationTests`（**整类 4 例串行**） | 4/4 Passed、0 skipped |
| Edit 新接线（数量口径解冻后） | `testMode=EditMode` + `testClass=InitialWorldProductWiringTests`（整类 9 例） | 9/9 Passed、0 skipped |
| 管理 17 例（既有回执有效） | `testMode=PlayMode` + `testClass=ManagementShellPlayTests` | 17/17（本轮不重跑；如重跑需 17/17） |

**2026-10-29 阶段 1/3 指令口径（收窄）**：本轮**只交 Kitchen 专项 1 例**；**不重跑**原 Farm 单例 `DefaultProductEntry_…`、不重跑管理 17 / Home Edit 4 / Edit 9（均在各自上一轮回执内有效）。若宿主必须复核回归，按上表独立执行，不得把“没跑”写成“通过”。

**不重跑**：Compiler 280、完整 Edit 1513、完整 Play 122（历史门禁；本包改动仅 3 个测试文件，按类回归即可）。
**失败处理**：保留真实 `message` / `stack`；测试夹具问题改夹具，落在产品 Save / Load / 投影才做产品窄修；**不得削弱已冻结 / 保留断言**。
**预期副作用**：Kitchen 专项新增 1 个 Initialization guard run（`captured → restored`）并追加 `KITCHEN-*` 证据行；Home Edit 类不写用户档（Edit 无文件写）。

---

## 12. 产物 vs 测试证据（区分）

| 类别 | 内容 | 是否等于通过 |
|---|---|---|
| 产品 / 配置产物 | 4.2 表内代码、XLSX、3 份 Mirror、`InitialWorldConfigDatabase.asset`、`VillagePresentationDatabase.asset` | ❌ 不等于测试通过 |
| 测试产物（本轮写入） | `InitialWorldProductWiringTests.cs`（解冻口径）、`ProductHomeFoundationTests.cs`（重载解析）、`InitializationProductPlayTests.cs`（新增 Kitchen 专项） | ❌ 只证明编译 + 发现 |
| 测试证据（历史宿主回执） | `testResults` 17 条 Passed、`0231ecde` / `909a6321` / `d10f7d0d` / `ec5d598a` / `ff7dc4eb`、Compiler 280、两处 play-evidence 日志与截图 | ✅ 仅对其记录的版本 / 范围有效 |
| 本轮自测 | 无（禁止 `tests-run`）；只有只读测量 + 按轮次的安全 Refresh（阶段 1 增 1 次、阶段 2 增 1 次）+ 编译后测试发现 | ❌ 不是通过 |

---

## 13. 文档同步（本轮实际写入）

| 文件 | 同步内容 |
|---|---|
| `Docs/项目状态快照.md` | 顶部新增 2026-10-29 初始化整包自审条目（范围 / 回执 / 保护面 / 未验证），R3-B 条目保持原日期历史 |
| `Docs/项目日志.md` | 顶部新增 2026-10-29 初始化整包条目（自审结论、变更、宿主待跑） |
| `Docs/文档地图.md` | 新增 Initialization 交付记录入口 |
| `Docs/开发验收交接.md` | 新增 2026-10-27 起新委托与 2026-10-29 交接（含守卫 / 未测 / 待跑口径） |
| `Docs/DevelopmentSpecs/README.md` | 顶部新增 2026-10-27 新委托 + 2026-10-29 当前记录指针 |

---

## 14. 未验证 / 风险（不写 Completed 的理由）

1. **Kitchen UI 专项已由宿主验证通过（1/1 Passed、1845 ms）**，证据与断言落地见 §18.1；其余新增 / 修改测试仍以宿主回执为准，本文不预填 PASS。
2. **Home Edit 4 例未运行**（本轮修复重载歧义后）：必须宿主按类跑；修复前该类的 `SetUp` 会在 `SaveSystem.NewGame` 重载上抛 `AmbiguousMatchException`，这是**真实缺口**，已修但未验证。
3. **Edit 9 例数量口径解冻后未运行**：由配置 / profile 派生后是否与实际默认内容一致，需宿主真实执行确认。
4. **A 档美术未达成**：Kitchen / House / Person 在默认 A 白模下仍是 `neutral_marker`，Farm 仍是 TECH PROOF 图；R2 真图只在 B 原型可见；精确修复提案见 §17（未执行）。**不声称美术保留完成，也不宣称整包完成**。
5. **Place 键缺口**：A 档 `village.place.<Kind>` 无对应生成键（仅 `village.place.r2.*`）；默认世界无独立绘制 Place，未改。
6. **住房别名迁移**：新增 / 完成住房会改变 `PersonId % 住房数` 的模空间；无持久住房—人物绑定（不改 Save DTO）。
7. **物理输入 / 客户端**：所有点击均为程序化 `EventSystem`（非物理鼠标 / 滚轮 / DPI / IME / 焦点）；采样与 registry 缝均 `#if UNITY_EDITOR`，**Player 构建内行为未验证**。
8. **guard 进程级**：`SaveSlotGuard` / `SaveFileGuard` 是测试内协议，不是进程级文件锁；进程 abort 时依赖人工取证。
9. **UI 文案像素 / 1920 实画面**：本轮未做字体测量或渲染探针（禁止在资源 Scope 外运行）。
10. **Git**：未 Commit / 未 Push；工作区仍有既有 dirty / untracked，本包不宣称 Branch / Repository Closed。
11. **整包状态**：**Verification PARTIAL**，不是 Completed，也不是制作人已验收。
12. **既有受影响测试**：§11.1 列出的 6 个方法已在阶段 3/3 按新正式实体语义修订并编译（§19），但**尚未运行**；另有 1 个同因方法（`VillageWorldProductIntegrationPlayTests.Presence_DerivedAndWorldVisible_…`）**仍未修订**，宿主若跑该文件会看到真实失败，属已识别缺口而非新回归。
13. **完整 Edit / Play 未重跑**：本轮只交按类回归；1513 / 122 是初始化包之前的真实通过，不能推算当前全量状态。
14. **既有套件的存档保护不均**：`VillageCommitmentProductIntegrationPlayTests` / `VillageV4ResourceSurplusPlayTests` / `Building*ProductPlayTests` 等旧套件在 `SetUp/TearDown` 里直接删除或重写用户单槽（无 `SaveSlotGuard`），这是**既有行为**，不是本包引入；但它意味着「宿主跑全量 Play」会触碰用户档。新套件（`InitialWorldProductWiringTests` / `InitialWorldConfigurationTests` / `InitializationProductPlayTests` / `ProductHomeFoundationPlayTests` / `ManagementShellPlayTests`）已使用持久 guard。本轮未运行这些旧套件，也未改动其存档行为（修订属后续阶段，修订时不得再无保护删档）。
15. **本轮 SetUp 异常恢复未验证**：`try/finally` + `RestoreCapturedSaveAfterFailedSetUp()` 已编译（§10.4 反射核对存在），但异常路径本身**只能在真实失败时被触发**，本轮未运行，因此不声称已验证。
16. **视觉绑定提案未执行**：§17 只是静态提案，XLSX / Mirror / 生成资产本轮 0 改动；A/B 语义取舍需制作人 / 主开发确认，方案 B（新增表现键行）会改变 Compiler 行数期望。
17. **只读探针副作用**：`TestRunnerApi.IsRunActive` 不存在导致 1 次 snippet 编译失败（`sequence 98681–98683`），仅发生在 MCP 片段沙箱；已登记，不清洗。§16.2 的单槽读数来自本轮只读测量，与历史值相同**不构成**“历史值可复用”的依据。

---

## 15. 边界声明

- 仅操作授权 Pilot 工程与授权目录；未访问未授权工程、认证目录或其他工作区。
- 未安装组件、未改权限、未改计费渠道、未创建额外付费子 Agent / Workflow / Goal。
- 未 `rql-mutate`、未 Shell / `Process` / Git 写、未全局保存 / 场景保存 / `SetDirty` / `ClearDirty`、未 PrefabStage、未写 Preferences、未启停 Editor、未做业务或渲染冒烟。
- §16–§17 为本轮（阶段 1/3）增量，适用同一禁令；尤其**未改 XLSX / Mirror / 生成资产 / 产品代码 / 断言强度**。
- 本记录与实现须由主开发专业核验；**不代表制作人批准**，也不表示初始化整包可玩验收完成。

---

## 16. 阶段 1/3 增量：Kitchen 真实 UI 派工 / 加工 / 保存 / 继续专项（宿主交付）

> 范围：**只交这 1 个用例**的宿主执行；不重跑原 Farm 单例，不重做已实现 Kitchen 专项的用例体，不预填 PASS。

### 16.1 方法唯一可发现（不是猜的名字）

| 判据 | 实测 |
|---|---|
| `tests` 源精确查询 | `fullName == "InitializationProductPlayTests.DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue"` → **1 条**（`PlayMode` / `BattleStage1.PlayModeTests`） |
| 同名重载 | 无：唯一 `[UnityTest] public IEnumerator` 无参方法 |
| 类内 `[UnityTest]` | **2** 个（本专项 + 原 Farm 单例），名称不同；程序集反射与源码逐字一致 |
| `namespace` | `""`（全局命名空间）→ **`testClass` 过滤会 0 匹配**，必须 `testMethod=<FullName>` |
| DLL | `BattleStage1.PlayModeTests.dll` 625152 B / `55743eeb…1315dd`，mtime `2026-10-29T10:57:06Z` |

### 16.2 先 Capture 当前用户单槽（实际测量，不套历史 13134）

- 用例内：`[UnitySetUp] SetUp()` 的**第一条语句**就是 `_guard.Capture()`，早于 `ResetForTests` 与 `LoadScene`。
- 执行层本轮只读捕获（**未写、未删、未改**）：

| 项 | 实测 |
|---|---|
| 路径 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json` |
| 存在 / 字节 / SHA256 | `True / 13134 / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08` |
| 同目录 `*.tmp` | **0**（无半写临时文件） |
| 本轮判定 | 数值恰与历史记录一致，但**以本轮实测为准**；不把历史值当作前提 |

### 16.3 现有恢复链复核（逐环，代码级）

| 环节 | 实际行为（`InitializationProductPlayTests.SaveSlotGuard`） | 复核结论 |
|---|---|---|
| `Capture()` | 先 `Assert.IsFalse(_captured)`；`BlockOnUnfinishedRuns()`；从 `SaveSystem.GetSavePath()` 取真实路径；建 run 目录（重名追加 `-2`）；读当前字节 → 用 `FileMode.CreateNew` 写 `original-save.bin` → 回读字节数与 SHA；**再复核当前档 hash 未被改写**才继续；最后写 `status=captured/absent` manifest | 捕获是“先备份 + 回读 + 防并发改写”，不是只记路径 |
| `Restore()` | 必须已 `captured`；幂等短路（先核当前档是否已等于基线）；**只读本 runID 的 `original-save.bin`**；写回前再验备份 SHA；写/删后回读比对；manifest `restored/restore-failed`；`Assert` 失败保留备份与 manifest | 不用历史备份、不猜路径、不静默 |
| `BlockOnUnfinishedRuns()` | 遍历保护根；白名单仅 `restored` 与 `protective-snapshot`；`manifest` 缺失 / 不可解析 / 其它状态（含 `captured`、`restore-failed`）→ **直接 Assert 阻止新 run** | 未完成 run 阻断在位 |
| `TearDown()` | `try { _guard?.Restore(); } finally { try { ResetForTests(); } finally { RestoreActivitySchedule(); } }` | 存档恢复先于重置；日程恢复放最外层 `finally`，前序异常不短路 |
| **本轮新增** | `SetUp` 的 `Capture` 之后改为 `try/finally`：`ResetForTests`/`LoadScene`/等待帧失败 → `RestoreCapturedSaveAfterFailedSetUp()` 用本 runID 备份恢复 + 写 `SAVE-RESTORE-SETUP-FAILURE`；恢复自身失败只告警 + 证据，不掩盖原始异常 | 补上“NUnit 不保证 SetUp 失败后执行 TearDown”的真实缺口（依据见 §8.4） |
| 未改 | 备份/恢复算法、白名单、manifest 字段、证据格式、TearDown 顺序、任何断言 | 仅补异常路径 |

本轮保护面实测（详表见 §9.1）：guard `67/67 restored`、R3B `345 = 344 + 1 protective-snapshot`、无 `captured` / `no-manifest` / `other` → 不阻断；字体 9 子资产 0 dirty；34/34；场景干净；Editor / 原生 runner / pending 均空闲。

### 16.4 真实 UI 链路（不调用回调替代点击）

每个点击都由 `RealPointerClick` 断言：`Canvas.ForceUpdateCanvases` → 目标中心 `WorldToScreenPoint` → `EventSystem.RaycastAll` **top hit 必须是目标或其子节点** → `ExecuteEvents.GetEventHandler<IPointerClickHandler>` 存在 → `ExecuteEvents.Execute` 返回 `true`。

顺序（全部真实控件，无 `onClick()` 直接调用、无领域回调替代）：

1. `MainMenuUI.btnNewGame` → HeroSelection；
2. HeroSelection 首张英雄卡 → `btnConfirm`；
3. Shell `ProductNavigation/Navigate2` → `Surface=="Management"`；
4. 页签 `ModalPanel/Tab_Population` → `ActiveTab=="Population"`；`ModalPanel/Tab_Workplaces` → `"Workplaces"`；
5. 人口行第 0 / 第 1 行 → `SelectedPersonId` 非空且两人不同；
6. **默认实例** Kitchen / Farm 岗位行（行序 = 管理快照真实岗位序；`content.childCount == rows.Count`）→ `SelectedWorkplaceId` 等于目标实例岗位；
7. `assignButton` → `Assignments.GetWorkplaceId(person) == workplace` + `statusMessage` 含“已绑定”；
8. `saveButton` → `statusMessage` 含“已保存” + 单槽文件真实存在；
9. `btnContinue` → 新 `ProductHomeWorldHost` 实例（`GetInstanceID` 不同）。

**不做**：`AddIngredients` 伪造原料、直接写 `Work`/`Resources`、直接改 Clock 静态字段、跳过 `Gateway/Management` 私有反射。

### 16.5 「默认生成实例」从实际快照派生（不写死 ID / 数量）

- `FirstCompletedKitchen(out buildingId)` / `FirstCompletedFarm(out buildingId)`：遍历 `ProductHomeWorldHost.Snapshot.Buildings`，按 `DefinitionId` + `LifecycleState=="Completed"` 取**实际** `InstanceId`，拼 `workplace:<InstanceId>:main`；断言 Farm / Kitchen 是两个不同实例、岗位已注册、Kitchen 是资源岗位（Ingredients→Food）。
- 断言 `buildings.Count == configuredBuildings.Count`（配置可调，不硬编码 7 / 5 / 1）。
- 断言 Continue 后 4 个历史静态 `ws_*` 既无定义、无 Place、无资源职责（正式实例档不重种）。

### 16.6 保留的劳动 / 转换 / 结算 / 恢复强断言

- **活动窗口隔离是测试初始化，不是正式入口**：先捕获 `VillageRuntimeBridge.ActivitySchedule` 实例，再注入 `IntradayActivitySchedule([], 15, 30, null)`（零机会点），TearDown 断言恢复为**同一实例**并写 `SCHEDULE-RESTORE sameInstance=True`；**未改产品默认日程**。
- Farm 41 工作分钟后：`WorkEvidenceMinutes(farmPerson, farmWorkplace) > 0`（真实劳务证据，不是伪造）。
- Kitchen：有界循环（≤10 步、每步 ≤40 分钟、且不越过 `Lunch-1`）直到 `Food` 增加；断言 `Food` 增加、`kitchenMinutes > 0`、**两个实例都留下资源批次**；不硬编码“每 40 分钟产 1”的比例，窗口上限由午休边界兜底。
- Save 前记录：单槽 hash、建筑 ID 串、Food / Ingredients / 批次 / 证据条数 / 双方分钟数。
- Continue 后逐项等值：单槽 hash 不变、建筑集合不变、双方职责恢复、Food / Ingredients / 批次 / 证据条数 / 双方分钟数相等；`ws_*` 未重种。
- 断言强度：**未削弱、未跳过**；`41 / 40` 只是既有 profile 速率下的有界推进入口，不是冻结比例。

### 16.7 宿主执行与失败口径

| 项 | 内容 |
|---|---|
| 执行参数 | `testMode=PlayMode` + `testMethod=InitializationProductPlayTests.DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` |
| 通过标准 | **1/1 Passed、0 skipped**；0 项发现 / skip 不算通过 |
| 不跑 | 原 Farm 单例、管理 17、Home Edit 4、Edit 9、Compiler 280、完整 Edit/Play |
| 预期副作用 | 新增 1 个 Initialization guard run（`captured → restored`，逐字节回读）；证据日志追加 `KITCHEN-WIRING / KITCHEN-ASSIGN / KITCHEN-LABOR / KITCHEN-SAVE / KITCHEN-CONTINUE / KITCHEN-END` 与 `FONT-PRE/POST`；无截图（本用例不截图） |
| 失败处理 | 按真实 `message` / `stack` 判：夹具问题改夹具；落在 Save / Load / 投影 / 资源结算才做产品窄修；**不得削弱已冻结断言**；不无限重试；单次失败先留证再定最窄修复 |
| 本轮状态 | **未运行**（本执行层禁止 `tests-run`）；不预填 PASS / FAIL |

---

## 17. 视觉绑定修复精确提案（静态，未执行；供维护核对窄 Compiler 包使用）

> 前提：`HomePresentationResolver` 忠实消费生成的 `BuildingPresentationBinding`；A 白模档缺美术是**内容 / 绑定**问题，不是解析 bug（§3.2–§3.3）。唯一正确链路是 **XLSX → Compiler → Canonical Mirror → Builder**；**禁止手写 Mirror**。本执行层**未改** XLSX / Mirror / 生成资产。

### 17.1 现状（本轮直接从 XLSX 读取，不只看 Mirror）

`Development/Config/RRPG_VillagePresentation.xlsx`：`5458 B / 0ee4d28cfcf8f8e45094ab29aec8456d23ef8d56d7271384091ddd3594ea2ba9`。

- sheet `VillagePresentation`：15 个数据行（行 5–19）。
- sheet `BuildingPresentationBinding`：8 个数据行（行 5–12），现状：

| XLSX 行 | BuildingDefinitionId | Lifecycle | 现值 PresentationKey | 现指向的 VisualAsset | 现图性质 |
|---:|---|---|---|---|---|
| 5 | `building_farm` | UnderConstruction | `village.building.farm.construction` | `va_village_farm_basic` | TECH PROOF（与 Completed 同图） |
| 6 | `building_farm` | **Completed** | `village.building.farm.basic` | `va_village_farm_basic` | **TECH PROOF / NOT FINAL ART** |
| 7 | `building_house` | UnderConstruction | `village.building.neutral` | `va_village_neutral_marker` | 白模 |
| 8 | `building_house` | **Completed** | `village.building.neutral` | `va_village_neutral_marker` | **白模** |
| 9 | `building_kitchen` | UnderConstruction | `village.building.neutral` | `va_village_neutral_marker` | 白模 |
| 10 | `building_kitchen` | **Completed** | `village.building.neutral` | `va_village_neutral_marker` | **白模** |
| 11 | `building_workshop` | UnderConstruction | `village.building.neutral` | `va_village_neutral_marker` | 白模（历史工坊，保留） |
| 12 | `building_workshop` | Completed | `village.building.neutral` | `va_village_neutral_marker` | 白模（历史工坊，保留） |

> 说明：`BuildingPresentationBinding.json` 的 `_generated.sourceSha256=d7e6ca3e…` 与本轮 XLSX 文件 SHA `0ee4d28c…` 数值不同；**本轮不据此判断新陈旧**（Compiler 的内嵌摘要算法不可从仓库反推）。可核的事实是**内容级一致**：Mirror 的 8 行与 XLSX sheet2 的 8 行逐字段相同；Mirror 的 15 行与 XLSX sheet1 的 15 行集合相同（顺序不同）。

### 17.2 已存在的 VillageVisualAsset 键与源图当前 SHA（不新增任何资源）

| VisualAssetId | SourceFile（`Development/SourceAssets/` 与 `Assets/Generated/StaticConfig/Assets/` 两份同字节） | 字节 | 当前 SHA256 |
|---|---|---:|---|
| `va_village_farm_basic` | `Village/farm-proof.png` | 4633 | `d1c3f317cc8727b7a77ab44abbaff789c8c646c369a5a1bb6442546580cabbc9` |
| `va_village_neutral_marker` | `Village/neutral_marker.png` | 672 | `dc276b76186e00f94838b5274b169a89788b3cfeaa6bae512b9240ff1114a1a8` |
| `va_village_r2_farm` | `Village/R2Prototype/farm.png` | 1880781 | `1b9d8d5d1bdb761875d24017c2d15f4621ea619b37ffaed1a82d14d6901f4d6c` |
| `va_village_r2_home` | `Village/R2Prototype/home.png` | 1566839 | `f255598eb987b7128b01cb748cf52c0f5611200d21e47dbbbab5efc485cbf748` |
| `va_village_r2_kitchen` | `Village/R2Prototype/kitchen.png` | 1889491 | `d342c5b69852e9c9f02d30cb62730b184b0ad65d445c221029997edf7b26207f` |
| `va_village_r2_workshop` | `Village/R2Prototype/workshop.png` | 1585255 | `c3619ccc9d2de6233dc5244c8b5920fea4c2c22d624147c951c4dfa1bcc3172c`（**保留，不改绑定**） |
| `va_village_r2_person` | `Village/R2Prototype/person.png` | 733634 | `59ba92f9ad70365c018b24fbdc74ba6992fd85c18be2c9e6451f7b3eb65cfc30`（本提案不用） |

对应 `VillagePresentation` 行（已存在，无需新增）：`village.building.r2.building_farm → va_village_r2_farm`（行 17）、`village.building.r2.building_house → va_village_r2_home`（行 16）、`village.building.r2.building_kitchen → va_village_r2_kitchen`（行 18）。

### 17.3 精确提案（推荐方案 A：只改 3 个 Completed 单元格，0 新增行、0 新增资源）

| XLSX 行 | Key | 现值 | **建议值** | 目标 VisualAsset | 源图当前 SHA | 是否涉及 UnderConstruction |
|---:|---|---|---|---|---|---|
| 6 | `building_farm` / **Completed** | `village.building.farm.basic`（TECH PROOF 图） | **`village.building.r2.building_farm`** | `va_village_r2_farm` | `1b9d8d5d…1f4d6c` | **否**：行 5 `UnderConstruction` 保持 `village.building.farm.construction` |
| 8 | `building_house` / **Completed** | `village.building.neutral`（白模） | **`village.building.r2.building_house`** | `va_village_r2_home` | `f255598e…cbf748` | **否**：行 7 `UnderConstruction` 保持 `village.building.neutral` |
| 10 | `building_kitchen` / **Completed** | `village.building.neutral`（白模） | **`village.building.r2.building_kitchen`** | `va_village_r2_kitchen` | `d342c5b6…26207f` | **否**：行 9 `UnderConstruction` 保持 `village.building.neutral` |
| 11–12 | `building_workshop` 两行 | `village.building.neutral` | **保持不动** | `va_village_neutral_marker` | `dc276b76…14a1a8` | 保留历史工坊，不新增/不删除 |
| 5 / 7 / 9 | farm / house / kitchen 的 `UnderConstruction` | 见 17.1 | **保持不动** | — | — | **明确不改**：仓库无在建造美术，用已完工图冒充在建属语义错误 |

方案 A 的净效果：A 白模档下 **默认 Completed Farm / Kitchen / House 显示既有真图**；`UnderConstruction` 与工坊仍是白模；**新增资源 0、删除资源 0、新增表行 0、手写 Mirror 0**；`VillagePresentation` 表完全不动。

### 17.4 必须由制作人 / 主开发确认的后果与备选

1. **A/B 视觉收敛**：A 档绑定改成 `village.building.r2.*` 后，与 B 原型 `PrototypeEnabled` 派生键相同 → 已完工建筑在 A/B 两档显示**同一张图**；A/B 差别只剩人物 / 地点。若「A 必须是可区分白模档」仍是硬要求，则不能选方案 A。
2. **备选方案 B（保留 A/B 键层区分）**：在 `VillagePresentation` **新增 3 行**（如 `village.building.farm.final` / `house.final` / `kitchen.final` → 同一批 `va_village_r2_*`），再让 sheet2 的 3 个 Completed 行指向新键。仍然**不新增美术资源**，但表由 15→18 行，随机生成面与 Compiler 行数期望变化，需要维护核对窄包一并确认。
3. **Person 未纳入本提案**：A 档人物用 `village.person.<PresentationIntent>`，生成 presentations 只有 `village.person.r2`，因此 A 档人物仍 `neutral_marker`；要修需新增 person 表现键行（非新增美术），属另一决策。
4. **Place 键缺口**（§3.4 尾注）：A 档 `village.place.<Kind>` 无生成键；默认世界当前不独立绘制 Place，本轮不改。

### 17.5 重新生成链与验收检查点（由维护核对窄 Compiler 包执行；本执行层禁止 Shell / 手写 Mirror）

```text
dotnet run --project Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj -c Release -- validate --root D:\AI_Agent_Lab\rrpg_pilot\repo
RRPG_一键导表.cmd            :: 或 Unity 菜单 RRPG/Static Config/M0 · Build Generated Root
```

验收检查点（宿主 / 维护包）：

1. `Development/Generated/StaticConfig/BuildingPresentationBinding.json`：**只有 3 行 `PresentationKey` 值变化**，其余 5 行与 `sourceSha256` 头部按 Compiler 规则更新；行数仍 8。
2. `VillagePresentation.json`：方案 A 下**数据行不变**（仍 15 行、键与 VisualAssetId 逐项相同）；`_generated.sourceSha256` 是否变化取决于 Compiler 的摘要范围（本轮未验证，**不得预设不变**）。
3. `Assets/Generated/StaticConfig/Databases/VillagePresentationDatabase.asset`：`buildings` 中 farm/house/kitchen 的 `Completed` 项指向新键；workshop 与所有 `UnderConstruction` 项不变；资产由 Builder 生成，不手改。
4. Compiler 回归：方案 A 无新增行 / 无新增引用键，期望仍 **280/280、0 skip**；若行数改变（方案 B），以维护包的新期望为准。
5. 视觉复核：A 档真实入口截图 / 目视（宿主 Scope 内执行），确认 farm / kitchen / house 显示既有真图、无重复旧 Place、无假 workshop。
6. 测试联动：视觉变更必须与 §11.1 的 6 个旧 Play 方法修订**同期**验证；`VillageVisualPrototypePlayTests` 的「A whitebox remains the default」语义在方案 A 下不再成立，需按新语义修订（不得留旧断言）。

### 17.6 当前结论

- **视觉缺口仍未解决**：本轮只给静态提案，未生成、未改绑定；默认 A 档 Farm 仍是 TECH PROOF 图、Kitchen / House / Person 仍是 `neutral_marker`。
- 因此**不得宣称初始化整包完成**，也不得把 §17.3 当作已实施。
- 本轮不预填任何 Compiler / Builder / 测试结果。

---

## 18. 阶段 2/3 增量：数量解冻后实例接线九例（宿主交付）

> 范围：回看 Kitchen 真实结果（必要窄修）→ 交 `InitialWorldProductWiringTests` 九例宿主执行。本执行层**未运行任何测试**；九例结果只能由宿主真实回执决定。

### 18.1 Kitchen 专项宿主回执（回看：通过，无需定位窄修）

| 项 | 实测 |
|---|---|
| 结果 | `testResults` 当前唯一回执：`InitializationProductPlayTests.DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue` → **Passed / 1845 ms / message=null** |
| 证据日志 | `Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-play-evidence.log` 第 16–26 行（`2026-10-29T11:00:32Z`–`11:00:34Z`） |
| 真实实例 | `[KITCHEN-WIRING] kitchenBuilding=building_instance_2 kitchenWorkplace=workplace:building_instance_2:main farmBuilding=building_instance_1 farmWorkplace=workplace:building_instance_1:main runtimeBuildings=7`（**从运行时快照找到的真实 ID，不是写死**） |
| 真实 UI 派工 | `[KITCHEN-ASSIGN] kitchenPerson=person_f0b0e83d… kitchenWorkplace=workplace:building_instance_2:main farmPerson=person_df89bc68… farmWorkplace=workplace:building_instance_1:main` |
| 真实转换 | `[KITCHEN-LABOR] minute=0->561 food=0->1 ingredients=0 farmMinutes=41 kitchenMinutes=41 batches=2 evidence=2`（**原料被消耗**：Farm 产出后 Kitchen 消耗为 0，Food 由 0→1） |
| 保存 / 继续 | `[KITCHEN-SAVE] sha256=84d24f53… batches=2 evidence=2 kitchenMinutes=41 farmMinutes=81`；`[KITCHEN-CONTINUE] sameBuildings=True … ingredients=0 food=1 batches=2 evidence=2`；`[KITCHEN-END] uiKitchenAssign=True uiFarmAssign=True conversion=True saveContinue=True noReseed=True` |
| 保护面 | `[SAVE-RESTORE] runId=20261029T110032Z matches-before=True`、`[SCHEDULE-RESTORE] sameInstance=True`、`[FONT-POST] unchanged=True`；guard 目录该 run `status=restored` |
| 结论 | **无需窄修**；断言强度、真实 UI 链、资源转换与 Continue 等值均按 §16 设计落地，未出现需要定位的产品 / 夹具缺陷 |

### 18.2 本轮唯一源码修复：两个测试守卫的 manifest 格式不兼容（源码级必然阻断）

**事实链（全部来自源码与磁盘实测）**：

1. `InitialWorldConfigurationTests.SaveFileGuard` 与 `InitializationProductPlayTests.SaveSlotGuard` **共用同一保护根** `Development/StudioRuns/Initialization-20261027/save-guard`。
2. 两者写出的 manifest 格式不同：
   - `SaveFileGuard` 紧凑写：`{"status":"restored","sourcePath":…}`；
   - `SaveSlotGuard` 缩进写：`"status": "restored"`（冒号后有空格，见 `save-guard/20261029T110032Z/manifest.json`）。
3. `SaveFileGuard.BlockOnUnfinishedRuns()` 旧实现是 `File.ReadAllText(manifest).Contains("\"status\":\"restored\"")` —— **对缩进写永远不匹配**。
4. 保护根现有 **5 个缩进格式（SaveSlotGuard 写）** 的已完成 run：`20261028T114612Z`、`20261028T114756Z`、`20261028T114757Z`、`20261028T114758Z`、`20261029T110032Z`。
5. 因此 `InitialWorldProductWiringTests` 九例（以及 `InitialWorldConfigurationTests` 十例）的 `SetUp` 第一步 `Capture()` 就会抛「存在未完成的初始世界存档保护 run」→ **九例在断言执行前必然全部失败**。这是本次新增 Kitchen 用例（同一保护根）写档后引入的**真实夹具阻断**，不是产品缺陷。

**修复（最小面，不放松语义）**：判定改为「去掉空白后精确匹配 `"status":"restored"`」，只解决两种已完成格式的互认；`captured` / `capture-failed` / `restore-failed` / `missing-manifest` 仍然阻断。

**验证**：反射调用修复后的 `SaveFileGuard.BlockOnUnfinishedRuns()`（只读 manifest，不写任何文件）→ **不抛异常**；按旧实现的同一字符串逻辑，对上述 5 个缩进 manifest 必然抛。

**未改**：备份 / 恢复算法、唯一目录规则、`CreateNew` 备份、回读校验、白名单语义（仍只认 `restored`）、任何断言；也**没有**去改 `SaveSlotGuard` 的写格式（避免动已通过 Play 用例的写入路径）。

### 18.3 九例逐例源码审查（哪些由配置 / profile 派生，哪些强语义原样保留）

| # | 用例（`RRPG.Initialization.Tests.InitialWorldProductWiringTests`） | 数量 / 容量来源（不冻结） | 保留的强断言 |
|---|---|---|---|
| 1 | `DefaultGeneratedNewGameCreatesSevenRealBuildingsWithoutDuplicateInitialization` | `expected.Buildings.Count`、每种定义计数、`expected.People.Count`；方法名里的 "Seven" 只是历史发现契约 | 实例定义与配置同源、每种定义数量同源、必须正式可建造、无 workshop / smithy、必须含 Farm / Kitchen、全部 `Completed`、`InstanceId` 唯一、重复 NewGame 不累加且 ID 序列相同、人物不重复 |
| 2 | `DefaultNewGameAssignsInstanceFarmKitchenAndHousingHasNoWorkplace` | 实例 ID 从 `Buildings.All` 取；容量 = `ProfileCapacity(FunctionProfile(building, "main"))`（`_testWorkplaces` 单一权威） | Farm / Kitchen 岗位真实注册、Housing **无岗位**（派工返回 `UnknownWorkplace`）、资源角色 Farm / Kitchen、三人可合法派工 |
| 3 | `NewWorldDropsStaticWorkplacesAndLegacySaveKeepsStaticAssignment` | 无数量断言 | 正式世界无 `ws_farm_01/ws_kitchen_01/ws_test_01/ws_test_02` 定义与 Place；旧兼容档可派工并保留职责；旧档 Continue **不补建**（0 建筑） |
| 4 | `WorkshopIsOfflineForFormalCommandsButLegacyInstanceStillLoads` | 无数量断言 | 正式目录拒绝 workshop / smithy 且诊断含“下线”；旧实例 `TryAddInitialCompleted → Save → Load` 恢复且恢复后仍不开放正式命令 |
| 5 | `HomeSnapshotUsesSameBuildingIdentityWithoutDuplicateHousingOrInstanceWorkplaces` | 住房数 = 默认配置中 `Category==Housing` 的定义数（且 >0）；`usedHousing ≤ housingIds.Count` | 快照与运行时同 `InstanceId` / 定义；实例岗位 Place 绑定真实实例且 `DrawsWorldEntity=false`；Home 只绑真实 Housing；同一实例不得以 Building + Place 画两套 |
| 6 | `FormalSaveLoadsDeterministicInstanceCatalogueInColdAndWarmSessionWithProductionEvidence` | 实例 ID 从运行时取；资源值来自真实 `TryBeginBatch/Work/ProcessBatch` 结算 | 480 分钟真实劳务证据、Ingredients > 0、冷 / 热会话 `WorldStateSnapshot` **全等**、无 `ws_*` 重种、职责 / 资源 / 批次 / 证据按档恢复、资源职责按建筑重建 |
| 7 | `LegacySaveWithoutBuildingsLoadsSameCompatibilityCatalogueColdAndWarm` | 无数量断言 | 旧兼容目录 4 岗位 + Place + Farm 资源职责；职责保留；Work 证据 `JsonUtility` 逐项等值；冷 / 热等值；Continue 不补建 |
| 8 | `FormalSaveRestoresOnlyLegacyWorkplacesItActuallyReferences` | 无数量断言（显式注入 1 个旧岗位定义） | 只恢复被引用岗位；未引用定义 / 资源职责 / Place 不得凭空重种；冷 / 热快照等值 |
| 9 | `HousingAliasesUseOnlyCompletedHousesAndStayStableAcrossOrderAndContinue` | 住房集合从运行时 `Completed + Housing` 派生 | 未完成住房不入别名且不改变既有别名；人员顺序反转不跳房；Continue 不跳房；别名只指向 `Completed` 住房 |

### 18.4 口径澄清（不是断言放松）

- **不写死** 7 / 5 / 1 / 2 / 1：数量来自生成配置，容量来自 `FunctionProfileId → VillageWorkplaceDef.Capacity`（`VillageRuntimeBridge.Buildings.cs:27` 与 `_testWorkplaces` 同一权威）。
- `Assert.GreaterOrEqual(characters.Count, 3)` 是本用例「需要 3 个不同人物」的**前置条件**，不是人口比例：配置不足 3 人时用例明确失败，不静默跳过。
- 用例 5 的「配置 Housing 数 == 运行时已完成住房数」在**当前语义**下成立：初始建筑没有生命周期字段，`TryAddInitialCompleted` 一律创建 `Completed`。若将来配置支持非 Completed 初始建筑，该断言必须按生命周期过滤（已登记为风险，不改默认配置）。
- 九例的存档保护：`SetUp` 先 `Capture()`（持久 run 目录 + `CreateNew` 备份 + 回读 SHA），失败 `catch → Restore → 重新抛出`，`TearDown` 在 `finally` 恢复；未完成 run 阻断（§18.2 修复后对两种已完成格式互认）。

### 18.5 编译 / 发现（本轮实测）

| 项 | 实测 |
|---|---|
| 改动源码 | `Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs`，**44666 B / `cd126162b80573d8098889a388329778dc42723e254adcf0030a9a11de5da289`**（mtime `2026-10-29T11:01:43Z`） |
| 程序集 | `InitialWorld.Tests.dll` **71168 B / `1e3dbb182bb919635fab1453807b1b17a458befcc618f18f9d3953d81d026814`**（mtime `2026-10-29T11:01:55Z`） |
| `tests` 源 | 前缀 `RRPG.Initialization.Tests.InitialWorldProductWiringTests.` → **9 条**，全部 `EditMode` / `InitialWorld.Tests` / `explicit=false` / `ignored=false` / `parameterized=false` |
| 类过滤 | `namespace=RRPG.Initialization.Tests`（非全局）→ `testClass=InitialWorldProductWiringTests` **可用** |
| 安全刷新 | `AssetDatabase.Refresh(ForceSynchronousImport)` ×1（本轮唯一一次）；刷新后 `isCompiling=False / isUpdating=False / isPlaying=False` |
| 只读守卫模拟 | `SaveFileGuard.BlockOnUnfinishedRuns()` 反射调用 → 通过（无 unfinished） |

### 18.6 宿主执行参数

| 项 | 内容 |
|---|---|
| 执行参数 | `testMode=EditMode` + `testClass=InitialWorldProductWiringTests`（整类 **9 例串行**） |
| 通过标准 | **9/9 Passed、0 skipped**；任何 0 项 / skip 都不算通过 |
| 逐条 fullName（备用） | `…DefaultGeneratedNewGameCreatesSevenRealBuildingsWithoutDuplicateInitialization`、`…DefaultNewGameAssignsInstanceFarmKitchenAndHousingHasNoWorkplace`、`…NewWorldDropsStaticWorkplacesAndLegacySaveKeepsStaticAssignment`、`…WorkshopIsOfflineForFormalCommandsButLegacyInstanceStillLoads`、`…HomeSnapshotUsesSameBuildingIdentityWithoutDuplicateHousingOrInstanceWorkplaces`、`…FormalSaveLoadsDeterministicInstanceCatalogueInColdAndWarmSessionWithProductionEvidence`、`…LegacySaveWithoutBuildingsLoadsSameCompatibilityCatalogueColdAndWarm`、`…FormalSaveRestoresOnlyLegacyWorkplacesItActuallyReferences`、`…HousingAliasesUseOnlyCompletedHousesAndStayStableAcrossOrderAndContinue` |
| 不跑 | Kitchen 专项（已 1/1 Passed）、原 Farm 单例、管理 17、Home Edit 4、Compiler 280、完整 Edit / Play |
| 预期副作用 | 9 个 guard run（`captured → restored`，逐字节回读）；**不写字体**（Edit 类不碰字体）；不改场景 / Prefab；用户单槽在每例结束后回到 `13134 B / adcf5d8b…` |
| 失败处理 | 按真实 `message` / `stack` 窄修：夹具问题改夹具，落在 Save / Load / 投影 / 资源职责才改产品；**不得削弱九例强断言**；不无限重试 |

### 18.7 本轮保护面 / 字体状态（实测，2026-10-29T11:0xZ）

| 项 | 实测 | 判定 |
|---|---|---|
| 用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，mtime `2026-10-29T11:00:34Z`（Kitchen 用例恢复写回），同目录 `*.tmp` = 0 | ✅ 逐字节回到基线 |
| Initialization guard | `dirs=68 / restored=68 / protective=0 / other=0 / noManifest=0`（68 = 上一轮 67 + Kitchen run） | ✅ 无 unfinished / 无未知 |
| R3B guard | `dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective 为既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体 | 磁盘 `14961033 B / 3c758e16…c404854`；`LoadAllAssetsAtPath` = **9** 子资产、`EditorUtility.IsDirty` = **0** | ✅ 无人为写入、无 pending |
| 34 项基线 | `34/34`（changed=0 / missing=0） | ✅ |
| Editor / 场景 / Prefab | `isPlaying=false / isCompiling=false / isUpdating=false / selection=0`；`SampleScene dirty=False roots=5`；`PrefabStage=null` | ✅ 空闲干净 |
| 原生 runner / pending | `Tool_Tests.HasPendingTestRun=False` | ✅ |
| Console | 过滤 `Error/Exception` → **0**（阶段 1 记录的 3 条只读探针错误已不在当前缓冲区；不据此改写历史） | ✅ 项目 / 测试程序集 0 错误 |

### 18.8 未验证 / 风险（本阶段不写 Completed 的理由）

1. **九例未运行**：本轮禁止 `tests-run`，9/9 只能由宿主回执决定；三例数量解冻改动（§8.2）从未执行过。
2. **守卫跨格式修复未在真实测试路径验证**：只做了只读反射模拟；真实触发点是九例 / 十例的 `SetUp`。
3. **`InitialWorldConfigurationTests` 同样受益也同样的未验证**：它的十例与九例共用该守卫；本轮不重跑（历史回执 `ff7dc4eb` 10/10 在修复前）。
4. **用例 5 的住房计数前提**：依赖「初始建筑一律 Completed」的现有语义（§18.4），未新增规则。
5. **视觉缺口未解决**（§17）：A 档 Farm / Kitchen / House / Person 仍需 XLSX + Compiler 由维护窄包处理，整包不得标完成。
6. **本轮未重跑**：完整 Edit / Play、Compiler 280、管理 17、Home Edit 4、原 Farm 单例。

---

## 19. 阶段 3/3 增量：受影响旧 Play 六方法修订（宿主先交施工两例）

> 范围：按新正式实体语义修订六个受影响方法，保留旧业务强断言；**先交 `BuildingConstructionProductPlayTests` 整类两例**，其余三类写准确 class / fullName 待后续精准门禁。本轮**未运行测试**，不预填 PASS。

### 19.1 逐例修订（读真实源码后决定派生方式）

| # | 类 / 方法 | 原失败原因（源码） | 本阶段改法 | 保留的旧强断言 |
|---|---|---|---|---|
| 1 | `RRPG.Building.Tests.BuildingConstructionProductPlayTests.HomeWorkersActualLaborTwoIndependentFunctionsAndContinue` | 派工写死 `ws_farm_01`/`ws_kitchen_01`（正式世界 `UnknownWorkplace`）；`Snapshot.Buildings.Count == 2`（现为默认 7 + 2） | 从 `Snapshot.Buildings` 找 **Completed 实例** → `workplace:<InstanceId>:main`，先断言两者不同实例；计数改为 **`baseline+2` 相对基线** | 施工两独立职能、真实劳动证据、`construction:<id>` 到场、撤出/重入、视图 `AreSame` 稳定、Save 不含 `construction:` PlaceId / `FunctionProfileId`、Continue 后职责与占用恢复 |
| 2 | `…ZeroWorkersAndLateLoadFailureRetainsWholeConstructionWorld` | `NewGame` 后断言 `Instances.Count==0` 且无 `workplace:` 定义（现为默认 N 建筑 + 实例岗位） | 记录 `baselineBuildings` / `baselineWorkplaces`，NewGame 后断言**回到同一相对基线**、岗位全部是 `workplace:`、无静态兼容岗位 | 零工人、未来标记注入 Load 被拒绝、**世界逐字节不变**（`before==after`）、`ActiveProjects` 空、无半状态 |
| 3 | `VillageCommitmentProductIntegrationPlayTests.VillageTMP_LabelsRender_AndKitchenRowsRemainStableAcrossRefresh` | 找 `WorkplaceRow_ws_kitchen_01` 并 `OnAssignClicked("ws_kitchen_01")` | 行名与派工目标改为**默认实例 Kitchen 岗位**；占用文本由 `GetWorkplaceDef(...).Capacity` 派生（`"1 / "+cap`），不写死 1 | 字体权威、TMP 字符可见、暂停/倍速标签、Kitchen 行 **InstanceID 与锚点 90 帧稳定**、人员行稳定、移除后占用归零 |
| 4 | `VillageCommitmentProductIntegrationPlayTests.IntradayWork_RealTownAssignment_CommutingLaborLunchVisible`（partial 文件 `IntradayWorkProductPlayTests.cs`） | 找 `WorkplaceRow_ws_farm_01` 并断言 `ws_farm_01` | 改为**默认实例 Farm 岗位**行与职责 | 真实 UI 点击派工、`岗位劳动` / `午休` / `下班 / 休息` 文本、午餐后资源不变、面板可见可读 |
| 5 | `VillageV4ResourceSurplusPlayTests.FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry` | 删档后 `TryAssignPerson(...,"ws_farm_01"/"ws_kitchen_01")` | 改派**默认实例 Farm / Kitchen 岗位**（断言两实例不同）；删档改为 **guard 捕获 + 真实 NewGame+Save** | 3 天逐日数值追踪（Farm 产出 / Kitchen 加工 / 在村人数）、Food Net Delta > 0、Jump 后显著积累、准备界面补给文本、真实 Carry 且 Village Food 恰好减少 Carry、Journey 非 0 补给、Retreat 收尾 |
| 6 | `RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests.ActivityPlacesCannotReflowRegisteredWorkplaces` | 直接索引 `PlaceViews["ws_farm_01"…"ws_test_02"]`（正式世界不再创建） | 锚点集合改为**从运行时快照派生**的 `workplace:` PlaceIds（断言非空且每项有视图） | Tavern 出现/消失 3 轮循环中**同一视图实例、同一锚点、同一 sprite、Body 可见**、`Domain()`（模拟/存档）逐字节不变、A/B 往返锚点不变 |

### 19.2 测试 initialization 与正式入口的分工（未偷换成回调）

| 环节 | 做法 |
|---|---|
| 用户档保护 | 五个文件（含 partial）统一 `SaveSlotGuard`：**先 Capture**（持久 run 目录 + `CreateNew` 备份 + 回读 SHA）→ 未完成 run 阻断 → 异常 SetUp 恢复 → `TearDown` 在 `finally` 恢复并回读校验 |
| 测试 initialization | 用**真实** `SaveSystem.NewGame()` + `Save()` 建立隔离世界（旧实现是直接删除用户档）；`VillageV4` / `VillageCommitment` 的删档调用已移除 |
| 正式入口 | 仍走产品路径 `MainMenuUI.OnContinue`（读档 → EnterTown → Initialize），未改成直接构造世界 |
| UI | 保留旧用例既有的点击方式（`onClick.Invoke` / `ExecuteEvents` 指针）；**没有新引入**“直接调用回调代替 UI”的捷径；VillageCommitment 的 `OnAssignClicked` 是既有控制器入口，本次只改传入的岗位 ID |
| 零建筑 / 旧兼容世界 | 由 `InitialWorldProductWiringTests` 用**显式兼容通道**（`EnsureInitialized` / `EnsureInitializedWithExtraWorkplaces`）构造；本阶段**没有**把默认配置改回空，也没有恢复全静态 |

### 19.3 编译与发现（本轮实测）

| 项 | 实测 |
|---|---|
| 源码 | `BuildingConstructionProductPlayTests.cs` 18740 B / `e6be723b…ac91`；`VillageCommitmentProductIntegrationPlayTests.cs` 41189 B / `477f12ba…5c73`；`IntradayWorkProductPlayTests.cs` 2602 B / `e8bb14f5…a76d`；`VillageVisualPrototypePlayTests.cs` 17382 B / `e159324f…46c`；`VillageV4ResourceSurplusPlayTests.cs` 24548 B / `ec1b6d13…0761` |
| 程序集 | `VillageStage1.PlayModeTests.dll` 156672 B / `4876d119d034427e269b96e37f542f68704e676aff129cb2d459e6e12ff5e5a7`（mtime `11:08:22Z`）；`VillageV4.PlayModeTests.dll` 109056 B / `7b1b00515432b88d9e9eb26f119916139ac57552fe0bd0ced44cb630897c889f`（mtime `11:06:48Z`） |
| 安全刷新 | `AssetDatabase.Refresh(ForceSynchronousImport)` ×3（五个文件写完后一次、修 commitment SetUp 后一次、把 NewGame 移到 EnterTown 后一次）；刷新后 `isCompiling=False / isUpdating=False / isPlaying=False`；Console `Error/Exception` = 0 |
| 静态岗位残留 | 五个修订文件内已无 `ws_*` 派工 / 行名引用（仅保留说明性注释）；`ManagementShellPlayTests` 的 `ws_farm_01` 是**既有 CASE-B 显式兼容注入**，未改 |
| 发现（施工类） | `tests` 源：`RRPG.Building.Tests.BuildingConstructionProductPlayTests.*` → **2 条**，`PlayMode` / `VillageStage1.PlayModeTests` / `explicit=false / ignored=false`；类有命名空间 → `testClass=BuildingConstructionProductPlayTests` **可用** |
| 发现（其余） | VillageCommitment 类 **8 条**（其中 4 条为本阶段相关：`VillageTMP_…`、`IntradayWork_…`、`CommitmentUI_RealTown_…`、`CommitmentUI_ReflectsRuntime…`）；VillageVisualPrototype 类 **2 条**；`VillageV4ResourceSurplusPlayTests` **1 条**，均在 `tests` 源可见 |

### 19.4 宿主门禁（本阶段只交施工两例）

| 目标 | 参数 | 通过标准 |
|---|---|---|
| **施工产品整类（本轮交）** | `testMode=PlayMode` + `testClass=BuildingConstructionProductPlayTests`（2 例串行） | **2/2 Passed、0 skipped** |
| 其余三类（**待后续精准门禁**） | 逐条 `testMethod=`：`VillageCommitmentProductIntegrationPlayTests.VillageTMP_LabelsRender_AndKitchenRowsRemainStableAcrossRefresh`、`VillageCommitmentProductIntegrationPlayTests.IntradayWork_RealTownAssignment_CommutingLaborLunchVisible`、`VillageCommitmentProductIntegrationPlayTests.CommitmentUI_RealTown_AssignAndProject_ClosedLoop_PanelDoesNotPauseSimulation`、`VillageCommitmentProductIntegrationPlayTests.CommitmentUI_ReflectsRuntimeAfterSaveLoad_MidProject`、`RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests.ActivityPlacesCannotReflowRegisteredWorkplaces`、`VillageV4ResourceSurplusPlayTests.FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry` | 各自 1/1 Passed |

**注意**：`VillageCommitmentProductIntegrationPlayTests` / `VillageVisualPrototypePlayTests` 是**整类多例**，本轮只修订其中相关方法；若宿主按整类跑，会连带执行未修订方法（未验证），因此本阶段建议**按 testMethod 精准门禁**。

**预期副作用**：每例新增 1 个 guard run（`captured → restored`）；用户单槽每例结束回到 `13134 B / adcf5d8b…`；Edit 侧 `SaveFileGuard` 已对两种 manifest 格式互认（§18.2），不会再被新 run 阻断。

### 19.5 保护面 / 字体（本轮实测）

| 项 | 实测 |
|---|---|
| 用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，`*.tmp`=0 |
| Initialization guard | `dirs=77 / restored=77 / protective=0 / other=0 / noManifest=0`（68 + 宿主 Edit 九例的 9 个 run） |
| R3B guard | `345 = 344 restored + 1 protective`（既有 `20261024T052704Z`） |
| 字体 | 磁盘 `14961033 B / 3c758e16…`；9 子资产 `dirty=0` |
| 34 项基线 | `34/34` |
| Editor / 场景 / runner | 空闲、`PrefabStage=null`、`HasPendingTestRun=False`、Console 0 错误 |

### 19.6 未验证 / 风险

1. **六方法全部未运行**：修订后的真实行为（尤其施工完成天数、UI 行名带冒号、资源/远征数值）只能由宿主回执确认。
2. **新发现未修订的同因方法**：`VillageWorldProductIntegrationPlayTests.Presence_DerivedAndWorldVisible_WithoutAnyManagementPanel_And_SurvivesSimToggle` 仍引用 `ws_test_01`，跑该文件必失败；属下一窄包 / 精准门禁项。
3. **整类门禁风险**：commitment 类另 4 个未修订方法（`IntradayActivity_…`、`IntradayOnsite_…`、`CommitmentModal_…`、`CommitmentRows_…`）依赖旧兼容 UI 行为，本轮未改也未验证。
4. **`VillageV4` 数值口径**：实例岗位的 FunctionProfile 仍是 `ws_farm_01` / `ws_kitchen_01`（同一 Profile，容量 2/1），因此 1 Farm + 1 Kitchen 的产出公式不变；若 Profile 改动，断言须随 Profile 更新（未写死新数值）。
5. **guard 复制**：五个文件各有一份同协议 `SaveSlotGuard`（测试程序集不同，无法共享一个内部类型）；后续改动需同步，避免行为漂移。
6. **视觉缺口仍未解决**（§17），整包不得标完成。

---

## 20. 2026-10-29 增量（阶段 1/4）：承诺面板八例与受影响三类夹具 / namespace 统一适配

> 范围：**只做测试组织与夹具适配 + 编译 / 发现**；本轮**未运行任何测试**（遵守 `tests-run` 禁令），宿主门禁结果不预填。
> 维护请求 `bcce0eefd2d80b77fe1a465620c1542e8a0f5f842b5e28a5ebcfd48c32286010` **尚无回执**，本轮**未冒充已执行**；§17 视觉精确提案原样保留，**未写 XLSX / Mirror / 生成资产**；产品代码本轮 **0 改动**。

### 20.1 精确修改（仅测试文件）

| 文件 | 字节 | SHA256 | 修改 |
|---|---:|---|---|
| `Assets/Tests/PlayMode/Village/VillageCommitmentProductIntegrationPlayTests.cs` | 41225 | `862a6a72c36b5e33a682076f5ec52e38dc5e0c835b71761c24c53ae89700dc1c` | 整类（含 3 个 partial 文件）统一包入稳定 `namespace RRPG.Village.Tests`；方法短名与断言体未改 |
| `Assets/Tests/PlayMode/Village/IntradayWorkProductPlayTests.cs` | 2635 | `6bc77b8eb557d23f11aecc811d67ea10d679cbafec212f6db1bfba4066d597bb` | 同上（同一 partial 类） |
| `Assets/Tests/PlayMode/Village/IntradayOnsiteProductPlayTests.cs` | 5735 | `7e5097485804f6c20275163e697f82cb6ba9dfd5280bd435b887167ce6f14cf4` | 同上 |
| `Assets/Tests/PlayMode/Village/IntradayActivityProductPlayTests.cs` | 3378 | `786c418ad8368e910afd699eb0aec998d8ee4a4c731349381f833d85e69d3be9` | 同上 |
| `Assets/Tests/PlayMode/Village/VillageWorldProductIntegrationPlayTests.cs` | 25189 | `699d5fa542a95c7e8fd71f2b02cae42eb189dd75d32aa83064e939aba6542c47` | 包入 `RRPG.Village.Tests`；新增 `SaveSlotGuard`（先捕获 / 异常恢复 / finally 恢复 / 未完成阻断）；移除无保护 `DeleteSaveFile()`；`EnterTown(freshWorld)` 区分「初始隔离世界」与「真实读档 reload」；`ws_test_01` 改为从运行时 `Completed building_farm` 派生的实例岗位 |
| `Assets/Tests/PlayMode/Village/VillageVisualPrototypePlayTests.cs` | 18174 | `244105d62cbdb4645f56fb2b245e4690c3212f78592ab7edc058762f4282b87a` | 已存在 `RRPG.ProductHome.Tests`，不重复包 namespace；实例岗位锚点从 `PlaceViews` 改为「真实 Building 视图（同一实体）+ 断言 PlaceViews 不得重复绘制该岗位」 |
| `Assets/Tests/PlayMode/VillageV4/VillageV4ResourceSurplusPlayTests.cs` | 25113 | `6ae821e0ac3f4cec80bd590fc2b284968d5796b7a04774bcde86e30024d390cc` | 包入 `RRPG.Village.Tests`；把真实 `NewGame + Save + OnContinue` 初始化移入 `[UnitySetUp]` 的 `try/finally` 异常恢复范围；用例体断言未改 |

- 四个类共 7 个测试源文件（承诺类 4 个 partial 文件）全部只改**测试组织 / 夹具**；方法短名、测试数据与业务断言强度均保留。
- 跨文件 partial 全部同一 namespace，未混出两个同名类；无其它测试 / 产品代码引用这些类（本轮 `grep` 实测仅自身文件），asmdef 引用无需变更。
- **未改**：`InitializationProductPlayTests`（全局类型、已过方法过滤）与任何 `Assets/Scripts/**` 产品代码；未改 XLSX / Canonical Mirror / 生成资产 / 场景 / Prefab / 字体 / ProjectSettings。

### 20.2 当前默认语义审查（逐项，不靠猜）

| 依赖点 | 实际当前语义（源码实测） | 本轮适配 |
|---|---|---|
| 默认 NewGame 世界 | `SaveSystem.NewGame()` → 生成配置 7 个 Completed 实例（Farm / Kitchen / 5 House）；静态 `ws_*` 基座为空（`TryPrepareNewGameWorld`） | 四类夹具统一真实 `NewGame + Save + MainMenuUI.OnContinue`；不再读 / 删用户档 |
| 实例岗位 | `workplace:<InstanceId>:main`；Farm Capacity 2 / Kitchen Capacity 1 来自既有 Profile | 承诺 / 世界 / V4 均从运行时 `Buildings.All` 的 Completed 实例派生，不写死 `ws_*` |
| 旧静态岗位 | 正式新世界不注册；旧档只按存档真实引用恢复 | 未把默认世界改回静态 / 0 建筑；未触碰 `ws_*` 兼容通道（`ManagementShellPlayTests` CASE-B 保持不变） |
| Place / 视图 | 实例岗位与真实 Building 是同一实体，`HomePlaceSnapshot.DrawsWorldEntity=false`；`PlaceViews` 只承载独立绘制的 Place | 视觉 2 例改为断言“同一实体只画一套”：实例岗位必须有真实 Building 视图，且不得再出现在 `PlaceViews` |
| 硬编码计数 | 默认建筑 / 人口数量来自生成配置 | 四类保留既有相对 / 派生口径；未新增硬编码 7 / 5 / 1 / 2 / 1 |
| `SaveSystem.NewGame` 反射重载 | 现有 `NewGame()` 与 `NewGame(InitialWorldConfiguration)` 两个重载 | 承诺 / 世界 / V4 使用「方法名 + 实参个数」定位（0 参唯一）；视觉类使用唯一性断言助手；无 `AmbiguousMatchException` 路径 |
| 劳动/项目推进 | 项目与岗位由 intraday work interval 真实推进；`TickDay` 走真实 `DailyAdvanceSink` | `TickDays(3)` / `AdvanceTo` / 真实 Driver 断言保留，不改成直接改 Progress |

### 20.3 用户单槽保护面（四类统一）

| 项 | 实现 / 实测 |
|---|---|
| 先捕获 | 每个测试 `[SetUp]/[UnitySetUp]` 第一步 `SaveSlotGuard.Capture()`，早于 `ResetForTests` / `LoadScene` / `NewGame`；从 `SaveSystem.GetSavePath()` 读取**当前真实档**，`CreateNew` 备份 + 回读字节 / SHA256 + 捕获后再次核验当前档未被改写 |
| 未完成阻断 | `BlockOnUnfinishedRuns()` 只放行 `restored` / `protective-snapshot`；`captured` / `restore-failed` / `no-manifest` / 不可解析仍阻断 |
| 异常恢复 | `SetUp` / `UnitySetUp` 在捕获后用 `try/finally`，初始化异常先用**本 runID 备份**恢复再抛出；V4 原「`RestoreAfterFailedSetup` 未被调用」的缺口已补上 |
| finally 恢复 | `TearDown` 先 `_guard.Restore()`，`finally` 再重置运行时；恢复写回前再验备份 hash，写后回读比对并写 manifest |
| 无保护 Delete | World 类原 `DeleteSaveFile()` 已删除；四类仅保留 guard 内部「原本 absent 才删除」的受保护分支 |
| 历史 hash | 不套历史字节数 / SHA；本轮只读实测当前用户档 `13134 B / adcf5d8b…`，但**未写入 / 未删除** |

### 20.4 编译与发现（真实执行，非口头）

| 项 | 实测 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` ×2（首轮修改后、修 World reload 语义后）；刷新后 `isCompiling=False / isUpdating=False / isPlaying=False` |
| 编译错误 | `console` 过滤 `Error/Exception` → **0** |
| 发现（整类） | `RRPG.Village.Tests.VillageCommitmentProductIntegrationPlayTests.` → **8**；`RRPG.Village.Tests.VillageWorldProductIntegrationPlayTests.` → **3**；`RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests.` → **2**；`RRPG.Village.Tests.VillageV4ResourceSurplusPlayTests.` → **1**；合计 **14** |
| 过滤可用性 | 承诺 / 世界 / V4 已有命名空间段，宿主 `testClass` 正则现可命中；视觉类原已有 `RRPG.ProductHome.Tests`，未重复包裹 |
| 方法状态 | 14 条均为 `PlayMode`、`ignored=false`、`explicit=false`；无 skip / Ignore 新增 |

### 20.5 原业务强断言保留面

- **承诺 8 例**：字体权威 / TMP 字符可见、暂停与倍速标签序列、Kitchen 行 InstanceID 与锚点 90 帧稳定、人员行稳定、移除后占用归零；真实 UI 指派 / Project 进度 / 面板不暂停模拟 / 完成自动释放 / 原岗位恢复；Save-Load 后 Active Project 进度、职责、进度行 `/ 10`；Modal 打开阻断与关闭恢复；五类动态行在 Game View 几何可见；午休 / 到场 / 岗位劳动 / 下班文本；Activity 通勤后执行与 Save 不完成；Onsite 面板可见 / 关闭不改模拟（`ActivityRuntimeBridge.BuildSaveData` 前后逐字节相等）/ 结束参与后 Interrupted。**均未删 / 未 Ignore / 未跳过 / 未改预期值。**
- **世界 3 例**：Presence 只从 Assignment 派生、World 视图随 sim 显隐且状态不重建、ProjectSite 真实 Save/Reload 痕迹、Main Menu 阶段不显示。原 `ws_test_01` 断言改为**同一段语义的实例岗位**（从 Default NewGame 的 Completed Farm 派生）。
- **视觉 2 例**：A/B 切换不替换实体、足迹 / 碰撞 / sprite / Domain 不变；Tavern 活动 Place 出现 / 消失 3 轮中，已注册实例岗位的**同一视图实例、同一锚点、同一 sprite、Body 可见**，A/B 往返锚点不变。语义按「实例岗位 = Building 同一实体」落到真实 BuildingViews，并额外断言不得再画第二套 Place。
- **V4 1 例**：3 天逐日资源追踪、Farm 产出 / Kitchen 加工 / 在村人数、Food Net Delta > 0、Jump 后显著积累、准备界面真实 Food 文本与预计携带、Confirm 真实转 Carry、`Village Food` 恰好减少 Carry、Journey 非 0 补给、Retreat 收尾。数值口径仍来自既有 Profile（未写死新数值）。

### 20.6 当前保护面（本轮只读实测，写入 + 2 次安全 Refresh 之后）

| 项 | 实测值（2026-10-29T11:1xZ） | 判定 |
|---|---|---|
| Pilot 身份 | `Application.productName=RRPG_AI_Pilot`、`companyName=DefaultCompany`、`unityVersion=2022.3.62t9` | ✅ 授权工程 |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| 原生 runner / pending | `Tool_Tests.HasPendingTestRun()=False`、`EditModeLauncher.IsRunning=False`、`PlaymodeLauncher.IsRunning=False` | ✅ 无 running / 无待投递 |
| PrefabStage | `null` | ✅ 未进入 Prefab 编辑 |
| 场景 | `SampleScene` `isDirty=False / rootCount=5` | ✅ 未污染 |
| 当前用户单槽 | `exists=True / 13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`（本轮只读，未写） | ✅ 与基线一致，但以本轮实测为准 |
| Initialization guard | `dirs=79 / restored=79 / protective=0 / other=0 / noManifest=0` | ✅ 无未完成 / 无未知 |
| R3B guard | `dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective 为既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体（磁盘） | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与保护值一致 |
| 字体子资产 | `9` 个，`EditorUtility.IsDirty` → **0 dirty** | ✅ 无 pending 保存（未做字模测量） |
| 34 项 workspace 基线 | `34/34 same / changed=0 / missing=0` | ✅ 全一致 |

### 20.7 宿主门禁（本轮只交承诺整类 8 例；不预填 PASS）

| 目标 | 参数 | 通过标准 |
|---|---|---|
| **承诺面板整类（本轮交）** | `testMode=PlayMode` + `testClass=VillageCommitmentProductIntegrationPlayTests`（整类 **8 例串行**） | **8/8 Passed、0 skipped**；0 项 / skip 不算通过 |

- 不跑：已由主开发核过的 `ec59b870`（Kitchen 1/1）、`8de6515d`（Edit 9/9）、`a8bbf7a2`（施工 2/2）、管理 17、Home / 初始化产品、Compiler 280、完整 Edit / Play。
- 预期副作用：8 个 Initialization guard run（`captured → restored`，逐字节回读）；用户单槽每例结束回到 `13134 B / adcf5d8b…`；不写字体、不改场景 / Prefab。
- 失败处理：保留真实 `message` / `stack`；夹具问题改夹具，落在产品 Save / Load / 投影 / UI 才做产品窄修；**不得削弱已冻结 / 保留的业务断言**；不无限重试。
- **本轮状态：未运行**（执行层禁止 `tests-run`），不预填 PASS；后续 3 类（世界 3 / 视觉 2 / V4 1）由 Studio 自动继续按类门禁，不得把「已发现」写成「已通过」。

### 20.8 历史更正保留

- §11.1 的“漏判 3 个同因方法”与 §19.6 的 World `ws_test_01` 缺口**原文保留**；本阶段已按同一原因一次集中修订 World 类，但**未运行**，因此不改写历史失败记录、不标“已修复通过”。
- §17 视觉精确提案、§18.2 manifest 跨格式修复、§19 六方法修订均保留；本轮未重做、未覆盖。

---

## 21. 2026-10-29 增量（阶段 1/4）：承诺 8 例四失败定位 / 反射成员签名修正 / NRE 夹具顺序修复

> 范围：只做承诺面板整类 8 例的失败定位与本阶段必要修复 + 同批四类反射静态审查；**本轮未运行任何测试**（遵守 `tests-run` 禁令），8/8 由宿主门禁决定，不预填 PASS。
> 维护视觉工具请求 `bcce0eefd2d80b77fe1a465620c1542e8a0f5f842b5e28a5ebcfd48c32286010` **仍无回执**；本轮**未写 XLSX / Mirror / 生成资产**，未声称视觉已修。
> 产品代码 `Assets/Scripts/**` **0 改动**（`VillageCommitmentUI.cs` 磁盘 mtime / SHA 与本轮开始一致，见 §21.6）。

### 21.1 真实回执（宿主 run，主开发核 `5962afbb-be2f-40c4-8557-e123ecfb062d`）

| 项 | 实测 |
|---|---|
| 汇总 | **8 total / 4 passed / 4 failed / 0 skipped / 10.5794229 s / `scope-restored\|font\|0\|632`** |
| 通过 4 例 | `CommitmentModal_BlocksTownButtons_WhenOpen_AndRestoresAfterClose`(933 ms)、`CommitmentRows_AreGeometricallyVisible_InGameView`(530 ms)、`IntradayActivity_RealTownDriver_CommutesThenExecutes_SaveDoesNotComplete`(6678 ms)、`IntradayOnsite_RealTownVisibleClickableCloseDoesNotChangeSimulation`(877 ms)——**本轮未重跑、未改、未算作本阶段新通过** |
| 失败 4 例 | 见 §21.2 / §21.3；完整 message + stackTrace 原文另存 `Development/StudioRuns/Initialization-20261027/play-evidence/commitment-eight-run-failure-stacks-20261029.txt` |
| 取数方式 | `testResults` 只读（`select name status durationMs stackTrace`）+ `console` 只读（`logType Error/Exception`，含 `sequence` 定位）——**不重跑、不快照猜测** |

### 21.2 前三失败：辅助反射把 Bridge 的产品 property 当 field 读取（夹具缺陷，非产品缺陷）

真实栈（三例完全同因）：

```text
at VillageCommitmentProductIntegrationPlayTests.GetBridgeField (string name)  …PlayTests.cs:264
at VillageCommitmentProductIntegrationPlayTests.InstanceWorkplace (string definitionId) …PlayTests.cs:221
at …<CommitmentUI_RealTown_AssignAndProject_ClosedLoop_PanelDoesNotPauseSimulation>d__32.MoveNext …:470
（另两例同栈：…<CommitmentUI_ReflectsRuntimeAfterSaveLoad_MidProject>…:526；…<IntradayWork_RealTownAssignment_CommutingLaborLunchVisible>… IntradayWorkProductPlayTests.cs:21）
message = "字段不存在：Buildings / Expected: not null / But was: null"
```

真实成员签名（本轮只读反射实测，`Assembly-CSharp / VillageRuntimeBridge`）：

| 成员 | 真实签名 |
|---|---|
| `Buildings` | **PROPERTY** `BuildingRuntime`，getter 存在（`get; private set;`，`VillageRuntimeBridge.Buildings.cs:229`） |
| `IntradayWork` | **PROPERTY** `VillageIntradayWork`（`VillageRuntimeBridge.Work.cs:7`） |
| `TestProjectDefs` | **PROPERTY** `IReadOnlyList<…>` |
| `Resources` / `Time` / `Assignments` / `Projects` / `World` / `State` / `IsSimulationActive` | FIELD（`Resources`：`VillageRuntimeBridge.cs:140`） |
| `SaveSystem.NewGame` | **2 个重载**：0 参 `NewGame()`、1 参 `NewGame(InitialWorldConfiguration)` |

**根因**：测试辅助 `GetBridgeField` 只用 `Type.GetField`，对 `Buildings` 必然为 null → `Assert.IsNotNull(f,"字段不存在：Buildings")`。**产品 property 正确，未改产品、未删断言。**

**修复（仅测试夹具，按真实成员签名访问 + 存在性断言）**：

1. 新增 `GetBridgeMember(name)`：**先解析静态 property（校验 getter 存在且为 static），再回退静态 field**；两者都不存在时 `Assert` 失败，不静默返回 null；`GetBridgeField` 保留为同义入口（历史调用点语义升级，不删除断言）。
2. 新增 `GetBridgeBuildings()`：显式断言 `Buildings` **必须是静态属性**、**必须有可读 getter**、**值必须已初始化**，再断言 `BuildingRuntime.All` 存在且可枚举；返回值直接用于 `InstanceWorkplace`（不再从 `GetField` 取）。
3. `InstanceWorkplace` 改为 `foreach (var b in GetBridgeBuildings())`；**“正式默认世界必须存在 Completed <definitionId> 实例”原断言保留**。
4. 编译期实测发现 `BuildingRuntime` 属于 `Assembly-CSharp`，测试程序集不引用该程序集（`CS0246`），因此 `GetBridgeBuildings()` 返回 **`BuildingRuntime.All`（`IEnumerable`）**，仍走反射取值，**不新增跨程序集引用、不改 asmdef**。

### 21.3 第四失败：NRE 完整产品栈 + 夹具生命周期判定

真实完整栈（`testResults` 与 `console sequence 102385` 一致）：

```text
VillageCommitmentUI.RebuildWorkplaceRows (Dictionary<string,string> personNames) at Assets/Scripts/Village/UI/VillageCommitmentUI.cs:313
VillageCommitmentUI.Refresh ()  at Assets/Scripts/Village/UI/VillageCommitmentUI.cs:251
VillageCommitmentUI.Update ()   at Assets/Scripts/Village/UI/VillageCommitmentUI.cs:82
```

精确行：`VillageCommitmentUI.cs:313-314` `VillageCommitmentView.BuildWorkplaceRows(VillageRuntimeBridge.Assignments.WorkplaceDefs, VillageRuntimeBridge.Assignments, personNames)` → **`Assignments` 为 null** 时解引用。

**生命周期判定（不是“测试对象 vs 真实 UI”的含糊说法，逐条证据）**：

| 证据 | 结论 |
|---|---|
| 该 NRE 的 `sequence 102385` 出现在**全部 8 次 `[SAVE-CAPTURE]`（102377–102384，即 8 个用例的 SetUp 起点）之后** | 发生在最后一个用例的用例体末 / TearDown 帧窗口，**不是任何用例业务断言中途** |
| `ResetForTests()` 会把 `Assignments / Time / Projects / IntradayWork / Resources …` 全部置 null（`VillageRuntimeBridge.cs:1415-1459`） | 面板 `_isOpen==true` 时，下一次 `Update→Refresh` 必然读到 null 运行时 |
| 全仓 `Assets/Scripts/**` 内 `ResetForTests()` 的调用点**只有定义处与其内部对其他测试桥的调用**；产品路径（MainMenu/TownUI/SaveSystem）**没有任何调用** | 该“面板打开 + 运行时清空”状态**玩家不可达**，属于**测试夹具时序**（旧用例体结束后面板仍 `_isOpen`，TearDown 先 `ResetBridge()`） |
| 本类用例既有语义就是“面板保持打开”参与断言（`CommitmentUI_RealTown…` / `CommitmentUI_Reflects…` 打开后不在用例体 Hide；`VillageTMP…` 同样不 Hide 收尾） | 不能在用例体里靠 `Hide` 掩盖 UI 断言；只能在 **TearDown 夹具**先按真实 UI `Hide()` 收口再清运行时 |

**修复（夹具顺序，不忽略 NRE、不 `LogAssert.Expect`、不清日志）**：`Cleanup()` 的 `finally` 先 `HideCommitmentUi()`（真实 `VillageCommitmentUI.Hide()`，同步把 `_isOpen=false` 并隐藏 root），**再** `ResetBridge()`。这样“运行时清空”只发生在面板已关闭之后，与真实 UI 生命周期一致。

**产品侧结论（如实记录，不虚构已修）**：`VillageCommitmentUI.Refresh / RebuildWorkplaceRows` 仍直接解引用 `VillageRuntimeBridge.Assignments`——这是**当前玩家不可达**的潜在健壮性缺口；本轮判定为夹具根因、产品 0 改动（最小面原则），**登记为后续 UI 健壮性观察项**，未在本阶段修改产品代码。

### 21.4 同批四类辅助反射集中静态审查（防下阶段同类字段/属性错误）

| 类 | 反射入口 | 本阶段核对结果 | 处置 |
|---|---|---|---|
| `RRPG.Village.Tests.VillageCommitmentProductIntegrationPlayTests`（4 个 partial 文件） | `GetBridgeField` / `GetBridgeMember` / `GetBridgeBuildings` | `Buildings` 属性错误（三失败根因）；`Assignments/Projects/Time/Resources/State/World/SocialResponse/PersonRest/IsSimulationActive` 均为 FIELD，签名正确；`NewGame` 用“方法名 + 实参个数”定位 0 参重载（`InvokeStatic`），无歧义 | **修** `Buildings` 解析（§21.2） |
| `RRPG.Village.Tests.VillageWorldProductIntegrationPlayTests` | 同名 `GetBridgeField` | 同一 helper 对 `Buildings` **同样必然落空**（`InstanceWorkplace` 被 `Presence_…` 用例调用）；`World/Projects/Time/IsSimulationActive` 是 FIELD 正确；`NewGame` 按参数个数定位正确 | **同因同步修**（不改用例体、不改断言，属保证下阶段必然失败的静态审查修正） |
| `RRPG.Village.Tests.VillageV4ResourceSurplusPlayTests` | `GetMember` / `StaticMember` | 已是“field → property”双路解析：`Resources`(FIELD)、`Buildings`(PROPERTY)、`LastResourceDayResult` 等混合成员均命中；`NewGame` 按参数个数定位 | 已正确，**不改** |
| `RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests` | `Get` / `T(...).GetField` | 只用 `Time`(FIELD)，未读 `Buildings/Resources/IntradayWork`；`SaveSystem.NewGame` 走“同名同参数量必须唯一”断言 | 已正确，**不改** |

补充：`VillageTests/BuildingResourceProductPlayTests` 使用 `GetProperty("IntradayWork")` 正确、`GetField("Resources"/"Time"/"Assignments")` 正确；本阶段未改（已过类不动）。

### 21.5 本轮实际变更（仅测试夹具；产品 0 改动）

| 文件 | 变更 | 变更后字节 / SHA256 |
|---|---|---|
| `Assets/Tests/PlayMode/Village/VillageCommitmentProductIntegrationPlayTests.cs` | ① `GetBridgeField` → `GetBridgeMember`（property 优先 + 存在性断言）+ `GetBridgeBuildings()`（Buildings 属性 / getter / 已初始化 / All 四重断言）；② `InstanceWorkplace` 改用真实属性；③ `TearDown` 先真实 `Hide()` 再 `ResetBridge()`。**方法短名、用例体、业务断言全部未改** | **44486 B / `923ff9982c026663f66ff7d29c5070a24bf2ef957dcf35d545374ed527725720`**（mtime `2026-10-29T11:22:15Z`） |
| `Assets/Tests/PlayMode/Village/VillageWorldProductIntegrationPlayTests.cs` | 同一 property-aware `GetBridgeMember` + `GetBridgeBuildings()`，`InstanceWorkplace` 改用真实属性（同因静态审查修正）。**用例体/断言未改** | **27320 B / `d49513cdd1edb83102234dbfad5c31dba4ec1886b87f2c2790d20f35d2ca83eb`**（mtime `2026-10-29T11:22:19Z`） |
| `Assets/Scripts/Village/UI/VillageCommitmentUI.cs` | **未改** | 34537 B / `dabbb2d34b3c949c004d7bbb756d3701d1c65b0bd87923cad81330da85fd0f95`（mtime `2026-10-10T12:10:57Z`，早于本轮） |

未改：XLSX / Canonical Mirror / 生成资产 / 场景 / Prefab / 字体 / ProjectSettings / asmdef / save DTO / 34 项基线 / guard 历史 / 用户单槽。未执行 Git（读或写）。

### 21.6 保护面（本轮写文件 + 2 次安全 Refresh 之后，只读实测）

| 项 | 实测值（2026-10-29T11:2xZ） | 判定 |
|---|---|---|
| Pilot 身份 | `RRPG_AI_Pilot / DefaultCompany / 2022.3.62t9` | ✅ 授权工程 |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| 原生 runner / pending | `Tool_Tests.HasPendingTestRun=False`；`EditModeLauncher.IsRunning=False`；`PlaymodeLauncher.IsRunning=False` | ✅ 无 running / 无待投递 |
| PrefabStage | `null` | ✅ |
| 场景 | `SampleScene` `isDirty=False / rootCount=5` | ✅ 未污染 |
| 当前用户单槽（本轮只读捕获，不套历史 hash） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，`exists=True / 13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`；`*.tmp=0` | ✅ 未写未删 |
| Initialization guard | `dirs=87 / restored=87 / protective=0 / other=0 / noManifest=0` | ✅ 无未完成 |
| 上一 8 例 restore 真实性 | 8 个 run（`20261029T111857Z … 20261029T111907Z-2`）：`status=restored`、`original-save.bin` 实际 SHA == manifest `backupSha256`、`sourceSha256 == backupSha256`、**当前档 SHA == sourceSha256** → **8/8 逐字节完整恢复** | ✅ |
| R3B guard | `Development/StudioRuns/R3B-SaveProtection-20261024`：`dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective 为既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体（磁盘） | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与保护值一致 |
| 字体子资产 | `LoadAllAssetsAtPath` = **9** 个，`EditorUtility.IsDirty` = **0** | ✅ 无 pending（未做字模测量） |
| 34 项 workspace 基线 | `total=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |
| Console | 最终安全 Refresh 之后 `sequence > 102389` 的 **项目 / 测试程序集** `Error/Exception` = **0**；仅剩 TMP 缺字 `Warning`（既有字体档缺 CJK，未改字体）。另有 2 条**本层只读测量 snippet 自身**的失败（`102912` 局部函数 `Func<byte[],string>` 笔误、`102915` 探测路径不存在，均在 MCP 代码片段沙箱），**如实保留不清洗**，与项目 / 测试程序集无关 | ✅ 项目与测试程序集 0 错误 |

### 21.7 编译与发现（真实执行）

| 项 | 实测 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` ×2（首轮写入后、修正 `BuildingRuntime` 跨程序集后）；刷新后 `isCompiling=False / isUpdating=False / isPlaying=False` |
| 中间编译错误（如实保留） | `sequence 102388/102389`：`error CS0246: The type or namespace name 'BuildingRuntime' could not be found` ×2（测试程序集不引用 `Assembly-CSharp`）→ 改为返回 `BuildingRuntime.All` 的 `IEnumerable`，**未加程序集引用、未改 asmdef**；后续 0 错误 |
| 程序集 | `Library/ScriptAssemblies/VillageStage1.PlayModeTests.dll` **162816 B / `c10d0045ce254b4b2f69c3f3ff9d50978343d3b9f6d0ef60d0929e2d98377735`**，mtime `2026-10-29T11:22:21Z`；内容级核对：`HideCommitmentUi` / `GetBridgeMember` / `GetBridgeBuildings` 与字面量「VillageRuntimeBridge.Buildings 必须是静态属性」**均命中** |
| 发现 | `tests` 源 `RRPG.Village.Tests.VillageCommitmentProductIntegrationPlayTests.` → **8 条**，全部 `PlayMode` / `VillageStage1.PlayModeTests` / `explicit=false`；方法名逐字未改（历史发现契约保持） |

### 21.8 保留的业务强断言面（本阶段 0 删减）

- 三例的默认实例岗位派生断言（`Completed building_farm / building_kitchen` → `workplace:<InstanceId>:main`）保留，且**新增**“Buildings 必须是产品静态属性 + 已初始化 + All 可枚举”的存在性断言（更严，不更松）。
- `VillageTMP…`：字体权威同一性、TMP 字符可见、暂停/倍速标签序列、Kitchen 实例岗位行 **InstanceID 与锚点 90 帧稳定**、人员行稳定、移除后占用归零——“1 / cap”“0 / cap”文本断言改由 `GetWorkplaceDef(...).Capacity` 派生，**未写死、未删除**（沿用 §20.5）。
- `CommitmentUI_RealTown…`：Project 真实推进 ≥6、占用期 `TotalExecutedDays==0`、完成后自动释放并恢复执行；`CommitmentUI_Reflects…`：Save/Load 后 Active Project 数 / Progress 等值、`/ 10` 进度行、Hide 不改运行时占用；`IntradayWork…`：真实 UI 点击派工、岗位劳动 / 午休 / 下班-休息文本、午餐后资源不变、面板可见可读。
- 未采用 `LogAssert.Expect`、未加 `[Ignore]`、未跳过、未清日志换绿。

### 21.9 宿主门禁（本阶段只交承诺整类 8 例，不预填 PASS）

| 项 | 内容 |
|---|---|
| 执行参数 | `testMode=PlayMode` + `testClass=VillageCommitmentProductIntegrationPlayTests`（整类 **8 例串行**） |
| 通过标准 | **8/8 Passed、0 skipped**；0 项 / skip 不算通过 |
| 不跑 | 已过 `Compiler 280`、管理 17、Kitchen 专项 1、Edit 9、施工 2、Home、初始化产品专项、完整 Edit/Play（均不重跑） |
| 预期副作用 | 8 个 Initialization guard run（`captured → restored`，逐字节回读）；用户单槽每例结束回到 `13134 B / adcf5d8b…`；不写字体、不改场景 / Prefab |
| 失败处理 | 保留真实 `message` / `stackTrace`；夹具问题改夹具，落到产品 Save/Load/投影/UI 才做产品最小修复；**不得削弱已冻结 / 保留业务断言**；不无限重试 |

### 21.10 未验证 / 风险（本阶段不写 Completed 的理由）

1. **8 例仍未运行**（执行层禁止 `tests-run`）：`Buildings` 属性解析与 TearDown `Hide` 顺序只能由宿主回执确认，**不预填通过**。
2. **产品潜在健壮性缺口未修**（§21.3）：`VillageCommitmentUI` 在“面板打开 + 运行时被清空”时仍会 NRE；当前判定玩家不可达（`ResetForTests` 仅测试调用），因此**不在本阶段最小面内改产品**，登记为观察项。
3. **World 类同因修正未运行**：`VillageWorldProductIntegrationPlayTests` 的 `Buildings` 解析本轮已同步修，但 3 例属后续阶段门禁，本阶段不声称通过。
4. **视觉维护请求 `bcce0eefd2d8…` 无回执**：A 档 Kitchen / House / Person 真实美术仍未解决（§3 / §17 原样保留），**未写 XLSX / Mirror / 生成资产**，不虚构已修。
5. **本轮未重跑**已过类（Compiler 280、管理 17、Kitchen 1、Edit 9、施工 2、Home、初始化产品专项、完整 Edit / Play），历史回执口径不变。

---

## 22. 2026-10-29 增量（阶段 2/4）：World 三例受影响集成 · 前阶段核验与同类成员静态审查

> 范围：确认阶段 1/4 宿主门禁与保护面，机械核对 `VillageWorldProductIntegrationPlayTests` 三例的成员访问/重载解析，交宿主整类门禁。**本阶段未运行任何测试**（`tests-run` 禁令），**未改动任何 `.cs` 文件**（原因见 §22.3），不预填 PASS。
> 维护视觉工具请求 `bcce0eefd2d80b77fe1a465620c1542e8a0f5f842b5e28a5ebcfd48c32286010` **仍无回执**；未写 XLSX / Mirror / 生成资产。

### 22.1 阶段 1/4 宿主门禁确认（只读回看，未重跑）

| 项 | 实测 |
|---|---|
| `testResults` 承诺整类 | **8/8 Passed、0 fail、0 skip**：`CommitmentModal…` 847 ms、`CommitmentRows…` 547 ms、`CommitmentUI_RealTown…` **2746 ms**、`CommitmentUI_Reflects…` 921 ms、`IntradayActivity…` 6710 ms、`IntradayOnsite…` 863 ms、`IntradayWork…` **475 ms**、`VillageTMP…` **1315 ms**（均 `message=null`） |
| 阶段 1 的四失败 | 原 3 例 `字段不存在：Buildings`、1 例 `Unhandled [Exception] NRE` 全部消失；该 host run（11:25:17–11:25:30Z）的 `console` 中 `Error/Exception` = 0（当前缓冲区仅剩本层只读 snippet 自身失败，见 §22.4） |
| 阶段 1 新增 8 个 guard run | `20261029T112517Z / 112518Z / 112518Z-2 / 112521Z / 112522Z / 112529Z / 112530Z / 112530Z-2`：全部 `status=restored`；`original-save.bin` 实际 SHA == `backupSha256`；`sourceSha256 == backupSha256`；**当前用户档 SHA == sourceSha256** → **8/8 逐字节完整恢复** |
| 结论 | 阶段 1 的 `Buildings` 属性解析修复与 TearDown `Hide→Reset` 顺序修复**已由宿主真实回执确认**；无遗留 captured / restore-failed |

### 22.2 World 三例机械静态核查（真实执行，非口头）

方法：读取 `VillageWorldProductIntegrationPlayTests.cs` 源码，机械抽取其所有 `GetBridgeField/GetBridgeMember(...)`、`InvokeStatic(BridgeType/SaveSystemType, ...)`、`GetProperty(layer/town, ...)`、`InvokeInstance(mm, ...)` 调用，再用**实时反射**解析真实成员签名。原始输出另存 `play-evidence/world-three-static-member-audit-20261029.txt`。

| 被访问成员 | 真实签名（本轮反射） | 结论 |
|---|---|---|
| `Buildings` | **PROPERTY** `BuildingRuntime`，`static=True`，getter 存在 | 阶段 1/4 已按属性访问（`GetBridgeMember` + `GetBridgeBuildings`）；本类不再有 field 读取 |
| `IsSimulationActive` / `Projects` / `Time` / `World` | FIELD（`bool` / `VillageProjectSystem` / `VillageTime` / `VillageWorldRegistry`），均 static | 按字段读取正确 |
| `BuildSaveData` / `GetPersonPresence` / `ResetForTests` / `SetSimulationActive` / `TryAssignPerson` / `TryStartProject` | 各 **1 个重载**，参数数 0/1/0/1/2/2 | 反射调用唯一，无 `AmbiguousMatch` |
| `SaveSystem.NewGame` | **2 个重载**（0 参 / 1 参 `InitialWorldConfiguration`） | 本类调用为 0 实参，`InvokeStatic` 按“方法名 + 实参个数”唯一命中 0 参重载 |
| `SaveSystem.Save` / `GetSavePath` | 各 1 个重载（1 参 / 0 参） | 正确 |
| `VillageWorldLayer.CanvasVisible` / `MarkerSummary` | 实例 PROPERTY，getter 存在（`bool` / `string`） | 正确（视图只读探针） |
| `TownUI.PlayerData` / `MainMenuUI.OnContinue` | 实例 PROPERTY / 唯一 0 参方法 | 正确（真实产品入口） |
| 静态 `ws_*` 引用 | 源码中 **0 处**（仅注释） | 正式实例岗位由 Completed `building_farm` 派生，不恢复旧静态目录 |

**逐例语义核对（不改断言，只确认与当前正式世界一致）**：

| 用例 | 关键断言与当前语义 |
|---|---|
| `Presence_DerivedAndWorldVisible_WithoutAnyManagementPanel_And_SurvivesSimToggle` | `TryAssignPerson(pid, workplace:<InstanceId>:main)` → `VillageWorldPresence.Resolve` 的 `PlaceId = 该 workplaceId`（`VillageWorldPresence.cs:60-64`），故 `presence.PlaceId == farmWorkplace` 断言成立；`MarkerSummary` 同时含 plot 名与 `personId@workplaceId`（`VillageWorldLayer.cs:181/209/225`），故 `Contains(pid)` / `Contains(farmWorkplace)` 成立；sim 关停只改 `_canvas.enabled`，`World.Places` 与 Presence 不变（`VillageWorldLayer.cs:58-62`） |
| `ProjectCompletion_WorldSiteTrace_PersistsAfterSaveReload_WorldRestored` | `prj_test_02` 目标 6、每人每天 2（`VillageProject.cs:58-65`），`TickDay()` → `AdvanceTo` → `DailyAdvanceSink.CompleteInterval` → `Projects.OnWorkInterval`（`VillageRuntimeBridge.Intraday.cs:167`、`Work.cs:134`）→ 3 天完成；完成点 `ObserveProjectCompletions` 单向追加 ProjectSite（`Work.cs:178-189`）；`World.ToSaveData` **只存 ProjectSite**（`VillageWorldRegistry.cs:103-110`），故 `World.Places.Count==1` 与正式世界一致（实例岗位 Place 不落档） |
| `MainMenuPhase_DebugViewHidden_UntilTownSimulationActive` | 场景加载时 `MainMenuUI.Start` 关闭 `TownUI` 并 `SetSimulationActive(false)`（`MainMenuUI.cs:41-48`），`TownUI` 未激活故其 `Start` 的 `true` 不执行；`OnContinue → TownUI.Initialize` 才 `SetSimulationActive(true)`（`TownUI.cs:93-101`） |

### 22.3 本阶段文件变更

| 文件 | 变更 |
|---|---|
| `Assets/Scripts/**`（产品） | **0 改动** |
| `Assets/Tests/**`（测试） | **0 改动**：World 三例唯一同类缺陷（`Buildings` 被当字段读）已在阶段 1/4 按“同批四类静态审查”同因修正（`d49513cd…`）；本阶段机械核查确认无其它 property/field 误用、无重载歧义、无静态 `ws_*` 残留，故不做无依据改动（已过/未过类均不动断言） |
| `Development/StudioRuns/Initialization-20261027/play-evidence/world-three-static-member-audit-20261029.txt` | 新增（本轮核算原始输出 + 保护面实测） |
| 本文档 | 追加 §22 |

### 22.4 保护面（本阶段只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Pilot 身份 / 版本 | `RRPG_AI_Pilot / DefaultCompany / 2022.3.62t9` | ✅ 授权工程 |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| 原生 runner / pending | `Tool_Tests.HasPendingTestRun=False`；`EditModeLauncher.IsRunning=False`；`PlaymodeLauncher.IsRunning=False` | ✅ |
| PrefabStage | `null` | ✅ |
| 场景 | `SampleScene` `dirty=False / rootCount=5` | ✅ |
| 当前用户单槽（本轮实测，不套历史 hash） | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，`*.tmp=0` | ✅ 未写未删 |
| Initialization guard | `dirs=95 / restored=95 / protective=0 / other=0 / noManifest=0` | ✅ 无未完成（95 = 阶段 1 前 87 + 阶段 1 门禁 8） |
| R3B guard | `dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective = 既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体 | 磁盘 `14961033 B / 3c758e16…c404854`；`LoadAllAssetsAtPath` = **9** 子资产、`IsDirty` = **0** | ✅ |
| 34 项基线 | `total=34 / same=34 / changed=0 / missing=0` | ✅ |
| 编译产物 | `VillageStage1.PlayModeTests.dll` `162816 B / c10d0045…`，mtime `2026-10-29T11:22:21Z`（本阶段无 .cs 改动，无需 Refresh/编译） | ✅ 未触发多余刷新 |
| Console | 阶段 1 门禁 run 之后**项目 / 测试程序集** `Error/Exception` = **0**；`102912/102915` 两条为本层只读测量 snippet 自身失败（`Func<>` 笔误 / 探测路径不存在），原样保留不清洗 | ✅ |

### 22.5 宿主门禁（本阶段只交 World 三例，不预填 PASS）

| 项 | 内容 |
|---|---|
| 执行参数 | `testMode=PlayMode` + `testClass=VillageWorldProductIntegrationPlayTests`（整类 **3 例串行**） |
| 通过标准 | **3/3 Passed、0 skipped**；0 项 / skip 不算通过 |
| 逐条 fullName（备用） | `…Presence_DerivedAndWorldVisible_WithoutAnyManagementPanel_And_SurvivesSimToggle`、`…ProjectCompletion_WorldSiteTrace_PersistsAfterSaveReload_WorldRestored`、`…MainMenuPhase_DebugViewHidden_UntilTownSimulationActive` |
| 不跑 | 已过 `Compiler 280`、管理 17、Kitchen 1、Edit 9、施工 2、Home、初始化产品专项、承诺 8（本轮已 8/8）、完整 Edit/Play——均不重跑 |
| 预期副作用 | 3 个 Initialization guard run（`captured → restored`，逐字节回读）；用户单槽每例结束回到 `13134 B / adcf5d8b…`；不写字体、不改场景 / Prefab |
| 失败处理 | 保留真实 `message` / `stackTrace`，按栈窄修（夹具问题改夹具，落到产品 Presence / World / Save / Load 才做最小产品修复）；**不得削弱业务强断言**；不无限重试 |

### 22.6 未验证 / 风险

1. **World 三例未运行**（执行层禁止 `tests-run`）：上述结论为静态 + 反射一致性证据，**不是通过回执**。
2. **数值口径依赖既有 Test 内容**：`prj_test_02` 目标 6 / 每天 2、Farm 实例岗位容量 2 均来自既有 Profile；若后续改动 Profile，断言需随 Profile 更新（本轮未写死新数值、未改 Profile）。
3. **视觉维护请求 `bcce0eefd2d8…` 无回执**：A 档 Kitchen / House / Person 真实美术仍未解决（§3 / §17 原样保留），未写 XLSX / Mirror / 生成资产，不虚构已修。
4. **本阶段未重跑**任何已过类，历史回执口径不变。

---

## 23. 2026-10-29 增量（阶段 3/4）：实体锚点 / 原型切换两例 · 前阶段核验与静态成员审查

> 范围：确认阶段 2/4（World 三例）宿主门禁与保护面，机械核对 `VillageVisualPrototypePlayTests` 两例的反射入口、DTO 构造与正式世界锚点语义，交宿主整类门禁。**本阶段未运行任何测试**（`tests-run` 禁令），**未改动任何 `.cs`**（原因见 §23.3），不预填 PASS。
> **A 白模美术缺口未修、也不冒充已修**：`Kitchen / House / Person` 在 A 档仍为 `neutral`（§3 / §17 原样保留），本包两例只验证「同一实体 / 锚点 / Domain 稳定」，测试 Setup 仍断言 `A whitebox remains the default`。维护请求 `bcce0eefd2d8…` **仍无回执**，未写 XLSX / Mirror / 生成资产。

### 23.1 阶段 2/4 宿主门禁确认（只读回看，未重跑）

| 项 | 实测 |
|---|---|
| `testResults` World 三例 | **3/3 Passed、0 fail、0 skip**：`MainMenuPhase_DebugViewHidden_UntilTownSimulationActive` 1558 ms、`Presence_DerivedAndWorldVisible_WithoutAnyManagementPanel_And_SurvivesSimToggle` 1881 ms、`ProjectCompletion_WorldSiteTrace_PersistsAfterSaveReload_WorldRestored` 1700 ms（均 `message=null`） |
| 该 run 的 guard | `20261029T113149Z / 113151Z / 113153Z`：`status=restored`；`original-save.bin` 实际 SHA == `backupSha256`；`sourceSha256 == backupSha256`；**当前用户档 SHA == sourceSha256** → **3/3 逐字节完整恢复** |
| Console | 该 host run 之后**项目 / 测试程序集** `Error/Exception` = 0（当前缓冲区仅剩本层只读测量 snippet 自身失败，见 §23.4） |
| 结论 | 阶段 2/4 交的 World 三例由宿主真实回执确认；无 captured / restore-failed |

### 23.2 两例静态核查（真实执行，原始输出另存 `play-evidence/visual-two-static-member-audit-20261029.txt`）

**反射入口机械核对**（读取源码抽取 `Static/Call/Get` 调用 + 实时反射解析）：

| 调用 | 真实签名 | 结论 |
|---|---|---|
| `Static("VillageRuntimeBridge","TryPlaceBuilding", args[5])` | 唯一 5 参：`(string, float, float, out BuildingInstance, out string)` | 唯一命中；`out` 由 `object[]` 回填 |
| `Static("SaveSystem","NewGame")` / `"Save"(1)` / `"GetSavePath"` | `NewGame` 2 重载（0/1 参），`Save` 1（1 参），`GetSavePath` 1（0 参） | 0 实参 → 唯一命中 `NewGame()`；无 `AmbiguousMatch` |
| `Static("SaveSystem","BuildSaveData",_player,bridge.BuildSaveData())` | `SaveSystem.BuildSaveData` **唯一 1 个重载（2 参）** | 正确 |
| `Call(_host,"RefreshNow"/"Reconcile"(1)/"SetVisualPrototype"(1))` | 各唯一 | 正确 |
| `Get(_host,"BuildingViews"/"PersonViews"/"PlaceViews"/"Selection"/"Shell"/"VisualPrototypeEnabled"/"Snapshot")` | 均为实例 PROPERTY，getter 存在 | 正确 |
| `Get(Shell,"Build")` → `OpenCatalog(0)/Choose(1)/SetCandidate(1)/Cancel(0)/PreviewValid` | `HomeProductShell.Build : HomeBuildingPlacement`，各唯一 | 正确 |
| `Call(person,"RefreshSelection")` / `Get(person,"DepthOrder"/"LabelVisible"/"ResolvedSharedAsset")` | 唯一 / 实例 PROPERTY | 正确 |
| `new HomePlaceSnapshot(…9 args…)` / `new ProductHomeSnapshot(…4 args…)` | `HomePlaceSnapshot(id,name,kind,source,capacity?,assigned,occupants,status,fallback, buildingInstanceId=opt, drawsWorldEntity=opt=true)`；`ProductHomeSnapshot(persons,places,diagnostics,buildings=opt)` | 9 参合法，**Tavern 默认 `drawsWorldEntity=true`**（必须被绘制）；4 参合法 |
| `Clock`（`T("VillageRuntimeBridge").GetField("Time",S)`） | `Time` 是 **FIELD** `VillageTime` | 正确 |

**与正式世界语义的一致性（源码核对，不改断言）**：

| 检查 | 事实 |
|---|---|
| 两处放置坐标合法性 | `BuildingPlacementValidator`：`MinX=-12/MaxX=12/MinY=-10/MaxY=8`，重叠判定为**严格小于**；`(-8,-4)` farm(3×2)、`(0,-4)` house(2×2) 与默认 7 实例（farm/kitchen `y=-1.2`、house `y=4.2`）均不重叠且在界内 |
| `entities` 来源 | 取自 `BuildingViews`（= `Snapshot.Buildings` 全部真实实例，默认 7 + 新放 2），**不是** `PlaceViews`；A/B 切换只 `SetVisualPrototype`，不重建实体 |
| 实例岗位锚点 | `registered` 由 `Snapshot.Places` 中 `workplace:` 前缀派生（默认 2 个：Farm / Kitchen 实例岗位）；`ProductHomeReadAdapter.LinkedBuildingId` 为 Workplace Place 绑定真实 `BuildingInstanceId` → `BuildingInstanceId` 非空、`BuildingViews.Contains(...)` 成立、`DrawsWorldEntity=false` → 断言「不得在 `PlaceViews` 再画第二套」当前成立 |
| Tavern 活动 Place | 测试合成快照（末参默认 true → 需绘制）；`_prototypeAnchors` 以 PlaceId 持有会话锚点 → 3 轮出现/消失不回流已注册建筑视图 |
| 原型默认档 | Setup 断言 `VisualPrototypeEnabled == false`（**A 白模仍是默认**，与 §17 缺口如实并存） |

**结论**：两例未发现与正式实例世界冲突的成员访问、DTO 构造或锚点语义；本阶段不改测试、不改产品。

### 23.3 本阶段文件变更

| 文件 | 变更 |
|---|---|
| `Assets/Scripts/**`（产品） | **0 改动** |
| `Assets/Tests/**`（测试） | **0 改动**（`VillageVisualPrototypePlayTests.cs` 18174 B / `244105d6…`，mtime `2026-10-29T11:14:43Z`，为阶段 20 适配后的现状；本阶段机械核查通过，不做无依据改动） |
| `Development/StudioRuns/Initialization-20261027/play-evidence/visual-two-static-member-audit-20261029.txt` | 新增（本轮核算原始输出 + 前阶段门禁 + 保护面） |
| 本文档 | 追加 §23 |

### 23.4 保护面（本阶段只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Pilot 身份 / 版本 | `RRPG_AI_Pilot / DefaultCompany / 2022.3.62t9` | ✅ |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| 原生 runner / pending | `Tool_Tests.HasPendingTestRun=False`；`EditModeLauncher.IsRunning=False`；`PlaymodeLauncher.IsRunning=False` | ✅ |
| PrefabStage | `null` | ✅ |
| 场景 | `SampleScene` `dirty=False / rootCount=5` | ✅ |
| 当前用户单槽（本轮实测，不套历史 hash） | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，`*.tmp=0` | ✅ 未写未删 |
| Initialization guard | `dirs=98 / restored=98 / protective=0 / other=0 / noManifest=0`（98 = 87 + 承诺 8 + World 3） | ✅ 无未完成 |
| R3B guard | `dirs=345 / restored=344 / protective=1 / other=0 / noManifest=0`（唯一 protective = 既有 `20261024T052704Z`） | ✅ 无 bad |
| 受保护字体 | 磁盘 `14961033 B / 3c758e16…c404854`；9 子资产、`IsDirty=0` | ✅ |
| 34 项基线 | `34/34 same / changed=0 / missing=0` | ✅ |
| Console | 前两阶段 host run 之后项目 / 测试程序集 `Error/Exception` = 0；`102936/102938` 等为本层只读 snippet 自身失败（`Assembly-CSharp`-only 类型查找笔误），原样保留 | ✅ |

### 23.5 宿主门禁（本阶段只交视觉两例，不预填 PASS）

| 项 | 内容 |
|---|---|
| 执行参数 | `testMode=PlayMode` + `testClass=VillageVisualPrototypePlayTests`（整类 **2 例串行**） |
| 通过标准 | **2/2 Passed、0 skipped**；0 项 / skip 不算通过 |
| 逐条 fullName（备用） | `RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests.ProfilesPreserveEntitiesFootprintsSelectionAndDomain`、`RRPG.ProductHome.Tests.VillageVisualPrototypePlayTests.ActivityPlacesCannotReflowRegisteredWorkplaces` |
| 不跑 | 已过 `Compiler 280`、管理 17、Kitchen 1、Edit 9、施工 2、Home、初始化产品专项、承诺 8、World 3——均不重跑 |
| 预期副作用 | 2 个 Initialization guard run（`captured → restored`，逐字节回读）；用户单槽每例结束回到 `13134 B / adcf5d8b…`；例 1 会通过真实 API 放 2 个建筑（只改本次隔离世界，TearDown 恢复单槽）；不写字体、不改场景 / Prefab |
| 失败处理 | 保留真实 `message` / `stackTrace` 按栈窄修（夹具问题改夹具，落产品锚点 / 视图 / Save 才做最小产品修复）；**不得削弱实体稳定 / 锚点 / Domain 强断言**；不无限重试 |

### 23.6 未验证 / 风险

1. **两例未运行**（执行层禁止 `tests-run`）：上述为静态 + 反射 + 源码语义证据，**不是通过回执**。
2. **A 白模美术缺口仍在**（§3 / §17）：Kitchen / House / Person 默认 A 档仍是 `neutral`；本包不冒充已修，也不改默认档。
3. **Test1 会真实放置 2 个建筑**（farm / house，`UnderConstruction`）：属本次隔离世界；若宿主并行运行其它写世界用例，仍需按整类串行门禁。
4. **本阶段未重跑**任何已过类，历史回执口径不变。

---

## 24. 2026-10-29 增量（视觉两例失败按真实栈窄修 · 阶段 1/2）

> 范围：只修 `VillageVisualPrototypePlayTests` 两例的**测试侧**失败（真实宿主门禁 `795c8934-c0f4-4567-9591-3ab46323f550`：视觉 2 total / 0 pass / 2 fail / 0 skip / 1.4510682 s）。先读 `testResults` 真实 `message` / `stackTrace` 与源码定位，**未运行任何测试**（执行层仍禁 `tests-run`，交 Studio 阶段 check），未用静态核查冒充通过。产品代码 0 改动；A 白模缺口原样保留，未改预期。

### 24.1 真实失败与原因（宿主回执原文）

| 例 | 宿主 message / stackTrace 落点 | 精确原因 |
|---|---|---|
| `ActivityPlacesCannotReflowRegisteredWorkplaces` | `Expected: same as <Building_building_instance_1 (HomeWorldEntityView)> But was: null`；`VillageVisualPrototypePlayTests.cs:288` | 第 288 行仍索引 `PlaceViews[workplace:<InstanceId>:main]`；但正式契约里实例岗位 `DrawsWorldEntity=false`，只由真实 Building 视图承载，`PlaceViews` 按设计**不含**该 id → 取回 null。**不是产品删除建筑**：同一测试第 270/271 行已先断言 `BuildingViews.Contains(BuildingInstanceId)` 且取到该视图，第 288 行期望值本身也是真实 `Building_building_instance_1` 视图；若 `Reconcile` 删了真实建筑，会先在第 270/271 行失败而不是第 288 行。 |
| `ProfilesPreserveEntitiesFootprintsSelectionAndDomain` | `String lengths are both 7858. Strings differ at index 65. Expected ... "SavedAtUnixTime":1793273679 ... But was ...1793273680`；`VillageVisualPrototypePlayTests.cs:240` | `SaveSystem.BuildSaveData` 每次采样都把 `SavedAtUnixTime=DateTimeOffset.UtcNow.ToUnixTimeSeconds()` 写入 SaveData（`SaveSystem.cs:499`）——这是**墙钟元数据**，不是模拟 / 世界时钟。第 231 行两次采样同一秒侥幸通过，第 240 行跨秒即失败；除该字段外 7858 字节完全相同。 |

### 24.2 最小修复（只改测试，不动断言强度）

| 文件 | 变更 | 公开契约保留 |
|---|---|---|
| `Assets/Tests/PlayMode/Village/VillageVisualPrototypePlayTests.cs` | 1) 新增 `CaptureDomain` / `AssertDomainFactsUnchanged`：只把 `SavedAtUnixTime` 这一个非模拟字段替换为 `<wall-clock>` 后逐字节比较其余 Save 内容，并单独比较 `VillageTime.ToSaveData()`（世界时钟）与墙钟秒合理性；2) 第 288 行改为「同一 `BuildingViews[BuildingInstanceId]` 视图实例仍在」+「`PlaceViews` 不得包含该实例岗位 id」；3) Tavern 快照构造前显式断言 baseline/active 保留真实 `Persons / Diagnostics / Buildings`，并断言 `Reconcile(baseline)` 后 `BuildingViews` 键集合与真实建筑一致 | 实体身份（`AreSame` 视图实例）、锚点、sprite、Body 可见、人物选择/深度、`Domain` 除墙钟外逐字节等值、A/B 往返锚点全部保留；未删 / 未 Ignore / 未改默认白模断言；**产品 `Assets/Scripts/**` 0 改动** |

- 记时间戳与世界时钟的区别：`SavedAtUnixTime` = 墙钟（`DateTimeOffset.UtcNow`），允许并只允许它随采样变化；`VillageTime.ToSaveData()`（Year/Month/Day/Speed/SecondsIntoDay/IntradayTime）必须逐字节不变，测试日志 `[VillageVisualPrototypePlayTests][DOMAIN] ... wallClockOnly=true worldClock=...` 留证。
- 同类 helper 核对：`ProductHomeFoundationPlayTests.Domain()` 只拼 `VillageRuntimeBridge` + `ActivityRuntimeBridge` 的 `BuildSaveData`，不含 `SaveSystem` 墙钟字段；其余测试的 `BuildSaveData` 比较同为 world-only。**本阶段无第二处同类跨秒偶发**（不是假设：按 `SavedAtUnixTime` 全仓 grep，写入点只有 `SaveSystem.BuildSaveData`，`Domain()` 只有本文件调用该入口）。

### 24.3 编译 / 证据（真实执行）

| 项 | 实测 |
|---|---|
| 安全 Refresh | `AssetDatabase.Refresh(ForceSynchronousImport)` 一次；随后 `isCompiling=False / isUpdating=False / isPlaying=False` |
| 编译 | 测试程序集重编译成功：`VillageStage1.PlayModeTests.dll` 166912 B / mtime `2026-10-29T11:37:35Z`；反射确认 `CaptureDomain`、`AssertDomainFactsUnchanged` 已进入程序集；Console 中 `BuildingRuntime` 编译错误计数 = 0（当前缓冲区仅剩更早的 snippet 自身失败） |
| 源文件 | `VillageVisualPrototypePlayTests.cs` **22923 B / `e9098d3a16529a04b390aa61f256c939acac848ffba84c1bcffb014890839eca`**（原 18174 B / `244105d6…`） |
| 未跑 | 两例本体未运行；通过/失败交 Studio 阶段 check，不预填 PASS |

### 24.4 保护面（本轮只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Pilot 身份 | `RRPG_AI_Pilot / DefaultCompany / 2022.3.62t9` | ✅ |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| 原生 runner / pending | `Tool_Tests.HasPendingTestRun=False`；`EditModeLauncher.IsRunning=False`；`PlaymodeLauncher.IsRunning=False` | ✅ |
| PrefabStage | `null` | ✅ |
| 场景 | `SampleScene` `dirty=False / rootCount=5` | ✅ |
| 当前用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08 / tmp=0`（只读） | ✅ |
| 字体（磁盘 + 子资产） | `14961033 B / 3c758e16…c404854`；`subAssets=9 / IsDirty=0` | ✅ |
| 34 项基线 | `total=34 / same=34 / changed=0 / missing=0` | ✅ |
| Initialization guard | `dirs=100 / restored=100 / protective=0 / other=0 / noManifest=0`（100 = 原 98 + 视觉失败 2 run）；最近两 run `20261029T113438Z`、`20261029T113439Z`：`status=restored`、`source/backup/当前档` 三者 SHA 同为 `adcf5d8b…8ceb08`、`readbackVerified=true`、`lastError=null` | ✅ 上一视觉 2 run 已逐字节恢复 |
| 全仓 `save-guard` 目录扫描 | 仅 `Development/StudioRuns/Initialization-20261027/save-guard` 一处；§23.4 记的 R3B 345 目录在本工作区当前不存在（本轮未伪造、未覆盖） | 如实记录 |

### 24.5 未验证 / 边界

1. **两例未运行**：交 Studio 阶段 check（`testClass=VillageVisualPrototypePlayTests`，2 例串行），不预填 PASS。
2. **A 默认白模**仍是待修产品缺口（Kitchen / House / Person `neutral`），本轮未改预期、未偷绿。
3. 维护请求 `bcce0eefd2d80b77fe1a465620c1542e8a0f5f842b5e28a5ebcfd48c32286010` **仍无回执**；未写 XLSX / Mirror / 生成资产。
4. 未重跑任一已过类（Compiler 280 / Edit 9 / 厨房 1 / 施工 2 / 管理 17 / Home 等），历史口径不变；未用 Shell / Process / tests-run / rql-mutate / SaveAssets / SaveChanges / Git / PrefabStage / 启停 Editor。

---

## 25. 2026-10-29 增量（阶段 2/2）：视觉真实回执 + V4 发现确认与剩余专项交门禁

> 范围：回看视觉两例的**宿主真实结果**与保护面；确认 `VillageV4ResourceSurplusPlayTests` 的实际调用面、namespaced 类与“整类 1 例”发现；交宿主整类门禁。**本阶段未运行任何测试**（执行层禁 `tests-run`），V4 结果**不预填**；未改任何 `.cs`（测试源 `25113 B / 6ae821e0…` 与 §20 一致），产品 0 改动。

### 25.1 视觉两例真实回执（只读回看 `testResults`，非静态推断）

| 项 | 实测 |
|---|---|
| `testResults` | `ActivityPlacesCannotReflowRegisteredWorkplaces` = **Passed / 1000 ms / message=null**；`ProfilesPreserveEntitiesFootprintsSelectionAndDomain` = **Passed / 491 ms / message=null** → **2/2 Passed、0 fail、0 skip** |
| 运行日志 | 两例均产生 `[SAVE-CAPTURE] runId=20261029T113932Z / 113933Z existed=True bytes=13134 sha256=adcf5d8b…8ceb08`；`[DOMAIN]` 日志逐条 `wallClockOnly=true`，`worldClock={"Year":1,"Month":1,"Day":1,"Speed":0,...}` —— 时间戳与世界 Clock 分离已按 §24 生效 |
| guard 恢复 | guard 总数 `102`（= 原 100 + 视觉新 2 run）；最近 `113932Z` / `113933Z` 均 `status=restored`、`sourceSha256==backupSha256==binSha`、`lastError=null` → **逐字节恢复** |
| 用户档 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08 / tmp=False`，与捕获值一致 | 

### 25.2 V4 实际方法与 namespaced 类发现（实时反射 + `tests` 源）

| 项 | 实测 |
|---|---|
| 发现 | `tests | where fullName contains "VillageV4ResourceSurplus"` = **1 条**：`RRPG.Village.Tests.VillageV4ResourceSurplusPlayTests.FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry`，`mode=PlayMode / ignored=false / explicit=false` |
| 程序集 / namespace | `VillageV4.PlayModeTests`；`namespace RRPG.Village.Tests`；程序集内带 NUnit/UnityTest 特性的方法数 = **1**（无重复类 / 无第二份 partial） |
| 测试源 | `Assets/Tests/PlayMode/VillageV4/VillageV4ResourceSurplusPlayTests.cs` **25113 B / `6ae821e0ac3f4cec80bd590fc2b284968d5796b7a04774bcde86e30024d390cc`**（本轮未改） |
| 反射入口（实时） | `VillageRuntimeBridge.ResetForTests/0`、`SetSimulationPersonSource/1`、`Buildings:BuildingRuntime`（`All:IReadOnlyList<BuildingInstance>`）、`Resources:VillageResourceState`（`Ingredients/Food` 均为 int 属性）、`LastResourceDayResult:VillageResourceDayResult`、`LastResourceInVillageCount/LastResourceExecutedFactCount:int`、`TryAssignPerson/2`；`SaveSystem.NewGame` 重载 `[0,1]`（测试调 0 参，唯一）、`Save/1`、`GetSavePath/0`；`MainMenuUI.OnContinue/0`、`TownUI.canvasRoot:GameObject`、`PlayerData:PlayerCollection`；`ExpeditionUI.ShowExpeditionFromTown/1`、`_identityPanel:ExpeditionIdentityPanel`、`btnConfirm:Button`、`OnConfirmExpedition/0`、`_currentAdventure:AdventureManager`、`RequestRetreatFromProgression/0`；`ExpeditionIdentityPanel.SupplyText:string`；`ExpeditionJourneyView.IsVisible:bool / SupplyDisplay:string`；`VillageResourceDayResult` 8 个 int 字段全部存在（`IngredientsProduced/FoodProduced/IngredientsConsumedByKitchen/FoodConsumed/PersonsFoodFed/PersonsEmergencyFed/PersonsUnderfed/EmergencyIngredientsConsumed`）；`AdventureManager.ProgressionState:ExpeditionProgressionState`、`Supply:ExpeditionSupplyState.InitialCarryUnits:int`；`VillageAssignmentResult.Success` 存在 |
| 断言保留面 | 真实 `NewGame + Save + OnContinue` 隔离世界；`InstanceWorkplace` 从运行时 `Buildings.All` 的 Completed 实例派生 `workplace:<InstanceId>:main`（不写死 `ws_*`）；3 天逐日 Farm 产出 / Kitchen 加工 / InVillage / Food 净增；`JumpToNextMonth` 显著积累；准备界面真实 Food 文本；`carry>0` 且 `Departure` 后 Village Food **恰好减少 carry**（真实 FarmKitchen 生产 / 远征 Carry 对账）；Journey 非 0 补给；Retreat 收尾。**未改断言、未注入资源** |
| 未执行 | `testResults` 无 V4、Console 无 `[VillageV4ResourceSurplusPlayTests]` 日志 → **V4 尚未运行**，结果不预填 |

### 25.3 保护面（视觉 run 之后只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Pilot 身份 | `RRPG_AI_Pilot / DefaultCompany / 2022.3.62t9` | ✅ |
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False` | ✅ 空闲 |
| 原生 runner / pending | `HasPendingTestRun=False`；`EditModeLauncher.IsRunning=False`；`PlaymodeLauncher.IsRunning=False` | ✅ |
| PrefabStage / 场景 | `null`；`SampleScene dirty=False roots=5` | ✅ |
| 用户单槽 | `13134 B / adcf5d8b…8ceb08 / tmp=False`（只读） | ✅ |
| 字体（磁盘 + 子资产） | `14961033 B / 3c758e16…c404854`；`subAssets=9 / IsDirty=0` | ✅ |
| 34 项基线 | `total=34 / same=34 / changed=0 / missing=0` | ✅ |
| Initialization guard | `dirs=102 / restored=102 / protective=0 / other=0 / noManifest=0` | ✅ 无未完成 |

### 25.4 未解决视觉缺口（不冒充已修）

- **A 默认白模仍是产品缺口**：`PrototypeEnabled=false` 时 Kitchen / House / Person 仍走 `neutral`，§3 / §17 原样保留；本轮视觉两例只验证实体 / 锚点 / Domain 稳定，测试 Setup 仍断言 `A whitebox remains the default`，**未改预期、未偷绿**。
- 维护请求 `bcce0eefd2d80b77fe1a465620c1542e8a0f5f842b5e28a5ebcfd48c32286010` **仍无回执**；未写 XLSX / Mirror / 生成资产。

### 25.5 宿主门禁（本阶段只交 V4 一例，不预填 PASS）

| 项 | 内容 |
|---|---|
| 执行参数 | `testMode=PlayMode` + `testClass=VillageV4ResourceSurplusPlayTests`（整类 **1 例**） |
| 逐条 fullName | `RRPG.Village.Tests.VillageV4ResourceSurplusPlayTests.FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry` |
| 通过标准 | **1/1 Passed、0 skipped**；skip 不算通过 |
| 预期副作用 | 1 个 Initialization guard run（`captured → restored`，逐字节回读）；用户单槽每例结束回到 `13134 B / adcf5d8b…`；只经真实 NewGame / Save / OnContinue 建立隔离世界，不注入资源 |
| 不跑 | 已过 Compiler 280 / Edit 9 / 厨房 1 / 施工 2 / 管理 17 / Home / 承诺 8 / World 3 / 视觉 2 均不重跑 |
| 失败处理 | 保留真实 `message` / `stackTrace` 按栈窄修；不得削弱真实生产 / Carry 对账断言；不无限重试 |

### 25.6 未验证 / 边界

1. **V4 未运行**：本节只是方法 / 类 / 发现的真实核对与门禁交办，**不是通过回执**。
2. **A 白模缺口仍在**（§25.4）；`bcce0eefd2d8…` 无回执，未写 XLSX / Mirror / 生成资产。
3. 未重跑任一已过类，历史口径不变；未用 Shell / Process / tests-run / rql-mutate / SaveAssets / SaveChanges / Git / PrefabStage / 启停 Editor。
