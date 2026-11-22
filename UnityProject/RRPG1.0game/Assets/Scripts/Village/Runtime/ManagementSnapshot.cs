using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// V17-R3-B 集中管理只读投影模型（Village.Runtime 纯数据）。
///
/// 职责：一次读取后只复制显示事实（人口 / 职责与场所 / 施工 / 资源）。
/// 不持有 Person / Assignment / Project / Resource / Save 的可写引用，
/// 不初始化世界、不推进时钟、不产生预测、不做自动派工。
/// 过滤与排序只属会话展示；快照本身不可变，只由 ManagementReadAdapter 组装。
/// </summary>

/// <summary>人口筛选（会话级展示条件，不写入任何 Authority）。</summary>
public enum ManagementPersonFilter
{
    All,
    InVillage,
    NotInVillage
}

/// <summary>岗位行来源：正式建筑实例 Function 岗位 / 既有静态岗位兼容记录。</summary>
public enum ManagementWorkplaceKind
{
    /// <summary>已完成 Building 实例的 FunctionBinding 岗位（正式新增派工目标）。</summary>
    BuildingFunction,
    /// <summary>既有静态 Assignment（兼容显示与撤职；不是正式新增目标）。</summary>
    LegacyCompatibility
}

/// <summary>产品命令结果码。领域拒绝保留领域原始诊断，不在 UI 层重新解释规则。</summary>
public enum ManagementCommandOutcome
{
    Success,
    NotReady,
    PersonMissing,
    PersonNotInVillage,
    WorldBusy,
    TargetUnknown,
    TargetNotCompleted,
    TargetAlreadyCompleted,
    TargetNotRegistered,
    StaticTargetNotAssignable,
    DomainRejected,
    SaveRejected
}

/// <summary>窄产品命令回执：成功 / 拒绝原因 / 领域原始诊断，均不含可写对象。</summary>
public sealed class ManagementCommandResult
{
    public ManagementCommandOutcome Outcome { get; }
    public bool Succeeded => Outcome == ManagementCommandOutcome.Success;
    public string PersonId { get; }
    public string TargetId { get; }
    /// <summary>面向玩家的中文说明（只描述已发生的本次操作）。</summary>
    public string Message { get; }
    /// <summary>领域枚举原文（如 CapacityFull / AlreadyAssigned / PersonCurrentlyUnavailable）；未进入领域时为空串。</summary>
    public string DomainDiagnostic { get; }

    public ManagementCommandResult(ManagementCommandOutcome outcome, string personId, string targetId,
        string message, string domainDiagnostic = "")
    {
        Outcome = outcome;
        PersonId = personId ?? string.Empty;
        TargetId = targetId ?? string.Empty;
        Message = message ?? string.Empty;
        DomainDiagnostic = domainDiagnostic ?? string.Empty;
    }

    public override string ToString()
        => Outcome + (string.IsNullOrEmpty(DomainDiagnostic) ? string.Empty : ":" + DomainDiagnostic) + " · " + Message;
}

/// <summary>人口行：同一快照内的姓名 / 职业 / 可用状态 / 主要职责 / 现时执行 / 临时承诺 / 饥饿。</summary>
public sealed class ManagementPersonRow
{
    public string PersonId { get; }
    public string DisplayName { get; }
    public bool IsHero { get; }
    public string Profession { get; }
    public VillageAvailability Availability { get; }
    public string AvailabilityLabel { get; }
    public string PrimaryWorkplaceId { get; }
    public string PrimaryResponsibility { get; }
    /// <summary>主要职责来自静态岗位兼容记录（不是 Completed Building 的 Function 岗位）。</summary>
    public bool AssignmentIsLegacy { get; }
    public string CurrentExecution { get; }
    public string TemporaryCommitment { get; }
    /// <summary>false = 当前源没有可读饥饿事实（不伪造“已满足”）。</summary>
    public bool HasNutritionFacts { get; }
    public bool Hungry { get; }
    public int ConsecutiveUnderfedDays { get; }

