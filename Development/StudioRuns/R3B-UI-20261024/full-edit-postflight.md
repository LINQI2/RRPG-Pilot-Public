# R3-B Pilot 完整 EditMode 回归 · 完整 Edit 后核对与精确快照恢复（阶段 1/1）

- 仓库 / 工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（本任务唯一授权目录）
- Unity 工程：`UnityProject/RRPG1.0game`（已连接的 Pilot Editor，**未启停**）
- 本阶段范围：**只做存档保护恢复 + 只读核对 + 本报告**。未改源码 / 测试 / 断言 / 场景 / Prefab / 资产 / 静态配置 / Packages / ProjectSettings / 字体；未 `AssetDatabase.Refresh`；未运行任何 `tests-run` / Test Runner / 反射启动 / Compiler 进程；未 `rql-mutate`；未 Shell；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 Git 写（commit/push/stage/切分支）；未 Load/Save 产品；未恢复历史 650 档；未覆盖任何其它 run / 快照；未用 HEAD 顶替；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件 / 改权限 / 创建额外付费子 Agent。
- 本阶段唯一写入：`Assets` 之外的**存档文件本身**（精确写回本轮已验快照，见 §4）、本轮 `20261024T142654Z/manifest.json` 的状态与证据字段、本报告。**未新增**任何 run 目录、未改 34 项基线文件。

> **不预填通过**：本文件不含任何新的测试 PASS/FAIL。测试结果引用的是宿主已完成、主开发已核的持久回执（§5），本包**未重跑** Edit，也未启动任何测试。

---

## 1. 结论速览

| 项 | 实测值 | 结论 |
|---|---|---|
| Unity 版本 | `2022.3.62t9` | 与 Pilot 要求一致 |
| Editor 空闲 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | 空闲，未启停 |
| TestRunner | `EditModeLauncher.IsRunning=false`、`PlaymodeLauncher.IsRunning=false` | 无 running / pending / activeRequest |
| 场景 | `SampleScene`，`Assets/Scenes/SampleScene.scene`，`isDirty=false`，`rootCount=5`，`isLoaded=true` | 干净 |
| 本轮快照 | `20261024T142654Z/original-save.bin`：**13405B / `4b2b56f6…`** | 与要求值**完全一致**，读回重算 |
| manifest 一致性 | `readbackVerified=true`、`backupSha256` 与文件实测 hash 相同、`sourcePath` 与 `SaveSystem.GetSavePath()` 归一化后相同 | 一致 |
| 测试后单槽状态 | **不存在**（`exists=false`）；存档目录 `files.count=0`、无 `.tmp`；目录 mtime `2026-10-24T14:31:46.8389287Z` | 被 Edit 夹具删除（符合预检 §6.1 风险判断） |
| 恢复方式 | 用本 runID 的已验 `original-save.bin` 精确写回；回读 `13405B / 4b2b56f6…` | **恢复成功且回读校验通过** |
| 状态迁移 | `protective-snapshot` → `restored`（`2026-10-24T14:33:25.6672726Z`） | 已准确登记，附回读 |
| 34 项基线（恢复后重算） | `total=34 same=34 missing=0 problems=0 all-match=True` | 一致 |
| TMP 字体（34 项之一） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线同值 |
| SaveSlotGuard 未完成 run | `runs=126`：`restored=125`、`protective-snapshot=1`（`20261024T052704Z`）、**unfinished=0**、无 manifest 缺失 | 不阻断后续 Play |
| 宿主 Edit 回执（引用） | 无 filter EditMode **1513/1513 Passed、0 failed、0 skipped**、耗时 `2:53.578`；`scope-restored|font|0|632`；外层 `interrupted` 保留 | 主开发已核；本包**未重跑** |
| Compiler 264 | 本层无合法运行入口 | **BLOCKED**（能力边界，见 §6） |

---

## 2. 只读预检（写任何东西之前）

### 2.1 版本 / Editor / TestRunner
- `script-execute` 只读片段：`unity=2022.3.62t9`、`dataPath=D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets`、`isPlaying=False`、`isCompiling=False`、`isUpdating=False`。
- `rql-query editor | select isPlaying isPaused isCompiling isUpdating unityVersion` → `false|false|false|false|2022.3.62t9`。
- TestRunner 反射只读：`UnityEditor.TestTools.TestRunner.EditModeLauncher.IsRunning=False`、`PlaymodeLauncher.IsRunning=False`；相关静态成员扫描仅发现 `EditModeRunner.m_RunningTests`（实例字段，无静态活动请求）、`CommandLineTest.RunData.isRunning`（实例字段，无命令行运行），**未发现 running / pending activeRequest**。
- 结论：具备“只读核对 + 精确写回存档”的前置条件；**本包未启停 Editor、未触发编译、未 Refresh**。

