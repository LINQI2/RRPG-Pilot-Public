# R3-B Pilot 完整 PlayMode 回归 · 保护预检报告（阶段 1/1）

- 仓库 / 工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（本任务唯一授权目录）
- Unity 工程：`UnityProject/RRPG1.0game`（当前已连接的 Pilot Editor，**未启停**）
- 本阶段范围：**只做只读预检 + 单槽保护快照 + 本报告**。未改源码 / 测试 / 断言 / 场景 / Prefab / 资产 / 静态配置 / 字体 / Packages / ProjectSettings；未 `AssetDatabase.Refresh`；未运行任何 `tests-run` / Test Runner / 反射启动 / Compiler 进程；未 `rql-mutate`；未 Shell / `Process.Start`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 Git 写（commit/push/stage/切分支）；未 Load/Save 产品流程；未恢复历史 650 档；未覆盖任何既有 run / 快照；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件 / 改权限 / 创建额外付费子 Agent。
- 本阶段唯一写入：本报告 + **一个新 run 目录**（`FileMode.CreateNew`，见 §4，未覆盖 / 未改动任何既有 run）。

> **不预填通过**：本文件**不含任何 PlayMode 测试 PASS/FAIL**。下文“通过/一致”只指**只读预检项**（身份、空闲、场景、34 项 SHA、字体、Play 发现、存档保护状态）成立；完整 PlayMode 全集尚未运行，结果由宿主 Studio 阶段 check 串行产出，本包**未调用 `tests-run`**。

---

## 1. 预检结论速览

