# R3-B pending 事件管理门禁专项（阶段 1/1 · 仅测试与文档）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 日期：**2026-10-24**（Asia/Shanghai；UTC+8）。
- 基线只读快照：**`2026-10-24T14:50:26.683Z`**（任务给定的固定只读快照；本文件引用该快照值，并在写入后用同一只读通道实测当前值）。
- 本阶段范围：**只补一个最小真实 UnityTest + 本文件 + 纠正上包 9 份文档/审计中的当前事实**。未改产品代码 / 公共 API / DTO / WorkClock / 规则 / 配置 Authority / Scene / Prefab / 字体 / Packages / ProjectSettings / 基线 / 存档本体；未 `rql-mutate`；未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`；未 Shell / `Process.Start`；未 Git 写；未安装组件 / 改权限 / 创建额外付费子 Agent。
- **测试未由本执行层运行**：本文件不声称新用例通过；Play 整类由宿主以短类名 `ManagementShellPlayTests` 串行 check。文档结论仍 **PARTIAL**。

---

## 1. 开工前只读核对（固定快照 + 当前实测）

| 项 | 事实 | 证据 |
|---|---|---|
| Editor 空闲 | `isPlaying=false` / `isPaused=false` / `isCompiling=false` / `isUpdating=false`；`selection=[]` | `rql-query editor`（改前） |
| 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`isLoaded=true`，`rootCount=5` | `rql-query scenes`（改前） |
| 34 项 workspace baseline | **34/34 same、0 missing、0 diff**，`complete=true`，`head=054b00d3e38fe2e2e142bfdb2502de90f7991030` | `script-execute` 只读逐项 SHA256（改前） |
| 受保护 TMP 字体 | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**（= 基线） | 同上 |
| 当前单槽存档 | **13405B / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`** | 同上（只读 `SaveSystem.GetSavePath` + 文件哈希） |
| `20261024T143611Z` | manifest `status=restored`；单槽与快照同 hash（13405B/`4b2b56f6…`） | run manifest + 只读哈希 |
| 所有保护 run | **137**：`restored=136` + `protective-snapshot=1`（仅 `20261024T052704Z`）；`otherStatus=0`、`noManifest=0`、binSha mismatch=0 → **无未完成 run** | 只读遍历 `Development/StudioRuns/R3B-SaveProtection-20261024/**/manifest.json`（判定口径与 `SaveSlotGuard.BlockOnUnfinishedRuns` 一致） |
| 固定快照登记值 | `HomeProductShell.cs` **16082B / `7da8fbb06aac044e37acbd21377ff7e32f4aa1b99659e4f18bdf66bbbd8f2f66`**；`SaveSystem.cs` **45001B / `97f48237e42f05c7d8768ee09a09b6f7a295189afb47329afc3e16268c8d8045`**；`ManagementProjectionTests.cs` **41759B / `7670c2834f9732b2ea24e007af65f8a0c9da59fe443da4c064744a4c4efeff9e`**；`ManagementShellPlayTests.cs` 改前 **261337B / `8449b313cf5ffa5b959fb28bdbe2ff692a2703bba287b70fa08ebd4c2fe53b35`** | 任务给定快照；写入后实测产品/Edit 文件与其一致（见 §4） |

> 以上为只读探测，不含任何写入。`20261024T143611Z` 的 `restored` 与 137 run 无未完成，均按 manifest 实际内容复核，不引用旧报告口径。

---

## 2. 目标与既有 Authority（为什么这样测）

- R3-P §8「远征/事件 gating」要求：busy / pending 事件保持既有优先级；关闭内部页不解锁 Return 或启动第二 Clock。§5 又明确：既有 Pending 事件 / Return 推进阻断优先保留，不能把事件 PauseOnOpen 或 busy 状态统改成不暂停。
- 统一事件唯一长期入口是 `EventRuntimeBridge.PendingVillageEvent`；阻断判定是 `VillageEventFlow.TryGetAdvanceBlockingInteraction` → `VillageTime.CheckInteraction` → `WorldAdvanceState.WaitingForInteraction`（Direct 与 Return Catch-up 共用同一个 `AdvanceSession` / `CheckInteraction` 缝）。
- 世界写锁既有规则：`VillageRuntimeBridge.IsWorldWriteLocked = Time.HasActiveAdvance || 未完成 Return`；`CanSaveWorld` 再加远征占用。管理命令与保存的既有拒绝语义是 `ManagementCommandGateway.CheckPerson` → `WorldBusy`；保存 UI 文案为「未保存：…」。
- 结论：**本专项不新增任何规则**，只证明既有 pending → waiting → worldBusy 链在管理页与 Return 上不可被 UI 绕过，并且合法解决后原请求只完成一次。

