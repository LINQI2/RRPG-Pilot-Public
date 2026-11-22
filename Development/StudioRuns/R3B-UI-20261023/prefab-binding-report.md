# R3B-UI ManagementView Prefab 创建与绑定回读报告（2026-10-23）

状态：**阶段 1/1 完成（仅证明 Prefab 资产存在且绑定结构正确）/ 待主开发专业核验**。
本包通过已连接 Unity Editor 创建唯一 `Assets/Resources/UI/ManagementView.prefab`，按既有 `ManagementView.cs` 的 35 个 `[SerializeField]` 契约绑定固定 UI、4 个行模板、滚动与全屏 Blocker，并重新加载 Prefab 逐项回读。
**未接 Shell / 未 Play / 未 tests-run / 未做全量回归 / 未改任何场景 / 不改控制器、投影、命令、测试。**
本报告只是 Prefab 绑定结构证据，不声称产品模态、交互或 Play 通过。

## 1. 授权与环境前置

- 唯一工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`；Unity 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`。
- Unity 实际连接：`2022.3.62t9`，`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`；未启动/关闭 Editor。
- 开工/收工 `editor | select isPlaying isPaused isCompiling isUpdating` → `false | false | false | false`。
- 当前场景始终为 `SampleScene | Assets/Scenes/SampleScene.scene | isDirty=false | rootCount=5`；本包没有写入、保存或切换场景。
- 未安装组件、未改权限/ACL/管理员 FullAccess、未动 Packages/字体/插件/正式工程、未 commit/push/stage/切分支/改远程、未 Shell/Process.Start、未访问其他工程或认证目录。
- 未读取或运行测试；开工前主开发已专业回读 `ManagementProjectionTests` 13/13 Passed（记录 `0f7051f6…`），本包不重跑。
- 创建方式：`script-execute` 内**有界 Editor 创建脚本**。先在独立 `NewPreviewScene()` 中用 Editor API 创建 GameObject/RectTransform/UGUI/TMP 组件，`PrefabUtility.SaveAsPrefabAsset` 保存，再 `LoadPrefabContents` + `SerializedObject` 绑定字段并保存；关闭预览场景、销毁临时对象。**没有直接文本/YAML 编辑 Prefab。** 创建前确认目标路径不存在，未覆盖任何已有未知改动。

## 2. 实际产物与哈希

| 项 | 实际值 |
|---|---|
| 资产路径 | `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` |
| 字节 | 282097 |
| SHA256 | `ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11` |
| AssetDatabase GUID | `4a54a9f0d1e97634a93836f4566fd5ce` |
| meta 文件 | `.../ManagementView.prefab.meta`，178 字节，SHA256 `b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559` |
| meta 序列化 guid | `CysasXukBXvOKR5CYXJYu30jbFiMEGHh6qKmQjA1bytPPAwAZC7GuLw=`（该工程自定义 GUID 编码；AssetDatabase 解析为上行 32 位 GUID） |
| 根组件 | `RectTransform, CanvasGroup, ManagementView`；**Canvas=0、GraphicRaycaster=0**（不建第二 Canvas，挂到既有 Home Canvas 下） |
| 根 RectTransform | anchorMin `(0,0)` / anchorMax `(1,1)` / pivot `(0.5,0.5)` / sizeDelta `(0,0)`，由 Shell 按既有 PersonDetail 方式铺满 |
| 控制器脚本 GUID | prefab 根 `m_Script guid: 4a7dfebd6d5993c499d4f9a186b7d2c0`（`ManagementView.cs`）；行模板 `f37df54fb96a70c45864d60504325b01`（`ManagementRowView.cs`） |
| 未改控制器 | `ManagementView.cs` 37518B `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d`；`ManagementRowView.cs` 1670B `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd` |

## 3. 35 字段回读（PrefabUtility.LoadPrefabContents + SerializedObject）

回读汇总：`SERIALIZED_FIELDS=35`，`NON_NULL_REFS=39`（33 个单引用 + 2 个数组各 3 元素），**无 null**。`MISSING_SCRIPTS=0`。数组长度/顺序与控制器固定枚举一致。

