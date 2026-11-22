# R3-B ESC 一次采样与真实 LateUpdate 门禁专项（2026-10-27，阶段 1/1）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`）。未访问未授权工程、认证目录或其他工作区；未安装组件、未改权限、未改计费渠道、未创建额外付费子 Agent。
- 本阶段性质：**只改 2 个源码/测试文件 + 只新增本文件**，并在 Editor 内做一次 `AssetDatabase.Refresh` 编译与只读发现核对；**模型未运行 `tests-run`**，16 例整类未重跑，新用例只交宿主 Studio 阶段 check 单例执行。
- 日期口径：当前 **2026-10-27**；本文件不重写 10-22 / 10-23 / 10-24 / 10-25 / 10-26 的任何历史产物。
- 状态口径：R3-B 仍 **Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；本文件须由**主开发专业核验**，不代表制作人批准，也不把 requiredChecks 升级为 Completed / awaiting_acceptance。

---

## 1. 本轮要修的准确缺口

原 R3-B 规格含“键盘不穿透”。此前所有 ESC 证据（含 CASE-E 的 `ESC-BUILD / ESC-CLOSE`）都是 `Call(_shell,"HandleEscape")` 直接调用 **public 回调**，绕过了产品真实帧里的 `LateUpdate → Input.GetKeyDown(KeyCode.Escape)` 轮询。因此：

- **已覆盖**：`HandleEscape` 的公开回调 / 取消顺序机制；
- **未覆盖**：public 回调与 `LateUpdate` 实际采样路由不是同一件事。

主开发批准的最小修法：仅在 `HomeProductShell` 加 **#if UNITY_EDITOR 实例级 internal 一次性 ESC 采样入口**，与 `HomeWorldCamera` 的 Editor-only 输入采样先例一致；不新增 public API、不改输入 Authority、不新增玩法、不改 Player 字段/分支。

---

## 2. 安全预检（只读实测，均为本轮亲自执行）

| 项 | 实测值 | 判定 |
|---|---|---|
| Pilot 空闲 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`、`isUpdating=false` | 空闲 |
| 场景 / PrefabStage | `SampleScene` loaded+active、`isDirty=False`、`PrefabStage=null` | 干净 |
| 34 项磁盘基线 | `total=34 / problems=0`（逐项 bytes + sha256 对比 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`） | 全一致 |
| 受保护 TMP 字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` | 未变 |
| 受保护 TMP 字体（内存） | `LoadAllAssetsAtPath` 9 个子资产、`EditorUtility.IsDirty` **dirty=0** | 无内存脏写；未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`、mtimeUtc `2026-10-27T02:38:46.2937955Z`、`.tmp=False` | 内容为原始单槽；mtime 由宿主上一轮 15/15 运行收尾恢复回写产生，本轮未改 |
| guard 目录 | `manifests=275`：`restored=274` + `protective-snapshot=1`；`pendingRestore=0`；最新 restored = `20261027T023845Z`（`readbackVerified=true`、`sourceSha256=4b2b56f6…d6aa`） | 无 unfinished / 无待人工取证项 |
| 分辨率 | 沿用 **1920×1080**（1280 已由制作人 `01a53eb8` 取消） | 本轮不询问、不设门禁、不由 1920 外推 |

> 预检时间约 `2026-10-27T02:43–02:44Z`；上述 34 项 / 字体 / 单槽 / guard 在本轮写后复核仍完全一致（见 §6）。

---

## 3. 代码改动（`HomeProductShell.cs`）

文件：`UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/HomeProductShell.cs`

### 3.1 真实 LateUpdate 路由（先读真实方法，未误写 Update）

改动前真实入口位于 **`private void LateUpdate()`** 第 95 行：

```csharp
if(Input.GetKeyDown(KeyCode.Escape)) HandleEscape();
```

改为（同一 `LateUpdate`、同一 `HandleEscape`、同一位置、同一前置 `ShellVisible` 门禁）：

```csharp
if(SampleEscapePressed()) HandleEscape();
```

### 3.2 私有采样方法 + Editor-only 一次性注入

