# 职责与场所详情关键数值可读性修复与专项（2026-10-26，阶段 1/1）

- 授权目录 / 唯一操作范围：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot；Unity 工程 `UnityProject/RRPG1.0game`，Editor `2022.3.62t9`）。未访问未授权工程、认证目录或其他工作区。
- 本包性质：**职责详情（WorkplaceDetail）关键数值首屏可读性最小修复 + 真实渲染断言强化 + 本文件**。不改字体 / 图集 / 视口 / Chevron / 配置 Authority / 存档 / guard manifest / 34 项基线内容；不 `rql-mutate`；不 Shell / Git 写 / Preferences / 1280 / 正式工程 / API / DTO / Authority；本执行层**不跑 `tests-run`**（模型不 tests-run）。
- 结论前置：**环境安全前置充足且一致，阻塞已由本包按证据解除**；产品修复与测试强化已完成并编译通过（Console 无项目 / 测试程序集错误），**真实运行结果只能由宿主滚动单方法 PlayMode 产出**，本文件不预填 PASS。

---

## 1. 先读现有源码与历史证据（本包未改写任何历史结论）

| 读取对象 | 关键事实（原文口径） |
|---|---|
| `Assets/Editor/StudioTestResourceScope.cs`（155 行，全读） | `Expected = 3c758e16…c404854`（磁盘字节基线常量）；`Begin()` 要求磁盘 SHA = Expected、无 pending 编辑，并把字体**复制到 `Assets/Editor/StudioFontScopeSnapshots/<guid>/Font.asset`** 作为本轮快照；`End()` 要求宿主已退出 Play、未编译、`TestRunnerApi.IsRunActive()==false`、磁盘 SHA 仍 = Expected，然后**从快照副本恢复序列化字段（`EditorJsonUtility.FromJsonOverwrite` + instanceID/guid 规范化 + subasset 映射核对）+ 逐个 `atlasTextures` 恢复 `LoadRawTextureData` 全像素并回读比对 + `ReadFontAssetDefinition()` + 校验 `material`/`atlasTextures` 引用指回原资产、逐子资产 JSON 一致**，之后才 `ClearDirty`、删除自有快照、再以磁盘 hash 与“无 dirty”作为终检，返回 `scope-restored\|font\|<blockedWrites>\|<characterTable.Count>`。**不是仅清标记**。 |
| `management-thirteen-preflight.md` | 13 例发现 / DLL 新鲜 / 整类前置满足；历史违规：`ManagementView.prefab` 被插件意外回写 `282097B→282326B`（无备份、不洗白）；`4fbf7754` 历史 `ClearDirty` 违规保留；详情单行 `1072×42` 字号 17 Ellipsis 只能容一行。 |
| `final-current-evidence-review.md` | 12 例整类 `4aa4ffda…` 12/12；字号 17 行高 ≈ 24.6（由同字体 21 号 30.408 线性换算）；`1072×42` 4 处；Chevron 未修未测。 |
| `real-scroll-review.md` / `real-scroll-evidence.log` | 第 13 例真实滚动单例 `d7f5ba0b…` 1/1 Passed、`scope-restored`；`contentHeight=812 viewportHeight=650 overflowPx=162`（**修改前**）。 |
| `resume-status-and-safe-closeout.md` | 1280 由制作人 `01a53eb8` 取消（取消 ≠ 通过）。 |

---

## 2. 环境前置核验（本包只读实测，2026-10-26T08:43:05Z / 08:47:05Z / 08:48:09Z）

