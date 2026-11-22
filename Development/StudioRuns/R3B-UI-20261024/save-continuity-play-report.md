# R3B-UI-20261024 · 保存连续性 Play 测试收尾报告

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（独立授权目录）
- 分支：`ai/rrpg-r3b-population-pilot`（未切换、未 commit/push/stage）
- Unity：用户已打开的 Pilot `2022.3.62t9`（本包只连接现有 Editor，未启动/关闭）
- 本包目标：核对上次 aborted 留下的保存连续性测试代码，完成编译收尾，报告实际完整性。
- 本包明确未做：未运行任何 Unity Test（未 `tests-run`、未通过反射启动测试）、未新写测试矩阵、未改产品实现 / Save DTO / Work / Clock / Authority / 序列化资产 / Scene、未读写用户存档、未使用 `rql-mutate`、未调用全局 `SaveAssets/SaveChanges/SaveDirtyScenes`。
- **结论摘要：新增 CASE-C 用例代码在语法与引用层面完整，无需修改源码；`AssetDatabase.Refresh` 后编译通过、Console 无 `error CS`/Error，测试发现列表含 6 个用例（原 5 + 新 1）。但 CASE-C 从未实际执行，运行时断言全部未验证，因此不预填 PASS。**

---

## 1. 当前测试文件与用例名单

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

- 主开发固定版本：**102647 字节，SHA256 `4610b0a2f24bd08193ec63c35e264904440918bb31325a876ac6a7c36357645b`**（此前 68385 字节 / `4bdf388…` 且 5/5 宿主 Passed）。
- 本包用只读方式读取该文件：**1721 行，类/方法/文件括号完整闭合**；本包**未写入、未覆盖、未重写该类**。
- 说明：本包工具边界内没有 shell/哈希命令，也未获准用 `script-execute` 做 Refresh 之外的代码执行，因此上述字节数/SHA256 采用主开发固定值，未在本包内独立重算；能独立证明的是“磁盘上的当前版本被 Unity 编译并发现为下列 6 个用例”。

Unity 只读测试发现结果（`tests | where fullName contains "ManagementShellPlayTests"`，**仅为发现，不是运行**）：

| # | 方法名 | 性质 |
|---|---|---|
| 1 | `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` | 原 5 例，未改 |
| 2 | `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` | 原 5 例，未改 |
| 3 | `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput` | 原 5 例，未改 |
| 4 | `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite` | 原 5 例，未改 |
| 5 | `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` | 原 5 例，未改 |
| 6 | `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` | **新增（CASE-C）** |

发现总数 = **6**，`PlayMode`；无重复方法、无遗漏的旧用例。旧 5 例的断言未被删除或放宽。

---

## 2. CASE-C 代码完整性核对（`ExplicitSaveContinueAndNewGame_…`，行 1288–1719）

新增用例按“真实产品入口 + 真实 Runtime Query”分为 7 段，代码层面均已落地：

