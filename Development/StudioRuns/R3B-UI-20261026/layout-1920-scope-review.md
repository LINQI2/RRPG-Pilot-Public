# R3-B 管理布局专项：取消 1280 后 1920 单分辨率收敛（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。
- 制作人范围决定：记录 `01a53eb8` 明确“不需要 1280 测试了”。本包据此**取消 1280 分支**，把布局长期回归收敛为**宿主真实 1920x1080 单分辨率**；不新增产品改动、不跑全集、不重跑原 11 例。
- 本包写入：仅 `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅现布局第 12 例与布局专用 helper）与本文件。其余文件、产品源码、Prefab、Scene、字体、34 项基线、存档、guard manifest、Preferences 均未改。
- 口径：**1280 是取消，不是通过**；最新 1920 单例**仅编译、从未执行**，结果待宿主真实运行；本文件不预填 PASS，也不代表制作人批准。

---

## 1. 写前现场预检（全部只读实测，2026-10-26T03:13Z 前后）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity / Editor | `2022.3.62t9`；`isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`、`rootCount=5` | 干净，无待保存场景 |
| Screen | **`1920x1080`、`Windowed`**（`Screen.width/height/fullScreenMode` 实测） | 真实 1920，不需要也不允许再切尺寸 |
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0`（逐文件 bytes + SHA256） | 全一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档（`SaveSystem.GetSavePath()`） | `rrpg_save_p0.json`；`exists=true`、`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、`.tmp=false` | 与要求一致；本包未 Load / Save / 恢复 |
| guard 根 | **176** 个 run：`restored=175`、`protective-snapshot=1`、其它状态 **0**、无 manifest **0** | 无 `unfinished` / `captured` / `restore-failed`，无需恢复或清锁 |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()`（反射实测）= **False** | 无活动 / 无残留运行 |
| 现布局测试源码（改前） | `324392B / 3186beeac181491adb007993e77d1153d3ca18aa1cc406228e6da2e4189bda6f`，mtime `2026-10-25T15:45:52.6002352Z` | 与主开发固定值一致 |
| 现 PlayMode 测试 DLL（改前） | `474112B`，mtime `2026-10-25T15:45:59.7095533Z` | 改前源码已编译（含旧双尺寸 + fixture 兜底） |

结论：宿主空闲、场景干净、无未知 guard 状态、真实 Screen 即 1920x1080 —— 满足“无未知才写”的前置，才开始改写。

---

## 2. 精确 diff（仅测试文件内布局第 12 例及专用 helper）

改后源码：**`318727B / 7a1738b4861044c3eec27b88d3496f7de8fc15029b04ef3f42a7dcae60cd4337`**，mtime `2026-10-26T03:14:58.7476077Z`（净减 5665B）。

### 2.1 删除（仅为已取消的尺寸切换添加，且全文件确认只被布局使用）

| 删除项 | 原用途 | 仅布局使用证据 |
|---|---|---|
| fixture 字段 `_layoutResolutionCaptured` / `_layoutOriginalWidth` / `_layoutOriginalHeight` / `_layoutOriginalMode`（含注释） | 记录原尺寸 / 模式，供 fixture 级兜底恢复 | `grep` 全文件仅出现在 `CaptureLayoutResolutionForFixture` / `RestoreLayoutResolutionAfterFixture` / 布局用例本体 |
| `[UnityTearDown] RestoreLayoutResolutionAfterFixture()`（含注释约 57 行） | 嵌套 IEnumerator 失败时独立恢复 `Screen.SetResolution` | 同上；该属性全文件唯一 |
| `CaptureLayoutResolutionForFixture(string)`（含注释约 14 行） | 改动分辨率前捕获原值 | 同上 |
| `ApplyTestResolution(int,int,string)`（含注释约 23 行） | `Screen.SetResolution` 请求并断言真实尺寸 | 同上 |
| 布局用例内 `targets = { 1920x1080, 1280x720 }` + `foreach` 双尺寸循环 | 双分辨率遍历 | 仅布局用例 |
| 布局用例 `finally { Screen.SetResolution(original...) }` 与 `RESOLUTION-RESTORE-VERIFY` / 末尾恢复断言 | 用例后恢复原分辨率 | 仅布局用例 |

### 2.2 修改

