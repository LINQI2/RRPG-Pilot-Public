# R3B-UI 真实产品入口与集中管理模态 Play 专项报告（2026-10-23）

状态：**阶段 1/1 测试代码已编写并编译通过；PlayMode 用例未由 DSH 运行，等待宿主 Studio 阶段 check 串行执行。本报告不预填宿主 PASS。**

本包只做一件事：在既有的真实产品入口 / Person Play 基座上，新增最小长期回归 `ManagementShellPlayTests`（3 个 `[UnityTest]`），验证集中管理 authored Prefab 的真实激活、行克隆、姓名过滤、人物详情往返、模态 Raycast 拦截、TopHUD 时钟控制、关闭/ESC 与旧 Town 输入恢复，并加入单槽存档字节保护与 34 项基线 / 受保护字体哈希证据。**未重写管理 UI、未改 Scene、未改产品源码、未跑 tests-run、未进入 Play、未宣称任何运行 PASS。**

## 1. 授权范围、环境与边界（实际回执）

- 唯一工作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`；唯一 Unity 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`。
- Unity 连接：`editor | select unityVersion projectPath isPlaying isPaused isCompiling isUpdating`
  → `2022.3.62t9 | D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game | false | false | false | false`；`selection=[]`。
- 场景：`SampleScene | Assets/Scenes/SampleScene.scene | isDirty=false | rootCount=5`；**本包未写场景 / 未保存场景 / 未改 SampleScene**。
- 分支与 HEAD：沿用任务给定 `ai/rrpg-r3b-population-pilot` / `054b00d3e38fe2e2e142bfdb2502de90f7991030`（基线 JSON 记录一致）。本包未执行任何 git 写操作（无 stage / commit / push / 切分支 / 改远程）。
- 未 Shell、未 `Process.Start`、未安装组件、未改 ACL/权限、未访问未授权工程或认证目录、未动 Packages / 插件 / 正式工程、未创建额外付费子 Agent。
- Unity 只连接用户已开的 Pilot 2022.3.62t9，未启动 / 未关闭 Editor。
- 未用 `script-execute` 启动测试；`script-execute` 仅用于只读检查（Prefab 结构、DLL 新鲜度、基线 / 文件哈希、类型存在性）。**未执行 `tests-run`**。
- 本包未改动 `HomeProductShell.cs`、`ManagementView.cs`、`ManagementRowView.cs`、`ManagementView.prefab`、`ProductHomeWorldHost.cs`、Work/Clock/公共核心 API、Save DTO、Authority 或任何 Prefab/Scene/YAML/字体/ProjectSettings。

## 2. 实际文件差异（新增 2 个文件，其余 0 改动）

