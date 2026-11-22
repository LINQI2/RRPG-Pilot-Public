# DSH 结果：Initial World 产品体验修复（A/C/B）

日期 2026-11-05。分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3…`，未 Commit/Push。
**最终状态：Implemented（代码已改、编译通过）/ Verification PARTIAL / 字体门禁在第 3 段拦截 → 按规格停止后续 Unity 操作。**

## 0. 结论先行

- A/C/B 三块代码均已实施，`AssetDatabase.Refresh` 后**编译零错误**（Console err=0，isCompiling=false）。
- 门禁段结果：step01 `PASS`；step02（编译）`PASS`；**step03（PlayMode 视觉原型回归）`end` 返回 exit=2 / CHANGED**。
- 按工作包与规格：**已立即停止全部后续 Unity 操作**，保留 incident，未复测、未 capture、未自动恢复。
- 因此：A/C/B 的**新功能专项测试尚未编写/运行**，Save→Continue、设置菜单、资源页导航等验收证据**尚缺**。

## 1. 字体门禁执行记录（真实退出码）

| 段 | Label | begin | 该段 Unity 动作 | end | 结果 |
|---|---|---|---|---|---|
| 01 | `iw-productfix-step01` | 0 | 只读 Editor 身份/场景/字体/Console | 0 | PASS |
| 02 | `iw-productfix-step02` | 0 | `AssetDatabase.Refresh` + 等编译 + Console 回读 | 0 | PASS（编译零错误） |
| 03 | `iw-productfix-step03` | 0 | PlayMode `VillageVisualPrototypePlayTests` 2 例 | **2** | **CHANGED → 停止** |

records 路径：`Development/StudioRuns/Initialization-20261105/font-protection/records/`
（`…080341Z-iw-productfix-step03-begin.json` 等 6 个文件）。

### 漂移事实

| 项 | 值 |
|---|---|
| 基线 SHA / 大小 | `f0f5b901…` / 14,969,377 B |
| 漂移后 SHA / 大小 | `d19fd6a1…` / 14,972,511 B（**+3,134 B**） |
| 写入时刻 | 2026-11-05T08:03:25.912Z（北京时间 16:03:25） |
| incident 目录 | `Development/StudioRuns/Initialization-20261105/font-protection/incidents/20261105T080341Z-iw-productfix-step03/` |
| incident 内容 | `incident.json` + `font-after.asset`（14,972,511B 完整字节） |

### 与本轮前置状态的因果关系（重要）

进入本轮时 Editor **处于 Play 模式**，字体资产在内存中已 `dirty=1`、字符数 **648→654**
（制作人 15:05 游玩时动态补字，磁盘尚未落盘）。退出 Play 后 dirty 仍未清除（step01 end 仍 PASS）。
该批待落盘字形在 step03 的 PlayMode 运行中被持久化，形成本次 +3,134B。

- 这不是新代码写字体：本轮未调用任何字体写入 API，也未 `ClearDirty`。
- 但**我不能证明**具体落盘触发点（无逐写日志），与历史三次漂移同型。

## 2. 本轮代码改动（均已编译通过）

| 文件 | 改动 | 现 SHA256 |
|---|---|---|
| `Village/Runtime/ProductHomeSnapshot.cs` | 新增 `DrawsStandaloneEntityInA`（默认 false） | `844d1ad4…` |
| `Village/ProductHome/ProductHomeReadAdapter.cs` | 注册地点按真实实体资格给值：仅未绑定建筑的 `ProjectSite` 为 true；合成虚拟地点保持默认 false | `9ea50e9d…` |
| `Village/ProductHome/ProductHomeWorldHost.cs` | 新增 `ShouldDrawPlace`：A 档按 `DrawsStandaloneEntityInA`，B 档仍按 `DrawsWorldEntity`；`RemoveMissing` 与创建使用同一判定 | `f5ff39ac…` |
| `Village/ProductHome/ManagementView.cs` | 资源行改为纯展示（不再整行导航、删除旧提示）；新增显式「查看岗位」按钮（携带完整 WorkplaceId，目标不可用则禁用并显示原因） | `226f44b6…` |
| `Village/ProductHome/HomeProductShell.cs` | 顶栏新增「设置」；透明全屏遮罩 + 三动作菜单（保存 / 保存并返回主菜单 / 关闭）；busy guard、状态反馈、`SurfaceModalActive` 纳入设置、`OnDisable` 清理 | `dd5db04b…` |
| `UI/MainMenuUI.cs` | 新增 `ShowMainMenuAfterHome()` 薄返回接缝；`EnterTown` 重新激活既有 `ProductHomeWorld`（不新建世界） | `35a9f663…` |

行为要点：
- **A**：虚拟 ActivityPlace/Tavern 类不再创建占位视图；Place 身份、锚点、人物到达/现场状态保留（锚点在 `DrawsWorldEntity` 判断之前计算，未改动）。
- **B**：`_prototype ? DrawsWorldEntity : DrawsStandaloneEntityInA`，B 对照表现语义不变。
- **C**：显式按钮精确定位 `WorkplaceId`，`NavigateResourceRow` 不清 `SelectedPersonId`，无派工/撤职/库存/时间写入。
- **B 设置**：保存复用 `SaveSystem.Save(town.PlayerData)`；保存前检查 `VillageRuntimeBridge.CanSaveWorld` 以给出可读失败原因；
  「保存并返回」先确认 `MainMenuUI` 存在再保存，成功后调用薄返回接缝。

## 3. 真实验证

| 项 | 结果 | 证据 |
|---|---|---|
| 编译 | **0 error**（`isCompiling=False`，Console `err=0`） | step02 回读 |
| `VillageVisualPrototypePlayTests`（2 例） | **2/2 Passed**，2.0s | step03 原始响应 |
| `ActivityPlacesCannotReflowRegisteredWorkplaces` | Passed | 同上 |
| `ProfilesPreserveEntitiesFootprintsSelectionAndDomain` | Passed | 同上 |

**未执行**：A/C/B 新功能专项测试（资源行点击/按钮定位、设置菜单开关与遮罩、保存失败/成功/返回主菜单、Save→Continue 一致性、
Compiler/validate、EditMode 相关套件）。这些因门禁拦截**没有运行**，不得写成 PASS。

## 4. 流程偏差（如实登记）

1. **改前未做字节备份**：6 个既有文件在编辑前未复制备份、未记录改前 SHA（工作包要求）。
   已补救：现记录当前 SHA（上表）；原始内容可按本文 §2 的改动逐条反向还原（每处替换均为精确文本替换）。
   这是本轮流程疏失，不隐瞒。
2. **首次身份读取时调用了 Refresh 而未先起门禁段**（发生在 step01 之前）。已记录；此后所有 Unity 操作均在门禁段内。

## 5. 需要制作人/主开发决策

**字体是否再次作为新基线采纳（`d19fd6a1… / 14,972,511`）？**

- 成因与本轮代码无关：是进入本轮前 Play 会话遗留的 6 个动态字形落盘。
- 若采纳：我可用既有 `font-guard.ps1 capture -Force` 重设基线（需明确授权），随后继续跑 A/C/B 专项测试。
- 若不采纳：按规格保持停止，不恢复、不手改，等待进一步指示。

## 6. 状态

- **Implemented**：A / C / B 代码（编译通过）。
- **Verified**：编译零错误；A/B 视觉原型回归 2/2。
- **Product Acceptance Pending**：全部三项的用户体验。
- **Blocked（按门禁规程）**：新功能专项测试与后续 Unity 验证。
- **Deferred**：字体写入成因仍未定位（第四次漂移已发生并被门禁捕获）。

## 7. 续执行（2026-11-05 18:00 后，制作人裁决后）

### 7.1 基线更新（已完成一次）

按要求先做安全检查再更新：磁盘为 `d19fd6a1…/14,972,511` 且自 08:03:25Z 未再变动；静态结构核对
654 字符、9 个文档块（1 FontAsset + 1 Material + 7 Texture2D）、7 条图集引用 + 1 空槽、`m_AtlasTextureIndex=6`、
`m_FaceInfo` 存在 —— 与漂移前同构，无截断/引用断裂。随后执行 `capture -Force`，新基线写入
`font-protection/baseline.json`（备份同步更新为 14,972,511B）。
**两个 incident 全部保留**：`20261105T080341Z-iw-productfix-step03`、`20261105T100237Z-iw-productfix-step05-tests`。

### 7.2 三项修复的真实专项验证（5/5 Passed）

| 用例 | 覆盖 | 结果 |
|---|---|---|
| `AProfileHidesVirtualActivityPlaceButKeepsFactsAndBRoundtrip` | A 档虚拟 ActivityPlace 无可见实体；Place/人物/建筑视图不变；B 档仍绘制；A→B→A 往返；Domain 事实不变 | **Passed** |
| `ResourceRowIsPlainAndViewWorkplaceNavigatesPrecisely` | 资源行普通点击不导航；「查看岗位」按完整 WorkplaceId 切到职责页并保留 Person 选择 | **Passed** |
| `SettingsSaveFailureStaysAndSuccessReturnsToMainMenu` | 设置打开纳入模态阻断；保存失败留页并显示原因；保存并返回失败不跳转；成功后返回真实主菜单 | **Passed** |
| `ActivityPlacesCannotReflowRegisteredWorkplaces` | 既有 B 语义回归 | Passed |
| `ProfilesPreserveEntitiesFootprintsSelectionAndDomain` | 既有 A/B 领域与布局回归 | Passed |

（`VillageVisualPrototypePlayTests` 整类 5/5，0 failed，0 skipped，3.5s。）

### 7.3 门禁第二次拦截（立即停止）

- `step05`（上述 5 例所在段）`end` 返回 **exit=2 / CHANGED**：
  `d19fd6a1…/14,972,511` → **`32b87c67…/14,973,886`（+1,375 B）**，写入时刻 `2026-11-05T10:02:22Z`。
- `step06` 的 `begin` 随即返回 **REFUSED_PRE_CHANGED（exit=2）**。
- 按规格：**已停止全部后续 Unity 操作**；未 capture、未恢复、未清 dirty。
- 因此 **Save→Continue 回归、Compiler/validate、EditMode 相关套件本轮未执行**。

## 8. 字体动态写入：机理结论与长期建议

### 8.1 已证实的机理（三次独立观测一致）

| 观测 | 事件 | 字节变化 |
|---|---|---|
| #1 | 制作人 15:05 游玩（内存 648→654 字符，dirty=1，磁盘未变） | 0（待落盘） |
| #2 | step03 PlayMode 运行 | +3,134 |
| #3 | step05 三轮 PlayMode 运行 | +1,375 |

结论：**该 TMP 资产是 `m_AtlasPopulationMode = Dynamic`**。UI 渲染到尚未收录的汉字时，TMP 在运行时把字形补进内存图集并
把资产标脏；随后的 Play 结束 / 测试 teardown 触发的资产保存把它落盘。字节增量随“新出现的字”而变，不是固定值。

尚未证明的只有**具体落盘调用点**（无逐写日志）；但“每次跑 PlayMode 就增长”这一行为已被三次观测复现，
机理层面可以确定不是随机损坏，也不是内容丢失。

### 8.2 长期修复建议（低侵入，未实施）

1. **运行时字体实例隔离（首选）**：启动时把受保护字体复制为运行时实例（`Instantiate`），
   仅该实例保持 Dynamic 并接收新字形；项目内 `.asset` 保持不被写入。UI 权威指向实例，随进程销毁。
2. **源资产改静态 + 运行时 fallback**：源资产设为不可增长，缺字走运行时动态 fallback 字体。
   风险：需确认静态资产不会在编辑器下仍被 `TryAddCharacters` 修改。
3. **编辑器侧防线**：在 `AssetModificationProcessor.OnWillSaveAssets` 中拒绝对该路径的保存（现有
   `StudioTestResourceScope` 已有同类钩子，可复用其思路但不复用其“自动恢复”语义）。

不建议：删除中文字形、改换字体、把字体改成 Static 而不加 fallback —— 都会造成汉字缺失。
该项属于表现架构改动，**应按约定单独出方案，不在本包扩大实施**。

## 9. 需要制作人决定

1. 是否**再次**采纳 `32b87c67…/14,973,886` 为新基线（本次已是第二次因 PlayMode 增长）。
   若不解决机理，每次跑 PlayMode 都会继续 +N 字节。
2. 是否把 §8.2 的**运行时字体实例隔离**作为独立小任务排期（推荐先做，再继续任何 PlayMode 验收）。

在得到指示前：不 capture、不恢复、不继续 Unity 测试。

## 10. 门禁升级：接受合法 Dynamic Atlas 增长（2026-11-05 18:36）

制作人决策方案 A（不换静态字体、不做运行时实例隔离）。门禁由"哈希必须相等"改为结构判定：

- 新增 `scripts/font-guard-core.ps1`（纯函数）；重写 `scripts/font-guard.ps1`（新增 `classify`）。
- 结果分四种：`PASS` / `LEGITIMATE_GROWTH`（自动记录并更新基线，exit 0）/ `INCIDENT`（exit 2）/ `NEEDS_REVIEW`（exit 4）。
- 合法增长须同时满足：块结构不变；字符表严格前缀；旧字形逐字节不变；字符数=字形数；新增字符与新增字形一一对应；
  Material / FaceInfo / 源字体 GUID 不变；Atlas Population Mode 仍为 Dynamic；图集引用只允许尾部追加；
  新增字形矩形（含 12px 栅格余量）之外像素逐字节不变；新矩形不与旧矩形重叠；新增矩形内有非零像素。
  任一项无法判定 → NEEDS_REVIEW，不自动接受。

自测 `scripts/font-gate-selftest.ps1`：**9/9 PASSED**。

| 样本 | 期望 | 实际 |
|---|---|---|
| 历史 648→654（新增 静增\|写钮榜） | LEGITIMATE_GROWTH | LEGITIMATE_GROWTH |
| 历史 654→656（新增 设菜） | LEGITIMATE_GROWTH | LEGITIMATE_GROWTH |
| 删旧字符 | INCIDENT | INCIDENT（character table shrank） |
| 删旧字形 | INCIDENT | INCIDENT（font block missing） |
| 改旧字形 | INCIDENT | INCIDENT（existing glyph modified at 0） |
| 破坏旧图集区域 | INCIDENT | INCIDENT（pixels changed outside new glyph rects） |
| 破坏 Material | INCIDENT | INCIDENT（material changed） |
| 删图集页 | INCIDENT | INCIDENT（atlas page removed） |
| 改 FaceInfo | INCIDENT | INCIDENT（faceinfo changed） |

真实运行：`verify` 将当前 654→656 判为合法增长并自动采纳，基线更新为 `32b87c67…` / 14,973,886 B；
写 `growth/20261105T103655Z-iw-productfix-growth-adopt.json`，旧基线归档 `history/d19fd6a1….asset`；
两条历史 incident **保留未删**。begin/end 冒烟：均 exit 0，records 各一条。

边界遵守：未改字体资产、未改 UI/产品代码、未实施方案 B/C、未删除 incident、未 Commit/Push。

