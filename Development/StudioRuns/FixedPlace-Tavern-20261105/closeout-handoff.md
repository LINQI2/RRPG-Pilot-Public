# Tavern 固定世界场所：Closeout / Handoff

日期：2026-11-05。状态：**Product Accepted（制作人 2026-11-05 确认三项验收通过）/ Closeout Prepared / Commit Pending**。

## 1. 关闭结论

Tavern 已作为 **Fixed World Place** 交付并通过制作人手动验收：NewGame / Continue 始终存在；Drink / Talk 等活动的开始、结束、
中断不影响它；多人引用不重复；A→B→A 不丢失不重复；A 档有专属可识别白模（小屋 + 悬挂招牌）。

**不再是开发项**：可建造酒馆、升级、经营、库存、岗位、施工、室内模拟均明确 Deferred，本任务不派生新工作。

## 2. 实现方案

- 配置：`va_village_tavern_a`（`Village/A-Tier/tavern-a.png`）；表现键 `village.place.fixed.Tavern`（A）、
  `village.place.r2.fixed.Tavern`（B）。
- 代码：`ProductHomeSnapshot.IsFixedPlace`；`ProductHomeReadAdapter.ApplyFixedPlaces`（配置驱动：AutonomousLife `Place` 表
  ∩ `village.place.fixed.<PlaceId>` 表现键）；`HomePresentationResolver` 档位键 + `village.place.*` 查表；`ProductHomeWorldHost`
  恒绘制；`HomeWorldEntityView.ApplyPlace` 固定场所不施加 Place 通用 tint。

## 3. 本轮真实发现并修复的缺陷（不美化）

| # | 缺陷 | 表现 | 修复 |
|---|---|---|---|
| 1 | `StaticConfigBuilder` 未更新生成库 | 运行时缺新表现键，酒馆回落 neutral | 重建生成库 |
| 2 | `HomePresentationResolver.TryResolve` 不查 `village.place.*` | 固定场所键永远查不到 → neutral | 补 `village.place.` 前缀 |
| 3 | `ApplyPlace` 对 Place 施加绿色 tint | 酒馆白模被染绿 | 固定场所改白色 |
| 4 | 首版截图仅有 neutral marker | 不能作为视觉验收证据 | 重新生成有效截图 |

**旧截图、失败记录与异常全部保留**，未删除或覆盖；本文件与 `dsh-result.md` §9 指向修复后的有效证据。

## 4. 最终证据

| 项目 | 结果 / 位置 |
|---|---|
| PlayMode | 10/10 Passed（Tavern 2 + 视觉原型 8），0 failed / 0 skipped |
| InitialWorld EditMode | 24/24 Passed |
| Compiler | 280/280，`artifacts/compiler-verify.log` |
| validate | PASS（errors=0，Mirror 未写入） |
| 有效截图 | `Development/StudioRuns/FixedPlace-Tavern-20261105/screenshots/a-fixed-tavern.png`（21:45:32，`bc1c0b50…`） |
| 字体门禁 | 各段 begin/end 均 0；字体 `7b5717b9…` 未变；历史 incident 保留 |
| Save / 场景 | 未新增 SaveVersion / DTO；用户单槽与场景未被覆盖 |

## 5. Git 文件归属审计（只读）

| 类别 | 结论 |
|---|---|
| A. Tavern 独立变更 | **无纯 Tavern 文件**：`ProductHomeSnapshot` / `ProductHomeReadAdapter` / `HomePresentationResolver` / `HomeWorldEntityView` / `ProductHomeWorldHost` 同时承载上一包 A/C/B 改动；`RRPG_VillagePresentation.xlsx` / `RRPG_VisualAssets.xlsx` 与 3 份生成 JSON 亦为两包共用 |
| B. Initial World 产品体验修复 | 与 A 共用上述文件与配置，无法按文件切分 |
| C. V17-R3-B 既有变更 | 存在，未触碰 |
| D. 其他历史 dirty / untracked | 1689 untracked + 64 modified，未确认归属者一律不动 |

**判定：不具备安全、可审计的 Tavern 专项 Commit 条件。** 按授权不执行 `git add .` / `-A` / 整包提交，
不做 hunk 级强拆（会切断 A/C/B 与 Tavern 的交织改动、易遗漏）。→ **Commit Pending**。

当前：分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3…`，暂存区空，dirty 195。未 push。

## 6. 文档同步

设计检查点、V17 Fixed-World-Place Spec、Product Home GameSpec、Building World GameSpec、`GameDesign/07 Village`、
`GameDesign/当前设计状态`、`Docs/项目状态快照.md`、`Docs/项目日志.md`、`Docs/文档地图.md`、`Docs/DevelopmentSpecs/README.md` 均已同步，
且明确 **Tavern 未写入 VillageWorldRegistry**、未新增 BuildingInstance。

## 7. 下一阶段的真实依赖

1. 由主开发决定 Tavern 与 Initial World 两包是否合并为一次提交（当前最安全），或先拆分 A/C/B 再单独提 Tavern。
2. 制作人体验后如需正式美术，另立美术任务（本轮白模不冻结画风）。
3. 可建造 / 经营的酒馆需要新的产品决策与 Save/经济边界，不在本次派生范围。
