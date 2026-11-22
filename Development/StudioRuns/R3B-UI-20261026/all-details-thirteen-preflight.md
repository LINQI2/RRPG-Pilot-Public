# 全部必要明细修复后 · ManagementShellPlayTests 整类 13 例 PlayMode 集成预检（2026-10-26，阶段 1/2）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 预检时刻：本轮只读实测窗口 **2026-10-26T10:27:16Z–10:31Z**（本机同日 18:27 +08:00）。下列每项均为本轮实测；上游转述与本机实测分开标注。
- 本包范围：**只读严格安全预检 + 读本轮单方法 run 的日志 / 截图 / 保护链原文 + 仅新增本文件**。**未改任何代码 / 测试 / 断言 / Prefab / Scene / 字体 / 图集 / 配置 / 存档 / guard manifest / 34 项基线**；**未** `AssetDatabase.Refresh`、**未** `PrefabStage`、**未** `rql-mutate`、**未** `tests-run`（模型不 tests-run）、**未** `AssetDatabase.SaveAssets / SaveChanges / SaveDirtyScenes`、**未** `SetDirty`、**未** `ClearDirty`、**未** `GetPreferredValues` / `ForceMeshUpdate` 预检探针、**未** Shell / Git 写、**未**写 Preferences、**未**启停 Editor。
- 上游记录（主开发已核，本执行层读不到 Studio 私有 runtime，按“转述”标注）：本包前滚动专项单方法 run **`e75c1ec5-d51c-4239-88f0-4ec5dc1ee4ae`：1/1 Passed、0 skipped、`3.1183267` s、`scope-restored`**；主开发已实际查看 `T102528Z` 资源与人口截图（资源 6 条真实行名额 / 劳动 / 实例 / 槽位可读；人口职责 / 执行 / 承诺 / 饥饿可读）。
- 结论前置：**字体磁盘 = 基线且内存 9 子资产 0 dirty、34/34、单槽 `13405-4b2b56`、guard 231 run 全部终态合法、Pilot 原生空闲、场景干净、13 例发现且 DLL 新鲜**；满足宿主整类串行运行前置。**本文件不预填 13 例结果。**

---

## 1. 身份、工程空闲与场景（只读实测）

| 项 | 实测值（2026-10-26T10:27:16Z 起） | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9`；`dataPath = D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`；`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 原生 TestRunner | 反射 `UnityEditor.TestTools.TestRunner.Api.TestRunnerApi.IsRunActive()` = **False**（`static`、`params=0`、`ret=Boolean`，需 `NonPublic` 绑定才能取到；仅用 `Public` 绑定会得到 method-not-found，属探针绑定口径，非运行态异常） | 无活动 / 残留运行 |
| 场景 | 活动场景 `Assets/Scenes/SampleScene.scene`：`isDirty=False`、`rootCount=5`、`loaded=True`、`sceneCount=1` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开（本包不打开） |
| Screen | **`1920x1080`、`fullScreen=false`**；`currentResolution=2560x1600`（桌面分辨率，非渲染尺寸） | 满足宿主 1920 前置；**未切 1280、未改 Prefs** |
| `testResults` | **1 条**：`ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`，`Passed / 2144ms / message=null` | 本轮单方法回执的本机可核对部分；**不带 runId / skipped，不能当整类证据** |
| Console（`Error`/`Exception`） | **0 条** | 无项目 / 测试程序集错误；无残留异常 |

口径说明：本机 `2144ms` 是方法内耗时；主开发转述的 `3.1183267 s` 是 run 级总时长，两者**口径不同**，分别标注、不互相替代。

---

## 2. 受保护字体：磁盘 / 内存 / 子资产（安全门禁，只读）

### 2.1 磁盘（重算字节 + SHA256，未写文件）

| 项 | 实测值 | 要求值 | 判定 |
|---|---|---|---|
| 路径 | `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | 同 | ✅ |
| bytes | **14961033** | 14961033 | ✅ |
| SHA256 | **`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 同（= 34 项基线之一） | ✅ |
| 文件 mtime | `2026-10-10T13:23:55.9520319Z` | — | 远早于本日全部会话，今日未被写 |
| `.meta` | `221B`，mtime `2026-10-10T12:10:57.0892496Z` | — | 同样未动 |

