# ManagementShellPlayTests 整类 12 例 PlayMode 集成门禁 · 宿主交付前预检（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。
- 预检时刻：**2026-10-26T03:17Z–03:19Z**（本机同日 11:17–11:19 +08:00；下列每项均为本轮只读实测，非文档转述）。
- 本包范围：**只读前置 + 只新增本文件**。不改产品源码 / 测试源码 / 断言 / 配置 / 字体 / Prefab / Scene / 存档 / guard manifest；不 `AssetDatabase.Refresh`、不 `tests-run`、不重跑完整 Play / Compiler / Edit、不 `rql-mutate`、不全局 Save / SaveChanges / SaveDirtyScenes、不 Shell、不 Git 写、不写用户 Preferences、不启停 Editor、不访问未授权工程 / 认证目录 / 其他工作区。
- 上游记录（主开发转述）：1920 布局新单例 run `6e005d40-11ee-46bd-8bea-6838cbc0b520`，**1/1 Passed、0 skipped、2.1164 s、scope-restored**；最新布局代码**无尺寸写入、无 `UnityTearDown`**，原 11 例保留，当前 12 例发现且 DLL 新鲜。
- 结论前置：**12 例源码 / DLL / 发现 / 保护面 / 宿主空闲均满足整类串行运行前置**；本文件**不预填 12 例结果**，PASS/FAIL 只能由宿主整类串行运行产出。

---

## 1. 现场前置（全部只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()`（反射实测）= **False** | 无活动 / 无残留运行 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`rootCount=5` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开，无未保存 Prefab |
| Screen | **`1920x1080`、`Windowed`**、`fullScreen=false` | 真实 1920；本轮**不请求、不切换、不恢复**尺寸 |
| 最近测试结果（`testResults`） | 仅 **1 条**：`ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`，`Passed / 1359ms / message=null` | 本轮布局单例的真实回执；**不带 runId / 时间戳**，不能单独当作整类证据 |
| Console | `Error/Exception` 共 **9 条**，逐条核对**全部为 MCP `Script / Execute` snippet 自身编译错误**（`CS0030`/`CS0136`/`CS0234`，时间 03:11–03:13Z），无项目 / 测试程序集错误 | 无项目错误 |

说明：`testResults` 的 `1359ms` 是方法内耗时；主开发转述的 `2.1164 s` 是 run 级总时长，两者**口径不同**，分别标注，不互相替代。

---

## 2. 保护面（字节 + SHA256 逐项实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`） | `total=34 / same=34 / diff=0 / missing=0`；manifest `complete=true` | 全一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档 | `SaveSystem.GetSavePath()` = `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`；`exists=true`、**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T03:16:23.1851515Z`、`.tmp=false` | **`13405-4b2b56` 未变**；mtime 正是本轮回执的 SAVE-RESTORE 恢复时刻（内容仍为原始字节） |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **177** 个 run 目录：`restored=176`、`protective-snapshot=1`、其它状态 / 无 manifest **0** | **无 `unfinished` / `captured` / `restore-failed`**，无需恢复或清锁 |
| guard 最新 run | `20261026T031621Z`：`restored`，manifest mtime `2026-10-26T03:16:23.1866508Z` | **最新 restored**；时间与 1920 单例回执、单槽 mtime 相接，本包未再 Load / Save / 恢复 |

---

