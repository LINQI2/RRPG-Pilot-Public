# RRPG V17-R3-B｜Person Management 集中管理 Execution Record

日期：**2026-10-24**（Asia/Shanghai；本机时钟核实 `2026-10-24T22:43:30+08:00`）。
Target Branch：`ai/rrpg-r3b-population-pilot`；生成时 HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`（读 `.git/HEAD` 与该 ref 文件，**未执行 git 命令**）。
状态：**R3-B Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**。
边界：本记录是开发期执行证据，不是 GameSpec Authority，不改玩家规则；**未提交、未发布、未关闭整包**；不写 “Closed / Published / Accepted”。

> **最新（2026-10-27 选人 / 独立详情 / 四命令返修后 17 例；优先于下文全部旧数字）**：管理 Play 整类 **17/17 Passed、Failed 0、Skipped 0**（宿主记录 `40f35ae0-b7d6-4e37-ad86-235aeb64ae45`，run 级 `00:00:17.2275503`、`resourceScopeEnd=scope-restored|font|0|632`，由**主开发读原记录确认**；本层只读 `testResults` 17 条全 `Passed`、逐条 `message=null`、method 合计 **16 199 ms**，第 1 例 654 ms / 第 17 例 750 ms）。第 17 例 `Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry` 已真实覆盖：人口行单击**留页选中**、选中高亮 `(0.28,0.43,0.36,1)`、**独立详情入口**（无选人禁用、点击进既有 Person 且关闭回 World 保留选择）、**跨页 / 选目标保留人物**、**四命令 `interactable` + 程序化 top-hit + 领域结果**（`occupants=1` → 施工参与者写入 → 撤出且 Primary 保持 → `occupants=0`，全程 `Surface=Management`、`simulationActive=True`）。保护面：字体 `14961033B / 3c758e16…c404854`（9 子资产 0 dirty）、34 项 `34/34`、**当前用户档 `13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`（不是 `13405`）**、guard **310 = 309 restored + 1 protective-snapshot、unfinished=0**、`SampleScene dirty=False`、无 PrefabStage。**仍未覆盖**：物理鼠标 / 滚轮 / 键盘 / 物理 ESC 手感、Player 面行为。详见 §14 与 `Development/StudioRuns/R3B-UI-20261028/selection-command-verified-handoff.md`。

> **最新（2026-10-26 详情修复后，优先于下文全部旧数字）**：§4.2 / §5 中“管理 Play 11 例 / 12 例、管理 Edit 17 例、Detail 单行、claim 施工未测、真实 overflow 滚动未覆盖”等表述已被 **§9（2026-10-26 12 例口径）与 §10（2026-10-26 详情修复后 13 例口径，最新）** 取代；当前为 **管理 Play 整类 13/13 Passed、管理 Edit 18/18、claim 施工已验证、岗位行详情关键数值可读、真实 overflow 滚轮 + 多轮拖拽已覆盖、1280 取消**；264/1513/122 仅为前阶段门禁，不是最新全量。

> 上游规划：[R3-P Person Detail / Management Development Spec V0.1](2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md)（§4 集中管理、§5 页面接缝、§7 风险与 Stop、§8 测试矩阵）。本包实施的是其中 **R3-B（集中管理）**，不实施 R3-A 之外的其它范围。

> **2026-10-25 当前事实更正（覆盖下文旧状态；历史失败与中止保留）**
> - **Compiler 不再 BLOCKED**：宿主真实运行 **264/264 Passed、0 skipped**；TRX Counters `total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0`；`tests`/`validate` 退出 0；validate 8 条导航 sheet warning、Mirror 未写入；494 项保护文件 SHA 未变（`changedProtected=[]`）。证据：`Development/StudioRuns/R3B-Compiler-20261025/`。本文件不重跑、不重算 TRX SHA。
> - **完整 Play 更新为 122/122**：主开发已核宿主回执 **122/122 Passed、0 skipped、`1:50.7921181`、`scope-restored`、记录 `5eb0ad4c`**；旧 121/121（`c048f135`）保留为历史。
> - **管理 Play 第 11 例已真实通过**：单例 `ba019b1c` **1/1 Passed**；最新独立整类 **11/11 Passed、0 skipped、记录 `7f600ef9`**；此前独立 **10/10 `0fd4b031`** 也真实存在。第 11 例 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` 与 TearDown `SCHEDULE-RESTORE sameInstance=True` 已实际通过。
> - **HomeProductShell 首次导航已产品修复**：`SyncSourceReset` 在 `54a6c399` 实改，`FirstNavigation` 在 `1088aa8e`、`0fd4b031`、11/11、122/122 中通过；不再说“产品未修”。
> - **撤职**：执行/贡献即停，但旧 `arrival` 仍在下一次工作边界才清；**第二 slot**投影已测、真实 Gateway 未测；**Activity claim** 不增加 busy 规则、持有者加入施工未测。
> - **当前存档**：已由 `20261025T074636Z` 精确写回 **13405B / `4b2b56f6…d6aa`** 并回读；残留 `075219Z-residue`（13402B / `4d2562a8…`）已备份；历史 650 未恢复。
> - **整包仍 PARTIAL / Not Committed / Not Published**：视觉 1920/1280、滚动/拖拽、物理输入、第二 slot 真实 Gateway、claim 施工等未测，不宣称 Accepted。
> - **历史保留**：`48d8820f` 轮 aborted、原计划纠正 9 文档未执行；本包只纠正 3 文档。

---

## 1. 原方案（R3-P 已采用，本包按原样执行）

| 项 | 原方案 | 本包是否沿用 |
|---|---|---|
| 范围 | 人口 / 职责与场所 / 资源三子页；跨人物与具体岗位/建筑比较现状；在合法命令上派工、撤职、施工加入/撤出 | 沿用 |
| Authority | 复用 `PlayerCollection`、`VillagePersonAvailability`、`ProductHomeReadAdapter`/Snapshot、唯一 Presence 与 `IntradayWork`、Assignment/Workplace、Building/Project、Resource、Save | 沿用，未新增第二套 Authority |
| 命令 | 已 Completed 动态岗位 `TryAssignPerson` / `RemovePersonAssignment`；施工只 `TryAddConstructionWorker` / `TryRemoveConstructionWorker`；保存走 `SaveSystem.Save` bool | 沿用（封装为 `ManagementCommandGateway`，语义不变） |
| UI | 原 Person/Management Surface 替换白模正文；保留 TopHUD/底部导航/World 背景；固定结构用 Editor 可编辑 Prefab；Runtime 只生成重复行 | 沿用（`Assets/Resources/UI/ManagementView.prefab`） |
| 模态 | 全屏 Raycast Blocker + World 输入阻断；打开内部页不 Pause、不 `SetSimulationActive(false)`；手动 Pause 保持；ESC 与关闭同一回调 | 沿用（`HomeProductShell.SurfaceModalActive`） |
| 保存 | 显式保存显示 bool；失败不称已保存、不回滚、可重试；busy 时禁用 | 沿用 |
| 拒绝实现 | Test Project 目录、Test Contribution 排行榜、人数估算完成日、自动最优派工、岗位→职业转换、批量事务、经济重平衡、材料仓储物流、建筑升级拆除、新 Activity 规则 | 沿用，未做 |
| 保护 | 完整 Edit/Play 会破坏单槽；先备份当前档字节，结束精确恢复；不得用历史 R2 备份覆盖用户新进展 | 沿用并加强（见 §4.3） |

---

## 2. 本次变化（实现事实）

### 2.1 新增产品代码 / 可编辑 UI
- `Assets/Scripts/Village/ProductHome/ManagementView.cs`（控制器：三页签、四类行模板、检索/筛选、选中与命令按钮、保存与状态文案）
- `Assets/Scripts/Village/ProductHome/ManagementRowView.cs`（人口 / 岗位 / 施工 / 资源行视图）
- `Assets/Scripts/Village/ProductHome/ManagementReadAdapter.cs`（只读投影：人口、实例岗位（Completed 才枚举 `FunctionBindings`）、施工、资源与饥饿）
- `Assets/Scripts/Village/Runtime/ManagementSnapshot.cs`（不可变快照模型）
- `Assets/Scripts/Village/ProductHome/ManagementCommandGateway.cs`（合法命令 + 拒绝语义：`PersonMissing` / `PersonNotInVillage` / `TargetNotCompleted` / `TargetAlreadyCompleted` / `TargetNotRegistered` / `StaticTargetNotAssignable` / `CapacityFull` / `AlreadyAssigned` / `WorldBusy` / `SaveReturnedFalse`）
- `Assets/Resources/UI/ManagementView.prefab`（+ `.meta`；39 个序列化引用 0 null；Blocker 顶部留 88px 给 TopHUD）

