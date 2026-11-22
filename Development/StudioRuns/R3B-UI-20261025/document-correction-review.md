# R3-B 三文档纠正与证据复核报告（2026-10-25）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot）。本包只做文档纠正与只读核对，**不运行 Unity、不刷新、不测试、不 `rql-mutate`、不 Shell、不 Git 写、不全局保存**。
- 本包实际写入：`Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`、`Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md`、`Development/StudioRuns/R3B-UI-20261024/full-play-postflight.md`、本报告。**未改**产品 / 测试 / 资产 / manifest / 基线 / 证据日志 / 存档 / 备份，未更新另 6 份索引文档。
- 2026-10-25 附记（路径修复包）：本报告 §3/§4/§5 的“当前位置与已处置”状态已更新；残留备份两文件仅移动目录、字节不变。该包另写 `residue-location-repair.md`。
- 结论口径：Compiler 已由 **BLOCKED** 更正为 **Passed（宿主回执）**；整包仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**，不宣称 Accepted。

---

## 1. 逐文件 diff 摘要

### 1.1 `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`
1. 顶部新增 **“2026-10-25 当前事实更正”** 10 条，覆盖 Compiler、122/122、11/11、首次导航产品修复、撤职边界、第二 slot、claim、存档、PARTIAL、历史中止。
2. 首部结论行：Compiler 由 BLOCKED 改为 2026-10-25 真实通过；整包 PARTIAL 原因改为视觉/输入、第二 slot 真实 Gateway、claim 施工等未测。
3. §1 表：Play 回执 121→**122/122 `5eb0ad4c`**；管理 Play 10→**11/11 `7f600ef9`** 并保留独立 **10/10 `0fd4b031`**；Compiler 行改为 **264/264**；存档行改为 2026-10-25 `20261025T074636Z` 恢复 + residue；保护 run 行改为 **175 目录 = 174 带 status（173 restored + 1 protective）+ 1 residue**；34 项补 494 项 `changedProtected=[]`。
4. §3 `HomeProductShell.cs` 行注明 `54a6c399` 已产品修复且旧 SHA 为修复前值；`ManagementShellPlayTests.cs` 行改为 **11 例 / 294535B / `cc55cb40…`**。
5. §4 矩阵：V2→122/122；V4→11/11；V5→`074636Z` + residue；V6→175 目录并标注 residue 误判；V10/V11/V15 改为已通过（V11 注明产品修复）；V13→**Passed**；V16/V17 维持未测。
6. §4.1 历史：`111850Z` 行补“产品后由 `54a6c399` 修复”；新增 `48d8820f` aborted 与 9 文档纠正未执行。
7. §5.1 改为“执行/贡献即停，旧 arrival 下一边界清理”；§5.4 事件 pending 与 Compiler 改为 Passed。
8. §6：删除“产品级同帧竞态保留”，新增 residue 被 `BlockOnUnfinishedRuns()` 误判风险。
9. §7：移除已完成 Compiler/事件 pending，改列视觉、claim、撤职实时、第二 slot、residue 处置、进程级保护、Git。
10. §8 增加 2026-10-25 只改 3 文档说明；§9 更新引用口径、无哈希工具、未重算 TRX SHA。

### 1.2 `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md`
1. 头部新增 2026-10-25 事实块（Compiler、122/122、11/11、首次导航、撤职/第二 slot/claim、存档、PARTIAL、历史保留）。
2. §2.3 Play 测试 10→**11 例**，补第 11 例与 TearDown 顺序。
3. §3 原“不修产品首次导航”行改为当轮边界 + 后续 `54a6c399` 已修。
4. §4.2 回执表：Play **122/122 `5eb0ad4c`**；管理 Play **11/11 `7f600ef9`** + **10/10 `0fd4b031`**；Compiler **264/264**；存档/保护 run 更新。
5. §4.3 补 2026-10-25 `074636Z` 写回与 `075219Z-residue`。
6. §5 更新：事件 pending 已通过、第二 slot 仍未测、claim 不加 busy、撤职边界、首次导航已修、Compiler 已通过、residue 误判。
7. §6 无“剩余最小动作”改为已完成后只列视觉、claim、撤职实时、第二 slot、residue、进程级保护、Git。
8. §8 区分 2026-10-24 自审与 2026-10-25 文档纠正边界。

### 1.3 `Development/StudioRuns/R3B-UI-20261024/full-play-postflight.md`
1. 顶部新增 2026-10-25 当前事实块（Compiler 264、122/122、11/11、当前存档/residue、整包 PARTIAL）。
2. §1 “Compiler 264”行标注“当时 BLOCKED；当前 Passed”；§9 标题与正文标注“2026-10-24 当日能力边界，2026-10-25 已通过”。
3. §7.1 字体哈希错字更正：`…ad26fc80bc…` → **`…ad26fc00bc…`**（真实值 `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`）。
4. 文末附记：§9 的 BLOCKED 仅为 2026-10-24 当日能力边界；2026-10-25 已完成 264/264，保留历史不改写。

