using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RRPG.StaticConfig;
using UnityEngine;

namespace RRPG.Initialization.Tests
{
    /// <summary>
    /// Initial World 正式配置生产链（Container → Registry → Catalog）Edit 契约测试。
    ///
    /// 覆盖本包新增的**生成数据容器**语义，不依赖真实默认内容：
    ///   - 读回：容器 → 深拷贝 → 纯 Runtime 模型，顺序 / 数值一致；
    ///   - 防外部变更：修改返回的副本不会污染容器，下一次读取仍是原值；
    ///   - 非法内容拒绝：空 / 多默认 profile / 重复序号 / 重复人物 / 负数 / NaN；
    ///   - 缺失生成内容 fail-closed：不静默变成 0 建筑；
    ///   - 显式配置独立：SaveSystem.NewGame(config) 不读取默认生成内容。
    ///
    /// 边界：本类不写用户存档、不跑 tests-run、不调用 Shell / Process；
    /// 由宿主 Studio 阶段在真实生成（Compiler build → Unity Builder）后运行。
    /// </summary>
    public sealed class InitialWorldConfigPipelineTests
    {
        private const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type SaveSystemType = Type.GetType("SaveSystem, Assembly-CSharp", true);
        private static readonly Type BridgeType = Type.GetType("VillageRuntimeBridge, Assembly-CSharp", true);
        private static readonly Type CatalogType = Type.GetType("InitialWorldConfigurationCatalog, Assembly-CSharp", true);
        private static readonly Type DataManagerType = Type.GetType("DataManager, Assembly-CSharp", true);
        private static readonly Type PlayerCollectionType = Type.GetType("PlayerCollection, Assembly-CSharp", true);

        private readonly List<UnityEngine.Object> _created = new List<UnityEngine.Object>();

        [SetUp]
        public void SetUp()
        {
            // 每例从干净 Registry 开始；不读取真实默认内容（需要时用内存覆盖显式注入）。
            StaticConfigRegistry.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            SetOverride(null);
            try { ResetRuntime(); }
            finally
            {
                foreach (UnityEngine.Object asset in _created)
                {
                    if (asset != null) UnityEngine.Object.DestroyImmediate(asset);
                }
                _created.Clear();
            }
        }

        // ==================== 1. 容器读回 / 深拷贝 ====================

        [Test]
        public void GeneratedContainerReadbackIsDeepCopiedAndOrdered()
        {
            InitialWorldConfigDatabase database = CreateDatabase(
                new[] { Profile("iw_default", true, "hero_001", 100, 1, 2) },
                new[]
                {
                    new GeneratedInitialPopulationEntry { StableIndex = 3, PersonDefinitionId = "person_c" },
                    new GeneratedInitialPopulationEntry { StableIndex = 1, PersonDefinitionId = "person_a" },
                    new GeneratedInitialPopulationEntry { StableIndex = 2, PersonDefinitionId = "person_b" }
                },
                new[]
                {
                    new GeneratedInitialBuildingEntry { StableIndex = 2, BuildingDefinitionId = "building_kitchen", WorldX = 0, WorldY = -6 },
                    new GeneratedInitialBuildingEntry { StableIndex = 1, BuildingDefinitionId = "building_farm", WorldX = -4, WorldY = -6 }
                });

            List<GeneratedInitialPopulationEntry> population = database.CopyPopulation();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, population.Select(p => p.StableIndex).ToArray(),
                "人口副本必须按稳定序号升序，顺序不依赖物理行序");
            CollectionAssert.AreEqual(new[] { "person_a", "person_b", "person_c" },
                population.Select(p => p.PersonDefinitionId).ToArray());

            List<GeneratedInitialBuildingEntry> buildings = database.CopyBuildings();
            CollectionAssert.AreEqual(new[] { "building_farm", "building_kitchen" },
                buildings.Select(b => b.BuildingDefinitionId).ToArray());

            // 外部修改副本 → 不得污染容器。
            population[0].PersonDefinitionId = "person_mutated";
            population.Clear();
            buildings[0].WorldX = 999f;

            Assert.AreEqual("person_a", database.CopyPopulation()[0].PersonDefinitionId,
                "人口读取必须是深拷贝：外部修改不能污染生成数据");
            Assert.AreEqual(-4f, database.CopyBuildings()[0].WorldX,
                "建筑读取必须是深拷贝：外部修改不能污染生成数据");
            Assert.IsTrue(database.TryCopyDefaultProfile(out GeneratedInitialWorldProfile profile, out string diagnostic), diagnostic);
            profile.Gold = 999;
            Assert.IsTrue(database.TryCopyDefaultProfile(out GeneratedInitialWorldProfile again, out _));
            Assert.AreEqual(100, again.Gold, "默认 profile 读取必须是深拷贝");
        }

