# R3-B 全部必要明细修复后 · 最终交付审阅（2026-10-26，阶段 2/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本阶段性质：**只读核对真实门禁回执 + 文档同步**。**未改任何代码 / 测试 / 断言 / Prefab / 字体 / 图集 / Scene / 配置 / 存档 / guard manifest / 34 项基线 / 备份**；**未** `AssetDatabase.Refresh`、**未** `PrefabStage`、**未** `rql-mutate`、**未** `tests-run`（模型不 tests-run，13 例由 Studio 宿主执行）、**未** `SaveAssets / SaveChanges / SaveDirtyScenes / SetDirty / ClearDirty`、**未** `GetPreferredValues` / `ForceMeshUpdate`、**未** Shell / Git 写、**未**写 Preferences、**未**启停 Editor。
- 状态：**R3-B = Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**。**不 Completed / 不 Accepted / 不 Published**；仍待主开发专业核验与制作人产品验收。

---

## 1. 本轮真实保护与恢复（13 例整类回执，逐例）

### 1.1 门禁回执

| 项 | 实测 | 口径 |
|---|---|---|
| `testResults` | **13 条全部 `Passed`、`message=null`**（方法耗时 547–1521 ms，合计 **11 754 ms**） | 本机只读核对 |
| 主证据日志运行段 | `2026-10-26T10:29:23.276Z–10:29:35.467Z`（≈**12.19 s**，日志时间差），主证据日志追加第 **3453–3642 行** | 本机只读核对 |
| 宿主 runId / run 级总时长 / skipped | **本层读不到，标“待主开发补”**，不编造 | 转述缺口 |
| 前一轮滚动专项（全部明细补齐后单方法） | 主开发已核 **`e75c1ec5-d51c-4239-88f0-4ec5dc1ee4ae`：1/1 Passed、0 skipped、`3.1183267` s、`scope-restored`** | 单例，不并入整类 |

### 1.2 逐例 `[SAVE-RESTORE]`（13/13，全部 `matches-before=True`）

| # | 日志行 | 时间（UTC） | runId | sha256 |
|---|---|---|---|---|
| 1 | 3458 | 10:29:24.427 | `20261026T102923Z` | `4b2b56f6…d6aa` |
| 2 | 3473 | 10:29:25.477 | `20261026T102924Z` | 同 |
| 3 | 3492 | 10:29:26.999 | `20261026T102925Z` | 同 |
| 4 | 3504 | 10:29:27.654 | `20261026T102927Z` | 同 |
| 5 | 3508 | 10:29:28.268 | `20261026T102927Z-2` | 同 |
| 6 | 3540 | 10:29:28.999 | `20261026T102928Z` | 同 |
| 7 | 3545 | 10:29:29.868 | `20261026T102929Z` | 同 |
| 8 | 3549 | 10:29:30.420 | `20261026T102929Z-2` | 同 |
| 9 | 3568 | 10:29:31.551 | `20261026T102930Z` | 同 |
| 10 | 3582 | 10:29:32.398 | `20261026T102931Z` | 同 |
| 11 | 3598 | 10:29:32.982 | `20261026T102932Z` | 同 |
| 12 | 3624 | 10:29:34.392 | `20261026T102933Z` | 同 |
| 13 | 3640 | 10:29:35.176 | `20261026T102934Z` | 同 |

每组均为 `existed=True` + `sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa` + `matches-before=True`，并附 `original-save.bin` / `manifest.json` 路径。

### 1.3 日程恢复 / 字体 / 基线

- `[SCHEDULE-RESTORE]` **3 组**：CASE-B 施工计量（3541）、真实滚动夹具（3569）、统一事件 pending 门禁（3599），均 `restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}`。
- `[FONT-PRE]`（3454）与 `[FONT-POST]`（3641）：`sha256=3c758e16…c404854`、`unchanged=True`。
- `[BASELINE-PRE]`（3453）`total=34 diff=0 all-match`；`[BASELINE-POST]`（3642）`total=34 same=34 changed=0 problems=0 all-match`。
- 日志末尾即 `[BASELINE-POST]`，**无 interrupted / unfinished / aborted**。

