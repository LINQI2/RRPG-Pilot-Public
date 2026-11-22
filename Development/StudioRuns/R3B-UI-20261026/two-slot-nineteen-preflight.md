# 双槽修后 ManagementProjectionTests 整类 19 例 · 宿主交付前预检（2026-10-26，阶段 1/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包范围：**只读恢复保护预检 + 仅新增本文件**。不改代码 / 配置 / Prefab / Prefs / manifest；不 `AssetDatabase.Refresh`、不 `SaveAssets / SaveChanges / ClearDirty / SetDirty`、不字体测量、不 PrefabStage、不 `rql-mutate`、不 Shell / Git 写、不启停 Editor、不运行 `tests-run`。
- 上游事实（主开发转述，非本执行层可直接读取的 Studio 私有 runtime）：双槽单例 run `740fd50c-8680-4dc6-8a4b-8589e501fedc`，**1/1 Passed、0 skipped、1.9201088 s、scope-restored**。本文件把“转述”与“本机只读实测”分开标注。
- 预检时刻：**2026-10-26T10:53:38Z**（本机 `18:53:38 +08:00`，China Standard Time）；只读复核至 **2026-10-26T10:54:32Z**。
- 结论前置：**19 例源码 / DLL / 发现 / 保护面满足整类串行运行前置**；本文件**不预填整类 19 例 PASS/FAIL**，真实结果只能由宿主以短类名 `ManagementProjectionTests` 整类 EditMode 串行运行产出。

---

## 1. 开工现场（全部只读实测，2026-10-26T10:53:38Z）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `Assets/Scenes/SampleScene.scene`：`dirty=False`、`roots=5`、`isLoaded=True` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开，无未保存 Prefab |
| 原生 TestRunner 活动运行 | `TestRunnerApi.IsRunActive()`（non-public static，反射实测）= **False** | 无活动运行 |
| Console | `Error/Exception` **6 条**，全部是 MCP `Script / Execute` 工具自身失败（§2.4），**无项目源码编译错误**；`EditorUtility.scriptCompilationFailed=False` | 编译面干净；不把工具错误写成项目错误 |
| 磁盘 | `D:\` free = **26.97 GB** | 充足 |

---

## 2. 保护面（字节 + SHA256 逐项实测）

### 2.1 受保护 TMP 字体

| 检查 | 实测 |
|---|---|
| 磁盘文件 | `14961033B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；mtime `2026-10-10T13:23:55.9520319Z` |
| 内存子资产 | `LoadAllAssetsAtPath` = **9** 个子资产；`EditorUtility.IsDirty` 统计 **dirty=0** |
| 判定 | 与基线同值；**未** `GetPreferredValues` / `ForceMeshUpdate` / 重导入 / 测量。0 dirty 只证明观察面，不推断全机零副作用 |

### 2.2 34 项 workspace 基线 + 单槽存档 + guard

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项基线（`R3B-20261022/workspace-baseline.json`） | `total=34 / same=34 / diff=0 / missing=0`（逐项 bytes+SHA256） | **34/34 一致** |
| 单槽存档（`Application.persistentDataPath/rrpg_save_p0.json`） | `exists=True`、**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T10:29:35.1763281Z`、`.tmp=False` | 与收尾基线一致；本包未 Load / Save |
| guard 根 `R3B-SaveProtection-20261024` | **244** 个 run 目录：`restored=243`、`protective-snapshot=1`、`other=0`、`noManifest=0` | 无 unfinished / unknown / captured / restore-failed |
| guard 最新 run | `20261026T102934Z`，manifest mtime `2026-10-26T10:29:35.1763281Z` | 与单槽收尾时刻一致 |

---

## 3. 双槽单例真实日志与收尾状态证据

### 3.1 可核对的直接证据

| 来源 | 内容 | 限制 |
|---|---|---|
| 主开发转述 | run `740fd50c-8680-4dc6-8a4b-8589e501fedc`：**1/1 Passed、0 skipped、1.9201088 s、scope-restored** | 本执行层无 Studio 私有 runtime 访问权，不能冒充复核 |
| 本机 `rql-query testResults` | 唯一 1 条：`RRPG.Management.Tests.ManagementProjectionTests.ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`，`Passed`、`durationMs=291`、`message=null` | 不带 runId / 时间戳 / skipped；`291 ms` 与转述 `1.9201088 s` 不是同一记录口径，**不能互相校验，也不改写任一数字** |
| 本机 MCP server 日志（`Library/mcp-server/win-x64/logs/server-log.txt:25809-25810`） | `2026-10-26 18:52:09.6346` `"tests-run" completed. IsError = False.`，调用耗时 `18673.5779 ms` | 只有工具调用级成功与总耗时；**不含参数、用例名、Passed/skipped、断言输出** |
| 第 19 例源码 | 无 `TestContext.WriteLine` / `[TWO-SLOT]` 标记；失败时才由断言 message 暴露 | **没有可逐行引用的运行日志正文** |

### 3.2 收尾三项：默认 registry 恢复 / 覆盖 null / 其他 domain 指纹不变

| 检查 | 实测（2026-10-26T10:54Z，只读反射） | 判定 |
|---|---|---|
| 当前建筑覆盖 | `GetBuildingsOverrideForTests()` 返回 **null** | 覆盖已清 |
| 默认建筑 registry | `TryGetBuildings=True`，诊断空；`definitions=4`、`bindings=3`（仅生产 `main`） | 默认路径可用，未残留双槽注入 |
| 其他 domain | `TryGetMonsters` / `TryGetEnemyPresentation` / `TryGetVisualAssets` / `TryGetEncounterConfig` / `TryGetEventConfig` / `TryGetAutonomousLifeConfig` / `TryGetSocialConfig` / `TryGetVillagePresentation` 全部 **True** | 8 个既有入口均可解析 |
| “与开工相同”的严格比较 | 第 19 例在 `finally` 后断言 `baselineShape == DescribeBuildingRegistry(restored)` 且 `otherDomainsBefore == CaptureOtherDomainFacts()`；该例 `Passed` 即这些断言未被触发失败 | **只有测试通过状态作为证明，没有独立运行日志正文**；本文件不把当前值外推成“历史逐项指纹已复核” |

> 说明：本机可见的日志只到“单方法运行成功”这一层；**收尾三断言的运行正文不可见**。本文件不补写、不臆造日志行。若主开发持有完整 Studio 记录，应以该记录为准。

---

## 4. Editor internal API 与 `#if` 边界（源码 + 编译后反射）

