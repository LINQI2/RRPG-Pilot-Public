using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace RRPG.Village.Tests
{
/// <summary>
/// V4 Functional Repair｜**Resource 链真实数值追踪 + Food Surplus 可达性**（Disposable Development Test / Test / Not Frozen）。
///
/// 人工验收发现：Fresh New Game → Farm + Kitchen → Jump To Next Month → Expedition Supply 仍为 0 / 0。
/// 本测试先做**真实数值追踪**（Farm DailyExecutionFact 人数 / Farm 产出 / Kitchen 加工 / InVillage 人数 /
/// Food Demand / 每日净变化），判定属于：
///   A. Farm / Kitchen 根本没执行；或
///   B. 确实执行了，但当天产出被 Village Consumption 全部吃掉。
/// 再验证 Current Functional Build 必须满足的可达性：
///   最小合法 Staffing（1 Farm + 1 Kitchen）在一个 Day Tick 内、完成 Village Consumption 之后，
///   Food Net Delta 必须 > 0，并最终形成可携带出征的真实 Carry（不注入、不凭空补粮、不绕过 Resource Authority）。
/// </summary>
public class VillageV4ResourceSurplusPlayTests
{
    private const BindingFlags F =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static Type GetType(string name) => Type.GetType(name + ", Assembly-CSharp");

    private static object FindInstance(string typeName)
    {
        var t = GetType(typeName);
        if (t == null) return null;
        foreach (var o in UnityEngine.Object.FindObjectsOfType(t, true))
            if (o != null) return o;
        return null;
    }

    private static object GetMember(object o, string name)
    {
        var f = o.GetType().GetField(name, F);
        if (f != null) return f.GetValue(o);
        var p = o.GetType().GetProperty(name, F);
        Assert.IsNotNull(p, "成员不存在：" + name + " @ " + o.GetType().FullName);
        return p.GetValue(o, null);
    }

    private static object InvokeInstance(object o, string method, params object[] args)
    {
        var m = o.GetType().GetMethod(method, F);
        Assert.IsNotNull(m, "方法不存在：" + method + " @ " + o.GetType().FullName);
        return m.Invoke(o, args);
    }

    private static object InvokeStatic(Type type, string method, params object[] args)
    {
        foreach (var m in type.GetMethods(F).Where(m => m.Name == method))
        {
            var ps = m.GetParameters();
            if (ps.Length != (args == null ? 0 : args.Length)) continue;
            bool ok = true;
            for (int i = 0; i < ps.Length; i++)
            {
                if (args[i] == null) continue;
                if (!ps[i].ParameterType.IsInstanceOfType(args[i])) { ok = false; break; }
            }
            if (ok) return m.Invoke(null, args);
        }
        Assert.Fail("静态方法不存在或参数不匹配：" + type.Name + "." + method);
        return null;
    }

    private static object CallStatic(string typeName, string method, params object[] args) =>
        InvokeStatic(GetType(typeName), method, args);

    // 注意：VillageRuntimeBridge 在 Assembly-CSharp（本测试程序集不引用）→ 统一用反射访问；
    // VillageTime / VillageResourceDayResult / VillageAssignmentResult 在 Village.Runtime，可直接引用类型。
    private static VillageTime BridgeTime() => (VillageTime)StaticMember("VillageRuntimeBridge", "Time");

    private static int Ingredients() =>
        (int)GetMember(StaticMember("VillageRuntimeBridge", "Resources"), "Ingredients");

    private static int Food() =>
        (int)GetMember(StaticMember("VillageRuntimeBridge", "Resources"), "Food");

    private static object StaticMember(string typeName, string name)
    {
        var t = GetType(typeName);
        var f = t.GetField(name, F);
        if (f != null) return f.GetValue(null);
        var p = t.GetProperty(name, F);
        Assert.IsNotNull(p, "静态成员不存在：" + typeName + "." + name);
        return p.GetValue(null, null);
    }

    private static VillageResourceDayResult LastDay() =>
        (VillageResourceDayResult)StaticMember("VillageRuntimeBridge", "LastResourceDayResult");

    private static int LastInVillageCount() =>
        (int)StaticMember("VillageRuntimeBridge", "LastResourceInVillageCount");

    private static int LastExecutedFactCount() =>
        (int)StaticMember("VillageRuntimeBridge", "LastResourceExecutedFactCount");

    private object _pc;
    private SaveSlotGuard _guard;

    // ==================== 用户单槽持久保护（先捕获 / 异常恢复 / finally 恢复 / 未完成阻断） ====================

    private static string RepoRoot() => System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", "..", ".."));

    [UnitySetUp] public IEnumerator ProtectUserSave()
    {
        // 先持久捕获真实用户单槽（不删档、不套历史 hash），再建立本用例隔离世界。
        _guard = new SaveSlotGuard();
        _guard.Capture();
        // UnitySetUp 失败时 NUnit 不保证执行普通 [TearDown]：初始化异常必须在本夹具内恢复。
        bool completed = false;
        try
        {
            yield return EnterTownFreshGame();
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
            Debug.LogWarning("[VillageV4ResourceSurplusPlayTests] SetUp 失败后的存档恢复失败（不掩盖原始异常）：" + ex.GetType().Name + ": " + ex.Message);
        }
    }

    [TearDown] public void RestoreUserSave()
    {
        try { _guard?.Restore(); }
        finally { CallStatic("VillageRuntimeBridge", "ResetForTests"); }
    }

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
            _path = (string)CallStatic("SaveSystem", "GetSavePath");
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
            Debug.Log("[VillageV4ResourceSurplusPlayTests][SAVE-CAPTURE] runId=" + _runId + " existed=" + _existed
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
            sb.Append("  \"capturedBy\": \"VillageV4ResourceSurplusPlayTests.SaveSlotGuard (test-only helper)\",\n");
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

    /// <summary>正式默认世界的真实 Completed 实例岗位（从运行时建筑找 ID，不写死 ws_*）。</summary>
    private static string InstanceWorkplace(string definitionId)
    {
        object runtime = StaticMember("VillageRuntimeBridge", "Buildings");
        var all = (IEnumerable)runtime.GetType().GetProperty("All", F).GetValue(runtime, null);
        foreach (var b in all)
        {
            var type = b.GetType();
            if ((string)type.GetProperty("BuildingDefinitionId", F).GetValue(b, null) != definitionId) continue;
            if (type.GetProperty("LifecycleState", F).GetValue(b, null).ToString() != "Completed") continue;
            return "workplace:" + (string)type.GetProperty("InstanceId", F).GetValue(b, null) + ":main";
        }
        Assert.Fail("正式默认世界必须存在 Completed " + definitionId + " 实例");
        return null;
    }

    private IEnumerator EnterTownFreshGame()
    {
        CallStatic("VillageRuntimeBridge", "ResetForTests");

        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;

        // 测试 initialization：用真实 NewGame + Save 建立隔离世界（用户原档已由 SaveSlotGuard 捕获，TearDown 恢复）；
        // 不再直接删除用户单槽文件。
        object fresh = CallStatic("SaveSystem", "NewGame");
        Assert.IsNotNull(fresh, "NewGame 失败");
        Assert.IsTrue((bool)CallStatic("SaveSystem", "Save", fresh), "新档应能写入");

        var mm = FindInstance("MainMenuUI");
        Assert.IsNotNull(mm, "MainMenuUI 应存在");
        InvokeInstance(mm, "OnContinue");
        yield return null;
        yield return null;

        var town = FindInstance("TownUI");
        Assert.IsNotNull(town, "TownUI 应存在");
        Assert.IsTrue(((GameObject)GetMember(town, "canvasRoot")).activeSelf, "应已进入 Town");

        _pc = GetMember(town, "PlayerData");
        Assert.IsNotNull(_pc, "TownUI.PlayerData 应存在");

        BridgeTime().SetSpeed(VillageTime.VillageTimeSpeed.Paused); // 确定性：手动按天推进
        CallStatic("VillageRuntimeBridge", "SetSimulationPersonSource", _pc);
    }

    // ==================== Resource Daily Trace + Food Surplus 可达性 ====================

    [UnityTest]
    public IEnumerator FreshNewGame_MinimalFarmKitchen_ProducesFoodSurplus_AndExpeditionCarry()
    {
        // 隔离世界（真实 NewGame + Save + 产品 OnContinue）已由 [UnitySetUp] 建立；断言范围不变。

        // ---- 最小合法 Staffing：1 Farm + 1 Kitchen（不同 Person；复用既有 Assignment Authority） ----
        var personIds = new List<string>();
        foreach (var c in (IEnumerable)GetMember(_pc, "OwnedCharacters"))
        {
            string pid = (string)GetMember(c, "PersonId");
            if (!string.IsNullOrEmpty(pid)) personIds.Add(pid);
        }
        Assert.GreaterOrEqual(personIds.Count, 2,
            "Fresh New Game 应有 ≥2 名在村 Character（Farm + Kitchen 最小 Staffing 需要）");

        // 正式实体语义：默认生成世界只有实例岗位（workplace:<InstanceId>:main），旧静态 ws_* 已退出正式目录。
        string farmWorkplace = InstanceWorkplace("building_farm");
        string kitchenWorkplace = InstanceWorkplace("building_kitchen");
        Assert.AreNotEqual(farmWorkplace, kitchenWorkplace, "默认 Farm / Kitchen 必须是两个独立实例岗位");
        Assert.AreEqual(VillageAssignmentResult.Success,
            (VillageAssignmentResult)CallStatic("VillageRuntimeBridge", "TryAssignPerson", personIds[0], farmWorkplace),
            "应能安排 Person 到默认 Farm 实例岗位（复用既有 Assignment Authority，不建第二套 Staffing）");
        Assert.AreEqual(VillageAssignmentResult.Success,
            (VillageAssignmentResult)CallStatic("VillageRuntimeBridge", "TryAssignPerson", personIds[1], kitchenWorkplace),
            "应能安排 Person 到默认 Kitchen 实例岗位");

        var trace = new System.Text.StringBuilder();
        trace.AppendLine("== Fresh New Game Resource Daily Trace（1 Farm + 1 Kitchen / Test Economy）==");
        trace.AppendLine("初始：Ingredients=" + Ingredients() + " Food=" + Food()
            + " 在村 Person=" + LastInVillageCount());

        int lastFoodNet = 0;
        int lastIngredientsNet = 0;
        for (int day = 1; day <= 3; day++)
        {
            int inBefore = Ingredients();
            int foodBefore = Food();

            BridgeTime().TickDay();
            yield return null;

            var r = LastDay();
            Assert.IsNotNull(r, "每个 Day Tick 必须产生资源结算结果（只读诊断 seam）");

            int inAfter = Ingredients();
            int foodAfter = Food();
            lastFoodNet = foodAfter - foodBefore;
            lastIngredientsNet = inAfter - inBefore;

            trace.AppendLine(string.Format(
                "Day{0}: InVillagePerson={1} 执行事实={2} | Farm→Ingredients={3} Kitchen→Food={4}(耗Ingredients={5}) | "
                + "Food消费={6}(足额{7}/应急{8}/未满足{9}) 应急Ingredients={10} | Ingredients {11}→{12} Δ{13} | Food {14}→{15} Δ{16}",
                day, LastInVillageCount(), LastExecutedFactCount(),
                r.IngredientsProduced, r.FoodProduced, r.IngredientsConsumedByKitchen,
                r.FoodConsumed, r.PersonsFoodFed, r.PersonsEmergencyFed, r.PersonsUnderfed, r.EmergencyIngredientsConsumed,
                inBefore, inAfter, inAfter - inBefore, foodBefore, foodAfter, foodAfter - foodBefore));
        }
        Debug.Log(trace.ToString());

        var lastDay = LastDay();

        // A / B 判定：不是“根本没执行”
        Assert.Greater(lastDay.IngredientsProduced, 0,
            "Farm 必须真实执行并产出 Ingredients（否则属于 A：根本没执行）\n" + trace);
        Assert.Greater(lastDay.FoodProduced, 0,
            "Kitchen 必须真实执行并把 Ingredients 加工为 Food（否则属于 A：根本没执行）\n" + trace);
        Assert.Greater(LastInVillageCount(), 0,
            "InVillage Person > 0（Village Consumption 真实发生）\n" + trace);

        // 功能约束：完成 Village Consumption 之后 Food Net Delta 必须 > 0
        Assert.Greater(lastFoodNet, 0,
            "最小合法 Staffing（1 Farm + 1 Kitchen）在一个 Day Tick 内、完成 Village Consumption 后"
            + " Food Net Delta 必须 > 0（不得靠 ConsumptionRate=0 掩盖）\n" + trace);
        Assert.GreaterOrEqual(lastIngredientsNet, 0, "Ingredients Net Delta 不得为负\n" + trace);

        // ---- Jump To Next Month：Food 必须真实积累（玩家无需 Debug 注入） ----
        int foodBeforeJump = Food();
        BridgeTime().JumpToNextMonth();
        for (int frame=0;BridgeTime().HasActiveAdvance && frame<200;frame++) yield return null;
        Assert.IsFalse(BridgeTime().HasActiveAdvance,"真实 Driver 续完 Jump 的原请求后才能进入出征");
        int foodAfterJump = Food();
        trace.AppendLine("Jump To Next Month：Food " + foodBeforeJump + " → " + foodAfterJump
            + "（Ingredients=" + Ingredients() + "）");
        Debug.Log(trace.ToString());

        Assert.Greater(foodAfterJump, foodBeforeJump + 20,
            "持续正常生产下 Jump To Next Month 后 Village Food 必须显著积累\n" + trace);

        // ---- 出征前可见性 + 真实 Carry（用真实 Food 携带出征） ----
        var expUI = FindInstance("ExpeditionUI");
        Assert.IsNotNull(expUI, "ExpeditionUI 应存在");
        InvokeInstance(expUI, "ShowExpeditionFromTown", _pc);
        yield return null;
        yield return null;

        object identityPanel = GetMember(expUI, "_identityPanel");
        Assert.IsNotNull(identityPanel, "准备界面应存在补给可见性表面");
        string supplyText = (string)GetMember(identityPanel, "SupplyText");
        StringAssert.Contains("村庄食物：" + foodAfterJump, supplyText,
            "准备界面必须显示真实 Village Food（只读，不复制资源状态）\n" + trace);
        StringAssert.Contains("预计携带：", supplyText, "准备界面必须显示预计携带量");
        StringAssert.Contains("第一步预计成本：", supplyText, "准备界面必须显示第一个合法节点的预计成本");
        Debug.Log("准备界面补给可见性文本：\n" + supplyText);

        var confirmBtn = (Button)GetMember(expUI, "btnConfirm");
        Assert.IsTrue(confirmBtn.interactable, "有足够 Food 时 Confirm Expedition 必须可用\n" + supplyText);

        InvokeInstance(expUI, "OnConfirmExpedition");
        yield return null;
        yield return null;

        object adventure = GetMember(expUI, "_currentAdventure");
        Assert.IsNotNull(adventure, "有足够 Food 时必须能真实确认出征（不得进入 0 Supply 无动作会话）\n" + trace);

        object supply = GetMember(GetMember(adventure, "ProgressionState"), "Supply");
        int carry = (int)GetMember(supply, "InitialCarryUnits");
        Assert.Greater(carry, 0, "Departure 必须把真实 Village Food 转移为 Carry\n" + trace);
        Assert.AreEqual(foodAfterJump - carry, Food(),
            "Departure 后 Village Food 必须减少恰好 Carry（不得凭空补粮 / 复制资源）\n" + trace);

        var journey = FindInstance("ExpeditionJourneyView");
        Assert.IsNotNull(journey, "确认出征后必须存在 Journey 表现宿主（Current 玩家表面）");
        Assert.IsTrue((bool)GetMember(journey, "IsVisible"), "确认出征后 Journey 必须可见");
        StringAssert.DoesNotContain("补给：0", (string)GetMember(journey, "SupplyDisplay"),
            "不得进入 0 Supply 的无动作远征会话");

        // 收尾：走既有 Retreat（Return 事务）回到 Town，并清理测试状态
        InvokeInstance(expUI, "RequestRetreatFromProgression");
        yield return null;
        yield return null;
        CallStatic("VillageRuntimeBridge", "ResetForTests");
    }
}
}