### 1.4 运行后只读复核（2026-10-26T10:30:31Z）

| 项 | 实测 | 判定 |
|---|---|---|
| Editor / 场景 / PrefabStage / TestRunner | 空闲（`playing/compiling/updating=False`）、`SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False` | 干净 |
| Screen | `1920x1080` | 未被用例改动 |
| 字体 | `14961033B / 3c758e16…c404854`，mtime 仍 `2026-10-10T13:23:55Z`；内存 9 子资产 **0 dirty** | 未写 |
| 34 项基线 | `34/34`、`diff=0`、`missing=0` | 未变 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`，mtime `10:29:35.1763281Z`，无 `.tmp` | 第 13 例收尾回读 |
| guard | **244** 目录 = **243 restored + 1 protective-snapshot**；无 `unfinished` / `noManifest`；最新 `20261026T102934Z restored / readbackVerified=true` | 本轮回执新增 13 个 run，全部终态合法 |
| Console | `Error` / `Exception` **0 条** | 无项目 / 测试程序集错误 |

### 1.5 新截图（本轮真实产出，历史图未覆盖）

| 路径 | bytes | SHA256 |
|---|---|---|
| `R3B-UI-20261026/play-evidence/management-real-scroll-workplaces-overflow-last-row-20261026T102931Z.png` | 310452 | `f4c0069d6cf0a28c8d4e29a42da834c9f99b5aa2d3b6c75db844c05c46d28732` |
| `…/management-real-scroll-resources-real-rows-20261026T102931Z.png` | 313330 | `d375bf3b0b97bd30a6d5463e2184f851a9ccdd3b875751627660d13198955efc` |
| `…/management-real-scroll-population-detail-facts-20261026T102931Z.png` | 274780 | `14c446da493e05dbaf372d0e6173d3fce15f272847ed19e5517c01c26d9a6049` |
| `R3B-UI-20261025/layout-input-evidence/management-population-1920x1080-20261026T102929Z.png` | 229965 | `d48cd7c35b3ec7e5c9c9cc7218b701f604bff71df5253c7187969fa411a51189` |
| `…/management-workplaces-1920x1080-20261026T102929Z.png` | 183167 | `d8c13a9e88c4583d7b55fac544db16c27f98472220a8b157ed49a6cf660e4594` |
| `…/management-resources-1920x1080-20261026T102929Z.png` | 176055 | `726645ca02d50d51434492eebdc528af6a24dc8db49bea04e4de711a378b6b27` |

均为 `1920x1080`。历史图 `T073954Z / T074624Z / T084927Z / T085326Z / T102527Z / T102528Z` 原样在位。

### 1.6 四类业务行真实渲染（第 13 例，`real-scroll-evidence.log` 第 89–126 行）

| 类 | 行数 | 关键实测 |
|---|---|---|
| 岗位 | 6 | detail `lines=3`、`preferredH=73.85`、`rect 高 76`、`checkedChars=51`；`绑定 0/2 · 实际劳动 0 · 现场 0` / `岗位 workplace:building_instance_N:main` / `建筑 … 槽位 main · 功能 ws_farm_01 · 绑定人：无` |
| 施工 | 1 | `lines=3`、**`checkedChars=58`**；`建筑 building_instance_7 · 进度 0 / 10 · 施工中` / `工人 1 人：林恩（person_96ea…）` / `现场 0 人` |
| 资源 | 6 | 标题 `Farm · 农田 · building_instance_N`（`chars=31 visible=27`）；detail `lines=3`、`checkedChars=41`；`名额 绑定 0/2 · 实际劳动 0` / `建筑 … 槽位 main` / 定位说明 |
| 人口 | 6 | 标题 `姓名 · 职业`（`chars=22 visible=20`）；detail `lines=3`、`checkedChars=40 / 37`；`在村 · 职责：…` / `执行：…` / `临时承诺：无 · 饥饿：Hungry（连续不足 10 天）` |

滚动真实性：职责与场所 `contentHeight=1044 / viewport 650 / overflowPx=394`；资源 `contentHeight=834 / overflowPx=184`；真实 `IScrollHandler`（`1→0.9555838` / `1→0.9048913`）+ 多轮拖拽（`steps=7 → norm 0 / contentY 394`；`steps=3 → 184`），末行滚动后 `visibleAfter=True` 且 Raycast 命中同一行。覆盖行 `[REAL-SCROLL-COVERAGE] Workplace=True Construction=True Resource=True Population=True`，`Resource=NotCoveredByThisCase` 本轮 **0 次**。

---

## 2. 原规格映射（R3-P Spec §4–§8；不新增规则）

状态口径：**当前已验证** = 有本轮版本真实回执；**已实现** = 代码 / UI 在位但只覆盖部分面；**未覆盖** = 无当前证据；**取消** = 制作人明确取消。

### 2.1 §4 集中管理

| 原规格要求 | 状态 | 当前依据 |
|---|---|---|
| 人口页：同 Roster 姓名 / 职业 / 在村 / 主要职责 / 现时执行 / 临时承诺 / 饥饿可比较 | **已实现 + 当前已验证** | `ROW-TITLE-RENDER`×6 + 6 条 `[REAL-SCROLL-DETAIL]`（`checkedChars=40/37`）+ `T102931Z` 截图 |
| 名称 / 状态筛选，点人物进详情，选人不自动派工 | 已实现 + 当前已验证 | 布局例搜索 / 过滤 top-hit；CASE-A / CASE-D |
| 职责与场所：一行一 `WorkplaceId` + `BuildingInstanceId` + `FunctionSlotId`，容量 / 绑定 / 劳动 / 现场分列 | **已实现 + 当前已验证（main slot）** | 岗位行高 132 / Detail 76；6 条岗位行逐字符核对；第二 FunctionSlot 仍**未覆盖** |
| 施工行单列进度与工人 | **已实现 + 当前已验证** | 施工行 3 行 `checkedChars=58`（进度 / 工人姓名与 PersonId / 现场） |
| 已 Completed 动态岗位 `TryAssignPerson` / `RemovePersonAssignment` | 已实现 + 当前已验证 | CASE-A、CASE-D |
| 施工只调用 `TryAddConstructionWorker` / `TryRemoveConstructionWorker` | 已实现 + 当前已验证 | CASE-B + Edit 第 18 例 |
| 资源页：Ingredients / Food、在村供餐对象、Hungry 人数、Farm / Kitchen 实例名额与实际劳动 | **已实现 + 当前已验证（业务行层）** | 6 条真实资源行逐字符核对（名额 / 劳动 / 实例 / 槽位）+ `T102931Z`；**批次 / 供餐细节仍 Deferred** |
| 动态岗位经 `FunctionBindings` + `BuildingWorkplaceId` / `GetWorkplaceDef` 互核 | 已实现 + 当前已验证（单 slot）；**第二 FunctionSlot 未覆盖** | Edit `SameDefinitionBuildingsAndRegisteredSlotsStayIndependent`；配置只登记 `main` |
| 未完工建筑不出可派工岗位；0 工人保留进度不写 Pause | 已实现 + 当前已验证 | CASE-B 0-worker progress=0 |
| 静态岗位（`ws_farm_01`）显示既有绑定并可撤职，不假装正式建筑 | 已实现 + 当前已验证 | CASE-B legacy row |
| 跨页同一 Selection；Building / Place ↔ Person 互跳 | 已实现 + 当前已验证 | CASE-A/C/D + 资源行点击回“职责与场所”（`tab=Workplaces`） |
| 名额与现场 / 实际劳动独立计算 | 已实现 + 当前已验证 | 劳动口径 + CASE-B/D |
| 不提供原子改派：`AlreadyAssigned` 拒绝，显式先撤后派 | 已实现 + 当前已验证 | CASE-A END |
| 撤职移除 Assignment 记录与累计统计，不包装无损 Undo | 已实现 + 当前已验证（含边界） | CASE-D；旧 `arrival` 只在下一边界清 |
| 管理命令成功后显式保存并显示 bool；失败不称已保存 | 已实现 + 当前已验证 | CASE-C SAVE-SUCCESS / FAIL-EXPECTED / RECOVERED |
| 推进 / 返程 busy 时命令与保存禁用并给已有诊断 | 已实现 + 当前已验证 | Return gating；事件 pending 门禁 |
| claim 持有者加入施工 | 已实现 + 当前已验证 | 时间线 464 → 465 → 480 → 481；Edit 18/18 |
| 拒绝实现清单（Test Project / 排行榜 / 完成日估算 / 自动最优派工 / 职业转换 / 批量事务 / 经济重平衡 / 仓储物流 / 升级拆除 / 新 Activity 规则 / 破坏性迁移） | 已遵守（未实现） | 代码与文档一致 |

### 2.2 §5 页面、UI 与路由接缝

| 要求 | 状态 |
|---|---|
| 唯一 World / 镜头 / Selection（`ProductHomeWorldHost`），读模型不写 Authority | 已实现 + 当前已验证（三表面等价） |
| 原 Person / Management Surface 替换白模正文，无额外 Scene / 第二 Home 入口 | 已实现 + 当前已验证 |
| `HomeSelectionContext` 来源替换取消页面选择 / 订阅 | 已实现 + 当前已验证 |
| 固定 UI 结构为 Editor 可编辑 Prefab，Runtime 只生成重复行 | 已实现 + 当前已验证 |
| 内部页面不 `SetSimulationActive(false)` / 不 Pause / 不改 Speed | 已实现 + 当前已验证 |
| 关闭返回 World 且保留有效选择；ESC 与关闭同一回调 | 已实现 + 当前已验证 |
| 全屏 Raycast Blocker + World 输入阻断 | 已实现 + 当前已验证（程序化 Raycast） |
| TopHUD 时间控制在模态允许区可操作 | 已实现 + 当前已验证（`top=Pause`） |
| 键盘 / 镜头 / **物理鼠标**穿透 | **未覆盖**（仅程序化 `RaycastAll + ExecuteEvents`） |
| 远征仍走 `Town.EnterProductExpedition` gating | 已实现 + 当前已验证 |
| 旧 Pending 事件 / Return 阻断优先级 | 已实现 + 当前已验证 |

### 2.3 §6–§8

- **§6 实施顺序**：已按原文执行；产品验收卡第 6–10 条当前已验证（CASE-A/B/C/D、Return gating）；第 1–5 条属 R3-A，沿用既有回执、本包未重跑；**制作人手工点击手感未覆盖**。
- **§7 风险 / 未决 / Stop**：管理命令前置（存在 → 可用性 → 世界锁）已实现并验证；撤职即时边界按事实断言；Growth / Social / 死者档案 / GameSpec 滞后 / Save Ownership / 世界级 Load transaction 保持既有缺口；字体保护成立；**1280 由制作人 `01a53eb8` 取消（取消 ≠ 通过）**。**Stop Conditions 未被违反**（未改 Work/Clock/Activity/Social 规则、SaveVersion/DTO、配置 Authority，未新增 busy 拒绝）。
- **§8 测试矩阵**：Edit 管理命令 **18/18（前阶段真实通过）**；Play 真实产品入口 / 劳动 / 资源 / Simulation 对照 / Save-Continue-NewGame / 远征-事件 gating **13/13 当前已验证**；模态与布局：**1920 布局、遮挡、真实 overflow 滚轮 + 多轮拖拽、四类业务行标题与详情、末行可达均已覆盖（程序化输入）**；**物理鼠标 / 1280 未覆盖（1280 取消）**；人物只读投影、关系 / 历史、成长沿用前阶段回执。

---

## 3. 当前差异（文件与完整 SHA，只读重算）

| 路径（仓库相对） | 变更性质 | bytes | SHA256 | mtime（UTC） |
|---|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/ManagementView.cs` | 人口 / 施工 / 资源行文本分行 + 行高 132；岗位行序沿用；`ConstructionDetail` 工人行独立 | **39037** | `f8316e436845bd7266cfd3a6f0fa013cdcd3acb5705161a94d5c67e172016473` | 2026-10-26T10:21:54.3087834Z |
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` | **四个行模板**统一 `1100×132`、`LayoutElement 132`、`Detail 1072×76@(14,-44)`、`Label 1072×32@(14,-8)` | **282332** | `31bf3e8c111700332adfaa87d93586ea065631090416cdc2f8cf2629a30e8764` | 2026-10-26T10:21:47.9465950Z |
| `…/ManagementView.prefab.meta` | 未改 | 178 | `b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559` | 2026-10-23T12:10:23Z |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 第 13 例强化：四类业务行逐字符断言、真实资源业务行、真实人口行、多轮拖拽 | **390868** | `219379e01567b36cab0af7f73aee9f133bb027abcf1566be1c69f39f51c7a45c` | 2026-10-26T10:24:36.3694583Z |
| `…/ManagementShellPlayTests.cs.meta` | 未改 | 267 | `9da27352b62ef02bed6e5ac640453677938d3a8fa885dcf896d5799713570404` | 2026-10-23T12:22:24Z |
| `…/Library/ScriptAssemblies/Assembly-CSharp.dll` | 编译产物（晚于源码 70.1 s） | 647680 | `b31f1b6408846a37deb2d948b840897189ef981f25c5ff9e514316f860dbe271` | 2026-10-26T10:23:04.4074813Z |
| `…/Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 编译产物（晚于源码 4.23 s） | 512512 | `40e41b98299322e32f5916df5376861e81645426ebe352784de258d85ea2ea3b` | 2026-10-26T10:24:40.5953248Z |
| `…/Library/ScriptAssemblies/VillageStage1.Tests.dll` | 本阶段未重编译 Edit 测试 | 153088 | `9d09dabbb127fc58235e271d1029c2b57ae2c155d9d3dc1fab08b86fa65b69da` | 2026-10-26T03:08:50.4871113Z |

