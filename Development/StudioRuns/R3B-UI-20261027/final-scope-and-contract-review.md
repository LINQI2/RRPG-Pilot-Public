# R3-B 完成条件与交付范围复核（2026-10-27，阶段 2/2 · 双槽第 15 例集成后）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未改计费渠道、未创建额外付费子 Agent。
- 本文件性质：**只读核证 + 文档**。按父 contract 真实 id（`execution(real)` / `regression(real)` / `design-audit(analysis)`）复核完成条件与交付范围；**不新增完成门槛、不降低门槛、不自行完成、不写 `awaiting_acceptance`**。
- 版本口径：当前 **2026-10-27（双槽 15 例整类集成后）**；2026-10-26 与 2026-10-27 早段（14 例 / 13 例）为历史，只读引用、不改写。
- 核对时刻：**2026-10-27T02:42Z 前后**（本机 `10:42 +08:00`，China Standard Time）。
- 前置产物（本包阶段 1，已落盘）：`current-fifteen-preflight.md`（只读预检 + 交宿主口径）。

---

## 1. 真实集成回执（宿主执行，本层只读）

| 项 | 值 |
|---|---|
| 运行范围 | `testClass=RRPG.ManagementShell.Tests.ManagementShellPlayTests`，`testMode=PlayMode`，**15/15** 用例（整类，不是单例） |
| 结果 | **15/15 Passed**、逐条 `message=null`、无 Failed / 无 Skipped 可见 |
| 方法耗时 | 合计 **13 996 ms**（567–1554 ms/例） |
| 日志段 | `management-shell-play-evidence.log` **L3886–L4108**：`2026-10-27T02:38:32.1552485Z` → `02:38:46.5804623Z` |
| 结构证据 | **15 组 `[ENTRY]`**、**15 组 `[SAVE-RESTORE] matches-before=True`**、**4 组 `[SCHEDULE-RESTORE] restored=true sameInstance=True`**、`[FONT-POST] unchanged=True`、`[BASELINE-POST] total=34 same=34 changed=0 problems=0` |
| 运行级缺口 | **15 类 runId / skipped / run 级时长本层不可见（`testResults` 字段仅 name/status/durationMs/message/stackTrace）→ 待主开发补**；日志跨度与 method 合计**都不是** run 时长 |
| 上游已核、本轮未重跑 | `33b8271a…` 双槽单例（1/1、0 skipped、2.6883936 s、scope-restored）、`46d65625` 旧 14/14、`99761145…` 管理 Edit 19/19（0 skipped、1.1578404 s）、`df6c6645…` 输入单例（1/1、0 skipped、2.6441609 s） |

**为什么必须整类而不是只跑第 15 例**：第 15 例改写同一 fixture 的运行环境——static 内存 `BuildingRegistry` 覆盖、`SceneManager.LoadScene("SampleScene")` + 真实 `btnContinue.onClick` 重建 host、活动日程隔离、以及最重的真实存档写路径。只有**同 fixture 串行整类**才能证明「前序用例不污染它、它不污染后继用例」（本轮 `CASE-E START override=<null>` + `END override=<null> defaultRegistryRestored=True` + 4 组日程恢复即该证明）。

---

## 2. 完成条件复核（按父 contract 原 id / 原类型）

### 2.1 `execution(real)` — **已覆盖**

| 执行 | 回执 |
|---|---|
| 管理 Play 整类（当前 15 例） | **15/15 Passed、`message=null`**（runId / skipped 待补） |
| 管理 Edit 整类（当前 19 例） | **19/19、0 skipped、1.1578404 s、scope-restored**（`99761145…`） |
| 双槽单例 | **1/1、0 skipped、2.6883936 s、scope-restored**（`33b8271a…`） |
| 输入门禁单例 | **1/1、0 skipped、2.6441609 s、scope-restored**（`df6c6645…`） |

结论：**有当前版本的真实宿主执行回执**，不是推理、不是历史外推。唯一缺项是 run 级元数据（runId / skipped / run 时长），**如实标“待主开发补”，不编造**。

