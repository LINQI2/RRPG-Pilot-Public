# R3B-UI 控制器与绑定契约报告（2026-10-23）

状态：**本小包实现完成 / 待主开发核验**。只交付 Management 页面控制器与行组件源码、绑定契约与编译回执。
**未创建 Prefab / 未改 Scene / 未改 Shell 接线 / 未跑 Play / 未跑 tests-run / 未做全量回归 / 不宣称 R3-B PASS。**
后续由主开发接 Editor 可编辑 Prefab + Shell 集成，再进入真实专项 Play 与完整回归收尾。

## 1. 授权范围与前置核对

- 唯一工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`；Unity 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`。
- Unity 实际连接核对：`editor | select unityVersion projectPath` → `2022.3.62t9 | D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`，未启动/关闭 Editor。
- 当前场景：`scenes | select name path isDirty` → `SampleScene | Assets/Scenes/SampleScene.scene | false`，本包未写场景。
- 开工基线与保护：`Development/StudioRuns/R3B-20261022/workspace-baseline.json` 逐项重算 SHA256 → `entries=34 same=34 diff=0 missing=0`。其中 26 条为既有修改（含任务所指 **14 项保护类**：字体 1 + NuGet 插件 10 + Packages 2 + ProjectSettings 1；另 12 条为既有 Growth ScriptableObjects），8 条为既有未跟踪工具链文件；全部与基线一致，未恢复 HEAD 旧备份、未覆盖既有修改。
- 权限/环境：未安装组件、未改权限、未改 Packages/字体/插件、未动 Save 用户档、未 Shell/Process.Start、未 commit/push/stage/切分支/改远程。未访问其他工程或认证目录。
- 技能读取：本执行层**未暴露** `unity:ui` / `unity:ui-ugui` 技能（会话 skill 目录仅有 `diagnose-windows-sandbox-acl`），如实记录，未绕过目录权限。实现依据原 R3-P 规格 §4/§5、R3-A `PersonDetailView` 现有代码模式与现有 UGUI/TMP 结构。

## 2. 实际文件差异与哈希

新增（未修改任何既有文件；meta 由 Unity 自动生成）：

| 文件 | 字节 | SHA256 |
|---|---:|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/ManagementView.cs` | 37518 | `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d` |
| `.../ManagementView.cs.meta` | 267 | `ef6483af28ab0418c203eeb71c0d0ab14265bcc41b8c0fde0f2dd3224dd289aa` |
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/ManagementRowView.cs` | 1670 | `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd` |
| `.../ManagementRowView.cs.meta` | 267 | `e6a60f11ee19e9440671c92952cac4dc9c944569b78ceed0ef15a425b2909132` |

编译产物：`Library/ScriptAssemblies/Assembly-CSharp.dll` 645120 bytes，写入 `2026-10-23T12:06:17.3885898Z`；源码写入 `2026-10-23T12:06:15.1653305Z` / `12:02:15.9366499Z` → `fresh=True`，`ManagementView` / `ManagementRowView` 均在 `Assembly-CSharp` 中实际存在（35 个 `[SerializeField]` 字段）。

明确未改：`HomeProductShell.cs`、`HomeProductRouter`/`HomeProjection`、`PersonDetailView.cs`、`PersonDetailReadAdapter`、Growth/Inspector、`ManagementSnapshot.cs`、`ManagementReadAdapter.cs`、`ManagementCommandGateway.cs`、`VillageRuntimeBridge`、Work/Clock/Activity、Save DTO、任何 Prefab/Scene/YAML。

## 3. Prefab 绑定契约（下包 Editor 创建时按此命名）

控制器不建固定 UI、不建 Canvas。要求 Prefab 根节点（建议 `UI/ManagementView`）包含**全屏 Raycast Blocker、模态面板与 TopHUD 允许区**，Blocker/World 输入拦截由 Prefab + Shell 负责；根节点默认 inactive 或由 Shell 控制激活。

### 3.1 固定字段（Inspector 序列化名即绑定键，顺序固定）