Prefab 结构计数（文本实测）：`1072×32`=4、`1072×28`=0、`1072×42`=0、`1072×76`=4、`1100×132`=4（`1100×100/92/84/86` 均 0）、`m_PreferredHeight: 132`=4、`anchoredPosition {14,-44}`=4、`{14,-8}`=4。测试源码：`[UnityTest]`=13、`[UnityTearDown]`=0、`Screen.SetResolution`=0、`new Vector2Int(1280`=0、`Resource=NotCoveredByThisCase`=0。

相对上一个交付轮（仅岗位行可读）的差异：**Person / Construction / Resource 三个模板行高 86/92/84→132、Detail 42→76，产品代码新增分行与工人行独立**，属本任务“必要明细补齐”范围；未新增功能、未改 Authority / DTO / 配置 / 存档格式。

---

## 4. 严格口径：当前验证 / 历史 / 未覆盖 / 取消

| 门禁 / 项 | 结果 | 版本范围 |
|---|---|---|
| 管理 Play 整类 13 例 | **13/13 Passed**；`skipped` 字段本层读不到（待主开发补）；方法耗时合计 **11 754 ms**；日志 10:29:23–10:29:35 段 13 组 `SAVE-RESTORE matches-before=True`、3 组 `SCHEDULE-RESTORE`、`FONT-POST`、`BASELINE-POST 34/34` | **当前版本真实回执**；宿主 runId / run 级时长 / skipped **待主开发补** |
| 滚动专项单方法（全部明细补齐后） | `e75c1ec5…` **1/1、0 skipped、`3.1183267` s、scope-restored**（主开发已核） | 单例 |
| 管理 Play 旧口径 | 13/13（08:53，仅岗位行详情版）、12/12 `4aa4ffda`、11/11 `7f600ef9`、10/10 `0fd4b031` | **历史**，不覆盖当前版本 |
| 管理 Edit 整类 | **18/18**（`99dc1dfe`） | **前阶段真实通过**，本阶段未重跑（Edit 源码未改） |
| Compiler / 完整 Edit / 完整 Play | `264/264`、`1513/1513`、`122/122` | **前阶段真实通过，不是最新全量** |
| 1280×720 | 制作人 `01a53eb8` **取消** | **取消 ≠ 通过** |

