# 经营游戏镜头调研与错误纠正

日期：2026-11-06。制作人纠正：上一轮建筑图不符合游戏逻辑、视角太平视；要求先寻找模拟经营美术图，解释错误并持续推进。

## 错误与处置

1. 已获持续推进授权，却在完成少量源图后停下。Unity接口缺失只能阻止实机接线，不阻止市场调研、艺术纠偏和素材迭代。
2. 将概念板的画风认可扩大成镜头与建筑尺度认可。制作人只选01画风；它的低视角、构图与假UI均未获认可。
3. 提示词写约35度俯视，却没有定义从水平面还是竖直方向测量；更没有根据生成结果判断实际投影。数字不能代替可见几何验收。
4. 先做立面细节丰富的单栋展示图，缺少同镜头地面、占地、人物、邻栋遮挡的校准。单图好看不能证明适合经营游戏。
5. 在Alpha检查后将源图称为候选，却没有优先淘汰错误视角。厨房还烘焙火焰，主体留白也不一致。

`kitchen_01.png` / `house_01.png` **REJECTED FOR GAMEPLAY USE**，保留为错误证据，不登记、不替换原产品资源。后续用_r2等版本名新生成，不覆盖历史。

## 本次实际观看的市场图

使用浏览器实际查看图像，不仅阅读搜索摘要。截图均为参考作品，不作为RRPG资产，不复制其建筑、角色或UI。

| 作品/样本 | 图像来源 | 观察与采用边界 |
|---|---|---|
| Norland | 官方Steam商店，截图#5；本次实际src见下方 | 高位管理视角、去顶室内、地面与人物关系清楚。可学习共同体空间信息层级；不因此给RRPG新增室内/去顶系统 |
| Against the Storm | GamePro截图，1440×810 | 从高处读建筑顶面、道路、资源和角色；同地面尺度组织经营。学习占地与作业区可读性；不继承画面暗度/密纹理 |
| Foundation | IndieDB早期Build 0.0.0截图，1920×1080 | 高位远景中聚落布局与建筑顶面优先；不是当前版本全面评测，也不照搬人物过小的总览距离 |
| The Wandering Village | WindowsCentral评测近景，1280×720 | 本样本视角较低、立面明显，恰好说明不能仅凭“经营游戏”名称就选镜头。可研究接地与2D人物，但此近景不作为RRPG管理镜头目标 |

来源：
- Norland官方页面：https://store.steampowered.com/app/1857090/Norland/
- Norland本次所见图片：https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/1857090/7d8eb8d6c91befe2084fef28f356f6d1e8aaeef5/ss_7d8eb8d6c91befe2084fef28f356f6d1e8aaeef5.1920x1080.jpg?t=1793358286
- Against the Storm来源页：https://www.gamepro.de/galerien/against_the_storm%2C135741.html
- 实际图：https://images.cgames.de/images/gsgp/287/against-the-storm_6160662.jpg
- Foundation来源页：https://www.indiedb.com/games/foundation/images/image-13
- 实际图：https://media.indiedb.com/images/games/1/69/68890/ss_f5402c4dfb2975fa967fb10817687e0cd.jpg
- Wandering Village来源页：https://www.windowscentral.com/gaming/pc-gaming/the-wandering-village-pc-review
- 实际图：https://cdn.mos.cms.futurecdn.net/zqxhDFHmDdAfWSE3AzcBq3.jpg

## 回到RRPG实际规则

本次只读生成镜像（不修改）：BuildingDefinition的House/Kitchen均2×2，Farm为3×2；InitialBuilding为Farm1/Kitchen1/House5。Tavern是固定Place，不新增可建造/经营规则。普通场景不能用概念图编造Workshop、额外生产链或房屋容量。

已有正常路径截图 `Development/StudioRuns/FP-NormalPath-20261106/screenshots/01-newgame-direct-home.png` 显示当前A白模5屋、Farm、Kitchen与Tavern；是历史运行证据，不是本轮实时Editor核验。新美术必须适配正式世界空间与实际占地，不因图好看重排保存坐标。

## 本轮改进方法

- 先出**单一Home视图的空间校准图**，保留01暖插画材质，丢弃原镜头；不把人物立绘/战斗插图拼进来分散验收。
- 明确高位鸟瞰正交候选：向地平面下俯60度（距竖直30度），只是本轮实验值，不设为Unity正式相机参数。
- 可见几何标准：无天际线/天空/远景透视；近远同型建筑同尺度；屋顶与地面水平面优先，立面压缩；能看见角色头顶与肩；地面前后进深明确；建筑间人物不被大立面普遍挡住。
- 单体先对齐2×2主体占地、接地点与一致留白；屋檐允许视觉伸出，但装饰不能暗增碰撞/占地。Farm按3×2验证。
- 厨房静态底图不含火/烟/人物，不暗示一直工作。动画与活动状态随后使用现有表现接口。
- UI文字、选择框和真实模拟规则依旧由产品组件承担；不从概念图直接抄。

下一步持续做空间校准→高俯视建筑源图→高俯视世界人物样本→同尺度组合检查。Unity接口可用后再完成运行接线；接口缺失期间不重复要求同一授权。
