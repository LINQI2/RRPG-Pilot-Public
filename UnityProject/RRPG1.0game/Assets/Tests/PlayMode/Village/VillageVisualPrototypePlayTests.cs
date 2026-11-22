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

namespace RRPG.ProductHome.Tests
{
public sealed class VillageVisualPrototypePlayTests
{
    // Long-term technical contracts only: profile/domain separation and stable PlaceId anchors.
    private const BindingFlags S=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static;
    private const BindingFlags I=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
    private static Type T(string n)=>Type.GetType(n+", Assembly-CSharp",true);
    private static object Static(string n,string m,params object[] a)
    {
        var candidates=T(n).GetMethods(S).Where(x=>x.Name==m && x.GetParameters().Length==a.Length).ToList();
        Assert.AreEqual(1,candidates.Count,"静态方法必须唯一："+n+"."+m+"/"+a.Length);
        return candidates[0].Invoke(null,a);
    }
    private static object Call(object o,string m,params object[] a)
    {
        var candidates=o.GetType().GetMethods(I).Where(x=>x.Name==m && x.GetParameters().Length==a.Length).ToList();
        Assert.AreEqual(1,candidates.Count,"方法必须唯一："+m+" @ "+o.GetType().FullName);
        return candidates[0].Invoke(o,a);
    }
    private static object Get(object o,string n)=>o.GetType().GetProperty(n,I).GetValue(o);
    private static object Find(string n)=>UnityEngine.Object.FindObjectOfType(T(n),true);
    private object _host,_player;
    private SaveSlotGuard _guard;
    private ProductHomeSnapshot Snapshot=>(ProductHomeSnapshot)Get(_host,"Snapshot");
    private VillageTime Clock=>(VillageTime)T("VillageRuntimeBridge").GetField("Time",S).GetValue(null);
    private string Domain()=>JsonUtility.ToJson(Static("SaveSystem","BuildSaveData",_player,Static("VillageRuntimeBridge","BuildSaveData")));

    // SavedAtUnixTime 是 SaveSystem.BuildSaveData 每次采样写入的墙钟元数据（DateTimeOffset.UtcNow），
    // 不属于模拟 / 世界时钟事实。表现切换只允许该字段不同，其余存档事实必须逐字节相等；
    // 世界时钟单独由 VillageTime.ToSaveData 比对，不把墙钟时间戳当成模拟推进。
    private sealed class DomainFacts
    {
        public string Facts;
        public long SavedAtUnixTime;
        public string WorldClock;
    }
    private DomainFacts CaptureDomain()
    {
        string json=Domain();
        const string key="\"SavedAtUnixTime\":";
        int keyIndex=json.IndexOf(key,StringComparison.Ordinal);
        Assert.GreaterOrEqual(keyIndex,0,"Domain 必须包含 SavedAtUnixTime 字段");
        int valueStart=keyIndex+key.Length;
        int valueEnd=json.IndexOf(',',valueStart);
        Assert.Greater(valueEnd,valueStart,"SavedAtUnixTime 后必须仍有存档字段，不能截断 Save");
        Assert.IsTrue(long.TryParse(json.Substring(valueStart,valueEnd-valueStart),out long stamp),
            "SavedAtUnixTime 必须是整数秒："+json.Substring(valueStart,valueEnd-valueStart));
        return new DomainFacts
        {
            // 只规范化这一个非模拟字段，其它每个字节都参与比较。
            Facts=json.Substring(0,valueStart)+"<wall-clock>"+json.Substring(valueEnd),
            SavedAtUnixTime=stamp,
            WorldClock=JsonUtility.ToJson(Clock.ToSaveData())
        };
    }
    private static void AssertDomainFactsUnchanged(string surface,DomainFacts expected,DomainFacts actual)
    {
        Assert.AreEqual(expected.Facts,actual.Facts,
            surface+"：表现变更不得改写除墙钟 SavedAtUnixTime 之外的任何模拟/存档事实");
        Assert.AreEqual(expected.WorldClock,actual.WorldClock,
            surface+"：Camera / selection / prototype 表现不得推进世界时钟");
        long now=DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Assert.LessOrEqual(Math.Abs(actual.SavedAtUnixTime-now),120,
            surface+"：SavedAtUnixTime 必须是当前墙钟秒，而不是世界时钟");
        Assert.LessOrEqual(Math.Abs(actual.SavedAtUnixTime-expected.SavedAtUnixTime),120,
            surface+"：两次 Domain 采样必须属于同一墙钟时刻附近");
        Debug.Log("[VillageVisualPrototypePlayTests][DOMAIN] "+surface+" savedAt="+actual.SavedAtUnixTime
            +"(prev="+expected.SavedAtUnixTime+") wallClockOnly=true worldClock="+actual.WorldClock);
    }

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
            Debug.Log("[VillageVisualPrototypePlayTests][SAVE-CAPTURE] runId="+_runId+" existed="+_existed
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
            sb.Append("  \"capturedBy\": \"VillageVisualPrototypePlayTests.SaveSlotGuard (test-only helper)\",\n");
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
            _player=Static("SaveSystem","NewGame");
            Assert.True((bool)Static("SaveSystem","Save",_player));Call(Find("MainMenuUI"),"OnContinue");yield return null;yield return null;
            _player=Get(Find("TownUI"),"PlayerData");Clock.Pause();_host=Find("ProductHomeWorldHost");Call(_host,"RefreshNow");
            Assert.False((bool)Get(_host,"VisualPrototypeEnabled"),"A whitebox remains the default");
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
            Debug.LogWarning("[VillageVisualPrototypePlayTests] UnitySetUp 失败后的存档恢复失败（不掩盖原始异常）："+ex.GetType().Name+": "+ex.Message);
        }
    }

