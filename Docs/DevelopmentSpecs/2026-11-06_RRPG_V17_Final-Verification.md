# V17 最终关闭核验与必要收尾

日期：2026-11-06；目标分支 `ai/rrpg-r3b-population-pilot`。开工 HEAD `4d9ea21`，index 空；未发现后续产品代码提交。本轮不是重复产品验收，综合产品验收和 Tavern 单独验收均保持通过。

## 结论与真实进展

**V17 PRODUCT ACCEPTED / LOCAL CLOSEOUT BLOCKED / NOT PUSHED。** 未证明整个工程可由提交快照独立重建，不标 LOCAL DEVELOPMENT CLOSED。

本次不再将上轮 Tools 排除作为当前授权障碍：制作人已明确要求核定 Compiler 依赖。完成 hunk 及路径核对后，精确本地提交 Initial World Compiler/Authoring/Mirror **13 文件**，实际 SHA：`fe11c406ac66f7103e2075e66d4fe21a955eaeac`。该组与本轮隔离验证的文件逐字节一致，不含其他 Tools、Runtime、Prefab、字体、场景、插件、Packages 或 ProjectSettings。既有实现原字节入库，没有新功能实施。

[本轮精确 Manifest](2026-11-06_RRPG_V17_Final-Verification-Manifest.json) 列 61 个路径，记录原始/本轮开工/当前 SHA、tracked、共享关系、依赖组、13 项实际提交及 48 项保留理由；取代旧 2452 项词法候选扫描作为本轮提交范围依据。旧 Manifest 和失败记录不删除。未将工作区文件存在误认为 Git 已保存。

## 两组原阻塞的核验

| 依赖 | 本轮核对及处置 |
| --- | --- |
| HomeProductShell → ManagementView/Prefab | HEAD 仍引用未跟踪的管理组件/Prefab。精确候选包含 View/Row/ReadAdapter/Gateway/Snapshot、Prefab 与 Meta；没有认为仅提交 Shell 就完成交付。 |
| Management Prefab 脚本/字体 | DSH 真实回执确认 Pilot Editor 未运行、MCP 端口未监听。12 个不同 GUID 中 9 个 UI/TMP 包引用已静态解析，3 个项目引用未证实，见下节。未加载到 Scene、未实例化、未测字体。 |
| Initial World Compiler / XLSX / 三镜像 | 7 个生产 Compiler 文件、2 个定向测试、1 个 XLSX、3 个 JSON 已完整精确提交；本轮隔离编译、16/16 测试、validate PASS。原 Compiler 授权/保存缺口已解决。 |
| Runtime / Editor / Registry / Save / PlayerCollection | 仍保留未提交：Catalog、Runtime 模型、DB、Root/Registry、Builder/Reader、SaveSystem/PlayerCollection、Building/Bridge。变更属于已验收 V17；共享是两个 V17 包之间，不据此虚构存在不相关任务混入。未确认该产品组在独立快照中能完整加载/生成/编译，故没有强行提交子集。 |
| 测试与保护夹具 | 管理 Play 读取绝对 root 的历史 workspace-baseline.json；文件未跟踪且其 34 项中 TMP 当前字节已与旧基线不同。仅补该 JSON 不足保证测试能跑，不能回滚合法字体或偷偷更新历史基线。Edit asmdef/Meta、初始化产品测试等纳入保留候选，未执行 Unity 测试。 |

## 剩余阻塞：具体文件和原因

1. `UnityProject/RRPG1.0game/Assets/Resources/UI/ManagementView.prefab` 的三个项目引用：
   - `4a7dfebd6d5993c499d4f9a186b7d2c0`（m_Script）；
   - `f37df54fb96a70c45864d60504325b01`（m_Script）；
   - `f9418c72505f4cb41ab51da03f9fd2e0`（m_fontAsset）。
   项目 Meta 使用编码 guid、Prefab 使用 hex；没有实际 AssetDatabase 回读，不能断言分别指向 ManagementView/ManagementRowView/指定字体，也不能断言 Missing Script。两个脚本及其 Meta、Prefab/Meta 均在 Manifest。DSH 解析了包组件，未解析这三项，不把全项目的 203 条重复未知 GUID 当作 203 个坏资源。
