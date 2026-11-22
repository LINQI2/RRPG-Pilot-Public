# 人物跨表面与远征UI首轮升级 R1

2026-11-07；Target Branch：ai/rrpg-r3b-population-pilot。持续美术任务的本地阶段记录，未提交/推送，未完成全游戏首轮。

## 现有宿主与改动

- 复用 PersonVisualBindingCatalog 的4条外观记录。3张新透明PNG通过Editor导入，在原有Journey/Battle/Prep override中绑定；默认图、HeroSelect与CardPortrait保持原配置。lin_en和han_zhao原本共享绿兜帽图，本轮沿用此映射，不据姓名创造身份。
- 红披肩、绿兜帽、紫帽采用相同成人比例、东南朝向、高位斜俯视和暖插画色系。它们是基础静态身体候选，不是骨骼素材或最终肖像。
- ExpeditionJourneyView仅增加现有HUD纸色底板、文字/按钮配色及成员标签配色；保留原回调、显隐与行进逻辑。新增底板raycastTarget=false。
- SampleScene通过Editor把partyStartPosition从(-120,-150)改为(-120,-20)。相对改前磁盘快照最终仅此字段不同；原先工作区变化保留。原卡片Prefab未修改。
- 三表面Scale均为1.5。Journey/Prep原1，Battle原2；这是新正方形透明构图的首版校准，实际部署后的Battle单位仍需检查，不以准备卡显示代替战场验证。

## 验证事实

- Asset Refresh后编译完成，Console查询Error/Exception为0；git diff --check通过。
- 真实MainMenu Continue → Town.EnterProductExpedition → 原补给/队伍校验 → Confirm，Journey可见且实际2人，读取绿兜帽和红披肩新图。
- 暂停按钮调用生效；截图journey-person-ui-r1.png显示脚底落在道路内，状态栏和3按钮颜色统一。Continue/Return当时interactable=true；本批没有执行Return保存或声称完整交互回归。
- 从当前远征实际可用Battle节点经原HandleNodeClick进入Battle准备，initializedFromExpedition=true。battle-person-r1.png证实两张准备卡读取新图，但旧白卡仍造成文字/立绘重叠，是下一批明确缺口。
- 紫帽当前没有进入这次真实两人队伍，仅Editor绑定与资源回读通过。尚不标记它已通过产品显示。
- 未验证部署、攻击、结算、返程及其他分辨率；没有制作动画。
- 保存文件SHA仍为29600FBC09239F6BC3C8AC423B1C586996D679AF2618E47F30EF047A8F17EF10。测试前备份到忽略的Unity Temp；退出Play丢弃测试远征，未保存。最终Play=false、Compiling=false、SceneDirty=false。

## 资产与证据

原始生成缓存和完整PNG保留；候选放在02_Candidates/Journey/Persons，Unity副本位于Assets/Art/Characters/ArtUpgrade01。提示词见PROMPTS-PERSON-SURFACES-R1.md。精确SHA及验证范围见04_Integrated/person-surface-integration-r1.json。

修改前配置与场景的只读备份在08_ExistingProject/person-binding-before-r1.txt和journey-scene-before-r1.txt，不作为正式生产Authority。Evidence/prep-person-r1.png是在同一调用中请求截图后切换界面，帧末捕获时机不可信，不作为准备界面证据；可靠截图为journey-person-ui-r1.png和battle-person-r1.png。

## 下一步

先处理战斗准备白卡文字重叠、基本HUD与敌方/地面槽位风格，再补主菜单、选人、管理子页、卡牌与图标。村庄局部精修后移，动画继续暂缓。全模块完成首轮统一后再整体细化。