### 2.2 `regression(real)` — **按“实际改动的必要回归”已覆盖**（不设“全集最新重跑”门槛）

判定方法：**先枚举改动 → 再枚举受影响面 → 再看每个受影响面是否有当前版本回执**，而不是“把 264/1513/122 再跑一遍换数字”。

| 改动 | 受影响面 | 当前回执 | 充分性 |
|---|---|---|---|
| 测试源码新增第 15 例（`446994B / 05d6de11…`） | 同一 fixture 全部 15 例的共享运行环境 | 15/15 | **必要且充分**：跨用例污染只能整类证明 |
| `HomeWorldCamera` 采样重构（`5077B / 39caba4b…`） | `ProductHomeWorldHost.Update` 每帧 `ReadInput` → 所有进入真实 Home 世界的 Play 用例 | 管理 Play 15/15；生产路径（`_inputSampleForTests==null`）回落**逐句等同**的 legacy Input 采样 | **必要**：管理面每帧都经该路径；**等价性可论证**：仅取数重构 + Editor-only 分支，门禁与 `Pan/Zoom` 未变 |
| `ManagementView.cs` + `ManagementView.prefab` 行明细 / 行高（`39037B / f8316e43…`、`282332B / 31bf3e8c…`） | 仅管理三子页渲染 | 布局例、滚动例、双槽两行例 + Edit 投影 19/19 | **必要且充分** |
| `StaticConfigRegistry` Editor-only 测试缝（`29248B / 1c0c62fa…`） | 仅显式调用者（Edit 第 19 例 / Play 第 15 例） | 默认无覆盖（本轮实测 null、默认 `FunctionBindings` 3 条全 `main`、**无 aux**）+ 两处 finally 恢复 | **充分** |
| 保护面 | 字体 / 34 项 / 单槽 / guard / 场景 | 全部一致（§5） | **充分** |

**为什么不把 264 / 1513 / 122 重跑列为完成门槛（明确不臆造）**：

- **264** 是宿主 `.NET` 工具链门禁（`dotnet build/test` + `rrpg-config validate`），**不是 Unity 产品脚本单测**；本包未改工具链 / 配置 / manifest。Unity 侧“当前源码能编译”已由**本轮真实运行**证明（`BattleStage1.PlayModeTests.dll` 晚于测试源码 3.86 s、`Assembly-CSharp.dll` 晚于相机源码 59.20 s，且 15/15 + 19/19 实际执行成功）。
- **1513 Edit**：管理相关 Edit 类只有 `ManagementProjectionTests` 一个（已 19/19）；本次改动不触及 Person / Growth / Social / History / Battle / Encounter 等领域。
- **122 Play**：管理面由本 fixture 全覆盖；其余 Play 类不经管理页。
- 三者的历史回执**仍为真实**（`5eb0ad4c` / `f116b56e` / 宿主普通用户 264），但**属前阶段版本**；本文件既不写成“当前版本通过”，也**不设成完成门槛**。

**若审阅人要求更宽的运行时证据（可选加强，非门槛）**：受影响 Play 类为 `ProductHomeFoundationPlayTests`、`VillageVisualPrototypePlayTests`、`BuildingConstructionProductPlayTests`、`BuildingResourceProductPlayTests`、`BuildingVisualReplacementPlayTests`、`R3AGrowthTransactionPlayTests`（均真实帧进入 `ProductHomeWorldHost` → 每帧经 `ReadInput`）。这是**原 122 全集的子集**、**不改断言不新增用例**，成本一次串行子集运行；本包**未运行**（约束：模型不 `tests-run`；且不为换数值重跑全集）。

### 2.3 `design-audit(analysis)` — **已覆盖（分析）+ 真实缺口列明**

见 §3、§4。核心变化：**四项**旧的“未覆盖”已由第 15 例 / 第 19 例关闭（双槽真实 Gateway、双槽 UI/SaveLoad、`CapacityFull` 领域诊断、建造态 ESC 公开回调）；剩下的是**性质不同**的缺口（物理 / 键盘轮询 / Player 面 / UI 文案像素 / guard 进程级），不能用“分析”冒充“实测”。

---