        // ==================== 2. 非法生成内容拒绝 ====================

        [Test]
        public void IllegalGeneratedContentIsRejectedBeforePublishing()
        {
            AssertRejected(new GeneratedInitialWorldProfile[0], Population("person_a"), Buildings("building_farm"));
            AssertRejected(
                new[]
                {
                    Profile("iw_a", true, "hero_001", 100),
                    Profile("iw_b", true, "hero_001", 100)
                },
                Population("person_a"), Buildings("building_farm"));
            AssertRejected(
                new[] { Profile("iw_a", false, "hero_001", 100) },
                Population("person_a"), Buildings("building_farm"));
            AssertRejected(
                new[] { Profile("iw_a", true, "hero_001", -5) },
                Population("person_a"), Buildings("building_farm"));
            AssertRejected(
                new[] { Profile("iw_a", true, "hero_001", 100) },
                new[]
                {
                    new GeneratedInitialPopulationEntry { StableIndex = 1, PersonDefinitionId = "person_a" },
                    new GeneratedInitialPopulationEntry { StableIndex = 1, PersonDefinitionId = "person_b" }
                },
                Buildings("building_farm"));
            AssertRejected(
                new[] { Profile("iw_a", true, "hero_001", 100) },
                new[]
                {
                    new GeneratedInitialPopulationEntry { StableIndex = 1, PersonDefinitionId = "person_a" },
                    new GeneratedInitialPopulationEntry { StableIndex = 2, PersonDefinitionId = "person_a" }
                },
                Buildings("building_farm"));
            AssertRejected(
                new[] { Profile("iw_a", true, "hero_001", 100) },
                Population("person_a"),
                new[]
                {
                    new GeneratedInitialBuildingEntry { StableIndex = 1, BuildingDefinitionId = "building_farm", WorldX = float.NaN, WorldY = 0 }
                });
            AssertRejected(
                new[] { Profile("iw_a", true, "hero_001", 100) },
                Population("person_a"),
                new GeneratedInitialBuildingEntry[0]);
        }

        // ==================== 3. 缺失生成内容 fail-closed ====================

        [Test]
        public void EmptyGeneratedContainerIsRejectedByCatalogAndRegistry()
        {
            InitialWorldConfigDatabase empty = ScriptableObject.CreateInstance<InitialWorldConfigDatabase>();
            _created.Add(empty);
            empty.ClearGeneratedContent(RRPGStaticConfigRoot.SupportedFormatVersion);
            SetOverride(empty);

            Assert.IsFalse(StaticConfigRegistry.TryGetInitialWorld(out _, out string registryDiagnostic),
                "空生成容器不得被当作可用初始世界配置");
            Assert.IsTrue(registryDiagnostic.Contains("为空"), "空容器必须给出明确诊断：" + registryDiagnostic);

            Assert.IsFalse(TryBuildFromGenerated(null, out _, out string nullDiagnostic));
            Assert.IsTrue(nullDiagnostic.Contains("null"), "null 容器必须给出明确诊断：" + nullDiagnostic);

            Assert.IsFalse(TryReadDefault(out _, out string defaultDiagnostic),
                "默认读取必须 fail-closed，不得静默退回 0 建筑 / 旧硬编码");
            Assert.IsTrue(defaultDiagnostic.Contains("为空"), "默认读取失败必须给出明确诊断：" + defaultDiagnostic);
        }

        // ==================== 4. 默认读取确实来自生成容器 ====================

