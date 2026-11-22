# R3-B 交付索引（全部必要明细修复后 · 2026-10-26 基线，2026-10-27 当前附记）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；`UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程 / 认证目录 / 其他工作区。
- 本索引性质：**可审交付入口**，把「原规格逐项状态 / 改动文件与 SHA / 当前与历史门禁 / 风险与 Deferred / 可复现实体入口 / 下一步」集中到一页。它是导航与事实摘要，**不是新的 Authority**，不替代执行记录与自审索引。
- 状态：**R3-B = Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**。**不 Completed / 不 Accepted / 不 Published**；仍待主开发专业核验与制作人产品验收。
- 历史补充（2026-10-26 阶段 2/2）：管理 Edit 整类 **19/19 Passed**（本机 `testResults` 19 条全 Passed、`message=null`；第 19 例为双 FunctionSlot 真实 Gateway）；第二 slot 的入口、输入证据边界见 `two-slot-and-input-review.md`。**未新增 public API、未构建 Player**；`requiredChecks` 仍 **partial**。
- **早段补充（2026-10-27 阶段 2/2，输入门禁后；**现为历史，当前口径见下一条与文末 §8**）**：管理 Play 整类 **14/14 Passed**（`testResults` 14 条全 Passed、`message=null`；方法耗时合计 **12 256 ms**；宿主 `tests-run` 调用级成功 `24 131.7488 ms`，`2026-10-27 10:20:50.7063 +08:00`）；主证据日志 `2026-10-27T02:20:37.866Z–02:20:50.545Z` 段 14 组 `[SAVE-RESTORE] matches-before=True`、**3 例** `[SCHEDULE-RESTORE] sameInstance=True`、`[INPUT-GATE-POSITIVE]` 非零 / MODAL、PERSON 不变 / AFTER-CLOSE 可逆 / CLEANUP、`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34`。**14 类 runId / skipped / run 级时长本层不可见 → 待主开发补**。19 Edit 以主开发原始核 `99761145-ddc9-43d9-a3eb-db6706e59acf`（19/19、0 skipped、1.1578404 s、scope-restored）为准；输入单例以 `df6c6645-56ce-4b85-92d0-3e4bf8aac552`（1/1、0 skipped、2.6441609 s、scope-restored）为准。**更正**：旧“模型未 `tests-run` 所以宿主未运行”的口径不成立——宿主已实际运行；仍不升级为 Completed / Accepted。
- **最新补充（2026-10-27 阶段 2/2，双槽第 15 例后）**：管理 Play 整类 **15/15 Passed**（本机 `testResults` 15 条全 `Passed`、逐条 `message=null`；方法耗时 567–1554 ms、合计 **13 996 ms**；日志段 `2026-10-27T02:38:32.155Z–02:38:46.580Z` 内 **15 组 `[SAVE-RESTORE] matches-before=True`**、**4 组 `[SCHEDULE-RESTORE] sameInstance=True`**（CASE-B / REAL-SCROLL / **CASE-E 双槽** / EVENT-GATE）、`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34`；**15 类 runId / skipped / run 级时长本层不可见 → 待主开发补**）。guard **275 = 274 restored + 1 protective-snapshot**、单槽仍 `13405B/4b2b56f6…d6aa`、34 项与字体未变。**由此关闭的旧未覆盖项**：双槽真实 Gateway / UI / SaveLoad、`CapacityFull` 领域诊断、建造态 ESC 公开回调（**物理 ESC / 键盘轮询仍未覆盖**）。**本文第 1–7 节保留 2026-10-26 / 10-27 早段（14 例）口径**；被本轮取代的具体行见文末 **§8**。
- **最终补充（2026-10-27 阶段 2/2，ESC 采样修后第 16 例）**：管理 Play 整类 **16/16 Passed**（本机 `testResults` 16 条全 `Passed`、逐条 `message=null`；method 合计 **14 634 ms**；日志 **L4139–L4374** 内 16 组 `[ENTRY]` / 16 组 `[SAVE-RESTORE] matches-before=True` / 4 组 `[SCHEDULE-RESTORE] sameInstance=True` / 第 16 例 9 行 `[ESC-FRAME]` / `[FONT-POST] unchanged=True` / `[BASELINE-POST] 34/34`；**16 类 runId / skipped / run 级时长本层不可见 → 待主开发补**）。ESC 已从“仅 public 回调”升级为**真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁**；OS 物理键 / `Input.GetKeyDown` 引擎实现 / 物理手感 / IME / DPI / Player 面仍未覆盖。guard **293 = 292 restored + 1 protective-snapshot**、单槽仍 `13405B/4b2b56f6…d6aa`、34 项与字体未变。**本文第 1–8 节为 15 例及更早口径，历史保留；当前以 §9 为准。**
- **本轮补充（2026-10-27 选人 / 独立详情 / 四命令返修后，**当前口径，见文末 §10；§9 为 16 例历史**）**：管理 Play 整类 **17/17 Passed**（本机 `testResults` 17 条全 Passed、逐条 `message=null / stackTrace=null`；方法耗时 **16 199 ms**；宿主记录 `40f35ae0-b7d6-4e37-ad86-235aeb64ae45` = 17/17、0 skipped、`00:00:17.2275503`、`resourceScopeEnd=scope-restored|font|0|632`，由主开发读原记录确认）；第 1 例 **654 ms**、第 17 例 **750 ms**。日志段内 17 组 `[SAVE-RESTORE] matches-before=True` / 17 组 `readback-verified=True` / `[FONT-POST] unchanged=True` / `[BASELINE-POST] 34/34`；**当前用户档 `13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`（不是 `13405 / 4b2b56f6…`）**；guard **310 = 309 restored + 1 protective-snapshot、unfinished=0**。新增第 17 例 `Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry` 已真实覆盖人口行**留页选中**、**选中高亮**、**独立详情入口**、**跨页保留人物**、**四命令 `interactable` + top-hit + 领域结果**。**仍未覆盖**：物理鼠标 / 滚轮 / 键盘 / 物理 ESC 手感、Player 面行为。详见 `Development/StudioRuns/R3B-UI-20261028/selection-command-verified-handoff.md`。
- 关联文档：`Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md`（§11 本轮）、`Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`（§12 本轮）、`Development/StudioRuns/R3B-UI-20261026/all-details-delivery-review.md`（最终交付审阅）、`all-details-thirteen-preflight.md`（阶段 1 预检）、`remaining-row-details-review.md`（四类明细补齐实现）、`workplace-detail-readability-review.md`（岗位行详情修复）、`real-scroll-review.md`（真实滚动专项）。

