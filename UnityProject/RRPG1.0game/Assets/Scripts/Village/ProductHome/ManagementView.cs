using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>集中管理页签（固定顺序，与 Prefab 的 tabButtons 一一对应）。</summary>
public enum ManagementViewTab
{
    Population = 0,
    Workplaces = 1,
    Resources = 2
}

/// <summary>
/// V17-R3-B 集中管理页面控制器（Assembly-CSharp 产品层）。
///
/// 职责边界：
///   - 固定 UI 结构（标题 / 关闭 / 保存 / 状态 / 页签 / 检索 / 筛选 / 滚动容器 / 行模板 / 操作区）
///     必须由 Editor 创建的可编辑 Prefab 绑定；本组件不在 Awake 运行时搭建固定 UI。
///   - 运行时只生成重复行：克隆已绑定的行模板，并给管理行组件赋值。
///   - 唯一当前选择是既有 HomeSelectionContext；这里只保存“本次页面的行候选”：
///     一个岗位候选 WorkplaceId + 一个施工候选 BuildingInstanceId，不保存第二份长期 selectedPerson。
///   - 人口行单击只调用既有 HomeSelectionContext.SelectPerson 并留在集中管理页（不切 Surface）；
///     查看人物详情是详情面板上独立的 authored 按钮，由 Shell 注入的 PersonDetailRequested 导航。
///   - 数据只来自不可变 ManagementSnapshot（ManagementReadAdapter.Read() 一次读取），
///     命令只调用既有 ManagementCommandGateway，每次命令后重新读取，不在 View 直接写域。
///   - 不调用 Pause / Speed / SetSimulationActive，不创建第二 Canvas，不拦截 ESC；
///     关闭按钮与 ESC 由 Shell 的统一回调负责；全屏 Blocker / World 输入拦截由 Prefab + Shell 提供。
///
/// 下包（Editor Prefab + Shell 集成）接线契约见 Development/StudioRuns/R3B-UI-20261023/controller-report.md。
/// </summary>
public sealed class ManagementView : MonoBehaviour
{
    private const string PendingGrowthMessage = "请先完成成长选牌或明确跳过。";
    private const string WorldBusyMessage = "世界正在推进或仍有远征未完成，命令暂不可用。";

    [Header("标题与全局动作")]
    [SerializeField] private TMP_Text title;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private TMP_Text statusMessage;

    [Header("页签（固定顺序：人口 / 职责与场所 / 资源）")]
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private GameObject populationPage;
    [SerializeField] private GameObject workplacePage;
    [SerializeField] private GameObject resourcePage;

    [Header("人口：检索 / 状态筛选 / 汇总 / 列表")]
    [SerializeField] private TMP_InputField nameSearch;
    [SerializeField] private Button[] statusFilterButtons;
    [SerializeField] private TMP_Text populationSummary;
    [SerializeField] private ScrollRect populationScroll;
    [SerializeField] private RectTransform populationContent;
    [SerializeField] private TMP_Text populationEmpty;

    [Header("职责与场所：汇总 / 岗位列表 / 施工列表")]
    [SerializeField] private TMP_Text workplaceSummary;
    [SerializeField] private ScrollRect workplaceScroll;
    [SerializeField] private RectTransform workplaceContent;
    [SerializeField] private TMP_Text workplaceEmpty;
    [SerializeField] private RectTransform constructionContent;
    [SerializeField] private TMP_Text constructionEmpty;

    [Header("资源：汇总 / 岗位列表")]
    [SerializeField] private TMP_Text resourceSummary;
    [SerializeField] private ScrollRect resourceScroll;
    [SerializeField] private RectTransform resourceContent;
    [SerializeField] private TMP_Text resourceEmpty;

    [Header("行模板（Authored、默认 inactive、放在列表容器之外）")]
    [SerializeField] private ManagementRowView personRowTemplate;
    [SerializeField] private ManagementRowView workplaceRowTemplate;
    [SerializeField] private ManagementRowView constructionRowTemplate;
    [SerializeField] private ManagementRowView resourceRowTemplate;

    [Header("详情与操作区")]
    [SerializeField] private TMP_Text detailTitle;
    [SerializeField] private TMP_Text detailBody;
    /// <summary>详情面板上的独立入口：只作用于当前 Selection 中的人物，不改变选择、不写域。</summary>
    [SerializeField] private Button personDetailButton;
    [SerializeField] private TMP_Text actionHint;
    [SerializeField] private Button assignButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button addConstructionButton;
    [SerializeField] private Button removeConstructionButton;

    /// <summary>下包 Shell 注入：进入现有 PersonDetail（HomeSurface.Person）。由 Shell 决定具体导航。</summary>
    public Action<string> PersonDetailRequested { get; set; }

    public ManagementViewTab ActiveTab => _activeTab;
    public string SelectedPersonId => _host != null && _host.Selection.Kind == HomeSelectionKind.Person
        ? _host.Selection.Id
        : null;
    public string SelectedWorkplaceId => _workplaceCandidate;
    public string SelectedConstructionBuildingId => _constructionCandidate;
    public ManagementSnapshot CurrentSnapshot { get; private set; }
    public bool IsVisible => gameObject.activeSelf;
    public string StatusText => statusMessage != null ? statusMessage.text : string.Empty;

