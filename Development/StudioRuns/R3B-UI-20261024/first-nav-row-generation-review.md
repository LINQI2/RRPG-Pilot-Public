# R3-B Pilot｜首次导航用例的行实例代际核对与稳定交互断言（用例 10 有界修复）

生成时间：2026-10-24（本机）
阶段：1/1（核对行实例代际与稳定交互断言）
状态：**已用仓库内真实失败证据定位到唯一断言；已把 `childCount == 当前业务行数` 这种“异步 Destroy 已完成”的无条件前置，替换为逐对象代际断言（旧引用到帧边界必须销毁 / 待销毁代必须 inactive 且不参与 Raycast / 当前代 active 行身份多重集唯一匹配新来源且真实命中）。仅改测试文件与本文件；`ManagementView.cs` 未改。编译只用 `AssetDatabase.Refresh`；PlayMode 由宿主以短类名 `ManagementShellPlayTests` 整类串行 check，本执行层未运行、不宣称通过。**

---

## 0. 范围声明

- 实际改动 2 个文件：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（唯一用例 10 + 3 个只读 helper + 类内 XML 说明）
  2. `Development/StudioRuns/R3B-UI-20261024/first-nav-row-generation-review.md`（本文件）
- **未改产品源码**：`ManagementView.cs` / `HomeProductShell.cs` / `HomeProductRouter.cs` / `ProductHomeWorldHost.cs` / `VillageRuntimeBridge.cs`、Prefab、Scene、Save DTO、WorkClock、Authority、规则 / API / DTO 均未改（`ManagementView.cs` 实测 `bytes=37518`、`sha256=908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d`，`ClearRows` 仍是“先 `SetActive(false)` 再 `Destroy`”）。
- 禁止项复核：未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未改动 34 项基线文件、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份 / manifest / 历史档；未改「14 与 34」相关历史档；未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent；未启动 Test Runner、未忙等模型测试。
- 保留：其余 9 例、NewGame 资源修复、Shell 首次导航修复（`SyncSourceReset` / `OpenSurface`）、`SaveSlotGuard` 一字未改；未恢复“夹具预 `RefreshNow` 规避”。

---

## 1. 本轮先决状态（只读实测，全部通过后才动文件）

| 项目 | 实际值 / 证据（本次实测） |
|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`；`selection=[]` |
| 场景干净 | 唯一打开场景 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`isLoaded=true`、`rootCount=5` |
| 34 项基线（改动前） | 按 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 逐项重算字节 + SHA256：`entries=34 problems=0 all-match` |
| 受保护字体（改动前） | `RRPG_SourceHanSansCN_Regular_TMP.asset`：`bytes=14961033`、`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致） |
| Save 保护历史档 | `R3B-SaveProtection-20261024/*/manifest.json` 共 **105** 个 run：`protective-snapshot:1` + `restored:104`，**未完成 run = 0**；最新 10 个（`20261024T140851Z` … `20261024T140859Z`）全部 `restored` |
| 测试发现 | `tests | where fullName contains "ManagementShellPlayTests"` → **10 例**、`PlayMode` |
| 测试文件（改动前） | `ManagementShellPlayTests.cs`：`bytes=246710`、`sha256=76fc802306402112f0330a1f60339d8957f2a73b32aef52f69eaf256f2b4e18b` |
| 测试程序集（改动前） | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：`bytes=427520`、`mtimeUtc=2026-10-24T14:07:13Z` |

> 该文件哈希与 14:07:13 编译出的程序集对应，而下方 14:08:51–14:09:00 那一轮日志正是由该程序集执行。因此“最新一次真实 10 例运行”与当前失败代码是同一版本，不存在读错版本的可能。

---

## 2. 最新 FIRST-NAV 证据：失败点唯一、旧引用断言已通过

### 2.1 仓库内可见的原始日志（`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`）

最后一轮 10 例运行（`2026-10-24T14:08:51Z` … `14:09:00Z`）中，用例 10 的原始行只有：

