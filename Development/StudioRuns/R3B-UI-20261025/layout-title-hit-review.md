# R3-B 布局专项：标题 top-hit 失败根因与夹具语义修正（阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 实测时刻：2026-10-25T15:40:52Z（本机 23:40:52 +08:00）。
- 范围：只修布局用例的标题语义（`ManagementShellPlayTests.cs` 的新增布局 helper 与调用），不改产品 / Prefab / Scene / 字体 / 原 11 例 / guard。
- 结论口径：**已完成失败对象契约核验与最小夹具修正，编译通过、目标唯一；本轮未运行测试，不宣称通过。** 结果交宿主 Studio 阶段单方法串行复验。

---

## 1. 先核保护面与现场（全部只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | 干净，可安全刷新 |
| guard 根 | **175** 个 run 目录：`restored=174`、`protective-snapshot=1`、`captured=0`、`restore-failed=0`、其它=0、无 manifest=0 | 无未知/捕获未恢复 |
| 最新 guard run | `20261025T153712Z`：`status=restored`、`readbackVerified=true`、`restored=true`、`lastError=null`，备份与本槽同为 `13405B / 4b2b56f6…d6aa` | 该布局运行已恢复，无待恢复项 |
| 单槽存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**（mtime `2026-10-25T15:37:13.5226921Z`，即 guard 恢复时刻）；无 `.tmp` | 内容与要求值一致 |
| 34 项 workspace 基线 | 按 `R3B-20261022/workspace-baseline.json` 逐项 bytes+SHA256：**total=34 / same=34 / diff=0 / missing=0** | 全一致 |
| 受保护字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 测试源码（修正前） | `316260B / 26b43bf8…599f`，mtime `15:30:57Z` | 与主开发固定值一致 |
| Prefab / 产品源码 | `ManagementView.prefab` `282097B / ade4c7e1…`；`ManagementView.cs` `37518B / 908451ea…` | 本包未改动 |
| 原运行分辨率 | `LAYOUT-START originalScreen=1920x1080 mode=Windowed`；当前 `Screen=1920x1080`、`Windowed`、`isPlaying=false` | 与原始值相同，无残留分辨率 |

**分辨率恢复证据说明**：该失败用例的 `finally` 中 `RESOLUTION-RESTORE` 标记未落盘（文件日志与 Console 均无），原因是断言在嵌套 `IEnumerator` helper 内抛出后用例协程被中止，`finally` 未执行；但本次请求值等于原值（1920x1080），当前实测 `Screen` 仍为 `1920x1080/Windowed`，主开发侧 `scope-restored` 亦已核。此点为夹具异常路径的既有行为，本包不改其控制流，仅如实记录；若宿主复跑在 1280 阶段失败，需按同一证据口径复核分辨率是否回到原值。

## 2. 原失败回执（宿主已核）

`testResults` 唯一 1 条：

- 名称：`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`
- `status=Failed`、`durationMs=1149`
- 消息：`管理布局 @1920x1080·Population·标题：top hit 不是目标（玩家点到的不是它）`
- hits：`0. ModalPanel | Canvas=ProductHomeShell#70 | Button=null | active=True`、`1. ModalBlocker | ...`

布局证据日志 `layout-input-evidence/layout-input-evidence.log`（`870B`）止于 Population 截图：

- `RESOLUTION`：1920x1080，`canvasPixelRect=1920x1080`、`canvasScaleFactor=1`、`canvasRootRect=1920x1080`
- `MODAL`：`top=ModalBlocker`（World 射线被管理模态阻断）
- `HIT`：TopHUD `top=Pause`
- `SCREENSHOT`：`management-population-1920x1080.png`，`184019B`，SHA256 `1135df56180d310bf21ea8b36dc31e473c1554540a6b5455e0dfd90c19482ad2`

截图可见 Population 页：标题为左上纯文本“集中管理 · 人口 / 职责与场所 / 资源”，不在任何按钮外观内。

## 3. 根因：把静态展示标题当成可交互控件

**authored Prefab 实测**（`Assets/Resources/UI/ManagementView.prefab`）：

| 对象 | 组件 | 实测契约 |
|---|---|---|
| `ModalPanel/Title` | `TextMeshProUGUI` | `m_RaycastTarget: 0`（第 2372 行）、文本“集中管理 · 人口 / 职责与场所 / 资源”、`m_fontSize: 30`、无 `Button`、无 `IPointerClickHandler` |
| `ModalPanel/Save`、`Close`、`Tab_Population/Workplaces/Resources`、`Filter_All/InVillage/NotInVillage`、`AssignButton`、`RemoveButton`、`AddConstructionButton`、`RemoveConstructionButton` | `Image` + `Button` | `m_RaycastTarget: 1`、`Button interactable=true` |
| `PopulationPage/NameSearch` | `Image` + `TMP_InputField` | `m_RaycastTarget: 1` |

原断言链（修正前第 1778 行）：