### 2.2 已加载对象与全部子资产（`AssetDatabase.LoadAllAssetsAtPath` + `EditorUtility.IsDirty`，只读）

`subassetCount = 9`、**`dirtySubassets = 0`**（未 Save / 未 SetDirty / 未 ClearDirty / 未重导入 / 未覆盖内存对象 / 未做任何 `GetPreferredValues`·`ForceMeshUpdate` 探针）：

| 项 | 实测 | 判定 |
|---|---|---|
| `TMP_FontAsset` | `RRPG_SourceHanSansCN_Regular_TMP`，`IsDirty=False` | clean |
| `Material` | `RRPG_SourceHanSansCN_Regular_TMP Material`，`IsDirty=False` | clean |
| `Texture2D` ×7 atlas | `… Atlas` … `… Atlas 6`，均 `IsDirty=False` | clean |
| `atlasTextures.Length` | **8**：索引 0–6 为 7 张真实 atlas，索引 7 为 `instanceID=0` 的空槽对象（Unity 语义下 `== null`） | 如实保留空槽，本包未写入 |
| `characterTable.Count` | **632** | 与历史一致 |
| `characterLookupTable.Count` | **644** | 与历史一致 |
| `atlasPopulationMode` | `Dynamic` | 未变 |
| `sourceFontFile` | `SourceHanSansCN-Regular` | 未变 |

**判定：字体磁盘字节 = 基线，内存字体对象与全部 9 子资产 0 dirty、0 pending 编辑。**本包**不做**任何清理性保存 / 清 dirty / 重导入。

> 观察面限定（如实）：字体磁盘 hash 未变只证明**文件观察面**未被写，不推断“全机零副作用”（更强结论需独立进程级 / 文件系统审计，不在本包授权内）。

---

## 3. 34 项基线 / 单槽存档 / guard（只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，`complete=true`，6866B / `bc7216f125aa0046d116ad23d71e33bd15d9e31d861ce3c317e614429540394e`） | 正则逐项重算 bytes + SHA256：**`total=34 / same=34 / diff=0 / missing=0`**（字体即 34 项之一） | **34/34 全一致** |
| 单槽存档（`SaveSystem.GetSavePath()` 反射）= `1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot\rrpg_save_p0.json` | **`13405B`** / SHA256 **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**；mtime `2026-10-26T10:25:28.3871461Z`；`.tmp` **不存在** | 内容仍为 `13405-4b2b56`；mtime 正是本轮单方法 run 的 guard 恢复时刻 |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **231** 个 run 目录：`restored=230`、`protective-snapshot=1`、**`noManifest=0`**、其它状态（`captured` / `restore-failed` / `unfinished` / unknown）**0** | **无 unfinished，无需恢复或清锁** |
| guard 最新 run | `20261026T102526Z`：`status=restored`、`readbackVerified=true`、`sourceSha256=4b2b56f6…d6aa`，manifest mtime `2026-10-26T10:25:28.3871461Z` | 与单槽 mtime、日志 10:25 段相接：上一轮单方法已真实恢复并回读 |

**本包未 Load / Save / 恢复任何存档，未移动或改写任何 guard run 或 manifest。**

---

## 4. 本轮（全部明细补齐后）单方法 run 原文核对

### 4.1 保护链日志 `management-shell-play-evidence.log`

`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`：**`763085B / 3452 行`**，SHA256 `269703951fca6bc11787073b00c063053850717f9109227a1f8a7426399dc235`，mtime `2026-10-26T10:25:28.7122628Z`；本轮为 **第 3429–3452 行**（第 3428 行为 08:53:30 的上一轮 `[BASELINE-POST]`，历史行原样保留）。

