# R3-B 最终专业交接（ESC 采样修后 16 例集成 · 2026-10-27）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 本文件性质：**只读核证 + 文档交接**。依据宿主真实最终门禁，对原规格 §4–§8、实现文件、当前真实证据、回归口径、保护链、历史保留项与父 contract 三个 `requiredChecks` 给**可审的条件结论**；**不新增门槛、不降低门槛、不修改 contract / requiredChecks、不直接改任务状态、不写 `awaiting_acceptance`**。
- 版本口径：当前 **2026-10-27（ESC 真实 `LateUpdate` 采样缝 + 第 16 例整类集成后）**；2026-10-26 与 2026-10-27 早段（13 / 14 / 15 例）一律标历史，只读引用、不改写。
- 核对时刻：**2026-10-27T07:08Z 前后**（本机 `15:08 +08:00`，China Standard Time）；宿主 16 例运行段 UTC `07:07:38.4677317Z–07:07:53.5415737Z`。
- 最终状态口径：R3-B = **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；父 contract 保持 **running 非终态**；Evidence 层级只能标 **partial**。本文件须由**主开发专业核验**，不代表制作人批准。

---

## 1. 当前最终门禁（真实回执，先摆事实）

| 门禁 | 当前结果 | 证据 |
|---|---|---|
| **管理 Play 整类（16 例，最终）** | **16/16 Passed、逐条 `message=null`、无 Failed** | 本机 `testResults` 16 条；方法耗时合计 **14 634 ms**（555–1446 ms/例）；主证据日志 **L4139–L4374**：16 组 `[ENTRY]`、16 组 `[SAVE-CAPTURE]`、**16 组 `[SAVE-RESTORE] matches-before=True`**、4 组 `[SCHEDULE-RESTORE] sameInstance=True`、第 16 例 9 行 `[ESC-FRAME]`、`[FONT-POST] unchanged=True`、`[BASELINE-POST] total=34 same=34 changed=0 problems=0` |
| 运行级元数据 | **runId / skipped / run 级时长本层不可见** | `testResults` 仅 `name / status / durationMs / message / stackTrace`；日志行跨度约 15.07 s 与 method 合计**都不是** run 时长 → **待主开发补**，不编造 |
| **ESC 单例（同修复版本，上游已核）** | **1/1 Passed、0 skipped、2.0916182 s、scope-restored** | 主开发原始核 `cc3adfc5-7ede-4a1f-959a-2cdb9581220c`；本机日志 L4122–L4138 |
| **管理 Edit 整类（19 例，当前源码未变）** | **19/19 Passed、0 skipped、1.1578404 s、scope-restored** | 主开发原始核 `99761145-ddc9-43d9-a3eb-db6706e59acf`；源码 19 个 `[Test]`、`70548B / 64c8c124…`、DLL `160768B / d85583f8…`，均 `2026-10-26T10:49Z`，早于该回执且之后未再变 |
| 管理 Play 15 / 14 / 13 例 | 历史（`02:38` / `02:20` / `10:29` 段） | **不覆盖当前 16 例**，不重写 |
| 完整 Play 122 / 完整 Edit 1513 / Compiler 264 | 前阶段真实通过（`5eb0ad4c` / `f116b56e` / 宿主普通用户 264） | **不是最新全量，也不作为本次完成门槛**（§4） |

> 16 例整类由**宿主 Studio 阶段 check** 执行；**模型未运行 `tests-run`**，本层只读 `testResults` / 证据日志 / guard / 保护面。

---

## 2. 原规格 §4–§8 逐项（按原文范围，不新增规则）

状态口径：**当前已验证** = 有当前版本真实回执；**已实现** = 代码 / UI 在位但只覆盖部分面；**未覆盖** = 无当前运行证据；**取消** = 制作人明确取消。

### 2.1 §4 集中管理

