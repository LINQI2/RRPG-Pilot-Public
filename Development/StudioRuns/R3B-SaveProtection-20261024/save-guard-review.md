# R3-B SaveSlotGuard 持久备份加固 · 存档恢复缺口核对报告

- 仓库/工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一授权目录）
- Unity：已打开的 Pilot `2022.3.62t9`（`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`），未启动/未关闭
- 本阶段：1/1 —— 持久保护当前单槽 + 核对历史漏 Restore 线索
- 本包**未运行任何测试**（未 `tests-run`、未反射启动），**未运行**产品 Save / Load / NewGame，**未恢复**任何历史档，未 `rql-mutate`，未 Shell / `Process.Start`，未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`，未动权限 / Packages / 插件 / 字体 / 正式工程，未 commit / push / stage / 切分支。
- 代码改动范围：`Assets/Tests/PlayMode/ManagementShellPlayTests.cs` 内 `SaveSlotGuard`（测试 helper）+ `[OneTimeTearDown]` 兜底钩子 + 文件头注释。未改任何产品代码 / Save DTO / SaveVersion / 游戏规则 / Studio 基础设施，未重写 102 KB 测试的业务方法。

> **不预填通过**：本报告不含任何 Play 测试 PASS/FAIL；唯一“通过”类结论限于编译与只读核验（§5、§6）。

---

## 1. 历史事实（只读回读，不做二次推断）

证据文件：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`（204 行）。

### 1.1 2026-10-23T15:53 那一轮（异常轮）逐行事实

| UTC | 记录 | 值 |
|---|---|---|
| 15:53:02.527 | `BASELINE-PRE` | total=34 diff=0 all-match |
| 15:53:02.667 | `FONT-PRE` | `3c758e16…` |
| 15:53:02.674 | `SAVE-CAPTURE` #1 | 21075B / `650c45c4d1226ab5606073cd9210545438b37482d2b5de47559a2ca6ecb06ee5` |
| 15:53:03.496 | `ENTRY` #1 | Product entry reached World |
| 15:53:03.504 | `SAVE-CAPTURE` #2 | 13405B / `33879c2a…`（距 ENTRY #1 仅 8 ms） |
| 15:53:03.951 | `ENTRY` #2 | — |
| 15:53:03.952 | `SAVE-CAPTURE` #3 | 13405B / `5b0f9952…`（距 ENTRY #2 仅 1 ms） |
| 15:53:04.390 | `ENTRY` #3 | — |
| 15:53:04.392 | `SAVE-CAPTURE` #4 | 13405B / `a5dc9d91…` |
| 15:53:04.834 | `ENTRY` #4 | — |
| 15:53:04.836 | `SAVE-CAPTURE` #5 | 13405B / `d2a11ea1…` |
| 15:53:05.277 | `ENTRY` #5 | — |
| 15:53:05.413 | `FONT-POST` | `3c758e16…` unchanged=True |
| 15:53:05.565 | `BASELINE-POST` | total=34 same=34 changed=0 problems=0 all-match |

- **该轮 5 次 `SAVE-CAPTURE` 没有任何一条 `SAVE-RESTORE`**；也没有任何 `CASE-A` / `CASE-B` 记录。
- 该轮结束后的槽位内容是第 5 例留下的 13405B（`d2a11ea1…`），05:01 轮从 `13405/4b2b56f6…` 捕获并成对恢复，说明 15:53 轮的写入**确实落盘且未被该轮 guard 还原**。
- 12:25 / 14:50 / 15:03 / 15:37（旧 3 例及 5 例早期版本）与 05:01 / 05:08 轮：每次 `SAVE-CAPTURE` 后都有 `SAVE-RESTORE … matches-before=True`，捕获值均为 `21075/650c45c4…`（05:01/05:08 为 13405/4b2b56f6…）。

### 1.2 可以确定排除的解释

1. **不是进程 aborted**：15:53 轮的 `OneTimeTearDown` 正常执行（`FONT-POST` + `BASELINE-POST all-match` 都在）。若进程被原生中止，这两条不会出现。故“该轮因 abort 而 TearDown 未跑”与日志事实不符。
2. **不是 `[UnitySetUp]` 早退**：5 次 `ENTRY` 全部在 `[UnitySetUp]` 末尾打出，说明 5 次 setup 都执行到了最后一行；NUnit 在测试主体失败时仍会调度 `[TearDown]`，所以 5 次 TearDown 都应被调度。
3. **不是“历史该轮 5 例都 Entry 且有 BaselinePost 所以其实没进夹具”**：反过来，Entry 与 BaselinePost 同时存在恰好证明夹具完整跑过、guard 的 `Capture()` 执行过 5 次。
4. **不是“当前历史档本来就是 650 被写坏”**：capture #2 的 13405B 是典型 `NewGame()+Save()` 产物（与 05:01 轮的恢复值同尺寸），且 4 次捕获 hash 递变，符合“每例真实写档一次”的行为。

