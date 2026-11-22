# R3-B 布局分辨率能力核查与测试恢复兜底（阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 实测时刻：2026-10-25T15:46Z 前后（本机 23:46 +08:00）。
- 本阶段范围：**只核查已跑证据 + 仅测试文件的最窄恢复加固 + 安全 Refresh 编译 + 只读 GameView 能力调查**。
- 不做：不运行测试、不启动播放、不改 GameView、不新增尺寸条目、不反射写私有状态、不点 1280、不改产品 / Prefab / Scene / 字体 / 原 11 例 / guard / 存档、不重做 UI、不扩大断言、不文档全集。
- 结论口径：**1280 阶段从未真实运行**；上一轮失败是 Editor 分辨率能力失败，不是 1280 布局业务断言失败。本包**不宣称布局用例 PASS**，只保证“分辨率恢复”从依赖用例内 `finally` 改为 fixture 级独立兜底，并如实给出 GameView 能力的 BLOCKED 判定与人工入口。

---

## 1. 现场与保护面（全部只读实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | 干净 |
| Prefab 编辑模式 | 未打开 | 无未保存 Prefab |
| 当前屏幕 | `Screen=1920x1080`、`Windowed`；`Screen.currentResolution=2560x1600`（显示器原生） | 1920x1080 来自 GameView 固定尺寸，不是显示器原生 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，mtime `2026-10-25T15:41:43.1615885Z`，无 `.tmp` | 与要求值一致 |
| guard 根 | **176** 个 run：`restored=175`、`protective-snapshot=1`、`captured=0`、`restore-failed=0`、其它=0、无 manifest=0 | 无未完成 / 未知 / 待恢复 |
| guard 最新 run | `20261025T154141Z`：`status=restored`、`readbackVerified=true`、`restored=true`、`lastError=null`，备份与单槽同为 `13405B / 4b2b56f6…d6aa` | 上一轮布局运行的存档已真实恢复 |
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0`（逐文件 bytes+SHA256） | 全一致 |
| 受保护字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 产品源码 / Prefab | `ManagementView.cs` `37518B / 908451ea…`；`ManagementView.prefab` `282097B / ade4c7e1…` | 本包未改动 |

结论：Pilot 空闲、场景干净、34 项与字体一致、单槽唯一、guard 最新 run 已 restored，可安全做只读核查与最小测试加固。

---

## 2. 已跑证据：1920 阶段实际完成了什么

数据源：`Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/layout-input-evidence.log`（`7644B / 5a577da8…`，mtime `15:41:43Z`）+ 3 张 1920 截图 + 宿主已核 `testResults`（`Failed`，`durationMs=1533`）。

### 2.1 运行骨架

| 时间（UTC） | 日志 | 说明 |
|---|---|---|
| 15:41:42.472 | `LAYOUT-START originalScreen=1920x1080 mode=Windowed` | 用例启动，原始分辨率记录 |
| 15:41:42.509 | `RESOLUTION-REQUEST @1920x1080 requested=1920x1080 before=1920x1080 actual=1920x1080` | 1920 请求即当前值 |
| 15:41:42.586 | `RESOLUTION @1920x1080 Screen=1920x1080 canvasPixelRect=1920x1080 scaleFactor=1 canvasRootRect=1920x1080` | Canvas 跟随真实 Screen |
| 15:41:42.896 | `LAYOUT-OK @1920x1080：三页签已逐个程序化点击…` | 1920 阶段全套布局断言通过 |
| 15:41:43.152 | `RESOLUTION-REQUEST @1280x720 requested=1280x720 before=1920x1080 actual=1920x1080 mode=Windowed` | **1280 请求未生效，实测仍 1920x1080** |
| 15:41:43.161 | guard run `20261025T154141Z` 完成 `restored` | 存档恢复（标准 TearDown 执行了） |

`testResults` 唯一条目：

- `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`
- `status=Failed`、`durationMs=1533`
- 消息：`管理布局 @1280x720：Editor 能力失败——Screen.SetResolution(1280x720) 未产生真实 Screen.width，实测 1920x1080；不以 RectTransform 缩放冒充真实分辨率 / Expected: 1280 / But was: 1920`

即：**1920 阶段已真实走完；1280 阶段只发出一次请求，未进入任何 1280 布局断言。**

### 2.2 1920 各页签逐项完成情况（按日志，不按“走到 1280”统称通过）

| 页签 | 已完成的真实证据 | 未覆盖 / 限制 |
|---|---|---|
| Population | `TITLE`（纯展示标题，`raycastTarget=False`，top=ModalPanel，可见/视口内/文本/尺寸/遮挡已核）；`HIT`：Close、Save、Assign/Remove/AddConstruction/RemoveConstruction、搜索 Caret、Filter_All/InVillage/NotInVillage 均 top-hit 正确；6 行人口；滚动 `overflow=False contentHeight=558 viewportHeight=596`，末行完整落在视口内且可点击；截图 `management-population-1920x1080.png`（`184677B / 2bad7064…`） | **真实溢出滚动分支未触发**（内容未超视口），只验证了“无需滚动 + 末行可达” |
| Workplaces | `TITLE`；Close/Save/4 个命令按钮 top-hit 正确；滚动 `overflow=False contentHeight=198 viewportHeight=650 last=<none>`，`empty-hint-visible=True`；截图 `management-workplaces-1920x1080.png`（`183801B / d6bb1480…`） | 本世界无已完成岗位/施工项，岗位滚动为**空提示**，无行、无滚动 |
| Resources | `TITLE`；Close/Save/4 个命令按钮 top-hit 正确；滚动 `overflow=False contentHeight=12 viewportHeight=650 last=<none>`，`empty-hint-visible=True`；截图 `management-resources-1920x1080.png`（`176703B / 410b77da…`） | 无 Farm/Kitchen 可导航实例，资源滚动为**空提示**，无行、无滚动 |

另：`MODAL`（World 射线被 `ModalBlocker` 阻断）、`HIT TopHUD top=Pause` 在 1920 已记录；三张截图均确认三页签、标题、搜索/过滤、关闭/保存、命令区、右侧人物面板与底部导航在 1920 视口内完整可见。

**准确表述**：1920 阶段“三页签逐个点击 + 标题/交互控件 top-hit + Canvas 跟随真实 Screen + 空态/无需滚动如实记录”完成；**真实 scroll 事件滚到末端的分支在 1920 也未被执行**（因为两处可滚动内容都不溢出），不能写成“滚动能力已全部验证”。

### 2.3 1280 从未运行

- `layout-input-evidence/` 下只有 3 张 `*-1920x1080.png`，**没有任何 1280 截图**。
- 日志中 1280 只有 1 行 `RESOLUTION-REQUEST`，没有 `RESOLUTION` / `MODAL` / `TITLE` / `HIT` / `SCROLL` / `ROW` / `SCREENSHOT` / `LAYOUT-OK`。
- 失败消息来自 `ApplyTestResolution` 的分辨率能力断言，不是 1280 布局业务断言。

---

## 3. 恢复状态与上一轮恢复缺口

### 3.1 实际恢复结果

| 项 | 实测 |
|---|---|
| 当前 `Screen` | `1920x1080 / Windowed`，等于 `LAYOUT-START` 记录的原值 |
| 日志中的恢复标记 | **没有** `RESOLUTION-RESTORE`、**没有** `RESOLUTION-RESTORE-VERIFY`（日志止于 1280 的 `RESOLUTION-REQUEST`） |
| 存档 | guard `20261025T154141Z` 已 `restored` 且回读校验通过；单槽 hash 与要求值一致 |
| 场景 | `isDirty=false` |

### 3.2 缺口定性

1280 能力断言在嵌套 `IEnumerator`（`ApplyTestResolution`）内抛出后，用例协程被中止，方法体 `try/finally` 的 `finally` **未执行**（日志无恢复标记可证）。本次没有残留分辨率，是因为“请求值恰好等于原值 1920”，**不是**因为恢复逻辑真实执行过。这正是本包要修的点：**不能把“恰好没变”当成恢复成功。**

---

## 4. 本包最窄加固（仅 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`）