    [TearDown] public void Cleanup()
    {
        try { _guard?.Restore(); }
        finally { Static("VillageRuntimeBridge","ResetForTests"); }
    }

    [UnityTest] public IEnumerator ProfilesPreserveEntitiesFootprintsSelectionAndDomain()
    {
        foreach(var pair in new[]{new{d="building_farm",x=-8f},new{d="building_house",x=0f}})
        {object[] args={pair.d,pair.x,-4f,null,null};Assert.True((bool)Static("VillageRuntimeBridge","TryPlaceBuilding",args),(string)args[4]);}
        Call(_host,"RefreshNow");var buildings=(IDictionary)Get(_host,"BuildingViews");var entities=buildings.Values.Cast<Component>().ToArray();
        var colliders=entities.Select(x=>x.GetComponent<BoxCollider2D>()).ToArray();var positions=entities.Select(x=>x.transform.position).ToArray();
        var sizes=colliders.Select(x=>x.size).ToArray();
        var persons=((IDictionary)Get(_host,"PersonViews")).Values.Cast<Component>().ToArray();
        var domain=CaptureDomain();Call(_host,"SetVisualPrototype",true);yield return null;
        AssertDomainFactsUnchanged("SetVisualPrototype(true)",domain,CaptureDomain());
        CollectionAssert.AreEquivalent(entities,((IDictionary)Get(_host,"BuildingViews")).Values);
        CollectionAssert.AreEquivalent(persons,((IDictionary)Get(_host,"PersonViews")).Values);
        for(int i=0;i<entities.Length;i++){Assert.AreEqual(positions[i],entities[i].transform.position);Assert.AreSame(colliders[i],entities[i].GetComponent<BoxCollider2D>());Assert.AreEqual(sizes[i],colliders[i].size);Assert.True((bool)Get(entities[i],"ResolvedSharedAsset"));}
        var person=(Component)((IDictionary)Get(_host,"PersonViews"))[Snapshot.Persons[0].PersonId];var selection=(HomeSelectionContext)Get(_host,"Selection");
        person.transform.position=entities[0].transform.position+new Vector3(0,1,0);Call(person,"RefreshSelection");
        Assert.Less((int)Get(person,"DepthOrder"),(int)Get(entities[0],"DepthOrder"));
        selection.SelectPerson(Snapshot.Persons[0].PersonId);Call(person,"RefreshSelection");Assert.Greater((int)Get(person,"DepthOrder"),(int)Get(entities[0],"DepthOrder"));
        Assert.True((bool)Get(person,"LabelVisible"));selection.Clear();Call(person,"RefreshSelection");
        person.transform.position=entities[0].transform.position-new Vector3(0,1,0);Call(person,"RefreshSelection");Assert.Greater((int)Get(person,"DepthOrder"),(int)Get(entities[0],"DepthOrder"));
        Call(_host,"SetVisualPrototype",false);AssertDomainFactsUnchanged("SetVisualPrototype(false)",domain,CaptureDomain());
        CollectionAssert.AreEquivalent(persons,((IDictionary)Get(_host,"PersonViews")).Values);
        foreach(var view in persons)Assert.True((bool)Get(view,"LabelVisible"));
        yield return null;
        var build=Get(Get(_host,"Shell"),"Build");Call(build,"OpenCatalog");Assert.True((bool)Call(build,"Choose","building_house"));
        Call(build,"SetCandidate",new Vector2(0,0));Assert.True((bool)Get(build,"PreviewValid"));
        Call(_host,"SetVisualPrototype",true);Assert.True((bool)Get(build,"PreviewValid"));AssertDomainFactsUnchanged("prototype preview",domain,CaptureDomain());
        Call(build,"SetCandidate",new Vector2(-8,-4));Assert.False((bool)Get(build,"PreviewValid"));
        Call(build,"Cancel");AssertDomainFactsUnchanged("build cancel",domain,CaptureDomain());
    }

