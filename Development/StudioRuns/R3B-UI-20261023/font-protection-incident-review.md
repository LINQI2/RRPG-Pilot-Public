# 受保护 TMP 字体变化取证复核（只读）

- 包：R3-B 只读取证包（制作人交接 a980518e 要求核查"保护字体何时变、为什么变"）
- 唯一授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot）；唯一 Unity 工程 `UnityProject/RRPG1.0game`
- 本包性质：**只读取证**。未运行测试、未 Refresh/Save/SetDirty、未重导入、未进 Play、未开场景、未 Prefab 实例化、未加载字体、未修改任何游戏源码/序列化资产/测试/基线；**未执行恢复**。
- 结论性质：本报告**不判定 R3-B 通过**，不给出新基线，结论中"推断"部分明确标注，未核清处保留"未知"。

---

## 0. 本包写入清单（全部产物）

| 路径 | 类型 |
|---|---|
| `Development/StudioRuns/R3B-UI-20261023/font-protection-incident-review.md` | 唯一产物（本文件） |

除本文件外**未写任何文件**；未产生小型快照 JSON（避免多余写入，取证事实全部内联于本文件）。

---

## 1. 复核范围的硬缺口（先说缺口，再说结论）

| 缺口 | 说明 | 影响 |
|---|---|---|
| **G1 本会话无哈希执行能力** | 本 DSH 会话工具集只有文件读写 / glob / grep / Unity MCP 只读查询，**没有 shell/exec，也无法在不触发编译或域重载的前提下计算 SHA256** | 本包**未能**重新计算字体与证据文件的 SHA256/bytes/mtime；详见 §6 |
| **G2 未使用 `script-execute`（主动放弃）** | 唯一可用的代码执行通道是 Unity `script-execute`（编译并运行 C# 片段）。但本包在 §4 已识别"每次域重载后 `GrowthDataInitializer` 的 `delayCall` 会调用 `AssetDatabase.SaveAssets()`"这条自动保存路径，若片段编译引发域重载，**存在再次改动受保护字体的现实风险**（与事故机理同型） | 为不制造第二次污染，本包不调用 `script-execute`；因此 G1 无法用该通道弥补 |
| **G3 原 f6ae 的 MCP 工具事件流不可见** | 本会话看不到原 f6ae 执行期的 tool call/参数/回执序列，只能看到 Editor Console 落地的结果行（§4）与 fixture 自写日志（§3） | 不猜日志名、不冒充"已审查原始调用"；原执行器侧调用清单属未核清 |
| **G4 `studio-test-record/66dd6cb1-…json` 不在本工作区** | `glob **/66dd6cb1*.json`、`glob **/*studio-test-record*` 在 Pilot 目录内均无结果（该类记录属 Studio 私有运行时，本包禁止访问） | 该 5 总 / 0 过 / 5 失败回执仅按任务给定事实登记，未由本包独立读取 |
| **G5 无字体字节备份** | 见 §5：授权位置内未找到任何字体字节备份/manifest | 无法做"原始字节 ↔ 当前字节"的逐字段差异定性 |

---

## 2. 基线事实（原样引用，未改）

- 文件：`Development/StudioRuns/R3B-20261022/workspace-baseline.json`
  - `at=2026-10-22T14:39:27.026Z`，`branch=ai/rrpg-r3b-population-pilot`，`head=054b00d3e38fe2e2e142bfdb2502de90f7991030`，`complete=true`，`consistency="status-stable; not an atomic filesystem snapshot"`
  - 内容为单行 JSON，共 **34** 条 `files[]`；首条即受保护字体：
    `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`，`status=" M"`，`bytes=14961033`，`sha256=3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`
  - 说明：该 JSON 为单行且被读取工具按 2000 字符截断，本包对 34 项清单的逐项核对采用 `shell-play-test-report.md` §7 的 34 行表（每条含路径 + SHA256，第 1 行与基线首条完全一致），并与 `management-actions-play-report.md` §4 的期望值交叉一致。
- 本包**未修改**该基线，也**未**用任何来源覆盖它。

---

## 3. 证据日志原样（`Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log`，共 16 行）

时间均为 UTC（fixture 写 `DateTime.UtcNow("O")`）：