| 原规格要求 | 状态 | 当前依据 |
|---|---|---|
| 人口页：同 Roster 姓名 / 职业 / 在村 / 主要职责 / 现时执行 / 临时承诺 / 饥饿可比较 | 已实现 + 当前已验证（渲染层） | 布局例 `ROW-TITLE-RENDER`×6 + 6 条 `[REAL-SCROLL-DETAIL]`（`lines=3`、`checkedChars=40/37`）+ `T102931Z` 截图 |
| 名称 / 状态筛选，点人物进详情，选人不自动派工 | 已实现 + 当前已验证 | 布局例搜索 / 过滤 top-hit；CASE-A / CASE-D |
| 职责与场所：一行一 `WorkplaceId` + `BuildingInstanceId` + `FunctionSlotId`，容量 / 绑定 / 劳动 / 现场分列 | 已实现 + 当前已验证（含同一实例 main + aux 两行） | 行高 132 / Detail `1072×76`；第 15 例 `SLOTS` / `LABOR`；6 条岗位行 `checkedChars=51` |
| 施工行单列进度与工人 | 已实现 + 当前已验证 | 施工行 `lines=3`、`checkedChars=58`：进度 / `工人 N 人：姓名（PersonId）` / 现场 |
| 已 Completed 动态岗位 `TryAssignPerson` / `RemovePersonAssignment` | 已实现 + 当前已验证 | CASE-A、CASE-D |
| 施工只调用 `TryAddConstructionWorker` / `TryRemoveConstructionWorker` | 已实现 + 当前已验证 | CASE-B + Edit 第 18 例 |
| 资源页：Ingredients / Food、在村供餐对象、Hungry、Farm / Kitchen 名额与实际劳动 | 已实现 + 当前已验证（业务行层） | 6 条真实资源行（`checkedChars=41`）+ 真实 overflow 滚动 + `T102931Z`；**批次 / 供餐细节 Deferred** |
| 动态岗位经 `FunctionBindings` + `BuildingWorkplaceId` / `GetWorkplaceDef` 互核，不按同名合并 | 已实现 + 当前已验证（**含第二 FunctionSlot 真实 Gateway / UI / SaveLoad**） | Edit 第 19 例 `ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent` + 19/19；Play 第 15 例真实 UI 派工 / 保存 / Continue；Editor-only 内存覆盖，生产配置仍只登记 `main` |
| 未完工建筑不出可派工岗位；0 工人保留进度不写 Pause | 已实现 + 当前已验证 | CASE-B 0-worker progress=0 |
| 静态岗位（`ws_farm_01`）显示既有绑定并可撤职，不假装正式建筑 | 已实现 + 当前已验证 | CASE-B legacy row bound=1 assign-disabled remove-enabled |
| 跨页同一 Selection；Building / Place ↔ Person 互跳 | 已实现 + 当前已验证 | CASE-A/C/D + 资源行点击后 `tab=Workplaces` 定位同一行 |
| 名额（长期绑定）与现场 / 实际劳动独立计算 | 已实现 + 当前已验证（按 slot 独立） | CASE-B / D；第 15 例 `main 2/2/2`、`aux 1/1/1`、`cross=0` |
| 不提供原子改派：`AlreadyAssigned` 拒绝，显式先撤后派 | 已实现 + 当前已验证 | CASE-A END |
| 撤职移除 Assignment 记录与累计统计，不包装无损 Undo | 已实现 + 当前已验证（含边界） | CASE-D；旧 `arrival` 只在下一边界清 |
| 管理命令成功后显式保存并显示 bool；失败不称已保存 | 已实现 + 当前已验证 | CASE-C `SAVE-SUCCESS / FAIL-EXPECTED / RECOVERED` |
| 推进 / 返程 busy 时命令与保存禁用并给已有诊断，不自动暂停 / 解锁 | 已实现 + 当前已验证 | Return gating；事件 pending 门禁 |
| 满员语义：已有域内诊断 + UI 禁用 | 已实现 + 当前已验证（**领域诊断值**） | 第 15 例 `FULL`：`DomainRejected` / `DomainDiagnostic=="CapacityFull"` / 零写 / 按钮禁用；**UI 展示文案像素仍未单测** |
| 拒绝实现清单（Test Project / 排行榜 / 完成日估算 / 自动最优派工 / 职业转换 / 批量事务 / 经济重平衡 / 仓储物流 / 升级拆除 / 新 Activity 规则 / 破坏性迁移） | 已遵守（未实现） | 代码与文档一致 |

