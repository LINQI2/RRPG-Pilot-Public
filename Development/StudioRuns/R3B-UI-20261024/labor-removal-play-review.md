# R3-B 专项：劳动撤职与施工撤出 Play 计量证据

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot，本任务唯一授权目录）；分支 `ai/rrpg-r3b-population-pilot`。
- Unity 工程：`UnityProject\RRPG1.0game`（用户已打开的 Pilot Editor）。
- 阶段：**1/1 —— 补撤职与撤出劳动计量专项**。本文件不越过阶段验证门禁；通过回执必须由 Studio 阶段 check 在本包之后产生才可登记。
- 本包实际写入（仅 2 个文件）：
  - `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  - `Development/StudioRuns/R3B-UI-20261024/labor-removal-play-review.md`（本文件）
- 本包未做：未运行任何测试（未 `tests-run`、未反射启动 TestRunner、未忙等结果）、未 `rql-mutate`、未 Shell/`Process.Start`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未改场景/Prefab/资产/配置/生成配置、未读写用户存档或存档备份/历史档、未改 34 项基线/14 项保护、未 Git 写（commit/push/stage/切分支）、未启动/关闭 Editor。安全刷新只调用 `AssetDatabase.Refresh`。
- 本包未改产品 `WorkClock` / `Work` / `Clock` / API / DTO / Authority；未新增测试钩子；未削弱、跳过或删除任何既有断言；CASE-C 独立资源 Authority 与 NewGame 修复原样保留。

---

## 0. 前置核对（E1，本轮只读实核）

| 项 | 实际值 | 来源 |
|---|---|---|
| Pilot 身份 | `.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`；该 ref = `054b00d3e38fe2e2e142bfdb2502de90f7991030` | `read` 两个文件（未执行任何 git 命令） |
| Editor 空闲（改前/改后） | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | `rql-query editor`（改前、Refresh 后各一次） |
| 场景干净 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | `rql-query scenes`（改前、改后各一次） |
| 34 项 SHA 基线 | `Development/StudioRuns/R3B-20261022/workspace-baseline.json` → **entries=34 problems=0 all-match**（改前、改后各重算一次） | 只读 `script-execute` 按 JSON 内 `bytes+sha256` 逐项重算 |
| 受保护 TMP 字体 | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（改前/改后同值） | 同上（该字体是 34 项之一） |
| 未完成 save run | `Development/StudioRuns/R3B-SaveProtection-20261024/` 共 16 个 run：15 个 `status="restored"`、1 个 `20261024T052704Z` 为 `protective-snapshot`；**无 captured / restore-failed / absent / no-manifest** | 只读 `grep` 全部 `manifest.json` 的 `status`（不读、不写、不移动存档与备份） |
| Play 测试发现（改前） | `ManagementShellPlayTests` = **6 条 PlayMode**（assembly `BattleStage1.PlayModeTests`） | `rql-query tests` |
| Edit 宿主回执（既有） | `ManagementProjectionTests` = **17 Passed / 0 非 Passed / 0 skipped**（当前 Editor `testResults` 可核） | `rql-query testResults`（既有回执，不是本包运行） |
| 任务给定 Edit 回执 | 最新 **f8d40b09 Edit 真实 17/17 通过、0 skip**（主开发已核） | 任务给定；本执行者未读取 Studio 私有 runtime 记录，按任务登记，不冒充复核 |

结论：身份、空闲、场景、34 项基线、字体、存档保护 run 全部满足前置，才继续本阶段写入。

### 0.1 Authority / 规格依据（只读回读）

- `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md`
  §4（职责与场所、撤职/施工命令边界）、§7（`RemovePersonAssignment` 无 `RefreshWorkIntent`、劳动中撤职风险与 Stop Conditions）、§8（Play“劳动与资源”验证行、串行 Test 与保护要求）。
- `GameSpec/Village/Village-GameSpec-V0.1.md`
  §2.3（Primary Assignment 与 Temporary Occupancy 两层；Activity Claim 不写入 Temporary Occupancy；真实工作/通勤才构成当前门禁）、
  §4.8（真实到岗后的劳动分钟产生岗位/项目进度；项目优先于岗位；项目完成后按实际边界重新通勤原职责；午休/下班/外出不劳动）。
- 当前实现（只读源码）：`VillageRuntimeBridge.Work.cs`（`ArrivedWorkers` / `CompleteWorkInterval` / `RefreshWorkIntent` / `CompleteWorkBoundary`）、
  `VillageIntradayWork.cs`（Arrival / Evidence / 480 单位批次）、`VillageProjectSystem.OnWorkInterval`（参与者数 × 分钟 × Def 速率）、
  `VillageRuntimeBridge.cs`（`RemovePersonAssignment` 无 `RefreshWorkIntent`）、`VillageRuntimeBridge.Buildings.cs`（施工撤出调用 `Leave + RefreshWorkIntent`）。
- 本包不改上述 Authority / 产品契约；差异只作为测试断言与未测风险记录。

---

## 1. 变更 1：CASE-B 施工撤出后的**项目计量**边界

### 1.1 原缺口

原 `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` 在 796 分钟撤出 p1 后**立即重新加入**，只断言了“参与者 1→2、Primary 不变、execution≠项目劳动”；没有推进任何区间，因此：

- 没有证明撤出者**停止贡献**（项 `LaborRemainder/Progress` 不再按 2 人增长）；
- 也没有证明“仍有 p2 在场时项目继续增长”（不能错误要求整个项目零增长）；
- 重新加入后没有证明“必须重新通勤到场才恢复贡献”。

### 1.2 只读计量口径

- 施工劳动**不写入** `IntradayWork.Evidence`（该 Evidence 是 `PersonId+WorkplaceId` 的岗位劳动分钟，只有岗位目标才 `Work`）；施工计量在 `VillageProjectSystem.OnWorkInterval`：
  `LaborRemainder += 已到场参与者数 × 区间分钟 × Def.TestProgressPerParticipantPerDay`，`Progress += LaborRemainder/480`，余数保留。
- 因此本用例用**只读项目事实 + 剩余参与者数 + 真实工作区间分钟 + 真实 Def 速率**推导期望，不读私有状态、不写项目、不造劳动：

  `期望 numerator = 撤出后 LaborRemainder + 剩余参与者数 × 区间分钟 × TestProgressPerParticipantPerDay`
  `期望 Progress = 撤出后 Progress + numerator / 480`；`期望 LaborRemainder = numerator % 480`

  该推导由新增测试辅助 `AssertProjectInterval(...)` 执行，速率从真实 `Projects.GetProjectDef(project.ProjectDefId)` 读取。

### 1.3 新增断言序列（不立即重加）

1. 撤出 p1（真实按钮）后：
   - `participants == [p2]`；`p1` 的 Primary 仍是 `ws_farm_01`；`ws_farm_01` 占用仍为 1；
   - `p1` 执行标签不再是“项目劳动”；
   - **同一时刻证明 p2 仍在真实施工现场**（`PersonPlace(p2) == construction:<id>`）且为“项目劳动”，作为“仍有人在贡献”的前置。
2. 撤出后不重加，关闭管理推进到 `removalMinute + 4`：
   - 断言 `participants == [p2]`、`p2` 仍现场且“项目劳动”、`p1` 不再“项目劳动”、Primary 不变；
   - 用 `AssertProjectInterval(..., workerCount=1)` 断言实际 `Progress/LaborRemainder` 精确等于“仅 p2 × 4 分钟”的推导值。
     若撤出者仍被计量，结果会是“2 人 × 4 分钟”，断言必失败；若错误要求零增长，断言同样失败。
3. 合法重新加入（真实按钮）：
   - `participants == [p1,p2]`、Primary 不变；
   - **重新加入当刻** `Progress/LaborRemainder` 与重加前完全相同（不直接产生劳动）；
   - `WorkArrival(p1)` 必须是**新登记**：`Destination == construction:<id>`、`ReadyMinute == 重加分钟 + Schedule.CommuteMinutes`；执行标签不是“项目劳动”。
4. 推进到新登记到场分钟：
   - 断言通勤区间按 `workerCount=1` 推导（p1 尚未到场，仅 p2 贡献）；p2 仍“项目劳动”。
5. 再推进 1 个真实工作分钟：
   - 断言该 1 分钟按 `workerCount=2` 推导（p1 已真实到场才重新计入）；`p1`、`p2` 均为“项目劳动”。

### 1.4 证据日志 tag

新增 `CASE-B` 证据行（写入既有证据日志 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`）：

