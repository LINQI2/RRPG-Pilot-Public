# ManagementProjectionTests 整类 18 例 EditMode 集成门禁 · 宿主交付前预检（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。
- 预检时刻：**2026-10-26T03:11Z**（本机 11:11 +08:00）。
- 本包范围：**只读预检 + 仅新增本文件**。不改产品源码 / 测试源码 / 断言 / 配置 / 字体 / Prefab / Scene / 存档 / guard manifest；不 `AssetDatabase.Refresh`、不 `tests-run`、不重跑完整 Edit / Play / Compiler、不 `rql-mutate`、不全局 Save / SaveChanges / SaveDirtyScenes、不 Shell、不 Git 写、不写用户 Preferences、不启停 Editor、不访问未授权工程 / 认证目录 / 其他工作区。
- 上游记录（主开发转述，非本执行层可读的 Studio 私有 runtime）：单例 run `41a9596f-9d17-4180-81c9-27451c4dbd98`，claim 施工单例 **1/1 Passed、0 Failed、0 skipped**，`scope-restored`。本文件把“转述”与“本机只读实测”分开标注。
- 结论前置：**18 例源码 / DLL / 发现 / 保护面均满足整类串行运行前置**；本文件**不预填整类结果**，18 例 PASS/FAIL 只能由宿主整类串行运行产出。

---

## 1. 现场前置（全部只读实测，2026-10-26T03:11Z）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`dirty=False`、`roots=5` | 干净，无待保存场景 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开，无未保存 Prefab |
| 原生 TestRunner 活动运行 | `TestRunnerApi.IsRunActive()`（non-public static，反射实测）= **False** | 无活动运行 |
| 未知投递（待运行队列） | `com.unity.test-framework@1.1.33` 的 `TestRunnerApi` 可见成员中**无** `pending* / activeRequest*`（过滤结果为空串）；私有 `callbacksHolder / ScheduleJob` 不能作为队列读取 | **不可见**，如实记录；不据此推断“有 / 无排队”，仅以 `IsRunActive=False` 作为“当前没有在跑”的判据 |
| 最近测试结果 | `testResults` 仅 **1 条**：`ManagementConstructionClaim_…UntilLegalAvailability`，`Passed / 89ms / message=null` | 与本包单例回执相符；该记录**不带 runId / 时间戳**，不能单独当作整类或历史门禁证据 |
| Canvas / Screen（只记录，不作门禁） | `Screen=1920x1080, Windowed`；`currentResolution=2560x1600` | **未切 1280**；1280 已由制作人取消（见 §5），本包不据此派任何尺寸任务 |

---

## 2. 保护面（字节 + SHA256 逐项实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`） | `total=34 / same=34 / diff=0 / missing=0` | 全一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档（`Application.persistentDataPath/rrpg_save_p0.json`） | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，mtime `2026-10-25T15:41:43.1615885Z`，`.tmp` 不存在 | **`13405-4b2b56` 与要求一致**，本包未 Load / Save / 恢复 |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **176** 个 run 目录：`restored=175`、`protective-snapshot=1`、其它状态 **0**、无 manifest **0** | **无 unfinished / unknown / captured / restore-failed**，无需恢复或清锁 |
| guard 最新 run | `20261025T154141Z`：`restored`，manifest mtime `2026-10-25T15:41:43.1615885Z` | 上一轮布局 run 的存档已恢复；与单槽内容一致 |
| 产品程序集 | `Assembly-CSharp.dll` mtime `2026-10-24T11:47:08.6030118Z` | 本包未触发产品重编 |
| Console | `Error` / `Exception` **0 条** | 无项目错误 |

---

## 3. EditMode 源码与 18 例发现（只读）

| 项 | 实测值 | 判定 |
|---|---|---|
| 测试文件 | `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`：**`54062B / f48f8f7d0cb9bfe299c3c8cbc1a6814540bd178495e632f907eedf21e9f16e0b`**，mtime `2026-10-26T03:08:46.6725349Z` | 与主开发固定值一致（本包未改） |
| 源码属性计数 | `[Test]=18`、`[UnityTest]=0`、`[TearDown]=1` | 18 个普通 EditMode 用例 |
| 测试程序集 | `Library/ScriptAssemblies/VillageStage1.Tests.dll`：`153088B`，mtime `2026-10-26T03:08:50.4871113Z`，**晚于源码** | DLL 新鲜（已包含第 18 例） |
| 发现（`tests` 查询） | `ManagementProjectionTests` 命中 **18** 条，`mode` 全部为 `EditMode`；第 18 例 FQN 唯一 | 与源码计数一致 |
| 用例清单 | 17 例既有 + 第 18 例 `ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability` | 原 17 例断言未改、未删除（上游文档记录，本包只读确认文件未变） |

说明：测试源码**不在** 34 项 workspace 基线内，故第 3 节以文件哈希 + DLL mtime + 反射/发现结果作为“未改且新鲜”的证据；本包未 Refresh、未重编。

---

## 4. 单例回执、四段日志与保护断言（不预填整类）

### 4.1 可核对的直接证据

- 宿主回执（主开发转述）：run `41a9596f-9d17-4180-81c9-27451c4dbd98`，**1/1 Passed、0 skip**，`scope-restored`。
- 本机只读佐证：`testResults` 中唯一一条即为该单例 FQN，`Passed / 89ms / message=null`；`IsRunActive=False`，说明单例已收尾、无残留运行。
- 限制：`testResults` 不保存 runId / 时间戳 / 日志正文；`Development/` 下也没有该 run 的日志落盘，**四段日志的运行时正文本执行层无法从磁盘读取**，只能核对源码中的四段标记与保护断言。

