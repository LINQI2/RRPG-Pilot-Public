# R3-B 修后核心自审与保护证据定稿索引（2026-10-26，阶段 1/1）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 核验时刻：**2026-10-26T07:28:21Z**（本机 `2026-10-26T15:28:21+08:00`，`China Standard Time`，UTC+8；由只读 `script-execute` 读 `DateTime` 取得）。
- 本包性质：**修后核心自审 + 保护证据定稿**。未运行 `tests-run`、未改代码 / 测试 / Prefab / Scene / 字体 / 配置 / 存档 / guard manifest、未 `AssetDatabase.Refresh`、未打开 PrefabStage、未 `rql-mutate`、未全局 `SaveAssets/SaveChanges/SaveDirtyScenes`、未写 Preferences、未启停 Editor、未 Git 写、未跑 Compiler / 完整 Edit / 完整 Play、未访问正式工程。
- 本包实际写入：**3 个文档**——本文件（新增）、`Development/StudioRuns/R3B-UI-20261024/final-self-audit.md`（追加 2026-10-26 最终当前事实 + §10）、`Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md`（追加 §9）。未更新 6 份索引文档（避免膨胀），未写 GameSpec / Authority / manifest / 备份。
- 状态口径：**R3-B = Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；**不写 Closed / Committed / Published / Accepted**，整包不因本轮收尾被宣布通过。

---

## 1. 只读实核：Pilot 空闲 / 场景干净 / 字体 / 34 项 / 单槽 / guard

全部数值为本轮**只读 C#**（读文件 + 重算 SHA256 + `EditorUtility.IsDirty` 只读判定 + 目录/manifest 统计）与 `rql-query` 实测，不是转述。

| 项 | 实测值（2026-10-26T07:2xZ） | 判定 |
|---|---|---|
| Unity / Editor | `2022.3.62t9`；`isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲，无播放 / 编译 / 导入 |
| 场景 | 仅 `Assets/Scenes/SampleScene.scene`：`dirty=False`、`roots=5`、`loaded=True` | 干净 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage()==null` | 未打开（本包不打开） |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()` = **False** | 无活动 / 残留运行 |
| Screen | `1920x1080 / Windowed` | 未切 1280（1280 已由制作人 `01a53eb8` 取消） |
| `testResults` | **12 条**，全部 `ManagementShellPlayTests`、全部 `Passed / message=null` | 与整类 12 例回执一致（见 §2） |
| 受保护字体（磁盘） | `14961033B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`；文件 mtime `2026-10-10T13:23:55.9520319Z`、`.meta` mtime `2026-10-10T12:10:57.0892496Z` | 与基线同值，今日未被写 |
| 字体（内存 / 子资产） | `LoadAllAssetsAtPath` = **9** 个子资产、`dirtySubassets=0`：`TMP_FontAsset`(dirty=False)、`Material`(False)、`Texture2D` ×7（`Atlas`…`Atlas 6`，均 False）；`atlasTextures.Length=8`（**1 个 null 空闲槽**）、`characterLookupTable.Count=644`、`atlasPopulationMode=Dynamic` | **9 subassets clean、0 dirty**；未 Save / 未 SetDirty / 未重导入 |
| 34 项 workspace 基线 | `R3B-20261022/workspace-baseline.json`（`complete=true`）逐项 bytes+SHA256：`total=34 / same=34 / diff=0 / missing=0` | **34/34 一致** |
| 单槽存档（`SaveSystem.GetSavePath()`） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`；`exists=True`、**`13405B / 4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T07:25:56.5055000Z`、`.tmp=false` | `13405-4b2b56`，为本轮整类收尾的 guard 恢复时刻 |
| guard 根（`R3B-SaveProtection-20261024`） | **202** 个 run 目录：`restored=201`、`protective-snapshot=1`、**noManifest=0**、非白名单状态 **0**（无 `captured` / `restore-failed` / `unfinished` / `unknown`） | 全部 run 状态合法，无未知执行残留 |
| guard 最新 run | `20261026T072555Z`：`restored`，manifest mtime `2026-10-26T07:25:56.5060019Z` | 与单槽 mtime、12 例收尾相接：真实恢复并回读 |
| Git（只读 `.git/HEAD` + ref） | branch `ai/rrpg-r3b-population-pilot`、HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030` | 未变；**本包未执行任何 Git 命令、无 Git 写** |

> 与 2026-10-25 报告（175 run / 176 run）相比，guard 数量增长来自后续真实运行；本轮只读统计，**未移动、未改写任何历史 run 或其 manifest**。

---

## 2. 本轮 12 例整类回执与保护证据原文摘要

### 2.1 整类回执（主开发已核 + 本机可核对部分）

- 主开发已核宿主回执：run **`4aa4ffda-3730-41be-ad11-2f9550ff31bc`**，**12/12 Passed、0 skipped、`11.4823431` s、`scope-restored`**。
- 本执行层可核对部分：`testResults` **12 条全部 `Passed / message=null`**（`CloseButton…1099ms`、`ExpeditionReturn…1081`、`ExplicitSaveContinue…1490`、`FirstNavigation…599`、`Management_ProductEntry…595`、`ManagementConstructionCommands…741`、`ManagementLayout_At1920…839`、`ManagementModal…545`、`ManagementWorkplaceCommands…899`、`PendingEvent…606`、`SurfaceEquivalentSimulation…1328`、`WorkplaceRemovalDuringLabor…776`）。
- 证据日志：`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，**665424B / 3004 行**，SHA256 `70d48944b3c0eb27e502313689e1690bb767ab57643548b2e7f9331220b21383`，mtime `2026-10-26T07:25:56.8038563Z`；本轮为 07:25:45–07:25:56 追加段，历史行原样保留。
- 限制：宿主 runId / run 级 `11.4823431` s / `scope-restored` 属主开发转述，本层读不到 Studio 私有 runtime；本层只以 `testResults` 12 条 + 日志 + guard manifest 作为可核对佐证，**不把转述冒充本机实测**。

