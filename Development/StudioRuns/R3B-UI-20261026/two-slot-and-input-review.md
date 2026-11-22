# 双槽 19 例结果与剩余输入门禁范围核对（2026-10-26，阶段 2/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包性质：**只读核对 + 文档更新**。未改代码 / 配置 / Prefab / Prefs / manifest / 场景 / 存档；未 `AssetDatabase.Refresh`、未 `SaveAssets / SaveChanges / ClearDirty / SetDirty`、未字体测量、未 PrefabStage、未 `rql-mutate`、未 Shell / Git 写、未启停 Editor；**模型未运行 `tests-run`**。
- 实际写入：本文件（新增）+ `reviewable-delivery-index.md`（更正第二 slot 口径、补 19 例当前结果与输入证据边界）。未删除历史记录。
- 核对时刻：**2026-10-26T11:00Z 前后**（本机 `19:00 +08:00`，China Standard Time）。
- 状态口径：R3-B 仍 **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；交付 contract 的 `requiredChecks` 仍 **partial**；不请求完成、不写 `awaiting_acceptance`、不代表制作人批准。

---

## 1. 管理 Edit 整类 19 例：可见结果与不可见项

### 1.1 本机可见的真实结果

`rql-query testResults` 当前返回 **19 条**，全部为 `RRPG.Management.Tests.ManagementProjectionTests.*`，全部 `Passed`、`message=null`：

| # | 用例 | 状态 | durationMs |
|---|---|---|---|
| 1 | `CapacityFullAndAlreadyAssignedPreserveDomainDiagnosticsWithoutWrite` | Passed | 3 |
| 2 | `CompletedBuildingRejectsConstructionWorkerWithoutWrite` | Passed | 7 |
| 3 | `ConstructionLaborDoesNotCountTowardPrimaryWorkplace` | Passed | 10 |
| 4 | `ConstructionWorkerKeepsPrimaryAssignment` | Passed | 2 |
| 5 | `DeadUnavailablePersonIsRejectedByEveryManagementCommandWithoutWrite` | Passed | 3 |
| 6 | `LegacyStaticAssignmentIsVisibleRemovableButNotANewTarget` | Passed | 1 |
| 7 | `ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability` | Passed | 9 |
| 8 | **`ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent`** | **Passed** | **32** |
| 9 | `MissingOrAwayPersonIsRejectedWithoutWrite` | Passed | 2 |
| 10 | `ReadProjectionDoesNotTouchWorldClockOrAuthority` | Passed | 5 |
| 11 | `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution` | Passed | 3 |
| 12 | `RemovingAssignmentDuringLaborStopsOldWorkplaceEvidence` | Passed | 2 |
| 13 | `ResourcesAndNutritionFactsComeFromExistingState` | Passed | 2 |
| 14 | `SameDefinitionBuildingsAndRegisteredSlotsStayIndependent` | Passed | 2 |
| 15 | `SameNameDifferentPersonIdsStayIndependentAndFilterIsSessionOnly` | Passed | 3 |
| 16 | `SaveFeedbackUsesExistingBoolAndRespectsWorldLock` | Passed | 5 |
| 17 | `UnderConstructionBuildingHasNoAssignableWorkplaceAndCommandRefuses` | Passed | 1 |
| 18 | `WorkplaceLaborOnlyCountsActualWorkplaceExecution` | Passed | 2 |
| 19 | `WorldBusyRejectsManagementCommandsWithoutWrite` | Passed | 2 |

- 方法耗时合计 **96 ms**（上表 19 项求和），全部 `message=null`。
- 本机 MCP server 日志 `Library/mcp-server/win-x64/logs/server-log.txt:26045-26046`：`2026-10-26 18:55:51.9683` `"tests-run" completed. IsError = False.`，调用耗时 `10013.2963 ms`。这只证明一次工具调用级成功，不包含用例名/通过数/skipped。
- 19 例发现仍为唯一集合：`tests | where fullName starts-with "RRPG.Management.Tests.ManagementProjectionTests."` = **19**；第 19 例 FQN 唯一命中 1；DLL 新鲜度见阶段 1 预检（`VillageStage1.Tests.dll` mtime `10:49:48Z` 晚于源码 `10:49:36Z`）。
- 双槽单例上游回执（主开发已核）：`740fd50c-8680-4dc6-8a4b-8589e501fedc`，**1/1 Passed、0 skipped、1.9201088 s、scope-restored**。本文件不把它与 19 例整类回执混为一条。

