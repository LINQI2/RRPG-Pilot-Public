# 双 FunctionSlot 真实 Gateway 补测实施：Editor-only internal 测试缝 + 第 19 例（2026-10-26，阶段 1/1）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包选择：主开发在 `two-function-slot-scope-review.md` 的 T1 基础上**收窄为 Editor-only INTERNAL 测试缝**，属原规格内测试工程实现；**不改**任何全局 public API / Authority / DTO / 玩家配置 / 生产资源。
- 实际写入（3 个文件）：
  1. `UnityProject/RRPG1.0game/Assets/Scripts/StaticConfig/StaticConfigRegistry.cs`（仅 `#if UNITY_EDITOR` 内新增 internal 可逆覆盖入口 + `TryGetBuildings` Editor 分支 + `Reset` 清除）；
  2. `UnityProject/RRPG1.0game/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`（**仅新增**第 19 例与专用 helper，原 18 例方法体未改）；
  3. 本文件。
- 未运行 `tests-run` / TestRunner（模型不 tests-run）；本文件**不预填**宿主单方法结果。未 `rql-mutate`、未 PrefabStage、未字体测量探针、未 Shell / Git / Preferences、未 `SaveAssets / SaveChanges / ClearDirty / SetDirty`、未启停 Editor、未改正式工程。
- 报告时刻：**2026-10-26T10:52Z**（本机 `2026-10-26T18:52+08:00`，`China Standard Time`），由只读 `script-execute` 读 `DateTime.UtcNow` 取得。

---

## 1. 开工前置只读实测（全部为本轮实测，非转述；2026-10-26T10:48:42Z）

| 项 | 实测值 | 判定 |
|---|---|---|
| Editor 空闲 | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False` | 空闲，无播放 / 编译 / 导入 |
| 场景 | `SampleScene`：`dirty=False`、`roots=5` | 干净 |
| Console | `Error` / `Exception` 查询 **0 条** | 无项目错误 |
| 受保护 TMP 字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；mtime `2026-10-10T13:23:55.9520319Z` | 与基线同值 |
| TMP 字体（内存子资产） | `LoadAllAssetsAtPath` = **9** 子资产、`dirty=0` | 9 clean / 0 dirty（只做只读判定，未测量、未重导入） |
| 单槽存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`；mtime `2026-10-26T10:29:35.1763281Z`；`.tmp=False` | 与收尾基线一致 |
| guard 根 `R3B-SaveProtection-20261024` | **244** 个 run 目录：`restored=243`、`protective=1`、`noManifest=0`、其它状态 **0** | 全部终态合法，无 unfinished |
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0`（逐项 bytes+SHA256） | **34/34 一致** |
| 磁盘 | `D:\` free = **26.98 GB** | 充足 |
| 测试源码（开工） | `54062B / f48f8f7d…f16e0b`、`[Test]` **18** 处、mtime `2026-10-26T03:08:46Z` | 与索引记录一致 |
| 测试程序集（开工） | `153088B / 9d09dabb…b69da`、mtime `2026-10-26T03:08:50Z` | 与索引记录一致 |
| 产品文件（开工） | `StaticConfigRegistry.cs`：`27236B / 6f3eaa0d…a70f91`、mtime `2026-10-10T12:10:57Z` | 本包前未被改 |
| 编译状态 | `scriptCompilationFailed=False` | 可安全 Refresh |

---

## 2. 实现 A：`StaticConfigRegistry.cs` 的 Editor-only internal 可逆建筑 Registry 覆盖

### 2.1 形态（仅内存 / 可逆 / 不改根）

```csharp
#if UNITY_EDITOR
        // 仅 Editor 编译存在的内存测试覆盖（Player 编译中字段 / 入口 / 分支全部不存在）。
        private static BuildingRegistry _buildingsOverrideForTests;
#endif

        public static bool TryGetBuildings(out BuildingRegistry buildings,out string diagnostic)
        {
#if UNITY_EDITOR
            // 仅 Editor：显式 internal 测试覆盖优先（内存；不写盘、不改 Root / 初始化路径 / 生产配置）。
            if (_buildingsOverrideForTests != null)
            {
                buildings = _buildingsOverrideForTests;
                diagnostic = "";
                return true;
            }
#endif
            buildings=null; // 原有默认路径原样保留
            ...
        }

#if UNITY_EDITOR
        internal static void SetBuildingsOverrideForTests(BuildingRegistry buildings) { ... }
        internal static BuildingRegistry GetBuildingsOverrideForTests() { ... } // 仅供测试保存 / 恢复旧状态