---

## 1. 原规格逐项（R3-P Spec §4–§8，按原文范围，不新增规则）

状态口径：**当前已验证** = 有本轮版本真实回执；**已实现** = 代码 / UI 在位但只覆盖部分面；**未覆盖** = 无当前运行证据；**取消** = 制作人明确取消。不把未覆盖升级为“需批准”，也不新增产品规则。

### 1.1 §4 集中管理

| 原规格要求 | 状态 | 当前依据 |
|---|---|---|
| 人口页：同 Roster 姓名 / 职业 / 在村 / 主要职责 / 现时执行 / 临时承诺 / 饥饿可比较 | **已实现 + 当前已验证（渲染层）** | 布局例 `ROW-TITLE-RENDER`×6（`visible=20/5`、`preferredH=30.41`）+ 6 条 `[REAL-SCROLL-DETAIL]`（`lines=3`、`checkedChars=40/37`；`在村 · 职责` / `执行` / `临时承诺 · 饥饿`）+ `T102931Z` 截图 |
| 名称 / 状态筛选，点人物进详情，选人不自动派工 | 已实现 + 当前已验证 | 布局例搜索 / 过滤 top-hit；CASE-A / CASE-D |
| 职责与场所：一行一 `WorkplaceId` + `BuildingInstanceId` + `FunctionSlotId`，**容量 / 绑定 / 劳动 / 现场分列** | 已实现 + 当前已验证（main slot） | 行高 132 / Detail `1072×76`；首行 `绑定 0/2 · 实际劳动 0 · 现场 0`、次行完整 `WorkplaceId`、第三行 `建筑/槽位/功能/绑定人`；6 条岗位行 `checkedChars=51`；截图 `T102931Z` |
| 职责与场所：施工行单列进度与工人 | **已实现 + 当前已验证（三行全可读）** | 施工行 `lines=3`、`checkedChars=58`：`建筑 … · 进度 0 / 10 · 施工中` / `工人 1 人：林恩（person_…）` / `现场 0 人` |
| 已 Completed 动态岗位 `TryAssignPerson` / `RemovePersonAssignment` | 已实现 + 当前已验证 | CASE-A、CASE-D |
| 施工只调用 `TryAddConstructionWorker` / `TryRemoveConstructionWorker` | 已实现 + 当前已验证 | CASE-B + Edit 第 18 例 |
| 资源页：Ingredients / Food、在村供餐对象、Hungry 人数、Farm / Kitchen 实例名额与实际劳动 | **已实现 + 当前已验证（业务行层）** | 6 条真实资源行（`名额 绑定 0/2 · 实际劳动 0` / `建筑 … 槽位 main` / 定位说明，`checkedChars=41`）+ 真实 overflow 滚动 + `T102931Z`；**批次 / 供餐细节仍 Deferred** |
| 动态岗位经 `FunctionBindings` + `BuildingWorkplaceId` / `GetWorkplaceDef` 互核，不按同名合并 | 已实现 + 当前已验证（EditMode 真实 Gateway 双 slot；UI 双档位 / Save-Load 仍未覆盖） | 第 19 例 `ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent` + 整类 19/19；Editor-only internal 内存覆盖，生产 `BuildingConfigDatabase.asset` 仍只登记 `main`；历史“第二 FunctionSlot 未覆盖 / 需授权”保留于 `two-function-slot-scope-review.md` |
| 未完工建筑不出可派工岗位；0 工人保留进度不写 Pause | 已实现 + 当前已验证 | CASE-B 0-worker progress=0 |
| 静态岗位（`ws_farm_01`）显示既有绑定并可撤职，不假装正式建筑 | 已实现 + 当前已验证 | CASE-B legacy row bound=1 assign-disabled remove-enabled |
| 跨页同一 Selection；Building / Place ↔ Person 互跳 | 已实现 + 当前已验证 | CASE-A/C/D + 资源行点击后 `tab=Workplaces` 定位同一行 |
| 名额（长期绑定）与现场 / 实际劳动独立计算 | 已实现 + 当前已验证 | 劳动口径 + CASE-B / D |
| 不提供原子改派：`AlreadyAssigned` 拒绝，显式先撤后派 | 已实现 + 当前已验证 | CASE-A END |
| 撤职移除 Assignment 记录与累计统计，不包装无损 Undo | 已实现 + 当前已验证（含边界） | CASE-D；旧 `arrival` 只在下一边界清 |
| 管理命令成功后显式保存并显示 bool；失败不称已保存 | 已实现 + 当前已验证 | CASE-C SAVE-SUCCESS / FAIL-EXPECTED / RECOVERED |
| 推进 / 返程 busy 时命令与保存禁用并给已有诊断，不自动暂停 / 解锁 | 已实现 + 当前已验证 | Return gating；事件 pending 门禁 |
| claim 持有者加入施工 | 已实现 + 当前已验证 | 时间线 464 保持 → 465 合法 Interrupted 释放 → 480 到场零劳动 → 481 劳动 1；Edit 19/19（第 19 例双槽后整类；18/18 为前阶段历史） |
| 拒绝实现清单（Test Project / 排行榜 / 完成日估算 / 自动最优派工 / 职业转换 / 批量事务 / 经济重平衡 / 仓储物流 / 升级拆除 / 新 Activity 规则 / 破坏性迁移） | 已遵守（未实现） | 代码与文档一致 |

### 1.2 §5 页面、UI 与路由接缝

