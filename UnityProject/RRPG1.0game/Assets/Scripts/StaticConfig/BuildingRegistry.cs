using System;
using System.Collections.Generic;
namespace RRPG.StaticConfig
{
    public sealed class BuildingRegistry
    {
        private readonly Dictionary<string,BuildingDefinition> _byId=new Dictionary<string,BuildingDefinition>(StringComparer.Ordinal);
        public IReadOnlyList<BuildingDefinition> All { get; }
        public IReadOnlyList<BuildingFunctionBinding> FunctionBindings { get; }
        public BuildingRegistry(IEnumerable<BuildingDefinition> definitions, IEnumerable<BuildingFunctionBinding> bindings=null)
        {
            var all=new List<BuildingDefinition>();
            if(definitions!=null) foreach(var d in definitions)
            {
                if(d==null || string.IsNullOrWhiteSpace(d.BuildingDefinitionId) || string.IsNullOrWhiteSpace(d.DisplayName)
                    || string.IsNullOrWhiteSpace(d.Category) || d.FootprintWidth<=0 || d.FootprintDepth<=0)
                    throw new InvalidOperationException("Invalid BuildingDefinition / footprint");
                if(_byId.ContainsKey(d.BuildingDefinitionId)) throw new InvalidOperationException("Duplicate BuildingDefinitionId: "+d.BuildingDefinitionId);
                _byId.Add(d.BuildingDefinitionId,d);all.Add(d);
            }
            All=all.AsReadOnly();
            var functions=new List<BuildingFunctionBinding>();
            var slots=new HashSet<string>(StringComparer.Ordinal);
            if(bindings!=null) foreach(var f in bindings)
            {
                if(f==null || !_byId.ContainsKey(f.BuildingDefinitionId) || string.IsNullOrWhiteSpace(f.FunctionSlotId)
                    || f.FunctionSlotId.Contains(":") || f.FunctionType!="Workplace"
                    || !BuildingFunctionBinding.IsKnownProfile(f.FunctionProfileId)
                    || !slots.Add(f.BuildingDefinitionId+":"+f.FunctionSlotId))
                    throw new InvalidOperationException("Invalid BuildingFunctionBinding / profile / duplicate slot");
                functions.Add(f);
            }
            FunctionBindings=functions.AsReadOnly();
        }
        public bool TryGetById(string id,out BuildingDefinition definition)
        { definition=null;return id!=null && _byId.TryGetValue(id,out definition); }

        /// <summary>
        /// 正式产品建造入口（Home Catalog / Choose / Confirm / TryPlaceBuilding）是否允许该定义。
        /// 工坊 / 铁匠铺保留历史定义、美术与旧实例 Load，但不再作为正式可建造内容；
        /// 旧实例恢复路径不得调用本判定。
        /// </summary>
        public static bool IsFormallyBuildable(string definitionId)
        {
            return !string.IsNullOrEmpty(definitionId)
                && definitionId!="building_workshop" && definitionId!="building_smithy";
        }
    }

    [Serializable]
    public sealed class BuildingFunctionBinding
    {
        public string BuildingDefinitionId, FunctionSlotId, FunctionType, FunctionProfileId;
        // C2 references existing prototype Workplace profiles. Capacity stays in VillageWorkplaceDef.
        public static bool IsKnownProfile(string id) => id=="ws_farm_01" || id=="ws_kitchen_01" || id=="ws_test_01" || id=="ws_test_02";
    }
}
