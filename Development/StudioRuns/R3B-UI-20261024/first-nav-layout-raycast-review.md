# R3-B Pilot｜首次激活管理页「回人口页签」Raycast 0 命中的同帧 Canvas 采样诊断与最窄修复

生成时间：2026-10-24（本机）
阶段：1/1（核对首次激活 Canvas 布局与 Raycast）
状态：**已定位根因（同帧激活后 Canvas 尚未批量处理 Graphic → `Graphic.depth == -1` → `GraphicRaycaster` 跳过）；仅做最窄测试侧修复 + 证据增强，产品源码未改；仅用 `AssetDatabase.Refresh()` 编译，工程编译错误 0；PlayMode 由宿主以短类名 `ManagementShellPlayTests` 整类串行执行，本执行层未运行、不宣称通过。**

---

## 0. 范围声明

- 实际改动 2 个文件：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅用例 10 的页签点击前同帧 Canvas 刷新 + 诊断证据行 + 类内 XML 说明）
  2. `Development/StudioRuns/R3B-UI-20261024/first-nav-layout-raycast-review.md`（本文件）
- **未改任何产品源码**（`ManagementView.cs` / `HomeProductShell.cs` / `ProductHomeWorldHost.cs` / `HomeProductRouter.cs` / Prefab / Scene 均未改）：复核后确认产品侧 `active` / `CanvasGroup` / 页签 `SetActive` 接线正确，失败来自**测试在同一帧内对刚激活的 UI 采样 Raycast**。
- 禁止项复核：未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未改动 `R3B-SaveProtection-20261024/*` 备份/manifest/历史档；未改 34 项基线、受保护 TMP 字体；未改 Authority / API / DTO / WorkClock / 规则；未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent；未启动 Test Runner、未忙等模型测试。

---

## 1. 本轮先决状态（只读实测，全部通过后才动文件）

| 项目 | 实际值 / 证据（本次实测） |
|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`；`selection=[]` |
| 场景干净 | 唯一打开场景 `Assets/Scenes/SampleScene.scene`，`isDirty=false`、`isLoaded=true`、`rootCount=5` |
| 34 项基线（改动前） | 按 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 逐项重算字节 + SHA256：`entries=34 problems=0 all-match` |
| 受保护字体（改动前） | `RRPG_SourceHanSansCN_Regular_TMP.asset`：`bytes=14961033`、`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致） |
| Save 保护历史档 | `R3B-SaveProtection-20261024/*` 共 **85** 个 run，`status` 非 `restored/protective-snapshot` 的未完成 run = **0**；最新 10 个 run（`20261024T115809Z` … `20261024T115818Z`，含 `-2` 序号）全部 `restored` |
| 测试发现 | `tests | where fullName contains "ManagementShellPlayTests"` → **10 例**、`mode=PlayMode`、`assembly=BattleStage1.PlayModeTests` |

> 失败回执 `edf56b77`（真实 10 例：9 过 1 败）属 Studio 私有运行记录，本执行层未读取；本节与第 2 节只使用**仓库内可见的真实证据**（Play 证据日志最后一轮 `2026-10-24T11:58` + Editor Console 同一轮运行日志）来定位。

---

## 2. 失败点的真实证据链（先定位“到底哪一条断言失败”）

### 2.1 用例 10 的实际执行痕迹

`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`（最后一轮，`2026-10-24T11:58`）只有：

```
[FIRST-NAV] periodic-reset surface=Cards→World shellSourceConsumed=True managementHidden=True shellVisible=True simulationActive=True
[FIRST-NAV] pre-click shellSourceIsLoaded=False bridgeSourceIsLoaded=True surface=World clock=0 staleCandidate=ws_farm_01 staleSearch=旧世界_QA_5c1a
```

Editor Console 同一轮（sequence 44123–44133）里，用例 10 的点击日志只到：

```
44132 Log [ManagementShellPlayTests] 程序化 EventSystem 点击（非物理鼠标人工体验）：首次导航回归·来源替换后第一次真实点击
44133 Log [ManagementShellPlayTests] SAVE-RESTORE runId=20261024T115813Z … matches-before=True   ← TearDown
```

`RealPointerClick` 的“程序化 EventSystem 点击”日志写在**断言全部通过之后**。因此：

- 第一次真实导航点击（`ProductNavigation/Navigate2`）**成功**（日志在 44132）；
- 之后**没有任何**“首次导航·回到人口页签”的点击日志；
- 也没有 `same-frame rows visible=…` / `first-click-committed` 证据行。