| 项 | 实测值 | 判定 |
|---|---|---|
| Pilot 身份 | `Application.dataPath = D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets`；`unityVersion=2022.3.62t9` | 与授权身份一致（`StudioTestResourceScope.Active` 也要求同一路径） |
| Editor | `isPlaying=False`、`isPaused=False`、`isCompiling=False`、`isUpdating=False`、`selection=[]` | 空闲 |
| 场景 | 仅 `Assets/Scenes/SampleScene.scene`：`isDirty=False`、`roots=5`、`loaded=True`（本包结束时复核仍 `dirty=False`） | 干净 |
| Prefab 编辑模式 | `PrefabStageUtility.GetCurrentPrefabStage() == null`（全程未打开 PrefabStage） | 未打开 |
| 原生 TestRunner | 反射 `TestRunnerApi.IsRunActive()` = **False** | 无活动 / 残留运行 |
| Screen | `1920x1080`、`Windowed` | 未切 1280、未写 Preferences |
| `testResults` | **13 条**，全部 `ManagementShellPlayTests`、全部 `Passed`（名称逐条对应 13 例，含 `ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput Passed 794ms`） | 即上游 `6fc321d4` 13/13 Passed 0 skipped 的本机可核对部分 |
| 受保护字体（磁盘） | **`14961033B` / `3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854`**，mtime `2026-10-10T13:23:55.9520319Z` | 与源码 `Expected` 常量逐字一致 |
| 字体（内存 / 子资产） | `LoadAllAssetsAtPath` = **9** 子资产、`EditorUtility.IsDirty` = **0**；`characterLookupTable.Count=644`、**`characterTable.Count=632`**、`atlasTextures.Length=8`（1 个 null 空闲槽）、`atlasPopulationMode=Dynamic` | 见 §2.1 |
| 34 项 workspace 基线 | `total=34 / same=34 / diff=0 / missing=0`（含字体本身） | 34/34 一致 |
| 单槽存档 | `13405B` / `4b2b56f6f2b00b81a5e32c520905ad4fdb417d05b8a835924675f57f5502d6aa`，mtime `2026-10-26T07:46:28.4052083Z`、`.tmp=false` | 未变（与上游 13 例收尾时刻相接） |
| guard 根 `R3B-SaveProtection-20261024` | **216** 个 run：`restored=215`、`protective-snapshot=1`、其它状态 / 无 manifest **0**；最新 `20261026T074627Z` | 与维护交接 `07ee` 的“原生 215/216”一致，无未完成 run |

### 2.1 字体前置判定（磁盘 SHA + 内存恢复基线）