| 要求 | 状态 |
|---|---|
| 唯一 World / 镜头 / Selection（`ProductHomeWorldHost`），读模型不写 Authority | 已实现 + 当前已验证（三表面等价） |
| 原 Person / Management Surface 替换白模正文，无额外 Scene / 第二 Home 入口 | 已实现 + 当前已验证（`Management_ProductEntry`） |
| `HomeSelectionContext` 来源替换取消页面选择 / 订阅 | 已实现 + 当前已验证 |
| 固定 UI 结构为 Editor 可编辑 Prefab，Runtime 只生成重复行 | 已实现 + 当前已验证 |
| 内部页面不 `SetSimulationActive(false)` / 不 Pause / 不改 Speed；手动 Pause 保持 | 已实现 + 当前已验证 |
| 关闭返回 World 且保留有效选择；ESC 与关闭同一回调 | 已实现 + 当前已验证 |
| 全屏 Raycast Blocker + World 输入阻断（非仅叠加 Panel） | 已实现 + 当前已验证（程序化 Raycast） |
| TopHUD 时间控制在模态允许区可操作 | 已实现 + 当前已验证（`top=Pause`） |
| 键盘 / 镜头 / **物理鼠标**穿透 | **镜头机制已覆盖**（第 14 例：真实 `Host.Update` → 同一 `ReadInput`，合成非零阳性 / 模态阻断 / 关闭可逆）；**物理鼠标 / 键盘 / 物理 ESC 仍未覆盖**（UI 为程序化 Raycast / `ExecuteEvents`；`HandleEscape`、`HandleWorldClick` 为 public 回调） |
| 远征仍走 `Town.EnterProductExpedition` gating，不借内部页恢复 Away Simulation | 已实现 + 当前已验证 |
| 旧 Pending 事件 / Return 阻断优先级保留 | 已实现 + 当前已验证 |

### 1.3 §6–§8（实施顺序、风险、测试矩阵）

- **§6 实施顺序与验收路径**：顺序已按原文执行；产品验收卡第 6–10 条当前已验证（CASE-A/B/C/D、Return gating）；第 1–5 条属 R3-A，沿用既有回执、本包未重跑；**制作人手工点击手感未覆盖**。
- **§7 风险 / 未决 / Stop**：管理命令前置（存在 → 可用性 → 世界锁）已实现并验证；撤职即时边界按事实断言；Growth / Social / 死者档案 / GameSpec 滞后 / Save Ownership / 世界级 Load transaction 保持既有缺口；字体保护成立；**1280 由制作人 `01a53eb8` 取消（取消 ≠ 通过）**。**Stop Conditions 未被违反**。
- **§8 测试矩阵**：Edit 管理命令 **19/19（当前整类；第 19 例双槽）**，18/18 为前阶段历史；管理 Play 真实产品入口 + 劳动 / 资源 / Simulation 对照 / Save-Continue-NewGame / 远征-事件 gating **14/14 当前已验证（含第 14 例输入门禁）**，13/13 为输入门禁前历史；模态与布局：**1920 布局、遮挡、真实 overflow 滚轮 + 多轮拖拽、四类业务行标题与详情、末行可达均已覆盖（程序化 `EventSystem`）**；第 14 例另有**真实 `ProductHomeWorldHost.Update` → 同一 `ReadInput`** 的合成 legacy Input 快照（阳性非零 / 模态阻断 / 关闭可逆，机制实证）；**物理鼠标 / 键盘 / 镜头 / 手感仍未覆盖（1280 取消）**；ESC 仍是 public `HandleEscape` 回调而非物理键；详见 `two-slot-and-input-review.md` 与 `modal-input-gate-review.md` 当前附记；人物只读投影、关系 / 历史、成长沿用前阶段回执。

---

## 2. 改动文件（当前 SHA，只读重算）

| 文件 | 变更性质 | bytes | SHA256 |
|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/ManagementView.cs` | 人口 / 施工 / 资源行文本分行 + 行高 132；`ConstructionDetail` 工人行独立；岗位行序沿用 | **39037** | `f8316e436845bd7266cfd3a6f0fa013cdcd3acb5705161a94d5c67e172016473` |
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` | **四个行模板**统一：行高 `1100×132`、`LayoutElement 132`、`Detail 1072×76@(14,-44)`、`Label 1072×32@(14,-8)`（`1072×42` / `1072×28` 均 0 处） | **282332** | `31bf3e8c111700332adfaa87d93586ea065631090416cdc2f8cf2629a30e8764` |
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | 提取私有 `SampleInput()` + `InputSnapshot`；Editor-only internal 实例级可逆采样覆盖；`ReadInput` public 签名与门禁不变 | **5077** | `39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 原 13 例源码未改；新增第 14 例输入门禁（复用真实 Host / Shell / `SaveSlotGuard`） | **408932** | `0025be07393e97462c1f57e429416fb2b9dcb888f06e8871a1448e6dd47332f1` |
| `UnityProject/RRPG1.0game/Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 编译产物（mtime 晚于源码 6.08 s） | 525312 | `bd131c42ac05aa71c86e924051824b9f7ac559c1cccb17f48c13c901789ca3a4` |
| `UnityProject/RRPG1.0game/Library/ScriptAssemblies/Assembly-CSharp.dll` | 编译产物（mtime 晚于 `HomeWorldCamera.cs` 59.20 s） | 649216 | `f80f8c804f10d9d84248a1f1e7ea717c3ab5ca676521cd2af9f8946a6877c852` |

> 上表为 **2026-10-27 只读重算的当前值**；10-26 的旧 SHA（390868 / `219379e0…`、512512 / `40e41b98…`、647680 / `b31f1b64…`）保留为历史，不再作为当前。`.prefab.meta`（178B / `b1c14b9c…`）与 `ManagementShellPlayTests.cs.meta`（267B / `9da27352…`）未改；`VillageStage1.Tests.dll` 当前 `160768B / d85583f8…`（mtime `2026-10-26T10:49:48Z`），10-26 早段 `153088B / 9d09dabb…` 为历史。

**本轮文档改动（未改代码 / 测试 / Prefab / 字体 / 存档 / manifest / 基线）**：`Docs/DevelopmentSpecs/2026-10-24_…Execution-Record.md`（§11）、`Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`（顶部块 + §12）、`Docs/DevelopmentSpecs/README.md`、`Docs/项目状态快照.md`、`Docs/项目日志.md`、`Docs/文档地图.md`、`Docs/开发验收交接.md`、`Docs/DevelopmentSpecs/2026-10-09_…Spec-V0.1.md`（6 份索引只更新 2026-10-26 当前附记 / 追加同日最终附记，不批量重写历史）、本文件、新增 `all-details-delivery-review.md`。

