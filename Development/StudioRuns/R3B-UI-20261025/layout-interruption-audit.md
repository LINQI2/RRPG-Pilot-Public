# R3-B 布局中断产物与工程只读审计（阶段 1/1）

- 审计时刻：2026-10-25T15:34:22Z（本机 23:34:22 +08:00）
- 授权范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`；本包唯一写入为本文件。
- 结论口径：**布局用例“已实现、已编译、未执行、无运行证据”，不判定 PASS，也不重做已实现用例。**

## 1. 结论摘要

1. 中断前最后写入的是测试源码 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（15:30:57Z，`316260B / 26b43bf8…`），与主开发固定值完全一致；产品源码 `ManagementView.cs` 与 Prefab 均未被本次布局工作改动。
2. 随后测试程序集在 15:31:41Z 完成编译（DLL 晚于源码 44 秒），`EditorUtility.scriptCompilationFailed=False`；DLL 内包含布局用例方法与 `layout-input-evidence` 等字符串，Test Runner 当前可发现 **12 例**（原 11 例 + 布局第 12 例）。
3. 布局证据目录 `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence` **不存在**，无日志、无截图；`testResults` 为空。可见范围内**没有该布局用例执行过的证据**，不能写成已运行/已通过。
4. 存档保护面稳定：单槽 `13405B / 4b2b56f6…d6aa`（mtime 07:46:36Z，未再变化）；guard 174 个目录 = 173 `restored` + 1 `protective-snapshot`，无 `captured` / `restore-failed` / 未知 / 不可读；最新 run 为 07:48:32Z，15:00Z 后无新 run。
5. 34 项 workspace 基线逐项 bytes+SHA256 **34/34 一致**；受保护字体 `14961033B / 3c758e16…c404854` 与基线一致。
6. 安全续接**成立**。最小下一步是 Studio 宿主**单方法**运行布局用例，而不是重跑既有 11 例或重写实现。

## 2. 身份、Editor 与原生测试器现场

| 项目 | 实测值 | 判定 |
|---|---|---|
| Unity 版本 | `2022.3.62t9` | 与授权身份一致 |
| 工程路径 | `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | 一致 |
| Editor | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` | 空闲 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`）`isDirty=false`、已加载、5 根 | 干净 |
| Prefab 编辑模式 | 未打开任何 Prefab | 无未保存 Prefab |
| 原生 TestRunner | `TestRunnerApi.IsRunActive=False`；`EditModeLauncher.IsRunning=False`；`PlaymodeLauncher.IsRunning=False`；`RunData.instance=null` | 空闲（内部 API 只读反射） |
| `testResults` | 0 条 | **只作旁证，未用它代替原生空闲判定** |
| Studio pending activeRequest | 无授权读取入口 | **不可见**；只能确认本包多次 MCP 只读调用均正常返回、无挂起调用 |
| 原执行原生工具记录 / Editor.log | 工作区内无原始会话日志；Editor.log 在授权目录外 | **不可见**，不猜测 |

## 3. 源码完整性与编译新鲜度

| 文件 | bytes | SHA256 | mtimeUtc | 说明 |
|---|---:|---|---|---|
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 316260 | `26b43bf89e6452c1aff829cb6b4b25a55ff61ca9095da2d3205331c3ceea599f` | 2026-10-25T15:30:57.0257006Z | 与主开发固定值一致；15:00Z 后 Assets 内唯一被写的文件 |
| `Assets/Scripts/Village/ProductHome/ManagementView.cs` | 37518 | `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d` | 2026-10-23T12:06:15Z | 未被本次布局工作改动；无 layout 方法 |
| `Assets/Resources/UI/ManagementView.prefab` | 282097 | `ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11` | 2026-10-23T12:10:23Z | 与固定值 `282097 / ade4c7e1` 一致 |

编译迹象：

- `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll`：468992B，mtime `2026-10-25T15:31:41.4291708Z`；pdb 118544B，`15:31:41.4259722Z`。均晚于源码 `15:30:57Z`。
- `EditorUtility.scriptCompilationFailed=False`；`EditorApplication.isCompiling=False`。
- DLL 字符串包含 `ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`、`layout-input-evidence`、`SCREENSHOT-UNAVAILABLE`、`RESOLUTION-RESTORE-VERIFY`，证明编译产物确实包含最终布局代码，而非仅旧 DLL 的发现缓存。
- Console 尾部仍保留 **中间版本** 的编译错误：seq 56898/56899，`ManagementShellPlayTests.cs(1698/1744): CS0030 Cannot convert GameObject to Component`。其后的最终编译只产生 `Screen.SetResolution` 过时警告（seq 56901/56902，行 1624/1869），且 `scriptCompilationFailed=False`；上述错误应视为**已被后续源码修复的陈旧错误**，不能据此判当前源码不可编译。
- 未发现被中断执行显式调用 `AssetDatabase.Refresh` 的记录；但 DLL 生成与 `scriptCompilationFailed=False` 证明最终刷新/编译已完成。场景当前干净，34 项与 Prefab 均未变，故没有“刷新时把 dirty 资产保存坏”的可见后果。

类括号与结构：文件首部 `namespace RRPG.ManagementShell.Tests {`（第 18 行）、`public sealed class ManagementShellPlayTests {`（第 58 行），文件尾第 4854/4855 行为类与命名空间闭合；最终编译成功，括号与语义结构闭合成立。