结论：失败发生在 `ClickChild(management, "ModalPanel/Tab_Population", "首次导航·回到人口页签")` →
`RealPointerClick` → `AssertTopHitBelongs` 的 `Assert.Greater(hits.Count, 0, what + "：Raycast 无命中 @ " + point)`，
即任务回执里的“回到人口页签 Raycast 0 @ (158, 857)”；其后的 `activeRows/pendingRows/下一帧销毁` 生命周期断言**从未执行**（“生命周期断言未到”）。失败点不是行 Raycast，也不是业务行计数。

### 2.2 0 命中的那一点就是页签的真实 authored 几何（排除“错对象 / 被 hide / 布局错位”）

由 authored Prefab `Assets/Resources/UI/ManagementView.prefab` 的实测数据：

| 节点 | 关键 authored 数据 | 位置推算（Canvas 缩放 = 1） |
|---|---|---|
| `ModalPanel`（Prefab 第 4602–4631 行） | `anchorMin(0,0)`、`anchorMax(1,1)`、`sizeDelta(-48,-184)`、`pivot(0.5,0.5)` | 左边距 24、上边距 92 |
| `Tab_Population`（第 8607–8626 行） | `anchorMin/Max(0,1)`、`anchoredPosition(24,-104)`、`sizeDelta(220,46)`、`pivot(0,1)` | 左 = 24+24 = 48；中心 x = 48+110 = **158**；中心距屏顶 = 92+104+23 = 219 |

- 实测取样点 `(158, 857)`：**x 与 authored 中心 158 完全一致**；y = 屏高 − 219 → 屏高约 1076（Play 窗口像素高），CanvasScaler 参考分辨率 1920×1080 → 缩放≈1。
- 即：取样点正是页签中心，`rect` 没有错位，页签没有被移到别处，也不是点到了别的对象。
- `Tab_Population` 的 `Image.m_RaycastTarget: 1`、`Button.m_Interactable: 1`；`ModalPanel` 背景 `Image.m_RaycastTarget: 1` 且覆盖全屏（左边距 24、上下各 92）。
  **如果这些 Graphic 参与了本帧 Raycast，取样点不可能 0 命中（至少会命中 `ModalPanel` 背景）。**

### 2.3 0 命中的机制：GraphicRaycaster 跳过“本帧尚未被 Canvas 处理”的 Graphic

仓库内真实 uGUI 源码（`Library/PackageCache/com.unity.ugui@1.0.0/`）：

- `Runtime/UI/Core/GraphicRaycaster.cs:355`：`if (!graphic.raycastTarget || graphic.canvasRenderer.cull || graphic.depth == -1) continue;`，注释明确写着 *“-1 means it hasn't been processed by the canvas, which means it isn't actually drawn”*；
- `Runtime/UI/Core/Graphic.cs:387`：`public int depth { get { return canvasRenderer.absoluteDepth; } }`。

本帧内真实发生产生的次序（`HomeProductShell.OpenSurface` → `SyncSourceReset` → `Router.Close()` → `Router.Open(Management)` → `RefreshNow()` → `Management.gameObject.SetActive(true)` → `ManagementView.OnEnable → RefreshNow`）（代码位置：`HomeProductShell.cs:107-155`、`ManagementView.cs:151-188`）：

1. 第一次真实点击同帧先把管理页 `SetActive(false)` 路径的 `Router.Close()` 走完；
2. 紧接着 `Router.Open(Management)` + `RefreshNow()` 把管理页 `SetActive(true)`；
3. 管理页全部 Graphic（`ModalPanel` 背景、三个页签、人口/岗位/资源页）都是**本帧新激活**，
   `CanvasRenderer.absoluteDepth` 尚未被 Canvas 批量处理 → `Graphic.depth == -1`（多数同时 `cull == true`）；
4. 该帧内再次 `EventSystem.RaycastAll` → `GraphicRaycaster` 把这批 Graphic 全部跳过 → **hit 数 = 0**；
5. 该帧稍后（渲染前的 `willRenderCanvases`）Canvas 才处理它们，真实玩家下一帧点击必然正常。

“同帧”是本次新引入的：上一版用例在页签点击前让出过一帧（因此当时同一点击能命中，失败发生在之后的 `childCount` 断言），
本轮 `row-rebuild-lifecycle-review.md` 把生命周期语义改成**同帧**，页签点击就落在管理页刚激活的同一帧里。

### 2.4 逐项排除的其它候选（都用同一轮真实证据排除）

