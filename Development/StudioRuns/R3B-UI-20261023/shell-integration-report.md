# R3B-UI Shell 接入与模态生命周期报告（2026-10-23）

状态：**阶段 1/1 完成（源码级最小接线 + 编译/Console/保护回读） / 待主开发专业核验**。
本包只做一件事：把已回读通过的 `Assets/Resources/UI/ManagementView.prefab` 接进现有 `HomeProductShell` 产品入口，复用既有 `Surface`/`Selection`/`CloseSurface`/`SetTownInputBlocked`/`WorldCamera.ReadInput` 输入 Guard 与 0.2s 刷新节流。
**未重做投影/命令/Prefab；未改控制器；未改 Scene；未跑 tests-run；未进入 Play；不宣称模态、TopHUD、输入拦截或产品通过。**

## 1. 授权范围与环境前置（实际回执）

- 唯一工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`；唯一 Unity 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`。
- Unity 连接：`editor | select unityVersion projectPath` → `2022.3.62t9 | D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`；本包未启动/关闭 Editor。
- 开工/收工 `editor | select isPlaying isPaused isCompiling isUpdating` → `false | false | false | false`；`selection=[]`。
- 场景始终：`SampleScene | Assets/Scenes/SampleScene.scene | isDirty=false | rootCount=5`；**本包未写场景、未改 SampleScene**。
- 未安装组件、未改权限/ACL、未动 Packages / 受保护字体 / 插件 / 正式工程；未 Shell / Process.Start；未 commit/push/stage/切分支/改远程；未访问其他工程或认证目录；未读写用户存档；未读 Studio 私有 runtime。
- 未用 `script-execute` 启动测试；`script-execute` 仅用于只读取证（基线 SHA256 重算、DLL 新鲜度、Prefab 只读回读、类型存在性）。

## 2. 本次实际改动（仅 1 个文件，精确 diff）

唯一被修改的文件：`UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeProductShell.cs`

| 项 | 实际值 |
|---|---|
| 改动前 | 15396 bytes，SHA256 `bf87f90c44e4bd9de71763005213fc624acf970eddb89aea8e3877f3555b113b` |
| 改动后 | 15497 bytes，SHA256 `7e9901d7092fda62aea06a2f1a8fbe3bfe2c84974df08eab0002a09015c07fcd` |
| 行数 | 188 → 204 行（+16 行，含 3 行空行/花括号） |

以下为全部 7 处改动（`-` 原行，`+` 新行；行号为改动后文件）：

### 2.1 公共状态：新增 `Management` 与统一模态判定（第 11–16 行）

```diff
     public PersonDetailView PersonDetail { get; private set; }
+    public ManagementView Management { get; private set; }
     public CharacterGrowthUI GrowthUI => FindObjectOfType<CharacterGrowthUI>(true);
-    public bool IsPersonModalOpen => Router.Surface==HomeSurface.Person || GrowthUI?.IsOpen==true;
+    /// <summary>Surface 模态（人物 / 管理 / 成长）统一复用既有 World 输入阻断，不新建第二套 Guard。</summary>
+    public bool SurfaceModalActive => Router.Surface==HomeSurface.Person || Router.Surface==HomeSurface.Management || GrowthUI?.IsOpen==true;
+    public bool IsPersonModalOpen => SurfaceModalActive;
```

- 说明：仓库中**不存在** `SurfaceModalActive` 成员，既有 Guard 名为 `IsPersonModalOpen`（被 `ProductHomeWorldHost` 使用）。本包新增 `SurfaceModalActive` 作为**唯一判定**，`IsPersonModalOpen` 保留为同义别名，因此 `ProductHomeWorldHost`（World 点击/镜头输入/`PointerOverUI` 早退）**无需改动**即覆盖 Management。
- 未新增第二份 `SelectedPerson`、未改 `HomeSelectionContext`、未新增领域规则。

### 2.2 加载唯一正式 Management Prefab（第 72–81 行，`Initialize` 内、`Build` 之后、PersonDetail 之前）