### 2.2 §5 页面、UI 与路由接缝

| 要求 | 状态 |
|---|---|
| 唯一 World / 镜头 / Selection（`ProductHomeWorldHost`），读模型不写 Authority | 已实现 + 当前已验证（三表面等价） |
| 原 Person / Management Surface 替换白模正文，无额外 Scene / 第二 Home 入口 | 已实现 + 当前已验证（`Management_ProductEntry`） |
| `HomeSelectionContext` 来源替换取消页面选择 / 订阅 | 已实现 + 当前已验证 |
| 固定 UI 结构为 Editor 可编辑 Prefab，Runtime 只生成重复行 | 已实现 + 当前已验证 |
| 内部页面不 `SetSimulationActive(false)` / 不 Pause / 不改 Speed；手动 Pause 保持 | 已实现 + 当前已验证 |
| 关闭返回 World 且保留有效选择；ESC 与关闭同一回调；嵌套不抢底层 | 已实现 + 当前已验证（**真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁**，见 §6） |
| 全屏 Raycast Blocker + World 输入阻断 | 已实现 + 当前已验证（程序化 Raycast） |
| TopHUD 时间控制在模态允许区可操作 | 已实现 + 当前已验证（`top=Pause`） |
| 键盘 / 镜头 / 鼠标**不穿透** | **机制已覆盖、物理面未覆盖**：镜头 / 滚轮经真实 `Host.Update` → 同一 `ReadInput` 合成阳性 / 阻断 / 可逆；ESC 经真实 `LateUpdate → SampleEscapePressed → HandleEscape` 一次采样；**OS 物理键 / 物理鼠标 / IME / DPI / 手感仍未覆盖**（§7） |
| 远征仍走 `Town.EnterProductExpedition` gating，不借内部页恢复 Away Simulation | 已实现 + 当前已验证 |
| 旧 Pending 事件 / Return 阻断优先级保留 | 已实现 + 当前已验证 |

### 2.3 §6–§8（实施顺序、风险、测试矩阵）

- **§6 实施顺序与验收路径**：顺序按原文执行；产品验收卡第 6–10 条当前已验证（CASE-A/B/C/D、Return gating）；第 1–5 条属 R3-A，沿用既有回执、本包未重跑；**制作人手工点击手感未覆盖**。
- **§7 风险 / 未决 / Stop**：管理命令前置（存在 → 可用性 → 世界锁）已实现并验证；撤职即时边界按事实断言；Growth / Social / 死者档案 / GameSpec 滞后 / Save Ownership / 世界级 Load transaction 保持既有缺口；字体保护成立；**1280 由制作人 `01a53eb8` 取消（取消 ≠ 通过）**；**Stop Conditions 未被违反**。
- **§8 测试矩阵**：管理 Edit **19/19**（第 19 例双槽）；管理 Play 真实产品入口 + 劳动 / 资源 / Simulation 对照 / Save-Continue-NewGame / 远征-事件 gating / 输入门禁 / 双槽 / **ESC 帧轮询** **16/16**；模态与布局：1920 布局、遮挡、真实 overflow 滚轮 + 多轮拖拽、四类业务行标题与详情、末行可达均已覆盖（程序化 `EventSystem` / `ExecuteEvents`）；**物理输入 / 手感 / 1280 未覆盖（1280 已取消）**；人物只读投影、关系 / 历史、成长沿用前阶段回执。

---

## 3. 实现文件 / SHA / 理由（当前只读重算）