        [Test]
        public void DefaultReadComesFromGeneratedContainerAndIsIndependentPerCall()
        {
            List<PersonDefinition> roster = Roster();
            Assert.GreaterOrEqual(roster.Count, 2, "当前初始名册至少需要 2 人用于本测试");

            InitialWorldConfigDatabase database = CreateDatabase(
                new[] { Profile("iw_generated", true, "hero_001", 321, 4, 5) },
                roster.Take(2).Select((p, i) => new GeneratedInitialPopulationEntry
                {
                    StableIndex = i + 1,
                    PersonDefinitionId = p.PersonDefinitionId
                }).ToArray(),
                new[]
                {
                    new GeneratedInitialBuildingEntry { StableIndex = 1, BuildingDefinitionId = "building_farm", WorldX = -4, WorldY = -6 },
                    new GeneratedInitialBuildingEntry { StableIndex = 2, BuildingDefinitionId = "building_kitchen", WorldX = 0, WorldY = -6 },
                    new GeneratedInitialBuildingEntry { StableIndex = 3, BuildingDefinitionId = "building_house", WorldX = 4, WorldY = -6 }
                });
            SetOverride(database);

            Assert.IsTrue(TryReadDefault(out InitialWorldConfiguration config, out string diagnostic), diagnostic);
            Assert.AreEqual("hero_001", config.InitialHeroId, "默认英雄必须来自生成 profile");
            Assert.AreEqual(321, config.Gold, "默认金币必须来自生成 profile，不是旧硬编码 100");
            Assert.AreEqual(4, config.InitialIngredients);
            Assert.AreEqual(5, config.InitialFood);
            CollectionAssert.AreEqual(roster.Take(2).Select(p => p.PersonDefinitionId).ToArray(),
                config.People.Select(p => p.PersonDefinitionId).ToArray(),
                "默认人口必须按生成容器的稳定序号映射");
            CollectionAssert.AreEqual(new[] { "building_farm", "building_kitchen", "building_house" },
                config.Buildings.Select(b => b.BuildingDefinitionId).ToArray());

            // 修改第一次读取到的模型 → 不得影响容器 / 下一次读取。
            config.Gold = 1;
            config.People.Clear();
            config.Buildings.Clear();
            Assert.IsTrue(TryReadDefault(out InitialWorldConfiguration second, out string secondDiagnostic), secondDiagnostic);
            Assert.AreEqual(321, second.Gold, "Catalog 返回的模型必须是独立副本");
            Assert.AreEqual(2, second.People.Count);
            Assert.AreEqual(3, second.Buildings.Count);
        }

        // ==================== 5. 显式配置独立于默认内容 ====================

        [Test]
        public void ExplicitConfigurationDoesNotReadGeneratedDefault()
        {
            List<PersonDefinition> roster = Roster();
            Assert.GreaterOrEqual(roster.Count, 2, "当前初始名册至少需要 2 人用于本测试");

            // 默认容器故意与显式配置完全不同：若 NewGame(config) 偷偷读默认，断言会失败。
            InitialWorldConfigDatabase unrelated = CreateDatabase(
                new[] { Profile("iw_other", true, "hero_001", 999, 0, 0) },
                new[] { new GeneratedInitialPopulationEntry { StableIndex = 1, PersonDefinitionId = roster[0].PersonDefinitionId } },
                new[] { new GeneratedInitialBuildingEntry { StableIndex = 1, BuildingDefinitionId = "building_house", WorldX = 4, WorldY = -6 } });
            SetOverride(unrelated);

            var explicitConfig = new InitialWorldConfiguration
            {
                InitialHeroId = "hero_001",
                Gold = 7,
                InitialIngredients = 2,
                InitialFood = 3
            };
            explicitConfig.People.Add(new InitialWorldPerson { PersonDefinitionId = roster[0].PersonDefinitionId });
            explicitConfig.People.Add(new InitialWorldPerson { PersonDefinitionId = roster[1].PersonDefinitionId });
            explicitConfig.Buildings.Add(new InitialWorldBuilding { BuildingDefinitionId = "building_farm", WorldX = -4f, WorldY = -6f });
            explicitConfig.Buildings.Add(new InitialWorldBuilding { BuildingDefinitionId = "building_kitchen", WorldX = 0f, WorldY = -6f });

            ResetRuntime();
            object collection = NewGame(explicitConfig);
            Assert.IsNotNull(collection, "显式合法配置必须能独立创建世界");

            Assert.AreEqual(7, (int)PlayerCollectionType.GetField("Gold", Instance).GetValue(collection),
                "显式金币不得被默认生成内容覆盖");
            Assert.AreEqual(2, Characters(collection).Count, "显式人口不得被默认生成内容覆盖");
            List<object> buildings = BuildingObjects();
            Assert.AreEqual(2, buildings.Count, "显式建筑不得被默认生成内容覆盖");
            CollectionAssert.AreEqual(new[] { "building_farm", "building_kitchen" },
                buildings.Select(b => (string)b.GetType().GetProperty("BuildingDefinitionId", Instance).GetValue(b, null)).ToArray());

            var resources = (VillageResourceState)BridgeType.GetField("Resources", Static).GetValue(null);
            Assert.AreEqual(2, resources.Ingredients);
            Assert.AreEqual(3, resources.Food);

            ResetRuntime();
        }

        // ==================== 辅助 ====================