| 文件 | 类型 | bytes | SHA256 |
|---|---|---:|---|
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 新增测试 | 38544 | `e9cc0c3a5664925d8a7a683278c8c27bce0b0700e89ae2dbd567a0d1f3178381` |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs.meta` | Editor 生成 meta（未手写 YAML） | 267 | `9da27352b62ef02bed6e5ac640453677938d3a8fa885dcf896d5799713570404` |

未改动的关键前置文件回读（与 `shell-integration-report.md` / controller-report 一致）：

| 文件 | bytes | SHA256 |
|---|---:|---|
| `.../ProductHome/HomeProductShell.cs` | 15497 | `7e9901d7092fda62aea06a2f1a8fbe3bfe2c84974df08eab0002a09015c07fcd` |
| `.../ProductHome/ManagementView.cs` | 37518 | `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d` |
| `.../ProductHome/ManagementRowView.cs` | 1670 | `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd` |
| `Assets/Resources/UI/ManagementView.prefab` | 282097 | `ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11` |
| `.../ProductHome/ProductHomeWorldHost.cs` | 14911 | `8e5dc3b1bd8a47b62f1a2bc184138af237d4be910162467efec08b4b20ceab16` |

- `.meta` 由 Unity `AssetDatabase.Refresh` 生成，未手工编辑序列化内容。
- 除新增测试与 meta 外，本包未产生其他仓库文件差异；证据日志由宿主运行测试时由 fixture 写入（见 §5）。

## 3. 真实基座与测试装配（复用既有路径，不另造世界）

- 测试程序集：根 `Assets/Tests/PlayMode` 由既有 `BattleStage1.PlayModeTests.asmdef` 覆盖（与 `R3AGrowthTransactionPlayTests` 同一装配路径），引用 `BattleStage1.Runtime` / `Growth.Runtime` / `Unity.TextMeshPro`，**不引用 Assembly-CSharp 产品类型**。
- 因此测试沿用 R3A 既有反射公共通道：`Type.GetType("..., Assembly-CSharp")` 读取真实组件 / 调用产品公共方法（`OnContinue`、`OpenSurface`、`CloseSurface`、`HandleEscape`、`HandleWorldClick`、`SelectPerson` 等）；**仅读取私有序列化 UI 绑定字段，不反射写入私有核心状态**。
- 入口链：`SceneManager.LoadScene("SampleScene")` → 创建隔离存档 → `MainMenuUI.RefreshContinueButton` → **`MainMenuUI.OnContinue` → `SaveSystem.Load` → `EnterTown` → `TownUI.Initialize`** → `ProductHomeWorldHost.Awake` → `HomeProductShell.Initialize`。未用 `SetActive` 代替产品初始化。
- 世界：
  - `SaveSystem.NewGame()` + `SaveSystem.Save(...)`（断言 Save 返回 true，主入口失败不忽略）；随后 `OnContinue` 真实 `Load`。
  - `VillageRuntimeBridge.ResetForTests()` 作为既有测试初始化方式；共享 `UnitySetUp` 等待 `ShellVisible` / `WorldVisible` 后暂停 Clock，保证确定性。
- 管理入口：**在真实 Shell 底部导航“管理”按钮（`ProductNavigation/Navigate2`）上用 `EventSystem.RaycastAll` + `ExecuteEvents.PointerClick` 命中真实 Button**，不直接调用 `OpenSurface` 绕过导航。
- 所有点击均为 **EventSystem 程序化 PointerClick（走真实 `Button.onClick` / 事件）**，不是物理鼠标人工体验；指针手感、拖拽、滚轮留后续专项 / 人工验收。

## 4. 三个 `[UnityTest]` 用例（入口 / 断言）

### 用例 1：`Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip`

- 入口：真实 `OnContinue` → Town → 世界选中一位人物 → 真实点击底部导航“管理”。
- 断言：
  - `Shell.Management` 为真实 authored Prefab 实例且 GameObject 名为 `ManagementView`；`Router.Surface==Management`；`IsVisible`；`SurfaceModalActive` 与旧 `IsPersonModalOpen` 别名均为 true。
  - 白模 `HomeInternalWhitebox` 已退出（inactive），但仍保留（未删除其他分支）。
  - 固定三页签全部 active；默认人口页；**39 个 `[SerializeField]` 对象引用计数 = 39、null = 0**。
  - 人口行克隆数量 = 真实快照人口数，且每个克隆行 active；真实点击“职责与场所”页签切换、再切回人口。
  - 姓名过滤输入（`TMP_InputField.text` → 真实 `onValueChanged`）触发：0 行 + 空提示 + 汇总文本变化；清空后恢复全部行。
  - 真实 Raycast 点击人口首行 → 进入既有 `PersonDetail`，`Selection.Id` / `DisplayedPersonId` 均等于该行同一 `PersonId`；管理页隐藏。
  - 真实 Raycast 点击 PersonDetail 关闭 → World，Selection 保留同一 PersonId；重新点击“管理” → `Management.SelectedPersonId` 仍为该人。
  - 全程 `VillageRuntimeBridge.IsSimulationActive==true`（正常浏览不关闭模拟）。

### 用例 2：`ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock`

- 入口：真实 Town 入口 → 选中人物 → 真实导航“管理”（Clock 由夹具暂停）。
- 断言：
  - `ModalBlocker` 左侧留白背景点真实 Raycast 的 top hit 属于 Blocker/面板，且无可点击 handler（不穿透 World/Town）。
  - 调用真实 `HandleWorldClick(背景点)` 后 Surface 仍为 Management、Selection 不变。
  - 底部导航“建造”（`Navigate6`）位置真实 Raycast 被管理模态覆盖、无可点击 handler；`Build.IsOpen==false`；Surface 不变。
  - TopHUD `Pause` / `CurrentSpeed` 中心点真实 Raycast top hit 为对应真实 Button，且 `interactable==true`（未被管理模态禁用）。
  - 真实点击 Pause → Clock 从 Paused 恢复；真实点击 Speed → Clock 速度倍率上升；真实点击 Pause → Clock 暂停。
  - 手动 Pause 后真实点击“资源”页签、`HandleEscape` 关闭、再真实导航重开管理：Clock 始终保持 Paused；`IsSimulationActive` 始终 true；Selection 保留。

### 用例 3：`CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`

- 入口：真实 Town 入口 → 选中人物 → 真实导航“管理”。
- 断言：
  - 管理模态时 `TownUI.canvasRoot` 的既有 `CanvasGroup.interactable/blocksRaycasts` 均被禁用（复用 Shell 既有 `SetTownInputBlocked`，不新建第二套 Guard）。
  - 真实 Raycast 点击管理 `Close` → World，Selection 保留；Town CanvasGroup 恢复进入前取值。
  - 重新真实导航打开管理 → 再次禁用旧 Town；调用 `Shell.HandleEscape` → 与关闭按钮相同的 World Surface / 同一 Selection；Town CanvasGroup 再次恢复。
  - `Build.IsOpen==false`，`IsSimulationActive` 不变。

> 说明：导航 busy / 远征 gating / 派遣施工保存连续性 / Simulation Return gating 属下一小包，本包未塞入；镜头 Pan/Zoom 输入如无安全注入能力，本包如实未造验证。

## 5. 存档保护策略与证据输出位置

### 5.1 单槽存档

- 来源：`SaveSystem.GetSavePath()`（即当前 Editor 的实际 `Application.persistentDataPath/rrpg_save_p0.json`）；**只读写这一个单槽文件，不扩展读取任何其他外部目录，不用 R2/R3A 历史档覆盖**。
- 每个用例 `[UnitySetUp]` 第一步：记录该槽位是否存在、原始字节、字节数、SHA256（不存在记 `<absent>`），写入证据日志。
- 每例 `[TearDown]` 的 `try/finally`：按原字节恢复；原本不存在则只删除本轮创建的档；恢复后重算 SHA256 并断言与 `[UnitySetUp]` 记录一致（不存在则必须仍不存在）。
- 测试本身通过 `NewGame` + `Save` 创建隔离世界并断言 Save 成功，再用 `OnContinue` 真实 Load；不忽略主入口 Save 失败。
- 未新增 / 未改动任何 Save DTO、Authority、SaveVersion。

### 5.2 34 项保护基线与字体

- `[OneTimeSetUp]`：读取 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`（断言 `files.Length==34`、`root` 等于当前仓库根），逐项重算 bytes + SHA256，断言与基线一致；记录受保护 TMP 字体 `RRPG_SourceHanSansCN_Regular_TMP.asset` SHA256。
- `[OneTimeTearDown]`：重算 34 项与字体 SHA256，断言 0 diff / 0 missing、字体未变；结果写入证据日志。
- 若 UI 动态字形渲染触发字体资产 dirty，仅记录哈希；**不写回受保护字体**，测试后由上述断言证明文件未变。