```csharp
private bool SampleEscapePressed()
{
#if UNITY_EDITOR
    if(_escapePressPendingForTests)
    {
        _escapePressPendingForTests=false;   // 一次性消费：同一注入绝不重复触发关闭
        return true;
    }
#endif
    return Input.GetKeyDown(KeyCode.Escape); // 生产默认（其余帧即回落到此默认采样）
}
#if UNITY_EDITOR
private bool _escapePressPendingForTests;
internal void SetEscapePressForTests() { _escapePressPendingForTests=true; }
internal void ClearEscapePressForTests() { _escapePressPendingForTests=false; }
#endif
```

明确契约：

| 状态 | `SampleEscapePressed()` 返回 |
|---|---|
| 未注入（生产 / Player / 普通帧） | `Input.GetKeyDown(KeyCode.Escape)`，与改动前逐句等价 |
| 注入一次且尚未被采样 | 返回 `true`，并**立即**把 pending 置 false（只消费一次） |
| 注入已消费后的其余帧 | 回落到生产默认采样（测试环境无物理 ESC ⇒ false），不会重放 |
| `ClearEscapePressForTests()` | 清除未消费注入，可逆、幂等；恢复生产采样 |

边界保证：

- `SetEscapePressForTests` / `ClearEscapePressForTests` 均 `internal`（反射检查 `IsAssembly=True`），**无 public 版本**；`SampleEscapePressed` 为 `private`；
- `public void HandleEscape()` 与 `public void CloseSurface()` 签名未改，`Build`/`PersonDetail`/`Router` 等公共属性未改；
- 不新增字段入口分支到 Player：字段、方法、`#if` 分支在 Player 编译中全部不存在（与 `HomeWorldCamera` 同口径）；
- `HandleEscape()` 内部既有的 `if(!Build.IsOpen) CloseSurface();` 未改，`LateUpdate` 既有的 `if(!ShellVisible) return;` 未改，因此未绕开 `Build.IsOpen` / `ShellVisible` / `SurfaceModalActive` 链路；
- 不写盘、不 `SetDirty / ClearDirty / SaveAssets / SaveChanges`、不碰存档 / Authority / Prefab / Scene；
- 本修法未引入更大的输入系统重构（无新输入层 / 无第二套 Input / 无 Authority 改动）；若将来需要真实注入 legacy 键盘事件，应另立授权包，本包不扩大。

静态核对（编译后反射 + 源码正则）：`SampleEscapePressed` 只有 1 处调用（LateUpdate），文件内生产 `Input.GetKeyDown(KeyCode.Escape)` 仍只有采样方法内 1 处，`SampleEscapePressed` 声明 1 处 + 调用 1 处。

---

## 4. 新增 Play 用例（只动 `ManagementShellPlayTests.cs`）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（+ 文件头 1 段说明；原 15 例逐字未改）

用例名（与主开发批准完全一致）：

```text
RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate
```

用例断言链（全部走真实产品入口 / 真实帧）：

1. **静态形态**：宿主单例；`SetEscapePressForTests` 存在、`internal`、无参、无 public；`ClearEscapePressForTests` 同为 `internal`；`SampleEscapePressed` 为 `private bool`；`LateUpdate` 是 private Unity 消息方法；`HandleEscape` 仍 `public void` 无参。
2. **管理模态**：真实底部导航 `Navigate2` 打开 → `Surface=Management`、`SurfaceModalActive=true`、Town `CanvasGroup.interactable/blocksRaycasts=false`；注入一次 press，让真实帧执行 `LateUpdate` → `Surface=World`、模态别名转 false、管理页未激活/不可见、`Selection` 不变、`Clock` 绝对分钟 / `Speed` / `SpeedMultiplier` / `IsSimulationActive` 不漂移、Town `CanvasGroup` 恢复打开前取值、建造未打开。关闭后的**下一帧**再断言 Surface 不回弹、不重复进入模态、Selection / Clock 不变。
3. **一次消费不残留 + 阴性输入**：真实导航重开管理，连续 **3 个无注入真实帧**（测试环境无物理 ESC）保持 `Surface=Management`、模态为真、Town 持续阻断；随后再注入一次才正常关闭，证明前面的重开不是被残留 press 误关。
4. **Person 模态**：真实底部导航 `Navigate1` 打开 → `Surface=Person`、`PersonDetail.IsVisible=true`、Town 阻断；注入一次 press → 真实帧只关闭 Person 回 World，PersonDetail 隐藏，Selection / Clock / Speed 不漂移，Town 恢复。
5. **建造门禁**：公开 `Build.OpenCatalog` + 公开 `Choose("building_farm")` + 公开 `SetCandidate((0,4))`，断言 `PreviewValid=true`（真实新世界无建筑：在建造范围内且无重叠）与 ghost 可见；注入一次 press → 真实 `LateUpdate` 进同一 `HandleEscape`，`Build.IsOpen` 分支必须让路：建造仍打开、候选定义 / 候选位置 / ghost 不变、Surface 不被关闭/切换、管理页不被重新激活、Selection 不变。
6. **合法关闭路径**：公开 `Build.Cancel` 关闭并清候选后，真实导航重开管理，再注入一次 press → 正常关闭当前管理 Surface，Town 恢复、Selection / Clock / Speed 不变。
7. **收尾**：`finally` 只调用 `ClearEscapePressForTests` 清除本实例注入标记，并断言私有 pending 字段为 false；不短路 `SaveSlotGuard` / `ResetForTests` / 活动日程恢复链；用例结束再断言宿主仍单例。

