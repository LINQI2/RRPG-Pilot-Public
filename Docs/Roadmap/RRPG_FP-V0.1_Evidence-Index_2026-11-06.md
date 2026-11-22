> **2026-11-06 FP 正常路径实际核验**：V17保持LOCAL DEVELOPMENT CLOSED。正式鼠标路径已到NewGame→选英雄→Home（未经过Tutorial），Home→Prep在Food0/实际Carry0/首步成本2处无法开始；独立设置保存返回→Continue→Home可用。G7规定链未走通，G6两次远征循环未验证，不判FP通过；未将补给守卫判为Bug，未改初始库存/代码。用户原档SHA原子归位一致，字体门禁0/0。详见仓库 Docs/Roadmap/RRPG_FP-V0.1_Normal-Player-Path_Verification_2026-11-06.md 与 Development/StudioRuns/FP-NormalPath-20261106/steps.json。旧整合文档V17 Blocked为历史记录，不覆盖已关闭事实。本轮仅报告/证据/引用，无Commit/Push/Merge，完成后停止。
# FP-V0.1 Evidence Index

日期：2026-11-06。Target Branch：ai/rrpg-r3b-population-pilot；读取基点 HEAD：4d9ea21。

## 检索范围与证据等级

先枚举当前 tracked/untracked 非 ignored 路径，再按产品设计、规则、Checkpoint、Roadmap、开发规格与代表性产品测试建立候选集。下方逐文件目录是候选覆盖清单，不等于所有正文逐字审阅。明确深读的关键源见后续摘录、历史索引与主报告证据表；其余仅目录/关键词定位，不将其内容当成已证实结论。场景、Prefab、生成SO、字体、Tools、配置二进制不做本轮绑定/重建验证。Development/StudioRuns 的全量日志与截图未逐份核读；已有Closeout对这些证据的引用只能按记录强度使用。未访问远端，不断言发布状态。

标签：Current Rule=领域正式规则；Confirmed Architecture=已批准职责；Concept=概念证据；Implemented=代码存在；Technical Verified=已有明确版本技术回执；Product Accepted=对应制作人验收；Deferred=明确后置。当前代码存在不能自动加后两标签。

## 历史证据（已读取）

| ID | Git定位 | 阅读与用途 |
|---|---|---|
| H1 | e3b8dec:GameSpec/Village/Village-GameSpec-V0.1.md §6.8 | 8/31最初14操作项；第5项当时为Trend变化，当前已改为Pattern/Recognition，不复活旧Trend Runtime。 |
| H2 | 6183990:Docs/DesignCheckpoints/2026-09-02_Base-Trunk_Person-Life_Architecture_Design-Brief_V0.1.docx | 已从Git二进制通过ZIP XML提取正文，§4五层边界、§8事实/解释职责、§14五问、§15路线；是Brief而非已实现声明。 |
| H3 | 6183990:Docs/DesignCheckpoints/2026-09-03_RRPG-Core-Validation-Plan-V0.1.md | 全文；§2验证五层、§3 V2产品风险、§4 V3因果、§5 Return摩擦、§9–12内测与外测。 |
| H4 | 6183990:Docs/DesignCheckpoints/2026-09-03_RRPG-Current-Design-Audit-Map-V0.1.md | 全文；历史战略盘面，非Current Truth。Food/Profession等历史候选不能覆盖后续已归位规则。 |
| H5 | 6183990 --stat | 8个文件=7个Markdown+1个二进制DOCX；5506插入行来自7个Markdown，不能称8份Markdown共5506行。 |
| H6 | Docs/项目日志.md 2026-09-03 V2（L855起）及 e9f05c4 / 387495b | V2原始实施/验收追溯入口；日志已读，提交对象只定位，未逐diff复核。未找到独立命名V2 Checkpoint，不能虚构。 |

Git历史源使用 commit:path，故意不伪装成当前可点击文件。6183990中的另五份Markdown仍有当前同名Checkpoint；以当前文件正文作设计起源阅读，以当前GameSpec裁决规则。未遍历所有分支每次提交的全文，未纳入仓库外不可达聊天/个人归档，因此不宣称绝对无遗漏。

## 已核对原始验收文本

以下为仓库自有设计资料的原文摘录，只用于防遗漏；不是将Checkpoint升级为规则。

