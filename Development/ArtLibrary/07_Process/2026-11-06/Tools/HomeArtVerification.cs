using System;
using System.Linq;
using UnityEngine;

public static class HomeArtVerification
{
    public static string Run()
    {
        var host=UnityEngine.Object.FindObjectOfType<ProductHomeWorldHost>(true);
        if(host==null || !host.WorldVisible) throw new Exception("Real Home is not visible");
        host.RefreshNow();
        Func<string> facts=()=>string.Join(";",host.Snapshot.Buildings.OrderBy(b=>b.InstanceId).Select(b=>b.InstanceId+":"+b.DefinitionId+":"+b.WorldX+":"+b.WorldY+":"+b.LifecycleState));
        string before=facts();
        var identities=host.BuildingViews.Values.Concat(host.PersonViews.Values).ToDictionary(v=>v.Identity,v=>v.GetInstanceID());
        float maxError=0;
        foreach(bool prototype in new[]{false,true,false,true})
        {
            host.SetVisualPrototype(prototype);host.RefreshNow();
            for(int x=-10;x<=10;x+=2)for(int y=-6;y<=6;y+=2)
            {
                var p=new Vector2(x+.25f,y+.75f);
                float error=Vector2.Distance(p,BuildingWorldMapper.ToLocal(BuildingWorldMapper.ToPresentation(p)));
                maxError=Mathf.Max(maxError,error);
                if(error>.002f) throw new Exception("Coordinate inverse failed: "+error);
            }
            if(facts()!=before) throw new Exception("Visual toggle changed building facts");
            foreach(var view in host.BuildingViews.Values.Concat(host.PersonViews.Values))
                if(identities[view.Identity]!=view.GetInstanceID() || !view.ResolvedSharedAsset) throw new Exception("Identity/asset failure: "+view.Identity);
        }
        Physics2D.SyncTransforms();
        var ground=host.GetComponentInChildren<HomeVisualPrototype>(true);
        var groundMeshes=ground.GetComponentsInChildren<MeshRenderer>(true);
        if(groundMeshes.Length!=4 || groundMeshes.Any(r=>r.sharedMaterial.mainTexture==null))throw new Exception("Ground art missing");
        var path=groundMeshes.Single(r=>r.name=="Village path").GetComponent<MeshFilter>().sharedMesh;
        if(!path.colors.Any(c=>c.a==0)||!path.colors.Any(c=>c.a==1))throw new Exception("Path feather missing");
        foreach(var building in host.BuildingViews.Values)
        {
            var hit=building.GetComponent<PolygonCollider2D>();
            if(hit==null || !hit.enabled || !hit.OverlapPoint(building.transform.position)) throw new Exception("Footprint collider failed: "+building.Identity);
            var line=building.GetComponentInChildren<LineRenderer>();
            if(line==null || Mathf.Abs(line.GetPosition(0).y-line.GetPosition(1).y)<.1f) throw new Exception("Footprint is not diagonal");
        }
        host.Shell.Build.OpenCatalog();
        if(!host.Shell.Build.Choose("building_house")) throw new Exception("Existing build catalog failed");
        var candidate=new Vector2(7.25f,-3.25f);
        host.Shell.Build.SetCandidate(candidate);
        var ghost=host.GetComponentsInChildren<SpriteRenderer>(true).Single(r=>r.name=="BuildingPlacementGhost");
        var outline=ghost.GetComponentInChildren<LineRenderer>();
        var center=Vector3.zero;
        for(int i=0;i<4;i++)center+=outline.transform.TransformPoint(outline.GetPosition(i))*.25f;
        float ghostError=Vector3.Distance(center,BuildingWorldMapper.ToPresentation(candidate));
        if(ghostError>.002f) throw new Exception("Ghost and footprint disagree: "+ghostError);
        bool ghostVisible=host.Shell.Build.GhostVisible;
        host.Shell.Build.Cancel();
        if(facts()!=before) throw new Exception("Preview changed building facts");
        return "PASS: real Home; 4 A/B transitions; 308 coordinate roundtrips; maxError="+maxError+"; persistent entities="+identities.Count+"; diagonal colliders="+host.BuildingViews.Count+"; ground meshes="+groundMeshes.Length+"; textured feathered paths; ghostVisible="+ghostVisible+"; ghostAnchorError="+ghostError+"; no placement/save invoked; frame="+Time.frameCount;
    }
}
