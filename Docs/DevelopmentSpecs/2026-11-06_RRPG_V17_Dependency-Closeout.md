# V17 最终依赖收尾与提交边界

日期：2026-11-06。Target Branch：`ai/rrpg-r3b-population-pilot`；开工 HEAD `b927bb9`，index 空。

## 最终判断

**V17 PRODUCT ACCEPTED / LOCAL CLOSEOUT BLOCKED / NOT PUSHED。** 不签发 LOCAL DEVELOPMENT CLOSED。本次完成静态依赖盘点，但没有完整交付快照的重建证据；没有新功能实施、编译、Builder、Unity 或测试执行。

本次指令明确排除 Tools，而 Initial World 正式 XLSX→Compiler→Mirror→Builder 链的 Compiler 实施位于尚未提交的 Tools。不能静默把“排除 Tools”解释成允许提交其中的 V17 文件，也不能把现有 JSON 留在磁盘等同完整源码重建。这个明确边界已经足以阻止整个包的安全提交，本轮不拆出不能独立交付的 Runtime 子集。

## Manifest 的范围与证据力度

[逐文件 JSON Manifest](2026-11-06_RRPG_V17_Dependency-Manifest.json) 包含路径、归属、当前 SHA256、原始清单 SHA256、是否匹配、HEAD blob、tracked/changed、候选依赖、共享状态和排除原因。原始清单位于 `D:/AI_Agent_Lab/audit-results/v17-local-closeout-20261106/pre-closeout-inventory.json`，没有覆盖它。原始无记录项为 null，不伪造开工 SHA；HEAD blob 是 Git 对象 ID，不冒充原始文件 SHA256。

方法：从 Management/InitialWorld 变更和已提交宿主出发，递归枚举项目类型引用候选、Prefab GUID、相邻 Meta，另补配置及测试文件读取边。类型词法扫描可能把注释、同名类型视为候选，因此是保守上界，不是编译器绑定证明；外部 Package/Builtin GUID 未解析单列，不能直接算 Missing Script。未改动基础文件仅标 reuse；改动共享文件标 review，不据自动扫描授权提交。JSON 中未确认文件明确保留 unknown，不能把它当成已冻结的提交白名单。

## Management 依赖组

1. 已提交 `HomeProductShell` 使用 `ManagementView`，并 `Resources.Load<ManagementView>("UI/ManagementView")`。因此必须同时纳入控制器、`Resources/UI/ManagementView.prefab` 和对应 Meta。
2. View → `ManagementReadAdapter` / `ManagementSnapshot` 读取投影，→ `ManagementCommandGateway` 执行四命令；共享 `HomeSelectionContext` / Shell / Person Detail / SaveSystem；行组件为 `ManagementRowView`。不能只提交 View 文件。
3. 领域依赖为现有 Person、Village Bridge、Building/Workplace/Project、资源和时间查询/命令。部分这些基础文件同时承载 Initial World 接线；不能凭文件名认定完全属于 R3-B。
4. 输入验证涉及 `HomeWorldCamera` 的 Editor-only 输入采样及 `HomeBuildingPlacement` 守卫衔接。它们是当前脏文件中的关联改动，而不是本轮新写代码；完整采用须与原测试契约一起审查。
5. `ManagementShellPlayTests` 的 `SaveSlotGuard` 在测试文件内部；不是必须新增外部类。但 `BaselineRelative` 实际读取 `Development/StudioRuns/R3B-20261022/workspace-baseline.json`，属于历史机器路径/哈希基线依赖。不能在排除历史保护文件后宣称这套测试可独立重跑。产品依赖与验收工具依赖分开登记，本轮不修改测试消除它。
6. Prefab 序列化包含字体、UI/TMP 和脚本引用；但本工程当前 `.meta` 的 guid 使用编码字符串，Prefab 中为十六进制 GUID，不能通过原文字典直接一一映射。Manifest 单列 203 条未解析 GUID 引用记录（含重复引用，不是 203 个损坏资源），管理 Prefab 的脚本/字体 GUID 绑定未得到 Editor 回读证明。View/Row 与 Prefab 的预期关系有规格及宿主来源，但不能当成已解析的 GUID 边。不解码猜测、不改 Meta、不把未解析说成 Missing Script，也不据它声称完整序列化依赖已证明。保留版本和当前字体/包是否等效仍未通过独立重建证明。