```diff
         Build=gameObject.AddComponent<HomeBuildingPlacement>();Build.Initialize(host);
+        var authoredManagement=Resources.Load<ManagementView>("UI/ManagementView");
+        if(authoredManagement==null) Debug.LogError("[Management] Missing authored UI/ManagementView prefab.");
+        else
+        {
+            Management=Instantiate(authoredManagement,transform);Management.name="ManagementView";
+            var managementRect=Management.GetComponent<RectTransform>();managementRect.localScale=Vector3.one;managementRect.anchorMin=Vector2.zero;managementRect.anchorMax=Vector2.one;managementRect.sizeDelta=Vector2.zero;managementRect.anchoredPosition=Vector2.zero;
+            Management.Initialize(host,this);
+            Management.PersonDetailRequested=id=>{ if(string.IsNullOrEmpty(id))return; host.Selection.SelectPerson(id); OpenSurface(HomeSurface.Person); };
+        }
         var authored=Resources.Load<PersonDetailView>("UI/PersonDetailView");
```

- 资源路径固定为 `Resources.Load<ManagementView>("UI/ManagementView")`，与 `PersonDetailView` **同一 Shell Canvas、同一铺满方式**（`anchorMin=0/anchorMax=1/sizeDelta=0/anchoredPosition=0/localScale=1`）；**未新建 Canvas / GraphicRaycaster**。
- Prefab 缺失时只 `Debug.LogError` 明确错误并**不创建任何 runtime 固定 UI 替代**；且该分支在 PersonDetail 加载之前，Management 缺失不会连带阻断 PersonDetail 加载。
- `Management.Initialize(host,this)` 内部以 `SetActive(false)` 收尾，故实例化后默认隐藏，只在 Management Surface 激活。
- `PersonDetailRequested` 用**同一** `host.Selection.SelectPerson(id)`（`HomeSelectionContext.Select` 对同人幂等，不会重复触发 `Changed`），然后走现有 `OpenSurface(HomeSurface.Person)` 进入既有 PersonDetail；不新开路由。

### 2.3 `LateUpdate`：ShellVisible 为假时同步隐藏 Management（第 94 行）

```diff
         RetirePrototype();
-        if(!ShellVisible) return;
+        if(!ShellVisible) { if(Management!=null && Management.gameObject.activeSelf) Management.gameObject.SetActive(false); return; }
```

- 严格实现“只在 `Router.Surface==Management && ShellVisible` 激活”：Canvas 因 `WorldVisible && IsSimulationActive` 关闭时，Management 不再残留 active 状态。
- 未改 `_canvas.enabled` 计算式，未改 `SimulationActive`。

### 2.4 `RefreshNow`：可见性 / 刷新 / 模态 Guard / 白模退场（第 123–133 行）

```diff
-        _surface.SetActive(Router.Surface!=HomeSurface.World && Router.Surface!=HomeSurface.Person);
+        _surface.SetActive(Router.Surface==HomeSurface.Cards || Router.Surface==HomeSurface.History);
+        if(Management!=null)
+        {
+            bool show=Router.Surface==HomeSurface.Management && ShellVisible;
+            if(Management.gameObject.activeSelf!=show) Management.gameObject.SetActive(show);
+            if(show) Management.RefreshNow();
+        }
         if(PersonDetail!=null) { PersonDetail.gameObject.SetActive(Router.Surface==HomeSurface.Person && ShellVisible); if(PersonDetail.IsVisible) PersonDetail.RefreshNow(); }
-        SetTownInputBlocked(IsPersonModalOpen && ShellVisible);
-        _attentionButton.interactable=!IsPersonModalOpen;
-        if(IsPersonModalOpen) _attentionPanel.SetActive(false);
+        SetTownInputBlocked(SurfaceModalActive && ShellVisible);
+        _attentionButton.interactable=!SurfaceModalActive;
+        if(SurfaceModalActive) _attentionPanel.SetActive(false);
```

- Management 激活/隐藏只由 `Router.Surface` + `ShellVisible` 决定；激活当帧由 `OnEnable` 与 Shell 各刷新一次（`ManagementView.RefreshNow` 幂等，读同一只读快照），随后只跟随 Shell 既有 **0.2s** `_nextRead` 节流刷新，**不每帧读/克隆全列表**。
- 原“Management 白模”`_surface.SetActive` 改为**仅 Cards/History** 使用，Management 不再显示白模文案（白模 GameObject 本身保留给 Cards/History 与 Debug 对照，未删除他人代码）。
- 旧 Town 按钮：`SetTownInputBlocked(...)` 复用既有 `TownUI.canvasRoot` CanvasGroup 关闭 `interactable/blocksRaycasts`，并且 `RetirePrototype()` 已让 legacy 按钮在非“调试/对照”模式隐藏；**未新增 ACL/权限/层级操作**。

