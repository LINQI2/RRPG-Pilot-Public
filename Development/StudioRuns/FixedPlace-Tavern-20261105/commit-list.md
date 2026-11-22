# Tavern 提交清单（准确版，待授权后一次执行）

分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3…`，暂存区空，dirty 195，`.git/index.lock` 已由 Codex 清理。

## 写入权限现状（实测）

```
git add -- "GameDesign/07 Village.md"
fatal: Unable to create 'D:/AI_Agent_Lab/rrpg_pilot/repo/.git/index.lock': Permission denied
ADD_EXIT=128
```

DSH 沙箱对本仓库 `.git/` 无写权限；不再通过编辑器进程绕行。

## 提交范围（两类，需一次授权）

### 甲、纯 Tavern 新增（与任何其他包无关）

```
Development/SourceAssets/Village/A-Tier/tavern-a.png (+ .meta)
UnityProject/RRPG1.0game/Assets/Tests/PlayMode/Village/TavernFixedPlacePlayTests.cs
Development/StudioRuns/FixedPlace-Tavern-20261105/**
```

### 乙、与 Initial World 修复包共用的既有文件（无法拆开）

代码 6 个：
`ProductHomeSnapshot.cs`、`ProductHomeReadAdapter.cs`、`HomePresentationResolver.cs`、
`HomeWorldEntityView.cs`、`ProductHomeWorldHost.cs`、`HomeProductShell.cs`、`MainMenuUI.cs`

测试 1 个：`VillageVisualPrototypePlayTests.cs`

配置与生成物 7 个：
`RRPG_VillagePresentation.xlsx`、`RRPG_VisualAssets.xlsx`（二进制，两包行并存）、
`Generated/StaticConfig/{VillagePresentation,VisualAsset,AssetManifest,BuildingPresentationBinding}.json`

文档 10 个：
`Docs/项目状态快照.md`、`Docs/项目日志.md`、`Docs/文档地图.md`、`Docs/开发验收交接.md`、
`Docs/DevelopmentSpecs/README.md`、检查点、V17 固定场所 Spec、Initial World Spec、
`GameDesign/07 Village.md`、`GameDesign/当前设计状态.md`、
`GameSpec/Village/{Product-Home-World,Village-Building-World}-GameSpec-V0.1.md`

另：`Development/StudioRuns/InitialWorld-ProductFix-20261105/**`（该包证据，文档引用它）

## 明确排除

- `Tools/RRPG.ConfigCompiler/**`、`Assets/Scripts/{Save,Core,StaticConfig,Village/BuildingRuntime,…}`、
  `Assets/Plugins/NuGet/**`、`Packages/*.json`、`ProjectSettings/**`、`SampleScene.scene`、字体资产
  —— 属 R3-B 或未确认归属，本次不动。
- 其余 1689 个未跟踪项的绝大多数（Library/证据噪声等）。

## 执行命令（逐路径，不用 `add .` / `-A`）

```powershell
cd D:\AI_Agent_Lab\rrpg_pilot\repo
powershell -NoProfile -ExecutionPolicy Bypass -File Development\StudioRuns\FixedPlace-Tavern-20261105\scripts\commit-tavern.ps1 -Commit
```

脚本已由 Codex 修复为默认预览、显式 `-Commit` 才提交，且不删锁、不隐藏错误。

## 需要你给的两项

1. **Git 写权限**：允许 DSH 在本仓库创建 `.git/index.lock` 并写索引（原生权限申请，不借编辑器）。
2. **范围确认**：乙类包含 Initial World 修复包的改动（Tavern 无法脱离它们单独可运行），
   请确认本次提交可包含该包。