| # | 字段 | 类型 | 实际绑定目标（Prefab 层级） |
|---:|---|---|---|
| 1 | `title` | TMP_Text | `ModalPanel/Title` |
| 2 | `closeButton` | Button | `ModalPanel/Close` |
| 3 | `saveButton` | Button | `ModalPanel/Save` |
| 4 | `statusMessage` | TMP_Text | `ModalPanel/StatusMessage` |
| 5 | `tabButtons[0..2]` | Button[3] | 顺序 `Tab_Population` / `Tab_Workplaces` / `Tab_Resources`（人口 / 职责与场所 / 资源） |
| 6 | `populationPage` | GameObject | `ModalPanel/PopulationPage`（默认 active） |
| 7 | `workplacePage` | GameObject | `ModalPanel/WorkplacePage`（默认 inactive） |
| 8 | `resourcePage` | GameObject | `ModalPanel/ResourcePage`（默认 inactive） |
| 9 | `nameSearch` | TMP_InputField | `ModalPanel/PopulationPage/NameSearch` |
| 10 | `statusFilterButtons[0..2]` | Button[3] | 顺序 `Filter_All` / `Filter_InVillage` / `Filter_NotInVillage`（全部 / 在村 / 不在村） |
| 11 | `populationSummary` | TMP_Text | `ModalPanel/PopulationPage/PopulationSummary` |
| 12 | `populationScroll` | ScrollRect | `ModalPanel/PopulationPage/PopulationScroll` |
| 13 | `populationContent` | RectTransform | `.../PopulationScroll/Viewport/PopulationContent` |
| 14 | `populationEmpty` | TMP_Text | `ModalPanel/PopulationPage/PopulationEmpty` |
| 15 | `workplaceSummary` | TMP_Text | `ModalPanel/WorkplacePage/WorkplaceSummary` |
| 16 | `workplaceScroll` | ScrollRect | `ModalPanel/WorkplacePage/WorkplaceScroll` |
| 17 | `workplaceContent` | RectTransform | `.../WorkplaceScroll/Viewport/WorkplaceScrollContent/WorkplaceContent` |
| 18 | `workplaceEmpty` | TMP_Text | `.../WorkplaceScrollContent/WorkplaceEmpty` |
| 19 | `constructionContent` | RectTransform | `.../WorkplaceScrollContent/ConstructionContent` |
| 20 | `constructionEmpty` | TMP_Text | `.../WorkplaceScrollContent/ConstructionEmpty` |
| 21 | `resourceSummary` | TMP_Text | `ModalPanel/ResourcePage/ResourceSummary` |
| 22 | `resourceScroll` | ScrollRect | `ModalPanel/ResourcePage/ResourceScroll` |
| 23 | `resourceContent` | RectTransform | `.../ResourceScroll/Viewport/ResourceContent` |
| 24 | `resourceEmpty` | TMP_Text | `ModalPanel/ResourcePage/ResourceEmpty` |
| 25 | `personRowTemplate` | ManagementRowView | `Templates/PersonRowTemplate`（activeSelf=false） |
| 26 | `workplaceRowTemplate` | ManagementRowView | `Templates/WorkplaceRowTemplate`（activeSelf=false） |
| 27 | `constructionRowTemplate` | ManagementRowView | `Templates/ConstructionRowTemplate`（activeSelf=false） |
| 28 | `resourceRowTemplate` | ManagementRowView | `Templates/ResourceRowTemplate`（activeSelf=false） |
| 29 | `detailTitle` | TMP_Text | `ModalPanel/DetailPanel/DetailTitle` |
| 30 | `detailBody` | TMP_Text | `ModalPanel/DetailPanel/DetailScroll/Viewport/DetailScrollContent/DetailBody` |
| 31 | `actionHint` | TMP_Text | `ModalPanel/DetailPanel/ActionHint` |
| 32 | `assignButton` | Button | `ModalPanel/DetailPanel/AssignButton`（派工） |
| 33 | `removeButton` | Button | `ModalPanel/DetailPanel/RemoveButton`（撤职） |
| 34 | `addConstructionButton` | Button | `ModalPanel/DetailPanel/AddConstructionButton`（加入施工） |
| 35 | `removeConstructionButton` | Button | `ModalPanel/DetailPanel/RemoveConstructionButton`（撤出施工） |

