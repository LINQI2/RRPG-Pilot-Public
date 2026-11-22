# R3-B Pilot 最终自审索引（阶段 2/2 · 日期化文档收尾）

- 仓库 / 唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot）；Unity 工程：`UnityProject/RRPG1.0game`（`2022.3.62t9`）
- 本地日期（时钟核实）：**2026-10-24**，`Asia/Shanghai`（`localNow=2026-10-24T22:43:30.6224819+08:00` / `utcNow=2026-10-24T14:43:30.6224819Z`，UTC+8）；未跨 10-25。
- 当前分支 / 基线（只读文件证据，未执行任何 Git 命令）：`.git/HEAD` = `ref: refs/heads/ai/rrpg-r3b-population-pilot`，该 ref = `054b00d3e38fe2e2e142bfdb2502de90f7991030`，与 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 的 `head` 相同 → **本轮未产生 Commit，工作树未提交、未发布**。
- 本阶段性质：**只做自审与文档收尾**。未新增测试、未改产品代码 / 测试 / 场景 / Prefab / 资产 / 配置 / 字体 / Packages / ProjectSettings、未运行 `tests-run`、未 `Refresh`、未 `rql-mutate`、未触发 Compiler、未 Shell、未全局 Save、未 Git 写、未改 14 / 34 保护基线、未读写用户存档本体、未恢复历史 650 档、未访问未授权工程 / 认证目录 / 其他工作区、未安装组件 / 改权限 / 创建额外付费子 Agent。
- **唯一一次 Unity 通道调用（如实披露）**：为核实“本地日期是否跨 10-25”，执行了 **1 次只读 `script-execute`**，代码只读 `DateTime.Now` / `DateTime.UtcNow` / `TimeZoneInfo.Local`，**未读写文件、未访问场景 / 资产 / 存档、未调用 Refresh / 保存 API、未运行测试**；返回 `2026-10-24T22:43:30+08:00`（UTC+8）。此后全部工作只用 DSH 文件工具（read / glob / grep / write / edit），未再调用 Unity 或 Compiler。
- 本阶段实际写入：本文件 + 新增日期化执行记录 + 5 份状态/导航文档同步 + 2 份旧记录的状态附记（清单见 §8）。
- **结论口径：R3-B Pilot = Implemented（主要链路）/ Verification PARTIAL / 未提交未发布 / 未整包关闭。** 不写 “Published / Closed / Accepted”；Compiler 264 已于 2026-10-25 真实通过，整包因视觉/输入与第二 slot 真实 Gateway、claim 施工等未测仍为 **PARTIAL**。

> **2026-10-26 全部必要明细修复后整类 13 例（最终同步，最新；优先于下列全部块，历史保留不删）**
> 1. **管理 Play 整类 13/13 Passed（当前版本）**：本机 `testResults` 13 条全 `Passed`、`message=null`（方法耗时 547–1521 ms，合计 **11 754 ms**）；主证据日志 `2026-10-26T10:29:23.276Z–10:29:35.467Z`（≈12.19 s）段 **13 组 `[SAVE-RESTORE] matches-before=True`**（runId `102923Z`→`102934Z`，sha 均 `4b2b56f6…d6aa`）、**3 组 `[SCHEDULE-RESTORE] restored=true sameInstance=True`**、`[FONT-POST] unchanged=True`、`[BASELINE-POST] total=34 same=34 changed=0 problems=0`。**宿主 runId / run 级总时长 / skipped 本层读不到，标“待主开发补”，不编造**。旧 13/13（08:53 仅岗位行版）、12/12 `4aa4ffda`、11/11、10/10 均为历史。
> 2. **滚动专项单方法**：主开发已核 **`e75c1ec5-d51c-4239-88f0-4ec5dc1ee4ae`：1/1 Passed、0 skipped、`3.1183267` s、`scope-restored`**；本机单条 `Passed / 2144ms`。
> 3. **四类业务行明细全部真实可读（本轮补齐结果）**：人口 `lines=3`（`checkedChars=40/37`，职责 / 执行 / 临时承诺·饥饿）；施工 `lines=3`（**`checkedChars=58`**，进度 / `工人 1 人：姓名（PersonId）` / 现场）；资源 6 条真实行（`checkedChars=41`，名额·劳动 / 实例·槽位 / 定位说明）；岗位 6 条（`checkedChars=51`）。职责 `contentHeight=1044 / overflowPx=394`、资源 `834 / 184`，真实 `IScrollHandler` + 多轮拖拽末行可达。截图 `T102931Z` 三张（workplaces `310452B/f4c0069d…`、resources `313330B/d375bf3b…`、population `274780B/14c446da…`）与布局例 `T102929Z` 三张（`229965B/d48cd7c3…`、`183167B/d8c13a9e…`、`176055B/726645ca…`），历史图未覆盖。
> 4. **当前代码 SHA**：`ManagementView.cs` `39037B / f8316e43…2016473`；`ManagementView.prefab` `282332B / 31bf3e8c…30e8764`（四模板 `1100×132 / Detail 1072×76 / Label 1072×32`，`1072×42` 与 `1072×28` 均 0）；`ManagementShellPlayTests.cs` `390868B / 219379e0…7a45c`；`Assembly-CSharp.dll` `647680B / b31f1b64…`（晚于源码）、`BattleStage1.PlayModeTests.dll` `512512B / 40e41b98…`（晚于源码）。
> 5. **保护面（2026-10-26T10:30:31Z，13 例运行后只读实测）**：Editor 空闲、`SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080`、Console 0 异常；字体 `14961033B / 3c758e16…c404854` + 9 子资产 0 dirty；34/34；单槽 `13405B / 4b2b56f6…d6aa`（`10:29:35.176Z` 收尾回读）；guard **244 = 243 restored + 1 protective-snapshot**（最新 `20261026T102934Z restored / readbackVerified=true`，无 unfinished）。
> 6. **管理 Edit 18/18（`99dc1dfe`）与前阶段 264 / 1513 / 122** 均为**前阶段真实通过、本阶段未重跑**，不是最新全量；**1280 由制作人 `01a53eb8` 取消（取消 ≠ PASS）**。
> 7. **仍未覆盖（保留）**：第二 FunctionSlot 真实 Gateway、物理鼠标 / 键盘 / 镜头手感、Chevron、`CapacityFull` 域内诊断文案、动态批次 Deferred、`SaveSlotGuard` 进程级缺口、历史 650 档。**已解决项不再列当前未测**：Person / Resource 行 detail 截断、施工工人列表隐藏、资源业务行标题无证据（历史文本保留标“当时”）。**状态仍 Implemented / Verification PARTIAL / Not Committed / Not Published。**

> **2026-10-26 详情修复后整类 13 例（当时口径：仅岗位行详情可读、资源/人口/施工工人尚未补齐；已被上方最终块取代，历史保留不删）**
> 1. **管理 Play 整类 13/13 Passed**：宿主真实运行，本执行层只读核对 `testResults` **13 条全部 `Passed`、`message=null`**（方法耗时 531–1477 ms，合计 11 653 ms）；支撑证据日志段 `2026-10-26T08:53:18.233Z–08:53:30.321Z`（≈12.09 s，日志时间差）内 **13 组 `[SAVE-RESTORE]` 全部 `matches-before=True`**、两例 `[SCHEDULE-RESTORE] restored=true sameInstance=True`、`[FONT-POST] unchanged=True`、`[BASELINE-POST] total=34 same=34 changed=0 problems=0`。**宿主 runId / run 级总时长 / skipped 字段本层读不到，未编造**；旧 12/12 `4aa4ffda` 保留为历史。
> 2. **详情修复后滚动单方法**：主开发已核 `49fcb102-24df-466d-9496-356a67dce8c2`，**1/1 Passed、0 skipped、`3.2231345` s、`scope-restored`**；本机 `testResults` 单条 `Passed / 2092ms`、`real-scroll-evidence.log` 本轮段完整。
> 3. **职责与场所详情关键数值已真实可读**：`ManagementView.cs` 详情行序改为「绑定 b/c · 实际劳动 l · 现场 o」→「岗位 完整 WorkplaceId」→「建筑/槽位/功能/绑定人」，`WorkplaceRowTemplate` 行高 100→132、Detail 42→76；本轮日志 6 条岗位行 `lines=3 preferredH=73.85 rect 高 76 checkedChars=51`、1 条施工行 `checkedChars=6`；主开发已看 `T084927Z` 截图，本层 `read_image` 亦独立确认「绑定 0/2 · 实际劳动 0 · 现场 0 / 岗位 workplace:building_instance_N:main / 建筑 … 槽位 main · 功能 ws_farm_01 · 绑定人：无」三行可读。
> 4. **当前代码 SHA**：`ManagementView.cs` `38207B / 9b593936…07c8fcb`；`ManagementView.prefab` `282326B / a05a7715…a0139a`（`1072×42`=3、`1072×76`=1、`1072×32`=4、`1072×28`=0、`1100×132`=1）；`ManagementShellPlayTests.cs` `371699B / 03a7c2c8…cabf0ee`。
> 5. **当前保护面（2026-10-26T08:51–08:54Z 只读实测）**：Editor 空闲、`SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080`；字体 `14961033B / 3c758e16…c404854`、内存 9 子资产 0 dirty（`characterTable=632`）；34/34 same；单槽 `13405B / 4b2b56f6…d6aa`（mtime 08:53:30.032Z，即本轮收尾回读）；guard **230** 目录 = **229 restored + 1 protective-snapshot**，无 unfinished / noManifest。
> 6. **管理 Edit 18/18**（`99dc1dfe`）与前阶段 **Compiler 264 / 完整 Edit 1513 / 完整 Play 122** 均为**前阶段真实通过、本阶段未重跑**，不是最新全量；**1280 由制作人 `01a53eb8` 取消（取消 ≠ PASS）**。
> 7. **仍未覆盖（保留）**：资源页业务行标题、第二 FunctionSlot 真实 Gateway、物理鼠标手感（本轮滚动为程序化 `EventSystem` + 多轮拖拽，非物理指针）、动态批次 Deferred、Chevron、Person/Resource 行 detail 单行、历史 650 档缺口。**状态仍 Implemented / Verification PARTIAL / Not Committed / Not Published。**