### 1.2 本层不可见、必须待主开发补的项

| 项 | 现状 | 处置 |
|---|---|---|
| 19 例整类 `runId` | `testResults` 不携带 runId；MCP server 日志不含参数 | **待主开发补** |
| 19 例整类 `skipped` | `testResults` 只给每条状态，无 run 级 skipped 汇总 | **待主开发补** |
| run 级总时长 | 只有工具调用 `10013.2963 ms`；非 TestRunner run 级字段 | **待主开发补**；不把工具耗时写成测试耗时 |
| 逐断言/日志正文 | 19 例没有 `[TWO-SLOT]` 或 `TestContext.WriteLine` 的收尾输出；通过状态等于断言未失败 | **没有可逐行引用的运行正文**；不补写、不臆造 |
| 双槽“收尾默认 registry 恢复 / 覆盖 null / 其他 domain 指纹不变” | 第 19 例源码在 `finally` 后有三项断言，且该例 `Passed`；本机当前只读复核覆盖为 null、默认建筑 registry 4 定义/3 binding、8 个其他领域入口均可解析 | 运行正文不可见；**严格前后指纹只能由主开发持有的完整 Studio 记录补** |

> 结论：**19/19 Passed 是当前真实可见结果**；但 runId、skipped、run 级时长、逐断言正文不可见，按实留空，不据此升级为 Completed / Accepted。

---

## 2. 原规格对输入的具体要求（按原文，不新增规则）

| 出处 | 原文要点 | 对本包的含义 |
|---|---|---|
| R3-P Spec §5（`Docs/DevelopmentSpecs/2026-10-09_...V0.1.md:112`） | 模态必须有全屏 Raycast Blocker，并结合 World 输入拦截；**世界点击、镜头输入、建造、底部导航、旧 Town 按钮在模态中都须阻断**；嵌套选择不能让底层抢 ESC；Home host 当前按 EventSystem Raycast 命中阻断鼠标，**键盘/镜头/旧模态组合仍需 Play 实证** | 规格明确要求“阻断镜头输入”，但**没有逐字规定 pan/zoom 的键位或灵敏度**；键位来自现有产品代码，不是本规格新增验收数值 |
| 同 §8 测试矩阵（`:160`） | PlayMode 模态/布局：**点击/拖拽/滚轮/键盘不穿透到 World/建造/旧 Town**；TopHUD 暂停/速度可操作；关闭回 World；中文/长列表/1920×1080 和 1280×720 可读 | 测试项是“穿透阻断”行为，不是“手感”评分；1280 已由制作人 `01a53eb8` 取消，取消 ≠ 通过 |
| 同 §5（`:110`） | 内部页面和成长模态不调用 `SetSimulationActive(false)` / `Time.Pause` / 改 Speed；**ESC 与关闭按钮走同一回调** | ESC 属“顶层关闭”语义，不应被嵌套层抢走；不是“ESC 必须被 World 忽略” |
| 同 §5（`:100`） | `ProductHomeWorldHost` 保留唯一世界、镜头、Selection；读模型不得写 Authority | 输入门禁应留在现有 Host/Shell，不新建第二套输入系统或第二世界 |

现有产品实际键位（源码事实，非规格新增）：

