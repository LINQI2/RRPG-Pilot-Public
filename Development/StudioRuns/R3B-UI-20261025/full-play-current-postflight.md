# R3B 完整 Play 后单槽精确恢复 · Postflight（2026-10-25）

执行者：R3B 阶段执行层（本包只做存档保护/精确恢复，未运行任何测试、未改源码、未整理 9 文档）。
授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`。写入仅限：本 run 恢复、残留备份/manifest、本报告。

## 1. 预检实际状态（只读）

| 项 | 实测 | 结论 |
|---|---|---|
| 宿主身份 | `2022.3.62t9`，`projectPath=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 一致 |
| Editor 状态 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，非 Play、非编译 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | 干净 |
| `testResults` | 122 条，`group-by status` = `Passed|122`，无其它状态 | 上一轮 122 例全部 Passed（宿主结果，非本执行者运行） |
| 原生 TestRunner 空闲（可见层） | 无 pending/active 标记；`console` 近 1000 条中 `Run started` / `Test Run` = 0 条；证据日志在 07:48:33 后无追加 | 无未完成运行 |
| 34 项基线 | `workspace-baseline.json` 逐项重算：`entries=34`、`problems=0`（逐文件 bytes+SHA256 全部一致） | all-match |
| 受保护字体 | `14961033B` / `3c758e16…c404854` | 与基线一致 |
| SaveSlotGuard 全部 run | 174 个 manifest：`restored=172`、`protective-snapshot=2`（`20261024T052704Z` 历史快照、`20261025T074636Z` 本轮快照），`captured/pending` 0 | 无未完成 run |
| 本轮 11 例恢复 | `[SAVE-RESTORE]` 11 条（07:48:24.846–07:48:32.837，runId `074824Z`、`-2`、`074825Z`、`074827Z`、`-2`、`074828Z`、`-2`、`074829Z`、`074830Z`、`-2`、`074832Z`），全部 `matches-before=True` | 11/11 成立 |
| 两例日程恢复 | `[SCHEDULE-RESTORE]` 07:48:28.853、07:48:30.745，均 `restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30` | 两例成立 |
| 本轮 BASELINE/FONT post | 07:48:33 `[FONT-POST] unchanged=True`；07:48:33 `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match`（宿主本轮证据日志尾部，本包只读引用） | 通过 |

## 2. 本轮唯一快照独立重算

`Development/StudioRuns/R3B-SaveProtection-20261024/20261025T074636Z/original-save.bin`

- 文件：`13405B`，SHA256 `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`（与 manifest `source/backup` 字段一致）。
- `sourcePath` 归一化（`Path.GetFullPath`）后 = `1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot\rrpg_save_p0.json`，与 `SaveSystem.GetSavePath()` 完全相等（`sourcePathMatchesGetSavePath=True`）。
- 快照 `status=protective-snapshot`、`restored=false`、`readbackVerified=true`，恢复前未写用户档。

## 3. 当前单槽判定（恢复前）

- 实测：`13402B` / `4d2562a833aa1d7a301e7da34bf943a8e66e7920734faa6117dbb5e23206f25b`，`LastWriteTimeUtc=2026-10-25T07:49:30.606Z`。
- 该 mtime 落在宿主本轮运行窗口内（1:50.792 → 约 07:47:44Z 起，`finished=07:49:34.644Z`）；内容与快照 `13405B/4b2b56f6…` 不同；全库 174 份既有备份中无任何一份同 hash（`otherBackupsWithSameHash=0`），即不是任何已捕获的合法状态，属本轮测试尾部写档残留。
- 运行前快照（07:46:36）之后、测试开始前无用户操作证据；判定：**本次测试残留，允许恢复**。

## 4. 残留留证（CreateNew 唯一目录）

- 目录（**历史/创建时路径，保留不改**）：`Development/StudioRuns/R3B-SaveProtection-20261024/20261025T075219Z-residue/`（创建前确认不存在，`File.Copy(..., overwrite:false)`）。
- **2026-10-25 当前位置**：`Development/StudioRuns/R3B-UI-20261025/full-play-residual-20261025T075219Z/`（整目录受控重命名移出守卫根；两文件字节与 SHA256 移动前后完全一致，详见 `residue-location-repair.md`）。
- `original-save.bin`：`13402B` / `4d2562a8…e23206f25b`，回读一致。
- `manifest.json`：`kind=residue-backup`、`relatedProtectiveSnapshotRunId=20261025T074636Z`、`sourceLastWriteTimeUtc=2026-10-25T07:49:30.6063290Z`、`restored=false`、`readbackVerified=true`。
- 本执行前不存在该残留备份目录（准确记录：absent → 已新建）。

