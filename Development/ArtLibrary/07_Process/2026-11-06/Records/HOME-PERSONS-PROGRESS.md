# 当前批次收尾与范围调整 — 2026-11-13

Target Branch: `ai/rrpg-r3b-population-pilot`。

用户要求停止新任务、收尾当前批次；人物只需一个视角，后续重点为常用、稳定、易更换的界面资源，不制作美术扩展资源。

## 收尾状态
本地收尾完成，整体美术升级暂停，等待用户后续指示。未commit、未push、未暂存，不宣称Work Package/Branch/Repository Closed；素材尚未最终美术验收。

领地B样板复用6张既有正面图。HomePresentationResolver只要求正面可解析；HomeWorldEntityView只显示同一张图，可水平镜像，不再查询或切换背面。人物替换不需要配套多视角资源。A原版路径保留。

已生成的6张背面图保留归档；源图和既有生成文件不删除。home-persons-integration-r1.json中NE条目Active=false，说明停用原因，索引标签为“已停用多视角扩展（历史归档）”。配置表中的历史NE行暂留，但当前显示路径不依赖它们。本次不再追加配置内容。

保留此前同锚点人物的显示排列修正：不同场所标识共享同一画面锚点时共用排列序号，避免林恩/苏晚重叠；不改变身份、场所、占用、到达或模拟规则。

## 历史批次证据与限制
2026-11-08已生成归档6张背面图，复用6张正面；两张配置表各追加12行。Artifact Tool负责单元格值，原包保留工具核验非目标XML部件字节不变及受保护工作表内容不变。

Compiler build/validate通过，0错误、9条导航跳过基线警告；仅AssetManifest、VisualAsset、VillagePresentation三个JSON内容变化。Unity Builder成功，12图均可解析。旧版本24方向模块检查、6人真实画面选中通过；修正前重叠失败记录属于历史测试。表格预览渲染器异常退出，未完成XLSX截图验收；配置通过回读、原包保留校验及Compiler/Unity导入核验。

以上为历史多视角版本证据，不能代替最终单视角验证。HomeDistinctPersonVerification.cs与HomePersonFacingVerification.cs依赖已移除字段，作为历史脚本停用，不得用于当前验收。

## 本次最终验证
- 修改2个.cs后Asset Refresh，编译结束；相关3个表现脚本git diff --check通过。
- 真实MainMenu Continue → 现有领地 → B样板，读取当前存档6人。
- 每人4方向、共24项单视角模块检查：body与selection始终保持同一正面图，不切背面；模拟SaveData JSON前后不变。
- 当前Console Error/Exception为0；最终Play=False、Compiling=False、Scene Dirty=False。
- 当前磁盘存档SHA256为 `B310D4A255E091ABF9F25E30E88A9A8669DCC618BD3F1D2729A5361EE77D0A63`，SaveUnchanged=True；测试前备份于Unity忽略Temp。未恢复旧2F425...存档，未执行游戏存盘、返程或NewGame确认。
- 索引66条＝64张ImageGen＋2几何导引，归档、源图、生成图与登记meta哈希通过；Ready/FinalArtAccepted仍0。

## 文件与后续边界
精确资源与状态见 [领地人物接入清单](../../../04_Integrated/home-persons-integration-r1.json)，历史提示词见 [HOME-PERSONS-PROMPTS.md](HOME-PERSONS-PROMPTS.md)。历史截图home-persons-product-final-r2.png和home-persons-back-module-r1.png对应已停用多视角版本。

备份位于08_ExistingProject/BeforeHomePersons-20261108，含原配置表、3份代码和Compiler镜像快照。当前代码范围为HomePresentationResolver.cs、HomeWorldEntityView.cs、ProductHomeWorldHost.cs；其他工作区改动保留。

用户再次明确继续后，只优先处理既有面板、按钮、边框、图标、信息可读性与稳定替换槽。不做人物多视角、动作配套或额外角色/场景扩展资源。旧计划中继续背面与自然转身验证的条目已被本决定取代。本次不启动下一批。
