# 管理与人物详情首轮基础样式（2026-11-07）

Target Branch：ai/rrpg-r3b-population-pilot。本地持续任务，未提交/推送，不代表全模块最终完成。

## 本轮范围
原Resources/UI/ManagementView.prefab和PersonDetailView.prefab通过Editor原位改色、字体和输入框选区，保留原页面层级、布局、ScrollRect、Mask、ModalBlocker与按钮回调。管理23个普通Image、人物10个普通Image统一纸色/暖棕；黑色ModalBlocker不改，Mask原alpha与raycast不变。
ManagementRowView.cs、ManagementView.cs、PersonDetailView.cs只修改动态列表和页签的正常/选中色，防止RefreshNow覆盖回旧绿色；数据、筛选、命令、选择Authority与存档代码不变。没有创建第二套页面、Canvas或规则源。
旧Prefab备份在08_ExistingProject/management-prefab-before-r1.txt、person-detail-prefab-before-r1.txt。本批不新增PNG，不改其他工作区文件；建筑Inspector/建造已有暖色基础样式，本批没有重复精修。

## 真实流程验证
实际MainMenu Continue→原Town初始化→Home ProductNavigation/Navigate2（管理）打开正式管理宿主；实际人口6人，首个人口行可选中，EventSystem首命中PersonRowTemplate(Clone)。管理人口/职责与场所/资源三类页签均能切换，保留实际快照。
从管理页OpenPersonDetailButton进入原Person表面，DisplayedPersonId沿用真实选中人物。Overview/Life/Growth/Relationships/LifeHistory五个分页均返回true且内容正常刷新；姓名检索林恩后人物列表1项，清空恢复。管理页搜索同样筛出1行。未执行派工、撤职、施工、成长或保存命令。
人物详情modal下调用原World点击入口，选中人物不变；原close关闭后再次原管理导航可用。两类滚动容器的content/viewport引用存在、几何不改；本轮只设置过列表滚动位置，没有完整验证拖动/滚轮、长列表及多分辨率，不冒称全量滚动验收。
参考截图：management-population-art-r1.png、management-workplaces-art-r1.png、management-resources-art-r1.png、person-detail-art-r1.png。原有profession_test、PersonId、building_instance等调试文案仍在，可能导致部分行截断；后续仅在表现层清理，不改身份/规则。本轮不把配色通过等同最终信息设计完成。

## 保存与检查
Asset Refresh→编译完成→Console Error/Exception=0。用户当前存档先备份至忽略Temp/ArtManagementSave-<SHA>.original；UI验证结束SaveCheck=true，SHA保持2F42500A5E84E70354F5CEA741F66FC881306322D607B6D7485088FCEC3735EF。世界仅在测试内暂停，并启用可逆B外观；退出Play丢弃内存变化，未保存、未恢复旧存档。最终Play=false、SceneDirty=false、Compiling=false。检查记录Evidence/management-person-art-check.json。

精确产品修改：两个上述Prefab、ManagementRowView.cs、ManagementView.cs、PersonDetailView.cs。Prefab均经MCP修改，没有手改YAML；C#仅调色，diff检查通过。保留SampleScene及其他已有工作区修改。

下一步补远征准备/事件卡/结果和成长等未统一表面，整理剩余首轮缺口；不继续把已覆盖页面局部精修。专属插画、装饰、骨骼动画后移。