## 5. 精确写回与回读

- 用本 runID 快照字节覆盖单槽（`File.Copy(snapshot, save, overwrite:true)`），未使用产品 Save/Load/NewGame、未启停 Editor、未 Refresh、未全局保存。
- 回读：`13405B` / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，`readbackBytes=13405`，`readbackMatches=True`；文件 mtime 由复制保留为 `2026-10-25T07:46:36.048Z`（快照文件时间）。
- 本轮 manifest 已更新：`status=restored`、`restored=true`、`readbackVerified=true`、`restoredUtc`、`restoreSource`、`restoredSavePath/Bytes/Sha256`、`preRestoreSaveBytes/Sha256`、`residueBackupRunId`；**原 capture 字段（captureUtc/sourcePath/sourceBytes/sourceSha256/sourceMtimeUnchanged/backup*/capturedBy/note）全部保留**，新增 `statusHistory`（`protective-snapshot` → `restored`）。
- 历史 `20261024T052704Z` 快照仍为 `protective-snapshot`（合法终态），历史 `21075B / 650c45c4…` 缺口保留，未恢复历史档、未改基线、未清失败记录。

## 6. 恢复后复验（只读）

| 项 | 实测 |
|---|---|
| 单槽 | `13405B` / `4b2b56f6…d6aa`（与快照一致） |
| 34 项基线 | `entries=34`、`problems=0` |
| 受保护字体 | `14961033B` / `3c758e16…c404854` |
| SaveSlotGuard manifest | 174 份：`restored=173`、`protective=1`（`052704Z`），未完成 0；本轮残留备份 `kind=residue-backup`（无 status 字段）已移出保护根至 `R3B-UI-20261025/full-play-residual-20261025T075219Z/`，不再被 `BlockOnUnfinishedRuns()` 枚举（移动前：175 目录 / 174 带 status + 该 residue `(unreadable)`） |
| 场景 / Editor | `SampleScene.isDirty=false`；`isPlaying/isCompiling/isUpdating=false`，`selection=[]`，`2022.3.62t9` |
| `testResults` | `Passed|122`，无新增运行 |

## 7. 声明边界

- **122/122 Passed、1:50.7921181、finished `2026-10-25T07:49:34.644Z`、`scope-restored|font|0|632` 是主开发核验的宿主真实运行结果，本执行者未运行测试、未产生新的测试回执**；本报告只登记只读核对与存档恢复证据。
- 本包未改源码、未跑 test、未整理 9 文档、未做 Git 写、未访问正式工程/未授权目录、未安装组件/改权限/计费。
- 完成报告须由主开发专业核验，本文件不代替制作人批准，也不预填后续阶段结论。

## 8. 2026-10-25 附记：residue 位置修复（已处置）

- 本文件 §4 记录的 residue 原路径 `Development/StudioRuns/R3B-SaveProtection-20261024/20261025T075219Z-residue/` **仅作为历史创建位置保留**，未改写原记录。
- 该目录已由后续授权包（仅路径修复）**整目录受控重命名**（同卷原子 `Directory.Move`，不递归删除、无覆盖）至 `Development/StudioRuns/R3B-UI-20261025/full-play-residual-20261025T075219Z/`；旧路径现已不存在。
- **移动字节不变**：`original-save.bin` `13402B / 4d2562a833aa1d7a301e7da34bf943a8e66e7920734faa6117dbb5e23206f25b`，`manifest.json` `974B / 6df497730840db912490e8cb2a21b56d4ee77bb9e7e14c463b1881372fa1022d`，移动前后逐一 SHA256 相等；manifest 内容（含 `kind=residue-backup`、`restored=false`）**未改一字**。
- 移动后按 `BlockOnUnfinishedRuns()` 同等只读口径枚举保护根：`174` 目录，`restored=173`、`protective-snapshot=1`（`20261024T052704Z`）、`no-manifest=0`、`unreadable=0`。`20261025T074636Z/manifest.json` 中的 `residueBackupRunId` 属身份字段，仍保留不改。
- 证据：`Development/StudioRuns/R3B-UI-20261025/residue-location-repair.md`。本附记不改变 §1–§7 的历史事实与时点结论。
