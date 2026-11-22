# R3-B Pilot｜首次导航「同帧同步」与「下一渲染帧交互」的语义分离（用例 10 有界修复）

生成时间：2026-10-24（本机）
阶段：1/1（区分首次导航同步与下一渲染帧交互）
状态：**已按真实诊断重排用例 10：首次导航点击仍严格同帧（无 RefreshNow / 无 yield），但同帧不再点击页签、也不再调用已被证伪的 `Canvas.ForceUpdateCanvases()` 补丁；页签点击改为在**下一真实渲染帧**用真实 Raycast + PointerClick 触发，命中与行身份在随后渲染帧上强断言。仅改测试与本文件；产品源码 / Prefab / Scene 未改。编译只用 `AssetDatabase.Refresh`；PlayMode 未由本执行层启动，交由宿主以短类名 `ManagementShellPlayTests` 整类串行 check，本文件不声称通过。**

---

## 0. 范围声明

- 实际改动 2 个文件：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅用例 10 的点击边界重排 + 旧代行夹具 + 证据行 + 类内 XML 说明）
  2. `Development/StudioRuns/R3B-UI-20261024/first-nav-render-frame-review.md`（本文件）
- 未改任何产品源码（`ManagementView.cs` / `HomeProductShell.cs` / `HomeProductRouter.cs` / `ProductHomeWorldHost.cs` / `VillageRuntimeBridge.cs` / Save DTO / WorkClock 均未改），未新增 / 修改 Prefab、Scene、meta 或资源。
- 禁止项复核：未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未改动 34 项基线、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份 / manifest / 历史档；未改「14 与 34」相关历史档；未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent；未启动 Test Runner、未忙等模型测试。

---

## 1. 本轮先决状态（只读实测，全部通过后才动文件）

| 项目 | 实际值 / 证据（本次实测） |
|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`；`selection=[]` |
| 场景干净 | 唯一打开场景 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`isLoaded=true`、`rootCount=5` |
| 34 项基线（改动前） | 按 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 逐项重算字节 + SHA256：`entries=34 problems=0 all-match` |
| 受保护字体（改动前） | `RRPG_SourceHanSansCN_Regular_TMP.asset`：`bytes=14961033`、`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致） |
| Save 保护历史档 | `R3B-SaveProtection-20261024/*/manifest.json` 共 **95** 个 run：94 个 `restored` + 1 个 `protective-snapshot`，**未完成 run = 0**；最新 10 个（`20261024T140228Z` … `20261024T140236Z`，含 `-2` 序号）全部 `restored` |
| 测试发现（改动前） | 反射加载 `RRPG.ManagementShell.Tests.ManagementShellPlayTests`：Play 测试 **10** 例（原 9 + 首次导航回归） |
| 测试文件（改动前） | `ManagementShellPlayTests.cs`：`bytes=240743`、`sha256=6d7f1995…b32d9776`（= 上一轮 ForceUpdate 补丁版本） |

---

## 2. 最新 FIRST-NAV 诊断尾段（原生文件工具读取大 log，摘录原始行）

