# ManagementShellPlayTests 整类 13 例 PlayMode 集成门禁 · 宿主交付前预检（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。
- 预检时刻：**2026-10-26T07:42:22Z–07:43:29Z**（本机同日 15:42–15:43 +08:00）。以下每项均为本轮只读实测，非文档转述。
- 本包范围：**只读前置 + 只新增本文件**。不改产品源码 / 测试源码 / 断言 / 配置 / 字体 / Prefab / Scene / 存档 / guard manifest；不 `AssetDatabase.Refresh`、不 `tests-run`、不重跑 Play / Compiler / Edit、不 `rql-mutate`、不全局 Save / SaveChanges / SaveDirtyScenes、不 Shell、不 Git 写、不写用户 Preferences、不启停 Editor、不访问未授权工程 / 认证目录 / 其他工作区。
- 上游事实（主开发已读）：真实滚动单例 run `d7f5ba0b-ee42-40c8-b632-7cd7b999f27b`，**1/1 Passed、0 skipped、2.360873 s、scope-restored**；`real-scroll-evidence.log` 已由主开发核对内容；截图 `management-real-scroll-workplaces-overflow-last-row-20261026T073954Z.png` 已由主开发目视看到标题可见。
- 结论前置：**13 例源码 / DLL / 发现 / 保护面 / 宿主空闲均满足整类串行运行前置**；本文件**不预填 13 例结果**，PASS/FAIL 只能由宿主整类串行运行产出。**1280 由制作人 `01a53eb8` 取消，取消 ≠ 通过，不并入本轮。**

---

## 1. 现场前置（全部只读实测）

| 项 | 实测值（2026-10-26T07:42–07:43Z） | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()`（反射实测）= **False** | 无活动 / 无残留运行 |
| 场景 | 活动场景 `Assets/Scenes/SampleScene.scene`：`isDirty=False`、`rootCount=5`、`sceneCount=1` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开，无未保存 Prefab |
| Screen | **`1920x1080`、`Windowed`**、`fullScreen=false` | 真实 1920；本包不请求、不切换、不恢复尺寸 |
| `testResults` | 仅 **1 条**：`ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`，`Passed / 1496ms / message=null` | 真实滚动单例回执；**不带 runId / 时间戳，不能当整类证据** |
| Console | `Error/Exception` 共 **6** 条，逐条核对**全部为本包 / 前序 MCP `Script/Execute` snippet 自身错误**：`sequence 65192–65194`（本机 15:34:11 一次读错字体相对路径的 `FileNotFoundException`）、`sequence 65759–65761`（本机 15:42:52 一次探针取错对象名的 `InvalidOperationException`）；**无项目 / 测试程序集错误** | 无项目错误；整类运行前无新增项目错误 |

说明：`testResults` 的 `1496ms` 是方法内耗时；主开发转述的 `2.360873 s` 是 run 级总时长，两者**口径不同**，分别标注、不互相替代。

---

## 2. 本轮真实滚动单例的恢复保护（SAVE / FONT / BASELINE post，逐行原文）

日志：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，**669497B / 3027 行**，SHA256 `17d1a023ada17a02e5496d71741cf2887dff9031e6f3cc184f30da207390ee04`，mtime `2026-10-26T07:39:54.5268894Z`；本轮为第 3005–3027 行，历史行原样保留。

| 行 | 时间（UTC） | 原文事实 |
|---|---|---|
| 3005 | 07:39:52.5729267 | `[BASELINE-PRE] total=34 diff=0 all-match` |
| 3006 | 07:39:52.7213394 | `[FONT-PRE] sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |
| 3007 | 07:39:52.7543282 | `[SAVE-BACKUP] runId=20261026T073952Z status=captured file=…\R3B-SaveProtection-20261024\20261026T073952Z\original-save.bin bytes=13405 sha256=4b2b56f6…d6aa readback-verified=True` |
| 3008 | 07:39:52.7553610 | `[SAVE-CAPTURE] runId=20261026T073952Z … existed=True bytes=13405 sha256=4b2b56f6…d6aa` |
| 3024 | 07:39:54.2043320 | `[SAVE-RESTORE] runId=20261026T073952Z existed=True sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa matches-before=True backup=… manifest=…` |
| 3025 | 07:39:54.2073790 | `[SCHEDULE-RESTORE] restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}（只读校验）` |
| 3026 | 07:39:54.3708467 | `[FONT-POST] sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854 unchanged=True` |
| 3027 | 07:39:54.5257445 | `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match` |

