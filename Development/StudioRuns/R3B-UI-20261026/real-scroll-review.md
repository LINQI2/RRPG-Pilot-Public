# R3-B 真实建筑业务行滚动与标题专项（2026-10-26，阶段 1/1）

## 0. 结论前置

- 本包在 `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` **只新增**：
  - `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`（第 13 例）；
  - 专用 helper：`RealScrollEvidence` / `CaptureRealScrollShot` / `RealScrollAndDragToEnd` / `AssertBusinessRowTitleRendered` / `AssertWorldInputsUnchanged`（以及小型矩形判定 helper）。
- 原 12 例未改名、未删除、未改写断言；测试文件当前 `[UnityTest] = 13`。
- `AssetDatabase.Refresh` 已执行，编译完成；`tests` 查询确认新方法**唯一发现**：`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`，`PlayMode / BattleStage1.PlayModeTests`。
- **按本任务“模型不 tests-run”的边界，本执行层没有启动 TestRunner、没有运行该用例**。真实 overflow / 滚轮 / 拖拽 / 截图 / 点击回执由宿主 Studio 后续单方法运行产生；本文件不预填这些运行时数值，不把设计推导冒充实测。
- 本包未 `rql-mutate`、未 Shell、未 Git 写、未全局 `SaveAssets/SaveChanges`、未保存场景 / Prefab / 字体、未改配置 Authority、未注入假建筑 / 假人物、未改 viewport / 行高、未克隆假行。

## 1. 先核 Pilot / 空闲 / 场景 / 字体 / 34 项 / 单槽 / guard（只读实测）

实测时刻：`2026-10-26T07:34:20Z`（`2026-10-26T15:34:20+08:00`）。全部为 `script-execute` 只读 C# 与 `rql-query` 实测。

| 项 | 实测 | 判定 |
|---|---|---|
| Unity / Editor | `2022.3.62t9`；`isPlaying=False`、`isCompiling=False`、`isUpdating=False`、`prefabStage=null` | 空闲；未播放 / 编译 / 打开 PrefabStage |
| Screen | `1920x1080 / Windowed` | 未切 1280、未改 Prefs / 尺寸 |
| 场景 | `Assets/Scenes/SampleScene.scene`；`dirty=False`、`roots=5`、`loaded=True` | 干净 |
| 字体磁盘 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；mtime `2026-10-10T13:23:55.9520319Z` | 与受保护基线一致 |
| 字体内存 / 子资产 | `LoadAllAssetsAtPath` 共 **9** 个子资产；`EditorUtility.IsDirty` = **0** | 磁盘 + 内存 clean |
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0`；`complete=true` | 34/34 一致 |
| 单槽存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`；`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`；`.tmp=False` | 单槽原字节在位 |
| guard 根 | `R3B-SaveProtection-20261024`：**202** 个 manifest；`restored=201`、`protective-snapshot=1`、其它状态 **0** | 无 `captured` / `restore-failed` / unknown，无未完成 run |

写代码并编译后再次只读复核（`2026-10-26T07:37:34Z`）：`isCompiling=False`、场景 `dirty=False`；字体仍 `14961033B / 3c758e16…c404854`；34 项仍 `34/34`；单槽仍 `13405B / 4b2b56f6…d6aa`；guard 仍 `202 = 201 restored + 1 protective`。编译未改变受保护资产 / 场景 / 单槽。

补充只读复核（编译后）：`ManagementView.prefab` 仍 `282326B / e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba`，mtime `2026-10-26T03:26:15.6250542Z`；字体 9 个子资产仍 `dirty=0`，`prefabStage=null`、场景 `dirty=False`。本包未改 Prefab / 字体 / Scene。

## 2. 合法建造数据源（全部为既有产品入口 / 配置）