    [UnityTest] public IEnumerator ActivityPlacesCannotReflowRegisteredWorkplaces()
    {
        Call(_host,"SetVisualPrototype",true);
        var actual=Snapshot;
        var stable=actual.Places.Where(p=>p.Kind!="ActivityPlace").ToArray();
        // 只在真实世界之上追加活动地点：people / diagnostics / buildings 原样保留，
        // 不构造裁剪的假世界（否则 Reconcile 会合法删除真实建筑视图）。
        Assert.IsNotEmpty(actual.Buildings,"默认世界必须包含真实 Building 快照");
        Assert.IsNotEmpty(actual.Persons,"默认世界必须包含真实人物快照");
        var baseline=new ProductHomeSnapshot(actual.Persons,stable,actual.Diagnostics,actual.Buildings);
        Assert.AreEqual(actual.Persons.Select(p=>p.PersonId).ToArray(),baseline.Persons.Select(p=>p.PersonId).ToArray(),"baseline 必须保留真实 people");
        Assert.AreEqual(actual.Diagnostics.ToArray(),baseline.Diagnostics.ToArray(),"baseline 必须保留真实 diagnostics");
        Assert.AreEqual(actual.Buildings.Select(b=>b.InstanceId).ToArray(),baseline.Buildings.Select(b=>b.InstanceId).ToArray(),"baseline 必须保留真实 buildings");
        Call(_host,"Reconcile",baseline);
        var places=(IDictionary)Get(_host,"PlaceViews");
        var buildings=(IDictionary)Get(_host,"BuildingViews");
        CollectionAssert.AreEquivalent(
            actual.Buildings.Select(b=>b.InstanceId).ToArray(),
            ((IDictionary)Get(_host,"BuildingViews")).Keys.Cast<object>().Select(k=>k.ToString()).ToArray(),
            "Reconcile 后不得删除任何真实 Building 视图");
        // 正式实体语义：默认世界注册的岗位 Place 是实例岗位（workplace:<InstanceId>:main），
        // 旧静态 ws_* Place 不再创建；且实例岗位与真实 Building 是同一实体，只由 Building 视图绘制。
        // 因此锚点集合从运行时快照派生，并断言每个实例岗位由对应真实建筑视图承载（不写死 ID）。
        var registered=actual.Places.Where(p=>p.PlaceId.StartsWith("workplace:",StringComparison.Ordinal))
            .GroupBy(p=>p.PlaceId,StringComparer.Ordinal).Select(g=>g.First()).ToArray();
        Assert.IsNotEmpty(registered,"默认生成世界必须至少注册一个实例岗位 Place");
        var idList=new List<string>();var viewList=new List<Component>();
        foreach(var place in registered)
        {
            Assert.IsFalse(string.IsNullOrEmpty(place.BuildingInstanceId),"实例岗位 Place 必须绑定真实建筑实例："+place.PlaceId);
            Assert.IsTrue(buildings.Contains(place.BuildingInstanceId),"实例岗位必须由真实建筑视图承载（同一实体不重复绘制）："+place.PlaceId);
            idList.Add(place.PlaceId);viewList.Add((Component)buildings[place.BuildingInstanceId]);
        }
        var ids=idList.ToArray();var views=viewList.ToArray();
        foreach(var id in ids)Assert.IsFalse(places.Contains(id),"实例岗位不得再画第二套 Place 实体："+id);
        var anchors=views.Select(v=>v.transform.position).ToArray();
        var sprites=views.Select(v=>v.transform.Find("Body").GetComponent<SpriteRenderer>().sprite).ToArray();
        var beforeFacts=CaptureDomain();
        var tavern=new HomePlaceSnapshot("VirtualSpot","VirtualSpot","ActivityPlace","Actual Presence / Destination",null,
            new string[0],new string[0],"Read-only projection",true);
        var active=new ProductHomeSnapshot(actual.Persons,stable.Concat(new[]{tavern}),actual.Diagnostics,actual.Buildings);
        Assert.AreEqual(actual.Persons.Select(p=>p.PersonId).ToArray(),active.Persons.Select(p=>p.PersonId).ToArray(),"活动地点不得挤掉真实 people");
        Assert.AreEqual(actual.Diagnostics.ToArray(),active.Diagnostics.ToArray(),"活动地点不得挤掉真实 diagnostics");
        Assert.AreEqual(actual.Buildings.Select(b=>b.InstanceId).ToArray(),active.Buildings.Select(b=>b.InstanceId).ToArray(),"活动地点不得挤掉真实 buildings");
        for(int cycle=0;cycle<3;cycle++)
        {
            Call(_host,"Reconcile",active);
            var tavernView=(Component)((IDictionary)Get(_host,"PlaceViews"))["VirtualSpot"];
            var tavernAnchor=tavernView.transform.position;
            for(int i=0;i<ids.Length;i++)
            {
                // 实例岗位与真实 Building 是同一实体：身份必须仍由同一个 Building 视图承载，
                // 且不得在 PlaceViews 再画第二套（DrawsWorldEntity=false 的产品契约）。
                Assert.AreSame(views[i],((IDictionary)Get(_host,"BuildingViews"))[registered[i].BuildingInstanceId],ids[i]+" must keep the same building view");
                Assert.IsFalse(((IDictionary)Get(_host,"PlaceViews")).Contains(ids[i]),ids[i]+" must not be drawn a second time as a Place");
                Assert.AreEqual(anchors[i],views[i].transform.position,ids[i]+" must not move when Tavern appears");
                Assert.AreSame(sprites[i],views[i].transform.Find("Body").GetComponent<SpriteRenderer>().sprite);
                Assert.True(views[i].transform.Find("Body").GetComponent<SpriteRenderer>().enabled);
            }
            Call(_host,"Reconcile",baseline);
            Call(_host,"Reconcile",active);
            Assert.AreEqual(tavernAnchor,((Component)((IDictionary)Get(_host,"PlaceViews"))["VirtualSpot"]).transform.position);
        }
        AssertDomainFactsUnchanged("Tavern churn",beforeFacts,CaptureDomain());
        Call(_host,"RefreshNow");
        Call(_host,"SetVisualPrototype",false);
        Call(_host,"SetVisualPrototype",true);
        for(int i=0;i<ids.Length;i++)Assert.AreEqual(anchors[i],views[i].transform.position,"B anchors survive A/B roundtrip");
        yield return null;
    }

