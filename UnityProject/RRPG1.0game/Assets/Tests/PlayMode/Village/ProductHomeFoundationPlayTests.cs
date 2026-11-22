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
/// <summary>
/// Home 世界展示 / 相机 / 选择稳定性的 Play 回归（按 Initial World 实例化契约更新）。
///
/// 旧夹具假设（已移除）：
///   - 默认新世界没有建筑、岗位是静态 ws_farm_01、Home 是独立 Place 实体；
/// 现夹具：
///   - 正式 products 入口 NewGame() 读取生成配置 → 真实 Completed 实例建筑；
///   - 派工只使用实例岗位 workplace:&lt;InstanceId&gt;:main；
///   - Home 是绑定真实 Completed 住房实例的只读投影（DrawsWorldEntity=false），
///     世界实体只由 Building 视图绘制，不再有独立 Home Place 视图。
///
/// 存档保护：复用持久 SaveSlotGuard 协议（先捕获当前用户单槽字节 + SHA256 + manifest，
/// 存在未完成 run 阻断；finally 只按本 run 已验备份恢复并回读校验）。
/// </summary>
public sealed class ProductHomeFoundationPlayTests
{
    private const BindingFlags S = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags I = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private const string SaveProtectionRelative = "Development/StudioRuns/Initialization-20261027/save-guard";

    private object _host, _town, _player;
    private string _id, _farmWorkplace, _farmBuilding, _kitchenWorkplace, _houseBuilding;
    private SaveSlotGuard _guard;

    private static Type T(string name) => Type.GetType(name + ", Assembly-CSharp", true);
    private static object Static(string type, string method, params object[] args)
    {
        int count = args == null ? 0 : args.Length;
        var candidates = T(type).GetMethods(S)
            .Where(m => m.Name == method && m.GetParameters().Length == count).ToList();
        Assert.AreEqual(1, candidates.Count, "静态方法必须唯一：" + type + "." + method + "/" + count);
        return candidates[0].Invoke(null, args);
    }
    private static object Call(object instance, string method, params object[] args)
    {
        int count = args == null ? 0 : args.Length;
        var candidates = instance.GetType().GetMethods(I)
            .Where(m => m.Name == method && m.GetParameters().Length == count).ToList();
        Assert.AreEqual(1, candidates.Count, "方法必须唯一：" + method + " @ " + instance.GetType().FullName);
        return candidates[0].Invoke(instance, args);
    }
    private static object Get(object instance, string property)
    {
        var field = instance.GetType().GetField(property, I);
        if (field != null) return field.GetValue(instance);
        return instance.GetType().GetProperty(property, I).GetValue(instance, null);
    }
    private static object Find(string type) => UnityEngine.Object.FindObjectOfType(T(type), true);
    private static VillageTime Clock => (VillageTime)T("VillageRuntimeBridge").GetField("Time", S).GetValue(null);
    private ProductHomeSnapshot Snapshot => (ProductHomeSnapshot)Get(_host, "Snapshot");
    private HomeSelectionContext Selection => (HomeSelectionContext)Get(_host, "Selection");
    private Component View(string id, bool person = true)
        => (Component)((IDictionary)Get(_host, person ? "PersonViews" : "PlaceViews"))[id];
    private Component BuildingView(string id) => (Component)((IDictionary)Get(_host, "BuildingViews"))[id];
    private string Domain() => JsonUtility.ToJson(Static("VillageRuntimeBridge", "BuildSaveData"))
        + JsonUtility.ToJson(Static("ActivityRuntimeBridge", "BuildSaveData"));

