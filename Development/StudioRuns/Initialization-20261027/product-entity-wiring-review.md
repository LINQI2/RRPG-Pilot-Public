# Initial World 产品实体接线 · 阶段 1/2 + 2/2 实施与核验记录

- 唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`
- Unity 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`（`2022.3.62t9`，`projectPath` 实测）
- 本阶段范围：**初始化实体展示、岗位与历史兼容接线**。不重做调查 / seed / Compiler / Builder 生成；不把本文件当作 Play 可玩验收或制作人批准。
- 执行方式：文件读写、Glob/Grep/Read、`rql-query` 只读、`script-execute`（安全 `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)`、只读保护面核验、一次非测试运行态冒烟）。
- **未使用**：Shell / Process / Git / `tests-run` / `rql-mutate` / `AssetDatabase.SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `ClearDirty` / 对字体或场景 `SetDirty` / PrefabStage / Preferences / 启停 Editor / 子 Agent / Workflow / Goal。
- 测试结论口径：本包**没有运行 Test Runner**；新增 5 例与既有 15 例均只做“编译后测试发现”，PASS/FAIL 必须以宿主 Studio 阶段回执为准。

---

## 1. 开工前置核验（真实值）

| 项 | 实测 | 结论 |
|---|---|---|
| CWD | `D:\AI_Agent_Lab\rrpg_pilot\repo` | ✅ 仅授权目录 |
| 编辑器 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | ✅ 空闲 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | ✅ 未污染 |
| 字体磁盘 | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与既有保护值一致 |
| 字体子资产 | `subassets=9 / dirty=0` | ✅ 无 pending 保存；未做字模 / UI 探针 |
| 用户当前档 | `Application.persistentDataPath/rrpg_save_p0.json`，`exists=true / 13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08` | ✅ 按当前实际值记录，未硬套旧 hash |
| Initialization guard | `Development/StudioRuns/Initialization-20261027/save-guard`：30 目录，全部 `restored`，无 unfinished / unknown | ✅ 新测试可安全 Capture |
| R3B guard | `Development/StudioRuns/R3B-SaveProtection-20261024`：310 目录 = **309 restored + 1 protective-snapshot**，0 other / 0 missing manifest | ✅ 与既有记录一致 |
| 34 项保护基线 | 读取 `baseline-r3b-20261022-current.json` 逐项复算：`entries=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |
| 生成链现状 | XLSX / 三份 `Initial*.json` / `InitialWorldConfigDatabase.asset` 已存在；默认 7 建筑、无 workshop | ✅ 本包无需改 XLSX，不手写 Mirror |

---

## 2. 精确变更面（本包真实落盘）

### 2.1 运行时代码

