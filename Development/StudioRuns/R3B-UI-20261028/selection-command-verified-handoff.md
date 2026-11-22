# 选人 / 独立详情 / 四命令用户路径：17 例真实通过后的保护复核与移交（只读收尾）

- 路径说明：本文件放在既有目录 `Development/StudioRuns/R3B-UI-20261028/`（不改名、不改历史日期）。**本机真实时钟核对**：`2026-10-27T18:36:04.952+08:00`（UTC `2026-10-27T10:36:04Z`，时区 `China Standard Time`）。即本机当前日期是 **2026-10-27**，目录名里的 `20261028` 是既有分配路径，本层不改写。
- 本层性质：**只读收尾 + 短文档同步**。未运行 `tests-run`、未 Refresh、未进 PrefabStage、未做字体测量（无 `ForceMeshUpdate` / `GetPreferredValues`）、未新建 UI 实例、未 `rql-mutate`、未 `SaveAssets / SaveChanges / SetDirty / ClearDirty`、未 Shell / Git / Prefs、未连正式工程、未启停 Editor、未创建额外付费子 Agent。
- 状态不变：**R3-B = Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；本文件不代表制作人批准，需主开发专业核验。

---

## 1. 本轮真实 run 元数据（宿主记录，本层只做只读复核）

| 项 | 值 | 来源 |
| --- | --- | --- |
| 宿主记录 ID | `40f35ae0-b7d6-4e37-ad86-235aeb64ae45` | **主开发读原记录确认**（Studio 私有 runtime，本执行层不可读、未冒充复核） |
| 结果 | `Play ManagementShellPlayTests` **17/17 Passed、Failed 0、Skipped 0** | 同上 + 本层只读 `testResults` 交叉核对（见 §3） |
| run 级时长 | `00:00:17.2275503` | 主开发读原记录；本层实测日志段 `2026-10-27T10:33:54Z–10:34:12Z`（≈18 s 墙钟）与之相容，**不互相替代** |
| `resourceScopeEnd` | `scope-restored\|font\|0\|632` | 主开发读原记录（Studio 私有字段，本层不可读，不伪造） |
| 第 17 例（用户路径） | `Passed`、**750 ms** | 本层只读 `testResults` 实测一致 |
| 第 1 例（产品入口/人口行往返） | `Passed`、**654 ms** | 本层只读 `testResults` 实测一致 |
| 方法耗时合计 | **16 199 ms**（17 条逐条相加） | 本层只读 `testResults`；与方法级时长口径一致，**不等于 run 时长** |

**修后（= 本轮 17 例所用版本）SHA（本层只读重算，文件均未改；修前字节见目录索引 §2）**：

| 文件 | bytes | SHA256 | mtime (UTC) |
| --- | --- | --- | --- |
| `Assets/Scripts/Village/ProductHome/ManagementView.cs`（修后） | 42941 | `e02de745c97edac5b2580aa1f9f33af64f7ae5cdac18bd15f65e2136cb702b3c` | `2026-10-27T08:48:40Z` |
| `Assets/Resources/UI/ManagementView.prefab`（修后） | 290093 | `f750ed83ee1c3a7503f3d72a6d5e4900b8c08b16eefd8ade6fe485a8c9f1977b` | `2026-10-27T08:46:51Z` |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（17 例） | 495525 | `24e47dcca95e27d027fb4094afada3983f8749ff2e3830d9ba12774434ec0812` | `2026-10-27T08:50:01Z` |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | 650240 | `676028ab8290f509aa09f531de42d4c73708a0c355c25759a198e4bd6f0c443d` | `2026-10-27T10:18:50Z` |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 575488 | `a9e333362a4bf9cc99e73b53da6d91d2881ddf75fefedf82078ffb52052ca378` | `2026-10-27T10:18:50Z` |

两个 DLL 的 mtime 晚于三份源码 → 本轮 17 例跑的就是当前源码编译出的程序集。“修前”字节（`39037` / `282332` / `408932` 一套）见 `reviewable-delivery-index.md` §2 与 Execution Record §13，本层不重复列。

---

## 2. 保护面复核（`2026-10-27T18:36Z` 前后只读实测，17 例之后）

