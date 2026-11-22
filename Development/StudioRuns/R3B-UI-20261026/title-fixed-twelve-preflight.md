# 姓名视觉修复后 · ManagementShellPlayTests 整类 12 例 PlayMode 集成预检（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。仅本目录与已连接的 Studio Unity MCP 工程，未访问未授权工程 / 认证目录 / 其他工作区。
- 预检时刻：脚本实测 `utcNow = 2026-10-26T07:21:08.1363707Z`（本机同日 15:21 +08:00）。下表每项均为**本轮只读实测**，与上游转述分开标注。
- 本包范围：**只读核验 + 仅新增本文件**。不重做 Label32 修复、不改产品/测试源码、不改 Prefab/Scene/字体/图集/尺寸/Prefs/存档/guard manifest；不 `AssetDatabase.Refresh`、不 `PrefabStage`、不 `tests-run`（模型不启动）、不 `rql-mutate`、不 Shell、不 Git 写、不全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、不 `SetDirty`、不 Save 任何资产或场景、不启停 Editor。
- 上游记录（主开发转述，本执行层不可读 Studio 私有 runtime）：姓名修复后 1920 单例 run `b1170647-5bc5-4718-b62c-4e14053661ff`，**1/1 Passed、0 skipped、2.4333919 s、scope-restored**；主开发已亲自查看 `management-population-1920x1080-20261026T071915Z.png`，6 行姓名职业真实可见。
- 结论前置：**字体磁盘 = 基线且内存/子资产 0 dirty、工程空闲、34 项 / 单槽 `13405-4b2b56` / guard 无 unfinished、12 例发现且 DLL 新鲜**，可交给宿主整类串行运行；本文件**不预填** 12 例结果。

---

## 1. 身份与工程空闲（只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `Assets/Scenes/SampleScene.scene`，`dirty=False`、`roots=5` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开（本包不打开，避免自动序列化回写） |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()`（反射实测）= **False** | 无活动 / 无残留运行 |
| Screen | `1920x1080`、`Windowed`、`fullScreen=False`；`currentResolution=2560x1600`（桌面分辨率，非渲染尺寸） | 满足宿主 1920 前置；**未切 1280、未改 Prefs 尺寸** |
| Console | 本轮查询 `Error`/`Exception` = **0 条** | 无项目 / 测试程序集错误 |
| `testResults` | **1 条**：`ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`，`Passed / 1559ms / message=null` | 姓名修复后单例本机回执；**不带 runId / 时间戳**，只能作单例佐证，不能当整类证据 |

---

## 2. 受保护字体：磁盘 / 内存 / 子资产（安全门禁）

### 2.1 磁盘（重算字节 + SHA256，未写文件）

| 项 | 实测值 | 要求值 | 判定 |
|---|---|---|---|
| 路径 | `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | 同 | ✅ |
| bytes | **14961033** | 14961033 | ✅ |
| SHA256 | **`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 同 | ✅ |
| 文件 mtime | `2026-10-10T13:23:55.9520319Z` | — | 远早于本日全部会话，**今日未被写** |
| `.meta` mtime | `2026-10-10T12:10:57.0892496Z` | — | 同样未动 |

### 2.2 已加载对象与全部子资产（`AssetDatabase.LoadAllAssetsAtPath` + `EditorUtility.IsDirty`，只读）

`subassetCount = 9`，**`dirtySubassets = 0`**（未 Save / 未 SetDirty / 未重导入 / 未覆盖内存对象）：

| # | 类型 | name | `EditorUtility.IsDirty` |
|---|---|---|---|
| 0 | `TMP_FontAsset` | `RRPG_SourceHanSansCN_Regular_TMP` | **False** |
| 1 | `Material` | `RRPG_SourceHanSansCN_Regular_TMP Material` | **False** |
| 2–8 | `Texture2D` ×7 atlas | `… Atlas`、`… Atlas 1` … `… Atlas 6`，均 `1024x1024 / Alpha8` | 全部 **False** |