    // ==================== 2026-11-05 产品体验修复专项 ====================

    private static object Field(object o,string name)
    {
        var f=o.GetType().GetField(name,I);
        Assert.IsNotNull(f,"字段不存在："+name+" @ "+o.GetType().Name);
        return f.GetValue(o);
    }

    /// <summary>证据截图：写入本包目录，仅用于人工复核，不参与断言。</summary>
    private static void Shoot(string name)
    {
        var dir=Path.Combine(RepoRoot(),"Development/StudioRuns/InitialWorld-ProductFix-20261105/screenshots");
        Directory.CreateDirectory(dir);
        ScreenCapture.CaptureScreenshot(Path.Combine(dir,name+".png"));
        Debug.Log("[ReviewShot] "+Path.Combine(dir,name+".png"));
    }

    /// <summary>
    /// A：虚拟 ActivityPlace（Tavern 类）在默认 A 档不得创建可见世界实体；
    /// Place 身份 / 人物 / 真实建筑视图保持不变，且切到 B 仍按既有语义绘制（A→B→A 往返）。
    /// </summary>
    [UnityTest] public IEnumerator AProfileHidesVirtualActivityPlaceButKeepsFactsAndBRoundtrip()
    {
        Call(_host,"SetVisualPrototype",false);Call(_host,"RefreshNow");
        var actual=Snapshot;
        var tavern=new HomePlaceSnapshot("VirtualSpot","VirtualSpot","ActivityPlace","Actual Presence / Destination",null,
            new string[0],new string[0],"Read-only projection",true);
        var withTavern=new ProductHomeSnapshot(actual.Persons,actual.Places.Concat(new[]{tavern}),actual.Diagnostics,actual.Buildings);
        var beforeFacts=CaptureDomain();
        var buildingCount=((IDictionary)Get(_host,"BuildingViews")).Count;
        var personIds=actual.Persons.Select(p=>p.PersonId).ToArray();

        Call(_host,"Reconcile",withTavern);
        Shoot("a-profile-virtual-place-hidden");yield return new WaitForEndOfFrame();
        var placeViews=(IDictionary)Get(_host,"PlaceViews");
        Assert.IsFalse(placeViews.Contains("VirtualSpot"),"A 档不得为虚拟 ActivityPlace 创建可见占位实体");
        Assert.AreEqual(buildingCount,((IDictionary)Get(_host,"BuildingViews")).Count,"真实 Farm/Kitchen/House 视图数量不得改变");
        Assert.IsNotNull(Snapshot.Place("VirtualSpot"),"A 档隐藏视图不等于删除 Place 身份");
        CollectionAssert.AreEqual(personIds,Snapshot.Persons.Select(p=>p.PersonId).ToArray(),"人物快照不得改变");
        var anchor=(Vector3)Call(_host,"Anchor","VirtualSpot");
        Assert.IsFalse(float.IsNaN(anchor.x)||float.IsNaN(anchor.y),"隐藏视图后仍必须提供定位锚点");

        Call(_host,"SetVisualPrototype",true);Call(_host,"Reconcile",withTavern);
        Assert.IsTrue(((IDictionary)Get(_host,"PlaceViews")).Contains("VirtualSpot"),"B 档必须保留既有 ActivityPlace 绘制语义");
        Call(_host,"SetVisualPrototype",false);Call(_host,"Reconcile",withTavern);
        Assert.IsFalse(((IDictionary)Get(_host,"PlaceViews")).Contains("VirtualSpot"),"回到 A 档应移除该视图且不销毁事实");
        AssertDomainFactsUnchanged("A virtual activity place",beforeFacts,CaptureDomain());
        yield return null;
    }