| 项 | 实测值 | 结论 |
|---|---|---|
| Unity 版本 | `2022.3.62t9`（`Application.unityVersion`） | 与 Pilot 要求一致 |
| Editor 状态 | `isPlaying=false / isCompiling=false / isUpdating=false`（`UnityEditor.EditorApplication` 与 `rql-query editor` 两路读数一致，`isPaused=false`） | 空闲，可进入宿主串行测试 |
| TestRunner | `EditModeLauncher.IsRunning=false`、`PlaymodeLauncher.IsRunning=false`（反射读取**静态字段**，非属性） | 无 running / pending 运行 |
| 场景 | `SampleScene`，`Assets/Scenes/SampleScene.scene`，`isDirty=false`，`rootCount=5`，`isLoaded=true` | 干净（`tests-run` 前置满足） |
| 34 项保护基线 | `total=34 same=34 missing=0 problems=0 allMatch=True`（逐项重算 bytes + SHA256，快照写入前后各核一次） | 一致 |
| 受保护 TMP 字体 | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（14961033B，34 项之一） | 与基线同值 |
| 单槽真实路径 | `SaveSystem.GetSavePath()` 实调 → `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot\rrpg_save_p0.json` | 存在 |
| 当前单槽内容 | `exists=true`、`13405B`、`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、`mtimeUtc=2026-10-24T14:33:25.6672726Z` | 与主开发回读的 `20261024T142654Z` restored 值**逐字节一致** |
| 本轮保护快照 | `R3B-SaveProtection-20261024/**20261024T143611Z**/original-save.bin`：`13405B / 4b2b56f6…`，回读复核一致 | 已实际落盘，`status=protective-snapshot` |
| 存档 run 总览 | **127** 个目录：`restored=125`、`protective-snapshot=2`（`20261024T052704Z`、`20261024T143611Z`）、未完成 **0**、无 manifest **0** | 无 `captured/absent/restore-failed` 残留；不阻断 Play |
| 完整 PlayMode 发现 | **121** 个用例 / **5** 个程序集（`BattleStage1 63 + VillageStage1 32 + ExpeditionV4F 13 + VillageV4 12 + PersonBootstrap 1 = 121`）；`explicit=0`、`ignored=0`、`parameterized=0` | 无 filter 全集已核 |
| 管理专项 | Play `ManagementShellPlayTests` **10** 条已发现（Edit 侧历史回执 1513/1513 只作引用，本包未重跑） | 与 R3-B Pilot 现状一致（发现层面） |
| Play 程序集新鲜度 | 5 个 `*.PlayModeTests.dll` 均存在；最新 `.cs`（`ManagementShellPlayTests.cs`）mtime `2026-10-24T14:22:29.5101197Z` < `BattleStage1.PlayModeTests.dll` mtime `2026-10-24T14:22:35.7217654Z` | DLL 不早于源码，`newestCsNewerThanDll=False` |
| 存档写风险 | **高风险，且是“删除 + 重写”**：49 个 PlayMode `.cs` 中 **35 个**直接 `File.Delete` 单槽、**7 个**写档，仅 **4 个**有内存字节备份（TearDown 回写），仅 **1 个**类（`ManagementShellPlayTests.SaveSlotGuard`）有**持久**备份 | 已按授权建立本轮保护快照（§4） |
| 场景 / 资产 API 写风险 | PlayMode 测试源码中对 `AssetDatabase / SaveScene / OpenScene / NewScene / PrefabUtility / EditorSceneManager / CreateAsset / SaveAssets / SetDirty` 的匹配数 = **0** | 无场景 / 资产 / Prefab 写入路径 |
| Compiler 264 | 本层无合法运行入口（无 dotnet / Shell / Compiler 工具） | **BLOCKED**（能力边界，见 §7） |

---

## 2. 只读预检（写任何东西之前）

### 2.1 身份 / Editor / TestRunner
- `script-execute` 只读片段：`unity=2022.3.62t9`、`dataPath=D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets`。
- `rql-query editor | select isPlaying isPaused isCompiling isUpdating unityVersion` → `false|false|false|false|2022.3.62t9`。
- 反射扫描 `UnityEditor.TestTools.TestRunner.EditModeLauncher` / `PlaymodeLauncher`（`UnityEditor.TestRunner`）：**静态字段** `IsRunning` 均为 `False`（这两个类型把 `IsRunning` 暴露为字段而非属性，`GetProperty` 找不到，已用 `GetField` 复核，避免把“属性缺失”误判为有运行）。
- 结论：具备“只读核对 + 精确快照”的前置条件；**本包未启停 Editor、未触发编译、未 Refresh**。

### 2.2 场景与 Console
- `scenes | select name path isDirty rootCount isLoaded` → `SampleScene | Assets/Scenes/SampleScene.scene | false | 5 | true`。本包未打开 / 保存 / 切换任何场景。
- 本包所有 `script-execute` 只读片段正常返回，未新增项目级 `error CS`、未触发任何（重）编译（`isCompiling=false`、`isUpdating=false`）。

### 2.3 34 项基线与字体（逐项重算，非引用）
- 依据：`Development/StudioRuns/R3B-20261022/workspace-baseline.json` 的 `files[]`（每项 `bytes` + `sha256`），以只读正则逐项解析（34 条全部匹配）。
- 方法：对每个路径读文件、重算 `FileInfo.Length` + SHA256，**不写文件**。
- 结果（快照写入前）：`total=34 same=34 missing=0 problems=0 allMatch=True`，字体项 `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`。
- 结果（快照写入后复算）：同上 `total=34 same=34 missing=0 problems=0 allMatch=True`，字体同值 → 本轮快照写入**未触碰** 34 项基线。
- 说明：34 项含制作人指定 14 项（TMP 字体 1 + Growth 12 + ProjectSettings 1）**子集**；NuGet/Packages 属 34 项但不属 14 项（沿用既往更正）。测试源码 `Assets/Tests/**` 不在 34 项内。

---

## 3. 完整 PlayMode 发现口径（无 filter 全集）

- 查询：`tests | where mode == "PlayMode" | count` → **121**。
- 程序集：`tests | where mode == "PlayMode" | group-by assembly` → **5** 个，逐项相加 = **121**：

  | 程序集 | 条数 |
  |---|---|
  | BattleStage1.PlayModeTests | 63 |
  | VillageStage1.PlayModeTests | 32 |
  | ExpeditionV4F.PlayModeTests | 13 |
  | VillageV4.PlayModeTests | 12 |
  | PersonBootstrap.PlayModeTests | 1 |
  | **合计** | **121** |

- 属性：`explicit=0`、`ignored=0`、`parameterized=0`（PlayMode 用例**无参数化展开**，执行条数应等于发现条数；若宿主回执数量 ≠ 121 或 0 发现，**不得算 PASS**，须按真实输出登记）。
- 管理专项：`tests | where mode == "PlayMode" and fullName contains "ManagementShellPlayTests" | count` → **10**（与既往 `ManagementShellPlayTests` 10 条一致；本包**未改**该文件）。
- 程序集文件：`Library/ScriptAssemblies/` 下 5 个 DLL 均存在（`BattleStage1 434176B @ 14:22:35`、`VillageStage1 129024B`、`VillageV4 100864B`、`ExpeditionV4F 62976B`、`PersonBootstrap 13824B`，后四者 mtime 为 2026-10-22T14:46Z）；最新 PlayMode `.cs` 早于 `BattleStage1.PlayModeTests.dll`，**无“源码新于 DLL”的陈旧编译迹象**。
- 宿主 check 口径（不由本包执行）：**完整 PlayMode、无 filter**（`testMode=PlayMode`，不带 `testAssembly/testNamespace/testClass/testMethod/query`），**不得**偷换为“只跑 Management 类”，也不得用 Editor 内 `EditMode` 结果顶替。

---

## 4. 单槽存档保护（真实路径、当前字节、本轮新增快照）

### 4.1 真实路径与当前字节（`SaveSystem.GetSavePath()` 实调）
```text
savePath = 1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot\rrpg_save_p0.json
save exists=True bytes=13405
sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa
mtimeUtc=2026-10-24T14:33:25.6672726Z
save.tmp exists=False
saveDir=1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot  files=[rrpg_save_p0.json]
```
- 与主开发回读的 `20261024T142654Z` `restoreReadback`（13405B / `4b2b56f6…`）**完全一致**；说明 Edit 全集的 postflight 恢复已被当前真实档印证（本包只读复核，未改写该 manifest）。
- 未使用 HEAD / Git 版本 / 历史 `21075B / 650c45c4…` 备份；历史 650 缺口本包**不恢复、不伪造**。

### 4.2 本轮保护快照（已实际落盘 + 独立回读复核）
| 项 | 值 |
|---|---|
| runID | **`20261024T143611Z`**（真实 UTC；目录预检不存在后 `Directory.CreateDirectory`） |
| run 目录 | `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T143611Z/` |
| 数据文件 | `original-save.bin` — **13405B**（`FileMode.CreateNew` 独占创建，**未覆盖任何既有 run**） |
| 数据 SHA256 | `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` |
| `captureUtc` | `2026-10-24T14:36:11.8809947Z`（bin `mtime=2026-10-24T14:36:11.8848489Z`） |
| manifest | `status=protective-snapshot`、`sourceExisted=true`、`sourceBytes=13405`、`sourceSha256=backupSha256=4b2b56f6…`、`readbackMatchesSource=true`、`readbackVerified=true`、`restored=false`、`pendingRestore=false`、`lastError=null` |
| 回读一致性 | 写入后重新读 `original-save.bin` 重算 SHA256 = 源 SHA256、字节数相同 → `readbackVerified=True`；再与**当前档**重算比对 `matchesSnapshot=True` |
| 对 Play 夹具影响 | `protective-snapshot` 属 `SaveSlotGuard.BlockOnUnfinishedRuns()` **白名单**，**不阻断** Play；本包**未写** `captured`。 |

**边界**：该快照只保护**本轮 Play 运行前**的真实原字节，**不是**历史 650 版本，**不得**用于覆盖其他跑次；本包**未写回存档**，`restored=false` 如实记录，**不冒充“已恢复/已完成”**。runID 登记为 `20261024T143611Z`，供全集结束后**精确恢复**。

### 4.3 保护根状态复核（快照写入后，只读枚举 manifests）
```text
protection dirs=127 | restored=125 | protective-snapshot=2 | unfinished=0 | noManifest=0
protectiveIds = 20261024T052704Z, 20261024T143611Z
newRunFiles = manifest.json, original-save.bin
```
- 与主开发“restored 13405/4b2b56…、34 匹配、无未完成”的回读一致：既有 126 个 run 一个未被改动（`restored=125` + 历史白名单 `052704Z`），本轮只**新增** `20261024T143611Z`。
- 未完成 / 缺失 / 无法解析的 run = **0** → 后续 Play 不会被误阻断；也不会被“清记录求绿”式改写。

---

## 5. Play 侧存档 / 场景 / 资产写风险审计（静态只读）

扫描范围：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/**/*.cs`（49 个文件），静态匹配 `File.Delete` / `File.WriteAllText|Bytes` / `ReadAllBytes` / `SaveSlotGuard` / `AssetDatabase|SaveScene|OpenScene|NewScene|PrefabUtility|EditorSceneManager|CreateAsset|SaveAssets|SetDirty`。

### 5.1 结论
1. **存档写风险 = 高，且无全集级保护**：49 个 Play 文件中 **35 个**在 `SetUp`/`TearDown`/`ResetBridgeAndDeleteSave`/`DeleteSaveFile` 中直接 `File.Delete(SaveSystem.GetSavePath())`（其中 `CharacterGrowthPlayTests` 有 5 处、多数为每例一次）；**7 个**会写档（`BattleVisualFoundation 4`、`BuildingPlacement 3`、`ManagementShell 2`、`BuildingConstruction 1`、`BuildingVisualReplacement 1`、`R3AGrowthTransaction 1`、`VillageVisualPrototype 1`）。
2. **只有 4 个文件**在 `TearDown` 用内存字节回写（`R3AGrowthTransactionPlayTests`、`BuildingVisualReplacementPlayTests`、`VillageVisualPrototypePlayTests`、`ManagementShellPlayTests`）；前 3 个**不是进程安全**：原生 abort / 域重载 / TearDown 提前失败时不恢复，也不产生持久备份。
3. **持久保护只有 `ManagementShellPlayTests.SaveSlotGuard`**（10 条用例）：`Capture()` 先 `BlockOnUnfinishedRuns()`，`CreateNew` 写 `<runID>/original-save.bin` + 回读 SHA256，`Restore()` 只从**本 runID** 已验备份写回并回读；`TearDown` 的 `try/finally` 与 `[OneTimeTearDown]` 兜底**不覆盖**原生 abort（这时 run 会停在 `captured`，下一轮被阻断，符合“不自动覆盖用户档”的设计）。
4. **场景 / 资产 / Prefab 写风险 = 未发现**：上述 API 在 Play 源码中**0 匹配**；Play 夹具不保存 / 打开 / 新建场景，不写 `AssetDatabase`。
5. **非 Assets 的磁盘写**：`BattleVisualFoundationPlayTests` 会写 `Directory.GetCurrentDirectory()/screenshots/WP-D/`（PNG + 同名 `.txt`，`ScreenCapture` 在 batchmode 不产文件时只写说明 txt）；`ManagementShellPlayTests` 追加 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`。二者均在 `Assets` 之外，不改变 34 项基线，但会新增 / 追加磁盘文件（已有先例，非本包产生）。