---

## 3. 门禁：当前 vs 历史（严格分开，禁止合并 / 外推）

| 门禁 | 结果 | 版本范围 / 证据 |
|---|---|---|
| **管理 Edit 整类（当前，双槽第 19 例后）** | **19/19 Passed、0 skipped，run 级 `1.1578404 s`、scope-restored**（method 级 `message=null`，合计 **96 ms**） | 主开发原始核 `99761145-ddc9-43d9-a3eb-db6706e59acf`；本机 `testResults` 19 条全 Passed；MCP server 日志 `2026-10-26 18:55:51.9683` 一次 `tests-run` 调用级成功、`10013.2963 ms`（工具耗时 ≠ run 时长）；runId / skip 以主开发回执为准 |
| 管理 Edit 整类（前阶段） | **18/18 Passed**（`99dc1dfe`） | **历史**，不覆盖当前版本 |
| **管理 Play 整类（当前，2026-10-27 输入门禁后）** | **14/14 Passed**，`message=null`（方法耗时 543–1476 ms，合计 **12 256 ms**） | 本机 `testResults` 14 条；主证据日志 `2026-10-27T02:20:37.866Z–02:20:50.545Z`（≈12.68 s）段 **14 组** `[SAVE-RESTORE] matches-before=True`、**3 例** `[SCHEDULE-RESTORE] sameInstance=True`、`[INPUT-GATE-POSITIVE]` 非零 / MODAL、PERSON 不变 / AFTER-CLOSE 可逆 / CLEANUP、`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34`；宿主 `tests-run` 调用级 `24 131.7488 ms`；**runId / skipped / run 级时长待主开发补**，不把工具耗时写成 run 时长 |
| 管理 Play 整类（2026-10-26 全部明细修复后；历史） | **13/13 Passed**，`message=null`（方法耗时 547–1521 ms，合计 **11 754 ms**） | 本机 `testResults` 13 条；日志 `10:29:23.276Z–10:29:35.467Z` 段 13 组 `[SAVE-RESTORE]`、3 例 `[SCHEDULE-RESTORE]`、`[FONT-POST]`、`[BASELINE-POST] 34/34`；**历史，不覆盖当前 14 例** |
| 滚动专项单方法（全部明细修复后） | **1/1 Passed、0 skipped、`3.1183267` s、`scope-restored`** | 主开发已核 `e75c1ec5-d51c-4239-88f0-4ec5dc1ee4ae`；本机单条 `Passed / 2144ms` |
| 管理 Play 旧口径 | 13/13（08:53 仅岗位行版）、12/12 `4aa4ffda`、11/11 `7f600ef9`、10/10 `0fd4b031` | **历史**，不覆盖当前版本 |
| 完整 Play | 122/122（`5eb0ad4c`） | **前阶段真实通过，不是最新全量** |
| 完整 Edit | 1513/1513（`f116b56e`） | **前阶段真实通过，不是最新全量** |
| Compiler | 264/264（宿主普通用户，非 DSH Shell） | **前阶段真实通过，不是最新全量** |
| 1280×720 | 制作人 `01a53eb8` **取消** | **取消 ≠ 通过**，不得由 1920 / 单例外推 |

**体验证据（当前，全部明细修复后）**：职责 `contentHeight=1044 / viewport 650 / overflowPx=394`；资源 `contentHeight=834 / overflowPx=184`；真实 `IScrollHandler`（`1→0.9555838` / `1→0.9048913`）+ 多轮拖拽（`steps=7 norm→0 contentY 394` / `steps=3 → 184`），末行滚动后 Raycast 命中；四类业务行逐字符 `isVisible` 与墨迹落行核对（岗位 `checkedChars=51`、施工 **58**、资源 **41**、人口 **40/37**），`[REAL-SCROLL-COVERAGE] Workplace=True Construction=True Resource=True Population=True`。

**截图（当前，历史图未覆盖）**：`T102931Z` 三张 —— workplaces `310452B / f4c0069d…`、resources `313330B / d375bf3b…`、population `274780B / 14c446da…`；同轮布局例 `T102929Z` 三张 —— population `229965B / d48cd7c3…`、workplaces `183167B / d8c13a9e…`、resources `176055B / 726645ca…`（均 1920×1080）。同轮 `layout-input-evidence.log`（`71596B / 377 行 / da9de56c…`）记录 `[REAL-SCREEN] 1920x1080`、`[MODAL] world-ray-blocked=True`、各点击命中正确、`[ROW-TITLE-VISIBLE] rows=6 checkedRows=4`（滚动后仅 4 行完整在视口内）、`[LAYOUT-OK]`。

**保护面（2026-10-27T02:21:19Z 只读实测，14 例运行后）**：字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）+ 9 子资产 **0 dirty**；34 项 `34/34`；单槽 `13405B / 4b2b56f6…d6aa`（`02:20:50.263Z` 收尾回读，无 `.tmp`）；guard **259 = 258 restored + 1 protective-snapshot**（本轮新增 14 个 `20261027T0220*` restored、全部 `readbackVerified=true`；无 unfinished / noManifest）；场景 `SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080`、Console `Error/Exception` 0（Play 重载已刷新缓冲，不能反推历史）。**磁盘 hash 未变只证明观察面，不推断全机零副作用。** 10-26 的 244/243 与 13 例保护面保留为历史。

---

## 4. 风险 / Deferred / 未覆盖（如实保留）

