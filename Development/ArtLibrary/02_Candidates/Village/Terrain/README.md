# 地表素材候选

- [grass_surface_r1.png](grass_surface_r1.png)：草地，`va_village_r5_grass`。
- [earth_surface_r1.png](earth_surface_r1.png)：夯土，`va_village_r5_earth`。

均为内置 ImageGen 原创平面纹理，原始提示词见[制作记录](../../../07_Process/2026-11-06/Prompts/PROMPTS-TERRAIN-R1.md)。原始输出未被图像脚本修改。

已通过既有配置链进入 B 实机地面。引擎以同一套斜向投影生成地表网格，使用重复 UV 和路边顶点透明渐隐；不改变建筑占地、存档坐标、导航和人物活动事实。此处是候选归档，运行源图仍在 Development/SourceAssets/Village/ArtUpgrade01。