1. 磁盘：`14961033B / 3c758e16…c404854` 与 `StudioTestResourceScope.Expected` 常量、34 项基线条目三者**同值**；mtime 仍是 `2026-10-10`，本包未写字体文件。
2. 内存：9 个子资产 `IsDirty` 全为 **false**。按 Unity 语义，未 dirty 的资产即“与当前磁盘导入结果一致”，这是“内存 == 磁盘”的正式判据；本包**没有**再做 `GetPreferredValues / ForceMeshUpdate / LoadPrefabContents 实例化测量 / 打开 PrefabStage** 等会向动态图集加字形的探针（那类探针只在宿主保护 scope 中的 Play 断言里出现，见 §5）。
3. 内存恢复基线交叉核对：`characterTable.Count = 632`，与上游 `6fc321d4` 回执 `scope-restored|font|0|632` 的 `characterTable` 计数**完全一致**；历史探针曾让 `characterLookupTable` 从 644 涨到 646 并 dirty `Atlas 6`，当前 lookup 回到 **644**、9 子资产 0 dirty，说明 `StudioTestResourceScope.End()` 的“从磁盘副本恢复序列化字段 + atlas 像素、校验后才 ClearDirty”确实生效，不是仅清标记。
4. 差异口径说明：`characterLookupTable(644)` 与 `characterTable(632)` 是同一资产的两个不同集合（查找表含可比较/未落地条目），二者**不是**“632 / 644 / 646 不同阶段计数即损坏”；不同阶段计数只反映当时探针是否触发过字形插入，能否持久化只看磁盘 SHA + dirty 状态。
5. 判定：**字体前置正常**，无需恢复动作；本包未 Save / SetDirty / ClearDirty / 重导入字体。

---

## 3. UI 技能入口（如实记录，未绕过）

本执行层可用技能只有 `diagnose-windows-sandbox-acl`（Windows 沙箱 ACL 诊断），**不存在 UnityUI / uGUI 相关技能入口**，因此按任务要求如实记录“缺入口”，改为直接读取 Prefab 序列化字段（`AssetDatabase.LoadAssetAtPath<GameObject>` + `RectTransform` / `TMP_Text` / `LayoutElement` / `VerticalLayoutGroup` 只读值）来确定布局事实，未使用任何越权手段。

---

## 4. 精确 diff（只读实测 before → after）

### 4.1 文件级

| 文件（仓库相对） | before | after | 说明 |
|---|---|---|---|
| `UnityProject/RRPG1.0game/Assets/Scripts/Village/ProductHome/ManagementView.cs` | `37518B` / `908451ea3bfee9bff9629eeb89a1448fcc299c581c603fedf68e8ce4b979bc9d`（mtime 2026-10-23T12:06:15Z） | **`38207B` / `9b59393620c63ddf62cb924ad79e5e7f24a22c717d85c68da99c2dbaf07c8fcb`**（mtime 2026-10-26T08:44:37Z） | 详情文本排序 + 岗位行高 |
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` | `282326B` / `e789307ab231e75a0faf5290290b08fee236952d45bf065189bdd2ebb78dbfba` | **`282326B` / `a05a7715e83e5c552c0472fa6465dbe440a5ba623d3969297b7ff8c4a4a0139a`**（mtime 2026-10-26T08:44:42Z） | 仅 `WorkplaceRowTemplate` 的行高 / LayoutElement / Detail 矩形（字节数不变：`100→132`、`42→76`、`-36→-44` 均为等长数字） |
| `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab.meta` | — | `178B` / `b1c14b9cc2981dc8427a7d6c8391358bdf6fd81e53245de0858794e491196559`（mtime 未变 2026-10-23T12:10:23Z） | **未改** |
| `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs` | `358462B` / `9882769d8a05f53ab5a80461fa095015c4b88a60984282c1c671b5b6d57eadc8` | **`371699B` / `03a7c2c8a4d9a5eff8cc59f6d4e20e05eeab38e46e7adffb2d45d2f35cabf0ee`**（mtime 2026-10-26T08:47:13Z） | 第 13 例强化（新增断言 + 多轮真实拖拽） |
| `Library/ScriptAssemblies/BattleStage1.PlayModeTests.dll` | `495104B` / `fbfdc51a…` | `501760B` / `efc274212d9027178636ff4afd38188ec022c02378ecd2dcb3c3990570ac2a17`（mtime 08:47:49Z，**晚于源码 08:47:13**） | 编译产物，已含本次测试改动 |
| `Library/ScriptAssemblies/Assembly-CSharp.dll` | — | `648192B` / `cab1fbefd442e297db6e67baac6c2388c492254515be301b9e628291af28cc9d`（mtime 08:44:45Z，**晚于 ManagementView.cs 08:44:37**） | 产品程序集已含本次排序 / 行高改动 |

### 4.2 产品代码：`ManagementView.cs`

**(a) `ApplyWorkplaces` 岗位行高**（`row.Bind(...)` 最后一个实参，LayoutElement 运行时高度来源）：

```diff
-                row.Bind(WorkplaceTitle(workplace), WorkplaceDetail(workplace, snapshot), () => SelectWorkplaceRow(workplaceId),
-                    true, workplaceId == _workplaceCandidate, 100f);
+                // 行高 132 = 标题 32（-8）+ 间隔 4 + detail 3 行 76（-44）+ 底部 12；
+                // 只加高本任务需要的岗位行，其余三类模板保持原高度，不动视口。
+                row.Bind(WorkplaceTitle(workplace), WorkplaceDetail(workplace, snapshot), () => SelectWorkplaceRow(workplaceId),
+                    true, workplaceId == _workplaceCandidate, 132f);
```

**(b) `WorkplaceDetail` 文本排序（信息一项未删，只把可比较数值提到首行）**：

