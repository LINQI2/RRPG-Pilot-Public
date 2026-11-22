# Initial World 初始化数据库 · Unity 精确生成与五例配置链验证记录（阶段 1/2）

- 授权 / 唯一操作目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（本轮 `working_directory` 实测）
- Unity 工程：`D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`（`projectPath` 实测；`unityVersion=2022.3.62t9`）
- 当前阶段：**1/2 已完成（前置核验 + Builder 精确生成 + 五例可发现）；2/2 追加见 §11（十例与持久 SaveFileGuard 交接）**
- 结论口径：**生成已完成并读回；不运行 `tests-run`，五例 / 十例 PASS/FAIL 均以宿主 Studio check 回执为准；本包不宣称整包可玩完成。**
- 本包未重做调查 / seed / Compiler；主开发受控回执作为来源引用，不据本包重新签发。

---

## 0. 前置实测（生成前）

| 项 | 实测值 | 结论 |
|---|---|---|
| CWD | `D:\AI_Agent_Lab\rrpg_pilot\repo` | ✅ 仅授权 Pilot 目录 |
| Unity 编辑器 | `isPlaying=false / isPaused=false / isCompiling=false / isUpdating=false` | ✅ 空闲 |
| 版本 | `2022.3.62t9` | ✅ 与 README / 回执一致 |
| 原生 TestRunner | `TestRunnerApi.IsRunActive=False`（只读反射） | ✅ 空闲，未启动 / 未忙等 |
| 场景 | `SampleScene`（`Assets/Scenes/SampleScene.scene`），`isDirty=false`，`rootCount=5` | ✅ 干净，无 PrefabStage |
| 受保护 TMP 字体（磁盘） | `14961033 B`，SHA256 `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`，mtime `2026-10-10T13:23:55.9520319Z` | ✅ 与既有保护值一致 |
| 字体 9 个子资产 | `subassets=9 / dirty=0` | ✅ 无待保存污染 |
| 34 项 workspace 基线 | 读取 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 逐项重算：`total=34 / same=34 / diff=0 / missing=0 / problems=0` | ✅ 全一致 |
| 用户存档（当前档） | `1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot\rrpg_save_p0.json`，`13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，`.tmp=False` | ✅ 与本包开工值一致，未写档 |
| Initialization guard | `Development/StudioRuns/Initialization-20261027/save-guard`：20 目录 = **20 restored**，0 unfinished / 0 unknown / 0 noManifest | ✅ |
| R3B guard | `Development/StudioRuns/R3B-SaveProtection-20261024`：310 目录 = **309 restored + 1 protective-snapshot**，0 unfinished | ✅ |
| 现有生成状态 | `InitialWorldConfigDatabase.asset` **不存在**；Root 尚未挂 InitialWorld 库；三份 Initial*.json 已存在 | ✅ 与「待本阶段生成」一致 |

---

## 1. 主开发 Compiler 受控回执（来源引用，非本包执行）

来源：主开发真实受控宿主回执 `compiler-host:8e7758c9290a8d750aae31b24d71a167219d2ba4906509fe7ec9e08a4f476b4a`，状态 `completed`。其记录：

- `build 0`；Compiler 测试 **280/280、0 skip**；`seed / build / validate` 均退出 **0**；9 项仅导航类 sheet warning（不阻断生成）。
- 交付文件与 SHA256（本包生成前逐个独立复算，全部一致）：

| 文件 | 字节 | SHA256（主开发回执值 = 本包复算值） |
|---|---|---|
| `Development/Config/RRPG_InitialWorld.xlsx` | 11207 | `a9ac696d6c5a8e638177bf89509b95866d86558dcf344f8f719320afb3a10e4d` |
| `Development/Generated/StaticConfig/InitialWorld.json` | 1569 | `5af02464bbf359c62568525cec1f8c4c11d7fec8a4ce91852a776399c8da321b` |
| `Development/Generated/StaticConfig/InitialPopulation.json` | 1377 | `ae0fe5456266ff6658bd2d2829d987631e74e35ebc0301c3af1514d87e6bcc65` |
| `Development/Generated/StaticConfig/InitialBuilding.json` | 2209 | `32abb64be5c5fbe715b985fae1d56e6301fda516d008d85aa6217b3ff64f0015` |

生成后再次复算这 4 个输入文件：**字节与 SHA 全部未变**（Builder 只读）。

---

## 2. Builder 真实接口与变更影响（先读源码，再执行）

读取 `Assets/Editor/StaticConfig/StaticConfigBuilder.cs` 与 `InitialWorldMirrorReader.cs` 的实际实现：

1. 入口为 `public static BuildOutcome Build(bool force)`；本包以 `force=false` 调用，未使用 `RebuildFromZero()`（避免删除整个 Managed Root）。
2. 输入：`Development/Generated/StaticConfig` 下全部 Canonical Mirror（本包实测 44 个 Mirror 文件），其中 InitialWorld 三件套由 `InitialWorldMirrorReader.Read(mirrorDir)` 一次读入：0 份 = 合法空态，1–2 份 = Hard Error；内容再做空表 / 多默认 / 重复序号 / 重复人物 / 非有限坐标校验。
3. `content-signature` 命中即 no-op；本次因新增三件套 Mirror，签名必然变化，属预期写路径。
4. 写路径 `WriteManagedRoot`：
   - 只写 `Assets/Generated/StaticConfig` 之内（Managed Root ownership）；
   - 创建 / 更新 11 个生成容器 + 唯一 `RRPGStaticConfigRoot`，并挂接 `root.InitialWorldDatabase`；
   - **只对本次生成目标调用 `AssetDatabase.SaveAssetIfDirty`**（`SaveGeneratedAsset`），`Exit` 明确不调用全局 `AssetDatabase.SaveAssets()`；
   - `DeleteStaleManagedAssets` 只删除 Managed Root 内、不在 keep 集合的陈旧文件，且根外路径一律跳过；
   - Builder 内部沿用既有 `EditorUtility.SetDirty` 标记本领域生成资产（与其它领域同一模式），本包未对场景 / 非生成资产 / 用户档调用 `SetDirty`。
5. 影响面预判：新增 `InitialWorldConfigDatabase.asset`（+ 自动 `.meta`），Root 内容因新增引用与签名而改变；其余 9 个既有生成数据库会被 Builder 按自身既有逻辑标记并精确保存，但**内容来源未变**，预期 SHA 不变。

本包**没有**修改 Builder / MirrorReader / 测试 / 默认数值；未出现需要窄修的代码错误。

---

## 3. 实际生成（真实调用与回执）

调用（`script-execute`，只走既有 Builder，不使用 `rql-mutate`）：

```csharp
var outcome = RRPG.StaticConfig.EditorTools.StaticConfigBuilder.Build(false);
```

真实结果：

```text
success=True  noOp=False
signature=0cf3f66cb3035fbb423900695457e8ec2904c814cf737b459f0d0d2bc44b4d1f
failure=(空)
action: created: Assets/Generated/StaticConfig/Databases/InitialWorldConfigDatabase.asset
action: updated: Assets/Generated/StaticConfig/Resources/RRPGStaticConfigRoot.asset
action: initial world database: profiles=1 population=5 buildings=7
action: mirror files: 44
（其余 action 为既有领域条目计数与 14 张 managed Sprite `unchanged`，无 deleted(stale) 条目）
```

生成期间 Builder 内部调用 `AssetDatabase.Refresh()` 两次（其既有安全刷新路径）；本包未另调 Shell、Git、全局保存、Prefs 或 Editor 启停。

---

## 4. 生成物读回与 Root 接线（真实读回）

| 检查 | 实测 |
|---|---|
| `InitialWorldConfigDatabase.asset` | 存在，`ProfileCount=1 / PopulationCount=5 / BuildingCount=7` |
| Root 接线 | `rootAsset.InitialWorldDatabase == 该库` → **True**；引用非空 |
| 库内 Mirror 指纹 | `InitialWorldSha256=9151009287e3e6ada701f8c0085d6d5ab8029d77be6febe37b3648f6563e52af`；`PopulationSha256=e597eb2a54ce9145353f61aff8b5eef86524e2ab12b7855e35d0ed848616b9b6`；`BuildingSha256=acffbc7c045451501d1670707011d7be8d8a411c93d57b9c34143623780327ba`（均为 Mirror 头部 `sourceSha256`，不是 Mirror 文件自身哈希） |
| Profile | `iw_default_test`，`IsDefault=True`，`hero_001`，`Gold=100`，`Ingredients=0`，`Food=0` |
| 人口（稳定顺序） | 1 `person_lin_en` / 2 `person_su_wan` / 3 `person_han_zhao` / 4 `person_qiu_shuang` / 5 `person_wei_an` |
| 建筑（稳定顺序） | 1 `building_farm @ (-6,-1.2)`；2 `building_kitchen @ (-2,-1.2)`；3–7 `building_house @ (-8.5,4.2), (-5.1,4.2), (-1.7,4.2), (1.7,4.2), (5.1,4.2)` |

数值仍为 Test / Not Frozen；本包未改默认值、未新增住房容量规则、未接入 workshop。

---

## 5. 生成前后文件 SHA 与真实变动面

完整逐文件证据（路径 / 字节 / SHA256 / mtime）：

- 生成前：`Development/StudioRuns/Initialization-20261027/generated-pre-snapshot.json`（54 个文件，12668 B，SHA256 `1d43cf4a1f54a67269ffa4c120881ff4fd74af949b69966acdb1010f0fe235bf`）
- 生成后：`Development/StudioRuns/Initialization-20261027/generated-post-snapshot.json`（56 个文件，13152 B，SHA256 `7f389e9fd55326b86b53b9fae7bf4a3fac52f5b4548bad2ee5403f40d43de7dd`）

逐项对比结果：

```text
FILES pre=54 post=56 identical=53
ADDED:   InitialWorldConfigDatabase.asset
         InitialWorldConfigDatabase.asset.meta