四个页根与详情面板互斥/共存关系由控制器 `ApplyTabs` 驱动；Prefab 不自行切页。

## 4. 行模板回读（4/4）

| 模板 | 高度(LayoutElement) | 子绑定 | activeSelf | 容器 |
|---|---:|---|---|---|
| `PersonRowTemplate` | 86 | label/detail/button/background/layout 全部非空 | false | `Templates`（父容器也 inactive） |
| `WorkplaceRowTemplate` | 100 | 同上 | false | 同上 |
| `ConstructionRowTemplate` | 92 | 同上 | false | 同上 |
| `ResourceRowTemplate` | 84 | 同上 | false | 同上 |

- 每个模板根：`RectTransform + Image + Button + LayoutElement + ManagementRowView`；`Button.targetGraphic = background Image`，整行为点击区。
- 子节点 `Label`（TMP，21pt）/`Detail`（TMP，17pt，自动换行）/`Chevron`（`›`），文本 `raycastTarget=false`；背景色 `.14,.24,.24,1`，选中色由 `ManagementRowView.Bind` 改为 `.28,.43,.36,1`。
- 模板位于 `Templates`，不混入 `PopulationContent` / `WorkplaceContent` / `ResourceContent`，控制器只克隆，不会被自身清理逻辑误删。

## 5. TMP_InputField 回读

`ModalPanel/PopulationPage/NameSearch`：

- `textViewport = NameSearch/Text`（有效 RectTransform，stretch 内缩 12/8）
- `textComponent = NameSearch/Text` 上的 `TextMeshProUGUI`（可编辑，单行，`characterLimit=64`）
- `placeholder = NameSearch/Placeholder` 上的 `TextMeshProUGUI`（提示“按姓名检索”）
- `interactable=true`，`lineType=SingleLine`，`text=''`，背景 Image 为 `targetGraphic` 且 `raycastTarget=true`
- 不是仅放占位文本；控制器 `onValueChanged` 有真实输入承载。

## 6. 滚动与布局回读

四个滚动区均为：`ScrollRect → Viewport(Image + Mask, showMaskGraphic=false) → Content(VerticalLayoutGroup + ContentSizeFitter)`；`horizontal=false`、`vertical=true`、`movementType=Clamped`、`scrollSensitivity=35`。

| 滚动区 | Viewport/Mask | Content | VLG + Fitter | 说明 |
|---|---|---|---|---|
| `PopulationScroll` | true/true | `PopulationContent` | true/true | 长人口列表可滚动 |
| `WorkplaceScroll` | true/true | `WorkplaceScrollContent` | true/true | 岗位 + 施工合一滚动区 |
| `ResourceScroll` | true/true | `ResourceContent` | true/true | 长资源列表可滚动 |
| `DetailScroll` | true/true | `DetailScrollContent` | true/true | 详情正文过长可滚动 |