| 候选原因 | 排除依据 |
|---|---|
| 来源复位吞掉首次导航 | 首次点击后 `Surface=="Management"`、`Management.IsVisible`、`SurfaceModalActive`、`ShellVisible`、`SimulationActive`、Clock 不推进、`shell._source` / `management._source == secondLoaded`、旧候选/检索已清、人口投影逐 PersonId 等价 —— 这些断言在失败点之前**全部通过**（否则会先在那里失败） |
| 错页签对象 / 页签被 hide / 布局错位 | 取样点 `(158,857)` 与 authored 中心逐像素吻合；`ClickChild` 之前 `FindPath`/`Button.interactable` 均通过；失败消息是“Raycast 无命中”，不是“子控件不存在 / 不可交互” |
| 被别的对象遮挡 | 遮挡只会产生“命中别的东西”，不会产生 0 命中；0 命中说明**本 Canvas 的所有 Graphic 都被跳过**（含全屏 `ModalPanel` 背景） |
| 产品 `CanvasGroup.blocksRaycasts` / `Town` 输入阻断接线错误 | `SetTownInputBlocked` 只作用于 `TownUI.canvasRoot` 的 `CanvasGroup`（`HomeProductShell.cs:163-169`），不作用于 Shell 自己的 `Canvas`；且本用例在同帧里由 `_townInput` 管线执行前后都未改 Shell Canvas |
| Shell Canvas 被关掉 | `ShellVisible`（= `_canvas.enabled`）在失败点之前已断言为 `true`；`LateUpdate` 的 `_canvas.enabled = _host.WorldVisible && IsSimulationActive` 本轮未被触发改变 |
| 布局（`populationContent`/行）尚未重算 | 该断言发生在 `populationContent` 采样之前；页签是固定 authored Rect，不依赖任何 LayoutGroup |

---

## 3. 最窄修复（测试侧，符合阶段授权）

在“首次真实导航已提交且明确断言 `Surface==Management`、Shell/Management 来源已消费”之后、页签真实点击之前，插入：

