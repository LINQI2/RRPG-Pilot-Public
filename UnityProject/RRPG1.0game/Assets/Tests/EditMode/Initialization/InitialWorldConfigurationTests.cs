using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;

namespace RRPG.Initialization.Tests
{
    /// <summary>
    /// Initial World Configuration（Test / Not Frozen）专项 Edit 测试。
    ///
    /// 覆盖本包第一垂直层：
    ///   - 纯配置结构校验 / 内容引用与初始布局校验；
    ///   - 初始人口、金币与 Ingredients / Food 由配置创建；
    ///   - 真实 Completed Farm / Kitchen 实例及其实例 Workplace / 资源岗位，可合法派工；
    ///   - Housing 建筑无岗位；
    ///   - NewGame 单次 / 重入无重复实例，EnsureInitialized / 测试重置不自动种产品内容；
    ///   - Continue 保持 Building InstanceId / 位置 / 职责 / 资源；
    ///   - 旧无 Buildings 存档不补建初始建筑且静态职责保留；
    ///   - 非法配置零半状态（世界逐字节不变）；
    ///   - Hero 选择「先 NewGame 再替换」时序不重复初始化、不丢 PersonId。
    ///
    /// 存档保护：每个测试开始前（先于任何运行时重置）对用户单槽做**持久字节快照**
    /// （Development/StudioRuns/Initialization-20261027/save-guard/<唯一目录>，CreateNew 写入），
    /// 快照回读字节 + SHA256 校验成功后才标记 captured；finally / TearDown 恢复并回读校验；
    /// 发现未完成 / 未知状态 run 时阻断新测试，要求人工核对，不自动清记录、不自动恢复未知状态。
    /// 本类只由宿主 Studio 阶段 check 运行；模型不在此执行 tests-run。
    /// </summary>
    public sealed class InitialWorldConfigurationTests
    {
        private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type SaveSystemType = Type.GetType("SaveSystem, Assembly-CSharp", true);
        private static readonly Type BridgeType = Type.GetType("VillageRuntimeBridge, Assembly-CSharp", true);
        private static readonly Type PlayerCollectionType = Type.GetType("PlayerCollection, Assembly-CSharp", true);
        private static readonly Type DataManagerType = Type.GetType("DataManager, Assembly-CSharp", true);
        private static readonly Type CatalogType = Type.GetType("InitialWorldConfigurationCatalog, Assembly-CSharp", true);
        private static readonly Type HeroInstanceType = Type.GetType("HeroInstance, Assembly-CSharp", true);

        private SaveFileGuard _guard;

        // ==================== 夹具 ====================

