# Initial World 加载来源确定性与住房显示别名 · 阶段 1/3 实施与待测记录

- 唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`，本轮实测）。
- 本阶段范围：**修复 Load 的岗位 / 资源目录来源不确定性 + 住房显示别名契约**；强化 `InitialWorldProductWiringTests`。不重做实现、不改生成链、不改 Save DTO / SaveVersion、不新增住房容量规则、不删 profile。
- 执行方式：文件 Read/Edit/Write、Glob/Grep、`rql-query` 只读、`script-execute`（安全 `AssetDatabase.Refresh(ForceSynchronousImport)`、只读实测）。
- **未使用**：Shell / Process / Git / `tests-run` / `rql-mutate` / `AssetDatabase.SaveAssets` / `SaveChanges` / 全局保存 / `ClearDirty` / 字体或场景 `SetDirty` / PrefabStage / Preferences / 启停 Editor / 子 Agent / Workflow / Goal；未启动 Play。**没有重复上一包被明确禁止的“Editor script NewGame / assign 无保护业务冒烟”**：本轮唯一 `script-execute` 业务内容为零，只有 Refresh 与只读文件 / 资源测量。
- 测试口径：本包**未运行 Test Runner**。9 例新类仅做到“编译后测试发现”；PASS/FAIL 必须以宿主 Studio 阶段 check 的串行 Edit 回执为准。
- 本文件须由主开发专业核验，**不代表制作人批准**，也不表示整包初始化或可玩验收完成。

---

## 1. 开工前置核验（真实只读值）

| 项 | 实测 | 判定 |
|---|---|---|
| CWD | `D:\AI_Agent_Lab\rrpg_pilot\repo` | ✅ 仅授权目录 |
| 工程 / 版本 | `project=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`、`unity=2022.3.62t9` | ✅ |
| Editor | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false / selection=[]` | ✅ 空闲 |
| nativeRunner | `TestRunnerApi.IsRunActive()` 在该版本不存在（`script-execute` 编译证据 `CS0117`，未伪造读数）；仓库内也无 runner 锁 / 状态文件；`testResults` 未显示运行中 | ⚠ 只能以“Editor 空闲 + 本轮未启动 Runner + 宿主未派发 check”作为空闲证据 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`）、`isDirty=false`、`rootCount=5` | ✅ 未污染 |
| 字体磁盘 | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | ✅ 与保护值一致 |
| 字体子资产 | `9 / dirty=0` | ✅ 无 pending，未做字模 / UI 探针 |
| 用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08` | ✅ 按实际值记录 |
| Initialization guard | `45` 目录，全部 `status=restored`，`noManifest=0` | ✅ 无未完成 / 未知 |
| R3B guard | `310 = 309 restored + 1 protective-snapshot`，`noManifest=0` | ✅ 与既有记录一致 |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |

---

## 2. 精确差异（本轮真实落盘）

### 2.1 产品源码

| 文件 | 变更前 | 变更后 | 变更 |
|---|---:|---:|---|
| `Assets/Scripts/Village/VillageRuntimeBridge.Buildings.cs` | `20501 B / 11a07a145b906acd5383fe74ca398892f164252185acca466aec4c076b1a55ee` | **`23335 B / b2fdb4c86cab0b4874f31fa079f571002262c0a6fd0238a6366afbc3a87169d0`** | `TryPrepareBuildingWorld` 的岗位 / 资源基座改为**只从 SaveData 可证明来源**推导；新增 `IsInstanceWorkplace` / `ReferencedStaticWorkplaceIds` / `CopyResourceWorkplace`；更新 `TryPrepareNewGameWorld` 过期注释 |
| `Assets/Scripts/Village/ProductHome/ProductHomeReadAdapter.cs` | `7851 B / 10b8f417b4bc78a546fa62bb04080f0b7a9853274882a7f4b4d79eeed4c0b066` | **`10629 B / 8d1f849a09e4e1653051dd9988edbeef465adba7b18c624faa7261c5205a39d6`** | 住房锚点仅 Completed；稳定数值序号排序；`PersonId` 的 FNV-1a 确定性模映射；契约 / 局限注释；新增 `HousingInstanceOrder` / `CompareInstanceIds` / `TryInstanceOrdinal` / `StablePersonHash` |

### 2.2 测试

| 文件 | 变更前 | 变更后 | 变更 |
|---|---:|---:|---|
| `Assets/Tests/EditMode/Initialization/InitialWorldProductWiringTests.cs` | `22268 B / 86d26f314a69d2c7a233ec44672527e2350d7e7a508130e8aaf04eaa3a3c8e56` | **`39733 B / 54a57f9a29f624c1f11f8d405eee51886079a297da271d834bc491a06d77a2f2`** | 新增 4 例（总 9 例）；新增 `Work()` / `BridgeResources()` / `WorldStateSnapshot()` / `Building snapshot` 等只读辅助；第 5 例的“住房去重”断言改为符合新别名契约的“≤ 且 >0”（见 §4.4） |

未修改：`Assets/Generated/**`、`Assets/Resources/**`、Scene / Prefab / 字体 / ProjectSettings / Packages / Tools / XLSX / Canonical Mirror / SaveVersion / 任何 Save DTO / `HomeBuildingPlacement` / `ProductHomeSnapshot` / `ProductHomeWorldHost`。

---

## 3. 加载来源契约（本轮实现语义）

### 3.1 唯一来源判定

`TryPrepareBuildingWorld(VillageSaveData save, BuildingRuntime buildings, …)` 只看两样东西：

1. `save`（真实存档区块）；
2. `buildings`（`TryPrepareBuildings(save.Buildings)` 的已校验候选）。

`hasSavedBuildings = buildings.All.Count > 0`：

- **正式实例存档**（有真实 Building 实例）：静态岗位目录 = 存档中**真实出现过的非实例 WorkplaceId**；资源职责 = 这些被引用静态岗位对应的既有资源 profile + 存档建筑派生的实例资源岗位。
- **旧无 Buildings 存档**（0 实例，含缺区块与空列表两种真实来源）：保留历史完整静态兼容目录 + 默认资源职责（原旧档行为不变）。

“存档真实出现”的字段（`ReferencedStaticWorkplaceIds`）：

- `save.Assignments.Records[].WorkplaceId`（Primary Assignment 职责）；
- `save.IntradayWork.Batches[].WorkplaceId`（资源批次 / 未完成加工）；
- `save.IntradayWork.Evidence[].WorkplaceId`（当日劳务证据）。

明确**不再读取**：当前会话 `Assignments`、当前 `_resourceWorkplaceDefs`、`Assignments == null` 与否、上一次 NewGame 的目录。因此同一正式档在“冷启动（会话没有任何 Assignments）”与“热会话（会话先持有完整静态兼容目录）”Load 必须得到同一目录 / 资源 / World 快照。

### 3.2 保留的既有防御（未削弱）

- 实例岗位引用未知 → `Unknown runtime Workplace assignment` 拒绝 Load；
- 实例资源批次角色 / 输入付费状态非法 → 拒绝；
- 实例岗位容量超限 → 拒绝；
- 施工 Project 绑定 / 参与者 / 进度校验、IntradayWork 头与列表校验原样保留；
- 未知静态岗位引用仍按旧防御被 `VillageAssignmentSystem.Restore` 丢弃，不静默改写实例身份。

### 3.3 正式 NewGame 注释修正

`TryPrepareNewGameWorld` 的摘要此前仍写“保留既有默认静态岗位”，与实现（空静态基座）矛盾。本轮改为：静态岗位 / 静态资源基座为**空**，旧档需要的静态定义只由 `TryPrepareBuildingWorld` 按存档真实引用恢复。实现未改。

### 3.4 legacy 边界（必须与正式档区分）

