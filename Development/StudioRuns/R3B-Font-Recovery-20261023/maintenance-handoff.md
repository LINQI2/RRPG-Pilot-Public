# 字体保护恢复与后续技术续接

2026-10-23，维护代理依据制作人“技术问题你自己确认”的委托执行。只限Pilot，不是产品验收、不授权正式RRPG/新权限/Git写操作。

## 已核实并处理

- 原字体完整字节备份在Studio维护范围内，SHA256与开工基线精确一致：3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854，14961033字节。原先只查Pilot导致遗漏该备份。
- 变化版本e40b6d77a93712ee5371d35311051edfeb50f730d4d07ef5e03bd7dd7556f2d8、14968490字节已完整备份；差异仅为图集像素、字符/字形表和图集矩形数据，未改保护基线。恢复经UnityEditor.FileUtil.ReplaceFile及ImportAsset，仅该字体，meta哈希保持原值。源字节在本目录original-font-bytes.bin，仅恢复证据，不是运行配置，不得自动导入。
- 已查明直接保存路径：原f6ae执行seq331在12:30:58Z调用rql-mutate的editor|call RefreshAssets()，字体mtime12:30:59Z。MCP1.3.0实际源码RqlUnity.Save.cs：Mutate执行普通call后调用SaveChanges，后者AssetDatabase.SaveAssets并保存dirty场景。RefreshAssets方法本身只有Refresh，但外层mutation仍全局保存。维护排查中重现相同写回后，确认精确同一e40版本已备份，再定向恢复，未盲目改基线或冒充通过。
- Studio已部署官方DSH ctx.tools.guard：仅受保护projectAuthorization任务拒绝rql-mutate；仍保留rql-query、script-execute和既有文件工具。刷新请通过script-execute调用AssetDatabase.Refresh；保存只用SaveAssetIfDirty/PrefabUtility等精确目标API。禁止用script-execute绕过为全局SaveAssets/SaveChanges。守卫不是任意C#安全沙箱。
- GrowthDataInitializer.cs修复另一个确定风险：移除全局SaveAssets和对已存在资产的无意义SetDirty，仅保存自己新建的资产。GrowthDataSeedTests.cs新增真实dirty无关材质不会被保存的回归。没有改变成长资产或数值。
- ManagementShellPlayTests.cs的AdvanceTo辅助函数原错误假设WorldInstant/WorldAdvanceOrigin在Assembly-CSharp，改为从真实clock.GetType().Assembly定位。业务断言、日期、到场和劳动标准未放宽。

## 原始宿主结果（executionId均为原f6ae，不是新模型执行）

通过studio_task只读路由读取studio-test-record/<UUID>.json：

1. 13f00282-ce09-43d7-82d9-238bee4a66eb：首次成长类筛选返回0/Unknown，保留原回执；核对原生空闲后只释放自己的锁，未标PASS。只读发现实际2例，改用官方RQL精确选择。
2. bcefcb68-ab6f-4b7c-8ff4-52c3b2b0c8ac：GrowthDataSeedTests真实2/2通过，0跳过，1.13768秒，含不保存无关dirty资产。
3. 720cdd97-d488-42dc-a2c8-b958653f6506：ManagementShellPlayTests真实3/5通过，2例TypeLoadException，原失败保留。
4. 273ef5ef-8f38-4d92-a03e-279ba52d8bcc：修正类型后真实4/5通过，0跳过，4.4705633秒。唯一失败ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor，期望p1实际到场construction:building_instance_1，实际null。已运行到真实业务断言，不是字体、Token或unknown阻塞。

14项保护与34项基线在最后测试后全部匹配；测试间不重写基线，不跳过保护。Studio类型检查/构建、127项自动化和20项隔离浏览器通过。新官方guard真实SDK initialize/shutdown激活成功，没有prompt；尚未付费验证模型实际尝试被拒场景，不声称任意C#不可绕过。

## 主开发接续

本轮维护源文件已交还，不与DSH并行写；原Thread、任务、历史保留。请读取原始回执和当前C#，在原d57任务上以一个明确小包修复剩余施工到场失败，先判断用例时间/输入前置与真实施工路径，不改断言求绿。正常技术问题在既有范围自主解决，不再将字体已恢复或普通夹具问题提交制作人选择。设计变更、超权限和正式项目仍须另行授权。

测试由Studio阶段check串行运行，全部通过后才做原R3-B剩余专业核验和中文交付。恢复报告/部分通过不等于整包完成。原f6ae仍failed是保留历史，不得改造成PASS。只读取证ff384已finished_unverified也不是产品完成。

## 2026-10-24 13:01 真实恢复核验（替代前文过早的成功判断）

维护暂停现在可以解除，普通业务失败交还原主开发，不要求制作人处理技术细节。

- 原生编辑器确认Pilot、2022.3.62t9、无编译失败、TestRunner空闲；不重启/关闭用户编辑器。
- 原字体磁盘SHA保持3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854；14项保护文件测试前后全匹配。
- 测试资源隔离已恢复真实内存字段/图集与引用，并验证引用仍属原字体，不指向已删除快照。早期维护恢复曾错误保留临时快照引用，造成MissingReference失败；该失败已保留，不算通过。现修正了持久GUID映射和引用归属检查。
- 最新宿主真实PlayMode记录3016ddd9-eaa5-45be-a2ab-cdc0d027fc89：5项，4通过、1失败，4.1753459秒。原始结果可沿原f6ae执行成果读取studio-test-record/3016ddd9-eaa5-45be-a2ab-cdc0d027fc89.json，不能把旧失败记录覆盖为通过。
- 失败为ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor：预期到场495、实际494；已经不是OneTimeSetUp字体差异、断线或临时资源引用异常。由主开发核对真实工作边界/通勤契约，修复相应测试或产品缺陷，不降低断言、不让制作人判断技术分钟数。
- 同次记录resourceScopeEnd=scope-restored|font|0|632，原始回执/磁盘保护均已核对。Editor当前无遗留运行/请求，工程锁已正常释放。
- Studio测试前Begin、测试后End资源回收逻辑已构建并经过134项自动化回归（含UnityConnection断线恢复），保持保护基线、串行测试和未知不重发规则。SDK侧rql-mutate全局保存拦截仍保持。
- 原R3-B目标、双Thread、原任务记录不变。请沿原任务最小修复并继续整包验证，不新建重复任务、不再次调查已完成的基础能力。不得用本次4/5宣称整包通过；其他原要求的验证仍需完成。