## 3. 交付范围（逐项状态，不冒称）

### 3.1 已覆盖（当前版本真实证据）

| 项 | 证据 |
|---|---|
| 双槽真实 Gateway：main(2) + aux(1) 独立容量 / 绑定 / 劳动 / 现场，交叉为 0 | 第 15 例 `SLOTS` / `LABOR`（`cross=0`、`40/40/40`） |
| 双路 `CapacityFull` 领域诊断 + 零写 + 原 UI 满员禁用 | 第 15 例 `FULL` + 源码断言（`DomainRejected` / `Diagnostic=="CapacityFull"` / 无写入 / 按钮 `interactable=false`） |
| 双槽真实 UI 派工（真实行点击 + 真实按钮） | 第 15 例 `ASSIGN`（main/aux 互不影响） |
| 双槽显式 Save + 真实 Continue 换源后逐项等值（身份 / 绑定人 / 容量 / 资源 / Clock / 批次 / 证据 / 完成计数，无重复结算） | 第 15 例 `SAVE` / `CONTINUE`（`clock=16360 ingredients=14 food=0 batches=1 evidence=3 completed=3`） |
| 建造态 ESC **公开回调顺序**（`Build.IsOpen` 时 `HandleEscape` 不关建造、不进 CloseSurface；`Build.Cancel` 后才关当前 Surface） | 第 15 例 `ESC-BUILD` / `ESC-CLOSE` |
| 模态阻断镜头 / Selection / Clock / 建造候选；关闭后可逆 | 输入门禁例（`POSITIVE` / `MODAL` / `PERSON` / `AFTER-CLOSE` / `CLEANUP`） |
| 施工 / 撤职 / 事件 pending / Return gating / 三表面等价 / Save-Continue-NewGame | 其余 14 例（15/15 内） |
| 管理命令 / 投影 / 双槽 Gateway（EditMode） | Edit 19/19（第 19 例 `ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`） |

### 3.2 **真实未覆盖**（存在，不能写成已验）

| 项 | 规格依据 | 为什么仍未覆盖 | 边界 |
|---|---|---|---|
| **真实键盘轮询**：`HomeProductShell.cs:95 Input.GetKeyDown(KeyCode.Escape) → HandleEscape()`（另 `HomeBuildingPlacement.cs:102` ESC/右键取消、`CharacterGrowthUI.cs:125` ESC） | R3-P §8 含“点击 / 拖拽 / 滚轮 / **键盘**不穿透”；§5 还明确“键盘 / 镜头 / 旧模态组合**仍需 Play 实证**” | 现有全部 ESC 证据都走 **public `HandleEscape` 回调**，**绕过** `Input.GetKeyDown` 这一行；legacy Input 的键盘事件无法在测试中被程序化合成；产品**无键盘镜头控制** | **不得说“键盘已测”**；也不把单行轮询风险夸大为产品缺陷 |
| 物理鼠标 / 滚轮 / 拖拽、OS 焦点 / DPI / IME / 多指针时序 | §8 要求“不穿透”（**未提“手感”**） | 程序化 `EventSystem` + 合成 legacy Input 只到**机制层** | **“手感”不是原规格验收项**，不得新增强制验收；物理输入实测属制作人人工体验 |
| Player 构建内行为 | 采样缝（`HomeWorldCamera`）与 registry 缝（`StaticConfigRegistry`）全部 `#if UNITY_EDITOR` | 未构建 Player；双槽用例依赖 Editor-only 缝，Player 内不存在 | 仅静态保证；如需实证须独立 Player 构建授权与包 |
| `CapacityFull` **UI 展示文案像素** | 规格只要求给既有诊断 | 按钮禁用已断言，文案文本未单测 | 低优先、普通可测 |
| `SaveSlotGuard` 进程级 abort | 现有 guard 只在用例收尾 / `OneTimeTearDown` 兜底 | 进程级崩溃不可覆盖 | 需单独授权设计 out-of-process 兜底 |
| 1280×720 | §8 矩阵原含 1280 | 制作人 `01a53eb8` **取消** | **取消 ≠ 通过**；已决定、不再询问、不由 1920 外推 |
| 动态批次（资源批次 / 供餐细节） | — | Deferred | 不虚构产出 |
| 历史 650 存档（`21075B / 650c45c4…`） | — | 授权内无字节备份 | 保留缺口，不恢复、不用当前档冒充 |

