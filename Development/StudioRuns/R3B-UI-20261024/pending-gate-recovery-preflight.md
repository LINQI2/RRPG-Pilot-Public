# R3-B pending 门禁 · 中止产物核对与 11 例移交前置（阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（唯一工作区）；Unity 工程 `UnityProject/RRPG1.0game`；Editor `2022.3.62t9`。
- 分支 / HEAD（只读文件）：`.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`；ref = `054b00d3e38fe2e2e142bfdb2502de90f7991030`（与 `workspace-baseline.json` 的 `head` 相同）。**未执行任何 Git 命令，无 Git 写。**
- 本包性质：**只核对中止产物 + 只读门禁核验 + 两处报告事实更正**。未运行任何测试（未 `tests-run`、未反射启动 TestRunner、未忙等）；未 `rql-mutate`；未 Shell / `Process.Start`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 `AssetDatabase.Refresh`；未改产品代码 / 测试 / 场景 / Prefab / 资产 / 配置 / 字体 / Packages / ProjectSettings / 基线 / 存档 / 保护备份；未安装组件 / 改权限 / 创建额外付费子 Agent。Unity 通道只用只读 `rql-query` 与只读 `script-execute`（反射读状态、逐项哈希、目录扫描；无写入、无保存 API）。
- 本包实际写入：本文件 + `pending-event-management-gate-review.md`（§6 中止声明、§7.4 更正）+ `final-self-audit.md`（顶部事后事实更正块）。**未动其它 7 份文档。**

---

## 1. 开工前只读门禁（实测时间 `2026-10-24T15:02:42Z` / `15:02:56Z`）

| # | 项 | 实测事实 | 判定 |
|---|---|---|---|
| 1 | Pilot 身份 / 版本 | `editor.projectPath = D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`；`unityVersion = 2022.3.62t9` | 通过 |
| 2 | 非 Play / 编译空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 通过 |
| 3 | 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`rootCount=5`；无其它已加载场景 | 通过 |
| 4 | TestRunner 无 running | 反射读静态字段：`EditModeLauncher.IsRunning=False`、`PlaymodeLauncher.IsRunning=False`（两类型只暴露 `IsRunning` 字段；未发现 running/pending/request 类静态成员） | 通过（可见层 + launcher 字段） |
| 5 | 无 pending / activeRequest | `testResults` 仍为旧 **121** 条（加入第 11 例前的完整 Play）；`PendingEvent` 结果 **0**；`TestResults/` 目录仅 `BattleStage1.EditMode.results.txt`（2026-10-10）与 `D-onsite-product.png`（14:39Z），**无 `.request` / `.pending` 文件**；Console 无 `Run started` / `Test Run` / `Running tests` 记录 | 通过（无新运行迹象） |
| 6 | Console 错误 | 仅 2 条旧 `snippet.cs` 片段自身错误（`CS0201` @ `22:59:19+08:00` = `14:59:19Z`，即中止包已披露的 2 次 snippet 语法失败），**无项目源码 `error CS`**，无需触发 Refresh | 通过 |

---

## 2. 存档 / 保护 run / 34 项 / 字体（实测）