**未覆盖（如实保留，不升级为“需制作人逐步审批”）**：

1. **第二 `FunctionSlot` 真实 Gateway**：`BuildingConfigDatabase.asset` 只登记 `FunctionBindings: main`，`StaticConfigRegistry` 无公共注入点——属**建筑配置面限制**，与单槽存档无关。
2. **物理鼠标 / 键盘 / 镜头穿透与手感**：全部为程序化 `EventSystem` 路径。
3. **Chevron**：仍 `44×28`、字号 24，非必要指示，未修未测。
4. **动态批次 / 供餐细节**：Deferred。
5. **历史 650 存档**：`21075B / 650c45c4…` 授权内无字节备份、未恢复、未伪造；当前 `13405-4b2b56` 不是它。
6. **`SaveSlotGuard` 进程级缺口**：仍只在用例收尾恢复。
7. **`CapacityFull` 域名诊断未经按钮进入**（`[CASE-A] FULL-AT-UI disabled=true`，见日志 3579）：按钮禁用已断言，域内诊断文案未单测。

**已解决、不再列为当前未测**（历史文本仍保留标“当时”）：Person / Resource 行 detail 截断、施工行工人列表隐藏、资源业务行标题无真实证据。

---

## 5. 必要下一步（不在本阶段执行）