| 文件 | 字节 | SHA256 | 变更 |
|---|---:|---|---|
| `Assets/Scripts/Village/Runtime/ProductHomeSnapshot.cs` | 5833 | `32092e4dbe4fef5f1a14b1ebafddc80689b2895d309fe3108b0fd892e7f652d3` | `HomePlaceSnapshot` 新增 `BuildingInstanceId`、`DrawsWorldEntity`（构造参数追加在末尾，旧调用兼容）；同一 Building 身份可被地点投影引用而不重复绘制 |
| `Assets/Scripts/Village/ProductHome/ProductHomeReadAdapter.cs` | 7851 | `10b8f417b4bc78a546fa62bb04080f0b7a9853274882a7f4b4d79eeed4c0b066` | 建筑快照提前读取；注册 Workplace / Construction Place 若属于真实 Building → 记录 `BuildingInstanceId` 且 `DrawsWorldEntity=false`；`Home:<PersonId>` 在存在真实 Housing 实例时绑定对应 `BuildingInstanceId` 且不绘制第二套房屋；无住房时保留原“只读地点投影”路径 |
| `Assets/Scripts/Village/ProductHome/ProductHomeWorldHost.cs` | 15351 | `67c373945226824be22737e33f8b402f9154d0690557ef506ab4100f98d853e9` | 只为 `DrawsWorldEntity=true` 的 Place 创建 / 保留视图；所有 Place 仍解析锚点，Building-backed 别名锚定到同一 Building 坐标；同一实例不再出现“建筑 + Place”两套世界实体 |
| `Assets/Scripts/StaticConfig/BuildingRegistry.cs` | 3283 | `8ba51a3e6fd28f6c0eff167c41b1ec15d54814d88bd65048d6196afdfbf12c62` | 新增 `IsFormallyBuildable(id)`：`building_workshop` / `building_smithy` 为 false，其余历史定义仍可由 Load 读取 |
| `Assets/Scripts/Village/ProductHome/HomeBuildingPlacement.cs` | 11511 | `28922610fc9c61e8afea000571911864d1f8f5b93f44c2e02945fd8ca6295e01` | 正式 `Catalog` 过滤 workshop/smithy；`OpenCatalog` 只列正式目录；`Choose` 拒绝下线定义；`Confirm` 仍走 `TryPlaceBuilding`（同样拒绝） |
| `Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs` | 20501 | `11a07a145b906acd5383fe74ca398892f164252185acca466aec4c076b1a55ee` | `TryPrepareNewGameWorld`：新世界岗位基座改为空列表，资源岗位基座改为空列表，只登记配置建筑派生的实例岗位；`TryPlaceBuilding` 拒绝工坊 / 铁匠铺；`TryPrepareBuildingWorld` 改为“新会话完整静态兼容目录，同会话 Load 只补存档记录真实引用的静态定义” |
| `Assets/Scripts/Village/VillageRuntimeBridge.cs` | 75424 | `8b618df392692a91eb6b2e1167dca1c23ad566bd627a1403b035d51bfbb7cca5` | `EnsureInitializedWithExtraWorkplaces` 在运行时已建立时也允许显式追加调用方给出的旧岗位与对应资源 profile；不恢复默认全集，不改变正式 NewGame |

### 2.2 测试

| 文件 | 字节 | SHA256 | 变更 |
|---|---:|---|---|
| `Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` | 43869 | `afd65baf238968d2fd0db6bec2309151fe080c6af11fd51429428876218b1946` | ① `SaveFileGuard` 仅由 `private` 放宽为 `internal`，供新类复用（无断言修改）；② 第 7 例改为：新世界先用真实实例 Farm 派工 → Save → 在存档中真实改写为旧静态 `ws_farm_01` 并移除 Buildings → Load。仍强断言“旧档不补建筑、旧静态职责保留”，未削弱 |
| `Assets/Tests/EditMode/Village/ProductHomeFoundationTests.cs` | 4232 | `a6720cf7e5544f6ad6f789f9e648b7b3f905af7714b4199d78f4af93c209820e` | 旧用例原先假设 NewGame 后存在 `ws_farm_01`；改为读取真实 Farm 实例的 `workplace:<InstanceId>:main`，继续验证 Capacity / Assigned / 到场事实同源 |
| `Assets/Tests/EditMode/Initialization/InitialWorldProductWiringTests.cs` | 22268 | `86d26f314a69d2c7a233ec44672527e2350d7e7a508130e8aaf04eaa3a3c8e56` | 新增 5 例真实 Edit 类（见 §5） |
| 同上 `.meta` | 267 | `06fbb9b13d0aca5116717673904c36447169adf72404f2b99491799783a1d15a` | Unity 安全 Refresh 自动生成，未手写 |

未修改：`Assets/Generated/**`、`Assets/Resources/**`、场景 / Prefab / 字体 / ProjectSettings / Packages / Tools / XLSX / Canonical Mirror / SaveVersion / Save DTO。

---

## 3. 默认 / Legacy 边界（本包实际语义）

### 3.1 正式 NewGame（生成配置 → 真实实体）