### 2.2 场景与 Console
- `scenes | select name path isDirty rootCount` → `SampleScene | Assets/Scenes/SampleScene.scene | false | 5`；`isLoaded=true`。
- `console | where message contains "error CS" | count` → **0**。
- `console | where logType == "Error"` 现存为**测试用例自身预期错误日志**（如 `[AdventureManager] Return Settlement 未成功…`、`PoolNotFound`、`Return Catch-up 在 Day 中途失败`），来自宿主已完成的完整 Edit 运行，**不是项目编译错误**；本包未新增任何 Console 条目（本包所有片段均正常返回）。

---

## 3. 快照与 manifest 重算（不引用口头结论）

| 检查 | 方法 | 结果 |
|---|---|---|
| 快照文件存在 | 只读 `File.Exists` | `true` |
| 快照字节数 | `FileInfo.Length` | **13405**（=要求值） |
| 快照 SHA256 | 逐字节重算 | **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**（=要求值） |
| 快照 mtime | `LastWriteTimeUtc` | `2026-10-24T14:26:54.5136237Z`（= `captureUtc`，未被改写） |
| manifest `readbackVerified` | 读取字段并与实测 hash 对照 | `true`，且 `sourceSha256 == backupSha256 == 实测 hash` |
| manifest `sourcePath` | 与 `SaveSystem.GetSavePath()` 实调值归一化比较（`\`→`/`） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，**一致** |
| 快照来源唯一性 | 仅使用 `20261024T142654Z/original-save.bin` | 未读取历史 `21075B/650c45c4…`、未用 HEAD、未动其它 run |

---

## 4. 测试后单槽核对与精确恢复（本阶段核心）

### 4.1 测试后（恢复前）实测
- `SaveSystem.GetSavePath()` 实调 → `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`。
- `save.exists=false`；`savePath+".tmp"` 不存在。
- 存档目录 `1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot`：`exists=True`、`files.count=0`（递归）、目录 `mtimeUtc=2026-10-24T14:31:46.8389287Z`、`ctimeUtc=2026-10-10T12:50:50.0790181Z`。
- **判读**：完整 EditMode 全集结束时，单槽文件已被夹具删除（与 `full-regression-preflight.md` §6.1 的“无字节备份直接删除”风险一致）。目录内**没有任何测试后残留字节**（0 文件、无 `.tmp`），因此：
  1. **不存在可 CreateNew 备份的“测试后版本”**——没有“先把测试后版本备份再写回”的对象，如实记录为缺失，未伪造备份；
  2. **不存在“测试结束后用户真实新进展”的 mtime/内容证据**（文件不存在，目录内也没有任何新写入文件），因此**不属于** “来源不明的新进展” 风险情形，允许按授权写回本轮已验快照。

### 4.2 精确写回
- 写回前再次校验源快照：`pre.snapshotBytes=13405`、`pre.snapshotSha=4b2b56f6…`（不符则中止，不写）。
- 写回动作：仅 `File.WriteAllBytes(savePath, original-save.bin bytes)`（与测试夹具同一落盘机制），**未调用** `SaveSystem.Save()` / `Load()` / `NewGame()`，**未 Load 产品流程**，**未写任何其它 run / 快照**。
- 回读回执：

```text
pre.saveExists=False
post.exists=True post.bytes=13405
post.sha=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa
post.mtimeUtc=2026-10-24T14:33:25.6672726Z
readback.equalsSnapshot=True | readbackVerified=True
```

- 恢复后再次只读复核：`GetSavePath` → 同一路径；`save.exists=True bytes=13405 sha=4b2b56f6…`；`save.readbackIsSourceSha=True`；`manifest.sourcePathMatchesGetSavePath=True`。

### 4.3 manifest 状态迁移（`protective-snapshot` → `restored`）
文件：`Development/StudioRuns/R3B-SaveProtection-20261024/20261024T142654Z/manifest.json`
- `status`: `protective-snapshot` → **`restored`**
- `restored`: `false` → **`true`**；`pendingRestore`: `false`
- `restoredUtc` / `restoreVerifiedUtc`: **`2026-10-24T14:33:25.6672726Z`**（写回回执实测）
- `updatedUtc` 同步为该时刻
- 新增 `restoreReadback`：`{ exists:true, bytes:13405, sha256:4b2b56f6…, bytesEqualSnapshot:true, readbackVerified:true }`
- 新增 `postRunObserved`：恢复前 `sourceExistedBeforeRestore=false`、`saveDirectoryFileCount=0`、`saveDirectoryMtimeUtc=2026-10-24T14:31:46.8389287Z`、观测时刻以 [快照 captureUtc, 写回时刻] 区间如实记录（未编造精确观测时刻）
- 新增 `statusHistory`：`protective-snapshot → restored`（时间、原因、回读 hash）
- 新增 `restoredBy`：`R3-B full EditMode postflight (post-edit restore only; no test run)`
- `captureUtc` / `sourceBytes` / `sourceSha256` / `backupBytes` / `backupSha256` / `readbackVerified` / `sourcePath` 均**原样保留未被改写**；`lastError=null`。

### 4.4 失败边界（本次未触发，记录约定）
- 若写回或回读失败：保留 `original-save.bin` 与失败证据，manifest 不得改为 `restored`，不得清记录求绿，**停止并报告**；本次未出现该情形。
- 未恢复历史 650（`21075B/650c45c4…`）：授权范围内仍无其备份，本包**不恢复、不伪造**。
- 未覆盖任何其它快照 / run：`20261024T052704Z` 仍是 `protective-snapshot`，历史 `restored` run 一个未动（见 §5.3 计数：`restored=125`、`protective-snapshot=1`）。

---

## 5. 恢复后核对

### 5.1 34 项保护基线 + 字体（恢复后重算，非引用）
- 依据 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 的 `files[]`（每项 `bytes` + `sha256`），逐项读文件重算，**不写文件**。
- 结果：`post-restore baseline: total=34 same=34 missing=0 problems=0 all-match=True`。
- 字体项（34 项之一，制作人指定 14 项子集）：`UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` → `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，与基线同值。
- 说明：34 项含制作人指定 14 项子集；NuGet/Packages 属 34 项但不属 14 项（沿用既往更正）。本包**未改**基线文件、未改任何受保护文件。

