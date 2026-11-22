using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Pilot专项测试资源隔离。只在Studio显式Begin/End期间拦截指定字体的自动保存；
/// 不改字体生成模式、不清除dirty伪装保护、不影响其他资产，也不进入玩家构建。
/// </summary>
public static class StudioTestResourceScope
{
    internal const string FontPath = "Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset";
    internal const string ScopeKey = "Studio.Pilot.FontTestScope";
    internal const string WritesKey = ScopeKey + ".BlockedWrites";
    private const string Expected = "3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854";
    private const string SnapshotRoot = "Assets/Editor/StudioFontScopeSnapshots";
    internal static bool Active => SessionState.GetBool(ScopeKey, false) &&
        Application.dataPath.Replace('\\', '/').Equals("D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets", StringComparison.OrdinalIgnoreCase);

    private static string Hash()
    {
        using (var stream = File.OpenRead(FontPath))
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }

    public static string Begin()
    {
        if (SessionState.GetBool(ScopeKey, false)) throw new InvalidOperationException("Previous test resource scope needs review");
        if (Hash() != Expected) throw new InvalidOperationException("Font baseline mismatch before resource scope");
        var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(FontPath);
        if (AssetDatabase.LoadAllAssetsAtPath(FontPath).Any(EditorUtility.IsDirty))
            throw new InvalidOperationException("Font already has pending edits; preserve them before test");
        if (!AssetDatabase.IsValidFolder(SnapshotRoot)) AssetDatabase.CreateFolder("Assets/Editor", "StudioFontScopeSnapshots");
        string folder = SnapshotRoot + "/" + Guid.NewGuid().ToString("N");
        AssetDatabase.CreateFolder(SnapshotRoot, Path.GetFileName(folder));
        if (!AssetDatabase.CopyAsset(FontPath, folder + "/Font.asset")) throw new InvalidOperationException("Font test snapshot failed");
        SessionState.SetString(ScopeKey + ".Snapshot", folder);
        SessionState.SetInt(WritesKey, 0);
        SessionState.SetBool(ScopeKey, true);
        if (!Active) { SessionState.SetBool(ScopeKey, false); throw new InvalidOperationException("Not authorized Pilot"); }
        return "scope-active|font|" + Expected;
    }

