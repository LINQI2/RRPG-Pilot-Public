# 人物差异与头像首轮接入 — 2026-11-08

Target Branch: `ai/rrpg-r3b-population-pilot`。本批已接入并验证；整体美术升级未完成，未进行最终美术Acceptance，未commit/push。维持先全面统一、后统一细化；不做动画。

## 本批变更与正式宿主
复用 PersonVisualBindingCatalog → 原 PersonVisualResolver / PersonVisualHostBinder / BattleCardPresentationResolver，不新增产品路径。通过Editor导入四张PNG并修改目录的6个sprite引用：
- person_han_zhao：Journey/Battle/Prep原绿色弓手替换为黑发低马尾深青弓手，CardPortrait替换配套头像。
- person_qiu_shuang / person_wei_an：各补一个专用CardPortrait，与上一批赭黄/青灰身体图匹配。
其余目录行、DefaultBody、offset、scale、HeroSelect保留不变；修改前备份和当前资产只读diff确认恰为上述6处引用。
这些外观仍为美术提案，不新增年龄/性别/人物故事设定，不改PersonDefinition、Runtime PersonId、职业、牌库规则或数值。
未修改.cs、正式Scene、Prefab、XLSX或其他SO。Unity Insight任务开始同步成功（日志目录EPERM但索引同步完成）。

## 验证与证据
- Editor实际读取项目路径、EditMode及Clean Scene，Console Error/Exception基线0。
- 真实 MainMenu Continue → Town.EnterProductExpedition → 原四个选人按钮（苏晚/韩昭/邱霜/魏安，林恩默认已选）→ 原确认出发。
- `expedition-six-distinct-persons-r1.png`：全部6人身体不同，韩昭与林恩区分明显，卡名/数值保留。
- Journey实际激活6个Body分别是green/wei/qiu/han/purple/red新图；原战斗遭遇选项进入Battle。
- 原BattlePrepCard拖拽：韩昭/邱霜/魏安前排、林恩/苏晚后排、英雄中排，原StartBattle开战，Started=True、Phase=PlayerTurn。
- `battle-distinct-portraits-turn1-r1.png`：首轮真实手牌6张；实际OwnerPersonId与头像对应：韩昭“独猎”“猎残”深青头像、邱霜“挥砍”赭黄头像、魏安“瞄准射击”青灰头像、苏晚“补位”紫帽头像、林恩“切配”绿帽头像。没有人工替换牌库或伪造卡牌展示。
- 同一真实战斗中active Body_Visual确认全部6个人物身体；附带3个enemy_mon_003狼和enemy_mon_005施法者。补齐003真实出场证据，当前六类敌人均已有真实出场历史证据（不等于本轮重验全部六类，更不代表攻击/动画/结算完成）。
- 本轮未执行出牌、结束回合、战斗结算、返程、存档或NewGame确认。
- Console最终0 Error/Exception；无源码改动，不要求新增测试。
- Play退出后5个自动Layout RectTransform坐标漂移。先通过Editor写忽略Temp诊断副本，与原磁盘Scene只读diff确认仅5处坐标；通过Editor重载已保存Scene丢弃本轮布局漂移，未保存到正式Scene。最终Play=False、Compiling=False、Dirty=False，6行新绑定均回读保留。
- 存档SHA256始终 `2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF`，SaveUnchanged=True。当前基线在忽略Temp另有备份，未恢复旧存档。

## 归档
4张新图均使用built-in image_gen，原缓存保留；Candidates和Unity项目各保存一份。
- 提示词：[PERSON-DIFFERENTIATION-PROMPTS.md](PERSON-DIFFERENTIATION-PROMPTS.md)
- [身体清单](../../../04_Integrated/person-surface-integration-r1.json)、[头像清单](../../../04_Integrated/portrait-integration-r1.json)、[敌人清单](../../../04_Integrated/enemy-first-pass-integration.json)记录精确hash和证据范围。
- Catalog与现有项目盘点更新：58条＝56张ImageGen＋2几何导引；归档与导入PNG/meta hash通过，187导航链接通过。Ready=0、FinalArtAccepted=0。
- 原目录备份：08_ExistingProject/PersonVisualBindingCatalog-before-person-differentiation-20261108.asset.backup。还原序列化资源仍经Editor操作。
- 既有身体/头像资源保留；历史共享绿色身体的旧清单不再代表韩昭当前配置，不删除林恩仍在使用的图。

## 当前缺口与下一批
6个现有人物已有各自身体与专用卡牌头像，首轮主体差异可用，脸型等相似性留统一细化。领地人物仍使用通用身体；战斗旗帜/设置按钮、血条、伤害和部分边框仍为旧风格；专属卡牌插画未覆盖。普通事件/岔路真实触发、结果真实结算仍待补验。继续优先清除这些未覆盖模块，不做动画和单点精修。

