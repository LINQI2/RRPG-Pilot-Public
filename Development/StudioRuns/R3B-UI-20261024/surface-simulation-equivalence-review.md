# R3-B surface-equivalence｜World / Person / Management 三表面持续模拟与事实等价（本阶段 1/1 回执）

状态：**实现与编译证据已落盘（E1，仅编译/发现级）；运行期 Play 结果未执行，不能声称 PASS。**
本阶段只完成“三表面模拟持续与事实等价”测试的最小实现、编译与静态核对；按任务约定测试由宿主随后以短类名
`ManagementShellPlayTests` 整类 Play 串行执行，本执行层不跑 Test Runner、不并行、不把“已登记/已排队”当通过。

---

## 1. 环境实核（只读，未做写操作）

| 项 | 实测 | 入口 |
|---|---|---|
| Pilot 路径 | `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game` | `rql-query editor.projectPath` |
| Unity 版本 | `2022.3.62t9` | `rql-query editor.unityVersion` |
| Play 状态 | `isPlaying=false`、`isPaused=false`、`isCompiling=false`（编译后复核） | `rql-query editor` |
| 场景 | 单场景 `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | `rql-query scenes` |
| Console | 编译后 Error 0 条 | `rql-query console where logType=="Error"` |
| Test Runner | 空闲；本类发现 8 例（原 7 + 新增 1），均为 PlayMode | `rql-query tests` |

本阶段未使用 Shell、未使用 `rql-mutate`、未做全局 Save/SaveAssets/SaveChanges、未做任何 Git 写操作，
未触碰 14/34 保护文件与历史备份档，未安装组件、未改权限、未创建付费子 Agent/Workflow/Goal。

## 2. 保护与基线核验

- 单槽保护目录 `Development/StudioRuns/R3B-SaveProtection-20261024/` 下共 30 个 run：
  29 个 `status=restored`、1 个 `status=protective-snapshot`，**无未完成 run**（无 `captured`/`restore-failed`/缺 manifest），
  因此 `SaveSlotGuard.BlockOnUnfinishedRuns` 的前置条件不成立，新测试不会被自身保护机制阻断。
  实测方式：`grep '"status"'` 覆盖全部 `manifest.json`（30/30 命中，无其它状态值）。
- 34 项 workspace 基线 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 存在且 `files` 数必须为 34；
  受保护 TMP 字体 `RRPG_SourceHanSansCN_Regular_TMP.asset` 存在。
  **逐字节 SHA 一致性由测试类 `OneTimeSetUp`（测试前）与 `OneTimeTearDown`（测试后）强断言**：
  任何一项 `bytes`/`sha256` 不一致或字体被改动都会让整类 Play 失败。本轮未运行测试，故不在本文声称“已核对一致”，
  只声明保护通道与断言仍在、未被本包削弱。
- 本包只修改测试文件，不新增/替换保护路径；`workspace-baseline.json` 的 34 项不含 `Assets/Tests/**`。

## 3. 回读的契约（只读）

- R3-P §8 `Simulation对照`：同一测试世界同一 seed/目标分钟，World、Person、Management 无操作时实际
  Clock/Work/Activity/Social/Resource 结果一致；Pause 保持；正常/加速/Jump/Return 共用唯一推进；
  禁止按动画到场判断。
- R3-P §5：内部页/成长模态不得 `SetSimulationActive(false)`、不得 Pause 或改 Speed；手动 Pause 必须保持。
- 真实入口与时钟链：`SampleScene → MainMenuUI.OnContinue → SaveSystem.Load → TownUI.Initialize →
  ProductHomeWorldHost.Awake → HomeProductShell.Initialize`；唯一时钟 `VillageTime` 由常驻 `VillageTimeDriver.Update`
  在 `VillageRuntimeBridge.IsSimulationActive` 为真时用 `Time.deltaTime` 驱动，`ProductHomeWorldHost.Update`
  每 0.1s 真实帧重建 `ProductHomeSnapshot`。本包不改这两处产品代码。
- `IntradayPathEquivalenceTests`（Edit）已用 `JsonUtility.ToJson(Village.BuildSaveData / Snapshot / Social facts /
  Selection / SocialFeedback)` 做路径等价比较，说明“同状态 DTO JSON 逐字等价”是仓库既有比较口径。
- `ManagementShellPlayTests.SaveSlotGuard`：Capture/Restore/BlockOnUnfinishedRuns/OneTime 兜底，本包完整复用，未改一行。
- 管理读模型 `ManagementReadAdapter.Read()` 为公开只读投影；`BuildSaveData`、`SocialRuntimeBridge.BuildSaveData`、
  `ActivityRuntimeBridge.BuildSaveData`、`IntradayWork.ToSaveData` 均为既有只读 Query/存档投影，未新增 Authority/DTO/API。

## 4. 新增测试（1 例，位于既有文件）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
用例：`SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous`

### 4.1 同一真实初始世界 W0

- 从 `UnitySetUp` 的真实产品入口（Continue → Load → EnterTown）出发，Clock=0、玩家 Pause。
- 真实建造 `building_farm`、`building_kitchen`，用真实人物 `TryAddConstructionWorker` + 唯一 Clock 推进到
  **14400（第 11 天 00:00，刚跨 Day Tick）** 完工；
- 在 14400 真实建造第三栋 `building_farm`（保持 `UnderConstruction`），通过**真实管理按钮**：
  `p4 → Farm 动态岗位`、`p5 → Kitchen 动态岗位`、`p1/p2 → 第三栋施工参与`；
- 通过既有公开资源入口 `AddIngredients(2)` 提供真实 Kitchen 输入；
- 用真实 `SaveSystem.Save` 把该世界写入单槽作为 W0，并记录 SHA256；每轮用**真实 `SaveSystem.Load` +
  `SetSimulationPersonSource`** 回到同一 W0（同 PersonId / 配置 / 库存 / 职责 / 施工 / Clock）。
- 未反射写私有核心状态、未造第二 Authority、未手工拼 DTO 覆盖世界；单槽字节由 `SaveSlotGuard` 于 TearDown 恢复。

### 4.2 自然帧持续（三表面各自独立证明，不靠直接 Advance 冒充）

对 `World` / `Person` / `Management` 各执行一次：

1. 恢复 W0，真实入口打开目标表面（Person：`OpenSurface(Person)` + 真实页签 `Life`；Management：真实底部导航点击 +
   真实“职责与场所”页签）；
2. 断言 `IsSimulationActive() == true`、`Clock.Speed == Paused`、绝对分钟 == 14400；
3. 用**既有 TopHUD Pause 同一回调** `HomeProductShell.TogglePause` 把 Clock 从 Paused 恢复为 Normal
   （仅把 `SecondsPerDay` 设为 72 作为**采样参数**，让真实帧可稳定观察到整分钟增量；不改任何规则）；
4. 只 `yield return null` 真实帧，断言：Clock 绝对分钟**严格增加**、Speed 仍为 Normal（表面未改速度）、
   Surface 不变、`IsSimulationActive` 仍为 true；
5. 继续等待 `ProductHomeWorldHost.Snapshot` 引用被真实帧替换（host 每 0.1s 重建），断言 Clock 不回退；
6. 采样结束用同一入口回到 Pause，并回 World，供下一轮恢复同一 W0。
   （自然帧与确定性 Advance 是两段分开的证据，日志 tag 区分。）

### 4.3 确定性区间与逐项比较

对三表面各恢复一次同一 W0、同 Paused 状态，用真实 `Clock.AdvanceTo` 推进到**同一绝对目标分钟 16360
（第 12 天 11:20）**，随后捕获并逐项比较：

- Clock：绝对分钟 / Speed / SpeedMultiplier / IsSimulationActive；
- Work：`VillageRuntimeBridge.BuildSaveData()` 全 DTO JSON（含 `Time`、`IntradayWork`、`Assignments`、`Projects`、
  `Resources`、`Social`、`World`、`Buildings`、`State`）+ `IntradayWork.ToSaveData` 的证据分钟与到场登记逐行文本；
- Activity：`ActivityRuntimeBridge.BuildSaveData()` 全 DTO JSON + 每 PersonId 的真实 claim；
- Social：`SocialRuntimeBridge.BuildSaveData()` 全 DTO JSON；
- Resource：`VillageResourceState` 的 Ingredients/Food + 管理资源投影逐行；
- 职责/施工：每 PersonId 的 primary / execution / `IsExecutingWork` / claim / place / availability、每建筑实例生命周期、
  活动项目参与者/进度/劳动剩余，以及 `ManagementReadAdapter.Read()` 的人口/职责场所/施工/资源逐行投影；
- World 快照：人物/地点/建筑生命周期逐行（排序稳定）。

非平凡性断言（防止“空集等价”）：目标分钟必须存在活动施工项目且 `Progress>0`、生产批次>0、生产证据>0、
岗位工作证据分钟>0、Day Tick 累计执行历史>0、Ingredients>0、Food>0、`p4` 仍绑定 Farm 动态岗位。
这些阈值与既有 `CASE-C` 已执行并断言的事实一致（同一推进区间 14400→16360），不是本包新造数值。

### 4.4 非确定字段的处理依据（不随意忽略差异）

- 比较采用**全 DTO JSON + 显式逐行事实**，不做“白名单跳过”；任何字段差异都会失败并打印首个差异位置与两侧片段。
- 为避免帧抖动污染 DTO，确定性区间从 Save/Load 恢复的同一 W0 出发、以 Paused 状态直接 `AdvanceTo`，
  `DriverRemainder` 由 W0 存档恢复，`SecondsPerDay` 在三轮中统一为默认 1440（自然帧期间的 72 只在采样段内，
  恢复 W0 时显式复位），因此 `SecondsIntoDay`、`ReadyMinute`、Day Tick 标记在比较面上可逐字比较。
- 唯一排序处理：所有逐行事实在输出前按稳定键（PersonId / WorkplaceId / InstanceId / 行文本）排序，
  仅消除容器迭代顺序噪声；不隐藏任何数值或布尔差异。
- 未以“读取投影自己”当 expected：基线是 World 表面同一时刻的真实 DTO/事实，Person/Management 各自独立重算。

## 5. 实际执行证据（本轮）

| 步骤 | 结果 |
|---|---|
| 编辑测试文件 | 已落盘；只改 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（原 7 例内容与断言未改） |
| 编译 | `script-execute` 调用 `AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate)`；未使用全局 Save |
| Console | 编译后 Error 0 条；无 `ManagementShellPlayTests` 相关 Warning |
| 发现 | 本类 8 例 PlayMode（新增用例已被 Test Runner 发现） |
| Editor 复核 | `isCompiling=false`、`isPlaying=false`、场景 `isDirty=false` |

**未执行（不得声称通过）**：`ManagementShellPlayTests` 整类 Play、任何 EditMode 回归、全量 Edit/Play 回归。
运行期数值（自然帧增量、16360 的生产/活动/社交条目数、DTO 是否逐字等价）只有宿主实跑后才能确认；
若运行期出现差异，按失败输出定位是“表面副作用”还是“恢复路径/比较面口径”，不通过放宽断言求绿。

## 6. 边界与剩余缺口

- **撤职 arrival 延迟清理边界**：本包未改动 `RemovePersonAssignment` 不调用 `RefreshWorkIntent` 的既有行为；
  `CASE-D` 仍只证明“撤职当刻标签/绑定停止、旧 arrival 行残留、下一工作区间边界清理”。真正“即时清意图”需要
  Work/Clock 领域变更，属制作人裁决，不在本包。
- **Return 路径**：本包的自然帧与确定性区间都是 Direct/Realtime 推进，不含远征 Return/最终 Save 会话；
  `IntradayPathEquivalenceTests` 的 Return 等价仍是 Edit 级历史证据，本包未在 Play 复现 Return。
- **全量回归**：完整 EditMode / PlayMode 串行回归未跑；本包只新增 1 例并保持原 7 例。
- **体验/输入**：物理鼠标、拖拽、镜头滚轮、键盘穿透、1920×1080 / 1280×720 与滚动长列表仍属视觉/输入验收包；
  本包的程序化 Raycast 点击不等价人工手感。
- **Social 单日窗口**：等价比较覆盖 Social 全 DTO JSON；若该窗口内关系事实为空，属真实单日事实（不伪造），
  运行时证据行会记录 `socialBytes` 供后续判断是否需要更长窗口。