### 2.2 既有产品的窄改（均未改 Authority / DTO / SaveVersion）
1. `HomeProductShell.cs`：管理 Surface 接入既有 Router/Shell；新增 `SurfaceModalActive` 作为**唯一**表面模态判定（`IsPersonModalOpen` 保留同义别名，`ProductHomeWorldHost` 无需改动即覆盖 Management）；导航/关闭/ESC 走同一保护入口；`SetTownInputBlocked` 复用既有 Guard。
2. `SaveSystem.NewGame()`（1 行）：在 `ResetTimeForNewGame()` 之后复用既有 `VillageRuntimeBridge.Resources.Restore(null)`，使 NewGame 回到资源初始 Authority（当前实现 0/0），再把新世界 DTO 写入单槽；**未新增 API/DTO/SaveVersion**，未改经济初值，未改 `Load`/`Save`/`ResetForTests`。
3. `ManagementReadAdapter.Labor(bound, workplaceId)`：历史缺陷——旧实现用 `IsExecutingWork` 单条件，会把“保留原岗位 Primary、实际在另一施工项目劳动”的人同时计入原岗位 `LaborCount`；改为同时要求 `GetPersonPresence` 的真实目标 == 该 WorkplaceId（施工时为 `ProjectSite`，不会回流）。
4. `Assets/Editor/GrowthDataInitializer.cs`（维护包）：移除全局 `AssetDatabase.SaveAssets()` 与对已存在资产的无意义 `SetDirty`，只保存自建资产（切断“重载后全局落盘 dirty 资产”的风险路径，见 §4.4）。