guard manifest 实测（`…\R3B-SaveProtection-20261024\20261026T073952Z\manifest.json`）：`status="restored"`、`sourceBytes=13405`、`sourceSha256=4b2b56f6…d6aa`、`readbackVerified=true`、`restored=true`、`lastError=null`。

---

## 3. 保护面（字节 + SHA256 逐项实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 受保护 TMP 字体（磁盘） | **`14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**；mtime `2026-10-10T13:23:55.9520319Z` | 与基线一致，今日未被写 |
| 字体（内存 / 子资产，首测） | `LoadAllAssetsAtPath` = **9** 个子资产、`dirtyCount=0`（`TMP_FontAsset` + `Material` + 7×`Texture2D` 全 clean）；`atlasTextures.Length=8`（1 个 null 空闲槽）、`characterLookupTable.Count=644`、`atlasPopulationMode=Dynamic` | 首测磁盘 + 内存 clean；未 Save / 未 SetDirty（后续探针副作用见下方披露） |
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，6866B） | **`entries=34 / same=34 / diff=0 / missing=0`**；`complete=true` | 34/34 全一致（字体即 34 项之一） |
| 单槽存档 | `SaveSystem.GetSavePath()` = `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot\rrpg_save_p0.json`；`exists=true`、**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T07:39:54.2033347Z`、`.tmp=false` | 原始字节在位；mtime 即本轮单例 SAVE-RESTORE 回写时刻 |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **203** 个 run 目录：`restored=202`、`protective-snapshot=1`、其它状态 / 无 manifest **0** | 无 `captured` / `restore-failed` / `unfinished` / unknown |
| guard 最新 run | `20261026T073952Z`：`restored`，manifest mtime `2026-10-26T07:39:54.2043320Z` | **最新 restored**；与单槽 mtime、单例回执相接 |

**字体内存的副作用与恢复（如实披露，不隐瞒）**：07:42:22 首测为 9 子资产 `dirty=0`。此后本包为核对 detail 单行截断，**只读加载 `ManagementView.prefab` 资产**并对其中 `Detail` / `Label` 调用 `TMP_Text.GetPreferredValues`（不写文本、不 `SetDirty`、不进 PrefabStage）；该探针触发 TMP 动态字体在**内存**向 `Atlas 6` 插入 2 个字形（`characterLookupTable` 644→646，`Atlas 6` 变 dirty）。本包**未 Save / 未 SaveAssets / 未 RefreshAssets / 未改磁盘字体**；尝试 `AssetDatabase.ImportAsset(ForceUpdate)` 未能清除该 dirty，最终仅对 `Atlas 6` 调用 `EditorUtility.ClearDirty`，9 子资产回到 **`dirty=0`**。磁盘仍为 **`14961033B / 3c758e16…c404854`**、mtime `2026-10-10T13:23:55.9520319Z` 未变；内存 `lookups=646` 的缓存差异**未持久化**。

---

## 4. 源码、DLL 与 13 例发现（只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| 测试文件 | `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：**`358462B / 9882769d8a05f53ab5a80461fa095015c4b88a60984282c1c671b5b6d57eadc8`**，mtime `2026-10-26T07:36:00.3044324Z` | 与真实滚动包改后固定值逐字节一致；本轮未改 |
| 源码属性计数 | `[UnityTest]=13`、`[UnityTearDown]=0`、`Screen.SetResolution=0`、`new Vector2Int(1280=0` | 无尺寸写入、无无意义全局恢复；1280 分支已移除 |
| 测试程序集 | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：**`495104B / fbfdc51a103e46a863d1c5f33187016e4ce1bc24f05a888e1d45b7dcfe2583fc`**，mtime `2026-10-26T07:36:27.2652717Z`，**晚于源码 26.96 s** | DLL 新鲜，已含第 13 例 |
| 发现（整类） | `tests | where fullName contains "ManagementShellPlayTests"` 命中 **13** 条，`mode` 全部 `PlayMode`，`assembly` 全部 `BattleStage1.PlayModeTests` | 与源码 `[UnityTest]=13` 一致 |
| 用例清单 | 原 12 例名称逐一在位 + 第 13 例 `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput` | 原 12 例保留，未改名 / 未删除 |
| 每例 SAVE-RESTORE 保护 | 夹具级 `[UnitySetUp]`（行 1150）每例 `new SaveSlotGuard()` + `Capture()`；`[TearDown]`（行 1194）先 `_saveGuard.Restore()`，再 `ResetForTests`，最外层 `RestoreActivityScheduleAfterFixture()` | 13 例逐例套用同一条保护链 |
| 两例日程恢复 | `[SCHEDULE-RESTORE] restored=true sameInstance=True`（本轮单例日志亦已出现该行） | 日程隔离 / 恢复链在位 |
| 第 13 例的真实性 | 只新增第 13 例与专用 helper；使用真实 `BuildingPlacementValidator` 空位、真实 `HomeProductShell.Build` / `TryAddConstructionWorker` / `Clock.AdvanceTo`、真实 `ManagementReadAdapter` 投影；未注入假人物 / 假建筑 / 假行 | 与 `real-scroll-review.md` 一致 |