1. `SaveSystem.NewGame()` → `InitialWorldConfigurationCatalog.TryReadDefault()` → 生成容器；默认 7 个建筑（1 Farm + 1 Kitchen + 5 House）来自现有 XLSX / Mirror / Builder，**本包未改 XLSX，未手写 Mirror**。
2. `TryPrepareNewGameWorld` 在候选 `BuildingRuntime` 上创建 **Completed** 实例；实例岗位复用既有 `BuildingFunctionBinding` + `VillageWorkplaceDef` profile（Capacity 单一权威）；实例资源岗位复用既有 `VillageResourceWorkplaceDef` profile。
3. 新世界岗位基座为**空列表**：不再注册 `ws_farm_01` / `ws_kitchen_01` / `ws_test_01` / `ws_test_02` 静态 Test 岗位，也不创建对应静态装饰 Place。
4. 新世界资源岗位基座为**空列表**：只登记配置建筑派生的 Farm / Kitchen 实例资源角色；静态 profile 只作为“实例复用的 profile 来源”保留。
5. Housing 定义没有 FunctionBinding，因此 House **不生成岗位、不加容量规则**；`TryAssignPerson(houseWorkplace)` 返回 `UnknownWorkplace`。
6. 默认人口 / 住房数量与坐标仍是 Test / Not Frozen；本包只把当前 5 房表现接成真实实体，不冻结数量比例。

### 3.2 Continue / Legacy Load

1. `Continue` 仍只走 `SaveSystem.Load`，**不重跑初始配置、不补初始建筑**。
2. 新会话加载旧档：岗位基座为完整既有静态兼容目录，旧静态职责可恢复。
3. 同会话 NewGame 后加载旧档：只从 `save.Assignments.Records` 的真实引用补充所需静态定义，不把整套静态岗位重新种入新世界。
4. 旧无 `Buildings` 档：`Buildings` 恢复为空，旧静态职责按记录恢复；新增测试覆盖。
5. `SaveVersion = 1`、`SaveData` / `VillageSaveData` / 任何 DTO 结构未改；初始配置不是 Save DTO 的一部分。

---

## 4. 显示身份去重与工坊正式下线

### 4.1 同一实体权威

- **实例 Farm / Kitchen**：`Assignments.WorkplaceDefs` 中只有 `workplace:<InstanceId>:main`；`ProductHomeSnapshot.Buildings` 与 `VillageRuntimeBridge.Buildings` 的 `InstanceId` / `DefinitionId` 逐项一致。
- **实例岗位 Place**：仍保留在 `ProductHomeSnapshot.Places` 中供管理 / 现场人数读取，但 `BuildingInstanceId` 指向真实建筑且 `DrawsWorldEntity=false`；World 视图只保留 Building 一套实体，不再画第二个 Place。
- **住房**：存在真实 Housing 实例时，`Home:<PersonId>` 投影绑定真实 Housing 的 `InstanceId` 且 `DrawsWorldEntity=false`；人物按顺序映射到住房实例（6 人 / 5 房时按序共用，不复制房屋实体）。无真实住房时才保留原合成锚点路径。
- **不引入假 Place**：所有去重后的 Place 仍来自真实 `VillageWorldRegistry` / 真实 Presence / 真实 Building 投影；没有手写坐标冒充实体，也没有第二套建筑或住房权威。
- **管理 / 派工 / 劳动 / Save / Continue**：`ManagementReadAdapter` 继续只从 `VillageRuntimeBridge.Buildings` + `Assignments` 读取 BuildingFunction 行；`SaveSystem.BuildSaveData` 保存同一 `BuildingRuntime`；`Load` 恢复同一实例身份。本包未新增第二套岗位或容量来源。

### 4.2 工坊 / 铁匠铺

- `BuildingRegistry.IsFormallyBuildable` 将 `building_workshop` / `building_smithy` 判为不可正式建造。
- 正式可达命令全部拒绝：Home `Catalog` 不列出、`Choose` 返回 false、`Confirm` 走 `TryPlaceBuilding` 返回 false 并给出“已从正式建造目录下线”诊断。
- 保留：BuildingDefinition、BuildingFunctionBinding、VillagePresentation 美术键、旧实例 `Load` 路径均未删除；测试用历史 Completed 工坊实例 Save → Load 仍恢复同一 InstanceId / 定义 / 状态。
- 旧实例恢复**不**重新开放正式建造命令。

---

## 5. 新增测试类（真实 Edit，不是自建显式配置绕过入口）

`RRPG.Initialization.Tests.InitialWorldProductWiringTests`，5 例：

