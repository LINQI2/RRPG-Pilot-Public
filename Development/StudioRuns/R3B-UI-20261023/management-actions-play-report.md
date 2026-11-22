# R3B-UI 管理派工 / 施工真实按钮 Play 小包报告（2026-10-23）

状态：**2 个新 `[UnityTest]` 已写入并编译通过、已被 Test Runner 发现（合计 5 例）；本包未运行 `tests-run`、未进入 Play，不预填任何 PASS。**
**保护前置当前不满足：34 项基线中受保护 TMP 字体实际 SHA256 与基线不一致（33/34 same，1 diff）。宿主一旦串行运行本类，会在 `[OneTimeSetUp]` 保护断言处直接失败，2 个新用例不会得到有效回执。该不一致发生在本次 `RefreshAssets` 编译刷新期间（文件 mtime 12:30:59Z），本包未主动写字体，也未自行放宽 / 跳过保护断言。阶段验证门禁因此阻塞，需主开发决定授权恢复后再重跑。**

## 1. 授权范围与环境

- 唯一目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`；唯一工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`。
- Unity：用户已开的 Pilot `2022.3.62t9`；`editor | select isPlaying isPaused isCompiling isUpdating` → `false|false|false|false`（编译完成后）。未启动 / 未关闭 Editor，未 Shell / `Process.Start`，未改 ACL / 权限，未访问未授权工程或认证目录，未执行 git 写操作。
- 未执行 `tests-run`；未用 `script-execute` 启动测试。`script-execute` 只用于只读哈希与文件存在性检查（其中一次因 `.unity` 路径写错产生执行器 Runtime 错误，已在 Console 清理核对中单独标注，不是项目源码编译错误）。
- 未改 Scene / Prefab / Work / Clock / 公共核心 API / Save DTO / Authority / 字体 / Packages / ProjectSettings。

## 2. 实际文件改动

| 文件 | 类型 | bytes | SHA256 |
|---|---|---:|---|
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 修改（仅新增辅助方法与 2 个用例） | 60481 | `ee859d86ca7cf344f02c2d51cfb0728ff6bdc87c9f05ff9996113f621e31c1c2` |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs.meta` | 未改 | 267 | `9da27352b62ef02bed6e5ac640453677938d3a8fa885dcf896d5799713570404` |

未受影响的关键文件回读（与 `shell-play-test-report.md` 一致）：

| 文件 | bytes | SHA256 |
|---|---:|---|
| `.../ProductHome/ManagementView.cs` | 37518 | `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d` |
| `.../ProductHome/ManagementCommandGateway.cs` | 13603 | `d912bfc9baea7c68f3aad3d77f606fcbd1e27912f0cf59710131a4bd5453cc5e` |
| `.../ProductHome/ManagementRowView.cs` | 1670 | `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd` |
| `.../ProductHome/HomeProductShell.cs` | 15497 | `7e9901d7092fda62aea06a2f1a8fbe3bfe2c84974df08eab0002a09015c07fcd` |
| `Assets/Resources/UI/ManagementView.prefab` | 282097 | `ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11` |
| `Assets/Scenes/SampleScene.scene` | 846770 | `570ab047004d8d3c9ff06f91f7aea18aad05b3aaff1e064ac1ca67795228c72c` |

## 3. 新增用例（复用原入口 / SaveSlotGuard / OneTime 保护，不另造世界）

入口仍为既有 `[UnitySetUp]`：`SampleScene → SaveSystem.NewGame → Save → MainMenuUI.OnContinue → SaveSystem.Load → TownUI.Initialize → ProductHomeWorldHost → HomeProductShell`，Clock 由夹具 Pause。所有管理命令均从真实底部导航进入管理，再在 **真实 `Button` 上执行 `EventSystem.RaycastAll` + `ExecuteEvents.PointerClick`**（程序化点击，不是物理鼠标人工体验）；未调用 UI 不存在的方法。

### 3.1 `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite`

覆盖：

- 用既有建造产品入口（`Build.OpenCatalog/Choose/SetCandidate/Confirm/Cancel`）创建两个**同类 Farm**实例，断言实例 ID 不同；用既有公开施工 API 加 worker，再真实 `Clock.AdvanceTo(14400)` 至两栋完工。
- 管理页“职责与场所”真实页签点击；真实点击对应岗位行选中 `workplace:<实例>:slot` 候选；真实点击 `assignButton` 派 p1、p2 到同一 Farm A，断言 A 名额占用 2、B 为 0。
- 满员（Capacity=2）：产品按钮 `assignButton.interactable==false`（`CanAssign=false` 前置拦截）；对禁用按钮仍做真实 Raycast 点击，断言 p3 无职责、A 占用不变 → **按钮层拒绝且不写入**。
- 真实点击 `removeButton` 撤 p1：断言 p1 无职责、A 占用 2→1（旧名额释放）、B 不受影响、状态文案含“已移除”。
- 真实点击 Farm B 行 + `assignButton` 派 p1 到 B：断言 p1 落到 B、A/B 岗位 ID 与建筑实例均独立、B 占用 1。
- 已派工再派 A：真实点击可交互的 `assignButton`，状态文案含“已有主要职责”，p1 仍为 B、A/B 占用均不变 → **AlreadyAssigned 失败不写**。

未宣称：满员时 `CapacityFull` 域诊断未通过按钮真正进入（UI 先禁用）；远征 / 快照消失 / Unavailable / WorldBusy 拒绝留给下一 gating 包。

### 3.2 `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`

覆盖：

- 真实建造一栋 Farm 后，先真实推进到 479 分钟：**0 worker** 时建筑仍 `UnderConstruction`、无活动施工项目、施工行 `Progress=0 / WorkerCount=0`（时间本身不伪造进度）。
- 通过既有公开 Assignment 入口给 p1 建立 `ws_farm_01` Primary Assignment；管理页真实点击施工行选中候选，真实点击 `addConstructionButton` 加入 p1、再选 p2 加入；断言施工参与者 2 人、`ws_farm_01` 占用仍为 1、p1 Primary Assignment 全程 `ws_farm_01`（**岗位占用 ≠ 施工人数**）。
- ESC 关闭管理后真实推进 480 / 481：断言 p1 `CurrentPlaceId=construction:<实例>`、`GetWorkExecutionLabel=项目劳动`、项目 `LaborRemainder>0`、`Progress>0`（**真实到场才贡献**）。
- 重开管理，真实点击 `removeConstructionButton` 撤 p1：断言参与者 2→1、p2 保留、Primary Assignment 仍 `ws_farm_01`、撤出当下 `GetWorkExecutionLabel != 项目劳动`（立即改写实际 WorkIntent）、岗位占用不变。
- 真实点击 `addConstructionButton` 重新加入：断言参与者回到 2、Primary Assignment 仍不变（**名额合法返还**）。
- 未宣称“撤出后下一边界不再贡献”“重新加入后立刻劳动”：前者需更长边界计量、后者需下一边界重新到场，均如实未测。

## 4. 存档保护与 34 项 / 字体证据（关键阻塞）

- 原类 `SaveSlotGuard` 未改：`[UnitySetUp]` 首步按当前 Editor 实际 `SaveSystem.GetSavePath()` 捕获，`[TearDown]` 的 `try/finally` 按原字节恢复；原本不存在则只删本轮创建档；`ForceSaveFailureForTests` 在 TearDown 先行复位。
- 本轮实测当前槽位：`1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot\rrpg_save_p0.json`，`existed=true`，当前 SHA256 `650c45c4d1226ab5606073cd9210545438b37482d2b5de47559a2ca6ecb06ee5`（与历史 `650c45…` 一致；运行期前后 SHA 由 fixture 写入证据日志）。
- 34 项基线只读复核：**total=34 / same=33 / diff=1**。
  - 唯一差异：受保护 TMP 字体 `Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`
  - 基线期望：bytes `14961033`，SHA256 `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`
  - 当前实际：bytes `14968490`，SHA256 `e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8`
  - 当前 mtime：`2026-10-23T12:30:59.0431402Z`；该时刻落在本包测试源码写入（12:30:54Z）与测试 DLL 写入（12:31:00Z）之间，即本次 `RefreshAssets` / 脚本重载期间。本包未对字体执行任何写操作，也未手工编辑 YAML。
- 影响：`[OneTimeSetUp]` 的 34 项断言会在任何新用例前失败；`[OneTimeTearDown]` 也会再次报同一差异。按“不得放宽断言 / skip / 假数”和“不得改字体”的边界，本包**没有**改写基线、没有写回字体、没有用旧备份或 HEAD 覆盖，也没有继续运行测试来制造回执。
- 证据日志（宿主运行时由 fixture 写入，当前未预填 PASS）：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`。本轮运行前该日志中的历史 3 例记录与 `SaveSlotGuard` 恢复 SHA 不会被覆盖，仅由新运行追加 `SAVE-CAPTURE` / `SAVE-RESTORE` / `FONT-PRE` / `FONT-POST` / `BASELINE-PRE` / `BASELINE-POST` / `CASE-A` / `CASE-B` / `ADVANCE` / `BUILD`。