## Initial World 依赖组

```text
RRPG_InitialWorld.xlsx
  → Tools/RRPG.ConfigCompiler（字段要求 / InitialWorldValidator / CLI Seed）
  → InitialWorld.json + InitialPopulation.json + InitialBuilding.json
  → Editor/StaticConfig/InitialWorldMirrorReader + StaticConfigBuilder
  → Generated InitialWorldConfigDatabase / RRPGStaticConfigRoot
  → StaticConfigRegistry / InitialWorldConfigurationCatalog
  → SaveSystem.NewGame(config) / PlayerCollection.InitializeFromConfiguration
  → VillageRuntimeBridge.TryPrepareNewGameWorld / CommitNewGameWorld
  → BuildingRuntime / 完成建筑实例、岗位、人物、资源
```

- 生成容器是 Builder 产物，`Assets/Generated/StaticConfig` 无 tracked 路径；磁盘存在不等于 HEAD 包含。应该用提交内可重建的生产链证明，而不是直接暂存生成 SO 或编辑 YAML。
- Runtime 模型、Catalog、ConfigDatabase、Root/Registry、BuildingRegistry、Bridge 的初始化路径、PlayerCollection/SaveSystem 的新游戏入口是同一依赖组；Continue 仍复用已有存档恢复，不重建。
- CompilerRunner 的脏 diff 明确调用 `InitialWorldValidator.Validate`；EntityRequirements 脏 diff 登记三张表必需字段；另有未跟踪 Validator/WorkbookSeed 和配套测试。若采用这些 diff 却排除对应类会缺依赖；若全部排除，则仍是旧 Compiler 契约，不能据当前工作区 Compiler280 历史回执证明提交链等效。
- Initialization Edit 测试 asmdef 引用 `Village.Runtime`、`Person.Runtime`、`StaticConfig.Runtime` 和 `Assembly-CSharp-Editor`；所以 Editor Reader/Builder 不是可省略的纯工具展示文件。Meta/asmdef 和配置 JSON 均列在 Manifest 中。
- 现有其他测试修复可能只是在适配默认新游戏、实例岗位或时间边界，也可能关联先前任务。自动类型依赖只能识别候选，未证明每项都应长期保留。原历史失败、事故和回执不删，不批量将其加入提交。

## 排除与未解决项

| 项 | 处置及原因 |
| --- | --- |
| Tools Compiler 组 | 明确排除且为完整配置链依赖，阻止完整包提交；需制作人后续明确其精确文件是否可纳入，当前不请求扩大整个 Tools 权限。 |
| TMP、场景、插件、Packages、ProjectSettings、Growth SO、历史恢复工具/事故文件 | 原样保留，不暂存、不恢复；GUID 能定位不等于字节归属已经获准。 |
| 测试基线与旧测试改动 | 分别登记为运行夹具/共享候选，不声称新 clone 可原样通过；没有为提交而改测试。 |
| 未确认归属 | 仍 excluded/unknown，不冒充完整安全 whitelist。 |

因此，Manifest 是完整候选闭包与排除记录，**不是已经证明可提交/可重建的完整 V17 manifest**。没有必要通过提交更多无关文件来人为消除缺口。

## 实际写入、提交与保护

本轮只新增 Manifest 和本说明，向 Local Closeout Report 与 `Docs/开发验收交接.md` 追加本轮事实；精确本地提交仅这些审计文档，不包含代码、Prefab、配置、Tools 或资产。实际 SHA 在最终会话报告及 Git 历史提供，不预填自身提交 SHA。

提交前后对原清单中除授权文档外的文件逐字节 SHA 检查；另对 Manifest 捕获的当前文件复核，检查没有缺失/变化。不把最初清单到现在已授权的文档更新误记为保护事故。核验回执保存在 `D:/AI_Agent_Lab/audit-results/v17-dependency-protection-20261106.json`。未 Push、Merge、ACL/权限操作。index 空只说明本轮精确提交结束，不要求工作区 clean。

下一步仅在排除边界得到精确处置后继续依赖提交；不会开发新功能。本次完成后停止。