日志：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`（1521 行，原生 `read`/`grep` 读取）。
最新一轮（`2026-10-24T14:02`，即 d8d69dfb 真实 10 总 9 过 1 败那一轮）用例 10 的原始行：

```
2026-10-24T14:02:32.0559388Z [FIRST-NAV] periodic-reset surface=Cards→World shellSourceConsumed=True managementHidden=True shellVisible=True simulationActive=True
2026-10-24T14:02:32.0579658Z [FIRST-NAV] pre-click shellSourceIsLoaded=False bridgeSourceIsLoaded=True surface=World clock=0 staleCandidate=ws_farm_01 staleSearch=旧世界_QA_5c1a
2026-10-24T14:02:32.0670512Z [FIRST-NAV] same-frame-tab-sample surface=Management point=(158.00, 857.00) raycastBeforeRefresh=0 raycastAfterRefresh=0 tabActiveSelf=True tabInHierarchy=True tabInteractable=True tabRaycastTarget=True tabDepthBeforeRefresh=-1 tabDepthAfterRefresh=-1 tabCull=False panelActiveSelf=True panelInHierarchy=True panelDepth=-1 managementActiveSelf=True managementInHierarchy=True canvasEnabled=True canvasActive=True populationPageSelf=False activeTab=Workplaces
2026-10-24T14:02:32.0744280Z [SAVE-RESTORE] runId=20261024T140231Z … matches-before=True   ← TearDown
```

即该轮失败点唯一：`ClickChild(management, "ModalPanel/Tab_Population")` → `RealPointerClick` → `AssertTopHitBelongs` 的 `Raycast 无命中 @ (158, 857)`；其后 `activeRows / pendingRows / 下一帧销毁` 断言从未执行（日志在 14:02:32.067 后直接进 TearDown）。

### 2.1 这些真实值说明了什么（确证 / 推断分开写）

**确证（本日志原始字段，未改写）：**

1. 首次真实导航点击**成功提交**：`surface=Management`、`managementActiveSelf=True`、`managementInHierarchy=True`、`canvasEnabled=True`、`canvasActive=True`；页签对象本身 `tabActiveSelf=True`、`tabInHierarchy=True`、`tabInteractable=True`、`tabRaycastTarget=True`、`tabCull=False`。
2. **同帧 `Canvas.ForceUpdateCanvases()` 前后 `Graphic.depth` 都是 -1**（`tabDepthBeforeRefresh=-1` / `tabDepthAfterRefresh=-1`），`ModalPanel` 的 `panelDepth` 也是 -1，同帧 Raycast 前后都 **0 命中**。→ 上一轮“同帧刷新即可命中”的补丁**被真实运行证伪**，不是猜测；本次不再重复同一方案。
3. 首次导航提交那一刻**当前页签不是人口页**：`activeTab=Workplaces`、`populationPageSelf=False`。旧 UI 会话的页签状态被产品保留（来源替换不改页签，`ManagementView.ApplyTabs` 只按 `_activeTab` 开关页容器）。→ 上一版在该帧统计“人口页可见行 = activeInHierarchy”并断言等于新来源人数，语义本身已不成立（人口页整体 inactive，行必然 0 可见），只是失败更早、从未执行到。

**推断（不作为确证，只据仓库内 uGUI 源码与上述取值判断）：**

- `Graphic.depth` 直接取 `canvasRenderer.absoluteDepth`（`Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Graphic.cs:387`）；`GraphicRaycaster` 在 `!raycastTarget || canvasRenderer.cull || depth == -1` 时跳过（同包 `GraphicRaycaster.cs:355`）。
- `Canvas.ForceUpdateCanvases()` 只触发托管侧 rebuild（CanvasUpdateRegistry），**不负责原生 canvas 批处理**，因此 `absoluteDepth` 仍为 -1、`cull` 仍为 false —— 这与“cull=False 但 depth=-1”的真实取值一致。该机制判断属推断；本轮**只用它来解释已实测的取值**，不据此宣称任何产品缺陷结论。
- 同理推断：页签点击把 `PopulationPage` 整体激活后，其子行 Graphic 在**同一帧**也还没被 Canvas 处理，因此“行命中”同样需要一帧真实渲染后才能断言。本文件把它当作需要实测的推断，宿主复跑时用新增证据行核对（见第 5 节判读规则）。

### 2.2 `ManagementView.ClearRows` 的真实语义（本次核对源码）

`Assets/Scripts/Village/ProductHome/ManagementView.cs:682-686`：

```csharp
    private static void ClearRows(List<ManagementRowView> rows)
    {
        foreach (var row in rows.Where(row => row != null))
            if (row != null) { row.gameObject.SetActive(false); Destroy(row.gameObject); }
    }