| 项 | 改前 | 改后 |
|---|---|---|
| 用例方法名 | `ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable` | `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable` |
| 尺寸来源 | `Screen.SetResolution` 请求 1920、再请求 1280，逐尺寸跑一遍 | **只读宿主当前真实 `Screen`**，开头严格断言 `1920x1080`，不设置任何尺寸 |
| Canvas 校验 helper | `AssertCanvasMatchesResolution(management, width, height, what)`（带 requested 参数） | `AssertCanvasMatchesScreen(management, what)`：只与真实 `Screen.width/height` 比对 |
| 截图 helper | 同名 PNG 先 `File.Delete` 再截图（会覆盖历史证据） | 同名已存在时**不删除、不覆盖**，改用 `management-<tab>-1920x1080-<UTC>.png` 新文件名；工具不可用仍如实记录 `SCREENSHOT-UNAVAILABLE` |
| 布局日志 | `originalScreen=... fixtureFallback=armed`、`RESOLUTION-REQUEST/RESTORE` | `realScreen=1920x1080 ... resolutionSwitch=none（1280 已由制作人 01a53eb8 取消）`、`REAL-SCREEN` |

### 2.3 新增

- `AssertRealScreenAt1920x1080(string what)`：`Assert.AreEqual(1920, Screen.width)`、`Assert.AreEqual(1080, Screen.height)`，失败文案明确“不使用 SetResolution / GameView / RectTransform 缩放改尺寸”。
- 用例开头与结束各调用一次（结束是“全程未改尺寸”的契约，不是恢复）。

### 2.4 完全未改（保护证明）

- 原 **11 例方法体**、`[TearDown]`、`SaveSlotGuard` 全类、活动日程 `IsolateActivityOpportunitiesForFixture` / `RestoreActivityScheduleAfterFixture` 恢复链、`AssertTabLayout` / `AssertScrollReachable` / `AssertRowReachable` / `AssertDisplayTitle` / `AssertControlHit` 等布局断言、`LayoutEvidence` 路径（`R3B-UI-20261025/layout-input-evidence`）均未改。
- 文件名、命名空间、类名不变；不新增产品 API、不改 Prefab / Scene。

---

## 3. 12 例发现与编译后核对（只读）