        [SetUp]
        public void SetUp()
        {
            // 必须先保护用户单槽，再做任何可能触碰存档的运行时初始化：
            // 即使后续 ResetRuntime 抛出，也不允许在无快照的情况下继续。
            _guard = SaveFileGuard.Capture();
            try
            {
                ResetRuntime();
            }
            catch
            {
                // SetUp 失败时 NUnit 不保证调用 TearDown；这里立即恢复并保留 guard 证据。
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

        // ==================== 1. 纯结构校验 ====================

        [Test]
        public void PureConfigurationValidationRejectsStructurallyInvalidInputs()
        {
            InitialWorldConfiguration valid = ConfigWithPeople(1, Building("building_farm", -4f, -6f));
            Assert.IsTrue(InitialWorldConfigurationValidator.TryValidate(valid, out string validDiagnostic), validDiagnostic);

            AssertRejected(null, "null");
            AssertRejected(Mutate(c => c.Gold = -1), "negative-gold");
            AssertRejected(Mutate(c => c.InitialIngredients = -1), "negative-ingredients");
            AssertRejected(Mutate(c => c.InitialFood = -1), "negative-food");
            AssertRejected(Mutate(c => c.InitialHeroId = null), "null-hero");
            AssertRejected(Mutate(c => c.People = null), "null-people");
            AssertRejected(Mutate(c => c.People.Add(null)), "null-person-entry");
            AssertRejected(Mutate(c => c.People.Add(new InitialWorldPerson { PersonDefinitionId = string.Empty })), "empty-person-id");

            InitialWorldConfiguration duplicate = ConfigWithPeople(1);
            duplicate.People.Add(new InitialWorldPerson { PersonDefinitionId = duplicate.People[0].PersonDefinitionId });
            Assert.IsFalse(InitialWorldConfigurationValidator.TryValidate(duplicate, out string duplicateDiagnostic));
            Assert.IsNotEmpty(duplicateDiagnostic, "重复人口必须给出诊断");

            AssertRejected(Mutate(c => c.Buildings = null), "null-buildings");
            AssertRejected(Mutate(c => c.Buildings.Add(null)), "null-building-entry");
            AssertRejected(Mutate(c => c.Buildings.Add(new InitialWorldBuilding { BuildingDefinitionId = string.Empty, WorldX = 0, WorldY = 0 })), "empty-building-id");
            AssertRejected(Mutate(c => c.Buildings.Add(new InitialWorldBuilding { BuildingDefinitionId = "building_farm", WorldX = float.NaN, WorldY = 0 })), "nan-x");
            AssertRejected(Mutate(c => c.Buildings.Add(new InitialWorldBuilding { BuildingDefinitionId = "building_farm", WorldX = 0, WorldY = float.PositiveInfinity })), "infinite-y");
        }

        // ==================== 2. 内容引用 / 布局校验 ====================

        [Test]
        public void ContentAndLayoutValidationRejectUnknownOrIllegalReferences()
        {
            AssertContentRejected(Mutate(c => c.People.Add(new InitialWorldPerson { PersonDefinitionId = "person_def_missing" })), "unknown-person");
            AssertContentRejected(Mutate(c => c.InitialHeroId = "hero_missing"), "unknown-hero");
            AssertContentRejected(Mutate(c => c.Buildings.Add(Building("building_missing", 0f, 0f))), "unknown-building");
            AssertContentRejected(Mutate(c => c.Buildings.Add(Building("building_farm", -4f, -6f))), "overlapping-buildings");
            AssertContentRejected(Mutate(c => c.Buildings.Add(Building("building_farm", 100f, 0f))), "out-of-bounds-building");

            InitialWorldConfiguration valid = ConfigWithPeople(1, Building("building_farm", -4f, -6f), Building("building_kitchen", 0f, -6f));
            Assert.IsTrue(TryValidateContent(valid, out string diagnostic), diagnostic);
        }

        // ==================== 3. 初始人口 / 金币 / 资源由配置 ====================

        [Test]
        public void NewGameCreatesConfiguredPopulationGoldAndResources()
        {
            List<PersonDefinition> roster = Roster();
            Assert.GreaterOrEqual(roster.Count, 2, "当前 PersonDefinition 初始名册至少需要 2 人用于本测试");

            InitialWorldConfiguration config = ConfigWithPeople(2);
            config.Gold = 77;
            config.InitialIngredients = 3;
            config.InitialFood = 2;

            object collection = NewGame(config);
            Assert.IsNotNull(collection, "合法初始配置必须创建玩家集合");

            IList characters = Characters(collection);
            Assert.AreEqual(2, characters.Count, "初始人口数量必须等于配置人口数量");
            CollectionAssert.AreEqual(
                config.People.Select(p => p.PersonDefinitionId).ToList(),
                characters.Cast<object>().Select(c => Field<string>(c, "PersonDefinitionId")).ToList(),
                "人物顺序 / PersonDefinitionId 必须与配置一致");

            var personIds = characters.Cast<object>().Select(c => Field<string>(c, "PersonId")).ToList();
            Assert.IsTrue(personIds.All(id => !string.IsNullOrEmpty(id)), "每个初始人物必须有真实 PersonId");
            Assert.AreEqual(personIds.Count, personIds.Distinct().Count(), "初始人物 PersonId 必须互不相同");

            IList heroes = Heroes(collection);
            Assert.AreEqual(1, heroes.Count, "初始英雄只创建一次");
            Assert.AreEqual(config.InitialHeroId, Field<string>(heroes[0], "HeroId"));
            Assert.IsNotEmpty(Field<string>(heroes[0], "PersonId"), "初始英雄必须有真实 PersonId");

            Assert.AreEqual(77, PlayerGold(collection), "初始金币必须由配置决定");
            Assert.AreEqual(3, BridgeResources().Ingredients, "InitialIngredients 必须由配置写入唯一资源 Authority");
            Assert.AreEqual(2, BridgeResources().Food, "InitialFood 必须由配置写入唯一资源 Authority");
        }

        // ==================== 4. Completed Farm / Kitchen 实例岗位 + Housing 无岗位 ====================

        [Test]
        public void NewGameCreatesCompletedFarmKitchenWorkplacesAndHousingWithoutWorkplace()
        {
            InitialWorldConfiguration config = ConfigWithPeople(3,
                Building("building_farm", -4f, -6f),
                Building("building_kitchen", 0f, -6f),
                Building("building_house", 4f, -6f));

            object collection = NewGame(config);
            Assert.IsNotNull(collection);

            List<object> buildings = BuildingObjects();
            Assert.AreEqual(3, buildings.Count, "配置中的每个初始建筑都必须创建为真实实例");
            CollectionAssert.AreEqual(
                new[] { "building_farm", "building_kitchen", "building_house" },
                buildings.Select(b => BuildingProp<string>(b, "BuildingDefinitionId")).ToArray());
            foreach (object building in buildings)
            {
                Assert.AreEqual("Completed", BuildingProp<object>(building, "LifecycleState").ToString(), "初始建筑必须是 Completed");
                Assert.IsNull(BuildingProp<string>(building, "ConstructionProjectId"), "Initial Completed 建筑不得携带施工绑定");
            }

            string farmId = BuildingProp<string>(buildings[0], "InstanceId");
            string kitchenId = BuildingProp<string>(buildings[1], "InstanceId");
            string houseId = BuildingProp<string>(buildings[2], "InstanceId");
            string farmWorkplace = "workplace:" + farmId + ":main";
            string kitchenWorkplace = "workplace:" + kitchenId + ":main";
            string houseWorkplace = "workplace:" + houseId + ":main";

            VillageAssignmentSystem assignments = Assignments();
            Assert.IsNotNull(assignments.GetWorkplaceDef(farmWorkplace), "Farm 必须登记实例 Workplace");
            Assert.IsNotNull(assignments.GetWorkplaceDef(kitchenWorkplace), "Kitchen 必须登记实例 Workplace");
            Assert.IsNull(assignments.GetWorkplaceDef(houseWorkplace), "Housing 定义没有岗位绑定，不得生成岗位");
            Assert.AreEqual(2, assignments.GetWorkplaceDef(farmWorkplace).Capacity, "Farm 容量继承既有 Profile（Capacity 单一权威）");
            Assert.AreEqual(1, assignments.GetWorkplaceDef(kitchenWorkplace).Capacity, "Kitchen 容量继承既有 Profile");

            List<VillageResourceWorkplaceDef> resourceDefs = ResourceWorkplaceDefs();
            Assert.IsTrue(resourceDefs.Any(d => d.WorkplaceId == farmWorkplace && d.Role == VillageResourceRole.Farm),
                "Farm 实例必须有资源岗位定义");
            Assert.IsTrue(resourceDefs.Any(d => d.WorkplaceId == kitchenWorkplace && d.Role == VillageResourceRole.Kitchen),
                "Kitchen 实例必须有资源岗位定义");

            IList characters = Characters(collection);
            string p1 = Field<string>(characters[0], "PersonId");
            string p2 = Field<string>(characters[1], "PersonId");
            string p3 = Field<string>(characters[2], "PersonId");
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(p1, farmWorkplace), "Farm 实例岗位必须可合法派工");
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(p2, kitchenWorkplace), "Kitchen 实例岗位必须可合法派工");
            Assert.AreEqual(VillageAssignmentResult.UnknownWorkplace, TryAssign(p3, houseWorkplace), "Housing 无岗位，派工必须被拒绝");
        }

