# R3-B Pilot｜来源切换后的首次导航不被复位吞掉（最窄产品修复 + 首次导航回归）

生成时间：2026-10-24（本机）
阶段：1/1（来源切换后的首次导航）
状态：**已改 `HomeProductShell` 最窄次序 + 新增 1 例真实首次导航回归；安全编译通过；PlayMode 未由本执行层启动，等待宿主以短类名 `ManagementShellPlayTests` 整类串行执行。本文件不声称新增/原有用例已经通过。**

---

## 0. 范围声明

- 仅改 3 个文件：
  1. `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeProductShell.cs`
  2. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  3. `Development/StudioRuns/R3B-UI-20261024/shell-first-navigation-sync-review.md`（本文件）
- 未改 Router / ManagementView / ProductHomeWorldHost / VillageRuntimeBridge / SaveSystem / Save DTO / WorkClock / Growth / Return 契约；
- 未新增公共 API、Authority 或玩法规则；未新增 Prefab / Scene / 资产；
- 未使用 Shell、`rql-mutate`、全局 `SaveAssets/SaveChanges`、Git 写；未改动/删除 `R3B-SaveProtection-20261024/*` 备份与历史档；
- 未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。

---

## 1. 本轮先决状态（只读核对，全部通过后才动文件）

| 项目 | 实际值 / 证据 |
|---|---|
| Editor 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`；`selection=[]` |
| 场景干净 | 唯一打开场景 `Assets/Scenes/SampleScene.scene`，`isDirty=false`、`isLoaded=true`、`rootCount=5` |
| 34 项基线（改动前） | 只读重算 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`：`entries=34 problems=0 all-match` |
| 受保护字体（改动前） | `RRPG_SourceHanSansCN_Regular_TMP.asset` 实测 `sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`（与基线一致） |
| Save 保护历史档 | `R3B-SaveProtection-20261024/*/manifest.json` 共 65 个 run：64 个 `status=restored`、1 个 `status=protective-snapshot`，**无未完成 run**；最新一轮 `20261024T114238Z`～`20261024T114246Z` 全部 `restored` |
| 测试发现（改动后） | `tests | where fullName contains "ManagementShellPlayTests"` → **10 例**、`mode=PlayMode`、`assembly=BattleStage1.PlayModeTests`；原 9 例名称与断言未改 |
| 编译 | 仅 `script-execute` 调 `AssetDatabase.Refresh()`；随后 `isCompiling=false`；工程 `Error/Exception`（`message starts-with "Assets/"`、以及含 `ManagementShellPlayTests.cs` 的 Error）均为 0 条 |

> 说明：任务给定的修复前失败回执 `09ec9d72` 属 Studio 私有运行记录，本执行层未读取、无法在仓库内复核；本文只把它当作“修复前因果由主开发持有”的给定事实，不以本文件冒充复现证据。

---

## 2. 根因（与既有 `return-management-reentry-fix.md` 一致，本轮改为产品侧修复）

真实链：

1. `SaveSystem.Load()` 把世界恢复到既有 Authority，并返回**新的** `PlayerCollection`；
2. 产品公共入口 `VillageRuntimeBridge.SetSimulationPersonSource(loaded)` 重新绑定长期人物真相源；
3. 此时 `HomeProductShell._source` 仍是旧实例，Shell 的 0.2s 周期刷新尚未到来；
4. 玩家第一次真实点击“管理” → `HomeProductShell.Navigate(2)` → `OpenSurface(Management)`：
   - **修复前**：`Router.Open(Management)` 先提交，随后同一次 `RefreshNow()` 才观察到来源变化 → `Router.Close()` 把刚打开的 Surface 复位回 `World` → `Management.SetActive(false)`；
5. 结果：玩家的首次合法导航被产品内部复位吞掉（不是管理规则拒绝，也不是旧的 `ManagementView` 实例）。

既有 `RefreshNow()` 中来源复位本身是必要语义（避免管理页继续显示旧世界投影），不能删除；缺陷是**复位被消费的时机晚于导航提交**。

---

## 3. 精确 diff（产品代码）

文件：`UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeProductShell.cs`

