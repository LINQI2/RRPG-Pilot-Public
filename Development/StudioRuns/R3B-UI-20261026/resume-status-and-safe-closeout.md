# R3-B 恢复后现场核验与安全收尾（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。
- 核验时刻：**2026-10-26T02:29Z**（本机 10:29 +08:00）。
- 本包范围：**只读现场核验 + 安全收尾文档**。未运行测试、未 Refresh、未改源码 / 测试 / 资产 / 字体 / Prefab / Scene / 存档 / guard manifest / 用户 Preferences、未 `rql-mutate`、未 Shell、未启停 Editor、未 Git 写。
- 上游记录：上次执行 `0c9625c9…0fe76` 为 `finished_unverified` 且模型 `completed`；artifact `layout-resolution-capability-review.md`（`18483B / afaa8547d5eb86d1fcd1aeae0b35a952f075535f11fe2aef8bba32bfca574b8d`，mtime `2026-10-25T15:47:05Z`）主开发已读，本文件只引用、不改写。该会话记录本身由主开发转述，本执行者未读取 Studio 私有 runtime。

---

## 1. 当前现场矩阵（全部只读实测）

| 项 | 实测值（2026-10-26T02:29Z） | 判定 |
|---|---|---|
| Unity | `2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | 干净 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage()==null` | 未打开，无未保存 Prefab |
| `Screen` | **`1920x1080`、`Windowed`**；`Screen.currentResolution=2560x1600`（显示器原生） | 当前仍是 1920，**未切 1280** |
| `PlayModeWindow.GetRenderingResolution`（public） | **`1920x1080`** | 同 Screen |
| GameView 实际尺寸 | 1 个实例；`Standalone` 组 8 项；`m_SelectedSizes=[3,0,…]` → 选中项 **`[3] Full HD (1920x1080)`**；`GetSizeOfMainGameView=(1920,1080)`；组内**无 1280x720 条目** | 固定分辨率 Full HD，尺寸请求会被忽略（与 2026-10-25 失败一致） |
| Canvas | 加载场景共 **16** 个 Canvas，`pixelRect` 全部 `1920x1080`（`GrowthCanvas` `scaleFactor=1.2`，其余 `1`；`BattleCanvas/ExpeditionCanvas/ExpeditionJourneyRoot` 的 `rtRect=0x0` 属未激活 / 未布局态） | Canvas 跟随真实 Screen=1920 |
| 原生 TestRunner `IsRunActive` | `TestRunnerApi.IsRunActive()` 为 **non-public static 方法**，实测返回 **`False`** | 无活动运行 |
| pending / activeRequest 可见层 | `com.unity.test-framework@1.1.33` 的 `TestRunnerApi` **无** `pending* / activeRequest*` 成员（仅私有 `callbacksHolder`、`ScheduleJob` 字段，不能作为“待运行队列”读取）；`TestRunnerWindow` 仅见 `m_testRunnerApi` 等私有字段 | **不可见**，如实记录；不据此推断“有 / 无排队” |
| `testResults` | **0 条** | 本次 Editor 会话无持久测试结果；不能当作任何用例 PASS 证据 |
| Console | `Error` / `Exception` **0 条** | 无项目错误 |
| 当前线程 / 锁 | guard 无 `captured` / `restore-failed` / 无 manifest 的 run（见 §2） | 无未知执行残留，无需清锁 |

结论：Pilot 空闲、场景干净、无 Prefab 编辑、无活动测试运行；但 **Screen / Canvas / GameView 仍为 1920x1080**，不满足“已切 1280”，也**不能把恢复任务当成已切 1280**。

---

## 2. 保护面矩阵（字节 + SHA256 逐项实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0`（逐文件 bytes+SHA256，含字体与 MCP 插件文件） | 全一致 |
| 受保护字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档（`SaveSystem.GetSavePath()` 实测） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`；`exists=true`、**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-25T15:41:43.1615885Z`、无 `.tmp` | 与要求值一致 |
| guard 根 | **176** 个 run：`restored=175`、`protective-snapshot=1`、无 manifest=**0**、无其它状态 | 无 `unfinished` / `unknown` / `captured` / `restore-failed` |
| guard 最新 run | `20261025T154141Z`：`status=restored`、`restored=true`、`readbackVerified=true`、`lastError=null`；备份与单槽同为 `13405B / 4b2b56f6…d6aa`；manifest mtime `15:41:43.1615885Z` | 上一轮布局运行的存档已真实恢复并回读通过 |
| 测试源码 | `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：**`324392B / 3186beeac181491adb007993e77d1153d3ca18aa1cc406228e6da2e4189bda6f`**，mtime `2026-10-25T15:45:52.6002352Z` | 与主开发固定值一致 |
| PlayMode 测试 DLL | `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：`474112B`，mtime `2026-10-25T15:45:59.7095533Z`，**晚于源码** | 兜底改动已编译进程序集（新鲜） |
| 产品 Prefab | `Assets/Resources/UI/ManagementView.prefab`：**`282097B / ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11`**，mtime `2026-10-23T12:10:23Z` | 与主开发固定值一致，未改动 |
| Git（只读 `.git/HEAD` + ref） | branch **`ai/rrpg-r3b-population-pilot`**、HEAD **`054b00d3e38fe2e2e142bfdb2502de90f7991030`** | 与主开发固定状态一致；本包未 Git 写 |
| 属性计数（源码文本） | `[UnityTest]` = **12**（原 11 + 布局 1）；`[UnityTearDown]` 实际属性 = **1**（`RestoreLayoutResolutionAfterFixture`，其余为注释）；`[TearDown]` = 1 | 与 12 例发现一致 |