2. `Assets/Editor/StaticConfig/StaticConfigBuilder.cs` / `InitialWorldMirrorReader.cs`、`Assets/Scripts/StaticConfig/InitialWorldConfigDatabase.cs` / `RRPGStaticConfigRoot.cs` / `StaticConfigRegistry.cs`：未执行独立快照 Unity 编译与 Builder 重建。隔离 Compiler PASS 不能代替这些 Unity 侧验证；生成 SO 在磁盘存在不代表 Git 已包含完整重建能力。
3. `Assets/Tests/PlayMode/ManagementShellPlayTests.cs` 的 OneTimeSetUp 读取 `Development/StudioRuns/R3B-20261022/workspace-baseline.json` 并对比 34 项。该 JSON root 固定为本工程路径；当前 TMP 已合法演化，和旧字体 SHA 不同。不能直接按旧夹具声称测试可重跑，也不能为过测试覆盖字体或抹掉旧事故。后续应独立处置当前运行保护夹具与历史基线的职责，本轮没有改测试。

这些是可定位的加载/验证/测试环境缺口，而非请求重做产品验收或开发新功能。没有发现需要以新玩法修改解决的证据。现有产品代码没有“归属未知所以一律不能提交”的泛化结论；当前保留原因是还未证明上述完整产品依赖组安全可重建。

## 本轮真实验证与局限

- 连接器 diagnose：绑定目标正确、DSH 空闲、无未知投递。固定 ID `v17-finalcheck-20261106-dispatch01` 预览后真实发送一次；accepted 与 turn-ended 之后读取了 result/Prefab/候选文件，不以受理等同完成。
- Compiler 在工程外隔离副本以 HEAD 为基准，仅覆入上述 13 个拟提交文件；定向 `InitialWorld` 测试 **16/16、0 failed/0 skipped**，编译成功。validate **PASS、0 errors/9 warnings、Mirror 未写**。现存缓存用于依赖解析，不是复制旧测试结果；测试程序集在隔离路径真实编译并执行。
- 首次 restore 访问宿主 NuGet.Config 被拒绝，指定临时 config 后仍拒绝，失败保留于记录；未读配置正文、未改 ACL。随后只向隔离目录复制已有 obj 依赖缓存，`--no-restore` 成功。此证据不证明全新机器无缓存联网 restore。
- 主开发复核 DSH 候选为不完整建议，未采用其不一致的分组数量与截短 SHA；本轮独立生成完整 SHA Manifest。其“Tools 另批”建议被当前制作人明确范围取代。Editor 不运行只证明本次没有 Unity 回读能力，不据此断言字体门禁脚本本身技术上不能 begin；本次未进行 Unity 操作，所以没有进入受保护流程，也没有自动启动或安装引擎。
- 未执行 Unity 编译、Builder、EditMode/PlayMode、Player、G6/G7；未重跑全量测试。历史测试和产品验收仍按原版本/范围使用，不作为本轮 PASS。

## 保护、状态同步与下一步

开工对 tracked 与 untracked 实际文件建立 4760 项 SHA 清单，位于 `D:/AI_Agent_Lab/audit-results/v17-finalcheck-20261106/before.json`。提交前后排除本轮明确状态/证据文件后复核其余原字节；真实保护结果在 `D:/AI_Agent_Lab/audit-results/v17-finalcheck-20261106/final-protection.json`，不以工作区 clean 为要求。原字体、场景、插件、配置以外的历史成果未回滚或清理。

已更新 Local Closeout Report、当前状态与 Handoff；本说明及精确 Manifest/必要回执另作文档本地提交，实际 SHA 见 Git 和最终会话报告，不预填自身 SHA。未 Push/Merge。

**下一步的具体操作**：需要在 Unity Hub 中打开 Pilot 项目 `D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game`，保持 Edit 模式；不要打开正式工程 `D:/BBB/RRPG1.0W`，不要保存场景、Prefab 或字体。之后由 AI 按现有字体 begin/end 门禁，回读上述三 GUID、实际引用与编译状态，再在隔离快照执行最小 Builder/配置回读验证；非 0 门禁停止。管理测试夹具需先完成保护语义审查，不能覆盖历史基线。当前未获得可用 Editor 能力，不能假称这些已执行。

完成本次核验后停止，不进入 FP 开发；G6/G7 正常玩家路径留待后续按整合文档进行。
