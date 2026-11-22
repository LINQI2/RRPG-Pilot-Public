using System.Collections.Generic;
using System.Linq;
using RRPG.StaticConfig;

/// <summary>Composes existing queries only. Does not initialize, synchronize or write any authority.</summary>
public static class ProductHomeReadAdapter
{
    public static ProductHomeSnapshot Read()
    {
        var people=new List<HomePersonSnapshot>();
        var places=new List<HomePlaceSnapshot>();
        var diagnostics=new List<string>();
        var source=VillageRuntimeBridge.SimulationPersonSource;
        if (source==null || VillageRuntimeBridge.Time==null || VillageRuntimeBridge.IntradayWork==null)
            return new ProductHomeSnapshot(people,places,new[]{"Village source not ready"});
        foreach (var person in VillagePersonAvailability.Resolve(VillageRuntimeBridge.BuildPersonInputs(source),VillageRuntimeBridge.ExpeditionPersonIds))
        {
            var actual=VillageRuntimeBridge.QueryIntradayPresence(person.PersonId);
            var assignment=VillageRuntimeBridge.Assignments?.GetWorkplaceId(person.PersonId);
            var def=VillageRuntimeBridge.Assignments?.GetWorkplaceDef(assignment);
            var character=source.GetCharacterByPersonId(person.PersonId);
            string profession=character==null ? "—" : character.ActiveProfessionProgress?.ProfessionId ?? "未形成职业";
            string occupancy=VillageRuntimeBridge.GetPersonOccupancyReason(person.PersonId);
            bool away=person.Availability!=VillageAvailability.InVillage || actual.State=="离村 / 不可用";
            people.Add(new HomePersonSnapshot(person.PersonId,person.DisplayName,profession,actual.State,actual.PlaceId,
                actual.DestinationPlaceId,assignment,def?.DisplayName ?? assignment ?? "无职责",person.Availability,
                occupancy,actual.State,away));
        }
        var registry=VillageRuntimeBridge.World;
        if (registry!=null)
            foreach (var place in registry.Places)
            {
                var def=place.Kind==VillagePlaceKind.Workplace ? VillageRuntimeBridge.Assignments?.GetWorkplaceDef(place.PlaceId) : null;
                var building=place.Kind==VillagePlaceKind.Construction ? VillageRuntimeBridge.Buildings.Find(place.SourceRef) : null;
                var construction=building==null ? null : VillageRuntimeBridge.Projects?.GetActiveProject(building.ConstructionProjectId);
                // A registered Workplace / Construction Place that belongs to a real Building instance is the same entity.
                // Keep the place projection for management / presence, but let the Building view own the single world visual.
                string linkedBuildingId=LinkedBuildingId(place);
                places.Add(new HomePlaceSnapshot(place.PlaceId,place.DisplayName,place.Kind.ToString(),place.SourceRef,
                    def==null ? (place.Kind==VillagePlaceKind.Construction ? 2 : (int?)null) : def.Capacity,
                    def==null ? construction?.ParticipantPersonIds ?? new List<string>() : VillageRuntimeBridge.Assignments.GetOccupantPersonIds(place.PlaceId),
                    people.Where(p=>!p.Away && p.CurrentPlaceId==place.PlaceId).Select(p=>p.PersonId),
                    place.Kind==VillagePlaceKind.ProjectSite ? "已完成痕迹" : place.Kind==VillagePlaceKind.Construction ? "施工中" : "岗位",false,
                    linkedBuildingId,linkedBuildingId==null,
                    // A 档：只有真实世界痕迹（已完成项目现场）保留独立可见实体；
                    // 旧静态岗位等未绑定真实 Building 的记录不生成可见占位建筑。
                    linkedBuildingId==null && place.Kind==VillagePlaceKind.ProjectSite));
            }

        // Snapshot referenced facts before synthesising Home aliases, so housing identity can be resolved once.
        var buildings=new List<HomeBuildingSnapshot>();
        BuildingRegistry definitions=null;
        if(RRPG.StaticConfig.StaticConfigRegistry.TryGetBuildings(out definitions,out var buildingDiagnostic))
            foreach(var b in VillageRuntimeBridge.Buildings.All)
            {
                if(!definitions.TryGetById(b.BuildingDefinitionId,out var d)) {diagnostics.Add("Unknown BuildingDefinitionId: "+b.BuildingDefinitionId);continue;}
                buildings.Add(new HomeBuildingSnapshot(b.InstanceId,b.BuildingDefinitionId,d.DisplayName,b.WorldX,b.WorldY,d.FootprintWidth,d.FootprintDepth,b.LifecycleState));
            }
        else diagnostics.Add(buildingDiagnostic);

        // Housing alias contract (pure display; no new persistent field / DTO):
        //   - anchors are real Housing Building instances in LifecycleState.Completed only; an
        //     UnderConstruction house is never used as an occupancy anchor;
        //   - anchor order is the stable numeric instance ordinal (fallback ordinal string), so a newly
        //     allocated (higher-numbered) house appends instead of reshuffling the existing ones;
        //   - person -> anchor is the persisted PersonId's deterministic hash modulo the anchor count.
        //     It does not depend on the resolved people list order, being away / leaving the village,
        //     assignments or non-housing buildings; Continue restores the same PersonId and the same
        //     completed housing set, so the same alias comes back.
        //   - limitation: opening / completing another Housing instance changes the anchor count (the
        //     modulo space), so some aliases may relocate. That is the only relocation source here; a
        //     persisted housing assignment would require a save DTO change, which this package must not do.
        var housing=buildings.Where(b=>b.LifecycleState==BuildingLifecycleState.Completed && IsHousing(definitions,b.DefinitionId))
            .OrderBy(b=>b.InstanceId,HousingInstanceOrder).ToList();
        var homeByPerson=new Dictionary<string,string>(System.StringComparer.Ordinal);
        if(housing.Count>0)
            foreach(var person in people)
                if(!string.IsNullOrEmpty(person.PersonId))
                    homeByPerson[person.PersonId]=housing[(int)(StablePersonHash(person.PersonId)%(uint)housing.Count)].InstanceId;

        // These are snapshots of referenced facts, not additions to VillageWorldRegistry.
        var referenced=people.SelectMany(p=>new[]{p.CurrentPlaceId,p.DestinationPlaceId,VillageRuntimeBridge.ReceptionPolicy.KnownHome(p.PersonId)})
            .Concat(VillageRuntimeBridge.Projects==null ? new string[0] : VillageRuntimeBridge.Projects.ActiveProjects.Select(p=>VillageRuntimeBridge.ProjectDestination(p.ProjectId)))
            .Where(id=>!string.IsNullOrEmpty(id)).Distinct().OrderBy(id=>id,System.StringComparer.Ordinal);
        foreach (string id in referenced)
        {
            if (places.Any(p=>p.PlaceId==id)) continue;
            var project=VillageRuntimeBridge.Projects?.GetActiveProject(id);
            var def=project==null ? null : VillageRuntimeBridge.Projects.GetProjectDef(project.ProjectDefId);
            bool home=id.StartsWith("Home:",System.StringComparison.Ordinal);
            string name=def?.DisplayName ?? (home ? "住所 · "+(people.FirstOrDefault(p=>"Home:"+p.PersonId==id)?.DisplayName ?? id) : id);
            if(home)
            {
                string personId=id.Substring("Home:".Length);
                if(homeByPerson.TryGetValue(personId,out string houseId))
                {
                    // Same Housing Building identity as the Building projection: one real house, one visual.
                    places.Add(new HomePlaceSnapshot(id,name,"Home","Housing instance:"+houseId,null,
                        new List<string>(),people.Where(p=>!p.Away && p.CurrentPlaceId==id).Select(p=>p.PersonId),"真实住房实例",false,
                        houseId,false));
                    continue;
                }
            }
            places.Add(new HomePlaceSnapshot(id,name,project!=null ? "ActiveProject" : home ? "Home" : "ActivityPlace",
                "Actual Presence / Destination",def==null ? (int?)null : def.ParticipantCapacity,
                project==null ? new List<string>() : project.ParticipantPersonIds,
                people.Where(p=>!p.Away && p.CurrentPlaceId==id).Select(p=>p.PersonId),"只读地点投影",true));
            diagnostics.Add("Unregistered presentation anchor: "+id);
        }
        return new ProductHomeSnapshot(people,places,diagnostics,buildings);
    }

