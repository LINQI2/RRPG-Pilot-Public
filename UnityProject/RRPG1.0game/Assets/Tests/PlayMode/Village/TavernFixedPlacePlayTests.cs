using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace RRPG.ProductHome.Tests
{
/// <summary>
/// 2026-11-05 固定世界场所（Fixed World Place）：Tavern 必须始终存在、有独立世界表现、
/// 不随活动生命周期创建/销毁、不重复、A/B 共享同一 Place 身份。
/// 只读断言；不改 Registry / Activity / Save。
/// </summary>
public sealed class TavernFixedPlacePlayTests
{
    private const BindingFlags S = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags I = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static System.Type T(string n) => System.Type.GetType(n + ", Assembly-CSharp", true);
    private static object Static(string n, string m, params object[] a)
    {
        var c = T(n).GetMethods(S).Where(x => x.Name == m && x.GetParameters().Length == a.Length).ToList();
        Assert.AreEqual(1, c.Count, "静态方法必须唯一：" + n + "." + m);
        return c[0].Invoke(null, a);
    }
    private static object Call(object o, string m, params object[] a)
    {
        var c = o.GetType().GetMethods(I).Where(x => x.Name == m && x.GetParameters().Length == a.Length).ToList();
        Assert.AreEqual(1, c.Count, "方法必须唯一：" + m);
        return c[0].Invoke(o, a);
    }
    private static object Get(object o, string n) => o.GetType().GetProperty(n, I).GetValue(o);
    private static object Find(string n) => UnityEngine.Object.FindObjectOfType(T(n), true);

    private object _host;
    private object _guard;
    private string _savePath, _backup, _sha;

    private static string RepoRoot() => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));
    private static string HashFile(string p)
    {
        using (var s = SHA256.Create()) using (var f = File.OpenRead(p))
            return System.BitConverter.ToString(s.ComputeHash(f)).Replace("-", "").ToLowerInvariant();
    }

    [UnitySetUp] public IEnumerator Setup()
    {
        // 保护当前用户单槽（不删除、不覆盖）：退场时按字节恢复。
        _savePath = (string)Static("SaveSystem", "GetSavePath");
        if (!string.IsNullOrEmpty(_savePath) && File.Exists(_savePath))
        {
            _sha = HashFile(_savePath);
            _backup = Path.Combine(Path.GetTempPath(), "tavern-guard-" + System.Guid.NewGuid().ToString("N") + ".json");
            File.Copy(_savePath, _backup, true);
        }
        Static("VillageRuntimeBridge", "ResetForTests");
        SceneManager.LoadScene("SampleScene");
        yield return null; yield return null;
        var player = Static("SaveSystem", "NewGame");
        Assert.IsNotNull(player, "NewGame 必须成功");
        Assert.IsTrue((bool)Static("SaveSystem", "Save", player), "新档必须可保存");
        Call(Find("MainMenuUI"), "OnContinue");
        yield return null; yield return null;
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "必须存在产品 Home 世界宿主");
        Call(_host, "RefreshNow");
    }

    [TearDown] public void Cleanup()
    {
        try { Static("VillageRuntimeBridge", "ResetForTests"); }
        finally
        {
            if (!string.IsNullOrEmpty(_backup) && File.Exists(_backup) && !string.IsNullOrEmpty(_savePath))
            {
                File.Copy(_backup, _savePath, true);
                Assert.AreEqual(_sha, HashFile(_savePath), "用户单槽必须按字节恢复");
                File.Delete(_backup);
            }
        }
    }

    private ProductHomeSnapshot Snapshot => (ProductHomeSnapshot)Get(_host, "Snapshot");

    private static void Shoot(string name)
    {
        var dir = Path.Combine(RepoRoot(), "Development/StudioRuns/FixedPlace-Tavern-20261105/screenshots");
        Directory.CreateDirectory(dir);
        ScreenCapture.CaptureScreenshot(Path.Combine(dir, name + ".png"));
        Debug.Log("[TavernShot] " + Path.Combine(dir, name + ".png"));
    }

    [UnityTest] public IEnumerator TavernExistsWithoutActivityAndSurvivesProfileSwitch()
    {
        var snap = Snapshot;
        var tavern = snap.Place("Tavern");
        Assert.IsNotNull(tavern, "默认世界必须始终存在 Tavern（不依赖活动引用）");
        Assert.IsTrue(tavern.IsFixedPlace, "Tavern 必须被识别为 Fixed World Place");
        Assert.AreEqual("FixedPlace", tavern.Kind, "固定场所必须有独立 Kind，而不是 ActivityPlace");

        var views = (IDictionary)Get(_host, "PlaceViews");
        Assert.IsTrue(views.Contains("Tavern"), "A 档必须有 Tavern 世界实体");
        Shoot("a-fixed-tavern");
        yield return new WaitForEndOfFrame();
        Vector3 anchor = ((Component)views["Tavern"]).transform.position;

        // 多次刷新（模拟人走人来的快照变化）不得重复或丢失
        for (int i = 0; i < 3; i++) { Call(_host, "RefreshNow"); yield return null; }
        views = (IDictionary)Get(_host, "PlaceViews");
        Assert.AreEqual(1, ((IDictionary)Get(_host, "PlaceViews")).Keys.Cast<object>().Count(k => k.ToString() == "Tavern"),
            "同一 Tavern 不得产生多个实体");
        Assert.AreEqual(anchor, ((Component)views["Tavern"]).transform.position, "固定场所位置不得随刷新漂移");

        Call(_host, "SetVisualPrototype", true); Call(_host, "RefreshNow"); yield return null;
        Assert.IsTrue(((IDictionary)Get(_host, "PlaceViews")).Contains("Tavern"), "B 档必须保留 Tavern");
        Call(_host, "SetVisualPrototype", false); Call(_host, "RefreshNow"); yield return null;
        Assert.IsTrue(((IDictionary)Get(_host, "PlaceViews")).Contains("Tavern"), "A→B→A 后 Tavern 必须仍在");
        Assert.AreEqual(anchor, ((Component)((IDictionary)Get(_host, "PlaceViews"))["Tavern"]).transform.position,
            "A/B/A 切换不得改变 Tavern 世界位置");
    }

    [UnityTest] public IEnumerator OtherVirtualActivityPlacesStayHiddenInA()
    {
        var snap = Snapshot;
        var virtualPlaces = snap.Places.Where(p => !p.IsFixedPlace && p.Kind == "ActivityPlace").ToList();
        var views = (IDictionary)Get(_host, "PlaceViews");
        foreach (var p in virtualPlaces)
            Assert.IsFalse(views.Contains(p.PlaceId), "未声明为固定场所的虚拟地点不得在 A 档出现：" + p.PlaceId);
        Assert.IsTrue(views.Contains("Tavern"), "固定场所不受该隐藏规则影响");
        yield return null;
    }
}
}