```
2026-10-24T14:08:55.5220393Z [FIRST-NAV] periodic-reset surface=Cards→World shellSourceConsumed=True managementHidden=True shellVisible=True simulationActive=True
2026-10-24T14:08:55.5235339Z [FIRST-NAV] pre-click shellSourceIsLoaded=False bridgeSourceIsLoaded=True surface=World clock=0 staleCandidate=ws_farm_01 staleSearch=林
2026-10-24T14:08:55.5330020Z [FIRST-NAV] same-frame-committed surface=Management activeTab=Workplaces populationPageSelf=False workplacePageSelf=True resourcePageSelf=False populationContentInHierarchy=False rowsSelfActive=6 rowsInHierarchy=0 expected=6 oldPendingInactive=1 tabActiveSelf=True tabInHierarchy=True tabInteractable=True tabRaycastTarget=True tabDepth=-1 tabCull=False tabRaycastHitsSameFrame=0（同帧 Raycast 值只留证，不作断言、不据此放宽任何业务断言）
2026-10-24T14:08:55.5493889Z [SAVE-RESTORE] runId=20261024T140855Z existed=True sha256=4b2b56f6… matches-before=True   ← TearDown
```

该轮日志在 `same-frame-committed` 之后**没有** `next-frame-before-tab-click`、也没有任何页签点击日志，16ms 后直接进 TearDown；随后 `SAVE-RESTORE` 明确 `matches-before=True`（存档已按 runID 恢复）。

### 2.2 失败断言与“旧引用是否已销毁”的严格对应

- 回执 `e0763cd4` 的失败信息为“**下一帧 childCount 12 当前代 6**”。
- 用例 10 中**只有一条**断言消息同时包含 `childCount=` 与 `当前代=`：
  `Assert.AreEqual(currentRows.Count, populationContent.childCount, "下一渲染帧：容器只应剩下当前代人口行（childCount=… 当前代=…）")`。
- 该断言在改动前文件中紧随其后对旧代引用的校验只有一行：
  `foreach (var oldRow in oldGenerationRows) Assert.IsTrue(oldRow == null, "下一渲染帧：被替换的旧代人口行必须已物理销毁");`
- 断言按顺序执行：能进入 `childCount/当前代` 那条断言（并输出其消息），即证明**紧邻其前的“首次点击前捕获的旧代引用全部 `== null`”已经通过**。

**结论（证据链，不是推测）：** `childCount=12` **不能**证明“最初捕获的旧引用没销毁”；相反，旧引用在下一渲染帧边界已物理销毁。失败来自“容器里又出现了一批同一业务身份的新实例”，而 `childCount == 当前业务行数` 这条前置把“本帧是否刚好没有第二次合法重建”当成了异步 Destroy 的完成条件。

### 2.3 12 的构成与机制（源码 + 真实取证）

同帧证据：`rowsSelfActive=6`、`expected=6`、`oldPendingInactive=1`、`childCount` 未记录但等于 1（旧代）+6（当前代）=7。
下一帧变成 12 的唯一解释是“6（第一次重建后当前代，被停用待销毁）+ 6（第二次重建的新当前代）”：

1. 首次真实点击同帧：`HomeProductShell.OpenSurface(Management)` → `SyncSourceReset` → `Router.Open` → `Management.OnEnable → RefreshNow()`。来源被替换（`_source` 变化）→ 清旧候选 / 检索，`_personRowsKey=null` → `ApplyPopulation` 重建一次：旧检索留下的 1 行被 `ClearRows` 停用，新来源 6 行生成。即同帧 `oldPendingInactive=1`。
2. 同帧稍后 Shell 再次 `RefreshNow`，`personKey` 未变，不再重建（同帧证据 `rowsSelfActive=6` 与之吻合）。
3. 下一帧 `ProductHomeWorldHost.Update`（每 0.1s，`ProductHomeWorldHost.cs:65-73`）观察到 `_presentationSource` 已被替换 → `RefreshNow()` → `Selection.Clear()`；`ManagementView.SelectionChanged()`（`ManagementView.cs:315-319`）→ `RefreshNow()`。
4. `ApplyPopulation` 的 `personKey` 含 `SelectedPersonId`（`ManagementView.cs:443-449`）。旧选择被清后 `SelectedPersonId` 由旧人物变为 `null` → `rebuildPerson=true`：
   `ClearRows(_personRows)` 把上一代 6 行 `SetActive(false)` + `Destroy`（本帧末才物理销毁），再 `Spawn` 新一代 6 行。
   测试协程在该帧恢复时读到：`childCount = 6（当前代 active）+ 6（上一代已停用、待销毁）= 12`。