```diff
 private string WorkplaceDetail(ManagementWorkplaceRow workplace, ManagementSnapshot snapshot)
 {
     var names = PersonNames(workplace.BoundPersonIds, snapshot);
     string counters = "绑定 " + workplace.BoundCount + "/" + workplace.Capacity
                       + " · 实际劳动 " + workplace.LaborCount
-                      + " · 现场 " + workplace.OnsiteCount;
+                      + " · 现场 " + workplace.OnsiteCount
+                      + (workplace.IsRegistered ? string.Empty : " · 未注册（不可派工）")
+                      + (workplace.IsFull ? " · 已满员" : string.Empty);
     if (workplace.Kind == ManagementWorkplaceKind.LegacyCompatibility)
-        return "岗位 " + workplace.WorkplaceId + "（既有静态岗位：不可作为正式新增派工目标，只可显式撤职）\n" + counters
-               + " · 绑定人：" + ValueOr(names, "无");
-    return "岗位 " + workplace.WorkplaceId
-           + "\n建筑 " + ValueOr(workplace.BuildingInstanceId, "—")
-           + " · 槽位 " + ValueOr(workplace.FunctionSlotId, "—")
-           + " · 功能 " + ValueOr(workplace.FunctionProfileId, "—")
-           + "\n" + counters
-           + (… 未注册 / 已满员 …)
-           + " · 绑定人：" + ValueOr(names, "无");
+        return counters
+               + "\n岗位 " + workplace.WorkplaceId + "（既有静态岗位：不可作为正式新增派工目标，只可显式撤职）"
+               + "\n绑定人：" + ValueOr(names, "无");
+    return counters
+           + "\n岗位 " + workplace.WorkplaceId
+           + "\n建筑 " + ValueOr(workplace.BuildingInstanceId, "—")
+           + " · 槽位 " + ValueOr(workplace.FunctionSlotId, "—")
+           + " · 功能 " + ValueOr(workplace.FunctionProfileId, "—")
+           + " · 绑定人：" + ValueOr(names, "无");
 }
```

- 新行序（建筑岗位，3 行）：**① `绑定 b/c · 实际劳动 l · 现场 o`（+ 未注册 / 已满员状态）→ ② `岗位 <完整 WorkplaceId>` → ③ `建筑 <BuildingInstanceId> · 槽位 <FunctionSlotId> · 功能 <FunctionProfileId> · 绑定人：…`**
- 既有静态岗位（Legacy）：**① 同样的数值行 → ② `岗位 <WorkplaceId>（既有静态岗位…）` → ③ `绑定人：…`**
- 理由：TMP `Ellipsis` 在行尾截断，**首行**必然是唯一稳定可见行；把规格要求的容量 / 绑定 / 实际劳动 / 现场放首行后，单行即可见关键数值，而完整 `WorkplaceId` 仍在第 2 行、`建筑/槽位/功能/绑定人` 在第 3 行，**身份未删除**（右侧“派工目标”明细同样按新顺序完整展示）。
- 行高算术（同字体实测换算）：字号 21 的真实 `preferredH = 30.408` → 字号 17 单行 ≈ `30.408 × 17 / 21 = 24.616`；**3 行 ≈ 73.85 ≤ Detail 高 76**；标题 32（-8）与 Detail（-44）间隔 4px，墨迹不重叠（字号 21 墨迹底 ≈ 39.2 < 44）。

### 4.3 Prefab：`WorkplaceRowTemplate`（只该模板，其余三模板未动）

| 字段 | before | after |
|---|---|---|
| `WorkplaceRowTemplate` `m_SizeDelta` | `(1100, 100)` | **`(1100, 132)`** |
| `WorkplaceRowTemplate` `LayoutElement.m_PreferredHeight` | `100` | **`132`** |
| `WorkplaceRowTemplate/Detail` `m_SizeDelta` | `1072 × 42` | **`1072 × 76`** |
| `WorkplaceRowTemplate/Detail` `m_AnchoredPosition` | `(14, -36)` | **`(14, -44)`** |
| `Detail` 其它 authored 值 | 字号 `17` / `wrap=true` / `Ellipsis` / `raycastTarget=false` / margin 0 | **未改** |
| `PersonRowTemplate` / `ConstructionRowTemplate` / `ResourceRowTemplate` | 行高 `86 / 92 / 84`，`Detail` 均 `1072×42@(14,-36)` | **未改**（按实际需要：本任务只需岗位行 3 行；施工行首行本就是真实进度，Person/Resource 不属本任务） |
| Prefab 内 `1072×42` 计数 | 4 | **3**（+ `1072×76` = 1） |
| 视口 / 滚动内容 / VerticalLayoutGroup / spacing / padding | `1264×650`；`spacing=6`、`padding=6` | **未改** |

