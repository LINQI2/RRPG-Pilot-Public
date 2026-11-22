# R3-B Pilot｜来源替换后首次导航的人口行重建生命周期（同帧旧行核对 + 测试语义修复）

生成时间：2026-10-24（本机）
阶段：1/1（同帧人口重建旧行生命周期）
状态：**已完成代码核对与测试侧最窄修复；仅用 `AssetDatabase.Refresh()` 编译通过；PlayMode 整类由宿主 check 串行执行，本执行层未运行、不宣称通过。**

---

## 0. 范围声明

- 实际改动 2 个文件：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  2. `Development/StudioRuns/R3B-UI-20261024/row-rebuild-lifecycle-review.md`（本文件）
- **未改 `ManagementView.cs`**：核对确认 `ClearRows` 已是“先 `SetActive(false)` 再 `Destroy`”，产品侧没有“旧行同帧仍 active / 仍可交互”的缺陷；按阶段指令改为修正测试计数语义，并保留独立交互/身份断言。
- 未改 `HomeProductShell.SyncSourceReset`、`ProductHomeWorldHost`、`HomeProductRouter`、`VillageRuntimeBridge`、SaveSystem / Save DTO / WorkClock / Authority / 规则 / DTO / API；
- 保留原 9 例强断言、`SaveSlotGuard`、NewGame 资源修复、首次导航产品修复与对应回归；未恢复“夹具预 `RefreshNow` 规避”；
- 未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未改 34 项基线、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份与历史档；
- 未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。

---

## 1. 本轮先决状态（只读核对，全部通过后才动文件）