| 文件 | bytes | SHA256 | 本包理由 / 边界 |
|---|---|---|---|
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | **17549** | `fbcb85949f6876557ee1aed56e5261763f30d74b6d0e12e672ce6a813f573878` | **唯一产品改动**：`LateUpdate` 的 `Input.GetKeyDown(KeyCode.Escape)` 改为调用私有 `SampleEscapePressed()`；`#if UNITY_EDITOR` 内加实例级 `internal` 一次性注入 / 清除。生产默认回落逐句等价；不新增 public API；Player 编译不含字段 / 方法 / 分支 |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **477356** | `5820d5308e5edadc9eefd53617cb91e14c01564bd8e915429324eeb5fc3c1033` | 新增第 16 例 `ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate`；原 15 例未改写；建造门禁段按真实指针阴性对照修正（夹具非产品） |
| `Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | 5077 | `39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` | 前阶段 Editor-only 实例级输入采样缝；`ReadInput` 签名 / 门禁未变，本轮未改 |
| `Assets/Scripts/StaticConfig/StaticConfigRegistry.cs` | 29248 | `1c0c62fa77678e5bd25a5dec09ec164d9a2ccbad7054fbb72267838b8d366e75` | 前阶段 Editor-only `Set/GetBuildingsOverrideForTests`；默认无覆盖，本轮未改 |
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | 39037 | `f8316e436845bd7266cfd3a6f0fa013cdcd3acb5705161a94d5c67e172016473` | 10-26 行文本 / 行高；本轮未改 |
| `Assets/Resources/UI/ManagementView.prefab` | 282332 | `31bf3e8c111700332adfaa87d93586ea065631090416cdc2f8cf2629a30e8764` | 10-26 四模板统一；本轮未改 |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 649216 | `1fb4e9e38038f0a4100c5af619e5c0973e33a2d245faf4d9f2b6dbb4b9d2f151` | 晚于 `HomeProductShell.cs` 47.97 s，编译有效 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 565248 | `0801d17f2dc576207cc2cfda81b3493dd865dd00798b61e625b22f7eac8d4200` | 晚于测试源码 4.31 s；反射 16 个 `[UnityTest]` |
| `Library/ScriptAssemblies/VillageStage1.Tests.dll` | 160768 | `d85583f889a4a6bd1e34cdb3c7f31eede5c1e8f3c124748faab6f86309c8e379` | 19 Edit 用例所在 DLL，晚于 Edit 源码；本轮未重跑 |

> 默认生产路由 / 公共 API / Authority 未改：`HandleEscape()` / `CloseSurface()` 等公开签名不变；ESC 测试缝 public 计数为 0；`SampleEscapePressed` 为 private，`LateUpdate` 为 private，注入 / 清除为 `internal`。未改 SaveVersion / DTO / 配置 / GameSpec。

---

## 4. 当前 16 Play 与 19 Edit 的真实证据（方法级 / 运行级分开）

### 4.1 16 Play 整类（最终）

- **16/16 Passed、逐条 `message=null`**；方法耗时合计 **14 634 ms**（555–1446 ms/例）。
- 日志 **L4139–L4374**（`2026-10-27T07:07:38.4677317Z`→`07:07:53.5415737Z`）结构：**16 组 `[ENTRY]` / 16 组 `[SAVE-CAPTURE]` / 16 组 `[SAVE-RESTORE] matches-before=True` / 4 组 `[SCHEDULE-RESTORE] sameInstance=True` / 9 行 `[ESC-FRAME]` / `[FONT-POST] unchanged=True` / `[BASELINE-POST] 34/34`**。
- 第 16 例在本轮整类中的直接证据（L4231–L4239）：`PRE` → `MANAGEMENT-CLOSED` → `NEGATIVE-FRAMES=3` → `PERSON-CLOSED` → `BUILD-POINTER-NEGATIVE` → `BUILD-NEGATIVE` → `BUILD-GATE`（`pointerStable=True`、`cameraStable=True`、`candidate=pointerDerived`、`buildOpen=True / ghost=True / surface=World / managementActive=False / buildCount=0`）→ `END` → `CLEANUP`。同段 `[SAVE-RESTORE] matches-before=True`、`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34`。
- **运行级 runId / skipped / run 时长：本层不可见 → 待主开发补**；不把 method 合计或日志跨度写成 run 时长。

### 4.2 19 Edit（当前源码未变，沿用主开发原始核）

- `99761145-ddc9-43d9-a3eb-db6706e59acf`：**19/19 Passed、0 skipped、1.1578404 s、scope-restored**。
- 当前源码 `ManagementProjectionTests.cs` **19 个 `[Test]`**、`70548B / 64c8c124…`；`VillageStage1.Tests.dll 160768B / d85583f8…` 晚于源码；均 `2026-10-26T10:49Z`，**在本轮 ESC 改动之前且未再变**，故该 19/19 对当前 Edit 源码仍有效。**本轮未重跑**（ESC 改动不经 Edit 路径）。

### 4.3 为什么 16 Play 是必要受影响集成

第 16 例注入 `HomeProductShell._escapePressPendingForTests`、走真实 `LateUpdate`、操作建造候选与 Surface，并在 `finally` 清除；只有同 fixture 串行整类才能证明它不污染前 15 例、前 15 例不污染它，同时验证 `SaveSlotGuard` / `ResetForTests` / 日程恢复链在 16 例中逐例收尾。单例 1/1 不含跨用例污染证据；19 Edit 不经 `LateUpdate` / 帧循环，不能替代。

---

## 5. 前阶段 264 / 1513 / 122 与当前受影响集成的差别

| 门禁 | 它实际测什么 | 与本次改动关系 | 结论 |
|---|---|---|---|
| Compiler **264** | 宿主 `.NET` 工具链（`dotnet build/test` + `rrpg-config validate`） | 未改工具链 / 配置 / manifest；Unity 侧编译有效已由当前 DLL 新鲜 + 16/16 + 19/19 实际执行证明 | **非必要回归**；历史回执真实但属前阶段 |
| 完整 Edit **1513** | 全工程 EditMode | 本次只改 `HomeProductShell.LateUpdate`（Play 帧路径）与 Play 测试；管理 Edit 面只有 `ManagementProjectionTests` 一个类（19/19） | **非必要**重跑全集；受影响 Edit 面已被 19/19 覆盖 |
| 完整 Play **122** | 全工程 PlayMode | 16 例包含管理面全部真实入口与第 16 例帧路径；其余 Play 类不经管理页 | **非必要**重跑全集；若审阅人要求更宽运行时证据，受影响类为 `ProductHomeFoundationPlayTests`、`VillageVisualPrototypePlayTests`、`BuildingConstructionProductPlayTests`、`BuildingResourceProductPlayTests`、`BuildingVisualReplacementPlayTests`、`R3AGrowthTransactionPlayTests`（原 122 子集，**可选非门槛**，本包未运行） |

> 历史回执 `5eb0ad4c`（122）、`f116b56e`（1513）、宿主普通用户 264 **仍为真实**，但都是**前阶段版本**；本交接既不说“当前全集已通过”，也不把全集重跑设为完成门槛。

---

## 6. ESC 当前机制证据与严格边界

### 6.1 已由真实帧证明的机制

| 机制 | 证据 | 边界 |
|---|---|---|
| **真实 `LateUpdate` 采样** | `HomeProductShell.LateUpdate` 在 `ShellVisible` 后调用 `SampleEscapePressed()`；第 16 例注入一帧后由真实帧关闭当前 Surface（L4232 / L4234） | 注入的是**合成一帧“按下”**，不是 OS 物理键 |
| **一次消费** | `SampleEscapePressed` 命中后立即置 pending=false；`NEGATIVE-FRAMES=3` 保持管理模态（L4233） | 测试环境无物理 ESC 是环境前置；同一注入不重放 |
| **阴性帧** | 无注入 3 帧不关闭、不重复进模态（L4233） | 不证明 OS 键盘穿透 / 按住重复 |
| **建造门禁** | `Build.IsOpen=True` 时 `HandleEscape` 让路：`buildOpen=True / definition=building_farm / ghost=True / surface=World / managementActive=False`；候选等于同帧真实指针换算（`candidate=pointerDerived`）、`buildCount=0`、Selection / Clock / Speed 不漂移（L4236–L4237） | 建造自身物理 ESC / 右键（`HomeBuildingPlacement.ReadInput`）未注入；本用例只证明 Shell 路由让路 |
| **Editor-only 作用域** | 字段 / `Set/ClearEscapePressForTests` / 分支全部在 `#if UNITY_EDITOR`；`SampleEscapePressed` private；public ESC 测试缝计数 0；Player 编译不含 | 未构建 Player，只有静态保证 |
| **finally 清理** | 用例 `finally` 调 `ClearEscapePressForTests()`；用例结束后断言 `_escapePressPendingForTests==false` 且宿主仍单例（源码 L6914–L6923） | 进程级 abort 不在保证内（`SaveSlotGuard` 同性质） |