| 输入 | 产品入口 | 行为 |
|---|---|---|
| 鼠标左键 | `ProductHomeWorldHost.Update` → `HandleWorldClick(Input.mousePosition)` | 非模态、非 UI、非建造时选中/清空 World |
| 鼠标右键 / 中键拖动 | `HomeWorldCamera.ReadInput` → `Pan` | 非模态且非建造右键抑制时平移镜头 |
| 鼠标滚轮 | `HomeWorldCamera.ReadInput` → `Zoom` | 非模态时缩放镜头（范围 2.5–18；原型 4–11） |
| ESC | `HomeProductShell.LateUpdate` → `HandleEscape` | 表面模态/普通页面关闭回 World；建造打开时 `HandleEscape` 不关表面 |
| ESC / 右键 | `HomeBuildingPlacement.ReadInput` | 建造中取消候选；左键在非 UI 时确认放置 |
| UI 点击/滚动/拖拽 | `EventSystem` + `ExecuteEvents` | 现有 Play 自动化的唯一“真实事件”路径，但仍是程序化事件 |

**关键结论：规格要求“镜头输入在模态中阻断”，但没有要求新增键盘镜头键位；当前产品也没有键盘 pan/zoom。** 因而不能把“键盘镜头”当作未实现的规格项；真正要核的是现有 ESC/建造键是否被模态正确隔离。

---

## 3. 实际输入入口与门禁（源码只读）

### 3.1 `ProductHomeWorldHost`（`Assets/Scripts/Village/ProductHome/ProductHomeWorldHost.cs`）

| 行 | 事实 |
|---|---|
| 75 | `bool overUI=Shell.IsPersonModalOpen || PointerOverUI(Input.mousePosition);` |
| 75 | `WorldCamera.ReadInput(overUI,Shell.Build.IsOpen);` — 模态时 `overUI=true`，镜头输入被跳过 |
| 76 | `if(Shell.Build.IsOpen) Shell.Build.ReadInput(overUI);` — 建造只在打开时读输入 |
| 78 | `if (!Shell.Build.IsOpen && !overUI && Input.GetMouseButtonDown(0)) HandleWorldClick(Input.mousePosition);` — 模态/UI/建造时不处理世界左键 |
| 182–188 | `public void HandleWorldClick(Vector2 screen)`：再次检查 `Shell.IsPersonModalOpen || PointerOverUI(screen)`，命中则 `Selection.Clear()` 或 `hit.Click()`，随后 `RefreshNow()` |
| 219–224 | `public bool PointerOverUI(Vector2 screen)`：`EventSystem.current.RaycastAll`，任意 UI 命中即 true |

### 3.2 `HomeProductShell`（`HomeProductShell.cs`）

| 行 | 事实 |
|---|---|
| 15 | `public bool SurfaceModalActive => Router.Surface==Person || Router.Surface==Management || GrowthUI?.IsOpen==true;` |
| 16 | `public bool IsPersonModalOpen => SurfaceModalActive;`（既有 Host 别名，覆盖 Management） |
| 95 | `LateUpdate` 中 `Input.GetKeyDown(KeyCode.Escape)` → `HandleEscape()` |
| 130 | `SetTownInputBlocked(SurfaceModalActive && ShellVisible)` |
| 131–132 | 模态时 Attention 按钮禁用、Attention 面板关闭 |
| 156 | `public void HandleEscape() { if(!Build.IsOpen) CloseSurface(); }` |
| 163–169 | `SetTownInputBlocked`：禁用/恢复 `TownUI.canvasRoot` 的 `CanvasGroup.interactable / blocksRaycasts` |
| 175 / 188 / 191 | 模态时 `Navigate` / `ToggleCompatibility` / `NavigateAttention` 直接 return |

### 3.3 `HomeWorldCamera`（`HomeWorldCamera.cs`）

| 行 | 事实 |
|---|---|
| 25–34 | `public void Pan(Vector2 delta)`：直接改 transform；原型边界 ±18 / ±12 |
| 35 | `public void Zoom(float delta)`：改 `orthographicSize`，2.5–18（原型 4–11） |
| 36–52 | `public void ReadInput(bool overUI,bool suppressRightPan=false)`：仅当 `!overUI` 才读 `Input.GetMouseButton(1/2)` 与 `Input.mouseScrollDelta`；**没有任何键盘分支** |

### 3.4 `HomeBuildingPlacement`（`HomeBuildingPlacement.cs`）

