# 人口行姓名渲染：恢复后字体安全核验与单例门禁预检（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。仅本目录与已连接的 Studio Unity MCP 工程，未访问未授权工程 / 认证目录 / 其他工作区。
- 预检时刻：**2026-10-26T07:17:40Z**（只读脚本实测 `utcNow`）。
- 本包范围：**只读核验 + 仅新增本文件**。不重做实现、不改代码 / 测试 / Prefab / Scene / 字体 / 图集 / 尺寸 / Prefs / 存档 / guard manifest；不 `AssetDatabase.Refresh`、不 `tests-run`（模型不启动）、不 `rql-mutate`、不 Shell、不 Git 写、不全局 `SaveAssets/SaveChanges`、不 `SetDirty`、不清 dirty、不重导入字体、不覆盖内存对象、不打开 PrefabStage、不保存任何资产或场景、不启停 Editor。
- 上游背景（主开发转述，非本执行层可读的 Studio 私有 runtime）：旧 run `b17cd5c1…` 的结论为 **rejected**，错误是 `Font already has pending edits`；测试**未发送**，**不是断言失败**。本包只补一次缺失门禁，不重演实现。
- 结论前置：**字体磁盘字节 = 基线、内存 0 dirty、工程空闲、34 项 / 单槽 / guard / Prefab / 测试源码 / DLL 全部一致**，门禁可安全交给宿主做 **1920 单方法 PlayMode**；本文件**不预填**该方法结果。

---

## 1. 身份与工程空闲（只读实测，2026-10-26T07:17:40Z）

| 项 | 实测值 | 判定 |
|---|---|---|
| Unity | `Application.unityVersion = 2022.3.62t9` | 与授权身份一致 |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selectionCount=0` | 空闲（pending 状态为空，无编译 / 导入 / 播放） |
| 场景 | `sceneCount=1`；`Assets/Scenes/SampleScene.scene`：`dirty=False`、`roots=5`、`loaded=True` | 干净，**不因重开假设 clean**：本次为实测非假设 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null` | 未打开（不打开 PrefabStage，避免原自动序列化回写副作用重演） |
| 原生 TestRunner 活动运行 | `TestRunnerApi.IsRunActive()`（`UnityEditor.TestTools.TestRunner.Api`，**non-public static**，反射实测）= **False** | 无活动运行，可安全提交 |
| 可见 `pending*` / `activeRequest*` | 对该类型全部成员（public + nonpublic）按名过滤 `pending` / `activerequest`：**命中 0 个** | 该 API 版本无可见投递队列，如实记录；**不据此推断“有 / 无排队”**，仅以 `IsRunActive=False` + `pending=False` + 场景干净作为判据 |
| Screen | `1920x1080`、`Windowed` | 满足宿主 1920 单例前置；**未切 1280**（1280 已由制作人取消，本包不改尺寸 / Prefs） |
| Console | `Error` / `Exception` = **0 条** | 无项目错误 |
| `testResults` | **0 条** | 本 Editor 会话无持久测试结果；旧 `b17cd5c1…` 的 rejected 与历史回执均**不能**当作本次门禁证据 |

---

## 2. 受保护字体：磁盘 / 内存 / 子资产（本包最关键门禁）

### 2.1 磁盘（只读字节 + SHA256 重算）

| 项 | 实测值 | 要求值 | 判定 |
|---|---|---|---|
| 路径 | `UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset` | 同 | ✅ |
| bytes | **14961033** | 14961033 | ✅ |
| SHA256 | **`3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`** | 同 | ✅ |
| 文件 mtime | `2026-10-10T13:23:55.9520319Z` | — | 远早于本日所有会话，**今日未被写过** |
| `.meta` mtime | `2026-10-10T12:10:57.0892496Z` | — | 同样未动 |

### 2.2 已加载对象与子资产（`AssetDatabase.LoadAllAssetsAtPath` + `EditorUtility.IsDirty`，全程只读）

`subassetCount = 9`，**`dirtySubassetCount = 0`**（未调用任何 Save / SetDirty / 重导入）：

| # | 类型 | name | `EditorUtility.IsDirty` |
|---|---|---|---|
| 1 | `TMP_FontAsset` | `RRPG_SourceHanSansCN_Regular_TMP` | **False** |
| 2 | `Material` | `RRPG_SourceHanSansCN_Regular_TMP Material` | **False** |
| 3–9 | `Texture2D`（7 张 atlas） | `… Atlas`、`… Atlas 1` … `… Atlas 6`，均 `1024x1024`、`Alpha8` | 全部 **False** |

