using RRPG.StaticConfig;
using System;
using System.Collections.Generic;
using System.Linq;
public static partial class VillageRuntimeBridge
{
    private static bool _stagingBuildingLoad;
    internal sealed class BuildingRestorePlan
    {
        internal BuildingRuntime Buildings;
        internal VillageAssignmentSystem Assignments;
        internal VillageProjectSystem Projects;
        internal VillageWorldRegistry World;
        internal VillageIntradayWork Work;
        internal List<VillageResourceWorkplaceDef> ResourceWorkplaces;
    }
    public static string ConstructionPlaceId(string buildingId) => "construction:"+buildingId;
    public static string BuildingWorkplaceId(string buildingId,string slot) => "workplace:"+buildingId+":"+slot;
    private static BuildingInstance BuildingForProject(string projectId) => Buildings.All.FirstOrDefault(b=>b.ConstructionProjectId==projectId);
    public static string ProjectDestination(string projectId) => BuildingForProject(projectId) is BuildingInstance b ? ConstructionPlaceId(b.InstanceId) : projectId;

    private static List<VillageWorkplaceDef> FunctionWorkplaces(BuildingInstance b,BuildingRegistry definitions)
    {
        var result=new List<VillageWorkplaceDef>();
        foreach(var f in definitions.FunctionBindings.Where(f=>f.BuildingDefinitionId==b.BuildingDefinitionId))
        {
            var profile=_testWorkplaces.FirstOrDefault(d=>d.WorkplaceId==f.FunctionProfileId);
            if(profile==null) throw new InvalidOperationException("Unknown Workplace profile: "+f.FunctionProfileId);
            result.Add(new VillageWorkplaceDef { WorkplaceId=BuildingWorkplaceId(b.InstanceId,f.FunctionSlotId),
                DisplayName=definitions.All.First(d=>d.BuildingDefinitionId==b.BuildingDefinitionId).DisplayName+" · "+b.InstanceId,
                Capacity=profile.Capacity,TestContributionPerDay=profile.TestContributionPerDay });
        }
        return result;
    }
    internal static bool IsInstanceWorkplace(string workplaceId) =>
        workplaceId!=null && workplaceId.StartsWith("workplace:",StringComparison.Ordinal);

    /// <summary>
    /// 存档可证明的静态岗位引用 = 存档中真实出现过的非实例 WorkplaceId
    /// （职责记录 / 当日批次 / 当日劳务证据）。这是正式实例存档恢复静态兼容目录的唯一来源；
    /// 不读取当前会话的 Assignments，也不从 NewGame 的静态目录反推。
    /// </summary>
    private static SortedSet<string> ReferencedStaticWorkplaceIds(VillageSaveData save)
    {
        var ids=new SortedSet<string>(StringComparer.Ordinal);
        foreach(var record in save?.Assignments?.Records ?? new List<VillageAssignmentSaveData>())
            if(record!=null && !string.IsNullOrEmpty(record.WorkplaceId) && !IsInstanceWorkplace(record.WorkplaceId))
                ids.Add(record.WorkplaceId);
        foreach(var batch in save?.IntradayWork?.Batches ?? new List<VillageWorkBatchSaveData>())
            if(batch!=null && !string.IsNullOrEmpty(batch.WorkplaceId) && !IsInstanceWorkplace(batch.WorkplaceId))
                ids.Add(batch.WorkplaceId);
        foreach(var evidence in save?.IntradayWork?.Evidence ?? new List<VillageWorkEvidenceSaveData>())
            if(evidence!=null && !string.IsNullOrEmpty(evidence.WorkplaceId) && !IsInstanceWorkplace(evidence.WorkplaceId))
                ids.Add(evidence.WorkplaceId);
        return ids;
    }

