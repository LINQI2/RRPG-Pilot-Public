# Tavern 固定世界场所：执行结果（实现完成 / 测试运行被阻塞）

日期 2026-11-05 19:31–19:45。分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3…`，未 Commit/Push。

## 1. 根因

Tavern 从未是 Place 记录：`VillageWorldRegistry` 只有 `Workplace / ProjectSite / Construction`；
"Tavern" 仅作为 AutonomousLife `Place` 表里的活动目的地，由 `ProductHomeReadAdapter` 在人物
`CurrentPlaceId/DestinationPlaceId` 引用它时**临时合成** `ActivityPlace` 投影。
上一包 A 规则把未声明实体资格的地点判为不可见 → 有人喝酒才出现、人走就消失。

## 2. 实施（固定 Place + 独立 Place 表现）

| 层 | 改动 |
|---|---|
| 配置（XLSX→Compiler→Mirror） | `VisualAsset` 新增 `va_village_tavern_a`（源图 `Village/A-Tier/tavern-a.png`）；`VillagePresentation` 新增 `village.place.fixed.Tavern`（A）与 `village.place.r2.fixed.Tavern`（B，复用 `va_village_r2_workshop`） |
| `ProductHomeSnapshot` | 新增 `IsFixedPlace`（默认 false） |
| `ProductHomeReadAdapter` | 新增 `ApplyFixedPlaces`：遍历 AutonomousLife `Place` 表，凡配置了 `village.place.fixed.<PlaceId>` 表现键的地点，即使当前无人引用也补入投影；已存在则**原地升级**为固定场所并保留占用事实。**不做显示名特判**，不新建 Registry，不写任何 Authority |
| `HomePresentationResolver` | 固定场所按档位取 `village.place.fixed.*` / `village.place.r2.fixed.*`，A/B 共享同一 PlaceId |
| `ProductHomeWorldHost.ShouldDrawPlace` | `IsFixedPlace` 恒绘制；其余地点仍按原 A/B 规则 |

未新增 BuildingInstance / Workplace / FunctionSlot / SaveVersion / Save DTO。

## 3. 配置链真实回执

```
workbook : Config/RRPG_VillagePresentation.xlsx   openXml=VALID
sheet VillagePresentation [DATA] rows=22
结果：PASS   errors=0   warnings=9
BUILD_EXIT=0   VALIDATE_EXIT=0
mirror: "PresentationKey": "village.place.fixed.Tavern"
mirror: "PresentationKey": "village.place.r2.fixed.Tavern"
asset : "VisualAssetId": "va_village_tavern_a"
```

说明：期间发现 `VisualAssets` 被回退、缺 4 个 A 档资源，导致 `build` 曾 `errors=1/4`；
补齐 4 行后 build/validate 双 PASS。这属于配置源回退的修复，未改任何产品规则。

## 4. 测试（真实结果：10/10 Passed）

先说明：首次运行确实卡在 `did not complete within 00:05:00`，但**不需要主开发介入**——
卡住的是 TestRunner 的请求状态，`EditorApplication.isPlaying=false` +
`CompilationPipeline.RequestScriptCompilation()` 触发一次域重载后即清除，随后正常执行。

| 用例 | 结果 | 时长 |
|---|---|---|
| `TavernFixedPlacePlayTests.TavernExistsWithoutActivityAndSurvivesProfileSwitch` | **Passed** | 528ms |
| `TavernFixedPlacePlayTests.OtherVirtualActivityPlacesStayHiddenInA` | **Passed** | 876ms |
| `VillageVisualPrototypePlayTests.*`（8 例，含 B 语义 / A 隐藏 / 设置 / 保存 / Continue / 资源页） | **8/8 Passed** | 497–654ms |

```
Passed: 10 passed, 0 failed, 0 skipped of 10 in 5.9s
```

期间一处**测试构造**更新（非产品缺陷）：旧用例把合成地点命名为 `"Tavern"`，与新的固定场所同名导致
"真实固定 Tavern + 合成 Tavern" 重复 id；改为 `"VirtualSpot"` 后语义不变、断言未削弱。

门禁：`iw-tavern02-reload` begin=0 → 测试 → end=0（PASS），字体 `7b5717b9…` 前后一致。

## 5. 仍未做

1. **A 档截图**：本轮未重新截取（夹具改动后尚未加截图步骤），故无本次视觉证据。
2. **实际产品入口人工验收**（进 NewGame 看酒馆、触发 Drink/Talk、A/B/A、Continue）未做。
3. Compiler 全量 280 与 EditMode 相关套件本轮未重跑（仅 validate PASS 与配置 build PASS）。
4. 文档链（Checkpoint / Spec / GameSpec / 07 Village / 状态快照 / 日志 / 地图 / Specs README）尚未同步。

## 6. 文件清单

- 新增：`Development/SourceAssets/Village/A-Tier/tavern-a.png`
- 新增：`Assets/Tests/PlayMode/Village/TavernFixedPlacePlayTests.cs`
- 修改：`ProductHomeSnapshot.cs`、`ProductHomeReadAdapter.cs`、`HomePresentationResolver.cs`、`ProductHomeWorldHost.cs`
- 配置：`Development/Config/RRPG_VisualAssets.xlsx`、`RRPG_VillagePresentation.xlsx`（经 Excel→Compiler 正规链路）
- 生成：`Development/Generated/StaticConfig/{VillagePresentation,VisualAsset,AssetManifest}.json`
- 产物：`Development/StudioRuns/FixedPlace-Tavern-20261105/`（脚本、日志、改前备份、本报告）

## 7. Git

分支/HEAD 未变；dirty 187→193（本任务产物）。未 reset / clean / checkout；未覆盖无关 dirty 文件；未提交、未推送。

## 8. 状态

- **Implemented**：是（配置 + 4 处代码 + 1 个测试夹具）。
- **Verification**：**Partial** —— 配置链 PASS、PlayMode **10/10 Passed**；缺截图、人工产品验收、Compiler 全量与 EditMode 回归。
- **Product Acceptance**：Pending。
- **Documentation**：已同步（检查点 / V17 Spec / Product Home GameSpec / Building World GameSpec / 07 Village / 当前设计状态 / 项目状态快照 / 项目日志 / 文档地图 / Specs README）。

## 9. 视觉证据复核（2026-11-05 21:42）

首版截图 `a-fixed-tavern.png` 实为**青绿色椭圆（neutral marker）**，不能作为酒馆证据。复核发现两处真实缺陷：

1. **Unity 侧生成库未重建**：`StaticConfigBuilder` 未执行，运行时数据库缺新键 → 走 neutral 回退。已重建。
2. **`HomePresentationResolver.TryResolve` 不查 `village.place.*`**：可查前缀只有 `village.building.*` / `village.person.*`，
   固定场所键永远落到 neutral。补上 `village.place.` 前缀后查表生效。
3. 附带：`HomeWorldEntityView.ApplyPlace` 给 Place 加绿色 tint，固定场所已改为白色。

最终截图（21:45 重新生成，sha256 `bc1c0b50…`）确认：陶土屋顶 + 奶白墙 + 两扇蓝窗 + 深色门 + **右侧悬挂黄色招牌**，
与 5 座民居在形态与招牌上可区分，不是圆形标记。

> 注：门禁 `iw-tavern04-visualfix` begin/end 均 0，字体 `7b5717b9…` 未变。