### 4.1 改动内容

1. 新增 fixture 静态字段：`_layoutResolutionCaptured`、`_layoutOriginalWidth`、`_layoutOriginalHeight`、`_layoutOriginalMode`。
2. 新增 `CaptureLayoutResolutionForFixture(string what)`：在布局用例**任何分辨率改动之前同步**读取真实 `Screen.width/height/fullScreenMode` 写入上述字段，并写 `LAYOUT-CAPTURE` 证据（`fixtureFallback=armed`）。
3. 布局用例开头改为调用该捕获，并以夹具字段作为方法内原值来源；方法内 `try/finally` 与 `RESOLUTION-RESTORE` / `RESOLUTION-RESTORE-VERIFY` 逻辑保持不变（happy path 仍先恢复）。
4. 新增 `[UnityTearDown] public IEnumerator RestoreLayoutResolutionAfterFixture()`：
   - 未捕获（其它 11 例）时 `yield break`，不改变原有生命周期语义；
   - 若进入时 `Screen` 已等于原尺寸/模式，**仍按实测值核对**，不以“没变过”代替核对；
   - 否则发起 `Screen.SetResolution(原尺寸, 原模式)`，最多等 60 帧 + 3 帧并 `Canvas.ForceUpdateCanvases()`；
   - 最终以 `Screen.width/height/fullScreenMode` 与捕获值逐项比对，写 `RESOLUTION-RESTORE-FIXTURE` / `RESOLUTION-RESTORE-VERIFY-FIXTURE`；不一致写 `RESOLUTION-RESTORE-FAILED` 并 `Assert` 抛错，**不吞错**；
   - 只有核对通过后才清 `_layoutResolutionCaptured`；失败保留捕获值，后续用例的同一兜底仍会再次尝试并继续暴露失败。
