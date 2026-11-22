using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RRPG.StaticConfig;
using UnityEngine;

namespace RRPG.Management.Tests
{
/// <summary>
/// V17-R3-B 集中管理：只读投影与产品命令防御的长期契约测试（最小代表集合）。
/// 覆盖：同源 PersonId / 同类多建筑多 slot 独立 / 未完工不派工 / 满员与 AlreadyAssigned 领域诊断 /
/// 消失、远征或死亡人物零写入 / 世界忙命令零写入 / 只读无副作用 / 施工保留 Primary Assignment /
/// 完工建筑拒绝施工加入 / 保存 bool 反馈与世界锁 / 静态兼容显示与撤职 /
/// 劳动中撤职后不再从旧岗位累计证据 / 撤职删除记录与累计统计但已结算贡献不回滚。
/// </summary>
public sealed class ManagementProjectionTests
{
    private const BindingFlags S = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags I = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static readonly Type Bridge = Type.GetType("VillageRuntimeBridge, Assembly-CSharp", true);
    private static readonly Type Adapter = Type.GetType("ManagementReadAdapter, Assembly-CSharp", true);
    private static readonly Type Gateway = Type.GetType("ManagementCommandGateway, Assembly-CSharp", true);
    private static readonly Type ActivityHost = Type.GetType("ActivityRuntimeBridge, Assembly-CSharp", true);
    private static readonly Type ActivityDay = Type.GetType("RRPG.AutonomousActivity.ActivityDayStamp, AutonomousActivity.Runtime", true);
    private static readonly Type ActivityWindow = Type.GetType("RRPG.AutonomousActivity.ActivityDailyWindow, AutonomousActivity.Runtime", true);
    private static readonly Type ActivityCandidate = Type.GetType("RRPG.AutonomousActivity.ActivityCandidate, AutonomousActivity.Runtime", true);
    private static readonly Type ActivityParticipation = Type.GetType("RRPG.AutonomousActivity.ActivityTargetParticipation, AutonomousActivity.Runtime", true);
    private static readonly Type ActivitySchedule = Type.GetType("IntradayActivitySchedule, Assembly-CSharp", true);
    private static readonly Type StaticConfigApi = Type.GetType("RRPG.StaticConfig.StaticConfigRegistry, StaticConfig.Runtime", true);
    private object _player;
    private object _scheduleBeforeTest;

    [SetUp]
    public void Setup()
    {
        Call("ResetForTests");
        Call("EnsureInitialized", new object[] { null });
        _player = BuildSameNamePlayer("person_alpha", "person_beta", "person_gamma");
        Call("SetSimulationPersonSource", _player);
        _scheduleBeforeTest = Static("ActivitySchedule");
        Clock.Pause();
    }

    [TearDown]
    public void Cleanup()
    {
        ForceSaveFailure(false);
        if (_scheduleBeforeTest != null) Call("ConfigureActivitySchedule", _scheduleBeforeTest);
        Call("ResetForTests");
    }