**准确表述**：ESC 证据已从“仅 public 回调”升级为“**真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁**”。但**严格不称** OS 物理 ESC、物理设备、IME / DPI / 手感或 `Input.GetKeyDown` 引擎实现已验。

### 6.2 仍未覆盖（真实存在，不写成已验）

| 项 | 现状 | 性质 |
|---|---|---|
| OS 物理 ESC / 真实键盘设备 | 未覆盖；合成采样不等于物理键 | 需物理设备 / 人工 |
| `Input.GetKeyDown` 引擎内部实现 | 未证明；只证明产品层同一采样 / 路由路径 | 引擎层，不在本包范围 |
| 物理鼠标 / 滚轮 / 拖拽手感、DPI / 焦点 / IME / 多指针时序 | 未覆盖；程序化 `EventSystem` + 合成快照只到机制层 | 规格 §8 只要求“不穿透”，**未把“手感”列为验收项** → 不得新增为强制验收 |
| Player 构建内行为 / 缝不存在 | 仅 `#if UNITY_EDITOR` 静态保证；未构建 Player | **测试边界**，不是原规格新验收门槛；实证需独立构建授权 |
| `SaveSlotGuard` 进程级 abort | 只在用例收尾 / `OneTimeTearDown` 兜底 | 需单独授权 out-of-process 设计 |
| `CapacityFull` UI 展示文案像素 | 领域诊断与按钮禁用已断言；像素 / 文本未单测 | 低优先、普通可测 |