---

## 3. 新增测试（最小、真实产品链、无私有反射造 pending）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
类：`RRPG.ManagementShell.Tests.ManagementShellPlayTests`（宿主短类名 `ManagementShellPlayTests`）
新增：`PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`（第 **11** 个 `[UnityTest]`；原 10 例、NewGame 资源断言、Shell 来源复位用例、施工 schedule 局部恢复均原样保留）。

### 3.1 pending 如何建立（不反射写 pending）

- 用既有 **正式 Recognition 触发条件**：同一真实人物第 3 次加入 Active Project（`prj_test_01`，`VillageTestSocialPatterns` 阈值 3；产品 post-add 路径 `TryStartProject` → `SocialFacts.RecordTemporaryCommitment` → `EvaluateDraftBuildupResponse` → `VillageEventFlow.RaiseRecognition` → `EventRuntimeBridge.SetPendingVillageEvent`）。
- 测试只调用既有公共入口（`VillageRuntimeBridge.TryStartProject` / `Clock.AdvanceTo` / `BeginExpeditionReturnMinutes` / `MarkExpeditionReturnSettled` / `RunReturnCatchUp` / `PresentAdvanceInteraction` / 事件 UI `OnEventOptionClicked` / `OnCloseClicked` / `PumpWorldAdvance`），**不反射写 `PendingVillageEvent`、不构造 `ResolvedEventInstance`、不调私有方法**。
- 测试起始用既有公共 `EventRuntimeBridge.Restore(null)`（等价“空事件区块”）做夹具隔离：本类入口不重置长期 Event 世界，上一用例可能留下历史 / Pending。这不是门禁绕过——之后所有 pending 都由真实触发条件建立，且每例只解决自己建立的实例。

### 3.2 用例流程与断言（真实回执由宿主产生）

1. **真实基座**：Farm A 完工并派真实人物；两名真实人物各完成 2 次 `prj_test_01`（共 40 天真实推进）；Farm B 真实施工并加入真实工人；真实资源写入 400/400。活动机会窗口用既有 `ConfigureActivitySchedule` 局部隔离（TearDown 恢复）。
2. **建立 pending**：第 3 次真实临时承诺 → `PendingVillageEvent.EventId == evt_test_village_temp_draft`；Pending 未解决前事件历史 = 0。
3. **阻断 Direct Advance**：`Clock.AdvanceTo(now+60, Direct)` 返回 `WaitingForInteraction`；Session 保留、`Origin=Direct`、`BlockingEventInstanceId` = 真实 pending 实例、`OriginalTarget` 未改写；Clock 不前进；`IsWorldWriteLocked=true`、`CanSaveWorld=false`。
4. **管理页不绕过**：真实底部导航点击进入 Management（不是不可达）；真实保存 / 撤职按钮禁用；`ManagementView.Save()` / `RemoveSelectedPersonAssignment()` 失败且状态含「未保存」「世界正在推进」；ESC 回 World；手动 Pause→Resume 跨真实帧后 Clock 不动、Session 仍 Waiting、Pending 仍在、`CanSaveWorld` 仍 false。
5. **命令 / 保存零写**：派工 / 撤职 / 加入施工 / 撤出施工 / 显式保存全部 `WorldBusy`；职责、岗位占用、施工参与者/进度、Ingredients/Food、生产批次、Clock、单槽文件 hash 全部不变。
6. **合法解决只完成一次 Direct**：事件 UI 真实选项 `opt_test_village_continue`（无效果）→ Pending 清除、事件历史 +1、Session 仍 Waiting（选择不等于确认）；UI 关闭 acknowledge 为 `Yielded`；`PumpWorldAdvance` 后 Session 释放、Clock 恰好到原目标（不多一分钟）、速度保持 Pause；职责 / 占用 / 施工 / 资源不变。
7. **阻断 Return Catch-up**：第 2 个真实触发条件建立新实例；`BeginExpeditionReturnMinutes(1 天, SafeReturn)` + `MarkExpeditionReturnSettled`；`RunReturnCatchUp()` 返回 `WaitingForInteraction`，Session `Origin=Return`、`OriginalTarget=now+1 天`、Clock 不变、`CanSaveWorld=false`；管理开关 / ESC / 保存 UI / 派工仍不绕过；资源 / 批次 / 施工事实不变、单槽零写。
8. **合法解决只完成一次 Return**：同一事件 UI 合法选项 → Pending 清除、历史 +2；关闭 acknowledge 后 `RunReturnCatchUp()` 返回 `Completed`，Clock 恰好 +1 天，`Phase=CatchUpComplete`、`DaysSimulated=1`；职责 / 占用 / 施工参与者保持、Farm B 仍 UnderConstruction；再次 `RunReturnCatchUp()` 返回 `AlreadyCaughtUp`，Clock / 资源 / 批次 / 施工事实全部不再变化（不重复结算）。
9. **投影一致**：`ManagementReadAdapter.Read()` 的 Resources 等于真实 Authority；目的 Person 行 `PrimaryWorkplaceId` 等于真实 Assignment；Farm B 施工行 `WorkerPersonIds` 等于真实施工参与者。

