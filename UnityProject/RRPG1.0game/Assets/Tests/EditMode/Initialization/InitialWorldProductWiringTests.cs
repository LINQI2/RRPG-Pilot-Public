using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RRPG.StaticConfig;
using UnityEngine;

namespace RRPG.Initialization.Tests
{
    /// <summary>
    /// Initial World 产品实体接线专项 Edit 测试（阶段 1/2）。
    ///
    /// 只读产品入口，不伪造 Place / 不手写 Mirror：
    ///   - 正式默认生成配置 → SaveSystem.NewGame() 创建真实 Completed Farm / Kitchen / House；
    ///   - 默认 7 个初始建筑在当前 Test 配置下可调，代码不冻结数量比例；
    ///   - 实例 Farm / Kitchen 可合法派工；Housing 没有岗位（不新增容量规则）；
    ///   - 正式新世界不再注册 ws_farm_01 / ws_kitchen_01 / ws_test_* 静态岗位与装饰 Place；
    ///   - 旧无 Buildings 存档 Load 仍按真实保存来源恢复静态职责，Continue 不补初始建筑；
    ///   - 工坊 / 铁匠铺从正式 Catalog / Choose / Confirm / TryPlaceBuilding 下线，旧实例仍可 Load；
    ///   - Home 显示快照与运行时使用同一 Building InstanceId：实例岗位 / 住房不再绘制第二套实体。
    ///
    /// 阶段 1/3 强化：Load 的岗位 / 资源目录只从存档可证明来源恢复（冷/热会话同一档必须等值）；
    /// 住房显示别名只用 Completed 住房，并按已存 PersonId 派生（不随人员排序 / 离村 / Continue 漂移）。
    ///
    /// 存档保护：复用 InitialWorldConfigurationTests.SaveFileGuard —— 先 Capture 用户单槽真实字节
    /// （持久快照 + 回读 SHA 校验），任何异常 / TearDown 的 finally 恢复并回读校验；
    /// 存在未完成 run 时阻断新测试。本类不调用 tests-run / Shell / Process / rql-mutate。
    /// </summary>
    public sealed class InitialWorldProductWiringTests
    {
        private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type SaveSystemType = Type.GetType("SaveSystem, Assembly-CSharp", true);
        private static readonly Type BridgeType = Type.GetType("VillageRuntimeBridge, Assembly-CSharp", true);
        private static readonly Type CatalogType = Type.GetType("InitialWorldConfigurationCatalog, Assembly-CSharp", true);
        private static readonly Type PlayerCollectionType = Type.GetType("PlayerCollection, Assembly-CSharp", true);
        private static readonly Type ProductHomeAdapterType = Type.GetType("ProductHomeReadAdapter, Assembly-CSharp", true);

        private InitialWorldConfigurationTests.SaveFileGuard _guard;

        [SetUp]
        public void SetUp()
        {
            // 必须先保护用户单槽，再做任何可能触碰存档的运行时初始化。
            _guard = InitialWorldConfigurationTests.SaveFileGuard.Capture();
            try
            {
                ResetRuntime();
                StaticConfigRegistry.Reset();
            }
            catch
            {
                _guard.Restore();
                _guard = null;
                throw;
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _guard?.Restore();
            }
            finally
            {
                _guard = null;
                ResetRuntime();
            }
        }

        // ==================== 1. 默认生成实体 / 无重复初始化 ====================