    public static string End()
    {
        // 允许宿主在投递回执丢失后幂等回收；仍须核对工程、真实空闲和原字节。
        SessionState.SetBool(ScopeKey, true);
        if (!Active) { SessionState.SetBool(ScopeKey, false); throw new InvalidOperationException("Not authorized Pilot"); }
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            throw new InvalidOperationException("Editor still running; keep resource scope for review");
        var running = typeof(UnityEditor.TestTools.TestRunner.Api.TestRunnerApi).GetMethod("IsRunActive", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (running == null || (bool)running.Invoke(null, null)) throw new InvalidOperationException("Native test runner not idle");
        if (Hash() != Expected) throw new InvalidOperationException("Font persisted unexpectedly; preserve scope and evidence");
        var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(FontPath);
        string folder = SessionState.GetString(ScopeKey + ".Snapshot", "");
        if (string.IsNullOrEmpty(folder))
        {
            // 初始维护恢复也使用真实字节副本恢复内存，单纯ImportAsset不会撤销dirty缓存。
            if (!AssetDatabase.IsValidFolder(SnapshotRoot)) AssetDatabase.CreateFolder("Assets/Editor", "StudioFontScopeSnapshots");
            folder = SnapshotRoot + "/" + Guid.NewGuid().ToString("N");
            AssetDatabase.CreateFolder(SnapshotRoot, Path.GetFileName(folder));
            if (!AssetDatabase.CopyAsset(FontPath, folder + "/Font.asset")) throw new InvalidOperationException("Font cache snapshot failed");
            SessionState.SetString(ScopeKey + ".Snapshot", folder);
        }
        if (!string.IsNullOrEmpty(folder))
        {
            Guid id;
            if (!folder.StartsWith(SnapshotRoot + "/", StringComparison.Ordinal) || !Guid.TryParseExact(Path.GetFileName(folder), "N", out id))
                throw new InvalidOperationException("Invalid owned test snapshot");
            var pristine = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(folder + "/Font.asset");
            if (pristine == null) throw new InvalidOperationException("Test snapshot missing");
            // CopyAsset按新文件名命名主对象；先还原名称再由TMP重算其派生名称哈希。
            // 不忽略hashCode/materialHashCode，也不改变原资产的实际配置。
            pristine.name = font.name;
            pristine.ReadFontAssetDefinition();
            var actualAssets = AssetDatabase.LoadAllAssetsAtPath(FontPath);
            var originalById = new Dictionary<long, UnityEngine.Object>();
            foreach (var asset in actualAssets) { string guid; long local; AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out local); originalById[local] = asset; }
            var map = new Dictionary<int, int>();
            var pairs = new List<Tuple<UnityEngine.Object, UnityEngine.Object>>();
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(folder + "/Font.asset"))
            {
                string guid; long local; AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out local);
                if (!originalById.ContainsKey(local)) throw new InvalidOperationException("Font subasset mapping changed");
                map[asset.GetInstanceID()] = originalById[local].GetInstanceID();
                pairs.Add(Tuple.Create(asset, originalById[local]));
            }
            string sourceGuid = AssetDatabase.AssetPathToGUID(FontPath);
            string snapshotGuid = AssetDatabase.AssetPathToGUID(folder + "/Font.asset");
            string Normalized(UnityEngine.Object asset) => Regex.Replace(EditorJsonUtility.ToJson(asset), "\"instanceID\":(-?\\d+)", m => {
                int from = int.Parse(m.Groups[1].Value); return "\"instanceID\":" + (map.ContainsKey(from) ? map[from] : from);
            }).Replace("\"guid\":\"" + snapshotGuid + "\"", "\"guid\":\"" + sourceGuid + "\"");
            // 真实恢复序列化字段和全部图集像素，再验证；不能只ClearDirty隐藏缓存变化。
            foreach (var pair in pairs.Where(p => !(p.Item1 is Texture2D)))
            {
                string name = pair.Item2.name;
                EditorJsonUtility.FromJsonOverwrite(Normalized(pair.Item1), pair.Item2);
                pair.Item2.name = name;
            }
            for (int i = 0; i < pristine.atlasTextures.Length; i++)
            {
                var src = pristine.atlasTextures[i];
                if (src == null) continue;
                var dst = font.atlasTextures[i];
                if (dst == null || dst.width != src.width || dst.height != src.height || dst.format != src.format)
                    throw new InvalidOperationException("Font atlas shape changed; keep snapshot");
                dst.LoadRawTextureData(src.GetRawTextureData());
                dst.Apply(false, false);
                if (!dst.GetRawTextureData().SequenceEqual(src.GetRawTextureData())) throw new InvalidOperationException("Font pixels not restored");
            }
            font.ReadFontAssetDefinition();
            if (font.material == null || AssetDatabase.GetAssetPath(font.material) != FontPath ||
                font.atlasTextures.Where(t => t != null).Any(t => AssetDatabase.GetAssetPath(t) != FontPath) ||
                Enumerable.Range(0, font.atlasTextureCount).Any(i => font.atlasTextures[i] == null))
                throw new InvalidOperationException("Font references must point to original owned subassets, not snapshot");
            foreach (var pair in pairs.Where(p => !(p.Item1 is Texture2D)))
            {
                // CopyAsset可能改主对象名称，只允许这一项规范化；所有实际字段/引用必须一致。
                string actual = EditorJsonUtility.ToJson(pair.Item2);
                string wanted = Normalized(pair.Item1);
                wanted = wanted.Replace("\"m_Name\":\"" + pair.Item1.name + "\"", "\"m_Name\":\"" + pair.Item2.name + "\"");
                if (actual != wanted) throw new InvalidOperationException("Font serialized state not restored: " + pair.Item2.name);
            }
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(FontPath)) EditorUtility.ClearDirty(asset);
            if (!AssetDatabase.DeleteAsset(folder)) throw new InvalidOperationException("Owned font snapshot cleanup failed");
            SessionState.EraseString(ScopeKey + ".Snapshot");
        }
        if (Hash() != Expected || AssetDatabase.LoadAllAssetsAtPath(FontPath).Any(EditorUtility.IsDirty)) throw new InvalidOperationException("Font cache not fully restored; keep resource scope for review");
        int count = SessionState.GetInt(WritesKey, 0);
        SessionState.SetBool(ScopeKey, false);
        return "scope-restored|font|" + count + "|" + font.characterTable.Count;
    }
}

public sealed class StudioProtectedFontSaveGuard : AssetModificationProcessor
{
    private static string[] OnWillSaveAssets(string[] paths)
    {
        if (!StudioTestResourceScope.Active) return paths;
        return paths.Where(p =>
        {
            string normalized = p.Replace('\\', '/');
            if (!normalized.Equals(StudioTestResourceScope.FontPath, StringComparison.OrdinalIgnoreCase) &&
                !normalized.Equals(StudioTestResourceScope.FontPath + ".meta", StringComparison.OrdinalIgnoreCase)) return true;
            SessionState.SetInt(StudioTestResourceScope.WritesKey, SessionState.GetInt(StudioTestResourceScope.WritesKey, 0) + 1);
            Debug.Log("[Studio test resource isolation] blocked automatic font save: " + p);
            return false;
        }).ToArray();
    }
}
