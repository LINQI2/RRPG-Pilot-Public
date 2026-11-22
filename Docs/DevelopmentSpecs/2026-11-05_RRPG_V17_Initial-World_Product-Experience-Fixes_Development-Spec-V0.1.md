# RRPG V17 Initial World 产品体验修复 DevelopmentSpec V0.1

日期：2026-11-05（北京时间）。状态：Implemented / Targeted Verification Passed / Product Acceptance Pending / Not Committed / Not Published。制作人已明确批准本规格A/B/C工程写入及必要DSH开发调用，原待实施稿保留。本轮实现、真实测试、保护核验与限制见[技术交付](../../Development/StudioRuns/InitialWorld-ProductFix-20261105/technical-review.md)。

## 授权与保护边界

设计依据为制作人本次确认的 A/B/C 产品方案。本文件不新增游戏规则，不替代既有 GameSpec，也不沿用旧工作包授权作为本包写入许可。

唯一工程 `D:\AI_Agent_Lab\rrpg_pilot\repo`；Unity 子目录 `UnityProject/RRPG1.0game`；Target Branch `ai/rrpg-r3b-population-pilot`，预检 HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`。当前默认统计77修改、108未跟踪项；本包实施前逐文件SHA清单保存在本稿同目录 `preimplementation-files.json`，旧Initial World清单继续保留。

不得修改正式工程、连接器、认证/权限、SaveVersion/DTO、人口与住房规则、经济平衡或初始数量。当前1 Farm/1 Kitchen/5 House仍为可调整测试配置。不得reset/clean/restore、覆盖无关资产、Commit/Push。产品验收保持Pending，不宣布整个Initial World关闭。

## 字体保护强制门禁

2026-11-05 18:36现行更新：制作人在原DSH会话选择门禁v2，接受经严格结构判定的合法Dynamic Atlas增长（非静态替换/运行时实例隔离）。以最新 `font-protection-README.md` 为准：PASS及LEGITIMATE_GROWTH退出0可继续，后者须自动归档旧基线并写growth证据；INCIDENT退出2、NEEDS_REVIEW退出4及其它非0仍立即停止。不得手动capture/-Force掩盖漂移、恢复/覆盖字体或清dirty。下方固定f0哈希是本包开始时观察，不是永久基线；保留为历史事实。该制作人决策与此前两个incident保留，不回写为首次流程通过。

已读取 `Development/StudioRuns/Initialization-20261105/font-protection-README.md` 与 `scripts/font-guard.ps1`。制作人已采用当前14969377B、SHA256 `f0f5b901a5ef56a68450803e39510846ccfc38623ab0d3c8654b22f33bcf1681` 为新基线，完整字节备份已存在。本次只读status实际退出0、match=true；这不是本包测试通过。

每一段Unity流程须串行、独立Label成对执行（命令从repo根运行）：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Development/StudioRuns/Initialization-20261105/scripts/font-guard.ps1 begin -Label iw-productfix-step01
# begin退出0后，才执行该段Unity读取/Refresh/Builder/Play或测试。
# 无论成功或异常，都在finally执行同Label end，并保留原流程错误。
powershell -NoProfile -ExecutionPolicy Bypass -File Development/StudioRuns/Initialization-20261105/scripts/font-guard.ps1 end -Label iw-productfix-step01
```

begin失败不进入Unity流程。end非0立即停止后续Unity操作，保留records/incidents，不用capture/-Force重设基线，不自动恢复字体，不覆盖dirty资产。旧 `StudioTestResourceScope` 的历史固定hash不能替代本门禁。不因门禁漂移而继续重试。实际Editor身份/Console/场景/字体dirty/runner均在begin成功后重新读取。

## 现有宿主与最小实现

### A. 活动地点投影

现有 `ProductHomeReadAdapter` 投影Place事实，`ProductHomeWorldHost.Reconcile` 用 `DrawsWorldEntity` 创建/移除PlaceView，同时维护定位anchors。当前A/B共用绘制开关，不能简单全局过滤而破坏B。

实现A档独立的可见世界实体资格判断：未明确配置独立可见实体的ActivityPlace/Tavern及类似虚拟地点不创建可见占位建筑。真实Farm/Kitchen/House BuildingView不变；实例岗位和住房别名继续不重复绘制。

保留PlaceId、活动事实、当前/目标Place、人物可用性、到达/现场状态和必要的非可见定位anchor。隐藏的是投影视图，不删除Place配置/Registry/Activity数据；不通过更改人物目标寻址来隐藏问题。检查Commons、ProjectSite、Home兼容投影等相似来源，按真实实体资格处理，不按显示名写死Tavern特判。

B保持既有对照表现与布局语义；A→B→A应在同一实体/事实下正确建立或移除允许的PlaceView，不能因A期间销毁事实而无法恢复B。

候选范围：`ProductHomeWorldHost.cs`、`ProductHomeReadAdapter.cs`、必要时 `ProductHomeSnapshot.cs`/`HomePresentationResolver.cs` 的最小纯表现接缝。不改活动或Building领域算法。

### B. Home设置与保存