| 场景 | Buildings | 静态岗位目录 | 资源职责 | 结果 |
|---|---|---|---|---|
| 正式 NewGame → Save → Load | 7 实例 | 仅存档引用（通常 0） | 实例 Farm/Kitchen 派生 | 不重种 `ws_*`；实例身份 / 职责 / 批次等值 |
| 旧静态世界（EnsureInitialized）派工 → Save（0 建筑） | 0 | 完整历史兼容目录 | 默认 `VillageTestResourceWorkplaces` | 旧职责 / 批次 / 兼容目录保留，不补建建筑 |
| 正式实例档 + 存档真实引用一个旧静态岗位 | ≥1 | 只恢复被引用者 | 被引用静态角色 + 实例角色 | 未引用静态岗位 / 资源 / Place 不凭空出现 |
| 同会话先 NewGame（实例目录）再 Load 旧无建筑档 | 存档 0 | 完整历史兼容目录 | 默认资源职责 | 与冷启动结果一致 |

---

## 4. 住房显示别名契约（纯显示，无持久 DTO 变更）

### 4.1 契约

1. **锚点只用合法 Completed 住房**：`LifecycleState==Completed` 且定义 `Category=="Housing"`；`UnderConstruction` 住房不进入锚点集合，也因此不会被任何 `Home:<PersonId>` 引用。
2. **锚点排序稳定**：按 `building_instance_<n>` 的**数值序号**升序（无数字后缀时退回 ordinal 字符串比较），不再按字符串排序（避免 `_10` 排在 `_2` 前）。新分配的更高序号住房追加到末尾，不重排既有锚点。
3. **人员 → 锚点**：`FNV-1a(PersonId UTF-16) % 锚点数`。只依赖**已存 PersonId 身份**，不依赖：
   - 人物列表解析顺序（`VillagePersonAvailability.Resolve` 顺序）；
   - 在村 / 离村 / 远征 / 暂不可用；
   - 职责、资源、当前 Presence；
   - 非住房建筑变化。
   Continue 恢复同一 PersonId + 同一 Completed 住房集合 → 同一别名。
4. **不使用** `string.GetHashCode()`（进程随机化），不新增持久字段 / Save DTO / 容量或人口比例规则，不删除任何 profile。

### 4.2 已记录局限（明确写进代码注释与本文件）

- 新增 / 完成一栋住房会改变锚点数量（模空间），部分人物的显示别名可能迁移；这是唯一迁移来源，且属“新住房真正可用”的有原因变化。
- 当前 SaveData 没有住房—人物持久绑定；要完全消除该迁移必须改持久 DTO，本包按约束**不做**。
- 因此选择“已存身份派生映射（PersonId 哈希）”而不是“显示索引取模”：后者会让排序 / 离村 / 任一新增建筑直接重排全部人物住房。

### 4.3 受影响显示路径

`ProductHomeReadAdapter.Read()` 生成的 `Home:<PersonId>` 投影：仍保留在 `Places` 供管理 / 人物位置语义；有 Completed 住房时 `BuildingInstanceId` 指向真实住房且 `DrawsWorldEntity=false`（不画第二套房）；无住房时才回退“只读地点投影”。`ProductHomeSnapshot` / `ProductHomeWorldHost` 未改。

### 4.4 第 5 例断言的窄修（非削弱）

原断言 `distinct(home 引用住房数) == min(homePlaces, housingIds)` 绑定旧“索引取模必然覆盖全部住房”的实现；新契约允许某栋 Completed 住房暂时无人。现改为：

- `usedHousing > 0`（有人物必须有真实 Completed 住房锚点）；
- `usedHousing <= housingIds.Count`（每栋住房最多被一个别名引用一次，不复制房屋实体）。

仍保留：Home 必须绑定真实 Housing 实例、不得指向非住房、`DrawsWorldEntity=false`、岗位 Place 同实例去重等全部强语义。

---

## 5. 测试强化（`InitialWorldProductWiringTests`，共 9 例 EditMode）

原 5 例保留（第 5 例仅按 §4.4 修正与新契约绑定的旧索引假设，未删断言）。新增 4 例：

6. `FormalSaveLoadsDeterministicInstanceCatalogueInColdAndWarmSessionWithProductionEvidence`
   - 默认 NewGame → 实例 Farm 派工 → 通过真实 `VillageIntradayWork` 域 API 生成可保存劳务证据 / 资源批次（不驱动 UI、不手改存档字段）→ Save；
   - **冷会话**（`ResetForTests` 后直接 Load）与**热会话**（先 `EnsureInitialized` 持有完整静态兼容目录再 Load）比较：建筑、岗位目录、资源职责、World Places、完整存档快照**逐项等值**；
   - 正式档不得出现 `ws_farm_01 / ws_kitchen_01 / ws_test_01 / ws_test_02` 的定义 / Place / 资源职责；
   - 实例职责、Ingredients、批次 / 劳务证据等值恢复（不重复生产）。
7. `LegacySaveWithoutBuildingsLoadsSameCompatibilityCatalogueColdAndWarm`
   - 真实旧兼容来源：显式兼容通道建立静态岗位世界 → 派工 `ws_farm_01` → 真实批次 / 证据 → Save（0 建筑）；
   - 冷 / 热会话（热会话先 NewGame 实例世界）Load 后比较完整快照等值；
   - 兼容目录（4 个静态岗位 + Place）、历史资源职责、批次 / 证据等值；0 建筑、不补建。
8. `FormalSaveRestoresOnlyLegacyWorkplacesItActuallyReferences`
   - 正式实例世界经显式兼容通道追加一个旧岗位定义并派工 → Save（Buildings 仍真实存在）；
   - Load 只恢复被引用的 `ws_farm_01`（定义 / 职责 / 资源角色 / Place），`ws_kitchen_01 / ws_test_01` 及其资源职责 / Place 不凭空出现；
   - 冷 / 热会话等值。
9. `HousingAliasesUseOnlyCompletedHousesAndStayStableAcrossOrderAndContinue`
   - 默认世界记录 `Home:<PersonId> → 住房 InstanceId`；通过真实 `TryPlaceBuilding` 建**未完成住房**：别名集合不变、该实例不被任何 Home 引用；
   - 反转真实人物列表顺序后别名不变；恢复顺序后仍不变；
   - Save → Load（Continue）后别名逐项不变；
   - 所有别名只指向 Completed 住房（未完成 / 非住房均排除）。

存档保护：4 个新测试全部沿用 `InitialWorldConfigurationTests.SaveFileGuard`（`[SetUp]` 先 Capture；异常 / TearDown 的 finally 恢复并回读 SHA；未完成 run 阻断；不自动恢复 unknown）。测试内的 Save 只写用户单槽，宿主运行后由 guard 恢复；本包未运行测试，故未新增 guard run。

---

## 6. 编译与发现证据（真实执行）