### F：七项Completion Gates（源 Docs/DesignCheckpoints/2026-09-07_RRPG_Functional-Prototype-V0.1_Concept-Checkpoint.md §3）

## 3. FP-V0.1 Completion Gates

### G1｜Battle Core
必须证明玩家能够理解并完成当前核心 Battle：
- 基础出牌；
- Connection；
- Battle Reward；
- 新牌在后续 Battle 中实际使用；
- 不依赖 Debug 入口。

### G2｜Expedition Core
必须证明 Expedition 不只是连续 Battle：
- 正式组队；
- 携带当前远征所需 Supply；
- 推进产生 Time + Supply Cost；
- 统一 Node Progression；
- 多后继才出现 Branch Choice；
- Return / Retreat 为 Expedition 层独立退出；
- Safe Return / Retreat / Failure 可区分。

### G3｜Person Continuity
- Home 选中的 Person 进入 Expedition；
- Battle / Run 使用同一 PersonId；
- Return 后仍是同一 Person；
- 长期状态、成长与 Return 结果连续；
- 不创建第二套长期人物真相源。

### G4｜Home Exists
- 存在真实 Village World；
- 能识别具体 Person；
- 能识别基础 Place；
- Person 有最低世界行为表现；
- 玩家能通过 World 理解人物与职责；
- Home 不只是结算背景。

### G5｜Battle ↔ Village Coupling
- Expedition 能读取 Village 的具体 Person；
- Expedition 能从 Village 取得长期资源 / Supply；
- Expedition 会产生时间、人物状态、收益或其它长期变量；
- Return 能把这些结果带回 Village；
- Village 能承接这些变量。
- 不要求为了验收主动制造 Village 危机。

### G6｜Loop Sustainability
- 第一次 Return 后可以继续 Prep；
- 可以再次 Expedition；
- 第二次 Return 后仍可继续；
- 核心循环可重复；
- 不需要人工修改数据才能进入下一轮。

### G7｜End-to-End Product Path
最终至少应覆盖：

> New Game  
> → Tutorial Expedition  
> → Return Home  
> → Formal Expedition Prep  
> → Expedition  
> → Return  
> → Save / Load  
> → Continue

该 Gate 是最终产品集成验收，不等于“好玩验证”。

---

### X：九体验问题、四动机、五条Anti-Friction（源 Docs/DesignCheckpoints/2026-09-03_Playable-Milestone-A_Integrated-Concept-Checkpoint-V0.1.md §41–43）

## 41. Milestone A 的核心验证问题

Milestone A 首先是：

> **Internal Vertical Slice**

而不是公开试玩版本。

必须回答：

### Commitment
> 抽走一个具体 Person，我会不会感觉自己真的牺牲了什么？

### Person Recognition
> 我会不会记住具体 Person，而不只记住数值最强的人？

### Build
> 我有没有主动想推动一个会改变 Village 的 Project？

### Shape
> 世界有没有重新引用我过去的处理方式，让我感觉共同体拥有历史？

### Home
> Village 是否已经从管理后台变成一个“地方”？

### Away
> 我能否专注玩 Battle，而不需要同时管理 Village？

### Return
> 我回来时是否感觉世界没有等我，同时又没有被信息和行政工作淹没？

### Re-engagement
> Return 后我是否自然产生下一步想法？

### Dual-Core Freedom
> 我是否可以在短中期偏向 Village 或 Battle，而不被硬性惩罚或强制切换？

---

## 42. Milestone A 的四个 Village 动机种子

Milestone A 至少应出现：

- **Build Seed**
- **Care Seed**
- **Shape Seed**
- **Discover Seed**

不要求每个都达到长期完整版。

---

## 43. Milestone A 的五条 Anti-Friction

### 1. Return ≠ Administrative Reset
回家不是重新办公。

### 2. Expedition Cost ≠ Preparation Chore
出征有机会成本，但准备不能变排班劳动。

### 3. Reactive World ≠ Interruptive World
世界可以持续回应，但不能持续打断。

### 4. Embodiment ≠ Traversal Tax
具身感不能转化成强制跑腿。

### 5. History ≠ Permanent Constraint
过去有重量，但玩家仍应有改变方向的自由。

---

### S：五条结构证据（源 Docs/DesignCheckpoints/2026-09-03_Playable-Milestone-A_Integrated-Concept-Checkpoint-V0.1.md §45）