> **2026-10-26 最终当前事实（上一轮 12 例口径；优先于下列历史块，历史事实保留不删）**
> 1. **管理 Play 专项已到 12/12 整类绿**：主开发已核 run `4aa4ffda-3730-41be-ad11-2f9550ff31bc`，**12/12 Passed、0 skipped、`11.4823431` s、`scope-restored`**；本执行层只读核对 `testResults` 12 条全部 `Passed/message=null`、12 组 `[SAVE-RESTORE]`（全部 `matches-before=True`，sha `4b2b56f6…d6aa`）、两例 `[SCHEDULE-RESTORE] restored=true sameInstance=True`、以及 `FONT-POST unchanged=True` / `BASELINE-POST total=34 same=34 problems=0`。旧 11/11 `7f600ef9`、10/10 `0fd4b031` 保留为历史。
> 2. **管理 Edit 专项已到 18/18**：主开发已核最新整类 `99dc1dfe`，**18/18 Passed、0 skipped、`1.2183834` s**；第 18 例 claim 施工单例 `41a9596f` **1/1 Passed**，时间线为 **464 claim 保持 → 465 合法 Interrupted 释放 → 480 到场零劳动 → 481 劳动 1**（工作优先中断未排程 Rest），未新增 busy 拒绝、未改 Work/Clock/Activity。
> 3. **人口行姓名可见已真实修复并验证**：Prefab 4 个行模板 Label 高 **28→32**（当前 `1072×32` 4 处、`1072×28` 0 处，`282326B / e789307a…`）；最新 12 例中 `[ROW-TITLE-RENDER]`×6 + `[ROW-TITLE-VISIBLE] rows=6 checkedRows=6`，主开发已亲自查看 `071915Z` 人口 PNG 六行姓名职业可见。旧 `b17cd5c1…` 为 **rejected（`Font already has pending edits`，测试未发送）**，**不是断言失败**；当前字体内存 9 子资产 0 dirty。
> 4. **1280 已由制作人 `01a53eb8` 取消**：记为“取消”，**不再待人工、不再 BLOCKED、不记 PASS**；不得由 1920 结果外推 1280。
> 5. **当前保护面（2026-10-26T07:2xZ 只读实测）**：Editor 空闲、场景 `SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`；字体 `14961033B / 3c758e16…c404854`、内存 9 子资产 0 dirty；34/34 same；单槽 `13405B / 4b2b56f6…d6aa`；guard **202** 个 run（`restored=201` + `protective-snapshot=1`，无未完成/未知）；Git HEAD 仍 `054b00d3…`（只读，无 Git 写）。
> 6. **版本范围（禁止外推）**：`264 / 1513 / 122` 是**前阶段真实通过**，**未以最新 Prefab / 最新 12 例源码重跑全集**；本包不因收尾重复全集，也不据此宣称整包全绿。
> 7. **仍未覆盖（保留）**：第二 FunctionSlot 真实 Gateway（配置只登记 `main`，无合法注入点；**与 SaveSystem 单槽存档无关**）、真实 overflow 滚动 / 拖拽 / 滚轮 / 物理鼠标、其他业务行标题截图、Chevron、Detail 单行 Ellipsis、历史 650 档缺口。这些**不升级**为“需批准”或“新增产品规则”。
> 8. **历史异常保留**：PrefabStage 只读打开触发非有意自动回写，`282097B → 282326B`（+229B），**只相关、未确定根因**，不能写“插件已确定根因”或“只改 4 个数字、其余未变”；旧文件无备份、不要求恢复旧 Prefab。
> 9. **状态不变**：R3-B = **Implemented / Verification PARTIAL / Not Committed / Not Published**；需完整可审交付，**不 Closed / 不 Committed / 不 Published / 不 Accepted**。
> 10. 详细逐项矩阵与当前代码 SHA 见本文 **§10** 与 `Development/StudioRuns/R3B-UI-20261026/final-current-evidence-review.md`。

> **2026-10-25 当前事实更正（本块覆盖下列旧结论；历史失败与中止记录保留，不抹除）**
> 1. **Compiler 不再 BLOCKED**：宿主真实运行 `264/264 Passed、0 skipped`，TRX Counters `total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0`，`tests`/`validate` 退出 0，validate 8 条导航 sheet warning、Mirror 未写入；494 项保护文件 SHA 未变（`changedProtected=[]`）。证据见 `Development/StudioRuns/R3B-Compiler-20261025/`。本包不运行 Compiler、不重算 TRX SHA，按主开发已核回执登记。
> 2. **完整 Play 已更新为 122/122**：主开发已核宿主回执 **122/122 Passed、0 skipped、`1:50.7921181`、`scope-restored`、记录 `5eb0ad4c`**；旧 121/121（`c048f135`）保留为历史回执。
> 3. **管理 Play 第 11 例已真实通过**：单例 `ba019b1c` **1/1 Passed**；最新独立整类 **11/11 Passed、0 skipped、记录 `7f600ef9`**；此前独立 **10/10 `0fd4b031`** 也真实存在。`PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` 与其 `SCHEDULE-RESTORE sameInstance=True` 已实际通过，TearDown 恢复顺序修复已生效。
> 4. **HomeProductShell 首次导航已产品修复**：`SyncSourceReset` 已在 `54a6c399` 包实改，`FirstNavigation` 在 `1088aa8e`、`0fd4b031`、11/11 与 122/122 中通过；不再说“产品未修 / 只改测试”。
> 5. **撤职当前语义**：执行/贡献即停，但旧 `arrival` 仍在下一次工作边界才清理；不宣称“撤职当刻清意图”。
> 6. **第二 slot**：只读投影已测；真实 Gateway 第二 slot 命令路径**仍未测**（未改配置 Authority、未伪造）。
> 7. **Activity claim**：保持既有语义，本包未增加 busy 拒绝规则；claim 持有者加入施工仍未测。
> 8. **当前存档与保护**：2026-10-25 完整 Play 后已由 `20261025T074636Z` 精确写回 **13405B / `4b2b56f6…d6aa`** 并回读；残留 `075219Z-residue`（13402B / `4d2562a8…`）已独立备份。历史 `21075B/650c45c4…` 仍无备份、未恢复。
> 9. **整包仍 PARTIAL**：视觉 1920×1080/1280×720、滚动/拖拽、物理输入、第二 slot 真实 Gateway、claim 持有者加入施工等仍未测；**Not Committed / Not Published**，不宣称整包 Accepted。
> 10. **历史保留**：原 `48d8820f` 轮 aborted、`pending-event-management-gate-review.md` §6 计划纠正 9 文档实际未执行，均保留为历史，不当作已完成。

