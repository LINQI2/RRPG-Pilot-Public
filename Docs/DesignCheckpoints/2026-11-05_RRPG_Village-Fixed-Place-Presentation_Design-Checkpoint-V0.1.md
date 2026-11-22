# RRPG 固定世界场所呈现设计检查点 V0.1

日期：2026-11-05（北京时间）  
状态：**Product Accepted（制作人 2026-11-05 确认三项验收通过）/ Implemented / Closeout Prepared / Commit Pending**；制作人 2026-11-05 授权实施。
实施结果见 [执行记录](../../Development/StudioRuns/FixedPlace-Tavern-20261105/dsh-result.md)。  
范围：Village / Product Home / Place Presentation  
权威关系：本文件记录本轮设计决策；可执行规则归位至 [Product Home World GameSpec](../../GameSpec/Village/Product-Home-World-GameSpec-V0.1.md) §4.1；Building / Place 身份边界继续服从 [Building World GameSpec](../../GameSpec/Village/Village-Building-World-GameSpec-V0.1.md)。

## 1. 问题与依据

当前规格存在三个不同层级的事实：

1. WP-AL-3 的 Home / Tavern / Workshop 是已确认的 **Prototype Content / Test / Not Frozen**，并不等于正式建筑内容。
2. Building World GameSpec §3 确认 **Building ≠ Place**，一个 Place 可以不来自 Building；§17 仅将 Workplace FunctionType 纳入 V0.1，ActivityPlace / Home / Storage / Social 继续 Deferred。
3. 2026-11-05 Initial World 产品体验修复包 A 为避免虚拟 ActivityPlace 随活动引用出现、消失而生成占位建筑，要求隐藏未配置独立可见实体的 ActivityPlace / Tavern 投影。

因此，当前问题不是 Activity 算法缺少酒馆，而是**固定场所的世界呈现生命周期被动态活动引用代替**：酒馆作为活动目的地存在，却没有被定义为始终存在的世界场所。

## 2. 本轮确认的产品规则

### 2.1 固定世界场所与临时活动地点分离

- **Fixed World Place（固定世界场所）**：被产品明确指定为村庄长期空间组成部分，拥有稳定 Place 身份和独立的世界表现。它不依赖当前是否有活动、是否有人占用或是否存在活动引用。
- **Transient / Virtual Activity Place（临时 / 虚拟活动地点）**：仅为某次活动、项目或临时交互提供语义目的地；除非另有明确的固定场所声明，否则不自动创建永久世界实体。
- Activity 开始、完成、失败、中断或无人使用，只能改变活动事实、人物在场状态及相关表现，**不得创建或销毁 Fixed World Place**。
- 不要求所有 Place 都可见。只有被明确指定为固定世界场所的 Place 才必须持续呈现。

### 2.2 Tavern（酒馆）的当前范围

- Tavern 被指定为当前初始 Home World 中的一个 **Fixed World Place**。
- 默认 A 表现必须始终可见，并能被玩家识别为酒馆；通用 neutral_marker 且无其他识别信息不满足本要求。可使用专用白模、轮廓、标识或明确标签，具体视觉方案不在本轮冻结。
- B 原型及 A/B/A 往返中也必须保持同一语义场所；每种模式最多呈现一个酒馆，不得因切换或活动投影产生重复实体。
- Drink / Leisure、Talk 等现有活动可以引用该固定 Place。活动的开始 / 结束不影响 Tavern 的存在、身份或世界位置。
- Tavern 的具体活动列表、活动概率、人物偏好、社交反馈数值仍按 Autonomous Life 的 Prototype / Calibration 边界处理；本决定不把测试内容整体升级为正式平衡。

### 2.3 Tavern 当前不是 BuildingInstance

本轮选择**固定 Place + 独立 Place Presentation**，而不是新增可建造酒馆：

