using UnityEngine;
using UnityEditor;
using TMPro;
public static class MenuArtApply
{
    static readonly Color Paper=new Color(.95f,.90f,.80f,.98f), Ink=new Color(.22f,.17f,.12f);
    static RectTransform Rect(Transform parent,string name,Vector2 pos,Vector2 size)
    {
        var found=parent.Find(name) as RectTransform;
        if(found==null){var go=new GameObject(name,typeof(RectTransform));found=go.GetComponent<RectTransform>();found.SetParent(parent,false);}
        found.anchorMin=found.anchorMax=found.pivot=new Vector2(.5f,.5f);found.anchoredPosition=pos;found.sizeDelta=size;return found;
    }
    static void Backdrop(Transform parent)
    {
        var rt=Rect(parent,"AdventureBackdrop",Vector2.zero,Vector2.zero);rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;rt.SetAsFirstSibling();
        var image=rt.GetComponent<UnityEngine.UI.Image>()??rt.gameObject.AddComponent<UnityEngine.UI.Image>();
        image.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Prototype/Journey/ChatGPT Image 2026年9月19日 21_24_25.png");image.color=new Color(.58f,.58f,.52f);image.raycastTarget=false;
    }
    static void Panel(Transform parent,string name,Vector2 pos,Vector2 size)
    {
        var rt=Rect(parent,name,pos,size);var i=rt.GetComponent<HomePanelImage>()??rt.gameObject.AddComponent<HomePanelImage>();i.color=Paper;i.raycastTarget=false;rt.SetSiblingIndex(1);
    }
    static void Label(Transform parent,string name,string value,Vector2 pos,Vector2 size,int font)
    {
        var rt=Rect(parent,name,pos,size);var t=rt.GetComponent<TextMeshProUGUI>()??rt.gameObject.AddComponent<TextMeshProUGUI>();t.text=value;t.font=CurrentUIFontAuthority.Default;t.fontSize=font;t.color=Ink;t.alignment=TextAlignmentOptions.Center;t.raycastTarget=false;
    }
    static void Button(UnityEngine.UI.Button b,Vector2 pos,Vector2 size)
    {
        var rt=b.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;
        var image=b.GetComponent<UnityEngine.UI.Image>();image.sprite=null;image.color=Color.white;
        var c=b.colors;c.normalColor=new Color(.88f,.80f,.64f);c.highlightedColor=new Color(.99f,.94f,.83f);c.pressedColor=new Color(.68f,.49f,.28f);c.selectedColor=c.normalColor;c.disabledColor=new Color(.74f,.70f,.61f,.75f);b.colors=c;
        var t=b.GetComponentInChildren<TMP_Text>(true);t.font=CurrentUIFontAuthority.Default;t.fontSize=28;t.color=Ink;t.alignment=TextAlignmentOptions.Center;
    }
    static void Save(){var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);}
    public static string MainMenu()
    {
        if(EditorApplication.isPlaying)return "Play active; stopped";
        var m=Object.FindObjectOfType<MainMenuUI>(true);var root=m.canvasRoot.transform;Backdrop(root);Panel(root,"MenuPaper",Vector2.zero,new Vector2(520,660));
        Label(root,"GameTitle","RRPG",new Vector2(0,230),new Vector2(440,100),54);
        Button(m.btnContinue,new Vector2(0,80),new Vector2(320,64));Button(m.btnNewGame,new Vector2(0,-20),new Vector2(320,64));Button(m.btnDeleteSave,new Vector2(0,-150),new Vector2(260,56));
        var t=m.infoText;t.rectTransform.anchorMin=t.rectTransform.anchorMax=new Vector2(.5f,.5f);t.rectTransform.anchoredPosition=new Vector2(0,-250);t.rectTransform.sizeDelta=new Vector2(440,70);t.font=CurrentUIFontAuthority.Default;t.fontSize=18;t.color=Ink;t.text="";
        Save();return "Main menu existing buttons retained and skinned";
    }
    public static string HeroMenu()
    {
        if(EditorApplication.isPlaying)return "Play active; stopped";
        var h=Object.FindObjectOfType<HeroSelectionUI>(true);var root=h.canvasRoot.transform;Backdrop(root);
        Panel(root,"HeroSelectionPaper",Vector2.zero,new Vector2(1200,760));
        foreach(var t in root.GetComponentsInChildren<TMP_Text>(true))if(t.name=="TitleText"){t.rectTransform.anchorMin=t.rectTransform.anchorMax=new Vector2(.5f,.5f);t.rectTransform.anchoredPosition=new Vector2(0,300);t.rectTransform.sizeDelta=new Vector2(1000,80);t.font=CurrentUIFontAuthority.Default;t.fontSize=40;t.color=Ink;t.alignment=TextAlignmentOptions.Center;}
        Button(h.btnConfirm,new Vector2(0,-290),new Vector2(320,64));
        var path=AssetDatabase.GetAssetPath(h.heroCardPrefab);var card=PrefabUtility.LoadPrefabContents(path);
        try{
            card.GetComponent<RectTransform>().sizeDelta=new Vector2(300,420);card.GetComponent<UnityEngine.UI.Image>().color=Paper;
            var a=card.transform.Find("Anchor_Visual") as RectTransform;a.anchorMin=a.anchorMax=new Vector2(.5f,1);a.pivot=new Vector2(.5f,1);a.anchoredPosition=new Vector2(0,-18);a.sizeDelta=new Vector2(136,136);
            foreach(var t in card.GetComponentsInChildren<TMP_Text>(true)){
                float y=t.name=="Text_Name"?20:t.name=="Text_HP"?-28:t.name=="Text_ATK"?-68:-140;
                var rt=t.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=new Vector2(0,y);rt.sizeDelta=new Vector2(260,t.name=="Text_Skill"?96:44);
                t.font=CurrentUIFontAuthority.Default;t.fontSize=t.name=="Text_Name"?28:t.name=="Text_Skill"?20:22;t.color=Ink;t.alignment=TextAlignmentOptions.Center;t.enableWordWrapping=true;t.raycastTarget=false;
            }
            PrefabUtility.SaveAsPrefabAsset(card,path);
        }finally{PrefabUtility.UnloadPrefabContents(card);}
        var catalog=AssetDatabase.LoadAssetAtPath<PersonVisualBindingCatalog>("Assets/Resources/ScriptableObjects/Presentation/PersonVisualBindingCatalog.asset");foreach(var e in catalog.Entries)if(e.AppearanceKey=="hero_001")e.HeroSelectSpriteOverride=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Characters/ArtUpgrade01/portrait_red_cape_r1.png");EditorUtility.SetDirty(catalog);AssetDatabase.SaveAssetIfDirty(catalog);
        EditorUtility.SetDirty(h);Save();return "Existing hero card layout/portrait updated; no save/gameplay operation";
    }
}