### 1.3 可证的根因候选（按证据强度排序，**未能唯一确定**）

- **候选 A（历史版本差异，证据最强）**：15:53 运行时的源码版本里 `[TearDown]` 未调用 guard 的 `Restore()`，或 restore 位于测试主体内。支撑：5 例测试主体持续时间仅 1–8 ms、且完全没有 `CASE-A`/`CASE-B` 记录 —— 当前版本这两例在 `CASE-A/B` 前会做大量真实 UI 操作，耗时远超 8 ms；说明 15:53 用的是**中间版本**（当时文件几经编辑，`5 例`/大小未知），其主体几乎立即失败，TearDown 也不具备当前 `try/finally → Restore()` 结构。
- **候选 B（旧实现的恢复异常先于证据落盘）**：旧 `Restore()` 的 `Evidence()` 写在 `File.WriteAllBytes` **之后**。若写回抛异常（文件被占用 / 共享冲突 / `_bytes` 为 null），则日志里既没有 `SAVE-RESTORE`，文件也不会被还原 —— 与本轮“无 RESTORE 记录 + 下一例捕获到上一例写入”完全一致。这是**当前版本也存在的可观测盲点**，本包已修复（见 §4）。
- **候选 C（静态字段 + 域重载丢 `_bytes`）**：若旧版本把 `_bytes` 放静态字段，测试间发生域重载会使字节丢失，`File.WriteAllBytes(_path, null)` 抛 `ArgumentNullException` → 无 RESTORE、文件保持上一例内容。无法证伪也无法证实（当前版本已改为**每例实例字段**，且 guard 状态只经 manifest 跨域可见）。
- **候选 D（钩子未注册）**：没有证据支持 —— `[TearDown]` 与 `Restore()` 在 05:01/05:08 轮均正常配对。

**结论（证不到的缺口，明确记录）**：能证明“该轮 5 例真实写档且未被还原、进程未 abort、夹具完整收尾”；**不能**在授权 Pilot 范围内证明 15:53 当时磁盘上的源码/DLL 具体形态，因此候选 A/B/C 中哪一个为真**未定**。原因：该轮的测试源码版本、`BattleStage1.PlayModeTests.dll`、NUnit 结果 XML 均不在本包授权可读的 Pilot 范围（也未去 Studio 私有 runtime），且不得用当前文件冒充历史版本。**不以“aborted”一句话归因。**

---

## 2. 当前单槽的精确只读备份（本包实际写入）

- 路径来源：`SaveSystem.GetSavePath()`（真实运行值，只读）
  `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`