---

## 7. 环境恢复 / 34 保护 / 当前档恢复链（16 例运行后只读实测）

| 项 | 实测 | 判定 |
|---|---|---|
| 34 项磁盘基线 | `total=34 / problems=0`（逐项 bytes + sha256 对比 `workspace-baseline.json`） | 全一致 |
| 受保护 TMP 字体（磁盘 / 内存） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`、mtime `2026-10-10T13:23:55.9520319Z`；9 子资产 `dirty=0` | 未变；未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、mtime `2026-10-27T07:07:53.2514600Z`、`.tmp=False` | 内容为原始单槽；mtime 为收尾恢复回写 |
| guard 目录 | `293 = 292 restored + 1 protective-snapshot(20261024T052704Z)`；`noManifest=0 / pendingRestore=0 / unverified=0`；最新 restored `20261027T070752Z`（`readbackVerified=true`、`sourceSha256=4b2b56f6…d6aa`） | 16 例逐例捕获 / 恢复，无 unfinished |
| 场景 / Editor | `SampleScene dirty=False roots=5`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080` | 干净空闲 |
| Console | 产品 `Error/Exception` 0 | 无新增异常证据 |
| 测试缝残留 | `HomeWorldCamera` 实例 0、`HomeProductShell` 实例 0；registry 默认覆盖 null | 无实例载体残留；默认路径生效 |

**恢复链事实**：16 例日志中每组 `[SAVE-CAPTURE]` 均有同 runId（含 `-2`）的 `[SAVE-RESTORE] matches-before=True`；第 16 例 `finally` 清 ESC 注入；`OneTimeTearDown` 兜底与既有 `SaveSlotGuard` / `ResetForTests` / 日程恢复链未被短路。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**

---

## 8. 历史保留 / 取消 / Deferred（不洗白、不伪造）

| 项 | 现状 |
|---|---|
| **历史 650 存档** | `21075B / 650c45c4…`（2026-10-25 记录）；授权范围内无字节备份，**未恢复、未伪造、不用当前档冒充** |
| **两次历史边界违规** | 旧 `4fbf7754` 的 `EditorUtility.ClearDirty`；详情修复包的 `AssetDatabase.SaveAssets()` 越界调用；**原文留存，不回滚、不洗白**；本包未重复 |
| **1280×720** | 制作人 `01a53eb8` **取消**；取消 ≠ 通过；不再询问、不设门禁、不由 1920 外推 |
| **Deferred** | 动态批次 / 供餐细节；`CapacityFull` UI 文案像素；`SaveSlotGuard` 进程级 abort；Chevron（`44×28 / 字号 24`，非必要指示，未修未测）；Player 构建实证（需独立授权） |
| Prefab 历史自动回写 | `282097B→282326B`（无备份、根因未定）；保留记录，不宣称“仅数字变化” |
| Git | 分支 `ai/rrpg-r3b-population-pilot`，**无 Commit / Push**（需 Git 授权） |