    /// <summary>
    /// C：资源行是纯展示（点击不导航）；显式「查看岗位」按完整 WorkplaceId 定位且保留人物选择。
    /// </summary>
    [UnityTest] public IEnumerator ResourceRowIsPlainAndViewWorkplaceNavigatesPrecisely()
    {
        var shell=Get(_host,"Shell");
        Call(shell,"OpenSurface",Enum.Parse(T("HomeSurface"),"Management"));
        yield return null;
        var management=Get(shell,"Management");
        Assert.IsNotNull(management,"管理页必须存在");
        string selectedPerson=Snapshot.Persons.First().PersonId;
        Assert.IsTrue((bool)Call(management,"SelectPersonRow",selectedPerson));
        Call(management,"SelectTab",Enum.Parse(T("ManagementViewTab"),"Resources"));
        yield return null;
        Call(management,"RefreshNow");
        Shoot("resources-explicit-view-workplace");yield return new WaitForEndOfFrame();
        var domainBefore=CaptureDomain();
        Shoot("resources-explicit-view-workplace");yield return new WaitForEndOfFrame();
        var rows=((IEnumerable)Field(management,"_resourceRows")).Cast<object>().ToList();
        Assert.IsNotEmpty(rows,"默认世界应有 Farm / Kitchen 资源行");
        var resources=Get(Get(management,"CurrentSnapshot"),"Resources");
        Assert.IsNotNull(resources,"资源快照必须可读");
        var workplaces=((IEnumerable)Get(resources,"Workplaces")).Cast<object>().ToList();
        Assert.AreEqual(rows.Count,workplaces.Count,"资源行数必须与实例岗位数一致");

        for(int i=0;i<rows.Count;i++)
        {
            Call(management,"SelectTab",Enum.Parse(T("ManagementViewTab"),"Resources"));
            yield return null;
            var row=(MonoBehaviour)((IEnumerable)Field(management,"_resourceRows")).Cast<object>().ElementAt(i);
            var workplace=workplaces[i];
            string workplaceId=(string)Get(workplace,"WorkplaceId");
            string personBefore=(string)Get(management,"SelectedPersonId");
            Assert.AreEqual(selectedPerson,personBefore,"必须保留真实非空人物选择");
            var tabBefore=Field(management,"_activeTab");

            // 1) 普通区域点击不得产生导航 / 写入。
            row.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            Assert.AreEqual(tabBefore,Field(management,"_activeTab"),"资源行普通点击不得导航："+workplaceId);

            // 2) 显式按钮必须精确携带完整 WorkplaceId 定位，且不清人物选择。
            var action=row.transform.Find("ViewWorkplace");
            Assert.IsNotNull(action,"资源行必须有显式「查看岗位」按钮："+workplaceId);
            var button=action.GetComponent<UnityEngine.UI.Button>();
            Assert.IsTrue(button.interactable,"可达岗位的查看按钮必须可用："+workplaceId);
            button.onClick.Invoke();
            Assert.AreEqual("Workplaces",Field(management,"_activeTab").ToString(),"查看岗位必须切到职责页");
            Assert.AreEqual(workplaceId,(string)Field(management,"_workplaceCandidate"),"必须按完整 WorkplaceId 定位，不得猜测同名岗位");
            Assert.AreEqual(personBefore,(string)Get(management,"SelectedPersonId"),"定位不得清空当前 Person 选择");
            yield return null;
            var targets=((IEnumerable)Get(Get(management,"CurrentSnapshot"),"Workplaces")).Cast<object>().ToList();
            int targetIndex=targets.FindIndex(w=>(string)Get(w,"WorkplaceId")==workplaceId);
            Assert.GreaterOrEqual(targetIndex,0,"职责页必须存在同一实例岗位");
            var targetRow=(Component)((IEnumerable)Field(management,"_workplaceRows")).Cast<object>().ElementAt(targetIndex);
            var scroll=(UnityEngine.UI.ScrollRect)Field(management,"workplaceScroll");
            Canvas.ForceUpdateCanvases();
            var bounds=RectTransformUtility.CalculateRelativeRectTransformBounds(scroll.viewport,targetRow.transform);
            Assert.IsTrue(scroll.viewport.rect.Overlaps(new Rect(bounds.min.x,bounds.min.y,bounds.size.x,bounds.size.y)),"定位后的实例岗位行必须位于可见区域");
        }
        AssertDomainFactsUnchanged("resource information and navigation",domainBefore,CaptureDomain());
        yield return null;
    }