诚实边界（本用例不冒称）：

- 注入的是**合成“按下”采样**，**不是 OS 物理 ESC**，也不证明 `UnityEngine.Input.GetKeyDown` 的引擎实现；它证明的是“与生产同一条 `LateUpdate → SampleEscapePressed → HandleEscape` 采样 / 路由 / 门禁路径被执行”。
- 建造自身的物理 ESC / 右键轮询（`ProductHomeWorldHost.Update → HomeBuildingPlacement.ReadInput`）**本用例不注入**；本阶段只证明 **Shell 路由**在 `Build.IsOpen` 时必须让路。
- 产品公开顺序中 `Build.OpenCatalog` 自身会收起管理表面（`Surface=World`），因此公开入口下无法构造 “`Build.IsOpen` 且 `Surface≠World`”；用例断言的是候选 / ghost / Surface / 管理页全部未被 Shell 误动，不冒称更弱的“非 World 表面被保留”。
- 未构建 Player；“Player 内缝不存在”只有 `#if UNITY_EDITOR` 的静态保证，未做 Player 实证。
- 阴性帧依赖“测试运行期间没有物理 ESC 被按下”的环境前置（与既有鼠标 / 指针类用例同性质）。

---

## 5. 安全 Refresh、编译新鲜度与新 FQN 发现

只通过 `script-execute` 调用 `AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate)` + `AssetDatabase.Refresh()`（源码首次定稿后一次、测试注释校正后一次、合法候选补强后一次），未手工保存任何资产 / 场景。

| 检查 | 实测 | 判定 |
|---|---|---|
| 编译 | `isCompiling=true` → 轮询后 `false`；`isUpdating=false` | 编译完成 |
| Console | 产品 / 测试源码 `Error/Exception` **0**（`console` 源按 `logType == "Error" or logType == "Exception"` 查询 → 0 results） | 无编译错误 |
| `HomeProductShell.cs` | `17549B / fbcb85949f6876557ee1aed56e5261763f30d74b6d0e12e672ce6a813f573878`、mtimeUtc `2026-10-27T02:45:50.6363386Z` | 本轮源码 |
| `ManagementShellPlayTests.cs` | `466886B / cfcde4c5a8f0a9fa84ab785e7c1ab94bdbef305675d7b8f39524c06b6c8947cf`、mtimeUtc `2026-10-27T02:48:42.0123308Z` | 本轮测试源码最终版 |
| `Assembly-CSharp.dll` | `649216B / 1fb4e9e38038f0a4100c5af619e5c0973e33a2d245faf4d9f2b6dbb4b9d2f151`、mtimeUtc `2026-10-27T02:46:38.6043467Z` | **晚于** Shell 源码 52.97 s，新鲜 |
| `BattleStage1.PlayModeTests.dll` | `559616B / 52dc2caf861dbf485b998dc6d8176c6cd73114803bc6f70b78c28a86121e2a8e`、mtimeUtc `2026-10-27T02:48:44.9902708Z` | **晚于**测试源码 2.98 s，新鲜 |

新 FQN 发现（运行时反射 `BattleStage1.PlayModeTests`，只读）：

```text
TESTASM loaded unityTests=16 targetMatches=1
target=ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate unique=True
```