---

## 9. 父 contract 三个 `requiredChecks` 的可审条件结论

> 不改 contract / `requiredChecks`、不直接改任务状态；以下只是证据结论。

| # | requiredCheck id | 类型 | 条件结论 | 具体依据 / 仍缺 |
|---|---|---|---|---|
| 1 | `execution` | `real` | **已覆盖（条件：16 类 run 级元数据待主开发补）** | 16 Play（宿主真实 16/16、`message=null`、method 14 634 ms）+ 19 Edit（`99761145…` 19/19、0 skipped、1.1578404 s）+ ESC 单例（`cc3adfc5…` 1/1、0 skipped、2.0916182 s）。缺：16 类 runId / skipped / run 时长 |
| 2 | `regression` | `real` | **已按“实际改动的必要回归”覆盖**（不等于最新全集重跑） | 受影响面：ESC 采样（16 例帧路径 + 单例）、管理 UI / 命令 / 双槽 / SaveLoad（16 Play + 19 Edit）、保护面（34 / 字体 / 单槽 / guard）。264 / 1513 / 122 未重跑且**不作门槛**（§5）；可选更宽子集未运行 |
| 3 | `design-audit` | `analysis` | **已覆盖（分析）+ 真实缺口列明** | §2 逐项对照；ESC 键盘路由已在产品层机制覆盖；**OS 物理键 / Input 引擎 / 物理手感 / IME / DPI / Player 面 / guard 进程级 / UI 文案像素仍未覆盖**，明确不冒称 |

**整体结论**：**不写 Completed、不写 awaiting_acceptance、Evidence 只能标 partial**。理由：`design-audit` 仍有真实未覆盖项（物理设备 / Player / 进程级等），且 16 类 run 级元数据待补；不为迎合完成谎称 PASS。R3-B 状态与父子 contract 终态均不变。

---

## 10. 交接清单（下一接手方按此顺序）

1. **主开发补 16 类 runId / skipped / run 级时长**（本层不可见；不把 method 合计 / 日志跨度写成 run 时长）。
2. **主开发专业核验**本交接与三份同步文档（contract evidence / 范围复核 / 交付索引）；核验通过前不升级状态。
3. **制作人产品验收**：按 1920 真实窗口手工走底部“管理”三页签、派工 / 撤职 / 施工 / 保存 / ESC / 滚轮拖拽；手感与物理输入按 §6.2 边界单独记录，**不由本包代替验收**。
4. **独立授权项**：Player 构建内实证、`SaveSlotGuard` out-of-process 兜底、动态批次、Git 提交 / 发布；均不在本包范围。
5. **不再派新实现 / 测试包**：本包无新增实现或测试需求；上述剩余项均为人工 / 独立授权 / 外部元数据，不属“换数值重跑”。

---

## 11. 本次实际写入与边界

