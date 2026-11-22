# 详情修复后 · ManagementShellPlayTests 整类 13 例 PlayMode 集成预检（2026-10-26，阶段 1/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 预检时刻：本轮只读实测窗口 **2026-10-26T08:51:25Z–08:52Z**（本机同日 16:51 +08:00）。下列每项均为本轮实测，上游转述与本机实测分开标注。
- 本包范围：**只读严格安全预检 + 读本轮 restore / 详情字符日志 + 仅新增本文件**。**未改任何代码 / 测试 / 断言 / Prefab / Scene / 字体 / 图集 / 配置 / 存档 / guard manifest / 34 项基线**；**未** `AssetDatabase.Refresh`、**未** `PrefabStage`、**未** `rql-mutate`、**未** `tests-run`（模型不 tests-run）、**未** `AssetDatabase.SaveAssets / SaveChanges / SaveDirtyScenes`、**未** `SetDirty`、**未** `ClearDirty`、**未** `GetPreferredValues` / `ForceMeshUpdate` 预检探针、**未** Shell / Git 写、**未**写 Preferences、**未**启停 Editor。
- 上游记录（主开发已核，本执行层读不到 Studio 私有 runtime，按“转述”标注）：详情修复后滚动专项单方法 run **`49fcb102-24df-466d-9496-356a67dce8c2`：1/1 Passed、0 skipped、`3.2231345` s、`scope-restored`**；主开发已实际查看 `management-real-scroll-workplaces-overflow-last-row-20261026T084927Z.png`（绑定 0/2、实际劳动 0、现场 0、完整 `WorkplaceId`、实例 / slot 均可读）。
- 结论前置：**字体磁盘 = 基线且内存 / 9 子资产 0 dirty、34/34、单槽 `13405-4b2b56`、guard 217 run 全部终态合法、Pilot 原生空闲、场景干净、13 例发现且 DLL 新鲜**；满足宿主整类串行运行前置。**本文件不预填 13 例结果。**

---

## 1. 身份、工程空闲与场景（只读实测）

