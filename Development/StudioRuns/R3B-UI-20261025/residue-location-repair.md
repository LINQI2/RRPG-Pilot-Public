# residue 备份移出 SaveSlotGuard 保护根 · 路径修复报告（2026-10-25）

授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot）。本包只做**精确可逆路径修复 + 只读校验 + 短报告**：未改测试 helper 白名单、未改任何 manifest 状态/内容、未删除备份、未运行测试、未触发 `Capture()`、未 Save/Load、未全局保存、未 Refresh、未 `rql-mutate`、未 Git/正式工程、未安装组件/改权限/计费。

---

## 1. 守卫实际源码（先读后动）

`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：

| 行 | 实际代码/事实 |
|---|---|
| 818 | `ProtectionRootRelative = "Development/StudioRuns/R3B-SaveProtection-20261024"` |
| 819-820 | `ManifestName = "manifest.json"`、`BackupName = "original-save.bin"` |
| 837-838 | `ProtectionRoot() => Path.Combine(RepoRoot(), ProtectionRootRelative.Replace('/', Path.DirectorySeparatorChar))` |
| 1012 | `RepoRoot() => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."))` → `D:\AI_Agent_Lab\rrpg_pilot\repo` |
| 954-977 | `BlockOnUnfinishedRuns()`：遍历保护根**所有子目录**；无 manifest → `(no-manifest)`；`JsonUtility.FromJson<ManifestStatus>` 读 `runId/status`；仅 `status == "restored" || status == "protective-snapshot"` 才放行；其余（含 `status == null`）计入 `unfinished` 并 `Assert.AreEqual(0, ...)` 阻断 |
| 844-849 | `Capture()` 第一步才调用 `BlockOnUnfinishedRuns()`；本包未调用 `Capture()` |

原 residue 的 manifest **无 `status` 字段**（`kind=residue-backup`、`restored=false`），因此被 `BlockOnUnfinishedRuns()` 计为 `20261025T075219Z-residue(unreadable)`。修复选择“保留备份、移出保护根”，不改 helper、不改 manifest。

## 2. 路径与移动前强校验（真实执行结果）

用 `script-execute` 执行纯 .NET 文件 API（**不含 `AssetDatabase` / `Save` / `Refresh`**，不触发资产导入或域重载）：

```
repo=D:\AI_Agent_Lab\rrpg_pilot\repo
src=D:\AI_Agent_Lab\rrpg_pilot\repo\Development\StudioRuns\R3B-SaveProtection-20261024\20261025T075219Z-residue inRepo=True
dst=D:\AI_Agent_Lab\rrpg_pilot\repo\Development\StudioRuns\R3B-UI-20261025\full-play-residual-20261025T075219Z inRepo=True
srcExists=True dstExists=False
srcEntries(top)=manifest.json | original-save.bin
  original-save.bin exists=True bytes=13402 sha256=4d2562a833aa1d7a301e7da34bf943a8e66e7920734faa6117dbb5e23206f25b
  manifest.json exists=True bytes=974 sha256=6df497730840db912490e8cb2a21b56d4ee77bb9e7e14c463b1881372fa1022d
```