| 行 | 时间（UTC） | 事件 |
|---|---|---|
| 1 | 2026-10-23T12:25:46.7448446Z | `BASELINE-PRE total=34 diff=0 all-match` |
| 2 | 2026-10-23T12:25:46.8821592Z | `FONT-PRE sha256=3c758e16…c404854` |
| 3–11 | 12:25:46.9 – 12:25:49.1 | 3 例盘存捕获/恢复，SHA `650c45c4…6ee5`，`matches-before=True` |
| 12 | 2026-10-23T12:25:49.2420689Z | `FONT-POST sha256=3c758e16… unchanged=True` |
| 13 | 2026-10-23T12:25:49.4023594Z | `BASELINE-POST total=34 same=34 changed=0 problems=0 all-match` ← **最后一次"完全一致"** |
| 14 | 2026-10-23T12:32:57.8833789Z | `BASELINE-PRE total=34 diff=2`：字体 `bytes expected=14961033 actual=14968490`；`sha256 expected=3c758e16… actual=e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8` ← **首次差异** |
| 15 | 2026-10-23T12:32:58.0392327Z | `FONT-POST sha256=e40b6d77… unchanged=False` |
| 16 | 2026-10-23T12:32:58.2012773Z | `BASELINE-POST total=34 same=34 changed=0 problems=2`（同上两条问题记录） |

### 3.1 "diff=2"≠"2 个文件"（重要口径）

`HashBaseline()` 对同一文件**分别**追加 `bytes` 与 `sha256` 两条 problem，因此 `diff=2 / problems=2` 精确对应**唯一 1 个文件**的两条问题记录：

- **实际差异文件数 = 1**（受保护 TMP 字体 1 项）
- 其余 **33 项** bytes + SHA256 全一致（`management-actions-play-report.md` §4 记为 `total=34 / same=33 / diff=1`，与本包口径一致）

### 3.2 第二次运行缺 `FONT-PRE` 的原因

第 2 次运行（12:32:57）日志只有 `BASELINE-PRE`（失败）→ `FONT-POST` → `BASELINE-POST`，**没有 `FONT-PRE`**：`OneTimeSetUp` 的 `Assert.AreEqual(0, problems.Count)` 在第 509 行先失败，跳过了 511–514 行的字体读取。这不是漏记。`FONT-POST unchanged=False` 是因为 `_fontBefore` 为 null（未赋值），此行的"False"不构成额外证据，`FONT-POST` 的真实信息只有 `sha256=e40b6d77…`。

---

## 4. Editor Console 实际事件（Unity 2022.3.62t9，只读 MCP 查询；时间 +08:00 = UTC+8）

查询方式：`console | where Sequence >= … | select Sequence LogType Timestamp Message`（只读；未清空 Console）。工程路径 `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game`，`isPlaying=false isCompiling=false isUpdating=false`（查询时）。

| 本地时间（+08:00） | UTC | Sequence | 事实 |
|---|---:|---:|---|
| 20:25:46.363 | 12:25:46.363Z | 19301 | `✅ 人物成长测试数据生成完成（测试值，未冻结）` — **`GrowthDataInitializer.CreateTestData()` 执行（内含 `AssetDatabase.SaveAssets()` + `AssetDatabase.Refresh()`）**，由 `[InitializeOnLoadMethod]` + `EditorApplication.delayCall` 在域重载后自动触发 |
| 20:25:46.369 | 12:25:46.369Z | 19302 | `[JourneyEventIllustration] Catalog 已存在（0 条）…` |
| 20:25:50.488 | 12:25:50.488Z | 19332 | 该轮最后一条 |
| — | — | 19333 | **Console 静默窗口 20:25:51 – 20:31:07**（约 5 分 17 秒无任何条目录入） |
| 20:31:07.244 | 12:31:07.244Z | 19333 | `✅ 人物成长测试数据生成完成` — 又一次域重载后的 `SaveAssets()` 路径 |
| 20:31:07.285–… | 12:31:07.285Z | 19335+ | 大批 `The character with Unicode value \uXXXX was not found in the [LiberationSans SDF] font asset…` 警告（`TitleText / ListTitle / SectionWorkplaceLabel / SectionParticipantLabel / infoText …`），说明该时刻渲染用的**不是**受保护字体 |
| 20:32:57.516 | 12:32:57.516Z | 19858 | `✅ 人物成长测试数据生成完成`（进入 Play 的域重载后） |
| 20:32:57.883 | 12:32:57.883Z | 19860 | `[ManagementShellPlayTests] BASELINE-PRE total=34 diff=2 …`（与 §3 行 14 同一条） |
| 20:32:58.04–.20 | 12:32:58.04–.20Z | 19861–19862 | `FONT-POST` / `BASELINE-POST` |
| 20:33:00.55 | 12:33:00.552Z | 19863–19864 | 最后两条（Warning） |
| 20:32:06.918 | 12:32:06.918Z | 19855–19857 | 执行器 Runtime 错误 `FileNotFoundException … SampleScene.unity`（原包只读哈希脚本路径写错；非项目源码编译错误） |