结论：主开发 `2026-10-26T02:26:42Z` 固定的 Git 状态在本次只读核验下全部吻合；无未知档需要恢复，**本包未恢复任何档**。

---

## 3. 验证范围与事实口径（纠正“12 例也已全过”的错误外推）

以下为**历史已核门禁与当前布局事实**的分层记录，禁止合并成“当前 12 例全绿”：

| 事实 | 数据 | 口径 |
|---|---|---|
| Compiler | `264/264 Passed、0 skipped`（2026-10-25 宿主 `R3B-Compiler-20261025`） | **历史已核门禁**，本包未重跑 |
| 完整 EditMode | `1513/1513 Passed`，记录 `f116b56e` | **历史已核门禁**，本包未重跑 |
| 完整 PlayMode | `122/122 Passed`，记录 `5eb0ad4c` | **历史已核门禁**，本包未重跑 |
| 管理 Play 11 例 | `11/11 Passed`，记录 `7f600ef9` | 绿于**布局第 12 例新增之前**；不能代表含布局的 12 例 |
| 布局第 12 例 | 唯一一次真实运行：`2026-10-25T15:41Z`，`Failed / durationMs=1533`，失败在 `ApplyTestResolution` 的 **Editor 分辨率能力**（`Screen.SetResolution(1280x720)` 实测仍 1920x1080） | **12 例从未整类绿**；该 FAIL 是能力失败，不是 1280 布局业务断言失败 |
| 新增 `[UnityTearDown]` 兜底 | 源码 `15:45:52Z` → DLL `15:45:59Z` 已编译；`testResults` 0 条、`IsRunActive=False`、guard 最新 run 为 `15:41:43Z`（早于编译） | **最新兜底仅编译、从未执行**；不得写成“恢复兜底已运行验证” |
| 1920 阶段真实完成度 | 三页签逐个程序化点击；标题/关闭/保存/命令区/搜索/过滤 top-hit；`TopHUD top=Pause`；World 射线被 `ModalBlocker` 阻断；3 张 1920 截图落盘 | **部分真实通过**；但两处可滚动内容均未溢出（Population `overflow=False contentHeight=558 viewportHeight=596`；Workplaces/Resources 为空态），**真实滚动事件到末端分支未触发**，不能写“滚动能力已全部验证” |
| 1280 阶段 | 日志仅有 1 行 `RESOLUTION-REQUEST @1280x720 … actual=1920x1080`（`15:41:43.152Z`），无任何 1280 的 `RESOLUTION / MODAL / TITLE / HIT / SCROLL / ROW / SCREENSHOT / LAYOUT-OK` | **1280 从未进入任何布局断言** |

证据：`Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/layout-input-evidence.log`（48 行，止于 1280 请求行；**无** `RESOLUTION-RESTORE*` 标记）+ 3 张 `*-1920x1080.png`。

### 3.1 对“Compiler / Edit / Play 当前 12 例也已全过”的纠正

正确表述：

1. **264 / 1513 / 122 是历史已核门禁**，不是本包或当前布局用例的结果；
2. **11 例绿对应布局第 12 例新增之前**；
3. 含布局的**最新 12 例从未整类绿**（唯一一次运行在 1280 能力断言处 Failed）；
4. 新增 fixture 级恢复兜底**最新仅编译**（DLL 新鲜），**没有运行证据**，不能在文档中记为已验证。

---

## 4. 当前 1280 状态与用户操作入口

- 当前 **未满足 1280**：`Screen=1920x1080`、`GetRenderingResolution=1920x1080`、GameView 选中 `Full HD (1920x1080)`，且尺寸组内**没有 1280x720 条目**。
- 本包**不设置 GameView、不写用户 Preferences**；恢复任务不等于已切 1280。
- 若下一包需要在真实 1280 下验证，简短的**用户操作入口**（由用户自行完成并自行恢复选择）：

> 在 Pilot 的 **Game 窗口 → 分辨率下拉** 中选择/新建 `1280x720`（当前列表无此项时点 “+” 新建 Fixed Resolution 1280x720 并选中），完成后回复“就绪”。验证结束后由用户自行把选择恢复为原条目（Full HD 1920x1080）。