| 区域 | 字段名 | 类型 | 说明 / 建议子节点 |
|---|---|---|---|
| 标题与全局动作 | `title` | `TMP_Text` | 标题 |
| | `closeButton` | `Button` | 关闭；点击 → `ManagementView.Close()` → Shell 同一关闭回调 |
| | `saveButton` | `Button` | 显式保存；不自动保存 |
| | `statusMessage` | `TMP_Text` | 命令/保存结果与空态诊断 |
| 页签 | `tabButtons[0..2]` | `Button[]` | **固定顺序**：人口 / 职责与场所 / 资源（仅前 3 个接线） |
| | `populationPage` / `workplacePage` / `resourcePage` | `GameObject` | 三个页根，控制器按 ActiveTab 切换 |
| 人口 | `nameSearch` | `TMP_InputField` | 姓名包含检索（会话级） |
| | `statusFilterButtons[0..2]` | `Button[]` | **固定顺序**：全部 / 在村 / 不在村（仅前 3 个接线） |
| | `populationSummary` | `TMP_Text` | 同一快照的“共 / 显示 / 在村”人数 |
| | `populationScroll` | `ScrollRect` | 人口滚动 |
| | `populationContent` | `RectTransform` | 人口行容器（Vertical Layout） |
| | `populationEmpty` | `TMP_Text` | 明确空态 |
| 职责与场所 | `workplaceSummary` | `TMP_Text` | 岗位/兼容/施工计数 |
| | `workplaceScroll` | `ScrollRect` | 职责滚动 |
| | `workplaceContent` | `RectTransform` | 岗位行容器 |
| | `workplaceEmpty` | `TMP_Text` | 空态 |
| | `constructionContent` | `RectTransform` | 施工行容器；为空时回退到 `workplaceContent` |
| | `constructionEmpty` | `TMP_Text` | 施工空态 |
| 资源 | `resourceSummary` | `TMP_Text` | Ingredients / Food / 在村供餐 / Hungry |
| | `resourceScroll` | `ScrollRect` | 资源滚动 |
| | `resourceContent` | `RectTransform` | Farm/Kitchen 行容器 |
| | `resourceEmpty` | `TMP_Text` | 空态 |
| 行模板 | `personRowTemplate` | `ManagementRowView` | Authored、默认 inactive、放在 Templates 容器 |
| | `workplaceRowTemplate` | `ManagementRowView` | 同上 |
| | `constructionRowTemplate` | `ManagementRowView` | 同上 |
| | `resourceRowTemplate` | `ManagementRowView` | 同上 |
| 详情与操作 | `detailTitle` | `TMP_Text` | 所选人物标题 |
| | `detailBody` | `TMP_Text` | 人物事实 + 岗位候选 + 施工候选 |
| | `actionHint` | `TMP_Text` | 操作提示 / 世界忙 / 远征禁用说明 |
| | `assignButton` | `Button` | 派工（Gateway `AssignToWorkplace`） |
| | `removeButton` | `Button` | 显式撤职（Gateway `RemoveAssignment`） |
| | `addConstructionButton` | `Button` | 加入施工（Gateway `AddConstructionWorker`） |
| | `removeConstructionButton` | `Button` | 撤出施工（Gateway `RemoveConstructionWorker`） |

### 3.2 行模板 `ManagementRowView` 契约

模板自身必须是 `ManagementRowView`，其子节点序列化字段：`label`（TMP_Text）、`detail`（TMP_Text）、`button`（Button）、`background`（Image）、`layout`（LayoutElement，可选）。
运行时只克隆模板并调用 `Bind(title, detail, onClick, interactable, selected, preferredHeight)`；模板本身不在 `populationContent` / `workplaceContent` / `resourceContent` 内，避免被清理逻辑误删（控制器按各自内容键只销毁/重建自己克隆的对应行，刷新不清空其他分区）。

### 3.3 固定子控件层级建议

