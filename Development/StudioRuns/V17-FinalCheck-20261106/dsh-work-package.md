# V17 Final Check：只读技术核验与精确依赖建议

制作人2026-11-06已授权最终关闭核验与必要收尾，不开发功能、不重复产品验收。目标唯一：D:/AI_Agent_Lab/rrpg_pilot/repo。主开发自己负责最终Git提交；执行端禁止Git写入、Push/Merge、ACL/权限修改、场景/Prefab/Meta/字体/存档/配置修改。已有成果大量dirty，禁止清理/恢复。可以仅新增本目录证据。

请先读 Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Report.md 及 Dependency-Closeout.md。上轮Tools排除是历史；本次制作人明确要求核定 Initial World Compiler 精确依赖，不授权无关Tools改动。

任务：
1. 使用现有Unity/MCP只读能力核对Pilot精确身份、当前Editor状态。必须先读 Development/StudioRuns/Initialization-20261105/font-protection-README.md 及当前v2实际用法，begin成功后才执行受保护操作，finally end，非0停止并保留incident。不得擅自退出用户Play、保存资产或恢复字体；不进Play、不Builder、不Refresh。
2. 只读 AssetDatabase / MonoScript / PrefabAsset 查询 ManagementView.prefab 的完整GUID依赖映射、ManagementView/ManagementRowView脚本、TMP/UI/字体等必要依赖与Missing Script/引用缺失。用实际AssetDatabase GUID说明磁盘编码Meta不能原文匹配的原因，不猜解码，不改YAML。禁止 Instantiate/加载到Scene/字体测量。若只读加载也可能触发保护则按门禁执行。
3. 核对现有编译错误状态，不因读取Compiled程序集就声称提交快照编译通过。输出当前源码SHA与证据。静态追踪 Management以及 Initial World 新增/修改文件，给出精确候选路径，标出混入其他任务者及原因，不仅输出泛化依赖扫描。
4. 审查ManagementShellPlayTests的workspace-baseline.json依赖如何影响提交快照的重跑，测试夹具和产品依赖分开，禁止改测试。检查Initial World Runtime/Editor/Compiler/Registry/三JSON/XLSX/Meta/asmdef，必要测试是否引用尚未提交的保护工具或其他资产。给出具体缺口。
5. 尽快输出本目录 result.md 及 prefab-dependencies.json / precise-candidates.json（逐路径归属、依赖、当前SHA、是否共享、是否推荐纳入）。明确未执行项目。主开发同时准备隔离Compiler定向验证，你不跑Compiler或全量测试。

不得处理G6/G7、FP功能；接口受理不是完成。有普通技术问题自行安全处理；确实无法安全查询时写具体能力/资源缺口，不修改连接器。完成证据后停止。