- `WorkplaceScrollContent` 子序列：`WorkplaceHeading`(30) → `WorkplaceContent`(嵌套 VLG+Fitter) → `WorkplaceEmpty`(48) → `ConstructionHeading`(30) → `ConstructionContent`(嵌套 VLG+Fitter) → `ConstructionEmpty`(48)。
- 因此 `constructionContent` 位于 `WorkplaceScroll` 的 `Viewport` 内容流中，**施工行在可滚动可见区域内**，不会堆到屏幕外的固定布局；岗位与施工共用该滚动区。
- 滚动 Content 的 `sizeDelta` 运行时由 `ContentSizeFitter` 驱动，Prefab 中记录为 `(0,0)` 属正常。
- 面板与固定区域（1920×1080 参考，CanvasScaler 与既有 Home 一致 `ScaleWithScreenSize / 1920×1080 / match 0.5`）：
  - `ModalPanel`：左右 24、上 96、下 88 → 1872×896；上边留出 96px 给 TopHUD，下边留 88px。
  - 标题 `(24,-18) 900×48`；保存 `(1592,-16) 124×44`；关闭 `(1724,-16) 124×44`；状态 `(24,-68) 1480×28`。
  - 页签 `(24/252/480,-104) 220×46`；页根 `(24,-164) 1288×712`；详情 `(1330,-164) 518×712`。
  - 人口：检索 `(12,-12) 360×44`、筛选 `384/542/700,-14 150×40`、汇总 `(12,-66) 1264×28`、滚动 `(12,-102) 1264×596`、空态覆盖 `(12,-330) 1264×64`。
  - 职责与场所：汇总 `(12,-12)`、滚动 `(12,-48) 1264×650`；施工区随内容流滚动。
  - 资源：汇总 `(12,-12)`、滚动 `(12,-48) 1264×650`、空态 `(12,-330)`。
  - 详情：标题 `(16,-14) 486×40`、正文滚动 `(16,-62) 486×390`、提示 `(16,-462) 486×96`、四按钮 `(16/265,-566/620) 237×48`。
  - 页根/详情底边到面板底 20px，页签下沿 -150 与页根上沿 -164 不重叠；详情四按钮最低到 -668，仍在 712 高面板内。
  - 1280×720 下由既有 CanvasScaler 等比缩放（match 0.5 → 约 0.667），固定像素偏移同比缩放；TopHUD 预留与按钮/正文相对关系不变。
- 文本使用当前共享 TMP 字体 `RRPG_SourceHanSansCN_Regular_TMP`（`CurrentUIFontAuthority.Default`，guid `f9418c72505f4cb41ab51da03f9fd2e0`），40 个 TMP 文本全部指向该字体；未修改字体资产/动态图集/材质。

## 7. Blocker 与 TopHUD 预留

- `ModalBlocker`：`anchorMin=(0,0)`、`anchorMax=(1,1)`、`offsetMin=(0,0)`、`offsetMax=(0,-88)`，Image `raycastTarget=true`，半透明黑 `.58`。在 1920×1080 下实际覆盖 **1920×992**（从屏幕底到距顶 88px），全屏遮挡世界背景射线，避免面板只覆盖局部造成穿透。
- 顶部 **88px** 为既有 TopHUD 可操作区：既有 `HomeProductShell` TopHUD 为顶部锚点、位置 `(0,-16)`、高 62px，实际占屏幕顶 16..78px；88px 洞口完整包住并留 10px 余量，未重做 TopHUD。
- `ModalPanel` 顶边再内缩到 96px（1920×1080 下顶边 y=984），比 Blocker 顶边 y=992 再低 8px：面板正文不与 TopHUD 重叠，且面板与遮罩在 984..992px 有 8px 重叠，不产生“未遮挡条带”；88..96px 条带只有遮罩、无交互控件，不影响 TopHUD。
- 层次沿用既有 PersonSurface 的“全屏遮罩 + 顶部让位”思路；本 Prefab 不创建第二个 Canvas/GraphicRaycaster，而由 HomeProductShell 现有 Canvas 承载，实例化顺序在 TopHUD 之后可正常盖住世界、保留 TopHUD 自身点击（88px 区域不在 Blocker 矩形内）。
- 本包只交付固定 Blocker 结构；**真正 World 键盘/镜头拦截由下一 Shell 包完成**，本包未改 `HomeProductShell`、未接入模态激活、未验证运行时输入。

## 8. 锚点/射线/缺字与可访问性