### 2.2 12 组 `[SAVE-RESTORE]`（逐条原文摘要，全部 `matches-before=True`）

| # | 时间（UTC，日志行） | runId | sha256 |
|---|---|---|---|
| 1 | 07:25:47.001（2840） | `20261026T072545Z` | `4b2b56f6…d6aa` |
| 2 | 07:25:48.086（2855） | `20261026T072547Z` | `4b2b56f6…d6aa` |
| 3 | 07:25:49.576（2874） | `20261026T072548Z` | `4b2b56f6…d6aa` |
| 4 | 07:25:50.175（2886） | `20261026T072549Z` | `4b2b56f6…d6aa` |
| 5 | 07:25:50.770（2890） | `20261026T072550Z` | `4b2b56f6…d6aa` |
| 6 | 07:25:51.510（2922） | `20261026T072550Z-2` | `4b2b56f6…d6aa` |
| 7 | 07:25:52.344（2927） | `20261026T072551Z` | `4b2b56f6…d6aa` |
| 8 | 07:25:52.895（2931） | `20261026T072552Z` | `4b2b56f6…d6aa` |
| 9 | 07:25:53.794（2944） | `20261026T072552Z-2` | `4b2b56f6…d6aa` |
| 10 | 07:25:54.397（2960） | `20261026T072553Z` | `4b2b56f6…d6aa` |
| 11 | 07:25:55.729（2986） | `20261026T072554Z` | `4b2b56f6…d6aa` |
| 12 | 07:25:56.506（3002） | `20261026T072555Z` | `4b2b56f6…d6aa` |

原文形态（示例，第 12 组）：

```
[SAVE-RESTORE] runId=20261026T072555Z existed=True sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa
  matches-before=True backup=…\R3B-SaveProtection-20261024\20261026T072555Z\original-save.bin manifest=…\manifest.json
```

结论：**12 例逐例捕获→恢复→回读通过，单槽内容始终是原始 `13405-4b2b56`，没有任何一例把测试写入字节留在用户档。**

### 2.3 两例 `[SCHEDULE-RESTORE]`（原文摘要）

| 日志行 | 时间（UTC） | 用例 | 原文 |
|---|---|---|---|
| 2923 | 07:25:51.511 | CASE-B 施工计量 | `restored=true sameInstance=True points=[420,1020,1200] commute=15 repeat=30 durations={Rest=60,Drink=60,Talk=60,VisitSick=90,Help=90}（只读校验）` |
| 2961 | 07:25:54.398 | 统一事件 pending 门禁 | 同上逐字一致 |

即两例的“同一实例 + 机会点逐项 + commute + repeat + 五项时长”全部恢复到用例前捕获值，且恢复发生在 `SAVE-RESTORE` 之后（存档兜底不因日程断言失败被短路）。

### 2.4 本轮 `FONT-POST` / `BASELINE-POST`（原文摘要）

```
[FONT-POST]     sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854 unchanged=True        （3003，07:25:56.647）
[BASELINE-POST] total=34 same=34 changed=0 problems=0 all-match                                            （3004，07:25:56.803）
```

与本轮独立只读重算（§1：字体 `3c758e16…/14961033`、34/34）一致，**不是仅凭日志自述**。

### 2.5 旧 `b17cd5c1…` 的 rejected 口径（不当作断言失败）

- 该轮结论为 **rejected**、错误为 `Font already has pending edits`，**测试未发送**，因此**不是断言失败、不是产品缺陷、不是 12 例失败**。
- 本轮只读门禁复核：字体磁盘字节 = 基线、**内存 9 个子资产 0 dirty**，说明当前不存在 pending 编辑；但本包仍**不做任何保存 / 清 dirty / 重导入**，若真有 pending 只会如实发现。

---

## 3. 本轮 `ROW-TITLE` 行标题真实渲染证据（人口页）