### 2.3 测试（长期回归）
- `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`：13 → **17** 例。新增：死亡不可用四命令零写入、真实远征入口造成的命令级 `WorldBusy` 零写入、完工后 `AddConstructionWorker` 拒绝零写入、撤职删除记录/累计统计但村庄级已结算贡献不回滚；另含劳动人数口径（施工不回流原岗位、通勤/活动 claim 不计劳动）等。
- `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：5 → **11** 例。新增：CASE-C（显式保存 / 强制失败 / Continue / NewGame 无旧世界泄漏）、CASE-D（劳动中撤职：执行/贡献即停、旧 arrival 残留、下一区间边界清理、已结算分钟不追回）、CASE-B 扩展（施工撤出后计量、重新加入须新通勤到场）、Return gating（出征/Return pending 零写入 + 合法结算 + Load 连续）、首次导航（来源切换后同帧提交不被复位吞掉）、三表面 Simulation 等价（同 W0 同分钟逐项等值 + 自然帧持续）、统一事件 pending 门禁（第 11 例，已实际通过）；TearDown 先存档恢复、再 `ResetForTests` 释放写锁、最后恢复活动日程。
- `Assets/Tests/EditMode/.../GrowthDataSeedTests.cs`（维护包）：真实 dirty 无关材质不被全局保存（宿主 2/2）。
- `SaveSlotGuard`（既有测试 helper）：持久备份 `<runId>/original-save.bin` + manifest + `Readback` 校验；`BlockOnUnfinishedRuns()` 白名单 `restored` / `protective-snapshot`；`TearDown`/`OneTimeTearDown` 双兜底；`Restore()` 写回后回读 hash 才算 `restored`。

---

## 3. 否决 / 未采用（含理由）

| 否决项 | 理由 |
|---|---|
| 在 Gateway 增加“Activity claim / 临时占用 → busy 拒绝” | Village §2.3 明确 Activity claim **不写入** Temporary Occupancy；Primary Assignment 是长期安排。加拒绝会改变现有规则语义（交互规则/公共契约），需制作人裁决。本包只记录现状与选项（N-0…N-4），未实施。 |
| 为第二 slot 改 `BuildingConfigDatabase.asset` 或新增 `StaticConfigRegistry` 公共注册 API | 属配置 Authority 与公共契约变更；当前生成配置只登记 `FunctionBindings: main`，无合法注入点 → 该路径**精确标未测**，不伪造。 |
| 给 `RemovePersonAssignment` 补 `RefreshWorkIntent`/`Leave`（让撤职当刻清意图） | 属 Work/Clock 核心语义变更；当前契约是“标签即停 + 下一区间不计量 + 下一边界清 arrival”，Play 按事实断言，不改核心。 |
| 在 R3-B 当轮直接修产品级“来源切换后首次导航同帧被复位吞掉” | 当轮只改测试时序（用公开 `RefreshNow` 让产品先消费来源切换）。**2026-10-25 当前事实**：后续 `54a6c399` 包已实改 `SyncSourceReset`，`FirstNavigation` 已通过；本行保留为当轮边界，不抹除历史。 |
| 自动保存 / 保存回滚 / 新 Save Trigger Ownership | 保持既有 `SaveSystem.Save` bool 语义与 Ownership；失败保留内存态并提示重试，不自动 rollback。 |
| 用历史备份 / HEAD / 历史 650 顶替当前档 | 保护只允许“本轮/该 runId 已验字节”；历史 650 无备份 → 缺口保留，不伪造。 |
| 新建万能 Management Framework / 第二 Home 入口 / 导航总线 | 原方案拒绝项；本包只做两个已确认 Surface。 |
| 放开/跳过 34 项与 14 项保护断言，或改基线求绿 | 保护断言是冻结契约；字体与基线异常走备份恢复（见 §4.4），不改断言。 |

---

## 4. 确认方案与验证

### 4.1 确认方案
- 复用唯一 Home 宿主 / `HomeSelectionContext` / Router，管理页是既有 Person Surface 的兄弟 Surface，不建第二世界或第二入口。
- 只读投影走既有 Bridge/Authority Query；命令走既有 Bridge 公共入口并统一在 `ManagementCommandGateway` 做前置校验（存在 → 可用性 → 世界锁），失败**零写入**。
- 实例岗位只从 Completed 建筑的 `FunctionBindings` 枚举，与 `Assignments.GetWorkplaceDef` 相互核对；静态兼容岗位只显示与撤职，不作新派工目标。
- 固定 UI 为 Editor 可编辑 Prefab；Runtime 只克隆行。
- 单槽保护：每例 `Capture()` 持久备份 + `Restore()` 回读；全集前另建 `protective-snapshot`，全集后精确写回。

### 4.2 验证回执（引用真实结果，不预填）

| 项 | 结果 | 来源 / 等级 |
|---|---|---|
| 完整 EditMode（无 filter） | **1513/1513 Passed、0 failed、0 skipped**、`2:53.578`、`scope-restored`、记录 `f116b56e` | 宿主回执，主开发已核（E2） |
| 完整 PlayMode（无 filter） | **122/122 Passed、0 failed、0 skipped**、`1:50.7921181`、`scope-restored`、记录 `5eb0ad4c`（旧 121/121 `c048f135` 为历史） | 宿主回执，主开发已核（E2） |
| 管理 Edit 专项 | **17/17 Passed、0 skipped**、`f8d40b09` | 宿主回执，主开发已核（E2） |
| 管理 Play 专项 | 类内 11 例；最新独立 **11/11 Passed、0 skipped**、`7f600ef9`；此前独立 **10/10**、`0fd4b031` | 宿主回执，主开发已核（E2） |
| 编译 | 各包 `AssetDatabase.Refresh` 后 `isCompiling=false`、Console 无项目 `error CS`；编译新鲜度多次以“程序集晚于源码”核对 | E1 |
| Compiler 264 | **264/264 Passed、0 skipped**；TRX `total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0`；`tests`/`validate` 退出 0；validate 8 条 warning、Mirror 未写入；494 项保护 SHA 未变 | 宿主回执，主开发已核（E2） |
| 存档 | 2026-10-25 当前档 `13405B / 4b2b56f6…` 由 `20261025T074636Z` 写回并回读；残留 `075219Z-residue`（13402B / `4d2562a8…`）已 `CreateNew` 留证 | E1 |
| 保护 run | 175 目录：174 份带 status（173 restored + 1 protective `052704Z`）+ 1 份 residue 无 status；无 captured/restore-failed | E1（residue 会被 `BlockOnUnfinishedRuns()` 误判，见 2026-10-25 报告） |
| 34 项 / 字体 | 恢复后 34/34；字体 `14961033B / 3c758e16…c404854` | E1（阶段 1 postflight 实测） |

### 4.3 单槽存档保护（本轮新增机制的使用与结果）
- 保护快照：`20261024T052704Z`（历史白名单）、`20261024T142654Z`（Edit 前）、`20261024T143611Z`（Play 前）；三者均 `13405B / 4b2b56f6…`。
- Edit 后：单槽被夹具删除（`exists=false`，目录 0 文件）→ 用 `20261024T142654Z` 写回并回读 → `restored`。
- Play 后：单槽为测试收尾写入 `13402B / a3f6d15f…`（mtime 与最后一笔测试日志同秒，内部 `SavedAtUnixTime` 与该时刻一致）→ 先 `CreateNew` 备份留证（`R3B-UI-20261024/full-play-residual-20261024T144136Z/`），再用 `20261024T143611Z` 写回并回读 → `restored`。
- 未 Load/Save 产品流程，未用 HEAD / 其它 run / 历史 650 顶替；恢复失败即停、不标 `restored` 的规则未被触发。
- **2026-10-25 更新**：完整 Play 后当前档由 `20261025T074636Z/original-save.bin` 精确写回为 **13405B / `4b2b56f6…d6aa`** 并回读；`075219Z-residue`（13402B / `4d2562a8…`）已 `CreateNew` 独立备份，未覆盖任何历史 run；该 residue 位于保护根内且无 `status`，会被 `BlockOnUnfinishedRuns()` 计为未完成（本文件只记录，未移动/未修改）。

### 4.4 字体事故（保留历史与根因，处置已核）
- 事实：`2026-10-23T12:30:59Z` 受保护 TMP 字体被写为 `e40b6d77…/14968490B`；直接路径为原执行器 `rql-mutate` 普通 call 后 MCP 1.3.0 的全局 `SaveChanges()`（`AssetDatabase.SaveAssets()` + 保存 dirty 场景）。
- 处置：变化版本完整备份留证；用原字体完整字节备份（SHA = 基线）经 `FileUtil.ReplaceFile` + `ImportAsset` **仅恢复该字体**（meta 未变），恢复后 SHA 回到 `3c758e16…/14961033`。
- 防范：受保护工程禁用 `rql-mutate`；刷新只 `AssetDatabase.Refresh`；`GrowthDataInitializer` 移除全局保存；新增“无关 dirty 资产不被保存”回归。守卫不是任意 C# 沙箱，仍有残余面（见 §5）。
- 边界：**字体恢复 ≠ 历史 650 存档恢复**。

---

## 5. 未解 / 风险（保留，不掩盖）

1. **事件 pending 专项已通过**：第 11 例已在 11/11 与 122/122 中真实运行；`EVENT-GATE` 零写入、合法选择后解锁、`SCHEDULE-RESTORE sameInstance=True` 均通过（历史“未测”仅指 2026-10-24 当时）。
2. **多分辨率 / 滚动 / 物理输入未测**：1920×1080 与 1280×720 实画面、滚动拖拽、物理鼠标/滚轮、镜头 pan/zoom、14 个未预烘字符的运行期补字，均只有静态结构或程序化点击证据。
3. **第二 slot 命令路径未测**：见 §3；投影已测，真实 Gateway 无法在不改配置 Authority 的前提下进入。
4. **Activity claim 持有者加入施工未测**：现绿只证明“无 claim 者”路径；活动结束后补登记缺口与产品期望存在差距（N-0…N-4 未实施）；本包未增加 busy 拒绝规则。
5. **撤职“即时”的边界**：执行/贡献即停，但旧 `arrival` 只在下一边界清理；无 Play 实时同分钟断言，无“已结算分钟不追回”以外的追回语义。
6. **产品级同帧导航窗口已修**：`SyncSourceReset` 已在 `54a6c399` 实改，`FirstNavigation` 已通过；本行旧“产品未改”仅作历史。
7. **保护只在 TearDown**：原生 abort / 域重载 / Unity 释放不自动恢复；`2026-10-23T15:53Z` 一轮 5 次 Capture 无 Restore 已发生。
8. **历史 650 存档缺口**：`21075B / 650c45c4…` 无字节备份，授权内不恢复、不伪造。
9. **NewGame 隔离仅 Ingredients/Food**：Activity / Social / Return / Event 区块未在本包验证；`World Host` 世界级事务债仍在。
10. **Compiler 264 已通过**：宿主 2026-10-25 真实运行 264/264、0 skip；TRX/validate/tests 证据见 `R3B-Compiler-20261025/`；本文件未重跑、未重算 TRX SHA。
11. **residue 无 status 会被守卫误判**：`20261025T075219Z-residue` 在保护根内且 manifest 无 `status`，`BlockOnUnfinishedRuns()` 会将其计为未完成（`(unreadable)`）；本包只读核对，未移动/修改备份。
12. **Issues 84 / 85 / 86 / 88 保持开放**；未关闭任何既有缺口。
13. **未提交未发布**：无 Commit / Push；任务外 dirty/untracked 与 index 本轮未核验（无 git 命令），不宣称 Branch / Repository Closed。

---

## 6. 下一步（最小动作，技术问题自办）

> 已完成（2026-10-25 宿主回执）：Compiler 264/264、0 skip；事件 pending 第 11 例 11/11、122/122 通过。下表只列仍未完成项。

| # | 动作 | 前置 |
|---|---|---|
| S1 | 视觉/输入验收包（1920/1280 实画面、滚动拖拽、物理鼠标、镜头、缺字补烘） | 无代码前置 |
| S2 | claim 持有者加入施工的 N-0 记录型用例 | 无 |
| S3 | 撤职 Play 实时同分钟断言 + 跨日/下班边界 | 无 |
| S4 | 第二 slot 真实 Gateway：需配置 Authority / 公共 API 授权后另立包 | 需授权 |
| S5 | 处置 `20261025T075219Z-residue` 无 status 的守卫误判（保留备份、不改历史 run） | 单独授权 |
| S6 | `SaveSlotGuard` 进程级缺口评估（runner 前持久快照 + 后精确写回脚本化） | 单独授权 |
| S7 | Git 只读定稿（index / dirty）、精确 Stage / Commit / Push 与三方核验 | 需制作人/主开发 Git 授权 |

---

## 7. 精确文件范围（本包代码 / UI / 测试）

- 新增：`ManagementView.cs`(+meta)、`ManagementRowView.cs`、`ManagementReadAdapter.cs`(+meta)、`ManagementSnapshot.cs`、`ManagementCommandGateway.cs`、`Assets/Resources/UI/ManagementView.prefab`(+meta)、`Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`(+meta)、`Assets/Tests/PlayMode/ManagementShellPlayTests.cs`(+meta)
- 修改：`HomeProductShell.cs`、`SaveSystem.cs`（NewGame 1 行）、`ManagementReadAdapter.Labor` 口径、`Assets/Editor/GrowthDataInitializer.cs`（维护包）、`GrowthDataSeedTests.cs`（维护包新增）
- 未改：`GameSpec/**`、Architecture、`决策记录`、Save DTO / SaveVersion / `PlayerCollection`、Work / Clock / Activity / Social 规则、`VillageRuntimeBridge` 撤职语义、`StaticConfigRegistry` / `BuildingConfigDatabase`、Scene / YAML / 字体 / Packages / ProjectSettings / 14 项 / 34 项基线。
- SHA 口径：各文件最近一次证据登记值见 `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md` §3；**本包禁止触发 Unity，未重算当前工作树 SHA**，不冒充。

---

## 8. 边界声明

- 本记录为开发期执行证据，不是玩家规则 Authority，不改变 GameSpec。
- 2026-10-24 自审阶段只做文档同步：未运行任何测试 / Compiler、未 `Refresh`、未保存、未改代码 / 资产 / 配置 / 存档（仅一次只读 `script-execute` 读 `DateTime` 核实时钟）；未提交、未发布、未关闭整包；**R3-B = Implemented / Verification PARTIAL**。
- **2026-10-25 文档纠正阶段**：只读核对并改写 3 份文档 + 新增短报告；未运行 Unity / tests-run / Compiler、未刷新、未保存、未 `rql-mutate`、未 Shell、未 Git 写、未改产品 / 测试 / 资产 / manifest / 证据日志 / 存档或备份；TRX SHA 未重算，按 REPORT 记录值登记。
- 需由主开发专业核验，不代表制作人批准；制作人验收与 Git 发布分别另行授权。

---

## 9. 2026-10-26 最终当前事实（修后自审与保护证据定稿；只读核验）

- 时刻 **2026-10-26T07:28:21Z**（本机 15:28 +08:00，UTC+8）。本阶段只做自审与文档定稿：**未运行测试 / Compiler**、未 `AssetDatabase.Refresh`、未打开 PrefabStage、未 `rql-mutate`、未全局保存、未改代码 / 测试 / Prefab / 字体 / 存档 / manifest / 基线 / Prefs，未 Git 写。唯一 Editor 操作为只读 `rql-query` 与只读 `script-execute`。
- 详细证据、原文摘要与当前代码 SHA 见 `Development/StudioRuns/R3B-UI-20261026/final-current-evidence-review.md`；自审索引见 `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md` §10。
- 状态：**Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；不 Closed / 不 Accepted。

### 9.1 验证事实（当前版本）

| 项 | 当前事实 | 口径 |
|---|---|---|
| 管理 Play 整类 | **12/12 Passed、0 skipped、`11.4823431` s、`scope-restored`，run `4aa4ffda-3730-41be-ad11-2f9550ff31bc`**（主开发已核）；本层只读核对 `testResults` 12 条全 `Passed`、12 组 `[SAVE-RESTORE]` 全 `matches-before=True`、两例 `[SCHEDULE-RESTORE] restored=true sameInstance=True`、`FONT-POST unchanged=True`、`BASELINE-POST total=34 same=34 problems=0` | 当前已验证（12 例范围：原 11 例 + 1920 布局/姓名可见） |
| 管理 Edit 整类 | **18/18 Passed、0 skipped、`1.2183834` s，记录 `99dc1dfe`**（主开发已核）；claim 施工单例 `41a9596f` **1/1 Passed** | 当前已验证 |
| claim 施工时间线 | **464 claim 保持 → 465 合法 Interrupted 释放 → 480 到场零劳动 → 481 劳动 1**；`AddConstructionWorker` `Success`、未新增 busy 拒绝、未改 Work/Clock/Activity；Primary/参与者集/原岗位名额与 0 证据保持 | 源码派生边界（非写死 460/481） |
| 人口行姓名可见 | Prefab 4 个行模板 Label **28→32**（`1072×32`×4、`1072×28`×0）；`[ROW-TITLE-RENDER]`×6 + `[ROW-TITLE-VISIBLE] rows=6 checkedRows=6`；人口 PNG 六行姓名职业可见（主开发目视 + 本层日志核对） | 当前已验证（仅人口页有真实业务行证据） |
| 旧 `b17cd5c1…` | **rejected，错误 `Font already has pending edits`，测试未发送** | **不是断言失败**；当前字体 9 子资产 0 dirty |
| Compiler / 完整 Edit / 完整 Play | `264/264`、`1513/1513`、`122/122` | **前阶段真实通过；未以最新 Prefab / 最新 12 例源码重跑全集** |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过；不待人工、不记 BLOCKED / PASS |

### 9.2 保护面（2026-10-26 只读实测）

| 项 | 实测 |
|---|---|
| 工程 | `2022.3.62t9`；`isPlaying/isPaused/isCompiling/isUpdating=False`；`SampleScene dirty=False roots=5`；`PrefabStage=null`；`IsRunActive=False`；`Screen=1920x1080` |
| 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；磁盘 mtime `2026-10-10T13:23:55Z`；`LoadAllAssetsAtPath` **9 子资产、0 dirty**（TMP_FontAsset + Material + 7 Atlas）；`atlasTextures.Length=8`（1 null 槽）、`lookup=644`、`Dynamic` |
| 34 项 | `total=34 / same=34 / diff=0 / missing=0` |
| 单槽 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，mtime `2026-10-26T07:25:56.5055Z`，无 `.tmp` |
| guard | **202** run：`restored=201` + `protective-snapshot=1`（无 manifest 缺失、无 `captured`/`restore-failed`/`unfinished`）；最新 `20261026T072555Z restored`，manifest mtime `07:25:56.506Z` |
| Git | 只读 `.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`，ref `054b00d3…`；**未执行 Git 命令、无 Git 写** |
| 证据日志 | `layout-input-evidence.log` `41498B / 230 行 / 160b1473…`；`management-shell-play-evidence.log` `665424B / 3004 行 / 70d48944…`（历史行全部保留） |

### 9.3 规格 §4–§8（按原规格逐项，已实现 / 已验证 / 未覆盖 / 取消）

- **§4 集中管理**：三子页、同 Roster 比较、筛选与详情、实例岗位（BuildingInstanceId + FunctionSlotId）容量/绑定/劳动/现场分列、施工行进度与工人、Completed 岗位派工/撤职、施工只走 Add/RemoveConstructionWorker、静态兼容岗位可见可撤、跨页同一 Selection、名额与劳动独立、AlreadyAssigned 拒绝原子改派、撤职删记录不包装无损 Undo、显式保存显示 bool、busy 禁用——**已实现 + 已验证**。资源页批次/供餐细节**部分验证（首片后置）**。claim 施工**已验证**。**第二 FunctionSlot 真实 Gateway 未覆盖**（配置只登记 `main`，无合法注入点；**与 SaveSystem 单槽存档无关**）。拒绝实现清单**已遵守**。
- **§5 页面/UI/路由**：唯一 World/Selection、原 Surface 替换白模、无第二入口、可编辑 Prefab、不 Pause 不改 Speed、关闭回 World 保选择、ESC 同回调、全屏 Blocker + World 阻断、TopHUD 可操作、远征 gating、pending/Return 优先级——**已实现 + 已验证（程序化 Raycast）**；**键盘/镜头/物理鼠标穿透未覆盖**。
- **§6 实施顺序与验收**：顺序已执行；产品验收卡第 6–10 条**已验证**（CASE-A/B/C/D、Return gating）；第 1–5 条属 R3-A 未在本包重跑；**手工手感未覆盖**。
- **§7 风险/未决**：管理命令前置**已验证**；撤职即时边界按事实断言；Growth/Social/死者档案/GameSpec 滞后/Save Ownership/世界级 Load transaction **保持既有缺口**；字体保护**成立**；**1280 取消**。
- **§8 测试矩阵**：Edit 管理命令、Play 真实入口、劳动与资源、Simulation 对照、Save/Continue/NewGame、远征/事件 gating **当前已验证**；模态/布局**部分验证**（1920 布局/遮挡/滚动可达分支，真实 overflow 滚动未覆盖）；人物投影/关系历史/成长**沿用前阶段回执**。

### 9.4 未覆盖（保留，不升级为需批准 / 不新增产品规则）

第二 FunctionSlot 真实 Gateway、真实 overflow 滚动 / 拖拽 / 滚轮 / 物理鼠标、其他业务行标题截图（Workplaces/Resources/Construction 空态）、Chevron、Detail 单行 Ellipsis、历史 650 存档缺口（`21075B / 650c45c4…` 无备份，当前 `13405-4b2b56` 不是它）。

### 9.5 历史异常（保留，不洗白）

- `ManagementView.prefab` 曾被只读打开 PrefabStage 触发**非有意自动序列化回写**：`282097B / ade4c7e1…` → `282326B / 5cf61738…`（+229B）。**只相关、根因未确定**：不能写“插件已确定根因”，也不能断言“仅 4 个数字变化、其余序列化形态未变”（缺逐字节对照）；旧文件无备份、不要求恢复旧 Prefab。
- 历史失败/中止（`b17cd5c1` rejected、`24c81ccf` claim 断言失败、`2026-10-23T15:53Z` 5 次 Capture 无 Restore、`48d8820f` aborted 等）全部保留，不改写为 PASS。

### 9.6 下一步

真实 overflow 滚动可用**合法世界增量**构造（真实推进/新增人物与建筑使 `contentHeight > viewportHeight`，再用真实滚动接口验证末端可达），不伪造 RectTransform / 不手改 Prefab / 不注入假数据——留后续独立包，本包不实现；物理输入、Chevron/Detail、第二 FunctionSlot、历史 650 档、6 份索引文档同步与 Git 定稿分别见 `final-self-audit.md` §7 / §10 与证据索引 §10。

### 9.7 本阶段边界

- 实际写入：本记录（追加 §9）、`Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`（顶部块 + §1 指引 + §10）、新增 `Development/StudioRuns/R3B-UI-20261026/final-current-evidence-review.md`。
- 未更新 6 份索引文档；未写 GameSpec / Authority / manifest / 备份；未安装组件、未改权限、未创建额外付费子 Agent；未访问未授权工程 / 认证目录 / 其他工作区。
- 写后扫描（排除 `.git/Library/Temp/Logs/obj`）：mtime 变化仅本记录、`final-self-audit.md`、新增证据索引 3 个文档，外加 Unity 插件自动刷新的 `UnityProject/RRPG1.0game/.com-unity-codely.json`（心跳文件，非本包刻意写入，不在 34 项基线内）；写后 Editor 仍空闲、场景 `dirty=False`、字体 `14961033B / 3c758e16…` 且 9 子资产 0 dirty、单槽 `13405B / 4b2b56f6…`。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 Implemented / Verification PARTIAL / Not Committed / Not Published。

---

## 10. 2026-10-26 详情修复后整类 13 例（阶段 2/2 交接收尾）

日期：**2026-10-26**（Asia/Shanghai；UTC 08:51–08:56Z 只读实测）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 10.1 本次变化（只这两处产品 / 测试改动，均由前序实现包完成，本阶段未再改）

| 文件 | 变化 | 当前 SHA |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | `WorkplaceDetail` **行序调整**（关键数值提前到首行，身份行保留）＋ 岗位行高 `100→132` | `38207B / 9b59393620c63ddf62cb924ad79e5e7f24a22c717d85c68da99c2dbaf07c8fcb` |
| `Assets/Resources/UI/ManagementView.prefab` | 仅 `WorkplaceRowTemplate`：行高 `100→132`、`LayoutElement 100→132`、`Detail 1072×42→1072×76@(14,-44)`；其余三模板未动 | `282326B / a05a7715e83e5c552c0472fa6465dbe440a5ba623d3969297b7ff8c4a4a0139a` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 仅第 13 例强化：新增详情逐字符可见断言 + 多轮真实拖拽 | `371699B / 03a7c2c8a4d9a5eff8cc59f6d4e20e05eeab38e46e7adffb2d45d2f35cabf0ee` |

详情文本新行序：**① `绑定 b/c · 实际劳动 l · 现场 o`（+未注册/已满员状态）→ ② `岗位 <完整 WorkplaceId>` → ③ `建筑 <BuildingInstanceId> · 槽位 <FunctionSlotId> · 功能 <FunctionProfileId> · 绑定人：…`**。静态兼容岗位同理。单行行高换算 24.616 → 3 行 ≈ 73.85 ≤ Detail 76。

### 10.2 验证（当前 / 历史严格分开）

| 门禁 | 当前结果 | 版本范围 |
|---|---|---|
| **管理 Play 整类 13 例** | **13/13 Passed、`message=null`**（本机 `testResults` 13 条）；日志段 `08:53:18.233Z–08:53:30.321Z`（≈12.09 s）内 13 组 `[SAVE-RESTORE] matches-before=True`、两例 `[SCHEDULE-RESTORE]`、`[FONT-POST]`、`[BASELINE-POST] 34/34` | **本轮（详情修复后）真实回执**；宿主 runId / run 级总时长未读到，不编造 |
| 滚动专项单方法（详情修复后） | 主开发已核 `49fcb102…`：**1/1 Passed、0 skipped、`3.2231345` s、`scope-restored`**；本机 `testResults` 单条 `Passed / 2092ms` | 单例，不并入整类 |
| 真实 overflow 滚动 / 详情可读 | `contentHeight=1004 / viewport 650 / overflowPx=354`；真实 `IScrollHandler` + **多轮拖拽** `strokes=1 steps=6 norm→0 contentY→354`；6 条岗位行 + 1 条施工行 detail 逐字符可见（`checkedChars=51 / 6`）；截图 `T084927Z / T085326Z` | **已覆盖**（程序化 `EventSystem`，非物理指针） |
| 管理 Edit | **18/18**（`99dc1dfe`，主开发已核） | 前阶段，未重跑 |
| Compiler 264 / 完整 Edit 1513 / 完整 Play 122 | **前阶段真实通过** | **不是最新全量**，不得当“最新 13 例全绿”的依据 |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过 |

### 10.3 保护面（2026-10-26T08:51–08:54Z 只读实测）

字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）、内存 9 子资产 0 dirty（`characterTable=632`、`lookup=644`、`atlasTextures=8` 含 1 null 槽）；34 项 `34/34`；单槽 `13405B / 4b2b56f6…d6aa`（mtime `08:53:30.032Z`，本轮收尾回读）；guard **230 = 229 restored + 1 protective-snapshot**（最新 `20261026T085329Z restored / readbackVerified=true`，无 unfinished / noManifest）；场景 `SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080`。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**

### 10.4 未覆盖 / 保留（不升级为需批准，不新增产品规则）

资源页业务行标题（`Resource=NotCoveredByThisCase`）、第二 FunctionSlot 真实 Gateway（配置只登记 `main`，无合法注入点；与单槽存档无关）、物理鼠标手感（本轮为程序化输入）、动态批次 Deferred、Chevron（`44×28/字号24`）、Person/Resource 行 detail 仍单行、历史 650 档（`21075B/650c45c4…` 无备份，当前档不是它）。**两次边界违规原文保留**：旧 `4fbf7754` 的 `ClearDirty`、详情修复包的 `AssetDatabase.SaveAssets()` 越界（原文在 `R3B-UI-20261026/workplace-detail-readability-review.md` §6.3）。

### 10.5 本阶段边界与交付

- 本阶段**未运行 tests-run**、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save、未 SetDirty / ClearDirty、未 `GetPreferredValues` / `ForceMeshUpdate`、未 Shell / Git 写；只读核对结果 + 文档。
- 交付索引：`Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md`（原规格逐项 / 改动文件 / 当前与历史门禁 / 风险 Deferred / 可复现实体入口 / 下一步）。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

---

## 11. 2026-10-26 全部必要明细修复后整类 13 例（最终同步；只读核对 + 文档）

日期：**2026-10-26**（Asia/Shanghai；UTC 10:27–10:33Z 只读实测）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 11.1 本次变化（由前序实现包完成，本阶段未再改）

| 文件 | 变化 | 当前 SHA |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | 人口 / 施工 / 资源三类行文本分行 + 行高 132；`ConstructionDetail` 工人行独立；岗位行序沿用 | `39037B / f8316e436845bd7266cfd3a6f0fa013cdcd3acb5705161a94d5c67e172016473` |
| `Assets/Resources/UI/ManagementView.prefab` | **四个行模板**统一行高 `1100×132`、`LayoutElement 132`、`Detail 1072×76@(14,-44)`、`Label 1072×32@(14,-8)`；`1072×42` / `1072×28` 均 0 处 | `282332B / 31bf3e8c111700332adfaa87d93586ea065631090416cdc2f8cf2629a30e8764` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 第 13 例强化：四类业务行逐字符断言、真实资源业务行、真实人口行、多轮真实拖拽 | `390868B / 219379e01567b36cab0af7f73aee9f133bb027abcf1566be1c69f39f51c7a45c` |

行文本（真实产品文案，未删字段）：人口 ① `在村 · 职责：…` ② `执行：…` ③ `临时承诺：… · 饥饿：…`；施工 ① `建筑 … · 进度 x / y · 施工中` ② `工人 N 人：姓名（PersonId）` ③ `现场 N 人`；资源 ① `名额 绑定 b/c · 实际劳动 l` ② `建筑 … · 槽位 …` ③ 定位说明；岗位 ① `绑定 b/c · 实际劳动 l · 现场 o` ② `岗位 <WorkplaceId>` ③ `建筑 / 槽位 / 功能 / 绑定人`。

### 11.2 验证（当前 / 历史严格分开）

| 门禁 | 当前结果 | 版本范围 |
|---|---|---|
| **管理 Play 整类 13 例** | **13/13 Passed、`message=null`**（方法耗时 547–1521 ms，合计 11 754 ms）；日志段 `10:29:23.276Z–10:29:35.467Z`（≈12.19 s）内 13 组 `[SAVE-RESTORE] matches-before=True`（`102923Z`→`102934Z`）、3 组 `[SCHEDULE-RESTORE]`、`[FONT-POST]`、`[BASELINE-POST] 34/34` | **当前版本真实回执**；宿主 runId / run 级总时长 / skipped **待主开发补** |
| 滚动专项单方法 | 主开发已核 `e75c1ec5…`：**1/1 Passed、0 skipped、`3.1183267` s、`scope-restored`**；本机单条 `Passed / 2144ms` | 单例 |
| 管理 Play 旧口径 | 13/13（08:53 仅岗位行版）、12/12 `4aa4ffda`、11/11 `7f600ef9`、10/10 `0fd4b031` | **历史** |
| 真实 overflow 滚动 / 四类明细 | 职责 `1044/650/394`、资源 `834/650/184`；真实 `IScrollHandler` + 多轮拖拽；岗位 51 / 施工 **58** / 资源 **41** / 人口 **40·37** 逐字符核对；截图 `T102931Z`、`T102929Z` | **已覆盖（程序化 `EventSystem`，非物理指针）** |
| 管理 Edit | **18/18**（`99dc1dfe`） | 前阶段，未重跑 |
| Compiler 264 / 完整 Edit 1513 / 完整 Play 122 | **前阶段真实通过** | **不是最新全量** |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过 |

### 11.3 保护面（2026-10-26T10:30:31Z，13 例运行后只读实测）

字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）+ 9 子资产 0 dirty；34 项 `34/34`；单槽 `13405B / 4b2b56f6…d6aa`（`10:29:35.176Z` 收尾回读，无 `.tmp`）；guard **244 = 243 restored + 1 protective-snapshot**（最新 `20261026T102934Z restored / readbackVerified=true`，无 unfinished / noManifest）；场景 `SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080`、Console `Error/Exception` 0。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**

### 11.4 未覆盖 / 保留（不升级为需批准，不新增产品规则）

第二 FunctionSlot 真实 Gateway（配置只登记 `main`，无合法注入点；与单槽存档无关）、物理鼠标 / 键盘 / 镜头手感（程序化输入）、Chevron（`44×28/字号24`）、`CapacityFull` 域内诊断文案、动态批次 Deferred、`SaveSlotGuard` 进程级缺口、历史 650 档（`21075B/650c45c4…` 无备份，当前档不是它）。**已解决项不再列当前未测**：Person / Resource 行 detail 截断、施工工人列表隐藏、资源业务行标题无证据。**两次边界违规原文保留**：旧 `4fbf7754` 的 `ClearDirty`、详情修复包的 `AssetDatabase.SaveAssets()` 越界（原文在 `R3B-UI-20261026/workplace-detail-readability-review.md` §6.3）。

### 11.5 本阶段边界与交付

- 本阶段**未运行 tests-run**、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save、未 SetDirty / ClearDirty、未 `GetPreferredValues` / `ForceMeshUpdate`、未 Shell / Git 写；只读核对结果 + 文档。
- 交付入口：`Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md`（逐项 / SHA / 门禁 / 风险 / 入口 / 下一步）与 `Development/StudioRuns/R3B-UI-20261026/all-details-delivery-review.md`（本轮保护恢复 / 原规格映射 / 当前差异 / 必要下一步）。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

## 12. 2026-10-27 双槽第 15 例集成后（当前事实；只读核对 + 文档）

日期：**2026-10-27**（Asia/Shanghai；宿主运行段 UTC `02:38:32Z–02:38:46Z`，只读实测 `02:37Z–02:42Z`）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 12.1 本次变化（由前序实现 / 测试包完成，本阶段未再改）

| 文件 | 变化 | 当前 SHA / bytes |
|---|---|---|
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **新增第 15 例** `ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts`（双槽真实 Gateway `CapacityFull` / 真实 UI 派工 / 真实 Save + Continue / 建造态 ESC 公开回调顺序；复用 Editor-only 内存 registry 覆盖与既有 `SaveSlotGuard`、`ResetForTests`、日程恢复链） | `446994B / 05d6de111fd64054cee3ff50deb615a938f4a2a85d98c0c2633d045d03dd1d8a` |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 编译产物（晚于测试源码 3.86 s） | `547840B / b8dae7339260189f68a1880cac5c122c4f3779b0f8e6aa92ccb0d53c7cdcdb1d` |
| `Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | 未改；采样缝维持 Editor-only internal 实例级可逆 | `5077B / 39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` |
| `Assets/Scripts/StaticConfig/StaticConfigRegistry.cs` | 未改；`Set/GetBuildingsOverrideForTests` 仍 Editor-only internal | `29248B / 1c0c62fa77678e5bd25a5dec09ec164d9a2ccbad7054fbb72267838b8d366e75` |
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` / `Assets/Resources/UI/ManagementView.prefab` | 未改（沿用 10-26 行明细 / 行高 132 版本） | `39037B / f8316e43…6473`、`282332B / 31bf3e8c…8764` |

### 12.2 验证（当前 / 历史严格分开）

| 门禁 | 当前结果 | 版本范围 |
|---|---|---|
| **管理 Play 整类 15 例** | **15/15 Passed**、逐条 `message=null`；method 合计 **13 996 ms**（567–1554 ms）；日志段 `02:38:32.155Z–02:38:46.580Z`（≈14.43 s，**仅日志行跨度**）内 15 组 `[SAVE-RESTORE] matches-before=True`、**4 组 `[SCHEDULE-RESTORE] sameInstance=True`**、`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34` | **当前版本真实回执**；15 类 runId / skipped / run 级时长**待主开发补** |
| 双槽单例（上一轮） | `33b8271a-79c1-45d1-b6c3-c9fa6261de63`：**1/1、0 skipped、2.6883936 s、scope-restored** | 主开发原始核；日志 L3861–L3885 |
| 管理 Edit 整类 19 例 | `99761145-ddc9-43d9-a3eb-db6706e59acf`：**19/19、0 skipped、1.1578404 s、scope-restored** | 主开发原始核；本轮未重跑 |
| 输入门禁单例 | `df6c6645-56ce-4b85-92d0-3e4bf8aac552`：**1/1、0 skipped、2.6441609 s、scope-restored** | 主开发原始核；本轮未重跑 |
| 管理 Play 14 例 | `46d65625`：14/14（主开发已核） | **历史**（测试源码 `02:32:36Z` 后新增第 15 例） |
| 管理 Play 13 例 / 12 / 11 / 10 例 | 见 §11 与交付索引 §3 | **历史** |
| Compiler 264 / 完整 Edit 1513 / 完整 Play 122 | 前阶段真实通过（`5eb0ad4c` / `f116b56e` / 宿主普通用户 264） | **不是最新全量；本次不作为完成门槛**（按实际改动必要回归，充分性见 `Development/StudioRuns/R3B-UI-20261027/current-contract-evidence.md` §3） |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过 |

### 12.3 逐变更影响（为什么 15 Play + 19 Edit + 保护面即为必要且充分）

1. **第 15 例会改写同一 fixture 的运行环境**（static registry 覆盖、`SceneManager.LoadScene` + 真实 Continue 重建 host、日程隔离、最重的存档写路径）→ **只有整类 15/15** 能证明前序用例不污染它、它不污染后继用例；单例 1/1 不构成该证明。
2. **`HomeWorldCamera` 采样重构**位于 `ProductHomeWorldHost.Update` 每帧调用的 `ReadInput` 上 → 管理 Play 全部用例的帧循环均经过；生产路径（无测试采样）逐句等同 legacy Input 采样，门禁与 `Pan/Zoom` 未变。
3. **管理渲染改动**（行文本 / 行高）只被管理页消费 → 布局例、滚动例、双槽行例 + Edit 投影 19/19 覆盖。
4. **264 属宿主 .NET 工具链门禁**（`dotnet build/test` + `rrpg-config validate`），非 Unity 产品脚本单测；本轮未改工具链 / 配置 → 非必要回归。Unity 侧当前源码可编译已由**本轮真实运行**（DLL 晚于源码 + 15/15 + 19/19）证明。
5. 若审阅人要求更宽运行时证据：受影响 Play 类为 `ProductHomeFoundationPlayTests`、`VillageVisualPrototypePlayTests`、`BuildingConstructionProductPlayTests`、`BuildingResourceProductPlayTests`、`BuildingVisualReplacementPlayTests`、`R3AGrowthTransactionPlayTests`（均真实帧进入 `ProductHomeWorldHost`）。属**可选加强**，非本次门槛；本包未运行。

### 12.4 保护面（2026-10-27T02:41Z 前后，15 例运行后只读实测）

字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）+ 9 子资产 **0 dirty**；34 项 `34/34`；单槽 `13405B / 4b2b56f6…d6aa`（无 `.tmp`）；guard **275 = 274 restored + 1 protective-snapshot**（最新 `20261027T023845Z`，全部 `readbackVerified=true`，无 unfinished / noManifest）；场景 `SampleScene dirty=False roots=5`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080`、Console `Error/Exception` 0。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**

### 12.5 未覆盖 / 保留（不新增产品规则，不升级为需批准）

真实键盘轮询（`HomeProductShell.cs:95 Input.GetKeyDown(KeyCode.Escape)` → `HandleEscape`）、物理鼠标 / 滚轮 / 拖拽手感与 DPI / 焦点 / IME、Player 构建内行为（采样缝与 registry 缝均 `#if UNITY_EDITOR`）、`CapacityFull` **UI 文案像素**、`SaveSlotGuard` 进程级 abort、Chevron（`44×28 / 字号 24`）、动态批次 Deferred、历史 650 档（`21075B/650c45c4…` 无备份）。**已由本轮关闭**：第二 FunctionSlot 真实 Gateway、UI 双档位 / Save-Load 双 slot、`CapacityFull` 领域诊断、建造态 ESC 公开回调顺序（**回调机制，不是物理键**）。**两次历史边界违规原文保留**（旧 `4fbf7754` 的 `ClearDirty`、详情修复包的 `AssetDatabase.SaveAssets()`），不洗白。

### 12.6 本阶段边界与交付

- 本阶段**未运行 `tests-run`**（15 例由宿主执行）、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save、未 `SetDirty` / `ClearDirty`、未字体测量、未 Shell / Git 写、未构建 Player；只读核对 + 文档。
- 交付入口：`Development/StudioRuns/R3B-UI-20261027/current-fifteen-preflight.md`（阶段 1 预检）、`current-contract-evidence.md`（当前 `requiredChecks` 证据与缺口）、`final-scope-and-contract-review.md`（完成条件与交付范围复核）、`Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md`（§8 当前附记）。

---

## 13. 2026-10-27 ESC 采样修后第 16 例集成（当前事实；只读核对 + 文档）

日期：**2026-10-27**（Asia/Shanghai；宿主运行段 UTC `07:07:38.4677317Z–07:07:53.5415737Z`，只读实测约 `07:06–07:08Z`）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 13.1 本次变化（由前序实现 / 测试包完成，本阶段未再改）

| 文件 | 变化 | 当前 SHA / bytes |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | **唯一产品改动**：`LateUpdate` 的 `Input.GetKeyDown(KeyCode.Escape)` 改为 `SampleEscapePressed()`；`#if UNITY_EDITOR` 内加实例级 `internal` 一次性注入 / 清除；生产默认回落等价；不新增 public API | `17549B / fbcb85949f6876557ee1aed56e5261763f30d74b6d0e12e672ce6a813f573878` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 新增第 16 例 `ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate`；建造门禁段按真实指针阴性对照修正（夹具非产品） | `477356B / 5820d5308e5edadc9eefd53617cb91e14c01564bd8e915429324eeb5fc3c1033` |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 编译产物（晚于 Shell 源码 47.97 s） | `649216B / 1fb4e9e38038f0a4100c5af619e5c0973e33a2d245faf4d9f2b6dbb4b9d2f151` |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 编译产物（晚于测试源码 4.31 s；反射 16 个 `[UnityTest]`） | `565248B / 0801d17f2dc576207cc2cfda81b3493dd865dd00798b61e625b22f7eac8d4200` |
| `HomeWorldCamera.cs` / `StaticConfigRegistry.cs` / `ManagementView.cs` / `ManagementView.prefab` | 未改（沿用既有 Editor-only 采样缝 / registry 缝与 10-26 行明细） | `5077B / 39caba4b…`、`29248B / 1c0c62fa…`、`39037B / f8316e43…`、`282332B / 31bf3e8c…` |

### 13.2 验证（当前 / 历史严格分开）

| 门禁 | 当前结果 | 版本范围 |
|---|---|---|
| **管理 Play 整类 16 例** | **16/16 Passed**、逐条 `message=null`；method 合计 **14 634 ms**；日志 L4139–L4374 内 16 组 `[ENTRY]` / 16 组 `[SAVE-RESTORE] matches-before=True` / 4 组 `[SCHEDULE-RESTORE] sameInstance=True` / 第 16 例 9 行 `[ESC-FRAME]` / `[FONT-POST] unchanged=True` / `[BASELINE-POST] 34/34` | **当前版本真实回执**；16 类 runId / skipped / run 级时长**待主开发补** |
| ESC 单例（同修复版本） | `cc3adfc5-7ede-4a1f-959a-2cdb9581220c`：**1/1、0 skipped、2.0916182 s、scope-restored** | 主开发原始核；日志 L4122–L4138 |
| 管理 Edit 整类 19 例 | `99761145-ddc9-43d9-a3eb-db6706e59acf`：**19/19、0 skipped、1.1578404 s、scope-restored**；当前 Edit 源码 19 个 `[Test]` / `70548B / 64c8c124…` 未变 | 主开发原始核；本轮未重跑 |
| 管理 Play 15 / 14 / 13 例 | 见 §12 / §11 与交付索引 | **历史** |
| Compiler 264 / 完整 Edit 1513 / 完整 Play 122 | 前阶段真实通过（`5eb0ad4c` / `f116b56e` / 宿主普通用户 264） | **不是最新全量；本次不作为完成门槛**（按实际改动必要回归） |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过 |

### 13.3 第 16 例证明的 ESC 机制（严格边界）

- **真实 `LateUpdate` 采样**：`Surface` 由真实帧关闭；`SampleEscapePressed` 是 `LateUpdate` 唯一调用点。
- **一次消费**：注入后立即置 pending=false；`NEGATIVE-FRAMES=3` 保持管理模态，证明不重放。
- **建造门禁**：`Build.IsOpen=True` 时 `HandleEscape` 让路；`buildOpen / definition / ghost / surface / buildCount` 不变，候选等于同帧真实指针换算（`candidate=pointerDerived`），Selection / Clock / Speed 不漂移。
- **finally 清理**：`ClearEscapePressForTests()`；用例结束断言 pending=false 且宿主单例。
- **不冒称**：注入是合成“一帧按下”，**不是 OS 物理 ESC**，也不证明 `Input.GetKeyDown` 引擎实现；建造自身物理 ESC / 右键未注入；未构建 Player，`#if UNITY_EDITOR` 只有静态保证。

### 13.4 保护面（16 例运行后只读实测）

字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）+ 9 子资产 **0 dirty**；34 项 `34/34`；单槽 `13405B / 4b2b56f6…d6aa`（`.tmp=False`，mtime `07:07:53.2514600Z` 收尾回写）；guard **293 = 292 restored + 1 protective-snapshot**（最新 `20261027T070752Z`，全部 `readbackVerified=true`，无 unfinished / noManifest / pendingRestore）；`SampleScene dirty=False roots=5`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080`、Console `Error/Exception` 0。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**

### 13.5 未覆盖 / 保留（不新增产品规则，不升级为需批准）

**仍未覆盖**：OS 物理 ESC、`Input.GetKeyDown` 引擎实现、物理鼠标 / 滚轮 / 拖拽手感、IME / 焦点 / DPI / 多指针时序、Player 构建内行为、`CapacityFull` UI 文案像素、`SaveSlotGuard` 进程级 abort。**已由第 16 例关闭（产品层）**：规格 §8“键盘不穿透”的真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁（**不是物理键**）。**Deferred**：动态批次 / 供餐细节、Chevron。**历史保留**：650 档（`21075B/650c45c4…` 无备份）、两次边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`）原文不洗白。**1280 取消**。

### 13.6 本阶段边界与交付

- 本阶段**未运行 `tests-run`**（16 例由宿主执行）、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save、未 `SetDirty / ClearDirty`、未字体测量、未 Shell / Git 写、未构建 Player；只读核对 + 文档。
- 交付入口：`Development/StudioRuns/R3B-UI-20261027/current-sixteen-preflight.md`（阶段 1）、`current-contract-evidence.md`（§0 当前口径）、`final-scope-and-contract-review.md`（§10）、`Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md`（§9）、`final-professional-handoff.md`（最终专业交接）。
- `requiredChecks`：`execution(real)` 已覆盖（16 类 run 级元数据待补）、`regression(real)` 按实际改动已覆盖、`design-audit(analysis)` 缺口列明；**不 mark Completed / awaiting_acceptance**，父 contract 保持 running 非终态。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。
- `requiredChecks`：`execution(real)` 已覆盖、`regression(real)` 按实际改动已覆盖、`design-audit(analysis)` 缺口列明；**不 mark Completed / awaiting_acceptance**，父 contract 保持 running 非终态。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

---

## 14. 2026-10-27 选人 / 独立详情 / 四命令返修后 17 例（当前事实；只读核对 + 文档）

日期：**2026-10-27**（Asia/Shanghai；本机真实时钟 `2026-10-27T18:36:04.952+08:00`，UTC `10:36:04Z`；宿主运行段 guard/日志时间戳 `10:33:54Z–10:34:12Z`）。目录 `Development/StudioRuns/R3B-UI-20261028/` 为既有分配路径，**未改名、不改历史日期**（本机当前日期为 10-27）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 14.1 本次变化（由返修实现包完成，本阶段未再改任何代码 / Prefab / 测试 / 资产）

| 文件 | 变化 | 当前 SHA / bytes（本层只读重算） |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | 新增 `SelectPersonRow`（只选人、留页）、`OpenSelectedPersonDetail`（独立详情）、序列化字段 `personDetailButton`；四命令 `interactable` 条件与 `actionHint` 逐项原因 | `42941B / e02de745c97edac5b2580aa1f9f33af64f7ae5cdac18bd15f65e2136cb702b3c` |
| `Assets/Resources/UI/ManagementView.prefab` | 新增 authored `OpenPersonDetailButton`（Image→Button→Label，文案“查看人物详情”）；`DetailTitle` 宽 306；新 fileID 段 `1450098202734151201…209` | `290093B / f750ed83ee1c3a7503f3d72a6d5e4900b8c08b16eefd8ade6fe485a8c9f1977b` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 修订第 1 例（人口行留页 + 独立入口 + 原往返覆盖保留）、新增第 17 例用户路径；引用总数断言 `39 → 40` | `495525B / 24e47dcca95e27d027fb4094afada3983f8749ff2e3830d9ba12774434ec0812` |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 编译产物（mtime `10:18:50Z`，晚于 `ManagementView.cs` `08:48:40Z`） | `650240B / 676028ab8290f509aa09f531de42d4c73708a0c355c25759a198e4bd6f0c443d` |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 编译产物（mtime 晚于测试源码 `08:50:01Z`；已编译 `[UnityTest]` 17 个） | `575488B / a9e333362a4bf9cc99e73b53da6d91d2881ddf75fefedf82078ffb52052ca378` |

### 14.2 验证（当前 / 历史严格分开）

| 门禁 | 当前结果 | 版本范围 |
|---|---|---|
| **管理 Play 整类 17 例** | **17/17 Passed、Failed 0、Skipped 0**；逐条 `message=null / stackTrace=null`；method 合计 **16 199 ms**；日志 `sequence 81191–81680` 内 17 组 `[SAVE-RESTORE] matches-before=True` / 17 组 `readback-verified=True` / `[FONT-POST] unchanged=True` / `[BASELINE-POST] 34/34`；运行段 `Error/Exception` 0 | **当前版本真实回执**：宿主记录 `40f35ae0-b7d6-4e37-ad86-235aeb64ae45`（**主开发读原记录**：`00:00:17.2275503`、`scope-restored\|font\|0\|632`）；本层只读 `testResults` 交叉一致 |
| 第 17 例 用户路径 | Passed / 750 ms；`USER-PATH` 5 行日志（row-click / assign / construction-add / construction-remove / remove-assignment） | 当前版本 |
| 第 1 例 产品入口 | Passed / 654 ms；留页 + 独立入口 + 关闭回 World + 重开保留人物 + 40 引用断言 | 当前版本 |
| 管理 Play 16 / 15 / 14 / 13 例 | §13 / §12 / §11 | **历史** |
| 管理 Edit 19 例 | `99761145…`：19/19、0 skipped、1.1578404 s、scope-restored（上游原始核） | 本轮未重跑 |
| Compiler 264 / 完整 Edit 1513 / 完整 Play 122 | 前阶段真实通过 | **不是最新全量、不作本次门槛** |
| 1280×720 | 制作人 `01a53eb8` **取消** | 取消 ≠ 通过 |

### 14.3 本轮关闭的验收缺口（按日志与断言，不夸大口径）

- **人口行单击留页**：`Surface=Management`、`PersonDetail` 不可见、`IsVisible=true`（第 1 / 第 17 例双覆盖）。
- **选人高亮明确**：选中行 `Image.color=(0.28,0.43,0.36,1)`；右栏 `detailTitle` 含姓名、`detailBody` 含 `PersonId`；`actionHint` 逐条说明不可用原因。
- **独立详情入口**：无选人 `interactable=false` + “尚未选择人物”；有选人可点；进既有 Person Surface（同一 `PersonId`），关闭回 World 且 Selection 保留；重开管理仍同一人。
- **跨页保留人物**：切页 / 选岗位行 / 选施工行 / 撤职前回选岗位行，`SelectedPersonId` 全程不变。
- **四命令 enabled + top-hit + 领域结果**：`assign occupants=1`、`construction-add participants=person_e3679e20…`（Primary 保持、占用仍 1）、`construction-remove remainingParticipants=`（Primary 仍保持）、`remove-assignment occupants=0`；状态栏“已绑定 / 已移除”；全程 `Surface=Management`、`IsSimulationActive=true`。
- **不夸大**：top-hit 是**程序化** `RaycastAll + ExecuteEvents.pointerClick`；物理鼠标 / 滚轮 / 拖拽 / 键盘 / 物理 ESC 的 OS 事件与手感**仍未覆盖**；第 17 例不产截图。

### 14.4 保护面（17 例运行后只读实测）

字体 `14961033B / 3c758e16…c404854`（mtime 仍 `2026-10-10T13:23:55Z`）+ 9 子资产 **0 dirty**；34 项 `same=34 / diff=0 / missing=0`（基线 `6866B / bc7216f1…394e` 未改）；用户存档 **`13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`**（`.tmp=False`，mtime `10:34:11.0025280Z`，即本轮收尾回写）；guard **310 = 309 restored + 1 protective-snapshot**、`unfinished=0`、`lastError` 非空 0 个（最新 `20261027T103410Z` `readbackVerified=true`）；`SampleScene dirty=False roots=5`、无 PrefabStage、`isPlaying=false`、`isCompiling=false`。**未恢复旧 `13405 / 4b2b56f6…`；未碰 650 档（`21075B / 650c45c4…`）。** 磁盘 hash 未变只证明观察面，不推断全机零副作用。

### 14.5 未覆盖 / 保留（不新增产品规则，不升级为需批准）

**仍未覆盖**：OS 物理鼠标 / 滚轮 / 拖拽 / 键盘 / 物理 ESC、IME / 焦点 / DPI / 多指针时序、Player 构建内行为（`#if UNITY_EDITOR` 缝）、`SaveSlotGuard` 进程级 abort、`CapacityFull` UI 文案像素。**Deferred**：动态批次 / 供餐细节、Chevron。**取消**：1280×720（`01a53eb8`，取消 ≠ 通过）。**历史不洗白**：650 档无备份不恢复；两次边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`）原文保留；`R3B-UI-20261027/management-selection-acceptance-fix.md` 仍为原字节 `6033B / da52af93…`。

### 14.6 本阶段边界与交付

- 本阶段**未运行 `tests-run`**（17 例由宿主执行）、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save / `SetDirty` / `ClearDirty`、未字体测量（无 `ForceMeshUpdate` / `GetPreferredValues`）、未建 UI 实例、未 Shell / Git / Prefs / 正式工程、未构建 Player、未恢复任何存档；**只读核对 + 文档**。披露：本层一次**只读**统计脚本因空过滤抛 `InvalidOperationException`（Console `sequence 81681–81683`，2 Error + 1 Exception），未写文件，Console 未清除。
- 交付入口：`Development/StudioRuns/R3B-UI-20261028/selection-command-verified-handoff.md`（本轮移交）、`person-selection-command-repair.md` §7（返修报告当前附记）、`compile-discovery-appendix.md`（上一轮只读核验，未改）、`Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md`（§10）。
- `requiredChecks`：`execution(real)` 已覆盖（17 类 run 级元数据已由主开发原始记录提供）、`regression(real)` 按实际改动已覆盖、`design-audit(analysis)` 缺口列明；**不 mark Completed / awaiting_acceptance**。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。