        // ==================== 5. NewGame 单次 / 重入无重复 ====================

        [Test]
        public void NewGameIsSingleShotAndRepeatableWithoutDuplicateInstances()
        {
            InitialWorldConfiguration config = ConfigWithPeople(2,
                Building("building_farm", -4f, -6f),
                Building("building_kitchen", 0f, -6f));
            config.InitialIngredients = 1;
            config.InitialFood = 1;

            object first = NewGame(config);
            Assert.IsNotNull(first);
            Assert.AreEqual(2, BuildingObjects().Count);
            Assert.AreEqual(1, BridgeResources().Ingredients);

            object second = NewGame(config);
            Assert.IsNotNull(second);
            Assert.AreEqual(2, BuildingObjects().Count, "重入 NewGame 不得累加建筑实例");
            CollectionAssert.AreEqual(
                new[] { "building_instance_1", "building_instance_2" },
                BuildingObjects().Select(b => BuildingProp<string>(b, "InstanceId")).ToArray(),
                "新的 NewGame 必须从同一确定性分配器重新开始（不携带旧世界实例）");
            Assert.AreEqual(2, Characters(second).Count, "重入 NewGame 不得累加人口");
            Assert.AreEqual(1, BridgeResources().Ingredients, "重入 NewGame 必须回到配置资源初值（不累加）");
            Assert.IsFalse(ResourceWorkplaceDefs().Any(d => d.WorkplaceId == "workplace:building_instance_3:main"),
                "不得残留第三次实例岗位");
        }

        // ==================== 6. Continue 保持实例 / 位置 / 职责 / 资源 ====================

        [Test]
        public void ContinuePreservesInstanceIdentityPlacementAssignmentAndResources()
        {
            InitialWorldConfiguration config = ConfigWithPeople(2,
                Building("building_farm", -4f, -6f),
                Building("building_kitchen", 0f, -6f));
            config.InitialIngredients = 5;
            config.InitialFood = 4;

            object collection = NewGame(config);
            Assert.IsNotNull(collection);
            string personId = Field<string>(Characters(collection)[0], "PersonId");
            string farmId = BuildingProp<string>(BuildingObjects()[0], "InstanceId");
            string farmWorkplace = "workplace:" + farmId + ":main";
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, farmWorkplace));

            var savedPersonIds = Characters(collection).Cast<object>().Select(c => Field<string>(c, "PersonId")).ToList();
            var savedBuildings = BuildingSnapshot();
            Assert.IsTrue(Save(collection), "真实 Save 必须成功");

            // 用另一套配置把运行世界彻底换掉，再 Continue，确保恢复来自存档而非初始配置。
            InitialWorldConfiguration other = ConfigWithPeople(1, Building("building_house", 4f, -6f));
            other.InitialIngredients = 0;
            other.InitialFood = 0;
            Assert.IsNotNull(NewGame(other), "第一次 NewGame 用于扰动运行世界");
            Assert.AreEqual(1, BuildingObjects().Count);