`ClearRows` 实际源码（未改）：

```csharp
private static void ClearRows(List<ManagementRowView> rows)
{
    foreach (var row in rows)
        if (row != null) { row.gameObject.SetActive(false); Destroy(row.gameObject); }
    rows.Clear();
}
```

即：**同一帧内不存在“12 条 active 业务行”**；多出来的 6 条是被产品明确停用、本帧末销毁的上一代实例。该判断由“真实失败签名（旧引用已销毁、childCount=12）+ `ClearRows` 硬语义”共同给出；新一代/待销毁代的 active 分裂由本轮新增证据行在宿主复跑时给出实测值（见第 5 节判读规则），本执行层不把未实测的字段写成已证事实。

### 2.4 是否属于产品生命周期泄漏 / 重复 active？

- 代码上，唯一产生行的地方是 `ApplyPopulation` 按 `filtered.Persons` 逐人 `Spawn`，每一代行数量恒等于筛选后人数；`ClearRows` 在生成新一代前无条件 `SetActive(false)`，不存在“旧行仍 active 与新行并存”的代码路径。
- 两次重建的触发都是合法业务事件：来源替换 + host 按既有规则清旧 `Selection`；重建后 `personKey` 稳定，不会再逐帧重建。
- 因此本包**不改 `ManagementView.cs`**：没有“重复 active”或“旧行未销毁生命周期泄漏”的确证。改动集中在测试对“代际”的表达，而不是放宽产品行为。
- 若宿主复跑证据出现 `activeGeneration > expected` 或“待销毁代在下一帧边界仍非 null / 仍可 Raycast”，那才是产品生命周期泄漏，应按最窄 `ManagementView.ClearRows` 方案另立包处理（本包不预改）。

---

## 3. 测试改动（唯一改动文件：用例 10 + 只读 helper）

### 3.1 新增只读 helper（第 1153–1217 行）

- `PopulationRowGenerations`：把 `populationContent` 子物体按**逐对象**分为
  - `Active = activeSelf == true`（当前代业务行）；
  - `Pending = activeSelf == false`（`ClearRows` 已停用、本帧末才销毁的上一代行）。
- `ClassifyPopulationRows(RectTransform)`：只做分组，不使用 `childCount` 判代际。
- `AssertCurrentPopulationGeneration(rows, expectedTitles, populationPageShown, what)`：
  - 当前代数量 == 真实新来源人数；
  - 当前代标题**多重集**与当前投影人物标题完全等价（NUnit `AreEquivalent` 计入重复计数；并断言行实例不重复）→ 唯一匹配、无旧来源残留、无重复业务行；
  - 每行 `activeSelf == true`；`activeInHierarchy` 与人口页是否显示一致；`Button.interactable == true`；`Button.IsActive()` 与人口页是否显示一致。
- `AssertPendingPopulationGeneration(rows, what)`：
  - 待销毁代必须 `activeSelf == false`、`activeInHierarchy == false`、`Button.IsActive() == false`（停用对象不参与布局 / 交互）；
  - 对该行屏幕中心做真实 `EventSystem.RaycastAll`，断言 top/任意 hit 都不属于该行 → **不参与 Raycast**。

以上均为只读反射 / 公开 API，不新增产品 API、不写私有字段。

### 3.2 边界二的重写（当前文件第 3262–3362 行）

**替换掉的旧前置：**

| 旧断言 | 旧断言的问题 |
|---|---|
| 下一帧 `currentRows.Count == populationContent.childCount`（本次失败点） | 把“本帧没有第二次合法重建”当作异步 Destroy 完成条件；无法区分“旧代未销毁”与“新代已生成” |
| 页签点击后 `newSourceIds.Count == tabRows.Count`（`tabRows` = 容器全部子物体） | 同上，把待销毁代算进业务行 |
| 页签点击后 `tabRows.Count == childCount`（“容器内不得再有旧代残留”） | 用总子物体数表达“无旧代”，在合法重建帧必然误伤 |
| 渲染帧后 `tabRows.Count == childCount`、数量/身份“不漂移” | 把一帧前的实例集合当成“必须跨帧不变”，与 host 清 Selection 的合法重建冲突 |

**替换后的逐对象代际断言（不删除业务标准，只把标准写精确）：**