- `p1 removed at minute ...; participants=[p2]; ... p2-place=... p2-execution=项目劳动; progress=... labor=...`
- `withdraw-stop measured at ...: interval=4 remaining-workers=1 progress=... labor=...`
- `p1 re-added at minute ...; arrival=construction:... ready=...; execution=通勤`
- `re-add commute to ...: ... (only p2 contributed)`
- `END p1 re-arrived at ...; participants=2; ... (2-worker interval derived after real arrival)`

---

## 2. 变更 2：新增“劳动中撤职”Play（动态岗位真实到场 + 非零证据）

新用例：`WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes`

### 2.1 前置全部来自真实产品入口

1. `PlaceBuilding("building_farm", 4f)`（真实建造面板）→ `TryAddConstructionWorker`（既有公开施工入口）→ `AdvanceTo(14400)`（唯一 Clock 推进）→ 断言 `Completed`。
2. 推进到完工后的下一工作日 `14400+481`，从当前 World 快照按只读事实挑人：`Availability == InVillage && ActivityClaim == null && 无既有 Primary`；找不到即失败并输出全部候选诊断，不伪造无 claim。
3. 打开管理 → 职责与场所 → 真实岗位行 → 真实“派工”按钮 → 断言绑定/占用/“已绑定”，并读取真实 arrival：`Destination == 动态岗位`、`ReadyMinute == 派工分钟 + CommuteMinutes`、证据为 0（不凭空产生）。
4. 关闭管理，推进到场分钟：执行标签“岗位劳动”，证据仍为 0；再推进 1 个工作分钟：`WorkplaceEvidenceMinutes > 0`（真实非零证据前置）。

