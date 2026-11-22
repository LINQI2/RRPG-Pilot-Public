# R3-B 当前完整 Play 回归保护预检（只读核验 + 当前快照）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`）。
- 日期：**2026-10-25**（Asia/Shanghai；UTC+8；文中证据时间为 UTC）。
- 阶段：**1/1，当前完整 Play 保护与回归预检**。本阶段只核身份/空闲/干净/基线/存档/守卫/刚完成的十一例证据，并为**当前单槽**建立唯一持久保护快照；**不运行测试、不改源码、不改测试、不做大文档收尾**。完整 PlayMode 由宿主 Studio 阶段 check 以**无 filter、串行**方式执行，本文件**不预填**任何完整回归结果或 PASS。
- 结论：**保护预检门禁全部成立，当前快照已建立并回读一致**。本层未发现需要停止的异常；历史 650 缺口保持原状，未触碰。

---

## 1. Pilot 身份与 Editor 空闲

| 项 | 实测 | 来源 |
|---|---|---|
| Unity 版本 | **`2022.3.62t9`** | `rql-query editor` |
| 工程路径 | `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 同上 |
| Editor 状态 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 同上 |
| TestRunner | `testResults` **共 11 条**，恰为刚完成的 `ManagementShellPlayTests` 十一例，无额外/未知投递记录 | `rql-query testResults` |
| 本层测试投递 | **0**。未调用 `tests-run`，未反射启动 TestRunner，未忙等 | 执行记录 |

→ 身份为 Pilot `2022.3.62t9`；原生测试空闲，未发现未知请求。

## 2. 场景干净

| 项 | 实测 |
|---|---|
| 已加载场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`） |
| `isLoaded` / **`isDirty`** | `true` / **`false`** |
| `rootCount` | `5` |

来源：`rql-query scenes`。本阶段未打开/新建/保存任何场景。

## 3. 34 项 workspace 基线与受保护字体（逐项字节 + SHA256）

按 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 的 34 组 `path/bytes/sha256` 逐项重读：

| 项 | 实测 |
|---|---|
| 34 项 baseline | **entries=34 / same=34 / diff=0 / missing=0**（全过程无读取失败） |
| 受保护 TMP 字体 | **14961033B** / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |

来源：只读 `script-execute`（`File.ReadAllBytes` + SHA256，无文件写入）。字体与基线同值。

## 4. 当前单槽存档（唯一写前真相）

| 项 | 实测 |
|---|---|
| 路径（`SaveSystem.GetSavePath()`） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot\rrpg_save_p0.json` |
| 字节数 | **13405** |
| SHA256 | **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`** |
| 当前 mtime（UTC） | **`2026-10-25T07:42:15.9317885Z`** |
| 与要求值 | **完全一致**（13405B / `4b2b56f6…d6aa`） |

来源：反射调用既有 `SaveSystem.GetSavePath` + `FileInfo` + SHA256（只读）。因一致，**未触发任何诊断性覆盖**；若不一致本阶段会停止并只报告来源，不自动覆盖。

## 5. SaveSlotGuard 无未完成 run（快照前）

| 项 | 实测 |
|---|---|
| 根目录 | `Development/StudioRuns/R3B-SaveProtection-20261024` |
| run 总数 | **162** |
| `restored` | **161** |
| `protective-snapshot` | **1**（`20261024T052704Z`，原有） |
| 未完成/未知（`captured` / `restore-failed` / no-manifest / unreadable） | **0** |

来源：只读遍历全部 `manifest.json` 的 `status`。因此新建快照前**不存在未完成 run**，也无需清锁或恢复未知档。

## 6. 刚完成的十一例证据（只读核验，非本层执行）

`testResults` 恰有 11 条 `ManagementShellPlayTests` 结果，全部 `Passed`、`message=null`；对应 console 中出现 **11 组新的 `SAVE-RESTORE`** 与 **2 例 `SCHEDULE-RESTORE`**，以及双端基线/字体回执：

| 项 | 实测 |
|---|---|
| 结果条数 | **11 / 11 Passed**，无 skipped、无 NotRun |
| 新 `SAVE-RESTORE` | **11 组**，runId：`20261025T074206Z`、`…074207Z`、`…074208Z`、`…074210Z`、`…074210Z-2`、`…074211Z`、`…074211Z-2`、`…074212Z`、`…074213Z`、`…074213Z-2`、`…074215Z` |
| 每组终态 | `status=restored`、`sourceBytes=backupBytes=13405`、`sourceSha256=backupSha256=4b2b56f6…d6aa`、`readbackVerified=true` |
| `SCHEDULE-RESTORE` | **2 例**：`20261025T074211Z`、`20261025T074213Z`；均 `restored=true`、`sameInstance=True`、`points=[420,1020,1200]`、`commute=15`、`repeat=30`、`durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}` |
| `BASELINE-POST` | `total=34 same=34 changed=0 problems=0 all-match`（console seq **54889**） |
| `FONT-POST` | `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854 unchanged=True`（seq **54888**） |

