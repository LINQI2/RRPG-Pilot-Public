# V17 Editor 已打开后的续核验（制作人明确要求继续）

唯一工程 D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game。继续既有V17收尾授权；不产品重验、不Play、不FP/G6/G7。主开发负责Git；你不得Commit/Push、ACL、安装升级或改连接器。

先读本目录原任务和 result.md，及 Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Final-Verification.md。原Tools阻塞已处理：fe11c40精确13文件已提交；Compiler隔离16/16与validate PASS是真实本轮，不必重跑。

1. 按已交付字体门禁实际README，begin成功才操作，finally end非0停止，记录精确Label/退出码。不要猜“Editor关闭则脚本不能begin”。回读当前Pilot身份且isPlaying=false；不要关闭制作人场景或退出Play（若实际Play则停相关操作告知）。
2. 用现有MCP/AssetDatabase只读核对ManagementView.prefab 12个GUID，特别4a7dfebd6d5993c499d4f9a186b7d2c0、f37df54fb96a70c45864d60504325b01、f9418c72505f4cb41ab51da03f9fd2e0。只读PrefabAsset/MonoScript类型/序列化引用、Missing Script、四个行模板及必要字体资产。不实例化到场景、不字体测量、不SaveAssets/Refresh。输出精确AssetDatabase GUID→路径、脚本类型和当前SHA。
3. 回读实际编译错误状态及当前源码程序集是否对应；如必须重新编译，只能在门禁与保护清单下RequestScriptCompilation，不改代码、不Refresh/Builder，不假称隔离提交编译。回读Root InitialWorldDatabase和三JSON对应数据、Registry/Catalog可用性，Builder现有纯校验/NoOp签名查询可用则只查询，不调用写资产入口。
4. 主开发准备将原61候选中13已提交之外产品Runtime/Editor/Prefab/Meta/必要长期Edit测试等作为完整闭包。逐hunk审查余48路径建议，哪些是已验收V17内共享、哪些有真实无关混合，逐路径不要笼统报“shared”。旧ManagementShellPlayTests读取机器绑定旧基线，考虑作为已完成一次性验收工具保留磁盘/历史证据但不纳入长期回归：核对其他拟保留源码是否编译依赖该测试类型；不修改/删除该测试或保护基线。不要为消除旧测试环境问题而改字体或弱化保护。
5. 证据只新增 editor-resume-result.md / editor-prefab-dependencies.json / editor-candidate-review.json，不覆盖旧结果。给出是否还有阻止产品源码/Prefab闭包提交的真实缺口。当前字体/场景/历史成果原样保留；所有副作用明确列出。

禁止新功能、场景修改、存档、资产替换。正常技术问题自主安全处理。完成即停止，不重复用户验收。