| 项 | 现状 | 处置 |
|---|---|---|
| 第二 FunctionSlot 真实 Gateway | **EditMode 真实 Gateway 已覆盖**：Editor-only `internal` 内存覆盖（`Set/GetBuildingsOverrideForTests`）+ 第 19 例；单例 `740fd50c` 1/1、整类 19/19（§3）。生产配置仍只登记 `main`；**未新增 public API、未构建 Player** | UI 双档位渲染 / Save-Load 双 slot 仍未覆盖；历史“需配置 Authority / 公共 API 授权”口径保留于 `two-function-slot-scope-review.md`，不再作为当前阻塞 |
| 物理鼠标 / 键盘 / 镜头 / 手感 | **机制级已补**：第 14 例用 Editor-only internal 实例采样缝，经真实 `Host.Update` → 同一 `ReadInput` 合成非零阳性 / 模态阻断 / 关闭可逆（`df6c6645…` 1/1；14 类 14/14）；**物理面仍未覆盖**：EventSystem 仍是程序化事件，`HandleEscape` 仍是 public 回调，非物理鼠标 / 键盘 / 物理 ESC | 物理输入、焦点 / DPI / IME 与手感仍需制作人 1920 真实窗口手工点击 / 拖拽 / 滚轮 / ESC 并记录；机制 / 物理边界见 `modal-input-gate-review.md` 与 `current-contract-evidence.md` |
| 交付 contract `requiredChecks` | **仍非 Completed**：`execution(real)` 已覆盖（19 Edit / 输入单例 / 14 Play 真实回执）；`regression(real)` **partial**（受影响集成已跑；最新整版本 122 / 1513 / 264 未重跑）；`design-audit(analysis)` 已覆盖但缺口列明（建造态 ESC 路由、Player 面、物理输入 / 手感等） | 不请求完成、不写 `awaiting_acceptance`；R3-B 状态不变；逐项见 `R3B-UI-20261027/current-contract-evidence.md` |
| 动态批次（资源批次 / 供餐细节） | Deferred | 保留，不虚构产出 |
| 历史 650 存档 | `21075B / 650c45c4…` 授权内无字节备份 | 保留缺口，不恢复、不用当前档冒充 |
| Chevron | 仍 `44×28`、字号 24 | 非必要指示，未修未测 |
| `CapacityFull` 域内诊断文案 | 按钮禁用已断言（`FULL-AT-UI disabled=true`） | 诊断文案未单测 |
| `SaveSlotGuard` 进程级缺口 | 只在用例收尾恢复 | 需单独授权设计 out-of-process 兜底 |
| 两次边界违规（历史） | 旧 `4fbf7754` 的 `EditorUtility.ClearDirty`；详情修复包 `AssetDatabase.SaveAssets()` 越界调用 | **原文留存、不洗白**（`workplace-detail-readability-review.md` §6.3） |
| Prefab 历史自动回写 | `282097B→282326B`（+229B），无备份、根因未定 | 保留，不宣称“仅 4 个数字变化” |
| Git | 分支 `ai/rrpg-r3b-population-pilot`，无 Commit / Push | 需 Git 定稿与提交 / 发布授权 |

> 已解决项（不再列为当前未测，历史文本保留标“当时”）：Person / Resource 行 detail 截断、施工行工人列表隐藏、资源业务行标题无真实证据。

---

## 5. 可复现实体验入口（人工，非自动化）

1. 打开 Unity 工程 `UnityProject/RRPG1.0game`，进入 Play，走真实产品 Home。
2. 点击底部 **“管理”** → 三页签。
3. **人口**：姓名检索 / 状态筛选；每条行三行详情应可读：`在村 · 职责：…` / `执行：…` / `临时承诺：… · 饥饿：…`；姓名与职业在行标题可见。
4. **职责与场所**：每条岗位行三行 —— ① `绑定 b/c · 实际劳动 l · 现场 o` ② `岗位 workplace:<instance>:main` ③ `建筑 <instance> · 槽位 main · 功能 … · 绑定人：…`；施工行三行 —— 进度 / `工人 N 人：姓名（PersonId）` / `现场 N 人`。
5. **资源**：每条资源行三行 —— `名额 绑定 b/c · 实际劳动 l` / `建筑 <instance> · 槽位 main` / 定位说明；点击回“职责与场所”同一行。
6. 用**真实鼠标滚轮 / 拖拽**滚动列表到末端，确认末行完整可见并可点击。
7. 执行派工 / 撤职 / 加入施工 / 撤出施工，观察页面刷新与**显式保存** bool 提示；**ESC** 或“关闭”返回 Home 且保留选择。
8. 与自动化口径的区别：UI 自动化是程序化 `RaycastAll + ExecuteEvents`，并直接调用 public `HandleWorldClick` / `HandleEscape`；第 14 例另以 Editor-only 实例采样缝注入合成 legacy Input，经真实 `Host.Update` → 同一 `ReadInput` 形成阳性 / 阻断 / 可逆的**机制**证据，**仍不代表**物理鼠标 / 键盘 / 物理 ESC 或手感；人工观察结论应单独记录，详见 `two-slot-and-input-review.md` 与 `modal-input-gate-review.md`。

---

## 6. 下一步（不在本阶段执行）

1. **制作人产品验收**：以本索引 + 第 5 节入口做一次人工体验（含真实滚轮 / 拖拽与手感），记录通过 / 问题；**不由文档或自动化代替验收**。
2. **主开发补 14 类 runId / run 级时长 / skipped**：19 Edit（`99761145…`，0 skipped、1.1578404 s）与输入单例（`df6c6645…`，0 skipped、2.6441609 s）已由主开发原始核；**仅当前 14 类**的 runId / skipped / run 级时长本层不可见，需补齐后再入档（不把工具调用 `24 131.7488 ms` 写成 run 时长）。
3. **第二 FunctionSlot**：EditMode 真实 Gateway 已由 Editor-only internal 隔离入口覆盖并通过（单例 1/1、整类 19/19）；**不再以“需新授权”作为当前阻塞**。剩余 UI 双档位 / Save-Load 覆盖另立包；历史“需配置 Authority / 公共 API 授权”判断保留在 `two-function-slot-scope-review.md`。
4. **动态批次**：按后续规划另行授权，当前 Deferred。
5. **Git 定稿 / 提交 / 发布**：需单独 Git 授权；当前无 Commit / Push。
6. **不建议**在收尾阶段重跑 264 / 1513 / 122 全集来“换一个数字”；如需覆盖最新 Prefab / 源码差异，应作为独立“最新版本完整回归包”并说明耗时与保护面。

---

## 7. 边界与诚信声明