| 项 | 改后实测 | 判定 |
|---|---|---|
| `AssetDatabase.Refresh(ForceUpdate)` | 经 `script-execute` 调用一次，返回后插件随域重载短暂断开并自行恢复 | 安全刷新，无全局 `SaveAssets` / `SaveChanges` |
| 编译结果 | PlayMode 测试 DLL **`468992B`**，mtime `2026-10-26T03:15:07.3274317Z`，**晚于源码** `03:14:58Z` | 已编译且新鲜 |
| Console | 无项目编译 `Error` / `Exception`；仅本会话两条 **MCP snippet 自身**的历史编译错误（`CS0030` / `CS0136`，非测试程序集） | 无项目错误 |
| 发现（新 FQN） | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`，**唯一 1 条** | 新方法可发现 |
| 发现（旧 FQN） | 查询 `AtTwoResolutions` 命中 **0** 条 | 原双尺寸方法已不再发现 |
| 整类数量 | `ManagementShellPlayTests` 命中 **12** 条，原 11 例名称逐一保留 | 仍 12 例 |
| 源码计数 | `[UnityTest]=12`、`[UnityTearDown]=0`、`Screen.SetResolution=0`、`new Vector2Int(1280=0` | 无尺寸切换残留，无无意义全局恢复 |
| 场景 | `SampleScene: isDirty=false`、`rootCount=5` | 刷新后仍干净 |
| Screen | `1920x1080 Windowed` | 刷新未改尺寸 |
| 34 项 / 字体 / 单槽 / guard | `34/34 same`、字体 `3c758e16…`、存档 `13405-4b2b56`、guard `176` 无其它状态 | 写后复核全部未变 |

---

## 4. 用例保留的真实布局强断言（新 1920 单例）

- 三页签：`tabButtons` 绑定 3 个，逐个真实 `PointerClick` 切页并断言 `ActiveTab`；页签控件位于真实视口内。
- 标题展示契约：`AssertDisplayTitle`（可见、视口内、文本非空、字号/矩形可排版、非 Button、未被面板外图层或可点击控件压住）。
- 按钮输入 top-hit：`closeButton`、`saveButton`、`nameSearch`、3 个状态过滤按钮、4 个命令区按钮（`assign/remove/addConstruction/removeConstruction`）逐个 `AssertControlHit`（真实 Raycast top hit + `IPointerClickHandler`）；关闭 / 保存 / 页签 / 人口行用真实 `RealPointerClick`。
- TopHUD：`TopHUD/Pause` 真实 top-hit + 点击处理器；`ModalBlocker` 阻断 World 射线且不产生点击处理器。
- 真实模态：`SurfaceModalActive`、`Surface == Management`。
- 截图：每页签真实帧末截图（`CaptureLayoutShot`），工具不可用时如实记 `SCREENSHOT-UNAVAILABLE`，不伪造文件。
- Canvas：`pixelRect` 必须跟随真实 `Screen`（1920x1080），`scaleFactor > 0`；**不以 RectTransform 缩放冒充**。
- 全部断言失败文案带 `@1920x1080`（真实 `width x height`）上下文。

---

## 5. 历史保留与“取消≠通过”口径

| 事实 | 数据 | 口径 |
|---|---|---|
| 1280 | 制作人 `01a53eb8` 取消；本包删除请求 / 循环 / 恢复代码 | **取消，非通过**；不得把 1920 结果外推为 1280 通过 |
| 旧布局第 12 例真实运行 | `2026-10-25T15:41Z`，`Failed / 1533ms`，失败在 `ApplyTestResolution` 的 1280 能力断言（实测仍 1920x1080） | 保留原失败事实；本次不是“修复 1280”，而是移除该分支 |
| 旧 fixture 兜底 | 源码 `15:45:52Z` → DLL `15:45:59Z` 仅编译，从未执行 | 本包删除它，不记作“已验证” |
| 1920 部分真实通过 | 三页签点击、标题/按钮 top-hit、TopHUD、模态阻断、3 张 1920 截图 | **部分真实通过**；不代表新单例已通过 |
| 新 1920 单例 | 改后源码 / DLL 新鲜；`IsRunActive=false`；未 `tests-run` | **仅编译、从未执行**，待宿主运行 |
| 历史证据 | `layout-input-evidence.log` 48 行（止于 1280 `RESOLUTION-REQUEST`，无 `RESOLUTION-RESTORE*`）+ 3 张 `*-1920x1080.png` + guard `20261025T154141Z`（restored） | **全部保留，不删除**；截图 helper 已改为同名存在时不覆盖 |

## 6. 滚动分支的实际覆盖（不得记作通过）

上一轮真实 1920 记录（`layout-input-evidence.log`，与 `resume-status-and-safe-closeout.md` §3 一致）：

- Population：`overflow=False`，`contentHeight=558`、`viewportHeight=596` → 走 `no-scroll-needed` 分支，末行直接可达。
- Workplaces / Resources：真实空态（只有空提示）→ 走空态分支并断言空提示可见。
- **真实 EventSystem scroll 事件直到末端（`verticalNormalizedPosition→0`、内容位置变化、末行可命中）这一分支从未触发**。

因此：

1. 新用例保留原有 `AssertScrollReachable` 的不造假逻辑：未溢出时只记录 `no-scroll-needed=True`，空态只断言空提示，**不伪造滚动数据**；
2. 本文件**不**把未触发的滚动分支记为通过；滚动到末端的真实能力仍属**未覆盖**，待真实内容溢出时由宿主运行体现；
3. 若宿主运行时仍无溢出，结果应如实写成“布局可达、滚动分支未触发（未覆盖）”。

---

## 7. 本包边界与实际副作用

- 唯一写入：测试文件 `ManagementShellPlayTests.cs`（布局第 12 例 + 专用 helper）与本文件（新增）。
- Editor 操作：`script-execute`（只读预检 / `AssetDatabase.Refresh(ForceUpdate)` / 只读复核）、`rql-query` 只读查询；**未** `rql-mutate`、**未** `tests-run`、**未**全局 `SaveAssets/SaveChanges`、**未**保存场景 / Prefab、**未**写 Preferences、**未**启停 Editor、**未** Shell / `Process.Start`、**未** Git 写（未执行 `git` 命令）。
- 未改产品源码 / 断言语义 / Prefab / Scene / 字体 / 34 项基线 / 存档本体 / guard manifest；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。
- 未批量文档收尾：本包只新增本文件，不改写其它 StudioRuns 文档。

## 8. 移交宿主

1. 仅运行**新单方法**（PlayMode）：
   `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`
2. 运行前确认宿主真实 `Screen=1920x1080`；用例自身会在开头/结尾严格断言，不请求、不恢复、不缩放尺寸。
3. **不重跑**原 11 例、不跑全集；不把本包“仅编译”写成通过。
4. 若失败：保留真实回执、用例名、message、stackTrace 与 `layout-input-evidence.log` 新增行 / 截图；滚动未触发分支仍按 §6 记为未覆盖。
5. 本文件及结论需主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。
