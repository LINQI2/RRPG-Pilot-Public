# R3-B 管理人口行「姓名/标题不可见」视觉缺陷定位与最小修复（2026-10-26，阶段 1/1）

- 授权目录：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。
- 本包只处理**人口行姓名/标题真实不可见**：先看真实截图 / Prefab / TMP / CanvasRenderer / Mask / 重叠，再最小修正 `ManagementView.prefab` 4 个行模板的 Label 显示布局，并只强化布局第 12 例的人口行标题可见性断言。
- 未重建 Prefab / Scene；未改产品规则、字体 / 图集、尺寸 / Prefs、行高、Detail、Chevron、数据绑定；未跑任何测试、未 `rql-mutate`、未 Shell / Git、未全局 `SaveAssets/SaveChanges`。

---

## 1. 直接根因（由真实渲染数据确认，不预设）

**TMP 的 Ellipsis 溢出模式在“矩形高度小于真实单行排版高度”时，会把整行截成 0 个可见字符。**

| 实测项 | 值 |
|---|---|
| 受保护字体 `RRPG_SourceHanSansCN_Regular_TMP` 的 `faceInfo.lineHeight / pointSize` | `130.32 / 90` |
| 字号 21 的单行真实排版高度 | **30.41 px**（`GetPreferredValues("林恩 · profession_test_c") = (247.23, 30.41)`） |
| 4 个行模板 Label RectTransform | `1072 × 28`，`anchoredPosition=(14,-8)`，字号 21，`overflowMode=Ellipsis` |
| 28 < 30.41 | TMP 判定 `isTextOverflowing=True` → **`textInfo.characterCount=0`、rendered 为负、无可见字符** |

阈值实测（同一保存后 Prefab、真实 Overlay Canvas、字号 21）：

| Label 高度 | characterCount | rendered | isTextOverflowing |
|---|---|---|---|
| 24 / 26 / 27 / 28 / 29 / 30 | **0** | 负值（未渲染） | True |
| 31 / 32 / 34 / 36 / 40 | **22** | `247.22 × 30.41` | False |

字号对照（矩形高 28）：17/18/19 号可渲染（19 号 preferred 高 27.52），**20 号（28.97）与 21 号（30.41）均被截为 0 字符**。

这解释了截图现象，而不是颜色 / alpha / Canvas cull / 遮挡：

- 截图像素扫描：行 1 绿色区域从 `fromTop≈364` 开始，Detail 墨迹在 `418–433`；标题应处的 `372–400` 区间**没有任何亮色字形像素**（不是暗色字，也不是被覆盖）。
- 同一模板的 Label 与 Chevron 都在顶部 `y=-8`、高 28；Detail 在 `y=-36`、高 42、字号 17，因此 Detail 仍可见，标题整行消失。
- 运行时只读测量：Label `color.a=1`、`CanvasRenderer.GetAlpha()=1`、`cull=False`、active/enabled、无 CanvasGroup 降 alpha；唯一 `RectMask2D` 是 `Viewport`，Label 墨迹完整在其内；标题中心覆盖层只有祖先背景与自身 TMP，没有后置遮挡控件。

结论：**姓名/标题并非“未绑定/空字符串/颜色透明/被裁切/被覆盖”，而是 Label 矩形高度 28 不足字体 21 的 30.41 单行高度，被 TMP Ellipsis 整行截断为 0 字符。** 既有绿测只比较 `TMP.text` 字符串与 Raycast，所以未发现。

---

## 2. 最小修改（精确范围）

### 2.1 Prefab

文件：`UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab`

仅将 4 个行模板的 `Label` RectTransform `sizeDelta.y` 从 **28 → 32**（x 仍 1072，位置仍 `(14,-8)`，字号 21 不变）：

| 模板 | Label 修改前 | 修改后 | Detail（未改） | Row 高度（未改） |
|---|---|---|---|---|
| PersonRowTemplate | `1072×28 @ (14,-8)` | `1072×32 @ (14,-8)` | `1072×42 @ (14,-36)` | 86 |
| WorkplaceRowTemplate | 同上 | 同上 | 同上 | 100 |
| ConstructionRowTemplate | 同上 | 同上 | 同上 | 92 |
| ResourceRowTemplate | 同上 | 同上 | 同上 | 84 |

