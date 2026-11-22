# 人口 / 施工 / 资源剩余行详情可读性补齐（2026-10-26，阶段 1/1）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包性质：**对 Person / Construction / Resource 三类行模板的详情高度、行高与文本分行做最小补齐；强化真实滚动第 13 例，新增真实非空资源业务行与真实人口行断言；新增本文件**。
- 不新增 Authority / API / DTO / 配置；不改玩法；不改 Workplace 模板（已 132 / 76 可读）；不美化 Chevron；不跑全集 / 不直接 `tests-run`（守卫拒绝，见 §5）；不 `rql-mutate`、不 PrefabStage、不全局保存。
- 本文件不预填 PASS。真实 Play 结果、截图与新滚动日志由 Studio 宿主阶段单方法运行产出；本包只登记**已真实完成的编译 / Prefab 回读 / 保护面静态核验**。

---

## 1. 先读现状与必需信息行数（按源码与 Prefab 实测）

| 行类型 | 改动前实际文本（源码） | 逻辑行数 | 改动前 Detail | 结果 |
|---|---|---|---|---|
| Person | ① `可用状态 · 职责：…（既有兼容记录）` ② `执行：… · 临时承诺：… · 饥饿：…` | 2 | `1072×42@(14,-36)`，行高 86 / Layout 86 | 42 高只容 1 行（同字体单行 ≈24.6），第 2 行被截；职责/执行/承诺/饥饿无法同时比较 |
| Construction | ① `建筑 … · 进度 … · 施工中` ② `工人 N 人：姓名（PersonId） · 现场 N 人` | 2 | `1072×42@(14,-36)`，行高 92 / Layout 92 | 第 2 行（工人姓名与人数）整行不可见 |
| Resource | ① `名额 绑定 b/c · 实际劳动 l · 建筑 … · 槽位 …` ② `点击定位到职责与场所…` | 2 | `1072×42@(14,-36)`，行高 84 / Layout 84 | 第 2 行不可见；首行过长时尾部的实例/槽位身份还会被 Ellipsis 截断 |
| Workplace | ① `绑定 b/c · 实际劳动 l · 现场 o` ② `岗位 …` ③ `建筑/槽位/功能/绑定人` | 3 | `1072×76@(14,-44)`，行高 132 / Layout 132 | **上包已修，本包未动** |

必需信息（一项未删）：
- 人口：可用状态、主要职责（含既有兼容标记）、现时执行、临时承诺、饥饿可比较（Hungry + 连续不足天数 / 已满足 / 暂无可读事实）。
- 施工：建筑实例、进度（含“施工中 / 执行暂停”）、工人人数与每个工人姓名+PersonId、现场人数。
- 资源：名额绑定 `b/c`、实际劳动人数、建筑实例、槽位、定位说明。

---

## 2. 精确改动（4 个文件）

### 2.1 `ManagementView.cs`

| 位置 | before → after |
|---|---|
| `ApplyPopulation` 的 `row.Bind(...)` | 行高 `86f → 132f`；文本拆为 3 行：① `可用状态 · 职责：…` ② `执行：…` ③ `临时承诺：… · 饥饿：…`（字段与默认文案一项未删） |
| `ApplyWorkplaces` 施工行 `row.Bind(...)` | 行高 `92f → 132f`（施工行仍调用同一 `ConstructionDetail`） |
| `ApplyResources` 资源行 `row.Bind(...)` | 行高 `84f → 132f`；文本拆为 3 行：① `名额 绑定 b/c · 实际劳动 l` ② `建筑 … · 槽位 …` ③ `点击定位到…` |
| `ConstructionDetail` | 工人行后新增换行：`\n现场 N 人`；工人姓名与人数独占第 2 行，不再与现场人数挤在同一行 |

文本未删字段：人口四项事实、施工工人姓名/人数/现场、资源名额/劳动/建筑/槽位/定位说明均保留；没有引入“有界摘要截断”（真实数据在 3 行内可容纳时优先完整显示），选中详情 `detailBody` 仍显示完整身份，未改。

### 2.2 `ManagementView.prefab`（只改 3 个模板的 9 个数值字段）