### 3.1 `RefreshNow()`：把来源复位提取为同一私有方法（行为不变）

修复前（第 107-111 行附近）：

```csharp
    public void RefreshNow()
    {
        transform.Find("R2VisualProfile").GetComponentInChildren<TMP_Text>().text=_host.VisualPrototypeEnabled ? "R2 对照：B 原型" : "R2 对照：A 白模";
        if(!ReferenceEquals(_source,VillageRuntimeBridge.SimulationPersonSource))
        { _source=VillageRuntimeBridge.SimulationPersonSource;Feed.ResetSession();Router.Close();_attentionPanel.SetActive(false); var growth=GrowthUI; if(growth!=null && growth.IsOpen && !growth.HasPendingChoice) growth.TryHide(); }
        Attention=HomeAttentionProjection.Read();
```

修复后（第 107-111 行）：

```csharp
    public void RefreshNow()
    {
        transform.Find("R2VisualProfile").GetComponentInChildren<TMP_Text>().text=_host.VisualPrototypeEnabled ? "R2 对照：B 原型" : "R2 对照：A 白模";
        SyncSourceReset();
        Attention=HomeAttentionProjection.Read();
```

新增私有方法（第 138-148 行，与修复前内联块逐句等价）：

```csharp
    /// <summary>
    /// 消费“长期人物来源被替换”（Continue / Return Load）时的既有复位：清会话 Feed、回 World、收起关注与成长页。
    /// 该复位必须先于用户本次导航被消费，否则随后的 RefreshNow 会把刚打开的 Surface 一并复位掉。
    /// 不新增 Authority / 公共 API，也不改写来源切换清旧 Selection（host）与候选（ManagementView）的既有归属。
    /// </summary>
    private void SyncSourceReset()
    {
        if(ReferenceEquals(_source,VillageRuntimeBridge.SimulationPersonSource)) return;
        _source=VillageRuntimeBridge.SimulationPersonSource;Feed.ResetSession();Router.Close();_attentionPanel.SetActive(false);
        var growth=GrowthUI;if(growth!=null && growth.IsOpen && !growth.HasPendingChoice) growth.TryHide();
    }
```

### 3.2 `OpenSurface(HomeSurface)`：先消费来源复位，再提交用户导航

修复前（第 139-144 行）：

```csharp
    public void OpenSurface(HomeSurface surface)
    {
        if(GrowthUI?.HasPendingChoice==true) { GrowthUI.TryHide();return; }
        if(GrowthUI?.IsOpen==true) { GrowthUI.TryHide(); }
        Build?.Cancel();Router.Open(surface);RefreshNow();
    }
```

修复后（第 149-155 行）：

```csharp
    public void OpenSurface(HomeSurface surface)
    {
        SyncSourceReset();
        if(GrowthUI?.HasPendingChoice==true) { GrowthUI.TryHide();return; }
        if(GrowthUI?.IsOpen==true) { GrowthUI.TryHide(); }
        Build?.Cancel();Router.Open(surface);RefreshNow();
    }
```

> 选择 `OpenSurface` 的原因：它是最窄的 Shell 组合层，真实导航 `Navigate(0..4)` 与 Inspector 的表面回调都经它提交；`Navigate(5/6)` 只做 `Router.Close()` / 建造面板，不提交 Surface，未被本竞态吞掉，故不改。

### 3.3 门禁顺序（修复后同一次点击内的精确次序）

1. `SyncSourceReset()`：来源不同 → `_source` 更新、`Feed.ResetSession()`、`Router.Close()`（World）、关关注面板、无未决选牌时收起 Growth；
2. `GrowthUI.HasPendingChoice` → `TryHide()` 后 **return**（成长未决仍禁止进入其他表面，未被绕过）；
3. `GrowthUI.IsOpen` → `TryHide()`（既有行为）；
4. `Build.Cancel()`；
5. `Router.Open(surface)`（提交用户本次导航）；
6. `RefreshNow()`：本次来源已消费 → 不再 `Router.Close()`；`Surface==Management && ShellVisible` → 激活并 `Management.RefreshNow()`；`SetTownInputBlocked(SurfaceModalActive && ShellVisible)`、管理计入 `SurfaceModalActive`。