| 段 | 行为 | 断言入口 / 保存与恢复路径 | 代码完整性 | 运行态 |
|---|---|---|---|---|
| 0 初始事实 | 隔离新世界基线 | `Ingredients()`/`Food()` ← `VillageRuntimeBridge.Resources`；`ProductionBatchCount()`/`ProductionEvidenceCount()` ← `IntradayWork.ToSaveData().Batches/.Evidence`；`AssignmentContributionHistory()` ← `Assignments.TotalTestContribution`；`ClockAbsoluteMinute()` ← `Time.CurrentInstant.Minute`；断言新世界 batches/evidence/completed 均为 0 | 完整 | 未执行 |
| 1 准备 | 真实建造 + 真实管理行/按钮建立动态岗位、施工参与、资源输入，再推进跨日 | `PlaceBuilding` → `Shell.Build.OpenCatalog/Choose/SetCandidate/Confirm`；`Bridge("TryAddConstructionWorker")`；`AdvanceTo(14400)` + `host.RefreshNow`；`BuildingState` ← `host.Snapshot.Building(id).LifecycleState`；`ClickWorkplaceRow`/`assignButton`、`ClickConstructionRow`/`addConstructionButton`；`ResourcesState().AddIngredients(2)`；ESC → `AdvanceTo(16360)` | 完整 | 未执行 |
| 2 保存前事实快照 | 捕获可辨别真实值 | `ActiveProject().ProjectId/Progress/LaborRemainder/ParticipantPersonIds`；Ingredients/Food；batches/evidence/completed；`WorkArrival(p1).Destination/ReadyMinute` ← `IntradayWork.ToSaveData().Arrivals`；`SaveSystem.GetSavePath()`；`File.Exists/HashFile`；`ReferenceEquals(SimulationPersonSource, TownUI.PlayerData)` | 完整 | 未执行 |
| 3 保存成功 | 真实管理 Save 按钮 + bool 回执 + 单槽写盘 | `management.saveButton` → `ManagementView.Save()` → `ManagementCommandGateway.SaveWorld()` → `SaveSystem.Save(PlayerCollection)`；状态文案断言 `已保存当前世界。`；`management.Save()` 必须返回 true；`SaveSystem.HasSave()`；文件 SHA256；JSON 含 `"SaveVersion": 1` 与 `"Ingredients":`；显式排除 UI 候选/检索 session 字段入档 | 完整 | 未执行 |
| 4 保存失败 | 短暂钩子失败：明确失败、内存不回滚、不写盘 | 反射取 `SaveSystem.ForceSaveFailureForTests`；`SetForceSaveFailure(() => true)`；`LogAssert.Expect(LogType.Warning, "Save forced to fail")`；`saveButton` 点击后状态含 `未保存` + `内存状态保留`；文件 hash 不变；Ingredients/Food/batches/evidence/completed/Progress/Clock/动态岗位/施工参与者逐项不变；`finally { SetForceSaveFailure(null); }` 复位后再真实保存成功并校验资源/进度不变 | 完整 | 未执行 |
| 5 MainMenu Continue | 场景重建 → 真实 Continue 按钮 → Load → EnterTown，新 host/shell、换源、事实连续 | `SceneManager.LoadScene("SampleScene")`；`MainMenuUI.RefreshContinueButton`；`btnContinue.interactable`；`btnContinue.onClick.Invoke()`（真实 Button 事件）；等待 `ProductHomeWorldHost`；新旧 host/shell InstanceID 不同；`SimulationPersonSource` 换源且等于 `TownUI.PlayerData`；Clock=`Paused` 且分钟数等于存档；存档 hash 不被 Load 改写；管理候选/检索/Selection 为空；动态岗位 ID、占用、施工项目 ID/参与者/Progress/LaborRemainder、p1 到场 Destination/ReadyMinute、资源/批次/证据/执行历史、管理施工行、`CurrentSnapshot.Resources` 全部连续 | 完整 | 未执行 |
| 6 MainMenu NewGame | 真实英雄选择 gating → 确认 → 新世界无旧实例/职责/候选泄露 | `DataManager.Instance.InitialPersonRoster.Count`（不写死人数）；`MainMenuUI.OnNewGame`；`HeroSelectionUI._heroCards`、`btnConfirm` 未选禁用 → 真实卡片 Button 点击启用 → 真实确认 Button 点击；`SaveSystem.HasSave()` 证明产品 Save 真写；新 Clock=0；新 town/host/shell；换源且等于 `TownUI.PlayerData`；旧 PersonId / 旧建筑实例 / 旧岗位职责不出现在新世界；管理刷新后 Roster 等于 Authority 配置、候选/检索/选择为空、施工行为空、资源投影与 Authority 一致、刷新不增减资源 | 完整 | 未执行 |

### 完整性判定

- **没有缺失括号、缺失引用、缺失 helper 或半成品方法**：6 个用例与所需 helper（`Ingredients`/`Food`/`ProductionBatchCount`/`ProductionEvidenceCount`/`AssignmentContributionHistory`/`ClockAbsoluteMinute`/`WorkplaceOccupants`/`Strings`/`ConfiguredInitialRosterCount`/`TotalPersonCount` 等）均有定义并被编译。
- 因此本包**未修改任何测试源码**；“补编译问题”实际结果为 0 处需要修补。
- 新用例只使用真实产品入口（MainMenu / HeroSelection 的真实 Button 事件、Management 的真实按钮、`SaveSystem.Load/Save/NewGame`）与只读 Runtime Query；**没有用 `SetActive` 代替产品初始化，没有用 DTO 存在性冒充 Authority 事实，没有反射写入私有核心状态**。
- CASE-C 的 UI 候选/检索只做“不得入档/不得恢复”的负向断言，不把 UI 选择当作实现事实。

---

## 3. 存档保护与恢复路径核对

