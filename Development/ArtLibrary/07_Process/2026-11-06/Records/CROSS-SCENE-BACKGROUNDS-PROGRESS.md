# 远征与战斗首轮背景替换

2026-11-07。Target Branch：`ai/rrpg-r3b-population-pilot`。未提交、未推送；不代表全模块首轮通过。

后续已完成主背景真实流程验证，并继续替换两张远征环境层，最新事实见[产品验证记录](CROSS-SCENE-PRODUCT-PROGRESS.md)。下文保留本批首次导入时的范围与限制。

## 目标与原宿主

按制作人要求先全局统一、再统一细化。本批不精修村庄、不制作动画。Editor实读BattleUI与ExpeditionJourneyView：战斗使用草地森林背景，另有备用占位背景；远征使用旧平视道路背景，仍存在独立近远景占位层。两个背景Image均为Simple、白色、非preserveAspect，既有布局保留。

内置image_gen生成两张低干扰、高位俯视、温暖插画背景，均为1672×941：

- `02_Candidates/Battle/Backgrounds/battle_meadow_highview_r1.png`
- `02_Candidates/Journey/Backgrounds/journey_trail_highview_r1.png`

完整原始提示词：`Prompts/PROMPTS-CROSS-SCENE-BACKGROUNDS-R1.md`。缓存原件保留，PNG没有经过图像脚本编辑。

## 替换及检查

按本领域既有Unity直接Sprite来源，原位换三份PNG：战斗主背景、战斗备用背景、远征主背景；不另建Catalog，不改场景或Prefab YAML，不修改玩法。原PNG备份于 `08_ExistingProject/BeforeCrossSceneFirstPass`。精确目标、原新SHA和Meta检查见 `04_Integrated/cross-scene-background-replacements.json`。

- 替换前核对Git目标无既有未提交变化，且PNG与原盘点SHA一致。
- 通过Editor ImportAsset导入，三份Sprite均可解析，回读实际宿主与备用Sprite纹理均为1672×941；Meta未变，场景isDirty=false。
- 证据：`journey-battle-background-bindings.json`、`background-image-host-properties.json`、`cross-scene-background-import.json`、`cross-scene-background-reference-readback.json`。
- 本轮没有修改C#、保存/重置玩家数据或强制激活另一套战斗流程。Editor导入与引用检查通过，**真实玩家远征/战斗流程尚未验证**，不得标为Product Integration Verification通过。

## 下一步覆盖缺口

先跑真实远征/战斗入口验证新背景与既有阵位/界面兼容，再继续近远景层、共享人物、敌人、卡框/图标/HUD的第一轮统一。已生成背景不继续加密细节；不把两个背景替换等同两整个模块完成。