### 5.2 由风险推出的硬约束（交给宿主）
- 完整 PlayMode 会在**多个夹具间反复删除 / 重写单槽**；若没有 §4 的持久快照，**不能**运行全集。
- 运行结束后**必须**用 `20261024T143611Z/original-save.bin` 精确写回并回读 `13405B / 4b2b56f6…`；**不得**用任一 Play 夹具的 TearDown 内存备份代替，**不得**用历史 650 / HEAD 顶替。
- 若某个 Play 夹具中断导致 `SaveSlotGuard` run 停在 `captured` / `restore-failed`，**不得**删除记录求绿；须人工核对并按该 runID 恢复。
- 本包**不修改**任何 Play 夹具去“加保护”（会改变冻结测试语义，且不在本阶段授权范围）。

---

## 6. 交给宿主的检查单（本包不执行；当前模型不忙等、不自行 `tests-run`）

```text
Mode: PlayMode
Filter: 无（不得设置 testAssembly / testNamespace / testClass / testMethod / query）
Discovery（当前实测）：121 用例 / 5 程序集 / explicit=0 / ignored=0 / parameterized=0
Service preflight: save=13405B/4b2b56f6… ; snapshot runID=20261024T143611Z (protective-snapshot, readbackVerified=true)
```

运行前后必须核：
1. **运行前**：`isPlaying=false`、场景 `isDirty=false`、存档 `13405 / 4b2b56f6…`（与 `20261024T143611Z` 快照一致）。
2. **执行**：宿主 `testMode=PlayMode`、**无 filter**、与 Compiler 等其它工具链**串行**；不得偷换管理类；真实执行条数应 > 0（0 发现不算 PASS）。
3. **运行后**：先只读核当前单槽（存在性 / bytes / SHA256 / 目录内 `.tmp`），再用 **`20261024T143611Z`** 的 `original-save.bin` 精确写回 `SaveSystem.GetSavePath()` 真实路径；回读必须为 `13405B / 4b2b56f6…` 才可登记 `restored`（附 `restoreReadback`、`statusHistory`，`pendingRestore=false`）。
4. **恢复后**：重算 34 项 + 字体 SHA（应 `34/34`、字体 `3c758e16…`），并复核保护根未完成 run = 0；本次 runID 不得被标成 `restored` 之前宣称完成。
5. **失败处理**：任一步不符 → 保留 `original-save.bin` / manifest / 现场输出，**停止并报告**；不改断言、不重试覆盖、不清记录求绿。
6. **历史 650**（`21075B / 650c45c4…`）在授权范围内仍无备份，**不恢复、不伪造**；如需取证须制作人单独授权。

