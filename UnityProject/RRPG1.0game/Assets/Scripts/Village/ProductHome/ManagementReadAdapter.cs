using System;
using System.Collections.Generic;
using System.Linq;
using RRPG.StaticConfig;

/// <summary>
/// V17-R3-B 集中管理薄只读投影（Assembly-CSharp 组合层）。
///
/// 只组合现有查询：ProductHomeReadAdapter（人口 / 地点）、VillageAssignmentSystem（岗位与名额）、
/// BuildingRuntime + StaticConfig FunctionBindings（真实 Function 岗位 identity）、
/// VillageProjectSystem（施工进度与工人）、VillageResourceState（Ingredients / Food）。
/// 不初始化世界、不推进时钟、不写 Assignment / Project / Resource / Save，不产生预测或自动派工。
/// </summary>
public static class ManagementReadAdapter
{
    private const string UnregisteredAnchorPrefix = "Unregistered presentation anchor:";

    public static ManagementSnapshot Read() => Read(null);

    /// <summary>
    /// definitions 非 null 只用于测试 / 诊断注入同一 BuildingRegistry 类型；
    /// 产品调用一律走 StaticConfigRegistry 的同一静态配置 Authority。
    /// </summary>
    public static ManagementSnapshot Read(BuildingRegistry definitions)
    {
        var diagnostics = new List<string>();
        var source = VillageRuntimeBridge.SimulationPersonSource;
        var clock = VillageRuntimeBridge.Time;
        var assignments = VillageRuntimeBridge.Assignments;
        var projects = VillageRuntimeBridge.Projects;
        var buildings = VillageRuntimeBridge.Buildings;

        if (source == null || clock == null || assignments == null || VillageRuntimeBridge.IntradayWork == null)
        {
            diagnostics.Add("村庄世界未就绪：缺少人物源 / 时钟 / 岗位或日内工作会话。");
            return ManagementSnapshot.NotReady(diagnostics);
        }

        var home = ProductHomeReadAdapter.Read();
        diagnostics.AddRange(home.Diagnostics.Where(d => d != null && !d.StartsWith(UnregisteredAnchorPrefix, StringComparison.Ordinal)));

        var persons = ReadPersons(source, assignments, home, diagnostics);
        var workplaces = new List<ManagementWorkplaceRow>();
        var construction = new List<ManagementConstructionRow>();
        ReadBuildings(definitions, buildings, projects, assignments, home, workplaces, construction, diagnostics);
        ReadLegacyWorkplaces(assignments, home, workplaces);
        if (VillageRuntimeBridge.Resources == null)
            diagnostics.Add("资源权威未就绪：Ingredients / Food 暂不可读。");

        workplaces.Sort(CompareWorkplaces);
        construction.Sort((a, b) => string.CompareOrdinal(a.BuildingInstanceId, b.BuildingInstanceId));
        var resourceRows = workplaces.Where(w => w.Kind == ManagementWorkplaceKind.BuildingFunction)
            .Where(w => w.FunctionProfileId == "ws_farm_01" || w.FunctionProfileId == "ws_kitchen_01")
            .OrderBy(w => w.WorkplaceId, StringComparer.Ordinal)
            .Select(w => new ManagementResourceWorkplaceRow(w.WorkplaceId,
                w.FunctionProfileId == "ws_farm_01" ? "Farm" : "Kitchen",
                w.BuildingInstanceId, w.FunctionSlotId, w.DisplayName, w.Capacity, w.BoundCount, w.LaborCount))
            .ToArray();

        var inVillage = new List<string>();
        var hungry = new List<string>();
        ReadFulfillment(source, inVillage, hungry);

        var resourceState = VillageRuntimeBridge.Resources;
        var resources = new ManagementResourceSnapshot(resourceState != null,
            resourceState != null ? resourceState.Ingredients : 0,
            resourceState != null ? resourceState.Food : 0,
            inVillage, hungry, resourceRows);

        return new ManagementSnapshot(true, persons, workplaces, construction, resources, diagnostics);
    }

