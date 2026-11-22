# 战斗意图占位清理与密集标签排布 — 2026-11-13

Target Branch: `ai/rrpg-r3b-population-pilot`。继续整体UI首轮统一，人物/玩法不变，无新增贴图，无commit/push。

## 已实施与验证
Editor读取确认UnitCard/Anchor_Intent/Image_IntentIcon默认残留红色意图2贴图；我方未调用敌方意图绑定器，因此错误显示该占位。通过Editor修改原Prefab的该Image：sprite=null、enabled=false，保留原宿主和敌方运行时绑定器。

真实Continue → Town远征 → 原确认出发 → 实际战斗选项 → 原拖拽布阵/开战：我方两人图标均False/none，敌方五人均True/icon_attack_r1。原结束回合后状态保持，说明没有隐藏敌方实际意图。截图battle-intent-placeholder-r1.png记录该轮。

五敌人密集排列发现上一批射程标签右侧位置与邻近敌方意图相碰，后将Scene与.cs默认enemyRangeOffset由(35,-93)调整为(-104,-93)，放至攻击数字左侧。范围仅该1个表现参数。Refresh后编译已结束，最终版本已从真实Continue进入远征准备；随后用户中断。**最终位置尚未开战复验，不标记该布局通过。** 截图仍为改位前版本，不能冒充最终效果。

## 收尾与恢复入口
中断时仅在准备界面，已请求退出Play，未按返程/确认新游戏/游戏存盘。当前测试基线SHA256：B310D4A255E091ABF9F25E30E88A9A8669DCC618BD3F1D2729A5361EE77D0A63，备份在Unity忽略Temp/ArtIntentUI-<SHA>.original。

改前备份在08_ExistingProject/BeforeIntentUI-20261113，包含当前批修改前Prefab、Scene与BattleUIEnemyIntent.cs；之前已有工作保留。Unity序列化修改均通过Editor/MCP。Unity Insight同步成功；第一次实际意图显示检查基线Console 0。

待恢复：确认Editor/存档状态 → 真实Continue/出发/战斗 → 检查(-104,-93)射程标签在密集阵形中的边界与意图图标 → Console → 退出Play/核对存档。无需新增资源或扩大人物范围。

中断收尾实测：Play=False，SaveUnchanged=True，Scene Dirty=True。未为清理状态保存或重载Scene，保留未保存编辑器状态；恢复时先核对该差异。相关.cs的git diff --check通过。

## 恢复验证完成（2026-11-13，取代上文待验证状态）
用户明确继续后，Editor仍为EditMode、Scene Dirty=True。通过Editor保存Temp诊断副本，与磁盘Scene只读diff无差异，因此安全重载已保存Scene；确认offset=(-104,-93)。当前磁盘存档重新备份后开始本轮真实验证。

真实MainMenu Continue → Town远征 → 原确认出发 → 实际遭遇选项 → 原拖拽布阵 → 原开战：本场为同列3个敌人的三槽密集排布。3个射程标签均有效，文字溢出0；用实际RectTransform屏幕边界逐对比较所有活动射程标签与敌方意图图标，相交计数0。截图battle-range-dense-final-r2.png显示最终位置。本轮没有重现5敌人组合，不将历史5敌人图冒充最终布局截图。

原结束回合后回到PlayerTurn：我方可见意图图标0、敌方可见图标3、射程标签3、文字溢出0。截图battle-range-dense-refresh-r2.png。敌人失能/防御/特殊意图本轮未触发，不能声称这些状态已验收。

Console基线与最终Error/Exception均0；相关.cs git diff --check通过。未新增实现变更，仅完成上次最终版本复验与记录。最终Play=False、Compiling=False、Scene Dirty=False，offset与Prefab空意图图读回正确。SaveUnchanged=True，SHA256仍为B310D4A255E091ABF9F25E30E88A9A8669DCC618BD3F1D2729A5361EE77D0A63。未执行返程或存盘，无需恢复旧存档。

本批实现与已述产品验证已完成，整体美术升级仍进行中。Target Branch仍为ai/rrpg-r3b-population-pilot；未暂存、commit或push。继续优先覆盖其他常用界面，人物单视角范围不变。