| 行 | 时间（UTC） | 原文事实 |
|---|---|---|
| 3429 | 10:25:26.116 | `[BASELINE-PRE] total=34 diff=0 all-match` |
| 3430 | 10:25:26.257 | `[FONT-PRE] sha256=3c758e16…c404854` |
| 3431–3432 | 10:25:26.34 | `[SAVE-BACKUP]` / `[SAVE-CAPTURE] runId=20261026T102526Z … bytes=13405 sha256=4b2b56f6…d6aa readback-verified=True` |
| 3433 | 10:25:27.407 | `[ENTRY] Product entry reached World; clock=Paused; saveGuard=active` |
| 3442 | 10:25:27.528 | `[REAL-SCROLL] placed=7 batch1=6 participants=[6 人] farms=[building_instance_1…6]` |
| 3444 | 10:25:27.556 | `[REAL-SCROLL] completed=6; construction=building_instance_7 worker=person_e604… progress=0 labor=0` |
| 3446–3448 | 10:25:27.9–28.24 | 岗位 / 施工 / 资源行点击均 `selected=True`，资源点击后 `tab=Workplaces`（定位回“职责与场所”） |
| 3449 | 10:25:28.387 | **`[SAVE-RESTORE] runId=20261026T102526Z existed=True sha256=4b2b56f6…d6aa matches-before=True`** |
| 3450 | 10:25:28.392 | **`[SCHEDULE-RESTORE] restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}`** |
| 3451 | 10:25:28.550 | `[FONT-POST] sha256=3c758e16…c404854 unchanged=True` |
| 3452 | 10:25:28.711 | `[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match` |

即：**本轮 run 的保护链完整**——先捕获（`captured` + `readback-verified`）→ 用例内日程捕获与恢复 → 结束时 `SAVE-RESTORE matches-before=True` → `SCHEDULE-RESTORE restored=true sameInstance=True` → `FONT-POST unchanged=True` → `BASELINE-POST 34/34`。日志末尾即 `[BASELINE-POST]`，**无 interrupted / unfinished / aborted 标记**。

### 4.2 四类业务行渲染日志 `real-scroll-evidence.log`

`Development/StudioRuns/R3B-UI-20261026/play-evidence/real-scroll-evidence.log`：**`34053B / 88 行`**，SHA256 `897f9c67c0fb6908424fd129529b3270229258af208a2753a3c616b32bdc947e`，mtime `2026-10-26T10:25:28.3674353Z`。历史 1–50 行（07:39 / 07:46 / 08:49 / 08:53 四轮）原样保留；**本轮为第 51–88 行**。

| 行 | 原文事实（摘要） |
|---|---|
| 51 | `[REAL-SCROLL-OVERFLOW] overflow=True contentHeight=1044 viewportHeight=650 overflowPx=394 last=ConstructionRowTemplate(Clone) lastVisibleBefore=False`（施工行 92→132 后真实内容再增高，**非缩视口**） |
| 52 | `[REAL-SCROLL-WHEEL] beforeNorm=1 afterNorm=0.9555838 contentY=0→17.5`（真实 `IScrollHandler`） |
| 53 | `[REAL-SCROLL-DRAG] beginDrag+Drag+endDrag=True strokes=1 steps=7 step=60 norm=0.9555838→0 contentY=17.5→394`（指针始终在列表内，非直接赋 `normalizedPosition`） |
| 54 | `[REAL-SCROLL-LAST-ROW] rowAfter=(66,128,1252,132) center=(692,194) top=ConstructionRowTemplate(Clone) visibleAfter=True` |
| 55–56 | 岗位 / 施工末行标题：`chars=59 visible=51` / `chars=7 visible=5`，`preferredH=30.41`、`overlapsDetail=False`、`rowFullyInsideViewport=True` |
| 57–62 | **6 条真实岗位行**：`绑定 0/2 · 实际劳动 0 · 现场 0\n岗位 workplace:building_instance_N:main\n建筑 building_instance_N · 槽位 main · 功能 ws_farm_01 · 绑定人：无`、`lines=3`、`preferredH=73.85`、`rect=(0,-76,1072,76)`、`checkedChars=51` |
| 63 | **真实施工行 3 行**：`建筑 building_instance_7 · 进度 0 / 10 · 施工中\n工人 1 人：林恩（person_e604…）\n现场 0 人`、`lines=3`、**`checkedChars=58`**（工人姓名 / 人数 / 现场均逐字符可见） |
| 66, 67–72, 77–78 | **6 条真实资源行**：标题 `Farm · 农田 · building_instance_N`（`chars=31 visible=27`）；detail `名额 绑定 0/2 · 实际劳动 0\n建筑 building_instance_N · 槽位 main\n点击定位到“职责与场所”的同一行…`、`lines=3`、`checkedChars=41` |
| 73–76 | 资源页真实 overflow：`contentHeight=834 / viewport 650 / overflowPx=184`；滚轮 `1→0.9048913`；拖拽 `steps=3 norm→0 contentY 17.5→184`；末行 `visibleAfter=True` |
| 80–87 | **6 条真实人口行**：`在村 · 职责：无主要职责\n执行：…\n临时承诺：无 · 饥饿：Hungry（连续不足 10 天）`、`lines=3`、`checkedChars=40/37`；首行标题 `林恩 · profession_test_c` `chars=22 visible=20`、`overlapsDetail=False` |
| 88 | `[REAL-SCROLL-COVERAGE] Workplace=True Construction=True Resource=True Population=True workplaceDetail=6 constructionDetail=1 workerNames=1 resourceDetail=6 populationDetail=6` |