1. 下一渲染帧（第一次 `yield return null` 之后）：
   - 首次点击前捕获的**旧代引用必须全部物理销毁**（保留原断言）；
   - 重新分组：`Pending` 全部 inactive / 不可交互 / 不被 Raycast 命中；`Active` 数量与身份多重集唯一匹配新来源，按钮可交互；
   - 记录 `childCount` 仅作证据，不再作为通过条件。
2. 页签真实点击（真实 `RaycastAll + PointerClick`，未改）之后同帧：
   - `ActiveTab=="Population"`、`populationPage.activeSelf/activeInHierarchy`；
   - 重新分组，`Pending` 仍须全部 inactive/不可交互/不被命中；
   - `Active` 仍是同一批对象（页签点击不得替换当前代），首次边界捕获的首行仍在当前代；旧代引用保持已销毁。
3. 行命中前的有界代际稳定（最多 3 帧，每帧都断言）：
   - 上一批 `Pending` 到本帧边界必须全部物理销毁；
   - 当前 `Pending` 必须全部 inactive/不可交互/不被命中；
   - 当前 `Active` 必须唯一匹配新来源、数量正确、按钮可交互；
   - 若本帧 `Active` 出现**新实例**（说明上一帧 host 周期刷新又触发了一次合法重建），只让出一帧让 Canvas 处理新 Graphic，再重新核对；
   - 3 帧后仍“每帧都出现新代” → 断言失败（真实持续重建），**不无限等帧**。
4. 在稳定代上做真实行命中：`RaycastAll` 的 top hit 必须属于当前代第一行（未放宽），并记录 `firstRowDepth / firstRowCull / activeGeneration / pendingGeneration`。
5. 其后的既有收尾全部保留：`Selection` 不得引用新来源外人物 → 真实 `_host.RefreshNow()` 清旧 Selection、不关闭已提交表面 → `first-click-committed` 证据行。

### 3.3 等价业务标准（为什么不是“删失败断言求绿”）

| 业务事实 | 旧表达 | 新表达（更强） |
|---|---|---|
| 容器里只有当前代业务行 | `childCount == 当前代行数` | `Active` 行数 == 新来源人数，且身份多重集逐项唯一匹配；`Pending` 行全部 inactive |
| 旧代不得被用户看到 / 点到 | `childCount == 当前代行数`（间接） | 旧代引用到帧边界必须 `== null`；待销毁代 `activeSelf/activeInHierarchy/IsActive` 全 false，且真实 Raycast 不命中 |
| 不能有重复业务行 | `childCount == 当前代行数`（间接） | `Active` 行实例不重复 + 身份多重集等价（重复计数一致）；数量等于真实来源人数 |
| 当前代可交互、可命中 | 行 Button 状态断言（保留） | 同上，并在**已稳定一帧**的当前代第一行做真实 `RaycastAll` top-hit 断言 |
| 异步 Destroy 已完成 | `childCount == 当前代行数` | 逐批捕获的待销毁代到**下一个合法帧边界**必须全部 `== null`；不在同帧要求物理消失 |

### 3.4 明确没有做的事

- 没有 `Invoke` 页签 / 强置 `interactable` / `SetActive` 管理页或页容器求绿；页签仍是真实 `RaycastAll + PointerClick`；
- 没有把首次导航放到 `yield` 之后，首次导航前没有 RefreshNow / 等帧 / 私有写；
- 没有删除 Raycast、身份、数量、销毁、Clock、模态、Shell 可见、来源消费等业务断言；
- 没有改生产代码 / Prefab / Scene / DTO / WorkClock；没有新增规则 / API / DTO / WorkClock；
- 没有新增“固定等待 N 帧必然成功”的延时求绿：等待只由“当前代是否为本帧新实例”驱动，且有 3 帧硬上限与失败判定。

---

## 4. 后续断言的帧条件梳理（避免继续一个错误一包）