| 模板 | 根 `m_SizeDelta` / `LayoutElement.m_PreferredHeight` | `Detail` |
|---|---|---|
| PersonRowTemplate | `86 → 132` / `86 → 132` | `1072×42@(14,-36) → 1072×76@(14,-44)` |
| ConstructionRowTemplate | `92 → 132` / `92 → 132` | `1072×42@(14,-36) → 1072×76@(14,-44)` |
| ResourceRowTemplate | `84 → 132` / `84 → 132` | `1072×42@(14,-36) → 1072×76@(14,-44)` |
| WorkplaceRowTemplate | 未动 | 未动（仍 `132 / 132 / 1072×76@(14,-44)`） |

保存方式：直接精确编辑 Prefab 序列化字段（未 `LoadPrefabContents`、未 `SaveAsPrefabAsset`、未 `SaveAssets`），随后 `AssetDatabase.Refresh` 导入。Prefab 加载回读（只读）实测四模板：

```text
PersonRowTemplate       row=1100x132 pref=132 detail=1072x76@(14.00,-44.00)
WorkplaceRowTemplate    row=1100x132 pref=132 detail=1072x76@(14.00,-44.00)
ConstructionRowTemplate row=1100x132 pref=132 detail=1072x76@(14.00,-44.00)
ResourceRowTemplate     row=1100x132 pref=132 detail=1072x76@(14.00,-44.00)
```

行高算术（沿用同字体已测换算 17 号 ≈24.616/行）：标题 32 位于 `y=-8`（墨迹底 ≈39.2 < 44），Detail 位于 `-44`，高 76 可容 3 行 ≈73.85；行高 132 = 标题 32 + 间隔 4 + Detail 76 + 底部 12。标题与 Detail 不重叠；运行时 `Bind(..., 132f)` 与 Prefab 的 `LayoutElement 132` 一致。

### 2.3 `ManagementShellPlayTests.cs`（只改第 13 例及专用 helpers；其余 12 例未动）

第 13 例 `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput` 新增/强化：

1. **施工行完整 3 行断言**：`建筑/进度` 第 0 行、`工人 N 人：每个姓名（PersonId）` 第 1 行、`现场 N 人` 第 2 行；先断言 detail 矩形高度 ≥ 真实 `GetPreferredValues` 且 `isTextOverflowing=false`，再逐字符 `isVisible` + 行号 + 字符墨迹落行（行完整在视口内时还落视口）。
2. **真实非空资源业务行**：同一真实 6 完工 Farm 世界切到资源页，断言 `Resources.HasWorld=true`、`Workplaces.Count=6`、行数 = 快照行数；每条真实资源行做 `名额 绑定 b/c · 实际劳动 l` / `建筑 实例 · 槽位` 逐字符核对；真实 `IScrollHandler` + 多轮拖拽滚到末行，末行完整在视口内、标题渲染、detail 可读、点击后选中同一 `WorkplaceId` 并定位回“职责与场所”。不再使用 `Resource=NotCoveredByThisCase`。
3. **真实人口行 4 项事实**：切到人口页，对全部真实人口行断言首行 `可用状态 · 职责：…`、第 2 行 `执行：…`、第 3 行 `临时承诺：… · 饥饿：…`；同样要求 detail 矩形容纳真实行高、逐字符可见并核对墨迹落行/视口。
4. 每次测量都在 Play 断言内（`ForceMeshUpdate` / `GetPreferredValues`），未在预检做字体测量；尾部 `REAL-SCROLL-COVERAGE` 改为 `Workplace=True Construction=True Resource=True Population=True`。
5. 原有强断言全部保留：真实 overflow >40、真实 scroll 改变 normalizedPosition、多轮拖拽到末端、末行滚动后可 Raycast + PointerClick、世界 Selection / Clock Pause 不变、岗位/施工末行身份回读。

新增专用 helpers：`AssertPersonDetailFactsRendered`、`AssertResourceDetailFactsRendered`、`AssertDetailRectFits`、`FindSnapshotPerson`；扩展 `AssertConstructionDetailProgressRendered`（增加 `snapshot` 参数以还原工人姓名）；新增 `ShowResourcesPage` / `ShowPopulationPage`。

---

## 3. 改动文件与当前 SHA（只读重算）

