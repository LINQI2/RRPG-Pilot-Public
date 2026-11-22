# 双 FunctionSlot 真实 Gateway 验证：合法构造范围核验与最小方案调查（2026-10-26，阶段 1/1）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包性质：**只读核验 + 调查报告**。**未新增测试**（结论：现有公开入口无法建立可达真实 Gateway 的双 slot 定义）、**未改代码 / 测试 / Prefab / Scene / 配置 / Authority / DTO / Save**、**未运行 `tests-run`**、**未 `AssetDatabase.Refresh`**、**未 `rql-mutate`**、**未 Shell / Git 写 / Preferences / PrefabStage**、**未重跑既有 13 Play / 18 Edit**。
- 本包实际写入：**1 个文件**——本文件（新增）。未改 `ManagementProjectionTests.cs` 及其它任何文件。
- 报告时刻：**2026-10-26T10:42Z**（本机 `2026-10-26T18:42+08:00`，`China Standard Time`），由只读 `script-execute` 读 `DateTime` 取得。
- 结论前置：**"新建一栋双 FunctionSlot 真实定义并由真实 `ManagementCommandGateway` 派工 / 满员拒绝 / 撤职"在当前公开 API 下不可行**；硬阻塞点是 `ManagementCommandGateway.AssignToWorkplace` 第 40–46 行对 `StaticConfigRegistry.TryGetBuildings` 的固定依赖，而 `StaticConfigRegistry._root` 只有 `Reset()` 清空、没有公开/内部注入点。**本包不臆造该方法、不造 0 项测试门禁，阶段停在证据**。最小合法方案在 §7，是否采纳由主开发决定。

---

## 1. 现场前置实测（全部为本轮只读实测，非转述）