日志：`Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/layout-input-evidence.log`，**41498B / 230 行**，SHA256 `160b14733ee6b191d288205732e93a5c914a728ea30cae4469e8323a272a0ae3`，mtime `2026-10-26T07:25:52.3431736Z`；本轮为第 182–230 行，历史 1–181 行原样保留。

逐行原文摘要（第 202–207 行；均 `PersonRowTemplate(Clone)`、`rect=1072×32 @ (0,-32)`、`preferredH=30.41`、`alpha=1`、`crAlpha=1`、`cull=False`、`overlapsDetail=False`、`rowFullyInsideViewport=True`）：

| 日志行 | text | chars | visible | rendered | titleInk (x,y,w,h) |
|---|---|---|---|---|---|
| 202 | 林恩 · profession_test_c | 22 | 20 | 247.2239×30.408 | (80.00, 672.80, 247.22, 30.41) |
| 203 | 苏晚 · profession_test_a | 22 | 20 | 248.3359×30.408 | (80.00, 580.80, 248.34, 30.41) |
| 204 | 邱霜 · profession_test_b | 22 | 20 | 249.4916×30.408 | (80.00, 488.80, 249.49, 30.41) |
| 205 | 青鹿 · 英雄 | 7 | 5 | 114.4063×30.408 | (80.00, 396.80, 114.41, 30.41) |
| 206 | 韩昭 · profession_test_e | 22 | 20 | 248.1463×30.408 | (80.00, 304.80, 248.15, 30.41) |
| 207 | 魏安 · profession_test_d | 22 | 20 | 249.5317×30.408 | (80.00, 212.80, 249.53, 30.41) |

第 208 行：`[ROW-TITLE-VISIBLE] … rows=6 checkedRows=6（TMP 可见字符/渲染尺寸/行高/alpha/cull/与 detail 不重叠/视口裁切，非仅字符串）`。

- 本轮 3 张 1920 截图（日志 187 / 209 / 219 行 `size=1920x1080`，新时间戳 `20261026T072552Z`，未覆盖历史图）：population `214710B / 4677cb246e56d5907601ffe8737a6baa5f732036a327ae9578ea85db13f23b6f`、workplaces `183296B / fb13ffaa9b7be82618077042df1be5b38820190afc17f1a9f728df40998550be`、resources `176231B / f6e63cc1d11392cba862ea2348637ca6fb635692f1f46dcf113d2d895234be2e`。
- 人口标题 **28→32 为真实修复**（Prefab 当前 `1072×32` 4 处、`1072×28` 0 处），并在最新 12 例整类运行中由逐行渲染数据 + 截图双重证明可见；主开发已亲自查看 `071915Z` 人口 PNG 六行姓名职业可见。

---

## 4. 版本范围（禁止把历史门禁说成“当前 12 例全绿”的依据）

| 门禁 | 结果 | 版本范围 |
|---|---|---|
| Compiler | `264/264 Passed、0 skipped` | **前阶段真实通过**（2026-10-25 宿主 `R3B-Compiler-20261025`）；**未以最新 Prefab / 最新 12 例源码重跑** |
| 完整 EditMode | `1513/1513 Passed、0 skipped`、`f116b56e` | **前阶段真实通过**；未以最新测试源码重跑 |
| 完整 PlayMode | `122/122 Passed、0 skipped`、`5eb0ad4c` | **前阶段真实通过**；未以最新 Prefab / 12 例重跑 |
| 管理 Edit 专项 | 最新 **`99dc1dfe`：18/18 Passed、0 skipped、`1.2183834` s**（主开发已核）；此前 `17/17 f8d40b09` | 当前 18 例范围（含第 18 例 claim 施工） |
| 管理 Play 专项 | 最新整类 **`4aa4ffda…`：12/12 Passed、0 skipped、`11.4823431`、`scope-restored`**；此前 `11/11 7f600ef9`、`10/10 0fd4b031` | 当前 12 例范围（含 1920 布局 + 姓名可见断言） |

口述必须保留的限定：**264 / 1513 / 122 是“前阶段真实通过”，并未因本轮收尾而重跑全集，也不覆盖最新 Prefab / 最新测试源码差异**；本包不因收尾重复全集，也不据此宣称整包全绿。

---

## 5. 规格 §4–§8 逐项状态（已实现 / 已验证 / 未覆盖 / 取消）

依据 `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md` §4–§8 原文逐项对齐。“已验证”指有当前版本运行回执；“已实现”指代码/UI 在位但只覆盖部分面上；“未覆盖”指无当前运行证据；不把未覆盖升级为“需批准”或“新增产品规则”。

### 5.1 §4 R3-B 集中管理