## 45. Milestone A 看似小但不能省的结构证据

### 一个真正连续的 Person
同一个人：
> Village生活 → 被安排 → 出征 → 经历 → 回来。

### 一个真正改变世界的 Project
证明 Build Seed。

### 一个真正引用过去的 Social Response
证明 Shape Seed。

### 一个真正持续运行的 Home
证明 V4 Continuity。

### 一个真正值得玩的 Battle Run
证明 Away 不是资源结算工具。

---

### O：五问与十四项Checklist（源 GameSpec/Village/Village-GameSpec-V0.1.md §6.8）

## 6.8 V4 Playable Milestone A

### 高层 Product Acceptance｜已批准五问

1. 我有没有开始把村民当成具体人，而不是 Worker？
2. 抽走一个具体 Person 去做别的事情时，我会不会产生真实犹豫？
3. 长期安排有没有让 Village 产生可感知的不同反应？
4. 选择六人远征时，我会不会考虑“他们是谁”，而不仅是战斗数值？
5. 远征回来以后，我会不会想看看家里和这些人又发生了什么？

### Operational Checklist

以下操作清单仅用于验证上述五问，不构成第二套平行 Milestone A 定义：

人工体验至少覆盖：

1. Village 时间与人物持续运行；
2. 玩家安排人物；
3. 至少一个 Assignment / Project；
4. 时间推进；
5. 长期事实经 Pattern / Recognition 形成可感知 Social Response；
6. Village Event 出现；
7. Choice 产生可见后果；
8. 玩家选择符合 Temporary Occupancy 条件的人物出征；Active Project 占用者不得同时出征；
9. Existing Battle / Run 正常进行；
10. SafeReturn；
11. HP / Exp / Pending 收益正确回流；
12. Village 状态变化；
13. Capability 至少一次改变 Unlocked Pool / 后续 Run 可能性；
14. 下一次远征能够实际读取该变化。

完成 Operational Checklist，并以五问完成高层 Product Acceptance 后：
**Village Playable Milestone A**

---

## 当前候选逐文件目录

共 118 条；“候选”表示已枚举路径，深读范围见上文和主报告。无关领域（如美术、配置架构、敌方资产）只作防漏定位，不自动纳入本轮产品要求。