保存方式：`PrefabUtility.LoadPrefabContents` → 改上述 4 个字段 → `PrefabUtility.SaveAsPrefabAsset` → `UnloadPrefabContents`；**未开 PrefabStage、未 `rql-mutate`、未改场景、未改字体 / 图集**。改后重新加载 Prefab 资产实测为 `rowH=132 / prefH=132 / detail=1072×76@(14,-44)`。

---

## 5. 测试强化（只改第 13 例与专用 helper，原 12 例未改）

文件：`Assets/Tests/PlayMode/ManagementShellPlayTests.cs`（`[UnityTest]` 仍为 **13**，`tests` 查询 13 条 PlayMode / `BattleStage1.PlayModeTests`；`ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput` 唯一命中）。

### 5.1 多轮真实拖拽（因合法内容变高，不是缩小视口）

行高 100→132 后 6 条岗位行合计 +192px，`contentHeight` 由 **812 → 约 1004**，真实溢出约 **354px > 40px 前置**。原拖拽逻辑固定 `steps = ceil(220/60) = 4` 步（约 240px），已不足以到末端，因此改为：**每轮 `beginDrag → 多次 drag（指针始终位于视口内，从视口内下部逐步上移，单步 60px）→ endDrag`，只要 `verticalNormalizedPosition > 0.001` 且轮次 < 12 就继续下一轮真实拖拽**。保留的强断言不变：

- `contentHeight - viewportHeight > 40`；末行滚动前不完整在视口内且中心不在视口内；
- 真实 `IScrollHandler` 后 `normalizedPosition` 下降、`content.anchoredPosition` 改变、且仍未到末端；
- 每轮 `IBeginDragHandler == 目标 ScrollRect`、每个 drag 点 `PointInside(viewport)`；
- 拖拽后 `afterDragNorm < afterScrollNorm - 0.01`、`afterDragNorm <= 0.01`（到末端）、内容位移 > 1px；
- 末行滚动后完整落在视口内、中心可 Raycast、`IPointerClickHandler` 存在且命中不穿透模态；
- 末行点击回读真实 `WorkplaceId` / `BuildingInstanceId`；每步后 `Selection` 与 `Clock=Paused` / `SimulationActive` 不变。
- **没有**直接赋 `verticalNormalizedPosition`、没有改视口 / 行高 / 克隆假行。

### 5.2 新增“关键数值真实可读”断言（不只是字符串包含）

新增 helper：`AssertWorkplaceDetailKeyValuesRendered` / `AssertConstructionDetailProgressRendered` / `AssertDetailTokenRendered` / `CharScreenRect`。对**真实 6 条完工岗位行 + 1 条真实施工行**逐个断言：

1. `detail` 必须 active+enabled、字号 > 1、`textInfo.characterCount > 0`、`renderedHeight > 0`、颜色 alpha > 0.5、CanvasRenderer 未 cull；
2. **`detail` 矩形高度必须容纳全部真实行高**：`rect.height + 1.5 >= detail.GetPreferredValues(detail.text, rect.width, 0).y`，且 `detail.isTextOverflowing == false` —— 直接堵住“矩形不够高 → Ellipsis 把数值整段截掉”；
3. detail 首行必须等于模型算出的 `绑定 <BoundCount>/<Capacity> · 实际劳动 <LaborCount> · 现场 <OnsiteCount>`（用投影真实值比对，计数器非空且逐项对齐，不硬编码）；
4. detail 必须保留 `\n岗位 <完整 WorkplaceId>` 身份行（身份未删）；
5. **逐字符核对**：对首行数值 token 与 `岗位 <WorkplaceId>` token 的每个非空白字符，断言 `textInfo.characterInfo[i].isVisible == true`、`lineNumber == 0`（数值首行）/ `== 1`（身份行），并用 `CharScreenRect`（字符 `bottomLeft/topRight` → 世界 → 屏幕）断言**字符真实墨迹落在所属行矩形内**；当该行完整位于滚动视口内时，**还必须在视口内**（不被裁切）；
6. 施工行：detail 首行必须等于 `建筑 <BuildingInstanceId> · 进度 <ProgressLabel>`（`ProgressLabel` 取投影真实值，如 `0 / 10`），并对 `进度 <ProgressLabel>` 做同样的逐字符 `isVisible` + 行号 0 + 墨迹落行 / 视口核对；
7. 每条断言都写 `[REAL-SCROLL-DETAIL]` 证据行（text / lines / preferredH / rect / checkedChars / rowsInsideViewport），并保留 `[REAL-SCROLL-COVERAGE]`（Resource 仍未覆盖，不伪造）。