---

## 7. Compiler 264 能力边界（结论：BLOCKED，不冒充 Unity 编译）

- Compiler 工程入口：`Tools/RRPG.ConfigCompiler/`（正式入口 `Tools\RRPG.ConfigCompiler\verify-toolchain.cmd`：`dotnet build` → `dotnet test` → `rrpg-config validate` → Git 规则核对 → 可解析 Unity 编辑器时 Builder + 全量 EditMode，否则必须显示 `SKIPPED` 且不得算通过）。
- 本层可用工具只有 DSH 文件工具、`rql-query`、`rql-schema`、`script-execute`（Editor 内 C#）、`tests-run`（Unity Test Runner）、`screenshot-*`、`unity-tool-list`；**无 dotnet / Compiler / Shell 入口**，且本任务明令禁止 Shell / `Process.Start` / 反射进程 / 用 Unity 编译顶替。
- 因此 **Compiler 264 在本阶段无法执行 → BLOCKED**；历史 R3-A `Compiler 264/264` 是历史回执，**不得**当本轮证据；Unity 编译 ≠ ConfigCompiler 264，两者不得互相冒充。
- 合法后续方式：由 Studio 阶段 check / 宿主按上述受控入口**单独串行**执行，与本工程 Unity 测试**不并行**。

---

