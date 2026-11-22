using UnityEngine;

/// <summary>Native uGUI paper panel geometry; no texture or gameplay dependency.</summary>
public sealed class HomePanelImage : UnityEngine.UI.Image
{
    protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vertices)
    {
        vertices.Clear();var rect=GetPixelAdjustedRect();
        if(rect.width<=0 || rect.height<=0)return;
        var shadow=rect;shadow.y-=3;
        Rounded(vertices,shadow,8,new Color(.15f,.10f,.06f,color.a*.18f));
        Rounded(vertices,rect,8,new Color(.40f,.29f,.17f,color.a*.80f));
        rect.xMin+=1;rect.xMax-=1;rect.yMin+=1;rect.yMax-=1;
        Rounded(vertices,rect,7,color);
    }
    private static void Rounded(UnityEngine.UI.VertexHelper vertices,Rect rect,float radius,Color tint)
    {
        radius=Mathf.Min(radius,rect.width*.5f,rect.height*.5f);
        int start=vertices.currentVertCount;vertices.AddVert(rect.center,tint,Vector2.zero);
        var centers=new[]{new Vector2(rect.xMin+radius,rect.yMin+radius),new Vector2(rect.xMax-radius,rect.yMin+radius),new Vector2(rect.xMax-radius,rect.yMax-radius),new Vector2(rect.xMin+radius,rect.yMax-radius)};
        for(int corner=0;corner<4;corner++)for(int segment=0;segment<=6;segment++)
        {
            float angle=(180+corner*90+segment*15)*Mathf.Deg2Rad;
            vertices.AddVert(centers[corner]+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius,tint,Vector2.zero);
        }
        for(int i=0;i<28;i++)vertices.AddTriangle(start,start+1+i,start+1+(i+1)%28);
    }
}