    public bool IsInVillage => Availability == VillageAvailability.InVillage;

    public ManagementPersonRow(string personId, string displayName, bool isHero, string profession,
        VillageAvailability availability, string availabilityLabel, string primaryWorkplaceId,
        string primaryResponsibility, bool assignmentIsLegacy, string currentExecution,
        string temporaryCommitment, bool hasNutritionFacts, bool hungry, int consecutiveUnderfedDays)
    {
        PersonId = personId ?? string.Empty;
        DisplayName = string.IsNullOrEmpty(displayName) ? PersonId : displayName;
        IsHero = isHero;
        Profession = profession ?? string.Empty;
        Availability = availability;
        AvailabilityLabel = availabilityLabel ?? string.Empty;
        PrimaryWorkplaceId = primaryWorkplaceId ?? string.Empty;
        PrimaryResponsibility = primaryResponsibility ?? string.Empty;
        AssignmentIsLegacy = assignmentIsLegacy;
        CurrentExecution = currentExecution ?? string.Empty;
        TemporaryCommitment = temporaryCommitment ?? string.Empty;
        HasNutritionFacts = hasNutritionFacts;
        Hungry = hungry;
        ConsecutiveUnderfedDays = consecutiveUnderfedDays;
    }
}

/// <summary>
/// 职责与场所行：一行一个真实 WorkplaceId。
/// 名额（长期 Assignment 绑定）/ 实际劳动人数 / 现场人数分列，互不替代。
/// </summary>
public sealed class ManagementWorkplaceRow
{
    public string WorkplaceId { get; }
    public ManagementWorkplaceKind Kind { get; }
    public string BuildingInstanceId { get; }
    public string BuildingDefinitionId { get; }
    public string FunctionSlotId { get; }
    public string FunctionProfileId { get; }
    public string DisplayName { get; }
    public int Capacity { get; }
    /// <summary>该 WorkplaceId 是否已在 Assignments 中真实注册（未注册不得派工）。</summary>
    public bool IsRegistered { get; }
    public IReadOnlyList<string> BoundPersonIds { get; }
    /// <summary>实际在该 WorkplaceId 执行劳动的 PersonId（真实目标匹配；绑定额不替代）。</summary>
    public IReadOnlyList<string> LaborPersonIds { get; }
    public IReadOnlyList<string> OnsitePersonIds { get; }

    public int BoundCount => BoundPersonIds.Count;
    public int LaborCount => LaborPersonIds.Count;
    public int OnsiteCount => OnsitePersonIds.Count;
    public bool IsFull => IsRegistered && Capacity > 0 && BoundCount >= Capacity;
    /// <summary>正式新增派工只接受已完成建筑实例、已注册、未满员的岗位。</summary>
    public bool CanAssign => Kind == ManagementWorkplaceKind.BuildingFunction && IsRegistered && !IsFull;

    public ManagementWorkplaceRow(string workplaceId, ManagementWorkplaceKind kind, string buildingInstanceId,
        string buildingDefinitionId, string functionSlotId, string functionProfileId, string displayName,
        int capacity, bool isRegistered, IEnumerable<string> bound, IEnumerable<string> labor,
        IEnumerable<string> onsite)
    {
        WorkplaceId = workplaceId ?? string.Empty;
        Kind = kind;
        BuildingInstanceId = buildingInstanceId ?? string.Empty;
        BuildingDefinitionId = buildingDefinitionId ?? string.Empty;
        FunctionSlotId = functionSlotId ?? string.Empty;
        FunctionProfileId = functionProfileId ?? string.Empty;
        DisplayName = string.IsNullOrEmpty(displayName) ? WorkplaceId : displayName;
        Capacity = capacity;
        IsRegistered = isRegistered;
        BoundPersonIds = Array.AsReadOnly((bound ?? Enumerable.Empty<string>()).ToArray());
        LaborPersonIds = Array.AsReadOnly((labor ?? Enumerable.Empty<string>()).ToArray());
        OnsitePersonIds = Array.AsReadOnly((onsite ?? Enumerable.Empty<string>()).ToArray());
    }
}

