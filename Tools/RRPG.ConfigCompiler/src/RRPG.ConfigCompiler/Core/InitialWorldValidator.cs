using System;
using System.Collections.Generic;
using System.Linq;

namespace RRPG.ConfigCompiler.Core
{
    /// <summary>
    /// Initial World 领域（正式 NewGame 初始世界配置）的领域级校验。
    ///
    /// 输入 = 同一份 Compiler 语料中的三张表：
    ///   - <c>InitialWorld</c>        ：默认 profile 身份 + 初始英雄 / 金币 / Ingredients / Food；
    ///   - <c>InitialPopulation</c>   ：稳定序号 + PersonDefinitionId；
    ///   - <c>InitialBuilding</c>     ：稳定序号 + BuildingDefinitionId + 世界坐标。
    ///
    /// 边界：
    ///   - 类型 / 范围（Row 3 的 &gt;= 规则）/ id 唯一 / <c>ref:BuildingDefinition</c> 由
    ///     通用 DataCompiler 与 CrossReferenceValidator 负责；本类不重复实现，也不削弱它们；
    ///   - PersonDefinition 不在同一 Compiler 语料里，因此这里**不能**伪造引用表；
    ///     <c>PersonDefinitionId</c> 只校验形状（非空 / 不重复），真实存在性由 Runtime
    ///     `InitialWorldConfigurationCatalog.TryValidate` 跨域校验；
    ///   - 坐标范围 / 占地重叠属于 Village 的 Placement Authority，不在这里另造第二套规则；
    ///   - 三张表**要么都有、要么都没有**：只写一半是 Hard Error，不允许静默当作「空建筑」。
    /// </summary>
    public static class InitialWorldValidator
    {
        public const string WorldSheet = "InitialWorld";
        public const string PopulationSheet = "InitialPopulation";
        public const string BuildingSheet = "InitialBuilding";

        public static void Validate(List<CompiledSheet> sheets, DiagnosticBag bag)
        {
            if (sheets == null) return;

            CompiledSheet world = Find(sheets, WorldSheet);
            CompiledSheet population = Find(sheets, PopulationSheet);
            CompiledSheet building = Find(sheets, BuildingSheet);

            bool anyPresent = world != null || population != null || building != null;
            if (!anyPresent) return; // 其它领域语料不受影响（该领域不存在 = 合法空态）

            if (world == null || population == null || building == null)
            {
                bag.Error(DiagnosticCodes.DatInitialWorldDomainShape,
                    "Initial World 领域必须同时提供 InitialWorld / InitialPopulation / InitialBuilding 三张表；"
                    + "缺失：" + string.Join(", ", new[]
                    {
                        world == null ? WorldSheet : null,
                        population == null ? PopulationSheet : null,
                        building == null ? BuildingSheet : null
                    }.Where(x => x != null))
                    + "。不允许只写一部分（缺少建筑表 = 正式 NewGame 无初始建筑，不是合法默认）。",
                    DiagContext.Of((world ?? population ?? building).Spec.Workbook,
                        (world ?? population ?? building).Spec.SheetName, 0));
                return;
            }

            ValidateWorld(world, bag);
            ValidatePopulation(population, bag);
            ValidateBuildings(building, bag);
        }

        private static void ValidateWorld(CompiledSheet sheet, DiagnosticBag bag)
        {
            var ctx = DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, 0);

            if (!HasFields(sheet, bag,
                    "ProfileId", "IsDefault", "InitialHeroId", "Gold", "InitialIngredients", "InitialFood"))
                return;

            if (sheet.Rows.Count == 0)
            {
                bag.Error(DiagnosticCodes.DatInitialWorldDefaultProfile,
                    "InitialWorld 至少需要一行默认 profile（IsDefault = TRUE）；空表不能作为正式新游戏配置。",
                    ctx);
                return;
            }

            int defaultProfiles = 0;
            var profileIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (CompiledRow row in sheet.Rows)
            {
                var rowCtx = DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, row.ExcelRow);