## 8. 本包实际产物与边界

| 路径 | 说明 |
|---|---|
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T143611Z/original-save.bin` | 本轮保护快照：13405B / `4b2b56f6…`（`CreateNew`，回读 SHA256 一致） |
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T143611Z/manifest.json` | `status=protective-snapshot`、`readbackVerified=true`、`restored=false`、`pendingRestore=false`；runID 供全集后精确恢复 |
| `Development/StudioRuns/R3B-UI-20261024/full-play-preflight.md` | 本报告（唯一文档写入） |

未做（再次声明）：未运行任何 Unity 测试 / Test Runner / Compiler；未 `AssetDatabase.Refresh`；未改源码 / 测试 / 断言 / 场景 / Prefab / 资产 / 静态配置 / 字体 / Packages / ProjectSettings / Save DTO / SaveVersion / Authority；未 `rql-mutate`；未全局 Save；未 Git 写；未改 `R3B-20261022/workspace-baseline.json`；未改动既有 `protective-snapshot`（`20261024T052704Z`）与任何 `restored` run；未恢复 / 覆盖历史 650；未启停 Editor；未创建额外付费子 Agent。

本报告及上述快照需由主开发专业核验，不代表制作人批准；完整 PlayMode 的实际执行、恢复回读与剩余失败修复属宿主 / 后续包。