| 检查 | 实测 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` → `refresh-requested`（两次：测试新代码一次、断言窄修一次） |
| 编译状态 | 刷新后 `isCompiling` 由 true 回到 `false`；`isUpdating=false / isPlaying=false` |
| 编译错误 | 首次新增测试出现 2 条真实编译错误（`BuildingInstance` / `BuildingPlacementValidator` 不在 `InitialWorld.Tests` 的 asmdef 引用内，属 Assembly-CSharp 类型），已按真实 asmdef 边界改为只用公开桥入口 + 反射；最终 Console `Error` = **0** |
| 测试发现 | `tests | where fullName contains "InitialWorldProductWiringTests"` → **9 条 EditMode**（原 5 + 新 4）；`InitialWorldConfigurationTests` + `InitialWorldConfigPipelineTests` → **15 条 EditMode** 仍在 |
| Console | 当前 `logType==Error` 查询 **0 条** |
| 场景 | `SampleScene isDirty=false / rootCount=5` |
| 字体 / 用户档 | 刷新与编译后仍 `14961033 B / 3c758e16…c404854`、`9 子资产 dirty=0`；用户档仍 `13134 B / adcf5d8b…8ceb08` |
| guard / 基线 | Initialization `45/45 restored`；R3B `309 restored + 1 protective`；34 项 `34/34 same` |

**未运行 `tests-run`、未启动 Play、未执行任何 NewGame / assign / Advance 业务冒烟**；以上 PASS/FAIL 留空，交宿主 Studio 阶段 check。

---

## 7. 副作用与保护面

| 项 | 状态 |
|---|---|
| 用户单槽 / 存档 DTO / SaveVersion | 未改（测试由宿主运行时 guard 保护，本包未跑测试） |
| 生成资产 / XLSX / Mirror / Registry / Builder / Compiler | 未改、未手写 |
| Scene / Prefab / 字体 / ProjectSettings / Packages | 未改；安全 Refresh 后 `dirty=0` |
| 新增持久数据 | 无（住房映射只存在于 `ProductHomeSnapshot` 瞬时投影） |
| 新增 UI / 字模探针 | 无（按约束本阶段不涉及） |
| Editor / Runner | 未启停；未调用 Test Runner |
| `rql-mutate` / 全局 Save / PrefabStage / Git | 未使用 |
| 付费子 Agent / Workflow / Goal / 安装 / 权限 / 计费 | 未创建 / 未修改 |

---

## 8. 待测与后续阶段（不在本阶段冒充完成）

1. **宿主串行 Edit check**：短类名 `InitialWorldProductWiringTests` 整类串行执行 **9 例**（不做方法级拆分、不并行），并回填真实 PASS/FAIL / skipped / 时长 / scope。重点观察：
   - 冷 / 热会话快照等值是否成立；
   - 真实 `TryPlaceBuilding` 在默认布局中找到合法空位（测试会遍历网格直到成功；若全部失败会以明确消息失败，不改默认世界）；
   - 第 5 例的“≤ 且 >0”住房去重断言。
2. **受影响 Play 夹具（阶段 2/3，本阶段未改）**：
   - `ProductHomeFoundationPlayTests` 仍假设 `Home:<PersonId>` 有**独立 Place 视图**、并直接派工 `ws_farm_01`；新契约下 Home 别名绑定真实 Completed 住房且 `DrawsWorldEntity=false`，默认世界也不再注册静态 `ws_farm_01`。必须在后续阶段按“实例岗位 + 真实住房别名 + 真实生成配置”更新夹具，保留原派工 / 计数 / 恢复断言；
   - `ManagementShellPlayTests` 仍含“旧空世界 / 独立 Home Place 视图 / 静态 `ws_farm_01` 行”假设，需按新实例岗位与真实建筑更新；
   - `BuildingConstructionProductPlayTests` / `IntradayWorkProductPlayTests` / `VillageV4ResourceSurplusPlayTests` / `IntradayWorkIntegrationTests` 等直接使用 `ws_farm_01` 的 Play / Edit 夹具需按“显式测试配置或真实实例岗位”分别处理，不得把默认世界改回 0 建筑。
3. **新增 `InitializationProductPlayTests`（后续阶段）**：真实主菜单 / 英雄 / 进入 Home 默认入口、真实 top-hit 按钮、截图唯一时间戳、Save/Continue 不重复生产、再次进村不重种、住房无岗位、正式工坊 Catalog / Choose 拒绝等，尚未创建。
4. **默认数值**：7 建筑 / 5 住房 / 6 人物仍为 Test / Not Frozen；本包只保证来源确定性，不冻结比例。
5. **真实验收**：未做 UI / 世界渲染验收；未声称“可玩验收完成”。

---

## 9. 阶段 1/3 续跑：两例 NRE 定位与最小修复（本文件追加）

> 本节接续宿主失败回执 `dsh:568f51ffee2873fde5892f1d62be73736a24d7eaee46fdb5f416bb45cf79ea16`；
> 宿主实测：9 总 / 7 pass / 2 fail / 0 skip / 1.5346358 s，scope `restored|font|0|632`。
> **未重做已实现产品改动**；只修测试反射访问，并在本文件记录真实根因。

### 9.1 真实失败栈（来自当前 `testResults`，只读获取）

| 用例 | 状态 | 时长 | 消息 |
|---|---|---:|---|
| `FormalSaveLoadsDeterministicInstanceCatalogueInColdAndWarmSessionWithProductionEvidence` | Failed | 19 ms | `System.NullReferenceException` |
| `LegacySaveWithoutBuildingsLoadsSameCompatibilityCatalogueColdAndWarm` | Failed | 12 ms | `System.NullReferenceException` |

两条栈完全相同，落点均为测试辅助 `Work()`：

```text
System.NullReferenceException : Object reference not set to an instance of an object
  at RRPG.Initialization.Tests.InitialWorldProductWiringTests.Work ()
      InitialWorldProductWiringTests.cs:522
  at ...FormalSave... ()  InitialWorldProductWiringTests.cs:316
  at ...LegacySave... ()  InitialWorldProductWiringTests.cs:377