5. 未改：标准 `[TearDown]` 的 `SaveSlotGuard.Restore → ResetForTests → RestoreActivityScheduleAfterFixture` 链路、原 11 例的方法体与断言、布局断言强度、产品代码、Prefab、Scene、字体。

### 4.2 恢复尝试为什么不会被存档链短路（依据本地 UTF 1.1.33 源码，只读）

- `Library/PackageCache/com.unity.test-framework@1.1.33/UnityEngine.TestRunner/NUnitExtensions/Runner/TestCommandBuilder.cs`：`new EnumerableSetUpTearDownCommand(command)` 包在 `new SetUpTearDownCommand(command)` 之外。
- `EnumerableSetUpTearDownCommand` 只处理 `[UnityTearDown]`，`SetUpTearDownCommand` 处理标准 `[TearDown]`。
- `BeforeAfterTestCommandBase.ExecuteEnumerable` 在内部测试命令/标准 TearDown 记录异常后，仍继续执行外层 AfterActions。

因此 `[UnityTearDown]` 在 `SaveSlotGuard.Restore`、`ResetForTests`、活动日程恢复**之后**执行；上述任一步抛错只记录结果，不会跳过本恢复尝试。上一轮 guard 已真实 restored（15:41:43）也旁证标准 TearDown 在该失败下确实执行过。

### 4.3 编译结果（安全 Refresh）

- 先确认场景 `isDirty=false`、未播放，再调用 `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)`；未全局 `SaveAssets/SaveChanges`、未保存场景/Prefab、未 `rql-mutate`。
- 编译结束：`scriptCompilationFailed=False`、`isCompiling=False`、`isUpdating=False`。
- 测试源码：`324392B / 3186beeac181491adb007993e77d1153d3ca18aa1cc406228e6da2e4189bda6f`，mtime `2026-10-25T15:45:52.6002352Z`。
- `BattleStage1.PlayModeTests.dll`：`474112B`，mtime `2026-10-25T15:45:59.7095533Z`（晚于源码）。
- 反射核对：`[UnityTest]` = **12**（原 11 + 布局 1，名称逐条核对）；布局方法唯一且带 `UnityTestAttribute`；`[UnityTearDown]` = **1**（`RestoreLayoutResolutionAfterFixture`，`IEnumerator`）；新增字段/helper 均存在。
- Console 无项目源码 `error CS`；最近错误仅为本阶段只读调查片段的自身编译失败（seq 58040–58047，发生于 15:43–15:44，早于本次 Refresh），非测试源码错误。
- **未运行任何测试**：`testResults` 仍是上一轮宿主运行的 1 条 `Failed/1533ms`；guard 仍为 176 个 run（无新增），单槽 mtime 仍为 15:41:43。