    private static string RepoRoot() => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));

    private static string HashBytes(byte[] bytes)
    {
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string HashFile(string path)
    {
        using (var sha = SHA256.Create())
        using (var stream = File.OpenRead(path))
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static void Evidence(string tag, string message)
    {
        string path = Path.Combine(RepoRoot(), "Development/StudioRuns/Initialization-20261027/play-evidence/product-home-foundation-play-evidence.log");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllText(path, DateTime.UtcNow.ToString("O") + " [" + tag + "] " + message + "\n");
        }
        catch (Exception ex) { Debug.LogWarning("[ProductHomeFoundationPlayTests] evidence write failed: " + ex.Message); }
        Debug.Log("[ProductHomeFoundationPlayTests] " + tag + " " + message);
    }

    // ==================== 持久单槽保护 ====================

    private sealed class SaveSlotGuard
    {
        private const string ManifestName = "manifest.json";
        private const string BackupName = "original-save.bin";
        [Serializable] private sealed class ManifestStatus { public string runId; public string status; }

        private string _path, _runId, _binPath, _manifestPath, _captureUtc, _beforeHash, _runDir;
        private bool _existed, _captured, _restored;
        private byte[] _bytes;
        public bool Restored => _restored;

        private static string ProtectionRoot() =>
            Path.Combine(RepoRoot(), SaveProtectionRelative.Replace('/', Path.DirectorySeparatorChar));

        public void Capture()
        {
            Assert.IsFalse(_captured, "SaveSlotGuard 同一实例只能 Capture 一次");
            BlockOnUnfinishedRuns();
            _path = (string)Static("SaveSystem", "GetSavePath");
            Assert.IsFalse(string.IsNullOrEmpty(_path), "SaveSystem.GetSavePath 必须返回真实路径");
            _captureUtc = DateTime.UtcNow.ToString("O");
            _runId = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
            string root = ProtectionRoot();
            _runDir = Path.Combine(root, _runId);
            for (int i = 2; Directory.Exists(_runDir); i++) _runDir = Path.Combine(root, _runId + "-" + i);
            _runId = Path.GetFileName(_runDir);
            Directory.CreateDirectory(_runDir);
            _binPath = Path.Combine(_runDir, BackupName);
            _manifestPath = Path.Combine(_runDir, ManifestName);
            _existed = File.Exists(_path);
            _bytes = _existed ? File.ReadAllBytes(_path) : null;
            _beforeHash = _existed ? HashBytes(_bytes) : "<absent>";
            if (_existed)
            {
                using (var stream = new FileStream(_binPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    stream.Write(_bytes, 0, _bytes.Length);
                Assert.AreEqual(_beforeHash, HashBytes(File.ReadAllBytes(_binPath)), "备份回读 SHA256 不一致");
                Assert.AreEqual(_beforeHash, HashFile(_path), "捕获后当前档已被改写，拒绝继续");
            }
            WriteManifest(_existed ? "captured" : "absent", null);
            _captured = true;
            Evidence("SAVE-CAPTURE", "runId=" + _runId + " existed=" + _existed + " sha256=" + _beforeHash);
        }

        public void Restore()
        {
            Assert.IsTrue(_captured, "SaveSlotGuard.Capture 未成功完成，禁止 Restore");
            if (_restored) return;
            byte[] source = null;
            if (_existed)
            {
                Assert.IsTrue(File.Exists(_binPath), "本次持久备份缺失，拒绝恢复：" + _binPath);
                source = File.ReadAllBytes(_binPath);
                Assert.AreEqual(_beforeHash, HashBytes(source), "本次备份回读 SHA256 不一致，拒绝写回");
            }
            try
            {
                if (_existed) File.WriteAllBytes(_path, source);
                else if (File.Exists(_path)) File.Delete(_path);
                bool exists = File.Exists(_path);
                string after = exists ? HashFile(_path) : "<absent>";
                bool ok = exists == _existed && (!_existed || after == _beforeHash);
                _restored = ok;
                WriteManifest(ok ? "restored" : "restore-failed", ok ? null : "sha256 mismatch after write");
                Evidence("SAVE-RESTORE", "runId=" + _runId + " existed=" + exists + " sha256=" + after + " matches-before=" + ok);
                Assert.IsTrue(ok, "单槽存档恢复失败（backup/manifest 已保留，需人工核对）");
            }
            catch (Exception ex)
            {
                if (!_restored) WriteManifest("restore-failed", ex.GetType().Name + ": " + ex.Message);
                throw;
            }
        }

        private static void BlockOnUnfinishedRuns()
        {
            string root = ProtectionRoot();
            if (!Directory.Exists(root)) return;
            var unfinished = new List<string>();
            foreach (string dir in Directory.GetDirectories(root))
            {
                string manifest = Path.Combine(dir, ManifestName);
                if (!File.Exists(manifest)) { unfinished.Add(Path.GetFileName(dir) + "(no-manifest)"); continue; }
                ManifestStatus parsed = null;
                try { parsed = JsonUtility.FromJson<ManifestStatus>(File.ReadAllText(manifest)); } catch (Exception) { }
                string status = parsed == null ? null : parsed.status;
                if (status == "restored" || status == "protective-snapshot") continue;
                unfinished.Add(Path.GetFileName(dir) + "(" + (status ?? "unreadable") + ")");
            }
            Assert.AreEqual(0, unfinished.Count, "存在未完成的存档保护 run，已阻止新测试："
                + string.Join("; ", unfinished) + " @ " + root);
        }

        private void WriteManifest(string status, string error)
        {
            var sb = new StringBuilder();
            sb.Append("{\n");
            sb.Append("  \"runId\": \"" + JsonText(_runId) + "\",\n");
            sb.Append("  \"status\": \"" + JsonText(status) + "\",\n");
            sb.Append("  \"captureUtc\": \"" + JsonText(_captureUtc) + "\",\n");
            sb.Append("  \"updatedUtc\": \"" + DateTime.UtcNow.ToString("O") + "\",\n");
            sb.Append("  \"sourcePath\": \"" + JsonText(_path) + "\",\n");
            sb.Append("  \"sourceExisted\": " + (_existed ? "true" : "false") + ",\n");
            sb.Append("  \"sourceBytes\": " + (_bytes == null ? 0 : _bytes.LongLength) + ",\n");
            sb.Append("  \"sourceSha256\": \"" + JsonText(_beforeHash) + "\",\n");
            sb.Append("  \"backupFile\": " + (_existed ? "\"" + BackupName + "\"" : "null") + ",\n");
            sb.Append("  \"backupBytes\": " + (_existed && File.Exists(_binPath) ? new FileInfo(_binPath).Length : 0) + ",\n");
            sb.Append("  \"backupSha256\": " + (_existed ? "\"" + JsonText(_beforeHash) + "\"" : "null") + ",\n");
            sb.Append("  \"readbackVerified\": " + (_existed ? "true" : "false") + ",\n");
            sb.Append("  \"restored\": " + (_restored ? "true" : "false") + ",\n");
            sb.Append("  \"lastError\": " + (error == null ? "null" : "\"" + JsonText(error) + "\"") + ",\n");
            sb.Append("  \"capturedBy\": \"ProductHomeFoundationPlayTests.SaveSlotGuard (test-only helper)\",\n");
            sb.Append("  \"note\": \"只允许用本 runID 的 original-save.bin 恢复；进程 abort 时 TearDown 不执行，备份留待人工取证。\"\n");
            sb.Append("}\n");
            File.WriteAllText(_manifestPath, sb.ToString());
        }

        private static string JsonText(string value)
            => value == null ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    [UnitySetUp]
    public IEnumerator Setup()
    {
        _guard = new SaveSlotGuard();
        _guard.Capture();
        Static("VillageRuntimeBridge", "ResetForTests");
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        _player = Static("SaveSystem", "NewGame");
        Assert.IsNotNull(_player, "正式默认 NewGame 必须成功");
        Assert.IsTrue((bool)Static("SaveSystem", "Save", _player), "隔离世界必须可保存");
        Call(Find("MainMenuUI"), "OnContinue");
        yield return null;
        yield return null;
        _town = Find("TownUI");
        Assert.IsNotNull(_town, "真实产品入口必须进入 TownUI");
        _player = Get(_town, "PlayerData");
        Assert.IsNotNull(_player, "TownUI.PlayerData 必须来自真实 Load");
        Clock.Pause();
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "ProductHomeWorldHost 必须由产品世界创建");
        Call(_host, "RefreshNow");
        _id = Snapshot.Persons[0].PersonId;

        HomeBuildingSnapshot farm = Snapshot.Buildings.First(b => b.DefinitionId == "building_farm"
            && b.LifecycleState.ToString() == "Completed");
        _farmBuilding = farm.InstanceId;
        _farmWorkplace = "workplace:" + _farmBuilding + ":main";
        HomeBuildingSnapshot kitchen = Snapshot.Buildings.First(b => b.DefinitionId == "building_kitchen"
            && b.LifecycleState.ToString() == "Completed");
        _kitchenWorkplace = "workplace:" + kitchen.InstanceId + ":main";
        HomePlaceSnapshot home = Snapshot.Places.First(p => p.Kind == "Home" && p.PlaceId == "Home:" + _id);
        _houseBuilding = home.BuildingInstanceId;
        Assert.IsFalse(string.IsNullOrEmpty(_houseBuilding), "人物 Home 投影必须绑定真实 Completed 住房");
        Evidence("SETUP", "person=" + _id + " farm=" + _farmBuilding + " farmWorkplace=" + _farmWorkplace
            + " kitchenWorkplace=" + _kitchenWorkplace + " house=" + _houseBuilding
            + " buildings=" + Snapshot.Buildings.Count);
    }

    [TearDown]
    public void Cleanup()
    {
        try
        {
            _guard?.Restore();
        }
        finally
        {
            Static("VillageRuntimeBridge", "ResetForTests");
        }
    }

    [UnityTest]
    public IEnumerator StableEntity_AwayReturnAndSimulationGating()
    {
        var view = View(_id);
        var farmView = BuildingView(_farmBuilding);
        var houseView = BuildingView(_houseBuilding);
        Assert.IsTrue((bool)Get(view, "ResolvedSharedAsset"));

        // 实例岗位派工（不是静态 ws_farm_01）。
        Assert.AreEqual("Success", Static("VillageRuntimeBridge", "TryAssignPerson", _id, _farmWorkplace).ToString(),
            "实例 Farm 岗位必须可合法派工");
        Call(_host, "RefreshNow");
        Assert.AreSame(view, View(_id), "派工不得替换人物长期视图");
        Assert.AreSame(farmView, BuildingView(_farmBuilding), "派工不得替换 Farm 建筑视图");

        // 离村：人物视图隐藏，住房 / 建筑实体不被复制或替换。
        Static("VillageRuntimeBridge", "SetExpeditionPersonIds", (object)new[] { _id });
        Call(_host, "RefreshNow");
        Assert.IsFalse(view.gameObject.activeSelf, "离村人物视图必须隐藏");
        Assert.IsTrue(Snapshot.Person(_id).Away, "离村状态必须来自真实快照");
        Assert.AreSame(houseView, BuildingView(_houseBuilding), "离村不得替换真实住房建筑视图");
        Assert.IsFalse(((IDictionary)Get(_host, "PlaceViews")).Contains("Home:" + _id),
            "Home 只作为住房实例投影，不得再创建独立 Place 世界实体");
        HomePlaceSnapshot home = Snapshot.Place("Home:" + _id);
        Assert.IsNotNull(home, "人物住房位置语义必须保留为 Home 投影");
        Assert.AreEqual(_houseBuilding, home.BuildingInstanceId, "Home 投影必须绑定真实 Completed 住房 InstanceId");
        Assert.IsFalse(home.DrawsWorldEntity, "Home 投影不得凭空再画一套房屋");

        Static("VillageRuntimeBridge", "ClearExpeditionPersonIds");
        Call(_host, "RefreshNow");
        Assert.AreSame(view, View(_id), "回村必须复用同一人物视图");
        Assert.IsTrue(view.gameObject.activeSelf, "回村人物视图必须恢复可见");
        Assert.AreSame(houseView, BuildingView(_houseBuilding), "回村不得替换住房视图");

        // 实例岗位 Place 与建筑同身份：只保留语义投影，不画第二套实体。
        HomePlaceSnapshot workplacePlace = Snapshot.Place(_farmWorkplace);
        Assert.IsNotNull(workplacePlace, "实例岗位必须有管理 / 在场语义投影");
        Assert.AreEqual(_farmBuilding, workplacePlace.BuildingInstanceId, "实例岗位投影必须指向同一建筑实例");
        Assert.IsFalse(workplacePlace.DrawsWorldEntity, "实例岗位不得在同一建筑上再画 Place 实体");
        Assert.IsFalse(((IDictionary)Get(_host, "PlaceViews")).Contains(_farmWorkplace),
            "实例岗位不得创建第二个世界实体");
        Assert.AreSame(farmView, BuildingView(_farmBuilding), "实例岗位刷新不得替换建筑视图");

        Static("VillageRuntimeBridge", "SetSimulationActive", false);
        yield return null;
        Assert.IsFalse((bool)Get(_host, "WorldVisible"), "模拟关闭后世界必须不可见");
        Assert.AreSame(view, View(_id), "模拟关闭不得销毁人物视图");
        Static("VillageRuntimeBridge", "SetSimulationActive", true);
        yield return null;
        Assert.IsTrue((bool)Get(_host, "WorldVisible"), "模拟恢复后世界必须重新可见");
        Assert.AreSame(view, View(_id), "模拟恢复不得替换人物视图");
        Evidence("STABLE", "personViewStable=True farmViewStable=True houseViewStable=True homePlaceEntity=False");
    }

    [UnityTest]
    public IEnumerator Traveling_DoesNotArriveOrAdvanceClock_ThenUsesActualArrival()
    {
        Static("VillageRuntimeBridge", "RemovePersonAssignment", _id);
        Assert.AreEqual("Success", Static("VillageRuntimeBridge", "TryAssignPerson", _id, _kitchenWorkplace).ToString(),
            "实例 Kitchen 岗位必须可合法派工");
        Assert.AreEqual(WorldAdvanceOutcome.Completed, Clock.AdvanceTo(new WorldInstant(500)).Outcome);
        Call(_host, "RefreshNow");
        Assert.AreEqual(_kitchenWorkplace, Snapshot.Person(_id).CurrentPlaceId, "到场后当前位置必须是实例 Kitchen 岗位");

        Assert.IsTrue((bool)Static("VillageRuntimeBridge", "RemovePersonAssignment", _id));
        Assert.AreEqual("Success", Static("VillageRuntimeBridge", "TryAssignPerson", _id, _farmWorkplace).ToString(),
            "实例 Farm 岗位必须可合法派工");
        Call(_host, "RefreshNow");
        Selection.SelectPerson(_id);
        Call(_host, "RefreshNow");
        var view = View(_id);
        var farmView = BuildingView(_farmBuilding);
        Assert.IsTrue(Snapshot.Person(_id).Traveling, "换岗位后人物必须处于真实通勤中");
        Assert.IsNull(Snapshot.Person(_id).CurrentPlaceId, "通勤中不得提前宣称已到场");

        string before = Domain();
        long minute = Clock.CurrentInstant.Minute;
        Call(view, "StepPresentation", 100f);
        Call(_host, "RefreshNow");
        Assert.AreEqual(minute, Clock.CurrentInstant.Minute, "表现步进不得推进真实 Clock");
        Assert.AreEqual(before, Domain(), "表现步进不得修改任何模拟事实");
        StringAssert.Contains("尚未到达", (string)Get(Get(_host, "Inspector"), "DisplayedText"),
            "通勤中检查器必须明确显示尚未到达");
        Assert.Greater(Vector2.Distance(view.transform.position, farmView.transform.position), .05f,
            "表现位置不得瞬移到目的建筑");

        Vector3 beforeArrival = view.transform.position;
        Clock.AdvanceTo(new WorldInstant(520));
        Call(_host, "RefreshNow");
        Assert.AreEqual(beforeArrival, view.transform.position, "真实到场只更新目标，不瞬移表现位置");
        Assert.IsFalse(Snapshot.Person(_id).Traveling, "真实到场后必须结束通勤");
        Assert.AreEqual(_farmWorkplace, Snapshot.Person(_id).CurrentPlaceId, "真实到场后当前位置必须是实例 Farm 岗位");
        Assert.AreSame(view, View(_id), "到场不得替换人物视图");
        CollectionAssert.Contains(Snapshot.Place(_farmWorkplace).OccupantPersonIds, _id,
            "实例 Farm 岗位占用必须包含真实到场人物");

        before = Domain();
        Call(view, "StepPresentation", .1f);
        Assert.Greater(Vector3.Distance(beforeArrival, view.transform.position), 0, "到场后表现必须继续移动");
        Assert.LessOrEqual(Vector3.Distance(beforeArrival, view.transform.position), .201f,
            "表现移动必须受真实步长约束");
        Call(_host, "RefreshNow");
        Vector3 target = (Vector3)view.GetType().GetField("_target", I).GetValue(view);
        Call(view, "StepPresentation", 100f);
        Assert.AreEqual(target, view.transform.position, "走完最后一段必须到达目标");
        Assert.AreEqual(before, Domain(), "走完最后一段不得修改模拟事实");
        Evidence("TRAVEL", "kitchenPlace=" + _kitchenWorkplace + " farmPlace=" + _farmWorkplace
            + " arrivedAt520=True domainStable=True");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CameraAndSelectionNotSaved_LoadRebuildsFromExistingAuthority()
    {
        Assert.AreEqual("Success", Static("VillageRuntimeBridge", "TryAssignPerson", _id, _farmWorkplace).ToString());
        Call(_host, "RefreshNow");
        string before = Domain();
        Selection.SelectPerson(_id);
        Assert.IsTrue((bool)Call(_host, "FocusSelected"), "人物选中必须能聚焦到真实人物视图");
        var cameraControl = Get(_host, "WorldCamera");
        var camera = (Camera)Get(cameraControl, "ViewCamera");
        Assert.AreEqual(View(_id).transform.position.x, camera.transform.position.x, "聚焦必须跟随人物视图");

        Call(cameraControl, "Pan", new Vector2(2, 1));
        Call(cameraControl, "Zoom", -2f);
        Assert.Less(camera.orthographicSize, 7.5f, "相机缩放必须真实生效");

        // 实例岗位没有独立 Place 实体；同一身份由真实 Building 视图承载并聚焦。
        Selection.SelectBuilding(_farmBuilding);
        Assert.IsTrue((bool)Call(_host, "FocusSelected"), "建筑选中必须能聚焦到真实建筑视图");
        Assert.AreEqual(BuildingView(_farmBuilding).transform.position.x, camera.transform.position.x,
            "聚焦建筑必须跟随同一建筑视图");
        Assert.AreEqual(before, Domain(), "相机 / 选择不得写入模拟事实");

        Assert.IsTrue((bool)Static("SaveSystem", "Save", _player), "含实例建筑 / 职责的世界必须可保存");
        string json = File.ReadAllText((string)Static("SaveSystem", "GetSavePath"));
        foreach (string token in new[] { "ProductHome", "HomeSelection", "WorldCamera", "WorldOrigin", "PresentationIntent" })
            StringAssert.DoesNotContain(token, json, "展示 / 相机 / 选择状态不得入档：" + token);

        var loaded = Static("SaveSystem", "Load");
        Assert.IsNotNull(loaded, "正式实例存档必须可 Continue");
        Call(_town, "Initialize", loaded);
        Call(_host, "RefreshNow");
        Assert.AreEqual(_id, Snapshot.Person(_id).PersonId, "Load 后必须保持同一真实人物");
        Assert.AreEqual(_farmWorkplace, Snapshot.Person(_id).AssignmentId, "Load 必须恢复实例岗位职责");
        Assert.IsNotNull(View(_id), "Load 后人物视图必须可从既有 Authority 重建");
        Assert.IsNotNull(Snapshot.Building(_farmBuilding), "Load 后实例建筑必须存在（不重种、不丢实例）");
        Assert.IsNotNull(BuildingView(_farmBuilding), "Load 后建筑视图必须可由实例身份重建");
        Evidence("CAMERA-SAVE", "person=" + _id + " workplace=" + _farmWorkplace
            + " building=" + _farmBuilding + " loadRestored=True");
        yield return null;
    }
}
}