| 位置（当前行号） | 执行帧 | 依赖的实例/状态 | 是否跨帧持有引用 | 处理 |
|---|---|---|---|---|
| 首次点击后同帧业务断言（3081–3104） | 点击帧 N | 无行实例 | 否 | 原样保留 |
| 同帧当前代/旧代断言（3178–3259） | N | `oldGenerationRows`（点击前捕获）、`currentRows`（同帧 activeSelf） | 否（同帧） | 原样保留；同帧只会有一次重建（host Update 在协程恢复前已执行），`pending == oldGenerationRows` 成立 |
| 下一渲染帧边界（3265–3289） | N+1 | `oldGenerationRows` | 是（N→N+1） | 保留“旧引用必须 null”；新增逐对象分组断言与证据 |
| 页签真实点击（3292–3296） | N+1 | `boundaryFirstRow` | 同帧 | 保留真实 Raycast + PointerClick；断言当前页与页容器 |
| 页签点击后分组（3302–3314） | N+1 | `boundaryFirstRow` | 同帧 | 断言当前代未被页签替换；待销毁代仍 inactive |
| 代际稳定循环（3320–3350） | N+2 … N+4（最多 3 帧） | `previousActive/previousPending` | 是（逐帧滚动） | 每帧核对待销毁代物理销毁 + 当前代身份/数量/按钮；新实例出现才多等一帧；超限失败 |
| 行命中（3352–3362） | 稳定帧 | `renderedActive[0]` | 否（本帧重新取） | 真实 Raycast top-hit，不移到“可能新建行”的同帧 |
| Selection / host 收尾（3363–3371） | 稳定帧 | 无行实例 | 否 | 原样保留（`Call(_host,"RefreshNow")` 仍验证清旧 Selection 且不关表面） |
| `first-click-committed` 证据（3373+） | 稳定帧 | `managementIds`（同帧快照） | 否 | 原样保留 |

> 关键点：本用例之后**任何**“容器行数 / 行身份”断言都不再跨帧持有旧实例集合；每个跨帧断言只持有一批“必须被销毁”的引用，并对“当前代”在本帧重新分类。

---

## 5. 本次实测复核（改动后）

