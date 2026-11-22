using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>B home shell inside the existing Town orchestration. Internal navigation is presentation only.</summary>
public sealed class HomeProductShell : MonoBehaviour
{
    public HomeProductRouter Router { get; }=new HomeProductRouter();
    public PersonDetailView PersonDetail { get; private set; }
    public ManagementView Management { get; private set; }
    public CharacterGrowthUI GrowthUI => FindObjectOfType<CharacterGrowthUI>(true);
    /// <summary>Surface 模态（人物 / 管理 / 成长 / 设置）统一复用既有 World 输入阻断，不新建第二套 Guard。</summary>
    public bool SurfaceModalActive => Router.Surface==HomeSurface.Person || Router.Surface==HomeSurface.Management || GrowthUI?.IsOpen==true || SettingsOpen;
    /// <summary>Home 右上「设置」菜单是否展开（纯本地 UI 状态，不进入存档、不改变世界速度）。</summary>
    public bool SettingsOpen => _settingsPanel!=null && _settingsPanel.activeSelf;
    public bool IsPersonModalOpen => SurfaceModalActive;
    private CanvasGroup _townInput;
    private bool _townBlocked,_oldTownInteractable,_oldTownRaycasts;
    public HomeVillageFeed Feed { get; private set; }
    public HomeBuildingPlacement Build { get; private set; }
    public IReadOnlyList<HomeAttentionItem> Attention { get; private set; }
    public bool ShellVisible => _canvas!=null && _canvas.enabled;
    public bool CompatibilityVisible { get; private set; }
    public string HudText => _date.text+" "+_speed.GetComponentInChildren<TMP_Text>().text+" "+_resources.text;
    private ProductHomeWorldHost _host;
    private TownUI _town;
    private Canvas _canvas;
    private TMP_Text _date,_resources,_feed,_surfaceTitle,_surfaceBody,_attentionText;
    private Button _pause,_speed,_attentionButton;
    private GameObject _surface,_attentionPanel;
    private Button _settingsButton;
    private GameObject _settingsPanel;
    private TMP_Text _settingsStatus;
    private readonly List<Button> _settingsActions=new List<Button>();
    private bool _settingsBusy;
    private int _lastSaveFrame=-1;
    private PlayerCollection _source;
    private float _nextRead;
    private readonly List<Button> _navigation=new List<Button>();
    private static readonly string[] LegacyNames={"InfoText","GoldText","CharCountText","ItemCountText","BtnViewDeck","BtnExpedition","VillageHUD","BtnCharacterGrowth","BtnVillageCommitment"};