| 文件 | bytes | SHA256 | mtime (UTC) |
|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/ManagementView.cs` | **39037** | `f8316e436845bd7266cfd3a6f0fa013cdcd3acb5705161a94d5c67e172016473` | `2026-10-26T10:21:54.3087834Z` |
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` | **282332** | `31bf3e8c111700332adfaa87d93586ea065631090416cdc2f8cf2629a30e8764` | `2026-10-26T10:21:47.9465950Z` |
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab.meta` | 178 | `b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559` | 未变（`2026-10-23T12:10:23.0493699Z`） |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **390868** | `219379e01567b36cab0af7f73aee9f133bb027abcf1566be1c69f39f51c7a45c` | `2026-10-26T10:24:36.3694583Z` |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 647680 | `b31f1b6408846a37deb2d948b840897189ef981f25c5ff9e514316f860dbe271` | `2026-10-26T10:23:04.4074813Z`（晚于 `ManagementView.cs`） |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 512512 | `40e41b98299322e32f5916df5376861e81645426ebe352784de258d85ea2ea3b` | `2026-10-26T10:24:40.5953248Z`（晚于测试源码） |

基线对照（本包开始前只读）：`ManagementView.cs 38207 / 9b593936…`、`Prefab 282326 / a05a7715…`、`Tests 371699 / 03a7c2c8…`。

禁调用自查（对本包 3 个代码 / 测试文件 grep）：`AssetDatabase.SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `ClearDirty` / `SetDirty` / `rql-mutate` / `LoadPrefabContents` / `PrefabStage` 均 **0 命中**；`GetPreferredValues` / `ForceMeshUpdate` 只出现在 Play 测试断言 helpers 中，未出现在产品代码或 Editor 预检。

---

## 4. 环境前置与保护面（只读实测）

