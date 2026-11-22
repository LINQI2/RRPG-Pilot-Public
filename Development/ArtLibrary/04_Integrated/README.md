# 已接入记录

已通过既有配置链接入 **8 项 B 档实机素材**：住宅、厨房、农田、通用人物前向与背向、固定酒馆、草地、夯土路面。人物两张原图加左右镜像支持四个斜向，尚未完成步行动画。它们尚不是最终量产美术；A 档继续保留原白模。

Home共享uGUI的纸张、暖棕样式已原位更新，覆盖状态栏、导航、动态栏、设置、建造与选中详情等现有生成UI。这属于原生界面样式，不另算一份PNG；见[UI阶段记录](../07_Process/2026-11-06/Records/HOME-UI-PROGRESS.md)。动画按制作人新要求暂缓，后续采用骨骼方案，未接入本批逐帧实验。

精确资源 ID、候选来源、正式源图及 SHA 见 [接入清单](home-prototype-integration.json)。本轮校验与比例认可见 [阶段记录](../07_Process/2026-11-06/Records/home-art-integration-progress.md)。

这里保存正式路径、VisualAssetId、接线配置、构建与实机证据的索引，不复制一份素材冒充第二个正式人工来源。

新增跨场景背景：战斗主图/备用图、远征主图共3个PNG目标已原位替换，使用2张新图。Editor导入和实际引用回读通过，真实流程仍待验证，不计入上面的8项Home实机通过数。见[替换清单](cross-scene-background-replacements.json)与[阶段记录](../07_Process/2026-11-06/Records/CROSS-SCENE-BACKGROUNDS-PROGRESS.md)。

后续更新：主背景均已通过真实远征/战斗准备流程，并补上2张Journey透明环境层；共5个目标、4张新图。4个宿主完成真实显示验证，战斗备用图仍仅Editor验证。见[产品验证记录](../07_Process/2026-11-06/Records/CROSS-SCENE-PRODUCT-PROGRESS.md)。整模块、人物和HUD尚未统一。

人物后续：3张成人斜俯视身体通过既有4条外观记录接入Journey/Battle/Prep override；2张已在真实Journey和Battle准备卡显示，紫帽仅完成Editor绑定。Journey队伍道路落点与基础暖纸HUD同步更新。见[人物接入清单](person-surface-integration-r1.json)及[阶段记录](../07_Process/2026-11-06/Records/PERSON-SURFACES-UI-PROGRESS.md)。战斗白卡/HUD等仍未升级，以上不代表整模块完成。

六类敌人已接原生产链，其中3类实际准备画面验证。见[接入清单](enemy-first-pass-integration.json)与[记录](../07_Process/2026-11-06/Records/ENEMY-FIRST-PASS-PROGRESS.md)。

回合、牌堆原生纸色HUD与卡牌通用回退图已接实机；见[接入清单](battle-ui-art-integration.json)与[阶段记录](../07_Process/2026-11-06/Records/HUD-CARD-PROGRESS.md)。不等于专属插画或全HUD完成。

[卡牌头像](portrait-integration-r1.json)、[状态图标](status-icons-integration-r1.json)已按既有表现槽接入，具体实机/待触发范围见[阶段记录](../07_Process/2026-11-06/Records/PORTRAITS-ICONS-PROGRESS.md)。

主菜单/选人基础统一复用现有背景和头像，无新增PNG。真实入口与选择验证见[阶段记录](../07_Process/2026-11-06/Records/MENU-SELECTION-PROGRESS.md)。

管理和人物详情原生Prefab样式已升级，无新增PNG。动态列表/页签同色系，真实入口与限制见[阶段记录](../07_Process/2026-11-06/Records/MANAGEMENT-PERSON-PROGRESS.md)。

远征准备、事件卡、成长与结果原生样式已调整，无新增PNG；产品与模块验证范围不同，详见[阶段记录](../07_Process/2026-11-06/Records/SECONDARY-UI-PROGRESS.md)。

2026-11-08最新人物状态：六个现有人物均有各自身体与卡牌头像，韩昭不再共用林恩形象；六人真实出发、部署开战，实际手牌显示新头像通过。六类敌人均有真实出场证据，最新补狼mon_003。详细验证边界见[人物差异记录](../07_Process/2026-11-06/Records/PERSON-DIFFERENTIATION-PROGRESS.md)。

单位血条/攻击数字底板与战斗右上两图标已统一，实际受伤后26/30与填充0.8667同步，最终按钮边界通过。见[原生宿主清单](battle-unit-hud-r1.json)与[本批记录](../07_Process/2026-11-06/Records/BATTLE-UNIT-HUD-PROGRESS.md)；领地人物本轮仅审计，仍待接入。

2026-11-13：人物调整为单视角，历史NE背面资源停用归档；本批收尾、后续任务暂停。见[收尾记录](../07_Process/2026-11-06/Records/HOME-PERSONS-PROGRESS.md)。