### 2.2 撤职当刻与下一区间的真实断言

- 撤职前快照岗位行 `BoundCount == 1`；撤职按钮可交互；用**真实按钮**撤职（不是直接调 Bridge）。
- 撤职当刻：
  - `PersonWorkplace(person) == null`、岗位占用 0、状态含“已移除”；
  - `WorkExecution == "空闲"`、`IsExecutingWork == false`（标签/判定立即停止）；
  - 岗位投影 `BoundCount=0 / LaborCount=0`；
  - `WorkplaceEvidenceMinutes` **等于撤职前值**（已结算分钟不追回、不回滚）。
- 旧 `arrival` 行按产品真实行为**原样读取并断言事实**：
  - 本实现 `RemovePersonAssignment` 没有 `RefreshWorkIntent`/`Leave`，所以撤职当刻 arrival 行仍存在，且仍指向撤职前岗位、`ReadyMinute` 保持原值；
  - 但 `IsExecutingWork == false`、执行标签“空闲”，即**残留旧意图 ≠ 当前劳动**；
  - 测试注释与断言消息明确：若产品将来改为“即时清理”，必须按新契约评审后更新该断言，本包不改核心契约。
- 下一次真实工作区间完成（同日下午 → `Schedule.Lunch` 边界）：
  - 旧岗位证据分钟数与撤职前完全相同（下一区间不再增长）；
  - `WorkArrival(person) == null`（旧意图在该区间完成时被清理；本包不把“下一步边界清理”说成“撤职当刻清理”）；
  - 执行标签“空闲”、`IsExecutingWork == false`；
  - 重开管理复核：岗位行仍可读，`BoundCount=0 / LaborCount=0`，证据不变。

### 2.3 证据日志 tag

`CASE-D` 证据行：候选诊断、派工 + arrival + evidence=0、撤职前 minute/evidence/arrival、撤职当刻 `arrival-residual=岗位@ReadyMinute current-intent=false`、`END boundary=... arrival=<cleared> evidence=...`。