- `[UnitySetUp] EnterTownThroughProductEntry`：每例开始先 `new SaveSlotGuard().Capture()`，按原字节备份 `persistentDataPath` 单槽并记录 SHA256；随后 `ResetForTests` → 场景重建 → `SaveSystem.NewGame()` + `Save()` → 真实 `MainMenuUI.OnContinue` 进入 World。
- `[TearDown]`：`try/finally` 三层——
  1. 先清 `SaveSystem.ForceSaveFailureForTests = null`；
  2. `finally` 中 `SaveSlotGuard.Restore()`：原存在则写回原字节，原不存在则只删除本轮创建的档；写回后重算 SHA256 并断言与捕获值一致；
  3. 最内层 `finally` 再 `VillageRuntimeBridge.ResetForTests()`（异常仅告警，不阻断恢复）。
- CASE-C 内部对失败钩子另有 `try/finally`，`SetForceSaveFailure(null)` 在离开失败段前必定复位；恢复成功后再次真实保存。
- 证据标签：`SAVE-CAPTURE` / `SAVE-RESTORE` / `BASELINE-PRE` / `BASELINE-POST` 写入 `Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`。
- **已核对的保护缺口（只记录，不越界修改）**：恢复只挂在 `[TearDown]`。若宿主在用例中途被原生 abort / Unity 被释放，`TearDown` 可能不执行，已捕获的单槽就无法由该 guard 写回。历史证据中已出现过“5 次 `SAVE-CAPTURE` 无任何 `SAVE-RESTORE`”的实例（见 §6）。本包不运行测试，故不新增 out-of-process 备份或改 helper 框架，留待下一有界包决定。

---

## 4. 反射调用签名核对（静态源码核对，非运行验证）

`T(string)` 固定 `Assembly-CSharp`；`Get/Static/Call` 使用 `Public|NonPublic|Instance|Static`，因此私有方法可被调用。逐项核对结果：

| 测试调用 | 实际产品成员（源码） | 结论 |
|---|---|---|
| `SaveSystem.GetSavePath()` / `HasSave()` / `Save(PlayerCollection)` / `NewGame()` / `Load()` | `Assets/Scripts/Save/SaveSystem.cs` 20 / 25 / 38 / 225 / 108 行，均为 `public static`，签名匹配 | 匹配 |
| `SaveSystem.ForceSaveFailureForTests` | 同文件 36 行 `public static System.Func<bool>` | 匹配（失败注入 seam 存在） |
| `MainMenuUI.RefreshContinueButton` / `OnContinue` / `OnNewGame` / `btnContinue` | `Assets/Scripts/UI/MainMenuUI.cs` 64 / 104 / 82 / 20 行；前两者为私有 `void`，反射 `NonPublic` 可调用；`btnContinue` 为 public `Button` | 匹配；真实产品路径 `OnContinue → SaveSystem.Load → TownUI.Initialize`、`OnNewGame → HeroSelectionUI.Show` |
| `HeroSelectionUI._heroCards` / `btnConfirm` / `Show()` / `OnConfirm` | `Assets/Scripts/UI/HeroSelectionUI.cs` 47 / 30 / 61 / 179 行；`_heroCards` 为 `List<GameObject>`；确认按钮未选英雄时 `interactable=false`，选卡后由产品事件置 true | 匹配；CASE-C 通过真实卡片/确认 Button 事件走 gating |
| `ManagementView.Save()` | `Assets/Scripts/Village/ProductHome/ManagementView.cs` 223 行 `public bool`；失败文案 `未保存：…当前内存状态保留…` 来自 `ManagementCommandGateway.cs` 126 行 | 匹配 |
| `ManagementView.SelectedPersonId` / `SelectedWorkplaceId` / `SelectedConstructionBuildingId` / `CurrentSnapshot` / `nameSearch` / `resourceSummary` / `statusMessage` / `saveButton` | 同文件 90 / 93 / 94 / 95 / 50 / 66 / 41 / 40 行；`nameSearch` 为 `TMP_InputField`，`resourceSummary/statusMessage` 为 `TMP_Text` | 匹配 |
| `DataManager.Instance` / `InitialPersonRoster` | `Assets/Scripts/Core/DataManager.cs` 21 / 52 行；`Instance` 静态属性、`InitialPersonRoster` 为 `List<PersonDefinition>` 属性 | 匹配 |
| `VillageRuntimeBridge.Time` / `Assignments` / `Projects` / `Resources` / `IntradayWork` / `SimulationPersonSource` / `IsSimulationActive` | `VillageRuntimeBridge.cs` 25 / 34 / 41 / 140 / 87 / 179 行；`IntradayWork` 在 `VillageRuntimeBridge.Work.cs` 7 行 | 匹配 |
| `TryAddConstructionWorker` / `GetPersonWorkplaceId` / `GetWorkplaceOccupantCount` | `VillageRuntimeBridge.Buildings.cs` 49 行；`VillageRuntimeBridge.cs` 1029 / 1035 行 | 匹配 |
| `IntradayWork.ToSaveData()` 的 `Batches` / `Evidence` / `Arrivals`，`Arrival.PersonId/Destination/ReadyMinute` | 既有 CASE-B/C 使用同一真实投影；`WorkArrival` 只读 | 匹配（编译通过） |
| `Assignments.TotalTestContribution` | `VillageAssignmentSystem.cs` 33 行 `public int { get; private set; }` | 匹配（只读属性） |
| `ManagementSnapshot` 行成员：`WorkplaceId` / `BuildingInstanceId` / `WorkerCount` / `Progress` / `PrimaryWorkplaceId` / `Resources.Ingredients/Food` | `Assets/Scripts/Village/Runtime/ManagementSnapshot.cs` 83/107/179 行等 | 匹配（编译通过） |
| `Clock.AdvanceTo(WorldInstant, WorldAdvanceOrigin, …)` / `Pause` / `Speed` / `SpeedMultiplier` | `VillageTime.cs` 82/38/10 行；`WorldInstant` 的构造与枚举从 Clock 所在程序集反射获取 | 匹配（旧例已在用；本包不运行） |