```

即：`FormalSave...` 在 `Work().TryBeginBatch(...)`（原 316 行）、`LegacySave...` 在
`Work().TryBeginBatch(...)`（原 377 行）调用前，`Work()` 自身先 NRE；不是产品 Save / Load / 资源逻辑的异常。

### 9.2 成员形态核对（`script-execute` 只读反射，非猜测）

| 成员 | 实际形态（Editor 2022.3.62t9 实测） | 旧测试访问 | 结果 |
|---|---|---|---|
| `VillageRuntimeBridge.IntradayWork` | **静态属性** `public static VillageIntradayWork IntradayWork { get; private set; }`（`VillageRuntimeBridge.Work.cs:7`），`CanRead=True` | `GetField("IntradayWork")` | 返回 `null` → `.GetValue(null)` NRE |
| `VillageRuntimeBridge.Resources` | **静态字段** `public static VillageResourceState Resources;`（`VillageRuntimeBridge.cs:140`） | `GetField("Resources")` | 正常 |

同一反射实测：`GetField("IntradayWork")=null`、`GetProperty("IntradayWork")=VillageIntradayWork`；
`GetField("Resources")=VillageResourceState`、`GetProperty("Resources")=null`。
`VillageIntradayWork` 的 `TryBeginBatch / Work / ProcessBatch / ToSaveData` 均为真实公开实例方法。

### 9.3 根因（只按真实栈归因）

- **直接根因**：测试辅助 `Work()` 把静态属性当作字段，`BridgeType.GetField("IntradayWork", …)` 返回 `null`，
  随后 `.GetValue(null)` 抛 NRE；两例都先使用 `Work()` 才触达产品逻辑，因此产品代码未被执行到。
- **不是已证根因**：`Resources` 字段访问本身正确，Save DTO / Load 目录 / 资源数量 / 住房别名均未在本栈中报错；
  本次没有产品侧 Narrow fix，因为没有产品异常栈可归因。

### 9.4 最小修复（仅测试辅助；断言与产品代码零改动）

`Assets/Tests/EditMode/Initialization/InitialWorldProductWiringTests.cs`：

- `Work()`：改为 `BridgeType.GetProperty("IntradayWork", Static)`，并新增两条明确断言——
  成员必须作为可读静态属性存在、运行时值必须非 null；
- `BridgeResources()`：保留字段访问（这是真实形态），同样新增成员存在 + 运行时非 null 断言；
- 成员被重命名 / 改形态时，失败信息为明确断言文本，不再退化成空引用。
- 未删用例、未跳过断言、未放宽生产证据 / 资源数量 / 冷热同档等值 / legacy 不补建筑等任何强断言；
- 未改 `Assets/Scripts/**`、`Assets/Generated/**`、Save DTO / SaveVersion、Scene / Prefab / 字体 / ProjectSettings。

| 文件 | 修复前 | 修复后 |
|---|---|---|
| `InitialWorldProductWiringTests.cs` | `39733 B / 54a57f9a29f624c1f11f8d405eee51886079a297da271d834bc491a06d77a2f2` | **`40921 B / 96890cb8b8dd247432ba3a1667d9c05494d73d3225f0091d1d5290cc61fb9e16`** |

### 9.5 编译 / 发现 / 前置核验（本轮真实执行）

| 检查 | 实测 |
|---|---|
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` → `refresh-requested`；域重载后 Editor `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` |
| Console | `logType==Error/Exception` → **0 条** |
| 测试发现 | `tests | where fullName contains "InitialWorldProductWiringTests"` → **9 条 EditMode**（与宿主 9 例一致，无 skip 标记） |
| 工程 / 版本 | `project=.../UnityProject/RRPG1.0game/Assets`、`unity=2022.3.62t9` |
| 字体磁盘 | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、子资产 `9 / dirty=0` |
| 用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`（未变） |
| Initialization guard | `54` 目录，`54 restored / 0 failed / 0 other`，无未完成 run |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` |
| 场景 | `SampleScene` 未保存、未变更（本轮无场景写入） |

**本轮仍未运行 Test Runner**（禁止 `tests-run`），未启动 Play，未做 Editor script NewGame / 派工 / 渲染冒烟。
因此九例 PASS/FAIL **不预填**，以宿主 Studio 阶段 check 的完整 `InitialWorldProductWiringTests` 串行 Edit 回执为准；
本阶段门禁以宿主“非 0 且 0 skip”九例回执为唯一通过依据。

**宿主阶段 1 回执（真实，2026-10-28）**：`true` → `InitialWorldProductWiringTests` **9 / 9 Passed / 0 Failed / 0 Skipped**
（`testResults | group-by status`：`Passed=9`）。失败记录见 §9.1，修复与保护面见 §9.2–§9.4。

### 9.6 若宿主仍失败时的窄修边界（留给门禁回执）

- 只按宿主新回执中的真实异常栈定位；若仍在测试反射 / 夹具，改测试；若落在产品 Save / Load 目录或资源恢复，
  才按栈做产品窄修；
- 不放宽 §5 已列强断言（生产证据、Ingredients 等值、冷/热快照等值、legacy 目录保留、0 补建、实例住房别名）。

---

## 10. 阶段 2/3：默认产品入口与保存继续 Play 专项（本轮实施）

### 10.1 本阶段新增 / 更新文件

| 文件 | 变更 | 字节 / SHA256 |
|---|---|---|
| `Assets/Tests/PlayMode/InitializationProductPlayTests.cs` | **新增**（1 个完整产品路径 Play 用例） | `48236 B / 13f1b3c5d041dc852996ea6150d3e915289bd31537ffe574ba19a5c8dd60fec9` |
| `Assets/Tests/PlayMode/Village/ProductHomeFoundationPlayTests.cs` | 按实例建筑 / 实例岗位 / Home 投影契约重写 3 例夹具（方法名保留） | `24401 B / 2448f2009506fbcbf5a15525cb5327d59e989609de45f327cb537b5b79c6bb1e` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 17 方法名 / 业务与恢复断言保留；CASE-B、首次导航、建筑行滚动、建造候选 4 处夹具按正式默认实例世界更新 | `500853 B / 0293c882cf1928088e4213ad224339dd00c266e515c3f5bd1354c8b04dd8ca6e` |

未改 `Assets/Scripts/**`、`Assets/Generated/**`、Save DTO / SaveVersion、Scene / Prefab / 字体 / ProjectSettings。

### 10.2 `InitializationProductPlayTests` 覆盖（真实产品路径，不绕过生成配置）

唯一用例 `DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed`：

1. **真实默认入口**：`SampleScene → MainMenuUI.btnNewGame` 真实 `EventSystem` 点击 → `HeroSelectionUI` 真实英雄卡 +
   真实确认按钮点击 → `SaveSystem.NewGame()` 读取 `InitialWorldConfigurationCatalog.TryReadDefault()`；
   读取的配置建筑数 / 人口数与运行时逐项比对（数量来自真实配置，不硬编码 7/5/6）。
2. **实例身份 / 住房无岗位**：运行时建筑 Completed、InstanceId 唯一且与配置定义同源；住房实例无
   `workplace:<id>:main` 定义 / Place / 资源职责；正式新世界无 `ws_farm_01 / ws_kitchen_01 / ws_test_01 / ws_test_02`。
3. **管理真实选人派工**：底部导航 `Navigate2` 真实点击 → 人口行真实点击 → 职责页签真实点击 →
   实例 Farm 岗位行真实点击 → 派工按钮真实点击；断言职责写入实例 `workplace:<FarmInstance>:main` 且状态含“已绑定”。
4. **资源劳动**：用既有公开注入点 `ConfigureActivitySchedule` 仅隔离本用例时间跨度活动机会（不改产品规则），
   真实 `Clock.AdvanceTo` 到工作区间内，断言 Ingredients 增加且存在该人物 / 该实例岗位的劳务证据；
   不直接写 Work / Resources。
5. **Save / Continue**：管理页 `saveButton` 真实点击 → 真实单槽写入 → 场景重建 → `MainMenuUI.btnContinue`
   真实 `EventSystem` 点击 → 新宿主；建筑 InstanceId 集合、实例职责、Ingredients / Food、批次 / 证据逐项等值。
6. **再次进村不重种**：Continue 后无静态岗位 / Place / 资源职责重种，住房仍无岗位，资源 / 批次 / 证据不重复生产。
7. **工坊 Catalog / Choose 拒绝**：`Navigate6` 真实打开建造；正式 Catalog 不含 `building_workshop / building_smithy`、
   不渲染工坊选择按钮；`Build.Choose("building_workshop")` 与 `VillageRuntimeBridge.TryPlaceBuilding` 均返回 false
   且诊断含“下线”。
8. **唯一截图 + 日志**：用例内只调用一次 `ScreenCapture`，UTC 时间戳 + 冲突后缀，绝不复用历史文件名；
   路径 `Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-save-continue-<UTC>.png`；
   batchmode / 无 Game View 时只记录 `SHOT-UNAVAILABLE`，不伪造。
   日志 `Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-play-evidence.log`
   （CONFIG / ENTRY / WIRING / MANAGEMENT / LABOR / SAVE / CONTINUE / WORKSHOP-REJECT / END / FONT）。

存档保护：`SaveSlotGuard` 与既有持久协议同源（先捕获当前用户单槽字节 + SHA256，备份用 `CreateNew`，
存在未完成 run 阻断；TearDown 的 finally 只按本 run 已验备份恢复并回读校验）。根目录为
`Development/StudioRuns/Initialization-20261027/save-guard`，不使用其它保护根。

### 10.3 受影响夹具更新（保留原方法名与业务 / 恢复断言）

- `ProductHomeFoundationPlayTests`（3 例原名保留）：
  - `StableEntity_AwayReturnAndSimulationGating`：派工改为实例 Farm；Home 不再创建独立 Place 视图，改为断言
    Home 投影 `BuildingInstanceId` 指向真实 Completed 住房且 `DrawsWorldEntity=false`，住房 / Farm 视图稳定；
  - `Traveling_DoesNotArriveOrAdvanceClock_ThenUsesActualArrival`：Kitchen / Farm 全部使用实例岗位；
   目的视图改用真实 Building 视图，保留“表现步进不改事实 / 到场只更新目标 / 不瞬移”全部断言；
  - `CameraAndSelectionNotSaved_LoadRebuildsFromExistingAuthority`：选择改为真实 Building 身份；
    相机 / 选择不入档、Save→Load 恢复实例职责与建筑的全部断言保留；
  - 存档：新增持久 SaveSlotGuard（Setup 先捕获；TearDown 的 finally 恢复并回读），不再直接删除用户档。
- `ManagementShellPlayTests`（17 例方法名与业务 / 恢复断言全部保留）：
  - `ManagementConstructionCommands...`（CASE-B）：正式默认世界只有实例岗位，因此通过既有公开兼容通道
    `EnsureInitializedWithExtraWorkplaces` **显式追加**静态 `ws_farm_01`（测试专用），保留“LegacyCompatibility 行 /
    派工禁用 / 撤职可用 / 施工不改写 Primary Assignment”全部断言；没有把正式默认世界改回 0 建筑；
  - `FirstNavigationAfterSourceReplacement...`：旧候选岗位改用当前正式世界的真实 Completed Farm 实例岗位；
    “来源切换清候选 / 首次导航不被复位吞掉”断言不变；
  - `ManagementScroll_RealBuildingRows...`：计数改为“开工真实基线岗位 / 资源行 + 新增 6 座 Completed Farm”，
    不再假设空世界；滚动 / 末行可达 / detail 逐行核对断言保留；
  - `ManagementEscape_FramePolling...`：建造候选手工起点从已与默认住房重叠的 `(0,4)` 改为空位 `(0,-6)`，
    仍先建立合法候选再做真实帧阴性对照；不固定候选值。

### 10.4 编译 / 发现 / 前置核验（本轮真实执行）

| 检查 | 实测 |
|---|---|
| 安全刷新 / 重编译 | `AssetDatabase.ImportAsset(...ForceUpdate|ForceSynchronousImport)` × 3 + `CompilationPipeline.RequestScriptCompilation()`；域重载后 `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` |
| Console | `logType==Error/Exception` → **0 条** |
| 测试发现 | 新类 **1 条 PlayMode**；`ProductHomeFoundationPlayTests` **3 条**（原名）；`ManagementShellPlayTests` **17 条**（原名）；合计 **21 条**，无 skip / ignore |
| 宿主 Edit 回执 | `InitialWorldProductWiringTests` **9/9 Passed**（阶段 1 门禁已过） |
| 字体磁盘 | `14961033 B / 3c758e16…c404854`，子资产 `9 / dirty=0` |
| 用户单槽 | `13134 B / adcf5d8b…8ceb08`（未变） |
| Initialization guard | `63` 目录：`63 restored / 0 failed / 0 other`，无未完成 run |
| 34 项基线 | `entries=34 / same=34 / changed=0 / missing=0` |
| 场景 | `SampleScene isDirty=false / rootCount=5` |

**本轮未运行 `tests-run`、未启动 Play、未用 Editor script 做 NewGame / 派工 / 渲染冒烟**；Play PASS/FAIL **不预填**，
以宿主 Studio 阶段 check 的 `InitializationProductPlayTests` + 两个受影响类串行 Play 回执为准。
截图 / 日志只有在宿主真实执行 Play 时才会产生；本文件不宣称已生成。

### 10.5 已知边界（不冒称）

- 程序化 `EventSystem` 点击不是物理鼠标人工体验；拖拽 / 指针手感仍留人工验收；
- 新类为单一端到端用例：通过即同时覆盖入口 / 管理派工 / 劳动 / SaveContinue / 不重种 / 住房 / 工坊拒绝；
  失败时按宿主真实 stack 窄修，不拆分删除断言；
- 截图在 batchmode 可能不可用；此时日志明确记录 unavailable，不伪造产物。

---

## 11. 阶段 1/3 续跑：产品默认入口专项（真实 0 匹配定位 + 接续口径）

> 本节接续宿主拒绝回执 `0483b34b-b2b6-4e75-a4d5-b452cce0912e`：
> `No tests found matching class 'InitializationProductPlayTests'`。
> **该回执是过滤零匹配，不是断言失败，也不是产品 / 测试逻辑异常；原记录保留，不删除、不改写。**
> 上一真实门禁 `d10f7d0d`（`InitialWorldProductWiringTests` **9/9 Passed / 0 skipped / 1.5906853 s / scope font0|632**）继续有效，本节不重跑、不改写该结论。
> 本节**没有重做整包**，没有改产品代码、没有改测试代码、没有放宽任何断言，也**没有运行 `tests-run`**。

### 11.1 开工只读复核（本轮实测，`rql-query` + `script-execute` 只读）

| 项 | 实测值（2026-10-28T11:42–11:45Z） | 判定 |
|---|---|---|
| 授权目录 / 工程 | `repo=D:\AI_Agent_Lab\rrpg_pilot\repo`；`project=...\UnityProject\RRPG1.0game`；`unity=2022.3.62t9` | ✅ 仅授权 Pilot |
| Editor 空闲 | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False`、`isPlayingOrWillChangePlaymode=False` | ✅ |
| 原生 TestRunner 空闲 | `TestRunnerApi.IsRunActive()`（`Static\|NonPublic`，与 `StudioTestResourceScope.End()` 同源反射）实测 **`False`**；插件侧 `Tool_Tests.HasPendingTestRun()=False` | ✅ 无运行中 / 无未完成 run |
| 字体资源隔离 scope | `SessionState.GetBool("Studio.Pilot.FontTestScope")=False`、`BlockedWrites=0` | ✅ 上一轮 scope 已正常 End |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`）`isDirty=False / rootCount=5` | ✅ 未污染 |
| Console | `logType==Error / Exception` → **0 条** | ✅ 编译干净 |
| 受保护字体（磁盘） | `14961033 B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，mtime `2026-10-10T13:23:55Z` | ✅ 与契约常量 / 34 项一致 |
| 字体子资产 | `9` 个，`EditorUtility.IsDirty` → **0 dirty** | ✅ |
| 34 项 workspace 基线 | `Development/StudioRuns/R3B-20261022/workspace-baseline.json`（`6866 B / bc7216f1…394e`）逐项 bytes+SHA256：`entries=34 / same=34 / changed=0 / missing=0` | ✅ 全一致 |
| 当前用户单槽 | `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`、`.tmp=False` | ✅ 按实际值记录，未硬套历史档 |
| 当前档 guard | `Development/StudioRuns/Initialization-20261027/save-guard`：**63 目录 = 63 restored + 0 protective + 0 noManifest + 0 unfinished** | ✅ 无未完成 run，不阻断新 run |

### 11.2 测试发现（`tests` 源实际字段，非源码反射推断）

| fullName | mode | assembly | fixture | namespace | ignored / explicit |
|---|---|---|---|---|---|
| `InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed` | `PlayMode` | `BattleStage1.PlayModeTests` | `InitializationProductPlayTests` | `""`（全局命名空间） | `false / false` |

- 该 fixture 在 `tests` 源中**仅 1 条**；`ProductHomeFoundationPlayTests` = **3 条**、`ManagementShellPlayTests` = **17 条**（合计 21 条，均 PlayMode、无 ignore）。
- 已加载程序集反射复核（`BattleStage1.PlayModeTests.dll`）：
  - `type FullName=[InitializationProductPlayTests]`、`Namespace=[<null>]`、`IsPublic=True`、`DeclaringType=<null>`；
  - `method FullName=[InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed]`；
  - 特性 = `IteratorStateMachineAttribute` + **`UnityEngine.TestTools.UnityTestAttribute`**；返回 `IEnumerator`、0 参数；类内 `[UnityTest]` 方法总数 = 1；
  - 生命周期成员实际形态：`OneTimeSetUp` ×1、`TearDown` ×1、`OneTimeTearDown` ×1（`UnitySetUp` 为 `SetUp`）。
- **方法名与任务给定名称逐字一致**（含大小写与下划线），无拼写偏差。

### 11.3 短类名 `testClass` 过滤 0 匹配：真实原因（工具实现级证据）

宿主拒绝原文是 `No tests found matching class 'InitializationProductPlayTests'`。本轮直接读取**当前加载的宿主工具程序集**
`Library/ScriptAssemblies/com.IvanMurzak.Unity.MCP.Editor.dll`（只读）并检查 `com.IvanMurzak.Unity.MCP.Editor.API.Tool_Tests` 的 IL：

- 类过滤正则构造方式（`Tool_Tests.CreateClassRegexPattern` IL 常量）：
  `"^.*\." + Regex.Escape(testClass) + "\.[^\.]+$"`；
- 该正则匹配对象是 NUnit 适配器的 `test.FullName`（`Tool_Tests.CountFilteredTests` IL 中 `get_FullName` → `Regex::IsMatch`）；
- `testMethod` 走的是另一条路径：`test.FullName.Equals(testMethod)`（序数比较，`Tool_Tests.CountFilteredTests` IL 证实）。

**用同一正则对真实 FullName 复算（本轮实测）**：

| 输入 | 正则 | 结果 |
|---|---|---|
| `InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed` | `^.*\.InitializationProductPlayTests\.[^\.]+$` | **`False` → 0 匹配** |
| `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_...`（对照，隶属 `RRPG.ManagementShell.Tests`） | `^.*\.ManagementShellPlayTests\.[^\.]+$` | `True`（历史上同类过滤可用） |

**根因**：该工具类过滤要求类名前**至少还有一个命名空间段（前面必须有一个 `.`）**。
本类按主开发要求位于**全局命名空间**，`FullName` 直接从类名开始（`InitializationProductPlayTests.DefaultProductEntry_...`），
因此 `^.*\.` 无法成立 → 必然 0 匹配。这与“测试未被发现 / DLL 里没有类 / 断言失败”无关，
`tests` 源与程序集反射都证明该类与方法真实存在。

**因此本节不采取的做法**（避免冲突与假通过）：
- 不把产品代码 / 断言改弱，不改 `testMode` 之外的过滤语义；
- **不给测试类加 namespace**：那会把 FullName 变成 `RRPG.X.InitializationProductPlayTests.…`，
  与预设 / 拒绝回执中的方法全名 `InitializationProductPlayTests.DefaultProductEntry_…` **字面冲突**，
  按任务约束“与预设 method 冲突则先停交报告，不盲测 0 项”，本轮停止在“确认 + 交方法过滤口径”。

**接续口径（下阶段宿主 check 唯一允许的过滤）**：
`testMode=PlayMode` + `testMethod=InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed`
（`FullName.Equals` 实测 `True`）；**不得**再用 `testClass=InitializationProductPlayTests`，也不得用短方法名。

### 11.4 编译 / DLL 新鲜度（本轮实际测量与内容级验证）

| 项 | 实测 |
|---|---|
| 源码 `Assets/Tests/PlayMode/InitializationProductPlayTests.cs` | `48236 B / 13f1b3c5d041dc852996ea6150d3e915289bd31537ffe574ba19a5c8dd60fec9`（与 §10.1 同值，本轮未改内容） |
| 源码 `ProductHomeFoundationPlayTests.cs` | `24401 B / 2448f200…bb1e`（同 §10.1） |
| 源码 `ManagementShellPlayTests.cs` | `500853 B / 0293c882…8ca6e`（同 §10.1） |
| `BattleStage1.PlayModeTests.dll` | `607232 B / 0cdabfb9a03f92d7e0277e27493d8c7dd83133c8bbbb6a947871425b0332efdb`、mtime `2026-10-28T11:39:08Z` |
| `Assembly-CSharp.dll` | `661504 B / 15c14b0b…a227`、mtime `2026-10-28T11:21:16Z` |
| `InitialWorld.Tests.dll` | `68096 B / 2d211c4a…51a7b`、mtime `2026-10-28T11:25:54Z` |

- **mtime 事实（如实披露，不用 mtime 冒充新鲜）**：三份源码写入时刻 `11:39:37Z`（本轮安全 `ImportAsset` 后为 `11:43:03Z`），
  **晚于** DLL 的 `11:39:08Z`。因此本轮不把“DLL mtime 更新”当作新鲜度证据，改用**内容级**验证：
  1. 对三份源码执行 `AssetDatabase.ImportAsset(ForceUpdate|ForceSynchronousImport)` + `CompilationPipeline.RequestScriptCompilation()`（**仅此安全刷新**，未 `SaveAssets` / `SaveChanges` / `ClearDirty`）；
  2. 刷新后观察窗口内 Unity **未重建** `BattleStage1.PlayModeTests.dll`（`isCompiling=False`、DLL bytes/mtime 不变）。
     如实说明：这不单独构成“编译已执行”的证明（请求可能被域重载吞掉），因此**不以此单独声明新鲜**，只用它说明“导入同内容未触发脚本变更”；
     新鲜度结论以下一条**内容级字符串核对**为主证据；
  3. 直接在当前 DLL 元数据中检索本次源码的**独有字符串**（UTF-16，按两种字节对齐检索，`a1=True`）：`存在未完成的存档保护 run，已阻止新测试以免覆盖当前档…`、`真实 Farm 劳动必须按既有 profile 产出 Ingredients（不重复结算、不手改资源）`、`Continue 不得重种 / 增删建筑实例`、`Continue 不得重复生产批次`、`正式 TryPlaceBuilding 必须拒绝工坊`、`住房不得生成岗位定义：`、`Continue 必须逐项恢复劳务证据`、`单槽存档恢复失败（backup/manifest 已保留，需人工核对）`、`本次持久备份缺失，拒绝恢复：`、`活动日程必须恢复为用例前捕获的同一实例`、`正式 Catalog 不得渲染工坊选择按钮`、`默认 NewGame 必须按生成配置创建同数量建筑（数量可调，不硬编码比例）`、`InitializationProductPlayTests.SaveSlotGuard (test-only helper)`、`SHOT-UNAVAILABLE`、`initialization-product-play-evidence.log` — **全部命中**。
- 结论：当前加载的 `BattleStage1.PlayModeTests` 与当前源码内容一致，`InitializationProductPlayTests` + `[UnityTest]` 方法已真实编译进 DLL；
  **不以“源码里反射存在”代替“程序集 / 发现里存在”**——三处（源码、程序集反射、`tests` 源发现）本轮均已实测。

### 11.5 读回并静态审查（默认入口 / 用户档持久 guard / 日程 finally）

- **默认入口（不绕过生成配置）**：`[UnityTest]` 内先 `InitialWorldConfigurationCatalog.TryReadDefault()`，
  再走真实 `EventSystem` 点击链 `MainMenuUI.btnNewGame → HeroSelectionUI` 英雄卡（点击前断言 `btnConfirm` 禁用）`→ btnConfirm`；
  进入 `TownUI / ProductHomeWorldHost` 后再逐项比对“配置建筑 / 人口 vs 运行时实例”。建筑数量与定义均取自真实配置，不硬编码 7/5/6。
- **用户档持久 guard**：`[UnitySetUp]` 首步 `SaveSlotGuard.Capture()`（`BlockOnUnfinishedRuns` 先阻断未完成 run → 捕获当前用户单槽 bytes+SHA256 → `CreateNew` 备份并回读字节 / SHA 校验 → 写 `manifest.json`）；写档路径来自 `SaveSystem.GetSavePath()` 而非硬编码。
  `[TearDown]` 的**第一层 finally** 调 `Restore()`：校验 `_captured`，只按本 run `original-save.bin` 恢复，回读比对 `existed`+SHA 并写 `restored / restore-failed`；异常路径保留 backup + manifest 供人工取证，不删除、不改写历史 run。
- **日程 finally 恢复**：`[TearDown]` 的 `finally { try { ResetForTests } finally { RestoreActivitySchedule } }`；
  恢复为用例前捕获的**同一实例**并断言 `ReferenceEquals`，且置回 `null` 防重复恢复；未隔离活动窗口（`_clockBeforeActivitySchedule==null`）时直接早退，不做多余写入。
- **边界观察（如实记录，不修改）**：若 `[UnitySetUp]` 在 `Capture()` 内部/之前失败，`[TearDown]` 仍会执行，`Restore()` 会以“Capture 未成功完成”再报一次断言（噪声，不掩盖首个失败，也不会误写档）；
  用例依赖真实 `SampleScene / TownUI / HeroSelectionUI / MainMenuUI` 场景组件，缺失时以明确断言文本失败，不降级为静默通过。
- 本轮**未做任何业务冒烟**（未 NewGame / 未派工 / 未渲染 / 未 Save-Load），符合本阶段边界。

### 11.6 本阶段实际写入与边界

| 项 | 实际 |
|---|---|
| 写入文件 | 仅本文件 `Development/StudioRuns/Initialization-20261027/product-play-integration-review.md`（追加本节） |
| Unity 侧动作 | 三份 PlayMode 测试源码 `ImportAsset(ForceUpdate\|ForceSynchronousImport)` + `CompilationPipeline.RequestScriptCompilation()`（安全刷新；未产生新 DLL、未改内容，三份源码 SHA256 与 §10.1 完全相同） |
| 未做 | 未运行 `tests-run`（含单方法 / 整类）、未启动或忙等 TestRunner、未 Play、未 `rql-mutate`、未 Shell / `Process.Start`、未 Git、未 `AssetDatabase.SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `SetDirty` / `ClearDirty`、未写 Preferences、未 PrefabStage、未启停 Editor、未改字体 / 场景 / 存档 / guard 历史 / 34 项基线、未重算或改写基线文件、未改产品与测试源码 / 断言 |
| 未创建 | 未创建额外付费子 Agent / Workflow / Goal；未安装组件、未改权限、未改计费渠道；未访问未授权工程 / 认证目录 / 其他工作区 |

### 11.7 本阶段结论与门禁（不预填 PASS）

1. 类、方法、`[UnityTest]`、程序集、DLL 内容、`tests` 源发现**全部真实成立**；拒绝回执 `0483b34b…` 的原因为
   **工具 `testClass` 正则要求“类名前必须存在命名空间段”，全局命名空间类必然 0 匹配**（已用同一正则复算 + IL 证据，非猜测）。
2. 本阶段**不做**产品 / 测试改动，也不加 namespace（避免与预设方法全名冲突）。
3. 阶段 1/3 的唯一通过依据：宿主以 **`testMode=PlayMode` + `testMethod=InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed`** 单例串行执行的真实回执；
   PASS/FAIL 由真实 stack 决定，本节不预填；若失败，按宿主真实异常栈窄修（不改弱任何强断言）。

---

## 12. 阶段 2/3：Home 实体显示与存档三例集成（本轮核验与交接）

> 本节只做「回看产品专项真实结果 → 确认 `ProductHomeFoundationPlayTests` 三例真实发现 + 持久用户档 guard → 交宿主整类」。
> **未重做整包、未改产品代码、未改测试代码、未放宽任何断言、未运行 `tests-run`、未做无保护业务冒烟。**

### 12.1 回看产品专项真实结果（不重复 Edit 9 例）

| 项 | 实测 |
|---|---|
| 宿主真实结果（`testResults` 源，只读） | `InitializationProductPlayTests.DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed` → **`Passed` / 1974 ms / `message=null`** |
| 用例证据日志（`play-evidence/initialization-product-play-evidence.log`，本轮只读） | 15 行完整：`FONT-PRE`→`SAVE-CAPTURE`→`CONFIG(buildings=7 people=5)`→`ENTRY`→`WIRING(farmWorkplace=workplace:building_instance_1:main)`→`MANAGEMENT(已绑定)`→`LABOR(minute=0->521 ingredients=0->1 evidenceMinutes=41)`→`SAVE(sha256=53f8998f…)`→`CONTINUE(sameBuildings=True 逐项等值)`→`SHOT(76821 B / 2ad2089b…)`→`WORKSHOP-REJECT`→`END`→`SAVE-RESTORE matches-before=True`→`SCHEDULE-RESTORE sameInstance=True`→`FONT-POST unchanged=True` |
| 结论 | 阶段 1/3 门禁已由宿主真实回执通过；本轮**不重跑、不改写**，也不重复 `d10f7d0d` 的 Edit 9 例 |

### 12.2 开工只读复核（本轮实测，2026-10-28T11:46–11:47Z）

| 项 | 实测值 | 判定 |
|---|---|---|
| 工程 / 版本 | `project=...\UnityProject\RRPG1.0game`、`unity=2022.3.62t9` | ✅ Pilot |
| Editor 空闲 | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False / isPlayingOrWillChangePlaymode=False` | ✅ |
| 原生 TestRunner 空闲 | `TestRunnerApi.IsRunActive()`（`Static\|NonPublic`）＝ **False** | ✅ |
| 字体资源隔离 scope | `SessionState "Studio.Pilot.FontTestScope"=False` | ✅ 已正常 End |
| 场景 | `SampleScene isDirty=False / rootCount=5` | ✅ |
| 受保护字体（磁盘 + 子资产） | `14961033 B / 3c758e16…c404854`；子资产 `9 / dirty=0` | ✅ |
| 34 项 workspace 基线 | `entries=34 / same=34 / changed=0 / missing=0` | ✅ |
| 当前用户单槽 | `13134 B / adcf5d8b…8ceb08`、`.tmp=False`（产品用例结束后由 guard 回写为本值） | ✅ 按实际值 |
| 当前档 guard | `64 目录 = 64 restored + 0 protective + 0 noManifest + 0 unfinished`（含阶段 1/3 产品用例新增的 1 个 run） | ✅ 无未完成 run |

### 12.3 `ProductHomeFoundationPlayTests` 三例真实发现（`tests` 源，非源码推断）

| fullName | mode | assembly | namespace | ignored / explicit |
|---|---|---|---|---|
| `RRPG.ProductHome.Tests.ProductHomeFoundationPlayTests.StableEntity_AwayReturnAndSimulationGating` | PlayMode | `VillageStage1.PlayModeTests` | `RRPG.ProductHome.Tests` | false / false |
| `RRPG.ProductHome.Tests.ProductHomeFoundationPlayTests.Traveling_DoesNotArriveOrAdvanceClock_ThenUsesActualArrival` | PlayMode | `VillageStage1.PlayModeTests` | `RRPG.ProductHome.Tests` | false / false |
| `RRPG.ProductHome.Tests.ProductHomeFoundationPlayTests.CameraAndSelectionNotSaved_LoadRebuildsFromExistingAuthority` | PlayMode | `VillageStage1.PlayModeTests` | `RRPG.ProductHome.Tests` | false / false |

已加载程序集反射复核（`VillageStage1.PlayModeTests.dll`）：`type=RRPG.ProductHome.Tests.ProductHomeFoundationPlayTests`、`sealed=True / public=True`；
三个 `[UnityTest]` 方法均为 `IEnumerator`、0 参数；生命周期实际形态 `Setup`（`[UnitySetUp]`）+ `Cleanup`（`[TearDown]`）；
嵌套 `SaveSlotGuard` 存在，公开成员 `Capture / Restore / Restored`。
编译期依赖类型实测来源：`VillageTime`、`ProductHomeSnapshot`、`HomeSelectionContext` 均来自 `Village.Runtime`（与 asmdef 引用一致）。

**类过滤可用性实测**（用宿主工具同一正则 `^.*\.<class>\.[^\.]+$` 复算）：`testClass=ProductHomeFoundationPlayTests` → **3/3 命中**（本类有命名空间，不受阶段 1/3 全局命名空间限制）。因此本阶段**无需改代码 / 加 namespace**。

### 12.4 三例静态审查（读回源码，断言未被削弱）

| 用例 | 同源 / 实体稳定断言（保留） | 相机 / 选择 / 存档断言（保留） |
|---|---|---|
| `StableEntity_AwayReturnAndSimulationGating` | 人物视图 `ResolvedSharedAsset`；真实 `TryAssignPerson(实例 Farm 岗位)`；派工不替换人物 / Farm 视图；离村隐藏人物视图且快照 `Away=True`；**住房视图不替换、`PlaceViews` 不含 `Home:<PersonId>`（不再有独立 Home Place 实体）；Home 投影 `BuildingInstanceId` = 真实 Completed 住房且 `DrawsWorldEntity=False`**；实例岗位投影指向同一建筑、`DrawsWorldEntity=False`；模拟关 / 开只影响 `WorldVisible`，不销毁 / 不替换视图 | — |
| `Traveling_DoesNotArriveOrAdvanceClock_ThenUsesActualArrival` | 实例 Kitchen → 实例 Farm 派工；真实 `Clock.AdvanceTo`；通勤中 `Traveling=True` 且 `CurrentPlaceId` 为空（不提前到场）；`StepPresentation(100f)` **不推进 Clock、不修改领域快照**、表现位置不瞬移；真实到场只更新目标位置、结束通勤、占用写入实例岗位、视图不替换；末段步长 ≤0.201 且到达目标 | 领域快照字符串前后 `Domain()` 等值（表现步进 / 到场不改模拟事实） |
| `CameraAndSelectionNotSaved_LoadRebuildsFromExistingAuthority` | 实例 Farm 派工后 `Domain()` 基线；选中人物 / 建筑均可 `FocusSelected` 且相机跟随对应视图 | 相机 `Pan / Zoom` 真实生效（`orthographicSize < 7.5`）但 `Domain()` 不变；单槽 JSON 断言**不含** `ProductHome / HomeSelection / WorldCamera / WorldOrigin / PresentationIntent`；`SaveSystem.Save → Load` 后人物、实例岗位职责、实例建筑、建筑视图全部由既有 Authority 重建 |

结论：三例仍是“真实产品实例世界 + 真实存档”路径，未回退到旧空世界 / 静态 `ws_farm_01` / 独立 Home Place 视图假设，断言只做了契约对齐，没有放宽或删除。

### 12.5 持久用户档 guard（本类实际形态）

- `Setup()`（`[UnitySetUp]`）**第一步**即 `_guard = new SaveSlotGuard(); _guard.Capture();` —— 先 `BlockOnUnfinishedRuns()`（`restored / protective-snapshot` 之外状态一律阻断），再取 `SaveSystem.GetSavePath()` 真实路径，`CreateNew` 备份并回读 SHA256，写 `manifest.json`；随后才 `ResetForTests → LoadScene → NewGame → Save → OnContinue`（即 Setup 会真实改写用户单槽，恢复责任完全由 guard 承担）。
- `Cleanup()`（`[TearDown]`）在 `try/finally` 内 `_guard.Restore()`：校验 `_captured`，只按**本 run** `original-save.bin` 写回，回读 `existed + SHA256` 并写 `restored / restore-failed`；失败保留 backup + manifest，不删除历史 run。
- 本协议与阶段 1/3 产品用例同源（同一保护根 `Development/StudioRuns/Initialization-20261027/save-guard`），且**已被本轮产品用例真实执行验证**：`SAVE-CAPTURE` → 测试真实改写单槽 → `SAVE-RESTORE matches-before=True`，当前档已回到 `13134 B / adcf5d8b…`。
- 边界观察（如实记录，不修改）：若 `Capture()` 内部失败，`Cleanup()` 仍会执行并以“Capture 未成功完成”再报一次断言（噪声，不会误写档）；`Restore()` 的 `if (_restored) return;` 为幂等短路，不重写。

### 12.6 编译 / DLL 新鲜度（内容级验证）

| 项 | 实测 |
|---|---|
| 源码 `Assets/Tests/PlayMode/Village/ProductHomeFoundationPlayTests.cs` | `24401 B / 2448f2009506fbcbf5a15525cb5327d59e989609de45f327cb537b5b79c6bb1e`（与 §10.1 同值，本轮未改） |
| `VillageStage1.PlayModeTests.dll` | `141312 B / 5390811056bc896599a389e0596f7977a54274f5b8611dec57b6ca72e04651cf`、mtime `2026-10-28T11:35:22Z` |
| mtime 关系 | 源码 mtime `11:43:03Z` **晚于** DLL `11:35:22Z` → 不用 mtime 冒充新鲜 |
| 内容级核对 | 在当前 DLL 元数据（UTF-16，双字节对齐检索）中命中本源码独有字符串：`Home 只作为住房实例投影，不得再创建独立 Place 世界实体`、`相机 / 选择不得写入模拟事实`、`表现步进不得推进真实 Clock`、`人物 Home 投影必须绑定真实 Completed 住房`、`实例岗位不得创建第二个世界实体`、`Home 投影不得凭空再画一套房屋`、`模拟关闭后世界必须不可见`、`模拟恢复后世界必须重新可见`、`Load 必须恢复实例岗位职责`、`Load 后实例建筑必须存在（不重种、不丢实例）`、`Load 后建筑视图必须可由实例身份重建`、`通勤中检查器必须明确显示尚未到达`、`真实到场只更新目标，不瞬移表现位置`、`单槽存档恢复失败（backup/manifest 已保留，需人工核对）`、`存在未完成的存档保护 run，已阻止新测试：`、`ProductHomeFoundationPlayTests.SaveSlotGuard (test-only helper)` — **全部命中** |
| Console 说明 | 当前 `Error/Exception` 3 条均为**本层自己的只读探针**（`Tool_Tests.IsTestRunActive` 需 `ref` 参数，探针传 0 参数导致 `TargetParameterCountException`，时间戳 19:44:50 本地）；**不是产品 / 测试 / 编译错误**，如实登记，不做清除 |

### 12.7 交接口径（阶段 2/3 门禁）

- 宿主执行：`testMode=PlayMode` + **`testClass=ProductHomeFoundationPlayTests`**（**整类 3 例串行，不拆方法、不并行**；类过滤已实测 3/3 命中）。
- 保留原断言集：实例同源（建筑 InstanceId / 岗位身份 / Home 绑定真实 Completed 住房）、表现步进不改事实、相机 / 选择不入档、Save→Load 由既有 Authority 重建、单槽 guard 恢复。
- PASS / FAIL **不预填**：若失败，按宿主真实异常栈做窄修（测试夹具问题改测试；落在产品 Save / Load / 投影才做产品窄修），不放宽任何强断言。
- 本轮未运行 `tests-run`、未进入 Play、未做 NewGame / 派工 / 渲染冒烟；三例真实结果以宿主统一 Studio 阶段 check 回执为准。

### 12.8 本阶段实际写入与边界

| 项 | 实际 |
|---|---|
| 写入文件 | 仅本文件 `Development/StudioRuns/Initialization-20261027/product-play-integration-review.md`（追加本节） |
| Unity 侧动作 | 无（本节只做 `rql-query` / `testResults` 读取与只读 C# 反射 / 字节核对；未 `Refresh`、未 `ImportAsset`、未重编译） |
| 未做 | 未 `tests-run`、未 Play、未 `rql-mutate`、未 Shell / `Process.Start`、未 Git、未 `SaveAssets / SaveChanges / SaveDirtyScenes / SetDirty / ClearDirty`、未写 Preferences、未 PrefabStage、未启停 Editor、未改产品 / 测试源码与断言、未改字体 / 场景 / 存档 / guard 历史 / 34 项基线、未访问未授权工程 / 认证目录 / 其他工作区、未安装组件 / 改权限 / 创建额外付费子 Agent |

---

## 13. 边界声明

- 仅操作授权 Pilot 工程；未访问未授权工程、认证目录或其他工作区。
- 未安装组件、未改权限、未改计费渠道、未创建额外付费子 Agent / Workflow / Goal。
- 未 `rql-mutate`、未全局保存、未保存场景、未 Git 写、未启停 Editor。
- 本文件与实现须由主开发专业核验；不代表制作人批准，也不表示初始化整包完成。

---

## 14. 管理 17 例续接：交接指针（2026-10-29）

> 本节只追加指针，**不改写**上文任何历史结论，也不预填管理 17 例的 PASS/FAIL。

- 原执行 `01a75064` 仍为 `unknown`、无原生终态；`recoveryReview.safeToContinue=true`
  （`2026-10-29T09:57:10.769Z`，source=`maintenance-host-process-and-native-test-review`），
  制作人 `2b6f25cf` 确认安全续接。旧错误历史保留。
- 恢复后实际现场、原中断副作用、管理 17 例源码 / 程序集 / 发现、四处夹具只读复核与宿主串行整类口径，
  全部记录在 **`Development/StudioRuns/Initialization-20261027/management-resume-review.md`**。
- 本阶段未改产品 / 测试源码、未运行 `tests-run`、未进 Play；`ManagementShellPlayTests` 17 例真实结果
  以宿主 `testMode=PlayMode + testClass=ManagementShellPlayTests` 整类串行回执为准；
  0 项 / skip 不算通过，失败保留真实 message / stack 后按栈窄修。
