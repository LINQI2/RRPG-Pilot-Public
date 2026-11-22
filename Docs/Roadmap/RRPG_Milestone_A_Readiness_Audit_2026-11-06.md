# RRPG Milestone A Readiness Audit

日期：2026-11-06。范围：只读调查与产品规划建议，不是 GameSpec、内容冻结、V18 立项或实施授权。

## 结论

RRPG 已有大量可复用的闭环机制和正式产品入口，V17 村庄综合体验已获制作人验收；**尚不能据此宣称 Milestone A 的完整核心价值已被证明**。主要差距是同一批人物、一次有意义承诺、一次过去被引用的社会回应、一次值得玩的远征及回归后的可感知变化，是否已在同一条正常体验中形成连续证据。不是缺少一整套新底层系统，也没有依据估算完成百分比或工期。

本轮没有运行游戏或测试。Existing & Proven 只用于已有明确证据的有限范围；实现文件及测试方法存在仅作接线线索。制作人本次五项验收只覆盖 V17，不能扩展为 Battle 乐趣、Social Response 或完整 Home–Away–Return 验收。

## 依据与 Authority

- [Milestone A Definition](RRPG_Milestone_A_Definition_v0.1.md)：四项核心产品价值，不要求完整职业/事件/地图/建筑/养成。
- [Integrated Concept Checkpoint](../DesignCheckpoints/2026-09-03_Playable-Milestone-A_Integrated-Concept-Checkpoint-V0.1.md)：Internal Vertical Slice、五结构证据、四动机和反摩擦方向；Concept，不冻结内容量、数值、开场或系统架构。
- `GameDesign/当前设计状态.md`、`01 产品定位与设计支柱.md`、`04 Person.md`、`06 Expedition.md`、`07 Village.md`：具体人物、共同体、Run 与长期成长分工，Village 资源不是唯一动机。
- [GameSpec 总入口](../../GameSpec/README.md)：Person、Battle、Expedition、Village/Product Home/Resource/Building、Save 的当前规则。Event 没有独立顶层 Current GameSpec，采用 `2026-09-26_RRPG_WP-Event-1-Unified-Event-Content-Foundation-FDS-V0.1.md` 及对应 Closeout，不恢复旧 Event 规则。
- [V17 最新本地收尾](../DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Report.md)、原 Final Status Report、R1/R2/R3 及 InitialWorld/ProductFix/Tavern 交付；Event-1、WP-AL-1～4、日内 A～D 和 Return/Battle 相关历史记录。

## 系统能力地图：机制、入口、体验分别判断

| 能力 | 底层机制 | 产品入口与证据 | 当前产品判断 |
| --- | --- | --- | --- |
| Person 连续身份/成长 | 稳定 PersonId、卡→人绑定、长期经验/成长、人生事实 | Person Detail 五页、出征选择与 Return 集成历史；V17 人物管理验收 | **Existing & Proven**：身份和管理范围；“玩家长期关心某人”仍 **Implemented but Unproven**。 |
| Assignment / Project / Building | 具体人占用、时间推进、施工/完成实例、生产 | 管理四命令、World 建筑；V17 生产/职责验收 | **Existing & Proven**：命令与实例变化；一个具有动机的 Project 内容体验 **Partially Implemented**。 |
| Social / Event / Memory | Social Fact、关系、Recognition、Event 条件/效果、历史及反馈 | Event-1 有 Technical Verified + Producer Play Acceptance；WP-AL-4 历史互惠已发布 | 基础 **Existing & Proven**；选定处理方式被后续人物/事件可理解地引用的完整切片 **Implemented but Unproven**，不是 Missing 全部 Social。 |
| Home 日内与 Away/Return | 统一 VillageTime、真实到场/劳动、Away 占用、Return 推进/待处理/结算 | 日内 A～D 历史验证；Return 集成方法与 V4 完整链测试存在；Home V17 验收 | 基础 **Existing & Proven**；当前默认开局下整条双历史汇合体验 **Implemented but Unproven**。 |
| Battle / Run | 当前 Battle 卡牌/AP/队伍与奖励规则，Formation/敌方意图/反馈、Progression/Branch/Supply/Retreat | Battle 正式入口历史到 Settlement；Return 成长修复历史 20 项专项/13 项产品链，Battle 产品反馈历史验收 | 入口与规则 **Existing & Proven**；“值得再打一局”的乐趣 **Implemented but Unproven**。不由通过数证明好玩。 |
| Save / Continue | SaveVersion 1、稳定 ID、本地单槽 | V17 保存返回及连续性综合验收；Product Fix 定向证据 | **Existing & Proven**：Home 保存；完整 Battle/Expedition 跨进程 Resume **Deferred / Not Frozen**。 |
| 正式内容与长期系统 | 测试/Calibration 内容与部分候选 | 没有完整正式内容冻结 | **Deferred / Not Frozen**，不是为 Milestone A 把全部内容生产补齐。 |