| 项 | 实测值 | 结论 |
| --- | --- | --- |
| 受保护字体（磁盘） | `14961033B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，mtime 仍 `2026-10-10T13:23:55Z` | 与 34 项基线一致，**未变** |
| 字体 9 子资产 | `subassets=9`、`dirty=0`（只读 `EditorUtility.IsDirty`，**未测量字模**） | 无脏化 |
| 34 项 workspace-baseline | `entries=34 / same=34 / diff=0 / missing=0 / problems=none`（逐项 bytes+SHA256） | **34/34 全一致**；基线文件本体 `6866B / bc7216f125aa0046d116ad23d71e33bd15d9e31d861ce3c317e614429540394e` 未改 |
| 用户存档（真实当前档） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：**`13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`**，`.tmp=False`（目录内 `*.tmp` 0 个），mtime `2026-10-27T10:34:11.0025280Z` | 与主开发给定当前档**逐字节一致**；本层未恢复任何档，**未碰旧 `13405 / 4b2b56f6…`** |
| SAVE-CAPTURE → SAVE-BACKUP → SAVE-RESTORE | 本轮日志：`SAVE-CAPTURE` **17** 条、`readback-verified=True` **17** 条、`SAVE-RESTORE … matches-before=True` **17** 条；无 `matches-before=False` / `readback-verified=False`（各 0） | 每例都“捕获→备份回读校验→收尾恢复并回读比对” |
| guard | `dirs=310 / manifests=310`，`restored=309` + `protective-snapshot=1`，**`unfinished=0`**（无其他状态、`lastError` 非空 0 个）；最新 `20261027T103410Z` `status=restored`、`readbackVerified=true`、`sourceSha256=b3831d4d…`。17 例新增 17 个 run 目录（`20261027T103354Z`…`20261027T103410Z`，293+17=310） | **无 unfinished / 无 noManifest / 无待恢复** |
| 场景 / Editor | `SampleScene dirty=False roots=5`；`PrefabStage` 无打开（`prefab` 源 0 项）；`isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`；工程 `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`（`2022.3.62t9`） | 场景干净、Editor 空闲、身份为 Pilot |
| 本轮 Console | 运行段 `sequence 81191–81680`（首条 `SAVE-CAPTURE runId=20261027T103354Z` 至末条 `BASELINE-POST`）内 `Error/Exception` **0**；`BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`、`FONT-POST … unchanged=True`。运行**之前**的 `81183–81185` 与运行**之后**的 `81681–81683` 是工具层异常（见下文披露），不属于 17 例 | 运行期无错误日志 |

> 磁盘 hash 未变只证明**观察面**未被改写，不推断全机零副作用；`resourceScopeEnd` / runId / skipped 属 Studio 私有 runtime 字段，本层读不到即不写“已复核”。

**本层自身的两次工具异常（如实披露，非产品问题）**：

1. `18:36:40` 一次**只读**统计脚本因过滤窗口为空抛 `InvalidOperationException: Sequence contains no elements`（序列 81681–81683，2 条 Error + 1 条 Exception）。该脚本只做目录/manifest 只读遍历，**未写任何文件**；随后改用目录名排序重跑成功。未清 Console，保留真实痕迹。
2. （上游层）`18:31:25` 的 2 条 `Script/Execute` 编译错误属 `compile-discovery-appendix.md` §6 已披露的同一次工具写法错误，不涉及产品程序集。

---

## 3. 17 条结果（本层只读 `testResults`，逐条 `status` 与耗时）

| # | 用例（`RRPG.ManagementShell.Tests.ManagementShellPlayTests.`） | 状态 | ms |
| --- | --- | --- | --- |
| 1 | `Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip` | Passed | **654** |
| 2 | `CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput` | Passed | 1266 |
| 3 | `ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite` | Passed | 853 |
| 4 | `ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor` | Passed | 759 |
| 5 | `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable` | Passed | 990 |
| 6 | `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput` | Passed | 1310 |
| 7 | `ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock` | Passed | 581 |
| 8 | `ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState` | Passed | 717 |
| 9 | `SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous` | Passed | 1383 |
| 10 | `ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak` | Passed | 1619 |
| 11 | `WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes` | Passed | 856 |
| 12 | `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` | Passed | 618 |
| 13 | `FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt` | Passed | 608 |
| 14 | `ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate` | Passed | 712 |
| 15 | `ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts` | Passed | 1400 |
| 16 | `ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement` | Passed | 1123 |
| 17 | **`Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry`** | **Passed** | **750** |

- 合计 `Failed 0`、`Skipped 0`；逐条 `message=null`、`stackTrace=null`（本层已选择性读取确认无失败详情）。
- **本层未运行任何测试**；上表来自宿主运行后在 Editor 里保留的 `testResults`（17 条），与主开发原始记录一致。

---

## 4. 第 17 例：真实用户路径的逐项落地事实（日志 + 断言映射）

日志来源：本轮 Console `sequence 81293–81310`（全部为 `Log` 级，非 Error）。**点击都是程序化 `EventSystem`**：`RealPointerClick` = `RaycastAll` 取 top-hit → 断言 top-hit 属于目标 → 取 `IPointerClickHandler` → `ExecuteEvents.Execute(pointerClick)`；**不是物理鼠标人工点击**（日志原文即标注“非物理鼠标人工体验”）。

### 4.1 前置状态（无选人）

- 先 `Selection.Clear()` + `RefreshNow()` → `SelectedPersonId=null`；底部导航真实点击进入管理（点击日志 `用户路径·管理（无选人）`）。
- 断言通过（用例未失败即全部成立）：`Surface=Management`、`SelectedPersonId=null`、独立详情入口 `interactable=false`、入口文案 `查看人物详情`、入口 `Image.raycastTarget=true` / 文案 `raycastTarget=false`、`actionHint` 含 `尚未选择人物`、`detailTitle` 含 `未选择人物`。

### 4.2 人口行单击 → 留在管理页 + 选人高亮（关键日志原文）

```
USER-PATH row-click selected=person_e3679e20fc0345d39b638bd0184aa897; surface=Management;
detail-open=False; highlight=True; hint="当前不可用的命令：派工：先在“职责与场所”选择岗位行；
撤职：该人物当前没有主要职责；加入施工 / 撤出施工：先在“职责与场所”选择施工行。"
```

对应断言（全部随用例 Passed 成立）：

| 用户可见要求 | 断言 |
| --- | --- |
| 单击人口行**不跳页** | `Surface()==Management`；`management.IsVisible==true`；`PersonDetail.IsVisible==false` |
| 单击即**选中**该人物 | `Selection==personId`；`management.SelectedPersonId==personId` |
| 选中后**详情入口可用** | `detailEntry.interactable==true` |
| **右栏**显示当前人物 | `detailTitle` 含 `DisplayName`；`detailBody` 含 `PersonId` |
| **行高亮**清楚 | 选中行 `Image.color == (0.28,0.43,0.36,1)` |
| 禁用原因**逐项说明** | `actionHint` 原文见上（派工/撤职/加入/撤出四条各自原因） |

### 4.3 独立详情入口 → 既有 Person Surface → 关闭回 World（人物保留）

- 点击日志：`用户路径·查看人物详情（独立入口）`、`用户路径·关闭人物详情`。
- 断言：`Surface=Person`；`PersonDetail.IsVisible=true`；`DisplayedPersonId==personId`（同一人）；管理页隐藏；关闭后 `Surface=World` 且 `Selection==personId`；重开管理（`用户路径·管理（重开）`）后 `SelectedPersonId` 仍为同一人。

### 4.4 跨页 / 选目标**不丢人物**

- 切到“职责与场所”页（`ShowWorkplacesPage`）后 `SelectedPersonId` 不变；点岗位行 `用户路径·岗位行（farm A）` 后不变；点施工行 `用户路径·施工行（farm B）` 后不变；撤职前再点 `用户路径·岗位行（撤职目标）` 仍不变。

### 4.5 四按钮 `interactable` + top-hit + 真实命令结果

| 步骤 | 按钮 `interactable` | top-hit / 点击 | 真实命令结果（日志原文） |
| --- | --- | --- | --- |
| 派工 | 合法岗位行下 `assign.interactable=true`；`remove.interactable=false`（提示含“没有主要职责”） | `RaycastAll` top-hit + `ExecuteEvents` 真实点击（`用户路径·派工`） | `USER-PATH assign ok workplace=workplace:building_instance_1:main occupants=1`；`PersonWorkplace(personId)==该岗位`；状态栏含“已绑定” |
| 加入施工 | `addConstruction.interactable=true`；`removeConstruction.interactable=false`（未参与） | 真实点击（`用户路径·加入施工`） | `USER-PATH construction-add ok primary=workplace:building_instance_1:main participants=person_e3679e20…`；参与者写入真实活动项目；**Primary Assignment 保持**、岗位占用仍 1；随后 `add` 转 disabled（提示含“已在本施工项目中”） |
| 撤出施工 | `removeConstruction.interactable=true` | 真实点击（`用户路径·撤出施工`） | `USER-PATH construction-remove ok primary=workplace:building_instance_1:main remainingParticipants=`（已移除）；Primary 仍保持、占用仍 1；`add` 恢复 enabled |
| 撤职 | 有主要职责时 `remove.interactable=true` | 真实点击（`用户路径·撤职`） | `USER-PATH remove-assignment ok workplace=<none> occupants=0; surface=Management; simulationActive=True`；状态栏含“已移除” |

收尾断言：四类命令后仍在 `Management` Surface，`IsSimulationActive` 保持为真（管理命令不关模拟、不关页面）。

### 4.6 截图

- **第 17 例不产截图**（用例代码内无截图步骤），本轮**没有**用户路径截图可读。
- 本轮同一 run 内 `ManagementScroll_RealBuildingRows…` 产出 3 张真实滚动截图（`2026-10-27T10:34:05Z`）：`management-real-scroll-workplaces-overflow-last-row-…png` `320140B / a45159b4…`、`…resources-real-rows-…png` `320936B / 8a6a608f…`、`…population-detail-facts-…png` `279706 / cc1b97cf…`，另有 `REAL-SCROLL-COVERAGE Workplace=True Construction=True Resource=True Population=True`。它们证明的是**滚动/行文本**，**不能**当作“详情入口/四命令按钮”的截图证据。

---

## 5. 第 1 例（654 ms）也走了修订后的路径

- 日志：`人口行（第一行·只选人）`、`查看人物详情·独立入口`、`人物详情·关闭`（`sequence 81281–81282` 及相邻）。
- 断言：人口行单击后 `Surface=Management`、`PersonDetail` 不可见、`Selection` 与 `SelectedPersonId` 同为第一人、独立入口可见且可点、右栏标题含姓名、选中行高亮 `(0.28,0.43,0.36,1)`；由图独立入口进入 Person、显示同一 `PersonId`、关闭回 World 且保留选择、重开管理仍同一人；`AssertSerializedObjectReferences(management, 40)`（0 个空引用口径不变）也通过。
- 即：**旧的“点人口行必须跳详情”期望已被真实覆盖替换**，旧路径（关闭回 World / 重开保留人物）仍被保护。

---

## 6. 人可理解的操作步骤（复现 / 人工体验用）

1. 打开 Pilot 工程 `UnityProject/RRPG1.0game`（Unity `2022.3.62t9`），进 Play，等真实产品 Home 出现、底部导航可见。
2. 先确保**没有**选中人物（世界点空白 / 清选择后刷新一次），再点底部 **“管理”**。
3. 看右栏：标题应显示“未选择人物”，操作提示应写“尚未选择人物”，**“查看人物详情”按钮应为灰色不可点**。
4. 在**人口**页点任意一条人口行：页面**不应**跳走；该行应变为绿色高亮，右栏出现“所选人物 · 姓名”与 PersonId，下方提示逐条说明四个命令为何当前不可用。
5. 点 **“查看人物详情”**：进入既有的人物详情页（同一人）；点关闭 → 回世界，人物仍被选中；再点“管理”重开 → 仍显示同一个人。
6. 切到 **“职责与场所”**：点一条岗位行 → 选“派工” → 状态栏出现“已绑定”，该岗位绑定人数 +1；点一条施工行 →“加入施工”（岗位绑定保持不变）→“撤出施工”（岗位绑定仍不变）。
7. 回到岗位行点 **“撤职”** → 状态栏“已移除”，岗位绑定人数 -1；全程页面不应自行关闭、模拟不应停止。
8. 与自动化的差别：上述自动化版点击是**程序化** `RaycastAll + ExecuteEvents`，能证明“点得到、点的是它、命令真的生效”，但**没有**覆盖物理鼠标手感、滚轮/拖拽手感、键盘/物理 ESC、DPI/焦点/IME、以及 Player 包内行为。人工体验结论需单独记录，不要用本文件代替。

---

## 7. 仍成立 / 仍未覆盖（不因 17/17 升级）

- **物理输入未验**：鼠标 / 滚轮 / 拖拽 / 键盘 / 物理 ESC 的**手感与 OS 事件层**仍未覆盖；本轮与上游第 14/16 例只覆盖“机制层”（合成输入经真实 `Update`/`LateUpdate` 链路）。规格只要求“不穿透”，未把“手感”列为验收项。
- **Player 构建内行为**未验（采样缝 / registry 缝均 `#if UNITY_EDITOR`）。
- `SaveSlotGuard` 仍是**用例内**兜底：进程级 abort 不自动恢复（本轮 `unfinished=0` 只代表没有中断残留）。
- **完整回归未重跑**：122 Play / 1513 Edit / 264 Compiler 均为前阶段真实通过，不是最新全量，也不作为本次门槛。
- Deferred / 取消不变：动态批次与供餐细节、Chevron、`CapacityFull` UI 文案像素；1280×720 由制作人 `01a53eb8` **取消（取消 ≠ 通过）**。
- 历史不洗白：旧 650 档（`21075B / 650c45c4…`，无字节备份，未恢复）；两次历史边界违规（旧 `ClearDirty`、详情修复包 `SaveAssets`）原文保留。
- **本轮缺陷定性**：人口行强制跳页、缺独立详情入口、按钮禁用原因笼统，是**产品/UI 与测试期望的缺陷**，已由代码与 Prefab 修复并由真实用例覆盖；**不是用户操作问题**。反之，本层也不把“17/17 通过”写成“制作人已验收”。