> 以上只证明“编译与签名存在”，**不证明运行时语义**。例如 Continue 后 `ClockSpeed()=="Paused"`、`ReadyMinute` 连续、NewGame 资源不归零等，均需下一包真实执行才能判定。

---

## 5. 编译与发现证据（本包实际执行）

1. 编译前 Editor 状态：`isPlaying=false, isPaused=false, isCompiling=false, isUpdating=false`；`SampleScene` `isDirty=false`（不需要保存，未调用任何全局保存）。
2. `script-execute` 仅执行安全刷新：`AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` → 返回 `refresh-requested`。
3. 编译状态观测：`isCompiling` 由 `true` → `false`（同一 Editor 会话，未启动/关闭 Unity）。
4. Console 只读回读：
   - `logType == "Error"`：**0 条**；
   - `message contains "error CS"` / `"Compilation"` / `"compiler"`：**0 条**；
   - 与 `ManagementShellPlayTests` 相关的 Warning：**0 条**（最近的测试日志仍是 05:09:01 的旧运行，未产生新运行日志）。
5. 测试发现（只读）：`tests | where fullName contains "ManagementShellPlayTests"` → **6 条**，包含 `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak`。
6. 编译后 Editor 仍 `isPlaying=false`、`SampleScene.isDirty=false`；本包未触发 `rql-mutate`。

**编译结论：当前测试文件可编译、可被发现；没有需要修补的普通编译问题。**

---

## 6. 用户单槽存档状态核对（只读 Evidence，不读写用户档）

证据文件：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`（共 204 行）。

### 最近一次有效运行（5 例旧版本，非当前 6 例）

- `2026-10-24T05:08:57Z` `BASELINE-PRE total=34 diff=0 all-match`
- 05:08:57–05:09:01 共 5 次 `SAVE-CAPTURE`，路径均为
  `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`，
  每次捕获值 `bytes=13405, sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`；
  每次对应 `SAVE-RESTORE … matches-before=True`。
- 05:09:01 `FONT-POST unchanged=True`；`BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`。
- 该日志**没有任何 CASE-C / `[CASE-C]` 记录**，也没有第 6 次 `ENTRY`；因此当前 102647 字节版本的新用例**尚未执行过**。

### 上次 aborted 执行是否触及单槽

- 05:09:01 之后证据日志**再无任何 `BASELINE-PRE` / `SAVE-CAPTURE` / `ENTRY` 追加**。若 aborted 执行曾进入本 fixture，`[OneTimeSetUp]` 会先写 `BASELINE-PRE`、`[UnitySetUp]` 会先写 `SAVE-CAPTURE`；两者都不存在，故**没有证据表明 aborted 运行进入过测试夹具或触碰单槽**。
- 但“未触及”只能到此为止：本包**没有读取用户档**（遵守“不读写用户档”），所以当前文件是否仍为 `13405/4b2b56…` 标为 **unknown**，不假设已经 Restore。

### 更早的历史异常（必须保留的事实）

- `2026-10-23T15:53:02Z` 一轮：首个 `SAVE-CAPTURE` 为 `bytes=21075 sha256=650c45c4…`；随后 4 次捕获变为 `bytes=13405`（hash 依次 `33879c2a…`、`5b0f9952…`、`a5dc9d91…`、`d2a11ea1…`），**5 次捕获全部没有对应的 `SAVE-RESTORE`**，该轮以 `BASELINE-POST all-match` 结束。
- 05:01 与 05:08 两轮从 `13405/4b2b56…` 捕获并正常恢复；即“恢复”恢复到的是 15:53 轮结束时的状态，而不是最初的 `21075/650c45c4…`。
- 仅凭日志无法证明 15:53 轮漏恢复的具体原因（TearDown 未执行 / 写回异常 / 证据写入异常），但事实是：**该轮 5 个用例后单槽未回到轮前字节**。这与 §3 的“保护只在 TearDown”缺口一致，下一有界包应优先处理（不得用“已 Restore”掩盖）。

> 本包未运行产品流程、未读取/写入 `rrpg_save_p0.json`；所有现存数据保持原样。

---

## 7. 34 项 / 指定 14 项保护核对

- `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 由主开发固定，**本包未修改**（只读打开）。
- 最近证据：05:08:57 `BASELINE-PRE total=34 diff=0 all-match`；05:09:01 `BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`；同一轮受保护 TMP 字体 hash `3c758e16…` 前后一致。
- 主开发固定状态：**2026-10-24T05:20:18Z 34 项基线全部匹配**。
- 本包没有写任何产品/资源/Packages/字体/ProjectSettings/Growth 文件；唯一计划写入是本报告（不在 34 项基线内）。测试文件 `ManagementShellPlayTests.cs` 也不在 34 项基线内。
- 指定 **14 项**按已更正分类为：TMP 字体 1 + Growth 12（Nodes 4 + Curve 1 + Table 1 + Catalog 1 + traits 5）+ ProjectSettings 1；**不把 NuGet/Packages 混入 14 项**。本包未重算 14 项 SHA256（不运行测试、不越界执行哈希代码），只声明：未触碰这些文件，沿用主开发 05:20:18Z 固定状态与上述 05:09:01 证据。
- `StudioTestResourceScope.cs`、`GrowthDataInitializer.cs` 与既有测试修复均保留，未修改。