    private static VillageResourceWorkplaceDef CopyResourceWorkplace(VillageResourceWorkplaceDef profile)
    {
        return new VillageResourceWorkplaceDef
        {
            WorkplaceId=profile.WorkplaceId, Role=profile.Role, PerWorkerDailyOutput=profile.PerWorkerDailyOutput,
            InputIngredientsPerUnit=profile.InputIngredientsPerUnit, OutputFoodPerUnit=profile.OutputFoodPerUnit,
            ProcessingUnitsPerWorkerPerDay=profile.ProcessingUnitsPerWorkerPerDay
        };
    }

    private static List<VillageResourceWorkplaceDef> FunctionResourceWorkplaces(BuildingInstance b,BuildingRegistry definitions)
    {
        var result=new List<VillageResourceWorkplaceDef>();
        foreach(var f in definitions.FunctionBindings.Where(f=>f.BuildingDefinitionId==b.BuildingDefinitionId))
        {
            var profile=_resourceWorkplaceProfiles.FirstOrDefault(d=>d.WorkplaceId==f.FunctionProfileId);
            if(profile==null) continue; // A legal non-resource Workplace remains a normal Workplace.
            result.Add(new VillageResourceWorkplaceDef { WorkplaceId=BuildingWorkplaceId(b.InstanceId,f.FunctionSlotId),
                Role=profile.Role,PerWorkerDailyOutput=profile.PerWorkerDailyOutput,
                InputIngredientsPerUnit=profile.InputIngredientsPerUnit,OutputFoodPerUnit=profile.OutputFoodPerUnit,
                ProcessingUnitsPerWorkerPerDay=profile.ProcessingUnitsPerWorkerPerDay });
        }
        return result;
    }
    public static VillageProjectResult TryAddConstructionWorker(string buildingId,string personId)
    {
        if(!GuardWorldWrite() || !CanSaveWorld) return VillageProjectResult.PersonCurrentlyUnavailable;
        EnsureInitialized(null);
        var b=Buildings.Find(buildingId);
        if(b==null || b.LifecycleState!=BuildingLifecycleState.UnderConstruction) return VillageProjectResult.UnknownProject;
        if(!PresentWorkPersons().Contains(personId)) return VillageProjectResult.PersonCurrentlyUnavailable;
        if(!StaticConfigRegistry.TryGetBuildings(out var defs,out _)) return VillageProjectResult.UnknownProjectDef;
        FunctionWorkplaces(b,defs); // Validate completion inputs before accepting a commitment.
        bool created=false;
        if(string.IsNullOrEmpty(b.ConstructionProjectId))
        {
            var result=Projects.TryCreateProject("prj_test_01",out var p);
            if(result!=VillageProjectResult.Success) return result;
            b.ConstructionProjectId=p.ProjectId;created=true;
        }
        var add=TryAddProjectParticipant(b.ConstructionProjectId,personId);
        if(add!=VillageProjectResult.Success && created)
        {Projects.TryRemoveActiveProject(b.ConstructionProjectId);b.ConstructionProjectId=null;}
        World.EnsureConstructionPlace(b.InstanceId,defs.All.First(d=>d.BuildingDefinitionId==b.BuildingDefinitionId).DisplayName);
        return add;
    }
    public static VillageProjectResult TryRemoveConstructionWorker(string buildingId,string personId)
    {
        if(!GuardWorldWrite() || !CanSaveWorld) return VillageProjectResult.PersonCurrentlyUnavailable;
        var b=Buildings.Find(buildingId);
        if(b==null || string.IsNullOrEmpty(b.ConstructionProjectId)) return VillageProjectResult.UnknownProject;
        bool wasWorker=Projects.GetActiveProject(b.ConstructionProjectId)?.ParticipantPersonIds.Contains(personId)==true;
        var result=Projects.TryRemoveParticipant(b.ConstructionProjectId,personId);
        if(result==VillageProjectResult.Success && wasWorker) {IntradayWork.Leave(personId);RefreshWorkIntent();}
        return result;
    }
    private static bool FinalizeConstruction(VillageProject p)
    {
        var b=BuildingForProject(p.ProjectId);
        if(b==null) return false;
        if(!StaticConfigRegistry.TryGetBuildings(out var definitions,out string diagnostic)) throw new InvalidOperationException(diagnostic);
        var functions=FunctionWorkplaces(b,definitions);
        var resourceFunctions=FunctionResourceWorkplaces(b,definitions);
        foreach(var f in functions)
        {
            var existing=Assignments.GetWorkplaceDef(f.WorkplaceId);
            if(existing!=null && (existing.Capacity!=f.Capacity || existing.TestContributionPerDay!=f.TestContributionPerDay))
                throw new InvalidOperationException("Conflicting building workplace: "+f.WorkplaceId);
        }
        foreach(var f in functions) {Assignments.TryRegisterWorkplace(f);World.EnsureWorkplacePlace(f.WorkplaceId,f.DisplayName);}
        b.LifecycleState=BuildingLifecycleState.Completed;b.ConstructionProjectId=null;
        foreach(var f in resourceFunctions)
            if(!_resourceWorkplaceDefs.Any(d=>d.WorkplaceId==f.WorkplaceId)) _resourceWorkplaceDefs.Add(f);
        World.RetireConstructionPlace(b.InstanceId);
        foreach(var id in p.ParticipantPersonIds) IntradayWork.Leave(id);
        return true;
    }