    /// <summary>
    /// B：设置菜单打开/关闭；保存失败留在 Home 并给出原因；成功后经主菜单接缝返回。
    /// </summary>
    [UnityTest] public IEnumerator SettingsSaveFailureStaysAndSuccessReturnsToMainMenu()
    {
        var shell=Get(_host,"Shell");
        var shellTransform=((Component)shell).transform;
        Call(shell,"ToggleSettings");
        var panel=shellTransform.Find("SettingsPanel");
        Assert.IsNotNull(panel,"设置面板必须存在");
        Assert.IsTrue(panel.gameObject.activeSelf,"点击设置应打开菜单");
        Assert.IsTrue((bool)Get(shell,"SettingsOpen"),"SettingsOpen 必须反映菜单状态");
        Assert.IsTrue((bool)Get(shell,"SurfaceModalActive"),"设置打开必须纳入既有模态阻断");

        var save=shellTransform.Find("SettingsPanel/SettingsMenu/SettingsSave").GetComponent<UnityEngine.UI.Button>();
        var saveExit=shellTransform.Find("SettingsPanel/SettingsMenu/SettingsSaveAndExit").GetComponent<UnityEngine.UI.Button>();
        Assert.IsNotNull(save,"设置菜单必须包含「保存游戏」按钮");
        Assert.IsNotNull(saveExit,"设置菜单必须包含「保存并返回主菜单」按钮");
        var menu=Find("MainMenuUI");
        var menuCanvas=(GameObject)Field(menu,"canvasRoot");
        var failure=T("SaveSystem").GetField("ForceSaveFailureForTests",S);

        // 1) 确定性失败：留在 Home，显示原因，不返回主菜单。
        failure.SetValue(null,(Func<bool>)(()=>true));
        save.onClick.Invoke();yield return null;
        Assert.IsTrue(panel.gameObject.activeSelf,"保存失败必须保留设置菜单");
        Assert.IsFalse(menuCanvas.activeSelf,"保存失败不得返回主菜单");
        var status=(TMPro.TMP_Text)Field(shell,"_settingsStatus");
        StringAssert.Contains("保存失败",status.text,"失败必须给出可理解原因");

        // 2) 保存并返回：先确认主菜单可用，再保存，成功后返回。
        saveExit.onClick.Invoke();yield return null;
        Assert.IsFalse(menuCanvas.activeSelf,"保存失败时保存并返回也不得跳转");
        failure.SetValue(null,null);
        saveExit.onClick.Invoke();yield return null;
        Assert.IsTrue(menuCanvas.activeSelf,"保存成功后必须返回真实主菜单");
        Call(shell,"ToggleSettings");
        yield return null;
    }

