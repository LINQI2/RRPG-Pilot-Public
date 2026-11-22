# R3-B 普通可测剩余：双 FunctionSlot 真实 UI / SaveLoad + 建造态 ESC 回调顺序机制

- 日期：2026-10-27
- 目录（唯一授权）：`D:\AI_Agent_Lab\rrpg_pilot\repo`
- 阶段：1/1「双槽 UI 保存续读及建造 ESC 机制专项」
- 本阶段新增文件：
  1. `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（**只追加** 1 个 `[UnityTest]` 与私有 helper；原 14 例强断言零改动）
  2. 本文件
- 未修改任何产品源码、Prefab、StaticConfig 生产配置、Editor 保护链；未新增 Shell / Build 输入缝；未改 Authority / private 字段 / public API。
- 未执行 `tests-run`（按指令由 Studio 宿主以单方法运行）；未使用 `rql-mutate` / `SaveAssets` / `SaveChanges` / `ClearDirty` / `SetDirty` / PrefabStage / Git / Prefs / 正式工程；未做字体幅度预检测量；未新增 1280 分辨率用例（已取消）。

## 1. 开工基线核对（只读）

| 项 | 核对方式 | 结果 |
| --- | --- | --- |
| Pilot 空闲 | `editor \| select isPlaying isPaused isCompiling` | `false / false / false` |
| 34 项 workspace 基线 | 读取 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` | 存在，`root` 指向本仓库，`files` 34 项（由既有 `OneTimeSetUp/OneTimeTearDown` 继续强校验） |
| 受保护 TMP 字体 | 目录存在性（**不做哈希预检测量**） | `Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` 存在 |
| 单槽 guard 前置 | 扫描 `Development/StudioRuns/R3B-SaveProtection-20261024/*/manifest.json` 的 `status` | 263 条记录中仅 1 条 `protective-snapshot`，其余 `restored`，无 `captured` / `restore-failed`；最新 run `20261027T022038Z` 为 `restored` → 不触发 `BlockOnUnfinishedRuns` |
| 默认建筑注册表 | 只读 `StaticConfigRegistry.TryGetBuildings` | `defs=4`，bindings = `building_farm:main:ws_farm_01 \| building_kitchen:main:ws_kitchen_01 \| building_workshop:main:ws_test_01`（无 aux，无预制 Completed） |

## 2. 新增用例

`ManagementShellPlayTests.ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts`

编译后反射核对：`BattleStage1.PlayModeTests` 中 `UnityTest` 方法总数 = **15**（原 14 + 新 1），原 14 个方法名逐项未变。

### 2.1 逐步真实入口（全部为产品公开入口，不造状态）

1. **内存双 slot Registry（仅 Editor 测试缝）**
   - 经 `StaticConfigRegistry` 的 Editor-only `internal` 缝（`SetBuildingsOverrideForTests` / `GetBuildingsOverrideForTests`，反射调用）注入一份**内存** `BuildingRegistry`：真实定义目录 + 在末尾追加 `building_farm:aux:Workplace:ws_test_02`（与第 19 个 Edit 用例同口径）。
   - 断言：覆盖真实生效（`AreSame`）、不改写真实定义目录与前序 bindings（前缀一致 + 后缀 aux）、不写盘。
   - 容量来自真实 profile：`ws_farm_01` Capacity 2，`ws_test_02` Capacity 1（源码 `VillageResourceCapacityWorkplaceDefs` / `VillageTestWorkplaces`，未复制数值）。
2. **真实 place**：既有公开建造入口 `Build.OpenCatalog → Choose("building_farm") → SetCandidate → Confirm → Cancel`（复用文件内既有 `PlaceBuilding` 辅助），断言 `UnderConstruction`。
3. **真实工人**：底部导航「管理」→ 职责与场所页 → 真实点击施工行 → 真实点击「加入施工」按钮两次（workerA / workerB），断言活动项目参与者集合等价、进度仍 0。
4. **真实推进完工**：`Shell.HandleEscape` 回 World → 真实 `Clock.AdvanceTo(14400)` → 断言建筑 `Completed`、无残留活动项目。两个 Workplace 由 `FinalizeConstruction` 按真实 `FunctionBindings` **自动注册**（未手工写 Completed / assignment / arrival）。
5. **两行独立**：重新真实导航进入管理 → 逐 `BuildingInstanceId + FunctionSlotId` 取行：
   - `workplace:<instance>:main` 与 `workplace:<instance>:aux` 互不相等；
   - 两行 `IsRegistered=true`、`Capacity=2/1`、`BoundCount=0`；
   - 同一建筑实例下恰好 2 行（不按建筑合并）。
