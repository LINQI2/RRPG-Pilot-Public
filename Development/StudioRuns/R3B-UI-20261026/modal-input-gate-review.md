# R3-B 输入门禁小包：合成输入阳性对照与真实 Host 模态阻断（2026-10-26）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包性质：**实现 + 测试源码**。只改 `HomeWorldCamera.cs`（Editor-only internal 输入采样缝）和 `ManagementShellPlayTests.cs`（新增 1 例；原 13 例未改）。本文件为说明 / 证据记录。
- 未改 World / Work / Clock 规则、Authority、SaveDTO、Prefab、Scene、存档、manifest、Prefs、34 项基线；未 `rql-mutate`、未 PrefabStage、未字体探针、未 Shell / Git 写、未安装输入包、未用 OS 注入、未改 1280。
- 状态口径（2026-10-26 当时）：实现与编译已完成；运行结果**当时**未取得——DSH 侧 `tests-run` 被持久测试门禁拒绝（原文见 §6），按规则不绕过，交 Studio 阶段 check 执行。R3-B 仍 **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**。
- **当前附记（2026-10-27，阶段 2/2 只读核对 + 文档）**：§6“没有新增用例的 Passed / 日志 / 证据行，交 Studio check”是 **2026-10-26 当时口径，已被宿主实际运行取代**，不再作为当前结论：
  - **输入单例**：主开发原始核 `df6c6645-56ce-4b85-92d0-3e4bf8aac552` → **1/1 Passed、0 skipped、2.6441609 s、scope-restored**；本机证据日志 `2026-10-26T11:08:08.320Z–11:08:10.395Z` 含 `INPUT-GATE-POINTER / POSITIVE / MODAL / PERSON / REOPEN / AFTER-CLOSE / CLEANUP`、`SAVE-RESTORE matches-before=True`、`FONT-POST unchanged=True`、`BASELINE-POST 34/34`。
  - **管理 Play 整类（含本用例）**：`2026-10-27T02:20:37.866Z–02:20:50.545Z` 段 **14/14 Passed**（`testResults` 14 条全 Passed、`message=null`、方法耗时合计 **12 256 ms**；宿主 `tests-run` 调用级 `24 131.7488 ms`）；本用例 `durationMs=667`，日志 `INPUT-GATE-POSITIVE` 非零、MODAL / PERSON 不变、AFTER-CLOSE 可逆、CLEANUP 有行。14 类 runId / skipped / run 级时长本层不可见 → **待主开发补**。
  - **仍未证**：物理鼠标 / 键盘 / 物理 ESC / 手感；`HandleEscape` 仍为 public 回调证据（非物理键）。R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**，不升级 Completed / Accepted。

---

## 1. 实际改动文件

| 文件 | 改动 | bytes | sha256 | mtimeUtc |
|---|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeWorldCamera.cs` | 提取私有只读输入快照 + Editor-only internal 实例级可逆采样覆盖；公共 `ReadInput(bool,bool)` 签名不变 | 5077 | `39caba4b2f49aa360c2fab765c4f8819d50951522f9ad5ea7fe92f581a66728f` | `2026-10-26T11:04:08.7201777Z` |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 新增 `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState`（1 例）及同例辅助；原 13 例源码未改 | 408932 | `0025be07393e97462c1f57e429416fb2b9dcb888f06e8871a1448e6dd47332f1` | `2026-10-26T11:06:12.7673634Z` |
| `Development/StudioRuns/R3B-UI-20261026/modal-input-gate-review.md` | 本文件（新增） | — | — | — |

未删除任何历史记录；`two-slot-and-input-review.md` 与其他既有文件未由本包改写。

---

## 2. `HomeWorldCamera` 采样缝（只替换 legacy Input 采样数据）

### 2.1 结构

- 新增 `private readonly struct InputSnapshot`：`Mouse`、`RightHeld`、`RightDown`、`MiddleHeld`、`MiddleDown`、`ScrollY` 六个只读字段。
- `ReadInput(bool overUI,bool suppressRightPan=false)` 仍在原位置、原签名，先 `input=SampleInput()`，之后的门禁判断与 `Pan` / `Zoom` / `_lastMouse` 赋值逐句沿用原逻辑：
  - `(!suppressRightPan && input.RightHeld) || input.MiddleHeld`
  - `!input.RightDown && !input.MiddleDown`
  - `Pan((Vector2)(_lastMouse-input.Mouse)*scale)`
  - `Zoom(-input.ScrollY*.55f)`
  - `_lastMouse=input.Mouse`
- `SampleInput()`：
  - `#if UNITY_EDITOR` 下若 `_inputSampleForTests.HasValue` 返回该采样；
  - 否则（生产默认）返回 `Input.mousePosition / GetMouseButton(1) / GetMouseButtonDown(1) / GetMouseButton(2) / GetMouseButtonDown(2) / mouseScrollDelta.y`。读 legacy Input 无副作用，因此把按钮 / 滚轮采样提前到统一快照，不改变任何分支语义。

