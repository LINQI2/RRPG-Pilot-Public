using System;
using System.Collections.Generic;
using RRPG.StaticConfig;
using UnityEngine;

/// <summary>
/// InitialWorldConfigurationCatalog —— 初始世界配置的**唯一读取 / 内容校验入口**（Host 层；Assembly-CSharp）。
///
/// 正式数据链（本包已实际接线）：
///   `Development/Config/RRPG_InitialWorld.xlsx`
///     → RRPG.ConfigCompiler → `Development/Generated/StaticConfig/Initial*.json`（Canonical Mirror）
///     → Unity Builder → `InitialWorldConfigDatabase`（挂在唯一 RRPGStaticConfigRoot）
///     → `StaticConfigRegistry.TryGetInitialWorld`
///     → 本类映射成纯 Runtime 模型 `InitialWorldConfiguration` → SaveSystem.NewGame
///
/// 边界：
///   - 本类**不是**第二套 Authoring：它只读生成容器，不写 JSON / SO / 存档；
///   - 默认产品入口 `SaveSystem.NewGame()` 实际走 <see cref="TryReadDefault"/>；
///     生成缺失 / 为空 / 内容非法 → **明确失败**（返回 false + 诊断），
///     不静默退回旧的 hero/gold/空建筑硬编码；
///   - 生成容器的读取一律走深拷贝（`InitialWorldConfigDatabase.Copy*`），
///     调用方拿到的配置与生成数据互不影响；
///   - 纯结构校验（模型自身）仍由 <see cref="InitialWorldConfigurationValidator"/> 负责；
///     本类额外做内容引用 + 占地校验：PersonDefinition / Hero / BuildingDefinition 必须真实存在，
///     建筑必须通过既有 BuildingPlacementValidator 的边界与重叠规则。
/// </summary>
public static class InitialWorldConfigurationCatalog
{
    /// <summary>
    /// 从 **Compiler → Builder → Registry 生成链**读取默认配置并完成全量校验。
    /// 每次返回新的模型对象；不共享可变状态。
    /// </summary>
    public static bool TryReadDefault(out InitialWorldConfiguration config, out string diagnostic)
    {
        config = null;

        if (!StaticConfigRegistry.TryGetInitialWorld(out InitialWorldConfigDatabase database, out diagnostic))
        {
            return false;
        }

        if (!TryBuildFromGenerated(database, out config, out diagnostic))
        {
            return false;
        }

        return TryValidate(config, out diagnostic);
    }

    /// <summary>
    /// 把生成容器映射成纯 Runtime 模型（**只做深拷贝 + 结构校验**，不访问 DataManager / 场景）。
    /// 测试可用内存容器显式验证映射，不依赖真实默认内容。
    /// </summary>
    public static bool TryBuildFromGenerated(InitialWorldConfigDatabase database,
        out InitialWorldConfiguration config, out string diagnostic)
    {
        config = null;

        if (database == null)
        {
            diagnostic = "初始世界生成容器为 null（尚未导表 / Builder 未生成）。";
            return false;
        }

        List<GeneratedInitialWorldProfile> profiles = database.CopyProfiles();
        List<GeneratedInitialPopulationEntry> population = database.CopyPopulation();
        List<GeneratedInitialBuildingEntry> buildings = database.CopyBuildings();

        if (!InitialWorldGeneratedConfigValidator.TryValidate(profiles, population, buildings, out diagnostic))
        {
            return false;
        }

        if (!database.TryCopyDefaultProfile(out GeneratedInitialWorldProfile profile, out diagnostic))
        {
            return false;
        }

        config = new InitialWorldConfiguration
        {
            InitialHeroId = profile.InitialHeroId,
            Gold = profile.Gold,
            InitialIngredients = profile.InitialIngredients,
            InitialFood = profile.InitialFood
        };

        foreach (GeneratedInitialPopulationEntry entry in population)
        {
            config.People.Add(new InitialWorldPerson { PersonDefinitionId = entry.PersonDefinitionId });
        }

        foreach (GeneratedInitialBuildingEntry entry in buildings)
        {
            config.Buildings.Add(new InitialWorldBuilding
            {
                BuildingDefinitionId = entry.BuildingDefinitionId,
                WorldX = entry.WorldX,
                WorldY = entry.WorldY
            });
        }

        return true;
    }

    /// <summary>
    /// 兼容旧调用点的 fail-closed 包装：读取默认配置失败时抛出明确异常，
    /// 绝不返回旧硬编码 / 空建筑。新代码应直接使用 <see cref="TryReadDefault"/>。
    /// </summary>
    public static InitialWorldConfiguration CreateDefault()
    {
        if (!TryReadDefault(out InitialWorldConfiguration config, out string diagnostic))
        {
            throw new InvalidOperationException("默认初始世界配置不可用：" + diagnostic);
        }
        return config;
    }

    /// <summary>
    /// 校验配置的内容引用与初始布局；true = 可安全提交给 NewGame。
    /// 本方法只读（DataManager / StaticConfigRegistry / 纯 Placement 规则），不写任何权威。
    /// </summary>
    public static bool TryValidate(InitialWorldConfiguration config, out string diagnostic)
    {
        if (!InitialWorldConfigurationValidator.TryValidate(config, out diagnostic)) return false;

        DataManager dm = DataManager.Instance;
        if (dm == null)
        {
            diagnostic = "DataManager 不可用：无法校验初始人口 / 英雄引用";
            return false;
        }

        if (dm.GetHeroById(config.InitialHeroId) == null)
        {
            diagnostic = "初始英雄模板不存在：HeroId=" + config.InitialHeroId;
            return false;
        }

        foreach (InitialWorldPerson person in config.People)
        {
            PersonDefinition definition = dm.GetPersonDefinitionById(person.PersonDefinitionId);
            if (definition == null)
            {
                diagnostic = "初始人口引用的 PersonDefinition 不存在：" + person.PersonDefinitionId;
                return false;
            }
            if (!PersonDefinitionValidator.TryValidate(definition, out List<string> errors))
            {
                diagnostic = "初始人口配置非法：" + person.PersonDefinitionId + "：" + string.Join(" | ", errors);
                return false;
            }
        }

        if (config.Buildings.Count > 0)
        {
            if (!StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, out diagnostic))
            {
                diagnostic = "初始建筑需要 BuildingRegistry，但当前不可用：" + diagnostic;
                return false;
            }

            var placed = new List<BuildingInstance>();
            for (int i = 0; i < config.Buildings.Count; i++)
            {
                InitialWorldBuilding building = config.Buildings[i];
                if (!definitions.TryGetById(building.BuildingDefinitionId, out _))
                {
                    diagnostic = "初始建筑[" + i + "] 引用的 BuildingDefinition 不存在：" + building.BuildingDefinitionId;
                    return false;
                }
                string placement = BuildingPlacementValidator.Validate(
                    definitions, building.BuildingDefinitionId, building.WorldX, building.WorldY, placed);
                if (placement.Length != 0)
                {
                    diagnostic = "初始建筑[" + i + "]（" + building.BuildingDefinitionId + " @ "
                        + building.WorldX + "," + building.WorldY + "）非法：" + placement;
                    return false;
                }
                // 仅用于后续条目的重叠判定；不会进入任何运行时 / 存档。
                placed.Add(new BuildingInstance("initial_layout_validation_" + i,
                    building.BuildingDefinitionId, building.WorldX, building.WorldY,
                    BuildingLifecycleState.Completed));
            }
        }

        return true;
    }
}