            object loaded = Load();
            Assert.IsNotNull(loaded, "Continue / Load 必须成功");
            CollectionAssert.AreEqual(savedBuildings, BuildingSnapshot(), "Continue 必须保持 Building InstanceId / 定义 / 位置 / 状态");
            Assert.AreEqual(farmWorkplace, Assignments().GetWorkplaceId(personId), "Continue 必须保持实例岗位职责");
            Assert.AreEqual(5, BridgeResources().Ingredients, "Continue 必须保持 Ingredients");
            Assert.AreEqual(4, BridgeResources().Food, "Continue 必须保持 Food");
            CollectionAssert.AreEqual(savedPersonIds,
                Characters(loaded).Cast<object>().Select(c => Field<string>(c, "PersonId")).ToList(),
                "Continue 必须保持人物 PersonId");
        }

        // ==================== 7. 旧无 Buildings 档：不补建 + 静态职责保留 ====================

        [Test]
        public void LegacySaveWithoutBuildingsDoesNotRebuildInitialWorldAndKeepsStaticAssignments()
        {
            InitialWorldConfiguration config = ConfigWithPeople(2,
                Building("building_farm", -4f, -6f),
                Building("building_kitchen", 0f, -6f));
            object collection = NewGame(config);
            Assert.IsNotNull(collection);
            string personId = Field<string>(Characters(collection)[0], "PersonId");
            string farmWorkplace = "workplace:" + BuildingProp<string>(BuildingObjects()[0], "InstanceId") + ":main";
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, farmWorkplace),
                "正式新世界应先使用真实实例岗位派工");
            Assert.IsTrue(Save(collection));
            Assert.AreEqual(2, BuildingObjects().Count);

            // 构造「旧档」：Village 区块存在，但没有 Buildings 成员，且职责引用旧静态岗位
            // （逐字节写回用户单槽，guard 负责恢复）。这是真实旧保存来源，而不是从新世界补写。
            string path = SavePath();
            string json = File.ReadAllText(path);
            StringAssert.Contains(farmWorkplace, json, "存档必须真实包含实例岗位职责");
            string legacy = json.Replace(farmWorkplace, "ws_farm_01");
            legacy = NullOutBuildings(legacy);
            Assert.AreNotEqual(json, legacy, "测试必须真实移除 Buildings 成员并改写为旧静态职责");
            StringAssert.Contains("ws_farm_01", legacy, "旧档职责必须引用既有静态岗位");
            File.WriteAllText(path, legacy);

            ResetRuntime();
            object loaded = Load();
            Assert.IsNotNull(loaded, "无 Buildings 的旧档必须仍可 Continue");
            Assert.AreEqual(0, BuildingObjects().Count, "旧档缺 Buildings 时不得由初始配置补建实例");
            Assert.AreEqual("ws_farm_01", Assignments().GetWorkplaceId(personId), "旧档静态职责必须保留");
            Assert.IsNotNull(Assignments().GetWorkplaceDef("ws_farm_01"));
        }

        // ==================== 8. 非法配置零半状态 ====================

        [Test]
        public void InvalidConfigurationLeavesCurrentWorldUnchangedAndReportsFailure()
        {
            InitialWorldConfiguration valid = ConfigWithPeople(2,
                Building("building_farm", -4f, -6f),
                Building("building_kitchen", 0f, -6f));
            valid.InitialIngredients = 2;
            object collection = NewGame(valid);
            Assert.IsNotNull(collection);
            string personId = Field<string>(Characters(collection)[0], "PersonId");
            string farmWorkplace = "workplace:" + BuildingProp<string>(BuildingObjects()[0], "InstanceId") + ":main";
            Assert.AreEqual(VillageAssignmentResult.Success, TryAssign(personId, farmWorkplace));

            string before = WorldJson();

            InitialWorldConfiguration overlap = ConfigWithPeople(2,
                Building("building_farm", -4f, -6f),
                Building("building_farm", -4f, -6f));
            Assert.IsNull(NewGame(overlap), "重叠初始布局必须失败");
            Assert.AreEqual(before, WorldJson(), "初始布局非法时不得写入半世界");

            InitialWorldConfiguration unknownPerson = ConfigWithPeople(1);
            unknownPerson.People.Add(new InitialWorldPerson { PersonDefinitionId = "person_def_missing" });
            Assert.IsNull(NewGame(unknownPerson), "未知初始人口必须失败");
            Assert.AreEqual(before, WorldJson(), "初始人口非法时不得写入半世界");

            InitialWorldConfiguration outside = ConfigWithPeople(1, Building("building_house", 100f, 0f));
            Assert.IsNull(NewGame(outside), "越界初始建筑必须失败");
            Assert.AreEqual(before, WorldJson(), "初始建筑越界时不得写入半世界");
            Assert.AreEqual(2, BuildingObjects().Count, "失败后既有建筑实例必须保持");
            Assert.AreEqual(farmWorkplace, Assignments().GetWorkplaceId(personId), "失败后既有职责必须保持");
        }

        // ==================== 9. 测试重置 / EnsureInitialized 不种产品内容 ====================

        [Test]
        public void TestResetAndEnsureInitializedDoNotSeedProductWorld()
        {
            ResetRuntime();

            // ResetForTests 的真实契约 = 回到「未初始化」：Assignments / Resources 明确置 null
            // （VillageRuntimeBridge.ResetForTests 第 1391/1401 行），由 EnsureInitialized 才建立。
            // 因此必须先断言 Reset 状态，不能对 null 调用 WorkplaceDefs（宿主唯一失败的根因）。
            Assert.IsNull(AssignmentsOrNull(),
                "测试重置后运行时必须处于未初始化状态：Assignments 必须为 null（不自动种默认岗位）");
            Assert.IsNull(ResourcesOrNull(),
                "测试重置后运行时必须处于未初始化状态：Resources 必须为 null");
            Assert.AreEqual(0, BuildingObjects().Count, "测试重置后不得存在初始建筑");

            EnsureInitialized();
            Assert.IsNotNull(AssignmentsOrNull(), "EnsureInitialized 必须建立运行时 Assignments（默认 Test 岗位目录）");
            EnsureInitialized();
            Assert.AreEqual(0, BuildingObjects().Count, "EnsureInitialized 不得创建初始建筑");
            Assert.IsFalse(Assignments().WorkplaceDefs.Any(d => d.WorkplaceId.StartsWith("workplace:", StringComparison.Ordinal)),
                "EnsureInitialized 不得创建产品内容岗位（实例岗位只能由显式 NewGame 配置创建）");
            Assert.AreEqual(0, BridgeResources().Ingredients);
            Assert.AreEqual(0, BridgeResources().Food);

            // 默认配置现在必须来自正式生成链（XLSX → Compiler → Builder → Registry）。
            // 本测试验证「默认来源一致性」，不再固定任何人数 / 金币 / 建筑数量：
            // 期望值全部从同一个生成容器读回，而不是在测试里再写一份默认内容。
            RRPG.StaticConfig.StaticConfigRegistry.Reset();
            Assert.IsTrue(TryReadDefault(out InitialWorldConfiguration defaults, out string defaultDiagnostic),
                "默认初始世界配置必须从生成链读取成功：" + defaultDiagnostic);
            Assert.IsTrue(RRPG.StaticConfig.StaticConfigRegistry.TryGetInitialWorld(
                    out RRPG.StaticConfig.InitialWorldConfigDatabase generated, out string generatedDiagnostic),
                "生成容器必须可用：" + generatedDiagnostic);
            Assert.IsTrue(generated.TryCopyDefaultProfile(
                    out RRPG.StaticConfig.GeneratedInitialWorldProfile profile, out string profileDiagnostic),
                "生成容器必须恰好有一个默认 profile：" + profileDiagnostic);

            Assert.AreEqual(profile.InitialHeroId, defaults.InitialHeroId, "默认英雄必须与生成 profile 一致");
            Assert.AreEqual(profile.Gold, defaults.Gold, "默认金币必须与生成 profile 一致（不是旧硬编码 100）");
            Assert.AreEqual(profile.InitialIngredients, defaults.InitialIngredients);
            Assert.AreEqual(profile.InitialFood, defaults.InitialFood);

            CollectionAssert.AreEqual(
                generated.CopyPopulation().Select(p => p.PersonDefinitionId).ToList(),
                defaults.People.Select(p => p.PersonDefinitionId).ToList(),
                "默认人口必须与生成容器同源（数量 / 名单均不写死）");
            CollectionAssert.AreEqual(
                generated.CopyBuildings().Select(b => b.BuildingDefinitionId + "@" + b.WorldX + "," + b.WorldY).ToList(),
                defaults.Buildings.Select(b => b.BuildingDefinitionId + "@" + b.WorldX + "," + b.WorldY).ToList(),
                "默认建筑布局必须与生成容器同源（默认值不再是 0 建筑）");

            Assert.Greater(defaults.People.Count, 0, "正式默认人口必须非空");
            Assert.Greater(defaults.Buildings.Count, 0, "正式默认建筑不得为 0（旧空默认只属过渡状态）");
            CollectionAssert.Contains(defaults.Buildings.Select(b => b.BuildingDefinitionId).ToList(), "building_farm");
            CollectionAssert.Contains(defaults.Buildings.Select(b => b.BuildingDefinitionId).ToList(), "building_kitchen");
            CollectionAssert.Contains(defaults.Buildings.Select(b => b.BuildingDefinitionId).ToList(), "building_house");
            Assert.IsFalse(defaults.Buildings.Any(b => b.BuildingDefinitionId == "building_workshop"),
                "workshop / 铁匠铺不得进入正式默认初始布局");
            Assert.IsNotEmpty(defaults.InitialHeroId);
        }

        // ==================== 10. Hero 选择替换时序 ====================

        [Test]
        public void HeroReplacementAfterNewGameKeepsSingleHeroAndPersonIds()
        {
            InitialWorldConfiguration config = ConfigWithPeople(2);
            object collection = NewGame(config);
            Assert.IsNotNull(collection);

            IList heroes = Heroes(collection);
            Assert.AreEqual(1, heroes.Count, "NewGame 默认只创建一次英雄");
            object oldHero = heroes[0];
            string oldHeroPersonId = Field<string>(oldHero, "PersonId");
            string heroId = Field<string>(oldHero, "HeroId");
            string displayName = Field<string>(oldHero, "DisplayName");
            int maxHp = Field<int>(oldHero, "MaxHP");
            Assert.IsNotEmpty(oldHeroPersonId);

            var characterPersonIds = Characters(collection).Cast<object>().Select(c => Field<string>(c, "PersonId")).ToList();

            // 与 HeroSelectionUI.OnConfirm 相同替换时序：清空默认英雄 → 加入所选英雄（新实例 = 新 PersonId）。
            heroes.Clear();
            object selectedHero = Activator.CreateInstance(HeroInstanceType, new object[] { heroId, displayName, maxHp });
            heroes.Add(selectedHero);
            string selectedPersonId = Field<string>(selectedHero, "PersonId");

            Assert.AreEqual(1, heroes.Count, "替换后仍只能有一个英雄（不重复初始化）");
            Assert.IsNotEmpty(selectedPersonId, "替换后的英雄必须拥有 PersonId");
            Assert.AreNotEqual(oldHeroPersonId, selectedPersonId, "替换后的英雄必须是新的具体人物实例");
            CollectionAssert.AreEqual(characterPersonIds,
                Characters(collection).Cast<object>().Select(c => Field<string>(c, "PersonId")).ToList(),
                "替换英雄不得影响其它人物的 PersonId");
            Assert.AreEqual(2, Characters(collection).Count, "替换英雄不得改变初始人口数量");

            Assert.IsTrue(Save(collection));
            object loaded = Load();
            Assert.IsNotNull(loaded);
            Assert.AreEqual(1, Heroes(loaded).Count, "Continue 后英雄数量必须保持 1");
            Assert.AreEqual(selectedPersonId, Field<string>(Heroes(loaded)[0], "PersonId"), "Continue 不得丢失英雄 PersonId");
            CollectionAssert.AreEqual(characterPersonIds,
                Characters(loaded).Cast<object>().Select(c => Field<string>(c, "PersonId")).ToList(),
                "Continue 不得丢失其它人物 PersonId");
        }

        // ==================== 断言 / 反射辅助 ====================

        private static void AssertRejected(InitialWorldConfiguration config, string label)
        {
            Assert.IsFalse(InitialWorldConfigurationValidator.TryValidate(config, out string diagnostic), "结构校验必须拒绝：" + label);
            Assert.IsNotEmpty(diagnostic, "结构校验拒绝必须给出诊断：" + label);
        }

        private static void AssertContentRejected(InitialWorldConfiguration config, string label)
        {
            Assert.IsFalse(TryValidateContent(config, out string diagnostic), "内容 / 布局校验必须拒绝：" + label);
            Assert.IsNotEmpty(diagnostic, "内容 / 布局校验拒绝必须给出诊断：" + label);
        }

        private static bool TryValidateContent(InitialWorldConfiguration config, out string diagnostic)
        {
            var args = new object[] { config, null };
            bool result = (bool)CatalogType.GetMethod("TryValidate", Static).Invoke(null, args);
            diagnostic = args[1] as string;
            return result;
        }

        private static bool TryReadDefault(out InitialWorldConfiguration config, out string diagnostic)
        {
            var args = new object[] { null, null };
            bool result = (bool)CatalogType.GetMethod("TryReadDefault", Static).Invoke(null, args);
            config = args[0] as InitialWorldConfiguration;
            diagnostic = args[1] as string;
            return result;
        }

        private static InitialWorldConfiguration ConfigWithPeople(int count, params InitialWorldBuilding[] buildings)
        {
            var config = new InitialWorldConfiguration();
            foreach (PersonDefinition definition in Roster().Take(count))
                config.People.Add(new InitialWorldPerson { PersonDefinitionId = definition.PersonDefinitionId });
            if (buildings != null) config.Buildings.AddRange(buildings);
            return config;
        }

        private static InitialWorldBuilding Building(string definitionId, float x, float y)
        {
            return new InitialWorldBuilding { BuildingDefinitionId = definitionId, WorldX = x, WorldY = y };
        }

        private static InitialWorldConfiguration Mutate(Action<InitialWorldConfiguration> mutation)
        {
            InitialWorldConfiguration config = ConfigWithPeople(1, Building("building_farm", -4f, -6f));
            mutation(config);
            return config;
        }

        private static object NewGame(InitialWorldConfiguration config)
        {
            return Invoke(SaveSystemType.GetMethod("NewGame", Static, null, new[] { typeof(InitialWorldConfiguration) }, null), null, config);
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

        private static List<PersonDefinition> Roster()
        {
            object dm = DataManagerType.GetProperty("Instance", Static).GetValue(null, null);
            var roster = (IEnumerable)DataManagerType.GetProperty("InitialPersonRoster", Instance).GetValue(dm, null);
            return roster.Cast<PersonDefinition>().ToList();
        }

        private static IList Characters(object collection) => (IList)PlayerCollectionType.GetField("OwnedCharacters", Instance).GetValue(collection);
        private static IList Heroes(object collection) => (IList)PlayerCollectionType.GetField("OwnedHeroes", Instance).GetValue(collection);
        private static int PlayerGold(object collection) => (int)PlayerCollectionType.GetField("Gold", Instance).GetValue(collection);

        private static T Field<T>(object target, string name) => (T)target.GetType().GetField(name, Instance).GetValue(target);

        private static VillageAssignmentSystem AssignmentsOrNull() =>
            (VillageAssignmentSystem)BridgeType.GetField("Assignments", Static).GetValue(null);

        private static VillageAssignmentSystem Assignments() => AssignmentsOrNull();

        private static VillageResourceState ResourcesOrNull() =>
            (VillageResourceState)BridgeType.GetField("Resources", Static).GetValue(null);

        private static VillageResourceState BridgeResources() => ResourcesOrNull();

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

        private static List<string> BuildingSnapshot()
        {
            return BuildingObjects().Select(b => string.Join("|",
                BuildingProp<string>(b, "InstanceId"),
                BuildingProp<string>(b, "BuildingDefinitionId"),
                BuildingProp<float>(b, "WorldX").ToString("R"),
                BuildingProp<float>(b, "WorldY").ToString("R"),
                BuildingProp<object>(b, "LifecycleState").ToString())).ToList();
        }

        private static string WorldJson()
        {
            object save = BridgeType.GetMethod("BuildSaveData", Static).Invoke(null, null);
            return JsonUtility.ToJson(save);
        }

        private static string SavePath() => (string)SaveSystemType.GetMethod("GetSavePath", Static).Invoke(null, null);

        private static bool Save(object collection) =>
            (bool)SaveSystemType.GetMethod("Save", Static).Invoke(null, new[] { collection });

        private static object Load() => SaveSystemType.GetMethod("Load", Static).Invoke(null, null);

        /// <summary>把 Save JSON 中 Village 区块的 Buildings 成员替换为 null（构造「旧无 Buildings 档」）。</summary>
        private static string NullOutBuildings(string json)
        {
            int key = json.IndexOf("\"Buildings\"", StringComparison.Ordinal);
            if (key < 0) return json;
            int colon = json.IndexOf(':', key);
            int start = colon + 1;
            while (start < json.Length && char.IsWhiteSpace(json[start])) start++;
            if (start >= json.Length || json[start] != '{') return json;

            int depth = 0;
            bool quoted = false;
            int end = start;
            for (; end < json.Length; end++)
            {
                char c = json[end];
                if (quoted && c == '\\') { end++; continue; }
                if (c == '"') { quoted = !quoted; continue; }
                if (quoted) continue;
                if (c == '{') depth++;
                else if (c == '}' && --depth == 0) { end++; break; }
            }
            return json.Substring(0, start) + "null" + json.Substring(end);
        }

        // ==================== 用户存档持久快照保护 ====================

        private static string RepoRoot => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));
        private static string GuardRoot => Path.Combine(RepoRoot, "Development", "StudioRuns", "Initialization-20261027", "save-guard");

        internal sealed class SaveFileGuard
        {
            private readonly string _path;
            private readonly byte[] _bytes;
            private readonly string _hash;
            private readonly string _runDir;
            private bool _restored;

            private SaveFileGuard(string path, byte[] bytes, string hash, string runDir)
            {
                _path = path;
                _bytes = bytes;
                _hash = hash;
                _runDir = runDir;
            }

            public static SaveFileGuard Capture()
            {
                // 先阻断未知 / 未完成 run，绝不覆盖他人快照。
                BlockOnUnfinishedRuns();

                string path = SavePath();
                byte[] bytes = File.Exists(path) ? File.ReadAllBytes(path) : null;
                string hash = bytes == null ? "absent" : Sha256(bytes);

                // 每次 run 使用唯一目录；原档备份用 CreateNew 落盘，已存在即失败而非覆盖。
                string stamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
                string dir = Path.Combine(GuardRoot, stamp + "-" + Guid.NewGuid().ToString("N").Substring(0, 8));
                Directory.CreateDirectory(dir);
                string backupPath = Path.Combine(dir, "original-save.bin");
                try
                {
                    if (bytes != null) WriteNewFile(backupPath, bytes);
                    // 只有「回读字节逐字节一致 + SHA256 一致」才允许标记 captured。
                    VerifySnapshot(backupPath, bytes, hash);
                    WriteManifest(dir, path, bytes, hash, "captured");
                }
                catch (Exception ex)
                {
                    // 保留原 manifest 字段结构，不清洗、不删除本次 run；异常继续上抛阻断测试。
                    try { WriteManifest(dir, path, bytes, hash, "capture-failed", ex.GetType().Name + ": " + ex.Message); } catch { }
                    throw;
                }
                return new SaveFileGuard(path, bytes, hash, dir);
            }

            public void Restore()
            {
                if (_restored) return;
                try
                {
                    if (_bytes == null)
                    {
                        if (File.Exists(_path)) File.Delete(_path);
                    }
                    else
                    {
                        File.WriteAllBytes(_path, _bytes);
                    }
                    string actual = File.Exists(_path) ? Sha256(File.ReadAllBytes(_path)) : "absent";
                    if (actual != _hash)
                    {
                        WriteManifest(_runDir, _path, _bytes, _hash, "restore-failed");
                        Assert.Fail("用户存档恢复回读不一致：期望 " + _hash + "，实际 " + actual + "；快照保留在 " + _runDir);
                    }
                    _restored = true;
                    WriteManifest(_runDir, _path, _bytes, _hash, "restored");
                }
                catch (Exception ex)
                {
                    try { WriteManifest(_runDir, _path, _bytes, _hash, "restore-failed", ex.GetType().Name); } catch { }
                    throw;
                }
            }

            private static void BlockOnUnfinishedRuns()
            {
                if (!Directory.Exists(GuardRoot)) return;
                var unfinished = new List<string>();
                foreach (string dir in Directory.GetDirectories(GuardRoot))
                {
                    string manifest = Path.Combine(dir, "manifest.json");
                    if (!File.Exists(manifest))
                    {
                        unfinished.Add(Path.GetFileName(dir) + "(missing-manifest)");
                        continue;
                    }
                    // 保护根被两个测试守卫共用，manifest 存在两种真实格式：
                    //   SaveFileGuard 紧凑写 → {"status":"restored",...}
                    //   InitializationProductPlayTests.SaveSlotGuard 缩进写 → "status": "restored"
                    // 两者都表示已完成恢复。这里先去掉空白再精确匹配 status 字段，避免把
                    // 另一种守卫已完成的 run 误判为未完成；语义不放松：captured /
                    // capture-failed / restore-failed / 缺失 manifest 仍然阻断。
                    string text = File.ReadAllText(manifest)
                        .Replace(" ", string.Empty).Replace("\t", string.Empty)
                        .Replace("\r", string.Empty).Replace("\n", string.Empty);
                    if (!text.Contains("\"status\":\"restored\""))
                        unfinished.Add(Path.GetFileName(dir));
                }
                if (unfinished.Count > 0)
                {
                    Assert.Fail("存在未完成的初始世界存档保护 run，已阻止新测试以免覆盖当前档；请人工核对后再继续："
                        + string.Join("; ", unfinished));
                }
            }

            private static void WriteManifest(string dir, string sourcePath, byte[] bytes, string hash, string status, string error = null)
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string manifest = "{\"status\":\"" + status + "\""
                    + ",\"sourcePath\":\"" + sourcePath.Replace("\\", "/") + "\""
                    + ",\"sourceExisted\":" + (bytes != null ? "true" : "false")
                    + ",\"bytes\":" + (bytes != null ? bytes.Length : 0)
                    + ",\"sha256\":\"" + hash + "\""
                    + ",\"updatedUtc\":\"" + DateTime.UtcNow.ToString("o") + "\""
                    + ",\"error\":\"" + (error ?? string.Empty).Replace("\\", "/") + "\""
                    + "}";
                File.WriteAllText(Path.Combine(dir, "manifest.json"), manifest);
            }

            private static void WriteNewFile(string path, byte[] bytes)
            {
                using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
            }

            private static void VerifySnapshot(string backupPath, byte[] expected, string expectedHash)
            {
                if (expected == null)
                {
                    if (File.Exists(backupPath))
                        throw new InvalidOperationException("源档不存在但备份文件已存在：" + backupPath);
                    return;
                }
                if (!File.Exists(backupPath))
                    throw new InvalidOperationException("原档备份未落盘：" + backupPath);

                byte[] actual = File.ReadAllBytes(backupPath);
                string actualHash = Sha256(actual);
                if (actual.Length != expected.Length || actualHash != expectedHash)
                    throw new InvalidOperationException("原档备份回读不一致：期望 " + expected.Length + "B/" + expectedHash
                        + "，实际 " + actual.Length + "B/" + actualHash + "；备份保留在 " + backupPath);
                for (int i = 0; i < actual.Length; i++)
                {
                    if (actual[i] != expected[i])
                        throw new InvalidOperationException("原档备份回读字节不一致：offset=" + i + "；备份保留在 " + backupPath);
                }
            }

            private static string Sha256(byte[] bytes)
            {
                using (var sha = SHA256.Create())
                    return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
