using System;
using System.Collections.Generic;
using System.Linq;
using RRPG.StaticConfig;

/// <summary>
/// V17-R3-B 窄产品命令 gateway（Assembly-CSharp）。
///
/// 每次调用都从当前源重查：人物存在 / 在村可用 / 世界不忙 / 目标建筑已完成且 Function slot 真实注册，
/// 全部通过后才调用既有 Bridge（TryAssignPerson / RemovePersonAssignment /
/// TryAddConstructionWorker / TryRemoveConstructionWorker / SaveSystem.Save）。
///
/// 边界：
///   - 满员 / AlreadyAssigned / PersonCurrentlyUnavailable 等领域诊断原样保留，不在 UI 层改写规则；
///   - 不自动 Remove→Assign、不做原子换岗、撤销职不重建原记录；
///   - 施工加入保留原主要职责，不改 Primary Assignment；
///   - 保存只复用既有 bool 回执并遵守 CanSaveWorld，不重写保存 Ownership、不制造自动存档。
/// </summary>
public static class ManagementCommandGateway
{
    /// <summary>派工：目标必须是已完成建筑实例上真实注册的 Function 岗位。</summary>
    public static ManagementCommandResult AssignToWorkplace(string personId, string workplaceId)
    {
        var blocked = CheckPerson(personId, workplaceId, out _, out string name);
        if (blocked != null) return blocked;
        if (string.IsNullOrEmpty(workplaceId)) return Fail(ManagementCommandOutcome.TargetUnknown, personId, workplaceId, "未指定目标岗位。");
        if (!workplaceId.StartsWith("workplace:", StringComparison.Ordinal))
            return Fail(ManagementCommandOutcome.StaticTargetNotAssignable, personId, workplaceId,
                "既有静态岗位（兼容记录）不是正式新增派工目标：请先撤职，再从已完工建筑的功能岗位派工。");

        if (!TrySplit(workplaceId, out string instanceId, out string slot))
            return Fail(ManagementCommandOutcome.TargetUnknown, personId, workplaceId, "岗位标识无法解析：" + workplaceId);
        var building = VillageRuntimeBridge.Buildings?.Find(instanceId);
        if (building == null)
            return Fail(ManagementCommandOutcome.TargetUnknown, personId, workplaceId, "目标建筑实例不存在：" + instanceId);
        if (building.LifecycleState != BuildingLifecycleState.Completed)
            return Fail(ManagementCommandOutcome.TargetNotCompleted, personId, workplaceId,
                "目标建筑仍在施工：未完工建筑不提供可派工岗位。");

        if (!StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, out string definitionDiagnostic))
            return Fail(ManagementCommandOutcome.NotReady, personId, workplaceId, "建筑功能定义不可用：" + definitionDiagnostic);
        var binding = definitions.FunctionBindings.FirstOrDefault(f =>
            f.BuildingDefinitionId == building.BuildingDefinitionId && f.FunctionSlotId == slot);
        if (binding == null)
            return Fail(ManagementCommandOutcome.TargetNotRegistered, personId, workplaceId,
                "该岗位未在 Building FunctionBindings 注册：" + workplaceId);

        var assignments = VillageRuntimeBridge.Assignments;
        var registered = assignments != null ? assignments.GetWorkplaceDef(workplaceId) : null;
        if (registered == null)
            return Fail(ManagementCommandOutcome.TargetNotRegistered, personId, workplaceId,
                "该岗位尚未真实注册，不能派工：" + workplaceId);