保存方式：`PrefabUtility.LoadPrefabContents` → 修改 → `PrefabUtility.SaveAsPrefabAsset` → `UnloadPrefabContents`；**未使用** `rql-mutate` / 全局保存 / Scene 保存。

修复后同一 Prefab 的真实 Canvas 复测（代表性标题）：

- `textInfo.characterCount=22`、可见字符 20（2 个空格不计）、`rendered=247.22×30.41`、`isTextOverflowing=False`；
- `alpha=1`、`CR alpha=1`、`cull=False`；
- 标题墨迹 `(80,672.8,247.22,30.41)` 完整落在行 `(66,626,1252,86)` 与视口 `(60,122,1264,596)` 内；
- 标题墨迹与 Detail 墨迹 `(80,642.69,211.61,24.62)` **不重叠**（`Overlaps=False`）。

### 2.2 测试（只强化第 12 例，不削弱原断言）

文件：`UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`

新增（仅人口页调用）：

- `TextScreenRect(TMP_Text)`：把 `textBounds` 转成真实 Screen 墨迹矩形；
- `AssertPopulationRowTitlesRendered(...)`：逐活动人口行断言
  - 标题 active + enabled、文本非空、字号 > 1；
  - **TMP 真实可见字符数 > 0**（`characterCount>0` 且 `characterInfo[i].isVisible` 计数 > 0）；
  - `isTextOverflowing=False`、`renderedWidth/Height>1`；
  - 矩形高度 ≥ 真实字体排版高度（`GetPreferredValues(...).y`）且 ≥ renderedHeight；
  - `color.a>0.5`、`CanvasRenderer.GetAlpha()>0.5`、`cull=False`；
  - 标题墨迹宽 > 10px、高 > 5px，且**与 Detail 墨迹不重叠**；
  - 标题墨迹在所属行内；完整位于视口内的行，其标题墨迹还必须完整在滚动视口内（视口裁切）；
  - 保留原业务标题对照：`CollectionAssert.AreEquivalent(ExpectedPopulationRowTitles(...), PopulationRowTitle(...))` + 行数等于投影人数；
- 在 `AssertTabLayout` 的 Population 分支、原 `AssertScrollReachable` 之后调用；**原 11 例方法体、原滚动/可达/Raycast/截图断言未改**。
- 新增证据标签：`[ROW-TITLE-RENDER]`（逐行字符/渲染/墨迹/alpha/cull）与 `[ROW-TITLE-VISIBLE]`（总行数/核对行数）。

---

## 3. 写前门禁（全部只读实测）

| 项 | 实测 | 判定 |
|---|---|---|
| Editor | `isPlaying=False / isPaused=False / isCompiling=False / isUpdating=False`，`selection=[]` | 空闲 |
| 场景 | 仅 `SampleScene`，`dirty=False`、`roots=5`；`PrefabStage=null` | 干净 |
| Screen | `1920×1080 Windowed` | 符合宿主 1920 |
| 34 项 workspace 基线 | `34/34 same，diff=0，complete=true` | 一致 |
| 受保护 TMP 字体 | `14961033B / 3c758e16…c404854` | 一致 |
| 单槽存档（`SaveSystem.GetSavePath()`） | `13405B / 4b2b56f6…d6aa`，`tmp=false` | 一致；未 Load/Save/恢复 |
| guard | **189** run：`restored=188`、`protective-snapshot=1`、其它 **0** | 无未完成/未知 |
| 技能入口 | 本会话可用 skill 仅 `diagnose-windows-sandbox-acl`；**无 `unity:ui` / `unity:ui-ugui` 可读入口** | 如实记录，未绕过权限 |

---

## 4. 前后哈希与编译证据

