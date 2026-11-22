# RRPG 美术资源库

本项目美术升级的统一工作入口。当前收录**30张ImageGen图、2张几何导引**及过程文件。初次整理的41个旧文件迁移后逐一SHA256一致；后续批次持续登记，缓存原件保留，库中不使用exec随机文件名。

**看所有图片与去留原因：[分类索引 CATALOG.md](CATALOG.md)**。机器可读清单：[catalog.csv](catalog.csv) / [catalog.json](catalog.json)，记录新旧路径、SHA、生成器缓存来源及提示词记录。

> 公开候选副本说明（2026-11-22）：为保护本机信息，`catalog.json` 中的 `GeneratorCachePath` 已统一替换为 `LOCAL_CACHE_PATH_REDACTED`，来源 SHA 与提示词索引未改变；本地生成器缓存、历史备份和保护现场不随公开文件快照提供。候选素材、测试截图和既有项目资产的公开许可仍需独立核实。

**当前制作策略：先全游戏统一升级一轮，再统一细化。** 村庄/Home已有首轮样板，暂停单点精修，优先补齐其他模块的白模、粗糙美术和不一致视角。动画继续暂缓，后续采用骨骼方案。执行以[最新路线](07_Process/2026-11-06/Records/ART-UPGRADE-ROADMAP.md)和[首轮覆盖清单](07_Process/2026-11-06/Records/FIRST-PASS-COVERAGE.md)为准，日期化旧记录的后续顺序不再覆盖本次决策。

## 按目的找文件

| 目录 | 放什么 | 当前状态 |
|---|---|---|
| [00_Inbox](00_Inbox/README.md) | 新生成、尚未审查的输入 | 空；新图先归这里 |
| [01_Direction](01_Direction/README.md) | 已选画风/视角、当前构图、几何导引 | 01画风、R4视角、R5对齐方案；各自认可范围有区别 |
| [02_Candidates](02_Candidates/README.md) | 准备继续加工或尝试接入的独立素材 | 共15张；8项Home已接样板，4项跨场景背景/环境层已进真实流程，3个人物差异待绑定 |
| [03_ReadyToIntegrate](03_ReadyToIntegrate/README.md) | 质量与规格检查完成、等待正式接线的素材 | 当前无素材，不能把候选自动算作就绪 |
| [04_Integrated](04_Integrated/README.md) | 接入记录与资源位置索引 | 8项已接B素材，Home共享UI首轮样式已接入；动画暂缓，后续骨骼方案 |
| [05_Experiments](05_Experiments/README.md) | 有对照价值但不用于当前制作的实验 | 相机对照，以及已停止制作的逐帧动作实验 |
| [06_Abandoned](06_Abandoned/README.md) | 未选方向、明确否决、规格失败、被替代版本 | 按原因分开保留，不删除失败证据 |
| [07_Process/2026-11-06](07_Process/2026-11-06/README.md) | 提示词、讨论记录、脚本、检查证据 | 原始提示词/历史路径不篡改 |
| [08_ExistingProject](08_ExistingProject/README.md) | 升级前既有项目素材的原位清单 | 原资源与生成绑定不移动、不重复设源 |
| [09_Previews](09_Previews/README.md) | 独立源图按确定性坐标组合的本地预览 | 网格、标签、占地参考与同类缩放；非游戏场景 |

## 当前采用与候选

- **画风**：[visual-target-01.png](01_Direction/Selected/visual-target-01.png)，采用温暖冒险插画色彩/材质；原镜头、假UI未采用。
- **视角**：[home-spatial-r4.png](01_Direction/Selected/home-spatial-r4.png)，制作人认可高位斜俯视和体积；原朝向与摆放需修正。
- **对齐方案**：[home-spatial-r5-grid.png](01_Direction/Working/home-spatial-r5-grid.png)，保留R4观感，统一斜向网格；仍是校准图，不是运行场景。
- **已接B样板**：[住宅](02_Candidates/Village/Buildings/house_r3.png)、[厨房](02_Candidates/Village/Buildings/kitchen_r3.png)、[农田](02_Candidates/Village/Farms/farm_r5_candidate.png)、[固定酒馆](02_Candidates/Village/Places/tavern_place_r1.png)、[世界人物母样](02_Candidates/Village/Persons/person_world_neutral_sw_r1.png)及[地面素材](02_Candidates/Village/Terrain/README.md)。不等于最终美术母版，人物母样不代表命名角色定稿。
- **人物基准与差异**：[制作基准](01_Direction/PERSON-LANGUAGE.md)，比例已获制作人认可；新增[成年女性](02_Candidates/Village/Persons/person_world_adult_woman_sw_r1.png)、[年长男性](02_Candidates/Village/Persons/person_world_older_man_sw_r1.png)、[矮壮成年](02_Candidates/Village/Persons/person_world_sturdy_sw_r1.png)三个差异候选。
- **人物缩略/灰度对照**：[person-family-preview.html](09_Previews/person-family-preview.html)，提供同画布对照；仅静态检查，未宣称浏览器视觉已验证。
- **本地组合预览**：[village-grid-preview.html](09_Previews/village-grid-preview.html)，浏览器工具无法打开file协议，已完成静态语法和资源检查，视觉/交互未验。
- **最新工作记录**：[Home实机接入与比例认可](07_Process/2026-11-06/Records/home-art-integration-progress.md)；[第三批人物差异](07_Process/2026-11-06/Records/BATCH-03-PERSONS.md)和[第二批进展](07_Process/2026-11-06/Records/BATCH-02-PROGRESS.md)保留原时点结论。

## 后续归档方式

新图进入00_Inbox，保留对象名和版本号；审查后分配状态，更新catalog。未选方向使用not_selected，技术/视觉失败使用rejected，被更好版本取代使用superseded，不混写为“制作人全部否决”。

同一对象的后续版本不覆盖旧图。只有检查完成后才进入03；正式接入后04记录实际VisualAssetId、正式路径、配置生成与实机证据。Unity .meta/.asset/.prefab不作为美术源图在本库手工维护。

本库是候选与设计归档区。**已有正式人工来源仍是Development/SourceAssets及相应XLSX**；候选经确认后按现有Compiler/Builder链进入正式来源，不能把运行依赖改成直接读取本库。已经接入的原素材不为整理目录而搬动。

本次旧目录仅留下README跳转。过程记录保留当时实际使用的原路径，查询其新位置用catalog或[迁移回执](07_Process/2026-11-06/Evidence/organization-move-receipt.json)，不要照历史路径写入。