## 四项核心目标

| 目标 | 已有 | 核心证据缺口 |
| --- | --- | --- |
| Person Continuity | 村庄、选择、卡绑定、经验与人生事实接缝 | 同一个可辨识人物经历劳动、出征、返回后，玩家能说清“他发生了什么、下一次怎么安排”。状态：**Partially Implemented（体验层）**。 |
| Expedition Value | 战斗、分支、临时成长、Supply/风险及 Return 类型 | 正常内容下风险、选择与构筑后果是否可读、是否有乐趣；不是只在 fixture 中触发。状态：**Implemented but Unproven**。 |
| Return Value | Return 对账/长期结果、Home 推进与成长接线 | 回归反馈是否自然呈现 Home 和 Away 两侧变化，并导出下一选择。状态：**Partially Implemented（呈现/综合证据）**。 |
| Village Meaning | 持续活动/生产、固定场所、管理和 Project | 除库存变化外，玩家能否认识人、记得过去、想塑造共同体。状态：**Partially Implemented**。 |

## 五项结构证据

| 结构 | 机制/产品入口核对 | 判定 |
| --- | --- | --- |
| 连续 Person | `PlayerCollection` 显式卡→人绑定；`ExpeditionReturnIntegrityProductPlayTests` 从真实选择名单收集 PersonId；Person Detail 真实事实/成长入口 | **Existing & Proven（技术子范围）**；跨本轮默认配置的统一产品证据尚未确认。 |
| 真实 Project | `VillageProjectSystem` 保存 ParticipantPersonIds、容量、临时占用和时间进度；Building 施工/完成改变 World；Commitment 产品测试入口存在 | **Existing & Proven（机制）/ Partially Implemented（动机内容）**。不能把初始 Completed 建筑算作玩家亲自承诺完成的 Project。 |
| Social Response | `VillageSocialResponseLog` 保存 GrantRest/ContinueUsing；真实 Town Event 测试分别覆盖未来休息阻止抽调/继续允许；EventCondition 委托条件求值，配置含 RecognitionPerson/RecognitionType；WP-AL-4 为既往照顾影响未来 Help 选择权重 | **Implemented but Unproven（指定体验）**。这些不是自然生成的同一故事回执，权重增加也不等于必然再次帮助。后续明确引用过去的内容链仍需挑选与验证，不能据表名宣称全量 Recognition/LifeEpisode 已完成。 |
| 持续 Home | `WorldAdvance` 统一 Direct/Realtime/Jump/Return；日内历史证明快进等价；`IntradayReturnProductPlayTests` 包含实际到达、pending 可点续行及最终 Save 失败冻结 | **Existing & Proven（有限技术链）/ Implemented but Unproven（当前完整体验）**。Away 继续生活不等于离线无限模拟或跨进程 Run 恢复。 |
| 有价值 Battle Run | 正式 Town→Expedition→Encounter→Formation→Battle→Settlement 历史产品链与反馈验收 | **Existing & Proven（能玩与接入）/ Implemented but Unproven（值得玩）**。需制作人/设计顾问判断决策与 Build，不以新增战斗系统替代。 |

## Build / Care / Shape / Discover

| 动机 | 现有种子 | 判定与最小问题 |
| --- | --- | --- |
| Build | 人物时间换建筑/岗位/产出与空间变化 | **Partially Implemented**：选择一个玩家真正想完成的既有 Project，确认结果可见；不要求完整建设树。 |
| Care | 稳定人物、生活/成长/关系/经历、共同远征 | **Implemented but Unproven**：玩家是否关心某个具体人的后续，而非只看效率。 |
| Shape | 社会回应选择、事实/关系与未来抽调/反馈 | **Partially Implemented**：选择一个可回看的过去和后续反应；不先做 Law/Policy。 |
| Discover | Journey 分支/事件候选与成长、未冻结 Theme 方向 | **Partially Implemented**：下一轮新问题/机会需要有可读实例；完整 Theme/Question Progression 仍 **Deferred / Not Frozen**。 |

## 五条 Anti-Friction 风险