| 数据 / 动作 | 真实来源 | 本用例用法 |
|---|---|---|
| 建筑定义 | `UnityProject/RRPG1.0game/Assets/Generated/StaticConfig/Databases/BuildingConfigDatabase.asset` | 使用既有 `building_farm`：`DisplayName=农田`、`FootprintWidth=3`、`FootprintDepth=2`、FunctionBinding `main → ws_farm_01`。不改配置。 |
| 合法空位 | `BuildingPlacementValidator`：`MinX=-12, MaxX=12, MinY=-10, MaxY=8`；占地中心 + 半宽 / 半深检测；与现有建筑重叠检测 | 7 座 Farm 放在 `x = -10.5, -7.5, -4.5, -1.5, 1.5, 4.5, 7.5`，`y=-4`；相邻 `Δx=3` 恰好不重叠，全部在建造范围内。 |
| 放置 | `HomeProductShell.Build` → `HomeBuildingPlacement.OpenCatalog/Choose/SetCandidate/Confirm` → `VillageRuntimeBridge.TryPlaceBuilding` | 复用测试文件既有 `PlaceBuilding` helper；无私有注入。 |
| 施工参与 | `VillageRuntimeBridge.TryAddConstructionWorker`（内部真实 `Projects.TryCreateProject` / `TryAddProjectParticipant` / `RefreshWorkIntent`） | 6 名真实人物各加入 1 座 Farm；第 7 座在真实推进后加入 1 名真实人物。 |
| 完成 | 真实 `Clock.AdvanceTo(14400)`（既有 `AdvanceTo` helper；Clock 保持 Paused） | 前 6 座 Farm 真实完工；第 7 座保持 `UnderConstruction`，作为唯一真实施工行。 |
| 人物 | `ProductHomeWorldHost.Snapshot.Persons`（真实 `PlayerCollection` / 初始名册） | 通过 `HostPersonIds()` 读取至少 6 名合法 PersonId；不新建 / 不克隆人物。 |
| 管理投影 | `ManagementReadAdapter.Read()` 的真实 `Workplaces / Construction / Resources` 行 | 只读断言行数、`WorkplaceId`、`BuildingInstanceId` 与真实建筑一一对应。 |

**容量与确定性**：初始真实人物为 6 名；每座 Farm 施工项目 1 名参与者，6 座可并行，真实推进 14400 分钟后各得 10 个工日进度并完工（与既有 CASE-A 的 2 座 Farm / 14400 分钟口径一致）。本用例只用真实人物与真实空位；未修改任何 Authority / 配置 / 产品规则。若宿主实际世界中 6 名人物不能全部参加施工，测试会在 `TryAddConstructionWorker` 的真实回执断言处明确失败，而不是注入假人物。

## 3. 真实 overflow / 输入前后值 / 截图：运行待回填（不预填）

本执行层**没有 tests-run**，因此下列运行时字段尚未产生，不能在报告中写成实测值。测试代码会在宿主单方法运行时写入新证据日志：

`Development/StudioRuns/R3B-UI-20261026/play-evidence/real-scroll-evidence.log`

| 运行时证据 tag | 测试实际记录字段 | 用途 |
|---|---|---|
| `REAL-SCROLL-OVERFLOW` | `contentHeight`、`viewportHeight`、`overflowPx`、末行 `lastRectBefore`、`viewport`、`lastVisibleBefore=False` | 证明真实 `contentHeight > viewportHeight`，末行初始不可见 |
| `REAL-SCROLL-WHEEL` | `beforeNorm`、`afterNorm`、`contentY before→after`、指针 `point` | 证明真实 `IScrollHandler` scroll 实际改变 `verticalNormalizedPosition` 与 `content.anchoredPosition` |
| `REAL-SCROLL-DRAG` | `beginDrag+Drag+endDrag=True`、`steps`、`step`、`start`、`viewport`、`norm before→after`、`contentY before→after` | 证明真实拖拽事件（指针始终在列表视口内）实际改变位置并到达末端 |
| `REAL-SCROLL-LAST-ROW` | 末行 `rowAfter`、`viewport`、`center`、top hit、`IPointerClickHandler`、`visibleAfter=True` | 证明末行从不可见到可见且 Raycast 可点击 |
| `REAL-SCROLL-TITLE` | 标题文本、`characterCount`、`visibleChars`、`renderedWidth×renderedHeight`、`preferredHeight`、`overlapsDetail`、`rowFullyInsideViewport` | 证明 Workplace / Construction 真实业务行标题渲染可见、行高足够、不与 detail 墨迹重叠 |
| `REAL-SCROLL-CLICK` | `workplace` / `building`、世界 `SelectionId`、`ClockSpeed` | 证明末行点击对应真实 `WorkplaceId` / `BuildingInstanceId`，世界选择与 Pause 未被改变 |
| `REAL-SCROLL-SHOT` | 唯一时间戳 PNG 路径、bytes、SHA256 | 真实截图证据；`ScreenCapture` 不可用时只记 `UNAVAILABLE`，不伪造文件 |