| 项 | 状态 | 依据 |
|---|---|---|
| 人口页：同 Roster 姓名/职业/在村/主要职责/现时执行/临时承诺/饥饿可比较 | **已实现 + 已验证（渲染层）** | 12 例布局例 `ROW-TITLE-RENDER/VISIBLE`；投影 `ManagementSnapshot` |
| 人口页：按名称/状态筛选，点人物进详情，选人不自动派工 | **已实现 + 已验证** | 布局例搜索/过滤 top-hit；CASE-A/CASE-D 断言选人/命令分离 |
| 职责与场所：一行一 WorkplaceId + BuildingInstanceId + FunctionSlotId，容量/绑定/劳动/现场分列 | **已实现 + 已验证（main slot）** | 投影测试 + CASE-A / CASE-B |
| 职责与场所：施工行单列进度与工人 | **已实现 + 已验证** | CASE-B 全程进度/工人证据 |
| 已 Completed 动态岗位 `TryAssignPerson`/`RemovePersonAssignment` | **已实现 + 已验证** | CASE-A、CASE-D |
| 施工只调用 `TryAddConstructionWorker`/`TryRemoveConstructionWorker` | **已实现 + 已验证** | CASE-B + 第 18 例 claim 施工 |
| 资源页：Ingredients/Food、在村供餐对象、Hungry 人数、Farm/Kitchen 实例与名额/劳动 | **已实现 + 部分已验证** | 资源页 top-hit + CASE-C 资源事实；批次/供餐细节仍后置 |
| 动态岗位经 FunctionBindings 枚举 + `BuildingWorkplaceId` 与 `GetWorkplaceDef` 互核，不按同名合并 | **已实现 + 已验证（单 slot）**；第二 FunctionSlot **未覆盖** | Edit `SameDefinitionBuildingsAndRegisteredSlotsStayIndependent`；配置只登记 `main`，无合法注入点 |
| 未完工建筑不出可派工岗位；0 工人保留进度不写 Pause | **已实现 + 已验证** | CASE-B 0-worker advance progress=0 |
| 静态岗位（`ws_farm_01`）显示既有绑定并可撤职，不假装正式建筑 | **已实现 + 已验证** | CASE-B legacy row bound=1 assign-disabled remove-enabled |
| 跨页同一 Selection；Building/Place↔Person 互跳 | **已实现 + 已验证** | CASE-A/CASE-C/CASE-D + 首次导航 |
| 名额（长期绑定）与现场/实际劳动数独立计算 | **已实现 + 已验证** | 劳动口径测试 + CASE-B/D |
| 不提供原子改派：AlreadyAssigned 拒绝，显式先撤后派 | **已实现 + 已验证** | CASE-A END |
| 撤职移除 Assignment 记录及累计统计，不包装无损 Undo | **已实现 + 已验证（含边界）** | CASE-D；旧 `arrival` 只在下一边界清 |
| 管理命令成功后显式保存并显示 bool，失败不称已保存 | **已实现 + 已验证** | CASE-C SAVE-SUCCESS / SAVE-FAIL-EXPECTED / SAVE-RECOVERED |
| 推进/返程 busy 时命令与保存禁用并给已有诊断，不自动暂停/解锁 | **已实现 + 已验证** | Return gating、事件 pending 门禁 |
| claim 持有者加入施工 | **已实现 + 已验证（claim 单例 `41a9596f` + 整类 18/18 `99dc1dfe`）** | 时间线：**464 claim 保持 → 465 合法 Interrupted 释放 → 480 到场零劳动 → 481 劳动 1**；`AddConstructionWorker` `Success`、无 busy 拒绝、无 `RefreshWorkIntent` |
| 拒绝实现清单（Test Project 目录 / Contribution 排行榜 / 估算完成日 / 自动最优派工 / 岗位→职业转换 / 批量事务 / 经济重平衡 / 仓储物流 / 建筑升级拆除 / 新 Activity 规则 / 破坏性迁移） | **已遵守（未实现）** | 代码与文档一致 |

### 5.2 §5 页面、UI 与路由接缝

| 项 | 状态 |
|---|---|
| 唯一 World / 镜头 / Selection（`ProductHomeWorldHost`），读模型不写 Authority | 已实现 + 已验证（三表面等价例） |
| 原 Person/Management Surface 替换白模正文，无额外 Scene / 第二 Home 入口 | 已实现 + 已验证（`Management_ProductEntry`） |
| `HomeSelectionContext` 来源替换取消页面选择/订阅 | 已实现 + 已验证（首次导航用例 + `SyncSourceReset` 产品修复） |
| 固定 UI 结构为 Editor 可编辑 Prefab，Runtime 只生成重复行 | 已实现 + 已验证 |
| 内部页面不 `SetSimulationActive(false)` / 不 Pause / 不改 Speed；手动 Pause 保持 | 已实现 + 已验证（`ManagementModal…`、模态例） |
| 关闭返回 World 且保留有效选择；ESC 与关闭同一回调 | 已实现 + 已验证 |
| 全屏 Raycast Blocker + World 输入阻断（非仅叠加 Panel） | 已实现 + 已验证（程序化 Raycast） |
| TopHUD 时间控制在模态允许区可操作 | 已实现 + 已验证（`top=Pause`） |
| 键盘 / 镜头 / 物理鼠标穿透 | **未覆盖**（只有程序化 Raycast / `ExecuteEvents`） |
| 远征仍走 `Town.EnterProductExpedition` gating，不借内部页恢复 Away Simulation | 已实现 + 已验证（Return gating 例） |
| 旧 Pending 事件 / Return 阻断优先级保留 | 已实现 + 已验证（第 11 例） |

