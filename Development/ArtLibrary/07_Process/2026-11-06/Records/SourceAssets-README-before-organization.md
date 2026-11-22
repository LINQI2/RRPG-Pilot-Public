# ArtUpgrade01：温暖冒险插画候选源图

制作人于2026-11-06选择视觉方向01。内置ImageGen以 `Development/ArtUpgrade/2026-11-06/visual-target-01.png` 作风格参考，新生成厨房与住宅源图。

**状态：REJECTED FOR GAMEPLAY USE / NOT REGISTERED。** 制作人指出视角太平视、不符合经营空间；本次核查采纳该纠正。两图仅保留为错误证据，不登记或用于运行替换。修正版使用独立_r2文件名；详见Development/ArtUpgrade/2026-11-06/MARKET-CAMERA-CORRECTION.md。

| 文件 | 用途 | 尺寸 |
|---|---|---|
| kitchen_01.png | 厨房建筑视觉候选，有开放作业区 | 1254×1254 RGBA |
| house_01.png | 住宅建筑视觉候选，闭合主体、门窗 | 1254×1254 RGBA |

上述两张为已否决的旧图。后续补充的 `house_r2_high.png` 与 `kitchen_r2_high.png` 是更高俯视的配对控制样本，墙体已压低、厨房无火焰；它们与R4中等高位经营视角不是同一套相机，不混用或批量登记。后续r3版本以R4作直接参考。

## 当前继续制作的候选：r3

- `house_r3.png`：直接参考home-spatial-r4的住宅与管理镜头，屋顶占比更大，墙体压低。
- `kitchen_r3.png`：参考house_r3的相机/材质，使用宽烟囱和内凹作业区区分用途，无烘焙火烟/人物，附属区不外扩占地。
- 两图均1254×1254 RGBA，alpha>8抽样内容范围几乎相同，画布留白一致；真实运行大小、占地对齐、导入采样、点击与遮挡未验证。
- 本轮提示词见Development/ArtUpgrade/2026-11-06/PROMPTS-R4-BUILDINGS.md。它们不自动成为正式资产；仍须按配置链与实际宿主接线。旧图不被新图覆盖。

## R5斜向网格后续

制作人已认可R4视角，要求所有对象统一斜向网格朝向。home-spatial-r5-grid.png展示当前构图方向；后续单体按同一组斜轴制作。

`farm_r5_square_rejected.png` 为首轮农田失败证据：三条种植带与轴线可辨，但底部外形近似投影正方形，不符合3×2占地，因此不登记、不替换原农田。重做使用farm-3x2-grid-guide.png明确3:2的地面长短边。

`farm_r5_background_rejected.png` 是几何修正后的低Alpha外围底雾版本，不进入运行绑定。`farm_r5_candidate.png` 是随后ImageGen抠图结果，1536×1024，斜向矩形/三条种植带及主体几何保留，抽样外围低Alpha残留显著减少；主体Alpha约253、少量低Alpha边缘仍记录。后者为未登记的当前农田候选，实机占地、缩放和边缘质量尚未验证。

提示词：`Development/ArtUpgrade/2026-11-06/PROMPTS-BUILDINGS-01.md`。样本检查与SHA：同目录 `source-art-inspection.json`（每4像素采样，不是全像素扫描）。

## 源图检查

- 两图存在alpha=0的背景，非烘焙棋盘格；主体大部分alpha约253而非255。后续须在实际底色上检查轻微透色与轮廓晕边，当前未归一化alpha或手工修改图片。
- 两图轮廓完整，未见主要建筑截断；厨房样本内容约x36–1224/y116–1140，住宅约x60–1208/y140–1140。水平留白未满足提示词8–10%的目标，不能据此直接设统一Pivot或点击域。
- 厨房炉口生成了火焰，不适合作为全部状态共用的静态底图；下一版应移除火焰，再按现有活动状态能力决定是否添加独立表现层。
- 两图在屋顶、墙体、木梁、明暗方向上具有一致家族感；尚未验证实际缩放、遮挡、脚底/选择域或运行时内存。

## 登记边界

连接Editor后先读取当前Catalog/Presentation与实际宿主；按人工XLSX→Compiler→Builder接线。当前生成管线固定sprite_point_100（Point/100PPU/Center），必须先实测与新插画的适配，不手改生成.meta，不擅改现有预设含义。

下一步先修正源图的状态层与留白，再在一栋实例上验证。风格选择不自动代表本批源图被接受为最终量产母版。