- [Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md](../../Docs/DevelopmentSpecs/2026-10-24_RRPG_V17-R3-B_Person-Management_Execution-Record.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Initial-World_Product-Experience-Fixes_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Initial-World_Product-Experience-Fixes_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-03_Base-Completeness-Addendum_Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-03_Base-Completeness-Addendum_Concept-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-03_Minimum-Village-World-Integration_Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-03_Minimum-Village-World-Integration_Concept-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-03_Playable-Milestone-A_Integrated-Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-03_Playable-Milestone-A_Integrated-Concept-Checkpoint-V0.1.md) — 已读产品循环/核心诊断/边界，长输出部分截断；§41–45原文独立提取
- [Docs/DesignCheckpoints/2026-09-03_V3-Social-Response_Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-03_V3-Social-Response_Concept-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-03_V4-Home-Away-Continuity_Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-03_V4-Home-Away-Continuity_Concept-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-04_RRPG_Village-Long-term-Build_Concept-Checkpoint.md](../../Docs/DesignCheckpoints/2026-09-04_RRPG_Village-Long-term-Build_Concept-Checkpoint.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-06_RRPG_Village-Future-Design-Envelope_Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-06_RRPG_Village-Future-Design-Envelope_Concept-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-07_RRPG_FP-V0.1_Cross-Document-Closure-Checkpoint.md](../../Docs/DesignCheckpoints/2026-09-07_RRPG_FP-V0.1_Cross-Document-Closure-Checkpoint.md) — 已读：§3–4 Authority与裁决
- [Docs/DesignCheckpoints/2026-09-07_RRPG_Functional-Prototype-V0.1_Concept-Checkpoint.md](../../Docs/DesignCheckpoints/2026-09-07_RRPG_Functional-Prototype-V0.1_Concept-Checkpoint.md) — 已读全文
- [Docs/DesignCheckpoints/2026-09-16_RRPG_Identity-Content-Design_Concept-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-16_RRPG_Identity-Content-Design_Concept-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-09-20_RRPG_Configuration-Architecture_Replan-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-09-20_RRPG_Configuration-Architecture_Replan-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-10-02_RRPG_History-Autonomous-Life_Architecture-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-10-02_RRPG_History-Autonomous-Life_Architecture-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-10-04_RRPG_Autonomous-Life-Vertical-Slice_Product-Spec-V0.2.md](../../Docs/DesignCheckpoints/2026-10-04_RRPG_Autonomous-Life-Vertical-Slice_Product-Spec-V0.2.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-10-04_RRPG_WP-AL-3_Autonomous-Selection_Design-Checkpoint-V0.2-Final.md](../../Docs/DesignCheckpoints/2026-10-04_RRPG_WP-AL-3_Autonomous-Selection_Design-Checkpoint-V0.2-Final.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-10-04_RRPG_WP-AL-4_Social-Feedback_Design-Checkpoint-V0.1-Final.md](../../Docs/DesignCheckpoints/2026-10-04_RRPG_WP-AL-4_Social-Feedback_Design-Checkpoint-V0.1-Final.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-10-07_RRPG_Village-Visual-Interaction-and-Building_Architecture-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-10-07_RRPG_Village-Visual-Interaction-and-Building_Architecture-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/2026-11-05_RRPG_Village-Fixed-Place-Presentation_Design-Checkpoint-V0.1.md](../../Docs/DesignCheckpoints/2026-11-05_RRPG_Village-Fixed-Place-Presentation_Design-Checkpoint-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DesignCheckpoints/README.md](../../Docs/DesignCheckpoints/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-07_RRPG_WP-1_Expedition-Progression_Final-Development-Spec.md](../../Docs/DevelopmentSpecs/2026-09-07_RRPG_WP-1_Expedition-Progression_Final-Development-Spec.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-17_RRPG_3P-Shared-Deck_Gate-0_Development-Spec.md](../../Docs/DevelopmentSpecs/2026-09-17_RRPG_3P-Shared-Deck_Gate-0_Development-Spec.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-A_Battle-Dual-Board-Spatial-Authority-Migration_Development-Spec.md](../../Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-A_Battle-Dual-Board-Spatial-Authority-Migration_Development-Spec.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-B_Expedition-Return-Integrity_Development-Spec.md](../../Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-B_Expedition-Return-Integrity_Development-Spec.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-C_Battle-Legacy-Product-Path-Cleanup_Development-Spec_FINAL.md](../../Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-C_Battle-Legacy-Product-Path-Cleanup_Development-Spec_FINAL.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-D_Battle-Visual-Foundation-V0.1_Development-Spec.md](../../Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-D_Battle-Visual-Foundation-V0.1_Development-Spec.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-E_Expedition-Journey-Visual-Foundation-V0.1_Development-Spec.md](../../Docs/DevelopmentSpecs/2026-09-18_RRPG_WP-E_Expedition-Journey-Visual-Foundation-V0.1_Development-Spec.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-19_RRPG_Journey-Event-Illustration-Binding-V0.1_Manual-Configuration-Guide.md](../../Docs/DevelopmentSpecs/2026-09-19_RRPG_Journey-Event-Illustration-Binding-V0.1_Manual-Configuration-Guide.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-19_RRPG_Person-Visual-Binding-V0.1_Manual-Configuration-Guide.md](../../Docs/DevelopmentSpecs/2026-09-19_RRPG_Person-Visual-Binding-V0.1_Manual-Configuration-Guide.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-20_RRPG_美术批量导入与Art-Import-Skill规划_V0.1.md](../../Docs/DevelopmentSpecs/2026-09-20_RRPG_美术批量导入与Art-Import-Skill规划_V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-20_RRPG_美术素材配置操作手册_V0.1.md](../../Docs/DevelopmentSpecs/2026-09-20_RRPG_美术素材配置操作手册_V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-22_RRPG_M2-Monster-Definition-Cutover-Pre-Implementation-Audit-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-22_RRPG_M2-Monster-Definition-Cutover-Pre-Implementation-Audit-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-22_RRPG_Static-Configuration-Migration-FDS-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-22_RRPG_Static-Configuration-Migration-FDS-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-23_RRPG_M2-Monster-Definition-Authority-Cutover-Closeout-Report-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-23_RRPG_M2-Monster-Definition-Authority-Cutover-Closeout-Report-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-23_RRPG_M3-Enemy-Presentation-Authority-Cutover-Closeout-Report-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-23_RRPG_M3-Enemy-Presentation-Authority-Cutover-Closeout-Report-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-23_RRPG_M4-Monster-Legacy-Physical-Retirement-Closeout-Report-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-23_RRPG_M4-Monster-Legacy-Physical-Retirement-Closeout-Report-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-25_RRPG_WP-Encounter-1-Enemy-Encounter-Generation-Foundation-Closeout-Report-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-25_RRPG_WP-Encounter-1-Enemy-Encounter-Generation-Foundation-Closeout-Report-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-26_RRPG_WP-Event-1-Unified-Event-Content-Foundation-Closeout-Report-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-26_RRPG_WP-Event-1-Unified-Event-Content-Foundation-Closeout-Report-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-26_RRPG_WP-Event-1-Unified-Event-Content-Foundation-FDS-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-26_RRPG_WP-Event-1-Unified-Event-Content-Foundation-FDS-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-09-30_RRPG_Battle-Authoring-Structure-Consolidation-V0.1.md](../../Docs/DevelopmentSpecs/2026-09-30_RRPG_Battle-Authoring-Structure-Consolidation-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-01_RRPG_Current-UI-TMP-Battle-Surface-V0.1_Report.md](../../Docs/DevelopmentSpecs/2026-10-01_RRPG_Current-UI-TMP-Battle-Surface-V0.1_Report.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-02_Battle-Combat-Feedback-Foundation-V0.1-Audit.md](../../Docs/DevelopmentSpecs/2026-10-02_Battle-Combat-Feedback-Foundation-V0.1-Audit.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-02_Battle-Combat-Feedback-Foundation-V0.1-Report.md](../../Docs/DevelopmentSpecs/2026-10-02_Battle-Combat-Feedback-Foundation-V0.1-Report.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-02_RRPG_工程现状与引用审计.md](../../Docs/DevelopmentSpecs/2026-10-02_RRPG_工程现状与引用审计.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_Autonomous-Life_Technical-Architecture-V0.1.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_Autonomous-Life_Technical-Architecture-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-1_Social-Foundation_Closeout.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-1_Social-Foundation_Closeout.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-1_Social-Foundation_Development-Spec-V0.2.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-1_Social-Foundation_Development-Spec-V0.2.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-2_Activity-Lifecycle-Daily-Integration_Closeout.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-2_Activity-Lifecycle-Daily-Integration_Closeout.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-2_Activity-Lifecycle-Daily-Integration_Development-Spec-V0.2.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-2_Activity-Lifecycle-Daily-Integration_Development-Spec-V0.2.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-3_Autonomous-Selection_Closeout.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-3_Autonomous-Selection_Closeout.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-3_Autonomous-Selection_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-3_Autonomous-Selection_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-4_Social-Feedback_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_WP-AL-4_Social-Feedback_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-04_RRPG_架构优化总结与验收清单.md](../../Docs/DevelopmentSpecs/2026-10-04_RRPG_架构优化总结与验收清单.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-05_RRPG_Intraday-AD_Execution-Record.md](../../Docs/DevelopmentSpecs/2026-10-05_RRPG_Intraday-AD_Execution-Record.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-05_RRPG_Intraday-Time-Expedition-Arrival_Development-Spec-V0.2.md](../../Docs/DevelopmentSpecs/2026-10-05_RRPG_Intraday-Time-Expedition-Arrival_Development-Spec-V0.2.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-05_RRPG_Intraday-Work-Lifecycle-Onsite_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-10-05_RRPG_Intraday-Work-Lifecycle-Onsite_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-05_RRPG_WP-AL-4_Social-Feedback_Closeout.md](../../Docs/DevelopmentSpecs/2026-10-05_RRPG_WP-AL-4_Social-Feedback_Closeout.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-07_RRPG_V16_Execution-Record.md](../../Docs/DevelopmentSpecs/2026-10-07_RRPG_V16_Execution-Record.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-07_RRPG_Village-Product-Home-and-Building-Foundation_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-10-07_RRPG_Village-Product-Home-and-Building-Foundation_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-08_RRPG_V17-R1_Execution-Record.md](../../Docs/DevelopmentSpecs/2026-10-08_RRPG_V17-R1_Execution-Record.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R2_Execution-Record.md](../../Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R2_Execution-Record.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-10-10_RRPG_V17-R3-A_Person-Detail_Execution-Record.md](../../Docs/DevelopmentSpecs/2026-10-10_RRPG_V17-R3-A_Person-Detail_Execution-Record.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-11-05_RRPG_Initial-World_Completion_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-11-05_RRPG_Initial-World_Completion_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Final-Status-Report.md](../../Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Final-Status-Report.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Fixed-World-Place-Presentation_Development-Spec-V0.1.md](../../Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Fixed-World-Place-Presentation_Development-Spec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Dependency-Closeout.md](../../Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Dependency-Closeout.md) — 已读全文
- [Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Receipt.md](../../Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Receipt.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Report.md](../../Docs/DevelopmentSpecs/2026-11-06_RRPG_V17_Local-Closeout-Report.md) — 已读全文
- [Docs/DevelopmentSpecs/Evidence/2026-09-23_M3-Enemy-Presentation-Authority/README.md](../../Docs/DevelopmentSpecs/Evidence/2026-09-23_M3-Enemy-Presentation-Authority/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/DevelopmentSpecs/README.md](../../Docs/DevelopmentSpecs/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/Roadmap/RRPG_Milestone_A_Definition_v0.1.md](../../Docs/Roadmap/RRPG_Milestone_A_Definition_v0.1.md) — 已读全文
- [Docs/Roadmap/RRPG_Milestone_A_Readiness_Audit_2026-11-06.md](../../Docs/Roadmap/RRPG_Milestone_A_Readiness_Audit_2026-11-06.md) — 已读全文
- [Docs/决策记录.md](../../Docs/决策记录.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/开发验收交接.md](../../Docs/开发验收交接.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/问题清单.md](../../Docs/问题清单.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [Docs/项目日志.md](../../Docs/项目日志.md) — 已核对：8/30 Battle、9/3 V2、9/10 V3C及MinimumWorld、9/15 V4相关段
- [Docs/项目状态快照.md](../../Docs/项目状态快照.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/01 产品定位与设计支柱.md](../../GameDesign/01%20产品定位与设计支柱.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/02 核心循环与系统关系.md](../../GameDesign/02%20核心循环与系统关系.md) — 已读正文：§2.1–2.7
- [GameDesign/03 Battle.md](../../GameDesign/03%20Battle.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/04 Person.md](../../GameDesign/04%20Person.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/05 Cards.md](../../GameDesign/05%20Cards.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/06 Expedition.md](../../GameDesign/06%20Expedition.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/07 Village.md](../../GameDesign/07%20Village.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/ArtDirection/README.md](../../GameDesign/ArtDirection/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/ArtDirection/RRPG视觉参考与对照矩阵-V0.1.md](../../GameDesign/ArtDirection/RRPG视觉参考与对照矩阵-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/ArtDirection/RRPG顶层美术架构-V0.1.md](../../GameDesign/ArtDirection/RRPG顶层美术架构-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/Characters/Cook.md](../../GameDesign/Identity/Characters/Cook.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/Characters/Farmer.md](../../GameDesign/Identity/Characters/Farmer.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/Characters/Herbalist.md](../../GameDesign/Identity/Characters/Herbalist.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/Characters/Hunter.md](../../GameDesign/Identity/Characters/Hunter.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/Characters/Worker.md](../../GameDesign/Identity/Characters/Worker.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/README.md](../../GameDesign/Identity/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/卡牌与Trait设计工具箱.md](../../GameDesign/Identity/卡牌与Trait设计工具箱.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/Identity/角色设计方法论.md](../../GameDesign/Identity/角色设计方法论.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/README.md](../../GameDesign/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameDesign/当前设计状态.md](../../GameDesign/当前设计状态.md) — 已核对：当前状态与三/四/Deferred段
- [GameDesign/数值设计原则.md](../../GameDesign/数值设计原则.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Battle/Battle-GameSpec-V0.1.md](../../GameSpec/Battle/Battle-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Battle/Battle-Scene-UI-Binding-V0.1.md](../../GameSpec/Battle/Battle-Scene-UI-Binding-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Battle/README.md](../../GameSpec/Battle/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Expedition/Expedition-Progression-GameSpec-V0.1.md](../../GameSpec/Expedition/Expedition-Progression-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Person/Person-Profession-Growth-GameSpec-V0.1.md](../../GameSpec/Person/Person-Profession-Growth-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Person/README.md](../../GameSpec/Person/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/README.md](../../GameSpec/README.md) — 已读：Authority矩阵与使用边界
- [GameSpec/Save/README.md](../../GameSpec/Save/README.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Save/Save-GameSpec-V0.1.md](../../GameSpec/Save/Save-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Village/Product-Home-World-GameSpec-V0.1.md](../../GameSpec/Village/Product-Home-World-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Village/Village-Building-World-GameSpec-V0.1.md](../../GameSpec/Village/Village-Building-World-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [GameSpec/Village/Village-GameSpec-V0.1.md](../../GameSpec/Village/Village-GameSpec-V0.1.md) — 已核对：§6.8全文及§4/§12定位
- [GameSpec/Village/Village-Resource-Coupling-GameSpec-V0.1.md](../../GameSpec/Village/Village-Resource-Coupling-GameSpec-V0.1.md) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [UnityProject/RRPG1.0game/Assets/Tests/EditMode/AutonomousLife/SocialFeedback/SocialFeedbackIntegrationTests.cs](../../UnityProject/RRPG1.0game/Assets/Tests/EditMode/AutonomousLife/SocialFeedback/SocialFeedbackIntegrationTests.cs) — 已核对：HistoricalReciprocity方法L229起，其余未全读
- [UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ExpeditionV4F/ExpeditionReturnIntegrityProductPlayTests.cs](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ExpeditionV4F/ExpeditionReturnIntegrityProductPlayTests.cs) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论
- [UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ExpeditionV4F/IntradayReturnProductPlayTests.cs](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/ExpeditionV4F/IntradayReturnProductPlayTests.cs) — 仅路径/关键词定位；未逐正文审阅，不作为独立已证实结论

- [UnityProject/RRPG1.0game/Assets/Tests/PlayMode/Village/VillageV3CProductChainPlayTests.cs](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/Village/VillageV3CProductChainPlayTests.cs) — 已核对：主链233–340与Recognition产生段178起
- [UnityProject/RRPG1.0game/Assets/Tests/PlayMode/VillageV4/VillageV4FullProductChainPlayTests.cs](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/VillageV4/VillageV4FullProductChainPlayTests.cs) — 已核对：Phase A–D及断言296–460
- [UnityProject/RRPG1.0game/Assets/Tests/PlayMode/BattleStage6BRewardLoopPlayTests.cs](../../UnityProject/RRPG1.0game/Assets/Tests/PlayMode/BattleStage6BRewardLoopPlayTests.cs) — 已读：RunRewardLoop与第二战DrawPile断言

## 本任务交接

本轮仅新增本索引与同目录产品整合总览；未修改现有Readiness Audit、GameSpec、Runtime、共享Handoff，未暂存/Commit/Push。未运行Unity、Builder、Compiler或测试。内容待主代理抽样检查；Git状态Ready for Commit（如后续获本任务精确授权），不宣称Work Package/Branch/Repository Closed。原V17 PRODUCT ACCEPTED / LOCAL CLOSEOUT BLOCKED / NOT PUSHED 保留。工作区原有186条状态保持，保护以主代理SHA核验为准。没有Disposable测试产物需清理。

## 主代理最终复核（2026-11-06）

已回读两份完整正文，并独立抽样核对当前核心循环§2.7、FP七Gate、Village§6.8、6183990文件统计及V4/Battle6B测试支持边界。本轮新增文档的本地链接检查零缺失。对开工记录的4757个tracked/untracked非ignored既有文件逐字节SHA256复核：零变化、零缺失；保护回执为 D:/AI_Agent_Lab/audit-results/fp-integration-20261106/review-checks.json。该保护集合不含ignored文件，不能冒充全磁盘清单。暂存区为空；HEAD仍4d9ea21；本轮仅新增本索引和整合总览。仓库全局diff检查报告的是既有字体资产空白，不处理该任务外文件，也不将全局检查写为通过。

审阅结论：本轮证据整合文档可供评审，检索覆盖与未读限制如上；不代表完整产品Gate通过或V17工程收尾完成。后续先取得可复核工程快照，再另行安排正常玩家路径的G6/G7验证；未发起新功能实施、引擎测试、暂存、提交或Push。
