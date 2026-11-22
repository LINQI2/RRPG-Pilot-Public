# R3B-UI-20261024 · 管理六例集成专项 · 只读前置核对（management six-case preflight）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，唯一操作目录）；Unity 仅连接用户已打开的 Pilot，未启动/关闭。
- 阶段：**1/2** —— 只做“完整管理 Play 专项”的只读前置核对与运行前事实登记；不重做功能、不新增/修改实现。
- 本包实际未做：未改任何源码/场景/资产；未 `AssetDatabase.Refresh`；未运行任何测试（未 `tests-run`、未反射启动模型或 TestRunner）；未 `rql-mutate`；未 Shell/`Process.Start`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未启停 Unity；未读写用户存档文件；未改 34 项基线 / 14 项保护 / 备份 / manifest / 证据日志；未 Git 写（commit/push/stage/切分支）。
- **结论：本次可读前置全部满足，未发现阻断项 → 可交宿主对 `ManagementShellPlayTests` 整类做一次串行 PlayMode check（6 例）。** 本文件不预填任何测试通过。

---

## 1. 只读判定表（均为本包工具实际回读）

| # | 检查项 | 实际值 | 判定 |
|---|---|---|---|
| 1 | Pilot 身份 | `unityVersion=2022.3.62t9`；`projectPath=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 通过 |
| 2 | 编译完成 | `isCompiling=false`、`isUpdating=false`；`console \| where logType=="Error"` = **0**；`message contains "error CS"` = **0** | 通过 |
| 3 | 场景干净 | 已加载场景仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`rootCount=5` | 通过 |
| 4 | 非播放态 | `isPlaying=false`、`isPaused=false` | 通过 |
| 5 | TestRunner 无待定（可见层） | `testResults` 只有 1 条已完成结果：`ExplicitSaveContinueAndNewGame_…` = **Passed / 2262ms**；`console` 中 `Run started` / `Test Run` = 0 条；无新 `ENTRY`/`SAVE-CAPTURE` 追加（证据日志末条仍为 `05:48:41 BASELINE-POST`） | 通过（可见层） |
| 6 | 存档保护无未完成 run | `Development/StudioRuns/R3B-SaveProtection-20261024/` 下 4 个 run：`054838Z`/`054336Z`/`053959Z` = `restored`，`052704Z` = `protective-snapshot`（`pendingRestore=false`）；**未完成 run = 0** | 通过 |
| 7 | 最新 run 单槽恢复 | `20261024T054838Z`：`status=restored`、`restored=true`、`readbackVerified=true`、`lastError=null`、`sourceBytes=backupBytes=13405`、`sourceSha256=backupSha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` | 通过 |
| 8 | 34 项基线 / 字体（最近一次真实运行） | `05:48:38 BASELINE-PRE total=34 diff=0 all-match`；`05:48:41 BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`；`FONT-PRE=FONT-POST sha256=3c758e16…854 unchanged=True` | 通过 |

> 说明：本包可读 RQL 无 TestRunner 原生 `running/pending/activeRequest` 字段；第 5 项依据“Editor 空闲 + 唯一已完成结果 + 无新测试日志/证据追加”的可见层事实，不用旧回执代替本次可见证据。原生 TestRunner 门禁继续由宿主阶段 check 前的空闲确认负责。

---

## 2. 当前 6 条唯一 FQN（只读测试发现，非运行）

`tests | where fullName starts-with "RRPG.ManagementShell.Tests.ManagementShellPlayTests."` → **count=6**，`assembly=BattleStage1.PlayModeTests`，全部 `mode=PlayMode`、`ignored=false`、`explicit=false`、`parameterized=false`（无 skip、无参数化、无重复方法）。

| # | 唯一 FQN | 源码行 | 性质 |
|---|---|---|---|
| 1 | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` | 981 | 原有 5 例 |
| 2 | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` | 1095 | 原有 5 例 |
| 3 | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput` | 1189 | 原有 5 例 |
| 4 | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite` | 1235 | 原有 5 例 |
| 5 | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` | 1339 | 原有 5 例 |
| 6 | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` | 1529 | CASE-C（已单跑 1/1 绿） |

- 测试文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`，本包只读（共 2000 行），未写入。
- 6 个方法均为 `public IEnumerator` + UnityTest；类内断言未放宽、未加 `[Ignore]`、未加分类过滤。
- 指定整类短类名 filter `ManagementShellPlayTests` 在当前发现结果中**只对应这 6 条**（无同前缀其他 fixture 命中）。

