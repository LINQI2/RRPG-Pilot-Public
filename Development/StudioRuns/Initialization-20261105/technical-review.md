# Initial World 本轮技术复核与可审查交付

日期：2026-11-05（北京时间）。目标分支 `ai/rrpg-r3b-population-pilot`，HEAD 仍为 `054b00d3e38fe2e2e142bfdb2502de90f7991030`，未 Commit/Push。**Implemented / 定向验证通过 / Product Acceptance Pending；保护异常未解决，整包仍 Partial。** 本文不是最终验收 Handoff，不宣布 Closed。

## 交付内容

保留既有 Initial World 框架和实例/岗位/存档实现；本轮没有重写初始化或管理系统，没有修改 SaveVersion/DTO。原 `RRPG_InitialWorld.xlsx` 及 InitialWorld/Population/Building 三份配置不变，当前5人物+英雄、1 Farm/1 Kitchen/5 House仍为可修改测试数据。

根据制作人在原生 DSH 会话选择的 A 白模方案，新增 `Development/SourceAssets/Village/A-Tier/{house,kitchen,person}-a.png`，通过两本 Authoring 工作簿追加资源与表现映射：

- `Development/Config/RRPG_VisualAssets.xlsx`：3个共享资源，原表逐字节备份到 `before-a-tier/`。
- `Development/Config/RRPG_VillagePresentation.xlsx`：5个人物/建筑表现键、Kitchen/House两生命周期共4行绑定；原表同目录备份。
- Compiler 生成 `VisualAsset.json`、`VillagePresentation.json`、`BuildingPresentationBinding.json`、`AssetManifest.json`；Unity Builder 生成数据库及管理PNG。未手改生成JSON或序列化YAML。
- `HomePresentationResolver.cs`：A人物表现键也从生成配置查表，修复“配置存在却永远取neutral”的缺陷。
- `HomeWorldEntityView.cs`：A人物Sprite等比缩放、高度1.05，静止颜色白色；B和人物通勤的既有着色语义保留。没有改身份、岗位、到达判定或Save数据。

工作包及续执行、DSH报告、执行脚本、Compiler日志、专项日志/截图和本轮复核证据都保留。详见 `dsh-resume-01-result.md`；恢复前报告保持原样。

## 真实验证与证据

| 验证 | 本次结果 | 证据 |
|---|---|---|
| Compiler | 280/280 Passed、0 skipped；首次testhost超时0项中止未隐去 | `test-results/initialworld-20261105-host.trx`、首次 `.trx` |
| 修改后配置validate | 主开发独立执行 PASS，errors=0 warnings=9（导航页），Mirror未写 | `reviewer-validate.log` |
| Source→Manifest→Unity PNG | 3/3两端SHA均与Manifest一致 | `reviewer-asset-chain.json` |
| 初始化EditMode | 24/24 Passed；主开发直接读取MCP逐例结果，message/stackTrace均null | `reviewer-test-results.txt` |
| 产品PlayMode | DSH最终产品入口/Kitchen真实UI 2/2 Passed，日志可核对派工/加工/Save/Continue | `../Initialization-20261027/play-evidence/initialization-product-play-evidence.log`，2026-11-05段 |
| V4+视觉稳定 | 主开发在最终代码上独立串行执行3例，3/3 Passed、0 failed、0 skipped，2.7028398s | `reviewer-play-v4-visual.txt` |
| 最终Editor | Pilot 2022.3.62t9，playing/compiling/updating=false，SampleScene dirty=false，字体9子资产dirty=0 | `reviewer-editor-final.txt` |
| 存档guard | 136个manifest全部restored，无missing/未完成 | `reviewer-guard-final.json` |

独立3例为：`ActivityPlacesCannotReflowRegisteredWorkplaces`、`ProfilesPreserveEntitiesFootprintsSelectionAndDomain`、`FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry`。不是测试发现或历史结果，而是本轮实际运行。

产品日志最后一轮明确记载实例Farm/Kitchen管理UI派工、真实劳动与输入加工、Continue sameBuildings=true、noReseed=true、housingNoWorkplace=true、workshopRejected=true。V4覆核真实三天生产、供餐、月度Food积累与远征carry扣减。Edit24覆盖旧无Buildings/静态职责恢复与新档不补种。

管理17例本轮不全量重跑：未改管理代码，选用真实产品入口2例和领域契约验证初始化接线；不将旧17/17冒充本轮。物理输入、IME、DPI、Player构建仍未验证。

## 保护核验与未解决字体异常

原1278文件清单保持不变。恢复前后的独立比对及最后差异见 `reviewer-original-changes.json`；原文件无删除。原有dirty成果中发生变化的仅：5份经授权追加的Current文档、既有产品专项日志追加本轮证据，以及下述字体异常。两本原本clean的Authoring和两份原本clean的产品C#为本轮任务改动，不混入原dirty成果。

用户存档：测试前/后均21611B，SHA256 `3ed26a77f3d8d2dc4d7599f6cc28faa8f60ea9fc535e65c75a090871b275e3f6`；测试通过既有guard逐字节恢复。SampleScene均846770B，SHA256 `570ab047004d8d3c9ff06f91f7aea18aad05b3aaff1e064ac1ca67795228c72c`。

**字体保护未通过，不隐藏、不据Git回退：**

- 接手清单/本轮测试前为14968844B，SHA256 `9ada53620a042f0fc823a1301e7cc5692d847a4f97845bf78be5ce342b1c22d5`。
- DSH编译刷新与后续测试之间变为14969377B（+533），SHA256 `f0f5b901a5ef56a68450803e39510846ccfc38623ab0d3c8654b22f33bcf1681`；主开发最后3例未再改变它，Editor当前dirty=0。
- 该资产原本已有未提交修改。Git HEAD和现有旧字体备份均不是接手时SHA；对Pilot目录检索也未找到14968844B的完整副本。不能据此把它恢复成Git版本，否则会覆盖既有成果；也不手改YAML或反向猜测新增533字节。
- 当前字节额外保存在 `review-backups/font-after-dsh.asset`，保留调查证据。尚无完整前字节供语义diff/精确恢复，原因仅与Refresh/TMP时间相关，**未证明**。因此本轮不宣称全部保护通过或工作包完成。

## 默认A体验与Deferred

在已经打开的Pilot Editor中按Play，从主菜单选新游戏进入Home；只体验现有存档时选Continue。新游戏/测试可能替换用户单槽，需先保留自己的存档。测试截图来自隔离世界且已恢复用户档，并未把测试存档留作默认用户档。

右下 `R2对照：A白模` 保持A默认；观察5座住房、带烟囱厨房、Farm及蓝色人物；管理页“职责与场所”选择实例岗位派工/撤职，推进时间观察生产，Save/Continue核对身份与库存。

画面证据：`../Initialization-20261027/play-evidence/initialization-product-save-continue-20261105T050547Z.png`（主开发已看图确认）。截图是技术白模，不等于最终美术验收。

Deferred/限制：Farm仍使用带TECH PROOF文字的原测试图；非建筑承载的通用Place仍可为neutral。真实住房/实例岗位在Home别名层不重复绘制，因此不为隐藏的别名凭空新增实体或岗位。最终画风、经济平衡、物理输入和平台构建未冻结/未验。本轮可识别性与B档观感等待制作人产品验收；字体异常另列，不算普通美术Deferred。