## 5. 编译与 Test Runner 发现回执

- `editor | call RefreshAssets()` 后 `editor | select isCompiling isUpdating` → `false|false`。
- `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` mtime `2026-10-23T12:31:00.9037914Z`，晚于测试源码 mtime `2026-10-23T12:30:54.6427233Z` → **DLL_FRESH=True**。
- `tests | where fullName contains "ManagementShellPlayTests"` → **5 个 PlayMode 用例，assembly=`BattleStage1.PlayModeTests`**：
  1. `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip`
  2. `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock`
  3. `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput`
  4. `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite`
  5. `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor`
- Console：项目源码最终编译后无新增 Error/Exception；`sequence 19856–19857` 两条 Error 是本包只读哈希检查脚本把 `SampleScene.unity` 路径写错导致的执行器 Runtime `FileNotFoundException`，不是项目源码或测试代码编译错误，已改用 `.scene` 完成只读复核。

## 6. 明确未测 / 留后续（不宣称 R3-B 完成）

1. **本包未运行测试**；且当前字体保护前置已失败，宿主运行会在 `OneTimeSetUp` 失败，2 个新用例的 PASS/FAIL 尚未获得真实回执。不得将本报告当作通过证据。
2. 用例 C（显式 Save / Continue / NewGame 连续性）本包**未写**：库存 Ingredients/Food、具体 workplace/instance/slot、施工进度与重复生产/历史、NewGame 无旧世界残留、UI session 不入档，全部留“保存连续性 / NewGame 缺项”独立包，明确未测。
3. 远征 / 人物从快照消失 / Unavailable / WorldBusy / 劳动中撤职的分钟贡献与下一边界清理、满员 `CapacityFull` 域诊断、回归完整矩阵均未测。
4. 未做视觉重设计、未全量回归、未改 Work/Clock 核心、未新增产品规则或原子换岗。
5. 下一包：先由主开发决定受保护字体差异的授权处置（本包不自写字体 / 不改基线），再重跑 `ManagementShellPlayTests`；随后补保存连续性 / NewGame、剩余 gating、最小长期回归与日期化文档最终自审。

## 7. 宿主 check 入口

PlayMode 短类名：`ManagementShellPlayTests`。本包到此结束，等待 Studio 宿主真实回执；在字体保护差异获授权处理前，预期回执为保护夹具失败而非用例业务失败。