| 文件 | 动作 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261027/current-sixteen-preflight.md` | 阶段 1 新增（只读预检 + 交宿主 16 例口径） |
| `Development/StudioRuns/R3B-UI-20261027/current-contract-evidence.md` | 阶段 2 顶部新增 2026-10-27 16 例当前口径（历史保留） |
| `Development/StudioRuns/R3B-UI-20261027/final-scope-and-contract-review.md` | 阶段 2 追加 16 例最终复核（历史保留） |
| `Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md` | 顶部当前附记 + 追加 §9（16 例） |
| `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md` | 追加 §13（16 例当前事实） |
| `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md` | 追加 §13（16 例自审） |
| 6 份项目索引（`Docs/DevelopmentSpecs/README.md`、`Docs/项目状态快照.md`、`Docs/项目日志.md`、`Docs/文档地图.md`、`Docs/开发验收交接.md`、`Docs/DevelopmentSpecs/2026-10-09_…Spec-V0.1.md`） | 各加 2026-10-27 当前附记（精准合并，不批量重写历史） |
| `Development/StudioRuns/R3B-UI-20261027/final-professional-handoff.md` | **本文件（新增）** |

- **未改**：任何 `.cs` / `.prefab` / `.scene` / `.asset` / `.meta` / 字体 / 存档 / manifest / 基线 / GameSpec / Authority / Prefs / contract；未 Commit / Push。
- **未执行**：`tests-run`（模型）、`AssetDatabase.Refresh`、`SaveAssets / SaveChanges / SaveDirtyScenes / ClearDirty / SetDirty`、`rql-mutate`、PrefabStage 编辑、字体测量（`GetPreferredValues` / `ForceMeshUpdate`）、Shell / Git / Preferences / 正式工程、启停 Editor、构建 Player。
- 只读手段：`rql-query`、`read` / `grep` / `glob`、只读 C#（哈希 / 反射 / 目录与 manifest 统计 / 日志读取）；只读 C# 未写任何资产。
- 不把机制证据写成物理设备证据；不把 method 合计 / 日志跨度写成 run 时长；不把历史回执写成当前版本；不为迎合完成谎称 PASS。
- 本文件与整包结论须由**主开发专业核验**，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

---

## 12. 写后只读复核

复核在其余同步文档写入后执行；未运行测试、未 Refresh、未保存任何资产 / 场景。

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / problems=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、`.tmp=False` | 未变 |
| guard | `293 = 292 restored + 1 protective`、`pendingRestore=0` | 无 unfinished |
| 场景 / Editor | `SampleScene dirty=False`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080` | 干净空闲 |
| 16 例回执 | `testResults` 16 条全 `Passed`；日志最后一条 `07:07:53.5415737Z`（L4374） | 未被本阶段改写 |

### 12.1 本轮写入文件最终指纹（只读实测，2026-10-27T07:12:13Z）

| 文件 | bytes | SHA256 |
|---|---|---|
| `Development/StudioRuns/R3B-UI-20261027/current-sixteen-preflight.md` | 22384 | `21005f6d5269cb16588efe164fe72a896f88f7c2c416bab130ff344a35a8274c` |
| `Development/StudioRuns/R3B-UI-20261027/current-contract-evidence.md` | 25559 | `7a3a18a4cdcdf68b25b16852255a00428352365daf669e1a52416ca455e96814` |
| `Development/StudioRuns/R3B-UI-20261027/final-scope-and-contract-review.md` | 25104 | `451ebf40b89e38e5ffa6bc6929db438bf0223114de3ba86f3326294c203522a1` |
| `Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md` | 36655 | `2a5227167d3ab6a3e6e2a3bf02f7618cbbb5246f975c36503c1d8cf1057e3c8f` |
| `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md` | 53386 | `b1e293a05e4e34126696887d5c54e4566fa7906fb33058da471e646364cc7d84` |
| `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md` | 62787 | `43ef22f09ad46a3c7925e9ffc90d6e6ce7f0e510bce75c32f5ad8adc6b14deb6` |
| `Docs/DevelopmentSpecs/README.md` | 28348 | `43c2c9705400495b48bde0f21103df8a4c3f38abfd979976446ce3c6201faee0` |
| `Docs/项目状态快照.md` | 64179 | `b257b36cd979792eb1fbf8e9b5d97f160a769219018e2a6a7ccf30e6f093e217` |
| `Docs/项目日志.md` | 249267 | `577edca6cc563cad737407737e0437bd93a7f8813a87c318fe27cef1fcc82dd3` |
| `Docs/文档地图.md` | 30339 | `ba36240e990b62e47c55062428a0bc750f872a23d28c9a046bc9bbc8c8c52994` |
| `Docs/开发验收交接.md` | 30368 | `2cb4ea3d7288b17e37116e3a6449f8d168eeb5e15b9b0024d4457fd88cb606b9` |
| `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md` | 51186 | `1b4157d37d2c86dda737d8ac78419d26534256da2feeb1e7b57f3f7084822329` |

> 本文件在追加本节之前为 **27657B / `6663e4182feb882b01cbecf9a60822c2b619a2821b680d7651ed7fdefca6dadf`**；追加后字节 / 哈希会变化，**最终以目录实际文件为准**。其余 12 份文档在本节写入后未再改动。