```

- 旧代行只做 `SetActive(false)` + `Destroy`（帧末物理销毁）；**不改 `Button.interactable`**。
- 所以“旧代无可交互”不能用 `button.interactable == false` 表达（它仍为 authored 的 true），必须用产品/引擎真实语义 `Selectable.IsActive()`（= `UIBehaviour.IsActive()` = `isActiveAndEnabled`，见 `Runtime/EventSystem/UIBehaviour.cs:26`；`Selectable.IsInteractable()` 只含 `m_Interactable && m_GroupsAllowInteraction`，见 `Selectable.cs:485`）。本次断言据此改为 `IsActive()`。

---

## 3. 测试重排（唯一改动文件：用例 10）

### 3.1 边界一：首次导航的“同帧同步”语义（不点击页签、不跨帧）

首次导航仍是**同帧、无 RefreshNow、无 yield、无私有字段写入**的真实 Raycast + PointerClick（`OpenManagementThroughNavigation` 保持不变），竞态再现条件原样保留：

- 点击前：`Bridge==secondLoaded`、`Shell._source` 仍是旧的（`ReferenceEquals` 为 false）、`Surface=="World"`、`Management.IsVisible==false`、旧派工已被 Load 替换 → 同帧点击。

点击后**同帧**强断言（全部保留原强度，新增页签/页容器一致性与旧代行语义）：

1. `Surface=="Management"`、`Management.IsVisible`、`SurfaceModalActive`、`ShellVisible`、`SimulationActive`、Clock 未推进；
2. `Shell._source == secondLoaded` 且 `Management._source == secondLoaded`（来源已在提交前被消费）；
3. 旧候选 / 旧检索清理：`SelectedWorkplaceId==null`、`SelectedConstructionBuildingId==null`、`nameSearch.text==""`；
4. 新源 snapshot PersonId 集合：`SnapshotPersonIds(management)` 与 `LegalPersonIdsFromSource(secondLoaded)` `AreEquivalent`，计数一致；
5. **页容器与当前页签一致**（新增，按产品 `ApplyTabs` 语义）：`populationPage.activeSelf == (ActiveTab=="Population")`、`workplacePage.activeSelf == (ActiveTab=="Workplaces")`、`resourcePage.activeSelf == (ActiveTab=="Resources")`；
6. **当前代人口行按当前页状态正确**：`activeSelf==true` 的行数 == 新来源人数、逐行标题对应新来源人物；每行 `Button.interactable==true`，且 `Button.IsActive()` **与该行所在页是否显示一致**（页签停在 Workplaces 时行不可交互，停在 Population 时行可交互）。不为了“同帧人口计数”再合成一次不现实的页签点击；
7. **旧代全部 inactive 无可交互**：点击前捕获的旧代行引用在帧末才销毁，本帧必须 `activeSelf==false`、`activeInHierarchy==false`、`Button.IsActive()==false`；且容器内 inactive 子物体**恰好**就是这批旧代行（数量、引用逐一核对）；
8. 证据行 `FIRST-NAV same-frame-committed …` 记录 `activeTab`、三个页容器 active、`populationContentInHierarchy`、`rowsSelfActive` / `rowsInHierarchy` / `expected`、`oldPendingInactive`、页签 active/interactable/raycastTarget/depth/cull 以及**同帧 Raycast 命中数**（仅留证，不作断言）。

> 已删除：`Canvas.ForceUpdateCanvases()` 同帧补丁及其 before/after 采样。删除理由不是“为了让测试变绿”，而是第 2.1 节 14:02 真实取值已证伪它无效；同帧 Raycast 事实仍写入证据行，判读不受影响。

### 3.2 边界二：下一真实渲染帧的第二次用户点击（页签）

- `yield return null;` —— 这是**第一次真实导航之后**、**第二次页签用户点击之前**的第一帧真实渲染（首次导航之前没有任何 yield）。
- 该帧断言：上一帧被替换的旧代行**已物理销毁**（`== null`）；容器 `childCount` 只等于当前代行数；证据行 `FIRST-NAV next-frame-before-tab-click …` 记录页签 `depth / cull / Raycast 命中数`。
- 然后 `ClickChild(management, "ModalPanel/Tab_Population", …)`：真实 `RaycastAll` + `ExecuteEvents.PointerClick`（`Button.interactable` 前置检查保留）。
- 点击后断言：`ActiveTab=="Population"`、`populationPage.activeSelf` 与 `activeInHierarchy` 均 true；容器行数 == 新来源人数；逐行标题对应新来源人物；每行 `activeInHierarchy`、`Button.interactable`、`Button.IsActive()` 均成立；首次提交时捕获的首行引用仍在当前代（未被无关重建替换）；容器内无旧代残留；证据行 `FIRST-NAV population-tab-clicked …`。
- 再 `yield return null;`（页签刚激活 `PopulationPage`，同样要等 Canvas 处理其子 Graphic），断言行身份/数量稳定，并**真实 Raycast 命中第一行**（`AssertTopHitBelongs`，不可放宽），证据行 `FIRST-NAV population-tab-row-hit …`（`raycastHits`、`firstRowDepth`、`firstRowCull`）。
- 其后的既有收尾全部保留：`Selection` 不得引用新来源外人物 → 真实 `_host.RefreshNow()` 断言 host 仍按既有“来源切换清 Selection”规则清空、且不关闭已提交表面 → `FIRST-NAV first-click-committed …` 证据行。

### 3.3 旧代行夹具的一处必要调整（并说明为什么不是“放宽”）

- 旧版把检索 session 写成固定串 `旧世界_QA_5c1a`（按姓名包含过滤，实际匹配 0 人），于是**点击那一刻人口容器里没有旧代行**，“旧代全部 inactive / 无可交互”只能对着空集合断言。
- 现改为：取管理投影里**真实人物 `DisplayName` 的首字**作为非空旧检索（仍是非空、仍会被来源切换清除、仍被三处断言逐字核对），并新增 `Assert.Greater(populationContent.childCount, 0, …)`；这样旧代行是**真实存在的行**，第 3.1 节第 7 条才有实际内容。
- 竞态再现条件未变：两次真实 `SaveSystem.Load` + `SetSimulationPersonSource`，第二次之后**不刷新、不等待帧**立即点击“管理”。
- 旧代行捕获位置：刚好在首次点击之前（`childCount` 全覆盖），未引入任何私有写入。

### 3.4 明确没有做的事

- 没有 `Invoke` 页签、没有强置 `interactable`、没有 `SetActive` 管理页/页容器求绿；页签仍由真实 Raycast + PointerClick 触发；
- 没有把首次导航放到 yield 之后，也没有在首次导航前加任何刷新/等待；
- 没有删除或放宽 Raycast / 行身份 / 数量 / 销毁 / Clock / 模态 / Shell 可见 / 来源消费断言；
- 没有改生产代码来“适配测试渲染”，没有新增 `Canvas.ForceUpdateCanvases` 之外的自定义刷新循环，也没有保留无效的同帧刷新补丁；
- 其余 9 例、`SaveSlotGuard`、NewGame 用例、Shell `SyncSourceReset`/`OpenSurface` 产品修复均未改。

---

## 4. 本次实测复核（改动后）

| 项目 | 实际结果 |
|---|---|
| 编译 | 仅 `script-execute → AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)`；随后 `isCompiling=false`、`isUpdating=false`、`isPlaying=false` |
| 工程编译错误 | `console | where message contains "error CS"` → 仅 3 条**旧轮（22:01:17）工具侧 snippet 报错**（`UnityTestAttribute` 命名空间写错，与工程资产无关）；无任何 `Assets/**/*.cs` 或 `ManagementShellPlayTests.cs` 相关错误 |
| 程序集实证 | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：`mtime=2026-10-24T14:07:13Z`、`bytes=427520`；旧串 `same-frame-tab-sample` / `same-frame rows visible` 已不存在，新串 `same-frame-committed` / `tabRaycastHitsSameFrame` / `next-frame-before-tab-click` / `population-tab-row-hit` 全部存在 → 本文件确已编译进测试程序集 |
| 测试装配 | 反射加载 `RRPG.ManagementShell.Tests.ManagementShellPlayTests`：`testTypeLoaded=True`、`playTests=10`（用例名 10 个齐全，未改名） |
| 34 项基线（改动后） | `entries=34 problems=0 all-match` |
| 受保护字体（改动后） | `bytes=14961033`、`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（未变） |
| Save 保护历史档（改动后） | `runs=95 statuses=protective-snapshot:1,restored:94 unfinished=0`（未新增 run，未改动任何备份） |
| 场景 | 仍唯一 `SampleScene`、`isDirty=false`、`rootCount=5` |
| 测试文件（改动后） | `ManagementShellPlayTests.cs`：`bytes=246710`、`sha256=76fc802306402112f0330a1f60339d8957f2a73b32aef52f69eaf256f2b4e18b` |
| 新增资产 | 无（只改 1 个已存在 `.cs` + 本文件） |