- fixture `[UnityTest]` 由 15 → **16**，且方法名在 fixture 内唯一；
- 源码 `[UnityTest]` 标注计数同样为 **16**；
- 原 15 例未改写、未删除、未弱化；未运行任何测试、未重复全集。

---

## 6. 写后只读复核（最终编译后，约 `2026-10-27T02:48:56Z`）

| 项 | 实测 | 判定 |
|---|---|---|
| Editor | `playing=false`、`paused=false`、`compiling=false`、`updating=false` | 仍空闲 |
| 场景 / PrefabStage | `SampleScene` loaded+active、`isDirty=False`、`PrefabStage=null` | 仍干净 |
| 34 项基线 | `total=34 / problems=0` | 未变 |
| 受保护字体 | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0` | 未变 / 未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、mtimeUtc `2026-10-27T02:38:46.2937955Z`、`.tmp=False` | 本轮零写盘 |
| guard | `manifests=275`（274 restored + 1 protective）、`pendingRestore=0` | 无 unfinished；本轮未新增 run |

本轮禁止项均未触碰：未 `SaveAssets / SaveChanges / SaveDirtyScenes / ClearDirty / SetDirty`，未 `rql-mutate`，未 PrefabStage，未字体测量（`GetPreferredValues` / `ForceMeshUpdate`），未 Preferences，未 Shell / Git / 正式工程，未启停 Editor，未构建 Player，未 `tests-run`。

---

## 7. 交宿主执行口径（本阶段只跑新单例）

```text
testMode: PlayMode
testClass: RRPG.ManagementShell.Tests.ManagementShellPlayTests
testMethod: RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate
```

- 期望发现：fixture 共 **16** 个 `[UnityTest]`，本次只执行上述 **1** 例（不跑 16 例整类、不重跑全集）。
- 通过标准：`1/1 Passed、0 skipped`、逐条 `message=null`，并取得真实 runId / run 级时长。
- 执行后核对既有保护链仍生效：`[BASELINE-PRE/POST]` 34/34、`[FONT-PRE/POST] unchanged=True`、`[SAVE-CAPTURE]` 与 `[SAVE-RESTORE] matches-before=True`、guard 新增 restored 且无 unfinished / `noManifest`、场景 `isDirty=False`、`PrefabStage=null`。
- 新用例重点证据行：`[ESC-FRAME] PRE / MANAGEMENT-CLOSED / NEGATIVE-FRAMES=3 / PERSON-CLOSED / BUILD-GATE / END / CLEANUP`。
- 若失败：保留完整 message / 堆栈 / 证据行，区分环境前置失败（如真实指针压在 UI、物理 ESC 被按下）与产品断言失败；**不得删改用例、降低断言或把失败改写成通过**。
- 单例通过 **不等于** 16 例整类通过，也不改写此前 `33b8271a` / `46d65625` / `99761145…` 等上游回执；整类 16 例是否需重跑由主开发另行决定。

---

## 8. 剩余边界（不冒称）

| 项 | 现状 |
|---|---|
| OS 物理 ESC / IME / 多指针时序 / 键盘手感 | 未覆盖；需物理设备 / 人工验收，本包不新增 requiredChecks |
| `Input.GetKeyDown` 引擎实现 | 未证明；本用例只证明产品层同一采样 / 路由路径 |
| Player 构建内缝不存在 | 仅 `#if UNITY_EDITOR` 静态保证，未构建 Player 实证 |
| 建造面板自身物理 ESC | 未注入；本阶段只证明 Shell 路由让路 |
| 双档位渲染 1280 | 制作人已取消；取消 ≠ 通过，不由 1920 外推 |
| 历史 650 存档缺口 / 旧 `ClearDirty` / 旧 `SaveAssets` 留痕 | 本包未处理、未洗白、未回滚 |
| 进程级 abort 下的 SaveSlotGuard | 仍只有用例 / `OneTimeTearDown` 兜底，非进程级保证 |

> 本文件与整包结论须由**主开发专业核验**；R3-B 状态、父 contract 终态与 requiredChecks 均不因本文件改变。

---

## 9. 追加：第 16 例 BUILD-GATE 首次宿主执行失败、根因与夹具修正（2026-10-27，阶段 1/1）

