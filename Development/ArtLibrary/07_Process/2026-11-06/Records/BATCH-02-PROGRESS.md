# 第二批：固定酒馆、世界人物母样与确定性素材预览

目标分支：ai/rrpg-r3b-population-pilot；日期2026-11-06。沿用制作人已采用的01温暖插画、R4较高视角与统一斜向网格。未Commit/Push，未修改Unity代码、配置、场景、Prefab、字体、存档或原游戏美术。

## 新源图与审查

| 文件 | 本轮结果 | 保留边界 |
|---|---|---|
| tavern_place_r1.png | 与住宅同家族的屋顶/墙面/光照，杯形招牌，不含火烟/人物或工作状态；1254×1254透明PNG | 固定Place，不能因视觉像建筑就新增BuildingInstance、建造/岗位/经营规则；投影、占地提示与显示尺度需实测 |
| person_world_neutral_sw_r1.png | 普通成人西南向空手静态母样，头顶/肩面可见，轮廓与衣裤分区清楚；1254×1254透明PNG | 不绑定PersonId或将全村改成同一外观；不是角色定稿、多方向或动作资产；头身比例/幼态感与小尺寸仍待产品审查 |

两图先进入00_Inbox，经目视与Alpha抽样后转02_Candidates的Places/Persons，保留缓存哈希和真实提示词。主体Alpha普遍约253，边缘存在少量低Alpha，未冒称正式质量/运行接线通过。原正式素材保持原位。

## 独立PNG组合预览

09_Previews/village-grid-preview.html直接引用住宅、厨房、农田、酒馆和人物5张源图；5栋住宅复用同一图，按u=(55,29)、v=(-55,29)摆放。支持网格/占地提示/标签开关、选择对象、调整同类尺寸与图内地面锚点、导出本地校准JSON。

9个示例对象包括5住宅+厨房+农田+固定酒馆+1通用人物母样；不是一份正常存档、不是人口快照。Tavern框和全部位置是美术工作值，不定义建造规则。导出JSON不会写游戏，也不属于正式Compiler配置。

静态检查通过：完整JS语法、5个本地PNG引用存在、9对象/5住宅、u/v轴线数据。浏览器file协议被工具URL策略拒绝，没有改用其它浏览器、临时服务器或其它绕过方式。**浏览器视觉/交互未验证**，不把静态检查冒称浏览器通过。用户可将此文件作为本地预览自行打开；没有对用户浏览器或系统设置作修改。

## 与真实实现的接缝

本轮只读源码用于接线规划，不替代实时Editor/Inspector核验：

- HomeBuildingPlacement.cs中的BuildingWorldMapper.ToPresentation当前只有WorldOrigin平移，ToLocal是反向平移，未实现统一斜向投影。
- HomeWorldEntityView.ApplyBuilding的A表现按Width/Depth缩放整张Sprite；B原型保留图宽高比。直接把大留白的新PNG替换进去，不能证明实际主体尺寸/脚底正确。
- Building占地、BoxCollider2D、HomeBuildingFootprint与点击拾取仍依赖当前坐标；若启用斜向投影，要同范围核对显示映射、逆映射、拾取、施工预览、人物/Place锚点与深度排序，不能只换图。
- Home人物走village.person状态表现；PersonVisualBinding的Journey/Battle/Prep/HeroSelect/CardPortrait是另一消费入口。世界视角PNG不应作为所有表面的默认立绘直接通用。
- InitialPopulation已有5个PersonDefinitionId，另有Hero。此母样不为这些人物擅自指定性别/年龄或改身份定义。

当前会话仍没有Unity专用MCP工具，因而没有正式接线、编译或Play验证；未绕过仓库序列化资产约束。

## 后续最小推进

先以预览检查房屋/酒馆相对尺度、人物头身/脚底、小尺寸识别；根据人物审查制作身份差异和必要方向。素材及投影约束清楚后再进入真实Home宿主的窄接线工作。UI/卡面/战斗/旅程仍在全面升级范围，不因本批两张源图而判整包完成。