---

## 5. 宿主复跑时的判读规则（证据行 → 事实）

| 证据行 | 能判定的真实事实 |
|---|---|
| `FIRST-NAV pre-click …` | 首次点击前确实是“Shell 旧源 / Bridge 新源 / World / 旧候选 + 旧检索仍在” |
| `FIRST-NAV same-frame-committed …` | 同帧：导航已提交、页签=？、三个页容器 active、当前代行数/新源人数、旧代 inactive 数、页签 depth/cull、同帧 Raycast 命中数（预期仍为 0 / -1，仅留证） |
| `FIRST-NAV next-frame-before-tab-click …` | 第二动作开始前：页签 `depth` 是否已 ≥0、`cull` 是否 false、真实 Raycast 是否已能命中；容器是否只剩当前代行 |
| 点击日志 `首次导航·下一渲染帧回到人口页签` | 第二次点击确实是真实 Raycast + PointerClick 打到了 `Tab_Population`（若失败，消息即 `Raycast 无命中`，不再有“补丁掩盖”） |
| `FIRST-NAV population-tab-clicked …` | 页签切换已提交、行数/身份、旧代已销毁、容器干净 |
| `FIRST-NAV population-tab-row-hit …` | 人口行在渲染帧后的真实 `depth/cull/raycastHits` 与第一行命中 |

