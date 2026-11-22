using System;
using System.Collections.Generic;
using UnityEngine;

namespace RRPG.StaticConfig
{
    /// <summary>生成的单条默认 profile（只读数据容器；不是第二 Authoring Authority）。</summary>
    [Serializable]
    public sealed class GeneratedInitialWorldProfile
    {
        public string ProfileId;
        public bool IsDefault;
        public string InitialHeroId;
        public int Gold;
        public int InitialIngredients;
        public int InitialFood;
    }

    /// <summary>生成的单条初始人口（StableIndex = 稳定创建顺序；PersonDefinitionId 只是定义引用）。</summary>
    [Serializable]
    public sealed class GeneratedInitialPopulationEntry
    {
        public int StableIndex;
        public string PersonDefinitionId;
    }

    /// <summary>生成的单条初始建筑（StableIndex = 实例分配顺序；坐标是 Village 本地坐标）。</summary>
    [Serializable]
    public sealed class GeneratedInitialBuildingEntry
    {
        public int StableIndex;
        public string BuildingDefinitionId;
        public float WorldX;
        public float WorldY;
    }

    /// <summary>
    /// InitialWorldConfigDatabase —— 正式初始世界配置的**生成数据容器**（Runtime；只读消费）。
    ///
    /// 数据链：
    ///   `RRPG_InitialWorld.xlsx → Compiler → Development/Generated/StaticConfig/Initial*.json
    ///    → Unity Builder → 本 ScriptableObject（挂在唯一 RRPGStaticConfigRoot）→ StaticConfigRegistry`
    ///   最后才由 `InitialWorldConfigurationCatalog` 映射成纯 Runtime 模型 `InitialWorldConfiguration`。
    ///
    /// 边界：
    ///   - 本类**不是**第二 Authoring Authority：`ApplyGeneratedContent` 只允许 Builder 调用，
    ///     内容必须来自 Compiler 生成的 Canonical Mirror；
    ///   - Runtime 只通过 `CopyProfiles / CopyPopulation / CopyBuildings` 取得**深拷贝**，
    ///     外部修改返回值不会污染生成数据，也无法产生全局可变 setter；
    ///   - 空内容 = 明确的「未生成 / 已失效」状态，由 Registry / Catalog 报错，
    ///     **不会**被当成「0 建筑」静默接受。
    /// </summary>
    public sealed class InitialWorldConfigDatabase : ScriptableObject
    {
        [SerializeField] private string formatVersion = string.Empty;
        [SerializeField] private string initialWorldSha256 = string.Empty;
        [SerializeField] private string populationSha256 = string.Empty;
        [SerializeField] private string buildingSha256 = string.Empty;

        [SerializeField] private List<GeneratedInitialWorldProfile> profiles = new List<GeneratedInitialWorldProfile>();
        [SerializeField] private List<GeneratedInitialPopulationEntry> population = new List<GeneratedInitialPopulationEntry>();
        [SerializeField] private List<GeneratedInitialBuildingEntry> buildings = new List<GeneratedInitialBuildingEntry>();

        public string FormatVersion => formatVersion;
        public string InitialWorldSha256 => initialWorldSha256;
        public string PopulationSha256 => populationSha256;
        public string BuildingSha256 => buildingSha256;

        public int ProfileCount => profiles == null ? 0 : profiles.Count;
        public int PopulationCount => population == null ? 0 : population.Count;
        public int BuildingCount => buildings == null ? 0 : buildings.Count;
        public int TotalCount => ProfileCount + PopulationCount + BuildingCount;

        /// <summary>
        /// Builder 唯一写入口。非法内容（空 / 多默认 profile / 重复序号 / 重复人物定义 /
        /// 非有限坐标）→ 抛出明确异常，不发布半套数据。
        /// </summary>
        public void ApplyGeneratedContent(string format,
            string worldSha256, string populationMirrorSha256, string buildingMirrorSha256,
            List<GeneratedInitialWorldProfile> generatedProfiles,
            List<GeneratedInitialPopulationEntry> generatedPopulation,
            List<GeneratedInitialBuildingEntry> generatedBuildings)
        {
            string diagnostic;
            if (!InitialWorldGeneratedConfigValidator.TryValidate(
                    generatedProfiles, generatedPopulation, generatedBuildings, out diagnostic))
            {
                throw new ArgumentException("初始世界生成数据非法：" + diagnostic, nameof(generatedProfiles));
            }

            formatVersion = format ?? string.Empty;
            initialWorldSha256 = worldSha256 ?? string.Empty;
            populationSha256 = populationMirrorSha256 ?? string.Empty;
            buildingSha256 = buildingMirrorSha256 ?? string.Empty;
            profiles = CopyProfiles(generatedProfiles);
            population = CopyPopulation(generatedPopulation);
            buildings = CopyBuildings(generatedBuildings);
        }

