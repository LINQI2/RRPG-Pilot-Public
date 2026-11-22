using System.IO;
using System.Linq;
using RRPG.ConfigCompiler.Core;
using Xunit;

namespace RRPG.ConfigCompiler.Tests
{
    /// <summary>
    /// Initial World 领域（正式 NewGame 初始世界配置）Compiler 契约测试。
    ///
    /// 验证的是**通过公开编译管线的可观察行为**，不是把校验实现再抄一遍：
    ///   - 合法三表 → 三份 Canonical Mirror；重复 build 字节一致；validate 通过；
    ///   - 三表不全 / 多默认 profile / 负数 / 重复人物 / 负序号 → Hard Error 且 Mirror 0 更新；
    ///   - 未知 BuildingDefinition 引用走既有 DAT008；
    ///   - 没有本领域 Sheet 时其它领域不受影响；
    ///   - PersonDefinitionId 只做形状校验（Compiler 不伪造跨域引用表）。
    /// </summary>
    public class InitialWorldConfigTests
    {
        private static TestSheet BuildingDefinitions()
        {
            return new TestSheet("BuildingDefinition")
                .Col("BuildingDefinitionId", "id", "", "建筑定义ID")
                .Col("DisplayName", "string", "", "中文名")
                .Col("Category", "string", "", "类别")
                .Col("FootprintWidth", "int", ">=1", "宽")
                .Col("FootprintDepth", "int", ">=1", "深")
                .Row("building_farm", "农田", "Production", 3, 2)
                .Row("building_kitchen", "厨房", "Production", 2, 2)
                .Row("building_house", "房屋", "Housing", 2, 2);
        }

        private static TestSheet WorldSheet(
            string profileId = "iw_default_test",
            bool isDefault = true,
            string heroId = "hero_001",
            int gold = 100,
            int ingredients = 0,
            int food = 0,
            bool includeRangeRules = true)
        {
            string rule = includeRangeRules ? ">=0" : "";
            return new TestSheet("InitialWorld")
                .Col("ProfileId", "id", "", "配置档案ID")
                .Col("IsDefault", "bool", "", "是否默认档案")
                .Col("InitialHeroId", "string", "", "初始英雄ID")
                .Col("Gold", "int", rule, "初始金币")
                .Col("InitialIngredients", "int", rule, "初始食材")
                .Col("InitialFood", "int", rule, "初始食物")
                .Row(profileId, isDefault, heroId, gold, ingredients, food);
        }

        private static TestSheet PopulationSheet(params object[][] rows)
        {
            var sheet = new TestSheet("InitialPopulation")
                .Col("StableIndex", "int", "key;>=0", "稳定序号")
                .Col("PersonDefinitionId", "string", "", "人物定义ID");
            foreach (object[] row in rows) sheet.Row(row);
            return sheet;
        }

        private static TestSheet BuildingSheet(params object[][] rows)
        {
            var sheet = new TestSheet("InitialBuilding")
                .Col("StableIndex", "int", "key;>=0", "稳定序号")
                .Col("BuildingDefinitionId", "ref:BuildingDefinition", "", "建筑定义ID")
                .Col("WorldX", "float", "", "世界X")
                .Col("WorldY", "float", "", "世界Y");
            foreach (object[] row in rows) sheet.Row(row);
            return sheet;
        }

        private static object[][] DefaultPopulationRows()
        {
            return new[]
            {
                new object[] { 1, "person_lin_en" },
                new object[] { 2, "person_su_wan" },
                new object[] { 3, "person_han_zhao" },
                new object[] { 4, "person_qiu_shuang" },
                new object[] { 5, "person_wei_an" }
            };
        }

        private static object[][] DefaultBuildingRows()
        {
            return new[]
            {
                new object[] { 1, "building_farm", -6.0, -1.2 },
                new object[] { 2, "building_kitchen", -2.0, -1.2 },
                new object[] { 3, "building_house", -8.5, 4.2 },
                new object[] { 4, "building_house", -5.1, 4.2 },
                new object[] { 5, "building_house", -1.7, 4.2 },
                new object[] { 6, "building_house", 1.7, 4.2 },
                new object[] { 7, "building_house", 5.1, 4.2 }
            };
        }

        private static TestSheet[] ValidDomain()
        {
            return new[]
            {
                BuildingDefinitions(),
                WorldSheet(),
                PopulationSheet(DefaultPopulationRows()),
                BuildingSheet(DefaultBuildingRows())
            };
        }

        private static string[] InitialWorldFiles()
        {
            return new[] { "InitialWorld.json", "InitialPopulation.json", "InitialBuilding.json" };
        }

        [Fact]
        public void ValidDomainPublishesThreeDeterministicMirrors()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", ValidDomain());

            CompileResult first = CompilerRunner.Run(root.Paths, "build", true);
            Assert.True(first.Success, string.Join("\n", first.Diagnostics));
            foreach (string file in InitialWorldFiles())
                Assert.True(File.Exists(root.GeneratedFile(file)), "缺少 Mirror：" + file);

            string worldJson = root.ReadGenerated("InitialWorld.json");
            string populationJson = root.ReadGenerated("InitialPopulation.json");
            string buildingJson = root.ReadGenerated("InitialBuilding.json");

            Assert.Contains("\"ProfileId\": \"iw_default_test\"", worldJson);
            Assert.Contains("\"IsDefault\": true", worldJson);
            Assert.Contains("\"InitialHeroId\": \"hero_001\"", worldJson);
            Assert.Contains("\"Gold\": 100", worldJson);

            Assert.Contains("\"PersonDefinitionId\": \"person_lin_en\"", populationJson);
            Assert.Equal(5, first.CompiledSheets.Single(s => s.Spec.SheetName == "InitialPopulation").Rows.Count);