REMOVED: （无）
CHANGED: Resources/RRPGStaticConfigRoot.asset
```

新增 / 变更文件的实际值：

| 文件 | 字节 | SHA256 |
|---|---|---|
| `Assets/Generated/StaticConfig/Databases/InitialWorldConfigDatabase.asset` | 1828 | `5859e59b67a8a83072bf1a7e21bffbe34b09677f5744c12abb48ff813e9c35b4` |
| `Assets/Generated/StaticConfig/Databases/InitialWorldConfigDatabase.asset.meta` | 213 | `7b1c36e0d925d2bd2f287465045d76119a49b32853967ba903d176175d0a2725` |
| `Assets/Generated/StaticConfig/Resources/RRPGStaticConfigRoot.asset` | 1446 | `db510e20e1bd8ff05b67b4b28dd62ed9187607713f50c9916fbac8bc87dd1bd3`（生成前 1354 B / `36df2f05…eac70f14`） |

其它 53 个文件字节 + SHA 全部不变。其中 9 个既有生成数据库**内容未变（SHA 相同）但 mtime 被 Builder 精确保存刷新**：

```text
AutonomousLifeConfigDatabase.asset / BuildingConfigDatabase.asset / EncounterConfigDatabase.asset
EnemyPresentationDatabase.asset / EventConfigDatabase.asset / MonsterConfigDatabase.asset
SocialConfigDatabase.asset / VillagePresentationDatabase.asset / VisualAssetCatalog.asset
```

这是现有 Builder 的既有行为（保存本次生成目标，而非全局保存）；**未改变这些领域的内容字节**。14 张 managed Sprite、其 14 个 `.meta`、6 个目录 `.meta`、9 个既有数据库 `.meta` 与 Root `.meta` 共 44 个文件保持原 mtime / SHA（`sameBytesAndMtime=44`），无新增或删除。

明确口径：没有「无故改其它领域资产内容」；仅 Root 内容变化、新增本领域库与 meta，以及 Builder 对自身生成数据库的正常 mtime 刷新。

---

## 6. 生成后保护面复核

| 项 | 生成后实测 | 结果 |
|---|---|---|
| 编辑器 | `isPlaying=false / isCompiling=false / isUpdating=false`；TestRunner `IsRunActive=False` | ✅ |
| 场景 | `SampleScene isDirty=false rootCount=5` | ✅ 未污染 |
| 字体 | `14961033 B / 3c758e16…c404854`，mtime 仍 `2026-10-10T13:23:55Z`；子资产 `9 / dirty=0` | ✅ 未变 |
| 34 项基线 | `total=34 / same=34 / diff=0 / missing=0 / problems=0` | ✅ 全一致 |
| 用户存档 | `13134 B / adcf5d8b…8ceb08`，`.tmp=False` | ✅ 未写 |
| Initialization guard | 20 = 20 restored，0 unfinished | ✅ 未新增 run |
| R3B guard | 310 = 309 restored + 1 protective-snapshot，0 unfinished | ✅ 未新增 run |
| 输入 4 文件 | xlsx 与三份 Initial*.json SHA 与 §1 完全相同 | ✅ 只读 |
| Console | 生成后 `sequence > 86887` 的 `Error` 仅 2 条，均为本包只读 snippet 自身的临时编译失败（`CopyProfiles` 参数签名 / `System.IO` 命名空间歧义），**不是项目程序集编译错误**；无 Builder / 测试程序集错误 | ✅ 项目编译正常 |

如实披露：为确认 5 例可发现与读回生成结果，本包执行了只读 `script-execute` 片段；其中两次片段编译失败被插件记为 Console Error（如上）。失败片段未成功执行、未写任何文件；随后修正并成功完成读回。本包未清 Console、未掩盖记录。

---

## 7. 五例可发现性与宿主交接

`tests` 源只读查询（非 `tests-run`）：

```text
RRPG.Initialization.Tests.InitialWorldConfigPipelineTests.DefaultReadComesFromGeneratedContainerAndIsIndependentPerCall   EditMode   InitialWorld.Tests
RRPG.Initialization.Tests.InitialWorldConfigPipelineTests.EmptyGeneratedContainerIsRejectedByCatalogAndRegistry          EditMode   InitialWorld.Tests
RRPG.Initialization.Tests.InitialWorldConfigPipelineTests.ExplicitConfigurationDoesNotReadGeneratedDefault               EditMode   InitialWorld.Tests
RRPG.Initialization.Tests.InitialWorldConfigPipelineTests.GeneratedContainerReadbackIsDeepCopiedAndOrdered               EditMode   InitialWorld.Tests
RRPG.Initialization.Tests.InitialWorldConfigPipelineTests.IllegalGeneratedContentIsRejectedBeforePublishing              EditMode   InitialWorld.Tests
```

**发现数 = 5**（不是 0；0 项不视为通过）。生成后再次查询仍为 5 条，程序集 `InitialWorld.Tests`。

交接要求：由宿主 Studio 阶段 check 串行执行该类的 5 例（以及既有 10 例 `InitialWorldConfigurationTests` 的回归），以宿主回执判定 PASS/FAIL。本包**不预填任何 PASS**。

---

## 8. 文档口径更正（seed 报告）

主开发静态复核指出的口径问题：seed 报告的实际交付文件是 **3 个**（`InitialWorldWorkbookSeed.cs` 修改、`InitialWorldWorkbookSeedTests.cs` 新增、`seed-bootstrap-temp-fix.md` 本报告），不是 4 个文件。`seed-bootstrap-temp-fix.md` 中出现的「4 例 / 4 个用例」是**测试用例数**，不是文件数。

本包据此更正口径：**seed 报告的文件范围 = 3；测试用例 = 4**。不把用例数写成文件数，也不据此声明任何测试已通过（Compiler 280/280 与 seed 4 例以主开发 Compiler 宿主回执为准）。

---

## 9. 本包明确未做 / 整包未完成事项

1. **未运行 `tests-run`**：五例实际结果待宿主 check；本包只证明可发现与输入已真实生成。
2. **未做 NewGame / 加载 UI / 字体探针**：未启动 Play、未实例化产品入口、未做 `ForceMeshUpdate` / `GetPreferredValues`。
3. **住房去重未做**：5 个 `building_house` 生成实例与现有 `Home:<personId>` 展示锚点同位置并存，收敛到同一权威属后续包。
4. **Farm / Kitchen 展示去重未做**：`ws_farm_01` / `ws_kitchen_01` 静态锚点与本领域实例并存。
5. **workshop 退出 / legacy 岗位后续接线未做**：本包默认内容不含 workshop，但旧静态岗位仍保留兼容，正式入口隔离属后续。
6. **真实可玩验收未做**：未验证 5 人 / hero_001 / 100 金币 / farm+kitchen+5 house 在产品 UI 的完整表现；不宣称整包完成。
7. 未改 SaveVersion / DTO；未访问未授权工程 / 认证目录 / 其他工作区；未安装组件、未改权限、未创建额外付费子 Agent。

---

## 10. 边界声明

- 仅使用既有 `StaticConfigBuilder.Build(false)` 生成；未手写 Mirror、未手写 `.asset`、未用占位冒充生成。
- 未使用 `rql-mutate`；未调用 `AssetDatabase.SaveAssets` / `SaveChanges` / `SaveDirtyScenes`；未写 Preferences；未启停 Editor；未 Shell / Git 写。
- 本文件与生成结果需由主开发专业核验；不代表制作人批准；不越过阶段验证门禁。

---

## 11. 阶段 2/2 追加：十例准备与持久 SaveFileGuard 交接（不预填结果）

### 11.1 前阶段真实结果回读（非本包重跑）

`testResults` 源实际记录（宿主已执行的上一次 run）：

```text
Passed 196ms  InitialWorldConfigPipelineTests.DefaultReadComesFromGeneratedContainerAndIsIndependentPerCall
Passed   2ms  InitialWorldConfigPipelineTests.EmptyGeneratedContainerIsRejectedByCatalogAndRegistry
Passed  33ms  InitialWorldConfigPipelineTests.ExplicitConfigurationDoesNotReadGeneratedDefault
Passed   3ms  InitialWorldConfigPipelineTests.GeneratedContainerReadbackIsDeepCopiedAndOrdered
Passed   3ms  InitialWorldConfigPipelineTests.IllegalGeneratedContentIsRejectedBeforePublishing
```

- 该 run 共 5 条，全部 `Passed`，无 `Failed` / `Skipped`，**没有需要按 stack 定位的失败**。
- 这是宿主上一阶段回执的只读回读，不是本包执行；本包全程未调用 `tests-run`。
- 因此本阶段没有做「失败修复」；`InitialWorldConfigurationTests.cs` **未改动**，仍为 `43378 B / f89ca9b89f1dd1b79e69b715e452eb131b166f5c4f1d713ec68c7539783c7bf0`（与上一实施记录一致）。

### 11.2 十例可发现性（真实查询）

`tests` 源查询 `fullName contains "InitialWorldConfigurationTests"` → **10 条**，均为 `EditMode` / `InitialWorld.Tests`：

```text
PureConfigurationValidationRejectsStructurallyInvalidInputs
ContentAndLayoutValidationRejectUnknownOrIllegalReferences
NewGameCreatesConfiguredPopulationGoldAndResources
NewGameCreatesCompletedFarmKitchenWorkplacesAndHousingWithoutWorkplace
NewGameIsSingleShotAndRepeatableWithoutDuplicateInstances
ContinuePreservesInstanceIdentityPlacementAssignmentAndResources
LegacySaveWithoutBuildingsDoesNotRebuildInitialWorldAndKeepsStaticAssignments
InvalidConfigurationLeavesCurrentWorldUnchangedAndReportsFailure
TestResetAndEnsureInitializedDoNotSeedProductWorld
HeroReplacementAfterNewGameKeepsSingleHeroAndPersonIds
```

同程序集合计 `InitialWorld*` 15 例（10 + 5），发现数均非 0；**发现不等于通过**。

### 11.3 持久 SaveFileGuard 实况（代码路径 + 磁盘证据）

`InitialWorldConfigurationTests` 内已有 `SaveFileGuard`（`Assets/Tests/EditMode/Initialization/InitialWorldConfigurationTests.cs` 末尾）：

1. **先于任何运行时初始化**：`[SetUp]` 第一行 `SaveFileGuard.Capture()`，之后才 `ResetRuntime()`；SetUp 异常时立即 `Restore()`，不留无快照状态。
2. **持久证据**：每个用例生成唯一目录 `Development/StudioRuns/Initialization-20261027/save-guard/<utc>-<guid>/`，原档以 `FileMode.CreateNew` 写 `original-save.bin`（已存在即失败，不覆盖），并写 `manifest.json`。
3. **捕获校验**：备份回读后做长度 + 逐字节 + SHA256 校验，全部一致才标记 `captured`；任何异常写 `capture-failed` 并上抛。
4. **未完成 run 阻断**：`BlockOnUnfinishedRuns()` 在任何 capture 前扫描 guard 根；存在缺失 manifest 或非 `restored` 的 run 即 `Assert.Fail`，不自动清记录、不自动恢复未知状态。
5. **恢复**：`[TearDown]` 先 `Restore()` 再 `ResetRuntime()`；恢复写回捕获的原始 `byte[]`，回读 SHA 与捕获值一致才写 `restored`，否则写 `restore-failed` 并 `Assert.Fail`，快照保留。
6. **当前实测**：用户档 `13134 B / adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`，`.tmp=False`；Initialization guard **20 目录 = 20 restored，0 unfinished / 0 unknown / 0 noManifest**，新用例可安全 capture。该 20 个 run 是既有证据，本轮未新增、未改写。
7. **持久快照与当前档一致性抽验**：最新 run `20261028T050130644Z-8d58a753` 的 `original-save.bin` 与当前用户档做**逐字节比较**：`13134 B` 全等（`firstDiff=-1`），SHA 同为 `adcf5d8bb0a638ea2d4dc0f7d936c1aaad7492cd19022521010e9f59fd8ceb08`；manifest 为 `"status":"restored"`、`error:""`，`updatedUtc=2026-10-28T05:01:30.6474364Z`。这可证明该 guard run 的恢复结果与捕获原档逐字节一致；**不据此推断任何测试 PASS/FAIL**。

### 11.4 十例在生成配置下的静态复核（不代替运行）

- 第 9 例 `TestResetAndEnsureInitializedDoNotSeedProductWorld` 已改为**来源一致性**：从 `StaticConfigRegistry.TryGetInitialWorld` 取生成容器，逐项对比 hero / gold / Ingredients / Food / 人口 / 建筑布局；断言默认人口与建筑**非空**、含 farm / kitchen / house、不含 workshop。**没有**固定人数 / 金币，也没有接受 0 建筑。
- 第 3–8 例使用显式 `InitialWorldConfiguration`，不依赖默认内容；`NewGame` 失败路径仍要求世界 JSON 逐字节不变，`Continue` 仍保持实例 / 位置 / 职责 / 资源。
- 第 7 例直接写「旧无 Buildings」档，但写入发生在 guard capture 之后，TearDown 按捕获字节恢复；未绕过 guard。
- `InitialWorldConfigurationCatalog.CreateDefault()` 仍 fail-closed 抛出，未改成静默空默认；`InitialWorldConfigDatabase` 仍拒绝非法内容。
- 结论：未发现需要窄修的代码错误；未改默认数值、未改断言、未降低 fail-closed。

### 11.5 交宿主执行与验收口径

- 请宿主 Studio 阶段 check 串行执行 `InitialWorldConfigurationTests` **10 例**（建议连同 `InitialWorldConfigPipelineTests` 5 例共 15 例），以宿主回执判定 PASS/FAIL；本文件**不预填**任何 PASS/FAIL。
- 运行后必须复核：
  1. 用户档字节数 / SHA256 仍为 `13134 / adcf5d8b…8ceb08`（逐字节恢复，`.tmp=False`）；
  2. `Development/StudioRuns/Initialization-20261027/save-guard` 新增 run 全部 `restored`，无 `capture-failed` / `restore-failed` / 未完成；
  3. 字体 `14961033 B / 3c758e16…` 与 9 子资产 `dirty=0`、34 项基线 `34/34`、场景 `isDirty=false` 未被测试运行改变。
- 若出现真实失败：按宿主 stack 定位最小修复，保留 fail-closed 与「非 0 建筑」语义，不通过放宽断言或改成 0 建筑换取通过。

### 11.6 本阶段边界

- 未运行 `tests-run`、未执行 `NewGame` / 加载 UI / 存档写读 / 字体测量；未写用户档、未新增 guard run。
- 未改产品源码 / 测试源码 / 断言 / 默认值 / Builder / Mirror / 生成资产 / 字体 / 34 项基线 / guard 历史。
- 未使用 `rql-mutate`、全局保存、Preferences、Shell、Git 写；未启停 Editor；未创建额外付费子 Agent。
- 本阶段准备与生成事实需由主开发专业核验；不代表制作人批准，也不表示整包可玩完成。
