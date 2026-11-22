> **2026-11-06 FP 正常路径实际核验**：V17保持LOCAL DEVELOPMENT CLOSED。正式鼠标路径已到NewGame→选英雄→Home（未经过Tutorial），Home→Prep在Food0/实际Carry0/首步成本2处无法开始；独立设置保存返回→Continue→Home可用。G7规定链未走通，G6两次远征循环未验证，不判FP通过；未将补给守卫判为Bug，未改初始库存/代码。用户原档SHA原子归位一致，字体门禁0/0。详见仓库 Docs/Roadmap/RRPG_FP-V0.1_Normal-Player-Path_Verification_2026-11-06.md 与 Development/StudioRuns/FP-NormalPath-20261106/steps.json。旧整合文档V17 Blocked为历史记录，不覆盖已关闭事实。本轮仅报告/证据/引用，无Commit/Push/Merge，完成后停止。
# RRPG FP-V0.1 产品设计与证据整合

日期：2026-11-06。Target Branch：`ai/rrpg-r3b-population-pilot`；读取基点 HEAD：`4d9ea21`。

本报告是产品设计的阅读入口与证据整合，不是新 GameSpec、V18 立项、玩法修改或开发授权。原方案应继续沿“历史设计起源 → 当前正式规则 → 实现/技术证据 → 对应范围产品验收”理解。已有系统基础相当完整，缺少的是同一正常玩家路径下可重复循环和产品价值的完整对应证据，不能据本次调查写 Milestone A 或 FP-V0.1 已完成。

逐文件候选清单、历史 commit:path、原始7 Gate / 9问题 / 5结构 / 4动机 / 5 Anti-Friction / 5问 / 14操作项见[证据索引](RRPG_FP-V0.1_Evidence-Index_2026-11-06.md)。索引明确区分深读与关键词定位；未读取的全量回执、远端与仓库外讨论不是已覆盖证据。

## 1. 产品起源、目标与命名

Current Design：[产品定位](../../GameDesign/01%20产品定位与设计支柱.md)、[核心循环](../../GameDesign/02%20核心循环与系统关系.md) §2.1–2.7。RRPG 是单人长期共同体经营与远征式RPG卡牌构筑；Person 是长期对象，Village 是共同体空间，Battle / Run 提供独立值得沉浸的构筑与风险决策。长期成长扩大人生、结构与组合可能性，不能只归结为数值增长。

| 验收层 | 来源 | 用途及关系 |
|---|---|---|
| 五项高层产品问题 | Village GameSpec §6.8；历史9/2 Brief §14 | 人物感、抽调犹豫、长期回应、选人身份与回家动机，属于高层Product Acceptance。 |
| 四项核心价值 | [Milestone Definition](RRPG_Milestone_A_Definition_v0.1.md) Core Validation | Person Continuity / Expedition Value / Return Value / Village Meaning；提炼价值，不删除五问。 |
| 九项体验诊断 | 9/3 Integrated Concept §41 | Commitment / Person Recognition / Build / Shape / Home / Away / Return / Re-engagement / Dual-Core Freedom。 |
| 七项功能Gate | 9/7 FP Checkpoint §3 | 完整可持续功能原型；G7最终集成验收明确不等于好玩验证，G1还包括玩家理解。 |
| 十四项操作清单 | Village GameSpec §6.8 | 为五问提供操作证据，不是第二套平行Milestone定义。 |

9/7 FP Checkpoint §1为避免和历史Battle Milestone A–D混淆而使用FP-V0.1；9/8 Definition仍保留Milestone A的产品验证目标。这里并列维护“功能原型目标”和“核心价值验证”，不擅自裁定它们完全同义、相互取代或新的开发编号。

验证策略来自历史Core Validation Plan §2、§9–12（索引H3）：Technical Proof → Internal Product Proof → Small Comprehension Check → Vertical Slice Playtest → Public Demo/Market Test。制作人内部体验可以证伪因果/摩擦，不能代替市场验证；当前不需要因为证据整合而立刻组织外部测试。

## 2. 设计演化与有效性裁决