- 当前实际状态（本包实测）：`exists=True`，**bytes=13405**，**SHA256=`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**，`mtimeUtc=2026-10-24T05:09:01.0013503Z`（即 05:08 轮恢复后的值）。
- 保护快照（唯一 runID，CreateNew 写入，回读校验一致）：

| 项 | 值 |
|---|---|
| run 目录 | `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T052704Z/` |
| 数据文件 | `original-save.bin` — 13405B |
| 数据 SHA256 | `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` |
| manifest | `manifest.json`（`status=protective-snapshot`、`sourceSha256`、`backupSha256`、`readbackMatchesSource=true`、`captureUtc=2026-10-24T05:27:04.5127685Z`、`pendingRestore=false`、`restored=false`） |
| 回读一致性 | 写入后重新读文件计算 SHA256 = 源 SHA256，bytes 相同 → `True` |

**边界**：该快照是**当前**档（13405B / `4b2b56f6…`），**不是**历史 `21075B / 650c45c4…`；不得用它“恢复历史档”，也不得假设用户档仍是 650。本包未修改、未移动、未删除原档。`status=protective-snapshot` 与 guard 的 `captured/restored` 语义区分，不阻塞后续测试。

### 2.1 授权范围内查找历史 650 备份（只读）

- 扫描根：`Development/StudioRuns`（递归）+ `UnityProject/RRPG1.0game/Assets/Editor/StudioRecoveryBackups`（递归）——**只有这两个已知目录**，未遍历其他目录、未读 Studio 私有 runtime、未用 Git HEAD、未把 `original-font-bytes.bin`（TMP 字体字节，非存档）冒充 save。
- 结果：**24 个文件**逐文件计算 SHA256，**命中 `650c45c4…` 的文件数 = 0**。
- 结论：**Pilot 授权范围内未找到 650 版本原始存档备份**。这不是“全机器无备份”的断言；如需继续找，必须另行授权范围。

---

## 3. 未完成 run 阻断策略（新代码行为）

`SaveSlotGuard.Capture()` 第一步（任何测试写档之前）执行 `BlockOnUnfinishedRuns()`：

1. 枚举 `Development/StudioRuns/R3B-SaveProtection-20261024/*/manifest.json`（**只读**）。
2. `status` ∈ {`restored`, `protective-snapshot`} → 正常；其余（`captured` / `absent` / `restore-failed` / `capture-mismatch` / 缺失 manifest / 无法解析）→ **夹具立即失败**，消息列出所有未完成 runID 与目录，要求人工核对。
3. 阻断时**不自动恢复、不自动删除、不自动改写任何 manifest**（不为求绿清记录）；只在失败消息里指路。
4. 新 run 目录用 `Directory.Exists` 预检 + 自增后缀，`original-save.bin` 用 `FileMode.CreateNew` **独占创建**，因此**绝不覆盖 / 删除**已有 runID 与已有备份。
5. 本包实测：当前该根目录下仅 `20261024T052704Z`（`protective-snapshot`），按上述规则**未完成数 = 0**，不阻断后续测试。

---

## 4. 代码改动（仅测试 helper，diff 摘要）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
- 改动前：102647B / `4610b0a2f24bd08193ec63c35e264904440918bb31325a876ac6a7c36357645b`（主开发固定值；本包在写入前未独立重算，改动后值为本包实测）
- 改动后：**113414B / `b1d2174a8e907394a6015ba8ae1685ae82f42224fdc3a3ea48f0eb831f317b73`**
- 业务方法（6 个用例、CASE-A/B/C 段、34 项基线/字体逻辑、原有断言）**一行未改**；只改 `SaveSlotGuard`（第 461–660 行原 30 行 → 现约 200 行）、新增 `_lastGuard` 字段、`[OneTimeTearDown]` 前置兜底块、文件头注释。

```diff
- SaveSlotGuard.Capture(): 只读 File.ReadAllBytes → 仅内存 + Evidence
- SaveSlotGuard.Restore(): File.WriteAllBytes(_bytes) → Evidence → Assert（Evidence 在写回之后）
+ SaveSlotGuard.Capture():
+   1) BlockOnUnfinishedRuns();                                  // 先阻断，不写盘
+   2) runID = UTC yyyyMMddTHHmmssZ，目录不存在才用（否则 -2/-3…），CreateDirectory
+   3) _bytes = File.ReadAllBytes(GetSavePath()); _beforeHash = SHA256
+   4) 原存在：FileStream(CreateNew) 写 <runDir>/original-save.bin
+      回读 SHA256 == 内存 hash == 当前文件 hash（任一不符 → Assert，夹具不继续写档）
+      原不存在：不造旧档，manifest 记 sourceExisted=false / status=absent
+   5) WriteManifest(captured|absent)（真实路径、存在性、bytes、SHA256、captureUtc、status…）
+   6) Evidence("SAVE-BACKUP" / "SAVE-CAPTURE", runId + backup + manifest)
+ SaveSlotGuard.Restore():
+   1) 未完成 Capture → Assert（禁止凭空写回）
+   2) 已 restored：校验当前文件仍等于捕获 hash → 幂等返回；漂移则用**同一 runID** 备份重写
+   3) source 只取本 runID 的 original-save.bin，回读 hash 与捕获 hash 不一致 → 拒绝写回
+   4) 写回/删除 → 回读 hash → _restored = ok → WriteManifest(restored|restore-failed)
+      → Evidence("SAVE-RESTORE", runId, backup, manifest) → Assert（失败必失败）
+   5) catch：先写 restore-failed manifest + 失败 Evidence，再 throw（不吞异常、不 Assert.Pass）
+ [OneTimeTearDown] 前置：
+   若 _lastGuard 存在且未 restored → 尝试 _lastGuard.Restore()（同 runID 已验备份）
+   成功/失败均写 SAVE-RESTORE-ONETIME 证据；失败仅告警不改写结论；
+   明确注释：进程级 abort 不会执行这里，不构成自动恢复保证。
```

保留不变（复核）：`[UnitySetUp]` 首行仍是 `new SaveSlotGuard().Capture()`（写入 `_lastGuard` 后继续原有 `ResetForTests → LoadScene → NewGame+Save → OnContinue` 流程）；`[TearDown]` 仍是 `try{复位 ForceSaveFailureForTests} finally{Restore()} finally{ResetForTests 仅告警}`。

---

## 5. 恢复权限边界（本包与后续包的硬约束）

1. `Restore()` **只允许**使用**本 runID** 的 `original-save.bin`（且捕获时已回读校验）；不读取、不发现、不施加任何其他 run 的备份。
2. **不自动发现“旧未完成”就覆盖用户当前档**：发现未完成 run 只做阻断 + 人工核对提示；不自动清记录求绿。
3. **绝不覆盖 / 删除未完成备份**（`CreateNew` + runID 不覆盖）；失败时 backup/manifest 必须保留。
4. 本包**未恢复任何历史档**、未移动/删除任何存档；历史 650 的处理属未授权决策，留给制作人/主开发单独安全取证（宿主级恢复）。
5. 进程 abort 情形：只保证**原始字节在磁盘上持久可找**（run 目录 + manifest）；**不宣称自动恢复**。`OneTimeTearDown` 兜底仅覆盖“TearDown 未跑完但夹具仍收尾”，不覆盖原生 abort / 进程被杀。
6. 未读 Studio 私有 runtime、未遍历授权目录以外的任何路径。

---

## 6. 编译与保护核验（本包实际执行，非口头）

| 检查 | 命令/方法 | 结果 |
|---|---|---|
| 前置状态 | `editor \| select isPlaying isPaused isCompiling isUpdating` | `false/false/false/false` |
| 场景 dirty | `scenes \| select name path isDirty` | `SampleScene … isDirty=false`（改前改后均 false，未保存任何场景） |
| 安全刷新 | `script-execute` → `AssetDatabase.Refresh(ForceSynchronousImport)` | `refresh-requested`；此后 `isCompiling=false`、`isUpdating=false` |
| Console Error | `console \| where logType=="Error"` | **0 条** |
| Console 相关警告 | `console \| where message contains "ManagementShellPlayTests" or "error CS"` | **0 条** |
| 测试发现 | `tests \| where fullName contains "ManagementShellPlayTests"` | **6 条 PlayMode**（原 5 + CASE-C，未增删） |
| DLL 新鲜度 | `BattleStage1.PlayModeTests.dll` | mtime `2026-10-24T05:27:55.914Z`，晚于 .cs `2026-10-24T05:27:52.752Z` → **DLL_FRESH=True** |
| 阻断规则自检（只读） | 复算保护根下 run 状态 | `dirs=1`、`unfinished=NONE`（后续测试不被误阻断） |

**编译结论：通过（仅 Windows/Editor 编译层面）。未运行任何测试，运行态仍全部未验证。**

- 34 项基线 / 指定 14 项：本包**未修改** `workspace-baseline.json`、未触碰字体 / Growth / ProjectSettings / Packages / NuGet；未执行测试，故不产出新的 `BASELINE-PRE/POST`。
- `StudioTestResourceScope.cs` / `GrowthDataInitializer.cs` / 既有恢复备份与 Studio 维护修复：未触碰。
- 证据日志 `R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`：本包**只读**，未追加、未清空。

---

## 7. 后续最小单方法 filter（不预填通过）

```text
PlayMode
RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak
```

单跑前的检查清单（由 Studio 阶段 check 执行，不由本报告替代）：

1. 所有打开场景 `isDirty=false`（tests-run 前置要求）。
2. 单跑结束后期望新增一条 `SAVE-BACKUP` + `SAVE-CAPTURE`（含 `runId=`）与一条 `SAVE-RESTORE … matches-before=True`；对应 run 目录 manifest 由 `captured` 变为 `restored`。
3. 若运行 abort：run 目录保留 `status=captured` 的 backup + manifest → **下一轮测试会被阻断**，必须人工核对后再继续（这正是本包要的行为，不得清记录绕过）。
4. 之后再做 6 例回归 + `BASELINE-PRE/POST all-match` + `FONT-PRE/POST unchanged=True`。

---

## 8. 本包 Artifact 清单（实际落盘）

| 路径 | 说明 |
|---|---|
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T052704Z/original-save.bin` | 当前单槽 13405B 保护快照（回读 SHA256 一致） |
| `Development/StudioRuns/R3B-SaveProtection-20261024/20261024T052704Z/manifest.json` | 真实路径 / 存在性 / bytes / SHA256 / captureUtc / status |
| `Development/StudioRuns/R3B-SaveProtection-20261024/save-guard-review.md` | 本报告 |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 仅 SaveSlotGuard / OneTimeTearDown 兜底 / 头注释改动（113414B / `b1d2174a…`） |

- 未运行测试 → 无测试回执；未运行产品流程 → 无 Save/Load/NewGame 回执；未恢复历史档。
- 本报告与上述改动需由主开发专业核验，不代表制作人批准；R3-B 其余 Save/Continue/NewGame、gating、全回归与文档仍待后续阶段。