## 3. 源码、DLL 与 12 例发现（只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| 测试文件 | `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：**`318727B / 7a1738b4861044c3eec27b88d3496f7de8fc15029b04ef3f42a7dcae60cd4337`**，mtime `2026-10-26T03:14:58.7476077Z` | 与 1920 布局包改后固定值**逐字节一致**，本包未改 |
| 源码属性计数 | `[UnityTest]=12`、`[UnityTearDown]=0`、`Screen.SetResolution=0`、`new Vector2Int(1280=0` | **无尺寸写入、无无意义全局恢复**，1280 分支已彻底移除 |
| 测试程序集 | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：**`468992B`**，mtime `2026-10-26T03:15:07.3274317Z`，**晚于源码 8.58 s** | DLL 新鲜，已含 1920 单例 |
| 发现（整类） | `tests | where fullName contains "ManagementShellPlayTests"` 命中 **12** 条，`mode` 全部 `PlayMode`，`assembly` 全部 `BattleStage1.PlayModeTests` | 与源码计数一致 |
| 用例清单 | 原 11 例名称逐一在位 + 第 12 例 `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`（`AtTwoResolutions` 旧 FQN 已 **0** 命中） | 原 11 例保留，未改名 / 未删除 |
| 每例 SAVE-RESTORE 保护 | `[UnitySetUp]` 每例 `new SaveSlotGuard()`；`[TearDown]` 恢复日程后 `_saveGuard.Restore()`；guard 输出 `[SAVE-RESTORE] runId=… sha256=…` / `[SAVE-RESTORE-ONETIME]` 兜底 | 保护链在源码中完整（行 1150 / 1194 / 815+ 实测） |
| 两例日程恢复 | `IsolateActivityOpportunitiesForFixture` 仅两处调用：`CASE-B construction measurement fixture`（行 2163）与 `统一事件 pending 门禁`（行 4535）；恢复输出 `[SCHEDULE-RESTORE] restored=true sameInstance=True`（行 468） | 两例的 schedule 隔离 / 恢复链在位 |

---

## 4. 本轮 1920 布局真实日志（`layout-input-evidence.log` 第 49–90 行）

- 日志文件：`Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/layout-input-evidence.log`，**14560B / 90 行**，mtime `2026-10-26T03:16:23.1812031Z`。
- 第 1–48 行为历史（含 2026-10-25 的双尺寸与 1280 `RESOLUTION-REQUEST` 失败记录），**原样保留**；本轮只追加第 49–90 行。
- 本轮序列（行号 → 事实）：

| 行 | 时间（UTC） | 事实 |
|---|---|---|
| 49 | 03:16:22.7813266 | `[REAL-SCREEN]` 开始前 `Screen=1920x1080 mode=Windowed 分辨率切换=none` |
| 50 | 03:16:22.7823260 | `[LAYOUT-START] realScreen=1920x1080 … resolutionSwitch=none` |
| 51 | 03:16:22.8678173 | `[RESOLUTION] Screen=1920x1080 canvasPixelRect=1920x1080 canvasScaleFactor=1 canvasRootRect=1920x1080` |
| 52 | 03:16:22.8708090 | `[MODAL] point=(12.00, 496.00) top=ModalBlocker world-ray-blocked=True` |
| 53 | 03:16:22.8738121 | `[HIT] TopHUD point=(456.00, 1033.00) top=Pause` |
| 54 | 03:16:22.9659675 | `[SCREENSHOT] tab=population size=1920x1080 produced=True`（新时间戳文件） |
| 55–65 | 03:16:22.968–976 | Population：标题、Close(1810,946)、Save(1678,946)、4 命令按钮、搜索 Caret(240,786)、3 过滤按钮全部 **top-hit 正确** |
| 66–68 | 03:16:22.981–984 | `[SCROLL] overflow=False contentHeight=558 viewportHeight=596`；`no-scroll-needed=True`；末行 rowRect y=166 h=86 视口内直接可达 |
| 69 | 03:16:23.0744424 | `[SCREENSHOT] tab=workplaces`（新时间戳文件） |
| 70–78 | 03:16:23.075–083 | Workplaces：标题 / Close / Save / 4 命令按钮 top-hit；`overflow=False contentHeight=198 viewportHeight=650 last=<none>`、`empty-hint-visible=True` |
| 79 | 03:16:23.1700664 | `[SCREENSHOT] tab=resources`（新时间戳文件） |
| 80–88 | 03:16:23.171–179 | Resources：标题 / Close / Save / 4 命令按钮 top-hit；`overflow=False contentHeight=12 viewportHeight=650`、`empty-hint-visible=True` |
| 89 | 03:16:23.1802014 | `[LAYOUT-OK]` 三页签逐个程序化点击、控件视口内且命中正确、TopHUD 可命中、World 射线被模态阻断 |
| 90 | 03:16:23.1802014 | `[REAL-SCREEN]` 结束后 `Screen=1920x1080 mode=Windowed 分辨率切换=none` |