### 6 例的保护/断言骨架（本包只读核对的公共结构）

- `[OneTimeSetUp]`：`LoadBaseline()`（硬断言 `files.Length == 34` 且 `root` 等于当前仓库根）→ `BASELINE-PRE`（`diff=0` 才继续）→ `FONT-PRE`。
- `[UnitySetUp]`：`new SaveSlotGuard().Capture()` → 先 `BlockOnUnfinishedRuns()`（白名单 `restored` / `protective-snapshot`，`ManagementShellPlayTests.cs:659–682`），再持久备份 + manifest + 回读 SHA256 → 复位 → 场景重建 → 真实产品入口进 World。
- `[TearDown] try/finally`：先清 `ForceSaveFailureForTests`，再 `Restore()`（写回后回读 hash 匹配才标 `restored`，失败抛异常使测试失败，不吞异常）→ 最内层 `VillageRuntimeBridge.ResetForTests()`。
- `[OneTimeTearDown]`：同类兜底恢复（不构成进程级保证）+ `FONT-POST` + `BASELINE-POST`（硬断言 problems=0 且字体 hash 不变）。

---

## 3. 存档保护 / 单槽恢复事实（只读）

| runId | status | restored | 字节 | sha256 | 备注 |
|---|---|---|---|---|---|
| `20261024T054838Z`（最新） | `restored` | true | 13405 / 13405 | `4b2b56f6…d2d6aa` | 读回校验 true，`lastError=null` |
| `20261024T054336Z` | `restored` | true | 13405 / 13405 | 同上 | 同源单槽 |
| `20261024T053959Z` | `restored` | true | 13405 / 13405 | 同上 | 同源单槽 |
| `20261024T052704Z` | `protective-snapshot` | false | 13405 / 13405 | 同上 | 只读保护快照，`pendingRestore=false`，按白名单不阻断 |

- 按 `BlockOnUnfinishedRuns()` 白名单判定：**未完成 run = 0**，不会阻断下一次串行 check。
- 单槽恢复实际证据（证据日志 272 行末段，与本包回读的 console 日志一致）：
  - `05:48:38.8537339Z [SAVE-BACKUP] runId=20261024T054838Z … bytes=13405 sha256=4b2b56f6… readback-verified=True`
  - `05:48:38.8550631Z [SAVE-CAPTURE] runId=20261024T054838Z …`
  - `05:48:41.1039434Z [SAVE-RESTORE] runId=20261024T054838Z existed=True sha256=4b2b56f6… matches-before=True`
  - 该 run 的 manifest 同时为 `status=restored`、`restored=true`；即**本轮已真实恢复**，不是“unknown”。
- CASE-C 单例事实（`testResults` 1/1 Passed，非本包运行）：`CASE-C START authorityInitial=0/0`、`NEWGAME resources=0/0 authorityInitial=0/0 batches=0 evidence=0 completed=0 clock=0 newIngredientsWereAuthority=True newFoodWasAuthority=True oldLeak=false`；`CONTINUE-ENTRY … clock=16360 speed=Paused`；`SAVE-FAIL-EXPECTED status=未保存 memory-preserved=true file-unchanged=true`。
- 保存恢复状态未退化：本轮之后 4 个 run 的 `sourceSha256 == backupSha256 == 4b2b56f6…`，与 05:43 / 05:39 两轮一致，单槽未被写偏。
- 历史缺口（保留不掩盖）：`2026-10-23T15:53:02Z` 一轮 5 次 `SAVE-CAPTURE` 无对应 `SAVE-RESTORE`，当前 `13405/4b2b56f6…` 是那轮结束后的状态，**不是**更早 `21075B/650c45c4…`；本次不恢复、不覆盖、不宣称修复。
- 本包未读取单槽文件本身（遵守“不读写用户档”），当前单槽状态取自上表 manifest + `SAVE-RESTORE matches-before=True` 的回读事实。