### 3.3 历史项（原文保留，不洗白）

- 两次边界违规：旧 `4fbf7754` 的 `EditorUtility.ClearDirty`、详情修复包的 `AssetDatabase.SaveAssets()` 越界（原文见 `R3B-UI-20261026/workplace-detail-readability-review.md` §6.3）。
- Prefab 历史自动回写（`282097B→282326B`，无备份、根因未定）。
- 旧口径：管理 Play 14/14（`46d65625`）、13/13、12/12、11/11、10/10；管理 Edit 18/18、17/17；完整 Play 122 / Edit 1513 / Compiler 264 均为**前阶段版本**；日志中 CASE-A 段的 `CapacityFull 域名诊断未经按钮进入` **只对 CASE-A 成立**（整类运行时它会再次打印，不得据此判仍未测）。

---

## 4. 物理 / OS 面与本包结论的边界（严格分栏）

| 结论 | 已被证明 | **未**被证明 |
|---|---|---|
| 镜头拖拽 / 滚轮 | 合成 legacy Input 快照经**真实 `Host.Update` → 同一 `ReadInput`** 产生非零 `Pan` / `Zoom`；模态内不变；关闭后可逆 | 物理设备、灵敏度、手感、多指针 |
| 模态阻断 | 真实 `overUI` 门禁（`Shell.IsPersonModalOpen || PointerOverUI`）阻断世界点击 / 镜头 / 建造 / 旧 Town | OS 级输入穿透、焦点切换、DPI、IME 组合输入 |
| ESC | `HandleEscape` **public 回调**只关顶层、`Build.IsOpen` 时不清候选、恢复旧 Town `CanvasGroup` | **物理 ESC 键**、`Input.GetKeyDown` 轮询、按住重复 |
| 双槽 UI / SaveLoad | 真实 `Button.onClick` 与真实 `SaveSystem` 读写换源 | 物理鼠标点击手感 |
| Player 面 | 源码 `#if UNITY_EDITOR` 静态排除 | 实际 Player 构建运行结果 |

---

## 5. 保护面（15 例运行后只读实测）

| 项 | 实测 | 判定 |
|---|---|---|
| 34 项基线 | `34/34`、`problems=0` | 全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、`.tmp=False` | 内容未变 |
| guard | `275 = 274 restored + 1 protective-snapshot`；最新 `20261027T023845Z`；全部 `readbackVerified=true`；`noManifest=0 / other=0` | 无 unfinished |
| 场景 / Editor | `SampleScene dirty=False`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080` | 干净空闲 |
| Console | 产品 `Error/Exception` **0** | 无新增异常证据 |

---

## 6. 完成条件判定与父 contract 状态

| 判定项 | 结论 |
|---|---|
| 是否可标 `Completed` | **否**。`design-audit` 仍有**真实未覆盖项**（§3.2），且 15 类 run 级元数据待补。 |
| 是否可标 `awaiting_acceptance` | **否**。本包不代制作人验收，也不请求验收。 |
| 交付 Evidence 层级 | **partial**（因 §3.2 的真实未覆盖项存在；不因本轮升级为完整）。 |
| 父 / 子状态 | 保持 **running 非终态**，不自动完成。 |
| R3-B 状态 | **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**（不变）。 |

### 6.1 仍然“卡完成”的**只有**哪些要求（其余已不卡）

1. **规格 §8「键盘不穿透」的真实按键路径**（现仅有 `HandleEscape` 回调证据）——**唯一属“规格内、技术可补、当前未测”的一项**。
2. 物理鼠标 / 焦点 / IME / 手感与 Player 构建内行为——**属人工体验或独立构建授权**，**不是**“普通技术问题抛制作人”：前者由制作人按 §3.2 边界人工体验记录，后者需独立 Player 构建授权包。

### 6.2 若要把第 1 项补齐：**可自主补的最小技术方案**（需另行授权，本包未做）

- **方案 A（最小代码改动，推荐）**：在 `HomeProductShell.Update` 的 ESC 轮询处加一个 `#if UNITY_EDITOR` 的实例级 `internal` 可注入布尔（与 `HomeWorldCamera._inputSampleForTests` 完全同风格：`private` 字段 + `internal` 注入/清除方法 + `finally` 清理），`Update` 内改为 `if (Input.GetKeyDown(KeyCode.Escape) || ConsumeEscapeForTests()) HandleEscape();`。Player 编译中字段 / 方法 / 分支**不存在**，生产语义不变，不新增 public API。
- **方案 B（配套用例，1 个）**：注入“ESC 按下一次”→ 断言 `HandleEscape` 被调用一次、管理模态只关顶层、`Build.IsOpen` 时不清候选 / 不进 CloseSurface、关闭后不再重复触发。
- **方案 C（不改代码的替代）**：由宿主普通用户在 Editor / Player 窗口做一次 OS 级按键注入并留日志；属宿主 / 人工动作，不计入自动化回归。
- 成本量级：方案 A ≈ 1 处 seam（数行）+ 方案 B ≈ 1 个 Play 用例；**不触碰 Authority / SaveVersion / DTO / 配置**。**需要主开发明确授权“代码修改 + 相应回归”后才可执行**；本包只给方案，不实施。