---

## 8. 文档同步（本层实际改动，只有文档）

| 文件 | 改动 | 说明 |
| --- | --- | --- |
| `Development/StudioRuns/R3B-UI-20261028/person-selection-command-repair.md` | 追加 §7“当前附记（17 例真实通过后的只读复核）” | 原文 1–6 节一字未改；更新前 `6033B / da52af93b74017783305686688228cd23f325f764162733531afde006bffdc65` |
| `Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md` | 顶部新增一条“最新补充（17 例）”+ 文末 §10 | 上文 §1–§9 历史口径保留 |
| `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md` | 顶部新增“2026-10-27 17 例当前事实”块 + 文末 §14 | §1–§13 历史保留 |
| `Development/StudioRuns/R3B-UI-20261028/selection-command-verified-handoff.md` | **新增（本文件）** | 真实 run 元数据 / 前后 SHA / 保护面 / 操作步骤 |

**本层改动后实字节（`2026-10-27T18:38:21+08:00` 只读重算）**：

| 文件 | bytes | SHA256 |
| --- | --- | --- |
| `R3B-UI-20261028/person-selection-command-repair.md` | 10649 | `d4b90c670aaf1840f744331eaade3c3c690eac1a5aacf57dd6dddaef69bd900c` |
| `R3B-UI-20261028/selection-command-verified-handoff.md`（本文件，生成时点） | 19901 | `6f9e77d2ef4206f0a8a0e928d2e182c0d398b9d23b9cc6e44e80643758b081d0` |
| `R3B-UI-20261026/reviewable-delivery-index.md` | 43261 | `2324f579dcfeb62698f708865bf994cfb00a039fcc32a1f1861c640fb0f39916` |
| `Docs/DevelopmentSpecs/2026-10-24_…Execution-Record.md` | 62487 | `14507da1ef572ef1c7c4065ed9792925ee90ef5bf4ecd26581d7cb4507bffad7` |
| `R3B-UI-20261028/compile-discovery-appendix.md`（未改） | 5807 | `27cb041e37c4de70199a2d024ecc2eedfe4d7fd7689c6c70e934fc0017fc47cc` |
| `R3B-UI-20261027/management-selection-acceptance-fix.md`（历史，未改） | 6033 | `da52af93b74017783305686688228cd23f325f764162733531afde006bffdc65` |