    private static string LinkedBuildingId(VillagePlaceRecord place)
    {
        if(place==null || string.IsNullOrEmpty(place.PlaceId)) return null;
        if(place.Kind==VillagePlaceKind.Workplace)
        {
            var building=VillageRuntimeBridge.Buildings?.All.FirstOrDefault(b=>
                place.PlaceId.StartsWith("workplace:"+b.InstanceId+":",System.StringComparison.Ordinal));
            return building?.InstanceId;
        }
        if(place.Kind==VillagePlaceKind.Construction)
        {
            var building=VillageRuntimeBridge.Buildings?.Find(place.SourceRef);
            return building?.InstanceId;
        }
        return null;
    }

    private static readonly IComparer<string> HousingInstanceOrder = Comparer<string>.Create(CompareInstanceIds);

    private static int CompareInstanceIds(string left,string right)
    {
        bool hasLeft=TryInstanceOrdinal(left,out int leftOrdinal);
        bool hasRight=TryInstanceOrdinal(right,out int rightOrdinal);
        if(hasLeft && hasRight && leftOrdinal!=rightOrdinal) return leftOrdinal.CompareTo(rightOrdinal);
        return System.StringComparer.Ordinal.Compare(left,right);
    }

    private static bool TryInstanceOrdinal(string instanceId,out int ordinal)
    {
        ordinal=0;
        if(string.IsNullOrEmpty(instanceId)) return false;
        int start=instanceId.Length;
        while(start>0 && char.IsDigit(instanceId[start-1])) start--;
        return start<instanceId.Length && int.TryParse(instanceId.Substring(start),out ordinal);
    }

    /// <summary>
    /// Deterministic FNV-1a over the persisted PersonId (UTF-16 code units) — never string.GetHashCode,
    /// which is randomized per process. Only used for a pure display alias choice.
    /// </summary>
    private static uint StablePersonHash(string personId)
    {
        const uint Offset=2166136261u, Prime=16777619u;
        uint hash=Offset;
        if(personId==null) return hash;
        foreach(char c in personId)
        {
            hash=(hash^(uint)(c&0xFF))*Prime;
            hash=(hash^(uint)(c>>8))*Prime;
        }
        return hash;
    }

    private static bool IsHousing(BuildingRegistry definitions,string definitionId)
    {
        return definitions!=null && definitions.TryGetById(definitionId,out var definition)
            && string.Equals(definition.Category,"Housing",System.StringComparison.OrdinalIgnoreCase);
    }
}