> **事后事实更正（2026-10-24T15:0xZ，R3-B 中止后只读核验；以下为当时状态，第 1 条已被上方 2026-10-25 块取代）**
> 1. 后续包已在 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` **新增第 11 例** `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice`（文件 289142B / `96982624…0750cd43`；程序集 `BattleStage1.PlayModeTests.dll` mtime `2026-10-24T14:59:24Z`；`tests` 源发现类内 **11** 例）。但**该用例从未被运行**：`testResults` 仍为旧 121 条、无 `PendingEvent` 结果；因此本文件 §4/§5 中“管理 Play 10 例”“V15 事件 pending 未测”仍是**运行回执层面**的当前事实，第 11 例只能标“已写入、未验证”。
> 2. `pending-event-management-gate-review.md` §6 声称“同步纠正 9 份文档”，**实际未执行**（该包中止，本文件与执行记录等 9 份文档内容、哈希均未变）。本文件以下正文保持中止前状态，不据该 §6 宣称已更正；文档更正经中止后独立小包重做。核验见 `pending-gate-recovery-preflight.md`。
> 3. 同文件 §7 原写“exactly-once 已验证”属未测口径，已在原文件就地更正为“尚未验证”。

---

## 1. 结论速览

> 下表是 **2026-10-25 状态**（保留为历史）；**2026-10-26 最新值**见顶部“最终当前事实”块与 §10（管理 Play 12/12、管理 Edit 18/18、guard 202 run 等）。

| 维度 | 当前事实 | 判定 |
|---|---|---|
| 产品链路 | 管理页（人口 / 职责与场所 / 资源）接入真实 Home 产品宿主；实例岗位、施工加入/撤出、派工/撤职、显式保存、Continue/NewGame 资源隔离已实现 | **Implemented**（主要链路） |
| 宿主完整 EditMode（无 filter） | **1513/1513 Passed、0 failed、0 skipped**、`2:53.578`、`scope-restored`、记录 `f116b56e`（主开发已核；发现口径 1387 / 24 程序集 / parameterized 59） | Passed（引用回执，本包未重跑） |
| 宿主完整 PlayMode（无 filter） | **122/122 Passed、0 failed、0 skipped**、`1:50.7921181`、`scope-restored`、记录 `5eb0ad4c`（主开发已核；旧 121/121 `c048f135` 为历史） | Passed（引用回执，本包未重跑） |
| 管理专项 Edit | **17/17 Passed、0 skipped**、`f8d40b09`（主开发已核） | Passed（专项） |
| 管理专项 Play | 类内 **11** 例；最新独立整类 **11/11 Passed、0 skipped**、`7f600ef9`；此前独立 **10/10**、`0fd4b031` 亦真实存在 | Passed（专项，第 11 例已实际运行） |
| Compiler 264 | **264/264 Passed、0 skipped**；TRX Counters `total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0`；`tests`/`validate` 退出 0；validate 8 条导航 sheet warning、Mirror 未写入；494 项保护 SHA 未变 | Passed（宿主回执，主开发已核；本包未重跑） |
| 单槽存档 | 当前 **13405B / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**；2026-10-25 完整 Play 后由 `20261025T074636Z` 精确写回并回读；残留 `075219Z-residue` 已备份 | 已恢复（当前档，≠历史 650） |
| 保护 run | **175 个目录**：174 份带 `status`（**173 restored + 1 protective-snapshot** `20261024T052704Z`）+ 1 份 `kind=residue-backup`（`20261025T075219Z-residue`，**无 `status` 字段**） | 无 `captured`/`restore-failed`；该 residue 会被 `BlockOnUnfinishedRuns()` 误判为未完成（见 `R3B-UI-20261025/document-correction-review.md`） |
| 34 项基线 | 本 run 恢复后重算 **34/34 same、0 missing、0 diff**（阶段 1 postflight 实测）；Compiler 维护另核 **494 项** `changedProtected=[]` | 一致 |
| 制作人指定 14 项 | TMP 字体 1 + Growth 12 + ProjectSettings 1；字体 `14961033B / 3c758e16…c404854` 与基线同值（NuGet/Packages 属 34 项但**不属** 14 项） | 一致 |
| 字体变化事故 | `e40b6d77…/14968490`（2026-10-23T12:30:59Z，`rql-mutate` 普通 call 后全局 `SaveChanges`）已用**完整字节备份**经 `FileUtil.ReplaceFile` 定向恢复为 `3c758e16…/14961033`；meta 未变 | 已恢复（≠历史 650 恢复） |
| 历史 650 存档 | 原档 `21075B / 650c45c4d1226ab5606073cd9210545438b37482d2b5de47559a2ca6ecb06ee5` 在授权范围内**无字节备份**；`2026-10-23T15:53Z` 轮 5 次 Capture 无 Restore | **缺口保留**，不恢复、不伪造 |
| 整包判定 | 未跑：多分辨率 / 滚动 / 物理输入、第二 slot 真实 Gateway、claim 持有者加入施工；Compiler 与事件 pending 第 11 例已于 2026-10-25 真实通过 | **PARTIAL**，不得宣称整包 PASS / Closed / Accepted |

---

## 2. 身份、保护面与“旧 14 项”逐项基线

### 2.1 本轮只读实核（非引用）
- 分支 / HEAD：见首部（`.git/HEAD` + ref 文件）。**未执行 git 命令**，因此 `index`、`status` 与“任务外 dirty/untracked 数量”本轮**未核验**；按“未提交未发布”记录，不宣称 Branch / Repository Closed。
- 34 项：`Development/StudioRuns/R3B-20261022/workspace-baseline.json`（`root` = 仓库根、`branch` 一致、`head` 一致、`complete=true`）在本 run 恢复后由阶段 1 postflight 逐项重算 → **34/34**；本包**禁止触发 Unity**，故不再重算，直接引用同日实测回执（`full-edit-postflight.md` §5.1、`full-play-postflight.md` §7.1）。
- 34 项与 14 项的集合关系：**14 项是 34 项的子集**；`NuGet` / `Packages` / `.codely` 等属 34 项但**不属** 14 项（沿用 `R3B-UI-20261023/controller-report.md` 末尾的主开发更正）。

### 2.2 制作人指定 14 项（逐项构成，全部与基线同值）
| # | 项 | 说明 |
|---|---|---|
| 1 | `Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | **14961033B / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** |
| 2–5 | `Assets/Resources/ScriptableObjects/Growth/Nodes/`：`GrowthNode_Lv2_CardGrowth` / `Lv3_StarterSlotUnlock` / `Lv4_TraitUpgrade` / `Lv5_UnlockedPoolExpand` | Growth Nodes 4 项 |
| 6 | `.../Growth/TestGrowthCurve.asset` | 曲线 |
| 7 | `.../Growth/TestGrowthTable.asset` | 成长表 |
| 8 | `.../Growth/TestTraitCatalog.asset` | Trait 目录 |
| 9–13 | `.../Growth/Traits/trait_char_001..004.asset`、`trait_hero_001.asset` | Traits 5 项 |
| 14 | `UnityProject/RRPG1.0game/ProjectSettings/ProjectSettings.asset` | ProjectSettings |

> 以上 14 项在 `R3B-UI-20261023/prefab-binding-report.md` §9 曾逐项复核；本轮由 34/34 全覆盖（14 ⊂ 34）。**本轮未单独重算 14 项 SHA**，不把“包含在 34/34”说成本包独立重算。

### 2.3 字体事故与恢复（保留历史与根因，不冒称“字体恢复 = 650 恢复”）
- 变化事实：`2026-10-23T12:30:59.043Z` 字体被写为 `e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8` / 14968490B（+7457B）。
- 已查明直接保存路径：原执行器 `f6ae` 的 seq331 在 `12:30:58Z` 调用 `rql-mutate` 的 `editor|call RefreshAssets()`；MCP 1.3.0 `RqlUnity.Save.cs` 在 mutation 普通 call 后调用 `SaveChanges()` → `AssetDatabase.SaveAssets()` 并保存 dirty 场景。`RefreshAssets` 本身只 Refresh，**外层 mutation 仍全局保存**。
- 处置：原字体完整字节备份（SHA = 基线 `3c758e16…`、14961033B）经 `UnityEditor.FileUtil.ReplaceFile` + `ImportAsset` **仅恢复该字体**，meta 哈希保持原值；变化版本 `e40b6d77…` 亦已完整备份留证。`GrowthDataInitializer.cs` 移除全局 `SaveAssets` 与对已存在资产的无意义 `SetDirty`，仅保存自己新建的资产；新增“无关 dirty 材质不被保存”的回归。
- 防范：Studio 已部署 `ctx.tools.guard`，受保护工程拒绝 `rql-mutate`；刷新改走 `script-execute` + `AssetDatabase.Refresh`，保存只用精确目标 API。**守卫不是任意 C# 安全沙箱**，本轮不再依赖“不会被误保存”的假设。
- 边界：**字体已恢复不代表历史 650 存档已恢复**；两者是不同的证据链，本文件分列，不互相冒充。

---

## 3. 代码文件差异（按真实证据汇总）

> **SHA 口径**：本包禁止触发 Unity / Compiler，且本层除 DSH 文件工具外**没有独立哈希通道**（`shell` 禁用、`script-execute` 属“触发 Unity”被本轮排除）。因此下表 SHA 为**该文件最近一次被本仓库证据登记时的实测值**（列出记录来源与日期），**不是本包对当前工作树的重新计算**；当前树 SHA 需由允许只读探测 / Git 的后续包复核。行数、字节数同理。

### 3.1 产品代码 / 序列化资产（R3-B 新增或修改）