    // ==================== 2026-11-05 复核补齐：真实输入链 / 重入 / Continue ====================

    /// <summary>
    /// 设置打开时，EventSystem 射线在人物屏幕位置必须命中设置遮罩（不穿透到世界）；
    /// 关闭后同一位置的命中不再是遮罩，恢复既有输入语义。
    /// </summary>
    [UnityTest] public IEnumerator SettingsOverlayBlocksWorldPointerAndCloseRestores()
    {
        var shell=Get(_host,"Shell");
        var shellT=((Component)shell).transform;
        var views=(IDictionary)Get(_host,"PersonViews");
        Assert.IsNotEmpty(views,"默认世界必须有可点击人物视图");
        Vector3 sp=new Vector3(Screen.width*0.5f,Screen.height*0.5f,0f);
        var es=UnityEngine.EventSystems.EventSystem.current;
        Assert.IsNotNull(es,"场景必须有 EventSystem");

        Call(shell,"ToggleSettings");yield return null;
        Shoot("settings-menu");yield return new WaitForEndOfFrame();
        var panel=shellT.Find("SettingsPanel");
        var results=new List<UnityEngine.EventSystems.RaycastResult>();
        var ped=new UnityEngine.EventSystems.PointerEventData(es){position=sp};
        es.RaycastAll(ped,results);
        Assert.IsNotEmpty(results,"设置打开时屏幕中心必须被 UI 命中（遮罩为全屏）");
        var top=results[0].gameObject;
        Assert.IsTrue(top==panel.gameObject || top.transform.IsChildOf(panel), "设置遮罩必须挡在世界之前，实际命中："+top.name);
        var townInput=(CanvasGroup)Field(shell,"_townInput");
        Assert.IsNotNull(townInput,"既有 Town 输入阻断组必须存在");
        Assert.IsFalse(townInput.blocksRaycasts,"设置打开时世界输入必须被阻断");

        Call(shell,"CloseSettings");yield return null;
        results.Clear();es.RaycastAll(ped,results);
        if(results.Count>0)
            Assert.IsFalse(results[0].gameObject==panel.gameObject || results[0].gameObject.transform.IsChildOf(panel),
                "关闭后不得仍由设置遮罩拦截："+results[0].gameObject.name);
        Assert.IsTrue(townInput.blocksRaycasts,"关闭后必须恢复既有输入阻断语义");
        Assert.IsFalse((bool)Get(shell,"SurfaceModalActive"),"关闭后模态必须解除");
    }

    /// <summary>
    /// 同帧双击只产生一次存档事务；失败留页并显示原因；成功只保存不跳转，且不重建实体。
    /// </summary>
    [UnityTest] public IEnumerator SettingsSaveIsSingleFlightPerFrameAndStaysOnFailure()
    {
        var shell=Get(_host,"Shell");
        var shellT=((Component)shell).transform;
        var menu=Find("MainMenuUI");
        var menuCanvas=(GameObject)Field(menu,"canvasRoot");
        var town=Find("TownUI");
        var player=(object)Get(town,"PlayerData");
        var peopleBefore=JsonUtility.ToJson(player);
        var buildingsBefore=((IDictionary)Get(_host,"BuildingViews")).Count;
        var failure=T("SaveSystem").GetField("ForceSaveFailureForTests",S);
        int calls=0;
        failure.SetValue(null,(Func<bool>)(()=>{calls++;return true;}));

        Call(shell,"ToggleSettings");yield return null;
        var save=shellT.Find("SettingsPanel/SettingsMenu/SettingsSave").GetComponent<UnityEngine.UI.Button>();
        save.onClick.Invoke();
        save.onClick.Invoke();
        Assert.AreEqual(1,calls,"同一帧重复点击只允许一次存档事务");
        Assert.IsFalse(menuCanvas.activeSelf,"失败不得返回主菜单");
        var status=(TMPro.TMP_Text)Field(shell,"_settingsStatus");
        StringAssert.Contains("保存失败",status.text,"失败必须给出可理解原因");

        yield return null;
        save.onClick.Invoke();
        Assert.AreEqual(2,calls,"跨帧后允许再次尝试");
        Assert.AreEqual(peopleBefore,JsonUtility.ToJson(player),"保存失败不得重建/改写人物数据");
        Assert.AreEqual(buildingsBefore,((IDictionary)Get(_host,"BuildingViews")).Count,"保存失败不得重建建筑视图");

        failure.SetValue(null,null);
        yield return null;
        save.onClick.Invoke();
        StringAssert.Contains("保存成功",((TMPro.TMP_Text)Field(shell,"_settingsStatus")).text);
        Assert.IsFalse(menuCanvas.activeSelf,"仅保存不得跳转主菜单");
        Assert.IsTrue(shellT.Find("SettingsPanel").gameObject.activeSelf,"成功反馈后菜单仍保持打开");
        Call(shell,"CloseSettings");
        yield return null;
    }

