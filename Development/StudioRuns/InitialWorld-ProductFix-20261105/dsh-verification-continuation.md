# 产品体验修复：技术复核后补齐验证

本条延续制作人已批准A/B/C，不新增产品范围。原dispatch01已交付且原生seq2533空闲，门禁v2完成并已由制作人选择合法Dynamic增长方案。不要重发原任务。

先读同目录规格引用与 `dsh-result.md` 最后§10，以及**最新** `Development/StudioRuns/Initialization-20261105/font-protection-README.md`、font-guard.ps1/core。v2的PASS/LEGITIMATE_GROWTH=0可继续；INCIDENT=2/NEEDS_REVIEW=4或其它非0仍立即停止后续Unity，保留incident，不capture/-Force，不静默改判定。历史incident保留。每一段仍begin/end finally成对、独立label（如iw-productfix-review01）。

Codex已核对代码与5例测试：实现存在，但剩余验收证据不足。请自主补齐必要最小测试/修复：

1. Home设置真实入口打开与关闭、仅3动作、全屏遮罩与世界输入链：通过EventSystem Raycast/实际产品世界点击路径证明不穿透到人物选择/建造，不能只assert SurfaceModalActive=true。关闭后恢复已有输入阻断语义。
2. 保存成功只保存、不跳转，反馈明确，Person/Building/Assignment/资源不重建；保存失败与保存并返回失败留原Home。利用ForceSaveFailureForTests测试缝计数/嵌套调用验证busy防重入，并验证同帧连续点击不会产生重复保存；若当前guard只在同步Save期间有效而同帧双击重写，做最小防重复修复，不改Save DTO/Version。
3. 保存并返回后，经真实主菜单OnContinue→Town.Initialize重新进入Home，验证人物ID/建筑实例ID/岗位职责/资源一致，无reseed/重复结算，Continue按钮状态真实可用。先保护**本次最新用户单槽**，不能恢复成上轮21611B旧值。
4. 资源页测试先选择一个真实Person再验证保持；将页切到Resources后普通信息区真实点击不导航、不写职责/库存/时钟。分别用Farm/Kitchen显式入口核对完整WorkplaceId、_scrollToLocatedRow/实际滚动定位，不只比候选字段。button交互是合法的最小Prefab/运行时接缝即可，不重做布局。
5. A隐藏Tavern/其它无独立实体活动地点须仍能以真实Activity/Presence读取目标与现场事实；现有合成投影测试保留，但不要将“合成Tavern快照+Domain不变”冒充实际活动调度验证。尽量复用现有活动/现场产品测试，再补最小投影断言。B既有2例保留，不削弱断言。
6. 本包修改后真实Compiler测试/validate与相关Edit/Play定向回归。旧280/24不可直接当本包结果；若只涉及Unity代码，可解释Compiler相关范围，但本包验收仍要求本次真实回执。Codex不并行跑测试。

本轮实现前补上你遗漏的备份要求：将**现在6个代码文件与现有测试**完整字节备份至本目录review-before/并记SHA，不猜测或逆改上轮dirty内容。原未备份偏差仍留在历史报告，不宣称已修复原历史缺失。

若新增测试暴露普通bug，在本包内窄修、Refresh/编译/Console与验证均置门禁段内。没有新权限不询问制作人。所有Unity段串行，非0立即停。

回传到独立 `dsh-verification-progress.md` / `dsh-verification-result.md`：逐例真实结果和**原始MCP JSON响应保存为文件**；runID/时间与门禁begin/end记录；存档/字体/场景/原文件保护核验；截图（Settings、Resources明确按钮、A场景）。不要仅口头PASS。保留原报告，不改Current文档或规格。完成后Codex独立复核并同步文档，Product Acceptance Pending。

仅限Pilot `D:\AI_Agent_Lab\rrpg_pilot\repo`，Target Branch不变；禁止正式工程/连接器/权限/Commit/Push。原生现有Unity通道21867/mcp使用前核对Pilot，勿重复启动Editor或安装包。
