# Initial World：环境恢复后的原工作包续执行

本条是已知完成的环境阻断回合之后的新续执行，不是重试或重发原投递。原派发编号 `initialworld-20261105-dispatch01` 已 accepted，原会话 seq167 返回结果；维护探针随后 completed（seq193）。2026-11-05 12:49:28 diagnose：原绑定正确、Pilot 项目读写、空闲、未知投递0、待答0。制作人现已明确要求“已解决问题，继续任务”。

读取同目录 `dsh-work-package.md` 和 `execution-record.md`，保持原有全部范围与约束，从当前实际状态继续 A–D；不要重复 Compiler 全量测试。Codex 已完成本轮 Compiler 280/280、0 skipped 及 validate PASS（0 error、9 navigation warnings）。原 testhost 超时0项回执保留。

维护文档 `D:\AI_Agent_Lab\Codex_DSH_Connector\docs\16-permission-approval.md` 只按需读取公开说明；不要读私有配置/原日志。此前 Win32 5 经维护修复且原会话 pwsh 探针已成功。**不请求 Full Access、不修 ACL、不修改连接器、不自行扩大权限。** 原报告中关于目录缺权限的推论及 Full Access 建议不作为后续行动依据。

1. 先用既有工具确认当前 CWD（显式 `.Path`）、文件写入/哈希可用，再检查本会话现在是否有 Unity 工具。勿把维护探针成功当作 Unity 成功。
2. Pilot 的 Temp 目录现在存在，但不能据此认定 Editor 在线。用实际工具确认唯一 Pilot 项目、Editor 版本、Console、编译/播放/runner/scene/font 状态。不得仅据旧日志或目录推定状态。
3. 若无 Unity MCP，先检查工程已有的官方 Unity MCP/Bridge 能否通过**现有已配置通道**使用。允许读取公开工程内工具说明、现有工具入口；不得新增框架、安装/升级 Package、改认证/连接器/全局配置或手改序列化 YAML。现有项目为 2022.3/Tuanjie，不安装仅适配 Unity 6 的 Pipeline。
4. 若通道实际可用，按原工作包执行 V4、A表现收尾、定向验证。Shell/Compiler 普通技术问题自行处理。配置动前说明以避免冲突，Codex 当前不改产品/配置文件。
5. 若仍缺 Unity 通道，准确报告实际尝试、真实错误、现有入口名称和缺少的连接步骤，不泛称“需要权限”，不重复要求已修 Shell。可以完成独立非Unity准备，但不要在无法Refresh/编译/Console的情况下修改产品C#。
6. 原 `dsh-progress.md`/`dsh-result.md` 保留为历史，不覆盖。新进度写 `dsh-resume-01-progress.md`，新结果写 `dsh-resume-01-result.md`；原报告若要引用，明确其为恢复前历史。开始即写进度，执行中更新，最终给实际证据路径。
7. 原保护清单继续有效；主开发已在前回合仅追加5份Current文档并备份，详见 protection-after-doc-sync.json。勿覆盖这些文档或本轮 DevelopmentSpec。

当前任务已有实施授权，普通技术步骤不再等制作人逐项批准。遇到真正新权限、重大设计变化或不可逆迁移时才询问，并说明准确操作与原因。完成任务后由 Codex 独立复核实际差异与证据；接口accepted或文件写入都不算验收。