**异常扫描：无。**本轮日志中 `rowsInsideViewport=False` 只出现在滚动后已移出视口的行（岗位第 1–3 行、资源第 5–6 行、人口第 5–6 行），与“行完整在视口内才要求字符落视口”的断言语义一致；`Resource=NotCoveredByThisCase` 在本轮日志中 **0 次**（历史行内保留），四类业务行均来自真实世界快照。

### 4.3 三张新截图（本执行层独立复核，非仅转述）

| 路径（`Development/StudioRuns/R3B-UI-20261026/play-evidence/`） | bytes | SHA256 | 像素 / mtime（UTC） |
|---|---|---|---|
| `management-real-scroll-workplaces-overflow-last-row-20261026T102527Z.png` | **308548** | `ad70d54ca278d4656486beb99fb56954dfe4f7485b2e8744d132fdd777fd62c9` | 1920x1080 / 10:25:27.840 |
| `management-real-scroll-resources-real-rows-20261026T102528Z.png` | **312405** | `ce87a9603df63d77266d5e61c20f9b0f7857d282a114acb780e5dc861d17e8c0` | 1920x1080 / 10:25:28.165 |
| `management-real-scroll-population-detail-facts-20261026T102528Z.png` | **273710** | `8a6c67c5f7b133849f9f54346ad41aa7c08914ae791682b0d99251b90569e4e8` | 1920x1080 / 10:25:28.345 |

本层 `read_image` 目视结论（与主开发结论一致）：

- **资源页**：`食材 0 · 食物 0 · 在村供餐 6 人 · 其中饥饿 6 人`；5 条可见资源行均为 3 行 detail（首行 `名额 绑定 0/2 · 实际劳动 0`、次行 `建筑 building_instance_N · 槽位 main`、第三行定位说明），标题 `Farm · 农田 · building_instance_N` 完整；右栏“所选人物 · 林恩”在栏。
- **人口页**：`共 6 人 · 当前显示 6 人 · 在村 6 人`；5 条可见行姓名 + 职业完整，detail 三行 `在村 · 职责：…` / `执行：…` / `临时承诺：无 · 饥饿：Hungry（连续不足 10 天）`；右栏派工目标显示 `绑定 0/2 · 实际劳动 0 · 现场 0` 与完整 `workplace:building_instance_6:main`。
- **职责与场所页**：岗位行三行 detail 完整；施工行 `建筑 building_instance_7 · 进度 0 / 10 · 施工中` / `工人 1 人：林恩（person_e604…）` / `现场 0 人` 三行均可见。
- 历史截图未被覆盖：`T085326Z` **302878B / `008044db…`**、`T084927Z` **302672B / `5662a324…`**、`T074624Z` **268936B / `c5cf85f2…`**、`T073954Z` **269447B / `973fa3c2…`**，均原样在位。