| 路径 | 变更性质 | 最近登记 SHA256（记录来源） |
|---|---|---|
| `Assets/Scripts/Village/ProductHome/ManagementView.cs`（+ `.meta` 267B `ef6483af28ab0418c203eeb71c0d0ab14265bcc41b8c0fde0f2dd3224dd289aa`） | 新增：管理页控制器（35 个 `[SerializeField]`、三页签、行模板与命令按钮接线） | 37518B / `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d`（`controller-report.md`，2026-10-23） |
| `Assets/Scripts/Village/ProductHome/ManagementRowView.cs` | 新增：行视图 | 1670B / `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd`（`prefab-binding-report.md`，2026-10-23） |
| `Assets/Scripts/Village/ProductHome/ManagementReadAdapter.cs` | 新增：只读投影（人口 / 实例岗位 / 施工 / 资源）+ 2026-10-23 劳动人数口径修正（`Labor(bound, workplaceId)` 需真实 Presence 目标匹配） | `752c6a2d8ebb0579ad30792fab93f4e8b70b56f2d5d933ae0f23cf0457f6fea7`（`labor-projection-review.md`，2026-10-23 修正后） |
| `Assets/Scripts/Village/Runtime/ManagementSnapshot.cs` | 新增：不可变快照模型（仅补 `LaborPersonIds` 注释，结构未变） | `1184f80307e8ef07563193f27433a6983077b7d79a312b1df57b53ae5ade1ce6`（同上） |
| `Assets/Scripts/Village/ProductHome/ManagementCommandGateway.cs` | 新增：合法命令网关（派工 / 撤职 / 施工加入撤出 / 保存），拒绝语义与零写入 | 13603B / `d912bfc9baea7c68f3aad3d77f606fcbd1e27912f0cf59710131a4bd5453cc5e`（`management-actions-play-report.md`，2026-10-23） |
| `Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | 修改：管理 Surface 接入（`SurfaceModalActive` 唯一判定、导航/ESC/关闭、Town 输入阻断；`IsPersonModalOpen` 保留同义别名）；**2026-10-25 当前事实：首次导航同帧复位的 `SyncSourceReset` 已在 `54a6c399` 包实改** | 15497B / `7e9901d7092fda62aea06a2f1a8fbe3bfe2c84974df08eab0002a09015c07fcd`（`shell-integration-report.md`，2026-10-23，**产品修复前**；当前文件 SHA 需后续包复核） |
| `Assets/Scripts/Save/SaveSystem.cs` | 修改（1 行）：`NewGame()` 复用既有 `VillageRuntimeBridge.Resources.Restore(null)`，使新世界回到资源初始 Authority（0/0）；未改 DTO / SaveVersion / 公共 API | 该行变化见 `newgame-resource-reset-review.md`（2026-10-24）；**该文件 SHA 未被任何证据登记** |
| `Assets/Resources/UI/ManagementView.prefab`（+ `.meta` 178B `b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559`） | 新增：可编辑固定 UI（标题/关闭/三页签/4 个行模板/滚动容器/全屏 Blocker，39 个序列化引用 0 null） | 282097B / `ade4c7e1b0fb3f9e09e69733ac8066d191cb59060f5b59970cfdb1053df83f11`（`prefab-binding-report.md`，2026-10-23） |
| `Assets/Editor/GrowthDataInitializer.cs` | 修改（维护包）：移除全局 `AssetDatabase.SaveAssets()` 与对已存在资产的 `SetDirty`，只保存自建资产 | 变化见 `R3B-Font-Recovery-20261023/maintenance-handoff.md`；**该文件 SHA 未登记** |

### 3.2 测试文件（长期回归 + 专项）

| 路径 | 变更性质 | 最近登记 SHA256（记录来源） |
|---|---|---|
| `Assets/Tests/EditMode/Village/ManagementProjectionTests.cs`（+ `.meta`） | 新增并多轮扩充：13 → **17** 例（命令拒绝零写入 / WorldBusy / 完工后施工拒绝 / 撤职累计统计 / 劳动人数口径 / 累计计时）；`Share` 基座不动 | 751 行 / 41449B / `f20a120ead170e9796b06fb9c59e3edcffc64c33c943e6b9f5ad79f036629ec5`（`command-edit-gap-review.md`，2026-10-24）；此后 `accumulated-labor-timing-fix.md` 又改 1 例（**未登记 SHA**） |
| `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（+ `.meta`） | 新增并扩展到 **11 例**：CASE-A/B/C/D、Return gating、首次导航、三表面等价、统一事件 pending 门禁（第 11 例）；内含 `SaveSlotGuard` 持久保护、TearDown 顺序修复与 34/字体硬断言 | **294535B / `cc55cb405216f43cb4b090d068f43446d4ccbfecf130c5274f4791388f80715a`**（`R3B-UI-20261025/pending-schedule-teardown-review.md`，2026-10-25；旧 10 例值 `76fc8023…` 为历史） |
| `Assets/Tests/EditMode/.../GrowthDataSeedTests.cs` | 新增（维护包）：真实 dirty 无关材质不被全局保存的回归（2/2 宿主通过） | 变化见 `R3B-Font-Recovery-20261023/maintenance-handoff.md`；**SHA 未登记** |

### 3.3 未改动的边界（复核结论）
- **未改**：`GameSpec/**`、`Docs/架构/**`、`Docs/决策记录.md`、Save DTO / SaveVersion / `PlayerCollection` 核心字段、Work / Clock / Activity / Social 领域规则、`VillageRuntimeBridge` 的 `RemovePersonAssignment` 语义（确认**仍无** `RefreshWorkIntent`）、`StaticConfigRegistry` / `BuildingConfigDatabase` 配置 Authority、任何 Scene / YAML / 字体 / Packages / ProjectSettings。
- 未新增第二套 Person / Growth / Assignment / Social / History / Save Authority；管理页只消费既有 Query 与既有合法 Command。

---

## 4. 验证矩阵（真实回执 / 历史 / 未测，逐项）

> 等级：**E1** = 本仓库内可复核的持久回执（`testResults` / 证据日志 / manifest / 文件）；**E2** = 主开发已核但执行层读取不到 Studio 私有 runtime 记录；**E3** = 仅历史版本；**未测** = 无运行回执。

| # | 范围 | 证据 | 等级 | 结论 |
|---|---|---|---|---|
| V1 | 完整 EditMode（无 filter） | 1513/1513 Passed、0 failed/skipped、2:53.578、`scope-restored`、`f116b56e` | E2（主开发已核） | Passed |
| V2 | 完整 PlayMode（无 filter） | 122/122 Passed、0 failed/skipped、1:50.7921181、`scope-restored`、`5eb0ad4c`（旧 121/121 `c048f135` 为历史） | E2（主开发已核） | Passed |
| V3 | 管理 Edit 专项 | `ManagementProjectionTests` **17/17 Passed、0 skipped**、`f8d40b09` | E2 | Passed |
| V4 | 管理 Play 专项 | 类内 11 例；最新独立 **11/11 Passed、0 skipped**、`7f600ef9`；此前独立 **10/10**、`0fd4b031` | E2（主开发已核） | Passed（第 11 例已实际运行） |
| V5 | 单槽存档精确恢复 | 2026-10-25 完整 Play 后由 `20261025T074636Z` 写回 + 回读 `13405B / 4b2b56f6…`；残留 `075219Z-residue`（13402B / `4d2562a8…`）已 `CreateNew` 留证 | E1 | Passed（当前档恢复） |
| V6 | 保护 run 完整性 | 175 目录：174 份带 status（173 `restored` + 1 `protective-snapshot`）+ 1 份无 status 的 residue 备份；无 `captured`/`restore-failed` | E1 | 状态记录无未完成；但 residue 会被 `BlockOnUnfinishedRuns()` 误判（报告已列） |
| V7 | 34 项 / 14 项 / 字体 | 恢复后 34/34；字体 `3c758e16…/14961033` | E1（阶段 1 实测） | Passed |
| V8 | NewGame 资源隔离 | CASE-C：`NEWGAME … resources=0/0 authorityInitial=0/0 batches=0 evidence=0 completed=0 clock=0`；产品侧 `SaveSystem.NewGame` 1 行修复 | E1（证据日志）+ 源码 | Passed（资源仅 Ingredients/Food） |
| V9 | 劳动中撤职 / 施工撤出计量 | CASE-D + CASE-B 扩展（撤出后仅剩余参与者计量、重新加入须新通勤到场） | E1（证据日志）+ V2 | Passed（边界见 §5.1） |
| V10 | Return gating | CASE（第 8 例）与第 11 例：出征/Return pending 期间命令与保存 `PersonNotInVillage`/`WorldBusy` 零写入；合法结算后解锁；Load 连续 | E1 + V2/V4 | Passed（第 11 例统一事件 pending 门禁已实际运行） |
| V11 | 首次导航 / 表面复位竞态 | 第 9 例：来源替换后首次真实导航提交 Management 且不被同帧复位吞掉；产品 `SyncSourceReset` 已在 `54a6c399` 实改 | E1 + V2/V4 | Passed（`FirstNavigation` 在 `1088aa8e`、`0fd4b031`、11/11、122/122 中通过；旧“产品未修”仅历史） |
| V12 | 三表面 Simulation 等价 | 第 10 例：World / Person / Management 同 W0、同目标分钟 16360，Clock/Work/Activity/Social/Resource 逐项等值 + 自然帧持续 | E1 + V2 | Passed（单世界、无 Return 混入） |
| V13 | Compiler 264 | TRX Counters `total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0`；`tests`/`validate` 退出 0；validate 8 条 warning、Mirror 未写入；494 项 `changedProtected=[]` | E2（主开发已核，`R3B-Compiler-20261025`） | **Passed** |
| V14 | 多分辨率 1920×1080 / 1280×720、滚动拖拽、物理鼠标手感 | 只有 1920 参考布局与 1280 等比推算、4 个 ScrollRect 静态结构、程序化 Raycast | 未测 | **未测** |
| V15 | 事件 pending 与 Return 叠加的独立会话语义 | 第 11 例 `PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice` 已实际运行通过（含 `EVENT-GATE` 与 `SCHEDULE-RESTORE sameInstance=True`） | E2 + V4/V2 | **Passed** |
| V16 | 第二 slot 的真实 Gateway 命令路径 | 投影已测；真实 Gateway 仍未测，见 §5.2 | 未测 | **未测** |
| V17 | Activity claim 持有者加入施工的产品行为 | 见 §5.3；未增加 busy 规则 | 未测 | **未测** |

