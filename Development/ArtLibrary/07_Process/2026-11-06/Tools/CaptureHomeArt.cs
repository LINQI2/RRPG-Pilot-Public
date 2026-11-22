using System;
using UnityEngine;

public static class CaptureHomeArt
{
    public static string Run()
    {
        if(!UnityEditor.EditorApplication.isPlaying || !SaveSystem.HasSave()) throw new Exception("Play/existing save required");
        var host=UnityEngine.Object.FindObjectOfType<ProductHomeWorldHost>(true);
        if(host==null || !host.WorldVisible)
        {
            var menu=UnityEngine.Object.FindObjectOfType<MainMenuUI>(true);
            typeof(MainMenuUI).GetMethod("OnContinue",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(menu,null);
            return "Continue requested; capture must run after the next product Update";
        }
        host.SetVisualPrototype(true);host.RefreshNow();
        string verification=HomeArtVerification.Run();
        var camera=host.WorldCamera.ViewCamera;
        host.WorldCamera.Focus(ProductHomeWorldHost.WorldOrigin+new Vector3(-2,1,0));
        camera.orthographicSize=8;
        var rt=new RenderTexture(1600,1000,24);
        var previous=camera.targetTexture;var active=RenderTexture.active;
        const string dir="D:/AI_Agent_Lab/rrpg_pilot/repo/Development/ArtLibrary/07_Process/2026-11-06/Evidence/";
        try
        {
            camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
            var texture=new Texture2D(1600,1000,TextureFormat.RGBA32,false);
            texture.ReadPixels(new Rect(0,0,1600,1000),0,0);texture.Apply();
            System.IO.File.WriteAllBytes(dir+"home-after-world-r3.png",texture.EncodeToPNG());
            UnityEngine.Object.Destroy(texture);
        }
        finally {camera.targetTexture=previous;RenderTexture.active=active;rt.Release();UnityEngine.Object.Destroy(rt);}
        ScreenCapture.CaptureScreenshot(dir+"home-after-game-r3.png");
        return verification+"; world image saved; full GameView capture requested; Screen="+Screen.width+"x"+Screen.height;
    }
}