### 3.1 静态可核的布局下界（不是运行时实测）

Prefab `Assets/Resources/UI/ManagementView.prefab` 的真实 authored 值：
- `WorkplaceScroll` 尺寸 `1264 × 650`；
- `WorkplaceRowTemplate` 行高 `100`，`ConstructionRowTemplate` 行高 `92`；
- `WorkplaceScrollContent` 为 `VerticalLayoutGroup`：`spacing=6`、上下 `padding=6`，`ContentSizeFitter verticalFit=PreferredSize`；
- 6 条岗位行 + 1 条施工行 + 两个 heading 的真实布局高度必然大于 `650`（仅 6 条 100 高岗位行 + 5 个 6 间距已达 `630`，再加 92 高施工行与 heading / padding）。

因此测试的 `overflow > 40px` 前置不是人为缩小 viewport，而是由 6 条真实 Completed 岗位行 + 1 条真实施工行构成；实际 `contentHeight / viewportHeight` 以运行日 `REAL-SCROLL-OVERFLOW` 为准。

### 3.2 截图

- 代码在滚动到末端、末行可见后执行 `CaptureRealScrollShot("workplaces-overflow-last-row")`。
- 文件名：`Development/StudioRuns/R3B-UI-20261026/play-evidence/management-real-scroll-workplaces-overflow-last-row-<UTC yyyyMMddTHHmmssZ>.png`；若同秒已存在则追加 `-2`，不覆盖历史。
- **本执行层尚未 tests-run，故当前没有本用例 PNG**；不拿旧截图冒充。宿主运行后以 `REAL-SCROLL-SHOT` 行的实际 `path / bytes / sha256` 为准。

## 4. 输入构造细节（测试代码，不用私有状态）

1. **真实滚轮**：`PointerEventData.position = ScreenCenter(viewport)`，`scrollDelta=(0,-0.5)`，通过 `ExecuteEvents.GetEventHandler<IScrollHandler>(Raycast top)` 取得真实 `ScrollRect` 并 `ExecuteEvents.Execute(..., scrollHandler)`；断言 `handler == scroll.gameObject`。
2. **真实拖拽**：起点 `ScreenCenter(viewport)`，通过 `GetEventHandler<IBeginDragHandler>` 取得同一 `ScrollRect`；
   - `beginDragHandler`；
   - 多次 `dragHandler`，`PointerEventData.position` 从视口中心逐步向上（每次 `step = min(60, viewportHeight*0.1)`，共约 220px），每一步 `PointInside(viewRect, next)` 断言指针仍在列表内；
   - `endDragHandler`。
3. 只在真实 `ScrollRect` 上发事件；**没有直接赋 `verticalNormalizedPosition`**，没有改 viewport / content 尺寸，没有手写行高。
4. 末行点击使用既有 `RealPointerClick`（真实 `EventSystem.RaycastAll` + `ExecuteEvents.pointerClickHandler`），先点末岗位行断言 `SelectedWorkplaceId`，再点末施工行断言 `SelectedConstructionBuildingId`。
5. 世界输入保持：每次滚动 / 行点击后断言 `SelectionId()` 不变、`ClockSpeed()=="Paused"`、`SimulationActive()==true`。

