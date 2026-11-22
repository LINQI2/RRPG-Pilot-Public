# 常用界面统一：射程标签 — 2026-11-13

Target Branch: `ai/rrpg-r3b-population-pilot`。用户确认“人物”为“任务”的笔误，恢复整体美术升级，以常用稳定UI为主、先整体后局部；人物保持单视角。本批不生成角色或扩展素材，未commit/push。

## 实施
复用BattleUIEnemyIntent.cs的原Text_Range动态宿主及当前AttackRange数据：默认过高文本框改为86×30，移到攻击数字旁(35,-93)，16号墨色字，原生HomePanelImage暖纸底板，不截获射线。射程为空时底板同步隐藏，不修改射程规则。默认值和现有Scene Inspector配置同时调整；Scene通过Editor/MCP保存，没有手改YAML。

范围：1份.cs和SampleScene上BattleUI的3个表现字段；人物、身份、数值、卡牌、贴图均未改变。改前备份位于08_ExistingProject/BeforeRangeUI-20261113。Unity Insight同步成功；沿用uGUI skill增量修改原宿主。

## 验证
- Console基线0；Refresh后编译成功，相关.cs的git diff --check通过。
- 原MainMenu Continue → Town远征入口 → 原确认出发 → 实际战斗遭遇选项 → 原拖拽布阵 → 原开战按钮。
- 实际敌方出现射程1与射程2，位置/尺寸正确，isTextOverflowing=False，纸色底板显示；截图battle-range-ui-r1.png。
- 原结束回合按钮后回到PlayerTurn，两个标签刷新正常且仍不溢出；截图battle-range-ui-refresh-r1.png。未触发敌人失能，空标签隐藏分支仅实现、未在本轮真实战斗触发。
- Console最终0 Error/Exception。退出Play后对Temp诊断Scene与磁盘Scene比对，仅5个既有自动布局坐标漂移；通过Editor重载已保存Scene丢弃漂移。最终Play=False、Compiling=False、Dirty=False，RangeOffset/Font回读正确。
- 当前磁盘存档SHA256 B310D4A255E091ABF9F25E30E88A9A8669DCC618BD3F1D2729A5361EE77D0A63，本轮SaveUnchanged=True；测试前备份于忽略Temp。未执行返程、游戏存盘或NewGame确认。

## 范围状态
这是全局UI首轮统一中的一组，不代表整体完成。红色提醒标及其他尚未统一的常用界面元素仍待覆盖；保持现有宿主、数据来源与可替换资源槽，再集中局部细化。此前暂停状态已由本轮用户明确继续解除，人物多视角禁用决定继续有效。