### 5.3 证据输出位置（宿主运行产生，非本次预填）

`D:\AI_Agent_Lab\rrpg_pilot\repo\Development\StudioRuns\R3B-UI-20261023\play-evidence\management-shell-play-evidence.log`

日志逐行格式：`UTC时间 [TAG] 内容`，包含：

- `SAVE-CAPTURE` / `SAVE-RESTORE`（路径、存在性、bytes、SHA256、matches-before）
- `FONT-PRE` / `FONT-POST`（字体 SHA256、unchanged）
- `BASELINE-PRE` / `BASELINE-POST`（total / same / changed / diff / missing、逐项问题）
- `ENTRY`（真实入口到达 World）
- 每次程序化点击的 `Debug.Log`（标注“非物理鼠标人工体验”）

宿主运行前本报告只声明输出位置，不预填 `BASELINE-POST` 或 PASS 结果。

## 6. 编译 / 装配 / 测试发现回执（实际）

1. `editor | call RefreshAssets()` 在最终测试源码之后执行；`editor | select isCompiling isUpdating` → `false | false`。
2. `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` 写入 `2026-10-23T12:22:34.4776434Z`，测试源码写入 `2026-10-23T12:22:31.1404750Z` → **DLL_FRESH=True**。
3. 回读程序集类型：`RRPG.ManagementShell.Tests.ManagementShellPlayTests` 已加载；`tests | where fullName contains "ManagementShellPlayTests"` 返回 **3 个 PlayMode 测试**（assembly=`BattleStage1.PlayModeTests`）：
   - `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip`
   - `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock`
   - `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`