            Assert.Contains("\"BuildingDefinitionId\": \"building_house\"", buildingJson);
            Assert.Equal(1, buildingJson.Split("\"BuildingDefinitionId\": \"building_farm\"").Length - 1);
            Assert.Equal(5, buildingJson.Split("\"BuildingDefinitionId\": \"building_house\"").Length - 1);
            Assert.DoesNotContain("building_workshop", buildingJson);

            byte[] before = File.ReadAllBytes(root.GeneratedFile("InitialWorld.json"));
            CompileResult second = CompilerRunner.Run(root.Paths, "build", true);
            Assert.True(second.Success, string.Join("\n", second.Diagnostics));
            Assert.Equal(before, File.ReadAllBytes(root.GeneratedFile("InitialWorld.json")));
            Assert.True(CompilerRunner.Run(root.Paths, "validate", false).Success, "validate 必须证明 Authoring == Mirror");
        }

        [Theory]
        [InlineData("World")]
        [InlineData("Population")]
        [InlineData("Building")]
        public void PartialDomainIsHardErrorAndPublishesNothing(string missing)
        {
            using var root = new TestRoot();
            var sheets = ValidDomain().Where(s => s.Name != "Initial" + missing).ToArray();
            // World 的表名是 InitialWorld，Population / Building 则是 InitialPopulation / InitialBuilding。
            if (missing == "World") sheets = ValidDomain().Where(s => s.Name != "InitialWorld").ToArray();

            root.WriteWorkbook("RRPG_InitialWorld.xlsx", sheets);
            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);

            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatInitialWorldDomainShape);
            foreach (string file in InitialWorldFiles())
                Assert.False(File.Exists(root.GeneratedFile(file)), "Hard Error 时不得写出：" + file);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void DefaultProfileMustBeExactlyOne(bool first, bool second)
        {
            using var root = new TestRoot();
            TestSheet world = WorldSheet("iw_a", first)
                .Row("iw_b", second, "hero_001", 100, 0, 0);
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", new[]
            {
                BuildingDefinitions(), world, PopulationSheet(DefaultPopulationRows()),
                BuildingSheet(DefaultBuildingRows())
            });

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatInitialWorldDefaultProfile);
            Assert.False(File.Exists(root.GeneratedFile("InitialWorld.json")));
        }

        [Fact]
        public void NegativeResourcesAreRejectedWithoutRelyingOnSheetRules()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", new[]
            {
                BuildingDefinitions(),
                WorldSheet(gold: -1, ingredients: -2, food: -3, includeRangeRules: false),
                PopulationSheet(DefaultPopulationRows()),
                BuildingSheet(DefaultBuildingRows())
            });

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatInitialWorldValueRange);
            Assert.False(File.Exists(root.GeneratedFile("InitialWorld.json")));
        }

        [Fact]
        public void DuplicatePersonDefinitionIsRejected()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", new[]
            {
                BuildingDefinitions(), WorldSheet(),
                PopulationSheet(
                    new object[] { 1, "person_lin_en" },
                    new object[] { 2, "person_lin_en" }),
                BuildingSheet(DefaultBuildingRows())
            });

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatInitialWorldPopulationShape);
            Assert.False(File.Exists(root.GeneratedFile("InitialPopulation.json")));
        }

        [Fact]
        public void NegativeStableIndexesAreRejected()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", new[]
            {
                BuildingDefinitions(), WorldSheet(),
                PopulationSheet(new object[] { 1, "person_lin_en" }),
                // 故意去掉 Row 3 的 >=0：验证领域校验自身不依赖表格声明。
                new TestSheet("InitialBuilding")
                    .Col("StableIndex", "int", "key", "稳定序号")
                    .Col("BuildingDefinitionId", "ref:BuildingDefinition", "", "建筑定义ID")
                    .Col("WorldX", "float", "", "世界X")
                    .Col("WorldY", "float", "", "世界Y")
                    .Row(-1, "building_farm", 0.0, 0.0)
            });

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatInitialWorldBuildingShape);
            Assert.False(File.Exists(root.GeneratedFile("InitialBuilding.json")));
        }

        [Fact]
        public void EmptyPopulationOrBuildingSheetIsRejected()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", new[]
            {
                BuildingDefinitions(), WorldSheet(),
                PopulationSheet(),
                BuildingSheet()
            });

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatInitialWorldDomainShape);
            Assert.False(File.Exists(root.GeneratedFile("InitialPopulation.json")));
            Assert.False(File.Exists(root.GeneratedFile("InitialBuilding.json")));
        }

        [Fact]
        public void UnknownBuildingReferenceIsRejectedAndNothingIsPublished()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_InitialWorld.xlsx", new[]
            {
                BuildingDefinitions(), WorldSheet(),
                PopulationSheet(DefaultPopulationRows()),
                BuildingSheet(new object[] { 1, "building_missing", 0.0, 0.0 })
            });

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.False(result.Success);
            Assert.Contains(result.Diagnostics, d => d.Code == DiagnosticCodes.DatRefUnresolved);
            Assert.False(File.Exists(root.GeneratedFile("InitialBuilding.json")));
        }

        [Fact]
        public void AbsentDomainDoesNotAffectOtherWorkbooks()
        {
            using var root = new TestRoot();
            root.WriteWorkbook("RRPG_Buildings.xlsx", BuildingDefinitions());

            CompileResult result = CompilerRunner.Run(root.Paths, "build", true);
            Assert.True(result.Success, string.Join("\n", result.Diagnostics));
            Assert.True(File.Exists(root.GeneratedFile("BuildingDefinition.json")));
            foreach (string file in InitialWorldFiles())
                Assert.False(File.Exists(root.GeneratedFile(file)));
        }
    }
}