### 5.3 §6 实施顺序与验收路径

| 项 | 状态 |
|---|---|
| 现场 Bootstrap / 保护 / Editor 宿主回读 → 投影与专项测试 → Prefab UI → 真实 MainMenu 入口 → 产品操作验收 → 最终回归 | 已按此顺序实施；最终回归为前阶段 264/1513/122 |
| 制作人产品验收卡第 6 条（两座同类完工建筑分别派人、满员拒绝、通勤与劳动可区分） | 已验证（CASE-A） |
| 第 7 条（撤职后另派、名额释放、不把撤职当撤施工） | 已验证（CASE-A/CASE-D） |
| 第 8 条（施工加入/撤出保留 Primary，无人停进度，到场才继续，完工同一建筑） | 已验证（CASE-B） |
| 第 9 条（资源页观察、保存后 Continue，库存/职责/施工/成长恢复，失败明确提示） | 已验证（CASE-C；成长结果由既有 Growth 回归覆盖，本包未新增成长断言） |
| 第 10 条（远征准备→回村、内部页不绕过 gating、模态背景点击不触发） | 已验证（Return gating + Blocker） |
| 第 1–5 条（R3-A 人物详情 / 成长 / 关系历史） | 不属本包范围；沿用 R3-A 既有回执，未在本包重跑 |
| 制作人手工“真实点击手感”验收 | **未覆盖**（程序化输入） |

### 5.4 §7 风险、未决与 Stop Conditions

| 风险项 | 当前处理 | 状态 |
|---|---|---|
| Growth UI pending / 保存 bool 未检查 | 本包未改成长契约；未新增持久 pending | 保持既有风险（不在 R3-B 范围） |
| `TryAssignPerson` 缺真实人物/Availability 防御 | 管理命令统一在 `ManagementCommandGateway.CheckPerson` 前置（存在 → 可用性 → 世界锁），拒绝零写入 | 已实现 + 已验证 |
| `RemovePersonAssignment` 无 `RefreshWorkIntent` | 按事实断言“执行/贡献即停 + 下一边界清 arrival”，未改 Work/Clock 核心 | 已实现 + 已验证（边界保留） |
| Social 无关系存在标记 | 本包未触碰；沿用历史缺口 | 未覆盖（不在本片） |
| 人物删除 / 无历史 Registry | 未调用、未修 | 未覆盖（保留） |
| GameSpec 状态滞后 | 未改 GameSpec；只在执行记录标注 | 未处理（不自行重写） |
| 无正式职业显示名 / Prototype 参数 | 显示真实 ID 与已存在内容，报告标注 Prototype | 已遵守 |
| Modal 与持续 Simulation、事件/Return busy 叠加、中文/低分辨率 | 1920 已验；**1280 由制作人 `01a53eb8` 取消**；真实滚动/物理鼠标未覆盖 | 部分验证 + 1 项取消 |
| Save Trigger Ownership 未统一 / 世界级 Load transaction | 只复用现有合法 Save 并反馈 bool；未新增自动保存 | 保持既有债（未解决） |
| 字体资产受保护 | 磁盘 = 基线、内存 0 dirty、未重导入 | 保护成立 |

### 5.5 §8 测试矩阵

| 矩阵行 | 状态 |
|---|---|
| EditMode 人物只读投影 | 沿用既有回执（`1513/1513` 前阶段）；本包未重跑 |
| EditMode 关系/历史 | 同上（不在 R3-B 新增范围） |
| EditMode 管理命令 | **当前已验证**：`99dc1dfe` 18/18（含多 slot 独立、未完工拒绝、满员/AlreadyAssigned、claim 施工） |
| PlayMode 真实产品入口 | **当前已验证**：`4aa4ffda…` 12/12（真实 MainMenu→Load→EnterTown） |
| PlayMode 成长 | 不在 R3-B 范围；沿用既有 |
| PlayMode 模态/布局 | **部分已验证**：1920 布局 + 遮挡 + 滚动可达分支；**真实 overflow 滚动、拖拽、滚轮、物理鼠标、1280 未覆盖 / 取消** |
| PlayMode 劳动与资源 | **当前已验证**：CASE-B/CASE-D/CASE-C + 三表面等价 |
| Simulation 对照 | **当前已验证**：`SURFACE-SIM` 三表面同时刻逐项等值 |
| Save/Continue/NewGame | **当前已验证**：CASE-C（显式保存、失败提示、Continue、NewGame 无旧世界泄漏） |
| 远征/事件 gating | **当前已验证**：Return gating + 统一事件 pending 门禁 |
| 测试执行纪律（Compile/Console→专项 Edit→专项 Play→受影响完整串行；触档先备份后恢复） | 已遵守：12 组 `SAVE-RESTORE` + 2 例 `SCHEDULE-RESTORE` + `FONT-POST`/`BASELINE-POST` |