```text
ManagementView (RectTransform, CanvasGroup, ManagementView)
├─ ModalRoot
│  ├─ Header / Title (TMP_Text)
│  ├─ Close (Button)
│  ├─ Save (Button)
│  ├─ StatusMessage (TMP_Text)
│  ├─ TabBar
│  │  ├─ Tab_Population / Tab_Workplaces / Tab_Resources (Button, 顺序固定)
│  ├─ PopulationPage
│  │  ├─ SearchInput (TMP_InputField)
│  │  ├─ Filter_All / Filter_InVillage / Filter_NotInVillage (Button, 顺序固定)
│  │  ├─ PopulationSummary (TMP_Text)
│  │  ├─ PopulationScroll (ScrollRect → Viewport → Content)
│  │  └─ PopulationEmpty (TMP_Text)
│  ├─ WorkplacePage
│  │  ├─ WorkplaceSummary (TMP_Text)
│  │  ├─ WorkplaceScroll (ScrollRect → Viewport → WorkplaceContent)
│  │  ├─ WorkplaceEmpty (TMP_Text)
│  │  ├─ ConstructionContent (RectTransform)
│  │  └─ ConstructionEmpty (TMP_Text)
│  ├─ ResourcePage
│  │  ├─ ResourceSummary (TMP_Text)
│  │  ├─ ResourceScroll (ScrollRect → Viewport → ResourceContent)
│  │  └─ ResourceEmpty (TMP_Text)
│  └─ DetailPanel
│     ├─ DetailTitle / DetailBody / ActionHint (TMP_Text)
│     └─ Assign / Remove / AddConstruction / RemoveConstruction (Button)
└─ Templates (inactive)
   ├─ PersonRowTemplate / WorkplaceRowTemplate / ConstructionRowTemplate / ResourceRowTemplate (ManagementRowView)
```

Blocker 与 TopHUD 可操作区作为 Prefab 兄弟节点，不属控制器字段；控制器**不创建 Canvas、不 Pause/Speed/SetSimulationActive、不独吞 ESC**。

## 4. 对外接口（下包 Shell 接线签名）

```csharp
public void Initialize(ProductHomeWorldHost host, HomeProductShell shell); // 只调用一次；内部 SetActive(false)
public void RefreshNow();        // Shell 每次在 Management Surface 可见时调用（0.2s 节流可沿用现有 Shell）
public void Close();             // 关闭按钮 → shell.CloseSurface()，与 ESC 同一回调
public Action<string> PersonDetailRequested { get; set; } // 点人物行：SelectPerson 后由 Shell 进入 Person Surface
public bool SelectTab(ManagementViewTab tab);
public bool OpenPerson(string personId);
public bool Save();
public bool AssignSelectedPersonToWorkplace();
public bool RemoveSelectedPersonAssignment();
public bool AddSelectedPersonToConstruction();
public bool RemoveSelectedPersonFromConstruction();
public string SelectedPersonId { get; }              // 只派生自 HomeSelectionContext
public string SelectedWorkplaceId { get; }           // 仅本页会话行候选
public string SelectedConstructionBuildingId { get; }// 仅本页会话行候选
public ManagementViewTab ActiveTab { get; }
public ManagementSnapshot CurrentSnapshot { get; }
public bool IsVisible { get; }
public string StatusText { get; }
```

最小 Shell 接线（**本包未实施**，供下包）：

```csharp
var authored = Resources.Load<ManagementView>("UI/ManagementView");
Management = Instantiate(authored, transform);
// RectTransform 铺满与 PersonDetail 相同
Management.Initialize(host, this);
Management.PersonDetailRequested = id => { host.Selection.SelectPerson(id); OpenSurface(HomeSurface.Person); };
// LateUpdate/RefreshNow 中：
Management.gameObject.SetActive(Router.Surface == HomeSurface.Management && ShellVisible);
if (Management.IsVisible) Management.RefreshNow();
// ESC 仍走 Shell.HandleEscape → CloseSurface；关闭按钮走 Management.Close() → 同一 CloseSurface
```

## 5. Selection 生命周期