### 5.2 场景 / Editor / Console 复核
- `scenesDirty=SampleScene:False`；`editor: isPlaying=False isCompiling=False isUpdating=False`；`testrunner: EditModeLauncher.IsRunning=False PlaymodeLauncher.IsRunning=False`。
- `console | where message contains "error CS" | count` → **0**。本包无项目级编译错误、无 Refresh、无全局保存。

### 5.3 SaveSlotGuard 未完成 run 复核（只读枚举 manifests）
`Development/StudioRuns/R3B-SaveProtection-20261024/*/manifest.json`：

```text
runs.total=126 | restored=125 | protective-snapshot=1 | unfinished=0 | noManifest=0
bad=NONE
```

- 唯一 `protective-snapshot` = `20261024T052704Z`（历史保护快照，白名单，不阻断）；其余 125 个 `restored`。
- **未完成 / 缺失 / 无法解析的 run = 0** → `SaveSlotGuard.BlockOnUnfinishedRuns()` 不会阻断后续 Play；本包**未改**任何其它 manifest。

### 5.4 宿主 Edit 回执与本包角色
- 持久回执（主开发已核，本包只读复核 `testResults`）：无 filter EditMode **1513 个结果、`Passed=1513`、非 Passed=0**（`testResults | where status == "Passed" | count` → 1513；`where status != "Passed"` → 0 条），耗时 `2:53.578`，`scope-restored|font|0|632`；**外层 `interrupted` 保留未改**。
- 本包**没有**重跑 Edit、没有启动测试；仅读取已持久化的结果。

---

## 6. 1387 发现与 1513 运行的区别（口径澄清）

| 项 | 数值 | 来源 | 说明 |
|---|---|---|---|
| EditMode **发现**用例数 | **1387** | `tests \| where mode=="EditMode" \| count`（本包复核）；`explicit=false` 计数同为 1387；`parameterized=true`=**59**；程序集 `distinct assembly`=**24** | 这是 Test Runner **方法级发现**口径：一个 `[TestCase]`/`[InlineData]` 方法在发现层只算 1 条 |
| EditMode **执行**用例数 | **1513** | 宿主无 filter 运行回执（持久 `testResults`） | 参数化用例在运行时按 `[InlineData]`/`[TestCase]` **展开成多条**，故执行数 > 发现数 |
| 差额 | `1513 − 1387 = 126` | — | 59 个参数化方法共展开出 126 条额外用例（同一方法的多个数据行），**不是**新增/遗漏用例，也**不是**发现口径错误 |