---

## 8. 未验证项与下一包建议（不预填 PASS）

### 下一包最小测试 filter

```text
PlayMode
RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak
```

建议先单跑 CASE-C（确认新用例真实可执行、暴露首个运行时断言），再跑整个 `ManagementShellPlayTests` 类做 6 例回归；跑前确认所有打开场景已保存（`SampleScene.isDirty=false`），跑后检查 `SAVE-CAPTURE/SAVE-RESTORE` 成对、`BASELINE-PRE/POST all-match`。不得由本报告代替测试回执。

### 运行态未验证清单（执行后才有结论）

1. 初始新世界 `batches/evidence/TotalTestContribution == 0`。
2. `AdvanceTo(14400)` 完成两栋建筑、`AdvanceTo(16360)` 后 Progress/Labor/资源/批次/证据/历史均大于 0 且可辨别。
3. 成功保存文案、`ManagementView.Save()` bool、单槽 JSON `SaveVersion:1`/`Ingredients`、UI session 字段不入档。
4. 失败钩子路径：文件不变、内存事实不回滚、状态文案含 `未保存`/`内存状态保留`、钩子复位后恢复保存成功。
5. Continue：新 host/shell、换源、Clock `Paused` 且分钟连续、到场 `ReadyMinute` 连续、动态岗位/占用/施工/资源/批次/证据/历史/manage 行连续、候选/检索为空。
6. NewGame：HeroSelection gating、产品 Save 真写、Clock 归 0、旧 PersonId/建筑/职责不泄露、Roster 等于 Authority、资源投影一致。
7. `SaveSystem.NewGame` 当前不重置 `VillageRuntimeBridge.Resources`（测试已按真实 Authority 取值并记录，不臆断归零）——这是已记录的核心边界，如需改变应另开产品包，不在测试包内越界。
8. TearDown 漏恢复风险：原生 abort/Unity 释放会绕过 TearDown；15:53 证据已出现一次未恢复实例。下一包应决定增加独立备份/预恢复策略，或在报告中继续如实标注单槽 unknown。

---

## 9. Artifact 证据

- 本报告：`Development/StudioRuns/R3B-UI-20261024/save-continuity-play-report.md`（本包唯一写入）。
- 测试源码：未改动；Unity 编译发现 6 用例，证明当前磁盘版本参与编译。
- 编译回执：`AssetDatabase.Refresh` 返回 `refresh-requested`；`isCompiling true→false`；Console Error/`error CS` 均为 0。
- 测试回执：**无**（本包未运行测试）；此前 5/5 Passed 只对应 68385 字节旧版本，不能代表当前 6 例。
- 34 项基线：未修改；证据与主开发固定状态如上。
