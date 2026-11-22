# RRPG V17 固定世界场所呈现 Development Spec V0.1

日期：2026-11-05（北京时间）  
状态：**Implemented / Verification Passed / Product Acceptance Pending / Not Committed / Not Published**  
（2026-11-05 制作人授权实施；本文旧状态 "Implementation Not Authorized" 已被本轮授权覆盖。
实施：`ProductHomeSnapshot.IsFixedPlace` + `ProductHomeReadAdapter.ApplyFixedPlaces`（配置驱动：
AutonomousLife `Place` 表 ∩ `village.place.fixed.<PlaceId>` 表现键）+ `HomePresentationResolver` 档位键
+ `ProductHomeWorldHost` 恒绘制；真实回执 PlayMode 10/10、EditMode 24/24、Compiler 280/280、validate PASS；
无新增 BuildingInstance / Workplace / Save 字段。证据：`Development/StudioRuns/FixedPlace-Tavern-20261105/dsh-result.md`。）
前置工作流：[2026-11-05 Initial World 产品体验修复](2026-11-05_RRPG_V17_Initial-World_Product-Experience-Fixes_Development-Spec-V0.1.md)。

## 1. 目标

在 Product Home 默认 A 世界中，为被明确指定的固定世界场所提供持续、稳定、可识别的世界表现。当前首个目标是 Tavern。

Tavern 必须作为稳定 Place 呈现，不再依赖 Drink / Talk 等活动是否正在引用它。活动生命周期不能控制固定场所实体生命周期。

## 2. 设计 Authority

