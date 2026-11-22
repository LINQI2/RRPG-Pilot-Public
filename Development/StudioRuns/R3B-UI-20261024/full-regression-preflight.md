# R3-B Pilot 完整 EditMode 回归 · 保护预检报告（阶段 1/1）

- 仓库 / 工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（本任务唯一授权目录）
- Unity 工程：`UnityProject/RRPG1.0game`（当前已连接的 Pilot Editor，未启停）
- 本阶段范围：**只做预检与文档**。未改源码 / 测试 / 断言 / 场景 / Prefab / 资产 / 配置 / 字体 / Packages / ProjectSettings；未 `AssetDatabase.Refresh`；未运行任何 `tests-run` / Test Runner / 反射启动 / Compiler 进程；未 `rql-mutate`；未 Shell / `Process.Start`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 Git 写（commit/push/stage/切分支）；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件 / 改权限 / 创建额外付费子 Agent。
- 本阶段唯一写入：本文件 + 新增一个 `protective-snapshot` run 目录（见 §4，`FileMode.CreateNew`，未覆盖任何既有 run）。

> **不预填通过**：本文件不含任何测试 PASS/FAIL。下文“通过”仅指**只读预检项**（身份、空闲、场景、34 项 SHA、字体、存档保护状态）成立；“完整 EditMode”尚未运行，结果由宿主 Studio 阶段 check 串行产出。

---

## 1. 预检结论速览