同一次只读重算确认：`ManagementView.cs` `e02de745…`、`ManagementView.prefab` `f750ed83…`、`ManagementShellPlayTests.cs` `24e47dcc…` 与 §1 表一致 → **本层未改任何代码 / Prefab / 测试 / 资产**。

**历史失败保留**：`Development/StudioRuns/R3B-UI-20261027/management-selection-acceptance-fix.md` 仍为原字节 `6033B / da52af93…`（read-only 回读一致，未改写）；`compile-discovery-appendix.md` 未改（`5807B / 27cb041e…`）。

---

## 9. 本层边界清单（自查）

未做：`tests-run` / 任何测试执行、`AssetDatabase.Refresh`、PrefabStage、字体测量（`ForceMeshUpdate` / `GetPreferredValues`）、UI 实例化、`rql-mutate`、`SaveAssets` / `SaveChanges` / `SaveDirtyScenes` / `SetDirty` / `ClearDirty`、Shell / `Process.Start`、Git（含只读命令亦未跑）、Preferences、正式工程、构建 Player；未改任何代码 / Prefab / 测试 / 资产 / 存档 / guard manifest / 34 项基线；未创建额外付费子 Agent、未安装组件、未改权限。

只读接口：`rql-query`（editor / scenes / testResults / console / prefab）、只读 `script-execute`（哈希 / 目录与 manifest 统计 / IsDirty）、本地 `read` / `glob` / `grep`。

结论：**需主开发专业核验；不代表制作人批准；R3-B 仍 Implemented / Verification PARTIAL / Not Committed / Not Published。**
