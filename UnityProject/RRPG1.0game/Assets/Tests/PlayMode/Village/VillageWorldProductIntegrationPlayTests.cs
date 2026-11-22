using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace RRPG.Village.Tests
{
/// <summary>
/// Minimum Village World Integration WP1 —— Runtime Foundation / Technical Proof 测试
/// （真实 SampleScene + VillageWorldLayer Debug Visualization；Village.Runtime 强类型 + Assembly-CSharp 反射）。
///
/// 覆盖（WP1 技术验收范围）：
///   - Person Presence 正确派生 + Debug 视图只随权威 Simulation 显示（Main Menu 前台阶段不显示）；
///   - Runtime truth / Presence / Place 与 UI 面板生命周期解耦（sim 关停只停视图、状态不失效不重建）；
///   - Project → Persistent World Change 数据链 + 真实 Save / Reload 后痕迹仍在。
///
/// VillageWorldLayer 仅为 **Debug Visualization / Test / Not Frozen**，不代表产品 World；
/// 本测试不断言任何 Spatial Presence / Place Memory / 产品 World 体验。
/// </summary>
public class VillageWorldProductIntegrationPlayTests
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags IF = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static System.Type BridgeType => System.Type.GetType("VillageRuntimeBridge, Assembly-CSharp");
    private static System.Type SaveSystemType => System.Type.GetType("SaveSystem, Assembly-CSharp");
    private static System.Type WorldLayerType => System.Type.GetType("VillageWorldLayer, Assembly-CSharp");
    private static System.Type TownUiType => System.Type.GetType("TownUI, Assembly-CSharp");

    private SaveSlotGuard _guard;

    [SetUp]
    public void Reset()
    {
        // 先持久捕获真实用户单槽（不是删除、不套历史 hash），再用真实 NewGame 建立本用例隔离世界。
        _guard = new SaveSlotGuard();
        _guard.Capture();
        bool completed = false;
        try
        {
            ResetBridge();
            completed = true;
        }
        finally
        {
            if (!completed) RestoreAfterFailedSetup();
        }
    }

    [TearDown]
    public void Cleanup()
    {
        try { _guard?.Restore(); }
        finally { ResetBridge(); }
    }

    private void RestoreAfterFailedSetup()
    {
        if (_guard == null) return;
        try { _guard.Restore(); }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[VillageWorldProductIntegrationPlayTests] SetUp 失败后的存档恢复失败（不掩盖原始异常）："
                + ex.GetType().Name + ": " + ex.Message);
        }
    }

    private static void ResetBridge()
    {
        if (BridgeType != null) InvokeStatic(BridgeType, "ResetForTests");
    }

    private static object InvokeStatic(System.Type type, string method, params object[] args)
    {
        var methods = System.Linq.Enumerable.Where(type.GetMethods(F), m => m.Name == method);
        MethodInfo selected = null;
        foreach (var m in methods)
        {
            var ps = m.GetParameters();
            if (ps.Length != (args == null ? 0 : args.Length)) continue;
            bool ok = true;
            for (int i = 0; i < ps.Length; i++)
            {
                if (args[i] == null) continue;
                if (!ps[i].ParameterType.IsAssignableFrom(args[i].GetType())) { ok = false; break; }
            }
            if (ok) { selected = m; break; }
        }
        Assert.IsNotNull(selected, "静态方法不存在或参数不匹配：" + method + " @ " + type.FullName);
        return selected.Invoke(null, args);
    }

    private static object InvokeInstance(object instance, string method, params object[] args)
    {
        var m = instance.GetType().GetMethod(method, IF);
        Assert.IsNotNull(m, "实例方法不存在：" + method + " @ " + instance.GetType().Name);
        return m.Invoke(instance, args);
    }

    /// <summary>
    /// 读取 Bridge 的静态成员：先按真实成员签名解析属性，再回退字段。
    /// 现有真实签名：`Buildings` / `IntradayWork` 是 property，`Time` / `Projects` / `World` 等是 field。
    /// 成员不存在时断言失败，不静默返回 null；产品 property 不因测试改成 field。
    /// </summary>
    private static object GetBridgeMember(string name)
    {
        var property = BridgeType.GetProperty(name, F);
        if (property != null)
        {
            var getter = property.GetGetMethod(true);
            Assert.IsNotNull(getter, "Bridge 属性缺少 getter：" + name);
            Assert.IsTrue(getter.IsStatic, "Bridge 属性必须是静态成员：" + name);
            return property.GetValue(null, null);
        }
        var field = BridgeType.GetField(name, F);
        Assert.IsNotNull(field, "Bridge 静态成员不存在（property / field 均无）：" + name);
        Assert.IsTrue(field.IsStatic, "Bridge 字段必须是静态成员：" + name);
        return field.GetValue(null);
    }

    private static object GetBridgeField(string name) => GetBridgeMember(name);

    /// <summary>
    /// 真实正式建筑运行时（Buildings 是 property）；存在性 / 可读性 / 已初始化显式断言。
    /// 返回 BuildingRuntime.All（测试程序集不引用 Assembly-CSharp，故仍经反射取值）。
    /// </summary>
    private static System.Collections.IEnumerable GetBridgeBuildings()
    {
        var property = BridgeType.GetProperty("Buildings", F);
        Assert.IsNotNull(property,
            "VillageRuntimeBridge.Buildings 必须是静态属性（真实成员签名）；不得按字段读取，也不得要求产品改成字段");
        Assert.IsNotNull(property.GetGetMethod(true), "VillageRuntimeBridge.Buildings 必须有可读 getter");
        var runtime = GetBridgeMember("Buildings");
        Assert.IsNotNull(runtime, "VillageRuntimeBridge.Buildings 必须已初始化（先走真实 NewGame / Continue）");
        var allProperty = runtime.GetType().GetProperty("All", IF);
        Assert.IsNotNull(allProperty, "BuildingRuntime.All 属性必须存在：" + runtime.GetType().FullName);
        var all = allProperty.GetValue(runtime, null) as System.Collections.IEnumerable;
        Assert.IsNotNull(all, "BuildingRuntime.All 必须可枚举且非 null");
        return all;
    }

    private static object GetProperty(object instance, string name)
    {
        var p = instance.GetType().GetProperty(name, IF);
        Assert.IsNotNull(p, "属性不存在：" + name + " @ " + instance.GetType().Name);
        return p.GetValue(instance, null);
    }

    private static object FindInstance(string typeName)
    {
        var t = System.Type.GetType(typeName + ", Assembly-CSharp");
        if (t == null) return null;
        foreach (var o in Object.FindObjectsOfType(t, true))
        {
            if (o != null) return o;
        }
        return null;
    }

    private static object EnterTown(bool freshWorld = true)
    {
        var town = FindInstance("TownUI");
        Assert.IsNotNull(town, "场景应有 TownUI");
        var mm = FindInstance("MainMenuUI");
        Assert.IsNotNull(mm, "场景应有 MainMenuUI");

        // 进入领地必须走真实产品入口：Main Menu 前台（sim=false 门禁）→ 玩家点击主菜单按钮 →
        // MainMenuUI.EnterTown(collection) → TownUI.Initialize → SetSimulationActive(true)。
        // 不得再用 SetActive(true) 依赖修复前「TownUI.Start 提前把 sim 置 true、MainMenu 不再复位」的旧行为。
        // freshWorld=true：测试 initialization 用真实 NewGame + Save 建立隔离世界（用户原档已由
        // SaveSlotGuard 捕获，TearDown 恢复），不读取 / 删除用户单槽；false：保留既有测试存档的真实读档路径。
        if (freshWorld)
        {
            var fresh = InvokeStatic(SaveSystemType, "NewGame");
            Assert.IsNotNull(fresh, "进入领地前置：应能创建新档数据");
            Assert.IsTrue((bool)InvokeStatic(SaveSystemType, "Save", fresh), "进入领地前置：新档应能写入真实存档");
        }
        InvokeInstance(mm, "OnContinue"); // 产品路径：继续游戏 → 读档 → EnterTown → Initialize
        return FindInstance("TownUI");
    }

    private static string FirstPersonId(object town)
    {
        var pc = GetProperty(town, "PlayerData");
        Assert.IsNotNull(pc, "Town 应持有真实 PlayerCollection");
        var chars = pc.GetType().GetField("OwnedCharacters").GetValue(pc) as System.Collections.IList;
        if (chars != null && chars.Count > 0)
        {
            var first = chars[0];
            // M1：人物长期实例身份 = PersonId（不再把模板 CharacterId 当 PersonId）
            var id = first.GetType().GetField("PersonId").GetValue(first) as string;
            if (!string.IsNullOrEmpty(id)) return id;
        }
        var heroes = pc.GetType().GetField("OwnedHeroes").GetValue(pc) as System.Collections.IList;
        if (heroes != null && heroes.Count > 0)
        {
            var first = heroes[0];
            var id = first.GetType().GetField("PersonId").GetValue(first) as string;
            if (!string.IsNullOrEmpty(id)) return id;
        }
        Assert.Fail("PlayerCollection 应至少包含一个长期 Person");
        return null;
    }

    /// <summary>从真实运行时建筑派生正式实例岗位（building_farm Completed → workplace:&lt;InstanceId&gt;:main），不写死 ws_*。</summary>
    private static string InstanceWorkplace(string definitionId)
    {
        var runtime = GetBridgeBuildings();
        foreach (var b in runtime)
        {
            var type = b.GetType();
            if ((string)type.GetProperty("BuildingDefinitionId", IF).GetValue(b, null) != definitionId) continue;
            if (type.GetProperty("LifecycleState", IF).GetValue(b, null).ToString() != "Completed") continue;
            return "workplace:" + (string)type.GetProperty("InstanceId", IF).GetValue(b, null) + ":main";
        }
        Assert.Fail("正式默认世界必须存在 Completed " + definitionId + " 实例");
        return null;
    }

    private static VillageTime VillageTime()
    {
        return (VillageTime)GetBridgeField("Time");
    }

    private static void TickDays(int days)
    {
        var t = VillageTime();
        for (int i = 0; i < days; i++) t.TickDay();
    }

    private static VillagePersonPresence PresenceOf(string personId)
    {
        return (VillagePersonPresence)InvokeStatic(BridgeType, "GetPersonPresence", personId);
    }

    [UnityTest]
    public IEnumerator Presence_DerivedAndWorldVisible_WithoutAnyManagementPanel_And_SurvivesSimToggle()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        var town = EnterTown();
        yield return null;
        yield return null;

        string pid = FirstPersonId(town);
        // 正式实体语义：默认生成世界只有实例岗位（workplace:<InstanceId>:main），旧静态 ws_test_01 已退出正式目录。
        string farmWorkplace = InstanceWorkplace("building_farm");
        Assert.AreEqual(VillageAssignmentResult.Success,
            InvokeStatic(BridgeType, "TryAssignPerson", pid, farmWorkplace), "真实调度 API 指派 Primary Assignment");

        var presence = PresenceOf(pid);
        Assert.AreEqual(VillagePresenceState.Workplace, presence.State, "Presence 只从 Assignment 真相源派生");
        Assert.AreEqual(farmWorkplace, presence.PlaceId);

        // Debug 视图只随权威 Simulation 显示（未打开任何管理面板）：用于开发观察 Presence 数据链
        yield return new WaitForSeconds(0.7f);
        var layer = FindInstance("VillageWorldLayer");
        Assert.IsNotNull(layer, "场景运行时应存在 VillageWorldLayer Debug 宿主（VillageTimeDriver 运行时自托管）");
        Assert.IsTrue((bool)GetProperty(layer, "CanvasVisible"), "Simulation 激活时 World 画布应可见");
        string summary = (string)GetProperty(layer, "MarkerSummary");
        Assert.IsTrue(!string.IsNullOrEmpty(summary), "World 应已绘制 Presence / Place Marker");
        Assert.IsTrue(summary.Contains(pid), "World 摘要应包含该具体 Person");
        Assert.IsTrue(summary.Contains(farmWorkplace), "World 摘要应包含其 Workplace Place");

        // UI/Sim gating 关停（等价 DeckViewer / Expedition Preparation 的 sim 关停路径）：
        // World 视图停绘，但 Runtime 状态与 Place 注册表不变、不销毁、不重建
        int placesBefore = ((VillageWorldRegistry)GetBridgeField("World")).Places.Count;
        InvokeStatic(BridgeType, "SetSimulationActive", false);
        yield return null;
        Assert.IsFalse((bool)GetProperty(layer, "CanvasVisible"), "sim 关停 → 只停视图");
        Assert.AreEqual(placesBefore, ((VillageWorldRegistry)GetBridgeField("World")).Places.Count, "Place truth 不因 UI 关停变化");
        Assert.AreEqual(VillagePresenceState.Workplace, PresenceOf(pid).State, "Presence truth 不因 UI 关停变化");

        // 返回领地（sim 恢复）→ 同一 Runtime 状态直接恢复显示，无重建失效
        InvokeStatic(BridgeType, "SetSimulationActive", true);
        yield return new WaitForSeconds(0.7f);
        Assert.IsTrue((bool)GetProperty(layer, "CanvasVisible"), "sim 恢复 → World 视图恢复");
        string summary2 = (string)GetProperty(layer, "MarkerSummary");
        Assert.IsTrue(summary2.Contains(pid) && summary2.Contains(farmWorkplace), "恢复后仍指向同一 Person / 同一 Place");
    }

    [UnityTest]
    public IEnumerator ProjectCompletion_WorldSiteTrace_PersistsAfterSaveReload_WorldRestored()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        var town = EnterTown();
        yield return null;
        yield return null;

        string pid = FirstPersonId(town);
        object player = GetProperty(town, "PlayerData");

        Assert.AreEqual(VillageProjectResult.Success,
            InvokeStatic(BridgeType, "TryStartProject", "prj_test_02", new[] { pid }), "真实调度启动 Test Project");

        var projects = (VillageProjectSystem)GetBridgeField("Projects");
        Assert.AreEqual(1, projects.ActiveProjects.Count);
        string projectId = projects.ActiveProjects[0].ProjectId;

        TickDays(3); // prj_test_02：1 人 × 2/天 → 3 天达 target 6 完成
        Assert.AreEqual(0, projects.ActiveProjects.Count, "Project 完成并释放参与者");

        var world = (VillageWorldRegistry)GetBridgeField("World");
        string placeId = VillageWorldRegistry.BuildProjectSitePlaceId(projectId);
        Assert.IsTrue(world.ContainsPlace(placeId), "完成点应单向追加 ProjectSite 痕迹");

        var saveData = (VillageSaveData)InvokeStatic(BridgeType, "BuildSaveData");
        Assert.IsNotNull(saveData.World, "存档应含 World 区块");
        Assert.IsTrue(saveData.World.Places != null && saveData.World.Places.Count == 1, "只持久化 ProjectSite（Workplace 派生不落档）");
        Assert.AreEqual(placeId, saveData.World.Places[0].PlaceId);

        Assert.IsTrue((bool)InvokeStatic(SaveSystemType, "Save", player), "真实存档路径写入");

        // 全量重载：Reset 运行时 → 重新加载真实场景 → Town 从存档恢复 Village 区块
        ResetBridge();
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        town = EnterTown(false); // 真实读档路径：不得在 reload 前再 NewGame 覆盖刚保存的 ProjectSite 痕迹
        yield return null;
        yield return null;

        var world2 = (VillageWorldRegistry)GetBridgeField("World");
        Assert.IsTrue(world2.ContainsPlace(placeId), "Save / Reload 后 ProjectSite 痕迹仍在");
        Assert.AreEqual(0, ((VillageProjectSystem)GetBridgeField("Projects")).ActiveProjects.Count, "已完成 Project 不恢复为 Active");

        yield return new WaitForSeconds(0.7f);
        var layer = FindInstance("VillageWorldLayer");
        Assert.IsNotNull(layer, "重载后场景仍有 VillageWorldLayer 根宿主");
        Assert.IsTrue((bool)GetProperty(layer, "CanvasVisible"), "重载进入领地后 World 画布应可见");
        Assert.IsTrue(((string)GetProperty(layer, "MarkerSummary")).Contains(placeId), "World 应显示持久痕迹 Place");
    }

    [UnityTest]
    public IEnumerator MainMenuPhase_DebugViewHidden_UntilTownSimulationActive()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        yield return null;

        // Main Menu 前台阶段：权威 Village Simulation 必须为 false（MainMenuUI 为外层产品门禁），
        // VillageWorldLayer Debug 视图不得显示（无论 TownUI.Start 是否先于 MainMenu.Start 执行过）
        Assert.IsFalse((bool)GetBridgeField("IsSimulationActive"), "Main Menu 阶段权威 sim 必须为 false");
        var layerAtMenu = FindInstance("VillageWorldLayer");
        bool hiddenAtMenu = layerAtMenu == null || !(bool)GetProperty(layerAtMenu, "CanvasVisible");
        Assert.IsTrue(hiddenAtMenu, "Main Menu 阶段 Village World Debug 视图不得显示");

        // 进入领地：TownUI.Initialize → 权威 sim=true → Debug 视图（仅开发观察）随之显示
        var town = EnterTown();
        yield return null;
        yield return null;
        Assert.IsTrue((bool)GetBridgeField("IsSimulationActive"), "进入领地后权威 sim=true");

        yield return new WaitForSeconds(0.7f);
        var layer = FindInstance("VillageWorldLayer");
        Assert.IsNotNull(layer, "进入领地后应存在 Debug 宿主");
        Assert.IsTrue((bool)GetProperty(layer, "CanvasVisible"), "进入领地后 Debug 视图显示");
    }

    // ==================== 用户单槽持久保护（先捕获 / 异常恢复 / finally 恢复 / 未完成阻断） ====================

    private static string RepoRoot() => System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", "..", ".."));

    private sealed class SaveSlotGuard
    {
        private const string ManifestName = "manifest.json";
        private const string BackupName = "original-save.bin";
        [System.Serializable] private sealed class ManifestStatus { public string runId; public string status; }
        private string _path, _runId, _runDir, _binPath, _captureUtc, _beforeHash;
        private bool _existed, _captured, _restored;
        private byte[] _bytes;
        public string RunId => _runId;
        public bool Restored => _restored;

        private static string ProtectionRoot()
            => System.IO.Path.Combine(RepoRoot(), "Development/StudioRuns/Initialization-20261027/save-guard".Replace('/', System.IO.Path.DirectorySeparatorChar));

        public void Capture()
        {
            Assert.IsFalse(_captured, "SaveSlotGuard 同一实例只能 Capture 一次");
            BlockOnUnfinishedRuns();
            _path = (string)InvokeStatic(SaveSystemType, "GetSavePath");
            Assert.IsFalse(string.IsNullOrEmpty(_path), "SaveSystem.GetSavePath 必须返回真实路径");
            _captureUtc = System.DateTime.UtcNow.ToString("O");
            _runId = System.DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
            string root = ProtectionRoot();
            _runDir = System.IO.Path.Combine(root, _runId);
            for (int i = 2; System.IO.Directory.Exists(_runDir); i++) _runDir = System.IO.Path.Combine(root, _runId + "-" + i);
            _runId = System.IO.Path.GetFileName(_runDir);
            System.IO.Directory.CreateDirectory(_runDir);
            _binPath = System.IO.Path.Combine(_runDir, BackupName);
            _existed = System.IO.File.Exists(_path);
            _bytes = _existed ? System.IO.File.ReadAllBytes(_path) : null;
            _beforeHash = _existed ? HashBytes(_bytes) : "absent";
            if (_existed)
            {
                using (var stream = new System.IO.FileStream(_binPath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.None))
                    stream.Write(_bytes, 0, _bytes.Length);
                byte[] readback = System.IO.File.ReadAllBytes(_binPath);
                Assert.AreEqual(_bytes.LongLength, readback.LongLength, "备份字节数回读不一致");
                Assert.AreEqual(_beforeHash, HashBytes(readback), "备份回读 SHA256 不一致");
                Assert.AreEqual(_beforeHash, HashFile(_path), "捕获后当前档已被改写，拒绝继续");
            }
            WriteManifest(_existed ? "captured" : "absent", null);
            _captured = true;
            Debug.Log("[VillageWorldProductIntegrationPlayTests][SAVE-CAPTURE] runId=" + _runId + " existed=" + _existed
                + " bytes=" + (_bytes == null ? 0 : _bytes.LongLength) + " sha256=" + _beforeHash);
        }

        public void Restore()
        {
            Assert.IsTrue(_captured, "SaveSlotGuard.Capture 未成功完成，禁止 Restore");
            if (_restored)
            {
                bool stillOk = System.IO.File.Exists(_path) == _existed && (!_existed || HashFile(_path) == _beforeHash);
                if (stillOk) return;
            }
            byte[] source = null;
            if (_existed)
            {
                Assert.IsTrue(System.IO.File.Exists(_binPath), "本次持久备份缺失，拒绝恢复：" + _binPath);
                source = System.IO.File.ReadAllBytes(_binPath);
                Assert.AreEqual(_beforeHash, HashBytes(source), "本次备份回读 SHA256 不一致，拒绝写回");
            }
            try
            {
                if (_existed) System.IO.File.WriteAllBytes(_path, source);
                else if (System.IO.File.Exists(_path)) System.IO.File.Delete(_path);
                bool exists = System.IO.File.Exists(_path);
                string afterHash = exists ? HashFile(_path) : "absent";
                bool ok = exists == _existed && (!_existed || afterHash == _beforeHash);
                _restored = ok;
                WriteManifest(ok ? "restored" : "restore-failed", ok ? null : "sha256 mismatch after write");
                Assert.IsTrue(ok, "单槽存档恢复失败（backup/manifest 已保留）：期望 existed=" + _existed
                    + " sha256=" + _beforeHash + "，实际 existed=" + exists + " sha256=" + afterHash);
            }
            catch (System.Exception ex)
            {
                if (!_restored) WriteManifest("restore-failed", ex.GetType().Name + ": " + ex.Message);
                throw;
            }
        }

        private static void BlockOnUnfinishedRuns()
        {
            string root = ProtectionRoot();
            if (!System.IO.Directory.Exists(root)) return;
            var unfinished = new List<string>();
            foreach (string dir in System.IO.Directory.GetDirectories(root))
            {
                string manifest = System.IO.Path.Combine(dir, ManifestName);
                if (!System.IO.File.Exists(manifest)) { unfinished.Add(System.IO.Path.GetFileName(dir) + "(no-manifest)"); continue; }
                ManifestStatus parsed = null;
                try { parsed = JsonUtility.FromJson<ManifestStatus>(System.IO.File.ReadAllText(manifest)); }
                catch (System.Exception) { }
                string status = parsed == null ? null : parsed.status;
                if (status == "restored" || status == "protective-snapshot") continue;
                unfinished.Add(System.IO.Path.GetFileName(dir) + "(" + (status ?? "unreadable") + ")");
            }
            Assert.AreEqual(0, unfinished.Count,
                "存在未完成的存档保护 run，已阻止新测试以免覆盖当前档；请人工核对后再继续："
                + string.Join("; ", unfinished) + " @ " + root);
        }

        private void WriteManifest(string status, string error)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("{\n");
            sb.Append("  \"runId\": \"" + JsonText(_runId) + "\",\n");
            sb.Append("  \"status\": \"" + JsonText(status) + "\",\n");
            sb.Append("  \"captureUtc\": \"" + JsonText(_captureUtc) + "\",\n");
            sb.Append("  \"updatedUtc\": \"" + JsonText(System.DateTime.UtcNow.ToString("O")) + "\",\n");
            sb.Append("  \"sourcePath\": \"" + JsonText(_path) + "\",\n");
            sb.Append("  \"sourceExisted\": " + (_existed ? "true" : "false") + ",\n");
            sb.Append("  \"sourceBytes\": " + (_bytes == null ? 0 : _bytes.LongLength) + ",\n");
            sb.Append("  \"sourceSha256\": \"" + JsonText(_beforeHash) + "\",\n");
            sb.Append("  \"backupFile\": " + (_existed ? "\"" + BackupName + "\"" : "null") + ",\n");
            sb.Append("  \"backupBytes\": " + (_existed && System.IO.File.Exists(_binPath) ? new System.IO.FileInfo(_binPath).Length : 0) + ",\n");
            sb.Append("  \"backupSha256\": " + (_existed ? "\"" + JsonText(_beforeHash) + "\"" : "null") + ",\n");
            sb.Append("  \"readbackVerified\": " + (_existed ? "true" : "false") + ",\n");
            sb.Append("  \"restored\": " + (_restored ? "true" : "false") + ",\n");
            sb.Append("  \"lastError\": " + (error == null ? "null" : "\"" + JsonText(error) + "\"") + ",\n");
            sb.Append("  \"capturedBy\": \"VillageWorldProductIntegrationPlayTests.SaveSlotGuard (test-only helper)\",\n");
            sb.Append("  \"note\": \"只允许用本 runID 的 original-save.bin 恢复；进程 abort 时 TearDown 不执行，备份留待人工取证。\"\n");
            sb.Append("}\n");
            System.IO.File.WriteAllText(System.IO.Path.Combine(_runDir, ManifestName), sb.ToString());
        }

        private static string JsonText(string value) => value == null ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static string HashBytes(byte[] bytes)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
            return System.BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string HashFile(string path)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        using (var stream = System.IO.File.OpenRead(path))
            return System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
    }
}
}