6. **真实点击两行、派不同人**：mainOwner → main（`assignButton` 真实点击）、auxOwner → aux；断言职责写入、名额 1/1、且 aux 派工不改变 main 名额。
7. **满员两条真实路径**：
   - Gateway 真实命令 `ManagementCommandGateway.AssignToWorkplace(workerB, aux)` → `DomainRejected` + `DomainDiagnostic=CapacityFull`，零写入；
   - 真实 UI 把 lateJoiner 派入 main（Capacity=2 填满）；
   - Gateway 真实命令 `AssignToWorkplace(workerB, main)` → `DomainRejected` + `CapacityFull`，main 名额 2 不变、aux 名额 1 不变、workerB 无职责；
   - **原 UI 满员禁用仍保留**：选中 workerB + 点击满员 main 行后 `assignButton.interactable=false`，对禁用按钮做真实指针点击后仍零写入。
8. **劳动 / 现场 / 证据按 slot 独立**：回 World → `AdvanceTo(16360)`（次日午前，等值于既有 CASE-C 的推进窗口）：
   - main `BoundCount=2 / LaborCount=2 / OnsiteCount=2`；aux `1 / 1 / 1`；
   - `WorkplaceEvidenceMinutes` 逐 (人, 岗位)：mainOwner/main、lateJoiner/main、auxOwner/aux 均 > 0；`mainOwner/aux`、`auxOwner/main` 均为 0；
   - 三人职责仍各自指向原 slot。
9. **显式 Save → 真实 Load → 重新进入管理**：
   - 管理页真实点击保存按钮 → 状态文案「已保存当前世界。」+ `ManagementView.Save()` bool 回执 true + `SaveSystem.HasSave`；存档 JSON 含 `SaveVersion: 1` 与资源事实，且不含 UI 候选字段。
   - 记录保存前的两 slot 事实、资源、Clock、生产批次 / 证据 / 执行历史计数。
   - 真实产品路径 `SampleScene` 重建 → `MainMenuUI.OnContinue`（真实按钮 `onClick.Invoke()`）→ `SaveSystem.Load` → `TownUI`/`ProductHostWorldHost`/`HomeProductShell` 重建：断言新 host 实例、`SimulationPersonSource` 与 `TownUI.PlayerData` 同一引用（来源同步）、`Paused`、Load 自身不推进 Clock。
   - 重新真实导航进入管理页：UI 候选与检索会话为空；两 slot 的 ID / 实例 / slot / 容量 / 注册 / 绑定人物集合 / 逐人职责相等；`Completed` 保留；资源 / Clock / 批次 / 证据 / 执行历史逐项相等；刷新管理页后资源与批次不再变化（无重复生产）。
10. **建造态 ESC 回调 / 取消顺序机制**（在 Load 后的新 Shell 上）：
    - 前置 `Surface=Management`；公开 `Build.OpenCatalog()` 打开候选 → `IsOpen=true`；公开 `Choose("building_farm")` → 候选与 `GhostVisible=true`；
    - 真实产品顺序记录：`OpenCatalog` 自身复用 `Shell.CloseSurface()` 收起管理表面（`Surface=World`），管理页未激活；
    - 调用 `Shell.HandleEscape()`：`Build.IsOpen` 仍为 true、`SelectedDefinitionId` 仍为 `building_farm`、`Surface` 保持 `World`、管理页未被重新激活 → 证明回调先判 `Build.IsOpen`，**不进入 `CloseSurface` 分支**；
    - 公开 `Build.Cancel()`：`IsOpen=false`、候选清空、ghost 隐藏；
    - 再用真实导航打开管理（`Surface=Management`）后调用 `Shell.HandleEscape()`：`Surface=World` 且管理页不再激活 → 建造关闭时 ESC 才关闭**当前 Surface**。

### 2.2 恢复 / 保护链（均不被短路）

- **本用例 `finally`**：只 `SetBuildingsOverrideForTests(previousOverride)`（开工前为 null），不 `Reset` 生产 Root、不触碰存档 / 字体 / 34 项。`finally` 之后只读复核：覆盖为 null、默认注册表指纹与开工完全一致、默认注册表不含 aux 测试绑定。
- **活动日程**：夹具用既有公开注入点 `VillageRuntimeBridge.ConfigureActivitySchedule` 排除本例时间跨度内的机会点（与 CASE-B 同理由、同恢复链），`TearDown` 的 `RestoreActivityScheduleAfterFixture` 在 `ResetForTests` 之后按同一实例 + 机会点 + 通勤 + 重复延迟 + 五项时长只读校验恢复。
- **单槽存档**：`SaveSlotGuard.Capture`（UnitySetUp）→ 本用例写盘 → `TearDown` 的 `try/finally` 按本 runID 已验 backup 恢复并回读 SHA256；`OneTimeTearDown` 兜底同 runID 恢复。本用例 `finally` 仅恢复配置，不提前返回、不吞异常，故不会短路 guard。
- **34 项 / 字体**：`OneTimeSetUp` / `OneTimeTearDown` 照旧（本用例不写这些文件）。

