# R3-B Pilot · 回村后管理重入 · 九例门禁运行前只读核对（nine-case preflight）

- 生成时间：2026-10-24T11:41:35Z（本机，只读核对完成时）
- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一操作目录）
- 阶段：**1/1** —— 只读复核保护面后，交宿主以短类名 `ManagementShellPlayTests` 对当前 **9** 例做一次整类串行 PlayMode check。
- 本包未做：未改任何源码/测试/场景/资产；未 `AssetDatabase.Refresh`；未启停 Unity；未 `rql-mutate`；未 Shell；未全局 `SaveAssets/SaveChanges`；未 Git 写；未创建/删除任何 Save 备份或历史 manifest；未运行模型测试或反射启动 TestRunner。
- **前置结论：下列只读项全部通过，未发现阻断项 → 可以进入本阶段唯一一次九例门禁。** 本文件不预填任何“通过”。

---

## 1. 身份 / 空闲 / 场景（本次工具实际回读）

| 检查项 | 实际值 | 判定 |
|---|---|---|
| Unity 版本 | `2022.3.62t9` | 通过 |
| 工程路径 | `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 通过 |
| 播放态 | `isPlaying=false`、`isPaused=false` | 通过 |
| 编译态 | `isCompiling=false`、`isUpdating=false` | 通过 |
| 已加载场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`isLoaded=true`、`rootCount=5` | 通过 |
| Console 错误 | `logType == Error` 或 `Exception`：**0 条** | 通过 |
| 恢复监视 | 按交接的恢复监视 `c96f533d` 只读确认：工程身份、Editor 空闲、场景干净、无待定测试，均与上表一致 | 通过 |

> 说明：TestRunner 原生 `running/pending` 字段仍不可由只读 RQL 直接读取；本包按可见层确认——Editor 空闲、`testResults` 为上一轮 9 条已完成结果、只读 Console 未见新增测试运行日志、证据日志无新增行。原生待定门禁由宿主 check 启动前再次承担。

## 2. 上一包 rejected 未运行事实（不伪报回执）

- 交接事实：上一包 `8daa5166` 被明确以 **`editor_busy` rejected**，**未发送**任何测试请求。
- 本包可读层佐证（相互独立）：
  1. `testResults` 仍只有 **9 条**上一轮（`2026-10-24T11:18Z`）结果：8 Passed / 1 Failed；失败仍是 `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement`，消息为“合法回村后真实管理页必须可见 / Expected: True But was: False”。**没有**新的结果集。
  2. 证据日志 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 共 **962** 行，最后一行仍为 `2026-10-24T11:18:58.7359328Z [BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match`；**11:18:58 之后无任何追加**。
  3. 最新 Save 保护 run 仍为 `20261024T111850Z`～`20261024T111857Z`（9 个，对应该轮 9 例），**无** 11:22 之后的新 run。
- 结论：交接所述“rejected 未发送”与可见层一致；本包不把旧失败回执当作本次运行结果，也不声称缺口已修通过。

## 3. 34 项基线 / 受保护字体（本包只读重算）

- 基线文件：`Development/StudioRuns/R3B-20261022/workspace-baseline.json`（`complete=true`，`root` 为本仓库根，`branch=ai/rrpg-r3b-population-pilot`）。
- 本包按基线逐项读取文件并重算 SHA-256（只读）：
  - **`total=34 same=34 mismatch=0 missing=0 all-match=True`**。
- 受保护 TMP 字体 `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`：
  - 实测 **`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**，与基线一致。
- 本包未写基线、字体、ProjectSettings、Packages 或 34 项内任何文件。

## 4. SaveSlotGuard：所有 run 与最近用户档恢复证据（只读）

`Development/StudioRuns/R3B-SaveProtection-20261024/*/manifest.json` 聚合：

| 指标 | 实际值 |
|---|---|
| manifest 总数 | **56** |
| `status=restored` 且 `restored=true` | **55** |
| `status=protective-snapshot` | **1** |
| 其它 / 未完成状态 | **0** |
| `sourceSha256 == backupSha256` 且 `sourceBytes == backupBytes` | **56 / 56** |
| `readbackVerified=true` | **56 / 56** |

最近一轮 9 个 run（`20261024T111850Z`、`111851Z`、`111852Z`、`111853Z`、`111854Z`、`111855Z`、`111855Z-2`、`111856Z`、`111857Z`）全部：

- `status=restored`、`restored=true`、`readbackVerified=true`、`lastError=null`；
- `sourceBytes=backupBytes=13405`；
- `sourceSha256=backupSha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`。

证据日志中同轮每条 `[SAVE-RESTORE] … matches-before=True` 与上表一致；即最近用户档已真实恢复，未出现 `captured` 悬挂或 hash 不匹配。历史 `20261024T052704Z` 为唯一 `protective-snapshot`（`pendingRestore=false` 语义，按测试白名单不阻断）。本包未读、未写、未删单槽存档文件本身。

## 5. 九例发现 / 源码与程序集新鲜度 / 上一包同步改动仍在

- `tests | where fullName contains "ManagementShellPlayTests"` → **9 例**，全部 `mode=PlayMode`、`assembly=BattleStage1.PlayModeTests`、`ignored=false`、`explicit=false`、`parameterized=false`：

| # | 唯一 FQN（短类名 `ManagementShellPlayTests`） |
|---|---|
| 1 | `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput` |
| 2 | `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement` |
| 3 | `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` |
| 4 | `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` |
| 5 | `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` |
| 6 | `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite` |
| 7 | `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` |
| 8 | `SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous` |
| 9 | `WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes` |

- 源码：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  - `mtime=2026-10-24T11:22:23.1112835Z`，size=217935，3446 行；未由本包写入。
- 程序集：`UnityProject/RRPG1.0game/Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`
  - `mtime=2026-10-24T11:22:30.8809619Z`，size=410112；**程序集晚于源码 7.77 秒**，判定为已编译最新改动。
- 上一包“Load 后先以 `RefreshNow` 同步来源切换，再真实导航”的改动仍在：
  - 测试源码第 2813 行：`Call(_host, "RefreshNow")`（Load 后事实断言前）；
  - 第 2867 行：`Call(_shell, "RefreshNow")`（真实导航前消费来源复位）；
  - 第 2844 / 2872 / 2887 行：`reentry-pre-sync`、`reentry-world-synced`、`reentry-nav-committed` 三组证据仍在；
  - 全文件 `RefreshNow` 出现 33 次；第 2884–2886 行 `Surface==Management`、`SurfaceModalActive==true`、`IsVisible==true` 三条强断言仍在，未放宽。
- 限制：本包不把“手动 `RefreshNow` 同步”当成产品首次点击竞态的修复；产品级同帧窗口仍属未修，需另行取证。

## 6. 本阶段唯一执行方式（本文件只登记，运行回执见第 7 节）

```text
测试模式：PlayMode（宿主 check）
filter.testClass：ManagementShellPlayTests（短类名，整类）
串行：单次运行；不并行、不 split、不追加其它 filter、不循环重试
前提：所有打开场景已保存（当前 SampleScene.isDirty=false）
```

- 9 例必须**全部实际执行**；结果条数必须为 9。任何 0 项 / 部分执行 / Skipped / Ignored / Inconclusive / 恢复失败 → 不 PASS，保留真实回执后停下核对，不重试。
- 本包不声称九例已通过；下一节只写入宿主实际返回的回执。

## 7. 宿主 check 实际回执

本执行层遵守“测试仅宿主 check”，**不调用 `tests-run`、不反射启动 TestRunner、不忙等**；本节由 Studio 阶段 check 在短类名 `ManagementShellPlayTests` 整类串行运行后按真实回执回填：

- 运行状态：待 Studio 阶段 check 执行（本执行层未运行）。
- 结果集：待回填（必须为 9 条且全部 Passed 才可判通过）。
- 新 Save run：待回填（须成对 `SAVE-BACKUP` / `SAVE-CAPTURE` 与 `SAVE-RESTORE … matches-before=True`，新 manifest 为 `status=restored`、`restored=true`、`sourceSha256=backupSha256=4b2b56f6…`、`sourceBytes=13405`）。
- 基线 / 字体后置：待回填（须 `BASELINE-PRE/POST total=34 all-match`、`FONT-PRE/POST 3c758e16… unchanged=True`、`resourceScopeEnd=scope-restored`）。
- 失败处置：保留真实回执后停下核对，**不循环重试**、不强清存档、不改保护面。

## 8. 保护范围声明

- 未修改 34 项基线文件、受保护 TMP 字体、Save 备份/manifest/历史证据日志、manifest 白名单；
- 未新增/修改产品代码、Prefab、Scene、Generated 资产；未改 WorkClock / API / DTO / 玩法规则；
- 未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未安装组件、未改权限、未创建额外付费子 Agent；
- 未访问未授权工程、认证目录或其他工作区；
- 本包唯一写入为本文件。