- 2026-10-27 阶段（只读核对 + 文档）**未运行 `tests-run`**、未 Refresh、未 PrefabStage、未 `rql-mutate`、未 `AssetDatabase.SaveAssets / SaveChanges / SaveDirtyScenes`、未 `SetDirty` / `ClearDirty`、未 `GetPreferredValues` / `ForceMeshUpdate`、未 Shell / Git 写、未写 Preferences、未启停 Editor；**14 例由宿主实际运行**，模型只读 `testResults` / 日志 / guard / 保护面。
- 只读接口：`rql-query`、只读 C#（哈希 / 反射 / 目录与 manifest 统计 / 日志读取）、本地 `read` / `grep` / `read_image`。
- 14 例真实结果来自宿主运行后的 `testResults` 与日志 / guard；**14 类 runId / skipped / run 级时长读不到即写“待主开发补”，不编造**；13 例为输入门禁前历史。
- 管理 Edit 19 例为宿主运行后 `testResults` 的 **19 条 Passed / `message=null`**，runId `99761145-ddc9-43d9-a3eb-db6706e59acf`、**0 skipped、1.1578404 s、scope-restored**；不把工具调用 `10013.2963 ms` 写成 run 时长。
- 交付 contract `requiredChecks` **仍为 partial**；本索引不请求完成、不写 `awaiting_acceptance`。
- 未改 GameSpec / Authority / manifest / 备份 / Prefs / 正式工程；未安装组件、未改权限、未创建额外付费子 Agent。
- 本索引及整包结论须由**主开发专业核验**，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

---

## 8. 2026-10-27 双槽 15 例后当前附记（覆盖与门禁更新；不重写上文历史）

日期：**2026-10-27**（Asia/Shanghai；UTC `02:38:32Z–02:38:46Z` 宿主运行段 + `02:40Z` 前后只读实测）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 8.1 当前门禁（覆盖 §3 中同名的 14 例行）

| 门禁 | 当前结果 | 证据 |
|---|---|---|
| **管理 Play 整类（15 例）** | **15/15 Passed**、逐条 `message=null`；method 合计 **13 996 ms**；日志段 ≈14.43 s（**不是 run 时长**） | 本机 `testResults` 15 条；日志 **L3886–L4108**：15 组 `[ENTRY]` / 15 组 `[SAVE-RESTORE] matches-before=True`（`20261027T023832Z`…`023845Z`）/ 4 组 `[SCHEDULE-RESTORE] sameInstance=True` / `[FONT-POST]` / `[BASELINE-POST] 34/34`；**runId / skipped / run 级时长待主开发补** |
| 管理 Play 14 例（`46d65625`） | 14/14（主开发已核） | **历史**：测试源码 `02:32:36Z` 新增第 15 例后不再代表当前版本 |
| 双槽单例 `33b8271a…` | **1/1、0 skipped、2.6883936 s、scope-restored** | 主开发原始核；日志 L3861–L3885 |
| 管理 Edit 整类 19 例 | **19/19、0 skipped、1.1578404 s、scope-restored** | 主开发原始核 `99761145…`；本轮未重跑 |
| 输入门禁单例 `df6c6645…` | **1/1、0 skipped、2.6441609 s、scope-restored** | 主开发原始核 |
| 完整 Play 122 / 完整 Edit 1513 / Compiler 264 | 前阶段真实通过（`5eb0ad4c` / `f116b56e` / 宿主普通用户） | **不是最新全量，且不作为本次完成门槛**；按实际改动必要回归的充分性说明见 `R3B-UI-20261027/current-contract-evidence.md` §3 |

> 保护的当前值：34 项 `34/34`、字体 `14961033B / 3c758e16…c404854`（9 子资产 0 dirty）、单槽 `13405B / 4b2b56f6…d6aa`（无 `.tmp`）、guard **275 = 274 restored + 1 protective-snapshot**（最新 `20261027T023845Z`，全部 `readbackVerified=true`，无 unfinished / noManifest）、场景 `SampleScene dirty=False`、`PrefabStage=null`、Console `Error/Exception` 0。

### 8.2 被本轮取代的具体表述（逐条对照，历史原文保留）

| 上文位置 | 旧表述 | 当前 |
|---|---|---|
| 顶部 2026-10-27 补充（14 例） | “管理 Play 整类 14/14” | **15/15**（新增第 15 例双槽） |
| §1.1「动态岗位经 `FunctionBindings` 互核」行 | “UI 双档位 / Save-Load 仍未覆盖” | **已覆盖**：真实行点击 + 真实派工 + 真实保存 + 真实 Continue 换源，两槽身份 / 绑定人 / 容量 / 资源 / Clock / 计数逐项连续 |
| §1.2「键盘 / 镜头 / 物理鼠标穿透」行 | “镜头机制已覆盖；物理鼠标 / 键盘 / 物理 ESC 仍未覆盖” | **镜头机制仍为已覆盖**；**建造态 ESC 公开回调顺序已覆盖**；**物理鼠标 / 键盘轮询 / 物理 ESC 仍未覆盖**（不冒称） |
| §3 门禁表「管理 Play 整类（2026-10-27 输入门禁后）」 | 14/14 | **历史**，以 8.1 的 15/15 为当前 |
| §4「第二 FunctionSlot 真实 Gateway」行 | “UI 双档位渲染 / Save-Load 双 slot 仍未覆盖” | **已覆盖**（Editor-only 内存覆盖，生产配置仍只登记 `main`） |
| §4「交付 contract `requiredChecks`」行 | `execution` 已覆盖 / `regression` partial（以全集未重跑为由）/ `design-audit` 缺建造态 ESC | **`regression` 改为“按实际改动必要回归已覆盖”**（全集未重跑**不作门槛**）；**建造态 ESC 已覆盖**；`design-audit` 剩余真实缺口改为**键盘轮询 / 物理输入 / Player 面 / UI 文案 / guard 进程级** |
| §4「`CapacityFull` 域内诊断文案」行 | “诊断文案未单测” | **领域诊断已覆盖**（`DomainDiagnostic=="CapacityFull"` + 零写 + UI 禁用）；**UI 展示文案像素仍未单测** |
| §6 下一步第 2 条 | 待补“14 类 runId” | 待补**“15 类 runId / skipped / run 级时长”**（14 类已成历史） |
| §6 下一步第 3 条 | “剩余 UI 双档位 / Save-Load 覆盖另立包” | **已由第 15 例关闭**，不再列为待办 |