---

## 6. 当前代码差异文件与完整 SHA（只读重算，非 Shell）

全部为本轮 `script-execute` 只读 C# 重算（`File.OpenRead` + `SHA256`）；字节数为实际长度，mtime 为 UTC。

| 路径（仓库相对） | 变更性质 | bytes | SHA256 | mtime |
|---|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` | 新增 + 行模板 Label 28→32 | 282326 | `e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba` | 2026-10-26T03:26:15.6250542Z |
| `…/ManagementView.prefab.meta` | 新增 | 178 | `b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559` | 2026-10-23T12:10:23.0493699Z |
| `…/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | 新增并扩到 12 例（含姓名可见断言） | 328163 | `a9ae921dbe12cf3d6cd02d83b63d2816390ea9906b92395862035e3042d64499` | 2026-10-26T03:26:42.3867672Z |
| `…/Assets/Tests/PlayMode/ManagementShellPlayTests.cs.meta` | 新增 | 267 | `9da27352b62ef02bed6e5ac640453677938d3a8fa885dcf896d5799713570404` | 2026-10-23T12:22:24.1752897Z |
| `…/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs` | 新增并扩到 18 例（第 18 例 claim 施工） | 54062 | `f48f8f7d0cb9bfe299c3c8cbc1a6814540bd178495e632f907eedf21e9f16e0b` | 2026-10-26T03:08:46.6725349Z |
| `…/Assets/Tests/EditMode/Village/ManagementProjectionTests.cs.meta` | 新增 | 267 | `eb42773ed3219876d1c9cc3a6747dc93e6d4605bbc7b89f5721eec8f3aaf5cf0` | 2026-10-22T14:48:08.9140285Z |
| `…/Assets/Scripts/Village/ProductHome/ManagementView.cs` | 新增：管理页控制器 | 37518 | `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d` | 2026-10-23T12:06:15.1653305Z |
| `…/Assets/Scripts/Village/ProductHome/ManagementRowView.cs` | 新增：行视图 | 1670 | `d5a2456ab13119635e9c6c31cac5a7932cef815a301c3f7536d208741543eebd` | 2026-10-23T12:02:15.9366499Z |
| `…/Assets/Scripts/Village/ProductHome/ManagementReadAdapter.cs` | 新增：只读投影（含劳动口径修正） | 14023 | `752c6a2d8ebb0579ad30792fab93f4e8b70b56f2d5d933ae0f23cf0457f6fea7` | 2026-10-23T05:30:36.3602426Z |
| `…/Assets/Scripts/Village/Runtime/ManagementSnapshot.cs` | 新增：不可变快照模型 | 14661 | `1184f80307e8ef07563193f27433a6983077b7d79a312b1df57b53ae5ade1ce6` | 2026-10-23T05:30:36.3664058Z |
| `…/Assets/Scripts/Village/ProductHome/ManagementCommandGateway.cs` | 新增：合法命令网关 | 13603 | `d912bfc9baea7c68f3aad3d77f606fcbd1e27912f0cf59710131a4bd5453cc5e` | 2026-10-22T14:46:21.6670871Z |
| `…/Assets/Scripts/Village/ProductHome/HomeProductShell.cs` | 修改：管理 Surface 接入 + `SyncSourceReset` 产品修复 | 16082 | `7da8fbb06aac044e37acbd21377ff7e32f4aa1b99659e4f18bdf66bbbd8f2f66` | 2026-10-24T11:45:30.2619300Z |
| `…/Assets/Scripts/Save/SaveSystem.cs` | 修改 1 行：NewGame 资源复位 | 45001 | `97f48237e42f05c7d8768ee09a09b6f7a295189afb47329afc3e16268c8d8045` | 2026-10-24T05:46:33.4765370Z |
| `…/Assets/Editor/GrowthDataInitializer.cs` | 维护包：移除全局 `SaveAssets` / 无意义 `SetDirty` | 5711 | `60867b91a580e398f4c6565ab80f241217fbe92a225ed6418b1d1be53a871a78` | 2026-10-23T14:43:05.4830020Z |
| `…/Assets/Tests/EditMode/GrowthDataSeedTests.cs` | 维护包新增：无关 dirty 资产不被全局保存 | 4211 | `ae9d28924d61e44501247a40b364635071f60c29222de19a64e01095a1ea4c83` | 2026-10-23T14:43:05.8189743Z |
| `UnityProject/RRPG1.0game/Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | 编译产物 | 474624 | `5bbdfdc83b5a35c9c9fcfd82d2e5523804de1aba68412bf14ab9c930ba86792e` | 2026-10-26T03:26:51.1664894Z |
| `UnityProject/RRPG1.0game/Library/ScriptAssemblies/VillageStage1.Tests.dll` | 编译产物 | 153088 | `9d09dabbb127fc58235e271d1029c2b57ae2c155d9d3dc1fab08b86fa65b69da` | 2026-10-26T03:08:50.4871113Z |

补充只读核验：Prefab 内 `m_SizeDelta: {x: 1072, y: 32}` = **4 处**、`{x: 1072, y: 28}` = **0 处**；PlayMode 测试 DLL **晚于**源码（03:26:51 > 03:26:42），Edit 测试 DLL 晚于源码（03:08:50 > 03:08:46）。

---

## 7. 未覆盖清单（如实保留，不升级为“需批准”或“新增产品规则”）

1. **第二 FunctionSlot 的真实 Gateway 路径**：当前 `BuildingConfigDatabase.asset` 只登记 `FunctionBindings: main`，`StaticConfigRegistry` 无公共注入/注册 API，`AssignToWorkplace` 同时要求绑定命中与 `GetWorkplaceDef` 已注册 → 在“不改配置 Authority / 不改公共 API / 不用私有反射注入”的边界内**没有合法注入点**。这是**建筑 FunctionSlot 配置面**的限制，**与 `SaveSystem` 单槽存档 / guard 无关**（不可用“唯一存档槽”解释）。
2. **实际 overflow 滚动**：Population `overflow=False contentHeight=558 < viewportHeight=596 no-scroll-needed=True`；Workplaces/Resources 空态。真实 EventSystem 滚动到末端（`verticalNormalizedPosition→0`、内容位移、末行滚动后命中）**本包未触发、未覆盖**，不得写“滚动通过”。
3. **物理鼠标 / 拖拽 / 滚轮 / 键盘穿透**：全部为 `EventSystem.RaycastAll + ExecuteEvents` 程序化点击；真实指针路径与手感**未覆盖**。
4. **其他业务行标题截图**：仅人口页有 6 行 `ROW-TITLE-RENDER` 真实证据；Workplaces / Resources / Construction 三个模板的 Label=32 只有结构证据（当前世界为空态），**没有业务行截图 / 渲染断言**。
5. **Chevron**：仍 `44×28`、字号 24，小于其真实行高；**未修、未测、未纳入断言**。
6. **Detail 单行 + Ellipsis**：`1072×42`、字号 17，只能单行（阈值 52 才容两行）；既有表现，**本包未改**。
7. **历史 650 存档缺口**：`21075B / 650c45c4d1226ab5606073cd9210545438b37482d2b5de47559a2ca6ecb06ee5` 在授权范围内**无字节备份**；当前 `13405-4b2b56` **不是它**，本包未恢复、未伪造。
8. **1280×720**：制作人 `01a53eb8` **取消**，因此记为“取消”，**不是通过**，不等待人工、不再标 BLOCKED。
9. **成长 / 关系 / 历史 / 死者档案** 等 R3-A 与相邻议题：不在本包新增范围。
10. **未知投递队列**：`com.unity.test-framework@1.1.33` 的 `TestRunnerApi` 无可见 `pending*/activeRequest*`；只以 `IsRunActive=False` 作为“当前没有在跑”的判据，**不推断有/无排队**。

以上未覆盖项**不阻塞**本包交付为“可审的 PARTIAL 证据包”，也**不被**升级为需要新增制作人批准的产品规则。

---

## 8. 必须保留的历史异常（不可洗白）

- `ManagementView.prefab` 在历史只读打开 PrefabStage 后被编辑器/插件**非有意自动序列化回写**：`282097B / ade4c7e1…` → `282326B / 5cf61738…`（实测 **+229B**；早期口述“232B”与之口径不同，按实际字节数列出，不合并）。
- **只相关性、未确定根因**：当时会话未执行 `SaveAssets/SaveChanges/rql-mutate`，事件与回写时间相关，但**不能**据此断言“插件已确定根因”，也**不能**断言“只改了 4 个数字、其余序列化形态未变”——缺回写前文件逐字节对照，YAML 缩进/字段顺序/浮点表示是否一并变化**无法证明**。
- 该旧文件无备份、**无法逐字节还原**；不要求恢复旧 Prefab。当前只能证明：4 处 `1072×32`、0 处 `1072×28`，且修复后 12 例整类绿、人口行标题渲染数据与截图可见。
- 1280 相关的旧失败、`b17cd5c1…` rejected、`24c81ccf…` claim 断言失败、`2026-10-23T15:53Z` 5 次 Capture 无 Restore、`48d8820f` aborted 等历史回执**全部保留**，不删除、不改写成 PASS。

---

## 9. 核心文档更新（本包实际写入）

| 路径 | 变更 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261024/final-self-audit.md` | 顶部追加“2026-10-26 最终当前事实”块；文末追加 **§10 最终当前事实定稿**（含 §4–§8 逐项矩阵、版本范围、未覆盖、SHA 指引） |
| `Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md` | 顶部加 2026-10-26 指针；文末追加 **§9 2026-10-26 最终当前事实**（回执、保护、规格逐项、未覆盖、下一步） |
| `Development/StudioRuns/R3B-UI-20261026/final-current-evidence-review.md` | **新增**：本索引（保护面 / 回执 / 行标题 / 版本范围 / 规格逐项 / 代码 SHA / 未覆盖 / 历史异常 / 下一步） |