---

## 5. 本轮真实滚动日志（`real-scroll-evidence.log` 全 9 行，独立复核）

文件：`Development/StudioRuns/R3B-UI-20261026/play-evidence/real-scroll-evidence.log`，**3027B / 9 行**，SHA256 `b1e491a12a5203fbc029c4e190262949599ba7dadf73ec7eb096d0cbef290ff6`，mtime `2026-10-26T07:39:54.1708396Z`。

| 行 | 时间（UTC） | 事实 |
|---|---|---|
| 1 | 07:39:53.9134996 | `overflow=True contentHeight=812 viewportHeight=650 overflowPx=162`；末行 `lastRectBefore=(66,-34,1252,92)`、`viewport=(60,122,1264,650)`、`lastVisibleBefore=False` |
| 2 | 07:39:53.9200720 | 真实 `IScrollHandler`：`beforeNorm=1 afterNorm=0.8919753 contentY=0→17.5 point=(692,447)` |
| 3 | 07:39:54.0051437 | 真实拖拽：`beginDrag+Drag+endDrag=True steps=4 step=60 start=(692,447)`、`norm=0.8919753→0 contentY=17.5→162`、指针始终在列表内 |
| 4 | 07:39:54.0061371 | 末行 `rowAfter=(66,128,1252,92)`、`center=(692,174)`、`top=ConstructionRowTemplate(Clone)`、`handler=ConstructionRowTemplate(Clone)`、`visibleAfter=True` |
| 5 | 07:39:54.0272888 | 岗位末行标题 `WorkplaceRowTemplate(Clone)`：`text="农田 · building_instance_6 · 实例 building_instance_6 · 槽位 main"`、`chars=59 visible=51 rendered=675.2119x30.408 preferredH=30.41 rect=(0,-32,1072,32) overlapsDetail=False rowFullyInsideViewport=True` |
| 6 | 07:39:54.0283316 | 施工末行标题 `ConstructionRowTemplate(Clone)`：`text="施工 · 农田"`、`chars=7 visible=5 rendered=114.4063x30.408 preferredH=30.41 rect=(0,-32,1072,32) overlapsDetail=False rowFullyInsideViewport=True` |
| 7 | 07:39:54.0298930 | `[REAL-SCROLL-COVERAGE] Workplace=True Construction=True Resource=NotCoveredByThisCase` |
| 8 | 07:39:54.1388279 | `[REAL-SCROLL-SHOT] … bytes=269447 sha256=973fa3c2264c4caa3ed71f391122b1aa57b78757389b46c692ce4720c3529b3e` |
| 9 | 07:39:54.1708396 | 点击岗位后再次核对施工标题，数值同上、`overlapsDetail=False` |