### 3.3 明确没有做

- 没有清 pending 私有状态来“让测试通过”；没有直接调用底层 Advance 绕过 gate；没有新增产品规则 / API / DTO / WorkClock；没有削弱、跳过或删除任何既有断言。
- 没有把“管理按钮必须拒绝”当作新规则：断言的是既有 `CanSaveWorld`/`WorldBusy` 与既有按钮 `interactable` 计算结果。
- 没有改 `SaveSlotGuard`：新用例直接复用每例 `Capture/Restore` 与 34 项 / 字体 OneTime 断言。

---

## 4. 文件与 SHA（实测，不沿用旧值）

### 4.1 本阶段唯一代码改动

| 文件 | 改前（固定快照） | 改后（实测） |
|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 261337B / `8449b313cf5ffa5b959fb28bdbe2ff692a2703bba287b70fa08ebd4c2fe53b35` | **289142B / `96982624fea6e6976a0c539fc309ed62842d6d54d69be94e35e3c84f0750cd43`** |

### 4.2 产品 / 基线文件（实测与固定快照一致，未改）

| 文件 | 实测 |
|---|---|
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | 16082B / `7da8fbb06aac044e37acbd21377ff7e32f4aa1b99659e4f18bdf66bbbd8f2f66` |
| `Assets/Scripts/Save/SaveSystem.cs` | 45001B / `97f48237e42f05c7d8768ee09a09b6f7a295189afb47329afc3e16268c8d8045` |
| `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs` | 41759B / `7670c2834f9732b2ea24e007af65f8a0c9da59fe443da4c064744a4c4efeff9e` |
| 受保护 TMP 字体 | 14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |
| 34 项 baseline | 34/34 same、0 diff |

### 4.3 编译证据（只 `AssetDatabase.Refresh`）