- 唯一当前选择：既有 `HomeSelectionContext`。人口行点击 → `SelectPerson(id)`；随后 `PersonDetailRequested(id)` 由 Shell 进入现有 PersonDetail。列表**不保存第二份长期 selectedPerson**，`SelectedPersonId` 每次从 Selection 派生。
- 岗位/施工点击只设置本页会话行候选（`_workplaceCandidate` / `_constructionCandidate`），不反向改写 Selection，因此“人物 + 岗位”可同时存在，不产生第二套 Selection/Person/Authority。
- 世界选中 Building / Place：`LocateSelectionIfNeeded()` 定位到对应职责行或施工行（按真实 `BuildingInstanceId` / `WorkplaceId` / `ConstructionPlaceId` 匹配），切到“职责与场所”页并滚动；多 slot、多同名实例不合并。
- 订阅：`Initialize` 时 `host.Selection.Changed += SelectionChanged`，`OnDestroy` 解绑；不可见时只标记定位失效，不刷新。
- 换 source：`RefreshNow` 用 `ReferenceEquals` 检测 `VillageRuntimeBridge.SimulationPersonSource` 变化 → 清岗位/施工候选、清状态消息、清姓名检索、状态筛选回“全部”、重新绑定 Selection（先解绑旧引用再订阅当前），再重新读取同一快照。
- 候选失效（撤销职导致兼容行消失、施工完成等）在每次快照后剪除，不保留幽灵目标。

## 6. 命令与数据来源

- 一次性只读：每个 `RefreshNow()` 只调用一次 `ManagementReadAdapter.Read()`；人口人数、筛选结果、职责行、施工行、资源行、详情均来自同一 `ManagementSnapshot`。
- 命令全部且只调用既有 `ManagementCommandGateway`：`AssignToWorkplace` / `RemoveAssignment` / `AddConstructionWorker` / `RemoveConstructionWorker` / `SaveWorld`；每次命令 Gateway 内部即时重查，View 不直接写 Assignment/Project/Resource/Save。
- 成功后 `RefreshNow()` 重新读取真实状态；失败只显示 Gateway `Message`（领域诊断原样保留），不自动回滚、不自动 Remove→Assign、不自动保存。
- 保存：`SaveWorld()` 的 bool/Message 结果直接显示为“已保存当前世界。”/“未保存：…”，不把内存变化称为已保存；失败保留当前内存状态。
- 按钮/命令 Guard：`VillageRuntimeBridge.CanSaveWorld`（世界推进/远征忙）、`ManagementWorkplaceRow.CanAssign`（已完成、已注册、未满员）、人物 `IsInVillage`、施工工人真实成员关系；不新增领域规则。远征/不可用/消失人物禁用按钮，Gateway 仍最终校验。成长 pending 时命令返回“请先完成成长选牌或明确跳过”，不绕过嵌套模态。
- 静态兼容岗位：只显示（含真实 WorkplaceId / 名额 / 劳动 / 现场）并可显式撤职；`CanAssign=false`，派工按钮禁用且 Gateway 拒绝，不新增派工。
- 资源页：真实 Ingredients / Food、在村供餐人数、其中 Hungry 人数；Farm/Kitchen 行只显示现有实例名额/劳动并可导航职责；无资源写按钮、无预测/效率排行榜。动态批次/供餐细节缺只读投影，**按规格后置，不伪造**。
- 世界忙/远征时只禁用命令，不自动 Pause/Unlock/SetSimulationActive。

## 7. 编译 / Console / 保护核验（实际回执）