### 4.1 工程内可见的自动写入路径（只读代码审查，非事件证据）

在 `UnityProject/RRPG1.0game/Assets` 内 `grep` 结果：

1. **`Assets/Editor/GrowthDataInitializer.cs`**：`[InitializeOnLoadMethod] AutoSeedTestDataOnLoad()` → `EditorApplication.delayCall += CreateTestData`；`CreateTestData()` 末尾无条件 `AssetDatabase.SaveAssets(); AssetDatabase.Refresh();`（第 100–101 行），并对 12 个 `Growth` 资产 `EditorUtility.SetDirty(existing)`（第 113–115 行）。**每次域重载后都会执行**，`AssetDatabase.SaveAssets()` 保存的是**当时所有 dirty 资产**（不限于 Growth），这是唯一可确认的"每次重载都会落盘 dirty 资产"的工程内自动路径。
2. `Assets/Editor/JourneyEventPresentationCatalogInitializer.cs`：同为 `[InitializeOnLoadMethod]`，但 Catalog 已存在即 `return`，**不 SaveAssets**（Console 每次输出"已存在"即为该分支）。
3. `Assets/Editor/VillageV4GateTestRunner.cs`：`[InitializeOnLoad]` + `EditorApplication.update` 轮询 `TestResults/village_v4_run.request` 触发 `TestRunnerApi`。当前 `TestResults/` 下**只有** `BattleStage1.EditMode.results.txt`，**无** `village_v4_run.request/pending` → 该通道本次未自行发起测试。
4. `AssetDatabase.SaveAssets() / Refresh()` 还出现在 `ExcelImporter / BattlePresentationLayoutAuthoring / CharacterGrowthUIAuthoring / BattlePresentationCutoverAuthoring / GrowthDataInitializer / BattleGate0CardContentBuilder / GameDataInitializer / StaticConfigBuilder`（均为**菜单/批处理入口**，非 `InitializeOnLoad` 自动执行，需人工点菜单或外部调用才跑）。
5. **全工程 `Assets/**/*.cs` 中 `TryAddCharacters`、`ClearFontAssetData`、`ReadFontAssetDefinition`、`atlasPopulationMode` 赋值：0 命中**。没有任何工程脚本显式写受保护字体。
6. 测试侧：`ManagementShellPlayTests.cs` 对字体**只读**（第 444–449 行 `HashFile` = `File.OpenRead` + SHA256；第 511–514 `FONT-PRE`；第 520–522 `FONT-POST`；第 477–497 逐项 bytes+sha 比对），首次 `SaveAssets` 命中列表里**不含**任何 PlayMode 测试文件。
7. `RefreshNow` 出现在该测试文件多处（286/357/929/1029/1079 行）——是 UI/宿主自带刷新方法调用，不是 `AssetDatabase.Refresh`（对照 grep：`Tests/PlayMode` 下无 `AssetDatabase.*` 命中）。

**结论（推断，非证明）**：受保护字体在 12:30:59Z 被写入，落在"测试源码写入（12:30:54.642Z）→ 编译/域重载 → `GrowthDataInitializer` 于 12:31:07.244Z 记日志"这段 **Console 完全静默**的窗口内；工程内确实存在"重载后 `SaveAssets()` 落盘 dirty 资产"的自动路径，但**该实例的日志时刻（12:31:07.244Z）晚于字体 mtime（12:30:59.043Z）约 8.2 秒**，且字体未在 12:31:07 被再次写入（否则 mtime 会更新），故：
- 该 `SaveAssets()` 实例**不能**直接解释 12:30:59.043Z 这次写入；
- 12:30:59.043Z 到底由谁写入，**目前未知**：Console 在该毫秒无任何记录，原 f6ae 执行器的 tool call 序列在本会话不可见（G3），Editor 主日志位于用户目录（未授权，未读）。

---

## 5. 备份线索核查（只读，限定授权位置）

检索范围：`Development/**`、`UnityProject/RRPG1.0game/Assets/**`（含 `Assets/Editor/StudioRecoveryBackups`），不遍历仓库外部目录/凭据目录。