```
AssertFieldControl(management, "title", what + "·标题", false)
  → AssertControlHit(rect, ...)
    → AssertTopHitBelongs(rect, ...)   // 要求 top raycast 命中 Title 自身
```

Title 的 `raycastTarget=0`，射线必然穿透到其父 `ModalPanel`（`Image raycastTarget=1`），因此 top hit 恒为 `ModalPanel`。**原断言对不可交互标题错误地要求 top-hit，属于夹具语义错误，不是产品遮挡或布局缺陷。**

## 4. 最小修正：新增标题专用 helper 并替换原调用

只改 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：

1. 新增 `AssertDisplayTitle(object view, string fieldName, string what)`（第 1776–1814 行）。
2. `AssertTabLayout` 中仅将标题一行由 `AssertFieldControl(management, "title", …, false)` 改为 `AssertDisplayTitle(management, "title", what + "·标题")`（第 1818 行）。
3. 原 11 例、`AssertControlHit`、`AssertFieldControl`、`AssertTabLayout` 其余内容、guard、产品代码、Prefab、Scene、字体均未改。

`AssertDisplayTitle` 对静态标题的独立核验点：

- 字段必须绑定 `TMP_Text`；
- `AssertInsideViewport`：`activeInHierarchy`、宽高 > 1、真实世界四角换算后完整位于当前 `Screen` 视口内（沿用真实 Canvas/相机换算，不做 RectTransform 缩放冒充）；
- 文本 `Trim` 后非空；`fontSize > 1`；
- 尺寸：矩形高度 ≥ 一行字号，宽度 ≥ 2 个字号；`GetPreferredValues` 排版宽度/高度必须被矩形容纳（不截断）；
- 不得是 `Button`；
- 标题中心执行真实 `EventSystem.RaycastAll`：必须有命中，且 top hit 必须属于 `ModalPanel`（排除被其它 Surface 图层压住）；
- top hit 及其父链不得有 `IPointerClickHandler`（排除被可点击控件压住）；
- 写 `TITLE` 证据：文本、`raycastTarget`、top hit 名称，并明确“不要求自身 top-hit”。

该 helper **不跳过、不放宽任何交互控件断言**：按钮 / 输入 / 页签 / 滚动行仍走原有 `AssertControlHit`、`RealPointerClick`、`ClickChild`、`AssertRowReachable`、`AssertScrollReachable`，继续要求真实 top-hit 与 `PointerClick`/`IScrollHandler`，并保留 1920/1280 的真实 `Screen`/`Canvas` 尺寸断言与真实 EventSystem scroll 事件（不直接写 `normalizedPosition`）。

## 5. 编译与目标唯一（安全刷新后）

- 已确认场景干净后调用 `AssetDatabase.Refresh(ForceSynchronousImport)`；编译结束：`EditorUtility.scriptCompilationFailed=False`、`isCompiling=False`、`isUpdating=False`。
- 修正后源码：`319153B / f9dc6dd5284783ba3d99cdf204cfff2a0be5aff6aedbcae0eb1f66a8a9830985`，mtime `2026-10-25T15:39:43.5725228Z`。
- `BattleStage1.PlayModeTests.dll`：`470528B`，mtime `2026-10-25T15:39:49.8458213Z`（晚于源码）；pdb `118740B`。
- 反射与 RQL `tests` 均确认：类内 `[UnityTest]` = **12**（原 11 + 布局 1）；`ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable` **唯一**，带 `UnityTestAttribute`；`AssertDisplayTitle` 已进入程序集。
- Console 无新增 `error CS`；仅历史 `Screen.SetResolution` 过时警告与既有 MCP 片段历史错误（时间早于本次修正）。

## 6. 本阶段边界与实际副作用

- **未运行** `tests-run`，未启动/忙等 TestRunner；无新测试回执，不填 PASS。
- 未 `rql-mutate`、未 Shell/`Process.Start`、未 Git 写、未全局 `SaveAssets/SaveChanges`、未保存场景/Prefab、未改分辨率、未启停播放、未 Save/Load。
- 未安装组件、未改权限、未创建付费子 Agent、未访问授权目录外工程或认证目录。
- 写入文件仅 2 个：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（新增 helper + 替换标题调用）；
  2. 本文件。
- 存档被原运行恢复为同内容（`13405B / 4b2b56f6…`）；34 项与字体保持一致。

## 7. 移交宿主单方法复验

交 Studio 阶段**仅串行执行**：

`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`（PlayMode）

复验时回读：`layout-input-evidence.log` 的 `TITLE` / `HIT` / `ROW` / `SCROLL` / `RESOLUTION` / `RESOLUTION-RESTORE` / `RESOLUTION-RESTORE-VERIFY`、1920 与 1280 截图、单槽 `13405B / 4b2b56f6…`、34 项基线、字体 `3c758e16…`、场景 clean。若后续交互控件被真实遮挡，保留失败并定位产品，不以夹具放宽掩盖。