**1920 开始 / 结束同尺寸**：行 49 与行 90 均为 `1920x1080 Windowed`；本轮无任何 `RESOLUTION-REQUEST`（该标记只出现在第 48 行的历史 1280 尝试中）。日志止于行 90，**无 `unfinished` / 中断标记**。

---

## 5. 三张本轮真实截图（精确相对路径 + 实测像素）

像素尺寸由脚本直接解析 PNG `IHDR` 得到（不依赖日志自述），文件哈希与 mtime 逐项实测：

| 页签 | 相对路径（仓库根） | 真实像素 | 字节 | SHA256 | mtime（UTC） |
|---|---|---|---|---|---|
| Population | `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/management-population-1920x1080-20261026T031622Z.png` | **1920x1080** | 184538 | `ad34789131c18152857283c67fdf88f5bfe470bc4c9ffd47b6139d09aa54209e` | 2026-10-26T03:16:22.9514235Z |
| Workplaces | `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/management-workplaces-1920x1080-20261026T031623Z.png` | **1920x1080** | 183706 | `37f447682db27e6b3970ba9091ad94e7e4bb895b6719ec6ce11b2165767b20f3` | 2026-10-26T03:16:23.0551818Z |
| Resources | `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/management-resources-1920x1080-20261026T031623Z.png` | **1920x1080** | 176677 | `c47b61d2b8623fd5a5c09b3ce90cf8d4413d075b0926dcf221b584b49aa956ac` | 2026-10-26T03:16:23.1553475Z |

- 三张均为 **1920x1080** 真实帧（Population 已人工目视复核：顶部 HUD、标题、三页签、搜索 / 过滤、6 行人物、右侧详情、4 个命令按钮、底部导航均正常呈现）。
- **历史截图未被覆盖**：同名无时间戳的 3 张旧图 mtime 仍为 `2026-10-25T15:41:42Z`（population `184677B / 2bad7064…`、workplaces `183801B / d6bb1480…`、resources `176703B / 410b77da…`），本轮写入的是**新文件名**，符合“不覆盖历史证据”契约。

---

## 6. 滚动分支的实际覆盖（不得称“滚动通过”）

| 页签 | 实测 | 分支 |
|---|---|---|
| Population | `overflow=False`，`contentHeight=558 < viewportHeight=596`，末行视口内直接可达 | `no-scroll-needed`（未滚动） |
| Workplaces | `overflow=False`，`contentHeight=198 < 650`，`empty-hint-visible=True` | 空态分支 |
| Resources | `overflow=False`，`contentHeight=12 < 650`，`empty-hint-visible=True` | 空态分支 |

**结论口径**：真实 EventSystem 滚动到末端（`verticalNormalizedPosition→0`、内容位置变化、末行经滚动后命中）这一分支**本轮仍未触发**，属**未覆盖**。不得把“无溢出 / 空态”写成“滚动通过”。用例的 `AssertScrollReachable` 保持“未溢出只记 `no-scroll-needed`、空态只断言空提示”的不造假逻辑。

---

## 7. 1280 与历史门禁口径（禁止合并）