| 字体对象成员 | 实测 | 判定 |
|---|---|---|
| `EditorUtility.IsDirty(font)` | **False** | 无内存待保存编辑 |
| `atlasTextures.Length` | **8**（索引 0–6 为上述 7 张真实 atlas，**索引 7 为 `null`**） | 如实记录该空闲槽；`null` 槽不构成 dirty，也不因本包被写入 |
| `material` | `RRPG_SourceHanSansCN_Regular_TMP Material`，`shader=TextMeshPro/Mobile/Distance Field`，`dirty=False`，`_MainTex` 指向 `… Atlas` | 无待保存编辑 |
| `atlasPopulationMode` | `Dynamic` | 与历史记录一致 |
| `characterLookupTable.Count` | **644** | 与历史 `644 字` 一致 |
| `sourceFontFile` | `SourceHanSansCN-Regular` | 未变 |

**判定：字体磁盘字节 = 基线、内存与 atlas / material 子资产 0 dirty。**未出现 `Font already has pending edits` 的实体证据，也未做任何保存、清 dirty、重导入或内存覆盖；若此前存在 pending 编辑，本次只会如实发现，不会清理。

---

## 3. 保护面（逐项实测）

| 项 | 实测值 | 判定 |
|---|---|---|
| 34 项 workspace 基线（`Development/StudioRuns/R3B-20261022/workspace-baseline.json`，`complete=true`） | 正则逐项重算：`total=34 / same=34 / diff=0 / missing=0` | **34/34 全一致**（含字体与 MCP 插件文件） |
| 单槽存档（`SaveSystem.GetSavePath()` 实测） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`；**`13405B`**、SHA256 **`4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`**、mtime `2026-10-26T03:19:17.3104239Z`、`.tmp` **不存在** | 与要求值一致；未 Load / Save / 恢复 |
| guard 根（`Development/StudioRuns/R3B-SaveProtection-20261024`） | **189** 个 run 目录：`restored=188`、`protective-snapshot=1`、其它状态 **0**、无 manifest **0** | **无 unfinished / unknown / captured / restore-failed**，无需恢复或清锁 |
| guard 最新 run | `20261026T031916Z`：`status=restored`，manifest mtime `2026-10-26T03:19:17.3114764Z` | 上一次运行的存档已真实恢复并回读 |
| 产品 Prefab `Assets/Resources/UI/ManagementView.prefab` | **`282326B` / `e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba`**，mtime `2026-10-26T03:26:15.6250542Z` | 与主开发固定值**完全一致**，本包未改 |
| 测试源码 `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | **`328163B` / `a9ae921dbe12cf3d6cd02d83b63d2816390ea9906b92395862035e3042d64499`**，mtime `2026-10-26T03:26:42.3867672Z` | 与主开发固定值**完全一致**，本包未改 |
| PlayMode 测试 DLL `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | `474624B`、SHA256 `5bbdfdc83b5a35c9c9fcfd82d2e5523804de1aba68412bf14ab9c930ba86792e`、mtime `2026-10-26T03:26:51.1664894Z`（**晚于源码 9s**） | 程序集新鲜，已编译最新源码 |

---

## 4. 新断言与 1920 新用例发现（只读）

| 项 | 实测 | 判定 |
|---|---|---|
| DLL 是否含新断言 | 二进制 UTF-8 文本包含 **`AssertPopulationRowTitlesRendered` = True**（UTF-16 检索 false，属正常：元数据字符串为 UTF-8 堆） | **DLL 确实包含该新断言方法**，非仅源码 |
| 源码新方法 | `private static void AssertPopulationRowTitlesRendered(object management, string what)`，第 **1838** 行 | 存在 |
| 调用点 | 第 **1965** 行，位于 `AssertTabLayout` 的 **Population 分支**，且在既有 `yield return AssertScrollReachable(…populationScroll…)` 之后（注释明示“不替换、不削弱原断言”） | 位置正确 |
| 断言实质（非仅字符串比较） | `characterCount>0`；逐 `characterInfo[i].isVisible` 计数 `>0`；`isTextOverflowing=False`；`renderedWidth/Height>1`；矩形高 ≥ `GetPreferredValues(...).y`；`color.a>0.5`、`canvasRenderer.GetAlpha()>0.5`、`cull=False`；标题墨迹宽 `>10`、高 `>5`、与 `detail` 墨迹不重叠；墨迹在所属行内；完整在视口内的行其墨迹不被裁切；并保留 `CollectionAssert.AreEquivalent(ExpectedPopulationRowTitles(...), …)` 与行数等于投影人数 | 断言链完整，覆盖“姓名真实可见”而非文本绑定 |
| 证据标签 | `LayoutEvidence("ROW-TITLE-RENDER", …)`（逐行 chars/visible/rendered/preferredH/rect/alpha/crAlpha/cull/titleInk/detailInk/overlapsDetail/rowFullyInsideViewport）与 `ROW-TITLE-VISIBLE`（rows/checkedRows） | 运行后可据此核对 |
| 用例发现 | `tests | where fullName contains "ManagementShellPlayTests"` = **12 条**，`mode` 全为 `PlayMode`；`[UnityTest]` 文本计数 **12** | 与 12 例一致 |
| 1920 新用例 FQN | **`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`**，在 12 条中**出现 1 次，唯一** | 新 FQN 唯一，可单方法定位 |

---

## 5. 体边界与实际副作用

- **唯一写入**：本文件 `Development/StudioRuns/R3B-UI-20261026/population-title-resume-preflight.md`（新增）。
- 唯一 Editor 操作：`script-execute` **只读** C#（读文件 / 重算 SHA256 / 反射读 `IsRunActive` 与成员表 / `EditorUtility.IsDirty` 只读判定 / 目录与 manifest 统计）与 `rql-query` 只读查询。
- **未**调用 `AssetDatabase.Refresh`、**未** `tests-run`、**未** `rql-mutate`、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未**启停 Editor。
- **未** Save / SetDirty / SaveAssets / SaveChanges / SaveDirtyScenes / 清 dirty / 重导入字体 / 覆盖内存对象；**未**打开 PrefabStage；**未**保存场景或 Prefab；**未**改尺寸与 Prefs。
- **未**改产品源码 / 测试源码 / 断言 / Prefab / Scene / 字体 / 图集 / 34 项基线 / 存档本体 / guard manifest / 配置 / Authority；**未**安装组件、**未**改权限、**未**创建额外付费子 Agent。

---

## 6. 移交宿主（严格单方法、1920）

前置已满足：`IsRunActive=False`、无可见 pending/activeRequest、场景 `dirty=False`、无 PrefabStage、字体磁盘 = `14961033B / 3c758e16…c404854` 且内存 0 dirty、单槽 `13405-4b2b56`、guard 无 unfinished、Prefab/测试源码/DLL 与固定值一致。

仅运行（单方法、PlayMode、当前 `Screen=1920x1080`）：

`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`

运行后应新增（**保留历史，不覆盖旧图**）：

- `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/layout-input-evidence.log` 中的 `[ROW-TITLE-RENDER]`（逐行 chars/visible/rendered/preferredH/alpha/cull/titleInk/detailInk/overlapsDetail）与 `[ROW-TITLE-VISIBLE]`；
- **新时间戳**的 `management-*-1920x1080-<UTC>.png`（现有 `20261026T031622Z` / `031912Z` 等历史图册不得覆盖）。

约束：模型**不启动** `tests-run`、不忙等、不主动 `Refresh`；若运行失败，保留真实 message / stackTrace 与新增日志行，不以“已修复”代替回执；若运行后字体或单槽出现差异，按事故留证停止，不做清理性保存。

---

## 7. 写后只读复核（本文件落盘后）

| 项 | 写后实测 | 判定 |
|---|---|---|
| Editor | `isPlaying/isPaused/isCompiling/isUpdating=False`、`selection=[]` | 仍空闲 |
| 场景 | `SampleScene dirty=False`、`PrefabStage=null` | 仍干净 |
| 字体磁盘 | `14961033B / 3c758e16…c404854` | 未变 |
| 字体内存 | font / material / 7 atlas `IsDirty=False` | 仍 0 dirty |
| 34 项 / 单槽 / guard | `34/34`；`13405 / 4b2b56f6…`，`.tmp=false`；`189 = 188 restored + 1 protective`，其它 0 | 未变 |
| 原生 TestRunner | `IsRunActive=False`；`testResults` 仍 0 条 | 未新增运行 |

**唯一新增产物**：本文件（`12193B / 78807b9e89988710dc5d867a4aba68d956baed2731cfc89f2463147ee914970d`、mtime `2026-10-26T07:18:32.6978514Z` 为**追加本句之前的版本**；最终版本在 `07:18:37Z` 只读复核之后仅追加了本句，未改其它内容，最终哈希以目录实际文件为准）。1920 单方法的真实回执尚未产生，等待宿主运行；本文件及结论须由主开发专业核验，**不代表制作人批准**。R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。