未更新：`Docs/DevelopmentSpecs/README.md`、`Docs/项目状态快照.md`、`Docs/项目日志.md`、`Docs/文档地图.md`、`Docs/开发验收交接.md` 等 6 份索引文档（按本包约定避免膨胀，留待下一次同步包）；未改 `GameSpec/**`、`Docs/Architecture/**`、`Docs/决策记录.md`、`Docs/问题清单.md`、基线 JSON、manifest、备份、存档、代码、测试、Prefab、字体、Scene、Packages、ProjectSettings。

---

## 10. 下一安全闭环建议（不在本包实现）

1. **真实 overflow 滚动**：可通过**合法的世界增量**构造（例如在测试内用真实产品入口继续推进/新增足够多的人物或岗位行，使 `contentHeight > viewportHeight`），再用真实 `IScrollHandler` / `verticalNormalizedPosition` 验证末端可达；**不伪造 RectTransform、不手改 Prefab 尺寸、不注入假数据**。需另立包，本包不实现。
2. **物理输入与手感**：仍建议由制作人在 1920 真实窗口手工点击/拖拽一次，记录“通过/问题”，不写成自动化 PASS。
3. **Chevron / Detail 两行 / 其他业务行标题**：如需覆盖，需构造非空 Workplaces/Resources/Construction 业务行（真实建筑与资源），或明确接受“仅人口页有渲染证据”。
4. **第二 FunctionSlot**：只有在获得配置 Authority 或公共注册 API 授权后才另立包，当前不改 `BuildingConfigDatabase.asset`、不伪造。
5. **历史 650 档**：如需取证/恢复，须制作人单独授权；不得用当前 `13405-4b2b56` 冒充。
6. **Guard 进程级缺口**：可评估“runner 前持久快照 + 后精确写回”脚本化；本轮 202 个 run 全部终态合法，无需清锁。
7. **6 份索引文档同步 + Git 只读定稿 / 提交发布**：分别属文档同步包与 Git 授权，不在本包。
8. **不建议**在收尾阶段重跑 264/1513/122 全集来“换一个数字”；如需覆盖最新 Prefab/源码差异，应作为独立的“最新版本完整回归包”，并明确其耗时与保护面。