| 项目 | 实际值 / 证据 |
|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` |
| 场景干净 | 唯一打开场景 `Assets/Scenes/SampleScene.scene`，`isDirty=false`、`isLoaded=true`、`rootCount=5` |
| 34 项基线（改动前） | 只读重算 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`：`total=34 problems=0` |
| 受保护字体（改动前） | `RRPG_SourceHanSansCN_Regular_TMP.asset` 实测 `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致） |
| Save 保护历史档 | `R3B-SaveProtection-20261024/*/manifest.json` 共 75 个 run，`status` 非 `restored/protective-snapshot` 的未完成 run = 0；最新 10 个 run 全部 `restored` |
| 测试发现（改动后） | `tests | where fullName contains "ManagementShellPlayTests"` → **10 例**、`PlayMode`、`BattleStage1.PlayModeTests` |

> 失败回执 `f2ec8446`（真实 10 例：9 过 1 败 0 skip）由 Studio 私有运行记录提供；本执行层只读到了仓库内同一时段（`2026-10-24T11:49:34Z`）的 `FIRST-NAV pre-click` 证据行，**未能读取完整 NUnit 栈**（Editor Console 已被后续日志截断，仓库内无 testResults XML）。失败断言可由“Expected 6 / Actual 12”唯一对应到新增用例的“人口行 = 快照人数”计数断言，详见第 3 节。

---

## 2. 失败用例与本轮核对对象

用例：`FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt`
（`ManagementShellPlayTests.cs`，类内第 10 例，旧行为第 3042-3046 行附近）

旧断言：

```csharp
var populationContent = (RectTransform)Get(management, "populationContent");
Canvas.ForceUpdateCanvases();
LayoutRebuilder.ForceRebuildLayoutImmediate(populationContent);
yield return null;
Assert.AreEqual(newSourceIds.Count, populationContent.childCount,
    "首次导航后人口行必须按新来源重建");
```

本次失败：`newSourceIds.Count = 6`，`populationContent.childCount = 12`。

需要判断的是：这 12 个 `childCount` 里，旧 6 行究竟是“active 且可交互的真实重复业务行”，还是“已被计数 helper（`childCount`）包含的 inactive / 待 `Destroy` 对象”。

---

## 3. 旧行生命周期核对（产品代码 + Unity Destroy 语义）

### 3.1 产品重查与行回收

`ManagementView.RefreshNow()`（第 162-188 行）：

- 观察到 `VillageRuntimeBridge.SimulationPersonSource` 被替换时，清旧候选/检索 session，并把四个 `*RowsKey` 置 `null`；
- 随后 `Apply()` → `ApplyPopulation()`。

`ApplyPopulation()`（第 434-474 行）：

- `personKey` 包含检索词、筛选、`SelectedPersonId` 与每位人物的显示事实（第 443-447 行）；
- `rebuildPerson = personKey != _personRowsKey`；需要重建时先 `_personRowsKey = personKey; ClearRows(_personRows);`（第 448-449 行），再逐人 `Spawn(...)`。

`Spawn()`（第 673-680 行）：`Instantiate(template, parent)` → `SetActive(true)` → 加入 `_personRows`。

`ClearRows()`（第 682-687 行，**修复前已存在的代码，本轮未改**）：

```csharp
private static void ClearRows(List<ManagementRowView> rows)
{
    foreach (var row in rows)
        if (row != null) { row.gameObject.SetActive(false); Destroy(row.gameObject); }
    rows.Clear();
}
```

### 3.2 Unity Destroy 延迟语义

`UnityEngine.Object.Destroy(GameObject)` 在运行期是**延迟销毁**：调用发生在当前帧的 Update 等回调中，真实销毁在本帧 Update 循环结束之后、渲染之前处理。因此在同一帧内：

- 被 `ClearRows` 回收的行已经 `activeSelf=false`，**不参与布局、不参与 UI Raycast、Button 不可交互**；
- 但对象尚未物理销毁，仍是 `populationContent` 的 `Transform` 子物体；
- `Transform.childCount` 仍是“当前代业务行 + 上一代待销毁行”的总和。

结论：`childCount` 不是业务可见行计数；用它断言“人口行 = 新来源人数”只在本帧没有发生第二次重建时才偶然成立。

### 3.3 12 的具体来源（6 + 6）

首次真实导航点击内部（产品路径）：

1. `HomeProductShell.OpenSurface(Management)` → `SyncSourceReset()` 消费来源替换 → `Router.Open(Management)`；
2. `Management.SetActive(true)` 触发 `OnEnable → RefreshNow()`：来源分支重置 key，`ClearRows` 回收旧行并 `Spawn` 新来源 6 行；
3. Shell 同一次 `RefreshNow()` 又调用一次 `Management.RefreshNow()`：`personKey` 未变，不再重建。

同帧稍后 / 点击后的首个被让出的帧，`ProductHomeWorldHost.RefreshNow()`（第 89-99 行）观察到 `_presentationSource` 已被替换，执行 `Selection.Clear()`；`ManagementView.SelectionChanged()`（第 315-319 行）随即 `RefreshNow()`。此时 `personKey` 中的 `SelectedPersonId` 由旧世界的 `oldPerson` 变为 `null`，`rebuildPerson=true`：

- `ClearRows` 把上一代 6 行 `SetActive(false)` 并 `Destroy`（本帧仍作为子物体存在）；
- 再 `Spawn` 新一代 6 行；
- 测试协程在该帧恢复后读 `childCount` = **6（当前代）+ 6（待销毁、已 inactive）= 12**。

这与“Expected 6 / Actual 12”的 2× 签名一致，也与证据日志中该用例只留下 `FIRST-NAV pre-click`、没有 `first-click-committed` 的行相吻合。

### 3.4 判定

- 旧 6 行**不是** active 且可交互的业务重复：`ClearRows` 先 `SetActive(false)` 是现有产品硬行为；inactive 行不参与布局/射线，Button 不可交互；
- 旧 6 行是**同帧被替换、已停用、待 `Destroy` 的上一代行**；失败来自测试把 `childCount` 当作业务行计数，而不是产品重复生成；
- 因此本轮**不改 `ManagementView.cs`**，按阶段指令修测试 helper 语义，并保留/加强独立断言：
  - 同帧只允许新来源身份的可交互行；
  - 待销毁旧行必须 inactive；
  - 下一帧旧对象必须物理销毁；
  - 不弱化原有 `PersonId` 集合等价断言。

---

## 4. 精确 diff（测试文件）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

### 4.1 新增只读 helper（第 1118-1146 行）

```csharp
private static string ExpectedPopulationRowTitle(object person)
{
    string profession = (string)Get(person, "Profession");
    bool isHero = (bool)Get(person, "IsHero");
    return (string)Get(person, "DisplayName") + " · "
           + (isHero ? "英雄" : (string.IsNullOrEmpty(profession) ? "职业资料暂缺" : profession));
}

private static List<string> ExpectedPopulationRowTitles(object management)
{
    object snapshot = Get(management, "CurrentSnapshot");
    Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
    return ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
        .Select(ExpectedPopulationRowTitle).ToList();
}

private static string PopulationRowTitle(Transform row)
{
    var view = row.GetComponent(T("ManagementRowView"));
    Assert.IsNotNull(view, "人口行必须绑定 ManagementRowView：" + row.name);
    var label = (TMP_Text)Get(view, "label");
    Assert.IsNotNull(label, "人口行 label 必须绑定：" + row.name);
    return label.text;
}
```

说明：只用现有只读投影与 `ManagementRowView` 的 authored `label` 做逐行身份对照；不新增产品 API，不写私有状态。

### 4.2 用例 10 的计数断言替换（第 3071-3141 行）

旧：单条 `Assert.AreEqual(newSourceIds.Count, populationContent.childCount, ...)`。

新：

1. 旧会话停在“职责与场所”页签，先真实 Raycast 点击回 `Tab_Population`，并断言 `ActiveTab=="Population"`、`populationPage.activeSelf==true`（否则 `activeInHierarchy` 不能代表业务可见性）；
2. 同帧把子物体分为：
   - `activeRows = child.activeInHierarchy`：业务可见行；
   - `pendingRows = child.!activeInHierarchy`：待销毁旧行；
   - 断言 `activeRows.Count == newSourceIds.Count`；
   - 断言每条业务行的 `label` 标题多重集与当前新来源投影人物标题**完全等价**（逐行身份，无旧来源残留、无重复业务行）；
   - 对每条业务行断言 `Button != null`、`button.interactable`、`button.gameObject.activeInHierarchy`；
   - 对第一条业务行执行真实 `RaycastAll`，断言 top hit 属于该行（真实可交互）；
   - 对每条 `pendingRows` 断言 `activeSelf == false`（已停用，不参与布局/交互）；
   - 写入证据行：
     `FIRST-NAV same-frame rows visible=… expected=… pendingDestroyInactive=… childCount=… uniqueVisible=True raycast=first-row`；
3. `yield return null` 后：
   - 断言上一帧捕获的每条 `pendingRows` 引用 `== null`（物理销毁）；
   - 断言 `activeRows.Count` 与逐行标题仍等于新来源身份；
   - 若本帧又产生新的待销毁行，再让一帧并断言其物理销毁；
   - 最终断言 `populationContent.childCount == 当前可见业务行数`（旧对象彻底移除）。

保留不动：前面的 `CollectionAssert.AreEquivalent(newSourceIds, managementIds)`、`TotalPersonCount==newSourceIds.Count`、旧候选/检索清理、Clock/门禁/模拟、最后 host 清旧 Selection 的断言。原 9 例未改。

### 4.3 用例 10 的 XML 说明补充

在用例注释中新增一条：人口行重建按同帧生命周期核对（业务行只算 active 且逐行匹配新来源；`ClearRows` 先停用再 Destroy 的旧行单列为待销毁，下一帧必须物理消失）。

---

## 5. 同帧 / 下一帧证据（本次编译实际结果 + 宿主将产出的运行证据）

本执行层实际完成：

| 证据 | 实际结果 |
|---|---|
| 编译 | 仅 `script-execute → AssetDatabase.Refresh()`；`isCompiling=false`、`isUpdating=false`、`isPlaying=false`；`console` 中 `error CS` 0 条、含 `ManagementShellPlayTests.cs` 的编译错误 0 条 |
| 测试装配 | `BattleStage1.PlayModeTests` 中 10 例可见；反射确认新增 helper（`ExpectedPopulationRowTitle/Titles`、`PopulationRowTitle`）已编入装配 |
| 场景 | 仍唯一 `SampleScene`、`isDirty=false`、`rootCount=5` |
| 基线/字体（改动后） | `total=34 problems=0`；`font sha256=3c758e16…` 未变 |
| 新增资产 | 无（只改 1 个已存在 `.cs` 与本文档） |

> 说明：Editor Console 另有本执行层两次“只读反射探查”产生的 Tool 异常（`TypeLoadException`，原因是从 `Assembly-CSharp` 查找本属 `Village.Runtime` 的 `ManagementPersonRow`）；随后已按正确装配复核类型/字段并通过，**不是工程编译错误**，也未写入任何产品状态。

宿主运行时应额外核对（写入既有
`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`）：

- `[FIRST-NAV] same-frame rows visible=6 expected=6 pendingDestroyInactive=6 childCount=12 …`（若同帧重建发生；若未发生则 `pendingDestroyInactive=0 childCount=6`，同样合法）；
- `[FIRST-NAV] first-click-committed …`（用例走完）；
- `[BASELINE-PRE/POST] total=34 …`、`[FONT-PRE/POST] unchanged=True`、`[SAVE-RESTORE] … matches-before=True`。

> 本执行层**未运行 PlayMode**：DSH 直接启动被宿主拒绝（“Unity 测试由 Studio 持久测试阶段执行；请提交 stages.check”）。因此本文不声称 10 例已通过；由宿主以短类名 `ManagementShellPlayTests` 整类串行执行判定。

---

## 6. 保护与未测

### 6.1 保护

- 未改产品 Authority / API / DTO / WorkClock / 规则 / 管理命令 Gateway / 固定 Prefab / 字体；
- 未改 `ManagementView.cs`（核对后确认无需改）；
- 未改 34 项基线、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份/manifest/历史档；
- 未使用 Shell、`rql-mutate`、全局 Save、Git 写；未启动 Test Runner、未忙等模型测试；
- `HomeProductShell.SyncSourceReset` 首次导航修复、原 9 例强断言、`SaveSlotGuard`、NewGame 资源修复均保留；未恢复夹具预 `RefreshNow`。

### 6.2 未测 / 交给宿主

1. **PlayMode 未由本执行层运行**：10 例（9 旧 + 1 新）的通过/失败、`testResults`、同帧证据行、`BASELINE/FONT/SAVE-RESTORE` 均以宿主整类串行执行为准。
2. **失败完整栈未在仓库内复核**：`f2ec8446` 属 Studio 私有记录；本层只读到了同段证据日志与失败计数签名（Expected 6 / Actual 12）。若宿主复跑仍失败，先看新增的 `same-frame` 证据行与断言消息，区分“真实 active 重复行”与“待销毁 inactive 行”。
3. **同帧重建的具体触发帧依赖宿主 0.1s 刷新相位**：本轮的断言对“本帧是否已发生第二次重建”两种相位都成立（`pendingDestroyInactive` 可为 0 或 6），不靠固定等待帧或放宽计数。
4. **物理输入手感**：仍是 `EventSystem` 程序化 Raycast + `ExecuteEvents.PointerClick`，不等价人工鼠标/拖拽验收。
5. **其它来源切换入口**：`Navigate(5)`（远征）/`Navigate(6)`（建造）不在本用例范围，未改。