---

## 2. 原始 TRX 计数与 Compiler 证据（只读复核，未重跑）

证据目录：`Development/StudioRuns/R3B-Compiler-20261025/`。

| 项 | 实测/登记值 | 来源 |
|---|---|---|
| TRX 结果汇总 | `ResultSummary outcome="Completed"` | `compiler.trx` 第 1601 行 |
| **原始 Counters** | `total=264`、`executed=264`、`passed=264`、`failed=0`、`error=0`、`timeout=0`、`aborted=0`、`inconclusive=0`、`passedButRunAborted=0`、`notRunnable=0`、`notExecuted=0`、`disconnected=0`、`warning=0`、`completed=0`、`pending=0` | `compiler.trx` 第 1602 行 |
| `tests.log` | 失败 **0**、通过 **264**、跳过 **0**、总计 **264**，耗时 7s | `tests.log` 第 5 行 |
| `host-result.json` | `tests.exitCode=0`、`validate.exitCode=0`、`protectedCount=494`、`changedProtected=[]` | `host-result.json` |
| `validate.log` | `结果：PASS errors=0 warnings=8`；8 条均为 `WB004_SHEET_SKIPPED` 导航 sheet；`Mirror 未写入` | `validate.log` 第 65-126 行 |
| TRX SHA256（**报告记录值，未重算**） | `587d9dd1c1dd2c990fafb1a1b1c2188985759b8314bef4b704574dc4d506b0f5` | `REPORT.md` 第 12 行 |
| 宿主结果 SHA256（同上） | `2690af3eff291dd72c8b3516811b931ea6cf05139c3346c5244149b4f8500ee8` | `REPORT.md` 第 13 行 |
| 受限首轮 | 首次受限环境 vstest 未能连接 testhost，运行中止；**不是业务测试失败**；日志保留 `restricted-test-attempt.log` | `REPORT.md` 第 7 行 |
| 保护面 | 运行前后 494 项 dirty/untracked 文件 SHA 未变 | `host-result.json` / `REPORT.md` 第 10 行 |

> 本包无独立哈希工具，**没有重算 TRX SHA**；`587d9dd1…b0f5` 只作为 `REPORT.md` 的登记值引用。原始 TRX/日志未改写。

---

## 3. SaveSlotGuard 对 `075219Z-residue` 的只读核对

### 3.1 事实
- 残留目录（**本报告时点的历史路径**）：`Development/StudioRuns/R3B-SaveProtection-20261024/20261025T075219Z-residue/`
  - **2026-10-25 已处置**：整目录受控重命名至 `Development/StudioRuns/R3B-UI-20261025/full-play-residual-20261025T075219Z/`，旧路径不存在；`original-save.bin` `13402B / 4d2562a8…e23206f25b`、`manifest.json` `974B / 6df49773…2fa1022d` 移动前后 SHA256 完全一致（证据：`residue-location-repair.md`）。
  - `original-save.bin`：13402B（与 manifest `backupBytes` 一致）。
  - `manifest.json`：`kind="residue-backup"`、`runId="20261025T075219Z-residue"`、`relatedProtectiveSnapshotRunId="20261025T074636Z"`、`restored=false`、`readbackVerified=true`，**无 `status` 字段**。
- 保护根：`Assets/Tests/PlayMode/ManagementShellPlayTests.cs:818`
  `ProtectionRootRelative = "Development/StudioRuns/R3B-SaveProtection-20261024"`。
- 枚举逻辑：同文件 `:954-977` `BlockOnUnfinishedRuns()`：
  - `:959` 遍历保护根下**所有子目录**；
  - `:962-966` 无 `manifest.json` → `(no-manifest)`；
  - `:968-970` `JsonUtility.FromJson<ManifestStatus>` 读取 `runId/status`；
  - `:971` 仅 `status == "restored" || status == "protective-snapshot"` 才 `continue`；
  - `:972` 其余（含 `status == null`）加入 `unfinished`；
  - `:974-976` `Assert.AreEqual(0, unfinished.Count, ...)` 失败，**阻断新测试**。

### 3.2 结论
**会误判。** 该 residue 目录位于保护根内，manifest 虽存在但无 `status`；`ManifestStatus.status` 解析为 `null`，不匹配白名单，因此被登记为 `20261025T075219Z-residue(unreadable)`，下一次 `Capture()` 调用 `BlockOnUnfinishedRuns()` 时会因断言失败而阻断测试。

