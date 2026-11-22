# Initial World 产品体验修复 V0.1：技术交付

日期2026-11-05（北京时间）。Target Branch `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`。**Implemented / Targeted Verification Passed / Product Acceptance Pending / Not Committed / Not Published**。只交付本修复包，不宣布整个Initial World关闭。

## 本轮最终行为

- A档：未配置独立实体的ActivityPlace/Tavern等不生成可见PlaceView；Place事实、定位anchor、人物目标/到达/现场投影仍保留。Farm/Kitchen/House视图不变。B按原DrawsWorldEntity规则，A按新增独立资格标记，A/B往返不破坏身份/领域事实。
- Home右上“设置”：仅保存游戏、保存并返回主菜单、关闭。全屏遮罩与既有SurfaceModalActive阻断输入；保存成功明确反馈，失败留页显示原因；busy及同帧保护阻止重复/重入。复用SaveSystem与现有MainMenu宿主，不重建世界、不改SaveVersion/DTO。
- 保存并返回时显式停止Village模拟（主开发复核发现原薄返回接缝只切Canvas，补上SetSimulationActive(false)）；Continue通过真实主菜单入口复用原Home宿主并恢复身份/职责/库存。
- 资源信息行不再整行跳转；独立“查看岗位”按完整WorkplaceId定位Farm/Kitchen实例slot、保留非空Person选择、不写职责/库存/时钟。主开发补上滚动定位请求及行缓存失效，避免只换候选不滚动。

初始1 Farm/1 Kitchen/5 House、数量可调测试性质、食材低库存和人口/住房规则均未调整。未修改正式工程/连接器/认证/权限，未Commit/Push。

## 实际文件范围

产品：`Assets/Scripts/Village/Runtime/ProductHomeSnapshot.cs`、`Village/ProductHome/ProductHomeReadAdapter.cs`、`ProductHomeWorldHost.cs`、`HomeProductShell.cs`、`ManagementView.cs`、`Assets/Scripts/UI/MainMenuUI.cs`（均在Unity子工程下）。测试扩展既有 `Assets/Tests/PlayMode/Village/VillageVisualPrototypePlayTests.cs`。未为本包新建Scene或Prefab/第二套Save实现。

规格、工作包/续执行、备份、脚本、截图、原始回执和Current文档同步也属于本包交付。最终带SHA清单见 `delivery-files.json`。字体门禁v2/core/selftest是制作人在原DSH会话另行确认的保护能力更新，原始报告与两条历史incident保留，不将它们改写成首次全部通过。

## 真实验证矩阵

| 项目 | 本轮结果 | 原始证据 |
|---|---|---|
| Compiler | DSH本包真实280通过/0失败/0跳过；validate errors=0 warnings=9、Mirror未写 | `artifacts/compiler-verify.log` |
| 产品/视觉8例 | DSH复跑8/8；Codex强化后首次7/8（1个测试字段反射错误），修正后Continue单例1/1；最终8个不同用例均有真实通过回执 | `artifacts/mcp-responses.md`、`codex-play-final.txt`、`codex-continue-activity.txt` |
| 资源行强化 | 非空人物选择、每次进入Resources后普通点击不导航、显式岗位身份、领域事实不变、两行均进入viewport | `artifacts/codex-resource.txt`，1/1 Passed |
| 初始化Edit | Codex本轮24/24 Passed、0skip | `artifacts/codex-edit.txt` |
| 真实活动/现场 | Codex本轮2/2 Passed、0skip：真实Town通勤→活动执行、现场可见/可点/关闭不改模拟 | `artifacts/codex-activity.txt` |
| Console与Editor | 明确过滤Error/Exception为0条；精确Pilot，playing/compiling/updating=false，SampleScene dirty=false | `artifacts/codex-errors.txt`、`codex-state-final.txt` |

失败保留：DSH遮罩首跑7/8及修正过程见原报告；Codex首次Continue补强因把Bridge字段误作属性在607行NullReference，已改为实际Field读取再单独通过。没有削弱断言、删失败记录或将首次失败改写为PASS。第一次“Continue+活动”筛选实际只发现Continue一例（其它是partial类，文件名不等于类名），未将其说成3例通过；随后按真实方法名独立跑活动/现场2例，均通过。

程序化EventSystem射线+既有世界输入guard验证设置遮罩，未做物理鼠标、IME、DPI或Player构建测试。Tavern专项用合成只读快照验证绘制资格和事实不变；实际通勤/活动/现场由独立真实产品回归验证，**不宣称自然调度已随机生成Tavern场景**。

## 字体与既有成果保护

本包开始前1416文件SHA清单位于 `D:\AI_Agent_Lab\audit-results\rrpg-initialworld-productfix-20261105\preimplementation-files.json`，未覆盖旧清单。原文件无删除；必要产品/测试、Current文档、授权字体门禁/基线以及自动生成的测试截图变化均单列 `protection-review.json`/最终清单。不宣称工作区全clean或所有旧文件字节都未变。

DSH首轮未改前备份、首次身份读取曾Refresh而未先begin：这两项流程偏差保留在 `dsh-result.md`，没有以补备份掩盖。续执行前7份当前字节在 `review-before/`，主开发修复前副本在 `codex-before/`。

当前最新用户单槽15518B，SHA `56a8f2f1731a0c9d0a0475ba0134bd333adb9a9ab1b637e46c5237b0dce3e697`，测试逐字节恢复；没有回到上轮21611B旧档。SampleScene SHA `570ab047004d8d3c9ff06f91f7aea18aad05b3aaff1e064ac1ca67795228c72c`。

门禁按制作人确认v2执行：合法追加经结构/旧字形/图集像素等判定才自动归档旧基线并采纳；INCIDENT=2、NEEDS_REVIEW=4仍停止。主开发final01/02/03/04各段begin/end为0（PASS），final05 end为0（LEGITIMATE_GROWTH_ADOPTED），最终字体SHA `7b5717b976bac0d90dce70ba65b4dda0995bf78e8b3fd317ea9918058109c016`。growth/history与原两条incident保留；无手动恢复、无ClearDirty、无Git覆盖。最后独立磁盘/manifest核验见 `final-protection-state.json`。

## 制作人体验步骤

1. 在Pilot Editor运行现有SampleScene，优先Continue自己的当前存档；新游戏仍走原有流程，注意会替换单槽。
2. 默认A：活动地点不再凭空出现占位建筑；人物仍有活动与现场信息。可切B对照，再切回A确认真实建筑不消失/重复。
3. Home右上设置：保存应提示成功且留在Home；保存并返回应显示原主菜单，点Continue核对同一人物、建筑、岗位与库存；关闭菜单后世界可正常交互。
4. 管理→资源：点击信息区保持资源页；点对应Farm/Kitchen“查看岗位”，职责页定位对应实例行，右侧所选人物不变。

截图已由主开发实际查看：`screenshots/settings-menu.png`、`screenshots/resources-explicit-view-workplace.png`（已纠正早期截图误停人口页）、`screenshots/a-profile-virtual-place-hidden.png`。截图为程序化隔离世界，不是用户存档被替换后的默认现场。

本包功能与定向技术验证已交付；主观观感/物理手感仍待制作人产品验收。没有发布、关闭分支或关闭整个Initial World。
