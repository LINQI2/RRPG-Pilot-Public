using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using RRPG.StaticConfig;

namespace RRPG.StaticConfig.EditorTools
{
    /// <summary>
    /// Initial World 领域 Mirror 读取（Editor Builder 输入；只读 Canonical Mirror）。
    ///
    /// 三件套：InitialWorld.json / InitialPopulation.json / InitialBuilding.json。
    /// 语义：
    ///   - 三个都不存在 = 本领域尚未生成（合法空态；Builder 会清空生成容器，Registry 明确报「未生成」）；
    ///   - 只存在一部分 = 半损坏 Mirror → Hard Error（绝不静默当成 0 建筑）；
    ///   - 内容结构非法（空表 / 多默认 profile / 重复序号 / 重复人物 / 非有限坐标）→ Hard Error。
    /// </summary>
    internal static class InitialWorldMirrorReader
    {
        internal static readonly string[] FileNames =
        {
            "InitialWorld.json", "InitialPopulation.json", "InitialBuilding.json"
        };

        private static readonly string[] WorldFields =
        {
            "ProfileId", "IsDefault", "InitialHeroId", "Gold", "InitialIngredients", "InitialFood"
        };

        private static readonly string[] PopulationFields = { "StableIndex", "PersonDefinitionId" };
        private static readonly string[] BuildingFields = { "StableIndex", "BuildingDefinitionId", "WorldX", "WorldY" };

        internal sealed class Payload
        {
            internal bool Present;
            internal string WorldSha256 = string.Empty;
            internal string PopulationSha256 = string.Empty;
            internal string BuildingSha256 = string.Empty;
            internal List<GeneratedInitialWorldProfile> Profiles = new List<GeneratedInitialWorldProfile>();
            internal List<GeneratedInitialPopulationEntry> Population = new List<GeneratedInitialPopulationEntry>();
            internal List<GeneratedInitialBuildingEntry> Buildings = new List<GeneratedInitialBuildingEntry>();
            internal int Count => Profiles.Count + Population.Count + Buildings.Count;
        }

        internal static Payload Read(string mirrorDir)
        {
            var payload = new Payload();

            int present = 0;
            foreach (string name in FileNames)
            {
                if (File.Exists(Path.Combine(mirrorDir, name))) present++;
            }

            if (present == 0) return payload; // 领域尚未生成
            if (present != FileNames.Length)
            {
                throw new InvalidDataException(
                    "Initial World Mirror 不完整：InitialWorld / InitialPopulation / InitialBuilding 必须同时存在（当前 "
                    + present + "/" + FileNames.Length + "）。请重新运行 Compiler build。");
            }

            payload.Present = true;

            foreach (Dictionary<string, string> row in ReadRows(mirrorDir, "InitialWorld", WorldFields,
                         out payload.WorldSha256))
            {
                payload.Profiles.Add(new GeneratedInitialWorldProfile
                {
                    ProfileId = MirrorJsonText.Unquote(row, "ProfileId"),
                    IsDefault = ReadBool(row, "IsDefault", "InitialWorld"),
                    InitialHeroId = MirrorJsonText.Unquote(row, "InitialHeroId"),
                    Gold = ReadInt(row, "Gold", "InitialWorld"),
                    InitialIngredients = ReadInt(row, "InitialIngredients", "InitialWorld"),
                    InitialFood = ReadInt(row, "InitialFood", "InitialWorld")
                });
            }

            foreach (Dictionary<string, string> row in ReadRows(mirrorDir, "InitialPopulation", PopulationFields,
                         out payload.PopulationSha256))
            {
                payload.Population.Add(new GeneratedInitialPopulationEntry
                {
                    StableIndex = ReadInt(row, "StableIndex", "InitialPopulation"),
                    PersonDefinitionId = MirrorJsonText.Unquote(row, "PersonDefinitionId")
                });
            }

            foreach (Dictionary<string, string> row in ReadRows(mirrorDir, "InitialBuilding", BuildingFields,
                         out payload.BuildingSha256))
            {
                payload.Buildings.Add(new GeneratedInitialBuildingEntry
                {
                    StableIndex = ReadInt(row, "StableIndex", "InitialBuilding"),
                    BuildingDefinitionId = MirrorJsonText.Unquote(row, "BuildingDefinitionId"),
                    WorldX = ReadFloat(row, "WorldX", "InitialBuilding"),
                    WorldY = ReadFloat(row, "WorldY", "InitialBuilding")
                });
            }

            if (!InitialWorldGeneratedConfigValidator.TryValidate(
                    payload.Profiles, payload.Population, payload.Buildings, out string diagnostic))
            {
                throw new InvalidDataException("Initial World Mirror 内容非法：" + diagnostic);
            }

            return payload;
        }

        private static List<Dictionary<string, string>> ReadRows(string dir, string sheetName, string[] fields,
            out string sourceSha256)
        {
            string path = Path.Combine(dir, sheetName + ".json");
            string json = File.ReadAllText(path);

            if (!MirrorJsonText.TryReadHeader(json, sheetName + ".json", out _, out sourceSha256, out string failure)
                || !MirrorJsonText.TryReadSheet(json, sheetName + ".json", sheetName, out failure)
                || !MirrorJsonText.TryReadColumns(json, sheetName + ".json", fields, out failure)
                || !MirrorJsonText.TryReadRows(json, sheetName + ".json", "rowCount", "rows", out _, out var rows,
                    out failure))
            {
                throw new InvalidDataException(failure);
            }

            return rows;
        }

        private static int ReadInt(IReadOnlyDictionary<string, string> row, string field, string sheet)
        {
            if (MirrorJsonText.TryInt(row, field, out int value)) return value;
            throw new InvalidDataException("Initial World Mirror：" + sheet + "." + field + " 不是整数。");
        }

        private static bool ReadBool(IReadOnlyDictionary<string, string> row, string field, string sheet)
        {
            if (MirrorJsonText.TryBool(row, field, out bool value)) return value;
            throw new InvalidDataException("Initial World Mirror：" + sheet + "." + field + " 不是 bool。");
        }

        private static float ReadFloat(IReadOnlyDictionary<string, string> row, string field, string sheet)
        {
            string raw = MirrorJsonText.Unquote(row, field);
            if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float value)
                && !float.IsNaN(value) && !float.IsInfinity(value))
            {
                return value;
            }

            throw new InvalidDataException("Initial World Mirror：" + sheet + "." + field + " 不是有限数值。");
        }
    }
}