---

## 5. 源码 / Prefab / DLL / 13 例发现（只读重算）

| 项 | 实测值 | 判定 |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | **`39037B / f8316e436845bd7266cfd3a6f0fa013cdcd3acb5705161a94d5c67e172016473`**，mtime `2026-10-26T10:21:54.3087834Z` | 与 `remaining-row-details-review.md` 记录逐字一致；本包未改 |
| `Assets/Scripts/Village/ProductHome/ManagementRowView.cs` | `1670B / d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd`，mtime 2026-10-23T12:02:15Z | 未改 |
| `Assets/Resources/UI/ManagementView.prefab` | **`282332B / 31bf3e8c111700332adfaa87d93586ea065631090416cdc2f8cf2629a30e8764`**，mtime `2026-10-26T10:21:47.9465950Z` | 与记录一致；本包**未改、未 Refresh、未开 PrefabStage** |
| `ManagementView.prefab.meta` | `178B / b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559`，mtime 2026-10-23T12:10:23Z | 未改 |
| Prefab 文本计数 | `1072×32` = **4**、`1072×28` = **0**、`1072×42` = **0**、`1072×76` = **4**；`1100×132` = **4**（`1100×100/92/84/86` 均 0）；`m_PreferredHeight: 132` = **4**；`m_AnchoredPosition {14,-44}` = **4**、`{14,-8}` = **4** | 四个行模板统一 `132 / 132 / Detail 1072×76@(14,-44)`、Label `1072×32@(14,-8)` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **`390868B / 219379e01567b36cab0af7f73aee9f133bb027abcf1566be1c69f39f51c7a45c`**，mtime `2026-10-26T10:24:36.3694583Z` | 与记录一致；本包未改 |
| 测试源码计数 | `[UnityTest]`=**13**、`[UnityTearDown]`=0、`Screen.SetResolution`=0、`new Vector2Int(1280`=0、`Resource=NotCoveredByThisCase`=0；`GetPreferredValues`=5、`ForceMeshUpdate`=8（**仅出现在 Play 断言 helpers**，未在预检 / 产品代码） | 13 例、无尺寸写入、资源覆盖已改为 True 分支 |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | `647680B / b31f1b6408846a37deb2d948b840897189ef981f25c5ff9e514316f860dbe271`，mtime `2026-10-26T10:23:04.4074813Z`（**晚于 `ManagementView.cs` 70.1 s**） | 产品程序集已含四类行高 / 分行改动 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | **`512512B / 40e41b98299322e32f5916df5376861e81645426ebe352784de258d85ea2ea3b`**，mtime `2026-10-26T10:24:40.5953248Z`（**晚于测试源码 4.23 s**） | DLL 新鲜、已含最新第 13 例 |
| `Library/ScriptAssemblies/VillageStage1.Tests.dll` | `153088B / 9d09dabbb127fc58235e271d1029c2b57ae2c155d9d3dc1fab08b86fa65b69da`，mtime `2026-10-26T03:08:50.4871113Z` | 本轮未改 Edit 测试，未重编译 |
| 发现（整类） | `tests \| where fullName contains "ManagementShellPlayTests"` = **13 条**，`mode` 全 `PlayMode`、`assembly` 全 `BattleStage1.PlayModeTests` | 与源码 `[UnityTest]=13` 一致 |

13 例 FQN（`RRPG.ManagementShell.Tests.ManagementShellPlayTests.` 前缀）：`CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`、`ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement`、`ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak`、`FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt`、`ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`、`ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`、`ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock`、**`ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`**、`ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite`、`Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip`、`PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`、`SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous`、`WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes`。

---

## 6. 门禁口径（禁止合并 / 禁止外推）

