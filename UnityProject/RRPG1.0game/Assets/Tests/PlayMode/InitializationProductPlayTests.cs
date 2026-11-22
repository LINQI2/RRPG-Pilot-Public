using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

/// <summary>
/// Initial World 产品入口 &amp; 保存继续 Play 专项（阶段 2/3）。
///
/// 真实产品默认入口（不绕过生成配置）：
///   SampleScene → MainMenuUI.btnNewGame 真实 EventSystem 点击 → HeroSelectionUI 真实英雄卡 + 确认按钮
///   → SaveSystem.NewGame() 读取 InitialWorldConfigurationCatalog 正式默认配置
///   → TownUI / ProductHomeWorldHost → Shell 底部“管理”导航真实点击
///   → 人口行真实点击选人 → 职责页真实岗位行 → 真实“派工”按钮。
///
/// 覆盖：
///   1) 默认生成配置与运行时实例逐项同源（数量可调，不硬编码比例；本类只读取当前配置）；
///   2) 管理页真实选人 / 派工到实例 Farm 岗位；正式新世界没有静态 ws_* 岗位 / Place；
///   3) 真实 Clock 推进产生真实资源劳动（Ingredients / 劳务证据），不直接写 Work / Resources；
///   4) 管理页真实“保存”按钮 → 场景重建 → MainMenu Continue 真实点击 → 逐项恢复；
///      再次进村不重种建筑 / 静态岗位，资源 / 批次 / 证据不重复生产；
///   5) 住房实例没有岗位；工坊已从正式 Catalog / Choose / TryPlaceBuilding 下线；
///   6) 唯一 UTC 时间戳截图 + 追加日志（路径见 EvidenceRelative）。
///
/// 存档保护：与 ManagementShellPlayTests.SaveSlotGuard 同一持久协议（先捕获当前用户单槽字节 +
/// SHA256 + manifest；存在未完成 run 阻断；finally 只按本 run 已验备份恢复并回读校验）。
/// 本类不使用 tests-run / Shell / Process / rql-mutate / 全局保存；不启停 Editor。
/// </summary>
public sealed class InitializationProductPlayTests
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private const string EvidenceRelative = "Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-play-evidence.log";
    private const string SaveProtectionRelative = "Development/StudioRuns/Initialization-20261027/save-guard";
    private const string ProtectedFontRelative = "UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset";

    private object _town, _management, _clockBeforeActivitySchedule;
    private static object _host, _shell;
    private SaveSlotGuard _guard;
    private static string _fontBeforeHash;

    // ==================== 反射公共通道（只读 + 调用真实产品方法） ====================

    private static Type T(string name) => Type.GetType(name + ", Assembly-CSharp", true);

    /// <summary>跨装配类型解析（StaticConfig.Runtime / Village.Runtime 类型不在 Assembly-CSharp）。</summary>
    private static Type AnyType(string name)
    {
        var type = Type.GetType(name + ", Assembly-CSharp", false) ?? Type.GetType(name, false);
        if (type != null) return type;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(name, false);
            if (type != null) return type;
        }
        Assert.Fail("类型不存在：" + name);
        return null;
    }

    private static object Get(object instance, string name)
    {
        Assert.IsNotNull(instance, "反射目标不得为 null：" + name);
        var type = instance.GetType();
        var field = type.GetField(name, F);
        if (field != null) return field.GetValue(instance);
        var property = type.GetProperty(name, F);
        Assert.IsNotNull(property, "成员不存在：" + name + " @ " + type.FullName);
        return property.GetValue(instance, null);
    }

    private static void Set(object instance, string name, object value)
    {
        var type = instance.GetType();
        var field = type.GetField(name, F);
        if (field != null) { field.SetValue(instance, value); return; }
        var property = type.GetProperty(name, F);
        Assert.IsNotNull(property, "成员不存在：" + name + " @ " + type.FullName);
        property.SetValue(instance, value, null);
    }

    private static object Static(string typeName, string method, params object[] args)
    {
        var candidates = T(typeName).GetMethods(F)
            .Where(m => m.Name == method && m.GetParameters().Length == (args == null ? 0 : args.Length)).ToList();
        Assert.AreEqual(1, candidates.Count,
            "静态方法必须唯一：" + typeName + "." + method + "/" + (args == null ? 0 : args.Length));
        return candidates[0].Invoke(null, args);
    }

    private static object StaticField(string typeName, string name)
    {
        var type = T(typeName);
        var field = type.GetField(name, F);
        if (field != null) return field.GetValue(null);
        var property = type.GetProperty(name, F);
        Assert.IsNotNull(property, "静态字段/属性不存在：" + typeName + "." + name);
        return property.GetValue(null, null);
    }

    private static object Call(object instance, string method, params object[] args)
    {
        var candidates = instance.GetType().GetMethods(F)
            .Where(m => m.Name == method && m.GetParameters().Length == (args == null ? 0 : args.Length)).ToList();
        Assert.AreEqual(1, candidates.Count,
            "方法必须唯一：" + method + " @ " + instance.GetType().FullName);
        return candidates[0].Invoke(instance, args);
    }

    private static object Find(string typeName)
    {
        var type = T(typeName);
        return UnityEngine.Object.FindObjectsOfType(type, true).FirstOrDefault(o => o != null);
    }

    private static Transform FindPath(Transform root, string path)
    {
        Assert.IsNotNull(root, "路径根不得为 null：" + path);
        var found = root.Find(path);
        if (found != null) return found;
        foreach (var candidate in root.GetComponentsInChildren<Transform>(true))
            if (candidate.name == path) return candidate;
        return null;
    }

    private static string RepoRoot() =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));

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
        string line = DateTime.UtcNow.ToString("O") + " [" + tag + "] " + message;
        Debug.Log("[InitializationProductPlayTests] " + tag + " " + message);
        try
        {
            string path = Path.Combine(RepoRoot(), EvidenceRelative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllText(path, line + "\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[InitializationProductPlayTests] evidence write failed: " + ex.Message);
        }
    }

    // ==================== 真实 EventSystem 点击（非物理鼠标） ====================

    private static List<RaycastResult> RaycastAllAt(Vector2 point)
    {
        var eventSystem = EventSystem.current;
        Assert.IsNotNull(eventSystem, "场景必须存在 EventSystem（真实 Raycast 前提）");
        var data = new PointerEventData(eventSystem)
        { position = point, button = PointerEventData.InputButton.Left, clickCount = 1 };
        var hits = new List<RaycastResult>();
        eventSystem.RaycastAll(data, hits);
        return hits;
    }

    private static string DumpHits(List<RaycastResult> hits)
    {
        if (hits == null || hits.Count == 0) return "（无命中）";
        return string.Join("\n", hits.Select((h, i) => i + ". " + (h.gameObject == null ? "<null>" : h.gameObject.name)
            + " | Button=" + (h.gameObject == null ? "null" : (h.gameObject.GetComponentInParent<Button>()?.name ?? "null"))));
    }

    private static void RealPointerClick(RectTransform target, string what)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 point = RectTransformUtility.WorldToScreenPoint(
            target.GetComponentInParent<Canvas>()?.worldCamera, target.TransformPoint(target.rect.center));
        var hits = RaycastAllAt(point);
        Assert.Greater(hits.Count, 0, what + "：Raycast 无命中 @ " + point);
        var top = hits[0].gameObject;
        Assert.IsTrue(top == target.gameObject || top.transform.IsChildOf(target),
            what + "：top hit 不是目标（玩家点到的不是它）\n" + DumpHits(hits));
        var handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(top);
        Assert.IsNotNull(handler, what + "：top hit 父链没有 IPointerClickHandler\n" + DumpHits(hits));
        var data = new PointerEventData(EventSystem.current)
        { position = point, button = PointerEventData.InputButton.Left, clickCount = 1 };
        Assert.IsTrue(ExecuteEvents.Execute(handler, data, ExecuteEvents.pointerClickHandler),
            what + "：PointerClick 未被 handler 处理");
    }

    private static void ClickChild(object root, string path, string what)
    {
        var transform = FindPath(((Component)root).transform, path);
        Assert.IsNotNull(transform, what + "：子控件不存在 " + path);
        var button = transform.GetComponent<Button>();
        Assert.IsNotNull(button, what + "：子控件不是 Button " + path);
        Assert.IsTrue(button.interactable, what + "：Button 不可交互");
        RealPointerClick((RectTransform)transform, what);
    }

    // ==================== 产品事实读取 ====================

    private static List<object> Buildings() =>
        ((IEnumerable)Get(Get(_host, "Snapshot"), "Buildings")).Cast<object>().ToList();

    private static List<object> Persons() =>
        ((IEnumerable)Get(Get(_host, "Snapshot"), "Persons")).Cast<object>().ToList();

    private static List<object> Places() =>
        ((IEnumerable)Get(Get(_host, "Snapshot"), "Places")).Cast<object>().ToList();

    private static string FirstCompletedFarm(out string buildingId)
    {
        foreach (object b in Buildings())
        {
            if ((string)Get(b, "DefinitionId") != "building_farm") continue;
            if (Get(b, "LifecycleState").ToString() != "Completed") continue;
            buildingId = (string)Get(b, "InstanceId");
            return "workplace:" + buildingId + ":main";
        }
        Assert.Fail("默认世界必须存在 Completed Farm 实例");
        buildingId = null;
        return null;
    }

    private static string FirstCompletedKitchen(out string buildingId)
    {
        foreach (object b in Buildings())
        {
            if ((string)Get(b, "DefinitionId") != "building_kitchen") continue;
            if (Get(b, "LifecycleState").ToString() != "Completed") continue;
            buildingId = (string)Get(b, "InstanceId");
            return "workplace:" + buildingId + ":main";
        }
        Assert.Fail("默认世界必须存在 Completed Kitchen 实例（生成默认配置）");
        buildingId = null;
        return null;
    }

    private static List<string> WorkBatchWorkplaceIds() =>
        ((IEnumerable)Get(WorkData(), "Batches")).Cast<object>()
            .Select(b => (string)Get(b, "WorkplaceId")).ToList();

    /// <summary>真实人口行点击（top-hit + PointerClick 由 RealPointerClick 断言）；返回真实选中 PersonId。</summary>
    private static string ClickPopulationRow(object management, int index, string what)
    {
        var content = (RectTransform)Get(management, "populationContent");
        Assert.IsNotNull(content, what + "：populationContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Assert.Greater(content.childCount, index, what + "：人口行数量不足");
        RealPointerClick((RectTransform)content.GetChild(index), what);
        string personId = (string)Get(management, "SelectedPersonId");
        Assert.IsNotEmpty(personId, what + "：真实人口行点击必须选中一个真实 PersonId");
        return personId;
    }

    /// <summary>真实岗位行点击：按当前快照领域 ID 定位行序，行数必须等于真实业务行数，top-hit 必须是目标行。</summary>
    private static void ClickWorkplaceRow(object management, string workplaceId, string what)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, what + "：Management.CurrentSnapshot 必须已读取");
        var rows = ((IEnumerable)Get(snapshot, "Workplaces")).Cast<object>().ToList();
        int index = rows.FindIndex(r => (string)Get(r, "WorkplaceId") == workplaceId);
        Assert.GreaterOrEqual(index, 0, what + "：管理快照必须包含实例岗位行 " + workplaceId);
        var content = (RectTransform)Get(management, "workplaceContent");
        Assert.IsNotNull(content, what + "：workplaceContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Assert.AreEqual(rows.Count, content.childCount, what + "：岗位行数必须等于真实快照岗位数");
        RealPointerClick((RectTransform)content.GetChild(index), what);
    }

    private static void ShowManagementTab(object management, string tabPath, string expectedTab, string pageField, string what)
    {
        ClickChild(management, tabPath, what);
        Assert.AreEqual(expectedTab, Get(management, "ActiveTab").ToString(), what + "：真实点击页签必须切到 " + expectedTab);
        var page = (GameObject)Get(management, pageField);
        Assert.IsNotNull(page, what + "：" + pageField + " 必须绑定");
        Assert.IsTrue(page.activeSelf, what + "：" + pageField + " 必须激活");
    }

    private static object Assignments() => StaticField("VillageRuntimeBridge", "Assignments");
    private static object World() => StaticField("VillageRuntimeBridge", "World");
    private static object WorkState() => StaticField("VillageRuntimeBridge", "IntradayWork");
    private static object ResourcesState() => StaticField("VillageRuntimeBridge", "Resources");

    private static string GetPersonWorkplace(string personId) =>
        (string)Call(Assignments(), "GetWorkplaceId", personId);

    private static string WorkplaceDefDisplay(string workplaceId)
    {
        object def = Call(Assignments(), "GetWorkplaceDef", workplaceId);
        return def == null ? null : (string)Get(def, "DisplayName");
    }

    private static bool HasResourceWorkplace(string workplaceId)
    {
        var defs = (IEnumerable)StaticField("VillageRuntimeBridge", "_resourceWorkplaceDefs");
        return defs.Cast<object>().Any(d => (string)Get(d, "WorkplaceId") == workplaceId);
    }

    private static int Ingredients() => Convert.ToInt32(Get(ResourcesState(), "Ingredients"));
    private static int Food() => Convert.ToInt32(Get(ResourcesState(), "Food"));

    private static object WorkData() => Call(WorkState(), "ToSaveData");

    private static int WorkEvidenceCount() =>
        ((IEnumerable)Get(WorkData(), "Evidence")).Cast<object>().Count();

    private static int WorkBatchCount() =>
        ((IEnumerable)Get(WorkData(), "Batches")).Cast<object>().Count();

    private static long WorkEvidenceMinutes(string personId, string workplaceId)
        => ((IEnumerable)Get(WorkData(), "Evidence")).Cast<object>()
            .Where(e => (string)Get(e, "PersonId") == personId && (string)Get(e, "WorkplaceId") == workplaceId)
            .Select(e => Convert.ToInt64(Get(e, "Minutes"))).DefaultIfEmpty(0).Sum();

    private static string SavePath() => (string)Static("SaveSystem", "GetSavePath");

    private static void PauseClock()
    {
        var clock = StaticField("VillageRuntimeBridge", "Time");
        Assert.IsNotNull(clock, "VillageRuntimeBridge.Time 必须存在");
        Call(clock, "Pause");
    }

    private static void AdvanceTo(long minute)
    {
        var clock = StaticField("VillageRuntimeBridge", "Time");
        Assert.IsNotNull(clock, "VillageRuntimeBridge.Time 必须存在");
        var assembly = clock.GetType().Assembly;
        object target = Activator.CreateInstance(assembly.GetType("WorldInstant", true), minute);
        object origin = Enum.Parse(assembly.GetType("WorldAdvanceOrigin", true), "Direct");
        object result = Call(clock, "AdvanceTo", target, origin, null, null);
        int retries = 0;
        while (Get(result, "Outcome").ToString() == "Yielded" && retries++ < 100)
        {
            object session = Get(clock, "AdvanceSession");
            Assert.IsNotNull(session, "Yielded 必须存在 AdvanceSession");
            result = Call(clock, "ResumeAdvance", Get(session, "RequestId"), null);
        }
        Assert.AreEqual("Completed", Get(result, "Outcome").ToString(),
            "世界推进未完成：" + (Get(result, "Diagnostic") ?? string.Empty));
    }

    private static string Surface() => Get(Get(_shell, "Router"), "Surface").ToString();

    private static string BuildingIdsSnapshot() =>
        string.Join(";", Buildings().Select(b => (string)Get(b, "InstanceId")).OrderBy(id => id, StringComparer.Ordinal));

    private static string SceneFacts()
    {
        return "buildings=" + BuildingIdsSnapshot()
            + " ingredients=" + Ingredients() + " food=" + Food()
            + " batches=" + WorkBatchCount() + " evidence=" + WorkEvidenceCount();
    }

    // ==================== 存档保护（持久协议，先捕获后恢复） ====================

    private sealed class SaveSlotGuard
    {
        private const string ManifestName = "manifest.json";
        private const string BackupName = "original-save.bin";

        [Serializable] private sealed class ManifestStatus { public string runId; public string status; }

        private string _path, _runId, _runDir, _binPath, _manifestPath, _captureUtc, _beforeHash;
        private bool _existed, _captured, _restored;
        private byte[] _bytes;

        public bool Restored => _restored;
        public string RunId => _runId;

        private static string ProtectionRoot()
            => Path.Combine(RepoRoot(), SaveProtectionRelative.Replace('/', Path.DirectorySeparatorChar));

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
                Assert.IsTrue(File.Exists(_binPath), "持久备份未生成：" + _binPath);
                byte[] readback = File.ReadAllBytes(_binPath);
                Assert.AreEqual(_bytes.LongLength, readback.LongLength, "备份字节数回读不一致");
                Assert.AreEqual(_beforeHash, HashBytes(readback), "备份回读 SHA256 不一致");
                Assert.AreEqual(_beforeHash, HashFile(_path), "捕获后当前档已被改写，拒绝继续");
            }

            WriteManifest(_existed ? "captured" : "absent", null);
            _captured = true;
            Evidence("SAVE-CAPTURE", "runId=" + _runId + " path=" + _path + " existed=" + _existed
                + " bytes=" + (_bytes == null ? 0 : _bytes.LongLength) + " sha256=" + _beforeHash);
        }

        public void Restore()
        {
            Assert.IsTrue(_captured, "SaveSlotGuard.Capture 未成功完成，禁止 Restore");
            if (_restored)
            {
                bool stillOk = File.Exists(_path) == _existed && (!_existed || HashFile(_path) == _beforeHash);
                if (stillOk) return;
            }

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
                string afterHash = exists ? HashFile(_path) : "<absent>";
                bool ok = exists == _existed && (!_existed || afterHash == _beforeHash);
                _restored = ok;
                WriteManifest(ok ? "restored" : "restore-failed", ok ? null : "sha256 mismatch after write");
                Evidence("SAVE-RESTORE", "runId=" + _runId + " existed=" + exists + " sha256=" + afterHash
                    + " matches-before=" + ok);
                Assert.IsTrue(ok, "单槽存档恢复失败（backup/manifest 已保留，需人工核对）：期望 existed=" + _existed
                    + " sha256=" + _beforeHash + "，实际 existed=" + exists + " sha256=" + afterHash);
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
                try { parsed = JsonUtility.FromJson<ManifestStatus>(File.ReadAllText(manifest)); }
                catch (Exception) { }
                string status = parsed == null ? null : parsed.status;
                if (status == "restored" || status == "protective-snapshot") continue;
                unfinished.Add(Path.GetFileName(dir) + "(" + (status ?? "unreadable") + ")");
            }
            Assert.AreEqual(0, unfinished.Count,
                "存在未完成的存档保护 run，已阻止新测试以免覆盖当前档；请人工核对后再继续："
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
            sb.Append("  \"capturedBy\": \"InitializationProductPlayTests.SaveSlotGuard (test-only helper)\",\n");
            sb.Append("  \"note\": \"只允许用本 runID 的 original-save.bin 恢复；进程 abort 时 TearDown 不执行，备份留待人工取证。\"\n");
            sb.Append("}\n");
            File.WriteAllText(_manifestPath, sb.ToString());
        }

        private static string JsonText(string value)
            => value == null ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    // ==================== Lifecycle ====================

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _guard = new SaveSlotGuard();
        _guard.Capture();

        // 与 ManagementShellPlayTests 同一协议：UnitySetUp 失败时 NUnit 不保证普通 [TearDown] 一定执行，
        // 因此 Capture 成功后任何异常（ResetForTests / LoadScene / 断言）都必须在本 fixture 内用
        // 本 runID 的已验备份恢复用户单槽；yield 不能放进带 catch 的 try，故用 try/finally。
        bool setUpCompleted = false;
        try
        {
            Static("VillageRuntimeBridge", "ResetForTests");
            SceneManager.LoadScene("SampleScene");
            yield return null;
            yield return null;
            setUpCompleted = true;
        }
        finally
        {
            if (!setUpCompleted) RestoreCapturedSaveAfterFailedSetUp();
        }
    }

    /// <summary>
    /// UnitySetUp 已 Capture 后发生任何异常时的唯一清理：只用本 runID 已验备份恢复并留痕；
    /// 恢复自身失败不掩盖原始 SetUp 异常（backup / manifest 保留供人工核对）。
    /// </summary>
    private void RestoreCapturedSaveAfterFailedSetUp()
    {
        if (_guard == null) return;
        try
        {
            _guard.Restore();
            Evidence("SAVE-RESTORE-SETUP-FAILURE", "runId=" + _guard.RunId
                + " ok=True note=UnitySetUp 失败后按本 runID 已验备份恢复，不依赖普通 TearDown");
        }
        catch (Exception ex)
        {
            Evidence("SAVE-RESTORE-SETUP-FAILURE", "runId=" + _guard.RunId
                + " ok=False error=" + ex.GetType().Name + " message=" + ex.Message
                + " backup/manifest 已保留，需人工核对");
            Debug.LogWarning("[InitializationProductPlayTests] UnitySetUp 失败后的存档恢复失败（不掩盖原始异常）："
                + ex.GetType().Name + ": " + ex.Message);
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            _guard?.Restore();
        }
        finally
        {
            try
            {
                Static("VillageRuntimeBridge", "ResetForTests");
            }
            finally
            {
                RestoreActivitySchedule();
            }
        }
    }

    private void RestoreActivitySchedule()
    {
        object before = _clockBeforeActivitySchedule;
        if (before == null) return;
        Static("VillageRuntimeBridge", "ConfigureActivitySchedule", before);
        object restored = StaticField("VillageRuntimeBridge", "ActivitySchedule");
        Assert.IsTrue(ReferenceEquals(restored, before), "活动日程必须恢复为用例前捕获的同一实例");
        _clockBeforeActivitySchedule = null;
        Evidence("SCHEDULE-RESTORE", "sameInstance=True");
    }

    // ==================== 唯一截图 ====================

    private IEnumerator CaptureUniqueScreenshot(string tag)
    {
        string dir = Path.Combine(RepoRoot(), "Development/StudioRuns/Initialization-20261027/play-evidence");
        Directory.CreateDirectory(dir);
        string stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
        string path = Path.Combine(dir, "initialization-product-" + tag + "-" + stamp + ".png");
        for (int i = 2; File.Exists(path); i++)
            path = Path.Combine(dir, "initialization-product-" + tag + "-" + stamp + "-" + i + ".png");
        try { ScreenCapture.CaptureScreenshot(path); }
        catch (Exception ex)
        {
            Evidence("SHOT-UNAVAILABLE", "tag=" + tag + " error=" + ex.GetType().Name + ":" + ex.Message + " path=" + path);
            yield break;
        }
        if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
        for (int i = 0; i < 4; i++) yield return null;
        bool produced = File.Exists(path) && new FileInfo(path).Length > 0;
        Evidence(produced ? "SHOT" : "SHOT-UNAVAILABLE",
            "tag=" + tag + " produced=" + produced + " path=" + path
            + (produced ? " bytes=" + new FileInfo(path).Length + " sha256=" + HashFile(path) : "（截图工具不可用，不伪造）"));
    }

    // ==================== 唯一完整产品路径用例 ====================

    [UnityTest]
    public IEnumerator DefaultProductEntry_ManagementAssignLabor_SaveContinueAndReenterWithoutReseed()
    {
        // ---------- 0. 正式默认配置（唯一 Authority 读入口；不硬套旧数量） ----------
        var args = new object[] { null, null };
        bool configRead = (bool)Static("InitialWorldConfigurationCatalog", "TryReadDefault", args);
        Assert.IsTrue(configRead, "正式默认初始世界配置必须可读：" + args[1]);
        object config = args[0];
        Assert.IsNotNull(config, "默认初始世界配置不得为 null");
        var configuredBuildings = ((IEnumerable)Get(config, "Buildings")).Cast<object>().ToList();
        var configuredPeople = ((IEnumerable)Get(config, "People")).Cast<object>().ToList();
        Assert.IsNotEmpty(configuredBuildings, "正式默认配置必须包含初始建筑（当前 Test 配置可调）");
        Assert.IsNotEmpty(configuredPeople, "正式默认配置必须包含初始人口");
        string configuredShape = string.Join(";", configuredBuildings.Select(b =>
            (string)Get(b, "BuildingDefinitionId")));
        Evidence("CONFIG", "buildings=" + configuredBuildings.Count + " people=" + configuredPeople.Count
            + " definitions=" + configuredShape);

        // ---------- 1. 真实 EventSystem 产品入口：MainMenu → HeroSelection → Town ----------
        object menu = Find("MainMenuUI");
        Assert.IsNotNull(menu, "MainMenuUI 必须由真实场景创建");
        Call(menu, "RefreshContinueButton");
        var newGameButton = (Button)Get(menu, "btnNewGame");
        Assert.IsNotNull(newGameButton, "MainMenuUI.btnNewGame 必须绑定真实 Button");
        Assert.IsTrue(newGameButton.interactable, "主菜单“新游戏”必须可点");
        RealPointerClick((RectTransform)newGameButton.transform, "主菜单·新游戏（真实 EventSystem）");
        yield return null;
        yield return null;

        object heroSelection = Find("HeroSelectionUI");
        Assert.IsNotNull(heroSelection, "真实 NewGame 必须显示 HeroSelectionUI");
        var heroCards = (IList)Get(heroSelection, "_heroCards");
        Assert.IsNotEmpty(heroCards, "HeroSelectionUI 必须有真实英雄卡");
        var confirm = (Button)Get(heroSelection, "btnConfirm");
        Assert.IsNotNull(confirm, "HeroSelectionUI.btnConfirm 必须绑定真实 Button");
        Assert.IsFalse(confirm.interactable, "未选英雄时确认必须禁用（gating 不绕过）");
        var firstCard = ((GameObject)heroCards[0]).GetComponent<Button>();
        Assert.IsNotNull(firstCard, "英雄卡根必须绑定真实 Button");
        RealPointerClick((RectTransform)firstCard.transform, "英雄选择·第一张英雄卡（真实 EventSystem）");
        yield return null;
        Assert.IsTrue(confirm.interactable, "真实选择英雄后确认按钮必须启用");
        RealPointerClick((RectTransform)confirm.transform, "英雄选择·确认（真实 EventSystem）");
        yield return null;
        yield return null;

        for (int i = 0; i < 20 && Find("ProductHomeWorldHost") == null; i++) yield return null;
        _town = Find("TownUI");
        Assert.IsNotNull(_town, "真实产品入口必须进入 TownUI");
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "真实产品入口必须创建 ProductHomeWorldHost");
        _shell = Get(_host, "Shell");
        Assert.IsNotNull(_shell, "HomeProductShell 必须存在");
        for (int i = 0; i < 20 && !(bool)Get(_shell, "ShellVisible"); i++) yield return null;
        for (int i = 0; i < 20 && !(bool)Get(_host, "WorldVisible"); i++) yield return null;
        Assert.IsTrue((bool)Get(_shell, "ShellVisible"), "真实产品入口后 Shell 必须可见");
        Assert.IsTrue((bool)Get(_host, "WorldVisible"), "真实产品入口后世界必须可见");
        PauseClock();
        Call(_host, "RefreshNow");
        object player = Get(_town, "PlayerData");
        Assert.IsNotNull(player, "TownUI.PlayerData 必须来自真实 NewGame");
        Evidence("ENTRY", "reachedWorld=True configuredBuildings=" + configuredBuildings.Count
            + " runtimeBuildings=" + Buildings().Count + " persons=" + Persons().Count + " paused=True");

        // ---------- 2. 默认生成配置 → 运行时实例逐项同源 ----------
        var buildings = Buildings();
        Assert.AreEqual(configuredBuildings.Count, buildings.Count,
            "默认 NewGame 必须按生成配置创建同数量建筑（数量可调，不硬编码比例）");
        CollectionAssert.AreEquivalent(
            configuredBuildings.Select(b => (string)Get(b, "BuildingDefinitionId")).ToArray(),
            buildings.Select(b => (string)Get(b, "DefinitionId")).ToArray(),
            "运行时建筑定义必须与默认生成配置同源");
        foreach (object b in buildings)
        {
            string id = (string)Get(b, "InstanceId");
            Assert.IsNotEmpty(id, "每个默认建筑必须有真实 InstanceId");
            Assert.AreEqual("Completed", Get(b, "LifecycleState").ToString(),
                "默认初始建筑必须是真实 Completed 实例：" + id);
        }
        Assert.AreEqual(buildings.Count, buildings.Select(b => (string)Get(b, "InstanceId")).Distinct().Count(),
            "默认建筑 InstanceId 不得重复");
        var housing = buildings.Where(b => (string)Get(b, "DefinitionId") == "building_house").ToList();
        Assert.IsNotEmpty(housing, "当前默认配置必须包含住房实例");

        // 住房没有岗位：定义 / Place / 资源职责都不得凭空出现。
        foreach (object house in housing)
        {
            string houseId = (string)Get(house, "InstanceId");
            string houseWorkplace = "workplace:" + houseId + ":main";
            Assert.IsNull((string)WorkplaceDefDisplay(houseWorkplace), "住房不得生成岗位定义：" + houseWorkplace);
            Assert.IsFalse((bool)Call(World(), "ContainsPlace", houseWorkplace), "住房不得生成岗位 Place：" + houseWorkplace);
            Assert.IsFalse(HasResourceWorkplace(houseWorkplace), "住房不得生成资源职责：" + houseWorkplace);
        }

        // 正式 NewGame 不再注册静态 Test 岗位 / Place / 资源职责。
        foreach (string legacy in new[] { "ws_farm_01", "ws_kitchen_01", "ws_test_01", "ws_test_02" })
        {
            Assert.IsNull((string)WorkplaceDefDisplay(legacy), "正式新世界不得注册静态岗位：" + legacy);
            Assert.IsFalse((bool)Call(World(), "ContainsPlace", legacy), "正式新世界不得创建静态岗位 Place：" + legacy);
            Assert.IsFalse(HasResourceWorkplace(legacy), "正式新世界不得注册静态资源职责：" + legacy);
        }

        string farmWorkplace = FirstCompletedFarm(out string farmBuilding);
        Assert.AreEqual("workplace:" + farmBuilding + ":main", farmWorkplace, "实例 Farm 岗位身份必须绑定真实建筑");
        Assert.IsNotNull((string)WorkplaceDefDisplay(farmWorkplace), "实例 Farm 岗位必须真实注册");
        Evidence("WIRING", "farmBuilding=" + farmBuilding + " farmWorkplace=" + farmWorkplace
            + " housing=" + housing.Count + " runtimeBuildings=" + buildings.Count);

        // ---------- 3. 真实管理入口：选人 + 派工到实例 Farm ----------
        var managementNav = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate2");
        Assert.IsNotNull(managementNav, "Shell 底部导航必须存在“管理”按钮 Navigate2");
        var managementButton = managementNav.GetComponent<Button>();
        Assert.IsNotNull(managementButton, "“管理”导航必须是真实 Button");
        Assert.IsTrue(managementButton.interactable, "世界状态下“管理”必须可点");
        RealPointerClick((RectTransform)managementNav, "底部导航·管理（真实 EventSystem）");
        yield return null;
        yield return null;
        Assert.AreEqual("Management", Surface(), "真实导航必须进入 Management Surface");
        _management = Get(_shell, "Management");
        Assert.IsNotNull(_management, "HomeProductShell.Management 必须是真实 authored Prefab");

        if (!(bool)Get(Get(_management, "populationPage"), "activeSelf"))
            ClickChild(_management, "ModalPanel/Tab_Population", "页签·人口");
        yield return null;
        var populationContent = (RectTransform)Get(_management, "populationContent");
        Assert.IsNotNull(populationContent, "populationContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(populationContent);
        Assert.Greater(populationContent.childCount, 0, "管理人口页必须渲染真实人物行");
        var firstRow = (RectTransform)populationContent.GetChild(0);
        RealPointerClick(firstRow, "人口行·第一行（真实 EventSystem）");
        yield return null;
        Assert.IsTrue((bool)Get(_management, "IsVisible"), "点击人口行后管理页必须仍可见");
        string personId = (string)Get(_management, "SelectedPersonId");
        Assert.IsNotEmpty(personId, "真实人口行点击必须选中一个真实 PersonId");
        Assert.IsTrue(Persons().Any(p => (string)Get(p, "PersonId") == personId), "选中人物必须来自真实世界快照");

        ClickChild(_management, "ModalPanel/Tab_Workplaces", "页签·职责与场所");
        yield return null;
        Assert.AreEqual("Workplaces", Get(_management, "ActiveTab").ToString(), "真实点击页签必须切到职责与场所");
        object workplaceSnapshot = Get(_management, "CurrentSnapshot");
        var workplaceRows = ((IEnumerable)Get(workplaceSnapshot, "Workplaces")).Cast<object>().ToList();
        int farmRowIndex = workplaceRows.FindIndex(r => (string)Get(r, "WorkplaceId") == farmWorkplace);
        Assert.GreaterOrEqual(farmRowIndex, 0, "管理快照必须包含实例 Farm 岗位行：" + farmWorkplace);
        var workplaceContent = (RectTransform)Get(_management, "workplaceContent");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(workplaceContent);
        Assert.AreEqual(workplaceRows.Count, workplaceContent.childCount, "岗位行数必须等于真实快照岗位数");
        RealPointerClick((RectTransform)workplaceContent.GetChild(farmRowIndex), "岗位行·实例 Farm（真实 EventSystem）");
        yield return null;
        var assignButton = (Button)Get(_management, "assignButton");
        Assert.IsNotNull(assignButton, "管理派工按钮必须绑定");
        Assert.IsTrue(assignButton.interactable, "实例 Farm 空位必须允许派工");
        RealPointerClick((RectTransform)assignButton.transform, "派工·选中人物→实例 Farm（真实 EventSystem）");
        yield return null;
        Assert.AreEqual(farmWorkplace, GetPersonWorkplace(personId), "真实派工按钮必须写入实例 Farm 职责");
        var status = (TMP_Text)Get(_management, "statusMessage");
        Assert.IsNotNull(status, "管理状态文案必须绑定");
        StringAssert.Contains("已绑定", status.text, "派工成功必须有真实领域回执");
        Evidence("MANAGEMENT", "person=" + personId + " workplace=" + farmWorkplace
            + " status=" + status.text.Replace("\n", " | "));

        // ---------- 4. 真实资源劳动（真实 Clock 推进，不直接写 Work / Resources） ----------
        _clockBeforeActivitySchedule = StaticField("VillageRuntimeBridge", "ActivitySchedule");
        Assert.IsNotNull(_clockBeforeActivitySchedule, "ActivitySchedule 必须存在");
        object isolated = Activator.CreateInstance(AnyType("IntradayActivitySchedule"),
            new object[] { new int[0], 15, 30, null });
        Static("VillageRuntimeBridge", "ConfigureActivitySchedule", isolated);
        Assert.IsTrue(ReferenceEquals(StaticField("VillageRuntimeBridge", "ActivitySchedule"), isolated),
            "测试专用活动窗口隔离必须真实生效（仅本用例跨度）");

        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "关闭管理必须回到 World");
        int ingredientsBefore = Ingredients();
        object clock = StaticField("VillageRuntimeBridge", "Time");
        long minuteBefore = Convert.ToInt64(Get(Get(clock, "CurrentInstant"), "Minute"));
        object schedule = Get(WorkState(), "Schedule");
        Assert.IsNotNull(schedule, "IntradayWork.Schedule 必须存在");
        long morningStart = Convert.ToInt64(Get(schedule, "MorningStart"));
        // 从当前分钟推进到真实工作区间内 41 分钟：41 × Farm rate(12) = 492 labor → 至少 1 Ingredients；
        // 不直接写 Work / Resources，也不依赖固定日程常量。
        long target = Math.Max(minuteBefore, morningStart) + 41;
        AdvanceTo(target);
        Call(_host, "RefreshNow");
        int ingredientsAfter = Ingredients();
        long workedMinutes = WorkEvidenceMinutes(personId, farmWorkplace);
        Assert.Greater(ingredientsAfter, ingredientsBefore,
            "真实 Farm 劳动必须按既有 profile 产出 Ingredients（不重复结算、不手改资源）");
        Assert.Greater(workedMinutes, 0, "真实 Clock 推进必须写下来源可证明的劳务证据");
        Evidence("LABOR", "minute=" + minuteBefore + "->" + target
            + " ingredients=" + ingredientsBefore + "->" + ingredientsAfter
            + " evidenceMinutes=" + workedMinutes);

        // ---------- 5. 真实保存 → 场景重建 → MainMenu Continue ----------
        var managementNav2 = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate2");
        Assert.IsNotNull(managementNav2, "重新进入管理前导航必须存在");
        RealPointerClick((RectTransform)managementNav2, "底部导航·管理（保存前）");
        yield return null;
        _management = Get(_shell, "Management");
        Assert.IsNotNull(_management, "重新打开管理页必须存在");
        var saveButton = (Button)Get(_management, "saveButton");
        Assert.IsNotNull(saveButton, "管理页保存按钮必须绑定");
        Assert.IsTrue(saveButton.interactable, "合法世界边界保存按钮必须可点");
        RealPointerClick((RectTransform)saveButton.transform, "管理·保存（真实 EventSystem）");
        yield return null;
        StringAssert.Contains("已保存", ((TMP_Text)Get(_management, "statusMessage")).text,
            "真实保存必须有成功回执");
        string savePath = SavePath();
        Assert.IsTrue(File.Exists(savePath), "真实保存必须写出单槽：" + savePath);
        string saveHash = HashFile(savePath);
        string savedBuildings = BuildingIdsSnapshot();
        string savedPersonWorkplace = GetPersonWorkplace(personId);
        int savedIngredients = Ingredients();
        int savedFood = Food();
        int savedBatches = WorkBatchCount();
        int savedEvidence = WorkEvidenceCount();
        long savedWorked = WorkEvidenceMinutes(personId, farmWorkplace);
        Evidence("SAVE", "sha256=" + saveHash + " " + SceneFacts()
            + " person=" + personId + " workplace=" + savedPersonWorkplace);

        int oldHostId = ((Component)_host).gameObject.GetInstanceID();
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        object menuAfter = Find("MainMenuUI");
        Assert.IsNotNull(menuAfter, "场景重建后 MainMenuUI 必须存在");
        Call(menuAfter, "RefreshContinueButton");
        var continueButton = (Button)Get(menuAfter, "btnContinue");
        Assert.IsNotNull(continueButton, "MainMenuUI.btnContinue 必须绑定真实 Button");
        Assert.IsTrue(continueButton.interactable, "单槽存在时 Continue 必须可用");
        RealPointerClick((RectTransform)continueButton.transform, "主菜单·继续（真实 EventSystem）");
        yield return null;
        yield return null;
        for (int i = 0; i < 20 && Find("ProductHomeWorldHost") == null; i++) yield return null;
        _town = Find("TownUI");
        Assert.IsNotNull(_town, "Continue 必须进入真实 TownUI");
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "Continue 必须重建 ProductHomeWorldHost");
        Assert.AreNotEqual(oldHostId, ((Component)_host).gameObject.GetInstanceID(),
            "Continue 必须得到新的产品世界宿主实例");
        _shell = Get(_host, "Shell");
        Assert.IsNotNull(_shell, "Continue 后必须重建 HomeProductShell");
        Call(_host, "RefreshNow");
        PauseClock();
        Assert.AreEqual(saveHash, HashFile(savePath), "Continue 不得改写单槽存档");

        // ---------- 6. 再次进村不重种：实例 / 岗位 / 资源 / 批次 / 证据逐项连续 ----------
        Assert.AreEqual(savedBuildings, BuildingIdsSnapshot(), "Continue 不得重种 / 增删建筑实例");
        Assert.AreEqual(savedPersonWorkplace, GetPersonWorkplace(personId), "Continue 必须恢复真实实例岗位职责");
        Assert.AreEqual(savedIngredients, Ingredients(), "Continue 不得重复生产 / 增减 Ingredients");
        Assert.AreEqual(savedFood, Food(), "Continue 不得重复生产 / 增减 Food");
        Assert.AreEqual(savedBatches, WorkBatchCount(), "Continue 不得重复生产批次");
        Assert.AreEqual(savedEvidence, WorkEvidenceCount(), "Continue 不得重复写劳务证据");
        Assert.AreEqual(savedWorked, WorkEvidenceMinutes(personId, farmWorkplace), "Continue 必须逐项恢复劳务证据");
        foreach (string legacy in new[] { "ws_farm_01", "ws_kitchen_01", "ws_test_01", "ws_test_02" })
        {
            Assert.IsNull((string)WorkplaceDefDisplay(legacy), "正式实例档 Continue 不得重种静态岗位：" + legacy);
            Assert.IsFalse((bool)Call(World(), "ContainsPlace", legacy), "正式实例档 Continue 不得重种静态 Place：" + legacy);
            Assert.IsFalse(HasResourceWorkplace(legacy), "正式实例档 Continue 不得重种静态资源职责：" + legacy);
        }
        foreach (object house in Buildings().Where(b => (string)Get(b, "DefinitionId") == "building_house"))
        {
            string houseWorkplace = "workplace:" + (string)Get(house, "InstanceId") + ":main";
            Assert.IsNull((string)WorkplaceDefDisplay(houseWorkplace), "Continue 后住房仍不得有岗位");
            Assert.IsFalse(HasResourceWorkplace(houseWorkplace), "Continue 后住房仍不得有资源职责");
        }
        Evidence("CONTINUE", "sameBuildings=True " + SceneFacts()
            + " personWorkplace=" + GetPersonWorkplace(personId));

        // 唯一截图（真实世界 + 已恢复的实例建筑）。
        yield return CaptureUniqueScreenshot("save-continue");

        // ---------- 7. 工坊正式 Catalog / Choose / TryPlaceBuilding 拒绝 ----------
        object build = Get(_shell, "Build");
        Assert.IsNotNull(build, "HomeProductShell.Build 必须存在");
        var buildNav = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate6");
        Assert.IsNotNull(buildNav, "Shell 底部导航必须存在“建造”按钮 Navigate6");
        RealPointerClick((RectTransform)buildNav, "底部导航·建造（真实 EventSystem）");
        yield return null;
        Assert.IsTrue((bool)Get(build, "IsOpen"), "真实建造导航必须打开建造目录");
        var catalog = ((IEnumerable)Get(build, "Catalog")).Cast<object>().ToList();
        Assert.IsNotEmpty(catalog, "正式 Catalog 必须至少包含 Farm（不伪造空目录）");
        CollectionAssert.DoesNotContain(
            catalog.Select(d => (string)Get(d, "BuildingDefinitionId")).ToArray(),
            "building_workshop", "工坊必须退出正式 Catalog");
        CollectionAssert.DoesNotContain(
            catalog.Select(d => (string)Get(d, "BuildingDefinitionId")).ToArray(),
            "building_smithy", "铁匠铺必须退出正式 Catalog");
        Assert.IsNull(FindPath(((Component)build).transform, "BuildingCatalog/CatalogRows/Choose_building_workshop"),
            "正式 Catalog 不得渲染工坊选择按钮");
        Assert.IsFalse((bool)Call(build, "Choose", "building_workshop"), "正式 Choose 必须拒绝工坊");
        var formal = AnyType("RRPG.StaticConfig.BuildingRegistry").GetMethod("IsFormallyBuildable", F);
        Assert.IsNotNull(formal, "BuildingRegistry.IsFormallyBuildable 必须存在");
        Assert.IsFalse((bool)formal.Invoke(null, new object[] { "building_workshop" }), "工坊不得为正式可建造");
        var placeArgs = new object[] { "building_workshop", 0f, -6f, null, null };
        Assert.IsFalse((bool)Static("VillageRuntimeBridge", "TryPlaceBuilding", placeArgs),
            "正式 TryPlaceBuilding 必须拒绝工坊");
        StringAssert.Contains("下线", placeArgs[4] as string ?? string.Empty, "拒绝必须给出明确下线诊断");
        Evidence("WORKSHOP-REJECT", "catalog=" + string.Join(",", catalog.Select(d => (string)Get(d, "BuildingDefinitionId")))
            + " choose=False place=False diagnostic=" + (placeArgs[4] as string));
        Call(build, "Cancel");
        Assert.IsFalse((bool)Get(build, "IsOpen"), "公开 Cancel 必须关闭建造目录");

        // ---------- 8. 终态事实（日志） ----------
        Evidence("END", "productEntry=True managementAssign=True labor=True saveContinue=True"
            + " noReseed=True housingNoWorkplace=True workshopRejected=True " + SceneFacts());
    }

    // ==================== 聚焦专项：默认生成 Kitchen 的真实 UI 派工与输入转换 ====================

    /// <summary>
    /// 默认生成入口 + **默认生成 Kitchen 实例岗位**的真实 UI 专项：
    ///   SampleScene → MainMenu“新游戏” → 英雄选择 → Town → 管理页
    ///   → 人口行真实点击选人 → 职责页签 → Kitchen 实例岗位行真实点击 → 派工按钮真实点击；
    ///   → 再选另一人 → Farm 实例岗位行 → 派工；
    ///   → 关闭管理，真实 Clock 推进：Farm 真实产出 Ingredients，Kitchen 真实消耗 Ingredients 产出 Food；
    ///   → 管理页真实“保存”按钮 → 场景重建 → MainMenu Continue → 岗位 / 资源 / 批次 / 证据逐项等值。
    /// 不直接写 Work / Resources（不使用 AddIngredients 伪造原料），不绕过 Gateway / UI 回调；
    /// 每个点击都由 RealPointerClick 断言真实 top-hit 与 IPointerClickHandler。
    /// </summary>
    [UnityTest]
    public IEnumerator DefaultGeneratedKitchen_RealUiAssignAndConversion_SaveContinue()
    {
        // ---------- 0. 正式默认配置（唯一 Authority 读入口） ----------
        var args = new object[] { null, null };
        bool configRead = (bool)Static("InitialWorldConfigurationCatalog", "TryReadDefault", args);
        Assert.IsTrue(configRead, "正式默认初始世界配置必须可读：" + args[1]);
        object config = args[0];
        Assert.IsNotNull(config, "默认初始世界配置不得为 null");
        var configuredBuildings = ((IEnumerable)Get(config, "Buildings")).Cast<object>().ToList();
        Assert.IsNotEmpty(configuredBuildings, "正式默认配置必须包含初始建筑（当前 Test 配置可调）");

        // ---------- 1. 真实产品入口 ----------
        object menu = Find("MainMenuUI");
        Assert.IsNotNull(menu, "MainMenuUI 必须由真实场景创建");
        Call(menu, "RefreshContinueButton");
        var newGameButton = (Button)Get(menu, "btnNewGame");
        Assert.IsNotNull(newGameButton, "MainMenuUI.btnNewGame 必须绑定真实 Button");
        Assert.IsTrue(newGameButton.interactable, "主菜单“新游戏”必须可点");
        RealPointerClick((RectTransform)newGameButton.transform, "主菜单·新游戏（Kitchen 专项，真实 EventSystem）");
        yield return null;
        yield return null;

        object heroSelection = Find("HeroSelectionUI");
        Assert.IsNotNull(heroSelection, "真实 NewGame 必须显示 HeroSelectionUI");
        var heroCards = (IList)Get(heroSelection, "_heroCards");
        Assert.IsNotEmpty(heroCards, "HeroSelectionUI 必须有真实英雄卡");
        var confirm = (Button)Get(heroSelection, "btnConfirm");
        Assert.IsNotNull(confirm, "HeroSelectionUI.btnConfirm 必须绑定真实 Button");
        var firstCard = ((GameObject)heroCards[0]).GetComponent<Button>();
        Assert.IsNotNull(firstCard, "英雄卡根必须绑定真实 Button");
        RealPointerClick((RectTransform)firstCard.transform, "英雄选择·第一张英雄卡（Kitchen 专项）");
        yield return null;
        RealPointerClick((RectTransform)confirm.transform, "英雄选择·确认（Kitchen 专项）");
        yield return null;
        yield return null;

        for (int i = 0; i < 20 && Find("ProductHomeWorldHost") == null; i++) yield return null;
        _town = Find("TownUI");
        Assert.IsNotNull(_town, "真实产品入口必须进入 TownUI");
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "真实产品入口必须创建 ProductHomeWorldHost");
        _shell = Get(_host, "Shell");
        Assert.IsNotNull(_shell, "HomeProductShell 必须存在");
        for (int i = 0; i < 20 && !(bool)Get(_host, "WorldVisible"); i++) yield return null;
        Assert.IsTrue((bool)Get(_host, "WorldVisible"), "真实产品入口后世界必须可见");
        PauseClock();
        Call(_host, "RefreshNow");

        // ---------- 2. 默认生成配置 → 运行时实例（数量 / 定义同源） ----------
        var buildings = Buildings();
        Assert.AreEqual(configuredBuildings.Count, buildings.Count,
            "默认 NewGame 必须按生成配置创建同数量建筑（数量可调，不硬编码比例）");
        string kitchenWorkplace = FirstCompletedKitchen(out string kitchenBuilding);
        string farmWorkplace = FirstCompletedFarm(out string farmBuilding);
        Assert.AreNotEqual(kitchenBuilding, farmBuilding, "默认生成 Farm / Kitchen 必须是两个独立建筑实例");
        Assert.IsNotNull((string)WorkplaceDefDisplay(kitchenWorkplace), "默认 Kitchen 实例岗位必须真实注册");
        Assert.IsTrue(HasResourceWorkplace(kitchenWorkplace), "默认 Kitchen 实例必须是真实资源岗位（Ingredients→Food）");
        Assert.AreEqual("Completed", Get(Buildings().First(b => (string)Get(b, "InstanceId") == kitchenBuilding), "LifecycleState").ToString(),
            "默认 Kitchen 必须是真实 Completed 实例");
        Evidence("KITCHEN-WIRING", "kitchenBuilding=" + kitchenBuilding + " kitchenWorkplace=" + kitchenWorkplace
            + " farmBuilding=" + farmBuilding + " farmWorkplace=" + farmWorkplace
            + " runtimeBuildings=" + buildings.Count);

        // ---------- 3. 真实管理入口 ----------
        var managementNav = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate2");
        Assert.IsNotNull(managementNav, "Shell 底部导航必须存在“管理”按钮 Navigate2");
        RealPointerClick((RectTransform)managementNav, "底部导航·管理（Kitchen 专项）");
        yield return null;
        yield return null;
        Assert.AreEqual("Management", Surface(), "真实导航必须进入 Management Surface");
        _management = Get(_shell, "Management");
        Assert.IsNotNull(_management, "HomeProductShell.Management 必须是真实 authored Prefab");

        // 3.1 人物 A → 默认 Kitchen 实例岗位（真实行 + 真实按钮 + top-hit）
        ShowManagementTab(_management, "ModalPanel/Tab_Population", "Population", "populationPage", "页签·人口（Kitchen 派工）");
        yield return null;
        string kitchenPerson = ClickPopulationRow(_management, 0, "人口行·第一行（Kitchen 派工）");
        yield return null;
        ShowManagementTab(_management, "ModalPanel/Tab_Workplaces", "Workplaces", "workplacePage", "页签·职责与场所（Kitchen）");
        yield return null;
        ClickWorkplaceRow(_management, kitchenWorkplace, "岗位行·默认 Kitchen 实例");
        yield return null;
        Assert.AreEqual(kitchenWorkplace, (string)Get(_management, "SelectedWorkplaceId"),
            "真实点击岗位行后 Management 选中候选必须是默认 Kitchen 实例岗位");
        var assignButton = (Button)Get(_management, "assignButton");
        Assert.IsNotNull(assignButton, "管理派工按钮必须绑定");
        Assert.IsTrue(assignButton.interactable, "默认 Kitchen 空位必须允许派工");
        RealPointerClick((RectTransform)assignButton.transform, "派工·人物A→默认 Kitchen（真实 EventSystem）");
        yield return null;
        Assert.AreEqual(kitchenWorkplace, GetPersonWorkplace(kitchenPerson), "真实派工按钮必须写入默认 Kitchen 实例职责");
        StringAssert.Contains("已绑定", ((TMP_Text)Get(_management, "statusMessage")).text,
            "Kitchen 派工成功必须由真实领域回执");

        // 3.2 人物 B → 默认 Farm 实例岗位（为 Kitchen 提供真实原料）
        ShowManagementTab(_management, "ModalPanel/Tab_Population", "Population", "populationPage", "页签·人口（Farm 派工）");
        yield return null;
        Assert.Greater(((RectTransform)Get(_management, "populationContent")).childCount, 1,
            "人口页必须至少有两行真实人物");
        string farmPerson = ClickPopulationRow(_management, 1, "人口行·第二行（Farm 派工）");
        yield return null;
        Assert.AreNotEqual(kitchenPerson, farmPerson, "必须为 Kitchen / Farm 选择两个不同真实人物");
        ShowManagementTab(_management, "ModalPanel/Tab_Workplaces", "Workplaces", "workplacePage", "页签·职责与场所（Farm）");
        yield return null;
        ClickWorkplaceRow(_management, farmWorkplace, "岗位行·默认 Farm 实例");
        yield return null;
        Assert.AreEqual(farmWorkplace, (string)Get(_management, "SelectedWorkplaceId"),
            "真实点击岗位行后 Management 选中候选必须是默认 Farm 实例岗位");
        assignButton = (Button)Get(_management, "assignButton");
        Assert.IsTrue(assignButton.interactable, "默认 Farm 空位必须允许派工");
        RealPointerClick((RectTransform)assignButton.transform, "派工·人物B→默认 Farm（真实 EventSystem）");
        yield return null;
        Assert.AreEqual(farmWorkplace, GetPersonWorkplace(farmPerson), "真实派工按钮必须写入默认 Farm 实例职责");
        Evidence("KITCHEN-ASSIGN", "kitchenPerson=" + kitchenPerson + " kitchenWorkplace=" + kitchenWorkplace
            + " farmPerson=" + farmPerson + " farmWorkplace=" + farmWorkplace
            + " selectedWorkplace=" + (string)Get(_management, "SelectedWorkplaceId"));

        // ---------- 4. 真实劳动与厨房输入转换（真实 Clock，不写 Work / Resources） ----------
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "关闭管理必须回到 World");

        _clockBeforeActivitySchedule = StaticField("VillageRuntimeBridge", "ActivitySchedule");
        Assert.IsNotNull(_clockBeforeActivitySchedule, "ActivitySchedule 必须存在");
        object isolated = Activator.CreateInstance(AnyType("IntradayActivitySchedule"),
            new object[] { new int[0], 15, 30, null });
        Static("VillageRuntimeBridge", "ConfigureActivitySchedule", isolated);
        Assert.IsTrue(ReferenceEquals(StaticField("VillageRuntimeBridge", "ActivitySchedule"), isolated),
            "测试专用活动窗口隔离必须真实生效（仅本用例跨度）");

        int foodBefore = Food();
        object clock = StaticField("VillageRuntimeBridge", "Time");
        long minuteBefore = Convert.ToInt64(Get(Get(clock, "CurrentInstant"), "Minute"));
        object schedule = Get(WorkState(), "Schedule");
        Assert.IsNotNull(schedule, "IntradayWork.Schedule 必须存在");
        long morningStart = Convert.ToInt64(Get(schedule, "MorningStart"));
        long lunchMinute = Convert.ToInt64(Get(schedule, "Lunch"));

        // 真实 Farm 劳动先产出原料：41 个工作分钟按既有 profile 至少 1 Ingredients。
        long target = Math.Max(minuteBefore, morningStart) + 41;
        AdvanceTo(target);
        Call(_host, "RefreshNow");
        long farmMinutes = WorkEvidenceMinutes(farmPerson, farmWorkplace);
        Assert.Greater(farmMinutes, 0, "真实 Farm 劳动必须留下来源可证明的劳务证据（为 Kitchen 提供原料）");

        // Kitchen 只有在拿到真实 Ingredients 后才能开始转换；在有界工作窗口内等待真实 Food 产出。
        for (int step = 0; step < 10 && Food() == foodBefore; step++)
        {
            long next = Math.Min(target + 40, lunchMinute - 1);
            if (next <= target) break;
            AdvanceTo(next);
            Call(_host, "RefreshNow");
            target = next;
        }
        Assert.Greater(Food(), foodBefore,
            "真实 Kitchen 必须把 Farm 产出的 Ingredients 转换（消耗输入）为 Food；未消耗原料不得凭空产出");
        long kitchenMinutes = WorkEvidenceMinutes(kitchenPerson, kitchenWorkplace);
        Assert.Greater(kitchenMinutes, 0, "真实 Clock 推进必须写下来源可证明的 Kitchen 劳务证据");
        Assert.Contains(kitchenWorkplace, WorkBatchWorkplaceIds(), "真实 Kitchen 转换必须留下该实例的资源批次");
        Assert.Contains(farmWorkplace, WorkBatchWorkplaceIds(), "真实 Farm 生产必须留下该实例的资源批次");
        Evidence("KITCHEN-LABOR", "minute=" + minuteBefore + "->" + target
            + " food=" + foodBefore + "->" + Food() + " ingredients=" + Ingredients()
            + " farmMinutes=" + farmMinutes + " kitchenMinutes=" + kitchenMinutes
            + " batches=" + WorkBatchCount() + " evidence=" + WorkEvidenceCount());

        // ---------- 5. 真实保存 → 场景重建 → MainMenu Continue ----------
        var managementNav2 = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate2");
        Assert.IsNotNull(managementNav2, "保存前导航必须存在");
        RealPointerClick((RectTransform)managementNav2, "底部导航·管理（Kitchen 保存）");
        yield return null;
        _management = Get(_shell, "Management");
        Assert.IsNotNull(_management, "保存前管理页必须存在");
        var saveButton = (Button)Get(_management, "saveButton");
        Assert.IsNotNull(saveButton, "管理页保存按钮必须绑定");
        Assert.IsTrue(saveButton.interactable, "合法世界边界保存按钮必须可点");
        RealPointerClick((RectTransform)saveButton.transform, "管理·保存（Kitchen 专项）");
        yield return null;
        StringAssert.Contains("已保存", ((TMP_Text)Get(_management, "statusMessage")).text, "真实保存必须有成功回执");
        string savePath = SavePath();
        Assert.IsTrue(File.Exists(savePath), "真实保存必须写出单槽：" + savePath);
        string saveHash = HashFile(savePath);
        string savedBuildings = BuildingIdsSnapshot();
        int savedFood = Food();
        int savedIngredients = Ingredients();
        int savedBatches = WorkBatchCount();
        int savedEvidence = WorkEvidenceCount();
        long savedKitchenMinutes = WorkEvidenceMinutes(kitchenPerson, kitchenWorkplace);
        long savedFarmMinutes = WorkEvidenceMinutes(farmPerson, farmWorkplace);
        Evidence("KITCHEN-SAVE", "sha256=" + saveHash + " " + SceneFacts()
            + " kitchenPerson=" + kitchenPerson + " farmPerson=" + farmPerson
            + " kitchenMinutes=" + savedKitchenMinutes + " farmMinutes=" + savedFarmMinutes);

        int oldHostId = ((Component)_host).gameObject.GetInstanceID();
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        object menuAfter = Find("MainMenuUI");
        Assert.IsNotNull(menuAfter, "场景重建后 MainMenuUI 必须存在");
        Call(menuAfter, "RefreshContinueButton");
        var continueButton = (Button)Get(menuAfter, "btnContinue");
        Assert.IsNotNull(continueButton, "MainMenuUI.btnContinue 必须绑定真实 Button");
        Assert.IsTrue(continueButton.interactable, "单槽存在时 Continue 必须可用");
        RealPointerClick((RectTransform)continueButton.transform, "主菜单·继续（Kitchen 专项）");
        yield return null;
        yield return null;
        for (int i = 0; i < 20 && Find("ProductHomeWorldHost") == null; i++) yield return null;
        _town = Find("TownUI");
        Assert.IsNotNull(_town, "Continue 必须进入真实 TownUI");
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "Continue 必须重建 ProductHomeWorldHost");
        Assert.AreNotEqual(oldHostId, ((Component)_host).gameObject.GetInstanceID(),
            "Continue 必须得到新的产品世界宿主实例");
        _shell = Get(_host, "Shell");
        Assert.IsNotNull(_shell, "Continue 后必须重建 HomeProductShell");
        Call(_host, "RefreshNow");
        PauseClock();
        Assert.AreEqual(saveHash, HashFile(savePath), "Continue 不得改写单槽存档");

        Assert.AreEqual(savedBuildings, BuildingIdsSnapshot(), "Continue 不得重种 / 增删建筑实例");
        Assert.AreEqual(kitchenWorkplace, GetPersonWorkplace(kitchenPerson), "Continue 必须恢复默认 Kitchen 实例职责");
        Assert.AreEqual(farmWorkplace, GetPersonWorkplace(farmPerson), "Continue 必须恢复默认 Farm 实例职责");
        Assert.AreEqual(savedFood, Food(), "Continue 不得增减 / 重复生产 Food");
        Assert.AreEqual(savedIngredients, Ingredients(), "Continue 不得增减 / 重复生产 Ingredients");
        Assert.AreEqual(savedBatches, WorkBatchCount(), "Continue 不得重种 / 重算资源批次");
        Assert.AreEqual(savedEvidence, WorkEvidenceCount(), "Continue 不得重复写劳务证据");
        Assert.AreEqual(savedKitchenMinutes, WorkEvidenceMinutes(kitchenPerson, kitchenWorkplace),
            "Continue 必须逐项恢复 Kitchen 劳务证据");
        Assert.AreEqual(savedFarmMinutes, WorkEvidenceMinutes(farmPerson, farmWorkplace),
            "Continue 必须逐项恢复 Farm 劳务证据");
        foreach (string legacy in new[] { "ws_farm_01", "ws_kitchen_01", "ws_test_01", "ws_test_02" })
        {
            Assert.IsNull((string)WorkplaceDefDisplay(legacy), "正式实例档 Continue 不得重种静态岗位：" + legacy);
            Assert.IsFalse((bool)Call(World(), "ContainsPlace", legacy), "正式实例档 Continue 不得重种静态 Place：" + legacy);
            Assert.IsFalse(HasResourceWorkplace(legacy), "正式实例档 Continue 不得重种静态资源职责：" + legacy);
        }
        Evidence("KITCHEN-CONTINUE", "sameBuildings=True " + SceneFacts()
            + " kitchenWorkplace=" + GetPersonWorkplace(kitchenPerson)
            + " farmWorkplace=" + GetPersonWorkplace(farmPerson));
        Evidence("KITCHEN-END", "uiKitchenAssign=True uiFarmAssign=True conversion=True saveContinue=True noReseed=True");
    }

    // ==================== 字体 / 基线只读保护（不修改任何受保护资产） ====================

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        string font = Path.Combine(RepoRoot(), ProtectedFontRelative.Replace('/', Path.DirectorySeparatorChar));
        Assert.IsTrue(File.Exists(font), "受保护 TMP 字体不存在：" + font);
        _fontBeforeHash = HashFile(font);
        Evidence("FONT-PRE", "bytes=" + new FileInfo(font).Length + " sha256=" + _fontBeforeHash);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        string font = Path.Combine(RepoRoot(), ProtectedFontRelative.Replace('/', Path.DirectorySeparatorChar));
        Assert.IsTrue(File.Exists(font), "受保护 TMP 字体不存在：" + font);
        string after = HashFile(font);
        Evidence("FONT-POST", "bytes=" + new FileInfo(font).Length + " sha256=" + after
            + " unchanged=" + (after == _fontBeforeHash));
        Assert.AreEqual(_fontBeforeHash, after, "受保护 TMP 字体在测试后被持久修改");
    }
}