### 8.3 仍然成立 / 仍未覆盖（不因本轮升级）

- **仍未覆盖（真实存在）**：真实键盘轮询（`Input.GetKeyDown(KeyCode.Escape)` → `HandleEscape`）、物理鼠标 / 滚轮 / 拖拽手感、DPI / 焦点 / IME、Player 构建内行为（采样缝与 registry 缝均 `#if UNITY_EDITOR`）、`SaveSlotGuard` 进程级 abort、`CapacityFull` **UI 文案像素**。
- **规格边界**：R3-P §8 只要求“点击 / 拖拽 / 滚轮 / 键盘**不穿透**”，**未把“手感”列为验收项** → 物理手感**不得**作为原规格新增强制验收；但“键盘不穿透”仍是规格项，**其真实按键路径未测，不得说已测**。
- **取消项**：1280×720 由制作人 `01a53eb8` 取消（取消 ≠ 通过），不再询问、不由 1920 外推。
- **不洗白**：两次历史边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`）与历史 650 档（`21075B / 650c45c4…`）原文保留、不恢复、不伪造。
- **交付 contract**：仍**非 Completed / 非 awaiting_acceptance**；`execution(real)` 已覆盖、`regression(real)` 按必要范围已覆盖、`design-audit(analysis)` 缺口列明；若需对外标注，Evidence 只能为 **partial**。
- 本轮**模型未运行 `tests-run`**（15 例由宿主执行）；未 Refresh、未 `rql-mutate`、未 PrefabStage、未字体测量、未全局 Save / SetDirty / ClearDirty、未 Shell / Git / Prefs / 正式工程、未构建 Player。

---

## 9. 2026-10-27 ESC 采样修后 16 例（当前附记；不重写 §1–§8）

日期：**2026-10-27**（Asia/Shanghai；宿主运行段 UTC `07:07:38.4677317Z–07:07:53.5415737Z`，只读实测约 `07:06–07:08Z`）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 9.1 当前门禁（覆盖 §3 / §8 同名 15 例行）

| 门禁 | 当前结果 | 证据 |
|---|---|---|
| **管理 Play 整类（16 例）** | **16/16 Passed**、逐条 `message=null`；method 合计 **14 634 ms** | 本机 `testResults` 16 条；日志 **L4139–L4374**：16 组 `[ENTRY]` / 16 组 `[SAVE-RESTORE] matches-before=True` / 4 组 `[SCHEDULE-RESTORE]` / 第 16 例 `[ESC-FRAME]` 9 行 / `[FONT-POST]` / `[BASELINE-POST] 34/34`；**runId / skipped / run 级时长待主开发补** |
| ESC 单例 `cc3adfc5…` | **1/1、0 skipped、2.0916182 s、scope-restored** | 主开发原始核；日志 L4122–L4138 |
| 管理 Edit 19 例 | **19/19、0 skipped、1.1578404 s、scope-restored** | 主开发原始核 `99761145…`；本轮未重跑 |
| 双槽单例 / 输入单例 | `33b8271a…` 1/1、`df6c6645…` 1/1（均为上游原始核） | 历史仍有效 |
| 完整 Play 122 / Edit 1513 / Compiler 264 | 前阶段真实通过 | **不是最新全量、不作本次门槛** |

### 9.2 本轮关闭 / 仍保留

- **关闭**：规格 §8“键盘不穿透”的**产品层路由**——第 16 例证明真实 `LateUpdate → SampleEscapePressed → HandleEscape` 采样、一次消费、3 个阴性帧不重放、`Build.IsOpen` 时让路、`finally` 清注入且宿主单例。
- **仍保留（不冒称）**：OS 物理 ESC、`Input.GetKeyDown` 引擎实现、物理鼠标 / 滚轮 / 拖拽手感、IME / 焦点 / DPI / 多指针时序、Player 构建内行为（`#if UNITY_EDITOR` 静态排除）、`SaveSlotGuard` 进程级 abort、`CapacityFull` UI 文案像素。
- **取消**：1280×720（制作人 `01a53eb8`，取消 ≠ 通过）。
- **历史保留**：650 档（`21075B/650c45c4…`，无备份、不恢复不伪造）；两次边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`）原文不洗白。
- **Deferred**：动态批次 / 供餐细节、Chevron、UI 文案像素、guard 进程级、Player 实证（独立授权）。

### 9.3 当前保护面（16 例运行后只读实测）

34 项 `34/34`；字体 `14961033B / 3c758e16…c404854`、9 子资产 0 dirty；单槽 `13405B / 4b2b56f6…d6aa`（无 `.tmp`，mtime `07:07:53.2514600Z` 收尾回写）；guard **293 = 292 restored + 1 protective-snapshot**（最新 `20261027T070752Z`，全部 `readbackVerified=true`，无 unfinished / noManifest / pendingRestore）；`SampleScene dirty=False roots=5`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080`、Console `Error/Exception` 0。

### 9.4 对 §1.2 / §4 的取代与仍成立

| 上文位置 | 旧表述 | 当前 |
|---|---|---|
| §1.2「键盘 / 镜头 / 物理鼠标穿透」 | “物理鼠标 / 键盘 / 物理 ESC 仍未覆盖；ESC 仍 public 回调” | 镜头 / 滚轮机制仍覆盖；**ESC 产品层真实 `LateUpdate` 采样已覆盖**；物理键 / 物理鼠标 / 手感仍未覆盖 |
| §4「物理鼠标 / 键盘 / 镜头 / 手感」 | “`HandleEscape` 仍是 public 回调” | 产品层采样路径已被第 16 例覆盖；OS 物理键 / 引擎实现仍未覆盖 |
| §4「交付 contract `requiredChecks`」 | `execution` 已覆盖 / `regression` 按必要范围 / `design-audit` 缺口列明 | 结论不变；16 类 run 级元数据待补，Evidence 仍 **partial** |
| §8.3「仍未覆盖」首项 | 真实键盘轮询未覆盖 | **产品层已覆盖**；OS 物理键 / 引擎实现仍未覆盖 |

