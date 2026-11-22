# 人物朝向阶段进展

Target Branch：`ai/rrpg-r3b-population-pilot`。持续任务记录，不是最终交接或工作包关闭；本任务未提交、未推送。

## 素材与产品接入

内置 image_gen 编辑已认可比例的通用前向样本，生成 `person_world_neutral_ne_r1.png`。候选原图位于 `02_Candidates/Village/Persons`；正式源图位于 `Development/SourceAssets/Village/ArtUpgrade01`。缓存原件保留。完整提示词见 `Prompts/PROMPTS-PERSON-NE-R1.md`。

通过既有共享 VisualAsset / VillagePresentation 配置链登记 `va_village_r5_person_ne` 和 `village.person.r2.ne`。复用 HomeWorldEntityView：按实际表现位移选择前/背向，再水平镜像形成另外两个斜向；周期刷新和选中轮廓保持一致。B档移动时保留服装原色，A档原行为保留。

这是两张原创静态视图加镜像的方向样板，**不是步行动画完成**。没有任意给命名人物指定性别、年龄或外观，也没有改变 PersonId、存档结构、移动速度或到达事实。

## 验证

- Compiler build / validate、Editor Builder 通过，0 error、9条既有配置 warning。Refresh 后没有新增编译/产品错误，已知夹具异常原样保留。
- `person-facing-runtime-verification.json`：真实 MainMenuUI.OnContinue 进入 Home 后，斜网格/预览回归通过；真实人物视图的四象限方向与镜像、选中轮廓、周期刷新通过；模块探针前后模拟 SaveData 相同。探针结束恢复视图状态。
- `person-facing-live-renderers.json`：随后正常产品 Update 实际产生了前向、背向及前向镜像；不以模块探针冒充自然运行。截图为 `home-after-game-r4-facing.png`。
- 最后通过既有暂停逻辑暂停当前 Home B 实机样板，不保存玩家存档。

## 配置包保护修复

发现 Artifact Tool 导出丢失原表列宽与冻结行。继续由 Artifact Tool 产生目标单元格内容，再用包级保留工具把**指定目标单元格**装回升级前原始XLSX；所有非目标ZIP部件逐字节不变，目标Sheet的其他内容保持一致，所有单元格值/公式与Artifact输出一致。保留原有命名空间兼容声明，OpenXML及Compiler最终校验通过。结果见 `workbook-package-preservation.json`。

`Preserve-WorkbookPackage.py` 只补充原生格式保留能力，禁止编辑游戏序列化资产。此前包组装夹具的 CRC/命名空间问题已修复，不把中途失败记为PASS。原PNG和原XLSX备份保留，临时依赖Junction已解除。

下一阶段：步行动作样板和身份差异；尚未完成最终美术量产或全游戏替换。