| 行 | 事实 |
|---|---|
| 99–106 | `public void ReadInput(bool overUI)`：`Input.GetKeyDown(Escape)` 或右键 → `Cancel()`；`SelectedDefinitionId==null || overUI` → return；否则 `UpdatePointer(Input.mousePosition)`，左键 `Confirm()` |
| 17–22 | 可观测：`IsOpen`、`SelectedDefinitionId`、`Candidate`、`Diagnostic`、`PreviewValid`、`GhostVisible` |

---

## 4. 已有 Play 断言实际覆盖到哪里

| 证据 | 实际做法 | 能证明 | 不能证明 |
|---|---|---|---|
| `ManagementShellPlayTests` 文件头 `:39-41` | 明确写“EventSystem.RaycastAll + ExecuteEvents.PointerClick 的程序化点击……不等价物理鼠标人工体验；指针手感/拖拽/镜头滚轮留后续/人工” | 作者未把程序化点击冒充物理输入 | — |
| `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` `:2950-3039` | 背景点 `RaycastAll` 命中 Blocker/面板；`Call(_host,"HandleWorldClick",background)`；底部导航 Raycast 不得命中按钮；TopHUD 用 `RealPointerClick` | 模态下 UI 射线命中 Blocker；**直接调用** World 点击处理器时 Selection/Surface 不变；TopHUD 可操作 | 没有经过 `Input.GetMouseButtonDown(0)`；不能证明物理鼠标点击被 OS/输入层阻断 |
| 同例 `:3027-3031`；`CloseButtonAndEscape...` `:3073-3083` | `Call(_shell,"HandleEscape")`，断言回 World、保留 Selection、恢复 Town CanvasGroup | ESC 与关闭走**同一 public 回调**，结果一致 | 没有经过 `Input.GetKeyDown(Escape)`；不能证明物理按键、焦点、IME、按住重复 |
| 同例 `:3058-3060` | 管理打开时 `TownUI.canvasRoot` 的 `CanvasGroup.interactable=false / blocksRaycasts=false` | 旧 Town 指针射线/交互被禁用 | 不能替代旧 Town 自身键盘提交的物理验证 |
| `:2980-2992` | 建造导航被 Blocker 覆盖；`Build.IsOpen=false` | 模态下 UI 层不能打开建造 | 不能证明物理右键/ESC 在建造态的行为 |
| `ProductHomeFoundationPlayTests:99-101` | 直接 `Call(cameraControl,"Pan",...)` / `Zoom(-2)`，断言相机变化 | Camera API 可用、Focus 后位置正确、镜头不入 Save | 不是输入测试；没有经过 `ReadInput`，不能证明 pan/zoom 阻断 |
| `R3AGrowthTransactionPlayTests:49` | `Call(host,"HandleWorldClick",...)`，断言 Selection 不变 | 人物模态下直接调用 World 处理器不改变选择 | 同样是直接回调，不是物理点击 |
| 全仓测试 grep | **0 处** `HomeWorldCamera.ReadInput` / `PointerOverUI` / `GetKey` / `KeyCode` / `mouseScrollDelta` 测试调用 | — | 镜头输入、键盘事件、物理鼠标穿透均无运行证据 |

**结论：**
- 已有 Play 覆盖的是 **UI 程序化射线 + 直接 public 回调**，不是物理输入。
- `HandleWorldClick` 是 public 产品入口，不是 internal；但**直接调用它仍然绕过了 `Input.GetMouseButtonDown` 与真实指针**。本文件不把它称作物理输入证据。
- `HandleEscape` 同理：它是 ESC 与关闭按钮的共同回调，可证明“回调语义”，不能证明“物理 ESC 键被正确路由且未被底层抢走”。

---

## 5. 可补的机制级自动化 vs 必须物理设备

### 5.1 可在现有公开入口补的机制级自动化（不需要改产品代码）