| 字体对象成员 | 实测 | 判定 |
|---|---|---|
| `atlasTextures.Length` | **8**（索引 0–6 为上述 7 张真实 atlas，**索引 7 为 `null`**） | 如实保留该空闲槽；null 槽不 dirty，本包未写入 |
| `material` | `RRPG_SourceHanSansCN_Regular_TMP Material`，`IsDirty=False` | 无待保存编辑 |
| `atlasPopulationMode` | `Dynamic` | 与历史一致 |
| `characterLookupTable.Count` | **644** | 与历史 `644 字` 一致 |
| `sourceFontFile` | `SourceHanSansCN-Regular` | 未变 |

**判定：字体磁盘字节 = 基线，内存字体对象与全部 9 个子资产 0 dirty。**未出现 `Font already has pending edits` 的实体证据；若存在 pending 编辑，本包只会如实发现，不做清理性保存。

---

## 3. 恢复证据与保护面（SAVE / GUARD / BASELINE post）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，`complete=true`） | 正则逐项重算：`total=34 / same=34 / diff=0 / missing=0` | **34/34 全一致** |
| 单槽存档（`SaveSystem.GetSavePath()` 反射实测） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot\rrpg_save_p0.json`；**`13405B`**、SHA256 **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T07:19:16.1281078Z`、`.tmp` **不存在** | 内容仍为 `13405-4b2b56`；mtime 正是本轮单例 run 的 guard 恢复时刻 |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **190** 个 run 目录：`restored=189`、`protective-snapshot=1`、**无 manifest 0**、其它状态 **0** | **无 unfinished / captured / restore-failed / unknown**，无需恢复或清锁 |
| guard 最新 run | `20261026T071914Z`：`restored`，manifest mtime `2026-10-26T07:19:16.1286940Z` | 与单槽 mtime、日志 07:19 段相接：上一轮单例已真实恢复并回读 |

**口径**：单槽内容哈希 `13405-4b2b56` 与 `files.*` 基线一致；guard 的 `restored` 表示 run 结束已恢复原字节（manifest 内 `readbackVerified=true`），本包未再 Load / Save / 恢复。

---

## 4. 本轮 `ROW-TITLE-RENDER` / `ROW-TITLE-VISIBLE` 真实日志

- 日志：`Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/layout-input-evidence.log`，**`31487B / 181 行`**，SHA256 `da999659c417dd1130301c144d1cb753e035f37b71ac763301271fbba957dfd5`，mtime `2026-10-26T07:19:16.1229306Z`。历史 1–152 行原样保留，本轮只追加 153–181 行。
- 标记计数：`ROW-TITLE-RENDER = 6 行`、`ROW-TITLE-VISIBLE = 1 行`；日志末尾为 `[LAYOUT-OK]`（第 180 行）与 `[REAL-SCREEN] … Screen=1920x1080 … 分辨率切换=none`（第 181 行），**无 interrupted / unfinished 标记**。

逐行实测（第 153–158 行，均为 `PersonRowTemplate(Clone)`，`rect=1072×32`、`alpha=1`、`crAlpha=1`、`cull=False`、`overlapsDetail=False`、`rowFullyInsideViewport=True`）：

| 行 | text | chars | visible | rendered | preferredH | titleInk |
|---|---|---|---|---|---|---|
| 153 | 林恩 · profession_test_c | 22 | 20 | 247.2239×30.408 | 30.41 | (80.00, 672.80, 247.22, 30.41) |
| 154 | 苏晚 · profession_test_a | 22 | 20 | 248.3359×30.408 | 30.41 | (80.00, 580.80, 248.34, 30.41) |
| 155 | 邱霜 · profession_test_b | 22 | 20 | 249.4916×30.408 | 30.41 | (80.00, 488.80, 249.49, 30.41) |
| 156 | 青鹿 · 英雄 | 7 | 5 | 114.4063×30.408 | 30.41 | (80.00, 396.80, 114.41, 30.41) |
| 157 | 韩昭 · profession_test_e | 22 | 20 | 248.1463×30.408 | 30.41 | (80.00, 304.80, 248.15, 30.41) |
| 158 | 魏安 · profession_test_d | 22 | 20 | 249.5317×30.408 | 30.41 | (80.00, 212.80, 249.53, 30.41) |