### 4.1 源码边界

`Assets/Scripts/StaticConfig/StaticConfigRegistry.cs` 共 4 处 `#if UNITY_EDITOR` / `#endif`：

| 位置 | 内容 | 边界 |
|---|---|---|
| 132–135 | `private static BuildingRegistry _buildingsOverrideForTests;` | 仅 Editor 编译存在 |
| 154–162 | `TryGetBuildings` 内覆盖优先分支 | 仅 Editor 编译进入；无覆盖时原默认路径原样 |
| 172–194 | `internal SetBuildingsOverrideForTests` / `internal GetBuildingsOverrideForTests` | 仅 Editor；`internal` 非 public |
| 245–248 | `Reset()` 末尾清空覆盖 | 仅 Editor；其余既有领域清理未改语义 |

`TryGetBuildings` 签名、默认 Root 读取、`_root` / `TryInitialize` / 资源路径 / 生产配置均未改；全仓库 `InternalsVisibleTo` 查询 **0 命中**。

### 4.2 编译后反射实测（Runtime 程序集 `StaticConfig.Runtime`）

| 检查 | 实测 |
|---|---|
| `SetBuildingsOverrideForTests` | 存在；`IsPublic=False`、`IsAssembly=True`、`IsStatic=True` |
| `GetBuildingsOverrideForTests` | 存在；`IsPublic=False`、`IsAssembly=True`、`IsStatic=True` |
| `_buildingsOverrideForTests` | 存在；`IsPrivate=True`、`IsStatic=True` |
| public `ForTests` 成员 | **0** |
| 当前覆盖 | **null**（§3.2） |

> 结论：这是 **Editor-only internal 测试缝**，不是公共 API；未新增 public API、未改 Authority / DTO / Save / 玩家配置。**未构建 Player**，故“Player 内确实不存在字段 / 入口 / 分支”只有源码 `#if` 证明，属未测。

---

## 5. 19 例发现与程序集新鲜度

| 项 | 实测值 | 判定 |
|---|---|---|
| 测试源码 | `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`：**`70548B / 64c8c12482ac1ad22c0ab524e6947138c789a752df7108233ae137bdd5ba68a7`**，mtime `2026-10-26T10:49:36.3747910Z` | 与双槽实施包写后值一致；本包未改 |
| 源码属性计数 | `[Test]=19`、`[UnityTest]=0` | 19 个普通 EditMode 用例 |
| 测试程序集 | `Library/ScriptAssemblies/VillageStage1.Tests.dll`：**`160768B / d85583f889a4a6bd1e34cdb3c7f31eede5c1e8f3c124748faab6f86309c8e379`**，mtime `2026-10-26T10:49:48.3067151Z` | 晚于源码 **11.93 s**，DLL 新鲜 |
| Runtime 程序集 | `Library/ScriptAssemblies/StaticConfig.Runtime.dll`：`52224B / 35636fc23b2c9b8bae1d993445a3bbc4f795cf31594906d05d34382e79acd417`，mtime `2026-10-26T10:49:47.6497792Z` | 晚于产品源码 `10:48:55Z` **51.99 s**，DLL 新鲜 |
| 产品源码 | `StaticConfigRegistry.cs`：`29248B / 1c0c62fa77678e5bd25a5dec09ec164d9a2ccbad7054fbb72267838b8d366e75`，mtime `2026-10-26T10:48:55.6554791Z` | 与实施包写后值一致 |
| 发现（`rql-query tests`） | 短类名前缀命中 **19** 条，`mode` 全部 `EditMode`，assembly 全部 `VillageStage1.Tests` | 与源码计数一致 |
| 目标方法唯一性 | `where fullName contains "ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent"` = **1** | 唯一，无重复 / 0 项 |