| 可补断言 | 现有公开入口 | 可观测字段 | 性质 |
|---|---|---|---|
| 管理/人物模态打开时 `SurfaceModalActive` 与 `IsPersonModalOpen` 同时为真；关闭后同时为假 | `Shell.OpenSurface/CloseSurface`；`Shell.SurfaceModalActive` / `IsPersonModalOpen` | `Router.Surface`、两布尔、`Selection.Id` | 机制/状态一致性；已有部分覆盖，可补“两别名始终同值” |
| 模态背景点 `PointerOverUI(point)==true`，且 `HandleWorldClick(point)` 不改变 Selection/Surface | `Host.PointerOverUI(Vector2)`、`Host.HandleWorldClick(Vector2)` | `Host.Selection.Kind/Id`、`Router.Surface`、`Host.Snapshot` | UI 门禁逻辑；**不是物理点击** |
| 嵌套 Surface 下 `HandleEscape()` 只关闭顶层，不把底层 World 选择/Clock 改掉 | `Shell.HandleEscape()`；`Shell.OpenSurface` | `Router.Surface`、`Selection.Id`、`VillageRuntimeBridge.Time.Speed`、`IsSimulationActive` | 回调语义；**不是物理 ESC** |
| 建造打开时 `HandleEscape()` 不关闭表面（`HandleEscape` 内 `if(!Build.IsOpen)`） | `Shell.HandleEscape`、`Shell.Build.IsOpen` | `Build.IsOpen`、`SelectedDefinitionId`、`Router.Surface` | 回调语义；可补“建造态 ESC 先取消建造” |
| 模态下旧 Town `CanvasGroup.interactable/blocksRaycasts` 为 false，关闭后恢复原值 | `Shell.SurfaceModalActive`、`TownUI.canvasRoot` | `CanvasGroup` 两字段 | 已有覆盖，可补“打开/关闭多次不漂移” |
| `HomeWorldCamera.ReadInput(true,true)` 后相机 transform / `orthographicSize` 不变 | `HomeWorldCamera.ReadInput(bool,bool)` | `ViewCamera.orthographicSize`、`transform.position` | **弱证据**：没有注入鼠标状态时，不变是预期/近乎恒真；不能单独算“阻断通过” |
| `HomeBuildingPlacement.ReadInput(true)` 不改 `Candidate` | `HomeBuildingPlacement.ReadInput(bool)` | `Candidate`、`Diagnostic`、`GhostVisible` | **弱证据**：同上，无输入注入时只能证明入口在 `overUI` 下提前 return |

> 这些“可补”项里，只有前 5 项是真正有意义的**机制级**断言；最后两项若没有输入注入，只能作为辅助，不应单独写成“镜头/建造物理输入阻断已通过”。

### 5.2 需要输入注入 seam（当前不存在）才能自动化

| 目标 | 当前阻塞 |
|---|---|
| 真实右键/中键拖动 → `Pan` | `HomeWorldCamera.ReadInput` 直接读 `Input.GetMouseButton(1/2)` 和 `Input.mousePosition`；legacy `Input` 没有可写 API；项目 `Packages/manifest.json` **无 `com.unity.inputsystem`**，也没有 `InputTestFixture` |
| 真实滚轮 → `Zoom` | 同上，`Input.mouseScrollDelta` 不可注入 |
| 真实左键点击 → `HandleWorldClick` | `Input.GetMouseButtonDown(0)` 不可注入；EventSystem 只能驱动 UI，不驱动 Host 的 `Update` 分支 |
| 真实 ESC → `HandleEscape` | `Input.GetKeyDown(KeyCode.Escape)` 不可注入；直接调 `HandleEscape` 绕过按键路由 |
| 建造右键/ESC 取消 | `HomeBuildingPlacement.ReadInput` 同上 |

若主开发批准新增测试缝，最小形态应是“输入快照参数化”，而不是反射改 `Input`：

- `HomeWorldCamera.ReadInput(WorldPointerInput input, bool overUI, bool suppressRightPan=false)`，其中 `WorldPointerInput` 为只读 struct（`mousePosition`、`rightHeld`、`middleHeld`、`scrollDelta`）；产品 `Update` 用 `Input` 填充，测试用合成值。
- `HomeProductShell.ProcessEscape(bool pressed)`（或等价 `TickInput(bool escapePressed)`）；产品 `LateUpdate` 用 `Input.GetKeyDown` 填充。
- 不应新增 `SetInputForTests` 全局静态单例；参数化入口更窄、可逆、无残留。