4. Console 说明（未清空 Console）：
   - `sequence 18247–18250` 的 4 条 `CS0120` 是本包首轮测试代码中 `Surface()` / `SelectionId()` 误写为 static 的**自身编译错误**，已修复；最终编译后 `sequence > 18778` 的 Error/Exception 查询为 **0 条**。
   - `sequence 18773–18778` 是执行器 `script-execute` 只读检查脚本自身的编译失败（`dynamic` / `NUnit` 引用），不是项目源码编译错误。
   - 本包未新增任何项目源码 Console Error/Exception；未清空 Console。

## 7. 测试前 34 项基线哈希（实测，全部 same）

- 校验时间：本报告写入前；`workspace-baseline.json` 固定 34 项、`root` 等于仓库根、HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`。
- 实测：**TOTAL=34 / SAME=34 / DIFF=0 / MISSING=0**。
- 制作人指定子集（TMP 字体 1 + Growth 12 + ProjectSettings 1）包含在下列 34 项中，均一致。

| # | 路径 | SHA256 |
|---:|---|---|
| 1 | `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |
| 2 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/.nuget-installed.json` | `3a62648db821facff9a132e0f14ab0f1fc89ea5216d89abfd7b38ddcd1b091d1` |
| 3 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/McpPlugin.Common.dll` | `a7a7f930a61e6c2badfc963510dd7672611d4c1f5e9a2c9ea02dde70d5cfaf9e` |
| 4 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/McpPlugin.Common.dll.meta` | `f834c878e2546517d3ef6620ea927ac272fdb989b816cfc02f03eb79caa2a5a1` |
| 5 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/McpPlugin.dll` | `3489756921aedcfdb79b0364edd4bee75dbe54c4e950a93082d185f03b7e9139` |
| 6 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/McpPlugin.dll.meta` | `9ee3c812123ae49b2a1192d009207c456161bdbef3d9b27ff5154e0f5a744155` |
| 7 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Microsoft.Extensions.DependencyInjection.Abstractions.dll.meta` | `29f71145f96de1ca326b7933969d22407ff1e5bde46020c45654565d985c2090` |
| 8 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Microsoft.Extensions.Logging.Abstractions.dll.meta` | `7168382ea37f03e565afd74d0e2ce880648d99b9c6a99989b9ddcb666710b589` |
| 9 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/ReflectorNet.dll` | `2d9ca3108e9eac9fa3ed9136649328c5f56c2f45c2f9a6cbf950aff2d85011b4` |
| 10 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/ReflectorNet.dll.meta` | `febfa2b8b7377cd03a8b724846f3a19fc4f8645a4951496256e57fab5a5389d4` |
| 11 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/System.Runtime.CompilerServices.Unsafe.dll.meta` | `6679267bad6fef39e59ab89839d695b8d08689f42a48febd801e336888faaafe` |
| 12 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Nodes/GrowthNode_Lv2_CardGrowth.asset` | `e574de08c1eadd6752a324f5bd4a5ea5a8965e6776b7ce35c43ba465e2d56a90` |
| 13 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Nodes/GrowthNode_Lv3_StarterSlotUnlock.asset` | `d840cfaf729e3d4a63cb4968b2282a554bcf3bf35b65f5ba34534a635a7d8b95` |
| 14 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Nodes/GrowthNode_Lv4_TraitUpgrade.asset` | `3a3a3bee19437fa3263eacf3f3e229d92fa6b898a71812a3f8906640634ca42c` |
| 15 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Nodes/GrowthNode_Lv5_UnlockedPoolExpand.asset` | `12940fab0d1dd0c41c2edaa652fe26298cc1d0b5ff5dcf7a3be64f8ad28f3001` |
| 16 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/TestGrowthCurve.asset` | `b0d918dd090f01f31994ef2303101e229cb804ddf5733a36051f6ba158833127` |
| 17 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/TestGrowthTable.asset` | `c119259d540f8186b6a0ff5f3e0ceed5a035a8832c528be4e8c15e55b905e139` |
| 18 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/TestTraitCatalog.asset` | `101c2c49783d6055d5f7f32a5d7fd5324b597fcc80676e6a94c9b671f4bd894f` |
| 19 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Traits/trait_char_001.asset` | `40f4703aa04ea08f62c96ef045f5f5b4db62f3addd73de3fe45deccf6c088568` |
| 20 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Traits/trait_char_002.asset` | `17b52c45dc93e04d3f9ec87dc53651b37d6bb32ea0c0a5a26b0e544c772adfd2` |
| 21 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Traits/trait_char_003.asset` | `4a44eff01fc7b16e460ee68ee46fbd25720dd427a8954f9663e9e39e2c094bfb` |
| 22 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Traits/trait_char_004.asset` | `41836550ce86568e419ba0050d87b1b8974547b29422f83cf710fc5dc81bef31` |
| 23 | `UnityProject/RRPG1.0game/Assets/Resources/ScriptableObjects/Growth/Traits/trait_hero_001.asset` | `fc5455b6db3085ab63db14b0488a7881123a3fb8863e89617ab7dd8bf9577974` |
| 24 | `UnityProject/RRPG1.0game/Packages/manifest.json` | `54b0a8418b8761fbc55fe7de21fd77343c19915f8a79d79dec872c8d1dbb97cd` |
| 25 | `UnityProject/RRPG1.0game/Packages/packages-lock.json` | `244a380c87f879fb2541467418731e46be410659cd0f94987abd86a5a8a93837` |
| 26 | `UnityProject/RRPG1.0game/ProjectSettings/ProjectSettings.asset` | `ca2848951b63101976e9984f89a462df0d64f8ab46f6f572a396ed6943e29280` |
| 27 | `Development/StudioRuns/R3B-tools-20261022/capability-check.md` | `9f8cda2014e871210af9de20c4f421dddc91bd043bda9de7a506df08606dbc4d` |
| 28 | `UNITY-r3b-toolchain-20261022.json` | `ff2f733d0a5865be93b2ff0f58082f89c075b1f6d4928586acbb53577aa9c230` |
| 29 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Rql.Contracts.dll` | `97ec341ff790e340bac5ff3e4a38e7b7669ea18a89716ed6e5f16717edb99b12` |
| 30 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Rql.Contracts.dll.meta` | `df214911db097564f502357d8a00e0ae6d07e7dee3cfab9bc14cc6190c8b5a73` |
| 31 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Rql.Execution.dll` | `82f1209e9380bc06212c903236799c76e6c5ce54abec4277839a3b82c36d7818` |
| 32 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Rql.Execution.dll.meta` | `4f105d81e2b07f526f1a22f4824bdbd9b17f2493a29d12d767525f964d5c7264` |
| 33 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Rql.Parser.dll` | `b6b6878845ed03766ec5e96e8cdb6af068c310fb38e853452ae121130c130de1` |
| 34 | `UnityProject/RRPG1.0game/Assets/Plugins/NuGet/Rql.Parser.dll.meta` | `9ea4b7cd54c8af7fef1a0c7ba9eabe7ca3002bfc2bae419a43d7be01d8945b5c` |