1. `editor | call RefreshAssets()` → `RqlEditorApi@1 | RefreshAssets() | null`（多次执行，最终一次在最终源码之后）。
2. 编译结束状态：`editor | select isCompiling isUpdating isPlaying` → `false | false | false`；`isPlaying=false`，本包未进入 Play。
3. Console 回读：最终 `console | where logType == "Error" or logType == "Exception" | sort sequence desc | first 6` → **0 条错误/异常**（日志缓冲已滚动）；会话中曾出现的 3 条错误/异常均为取证脚本自身临时片段的作用域错误（`snippet.cs ... CS0103: sha`，非项目源码，随后用修正片段成功完成基线核验），**没有 Assembly-CSharp 编译错误/异常**。
4. 编译新鲜度：`Assembly-CSharp.dll` 写入 `2026-10-23T12:06:17.3885898Z`，源码最终写入 `2026-10-23T12:06:15.1653305Z` → `fresh=True`；`ManagementView` / `ManagementRowView` 类型实际存在，序列化字段 35 个。
5. 保护核验：34/34 基线条目 SHA256 + bytes 全部一致（`same=34 diff=0 missing=0`），未恢复 HEAD 旧备份；`Assets/Editor/StudioRecoveryBackups/2026-10-23T11-49-56-028Z` 及其 meta 未触碰。
6. 场景仍为 `SampleScene` 且 `isDirty=false`。

## 8. 未测 / 明确留待下包

- 未创建 Prefab、未创建 Blocker、未改 Scene、未接入 HomeProductShell/Router/PersonDetail 导航，因此**未做任何运行时/Play 验证**：页签切换、行克隆、筛选、命令按钮、Selection 定位、ESC/关闭、TopHUD 可操作、World 输入拦截均待下包 Play 实证。
- 按授权未跑 `tests-run`、未跑全量回归；现有 `ManagementProjectionTests` 的 13/13 通过为开工前置事实，本包未改写测试。
- 资源批次/供餐细节、动态只读投影未实现（规格允许后置）。
- 下包最小接线步骤：
  1. Editor 创建 `UI/ManagementView` Prefab，按 §3 绑定全部 35 个字段与 4 个行模板；配置全屏 Blocker 与 TopHUD 允许区。
  2. `HomeProductShell` 加载/实例化 Prefab，调用 `Initialize(host, this)`，设置 `PersonDetailRequested`，在 `LateUpdate`/`RefreshNow` 按 `Router.Surface==Management && ShellVisible` 激活并刷新。
  3. 移除/替换 Management 白模正文（`RefreshSurface` 中的临时文案），保留 TopHUD/底部导航/World 背景，不加第二 Canvas。
  4. 真实 Play 专项：人物→详情返回、Building/Place 定位、同建筑多 slot 不合并、派工/AlreadyAssigned/满员、远征/世界忙禁用、撤职、施工加入/撤出保留 Primary Assignment、保存失败不称成功、ESC/关闭一致、嵌套成长 pending 不被绕过。
  5. 专项通过后再跑完整回归；不要以本报告代替最终 R3-B 验收。

## 主开发复核更正

本节只更正保护项分类，不改写上文历史记录，也不改任何历史工具日志；上文（尤其 §1 “14 项保护类：字体 1 + NuGet 插件 10 + Packages 2 + ProjectSettings 1”）保留为原始历史描述，并明确标记其中“14 项”的构成属**误述**，最终交付不得沿用该错误分类。

- 制作人指定的 **14 项**实际应为：**TMP 字体 1 项 + Growth 12 项 + ProjectSettings 1 项**。
  - TMP 字体：`Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`。
  - Growth 12 项：`Nodes` 4（Lv2 CardGrowth / Lv3 StarterSlotUnlock / Lv4 TraitUpgrade / Lv5 UnlockedPoolExpand）+ `TestGrowthCurve` 1 + `TestGrowthTable` 1 + `TestTraitCatalog` 1 + `Traits` 5（trait_char_001..004、trait_hero_001）。
  - ProjectSettings：`ProjectSettings/ProjectSettings.asset`。
- `NuGet` / `Packages` 相关文件确实属于 **34 项保护基线集合**，但**不是制作人指定的 14 项**；不得把它们混入 14 项保护类别。
- 上述 14 项已在 `prefab-binding-report.md` §9 逐项复核 SHA256，收工前后均与 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 一致（34/34 same，diff=0，missing=0）。
- 本条为追加更正，不修改原 §1–§8 的历史正文，也不修改任何既有工具日志/命令回执。