### 4.1 保留的历史失败 / 中断（不删除、不改写、不算 PASS）
| 时间 / 记录 | 事实 | 与当前的关系 |
|---|---|---|
| `2026-10-23T12:32Z` `ManagementProjectionTests` | **12/13**（1 Failed，WindowConflict；`labor-gate-last-result.json` 13:32:23+08:00） | 旧失败；后续 13/13 → 17/17 通过，两者不互相抹除 |
| `2026-10-24T05:51Z` Play 整类 | CASE-B 1 败（`p2Arrival == null`） | 夹具前置缺陷（claim 干扰）已修；根因属**推断**（E4），未直接观测旧失败者 claim |
| `2026-10-23T15:53:02Z` | 一轮 5 次 `SAVE-CAPTURE`（首捕获 `21075B/650c45c4…`）**无任何 `SAVE-RESTORE`** | 保护只在 TearDown 的缺口；导致历史 650 档不再可回取 |
| 宿主 `6 Passed / 1 Failed`（`c875296a`） | CASE-D 绝对/日内分钟混比（`Expected 496 / Actual 14896`） | 测试口径修复后通过；产品时钟无缺陷 |
| 宿主 `edf56b77`、`f2ec8446`（各 9/10） | 首次导航“人口行 6 vs 12”等同帧复位竞态 | 由第 9 例与测试代际断言收口 |
| 宿主 `111850Z` 轮（8/9） | 合法回村后管理页不可见（Shell 0.2s 刷新复位吞掉首次导航） | 当时只改测试时序 `RefreshNow`；**2026-10-25 当前事实：产品 `SyncSourceReset` 已在 `54a6c399` 实改，首次导航随后通过**；旧“产品未修”保留为历史 |
| 宿主 `720cdd97`（3/5，2×TypeLoadException）、`273ef5ef`（4/5）、`3016ddd9`（4/5） | 字体/临时快照引用异常 → 到场分钟 495/494 口径 | 失败全部保留；后续由字体恢复与测试口径修复解决 |
| 宿主 `66dd6cb1`（5 总 / 0 过 / 5 失败）、`13f00282`（0/Unknown）、`ff384`（finished_unverified） | 字体事故期与被拒绝的启动轮 | 保留原回执，不改造为 PASS |
| 2026-10-24 `48d8820f` 轮 | 写完后 **aborted**；`pending-event-management-gate-review.md` §6 计划纠正 9 文档**实际未执行** | 保留为历史；本包（2026-10-25）只纠正 3 文档，不据该 §6 宣称已同步 |
| 外层 `interrupted` 标记 | 宿主外层运行标记 | 保留未改 |
| 历史 650 存档缺口 | `21075B / 650c45c4…` 无字节备份 | 授权内不恢复、不伪造；`13405/4b2b56f6…` 不是它 |

---

## 5. 逐项重点结论（本轮指定）

### 5.1 撤职：执行 / 贡献即停，但旧 `arrival` 只在下一边界清理
- **标签与执行即停**：`WorkDestination` 派生自“Active Project → Primary Assignment”，撤职后下一帧起 `GetWorkExecutionLabel == "空闲"`、`IsExecutingWork == false`、岗位 `BoundCount=0 / LaborCount=0`。
- **贡献计量即停**：`CompleteWorkInterval(from,to)` 用 `from` 分钟的 `ArrivedWorkers(prepare:true)` 与当前 `Assignments` 计算，撤职后下一区间不再累计；`WorkplaceEvidenceMinutes` 不回滚、不追回已结算分钟。
- **旧 `arrival` 残留**：`RemovePersonAssignment` **确认仍无** `RefreshWorkIntent`/`Leave`；撤职当刻 `IntradayWork.Arrivals` 中旧行仍在（指向旧岗位、`ReadyMinute` 原值），只在**下一次工作边界**的 `ArrivedWorkers(prepare:true)` 被清理。Play CASE-D 按此事实断言（“残留旧意图 ≠ 当前劳动”），**不宣称“撤职当刻清意图”**。
- **累计统计**：单条 Assignment 的 `WorkedMinutes/TotalExecutedDays/ContributionRemainder` 随记录删除；村庄级 `TotalTestContribution` 不回滚（Edit `RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution`）。
- **未测**：跨日 Day Tick / 下班边界 / 第二天上午补登记；Play 实时同分钟断言；若产品要求“真正即时清意图 / 追回已结算”，属 Work/Clock 核心语义变更，需另立决策，不在本包。

### 5.2 多 slot 投影已测，真实 Gateway 第二 slot 未测（不改配置 Authority）
- 已测（Edit `SameDefinitionBuildingsAndRegisteredSlotsStayIndependent`）：两个同 Def 建筑的岗位 ID 不同、实例+slot 独立映射、容量/绑定独立；向**只读投影**注入自定义 `BuildingRegistry` 后出现 `aux` slot 并保持独立。
- 未测：**Gateway 的第二 slot 命令路径**。原因（只读核对，非推测）：`Assets/Generated/StaticConfig/Databases/BuildingConfigDatabase.asset` 中 `building_farm/kitchen/workshop` 只登记 `FunctionSlotId: main`；`StaticConfigRegistry` 无公共注入/注册 API；`ManagementCommandGateway.AssignToWorkplace` 同时要求 `FunctionBindings` 命中与 `Assignments.GetWorkplaceDef` 已注册。
- 结论：在“只改测试、不改产品 / 公共 API / 配置 Authority、不用私有反射注入”的边界内**不存在合法注入点**，因此精确标为**未测**；未改 `BuildingConfigDatabase.asset`，未伪造第二 slot。

### 5.3 Activity claim 不改变 Primary 职责：保留既有语义，不自行引入 busy 拒绝
- `ManagementCommandGateway.CheckPerson` 只校验：人物存在 → `VillagePersonAvailability.Resolve`（远征 / `IsDead`）→ `CanSaveWorld`；**不调用** `GetPersonOccupancyReason`、**不调用** `ActivityRuntimeBridge.QueryClaim`。
- `GetPersonOccupancyReason` 覆盖远征 / unavailable / “Active Project 且 IsExecutingWork”，**不覆盖** Activity claim 与 Rest；Activity claim 按 Village §2.3 **不写入** Temporary Occupancy Authority。
- Activity claim 的唯一产品效果：`ArrivedWorkers` 对 `QueryClaim != null` 跳过 → 不登记工作到场；`IsExecutingWork=false`；管理投影显示“自主活动进行中”；活动结束后 `CompleteActivityBoundary` **不调用** `RefreshWorkIntent`，补登记只能等下一个工作边界。
- 因此：**Primary Assignment 仍是长期安排，claim 不改变职责**；本包**没有**也不会自行新增“claim 期间禁止派工/加入施工”的 busy 拒绝（会改变现有规则语义，需制作人裁决）。
- 未测：claim 持有者加入施工的真实产品行为（命令成功但因跳过登记而无到场；活动结束后是否/何时补登记）。已列选项 N-0（只加测试记录现状）/ N-1（仅 UI 禁用）/ N-2（Gateway 只读前置）/ N-3（活动结束补登记）/ N-4（改 Occupancy）——**均未实施**，推荐先 N-0。

### 5.4 事件 pending、多分辨率 / 滚动 / 物理体验、Compiler 264
| 项 | 现状 | 判定 |
|---|---|---|
| 事件 pending 专项 | 第 11 例已真实运行：统一事件 pending 阻断 Advance/Return，命令与保存零写入，合法选择后解锁；`EVENT-GATE` 与 `SCHEDULE-RESTORE sameInstance=True` 均通过 | **Passed**（11/11、122/122 覆盖） |
| 1920×1080 / 1280×720 实画面 | 只有 1920 参考布局与 1280 等比缩放推算；无运行期分辨率切换与文字裁切断言 | **未测** |
| 滚动拖拽 / 长列表体验 | 4 个 ScrollRect + Viewport/Mask + Content VLG/Fitter 静态结构已核；无运行期拖拽/滚轮 | **未测** |
| 物理鼠标 / 拖拽手感 / 镜头 pan·zoom / 键盘穿透 | 全部为 `EventSystem.RaycastAll + ExecuteEvents.PointerClick` 程序化点击 | **未测** |
| Compiler 264 | **264/264 Passed、0 skipped**；TRX `total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0`；validate 退出 0、8 条 warning、Mirror 未写入 | **Passed**（宿主回执，主开发已核） |