                string profileId = Text(row, "ProfileId");
                if (string.IsNullOrEmpty(profileId))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldDefaultProfile,
                        "InitialWorld.ProfileId 不能为空。", rowCtx, "ProfileId");
                }
                else if (!profileIds.Add(profileId))
                {
                    bag.Error(DiagnosticCodes.DatKeyDuplicate,
                        "InitialWorld.ProfileId 重复：" + profileId + "。", rowCtx, "ProfileId");
                }

                if (Bool(row, "IsDefault")) defaultProfiles++;

                string heroId = Text(row, "InitialHeroId");
                if (string.IsNullOrEmpty(heroId))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldDefaultProfile,
                        "InitialWorld.InitialHeroId 不能为空（正式 NewGame 必须能解析默认英雄）。",
                        rowCtx, "InitialHeroId");
                }

                RequireNonNegative(row, "Gold", sheet, bag);
                RequireNonNegative(row, "InitialIngredients", sheet, bag);
                RequireNonNegative(row, "InitialFood", sheet, bag);
            }

            if (defaultProfiles != 1)
            {
                bag.Error(DiagnosticCodes.DatInitialWorldDefaultProfile,
                    "InitialWorld 必须恰好有 1 行 IsDefault = TRUE（当前 " + defaultProfiles
                    + " 行）。多默认 profile 会让「默认产品入口读哪一份」变成隐式歧义。",
                    ctx, "IsDefault");
            }
        }

        private static void ValidatePopulation(CompiledSheet sheet, DiagnosticBag bag)
        {
            var ctx = DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, 0);

            if (!HasFields(sheet, bag, "StableIndex", "PersonDefinitionId")) return;

            if (sheet.Rows.Count == 0)
            {
                bag.Error(DiagnosticCodes.DatInitialWorldPopulationShape,
                    "InitialPopulation 至少需要 1 行；空人口不是合法的正式新游戏默认配置。",
                    ctx);
                return;
            }

            var definitions = new HashSet<string>(StringComparer.Ordinal);

            foreach (CompiledRow row in sheet.Rows)
            {
                var rowCtx = DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, row.ExcelRow);

                long? stable = Int(row, "StableIndex");
                if (!stable.HasValue || stable.Value < 0)
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldPopulationShape,
                        "InitialPopulation.StableIndex 必须是非负整数稳定序号。", rowCtx, "StableIndex");
                }

                string definitionId = Text(row, "PersonDefinitionId");
                if (string.IsNullOrEmpty(definitionId))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldPopulationShape,
                        "InitialPopulation.PersonDefinitionId 不能为空。", rowCtx, "PersonDefinitionId");
                }
                else if (!definitions.Add(definitionId))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldPopulationShape,
                        "InitialPopulation 出现重复 PersonDefinitionId：" + definitionId
                        + "（同一人物定义不能在一次 NewGame 中创建两次）。",
                        rowCtx, "PersonDefinitionId");
                }
            }
        }

        private static void ValidateBuildings(CompiledSheet sheet, DiagnosticBag bag)
        {
            var ctx = DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, 0);

            if (!HasFields(sheet, bag, "StableIndex", "BuildingDefinitionId", "WorldX", "WorldY")) return;

            if (sheet.Rows.Count == 0)
            {
                bag.Error(DiagnosticCodes.DatInitialWorldBuildingShape,
                    "InitialBuilding 至少需要 1 行；缺少建筑表内容会让正式 NewGame 静默变成 0 建筑，"
                    + "这是被明确拒绝的 fallback。",
                    ctx);
                return;
            }

            foreach (CompiledRow row in sheet.Rows)
            {
                var rowCtx = DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, row.ExcelRow);

                long? stable = Int(row, "StableIndex");
                if (!stable.HasValue || stable.Value < 0)
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldBuildingShape,
                        "InitialBuilding.StableIndex 必须是非负整数稳定序号。", rowCtx, "StableIndex");
                }

                string definitionId = Text(row, "BuildingDefinitionId");
                if (string.IsNullOrEmpty(definitionId))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldBuildingShape,
                        "InitialBuilding.BuildingDefinitionId 不能为空。", rowCtx, "BuildingDefinitionId");
                }

                double? x = Float(row, "WorldX");
                double? y = Float(row, "WorldY");
                if (!x.HasValue || !IsFinite(x.Value))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldBuildingShape,
                        "InitialBuilding.WorldX 必须是有限数值。", rowCtx, "WorldX");
                }
                if (!y.HasValue || !IsFinite(y.Value))
                {
                    bag.Error(DiagnosticCodes.DatInitialWorldBuildingShape,
                        "InitialBuilding.WorldY 必须是有限数值。", rowCtx, "WorldY");
                }
            }
        }

        private static void RequireNonNegative(CompiledRow row, string field, CompiledSheet sheet, DiagnosticBag bag)
        {
            long? value = Int(row, field);
            if (value.HasValue && value.Value < 0)
            {
                bag.Error(DiagnosticCodes.DatInitialWorldValueRange,
                    "InitialWorld." + field + " 不能为负数：" + value.Value,
                    DiagContext.Of(sheet.Spec.Workbook, sheet.Spec.SheetName, row.ExcelRow), field);
            }
        }

        private static bool HasFields(CompiledSheet sheet, DiagnosticBag bag, params string[] fields)
        {
            bool ok = true;
            foreach (string field in fields)
            {
                if (sheet.Spec.Fields.Any(f => string.Equals(f.Field, field, StringComparison.Ordinal))) continue;
                ok = false; // SchemaCompiler 已报 SCH027；这里只避免后续 NRE / 重复噪声
            }
            return ok;
        }

        private static CompiledSheet Find(List<CompiledSheet> sheets, string name)
        {
            return sheets.FirstOrDefault(s => string.Equals(s.Spec.SheetName, name, StringComparison.Ordinal));
        }

        private static string Text(CompiledRow row, string field)
        {
            return row.Get(field)?.Value as string;
        }

        private static long? Int(CompiledRow row, string field)
        {
            object value = row.Get(field)?.Value;
            if (value is long l) return l;
            if (value is double d && d == Math.Floor(d) && IsFinite(d)) return (long)d;
            return null;
        }

        private static double? Float(CompiledRow row, string field)
        {
            object value = row.Get(field)?.Value;
            if (value is double d) return d;
            if (value is long l) return l;
            return null;
        }

        private static bool Bool(CompiledRow row, string field)
        {
            object value = row.Get(field)?.Value;
            return value is bool b && b;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