- 所有固定控件使用明确的 `anchorMin/anchorMax/pivot`；页根与详情为左上固定尺寸，标题/按钮/滚动均有确定矩形，没有依赖运行时程序化搭建。
- `raycastTarget`：Blocker/Panel/页面背景/按钮/滚动 Viewport 为 true；TMP 文本与空态为 false，避免文本吞点击；行模板整行 Button 可点击。
- TMP 字体：`RRPG_SourceHanSansCN_Regular_TMP`，`atlasPopulationMode=Dynamic`、图集 1024×1024、源字体 `SourceHanSansCN-Regular`、当前 lookup 644 字。
- **缺字记录（只读检查，未 rebuild，未改字体）**：当前图集 lookup 未预烘 14 个字符：`›(U+203A)、供(U+4F9B)、兼(U+517C)、威(U+5A01)、就(U+5C31)、按(U+6309)、既(U+65E2)、显(U+663E)、权(U+6743)、直(U+76F4)、绪(U+7EEA)、说(U+8BF4)、题(U+9898)、餐(U+9910)`。因字体为 Dynamic，运行时通常可按需补入；**本包不声称所有中文已渲染通过，留待下一 Shell/Play 实证**。未执行 `TMP_FontAsset` 重建、未改受保护字体/材质/图集。

## 9. 保护核验（前后 SHA256）

- 基线文件：`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，固定 **34 项**。
- 开工前：`total=34 same=34 diff=0 missing=0`。
- 收工后：`total=34 same=34 diff=0 missing=0`；未覆盖 HEAD、未恢复旧备份、未改动恢复备份 `Assets/Editor/StudioRecoveryBackups/2026-10-23T11-49-56-028Z` 及其 meta。
- 制作人指定 **14 项**实际构成：TMP 字体 1 + Growth 12（Nodes 4 + Curve 1 + Table 1 + Catalog 1 + traits 5）+ ProjectSettings 1。本次逐项复核全部一致：

| 保护项 | SHA256 |
|---|---|
| `Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |
| `Assets/Resources/ScriptableObjects/Growth/Nodes/GrowthNode_Lv2_CardGrowth.asset` | `e574de08c1eadd6752a324f5bd4a5ea5a8965e6776b7ce35c43ba465e2d56a90` |
| `.../Nodes/GrowthNode_Lv3_StarterSlotUnlock.asset` | `d840cfaf729e3d4a63cb4968b2282a554bcf3bf35b65f5ba34534a635a7d8b95` |
| `.../Nodes/GrowthNode_Lv4_TraitUpgrade.asset` | `3a3a3bee19437fa3263eacf3f3e229d92fa6b898a71812a3f8906640634ca42c` |
| `.../Nodes/GrowthNode_Lv5_UnlockedPoolExpand.asset` | `12940fab0d1dd0c41c2edaa652fe26298cc1d0b5ff5dcf7a3be64f8ad28f3001` |
| `.../Growth/TestGrowthCurve.asset` | `b0d918dd090f01f31994ef2303101e229cb804ddf5733a36051f6ba158833127` |
| `.../Growth/TestGrowthTable.asset` | `c119259d540f8186b6a0ff5f3e0ceed5a035a8832c528be4e8c15e55b905e139` |
| `.../Growth/TestTraitCatalog.asset` | `101c2c49783d6055d5f7f32a5d7fd5324b597fcc80676e6a94c9b671f4bd894f` |
| `.../Traits/trait_char_001.asset` | `40f4703aa04ea08f62c96ef045f5f5b4db62f3addd73de3fe45deccf6c088568` |
| `.../Traits/trait_char_002.asset` | `17b52c45dc93e04d3f9ec87dc53651b37d6bb32ea0c0a5a26b0e544c772adfd2` |
| `.../Traits/trait_char_003.asset` | `4a44eff01fc7b16e460ee68ee46fbd25720dd427a8954f9663e9e39e2c094bfb` |
| `.../Traits/trait_char_004.asset` | `41836550ce86568e419ba0050d87b1b8974547b29422f83cf710fc5dc81bef31` |
| `.../Traits/trait_hero_001.asset` | `fc5455b6db3085ab63db14b0488a7881123a3fb8863e89617ab7dd8bf9577974` |
| `ProjectSettings/ProjectSettings.asset` | `ca2848951b63101976e9984f89a462df0d64f8ab46f6f572a396ed6943e29280` |

> 分类更正：`NuGet`/`Packages` 属于 34 项保护基线集合，但**不属于制作人指定的 14 项**。原 `controller-report.md` §1 把 14 项写成“字体 1 + NuGet 10 + Packages 2 + ProjectSettings 1”属误述，已在原报告末尾追加“主开发复核更正”，历史原文保留。