1. `DefaultGeneratedNewGameCreatesSevenRealBuildingsWithoutDuplicateInitialization`
   - 直接 `SaveSystem.NewGame()` 默认入口；断言默认生成 7 实体、1 Farm / 1 Kitchen / 5 House、全部 Completed、实例 ID 唯一；重复 NewGame 不累加建筑 / 人口，实例 ID 从同一分配器重新开始。
2. `DefaultNewGameAssignsInstanceFarmKitchenAndHousingHasNoWorkplace`
   - 默认入口；Farm / Kitchen 实例岗位真实注册且可派工；Housing 无岗位定义、派工 `UnknownWorkplace`；实例资源角色复用既有 profile。
3. `NewWorldDropsStaticWorkplacesAndLegacySaveKeepsStaticAssignment`
   - 新世界无 `ws_*` 静态岗位 / 装饰 Place；用真实旧保存来源（静态岗位派工 Save，无 Buildings）Continue，断言不补建筑且静态职责与定义保留。
4. `WorkshopIsOfflineForFormalCommandsButLegacyInstanceStillLoads`
   - 正式 `TryPlaceBuilding` 拒绝工坊并给下线诊断；历史 Completed 工坊实例 Save → Load 仍恢复；旧实例恢复不重新开放正式命令。
5. `HomeSnapshotUsesSameBuildingIdentityWithoutDuplicateHousingOrInstanceWorkplaces`
   - `ProductHomeReadAdapter.Read()` 建筑身份与运行时逐项一致；实例岗位 / 住房别名均绑定真实 Building 且不绘制实体；5 个住房实例去重后最多 5 个视觉身份，6 个人物共用不复制房屋，无“Building + Place”双画。

存档保护：复用 `InitialWorldConfigurationTests.SaveFileGuard`。`[SetUp]` 第一行 `Capture()`（唯一目录 + 原始字节 CreateNew + 回读字节 / SHA 校验后才 captured），任何异常 / `TearDown` 的 `finally` 恢复并回读校验；存在未完成 run 时阻断新测试，不自动清记录、不覆盖他人快照。当前实测 30 个 guard run 全部 restored，用户档 13134 B / `adcf…`，新类尚未运行、未新增 run。

未削弱既有 15 例：10 例 `InitialWorldConfigurationTests` 断言全部保留（第 7 例强化为真实旧保存来源）；5 例 `InitialWorldConfigPipelineTests` 未改。

---

## 6. 编译与运行态冒烟（真实执行，非 tests-run）

| 检查 | 实测 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` → `refresh-requested` |
| 编译状态 | 刷新后 `isCompiling=true`；完成后复核 `isCompiling=false / isUpdating=false / isPlaying=false` |
| Console 新增 `Error` | `sequence > 86896` 的项目 / 测试程序集错误 = **0 条**；既有 86886–86896 均为本包 / 先前只读 snippet 的临时编译错误，未清洗 |
| 新类测试发现 | `tests | where fullName contains "InitialWorldProductWiringTests"` → **5 条 EditMode**（`InitialWorld.Tests`） |
| 既有 10 例发现 | `InitialWorldConfigurationTests` → **10 条 EditMode**；与 5 例 Pipeline 合计原 15 例可发现 |
| 场景 | `SampleScene isDirty=false` |
| 字体 / 用户档 | 刷新与冒烟后仍 `14961033 B / 3c758e16…c404854`、`subassets=9 / dirty=0`；用户档仍 `13134 B / adcf5d8b…8ceb08` |
| 34 项基线 | 刷新后复算 `34/34 same`；Initialization guard 30 restored；R3B 309 restored + 1 protective |

**非测试运行态冒烟**（`script-execute`，完成后 `finally VillageRuntimeBridge.ResetForTests()`；不写档、不跑 Test Runner、不测 UI / 字模）：

```text
buildings=7 farm=building_instance_1 kitchen=building_instance_2 house=building_instance_3 staticFarmDef=False assignFarm=Success assignKitchen=Success assignHouse=UnknownWorkplace snapshotBuildings=7 homeAliases=6 homeDrawn=0 workplaceAliases=2 snapshotSameIds=True
```

该冒烟只证明当前源码在 Editor 中可按默认入口创建真实 7 建筑、实例岗位可派工、Housing 无岗位、显示快照同实例且无重复房屋绘制；**不替代**宿主对 5+15 例的正式 check，也不构成 PlayMode / 真实 UI 验收。

---

## 7. 当前保护面

| 项 | 本包结束后实测 |
|---|---|
| 编辑器 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` |
| 场景 | `SampleScene isDirty=false rootCount=5` |
| 字体 | `14961033 B / 3c758e16…c404854`，9 子资产 `dirty=0`；未做字模测量 |
| 用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，未写档 |
| Initialization guard | 30 目录全部 restored，0 unfinished / unknown |
| R3B guard | 310 = 309 restored + 1 protective-snapshot，0 unfinished |
| 34 项基线 | `same=34 / changed=0 / missing=0` |
| 既有生成物 / XLSX / Mirror | 未修改、未手写 |
| SaveVersion / DTO | 未修改 |

