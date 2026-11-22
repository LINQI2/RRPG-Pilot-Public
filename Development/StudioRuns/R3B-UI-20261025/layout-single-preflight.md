# R3-B 双分辨率布局单方法门禁前置核查（只读，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，`2022.3.62t9`）。
- 实测时刻：**2026-10-25T15:36:39Z**（本机 23:36:39 +08:00）。
- 本阶段范围：**只读前置核对**。不重写实现、不 `AssetDatabase.Refresh`、不运行旧 11 例、不运行任何测试、不改源码/manifest/分辨率。
- 本文件唯一写入：`Development/StudioRuns/R3B-UI-20261025/layout-single-preflight.md`。
- 结论口径：前置条件**全部成立**，交宿主 Studio 阶段**串行真实执行单方法**；**本文件不含任何 PASS 结论**，结果不预填。

---

## 1. 身份与宿主空闲

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity 版本 | `2022.3.62t9` | 与授权身份一致 |
| `EditorUtility.scriptCompilationFailed` | `False` | 无编译失败 |
| `EditorApplication.isCompiling` | `False` | 编译结束 |
| `EditorApplication.isUpdating` | `False` | 无资产导入 |
| `Application.isPlaying` / `EditorApplication.isPlaying` | `False` / `False` | 未进入播放 |
| `isPaused` | `False` | 未暂停 |
| `selection` | `[]` | 无选中干扰 |
| 场景 | 仅 `SampleScene`（`Assets/Scenes/SampleScene.scene`），**`isDirty=false`**，`rootCount=5` | 干净，无未保存场景 |
| Prefab 编辑模式 | 未打开 | 无未保存 Prefab |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()` = `False`；`EditModeLauncher.IsRunning` = `False`；`PlaymodeLauncher.IsRunning` = `False`（只读反射，字段/方法的实际类型已按名核对） | 空闲，无活动/挂起运行 |
| `RunData` | 本次仅定位到类型 `UnityEditor.TestTools.TestRunner.CommandLineTest.RunData`，静态实例名未公开，**未取值**；空闲判定不以它为依据 | 限制已标注，不猜测 |
| `testResults` | **0 条**（旁证） | 无历史/正在投递的结果 |
| Editor 装配件 | `UnityEditor.TestRunner` 存在且可只读反射 | 原生检测通道有效 |

## 2. 目标 FQN 唯一且已编译

| 项 | 实测值 |
|---|---|
| 目标 FQN | `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable` |
| 源码位置 | `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` **第 1816 行**，`[UnityTest]`，方法 `public IEnumerator` |
| 名称在源码中的出现次数 | **1 次**（无重载/同名重复） |
| RQL `tests` 按 `fullName contains "ManagementLayout_AtTwoResolutions"` | **1 条**，`mode=PlayMode`（唯一） |
| 程序集 `BattleStage1.PlayModeTests` 内类型 | 存在 |
| 方法在 DLL 中 | 存在，且带 `UnityTestAttribute` |
| 未知测试 | **无**（未出现无法归属的测试项） |

## 3. 源码字节与编译新鲜度

| 项 | 实测值 | 与固定值比对 |
|---|---|---|
| `ManagementShellPlayTests.cs` bytes | **316260** | 一致 |
| SHA256 | **`26b43bf89e6452c1aff829cb6b4b25a55ff61ca9095da2d3205331c3ceea599f`** | 一致 |
| mtimeUtc | `2026-10-25T15:30:57.0257006Z` | 未再变动 |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | `468992B`，mtime `2026-10-25T15:31:41.4291708Z` | DLL 晚于源码 44 秒 |

本阶段**未触发**任何重新编译、未调用 `AssetDatabase.Refresh`、未保存资产或场景；上述字节与 mtime 与 `layout-interruption-audit.md` 记录值逐项一致。

## 4. 保护面（guard / 单槽 / 34 项 / 字体）

| 项 | 实测值 | 判定 |
|---|---|---|
| guard 根 `Development/StudioRuns/R3B-SaveProtection-20261024` | **174** 个 run 目录：`restored=173`、`protective-snapshot=1`、`captured=0`、`restore-failed=0`、其它=0、无 manifest=0 | **无未完成/未知/captured**，无需恢复或清锁 |
| guard 最新 run | `20261025T074832Z`（`status=restored`） | 15:00Z 后无新 run |
| 单槽存档 | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**，mtime `2026-10-25T07:46:36.0484820Z` | 与要求值一致；未 Save/Load/恢复 |
| 34 项 workspace 基线（按 `R3B-20261022/workspace-baseline.json` 逐项 bytes+SHA256） | **entries=34 / same=34 / diff=0 / missing=0** | 全一致 |
| 受保护字体 `Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 与基线同值 |

## 5. 布局运行证据目录

| 检查 | 实测 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/` | **不存在** |
| 该用例运行日志/截图 | 无 |
| `testResults` | 0 条 |

即：该布局用例**仍未执行**，本轮只做前置核对，不产生也不补写运行证据。

## 6. 门禁判定与移交

前置门禁条件（本阶段逐项实测）：

1. Unity `2022.3.62t9` ✔
2. Editor 空闲：未播放、未暂停、未编译、未更新 ✔
3. 无编译失败（`scriptCompilationFailed=False`）✔
4. 场景干净（`isDirty=false`）✔
5. guard 无未完成/未知/captured（174 = 173 restored + 1 protective-snapshot）✔
6. 测试源码当前字节与 SHA256 等于固定值 ✔
7. 目标 FQN 唯一且已编译（源码 1 处、DLL 中方法存在且含 `UnityTestAttribute`）✔
8. 无未知测试 ✔

**移交项（本执行层不做）**：宿主 Studio 阶段 check 串行**真实执行且仅执行**该一个方法
`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`（PlayMode），不运行既有 11 例。执行后由后续阶段回读真实分辨率、截图、`RESOLUTION-RESTORE-VERIFY`、SAVE/BASELINE/FONT 证据；若宿主 batchmode 的 `Screen.SetResolution` 能力不支持，保留该用例自身的“Editor 能力失败”真实失败，**不改 expected、不伪造分辨率或截图**。

## 7. 本阶段边界与实际副作用

- 未运行 `tests-run`，未启动/忙等 TestRunner，未投递任何测试请求。
- 未修改产品源码 / 测试源码 / 断言 / Scene / Prefab / 资产 / 静态配置 / 字体 / Packages / ProjectSettings / 34 项基线 / 存档本体 / `SaveSlotGuard` / 备份历史 run / 已有证据日志。
- 未 `rql-mutate`；未 Shell / `Process.Start`；未 Git 写；未全局 `SaveAssets/SaveChanges`；未 `AssetDatabase.Refresh`；未设置分辨率；未启停播放；未 Save/Load；未写 manifest。
- 读取手段仅：`rql-query`（editor/scenes/tests/testResults/console）与**纯读取** `script-execute` C# 片段（文件字节/SHA256/mtime、基线比对、只读反射原生测试器状态），无文件写入。
- 唯一写入：本文件（不在 34 项基线内）。
- 未安装组件、未改权限、未创建额外付费子 Agent、未访问授权目录外的工程或认证目录。
- 未能取值项：原生 `RunData` 静态实例（不公开），已如实标注，不影响空闲判定。