    internal static bool TryPrepareBuildingWorld(VillageSaveData save,BuildingRuntime buildings,out BuildingRestorePlan plan,out string diagnostic)
    {
        plan=null;diagnostic="";
        try
        {
            BuildingRegistry defs=null;
            if(buildings.All.Count>0 && !StaticConfigRegistry.TryGetBuildings(out defs,out diagnostic)) return false;
            var projects=new VillageProjectSystem(_testProjects);projects.Restore(save?.Projects);
            // Restore source = the save, never the session. A save that carries real Building instances is a
            // formal world: its static Workplace catalogue is exactly the set of non-instance WorkplaceIds the
            // save actually references (assignment records / work batches / work evidence), so a cold start and
            // an in-session Load produce the same catalogue. A legacy save without Buildings keeps the historical
            // full static compatibility catalogue because its world predates instance workplaces.
            bool hasSavedBuildings=buildings.All.Count>0;
            var referencedStatic=ReferencedStaticWorkplaceIds(save);
            var baseWorkplaces=new List<VillageWorkplaceDef>();
            if(!hasSavedBuildings) baseWorkplaces.AddRange(_testWorkplaces);
            else
                foreach(var workplaceId in referencedStatic)
                {
                    var legacyProfile=_testWorkplaces.FirstOrDefault(d=>d.WorkplaceId==workplaceId);
                    if(legacyProfile!=null) baseWorkplaces.Add(legacyProfile);
                }
            var assignments=new VillageAssignmentSystem(baseWorkplaces);
            var world=new VillageWorldRegistry();world.EnsureWorkplacePlaces(assignments.WorkplaceDefs);world.Restore(save?.World);
            var bound=new HashSet<string>(StringComparer.Ordinal);
            // Resource responsibility follows the same source: a formal instance save only re-derives resource
            // roles for the static workplaces it references, plus the roles of its restored Buildings; it never
            // re-seeds the whole static resource catalogue. Legacy no-Buildings saves keep the historical roles.
            var resources=new List<VillageResourceWorkplaceDef>();
            if(!hasSavedBuildings) resources.AddRange(VillageTestResourceWorkplaces.CreateDefaults());
            else
                foreach(var def in assignments.WorkplaceDefs)
                {
                    var profile=_resourceWorkplaceProfiles.FirstOrDefault(p=>p.WorkplaceId==def.WorkplaceId);
                    if(profile!=null) resources.Add(CopyResourceWorkplace(profile));
                }
            foreach(var b in buildings.All)
            {
                if(!string.IsNullOrEmpty(b.ConstructionProjectId))
                {
                    if(!bound.Add(b.ConstructionProjectId)) throw new InvalidOperationException("Duplicate ConstructionProject binding");
                    var source=save?.Projects?.ActiveProjects?.SingleOrDefault(p=>p.ProjectId==b.ConstructionProjectId);
                    if(source==null || source.ProjectDefId!="prj_test_01" || source.Progress<0 || source.LaborRemainder<0 || source.LaborRemainder>=480
                        || source.ParticipantPersonIds==null || source.ParticipantPersonIds.Count>2
                        || source.ParticipantPersonIds.Any(string.IsNullOrEmpty) || source.ParticipantPersonIds.Distinct().Count()!=source.ParticipantPersonIds.Count)
                        throw new InvalidOperationException("Invalid ConstructionProject binding / state");
                    var active=projects.GetActiveProject(b.ConstructionProjectId);
                    if(active==null)
                    {
                        if(!projects.IsProjectCompleted(b.ConstructionProjectId)) throw new InvalidOperationException("Missing ConstructionProject");
                        b.LifecycleState=BuildingLifecycleState.Completed;b.ConstructionProjectId=null;
                    }
                    else if(!active.ParticipantPersonIds.SequenceEqual(source.ParticipantPersonIds)) throw new InvalidOperationException("Conflicting construction workers");
                }
                if(b.LifecycleState==BuildingLifecycleState.Completed)
                {
                    foreach(var f in FunctionWorkplaces(b,defs)) {assignments.TryRegisterWorkplace(f);world.EnsureWorkplacePlace(f.WorkplaceId,f.DisplayName);}
                    resources.AddRange(FunctionResourceWorkplaces(b,defs));
                }
                else world.EnsureConstructionPlace(b.InstanceId,defs.All.First(d=>d.BuildingDefinitionId==b.BuildingDefinitionId).DisplayName);
            }
            // Dynamic references must never be silently discarded by the legacy restore policy.
            foreach(var r in save?.Assignments?.Records ?? new List<VillageAssignmentSaveData>())
                if(r!=null && r.WorkplaceId!=null && r.WorkplaceId.StartsWith("workplace:",StringComparison.Ordinal)
                    && assignments.GetWorkplaceDef(r.WorkplaceId)==null) throw new InvalidOperationException("Unknown runtime Workplace assignment: "+r.WorkplaceId);
            assignments.Restore(save?.Assignments);
            foreach(var r in save?.Assignments?.Records ?? new List<VillageAssignmentSaveData>())
                if(r?.WorkplaceId!=null && r.WorkplaceId.StartsWith("workplace:",StringComparison.Ordinal)
                    && assignments.GetWorkplaceId(r.PersonId)!=r.WorkplaceId)
                    throw new InvalidOperationException("Runtime Workplace assignment could not be restored: "+r.PersonId);
            foreach(var d in assignments.WorkplaceDefs)
                if(d.WorkplaceId.StartsWith("workplace:",StringComparison.Ordinal) && assignments.GetOccupantCount(d.WorkplaceId)>d.Capacity)
                    throw new InvalidOperationException("Runtime Workplace capacity exceeded: "+d.WorkplaceId);
            foreach(var batch in save?.IntradayWork?.Batches ?? new List<VillageWorkBatchSaveData>())
                if(batch.WorkplaceId.StartsWith("workplace:",StringComparison.Ordinal))
                {
                    var resource=resources.FirstOrDefault(d=>d.WorkplaceId==batch.WorkplaceId);
                    if(resource==null) throw new InvalidOperationException("Unknown runtime Resource Workplace batch: "+batch.WorkplaceId);
                    if(resource.Role==VillageResourceRole.Farm && batch.InputPaid
                        || resource.Role==VillageResourceRole.Kitchen && batch.LaborNumerator>0 && !batch.InputPaid)
                        throw new InvalidOperationException("Invalid runtime Resource Workplace batch: "+batch.WorkplaceId);
                }
            var work=new VillageIntradayWork();
            if(!work.Restore(save?.IntradayWork)) throw new InvalidOperationException("Invalid IntradayWork");
            plan=new BuildingRestorePlan {Buildings=buildings,Assignments=assignments,Projects=projects,World=world,Work=work,ResourceWorkplaces=resources};return true;
        }
        catch(Exception ex) {diagnostic=ex.Message;return false;}
    }
    internal static void CommitBuildingWorld(BuildingRestorePlan plan)
    {Buildings=plan.Buildings;Assignments=plan.Assignments;Projects=plan.Projects;World=plan.World;IntradayWork=plan.Work;
        _resourceWorkplaceDefs.Clear();_resourceWorkplaceDefs.AddRange(plan.ResourceWorkplaces);
        _observedProjectCompletions=Projects.CompletedProjects.Count;}
    public static BuildingRuntime Buildings { get; private set; }=new BuildingRuntime();
    public static bool TryPlaceBuilding(string definitionId,float x,float y,out BuildingInstance instance,out string diagnostic)
    {
        instance=null;
        if(!GuardWorldWrite() || !CanSaveWorld) { diagnostic="村庄正在结算或远征中";return false; }
        if(!StaticConfigRegistry.TryGetBuildings(out var definitions,out diagnostic)) return false;
        if(!BuildingRegistry.IsFormallyBuildable(definitionId))
        { diagnostic="该建筑已从正式建造目录下线（历史定义 / 旧实例仍保留）："+definitionId; return false; }
        if(!Buildings.TryPlace(definitions,definitionId,x,y,out instance,out diagnostic)) return false;
        EnsureInitialized(null);World.EnsureConstructionPlace(instance.InstanceId,definitions.All.First(d=>d.BuildingDefinitionId==definitionId).DisplayName);return true;
    }
    internal static bool TryPrepareBuildings(VillageBuildingsSaveData data,out BuildingRuntime prepared,out string diagnostic)
    {
        BuildingRegistry definitions=null;
        if(data?.Instances!=null && data.Instances.Count>0 && !StaticConfigRegistry.TryGetBuildings(out definitions,out diagnostic))
        {prepared=null;return false;}
        return BuildingRuntime.TryPrepareRestore(data,definitions,out prepared,out diagnostic);
    }
    internal static void CommitBuildingsRestore(BuildingRuntime prepared) { Buildings=prepared; }