---

## 8. 明确未验证 / 后续必须项

1. **未运行 `tests-run`**：新增 5 例与既有 15 例均待宿主 Studio 阶段 check；本文件不预填 PASS/FAIL。
2. **未做真实 Play / UI 验收**：未启动 Play，未验证 Home 世界、管理页、建造页、派工按钮、Continue UI 的最终表现；不宣称“可玩验收完成”。
3. **PlayMode 既有夹具需后续更新**：`ProductHomeFoundationPlayTests` 现存断言仍假设 `Home:<PersonId>` 有独立 Place 视图；`ManagementShellPlayTests` 仍假设“真实新世界没有建筑 / 建造候选点无重叠”。本包按新语义移除了重复 Place 视图并让默认 NewGame 生成真实建筑，后续 Play 包需按新实体权威更新这些夹具（本阶段未伪装 Play 通过）。
4. **UI 渲染 / 字模探针**：按约束未在 Editor 脚本冒烟；只能在宿主资源 Scope 内另行验证。
5. **默认数量 / 比例未冻结**：当前 7 建筑、5 住房、6 人物（5 角色 + 1 英雄）均为 Test / Not Frozen；后续改 XLSX 后由主开发固定生成链重新生成，本包不手写 Mirror。
6. **未新增住房岗位 / 容量规则**：Housing 仍无 FunctionBinding，不把住房变成工作岗位。

---

## 9. 边界声明

- 仅操作授权 Pilot 工程；未访问未授权工程、认证目录或其他工作区。
- 未安装组件、未改权限、未改计费渠道、未创建额外付费子 Agent。
- 未使用 `rql-mutate`、全局保存、场景保存、字体 / PrefabStage / Preferences / Git。
- 本文件与实现须由主开发专业核验；**不代表制作人批准**，也不表示初始化整包或可玩验收完成。

---

## 10. 阶段 2/2 追加：新类宿主真实结果 · 十例准备 · 剩余 Play

### 10.1 新类真实宿主结果（只读回看，非本包重跑）

来源：当前 Unity `testResults` 源只读查询（`rql-query`），最近一次 run 共 5 条，全部为 `InitialWorldProductWiringTests`：

```text
Passed  58ms  DefaultGeneratedNewGameCreatesSevenRealBuildingsWithoutDuplicateInitialization
Passed  15ms  DefaultNewGameAssignsInstanceFarmKitchenAndHousingHasNoWorkplace
Passed  13ms  HomeSnapshotUsesSameBuildingIdentityWithoutDuplicateHousingOrInstanceWorkplaces
Passed  59ms  NewWorldDropsStaticWorkplacesAndLegacySaveKeepsStaticAssignment
Passed  18ms  WorkshopIsOfflineForFormalCommandsButLegacyInstanceStillLoads
```

- 5/5 Passed，0 Failed、0 Skipped；`message` / `stackTrace` 均为 null。
- 本包**未调用 `tests-run`**；以上是宿主真实执行回执的只读回看，不是本包自跑。
- 因无失败项，本阶段**没有按 stack 做窄修**，也没有需要保留的失败记录；不伪造失败或修复前后对照。