### 2.2 Editor-only internal 可逆入口

```csharp
#if UNITY_EDITOR
private InputSnapshot? _inputSampleForTests;
internal void SetInputSampleForTests(Vector3 mousePosition,bool rightHeld,bool rightDown,
                                     bool middleHeld,bool middleDown,float scrollY)
internal void ClearInputSampleForTests()
#endif
```

- 实例级字段，不是静态 / 全局单例；不写盘、不进入 SaveDTO、不影响 Authority。
- 覆盖只决定“输入快照数据”，**不**决定 `overUI` / `suppressRightPan`；真实 `ProductHomeWorldHost.Update` 仍计算 `overUI=Shell.IsPersonModalOpen || PointerOverUI(Input.mousePosition)`，仍调用同一个 `ReadInput`。
- `#if UNITY_EDITOR` 之外没有字段、方法、分支：Player 中不存在测试覆盖入口，也没有对应判断。
- 未新增 public API；`ReadInput` 仍为唯一 public 输入入口，签名 `(bool,bool)` 不变。

---

## 3. 新增 Play 用例覆盖（单例，复用同一 `SaveSlotGuard` 与真实产品入口）

用例：`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState`

| 段 | 断言 | 性质 |
|---|---|---|
| 入口 | `ProductHomeWorldHost` 必须单例、`HomeWorldCamera` 必须唯一；`SetInputSampleForTests` 存在且 `IsAssembly`（internal）、public 版本不存在；`ReadInput` 参数仍为 `(bool,bool)` | 编译期 / 反射边界 |
| 阳性对照（非模态 World） | 真实指针必须不在 UI 上（环境前置，失败给出 Raycast 命中证据）→ 注入右键拖动 + 滚轮 → 只 `yield` 真实帧 → 相机 `transform.position` 与 `orthographicSize` 必须改变 | **真实 Host.Update → 同一 ReadInput**，不是直接 `Pan/Zoom` |
| 清空采样 | `ClearInputSampleForTests()` 后连续真实帧相机保持稳定 | 可逆性（实例级） |
| Management 模态 | 真实底部导航打开管理；`Surface` 保持 `Management`；`SurfaceModalActive` 与 `IsPersonModalOpen` 同时为真；旧 Town `CanvasGroup.interactable / blocksRaycasts` 为假；背景点 `PointerOverUI==true` 且 `HandleWorldClick` 不改变 Selection / Surface | 模态门禁 + 公开回调 / 真实 Raycast |
| 相同的合成输入序列（模态中） | 与阳性对照相同的鼠标位置、按下序列、滚轮值；连续 5 个真实帧后相机位置 / 尺寸**不变**，Selection、建造 `IsOpen / SelectedDefinitionId / Candidate`、Clock `Speed / SpeedMultiplier`、`IsSimulationActive`、Surface 全部保持 | **真实帧阻断证据**（overUI 由 `Shell.IsPersonModalOpen` 决定，不依赖真实指针位置） |
| ESC（公开回调） | `Call(_shell,"HandleEscape")` 只关闭当前 Management → World；`SurfaceModalActive / IsPersonModalOpen` 同步为假；旧 Town CanvasGroup 恢复原值；Selection / Clock 不漂移；建造未打开 | **回调机制**，非物理 ESC |
| Person 模态 | 真实“人物”导航打开；两别名同时为真；旧 Town 再次禁用；相同合成输入序列连续真实帧后相机 / Selection / Clock / Surface 保持 | **真实帧阻断证据** |
| ESC（Person） | 只关闭当前 Person Surface；别名同步；CanvasGroup 恢复；Selection / Clock 不漂移 | **回调机制**，非物理 ESC |
| 重复开关 | 再次打开管理 → 旧 Town 禁用；`CloseSurface()` → 恢复原值；Selection / Clock 不漂移 | Town CanvasGroup 不漂移 |
| 关闭后再次注入 | 真实指针前置 → 相同合成输入 → 相机位置 / 尺寸再次改变 | 门禁可逆（依赖模态而非永久吞输入） |
| `finally` | 无论如何 `ClearInputSampleForTests()`，并写 `INPUT-GATE-CLEANUP` 证据行 | 清理 |

明确口径：本用例**不**证明物理鼠标 / 物理键盘 / 物理 ESC；`HandleEscape`、`HandleWorldClick`、`RealPointerClick`（EventSystem 程序化点击）都标为回调 / 程序化事件机制证据。也没有走“无输入的 `ReadInput(true)` 不变”这种弱证据。

---

## 4. 已取得的实际执行证据（本机，Editor 内）

