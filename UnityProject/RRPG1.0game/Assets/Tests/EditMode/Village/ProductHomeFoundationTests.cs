using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace RRPG.ProductHome.Tests
{
public sealed class ProductHomeFoundationTests
{
    private const BindingFlags F=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static;
    private object _player;
    private string _id;
    private const BindingFlags I=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
    private static Type T(string name)=>Type.GetType(name+", Assembly-CSharp",true);
    /// <summary>
    /// 按“名称 + 实参个数”唯一定位静态方法：初始化包为 SaveSystem 增加了 NewGame(InitialWorldConfiguration) 重载，
    /// 旧的 GetMethod(name, flags) 会在 NewGame()/NewGame(config) 之间抛 AmbiguousMatchException（与
    /// ManagementShellPlayTests 同一根因）。无法唯一确定时以显式断言失败，不臆选重载、不改产品 API。
    /// </summary>
    private static object Call(string type,string method,params object[] args)
    {
        int count=args==null?0:args.Length;
        var candidates=T(type).GetMethods(F).Where(m=>m.Name==method && m.GetParameters().Length==count).ToList();
        Assert.AreEqual(1,candidates.Count,"静态方法必须唯一："+type+"."+method+"/"+count);
        return candidates[0].Invoke(null,args);
    }
    private static ProductHomeSnapshot Read()=>(ProductHomeSnapshot)Call("ProductHomeReadAdapter","Read");
    private static VillageTime Clock=>(VillageTime)T("VillageRuntimeBridge").GetField("Time",F).GetValue(null);
    private static string SaveState()=>JsonUtility.ToJson(Call("VillageRuntimeBridge","BuildSaveData"))+JsonUtility.ToJson(Call("ActivityRuntimeBridge","BuildSaveData"));
    [SetUp] public void Setup()
    {
        Call("VillageRuntimeBridge","ResetForTests");
        _player=Call("SaveSystem","NewGame");Call("VillageRuntimeBridge","EnsureInitialized",new object[]{null});
        Call("VillageRuntimeBridge","SetSimulationPersonSource",_player);Clock.Pause();
        _id=Read().Persons[0].PersonId;
    }
    [TearDown] public void Cleanup()=>Call("VillageRuntimeBridge","ResetForTests");
    [Test] public void IdentityAndNameAreFromLongTermSource()
    {
        var input=(System.Collections.IEnumerable)Call("VillageRuntimeBridge","BuildPersonInputs",_player);
        foreach (VillagePersonInput p in input) { Assert.AreEqual(p.DisplayName,Read().Person(p.PersonId).DisplayName); }
        Assert.IsTrue(Read().Persons.Select(p=>p.PersonId).Distinct().Count()==Read().Persons.Count);
    }
    [Test] public void AssignmentCapacityAndReservedOccupancyUseExistingAuthority()
    {
        string farm=FarmWorkplace();
        Assert.AreEqual(VillageAssignmentResult.Success,Call("VillageRuntimeBridge","TryAssignPerson",_id,farm));
        var snapshot=Read();var assignments=(VillageAssignmentSystem)T("VillageRuntimeBridge").GetField("Assignments",F).GetValue(null);
        Assert.AreEqual(assignments.GetWorkplaceId(_id),snapshot.Person(_id).AssignmentId);
        Assert.AreEqual(assignments.GetWorkplaceDef(farm).Capacity,snapshot.Place(farm).Capacity);
        CollectionAssert.AreEquivalent(assignments.GetOccupantPersonIds(farm),snapshot.Place(farm).AssignedPersonIds);
        Assert.IsFalse(snapshot.Place(farm).OccupantPersonIds.Contains(_id),"Assignment alone is not arrival");
    }
    private string FarmWorkplace()
    {
        object runtime=T("VillageRuntimeBridge").GetProperty("Buildings",F).GetValue(null);
        var all=(System.Collections.IEnumerable)runtime.GetType().GetProperty("All",I).GetValue(runtime,null);
        foreach(object b in all)
        {
            if((string)b.GetType().GetProperty("BuildingDefinitionId",I).GetValue(b,null)!="building_farm") continue;
            return "workplace:"+(string)b.GetType().GetProperty("InstanceId",I).GetValue(b,null)+":main";
        }
        Assert.Fail("默认新世界必须包含真实 Farm 实例");
        return null;
    }
    [Test] public void SnapshotAndSelectionDoNotWriteRuntimeOrRegistry()
    {
        string before=SaveState();var registry=(VillageWorldRegistry)T("VillageRuntimeBridge").GetField("World",F).GetValue(null);
        int count=registry.Places.Count;
        var s=new HomeSelectionContext();s.SelectPerson(_id);s.SelectPlace(Read().Places[0].PlaceId);s.Clear();
        for(int i=0;i<20;i++) Read();
        Assert.AreEqual(before,SaveState());Assert.AreEqual(count,registry.Places.Count);
    }
    [Test] public void CollectionsCannotBeMutatedThroughSnapshot()
    {
        var snapshot=Read();Assert.Throws<NotSupportedException>(()=>((System.Collections.Generic.IList<HomePersonSnapshot>)snapshot.Persons).Clear());
        Assert.Throws<NotSupportedException>(()=>((System.Collections.Generic.IList<string>)snapshot.Places[0].AssignedPersonIds).Add(_id));
    }
}
}