| 项 | 本包开始前（只读） | 编译后（10:23Z） |
|---|---|---|
| Pilot | `D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets`；`2022.3.62t9` | 同 |
| Editor | `playing=False paused=False compiling=False updating=False` | `playing=False compiling=False` |
| 场景 | `SampleScene dirty=False roots=5` | `dirty=False roots=5` |
| PrefabStage | `null` | `null` |
| Screen | `1920x1080 Windowed` | 同（未切 1280、未写 Preferences） |
| 受保护字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | **完全相同** |
| 字体内存 | 9 子资产、`dirty=0`，`characterTable=632`、`lookup=644`、`atlas=8` | 9 子资产、`dirty=0` |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`，mtime `2026-10-26T08:53:30.032Z`，无 `.tmp` | 完全相同 |
| guard | `230` run = `229 restored` + `1 protective-snapshot`，其它 / 无 manifest **0**，最新 `20261026T085329Z` | 同（本包未跑测试，不产生新 run） |
| 34 项基线 | 上包已核 `34/34`；本包未重跑 | 由宿主单例的 `[BASELINE-POST]` 断言复核 |

编译：`AssetDatabase.Refresh(ForceUpdate)` 两次（第二次为断言改为只统计活动行后的重新编译），均 `isCompiling=True → False`；最终 `isCompiling=False`。最终 Console 查询 `logType == "Error"` 为 **0 条**，最近 6 条均为既存 `LiberationSans SDF` 缺字 Warning（Legacy Text，非本包引入）；本包一次只读 `script-execute` 曾在 `TestRunnerApi.IsRunActive` 上编译失败（探针代码错误），未涉及产品 / 测试汇编。两个 DLL 均已重建且晚于源码。

---

## 5. 宿主运行：被守卫拒绝，未伪造结果

DSH 直接启动测试被平台守卫拒绝，原文：

```text
Unity测试由Studio持久测试阶段执行；请提交stages.check，勿从DSH直接启动。
```

因此本包**没有**新的 `testResults`、没有新截图、没有新滚动日志。Studio 阶段需运行**仅这一个** PlayMode 单方法：

```text
RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput
```

期望由宿主真实产出（本文件不预填路径 / 字节 / PASS）：
- `[REAL-SCROLL-OVERFLOW]`：workplace content 因施工行 92→132 增高，overflow 应比上包更大；
- `[REAL-SCROLL-DETAIL]`：
  - 6 条真实岗位行（原断言保留）；
  - 1 条真实施工行 `lines=3`、`进度` 第 0 行、`工人 1 人：姓名（PersonId）` 第 1 行、`现场 N 人` 第 2 行；
  - 6 条真实资源行 `名额 绑定 b/c · 实际劳动 l` 第 0 行、`建筑 实例 · 槽位` 第 1 行；
  - 全部真实人口行 `职责` 第 0 行、`执行` 第 1 行、`临时承诺` / `饥饿` 第 2 行；
  - 每行均有 `checkedChars > 0`、`rowsInsideViewport` 与视口事实一致；
- 资源页真实 `IScrollHandler` + 多轮拖拽到末行，末行点击回读同一 `WorkplaceId`；
- 新时间戳截图（`resources-real-rows` / `population-detail-facts` 与既有 workplace 图），不覆盖历史图；
- `[SAVE-RESTORE] matches-before=True`、`[SCHEDULE-RESTORE]`、`[FONT-POST]`、`[BASELINE-POST] 34/34`。

若任一新断言失败，按真实 `message / stackTrace` 与本日志修正（例如某一真实文本超出 3 行，再按“有界摘要 + 选中详情保完整”最小调整），不得删断言 / 伪造截图。

---

## 6. 当前可见性结论（严格区分）

**已静态证实**
- 三类 Detail 矩形均为 `1072×76@(14,-44)`，行高 / Layout 均为 132，运行时 `Bind` 高度一致；标题 32 保留，算术上不重叠。
- 人口四项事实、施工工人姓名与人数、资源名额/劳动/实例/槽位文本均进入真实行 detail 字符串，无一字段被删。
- 真实资源业务行可构造：同一 6 完工 Farm 世界经 `ManagementReadAdapter` 产出 6 条 Farm 资源行；测试已按真实快照逐行断言，不再使用“资源页为空”口径。
- 编译通过，DLL 晚于源码；字体 / 单槽 / guard / 场景 / PrefabStage / Screen 未被本包改动。

**仍待宿主 Play 证实（不称已可读）**
- TMP 真实 `isVisible` / 字符墨迹是否落在行内、`GetPreferredValues` 是否 ≤76；
- 资源页与人口页真实滚动 / 末行可达 / 新截图；
- 物理鼠标 / 人工手感（本测试仍为 `EventSystem` 程序化输入）。

---

## 7. 未覆盖 / 保留（不升级为通过）

1. 物理鼠标 / 滚轮手感、镜头、键盘穿透：未测。
2. 1280×720：制作人 `01a53eb8` **取消**（取消 ≠ 通过）。
3. 第二 FunctionSlot 真实 Gateway：配置只登记 `main`，无合法注入点（与单槽存档无关）。
4. 动态批次 / 供餐细节：仍 Deferred。
5. Chevron：仍 `44×28`、字号 24，未修未测。
6. 历史 650 存档缺口：`21075B / 650c45c4…` 无字节备份，未恢复、未冒充。
7. 历史违规原文保留、不洗白：旧 `4fbf7754` 的 `ClearDirty`；详情修复包越界 `AssetDatabase.SaveAssets()`；`ManagementView.prefab` 历史插件自动回写 `282097B→282326B`（无备份、根因未定）。
8. `SaveSlotGuard` 进程级缺口：仍只在 `[TearDown]` 恢复。

---

## 8. 边界与诚信声明

- 本包实际写入 4 个文件：`ManagementView.cs`、`ManagementView.prefab`、`ManagementShellPlayTests.cs`、本文件；Prefab `.meta`、字体、图集、场景、存档、guard manifest、34 项基线内容、配置 Authority、Packages、ProjectSettings 均未改。
- Editor 操作：只读 `rql-query`、`script-execute` 只读核验 + `AssetDatabase.Refresh(ForceUpdate)` 1 次；**未** `rql-mutate`、**未** PrefabStage、**未** `LoadPrefabContents` / `SaveAsPrefabAsset`、**未** 全局保存、**未** `SetDirty` / `ClearDirty`、**未** Shell / Git 写、**未** 写 Preferences、**未** 启停 Editor、**未** 从 DSH 启动测试。
- 未安装组件、未改权限、未创建额外付费子 Agent；未访问未授权工程 / 认证目录 / 其他工作区。
- 本文件及上述结论须由**主开发专业核验**、真实 Studio 单例运行复核，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**，不越过阶段验证门禁。