---

## 7. 本轮实际写入（仅文档；未改代码 / 资产）

| 文件 | 动作 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261027/current-fifteen-preflight.md` | 阶段 1 新增（只读预检 + 交宿主 15 例口径） |
| `Development/StudioRuns/R3B-UI-20261027/current-contract-evidence.md` | 阶段 2 重写为当前（15 例）证据与缺口，历史口径标历史 |
| `Development/StudioRuns/R3B-UI-20261027/final-scope-and-contract-review.md` | **本文件（新增）** |
| `Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md` | 顶部当前补充 + 新增 **§8**（当前门禁 / 被取代表述逐条对照 / 仍未覆盖） |
| `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md` | 新增 **§12**（双槽第 15 例集成后当前事实） |
| `Docs/项目状态快照.md` | 顶部新增 **2026-10-27 当前条目** + 更新“最后核对”行 |

**未改**：任何 `.cs` / `.prefab` / `.scene` / `.asset` / `.meta` / 字体 / 存档 / manifest / 基线 / GameSpec / Authority / Prefs；未 Commit / Push。

---

## 8. 写后只读复核

复核在全部文档写入后执行（2026-10-27T02:42:5xZ 前后）；未运行测试、未 Refresh、未保存任何资产 / 场景。

### 8.1 保护面与现场

| 项 | 实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / problems=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、`.tmp=False` | 未变 |
| guard | `275 = 274 restored + 1 protective-snapshot`、`noManifest=0` | 仍无 unfinished |
| 场景 / Editor | `SampleScene dirty=False`（`dirtyScenes=0`）、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false` | 干净空闲 |
| 源码 / DLL | `ManagementShellPlayTests.cs 446994 / 05d6de11…`、`BattleStage1.PlayModeTests.dll 547840 / b8dae733…`、`Assembly-CSharp.dll 649216 / f80f8c80…` | 与运行段一致，未改 |

### 8.2 本轮文档字节 / SHA256（实测）