### 2.5 底部导航 Guard（第 164 行，`Navigate`）

```diff
     private void Navigate(int index)
     {
-        if(IsPersonModalOpen)return;
+        if(SurfaceModalActive)return;
```

- Management（及 Person/Growth）打开时，底部导航 村庄/人物/管理/卡牌/历史/远征/建造 全部在入口处被拒绝（含“建造”目录 `index==6`、远征 `index==5`），不依赖 sortingOrder。

### 2.6 调试对照与关注面板 Guard（第 177、180 行）

```diff
     public void ToggleCompatibility()
-    { if(IsPersonModalOpen)return;CompatibilityVisible=!CompatibilityVisible;_host.ToggleDebug();RetirePrototype(); }
+    { if(SurfaceModalActive)return;CompatibilityVisible=!CompatibilityVisible;_host.ToggleDebug();RetirePrototype(); }
     public void NavigateAttention(HomeAttentionItem item)
     {
-        if(IsPersonModalOpen || item==null || ...) return;
+        if(SurfaceModalActive || item==null || ...) return;
```

### 2.7 `RefreshSurface`：Management 不再写白模文案（第 187–201 行）

```diff
-        var snapshot=_host.Snapshot;if(snapshot==null || Router.Surface==HomeSurface.World) return;
+        var snapshot=_host.Snapshot;if(snapshot==null || Router.Surface==HomeSurface.World || Router.Surface==HomeSurface.Management) return;
...
-            case HomeSurface.Management:
-                _surfaceTitle.text="管理";_surfaceBody.text=note+"人口 / 职责与场所 / 资源\n\n"+snapshot.Persons.Count+" 位人物 · "+snapshot.Places.Count+" 个地点\n\n"+_resources.text+"\n\n集中调度页面待后续实现。";break;
             case HomeSurface.Cards:
```

- 删除的仅是 Management 白模标题/正文（临时文案），Person/Cards/History 分支与既有行为保留。

## 3. 明确未改的文件（哈希回读）

| 文件 | bytes | SHA256 | 说明 |
|---|---:|---|---|
| `.../ProductHome/ManagementView.cs` | 37518 | `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d` | 与 controller-report 一致，**未改** |
| `.../ProductHome/ManagementRowView.cs` | 1670 | `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd` | 未改 |
| `Assets/Resources/UI/ManagementView.prefab` | 282097 | `ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11` | 与 prefab-binding-report 一致，**未改** |
| `.../ProductHome/ProductHomeWorldHost.cs` | 14911 | `8e5dc3b1bd8a47b62f1a2bc184138af237d4be910162467efec08b4b20ceab16` | 未改（复用其 `IsPersonModalOpen` 早退） |
| `.../ProductHome/PersonDetailView.cs` | 6748 | `c3fc26aff4e6f65195cb83d44b974c3ea1084a93d9afc05ba7281f4a7247de4d` | 未改 |
| `.../ProductHome/HomeProductProjection.cs` | 4325 | `edfd514feca28066da3e1e10a829111f63318a6957a6d5871996f212c492ab68` | 未改（Router 语义不变） |

未改：Management 投影/命令/快照/读适配器、Growth/Inspector、VillageRuntimeBridge、Work/Clock/Activity、Save DTO、任何 Prefab/Scene/YAML/字体/ProjectSettings。

## 4. 宿主加载 / 可见性 / 选择 / 关闭 / ESC / Guard 路径（源码级结论，非运行验证）

### 4.1 加载与实例化
`ProductHomeWorldHost.Awake` → `HomeProductShell.Initialize(host)` → `Resources.Load<ManagementView>("UI/ManagementView")`（回读 `RES_LOAD=True TYPE=ManagementView`）→ `Instantiate(authored, transform)` → 与 `PersonDetailView` 相同铺满设置 → `Initialize(host,this)`（内部隐藏）→ 注入 `PersonDetailRequested`。Prefab 缺失只报错不留白模。

### 4.2 可见性
`Router.Surface==HomeSurface.Management && ShellVisible` 时 `SetActive(true)` + `RefreshNow()`；其它 Surface 或 Canvas 关闭时 `SetActive(false)`；刷新跟随 Shell 既有 0.2s 节流。隐藏时 `ManagementView` 保留 `Selection.Changed` 订阅但 `SelectionChanged` 在非激活时不刷新（`OnDestroy` 解绑），无每帧读取。