| 检索 | 结果 |
|---|---|
| `glob **/*SourceHanSans*` | 仅现存资产与源 OTF：`Assets/Art/Fonts/SourceHanSansCN/{RRPG_SourceHanSansCN_Regular_TMP.asset, .asset.meta, RRPG_SourceHanSansCN_Medium_TMP.asset, SourceHanSansCN-Regular.otf, SourceHanSansCN-Medium.otf, …}` + `.codely-cli` 扩展包内的 `SourceHanSansSC-*.otf`（工具包自带字体，非 TMP 资产） |
| `glob **/*.bak` | 0 命中 |
| `glob **/*font*` | 仅 `Library/PackageCache/cn.tuanjie.codely.bridge@1.0.64/Editor/Bridge/fonts.meta`（与目标无关） |
| `Assets/Editor/StudioRecoveryBackups/**` | **仅 3 个文件**：`2026-10-23T11-49-56-028Z/SampleScene-unsaved.scene`、同名 `.scene.meta`、目录 `.meta`。**无字体字节、无 manifest** |
| `grep Development: 14961033 / 3c758e16d62ef8bd` | 仅 4 处文本记录（基线 JSON、本目录 2 份报告、证据日志），**均非字节副本** |
| `grep Development\|Assets: StudioRecoveryBackups` | 仅报告中"未触碰该备份目录"的文字声明，无字体备份引用 |

**结论**：授权位置内**不存在**可用的字体字节备份，**不存在**与其对应的 manifest/hash 记录。基线 JSON 里的 34 项 SHA256 是"哈希清单"，不是可恢复的字节副本；按任务约束也**不得**把 Git 对象/HEAD 版本当作"原未提交备份"。
→ 因此 §7 的"字段级差异定性"**无法完成**，只能记录当前事实。

---

## 6. 受保护字体：当前只读事实（**不做**哈希与逐字段差异声称）

只读方式：`read` 该文件有限行 + `grep` 定位字段（**未加载字体资产、未调用 TMP API、未 TryAddCharacters、未 SaveAssets**）。