    private static List<ManagementPersonRow> ReadPersons(PlayerCollection source, VillageAssignmentSystem assignments,
        ProductHomeSnapshot home, List<string> diagnostics)
    {
        var rows = new List<ManagementPersonRow>();
        var views = VillagePersonAvailability.Resolve(VillageRuntimeBridge.BuildPersonInputs(source),
            VillageRuntimeBridge.ExpeditionPersonIds);
        foreach (var view in views)
        {
            var person = source.FindPersonByPersonId(view.PersonId);
            if (person == null)
            {
                // 源在读取瞬间发生变化：明确空态，不猜测、不合成人物。
                diagnostics.Add("人物来源中缺少该 PersonId：" + view.PersonId);
                continue;
            }

            var character = person as CharacterInstance;
            string profession = character == null
                ? "Hero（沿用现有英雄成长）"
                : character.ActiveProfessionProgress?.ProfessionId ?? "尚未形成职业";
            string label = view.Availability == VillageAvailability.InVillage ? "在村"
                : view.Availability == VillageAvailability.Expedition ? "远征中（保留职责）" : "不可用";
            string assignmentId = assignments.GetWorkplaceId(view.PersonId) ?? string.Empty;
            var assignmentDef = assignments.GetWorkplaceDef(assignmentId);
            string responsibility = string.IsNullOrEmpty(assignmentId)
                ? "无主要职责"
                : assignmentDef != null ? assignmentDef.DisplayName : assignmentId;
            bool legacy = !string.IsNullOrEmpty(assignmentId)
                          && !assignmentId.StartsWith("workplace:", StringComparison.Ordinal);

            string execution = VillageRuntimeBridge.GetWorkExecutionLabel(view.PersonId);
            string commitment = VillageRuntimeBridge.GetPersonOccupancyReason(view.PersonId);
            if (string.IsNullOrEmpty(commitment))
            {
                string claim = ActivityRuntimeBridge.QueryClaim(view.PersonId);
                if (!string.IsNullOrEmpty(claim)) commitment = "自主活动进行中：" + claim;
            }

            var host = person as IFulfillmentHost;
            rows.Add(new ManagementPersonRow(view.PersonId, view.DisplayName, character == null, profession,
                view.Availability, label, assignmentId, responsibility, legacy, execution,
                commitment, host != null, host != null && host.Hungry, host != null ? host.ConsecutiveUnderfedDays : 0));
        }

        rows.Sort((a, b) =>
        {
            int byName = string.Compare(a.DisplayName, b.DisplayName, StringComparison.Ordinal);
            return byName != 0 ? byName : string.CompareOrdinal(a.PersonId, b.PersonId);
        });
        return rows;
    }

    private static void ReadBuildings(BuildingRegistry definitions, BuildingRuntime buildings,
        VillageProjectSystem projects, VillageAssignmentSystem assignments, ProductHomeSnapshot home,
        List<ManagementWorkplaceRow> workplaces, List<ManagementConstructionRow> construction, List<string> diagnostics)
    {
        if (buildings == null) return;
        if (definitions == null
            && !StaticConfigRegistry.TryGetBuildings(out definitions, out string buildingDiagnostic))
        {
            diagnostics.Add(buildingDiagnostic);
            return;
        }

        foreach (var building in buildings.All)
        {
            if (!definitions.TryGetById(building.BuildingDefinitionId, out var definition))
            {
                diagnostics.Add("未知 BuildingDefinitionId：" + building.BuildingDefinitionId);
                continue;
            }

            if (building.LifecycleState == BuildingLifecycleState.Completed)
            {
                foreach (var binding in definitions.FunctionBindings.Where(f => f.BuildingDefinitionId == building.BuildingDefinitionId))
                {
                    string workplaceId = VillageRuntimeBridge.BuildingWorkplaceId(building.InstanceId, binding.FunctionSlotId);
                    var registered = assignments.GetWorkplaceDef(workplaceId);
                    if (registered == null)
                        diagnostics.Add("已完工建筑的功能岗位未注册：" + workplaceId);
                    var bound = registered == null ? new List<string>() : assignments.GetOccupantPersonIds(workplaceId);
                    workplaces.Add(new ManagementWorkplaceRow(workplaceId, ManagementWorkplaceKind.BuildingFunction,
                        building.InstanceId, binding.BuildingDefinitionId, binding.FunctionSlotId, binding.FunctionProfileId,
                        registered != null ? registered.DisplayName : definition.DisplayName + " · " + binding.FunctionSlotId,
                        registered != null ? registered.Capacity : 0, registered != null,
                        bound, Labor(bound, workplaceId), Onsite(home, workplaceId)));
                }
                continue;
            }

            var project = string.IsNullOrEmpty(building.ConstructionProjectId)
                ? null
                : projects?.GetActiveProject(building.ConstructionProjectId);
            var projectDef = project != null ? projects.GetProjectDef(project.ProjectDefId) : ConstructionProjectDef();
            var workers = project != null ? project.ParticipantPersonIds : new List<string>();
            construction.Add(new ManagementConstructionRow(building.InstanceId, building.BuildingDefinitionId,
                definition.DisplayName, project?.ProjectId ?? string.Empty, project?.Progress ?? 0,
                projectDef?.TestProgressTarget ?? 0, project == null || project.ExecutionPaused,
                workers, Onsite(home, VillageRuntimeBridge.ConstructionPlaceId(building.InstanceId))));
        }
    }

