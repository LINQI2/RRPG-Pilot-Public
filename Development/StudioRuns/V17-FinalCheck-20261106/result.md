# V17 Final Check：只读核验结果

日期 2026-11-06。仓库 `D:/AI_Agent_Lab/rrpg_pilot/repo`，分支 `ai/rrpg-r3b-population-pilot`，
HEAD `4d9ea21ae1799f57a741cae5df58a7685996cc1e`，暂存区空，`git status` 189 条。

**结论：本轮只完成了静态可核验部分；Unity 侧三项（Editor 身份/状态、AssetDatabase GUID 绑定、编译错误状态）因 Editor 未运行而无法执行，已如实登记为能力缺口。未提交、未推送、未改任何资产。**

## 1. Pilot 身份与 Editor 状态

| 检查 | 实测 |
|---|---|
| Unity/Tuanjie 进程 | **none** |
| `localhost:21867` 监听 | **0 条** |
| `Temp/TuanjieLockfile` / `Temp/UnityLockfile` | 均不存在 |
| MCP HTTP 探测 | `WebException`（无法连接） |

因此：**门禁 begin 无法执行**（脚本本身依赖 Editor 侧会话上下文），本轮**没有进入任何受保护操作**——
未进 Play、未 Builder、未 Refresh、未保存资产、未恢复字体。字体磁盘哈希 `7b5717b9…`
与 `font-protection/baseline.json` 基线一致（status match=true，exit 0），属开工前后未变的独立证据。

## 2. ManagementView.prefab 依赖

详见 `prefab-dependencies.json`。要点：

- Prefab 内共 **12 条** 32 位十六进制 GUID 引用：11 条 `m_Script` + 1 条 `m_fontAsset`。
- **9 条可在 PackageCache 直接解析**：TextMeshProUGUI、TMP_InputField、Button、Image、Mask、ScrollRect、
  ContentSizeFitter、LayoutElement、VerticalLayoutGroup（TMP 3.0.9 / uGUI 1.0.0）。
- **3 条未解析**：`4a7dfebd…`、`f37df54f…`（疑似 ManagementView / ManagementRowView）、
  `f9418c72…`（`m_fontAsset` 指向的字体）。
- **为什么不能靠磁盘 Meta 原文匹配**：项目 `Assets` 下 1118 个 `.meta` 中，
  仅 **46 个**是 32 位十六进制 guid，**1072 个（95.9%）是 56 字符编码 guid**；
  而 Prefab 内一律写十六进制。两者格式不同，**不做解码猜测**，因此项目内 GUID→资产映射只能由
  `AssetDatabase.GUIDToAssetPath` 提供——本轮无此能力。

未做：Instantiate、加载进场景、字体测量、任何序列化/YAML 写入；未断言 Missing Script。

## 3. 编译状态与候选项

- **编译错误状态无法读取**（Editor 未运行）；本轮**不声称**提交快照可编译，也不以"读得到已编译程序集"推断通过。
- 候选清单见 `precise-candidates.json`（39 条，含 path / 状态 / SHA256 前 16 位 / 归属 / 原因）：

| 组 | 数量 | 说明 |
|---|---:|---|
| iw-compiler（Tools） | 7 | CompilerRunner、DiagnosticCodes、EntityRequirements、PrimaryErrorSummary、Program、InitialWorldValidator、WorkbookSeed（后两者未跟踪） |
| iw-runtime | 6 | StaticConfigBuilder、PlayerCollection、SaveSystem、RRPGStaticConfigRoot、InitialWorldMirrorReader、InitialWorldConfigDatabase、InitialWorldConfiguration |
| iw-config | 4 | RRPG_InitialWorld.xlsx + 三份 Initial Mirror JSON（均未跟踪） |
| mgmt | 8 | ManagementView/RowView/ReadAdapter/CommandGateway、ManagementSnapshot、ManagementView.prefab(+meta) |
| mgmt-shared | 5 | BuildingRegistry、StaticConfigRegistry、HomeBuildingPlacement、HomeWorldCamera、VillagePresentation.xlsx |
| noise（不纳入） | 2 | 两份 README |

**混入他任务的文件**：`SaveSystem.cs`、`BuildingRegistry.cs`、`StaticConfigRegistry.cs`、`HomeBuildingPlacement.cs`、
`HomeWorldCamera.cs` 同时承载 Initial World 与 Management 两侧改动，且部分已被 `f2944858` 提交；
不能按文件名认定整体归属，需逐 hunk 评审（本轮未做，属缺口）。

## 4. 测试基线依赖（影响提交快照重跑）

- `UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ManagementShellPlayTests.cs`
  - `BaselineRelative = "Development/StudioRuns/R3B-20261022/workspace-baseline.json"`（L71）
  - `OneTimeSetUp` 读取该文件并逐项比对 34 项 SHA（L63、L1389）
  - 该 JSON **磁盘存在但未被 git 跟踪**（`tracked=False`）
- 结论：**把产品代码提交、但不提交该基线，新 clone 上这套测试会在 OneTimeSetUp 失败**。
  这是"测试夹具依赖"，不是产品依赖；按工作包要求**未修改测试**，仅登记。
- 产品侧另需：Prefab + 脚本 + Meta + 字体引用；配置侧需 XLSX + 三 Mirror + Builder/Registry/DB。

## 5. 未执行项（明确）

1. Unity 身份/Editor 状态/Console/编译错误回读（Editor 未运行）。
2. AssetDatabase/MonoScript/PrefabAsset 的 GUID 绑定与 Missing Script 判定。
3. 门禁 begin/end（无 Unity 操作，故未进入）。
4. Compiler 定向验证、EditMode/PlayMode 测试、Builder、截图（工作包明确由主开发隔离执行）。
5. G6/G7、FP 功能（明确排除）。

## 6. 对提交边界的直接建议

- `Tools` 组是 Initial World 配置链的**完整依赖**（Validator/Seed/CLI/字段要求），若排除则配置链不可重建；
  是否纳入需制作人单批，本轮只给路径与 SHA。
- 即便纳入 Tools，`ManagementView` 组仍依赖未跟踪的 Prefab/Meta 与四个 Management 脚本；
  `HomeProductShell`（已提交）在运行时 `Resources.Load<ManagementView>("UI/ManagementView")`，
  缺失即管理界面不可用 → **这些文件必须作为同一闭包提交，否则 HEAD 仍不可重建**。
- `workspace-baseline.json` 属夹具依赖，可与产品闭包分开决定；但分开提交前应知晓"测试不可重跑"这一后果。

## 7. 证据文件

- `prefab-dependencies.json`：12 条 GUID、解析结果、Meta 编码统计、未证明项。
- `precise-candidates.json`：39 条候选（路径/状态/SHA/归属/原因）、能力缺口。
- 本文件。