## 4. 12 例发现与布局 helpers

- 源码 `[UnityTest]` 正好 **12** 处；RQL `tests` 源按 `ManagementShellPlayTests` 过滤返回 **12** 条，含 `ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`。
- 布局用例位于第 1816 行，真实请求 `1920x1080` 与 `1280x720`，用 `Screen.SetResolution`，不做 RectTransform 缩放冒充；batchmode 若不支持会产生明确“Editor 能力失败”断言，而不是伪造。
- 新增 helpers 均已定义且被引用：`LayoutEvidenceDir`、`LayoutEvidence`、`ScreenRectOf`、`AssertInsideViewport`、`AssertControlHit`、`AssertFieldControl`、`AssertCanvasMatchesResolution`、`ApplyTestResolution`、`CaptureLayoutShot`、`LastActiveRow`、`AssertRowReachable`、`AssertScrollReachable`、`AssertTabLayout`（共 13 个；定义位置 1502–1776 行）。
- 产品侧没有新增 layout 方法；布局验证完全落在测试侧，符合“不改产品结构”的预期。

## 5. 运行/截图证据

| 检查 | 结果 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/` | 不存在 |
| 该目录下 `layout-input-evidence.log` / PNG | 不存在 |
| `Development` 下新增布局截图 | 未发现（仅历史 SourceAssets 图片） |
| `testResults` | 0 条 |
| `layout-input-test-review.md` | 不存在 |

**判定**：布局用例没有可交叉核对的运行回执。没有证据时只记录“未执行/无证据”，不推断曾通过，也不因中断而重做已写好的 12 例。

## 6. 34 项、字体、单槽与 guard

| 项目 | 实测 | 判定 |
|---|---|---|
| 34 项 workspace baseline | `entries=34 / same=34 / diff=0 / missing=0`（逐文件 bytes+SHA256，按 `R3B-20261022/workspace-baseline.json`） | 全一致 |
| 受保护字体 | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 与基线一致 |
| 单槽存档 | `1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot\rrpg_save_p0.json`：`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，mtime `2026-10-25T07:46:36.0484820Z` | 与要求值一致；无 `.tmp` |
| guard 根 | 174 个 run 目录：`restored=173`、`protective-snapshot=1`（`20261024T052704Z`）、`captured=0`、`restore-failed=0`、其它=0、无 manifest=0、不可读=0 | 无未完成/未知 |
| guard 最新 run | `20261025T074832Z`，`status=restored`，capture/updated 均为 `2026-10-25T07:48:32Z` | 15:00Z 后无新 run |

没有 `captured`/`unknown`，因此本包**未恢复、未清锁、未重跑**任何历史 run 或存档。

## 7. 原执行最后写入与中断点（可见部分）

1. `15:30:57Z`：布局测试源码最后一次写入（`316260B / 26b43bf8…`）。
2. `15:31:41Z`：`BattleStage1.PlayModeTests.dll` 生成，编译通过（仅过时 API 警告）。
3. `15:31:46Z`：主开发固定状态时点。
4. 此后至审计时：无新 guard run、单槽 mtime 未变、无布局证据目录、无 `layout-input-test-review.md`。

据此可将中断点定位为：**实现写完并编译成功之后、布局用例执行/取证/写报告之前**。原始 DSH 会话的完整工具序列与 Studio `activeRequest` 队列不可见，以上为工件级证据，不冒充原始日志。

## 8. 安全续接条件与下一最小步骤

安全续接条件（当前均成立）：

- Unity 身份 `2022.3.62t9`，Editor 空闲，场景 `isDirty=false`，无打开 Prefab；
- 测试源码等于固定 `316260B / 26b43bf8…`，编译产物更新且 `scriptCompilationFailed=False`；
- 12 例可发现，布局用例在 DLL 中；
- guard 无未完成 run，单槽 `13405B / 4b2b56f6…`，字体与 34 项一致；
- 不在已有运行证据缺失时宣称 PASS。

下一最小步骤（交 Studio 阶段执行，本包不做）：

1. 运行前只读复核身份/场景/guard/单槽/字体（不重复本包已做的源码改写）。
2. 宿主**只跑单方法** `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_AtTwoResolutions_KeepsTabsCommandsAndScrollReachable`（PlayMode），不重跑既有 11 例。
3. 运行后检查 `layout-input-evidence/layout-input-evidence.log` 与截图；若 batchmode 不支持 `Screen.SetResolution`，按测试自身“Editor 能力失败”记录，不伪造分辨率或截图。
4. 仅在该单方法有真实回执后，再决定是否补写 `layout-input-test-review.md`。

## 9. 本包边界与实际副作用

- 未运行 `tests-run`、未启动/忙等 TestRunner、未修改/重编译源码、未 `AssetDatabase.Refresh`、未保存资产或场景。
- 未 `rql-mutate`、未 Shell/`Process.Start`、未 Git 写、未导入资产、未设屏幕尺寸、未 Save/Load、未写 manifest、未改存档/guard/证据日志。
- 仅使用只读 `rql-query` 与纯读取 C# 片段（文件字节/SHA256/JSON/反射状态），无文件写入。
- 唯一写入：本文件 `Development/StudioRuns/R3B-UI-20261025/layout-interruption-audit.md`（不在 34 项基线内）。
- 未能读取的层次：原 DSH 工具记录、Studio pending `activeRequest`、Editor 原生日志；已如实标注，不猜测。