19 例清单（发现级，按查询顺序）：`CapacityFullAndAlreadyAssigned…`、`CompletedBuildingRejectsConstructionWorker…`、`ConstructionLaborDoesNotCountTowardPrimaryWorkplace`、`ConstructionWorkerKeepsPrimaryAssignment`、`DeadUnavailablePersonIsRejected…`、`LegacyStaticAssignment…`、`ManagementConstructionClaim_PreservesPrimary…`、`ManagementGateway_TwoFunctionSlots…`、`MissingOrAwayPerson…`、`ReadProjectionDoesNotTouchWorldClock…`、`RemovingAssignmentDropsRecordAndAccumulatedLabor…`、`RemovingAssignmentDuringLabor…`、`ResourcesAndNutritionFacts…`、`SameDefinitionBuildingsAndRegisteredSlots…`、`SameNameDifferentPersonIds…`、`SaveFeedbackUsesExistingBool…`、`UnderConstructionBuilding…`、`WorkplaceLaborOnlyCountsActualWorkplaceExecution`、`WorldBusyRejectsManagementCommands…`。

---

## 6. 交宿主门禁

1. 请宿主以**短类名 `ManagementProjectionTests`** 运行 **EditMode 整类 19 例、串行**；不预填结果，以真实回执（runId / 通过数 / skipped / 耗时 / 失败 message / stackTrace / 日志）为准。
2. 运行前请确认：场景 `dirty=False`、`IsRunActive=False`、单槽 `13405-4b2b56` 未变、guard 无 unfinished。本包实测均满足（§1–§2）。
3. 若失败：保留真实失败用例、message、stackTrace 与第 19 例收尾断言位置，不以放宽夹具 / 删断言掩盖；本文件不回写为 PASS。
4. 本文件及预检结论须由**主开发专业核验**，**不代表制作人批准**；R3-B 仍为 `Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published`。

---

## 7. 本包边界与实际副作用

- **唯一写入**：本文件（新增）。未改任何既有文档、代码、测试、断言、Prefab、Scene、字体、存档、guard manifest、34 项基线。
- 只读接口：`rql-query`、只读 C#（`File` 读取 / SHA256 / `EditorUtility.IsDirty` / `LoadAllAssetsAtPath` / 目录与 manifest 统计 / 反射 / 读 `Editor.log`）、本地 `read` / `grep`。
- **未运行** `tests-run` / TestRunner；**未** Refresh、**未** `rql-mutate`、**未** PrefabStage、**未** Shell / `Process.Start`、**未** Git 写、**未** Preferences、**未**启停 Editor。
- **未** `SaveAssets / SaveChanges / SaveDirtyScenes / SetDirty / ClearDirty`；**未** 重导入或探针字体。
- 未安装组件、未改权限、未创建额外付费子 Agent。
- 本包仅完成**阶段 1/2**；阶段 2 文档与源码只读核对由 Studio 在宿主 19 例回执之后自动继续，本文件不越阶段执行、不修改 `reviewable-delivery-index.md`。

---

## 8. 写后只读复核（2026-10-26T10:55:08Z，写入后实测）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、mtime `2026-10-10T13:23:55Z`、9 子资产 **0 dirty** | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、mtime `2026-10-26T10:29:35Z`、`.tmp=False` | 未变 |
| guard | `dirs=244 / restored=243 / protective=1 / other=0 / noManifest=0` | 仍无 unfinished |
| 场景 / Editor | `playing=False`、`compiling=False`、`sceneDirty=False roots=5`、`PrefabStage=null` | 仍干净空闲 |
| 测试源码 | `70548B / 64c8c124…a68a7`、mtime `2026-10-26T10:49:36Z`、`[Test]=19` | 未改 |
| 本文件 | §8 写入前初版 `11947B / e33014a5…8ab29`（mtime `2026-10-26T10:55:02Z`）；本表及后续补记会改变自身字节，故**不以文内旧哈希自称最终版**，最终以目录实际文件为准 | 本文件为唯一新增产物 |

> 写后复核未再运行测试、未 Refresh、未保存任何资产 / 场景。19 例整类真实回执尚未产生，等待宿主串行运行。