| 文件 | bytes | SHA256 | mtimeUtc |
|---|---|---|---|
| `R3B-UI-20261027/current-fifteen-preflight.md` | 26147 | `b431c36c057323bcbb8fc6af34ace53b149e46bd840bbc2573d58effebb1fe43` | `2026-10-27T02:38:02.7202685Z` |
| `R3B-UI-20261027/current-contract-evidence.md` | 22539 | `e3f7b75fa24757a12f61fb8464f3754b59b31c0e61dcefe87ecdbbf593910d37` | `2026-10-27T02:40:45.5637623Z` |
| `R3B-UI-20261026/reviewable-delivery-index.md` | 31643 | `1fcd2dd2ee54fcb14836281e0dfa48250c49326e4b197dbac0ccb2e6c610150a` | `2026-10-27T02:42:43.8078073Z` |
| `Docs/DevelopmentSpecs/2026-10-24_…Execution-Record.md` | 46978 | `cbbbc6a9c9f382cfbf5a8a5de80b01de37bfd6492d96b0eedda59fcfee5b59da` | `2026-10-27T02:41:39.7315967Z` |
| `Docs/项目状态快照.md` | 61717 | `e81c4e5c32dd21e437815d9f05d36e5f7117a7cbe1f599abf06e7fb06418019c` | `2026-10-27T02:41:51.2103063Z` |
| **本文件（追加本节之前）** | **18892** | `1a7430cd9f9fcd899938f5fe9d1c3ab092e44899563162bff6dd8f7c53ddf70a` | `2026-10-27T02:42:22.8320746Z` |

> 追加本节后本文件字节 / 哈希会变化，**最终以目录实际文件为准**；其余五份文档本轮写后未再改动。

---

## 9. 边界与诚信声明

- **模型未运行 `tests-run`**；15 例整类由宿主 Studio 阶段 check 执行，本层只读 `testResults` / 证据日志 / guard / 保护面。
- 未 Refresh、未 `rql-mutate`、未 PrefabStage、未字体测量（`GetPreferredValues` / `ForceMeshUpdate`）、未 `SaveAssets / SaveChanges / SaveDirtyScenes / ClearDirty / SetDirty`、未 Shell / Git 写、未写 Preferences、未启停 Editor、未构建 Player、未访问正式工程。
- 未重跑 264 / 1513 / 122，且明确**不把它们设成本次完成门槛**；若审阅人需要更宽运行时证据，具体受影响类已在 §2.2 列出（可选、非门槛）。
- 不把机制证据写成物理设备证据；不把回调写成物理按键；不把 method 合计 / 日志跨度写成 run 时长；不把历史回执写成当前版本回归。
- 历史违规与 650 缺口原文保留，不洗白（§3.3）。
- **完成条件结论**：`execution(real)` 已覆盖；`regression(real)` 按实际改动必要回归已覆盖（全集未重跑，非门槛）；`design-audit(analysis)` 已覆盖但 §3.2 缺口真实存在 → **整体保持非 Completed / 非 awaiting_acceptance / Evidence=partial**，父 contract 保持 running 非终态。
- 本文件须由**主开发专业核验**，**不代表制作人批准**。

---

## 10. 2026-10-27 追加：ESC 采样修后 16 例最终复核（当前口径）

> 本节**只追加**，不重写 §1–§9 的 15 例当时记录；冲突时以本节为准。本阶段只读 + 文档，模型未 `tests-run`；16 例由宿主 Studio 阶段 check 执行。

### 10.1 真实最终门禁

| 项 | 当前值 |
|---|---|
| 管理 Play 整类 | **16/16 Passed、逐条 `message=null`**；method 合计 **14 634 ms**（555–1446 ms/例）；日志 **L4139–L4374**（`07:07:38.4677317Z–07:07:53.5415737Z`） |
| 日志结构 | 16 组 `[ENTRY]` / 16 组 `[SAVE-CAPTURE]` / **16 组 `[SAVE-RESTORE] matches-before=True`** / 4 组 `[SCHEDULE-RESTORE] sameInstance=True` / 第 16 例 9 行 `[ESC-FRAME]` / `[FONT-POST] unchanged=True` / `[BASELINE-POST] 34/34` |
| 第 16 例 | `PRE / MANAGEMENT-CLOSED / NEGATIVE-FRAMES=3 / PERSON-CLOSED / BUILD-POINTER-NEGATIVE / BUILD-NEGATIVE / BUILD-GATE / END / CLEANUP`（L4231–L4239）；`BUILD-GATE` 中 `pointerStable=True`、`candidate=pointerDerived`、`buildOpen=True / ghost=True / buildCount=0` |
| ESC 单例（上游） | `cc3adfc5…`：**1/1、0 skipped、2.0916182 s、scope-restored** |
| 管理 Edit | `99761145…`：**19/19、0 skipped、1.1578404 s、scope-restored**；Edit 源码 19 个 `[Test]` / `70548B / 64c8c124…` 未变 |
| 运行级元数据 | **16 类 runId / skipped / run 时长本层不可见 → 待主开发补**（不把 method 合计 / 日志跨度写成 run 时长） |