#endif

        public static void Reset()
        {
            ...原有 5 个领域缓存清理原样保留...
#if UNITY_EDITOR
            _buildingsOverrideForTests = null;
#endif
        }
```

### 2.2 边界逐条核对

| 约束 | 实际实现 | 判定 |
|---|---|---|
| 只允许 `StaticConfigRegistry.cs` 改产品 | 产品源码仅此 1 个文件（写后 `Assets/` 最近 20 分钟仅 2 个文件变化：本文件 + 测试源码） | 成立 |
| 仅 `#if UNITY_EDITOR` 内新增 | 字段 / 两个 internal 方法 / `TryGetBuildings` 分支 / `Reset` 清除 4 处全部包在 `#if UNITY_EDITOR` | 成立 |
| 明确 `ForTests` 命名 | `SetBuildingsOverrideForTests` / `GetBuildingsOverrideForTests` / `_buildingsOverrideForTests` | 成立 |
| 可逆 | `SetBuildingsOverrideForTests(null)` 即清除；测试 finally 恢复开工前状态 | 成立 |
| Reset 清除且其他 registry 未动 | `Reset()` 只在原有清理之后追加清空本字段；未改 `_root` / `TryInitialize` / 资源路径 / 其他领域缓存语义 | 成立（测试含其他领域前后指纹断言） |
| 无 public setter / 无公共 API 新增 | 两个入口均为 `internal`；编译后反射实测 `IsPublic=False`、`IsAssembly=True`；public `ForTests` 成员数 = **0** | 成立 |
| 不改 `_root` / 初始化资源路径 / 生产配置 | 覆盖只影响 `TryGetBuildings` 的返回值；`_root`、`RRPGStaticConfigRoot.Load()`、`Assets/Generated/**`、DTO、Save 未触碰 | 成立 |
| 不加全局 `InternalsVisibleTo` | 未新增任何 `InternalsVisibleTo`；测试经既有反射方式调用 internal 方法（与现有反射调用 Runtime 类型同型） | 成立 |
| 不反射写私有字段 | 测试只调用明确命名的 internal 测试方法；不写 `_root` / `_initialized` / 本字段 | 成立 |
| 播放器编译完全不存在字段入口 / 分支 | 源码层面由 `#if UNITY_EDITOR` 保证；**未构建 Player**，因此“Player 内确实不存在”属未测（见 §6） | 源码成立 / 运行未测 |

### 2.3 Editor-only 证据（编译后反射实测，2026-10-26T10:50Z）

| 检查 | 实测 |
|---|---|
| Runtime 程序集 | `StaticConfig.Runtime` |
| `SetBuildingsOverrideForTests` | 存在；`IsPublic=False`、`IsAssembly=True`、`IsStatic=True` |
| `GetBuildingsOverrideForTests` | 存在；`IsPublic=False`、`IsAssembly=True` |
| 字段 `_buildingsOverrideForTests` | 存在；`IsPublic=False`、`IsPrivate=True`、`IsStatic=True` |
| public `ForTests` 成员 | **0** |
| `StaticConfigRegistry` 的 Editor 分支 | 仅在 Editor 编译进入；`TryGetBuildings` 默认路径与全部诊断常量未改 |

---

## 3. 实现 B：`ManagementProjectionTests` 第 19 例

### 3.1 用例名与结构

`ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`（`[Test]`，EditMode；新增位置在既有第 18 例之后）。

流程（全部使用真实产品链，不手工伪造 Completed / assignment / arrival）：

1. 读取开工基线：`StaticConfigRegistry.TryGetBuildings` 的真实 `BuildingRegistry` 内容指纹 + 其他领域（Monsters / Presentation / VisualAssets / Encounter / Event / AutonomousLife / Social / VillagePresentation）结果指纹；读取并断言当前无遗留覆盖（`previousOverride == null`）。
2. 构造**仅内存**的双 slot 注册表：`new BuildingRegistry(baseline.All, baseline.FunctionBindings + aux)`；`aux` = `building_farm / aux / Workplace / ws_test_02`（真实已知 profile，`Capacity=1`；`main` = `ws_farm_01`，`Capacity=2`）。
3. `SetBuildingsOverrideForTests(twoSlotDefinitions)` 并断言覆盖真实生效。
4. **真实建造链**：`TryPlaceBuilding` → 两名工人经 **`ManagementCommandGateway.AddConstructionWorker`** 加入施工 → `Advance` 真实推进世界时钟（`prj_test_01` 目标 10 进度、每人每日 1 进度；两名工人 → 5 个工作日）→ 生产 `FinalizeConstruction` 完工并**自动注册两个 Workplace**；断言 `main` / `aux` 均已真实注册且 Capacity = 2 / 1、`ConstructionProjectId` 已清空。
5. **真实 Gateway 命令独立性**：
   - alpha → main `Success`；beta → aux `Success`；
   - gamma → aux → `DomainRejected` + 领域原文 **`CapacityFull`**，零写入，main 名额不变；
   - alpha → aux → `DomainRejected` + **`AlreadyAssigned`**，不自动换岗；
   - gamma → main `Success`（aux 满员不影响 main 派工）；
   - 撤职 beta（aux）→ aux 0、main 仍 2；
   - beta → main → `DomainRejected` + **`CapacityFull`**（main 满、aux 空，容量互不影响）；
   - 撤职 gamma（main）→ main 1；beta → main `Success`；aux 仍 0。
