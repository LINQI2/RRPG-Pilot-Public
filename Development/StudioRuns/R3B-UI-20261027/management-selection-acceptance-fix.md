# R3-B 集中管理：人口行选人 / 独立详情入口 / 四命令用户路径返修（2026-10-28，阶段 1/1）

## 1. 本包范围与边界

- 只做 Pilot 文件读写：`ManagementView.cs`、`ManagementView.prefab`、`ManagementShellPlayTests.cs`、本报告。
- 未调用 Unity / MCP（含只读查询）、AssetDatabase.Refresh、编译、Prefab 保存或加载、tests-run、Shell、Git、Prefs、任何字体操作；未连接正式工程，未读写用户目录与用户存档。
- 未改 Authority、未改保护/断言口径、未新增额外付费子 Agent；字体 pending 与宿主 Atlas6 dirty 留 Studio 侧处理。

## 2. 修复的验收缺陷

| 缺陷 | 修复 |
| --- | --- |
| 人口行单击被强制导航到 Person Surface，用户无法留在管理页选岗位/施工目标 | `ManagementView` 新增 `SelectPersonRow(personId)`：只调用既有 `HomeSelectionContext.SelectPerson`，`RefreshNow` 后留在管理页；行模板点击回调改绑此方法。`OpenPerson` 保留“选中 + 独立详情”语义 |
| “查看人物详情”缺少明确独立入口 | 详情面板新增 authored `OpenPersonDetailButton`（新序列化字段 `personDetailButton`，文案“查看人物详情”）；点击只对当前 Selection 请求 `PersonDetailRequested`，无选人时禁用并在状态栏说明 |
| 选中高亮/右栏不明确 | 行 `Bind(..., selected)` 既有高亮保留；`detailTitle` 显示“所选人物 · 姓名”、`detailBody` 显示 PersonId 与职责事实；无选人时右栏与操作提示都说明“只选中、不跳转” |
| 禁用按钮原因笼统 | `ApplyDetail` 的 `actionHint` 逐项说明不可用原因：未选人、离村/不可用、未选岗位行、静态兼容岗位、未注册、满员、无主要职责、未选施工行、已在/不在该施工项目。四按钮 `interactable` 条件未放松 |
| 切页/选目标丢人物 | 既有 `SelectTab` / `SelectWorkplaceRow` / `SelectConstructionRow` 不触碰 Selection；新测试逐段断言 `SelectedPersonId` 保持 |

## 3. Prefab YAML 精确局部编辑（未运行 Editor 保存）

- `DetailPanel` 子列表新增 `{fileID: 1450098202734151202}`（唯一引用点）。
- `DetailTitle` 宽度 `486 → 306`；新按钮矩形 `(x:328,y:-14,w:174,h:40)`，完整落在 518 宽详情面板内，与标题留 6px 间隔。
- 新增对象：`OpenPersonDetailButton`（RectTransform / CanvasRenderer / Image / Button）及子 `Label`（TMP，文案“查看人物详情”），Layer 5，Button `m_TargetGraphic` 指向自身 Image，Image `m_RaycastTarget: 1`、Label `m_RaycastTarget: 0`。
- 新 fileID 段 `1450098202734151201…209` 在 Prefab 内逐项 grep 核对：每一定义仅 1 个 `&` 定义，引用均为上述 1 个子节点 / 1 个字段 / 组件表；未复用旧 ID。
- `ManagementView` 组件新增 `personDetailButton: {fileID: 1450098202734151205}`。`ManagementView.cs` 新增 1 个序列化引用，测试中的引用总数断言 `39 → 40`（0 null 口径不变）。

## 4. Play 测试变更（未运行）

- 修订旧第 1 例：人口行真实点击后断言 **Surface 仍为 Management、PersonDetail 不可见、Selection/行高亮/右栏为同一人**；随后点击独立详情入口才进入 Person Surface，原“人物往返 + 关闭回 World + 重开保留”覆盖保留。
- 新增第 17 例 `Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry`：
  1. 世界先 `Clear` 至无选人，再真实底部导航打开管理；断言独立详情入口禁用、右栏“未选择人物”、提示含“尚未选择人物”。
  2. 真实 EventSystem 指针点击人口行（`RealPointerClick` = RaycastAll + ExecuteEvents.PointerClick）；断言不跳页、Selection 为同一 PersonId、行高亮色 `(0.28,0.43,0.36,1)`、右栏含姓名与 PersonId。
  3. 独立详情入口 `RaycastAll` 命中 + 真实点击进入 PersonDetail，关闭回 World 且 Selection 保留；重开管理仍为同一人。
  4. 切职责页选岗位行/施工行，人物保留；派工、加入施工、撤出施工、撤职逐项断言按钮 `interactable`、真实指针点击（top-hit 由 RaycastAll 断言）与领域结果：岗位占用、施工参与者、`PersonWorkplace`；加入/撤出施工期间 Primary Assignment 保持。
  5. 全链路断言未离开 Management Surface、`IsSimulationActive` 保持。
- 夹具使用既有公开注入点 `ConfigureActivitySchedule` 隔离本例时间跨度内活动机会，TearDown 恢复；未新增产品 API、未写私有状态。

## 5. 文件内静态核对结果（非编译/非测试结论）

- `ManagementShellPlayTests.cs`：17 个 `[UnityTest]`；原 16 例方法名全部保留；新增第 17 例；旧“人口行点击必须进入 Person Surface”期望已删除并替换。
- `ManagementView.cs`：`SelectPersonRow` / `OpenSelectedPersonDetail` / `personDetailButton` 名称在 .cs 与 Prefab 字段间一一对应；四命令 `interactable` 表达式逐字保留原条件；`Server`/`Authority` 未改。
- `ManagementView.prefab`：新按钮 Image → Button → Label 引用链自洽；`ModalBlocker` 仍 `raycastTarget: 1` 且覆盖全屏；人口行模板 Image `raycastTarget: 1` 且 Button 与 Image 同对象；`ManagementView` 根 CanvasGroup 仍 `alpha=1 / interactable=1 / blocksRaycasts=1`。未发现需改动的同类 CanvasGroup/raycast/绑定缺陷。
- 受保护字体文件与 34 项 workspace-baseline 未被本包修改。

## 6. 待验证（明确未完成）

- **待 Unity 编译**：本包未编译，新增/修改 C# 的编译结果未验证。
- **待真实按钮命中验证**：新按钮与四命令按钮的物理/程序化 top-hit 需在 Play 中由第 17 例与第 1 例实际执行。
- **待 Play 集成验证**：第 1 例修订与第 17 例新增长路径均未运行；结果以 Studio 阶段 check 的 NUnit 回执为准。
- 字体仍 pending（磁盘为基线），本包未做任何字体操作；宿主既有 Atlas6 dirty 风险由 Studio 处理。