第 159 行：`[ROW-TITLE-VISIBLE] … rows=6 checkedRows=6（TMP 可见字符/渲染尺寸/行高/alpha/cull/与 detail 不重叠/视口裁切，非仅字符串）`。

即：**6 行均有真实可见字符、渲染尺寸 ≥ 真实单行高、与 Detail 不重叠、完整在视口内**——姓名/职业不可见的原缺陷在**人口页**已被渲染数据与截图双重证明修复。

---

## 5. 新 3 张 PNG 实测（像素由 PNG IHDR 解析，非日志自述）

| 页签 | 相对路径（仓库根） | 像素 | 字节 | SHA256 | mtime（UTC） |
|---|---|---|---|---|---|
| Population | `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/management-population-1920x1080-20261026T071915Z.png` | **1920x1080** | 214846 | `bf4425d8a613ef9e7c0686901e469d3b782ee01e100a68144263a6f976bbf03a` | 2026-10-26T07:19:15.8543899Z |
| Workplaces | `…/management-workplaces-1920x1080-20261026T071915Z.png` | **1920x1080** | 183359 | `73a6a7ab8bdf03c1cac79d4b1b5d51bf9addf52bd945724850d915b63919dadc` | 2026-10-26T07:19:15.9915892Z |
| Resources | `…/management-resources-1920x1080-20261026T071916Z.png` | **1920x1080** | 176366 | `08a57d5ac3849bafa37e2c135ccd21a6168f75f233e4ed9ccf9dad7c19d48e9a` | 2026-10-26T07:19:16.0974198Z |

- 本执行层另行 `read_image` 复核 Population 新图：顶部 HUD、标题、三页签、搜索/过滤、**6 行“姓名 · 职业”**、右侧所选人物详情、4 个命令按钮、底部导航均真实呈现；与主开发目视结论一致（不是转述替代）。
- 历史图未被覆盖：无时间戳旧图仍为 `2026-10-25T15:41:42Z`（population `184677B / 2bad7064…`、workplaces `183801B / d6bb1480…`、resources `176703B / 410b77da…`）；`03:16:22Z` 缺陷现场 `184538B / ad3478…`、`03:19:12Z` 前一轮 `184460B / 5cc9e564…` 也原样在位。

---

## 6. 源码、DLL 与 12 例发现（只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| 测试源码 | `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：**`328163B / a9ae921dbe12cf3d6cd02d83b63d2816390ea9906b92395862035e3042d64499`**，mtime `2026-10-26T03:26:42.3867672Z` | 与姓名修复包记录值逐字节一致，本包未改 |
| 属性计数 | `[UnityTest]=12`、`[UnityTearDown]=0`、`Screen.SetResolution=0`、`new Vector2Int(1280=0` | 无尺寸写入、无无意义全局恢复、1280 分支仍为 0 |
| 测试程序集 | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：**`474624B / 5bbdfdc83b5a35c9c9fcfd82d2e5523804de1aba68412bf14ab9c930ba86792e`**，mtime `2026-10-26T03:26:51.1664894Z`（**晚于源码 8.78 s**） | DLL 新鲜，已含最新源码 |
| 发现（整类） | `tests | where fullName contains "ManagementShellPlayTests"` = **12 条**，`mode` 全 `PlayMode`、`assembly` 全 `BattleStage1.PlayModeTests` | 与源码计数一致 |
| 用例清单 | 12 个 FQN：CloseButton…、ExpeditionReturn…、ExplicitSaveContinue…、FirstNavigation…、ManagementConstructionCommands…、**ManagementLayout_At1920…**、ManagementModal…、ManagementWorkplaceCommands…、Management_ProductEntry…、PendingEvent…、SurfaceEquivalentSimulation…、WorkplaceRemovalDuringLabor… | 原 11 例名称逐一在位 + 第 12 例布局；`AtTwoResolutions` 旧名 0 命中 |

