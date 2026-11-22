# R3B-UI-20261024 · 单方法保存用例 · 运行前只读预检

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一操作目录）
- 阶段：1/1 —— 单跑新增 CASE-C（Continue/NewGame 端到端）前的只读预检
- 本包边界（已遵守）：未改源码、未 Refresh/AssetDatabase 刷新、未运行产品 Save/Load/NewGame、未运行任何测试（未 `tests-run`、未 `script-execute`、未反射启动）、未 `rql-mutate`、未 Shell/`Process.Start`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未启停 Unity、未动权限/Packages/插件/字体/正式工程、未 commit/push/stage/切分支、未读授权目录以外的路径。

> **结论：5 项通过，1 项不满足 → 预检未通过，单跑不应在本次编辑器中直接启动。** 未满足项：Editor 处于 Play 会话（`isPlaying=true`），工程非空闲。本包只报告事实，不退出 Play、不启动/关闭 Unity。

## 1. 判定表（全部为本次只读工具实际回读）

| # | 检查项 | 实际值 | 判定 |
|---|---|---|---|
| 1 | Pilot 身份 | `editor.unityVersion=2022.3.62t9`；`projectPath=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 通过 |
| 2 | 编译空闲 | `isCompiling=false`、`isUpdating=false`；`console \| where logType=="Error"` = **0 条** | 通过 |
| 3 | 场景干净 | `scenes \| select name path isDirty` → `SampleScene / Assets/Scenes/SampleScene.scene / isDirty=false`（仅 1 个已加载场景） | 通过 |
| 4 | 测试发现 | `tests \| where fullName contains "ManagementShellPlayTests"` = **6 条**（原 5 + CASE-C，无重复）；目标方法精确查询 `count=1`，`mode=PlayMode`，`assembly=BattleStage1.PlayModeTests` | 通过 |
| 5 | 保护目录未完成 run | `Development/StudioRuns/R3B-SaveProtection-20261024/` 下仅 `20261024T052704Z/manifest.json`：`status=protective-snapshot`、`restored=false`、`pendingRestore=false`、`sourceSha256=backupSha256=4b2b56f6…`、`sourceBytes=backupBytes=13405`。按 `SaveSlotGuard.BlockOnUnfinishedRuns()` 第 623 行白名单（`restored` / `protective-snapshot`）→ **未完成 run 数 = 0** | 通过 |
| 6 | 工程空闲（Play 状态） | `editor.isPlaying=true`、`isPaused=false`、`selection=[]` | **不满足** |

补充事实（只读）：
- 目标方法定义在 `ManagementShellPlayTests.cs:1476`，CASE-C 证据点位于 1488–1901（START/PREPARE/SAVED-FACTS/SAVE-SUCCESS/SAVE-FAIL-EXPECTED/SAVE-RECOVERED/CONTINUE/NEWGAME），唯一声明，无同名重载。
- 证据日志 `R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 最后记录仍为 `2026-10-24T05:09:01.2930215Z [BASELINE-POST] all-match`（204 行）；05:27 保护快照之后**没有任何新的 `SAVE-CAPTURE`/`SAVE-RESTORE`**，即上一包之后未发生测试写档。
- `testResults` 中仅 5 条旧用例的历史结果（Passed），无 CASE-C 结果 → 本用例运行态仍未验证。
- 源码 113414B / `b1d2174a…` 为主开发/上一包登记值；本包无 Shell/哈希工具且不得 `script-execute`，**未独立重算**，仅以“编译产物中被发现的唯一方法 + Console 0 Error”作为已编译佐证。
- 保护根目录仅能按文件 glob 证明“只有 1 个 manifest”（空目录无法用现有只读工具枚举）；若存在无 manifest 的空 run 目录，`BlockOnUnfinishedRuns()` 会在测试 Capture 时阻断，不会静默覆盖。
- 历史 `2026-10-23T15:53` 轮 Capture 无 Restore 仍未核清（候选 A/B/C 未定，见 `R3B-SaveProtection-20261024/save-guard-review.md` §1.3）；当前 13405B/`4b2b56f6…` 是保护快照，**不是** 15:53 轮前用户档，本包不处理历史档恢复。

## 2. 交给宿主 check 的前置条件与单方法 filter

阻塞项处理建议（由宿主/人工决定，不在本包执行）：先结束当前 Editor Play 会话并确认 `isPlaying=false`、场景仍 `isDirty=false`，再执行单方法 check。

```text
测试模式：PlayMode（Studio 阶段 check）
filter.testMethod：RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak
```

单跑后期望（不作预填通过）：新增一条 `SAVE-BACKUP` + `SAVE-CAPTURE`（含 `runId=`）与一条 `SAVE-RESTORE … matches-before=True`；新 run 目录 manifest 由 `captured` 转 `restored`；若 abort 则保留 `captured` backup/manifest 并阻断下一轮，需人工核对，不得清记录或恢复旧档求绿。

## 3. 本包产物

- 仅本文件 `Development/StudioRuns/R3B-UI-20261024/save-single-case-preflight.md`。
- 未新增/修改/删除任何 run 目录、manifest、备份、证据日志、源码、场景、基线。