| 门禁 | 结果 | 版本范围 |
|---|---|---|
| 全部明细补齐后滚动单方法 | 上游 `e75c1ec5…` **1/1 Passed、0 skipped、`3.1183267` s、scope-restored**（主开发转述）；本机 `testResults` 单条 `Passed / 2144ms`、日志 / 截图 / guard 时间戳一致 | **单例真实通过（仅此一例）**，不并入整类 |
| 原 13 例整类（08:53） | 13/13（本机 `testResults` 曾 13 条 Passed、日志 13 组 `[SAVE-RESTORE]`） | **历史**：该轮回执产生于 10:21 代码 / 10:24 测试改动**之前**，不覆盖当前版本 |
| 整类 13 例（最新代码） | 13 例发现、DLL 新鲜、保护面 clean、宿主空闲 | **前置满足，结果待宿主产出**；本文件不预填 |
| 管理 Edit 专项 | 最新 **18/18**（前阶段真实通过） | 前阶段；本包未重跑（Edit 源码本轮未改，DLL 时间戳 03:08Z 早于本包） |
| Compiler / 完整 Edit / 完整 Play | `264/264`、`1513/1513`、`122/122`（前阶段真实通过） | **前阶段，不是最新全量**；本包未重跑 |
| 1280×720 | 制作人记录 `01a53eb8` **取消** | **取消 ≠ 通过**，不得由 1920 / 单例外推 |

---

## 7. 已知未覆盖（如实保留，不升级为“通过”）

1. **第二 `FunctionSlot` 真实 Gateway**：`BuildingConfigDatabase.asset` 只登记 `FunctionBindings: main`，`StaticConfigRegistry` 无合法注入点——属**建筑 FunctionSlot 配置面限制**，与单槽存档 / guard 无关。
2. **物理鼠标 / 手感**：本轮“真实滚轮 + 多轮拖拽”仍是 `EventSystem` 程序化路径，**程序化 ≠ 物理手感**；真实指针手感需制作人手工观察。
3. **Chevron**：仍 `44×28`、字号 24，非必要指示，未修未测、未纳入断言。
4. **动态批次 / 供餐细节**：Deferred（后置），本包未做。
5. **历史 650 存档缺口**：`21075B / 650c45c4…` 在授权范围内无字节备份、未恢复、未伪造；当前 `13405-4b2b56` **不是它**。
6. **`SaveSlotGuard` 进程级缺口**：仍只在用例收尾（`[TearDown]` 路径）恢复，进程崩溃场景无兜底。
7. **历史异常保留（不可洗白）**：① 旧 `4fbf7754` 的 **`EditorUtility.ClearDirty` 违规**；② 详情修复包 **`AssetDatabase.SaveAssets()` 越界调用**（`workplace-detail-readability-review.md` §6.3 原文留存）；③ `ManagementView.prefab` 历史插件自动回写 `282097B→282326B`（无备份、根因未定）。**本包未重复上述任何操作**，也未用本包结果为其“洗白”。

> 本轮预检只证明“当前版本四类业务行在单例 run 中真实可读”，**不**代表整类 13 例已通过，也**不**代表制作人产品验收完成。

---

## 8. 移交宿主：整类 13 例串行 PlayMode

前置已满足：`IsRunActive=False`、`testResults` 仅 1 条单方法、场景 `dirty=False`、`PrefabStage=null`、`Screen=1920x1080`、字体 `14961033B / 3c758e16…c404854` 且 9 子资产 0 dirty、34/34、单槽 `13405-4b2b56`、guard 231（230 restored + 1 protective，无 unfinished）、13 例发现、DLL 新鲜。

1. 以**短类名** `ManagementShellPlayTests` 运行 **PlayMode 整类 13 例、串行**；运行前真实 `Screen` 必须仍为 `1920x1080`（用例自身严格断言，**不请求 / 不恢复 / 不缩放**尺寸）。
2. 逐例核对：每例 `[SAVE-RESTORE]`（含 `runId`、`sha256`、`matches-before=True`）在案；两例日程用例（`ManagementConstructionCommands_…`、`PendingEvent_…`）需 `[SCHEDULE-RESTORE] restored=true sameInstance=True`；第 13 例需本轮新增 `[REAL-SCROLL-*]`（四类覆盖全 True）与**新时间戳**截图，不得覆盖 `T073954Z` / `T074624Z` / `T084927Z` / `T085326Z` / `T102527Z` / `T102528Z`。
3. 运行后复核：字体仍 `14961033B / 3c758e16…` 且内存 0 dirty、单槽仍 `13405-4b2b56`、guard 无 `unfinished`、场景 `isDirty=False`。
4. 任一例失败：保留真实 `message / stackTrace`、用例名与新增日志行，**不以“已修复”代替回执**，不写成本报告 PASS。
5. 本文件及结论须由主开发专业核验，**不代表制作人批准**；R3-B 仍为 **Implemented / Verification PARTIAL / Not Committed / Not Published**。

