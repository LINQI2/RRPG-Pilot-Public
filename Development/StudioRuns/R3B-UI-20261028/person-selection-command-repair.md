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

---

## 7. 当前附记（2026-10-27 只读复核：17 例真实通过；本节新增，§1–§6 原文未改）

- **本机真实时钟**：`2026-10-27T18:36:04.952+08:00`（UTC `10:36:04Z`）。目录 `R3B-UI-20261028` 是既有分配路径，**未改名、未改历史日期**；本机当前日期是 10-27，不在文档里假装是 10-28。
- **宿主记录**（主开发读原记录确认，本层不可读 Studio 私有 runtime）：`40f35ae0-b7d6-4e37-ad86-235aeb64ae45` → **Play `ManagementShellPlayTests` 17/17 Passed、Failed 0、Skipped 0、`00:00:17.2275503`、`resourceScopeEnd=scope-restored|font|0|632`**；第 17 例 **750 ms**、第 1 例 **654 ms**。
- **本层只读交叉核对**：`testResults` 现有 **17 条全 `Passed`**、逐条 `message=null / stackTrace=null`（无失败详情）；方法耗时合计 **16 199 ms**；第 17 例 `Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry` **750 ms**、第 1 例 `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` **654 ms**，与宿主记录一致。
- **§3 的四项缺陷已由真实用例闭环**（不再是“待验证”）：
  1. 人口行单击**留页**：第 1 例与第 17 例都断言 `Surface=Management` + `PersonDetail` 不可见 + `Selection/SelectedPersonId` 同一人。
  2. **选人高亮**：选中行 `Image.color == (0.28,0.43,0.36,1)`，右栏标题含姓名、正文含 `PersonId`。
  3. **独立详情入口**：`personDetailButton` 文案“查看人物详情”、`Image.raycastTarget=1` / 文案 `raycastTarget=0`；无选人时 `interactable=false` 且提示含“尚未选择人物”；真实点击进入既有 Person Surface（同一 `PersonId`），关闭回 World 并保留选择。
  4. **跨页保留人物 + 四命令**：切页 / 选岗位行 / 选施工行后 `SelectedPersonId` 均不变；派工、加入施工、撤出施工、撤职四按钮 `interactable` 与真实 `RaycastAll` top-hit 点击全部按预期，领域结果为 `occupants=1 → 施工参与者写入 → 参与者移除且 Primary 保持 → occupants=0`，全程 `Surface=Management`、`IsSimulationActive` 保持。
- **关键日志原文**（本轮 Console `sequence 81293–81310`，非 Error）：
  - `USER-PATH row-click selected=person_e3679e20fc0345d39b638bd0184aa897; surface=Management; detail-open=False; highlight=True; hint="当前不可用的命令：…"`
  - `USER-PATH assign ok workplace=workplace:building_instance_1:main occupants=1`
  - `USER-PATH construction-add ok primary=workplace:building_instance_1:main participants=person_e3679e20…`
  - `USER-PATH construction-remove ok primary=workplace:building_instance_1:main remainingParticipants=`
  - `USER-PATH remove-assignment ok workplace=<none> occupants=0; surface=Management; simulationActive=True`
- **保护面（17 例之后只读实测）**：字体 `14961033B / 3c758e16…c404854`（mtime 仍 2026-10-10）+ 9 子资产 `dirty=0`；34 项 `same=34 / diff=0 / missing=0`；用户存档 **`13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`**（`tmp=False`）；guard **310 = 309 restored + 1 protective-snapshot、`unfinished=0`**；`SampleScene dirty=False roots=5`、无 PrefabStage、`isPlaying/isCompiling=False`，运行段 Console `Error/Exception` 0。本轮 17 例各自 `SAVE-CAPTURE` + `readback-verified=True` + `SAVE-RESTORE matches-before=True`（各 17 条，0 反例）。
- **§5–§6 已过期口径的替代**：编译与 17 例发现由 `compile-discovery-appendix.md` 覆盖；运行结果由本节覆盖。**§6 第 2 条“待真实按钮命中验证”已满足，但只限于程序化 `RaycastAll + ExecuteEvents` top-hit；物理鼠标 / 滚轮 / 键盘 / 物理 ESC 手感仍未覆盖，不得写成“物理输入已验”。**
- **截图**：第 17 例**不产截图**，本轮没有用户路径截图；同 run 的 3 张 `2026-10-27T10:34:05Z` 滚动截图只证明滚动与行文本。
- **本层边界**：只读 `rql-query` / 只读 `script-execute`（哈希 / manifest 统计 / IsDirty）/ 本地读写文档；未 `tests-run`、未 Refresh、未 PrefabStage、未字体测量（无 `ForceMeshUpdate` / `GetPreferredValues`）、未建 UI 实例、未 `rql-mutate`、未 `SaveAssets/SetDirty/ClearDirty`、未 Shell/Git/Prefs/正式工程、未恢复任何存档（旧 `13405` 未碰）。
- 详细 run 元数据、逐项证据与人工操作步骤见同目录 `selection-command-verified-handoff.md`。**需主开发专业核验，不代表制作人批准；R3-B 仍 Implemented / Verification PARTIAL / Not Committed / Not Published。**