### 3.4 未改变的既有归属（保护点）

- 来源切换清旧 **Selection**：仍在 `ProductHomeWorldHost.RefreshNow()` 的 `_presentationSource` 分支（`Selection.Clear()`），本轮未动；
- 来源切换清 **Management 候选 / 检索 session**：仍在 `ManagementView.RefreshNow()` 的 `_source` 分支，本轮未动；
- Growth / Person / Management 统一 `SurfaceModalActive`、`SetTownInputBlocked`、`ShellVisible`、Return `CanSaveWorld` / `SimulationActive` 门禁均未改；
- 周期刷新入口 `RefreshNow()` 仍保留完整复位（不是把复位删掉，只是把同一复位提前到导航提交之前消费）。

---

## 4. 新增真实首次导航回归（测试文件）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
新增用例：`FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt`（第 2929-3064 行；类内第 10 例）。

### 4.1 设计（全部真实产品入口，只读断言，不伪造）

1. **建立真实旧会话**：从宿主快照中选 1 名“无既有职责”的真实角色 → `VillageRuntimeBridge.TryAssignPerson(..., "ws_farm_01")`（既有公开入口，`ProductHomeFoundationPlayTests` 同款用法）→ 真实导航打开管理 → 真实行点击 `ws_farm_01` 设岗位候选 → 真实输入 `nameSearch` → ESC 回 World。此时旧候选/检索确实存在。
2. **来源复位仍有效**：真实底部导航打开 `Cards` → `SaveSystem.Load()` + `SetSimulationPersonSource(loaded1)` → 调一次产品周期刷新同一入口 `RefreshNow()`：
   - 断言 `Surface()` 由 `Cards` 回 `World`（复位未被削弱）；
   - `_source` 已消费为 `loaded1`（只读读字段）、管理页未被打开、`ShellVisible` / `SimulationActive` 保持。
3. **首次导航竞态**：再次 `SaveSystem.Load()` + `SetSimulationPersonSource(loaded2)`，**不调用 RefreshNow、不 yield 等待帧**，先只读断言：
   - Bridge 来源 = `loaded2`，而 Shell `_source` 仍是 `loaded1`（旧）→ 测试确实卡在“`_source` 旧而真实 Bridge 新”的条件；
   - `Surface=="World"`、管理页 inactive、旧派工已随 Load 消失（`PersonWorkplace(oldPerson)==null`）；
   随后执行第一次真实 Raycast + `ExecuteEvents.PointerClick` 点击 `ProductNavigation/Navigate2`。
4. **提交后断言**：`Surface=="Management"`、`Management.IsVisible`、`SurfaceModalActive`、`ShellVisible`、`SimulationActive`、Clock 未推进；Shell `_source` 与 Management `_source` 均为 `loaded2`；旧岗位/施工候选为 null、`nameSearch` 为空；管理人口投影与 `LegalPersonIdsFromSource(loaded2)` 逐 PersonId 等价，人口行按新来源重建；第一次点击后 Selection 不得引用新来源之外的人物。
5. **既有清旧 Selection 规则仍在**：随后调真实 `ProductHomeWorldHost.RefreshNow()`，断言 `Selection` 被清空、`Surface` 仍为 Management、管理页仍可见（host 复位不会关闭已提交表面）。

### 4.2 修复前会在哪一条失败（因果可判别）

修复前，第 4 步第一次点击内部：

- `Router.Open(Management)` 先执行；
- 同一次 `RefreshNow()` 观察到 `_source` 旧 → `Router.Close()` → `Surface` 回 `World`，`Management.SetActive(false)`；
- 因此 `Assert.AreEqual("Management", Surface(), "来源替换后第一次真实导航必须被提交，不得被来源复位吞掉")` 与 `Assert.IsTrue(Management.IsVisible)` 会在同一次真实点击后失败；第 3 步的只读前置断言保证失败原因只能是“复位吞掉导航”，不是 `_source` 已同步或管理页被提前打开。

> 该用例不 SetActive、不写 Router、不写任何私有字段；仅在必要时**只读** `_source` / `_source`（测试文件既有做法），用于证明“旧 `_source` + Bridge 新来源”的真实条件。