| 项 | 实测值 | 结论 |
|---|---|---|
| Unity 版本 | `2022.3.62t9`（`Application.unityVersion`） | 与 Pilot 要求一致 |
| Editor 状态 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | 空闲，可进入宿主串行测试 |
| 场景 | `SampleScene`，`Assets/Scenes/SampleScene.scene`，`isDirty=false`，`rootCount=5` | 干净（`tests-run` 前置满足） |
| 34 项保护基线 | `entries=34 same=34 missing=0 problems=0 all-match=True`（逐项重算 bytes+SHA256） | 一致 |
| 受保护 TMP 字体 | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（34 项之一） | 与基线同值 |
| 单槽存档真实路径 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`（`SaveSystem.GetSavePath()` 实调） | 存在 |
| 当前单槽内容 | `13405B` / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，`mtimeUtc=2026-10-24T14:24:33.4521394Z` | 与既有保护快照**字节一致** |
| 既有保护快照 | `R3B-SaveProtection-20261024/20261024T052704Z`：13405B / `4b2b56f6…`（本次读回 `original-save.bin` 重算一致） | 匹配当前档，未被覆盖 |
| 存档 run 总览 | 126 个 manifest：`restored=124`、`protective-snapshot=2`、未完成 **0** | 无 `captured/absent/restore-failed` 残留 |
| 最新 10 个 run | 9 个 `restored` + 本次新增 1 个 `protective-snapshot`，全部 `13405B / 4b2b56f6…` | 最近真实运行均已恢复原字节 |
| Compiler（264） | 测试工程存在（`net10.0` + xUnit）；静态计数 `[Fact]=225`、`[InlineData]=39` → 朴素合计 **264** | 只能静态核对；**本层无合法运行入口 → BLOCKED**（见 §3） |
| 完整 EditMode 发现 | **1387** 个用例 / **24** 个 EditMode 程序集；`explicit=0`、`ignored=0`、`parameterized=59` | 无 filter 全集已核 |
| 管理专项 | Edit `ManagementProjectionTests` **17/17** 已发现；Play `ManagementShellPlayTests` **10** 已发现 | 与 R3-B Pilot 现状（10/10、17/17）一致 |
| 存档写风险 | **高风险，且是“删除 + 重写”**：至少 **34** 个 EditMode 测试文件在 `SetUp`/`TearDown` 直接 `File.Delete` 单槽（或经 `*TestKit.DeleteSave()`），均**无字节备份**；另有 4 个夹具只在 `TearDown` 回写字节 | 已按授权建立本轮保护快照（§4） |

---

## 2. 身份 / 空闲 / 场景 / 保护面（真实只读回执）

### 2.1 版本与工程
- `script-execute` 只读片段返回：`unity=2022.3.62t9`、`dataPath=D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets`、`repo=D:\AI_Agent_Lab\rrpg_pilot\repo`。
- 34 项基线 `root` 字段 = `D:\AI_Agent_Lab\rrpg_pilot\repo`，与本任务授权目录、当前工作目录一致；`complete=true`。**本包未改基线文件**。

### 2.2 34 项 SHA 与字体（逐项重算，不引用历史口头结论）
- 依据：`Development/StudioRuns/R3B-20261022/workspace-baseline.json` 的 `files[]`（每项 `bytes` + `sha256`）。
- 方法：`script-execute` 只读片段按 JSON 逐项读文件、重算 SHA256 与字节数，**不写文件**。
- 结果：`total=34 same=34 missing=0 problems=0 all-match=True`；字体项 `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`。
- 说明：34 项含制作人指定 14 项（TMP 字体 1 + Growth 12 + ProjectSettings 1）**子集**；NuGet/Packages 属 34 项但不属 14 项（沿用既往更正）。测试源码 `Assets/Tests/**` 不在 34 项内。

### 2.3 场景与 Editor
- `editor | select isPlaying isPaused isCompiling isUpdating` → `false|false|false|false`。
- `scenes | select name path isDirty rootCount` → `SampleScene | Assets/Scenes/SampleScene.scene | false | 5`。本包未保存、未打开、未切换任何场景。

### 2.4 Console
- `console | where logType == "Error"` 现存 4 条，全部是**既往会话的 `Script / Execute` 片段编译/运行错误**（`snippet.cs(28,27): error CS1501`、`__CodeExecSnippet.__Run` NRE），不是项目程序集编译错误，也不来自本包（本包 3 次片段均正常返回/中途 2 次自身类型错误，已即时修正）。
- 未发现项目级 `error CS`；`isCompiling=false`、`isUpdating=false`。**本包未触发任何（重）编译**。

---

## 3. Compiler 工具链能力核对（结论：BLOCKED，不冒充 Unity 编译）

### 3.1 事实
- 工程入口：`Tools/RRPG.ConfigCompiler/`（`src/RRPG.ConfigCompiler`，`net10.0`，`AssemblyName=rrpg-config`；`tests/RRPG.ConfigCompiler.Tests` 为 xUnit，`Microsoft.NET.Test.Sdk 17.12.0 + xunit 2.9.2`）。
- 文档入口：`Tools/RRPG.ConfigCompiler/README.md` —— 正式用法是 `rrpg-config validate|build|package-check`，**可重复验证入口 `Tools\RRPG.ConfigCompiler\verify-toolchain.cmd`**（`dotnet build` → `dotnet test` → `rrpg-config validate` → Git 规则核对 → 若可解析 Unity 编辑器再跑 Builder + 全量 EditMode，编辑器不可用则明确 `SKIPPED`）。
- R3-A 执行记录（`Docs/DevelopmentSpecs/2026-10-10_RRPG_V17-R3-A_Person-Detail_Execution-Record.md`）中的 `Compiler264/264` 是 **R3-A 交付版本的历史回执**，不是本轮证据。
- **本层可用工具**：DSH 文件工具、`rql-query`、`rql-schema`、`script-execute`（Unity Editor 内 C# 片段）、`tests-run`（Unity Test Runner）、`screenshot-*`、`unity-tool-list`。
- `unity-tool-list` 实测 18 个工具，**无 dotnet / Compiler / 配置编译工具**（`package-*`、`rql-*`、`screenshot-*`、`script-*`、`tests-run`、`unity-tool-list`、`tool-set-enabled-state`）。
- `UnityProject/RRPG1.0game/Assets/Editor/StaticConfig/StaticConfigBuilder.cs` 只在 `Tools/RRPG.ConfigCompiler/.build-state/` 写 `builder.stage.txt`，**不是** Compiler 本体入口，也没有在 Editor 内启动 `dotnet` 的受控菜单（`Process.Start` 被任务明令禁止，且本层无 Shell）。

### 3.2 结论与后续合法运行方式
- **本阶段 Compiler 无法执行：BLOCKED**。禁止以 `Process.Start`、反射进程、Shell 或 Unity 编译顶替；**Unity 编译 ≠ ConfigCompiler 264**，不得互相冒充。
- 后续合法运行方式（由 **Studio 阶段 check / 宿主** 串行执行，与本工程 Unity 测试**不并行**）：
  1. 首选：`Tools\RRPG.ConfigCompiler\verify-toolchain.cmd`（一次覆盖 build/test/validate/Git 规则；Unity 不可解析时 Builder 与 EditMode 段必须显示 `SKIPPED`，不得当作通过）。
  2. 最小等价：`dotnet build Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj` → `dotnet test Tools/RRPG.ConfigCompiler/tests/RRPG.ConfigCompiler.Tests/RRPG.ConfigCompiler.Tests.csproj`（期望 264、0 失败/0 跳过；若数量不再是 264，按真实输出登记，不改断言、不补历史）。
  3. 首次 restore 可能需 NuGet 网络；`validate`/`build` 正常不联网。
- 本包静态计数仅作**发现口径对照**：`[Fact]=225`、`[Theory]=11`、`[InlineData]=39`，`225+39=264` 与历史回执数量吻合——这是**静态代码计数，不是执行结果**。

---

## 4. 单槽存档保护（真实路径、比对、本轮新增快照）

### 4.1 真实路径与当前字节
- 调用链：`SaveSystem.GetSavePath()`（Assembly-CSharp，公共静态，只读反射调用）→
  `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`
- 当前：`exists=True`、`bytes=13405`、`sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、`mtimeUtc=2026-10-24T14:24:33.4521394Z`。

### 4.2 与既有保护快照比对（不覆盖旧快照）
- 既有 `20261024T052704Z`：`status=protective-snapshot`、`sourceBytes=13405`、`sourceSha256=4b2b56f6…`；本包读回其 `original-save.bin` 重算：`13405B / 4b2b56f6…` → **与当前档字节一致**。
- 最近 10 个 run（按 runId 排序，`restored` 9 个 + 本轮新增 1 个）全部为 `13405 / 4b2b56f6…`；存档 run 根下 `restored=124`、`protective-snapshot=2`、**未完成=0**。
- 未发现与历史 `21075B / 650c45c4…` 相关的任何新进展被覆盖；历史 650 版本仍**未在授权范围内找到备份**（沿用 `R3B-SaveProtection-20261024/save-guard-review.md` §2.1），本包**不恢复、不伪造**历史档。

### 4.3 本轮新增保护快照（已实际落盘 + 回读校验）
| 项 | 值 |
|---|---|
| run 目录 | `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T142654Z/` |
| 数据文件 | `original-save.bin` — 13405B（`FileMode.CreateNew` 独占创建，未覆盖任何既有 run） |
| 数据 SHA256 | `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` |
| manifest | `manifest.json`：`status=protective-snapshot`、`sourceSha256`、`backupSha256`、`readbackMatchesSource=true`、`readbackVerified=true`、`captureUtc=2026-10-24T14:26:54.5136237Z`、`restored=false`、`pendingRestore=false` |
| 回读一致性 | 写入后重新读文件重算 SHA256 = 源 SHA256，字节数相同 → `True` |
| 对 Management 夹具影响 | `status=protective-snapshot` 属 `SaveSlotGuard.BlockOnUnfinishedRuns()` 的**白名单**，**不阻断**后续 Play 夹具；本包**未写 `captured`**，因此不会阻断 |

**边界**：该快照是**当前档**（13405B / `4b2b56f6…`），内容与 `20261024T052704Z` 相同但 runID 独立；**只允许**用它恢复本轮 Edit 全量运行前原字节。它不是历史 650 版本，也不得覆盖其他跑次；本包**未写回存档**，`restored=false` 如实记录，**不冒充“已恢复/已完成”**。

---

## 5. 完整 EditMode 发现口径（无 filter 全集）

- 查询：`tests | where mode == "EditMode"`。
- **总数 1387**；程序集 **24** 个（逐个程序集用 `where assembly == "…" | count` 单独查询，非按分组顺序推断）：
  | 程序集 | 条数 | 程序集 | 条数 |
  |---|---|---|---|
  | BattleStage1.Tests | 247 | VillageStage1.Tests | 205 |
  | Event.Tests | 126 | AutonomousSelection.Tests | 77 |
  | ActivityLifecycle.Tests | 72 | EventProduct.Tests | 72 |
  | StaticConfig.Tests | 63 | SocialFeedback.Tests | 57 |
  | Encounter.Tests | 48 | ExpeditionProgression.Tests | 47 |
  | VillageResource.Tests | 47 | ExpeditionJourney.Tests | 44 |
  | ExpeditionV4F.Tests | 43 | PersonIdM1.Tests | 39 |
  | SocialFoundation.Tests | 39 | ProfessionM6.Tests | 32 |
  | ProfessionM5.Tests | 23 | PersonVisual.Tests | 21 |
  | VillageV4.Tests | 21 | ProfessionM3.Tests | 17 |
  | ProfessionM2.Tests | 16 | ProfessionM4.Tests | 16 |
  | BattlePersonExp.Tests | 8 | PersonBootstrap.Tests | 7 |

  逐项相加 = **1387**，与 `count` 总数一致（`BattleStage1.Tests` 247 与 `VillageStage1.Tests` 205 两项均单独复算过）。
- 属性：`explicit=0`、`ignored=0`、`parameterized=59`（参数化用例执行时按 `[InlineData]`/`[TestCase]` 展开，**最终执行条数可能大于 1387**，以宿主回执为准，不得把“数量变化”当失败或当通过）。
- 对照：R3-A 的 `EditMode 1495/1495` 是**历史版本**，与当前发现 1387 **不同**，本包不引用为证据、也不把差异解释为回归（源码与用例集在此期间已变）。
- 管理专项：`ManagementProjectionTests` = **17** 条（含本包之前新增的命令/劳动/WorldBusy 用例）；`ManagementShellPlayTests` = **10** 条 PlayMode。二者均与 R3-B Pilot 现状（Edit 专项 17/17、管理 Play 10/10）**在发现层面**一致；执行结果仍待宿主。
- 宿主 check 口径：**完整 EditMode、无 filter**（`testMode=EditMode`，不带 `testNamespace/testClass/testMethod/query` 过滤），**不得**偷换为“只跑 Management 类”。若最终 0 发现或数量异常，按既有规则**不算 PASS**。

---

## 6. 全集写风险与恢复边界（本阶段最关键结论）

### 6.1 风险：完整 EditMode 会**删除或重写**单槽存档
EditMode 夹具有两条模式，均**不经过** `SaveSlotGuard`（该 guard 只在 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`）：

1. **无备份直接删除**（`SetUp`/`TearDown` 都删）——最危险，用户档会**直接消失**：
   - `Assets/Tests/EditMode/PersonBootstrap/PersonBootstrapContractTests.cs`（19–37：`SetUp` 即 `DeleteSaveFile()`，`TearDown` 再删）
   - `.../PersonBootstrap/PersonBootstrapCapabilityTests.cs`（27–36）、`.../PersonBootstrap/PersonBootstrapSaveTests.cs`（34–44）
   - `.../PersonIdM1/DuplicateRecruitIsolationTests.cs`（16–32）、`.../PersonIdM1/DuplicateCardIdBindingKeyTests.cs`（18–34）、`.../PersonIdM1/PersonIdSaveMigrationTests.cs`（17–33）、`.../PersonIdM1/FoundationIdentityCleanupTests.cs`（34、50）
   - `.../BattlePersonExp/BattlePersonExpChainTests.cs`（38、42、44–46）
   - `.../VillageV4/VillageV4ExpeditionIdentityTests.cs`（19、27）、`.../VillageV4/VillageV4ReturnConsequenceTests.cs`（21、29）、`.../VillageV4/VillageV4CapabilityGrantTests.cs`（27、34、37–40）
   - `.../ExpeditionV4F/`：`ExpeditionReturnIntegrityTests`（31、41）、`WP2ExpeditionProfessionLoopTests`（39、48）、`ExpeditionReturnGuardTests`（25、34）、`ProjectExpeditionMutualExclusionTests`（17、26）、`ExpeditionCatchUpFailureTests`（20、30）、`ExpeditionCatchUpDayTests`（16、26）、`ExpeditionReturnSessionTests`（14、22）、`ExpeditionReturnOrderSaveTests`（14、23）、`IntradaySaveIntegrationTests`（6–7）、`IntradayWorkIntegrationTests`（15、23）——经 `V4FTestKit.DeleteSave()`（`V4FTestKit.cs` 211–214）
   - `.../EventProduct/VillageEventFlowContractTests.cs`（59、66、78–81）
   - `.../ProfessionM3|M4|M5|M6/*`（`ProfessionM3TestKit.cs` 141–144、`ProfessionM4TestKit.cs` 131–134、`ProfessionM5TestKit.cs` 156–159、`ProfessionM6TestKit.cs` 270–273，各 `*Save*Tests` 在 SetUp/TearDown 调用）
   - `.../Village/VillageSaveCompatibilityTests.cs`（29、34–37）、`VillageProjectSaveTests.cs`（32、37–40）、`VillageSocialFactSaveTests.cs`（34、45–48）、`VillageAssignmentSaveTests.cs`（33、38–41）、`VillageSocialResponseSaveTests.cs`（28、38–41）
   - `.../BattleLegacyProductPathCleanupTests.cs`（461–484：读取真实路径后 `File.Delete`）
   - 说明：`SaveSystem.NewGame()`（`SaveSystem.cs` 225–241）与 `Load()`（108–223）**只改内存态/只读文件**，本身不落盘；真正落盘只有 `SaveCore` 的 `File.WriteAllText(GetSavePath(), json)`（第 98 行）与测试里的显式 `File.WriteAllText/Delete`。因此 `PersonDetailProjectionTests`、`ProductHomeFoundationTests`、`HomeProductWhiteboxTests` 等调用 `NewGame` 的夹具**不直接写档**，但它们的同名/同目录的 `*Save*` 用例会把内存态写入同一单槽。

2. **有字节备份但只在 `TearDown` 恢复**（写入/失败/中断即可能留下新字节，恢复本身也可能抛错）：
   - `.../AutonomousLife/SocialSaveIntegrationTests.cs`（30 捕获、42–59 恢复）
   - `.../AutonomousLife/Selection/SelectionIntegrationTestKit.cs`（76–77 捕获、90–110 恢复）
   - `.../AutonomousLife/ActivityLifecycle/ActivityIntegrationTestKit.cs`（61 捕获、68–78 恢复）
   - `.../AutonomousLife/SocialFeedback/SocialFeedbackIntegrationTests.cs`（70 捕获、77–87 恢复）
   **这 4 处仍不是进程安全**：原生 abort / 域重载 / `TearDown` 失败时不会恢复（与 Play 侧 15:53 缺口同类，见 `R3B-SaveProtection-20261024/save-guard-review.md` §1）。

### 6.2 结论
- **完整 EditMode 全集运行会破坏当前用户单槽**（最可能的结果是文件被删除；也可能被 `NewGame+Save` 内容替换）。**若没有 §4.3 的快照，本阶段不允许运行全集**。
- 现有 Edit 侧“保护”只有 4 个夹具的 TearDown 字节回写，**不覆盖删除路径、不覆盖中断**；本包**不修改**任何 Edit 夹具去加保护（不在本阶段授权范围，且会改变冻结测试语义）。

### 6.3 恢复边界（交给后续包 / 宿主，本包不执行）
1. 运行全集**之前**已具备本轮快照 `20261024T142654Z`（13405B / `4b2b56f6…`，回读校验通过）。
2. 运行全集**之后**，必须由后续授权包用**该 runID 的 `original-save.bin`** 写回 `SaveSystem.GetSavePath()` 的真实路径，并回读校验 `bytes=13405` + `sha256=4b2b56f6…`；只有校验通过才能登记“已恢复”。
3. 恢复**不得**使用历史 `21075B / 650c45c4…` 备份，**不得**用 Git/HEAD 版本顶替，**不得**覆盖他人 runID。
4. 若恢复失败：**保留证据并停止**，不得清记录、不得改 manifest 求绿，不得宣称已恢复。
5. 顺序约束：全集 Edit 结束 → 恢复并核验 → 才可进入任何 `ManagementShellPlayTests`（Play 侧 `SaveSlotGuard.Capture()` 若发现档不存在会记 `status=absent` 并**阻断后续 Play**，这是既有设计，必须靠恢复解决而不是改 guard）。
6. 存档历史缺口（650 原档不可得）**不由本包闭环**；如要求取证/恢复，需制作人单独授权。

---

## 7. 交给宿主的检查单（本包不执行）

```text
Mode: EditMode
Filter: 无（不得设置 testAssembly / testNamespace / testClass / testMethod / query）
Discovery（当前实测）：1387 用例 / 24 程序集 / explicit=0 / ignored=0 / parameterized=59
```

运行前后必须核：
1. 运行前：`isPlaying=false`、场景 `isDirty=false`、存档 `13405 / 4b2b56f6…`（与 `20261024T142654Z` 快照一致）。
2. 结果：真实执行条数 > 0；失败/跳过按真实输出登记（**0 发现不算 PASS**）。
3. 运行后：先恢复存档并核验（§6.3），再核 34 项 + 字体 SHA 未变。
4. 本轮 Console 不得出现项目级 `error CS`；`tests-run` 的现场错误按原样登记，不重试覆盖。
5. Compiler 264 由宿主按 §3.2 的受控入口**单独串行**执行；本层不代跑、不冒充。

---

## 8. 本包实际产物与边界

| 路径 | 说明 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261024/full-regression-preflight.md` | 本报告（唯一文档写入） |
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T142654Z/original-save.bin` | 本轮保护快照（13405B，回读 SHA256 一致） |
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T142654Z/manifest.json` | `status=protective-snapshot`、`readbackVerified=true`、`restored=false` |

未做（再次声明）：未运行任何 Unity 测试 / Test Runner / Compiler；未 `AssetDatabase.Refresh`；未改源码 / 测试 / 断言 / 场景 / Prefab / 资产 / 静态配置 / 字体 / Packages / ProjectSettings / Save DTO / SaveVersion / Authority；未 `rql-mutate`；未全局 Save；未 Git 写；未改 `R3B-20261022/workspace-baseline.json`、未改既有 `protective-snapshot`/`restored` run 目录、未碰历史备份。

本报告需由主开发专业核验，不代表制作人批准；完整 EditMode / Compiler 的实际结果、存档恢复与剩余失败修复属后续包。