1. **制作人产品验收**：以交付索引 §5 入口在 1920 真实窗口手工滚轮 / 拖拽 / 点击一次，记录手感与观感（自动化不代替）。
2. **主开发补 runId / run 级时长 / skipped**：本层只读面读不到，需宿主运行记录补齐后再入档。
3. **第二 FunctionSlot**：仅在获得配置 Authority 或公共注册 API 授权后另立包。
4. **动态批次**：按后续规划另行授权，当前 Deferred。
5. **Git 定稿 / 提交 / 发布**：需单独 Git 授权；当前无 Commit / Push。
6. **不建议**在收尾阶段重跑 264 / 1513 / 122 换数字；如需覆盖最新源码 / Prefab 差异，应作为独立“最新版本完整回归包”并说明耗时与保护面。

---

## 6. 边界与诚信声明

- 本阶段唯一 Editor 操作：`rql-query` 只读查询、`script-execute` **只读** C#（读文件 / 重算 SHA256 / 解析基线 JSON / 解析 PNG IHDR / 反射读 `IsRunActive` 与 `SaveSystem.GetSavePath` / `EditorUtility.IsDirty` / 目录与 manifest 统计 / 读日志）；本地 `read` / `grep` / `read_image`。
- 未运行 `tests-run`、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局保存、未 SetDirty / ClearDirty、未 Shell / Git 写、未写 Preferences、未启停 Editor；未改代码 / 测试 / Prefab / 字体 / 存档 / manifest / 基线 / 备份 / 正式工程。
- **两次历史边界违规原文保留、不洗白**：① 旧 `4fbf7754` 轮 `EditorUtility.ClearDirty`；② 详情修复包 `AssetDatabase.SaveAssets()` 越界调用（`workplace-detail-readability-review.md` §6.3）。另：`ManagementView.prefab` 历史插件自动回写 `282097B→282326B`（无备份、根因未定），不宣称“仅 4 个数字变化”。
- 字体 / 34 项磁盘 hash 未变只证明**观察面**未被写，**不推断全机零副作用**；本阶段唯一新增写入为本文件与文档同步。
- 本文件及整包结论须由**主开发专业核验**，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。