### 4.3 选择（唯一 Selection）
- 人口行点击 → `ManagementView.OpenPerson(id)` → `_host.Selection.SelectPerson(id)` → `PersonDetailRequested` → Shell `host.Selection.SelectPerson(id)`（同人幂等）→ `OpenSurface(HomeSurface.Person)` 进入**既有** PersonDetail。
- 无第二份 SelectedPerson；`SelectedPersonId` 每次派生自 `HomeSelectionContext`。岗位/施工点击只写页面会话候选 `_workplaceCandidate` / `_constructionCandidate`，不写 Selection。
- 关闭/ESC 走 `Router.Close()` → `Surface=World`，**Selection 不被清除**（`HomeProductRouter.Close` 只改 Surface）。因此“人物 → 人物详情 → 关闭/ESC → 世界 → 底部‘管理’”后，同一人物仍为当前选择，人口行高亮（`personKey` 含 `SelectedPersonId`），随后点岗位行即可派工 —— **这是本包核对过的“选人再派岗”可用路径**（详见 §6 的路径核对与成本说明）。

### 4.4 关闭与 ESC（同一 `CloseSurface`）
- Prefab `Close` 按钮 → `ManagementView.Close()` → `_shell.CloseSurface()`；`Shell.HandleEscape()` →（`Build.IsOpen==false`）→ `CloseSurface()`。两者同一入口：先处理 Growth（`growth.IsOpen && !TryHide()` 则直接 return，不下发关闭）→ `Router.Close()` → `RefreshNow()`；嵌套 Growth pending 不会被绕过。
- 未新增/未改写 Pause、Speed、`SimulationActive`；导航关闭不会恢复手动 Pause（`CloseSurface` 不触碰 Clock）。

### 4.5 World 输入 Guard（全部复用既有，不靠 sortingOrder）
- 世界点击：`ProductHomeWorldHost.Update` 的 `overUI=Shell.IsPersonModalOpen || PointerOverUI(...)` 现含 Management；`HandleWorldClick` 内另有 `Shell.IsPersonModalOpen` 早退 → 世界点选被阻断。
- 镜头拖拽/滚轮：同一 `overUI` 传入 `HomeWorldCamera.ReadInput(overUI, ...)`，`overUI==true` 时不做 Pan/Zoom。
- 建造目录/远征：底部导航 `Navigate` 被 `SurfaceModalActive` 拦下；`Shell.Build` 未打开。
- 旧 Town 按钮：`SetTownInputBlocked(true)` 关闭 `TownUI.canvasRoot` 的 CanvasGroup 交互与射线，另有 legacy 隐藏逻辑。
- 关注面板：模态时 `_attentionPanel.SetActive(false)` 且 Attention 按钮 `interactable=false`（既有行为）。
- TopHUD：`_pause` / `_speed` 未加任何 interactable 限制，保持可用；TopHUD 未被隐藏、未重建。

## 5. 编译 / Console / 类型 / Prefab 几何回读（实际回执）

1. `editor | call RefreshAssets()` 执行两次（最终源码之后一次），`isCompiling` 由 `true` → `false`，`isUpdating=false`。
2. `Library/ScriptAssemblies/Assembly-CSharp.dll` 写入 `2026-10-23T12:15:33.8098860Z`，`HomeProductShell.cs` 写入 `2026-10-23T12:15:31.2528418Z` → `FRESH=True`。
3. 类型/成员存在性（`Assembly.LoadFrom(Assembly-CSharp.dll)` 回读）：`HomeProductShell=True`、`SurfaceModalActive=True`、`Management` 属性=True、`ManagementView` 类型在 `Assembly-CSharp` 中=True、`ManagementView.Initialize` 存在=True。
4. Console（`console | where logType == "Error" or logType == "Exception"`）**本包未新增任何错误/异常**：`sequence > 17202` 的 Error/Exception 查询为 **0 条**。历史错误全部 ≤17202，属上一份 `prefab-binding-report.md` §10 已记录的**执行器自身脚本片段/资产句柄**问题（`snippet.cs CS1503`、`Destroying assets is not permitted`），不是项目源码编译错误。本包未清空 Console。
5. 新增观察（如实记录、非本包源码错误）：`sequence > 17202` 有 998 条 `Warning`，均为既有 TMP **LiberationSans SDF 缺字**警告（文本对象如 `InfoText`/`Label`，属旧 Town/占位文本；本包 Management Prefab 使用 `RRPG_SourceHanSansCN_Regular_TMP`），`message contains "Management"` 的日志 **0 条**。
6. Prefab 只读回读（`Resources.Load` 资产本身，不实例化、不入场景）：
   - `ROOT` anchors `(0,0)..(1,1)`、`sizeDelta=(0,0)`、`canvas=0`、`raycaster=0`、`CanvasGroup=True`（不建第二 Canvas）。
   - `ModalBlocker`：`anchorMin=(0,0)`、`anchorMax=(1,1)`、`offsetMin=(0,0)`、`offsetMax=(0,-88)`、`raycast=True`、默认 active → 顶部 88px 让位；既有 TopHUD 位于屏幕顶 16..78px，标题/面板顶边 `offsetMax.y=-96`，**Blocker 与面板均不覆盖 HUD**。
   - `Templates` 默认 inactive；四个行模板默认 inactive；`SERIALIZED_OBJREFS=39 NULL=0`（35 字段、无缺失引用）。
