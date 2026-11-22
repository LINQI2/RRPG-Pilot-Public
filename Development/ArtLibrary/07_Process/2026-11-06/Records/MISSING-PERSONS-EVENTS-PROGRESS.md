# 人物缺口与事件插图首轮接入 — 2026-11-08

Target Branch: `ai/rrpg-r3b-population-pilot`。本批继续全面统一，不代表整体美术升级完成；未 commit / push，未请求最终美术 Acceptance。

## Existing Host Map 与实施范围
- 复用 PersonVisualBindingCatalog → PersonVisualResolver → 原 Prep / Journey / Battle 宿主；原4行保留，新增 person_qiu_shuang、person_wei_an 两行，三个身体 override 均1.5倍，offset零。DefaultBody、HeroSelect、CardPortrait为空；没有将全身图冒充专用头像。
- PersonDefinitionCatalog 确认两人已有精确身份键和卡绑定，但没有年龄/性别外观设定。赭黄旅行装与青灰持盾装均为美术提案，未修改身份、职业、卡牌、数值或存档。
- 复用 SampleScene 的 ExpeditionJourneyView 三类默认事件插图。Editor 查询 JourneyEventPresentationCatalog 当前无有效逐事件配置行；保留原 resolver 和逐事件优先逻辑，不新增事件玩法或新宿主。
- 新增5张PNG均通过 built-in image_gen 生成、保留原缓存，归档 Candidates；尚无最终验收。完整提示词见 MISSING-PERSON-BODIES-PROMPTS.md、EVENT-ILLUSTRATIONS-PROMPTS.md。
- 普通遭遇按钮和战斗按钮移除程序实现说明副标题，保留原 Label/Role/NodeId/回调、正文与规则。

## 本轮验证
- Console 基线0 Error/Exception；修改.cs后 Refresh，编译结束后及产品流程后仍0 Error/Exception。
- 真实 MainMenu Continue → Town.EnterProductExpedition → 原按钮选邱霜/魏安 → 原确认出发。
- prep截图两人灰色身体已替换；Journey实际激活身体分别为新图，队伍4人。
- 真实自动遭遇战斗 → 新 ruins 插图 → 原 BtnOption_0 → Battle prep → 原拖拽接口部署4人 → 开战。Started=True，邱霜/魏安 active Body_Visual 均为新图。未执行攻击或结算。
- 最终移除副标题版本另经真实 Continue → 默认队伍出发 → 自动战斗遭遇复验，截图最终 Title/Body/Option不重叠，活动文本只有标题、正文和“进入战斗”。
- 普通遭遇、岔路：原 BuildEncounterCardConfig / BuildBranchCardConfig → 原 JourneyEventCardView.Render 显示通过，测试节点明确标记 art-module-only，截图标题标明模块检查、无推进回调。**不记为真实进度触发通过**。
- 退出Play后曾有5个自动布局RectTransform坐标漂移导致Dirty。先经Editor存Temp诊断副本，与正式磁盘Scene差异仅这5项；经Editor重载已保存Scene丢弃本轮自动布局漂移，不改正式磁盘。最终再次读取：Play=False，Compiling=False，Dirty=False；三图引用和6行人物目录均保留。
- 存档全程保留当前基线，SHA256 `2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF`，SaveUnchanged=True。未点 Return / Retreat / Save / NewGame Confirm。
- ArtLibrary校验54条＝52张ImageGen +2导引，所有归档hash一致、178导航链接通过，Ready/FinalArtAccepted仍0。新图PNG与meta额外由integration manifest校验。
- git diff --check：.cs无空白问题；Scene仍包含Unity序列化空字段尾空格提示，不手工改YAML或清理原有未提交改动。

## 证据与回滚
- expedition-missing-persons-r1.png
- journey-event-ruins-product-r1.png（4人、原副标题版）
- battle-missing-persons-r1.png（原部署后真实开战）
- journey-event-final-product-r2.png（最终副标题清理版）
- module-event-content-r1.png / module-event-branch-r1.png（仅模块）
- [人物清单](../../../04_Integrated/person-surface-integration-r1.json)、[事件插图清单](../../../04_Integrated/event-illustrations-r1.json) 是精确资产与验证范围清单。
- 08_ExistingProject 中保留修改前 PersonVisualBindingCatalog 与 SampleScene 备份；恢复序列化内容仍须Editor操作，不能直接覆盖YAML。原三张事件PNG没有删除或覆盖。

## 下一步
继续首轮补齐：韩昭与林恩当前仍共用绿色身体；邱霜/魏安缺专用卡牌头像；领地人物仍通用身体；战斗旗帜/齿轮/血条/伤害等旧图标边框和专属卡牌插画仍待统一。普通事件/岔路待真实进度触发补验，战斗结果仅历史模块证据；暂不做动画和单点精修。