    /// <summary>
    /// 正式 NewGame 初始世界候选（未提交）。与 <see cref="BuildingRestorePlan"/> 同属一个 Building/Workplace 权威，
    /// 只服务「从显式 InitialWorldConfiguration 创建真实实体」，不进入 Continue / EnsureInitialized / 测试重置。
    /// </summary>
    internal sealed class NewGameWorldPlan
    {
        internal BuildingRuntime Buildings;
        internal VillageAssignmentSystem Assignments;
        internal VillageProjectSystem Projects;
        internal VillageWorldRegistry World;
        internal VillageIntradayWork Work;
        internal List<VillageResourceWorkplaceDef> ResourceWorkplaces;
        internal int Ingredients;
        internal int Food;
    }

    /// <summary>
    /// 校验并构建 NewGame 候选世界（**纯准备，不写任何运行时权威**）：
    ///   - 在候选 BuildingRuntime 上按配置创建 Completed 实例（全部成功才返回 true）；
    ///   - 复用既有 FunctionWorkplaces / FunctionResourceWorkplaces 注册实例岗位与资源岗位；
    ///   - 静态岗位 / 静态资源岗位基座为**空**：正式新世界不再保留默认静态岗位；
    ///     旧档需要的静态定义只由 <see cref="TryPrepareBuildingWorld"/> 按存档真实引用恢复；
    ///   - 任一建筑非法（未知定义 / 越界 / 重叠 / 冲突岗位）→ 返回 false，调用方不得提交。
    /// 失败时调用方的当前世界完全不变（零半状态）。
    /// </summary>
    internal static bool TryPrepareNewGameWorld(InitialWorldConfiguration config,out NewGameWorldPlan plan,out string diagnostic)
    {
        plan=null;diagnostic="";
        try
        {
            BuildingRegistry definitions=null;
            if(config==null) {diagnostic="初始世界配置为 null";return false;}
            if(config.Buildings.Count>0 && !StaticConfigRegistry.TryGetBuildings(out definitions,out diagnostic)) return false;

            var buildings=new BuildingRuntime();
            foreach(var b in config.Buildings)
            {
                if(!buildings.TryAddInitialCompleted(definitions,b.BuildingDefinitionId,b.WorldX,b.WorldY,out _,out diagnostic))
                {diagnostic="初始建筑（"+b.BuildingDefinitionId+" @ "+b.WorldX+","+b.WorldY+"）非法："+diagnostic;return false;}
            }

            // 正式新世界只登记由配置建筑派生的实例岗位；ws_farm_01 / ws_kitchen_01 / ws_test_* 静态
            // Test 岗位及其装饰 Place 不再进入 NewGame（旧档 Load 仍保留静态定义，见 TryPrepareBuildingWorld）。
            // 实例岗位的资源角色继续复用既有 VillageResourceWorkplaceDef profile。
            var assignments=new VillageAssignmentSystem(new List<VillageWorkplaceDef>());
            var world=new VillageWorldRegistry();
            world.EnsureWorkplacePlaces(assignments.WorkplaceDefs);
            var resources=new List<VillageResourceWorkplaceDef>();
            foreach(var b in buildings.All)
            {
                foreach(var f in FunctionWorkplaces(b,definitions))
                {
                    if(!assignments.TryRegisterWorkplace(f))
                        throw new InvalidOperationException("Conflicting initial Workplace: "+f.WorkplaceId);
                    world.EnsureWorkplacePlace(f.WorkplaceId,f.DisplayName);
                }
                resources.AddRange(FunctionResourceWorkplaces(b,definitions));
            }

            plan=new NewGameWorldPlan
            {
                Buildings=buildings,
                Assignments=assignments,
                Projects=new VillageProjectSystem(_testProjects),
                World=world,
                Work=new VillageIntradayWork(),
                ResourceWorkplaces=resources,
                Ingredients=config.InitialIngredients,
                Food=config.InitialFood
            };
            return true;
        }
        catch(Exception ex)
        {
            diagnostic=ex.Message;
            plan=null;
            return false;
        }
    }