    // ---------------- 1. 同一名册 / 同姓名不同 PersonId ----------------
    [Test]
    public void SameNameDifferentPersonIdsStayIndependentAndFilterIsSessionOnly()
    {
        var snapshot = Read();
        Assert.IsTrue(snapshot.Ready);
        Assert.AreEqual(3, snapshot.Persons.Count);
        Assert.AreEqual(3, snapshot.Persons.Select(p => p.PersonId).Distinct().Count());
        Assert.IsTrue(snapshot.Persons.All(p => p.DisplayName == "林恩"));
        CollectionAssert.AreEquivalent(new[] { "person_alpha", "person_beta", "person_gamma" },
            snapshot.Persons.Select(p => p.PersonId));

        Assert.AreEqual(3, snapshot.Filter("林", ManagementPersonFilter.All).Persons.Count);
        Assert.AreEqual(0, snapshot.Filter("不存在", ManagementPersonFilter.All).Persons.Count);
        Assert.AreEqual(3, snapshot.Filter("", ManagementPersonFilter.InVillage).Persons.Count);
        Assert.AreEqual(0, snapshot.Filter("", ManagementPersonFilter.NotInVillage).Persons.Count);
        Assert.AreEqual(3, Read().Persons.Count, "筛选只产生新快照，不改动来源快照");

        var building = Complete("building_farm", 0);
        string workplace = Workplace(building, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        var after = Read();
        Assert.AreEqual(workplace, after.Person("person_alpha").PrimaryWorkplaceId);
        Assert.IsFalse(after.Person("person_beta").PrimaryWorkplaceId.Length > 0);
        Assert.IsFalse(after.Person("person_gamma").PrimaryWorkplaceId.Length > 0);
    }

    // ---------------- 2. 同类多建筑 / 多 slot 不合并 ----------------
    [Test]
    public void SameDefinitionBuildingsAndRegisteredSlotsStayIndependent()
    {
        var first = Complete("building_farm", 0);
        var second = Complete("building_farm", 1);
        string a = Workplace(first, "main"), b = Workplace(second, "main");
        Assert.AreNotEqual(a, b);

        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", a).Succeeded);
        Assert.IsTrue(Command("AssignToWorkplace", "person_beta", b).Succeeded);

        var snapshot = Read();
        var rowA = snapshot.Workplace(a);
        var rowB = snapshot.Workplace(b);
        Assert.IsNotNull(rowA); Assert.IsNotNull(rowB);
        Assert.AreEqual(Id(first), rowA.BuildingInstanceId);
        Assert.AreEqual(Id(second), rowB.BuildingInstanceId);
        Assert.AreEqual("main", rowA.FunctionSlotId);
        Assert.AreEqual("ws_farm_01", rowA.FunctionProfileId);
        Assert.IsTrue(rowA.IsRegistered);
        Assert.AreEqual(2, rowA.Capacity);
        CollectionAssert.AreEquivalent(new[] { "person_alpha" }, rowA.BoundPersonIds);
        CollectionAssert.AreEquivalent(new[] { "person_beta" }, rowB.BoundPersonIds);

        // 同一建筑定义的第二 slot：投影按真实 FunctionBinding 枚举，不用 FirstOrDefault 合并。
        var definitions = RegistryWithSecondSlot();
        Assert.IsTrue(Assignments.TryRegisterWorkplace(new VillageWorkplaceDef
        {
            WorkplaceId = Workplace(first, "aux"), DisplayName = "Farm · aux", Capacity = 1, TestContributionPerDay = 1
        }));
        Assert.AreEqual(VillageAssignmentResult.Success, Call("TryAssignPerson", "person_gamma", Workplace(first, "aux")));

        var withSlots = Read(definitions);
        var aux = withSlots.Workplace(Workplace(first, "aux"));
        Assert.IsNotNull(aux, "注入同一 BuildingRegistry 后第二个 slot 必须出现在投影中");
        Assert.AreEqual("aux", aux.FunctionSlotId);
        Assert.IsTrue(aux.IsRegistered);
        Assert.AreEqual(1, aux.Capacity);
        CollectionAssert.AreEquivalent(new[] { "person_gamma" }, aux.BoundPersonIds);
        Assert.AreEqual(1, withSlots.Workplace(a).BoundCount, "slot 之间名额独立");
        Assert.AreEqual(1, withSlots.Workplace(b).BoundCount);
    }

    // ---------------- 3. 未完工建筑不提供可派工岗位 ----------------
    [Test]
    public void UnderConstructionBuildingHasNoAssignableWorkplaceAndCommandRefuses()
    {
        var building = Place("building_farm", 0);
        string workplace = Workplace(building, "main");
        var snapshot = Read();

        Assert.IsNull(snapshot.Workplace(workplace), "未完工建筑的 Function 岗位不得进入可派工列表");
        var construction = snapshot.Construction.Single(c => c.BuildingInstanceId == Id(building));
        Assert.AreEqual(0, construction.Progress);
        Assert.AreEqual(10, construction.Target);
        Assert.AreEqual(0, construction.WorkerCount);
        Assert.IsTrue(construction.ExecutionPaused);

        var result = Command("AssignToWorkplace", "person_alpha", workplace);
        Assert.AreEqual(ManagementCommandOutcome.TargetNotCompleted, result.Outcome);
        Assert.AreEqual(0, Assignments.Records.Count);
        Assert.IsTrue(result.Message.Contains("施工"));
    }

    // ---------------- 4. 满员 / AlreadyAssigned 保留领域诊断并零写入 ----------------
    [Test]
    public void CapacityFullAndAlreadyAssignedPreserveDomainDiagnosticsWithoutWrite()
    {
        var first = Complete("building_farm", 0);
        var second = Complete("building_farm", 1);
        string a = Workplace(first, "main"), b = Workplace(second, "main");

        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", a).Succeeded);
        Assert.IsTrue(Command("AssignToWorkplace", "person_beta", a).Succeeded);
        Assert.AreEqual(2, Assignments.GetOccupantCount(a));

        var full = Command("AssignToWorkplace", "person_gamma", a);
        Assert.AreEqual(ManagementCommandOutcome.DomainRejected, full.Outcome);
        Assert.AreEqual("CapacityFull", full.DomainDiagnostic);
        Assert.IsNull(Assignments.GetWorkplaceId("person_gamma"));
        Assert.AreEqual(2, Assignments.Records.Count);

        var already = Command("AssignToWorkplace", "person_alpha", b);
        Assert.AreEqual(ManagementCommandOutcome.DomainRejected, already.Outcome);
        Assert.AreEqual("AlreadyAssigned", already.DomainDiagnostic);
        Assert.AreEqual(a, Assignments.GetWorkplaceId("person_alpha"), "不自动 Remove→Assign / 不做原子换岗");
        Assert.AreEqual(2, Assignments.Records.Count);
        Assert.AreEqual(0, Assignments.GetOccupantCount(b));
    }

    // ---------------- 5. 消失 / 远征人物零写入 ----------------
    [Test]
    public void MissingOrAwayPersonIsRejectedWithoutWrite()
    {
        var building = Complete("building_farm", 0);
        string workplace = Workplace(building, "main");

        var missing = Command("AssignToWorkplace", "person_missing", workplace);
        Assert.AreEqual(ManagementCommandOutcome.PersonMissing, missing.Outcome);
        Assert.AreEqual(0, Assignments.Records.Count);

        Call("SetExpeditionPersonIds", new object[] { new[] { "person_beta" } });
        var away = Command("AssignToWorkplace", "person_beta", workplace);
        Assert.AreEqual(ManagementCommandOutcome.PersonNotInVillage, away.Outcome);
        Assert.IsNull(Assignments.GetWorkplaceId("person_beta"));
        Assert.AreEqual(0, Assignments.Records.Count);

        var snapshot = Read();
        Assert.AreEqual(VillageAvailability.Expedition, snapshot.Person("person_beta").Availability);
        Assert.AreEqual("远征中（保留职责）", snapshot.Person("person_beta").AvailabilityLabel);
    }

    // ---------------- 6. 只读投影无副作用 ----------------
    [Test]
    public void ReadProjectionDoesNotTouchWorldClockOrAuthority()
    {
        var building = Complete("building_farm", 0);
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", Workplace(building, "main")).Succeeded);

        string saveBefore = JsonUtility.ToJson(Call("BuildSaveData"));
        int placesBefore = World().Places.Count;
        int recordsBefore = Assignments.Records.Count;
        long minuteBefore = Clock.CurrentInstant.Minute;
        int projectsBefore = Projects.ActiveProjects.Count;

        for (int i = 0; i < 20; i++) Read();

        Assert.AreEqual(saveBefore, JsonUtility.ToJson(Call("BuildSaveData")));
        Assert.AreEqual(placesBefore, World().Places.Count);
        Assert.AreEqual(recordsBefore, Assignments.Records.Count);
        Assert.AreEqual(projectsBefore, Projects.ActiveProjects.Count);
        Assert.AreEqual(minuteBefore, Clock.CurrentInstant.Minute);
    }

    // ---------------- 7. 施工保留 Primary Assignment ----------------
    [Test]
    public void ConstructionWorkerKeepsPrimaryAssignment()
    {
        var building = Place("building_farm", 0);
        Assert.AreEqual(VillageAssignmentResult.Success, Call("TryAssignPerson", "person_alpha", "ws_farm_01"));

        var added = Command("AddConstructionWorker", Id(building), "person_alpha");
        Assert.IsTrue(added.Succeeded, added.ToString());
        Assert.AreEqual("ws_farm_01", Assignments.GetWorkplaceId("person_alpha"), "施工加入不得破坏 Primary Assignment");
        var project = Projects.GetActiveProject((string)Get(building, "ConstructionProjectId"));
        CollectionAssert.Contains(project.ParticipantPersonIds, "person_alpha");

        var snapshot = Read();
        var row = snapshot.Construction.Single(c => c.BuildingInstanceId == Id(building));
        Assert.AreEqual(1, row.WorkerCount);
        CollectionAssert.Contains(row.WorkerPersonIds, "person_alpha");

        var removed = Command("RemoveConstructionWorker", Id(building), "person_alpha");
        Assert.IsTrue(removed.Succeeded, removed.ToString());
        Assert.AreEqual("ws_farm_01", Assignments.GetWorkplaceId("person_alpha"), "撤出施工同样不得破坏 Primary Assignment");
        Assert.IsEmpty(project.ParticipantPersonIds);
        Assert.AreEqual(0, Read().Construction.Single(c => c.BuildingInstanceId == Id(building)).WorkerCount);
    }

    // ---------------- 8. 保存 bool 反馈与 CanSaveWorld ----------------
    [Test]
    public void SaveFeedbackUsesExistingBoolAndRespectsWorldLock()
    {
        var building = Complete("building_farm", 0);
        string workplace = Workplace(building, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        ForceSaveFailure(true);
        var failed = Command("SaveWorld");
        Assert.AreEqual(ManagementCommandOutcome.SaveRejected, failed.Outcome);
        Assert.IsFalse(failed.Succeeded);
        Assert.AreEqual("SaveReturnedFalse", failed.DomainDiagnostic);
        Assert.IsTrue(failed.Message.Contains("未成功"));
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"), "保存失败保留内存状态，可稍后重试");

        ForceSaveFailure(false);
        Call("SetExpeditionPersonIds", new object[] { new[] { "person_beta" } });
        var busy = Command("SaveWorld");
        Assert.AreEqual(ManagementCommandOutcome.WorldBusy, busy.Outcome);
        Assert.IsFalse(busy.Message.Contains("成功"));
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"));
    }

    // ---------------- 9. 静态兼容岗位：显示与撤职，但不作正式新增目标 ----------------
    [Test]
    public void LegacyStaticAssignmentIsVisibleRemovableButNotANewTarget()
    {
        Assert.AreEqual(VillageAssignmentResult.Success, Call("TryAssignPerson", "person_alpha", "ws_farm_01"));

        var snapshot = Read();
        var legacy = snapshot.LegacyAssignments.Single(r => r.WorkplaceId == "ws_farm_01");
        Assert.AreEqual(ManagementWorkplaceKind.LegacyCompatibility, legacy.Kind);
        Assert.AreEqual(2, legacy.Capacity);
        CollectionAssert.AreEquivalent(new[] { "person_alpha" }, legacy.BoundPersonIds);
        Assert.IsFalse(legacy.CanAssign, "静态兼容记录不是正式新增派工目标");
        Assert.IsTrue(snapshot.Person("person_alpha").AssignmentIsLegacy);

        var refused = Command("AssignToWorkplace", "person_beta", "ws_farm_01");
        Assert.AreEqual(ManagementCommandOutcome.StaticTargetNotAssignable, refused.Outcome);
        Assert.AreEqual(1, Assignments.Records.Count);

        var removed = Command("RemoveAssignment", "person_alpha");
        Assert.IsTrue(removed.Succeeded, removed.ToString());
        Assert.AreEqual(0, Assignments.GetOccupantCount("ws_farm_01"));
        Assert.IsNull(Read().LegacyAssignments.SingleOrDefault(r => r.WorkplaceId == "ws_farm_01"));
    }

    // ---------------- 10. 劳动中撤职：不再从旧岗位累计证据（有界隔离验证） ----------------
    [Test]
    public void RemovingAssignmentDuringLaborStopsOldWorkplaceEvidence()
    {
        var building = Complete("building_farm", 0);
        string workplace = Workplace(building, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        Advance(481);
        Assert.AreEqual("岗位劳动", Call("GetWorkExecutionLabel", "person_alpha"));
        Assert.AreEqual(1, Read().Workplace(workplace).LaborCount);
        int evidenceBefore = EvidenceMinutes(workplace);
        Assert.Greater(evidenceBefore, 0, "撤职前必须已产生真实劳动证据");

        var removed = Command("RemoveAssignment", "person_alpha");
        Assert.IsTrue(removed.Succeeded, removed.ToString());
        Assert.AreEqual("空闲", Call("GetWorkExecutionLabel", "person_alpha"));
        Assert.AreEqual(0, Read().Workplace(workplace).BoundCount);

        Advance(483);
        Assert.AreEqual(evidenceBefore, EvidenceMinutes(workplace), "撤职后不得再从旧岗位累计劳动证据");
        Assert.IsFalse(Arrivals().Any(a => a.PersonId == "person_alpha"), "旧到岗意图在下一边界被清理");
    }

    // ---------------- 11. 资源真实事实与在村供餐对象 ----------------
    [Test]
    public void ResourcesAndNutritionFactsComeFromExistingState()
    {
        var building = Complete("building_farm", 0);
        Resources.AddIngredients(4);
        Resources.AddFood(7);
        SetNutritionFacts(1, hungry: true, consecutiveUnderfedDays: 2);

        var snapshot = Read();
        Assert.AreEqual(4, snapshot.Resources.Ingredients);
        Assert.AreEqual(7, snapshot.Resources.Food);
        Assert.AreEqual(3, snapshot.Resources.InVillageCount);
        CollectionAssert.AreEquivalent(new[] { "person_alpha", "person_beta", "person_gamma" },
            snapshot.Resources.InVillagePersonIds);
        CollectionAssert.AreEquivalent(new[] { "person_beta" }, snapshot.Resources.HungryPersonIds);

        var farm = snapshot.Resources.Workplaces.Single(w => w.BuildingInstanceId == Id(building));
        Assert.AreEqual("Farm", farm.RoleLabel);
        Assert.AreEqual(Workplace(building, "main"), farm.WorkplaceId);
        Assert.AreEqual(2, farm.Capacity);
        Assert.IsTrue(snapshot.Person("person_beta").HasNutritionFacts);
        Assert.IsTrue(snapshot.Person("person_beta").Hungry);
        Assert.AreEqual(2, snapshot.Person("person_beta").ConsecutiveUnderfedDays);
    }

    // ---------------- 12. 施工劳动不回流 Primary Workplace 的实际劳动人数 ----------------
    [Test]
    public void ConstructionLaborDoesNotCountTowardPrimaryWorkplace()
    {
        var first = Complete("building_farm", 0);
        string workplace = Workplace(first, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        // 前置：先建立真实非零岗位劳动与证据。
        Advance(481);
        Assert.AreEqual("岗位劳动", Call("GetWorkExecutionLabel", "person_alpha"));
        Assert.AreEqual(1, Read().Workplace(workplace).LaborCount);
        int evidenceBefore = EvidenceMinutes(workplace);
        Assert.Greater(evidenceBefore, 0, "前置必须已有真实岗位劳动证据");

        var second = Place("building_farm", 1);
        Assert.AreEqual(VillageProjectResult.Success, Call("TryAddConstructionWorker", Id(second), "person_alpha"));
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"), "施工加入保留 Primary Assignment");
        var project = Projects.GetActiveProject((string)Get(second, "ConstructionProjectId"));
        long projectLaborBefore = project.LaborRemainder;

        Advance(500);
        Assert.AreEqual("项目劳动", Call("GetWorkExecutionLabel", "person_alpha"));
        var row = Read().Workplace(workplace);
        Assert.AreEqual(1, row.BoundCount, "长期绑定仍在原岗位");
        Assert.AreEqual(0, row.LaborCount, "实际目标为施工项目时不得计作原岗位劳动");
        Assert.AreEqual(1, Read().Construction.Single(c => c.BuildingInstanceId == Id(second)).WorkerCount,
            "施工工人事实独立于原岗位投影");
        Assert.Greater(project.LaborRemainder, projectLaborBefore, "真实施工劳动独立累计");
        Assert.AreEqual(evidenceBefore, EvidenceMinutes(workplace), "施工劳动不得写成原岗位证据");
    }

    // ---------------- 13. 通勤 / 下班 / 活动不冒充岗位劳动，正常劳动仍计数 ----------------
    [Test]
    public void WorkplaceLaborOnlyCountsActualWorkplaceExecution()
    {
        // 夹具窗口所有权隔离（既有公开注入点 ConfigureActivitySchedule，未改领域规则）：
        // 生产默认日程的机会点为 420/1020/1200；Advance 到 470 时会由 AdvanceSink 在 420 合法
        // 打开当日 intraday 窗口。窗口是「当日唯一」，因此夹具随后再为同一 DayStamp 打开日窗口
        // 会被 ActivityLifecycleService 正确判为 WindowConflict（同日不同快照）。
        // 本用例只需要一个当日窗口来承载真实 claim，故把机会点排到推进区间（0→497）之外，
        // 让夹具成为当日窗口的唯一建立者；断言不做任何放松，TearDown 会恢复原日程。
        // 日程类型属于 Assembly-CSharp，本测试装配仅引用 Village.Runtime，故经既有 getter/注入点反射调用。
        Call("ConfigureActivitySchedule",
            Activator.CreateInstance(ActivitySchedule, new object[] { new[] { 1020, 1200 }, 15, 30, null }));

        var building = Complete("building_farm", 0);
        string workplace = Workplace(building, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        // 上班前（仍在通勤窗口）：有绑定但未开始劳动。
        Advance(470);
        Assert.AreEqual(0, Read().Workplace(workplace).LaborCount);

        // 正常岗位劳动：真实非零前置。
        Advance(481);
        Assert.AreEqual("岗位劳动", Call("GetWorkExecutionLabel", "person_alpha"));
        Assert.AreEqual(1, Read().Workplace(workplace).LaborCount);
        int evidence = EvidenceMinutes(workplace);
        Assert.Greater(evidence, 0, "前置必须已有真实岗位劳动证据");

        // 通勤：真实 Work 到岗尚未 ready。
        Work.Leave("person_alpha");
        Assert.IsFalse(Work.Arrived("person_alpha", workplace, 481));
        Assert.AreEqual("通勤", Call("GetWorkExecutionLabel", "person_alpha"));
        Assert.AreEqual(0, Read().Workplace(workplace).LaborCount, "通勤不得冒充岗位劳动");

        Advance(496);
        Advance(497);
        Assert.AreEqual("岗位劳动", Call("GetWorkExecutionLabel", "person_alpha"));
        Assert.AreEqual(1, Read().Workplace(workplace).LaborCount, "真实到岗后正常计数");
        Assert.Greater(EvidenceMinutes(workplace), evidence, "真实到岗劳动继续产生证据");

        // 活动：真实 Activity claim 期间不冒充岗位劳动。
        CommitRestActivity("person_alpha");
        Assert.IsNotNull(ActivityCall("QueryClaim", "person_alpha"));
        Assert.AreEqual(0, Read().Workplace(workplace).LaborCount, "活动进行中不得冒充岗位劳动");
    }

    // ---------------- 14. 死亡 Unavailable：所有管理命令按既有可用性校验拒绝且零写入 ----------------
    [Test]
    public void DeadUnavailablePersonIsRejectedByEveryManagementCommandWithoutWrite()
    {
        var completed = Complete("building_farm", 0);
        string workplace = Workplace(completed, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        var construction = Place("building_farm", 1);
        string constructionId = Id(construction);
        Assert.AreEqual(VillageProjectResult.Success,
            (VillageProjectResult)Call("TryAddConstructionWorker", constructionId, "person_alpha"));
        var project = Projects.GetActiveProject((string)Get(construction, "ConstructionProjectId"));
        Assert.IsNotNull(project);

        // 真实长期人物字段（fixture 只改既有 CharacterInstance.IsDead，不新增运行时私有钩子）。
        SetDead("person_alpha", true);

        string saveBefore = SaveJson();
        long minuteBefore = Clock.CurrentInstant.Minute;
        int recordsBefore = Assignments.Records.Count;
        int activeProjectsBefore = Projects.ActiveProjects.Count;
        int ingredientsBefore = Resources.Ingredients;
        int foodBefore = Resources.Food;
        List<string> participantsBefore = project.ParticipantPersonIds.ToList();

        var assign = Command("AssignToWorkplace", "person_alpha", workplace);
        var addWorker = Command("AddConstructionWorker", constructionId, "person_alpha");
        var removeWorker = Command("RemoveConstructionWorker", constructionId, "person_alpha");
        var removeAssignment = Command("RemoveAssignment", "person_alpha");

        foreach (var result in new[] { assign, addWorker, removeWorker, removeAssignment })
        {
            Assert.AreEqual(ManagementCommandOutcome.PersonNotInVillage, result.Outcome, result.ToString());
            Assert.IsFalse(result.Succeeded, result.ToString());
            Assert.IsTrue(result.Message.Contains("不可用"), result.Message);
        }

        Assert.AreEqual(saveBefore, SaveJson(), "死亡人物命令不得改动任何已持久化事实");
        Assert.AreEqual(minuteBefore, Clock.CurrentInstant.Minute);
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"), "撤职被拒后原职责必须保留");
        Assert.AreEqual(recordsBefore, Assignments.Records.Count);
        Assert.AreEqual(activeProjectsBefore, Projects.ActiveProjects.Count);
        CollectionAssert.AreEquivalent(participantsBefore, project.ParticipantPersonIds,
            "撤出施工被拒后项目参与者必须保留");
        Assert.AreEqual(ingredientsBefore, Resources.Ingredients);
        Assert.AreEqual(foodBefore, Resources.Food);

        var row = Read().Person("person_alpha");
        Assert.AreEqual(VillageAvailability.Unavailable, row.Availability);
        Assert.AreEqual("不可用", row.AvailabilityLabel);
    }

    // ---------------- 15. 世界忙（CanSaveWorld=false）：管理命令 WorldBusy 且零写入 ----------------
    [Test]
    public void WorldBusyRejectsManagementCommandsWithoutWrite()
    {
        var completed = Complete("building_farm", 0);
        string workplace = Workplace(completed, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        var construction = Place("building_farm", 1);
        string constructionId = Id(construction);
        Assert.AreEqual(VillageProjectResult.Success,
            (VillageProjectResult)Call("TryAddConstructionWorker", constructionId, "person_alpha"));
        var project = Projects.GetActiveProject((string)Get(construction, "ConstructionProjectId"));
        Assert.IsNotNull(project);

        // 只走既有公共远征入口制造真实 WorldBusy，不反射私有锁、不新增测试钩子。
        Call("SetExpeditionPersonIds", new object[] { new[] { "person_beta" } });
        Assert.IsFalse(CanSaveWorld, "前置：既有远征登记必须使 CanSaveWorld=false");

        string saveBefore = SaveJson();
        long minuteBefore = Clock.CurrentInstant.Minute;
        int recordsBefore = Assignments.Records.Count;
        int activeProjectsBefore = Projects.ActiveProjects.Count;
        int ingredientsBefore = Resources.Ingredients;
        int foodBefore = Resources.Food;
        List<string> participantsBefore = project.ParticipantPersonIds.ToList();

        var assign = Command("AssignToWorkplace", "person_alpha", workplace);
        var addWorker = Command("AddConstructionWorker", constructionId, "person_alpha");
        var removeWorker = Command("RemoveConstructionWorker", constructionId, "person_alpha");
        var removeAssignment = Command("RemoveAssignment", "person_alpha");

        foreach (var result in new[] { assign, addWorker, removeWorker, removeAssignment })
        {
            Assert.AreEqual(ManagementCommandOutcome.WorldBusy, result.Outcome, result.ToString());
            Assert.IsFalse(result.Succeeded, result.ToString());
        }

        Assert.AreEqual(saveBefore, SaveJson(), "世界忙时管理命令不得改动任何已持久化事实");
        Assert.AreEqual(minuteBefore, Clock.CurrentInstant.Minute);
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"));
        Assert.AreEqual(recordsBefore, Assignments.Records.Count);
        Assert.AreEqual(activeProjectsBefore, Projects.ActiveProjects.Count);
        CollectionAssert.AreEquivalent(participantsBefore, project.ParticipantPersonIds);
        Assert.AreEqual(ingredientsBefore, Resources.Ingredients);
        Assert.AreEqual(foodBefore, Resources.Food);
        Assert.IsFalse(CanSaveWorld, "命令不得自行解除既有世界忙状态");
    }

    // ---------------- 16. 完工建筑：施工加入按既有校验拒绝且零写入 ----------------
    [Test]
    public void CompletedBuildingRejectsConstructionWorkerWithoutWrite()
    {
        var building = Complete("building_farm", 0);
        Assert.IsNull((string)Get(building, "ConstructionProjectId"));

        string saveBefore = SaveJson();
        long minuteBefore = Clock.CurrentInstant.Minute;
        int recordsBefore = Assignments.Records.Count;
        int activeProjectsBefore = Projects.ActiveProjects.Count;
        int completedProjectsBefore = Projects.CompletedProjects.Count;
        int ingredientsBefore = Resources.Ingredients;
        int foodBefore = Resources.Food;

        var result = Command("AddConstructionWorker", Id(building), "person_alpha");
        Assert.AreEqual(ManagementCommandOutcome.TargetAlreadyCompleted, result.Outcome, result.ToString());
        Assert.IsFalse(result.Succeeded);
        Assert.IsTrue(result.Message.Contains("已完工"), result.Message);

        Assert.AreEqual(saveBefore, SaveJson(), "完工建筑拒绝加入施工后不得产生任何写入");
        Assert.AreEqual(minuteBefore, Clock.CurrentInstant.Minute);
        Assert.IsNull((string)Get(building, "ConstructionProjectId"), "拒绝后不得为完工实例新建施工项目");
        Assert.AreEqual(recordsBefore, Assignments.Records.Count);
        Assert.AreEqual(activeProjectsBefore, Projects.ActiveProjects.Count);
        Assert.AreEqual(completedProjectsBefore, Projects.CompletedProjects.Count);
        Assert.AreEqual(0, Assignments.Records.Count);
        Assert.IsEmpty(Projects.ActiveProjects);
        Assert.AreEqual(ingredientsBefore, Resources.Ingredients);
        Assert.AreEqual(foodBefore, Resources.Food);
        Assert.IsFalse(Read().Construction.Any(c => c.BuildingInstanceId == Id(building)),
            "完工实例不得重新进入施工投影");
    }

    // ---------------- 17. 撤职删除记录与累计统计；已结算贡献不回滚、旧岗位不再新增 ----------------
    [Test]
    public void RemovingAssignmentDropsRecordAndAccumulatedLaborButKeepsSettledContribution()
    {
        // 夹具隔离：本用例不需要任何真实活动 claim，只需把机会点清空（既有公开注入点 + TearDown 恢复）。
        // 机会点必须是合法日内分钟（0..1439）：旧写法 { 4000 } 会被 IntradayActivitySchedule 构造器
        // 按既有契约判为 ArgumentException("Invalid activity timing")——那是非法测试输入，不是生产缺陷。
        // 空机会点配置已被其他 Edit/Play 用例使用，不改变任何领域规则，断言保持原样。
        Call("ConfigureActivitySchedule",
            Activator.CreateInstance(ActivitySchedule, new object[] { new int[0], 15, 30, null }));

        var building = Complete("building_farm", 0);
        string workplace = Workplace(building, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded);

        // 真实推进跨两个午夜：午夜 Day Tick 把当日真实劳动证据结算进 Assignment 累计统计。
        Advance(2881);
        var record = Assignments.GetRecord("person_alpha");
        Assert.IsNotNull(record);
        Assert.Greater(record.WorkedMinutes, 480, "撤职前必须有真实累计工作统计");
        Assert.GreaterOrEqual(record.TotalExecutedDays, 1);
        int contributionBefore = Assignments.TotalTestContribution;
        Assert.Greater(contributionBefore, 0, "已结算的村庄级累计贡献必须是非零前置");
        int saveContributionBefore = Assignments.ToSaveData().TotalTestContribution;

        var removed = Command("RemoveAssignment", "person_alpha");
        Assert.IsTrue(removed.Succeeded, removed.ToString());

        Assert.IsNull(Assignments.GetRecord("person_alpha"), "撤职必须删除原 Assignment 记录及其累计统计");
        Assert.IsNull(Assignments.GetWorkplaceId("person_alpha"));
        Assert.AreEqual(0, Assignments.GetOccupantCount(workplace));
        Assert.AreEqual(contributionBefore, Assignments.TotalTestContribution,
            "已结算的村庄级累计贡献不回滚、不追回");
        Assert.AreEqual(saveContributionBefore, Assignments.ToSaveData().TotalTestContribution);

        int evidenceAfterRemoval = EvidenceMinutes(workplace);
        Advance(3361);

        Assert.IsNull(Assignments.GetRecord("person_alpha"), "后续旧岗位不得重建 Assignment");
        Assert.AreEqual(contributionBefore, Assignments.TotalTestContribution,
            "后续旧岗位不得新增累计贡献");
        Assert.AreEqual(evidenceAfterRemoval, EvidenceMinutes(workplace),
            "后续旧岗位不得新增劳动证据");
        Assert.AreEqual(0, Read().Workplace(workplace).LaborCount);
    }

    // ---------------- 18. Activity claim 持有者：合法加入施工，Labor 延后到合法可用 ----------------
    /// <summary>
    /// 现契约专项（只记录/锁定生产行为，不新增 busy 拒绝、不改 Work/Clock/Activity）：
    /// 1) Activity claim 只占 Activity Authority，不写 Temporary Occupancy / Assignment / Project；
    ///    Gateway.CheckPerson 只看来源存在、远征/死亡可用性与 CanSaveWorld，故 claim 持有者加入施工是
    ///    当前合法路径（真实回执 Success），不是“busy”拒绝；
    /// 2) 加入施工保留 Primary Assignment，claim 身份保持，项目参与者集准确；
    /// 3) 真实通勤工作边界之前、claim 持续期间，ArrivedWorkers 跳过该人物（prepare 与查询两条路径），
    ///    故无到场登记、无劳动、项目不凭空推进；
    /// 4) 到达 MorningStart - CommuteMinutes 的真实工作边界时，生产按既有“工作优先”规则中断
    ///    未排程 Rest（Committed→Interrupted），claim 由 Activity Authority 合法释放；同一边界登记
    ///    Arrival，ReadyMinute = 边界 + 既有 CommuteMinutes；WorkStart 到场但尚未结算劳动，
    ///    工作时段第一分钟才产生真实施工劳动。
    /// </summary>
    [Test]
    public void ManagementConstructionClaim_PreservesPrimaryAndDefersLaborUntilLegalAvailability()
    {
        // 窗口所有权隔离（复用既有公开注入点 ConfigureActivitySchedule；TearDown 恢复原日程）：
        // 生产默认机会点 420 会在本用例推进到通勤边界前打开当日窗口，导致夹具 CommitRestActivity 的
        // OpenDailyWindow 按契约返回 WindowConflict。本用例只需要一个真实日窗口承载 claim，故把机会点排到推进区间之外。
        Call("ConfigureActivitySchedule",
            Activator.CreateInstance(ActivitySchedule, new object[] { new[] { 1020, 1200 }, 15, 30, null }));

        var completed = Complete("building_farm", 0);
        string workplace = Workplace(completed, "main");
        Assert.IsTrue(Command("AssignToWorkplace", "person_alpha", workplace).Succeeded,
            "前置：必须存在真实 Primary Assignment");

        var construction = Place("building_farm", 1);
        string constructionId = Id(construction);

        // 真实 WorkSchedule 派生边界（不写死绝对分钟）：生产在 MorningStart - CommuteMinutes
        // 建立通勤工作边界，WorkStart 起才可能结算劳动。
        long commute = Work.Schedule.CommuteMinutes;
        long commuteBoundary = Work.Schedule.MorningStart - commute;
        long workStart = Work.Schedule.MorningStart;
        long firstLaborMinute = workStart + 1;

        // 只推进到真实通勤边界之前，避免前置推进本身为 alpha 留下旧 Arrival 行，干扰后续“无到场”断言。
        Advance(commuteBoundary - 1);
        Assert.IsFalse(Arrivals().Any(a => a.PersonId == "person_alpha"), "前置：claim 之前不应已有到场登记");

        // 真实非空 QueryClaim（既有公开窗口/commit helper；不私有写 claim，不随机选择活动）。
        CommitRestActivity("person_alpha");
        string claimId = (string)ActivityCall("QueryClaim", "person_alpha");
        Assert.AreEqual("management:rest:person_alpha", claimId, "前置：必须建立真实 Activity claim");
        Assert.AreEqual("Committed", Get(ActivityCall("QueryActivityInstance", claimId), "State").ToString());

        // 当前契约：claim 持有者仍可被合法加入施工（不新增 busy 拒绝，不伪装成 WorldBusy）。
        var added = Command("AddConstructionWorker", constructionId, "person_alpha");
        Assert.AreEqual(ManagementCommandOutcome.Success, added.Outcome, added.ToString());
        Assert.IsTrue(added.Succeeded, added.ToString());
        Assert.IsEmpty(added.DomainDiagnostic);
        Assert.IsTrue(added.Message.Contains("原主要职责保留"), added.Message);

        // claim 身份保持；Primary Assignment 不变；项目参与者集准确。
        Assert.AreEqual(claimId, ActivityCall("QueryClaim", "person_alpha"), "加入施工不得释放或替换 claim");
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"),
            "施工加入不得改变长期 Primary Assignment");
        var project = Projects.GetActiveProject((string)Get(construction, "ConstructionProjectId"));
        Assert.IsNotNull(project);
        CollectionAssert.AreEquivalent(new[] { "person_alpha" }, project.ParticipantPersonIds,
            "施工参与者集必须准确");
        Assert.AreEqual(0, project.Progress);
        Assert.AreEqual(0L, project.LaborRemainder);

        // 只读投影：长期绑定仍在原岗位，短期占用由 claim 表达，可用性不是远征/不可用。
        var duringClaim = Read();
        Assert.AreEqual(VillageAvailability.InVillage, duringClaim.Person("person_alpha").Availability);
        Assert.AreEqual(workplace, duringClaim.Person("person_alpha").PrimaryWorkplaceId);
        Assert.AreEqual("自主活动进行中：" + claimId,
            duringClaim.Person("person_alpha").TemporaryCommitment);
        var constructionRow = duringClaim.Construction.Single(c => c.BuildingInstanceId == constructionId);
        Assert.AreEqual(1, constructionRow.WorkerCount);
        CollectionAssert.AreEquivalent(new[] { "person_alpha" }, constructionRow.WorkerPersonIds);

        string destination = (string)Call("ProjectDestination", project.ProjectId);
        Assert.AreEqual("construction:" + constructionId, destination, "施工执行目标必须是该建筑实例");

        // 分界一：真实通勤边界前一分钟仍是休息时段，claim 由 Activity Authority 持有；
        // 管理命令与投影都不得在该时段产生任何施工贡献。
        Assert.AreEqual(commuteBoundary - 1, Clock.CurrentInstant.Minute);
        Assert.AreEqual(claimId, ActivityCall("QueryClaim", "person_alpha"),
            "claim 在真实工作边界之前必须保持同一身份");
        Assert.AreEqual("Committed", Get(ActivityCall("QueryActivityInstance", claimId), "State").ToString(),
            "claim 期间实例保持 Committed");
        Assert.AreEqual("下班 / 休息", Call("GetWorkExecutionLabel", "person_alpha"),
            "非工作时段且未到岗的 claim 持有者不是劳动状态");
        Assert.IsFalse(Arrivals().Any(a => a.PersonId == "person_alpha"),
            "claim 持续期间 ArrivedWorkers 必须跳过该人物，不得登记到场");
        Assert.IsFalse(Work.HasArrived("person_alpha", destination, commuteBoundary - 1));
        Assert.AreEqual(0, project.Progress, "claim 期间项目不得凭空推进");
        Assert.AreEqual(0L, project.LaborRemainder, "claim 期间不得产生项目劳动");
        Assert.AreEqual(0, Read().Workplace(workplace).LaborCount, "claim 不得冒充原岗位劳动");
        Assert.AreEqual(0, EvidenceMinutes(workplace), "claim 不得写入原岗位劳动证据");
        Assert.AreEqual(1, Read().Construction.Single(c => c.BuildingInstanceId == constructionId).WorkerCount,
            "长期/名单状态仍显示 1 名施工参与者");
        TestContext.WriteLine("[CLAIM-CONSTRUCTION] before work boundary minute=" + Clock.CurrentInstant.Minute
            + " id=" + claimId + " outcome=" + added.Outcome
            + " label=" + Call("GetWorkExecutionLabel", "person_alpha")
            + " arrivals=" + Arrivals().Count + " project.Progress=" + project.Progress
            + " laborRemainder=" + project.LaborRemainder + " primary=" + Assignments.GetWorkplaceId("person_alpha"));

        // 分界二：推进到真实通勤工作边界。生产按既有“工作优先”规则在 CompleteActivityBoundary 中
        // 把该未排程 Rest 从 Committed 终止为 Interrupted，claim 由 Activity Authority 合法释放；
        // 这不是 AddConstructionWorker / RefreshWorkIntent 抢占（加入施工回执已 Success，边界前后没有管理命令）。
        Advance(commuteBoundary);
        Assert.AreEqual(commuteBoundary, Clock.CurrentInstant.Minute);
        Assert.AreEqual("Interrupted", Get(ActivityCall("QueryActivityInstance", claimId), "State").ToString(),
            "工作优先边界按既有 FSM 终止未排程 Rest（Committed → Interrupted）");
        Assert.IsNull(ActivityCall("QueryClaim", "person_alpha"),
            "Rest 在真实工作边界合法终止后，claim 必须由 Activity Authority 释放");
        CollectionAssert.AreEquivalent(new[] { "person_alpha" }, project.ParticipantPersonIds,
            "claim 释放不是撤出施工，参与者集必须保留");
        Assert.AreEqual(1, Read().Construction.Single(c => c.BuildingInstanceId == constructionId).WorkerCount);
        var arrival = Arrivals().Single(a => a.PersonId == "person_alpha");
        Assert.AreEqual(destination, arrival.Destination);
        Assert.AreEqual(commuteBoundary + commute, arrival.ReadyMinute,
            "既有到达契约：ReadyMinute = 真实登记分钟 + CommuteMinutes");
        Assert.IsFalse(Work.HasArrived("person_alpha", destination, commuteBoundary),
            "通勤未满 CommuteMinutes 不得视为到场");
        Assert.AreEqual(0, project.Progress, "活动终止瞬间项目不得凭空推进");
        Assert.AreEqual(0L, project.LaborRemainder, "活动终止瞬间不得产生项目劳动");
        Assert.AreEqual(0, Read().Workplace(workplace).LaborCount);
        Assert.AreEqual(0, EvidenceMinutes(workplace));
        TestContext.WriteLine("[CLAIM-CONSTRUCTION] legal end minute=" + Clock.CurrentInstant.Minute
            + " state=Interrupted claim=" + ActivityCall("QueryClaim", "person_alpha")
            + " arrival.destination=" + arrival.Destination + " readyMinute=" + arrival.ReadyMinute);

        // 分界三：WorkStart 已达到 ReadyMinute（真实到场），但前一区间不是工作时段，
        // 因而不结算任何劳动。
        Advance(workStart);
        Assert.AreEqual(workStart, Clock.CurrentInstant.Minute);
        Assert.IsTrue(Work.HasArrived("person_alpha", destination, workStart),
            "通勤满 CommuteMinutes 后必须真实到场");
        Assert.AreEqual(0L, project.LaborRemainder, "到场边界本身不结算劳动");
        Assert.AreEqual(0, project.Progress);
        TestContext.WriteLine("[CLAIM-CONSTRUCTION] at work start arrived=True laborRemainder="
            + project.LaborRemainder);

        // 分界四：工作时段的第一分钟才产生真实施工劳动；Primary 与参与者集保持不变。
        Advance(firstLaborMinute);
        Assert.AreEqual(firstLaborMinute, Clock.CurrentInstant.Minute);
        Assert.IsTrue(Work.HasArrived("person_alpha", destination, firstLaborMinute));
        Assert.AreEqual("项目劳动", Call("GetWorkExecutionLabel", "person_alpha"));
        var projectDef = Projects.GetProjectDef(project.ProjectDefId);
        long expectedLabor = (firstLaborMinute - workStart) * projectDef.TestProgressPerParticipantPerDay;
        Assert.AreEqual(expectedLabor, project.LaborRemainder,
            "仅工作时段内的真实到场分钟计入项目劳动");
        Assert.AreEqual(0, project.Progress);
        Assert.IsFalse(project.ExecutionPaused, "有真实到场劳动后项目不再处于零人暂停");
        Assert.AreEqual(workplace, Assignments.GetWorkplaceId("person_alpha"),
            "真实劳动期间 Primary Assignment 仍不变");
        CollectionAssert.AreEquivalent(new[] { "person_alpha" }, project.ParticipantPersonIds);
        Assert.AreEqual(0, EvidenceMinutes(workplace), "施工劳动不得写成原岗位证据");
        Assert.AreEqual(1, Assignments.GetOccupantCount(workplace),
            "原岗位长期绑定名额不因施工劳动变化");
        TestContext.WriteLine("[CLAIM-CONSTRUCTION] first labor minute=" + Clock.CurrentInstant.Minute
            + " label=" + Call("GetWorkExecutionLabel", "person_alpha")
            + " laborRemainder=" + project.LaborRemainder + " progress=" + project.Progress
            + " primary=" + Assignments.GetWorkplaceId("person_alpha"));
    }

    // ---------------- 19. 真实双 FunctionSlot：Gateway 派工 / 满员 / 撤职与容量独立 ----------------
    /// <summary>
    /// 原规格 §4 / §8 的双 slot 真实 Gateway 契约（此前仅投影层覆盖）：
    ///   1) 用 Editor-only internal 测试缝注入一份**内存** BuildingRegistry（真实 building_farm 定义 +
    ///      main(ws_farm_01, Capacity 2) + aux(ws_test_02, Capacity 1)），不写盘、不改 Root / 生产配置；
    ///   2) 全程走真实链：place → 两名工人加入施工 → 真实推进时钟完工 → FinalizeConstruction 自动注册
    ///      两个 Workplace（不手工伪造 Completed / assignment / arrival）；
    ///   3) Gateway 分别派工 / 满员拒绝 / AlreadyAssigned / 撤职：两 slot 名额与命令互不影响；
    ///   4) 只读投影逐 WorkplaceId 与真实注册 / 绑定互核，同一 BuildingInstanceId 下 slot 必须独立；
    ///   5) finally 恢复开工前覆盖状态并 Reset，默认建筑注册表内容与开工相同、其他领域注册结果不变，
    ///      Editor 测试缝保持 internal（无 public 测试入口）。
    /// </summary>
    [Test]
    public void ManagementGateway_TwoFunctionSlots_KeepCapacityAndCommandsIndependent()
    {
        Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry baseline, out string baselineDiagnostic),
            baselineDiagnostic);
        string baselineShape = DescribeBuildingRegistry(baseline);
        string otherDomainsBefore = CaptureOtherDomainFacts();

        BuildingRegistry previousOverride = BuildingsOverrideForTests();
        Assert.IsNull(previousOverride, "开工前置：不得已有其他用例遗留的 BuildingRegistry 覆盖");

        try
        {
            // 真实定义目录 + 仅内存的第二 FunctionBinding（known profile ws_test_02，Capacity 1）。
            var twoSlotDefinitions = new BuildingRegistry(baseline.All, baseline.FunctionBindings.Concat(new[]
            {
                new BuildingFunctionBinding
                {
                    BuildingDefinitionId = "building_farm",
                    FunctionSlotId = "aux",
                    FunctionType = "Workplace",
                    FunctionProfileId = "ws_test_02"
                }
            }));
            SetBuildingsOverrideForTests(twoSlotDefinitions);
            Assert.AreSame(twoSlotDefinitions, BuildingsOverrideForTests(), "内存覆盖必须真实生效");

            // 真实链：place → 两名工人加入施工 → 推进时钟完工 → 自动注册两个 Workplace。
            var building = CompleteByAdvance("building_farm", 0, "person_alpha", "person_beta");
            string instanceId = Id(building);
            string mainWorkplace = Workplace(building, "main");
            string auxWorkplace = Workplace(building, "aux");
            var assignments = Assignments;
            var mainDef = assignments.GetWorkplaceDef(mainWorkplace);
            var auxDef = assignments.GetWorkplaceDef(auxWorkplace);
            Assert.IsNotNull(mainDef, "真实完工链必须自动注册 main");
            Assert.IsNotNull(auxDef, "真实完工链必须自动注册 aux");
            Assert.AreEqual(2, mainDef.Capacity);
            Assert.AreEqual(1, auxDef.Capacity);
            Assert.AreNotEqual(mainWorkplace, auxWorkplace);

            // Gateway 派工：两 slot 各自真实成功。
            var alphaMain = Command("AssignToWorkplace", "person_alpha", mainWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.Success, alphaMain.Outcome, alphaMain.ToString());
            var betaAux = Command("AssignToWorkplace", "person_beta", auxWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.Success, betaAux.Outcome, betaAux.ToString());
            Assert.AreEqual(1, assignments.GetOccupantCount(mainWorkplace));
            Assert.AreEqual(1, assignments.GetOccupantCount(auxWorkplace));

            // aux 满员（Capacity 1）→ 真实 CapacityFull；main 名额不受影响。
            var gammaAux = Command("AssignToWorkplace", "person_gamma", auxWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.DomainRejected, gammaAux.Outcome, gammaAux.ToString());
            Assert.AreEqual("CapacityFull", gammaAux.DomainDiagnostic);
            Assert.IsNull(assignments.GetWorkplaceId("person_gamma"), "满员拒绝不得写入");
            Assert.AreEqual(1, assignments.GetOccupantCount(auxWorkplace));
            Assert.AreEqual(1, assignments.GetOccupantCount(mainWorkplace), "aux 满员不得改变 main 名额");

            // 已在 main 的人物再派 aux → 真实 AlreadyAssigned；不自动换岗。
            var alphaAux = Command("AssignToWorkplace", "person_alpha", auxWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.DomainRejected, alphaAux.Outcome, alphaAux.ToString());
            Assert.AreEqual("AlreadyAssigned", alphaAux.DomainDiagnostic);
            Assert.AreEqual(mainWorkplace, assignments.GetWorkplaceId("person_alpha"), "AlreadyAssigned 不得自动换岗");

            // main 仍有空位：gamma 可真实派入 main（aux 满员不影响 main 的派工能力）。
            var gammaMain = Command("AssignToWorkplace", "person_gamma", mainWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.Success, gammaMain.Outcome, gammaMain.ToString());
            Assert.AreEqual(2, assignments.GetOccupantCount(mainWorkplace));
            Assert.AreEqual(1, assignments.GetOccupantCount(auxWorkplace));

            // 撤职 aux 释放 aux 名额，main 绑定不变。
            var removedBeta = Command("RemoveAssignment", "person_beta");
            Assert.IsTrue(removedBeta.Succeeded, removedBeta.ToString());
            Assert.IsNull(assignments.GetWorkplaceId("person_beta"));
            Assert.AreEqual(0, assignments.GetOccupantCount(auxWorkplace));
            Assert.AreEqual(2, assignments.GetOccupantCount(mainWorkplace), "撤职 aux 不得改变 main 名额");

            // main 满员而 aux 空：真实 CapacityFull 证明容量互不影响。
            var betaMain = Command("AssignToWorkplace", "person_beta", mainWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.DomainRejected, betaMain.Outcome, betaMain.ToString());
            Assert.AreEqual("CapacityFull", betaMain.DomainDiagnostic);
            Assert.IsNull(assignments.GetWorkplaceId("person_beta"));

            // 撤职 main 释放名额后同一真实命令成功；aux 仍空。
            var removedGamma = Command("RemoveAssignment", "person_gamma");
            Assert.IsTrue(removedGamma.Succeeded, removedGamma.ToString());
            Assert.AreEqual(1, assignments.GetOccupantCount(mainWorkplace));
            var betaMainAgain = Command("AssignToWorkplace", "person_beta", mainWorkplace);
            Assert.AreEqual(ManagementCommandOutcome.Success, betaMainAgain.Outcome, betaMainAgain.ToString());
            Assert.AreEqual(2, assignments.GetOccupantCount(mainWorkplace));
            Assert.AreEqual(0, assignments.GetOccupantCount(auxWorkplace));

            // 只读投影：逐 id 与真实注册 / 绑定互核；同一实例下 slot 身份独立，不按建筑合并。
            var snapshot = Read();
            var mainRow = snapshot.Workplace(mainWorkplace);
            var auxRow = snapshot.Workplace(auxWorkplace);
            Assert.IsNotNull(mainRow, "投影必须包含 main 行");
            Assert.IsNotNull(auxRow, "投影必须包含 aux 行");
            Assert.AreNotEqual(mainRow.WorkplaceId, auxRow.WorkplaceId);
            Assert.AreEqual(instanceId, mainRow.BuildingInstanceId);
            Assert.AreEqual(instanceId, auxRow.BuildingInstanceId, "同一建筑实例下 slot 仍按 WorkplaceId 独立");
            Assert.AreEqual("main", mainRow.FunctionSlotId);
            Assert.AreEqual("aux", auxRow.FunctionSlotId);
            Assert.AreEqual("ws_farm_01", mainRow.FunctionProfileId);
            Assert.AreEqual("ws_test_02", auxRow.FunctionProfileId);
            Assert.AreEqual(ManagementWorkplaceKind.BuildingFunction, mainRow.Kind);
            Assert.AreEqual(ManagementWorkplaceKind.BuildingFunction, auxRow.Kind);
            Assert.IsTrue(mainRow.IsRegistered);
            Assert.IsTrue(auxRow.IsRegistered);
            Assert.AreEqual(2, mainRow.Capacity);
            Assert.AreEqual(1, auxRow.Capacity);
            Assert.IsTrue(mainRow.IsFull, "main 绑定 2 / Capacity 2 必须为满员");
            Assert.IsFalse(auxRow.IsFull, "main 满员不得让 aux 显示满员");

            foreach (string workplaceId in new[] { mainWorkplace, auxWorkplace })
            {
                var registered = assignments.GetWorkplaceDef(workplaceId);
                var row = snapshot.Workplace(workplaceId);
                Assert.AreEqual(registered.WorkplaceId, row.WorkplaceId);
                Assert.AreEqual(registered.DisplayName, row.DisplayName);
                Assert.AreEqual(registered.Capacity, row.Capacity);
                CollectionAssert.AreEquivalent(assignments.GetOccupantPersonIds(workplaceId), row.BoundPersonIds,
                    "投影必须逐 id 与真实注册 / 绑定互核：" + workplaceId);
            }

            CollectionAssert.AreEquivalent(new[] { "person_alpha", "person_beta" }, mainRow.BoundPersonIds);
            Assert.IsEmpty(auxRow.BoundPersonIds);
            Assert.AreEqual(1, snapshot.Workplaces.Count(w => w.WorkplaceId == mainWorkplace));
            Assert.AreEqual(1, snapshot.Workplaces.Count(w => w.WorkplaceId == auxWorkplace));
            Assert.AreEqual(2, snapshot.Workplaces.Count(w => w.BuildingInstanceId == instanceId),
                "同一实例的两个 FunctionSlot 必须各占一行");
        }
        finally
        {
            // 恢复开工前覆盖状态；Reset 清除 Editor 覆盖并让默认 Root 重新解析。运行时世界由既有 fixture TearDown 恢复。
            SetBuildingsOverrideForTests(previousOverride);
            StaticConfigRegistry.Reset();
        }

        // Reset 后复核：覆盖已清、默认注册表内容与开工相同、其他领域 registry 结果未被本缝改变。
        Assert.IsNull(BuildingsOverrideForTests(), "Reset 必须清除 Editor 内部覆盖");
        Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry restored, out string restoredDiagnostic),
            restoredDiagnostic);
        Assert.AreEqual(baselineShape, DescribeBuildingRegistry(restored), "Reset 后默认建筑注册表内容必须与开工相同");
        Assert.AreEqual(otherDomainsBefore, CaptureOtherDomainFacts(),
            "Reset 只清除建筑覆盖，不得改变其他领域注册结果");

        // Editor 隔离 / 公共 API 面：内部测试缝可被反射发现但不是 public，且没有 public ForTests 入口。
        var setter = StaticConfigApi.GetMethod("SetBuildingsOverrideForTests", S);
        Assert.IsNotNull(setter, "Editor 编译下必须能反射发现 internal 测试缝");
        Assert.IsFalse(setter.IsPublic, "测试缝不得是 public API");
        Assert.IsTrue(setter.IsAssembly, "测试缝必须是 internal（assembly）可见性");
        Assert.IsFalse(StaticConfigApi.GetMethods(S).Any(m => m.IsPublic && m.Name.IndexOf("ForTests", StringComparison.Ordinal) >= 0),
            "StaticConfigRegistry 不得新增 public 测试入口");
#if !UNITY_EDITOR
        Assert.Fail("双槽 Gateway 契约依赖 Editor-only internal 测试缝，不能在 Player 编译运行。");
#endif
    }

    // ---------------- helpers ----------------
    private static object Call(string name, params object[] args) => Bridge.GetMethod(name, S).Invoke(null, args);

    private static object ActivityCall(string name, params object[] args)
        => ActivityHost.GetMethod(name, S).Invoke(null, args);

    /// <summary>
    /// 用真实 Activity Runtime 建立 claim；不伪造 QueryClaim 字符串，也不镜像劳动判断。
    /// 夹具前置：本用例已把机会点排到推进区间之外，故此时不应存在任何当日窗口；
    /// 若已存在同日窗口（例如生产默认的 420 分钟晨间机会点），OpenDailyWindow 会按既有契约
    /// 返回 WindowConflict（同一 DayStamp 只允许一个快照），这是正确的领域行为，不是本用例要测的语义。
    /// OpenDailyWindow 只接受非日内（OpportunityId 为 null）的日窗口：带 OpportunityId/Minute 的
    /// 5 参构造会使 IsIntraday=true，服务在 OpenDailyWindow 入口直接返回 InvalidWindow。
    /// Rest 在这里是普通（非排程）活动，不需要日内机会点，因此用 3 参构造的真实日窗口。
    /// </summary>
    private static void CommitRestActivity(string personId)
    {
        Assert.IsNull(ActivityCall("QueryCurrentWindow"),
            "夹具推进区间内不得已有当日活动窗口（否则同日开窗按契约返回 WindowConflict）");
        object day = Activator.CreateInstance(ActivityDay, Clock.Year, Clock.Month, Clock.Day);
        string id = "management:rest:" + personId;
        object window = Activator.CreateInstance(ActivityWindow, day, new[] { personId }, new[] { personId });
        Assert.AreEqual("Success", ActivityCall("OpenDailyWindow", window).ToString());
        object candidate = Activator.CreateInstance(ActivityCandidate, id, "Rest", "management-tests", personId,
            null, "management:test", null, Enum.ToObject(ActivityParticipation, 0));
        Assert.AreEqual("Success", ActivityCall("TryCommitActivity", id, candidate).ToString());
    }

    private static object Static(string name)
    {
        var field = Bridge.GetField(name, S);
        return field != null ? field.GetValue(null) : Bridge.GetProperty(name, S).GetValue(null);
    }

    private static ManagementSnapshot Read()
        => (ManagementSnapshot)Adapter.GetMethod("Read", S, null, Type.EmptyTypes, null).Invoke(null, null);

    private static ManagementSnapshot Read(BuildingRegistry definitions)
        => (ManagementSnapshot)Adapter.GetMethod("Read", S, null, new[] { typeof(BuildingRegistry) }, null)
            .Invoke(null, new object[] { definitions });

    private static ManagementCommandResult Command(string name, params object[] args)
        => (ManagementCommandResult)Gateway.GetMethod(name, S).Invoke(null, args);

    private static VillageTime Clock => (VillageTime)Static("Time");
    private static VillageAssignmentSystem Assignments => (VillageAssignmentSystem)Static("Assignments");
    private static VillageProjectSystem Projects => (VillageProjectSystem)Static("Projects");
    private static VillageIntradayWork Work => (VillageIntradayWork)Static("IntradayWork");
    private static VillageResourceState Resources => (VillageResourceState)Static("Resources");
    private static VillageWorldRegistry World() => (VillageWorldRegistry)Static("World");

    private static void ForceSaveFailure(bool enabled)
    {
        var field = Type.GetType("SaveSystem, Assembly-CSharp", true).GetField("ForceSaveFailureForTests", S);
        field.SetValue(null, enabled ? new Func<bool>(() => true) : null);
    }

    private static string SaveJson() => JsonUtility.ToJson(Call("BuildSaveData"));

    private static bool CanSaveWorld => (bool)Bridge.GetProperty("CanSaveWorld", S).GetValue(null);

    /// <summary>Fixture：写既有长期人物字段 IsDead（与 BuildPersonInputs 的真实读取链一致）。</summary>
    private void SetDead(string personId, bool dead)
    {
        var characters = (System.Collections.IList)_player.GetType().GetField("OwnedCharacters").GetValue(_player);
        foreach (object person in characters)
        {
            if (person == null) continue;
            if ((string)person.GetType().GetField("PersonId").GetValue(person) != personId) continue;
            person.GetType().GetField("IsDead").SetValue(person, dead);
            return;
        }
        Assert.Fail("fixture 中找不到人物：" + personId);
    }

    private static object BuildSameNamePlayer(params string[] personIds)
    {
        object player = Activator.CreateInstance(Type.GetType("PlayerCollection, Assembly-CSharp"));
        var characterType = Type.GetType("CharacterInstance, Assembly-CSharp");
        var characters = (System.Collections.IList)player.GetType().GetField("OwnedCharacters").GetValue(player);
        foreach (string id in personIds)
        {
            object person = Activator.CreateInstance(characterType, new object[] { "char_001", "林恩", 30, 6 });
            characterType.GetField("PersonId").SetValue(person, id);
            characters.Add(person);
        }
        return player;
    }

    private static object Get(object value, string name) => value.GetType().GetProperty(name, I).GetValue(value);

    /// <summary>直接写长期人物饥饿字段（测试 fixture；只用于验证投影读取真实事实）。</summary>
    private void SetNutritionFacts(int index, bool hungry, int consecutiveUnderfedDays)
    {
        var characters = (System.Collections.IList)_player.GetType().GetField("OwnedCharacters").GetValue(_player);
        object person = characters[index];
        person.GetType().GetProperty("Hungry", I).SetValue(person, hungry);
        person.GetType().GetProperty("ConsecutiveUnderfedDays", I).SetValue(person, consecutiveUnderfedDays);
    }
    private static string Id(object building) => (string)Get(building, "InstanceId");
    private static string Workplace(object building, string slot) => "workplace:" + Id(building) + ":" + slot;

    private static object Place(string definitionId, int index)
    {
        object[] args = { definitionId, 4f + index * 4f, -4f, null, null };
        Assert.IsTrue((bool)Call("TryPlaceBuilding", args), args[4]?.ToString());
        return args[3];
    }

    /// <summary>放置并确定性完成一座建筑（不依赖真实自然日推进）。</summary>
    private object Complete(string definitionId, int index)
    {
        var building = Place(definitionId, index);
        Assert.AreEqual(VillageProjectResult.Success,
            (VillageProjectResult)Call("TryAddConstructionWorker", Id(building), "person_alpha"));
        var project = Projects.GetActiveProject((string)Get(building, "ConstructionProjectId"));
        project.Progress = 10;
        Projects.OnDayTick();
        Call("ObserveProjectCompletions");
        Assert.AreEqual(BuildingLifecycleState.Completed, Get(building, "LifecycleState"));
        return building;
    }

    /// <summary>
    /// 真实推进世界时钟完成施工（不手工写 Completed / assignment / arrival）：
    /// place → 每名工人经产品 Gateway 加入施工 → 推进到每个工作日结束，直到 FinalizeConstruction 完工。
    /// prj_test_01 = 10 进度、每人每日 1 进度；两名工人 → 5 个工作日。
    /// </summary>
    private object CompleteByAdvance(string definitionId, int index, params string[] workerPersonIds)
    {
        var building = Place(definitionId, index);
        foreach (string personId in workerPersonIds)
        {
            var added = Command("AddConstructionWorker", Id(building), personId);
            Assert.AreEqual(ManagementCommandOutcome.Success, added.Outcome, added.ToString());
        }

        var project = Projects.GetActiveProject((string)Get(building, "ConstructionProjectId"));
        Assert.IsNotNull(project);
        Assert.AreEqual(workerPersonIds.Length, project.ParticipantPersonIds.Count,
            "前置：施工参与者必须来自真实加入命令");

        int day = 1;
        while (Get(building, "LifecycleState").ToString() != "Completed" && day <= 12)
        {
            Advance(day * 1440L + 1021L);
            day++;
        }

        Assert.AreEqual(BuildingLifecycleState.Completed, Get(building, "LifecycleState"),
            "真实推进后施工必须完成（实际 progress=" + project.Progress + "）");
        Assert.IsNull((string)Get(building, "ConstructionProjectId"), "完工后不得保留施工项目绑定");
        return building;
    }

    /// <summary>反射调用 Editor-only internal 测试缝：读取当前建筑注册表覆盖（不反射写私有字段）。</summary>
    private static BuildingRegistry BuildingsOverrideForTests()
    {
        var method = StaticConfigApi.GetMethod("GetBuildingsOverrideForTests", S);
        Assert.IsNotNull(method, "Editor 编译下必须存在 internal GetBuildingsOverrideForTests（ForTests 命名）");
        return (BuildingRegistry)method.Invoke(null, null);
    }

    /// <summary>反射调用 Editor-only internal 测试缝：注入 / 清除内存建筑注册表覆盖。</summary>
    private static void SetBuildingsOverrideForTests(BuildingRegistry registry)
    {
        var method = StaticConfigApi.GetMethod("SetBuildingsOverrideForTests", S);
        Assert.IsNotNull(method, "Editor 编译下必须存在 internal SetBuildingsOverrideForTests（ForTests 命名）");
        method.Invoke(null, new object[] { registry });
    }

    /// <summary>建筑注册表内容指纹（用于 Reset 后默认内容与开工相同的比较，不依赖引用身份）。</summary>
    private static string DescribeBuildingRegistry(BuildingRegistry registry)
    {
        if (registry == null) return "<null>";
        return string.Join(";", registry.All.Select(d => d.BuildingDefinitionId + "/" + d.DisplayName + "/"
                + d.Category + "/" + d.FootprintWidth + "x" + d.FootprintDepth))
            + "||" + string.Join(";", registry.FunctionBindings.Select(f => f.BuildingDefinitionId + ":"
                + f.FunctionSlotId + ":" + f.FunctionType + ":" + f.FunctionProfileId));
    }

    /// <summary>其他领域注册结果指纹：只读现有入口，用于证明 Reset 只清建筑覆盖、没有改动其他领域。</summary>
    private static string CaptureOtherDomainFacts()
    {
        var parts = new List<string>();
        parts.Add(StaticConfigRegistry.TryGetMonsters(out MonsterRegistry monsters, out string monstersDiagnostic)
            ? "monsters=" + monsters.Count : "monsters=unavailable:" + monstersDiagnostic);
        parts.Add(StaticConfigRegistry.TryGetEnemyPresentation(out EnemyPresentationDatabase presentation, out _)
            ? "presentation=" + presentation.Count : "presentation=unavailable");
        parts.Add(StaticConfigRegistry.TryGetVisualAssets(out VisualAssetCatalog visualAssets, out _)
            ? "visual=" + visualAssets.Count : "visual=unavailable");
        parts.Add(StaticConfigRegistry.TryGetEncounterConfig(out EncounterConfigDatabase encounter, out _)
            ? "encounter=" + encounter.TotalCount : "encounter=unavailable");
        parts.Add(StaticConfigRegistry.TryGetEventConfig(out EventConfigDatabase events, out _)
            ? "event=" + events.TotalCount : "event=unavailable");
        parts.Add("autonomous=" + StaticConfigRegistry.TryGetAutonomousLifeConfig(out _, out _));
        parts.Add("social=" + StaticConfigRegistry.TryGetSocialConfig(out _, out _));
        parts.Add("village=" + StaticConfigRegistry.TryGetVillagePresentation(out _, out _));
        return string.Join("|", parts);
    }

    private static BuildingRegistry RegistryWithSecondSlot()
    {
        Assert.IsTrue(StaticConfigRegistry.TryGetBuildings(out BuildingRegistry definitions, out string diagnostic), diagnostic);
        var bindings = definitions.FunctionBindings.Concat(new[]
        {
            new BuildingFunctionBinding
            {
                BuildingDefinitionId = "building_farm",
                FunctionSlotId = "aux",
                FunctionType = "Workplace",
                FunctionProfileId = "ws_farm_01"
            }
        });
        return new BuildingRegistry(definitions.All, bindings);
    }

    private void Advance(long minute)
    {
        var result = Clock.AdvanceTo(new WorldInstant(minute));
        int attempts = 0;
        while (result.Outcome == WorldAdvanceOutcome.Yielded && attempts++ < 100)
            result = Clock.ResumeAdvance(Clock.AdvanceSession.RequestId);
        Assert.AreEqual(WorldAdvanceOutcome.Completed, result.Outcome, result.Diagnostic);
    }

    private static int EvidenceMinutes(string workplaceId)
        => Work.ToSaveData().Evidence.Where(e => e.WorkplaceId == workplaceId).Sum(e => e.Minutes);

    private static List<VillageWorkArrivalSaveData> Arrivals() => Work.ToSaveData().Arrivals.ToList();
}
}