        [Test]
        public void DefaultGeneratedNewGameCreatesSevenRealBuildingsWithoutDuplicateInitialization()
        {
            // 方法名保留历史发现契约（"Seven" 只是当时的 Test 数量）；本用例的数量断言一律由
            // 当前生成配置派生，不冻结建筑数量 / 人口 / 住房比例，也不写死容量数值。
            Assert.IsTrue(TryReadDefault(out InitialWorldConfiguration expected, out string defaultDiagnostic),
                "正式默认初始世界配置必须可用：" + defaultDiagnostic);
            Assert.IsNotEmpty(expected.Buildings, "正式默认配置必须包含初始建筑（数量可调，代码不冻结比例）");
            Assert.IsNotEmpty(expected.People, "正式默认配置必须包含初始人口");

            object first = NewGameDefault();
            Assert.IsNotNull(first, "正式默认 NewGame 必须成功");

            List<object> buildings = BuildingObjects();
            Assert.AreEqual(expected.Buildings.Count, buildings.Count,
                "默认 NewGame 必须按当前生成配置创建同数量真实建筑实例");
            CollectionAssert.AreEquivalent(
                expected.Buildings.Select(b => b.BuildingDefinitionId).ToArray(),
                buildings.Select(b => BuildingProp<string>(b, "BuildingDefinitionId")).ToArray(),
                "实例定义必须与生成配置同源");
            // 每种定义的数量同样来自配置；保留非空 / 类型范围 / 无 workshop / 不重复强语义。
            foreach (string definitionId in expected.Buildings.Select(b => b.BuildingDefinitionId).Distinct())
            {
                int configuredCount = expected.Buildings.Count(b => b.BuildingDefinitionId == definitionId);
                Assert.AreEqual(configuredCount,
                    buildings.Count(b => BuildingProp<string>(b, "BuildingDefinitionId") == definitionId),
                    "默认实例数量必须与生成配置同源：" + definitionId);
                Assert.IsTrue(BuildingRegistry.IsFormallyBuildable(definitionId),
                    "默认初始建筑必须是正式可建造类型（工坊 / 铁匠铺已下线）：" + definitionId);
            }
            Assert.IsFalse(buildings.Any(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_workshop"),
                "正式默认世界不得包含工坊");
            Assert.IsFalse(buildings.Any(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_smithy"),
                "正式默认世界不得包含铁匠铺");
            Assert.IsTrue(buildings.Any(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_farm"),
                "正式默认世界必须包含真实 Farm 实例");
            Assert.IsTrue(buildings.Any(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_kitchen"),
                "正式默认世界必须包含真实 Kitchen 实例");
            foreach (object building in buildings)
                Assert.AreEqual("Completed", BuildingProp<object>(building, "LifecycleState").ToString(),
                    "初始世界建筑必须是真实 Completed 实例");

            string[] ids = buildings.Select(b => BuildingProp<string>(b, "InstanceId")).ToArray();
            Assert.AreEqual(ids.Length, ids.Distinct().Count(), "默认 NewGame 实例身份不得重复");

            object second = NewGameDefault();
            Assert.IsNotNull(second, "重复调用默认 NewGame 必须仍成功（单次替换而非累加）");
            Assert.AreEqual(expected.Buildings.Count, BuildingObjects().Count, "重复 NewGame 不得累加建筑实例");
            CollectionAssert.AreEqual(ids, BuildingObjects().Select(b => BuildingProp<string>(b, "InstanceId")).ToArray(),
                "重复 NewGame 必须从同一确定性分配器开始，不携带旧世界实例");
            Assert.AreEqual(expected.People.Count, Characters(second).Count,
                "默认人口数量必须来自生成配置，不因重复初始化累加");
            Assert.AreEqual(1, Heroes(second).Count, "默认英雄只创建一次");
            Assert.AreEqual(Characters(second).Count,
                Characters(second).Cast<object>().Select(c => Field<string>(c, "PersonId")).Distinct().Count(),
                "重复初始化不得产生重复 PersonId");
        }

        // ==================== 2. 实例 Farm / Kitchen 派工 + Housing 无岗位 ====================

        [Test]
        public void DefaultNewGameAssignsInstanceFarmKitchenAndHousingHasNoWorkplace()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);

            List<object> buildings = BuildingObjects();
            object farm = buildings.First(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_farm");
            object kitchen = buildings.First(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_kitchen");
            object house = buildings.First(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_house");
            string farmWorkplace = WorkplaceOf(farm);
            string kitchenWorkplace = WorkplaceOf(kitchen);
            string houseWorkplace = WorkplaceOf(house);

            Assert.IsNotNull(Assignments().GetWorkplaceDef(farmWorkplace), "Farm 实例岗位必须真实注册");
            Assert.IsNotNull(Assignments().GetWorkplaceDef(kitchenWorkplace), "Kitchen 实例岗位必须真实注册");
            Assert.IsNull(Assignments().GetWorkplaceDef(houseWorkplace), "Housing 不得生成岗位");
            // 容量不写死数值：期望值取自同一 FunctionProfile 的既有 VillageWorkplaceDef（Capacity 单一权威）。
            Assert.AreEqual(ProfileCapacity(FunctionProfile("building_farm", "main")),
                Assignments().GetWorkplaceDef(farmWorkplace).Capacity,
                "Farm 容量必须继承既有 Profile（数值可调，不写死）");
            Assert.AreEqual(ProfileCapacity(FunctionProfile("building_kitchen", "main")),
                Assignments().GetWorkplaceDef(kitchenWorkplace).Capacity,
                "Kitchen 容量必须继承既有 Profile（数值可调，不写死）");

            List<VillageResourceWorkplaceDef> resourceDefs = ResourceWorkplaceDefs();
            Assert.IsTrue(resourceDefs.Any(d => d.WorkplaceId == farmWorkplace && d.Role == VillageResourceRole.Farm),
                "Farm 实例必须复用既有资源 profile");
            Assert.IsTrue(resourceDefs.Any(d => d.WorkplaceId == kitchenWorkplace && d.Role == VillageResourceRole.Kitchen),
                "Kitchen 实例必须复用既有资源 profile");

            IList characters = Characters(collection);
            Assert.GreaterOrEqual(characters.Count, 3, "当前默认人口至少需要 3 人用于本测试");
            string p1 = Field<string>(characters[0], "PersonId");
            string p2 = Field<string>(characters[1], "PersonId");
            string p3 = Field<string>(characters[2], "PersonId");
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(p1, farmWorkplace), "Farm 实例岗位必须可合法派工");
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(p2, kitchenWorkplace), "Kitchen 实例岗位必须可合法派工");
            Assert.AreEqual(VillageAssignmentResult.UnknownWorkplace, TryAssign(p3, houseWorkplace),
                "Housing 无岗位，派工必须被拒绝");
        }

        // ==================== 3. 新世界静态岗位退出 + 旧无建筑档静态职责保留 ====================

        [Test]
        public void NewWorldDropsStaticWorkplacesAndLegacySaveKeepsStaticAssignment()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);

            string[] legacyIds = { "ws_farm_01", "ws_kitchen_01", "ws_test_01", "ws_test_02" };
            foreach (string legacy in legacyIds)
            {
                Assert.IsNull(Assignments().GetWorkplaceDef(legacy),
                    "正式新世界不得注册静态 Test 岗位：" + legacy);
                Assert.IsFalse(World().ContainsPlace(legacy),
                    "正式新世界不得创建静态岗位装饰 Place：" + legacy);
            }

            string personId = Field<string>(Characters(collection)[0], "PersonId");

            // 用真实旧保存来源构造兼容档：先显式建立旧静态岗位世界并派工，再 Save（无 Buildings）。
            ResetRuntime();
            EnsureInitialized();
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, "ws_farm_01"),
                "旧静态岗位必须仍可按显式兼容通道派工");
            Assert.IsTrue(Save(collection), "真实旧兼容档必须写出成功");