### 4.2 源码中的四段日志标记（`[CLAIM-CONSTRUCTION]`，均在断言之后输出）

| 段 | 源码位置 | 覆盖事实 |
|---|---|---|
| before work boundary | 第 697 行 | 通勤边界前一分钟：claim 仍为同一 `claimId`、实例 `Committed`、`Success` 回执、无 Arrival、`Progress=0`、`LaborRemainder=0`、Primary 仍为原岗位 |
| legal end | 第 725 行 | 真实通勤边界：实例 `Interrupted`、`QueryClaim=null`、Arrival 目的为 `construction:<id>`、`ReadyMinute=边界+CommuteMinutes`、参与者集与施工名单保留 |
| at work start | 第 737 行 | `WorkStart`：`HasArrived=True` 但 `LaborRemainder=0`、`Progress=0`（**480 到场零劳动**） |
| first labor minute | 第 757 行 | 工作时段第一分钟：`项目劳动`、`LaborRemainder=(firstLaborMinute-workStart)×TestProgressPerParticipantPerDay`（即 **481 劳动 1**）、Primary / 参与者集 / 原岗位证据保持 |

### 4.3 保护性断言（与失败回归点对应）

- 失败历史（保留）：`24c81ccf…` 曾在 `Advance(481)` 断言 claim 身份失败，原因是夹具把断言放在**合法 Interrupted 释放之后**；现断言已按真实 `WorkSchedule` 派生边界重写（`commuteBoundary = MorningStart - CommuteMinutes`），不写死 `460/481/482/496/497`。
- 单例通过即证明：**464 claim 保持 → 465 合法 Interrupted 释放 → 480 到场零劳动 → 481 劳动 1** 这条时间线在当前源码 / DLL 下成立，且未削弱 Primary Assignment、参与者集、原岗位名额与 0 证据等业务断言。
- **不预填整类**：单例绿只说明第 18 例自身通过；其余 17 例本次未运行，不得写成“18 例已过”。

---

## 5. Play 布局第 12 例与历史门禁口径（禁止合并）

| 事实 | 数据 | 口径 |
|---|---|---|
| Compiler | `264/264 Passed、0 skipped`（2026-10-25 宿主 `R3B-Compiler-20261025`） | **历史已核门禁**，本包未重跑 |
| 完整 EditMode | `1513/1513 Passed`（记录 `f116b56e`） | **历史已核门禁**，本包未重跑 |
| 完整 PlayMode | `122/122 Passed`（记录 `5eb0ad4c`） | **历史已核门禁**，本包未重跑 |
| 管理 Play 11 例 | `11/11 Passed`（记录 `7f600ef9`） | 绿于布局第 12 例新增之前 |
| Play 布局第 12 例 | 源码 `324392B / 3186beeac181491adb007993e77d1153d3ca18aa1cc406228e6da2e4189bda6f`，mtime `2026-10-25T15:45:52Z`；`BattleStage1.PlayModeTests.dll` `474112B`、mtime `15:45:59Z`（晚于源码）；`[UnityTest]=12`、`[UnityTearDown]=1` | **最新改动仅编译、从未执行**；源码仍含 1280 用例，但**不认作 12 例已过** |
| 1280 测试授权 | 制作人记录 `01a53eb8` **已取消**；上游报告明确 1280 **不再是阻塞** | 本包**不请求人工切 1280、不派双尺寸、不等待“就绪”**；不为 1280 改布局代码 |

结论：本包**不动布局代码、不重跑 Play**；历史 264 / 1513 / 122 与“新增 18 例范围”严格区分，新增范围当前只到“18 例可发现 + 单例 1/1 通过”。

---

## 6. 本包边界与实际副作用

- **唯一写入**：本文件 `Development/StudioRuns/R3B-UI-20261026/management-eighteen-preflight.md`（新增）。
- 唯一 Editor 操作：`script-execute` 只读 C#（读文件 / 反射 / 目录统计）与 `rql-query` 只读查询；**未** `AssetDatabase.Refresh`、**未** `rql-mutate`、**未** `tests-run`、**未** 全局保存、**未**保存场景 / Prefab、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor。
- 未改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 34 项基线 / 存档本体 / guard manifest / 配置 / Authority。
- 未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。

---

## 7. 移交宿主与核验声明

1. 请宿主按**短类名** `ManagementProjectionTests` 运行 **EditMode 整类 18 例、串行**；不预填结果，以真实回执为准。
2. 若失败：保留真实回执、用例名、message、stackTrace 与 `[CLAIM-CONSTRUCTION]` 四段日志，不以夹具放宽或删断言掩盖；本报告不回写为 PASS。
3. §1–§3 预检项全部满足；`IsRunActive=False`、场景 `dirty=False`、guard 无 unfinished、单槽 `13405-4b2b56` 未变。
4. 本文件及结论需主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。

---

## 8. 写后只读复核（2026-10-26T03:11Z 之后，证明本包无副作用）

| 项 | 写后实测 | 判定 |
|---|---|---|
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0` | 仍全一致 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa` | 未变 |
| guard | `dirs=176 / restored=175 / protective=1 / other=0 / noManifest=0` | 仍无 unfinished |
| 场景 | `SampleScene: dirty=False` | 仍干净 |
| 原生 TestRunner | `IsRunActive=False`；`testResults` 仍 **1** 条（单例 Passed） | 未新增运行、未产生整类结果 |
| Edit 测试源码 | `54062B / f48f8f7d…f16e0b` | 未改 |

**唯一新增产物**：本文件（新增，未修改任何既有文件）。整类 18 例的真实回执尚未产生，等待宿主串行运行。