**本包不实施上述代码缝**；它属于主开发决定，且会改产品源码。

### 5.3 必须物理设备 / 人工手感

| 项 | 原因 |
|---|---|
| 真实鼠标点击/拖拽/滚轮穿透 | OS 指针、焦点、窗口坐标、UI 命中顺序、DPI/缩放共同决定；程序化 `EventSystem` 不能等价 |
| 真实 ESC 键、键盘焦点、IME/输入法 | `Input.GetKeyDown` 的物理路由无法由现有 public 回调等价 |
| 镜头平移/缩放手感（速度、步进、边界） | 规格只要求“模态中阻断”，未冻结手感数值；属制作人人工体验，不是自动断言 |
| 多键/多指针组合、快速连点、拖拽中断 | 需要真实设备与时间序列；当前无输入注入工具 |

> 不把所有缺测都推给人工：上表 5.1 的 5 项机制级断言可在现有宿主内补；5.2 的输入链路在无工具时不能自动化；5.3 才是物理设备/手感。**“没有合法输入注入工具”是当前工具限制，不是产品缺陷，也不是新的授权门槛。**

---

## 6. 可在既有宿主内补充的最小测试方法名 / 字段

### 6.1 建议方法名（仅命名，不在本包新增）

1. `ManagementModal_EscapeCallback_ClosesOnlyTopSurface_AndKeepsSelection`
2. `ManagementModal_PointerOverBlocker_MakesWorldHandlerNoOp`
3. `ManagementModal_BuildOpen_EscapeCancelsBuildBeforeClosingSurface`
4. `ManagementModal_TownCanvasGroup_BlocksAndRestoresAcrossRepeatedOpenClose`
5. `PersonModal_EscapeAndClose_ShareSameCallback_AndKeepSelection`（可复用既有用例，不重复造）

### 6.2 现有可观测字段 / 入口

| 类型 | 成员 |
|---|---|
| `ProductHomeWorldHost` | `WorldCamera`、`Shell`、`Selection`、`Snapshot`、`WorldVisible`、`PointerOverUI(Vector2)`、`HandleWorldClick(Vector2)`、`RefreshNow()` |
| `HomeProductShell` | `SurfaceModalActive`、`IsPersonModalOpen`、`Router`、`ShellVisible`、`Build`、`OpenSurface(HomeSurface)`、`CloseSurface()`、`HandleEscape()`、`HudText` |
| `HomeProductRouter` | `Surface`（`World/Person/Management/Cards/History`） |
| `HomeWorldCamera` | `ViewCamera`、`ReadInput(bool,bool)`、`Pan(Vector2)`、`Zoom(float)`、`Focus(Vector3)`；可观测 `ViewCamera.orthographicSize`、`transform.position` |
| `HomeBuildingPlacement` | `IsOpen`、`SelectedDefinitionId`、`Candidate`、`Diagnostic`、`PreviewValid`、`GhostVisible`、`ReadInput(bool)`、`Cancel()` |
| `HomeSelectionContext` | `Kind`、`Id`、`SelectPerson/SelectPlace/SelectBuilding`、`Clear` |
| `VillageRuntimeBridge` | `IsSimulationActive`、`Time.Speed`、`Time.SpeedMultiplier`、`Assignments`、`Projects` |
| `TownUI` | `canvasRoot` 上的 `CanvasGroup.interactable / blocksRaycasts` |

---

## 7. 本包工具限制（精确记录，不新开 Shell / 外部进程 / 插件）

