# R3B-UI-20261024 · CASE-C Continue 人口数量 5/6 诊断与最小修复

- 仓库：`D:\AI_Agent_Lab\rrpg_pilot\repo`（本包独立授权目录）
- 阶段：1/1 —— 核对 Continue/NewGame 真实人口口径并最小修复
- 本包写入：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（仅测试 expected 口径）与本报告
- 未使用：shell、`rql-mutate`、`SaveAssets/SaveChanges/SaveDirtyScenes`、`tests-run` / 反射启动测试、模型启动、Git 写、历史存档恢复
- 未修改：产品源码、Save DTO、Authority、场景、Prefab/UI 资产、34 项保护基线、14 项指定保护、`SaveSlotGuard`、时间类型辅助函数、证据备份

---

## 1. 结论

**根因是测试 expected 口径错误，不是产品人口投影过期，也不是 Continue 产品 bug。**

失败断言比较的两个值来源不同：

| 值 | 来源 | 口径 | 结果 |
|---|---|---|---|
| Expected `5` | `DataManager.Instance.InitialPersonRoster.Count` | 只数 Authority 初始名册中的**普通角色 PersonDefinition**（配置 5 行） | 5 |
| Actual `6` | `ManagementSnapshot.Persons` | 真实人物源 `OwnedCharacters`（5）+ `OwnedHeroes`（1） | 6 |

`SaveSystem.NewGame()` 的 `PlayerCollection.InitializeDefaultCollection()` 在建立 5 名初始角色的同时，必定建立 1 名英雄（`hero_001`）；`HeroSelectionUI.OnConfirm` 随后清空并写入玩家实际选择的 1 名英雄。英雄拥有独立真实 `PersonId`，是管理人口投影的合法人物。旧测试只枚举 `OwnedCharacters`（且 expected 只取角色配置数），遗漏了合法 Hero，因此恒差 1。

本包只把测试改为“从长期人物真相源独立枚举合法 PersonId 集合（角色含死亡 + 英雄），再与管理投影逐 PersonId 强比较”，并保留/加强旧世界残留排除；**未放宽任何业务断言，未删断言，未 skip，未改产品**。

---

## 2. 本次原始回执与运行证据核对

### 2.1 宿主回执（dbb74021）

- `1 total / 0 passed / 1 failed`
- 失败消息：`Management 必须刷新到新世界真实 Roster`，`Expected: 5`，`But was: 6`
- 失败点：`ManagementShellPlayTests.cs` 旧第 1855 行
  `Assert.AreEqual(configuredRoster, TotalPersonCount(management), ...)`
- 失败发生在 NewGame 段的“管理刷新后 Roster 计数”断言；此前 Continue 段已通过。由于该断言失败，后续 NewGame 资源/旧实例检查未执行。

只读 `testResults` 复核到同一条失败消息与堆栈入口，说明当前 Editor 中保留的最近结果就是该次运行，不是旧 5 例结果。

### 2.2 证据日志（`R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`）

05:39:59Z 起的 CASE-C 运行实际留下了：