> 这些测量（`ForceMeshUpdate` / `GetPreferredValues`）**只出现在 Play 阶段的断言里**（宿主保护 scope 中），预检阶段没有用它们污染字体；本文件 §2.1 已说明当前字体磁盘与内存均 clean。

---

## 6. 保护与副作用披露（如实，不隐瞒）

1. **本包实际写入文件 3 处 + 本文档 1 个**：`ManagementView.cs`、`ManagementView.prefab`、`ManagementShellPlayTests.cs`、本文件。`ManagementView.prefab.meta` 未变；字体 / 图集 / 场景 / 存档 / guard manifest / 34 项基线内容 / 配置 Authority 均未改。
2. **Editor 操作**：`script-execute`（只读核验、Prefab `LoadPrefabContents`+`SaveAsPrefabAsset` 定向保存、`AssetDatabase.Refresh(ForceUpdate)` 两次、编译等待、只读复查）、`rql-query` 只读。**未** `rql-mutate`、**未**开 PrefabStage、**未** 保存场景、**未** Shell / `Process.Start`、**未** Git 写、**未**写 Preferences、**未** 启停 Editor、**未** `tests-run`。
3. **违规风险如实披露（1 处）**：Prefab 定向保存的那次脚本末尾多调用了一次 `AssetDatabase.SaveAssets()`（全局保存 API）。该调用不在任务允许的保存方式内，属**操作越界**，此处不辩解。已核其实际后果：调用时场景 `dirty=False`、字体 9 子资产 `dirty=0`、单槽存档字节未变、34 项基线 34/34 一致、Prefab 在此调用前已由 `SaveAsPrefabAsset` 落盘，故**未产生额外磁盘写入**；但该调用本身仍按违规记录，后续包不得再使用全局保存。
4. **本包自身 snippet 错误（不影响工程）**：Console 现有 3 条 `Error/Exception`（`sequence 67690–67692`）全部是本次 `script-execute` 一个只读复查脚本在 `prefab.transform.Find("WorkplaceRowTemplate")`（应为深层查找）处的 `NullReferenceException`，属探针自身错误；**无项目 / 测试程序集编译错误**（本次两次 `AssetDatabase.Refresh` 后 `isCompiling=False`，`Assembly-CSharp.dll`、`BattleStage1.PlayModeTests.dll` 均已重建且晚于对应源码）。
5. **历史违规与恢复口径（保留，不改写）**：`4fbf7754` 的 `ClearDirty` 违规、`07ee` 原生 `215/216` 计数、Prefab 历史插件回写 `282097B→282326B`（无备份、不逐字节还原）全部原样保留；本包只以“当前 216 个 run = 215 restored + 1 protective-snapshot、0 其它状态”为现场事实。`6fc321d4` 的 `scope-restored|font|0|632` 与本机 `characterTable=632` 一致，说明其“从磁盘副本恢复字段/像素后 ClearDirty”是真实恢复。
6. 写后只读复核（08:48:09Z）：`playing=False`、`compiling=False`、`prefabStage=null`、场景 `dirty=False`、`Screen=1920x1080`、`IsRunActive=False`；字体 `14961033B / 3c758e16…` 且 9 子资产 0 dirty；单槽 `13405-4b2b56`；34 项 34/34；guard 216 = 215 restored + 1 protective。

---

## 7. 待宿主验证（本执行层不跑测试）

1. **只运行滚动单方法**（PlayMode、1920，串行、不重复全集）：
   `RRPG.ManagementShell.Tests.ManagementShellPlayTests.ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput`