    private ProductHomeWorldHost _host;
    private HomeProductShell _shell;
    private HomeSelectionContext _selection;
    private PlayerCollection _source;
    private bool _initialized;
    private ManagementViewTab _activeTab = ManagementViewTab.Population;
    private ManagementPersonFilter _personFilter = ManagementPersonFilter.All;
    private string _workplaceCandidate;
    private string _constructionCandidate;
    private string _locatedSelectionKey;
    private bool _scrollToLocatedRow;
    private string _personRowsKey, _workplaceRowsKey, _constructionRowsKey, _resourceRowsKey;
    private readonly List<ManagementRowView> _personRows = new List<ManagementRowView>();
    private readonly List<ManagementRowView> _workplaceRows = new List<ManagementRowView>();
    private readonly List<ManagementRowView> _constructionRows = new List<ManagementRowView>();
    private readonly List<ManagementRowView> _resourceRows = new List<ManagementRowView>();

    /// <summary>Shell 接线：先 Initialize，再按 Router.Surface==Management 激活并调用 RefreshNow。</summary>
    public void Initialize(ProductHomeWorldHost host, HomeProductShell shell)
    {
        if (_initialized) return;
        _initialized = true;
        _host = host;
        _shell = shell;

        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (saveButton != null) saveButton.onClick.AddListener(() => Save());
        if (nameSearch != null) nameSearch.onValueChanged.AddListener(_ => RefreshNow());
        if (tabButtons != null)
            for (int i = 0; i < tabButtons.Length; i++)
            {
                int index = i;
                if (tabButtons[i] != null && index >= 0 && index <= (int)ManagementViewTab.Resources)
                    tabButtons[i].onClick.AddListener(() => SelectTab((ManagementViewTab)index));
            }
        if (statusFilterButtons != null)
            for (int i = 0; i < statusFilterButtons.Length; i++)
            {
                int index = i;
                if (statusFilterButtons[i] != null && index >= 0 && index <= (int)ManagementPersonFilter.NotInVillage)
                    statusFilterButtons[i].onClick.AddListener(() => SelectPersonFilter((ManagementPersonFilter)index));
            }
        if (assignButton != null) assignButton.onClick.AddListener(() => AssignSelectedPersonToWorkplace());
        if (removeButton != null) removeButton.onClick.AddListener(() => RemoveSelectedPersonAssignment());
        if (addConstructionButton != null) addConstructionButton.onClick.AddListener(() => AddSelectedPersonToConstruction());
        if (removeConstructionButton != null)
            removeConstructionButton.onClick.AddListener(() => RemoveSelectedPersonFromConstruction());
        if (personDetailButton != null) personDetailButton.onClick.AddListener(() => OpenSelectedPersonDetail());

        RebindSelection();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_initialized) RefreshNow();
    }

    private void OnDestroy()
    {
        UnbindSelection();
    }

    /// <summary>一次性只读刷新：同一快照提供人数、筛选、职责、施工与资源；命令后由 Shell 或操作本身再次调用。</summary>
    public void RefreshNow()
    {
        if (!_initialized || _host == null) return;

        var source = VillageRuntimeBridge.SimulationPersonSource;
        if (!ReferenceEquals(_source, source))
        {
            _source = source;
            _workplaceCandidate = null;
            _constructionCandidate = null;
            _personFilter = ManagementPersonFilter.All;
            _locatedSelectionKey = null;
            _scrollToLocatedRow = false;
            _personRowsKey = null;
            _workplaceRowsKey = null;
            _constructionRowsKey = null;
            _resourceRowsKey = null;
            if (nameSearch != null) nameSearch.SetTextWithoutNotify(string.Empty);
            SetStatus(string.Empty);
            RebindSelection();
        }

        CurrentSnapshot = ManagementReadAdapter.Read();
        LocateSelectionIfNeeded();
        PruneCandidates();
        Apply(CurrentSnapshot);
    }

    // ---------------- 页签 / 筛选 ----------------

    public bool SelectTab(ManagementViewTab tab)
    {
        _activeTab = tab;
        if (gameObject.activeInHierarchy) RefreshNow();
        return true;
    }

    private void SelectPersonFilter(ManagementPersonFilter filter)
    {
        _personFilter = filter;
        RefreshNow();
    }

    /// <summary>
    /// 人口行单击的唯一行为：通过既有 HomeSelectionContext 选中该人物，并留在集中管理页。
    /// 不切换 Surface、不请求人物详情、不写任何域状态；选中高亮与右栏由随后的 RefreshNow 反映。
    /// </summary>
    public bool SelectPersonRow(string personId)
    {
        if (_host == null || string.IsNullOrEmpty(personId)) return false;
        if (CurrentSnapshot != null && CurrentSnapshot.Person(personId) == null) return false;
        _scrollToLocatedRow = false;
        if (_host.Selection.Kind != HomeSelectionKind.Person || _host.Selection.Id != personId)
            _host.Selection.SelectPerson(personId);
        RefreshNow();
        return true;
    }

    /// <summary>
    /// 独立“查看人物详情”入口：只对当前 Selection 中的人物请求导航，不改变选择、不写域。
    /// 无人物时保持当前页面并给出明确原因。
    /// </summary>
    public bool OpenSelectedPersonDetail()
    {
        string personId = SelectedPersonId;
        if (string.IsNullOrEmpty(personId))
        {
            SetStatus("请先在人口列表选择一位人物，再查看人物详情。");
            return false;
        }
        if (CurrentSnapshot != null && CurrentSnapshot.Person(personId) == null)
        {
            SetStatus("所选人物已不在当前快照中，请刷新后重新选择。");
            return false;
        }
        PersonDetailRequested?.Invoke(personId);
        return true;
    }

    /// <summary>公共入口：先经既有 Selection 选中该人物，再由独立详情入口导航（不绕过 Selection）。</summary>
    public bool OpenPerson(string personId)
    {
        if (!SelectPersonRow(personId)) return false;
        return OpenSelectedPersonDetail();
    }

    /// <summary>与 ESC 使用同一 Shell 回调；本组件不自行拦截/消费 ESC。</summary>
    public void Close()
    {
        _shell?.CloseSurface();
    }

    // ---------------- 命令：全部走既有 Gateway，每次即时重查 ----------------

    public bool Save()
    {
        if (BlockedByPendingGrowth()) return false;
        var result = ManagementCommandGateway.SaveWorld();
        SetStatus(result.Succeeded ? "已保存当前世界。" : "未保存：" + result.Message);
        RefreshNow();
        return result.Succeeded;
    }

    public bool AssignSelectedPersonToWorkplace()
    {
        if (BlockedByPendingGrowth()) return false;
        string personId = SelectedPersonId;
        if (string.IsNullOrEmpty(personId)) { SetStatus("请先在人口列表选择一位人物。"); return false; }
        if (string.IsNullOrEmpty(_workplaceCandidate)) { SetStatus("请先选择一个职责岗位行作为派工目标。"); return false; }
        var row = CurrentSnapshot?.Workplace(_workplaceCandidate);
        if (row == null) { SetStatus("所选岗位已不在当前快照中，请刷新后重试。"); return false; }
        if (row.Kind == ManagementWorkplaceKind.LegacyCompatibility)
        { SetStatus("既有静态岗位（兼容记录）只支持撤职，不支持新增派工。"); return false; }
        if (!row.CanAssign)
        {
            SetStatus(row.IsFull ? "目标岗位已满员，本次未写入。" : "目标岗位未注册或不可派工，本次未写入。");
            return false;
        }

        var result = ManagementCommandGateway.AssignToWorkplace(personId, row.WorkplaceId);
        SetStatus(result.Message);
        RefreshNow();
        return result.Succeeded;
    }

    public bool RemoveSelectedPersonAssignment()
    {
        if (BlockedByPendingGrowth()) return false;
        string personId = SelectedPersonId;
        if (string.IsNullOrEmpty(personId)) { SetStatus("请先选择一位人物。"); return false; }
        var result = ManagementCommandGateway.RemoveAssignment(personId);
        SetStatus(result.Message);
        RefreshNow();
        return result.Succeeded;
    }

    public bool AddSelectedPersonToConstruction()
    {
        if (BlockedByPendingGrowth()) return false;
        string personId = SelectedPersonId;
        if (string.IsNullOrEmpty(personId)) { SetStatus("请先选择一位人物。"); return false; }
        if (string.IsNullOrEmpty(_constructionCandidate)) { SetStatus("请先选择一个施工建筑行。"); return false; }
        var result = ManagementCommandGateway.AddConstructionWorker(_constructionCandidate, personId);
        SetStatus(result.Message);
        RefreshNow();
        return result.Succeeded;
    }

    public bool RemoveSelectedPersonFromConstruction()
    {
        if (BlockedByPendingGrowth()) return false;
        string personId = SelectedPersonId;
        if (string.IsNullOrEmpty(personId)) { SetStatus("请先选择一位人物。"); return false; }
        if (string.IsNullOrEmpty(_constructionCandidate)) { SetStatus("请先选择一个施工建筑行。"); return false; }
        var result = ManagementCommandGateway.RemoveConstructionWorker(_constructionCandidate, personId);
        SetStatus(result.Message);
        RefreshNow();
        return result.Succeeded;
    }

    private bool BlockedByPendingGrowth()
    {
        if (_shell != null && _shell.GrowthUI != null && _shell.GrowthUI.HasPendingChoice)
        {
            SetStatus(PendingGrowthMessage);
            return true;
        }
        return false;
    }

    // ---------------- Selection 生命周期 ----------------

    private void RebindSelection()
    {
        UnbindSelection();
        if (_host == null) return;
        _selection = _host.Selection;
        if (_selection != null) _selection.Changed += SelectionChanged;
    }

    private void UnbindSelection()
    {
        if (_selection != null) _selection.Changed -= SelectionChanged;
        _selection = null;
    }

    private void SelectionChanged()
    {
        if (gameObject.activeInHierarchy) RefreshNow();
        else _locatedSelectionKey = null;
    }

    /// <summary>世界选中 Building / Place 时定位到对应职责行或施工行；不反向改写 Selection。</summary>
    private void LocateSelectionIfNeeded()
    {
        if (_host == null || CurrentSnapshot == null) return;
        var selection = _host.Selection;
        string key = selection.Kind + ":" + selection.Id;
        if (_locatedSelectionKey == key) return;
        _locatedSelectionKey = key;

        if (selection.Kind == HomeSelectionKind.Building)
        {
            var workplace = CurrentSnapshot.Workplaces.FirstOrDefault(w => w.BuildingInstanceId == selection.Id);
            if (workplace != null)
            {
                _workplaceCandidate = workplace.WorkplaceId;
                _constructionCandidate = null;
                _activeTab = ManagementViewTab.Workplaces;
                _scrollToLocatedRow = true;
                return;
            }
            var construction = CurrentSnapshot.Construction.FirstOrDefault(c => c.BuildingInstanceId == selection.Id);
            if (construction != null)
            {
                _constructionCandidate = construction.BuildingInstanceId;
                _workplaceCandidate = null;
                _activeTab = ManagementViewTab.Workplaces;
                _scrollToLocatedRow = true;
            }
            return;
        }

        if (selection.Kind != HomeSelectionKind.Place) return;
        var byWorkplace = CurrentSnapshot.Workplaces.FirstOrDefault(w => w.WorkplaceId == selection.Id);
        if (byWorkplace == null)
            byWorkplace = CurrentSnapshot.Workplaces.FirstOrDefault(w =>
                !string.IsNullOrEmpty(w.BuildingInstanceId)
                && VillageRuntimeBridge.ConstructionPlaceId(w.BuildingInstanceId) == selection.Id);
        if (byWorkplace != null)
        {
            _workplaceCandidate = byWorkplace.WorkplaceId;
            _constructionCandidate = null;
            _activeTab = ManagementViewTab.Workplaces;
            _scrollToLocatedRow = true;
            return;
        }
        var byPlace = CurrentSnapshot.Construction.FirstOrDefault(c =>
            VillageRuntimeBridge.ConstructionPlaceId(c.BuildingInstanceId) == selection.Id);
        if (byPlace != null)
        {
            _constructionCandidate = byPlace.BuildingInstanceId;
            _workplaceCandidate = null;
            _activeTab = ManagementViewTab.Workplaces;
            _scrollToLocatedRow = true;
        }
    }

    private void PruneCandidates()
    {
        if (CurrentSnapshot == null) return;
        if (!string.IsNullOrEmpty(_workplaceCandidate) && CurrentSnapshot.Workplace(_workplaceCandidate) == null)
            _workplaceCandidate = null;
        if (!string.IsNullOrEmpty(_constructionCandidate)
            && !CurrentSnapshot.Construction.Any(c => c.BuildingInstanceId == _constructionCandidate))
            _constructionCandidate = null;
    }

    // ---------------- 渲染 ----------------

    private void Apply(ManagementSnapshot snapshot)
    {
        if (snapshot == null) return;

        if (title != null) title.text = snapshot.Ready ? "集中管理 · 人口 / 职责与场所 / 资源" : "集中管理 · 世界未就绪";

        ApplyTabs();
        ApplyPopulation(snapshot);
        ApplyWorkplaces(snapshot);
        ApplyResources(snapshot);
        ApplyDetail(snapshot);
        ApplyActions(snapshot);

        if (!snapshot.Ready && string.IsNullOrEmpty(StatusText))
            SetStatus(snapshot.Diagnostics.Count > 0 ? string.Join("；", snapshot.Diagnostics) : "村庄世界未就绪。");
        _scrollToLocatedRow = false;
    }

    private void ApplyTabs()
    {
        if (populationPage != null) populationPage.SetActive(_activeTab == ManagementViewTab.Population);
        if (workplacePage != null) workplacePage.SetActive(_activeTab == ManagementViewTab.Workplaces);
        if (resourcePage != null) resourcePage.SetActive(_activeTab == ManagementViewTab.Resources);
        if (tabButtons == null) return;
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] == null) continue;
            var image = tabButtons[i].GetComponent<Image>();
            if (image != null)
                image.color = i == (int)_activeTab
                    ? new Color(.92f, .78f, .51f, .96f)
                    : new Color(.88f, .80f, .64f, .94f);
        }
        if (statusFilterButtons == null) return;
        for (int i = 0; i < statusFilterButtons.Length; i++)
        {
            if (statusFilterButtons[i] == null) continue;
            var image = statusFilterButtons[i].GetComponent<Image>();
            if (image != null)
                image.color = i == (int)_personFilter
                    ? new Color(.92f, .78f, .51f, .96f)
                    : new Color(.88f, .80f, .64f, .94f);
        }
    }

    private void ApplyPopulation(ManagementSnapshot snapshot)
    {
        var filtered = snapshot.Filter(nameSearch != null ? nameSearch.text : string.Empty, _personFilter);
        int total = snapshot.Persons.Count;
        int inVillage = snapshot.Persons.Count(p => p.IsInVillage);
        if (populationSummary != null)
            populationSummary.text = "共 " + total + " 人 · 当前显示 " + filtered.Persons.Count + " 人 · 在村 " + inVillage + " 人";

        int index = 0;
        string personKey = (nameSearch != null ? nameSearch.text : string.Empty) + "|" + (int)_personFilter
                           + "|" + SelectedPersonId + "|"
                           + string.Join(",", filtered.Persons.Select(p => p.PersonId + p.DisplayName + p.IsHero + p.Profession
                               + p.Availability + p.AvailabilityLabel + p.PrimaryWorkplaceId + p.PrimaryResponsibility
                               + p.AssignmentIsLegacy + p.CurrentExecution + p.TemporaryCommitment + p.Hungry + p.ConsecutiveUnderfedDays));
        bool rebuildPerson = personKey != _personRowsKey;
        if (rebuildPerson) { _personRowsKey = personKey; ClearRows(_personRows); }
        foreach (var person in filtered.Persons)
        {
            string personId = person.PersonId;
            var row = rebuildPerson ? Spawn(personRowTemplate, populationContent, _personRows) : null;
            if (rebuildPerson && row == null) break;
            if (row != null)
                // 行高 132 = 标题 32（-8）+ 间隔 4 + detail 3 行 76（-44）+ 底部 12；
                // 职责 / 现时执行 / 临时承诺 / 饥饿按真实字体行高各占一行，信息一项未删。
                row.Bind(person.DisplayName + " · " + (person.IsHero ? "英雄" : ValueOr(person.Profession, "职业资料暂缺")),
                    person.AvailabilityLabel + " · 职责：" + ValueOr(person.PrimaryResponsibility, "无主要职责")
                    + (person.AssignmentIsLegacy ? "（既有兼容记录）" : string.Empty)
                    + "\n执行：" + ValueOr(person.CurrentExecution, "暂无执行事实")
                    + "\n临时承诺：" + ValueOr(person.TemporaryCommitment, "无")
                    + " · 饥饿：" + NutritionLabel(person),
                    () => SelectPersonRow(personId), true, personId == SelectedPersonId, 132f);
            if (rebuildPerson && _scrollToLocatedRow && personId == SelectedPersonId)
                ScrollTo(populationScroll, populationContent, index, filtered.Persons.Count);
            index++;
        }
        if (populationEmpty != null)
        {
            populationEmpty.gameObject.SetActive(filtered.Persons.Count == 0);
            populationEmpty.text = total == 0
                ? "人物资料暂不可用：世界未就绪或没有人物。"
                : "没有符合姓名 / 状态筛选的人物。";
        }
    }

    private void ApplyWorkplaces(ManagementSnapshot snapshot)
    {
        int buildingFunctions = snapshot.Workplaces.Count(w => w.Kind == ManagementWorkplaceKind.BuildingFunction);
        int legacy = snapshot.Workplaces.Count(w => w.Kind == ManagementWorkplaceKind.LegacyCompatibility);
        if (workplaceSummary != null)
            workplaceSummary.text = "岗位 " + buildingFunctions + " 个 · 既有兼容记录 " + legacy + " 条 · 施工 " + snapshot.Construction.Count + " 项";

        int index = 0;
        string workplaceKey = _workplaceCandidate + "|" + string.Join(",", snapshot.Workplaces.Select(w =>
            w.WorkplaceId + w.Kind + w.DisplayName + w.FunctionProfileId + w.BoundCount + w.LaborCount + w.OnsiteCount + w.Capacity + w.IsRegistered));
        bool rebuildWorkplace = workplaceKey != _workplaceRowsKey;
        if (rebuildWorkplace) { _workplaceRowsKey = workplaceKey; ClearRows(_workplaceRows); }
        foreach (var workplace in snapshot.Workplaces)
        {
            string workplaceId = workplace.WorkplaceId;
            var row = rebuildWorkplace ? Spawn(workplaceRowTemplate, workplaceContent, _workplaceRows) : null;
            if (rebuildWorkplace && row == null) break;
            if (row != null)
                // 行高 132 = 标题 32（-8）+ 间隔 4 + detail 3 行 76（-44）+ 底部 12；
                // 只加高本任务需要的岗位行，其余三类模板保持原高度，不动视口。
                row.Bind(WorkplaceTitle(workplace), WorkplaceDetail(workplace, snapshot), () => SelectWorkplaceRow(workplaceId),
                    true, workplaceId == _workplaceCandidate, 132f);
            if (rebuildWorkplace && _scrollToLocatedRow && workplaceId == _workplaceCandidate)
                ScrollTo(workplaceScroll, workplaceContent, index, snapshot.Workplaces.Count);
            index++;
        }
        if (workplaceEmpty != null)
        {
            workplaceEmpty.gameObject.SetActive(snapshot.Workplaces.Count == 0);
            workplaceEmpty.text = snapshot.Ready
                ? "目前没有已完成建筑的功能岗位或既有职责记录。"
                : "世界未就绪：岗位资料暂不可用。";
        }

        var constructionRows = snapshot.Construction;
        var constructionParent = constructionContent != null ? constructionContent : workplaceContent;
        index = 0;
        string constructionKey = _constructionCandidate + "|" + string.Join(",", constructionRows.Select(c =>
            c.BuildingInstanceId + c.DisplayName + c.Progress + c.WorkerCount + c.OnsiteCount + c.ExecutionPaused));
        bool rebuildConstruction = constructionKey != _constructionRowsKey;
        if (rebuildConstruction) { _constructionRowsKey = constructionKey; ClearRows(_constructionRows); }
        foreach (var construction in constructionRows)
        {
            string buildingId = construction.BuildingInstanceId;
            var row = rebuildConstruction ? Spawn(constructionRowTemplate, constructionParent, _constructionRows) : null;
            if (rebuildConstruction && row == null) break;
            if (row != null)
                // 与其余业务行同高：标题 32（-8）+ detail 3 行 76（-44），工人名单独占一行，不被截断。
                row.Bind("施工 · " + construction.DisplayName, ConstructionDetail(construction, snapshot),
                    () => SelectConstructionRow(buildingId), true, buildingId == _constructionCandidate, 132f);
            if (rebuildConstruction && _scrollToLocatedRow && buildingId == _constructionCandidate)
                ScrollTo(workplaceScroll, constructionParent, index, constructionRows.Count);
            index++;
        }
        if (constructionEmpty != null)
        {
            constructionEmpty.gameObject.SetActive(constructionRows.Count == 0);
            constructionEmpty.text = snapshot.Ready ? "当前没有施工中的建筑。" : "世界未就绪：施工资料暂不可用。";
        }
    }

    private void ApplyResources(ManagementSnapshot snapshot)
    {
        var resources = snapshot.Resources;
        if (resourceSummary != null)
            resourceSummary.text = resources != null && resources.HasWorld
                ? "食材 " + resources.Ingredients + " · 食物 " + resources.Food
                  + " · 在村供餐 " + resources.InVillageCount + " 人 · 其中饥饿 " + resources.HungryCount + " 人"
                : "资源权威未就绪：Ingredients / Food 暂不可读。";

        int index = 0;
        string resourceKey = _workplaceCandidate + "|" + (resources == null ? "null" : resources.HasWorld + "|" + resources.Ingredients
            + "|" + resources.Food + "|" + resources.InVillageCount + "|" + resources.HungryCount + "|"
            + string.Join(",", resources.Workplaces.Select(w => w.WorkplaceId + w.RoleLabel + w.DisplayName
                + w.BuildingInstanceId + w.FunctionSlotId + w.BoundCount + w.LaborCount + w.Capacity)));
        bool rebuildResource = resourceKey != _resourceRowsKey;
        if (rebuildResource) { _resourceRowsKey = resourceKey; ClearRows(_resourceRows); }
        if (resources != null)
            foreach (var workplace in resources.Workplaces)
            {
                string workplaceId = workplace.WorkplaceId;
                var row = rebuildResource ? Spawn(resourceRowTemplate, resourceContent, _resourceRows) : null;
                if (rebuildResource && row == null) break;
                if (row != null)
                {
                    // 资源行是纯展示信息：不再把整行绑定为导航，也不保留旧“点击定位”提示。
                    // 导航只通过右侧显式「查看岗位」按钮，并携带完整 WorkplaceId。
                    bool targetAvailable = snapshot.Workplaces != null
                        && snapshot.Workplaces.Any(w => w.WorkplaceId == workplaceId);
                    row.Bind(workplace.RoleLabel + " · " + workplace.DisplayName,
                        "名额 绑定 " + workplace.BoundCount + "/" + workplace.Capacity
                        + " · 实际劳动 " + workplace.LaborCount
                        + "\n建筑 " + ValueOr(workplace.BuildingInstanceId, "—")
                        + " · 槽位 " + ValueOr(workplace.FunctionSlotId, "—")
                        + "\n" + (targetAvailable ? "资源只读；用「查看岗位」定位到职责与场所的同一行。"
                                                  : "资源只读；该岗位当前不在职责页，未提供跳转。"),
                        null, true, workplaceId == _workplaceCandidate, 132f);
                    BindResourceRowAction(row, workplaceId, targetAvailable);
                }
                if (rebuildResource && _scrollToLocatedRow && workplaceId == _workplaceCandidate)
                    ScrollTo(resourceScroll, resourceContent, index, resources.Workplaces.Count);
                index++;
            }
        if (resourceEmpty != null)
        {
            resourceEmpty.gameObject.SetActive(resources == null || resources.Workplaces.Count == 0);
            resourceEmpty.text = resources == null || !resources.HasWorld
                ? "资源权威未就绪：暂无可读的 Ingredients / Food。"
                : "当前没有可导航的 Farm / Kitchen 实例。";
        }
    }

    private void ApplyDetail(ManagementSnapshot snapshot)
    {
        var person = !string.IsNullOrEmpty(SelectedPersonId) ? snapshot.Person(SelectedPersonId) : null;
        var workplace = !string.IsNullOrEmpty(_workplaceCandidate) ? snapshot.Workplace(_workplaceCandidate) : null;
        var construction = !string.IsNullOrEmpty(_constructionCandidate)
            ? snapshot.Construction.FirstOrDefault(c => c.BuildingInstanceId == _constructionCandidate)
            : null;

        if (detailTitle != null)
            detailTitle.text = person != null ? "所选人物 · " + person.DisplayName : "未选择人物";

        if (detailBody != null)
        {
            string personText = person == null
                ? "在人口列表单击只在本页选中人物（不跳转）；选中后用详情面板上的“查看人物详情”进入人物详情，"
                  + "或切到“职责与场所”选择岗位 / 施工目标。"
                : person.DisplayName + "（" + person.PersonId + "） · " + person.AvailabilityLabel
                  + "\n职业：" + ValueOr(person.Profession, "职业资料暂缺")
                  + "\n主要职责：" + ValueOr(person.PrimaryResponsibility, "无主要职责")
                  + (person.AssignmentIsLegacy ? "（既有兼容记录，可显式撤职）" : string.Empty)
                  + "\n现时执行：" + ValueOr(person.CurrentExecution, "暂无执行事实")
                  + "\n临时承诺：" + ValueOr(person.TemporaryCommitment, "无")
                  + "\n饥饿对照：" + NutritionLabel(person);
            string workplaceText = workplace == null
                ? "未选择岗位行"
                : WorkplaceTitle(workplace) + "\n" + WorkplaceDetail(workplace, snapshot);
            string constructionText = construction == null
                ? "未选择施工行"
                : construction.DisplayName + " · 进度 " + construction.ProgressLabel
                  + " · 工人 " + construction.WorkerCount + " 人";
            detailBody.text = personText + "\n\n派工目标：" + workplaceText + "\n\n施工目标：" + constructionText;
        }

        if (actionHint != null)
        {
            if (!VillageRuntimeBridge.CanSaveWorld)
            {
                actionHint.text = WorldBusyMessage;
            }
            else if (person == null)
            {
                actionHint.text = "尚未选择人物：点人口行只在本页选中人物（不会跳转）；选中后可用“查看人物详情”，"
                                  + "或切到“职责与场所”选择岗位 / 施工目标。";
            }
            else if (!person.IsInVillage)
            {
                actionHint.text = "该人物当前为“" + person.AvailabilityLabel + "”，只可查看；命令按钮已禁用，Gateway 仍会最终校验。";
            }
            else
            {
                // 只说明“为什么这个按钮现在不可用”，不改变 ApplyActions 的合法条件。
                var unavailable = new List<string>();
                if (workplace == null)
                    unavailable.Add("派工：先在“职责与场所”选择岗位行");
                else if (workplace.Kind == ManagementWorkplaceKind.LegacyCompatibility)
                    unavailable.Add("派工：所选是既有静态岗位（兼容记录），只可撤职");
                else if (!workplace.IsRegistered)
                    unavailable.Add("派工：所选岗位未注册，不可派工");
                else if (workplace.IsFull)
                    unavailable.Add("派工：所选岗位已满员");
                if (string.IsNullOrEmpty(person.PrimaryWorkplaceId))
                    unavailable.Add("撤职：该人物当前没有主要职责");
                bool isWorker = construction != null && construction.WorkerPersonIds.Contains(person.PersonId);
                if (construction == null)
                {
                    unavailable.Add("加入施工 / 撤出施工：先在“职责与场所”选择施工行");
                }
                else if (isWorker)
                {
                    unavailable.Add("加入施工：该人物已在本施工项目中，可改用“撤出施工”");
                }
                else
                {
                    unavailable.Add("撤出施工：该人物不在本施工项目中，可改用“加入施工”");
                }
                actionHint.text = unavailable.Count == 0
                    ? "人物与目标已就绪：四类命令均可执行；施工加入 / 撤出会保留 Primary Assignment，命令不自动保存。"
                    : "当前不可用的命令：" + string.Join("；", unavailable) + "。";
            }
        }
    }

    private void ApplyActions(ManagementSnapshot snapshot)
    {
        bool canWrite = VillageRuntimeBridge.CanSaveWorld;
        var person = !string.IsNullOrEmpty(SelectedPersonId) ? snapshot.Person(SelectedPersonId) : null;
        var workplace = !string.IsNullOrEmpty(_workplaceCandidate) ? snapshot.Workplace(_workplaceCandidate) : null;
        var construction = !string.IsNullOrEmpty(_constructionCandidate)
            ? snapshot.Construction.FirstOrDefault(c => c.BuildingInstanceId == _constructionCandidate)
            : null;

        bool personReady = person != null && person.IsInVillage;
        if (saveButton != null) saveButton.interactable = canWrite;
        if (personDetailButton != null) personDetailButton.interactable = person != null;
        if (assignButton != null)
            assignButton.interactable = canWrite && personReady && workplace != null && workplace.CanAssign;
        if (removeButton != null)
            removeButton.interactable = canWrite && personReady && person != null
                                        && !string.IsNullOrEmpty(person.PrimaryWorkplaceId);
        bool isWorker = construction != null && person != null && construction.WorkerPersonIds.Contains(person.PersonId);
        if (addConstructionButton != null)
            addConstructionButton.interactable = canWrite && personReady && construction != null && !isWorker;
        if (removeConstructionButton != null)
            removeConstructionButton.interactable = canWrite && personReady && construction != null && isWorker;
    }

    // ---------------- 行候选（仅本次页面会话，不是第二份 Selection） ----------------

    private void SelectWorkplaceRow(string workplaceId)
    {
        _workplaceCandidate = workplaceId;
        _constructionCandidate = null;
        RefreshNow();
    }

    private void SelectConstructionRow(string buildingId)
    {
        _constructionCandidate = buildingId;
        _workplaceCandidate = null;
        RefreshNow();
    }

    private void NavigateResourceRow(string workplaceId)
    {
        _workplaceCandidate = workplaceId;
        _constructionCandidate = null;
        _activeTab = ManagementViewTab.Workplaces;
        _scrollToLocatedRow = true;
        _workplaceRowsKey = null;
        if (gameObject.activeInHierarchy) RefreshNow();
    }

    /// <summary>
    /// 资源行的显式导航入口：只携带完整 WorkplaceId（实例 ID + slot）定位到职责页同一行；
    /// 不派工、不撤职、不写库存 / 时间，也不清空当前 Person 选择。
    /// 目标不在职责页时禁用并显示原因，不猜测另一个同名岗位。
    /// </summary>
    private void BindResourceRowAction(ManagementRowView row, string workplaceId, bool available)
    {
        var existing = row.transform.Find("ViewWorkplace") as RectTransform;
        Button action = existing != null ? existing.GetComponent<Button>() : null;
        if (action == null)
            action = HomeProductUI.Button(row.transform, "ViewWorkplace", new Vector2(1, 0),
                new Vector2(-12, 10), new Vector2(128, 34), "查看岗位", null);
        var caption = action.GetComponentInChildren<TMP_Text>();
        if (caption != null) caption.text = available ? "查看岗位" : "岗位不可用";
        action.onClick.RemoveAllListeners();
        if (available)
        {
            string target = workplaceId;
            action.onClick.AddListener(() => NavigateResourceRow(target));
        }
        action.interactable = available;
    }

    // ---------------- 行生成 / 回收 ----------------

    private ManagementRowView Spawn(ManagementRowView template, RectTransform parent, List<ManagementRowView> bucket)
    {
        if (template == null || parent == null) return null;
        var row = Instantiate(template, parent);
        row.gameObject.SetActive(true);
        bucket.Add(row);
        return row;
    }

    private static void ClearRows(List<ManagementRowView> rows)
    {
        foreach (var row in rows)
            if (row != null) { row.gameObject.SetActive(false); Destroy(row.gameObject); }
        rows.Clear();
    }

    private void ScrollTo(ScrollRect scroll, RectTransform content, int index, int count)
    {
        if (scroll == null || content == null || count <= 1) return;
        Canvas.ForceUpdateCanvases();
        scroll.verticalNormalizedPosition = 1f - Mathf.Clamp01((float)index / (count - 1));
    }

    // ---------------- 文案 ----------------

    private string WorkplaceTitle(ManagementWorkplaceRow workplace)
    {
        if (workplace.Kind == ManagementWorkplaceKind.LegacyCompatibility)
            return workplace.DisplayName + " · 既有岗位（兼容记录）";
        return workplace.DisplayName + " · 实例 " + ValueOr(workplace.BuildingInstanceId, "—")
               + " · 槽位 " + ValueOr(workplace.FunctionSlotId, "—");
    }

    /// <summary>
    /// 职责与场所行的次要说明。detail 矩形按真实 3 行高度布局（见 Prefab WorkplaceRowTemplate/Detail），
    /// Ellipsis 只在行尾截断，因此第 1 行必须是规格要求的可比较数值（容量 / 绑定 / 实际劳动 / 现场）；
    /// 完整身份（WorkplaceId / BuildingInstanceId / FunctionSlotId）保留在第 2、3 行，禁止为求简洁删除身份。
    /// </summary>
    private string WorkplaceDetail(ManagementWorkplaceRow workplace, ManagementSnapshot snapshot)
    {
        var names = PersonNames(workplace.BoundPersonIds, snapshot);
        string counters = "绑定 " + workplace.BoundCount + "/" + workplace.Capacity
                          + " · 实际劳动 " + workplace.LaborCount
                          + " · 现场 " + workplace.OnsiteCount
                          + (workplace.IsRegistered ? string.Empty : " · 未注册（不可派工）")
                          + (workplace.IsFull ? " · 已满员" : string.Empty);
        if (workplace.Kind == ManagementWorkplaceKind.LegacyCompatibility)
            return counters
                   + "\n岗位 " + workplace.WorkplaceId + "（既有静态岗位：不可作为正式新增派工目标，只可显式撤职）"
                   + "\n绑定人：" + ValueOr(names, "无");
        return counters
               + "\n岗位 " + workplace.WorkplaceId
               + "\n建筑 " + ValueOr(workplace.BuildingInstanceId, "—")
               + " · 槽位 " + ValueOr(workplace.FunctionSlotId, "—")
               + " · 功能 " + ValueOr(workplace.FunctionProfileId, "—")
               + " · 绑定人：" + ValueOr(names, "无");
    }

    /// <summary>
    /// 施工行说明：进度 / 工人名单与人数 / 现场人数分 3 行；detail 矩形按 3 行高度布局，
    /// 工人姓名（含 PersonId 身份）独占一行，不被行尾 Ellipsis 截掉。
    /// </summary>
    private string ConstructionDetail(ManagementConstructionRow construction, ManagementSnapshot snapshot)
    {
        return "建筑 " + ValueOr(construction.BuildingInstanceId, "—")
               + " · 进度 " + construction.ProgressLabel
               + " · " + (construction.ExecutionPaused ? "执行暂停（只读事实）" : "施工中")
               + "\n工人 " + construction.WorkerCount + " 人：" + ValueOr(construction.WorkerPersonIds.Count > 0
                   ? PersonNames(construction.WorkerPersonIds, snapshot)
                   : string.Empty, "无")
               + "\n现场 " + construction.OnsiteCount + " 人";
    }

    private static string PersonNames(IEnumerable<string> ids, ManagementSnapshot snapshot)
    {
        if (ids == null) return string.Empty;
        return string.Join("、", ids.Select(id =>
        {
            var person = snapshot?.Person(id);
            return person == null ? id : person.DisplayName + "（" + id + "）";
        }));
    }

    private static string NutritionLabel(ManagementPersonRow person)
    {
        if (person == null || !person.HasNutritionFacts) return "暂无可读饥饿事实";
        return person.Hungry
            ? "Hungry（连续不足 " + person.ConsecutiveUnderfedDays + " 天）"
            : "已满足";
    }

    private static string ValueOr(string value, string fallback)
        => string.IsNullOrEmpty(value) ? fallback : value;

    private void SetStatus(string message)
    {
        if (statusMessage != null) statusMessage.text = message ?? string.Empty;
    }
}
