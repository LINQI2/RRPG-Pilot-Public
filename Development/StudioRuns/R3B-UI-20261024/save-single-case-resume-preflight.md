# R3B-UI-20261024 · CASE-C 单方法续跑前只读预检（resume preflight）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一操作目录）；Unity 仅连接用户已打开的 Pilot，不启动/关闭。
- 阶段：1/1 —— 空闲恢复后，交宿主串行跑**唯一**保存连续性用例前的只读复核。本包不新增实现、不重做已有部分。
- 本包实际未做：未改源码；未 `AssetDatabase.Refresh`；未运行产品 Save/Load/NewGame；未运行任何测试（未 `tests-run`、未反射启动、未反射读取 TestRunner 私有状态）；未 `rql-mutate`；未 Shell/`Process.Start`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未启停 Unity；未动权限/包/插件/字体/正式工程；未 commit/push/stage/切分支；未读授权目录外路径。
- **结论：本次可读前置全部满足，未发现阻断项 → 可交宿主对唯一方法做一次串行 PlayMode check。** 本文件不预填测试通过。

## 1. 只读判定表（均为本次工具实际回读）

| # | 检查项 | 实际值 | 判定 |
|---|---|---|---|
| 1 | Pilot 身份 | `unityVersion=2022.3.62t9`；`projectPath=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 通过 |
| 2 | 编译/播放空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`；`console \| where logType=="Error"` = **0** | 通过 |
| 3 | 场景干净 | 已加载场景仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false` | 通过 |
| 4 | 测试发现唯一 | 类内共 6 条 PlayMode；目标 FQN 精确查询 `count=1`（`assembly=BattleStage1.PlayModeTests`，无同名重载）；源码唯一声明位于 `ManagementShellPlayTests.cs:1476`（当前文件 1908 行） | 通过 |
| 5 | 保护目录无未完成 run | `Development/StudioRuns/R3B-SaveProtection-20261024/` 下仅 `20261024T052704Z`：`status=protective-snapshot`、`restored=false`、`pendingRestore=false`、`sourceBytes=backupBytes=13405`、`sourceSha256=backupSha256=4b2b56f6…`、`readbackMatchesSource=true`；未出现 `captured`/`restore-failed`。按 `BlockOnUnfinishedRuns()` 白名单（`restored` / `protective-snapshot`，第 623 行）→ **未完成 run = 0** | 通过 |
| 6 | TestRunner 可见空闲 | `testResults` 仍为旧 5 例（Passed），无 CASE-C 结果；证据日志仍 204 行，末条 `2026-10-24T05:09:01.2930215Z [BASELINE-POST] all-match`，05:27 保护快照后无新 `SAVE-CAPTURE`/`ENTRY`/`CASE-C`；无 “Run started/Test Run” 日志 | 通过（可见层） |

补充事实：
- 本包可读 RQL 无 TestRunner 原生 `running/pending/activeRequest` 字段；通知 `0e699bd7` 记录的 2026-10-24 13:35:31 原生 `inspect+reviewIdle` 为 `running=false / pending 空 / activeRequest 空`。之后无新测试日志或结果变化，故当前可见空闲与通知一致；本包不用旧回执代替本次可见证据。
- 原投递 `98d22977` 为**明确 rejected、未启动**：不计作断言失败，也不称“未知投递”；本包不重放旧模型、不据此重跑。
- 保护快照是**当前档** `13405B / 4b2b56f6…` 的精确备份，**不是**历史 `21075B / 650c45c4…` 版本；历史缺口继续保留，本次不恢复、不覆盖、不宣称已恢复。
- 既有备份与隔离设施只读确认存在且未改动：`20261024T052704Z/original-save.bin`、`manifest.json`、`save-guard-review.md`、`Assets/Editor/StudioRecoveryBackups/2026-10-23T11-49-56-028Z/*`、`GrowthDataInitializer.cs`、`StudioTestResourceScope.cs`。`StudioFontScopeSnapshots/` 当前无遗留快照。
- 34 项基线文件 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 存在、未修改；本包无 shell/哈希执行授权，未重算 34 项，运行前后由测试自身 `BASELINE-PRE/POST` 与字体哈希核验。
- 保护链仍为：`[UnitySetUp]` 首行 `SaveSlotGuard.Capture()`（先 `BlockOnUnfinishedRuns()`，再 `CreateNew` 持久备份 + manifest + 回读 SHA256 校验）→ `[TearDown] try/finally` 复位失败钩子后 `Restore()`（只读本 runId 备份，写回后回读 hash 匹配才标 `restored`）→ `[OneTimeTearDown]` 同类兜底。断言未放宽、未 skip、未绕过。

## 2. 交宿主的唯一 filter

```text
测试模式：PlayMode（Studio 阶段 check）
filter.testMethod：RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak
```

只运行这 1 个方法，不整类、不跑全量；由宿主串行执行，本包不 `tests-run`、不脚本反射启动、不忙等。

## 3. 宿主运行后必须回读的本次证据（不满足即停）

1. 证据日志新增**成对**的 `SAVE-BACKUP` + `SAVE-CAPTURE runId=…` 与 `SAVE-RESTORE runId=… matches-before=True`；新 run 的 `manifest.json` 为 `status=restored`、`restored=true`、`sourceSha256=backupSha256=4b2b56f6…`、`sourceBytes=13405`。
2. 若出现 `captured`、`restore-failed` 或 hash/字节不匹配：**停止，不清记录、不改档、不重放旧模型、不盲目重试**，保留 backup/manifest 交人工核对。
3. 同次必须核：`BASELINE-PRE/POST total=34 all-match`、`FONT-PRE/POST 3c758e16… unchanged=True`、`resourceScopeEnd=scope-restored|font|…`；34 项基线、指定 14 项、字体与 `StudioTestResourceScope` 资源隔离均不得被禁用或绕过。
4. 测试结束场景仍应 `isDirty=false`；任何未完成/异常结果只按原样记录，由主开发核验后继续剩余 gating/回归文档。

## 4. 本包产物

- 仅本文件 `Development/StudioRuns/R3B-UI-20261024/save-single-case-resume-preflight.md`。
- 未新增/修改/删除任何源码、场景、资产、run 目录、manifest、备份、基线或证据日志；测试结果与后续结论以宿主本次回执为准。