| 时间与来源 | 原贡献 | 今天的读取方式 |
|---|---|---|
| 8/31 Village初版 §6.8（H1） | 14项操作链 | 当时第5项Trend变化；当前已是长期事实→Pattern/Recognition，不能恢复旧Trend模型。 |
| 9/2 Base Trunk Brief（H2） | Base DNA、Milestone Closure、Base Completeness、Candidate、Polish分层；五问与V2→V3→Minimum World→V4路线 | Brief提供起源，不直接冻结实现。当前批准原则核对GameDesign当前状态与各GameSpec。 |
| 9/3七MD+一DOCX（H5） | V3、V4、Minimum World、Base Completeness、Integrated Concept、Validation Plan、Audit Map | 8文件共5506行插入来自7MD，不把历史Audit Map当Current。V2用Village§4和9/3日志追溯，无独立V2 Checkpoint的已读证据。 |
| 9/4 [Long-term Build](../DesignCheckpoints/2026-09-04_RRPG_Village-Long-term-Build_Concept-Checkpoint.md)、9/6 [Future Envelope](../DesignCheckpoints/2026-09-06_RRPG_Village-Future-Design-Envelope_Concept-Checkpoint-V0.1.md) | 长期Village方向与最小切片 | 包络不等于首版必做清单；正文未逐条裁定实现状态。 |
| 9/7 [FP概念](../DesignCheckpoints/2026-09-07_RRPG_Functional-Prototype-V0.1_Concept-Checkpoint.md) §4–11、20–28 | Battle-first，第一远征教Battle，第二远征教Expedition；稳定村庄慢构筑 | 已在核心循环§2.7归位，不能退回“开场是否Battle-first完全空白”；具体剧情、同行人数和教学内容仍未冻结。 |
| 9/7 [Cross-document Closure](../DesignCheckpoints/2026-09-07_RRPG_FP-V0.1_Cross-Document-Closure-Checkpoint.md) §4 | Food映射、Return顺序、日初快照/日末释放、整数天 | 历史裁决；Return保持Occupancy→Catch-up→Carry回流→Clear→唯一Save的语义，具体当前细时间规则由当前GameSpec决定。 |
| 9/18–19 当前Expedition设计 | Journey Presentation、6 Character + optional Hero | Journey不改变Node规则；远征携带人数不等于单场战斗人数。来源当前设计状态§二、Expedition GameSpec。 |
| 9/26 Event-1，10月AL及日内A–D | 正式事件基础、关系反馈、持续活动与分钟时间 | 已实现子范围不代表完整Recognition/LifeEpisode；9/7“不做半天/小时”不能覆盖后续分钟日程。 |
| 10/7 V16；11/5 Tavern；11/6 V17收尾 | 真实Home/建筑/管理/资源/存档及固定Place | V17当前PRODUCT ACCEPTED；LOCAL CLOSEOUT BLOCKED独立保留，不把产品验收扩大为Milestone五问。 |

历史“Farm→Food”已被Ingredients→Kitchen→Food链替代；早期Food只是Candidate的措辞不能覆盖当前最小资源循环。历史Person-specific Trait也不能覆盖当前Profession Trait规则，参见[当前设计状态](../../GameDesign/当前设计状态.md) §三、四。

## 3. 完整体验与系统职责

常态循环为Home → Prep → Expedition Progression → Battle/Situation/Reward → Return → Home → Continue；开场可以先Tutorial Expedition再揭示Home（当前方向Battle-first）。第一远征强调操作、Connection、奖励与后续新牌；第一次Home建立“同行者也在这里生活”；第二远征教授正式组队、职责机会成本、Food/Supply、分支、时间、风险与Return/Retreat。新剧情、人数、成本值不能由这份整合冻结。

V2 Commitment表达具体人物时间的机会成本；V3 Social Response让过去安排经合理感知成为回应与真实后果；Minimum World让家是能识别人与Place的地方；V4让离家期间Home继续运行，Return汇合Home和Away历史。Return的产品语言为Remember → Reorient → Re-engage，而非行政重置（Integrated Concept §22–30）。