        private static void AssertRejected(GeneratedInitialWorldProfile[] profiles,
            GeneratedInitialPopulationEntry[] population, GeneratedInitialBuildingEntry[] buildings)
        {
            Assert.IsFalse(InitialWorldGeneratedConfigValidator.TryValidate(
                profiles.ToList(), population.ToList(), buildings.ToList(), out string diagnostic),
                "非法生成数据必须被拒绝");
            Assert.IsNotEmpty(diagnostic, "拒绝必须给出诊断");

            InitialWorldConfigDatabase database = ScriptableObject.CreateInstance<InitialWorldConfigDatabase>();
            try
            {
                Assert.Throws<ArgumentException>(() => database.ApplyGeneratedContent(
                        RRPGStaticConfigRoot.SupportedFormatVersion, "w", "p", "b",
                        profiles.ToList(), population.ToList(), buildings.ToList()),
                    "非法生成数据不得被写入容器");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(database);
            }
        }

        private InitialWorldConfigDatabase CreateDatabase(
            GeneratedInitialWorldProfile[] profiles,
            GeneratedInitialPopulationEntry[] population,
            GeneratedInitialBuildingEntry[] buildings)
        {
            InitialWorldConfigDatabase database = ScriptableObject.CreateInstance<InitialWorldConfigDatabase>();
            _created.Add(database);
            database.ApplyGeneratedContent(RRPGStaticConfigRoot.SupportedFormatVersion, "w-sha", "p-sha", "b-sha",
                profiles.ToList(), population.ToList(), buildings.ToList());
            return database;
        }

        private static GeneratedInitialWorldProfile Profile(string id, bool isDefault, string hero, int gold,
            int ingredients = 0, int food = 0)
        {
            return new GeneratedInitialWorldProfile
            {
                ProfileId = id,
                IsDefault = isDefault,
                InitialHeroId = hero,
                Gold = gold,
                InitialIngredients = ingredients,
                InitialFood = food
            };
        }

        private static GeneratedInitialPopulationEntry[] Population(params string[] definitions)
        {
            return definitions.Select((definition, index) => new GeneratedInitialPopulationEntry
            {
                StableIndex = index + 1,
                PersonDefinitionId = definition
            }).ToArray();
        }

        private static GeneratedInitialBuildingEntry[] Buildings(params string[] definitions)
        {
            return definitions.Select((definition, index) => new GeneratedInitialBuildingEntry
            {
                StableIndex = index + 1,
                BuildingDefinitionId = definition,
                WorldX = -4 + index * 4,
                WorldY = -6
            }).ToArray();
        }

        private static void SetOverride(InitialWorldConfigDatabase database)
        {
            typeof(StaticConfigRegistry)
                .GetMethod("SetInitialWorldOverrideForTests", Static)
                .Invoke(null, new object[] { database });
        }

        private static bool TryReadDefault(out InitialWorldConfiguration config, out string diagnostic)
        {
            var args = new object[] { null, null };
            bool result = (bool)CatalogType.GetMethod("TryReadDefault", Static).Invoke(null, args);
            config = args[0] as InitialWorldConfiguration;
            diagnostic = args[1] as string;
            return result;
        }

        private static bool TryBuildFromGenerated(InitialWorldConfigDatabase database,
            out InitialWorldConfiguration config, out string diagnostic)
        {
            var args = new object[] { database, null, null };
            bool result = (bool)CatalogType.GetMethod("TryBuildFromGenerated", Static).Invoke(null, args);
            config = args[1] as InitialWorldConfiguration;
            diagnostic = args[2] as string;
            return result;
        }

        private static object NewGame(InitialWorldConfiguration config)
        {
            return Invoke(SaveSystemType.GetMethod("NewGame", Static, null,
                new[] { typeof(InitialWorldConfiguration) }, null), null, config);
        }

        private static object Invoke(MethodInfo method, object target, params object[] args)
        {
            try { return method.Invoke(target, args); }
            catch (TargetInvocationException ex) when (ex.InnerException != null) { throw ex.InnerException; }
        }

        private static void ResetRuntime()
        {
            BridgeType.GetMethod("ResetForTests", Static).Invoke(null, null);
            FieldInfo failure = SaveSystemType.GetField("ForceSaveFailureForTests", Static);
            if (failure != null) failure.SetValue(null, null);
        }

        private static List<PersonDefinition> Roster()
        {
            object dm = DataManagerType.GetProperty("Instance", Static).GetValue(null, null);
            var roster = (IEnumerable<PersonDefinition>)DataManagerType.GetProperty("InitialPersonRoster", Instance)
                .GetValue(dm, null);
            return roster.ToList();
        }

        private static System.Collections.IList Characters(object collection) =>
            (System.Collections.IList)PlayerCollectionType.GetField("OwnedCharacters", Instance).GetValue(collection);

        private static List<object> BuildingObjects()
        {
            object runtime = BridgeType.GetProperty("Buildings", Static).GetValue(null);
            var all = (System.Collections.IEnumerable)runtime.GetType().GetProperty("All", Instance).GetValue(runtime, null);
            return all.Cast<object>().ToList();
        }
    }
}