| 项 | 实测值（2026-10-26T10:42Z） | 判定 |
|---|---|---|
| Editor 空闲 | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `Assets/Scenes/SampleScene.scene`：`dirty=False`、`isLoaded=True`、`roots=5` | 干净 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage()==null` | 未打开（本包不打开） |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()` = **False**（该方法为 `static` 且 **非 public**，经反射调用） | 无活动 / 残留运行 |
| Screen | `1920x1080` | 未切 1280（1280 已由制作人 `01a53eb8` 取消） |
| Console | `Error` / `Exception` 查询 **0 条** | 无源码编译错误、无残留异常 |
| 受保护 TMP 字体（磁盘） | `14961033B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；mtime `2026-10-10T13:23:55.9520319Z` | 与基线同值 |
| TMP 字体（内存子资产） | `LoadAllAssetsAtPath` = **9** 个子资产，`dirtySubassets=0`（`TMP_FontAsset` / `Material` / `Texture2D`×7 全部 `dirty=False`） | 9 个 clean、0 dirty |
| 34 项 workspace 基线 | `R3B-20261022/workspace-baseline.json` 逐项 bytes+SHA256：`total=34 / same=34 / diff=0 / missing=0` | **34/34 一致** |
| 单槽存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：`exists=True`、**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T10:29:35.1763281Z`、`.tmp=false` | 与收尾时刻一致 |
| guard 根 `R3B-SaveProtection-20261024` | **244** 个 run 目录：`restored=243`、`protective-snapshot=1`、`noManifest=0`、其它状态 **0**；最新 `20261026T102934Z`（`restored` / `readbackVerified=true` / `sourceSha256=4b2b56f6…d6aa`） | 全部终态合法，无 unfinished / 无未知残留 |
| 磁盘 | `D:\` free = **26.99 GB** | 充足（本包不新增资源） |
| 测试源码 / 测试程序集 | `ManagementProjectionTests.cs`：`54062B` / `f48f8f7d0cb9bfe299c3c8cbc1a6814540bd178495e632f907eedf21e9f16e0b`、mtime `2026-10-26T03:08:46.6725349Z`、`[Test]` **18** 处；`VillageStage1.Tests.dll`：`153088B` / `9d09dabbb127fc58235e271d1029c2b57ae2c155d9d3dc1fab08b86fa65b69da`、mtime `2026-10-26T03:08:50.4871113Z` | 与索引记录逐字节一致；**本包未触碰**（DLL 晚于源码 3.81 s） |

**TMP 探针纪律（避免副作用）**：本轮只做 `AssetDatabase.LoadAllAssetsAtPath` + `EditorUtility.IsDirty` 只读判定，**未**调用 `GetPreferredValues` / `ForceMeshUpdate` / `SetDirty` / 重导入 / 保存；字体 9 子资产 0 dirty，磁盘 hash 与基线同值。**读得 0 dirty 只证明观察面，不推断全机零副作用。**

---

## 2. 规格原文对该项的要求（不改门槛、不新增规则）

| 出处 | 原文（摘） |
|---|---|
| R3-P Spec §4（`Docs/DevelopmentSpecs/2026-10-09_…V0.1.md:81`） | 「职责与场所：一行一个 `WorkplaceId`，关联 `BuildingInstanceId` + `FunctionSlotId`，容量 / 已绑定名额 / 实际劳动人数 / 现场人数分列」；「已 Completed 动态岗位可 `TryAssignPerson` / `RemovePersonAssignment`」 |
| 同 §4（`:84`） | 「动态岗位通过正式 Building registry 的 FunctionBindings 枚举 slot，使用现有 `BuildingWorkplaceId(instance, slot)` 与实际注册 `GetWorkplaceDef` **相互核对，不按同名合并，不仅取 FirstOrDefault**」 |
| 同 §4（`:88`） | 「岗位名额是长期 Assignment 绑定数……实际在场 / 实际劳动数独立计算」 |
| §8 测试矩阵（`:157`） | 「EditMode：管理命令 —— 两同类建筑多 Slot 独立、未完工拒绝派岗、满员 / `AlreadyAssigned` / 重复加入拒绝或幂等；人物离开 / 消失 / busy 时不写」 |
| §8 执行纪律（`:166`） | 「实施验证按 Compile/Console → 专项 Edit → 专项 Play → 受影响完整 Edit/Play **串行**执行；不并行 Test Runner、**不把 0 发现算 PASS**」 |

即：**"一行一 `(WorkplaceId, BuildingInstanceId, FunctionSlotId)` 且多 slot 名额独立"是原文门槛，本包不改门槛**；本包只核验"能否用合法测试构造命中**真实 Gateway**"。

---

## 3. 现有 18 例中多 slot 用例的真实覆盖面（这是关键事实）

`Assets/Tests/EditMode/Village/ManagementProjectionTests.cs:83-123`（`SameDefinitionBuildingsAndRegisteredSlotsStayIndependent`）实际做的是：

1. `Complete("building_farm", 0/1)` 用**真实** `TryPlaceBuilding` + 真实施工/完工链建两座同类完工建筑（`:873-884`）。
2. 断言两座建筑各自 `main` 的 `WorkplaceId` / `BuildingInstanceId` / `Capacity=2` / 绑定人互不串（`:93-104`）。
3. 「第二 slot」部分（`:106-122`）：
   - `RegistryWithSecondSlot()`（`:886-900`）**在测试内本地 `new BuildingRegistry(definitions.All, bindings)`**，即**只存在于测试局部变量**的注册表副本；
   - `Assignments.TryRegisterWorkplace(workplace:…:aux)`（`:108-111`）走的是**公开**的运行时注册入口；
   - `Call("TryAssignPerson", …)`（`:112`）直接调 Bridge 的派工；
   - 断言用的 `Read(definitions)`（`:114`）是 `ManagementReadAdapter.Read(BuildingRegistry)`（`Assets/Scripts/Village/ProductHome/ManagementReadAdapter.cs:18-24`），其注释明确写着「definitions 非 null **只用于测试 / 诊断注入**……产品调用一律走 `StaticConfigRegistry` 的同一静态配置 Authority」。

**因此该用例证明的是"只读投影 + 运行时注册 + Bridge 派工"的多 slot 独立性，完全没有经过 `ManagementCommandGateway`。** 索引里"多 slot 独立已验证"只在**投影层**成立；`Gateway` 层第二 slot 仍是未覆盖项，与本轮调查报告一致。

---

## 4. 真实 Gateway 调用链（逐行读源码）与硬阻塞点

### 4.1 派工链

1. `ManagementCommandGateway.AssignToWorkplace(personId, workplaceId)`（`Assets/Scripts/Village/ProductHome/ManagementCommandGateway.cs:22-58`）
   - `:24` `CheckPerson`：来源存在 / 在村可用 / `CanSaveWorld`（不涉及 slot 定义）。
   - `:27-29` 必须是 `workplace:` 前缀。
   - `:31-35` 解析 `instanceId` + `slot`，`VillageRuntimeBridge.Buildings?.Find(instanceId)`。
   - `:36-38` 必须 `BuildingLifecycleState.Completed`。
   - **`:40-46`（第一道定义校验）**：`StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, …)` → `definitions.FunctionBindings.FirstOrDefault(f => f.BuildingDefinitionId == building.BuildingDefinitionId && f.FunctionSlotId == slot)`；`null` → `TargetNotRegistered`。
   - `:48-52`（第二道运行时注册校验）：`assignments.GetWorkplaceDef(workplaceId)` 必须非 null。
   - `:54` `VillageRuntimeBridge.TryAssignPerson` → `:55-57` `Success` / 领域诊断原样保留（`CapacityFull`、`AlreadyAssigned`）。
2. `StaticConfigRegistry.TryGetBuildings`（`Assets/Scripts/StaticConfig/StaticConfigRegistry.cs:147-156`）
   - `:150` `TryInitialize` → `:152` `_root.BuildingDatabase.CreateRegistry()`。
3. `StaticConfigRegistry.TryInitialize`（`:161-172`）
   - `:163-168` **仅当 `_initialized == false`** 时 `_root = RRPGStaticConfigRoot.Load()`。
4. `RRPGStaticConfigRoot.Load()`（`Assets/Scripts/StaticConfig/RRPGStaticConfigRoot.cs:92-95`）
   - `Resources.Load<RRPGStaticConfigRoot>(ResourcesPath)`，`ResourcesPath = "RRPGStaticConfigRoot"`（`:22`，注释写明「改这个常量 = 改架构契约」）。
   - `:30-47` 全部域库字段均为 **private `[SerializeField]`**，只有 getter。
5. `BuildingConfigDatabase`（`Assets/Scripts/StaticConfig/BuildingConfigDatabase.cs:5-16`）
   - `:11-14` `ApplyGeneratedContent(List<BuildingDefinition>)` / `ApplyGeneratedFunctions(List<BuildingFunctionBinding>)` 是 **public**，但写的是**该 ScriptableObject 实例自身**的序列化字段；
   - `:15` `CreateRegistry()` 每次按当前字段新建 `BuildingRegistry`。
6. `BuildingRegistry` 构造校验（`Assets/Scripts/StaticConfig/BuildingRegistry.cs:24-33`）
   - 逐条校验：定义必须存在、`FunctionSlotId` 非空且不含 `:`、`FunctionType=="Workplace"`、`BuildingFunctionBinding.IsKnownProfile(FunctionProfileId)`、`definitionId:slotId` 不重复。
   - `:44` `IsKnownProfile` 只接受 `ws_farm_01` / `ws_kitchen_01` / `ws_test_01` / `ws_test_02`（四个 profile 在 `VillageWorkplace.cs:42,49` 等运行时候选目录中都存在）。

### 4.2 施工 / 完工链（同样被同一道门卡住）

- `VillageRuntimeBridge.TryAddConstructionWorker`（`VillageRuntimeBridge.Buildings.cs:49-70`）：`:56` 直接 `StaticConfigRegistry.TryGetBuildings`，`:57` `FunctionWorkplaces(b, defs)` 校验完工输入。
- `FinalizeConstruction`（`:81-101`）：`:85` 再次 `StaticConfigRegistry.TryGetBuildings`，`:86-94` 才把每个 Function 注册成 `workplace:<instance>:<slot>` 并写入 `_resourceWorkplaceDefs`。
- 即：**双 slot 岗位只有在定义源里真有第二条 binding 时才会被完工链注册**；靠运行时手工注册（现有 `TryRegisterWorkplace`）虽可行，但那样 Gateway 的第一道校验仍然挡回。

### 4.3 阻塞点定性（一句话）

> **Gateway 的"该 slot 是否真实注册"判定 = `StaticConfigRegistry.Root.BuildingDatabase` 的内容；该 Root 的实例只能由 `Resources.Load("RRPGStaticConfigRoot")` 得到，且 `_root` 无 setter、无 `internal` 注入点、Runtime 程序集无 `InternalsVisibleTo`（全仓库 grep `InternalsVisibleTo` = 0 命中）。**

生产配置实际内容也确认只有 `main`：`Assets/Generated/StaticConfig/Databases/BuildingConfigDatabase.asset:36-47` 只有 3 条 binding（`building_farm`/`building_kitchen`/`building_workshop` 各一条 `main`），`RRPGStaticConfigRoot.asset:24` 指向该子资产。

---

## 5. 逐项排查"测试作用域合法构造"（全部按本包禁止项过滤）

| # | 候选合法构造 | 是否可行 | 事实依据 / 失败点 |
|---|---|---|---|
| A | 临时 **内存 ScriptableObject** `BuildingConfigDatabase`（`CreateInstance` + public `ApplyGeneratedContent/ApplyGeneratedFunctions`）再交给 Registry | **不可行** | 数据库能建、`CreateRegistry()` 也能建出双 slot 的 `BuildingRegistry`，但**没有任何公开入口把它交给 `StaticConfigRegistry`**：唯一读取点是 `_root.BuildingDatabase`（`StaticConfigRegistry.cs:152`），`_root` 由 `Load()` 独占赋值（`:165`）。 |
| B | 临时**独占测试资源**（仅 Pilot 测试目录）放一个 `Resources/RRPGStaticConfigRoot.asset` | **不可行（且危险）** | `Resources.Load` 按**名字**全局解析，路径是编译期常量 `"RRPGStaticConfigRoot"`（`RRPGStaticConfigRoot.cs:22`）。同名资产会让 Unity 回落到**不确定命中**（并告警），且会**遮蔽整机生产配置**（不只是测试作用域）。当前全仓库仅 `Assets/Generated/StaticConfig/Resources/` 一份。 |
| C | 用 public `RRPGStaticConfigRoot.ApplyGeneratedContent(...)` / `BuildingConfigDatabase.ApplyGeneratedFunctions(...)` 在**内存里改生产 root 的子资产**，Finally 再改回 | **禁止** | 这是对**正式生产配置资产实例**的序列化写入：会置 dirty，插件在普通 call 后自动保存 dirty 资产 → 等价于改 `BuildingConfigDatabase.asset`。且 `ApplyGeneratedContent` 需要同时传 9 个域库，任何漏传/传空都会破坏其它域。**本包明令不篡改正式生产配置。** |
| D | 只用**公开 Runtime API** 组合：`BuildingRuntime.TryPlace(registry, …)`（`BuildingRuntime.cs:50-57`，参数带 registry）+ `Assignments.TryRegisterWorkplace(…)`（`VillageAssignmentSystem.cs:39`） | **不可行** | 两条都能真跑，但 Gateway 的 `:40-46` 仍按 `StaticConfigRegistry` 的真配置查 binding → `aux` 恒为 `null` → `TargetNotRegistered`。（且 `BuildingInstance.LifecycleState` 的 setter 是 `internal`，`:13`，完工仍需走真实完工链。） |
| E | 改**产品内容**：在 Config Authority（xlsx / Mirror）给 farm 增加第二个正式 slot 后重建生成根 | 可行但**超范围** | 这会改变**玩家可见产品内容**（多出一个正式岗位），属 Config Authority / 产品规则变更，不是"测试构造"；本包无权，也不是测试问题。 |
| F | 反射写 `StaticConfigRegistry._root`（或 `_initialized`） | **禁止** | 任务明确"不反射写私有 Registry"。 |
| G | 给 Gateway 加一个带 `BuildingRegistry` 参数的重载（仿 `ManagementReadAdapter.Read(BuildingRegistry)`） | 可行但**本包禁止**，且不足 | 属改公共 API；且**单靠它还不行**：完工注册链（`FinalizeConstruction:85`）仍读 `StaticConfigRegistry`，双 slot 不会被自动注册，还要手工 `TryRegisterWorkplace`，"真实链"被削弱。 |

**结论：A–D、F、G 在"不改公共 API / 不改 Authority / 不改生产配置 / 不反射写私有 Registry"四条边界内全部不通；E 才是 Authority 面但会改产品内容。因此现在没有"最小隔离测试"可写。** 本包**不新增** `ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`，避免造出"永远 skip / 0 项"的门禁。

---

## 6. 为什么这不算"缺授权"就停（与索引断言的差别）

本包已把"要做到这件事，最少需要哪一处改动"查到底（§7）。当前状态是：**产品内容（只有 main）与 Gateway 定义校验的固定依赖共同构成结构阻塞**，不是"制作人没批"。要达到原规格的多 slot 真实 Gateway 验证，最少要动**一处**代码缝或一处**内容**，两者都不是本包可自行决定的：

- 若选代码缝 → 需要主开发批准"在 Runtime 增加一个 `SetInstanceForTests` 风格的公开测试缝"（工程决定，不动规则）。
- 若选内容 → 需要 Config Authority + 制作人批准（产品决定，会新增正式岗位）。

---

## 7. 实际可选的最小方案（供主开发派唯一门禁时选择，本包不实施）

### T1（推荐，单缝、覆盖面最大）

在 `StaticConfigRegistry` 增加一个**显式命名、内存注入、可逆**的测试缝，语义完全对齐仓库里**已有的同类先例**：

- `Assets/Scripts/UI/PersonVisualBindingCatalog.cs:75-96`：`public static void SetInstanceForTests(catalog)`（「测试 / 迁移用：注入一个内存配置面（null = 恢复按 Resources 路径读取）」）+ `ResetForTests()` + `SetEntriesForTests(rows)`（「仅内存注入，不写盘、不进入正式产品路径」）。
- `Assets/Scripts/Save/SaveSystem.cs:36`：`public static Func<bool> ForceSaveFailureForTests` 也是产品代码里的公开测试缝。
- 同型还有 `JourneyEventPresentationCatalog.SetInstanceForTests`（`Assets/Scripts/UI/JourneyEventPresentationCatalog.cs:81`）。

拟定形态（**仅为提案，本包未写**）：

```csharp
// StaticConfigRegistry.cs —— 与 PersonVisualBindingCatalog 同型；不写盘、不进产品路径
public static void SetBuildingsForTests(BuildingRegistry buildings) { _buildingsForTests = buildings; }
// TryGetBuildings 内：if (_buildingsForTests != null) { buildings = _buildingsForTests; diagnostic=""; return true; }
// Reset() 内：_buildingsForTests = null;
```

选它的理由（这是与 T2 的决定性差别）：**一处缝即可让整条真实链合法跑通**——`TryPlaceBuilding`（`:172-179`）→ `TryAddConstructionWorker`（`:49-70`）→ `FinalizeConstruction`（`:81-101`，自动注册两个 slot）→ `Gateway.AssignToWorkplace` 两道校验 → `TryAssignPerson` → 满员 `CapacityFull` / 撤职 `RemovePersonAssignment`。全程无反射写私有字段、无手工注册替身。

代价 / 风险：新增 1 个 Runtime 公开静态 API；必须在 `Reset()` 清除，测试 `Finally` 恢复真实 Root（`StaticConfigRegistry.Reset()` 后重新 `TryInitialize`）；需说明它**不进入产品路径**（与 `Read(BuildingRegistry)` 同一条"仅测试 / 诊断"注释纪律）。

对拟测用例的可行性预判（若 T1 获批）：一座 `building_farm` 定义 + `main`（profile `ws_farm_01`，Capacity 2）+ `aux`（profile `ws_farm_01` 或 `ws_test_02`）可通过 `BuildingRegistry` 校验（只禁同 `definitionId:slotId` 重复）；两个 slot 各自 Capacity 独立，第二 slot 可用 `Capacity=1` 的 profile 造"满员拒绝"。`WorkplaceId = workplace:<instance>:main|aux`、`BuildingInstanceId` 同实例、`FunctionSlotId` 不同 → 满足规格 §4/§8 的独立性断言。

### T2（次选，仅 Gateway 缝）

`ManagementCommandGateway` 增加测试/诊断重载（形如 `AssignToWorkplace(personId, workplaceId, BuildingRegistry definitions)`，2 参版委托并传 `null` 回落 `StaticConfigRegistry`），对齐 `ManagementReadAdapter.Read()` / `Read(BuildingRegistry)` 的既有双入口。问题：完工注册链仍读 `StaticConfigRegistry`，双 slot 不会被自动注册，仍需测试手工 `TryRegisterWorkplace`，"真实完工→注册"这一环不真实。**不推荐单用。**

### T3（产品内容，非测试构造）

Config Authority 侧在 `RRPG_Buildings` 正式内容给 farm 增加第二个 slot 并重建生成根。会新增玩家可见岗位 → 需制作人 / Config Authority 授权，且必须重跑受影响回归。**不是"补测试"，是改产品。**

### 明确回答"是否需要改 Authority / API"

| 方案 | 需要改 Authority / DTO / Save / 生产内容？ | 具体需要什么 |
|---|---|---|
| T1 | **不需要**（不改规则、DTO、存档、产品内容） | 需要主开发批准**新增 1 个 Runtime 测试缝 API**（已有同型先例） |
| T2 | **不需要** | 需要批准新增 Gateway 诊断重载；且需配合 T1 或手工注册 |
| T3 | **需要**（Config Authority + 产品决定） | 制作人 / Config Authority 授权 + 重建生成根 + 回归 |

---

## 8. 更正交付索引"main 必然需授权"的过强断言（本包只报告，不改文档）

需要更正的表述（均在既有文件里，本包**未修改**）：

| 文件 / 行 | 现表述 | 问题 |
|---|---|---|
| `reviewable-delivery-index.md:102` | 「配置只登记 `FunctionBindings: main`，`StaticConfigRegistry` 无公共注入点 → 未覆盖；**需配置 Authority / 公共 API 授权后另立包**」 | 把"内容事实（只有 main）"与"是否允许加测试缝"混成一个"授权"结论 |
| 同 `:134` | 「**第二 FunctionSlot**：仅在获得配置 Authority 或公共注册 API 授权后另立包」 | 同上，暗示非授权无解 |
| `final-current-evidence-review.md:250` | 「在"不改配置 Authority / 不改公共 API / 不用私有反射注入"的边界内**没有合法注入点**」 | 这句本身准确（三边界同时成立时确实没有）；但被索引引用后**丢掉了"同时成立"这一前提** |
| `final-current-evidence-review.md:291` | 「只有在获得配置 Authority 或公共注册 API 授权后才另立包」 | 同 `:134` |

**更正理由（逐条）：**

1. **两件事不是同一个授权面。**（a）"生产内容只有 `main`"是内容事实（`BuildingConfigDatabase.asset:36-47`）；（b）"测试能不能注入一个内存双 slot 注册表"是**代码可达性**问题。前者需要 Config Authority，后者只需要"是否允许新增一个测试缝 API"的**工程决定**——仓库已有同型先例（`PersonVisualBindingCatalog.SetInstanceForTests`、`SaveSystem.ForceSaveFailureForTests`）。把（b）也写成"必须授权"是把工程决定升级成了产品授权门槛。
2. **"必然"不成立。** T1 是单点、内存、可逆、与既有先例同型的改动，**不动 Authority / DTO / Save / 产品内容**，也不进入产品路径；它属于主开发可评估的技术选择，而不是必须先有"配置 Authority 授权"。
3. **原表述漏了真正的前置条件。** 准确口径应是："因 `_root` 无注入点且 Runtime 程序集无 `InternalsVisibleTo`，**在"不加任何代码缝 / 不改公共 API / 不反射写私有 Registry / 不改生产配置"这四条同时成立的约束下**才没有合法构造"；一旦允许 T1，该项即可真实覆盖。
4. **阻塞位置被表述得模糊。** 真实阻塞点是 `ManagementCommandGateway.cs:40-46` 的 `StaticConfigRegistry.TryGetBuildings` 固定依赖（叠加 `_root` 无注入点），**与 `SaveSystem` 单槽存档无关**——索引 `:102` 的"与 SaveSystem 单槽存档无关"这句是对的、应保留，但要避免读者把它误读成"存档槽导致未覆盖"。
5. **建议替代措辞**（不改文件，供主开发/后续同步包采用）：
   > 第二 FunctionSlot 真实 Gateway：**当前未覆盖**。生产配置只登记 `main`；`ManagementCommandGateway` 的 slot 注册校验固定读 `StaticConfigRegistry`（其 `_root` 仅由 `Resources.Load("RRPGStaticConfigRoot")` 赋值，无注入点，Runtime 程序集无 `InternalsVisibleTo`），故在"不加代码缝 / 不改公共 API / 不反射写私有 Registry / 不改生产配置"四条同时成立的约束下没有合法测试构造。**最小可行路径是新增一个 `SetInstanceForTests` 风格的 Runtime 测试缝（无需改 Authority/DTO/Save/产品内容）**，或由 Config Authority 增加正式第二 slot。详见 `Development/StudioRuns/R3B-UI-20261026/two-function-slot-scope-review.md`。

---

## 9. 保护面与边界（本包实际影响面）

- **只读接口**：`rql-query`（editor / scenes / console）、只读 `script-execute`（`File` 读写只用于读、`SHA256`、`EditorUtility.IsDirty`、`AssetDatabase.LoadAllAssetsAtPath`、目录 / manifest 统计、反射调 `TestRunnerApi.IsRunActive`）、本地 `read` / `glob` / `grep`。
- **未运行** `tests-run` / TestRunner（`IsRunActive=False` 为运行前实测，非运行后结论）；**未** `AssetDatabase.Refresh`（本包无代码改动，无需编译发现；也避免触发资产重导入）；**未** `rql-mutate`、**未** PrefabStage、**未** Shell / `Process.Start`、**未** Git 写、**未** Preferences、**未** 启停 Editor。
- **未** Save / `SetDirty` / `ClearDirty` / `SaveAssets` / `SaveChanges` / `SaveDirtyScenes`；**未** 重导入或探针字体（`GetPreferredValues` / `ForceMeshUpdate` 均未调用）。
- **未改** 代码 / 测试 / 断言 / Prefab / Scene / 配置 / 生成根 / Authority / DTO / 存档 / guard manifest / 34 项基线；未安装组件、未改权限、未创建额外付费子 Agent。
- **18 例保护**：`ManagementProjectionTests.cs` = `54062B / f48f8f7d…f16e0b`、`VillageStage1.Tests.dll` = `153088B / 9d09dabb…b69da`，与本包开始时**逐字节一致**；本包**未新增、未删除、未修改**任何用例，因此**不产生"新增 0 项"门禁**。
- 本包**唯一写入**：本文件。写后扫描披露：`UnityProject/RRPG1.0game` 下最近 15 分钟内 mtime 变化的文件**全部位于 `Library/` / `Temp/`（MCP 服务器日志、`SourceAssetDB`/`ArtifactDB` 等 Unity 自动库）与插件心跳 `.com-unity-codely.json`（300B）**，`Assets/` 下**零文件**变化。
- 结论须由**主开发专业核验**，不代表制作人批准；R3-B 状态不变（**Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**）。

---

## 10. 写后复核（只读）

复核时刻 **2026-10-26T10:44:07Z**（写入后只读重算）：

| 项 | 值（写后实测） |
|---|---|
| 本文件（写入后，本次编辑之前） | `26360B` / `d55b91852961bfca4dc2fc398027c157bd87e41a7e27ab754b431377107926b1`（mtime `2026-10-26T10:44:03.0731828Z`）；**本次 §10 补记后哈希以目录实际文件为准**，不据该旧值宣称未改 |
| 测试源码未变 | `54062B` / `f48f8f7d0cb9bfe299c3c8cbc1a6814540bd178495e632f907eedf21e9f16e0b` |
| 测试程序集未变 | `153088B` / `9d09dabbb127fc58235e271d1029c2b57ae2c155d9d3dc1fab08b86fa65b69da` |
| 字体未变 | `14961033B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、9 子资产 0 dirty |
| 单槽未变 | `13405B` / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、`.tmp=false` |
| 34 项 | `total=34 / same=34 / diff=0 / missing=0` |
| Editor | 仍 `isPlaying=False` / `isPaused=False` / `isCompiling=False` / `isUpdating=False`、场景 `dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False` |
| Console | 本包全程 `Error` / `Exception` 0 条（写前查 1 次；写后未再新增源码编译错误，因为**未改任何源码 / 未 Refresh**） |