- 备份目录 `Assets/Editor/StudioRecoveryBackups/2026-10-23T11-49-56-028Z` 及 `.meta` 未触碰、未恢复旧备份、未覆盖 HEAD。
- 测试后的 34 项哈希由 fixture `[OneTimeTearDown]` 重算并写入 §5.3 证据日志；宿主运行前不预填结果。

## 8. 明确未测 / 留后续（不宣称 R3-B 完成）

1. 本包**未运行** `tests-run` / 未进入 Play；3 个用例的真实结果、Console 运行期警告、存档恢复与 34 项测试后哈希均由宿主 Studio 阶段 check 串行执行后给出。
2. 未做物理鼠标人工手感、拖拽、滚轮、镜头 Pan/Zoom 输入验证（本包未造不安全注入；如无安全注入能力留后续专项）。
3. 未做派遣 / 施工命令保存连续性、`Simulation Return` gating、导航 busy / 远征 gating、完整回归；属下一小包。
4. 未重新设计任何产品交互；未改管理 UI / Prefab / Scene / Work / Clock / 公共核心 API / Save DTO / Authority。
5. 本报告只证明测试代码已新增、编译通过、测试被 Test Runner 发现、34 项测试前保护一致；**不证明用例 PASS，也不代表 R3-B 完成**。最终 PASS/FAIL 以宿主的统一 Studio 阶段 check 回执与主开发专业核验为准。