    /// <summary>
    /// 保存并返回 → 真实主菜单 OnContinue → Town.Initialize 重进 Home：
    /// 人物 ID / 建筑实例 ID / 岗位职责 / 资源必须一致，且无 reseed。
    /// </summary>
    [UnityTest] public IEnumerator SaveAndReturnThenContinueKeepsIdentityAssignmentsAndResources()
    {
        var shell=Get(_host,"Shell");
        var shellT=((Component)shell).transform;
        var town=Find("TownUI");
        var player=(object)Get(town,"PlayerData");
        string peopleBefore=JsonUtility.ToJson(player);
        var snapBefore=Snapshot;
        string[] personsBefore=snapBefore.Persons.Select(p=>p.PersonId).ToArray();
        string[] assignmentsBefore=snapBefore.Persons.Select(p=>p.AssignmentId).ToArray();
        string[] buildingsBefore=snapBefore.Buildings.Select(b=>b.InstanceId).ToArray();
        var resourceProperty=T("VillageRuntimeBridge").GetField("Resources",S);
        var resourcesBefore=resourceProperty.GetValue(null);
        int ingredientsBefore=(int)Get(resourcesBefore,"Ingredients"), foodBefore=(int)Get(resourcesBefore,"Food");

        Call(shell,"ToggleSettings");yield return null;
        shellT.Find("SettingsPanel/SettingsMenu/SettingsSaveAndExit").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
        yield return null;
        var menu=Find("MainMenuUI");
        Assert.IsTrue(((GameObject)Field(menu,"canvasRoot")).activeSelf,"保存并返回后必须回到真实主菜单");
        Assert.IsFalse((bool)T("VillageRuntimeBridge").GetField("IsSimulationActive",S).GetValue(null),"主菜单前台不得继续村庄模拟");
        Assert.IsTrue(((UnityEngine.UI.Button)Field(menu,"btnContinue")).interactable,"真实Continue入口必须可用");

        Call(menu,"OnContinue");
        yield return null;yield return null;
        var host2=Find("ProductHomeWorldHost");
        Assert.IsNotNull(host2,"Continue 后必须存在同一 World 宿主");
        Assert.IsTrue(((Component)host2).gameObject.activeInHierarchy,"Continue 后 World 必须重新激活");
        var snapAfter=(ProductHomeSnapshot)Get(host2,"Snapshot");
        CollectionAssert.AreEqual(personsBefore,snapAfter.Persons.Select(p=>p.PersonId).ToArray(),"Continue 后人物 ID 必须一致");
        CollectionAssert.AreEqual(assignmentsBefore,snapAfter.Persons.Select(p=>p.AssignmentId).ToArray(),"岗位职责必须一致");
        CollectionAssert.AreEqual(buildingsBefore,snapAfter.Buildings.Select(b=>b.InstanceId).ToArray(),"建筑实例 ID 必须一致，不得 reseed");
        var town2=Find("TownUI");
        Assert.AreEqual(peopleBefore,JsonUtility.ToJson(Get(town2,"PlayerData")),"Continue 后人物数据必须与保存前一致");
        var resourcesAfter=resourceProperty.GetValue(null);
        Assert.AreEqual(ingredientsBefore,(int)Get(resourcesAfter,"Ingredients"),"Continue必须恢复食材而非重复结算");
        Assert.AreEqual(foodBefore,(int)Get(resourcesAfter,"Food"),"Continue必须恢复食物而非重新初始化");
        yield return null;
    }
}
}
