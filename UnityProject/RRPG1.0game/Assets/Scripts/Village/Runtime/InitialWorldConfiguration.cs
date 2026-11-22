using System;
using System.Collections.Generic;

/// <summary>
/// InitialWorldConfiguration —— 正式 New Game 初始世界配置的**统一数据模型**（纯逻辑；Village.Runtime）。
///
/// 职责：
///   - 把「初始人口 / 初始资源 / 初始建筑布局」放进**同一份配置**，由同一读入口消费；
///   - 只承载数据，不创建实例、不写运行时状态、不读 Unity 场景 / Prefab / DTO。
///
/// Authority 边界：
///   - 本类不是 Authoring Authority：默认值由
///     <c>InitialWorldConfigurationCatalog.TryReadDefault()</c> 从生成链
///     （XLSX → Compiler → Canonical Mirror → Unity Builder → StaticConfigRegistry）映射而来；
///   - 生成缺失 / 非法 → 明确失败，不退回硬编码 hero / gold / 0 建筑；
///   - 所有初始数值均为 **Test / Not Frozen**：不冻结人口数量、住房数量或建筑布局。
///
/// 禁止：
///   - 公开全局可变 setter；配置只以显式参数 / 新副本形式传递；
///   - 把本模型接成 Save DTO（SaveVersion 1 / DTO 结构不变；Continue 只恢复存档，不重跑本配置）。
/// </summary>
[Serializable]
public sealed class InitialWorldConfiguration
{
    /// <summary>初始英雄模板 ID（当前实际值 hero_001；Hero 选择的替换时序见 NewGame 实现）。</summary>
    public string InitialHeroId = "hero_001";

    /// <summary>初始金币（当前 PlayerCollection.InitializeDefaultCollection 实际值 100；Test / Not Frozen）。</summary>
    public int Gold = 100;

    /// <summary>初始 Ingredients（当前 VillageResourceState 默认 0；Test / Not Frozen）。</summary>
    public int InitialIngredients;

    /// <summary>初始 Food（当前 VillageResourceState 默认 0；Test / Not Frozen）。</summary>
    public int InitialFood;

    /// <summary>
    /// 初始人口（按 PersonDefinitionId 顺序创建；空列表 = 沿用既有 Legacy 卡册路径，保持旧工程兼容）。
    /// 代码不写死人数或名单：默认值从 DataManager.InitialPersonRoster 提取。
    /// </summary>
    public List<InitialWorldPerson> People = new List<InitialWorldPerson>();

    /// <summary>
    /// 初始建筑布局（按列表顺序分配实例身份；本模型只表达 Completed 初始建筑）。
    /// 空列表 = 与当前产品 NewGame 实际值一致（无初始建筑）。
    /// </summary>
    public List<InitialWorldBuilding> Buildings = new List<InitialWorldBuilding>();
}

/// <summary>初始人口条目（只引用既有 PersonDefinition；不复制人物长期数据）。</summary>
[Serializable]
public sealed class InitialWorldPerson
{
    public string PersonDefinitionId;
}

/// <summary>初始建筑条目（只引用既有 BuildingDefinition；位置为世界坐标，占用/重叠由既有 Placement 校验）。</summary>
[Serializable]
public sealed class InitialWorldBuilding
{
    public string BuildingDefinitionId;
    public float WorldX;
    public float WorldY;
}

/// <summary>
/// InitialWorldConfigurationValidator —— 配置的**纯结构校验**（不访问 DataManager / Registry / 场景）。
///
/// 只回答「这份配置自身是否自洽」：null / 必填 ID / 重复 / 数值范围 / 非有限坐标。
/// 内容引用（PersonDefinition / Hero / BuildingDefinition 是否真实存在）与占地校验
/// 由 Host 层 `InitialWorldConfigurationCatalog.TryValidate` 与
/// `VillageRuntimeBridge.TryPrepareNewGameWorld` 在**任何世界写入之前**完成。
/// </summary>
public static class InitialWorldConfigurationValidator
{
    public static bool TryValidate(InitialWorldConfiguration config, out string diagnostic)
    {
        diagnostic = string.Empty;
        if (config == null)
        {
            diagnostic = "初始世界配置为 null";
            return false;
        }

        if (string.IsNullOrEmpty(config.InitialHeroId))
        {
            diagnostic = "初始英雄模板 ID 不能为空";
            return false;
        }

        if (config.Gold < 0)
        {
            diagnostic = "初始金币不能为负数：" + config.Gold;
            return false;
        }

        if (config.InitialIngredients < 0)
        {
            diagnostic = "初始 Ingredients 不能为负数：" + config.InitialIngredients;
            return false;
        }

        if (config.InitialFood < 0)
        {
            diagnostic = "初始 Food 不能为负数：" + config.InitialFood;
            return false;
        }

        if (config.People == null)
        {
            diagnostic = "初始人口列表为 null（空列表是合法状态）";
            return false;
        }

        var personIds = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < config.People.Count; i++)
        {
            InitialWorldPerson person = config.People[i];
            if (person == null || string.IsNullOrEmpty(person.PersonDefinitionId))
            {
                diagnostic = "初始人口[" + i + "] 缺少 PersonDefinitionId";
                return false;
            }
            if (!personIds.Add(person.PersonDefinitionId))
            {
                diagnostic = "初始人口出现重复 PersonDefinitionId：" + person.PersonDefinitionId;
                return false;
            }
        }

        if (config.Buildings == null)
        {
            diagnostic = "初始建筑列表为 null（空列表是合法状态）";
            return false;
        }

        for (int i = 0; i < config.Buildings.Count; i++)
        {
            InitialWorldBuilding building = config.Buildings[i];
            if (building == null || string.IsNullOrEmpty(building.BuildingDefinitionId))
            {
                diagnostic = "初始建筑[" + i + "] 缺少 BuildingDefinitionId";
                return false;
            }
            if (float.IsNaN(building.WorldX) || float.IsInfinity(building.WorldX)
                || float.IsNaN(building.WorldY) || float.IsInfinity(building.WorldY))
            {
                diagnostic = "初始建筑[" + i + "]（" + building.BuildingDefinitionId + "）坐标不是有限数值";
                return false;
            }
        }

        return true;
    }
}