6. **投影逐 id 互核**：`ManagementReadAdapter.Read()`（走同一 `StaticConfigRegistry`）中 main / aux 各一行；同一 `BuildingInstanceId`、不同 `WorkplaceId` / `FunctionSlotId` / `FunctionProfileId`；`Capacity` 2 / 1；`IsRegistered=True`；逐 id 比对各 `VillageWorkplaceDef` 的 `WorkplaceId / DisplayName / Capacity` 与 `GetOccupantPersonIds` vs `BoundPersonIds`；main `IsFull=True`、aux `IsFull=False`；同一实例恰好 2 行。
7. `finally`：恢复开工前覆盖（`previousOverride`）并 `StaticConfigRegistry.Reset()`；`TearDown` 既有 fixture 继续恢复运行时世界。
8. 收尾（`finally` 之后、仅成功路径）：`BuildingsOverrideForTests() == null`（Reset 清覆盖）；默认 `TryGetBuildings` 内容指纹与开工相同；其他领域指纹与开工相同（Reset 只清建筑覆盖）；反射断言测试缝 `internal` 且无 public `ForTests` 入口；`#if !UNITY_EDITOR` 时明确失败（本用例依赖 Editor-only 缝）。

### 3.2 为什么这不是“削弱真实链”

- 施工完工由**真实时钟推进**触发（`Advance` 跨 5 个工作日），不是直接写 `LifecycleState` / `Progress` / `Assignment` / `Arrival`；
- 两个 `Workplace` 由生产 `FinalizeConstruction` 从注入定义**自动注册**，不是测试手工 `TryRegisterWorkplace`；
- 派工 / 满员 / AlreadyAssigned / 撤职全部经 `ManagementCommandGateway` 的两道真实校验（`StaticConfigRegistry` 定义 + `Assignments.GetWorkplaceDef` 运行时注册）；
- 覆盖仅替换“定义来源”，不替换 Gateway、Assignments、ReadAdapter、Project/Work、Clock 的任何逻辑。

---

## 4. 编译 / 测试发现（宿主门禁前置，非测试结果）

安全 `AssetDatabase.Refresh()` 后（未 `SaveAssets`、未保存场景 / 资产）：

| 检查 | 实测（2026-10-26T10:49:47Z 前后） |
|---|---|
| 编译 | `scriptCompilationFailed=False`；`StaticConfig.Runtime.dll` 与 `VillageStage1.Tests.dll` 均晚于各自源码 |
| 测试方法发现 | 反射：`ManagementProjectionTests` 上 `[Test]` = **19**；目标方法存在且 `isTest=True`；源码 `[Test]` = 19 处 |
| RQL test discovery | `tests` + `where fullName contains "ManagementGateway_TwoFunctionSlots"` = **1 条**（`EditMode`）——唯一，无重复/0 项 |
| Console | 编译后 `Error` / `Exception` = **0**；`Warning` 仅 10 条 SampleScene `LiberationSans SDF` 缺中文字形（场景文字渲染既有噪声，与本包改动无关） |
| 程序集新鲜度 | `StaticConfig.Runtime.dll` `52224B / 35636fc2…acd417`、mtime `2026-10-26T10:49:47.6497792Z`；`VillageStage1.Tests.dll` `160768B / d85583f8…09c8e379`、mtime `2026-10-26T10:49:48.3067151Z` |

> 本阶段**未启动 TestRunner**；以上只证明“编译通过 + 新方法唯一可发现”。实际绿/红由宿主单方法 Edit 门禁给出，本文件不预填。

---

## 5. 公共 API / 边界零新增核对