- 该判断只依据**源码与 manifest 文本**，本包未运行测试、未触发 `Capture()`、未移动/修改备份。
- **2026-10-25 后续处置（已执行）**：按“保留备份 + 移出保护根”方案，整目录移入 `R3B-UI-20261025/full-play-residual-20261025T075219Z/`；移动后只读枚举保护根 = `174` 目录（`restored=173` + `protective-snapshot=1` `052704Z`，`no-manifest=0`、`unreadable=0`），该误判已消除。未改 helper 白名单、未改任何 manifest 状态、未删除备份。
- `full-play-current-postflight.md` §6/§7 当时所说“另 1 份 residue 不计入 guard 终态”只对其**手工状态普查**成立；对 `SaveSlotGuard.BlockOnUnfinishedRuns()` **不成立**，两者不能混用（该处已由 2026-10-25 路径修复包在同一文件 §4/§6/§8 更正）。
- 未验证/未做（**文档纠正包当时**）：未实际运行 `Capture()` 观察断言文本；未核 residue 之外的 174 份 manifest 的逐份内容（只按 `status` 行与目录清单核对）；未重算任何文件 SHA。**2026-10-25 路径修复包**已补：用纯 .NET SHA256 重算 residue 两文件与 34 项基线，并按 helper 同等口径只读枚举保护根（见 `residue-location-repair.md`），仍未运行 `Capture()`。

### 3.3 处置建议（本包不执行）
1. 由后续有界包在保留 `original-save.bin` 与 manifest 的前提下，给 residue 一个守卫认可且语义准确的终态（例如扩展白名单 `kind=residue-backup`，或把 residue 移出保护根到 `R3B-UI-*` 证据目录）；两种做法都需改测试 helper 或移动备份，**须单独授权**。
   - **2026-10-25 已按第二种方案执行**：只移动目录，未改 helper、未改 manifest；移动前后字节/SHA256 一致。
2. 在处置前不要运行依赖 `SaveSlotGuard.Capture()` 的测试，否则会被该 residue 阻断；这不是存档损坏，也不代表用户新进展。
3. 不得删除 residue、不得改写其 manifest 求绿；历史 `650` 缺口仍保留。

---

## 4. 未验证 / 证据等级（如实登记）

| 项 | 本包状态 |
|---|---|
| Compiler 264/264 | 只读核对本地 TRX/日志；**未重跑**；TRX SHA **未重算** |
| Edit 1513/1513 `f116b56e` | 主开发已核宿主回执；本包未读取 Studio 私有 runtime，未重跑 |
| 管理 Edit 17/17 `f8d40b09` | 同上 |
| 管理 Play 独立 10/10 `0fd4b031` | 同上（仓库 `pending-event-management-gate-review.md` 有登记） |
| 管理 Play 单例 `ba019b1c` 1/1 | 主开发已核宿主回执；本包未在本地 runtime 读到该记录，未重跑 |
| 最新独立 11/11 `7f600ef9` | 主开发已核宿主回执；本包未在本地 runtime 读到该记录，未重跑 |
| 完整 Play 122/122 `5eb0ad4c`、`1:50.7921181` | 主开发已核宿主回执；本包只读引用 `R3B-UI-20261025/full-play-current-postflight.md`，未重跑 |
| 当前单槽 13405B / `4b2b56f6…` | 2026-10-25 位置修复包已用只读 SHA256 重算复核：`13405B` / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`（未读写档） |
| 首次导航产品修复 `54a6c399` | 依据既有审查文档登记；本包未重读产品源码/SHA |
| 视觉 1920/1280、滚动/拖拽、物理输入 | **未测**，保持 PARTIAL |
| 第二 slot 真实 Gateway、claim 持有者加入施工 | **未测**，保持 PARTIAL |
| 当前工作树代码 SHA / Git index/status | 本包无 Git/哈希通道，**未核验** |

---

## 5. 下一步

1. **residue 守卫误判已处置（2026-10-25 路径修复包）**：保留备份并移出保护根；未改 helper 白名单、未改任何 manifest 状态、未运行测试/未触发 `Capture()`。守护根现为 `174` 目录全部 `restored`/`protective-snapshot`。证据：`residue-location-repair.md`。
2. 视觉/输入验收包：1920×1080、1280×720 实画面，滚动拖拽，物理鼠标/滚轮，镜头 pan/zoom，缺字补烘。
3. 第二 slot：仅在配置 Authority / 公共 API 授权后另立包；当前不改 `BuildingConfigDatabase.asset`、不伪造。
4. Activity claim 持有者加入施工的 N-0 记录型用例；撤职 Play 实时同分钟与跨日边界补测。
5. `SaveSlotGuard` 进程级缺口（out-of-process 兜底）评估。
6. 另 6 份索引文档由下一包按本包事实同步；Git 只读定稿与提交/发布另行授权。

> 本报告及 3 份纠正后的文档需由主开发专业核验，不代表制作人批准；整包仍 **PARTIAL / Not Committed / Not Published**。