截图独立复核：文件 `Development/StudioRuns/R3B-UI-20261026/play-evidence/management-real-scroll-workplaces-overflow-last-row-20261026T073954Z.png`，**269447B / SHA256 `973fa3c2…29b3e` / mtime `2026-10-26T07:39:54.1091546Z` / PNG IHDR 实测 `1920x1080`**；与日志第 8 行一致。

**口径**：`visible=51` / `visible=5` 是 TMP `characterInfo[i].isVisible` 的**非空白可见字形数**（空格不计），不是 Ellipsis 截断；用例对标题显式断言 `isTextOverflowing=false` 且本单例 `Passed`，日志 `rendered` 宽度 675.21 / 114.41 均远小于 1072，故标题为完整渲染、未溢出、未与 detail 墨迹重叠。`Resource=NotCoveredByThisCase` 如实保留，不写成资源行已覆盖。

---

## 6. 截图中“职责 detail”实际可见性（只读事实，不改代码，不宣称可读性通过）

主开发在截图中观察到：职责与场所列表每行的第二行只露出长 `WorkplaceId` 并以 `…` 结束。以下对照 `ManagementView` 的 detail 文本生成与 Prefab 的 42 高单行布局，给出**直接文本 / 布局事实**与影响判断。

### 6.1 代码生成的 detail 文本（`ManagementView.cs` 706–723，`WorkplaceDetail`）

建筑岗位行（非 Legacy）生成的字符串是**固定 3 行**：

1. `岗位 <WorkplaceId>`（如 `岗位 workplace:building_instance_6:main`）
2. `建筑 <BuildingInstanceId> · 槽位 <FunctionSlotId> · 功能 <FunctionProfileId>`
3. `绑定 <BoundCount>/<Capacity> · 实际劳动 <LaborCount> · 现场 <OnsiteCount>`（未注册 / 已满员时追加状态）` · 绑定人：<PersonNames 或 无>`

行绑定时（`ManagementView.cs` 494）把**完整三段文本**传给 `ManagementRowView.Bind`；`ManagementRowView.cs` 30 直接 `detail.text = description`，**数据层没有截断**，截断只发生在 TMP 显示层。

### 6.2 Prefab 的 42 高单行布局（`Assets/Resources/UI/ManagementView.prefab`，只读加载实测）

| 对象 | 实测 authored 值 |
|---|---|
| `WorkplaceRowTemplate`（行） | `1100×100` |
| `WorkplaceRowTemplate/Detail` | **`1072×42`、字号 17、`enableWordWrapping=true`、`overflowMode=Ellipsis`** |
| `WorkplaceRowTemplate/Label`（标题） | `1072×32`、字号 21、`overflowMode=Ellipsis` |
| 其余三行模板 `PersonRowTemplate / ConstructionRowTemplate / ResourceRowTemplate` 的 `Detail` | 同为 **`1072×42`、字号 17、Ellipsis**；行高分别 `86 / 92 / 84` |
| Prefab 内 `1072×42` 出现次数 | **4 处**（四个行模板的 Detail）；`1072×28` = **0 处** |
| 右侧 `DetailPanel / DetailBody` | 面板 `518×712`；`DetailScroll` `486×390` 且带 `ScrollRect`；`DetailBody` 在 `DetailScrollContent` 下 stretch（prefab 资产态 rect 为 0×0，布局由运行时拉伸）、字号 18、Ellipsis、wrap=true |

### 6.3 为什么只显示一行（直接算术，非实测该 label 的 preferredHeight）

真实运行日志第 5 行给出：字号 21 的标题 `preferredH=30.408`，即该字体行高系数 `30.408 / 21 ≈ 1.448`。据此字号 17 的单行行高 ≈ **24.6px**，两行 ≈ **49.2px**；`Detail` 矩形高度仅 **42px**，**放不下第二行**。TMP 在 `Ellipsis` 模式下于是只排第一行，第二、三行被省略号取代。