在现有 `HomeProductShell` 顶栏右上增加“设置”，协调关注/资源文本避免遮挡。轻量菜单仅三个动作：“保存游戏”“保存并返回主菜单”“关闭”；反馈文字不算新增菜单功能。

复用唯一 `SaveSystem.Save` 和当前Town的同一PlayerCollection。保存成功显示“保存成功”；失败保留菜单、世界与当前选择，显示可理解原因。SaveSystem现有公开入口返回bool，如需失败原因，允许新增窄的非持久化结果/诊断接缝，但仍由同一Save事务写盘，不复制序列化实现或吞掉错误。

“保存并返回主菜单”必须先确认既有主菜单宿主可用，再保存；仅成功后通过 `MainMenuUI`/`TownUI` 正式入口退出Home、恢复真实主菜单及Continue可用状态。当前MainMenu只有EnterTown链，允许原位增加薄返回方法；不另造主菜单Canvas、不调用NewGame、不创建/重建世界实体。关闭/禁用Shell时清理菜单生命周期。

菜单打开纳入既有 `SurfaceModalActive` 和世界输入阻断，透明遮罩接收Raycast，并阻断世界选择、建造/拖拽等旁路；关闭后恢复原阻断状态，不解除其它模态。保存操作期间禁用动作按钮并用busy guard阻止重复/重入；结束后恢复。不得自动改变世界速度、丢弃未处理成长选择或绕过既有CanSaveWorld限制。

候选范围：`HomeProductShell.cs`、`MainMenuUI.cs`、必要时 `TownUI.cs`/窄设置视图、`SaveSystem.cs`诊断接缝。默认复用现有运行时uGUI创建方式；确需Prefab/Scene资产时只能通过Editor工具原位修改。

### C. 资源页显式导航

当前 `ManagementView.ApplyResources` 将整行Bind到 `NavigateResourceRow`，并用文案提示“点击定位”；改为普通信息区域不触发任何导航/写入，删除旧整行跳转提示。

增加显式“查看岗位”按钮，携带原 `WorkplaceId`（实例ID+slot），调用现有职责页定位逻辑。目标不可用显示原因/禁用入口，不猜测另一个同名Farm/Kitchen。保留选中PersonId，定位准确行并滚动到可见；不经全局SelectBuilding/SelectPlace覆盖人物选择，不派工/撤职、不写库存或模拟时间。

候选范围：`ManagementView.cs`、`ManagementRowView.cs`；若需原Management prefab新增按钮，通过Editor定向修改，不重做页面布局。

## 验证矩阵（本包必须产生真实回执）

1. A Tavern及同类虚拟地点出现/消失/多人活动时没有可见占位实体、建筑不重叠；Place身份、活动事实、目标anchor与现场状态正常，Farm/Kitchen/House视图身份稳定。
2. B原型及A/B往返：保留既有对照实体与anchor、选择/到达/领域数据不变；不削弱旧视觉稳定断言。
3. Home真实入口设置菜单打开/关闭、三个准确按钮与成功反馈；EventSystem/世界选择和建造输入测试点击不穿透。
4. 正常保存只执行一次，实体/岗位/资源不重建；重复点击与重入受控。
5. 确定性注入既有Save写失败路径/守卫拒绝：保存游戏和保存返回都显示原因、留在Home、不调用主菜单。不得通过破坏真实用户存档模拟失败。
6. 保存成功后真实返回主菜单，再点Continue进入Town，PersonId/建筑InstanceId/岗位/库存一致，无reseed/重复结算。
7. 资源页点击普通区域无导航/写入；点击Farm/Kitchen各自“查看岗位”定位对应实例slot，PersonId保持，库存/职责与时钟不变。
8. Compiler定向/必要完整验证与validate；初始化、管理导航、模态输入、Save/Continue、Home视觉的相关Edit/Play串行执行。C#修改后Refresh→等编译→Console回读，负例日志与新增错误分开。
9. 每流程门禁begin/end退出码、存档guard恢复字节、场景/字体及原dirty清单比对，异常不伪造PASS。物理输入与程序化EventSystem验证区分。

## 执行、交付与技术复核

批准后由Codex将本规格与DSH工作包落盘并同步DevelopmentSpecs索引/Current引用，重新diagnose后delegate预览和send。新产品包使用自己的稳定投递编号 `initialworld-productfix-20261105-dispatch01`；任何unknown先核对原session/requestId，不换号重发。

DSH按A→C→B串行实施，先备份将改的既有文件（SHA），各阶段门禁成对，统一测试与报告。工作文件不由Codex并行修改。DSH写本包独立进度/结果，附原始测试响应、门禁records/incidents、前后SHA、变更清单、截图与用户体验入口；不覆盖旧报告或写Current文档。

Codex通过status/inbox及共享成果文件主动读取实际回传，独立审查差异、测试证据、生成链及保护核验；必要定向复测同样执行门禁。最后同步项目状态、日志、文档地图、索引和本包记录。Implemented、Verified、Product Acceptance Pending、Deferred分开，不以accepted/空闲/历史通过代替本包完成。