### 10.2 完成条件（按父 contract 原 id 复核，结论与 §2 相同、证据更新）

- `execution(real)`：**已覆盖**——16 Play + 19 Edit + ESC 单例均为真实宿主执行；唯一缺项是 16 类 run 级元数据。
- `regression(real)`：**按实际改动的必要回归已覆盖**——本轮产品改动仅 `HomeProductShell.LateUpdate` ESC 采样（Editor-only internal seam），受影响面为管理 Play 帧路径 / UI / 命令 / 双槽 / SaveLoad，已由 16 Play + 19 Edit + 保护面覆盖；264 / 1513 / 122 未重跑且**不作门槛**。
- `design-audit(analysis)`：**已覆盖（分析）+ 真实缺口列明**——ESC 已由第 16 例补到**真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁**；**OS 物理键 / `Input.GetKeyDown` 引擎实现 / 物理手感 / IME / DPI / Player 面 / guard 进程级 / UI 文案像素仍未覆盖**。

### 10.3 对 §6 / §6.1 的取代（旧“唯一卡完成项”已不成立）

- §6.1 第 1 项“规格 §8 键盘不穿透的真实按键路径未测”**已被第 16 例关闭到产品层**：`LateUpdate → SampleEscapePressed → HandleEscape` 是真实帧路径，注入只替换“一帧按下”的采样值，且一次消费 / 阴性帧 / `Build.IsOpen` 让路均有断言。
- 仍**不**能说“键盘已完全验”：OS 物理 ESC、按住重复、IME / 焦点、`Input.GetKeyDown` 引擎实现仍未覆盖；这些是**物理设备 / 引擎边界**，不是原规格新增验收门槛。
- §6.2 的“最小技术方案（未做）”**已实施**（实例级 `internal` 一次性注入 + 1 个 Play 用例），不再列为待授权方案；实施与边界见 `escape-frame-routing-review.md`、`current-sixteen-preflight.md`、`final-professional-handoff.md` §6。

### 10.4 最终判定（条件结论）

| 判定项 | 结论 |
|---|---|
| 是否可标 `Completed` | **否**：`design-audit` 仍有真实未覆盖项（物理 / 引擎 / Player / 进程级），且 16 类 run 级元数据待补 |
| 是否可标 `awaiting_acceptance` | **否**：本包不代制作人验收、不请求验收 |
| Evidence 层级 | **partial** |
| 父 / 子状态 | 保持 **running 非终态**，不自动完成 |
| R3-B 状态 | **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**（不变） |

### 10.5 保护面 / 现场（16 例运行后只读实测）

34 项 `34/34`；字体 `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0`；单槽 `13405B / 4b2b56f6…d6aa`、`.tmp=False`；guard **293 = 292 restored + 1 protective-snapshot**（最新 `20261027T070752Z`，全部 `readbackVerified=true`，无 unfinished / noManifest / pendingRestore）；`SampleScene dirty=False roots=5`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080`、Console `Error/Exception` 0。

### 10.6 本轮写入与边界

只新增 / 更新文档：`current-sixteen-preflight.md`（阶段 1）、`current-contract-evidence.md`（§0 当前口径）、本文件（§10）、`reviewable-delivery-index.md`（§9）、执行记录 §13、自审 §13、6 份项目索引 2026-10-27 附记、`final-professional-handoff.md`。**未改**任何代码 / 测试 / Prefab / Scene / 字体 / 存档 / manifest / 基线 / GameSpec / Authority / contract；未 Refresh、未 `rql-mutate`、未 PrefabStage、未字体测量、未全局 Save / SetDirty / ClearDirty、未 Shell / Git / Prefs / 正式工程、未构建 Player；**模型未 `tests-run`**。本文件与整包结论须由**主开发专业核验**，不代表制作人批准。