（注：本包未在运行中的该 label 上另测 `preferredHeight`；上述 24.6/49.2 是由同一字体、同一 TMP 的真实标题行高按字号线性换算所得，标注为换算值。）

### 6.4 截图内实际看到什么（`073954Z` PNG）

- 列表可见 5 条岗位行（`building_instance_2…6`）+ 1 条施工行；**每行 detail 只有第一行**：`岗位 workplace:building_instance_N:main…`。
- **容量 / 绑定 / 劳动 / 现场四项在列表中不可见**：它们位于 detail 第 3 行，被 42px 单行 + Ellipsis 截掉；detail 第 2 行的 `建筑 / 槽位 / 功能` 同样不可见（列表里出现的 `main` 来自 `WorkplaceId` 自带后缀与标题）。
- 标题可见且信息完整：如 `农田 · building_instance_6 · 实例 building_instance_6 · 槽位 main`（日志 `rendered=675.21×30.408`、未溢出、不与 detail 墨迹重叠）。
- 右侧详情栏在本张截图时**选中的是人物**（`所选人物 · 林恩`），下方是 `派工目标：未选择岗位行` / `施工目标：未选择施工行`；**因此本张截图里“容量/绑定/劳动/现场”在右栏也没有出现**。日志显示岗位行点击发生在截图**之后**（截图 07:39:54.1388，点击 07:39:54.1642），截图未捕获点击后的右栏。
- 代码路径上，选中岗位行后 `ApplyDetail`（`ManagementView.cs` 599–606）会把 `WorkplaceTitle + "\n" + WorkplaceDetail` 写入右侧 `detailBody`（字号 18、486×390 滚动区）。但**本轮没有点击后的截图 / 像素证据**，故本文件不声称“右栏可读”已通过，只记录路径存在。

### 6.5 是否影响规格必要信息（留主开发判断，本包不修）

- 规格 §4 要求“职责与场所”一行呈现 `WorkplaceId + BuildingInstanceId + FunctionSlotId`，并列出**容量 / 绑定 / 劳动 / 现场**。当前**列表行**中：`WorkplaceId` 可见、`BuildingInstanceId/槽位` 在标题可见，但 `容量 / 绑定 / 劳动 / 现场` 与 `功能` **实际不可见**（被 42 高单行截断）。
- 这属于**列表行可读性 / 信息密度缺口**，不是数据缺失（完整文本已写入 TMP，`ManagementReadAdapter` 数据在案）；也不是本用例的断言失败——第 13 例只断言标题渲染、末行可见与点击回读，**未断言 detail 文本可读**。
- 因此：**不修代码、不改 Prefab、不伪称“detail 可读性已通过”**；“是否做最小 UI 改进（例如行内只放一行摘要、把四项移到右栏或加高 detail / 换行）”留给主开发判断；该观察**不阻止**宿主按现行行为跑整类 13 例。

---

## 7. 1280 与版本门禁口径（禁止合并）

| 事实 | 数据 | 口径 |
|---|---|---|
| 1280 测试 | 制作人记录 `01a53eb8` **取消** | **取消 ≠ 通过**；不得由 1920 / 本单例外推 1280 |
| 真实滚动第 13 例 | 宿主 run `d7f5ba0b…` **1/1 Passed、0 skipped、2.360873 s、scope-restored**；本机 `testResults` 单条 `Passed / 1496ms`；日志 / 截图 / guard 时间戳一致 | **单例真实通过**（仅此一例） |
| 原 12 例 | 最新整类 `4aa4ffda…` **12/12 Passed**（历史）；本包未重跑 | **历史**，不并入本轮 |
| 整类 13 例 | 发现 13、DLL 新鲜、保护面 clean、宿主空闲 | **前置满足，结果待宿主产出**；本文件不预填 |
| Compiler / 完整 Edit / 完整 Play | `264/264`、`1513/1513`、`122/122`（历史） | **历史门禁**，本包未重跑 |