| 项目 | 实际结果 |
|---|---|
| 编译 | 仅 `script-execute → AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)`；随后 `isPlaying=false`、`isCompiling=false`、`isUpdating=false` |
| 脚本编译错误 | 工程侧：`console` 查询 `message contains "error CS" or message contains "ManagementShellPlayTests.cs("` → **0 条**；程序集已在 14:15:23 更新且新 helper 可反射加载，证明 `ManagementShellPlayTests.cs` 编译通过。工具侧：本次执行层的 2 条只读反射片段报错（`snippet.cs` 缺 `using System.Reflection` 导致 `GetCustomAttributes()` 重载缺失，随后已修正并用正确写法复核通过），与工程资产编译无关 | |
| 程序集实证 | `BattleStage1.PlayModeTests.dll`：`bytes=431104`、`mtimeUtc=2026-10-24T14:15:23Z`；类型内新增 `ClassifyPopulationRows` / `AssertCurrentPopulationGeneration` / `AssertPendingPopulationGeneration` 与嵌套 `PopulationRowGenerations` 均已编入；新增证据串 `next-frame-before-tab-click`、`render-frame-generation`、`freshlyRebuilt`、`population-tab-row-hit` 全部存在于 #US 堆；旧串 `容器只应剩下当前代人口行` / `容器行数必须等于新来源人数` 已不存在 |
| 测试装配 | 反射 `RRPG.ManagementShell.Tests.ManagementShellPlayTests`：`UnityTest` 方法 **10** 个（未改名、未增删） |
| 34 项基线（改动后） | `entries=34 problems=0 all-match` |
| 受保护字体（改动后） | `bytes=14961033`、`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（未变） |
| Save 保护历史档（改动后） | `runs=105 unfinished=0 statuses=protective-snapshot:1,restored:104`；未新增 run、未改任何备份/manifest |
| 场景 | 仍唯一 `SampleScene`、`isDirty=false`、`rootCount=5` |
| 测试文件（改动后） | `bytes=253418`、`sha256=f290ea602e6a34d5b0217a66384a16e874f5aa7084b51589e719b44d500c784d` |
| 新增资产 | 无（只改 1 个已存在 `.cs` + 本文件） |

---

## 6. 宿主复跑判读规则（证据行 → 事实）

新增证据行都写入既有
`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`：

| 证据行 | 可判定的真实事实 |
|---|---|
| `FIRST-NAV same-frame-committed … rowsSelfActive=6 expected=6 oldPendingInactive=1` | 同帧只有一次重建：旧检索行（1）待销毁，新来源当前代 6 |
| `FIRST-NAV next-frame-before-tab-click … childCount=… activeGeneration=… pendingGeneration=… oldGenerationRefsDestroyed=True pendingAllInactive=True` | 下一帧：旧引用已物理销毁；容器可能 `6+6`；12 中的 6 是 inactive 待销毁代，不是重复 active 业务行 |
| `FIRST-NAV population-tab-clicked … activeGeneration=6 pendingGeneration=…` | 页签点击没有替换当前代；待销毁代仍未参与业务可见面 |
| `FIRST-NAV render-frame-generation attempt=… freshlyRebuilt=True/False` | 是否又发生一次合法重建（host 清旧 Selection）；`True` 时只多等一帧让 Canvas 处理新 Graphic |
| `FIRST-NAV population-tab-row-hit … raycastHits / firstRowDepth / firstRowCull` | 在**已稳定一帧**的当前代第一行上的真实命中；`depth` 应为 `>=0`、`cull=false` |

判读：

1. 若 `next-frame-before-tab-click` 为 `oldGenerationRefsDestroyed=True pendingAllInactive=True activeGeneration=expected`，而后 `population-tab-clicked`、`render-frame-generation` 均 `activeGeneration=expected`、`population-tab-row-hit raycastHits>0` → 用例通过，且证明“12 = 6 active + 6 待销毁 inactive”。
2. 若 `pendingGeneration>0` 但 `pendingAllInactive=False` 或待销毁行出现在 Raycast 命中里 → **真实产品生命周期泄漏**（重复 active / 停用不彻底），按第 2.4 节另立最窄产品修复包；本轮不预改。
3. 若 `render-frame-generation` 连续 `attempt=0/1/2` 都是 `freshlyRebuilt=True` → 真实“每帧持续重建”，用例以“代际必须在限定渲染帧内稳定”失败；这属于产品 / host 刷新语义缺陷，不放宽。
4. 若 `population-tab-row-hit raycastHits=0` 且当前代已稳定 → 按几何 / Canvas 接线缺陷处理（证据已含 `point/firstRowDepth/firstRowCull`），不用“再多等几帧”掩盖。
5. `SAVE-RESTORE … matches-before=True`、`BASELINE-POST total=34`、`FONT-POST unchanged=True` 仍为收尾保护判据。

---

## 7. 保护与未测

### 7.1 保护

- 未改产品源码（含 `ManagementView.ClearRows`）、Prefab、Scene、Save DTO、WorkClock、Authority / 规则 / API；
- 未改 34 项基线、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份 / manifest / 历史档；
- 未使用 Shell、`rql-mutate`、全局 Save、Git 写；未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent；
- 原 9 例、NewGame 用例、Shell 首次导航修复、`SaveSlotGuard` 均保留；未改测试类名 / 用例名 / 程序集。

### 7.2 未测 / 留给宿主

1. **PlayMode 未由本执行层运行**：10 例通过/失败、新增 5 类证据行的真实数值、`SAVE-RESTORE matches-before=True`、`BASELINE/FONT` 均以宿主短类名 `ManagementShellPlayTests` **整类串行**执行 check 为准；本文件不宣称通过。
2. **失败回执 `e0763cd4` 的完整 NUnit 栈属 Studio 私有记录**：本层以“唯一含 `当前代=` 的断言消息 + 同轮证据日志在 `same-frame-committed` 后直接 TearDown + 前一断言顺序”唯一确定失败点为旧 `childCount` 断言；旧引用已销毁由“能执行到该断言”逻辑证明，未读取私有栈。
3. **“12 = 6 active + 6 待销毁 inactive”的 active 分裂**：由失败签名 + 产品 `ClearRows` 语义推出；`activeGeneration/pendingGeneration` 的实测值待宿主证据行确认。本包据此不预改产品。
4. **host 周期刷新的相位**：`render-frame-generation` 的 `freshlyRebuilt` 分支覆盖“清 Selection 的合法重建发生在 N+1 或之后的 0.1s tick”，不依赖固定帧数；3 帧上限内必须稳定。
5. **物理输入未测**：仍是 `EventSystem` 程序化 Raycast + PointerClick，不等价人工鼠标手感 / 拖拽验收。
6. **其它来源切换入口**（`Navigate(5)` 远征 / `Navigate(6)` 建造）不提交 Surface，未被本用例覆盖，本包未改。