| 项 | 实测 | 与固定只读口径 |
|---|---|---|
| 当前单槽 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`（`SaveSystem.GetSavePath` 只读 + 文件哈希） | 一致（固定只读 15:01:25Z 同值） |
| 保护 run 总数 | 目录 **137** 个（`R3B-SaveProtection-20261024/**`） | 一致 |
| run 状态 | `restored=136` + `protective-snapshot=1`；`otherStatus=0`、`noManifest=0`、bin/manifest hash 不一致 **0** | **无未完成 run**；未发现中止后新增 `captured` 档 |
| 最新 run | `20261024T143827Z`（restored），**14:39 后无新 run** | 不自动恢复、不重放未知档 |
| 34 项 workspace baseline | `total=34 / same=34 / missing=0 / diff=0`（只读逐文件 SHA256 + bytes 重算） | 一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 一致（= 基线） |

> 结论：存档与保护面处于可安全测试的已收口状态；本包**未**碰任何存档 / 备份 / manifest / 基线文件。

---

## 3. 代码完整性与“新鲜度”

### 3.1 文件实测（与固定只读快照逐项一致）

| 文件 | 实测 |
|---|---|
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **289142B / `96982624fea6e6976a0c539fc309ed62842d6d54d69be94e35e3c84f0750cd43`**，mtime `2026-10-24T14:58:55.7858967Z`，共 **4415** 行 |
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | 16082B / `7da8fbb0…8f2f66`（快照同值） |
| `Assets/Scripts/Save/SaveSystem.cs` | 45001B / `97f48237…8d8045`（快照同值） |
| `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs` | 41759B / `7670c283…4efeff9e`（快照同值） |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | mtime **`2026-10-24T14:59:24.0038652Z`**、450048B → **晚于**源文件 mtime（14:58:55Z），程序集新鲜 |

### 3.2 源码完整性（只读通读结论，未改一行）

- **11 例发现**：`tests | where fullName contains "ManagementShellPlayTests"` 返回 **11** 条 PlayMode；新用例 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` 在列表内。类内 `[UnityTest]` 属性亦为 **11** 个（1366/1480/1574/1620/1724/2053/2243/2733/3162/3901/4095 行）。
- **新增段完整**：第 4095–4405 行为新用例，末尾有 `EVENT-GATE … repeat=AlreadyCaughtUp projection=consistent` 证据收尾与闭合；第 4407–4413 行为 `CompleteTestProject3rd` 辅助方法；第 4414/4415 行为类与命名空间闭合（**无截断、无悬空括号**）。
- **恢复链完整**：`IsolateActivityOpportunitiesForFixture`（386–429）捕获原日程并只读校验替换；`RestoreActivityScheduleAfterFixture`（431–456）由 `[TearDown]`（1178–1185）优先调用并校验同一实例 / 机会点 / 通勤分钟；`finally` 中继续 `ForceSaveFailureForTests` 清理与 `SaveSlotGuard.Restore()`；`OneTimeTearDown` 另有同 runID 兜底恢复。
- **断言形态**：真实 Recognition 触发 pending（`CompleteTestProject3rd`），Direct `WaitingForInteraction`、管理导航 / ESC / Pause 不绕过、`WorldBusy` 零写（职责 / 占用 / 施工 / 资源 / 批次 / Clock / 单槽 hash）、合法选择只完成一次、Return `CatchUpComplete` + 重复 `AlreadyCaughtUp`、投影一致；**全部为强断言，未预填 PASS、未削弱既有用例**。
- **结论：无需修任何编译残缺，也未触发 `AssetDatabase.Refresh`。**

---

## 4. 中止产物核对（如实标注未完成）

| 中止包声称 | 实际 | 结论 |
|---|---|---|
| 新增第 11 例并“编译通过” | 文件 / 程序集 / 11 例发现均与声称一致（见 §3） | 已落盘、源码完整、程序集新鲜 |
| “同步纠正 9 份文档”（`pending-event-management-gate-review.md` §6） | **未执行**：该 9 份文档仍为旧口径（自审索引仍“管理 Play 10 例”“V15 未测”“产品级同帧竞态保留”；执行记录 §5 仍“事件 pending 专项未测”；状态快照 / 日志 / 交接 / 地图 / 规格附记均未更新） | **中止未完成**，已在 §6 就地声明；不得当作已纠正 |
| “exactly-once 已验证”（同文件 §7.4） | 该用例从未运行（`testResults` 121 条、无 `PendingEvent`） | 已在 §7.4 更正为“尚未验证” |
| 测试结果 | 无新回执；`testResults` 仍是加入第 11 例前的 121 条 | **11 例全部未由本执行层运行**，不写通过 |

> 处置边界：不重放实现、不改 9 份文档、不恢复未知档；文档更正留待独立小包重做。

---

## 5. 移交（唯一剩余验证动作）

- 前置条件已满足：Pilot 身份/版本正确、非 Play 编译空闲、TestRunner 无 running/pending/activeRequest、场景干净、存档已恢复、137 run 无未完成、34/34 与字体一致、源码完整且程序集新鲜、11 例已发现。
- **交 Studio 宿主 check**：以**短类名 `ManagementShellPlayTests`** 整类 **PlayMode 串行**执行（11 例），不按方法过滤拆散类内共享夹具、不并行 Test Runner。
- 本执行层**不调用 `tests-run`、不反射启动 TestRunner、不忙等**；通过与否以宿主真实回执为准，不预填。
- 失败时先区分“真实产品门禁行为”与“夹具前置”（Test Project 10 天完成 / Recognition 阈值 3 / 活动机会隔离 / claim 干扰），并按真实证据登记；通过后仍不据此关闭整包或宣称制作人验收。
- 文档层面的 9 份同步与“管理 Play 独立回执口径”更正，须由后续独立小包在重新只读核对后执行。

---

## 6. 本包边界声明

- 未访问授权目录外路径 / 其它工作区 / 认证目录；未安装组件、未改权限、未创建额外付费子 Agent / Workflow / Goal。
- 未运行模型测试；未 `rql-mutate`；未全局保存；未 Git 写；未改保护 14 / 34 项与备份历史档。
- Unity 通道仅只读：`rql-query`（editor / scenes / console / tests / testResults）、`script-execute`（反射读 `IsRunning`、文件 SHA256、目录与 manifest 扫描、时间戳；全部无写入）。无 `AssetDatabase.Refresh`、无保存调用。