## 5. 标题覆盖类

| 业务行类 | 本用例实际覆盖 | 说明 |
|---|---|---|
| Workplace | **是** | 滚动后取真实 `workplaceContent` 最后一条 active 行，核对 TMP 可见字符 / rendered / preferred 高度 / 与 detail 墨迹不重叠，并点击回读真实 `WorkplaceId`。 |
| Construction | **是** | 滚动后取真实 `constructionContent` 唯一行，同样核对标题渲染，并点击回读真实 `BuildingInstanceId`。 |
| Resource | **未覆盖** | 本用例只打开职责与场所页；资源页真实行虽由同批 Completed Farm 产生，但没有在该用例中切页 / 滚动 / 点击。报告不把 Resource 写成已验证。 |

## 6. 编译 / 发现 / 文件证据

| 项 | 实测 |
|---|---|
| `AssetDatabase.Refresh` | `2026-10-26T07:36:25Z` 请求；`isCompiling=True` 后复测 `isCompiling=False` |
| 编译后 Console Error | 查询 `sequence > 65194` 的 Error = **0**；更早仅两条为本包第一次 preflight 读取错误路径的脚本异常，与本测试编译无关 |
| 测试发现 | `tests | where fullName contains "ManagementScroll_RealBuildingRows"` = **1** 条，`PlayMode / BattleStage1.PlayModeTests` |
| 整类发现 | `ManagementShellPlayTests` PlayMode = **13** 条；源码 `[UnityTest]` = **13** |
| 测试源码 | `358462B / 9882769d8a05f53ab5a80461fa095015c4b88a60984282c1c671b5b6d57eadc8`；mtime `2026-10-26T07:36:00.3044324Z` |
| PlayMode 测试 DLL | `495104B / fbfdc51a103e46a863d1c5f33187016e4ce1bc24f05a888e1d45b7dcfe2583fc`；mtime `2026-10-26T07:36:27.2652717Z`，**晚于源码** |
| 原 12 例 | 未改名 / 未删除 / 未改写断言；本包只新增第 13 例与专用 helper |

> 说明：编译后测试文件自身字节 / SHA 与 DLL 为实际读取值；DLL 新鲜只能证明已编译包含当前源码，不能替代宿主运行回执。

## 7. 未测 / 未覆盖（如实保留）

1. **本用例尚未由 TestRunner 执行**：按任务边界模型不 tests-run；真实 overflow、滚轮 / 拖拽前后值、截图、末行点击回执等待 Studio 宿主单方法运行。
2. **物理鼠标 / 人工手感**：仍为 `EventSystem.RaycastAll + ExecuteEvents` 程序化输入；真实指针设备与手感未测。
3. **Resource 业务行标题**：未在本用例切到资源页核对，未伪造。
4. **1280×720**：制作人已取消；本用例不请求、不恢复、不缩放尺寸。
5. **PrefabStage / 字体写 / 全局保存 / Git**：本包均未执行。
6. **世界容量**：本夹具基于当前真实初始 6 人；若宿主实际人物数不足 6，测试会以真实命令回执失败并保留证据，不注入假人物。当前配置/世界容量下设计为 6 条 Completed 岗位 + 1 条施工行；是否实际溢出以运行日 `REAL-SCROLL-OVERFLOW` 为准。

## 8. 本包实际写入

- `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：仅新增第 13 例与专用 helper。
- `Development/StudioRuns/R3B-UI-20261026/real-scroll-review.md`：本文件（新增）。
- 未改产品源码、Prefab、Scene、字体、图集、配置、Authority、存档、guard manifest、34 项基线；未 `rql-mutate`、未 Shell、未 Git 写、未 tests-run。

本文件及结论须由主开发专业核验，不代表制作人批准；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。