---

## 9. 本包边界与实际副作用

- **唯一写入**：本文件（新增）；不覆盖任何历史截图 / 文档 / 日志。
- 唯一 Editor 操作：`rql-query` 只读查询、`script-execute` **只读** C#（读文件 / 重算 SHA256 / 解析基线 JSON / 解析 PNG IHDR / 反射读 `IsRunActive` 与 `SaveSystem.GetSavePath` / `EditorUtility.IsDirty` 只读判定 / 目录与 manifest 统计 / 读 `DateTime` / `AssetDatabase.LoadAllAssetsAtPath` 只读加载）、对本地 PNG 的 `read_image` 目视复核、对本地文本文件的 `read` / `grep`。
- **未** `AssetDatabase.Refresh`、**未** `AssetDatabase.SaveAssets / SaveChanges / SaveDirtyScenes`、**未** `SetDirty`、**未** `ClearDirty`、**未**打开 `PrefabStage`、**未** `GetPreferredValues` / `ForceMeshUpdate` 预检探针、**未** `rql-mutate`、**未** `tests-run`、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor。
- 未改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 图集 / 34 项基线 / 存档本体 / guard manifest / 配置 / GameSpec / Authority / 备份 / 正式工程；未安装组件、未改权限、未创建额外付费子 Agent；未访问未授权工程 / 认证目录 / 其他工作区。
- 不做文档全集收尾（6 索引与交付索引更新属阶段 2 且仅在宿主 13 例通过后）；本阶段只新增本文件。

---

## 10. 写后只读复核（2026-10-26T10:28:49Z，证明本包无副作用）

| 项 | 写后实测 | 判定 |
|---|---|---|
| Editor / PrefabStage | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`PrefabStage=null` | 仍空闲、未开 PrefabStage |
| 场景 | `SampleScene`：`dirty=False`、`roots=5`、`sceneCount=1` | 仍干净 |
| Screen | `1920x1080`、`fullScreen=False` | 未变（未切 1280、未改 Prefs） |
| 原生 TestRunner | `IsRunActive=False` | 未新增运行 |
| 字体磁盘 | **`14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**，mtime `2026-10-10T13:23:55.9520319Z` | 未变 |
| 字体内存 | 9 子资产、`dirty=0` | 仍 0 dirty |
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`，mtime `2026-10-26T10:25:28.3871461Z`、`.tmp=false` | 未变，仍是上一轮恢复后的原字节 |
| guard | `dirs=231`：`restored=230`、`protective-snapshot=1`；无 `unfinished` / `noManifest`；最新 `20261026T102526Z restored / readback=true` | 未新增 run、未改 manifest |
| 本文件（追加本节前） | `24096B / 9c3d55ae76335ca3c50ebccecb3fc91bcde8e8d5e3affb3f37afd0c79226d851`，mtime `2026-10-26T10:28:43.5210435Z` | 追加本节后哈希变化，**最终值以目录实际文件为准**，不据旧值宣称未改 |
| Unity 插件心跳 `UnityProject/RRPG1.0game/.com-unity-codely.json` | `300B`，mtime `2026-10-26T10:28:49.2740464Z`（随只读 MCP 调用自动刷新） | 插件自身心跳文件，**不在 34 项基线内、不在 `Assets/` 内**；如实披露，非本包刻意写入 |

**唯一新增产物**为本文件；整类 13 例真实回执尚未产生，等待宿主串行运行。本文件及结论须由主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。