---

## 8. 本包边界与实际副作用

- **唯一写入**：本文件 `Development/StudioRuns/R3B-UI-20261026/management-thirteen-preflight.md`（新增）；不覆盖任何历史截图 / 文档。
- 唯一 Editor 操作：`rql-query` 只读查询、`script-execute` 只读 C#（读文件 / 反射 / 解析 PNG 头 / 只读加载 Prefab 资产读 authored 字段）、对本地 PNG 的 `read_image` 目视复核；**未** `AssetDatabase.Refresh`、**未** `rql-mutate`、**未** `tests-run`、**未**全局保存、**未**保存场景 / Prefab、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor。
- 未改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 34 项基线 / 存档本体 / guard manifest；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- **副作用披露（与 §3 同源）**：只读加载 Prefab 资产做 `GetPreferredValues` 行高探针时，TMP 动态字体在内存向 `Atlas 6` 加了 2 个字形并置 dirty；**未保存、磁盘字节 / SHA / mtime 未变**，最终用 `EditorUtility.ClearDirty` 将 9 子资产恢复为 `dirty=0`（内存 `lookups=646` 的缓存差异未持久化）。此外 `AssetDatabase.ImportAsset(ForceUpdate)` 仅用于尝试丢弃该内存 dirty，未改写源文件（import 前后磁盘 SHA 一致）。
- 不做文档全集收尾，只新增本文件。

---

## 9. 移交宿主（整类 13 例串行）

1. 以**短类名** `ManagementShellPlayTests` 运行 **PlayMode 整类 13 例、串行**；运行前宿主真实 `Screen` 必须仍是 `1920x1080`（用例自身严格断言、**不请求 / 不恢复 / 不缩放**尺寸）。
2. 逐例核对真实回执：每例 `[SAVE-RESTORE]`（含 `runId`、`sha256`）在案；两例日程用例（`ManagementConstructionCommands_…`、`PendingEvent_BlocksAdvanceAndReturn_…`）`[SCHEDULE-RESTORE] restored=true sameInstance=True`；第 13 例 `[REAL-SCROLL-*]` 与截图时间戳为新增文件。
3. 运行后复核：字体仍为 `14961033B / 3c758e16…`、单槽仍为 `13405-4b2b56`、guard 无 `unfinished`、场景 `isDirty=false`。
4. 失败时保留真实 `message / stackTrace` 与新增日志行 / 截图；`Resource=NotCoveredByThisCase` 仍按未覆盖；1280 仍按取消。
5. **不预填 13 通过**：本文件只证明“前置满足”；结果以宿主整类真实回执为准。
6. 本文件及结论需主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。

---

## 10. 写后只读复核（2026-10-26T07:45:13Z，证明本包无磁盘副作用）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 受保护字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，mtime `2026-10-10T13:23:55.9520319Z` | 未变 |
| 字体（内存） | 9 子资产 `dirty=0`；`lookups=646`（见 §3 探针副作用披露，未持久化） | 已回到 clean 标记 |
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`，mtime `2026-10-26T07:39:54.2033347Z`、`.tmp=false` | 未变 |
| guard | `dirs=203`：`restored=202`、`protective-snapshot=1`；最新 `20261026T073952Z restored` | 无未完成 run |
| 测试源码 / DLL | `358462B / 9882769d…`、`495104B / fbfdc51a…` | 未改、仍新鲜 |
| Editor / 场景 / Screen | `playing=False`、`compiling=False`、`sceneDirty=False`、`prefabStage=null`、`1920x1080` | 仍空闲、仍 1920 |
| `testResults` | 仍 **1** 条（真实滚动单例 `Passed / 1496ms`） | 未新增运行，整类 13 结果未产生 |

本文件自身：最终字节 / SHA / mtime 以目录实际文件为准（本节追加后再无写入；主开发可自行重算，不据旧值宣称未改）。**唯一新增产物**为本文件；整类 13 例真实回执仍未产生，等待宿主串行运行。
