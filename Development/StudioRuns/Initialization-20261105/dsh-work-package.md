# Initial World 收尾：DSH 执行工作包

日期：2026-11-05。主开发 Codex；制作人已在 Codex 会话明确授权本工作包及必要 DSH 开发调用。

## 授权与归属

- 唯一可修改工程：`D:\AI_Agent_Lab\rrpg_pilot\repo`；Unity 子目录 `UnityProject/RRPG1.0game`。
- Target Branch：`ai/rrpg-r3b-population-pilot`；不得切分支、Commit、Push、reset、clean、restore 或覆盖原有成果。
- 正式工程 `D:\BBB\RRPG1.0W` 不在范围内；不得修改连接器、认证、ACL、Package、引擎、全局权限。
- 指定执行会话：`session-2af1b774-439f-416a-9e17-092a43095a66`。2026-11-05 11:04:16 诊断显示身份匹配、Pilot 目录、项目读写、无活跃执行、无未知投递。执行前再次核对自身目录。
- 此文件是执行规格，不是独立游戏设计 Authority。保留根 AGENTS.md 与 .clinerules.md 的工程约定；当前制作人指令优先。
- 原 dirty/untracked 保护清单已由 Codex 保存，不覆盖或删除历史文件、测试、备份、临时证据。对计划修改的既有文件先在本工作包目录备份并记 SHA256。

## 已确认设计与现有实现

不重写初始化系统，不重新设计游戏。人口、住房、农田、厨房、资源和英雄参数由可修改的数据配置决定；当前数量、比例和数值均未冻结。不新增住房容量、人口上限或成长规则。

正式建筑仅 Farm、Kitchen、House。工坊/铁匠铺不得进入正式目录或默认初始化，但保留历史资产、配置和存档兼容。Farm/Kitchen 必须是真实唯一建筑实例，完工后按功能定义注册独立岗位，支持管理 UI 派工/撤职、现有劳动和资源生产、保存恢复。住房不生成岗位。不能靠解锁 ws_* 静态岗位绕过实例系统。

已有实现：InitialWorldConfiguration、InitialWorldConfigurationCatalog、InitialWorldConfigDatabase、三份 Initial Mirror、Compiler 校验/Seed、Builder/Registry、SaveSystem.NewGame(config)、BuildingRuntime.TryAddInitialCompleted、VillageRuntimeBridge 候选世界事务、资源实例岗位、住房别名和显示去重。优先复用。

NewGame 只初始化一次；Continue 只恢复存档真实人物、建筑、岗位和资源，不补种默认世界、不重复结算。保持 Save Authority、SaveVersion=1 和既有 DTO；确实需要破坏性迁移或重大设计变化时停止相关操作并向制作人说明。

## 资料入口（不重做全仓审计）

1. 根 AGENTS.md 与 .clinerules.md；Docs/项目状态快照.md、Docs/开发验收交接.md。
2. `Development/StudioRuns/Initialization-20261027/final-delivery-review.md`，重点最后 §24–25。
3. 同目录初始化基础、配置管线、Unity 生成、产品实体接线、Play 集成及管理续接报告；仅按当前问题读取。
4. `Docs/DevelopmentSpecs/2026-10-07_RRPG_Village-Product-Home-and-Building-Foundation_Development-Spec-V0.1.md`。
5. `Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md`。

历史 Compiler 280、初始化 Edit 10/5/9、Kitchen UI 1、Home 3、管理 17、视觉 2 等记录只是历史，不当成本次执行通过。Codex 本轮正在独立运行 Compiler 测试及 validate，请勿并行改 Tools、XLSX、Mirror；先做下述 Unity 现场核对与 V4 验证，随后再动配置。开始动配置前用公开回复说明。

## 执行顺序

### A. Unity 能力与保护面

先确认实际可用 Unity 工具与精确 Pilot Editor。读取 productName/版本/项目路径、Console 基线、播放/编译/更新状态、Test Runner/pending、场景 dirty、PrefabStage、字体 dirty 与磁盘 SHA、当前用户存档及其 .tmp、既有 guard 状态。不得仅凭路径/权限标签声称工具可用。