            ResetRuntime();
            object loaded = Load();
            Assert.IsNotNull(loaded, "旧无建筑档必须仍可 Continue / Load");
            Assert.AreEqual(0, BuildingObjects().Count, "Continue 不得按初始配置补建建筑");
            Assert.AreEqual("ws_farm_01", Assignments().GetWorkplaceId(personId),
                "旧档静态职责必须按真实保存来源保留");
            Assert.IsNotNull(Assignments().GetWorkplaceDef("ws_farm_01"),
                "Load 必须为已存在的旧静态职责恢复定义");
        }

        // ==================== 4. 工坊正式命令拒绝 + 旧实例 Load ====================

        [Test]
        public void WorkshopIsOfflineForFormalCommandsButLegacyInstanceStillLoads()
        {
            Assert.IsFalse(BuildingRegistry.IsFormallyBuildable("building_workshop"), "工坊必须退出正式建造目录");
            Assert.IsFalse(BuildingRegistry.IsFormallyBuildable("building_smithy"), "铁匠铺必须退出正式建造目录");
            Assert.IsTrue(BuildingRegistry.IsFormallyBuildable("building_farm"), "Farm 仍为正式可建造内容");

            ResetRuntime();
            object[] placement = { "building_workshop", 4f, -6f, null, null };
            Assert.IsFalse((bool)Invoke(BridgeType.GetMethod("TryPlaceBuilding", Static), null, placement),
                "正式 Confirm / TryPlaceBuilding 必须拒绝工坊");
            StringAssert.Contains("下线", placement[4] as string ?? string.Empty,
                "拒绝必须给出明确下线诊断");

            // 旧实例恢复：直接建立历史 Completed 工坊实例（绕过正式命令），Save → Load 仍可恢复。
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);
            ResetRuntime();
            Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, out string definitionDiagnostic),
                definitionDiagnostic);
            object runtime = BridgeType.GetProperty("Buildings", Static).GetValue(null);
            MethodInfo addCompleted = runtime.GetType().GetMethod("TryAddInitialCompleted", Instance);
            Assert.IsNotNull(addCompleted, "BuildRuntime 必须保留历史 Completed 实例的窄入口");
            object[] addArgs = { definitions, "building_workshop", 4f, -6f, null, null };
            Assert.IsTrue((bool)addCompleted.Invoke(runtime, addArgs), addArgs[5] as string);
            string workshopId = BuildingProp<string>(addArgs[4], "InstanceId");

            EnsureInitialized();
            Assert.IsTrue(Save(collection), "含历史工坊实例的存档必须可保存");

            ResetRuntime();
            object loaded = Load();
            Assert.IsNotNull(loaded, "含历史工坊实例的旧档必须可 Continue / Load");
            object restored = BuildingObjects().SingleOrDefault(b => BuildingProp<string>(b, "InstanceId") == workshopId);
            Assert.IsNotNull(restored, "旧实例 Load 不得因正式目录下线而丢失");
            Assert.AreEqual("building_workshop", BuildingProp<string>(restored, "BuildingDefinitionId"));
            Assert.AreEqual("Completed", BuildingProp<object>(restored, "LifecycleState").ToString());
            Assert.IsFalse(BuildingRegistry.IsFormallyBuildable("building_workshop"),
                "旧实例恢复不得重新开放正式建造命令");
        }

        // ==================== 5. Home 显示快照同实例身份去重 ====================

        [Test]
        public void HomeSnapshotUsesSameBuildingIdentityWithoutDuplicateHousingOrInstanceWorkplaces()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);
            SetSimulationPersonSource(collection);

            object snapshot = ReadHome();
            List<object> runtimeBuildings = BuildingObjects();
            List<object> snapshotBuildings = PropList(snapshot, "Buildings");
            List<object> places = PropList(snapshot, "Places");

            CollectionAssert.AreEquivalent(
                runtimeBuildings.Select(b => BuildingProp<string>(b, "InstanceId")).ToArray(),
                snapshotBuildings.Select(b => GetProp<string>(b, "InstanceId")).ToArray(),
                "Home 显示建筑必须与运行时是同一批 InstanceId");
            CollectionAssert.AreEquivalent(
                runtimeBuildings.Select(b => BuildingProp<string>(b, "BuildingDefinitionId")).ToArray(),
                snapshotBuildings.Select(b => GetProp<string>(b, "DefinitionId")).ToArray(),
                "Home 显示建筑定义必须与运行时同源");

            var buildingIds = new HashSet<string>(snapshotBuildings.Select(b => GetProp<string>(b, "InstanceId")),
                StringComparer.Ordinal);
            foreach (object place in places.Where(p => GetProp<string>(p, "PlaceId").StartsWith("workplace:", StringComparison.Ordinal)))
            {
                string linked = GetProp<string>(place, "BuildingInstanceId");
                Assert.IsNotEmpty(linked, "实例岗位 Place 必须绑定真实 Building InstanceId");
                Assert.IsTrue(buildingIds.Contains(linked), "实例岗位 Place 不得指向快照外建筑");
                Assert.IsFalse(GetProp<bool>(place, "DrawsWorldEntity"),
                    "实例岗位不得在同一建筑上再画一个 Place 实体");
            }

            Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, out string diagnostic),
                diagnostic);
            var housingIds = new HashSet<string>(snapshotBuildings
                .Where(b => definitions.TryGetById(GetProp<string>(b, "DefinitionId"), out BuildingDefinition d)
                            && string.Equals(d.Category, "Housing", StringComparison.OrdinalIgnoreCase))
                .Select(b => GetProp<string>(b, "InstanceId")), StringComparer.Ordinal);
            // 住房数量可调：期望值取自当前生成配置，不冻结人口 / 住房比例。
            Assert.IsTrue(TryReadDefault(out InitialWorldConfiguration defaultConfig, out string defaultConfigDiagnostic),
                "正式默认初始世界配置必须可用：" + defaultConfigDiagnostic);
            int configuredHousing = defaultConfig.Buildings.Count(b =>
                definitions.TryGetById(b.BuildingDefinitionId, out BuildingDefinition configuredDefinition)
                && string.Equals(configuredDefinition.Category, "Housing", StringComparison.OrdinalIgnoreCase));
            Assert.Greater(configuredHousing, 0, "正式默认配置必须包含住房实例（数量可调）");
            Assert.AreEqual(configuredHousing, housingIds.Count,
                "Home 显示住房实例数必须与当前生成配置同源（不冻结比例）");

            var homePlaces = places.Where(p => GetProp<string>(p, "Kind") == "Home").ToList();
            Assert.IsNotEmpty(homePlaces, "人物住房位置语义必须保留为 Home 投影");
            foreach (object home in homePlaces)
            {
                string linked = GetProp<string>(home, "BuildingInstanceId");
                Assert.IsNotEmpty(linked, "有真实住房时，Home 投影必须绑定真实 Housing InstanceId");
                Assert.IsTrue(housingIds.Contains(linked), "Home 投影不得指向非 Housing 实例");
                Assert.IsFalse(GetProp<bool>(home, "DrawsWorldEntity"),
                    "Home 投影不得凭空再画一套房屋");
            }
            int usedHousing = homePlaces.Select(p => GetProp<string>(p, "BuildingInstanceId")).Distinct().Count();
            Assert.Greater(usedHousing, 0, "有人物时必须有真实住房作为入住锚点");
            Assert.LessOrEqual(usedHousing, housingIds.Count,
                "每个真实 Housing 实例最多绘制一次，人物共用不复制房屋实体（住房别名不保证每栋都被占用）");

            Assert.IsFalse(places.Any(p => GetProp<bool>(p, "DrawsWorldEntity")
                && (GetProp<string>(p, "Kind") == "Home"
                    || GetProp<string>(p, "PlaceId").StartsWith("workplace:", StringComparison.Ordinal))),
                "同一实例身份不得同时以 Building 和 Place 绘制两套世界实体");
        }

        // ==================== 6. 正式实例档：冷/热会话 Load 目录确定性 ====================

        [Test]
        public void FormalSaveLoadsDeterministicInstanceCatalogueInColdAndWarmSessionWithProductionEvidence()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);
            SetSimulationPersonSource(collection);
            object farm = BuildingObjects().First(b => BuildingProp<string>(b, "BuildingDefinitionId") == "building_farm");
            string farmWorkplace = WorkplaceOf(farm);
            string personId = Field<string>(Characters(collection)[0], "PersonId");
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, farmWorkplace));

            VillageResourceWorkplaceDef farmProfile = ResourceWorkplaceDefs().Single(d => d.WorkplaceId == farmWorkplace);
            Assert.AreEqual(VillageResourceRole.Farm, farmProfile.Role, "实例 Farm 必须复用既有资源角色");

            // 真实 Village.IntradayWork 域 API 写入可持久化劳务证据 / 资源批次（不驱动 UI、不手改存档字段）。
            Assert.IsTrue(Work().TryBeginBatch(BridgeResources(), farmProfile), "Farm 资源批次必须可合法开始");
            Work().Work(personId, farmWorkplace, 480);
            Work().ProcessBatch(BridgeResources(), farmProfile, 480);
            int ingredients = BridgeResources().Ingredients;
            Assert.Greater(ingredients, 0, "真实资源批次处理后必须有 Ingredients 产出证据");
            string savedWork = JsonUtility.ToJson(Work().ToSaveData());
            Assert.IsTrue(Work().ToSaveData().Evidence.Any(e => e.WorkplaceId == farmWorkplace && e.Minutes == 480),
                "存档前必须存在实例 Farm 的真实劳务证据");
            Assert.IsTrue(Save(collection), "正式实例世界必须可保存");

            // 冷会话：运行时完全重置后再 Load（Load 时会话内没有既有 Assignments）。
            ResetRuntime();
            object coldLoaded = Load();
            Assert.IsNotNull(coldLoaded, "正式实例档必须可在冷会话 Continue");
            string coldState = WorldStateSnapshot();

            // 热会话：先让当前会话持有完整静态兼容目录（旧世界里最容易造成“按会话状态恢复”的前置），再 Load 同一档。
            ResetRuntime();
            EnsureInitialized();
            Assert.IsNotNull(Assignments().GetWorkplaceDef("ws_farm_01"), "热会话前置必须确实持有静态兼容目录");
            Assert.IsTrue(World().ContainsPlace("ws_kitchen_01"), "热会话前置必须确实持有静态岗位 Place");
            object warmLoaded = Load();
            Assert.IsNotNull(warmLoaded, "正式实例档必须可在热会话 Continue");
            string warmState = WorldStateSnapshot();

            Assert.AreEqual(coldState, warmState,
                "冷/热会话加载同一正式实例档必须得到同一岗位目录 / 资源职责 / World / 存档快照");

            foreach (string legacy in new[] { "ws_farm_01", "ws_kitchen_01", "ws_test_01", "ws_test_02" })
            {
                Assert.IsNull(Assignments().GetWorkplaceDef(legacy),
                    "正式实例档不得凭空重种静态岗位定义：" + legacy);
                Assert.IsFalse(World().ContainsPlace(legacy),
                    "正式实例档不得凭空重种静态岗位 Place：" + legacy);
                Assert.IsFalse(ResourceWorkplaceDefs().Any(d => d.WorkplaceId == legacy),
                    "正式实例档不得凭空重种静态资源职责：" + legacy);
            }

            Assert.AreEqual(farmWorkplace, Assignments().GetWorkplaceId(personId), "实例岗位职责必须按存档恢复");
            Assert.AreEqual(ingredients, BridgeResources().Ingredients, "资源数量必须按存档恢复（不重复生产）");
            Assert.AreEqual(savedWork, JsonUtility.ToJson(Work().ToSaveData()),
                "生产证据（批次 / 劳务证据）必须逐项等值恢复");
            Assert.IsTrue(ResourceWorkplaceDefs().Any(d => d.WorkplaceId == farmWorkplace && d.Role == VillageResourceRole.Farm),
                "实例 Farm 资源职责必须按存档建筑重建");
        }

        // ==================== 7. 旧无建筑档：冷/热会话恢复同一兼容目录 ====================

        [Test]
        public void LegacySaveWithoutBuildingsLoadsSameCompatibilityCatalogueColdAndWarm()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);
            string personId = Field<string>(Characters(collection)[0], "PersonId");

            // 真实旧兼容世界来源：显式兼容通道建立静态岗位世界并派工（此时 Buildings 为空）。
            ResetRuntime();
            EnsureInitialized();
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, "ws_farm_01"),
                "旧静态岗位必须仍可按显式兼容通道派工");
            VillageResourceWorkplaceDef legacyFarm = ResourceWorkplaceDefs().Single(d => d.WorkplaceId == "ws_farm_01");
            Assert.IsTrue(Work().TryBeginBatch(BridgeResources(), legacyFarm));
            Work().Work(personId, "ws_farm_01", 240);
            Work().ProcessBatch(BridgeResources(), legacyFarm, 240);
            string savedWork = JsonUtility.ToJson(Work().ToSaveData());
            Assert.IsTrue(Save(collection), "旧兼容无建筑档必须可保存");

            ResetRuntime();
            object coldLoaded = Load();
            Assert.IsNotNull(coldLoaded, "旧无建筑档必须可在冷会话 Continue");
            string coldState = WorldStateSnapshot();
            AssertLegacyCompatibilityCatalogue(personId, savedWork);

            // 热会话：当前会话已持有正式实例目录（静态定义全部缺失），再 Load 旧档。
            ResetRuntime();
            object warmSeed = NewGameDefault();
            Assert.IsNotNull(warmSeed);
            Assert.IsNull(Assignments().GetWorkplaceDef("ws_farm_01"), "正式实例世界必须没有静态岗位（热会话前置）");
            object warmLoaded = Load();
            Assert.IsNotNull(warmLoaded, "旧无建筑档必须可在热会话 Continue");
            Assert.AreEqual(coldState, WorldStateSnapshot(),
                "旧无建筑档在冷/热会话必须恢复同一兼容岗位目录 / 资源职责 / World / 存档快照");
            AssertLegacyCompatibilityCatalogue(personId, savedWork);
            Assert.AreEqual(0, BuildingObjects().Count, "旧档 Load 不得按初始配置补建建筑");
        }

        private static void AssertLegacyCompatibilityCatalogue(string personId, string savedWork)
        {
            Assert.AreEqual("ws_farm_01", Assignments().GetWorkplaceId(personId), "旧档静态职责必须保留");
            foreach (string legacy in new[] { "ws_farm_01", "ws_kitchen_01", "ws_test_01", "ws_test_02" })
            {
                Assert.IsNotNull(Assignments().GetWorkplaceDef(legacy), "旧档兼容目录必须保留历史岗位：" + legacy);
                Assert.IsTrue(World().ContainsPlace(legacy), "旧档兼容目录必须保留岗位 Place：" + legacy);
            }
            Assert.IsTrue(ResourceWorkplaceDefs().Any(d => d.WorkplaceId == "ws_farm_01" && d.Role == VillageResourceRole.Farm),
                "旧档必须保留历史资源职责");
            Assert.AreEqual(savedWork, JsonUtility.ToJson(Work().ToSaveData()),
                "旧档资源批次 / 劳务证据必须逐项等值恢复");
        }

        // ==================== 8. 正式实例档只恢复它真实引用的旧静态岗位 ====================

        [Test]
        public void FormalSaveRestoresOnlyLegacyWorkplacesItActuallyReferences()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);
            string personId = Field<string>(Characters(collection)[0], "PersonId");

            // 真实兼容通道显式追加一个旧岗位定义后派工；随后保存时 Buildings 仍真实存在。
            BridgeType.GetMethod("EnsureInitializedWithExtraWorkplaces", Static).Invoke(null, new object[]
            {
                null,
                new List<VillageWorkplaceDef>
                {
                    new VillageWorkplaceDef
                    {
                        WorkplaceId = "ws_farm_01", DisplayName = "Farm（Test / Not Frozen）",
                        Capacity = 2, TestContributionPerDay = 1
                    }
                }
            });
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, "ws_farm_01"));
            Assert.IsTrue(Save(collection), "同时含实例建筑与旧静态职责的存档必须可保存");

            ResetRuntime();
            object coldLoaded = Load();
            Assert.IsNotNull(coldLoaded, "同时含实例建筑与旧静态职责的存档必须可 Continue");
            Assert.AreEqual("ws_farm_01", Assignments().GetWorkplaceId(personId), "被存档真实引用的旧职责必须恢复");
            Assert.IsNotNull(Assignments().GetWorkplaceDef("ws_farm_01"), "被引用的旧静态定义必须恢复");
            Assert.IsNull(Assignments().GetWorkplaceDef("ws_kitchen_01"), "未被引用的静态岗位不得凭空重种");
            Assert.IsNull(Assignments().GetWorkplaceDef("ws_test_01"), "未被引用的静态岗位不得凭空重种");
            Assert.IsTrue(ResourceWorkplaceDefs().Any(d => d.WorkplaceId == "ws_farm_01" && d.Role == VillageResourceRole.Farm),
                "被引用的旧静态资源职责必须恢复");
            Assert.IsFalse(ResourceWorkplaceDefs().Any(d => d.WorkplaceId == "ws_kitchen_01"),
                "未被引用的静态资源职责不得凭空重种");
            Assert.IsFalse(World().ContainsPlace("ws_kitchen_01"), "未被引用的静态岗位 Place 不得凭空出现");
            string coldState = WorldStateSnapshot();

            ResetRuntime();
            EnsureInitialized();
            object warmLoaded = Load();
            Assert.IsNotNull(warmLoaded);
            Assert.AreEqual(coldState, WorldStateSnapshot(),
                "引用旧静态岗位的正式实例档在冷/热会话必须得到同一目录 / 资源职责 / World 快照");
        }

        // ==================== 9. 住房显示别名：只用 Completed、按身份稳定 ====================

        [Test]
        public void HousingAliasesUseOnlyCompletedHousesAndStayStableAcrossOrderAndContinue()
        {
            object collection = NewGameDefault();
            Assert.IsNotNull(collection);
            SetSimulationPersonSource(collection);
            Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, out string diagnostic),
                diagnostic);

            string before = HousingHomeMapping(ReadHome());
            Assert.IsNotEmpty(before, "默认世界必须至少有一个 Home 显示别名");

            string constructionHouseId = PlaceUnderConstructionHouse();
            Assert.IsNotNull(constructionHouseId, "测试必须真实创建一个未完成住房实例");
            Assert.AreEqual("UnderConstruction",
                BuildingProp<object>(BuildingObjects().Single(b => BuildingProp<string>(b, "InstanceId") == constructionHouseId), "LifecycleState").ToString());

            string withConstruction = HousingHomeMapping(ReadHome());
            Assert.AreEqual(before, withConstruction, "未完成住房不得成为入住锚点，也不得改变既有别名");
            CollectionAssert.DoesNotContain(withConstruction.Split(';').Select(e => e.Substring(e.IndexOf('=') + 1)).ToArray(),
                constructionHouseId, "未完成住房不得被任何 Home 投影引用");

            // 人员排序变化（真实列表顺序反转）不得让现有人员跳住房。
            IList characters = Characters(collection);
            var originalOrder = characters.Cast<object>().ToList();
            foreach (object person in originalOrder) characters.Remove(person);
            for (int i = originalOrder.Count - 1; i >= 0; i--) characters.Add(originalOrder[i]);
            SetSimulationPersonSource(collection);
            Assert.AreEqual(before, HousingHomeMapping(ReadHome()), "人员排序变化不得让现有人员跳住房");
            foreach (object person in originalOrder) characters.Remove(person);
            foreach (object person in originalOrder) characters.Add(person);
            SetSimulationPersonSource(collection);
            Assert.AreEqual(before, HousingHomeMapping(ReadHome()), "恢复原顺序后别名必须保持不变");

            // Continue：同一 PersonId + 同一 Completed 住房集合必须得到同一别名。
            Assert.IsTrue(Save(collection), "含未完成住房的世界必须可保存");
            ResetRuntime();
            object loaded = Load();
            Assert.IsNotNull(loaded, "含未完成住房的存档必须可 Continue");
            SetSimulationPersonSource(loaded);
            string afterContinue = HousingHomeMapping(ReadHome());
            Assert.AreEqual(before, afterContinue, "Continue 不得让现有人员无原因跳住房");

            var completedHousing = BuildingObjects()
                .Where(b => BuildingProp<object>(b, "LifecycleState").ToString() == "Completed"
                            && definitions.TryGetById(BuildingProp<string>(b, "BuildingDefinitionId"), out BuildingDefinition d)
                            && string.Equals(d.Category, "Housing", StringComparison.OrdinalIgnoreCase))
                .Select(b => BuildingProp<string>(b, "InstanceId")).ToHashSet();
            Assert.IsTrue(completedHousing.Count > 0, "默认世界必须存在 Completed 住房");
            foreach (string entry in afterContinue.Split(';'))
                Assert.IsTrue(completedHousing.Contains(entry.Substring(entry.IndexOf('=') + 1)),
                    "Home 别名只能指向 Completed 住房实例：" + entry);
        }

        // ==================== 反射 / 断言辅助 ====================

        // 真实成员形态（宿主 NRE 根因）：VillageRuntimeBridge.IntradayWork 是静态**属性**
        // （VillageRuntimeBridge.Work.cs：public static VillageIntradayWork IntradayWork { get; private set; }），
        // 用 GetField 取会返回 null → 原先 .GetValue(null) 直接 NRE。Resources 仍是静态字段。
        // 这里显式断言成员存在，成员被重命名 / 改形态时给出明确失败信息而不是空引用。
        private static VillageIntradayWork Work()
        {
            PropertyInfo property = BridgeType.GetProperty("IntradayWork", Static);
            Assert.IsNotNull(property,
                "VillageRuntimeBridge.IntradayWork 必须作为可读静态属性存在（当前实际成员形态）");
            var work = (VillageIntradayWork)property.GetValue(null, null);
            Assert.IsNotNull(work, "VillageRuntimeBridge.IntradayWork 必须在当前运行时已初始化");
            return work;
        }

        private static VillageResourceState BridgeResources()
        {
            FieldInfo field = BridgeType.GetField("Resources", Static);
            Assert.IsNotNull(field, "VillageRuntimeBridge.Resources 必须作为静态字段存在（当前实际成员形态）");
            var resources = (VillageResourceState)field.GetValue(null);
            Assert.IsNotNull(resources, "VillageRuntimeBridge.Resources 必须在当前运行时已初始化");
            return resources;
        }

        private static string WorldStateSnapshot()
        {
            var parts = new List<string>
            {
                "buildings=" + string.Join(";", BuildingObjects().Select(b => string.Join("|",
                    BuildingProp<string>(b, "InstanceId"), BuildingProp<string>(b, "BuildingDefinitionId"),
                    BuildingProp<float>(b, "WorldX").ToString("R"), BuildingProp<float>(b, "WorldY").ToString("R"),
                    BuildingProp<object>(b, "LifecycleState").ToString()))),
                "workplaces=" + string.Join(";", Assignments().WorkplaceDefs.Select(d => string.Join("|",
                    d.WorkplaceId, d.DisplayName, d.Capacity, d.TestContributionPerDay))),
                "resources=" + string.Join(";", ResourceWorkplaceDefs().Select(d => string.Join("|",
                    d.WorkplaceId, d.Role, d.PerWorkerDailyOutput, d.InputIngredientsPerUnit,
                    d.OutputFoodPerUnit, d.ProcessingUnitsPerWorkerPerDay))),
                "world=" + string.Join(";", World().Places.Select(p => string.Join("|",
                    p.PlaceId, p.Kind, p.DisplayName, p.SourceRef))),
                "save=" + JsonUtility.ToJson(BuildVillageSaveData())
            };
            return string.Join("\n", parts);
        }

        private static VillageSaveData BuildVillageSaveData() =>
            (VillageSaveData)BridgeType.GetMethod("BuildSaveData", Static).Invoke(null, null);

        private static string HousingHomeMapping(object snapshot)
        {
            return string.Join(";", PropList(snapshot, "Places")
                .Where(p => GetProp<string>(p, "Kind") == "Home")
                .Select(p => GetProp<string>(p, "PlaceId") + "=" + GetProp<string>(p, "BuildingInstanceId"))
                .OrderBy(e => e, StringComparer.Ordinal));
        }

        private static string PlaceUnderConstructionHouse()
        {
            for (float y = -10f; y <= 8f; y += 2f)
            {
                for (float x = -12f; x <= 12f; x += 2f)
                {
                    object[] placement = { "building_house", x, y, null, null };
                    if ((bool)Invoke(BridgeType.GetMethod("TryPlaceBuilding", Static), null, placement))
                        return BuildingProp<string>(placement[3], "InstanceId");
                }
            }
            Assert.Fail("默认布局下必须存在一个合法空余位置用于测试未完成住房");
            return null;
        }

        /// <summary>实例岗位引用的 FunctionProfile 身份（来自生成 BuildingFunctionBinding）。</summary>
        private static string FunctionProfile(string buildingDefinitionId, string slotId)
        {
            Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry registry, out string diagnostic),
                diagnostic);
            BuildingFunctionBinding binding = registry.FunctionBindings.FirstOrDefault(f =>
                f.BuildingDefinitionId == buildingDefinitionId && f.FunctionSlotId == slotId);
            Assert.IsNotNull(binding, "生成配置必须存在功能绑定：" + buildingDefinitionId + "/" + slotId);
            return binding.FunctionProfileId;
        }

        /// <summary>既有 VillageWorkplaceDef Profile 的容量（不写死数值，也不新建第二套 Capacity）。</summary>
        private static int ProfileCapacity(string profileId)
        {
            FieldInfo field = BridgeType.GetField("_testWorkplaces", Static);
            Assert.IsNotNull(field, "VillageRuntimeBridge._testWorkplaces 必须存在（Capacity 单一权威）");
            var profiles = (IEnumerable<VillageWorkplaceDef>)field.GetValue(null);
            VillageWorkplaceDef profile = profiles.FirstOrDefault(d => d.WorkplaceId == profileId);
            Assert.IsNotNull(profile, "岗位 Profile 必须存在：" + profileId);
            Assert.Greater(profile.Capacity, 0, "岗位 Profile 容量必须为正：" + profileId);
            return profile.Capacity;
        }

        private static object NewGameDefault()
        {
            MethodInfo method = SaveSystemType.GetMethod("NewGame", Static, null, Type.EmptyTypes, null);
            Assert.IsNotNull(method, "SaveSystem.NewGame() 默认入口必须存在");
            return Invoke(method, null);
        }

        private static object Invoke(MethodInfo method, object target, params object[] args)
        {
            try
            {
                return method.Invoke(target, args);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        private static void ResetRuntime()
        {
            BridgeType.GetMethod("ResetForTests", Static).Invoke(null, null);
            FieldInfo failure = SaveSystemType.GetField("ForceSaveFailureForTests", Static);
            if (failure != null) failure.SetValue(null, null);
        }

        private static void EnsureInitialized()
        {
            BridgeType.GetMethod("EnsureInitialized", Static).Invoke(null, new object[] { null });
        }

        private static bool TryReadDefault(out InitialWorldConfiguration config, out string diagnostic)
        {
            var args = new object[] { null, null };
            bool result = (bool)CatalogType.GetMethod("TryReadDefault", Static).Invoke(null, args);
            config = args[0] as InitialWorldConfiguration;
            diagnostic = args[1] as string;
            return result;
        }

        private static object ReadHome()
        {
            MethodInfo method = ProductHomeAdapterType.GetMethod("Read", Static);
            Assert.IsNotNull(method, "ProductHomeReadAdapter.Read 必须存在");
            return Invoke(method, null);
        }

        private static List<object> PropList(object target, string property)
        {
            var list = (IEnumerable)target.GetType().GetProperty(property, Instance).GetValue(target, null);
            return list.Cast<object>().ToList();
        }

        private static T GetProp<T>(object target, string property) =>
            (T)target.GetType().GetProperty(property, Instance).GetValue(target, null);

        private static IList Characters(object collection) =>
            (IList)PlayerCollectionType.GetField("OwnedCharacters", Instance).GetValue(collection);
        private static IList Heroes(object collection) =>
            (IList)PlayerCollectionType.GetField("OwnedHeroes", Instance).GetValue(collection);
        private static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, Instance).GetValue(target);

        private static VillageAssignmentSystem Assignments() =>
            (VillageAssignmentSystem)BridgeType.GetField("Assignments", Static).GetValue(null);
        private static VillageWorldRegistry World() =>
            (VillageWorldRegistry)BridgeType.GetField("World", Static).GetValue(null);
        private static List<VillageResourceWorkplaceDef> ResourceWorkplaceDefs() =>
            (List<VillageResourceWorkplaceDef>)BridgeType.GetField("_resourceWorkplaceDefs", Static).GetValue(null);

        private static VillageAssignmentResult TryAssign(string personId, string workplaceId) =>
            (VillageAssignmentResult)BridgeType.GetMethod("TryAssignPerson", Static).Invoke(null, new object[] { personId, workplaceId });

        private static List<object> BuildingObjects()
        {
            object runtime = BridgeType.GetProperty("Buildings", Static).GetValue(null);
            var all = (IEnumerable)runtime.GetType().GetProperty("All", Instance).GetValue(runtime, null);
            return all.Cast<object>().ToList();
        }

        private static T BuildingProp<T>(object building, string name) =>
            (T)building.GetType().GetProperty(name, Instance).GetValue(building, null);

        private static string WorkplaceOf(object building) =>
            "workplace:" + BuildingProp<string>(building, "InstanceId") + ":main";

        private static bool Save(object collection) =>
            (bool)SaveSystemType.GetMethod("Save", Static).Invoke(null, new[] { collection });

        private static object Load() => SaveSystemType.GetMethod("Load", Static).Invoke(null, null);

        private static void SetSimulationPersonSource(object collection) =>
            BridgeType.GetMethod("SetSimulationPersonSource", Static).Invoke(null, new object[] { collection });
    }
}
