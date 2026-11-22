using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class HomeUIVerification
{
    private static void Click(Transform root,string path)
    {
        var target=root.Find(path);
        if(target==null)throw new Exception("Missing UI: "+path);
        var button=target.GetComponent<UnityEngine.UI.Button>();
        if(button==null || !button.interactable)throw new Exception("Unavailable UI: "+path);
        Canvas.ForceUpdateCanvases();
        var rt=(RectTransform)target;
        var data=new PointerEventData(EventSystem.current){position=RectTransformUtility.WorldToScreenPoint(null,rt.TransformPoint(rt.rect.center)),button=PointerEventData.InputButton.Left};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
        if(hits.Count==0 || hits[0].gameObject.GetComponentInParent<UnityEngine.UI.Button>()!=button)throw new Exception("UI raycast blocked: "+path);
        ExecuteEvents.ExecuteHierarchy(hits[0].gameObject,data,ExecuteEvents.pointerClickHandler);
    }
    public static string Run()
    {
        var host=UnityEngine.Object.FindObjectOfType<ProductHomeWorldHost>(true);
        if(host==null || !host.WorldVisible)throw new Exception("Real Home required");
        var shell=host.Shell;var root=shell.transform;
        const string key="RRPG.Art.UIProbe";
        int stage=UnityEditor.SessionState.GetInt(key,0);
        if(UnityEngine.Object.FindObjectsOfType<EventSystem>().Length!=1 || shell.GetComponent<UnityEngine.UI.GraphicRaycaster>()==null)throw new Exception("Input host mismatch");
        // Each tool call permits a real player loop between newly opened panels.
        if(stage==0)
        {
            host.SetVisualPrototype(true);host.RefreshNow();
            if(VillageRuntimeBridge.Time.Speed!=VillageTime.VillageTimeSpeed.Paused)shell.TogglePause();
            shell.CloseSurface();shell.RefreshNow();
            UnityEditor.SessionState.SetString(key+".Before",JsonUtility.ToJson(VillageRuntimeBridge.BuildSaveData()));
            Click(root,"ProductNavigation/Navigate3");
        }
        else if(stage==1)
        {
            if(shell.Router.Surface!=HomeSurface.Cards)throw new Exception("Cards navigation failed");
            Click(root,"HomeInternalWhitebox/CloseToWorld");Click(root,"ProductNavigation/Navigate4");
        }
        else if(stage==2)
        {
            if(shell.Router.Surface!=HomeSurface.History)throw new Exception("History navigation failed");
            Click(root,"HomeInternalWhitebox/CloseToWorld");Click(root,"ProductNavigation/Navigate6");
        }
        else if(stage==3)
        {
            if(!shell.Build.IsOpen)throw new Exception("Build navigation failed");
            shell.RefreshNow();
            if(root.Find("ProductNavigation/Navigate6").GetComponent<UnityEngine.UI.Button>().colors.normalColor==root.Find("ProductNavigation/Navigate0").GetComponent<UnityEngine.UI.Button>().colors.normalColor)throw new Exception("Build navigation lacks selected state");
            Click(root,"BuildingCatalog/CatalogRows/Choose_building_house");
            if(!shell.Build.GhostVisible)throw new Exception("Build preview failed");
            Click(root,"BuildingCatalog/CancelPlacement");
            if(shell.Build.IsOpen)throw new Exception("Build cancel failed");
            Click(root,"TopHUD/Settings");
        }
        if(stage<4){UnityEditor.SessionState.SetInt(key,stage+1);return "STEP "+(stage+1)+"/5 passed; wait for actual player loop";}
        Canvas.ForceUpdateCanvases();
        if(!shell.SettingsOpen || !shell.IsPersonModalOpen)throw new Exception("Settings guard failed");
        var maskData=new PointerEventData(EventSystem.current){position=new Vector2(Screen.width*.5f,Screen.height*.5f)};
        var maskHits=new List<RaycastResult>();EventSystem.current.RaycastAll(maskData,maskHits);
        if(!maskHits.Any(h=>h.gameObject.name=="SettingsPanel"))throw new Exception("Transparent settings mask lost raycast");
        string selected=host.Selection.Kind+":"+host.Selection.Id;
        host.HandleWorldClick(maskData.position);
        if(selected!=host.Selection.Kind+":"+host.Selection.Id)throw new Exception("Modal allowed world click");
        Click(root,"SettingsPanel/SettingsMenu/SettingsClose");
        if(shell.SettingsOpen || shell.IsPersonModalOpen)throw new Exception("Settings close failed");
        foreach(string name in new[]{"NavigationBackdrop","VillageFeedBackdrop"})
            if(root.Find(name).GetComponent<UnityEngine.UI.Image>().raycastTarget)throw new Exception("Decoration blocks clicks: "+name);
        var top=(RectTransform)root.Find("TopHUD");var corners=new Vector3[4];top.GetWorldCorners(corners);
        if(corners[0].x<0 || corners[2].x>Screen.width+1)throw new Exception("HUD outside viewport");
        if(UnityEditor.SessionState.GetString(key+".Before","")!=JsonUtility.ToJson(VillageRuntimeBridge.BuildSaveData()))throw new Exception("UI probe changed simulation data");
        UnityEditor.SessionState.EraseInt(key);UnityEditor.SessionState.EraseString(key+".Before");
        shell.RefreshNow();
        return "PASS: real Continue/Home; EventSystem=1; actual raycast+pointer clicks for Cards, History, close, build catalog/choose/cancel and Settings/close; transparent modal mask blocks; decorative backplates do not block; HUD in viewport; simulation save data unchanged; Screen="+Screen.width+"x"+Screen.height;
    }
}
