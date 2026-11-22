using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace RRPG.Village.Tests
{
/// <summary>
/// V2C 产品集成验证（真实 Town 产品路径 + VillageCommitmentUI Functional UI）。
///
/// 从真实场景验证：
///   1. MainMenu→Town 场景中打开 Commitment 面板后 Person 列表来自真实 PlayerCollection；
///   2. UI 控制器操作（指派岗位 / 启动 Test Project）真实调用 VillageRuntimeBridge / Runtime 系统；
///   3. 面板打开期间 Village 模拟仍推进（UI 不暂停 Simulation；Project Progress 随时间增长，
///      Assignment 贡献暂停）；
///   4. Project Completion 后 UI/Runtime 显示自动解除占用并恢复原 Assignment 执行；
///   5. Save/Load 后 UI 重新反映一致状态。
/// 团结引擎限制：UI/TownUI/SaveSystem（Assembly-CSharp）经反射；Village.Runtime 强类型。
/// </summary>
public partial class VillageCommitmentProductIntegrationPlayTests
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags IF = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static System.Type BridgeType => System.Type.GetType("VillageRuntimeBridge, Assembly-CSharp");
    private static System.Type SaveSystemType => System.Type.GetType("SaveSystem, Assembly-CSharp");
    private static System.Type UiType => System.Type.GetType("VillageCommitmentUI, Assembly-CSharp");

    [SetUp]
    public void Reset()
    {
        // 先保护用户单槽（持久快照 + 回读校验），再用真实 NewGame 建立本用例的隔离世界。
        _guard = new SaveSlotGuard();
        _guard.Capture();
        bool completed = false;
        try
        {
            ResetBridge();
            // 注意：NewGame 必须在场景加载后执行（DataManager / 场景宿主就绪）；由 EnterTown() 统一建立隔离世界。
            completed = true;
        }
        finally
        {
            if (!completed) RestoreAfterFailedSetup();
        }
    }

    private void RestoreAfterFailedSetup()
    {
        if (_guard == null) return;
        try { _guard.Restore(); }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[VillageCommitmentProductIntegrationPlayTests] SetUp 失败后的存档恢复失败（不掩盖原始异常）：" + ex.GetType().Name + ": " + ex.Message);
        }
    }

    [TearDown]
    public void Cleanup()
    {
        try { _guard?.Restore(); }
        finally
        {
            // 先按真实 UI 语义关闭仍打开的面板，再执行测试专用运行时清空。
            // VillageRuntimeBridge.ResetForTests 会把 Assignments / Time 等置 null；若面板仍 _isOpen，
            // 下一次 Update→Refresh 会解引用 null 运行时并抛出 Unity 未处理 NRE
            //（真实栈：VillageCommitmentUI.RebuildWorkplaceRows:313 ← Refresh:251 ← Update:82）。
            // 该状态不是玩家可达生命周期（产品路径不会在面板打开时清空运行时），
            // 属于夹具顺序问题；修复方式是补上真实 Hide，而不是 LogAssert.Expect / 忽略异常 / 清日志。
            HideCommitmentUi();
            ResetBridge();
        }
    }

    private static void HideCommitmentUi()
    {
        var ui = FindInstance("VillageCommitmentUI");
        if (ui == null) return;
        InvokeInstance(ui, "Hide");
    }

    // ==================== 用户单槽持久保护 ====================

    private SaveSlotGuard _guard;

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
            Debug.Log("[VillageCommitmentProductIntegrationPlayTests][SAVE-CAPTURE] runId=" + _runId + " existed=" + _existed
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
            sb.Append("  \"capturedBy\": \"VillageCommitmentProductIntegrationPlayTests.SaveSlotGuard (test-only helper)\",\n");
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

    // ==================== 正式实例岗位（从运行时建筑找 ID，不写死 ws_*） ====================

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

    // ==================== helpers ====================

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
        Assert.IsNotNull(selected, "方法不存在或参数不匹配：" + method + " @ " + type.FullName);
        return selected.Invoke(null, args);
    }

    /// <summary>
    /// 读取 Bridge 的静态成员：先按真实成员签名解析属性，再回退字段。
    /// 现有真实签名：`Buildings` / `IntradayWork` 是 property（get; private set;），
    /// `Time` / `Assignments` / `Projects` / `Resources` / `World` / `State` 是 field。
    /// 产品 property 不得为了测试改成 field；成员不存在时必须断言失败，不得静默返回 null。
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
    /// 真实正式建筑运行时（VillageRuntimeBridge.Buildings 是 property，按字段读取必然落空）。
    /// 存在性 / 可读性 / 已初始化全部显式断言，避免下一次同类字段-属性错误。
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

    private static object InvokeInstance(object instance, string method, params object[] args)
    {
        var m = instance.GetType().GetMethod(method, IF);
        Assert.IsNotNull(m, "实例方法不存在：" + method + " @ " + instance.GetType().Name);
        return m.Invoke(instance, args);
    }

    private static object GetInstanceField(object instance, string name)
    {
        var f = instance.GetType().GetField(name, IF);
        Assert.IsNotNull(f, "实例字段不存在：" + name + " @ " + instance.GetType().Name);
        return f.GetValue(instance);
    }

    private static object GetProperty(object instance, string name)
    {
        var p = instance.GetType().GetProperty(name, IF);
        Assert.IsNotNull(p, "属性不存在：" + name + " @ " + instance.GetType().Name);
        return p.GetValue(instance, null);
    }

    // ==================== 进入真实 Town（场景宿主） ====================

    private static object EnterTown()
    {
        var town = FindInstance("TownUI");
        Assert.IsNotNull(town, "场景应有 TownUI");
        var mm = FindInstance("MainMenuUI");
        Assert.IsNotNull(mm, "场景应有 MainMenuUI");

        // 进入领地必须走真实产品入口：Main Menu 前台（sim=false 门禁）→ 玩家点击主菜单按钮 →
        // MainMenuUI.EnterTown(collection) → TownUI.Initialize → SetSimulationActive(true)。
        // 不得再用 SetActive(true) 依赖修复前「TownUI.Start 提前把 sim 置 true、MainMenu 不再复位」的旧行为。
        // 测试 initialization：显式建立隔离世界（用户原档已由 SaveSlotGuard 捕获，TearDown 恢复），
        // 不再删除用户单槽；随后仍走真实产品路径 OnContinue 进入 Town。
        var fresh = InvokeStatic(SaveSystemType, "NewGame");
        Assert.IsNotNull(fresh, "进入领地前置：应能创建新档数据");
        Assert.IsTrue((bool)InvokeStatic(SaveSystemType, "Save", fresh), "进入领地前置：新档应能写入真实存档");
        InvokeInstance(mm, "OnContinue"); // 产品路径：继续游戏 → 读档 → EnterTown → Initialize
        return FindInstance("TownUI");
    }

    private static object FindUi()
    {
        var ui = FindInstance("VillageCommitmentUI");
        Assert.IsNotNull(ui, "场景应有 VillageCommitmentUI（Editor Authoring 构建）");
        return ui;
    }

    private static string FirstPersonId(object town)
    {
        var pc = GetProperty(town, "PlayerData");
        Assert.IsNotNull(pc, "Town 应已持有真实 PlayerCollection（NewGame 默认）");

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

    [UnityTest]
    public IEnumerator VillageTMP_LabelsRender_AndKitchenRowsRemainStableAcrossRefresh()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null; yield return null;
        var town = EnterTown();
        yield return null; yield return null;
        // Prototype labels remain available only through explicit compatibility mode.
        InvokeInstance(FindInstance("HomeProductShell"), "ToggleCompatibility");
        yield return null; yield return null;
        var hud = FindInstance("VillageHUD");
        Assert.IsNotNull(hud);
        var authorityType = System.Type.GetType("CurrentUIFontAuthority, Assembly-CSharp");
        var font = (TMP_FontAsset)authorityType.GetProperty("Default").GetValue(null);
        var townTransform = ((Component)town).transform;
        foreach (string path in new[] { "TownCanvas/VillageHUD/VillageDateText", "TownCanvas/VillageHUD/BtnPause/Label", "TownCanvas/VillageHUD/BtnSpeed/Label", "TownCanvas/BtnViewDeck/Text (Legacy)" })
        {
            var text = townTransform.Find(path).GetComponent<TMP_Text>();
            Assert.AreSame(font, text.font, path + " uses shared font authority");
            text.ForceMeshUpdate();
            Assert.Greater(text.textInfo.characterCount, 0, path);
            for (int i = 0; i < text.textInfo.characterCount; i++)
            {
                var c = text.textInfo.characterInfo[i];
                if (!char.IsWhiteSpace(c.character)) Assert.IsTrue(c.isVisible, path + " character must render: " + c.character);
            }
        }
        var pause = (Button)GetInstanceField(hud, "btnPause");
        var speed = (Button)GetInstanceField(hud, "btnSpeed");
        var pauseLabel = (TMP_Text)GetInstanceField(hud, "pauseLabel");
        var speedLabel = (TMP_Text)GetInstanceField(hud, "speedLabel");
        pause.onClick.Invoke(); yield return null;
        Assert.AreEqual("继续", pauseLabel.text);
        pause.onClick.Invoke(); yield return null;
        Assert.AreEqual("暂停", pauseLabel.text);
        speed.onClick.Invoke(); yield return null;
        Assert.AreEqual("x2", speedLabel.text);
        speed.onClick.Invoke(); yield return null;
        Assert.AreEqual("x" + (int)VillageTime.VillageTimeSpeed.Fast, speedLabel.text);
        foreach (var label in new[] { "x8", "x12", "x24" })
        {
            speed.onClick.Invoke(); yield return null;
            Assert.AreEqual(label, speedLabel.text);
        }
        speed.onClick.Invoke(); yield return null;
        Assert.AreEqual("x0.5", speedLabel.text);
        speed.onClick.Invoke(); yield return null;
        Assert.AreEqual("x1", speedLabel.text);
        string evidence = System.Environment.GetEnvironmentVariable("RRPG_VILLAGE_UI_EVIDENCE");
        if (!string.IsNullOrEmpty(evidence)) ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(evidence, "town-labels-fixed.png"));
        yield return null; yield return null;

        var ui = FindUi(); InvokeInstance(ui, "Show");
        yield return null; yield return null;
        string pid = FirstPersonId(town);
        // 正式实体语义：默认世界只有实例岗位（workplace:<InstanceId>:main），没有静态 ws_*。
        string kitchenWorkplace = InstanceWorkplace("building_kitchen");
        int kitchenCapacity = ((VillageAssignmentSystem)GetBridgeField("Assignments")).GetWorkplaceDef(kitchenWorkplace).Capacity;
        Assert.Greater(kitchenCapacity, 0, "默认 Kitchen 实例岗位容量必须来自既有 Profile");
        InvokeInstance(ui, "OnPersonClicked", pid);
        var workplaces = (RectTransform)GetInstanceField(ui, "workplaceListContainer");
        var kitchen = workplaces.Find("WorkplaceRow_" + kitchenWorkplace);
        Assert.IsNotNull(kitchen, "管理/承诺 UI 必须显示默认 Kitchen 实例岗位行：" + kitchenWorkplace);
        int kitchenId = kitchen.GetInstanceID();
        var personContainer = (RectTransform)GetInstanceField(hud, "availabilityContainer");
        var person = personContainer.Find("VillagePersonRow_" + pid);
        int personId = person.GetInstanceID();
        InvokeInstance(ui, "OnAssignClicked", kitchenWorkplace);
        yield return null; yield return null;
        StringAssert.Contains("1 / " + kitchenCapacity, kitchen.GetComponent<TMP_Text>().text);
        var kitchenPosition = ((RectTransform)kitchen).anchoredPosition;
        for (int frame = 0; frame < 90; frame++)
        {
            InvokeInstance(ui, "Refresh");
            yield return null;
            Assert.AreEqual(kitchenId, workplaces.Find("WorkplaceRow_" + kitchenWorkplace).GetInstanceID());
            Assert.AreEqual(kitchenPosition, ((RectTransform)kitchen).anchoredPosition, "Kitchen row must not jump during unchanged refresh");
            Assert.AreEqual(personId, personContainer.Find("VillagePersonRow_" + pid).GetInstanceID());
            Assert.IsTrue(kitchen.gameObject.activeInHierarchy);
        }
        InvokeInstance(ui, "OnRemoveAssignmentClicked"); yield return null;
        Assert.AreEqual(kitchenId, workplaces.Find("WorkplaceRow_" + kitchenWorkplace).GetInstanceID());
        StringAssert.Contains("0 / " + kitchenCapacity, kitchen.GetComponent<TMP_Text>().text);
        if (!string.IsNullOrEmpty(evidence)) ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(evidence, "commitment-kitchen-stable.png"));
        yield return null; yield return null;
    }

    // ==================== Test 1：真实 Town 闭环 ====================

    [UnityTest]
    public IEnumerator CommitmentUI_RealTown_AssignAndProject_ClosedLoop_PanelDoesNotPauseSimulation()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;

        var town = EnterTown();
        yield return null;
        yield return null;

        var ui = FindUi();
        var root = (GameObject)GetInstanceField(ui, "root");
        Assert.IsFalse(root.activeInHierarchy, "面板默认隐藏，不阻挡 Town 主界面");

        // 打开面板
        InvokeInstance(ui, "Show");
        yield return null;
        yield return null;
        Assert.IsTrue(root.activeInHierarchy, "Show() 后面板应可见");

        // Person 列表来自真实长期人物（真实 Town 数据）
        var personList = (RectTransform)GetInstanceField(ui, "personListContainer");
        Assert.IsTrue(personList.childCount > 0, "Commitment 面板应显示真实 Person 列表");
        string pid = FirstPersonId(town);
        Assert.IsFalse(string.IsNullOrEmpty(pid));

        // —— 指派 Primary Assignment（UI 控制器 → VillageRuntimeBridge）——
        // 正式实体语义：默认世界只有实例岗位；旧静态 ws_test_01 已退出正式目录。
        string primaryWorkplace = InstanceWorkplace("building_farm");
        InvokeInstance(ui, "OnPersonClicked", pid);
        InvokeInstance(ui, "OnAssignClicked", primaryWorkplace);
        yield return null;

        var assignments = (VillageAssignmentSystem)GetBridgeField("Assignments");
        var record = assignments.GetRecord(pid);
        Assert.IsNotNull(record, "UI 指派后 Runtime 应存在 Primary Assignment");
        Assert.AreEqual(primaryWorkplace, record.WorkplaceId);

        // —— 启动 Test Project（UI 勾选 participant → Runtime）——
        var chosen = (HashSet<string>)GetInstanceField(ui, "_chosenParticipants");
        chosen.Add(pid);
        InvokeInstance(ui, "OnStartProjectClicked");
        yield return null;

        var projects = (VillageProjectSystem)GetBridgeField("Projects");
        Assert.AreEqual(1, projects.ActiveProjects.Count, "UI 启动后应存在 1 个 Active Project");
        Assert.IsTrue(projects.IsPersonInActiveProject(pid), "参与者应进入 Active Project");

        // —— 面板打开期间模拟持续；Project 推进、Assignment 暂停 ——
        var time = (VillageTime)GetBridgeField("Time");
        time.SecondsPerDay = 10000f; // Isolate elapsed-frame jitter; advance through the real driver below.
        for (int i = 0; i < 7; i++) yield return AdvanceOneDayThroughDriver(time);
        var progress = projects.ActiveProjects[0].Progress;
        Assert.GreaterOrEqual(progress, 6, "Project 应随时间推进（模拟未被面板暂停）");
        Assert.AreEqual(0, record.TotalExecutedDays, "Project 占用期间 Assignment 不得执行");

        // —— Project 完成：自动解除占用；随后原岗位自动恢复执行 ——
        for (int i = 0; i < 3; i++) yield return AdvanceOneDayThroughDriver(time);
        Assert.AreEqual(0, projects.ActiveProjects.Count, "完成后应解除占用");
        Assert.IsTrue(projects.IsProjectCompleted(projects.CompletedProjects[projects.CompletedProjects.Count - 1].ProjectId));
        yield return AdvanceOneDayThroughDriver(time);
        Assert.GreaterOrEqual(assignments.GetRecord(pid).TotalExecutedDays, 1,
            "Project 完成后原 Primary Assignment 应自动恢复实际执行");
    }

    // ==================== Test 2：Save/Load 后 UI 重新反映一致状态 ====================

    [UnityTest]
    public IEnumerator CommitmentUI_ReflectsRuntimeAfterSaveLoad_MidProject()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;

        var town = EnterTown();
        yield return null;
        yield return null;

        var ui = FindUi();
        InvokeInstance(ui, "Show");
        yield return null;
        yield return null;

        string pid = FirstPersonId(town);
        string primaryWorkplace = InstanceWorkplace("building_farm");
        InvokeInstance(ui, "OnPersonClicked", pid);
        InvokeInstance(ui, "OnAssignClicked", primaryWorkplace);
        var chosen = (HashSet<string>)GetInstanceField(ui, "_chosenParticipants");
        chosen.Add(pid);
        InvokeInstance(ui, "OnStartProjectClicked");
        yield return null;

        // 让项目推进几天（中途状态）
        var time = (VillageTime)GetBridgeField("Time");
        time.SecondsPerDay = 10000f;
        for (int i = 0; i < 2; i++) yield return AdvanceOneDayThroughDriver(time);

        var projectsBefore = (VillageProjectSystem)GetBridgeField("Projects");
        Assert.AreEqual(1, projectsBefore.ActiveProjects.Count);
        var midProgress = projectsBefore.ActiveProjects[0].Progress;
        Assert.GreaterOrEqual(midProgress, 1, "保存前项目应有进度");

        // 真实保存链路（带 PlayerCollection 与 Village/Project 状态）
        object pc = GetProperty(town, "PlayerData");
        Assert.IsTrue((bool)InvokeStatic(SaveSystemType, "Save", pc), "Save 应成功");

        // 模拟重启后 Load（bridge 重建）
        ResetBridge();
        object loaded = InvokeStatic(SaveSystemType, "Load");
        Assert.IsNotNull(loaded, "Load 应成功");
        ((VillageTime)GetBridgeField("Time")).SecondsPerDay = 10000f;

        // UI 重新刷新：应重新读取 Runtime（不再用旧的 progress 缓存）
        InvokeInstance(ui, "Refresh");
        yield return null;
        yield return null;

        var projectsAfter = (VillageProjectSystem)GetBridgeField("Projects");
        Assert.AreEqual(1, projectsAfter.ActiveProjects.Count, "Load 后 Active Project 应恢复");
        Assert.AreEqual(midProgress, projectsAfter.ActiveProjects[0].Progress, "Load 后 Progress 应与保存一致");
        Assert.IsTrue(projectsAfter.IsPersonInActiveProject(pid));

        var assignmentsAfter = (VillageAssignmentSystem)GetBridgeField("Assignments");
        var recordAfter = assignmentsAfter.GetRecord(pid);
        Assert.IsNotNull(recordAfter, "Load 后 Primary Assignment 应恢复");
        Assert.AreEqual(primaryWorkplace, recordAfter.WorkplaceId);
        Assert.AreEqual(0, recordAfter.TotalExecutedDays, "Project 占用关系应保持（Assignment 未执行）");

        // UI 行反映 Runtime：Active Project 进度行存在且包含进度文本
        var activeList = (RectTransform)GetInstanceField(ui, "activeProjectListContainer");
        Assert.IsTrue(activeList.childCount > 0, "UI 应显示进行中的 Project 进度行");
        bool hasProgressText = false;
        foreach (Transform child in activeList)
        {
            var text = child.GetComponent<TMP_Text>();
            if (text != null && text.text.Contains("/ 10")) { hasProgressText = true; break; }
        }
        Assert.IsTrue(hasProgressText, "UI Project 进度行应反映 Runtime Progress（10 为 Test Project 目标）");

        // 关闭面板不影响 Runtime 与已恢复状态
        InvokeInstance(ui, "Hide");
        yield return null;
        var root = (GameObject)GetInstanceField(ui, "root");
        Assert.IsFalse(root.activeInHierarchy, "Hide() 后应隐藏");
        Assert.IsTrue(projectsAfter.IsPersonInActiveProject(pid), "隐藏面板不改变 Runtime 占用关系");
    }

    // Exercise the actual Town frame driver without assuming seven days fit a wall-clock wait.
    static IEnumerator AdvanceOneDayThroughDriver(VillageTime time)
    {
        long target=time.CurrentInstant.Minute+1440;
        float previousRate=time.SecondsPerDay;
        time.SecondsPerDay=0.2f;
        try
        {
            for(int frame=0;time.CurrentInstant.Minute<target && frame<300;frame++) yield return null;
            Assert.GreaterOrEqual(time.CurrentInstant.Minute,target,"真实 Driver 必须经过劳动/午餐边界，不 Restore 到午夜");
        }
        finally {time.SecondsPerDay=previousRate;}

    }

    // ==================== 可见性 / Modal 辅助 ====================

    private static Transform FindDescendant(Transform t, string name)
    {
        if (t.name == name) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            var r = FindDescendant(t.GetChild(i), name);
            if (r != null) return r;
        }
        return null;
    }

    private static List<GameObject> RowChildren(RectTransform container)
    {
        var list = new List<GameObject>();
        if (container == null) return list;
        foreach (Transform child in container)
        {
            if (child != null && child.name != "RecentlyCompleted") list.Add(child.gameObject);
        }
        return list;
    }

    private static Rect WorldRectOf(RectTransform rt)
    {
        var corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    private static bool Overlaps(Rect a, Rect b)
    {
        return a.xMax > b.xMin && a.xMin < b.xMax && a.yMax > b.yMin && a.yMin < b.yMax;
    }

    private static void AssertRowVisible(GameObject row, Rect viewportWorld, string context)
    {
        var rt = row.GetComponent<RectTransform>();
        Assert.IsNotNull(rt, context + " 行应有 RectTransform");
        var rect = WorldRectOf(rt);
        Assert.IsTrue(rect.width > 1f, context + " 行宽度应 > 0：" + rect.width);
        Assert.IsTrue(rect.height > 1f, context + " 行高度应 > 0：" + rect.height);
        Assert.IsTrue(row.activeInHierarchy, context + " 行应 activeInHierarchy");

        var text = row.GetComponent<TMP_Text>();
        Assert.IsNotNull(text, context + " 行应有 TMP_Text：" + row.name);
        Assert.IsNotNull(text.font, context + " 行字体不能为空（否则不可见）：" + row.name);
        Assert.IsTrue(text.color.a > 0f, context + " 行文字 alpha 应 > 0");
        Assert.IsFalse(string.IsNullOrEmpty(text.text), context + " 行文本不应为空");
        Assert.IsNotNull(row.GetComponent<CanvasRenderer>(), context + " 行应有 CanvasRenderer");

        // 必须与 Viewport 可视区域相交（未被 Mask 裁光/滚动到视野外/压缩成 0）
        Assert.IsTrue(Overlaps(rect, viewportWorld),
            context + " 行可视 Rect 应与 Viewport 可视 Rect 相交（被裁/被压缩/在视野外）：" + row.name);
    }

    private static GameObject TopHitAt(Vector2 screenPoint)
    {
        var es = Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>(true);
        if (es == null) return null;
        var data = new UnityEngine.EventSystems.PointerEventData(es) { position = screenPoint };
        var results = new List<UnityEngine.EventSystems.RaycastResult>();
        es.RaycastAll(data, results);
        return results.Count > 0 ? results[0].gameObject : null;
    }

    private static bool IsChildOf(GameObject go, string rootName)
    {
        var t = go != null ? go.transform : null;
        while (t != null)
        {
            if (t.name == rootName) return true;
            t = t.parent;
        }
        return false;
    }

    // ==================== Test 3：打开 Modal 后底层 Town 不可点击，关闭后恢复 ====================

    [UnityTest]
    public IEnumerator CommitmentModal_BlocksTownButtons_WhenOpen_AndRestoresAfterClose()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        EnterTown();
        yield return null;
        yield return null;

        var ui = FindUi();
        var root = (GameObject)GetInstanceField(ui, "root");
        var btnExpedition = GameObject.Find("Navigate5");
        Assert.IsNotNull(btnExpedition, "Home 应有远征导航");
        var rt = btnExpedition.GetComponent<RectTransform>();
        Vector2 center = (Vector2)rt.TransformPoint(new Vector3(rt.rect.center.x, rt.rect.center.y, 0f));

        // 关闭状态：不应命中 Modal，Town 保持可交互
        var hitClosed = TopHitAt(center);
        Assert.IsTrue(hitClosed != null && !IsChildOf(hitClosed, "PanelRoot"),
            "关闭后点击不应命中 Modal（命中："
            + (hitClosed != null ? hitClosed.name : "null") + "）");
        Assert.IsFalse(IsChildOf(hitClosed, "Blocker"), "关闭后 Blocker 不应存在/拦截");

        // 打开 Modal：应命中 PanelRoot/Blocker（遮罩层），而不是 Town 按钮
        InvokeInstance(ui, "Show");
        yield return null;
        yield return null;
        Assert.IsTrue(root.activeInHierarchy, "Modal 应打开");
        var hitOpen = TopHitAt(center);
        Assert.IsTrue(hitOpen != null && IsChildOf(hitOpen, "PanelRoot"),
            "打开 Modal 后点击应命中 PanelRoot/Blocker，不能点到 Town 按钮（命中："
            + (hitOpen != null ? hitOpen.name : "null") + "）");
        Assert.IsFalse(IsChildOf(hitOpen, "Navigate5"),
            "打开 Modal 后 BtnExpedition 不得响应点击");

        // 关闭后恢复（不再拦截）
        InvokeInstance(ui, "Hide");
        yield return null;
        var hitAgain = TopHitAt(center);
        Assert.IsTrue(hitAgain != null && !IsChildOf(hitAgain, "PanelRoot"),
            "关闭后点击应恢复为不命中 Modal（命中："
            + (hitAgain != null ? hitAgain.name : "null") + "）");
    }

    // ==================== Test 4：动态行在 Game View 几何可见 ====================

    [UnityTest]
    public IEnumerator CommitmentRows_AreGeometricallyVisible_InGameView()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        EnterTown();
        yield return null;
        yield return null;

        var ui = FindUi();
        InvokeInstance(ui, "Show");
        yield return null;
        yield return null;

        var uiTransform = ((Component)ui).transform;
        var scrollGo = FindDescendant(uiTransform, "ScrollView");
        Assert.IsNotNull(scrollGo, "应存在 ScrollView");
        var scrollRect = scrollGo.GetComponent<ScrollRect>();
        Assert.IsNotNull(scrollRect);
        var viewportTransform = FindDescendant(scrollGo, "Viewport");
        Assert.IsNotNull(viewportTransform, "应存在 Viewport");
        var viewportWorld = WorldRectOf(viewportTransform.GetComponent<RectTransform>());
        Assert.IsTrue(viewportWorld.width > 10f && viewportWorld.height > 10f, "Viewport 可视 Rect 非零");

        // 各动态区至少有一行，且在某滚动位置下与 Viewport 真实相交（未被裁/压/移出视野）
        var containers = new Dictionary<string, string>
        {
            { "PersonList", "Person 行" },
            { "WorkplaceList", "Workplace 分配行" },
            { "ProjectDefList", "Project 定义行" },
            { "ParticipantList", "Participant 勾选行" },
            { "ActiveProjectList", "ActiveProject 进度行" }
        };

        foreach (var pair in containers)
        {
            var containerGo = FindDescendant(uiTransform, pair.Key);
            Assert.IsNotNull(containerGo, "应存在容器：" + pair.Key);
            var container = containerGo.GetComponent<RectTransform>();
            Assert.IsTrue(containerGo.gameObject.activeInHierarchy, pair.Key + " 容器应激活");

            bool rowExists = false;
            for (int i = 0; i < container.childCount; i++)
            {
                var child = container.GetChild(i);
                if (child == null || string.IsNullOrEmpty(child.name)) continue;
                if (child.GetComponent<TMP_Text>() == null) continue;
                if (child.name == "RecentlyCompleted") continue;
                rowExists = true;
                break;
            }
            Assert.IsTrue(rowExists, pair.Key + " 应包含动态内容行（当前 childCount=" + container.childCount + "）");

            // 滚动采样：从底部 1 → 顶部 0
            bool visible = false;
            float step = 0.1f;
            for (float pos = 1f; pos >= -0.001f && !visible; pos -= step)
            {
                scrollRect.verticalNormalizedPosition = pos;
                yield return null;
                yield return null;

                for (int i = 0; i < container.childCount && !visible; i++)
                {
                    var child = container.GetChild(i);
                    if (child == null || child.GetComponent<TMP_Text>() == null) continue;
                    if (child.name == "RecentlyCompleted") continue;
                    var rowRect = WorldRectOf(child.GetComponent<RectTransform>());
                    if (Overlaps(rowRect, viewportWorld))
                    {
                        AssertRowVisible(child.gameObject, viewportWorld, pair.Value);
                        visible = true;
                    }
                }
            }
            Assert.IsTrue(visible, pair.Value + " 应至少在某一滚动位置与 Viewport 相交（Game View 可见）");
        }
    }
}
}
