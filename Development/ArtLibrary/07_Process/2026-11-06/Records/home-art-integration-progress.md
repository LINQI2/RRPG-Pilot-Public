# Home 美术接入阶段记录

日期：2026-11-06。Target Branch：`ai/rrpg-r3b-population-pilot`。本任务未获 Git 提交/推送授权；无提交、无推送。本文件是持续工作记录，不是最终交接或任务关闭声明。

## 现有宿主与范围

复用 `ProductHomeWorldHost`、`HomePresentationResolver`、`HomeWorldEntityView`、`HomeBuildingPlacement` 和现有 A/B 切换。默认 A 保留，B 使用统一斜向投影；没有创建另一套玩家入口。

源图经 Development/SourceAssets → ConfigCompiler → Mirror → Editor StaticConfigBuilder → Shared VisualAsset → Home 消费。酒馆新增独立 VisualAsset 及 B 表现键绑定，仍是固定 Place。没有更改 Save 字段、存档坐标、建筑定义占地或模拟规则。

已接入八项样板：住宅、厨房、农田、通用成人前向与背向、固定酒馆、草地和夯土路面。四份原 R2 PNG 和两本原 XLSX 已备份于 `08_ExistingProject/BeforeHomeUpgrade-2026-11-06`。三个人物差异候选未任意绑定到命名人物。

## 本轮验证

- 真实 MainMenuUI.OnContinue 进入现有 Home，7 座建筑、6 名人物、1 个固定场所。
- Compiler build 与 validate 通过，0 error、9 条既有配置 warning；Editor Builder 成功。代码 Refresh 后 Console 无 Error/Exception。
- 现有 EditMode 共享视觉解析与建筑放置基础测试 4/4 通过。
- 真正 Home 内 4 次 A/B 转换、308 次坐标往返、13 个持续实体身份、7 座建筑斜向碰撞框及建造预览锚点验证通过。最大坐标误差 0.0003252，预览锚点误差 0.0009766。未调用确认建造或保存。
- 截图：`Evidence/home-after-world-r2.png` 是世界相机；`Evidence/home-after-game-r2.png` 是 1920×1080 完整 GameView。
- 比例认可后的地面细化：`Evidence/home-after-world-r3.png`、`Evidence/home-after-game-r3.png`。4 个地面网格使用托管纹理，土路边缘包含透明到全不透明的渐隐；纹理保持原始 PNG，不用图像脚本修改。实际显示已检查，未发现突出的硬拼缝；不把目测等同于数学无缝保证。
- `home-terrain-runtime-verification.json`：地面接入后重新验证 A/B、308次坐标往返、碰撞框和预览锚点，通过。`home-screen-selection-verification.json`：经真实屏幕坐标选中农田 Building 与固定酒馆 Place，通过。
- 屏幕点击夹具首次假定建筑锚点无人遮挡，实际选中经过该点的人物。诊断已确认人物优先选择符合原产品策略；夹具改为挑选未遮挡点后通过，未为通过测试改变人物优先策略。
- 截图夹具首次在 Continue 同一帧检查 WorldVisible 而失败；已改为等待真实产品 Update 后捕获，后续通过。保留失败回执，不归为产品缺陷。
- 使用内置 Artifact Tool 更新两本 XLSX，回读检查非目标区域值/公式 5112 与 5118 项保持一致，Compiler 验证通过。Windows 原生表格图片渲染在沙箱内、外均直接退出 code 1；未声称表格渲染通过。此限制不影响 Unity 美术截图验证。
- 地面资源登记追加两行共享 VisualAsset，回读非目标区域值/公式5104项不变。再次 Compiler build / validate、Editor Builder、Refresh 和实机地面验证通过。Console 中保留两类已说明的夹具异常；过滤这两类已知消息后，没有其他 Error/Exception。
- 停留状态：真实 Home B 档 Play，使用现有游戏暂停逻辑暂停模拟，场景未脏、未编译；未保存或重置玩家存档。临时依赖目录 junction 已解除，内置依赖未修改。

证据位于同级上层 `Evidence`：`home-final-runtime-verification.json`、`home-focused-edit-tests.json`、`home-final-compile-console.json`、两份 Builder 回执及截图。

## 制作人认可与下一步

制作人对完整实机样板明确答复：**“当前比例可以，继续细化”**。该认可只覆盖当前人物与建筑的相对比例，不代表地面、UI、人物身份差异、多向动画或最终量产标准全部验收。

已按该比例完成第一版草地与土路实机接入。下一步推进人物身份差异与多方向、动作，然后细化建筑变化与 UI；不回退重做已认可的基础尺度。装饰道路围绕初始布局，不是任意新建筑的自动路网，也不构成导航实现。

后续更新：已完成两张原创静态视图加镜像的四斜向接入，原配置表的列宽、冻结行和其他包内容也已保护修复；详见[人物朝向记录](PERSON-FACING-PROGRESS.md)。步行动画与角色差异仍未完成。