1. 只读诊断采样（页签 Rect 点位、刷新前命中数、刷新前 `Graphic.depth`、`canvasRenderer.cull`、`activeSelf/activeInHierarchy`、`interactable`、`raycastTarget`、`ModalPanel` 的 active/depth、管理页 active、`Canvas.enabled/activeInHierarchy`、`populationPage.activeSelf`、`ActiveTab`）；
2. **同帧**标准 `Canvas.ForceUpdateCanvases()`（uGUI 文档明确：它让 Canvas 在“调用时”而不是帧末更新：`Library/PackageCache/com.unity.ugui@1.0.0/Documentation~/class-RectTransform.md:28`；[Unity 官方 API](https://docs.unity3d.com/cn/2022.3/ScriptReference/Canvas.ForceUpdateCanvases.html)）；
3. 刷新后再次只读 Raycast 取命中数写入证据行；
4. 之后仍然走原来的真实 `ClickChild`（真实 `RaycastAll` + `ExecuteEvents.PointerClick`），断言一字未放宽。

精确 diff（`ManagementShellPlayTests.cs`，用例 10，原第 3075-3077 行位置）：

```csharp
+        var navigationTab = (RectTransform)FindPath(((Component)management).transform, "ModalPanel/Tab_Population");
+        Assert.IsNotNull(navigationTab, "首次导航·回到人口页签：Tab_Population 必须存在");
+        var navigationTabGraphic = navigationTab.GetComponent<Graphic>();
+        var navigationTabButton = navigationTab.GetComponent<Button>();
+        var navigationTabCanvas = navigationTab.GetComponentInParent<Canvas>();
+        var navigationPanel = (RectTransform)FindPath(((Component)management).transform, "ModalPanel");
+        var navigationPanelGraphic = navigationPanel != null ? navigationPanel.GetComponent<Graphic>() : null;
+        Vector2 navigationTabPoint = ScreenCenter(navigationTab);
+        int tabHitsBeforeRefresh = RaycastAllAt(navigationTabPoint).Count;
+        int tabDepthBeforeRefresh = navigationTabGraphic != null ? navigationTabGraphic.depth : int.MinValue;
+        Canvas.ForceUpdateCanvases();
+        int tabDepthAfterRefresh = navigationTabGraphic != null ? navigationTabGraphic.depth : int.MinValue;
+        Evidence("FIRST-NAV", "same-frame-tab-sample surface=" + Surface()
+            + " point=" + navigationTabPoint
+            + " raycastBeforeRefresh=" + tabHitsBeforeRefresh
+            + " raycastAfterRefresh=" + RaycastAllAt(navigationTabPoint).Count
+            + " tabActiveSelf=" + ...
+            + " tabInHierarchy=" + ...
+            + " tabInteractable=" + ...
+            + " tabRaycastTarget=" + ...
+            + " tabDepthBeforeRefresh=" + tabDepthBeforeRefresh + " tabDepthAfterRefresh=" + tabDepthAfterRefresh
+            + " tabCull=" + ...
+            + " panelActiveSelf=" + ... + " panelInHierarchy=" + ... + " panelDepth=" + ...
+            + " managementActiveSelf=" + ... + " managementInHierarchy=" + ...
+            + " canvasEnabled=" + ... + " canvasActive=" + ...
+            + " populationPageSelf=" + ... + " activeTab=" + Get(management, "ActiveTab"));
         ClickChild(management, "ModalPanel/Tab_Population", "首次导航·回到人口页签");
```

同时用例 10 的 XML 说明新增一条，记录“同帧激活后 Graphic 尚未被 Canvas 处理 → `absoluteDepth==-1` → Raycast 0”的前提与“只在首次点击已提交后同帧刷新几何”的边界。

### 3.1 为什么不是产品缺陷、因此不改产品代码

- 真实玩家点击“管理”后，管理页的激活发生在**输入事件分发内部**；第二次点击必然发生在之后的帧，此时 Canvas 已处理 Graphic，Raycast 正常。同帧第二次程序化点击只是测试为了证明“竞态不被复位吞掉”而人为压缩到一帧。
- 产品 `active` 接线已逐项核对：`ManagementView.IsVisible => gameObject.activeSelf`；`ApplyTabs()` 用 `populationPage/workplacePage/resourcePage.SetActive(activeTab…)`；`HomeProductShell.RefreshNow()` 用 `Router.Surface==Management && ShellVisible` 决定 `SetActive`；`SurfaceModalActive` 决定 `SetTownInputBlocked`；Prefab 里页签/背景 `raycastTarget=1`、`Button.interactable=1`、无任何 `CanvasGroup.blocksRaycasts=false`。
- 因此**没有任何“最窄产品修正”的必要确证**，本轮不动产品代码。

### 3.2 明确没有做的事（保持原语义）

- 没有 `Invoke` 页签、没有强置 `interactable`、没有 `SetActive` 管理页求绿（页签仍然必须由真实 `RaycastAll + PointerClick` 触发）；
- 没有删除/放宽 Raycast 与生命周期断言（`activeRows`/`pendingRows`/下一帧物理销毁/`childCount` 结算全部保留原强度）；
- 没有把首次真实导航放到 `yield` 之后（首次导航仍是同帧、无刷新、无等待、无私有写）；
- 刷新只调用标准 `Canvas.ForceUpdateCanvases()`：不触碰产品 `source`/`Router`/`Selection`/候选，不跨帧；
- 保留：首次点击前“Shell 旧源 + Bridge 新源”的只读前置、同帧 active 业务行集合 / 旧 inactive 行、下一帧物理销毁、Selection 候选清理、原 9 例强断言、`SaveSlotGuard`、NewGame、Shell 首次导航修复。

---

## 4. 证据设计（宿主运行后可区分源同步 / 几何更新 / 业务行生命周期）

| 证据 | 位置 | 可区分的事实 |
|---|---|---|
| `FIRST-NAV periodic-reset …` / `pre-click shellSourceIsLoaded=False bridgeSourceIsLoaded=True surface=World staleCandidate=ws_farm_01` | 已存在（未改） | **来源同步**：首次点击前 Shell 仍持旧源、Bridge 已是新源、管理页未提前打开 |
| 首次点击后的 `Surface/SurfaceModalActive/ShellVisible/SimulationActive/Clock/_source/候选/检索/人口投影` 断言 | 已存在（未改） | **导航提交 + 新源消费**（同帧，无 yield） |
| 新增 `FIRST-NAV same-frame-tab-sample …` | 本次新增 | **几何/Canvas 采样**：`raycastBeforeRefresh`、`raycastAfterRefresh`、`tabDepthBeforeRefresh/After`、`tabCull`、`panelDepth`、`activeSelf/inHierarchy/interactable/raycastTarget`、`canvasEnabled/Active`、`populationPageSelf`、`activeTab` |
| 既有 `FIRST-NAV same-frame rows visible=… pendingDestroyInactive=…` + `第一行 Raycast` | 已存在（未改） | **业务行生命周期**：可见行只来自新源、旧行 inactive 待销毁、第一行真实可点 |
| `FIRST-NAV first-click-committed …` + `SAVE-RESTORE matches-before=True` + `BASELINE/FONT total=34` | 已存在（未改） | 收尾完整性与存档/基线保护 |

判读规则（宿主复跑时）：

- `raycastBeforeRefresh > 0` → 本次“同帧未处理”假设不成立，需按命中链重新定位（但此修复不会掩盖失败，因为随后仍是真实点击 + 原断言）；
- `raycastBeforeRefresh == 0` 且 `tabDepthBeforeRefresh == -1` 且 `raycastAfterRefresh > 0` → 直接证实本轮根因与修复；
- `raycastAfterRefresh == 0` 且 `tabDepthAfterRefresh == -1` → `Canvas.ForceUpdateCanvases()` 在该 Unity 版本未刷新 `absoluteDepth`，需另立最窄方案（本文件不声称已验证通过）。

---

## 5. 改动后复核（本次实测）

| 项目 | 实际结果 |
|---|---|
| 编译 | 仅 `script-execute → AssetDatabase.Refresh(ForceSynchronousImport)`；随后 `isCompiling=false`、`isUpdating=false`、`isPlaying=false` |
| 工程编译错误 | `console | where logType=="Error" and (message starts-with "Assets/" or message contains "ManagementShellPlayTests.cs(")` → **0 条**；错误/异常总表中仅 3 条**工具侧** snippet 编译报错（本层只读诊断片段把 `UnityTestAttribute` 写进 `NUnit.Framework` 命名空间所致，随后已用正确命名空间复核通过），与工程资产编译无关 |
| 测试装配 | 反射加载 `RRPG.ManagementShell.Tests.ManagementShellPlayTests`：`testTypeLoaded=True`、`playTests=10`（`tests` 源同样 10 例、`PlayMode`、`BattleStage1.PlayModeTests`） |
| 34 项基线（改动后） | `entries=34 problems=0 all-match` |
| 受保护字体（改动后） | `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（未变） |
| 测试文件 | `ManagementShellPlayTests.cs`：`bytes=240743`、`sha256=6d7f199521de32078541d4b86c149918a52fc9542916196897868575b32d9776` |
| 场景 | 仍唯一 `SampleScene`、`isDirty=false`、`rootCount=5` |
| 新增资产 | 无（只改 1 个已存在 `.cs` + 本文件；未产生 Prefab/Scene/meta/资源） |

---

## 6. 根因结论（一句话）

失败不是来源复位、不是错页签、不是被隐藏、不是产品 `CanvasGroup/active` 接线错误，而是
**管理页在当前帧内被 `Router.Close → Router.Open` 连续关/激活后，测试在同一个帧里就用真实 Raycast 采样页签**；
此时这些 Graphic 还没被 Canvas 批量处理（`Graphic.depth == canvasRenderer.absoluteDepth == -1`、`cull == true`），
uGUI 的 `GraphicRaycaster` 按设计跳过它们，因而 `RaycastAll` 返回 0。修复只把“已提交后的页签采样”放到同帧一次标准
`Canvas.ForceUpdateCanvases()` 之后，并留下可区分源同步 / 几何更新 / 行生命周期的证据行。

---

## 7. 未测 / 交给宿主

1. **PlayMode 未由本执行层运行**：10 例通过/失败、`same-frame-tab-sample` 实际数值、`BASELINE-PRE/POST`、`FONT-PRE/POST unchanged=True`、`SAVE-RESTORE matches-before=True` 均以宿主以短类名 `ManagementShellPlayTests` 整类串行执行为准；本文件不宣称通过。
2. **`Canvas.ForceUpdateCanvases()` 是否能立即刷新 `absoluteDepth` 未在本层实测**：依据是 uGUI 自带文档（“force Canvas to be updated not at the end of the frame, but when that method is called”）与仓库内 `GraphicRaycaster/Graphic` 源码判据；若宿主复跑仍 0 命中且 `tabDepthAfterRefresh == -1`，则需改用其它最窄方案（例如先断言页签几何与 active 事实，再在下一帧前把页签点击作为“已提交后”的独立步骤），并按新证据重写本文件结论。
3. **`SetTownInputBlocked` 对 `TownUI.canvasRoot` 的影响未做物理输入验证**：本轮只按代码路径排除（它不改 Shell 自己的 `Canvas`/`CanvasGroup`），物理鼠标人工体验仍不在本包范围。
4. **`edf56b77` 完整 NUnit 栈未在仓库内复核**（Studio 私有记录）：本层用同一轮 Console 序列（44132 之后无页签点击日志、直接进 TearDown）与证据日志缺行来唯一确定失败点为页签 Raycast。
5. **其它来源切换入口**：`Navigate(5)`（远征）/`Navigate(6)`（建造）不提交 Surface，未被本竞态覆盖；本包按最小范围未改。