**关于“原 11 例未改”的边界**：本包只有**间接证据**——测试源码哈希 `a9ae921d…` 等于姓名修复包记录的“只改第 12 例断言之后”的固定值，且 12 个方法名逐一保留；本会话**没有**逐例 YAML/AST 级三方 diff，也**没有**重跑这 11 例。故只能说“自姓名修复包交付以来源码字节未变、名称未变、原 11 例未被删除/改名”，**不能说“11 例已重新验证通过”**。

---

## 7. Prefab：4 个行模板 Label=32 与历史回写边界

| 项 | 实测值 |
|---|---|
| `Assets/Resources/UI/ManagementView.prefab` | **`282326B / e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba`**，mtime `2026-10-26T03:26:15.6250542Z` |
| `m_SizeDelta: {x: 1072, y: 32}` | **4 处**（YAML 第 283 / 497 / 5475 / 8487 行，分别对应第 260 / 474 / 5452 / 8464 行的行模板 `Label`） |
| `m_SizeDelta: {x: 1072, y: 28}` | **0 处** |

**必须保留的历史异常（不可洗白）**：该 Prefab 在只读打开 PrefabStage 后被编辑器/插件**非有意自动序列化回写**，大小由历史 `282097B / ade4c7e1…` 变为 `282326B / 5cf61738…`（实测字节差 **+229B**；上游/前包口述的“232B”与之口径不一致，此处按实际字节数列出，不合并）。该旧文件无备份，**无法逐字节还原**，因此：

- 能证明的只有：当前文件中 **4 个 `1072×32` 与 0 个 `1072×28`**，位置/字号/行高与修复记录一致；
- **不能**宣称“本轮只改了 4 个数字”或“除 Label 高度外序列化形态未变”——缺少回写前文件的逐字节对照，YAML 缩进/字段顺序/浮点表示是否一并变化**无法证明**；
- 本包**未**再打开 PrefabStage、**未** Save Prefab、**未** `rql-mutate`，故该文件自 `03:26:15Z` 起未再被本包触碰。

---

## 8. 未知边界（禁止夸大清单）

1. **仅人口页有真实行标题渲染证据**（6 行 `PersonRowTemplate(Clone)`）。Workplaces / Resources 在当前世界为**空态**（`contentHeight=198/12 < viewport 650`、`empty-hint-visible=True`），Construction 行同样无业务行；这三个模板的 Label=32 只有**结构证据**，没有真实业务行渲染 / 截图 / 断言证据。
2. **滚动溢出分支仍未触发**：Population `overflow=False`、`contentHeight=558 < viewportHeight=596`、`no-scroll-needed=True`（日志 150–151 行）。“真实 EventSystem 滚动到末端”**未覆盖**，不得写成“滚动通过”。
3. **未验证物理鼠标**：日志第 152 行明确“程序化 Raycast，非物理鼠标”；点击手感 / 真实指针路径未测。
4. **Chevron 未修未测**：仍 `44×28`、字号 24，小于其真实行高；本包未纳入断言，不宣称可见。
5. **Detail 仍单行 + Ellipsis**（`1072×42`、字号 17），属既有表现，本包未改。
6. **单例 ≠ 整类**：`testResults` 仅 1 条布局单例 `Passed / 1559ms`；上游 run `b1170647…` 的 runId / 2.4333919 s / scope-restored 属**主开发转述**，本执行层无法从磁盘读取该 runtime 记录。12 例结果**本文件不预填**。
7. **1280 仍是“取消”而非“通过”**：制作人记录 `01a53eb8` 已取消；本包未请求、未切尺寸、未改 Prefs，也**不得**由 1920 结果外推 1280。

---

## 9. 移交宿主：整类 12 例串行 PlayMode

前置已满足：`IsRunActive=False`、`testResults` 仅 1 条单例、场景 `dirty=False`、`PrefabStage=null`、字体 `14961033B / 3c758e16…c404854` 且 0 dirty、34/34、单槽 `13405-4b2b56`、guard `190 = 189 restored + 1 protective`（无 unfinished）、12 例发现、DLL 新鲜、当前 `Screen=1920x1080`。