1. `script-execute` 调用 `AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate)` 两次（改动后各一次），Editor 完成编译；`console` 中无本包源码的编译错误（仅有本包自身失败的探测 `script-execute` 片段报错，非产品源码错误）。
2. 编译后反射核对（`script-execute`，只读）：
   - `HomeWorldCamera.SetInputSampleForTests`：存在、`IsAssembly=true`（internal）、唯一 1 个；
   - `ClearInputSampleForTests`：存在；
   - `_inputSampleForTests`：`IsPrivate=true`；
   - `ReadInput`：public、参数数 **2**；
   - `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState`：编译存在、返回 `IEnumerator`；
   - 该 fixture 的 `[UnityTest]` 方法数：**14**（原 13 + 新增 1；原 13 例未改）。
3. 未运行 Player 构建；Player 中“字段 / 方法 / 分支不存在”由 `#if UNITY_EDITOR` 静态保证，本报告不声称已用 Player 构建实证。

> **2026-10-27 追加（宿主实际运行，只读回执）**：新增单例由宿主运行并通过（`df6c6645…`，1/1、0 skipped、2.6441609 s、scope-restored；日志 `INPUT-GATE-*` 全行 + `BASELINE-POST 34/34`）；同源管理 Play 整类 `2026-10-27T02:20:37.866Z–02:20:50.545Z` 段 **14/14 Passed**（方法耗时合计 12 256 ms，本用例 667 ms）。此前“未取得运行结果”仅指 2026-10-26 当时，**不是**“宿主从未运行”。

---

## 5. 宿主 / 资产安全核对

- `OneTimeSetUp` 的 34 项基线不包含 `HomeWorldCamera.cs` 或 `ManagementShellPlayTests.cs`，本次修改不会触发基线哈希断言；未读取 / 改写基线文件。
- 受保护 TMP 字体、单槽存档、既有 Run 目录未由本包写入（2026-10-26 当时 `tests-run` 未启动；后续宿主运行与保护面见顶部当前附记与 `R3B-UI-20261027/current-contract-evidence.md`）。
- 场景 `SampleScene` 未变（`isDirty=false`，5 roots）；Editor 当前 `isPlaying=false`、`isCompiling=false`。
- 未 `SaveAssets / SaveChanges / ClearDirty / SetDirty`；未 `AssetDatabase.SaveAssets`；未创建 / 修改 Prefab 或场景。

---

## 6. 未取得的执行结果：DSH 测试门禁（精确原文；2026-10-26 当时口径，已由顶部当前附记更正）

尝试用 `tests-run` 只跑新增用例（PlayMode，`testMethod` 精确 FQN），返回：

> `Unity测试由Studio持久测试阶段执行；请提交stages.check，勿从DSH直接启动。`

因此本包（2026-10-26 当时）**没有**新增用例的 Passed / 日志 / 证据行；`Library/.../management-shell-play-evidence.log` 未被本包追加。按规则不绕过门禁，交 Studio 阶段 check 执行。**（2026-10-27 更正：宿主已实际运行并取得回执，见顶部当前附记与上表后的追加。）**

Studio 建议执行（只跑新增单例，复用同一 `SaveSlotGuard` 与真实产品入口）：

```text
testMode: PlayMode
testMethod: RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState
```

执行后应重点核对证据行：`INPUT-GATE-POINTER / POSITIVE / MODAL / PERSON / REOPEN / AFTER-CLOSE / CLEANUP`。

### 6.1 已知环境前置（必须如实记录）

阳性对照与“关闭后再次注入”两段依赖 `ProductHomeWorldHost.Update` 的既有 `overUI` 计算，其中 `PointerOverUI(Input.mousePosition)` 读取**真实** legacy 指针。若 Studio 运行时真实指针恰好压在 UI 上，用例会以明确的环境前置断言失败并打印 Raycast 命中列表；**这不能解释为产品模态阻断开镜失败**，也不是本包可以合法消除的输入注入范围（禁止 OS 注入、禁止改 Host 门禁）。模态阻断段本身不依赖真实指针位置（`overUI` 由 `Shell.IsPersonModalOpen` 为真决定）。**2026-10-27 宿主运行实测**：真实指针 `(42194.94, -43048.71)` 在屏外，`overUI=False`、无 UI 命中，未触发该环境前置失败。

---

## 7. 边界与结论

- 实现内容：仅 `HomeWorldCamera` 的私有快照 + Editor-only internal 实例级可逆采样；仅一个 Play 用例；原 13 例未改；无 public API / Authority / SaveDTO / 规则变更。
- 已取得（2026-10-26）：编译通过、方法唯一性与可见性、用例存在、fixture 14 个 `[UnityTest]`。
- **已取得（2026-10-27 宿主运行）**：输入单例 `df6c6645…` **1/1 Passed、0 skipped、2.6441609 s、scope-restored**；管理 Play 整类 **14/14 Passed**（`message=null`，方法耗时合计 12 256 ms；14 类 runId / skipped / run 级时长待主开发补）。原先“未取得运行结果”仅指 2026-10-26 当时门禁拒绝，已被宿主实际运行取代。
- 证据边界：本包不声称物理鼠标 / 滚轮 / 键盘 ESC 穿透已验证；不声称 Player 构建已验证；不声称 R3-B 完成或获制作人批准。
- 本文件须由主开发专业核验。