---

## 4. 34 项基线 / 14 项保护 / 字体（只读事实 + 未越界声明）

- `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 存在、未被本包修改（`root` = 当前仓库根，`branch=ai/rrpg-r3b-population-pilot`，`head=054b00d3…`，`complete:true`）。
- 最近一次真实运行（CASE-C 单例）当场完成 34 项双端核对：`BASELINE-PRE total=34 diff=0 all-match` → `BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`；该项由测试硬断言（problems≠0 即失败），不是本包口头声明。
- 受保护 TMP 字体（`Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`）在 05:39 / 05:43 / 05:48 三轮均为 `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、`unchanged=True`。
- 本包唯一写入是**本文件**（`Development/StudioRuns/...` 下的文档，不在 34 项基线内），未触碰 14 项保护文件、备份、manifest、证据日志、字体、ProjectSettings、Packages 或 Growth 数据。
- 限制说明：基线 JSON 是单行 34 项的大 JSON，本包只读工具对超长行有截断，未在包内逐项重算 34 个 SHA256；权威的逐项等值判定来自上面同一台 Editor 的 `HashBaseline` 双端断言与证据日志，以及接下来整类 check 的 `BASELINE-PRE/POST`。

---

## 5. 交宿主的唯一执行方式（本包不执行）

```text
测试模式：PlayMode（Studio 阶段 check）
filter.testClass：ManagementShellPlayTests
（串行，单次运行；不并行、不 split、不追加其它 filter）
```

- 前提：先确认所有打开场景已保存（当前 `SampleScene.isDirty=false`）。
- 6 例**必须全部实际执行**：结果集条数必须为 6；任何“0 项 / 部分执行 / Skipped / Ignored / Inconclusive / 恢复失败”都不 PASS，直接停下交人工核对。
- 本包不 `tests-run`、不反射启动、不忙等、不重放旧模型。

## 6. 宿主运行后必须回读的本次证据（不满足即停）

1. 结果：`testResults` 含 **6** 条 `ManagementShellPlayTests` 结果且全部 `Passed`（含 CASE-C 与 5 个旧例）。
2. 保护：新增**成对** `SAVE-BACKUP` + `SAVE-CAPTURE runId=…` 与 `SAVE-RESTORE runId=… matches-before=True`；新 run 的 `manifest.json` 为 `status=restored`、`restored=true`、`sourceSha256=backupSha256=4b2b56f6…`、`sourceBytes=13405`。出现 `captured`/`restore-failed`/hash 不匹配 → 停止，不清记录、不改档、不重试，保留 backup/manifest 交人工。
3. 保护面：`BASELINE-PRE/POST total=34 all-match`、`FONT-PRE/POST 3c758e16… unchanged=True`、`resourceScopeEnd=scope-restored`；34 项基线、14 项、字体、`StudioTestResourceScope` 资源隔离均不得被禁用或绕过。
4. 收尾：场景仍 `isDirty=false`；证据日志按 6 次 ENTRY 追加；其余异常按原样记录，由主开发核验后再进入阶段 2（设计验证缺口矩阵）。

## 7. 本包产物

- 仅本文件 `Development/StudioRuns/R3B-UI-20261024/management-six-case-preflight.md`。
- 未新增/修改/删除任何源码、场景、资产、run 目录、manifest、备份、基线、字体或证据日志；测试结论以宿主本次整类回执为准。