1. 以**短类名** `ManagementShellPlayTests` 运行 **PlayMode 整类 12 例、串行**；运行前真实 `Screen` 必须仍为 `1920x1080`（用例自身在首尾严格断言，**不请求、不恢复、不缩放尺寸**）。
2. 逐例核对：每例 `[SAVE-RESTORE]`（含 `runId`、`sha256`）在案；两例日程用例（`ManagementConstructionCommands_…`、`PendingEvent_BlocksAdvanceAndReturn_…`）需 `[SCHEDULE-RESTORE] restored=true sameInstance=True`；布局例需新增 `[ROW-TITLE-RENDER]`（逐行）与 `[ROW-TITLE-VISIBLE]`、以及**新时间戳** `management-*-1920x1080-<UTC>.png`（不覆盖 `071915Z/071916Z` 及全部历史图）。
3. 运行后复核：字体仍 `14961033B / 3c758e16…` 且内存 0 dirty、单槽仍 `13405-4b2b56`、guard 无 `unfinished`。
4. 任一例失败：保留真实 `message / stackTrace`、用例名与新增日志行，**不以“已修复”代替回执**，不写成本报告 PASS。
5. 本文件及结论须由主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。

---

## 10. 本包边界与实际副作用

- **唯一写入**：本文件（新增）；不覆盖任何历史截图 / 文档 / 日志。
- 唯一 Editor 操作：`rql-query` 只读查询、`script-execute` **只读** C#（读文件 / 重算 SHA256 / 解析 PNG IHDR / 反射读 `IsRunActive` 与 `SaveSystem.GetSavePath` / `EditorUtility.IsDirty` 只读判定 / 目录与 manifest 统计）、对本地 PNG 的 `read_image` 目视复核。
- **未** `AssetDatabase.Refresh`、**未**打开 PrefabStage、**未** `tests-run`、**未** `rql-mutate`、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor。
- **未** Save / SetDirty / SaveAssets / SaveChanges / SaveDirtyScenes / 清 dirty / 重导入字体 / 覆盖内存对象；**未**保存场景或 Prefab；**未**改尺寸与 Prefs。
- **未**改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 图集 / 34 项基线 / 存档本体 / guard manifest / 配置 / Authority；**未**安装组件、**未**改权限、**未**创建额外付费子 Agent；**未**访问未授权工程 / 认证目录 / 其他工作区。

---

## 11. 写后只读复核（2026-10-26T07:22:01Z，证明本包无副作用）

| 项 | 写后实测 | 判定 |
|---|---|---|
| Editor | `isPlaying=False`、`isCompiling=False`、`isUpdating=False`；`PrefabStage=null` | 仍空闲、未开 PrefabStage |
| 场景 | `SampleScene: dirty=False`、`roots=5` | 仍干净 |
| Screen | `1920x1080 / Windowed` | 未变（未切 1280、未改 Prefs） |
| 原生 TestRunner | `IsRunActive=False`；`testResults` 仍 1 条布局单例 | 未新增运行、未产生整类结果 |
| 字体磁盘 | **`14961033B / 3c758e16…c404854`** | 未变 |
| 字体内存 | `subassets=9`、`dirty=0` | 仍 0 dirty |
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 单槽存档 | **`13405B / 4b2b56f6…d6aa`**，mtime `2026-10-26T07:19:16.1281078Z`，`.tmp` 无 | 未变，仍是上一轮 run 恢复后的原字节 |
| guard | `dirs=190 / restored=189 / protective-snapshot=1 / noManifest=0` | 仍无 unfinished |
| 新人口 PNG | `214846B / bf4425d8…f03a` | 未覆盖、未变 |

**本文件自身**：写入后（追加本节之前）`16900B / 227f8aae9b1a0fbb53a1ff3832afad724d085c18a79c4fd73229106e68601906`，mtime `2026-10-26T07:21:55.0672499Z`；追加本节后哈希变化，最终值以目录实际文件为准，不据旧值宣称未改。本文件及结论须由主开发专业核验，**不代表制作人批准**；整类 12 例真实回执尚未产生，等待宿主串行运行。