    private static VillageProjectDef ConstructionProjectDef()
    {
        var defs = VillageRuntimeBridge.TestProjectDefs;
        var preferred = defs.FirstOrDefault(d => d != null && d.ProjectDefId == "prj_test_01");
        return preferred ?? defs.FirstOrDefault();
    }

    private static void ReadLegacyWorkplaces(VillageAssignmentSystem assignments, ProductHomeSnapshot home,
        List<ManagementWorkplaceRow> workplaces)
    {
        foreach (var def in assignments.WorkplaceDefs)
        {
            if (def == null || string.IsNullOrEmpty(def.WorkplaceId)) continue;
            if (def.WorkplaceId.StartsWith("workplace:", StringComparison.Ordinal)) continue;
            var bound = assignments.GetOccupantPersonIds(def.WorkplaceId);
            if (bound.Count == 0) continue; // 只显示真实存在的兼容记录，不把固定种子扩成建筑全集
            workplaces.Add(new ManagementWorkplaceRow(def.WorkplaceId, ManagementWorkplaceKind.LegacyCompatibility,
                null, null, null, null, def.DisplayName, def.Capacity, true, bound, Labor(bound, def.WorkplaceId),
                Onsite(home, def.WorkplaceId)));
        }
    }

    /// <summary>
    /// 在村供餐对象与 Hungry 人数：与每日结算输入同源（非死亡、非远征的 IFulfillmentHost），
    /// 只读复制事实，不复制结算规则、不写 Hungry。
    /// </summary>
    private static void ReadFulfillment(PlayerCollection source, List<string> inVillage, List<string> hungry)
    {
        var expedition = new HashSet<string>(VillageRuntimeBridge.ExpeditionPersonIds ?? new string[0], StringComparer.Ordinal);
        if (source.OwnedCharacters != null)
            foreach (var character in source.OwnedCharacters)
            {
                if (character == null || character.IsDead || string.IsNullOrEmpty(character.PersonId)) continue;
                if (expedition.Contains(character.PersonId)) continue;
                if (!(character is IFulfillmentHost host)) continue;
                inVillage.Add(host.PersonId);
                if (host.Hungry) hungry.Add(host.PersonId);
            }
        if (source.OwnedHeroes != null)
            foreach (var hero in source.OwnedHeroes)
            {
                if (hero == null || string.IsNullOrEmpty(hero.PersonId)) continue;
                if (expedition.Contains(hero.PersonId)) continue;
                if (!(hero is IFulfillmentHost host)) continue;
                inVillage.Add(host.PersonId);
                if (host.Hungry) hungry.Add(host.PersonId);
            }
        inVillage.Sort(StringComparer.Ordinal);
        hungry.Sort(StringComparer.Ordinal);
    }

    /// <summary>
    /// 实际劳动人数：绑定只提供候选，必须同时满足「正在执行工作」且现有 Presence 权威派生的
    /// 真实目标就是该 WorkplaceId（ProjectSite 优先，施工不回流原岗位）。只看绑定或 IsExecutingWork
    /// 会把保留 Primary Assignment、实际投入施工的人误计为原岗位劳动。
    /// </summary>
    private static IReadOnlyList<string> Labor(IEnumerable<string> bound, string workplaceId)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(workplaceId)) return result;
        foreach (string id in bound)
        {
            if (!VillageRuntimeBridge.IsExecutingWork(id)) continue;
            var presence = VillageRuntimeBridge.GetPersonPresence(id);
            if (presence.State != VillagePresenceState.Workplace
                || !string.Equals(presence.PlaceId, workplaceId, StringComparison.Ordinal)) continue;
            result.Add(id);
        }
        return result;
    }

    private static IReadOnlyList<string> Onsite(ProductHomeSnapshot home, string placeId)
        => home?.Place(placeId)?.OccupantPersonIds ?? (IReadOnlyList<string>)new string[0];

    private static int CompareWorkplaces(ManagementWorkplaceRow a, ManagementWorkplaceRow b)
    {
        int byKind = a.Kind.CompareTo(b.Kind);
        if (byKind != 0) return byKind;
        int byBuilding = string.CompareOrdinal(a.BuildingInstanceId, b.BuildingInstanceId);
        if (byBuilding != 0) return byBuilding;
        int bySlot = string.CompareOrdinal(a.FunctionSlotId, b.FunctionSlotId);
        return bySlot != 0 ? bySlot : string.CompareOrdinal(a.WorkplaceId, b.WorkplaceId);
    }
}
