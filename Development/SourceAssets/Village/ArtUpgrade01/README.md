# 美术升级已登记源图

早期未登记候选已迁入[ArtLibrary](../../../ArtLibrary/README.md)，按候选/实验/放弃分类。本目录现在只接收已登记的正式源图输入；候选不能直接从 ArtLibrary 成为运行依赖。

`tavern.png` 已以 `va_village_r5_tavern` 登记于共享 VisualAsset 表，绑定 B 档固定酒馆表现键。它是实机样板素材，仍等待后续美术细化，不新增可建造建筑、岗位或经营规则。

`grass_surface_r1.png` / `earth_surface_r1.png` 分别登记为 `va_village_r5_grass` / `va_village_r5_earth`，由同一 Home B 地面宿主消费；不赋予装饰土路导航事实。

`person_world_neutral_ne_r1.png` 登记为 `va_village_r5_person_ne`，供 `village.person.r2.ne` 背向表现键使用；与前向图加镜像形成四个斜向的静态样板，不代表步行动画完成。

新旧文件位置、SHA与去留原因见[分类索引](../../../ArtLibrary/CATALOG.md)。候选未来经确认后再按正式配置链进入SourceAssets，不直接从ArtLibrary读取运行资源。