| 事实 | 数据 | 口径 |
|---|---|---|
| 1280 测试 | 制作人记录 `01a53eb8` **取消** | **取消 ≠ 通过**；不得由 1920 结果外推 1280 |
| 旧双尺寸第 12 例真实运行 | 2026-10-25T15:41Z `Failed / 1533ms`（1280 能力断言失败） | 历史失败事实保留 |
| 1920 新单例 | 宿主转述 run `6e005d40…` **1/1 Passed、0 skipped、2.1164 s、scope-restored**；本机 `testResults` 单条 `Passed / 1359ms`；日志与 3 张 1920 截图时间戳一致；guard `20261026T031621Z restored` | **单例真实通过**（仅此一例） |
| 原 11 例 | 绿于记录 `7f600ef9` | **历史**；本轮未重跑，不并入本次结果 |
| Compiler / 完整 Edit / 完整 Play | `264/264`、`1513/1513`、`122/122`（历史记录） | **历史已核门禁**，本包未重跑 |
| 整类 12 例 | 发现 12、DLL 新鲜、宿主空闲 | **前置满足，结果待宿主产出**；本文件不预填 |

---

## 8. 本包边界与实际副作用

- **唯一写入**：本文件 `Development/StudioRuns/R3B-UI-20261026/management-twelve-preflight.md`（新增）；不覆盖任何历史截图 / 文档。
- 唯一 Editor 操作：`rql-query` 只读查询、`script-execute` 只读 C#（读文件 / 反射 / 解析 PNG 头）、对本地 PNG 的 `read_image` 目视复核；**未** `AssetDatabase.Refresh`、**未** `rql-mutate`、**未** `tests-run`、**未**全局保存、**未**保存场景 / Prefab、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor。
- 未改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 34 项基线 / 存档本体 / guard manifest；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 不做文档全集收尾，只新增本文件。

---

## 9. 移交宿主（整类 12 例串行）

1. 以**短类名** `ManagementShellPlayTests` 运行 **PlayMode 整类 12 例、串行**；运行前宿主真实 `Screen` 必须仍是 `1920x1080`（用例自身在开头 / 结尾严格断言，**不请求、不恢复、不缩放尺寸**）。
2. 逐例核对真实回执：每例 `[SAVE-RESTORE]`（含 `runId`、`sha256`）在案；两例日程用例（`ManagementConstructionCommands_…`、`PendingEvent_BlocksAdvanceAndReturn_…`）`[SCHEDULE-RESTORE] restored=true sameInstance=True`；运行后字体仍为 `14961033B / 3c758e16…`、单槽仍为 `13405-4b2b56`、guard 无 `unfinished`。
3. 布局例失败时保留真实 `message / stackTrace` 与 `layout-input-evidence.log` 新增行 / 新截图；滚动未触发分支仍按 §6 记为**未覆盖**。
4. **不预填 12 通过**：本文件只证明“前置满足”；分类结果以宿主整类运行的真实回执为准。
5. 本文件及结论需主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。

---

## 10. 写后只读复核（2026-10-26T03:18Z，证明本包无副作用）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 受保护字体 | `14961033B / 3c758e16…` | 未变 |
| 单槽存档 | `13405B / 4b2b56f6…`，mtime `2026-10-26T03:16:23.1851515Z` | 未变（仍是本轮 run 恢复后的原始字节） |
| guard | `dirs=177`：`restored=176`、`protective-snapshot=1`，其它 0 | 仍无 `unfinished` |
| 测试源码 | `318727B / 7a1738b4…`，mtime `2026-10-26T03:14:58.7476077Z` | 未改 |
| PlayMode 测试 DLL | `468992B`，mtime `2026-10-26T03:15:07.3274317Z`（晚于源码） | 仍新鲜 |
| 场景 | `SampleScene: isDirty=false`、`rootCount=5` | 仍干净 |
| Editor | `isPlaying=false`、`isCompiling=false`、`isUpdating=false`；`Screen=1920x1080` | 仍空闲、仍 1920 |
| `testResults` | 仍 **1** 条（布局单例 `Passed / 1359ms`） | 未新增运行、未产生整类结果 |

**唯一新增产物**：本文件（新增，未修改任何既有文件、未覆盖任何历史截图）。整类 12 例的真实回执尚未产生，等待宿主串行运行。