> 本轮模型未 `tests-run`（16 例由宿主执行）；未 Refresh、未 `rql-mutate`、未 PrefabStage、未字体测量、未全局 Save / SetDirty / ClearDirty、未 Shell / Git / Prefs / 正式工程、未构建 Player；未改任何代码 / 测试 / 资产 / 基线 / contract。需主开发专业核验，不代表制作人批准。

---

## 10. 2026-10-27 选人 / 独立详情 / 四命令返修后 17 例（当前口径；不重写 §1–§9）

日期：**2026-10-27**（Asia/Shanghai；本机真实时钟 `2026-10-27T18:36:04.952+08:00`，UTC `10:36:04Z`。目录 `R3B-UI-20261028` 为既有分配路径，未改名、不改历史日期）。状态不变：**Implemented / Verification PARTIAL / Not Committed / Not Published**。

### 10.1 当前门禁（覆盖 §3 / §8 / §9 同名 16 例行）

| 门禁 | 当前结果 | 证据 |
|---|---|---|
| **管理 Play 整类（17 例）** | **17/17 Passed**、Failed 0、Skipped 0；逐条 `message=null / stackTrace=null`；method 合计 **16 199 ms**；宿主 run `00:00:17.2275503`、`scope-restored\|font\|0\|632` | 宿主记录 `40f35ae0-b7d6-4e37-ad86-235aeb64ae45`（**主开发读原记录确认**，本层不可读 Studio 私有 runtime）；本层只读 `testResults` 17 条交叉一致；日志 `sequence 81191–81680`：17 组 `[SAVE-RESTORE] matches-before=True`、17 组 `readback-verified=True`、`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34`、运行段 `Error/Exception` 0 |
| 第 17 例（用户完整路径） | **Passed、750 ms** | 日志 `USER-PATH` 5 行 + 12 条程序化点击记录 |
| 第 1 例（产品入口 / 人口行往返） | **Passed、654 ms** | 修订后：人口行单击留页 + 独立入口进 Person + 关闭回 World 保留选择 + 重开保留人物 + `AssertSerializedObjectReferences(…, 40)` |
| 管理 Play 16 / 15 / 14 / 13 例 | §9 / §8 / §3 | **历史**，不覆盖当前 17 例 |
| 管理 Edit 19 例 / 完整 Play 122 / Edit 1513 / Compiler 264 | 前阶段真实通过（`99761145…` / `5eb0ad4c` / `f116b56e` / 宿主普通用户） | **不是最新全量、不作本次门槛** |

### 10.2 本轮关闭的验收缺口（只用真实日志 / 断言说话）

- **人口行单击留页 + 选中**：`surface=Management; detail-open=False; highlight=True`；`Selection` 与 `SelectedPersonId` 同为 `person_e3679e20…`；选中行 `Image.color=(0.28,0.43,0.36,1)`。
- **独立详情入口**：文案“查看人物详情”、无选人时禁用 + 提示“尚未选择人物”；真实点击进既有 Person Surface（同一 `PersonId`），关闭回 World 且选择保留。
- **跨页保留人物**：切“职责与场所”、选岗位行、选施工行、撤职前回选岗位行，`SelectedPersonId` 均不变。
- **四命令 enabled / top-hit / 领域结果**：`assign occupants=1` → `construction-add participants=person_e3679e20…（Primary 保持）` → `construction-remove remainingParticipants=（空，Primary 仍保持）` → `remove-assignment occupants=0`；全程 `Surface=Management`、`simulationActive=True`；四按钮 `interactable` 逐段断言，点击均为 `RaycastAll` top-hit + `ExecuteEvents.pointerClick`。
- **不夸大**：以上 top-hit 是**程序化**命中；物理鼠标 / 滚轮 / 键盘 / 物理 ESC 手感、DPI / 焦点 / IME、Player 包内行为**仍未覆盖**。

### 10.3 保护面（17 例运行后只读实测，`2026-10-27T18:36Z` 前后）

字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）+ 9 子资产 **0 dirty**；34 项 `same=34 / diff=0 / missing=0`（基线文件 `6866B / bc7216f1…394e` 未改）；用户存档 **`13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`**（`.tmp=False`，mtime `10:34:11.0025280Z`，**是本轮测试收尾回写的当前档**）；guard **310 = 309 restored + 1 protective-snapshot**，`unfinished=0`、`lastError` 非空 0 个，最新 `20261027T103410Z` `readbackVerified=true`；`SampleScene dirty=False roots=5`、无 PrefabStage、`isPlaying/isCompiling=False`；`Assembly-CSharp.dll 650240B / 676028ab…`、`BattleStage1.PlayModeTests.dll 575488B / a9e33336…`（mtime `10:18:50Z`，晚于三份源码）。**未恢复旧 `13405 / 4b2b56f6…`，未碰 650 档。** 磁盘 hash 未变只证明观察面，不推断全机零副作用。

### 10.4 仍成立 / 未覆盖 / 历史保留

- **仍成立**：17/17 只覆盖“程序化 UI + 领域命令 + 保存/恢复链路”；物理输入、Player 面、`SaveSlotGuard` 进程级 abort、`CapacityFull` UI 文案像素仍缺。
- **Deferred / 取消**：动态批次与供餐细节、Chevron 未变；1280×720 由制作人 `01a53eb8` **取消（取消 ≠ 通过）**。
- **历史保留**：650 档（`21075B / 650c45c4…`，无备份、不恢复不伪造）；两次边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`）原文不洗白；`R3B-UI-20261027/management-selection-acceptance-fix.md` 仍为原字节 `6033B / da52af93…`。
- **本层边界**：未 `tests-run`、未 Refresh、未 PrefabStage、未字体测量、未建 UI 实例、未 `rql-mutate`、未保存/置脏、未 Shell/Git/Prefs/正式工程；只读 `rql-query` / 只读 `script-execute` / 本地文档读写。披露：本层一次只读统计脚本因空过滤抛 `InvalidOperationException`（序列 81681–81683，2 Error + 1 Exception），未写任何文件，Console 未清除。
- 详细移交：`Development/StudioRuns/R3B-UI-20261028/selection-command-verified-handoff.md`；返修报告当前附记：同目录 `person-selection-command-repair.md` §7。**需主开发专业核验，不代表制作人批准。**