---

## 6. 风险 / Deferred / 保留边界

1. **保护只在 TearDown**：`SaveSlotGuard` 依赖 `[TearDown]`/`[OneTimeTearDown]`；原生 abort / 域重载 / Unity 释放时不会自动恢复（`15:53` 已发生一次 5 次 Capture 无 Restore）。当前通过“运行前持久快照 + 运行后精确写回”补偿，但**不是进程级保证**。
2. **产品级首次导航同帧复位已修**：`HomeProductShell.SyncSourceReset` 已在 `54a6c399` 包实改，`FirstNavigation` 在 `1088aa8e`、`0fd4b031`、11/11、122/122 中通过；本文件旧“产品未修 / 只改测试时序”只保留为历史，不再作为当前事实。
3. **`NewGame` 资源隔离仅 Ingredients/Food**：Activity / Social / Return / Event 区块的 NewGame 隔离未在本包验证。
4. **跨进程 / 世界级事务债**：stale checkpoint replay、跨进程 Return 恢复、Village/Event 世界级 Load transaction 仍未解决（沿用既有 Current 风险）。
5. **历史 650 存档**：`21075B / 650c45c4…` 在授权范围内无备份；如需取证/恢复须制作人单独授权；**不得**用 HEAD、旧阶段文件或当前 `13405/4b2b56f6…` 冒充。
6. **字体历史**：`e40b6d77…` 已备份留证；如需与新基线并存或重新定义基线，须单独授权，不能由本包顺手改基线 JSON。
7. **Issues 84 / 85 / 86 / 88 保持开放**：Commons 伪 Place、施工 Place 身份、运行期 Workplace 注册泛化、正式 Product Home World Surface 均不在 R3-B 范围。
8. **未提交未发布**：无 Commit / Push；不标 Work Package Closed / Branch Closed / Repository Closed；任务外 dirty/untracked 本轮未核验（无 git 命令），不宣称工作区 clean。
9. **Residue 备份会被守卫误判**：`20261025T075219Z-residue` 位于保护根内且 manifest 无 `status`，`SaveSlotGuard.BlockOnUnfinishedRuns()` 会将其计为 `unfinished`（`(unreadable)`），阻断后续测试；本包只读核对，未移动/修改备份，处置见 2026-10-25 报告。

---

## 7. 剩余明确必需且可安全自办的最小后续动作

> 技术问题在既有范围内自办，不包装成制作人审批；只有改变权威/规则/公共契约才升级决策。

> 已完成的后续：Compiler 264（宿主 2026-10-25，264/264、0 skip）与事件 pending 第 11 例（11/11、122/122）已真实通过；下表只列仍未完成项。

| # | 动作 | 性质 | 前置 |
|---|---|---|---|
| S1 | 视觉/输入验收包：1920×1080 与 1280×720 实画面、滚动拖拽、物理鼠标/滚轮、镜头 pan/zoom、14 个缺字运行期补字 | 测试/人工体验 | 无代码前置 |
| S2 | claim 持有者加入施工的 N-0 记录型用例（只记录真实命令结果、是否登记到场、活动结束后是否补登记），结果交主开发决定是否升级产品包 | 测试 | 无 |
| S3 | 撤职 Play 追加“同一分钟不新增证据 + 已结算分钟不追回”的实时断言；跨日/下班边界补测 | 测试 | 无 |
| S4 | 第二 slot 真实 Gateway 命令路径：仅在获得配置 Authority / 公共 API 授权后另立包；当前不改 `BuildingConfigDatabase.asset`、不伪造 | 测试/产品 | 需授权 |
| S5 | 处置 `20261025T075219Z-residue` 无 status 导致 `BlockOnUnfinishedRuns()` 误判：由后续包在保留备份前提下补合法终态/移出保护根；本包不移动、不改 | 基础设施 | 需单独授权 |
| S6 | 关注 `SaveSlotGuard` 进程级缺口：评估“运行前持久快照 + 运行后精确写回”脚本化，或在 TearDown 之外增加 out-of-process 兜底 | 基础设施 | 需单独授权与设计，不在本包 |
| S7 | 任务外 dirty/untracked 与 index 的定稿核对（Git 只读），以及提交/发布授权 | Git | 需制作人/主开发 Git 授权 |

---

## 8. 本阶段文档同步清单（实际写入）