/// <summary>施工行：项目进度与工人单列；零人不写进度字段（ExecutionPaused 只是读取事实）。</summary>
public sealed class ManagementConstructionRow
{
    public string BuildingInstanceId { get; }
    public string BuildingDefinitionId { get; }
    public string DisplayName { get; }
    public string ProjectId { get; }
    public int Progress { get; }
    public int Target { get; }
    public bool ExecutionPaused { get; }
    public IReadOnlyList<string> WorkerPersonIds { get; }
    public IReadOnlyList<string> OnsitePersonIds { get; }

    public int WorkerCount => WorkerPersonIds.Count;
    public int OnsiteCount => OnsitePersonIds.Count;
    public string ProgressLabel => Target > 0 ? Progress + " / " + Target : Progress.ToString();

    public ManagementConstructionRow(string buildingInstanceId, string buildingDefinitionId, string displayName,
        string projectId, int progress, int target, bool executionPaused,
        IEnumerable<string> workers, IEnumerable<string> onsite)
    {
        BuildingInstanceId = buildingInstanceId ?? string.Empty;
        BuildingDefinitionId = buildingDefinitionId ?? string.Empty;
        DisplayName = string.IsNullOrEmpty(displayName) ? BuildingInstanceId : displayName;
        ProjectId = projectId ?? string.Empty;
        Progress = progress;
        Target = target;
        ExecutionPaused = executionPaused;
        WorkerPersonIds = Array.AsReadOnly((workers ?? Enumerable.Empty<string>()).ToArray());
        OnsitePersonIds = Array.AsReadOnly((onsite ?? Enumerable.Empty<string>()).ToArray());
    }
}

/// <summary>资源页岗位行（现有 Farm / Kitchen 实例名额与实际劳动人数；不含预测或效率排名）。</summary>
public sealed class ManagementResourceWorkplaceRow
{
    public string WorkplaceId { get; }
    public string RoleLabel { get; }
    public string BuildingInstanceId { get; }
    public string FunctionSlotId { get; }
    public string DisplayName { get; }
    public int Capacity { get; }
    public int BoundCount { get; }
    public int LaborCount { get; }

    public ManagementResourceWorkplaceRow(string workplaceId, string roleLabel, string buildingInstanceId,
        string functionSlotId, string displayName, int capacity, int boundCount, int laborCount)
    {
        WorkplaceId = workplaceId ?? string.Empty;
        RoleLabel = roleLabel ?? string.Empty;
        BuildingInstanceId = buildingInstanceId ?? string.Empty;
        FunctionSlotId = functionSlotId ?? string.Empty;
        DisplayName = string.IsNullOrEmpty(displayName) ? WorkplaceId : displayName;
        Capacity = capacity;
        BoundCount = boundCount;
        LaborCount = laborCount;
    }
}

/// <summary>资源投影：真实 Ingredients / Food、在村供餐对象与其中已有 Hungry 人数。</summary>
public sealed class ManagementResourceSnapshot
{
    public bool HasWorld { get; }
    public int Ingredients { get; }
    public int Food { get; }
    public IReadOnlyList<string> InVillagePersonIds { get; }
    public IReadOnlyList<string> HungryPersonIds { get; }
    public IReadOnlyList<ManagementResourceWorkplaceRow> Workplaces { get; }

    public int InVillageCount => InVillagePersonIds.Count;
    public int HungryCount => HungryPersonIds.Count;