7. 模板实例化注意点（源码核对）：`ManagementView.Spawn` 先 `Instantiate(template, parent)` 再显式 `row.gameObject.SetActive(true)`，即使模板与 `Templates` 父节点为 inactive，克隆到 active 列表容器后 `activeSelf=true`，不会出现“行生成但不可见”。（运行期未验证。）

## 6. 人口管理“选人 → 派岗”路径核对（按要求特别核对）

Prefab 人口行整行点击绑定 `() => OpenPerson(personId)`，即**点击人物行会立即进入 Person Surface 详情**（这是规格要求的“点人物进入现有 Person Surface”）。本包不新增第二按钮、不改 Prefab、不新增规则，因此可用路径为：

1. 世界点选人物 →（底部“管理”）→ Management，`Selection` 已是该人物 → 点“职责与场所”岗位行 → `派工` 可用（`assignButton.interactable = canWrite && personReady && workplace.CanAssign`）。**这是最短可用路径，未受本包改动影响。**
2. 或在管理内点人口行 → 进入既有人物详情（同一 Selection）→ 关闭/ESC → `CloseSurface` 回 World（Selection 保留）→ 底部“管理” → 同一人物仍选中（行高亮、详情面板显示该人物）→ 选岗位行 → 派工。

- 结论：**不存在“无法选人再派岗”的死路**；关闭/ESC 后底部导航恢复可点（`SurfaceModalActive=false`），管理重入后 `LocateSelectionIfNeeded` 对 `Person` 选择不做页签跳转，仅保留选择。
- 成本如实说明：路径 2 需要“关闭回世界再进管理”两步；若后续产品希望“管理内点人物只选中不离页”，需新增窄交互（例如行内选择按钮或详情面板“查看详情”按钮），必须再改 Prefab（Editor）与 `ManagementView`，**本包未做**，以免擅自新增规则/第二 Selection 语义。

## 7. 运行时尚未验证清单（明确不声称通过）

以下全部**未**在本包验证（无 Play、无 tests-run、无真实输入注入）：

1. Prefab 实例化进入 Shell 后的实际渲染、层级、`ModalBlocker` 遮挡效果与 1920×1080 / 1280×720 缩放表现。
2. 世界点击 / 右键拖拽 / 滚轮 / 建造目录 / 底部导航 / 旧 Town 按钮在 Management 打开时是否**真的**被拦（仅源码 Guard 路径核对）。
3. TopHUD 暂停/速度按钮在 Blocker 88px 洞口内的真实可点性与视觉不重叠。
4. ESC 与关闭按钮一致性、嵌套 Growth pending 不被绕过、远征 Return/忙/待处理事件的既有优先级是否被解锁。
5. 行克隆在 inactive 模板 + active 容器下的真实可见性、滚动定位、筛选/检索、页签切换。
6. 派工/撤职/施工加入撤出/保存按钮的真实 Gateway 结果、`CanSaveWorld` 忙时禁用、手动 Pause 不被导航恢复。
7. 动态字体运行时补字（`›`、`供`、`兼` 等 14 字）是否正常显示。
8. Management 打开期间 0.2s 刷新对帧率/GC 的真实影响。

## 8. 下一包（真实 Play 专项）所需现有基座与测试类建议

已具备的基座（本包与前置包已确认存在，无需新建）：