| 路径 | 变更 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md` | **新增**：本索引（代码差异 + SHA 口径 + 验证矩阵 + 逐项结论 + 风险 + 后续） |
| `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md` | **新增**：日期化执行记录（原方案 / 本次变化 / 否决 / 确认 / 验证 / 未解 / 下一步） |
| `Docs/DevelopmentSpecs/README.md` | 同步顶部状态与索引（R3-B 已实施 / PARTIAL / 未发布） |
| `Docs/项目状态快照.md` | 同步顶部状态、主要领域 Current（管理 Surface）、验证基线与下一步；删除“R3-B 未启动”的当前口径 |
| `Docs/项目日志.md` | 新增 2026-10-24 条目（实现 / 验证 / 保护 / 事故 / 缺口） |
| `Docs/文档地图.md` | 同步顶部导航状态（R3-B 实施事实入口） |
| `Docs/开发验收交接.md` | 顶部状态 + Handoff81（R3-B 事实、Git 未提交、验证、遗留） |
| `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md` | 末尾追加 2026-10-24 状态附记（历史正文不改写） |
| `Docs/DevelopmentSpecs/2026-10-10_RRPG_V17-R3-A_Person-Detail_Execution-Record.md` | 状态行后追加 2026-10-24 附记（历史正文不改写） |

未改：`GameSpec/**`（规则 Authority）、`Docs/Architecture/**`、`Docs/决策记录.md`、`Docs/问题清单.md`、`README.md`、`AI_CONTEXT.md`、`.clinerules.md`、`Docs/AI开发执行规则.md`、`Docs/AI多Agent开发协作协议.md`、任何代码 / 测试 / 资产 / 配置 / 基线 / manifest / 证据日志 / 存档。

> **2026-10-25 文档状态**：本包只纠正 `final-self-audit.md`、本执行记录、`full-play-postflight.md` 3 份文档，并新增 `R3B-UI-20261025/document-correction-review.md`；上表另 6 份索引文档**本包未更新**，按计划由下一包同步。`48d8820f` 轮计划纠正 9 文档**未执行**，保留为历史。

---

## 9. 诚信与边界声明

- 本文件不新增任何测试结果：V1/V2/V3/V4/V13/V15 为主开发已核的宿主回执（E2，本执行层读不到 Studio 私有 runtime 记录，按任务登记，不冒充复核）；其均为**引用**，不是本包运行。
- **本包（2026-10-25 文档纠正）边界**：未运行 Unity / tests-run / Test Runner、未刷新、未保存、未 `rql-mutate`、未 Shell、未 Git 写、未改产品 / 测试 / 资产 / manifest / 证据日志 / 存档 / 备份；只读核对了 TRX Counters、Compiler 日志、SaveSlotGuard 枚举源码与 residue manifest。
- **TRX SHA 未重算**：本层无独立哈希工具；`587d9dd1…b0f5` 是 `R3B-Compiler-20261025/REPORT.md` 记录值，不冒充本包重算。
- 本包**未能**独立读取的项已逐处标注（Studio 私有路由、当前树代码 SHA、Git index/status）；不把“未读取”写成“已验证”。
- **字体已恢复 ≠ 历史 650 存档已恢复**；**当前档已恢复 ≠ 历史 650 已恢复**；两者分列，不互相冒充。
- **本轮不提交、不发布、不关闭整包**；R3-B 为 Implemented / Verification PARTIAL，Compiler 264 已通过但整包仍不 Accepted。本文件需主开发专业核验，不代表制作人批准。

---

## 10. 2026-10-26 最终当前事实定稿（只读核验；覆盖上文相应旧数字）

- 时刻：**2026-10-26T07:28:21Z**（本机 `2026-10-26T15:28:21+08:00`，UTC+8）。本阶段只做自审与证据定稿：未跑 `tests-run`、未 Refresh、未 PrefabStage、未 `rql-mutate`、未保存、未改代码/测试/Prefab/字体/存档/manifest/基线，未 Git 写。
- 唯一 Editor 操作：`rql-query` 只读 + `script-execute` 只读 C#（读文件/重算 SHA256/`IsDirty` 只读判定/反射读 `IsRunActive`、`GetSavePath`/目录与 manifest 统计/读 `DateTime`）。
- 详细证据与原文摘要：`Development/StudioRuns/R3B-UI-20261026/final-current-evidence-review.md`。
- 本阶段实际写入：本文件（顶部块 + §1 指引 + 本 §10）、执行记录（顶部指针 + §9）、新增证据索引；写后扫描（排除 `.git/Library/Temp/Logs/obj`）另见 Unity 插件自动刷新的 `UnityProject/RRPG1.0game/.com-unity-codely.json` 心跳（非本包刻意写入，不在 34 项基线内）。写后 Editor 仍空闲、场景 `dirty=False`、字体 9 子资产 0 dirty、单槽 `13405-4b2b56` 未变。

### 10.1 当前回执（与上文旧值的关系）

| 项 | 2026-10-26 当前值 | 与上文关系 |
|---|---|---|
| 管理 Play 整类 | **12/12 Passed、0 skipped、`11.4823431`、`scope-restored`，run `4aa4ffda…`**（主开发已核） | 覆盖 §1/V4 的 11/11；`testResults` 12 条全 Passed 为本层核对 |
| 管理 Edit 整类 | **18/18 Passed、0 skipped、`1.2183834`，记录 `99dc1dfe`**（主开发已核）；claim 单例 `41a9596f` 1/1 | 覆盖 §1/V3 的 17/17 |
| 人口行标题 | Label **28→32**；`ROW-TITLE-RENDER`×6、`ROW-TITLE-VISIBLE rows=6 checkedRows=6`；人口 PNG 六行可见（主开发目视 + 本层日志/截图） | 新增；旧 b17 rejected 为字体 pending，**非断言失败** |
| Compiler 264 / 完整 Edit 1513 / 完整 Play 122 | **前阶段真实通过，未以最新 Prefab/源码重跑** | V1/V2/V13 的版本范围限定，仍为 Passed（历史门禁） |
| 字体 | `14961033B / 3c758e16…c404854`；磁盘 mtime `2026-10-10T13:23:55Z`；内存 9 子资产 **0 dirty** | 与 §2.3/§4 V7 一致 |
| 34 项 | `total=34 / same=34 / diff=0 / missing=0`（本轮独立重算） | 与 V7 一致 |
| 单槽 | `13405B / 4b2b56f6…d6aa`，mtime `2026-10-26T07:25:56.5055Z`，无 `.tmp` | 与 V5 一致 |
| guard | **202** run：`restored=201` + `protective-snapshot=1`，无 manifest 缺失、无非白名单状态；最新 `20261026T072555Z restored` | 覆盖 §1/V6 的 175 run（同口径，只是数量增长） |
| 工程状态 | `isPlaying/isPaused/isCompiling/isUpdating=False`、`SampleScene dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080` | 现场干净 |

### 10.2 规格 §4–§8 逐项状态（已实现 / 已验证 / 未覆盖 / 取消）

- **§4 集中管理**：三子页、同 Roster 比较、筛选与选人进详情、实例岗位（BuildingInstanceId + FunctionSlotId）容量/绑定/劳动/现场分列、施工行进度与工人、Completed 动态岗位派工/撤职、施工只走 `TryAddConstructionWorker/TryRemoveConstructionWorker`、静态兼容岗位（`ws_farm_01`）可见可撤、同一 Selection 跨页、名额与劳动数独立、AlreadyAssigned 不做原子改派、撤职删记录且累计统计不回滚、显式保存显示 bool、busy 时命令与保存禁用——**全部已实现并已验证**（12 例 Play + 18 例 Edit）。资源页批次/供餐细节：**已实现/部分验证**（首片后置不阻塞）。**claim 持有者加入施工：已实现 + 已验证**（`41a9596f` 单例 + 18/18 整类；464/465/480/481 时间线）。**第二 FunctionSlot 真实 Gateway：未覆盖**（配置只登记 `main`，无合法注入点；与单槽存档无关）。拒绝实现清单：**已遵守**。
- **§5 页面/UI/路由**：唯一 World/镜头/Selection、原 Surface 替换白模、来源替换取消选择、Editor 可编辑 Prefab 固定结构、内部页不 Pause/不改 Speed、关闭回 World 保留有效选择、ESC 与关闭同回调、全屏 Blocker + World 阻断、TopHUD 可操作、远征仍走既有 gating、pending/Return 优先级保留——**已实现并已验证**（程序化 Raycast）。**键盘 / 镜头 / 物理鼠标穿透：未覆盖**。
- **§6 实施顺序与验收路径**：顺序已按原文执行；产品验收卡第 6/7/8/9/10 条**已验证**（CASE-A/B/C/D、Return gating）；第 1–5 条属 R3-A，沿用既有回执未重跑；**手工点击手感未覆盖**。
- **§7 风险/未决/Stop**：管理命令前置（存在→可用性→世界锁）**已实现并已验证**；撤职“即时”边界按事实断言（**已实现/已验证，边界保留**）；Growth/Social/死者档案/GameSpec 滞后/Save Ownership/世界级 Load transaction **保持既有缺口，未在本包处理**；字体保护**成立**；**1280 由制作人 `01a53eb8` 取消（取消 ≠ 通过）**。
- **§8 测试矩阵**：Edit 管理命令 18/18、Play 真实入口 12/12、劳动与资源、Simulation 对照、Save/Continue/NewGame、远征/事件 gating——**当前已验证**；模态/布局**部分验证**（1920 布局、遮挡、滚动可达分支；真实 overflow 滚动/拖拽/滚轮/物理鼠标未覆盖）；人物只读投影、关系/历史、成长——**沿用前阶段回执，本包未重跑**。

### 10.3 当前代码差异文件与完整 SHA（只读重算）

完整表（含 `.meta`、DLL、Prefab 行模板计数）见 `final-current-evidence-review.md` §6。关键值：

- `ManagementView.prefab` **282326B / `e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba`**（`1072×32`×4、`1072×28`×0）
- `ManagementShellPlayTests.cs` **328163B / `a9ae921dbe12cf3d6cd02d83b63d2816390ea9906b92395862035e3042d64499`**
- `ManagementProjectionTests.cs` **54062B / `f48f8f7d0cb9bfe299c3c8cbc1a6814540bd178495e632f907eedf21e9f16e0b`**
- `HomeProductShell.cs` **16082B / `7da8fbb06aac044e37acbd21377ff7e32f4aa1b99659e4f18bdf66bbbd8f2f66`**（含 `SyncSourceReset` 产品修复；上表 §3.1 的 15497B/`7e9901d7…` 为修复前历史值）
- `SaveSystem.cs` **45001B / `97f48237e42f05c7d8768ee09a09b6f7a295189afb47329afc3e16268c8d8045`**
- `BattleStage1.PlayModeTests.dll` **474624B / `5bbdfdc8…6792e`**（mtime 晚于源码）；`VillageStage1.Tests.dll` **153088B / `9d09dabb…b69da`**

### 10.4 风险与后续

- **README 口径**：264/1513/122 只能写“前阶段真实通过”，不得写“当前 12 例全绿的依据”。
- **PrefabStage 自动回写（历史异常，保留）**：`282097B / ade4c7e1…` → `282326B / 5cf61738…`（+229B）；**只相关性、根因未确定**，不能写“插件已确定根因”，也不能断言“仅 4 个数字变化、其余序列化形态未变”（缺逐字节对照）。当前已由 `03:26:15Z` 的 4 处 Label 修复变为 `e789307a…`。
- **未覆盖清单**（不升级为需批准/不新增产品规则）：第二 FunctionSlot 真实 Gateway、真实 overflow 滚动、物理鼠标/拖拽/滚轮、其他业务行标题截图、Chevron、Detail 单行 Ellipsis、历史 650 档。
- **下一安全闭环建议**：真实 overflow 滚动可用**合法世界增量**构造（推进/新增真实人物与建筑使 `contentHeight > viewportHeight`，再用真实滚动接口验证末端可达），**不伪造 RectTransform、不手改 Prefab、不注入假数据**——留给后续独立包，本包不实现；其余见本文件 §7 与证据索引 §10。
- **状态**：Implemented / Verification PARTIAL；需完整可审交付，**不 Closed / 不 Committed / 不 Published / 不 Accepted**，不代表制作人批准。

---

## 11. 2026-10-26 详情修复后整类 13 例收尾（阶段 2/2，只读核对 + 文档）

- 时刻：**2026-10-26T08:51–08:56Z**（本机同日 16:51–16:56 +08:00）。本阶段**未运行 tests-run**、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save / SaveChanges、未 SetDirty / ClearDirty、未 `GetPreferredValues` / `ForceMeshUpdate`、未 Shell / Git 写、未改代码 / 测试 / Prefab / 字体 / 存档 / manifest / 基线；只做 `rql-query` 只读、只读 C#（哈希 / 反射 / 目录统计 / PNG 头）、本地 `read` / `grep` / `read_image`。
- 13 例结果来源：`testResults` **13 条全 `Passed` / `message=null`**（本机可核对）；宿主 runId / run 级总时长 / skipped 字段未读到，**不编造**。
- 逐例 `[SAVE-RESTORE]` runId（主证据日志第 3245–3426 行段）：`085318Z`、`085319Z`、`085320Z`、`085322Z`、`085322Z-2`、`085323Z`、`085324Z`、`085325Z`、`085325Z-2`、`085326Z`、`085327Z`、`085327Z-2`、`085329Z`，全部 `matches-before=True`、sha `4b2b56f6…d6aa`；两例 `[SCHEDULE-RESTORE]`（`085323Z`、`085327Z`）`restored=true sameInstance=True`。
- 详情修复的代码 / Prefab 变更与实测、原规格逐项状态、当前 / 历史门禁、风险 Deferred、可复现入口与下一步：见新增 **`Development/StudioRuns/R3B-UI-20261026/reviewable-delivery-index.md`**（本文件不重复整表）。
- **两次边界违规原文保留、不洗白**：① 旧 `4fbf7754` 轮 `EditorUtility.ClearDirty`；② 上一包（详情修复）`AssetDatabase.SaveAssets()` 越界调用（原文见 `R3B-UI-20261026/workplace-detail-readability-review.md` §6.3，已核无额外写入，但调用本身仍按违规记录）。本阶段未重复。
- 字体 / 34 项磁盘 hash 未变只证明**观察面**未被写，**不推断全机零副作用**；本阶段自身 2 次只读 `script-execute` 探针各抛一次异常（`NullReferenceException` / `IndexOutOfRangeException`），仅为探针自身错误，Console 无项目 / 测试程序集错误，如实记录。
- 文档面：本文件顶部新增块 + 本节；执行记录追加 §10；6 份索引文档只加 2026-10-26 当前附记（不批量重写历史）；新增交付索引 1 份。

---

## 12. 2026-10-26 全部必要明细修复后整类 13 例（阶段 2/2 最终同步，只读核对 + 文档）

- 时刻：**2026-10-26T10:27–10:33Z**（本机同日 18:27–18:33 +08:00）。本阶段**未运行 tests-run**、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save / SaveChanges / SaveDirtyScenes、未 SetDirty / ClearDirty、未 `GetPreferredValues` / `ForceMeshUpdate`、未 Shell / Git 写、未改代码 / 测试 / Prefab / 字体 / 存档 / manifest / 基线；只做 `rql-query` 只读、只读 C#（哈希 / 反射 / 目录统计 / PNG 头 / 日志读取）、本地 `read` / `grep` / `read_image`。
- 13 例结果来源：`testResults` **13 条全 `Passed` / `message=null`**（本机可核对，方法耗时 547–1521 ms、合计 11 754 ms）；宿主 runId / run 级总时长 / skipped 未读到，**标“待主开发补”，不编造**。
- 逐例 `[SAVE-RESTORE]` runId（主证据日志第 3458–3640 行）：`102923Z`、`102924Z`、`102925Z`、`102927Z`、`102927Z-2`、`102928Z`、`102929Z`、`102929Z-2`、`102930Z`、`102931Z`、`102932Z`、`102933Z`、`102934Z`，全部 `matches-before=True`、sha `4b2b56f6…d6aa`；**3 组 `[SCHEDULE-RESTORE]`**（3541 / 3569 / 3599）`restored=true sameInstance=True`。
- 四类业务行真实渲染（`real-scroll-evidence.log` 第 89–126 行）：岗位 6（`checkedChars=51`）、施工 1（3 行、`checkedChars=58`）、资源 6（`checkedChars=41`）、人口 6（`checkedChars=40/37`）；`[REAL-SCROLL-COVERAGE]` 四类全 True，`Resource=NotCoveredByThisCase` 本轮 0 次；职责 `1044>650`、资源 `834>650`，真实滚轮 + 多轮拖拽末行可达。
- 保护面（运行后 10:30:31Z 只读实测）：字体 `14961033B / 3c758e16…` + 9 子资产 0 dirty；34/34；单槽 `13405B / 4b2b56f6…`（`10:29:35.176Z` 回读）；guard **244 = 243 restored + 1 protective**，无 unfinished；场景 `dirty=False roots=5`、`PrefabStage=null`、`IsRunActive=False`、`Screen=1920x1080`、Console 0 异常。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**
- 逐项矩阵、当前 / 历史门禁、可复现入口与必要下一步见 **`Development/StudioRuns/R3B-UI-20261026/all-details-delivery-review.md`** 与 **`reviewable-delivery-index.md`**（本文件不重复整表）。
- 文档面：本文件顶部新增最终块 + 本节；执行记录追加 §11；6 份索引更新 2026-10-26 当前附记 / 追加同日最终附记（历史正文不改写）；新增最终交付审阅 1 份。

---

## 13. 2026-10-27 ESC 采样修后第 16 例集成（阶段 2/2 最终同步，只读核对 + 文档）

- 时刻：**2026-10-27T07:06–07:08Z**（本机同日 15:06–15:08 +08:00；宿主运行段 `07:07:38.4677317Z–07:07:53.5415737Z`）。本阶段**未运行 tests-run**、未 Refresh、未 PrefabStage、未 `rql-mutate`、未全局 Save / SaveChanges / SaveDirtyScenes、未 SetDirty / ClearDirty、未 `GetPreferredValues` / `ForceMeshUpdate`、未 Shell / Git 写、未改代码 / 测试 / Prefab / 字体 / 存档 / manifest / 基线；只做 `rql-query` 只读、只读 C#（哈希 / 反射 / 目录统计 / 日志读取）、本地 `read` / `grep`。
- 16 例结果来源：`testResults` **16 条全 `Passed` / `message=null`**（本机可核对，方法耗时 555–1446 ms、合计 **14 634 ms**）；宿主 runId / run 级总时长 / skipped 未读到，**标“待主开发补”，不编造**。
- 逐例结构（主证据日志 L4139–L4374）：16 组 `[ENTRY]`、16 组 `[SAVE-CAPTURE]`、**16 组 `[SAVE-RESTORE] matches-before=True`**（runId `070738Z`…`070752Z`，含 `-2`）、**4 组 `[SCHEDULE-RESTORE] sameInstance=True`**；第 16 例 9 行 `[ESC-FRAME]`（`PRE / MANAGEMENT-CLOSED / NEGATIVE-FRAMES=3 / PERSON-CLOSED / BUILD-POINTER-NEGATIVE / BUILD-NEGATIVE / BUILD-GATE / END / CLEANUP`）；`[FONT-POST] unchanged=True`、`[BASELINE-POST] 34/34`。
- ESC 单例 `cc3adfc5…`：**1/1、0 skipped、2.0916182 s、scope-restored**（主开发原始核）；管理 Edit `99761145…`：**19/19、0 skipped、1.1578404 s、scope-restored**（当前 Edit 源码 19 个 `[Test]` / `70548B / 64c8c124…` 未变）。
- 保护面（运行后 07:08:26Z 只读实测）：字体 `14961033B / 3c758e16…` + 9 子资产 0 dirty；34/34；单槽 `13405B / 4b2b56f6…`（`07:07:53.251Z` 回读，无 `.tmp`）；guard **293 = 292 restored + 1 protective**（最新 `20261027T070752Z`，无 unfinished / noManifest / pendingRestore）；`SampleScene dirty=False roots=5`、`PrefabStage=null`、`isPlaying=false`、`isCompiling=false`、`1920×1080`、Console 0 异常。**磁盘 hash 未变只证明观察面，不推断全机零副作用。**
- 逐项矩阵、当前 / 历史门禁、可复现入口与必要下一步见 **`Development/StudioRuns/R3B-UI-20261027/final-professional-handoff.md`**、`current-contract-evidence.md` §0、`final-scope-and-contract-review.md` §10 与 `reviewable-delivery-index.md` §9（本文件不重复整表）。
- 文档面：本文件追加本节；执行记录追加 §13；交付索引追加 §9；6 份索引各加 2026-10-27 当前附记；新增最终专业交接 1 份。
- **两次边界违规原文保留、不洗白**：旧 `4fbf7754` 的 `EditorUtility.ClearDirty`；详情修复包的 `AssetDatabase.SaveAssets()` 越界调用。本阶段未重复。
- **ESC 诚实边界**：第 16 例证明产品层真实 `LateUpdate` 采样 / 一次消费 / 阴性帧 / 建造门禁；**不证明** OS 物理 ESC、`Input.GetKeyDown` 引擎实现、物理手感、IME / DPI、Player 构建内行为。1280 已取消；650 档保留；Deferred 保留。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。
- **两次边界违规原文保留、不洗白**：旧 `4fbf7754` 的 `EditorUtility.ClearDirty`；详情修复包的 `AssetDatabase.SaveAssets()` 越界调用。本阶段未重复。
- 需主开发专业核验，**不代表制作人批准**；R3-B 仍 **Implemented / Verification PARTIAL / Not Committed / Not Published**。