        /// <summary>
        /// 清空生成内容（只用于 Mirror 中本领域**整体不存在**的合法空态）。
        /// Registry / Catalog 会把它报成「未生成」，绝不当作 0 建筑默认值。
        /// </summary>
        public void ClearGeneratedContent(string format)
        {
            formatVersion = format ?? string.Empty;
            initialWorldSha256 = string.Empty;
            populationSha256 = string.Empty;
            buildingSha256 = string.Empty;
            profiles = new List<GeneratedInitialWorldProfile>();
            population = new List<GeneratedInitialPopulationEntry>();
            buildings = new List<GeneratedInitialBuildingEntry>();
        }

        /// <summary>默认 profile 的**深拷贝**；必须恰好一条 IsDefault = true。</summary>
        public bool TryCopyDefaultProfile(out GeneratedInitialWorldProfile profile, out string diagnostic)
        {
            profile = null;
            diagnostic = string.Empty;
            GeneratedInitialWorldProfile found = null;
            int defaults = 0;

            if (profiles != null)
            {
                foreach (GeneratedInitialWorldProfile candidate in profiles)
                {
                    if (candidate == null || !candidate.IsDefault) continue;
                    defaults++;
                    found = candidate;
                }
            }

            if (defaults != 1)
            {
                diagnostic = "初始世界配置必须恰好有 1 条默认 profile（当前 " + defaults + " 条）。";
                return false;
            }

            profile = CopyProfile(found);
            return true;
        }

        /// <summary>人口深拷贝（按 StableIndex 升序；保留真实创建顺序）。</summary>
        public List<GeneratedInitialPopulationEntry> CopyPopulation()
        {
            return CopyPopulation(population);
        }

        /// <summary>建筑深拷贝（按 StableIndex 升序；保留实例分配顺序）。</summary>
        public List<GeneratedInitialBuildingEntry> CopyBuildings()
        {
            return CopyBuildings(buildings);
        }

        /// <summary>全部 profile 深拷贝（含非默认档案；供诊断 / 测试）。</summary>
        public List<GeneratedInitialWorldProfile> CopyProfiles()
        {
            return CopyProfiles(profiles);
        }

        private static List<GeneratedInitialWorldProfile> CopyProfiles(List<GeneratedInitialWorldProfile> source)
        {
            var copy = new List<GeneratedInitialWorldProfile>();
            if (source == null) return copy;
            foreach (GeneratedInitialWorldProfile row in source) copy.Add(CopyProfile(row));
            copy.Sort((a, b) => string.CompareOrdinal(a?.ProfileId, b?.ProfileId));
            return copy;
        }

        private static GeneratedInitialWorldProfile CopyProfile(GeneratedInitialWorldProfile source)
        {
            if (source == null) return null;
            return new GeneratedInitialWorldProfile
            {
                ProfileId = source.ProfileId,
                IsDefault = source.IsDefault,
                InitialHeroId = source.InitialHeroId,
                Gold = source.Gold,
                InitialIngredients = source.InitialIngredients,
                InitialFood = source.InitialFood
            };
        }

        private static List<GeneratedInitialPopulationEntry> CopyPopulation(List<GeneratedInitialPopulationEntry> source)
        {
            var copy = new List<GeneratedInitialPopulationEntry>();
            if (source == null) return copy;
            foreach (GeneratedInitialPopulationEntry row in source)
            {
                if (row == null) continue;
                copy.Add(new GeneratedInitialPopulationEntry
                {
                    StableIndex = row.StableIndex,
                    PersonDefinitionId = row.PersonDefinitionId
                });
            }
            copy.Sort((a, b) => a.StableIndex.CompareTo(b.StableIndex));
            return copy;
        }

        private static List<GeneratedInitialBuildingEntry> CopyBuildings(List<GeneratedInitialBuildingEntry> source)
        {
            var copy = new List<GeneratedInitialBuildingEntry>();
            if (source == null) return copy;
            foreach (GeneratedInitialBuildingEntry row in source)
            {
                if (row == null) continue;
                copy.Add(new GeneratedInitialBuildingEntry
                {
                    StableIndex = row.StableIndex,
                    BuildingDefinitionId = row.BuildingDefinitionId,
                    WorldX = row.WorldX,
                    WorldY = row.WorldY
                });
            }
            copy.Sort((a, b) => a.StableIndex.CompareTo(b.StableIndex));
            return copy;
        }
    }