        var result = VillageRuntimeBridge.TryAssignPerson(personId, workplaceId);
        if (result == VillageAssignmentResult.Success)
            return Ok(personId, workplaceId, name + " 已绑定到岗位：" + registered.DisplayName);
        return Fail(ManagementCommandOutcome.DomainRejected, personId, workplaceId, AssignMessage(result), result.ToString());
    }

    /// <summary>撤职：移除当前主要职责（静态兼容记录同样可撤）；不重新指派、不恢复累计统计。</summary>
    public static ManagementCommandResult RemoveAssignment(string personId)
    {
        var blocked = CheckPerson(personId, null, out _, out string name);
        if (blocked != null) return blocked;
        string existing = VillageRuntimeBridge.GetPersonWorkplaceId(personId);
        if (!VillageRuntimeBridge.RemovePersonAssignment(personId))
            return Fail(ManagementCommandOutcome.DomainRejected, personId, existing ?? string.Empty,
                "撤职未生效：该人物当前没有主要职责，或世界暂不可写。", "RemoveAssignmentRejected");
        return Ok(personId, existing ?? string.Empty, name + " 的主要职责已移除" +
            (string.IsNullOrEmpty(existing) ? "。" : "（" + existing + "）。"));
    }

    /// <summary>加入施工：只接受施工中的建筑实例；原 Primary Assignment 保留。</summary>
    public static ManagementCommandResult AddConstructionWorker(string buildingId, string personId)
    {
        var blocked = CheckPerson(personId, buildingId, out _, out string name);
        if (blocked != null) return blocked;
        if (string.IsNullOrEmpty(buildingId))
            return Fail(ManagementCommandOutcome.TargetUnknown, personId, buildingId, "未指定施工建筑。");
        var building = VillageRuntimeBridge.Buildings?.Find(buildingId);
        if (building == null)
            return Fail(ManagementCommandOutcome.TargetUnknown, personId, buildingId, "目标建筑实例不存在：" + buildingId);
        if (building.LifecycleState != BuildingLifecycleState.UnderConstruction)
            return Fail(ManagementCommandOutcome.TargetAlreadyCompleted, personId, buildingId,
                "该建筑已完工：施工工人只用于施工中建筑，派工请走功能岗位。");

        var result = VillageRuntimeBridge.TryAddConstructionWorker(buildingId, personId);
        if (result == VillageProjectResult.Success)
            return Ok(personId, buildingId, name + " 已加入施工（原主要职责保留）。");
        return Fail(ManagementCommandOutcome.DomainRejected, personId, buildingId, ProjectMessage(result), result.ToString());
    }

    /// <summary>撤出施工：只移除施工参与，不改 Primary Assignment。</summary>
    public static ManagementCommandResult RemoveConstructionWorker(string buildingId, string personId)
    {
        var blocked = CheckPerson(personId, buildingId, out _, out string name);
        if (blocked != null) return blocked;
        if (string.IsNullOrEmpty(buildingId))
            return Fail(ManagementCommandOutcome.TargetUnknown, personId, buildingId, "未指定施工建筑。");
        var building = VillageRuntimeBridge.Buildings?.Find(buildingId);
        if (building == null)
            return Fail(ManagementCommandOutcome.TargetUnknown, personId, buildingId, "目标建筑实例不存在：" + buildingId);
        if (building.LifecycleState != BuildingLifecycleState.UnderConstruction)
            return Fail(ManagementCommandOutcome.TargetAlreadyCompleted, personId, buildingId,
                "该建筑已完工，没有可撤出的施工工人。");

        var result = VillageRuntimeBridge.TryRemoveConstructionWorker(buildingId, personId);
        if (result == VillageProjectResult.Success)
            return Ok(personId, buildingId, name + " 已撤出施工（原主要职责保留）。");
        return Fail(ManagementCommandOutcome.DomainRejected, personId, buildingId, ProjectMessage(result), result.ToString());
    }

    /// <summary>显式保存：只复用既有 SaveSystem bool 回执；失败不声称已保存、不自动回滚。</summary>
    public static ManagementCommandResult SaveWorld()
    {
        var source = VillageRuntimeBridge.SimulationPersonSource;
        if (source == null)
            return Fail(ManagementCommandOutcome.NotReady, string.Empty, string.Empty,
                "村庄世界未就绪：缺少人物来源，未执行保存。");
        if (!VillageRuntimeBridge.CanSaveWorld)
            return Fail(ManagementCommandOutcome.WorldBusy, string.Empty, string.Empty,
                "世界正在推进或仍有远征未完成，当前不能保存。");
        if (SaveSystem.Save(source))
            return Ok(string.Empty, string.Empty, "保存成功。");
        return Fail(ManagementCommandOutcome.SaveRejected, string.Empty, string.Empty,
            "保存未成功：当前内存状态保留，可稍后重试。", "SaveReturnedFalse");
    }

    /// <summary>人物前置重查（存在 / 在村可用 / 世界不忙）；通过时返回 null。</summary>
    private static ManagementCommandResult CheckPerson(string personId, string targetId, out PlayerCollection source,
        out string displayName)
    {
        source = null;
        displayName = null;
        if (string.IsNullOrEmpty(personId))
            return Fail(ManagementCommandOutcome.PersonMissing, personId, targetId, "未指定人物。");

        source = VillageRuntimeBridge.SimulationPersonSource;
        if (source == null)
            return Fail(ManagementCommandOutcome.NotReady, personId, targetId, "村庄世界未就绪：缺少人物来源。");
        var person = source.FindPersonByPersonId(personId);
        if (person == null)
            return Fail(ManagementCommandOutcome.PersonMissing, personId, targetId,
                "人物不存在或已离开当前世界：" + personId);
        displayName = string.IsNullOrEmpty(person.DisplayName) ? personId : person.DisplayName;

        var views = VillagePersonAvailability.Resolve(VillageRuntimeBridge.BuildPersonInputs(source),
            VillageRuntimeBridge.ExpeditionPersonIds);
        var view = views.FirstOrDefault(v => v.PersonId == personId);
        if (view == null || view.Availability != VillageAvailability.InVillage)
            return Fail(ManagementCommandOutcome.PersonNotInVillage, personId, targetId,
                view != null && view.Availability == VillageAvailability.Expedition
                    ? "该人物正在远征中，当前不能安排或撤职。"
                    : "该人物当前不可用（暂不可调度）。");

        if (!VillageRuntimeBridge.CanSaveWorld)
            return Fail(ManagementCommandOutcome.WorldBusy, personId, targetId, WorldBusyMessage());
        return null;
    }

    private static string WorldBusyMessage()
    {
        string diagnostic = VillageRuntimeBridge.WorldAdvanceDiagnostic;
        return string.IsNullOrEmpty(diagnostic)
            ? "世界正在推进 / 返程结算或仍有远征未完成，命令暂不可用。"
            : "世界正在推进 / 返程结算或仍有远征未完成，命令暂不可用（" + diagnostic + "）。";
    }

    private static bool TrySplit(string workplaceId, out string instanceId, out string slot)
    {
        const string prefix = "workplace:";
        instanceId = null;
        slot = null;
        if (!workplaceId.StartsWith(prefix, StringComparison.Ordinal)) return false;
        string rest = workplaceId.Substring(prefix.Length);
        int index = rest.LastIndexOf(':');
        if (index <= 0 || index >= rest.Length - 1) return false;
        instanceId = rest.Substring(0, index);
        slot = rest.Substring(index + 1);
        return instanceId.Length > 0 && slot.Length > 0;
    }

    private static string AssignMessage(VillageAssignmentResult result)
    {
        switch (result)
        {
            case VillageAssignmentResult.AlreadyAssigned:
                return "该人物已有主要职责：请先显式撤职，再做新派工（不自动换岗）。";
            case VillageAssignmentResult.CapacityFull:
                return "目标岗位已满员，本次未写入。";
            case VillageAssignmentResult.UnknownWorkplace:
                return "目标岗位未注册。";
            case VillageAssignmentResult.InvalidPersonId:
                return "人物标识非法，本次未写入。";
            default:
                return "领域拒绝了本次派工。";
        }
    }

    private static string ProjectMessage(VillageProjectResult result)
    {
        switch (result)
        {
            case VillageProjectResult.PersonAlreadyInActiveProject:
                return "该人物已在另一个施工项目中（同一人物不能同时投入两个项目）。";
            case VillageProjectResult.CapacityFull:
                return "该施工项目工人已满员，本次未写入。";
            case VillageProjectResult.PersonCurrentlyUnavailable:
                return "该人物当前不可调度，本次未写入。";
            case VillageProjectResult.PersonInRestWindow:
                return "该人物处于恢复窗口，暂不能加入施工。";
            case VillageProjectResult.UnknownProject:
                return "施工项目不存在或已结束。";
            case VillageProjectResult.InvalidPersonId:
                return "人物标识非法，本次未写入。";
            default:
                return "领域拒绝了本次施工操作。";
        }
    }

    private static ManagementCommandResult Ok(string personId, string targetId, string message)
        => new ManagementCommandResult(ManagementCommandOutcome.Success, personId, targetId, message);

    private static ManagementCommandResult Fail(ManagementCommandOutcome outcome, string personId, string targetId,
        string message, string domainDiagnostic = "")
        => new ManagementCommandResult(outcome, personId, targetId, message, domainDiagnostic);
}