    public void Initialize(ProductHomeWorldHost host)
    {
        _host=host;_town=FindObjectOfType<TownUI>(true);Feed=new HomeVillageFeed();Attention=System.Array.AsReadOnly(new HomeAttentionItem[0]);
        _canvas=gameObject.AddComponent<Canvas>();_canvas.renderMode=RenderMode.ScreenSpaceOverlay;_canvas.sortingOrder=70;
        gameObject.AddComponent<GraphicRaycaster>();var scaler=gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;
        var top=HomeProductUI.Panel(transform,"TopHUD",new Vector2(.5f,1),new Vector2(0,-16),new Vector2(1872,62),new Color(.03f,.065f,.075f,.87f));
        _date=HomeProductUI.Text(top,"DateClock",new Vector2(0,1),new Vector2(18,-17),new Vector2(360,36),"");
        _pause=HomeProductUI.Button(top,"Pause",new Vector2(0,1),new Vector2(385,-11),new Vector2(94,40),"暂停",TogglePause);
        _speed=HomeProductUI.Button(top,"CurrentSpeed",new Vector2(0,1),new Vector2(490,-11),new Vector2(100,40),"1×",CycleSpeed);
        // 顶栏右侧顺序：资源文本 ← 关注 ← 设置，避免相互遮挡。
        _resources=HomeProductUI.Text(top,"Resources",new Vector2(1,1),new Vector2(-294,-17),new Vector2(400,36),"");_resources.alignment=TextAlignmentOptions.TopRight;
        _attentionButton=HomeProductUI.Button(top,"Attention",new Vector2(1,1),new Vector2(-134,-11),new Vector2(150,40),"关注 0",()=>_attentionPanel.SetActive(!_attentionPanel.activeSelf));
        _settingsButton=HomeProductUI.Button(top,"Settings",new Vector2(1,1),new Vector2(-14,-11),new Vector2(110,40),"设置",ToggleSettings);
        _attentionPanel=HomeProductUI.Panel(transform,"AttentionPanel",Vector2.one,new Vector2(-24,-88),new Vector2(370,185),new Color(.035f,.075f,.095f,.96f)).gameObject;
        _attentionText=HomeProductUI.Text(_attentionPanel.transform,"ActionFacts",new Vector2(0,1),new Vector2(16,-14),new Vector2(338,105),"");
        HomeProductUI.Button(_attentionPanel.transform,"NavigateAttention",new Vector2(0,0),new Vector2(16,14),new Vector2(338,40),"查看下一项",NavigateNextAttention);
        _attentionPanel.SetActive(false);
        // 设置菜单：透明全屏遮罩接收 Raycast，阻断世界点击 / 建造 / 拖拽旁路；菜单本身只含三个动作。
        _settingsPanel=HomeProductUI.Panel(transform,"SettingsPanel",Vector2.zero,Vector2.zero,Vector2.zero,new Color(0,0,0,0)).gameObject;
        var settingsRect=_settingsPanel.GetComponent<RectTransform>();
        settingsRect.anchorMin=Vector2.zero;settingsRect.anchorMax=Vector2.one;settingsRect.offsetMin=Vector2.zero;settingsRect.offsetMax=Vector2.zero;
        _settingsPanel.GetComponent<Image>().raycastTarget=true;
        var menu=HomeProductUI.Panel(_settingsPanel.transform,"SettingsMenu",Vector2.one,new Vector2(-14,-60),new Vector2(260,214),new Color(.035f,.075f,.095f,.98f));
        _settingsStatus=HomeProductUI.Text(menu,"SettingsStatus",new Vector2(0,0),new Vector2(14,8),new Vector2(232,40),"",15);
        _settingsActions.Add(HomeProductUI.Button(menu,"SettingsSave",new Vector2(0,1),new Vector2(14,-14),new Vector2(232,40),"保存游戏",()=>SettingsSave(false)));
        _settingsActions.Add(HomeProductUI.Button(menu,"SettingsSaveAndExit",new Vector2(0,1),new Vector2(14,-62),new Vector2(232,40),"保存并返回主菜单",()=>SettingsSave(true)));
        _settingsActions.Add(HomeProductUI.Button(menu,"SettingsClose",new Vector2(0,1),new Vector2(14,-110),new Vector2(232,40),"关闭",CloseSettings));
        _settingsPanel.SetActive(false);
        var nav=HomeProductUI.Rect(transform,"ProductNavigation",new Vector2(.5f,0),new Vector2(0,24),new Vector2(1080,50));
        string[] labels={"村庄","人物","管理","卡牌","历史","远征 ↗","建造"};
        for(int i=0;i<labels.Length;i++)
        {
            int index=i;
            var button=HomeProductUI.Button(nav,"Navigate"+i,Vector2.zero,new Vector2(i*155,0),new Vector2(147,50),labels[i],()=>Navigate(index));
            _navigation.Add(button);
        }
        HomeProductUI.Button(transform,"DebugCompatibility",Vector2.zero,new Vector2(24,24),new Vector2(155,38),"调试 / 对照",ToggleCompatibility);
        HomeProductUI.Button(transform,"R2VisualProfile",new Vector2(1,0),new Vector2(-24,24),new Vector2(220,38),"R2 对照：A 白模",()=>{
            _host.SetVisualPrototype(!_host.VisualPrototypeEnabled);
            transform.Find("R2VisualProfile").GetComponentInChildren<TMP_Text>().text=_host.VisualPrototypeEnabled ? "R2 对照：B 原型" : "R2 对照：A 白模";
        });
        HomeProductUI.Text(transform,"R2PrototypeNotice",new Vector2(1,0),new Vector2(-24,68),new Vector2(320,30),"TECHNICAL PROTOTYPE / NOT FINAL ART",13).alignment=TextAlignmentOptions.Right;
        _feed=HomeProductUI.Text(transform,"VillageFeed",Vector2.zero,new Vector2(24,98),new Vector2(525,108),"",19);
        _surface=HomeProductUI.Panel(transform,"HomeInternalWhitebox",new Vector2(0,1),new Vector2(24,-105),new Vector2(620,700),new Color(.035f,.07f,.085f,.94f)).gameObject;
        _surfaceTitle=HomeProductUI.Text(_surface.transform,"Title",new Vector2(0,1),new Vector2(24,-24),new Vector2(490,44),"",28);
        HomeProductUI.Button(_surface.transform,"CloseToWorld",Vector2.one,new Vector2(-18,-18),new Vector2(70,40),"关闭",CloseSurface);
        _surfaceBody=HomeProductUI.Text(_surface.transform,"WhiteboxContent",new Vector2(0,1),new Vector2(24,-95),new Vector2(572,540),"",22);
        _surface.SetActive(false);
        Build=gameObject.AddComponent<HomeBuildingPlacement>();Build.Initialize(host);
        var authoredManagement=Resources.Load<ManagementView>("UI/ManagementView");
        if(authoredManagement==null) Debug.LogError("[Management] Missing authored UI/ManagementView prefab.");
        else
        {
            Management=Instantiate(authoredManagement,transform);Management.name="ManagementView";
            var managementRect=Management.GetComponent<RectTransform>();managementRect.localScale=Vector3.one;managementRect.anchorMin=Vector2.zero;managementRect.anchorMax=Vector2.one;managementRect.sizeDelta=Vector2.zero;managementRect.anchoredPosition=Vector2.zero;
            Management.Initialize(host,this);
            Management.PersonDetailRequested=id=>{ if(string.IsNullOrEmpty(id))return; host.Selection.SelectPerson(id); OpenSurface(HomeSurface.Person); };
        }
        var authored=Resources.Load<PersonDetailView>("UI/PersonDetailView");
        if(authored==null) { Debug.LogError("[PersonDetail] Missing authored UI/PersonDetailView prefab."); return; }
        PersonDetail=Instantiate(authored,transform);PersonDetail.name="PersonDetail";
        var personRect=PersonDetail.GetComponent<RectTransform>();personRect.localScale=Vector3.one;personRect.anchorMin=Vector2.zero;personRect.anchorMax=Vector2.one;personRect.sizeDelta=Vector2.zero;personRect.anchoredPosition=Vector2.zero;
        PersonDetail.Initialize(host,this);
    }
    private void LateUpdate()
    {
        if(_host==null) return;
        _town=FindObjectOfType<TownUI>(true);
        _canvas.enabled=_host.WorldVisible && VillageRuntimeBridge.IsSimulationActive;
        RetirePrototype();
        if(!ShellVisible) { if(Management!=null && Management.gameObject.activeSelf) Management.gameObject.SetActive(false); return; }
        if(SampleEscapePressed()) HandleEscape();
        if(Time.unscaledTime>=_nextRead) { _nextRead=Time.unscaledTime+.2f;RefreshNow(); }
    }
    /// <summary>
    /// ESC 的逐帧采样入口：生产默认仍是 Unity legacy Input.GetKeyDown(KeyCode.Escape)；
    /// Editor 测试只可注入“一帧按下”，由真实 LateUpdate 走本方法消费一次，随后立即回落为生产采样，
    /// 不形成长按 / 重复关闭，也不绕过 HandleEscape 的 Build.IsOpen 与调用方的 ShellVisible 门禁。
    /// </summary>
    private bool SampleEscapePressed()
    {
#if UNITY_EDITOR
        if(_escapePressPendingForTests)
        {
            _escapePressPendingForTests=false;
            return true;
        }
#endif
        return Input.GetKeyDown(KeyCode.Escape);
    }
#if UNITY_EDITOR
    // 仅 Editor 编译存在的实例级一次性 ESC 采样注入（Player 编译中字段 / 方法 / 分支全部不存在）。
    // true = 下一次采样返回一次“按下”后立即回落为 false；不写盘、不改任何 Authority、不新增 public API。
    private bool _escapePressPendingForTests;
    /// <summary>【Editor-only internal 实例级测试缝】注入一帧 ESC 按下，由真实 LateUpdate 采样并消费一次。</summary>
    internal void SetEscapePressForTests() { _escapePressPendingForTests=true; }
    /// <summary>【Editor-only internal】清除未消费的注入（可逆、幂等），恢复生产 Input.GetKeyDown 采样。</summary>
    internal void ClearEscapePressForTests() { _escapePressPendingForTests=false; }
#endif
    private void RetirePrototype()
    {
        if(_town?.canvasRoot==null) return;
        foreach(string name in LegacyNames)
        {
            var child=_town.canvasRoot.transform.Find(name);
            if(child!=null && child.gameObject.activeSelf!=CompatibilityVisible) child.gameObject.SetActive(CompatibilityVisible);
        }
    }
    public void RefreshNow()
    {
        transform.Find("R2VisualProfile").GetComponentInChildren<TMP_Text>().text=_host.VisualPrototypeEnabled ? "R2 对照：B 原型" : "R2 对照：A 白模";
        SyncSourceReset();
        Attention=HomeAttentionProjection.Read();
        var clock=VillageRuntimeBridge.Time;var resources=VillageRuntimeBridge.Resources;
        _date.text=clock==null ? "—" : $"{clock.Year}年 {clock.Month}月 {clock.Day}日   {clock.MinuteOfDay/60:00}:{clock.MinuteOfDay%60:00}";
        _pause.GetComponentInChildren<TMP_Text>().text=clock?.Speed==VillageTime.VillageTimeSpeed.Paused ? "继续" : "暂停";
        _speed.GetComponentInChildren<TMP_Text>().text=(clock?.SpeedMultiplier ?? 0).ToString("0.#",System.Globalization.CultureInfo.InvariantCulture)+"×";
        _resources.text=$"食物 {resources?.Food ?? 0}     食材 {resources?.Ingredients ?? 0}";
        _attentionButton.GetComponentInChildren<TMP_Text>().text="关注 "+Attention.Count;
        _attentionText.text=Attention.Count==0 ? "当前没有待处理事项" : string.Join("\n",Attention.Take(3).Select(a=>a.Subject+" · "+a.Message));
        _feed.text=string.Join("\n",Feed.Recent.Select(e=>"· "+e.Message));
        var subjects=new HashSet<string>(Attention.Where(a=>a.Intent==HomeAttentionIntent.PersonGrowth).Select(a=>a.SubjectId));
        foreach(var view in _host.PersonViews) view.Value.SetAttention(subjects.Contains(view.Key));
        _surface.SetActive(Router.Surface==HomeSurface.Cards || Router.Surface==HomeSurface.History);
        if(Management!=null)
        {
            bool show=Router.Surface==HomeSurface.Management && ShellVisible;
            if(Management.gameObject.activeSelf!=show) Management.gameObject.SetActive(show);
            if(show) Management.RefreshNow();
        }
        if(PersonDetail!=null) { PersonDetail.gameObject.SetActive(Router.Surface==HomeSurface.Person && ShellVisible); if(PersonDetail.IsVisible) PersonDetail.RefreshNow(); }
        SetTownInputBlocked(SurfaceModalActive && ShellVisible);
        _attentionButton.interactable=!SurfaceModalActive;
        if(SurfaceModalActive) _attentionPanel.SetActive(false);
        _host.Inspector.SetVisible(_host.WorldVisible && Router.Surface==HomeSurface.World);
        foreach(var pair in _navigation.Select((button,index)=>new{button,index}))
            pair.button.GetComponent<Image>().color=(int)Router.Surface==pair.index ? new Color(.28f,.43f,.36f,.96f) : new Color(.14f,.24f,.24f,.94f);
        RefreshSurface();
    }
    /// <summary>
    /// 消费“长期人物来源被替换”（Continue / Return Load）时的既有复位：清会话 Feed、回 World、收起关注与成长页。
    /// 该复位必须先于用户本次导航被消费，否则随后的 RefreshNow 会把刚打开的 Surface 一并复位掉。
    /// 不新增 Authority / 公共 API，也不改写来源切换清旧 Selection（host）与候选（ManagementView）的既有归属。
    /// </summary>
    private void SyncSourceReset()
    {
        if(ReferenceEquals(_source,VillageRuntimeBridge.SimulationPersonSource)) return;
        _source=VillageRuntimeBridge.SimulationPersonSource;Feed.ResetSession();Router.Close();_attentionPanel.SetActive(false);
        var growth=GrowthUI;if(growth!=null && growth.IsOpen && !growth.HasPendingChoice) growth.TryHide();
    }
    public void OpenSurface(HomeSurface surface)
    {
        SyncSourceReset();
        if(GrowthUI?.HasPendingChoice==true) { GrowthUI.TryHide();return; }
        if(GrowthUI?.IsOpen==true) { GrowthUI.TryHide(); }
        Build?.Cancel();Router.Open(surface);RefreshNow();
    }
    public void HandleEscape() { if(!Build.IsOpen) CloseSurface(); }
    public void CloseSurface()
    {
        var growth=GrowthUI;
        if(growth?.IsOpen==true && !growth.TryHide())return;
        Router.Close();RefreshNow();
    }
    // ---------------- 设置（保存 / 保存并返回主菜单 / 关闭） ----------------
    private void ToggleSettings()
    {
        if(_settingsPanel==null)return;
        if(_settingsPanel.activeSelf){CloseSettings();return;}
        if(SettingsBusy())return;
        _settingsPanel.SetActive(true);
        if(_settingsStatus!=null)_settingsStatus.text=string.Empty;
        SetSettingsBusy(false);
        RefreshNow();
    }
    private void CloseSettings()
    {
        if(_settingsPanel==null || !_settingsPanel.activeSelf)return;
        if(SettingsBusy())return;
        if(_settingsStatus!=null)_settingsStatus.text=string.Empty;
        _settingsPanel.SetActive(false);
        RefreshNow();
    }
    private bool SettingsBusy() => _settingsBusy;
    private void SetSettingsBusy(bool busy)
    {
        _settingsBusy=busy;
        foreach(var button in _settingsActions) if(button!=null) button.interactable=!busy;
        if(_settingsButton!=null) _settingsButton.interactable=!busy;
    }
    private void SetSettingsStatus(string text)
    {
        if(_settingsStatus!=null)_settingsStatus.text=text==null?string.Empty:text;
    }
    /// <summary>
    /// 设置里的保存入口。复用唯一 SaveSystem.Save 与当前 Town 的同一 PlayerCollection：
    /// 不 NewGame、不重建实体、不写第二份序列化；失败留在 Home 并显示原因。
    /// </summary>
    private void SettingsSave(bool returnToMenu)
    {
        if(_settingsBusy)return;
        // 同一帧内重复触发（例如双击 / 同帧两次 Invoke）只允许一次存档事务，避免重复写盘。
        if(Time.frameCount==_lastSaveFrame)return;
        _lastSaveFrame=Time.frameCount;
        var town=_town!=null?_town:FindObjectOfType<TownUI>(true);
        var data=town!=null?town.PlayerData:null;
        var menu=returnToMenu?FindObjectOfType<MainMenuUI>(true):null;
        if(returnToMenu && menu==null){SetSettingsStatus("保存并返回失败：主菜单不可用。");return;}
        if(data==null){SetSettingsStatus("保存失败：当前世界数据未就绪。");return;}
        if(!VillageRuntimeBridge.CanSaveWorld){SetSettingsStatus("保存失败：当前世界状态不允许保存（存在进行中事务）。");return;}
        SetSettingsBusy(true);
        bool ok=false;string error=null;
        try { ok=SaveSystem.Save(data); }
        catch(System.Exception ex) { error=ex.GetType().Name+": "+ex.Message; }
        finally { SetSettingsBusy(false); }
        if(!ok)
        {
            // 失败保留菜单、世界与当前选择，只显示可理解原因。
            SetSettingsStatus("保存失败："+(error??"存档未写入（可能被守卫拒绝或磁盘不可写）。"));
            RefreshNow();
            return;
        }
        if(!returnToMenu){SetSettingsStatus("保存成功。");RefreshNow();return;}
        // 仅成功后由既有主菜单宿主接缝返回；不 NewGame、不创建第二套主菜单。
        _settingsPanel.SetActive(false);
        menu.ShowMainMenuAfterHome();
    }
    private void OnDisable()
    {
        if(_settingsPanel!=null && _settingsPanel.activeSelf) _settingsPanel.SetActive(false);
        _settingsBusy=false;
    }
    private void SetTownInputBlocked(bool blocked)
    {
        if(_town?.canvasRoot==null)return;
        if(_townInput==null) { _townInput=_town.canvasRoot.GetComponent<CanvasGroup>(); if(_townInput==null) _townInput=_town.canvasRoot.AddComponent<CanvasGroup>(); }
        if(blocked && !_townBlocked) { _oldTownInteractable=_townInput.interactable;_oldTownRaycasts=_townInput.blocksRaycasts;_townInput.interactable=false;_townInput.blocksRaycasts=false;_townBlocked=true; }
        else if(!blocked && _townBlocked) { _townInput.interactable=_oldTownInteractable;_townInput.blocksRaycasts=_oldTownRaycasts;_townBlocked=false; }
    }
    public void TogglePause()
    { var clock=VillageRuntimeBridge.Time;if(clock==null) return;if(clock.Speed==VillageTime.VillageTimeSpeed.Paused) clock.Resume();else clock.Pause();RefreshNow(); }
    public void CycleSpeed() { VillageRuntimeBridge.Time?.CycleSpeed();RefreshNow(); }
    private void Navigate(int index)
    {
        if(SurfaceModalActive)return;
        if(index==6) {Build.OpenCatalog();return;}
        Build.Cancel();
        if(index<=4) OpenSurface((HomeSurface)index);
        else if(index==5)
        {
            Router.Close();
            if(_town==null) _town=FindObjectOfType<TownUI>(true);
            _town?.EnterProductExpedition();
            _canvas.enabled=VillageRuntimeBridge.IsSimulationActive;
        }
    }
    public void ToggleCompatibility()
    { if(SurfaceModalActive)return;CompatibilityVisible=!CompatibilityVisible;_host.ToggleDebug();RetirePrototype(); }
    public void NavigateAttention(HomeAttentionItem item)
    {
        if(SurfaceModalActive || item==null || !Attention.Any(a=>a.SubjectId==item.SubjectId && a.Intent==item.Intent)) return;
        if(item.Intent==HomeAttentionIntent.PersonGrowth) { _host.Selection.SelectPerson(item.Target);OpenSurface(HomeSurface.Person);PersonDetail?.SelectTab(PersonDetailTab.Growth); }
        else FindObjectOfType<VillageSocialEventUI>(true)?.ShowAdvancePending(item.Target);
    }
    private int _attentionCursor;
    private void NavigateNextAttention()
    { RefreshNow();if(Attention.Count>0) NavigateAttention(Attention[_attentionCursor++%Attention.Count]); }
    private void RefreshSurface()
    {
        var snapshot=_host.Snapshot;if(snapshot==null || Router.Surface==HomeSurface.World || Router.Surface==HomeSurface.Management) return;
        const string note="产品白模 · 只读框架\n村庄持续运行，可使用顶部时间控制。\n\n";
        switch(Router.Surface)
        {
            case HomeSurface.Person:
                _surfaceTitle.text="人物";
                var p=snapshot.Person(_host.Selection.Id);
                _surfaceBody.text=note+(p==null ? "从世界选择一位人物，即可在同一 Home 查看。\n\n"+string.Join("、",snapshot.Persons.Select(v=>v.DisplayName)) : p.DisplayName+"\n\n职业："+p.Profession+"\n活动："+p.ActivitySummary+"\n职责："+p.AssignmentSummary+"\n\n概览 / 生活 / 职业成长 / 关系 / 人生经历\n完整页面待后续实现。");break;
            case HomeSurface.Cards:
                _surfaceTitle.text="卡牌";_surfaceBody.text=note+"角色牌 / 通用牌\n\n职业理论牌池 / 人物已解锁牌池\n\n正式卡牌浏览待后续实现。";break;
            case HomeSurface.History:
                _surfaceTitle.text="历史";_surfaceBody.text=note+"编年史 / 人物档案 / 村庄历史\n\n长期历史查询页面待后续实现。\n左下近期动态仅属于当前会话。";break;
        }
    }
    private void OnDestroy() { SetTownInputBlocked(false);Feed?.Dispose(); }
}