> 本文件在 `2026-10-26T10:44:12Z` 时的版本：`26834B / 55112a6a857013d9aadd68b9bdad09be5777b2594c807e81cb221da98d3981dc`；此后仅追加本节复核文字与写后扫描披露，未改正文论断。最终哈希以目录实际文件为准，不据旧值宣称未改。

---

## 11. 下一步（不在本包执行）

1. **由主开发决定 T1 / T2 / T3 之一**，并据此派**唯一门禁**（本包不自行开测试、不造 0 项门禁）。
2. 若批 **T1**：另立包 = 新增 1 个 Runtime 测试缝（含 `Reset()` 清理）+ 新增 1 个 EditMode 用例 `ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`（真实 Gateway 派工 / 满员拒绝 / 撤职、两 slot 名额与命令独立、`WorkplaceId` / `BuildingInstanceId` / `FunctionSlotId` 真实独立、`Finally` 恢复 `StaticConfigRegistry` 与临时 `BuildingRegistry`），再按 §8 纪律串行 Compile → 专项 Edit → 受影响回归；原 18 例不改。
3. 若批 **T3**：走 Config Authority + 制作人，并另行评估生成根重建后的完整回归面。
4. 索引 §2 的更正措辞由后续文档同步包落地（本包不批量改文档）。
