using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace RRPG.Building.Tests
{
public sealed class BuildingConstructionProductPlayTests
{
    private const BindingFlags S=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static;
    private const BindingFlags I=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
    private static Type T(string name)=>Type.GetType(name+", Assembly-CSharp",true);
    private static object Static(string type,string method,params object[] args)
    {
        var candidates=T(type).GetMethods(S).Where(m=>m.Name==method && m.GetParameters().Length==args.Length).ToList();
        Assert.AreEqual(1,candidates.Count,"静态方法必须唯一："+type+"."+method+"/"+args.Length);
        return candidates[0].Invoke(null,args);
    }
    private static object Call(object value,string method,params object[] args)
    {
        var candidates=value.GetType().GetMethods(I).Where(m=>m.Name==method && m.GetParameters().Length==args.Length).ToList();
        Assert.AreEqual(1,candidates.Count,"方法必须唯一："+method+" @ "+value.GetType().FullName);
        return candidates[0].Invoke(value,args);
    }
    private static object Get(object value,string name)=>value.GetType().GetProperty(name,I).GetValue(value);
    private static object Find(string type)=>UnityEngine.Object.FindObjectOfType(T(type),true);
    private VillageTime Clock=>(VillageTime)T("VillageRuntimeBridge").GetField("Time",S).GetValue(null);
    private VillageProjectSystem Projects=>(VillageProjectSystem)T("VillageRuntimeBridge").GetField("Projects",S).GetValue(null);
    private VillageAssignmentSystem Assignments=>(VillageAssignmentSystem)T("VillageRuntimeBridge").GetField("Assignments",S).GetValue(null);
    private object _host,_player;
    private SaveSlotGuard _guard;
    private ProductHomeSnapshot Snapshot=>(ProductHomeSnapshot)Get(_host,"Snapshot");
    private HomeSelectionContext Selection=>(HomeSelectionContext)Get(_host,"Selection");
    private string SavePath=>(string)Static("SaveSystem","GetSavePath");
    private VillageSaveData Save=>(VillageSaveData)Static("VillageRuntimeBridge","BuildSaveData");
    private Component BuildingView(string id)=>(Component)((IDictionary)Get(_host,"BuildingViews"))[id];

    // ==================== 用户单槽持久保护（先捕获 / 异常恢复 / finally 恢复 / 未完成阻断） ====================

    private static string RepoRoot()=>Path.GetFullPath(Path.Combine(Application.dataPath,"..","..",".."));

    private sealed class SaveSlotGuard
    {
        private const string ManifestName="manifest.json";
        private const string BackupName="original-save.bin";
        [Serializable] private sealed class ManifestStatus { public string runId; public string status; }
        private string _path,_runId,_runDir,_binPath,_captureUtc,_beforeHash;
        private bool _existed,_captured,_restored;
        private byte[] _bytes;
        public string RunId=>_runId;
        public bool Restored=>_restored;

        private static string ProtectionRoot()
            =>Path.Combine(RepoRoot(),"Development/StudioRuns/Initialization-20261027/save-guard".Replace('/',Path.DirectorySeparatorChar));

        public void Capture()
        {
            Assert.IsFalse(_captured,"SaveSlotGuard 同一实例只能 Capture 一次");
            BlockOnUnfinishedRuns();
            _path=(string)Static("SaveSystem","GetSavePath");
            Assert.IsFalse(string.IsNullOrEmpty(_path),"SaveSystem.GetSavePath 必须返回真实路径");
            _captureUtc=DateTime.UtcNow.ToString("O");
            _runId=DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
            string root=ProtectionRoot();
            _runDir=Path.Combine(root,_runId);
            for(int i=2;Directory.Exists(_runDir);i++) _runDir=Path.Combine(root,_runId+"-"+i);
            _runId=Path.GetFileName(_runDir);
            Directory.CreateDirectory(_runDir);
            _binPath=Path.Combine(_runDir,BackupName);
            _existed=File.Exists(_path);
            _bytes=_existed?File.ReadAllBytes(_path):null;
            _beforeHash=_existed?HashBytes(_bytes):"absent";
            if(_existed)
            {
                using(var stream=new FileStream(_binPath,FileMode.CreateNew,FileAccess.Write,FileShare.None)) stream.Write(_bytes,0,_bytes.Length);
                byte[] readback=File.ReadAllBytes(_binPath);
                Assert.AreEqual(_bytes.LongLength,readback.LongLength,"备份字节数回读不一致");
                Assert.AreEqual(_beforeHash,HashBytes(readback),"备份回读 SHA256 不一致");
                Assert.AreEqual(_beforeHash,HashFile(_path),"捕获后当前档已被改写，拒绝继续");
            }
            WriteManifest(_existed?"captured":"absent",null);
            _captured=true;
            Debug.Log("[BuildingConstructionProductPlayTests][SAVE-CAPTURE] runId="+_runId+" existed="+_existed
                +" bytes="+(_bytes==null?0:_bytes.LongLength)+" sha256="+_beforeHash);
        }

        public void Restore()
        {
            Assert.IsTrue(_captured,"SaveSlotGuard.Capture 未成功完成，禁止 Restore");
            if(_restored)
            {
                bool stillOk=File.Exists(_path)==_existed && (!_existed || HashFile(_path)==_beforeHash);
                if(stillOk) return;
            }
            byte[] source=null;
            if(_existed)
            {
                Assert.IsTrue(File.Exists(_binPath),"本次持久备份缺失，拒绝恢复："+_binPath);
                source=File.ReadAllBytes(_binPath);
                Assert.AreEqual(_beforeHash,HashBytes(source),"本次备份回读 SHA256 不一致，拒绝写回");
            }
            try
            {
                if(_existed) File.WriteAllBytes(_path,source);
                else if(File.Exists(_path)) File.Delete(_path);
                bool exists=File.Exists(_path);
                string afterHash=exists?HashFile(_path):"absent";
                bool ok=exists==_existed && (!_existed || afterHash==_beforeHash);
                _restored=ok;
                WriteManifest(ok?"restored":"restore-failed",ok?null:"sha256 mismatch after write");
                Assert.IsTrue(ok,"单槽存档恢复失败（backup/manifest 已保留）：期望 existed="+_existed+" sha256="+_beforeHash+"，实际 existed="+exists+" sha256="+afterHash);
            }
            catch(Exception ex)
            {
                if(!_restored) WriteManifest("restore-failed",ex.GetType().Name+": "+ex.Message);
                throw;
            }
        }

        private static void BlockOnUnfinishedRuns()
        {
            string root=ProtectionRoot();
            if(!Directory.Exists(root)) return;
            var unfinished=new List<string>();
            foreach(string dir in Directory.GetDirectories(root))
            {
                string manifest=Path.Combine(dir,ManifestName);
                if(!File.Exists(manifest)) { unfinished.Add(Path.GetFileName(dir)+"(no-manifest)"); continue; }
                ManifestStatus parsed=null;
                try { parsed=JsonUtility.FromJson<ManifestStatus>(File.ReadAllText(manifest)); }
                catch(Exception) { }
                string status=parsed==null?null:parsed.status;
                if(status=="restored" || status=="protective-snapshot") continue;
                unfinished.Add(Path.GetFileName(dir)+"("+(status??"unreadable")+")");
            }
            Assert.AreEqual(0,unfinished.Count,
                "存在未完成的存档保护 run，已阻止新测试以免覆盖当前档；请人工核对后再继续："
                +string.Join("; ",unfinished)+" @ "+root);
        }

        private void WriteManifest(string status,string error)
        {
            var sb=new StringBuilder();
            sb.Append("{\n");
            sb.Append("  \"runId\": \""+JsonText(_runId)+"\",\n");
            sb.Append("  \"status\": \""+JsonText(status)+"\",\n");
            sb.Append("  \"captureUtc\": \""+JsonText(_captureUtc)+"\",\n");
            sb.Append("  \"updatedUtc\": \""+JsonText(DateTime.UtcNow.ToString("O"))+"\",\n");
            sb.Append("  \"sourcePath\": \""+JsonText(_path)+"\",\n");
            sb.Append("  \"sourceExisted\": "+(_existed?"true":"false")+",\n");
            sb.Append("  \"sourceBytes\": "+(_bytes==null?0:_bytes.LongLength)+",\n");
            sb.Append("  \"sourceSha256\": \""+JsonText(_beforeHash)+"\",\n");
            sb.Append("  \"backupFile\": "+(_existed?"\""+BackupName+"\"":"null")+",\n");
            sb.Append("  \"backupBytes\": "+(_existed && File.Exists(_binPath)?new FileInfo(_binPath).Length:0)+",\n");
            sb.Append("  \"backupSha256\": "+(_existed?"\""+JsonText(_beforeHash)+"\"":"null")+",\n");
            sb.Append("  \"readbackVerified\": "+(_existed?"true":"false")+",\n");
            sb.Append("  \"restored\": "+(_restored?"true":"false")+",\n");
            sb.Append("  \"lastError\": "+(error==null?"null":"\""+JsonText(error)+"\"")+",\n");
            sb.Append("  \"capturedBy\": \"BuildingConstructionProductPlayTests.SaveSlotGuard (test-only helper)\",\n");
            sb.Append("  \"note\": \"只允许用本 runID 的 original-save.bin 恢复；进程 abort 时 TearDown 不执行，备份留待人工取证。\"\n");
            sb.Append("}\n");
            File.WriteAllText(Path.Combine(_runDir,ManifestName),sb.ToString());
        }

        private static string JsonText(string value)=>value==null?string.Empty:value.Replace("\\","\\\\").Replace("\"","\\\"");
    }

    private static string HashBytes(byte[] bytes)
    {
        using(var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-",string.Empty).ToLowerInvariant();
    }

    private static string HashFile(string path)
    {
        using(var sha=SHA256.Create()) using(var stream=File.OpenRead(path))
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-",string.Empty).ToLowerInvariant();
    }

    // ==================== Lifecycle ====================

    [UnitySetUp] public IEnumerator Setup()
    {
        _guard=new SaveSlotGuard();
        _guard.Capture();
        // UnitySetUp 失败时 NUnit 不保证执行普通 [TearDown]：Capture 后必须在本夹具内恢复。
        bool completed=false;
        try
        {
            Static("VillageRuntimeBridge","ResetForTests");SceneManager.LoadScene("SampleScene");yield return null;yield return null;
            _player=Static("SaveSystem","NewGame");Assert.IsTrue((bool)Static("SaveSystem","Save",_player));
            Call(Find("MainMenuUI"),"OnContinue");yield return null;yield return null;
            _player=Get(Find("TownUI"),"PlayerData");Clock.Pause();_host=Find("ProductHomeWorldHost");Call(_host,"RefreshNow");
            completed=true;
        }
        finally
        {
            if(!completed) RestoreAfterFailedSetup();
        }
    }

    private void RestoreAfterFailedSetup()
    {
        if(_guard==null) return;
        try { _guard.Restore(); }
        catch(Exception ex)
        {
            Debug.LogWarning("[BuildingConstructionProductPlayTests] UnitySetUp 失败后的存档恢复失败（不掩盖原始异常）："+ex.GetType().Name+": "+ex.Message);
        }
    }

    [TearDown] public void Cleanup()
    {
        try { _guard?.Restore(); }
        finally { Static("VillageRuntimeBridge","ResetForTests"); }
    }

    // ==================== helpers ====================

    /// <summary>默认生成世界的真实 Completed 实例岗位（从运行时建筑找 ID，不写死 ws_*）。</summary>
    private string InstanceWorkplace(string definitionId)
    {
        foreach(var b in Snapshot.Buildings)
        {
            if(b.DefinitionId!=definitionId) continue;
            if(b.LifecycleState!=BuildingLifecycleState.Completed) continue;
            return "workplace:"+b.InstanceId+":main";
        }
        Assert.Fail("正式默认世界必须存在 Completed "+definitionId+" 实例");
        return null;
    }

    private string Place(float x)
    {
        var build=Get(Get(_host,"Shell"),"Build");Call(build,"OpenCatalog");Assert.IsTrue((bool)Call(build,"Choose","building_farm"));
        Call(build,"SetCandidate",new Vector2(x,-4));Assert.IsTrue((bool)Call(build,"Confirm"));Call(build,"Cancel");Call(_host,"RefreshNow");return Selection.Id;
    }
    private void WorkerButton(string building,string person)
    {
        Selection.SelectBuilding(building);Call(_host,"RefreshNow");
        var inspector=(Component)Get(_host,"Inspector");var row=inspector.GetComponentsInChildren<Transform>(true).LastOrDefault(t=>t.name=="Worker_"+person && t.gameObject.activeInHierarchy);
        Assert.IsNotNull(row,"Player worker command must be present");
        var button=row.GetComponents<Component>().Single(c=>c.GetType().FullName=="UnityEngine.UI.Button");
        Call(Get(button,"onClick"),"Invoke");Call(_host,"RefreshNow");
    }
    private void Advance(long minute)
    {
        var result=Clock.AdvanceTo(new WorldInstant(minute));int retries=0;
        while(result.Outcome==WorldAdvanceOutcome.Yielded && retries++<100) result=Clock.ResumeAdvance(Clock.AdvanceSession.RequestId);
        Assert.AreEqual(WorldAdvanceOutcome.Completed,result.Outcome,result.Diagnostic);Call(_host,"RefreshNow");
    }

    [UnityTest] public IEnumerator HomeWorkersActualLaborTwoIndependentFunctionsAndContinue()
    {
        // 新正式实体语义：默认世界已有生成实例（数量可调）；施工在默认世界上增量创建两个真实工地。
        int baselineBuildings=Snapshot.Buildings.Count;
        string farmWp=InstanceWorkplace("building_farm"),kitchenWp=InstanceWorkplace("building_kitchen");
        Assert.AreNotEqual(farmWp,kitchenWp,"默认 Farm / Kitchen 必须是两个独立实例岗位");
        string a=Place(4),b=Place(8),p1=Snapshot.Persons[0].PersonId,p2=Snapshot.Persons[1].PersonId;
        Assert.AreEqual(baselineBuildings+2,Snapshot.Buildings.Count,"真实建造必须在默认生成世界之上增量创建两个实例");
        Assert.AreEqual(VillageAssignmentResult.Success,(VillageAssignmentResult)Static("VillageRuntimeBridge","TryAssignPerson",p1,farmWp),
            "默认实例 Farm 岗位必须可派工");
        Assert.AreEqual(VillageAssignmentResult.Success,(VillageAssignmentResult)Static("VillageRuntimeBridge","TryAssignPerson",p2,kitchenWp),
            "默认实例 Kitchen 岗位必须可派工");
        WorkerButton(a,p1);WorkerButton(b,p2);var firstView=BuildingView(a);
        Assert.AreEqual(farmWp,Assignments.GetWorkplaceId(p1),"施工参与不得改变默认实例 Primary Assignment");
        Advance(479);Assert.AreEqual(0,Projects.ActiveProjects[0].LaborRemainder);Advance(480);Advance(481);
        Assert.AreEqual("construction:"+a,Snapshot.Person(p1).CurrentPlaceId);Assert.Greater(Projects.ActiveProjects[0].LaborRemainder,0);
        WorkerButton(a,p1);Assert.AreEqual(0,Projects.ActiveProjects.Single(p=>p.ParticipantPersonIds.Contains(p2)==false).ParticipantPersonIds.Count);
        WorkerButton(a,p1);Advance(17280);
        Assert.AreSame(firstView,BuildingView(a));Assert.AreEqual(BuildingLifecycleState.Completed,Snapshot.Building(a).LifecycleState);
        Assert.AreEqual(BuildingLifecycleState.Completed,Snapshot.Building(b).LifecycleState);
        Static("VillageRuntimeBridge","RemovePersonAssignment",p1);Static("VillageRuntimeBridge","RemovePersonAssignment",p2);
        WorkerButton(a,p1);WorkerButton(b,p2);string wa="workplace:"+a+":main",wb="workplace:"+b+":main";
        Assert.AreEqual(wa,Assignments.GetWorkplaceId(p1));Assert.AreEqual(wb,Assignments.GetWorkplaceId(p2));
        Advance(17761);Assert.AreEqual(wa,Snapshot.Person(p1).CurrentPlaceId);Assert.AreEqual(wb,Snapshot.Person(p2).CurrentPlaceId);
        Assert.IsTrue((bool)Static("SaveSystem","Save",_player));string json=File.ReadAllText(SavePath);
        StringAssert.DoesNotContain("\"PlaceId\": \"construction:",json);StringAssert.DoesNotContain("FunctionProfileId",json);
        Call(Find("MainMenuUI"),"OnContinue");yield return null;yield return null;Clock.Pause();Call(_host,"RefreshNow");
        Assert.AreEqual(wa,Assignments.GetWorkplaceId(p1));Assert.AreEqual(wb,Assignments.GetWorkplaceId(p2));
        Assert.AreEqual(baselineBuildings+2,Snapshot.Buildings.Count,"Continue 必须恢复同一批真实建筑实例（相对默认基线）");
        Assert.AreEqual(1,Assignments.GetOccupantCount(wa));Assert.AreEqual(1,Assignments.GetOccupantCount(wb));
    }

    [UnityTest] public IEnumerator ZeroWorkersAndLateLoadFailureRetainsWholeConstructionWorld()
    {
        int baselineBuildings=Snapshot.Buildings.Count;
        int baselineWorkplaces=Assignments.WorkplaceDefs.Count;
        string a=Place(4),person=Snapshot.Persons[0].PersonId;WorkerButton(a,person);Advance(600);WorkerButton(a,person);
        Assert.IsTrue((bool)Static("SaveSystem","Save",_player));string zero=File.ReadAllText(SavePath);
        Assert.IsNotNull(Static("SaveSystem","Load"));Assert.AreEqual(0,Projects.ActiveProjects[0].ParticipantPersonIds.Count);
        Place(8);string before=JsonUtility.ToJson(Save);
        string bad=System.Text.RegularExpressions.Regex.Replace(zero,"\"LastOpportunityMinute\": [0-9]+","\"LastOpportunityMinute\": 999999");
        Assert.IsTrue(zero!=bad,"Future marker injection must change the serialized input");File.WriteAllText(SavePath,bad);LogAssert.Expect(LogType.Warning,"[SaveSystem] Load rejected: future opportunity marker.");
        Assert.IsNull(Static("SaveSystem","Load"));Assert.AreEqual(before,JsonUtility.ToJson(Save));
        Assert.IsNotNull(Static("SaveSystem","NewGame"));
        // 正式默认 NewGame 语义（数量来自生成配置，不写死；静态基座为空）：
        Assert.AreEqual(baselineBuildings,Save.Buildings.Instances.Count,"NewGame 必须回到默认生成建筑数量（不累加测试内新建实例）");
        Assert.AreEqual(baselineWorkplaces,Assignments.WorkplaceDefs.Count,"NewGame 必须回到默认生成岗位目录");
        Assert.IsFalse(Assignments.WorkplaceDefs.Any(w=>!w.WorkplaceId.StartsWith("workplace:",StringComparison.Ordinal)),
            "正式默认世界不得重新注册静态兼容岗位");
        Assert.IsEmpty(Projects.ActiveProjects);
        yield return null;
    }
}
}