- 入口：`ProductHomeWorldHost`（`Awake` 内建 Camera/Inspector/Shell）、`HomeProductShell.Initialize` 的 `Resources.Load` 路径、`HomeProductRouter`（`Open/Close`）、`HomeSelectionContext`（`SelectPerson`/`Changed`）。
- 构件：`Assets/Resources/UI/ManagementView.prefab`（39 引用无 null、Blocker 顶留 88）、`Assets/Resources/UI/PersonDetailView.prefab`、`ManagementView`（35 字段、`Initialize/RefreshNow/Close/PersonDetailRequested`）、`ManagementReadAdapter` / `ManagementCommandGateway` / `ManagementSnapshot`。
- 测试：`ManagementProjectionTests`（前置 13/13 Passed）提供真实宿主与快照读取的既有测试夹具；Play 专项应复用它已有的宿主构造，而不是新建完整世界。

建议新增/使用的测试类（下一包，不在本包创建）：

1. `ManagementShellPlayTests`（PlayMode，`[UnityTest]`）：
   - `OpenManagement_ActivatesAuthoredPrefab_AndHidesWhitebox`：断言 `Shell.Management != null`、`Router.Surface==Management` 时 `IsVisible`、白模 `HomeInternalWhitebox` inactive。
   - `CloseAndEscape_ReturnWorld_AndKeepSelection`：`host.Selection.SelectPerson(id)` → 开管理 → `Close()`/`HandleEscape()` → `Surface==World` 且 `Selection.Id` 不变。
   - `ManagementOpen_BlocksWorldClickAndBottomNav`：模态时调用 `host.HandleWorldClick(screen)` 与 `Navigate` 后 Selection/Surface 不变。
   - `PersonRowClick_EntersPersonSurface_AndSelectionPersistsOnReturn`：模拟 `OpenPerson(id)` → `Surface==Person`；`CloseSurface()` 后 `SelectPerson` 仍为该 id；再 `OpenSurface(Management)` 后 `SelectedPersonId==id`。
   - `AssignAfterReturn_UsesSameSelection`：返回管理后选岗位行 → `AssignSelectedPersonToWorkplace()`，断言 Gateway 结果与快照一致（父级真实宿主）。
   - `TopHudPauseSpeed_RemainUsable_WhileManagementOpen`：点击 `_pause`/`_speed` 走 `TogglePause`/`CycleSpeed`，断言 Clock 状态变化且管理仍打开。
2. `ManagementModalInputPlayTests`（PlayMode）：相机 `orthographicSize`/位置在模态时滚轮与右键拖拽后不变；建造目录不打开。
3. 复用现有 `ManagementProjectionTests`（EditMode）作为回归基线，本包未运行。

建议 Play 专项执行顺序：先 Management 专项 → 再串行完整回归；不以本报告替代。

## 9. 保护核验（34 项基线，收工回读）

- 基线：`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，固定 **34** 项，`head=054b00d3e38fe2e2e142bfdb2502de90f7991030`。
- 收工重算：`total=34 same=34 diff=0 missing=0`（逐项 bytes + SHA256 一致）；未覆盖 HEAD、未恢复旧备份、未改 `Assets/Editor/StudioRecoveryBackups/2026-10-23T11-49-56-028Z` 及 `.meta`（`DIR_EXISTS=True / META_EXISTS=True`）。
- 制作人 **14 项**（TMP 字体 1 + Growth 12 + ProjectSettings 1）属上述 34 项子集，随 34/34 same 一并确认；NuGet/Packages 属 34 项基线但不属 14 项（沿用 controller-report 末尾更正）。
- 本包只编辑 `HomeProductShell.cs`（不在 34 项基线内），未触碰任何受保护资产/设置；**未做改动前基线读取**（沿用上一包 34/34 回执），本包只提供改动后 34/34 一致证据，如实说明。

## 10. 边界与 Artifact 声明

- 本包**未**：重做投影/命令/Prefab/控制器、重建 UI、改 Scene、跑 tests-run、跑 Play、跑完整回归、commit/push/stage、安装组件、改权限、读写用户档。
- 本包**未**声称模态拦截、TopHUD 可点、ESC 行为、行渲染、字体补字或任何交互在运行时真实通过；以上均为源码级接线与静态资产回读。
- 报告文件存在即满足本包 artifact 门禁；门禁只证明报告存在，**不产品 PASS**，后续由 Studio 按阶段验证门禁继续，再由主开发专业核验。
