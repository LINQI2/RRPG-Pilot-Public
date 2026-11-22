# Initial World 产品体验修复：已授权 DSH 工作包

2026-11-05。制作人已在Codex会话明确“批准”本次A/B/C工作包、Pilot工程写入和必要DSH开发调用。

## 先读

- `Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Initial-World_Product-Experience-Fixes_Development-Spec-V0.1.md`（本包正式规格，全部行为和验证要求）。
- 根AGENTS.md/.clinerules.md；按需读取既有UI/Save/投影宿主，不重复全仓审计。
- `Development/StudioRuns/Initialization-20261105/font-protection-README.md` 与 `scripts/font-guard.ps1`：**本包新增强制门禁**，不能沿用你上次的无门禁执行方式。

唯一工程 `D:\AI_Agent_Lab\rrpg_pilot\repo`，Target Branch `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`。禁止正式工程、连接器、认证、权限、Package/引擎变更、Commit/Push/reset/clean/restore。保留当前所有历史未提交成果，既有文件改前备份并记SHA。根外的本包保护清单已由Codex记录；原77修改/108未跟踪项展开1416文件。

## 工作要求

按规格串行实施 A→C→B 并完成真实验证：

A：A档未配置独立世界实体的虚拟ActivityPlace（Tavern及同类）不创建可见建筑/占位图，但Place身份/活动事实/目标anchor/人物到达与现场状态全部保留。真实Farm/Kitchen/House实例视图不动，B原型对照保持原行为。不要全局改DrawsWorldEntity导致B改变，不删除活动系统/配置，不按显示名硬编码Tavern特判。

C：管理资源行保持纯展示，撤掉整行跳转与旧提示，添加明确“查看岗位”按钮，通过完整WorkplaceId精确定位Farm/Kitchen实例slot并滚动，保留当前Person选择；无派工/撤职/资源或时间写入。不重做整个管理UI。

B：现有Home右上“设置”，轻量菜单仅“保存游戏/保存并返回主菜单/关闭”。复用唯一SaveSystem和真实MainMenu/Town入口。成功反馈明确；失败保留原页面并显示原因，保存返回只能成功后跳转。busy guard和禁用按钮防重复/重入；全屏Raycast遮罩及既有SurfaceModalActive阻断世界点击/建造旁路。不得NewGame/重建实体，不添加其它设置。若原MainMenu无返回方法，允许在原宿主新增薄返回接缝，禁止第二套主菜单/存档系统。

初始1 Farm/1 Kitchen/5 House及资源/人口/住房规则不变，SaveVersion/DTO不变。普通实现、编译/测试失败自主窄修，不重复请求制作人批准；只有真正越权/新权限/不可逆或重大设计取舍才询问。

## 字体门禁：必须执行且不得绕过

从repo根调用：
`powershell -NoProfile -ExecutionPolicy Bypass -File Development/StudioRuns/Initialization-20261105/scripts/font-guard.ps1 begin -Label iw-productfix-dsh-step01`

仅begin退出0后，才执行该段Unity读取/Refresh/Builder/Play/测试；无论成功/异常，finally执行相同Label的 `end`。不同段用不同Label，显式记录两个退出码。**end任意非0，立即停止之后的全部Unity操作，保留incident，不继续复测，不capture/-Force，不自动恢复，不清dirty伪造保护。** 前一步begin失败也不能继续Unity。

现行基线f0f5b901…/14969377B已经制作人采用且有完整备份；不使用旧固定3c758e16的StudioTestResourceScope替代。门禁脚本可能在受限Shell不能写records；如实报告精确失败。可在现有普通用户执行能力内用既有方式执行，但不得通过未经授权的ACL/FullAccess修复。

当前既有Unity HTTP通道历史21867/mcp可供发现，但必须通过实际公开工程配置及真实API再次确认精确Pilot身份；不猜端口、安装工具或启动重复Editor。每次C#后Refresh/等待编译/Console必须处于门禁段内。序列化资产只经Unity Editor/MCP，不手改YAML。

## 验证与原始证据

至少完成规格9组矩阵：A活动地点保留事实但无伪实体；B原型回归；Home设置开关/成功/失败/成功后返回；重复点击与世界/建造穿透阻断；Save→真实Continue保持Person/Building/Assignment/Resources；资源普通点击无导航及显式查看岗位精确定位且Person选择保留；Compiler/validate与相关Edit/Play真实回执；存档/场景/字体及历史成果保护。

可复用已有测试夹具，新增最小必要测试。测试仅串行；失败保留真实stack并窄修，不削弱断言/隐藏失败，不将旧280/24/3当本包PASS。确定性保存失败用已有测试接缝，不损坏真实用户存档。保存/Continue须真实产品入口，程序化EventSystem与物理输入证据区分。

每段原始MCP测试响应、逐例结果、门禁records路径、存档guard、截图都写到本包目录，不只留口头摘要。禁止清除Console以伪装没有错误；区分基线和本包新增。

## 所有权与回传

DSH可改本包必要产品/测试/Prefab（Editor定向），Codex暂不并行改产品文件。DSH不改Current文档/本包DevelopmentSpec/索引；Codex负责统一同步，避免冲突。

马上写本目录 `dsh-progress.md`，执行中每个阶段更新，完成/阻塞写 `dsh-result.md`。报告实际文件清单、前后SHA/备份、Editor现场、编译Console、真实测试逐例和原始响应路径、门禁begin/end退出码与incidents、保存/导航产品证据、保护核验和制作人体验步骤。Implemented/Verified/Product Acceptance Pending/Deferred分开。

出现字体门禁非0立刻停止Unity，并将状态写入结果文件与公开回复，不只登记ask_codex。普通技术裁决可用ask_codex，需制作人操作时必须说明在哪个会话/界面做什么及原因。不要假定自动成果回传。Codex会读取实际回复/文件并独立技术复核。