### 4.3 原有 9 例的处理

- 9 例名称、业务断言、SaveSlotGuard、NewGame 修复全部保留未改；
- 仅两处注释/计数更新：
  - 类头“最小长期回归 8 例”→“10 例”，并补一句本包新增用例说明；
  - `ExpeditionReturn_...` 中“来源复位会被后续 RefreshNow 吞掉，因此先用 RefreshNow 规避”的注释改为指向本轮产品修复与用例 10；该用例仍保留一次显式刷新，覆盖“复位先被周期刷新消费”的路径，断言不变。

---

## 5. 来源复位仍有效的证据（本轮测试内置）

| 证据 | 断言 |
|---|---|
| 周期刷新路径 | Cards → Load+SetSource → `RefreshNow()` → `Surface=="World"`，管理页未开，`_source==loaded1` |
| Select 复位 | 首次导航后 host `RefreshNow()` → `Selection==null`，Surface/管理页不被关闭 |
| Clock / 模拟 / 门禁 | 全过程 Clock 不推进、`SimulationActive==true`、`ShellVisible==true`、管理计入 `SurfaceModalActive` |
| 旧候选 | 首次导航后 `SelectedWorkplaceId==null`、`SelectedConstructionBuildingId==null`、`nameSearch==""` |
| 新来源绑定 | 管理人口投影 `AreEquivalent` 于 `LegalPersonIdsFromSource(loaded2)`，人口行数一致 |

`FIRST-NAV` 证据行（`periodic-reset` / `pre-click` / `first-click-committed`）会写入
`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，实际内容以宿主执行为准。

---

## 6. 改动后复核

- 安全编译：仅 `AssetDatabase.Refresh()`；`isCompiling=false`、`isUpdating=false`、`isPlaying=false`；
- Console：工程编译错误 0 条；
- 测试发现：`ManagementShellPlayTests` 共 10 例（原 9 + 新增 1），`PlayMode`；
- 34 项基线（改动后）与字体（改动后）只读重算：`entries=34 problems=0 all-match`、`font=3c758e16…`（未变）；
- 场景仍 `SampleScene`、`isDirty=false`；未产生新资产 / meta / Prefab / Scene。

---

## 7. 保护与未测

### 7.1 保护

- 未改 34 项基线文件、受保护 TMP 字体、`R3B-SaveProtection-20261024/*` 备份/manifest/历史档；
- 未改 `WorkClock`、Save DTO、`SaveSystem`、`VillageRuntimeBridge`、`ManagementView`、`ProductHomeWorldHost`、`HomeProductRouter`；
- 未使用 Shell、`rql-mutate`、全局 Save、Git 写；未启动 Test Runner、未忙等模型测试。

### 7.2 未测 / 留给宿主

1. **PlayMode 未由本执行层运行**：新增用例与原有 9 例的通过/失败、`testResults`、`BASELINE-PRE/POST total=34`、`FONT-PRE/POST unchanged=True`、`resourceScopeEnd=scope-restored` 均以宿主整类串行执行为准；本文件不宣称通过。
2. **夹具前置的真实性未在宿主外执行**：`TryAssignPerson(..., "ws_farm_01")`（0 分钟、Paused、无既有职责人物）与 `ws_farm_01` 兼容行/候选路径沿用 `ProductHomeFoundationPlayTests` / CASE-B 已证用法，但本轮未运行，失败时先看 `FIRST-NAV pre-click` 与 `SAVE-RESTORE` 证据行。
3. **首次 Load 后旧派工消失**：本用例选择“无既有职责”的真实角色，因此 `PersonWorkplace(oldPerson)==null` 可判别；若宿主初始名册/静态岗位配置变化导致无可选人物，用例会以明确消息失败而不是放宽断言。
4. **其它来源切换入口**：`Navigate(5)`（远征）与 `Navigate(6)`（建造）不提交 Surface，未被本竞态覆盖；本包按最小范围未改。
5. **物理输入**：仍是 `EventSystem` 程序化点击真实 Button，不等价人工鼠标手感/拖拽验收。
6. **修复前回执 09ec9d72**：属 Studio 私有运行记录，本执行层未读取；由主开发核验。