- 产品规则：[固定世界场所呈现设计检查点](../DesignCheckpoints/2026-11-05_RRPG_Village-Fixed-Place-Presentation_Design-Checkpoint-V0.1.md)。
- Product World 规则：[Product Home World GameSpec §4.1](../../GameSpec/Village/Product-Home-World-GameSpec-V0.1.md#41-fixed-world-place-vs-transient-activity-place)。
- 身份边界：[Building World GameSpec §3 / §17](../../GameSpec/Village/Village-Building-World-GameSpec-V0.1.md)。
- Autonomous Life 的 Tavern / Drink / Talk 仍为 Prototype Content / Calibration；不得把原型内容整体升级为正式平衡。

## 3. 前置状态与不可混包边界

2026-11-05 Initial World 产品体验修复 V0.1 已将未配置独立可见实体的 ActivityPlace / Tavern 投影在 A 档隐藏，目的是阻止临时活动投影伪装成永久建筑。该包当前技术实施与验证状态、制作人产品验收状态按原规格与技术报告读取。

本工作包是**独立后续增量**：
- 不回写、不篡改前一工作包的原始授权、执行记录或测试结论。
- 不将前一包 A 档隐藏规则解释为错误实现；该包按当时 Authority 正确执行。
- 后续为 Tavern 增加显式 Fixed World Place 声明和独立 Place Presentation Binding，使其满足固定场所资格。
- 不与当前 Initial World 产品修复包混合提交或清理。
- 当前分支与工作区存在大量既有 dirty / untracked 文件；本规格不授权 reset / clean / restore / Commit / Push，也不授权覆盖任何既有变更。

## 4. 实施范围

### A. 只读审计

开工后先核对：
- Tavern 当前 PlaceId / Kind / SourceRef 的真实来源与稳定性；
- Place Registry、Activity Provider、ReadAdapter、Snapshot、WorldHost 的数据链；
- 当前 A / B 的 PlaceView 与 BuildingView 资格判断；
- Initial World XLSX / Compiler / Generated Mirror / Builder 是否已有可复用的 Place Presentation 配置入口；
- NewGame / Continue 是否能在无活动时提供固定 Place 事实。

如果现有 Tavern PlaceId 由活动实例生成或生命周期不稳定，先设计 canonical stable Place identity；不得直接复用不稳定 ID。

### B. 固定 Place 声明与表现绑定

- 通过现有权威配置链声明 Tavern 为 Fixed World Place；优先复用现有 Place Authority 与静态配置管线。
- 为 Tavern 提供独立、数据驱动的 Place Presentation Binding；A 档使用可识别白模 / 专属占位表现，不能退化为无差别 neutral marker。
- B 档可以使用既有原型资源，但必须与 A 共用同一语义 Place 身份。
- A / B / A 往返时，固定 Place 保持单实例；动态 ActivityPlace 引用不得重复创建第二个 View。
- 不按 DisplayName / Sprite / World Position 推导 PlaceId；不为 Tavern 单独写死 Host 分支。
- 若需增加配置，人工源必须进入现有权威 Workbook / Compiler / Builder 链；禁止手工改 Generated JSON / Mirror 伪造配置。

### C. 生命周期与状态隔离

- Fixed World Place 的创建 / 销毁由世界配置与世界初始化 / 重建控制，不由 Activity occurrence / 当前引用控制。
- Activity 只更新既有活动事实、Person Presence / Arrival / Occupant 派生和必要的 Anchor 状态。
- Product Home 仍只读；不得写 Activity、Presence、Assignment、Occupancy 或 VillageTime。
- Place Position / Sprite / Anchor / Selection 属于 Presentation；不新增 SaveVersion / Save DTO / Presentation Save 字段。
- 不创建 BuildingInstance、BuildingDefinition、FunctionSlot、Workplace 或 Construction Project。

### D. 明确不做

- 可建造酒馆、建造目录、施工、材料成本、升级、经营、库存、收益、岗位；
- Building Interior / Room Simulation / NavMesh 正式方案；
- 新 Activity / Social / Relationship / Needs / Schedule 规则；
- 全部 Place 自动可见；
- 最终美术、完整动画、最终坐标与布局；
- 任何无关 Initial World / R3-B / 人口 / 经济 / Save 迁移工作。

## 5. 验证矩阵

| 验证 | 必须证明 |
|---|---|
| NewGame / 无活动 | A 档 Tavern 一开始就存在，且恰好一个、可识别 |
| Activity 生命周期 | Drink / Talk 开始、执行、完成、失败、中断均不创建 / 销毁 Tavern |
| 多人引用 | 多人同时以 Tavern 为目标时仍只有一个固定实体 |
| Place Identity | NewGame、活动切换、A/B/A、Continue 期间 canonical PlaceId 稳定 |
| A/B/A | A 白模与 B 原型可以不同，但场所身份与数量稳定，不重复、不丢失 |
| Continue | 通过既有 Authority 重建 Place；无活动也存在；不写入表现坐标 |
| 负向回归 | 未声明为 Fixed World Place 的虚拟 ActivityPlace 仍不生成可见占位实体 |
| 领域隔离 | Activity / Person / Assignment / Occupancy / VillageTime / Building 数据不被表现层写入 |
| 结构隔离 | 无新增 BuildingInstance / Workplace / FunctionSlot / SaveVersion / Save DTO 字段 |

需要真实 Home 入口截图与 Edit / Play 定向测试。若修改配置源，另验 Compiler / validate / Builder，确认 Generated Mirror 与人工源一致且 Builder 可重复 NoOp。所有 Unity 操作必须遵守当时有效的字体保护门禁及用户存档 / 场景保护规则。

## 6. 允许修改范围（实施前必须按只读审计收窄）

候选而非预先授权：
- ProductHomeReadAdapter.cs
- ProductHomeSnapshot.cs
- ProductHomeWorldHost.cs
- HomePresentationResolver.cs
- 既有 Place / Initial World 配置源及其 Compiler / Builder
- 对应 EditMode / PlayMode 测试

禁止先验扩大到 Activity Runtime、VillageWorldRegistry Authority、BuildingRuntime、Save DTO / SaveVersion、资源经济、人口规则或整个 Home UI。

## 7. 状态与下一步

- 本轮已完成：设计决策、产品规则归位、后续实施规格与验收矩阵。
- 本轮未执行：Runtime / 配置 / Unity / Compiler / Builder / Tests / Save 操作。
- 当前状态：Planning Accepted；Runtime Not Started；Implementation Not Authorized；未 Commit / Push。
- 下一步：先完成当前 Initial World 产品体验修复包的制作人验收与独立收尾；之后由制作人单独授权本包只读预检和实施。不得从本规划文档自动开始代码修改。