---

## 5. 当前 Editor 合法 GameView 固定分辨率能力（只读调查）

### 5.1 现状：为什么 `Screen.SetResolution(1280,720)` 不生效

只读反射实测（未改任何值）：

- GameView 窗口存在（`UnityEditor.GameView`，docked、maximized=false，`m_SelectedSizes=[3,0,0,…]`，`selectedSizeIndex=3`）。
- 当前 size group = `Standalone`，共 8 项：`[0] Free Aspect`、`[1] 16:9 Aspect`、`[2] 16:10 Aspect`、`[3] Full HD (1920x1080)`、`[4] WXGA (1366x768)`、`[5] QHD (2560x1440)`、`[6] 4K UHD (3840x2160)`、`[7] Codely 960x540`（自定义）。
- `GameView.GetSizeOfMainGameView()=(1920,1080)`、`targetSize=(1920,1080)`、`targetRenderSize=(1920,1080)`。

即：当前 GameView 选中 **固定分辨率条目 Full HD (1920x1080)**；Editor 在该模式下忽略 `Screen.SetResolution` 的尺寸请求，故 1280 请求后 `Screen` 仍为 1920x1080。这与实测失败一致。

### 5.2 可用 API 边界

| 能力 | 可见性 | 结论 |
|---|---|---|
| `UnityEditor.PlayModeWindow.GetRenderingResolution(out uint width, out uint height)` | **public**（`UnityEditor.CoreModule`） | 可只读取当前渲染分辨率（实测来自 `targetSize`） |
| `UnityEditor.PlayModeWindow.SetCustomRenderingResolution(uint width, uint height, string baseName)` | **public** | 唯一可编程改 GameView 固定分辨率的公开入口 |
| `PlayModeWindow.GetPlayModeFocused/SetPlayModeFocused/GetViewType/SetViewType` | **public** | 影响焦点/视图类型，不解决尺寸状态捕获 |
| `UnityEditor.GameView.selectedSizeIndex / currentGameViewSize / SetPlayModeViewSize / SetCustomResolution` | internal 类型成员（`GameView` 非 public） | 非公开 API；使用需反射，不符合本任务边界 |
| `UnityEditor.GameViewSizes / GameViewSizeGroup / GameViewSize` | internal | 非公开 API |

对 `SetCustomRenderingResolution` 的只读 IL 调查（`GameView.SetCustomResolution`）：

1. `GetOrCreateWindow()`（可能创建/聚焦 GameView）；
2. 在当前 group 内找第一个 `displayText.StartsWith(baseName)` 的条目，**直接改写其 width/height**（若 baseName 命中的是内建条目，会把内建条目改掉）；
3. 未命中则 `new GameViewSize(...)` + `AddCustomSize`（**新增尺寸条目**）；
4. 无论命中与否都调用 `GameViewSizes.SaveToHDD()`（**持久化写入**）；
5. 设置 `selectedSizeIndex`、更新 ZoomArea、`SceneView.RepaintAll()`。

`GameViewSizes` 是 `ScriptableSingleton<GameViewSizes>`，标注 `[FilePath("GameViewSizes.asset", Location.PreferencesFolder)]`；实测解析路径为
`1PUBLIC_USER/AppData/Roaming/Tuanjie/Editor-5.x/Preferences/GameViewSizes.asset`（**用户级全局编辑器 Preferences，位于授权工作区之外；本包未访问该文件**）。

### 5.3 判定：BLOCKED（当前约束下无合规自动能力）

要满足“真实 1280 + 不改持久状态 + 可捕获并恢复用户 GameView 状态”，需要同时具备：