- Tavern 不加入当前正式 BuildingDefinition / 玩家建造目录。
- 不创建伪造的 BuildingInstance / BuildingView 来冒充场所。
- 不启用 ActivityPlace FunctionSlot，不新增 Workplace、施工、成本、升级、室内、经营、库存或资源经济。
- 视觉上可以采用酒馆建筑形态，但领域身份仍是 Place，不因此变成 Building。
- 如果未来需要玩家建造、经营或升级酒馆，必须另立产品决策并扩展 Building Function / Save / 经济边界；不能从本次可见性要求自动推导。

## 3. Authority 与实现边界

- VillageWorldRegistry 继续是 Place 事实来源；不得建立第二套 Place Registry。
- ProductHomeReadAdapter 继续只读投影；ProductHomeWorldHost 只负责世界表现，不拥有 Activity / Presence / Occupancy 真相。
- Fixed Place 的可见资格必须来自明确配置或稳定身份映射，不能按显示名写死 Tavern 特判，也不能从 ActivityInstanceId 推导 Place 身份。
- Place Presentation Binding 与 Building Presentation Binding 保持分离；固定 Place 不要求先有 BuildingInstance。
- A/B 的视觉资源可以不同，但共享同一个语义 Place 身份；切换不得修改 Simulation Facts。
- Place 的世界坐标、Anchor、Sprite、动画与选择高亮仍属于 Presentation；不新增 Save 字段，不保存表现位置。

## 4. 明确保留的旧规则

- Building ≠ Place ≠ Presentation 不变。
- Building 可以提供 0..N 个 Place；Place 可以不来自 Building。
- Activity / Social / Relationship / VillageTime / Assignment / Occupancy 仍使用既有 Authority。
- 当前正式初始化建筑仍为 Farm / Kitchen / House；默认数量配置仍由现有 XLSX + Compiler 管理。
- ActivityPlace Building FunctionType、Tavern System、Building Interior、Room Simulation、Real Logistics 继续 Deferred。
- ProjectSite、施工地点及其他临时目的地不因 Tavern 被指定为固定场所而自动升级为固定世界实体。

## 5. 已否决方案

| 方案 | 结论 | 原因 |
|---|---|---|
| 继续让 Tavern 只在活动引用时出现 | 否决 | 固定场所不应随活动生命周期闪现 / 消失 |
| 把所有 Place 自动转成建筑 | 否决 | 混淆 Building / Place，并扩大内容与实现范围 |
| 用伪造 BuildingInstance 表现 Tavern | 否决 | 制造第二套或虚假的 Building 身份 |
| 按 DisplayName 写死 Tavern 特判 | 否决 | 不可扩展、易产生隐式规则 |
| 新建平行 Place Registry | 否决 | 违反 No Parallel Authority |
| 本轮实现完整酒馆经营系统 | 后置 | 与当前问题无关，且会扩展施工、岗位、经济和存档范围 |

## 6. 产品验收条件

1. NewGame 后即使没有 Drink / Talk 活动，A 档也有且只有一个可识别 Tavern。
2. Drink / Talk 开始、进行、结束、失败或中断时，Tavern 的 Place 身份、实体数量和位置保持不变。
3. 多名人物同时引用 Tavern，不生成多个酒馆实体；人物在场 / 到达事实仍按既有 Authority 派生。
4. A → B → A 切换前后，Tavern 始终只有一个表现实体；切换不改变 Place / Activity / Person / Save 领域事实。
5. Continue 后从既有 Place 事实与表现配置重建 Tavern；不要求保存 Presentation Position。
6. 其他未声明为固定场所的虚拟 ActivityPlace 仍遵循原隐藏规则，不被一并显示。
7. 不新增 BuildingInstance、Workplace、FunctionSlot、SaveVersion 或 Save DTO 字段。

## 7. 未冻结 / 后续

- Tavern 最终美术、白模形态、尺寸、坐标、地面布局、招牌、动画与交互细节；
- 其他固定世界场所的名单及优先级；
- Tavern 是否未来成为可建造 / 可经营建筑；
- 正式 Activity 内容、人物偏好、行为参数与社交平衡；
- 具体配置 Sheet / 字段 / 类名，由实施前代码与配置链审计决定。

本检查点只确认固定场所的产品呈现规则，不代表 Runtime 已实施，也不自动授权新的 Unity Work Package。