PersonId跨域稳定；Assignment不等于Profession；Battle Card / Unit不等于长期Person。领域拥有事实，History负责记忆，Event判断当前注意/介入价值，消费者各自解释；不建立万能History Interpreter或第二份Person真相。来源[核心循环](../../GameDesign/02%20核心循环与系统关系.md) §2.5、[Person规格](../../GameSpec/Person/Person-Profession-Growth-GameSpec-V0.1.md)、Village §12。

双核心连接来自人物、历史、机会；玩家短中期偏向某侧的自由是产品目标，不应据此推断已有无限离线模拟、派遣系统或所有持续性问题已解决。Home与Battle各自有决策，同时彼此改变意义（Integrated Concept §13–21；H3 §7）。

## 4. 当前证据台账与支持边界

以下源码核对表示本轮静态阅读；Technical Verified与Product Accepted均指明确历史记录，不是本轮运行结果。

| ID | 证据定位 | 可支持 | 不能支持 |
|---|---|---|---|
| E1 | [V3C测试](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/Village/VillageV3CProductChainPlayTests.cs) L178、233–340；[项目日志](../项目日志.md) 2026-09-10 V3C | 重复承诺→Recognition→配置Event→GrantRest选择→可见恢复窗口/History→Save/Load→后续Project调度。历史Product Accepted限定本包，内容Test/Not Frozen。 | 自然游玩中所有社会因果可读；完整正式事件库或LifeEpisode。 |
| E2 | [V4完整链](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/VillageV4/VillageV4FullProductChainPlayTests.cs) L296–460 | Test Project/Capability扩大同一人物池；真实Expedition、Supply、Return/Catch-up、人物后果、Save/Load后候选查询。 | L315手动TickDay，L397注入HP；Return是Retreat；Phase D只查询Growth/Reward，不执行第二次完整Expedition，也不是清单10 SafeReturn的单独完整证明。 |
| E3 | [Battle6B](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/BattleStage6BRewardLoopPlayTests.cs) RunRewardLoop、Battle2断言；项目日志2026-08-30 Stage6B | 战斗胜利触发奖励、按钮选择/Skip、同一Person新牌进入第二战SharedDeck。日志历史人工验收记跨战抽新牌。 | 测试以9999伤害结束敌方，第二战仅InitForBattle检查DrawPile；未验证玩家理解、新牌实际打出、当前无Debug完整教程或Battle乐趣。 |
| E4 | [SocialFeedback集成](../../UnityProject/RRPG1.0game/Assets/Tests/EditMode/AutonomousLife/SocialFeedback/SocialFeedbackIntegrationTests.cs) H_HistoricalReciprocityRealSaveLoadControlWorlds L229起；[AL4 Closeout](../DevelopmentSpecs/2026-10-05_RRPG_WP-AL-4_Social-Feedback_Closeout.md) | 过去互动经关系提高未来Help候选Score/Weight，Save/Load保持；不重复计入Raw Fact。历史Technical Verified/Published记录按该版本读取。 | 权重提高不等于必然选择；不是完整Recognition/Event/LifeEpisode后续表现。 |
| E5 | [V17本地收尾](../DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Report.md) 结论、R3-B处置；[依赖收尾](../DevelopmentSpecs/2026-11-06_RRPG_V17_Dependency-Closeout.md) | 五项综合产品验收与Tavern验收成立；产品已验收无需重验。 | 不证明整体当前dirty统一回归，不证明HEAD可独立重建，不等于G6/G7或Milestone产品五问。 |
| E6 | 项目日志2026-09-15 V4 Objective Functional Acceptance（L569附近）、2026-09-10 Minimum World（L607附近） | V4功能验收明确通过；Minimum World真实观察人物/Project完成痕迹/Save-Continue有历史人工验收。 | V4记录明确Milestone五问Deferred，不能覆盖写成已通过。 |

## 5. 七Gate交叉审计

“部分证据”表示未形成完整Gate结论，不是功能一定缺失。G编号只属于9/7 FP Checkpoint。