| 事实 | 值 | 说明 |
|---|---|---|
| 路径 | `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | 存在 |
| 文本结构 | 共 13305 行；首对象为 `Texture2D`（`m_Name: RRPG_SourceHanSansCN_Regular_TMP Atlas 5`，1024×1024，`m_CompleteImageSize: 1048576`），后续内嵌多个 atlas `Texture2D` 与 `MonoBehaviour`（TMP_FontAsset）段 | 单文件内嵌图集数据，故体量约 15 MB |
| `m_AtlasPopulationMode` | **1（Dynamic）** | **仅当前事实**；不足以推出"自动字形扩充就是本次变化原因"（见 §7 禁止项） |
| `m_GlyphTable` | 646 条（行 220–9909，每字形 15 行：`m_Index / m_Metrics(6) / m_GlyphRect(4) / m_Scale / m_AtlasIndex / m_ClassDefinitionType`；9690/15=646） | 行区间推算，非 API 读取 |
| `m_CharacterTable` | **646 条**（`grep '^  - m_ElementType:'` 精确计数 646；行 9911–12494） | 字符数与字形数相等：当前不存在"有字符无字形"或反之的缺口 |
| 字符表首项 | `m_Unicode: 24352`（张）、`m_GlyphIndex: 13136`；随后 19977(三)/26446(李)/22235(四)/26519(林)… | 表序 = 插入序线索，但**不能**据此断定插入时点或插入者 |
| Atlas 槽位 | `m_AtlasTextures` 7 个有效引用 + 1 个 `{fileID: 0}` 空槽；`m_AtlasTextureIndex: 6`；`m_IsMultiAtlasTexturesEnabled: 1`；`m_AtlasWidth/Height: 1024`；`m_ClearDynamicDataOnBuild: 0` | 多图集已用到索引 6 |
| `m_FaceInfo` | FamilyName `Source Han Sans CN`，Style `Regular`，`m_PointSize: 90`，`m_UnitsPerEM: 1000` | 采样参数 |
| `m_SourceFontFileGUID` | `a76da0bba34c81c429c5bf92925a9c60` | 指向同目录 `SourceHanSansCN-Regular.otf` 家族（未打开验证） |
| bytes / SHA256 / mtime（当前） | **本包无法计算**（G1/G2）；最近一次外部事实：`bytes=14968490`、`sha256=e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8`、`mtime=2026-10-23T12:30:59.0431402Z`（`management-actions-play-report.md` §4）；主开发 2026-10-23T14:10:47Z 固定状态 `complete=true` 时"仍相同变化" | 需宿主用"不触发重载的只读哈希工具"复核，见 §9 |

**相对基线不可回避的数字**：`14968490 − 14961033 = +7457` 字节（约 +0.05%）。在 646 字形/646 字符、7 图集、内嵌纹理的 YAML 里，这个增量既可能来自字形/字符表新增，也可能来自图集重排后的数据与浮点序列化差异——**无原字节即为不可判定**，本包不选择其中任一种说法。

---

## 7. 定性（已知 / 推断 / 未知）

### 7.1 已知（有原始证据行）

1. **最后一次"字体完全未变"的可核验时刻**：`2026-10-23T12:25:49.2420689Z`（`FONT-POST unchanged=True`，SHA `3c758e16…`），同一轮 `12:25:49.4023594Z` 34 项 `problems=0`。
2. **首次观测到差异的时刻**：`2026-10-23T12:32:57.8833789Z`（`BASELINE-PRE diff=2`，bytes 14968490 / SHA `e40b6d77…`）。
3. **文件系统 mtime**：`2026-10-23T12:30:59.0431402Z`（原包以 Studio 文件系统读取，落在上述两点之间）。
4. **该窗口内 Console 完全静默**：`12:25:50.488Z` 之后下一条为 `12:31:07.244Z`（`人物成长测试数据生成完成`），`12:30:59.043Z` 处无任何 Editor 日志。
5. **工程内存在自动落盘路径**：`GrowthDataInitializer` 每次域重载后 `SaveAssets()`（§4.1-1），在该窗口的两次重载中分别于 `12:25:46.363Z`、`12:31:07.244Z` 记日志。
6. **工程内无脚本写字体**：`TryAddCharacters` / 字体持久写 API 命中 0；测试对字体只读；`Tests/PlayMode` 无 `AssetDatabase.*`。
7. **两次 34 项比对都只指向同一文件**：第 1 次 `diff=0`；第 2 次 `diff=2`（=1 文件 2 条问题记录）。
8. **`RefreshAssets` 与 `12:30:59.043Z` 时段重叠**：测试源码写入 `12:30:54.6427233Z`、编译产物 `BattleStage1.PlayModeTests.dll` 写入 `12:31:00.9037914Z`（原报告 §5 实测），字体 mtime 夹在这两者之间——**仅时间相关，不构成来源证明**。

### 7.2 推断（可被推翻）

- **I1（中等置信）**：写入发生在"编译 / 域重载"过程本身（`RefreshAssets → 导入 → 编译 → 重载`）内，因该时段 Console 无记录、且恰在 DLL 落盘前后，符合"Editor 内部资产管线/重载期落盘"的形态，而非人工或脚本显式写字体。
- **I2（低-中置信）**：若字体在重载前已是 dirty（例如 Play 期间 TMP 动态图集在 Editor 内存里扩充字形并置脏），则重载期/重载后 `AssetDatabase.SaveAssets()` 会把 dirty 状态落盘。**但**：`12:31:07.244Z` 那次 `SaveAssets()` 之后字体 mtime 未前进到 `12:31:07`，说明在 `12:31:07` 时字体已不 dirty（写入已在 `12:30:59` 完成）——所以**不能**把这次 `SaveAssets()` 当作本次写入的执行者。
- **I3（低置信）**：`12:30:59.043Z` 那次写入可能是 Unity 自身在重载/导入期对"已变更但未落盘"资产的一次 flush。无直接证据。

### 7.3 未知（不得猜测）

- **U1 写入主体**：哪个进程/代码路径在 `12:30:59.043Z` 写入了该文件——**未知**。原 f6ae 的 tool call 序列不可见（G3），Editor 主日志不在授权目录。
- **U2 差异内容**：`+7457` 字节具体改变了哪些字段（字形表？字符表？图集像素？序列化格式？）——**未知**，无原字节不可判定。
- **U3 是否会影响字形渲染正确性/是否有字符被替换**——未知（当前 646 字形 / 646 字符闭合，但无法与原有集合比对）。

### 7.4 明确禁止的推论（本包不做）

- ❌ 不以 `m_AtlasPopulationMode: 1 (Dynamic)` 直接断言"自动字形扩充是原因"；
- ❌ 不把"mtime 与 Refresh 时段重叠"当作因果证明；
- ❌ 不把 `FONT-POST unchanged=False`（第二次运行的 null 基线产物）当作额外证据；
- ❌ 不用 Git HEAD / 旧阶段文件 / 哈希清单冒充原始字节备份。

---

## 8. 最小安全恢复选项（**均未执行**，恢复需制作人精确授权）

| 选项 | 做法 | 风险 | 前置条件 |
|---|---|---|---|
| **A. 维持现状，只停受影响执行**（当前默认） | 不改字体、不改基线；`ManagementShellPlayTests` 等保护前置继续失败，受影响执行保持暂停 | 0 写风险；但保护门禁持续红，业务用例拿不到真实回执；时间越久"原始差异"越难补证 | 制作人接受"暂停 + 继续取证" |
| **B. 若日后找到字节级原始副本**（本包未找到） | 制作人授权 → 关闭 Play / 停止 Editor 侧保存 → 用该字节写回 → 立即双哈希校验（应为 `3c758e16…` / `14961033`） | 写回本身是一次受保护资产写入；若 Editor 仍开且处于重载窗口，可能再次被 dirty 落盘覆盖；若"副本"未经哈希证实等于原基线，等于引入新污染 | **必须先证明副本 SHA256 == 3c758e16…**（仅"存在某路径"或"HEAD 版本"不算） |
| **C. 接受新哈希 + 重定义基线** | 由制作人明确接受 `e40b6d77…` / `14968490` 为新保护基线并另行授权改基线 JSON | 会**永久掩盖**原因不明的写入路径；必须同时定位 U1，否则同类事故会再次发生并使新基线再次失效 | 制作人精确授权 + U1 已定位或明确"接受未知" |
| **D. 让 TMP 重新打包/重建图集** | 主动重生成字体资产 | 产生第三个 hash，且丢失现有差异证据；在未授权时属破坏证据 | 仅制作人明确要求时 |
| **E. ❌ 用 HEAD / 旧阶段文件恢复** | — | 不满足"原未提交基线"（任务已限定），会引入另一份非基线字节 | **不允许** |

调查侧（无需审批、可继续做）：在**不进入 Play、不 Refresh、不 Save** 的前提下由宿主用固定只读哈希工具复核当前字体 SHA/bytes（§9），并补齐原 f6ae 执行器的 tool call 清单（若宿主侧可见）。

---

## 9. 需要宿主/主开发补的最后一步（本包无法完成）

1. **字体当前 SHA256/bytes 复核**：用不触发域重载的只读哈希能力（如宿主固定只读 `git status+sha256` 工具，或 Studio 侧 `Get-FileHash` 等价能力）复核
   `D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game\Assets\Art\Fonts\SourceHanSansCN\RRPG_SourceHanSansCN_Regular_TMP.asset`
   期望（若本包调查未改变文件）：`14968490` / `e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8`。
2. **证据文件原样哈希**（本包 G1 未完成）：`workspace-baseline.json`、`management-shell-play-evidence.log`、本目录 5 份报告 md，以及 G4 所指 `studio-test-record/66dd6cb1-91c0-4a48-93fd-ab293ffea8aa.json`。
3. **原 f6ae 工具事件清单**（G3）：若宿主侧保留执行器调用记录，请按时间戳对齐 `12:28:00Z–12:31:10Z`，确认期间是否有 `RefreshAssets` / `script-execute` / 菜单项调用落在 `12:30:59.043Z`。
4. 若第 1 步结果与期望不同，说明**本包调查期间字体又发生变化**，须立即停止一切执行并按 §8-B 走授权流程。

---

## 10. 本包边界自证

- 只读操作：`glob` / `grep` / `read`（文本与字体 YAML 有限行）/ Unity MCP 只读查询（`rql-query`）。
- **未执行**：`tests-run`、`script-execute`、`RefreshAssets`、`SaveAssets`、`SetDirty`、重导入、进 Play、开/关场景、Prefab 实例化、加载字体资产、`TryAddCharacters`、任何 git 写操作、任何恢复/覆盖。
- **未修改**：游戏源码、序列化资产（含字体）、测试、基线 JSON、证据日志（本文件之外 0 写入）。
- 未访问正式工程 / Studio 私有 runtime / 认证目录 / 其他工作区；未安装组件、未改权限、未建付费子 Agent。
- 本报告仅证明"取证与线索核查到此"，**不证明 R3-B 通过**，不代表制作人已批准任何恢复。
