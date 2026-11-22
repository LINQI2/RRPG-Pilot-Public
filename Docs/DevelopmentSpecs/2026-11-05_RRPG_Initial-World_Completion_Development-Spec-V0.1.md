# Initial World 收尾执行规格 V0.1

日期：2026-11-05。状态：Implemented（本轮功能范围）/ Verification Partial（字体保护异常未解决）/ Product Acceptance Pending / Not Committed / Not Published。实际回执及限制见[技术复核](../../Development/StudioRuns/Initialization-20261105/technical-review.md)，不将历史测试当成本轮通过。

## 依据与边界

制作人于本次 Codex 接手会话明确批准 Initial World 收尾及必要 DSH 调用，唯一范围为 `D:\AI_Agent_Lab\rrpg_pilot\repo`。Target Branch 为 `ai/rrpg-r3b-population-pilot`，接手 HEAD 为 `054b00d3e38fe2e2e142bfdb2502de90f7991030`。不得修改正式工程、连接器、ACL 或认证，不得 Commit/Push。

本文件将本次制作人已确认要求整理为开发契约，不创建第二份 GameSpec，不以历史报告反推设计。既有 Home/Building 与 Person Management 规格继续适用：

- [Home/Building Foundation](2026-10-07_RRPG_Village-Product-Home-and-Building-Foundation_Development-Spec-V0.1.md)
- [Person Management](2026-10-09_RRPG_V17-R3_Person-Management_Development-Spec-V0.1.md)
- [历史 Initial World 交付与最后增量](../../Development/StudioRuns/Initialization-20261027/final-delivery-review.md)

## 冻结的行为契约

1. 初始化配置必须独立且数据驱动，包含人物与来源、住房、农田、厨房、资源、英雄及已有初始化参数。所有数量、比例和经济参数仍为可修改测试值，不冻结人口或住房规则。
2. 当前正式建筑类型仅 Farm、Kitchen、House。Workshop/Smithy 不进入正式目录或默认村庄；保留必要历史配置、资产和存档兼容，不粗暴删除。
3. Farm/Kitchen 是有唯一身份的真实 Completed 建筑实例，按各自功能注册独立岗位与资源角色。真实管理 UI 能派工/撤职，既有劳动系统实际生产；House 不凭空生岗位。
4. 住房和人物保留现有显示关联与去重方式，不引入入住容量、人口上限或住房成长。
5. NewGame 只执行一次初始化；Continue 恢复存档人物、建筑、岗位与资源，不补种默认世界、不重复结算。
6. 保留既有 Save Authority。无必要不改 SaveVersion=1、Save DTO 或引入迁移；重大设计或不可逆变更另行决策。
7. A 仍为默认档，补齐默认可识别表现；沿用既有美术方向，不以切默认 B 代替 A 收尾，不冻结最终画风。

## Existing Host Map

| 职责 | 复用入口 |
|---|---|
| 配置人工源 | `Development/Config/RRPG_InitialWorld.xlsx` |
| 配置生成 | Compiler → `InitialWorld/InitialPopulation/InitialBuilding.json` → Unity `StaticConfigBuilder` |
| 运行时读取 | `InitialWorldConfigDatabase` → Root/Registry → `InitialWorldConfigurationCatalog` |
| 一次性创建 | `SaveSystem.NewGame(config)` → `TryPrepareNewGameWorld` → `CommitNewGameWorld` |
| 实例建筑 | `BuildingRuntime.TryAddInitialCompleted`，复用既有位置校验和实例 ID |
| 岗位与资源 | `VillageRuntimeBridge.Buildings` 的功能岗位/资源岗位派生；岗位 ID 包含实例 ID 和 FunctionSlotId |
| Home 显示 | `ProductHomeReadAdapter`、`ProductHomeWorldHost`、`HomePresentationResolver`、`HomeWorldEntityView` |
| 集中管理 | 既有 Management 查询、Gateway 和 UI；仅允许必要最小接口修复 |
| 保存恢复 | 既有 `SaveSystem` 与 Bridge restore；旧无 Buildings/静态职责保留兼容 |

不得另建并行正式产品流程或通过解锁静态 ws_* 岗位绕过实例系统。序列化资产经 Unity Editor 操作；生成 Mirror 不手工修改。配置链以 XLSX 为唯一人工 Authority。

## 验证矩阵

| 项目 | 可接受证据 |
|---|---|
| Compiler | 本轮真实测试回执；validate 无 Hard Error，Mirror 与源一致且 validate 不写 Mirror |
| 初始化模块 | 配置/引用/布局校验、失败零半状态、真实实例与独立岗位、House 无岗位 |
| 真实产品 | NewGame 或 Continue 经现有入口进入 Town/Home，UI 派工/撤职，实际劳动和加工 |
| V4 缺口 | `VillageV4ResourceSurplusPlayTests` 1 例真实通过，0 skip；生产、供餐、Food 盈余和远征 carry 扣减对账 |
| Save/Continue | 人物/建筑实例 ID/岗位职责/资源恢复；无重复生成与重复结算 |
| 旧档 | 无 Buildings 与静态职责的兼容恢复；不把旧职责当正式新建筑 |
| 表现 | 默认 A 的 Farm/Kitchen/House/Person 可识别、实体无重复；A/B 切换不改领域事实 |
| 保护 | 测试前后存档逐字节回读、字体/场景保护；所有既有未提交文件保留或有明确授权差异记录 |

历史测试不是本轮通过。0 项、skip、中止、静态核查、编译与产品集成分开记录。Unity 全量和写世界专项串行；按改动选择最小必要回归，不无理由重跑全部长测。不削弱断言或删除失败证据。

## 执行与证据

- [DSH 工作包](../../Development/StudioRuns/Initialization-20261105/dsh-work-package.md)
- [本轮执行记录](../../Development/StudioRuns/Initialization-20261105/execution-record.md)
- DSH 进度与最终结果在同目录 `dsh-progress.md`、`dsh-result.md`；文件存在或接口 accepted 不表示完成。
- 原始既有文件清单位于仓库外 `D:\AI_Agent_Lab\audit-results\rrpg-initialworld-20261105\preexisting-files.json`；1278 文件，SHA256 `34e304725cc7a2b3f91d0e4d36e62e1a3692bb6df837b122f09d77947a2a2958`。不覆盖该基线。

技术验证、产品验收和发布状态分别记录；没有真实验证时保持 Partial，制作人未体验验收前保持 Product Acceptance Pending。