- 两个绝对路径 `Path.GetFullPath` 后均在 `D:\AI_Agent_Lab\rrpg_pilot\repo\` 之内（`inRepo=True`）。
- 源目录**仅 2 个文件**、无子目录；目标目录**不存在**（无覆盖风险）；两者独立 SHA256 与任务给定值逐位一致（`bin 13402 / 4d2562a8…e23206f25b`、`manifest 974 / 6df49773…2fa1022d`）。

## 3. 受控移动（同卷原子重命名，不 Shell、不递归删除、无覆盖）

`Directory.Move(src, dst)` 前在同一脚本内再次强校验（路径在 Pilot 内、源存在、目标不存在、目标父目录存在、源恰为 2 个普通文件、两文件字节/SHA 与给定值一致），任一不满足即抛出且不移动。实际输出：

```
PRE srcExists=True dstExists=False entries=2 bin=13402/4d2562a833aa1d7a301e7da34bf943a8e66e7920734faa6117dbb5e23206f25b manifest=974/6df497730840db912490e8cb2a21b56d4ee77bb9e7e14c463b1881372fa1022d
MOVED srcExistsAfter=False dstExistsAfter=True
dstEntries=manifest.json | original-save.bin
POST original-save.bin bytes=13402 sha256=4d2562a833aa1d7a301e7da34bf943a8e66e7920734faa6117dbb5e23206f25b byteEqual=True
POST manifest.json bytes=974 sha256=6df497730840db912490e8cb2a21b56d4ee77bb9e7e14c463b1881372fa1022d byteEqual=True
MOVE_OK=True
```

- 移动后逐字节 SHA256 复核：两文件与移动前**完全一致**（字节不变、manifest 未改一字、`restored=false` 未改）。
- 旧路径已不存在；目标目录恰有 `original-save.bin` + `manifest.json` 两文件。
- 移动前后无任何复制/删除：资源管理器外的独立文件工具复核 `Development/StudioRuns/R3B-SaveProtection-20261024/20261025T075219Z-residue/**` 返回 `No files found`。

## 4. 移动后保护根只读枚举（与 `BlockOnUnfinishedRuns()` 同等口径，未触发 Capture）

```
GUARD dirs=174 restored=173 protective=1 missing=0 unreadable=0
GUARD nonWhitelisted=<none>
  protectiveRun=20261024T052704Z(protective-snapshot)
```

- 全部真实 run 仅 `restored` / `protective-snapshot`；`no-manifest=0`、`unreadable=0`、`captured=0`。
- 移动前同一实测为 `175` 目录：`restored=173` + `protective-snapshot=1` + `20261025T075219Z-residue(unreadable)`，与 `document-correction-review.md` §3.2 的预测一致。
- 未调用 `SaveSlotGuard.Capture()`、未运行测试，因此没有产生新的 run、没有写档。

## 5. 当前单槽 / 34 项基线 / 字体（只读重算）

```
save exists=True bytes=13405 sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa mtimeUtc=2026-10-25T07:46:36.0484820Z
baselineEntries=34 match=34 problems=0
font bytes=14961033 sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854
```

- 单槽只读确认 `13405B / 4b2b56f6…d6aa`，与 `20261025T074636Z` 保护快照一致；未读写、未覆盖。
- `workspace-baseline.json` 34 项逐文件 bytes+SHA256 全部一致；受保护字体 `14961033B / 3c758e16…c404854` 与基线一致。

## 6. 身份字段与文档处置

- `Development/StudioRuns/R3B-SaveProtection-20261024/20261025T074636Z/manifest.json` 的 `residueBackupRunId = "20261025T075219Z-residue"` 属**身份字段**，仍保留不改（该 manifest 本次完全未写）。
- 已只更正两份文档的“当前位置 / 已处置附记”，保留原历史路径并声明移动字节不变：
  - `Development/StudioRuns/R3B-UI-20261025/full-play-current-postflight.md`：§4 增“2026-10-25 当前位置”、§6 表更新 residue 位置、新增 §8 附记。
  - `Development/StudioRuns/R3B-UI-20261025/document-correction-review.md`：§3.1 补当前位置与 SHA、§3.2 补已处置结论、§3.3/§4/§5 标注执行状态。
- 未写 9 索引、未改测试、未 Save/Load、未全局保存、未 `rql-mutate`、未 Git。

## 7. 可逆性

移动为同卷整目录重命名：如需回退，将 `full-play-residual-20261025T075219Z` 整目录 `Directory.Move` 回原保护根路径即可，字节仍为上述两个 SHA256；本包不自动回退。

> 本报告与两份文档更正须由主开发专业核验，不代表制作人批准；整包仍 PARTIAL / Not Committed / Not Published。