| 原则 | 现有保护/风险 | 建议核验问题（不是本轮实施） |
| --- | --- | --- |
| Return ≠ Administrative Reset | 返回结算与保存已有接缝；管理/成长/事件可能堆叠成待办 | 返回一次后，不逐项重新派工也能理解变化并继续选择吗？避免自动弹出全量报表。 |
| Expedition Cost ≠ Preparation Chore | Farm→Kitchen→Food→Carry 已接通；低库存不是本轮授权调整项 | 实际默认节奏是否迫使玩家每次先等粮/点多项准备？先测体验，不直接改数值。 |
| Reactive World ≠ Interruptive World | Recognition/Event、pending 暂停与续行保护；多触发可能切碎注意力 | 一次 Away/Return 中有哪些阻塞，哪些可稍后看？不把反馈全改成强制弹窗。 |
| Embodiment ≠ Traversal Tax | 世界点人、集中管理、显式岗位导航及固定场所已有 | 认识空间是否需要重复镜头/寻人/徒步成本？不为加强世界感先开发 NavMesh。 |
| History ≠ Permanent Constraint | 可保留事实与关系；部分休息窗口有期限；完整长期历史未做 | 玩家能否理解过去影响并改变未来？不要恢复 Legacy 永久伤残/死亡规则或把暂定处理变成永久惩罚。 |

## 当前可拼接的链与最小体验流程

现有入口是 MainMenu→NewGame/Continue→真实 Home→人物详情/管理→职责/施工与生产→选择人物出征→Journey/Encounter/Battle→Return→人物成长与 Home 状态→Save/Continue。分段有实现与历史产品测试；**本轮没有证据证明这是同一份当前默认配置、无 Debug 注入、完整体验一气通过的最新回执**。

建议讨论后选取一轮内部体验：认识同一批人物和 Home；安排一个人的职责；投入一个实际 Project 并看见机会成本；遇到一个社会问题并决定；选择既有允许人数出征，经历至少一次有意义分支/构筑决策；Away 时 Home 推进；返回看见人物结果和 Home 变化，以及对过去处理的再次引用；作出下一轮选择并保存继续。开场先短 Away 再回家是 Checkpoint 强方向，但尚非强制规格；六人队伍措辞也须服从 Current Party/Hero 容量，不能直接写新内容数值。

## 最重要的五个缺口及依赖

1. **一条无测试注入的代表性体验脚本与内容选择。** 先盘活现有入口，定义玩家应观察到的因果，不先做系统。
2. **同一具体人物的变化可读。** 依赖 1 的人物/出征选择，复用 PersonId、Detail、成长与 LifeHistory；讨论哪些变化才有意义。
3. **一次过去被再次引用的社会因果。** 依赖 1、2 的事件/人物选择，复用 Event 条件、Social Fact/反馈；需先确定引用方式与设计语义，避免另造 Memory Authority。
4. **Return 双侧变化汇合和下一轮动机。** 依赖 2、3 及既有 Home/Away 时钟/Return；先确认现有反馈够不够，不预先决定新 Summary UI。
5. **Battle 本身乐趣与整轮摩擦。** 用上述链评估风险/构筑/节奏及粮食准备；是产品证据缺口，未证实必须改战斗或经济规则。

推荐顺序：先解决 V17 完整本地依赖快照→制作人与 AI 游戏设计顾问确定 1 的体验和证据→选 2/3 的既有内容与表达→串 4→试玩评估 5 和四种动机→只对证实缺口另立获批 DevelopmentSpec。没有可靠依据给工期，也不建立 V18 GameSpec。

## 待讨论而非本轮决定

- Milestone A 从短 Away 还是 Home 开始？要让玩家记住哪几个具体人物？
- 哪个现有 Project 足以产生主动 Build/Care 动机？完成变化和机会成本如何被理解？
- 哪种过去处理被谁、以什么可见方式再次引用？要证明社会连续性还是设计新的治理规则？
- 哪些 Return 信息必须主动呈现，哪些让玩家查看？允许多久不回村、多久不出征？
- 什么 Battle/Build 决策算“值得玩”？默认粮食/Carry 是否产生准备劳动？
- 内部体验目标平台、单次时长、存档中断边界与成功判据是什么？需要 Player 才另行授权构建。

## 复用入口与非目标

复用 Person/Growth、VillageTime/WorldAdvance、Assignment/Project/Building、Resource、Event/Social、Expedition/Return、Battle、Save 和现有 Home UI；入口分别在 `GameSpec/Person`、`Battle`、`Expedition`、`Village`、`Save`，架构查 `Docs/Architecture/技术架构总览.md`。原始 Event FDS、WP-AL-3/4 Checkpoints、R3-P、V17 Execution Records 保留为设计/实现边界，不因类名或旧编号自动派发任务。

不提前开发完整职业生态、事件库/地图、经济/物流、美术量产、全人物社会/历史、Law/Policy、Population Renewal、NavMesh、完整跨进程 Battle Resume。没有为规划修改现有设计权威。本次仅新增本报告；完成 Phase 1 状态交接与 Phase 2 报告后停止。