| 项 | 结论 | 证据 |
|---|---|---|
| public API 新增 | **0** | 两个缝均为 `internal`；反射 public `ForTests` 成员 = 0；`TryGetBuildings` 签名未变 |
| Authority / DTO / Save / 玩家配置 | 未改 | 仅测试与 Editor 内存覆盖；未写生成根 / 资产 / 配置 |
| 生产配置 / `_root` / 资源路径 | 未改 | 覆盖仅在 `TryGetBuildings` 返回前短路；`TryInitialize` / `Load()` 原样 |
| 全局 `InternalsVisibleTo` | 未加 | 全仓库未新增（开工 review 已记录 0 命中，本包未触碰程序集信息） |
| 仅必要文件 | 成立 | 产品 1 个 + 测试 1 个 + 本文件 1 个；`Assets/` 最近 20 分钟仅 2 个源码 / 测试文件变化 |
| 原 18 例 | 未改 | 新用例追加在第 18 例之后；原 18 个 `[Test]` 方法体与 helper 语义未动（新增 helper 仅被第 19 例调用） |

---

## 6. 保护面（编译后只读复核，2026-10-26T10:50:38Z）

| 项 | 实测 | 判定 |
|---|---|---|
| Editor | `isPlaying=False` / `isPaused=False` / `isCompiling=False` / `isUpdating=False` | 空闲 |
| 场景 | `SampleScene dirty=False roots=5` | 未保存、未改 |
| 受保护字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、mtime 仍 `2026-10-10T13:23:55Z`；9 子资产 **0 dirty** | 未变 / 未探测 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、mtime `2026-10-26T10:29:35Z`、无 `.tmp` | 未变 |
| guard | `244 = 243 restored + 1 protective`，`noManifest=0`、其它 **0** | 无未知状态 |
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 34/34 一致 |
| 磁盘 | `26.97 GB` free | 充足 |
| 写入文件哈希（写后） | `StaticConfigRegistry.cs` `29248B / 1c0c62fa77678e5bd25a5dec09ec164d9a2ccbad7054fbb72267838b8d366e75`；`ManagementProjectionTests.cs` `70548B / 64c8c12482ac1ad22c0ab524e6947138c789a752df7108233ae137bdd5ba68a7` | 本文件写出前实测 |
| `Assets/` 近 20 分钟变化 | 仅上述 2 个文件 | 无 Prefab / Scene / 字体 / 配置 / 生成根写入 |
| 未执行 | `tests-run`、`rql-mutate`、PrefabStage、`SaveAssets` / `SaveChanges` / `ClearDirty` / `SetDirty`、字体探针、Shell / Git 写 / Preferences、启停 Editor、正式工程 | — |

> 磁盘 hash / 0 dirty 只证明观察面，不推断“全机零副作用”。**未** Save / SaveAssets / SaveChanges / ClearDirty / SetDirty；插件自动保存行为未用于本包任何资产。

---

## 7. 未测 / 风险 / 需宿主与主开发核验

1. **宿主单方法 Edit 未运行**（模型不 tests-run）：第 19 例真实红 / 绿、耗时、日志由 Studio 宿主单方法执行；本文件不预填结果。
2. **Player 编译未构建**：`#if UNITY_EDITOR` 源码边界已核对，但“Player 内确实无字段 / 入口 / 分支”只有源码证明，未做 Player 构建验证。
3. **真实 5 工作日推进的副作用面**：第 19 例跨越 5 个世界日（`Advance` 真实时钟），会触发既有日结算 / 活动窗口 / 资源结算；断言只涉及建筑 / 岗位 / 派工与投影，但不能宣称“运行期间无任何其它系统状态变化”。若宿主运行出现与建筑链无关的既有系统异常，应按真实 message / stackTrace 留证，不得改写断言换取绿色。
4. **测试内部反射依赖**：若后续有人移除 / 重命名 `ForTests` 方法，第 19 例会明确失败（这是有意契约）；不得为过测改回 public 或改核心。
5. **原 18 例未重跑**：本包未重跑已过全集（按任务纪律）；原 18 例方法体未改，但其与本例共享 fixture，若宿主整类运行出现秩序相关差异需按真实回执处理。
6. **未覆盖**：UI 双档位渲染、Save/Load 双 slot 持久化、Config Authority 新增正式第二 slot（属产品决定）；本包仅补 EditMode 真实 Gateway 契约。
7. 本报告须由**主开发专业核验**；不代表制作人批准。R3-B 状态不变（Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published）。

---

## 8. 下一步（不在本包执行）

1. Studio 宿主执行**单方法 Edit**：`RRPG.Management.Tests.ManagementProjectionTests.ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`，保留真实回执（runId / 耗时 / message / stackTrace / 日志）。
2. 若通过：由主开发核验 §2–§6 证据并决定是否更新交付索引；若失败：保留具体失败与新增日志行，**不擅自升级 public、不改核心、不改冻结断言**。
3. 后续如需“宿主整类 19 例”回归，应作为独立完整回归包授权，不在本包内执行。
