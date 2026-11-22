using System;
using System.Linq;
using UnityEngine;

public static class HomeScreenSelectionVerification
{
    public static string Run()
    {
        var host=UnityEngine.Object.FindObjectOfType<ProductHomeWorldHost>(true);
        Physics2D.SyncTransforms();
        var farm=host.BuildingViews.Values.First(v=>host.Snapshot.Building(v.Identity).DefinitionId=="building_farm");
        var tavern=host.PlaceViews.Values.Single(v=>v.Identity=="Tavern");
        foreach(var target in new[]{farm,tavern})
        {
            bool tested=false;
            foreach(var offset in new[]{Vector2.zero,new Vector2(.4f,0),new Vector2(-.4f,0),new Vector2(0,.3f),new Vector2(0,-.3f)})
            {
                var world=target.transform.position+(Vector3)offset;
                var hits=Physics2D.OverlapPointAll(world,1<<ProductHomeWorldHost.WorldLayer);
                // A live villager can stand on the anchor. Keep the real person-first picking policy.
                if(hits.Length==0 || hits.Any(c=>c.GetComponentInParent<HomeWorldEntityView>()!=target))continue;
                var screen=host.WorldCamera.ViewCamera.WorldToScreenPoint(world);
                if(host.PointerOverUI(screen))continue;
                host.HandleWorldClick(screen);
                if(host.Selection.Kind!=target.Kind || host.Selection.Id!=target.Identity)throw new Exception("Screen selection mismatch: "+target.Identity);
                tested=true;break;
            }
            if(!tested)throw new Exception("No unoccluded screen point: "+target.Identity);
        }
        host.Selection.Clear();host.RefreshNow();
        return "PASS: real screen-coordinate picking selects actual farm Building and fixed Tavern Place; no placement/save; frame="+Time.frameCount;
    }
}
