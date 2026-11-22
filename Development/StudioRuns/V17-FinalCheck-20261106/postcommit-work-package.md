# V17 本地提交后最后核验

当前真实HEAD已由主开发提交到21004a26e716c6bcdae10b8ada2f329de27e4e81；Compiler fe11c40此前已提交。你先前报告HEAD4d9ea21是旧值，不要复制旧模板。先用真实git rev-parse HEAD取当前值并保留原始文本回执。

沿用原任务门禁和保护，无Play、无产品重验、无Git写、无新功能、无资产/Scene/Save/字体修改。begin成功后才Unity只读查询，finally end非0停止。

1. 请求一次正常ScriptCompilation（不是Refresh/Builder/Play），等待域重载结束，再回读精确Pilot project、scriptCompilationFailed、Console编译错误、isCompiling/updating、当前程序集加载及ManagementView/InitialWorldConfigDatabase类型。保留真正工具返回的原文JSON，不只写总结。不清Console。重载不等于产品测试通过。
2. 只读反射调用现有MirrorReader/ComputeSignature/IsUpToDate等查询（可读取StaticConfigBuilder真实实现后构造参数）。确定当前生成Root/InitialWorldDatabase的三个镜像SHA/数量、签名与现有配置是否匹配。禁止调用写入入口。若可安全证明Build(false)必然NoOp则仍优先只读查询，不执行Builder写入。无法构造纯校验就如实输出，只报告已有库可读不能改写成重建通过。
3. 回读Prefab同三GUID和missing script是否仍0。对本轮提交55路径中当前Editor相关源码记录SHA，主开发核对git对象。保留真实工具原文至 postcommit-raw-responses.md；仅新增 postcommit-result.md / postcommit-state.json；不要覆盖旧报告。
4. 基于真实证据区分“已提交源码/Prefab闭包完整且当前Editor编译通过”和“冷启动clean clone全重建没有执行”。不自行宣布产品/版本关闭，主开发决定。旧ManagementShell验收脚本机器基线作为Disposable历史保留磁盘不进长期回归，不是产品依赖，不要再据其未提交一律判产品阻塞。

普通技术问题自主安全处理，完成即停止；不进入G6/G7。