---

## 3. 变更 3：静态兼容岗位的最小 UI 事实（仅少量必要断言）

在 CASE-B 中 p1 建立 `ws_farm_01` Primary 后、施工加入前，新增只读/按钮状态断言：

- 快照 `Workplaces` 中 `ws_farm_01` 行存在，`Kind == LegacyCompatibility`、`CanAssign == false`、`BoundCount == 1`；
- 真实点击该行后：`assignButton.interactable == false`（静态岗位不是正式新增目标）、`removeButton.interactable == true`（撤职 UI 可用）；
- **不点击撤职**，避免破坏本用例“施工保留 Primary Assignment”的前置；也不意图覆盖全部静态岗位缺口。
- 证据行：`legacy row visible=ws_farm_01 kind=LegacyCompatibility bound=1 assign-disabled=True remove-enabled=True (remove not clicked to keep primary prerequisite)`。

---

## 4. 编译 / 测试发现回执（E1，本轮实核）

| 项 | 实际值 |
|---|---|
| 刷新方式 | `script-execute` 内 `AssetDatabase.Refresh()`（唯一安全刷新；未用 `rql-mutate`、未全局保存） |
| 刷新副作用 | 刷新前/后场景均 `isDirty=false`；本包未改资产/Prefab/场景/配置，因此刷新不涉及目标资产保存。未逐一枚举全部资产的 dirty 状态（无该只读口径），不宣称“所有资产都未 dirty”。 |
| 编译结果 | `isCompiling=false / isUpdating=false`；新增 Play 用例被编译发现 |
| 测试发现 | `ManagementShellPlayTests` = **7 条 PlayMode**（原 6 + 新增 1），assembly `BattleStage1.PlayModeTests` |
| Edit 测试发现（对照） | `ManagementProjectionTests` 仍为 17 条（本包未改该文件） |
| Console | 本轮 Error/Exception 仅 3 条，均为**本执行者早前一次只读哈希片段的 `NullReferenceException`（JSON 解析写法问题）**，随后改用正则解析成功；**无编译错误、无项目错误** |
| 是否运行测试 | **未运行任何测试**（按要求交 Studio 阶段 check；未 `tests-run`、未忙等） |

> 因此本包**不能**声称新增任何“通过回执”。§7 的 17/17 与 Play 6/6 都是既有宿主证据，不是本包运行结果。

---

## 5. 保护回执

| 项 | 改前 | 改后 |
|---|---|---|
| 34 项基线 | 34/34 all-match | 34/34 all-match，problems=0 |
| 受保护 TMP 字体 SHA256 | `3c758e16…c404854` | 同值，unchanged |
| 场景 | `SampleScene` `isDirty=false` | 同（未改场景/资产/Prefab/配置） |
| 存档 / 备份 / 历史档 | 未读、未写、未移动、未恢复 | 同 |
| 未完成 save run | 16 run 全为 restored / protective-snapshot | 同（本包未产生新 run） |
| 产品代码 / API / DTO / Authority | — | 未改；仅测试文件与本文档 |
| Git 写 | — | 无 commit/push/stage/切分支 |

---

## 6. 未测 / 证据边界 / 风险（必须随通过回执一起读）