| 工具 / 能力 | 实际限制 |
|---|---|
| `rql-query` | 只读场景/测试结果/Console；不能注入输入 |
| `script-execute` | 可在 Editor 内跑只读 C# / 反射；**不能写 legacy `Input` 状态**；不调用 `SendInput`、不 `Process.Start`、不装插件 |
| `EventSystem` / `ExecuteEvents` | 只能驱动 UI 事件；不是 `Input.GetMouseButton*` / `GetKeyDown` / `mouseScrollDelta` |
| Unity Input System `InputTestFixture` | 工程 `Packages/manifest.json` 无 `com.unity.inputsystem`，不可用；装包被本包禁止 |
| OS 级输入注入 | 需 P/Invoke `SendInput` 或外部进程/Shell，均被本包禁止 |
| `tests-run` | 模型不运行；本包 19 例结果是宿主已产生的 `testResults`，本包只读 |
| PrefabStage / 字体测量 / Refresh / Save | 本包均未执行 |

因此：**当前授权内没有合法输入注入工具**；可用的是“public 回调 + EventSystem 程序化事件 + 状态字段断言”。任何“物理鼠标/键盘/镜头穿透已通过”的表述都会越出证据。

---

## 8. 与交付索引的同步

本包同时更正 `reviewable-delivery-index.md`：

1. 第二 FunctionSlot 不再写“需配置 Authority / 公共 API 授权后另立包”；改为 **Editor-only internal 隔离入口已实现、单例 1/1、整类 19/19**，并注明未新增 public API、未构建 Player、生产配置仍只有 `main`。
2. 增加**管理 Edit 整类当前 19/19** 门禁行，保留 18/18 为前阶段历史。
3. 输入风险行改为精确口径：已有自动化是 EventSystem 程序化 UI + 直接 `HandleWorldClick` / `HandleEscape`；物理鼠标/键盘/镜头与手感仍未覆盖。
4. 增加 `requiredChecks` 仍为 **partial** 的记录；不请求完成、不写 `awaiting_acceptance`。
5. 历史“未覆盖 / 需授权”判断保留在原 `two-function-slot-scope-review.md` 与索引历史行，不删除、不洗白。

---

## 9. 边界与结论

- **唯一写入**：本文件（新增）+ `reviewable-delivery-index.md`（文档更正）。未改代码 / 测试 / 断言 / Prefab / Scene / 字体 / 存档 / manifest / 34 项基线。
- 未运行 `tests-run`；未 Refresh、未 `rql-mutate`、未 PrefabStage、未 Shell / Git 写、未 Preferences、未启停 Editor。
- 19 例结果：**本机 19/19 Passed、message=null**；`runId / skipped / run 级时长` 与逐断言正文不可见，**待主开发补**。
- 第二 slot：**真实 EditMode Gateway 已覆盖**；UI 双档位渲染、Save/Load 双 slot 仍未覆盖；**不是 public API 变更，也不声称 Player 已构建**。
- 输入：机制级可补项 5 项；真实输入注入工具不存在；物理鼠标/键盘/镜头/手感必须人工或后续批准输入缝；不把 `HandleWorldClick` 直接调用冒充物理输入。
- R3-B 状态不变：**Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；`requiredChecks` 仍 **partial**；本文件须由主开发专业核验，不代表制作人批准。

---

## 10. 写后只读复核（2026-10-26T11:00:23Z）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、mtime `2026-10-10T13:23:55Z`、9 子资产 **0 dirty** | 未变 / 未测量 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、mtime `2026-10-26T10:29:35Z`、`.tmp=False` | 未变 |
| guard | `dirs=244 / restored=243 / protective=1 / other=0 / noManifest=0` | 仍无 unfinished |
| 场景 / Editor | `playing=False`、`compiling=False`、`sceneDirty=False roots=5`、`PrefabStage=null` | 仍干净空闲 |
| 测试源码 | `70548B / 64c8c124…a68a7`、mtime `2026-10-26T10:49:36Z`、`[Test]=19` | 未改 |
| 本文件（初版） | `22934B / b15fcc1c…14de`、mtime `2026-10-26T10:58:17Z` | §10 追加后会改变自身字节，最终以目录实际文件为准 |
| 交付索引（本包改后） | `21247B / a9916ea8…cf9e`、mtime `2026-10-26T11:00:04Z` | 文档更正；若后续再改，以目录实际为准 |

> 写后复核未运行测试、未 Refresh、未保存任何资产 / 场景。19 例整类 runId / skipped / run 级时长仍待主开发补。