1. 读取用户当前选中的尺寸条目/索引/模式（用于精确恢复）；
2. 设置目标固定分辨率而不新增/改写任何持久尺寸条目；
3. 不写任何持久文件、不改变窗口焦点/布局。

公开 API 只能做到“读当前分辨率 + 设置自定义分辨率”，且设置必然 `SaveToHDD()`，并可能新增/改写条目；**没有公开 API 读取选中的条目身份/索引**，无法恢复“用户原来选的是 Full HD 还是 Free Aspect/自定义条目”。`SetCustomRenderingResolution(原分辨率, "Full HD")` 只能把分辨率与选中项近似回 Full HD：会持久写全局 Preferences、可能匹配错条目，且无法覆盖焦点/布局变化，因此**不能用“首次 SetResolution 成功假设”代替真实 restore 核对**。故本阶段结论为 **BLOCKED**，不实施 GameView 变更。

### 5.4 最小可选方案（本包不执行，需制作人/宿主授权）

- **人工入口（推荐，改动最小）**：由制作人在 Game View 工具栏手动选择/新建固定 `1280x720` 尺寸（这是用户主动操作，会写入其全局 Preferences），再运行**不调用 `Screen.SetResolution` 的 1280 变体**（该变体属于后续获授权的测试改动，本包不做），断言“当前真实 Screen 就是 1280x720 + Canvas 跟随 + 布局可达”。风险：仍需新增测试变体，且 GameView 选择状态由人工负责恢复。
- **宿主/播放器入口**：由 Studio 在独立播放器或 CI 以 `-screen-width 1280 -screen-height 720 -screen-fullscreen 0` 运行该用例（真实 Screen 由播放器窗口决定，不写编辑器 Preferences）；需要构建/启动外部进程，超出本阶段授权。
- **程序化入口（当前不建议）**：`PlayModeWindow.SetCustomRenderingResolution`。技术上能改 GameView 固定分辨率，但会 `SaveToHDD` 写全局 Preferences、新增/改写尺寸条目，且无法精确恢复用户原选中项；若制作人明确接受“只恢复分辨率、接受全局 Preferences 写入与条目新增”，还需另行授权并补充恢复证据，本包不采用。

在授权与方案确定前，布局用例的 1280 阶段应保持**真实能力失败**，不以 RectTransform/截图 resize 冒充，也不把 1920 结果外推为 1280 通过。

---

## 6. 本阶段边界与实际副作用

- 写入文件仅 2 个：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅新增夹具字段、捕获 helper、`[UnityTearDown]` 兜底，并让布局用例改用夹具捕获值）；
  2. 本文件。
- 未运行 `tests-run`、未启动/忙等 TestRunner、未进入播放；`testResults` 未新增。
- 未 `rql-mutate`；未 Shell / `Process.Start`；未 Git 写；未全局 `SaveAssets/SaveChanges`；未保存场景/Prefab；未改分辨率；未改 GameView；未新增尺寸条目；未反射写任何私有状态；未 Save/Load；未写 manifest。
- 未修改产品源码/Prefab/Scene/字体/34 项基线/存档本体/guard 历史 run；单槽仍为 `13405B / 4b2b56f6…`，guard 仍为 176 = 175 restored + 1 protective-snapshot。
- 未安装组件、未改权限、未创建付费子 Agent、未访问授权目录外工程/认证目录（GameViewSizes 只读取了类型标注与解析路径，未打开该用户级文件）。

## 7. 移交 Studio 的下一最小步骤

1. 宿主**仅**串行运行布局单方法（PlayMode），验证新增 `RESOLUTION-RESTORE-FIXTURE` / `RESOLUTION-RESTORE-VERIFY-FIXTURE` 在失败路径真实落盘并核对通过；不重跑原 11 例。
2. 预期仍会在 1280 能力断言处失败（除非制作人先选定 5.4 的人工/播放器方案）；该失败应保留，不得改成 PASS，也不得放宽为 RectTransform 冒充。
3. 若走 5.4 人工/播放器方案，需先由制作人授权测试变体与 GameView 状态责任，再补 1280 真实截图与日志；本包不预填 1280 结果。
