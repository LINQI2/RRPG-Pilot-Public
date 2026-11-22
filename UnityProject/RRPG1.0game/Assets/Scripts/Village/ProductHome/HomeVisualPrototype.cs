using System.Collections.Generic;
using System.Linq;
using RRPG.StaticConfig;
using UnityEngine;

/// <summary>Session-only B ground art. Decorative paths do not declare navigation or arrival.</summary>
public sealed class HomeVisualPrototype : MonoBehaviour
{
    private readonly List<Material> _materials=new List<Material>();
    private const float TileSize=4f;

    public void Initialize()
    {
        var grass=Terrain("va_village_r5_grass",new Color(.53f,.61f,.42f));
        var earth=Terrain("va_village_r5_earth",new Color(.78f,.68f,.48f));
        Patch("Grass surface",Vector2.zero,new Vector2(60,40),grass,0,-20000);
        // Presentation-only routes around the initial housing row. Saved building coordinates remain facts.
        Patch("Village path",new Vector2(-1,6.3f),new Vector2(22,.9f),earth,.16f,-19990);
        Patch("Tavern path",new Vector2(7,1.65f),new Vector2(.9f,10.2f),earth,.16f,-19990);
        Patch("Garden access",new Vector2(-6.8f,3.6f),new Vector2(.85f,5.8f),earth,.16f,-19990);
    }

    private Material Terrain(string id,Color fallback)
    {
        var material=new Material(Shader.Find("Sprites/Default"));
        if(StaticConfigRegistry.TryGetVisualAssets(out var catalog,out var failure)
            && catalog.TryGetById(id,out var asset) && asset.Sprite!=null)
        {
            material.mainTexture=asset.Sprite.texture;
            var rect=asset.Sprite.textureRect;
            material.mainTextureScale=new Vector2(rect.width/asset.Sprite.texture.width,rect.height/asset.Sprite.texture.height);
            material.mainTextureOffset=new Vector2(rect.x/asset.Sprite.texture.width,rect.y/asset.Sprite.texture.height);
            material.color=Color.white;
        }
        else
        {
            material.color=fallback;
            Debug.LogWarning("[ProductHome] Missing terrain VisualAsset: "+id+" "+failure);
        }
        _materials.Add(material);return material;
    }

    private static float[] Cuts(float min,float max,float feather)
    {
        var cuts=new SortedSet<float>{min,max};
        if(feather>0){cuts.Add(min+feather);cuts.Add(max-feather);}
        for(float p=Mathf.Ceil(min/TileSize)*TileSize;p<max;p+=TileSize)if(p>min)cuts.Add(p);
        return cuts.ToArray();
    }

    private void Patch(string name,Vector2 center,Vector2 size,Material material,float feather,int order)
    {
        var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));
        go.layer=ProductHomeWorldHost.WorldLayer;go.transform.SetParent(transform,false);
        var min=center-size*.5f;var max=center+size*.5f;
        var xs=Cuts(min.x,max.x,feather);var ys=Cuts(min.y,max.y,feather);
        var vertices=new List<Vector3>();var uv=new List<Vector2>();var colors=new List<Color>();var triangles=new List<int>();
        for(int x=0;x<xs.Length-1;x++)for(int y=0;y<ys.Length-1;y++)
        {
            float tileX=Mathf.Floor((xs[x]+xs[x+1])*.5f/TileSize)*TileSize;
            float tileY=Mathf.Floor((ys[y]+ys[y+1])*.5f/TileSize)*TileSize;
            var corners=new[]{new Vector2(xs[x],ys[y]),new Vector2(xs[x+1],ys[y]),new Vector2(xs[x+1],ys[y+1]),new Vector2(xs[x],ys[y+1])};
            int start=vertices.Count;
            foreach(var p in corners)
            {
                vertices.Add(BuildingWorldMapper.Offset(p));
                uv.Add(new Vector2((p.x-tileX)/TileSize,(p.y-tileY)/TileSize));
                float edge=Mathf.Min(p.x-min.x,max.x-p.x,p.y-min.y,max.y-p.y);
                colors.Add(new Color(1,1,1,feather>0 ? Mathf.SmoothStep(0,1,edge/feather) : 1));
            }
            triangles.AddRange(new[]{start,start+1,start+2,start,start+2,start+3});
        }
        var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetColors(colors);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();
        go.GetComponent<MeshFilter>().sharedMesh=mesh;
        var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.sortingOrder=order;
    }

    private void OnDestroy()
    {
        foreach(var filter in GetComponentsInChildren<MeshFilter>(true))Destroy(filter.sharedMesh);
        foreach(var material in _materials)if(material!=null)Destroy(material);
    }
}
