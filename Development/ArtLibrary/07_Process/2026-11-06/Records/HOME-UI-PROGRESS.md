# Home 共享界面美术阶段记录

Target Branch：`ai/rrpg-r3b-population-pilot`。持续升级，未关闭全面美术任务；未提交、未推送。

## 本轮范围

制作人要求动画先不做，后续使用骨骼动画。因此两张逐帧实验分别进入失败/实验目录，提示词和缓存原件保留，**没有新增动画配置或运行代码**。

复用现有Home uGUI宿主及回调，原位升级 `HomeProductUI`、`HomeProductShell`、建造目录、选中详情。纸张底色、深棕文字、暖棕选中态、圆角边框与轻阴影，动态栏增加背景和标题，建造模式突出对应导航项。顶部改为横向拉伸锚定。保留原CanvasScaler的1920×1080参考分辨率和ScaleWithScreenSize。

使用原生uGUI几何 `HomePanelImage` 实现圆角，不生成额外栅格素材，不建立另一套产品入口，不手写Unity序列化文件。该共享UI样式同时用于A/B；世界美术仍保留原A/B切换。

动态栏仅在显示层用当前人物快照的精确PersonId→DisplayName映射替换已知内部ID，不修改事件原文、人物身份、历史或存档。

## 验证与证据

- Editor实读确认正确RRPG工程、SampleScene、既有HomeShell、Canvas与根节点；遵循Unity UI / uGUI技能，复用既有层级，不重建场景或Prefab。
- Refresh后无新增编译/产品错误。Console保留已知夹具异常，以及一次内置UI Sprite探针未找到资源的记录；生产代码没有该资源依赖。
- 真实MainMenuUI.OnContinue进入现有Home。`home-ui-pointer-verification.json`及五份final-pointer-step回执：经EventSystem真实射线和pointerClickHandler点击，验证卡牌/历史开关、建造目录/选择/取消、设置开关、透明遮罩、世界点击阻断、装饰背景不阻断点击、建造高亮及HUD在视口内，全部通过。
- 探针前后模拟SaveData一致；未调用确认建造、保存或新游戏。
- 初版夹具在打开面板同帧立即点击关闭而误报。实读下一帧命中关闭按钮正常，改为跨真实玩家帧五步核验后通过；保留失败回执，不将其伪称产品修复。
- 本轮已目检1920×1080实际GameView：`home-ui-r2-game.png`、`home-ui-r2-settings.png`、`home-ui-r2-build.png`。其他分辨率尚未实机验证。首帧状态颜色可能处于按钮过渡中，不把截图单帧当作完整交互验收。

## 仍未完成

这是Home共享样式首轮，不代表人物/管理等全部已著作Prefab子页、图标、卡牌、远征或战斗已完成替换；最终UI艺术母版尚未冻结。继续建筑/环境细节、人物外观差异和子页统一，动画按骨骼路线暂缓。