| 对象 | 修改前 | 修改后 |
|---|---|---|
| `ManagementView.prefab` | `282326B / 5cf61738c30a1ec6a86973d0b6c2d02a7db3ded1b7ffc09afd900450be1fa8ac`，mtime `2026-10-26T03:23:41.4471058Z` | `282326B / e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba`，mtime `2026-10-26T03:26:15.6250542Z` |
| `ManagementShellPlayTests.cs` | `318727B / 7a1738b4861044c3eec27b88d3496f7de8fc15029b04ef3f42a7dcae60cd4337` | `328163B / a9ae921dbe12cf3d6cd02d83b63d2816390ea9906b92395862035e3042d64499` |
| `BattleStage1.PlayModeTests.dll` | 历史 `468992B`（03:15:07Z） | **`474624B`，mtime `2026-10-26T03:26:51.1664894Z`**（晚于源码，已编译） |
| 字体 / 34 项 / 单槽 / guard | — | 写后复核仍 `34/34`、字体 `3c758e16…`、单槽 `13405-4b2b56`、guard `189`（188 restored + 1 protective，其它 0） |

**必须披露的 Prefab 哈希异常（非本包有意修改）**：进入只读 PrefabStage 检查后，Prefab 文件 mtime 变为 `03:23:41Z`、大小由历史 `282097B / ade4c7e1…` 变为 `282326B / 5cf61738…`；与本会话内核对时间的相关性表明是**打开 PrefabStage 时编辑器/插件自动回写序列化**（本会话未执行 `SaveAssets`/`SaveChanges`/`rql-mutate`）。该文件无备份，无法逐字节还原；随后本包唯一有意修改是 4 个 Label 高度 28→32。不能宣称“未触碰 Prefab 序列化形态”，特此留证，供主开发核验该自动回写是否可接受。

---

## 5. 证据路径与未测项

**已看截图（原样保留，未覆盖）**

- `Development/StudioRuns/R3B-UI-20261025/layout-input-evidence/management-population-1920x1080-20261026T031622Z.png`（缺陷现场；`sha256=ad34789131c18152857283c67fdf88f5bfe470bc4c9ffd47b6139d09aa54209e`）
- `Development/StudioRuns/R3B-UI-20261026/population-row-title-crop.png`（本次生成的 2× 放大裁切，仅作定位证据）

**编译 / 发现（只读）**

- Console 现有 `Error/Exception` 6 条，逐条均为本会话 MCP `Script/Execute` snippet 自身编译/运行错误（`CS1061`、`NullReferenceException`），**无项目 / 测试程序集编译错误**。
- `tests | where fullName contains "ManagementShellPlayTests"` 仍命中 **12** 条，方法名逐一保留；第 12 例仍是 `ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`。

**未测 / 未覆盖（不宣称通过）**

1. **未运行任何测试**（无 `tests-run`、无 TestRunner、无反射启动）。修复后的真实 UI 截图与第 12 例结果由宿主在 **1920 单方法**运行后产出；旧 PNG 不覆盖，新图应使用新时间戳。
2. 原 11 例未重跑；滚动溢出分支仍未触发（当前 Population `overflow=False`）。
3. Workplaces / Resources / Construction 三个模板虽同步修正 Label 高度，但本世界该两页为空态，**没有真实行业务行截图证据**。
4. `Chevron` 仍为 `44×28`、字号 24（同样小于其真实行高），**本包未修改、未纳入断言、未宣称可见**。
5. Detail 仍为 `1072×42`、字号 17，只能显示一行 + Ellipsis（阈值实测 52 才容纳两行）；这是既有表现，**本包未改**。
6. 未验证物理鼠标点击手感；未改动字体 / 图集 / 尺寸 / Prefs / 1280 分支。

---

## 6. 移交宿主（严格单方法）

运行前确认真实 `Screen=1920×1080`，仅运行：

`RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable`

运行后应新增：

- `layout-input-evidence.log` 的 `[ROW-TITLE-RENDER]`（每行 `chars/visible/rendered/preferredH/alpha/cull/titleInk/detailInk/overlapsDetail`）与 `[ROW-TITLE-VISIBLE]`；
- 新时间戳的 `management-population-1920x1080-<UTC>.png`（不覆盖旧图）。

若失败，保留真实 message/stackTrace 与新增日志行，不以“已修复”代替回执。本文件及结论须由主开发专业核验，不代表制作人批准；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`。