    /// <summary>
    /// 生成数据的**结构校验**（Runtime 第二道防线；Compiler 已在校验一次，这里防手改 / 半损坏生成资产）。
    /// 引用真实存在性与占地仍由 `InitialWorldConfigurationCatalog.TryValidate` 走既有 Authority。
    /// </summary>
    public static class InitialWorldGeneratedConfigValidator
    {
        public static bool TryValidate(
            IList<GeneratedInitialWorldProfile> profiles,
            IList<GeneratedInitialPopulationEntry> population,
            IList<GeneratedInitialBuildingEntry> buildings,
            out string diagnostic)
        {
            diagnostic = string.Empty;

            if (profiles == null || profiles.Count == 0)
            {
                diagnostic = "初始世界配置为空：InitialWorld 未生成任何 profile。";
                return false;
            }

            if (population == null || population.Count == 0)
            {
                diagnostic = "初始世界配置缺少初始人口：InitialPopulation 为空。";
                return false;
            }

            if (buildings == null || buildings.Count == 0)
            {
                diagnostic = "初始世界配置缺少初始建筑：InitialBuilding 为空（不允许静默 fallback 成 0 建筑）。";
                return false;
            }

            var profileIds = new HashSet<string>(StringComparer.Ordinal);
            int defaults = 0;

            for (int i = 0; i < profiles.Count; i++)
            {
                GeneratedInitialWorldProfile profile = profiles[i];
                if (profile == null)
                {
                    diagnostic = "初始世界配置第 " + i + " 条 profile 为 null。";
                    return false;
                }
                if (string.IsNullOrEmpty(profile.ProfileId))
                {
                    diagnostic = "初始世界配置第 " + i + " 条 profile 缺少 ProfileId。";
                    return false;
                }
                if (!profileIds.Add(profile.ProfileId))
                {
                    diagnostic = "初始世界配置出现重复 ProfileId：" + profile.ProfileId;
                    return false;
                }
                if (profile.IsDefault) defaults++;
                if (string.IsNullOrEmpty(profile.InitialHeroId))
                {
                    diagnostic = "初始世界配置 profile 缺少 InitialHeroId：" + profile.ProfileId;
                    return false;
                }
                if (profile.Gold < 0 || profile.InitialIngredients < 0 || profile.InitialFood < 0)
                {
                    diagnostic = "初始世界配置资源为负数：" + profile.ProfileId;
                    return false;
                }
            }

            if (defaults != 1)
            {
                diagnostic = "初始世界配置必须恰好有 1 条默认 profile（当前 " + defaults + " 条）。";
                return false;
            }

            var indexes = new HashSet<int>();
            var personDefinitions = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < population.Count; i++)
            {
                GeneratedInitialPopulationEntry entry = population[i];
                if (entry == null)
                {
                    diagnostic = "初始人口第 " + i + " 条为 null。";
                    return false;
                }
                if (entry.StableIndex < 0)
                {
                    diagnostic = "初始人口稳定序号不能为负数：" + entry.StableIndex;
                    return false;
                }
                if (!indexes.Add(entry.StableIndex))
                {
                    diagnostic = "初始人口稳定序号重复：" + entry.StableIndex;
                    return false;
                }
                if (string.IsNullOrEmpty(entry.PersonDefinitionId))
                {
                    diagnostic = "初始人口第 " + i + " 条缺少 PersonDefinitionId。";
                    return false;
                }
                if (!personDefinitions.Add(entry.PersonDefinitionId))
                {
                    diagnostic = "初始人口出现重复 PersonDefinitionId：" + entry.PersonDefinitionId;
                    return false;
                }
            }

            indexes.Clear();
            for (int i = 0; i < buildings.Count; i++)
            {
                GeneratedInitialBuildingEntry entry = buildings[i];
                if (entry == null)
                {
                    diagnostic = "初始建筑第 " + i + " 条为 null。";
                    return false;
                }
                if (entry.StableIndex < 0)
                {
                    diagnostic = "初始建筑稳定序号不能为负数：" + entry.StableIndex;
                    return false;
                }
                if (!indexes.Add(entry.StableIndex))
                {
                    diagnostic = "初始建筑稳定序号重复：" + entry.StableIndex;
                    return false;
                }
                if (string.IsNullOrEmpty(entry.BuildingDefinitionId))
                {
                    diagnostic = "初始建筑第 " + i + " 条缺少 BuildingDefinitionId。";
                    return false;
                }
                if (float.IsNaN(entry.WorldX) || float.IsInfinity(entry.WorldX)
                    || float.IsNaN(entry.WorldY) || float.IsInfinity(entry.WorldY))
                {
                    diagnostic = "初始建筑坐标不是有限数值：" + entry.BuildingDefinitionId;
                    return false;
                }
            }

            return true;
        }
    }
}