- 若 `next-frame-before-tab-click` 的 `tabRaycastHits > 0` 而某条业务断言仍失败 → 失败是业务事实，不是渲染时序，按业务断言原文定位；
- 若真实渲染帧后 `tabDepth` 仍为 -1 / `tabRaycastHits == 0` → 按任务要求**视为 Canvas 实际接线缺陷**处理并保留该证据，**禁止放宽或改断言**；本包不再新增同帧刷新尝试；
- 若 `population-tab-row-hit` 仍 0 命中 → 同上按几何/接线缺陷处理，证据行已给出 `firstRowDepth/firstRowCull/point`。

---

## 6. 保护与未测

### 6.1 保护

- 未改 34 项基线文件、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份 / manifest / 历史档；
- 未改产品源码（含 `HomeProductShell.SyncSourceReset` / `OpenSurface` 次序修复）、Prefab、Scene、Save DTO、WorkClock、Authority；
- 未使用 Shell、`rql-mutate`、全局 Save、Git 写；未启动 Test Runner、未忙等模型测试。

### 6.2 未测 / 留给宿主

1. **PlayMode 未由本执行层运行**：10 例通过/失败、`same-frame-committed` / `next-frame-before-tab-click` / `population-tab-clicked` / `population-tab-row-hit` 的真实数值、`BASELINE/FONT`、`SAVE-RESTORE matches-before=True` 均以宿主短类名整类串行执行为准；本文件不宣称通过。
2. **“页签/页容器激活后同帧行不可 Raycast”是推断**：依据是 14:02 已测的 `cull=False + depth=-1` 与 uGUI 源码判据；本包据此在页签点击后再让出一帧才断言行命中。若宿主实测该帧本就 >0 命中，多出的一帧不影响任何业务断言（不掩盖缺陷，因为所有业务断言在点击后同帧就已强断言）。
3. **旧代行夹具依赖真实姓名首字**：若宿主初始/加载后人物 `DisplayName` 全为空，用例会以明确消息失败（`夹具需要至少 1 名有姓名的人物…`），而不是放宽断言。
4. **`d8d69dfb` 的完整 NUnit 栈仍属 Studio 私有记录**：本层只用仓库内可见的 14:02 证据日志与 Console 序列（1425 行之后直接 TearDown）定位失败点，未读取私有回执。
5. **物理输入未测**：仍是 `EventSystem` 程序化点击真实 Button，不等价人工鼠标手感 / 拖拽验收。
6. **其它来源切换入口**（`Navigate(5)` 远征 / `Navigate(6)` 建造）不提交 Surface，未被本竞态覆盖，本包按最小范围未改。