- 在用户确认 1280 之前，下一包**不得**把 1920 结果外推为 1280 通过，也不得以 RectTransform / 截图 resize 冒充真实分辨率。

---

## 5. 不依赖尺寸即可继续的最小包（无需额外批准）

1. **claim 记录型专项**：整理 claim 的触发 → 记录 → 读取证据链（源码/配置/既有回执的逐项对照表），只做记录型核验与文档定稿，**不运行测试、不依赖 GameView 尺寸**。
2. **第二 FunctionSlot 真实 Gateway 限制**：如实记录当前静态配置 `BuildingConfigDatabase` 只登记 `FunctionBindings: main`，因此第二建筑 `FunctionSlotId`（如 `aux`）在真实产品 Gateway 路径没有合法注入点；这是 **Building FunctionSlot 配置面**的限制，**与 `SaveSystem` 单槽存档 / guard 无关**，不能用“唯一存档槽、guard 只保护该槽”解释 Gateway 的 FunctionSlot 限制。该限制不是测试缺陷；不扩断言、不伪造第二 FunctionSlot，也不改配置 Authority。
3. **当前 SHA 只读定稿**：以本文件 §2 的 34 项 / 字体 / 单槽 / 测试源码 / DLL / Prefab / Git HEAD 作为当前只读定稿基线，供后续包引用；不重算历史 TRX、不改基线文件。

以上三项均不依赖 1280，也不索取新批准；若涉及真实第二 slot 或历史 650 档取证，才需制作人单独授权。

> **§5.2 更正附记（2026-10-26 本包追加；错误原文保留，仅作历史）**
>
> 原 §5.2 把“第二 slot”写成 `SaveSystem` 单槽存档，并称 Gateway 限制为“唯一槽位、guard 只保护该槽”，属明显概念错误：`SaveSystem` 的单槽存档与 `BuildingFunctionBinding.FunctionSlotId` 是不同领域，前者不能解释后者。原文（已作废，逐字保留）：
>
> > 2. **第二 slot 真实 Gateway 限制**：如实记录当前 `SaveSystem` 单槽 Gateway 对“第二 slot”的硬限制（唯一槽位、guard 只保护该槽），明确该限制不是测试缺陷、不扩断言、不伪造第二槽。
>
> 正确口径见上方修订后的 §5.2 与 `activity-claim-construction-review.md` §5：第二 slot 指建筑 FunctionSlotId；当前配置只登记 `main`，第二 FunctionSlot 没有合法产品注入路径。这是配置 / Authority 缺口，**不是存档槽限制**，也不是测试缺陷。

---

## 6. 保留项（不删除、不重跑、不改写）

- **历史 650 存档缺口**：`21075B / 650c45c4d1226ab5606073cd9210545438b37482d2b5de47559a2ca6ecb06ee5`，授权范围内无字节备份；当前 `13405B / 4b2b56f6…` **不是它**。本包未恢复、未伪造。
- **NotCommitted / NotPublished**：R3-B 仍为 `Implemented / Verification PARTIAL`，未提交、未发布、未整包关闭；本文件不代表制作人批准。
- **旧通过不重跑**：264 / 1513 / 122 / 11 例等历史回执只引用，不因本包重跑。
- **旧失败不删除**：布局第 12 例的 1280 能力 FAIL、`layout-input-evidence.log` 的 1920/1280 记录、`20261025T154141Z` guard run 均保留原样。
- guard 的 `protective-snapshot:1`（历史合法终态）继续保留，不改写其 manifest。

---

## 7. 本阶段边界与实际副作用

- **唯一写入文件**：本文件 `Development/StudioRuns/R3B-UI-20261026/resume-status-and-safe-closeout.md`。
- 未运行 `tests-run` / TestRunner（`IsRunActive=False`、`testResults` 0 条为只读实测，不是运行结果）。
- 未 `AssetDatabase.Refresh`、未全局 `SaveAssets/SaveChanges`、未保存场景 / Prefab、未 `rql-mutate`、未 Shell / `Process.Start`、未启停 Editor、未 Git 写。
- 未改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 34 项基线 / 存档本体 / guard manifest；未改 GameView、未新增尺寸条目、未写用户 Preferences（仅只读反射读取 GameView 尺寸组）。
- 未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。

## 8. 移交 Studio 与核验声明

1. 本阶段按只读核验完成，当前现场：Editor 空闲、场景干净、无 Prefab 编辑、无活动测试、guard 无未知 run、34/字体/单槽/Prefab/HEAD 全一致。
2. **1280 未就绪**：如需真实 1280 验证，等用户按 §4 操作并回复“就绪”后，由下一包在**当前尺寸**做只读/布局验证；本包不预设 1280 结果。
3. §5 三项可在 1280 就绪前独立推进；§6 保留项不得删除或重跑。
4. 本文件及上述结论需主开发专业核验，不代表制作人批准。