1. **本包未运行 7 例 Play**：新增/修改断言全部只有源码级推导；通过数必须由 Studio 阶段整类串行 check 产生后才可登记。CASE-B 的既有 6/6 属历史回执，不能覆盖本包改动。
2. **CASE-B 的项目计量推导是“公式推导 + 运行期精确相等断言”**：期望值不是常量，而是运行期读取的 `Progress/LaborRemainder/区间分钟/剩余参与者数/Def 速率`；仍属“测试未跑”状态，不能写成已观测通过。
3. **CASE-D 的 arrival 清理时序**来自源码：`RemovePersonAssignment` 不 `RefreshWorkIntent`/`Leave`；清理发生在下一次 `CompleteWorkInterval` 的 `ArrivedWorkers(prepare:true)`。本包按事实断言“撤职当刻仍有残留行、下一区间边界被清理”，**没有要求也不宣称**“撤职当刻 arrival 立即消失”。`CASE-D` 只覆盖到同日午休边界，不覆盖跨日 Day Tick、下班边界或第二天上午的补登记行为。
4. **岗位行 `LaborCount` 不是劳动证据**：它走“现场地点投影”（`GetPersonPresence` 在工作边界登记）。本夹具在 480 边界后才派工，因此 CASE-D 不用 `LaborCount==1` 当劳动前置，改用 `IsExecutingWork`/执行标签/`IntradayWork.Evidence` 分钟；这是口径说明，不是放宽业务断言。
5. **撤职删除累计工作统计**（`TotalTestContribution` 等）由 Edit `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution` 覆盖（宿主 17/17 证据）；本包 CASE-D 只证明**当分钟/下一区间**的岗位证据分钟不增长、不回滚，不重复证明 Day Tick 累计统计语义。
6. **活动 claim 前置**：CASE-D 在 481 分钟选人时断言 `ActivityClaim == null`；若当日 420 机会点选中所有人则夹具失败并输出候选诊断，不伪造 claim、不改活动 schedule。CASE-B 延续既有“无 claim 才加入”的前置。**“claim 持有者加入施工”的真实产品行为仍未被本包覆盖**（矩阵 C-03 缺口保留）。
7. **未覆盖**：多 Slot 命令路径、完工后 `AddConstructionWorker` 拒绝的 Play、真实远征/Return gating、World/Person/Management 三表面 Simulation 对照、滚动/多分辨率/物理鼠标、跨日 Continue 的撤职/撤出连续性。以上不在本阶段范围，未用旁证冒充。
8. **保护缺口仍属既有**：`SaveSlotGuard` 依赖 TearDown 恢复，进程级 abort 不构成自动恢复保证；本包未触发存档写操作。

---

## 7. 矩阵最新证据补记（不改矩阵文件正文）

> 本节只补记本包直接相关项的最新口径；`requirement-evidence-gap-matrix.md` 正文未改（非本包授权写入）。

| 项 | 补记 |
|---|---|
| Edit 总回执 | 最新宿主证据：`ManagementProjectionTests` **17/17 Passed、0 skipped**（任务给定 `f8d40b09`，主开发已核；当前 Editor `testResults` 也为 17 Passed / 0 非 Passed，可核）。该回执覆盖本包之前的 17 例，不覆盖本包 Play 改动。 |
| Play 总回执 | 既有 `ManagementShellPlayTests` **6/6 Passed** 为改动前历史回执；本包改动后整类为 **7 例**，**尚无通过回执**。 |
| B-05（撤职删除记录与累计统计） | Edit 17/17 宿主证据已含 `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution`；Play 侧“劳动中撤职 + 已结算分钟不追回”由本包 CASE-D 补测，**待宿主整类 check**。 |
| B-06 / D-01（劳动中撤职意图与贡献） | 本包 CASE-D 补齐“撤职当刻标签/绑定/证据不增长 + 旧 arrival 残留事实 + 下一区间边界清理”的 Play 断言；**未改 Work/Clock 契约**，仍待宿主回执。 |
| B-04（静态兼容岗位） | 本包 CASE-B 增加显示/派工禁用/撤职可用的最小 Play 断言；真实撤职点击未加（保护 CASE-B Primary 前置），**待宿主回执**，不关闭完整 B-04 缺口。 |
| C-02 / D-04（施工撤出后停止贡献） | 本包 CASE-B 增加“撤出后仅剩 p2 仍按 1 人计量、撤出者停止、重加必须新通勤到场后才按 2 人计量”的 Play 断言；**待宿主回执**。 |

---

## 8. 交接

请 Studio 阶段以短类名 `ManagementShellPlayTests` 整类 **PlayMode 串行** check（7 例），并用该轮实际回执替换本文件的“待宿主”口径。本文件不构成通过声明，也不代制作人批准；新增断言若在宿主运行中失败，应先按 §6 的证据边界区分“测试推导错误 / 产品行为差异”，不得通过削弱断言或改产品核心来求绿。