- `SAVE-BACKUP` / `SAVE-CAPTURE`：`runId=20261024T053959Z`，`bytes=13405`，`sha256=4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，备份回读校验通过。
- `CASE-C START`：`ingredients=0 food=180 batches=0 evidence=0 completedProjects=0 clock=0`
- `CASE-C persons`：`p1=person_a8e4… p2=person_c57c… p3=person_daaf… p4=person_de16… p5=person_d843…`（5 名普通角色）
- `CASE-C SAVED-FACTS`：`clock=16360 project=prj_test_01_3 progress=2 labor=80 ...`
- `SAVE-SUCCESS` / `SAVE-FAIL-EXPECTED` / `SAVE-RECOVERED` 均已完成
- `CONTINUE-ENTRY`：新 host/newShell、`clock=16360 speed=Paused`
- `CONTINUE-FACTS`：施工、资源、批次、证据、历史、Clock 全部连续
- **此后没有 `NEWGAME` 证据行**；紧接着是 `SAVE-RESTORE … matches-before=True` 与 `BASELINE-POST all-match`。这与“NewGame 段在 Roster 断言处失败、TearDown 正常恢复”完全吻合。

### 2.3 当前空闲与保护状态（只读）

- `editor`：`isPlaying=false, isPaused=false, isCompiling=false, isUpdating=false`（修复编译后复核仍空闲）
- `SampleScene`：`isDirty=false`
- `tests`：发现 6 条 `ManagementShellPlayTests`（原 5 + CASE-C），无重复
- 存档保护目录：
  - `20261024T053959Z/manifest.json`：`status=restored`、`restored=true`、13405B / `4b2b56…`，备份回读一致
  - `20261024T052704Z/manifest.json`：`status=protective-snapshot`（只读保护快照，按既有约定不阻断）
- 因此当前无 `captured` / `restore-failed` / 无 manifest 的未完成 run；未对存档做任何写回、删除或恢复。历史 650 缺口保持原记录，未恢复历史档。

---

## 3. 真实人口来源与 5/6 的集合差异

### 3.1 生产链路（只读源码核对）

1. `SaveSystem.NewGame()`（`Assets/Scripts/Save/SaveSystem.cs:225`）
   → `new PlayerCollection()` → `collection.InitializeDefaultCollection()`。
2. `PlayerCollection.InitializeDefaultCollection()`（`PlayerCollection.cs:70`）：
   - `OwnedHeroes.Count == 0` 时加载 `hero_001` 并 `OwnedHeroes.Add(hero)`；
   - `OwnedCharacters.Count == 0` 时按 `dm.InitialPersonRoster` 创建角色（配置 N 行 → N 名角色）。
3. `HeroSelectionUI.OnConfirm()`（`HeroSelectionUI.cs:179`）：
   - `SaveSystem.NewGame()`；
   - `collection.OwnedHeroes.Clear()` 后写入所选英雄 1 名；
   - `SaveSystem.Save(collection)` → `townUI.Initialize(collection)`。
4. `ManagementReadAdapter.ReadPersons()`（`ManagementReadAdapter.cs:73`）：
   - `VillageRuntimeBridge.BuildPersonInputs(source)`；
   - `BuildPersonInputs`（`VillageRuntimeBridge.cs:922`）先遍历 `OwnedCharacters`（非 null、非空 `PersonId`，含死亡），再遍历 `OwnedHeroes`（非 null、非空 `PersonId`）；
   - `ProductHomeReadAdapter` 使用同一输入口径。
5. `VillagePersonAvailability.Resolve` 只决定 `InVillage / Expedition / Unavailable`，**不会把死亡、远征或英雄从人口集合中删掉**；管理投影仍会显示（死亡为“不可用”，远征为“远征中（保留职责）”）。
6. `HeroInstance` 构造函数（`HeroInstance.cs:118`）调用 `PersonIdentity.NewPersonId()`，英雄身份是独立真实 `PersonId`，不是模板 `HeroId`。

### 3.2 配置事实（只读资产）

- `Assets/Resources/ScriptableObjects/Persons/PersonDefinitionCatalog.asset`：`InitialRosterOrder` 1–5 共 **5** 条 `PersonDefinition`。
- `Assets/Resources/ScriptableObjects/Heroes/hero_001_青鹿.asset`：**1** 个英雄模板（`HeroId=hero_001`）。

### 3.3 集合差异

- 旧 expected：`InitialPersonRoster.Count = 5`，语义是“初始普通角色配置数”。
- 新世界真实人物源：`OwnedCharacters` 5 个运行时 `PersonId`（CASE-C 证据中的 p1…p5，全部为新生成身份）+ `OwnedHeroes` 1 个运行时 `PersonId`（所选英雄）= **6**。
- 差异项就是 **Hero 那 1 个合法 PersonId**；不是旧世界 `p1…p5` 残留（旧测试在 1846 行已断言 `newPersonIds` 不含这 5 个旧 ID，且该断言在这次运行中已通过，失败发生在其后的计数断言）。
- 旧断言只比较 count，无法指出“少的是英雄”还是“多了旧人物”；本包改为集合等价断言后，任何遗漏/多出/旧残留都会以具体 PersonId 报出。

> 说明：本次回执未记录英雄的运行时 `PersonId` 字面值（证据行只打印了 5 名角色）。但 `NewGame → InitializeDefaultCollection` 必加 1 名英雄、`HeroSelection.OnConfirm` 再替换为 1 名所选英雄，是确定的单一生产路径；新断言会在宿主下一次单方法 check 中直接打印并比较完整集合，届时以运行证据为准，不在此预填 PASS。

---

## 4. 最小修复（仅测试）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（修改后 1961 行）

### 4.1 新增只读 helper

- `PersonIdsOf(source, member)`：从 `PlayerCollection.OwnedCharacters` / `OwnedHeroes` 独立读取合法 `PersonId`（跳过 null 与空 ID，不经过 `ManagementReadAdapter`）。
- `LegalPersonIdsFromSource(source)`：角色（含死亡）+ 英雄的完整合法集合。
- `SnapshotPersonIds(management)`：只读取管理投影的 `Persons` 身份列，作为 actual，不作为 expected。

### 4.2 替换失败断言

旧：

```csharp
var newPersonIds = ((IEnumerable)Get(newSource, "OwnedCharacters"))...
Assert.AreEqual(configuredRoster, newPersonIds.Count, "NewGame 初始名册人数必须等于 Authority 配置");
...
Assert.AreEqual(configuredRoster, TotalPersonCount(management), "Management 必须刷新到新世界真实 Roster");
```

新：

```csharp
var newCharacterIds = PersonIdsOf(newSource, "OwnedCharacters");
var newHeroIds = PersonIdsOf(newSource, "OwnedHeroes");
var newPersonIds = LegalPersonIdsFromSource(newSource);
Assert.AreEqual(configuredRoster, newCharacterIds.Count, "配置 N 行 → N 名角色");
Assert.Greater(newHeroIds.Count, 0, "必须包含所选英雄");
Assert.AreEqual(configuredRoster + newHeroIds.Count, newPersonIds.Count, "合法人口 = 角色 + 英雄");
Assert.AreEqual(newPersonIds.Count, new HashSet<string>(...).Count, "不得重复 PersonId");
foreach (string oldPersonId in oldPersonIds) Assert.IsFalse(newPersonIds.Contains(oldPersonId), ...);
...
var managementPersonIds = SnapshotPersonIds(management);
CollectionAssert.AreEquivalent(newPersonIds, managementPersonIds,
    "Management 人口投影必须与真实人物源逐 PersonId 一致");