---

## 11. 边界与诚信声明

- 本包唯一 Editor 操作：`rql-query` 只读查询、`script-execute` **只读** C#（读文件、重算 SHA256、`EditorUtility.IsDirty` 只读判定、反射读 `IsRunActive` / `SaveSystem.GetSavePath`、目录与 manifest 统计、读 `DateTime`）。
- **未**运行 `tests-run` / TestRunner、**未** `AssetDatabase.Refresh`、**未**打开 PrefabStage、**未** `rql-mutate`、**未** Shell / `Process.Start`、**未** Git 写、**未** 写 Preferences、**未** 启停 Editor、**未** Save / SetDirty / SaveAssets / SaveChanges / SaveDirtyScenes、**未** 清 dirty、**未** 重导入字体、**未** 覆盖内存对象。
- 未改产品 / 测试 / 断言 / Prefab / Scene / 字体 / 图集 / 34 项基线 / 存档本体 / guard manifest / 配置 / Authority / 备份；未安装组件、未改权限、未创建额外付费子 Agent。
- **写后扫描披露（如实）**：本轮 mtime 变化的文件仅 4 个——本文件、`final-self-audit.md`、执行记录，以及 Unity 插件自身的 `UnityProject/RRPG1.0game/.com-unity-codely.json`（300B，内容为 `unity_port/stream_port/last_heartbeat` 心跳，由 MCP/插件在调用时自动刷新，非本包刻意写入；该文件**不在 34 项基线内**，也未进 `Assets/`）。写后复核：Editor 仍空闲、场景仍 `dirty=False`、字体仍 `14961033B / 3c758e16…` 且 9 子资产 0 dirty、单槽仍 `13405B / 4b2b56f6…`。
- 本文件写入后版本（追加本句之前）`33430B / 556cf2bff5520422e97175d0c2395ce02e13f260569f4f93211d2bb1b4802bbb`；最终哈希以目录实际文件为准，不据旧值宣称未改。
- 主开发已核的宿主 runId / run 级耗时 / `scope-restored` 与 Studio 私有 runtime 记录，本层读不到，按“转述”标注；本层可核对部分（`testResults`、日志、manifest、字节、SHA）已逐项列出，不把“未读取”写成“已验证”。
- **R3-B = Implemented（主要链路）/ Verification PARTIAL / Not Committed / Not Published**；本文件及结论须由主开发专业核验，**不代表制作人批准**，整包不 Closed / 不 Accepted。