证据位置：当前 Editor `console`（seq 约 **54632–54889**）与 `rql-query testResults`。上述 11 条结果为宿主刚完成运行的**历史回执**，本层只读核验，不冒认执行。

## 7. 本阶段新建的当前保护快照（唯一新 run）

按既有合法入口 `SaveSystem.GetSavePath()` 定位当前单槽，使用 **`FileMode.CreateNew`** 新建唯一 run 目录，未覆盖任何旧 run：

| 项 | 实测 |
|---|---|
| **runID** | **`20261025T074636Z`** |
| 目录 | `Development/StudioRuns/R3B-SaveProtection-20261024/20261025T074636Z` |
| `original-save.bin` | **13405B** / **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`** |
| 备份回读 | 字节数一致；SHA256 与源一致；**readbackVerified=true** |
| 源文件捕获后复核 | 字节数与 SHA256 不变；mtime 保持 `2026-10-25T07:42:15.9317885Z`（`sourceMtimeUnchanged=true`） |
| `manifest.json` | **963B** / **`155ebf3d0b93dfbef769eeb69aad1d17ee23514e6f8965dcd853dc95a427f793`** |
| 状态 | **`protective-snapshot`**（`restored=false`、`lastError=null`） |

快照后重扫 guard：

| 项 | 实测 |
|---|---|
| run 总数 | **163** |
| `restored` | **161** |
| `protective-snapshot` | **2**（原有 `20261024T052704Z` + 本次 `20261025T074636Z`） |
| 未完成/未知 | **0** |

→ 旧 protective 快照仍在且未被覆盖；本次快照是新增唯一 run。守卫将 `protective-snapshot` 视为合法终态，不会阻止后续测试。

## 8. 当前完整 Play 发现计数（只读，不预填）

| 查询 | 实测 |
|---|---|
| `tests \| where mode == "PlayMode" \| count` | **122** |
| 分页复核 | `skip 117 \| first 10` 返回 **5** 条，首尾合计与 122 一致 |
| ignored / explicit / parameterized | 全部 `false`（`group-by ignored` = 122 条 `false`） |

→ 当前完整 PlayMode 发现 **122 例**。该数是只读发现值，**不是执行结果**；最终执行数以宿主无 filter 串行 check 的真实发现与结果为准。

## 9. 门禁结论与交宿主

**预检门禁：成立。** 身份 `2022.3.62t9`；Editor 空闲；场景干净；34 项 baseline 全一致；字体 hash 一致；单槽 `13405B / 4b2b56f6…d6aa` 与要求值一致；快照前 `SaveSlotGuard` 无未完成 run；刚完成十一例的 11 组 `SAVE-RESTORE`、2 例 `SCHEDULE-RESTORE`、`BASELINE-POST`/`FONT-POST` 均可只读复核；当前唯一保护快照 `20261025T074636Z` 已建立并回读一致。

交宿主（本层不做）：
- 以**无 filter、PlayMode、串行**执行完整 Play 回归；本层**不启动测试、不偷换专项、不预填 122 结果**。
- 完整 Play 旧测试会删写单槽，运行后以本次 `20261025T074636Z` 为恢复依据；若出现残留/未知状态，由**后续有界包**先核实空闲与残留来源、**先备份残留**，再按本 runID 的 `original-save.bin` 精确恢复。本文件**不预填**该恢复动作或 PASS。
- 运行后仍以测试自身 `BASELINE-POST`、`FONT-POST`、`SaveSlotGuard` manifest 终态与单槽 hash 作为保护面回执。

## 10. 本阶段边界与未做事项

- **未**运行 `tests-run` / TestRunner，**未**忙等；本文件不含完整 Play 的 PASS/FAIL。
- **未** `rql-mutate`；**未** Shell / `Process.Start`；**未** Git 写。
- **未** `SaveAssets` / `SaveChanges` / `SaveDirtyScenes`；**未** `AssetDatabase.Refresh`；**未**启停 Editor。
- **未**改产品源码、测试源码、断言、Scene、Prefab、资产、静态配置、字体、Packages、ProjectSettings、34 项基线、存档本体、既有 `SaveSlotGuard` 历史 run/备份/证据日志。
- 写入仅限：本文件，以及新 run 目录 `20261025T074636Z` 下的 `original-save.bin` + `manifest.json`（均为本任务保护对象）。
- 历史 650 缺口**保留**，未修改、未宣称修复。
- 未安装组件、未改权限、未创建额外付费子 Agent、未访问正式工程或认证目录。