- 结论：**1387 是发现口径，1513 是参数化展开后的执行口径**，两者互不矛盾；**不得**把 1513 当作“发现数”写进文档，也**不得**把 1387 当作“少跑了 126 条”判为回归。若后续发现数变化，必须以真实 `tests` 查询为准并重新登记。
- 本包未改变任何测试源码 / 用例属性，故本包前后发现数应保持 1387（本次复核确认仍为 1387）。

---

## 7. Compiler 264 能力边界（结论：BLOCKED，不冒充）

- 事实（沿用并复核预检 §3）：Compiler 工程入口为 `Tools/RRPG.ConfigCompiler/`（README 正式入口 `verify-toolchain.cmd`：`dotnet build` → `dotnet test` → `rrpg-config validate` → Git 规则核对 → 可解析 Unity 时 Builder + 全量 EditMode，否则必须显示 `SKIPPED` 不得算通过）。
- 本层可用工具只有 DSH 文件工具、`rql-query`、`rql-schema`、`script-execute`（Editor 内 C#）、`tests-run`、`screenshot-*`、`unity-tool-list`；**无 dotnet / Compiler / Shell 入口**，且本任务明令禁止 Shell / `Process.Start` / 反射进程 / 用 Unity 编译顶替。
- 因此 **Compiler 264 在本阶段无法执行 → BLOCKED**；历史 R3-A `Compiler 264/264` 是历史版本回执，**不得**当本轮证据；静态计数（`[Fact]=225` + `[InlineData]=39`）只作发现口径对照，**不是执行结果**。
- 合法后续方式：由 Studio 阶段 check / 宿主按 `Tools\RRPG.ConfigCompiler\verify-toolchain.cmd`（或等价的 `dotnet build` + `dotnet test`）**单独串行**执行，与本工程 Unity 测试**不并行**；Unity 编译 ≠ ConfigCompiler 264，两者不得互相冒充。

---

## 8. 完整 Play 与后续边界（本包不执行）

- 完整 PlayMode 回归**不属于本包**，本包**未启动**任何 Play / Edit 测试、未启停 Editor；后续由 Studio 自动继续并另包授权。
- 顺序前置已就绪：单槽已恢复并回读一致（13405B / `4b2b56f6…`）、`SaveSlotGuard` 未完成 run = 0、场景干净、Editor 空闲。
- 若后续 Play 夹具再次写档，必须仍走 `SaveSlotGuard` 的 `Capture/Restore`（本 runID 独立、`CreateNew` 不覆盖），**不得**用本轮快照去覆盖其他跑次，也不得清记录求绿。
- 历史 650 档缺口（`21075B/650c45c4…` 在授权范围内无备份）**不由本包闭环**；如需取证 / 恢复，须制作人单独授权。

---

## 9. 本包实际产物与边界

| 路径 | 说明 |
|---|---|
| `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json` | 已精确写回本轮已验快照：13405B / `4b2b56f6…`，回读校验一致（未 Load/Save 产品） |
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T142654Z/manifest.json` | `status=restored`、`restored=true`、`restoredUtc`、`restoreReadback`、`postRunObserved`、`statusHistory`；原捕获字段未改写 |
| `Development/StudioRuns/R3B-UI-20261024/full-edit-postflight.md` | 本报告（唯一新增文档） |

未做（再次声明）：未运行任何 Unity 测试 / Test Runner / Compiler；未 `AssetDatabase.Refresh`；未改源码 / 测试 / 断言 / 场景 / Prefab / 资产 / 静态配置 / 字体 / Packages / ProjectSettings / Save DTO / SaveVersion / Authority；未 `rql-mutate`；未全局 Save；未 Git 写；未改 `R3B-20261022/workspace-baseline.json`；未改既有 `protective-snapshot`（`20261024T052704Z`）与任何 `restored` run；未碰历史 650 备份；未用 HEAD；未启停 Editor；未创建额外付费子 Agent。

本报告及上述恢复动作需由主开发专业核验，不代表制作人批准；完整 Play 回归、Compiler 264 的实际执行与剩余失败修复属后续包。