    /// <summary>
    /// 提交 NewGame 候选世界（唯一调用方 = SaveSystem.NewGame）。一次性替换 Buildings / Assignments /
    /// Projects / World / IntradayWork / 资源岗位派生表，并把资源恢复为配置初值；不创建第二套保存/恢复路径。
    /// </summary>
    internal static void CommitNewGameWorld(NewGameWorldPlan plan)
    {
        if(plan==null) throw new InvalidOperationException("NewGame world plan 为 null");
        Buildings=plan.Buildings;
        Assignments=plan.Assignments;
        Projects=plan.Projects;
        World=plan.World;
        IntradayWork=plan.Work;
        _resourceWorkplaceDefs.Clear();_resourceWorkplaceDefs.AddRange(plan.ResourceWorkplaces);
        _observedProjectCompletions=0;
        if(Resources==null) Resources=new VillageResourceState();
        Resources.Restore(new VillageResourceSaveData {Ingredients=plan.Ingredients,Food=plan.Food});
    }

    internal static void ResetBuildingsForNewGame()
    {
        Buildings=new BuildingRuntime();
        _resourceWorkplaceDefs.Clear();_resourceWorkplaceDefs.AddRange(VillageTestResourceWorkplaces.CreateDefaults());
        if(Assignments==null) return;
        Assignments=new VillageAssignmentSystem(_testWorkplaces);
        Projects=new VillageProjectSystem(_testProjects);
        World=new VillageWorldRegistry();World.EnsureWorkplacePlaces(Assignments.WorkplaceDefs);
        IntradayWork?.Restore(null);_observedProjectCompletions=0;
    }
}