Assert.AreEqual(newPersonIds.Count, TotalPersonCount(management), ...);
```

保留并加强的语义：

- 仍断言“配置 N 行 → N 名初始角色”，不写死 5。
- 仍断言 NewGame 真实替换人物源、`TownUI.PlayerData` 即新源。
- 旧世界残留排除范围从 p1…p5 扩大到**旧人物源的全部合法 PersonId（角色 + 英雄）**，并保留旧人物岗位职责不得泄露。
- 管理投影不再只数行数，而是逐 `PersonId` 与真实源集合等价；既不接受遗漏英雄，也不接受多出旧世界人物。
- 死亡/远征口径正确保留：expected 与投影都不按死亡/远征过滤（管理显示为不可用/远征中）。

### 4.3 证据增强

`NEWGAME` 证据行现在同时输出：

- `configuredCharacters`（Authority 配置角色数）
- `sourceCharacters` / `sourceHeroes`（真实源角色/英雄数）
- `sourceIds`（真实源完整合法 PersonId 集合）
- `managementIds`（管理投影实际 PersonId 集合）
- `roster`（实际合法人口数）

下一次运行可直接从日志判断集合是否逐项一致。

### 4.4 未改内容

- 未改 `ManagementReadAdapter` / `ProductHomeReadAdapter` / `PlayerCollection` / `SaveSystem` / `HeroSelectionUI` 等任何产品代码：没有证据表明产品在 Continue 后使用了陈旧缓存；本次失败可由测试 expected 口径单独解释。
- 未改 `SaveSlotGuard`、单槽备份/恢复、`[OneTimeSetUp]/[OneTimeTearDown]`、时间类型辅助函数、既有 5 例断言。
- 未新增/删除测试用例；仍为 6 例。

---

## 5. 编译与发现证据（本包实际执行）

1. 修改前 Editor 空闲：`isPlaying=false, isPaused=false, isCompiling=false, isUpdating=false`；`SampleScene.isDirty=false`。
2. 仅调用安全刷新：
   `AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport)` → 返回 `refresh-requested`。
3. 刷新触发域重载，插件连接短暂断开（请求未重发）；重连后只读查询：
   `isPlaying=false, isCompiling=false, isUpdating=false`。
4. Console 只读回读：
   - `logType == "Error"` 或 `message contains "error CS"`：**0 条**；
   - 与 `ManagementShellPlayTests` / `CS` 相关的 Warning：**0 条**。
5. 测试发现（只读）：`tests | where fullName contains "ManagementShellPlayTests"` = **6 条 PlayMode**，目标方法仍在列。
6. 编译后 `SampleScene.isDirty=false`；未使用 `rql-mutate`，未触发任何资产/场景保存。

---

## 6. 保护状态与未测项

### 6.1 保护

- 34 项 `workspace-baseline.json` 未修改；测试文件不在 34 项基线内。
- 最近证据：05:39:59Z `BASELINE-PRE total=34 diff=0 all-match`；05:40:01Z `BASELINE-POST total=34 same=34 changed=0 problems=0 all-match`。
- 受保护 TMP 字体本轮未触碰；历史证据 05:40:01Z `FONT-POST unchanged=True`。
- `20261024T053959Z` manifest `status=restored`，13405B / `4b2b56…` 与恢复前匹配；`20261024T052704Z` 为 `protective-snapshot`，均不构成未完成 run。
- 原存档历史 650 缺口仍按既有记录保留，本包未恢复历史档，也未读写用户单槽存档。
- 本包未运行测试，因此不产生新的 `BASELINE-PRE/POST`、`SAVE-CAPTURE/RESTORE` 或测试回执；保护结论沿用既有证据，不冒充本轮运行结果。

### 6.2 未测 / 待宿主 check

- 修改后的逐 PersonId 等价断言**尚未执行**；本报告不声明 CASE-C 已 PASS。
- NewGame 之后剩余断言（资源投影、旧建筑/职责不泄露、资源汇总、刷新不增减资源）在本次回执中仍未执行，需在下一次完整通过 Roster 段后继续暴露。
- 旧世界英雄 PersonId 的具体字面值、管理投影 6 个实际 PersonId 需由下一次运行证据确认。
- 历史 650 缺口（21075B/`650c45c4…` 未恢复）仍是既有事实，本包未处理。

### 6.3 交宿主

建议由主开发执行唯一单方法 check：

```text
PlayMode
RRPG.ManagementShell.Tests.ManagementShellPlayTests.ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak
```

运行前确认所有打开场景已保存、无未完成保护 run；运行后核对 `SAVE-CAPTURE/SAVE-RESTORE` 成对、`NEWGAME sourceIds/managementIds` 集合一致、`BASELINE-PRE/POST all-match`。若出现新失败，以宿主新回执为准，等主开发回执再判，不在本包越界继续。