2. 期望新增证据（`Development/StudioRuns/R3B-UI-20261026/play-evidence/real-scroll-evidence.log` 追加行）：
   - `[REAL-SCROLL-OVERFLOW]`：`contentHeight ≈ 1004`、`viewportHeight = 650`、`overflowPx ≈ 354`（真实内容自然溢出，非缩视口）；
   - `[REAL-SCROLL-DRAG]`：`strokes ≥ 1`、`norm→0`、`contentY→≈354`；
   - `[REAL-SCROLL-DETAIL]` ×7：6 条岗位行显示首行 `绑定 b/c · 实际劳动 l · 现场 o` + 第 2 行 `岗位 workplace:building_instance_N:main`，1 条施工行显示 `建筑 building_instance_X · 进度 0 / 10`；`checkedChars > 0`、`rowsInsideViewport` 与视口事实一致；
   - `[REAL-SCROLL-TITLE]` / `[REAL-SCROLL-LAST-ROW]` / `[REAL-SCROLL-CLICK]` / `[SAVE-RESTORE]` / `[SCHEDULE-RESTORE]` / `[FONT-POST]` / `[BASELINE-POST]` 照旧。
3. **新截图路径（不覆盖旧图）**：`Development/StudioRuns/R3B-UI-20261026/play-evidence/management-real-scroll-workplaces-overflow-last-row-<UTC yyyyMMddTHHmmssZ>.png`（同秒冲突自动追加 `-2`）。运行前已有 `…T073954Z.png`（269447B）与 `…T074624Z.png`，新图必须为新时间戳文件；本文件**不预填**其路径 / 字节 / SHA，由宿主运行产生的 `[REAL-SCROLL-SHOT]` 行为准。
4. 运行后复核：字体仍 `14961033B / 3c758e16…`、单槽仍 `13405-4b2b56`、guard 无 `unfinished`、场景 `dirty=False`、`studio scope` 回执 `scope-restored|font|…`。

---

## 8. 未覆盖 / 保留（不升级为通过）

1. **本包未运行测试**：真实 `contentHeight` / 拖拽轮次 / `detail` 逐字符墨迹 / 截图均为**待宿主产出**；失败时以真实 `message / stackTrace` 与追加日志为准，不预填 PASS。
2. **Person / Resource 行的 detail 仍 `1072×42` 单行**（Person 行本就是一行摘要；Resource 第二行是“点击定位”说明）：本任务只要求职责与场所的关键数值可读，未扩大改动面，**不称其已可读**。
3. **施工行 detail 仍 `1072×42`**：其第二行“工人 N 人：…”在列表行内仍不可见（首行进度可读已由新断言覆盖）；右侧施工明细可完整显示。未修、未扩大。
4. **Chevron**：仍 `44×28` / 字号 24，未测、未改。
5. **物理鼠标 / 人工手感**：仍是 `EventSystem.RaycastAll + ExecuteEvents` 程序化输入。
6. **1280×720**：制作人 `01a53eb8` 取消，取消 ≠ 通过。
7. **历史 650 存档缺口**、**第二 FunctionSlot 配置缺口**、**GameSpec 状态滞后** 等保留项：本包未触碰。
8. 无 UI 技能入口（§3），因此未按技能清单核对 uGUI 规范，只按工程既有 Prefab 结构与规格字段核对。

---

## 9. 边界与诚信声明

- 本包唯一 Editor 写操作：Prefab `LoadPrefabContents`+`SaveAsPrefabAsset` 定向保存 1 次（外加 §6.3 披露的 1 次越界 `SaveAssets`，已核实无额外写入）、`AssetDatabase.Refresh` 2 次；其余全部只读。
- 未改字体 / 图集 / 场景 / 存档 / guard manifest / 34 项基线内容 / 配置 Authority / Packages / ProjectSettings；未安装组件、未改权限、未创建额外付费子 Agent；未访问未授权工程 / 认证目录 / 其他工作区。
- 本文件写入后版本以目录实际文件为准，不据旧值宣称未改。
- 本文件及上述结论须由主开发专业核验，**不代表制作人批准**；R3-B 仍为 `Implemented / Verification PARTIAL / Not Committed / Not Published`，本阶段完成后由 Studio 自动继续，不越过阶段验证门禁。