| Gate | 相关Checklist / 价值 | 已有依据 | 当前结论与缺口 |
|---|---|---|---|
| G1 Battle Core | O9；Expedition Value | E3及历史Battle产品验收 | 部分证据；玩家理解、Connection、新牌后续实际使用、非Debug完整链需对应回执。 |
| G2 Expedition Core | O8–11；Expedition Value | 当前Expedition规则、E2、日内Return历史 | 部分证据；组队/Supply/Node/Branch/退出类别需同一正常体验证明，不以Retreat覆盖SafeReturn/Failure。 |
| G3 Person Continuity | O2/8/11/13/14；Person Continuity | E2/E6、Person规格、V17人物验收 | 稳定身份有技术/局部产品证据，尚不足把长期人物感整体验收写PASS。 |
| G4 Home Exists | O1–4/12；Village Meaning | E5/E6、Product Home/Building规则 | Home真实存在有强局部产品证据；不等于完整Milestone Home意义。 |
| G5 Battle↔Village | O3/8/10–14；Return Value | E2及资源/Return既有链 | 部分技术链成立，当前默认内容综合产品对应仍未完整确认。 |
| G6 Loop Sustainability | O14加“第二次Return仍可继续” | E2只到Load后候选读取 | 未取得完整证据；必须实际再Prep、再Expedition、再Return且可继续，不人工改数据。 |
| G7 End-to-End Path | O1–14整体；四价值 | 分段E1–E6 | 未取得完整同版本链；NewGame→Tutorial→Home→正式Prep→Expedition→Return→Save/Load→Continue。它验证集成，不自动验证好玩。 |

## 6. 十四操作项逐项对照

原文完整保留于证据索引O；这里用短名定位，不能以合并单元格漏掉任何一项。

| O | 操作要求 | Gate | 证据与仍需辨别的范围 |
|---|---|---|---|
| 1 | 时间/人物持续运行 | G4/5 | 日内A–D、E5；非本轮统一运行。 |
| 2 | 安排人物 | G3/4 | V2日志、E5管理验收。 |
| 3 | Assignment / Project | G4/5 | V2、E2；Test Project不证明动机。 |
| 4 | 时间推进 | G4/5 | E2 TickDay与日内历史；手动推进不是自然节奏。 |
| 5 | 事实经Pattern/Recognition形成回应 | G3/4/5 | E1专项；不能用AL4权重代替完整社会回应。 |
| 6 | Village Event出现 | G4/5 | E1、Event-1历史基础。 |
| 7 | Choice有可见后果 | G4/5 | E1恢复窗口；整体因果可读仍待产品链。 |
| 8 | 合法选人出征/Project互斥 | G2/3/5 | V2验收、E2；不要求手动取消普通Assignment。 |
| 9 | Existing Battle / Run | G1/2 | E3、Battle历史；不证明值得玩。 |
| 10 | SafeReturn | G2/5 | Return专项历史入口；E2的Retreat不能独自替代SafeReturn覆盖。 |
| 11 | HP/Exp/Pending回流 | G3/5 | E2注入伤病，9/15 EXP修复日志及对应Person规则；不同收益规则不能泛化。 |
| 12 | Village状态变化 | G4/5 | E2、E5/E6；玩家是否注意到变化另判断。 |
| 13 | Capability改变Unlocked Pool/后续Run | G3/5 | E2 Phase A，Test Capability范围。 |
| 14 | 下一远征实际读取变化 | G3/5/6 | E2证明候选读取，未完整进行下一远征；O14不等于G6已经通过。 |

## 7. 产品问题、结构证据、动机和摩擦

九体验问题逐项保留，相关映射为审计组织方式，不是原设计规定的一对一关系。

| 体验问题 | 相关五问/价值 | 已有与产品证据缺口 |
|---|---|---|
| Commitment | 五问2；Village Meaning | 占用/恢复成立；抽调是否真的犹豫尚无整体验收。 |
| Person Recognition | 五问1/4；Person Continuity | 稳定身份/详情可读；为何记住人需产品体验。 |
| Build | Village Meaning | Project/建筑变化存在；主动想推动Project尚未被技术证明。 |
| Shape | 五问3；Village Meaning | E1社会后果和E4关系反馈；正常内容再次引用过去的体验不足。 |
| Home | 五问1/5；Village Meaning | E5/E6世界验收；“家”的连续意义仍需整合体验。 |
| Away | Expedition Value | 后台推进与Return机制；专注Battle不被打断需体验记录。 |
| Return | 五问5；Return Value | 时间/状态回流有证据；信息不过载与公平性未整体确认。 |
| Re-engagement | 五问5；Return/Village | Load后候选存在；自然产生下一决定不是API查询。 |
| Dual-Core Freedom | 全部价值 | 不强制切换方向保留；不宣称长时间偏玩两边均已验证。 |

