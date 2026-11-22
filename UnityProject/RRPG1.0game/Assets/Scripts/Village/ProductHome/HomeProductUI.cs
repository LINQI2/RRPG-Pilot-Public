using TMPro;
using UnityEngine;

/// <summary>Shared Home paper-and-wood skin, using the current shared font.</summary>
public static class HomeProductUI
{
    internal static readonly Color Paper=new Color(.95f,.90f,.80f,.98f);
    internal static readonly Color Ink=new Color(.22f,.17f,.12f);
    internal static readonly Color MutedInk=new Color(.43f,.35f,.25f);
    private static readonly Color ButtonPaper=new Color(.88f,.80f,.64f);
    private static readonly Color SelectedWood=new Color(.44f,.30f,.18f);
    public static RectTransform Rect(Transform parent,string name,Vector2 anchor,Vector2 position,Vector2 size)
    {
        var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);
        var rt=go.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=anchor;rt.pivot=anchor;rt.anchoredPosition=position;rt.sizeDelta=size;
        return rt;
    }
    public static RectTransform Panel(Transform parent,string name,Vector2 anchor,Vector2 position,Vector2 size,Color color)
    {
        var rt=Rect(parent,name,anchor,position,size);var image=rt.gameObject.AddComponent<HomePanelImage>();image.color=color;
        return rt;
    }
    public static TMP_Text Text(Transform parent,string name,Vector2 anchor,Vector2 position,Vector2 size,string value,int fontSize=20)
    {
        var rt=Rect(parent,name,anchor,position,size);var text=rt.gameObject.AddComponent<TextMeshProUGUI>();
        text.font=CurrentUIFontAuthority.Default;text.fontSize=fontSize;text.text=value;text.color=Ink;
        text.raycastTarget=false;text.enableWordWrapping=true;text.overflowMode=TextOverflowModes.Ellipsis;text.alignment=TextAlignmentOptions.TopLeft;
        return text;
    }
    public static UnityEngine.UI.Button Button(Transform parent,string name,Vector2 anchor,Vector2 position,Vector2 size,string label,System.Action action)
    {
        var rt=Panel(parent,name,anchor,position,size,Color.white);
        var button=rt.gameObject.AddComponent<UnityEngine.UI.Button>();button.onClick.AddListener(()=>action?.Invoke());
        var text=Text(rt,"Label",Vector2.zero,Vector2.zero,size,label);text.alignment=TextAlignmentOptions.Center;
        ApplyButtonSkin(button);
        return button;
    }
    internal static void ApplyButtonSkin(UnityEngine.UI.Button button,bool selected=false)
    {
        button.GetComponent<UnityEngine.UI.Image>().color=Color.white;
        var colors=button.colors;
        colors.normalColor=selected ? SelectedWood : ButtonPaper;
        colors.highlightedColor=selected ? new Color(.53f,.38f,.22f) : new Color(.99f,.94f,.83f);
        colors.pressedColor=new Color(.68f,.49f,.28f);
        colors.selectedColor=colors.normalColor;
        colors.disabledColor=new Color(.74f,.70f,.61f,.75f);
        colors.fadeDuration=.12f;button.colors=colors;
        var label=button.GetComponentInChildren<TMP_Text>();
        if(label!=null){label.color=selected ? Paper : Ink;label.fontStyle=selected ? FontStyles.Bold : FontStyles.Normal;}
    }
}