> 本节**只追加**，不重写 §1–§8 的历史记录；§4 第 5 条与 §7 中“候选位置不变”的旧期望，以本节 §9.3 的修正口径为准（旧口径在真实帧里把正常指针更新误当成 ESC 改写，已被宿主执行证伪）。本轮仍由同一编制者执行：只改第 16 例与 2 个专用只读 helper、只追加本文件；**模型未 `tests-run`**；单例仍交宿主 Studio 阶段 check。

### 9.1 原失败回执（宿主 Studio 阶段 check，真实结果）

| 项 | 值 |
|---|---|
| runId | `d6315432-f503-4768-969a-3b938d233dcd` |
| 结果 | `1 total / 0 passed / 1 failed / 0 skipped`（scope-restored） |
| 失败断言 | 第 16 例第 4 段：`Build.IsOpen 时 Shell 采样不得改写建造候选位置` |
| NUnit 值 | `Expected (0, 4)` / `Actual (578.06, -593.47)` |

证据日志（`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，`2026-10-27T02:49:56` 段）只有：

```text
[ESC-FRAME] PRE ...
[ESC-FRAME] MANAGEMENT-CLOSED ...
[ESC-FRAME] NEGATIVE-FRAMES=3 ...
[ESC-FRAME] PERSON-CLOSED ...
[ESC-FRAME] CLEANUP injection cleared; ...
```

没有 `BUILD-GATE` / `END`，说明失败发生在建造门禁段，`finally` 仍执行了注入清理；同 run 的 guard 新增 `20261027T024955Z` restored（`pendingRestore=0`）、场景 `isDirty=False`、单槽内容仍为原 `4b2b56f6…d6aa`，与“失败未写世界 / 未坏档”一致。

### 9.2 真实帧路径读取与根因实测（先读代码，再做只读实测；不是猜测）

产品真实帧路径（逐行核读）：

1. `ProductHomeWorldHost.Update` 第 75–76 行：每个真实帧
   `bool overUI=Shell.IsPersonModalOpen || PointerOverUI(Input.mousePosition);`
   `WorldCamera.ReadInput(overUI,Shell.Build.IsOpen);`
   `if(Shell.Build.IsOpen) Shell.Build.ReadInput(overUI);`
2. `HomeBuildingPlacement.ReadInput(overUI)` 第 99–106 行：
   `if(!IsOpen) return;` → `if(ESC || 右键) {Cancel();return;}` → `if(SelectedDefinitionId==null || overUI) return;` → `UpdatePointer(Input.mousePosition)` → 左键 `Confirm()`。
3. `UpdatePointer` → `WorldCamera.ViewCamera.ScreenToWorldPoint(new Vector3(mouse.x,mouse.y,10))` → `BuildingWorldMapper.ToLocal(point)` → `SetCandidate(...)`。
4. 全仓 `Candidate` 写入点只有 `SetCandidate`；真实帧里会调用它的产品来源只有 `Choose`（打开时一次，取相机位置）、`UpdatePointer`（每帧按真实指针）、`SetVisualPrototype`（重设同值）。`HomeProductShell.SampleEscapePressed / HandleEscape` **没有任何候选写入分支**，`HandleEscape` 在 `Build.IsOpen` 时直接让路。

只读实测（Editor 空闲、未启 Play、未运行测试）：

| 项 | 实测 |
|---|---|
| `Input.mousePosition` | `(42580.66, -42189.78, 0)`；间隔 300 ms 连续 3 次采样一致（headless / 无物理指针环境的合成指针值） |
| 屏幕 | `Screen = 1920×1080`、`dpi=168` |
| UI 命中 | Edit 只读探针时 `EventSystem.current == null`；PlayMode 由产品自身创建 EventSystem，但该指针远在屏幕外，`PointerOverUI` / `RaycastAll` 无 UI 命中（用例运行时仍会用真实 `PointerOverUI` 断言并把命中证据写入 `BUILD-POINTER-NEGATIVE`），即 `overUI=false`，真实帧必定进入 `UpdatePointer` |
| 产品换算复算 | 相机 `(10000,0,-10)`、`orthographicSize=7.5`、aspect `16/9`、`z=10`：<br>x = `10000 + (2×42580.66/1920 − 1) × 7.5×(16/9)` = `10578.06` → `candidate.x = 578.06`<br>y = `(2×(−42189.78)/1080 − 1) × 7.5` = `−593.47` → `candidate.y = −593.47` |

复算值与宿主回执的 `Actual (578.06, -593.47)` **逐位一致**。因此准确根因是：

> `SetCandidate((0,4))` 之后只要跨真实帧，`Host.Update → Build.ReadInput(!overUI) → UpdatePointer(Input.mousePosition)` 就会按当前真实（此处为 headless 合成值）指针把候选重算为 `(578.06,-593.47)`。这是**正常指针更新**，不是 `SampleEscapePressed / HandleEscape` 的 ESC 改写；第 16 例原断言把“活跃建造时的运行期候选”冻结成手动值，属**夹具断言错误**，不是产品缺陷。原运行没有证据表明 Shell 取消了建造、进入 `CloseSurface` 或写了世界（`BUILD-GATE` 未到达即失败，但失败点仅是候选位置值）。

### 9.3 夹具修正（只改第 16 例 + 2 个专用只读 helper）

`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`：

1. 新增专用 helper（均只读，不写产品状态）：
   - `PointerDerivedBuildCandidate(Vector3 pointer)`：与产品同一 `HomeWorldCamera.ViewCamera.ScreenToWorldPoint(pointer, z=10)` + `BuildingWorldMapper.ToLocal` 的等价换算，证明候选来源；
   - `WorldBuildingCount()`：只读 `Host.Snapshot.Buildings` 的真实建筑实例数，用于“未放置建筑 / 未写世界”。
2. 第 4 段改为真实帧阴性对照流程，**不冻结候选、不注入指针、不禁用 Host/Build 更新**：
   - 公开 `OpenCatalog` + `Choose("building_farm")` + 合法的 `SetCandidate((0,4))`，只断言“真实帧前的起始夹具”合法且生效；
   - 先跑 **2 个无 ESC 真实帧**，捕获同帧 `Input.mousePosition` 与 `Candidate`：
     - 环境前置（明确失败信息 + Raycast 命中）：真实指针必须不在 UI 上；指针换算必须 ≠ 手动候选；
     - 断言候选 **X/Y 都等于 `PointerDerivedBuildCandidate(真实指针)`**（直接证明候选变化来自正常指针更新）；
     - 断言 `IsOpen` / `SelectedDefinitionId` / `GhostVisible` / Selection / 世界建筑数不变；
   - 再跨 1 个无 ESC 真实帧（阴性对照），候选仍必须与真实指针换算一致；
   - 随后才 `SetEscapePressForTests()` + 2 个真实帧：
     - 断言注入位已被真实 `LateUpdate` 消费（`_escapePressPendingForTests == false`），证明路由真的执行；
     - 指针未变、相机位姿未变且非 UI 时，断言 ESC 帧候选与阴性帧候选**完全一致**，且等于 `UpdatePointer(当前指针)` 的换算；指针或相机位姿变化时按当前指针重算并记 `pointerStable/cameraStable=False`（不把正常 `UpdatePointer` / 滚轮缩放误判为 ESC 错误）；指针落入 UI（`ReadInput` 提前返回）时改为与阴性帧候选一致。
3. 保留的强语义（全部仍为真实断言）：
   - `Build.IsOpen` 保持为真（Shell 不取消建造）；
   - `SelectedDefinitionId == "building_farm"`（不清候选）；
   - `GhostVisible == true`（ghost 保持）；
   - `Surface == "World"` 且 `SurfaceModalActive == false`、管理页未激活（不进入 `CloseSurface` / 不重开管理）；
   - `Selection`、`Clock` 绝对分钟、`Speed`、`SpeedMultiplier`、`IsSimulationActive` 不漂移；
   - `Host.Snapshot.Buildings` 数量不变（未放置建筑 / 无世界写入）；
   - 公开 `Build.Cancel` 后才能关闭，`HandleEscape` 的 `Build.IsOpen` 让路语义未被削弱。

### 9.4 本轮编译 / 写后只读复核（实测证据）

| 项 | 实测 |
|---|---|
| 安全 Refresh | 仅 `script-execute` 调用 `AssetDatabase.Refresh(ForceUpdate)` + `AssetDatabase.Refresh()`（源码定稿后 3 次）；`isCompiling=true → false`，`isUpdating=false`；未手工保存任何资产 / 场景 |
| Console | 产品 / 测试编译 `Error/Exception` **0** 项（最终一次编译后查询 0 results） |
| `ManagementShellPlayTests.cs` | `477356B / 5820d5308e5edadc9eefd53617cb91e14c01564bd8e915429324eeb5fc3c1033`、mtimeUtc `2026-10-27T07:02:34.1971111Z` |
| `BattleStage1.PlayModeTests.dll` | `565248B / 0801d17f2dc576207cc2cfda81b3493dd865dd00798b61e625b22f7eac8d4200`、mtimeUtc `2026-10-27T07:02:38.5104572Z`（晚于测试源码 4.31 s，新鲜） |
| fixture 发现（只读反射） | `[UnityTest] = 16`；目标 FQN 唯一存在、无参；`PointerDerivedBuildCandidate` / `WorldBuildingCount` 已编译进 `BattleStage1.PlayModeTests.dll` |
| 产品文件未改 | `HomeProductShell.cs 17549B / fbcb8594…573878`（与 §5 相同）；`HomeBuildingPlacement.cs d650c1f4…489335`、`ProductHomeWorldHost.cs 8e5dc3b1…ceab16`、`Assembly-CSharp.dll 649216B / 1fb4e9e3…d151` 均未变 |
| 34 项磁盘基线 | `total=34 / problems=0` |
| 受保护 TMP 字体（磁盘 / 内存） | `14961033B / 3c758e16…c404854`、9 子资产 `dirty=0`；未测量字模 |
| 单槽存档 | `13405B / 4b2b56f6…d6aa`、`.tmp=False`、mtimeUtc `2026-10-27T02:49:56.6790499Z`（上一次运行收尾恢复，本轮零写盘） |
| guard | `manifests=276`：`restored=275 + protective-snapshot=1`、`other=0`（无 unfinished / 无待人工项）；本轮未新增 run |
| 场景 / PrefabStage / Editor | `SampleScene` loaded、`isDirty=False`、`PrefabStage=null`；`playing=false / compiling=false / updating=false` |

本轮禁止项均未触碰：未 `SaveAssets / SaveChanges / ClearDirty / SetDirty`，未 `rql-mutate`，未 PrefabStage，未字体探针（`GetPreferredValues` / `ForceMeshUpdate`），未 Preferences，未 Shell / Git / 正式工程，未启停 Editor，未构建 Player，未 `tests-run`（未跑整类、未跑全集）。

### 9.5 交宿主执行口径（只跑新单例，与 §7 相同）

```text
testMode: PlayMode
testClass: RRPG.ManagementShell.Tests.ManagementShellPlayTests
testMethod: RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate
```

- 期望：fixture 共 16 个 `[UnityTest]`，本次只执行 1 例；通过标准 `1/1 Passed、0 skipped`。
- 新证据行顺序预期为：`PRE / MANAGEMENT-CLOSED / NEGATIVE-FRAMES=3 / PERSON-CLOSED / BUILD-POINTER-NEGATIVE / BUILD-NEGATIVE / BUILD-GATE(pointerStable=…) / END / CLEANUP`。
- 若失败：完整保留 message / 堆栈 / 证据行，区分环境前置（真实指针压在 UI、指针换算恰好等于手动候选、OS 指针在采样间移动）与产品断言失败；**不改写规则、不降低断言、不把失败改成通过**。

### 9.6 诚实边界（不冒称）

- 仍未覆盖 OS 物理 ESC / 物理鼠标 / IME / 多指针时序；注入仍是合成“一帧按下”采样，不证明 `Input.GetKeyDown` 引擎实现。
- 建造自身的物理 ESC / 右键取消（`HomeBuildingPlacement.ReadInput`）本用例不注入；本包只证明 Shell 路由在 `Build.IsOpen` 时让路。
- 本轮 headless 环境的 `Input.mousePosition` 是合成大值，故“候选落在建造范围外、`PreviewValid` 在真实帧后可能为 false”属正常结果，不作为 ESC 结论；`PreviewValid` 只在 `SetCandidate((0,4))` 后、真实帧前断言。
- 本次通过仍以宿主阶段 check 单例回执为准；整类 16 例是否重跑由主开发另行决定，不因本附录升级 R3-B 状态或 requiredChecks。

> 本文件与整包结论仍须由**主开发专业核验**，不代表制作人批准。