若 Unity 未连接，或 Shell/Unity 工具实际失败：不要安装工具、重启正式工程、修权限、复制认证或改连接器。把准确失败和需人工操作写入进度与结果文件，并在 DSH 会话直接说明等待谁操作、操作位置与原因。不要只登记 ask_codex 后假定有人接手。

### B. 最小真实 V4 回归

先执行现有整类 `RRPG.Village.Tests.VillageV4ResourceSurplusPlayTests`（当前 1 例）：`FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry`。

要求 1/1 Passed、0 skipped；记录运行 ID/时间/总数/逐例结果/message/stackTrace/实际日志路径。它必须使用真实 NewGame、Save、MainMenu Continue、实例岗位派工、三天 Farm/Kitchen 生产、月度积累、真实远征补给扣减与撤退收尾。不得注入资源、削弱断言或把 skip/发现/静态检查写为 PASS。

先检查测试的存档 guard；按既有保护机制捕获并逐字节恢复用户存档、字体和场景。一个 Unity 写入/测试 run 完整结束后才启动下一个。失败按真实栈窄修，记录修前与修后，不无限重试。

### C. 默认 A 档表现绑定收尾

当前 A 为默认，Farm 有 TECH PROOF，Kitchen/House/Person 存在 neutral 占位；B 有既有 R2 资源。沿用现有美术方向与资源，修复 A 默认可识别表现；不得通过把默认切成 B 掩盖问题，不冻结最终画风。

先查现有图与实际映射入口，选择最小方案。正式配置必须走 XLSX → Compiler → Canonical Mirror → Unity Builder → Registry → Catalog，不直接编辑生成 JSON 或序列化 YAML。若涉及人物显示入口，仅做必要最小接缝，不重构整套 Home/管理 UI。保留 legacy workshop 资产，不将其加入正式入口。

必要 C# 修改后必须 Asset Refresh、等待编译、读取 Console 并区分基线；序列化资产只经 Unity Editor 工具修改。工具缺失则停止该写操作并如实回报。

### D. 定向验证与回传

按实际改动选最小测试：初始化配置/接线、真实 Kitchen UI 派工及加工、住房人物映射去重、Home A/B 实体稳定、管理派工/撤职、保存继续、旧档无 Buildings/静态职责兼容。不要无理由重跑全量长测。不能用局部测试替代真实产品入口。

核对 SaveVersion/DTO 未破坏、NewGame 不重复实例化、Continue 不重跑初始化、不重复结算资源。必要新增长期契约测试或专项测试均须真实运行，保留旧证据不删。

## 文件所有权与报告

DSH 可改与本包直接相关的产品/测试/Authoring及其正规生成物。Codex 负责最终统一审查与 Current 文档同步；DSH 不改 Docs/项目状态快照.md、Docs/开发验收交接.md、Docs/项目日志.md、Docs/文档地图.md 或 Docs/DevelopmentSpecs/README.md，避免并行冲突。

开始时在本目录新增 `dsh-progress.md`，记录正在做的步骤；遇到实质阻塞即写明，不等到最终才报告。完成后写 `dsh-result.md`，包含：

1. 实际修改路径、修改原因、前后 SHA256 和备份路径；既有成果保留情况。
2. Unity 连接与工具实际执行证据、配置生成命令和结果。
3. 测试命令/参数、运行 ID、逐例真实结果、0 项/skip/失败/未运行均单列。
4. NewGame → 管理派工/撤职 → 生产 → Save → Continue 和旧档验证证据。
5. 测试前后用户存档、字体、场景、guard 的保护核验。
6. 默认 A 表现的截图或可审查证据路径，以及制作人体验入口。
7. Implemented、Verified、Product Acceptance Pending、Deferred 分开描述。
8. 尚未解决的问题、具体人工介入步骤；不宣称整包验收通过。

Codex 会读取本地成果与差异并独立核验。消息 accepted、会话空闲或文件修改不代表完成。不得伪造回执或删除失败记录。