## 10. Console 核对（如实记录，不称全清）

- 收工 `console | where logType == "Error" or logType == "Exception"` 最近记录：
  - `sequence 17202 Error: "Destroying assets is not permitted to avoid data loss..."`：本包创建脚本在保存后对 `SaveAsPrefabAsset` 返回的 **Prefab asset 句柄**调用了 `DestroyImmediate`，Unity 拒绝销毁并记录该错误；资产未被销毁，随后 `LoadPrefabContents` 完整回读与哈希复核均通过。属执行器自身多余调用，不是项目源码错误。
  - `sequence 17196–17201`：本包两版草稿 `script-execute` 的 C# 片段参数类型写错（`GameObject` → `Transform`）导致的编译失败；修正后成功。**不是 Assembly-CSharp/项目源码编译错误**，未改项目脚本。
- 收工 `isCompiling=false`、`isUpdating=false`、`isPlaying=false`；没有 Play、没有 tests-run。
- 本包未清空 Console（保留真实证据），因此不能声称 Console 0 error；上述错误均已定位为执行器临时脚本/句柄，**唯一新增资产通过独立回读与哈希验证**。

## 11. 未测 / 未做（边界）

- 未接 `HomeProductShell`/Router、未替换 Management 白模、未加载 Resources、未实例化进产品路径。
- 未 Play，未验证页签切换、行克隆、筛选、命令按钮、Selection 定位、ESC/关闭、TopHUD 可点击、世界输入拦截、Dynamic 字体运行时补字。
- 未运行 `ManagementProjectionTests` 或任何测试；13/13 为开工前既有事实，本包未改写测试。
- 未启动/关闭 Editor，未改 SampleScene，未改控制器/投影/命令/测试源码。
- 未做截图：MCP 截图需目标位于已加载场景，为避免触碰用户现场与进入 Play，本包只用 Prefab 内容回读与几何参数审计留存证据；如需视觉快照，应在下一 Shell 包用受控预览场景单独取证。

## 12. 下一 Shell 包精确加载/接线接口

```csharp
// 既有 HomeProductShell.Initialize 内，PersonDetail 同层，不新建 Canvas
var authored = Resources.Load<ManagementView>("UI/ManagementView");
if (authored == null) { Debug.LogError("[Management] Missing authored UI/ManagementView prefab."); return; }
Management = Instantiate(authored, transform);
Management.name = "ManagementView";
var r = Management.GetComponent<RectTransform>();
r.localScale = Vector3.one;
r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
r.sizeDelta = Vector2.zero; r.anchoredPosition = Vector2.zero;
Management.Initialize(host, this);
Management.PersonDetailRequested = id => {
    host.Selection.SelectPerson(id);
    OpenSurface(HomeSurface.Person);
};
// LateUpdate / RefreshNow：
Management.gameObject.SetActive(Router.Surface == HomeSurface.Management && ShellVisible);
if (Management.IsVisible) Management.RefreshNow();
// ESC 仍走 Shell.HandleEscape → CloseSurface；关闭按钮已绑定 Management.Close() → 同一 CloseSurface。
```

- 资源路径固定：`Resources.Load<ManagementView>("UI/ManagementView")`。
- 与既有 `PersonDetailView` 同 Canvas、同铺满方式；**不要**再给 Management 加 Canvas 或 GraphicRaycaster。
- Blocker 顶部 88px 让位为既有 TopHUD；Shell 激活 Management 时不要移动/重建 TopHUD。
- 真正 World 键盘/镜头拦截、模态互斥、ESC 统一由下一 Shell 包完成；本 Prefab 只提供固定结构。

## 13. Artifact 门禁声明

本报告存在即满足本包 artifact 门禁；门禁**只证明报告文件存在**，不充当功能测试，也不代表 R3-B 或产品验收通过。下一阶段由 Studio 按阶段验证门禁继续；本包不越过门禁声称交互/Play 通过。