## 3. 实际执行证据（本阶段真实做过的验证）

1. `script-execute` 调用 `AssetDatabase.Refresh(ForceUpdate)`（两次，第二次在 evidence 标签改名后）。
2. 编译后 `console | where logType == "Error" or "Exception"`：仅剩 3 条**来自本 Agent 早前 snippet** 的错误（`snippet.cs(6,92): error CS0234 NUnit.Framework.UnityTestAttribute`，即我自己写错的验证片段），**无任何项目源码 / 测试程序集编译错误**。
3. 程序集反射核对：`BattleStage1.PlayModeTests` → `RRPG.ManagementShell.Tests.ManagementShellPlayTests` 存在新方法；`UnityTest` 方法数 = 15；`WorkplaceIdForSlot` / `RegistryWithAuxSlot` 等 helper 均可见。
4. 双 slot 注册表 helper 等价演练（Editor 内、`try/finally` 后立即清除覆盖）：
   - `same=True startsWithBaseline=True endsWithAux=True`
   - `twoSlotShape=building_farm/农田;building_house/房屋;building_kitchen/厨房;building_workshop/工坊||building_farm:main:Workplace:ws_farm_01;building_kitchen:main:Workplace:ws_kitchen_01;building_workshop:main:Workplace:ws_test_01;building_farm:aux:Workplace:ws_test_02`
   - `overrideAfterClear=True restoredEqualsBaseline=True`（演练后覆盖已确认为 null，未污染后续测试基线）。

## 4. 明确的边界与仍未证明项（不冒称）

1. **Play 用例本身尚未由本阶段执行**：按指令模型不跑 `tests-run`；结果由 Studio 宿主单方法运行后回填。本文只声明编译、程序集反射与 helper 等价演练证据，不声明测试通过。
2. **Player 编译仅静态边界**：双 slot 依赖 `#if UNITY_EDITOR` 的 `internal` 测试缝，Player 中该方法与覆盖分支不存在；本用例不证明 Player 运行时可用。
3. **物理 ESC / Input 路径未测**：建造态验证的是 `Shell.HandleEscape()` / `Build.Cancel()` **回调与取消顺序**，不是物理按键；`HomeBuildingPlacement.ReadInput`（`Input.GetKeyDown(Escape)` / `Input.GetMouseButtonDown`）与 `HomeProductShell.LateUpdate` 的真实按键分支未经真实输入驱动。鼠标同样为程序化 `EventSystem.RaycastAll + ExecuteEvents.PointerClick`，不等价物理指针体验。
4. **`Build.IsOpen` 且 `Surface≠World` 的公开组合不存在**：`Build.OpenCatalog()` 内部复用 `Shell.CloseSurface()`，故公开入口下无法构造该组合。用例因此只证明「`Build.IsOpen` 时 `HandleEscape` 不进入 `CloseSurface`（Surface 保持调用前状态、建造与候选不被取消）」，并在代码注释与本文件中明示；「某个非 World 表面在建造态被保留」未证明、也未声称。
5. **劳动 / 现场断言的时序前提**：main / aux 的 `LaborCount` / `OnsiteCount` / 逐人 Evidence 成立依赖「派工发生在 420 分钟通勤登记边界之前」（本例在 14400 整点派工），以及推进到 16360 时已跨过 480 工作边界；若产品改动通勤 / 边界登记规则，需按新契约重新核对（本用例不做替身或镜像规则）。
6. **资源联动**：main 使用真实 profile `ws_farm_01`（Farm 资源岗位），会参与真实资源结算；用例只比较 Save / Load 前后一致性（Ingredients / Food / 批次 / 证据 / 历史），不声称资源数值口径。
7. **不证明**：卡牌 / 远征 / 成长 pending 等其它结算路径与双 slot 的交互；也不新增 1280 分辨率用例（已取消）。

## 5. 复跑方式（交 Studio 宿主，单方法）

```
testMode: PlayMode
testMethod: RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts
```

- 期望：`Passed`，`Skipped=0`。
- 失败排查顺序（证据日志 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log` 中 `CASE-E` 段）：
  `OVERRIDE` → `PLACE` → `SLOTS` → `ASSIGN` → `FULL` → `LABOR` → `SAVE` → `CONTINUE` → `ESC-BUILD` → `ESC-CLOSE` → `END`。
- 若 `SAVE-BACKUP` 出现 `captured` 且用例中断：`Development/StudioRuns/R3B-SaveProtection-20261024/<runId>/` 保留 `original-save.bin` + `manifest.json`，按既有 guard 人工核对，不自动清理。