五条不可省结构证据（Integrated §45）：连续Person（E2/E6有技术链）、改变世界Project（E6有可见持久痕迹）、引用过去Social Response（E1有限专项）、持续Home（E5及日内技术历史）、值得玩Battle Run（E3不证明乐趣）。它们提供结构与价值证据，不能只按“类存在”全部勾选。

四Village动机种子（Integrated §42）：Build=想改变家园结构；Care=关心具体人物；Shape=过去决定改变共同体回应；Discover=想看下一人物/世界问题与新可能。当前Build有功能、Care有人物入口、Shape有E1/E4、Discover有Expedition与成长入口；均不足从组件存在推导动机体验已成立。完整Theme/Institution/人物生态不是补齐这些种子的前置。

五Anti-Friction（Integrated §43）：Return不是重新办公（检查自动恢复/待办量）；出征成本不是排班劳动（检查准备步骤和粮食等待）；世界回应不能持续打断（检查pending/Modal频率）；具身不成为跑腿税（世界选择与集中管理兼容）；历史不永久锁死未来（事实保留与决策可改变）。本轮未试玩，五项均保留风险判断，不能写体验PASS。H3 §5的10–20秒回家理解测试是历史低成本证伪方法，不是新UI性能指标。

## 8. 未完成分类、Readiness补充与执行顺序

对既有[Readiness Audit](RRPG_Milestone_A_Readiness_Audit_2026-11-06.md)的补充：来源需加H1–H6、FP与Cross-doc两Checkpoint，保留五问/九诊断/七Gate/十四操作层级。Battle-first已经在Current Design §2.7归位，旧报告“从Home还是Away开始”应收窄为具体教程与正式内容选择，不重新讨论已采用方向。V4功能验收已明确成立且五问Deferred；不能把V4重造为新系统。当前分钟时间取代9/7历史整数天限制；材料链与Trait读取当前规则。原报告的整体“产品证据不足”结论仍成立，但应以上述更精确覆盖与支持边界理解。

| 分类 | 当前保留项 | 后续动作 |
|---|---|---|
| 已确认方向，完整产品链尚无证据 | Battle-first双教学职责、G6/G7可持续循环 | 下一工作包先映射实际入口和选定内容，不直接从本报告生成代码。 |
| 已实现/局部验证，产品价值未完整验证 | 人物长期关心、承诺重量、回家动机、双核心自由、Return摩擦、Battle乐趣 | 制作人内部完整体验，沿原五问/九诊断记录，而非追加大系统。 |
| 未冻结 | 具体剧情/人物组合、正式Event/Project内容、正式数值、最终美术/镜头及体验时长 | 仅遇到代表链必要依赖时再作精确选择；本轮无新裁决。 |
| 明确Deferred | 完整Formation/社会历史表现、Law/Policy、Population Renewal、Campaign终局、物流、多章节、完整Battle/Expedition跨进程Resume | 不回填进首版必做清单；AL已有子系统不自动扩大原型范围。 |
| 独立工程阻塞 | V17完整本地快照与Compiler/测试基线/Prefab绑定证明 | 保留LOCAL CLOSEOUT BLOCKED；本轮不扩大Tools提交边界、不提交不完整Runtime。 |

执行顺序：本轮先完成设计证据整合与静态审阅；主代理检查来源、覆盖和保护SHA。后续代表性体验应在可复核的完整工程快照上完成G6/G7，再按五问与九诊断判断产品价值。只有确认真实缺口后才为必要改动编写对应DevelopmentSpec。暂时不新增讨论题，不提前估工期、完成百分比或请求重新验收V17。

本轮验证为文档/历史/代表性源码静态核对、原文条目数与链接检查；未运行引擎、Builder或测试。两份文档仅本地新增、未暂存、未提交、未Push；不宣称整体开发关闭。完整覆盖范围和未读边界见证据索引，本报告不承诺恢复仓库外全部历史对话。
