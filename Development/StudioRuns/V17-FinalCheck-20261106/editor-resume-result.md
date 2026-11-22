# V17 Editor 续核验结果（editor-resume）

日期 2026-11-06。仓库 `D:/AI_Agent_Lab/rrpg_pilot/repo`，分支 `ai/rrpg-r3b-population-pilot`，
HEAD `4d9ea21ae1799f57a741cae5df58a7685996cc1e`，暂存区空。
本轮只读取证：**未 Play、未 Builder 写入、未 Refresh、未 SaveAssets、未实例化 Prefab、未改任何资产/测试/场景/存档/字体，未 Commit/Push。**

## 1. 门禁与 Pilot 身份（真实退出码）

| 项 | 值 |
|---|---|
| Label | `v17-resume-01` |
| begin | **exit 0**（`records/20261106T061055Z-v17-resume-01-begin.json`） |
| end | **exit 0**（`records/20261106T061128Z-v17-resume-01-end.json`） |
| 字体 SHA256 | `7b5717b976bac0d90dce70ba65b4dda0995bf78e8b3fd317ea9918058109c016`（前后一致） |

Editor 现场（`script-execute` 实取）：

```
project  = D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game
version  = 2022.3.62t9   product = RRPG_AI_Pilot
playing=False  compiling=False  updating=False
scene=Assets/Scenes/SampleScene.scene  dirty=False
scriptCompilationFailed = False
```

未关闭制作人场景，未退出任何 Play（实际非 Play）。

## 2. ManagementView.prefab 的 12 条 GUID（全部解析）

详见 `editor-prefab-dependencies.json`。**12/12 解析成功，缺失 0**：

| GUID 前 8 位 | 解析结果 | 类型 |
|---|---|---|
| `4a7dfebd` | `Assets/Scripts/Village/ProductHome/ManagementView.cs` | **ManagementView**（项目内脚本，SHA `6c218dafbea1`） |
| `f37df54f` | `Assets/Scripts/Village/ProductHome/ManagementRowView.cs` | **ManagementRowView**（SHA `d5a2456ab131`） |
| `f9418c72` | `Assets/Art/Fonts/…/RRPG_SourceHanSansCN_Regular_TMP.asset` | 受保护 TMP 字体（SHA `7b5717b9…`） |
| 其余 9 条 | TextMeshProUGUI、TMP_InputField、Button、Image、Mask、ScrollRect、ContentSizeFitter、LayoutElement、VerticalLayoutGroup | TMP 3.0.9 / uGUI 1.0.0 |

Prefab 本体：GUID `4a54a9f0d1e97634a93836f4566fd5ce`，SHA `f750ed83ee1c`；
3 个根子物体 `ModalBlocker / ModalPanel / Templates`；81 个 Transform；**missingScript=0**；
17 个按钮，其中 **4 个行模板**：`PersonRowTemplate`、`WorkplaceRowTemplate`、`ConstructionRowTemplate`、`ResourceRowTemplate`。

**上一轮静态结论已被推翻**：当时记「3 条未解析 + 1072/1118 编码 meta 无法匹配」，本轮 AssetDatabase 实测全部可解析；
编码 meta 只是磁盘表示，`GUIDToAssetPath` 才是权威。

## 3. 编译状态与配置链可读性

- `scriptCompilationFailed = False`，`isCompiling=False`；**未重新编译**（无 RequestScriptCompilation）。
  说明：这只代表当前 Editor 的编译状态，不等同"隔离提交快照可编译"。
- `InitialWorldConfigDatabase` / `VillagePresentation` / `VisualAssetCatalog` 三者 `TryGet*` **均为 True**。
- 三份 Mirror 当前 SHA：`InitialWorld.json=5af02464bbf3`、`InitialPopulation.json=ae0fe5456266`、`InitialBuilding.json=32abb64be5c5`。
- `StaticConfigBuilder` 仅查询签名、未调用：`BuildFromMenu() / Build(string) / RebuildFromZero() / ExecuteBuild() / ExecuteRebuildFromZero() / RepoRoot() / Absolute(string)`。

## 4. 候选逐 hunk 评审（详见 `editor-candidate-review.json`）

现场 39 条候选（口径与历史 61/48 不同，已在文件内标注）：

- **明确 V17 产品闭包**：PlayerCollection、StaticConfigRegistry、RRPGStaticConfigRoot、BuildingRegistry、ProductHomeFoundationTests、ProductHomeFoundationPlayTests，以及未跟踪的 Management 四脚本 + Prefab + Meta + ManagementSnapshot + Initial World Runtime/Reader/Config/三 Mirror。
- **真实混合（需人工逐 hunk 拆分）**：
  - `StaticConfigBuilder.cs` +75/−11，18 hunks，同文件含 Initial World 与怪物/表现生成段；
  - `SaveSystem.cs` +49/−2，5 hunks，两侧共用；
  - `HomeBuildingPlacement.cs` +6/−3、`HomeWorldCamera.cs` +54/−6，无 IW 关键词 → 归属待确认。
- **不纳入**：两份 README。

**测试夹具**：`ManagementShellPlayTests.cs` 依赖未跟踪的 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`
（`tracked=False`）。按工作包：保留磁盘证据、**不纳入长期回归**、不改测试、不删基线。
产品 Runtime 不依赖该测试类型（外部引用 3 处均在测试/说明内，建议主开发复核该计数口径）。

## 5. 是否还有真实缺口（回答工作包第 5 点）

**有，且不止一个**：

1. `HomeBuildingPlacement.cs`、`HomeWorldCamera.cs` 的改动归属未确认——不确认就无法判定闭包是否完整或是否夹带他包。
2. `StaticConfigBuilder.cs` 单文件跨多个生成段，整文件提交会带他任务改动；逐 hunk 拆分的风险需你决定是否接受。
3. Tools 组是否已由 `fe11c40` 覆盖（本轮未复核该提交清单），若仍有未跟踪副本则存在重复提交风险。
4. `ManagementShellPlayTests` 的旧基线未跟踪 → 长期回归在干净 clone 上必然失败（已隔离，不是产品缺陷）。

除上述四点，**产品闭包本身没有新的技术阻塞**：12 条 Prefab 引用全部可解析、0 missing script、4 个行模板齐全、配置链三库可读、编译状态正常。

## 6. 副作用清单

- 新增 3 个证据文件：本文件、`editor-prefab-dependencies.json`、`editor-candidate-review.json`。
- 门禁 records 2 条（begin/end）。
- 无其他写入；未改代码/配置/资产/测试/场景/存档；未 Commit/Push。