| 项 | 实测值（2026-10-26T08:51:25Z 起） | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9`；`Application.dataPath = D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False` | 空闲，无播放 / 编译 / 导入 |
| 原生 TestRunner | 反射 `UnityEditor.TestTools.TestRunner.Api.TestRunnerApi.IsRunActive()` = **False**（`params=0 / ret=Boolean`，非属性） | 无活动 / 残留运行 |
| 场景 | 活动场景 `Assets/Scenes/SampleScene.scene`：`isDirty=False`、`rootCount=5`、`sceneCount=1` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开（本包不打开） |
| Screen | **`1920x1080`、`fullScreen=false`**；`currentResolution=2560x1600`（桌面分辨率，非渲染尺寸） | 满足宿主 1920 前置；**未切 1280、未改 Prefs** |
| `testResults` | **1 条**：`ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`，`Passed / 2092ms / message=null` | 详情修复后单方法回执的本机可核对部分；**不带 runId / 时间戳，不能当整类证据** |
| Console（`Error`/`Exception`） | 共 **3 条**，`sequence 67690–67692`，全部为本机 16:48:04（08:48:04Z）上包一次只读复查 `Script/Execute` 探针自身的 `NullReferenceException` | **无项目 / 测试程序集错误**；与产品代码无关，原样保留不清洗 |

口径说明：本机 `2092ms` 是方法内耗时；主开发转述的 `3.2231345 s` 是 run 级总时长，两者**口径不同**，分别标注、不互相替代。

---

## 2. 受保护字体：磁盘 / 内存 / 子资产（安全门禁，只读）

### 2.1 磁盘（重算字节 + SHA256，未写文件）

| 项 | 实测值 | 要求值 | 判定 |
|---|---|---|---|
| 路径 | `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | 同 | ✅ |
| bytes | **14961033** | 14961033 | ✅ |
| SHA256 | **`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 同（亦 = `StudioTestResourceScope.Expected` 常量 = 34 项基线之一） | ✅ |
| 文件 mtime | `2026-10-10T13:23:55.9520319Z` | — | 远早于本日全部会话，今日未被写 |
| `.meta` mtime | `2026-10-10T12:10:57.0892496Z` | — | 同样未动 |

### 2.2 已加载对象与全部子资产（`AssetDatabase.LoadAllAssetsAtPath` + `EditorUtility.IsDirty`，只读）

`subassetCount = 9`、**`dirtySubassets = 0`**（未 Save / 未 SetDirty / 未 ClearDirty / 未重导入 / 未覆盖内存对象 / 未做任何 `GetPreferredValues`·`ForceMeshUpdate` 探针）：

| 项 | 实测 | 判定 |
|---|---|---|
| `TMP_FontAsset` | `RRPG_SourceHanSansCN_Regular_TMP`，`IsDirty=False` | clean |
| `Material` | `RRPG_SourceHanSansCN_Regular_TMP Material`，`IsDirty=False` | clean |
| `Texture2D` ×7 atlas | `Atlas`…`Atlas 6`，均 `IsDirty=False` | clean |
| `atlasTextures.Length` | **8**（索引 0–6 为 7 张真实 atlas，索引 7 为 `null` 空闲槽） | 如实保留空闲槽，本包未写入 |
| `characterLookupTable.Count` | **644** | 与历史一致 |
| `characterTable.Count` | **632** | 与上游 `6fc321d4` 回执 `scope-restored\|font\|0\|632` 一致 |
| `atlasPopulationMode` | `Dynamic` | 未变 |
| `sourceFontFile` | `SourceHanSansCN-Regular` | 未变 |

**判定：字体磁盘字节 = 基线，内存字体对象与全部 9 子资产 0 dirty、0 pending 编辑。**未出现 `Font already has pending edits` 的实体证据；本包**不做**任何清理性保存 / 清 dirty / 重导入。

> 观察面限定（如实）：字体磁盘 hash 未变只证明**文件观察面**未被写，不推断“全机零副作用”（如需更强结论需独立的进程级 / 文件系统审计，不在本包授权内）。

---

## 3. 34 项基线 / 单槽存档 / guard（只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，`complete=true`，6866B / `bc7216f1…394e`） | 正则逐项重算 bytes + SHA256：**`total=34 / same=34 / diff=0 / missing=0`**（字体即 34 项之一） | **34/34 全一致** |
| 单槽存档（`SaveSystem.GetSavePath()` 反射）= `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json` | **`13405B`** / SHA256 **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**；mtime `2026-10-26T08:49:27.9808544Z`；`.tmp` **不存在** | 内容仍为 `13405-4b2b56`；mtime 正是本轮滚动单方法 run 的 guard 恢复时刻 |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **217** 个 run 目录：`restored=216`、`protective-snapshot=1`、**`noManifest=0`**、其它状态（`captured` / `restore-failed` / `unfinished` / unknown）**0** | **无 unfinished，无需恢复或清锁** |
| guard 最新 run | `20261026T084925Z`：`status=restored`、`readbackVerified=true`、`sourceSha256=4b2b56f6…d6aa`，manifest mtime `2026-10-26T08:49:27.9808544Z` | 与单槽 mtime、日志 08:49 段相接：上一轮单方法已真实恢复并回读 |

**本包未 Load / Save / 恢复任何存档，未移动或改写任何 guard run 或 manifest。**

---

## 4. 本轮（详情修复后）restore 与详情字符日志原文核对

### 4.1 主证据日志 `management-shell-play-evidence.log`

`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`：**`716215B / 3239 行`**，SHA256 `993f7b65ef65812a8ad1121f5a24533bf2f1d1c0d062969a3b8a9b147c57f161`，mtime `2026-10-26T08:49:28.3937823Z`；本轮为 **第 3217–3239 行**（上轮末行 3216 为 07:46:28 的 `[BASELINE-POST]`，历史行原样保留）。

| 行 | 时间（UTC） | 原文事实 |
|---|---|---|
| 3217 | 08:49:25.742 | `[BASELINE-PRE] total=34 diff=0 all-match` |
| 3218 | 08:49:25.912 | `[FONT-PRE] sha256=3c758e16…c404854` |
| 3219–3220 | 08:49:25.999 / 26.001 | `[SAVE-BACKUP]` / `[SAVE-CAPTURE] runId=20261026T084925Z … bytes=13405 sha256=4b2b56f6…d6aa readback-verified=True` |
| 3222 | 08:49:27.363 | `[CASE-B-SCHEDULE] REAL-SCROLL building rows fixture: captured=IntradayActivitySchedule original-points=[420,1020,1200] … commute=15 repeat=30 …` |
| 3231 | 08:49:27.495 | `[ADVANCE] targetMinute=14400 outcome=Completed` |
| 3236 | 08:49:27.980 | **`[SAVE-RESTORE] runId=20261026T084925Z existed=True sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa matches-before=True`** |
| 3237 | 08:49:27.986 | **`[SCHEDULE-RESTORE] restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}`** |
| 3238 | 08:49:28.200 | `[FONT-POST] sha256=3c758e16…c404854 unchanged=True` |
| 3239 | 08:49:28.393 | `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match` |

即：**本轮 run 的保护链完整**——先捕获（`captured` + `readback-verified`）→ 用例内日程捕获与恢复 → 结束时 `SAVE-RESTORE matches-before=True` → `SCHEDULE-RESTORE restored=true sameInstance=True` → `FONT-POST unchanged=True` → `BASELINE-POST 34/34`。日志末尾即 `[BASELINE-POST]`，**无 interrupted / unfinished / aborted 标记**。

### 4.2 滚动 / 详情字符日志 `real-scroll-evidence.log`

`Development/StudioRuns/R3B-UI-20261026/play-evidence/real-scroll-evidence.log`：**`12461B / 34 行`**，SHA256 `7b122d7da455352a5ce4a87381d44dd321e10a7e4addc0b04caba820790ed48d`，mtime `2026-10-26T08:49:27.9377876Z`。历史 1–18 行（07:39 / 07:46 两轮）原样保留；**本轮为第 19–34 行**。

| 行 | 事实（原文摘要） |
|---|---|
| 19 | `[REAL-SCROLL-OVERFLOW] overflow=True contentHeight=1004 viewportHeight=650 overflowPx=354 last=ConstructionRowTemplate(Clone) lastVisibleBefore=False`（行高 100→132 后真实内容自然变高，**非缩视口**） |
| 20 | `[REAL-SCROLL-WHEEL] beforeNorm=1 afterNorm=0.950565 contentY=0→17.5 point=(692,447)`（真实 `IScrollHandler`） |
| 21 | `[REAL-SCROLL-DRAG] beginDrag+Drag+endDrag=True strokes=1 steps=6 step=60 norm=0.950565→0 contentY=17.5→354`（**多轮真实拖拽**，指针始终在列表内，非直接赋 `normalizedPosition`） |
| 22 | `[REAL-SCROLL-LAST-ROW] rowAfter=(66,128,1252,92) center=(692,174) top=ConstructionRowTemplate(Clone) handler=ConstructionRowTemplate(Clone) visibleAfter=True` |
| 23–24 | `[REAL-SCROLL-TITLE]` 岗位 / 施工末行标题：`chars=59 visible=51 rendered=675.2119x30.408 preferredH=30.41 rect=(0,-32,1072,32) overlapsDetail=False rowFullyInsideViewport=True` / `chars=7 visible=5 rendered=114.4063x30.408` 同上 |
| 25–30 | `[REAL-SCROLL-DETAIL]` **6 条真实完工岗位行**：`text="绑定 0/2 · 实际劳动 0 · 现场 0\n岗位 workplace:building_instance_N:main\n建筑 building_instance_N · 槽位 main · 功能 ws_farm_01 · 绑定人：无"`、`lines=3`、**`preferredH=73.85`**、`rect=(0,-76,1072,76)`、**`checkedChars=51`**（逐字符 `isVisible` + 字符墨迹落行/视口）；第 1–3 行 `rowsInsideViewport=False`（滚动到末端后已滚出视口），第 4–6 行 `rowsInsideViewport=True` |
| 31 | `[REAL-SCROLL-DETAIL]` **真实施工行**：`text="建筑 building_instance_7 · 进度 0 / 10 · 施工中\n工人 1 人：林恩（person_adbcc…） · 现场 0 人"`、`lines=1`、`checkedChars=6`、`rowsInsideViewport=True` |
| 32 | `[REAL-SCROLL-COVERAGE] Workplace=True Construction=True Resource=NotCoveredByThisCase detail-key-values=Workplace6+Construction1` |
| 33 | `[REAL-SCROLL-SHOT] …T084927Z.png bytes=302672 sha256=5662a324…e3feeda5` |
| 34 | `[REAL-SCROLL-TITLE]` 点击岗位后施工标题仍 `visible=5 / overlapsDetail=False / rowFullyInsideViewport=True` |

**详情字符日志判定（异常扫描：无）**：本轮 6+1 条 detail 均 `lines` 与模型文本一致、`preferredH=73.85 ≤ rect 高 76`、`checkedChars=51 / 6 > 0`，无 `isTextOverflowing` 失败、无逐字符不可见、无日志自述与断言冲突；`rowsInsideViewport=False` 只出现在已滚出视口的前 3 行，与“该行完整在视口内才要求字符在视口内”的断言语义一致，**不是异常**。`Resource=NotCoveredByThisCase` 如实保留，**不写成资源行已覆盖**。

### 4.3 新截图（本执行层独立复核，非仅转述）

| 项 | 实测 |
|---|---|
| 路径 | `Development/StudioRuns/R3B-UI-20261026/play-evidence/management-real-scroll-workplaces-overflow-last-row-20261026T084927Z.png` |
| bytes / SHA256 | **302672** / `5662a324c5bb5fabe521967877a1dde85ba9c8f52d30d7b88da119dee3feeda5` |
| 像素（PNG IHDR 实测） | **1920x1080** |
| mtime（UTC） | `2026-10-26T08:49:27.8522453Z`（与日志第 33 行一致） |
| 本层 `read_image` 目视结论 | 列表 4 条可见岗位行均显示 3 行 detail：**首行 `绑定 0/2 · 实际劳动 0 · 现场 0`**、第二行 **完整 `岗位 workplace:building_instance_N:main`**、第三行 `建筑 building_instance_N · 槽位 main · 功能 ws_farm_01 · 绑定人：无`；施工行 `建筑 building_instance_7 · 进度 0 / 10 · 施工中…`；标题 `农田 · building_instance_N · 实例 building_instance_N · 槽位 main` 完整；右栏 `所选人物 · 林恩` 与 4 个命令按钮在位。**与主开发目视结论一致** |
| 历史图未被覆盖 | `…T073954Z.png` **269447B / `973fa3c2…29b3e` / mtime 07:39:54.109**；`…T074624Z.png` **268936B / `c5cf85f2…a9934` / mtime 07:46:24.683**；均原样在位 |

---

## 5. 源码 / Prefab / DLL / 13 例发现（只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | **`38207B / 9b59393620c63ddf62cb924ad79e5e7f24a22c717d85c68da99c2dbaf07c8fcb`**，mtime `2026-10-26T08:44:37.2308738Z` | 与任务给定当前 SHA 逐字一致；本包未改 |
| `Assets/Scripts/Village/ProductHome/ManagementRowView.cs` | `1670B / d5a2456a…43eebd`，mtime 2026-10-23T12:02:15Z | 未改 |
| `Assets/Resources/UI/ManagementView.prefab` | **`282326B / a05a7715e83e5c552c0472fa6465dbe440a5ba623d3969297b7ff8c4a4a0139a`**，mtime `2026-10-26T08:44:42.4329323Z` | 与任务给定当前 SHA 逐字一致；本包**未改、未 Refresh、未开 PrefabStage** |
| `ManagementView.prefab.meta` | `178B / b1c14b9c…196559`，mtime 2026-10-23T12:10:23Z | 未改 |
| Prefab 文本计数 | `m_SizeDelta: {x: 1072, y: 42}` = **3**；`{x: 1072, y: 76}` = **1**；`{x: 1072, y: 32}` = **4**；`{x: 1072, y: 28}` = **0**；`{x: 1100, y: 100}` = **0**；`{x: 1100, y: 132}` = **1**；`m_PreferredHeight: 132` 命中 1 处（行 3284）、`m_AnchoredPosition: {x: 14, y: -44}` 命中 1 处（行 3619） | 与详情修复包记录一致：仅 `WorkplaceRowTemplate` 加高（100→132）、其 Detail 42→76@-44；其余三模板仍 `1072×42` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **`371699B / 03a7c2c8a4d9a5eff8cc59f6d4e20e05eeab38e46e7adffb2d45d2f35cabf0ee`**，mtime `2026-10-26T08:47:13.4457833Z` | 与任务给定当前 SHA 逐字一致；本包未改 |
| 源码属性计数 | `[UnityTest]=13`、`[UnityTearDown]=0`、`Screen.SetResolution=0`、`new Vector2Int(1280` = 0 | 无尺寸写入、无无意义全局恢复、1280 分支为 0 |
| 测试程序集 | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：**`501760B / 81f83defb8de116ba8054e49c39b68e5fe4fa045ad34257e86522ab99a315982`**，mtime `2026-10-26T08:47:49.0712713Z`（**晚于源码 35.63 s**） | DLL 新鲜、已含第 13 例强化版；本层两次独立重算同值 |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | `648192B / cab1fbefd442e297db6e67baac6c2388c492254515be301b9e628291af28cc9d`，mtime `2026-10-26T08:44:45.6741843Z`（**晚于 `ManagementView.cs` 2.44 s**） | 产品程序集已含详情排序 / 行高改动 |
| 发现（整类） | `tests \| where fullName contains "ManagementShellPlayTests"` = **13 条**，`mode` 全 `PlayMode`、`assembly` 全 `BattleStage1.PlayModeTests` | 与源码 `[UnityTest]=13` 一致 |

**关于 DLL 哈希与上包记录值不同的如实记录**：`workplace-detail-readability-review.md` §4.1 记录为 `efc274212d90…0ac2a17`（501760B，mtime 08:47:49Z）；本层两次独立重算均为 `81f83def…315982`（**同一字节数、同一 mtime**）。**本包无法从只读面确定差异原因**（Unity 重编译会改变 MVID，但通常同时更新 mtime），因此只把它当作“**当前 DLL 晚于当前源码、已含 13 例**”的判据，**不据此推断编译次数，也不宣称上包记录错**。

13 例 FQN（`RRPG.ManagementShell.Tests.ManagementShellPlayTests.` 前缀）：`CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`、`ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement`、`ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak`、`FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt`、`ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`、`ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`、`ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock`、**`ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`**、`ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite`、`Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip`、`PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`、`SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous`、`WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes`。

---

## 6. 门禁口径（禁止合并 / 禁止外推）

| 门禁 | 结果 | 版本范围 |
|---|---|---|
| 详情修复后滚动单方法 | 上游 `49fcb102…` **1/1 Passed、0 skipped、3.2231345 s、scope-restored**（主开发转述）；本机 `testResults` 单条 `Passed / 2092ms`、日志 / 截图 / guard 时间戳一致 | **单例真实通过（仅此一例）**，不并入整类 |
| 原 13 例整类 | 详情修复前 `6fc321d4` 13/13（历史，主开发转述） | **历史**，不并入本轮 |
| 整类 13 例（最新代码） | 13 例发现、DLL 新鲜、保护面 clean、宿主空闲 | **前置满足，结果待宿主产出**；本文件不预填 |
| 管理 Edit 专项 | 最新 **18/18**（前阶段真实通过） | 前阶段；本包未重跑 |
| Compiler / 完整 Edit / 完整 Play | `264/264`、`1513/1513`、`122/122`（前阶段真实通过） | **前阶段，不是最新全量**；本包未重跑 |
| 1280×720 | 制作人记录 `01a53eb8` **取消** | **取消 ≠ 通过**，不得由 1920 / 单例外推 |

---

## 7. 已知未覆盖（如实保留，不升级为“通过”）

1. **资源页业务行标题 / 第二 `FunctionSlot` Gateway 仍未覆盖**：`Resource=NotCoveredByThisCase`；`BuildingConfigDatabase.asset` 只登记 `FunctionBindings: main`，无合法注入点。
2. **物理鼠标 / 手感**：本轮“真实滚轮 + 多轮拖拽”是 `EventSystem` 程序化路径（日志原文即“非物理鼠标”），**程序化 ≠ 物理手感**；真实指针手感仍需制作人手工观察。
3. **动态批次**：Deferred（后置），本包未做。
4. **历史 650 存档缺口**：`21075B / 650c45c4…` 在授权范围内无字节备份、未恢复、未伪造；当前 `13405-4b2b56` **不是它**。
5. **Chevron**：仍 `44×28`、字号 24，未修未测。
6. **Person / Resource 行 detail** 仍 `1072×42` 单行；施工行 detail 第二行（工人列表）在列表行内仍不可见——本任务只覆盖岗位行关键数值，**不称其已可读**。
7. **历史异常保留（不可洗白）**：① 旧 `4fbf7754` 的 **`EditorUtility.ClearDirty` 违规**原文留存；② **上一包（详情修复）`AssetDatabase.SaveAssets()` 越界调用**（`workplace-detail-readability-review.md` §6.3 原文留存，已核当时场景 / 字体 / 单槽 / 34 项无额外写入，但调用本身仍按违规记录）；③ `ManagementView.prefab` 历史插件自动回写 `282097B→282326B`（无备份、不逐字节还原）。**本包未重复上述任何操作**，也未用本包结果为其“洗白”。

---

## 8. 移交宿主：整类 13 例串行 PlayMode

前置已满足：`IsRunActive=False`、`testResults` 仅 1 条单方法、场景 `dirty=False`、`PrefabStage=null`、`Screen=1920x1080`、字体 `14961033B / 3c758e16…c404854` 且 9 子资产 0 dirty、34/34、单槽 `13405-4b2b56`、guard 217（216 restored + 1 protective，无 unfinished）、13 例发现、DLL 新鲜。

1. 以**短类名** `ManagementShellPlayTests` 运行 **PlayMode 整类 13 例、串行**；运行前真实 `Screen` 必须仍为 `1920x1080`（用例自身严格断言，**不请求 / 不恢复 / 不缩放**尺寸）。
2. 逐例核对：每例 `[SAVE-RESTORE]`（含 `runId`、`sha256`、`matches-before=True`）在案；两例日程用例（`ManagementConstructionCommands_…`、`PendingEvent_…`）需 `[SCHEDULE-RESTORE] restored=true sameInstance=True`；第 13 例需本轮新增 `[REAL-SCROLL-*]` 与**新时间戳**截图，不得覆盖 `T073954Z` / `T074624Z` / `T084927Z` 及更早历史图。
3. 运行后复核：字体仍 `14961033B / 3c758e16…` 且内存 0 dirty、单槽仍 `13405-4b2b56`、guard 无 `unfinished`、场景 `isDirty=False`。
4. 任一例失败：保留真实 `message / stackTrace`、用例名与新增日志行，**不以“已修复”代替回执**，不写成本报告 PASS。
5. 本文件及结论须由主开发专业核验，**不代表制作人批准**；R3-B 仍为 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

---

## 9. 本包边界与实际副作用

- **唯一写入**：本文件（新增）；不覆盖任何历史截图 / 文档 / 日志。
- 唯一 Editor 操作：`rql-query` 只读查询、`script-execute` **只读** C#（读文件 / 重算 SHA256 / 解析 PNG IHDR / 反射读 `IsRunActive` 与 `SaveSystem.GetSavePath` / `EditorUtility.IsDirty` 只读判定 / 目录与 manifest 统计 / 读 `DateTime`）、对本地 PNG 的 `read_image` 目视复核、对本地文本文件的 `read` / `grep`。
- **未** `AssetDatabase.Refresh`、**未** `AssetDatabase.SaveAssets / SaveChanges / SaveDirtyScenes`、**未** `SetDirty`、**未** `ClearDirty`、**未**打开 `PrefabStage`、**未** `GetPreferredValues` / `ForceMeshUpdate` 预检探针、**未** `rql-mutate`、**未** `tests-run`、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor、**未**调 `StudioTestResourceScope`。
- 未改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 图集 / 34 项基线 / 存档本体 / guard manifest / 配置 / GameSpec / Authority / 备份 / 正式工程；未安装组件、未改权限、未创建额外付费子 Agent；未访问未授权工程 / 认证目录 / 其他工作区。
- 不做文档全集收尾（6 索引更新属阶段 2 且仅在宿主通过后）；本阶段只新增本文件。

---

## 10. 写后只读复核（2026-10-26T08:52:45Z，证明本包无副作用）

| 项 | 写后实测 | 判定 |
|---|---|---|
| Editor / PrefabStage | `isPlaying=False`、`isCompiling=False`、`isUpdating=False`、`PrefabStage=null` | 仍空闲、未开 PrefabStage |
| 场景 | `SampleScene`：`dirty=False`、`roots=5` | 仍干净 |
| Screen | `1920x1080` | 未变（未切 1280、未改 Prefs） |
| 原生 TestRunner | `IsRunActive=False` | 未新增运行 |
| 字体磁盘 | **`14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**，mtime `2026-10-10T13:23:55.9520319Z` | 未变 |
| 字体内存 | 9 子资产、`dirty=0` | 仍 0 dirty |
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`，mtime `2026-10-26T08:49:27.9808544Z`、`.tmp=false` | 未变，仍是上一轮恢复后的原字节 |
| guard | `dirs=217`：`restored=216`、`protective-snapshot=1`；无 `unfinished` / `noManifest` | 未新增 run、未改 manifest |
| 本文件（追加本节前） | `23098B / 89af08d600351d3df1c25acb704e6950319d11603582be91f8f577ba971a161a`，mtime `2026-10-26T08:52:32.7623147Z` | 追加本节后哈希变化，最终值以目录实际文件为准，不据旧值宣称未改 |

**唯一新增产物**为本文件；整类 13 例真实回执尚未产生，等待宿主串行运行。本文件及结论须由主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。