### 10.2 新类运行后的保护面复核（真实值）

| 项 | 实测 |
|---|---|
| 用户档 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，未被改写 |
| Initialization guard | 35 目录 = **35 restored**，0 unfinished / unknown |
| 最新 5 个 guard run | 每个 `original-save.bin` 均 `13134 B`、`sha=adcf…8ceb08`，与当前用户档逐字节 `match=True` |
| R3B guard | 310 = **309 restored + 1 protective-snapshot**，0 unfinished |
| 字体 | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，9 子资产 `dirty=0` |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` |
| 场景 / 编辑器 | `SampleScene isDirty=false / rootCount=5`；`isPlaying=false / isCompiling=false / isUpdating=false` |
| Console 项目 / 测试程序集 Error | `sequence > 86896` = **0 条** |

### 10.3 原 `InitialWorldConfigurationTests` 十例准备

- 当前测试发现仍为 **10 条 EditMode**（`InitialWorld.Tests`），与 5 例新类合计 15 例可发现；未运行，结果待宿主 check。
- 第 7 例已按“正式 / legacy 分流”修订：新世界使用真实实例 Farm 派工，再在保存 JSON 中真实改写为旧静态 `ws_farm_01` 并移除 `Buildings`；Load 后断言 0 建筑与旧静态职责保留。没有把旧静态岗位重新种回新世界，也没有放松“不补建筑 / 保留职责”的断言。
- 其余 9 例逐项静态复核：显式配置仍只消费显式参数；Continue 仍保持实例身份 / 职责 / 资源；非法配置仍零半状态；Reset / EnsureInitialized 仍不种产品内容；Hero 替换仍不重复初始化。未发现需要进一步窄修的断言或产品路径。
- 没有修改 `InitialWorldConfigPipelineTests` 5 例；原 15 例未削弱。

### 10.4 剩余 Play / 真实 UI 工作（本包不冒充完成）

1. **运行十例**：由宿主 Studio 阶段 check 串行执行 `InitialWorldConfigurationTests` 10 例（建议连同 5 例 Pipeline 共 15 例）并给出新回执。
2. **PlayMode 夹具更新**：`ProductHomeFoundationPlayTests` 仍假设 `Home:<PersonId>` 有独立 Place 视图；`ManagementShellPlayTests` 仍假设“真实新世界没有建筑 / 建造候选点无重叠”。后续 Play 包需按真实 Building 实体与 `DrawsWorldEntity` 语义更新夹具。
3. **真实 Home / 管理 / 建造 / Continue UI**：未启动 Play；实例 Farm / Kitchen 的派工按钮、资源劳动面板、住房位置表现、Save / Continue 入口仍需真实 UI 验收。
4. **UI 渲染 / 字模探针**：只能在宿主资源 Scope 内测试；本包未在 Editor 脚本冒烟。
5. **Continue 的资源 profile 兼容边界**：`TryPrepareBuildingWorld` 仍以既有静态资源 profile 作为兼容基座，再追加存档建筑派生的实例资源岗位；这是旧档兼容与实例复用同一 profile 的保守边界，未来若要求“严格只按存档来源恢复资源 profile”，需另开窄包并以保存记录证明。
6. **默认数量 / 比例未冻结**：当前 7 建筑、5 住房、6 人物仍为 Test / Not Frozen；改 XLSX 后由主开发固定生成链重建，本包不手写 Mirror。

### 10.5 阶段 2/2 边界

- 未运行 `tests-run`、未启动 Play、未做 UI / 字模探针、未写用户档、未新增 guard run。
- 未修改产品源码 / 测试源码 / 断言 / 默认值 / Builder / Mirror / 生成资产 / 字体 / 34 项基线 / guard 历史。
- 未使用 `rql-mutate`、Shell、Process、Git、全局保存、场景保存、Preferences、PrefabStage；未启停 Editor；未创建额外付费子 Agent。
- 真实宿主结果与保护面由主开发专业核验；本文件不代表制作人批准，也不表示整包可玩验收完成。