- `AssetDatabase.Refresh()` 后 `isCompiling=false`；项目 Console 无新增 `error CS`（仅有下条披露的 snippet 自身错误）。
- `BattleStage1.PlayModeTests.dll` 实测 mtime **`2026-10-24T14:59:24.0038652Z`**；反射实核类内 `UnityTest` 属性方法 **11** 个，新方法 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` 存在。
- **执行通道披露**：本阶段用 `script-execute` 做 `AssetDatabase.Refresh`、只读哈希与程序集反射。期间有 2 次**片段自身**语法错误编译失败（Console `CS0201` / `CS1955`，`snippet.cs`），未触及任何项目源码；随后 Refresh 与程序集编译成功。未运行 `tests-run`、未启动 Test Runner、未忙等测试。

---

## 5. 验证状态（必须保持 PARTIAL）

- 新用例**未由本执行层运行**，也不声称通过；宿主需以短类名 `ManagementShellPlayTests` 整类 PlayMode 串行执行。
- 改前已核宿主回执仍是历史事实：完整 Edit **1513/1513**（`f116b56e`）、完整 Play **121/121**（`c048f135`）、管理 Edit **17/17**（`f8d40b09`）、管理 Play 独立 **10/10**（`0fd4b031`，主开发已核）。**这些数字对应新增用例之前的 10 例快照**；加入第 11 例后，Edit 仍为 1513 条（本阶段未改 Edit），Play 条数需宿主重跑后按真实结果登记（预期 122，但**不预填、不冒充**）。
- 因“新 pending 用例尚未宿主运行”，R3-B 整体仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**；Compiler 264 仍 **BLOCKED**；不写 Closed / Published / Accepted。

---

## 6. 纠正上包 9 份文档 / 审计中的当前事实

> **（2026-10-24T15:0xZ 事后更正）本节是“计划的纠正清单”，不是已执行事实。** 写入本文件的 R3-B 执行包在写完后被中止（aborted），**下列 9 份文档实际一份也没有被修改**：`final-self-audit.md` 与 `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_…Execution-Record.md` 的内容与哈希均保持中止前状态（自审索引仍写“管理 Play 10 例”“V15 事件 pending 未测”“产品级同帧竞态保留”；执行记录 §5 仍写“事件 pending 专项未测”）。因此：
> - 本节的“纠正内容”只能作为**待执行的更正意见**，不得据其宣称文档已同步；9 份文档的旧口径在其被真正改写前仍然有效。
> - 后续独立小包重做文档更正前，须重新只读核对事实，不得沿用本节的“已改”口气。
> - 证据与中止核验见 `Development/StudioRuns/R3B-UI-20261024/pending-gate-recovery-preflight.md`。

以下为原计划的 9 份上包新文档/审计（历史推测保留为历史，但加“当前更正”）：

1. `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`
2. `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md`
3. `Docs/DevelopmentSpecs/README.md`
4. `Docs/项目状态快照.md`
5. `Docs/项目日志.md`
6. `Docs/文档地图.md`
7. `Docs/开发验收交接.md`
8. `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md`（末尾附记）
9. `Docs/DevelopmentSpecs/2026-10-10_RRPG_V17-R3-A_Person-Detail_Execution-Record.md`（状态附记）

纠正内容：

- **管理 Play 独立 10/10 回执 `0fd4b031` 确实存在且主开发已核**：不能再写“此前无独立 10/10 回执 / 只由 121 全集覆盖”。
- **Shell 产品级 `SyncSourceReset` 已在 `54a6c399` 包实改**；`FirstNavigation` 在 `1088aa8e` 以及 `0fd4b031` / 完整 Play 中已通过。不能再写“未改产品 / 产品级首次点击竞态仍保留”。历史包当时“只改测试”的记录保留为历史，但当前事实按本条更正。
- **字体 hash 错字更正**：`full-play-postflight.md` 原工具证据中的 `…ad26fc80bc…` 为错字，真实值为 `…ad26fc00bc…`（`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`）；本阶段只在本审计/新文件注明，不改原始工具证据文件。
- **不能笼统说“本整包从未改 Scene”**：R3-B 历史中含 Growth 维护代码的授权改动及配套授权变更；上包文档中“整包未改 Scene / 任何 Scene 未改”的笼统表述已按“本阶段未改 Scene；历史授权改动不并入本阶段声明”收窄。本阶段（2026-10-24 收尾 + 本专项）确实未改任何 Scene / Prefab / 资产。
- **上包“事件 pending 专项未测”仍然成立**：本阶段只新增用例与文档，未运行；不得把新增用例写成已通过。

---

## 7. 保留边界与残余风险

1. 新用例依赖既有 Test Project `prj_test_01` 的 10 天完成节奏与 Recognition 阈值 3（均 Test / Not Frozen），未改其数值或规则；若未来阈值/节奏变化，测试需按新事实调整（属规则变更包）。
2. 用例用既有 `ConfigureActivitySchedule` 在**本例时间跨度**隔离活动机会；TearDown 恢复原实例并只读校验。其它 10 例仍跑真实活动机会。
3. 用例解决事件使用既有 `opt_test_village_continue`（无效果）；GrantRest 分支仍未在管理门禁包覆盖。
4. Return 只走到 `CatchUpComplete`（1 天）；**exactly-once 尚未验证**——本执行层从未运行该用例（宿主未产生新回执），该断言只是用例内写的强断言，不得写成“已验证”。
5. 保护只在 TearDown：原生 abort / 域重载不自动恢复；本轮未新增保护机制，也未改动 137 个既有 run。
6. 历史 `650` 存档（`21075B/650c45c4…`）仍无字节备份：不恢复、不伪造；当前 `13405B/4b2b56f6…` 不是它。
7. 未提交未发布：无 Commit / Push；任务外 dirty/untracked 与 index 本阶段未核验（无 git 命令），不宣称 Branch / Repository Closed。

---

## 8. 宿主执行建议（唯一剩余验证动作）

- 以短类名 `ManagementShellPlayTests` 整类串行执行 PlayMode（不要并行 Test Runner、不要按方法过滤拆散类内共享夹具）。
- 关注新用例的 `EVENT-GATE` 证据行与首败断言；若失败，先区分“真实产品门禁行为”与“夹具前置（项目 10 天完成 / 事件配置 / 活动 claim 隔离）”。
- 通过后按真实条数更新执行记录与自审索引；仍不得据此关闭整包或宣称制作人验收。