    public ManagementResourceSnapshot(bool hasWorld, int ingredients, int food,
        IEnumerable<string> inVillage, IEnumerable<string> hungry, IEnumerable<ManagementResourceWorkplaceRow> workplaces)
    {
        HasWorld = hasWorld;
        Ingredients = ingredients;
        Food = food;
        InVillagePersonIds = Array.AsReadOnly((inVillage ?? Enumerable.Empty<string>()).ToArray());
        HungryPersonIds = Array.AsReadOnly((hungry ?? Enumerable.Empty<string>()).ToArray());
        Workplaces = Array.AsReadOnly((workplaces ?? Enumerable.Empty<ManagementResourceWorkplaceRow>()).ToArray());
    }
}

/// <summary>集中管理一次性只读快照。分组（人口 / 职责与场所 / 施工 / 资源）共用同一次读取。</summary>
public sealed class ManagementSnapshot
{
    public bool Ready { get; }
    public IReadOnlyList<ManagementPersonRow> Persons { get; }
    public IReadOnlyList<ManagementWorkplaceRow> Workplaces { get; }
    /// <summary>既有静态岗位兼容记录（只用于显示与撤职，不作为正式新增目标）。</summary>
    public IReadOnlyList<ManagementWorkplaceRow> LegacyAssignments { get; }
    public IReadOnlyList<ManagementConstructionRow> Construction { get; }
    public ManagementResourceSnapshot Resources { get; }
    public IReadOnlyList<string> Diagnostics { get; }

    public ManagementSnapshot(bool ready, IEnumerable<ManagementPersonRow> persons,
        IEnumerable<ManagementWorkplaceRow> workplaces, IEnumerable<ManagementConstructionRow> construction,
        ManagementResourceSnapshot resources, IEnumerable<string> diagnostics)
    {
        Ready = ready;
        Persons = Array.AsReadOnly((persons ?? Enumerable.Empty<ManagementPersonRow>()).ToArray());
        var rows = (workplaces ?? Enumerable.Empty<ManagementWorkplaceRow>()).ToArray();
        Workplaces = Array.AsReadOnly(rows);
        LegacyAssignments = Array.AsReadOnly(rows.Where(r => r.Kind == ManagementWorkplaceKind.LegacyCompatibility).ToArray());
        Construction = Array.AsReadOnly((construction ?? Enumerable.Empty<ManagementConstructionRow>()).ToArray());
        Resources = resources ?? new ManagementResourceSnapshot(false, 0, 0, null, null, null);
        Diagnostics = Array.AsReadOnly((diagnostics ?? Enumerable.Empty<string>()).ToArray());
    }

    public ManagementPersonRow Person(string id)
        => string.IsNullOrEmpty(id) ? null : Persons.FirstOrDefault(p => p.PersonId == id);

    public ManagementWorkplaceRow Workplace(string id)
        => string.IsNullOrEmpty(id) ? null : Workplaces.FirstOrDefault(w => w.WorkplaceId == id);

    /// <summary>会话级筛选（姓名包含 + 可用状态）；只返回新快照，不改动源数据或 Authority。</summary>
    public ManagementSnapshot Filter(string nameContains, ManagementPersonFilter filter)
    {
        var persons = Persons.Where(p =>
                (string.IsNullOrEmpty(nameContains)
                 || (p.DisplayName ?? string.Empty).IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0)
                && (filter == ManagementPersonFilter.All
                    || (filter == ManagementPersonFilter.InVillage && p.IsInVillage)
                    || (filter == ManagementPersonFilter.NotInVillage && !p.IsInVillage)))
            .ToArray();
        return new ManagementSnapshot(Ready, persons, Workplaces, Construction, Resources, Diagnostics);
    }

    /// <summary>世界未就绪：保留明确空态，不伪造人口 / 资源行。</summary>
    public static ManagementSnapshot NotReady(IEnumerable<string> diagnostics)
        => new ManagementSnapshot(false, null, null, null,
            new ManagementResourceSnapshot(false, 0, 0, null, null, null), diagnostics);
}
