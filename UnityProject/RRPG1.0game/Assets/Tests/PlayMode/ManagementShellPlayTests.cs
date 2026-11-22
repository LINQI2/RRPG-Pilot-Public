using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace RRPG.ManagementShell.Tests
{
/// <summary>
/// V17-R3-B 真实产品入口 / 集中管理模态 Play 专项（长期回归 17 例）。
/// 本包新增第 17 例“用户完整路径”：从世界无选人打开管理 → 真实 EventSystem 点击人口行只选人并留在管理
/// （旧“单击立即进入 Person Surface”的期望已修订）→ 切职责页选岗位 / 施工目标（人物保留）→
/// 派工 / 加入施工 / 撤出施工 / 撤职逐项按钮 enabled + 真实指针点击（top-hit）+ 真实领域回执，
/// 施工期间 Primary Assignment 保持 → 独立 authored“查看人物详情”入口正常导航。
/// 本包新增：来源切换（Continue / Return Load）后的首次真实导航回归——真实 Load 替换来源后
/// 不提前刷新、不等待周期刷新，第一次真实“管理”点击必须被提交（不被 Shell 来源复位吞掉）、
/// 绑定新来源并清除旧候选；同时保留一次周期刷新路径，证明来源复位本身仍有效。
/// 本包新增：CASE-B 施工撤出后的项目计量边界（剩余参与者继续贡献 + 重新加入需新通勤到场）、
/// 劳动中撤职的下一区间证据停止与旧 arrival 清理时序；CASE-B 另加静态兼容岗位的最小 UI 事实断言。
/// R3-B surface-equivalence 追加 1 例：World / Person / Management 三表面在同一真实初始世界、
/// 同一绝对目标分钟下的 Clock/Work/Activity/Social/Resource/职责施工事实等价，并各自独立证明
/// 表面打开时 IsSimulationActive 保持且 host 真实帧推进（自然帧与确定性 Advance 两段证据分开）。
/// pending-event-management-gate 追加 1 例：由既有正式 Recognition 触发条件（第 3 次真实临时承诺）
/// 建立统一事件 Pending，证明 pending 阻断 Direct Advance 与 Return Catch-up 时管理页开关 / ESC /
/// 手动 Pause 不绕过、命令与保存按既有 WorldBusy 规则拒绝零写，合法选项解决后只完成一次原推进/结算。
///
/// 入口基座复用既有 ProductHome / Person Play 夹具（R3A / ProductHomeFoundation）：
///   SampleScene → MainMenuUI.OnContinue → SaveSystem.Load → TownUI.Initialize
///   → ProductHomeWorldHost.Awake → HomeProductShell.Initialize → 真实 authored Prefab。
/// 不 SetActive 代替产品初始化，不新建第二世界，不 mock Canvas，不反射写私有核心状态。
///
/// 输入说明：本文件使用 EventSystem.RaycastAll + ExecuteEvents.PointerClick 的程序化点击，
/// 验证“命中的是真实 Button 并走 Button.onClick 事件”；它不等价物理鼠标人工体验，
/// 指针手感 / 拖拽 / 镜头滚轮仍留后续专项或人工验收。
/// ESC 帧轮询专项追加 1 例：沿用 HomeWorldCamera 的 Editor-only internal 实例级采样先例，
/// 只替换 Shell 的 ESC 采样数据（一次性 press，消费后回落生产 Input.GetKeyDown），
/// 由真实 LateUpdate 执行产品路由与 Build.IsOpen 门禁；它不是 OS 物理 ESC，也不证明引擎按键实现。
///
/// 保护策略：
///   - 每个用例前只备份当前 Editor persistentDataPath 下 SaveSystem 唯一槽位的实际字节与 SHA256，
///     并额外把该字节持久化到 Development/StudioRuns/R3B-SaveProtection-20261024/&lt;runId&gt;/
///     original-save.bin + manifest.json（回读 SHA256 一致才允许继续）；用例后（TearDown，try/finally）
///     以及 UnitySetUp 已 Capture 后的异常路径（try/finally）都只按本 runID 已验备份恢复并回读校验；
///     原本不存在则只删除本轮创建的档。
///   - 反射调用按“名称 + 实际参数签名”选唯一重载（SaveSystem.NewGame() / NewGame(config) 并存），
///     禁止再用 GetMethod(name, flags) 的隐式重载选择。
///   - 存在未完成 run（status 非 restored / protective-snapshot）时阻止新测试，交人工核对。
///   - 施工计量夹具（CASE-B）只在本例时间跨度内使用既有公开注入点 ConfigureActivitySchedule
///     排除自主活动机会点（不改产品规则），并在 TearDown 的 ResetForTests 释放世界写锁之后
///     用同一实例恢复 + 只读校验（ConfigureActivitySchedule 有 GuardWorldWrite，锁未释放会被契约拒绝）；
///     其它用例仍跑真实活动机会（不全局禁用活动）。
///   - OneTimeSetUp 记录受保护字体 SHA256 与 34 项 workspace-baseline 的实测 SHA256；
///     OneTimeTearDown 重算并断言与基线完全一致，结果写入
///     Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log
///     （路径由 Application.dataPath 推出仓库根，不访问仓库外目录）。
/// </summary>
public sealed class ManagementShellPlayTests
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private const string BaselineRelative = "Development/StudioRuns/R3B-20261022/workspace-baseline.json";
    private const string EvidenceRelative = "Development/StudioRuns/R3B-UI-20261023/play-evidence/management-shell-play-evidence.log";
    private const string ProtectedFontRelative = "UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset";

    private object _town, _host, _shell, _player;
    /// <summary>施工计量夹具隔离前捕获的真实活动日程（TearDown 恢复后置空）。</summary>
    private object _activityScheduleBefore;
    private SaveSlotGuard _saveGuard;
    private static SaveSlotGuard _lastGuard;
    private static Dictionary<string, string> _baselineBefore;
    private static string _fontBefore;

    // ==================== 反射公共通道（仅读取 + 调用真实产品方法） ====================

    private static Type T(string name) => Type.GetType(name + ", Assembly-CSharp", true);

    /// <summary>
    /// 跨运行时装配类型解析：Village.Runtime 等 asmdef 类型不在 Assembly-CSharp 中，
    /// 测试程序集只引用测试框架，因此按已加载装配查找同名类型（不新增产品引用）。
    /// </summary>
    private static Type AnyType(string name)
    {
        var type = Type.GetType(name + ", Assembly-CSharp", false) ?? Type.GetType(name, false);
        if (type != null) return type;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(name, false);
            if (type != null) return type;
        }
        Assert.Fail("类型不存在：" + name);
        return null;
    }

    private static object Get(object instance, string name)
    {
        var type = instance.GetType();
        var field = type.GetField(name, Flags);
        if (field != null) return field.GetValue(instance);
        var property = type.GetProperty(name, Flags);
        Assert.IsNotNull(property, "成员不存在：" + name + " @ " + type.FullName);
        return property.GetValue(instance, null);
    }

    private static object Static(string typeName, string method, params object[] args)
    {
        var methodInfo = ResolveMethod(T(typeName), typeName, method, args ?? Array.Empty<object>(), true);
        return methodInfo.Invoke(null, args);
    }

    /// <summary>
    /// 按“名称 + 实际参数签名”选择唯一重载，禁止再用 GetMethod(name, flags)：
    /// SaveSystem 同时存在 NewGame() 与 NewGame(InitialWorldConfiguration)，
    /// 旧写法在 SetUp 初始化入口抛 AmbiguousMatchException，导致 Capture 后未走到 Save。
    /// 先按参数个数过滤，再按参数类型兼容性 / 精确类型唯一化；无法唯一确定时带候选签名失败，
    /// 不臆选重载、不修改产品 API。
    /// </summary>
    private static MethodInfo ResolveMethod(Type type, string owner, string method, object[] args, bool requireStatic)
    {
        var named = type.GetMethods(Flags)
            .Where(info => info.Name == method && info.IsStatic == requireStatic)
            .ToList();
        Assert.IsTrue(named.Count > 0, "方法不存在：" + owner + "." + method
            + "（requireStatic=" + requireStatic + "）");
        var arity = named.Where(info => info.GetParameters().Length == args.Length).ToList();
        Assert.IsTrue(arity.Count > 0, "方法参数个数不匹配：" + owner + "." + method + "/" + args.Length
            + "，可用重载：" + DescribeMethods(named));
        var compatible = arity.Where(info => ParametersCompatible(info.GetParameters(), args)).ToList();
        if (compatible.Count == 1) return compatible[0];
        if (compatible.Count > 1)
        {
            var exact = compatible.Where(info => ParametersExact(info.GetParameters(), args)).ToList();
            if (exact.Count == 1) return exact[0];
            Assert.Fail("反射方法重载不唯一（显式参数签名无法唯一确定）：" + owner + "." + method
                + "(" + DescribeArgs(args) + ")；候选："
                + DescribeMethods(exact.Count > 0 ? exact : compatible));
        }
        Assert.Fail("反射方法没有匹配参数类型的重载：" + owner + "." + method
            + "(" + DescribeArgs(args) + ")；候选：" + DescribeMethods(arity));
        return null;
    }

    private static bool ParametersCompatible(ParameterInfo[] parameters, object[] args)
    {
        if (parameters.Length != args.Length) return false;
        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameter = parameters[i].ParameterType;
            if (parameter.IsByRef) parameter = parameter.GetElementType();
            object arg = args[i];
            if (arg == null)
            {
                if (parameter != null && parameter.IsValueType && System.Nullable.GetUnderlyingType(parameter) == null)
                    return false;
                continue;
            }
            if (parameter != null && parameter.IsInstanceOfType(arg)) continue;
            // 反射 Invoke 支持的数值拓宽（如 int → long / float → double）；枚举不在此处隐转。
            if (parameter != null && !parameter.IsEnum && arg is IConvertible
                && (parameter.IsPrimitive || parameter == typeof(decimal)))
            {
                try { Convert.ChangeType(arg, parameter); continue; }
                catch (Exception) { return false; }
            }
            return false;
        }
        return true;
    }

    private static bool ParametersExact(ParameterInfo[] parameters, object[] args)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameter = parameters[i].ParameterType;
            if (parameter.IsByRef) parameter = parameter.GetElementType();
            object arg = args[i];
            if (arg == null)
            {
                if (parameter != null && parameter.IsValueType && System.Nullable.GetUnderlyingType(parameter) == null)
                    return false;
                continue;
            }
            if (parameter == null || arg.GetType() != parameter) return false;
        }
        return true;
    }

    private static string DescribeArgs(object[] args)
        => args == null || args.Length == 0
            ? string.Empty
            : string.Join(", ", args.Select(arg => arg == null ? "<null>" : arg.GetType().Name));

    private static string DescribeMethods(IEnumerable<MethodInfo> methods)
        => string.Join("; ", methods.Select(info => info.Name + "("
            + string.Join(",", info.GetParameters().Select(parameter => parameter.ParameterType.Name))
            + ")"));

    /// <summary>按明确参数类型取唯一方法；不存在时给出显式签名失败信息（不是 AmbiguousMatchException）。</summary>
    private static MethodInfo ExactMethod(Type type, string method, BindingFlags flags, string what,
        params Type[] parameterTypes)
    {
        var info = type.GetMethod(method, flags, null, parameterTypes, null);
        Assert.IsNotNull(info, what + "：方法不存在（显式签名）" + type.FullName + "." + method
            + "(" + string.Join(",", parameterTypes.Select(parameter => parameter.Name)) + ")");
        return info;
    }

    /// <summary>按明确参数类型取方法；用于“必须不存在 public 暴露”之类允许返回 null 的断言。</summary>
    private static MethodInfo MethodOrNull(Type type, string method, BindingFlags flags,
        params Type[] parameterTypes)
        => type.GetMethod(method, flags, null, parameterTypes, null);

    private static object StaticField(string typeName, string name)
    {
        var type = T(typeName);
        var field = type.GetField(name, Flags);
        if (field != null) return field.GetValue(null);
        var property = type.GetProperty(name, Flags);
        Assert.IsNotNull(property, "静态字段/属性不存在：" + typeName + "." + name);
        return property.GetValue(null, null);
    }

    private static void SetMember(object instance, string name, object value)
    {
        var type = instance.GetType();
        var field = type.GetField(name, Flags);
        if (field != null) { field.SetValue(instance, value); return; }
        var property = type.GetProperty(name, Flags);
        Assert.IsNotNull(property, "成员不存在：" + name + " @ " + type.FullName);
        property.SetValue(instance, value, null);
    }

    private static object Call(object instance, string method, params object[] args)
    {
        Assert.IsNotNull(instance, "反射调用目标不得为 null：" + method);
        var methodInfo = ResolveMethod(instance.GetType(), instance.GetType().FullName, method,
            args ?? Array.Empty<object>(), false);
        return methodInfo.Invoke(instance, args);
    }

    private static object Find(string typeName)
    {
        var type = T(typeName);
        return UnityEngine.Object.FindObjectsOfType(type, true).FirstOrDefault(o => o != null);
    }

    private static object FindOptional(string typeName)
    {
        var type = T(typeName);
        return UnityEngine.Object.FindObjectsOfType(type, true).FirstOrDefault(o => o != null);
    }

    private string Surface() => Get(Get(_shell, "Router"), "Surface").ToString();

    private static object Clock() => StaticField("VillageRuntimeBridge", "Time");

    private static string ClockSpeed() => Get(Clock(), "Speed").ToString();

    private static double ClockMultiplier() => Convert.ToDouble(Get(Clock(), "SpeedMultiplier"));

    private static bool SimulationActive() => (bool)StaticField("VillageRuntimeBridge", "IsSimulationActive");

    private string SelectionId() => (string)Get(Get(_host, "Selection"), "Id");

    private static Transform FindPath(Transform root, string path)
    {
        var current = root;
        foreach (var part in path.Split('/'))
        {
            if (current == null) return null;
            current = current.Find(part);
        }
        return current;
    }

    private static Array SerializedArray(object view, string fieldName)
    {
        var value = Get(view, fieldName) as Array;
        Assert.IsNotNull(value, "序列化数组不存在：" + fieldName);
        return value;
    }

    /// <summary>
    /// 按类型名在当前已加载装配中查找运行时类型（例如 RunEndMode 位于 BattleStage1.Runtime）。
    /// 只读类型解析；不写入产品状态。
    /// </summary>
    private static Type LoadedType(string name)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly == null) continue;
            var type = assembly.GetType(name, false);
            if (type != null) return type;
        }
        Assert.Fail("运行时类型不存在：" + name);
        return null;
    }

    /// <summary>
    /// 按“名称 + 参数个数”选择唯一静态重载（用于 BeginExpeditionReturn 这类既有重载方法），
    /// 不写私有状态、不新增产品 API。
    /// </summary>
    private static object StaticByArgs(string typeName, string method, params object[] args)
    {
        var candidates = T(typeName).GetMethods(Flags)
            .Where(info => info.Name == method && info.GetParameters().Length == args.Length).ToList();
        Assert.AreEqual(1, candidates.Count,
            "静态重载必须唯一：" + typeName + "." + method + "/" + args.Length + " 实际=" + candidates.Count);
        return candidates[0].Invoke(null, args);
    }

    /// <summary>只读管理投影中某位人物行（真实 Authority 的只读读模型）。</summary>
    private static object ProjectionPerson(string personId)
    {
        object snapshot = ReadManagementProjection();
        Assert.IsNotNull(snapshot, "ManagementReadAdapter.Read() 必须返回投影");
        return ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "PersonId"), personId, StringComparison.Ordinal));
    }

    /// <summary>只读管理投影中某建筑实例的真实功能岗位 ID（仅 Completed 建筑有行）。</summary>
    private static string WorkplaceIdFromProjection(string buildingId)
    {
        object snapshot = ReadManagementProjection();
        Assert.IsNotNull(snapshot, "ManagementReadAdapter.Read() 必须返回投影");
        object row = ((IEnumerable)Get(snapshot, "Workplaces")).Cast<object>()
            .FirstOrDefault(w => string.Equals((string)Get(w, "BuildingInstanceId"), buildingId, StringComparison.Ordinal));
        Assert.IsNotNull(row, "管理投影必须包含建筑 " + buildingId + " 的真实功能岗位行");
        return (string)Get(row, "WorkplaceId");
    }

    private static string CommandOutcome(object result)
    {
        Assert.IsNotNull(result, "ManagementCommandResult 不得为 null");
        return Get(result, "Outcome").ToString();
    }

    // ==================== 真实 Pointer / Raycast 工具（程序化点击，非物理鼠标） ====================

    private static Vector2 ScreenCenter(RectTransform rect)
    {
        var canvas = rect.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center));
    }

    private static Vector2 ScreenPointAt(RectTransform rect, Vector2 localPoint)
    {
        var canvas = rect.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(localPoint));
    }

    private static List<RaycastResult> RaycastAllAt(Vector2 screenPoint)
    {
        var eventSystem = EventSystem.current;
        Assert.IsNotNull(eventSystem, "场景必须存在 EventSystem（真实 Raycast 前提）");
        var data = new PointerEventData(eventSystem) { position = screenPoint, button = PointerEventData.InputButton.Left, clickCount = 1 };
        var hits = new List<RaycastResult>();
        eventSystem.RaycastAll(data, hits);
        return hits;
    }

    private static string DumpHits(List<RaycastResult> hits)
    {
        if (hits == null || hits.Count == 0) return "（无 Raycast 命中）";
        var builder = new StringBuilder();
        for (int i = 0; i < hits.Count; i++)
        {
            var go = hits[i].gameObject;
            var button = go != null ? go.GetComponentInParent<Button>() : null;
            var canvas = go != null ? go.GetComponentInParent<Canvas>() : null;
            builder.AppendLine(i + ". " + (go != null ? go.name : "<null>")
                + " | Canvas=" + (canvas != null ? canvas.name + "#" + canvas.sortingOrder : "null")
                + " | Button=" + (button != null ? button.name : "null")
                + " | active=" + (go != null && go.activeInHierarchy));
        }
        return builder.ToString();
    }

    private static void AssertTopHitBelongs(RectTransform target, string what, Vector2 point, List<RaycastResult> hits)
    {
        Assert.Greater(hits.Count, 0, what + "：Raycast 无命中 @ " + point);
        var top = hits[0].gameObject;
        bool belongs = top == target.gameObject || top.transform.IsChildOf(target.transform);
        Assert.IsTrue(belongs, what + "：top hit 不是目标（玩家点到的不是它）\n" + DumpHits(hits));
    }

    private static void AssertTopHitInside(Transform container, string what, Vector2 point, List<RaycastResult> hits)
    {
        Assert.Greater(hits.Count, 0, what + "：Raycast 无命中 @ " + point);
        var top = hits[0].gameObject;
        Assert.IsTrue(top == container.gameObject || top.transform.IsChildOf(container),
            what + "：top hit 不属于预期模态容器\n" + DumpHits(hits));
    }

    private static void RealPointerClick(RectTransform target, string what)
    {
        Vector2 point = ScreenCenter(target);
        var hits = RaycastAllAt(point);
        AssertTopHitBelongs(target, what, point, hits);
        var eventSystem = EventSystem.current;
        var data = new PointerEventData(eventSystem)
        {
            position = point,
            button = PointerEventData.InputButton.Left,
            clickCount = 1
        };
        var handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
        Assert.IsNotNull(handler, what + "：top hit 父链没有 IPointerClickHandler（真实点击不会触发按钮）\n" + DumpHits(hits));
        Assert.IsTrue(ExecuteEvents.Execute(handler, data, ExecuteEvents.pointerClickHandler),
            what + "：PointerClick 未被 handler 处理");
        Debug.Log("[ManagementShellPlayTests] 程序化 EventSystem 点击（非物理鼠标人工体验）：" + what);
    }

    private static void ClickChild(object root, string path, string what)
    {
        var transform = FindPath(((Component)root).transform, path);
        Assert.IsNotNull(transform, what + "：子控件不存在 " + path);
        var button = transform.GetComponent<Button>();
        Assert.IsNotNull(button, what + "：子控件不是 Button " + path);
        Assert.IsTrue(button.interactable, what + "：Button 不可交互");
        RealPointerClick((RectTransform)transform, what);
    }

    // ==================== 管理命令 / 建造 / 世界推进辅助（真实产品公开入口） ====================

    private static object Bridge(string method, params object[] args)
        => Static("VillageRuntimeBridge", method, args);

    /// <summary>
    /// CASE-B 的“静态兼容岗位”最小 UI 事实夹具：正式默认新世界只有实例岗位（0 静态岗位），
    /// 因此通过既有公开兼容通道**显式追加** ws_farm_01 定义 / Place / 资源职责（测试专用配置），
    /// 不把正式默认世界改回 0 建筑或全量静态目录；其它用例仍共享正式默认实例世界。
    /// </summary>
    private static void RegisterLegacyFarmWorkplaceForFixture()
    {
        Type defType = AnyType("VillageWorkplaceDef");
        object def = Activator.CreateInstance(defType);
        SetMember(def, "WorkplaceId", "ws_farm_01");
        SetMember(def, "DisplayName", "Farm（Test / Not Frozen）");
        SetMember(def, "Capacity", 2);
        SetMember(def, "TestContributionPerDay", 1);
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(defType));
        list.Add(def);
        Static("VillageRuntimeBridge", "EnsureInitializedWithExtraWorkplaces", null, list);
        object assignments = StaticField("VillageRuntimeBridge", "Assignments");
        Assert.IsNotNull(assignments, "显式追加静态岗位后 Assignments 必须存在");
        Assert.IsNotNull(Call(assignments, "GetWorkplaceDef", "ws_farm_01"),
            "显式追加的静态兼容岗位必须真实注册");
        Evidence("CASE-B", "FIXTURE legacy workplace ws_farm_01 registered explicitly (test-only compatibility;"
            + " formal default world stays instance-only with real buildings)");
    }

    /// <summary>当前正式实例世界中第一座 Completed Farm 的实例岗位（用于旧“独立静态岗位候选”夹具）。</summary>
    private string FirstInstanceFarmWorkplace(out string buildingId)
    {
        if (Get(_host, "Snapshot") == null) Call(_host, "RefreshNow");
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "产品世界快照必须已建立");
        foreach (object b in ((IEnumerable)Get(snapshot, "Buildings")).Cast<object>())
        {
            if ((string)Get(b, "DefinitionId") != "building_farm") continue;
            if (Get(b, "LifecycleState").ToString() != "Completed") continue;
            buildingId = (string)Get(b, "InstanceId");
            return "workplace:" + buildingId + ":main";
        }
        Assert.Fail("正式默认世界必须存在 Completed Farm 实例");
        buildingId = null;
        return null;
    }

    private static string PersonWorkplace(string personId)
        => (string)Bridge("GetPersonWorkplaceId", personId);

    private static int WorkplaceOccupants(string workplaceId)
        => Convert.ToInt32(Bridge("GetWorkplaceOccupantCount", workplaceId));

    private static List<string> Strings(object value)
        => value == null
            ? new List<string>()
            : ((IEnumerable)value).Cast<object>().Select(element => element?.ToString()).ToList();

    private static object ActiveProject()
    {
        object projects = StaticField("VillageRuntimeBridge", "Projects");
        Assert.IsNotNull(projects, "VillageRuntimeBridge.Projects 必须存在");
        return ((IEnumerable)Get(projects, "ActiveProjects")).Cast<object>().FirstOrDefault();
    }

    private string SnapshotPersonIdAt(int index)
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        var persons = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>().ToList();
        Assert.Greater(persons.Count, index, "夹具需要至少 " + (index + 1) + " 名真实人物");
        return (string)Get(persons[index], "PersonId");
    }

    private string BuildingState(string buildingId)
    {
        object building = Call(Get(_host, "Snapshot"), "Building", buildingId);
        Assert.IsNotNull(building, "世界快照必须包含建筑 " + buildingId);
        return Get(building, "LifecycleState").ToString();
    }

    private string PersonPlace(string personId)
    {
        object person = Call(Get(_host, "Snapshot"), "Person", personId);
        Assert.IsNotNull(person, "世界快照必须包含人物 " + personId);
        return (string)Get(person, "CurrentPlaceId");
    }

    /// <summary>读取当前真实工作到场登记（只读生产状态；不创建、不改写）。</summary>
    private static object WorkArrival(string personId)
    {
        object work = StaticField("VillageRuntimeBridge", "IntradayWork");
        Assert.IsNotNull(work, "VillageRuntimeBridge.IntradayWork 必须存在");
        object data = Call(work, "ToSaveData");
        Assert.IsNotNull(data, "IntradayWork.ToSaveData 必须返回真实存档投影");
        return ((IEnumerable)Get(data, "Arrivals")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "PersonId"), personId, StringComparison.Ordinal));
    }

    private static string WorkExecution(string personId)
        => (string)Bridge("GetWorkExecutionLabel", personId);

    /// <summary>
    /// 真实 Activity claim（只读）：非 null 表示该人物当前被自主活动占用。
    /// 生产规则下 RefreshWorkIntent → ArrivedWorkers 会跳过被 claim 的人物（活动优先于工作到场登记），
    /// 因此它是“加入施工后是否登记到场计划”的真实前置，不是测试私有状态。
    /// </summary>
    private static string ActivityClaim(string personId)
        => (string)Static("ActivityRuntimeBridge", "QueryClaim", personId);

    /// <summary>当前真实活动日程（VillageRuntimeBridge 唯一公开 getter；不反射私有状态）。</summary>
    private static object ActivitySchedule()
        => StaticField("VillageRuntimeBridge", "ActivitySchedule");

    private static List<int> OpportunityPoints(object schedule)
        => ((IEnumerable)Get(schedule, "OpportunityMinutes")).Cast<object>().Select(Convert.ToInt32).ToList();

    /// <summary>只读记录活动时长（用于证明隔离日程与产品默认时长口径一致，不写任何状态）。</summary>
    private static string ActivityDurationSnapshot(object schedule)
    {
        var parts = new List<string>();
        foreach (var key in new[] { "Rest", "Drink", "Talk", "VisitSick", "Help" })
        {
            try { parts.Add(key + "=" + Call(schedule, "Duration", key)); }
            catch (Exception ex)
            {
                var cause = ex.InnerException ?? ex;
                parts.Add(key + "=<" + cause.GetType().Name + ">");
            }
        }
        return string.Join(",", parts);
    }

    /// <summary>
    /// 施工计量夹具“活动窗口所有权”隔离（只走既有公开注入点 VillageRuntimeBridge.ConfigureActivitySchedule，
    /// 不新增产品 API、不改产品活动规则、不反射写私有状态）：
    /// 生产默认机会点为 420/1020/1200。本用例在 Clock 暂停下从当日 0 分钟真实推进到约 816 分钟，
    /// 420 的真实自主活动机会会为当时在场的多数人物建立 Activity claim（真实回执：6 人里 5 人被占），
    /// 而生产规则是“自主活动优先于工作到场登记”，于是“至少 2 名未被占用且无既有职责的自由候选”
    /// 变成随机结果——这不是施工业务断言失败，是夹具前置的随机活动干扰。
    /// 这里排除本例时间跨度（当日当前分钟之后）内的机会点，使本例成为该窗口唯一的活动建立者；
    /// 跨整日时机会点为空集（空集是 IntradayActivitySchedule 的合法输入，构造器只拒绝 &lt;0 或 &gt;=1440 的分钟）。
    /// 原日程引用被捕获，TearDown 恢复并只读校验；其它 9 例仍跑真实活动机会。
    /// </summary>
    private void IsolateActivityOpportunitiesForFixture(string what)
    {
        object before = ActivitySchedule();
        Assert.IsNotNull(before, "VillageRuntimeBridge.ActivitySchedule 必须存在");
        Assert.IsNull(_activityScheduleBefore, "同一用例不得重复捕获活动日程（上次未恢复会掩盖泄漏）");
        _activityScheduleBefore = before;

        long nowMinuteOfDay = ClockMinuteOfDay();
        int commute = Convert.ToInt32(Get(before, "CommuteMinutes"));
        int repeat = Convert.ToInt32(Get(before, "RepeatOpportunityDelay"));
        List<int> original = OpportunityPoints(before);
        // 本例只会向前推进（Clock 暂停 + AdvanceTo），因此“当日当前分钟及之后”的机会点都可能在本例触发；
        // 已过去的机会点不会再次触发，保留原值以减少与生产日程的差异。
        var kept = original.Where(minute => minute < nowMinuteOfDay).ToList();
        object isolated = Activator.CreateInstance(T("IntradayActivitySchedule"),
            new object[] { kept.ToArray(), commute, repeat, null });
        Static("VillageRuntimeBridge", "ConfigureActivitySchedule", isolated);

        object applied = ActivitySchedule();
        Assert.IsTrue(ReferenceEquals(applied, isolated), "本例活动日程必须被既有公开注入点真实替换");
        List<int> appliedPoints = OpportunityPoints(applied);
        Assert.IsTrue(appliedPoints.All(minute => minute < nowMinuteOfDay),
            "本例时间跨度内不得残留活动机会点：" + string.Join(",", appliedPoints)
            + "（当日当前分钟=" + nowMinuteOfDay + "）");
        Evidence("CASE-B-SCHEDULE", what + ": captured=" + before.GetType().Name
            + " original-points=[" + string.Join(",", original) + "]"
            + " original-durations={" + ActivityDurationSnapshot(before) + "}"
            + " commute=" + commute + " repeat=" + repeat
            + " isolated-points=[" + string.Join(",", appliedPoints) + "]"
            + " isolated-durations={" + ActivityDurationSnapshot(applied) + "}"
            + " window=minute-of-day>=" + nowMinuteOfDay
            + " (仅本用例时间跨度；其它 9 例仍跑真实活动机会)");
    }

    /// <summary>
    /// 恢复本例捕获的活动日程并只读校验（同一实例 + 机会点逐项 + 通勤分钟 + 重复机会延迟 + 五项时长）。
    /// 必须由 TearDown 在 ResetForTests 之后调用：ConfigureActivitySchedule 是带 GuardWorldWrite 的产品写入口，
    /// 本例以 Return 最终保存前的世界写锁结束；锁未释放时该入口按产品契约静默拒绝写入（既不复制也不默认化），
    /// 于是旧顺序下“恢复”实际是空操作、引用身份断言失败。写锁由 ResetForTests（Time=null、_returnRecord=null）释放。
    /// 无捕获时为幂等空操作。恢复未通过时**保留**捕获引用并抛错（不自动清记录、不掩盖泄漏），
    /// 下一个用例的 Isolate 会因“同一用例不得重复捕获 / 上次未恢复”直接失败，作为不确定状态的停止信号。
    /// 进程级 abort / 域重载不会执行这里，但那时静态日程随域一起重建，也不会泄漏到下一夹具。
    /// </summary>
    private void RestoreActivityScheduleAfterFixture()
    {
        object before = _activityScheduleBefore;
        if (before == null) return;

        object current = ActivitySchedule();
        if (!ReferenceEquals(current, before))
            Static("VillageRuntimeBridge", "ConfigureActivitySchedule", before);

        object restored = ActivitySchedule();
        bool locked = (bool)StaticField("VillageRuntimeBridge", "IsWorldWriteLocked");
        Assert.IsTrue(ReferenceEquals(restored, before),
            "活动日程必须恢复为用例前捕获的同一实例（既有 ConfigureActivitySchedule 直接赋值同一引用；"
            + "当前 IsWorldWriteLocked=" + locked + "，为 true 时产品入口按契约拒绝写入）");
        List<int> expectedPoints = OpportunityPoints(before);
        List<int> actualPoints = OpportunityPoints(restored);
        CollectionAssert.AreEqual(expectedPoints, actualPoints, "活动日程机会点必须恢复到用例前原值");
        Assert.AreEqual(Convert.ToInt32(Get(before, "CommuteMinutes")), Convert.ToInt32(Get(restored, "CommuteMinutes")),
            "活动日程通勤分钟必须恢复到用例前原值");
        Assert.AreEqual(Convert.ToInt32(Get(before, "RepeatOpportunityDelay")),
            Convert.ToInt32(Get(restored, "RepeatOpportunityDelay")),
            "活动日程重复机会延迟必须恢复到用例前原值");
        string expectedDurations = ActivityDurationSnapshot(before);
        string actualDurations = ActivityDurationSnapshot(restored);
        Assert.AreEqual(expectedDurations, actualDurations, "活动日程五项活动时长必须恢复到用例前原值");
        // 只有同一实例与全部值都通过后才清除捕获；失败时保留，供下一个用例 / 证据复核。
        _activityScheduleBefore = null;
        Evidence("SCHEDULE-RESTORE", "restored=true sameInstance=True points=[" + string.Join(",", actualPoints)
            + "] commute=" + Get(restored, "CommuteMinutes")
            + " repeat=" + Get(restored, "RepeatOpportunityDelay")
            + " durations={" + actualDurations + "}（只读校验）");
    }

    /// <summary>当前 World 快照中全部真实 PersonId（保持快照顺序，不重排、不合成）。</summary>
    private List<string> HostPersonIds()
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        return ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .Select(person => (string)Get(person, "PersonId")).ToList();
    }

    /// <summary>只读人物诊断（availability / claim / primary / execution），用于区分失败原因。</summary>
    private string PersonDiagnostic(string personId)
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        object person = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "PersonId"), personId, StringComparison.Ordinal));
        string availability = person == null ? "<missing>" : Get(person, "Availability").ToString();
        return personId + "{availability=" + availability
            + " claim=" + (ActivityClaim(personId) ?? "<null>")
            + " primary=" + (PersonWorkplace(personId) ?? "<null>")
            + " execution=" + WorkExecution(personId) + "}";
    }

    /// <summary>
    /// 只读：当前真实到场登记（PersonId → Destination@ReadyMinute），直接读 IntradayWork 权威投影。
    /// 用于区分“施工命令成功”与“工作到场计划是否真的登记”。
    /// </summary>
    private static string ArrivalDiagnostic()
    {
        object work = StaticField("VillageRuntimeBridge", "IntradayWork");
        if (work == null) return "arrivals=<no IntradayWork>";
        object data = Call(work, "ToSaveData");
        var rows = new List<string>();
        foreach (object row in (IEnumerable)Get(data, "Arrivals"))
            rows.Add((string)Get(row, "PersonId") + "→" + (string)Get(row, "Destination") + "@" + Get(row, "ReadyMinute"));
        rows.Sort(StringComparer.Ordinal);
        return "arrivals=[" + string.Join(",", rows) + "]";
    }

    /// <summary>只读施工项目诊断（参与者 / 进度 / 劳动剩余），用于比较真实命令前后状态。</summary>
    private static string ProjectDiagnostic()
    {
        object project = ActiveProject();
        if (project == null) return "project=<null>";
        return "project=" + Get(project, "ProjectId")
            + " participants=[" + string.Join(",", Strings(Get(project, "ParticipantPersonIds"))) + "]"
            + " progress=" + Get(project, "Progress")
            + " labor=" + Get(project, "LaborRemainder");
    }

    private static long ClockMinuteOfDay() => Convert.ToInt64(Get(Clock(), "MinuteOfDay"));

    /// <summary>
    /// 只读真实劳动证据：IntradayWork 权威投影中该人该岗位已累计的工作分钟。
    /// 只读生产事实，不创建 / 不改写证据；没有记录返回 0。
    /// </summary>
    private static int WorkplaceEvidenceMinutes(string personId, string workplaceId)
    {
        object data = IntradayData();
        object row = ((IEnumerable)Get(data, "Evidence")).Cast<object>()
            .FirstOrDefault(e => string.Equals((string)Get(e, "PersonId"), personId, StringComparison.Ordinal)
                && string.Equals((string)Get(e, "WorkplaceId"), workplaceId, StringComparison.Ordinal));
        return row == null ? 0 : Convert.ToInt32(Get(row, "Minutes"));
    }

    private string PersonAvailability(string personId)
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        object person = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "PersonId"), personId, StringComparison.Ordinal));
        Assert.IsNotNull(person, "世界快照必须包含人物 " + personId);
        return Get(person, "Availability").ToString();
    }

    private static object ProjectDefOf(object project)
    {
        Assert.IsNotNull(project, "活动项目必须存在");
        object projects = StaticField("VillageRuntimeBridge", "Projects");
        Assert.IsNotNull(projects, "VillageRuntimeBridge.Projects 必须存在");
        object def = Call(projects, "GetProjectDef", (string)Get(project, "ProjectDefId"));
        Assert.IsNotNull(def, "项目定义必须存在：" + Get(project, "ProjectDefId"));
        return def;
    }

    /// <summary>
    /// 只用真实项目 Def 速率 + 剩余参与者数 + 真实工作区间分钟数推导计量期望：
    /// 每名已到场参与者每工作分钟贡献 TestProgressPerParticipantPerDay，累计 480 单位 = 1 Progress。
    /// 只读推导，不写项目、不造劳动、不假设“整个项目零增长”。
    /// </summary>
    private static void AssertProjectInterval(object project, int progressBefore, long laborBefore,
        long intervalMinutes, int workerCount, string what)
    {
        int rate = Convert.ToInt32(Get(ProjectDefOf(project), "TestProgressPerParticipantPerDay"));
        long numerator = laborBefore + checked(workerCount * intervalMinutes * rate);
        int expectedProgress = progressBefore + (int)(numerator / 480L);
        long expectedLabor = numerator % 480L;
        string detail = "（期望 = 剩余参与者 " + workerCount + " 人 × " + intervalMinutes + " 分钟 × 速率 " + rate
                        + "；480 单位 = 1 Progress）";
        Assert.AreEqual(expectedProgress, Convert.ToInt32(Get(project, "Progress")),
            what + "：按真实区间与剩余参与者推导的 Progress 不符" + detail);
        Assert.AreEqual(expectedLabor, Convert.ToInt64(Get(project, "LaborRemainder")),
            what + "：按真实区间与剩余参与者推导的 LaborRemainder 不符" + detail);
    }

    private static string WorkplaceIdForBuilding(object management, string buildingId, string what)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, what + "：Management.CurrentSnapshot 必须已读取");
        object row = ((IEnumerable)Get(snapshot, "Workplaces")).Cast<object>()
            .FirstOrDefault(w => string.Equals((string)Get(w, "BuildingInstanceId"), buildingId, StringComparison.Ordinal));
        Assert.IsNotNull(row, what + "：必须存在建筑 " + buildingId + " 的真实功能岗位行");
        return (string)Get(row, "WorkplaceId");
    }

    private static object FindRow(object management, string listMember, string keyMember, string id)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
        return ((IEnumerable)Get(snapshot, listMember)).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, keyMember), id, StringComparison.Ordinal));
    }

    private void SelectForManagement(object management, string personId)
    {
        Call(Get(_host, "Selection"), "SelectPerson", personId);
        Call(management, "RefreshNow");
        Assert.AreEqual(personId, (string)Get(management, "SelectedPersonId"), "管理页必须保持同一人物选择");
    }

    private static void ShowWorkplacesPage(object management)
    {
        ClickChild(management, "ModalPanel/Tab_Workplaces", "页签·职责与场所");
        Assert.AreEqual("Workplaces", Get(management, "ActiveTab").ToString(), "真实点击页签必须切到职责与场所");
    }

    private static void ShowResourcesPage(object management)
    {
        ClickChild(management, "ModalPanel/Tab_Resources", "页签·资源");
        Assert.AreEqual("Resources", Get(management, "ActiveTab").ToString(), "真实点击页签必须切到资源");
        var page = (GameObject)Get(management, "resourcePage");
        Assert.IsNotNull(page, "resourcePage 必须绑定");
        Assert.IsTrue(page.activeSelf, "真实点击资源页签后 resourcePage 必须激活");
    }

    private static void ShowPopulationPage(object management)
    {
        ClickChild(management, "ModalPanel/Tab_Population", "页签·人口");
        Assert.AreEqual("Population", Get(management, "ActiveTab").ToString(), "真实点击页签必须切到人口");
        var page = (GameObject)Get(management, "populationPage");
        Assert.IsNotNull(page, "populationPage 必须绑定");
        Assert.IsTrue(page.activeSelf, "真实点击人口页签后 populationPage 必须激活");
    }

    private static void ClickButtonField(object view, string fieldName, string what, bool requireInteractable = true)
    {
        var button = (Button)Get(view, fieldName);
        Assert.IsNotNull(button, what + "：管理按钮绑定不得为 null");
        Assert.IsTrue(button.gameObject.activeInHierarchy, what + "：管理按钮必须可见");
        if (requireInteractable) Assert.IsTrue(button.interactable, what + "：管理按钮必须可交互");
        RealPointerClick((RectTransform)button.transform, what);
    }

    private static void AssertStatusContains(object management, string expected)
    {
        var status = (TMP_Text)Get(management, "statusMessage");
        Assert.IsNotNull(status, "管理状态文案必须绑定");
        Assert.IsTrue(status.text.Contains(expected), "状态文案必须包含“" + expected + "”，实际：" + status.text);
    }

    /// <summary>
    /// 按“真实快照领域 ID + 行容器顺序”重新解析当前目标行。
    /// 只在 Resolve 内强制一次布局；不写 normalizedPosition、不改 Selection / Surface / Clock。
    /// 每次滚动步骤后都重新调用本方法，禁止把滚动前拿到的行对象当作点击目标（Refresh 可能重建行）。
    /// </summary>
    private static RectTransform ResolveDomainRow(object management, string listMember, string keyMember,
        string id, string contentField, string what, out int rowIndex, out int rowCount)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, what + "：Management.CurrentSnapshot 必须已读取");
        var rows = ((IEnumerable)Get(snapshot, listMember)).Cast<object>().ToList();
        rowCount = rows.Count;
        rowIndex = rows.FindIndex(row => string.Equals((string)Get(row, keyMember), id, StringComparison.Ordinal));
        Assert.GreaterOrEqual(rowIndex, 0, what + "：快照必须包含 " + id);
        var content = (RectTransform)Get(management, contentField);
        Assert.IsNotNull(content, what + "：" + contentField + " 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Assert.AreEqual(rows.Count, content.childCount, what + "：行数必须等于真实快照业务行数");
        var row = content.GetChild(rowIndex) as RectTransform;
        Assert.IsNotNull(row, what + "：目标业务行必须是 RectTransform");
        Assert.IsTrue(row.gameObject.activeInHierarchy, what + "：目标业务行必须可见");
        return row;
    }

    /// <summary>只读判定指定屏幕点是否真实命中目标行（自身或子物体为 top hit）。</summary>
    private static bool TopHitBelongs(RectTransform target, Vector2 point, out List<RaycastResult> hits)
    {
        hits = RaycastAllAt(point);
        if (hits.Count == 0) return false;
        var top = hits[0].gameObject;
        return top == target.gameObject || top.transform.IsChildOf(target.transform);
    }

    private static string TopHitLabel(RectTransform target, Vector2 point, List<RaycastResult> hits)
    {
        if (hits == null || hits.Count == 0) return "<no-hit>";
        var top = hits[0].gameObject;
        bool belongs = top == target.gameObject || top.transform.IsChildOf(target.transform);
        return top.name + (belongs ? "（目标行）" : "（非目标）");
    }

    /// <summary>
    /// 真实目标行点击：先按领域 ID 解析当前行；若目标行仅因超出列表视口而不可点，
    /// 只用真实 EventSystem scroll 事件（支持向下 / 向上，固定步长、有限步数）把**该目标行**滚到
    /// 完整落在 viewport 内，再重新按同一领域 ID 解析行并执行原有 top-hit + PointerClick。
    ///
    /// 边界：
    ///   - 不直接赋 normalizedPosition / anchoredPosition（滚动只由 IScrollHandler 事件驱动）；
    ///   - 不缩小 viewport、不改行高、不关闭 ModalBlocker / ModalPanel、不克隆假行；
    ///   - 每一步都打印目标 ID / rowRect / viewport / content 位置 / normalizedPosition / top hit；
    ///   - 滚动前后断言 Clock / Selection / Surface 未改变（滚动动作不得推进业务事实）；
    ///   - 若合法滚动到端点后真实 top-hit 仍非目标行，原 AssertTopHitBelongs 会带实际命中栈失败，
    ///     并把几何诊断留在 real-scroll-evidence.log 中，供判定是否需要产品布局窄修。
    /// </summary>
    private void ClickDomainRow(object management, string listMember, string keyMember, string id,
        string contentField, string selectedField, string what)
    {
        var scroll = (ScrollRect)Get(management, "workplaceScroll");
        Assert.IsNotNull(scroll, what + "：workplaceScroll 未绑定");
        Assert.IsNotNull(scroll.content, what + "：workplaceScroll.content 未绑定");
        var outerContent = (RectTransform)scroll.content;
        var viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;

        string selectionBefore = SelectionId();
        string surfaceBefore = Surface();
        long clockBefore = ClockAbsoluteMinute();
        float normBefore = scroll.verticalNormalizedPosition;
        Vector2 contentBefore = outerContent.anchoredPosition;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(outerContent);
        RectTransform target = ResolveDomainRow(management, listMember, keyMember, id, contentField, what,
            out int rowIndex, out int rowCount);
        Rect viewRect = ScreenRectOf(viewport);
        Rect rowRect = ScreenRectOf(target);
        Vector2 startPoint = ScreenCenter(target);
        var startHits = RaycastAllAt(startPoint);
        RealScrollEvidence("REAL-SCROLL-TARGET-START", what + " id=" + id
            + " rowIndex=" + rowIndex + "/" + rowCount
            + " rowRect=" + rowRect + " viewport=" + viewRect
            + " contentRect=" + outerContent.rect + " contentPos=" + contentBefore
            + " norm=" + normBefore
            + " fullyInside=" + RectInside(rowRect, viewRect)
            + " topHit=" + TopHitLabel(target, startPoint, startHits)
            + "（真实领域 ID 定位，非陈旧行对象）");

        const int maxSteps = 240;
        int steps = 0;
        while (steps < maxSteps)
        {
            rowRect = ScreenRectOf(target);
            Vector2 point = ScreenCenter(target);
            bool belongs = TopHitBelongs(target, point, out _);
            if (RectInside(rowRect, viewRect) && belongs) break;

            // 目标在视口下方 -> 滚轮向下（负 scrollDelta，内容上移）；目标在上方 -> 向上。
            float direction = rowRect.center.y < viewRect.center.y ? -1f : 1f;
            Vector2 wheelPoint = ScreenCenter(viewport);
            var wheelHits = RaycastAllAt(wheelPoint);
            Assert.Greater(wheelHits.Count, 0, what + "：目标行滚动必须能在视口中心真实 Raycast");
            var handler = ExecuteEvents.GetEventHandler<IScrollHandler>(wheelHits[0].gameObject);
            Assert.IsNotNull(handler, what + "：视口中心命中父链没有 IScrollHandler\n" + DumpHits(wheelHits));
            Assert.IsTrue(handler == scroll.gameObject,
                what + "：目标行滚动必须由 workplaceScroll 处理，实际=" + handler.name + "\n" + DumpHits(wheelHits));
            var data = new PointerEventData(EventSystem.current)
            {
                position = wheelPoint,
                button = PointerEventData.InputButton.Left,
                scrollDelta = new Vector2(0f, direction)
            };
            Assert.IsTrue(ExecuteEvents.Execute(handler, data, ExecuteEvents.scrollHandler),
                what + "：真实 EventSystem scroll 事件必须被 ScrollRect 处理");
            steps++;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(outerContent);
            // Refresh 可能重建行：每一步都按同一领域 ID 重新解析，绝不持有陈旧行对象。
            target = ResolveDomainRow(management, listMember, keyMember, id, contentField, what,
                out rowIndex, out rowCount);
            rowRect = ScreenRectOf(target);
            Vector2 stepPoint = ScreenCenter(target);
            var stepHits = RaycastAllAt(stepPoint);
            RealScrollEvidence("REAL-SCROLL-TARGET-STEP", what + " id=" + id
                + " step=" + steps + " direction=" + direction
                + " rowRect=" + rowRect + " viewport=" + viewRect
                + " contentPos=" + outerContent.anchoredPosition
                + " norm=" + scroll.verticalNormalizedPosition
                + " fullyInside=" + RectInside(rowRect, viewRect)
                + " topHit=" + TopHitLabel(target, stepPoint, stepHits));
        }

        rowRect = ScreenRectOf(target);
        Vector2 finalPoint = ScreenCenter(target);
        var finalHits = RaycastAllAt(finalPoint);
        RealScrollEvidence("REAL-SCROLL-TARGET-READY", what + " id=" + id
            + " steps=" + steps + " rowRect=" + rowRect + " viewport=" + viewRect
            + " contentPos=" + outerContent.anchoredPosition
            + " norm=" + scroll.verticalNormalizedPosition
            + " fullyInside=" + RectInside(rowRect, viewRect)
            + " topHit=" + TopHitLabel(target, finalPoint, finalHits));

        Assert.AreEqual(clockBefore, ClockAbsoluteMinute(), what + "：目标行真实滚动不得推进 Clock");
        Assert.AreEqual(selectionBefore, SelectionId(), what + "：目标行真实滚动不得改变 Selection");
        Assert.AreEqual(surfaceBefore, Surface(), what + "：目标行真实滚动不得改变 Surface");

        // 再次按领域 ID 解析，保证点击的是当前行对象；原 top-hit + PointerClick 断言保持不变。
        target = ResolveDomainRow(management, listMember, keyMember, id, contentField, what,
            out rowIndex, out rowCount);
        RealPointerClick(target, what);
        string expected = (string)Get(management, selectedField);
        Assert.AreEqual(id, expected, what + "：真实行点击必须设为对应候选");
    }

    private void ClickWorkplaceRow(object management, string workplaceId, string what)
    {
        ClickDomainRow(management, "Workplaces", "WorkplaceId", workplaceId,
            "workplaceContent", "SelectedWorkplaceId", what);
    }

    private void ClickConstructionRow(object management, string buildingId, string what)
    {
        ClickDomainRow(management, "Construction", "BuildingInstanceId", buildingId,
            "constructionContent", "SelectedConstructionBuildingId", what);
    }

    private string PlaceBuilding(string definition, float x)
    {
        object build = Get(_shell, "Build");
        Call(build, "OpenCatalog");
        Assert.IsTrue((bool)Call(build, "Choose", definition), "建造目录必须可选择 " + definition);
        Call(build, "SetCandidate", new Vector2(x, -4f));
        Assert.IsTrue((bool)Call(build, "Confirm"), "合法落点必须能确认建造 " + definition);
        Call(build, "Cancel");
        Assert.IsFalse((bool)Get(build, "IsOpen"), "确认建造后建造面板必须关闭");
        Call(_host, "RefreshNow");
        string id = SelectionId();
        Assert.IsFalse(string.IsNullOrEmpty(id), "建造确认后必须选中新建筑实例");
        Evidence("BUILD", "definition=" + definition + " instance=" + id);
        return id;
    }

    /// <summary>真实 Clock.AdvanceTo；宿主夹具已 Pause，这里只做确定性分钟推进（不直接改 Work 状态）。</summary>
    private static void AdvanceTo(long minute)
    {
        object clock = Clock();
        // 时间值类型与真实 Clock 位于同一运行时装配，不属于 Assembly-CSharp。
        var clockAssembly = clock.GetType().Assembly;
        object target = Activator.CreateInstance(clockAssembly.GetType("WorldInstant", true), minute);
        object origin = Enum.Parse(clockAssembly.GetType("WorldAdvanceOrigin", true), "Direct");
        object result = Call(clock, "AdvanceTo", target, origin, null, null);
        int retries = 0;
        while (Get(result, "Outcome").ToString() == "Yielded" && retries++ < 100)
        {
            object session = Get(clock, "AdvanceSession");
            Assert.IsNotNull(session, "Yielded 状态必须存在 AdvanceSession");
            result = Call(clock, "ResumeAdvance", Get(session, "RequestId"), null);
        }
        Assert.AreEqual("Completed", Get(result, "Outcome").ToString(),
            "世界推进未完成：" + (Get(result, "Diagnostic") ?? string.Empty));
        Evidence("ADVANCE", "targetMinute=" + minute + " outcome=Completed");
    }

    // ==================== 显式保存 / Continue / NewGame 连续性只读取证 ====================

    private static object ResourcesState()
    {
        object resources = StaticField("VillageRuntimeBridge", "Resources");
        Assert.IsNotNull(resources, "VillageRuntimeBridge.Resources 必须存在（真实资源 Authority）");
        return resources;
    }

    private static int Ingredients() => Convert.ToInt32(Get(ResourcesState(), "Ingredients"));

    private static int Food() => Convert.ToInt32(Get(ResourcesState(), "Food"));

    /// <summary>
    /// 从 VillageResourceState 自身构造取得“新世界初始资源 Authority”（默认 0/0），
    /// 不读取当前 Singleton 的库存值，也不把上一世界/用户旧档的投影当作 expected。
    /// 只借用当前 Authority 的运行时类型（测试程序集不直接引用 Village.Runtime）。
    /// </summary>
    private static object FreshResourceAuthority()
    {
        object current = StaticField("VillageRuntimeBridge", "Resources");
        Assert.IsNotNull(current, "VillageRuntimeBridge.Resources 必须存在，才能取得 VillageResourceState Authority 类型");
        return Activator.CreateInstance(current.GetType());
    }

    private static int InitialResourceIngredients() => Convert.ToInt32(Get(FreshResourceAuthority(), "Ingredients"));

    private static int InitialResourceFood() => Convert.ToInt32(Get(FreshResourceAuthority(), "Food"));

    private static object IntradayData()
    {
        object work = StaticField("VillageRuntimeBridge", "IntradayWork");
        Assert.IsNotNull(work, "VillageRuntimeBridge.IntradayWork 必须存在");
        return Call(work, "ToSaveData");
    }

    private static int ProductionBatchCount()
        => ((IEnumerable)Get(IntradayData(), "Batches")).Cast<object>().Count();

    private static int ProductionEvidenceCount()
        => ((IEnumerable)Get(IntradayData(), "Evidence")).Cast<object>().Count();

    /// <summary>
    /// 真实历史计数：VillageAssignmentSystem 的累计 Test Contribution 由真实实际执行事实在
    /// Day Tick 提交并随后写入存档；用于验证 Continue 不重复结算历史，且不是 DTO 存在性判断。
    /// </summary>
    private static int AssignmentContributionHistory()
    {
        object assignments = StaticField("VillageRuntimeBridge", "Assignments");
        Assert.IsNotNull(assignments, "VillageRuntimeBridge.Assignments 必须存在");
        return Convert.ToInt32(Get(assignments, "TotalTestContribution"));
    }

    private static long ClockAbsoluteMinute()
        => Convert.ToInt64(Get(Get(Clock(), "CurrentInstant"), "Minute"));

    private static string CurrentSavePath() => (string)Static("SaveSystem", "GetSavePath");

    private static string ReadSaveJson()
    {
        string path = CurrentSavePath();
        Assert.IsTrue(File.Exists(path), "真实单槽存档必须存在：" + path);
        return File.ReadAllText(path);
    }

    private static void SetForceSaveFailure(Func<bool> hook)
        => T("SaveSystem").GetField("ForceSaveFailureForTests", Flags).SetValue(null, hook);

    private static int ConfiguredInitialRosterCount()
    {
        object data = StaticField("DataManager", "Instance");
        Assert.IsNotNull(data, "DataManager.Instance 必须存在");
        var roster = (IList)Get(data, "InitialPersonRoster");
        Assert.IsNotNull(roster, "DataManager.InitialPersonRoster 必须存在");
        return roster.Count;
    }

    /// <summary>
    /// 直接从长期人物真相源（PlayerCollection.OwnedCharacters / OwnedHeroes）枚举某一容器的合法 PersonId。
    /// 口径与管理投影的最小合法人物口径一致（跳过 null 与空 PersonId），但不经过 ManagementReadAdapter，
    /// 因此不会把被测投影自己当成 expected。角色死亡仍属合法人物（管理投影显示 Unavailable）；英雄恒为合法人物。
    /// </summary>
    private static List<string> PersonIdsOf(object source, string member)
    {
        var ids = new List<string>();
        Assert.IsNotNull(source, "人物真相源不得为 null：" + member);
        var persons = (IEnumerable)Get(source, member);
        if (persons == null) return ids;
        foreach (object person in persons)
        {
            if (person == null) continue;
            string personId = (string)Get(person, "PersonId");
            if (!string.IsNullOrEmpty(personId)) ids.Add(personId);
        }
        return ids;
    }

    /// <summary>真实人物源全部合法 PersonId：角色（含死亡）+ 英雄，各自独立读取。</summary>
    private static List<string> LegalPersonIdsFromSource(object source)
    {
        var ids = PersonIdsOf(source, "OwnedCharacters");
        ids.AddRange(PersonIdsOf(source, "OwnedHeroes"));
        return ids;
    }

    /// <summary>管理人口投影的 PersonId 列表（只读取投影身份，不借用其计数/集合作为 expected）。</summary>
    private static List<string> SnapshotPersonIds(object management)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
        return ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .Select(person => (string)Get(person, "PersonId")).ToList();
    }

    // ==================== 存档单槽字节保护 ====================

    private sealed class SaveSlotGuard
    {
        /// <summary>持久备份根（Pilot 仓库内，测试专用；不入 Runtime / Save DTO）。</summary>
        private const string ProtectionRootRelative = "Development/StudioRuns/R3B-SaveProtection-20261024";
        private const string ManifestName = "manifest.json";
        private const string BackupName = "original-save.bin";

        [Serializable] private sealed class ManifestStatus
        {
            public string runId;
            public string status;
        }

        private string _path;
        private string _runId, _runDir, _binPath, _manifestPath;
        private string _captureUtc;
        private bool _existed, _captured, _restored;
        private byte[] _bytes;
        private string _beforeHash;

        public bool Restored => _restored;
        public string RunId => _runId;

        private static string ProtectionRoot()
            => Path.Combine(RepoRoot(), ProtectionRootRelative.Replace('/', Path.DirectorySeparatorChar));

        /// <summary>
        /// 任何测试写档之前调用：先阻断未完成 run，再把当前真实单槽字节持久化 + manifest，
        /// 回读 SHA256 与内存/当前文件完全一致后才允许 fixture 继续。
        /// </summary>
        public void Capture()
        {
            Assert.IsFalse(_captured, "SaveSlotGuard 同一实例只能 Capture 一次");

            // (1) 已有未完成 run（非 restored）时阻止新测试，交人工核对；不自动删除、不覆盖用户当前档。
            BlockOnUnfinishedRuns();

            _path = (string)Static("SaveSystem", "GetSavePath");
            Assert.IsFalse(string.IsNullOrEmpty(_path), "SaveSystem.GetSavePath 必须返回真实路径");

            _captureUtc = DateTime.UtcNow.ToString("O");
            _runId = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
            string root = ProtectionRoot();
            _runDir = Path.Combine(root, _runId);
            for (int i = 2; Directory.Exists(_runDir); i++) _runDir = Path.Combine(root, _runId + "-" + i);
            _runId = Path.GetFileName(_runDir);
            Directory.CreateDirectory(_runDir); // runID 目录已确认不存在，绝不覆盖

            _binPath = Path.Combine(_runDir, BackupName);
            _manifestPath = Path.Combine(_runDir, ManifestName);

            _existed = File.Exists(_path);
            _bytes = _existed ? File.ReadAllBytes(_path) : null;
            _beforeHash = _existed ? HashBytes(_bytes) : "<absent>";

            if (_existed)
            {
                // CreateNew：绝不覆盖 / 删除任何已有备份
                using (var stream = new FileStream(_binPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    stream.Write(_bytes, 0, _bytes.Length);

                Assert.IsTrue(File.Exists(_binPath), "持久备份未生成：" + _binPath);
                byte[] readback = File.ReadAllBytes(_binPath);
                string readbackHash = HashBytes(readback);
                Assert.AreEqual(_bytes.LongLength, readback.LongLength, "备份字节数回读不一致：" + _binPath);
                Assert.AreEqual(_beforeHash, readbackHash, "备份回读 SHA256 与捕获不一致：" + _binPath);
                Assert.AreEqual(_beforeHash, HashFile(_path), "捕获后当前档已被改写，拒绝继续写档：" + _path);
                Evidence("SAVE-BACKUP", "runId=" + _runId + " status=captured file=" + _binPath
                    + " bytes=" + readback.LongLength + " sha256=" + readbackHash + " readback-verified=True");
            }
            else
            {
                // 原无档：不造旧档，manifest 明确记录“不存在”。
                Evidence("SAVE-BACKUP", "runId=" + _runId + " status=absent file=<none> sha256=<absent> readback-verified=n/a");
            }

            WriteManifest(_existed ? "captured" : "absent", null);
            _captured = true;
            Evidence("SAVE-CAPTURE", "runId=" + _runId + " path=" + _path + " existed=" + _existed
                + " bytes=" + (_bytes == null ? 0 : _bytes.LongLength) + " sha256=" + _beforeHash
                + " backup=" + (_existed ? _binPath : "<none>") + " manifest=" + _manifestPath);
        }

        /// <summary>
        /// 只用本次 runID 已验 hash 的备份恢复；写回后回读 hash 完全一致才标 restored。
        /// 失败时保留 backup/manifest 并抛异常（不吞异常、不 Assert.Pass、不自动清记录求绿）。
        /// </summary>
        public void Restore()
        {
            Assert.IsTrue(_captured, "SaveSlotGuard.Capture 未成功完成，禁止 Restore（不得凭空写回）");

            if (_restored)
            {
                bool stillOk = File.Exists(_path) == _existed && (!_existed || HashFile(_path) == _beforeHash);
                if (stillOk)
                {
                    Evidence("SAVE-RESTORE", "runId=" + _runId + " idempotent=True existed=" + _existed
                        + " sha256=" + (_existed ? _beforeHash : "<absent>") + " matches-before=True");
                    return;
                }
                Evidence("SAVE-RESTORE", "runId=" + _runId + " idempotent=True drift-detected=True 仍按本次已验备份恢复");
            }

            byte[] source = null;
            if (_existed)
            {
                Assert.IsTrue(File.Exists(_binPath), "本次持久备份缺失，拒绝恢复：" + _binPath);
                source = File.ReadAllBytes(_binPath);
                Assert.AreEqual(_beforeHash, HashBytes(source), "本次备份回读 SHA256 与捕获不一致，拒绝写回：" + _binPath);
            }

            try
            {
                if (_existed) File.WriteAllBytes(_path, source);
                else if (File.Exists(_path)) File.Delete(_path);

                bool exists = File.Exists(_path);
                string afterHash = exists ? HashFile(_path) : "<absent>";
                bool ok = exists == _existed && (!_existed || afterHash == _beforeHash);
                _restored = ok;
                WriteManifest(ok ? "restored" : "restore-failed", ok ? null : "sha256 mismatch after write");
                Evidence("SAVE-RESTORE", "runId=" + _runId + " existed=" + exists + " sha256=" + afterHash
                    + " matches-before=" + ok + " backup=" + (_existed ? _binPath : "<none>") + " manifest=" + _manifestPath);
                Assert.IsTrue(ok, "单槽存档恢复失败（backup/manifest 已保留，需人工核对）：期望 existed=" + _existed
                    + " sha256=" + _beforeHash + "，实际 existed=" + exists + " sha256=" + afterHash
                    + "；runId=" + _runId + " backup=" + (_existed ? _binPath : "<none>"));
            }
            catch (Exception ex)
            {
                if (!_restored) WriteManifest("restore-failed", ex.GetType().Name + ": " + ex.Message);
                Evidence("SAVE-RESTORE", "runId=" + _runId + " error=" + ex.GetType().Name + " message=" + ex.Message
                    + " backup-kept=" + _existed + " manifest=" + _manifestPath);
                throw; // 不吞异常：恢复失败必须让测试失败
            }
        }

        /// <summary>
        /// 未完成 run 阻断：status 不是 restored / protective-snapshot（或无 manifest）一律视为未完成。
        /// 只读判断，不自动恢复、不移动、不删除任何档。
        /// </summary>
        private static void BlockOnUnfinishedRuns()
        {
            string root = ProtectionRoot();
            if (!Directory.Exists(root)) return;
            var unfinished = new List<string>();
            foreach (var dir in Directory.GetDirectories(root))
            {
                string manifest = Path.Combine(dir, ManifestName);
                if (!File.Exists(manifest))
                {
                    unfinished.Add(Path.GetFileName(dir) + "(no-manifest)");
                    continue;
                }
                ManifestStatus parsed = null;
                try { parsed = JsonUtility.FromJson<ManifestStatus>(File.ReadAllText(manifest)); }
                catch (Exception) { }
                string status = parsed == null ? null : parsed.status;
                if (status == "restored" || status == "protective-snapshot") continue;
                unfinished.Add(Path.GetFileName(dir) + "(" + (status ?? "unreadable") + ")");
            }
            Assert.AreEqual(0, unfinished.Count,
                "存在未完成的存档保护 run，已阻止新测试以免覆盖当前档；请人工核对后再继续：" + string.Join("; ", unfinished)
                + " @ " + root);
        }

        private void WriteManifest(string status, string error)
        {
            var sb = new StringBuilder();
            sb.Append("{\n");
            sb.Append("  \"runId\": \"" + JsonText(_runId) + "\",\n");
            sb.Append("  \"status\": \"" + JsonText(status) + "\",\n");
            sb.Append("  \"captureUtc\": \"" + JsonText(_captureUtc) + "\",\n");
            sb.Append("  \"updatedUtc\": \"" + DateTime.UtcNow.ToString("O") + "\",\n");
            sb.Append("  \"sourcePath\": \"" + JsonText(_path) + "\",\n");
            sb.Append("  \"sourceExisted\": " + (_existed ? "true" : "false") + ",\n");
            sb.Append("  \"sourceBytes\": " + (_bytes == null ? 0 : _bytes.LongLength) + ",\n");
            sb.Append("  \"sourceSha256\": \"" + JsonText(_beforeHash) + "\",\n");
            sb.Append("  \"backupFile\": " + (_existed ? "\"" + BackupName + "\"" : "null") + ",\n");
            sb.Append("  \"backupBytes\": " + (_existed && File.Exists(_binPath) ? new FileInfo(_binPath).Length : 0) + ",\n");
            sb.Append("  \"backupSha256\": " + (_existed ? "\"" + JsonText(_beforeHash) + "\"" : "null") + ",\n");
            sb.Append("  \"readbackVerified\": " + (_existed ? "true" : "false") + ",\n");
            sb.Append("  \"restored\": " + (_restored ? "true" : "false") + ",\n");
            sb.Append("  \"lastError\": " + (error == null ? "null" : "\"" + JsonText(error) + "\"") + ",\n");
            sb.Append("  \"capturedBy\": \"ManagementShellPlayTests.SaveSlotGuard (test-only helper)\",\n");
            sb.Append("  \"note\": \"只允许用本 runID 的 original-save.bin 恢复；进程 abort 时 TearDown 不执行，备份留待人工取证，不宣称自动恢复。\"\n");
            sb.Append("}\n");
            File.WriteAllText(_manifestPath, sb.ToString());
        }

        private static string JsonText(string value)
        {
            if (value == null) return string.Empty;
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }

    // ==================== 证据 / 保护哈希 ====================

    private static string RepoRoot() => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));

    private static string EvidencePath() => Path.Combine(RepoRoot(), EvidenceRelative.Replace('/', Path.DirectorySeparatorChar));

    private static void Evidence(string tag, string message)
    {
        string line = DateTime.UtcNow.ToString("O") + " [" + tag + "] " + message;
        Debug.Log("[ManagementShellPlayTests] " + tag + " " + message);
        try
        {
            string path = EvidencePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllText(path, line + "\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ManagementShellPlayTests] evidence write failed: " + ex.Message);
        }
    }

    private static string HashBytes(byte[] bytes)
    {
        using (var sha = SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static string HashFile(string path)
    {
        using (var sha = SHA256.Create())
        using (var stream = File.OpenRead(path))
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty).ToLowerInvariant();
    }

    [Serializable] public sealed class BaselineEntry
    {
        public string path;
        public long bytes;
        public string sha256;
    }

    [Serializable] public sealed class BaselineDocument
    {
        public string root;
        public BaselineEntry[] files;
    }

    private static BaselineDocument LoadBaseline()
    {
        string path = Path.Combine(RepoRoot(), BaselineRelative.Replace('/', Path.DirectorySeparatorChar));
        Assert.IsTrue(File.Exists(path), "34 项基线不存在：" + path);
        var document = JsonUtility.FromJson<BaselineDocument>(File.ReadAllText(path));
        Assert.IsNotNull(document, "34 项基线 JSON 解析失败");
        Assert.IsNotNull(document.files, "34 项基线缺少 files");
        Assert.AreEqual(34, document.files.Length, "基线项数必须为 34");
        Assert.AreEqual(Path.GetFullPath(RepoRoot()).TrimEnd(Path.DirectorySeparatorChar),
            Path.GetFullPath(document.root).TrimEnd(Path.DirectorySeparatorChar), "基线 root 必须等于当前仓库根");
        return document;
    }

    private static Dictionary<string, string> HashBaseline(BaselineDocument document, List<string> problems)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var entry in document.files)
        {
            string absolute = Path.Combine(RepoRoot(), entry.path.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(absolute))
            {
                problems.Add("missing " + entry.path);
                result[entry.path] = "<missing>";
                continue;
            }
            long length = new FileInfo(absolute).Length;
            string hash = HashFile(absolute);
            result[entry.path] = hash;
            if (length != entry.bytes) problems.Add("bytes " + entry.path + " expected=" + entry.bytes + " actual=" + length);
            if (!string.Equals(hash, entry.sha256, StringComparison.Ordinal))
                problems.Add("sha256 " + entry.path + " expected=" + entry.sha256 + " actual=" + hash);
        }
        return result;
    }

    // ==================== Lifecycle ====================

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var document = LoadBaseline();
        var problems = new List<string>();
        _baselineBefore = HashBaseline(document, problems);
        Evidence("BASELINE-PRE", "total=" + document.files.Length + " diff=" + problems.Count
            + (problems.Count == 0 ? " all-match" : " " + string.Join("; ", problems)));
        Assert.AreEqual(0, problems.Count, "测试前 34 项基线已不一致：" + string.Join("; ", problems));

        string font = Path.Combine(RepoRoot(), ProtectedFontRelative.Replace('/', Path.DirectorySeparatorChar));
        Assert.IsTrue(File.Exists(font), "受保护 TMP 字体不存在：" + font);
        _fontBefore = HashFile(font);
        Evidence("FONT-PRE", "sha256=" + _fontBefore);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        // 兜底：最后一次 TearDown 若未成功恢复，尝试用同一 guard（同 runID 已验备份）恢复一次。
        // 这只覆盖“TearDown 未跑完但 fixture 仍收尾”的情形；进程级 abort 不会执行这里，
        // 那时只能依靠磁盘上的持久 backup/manifest 人工取证，本方法不构成进程级保证。
        if (_lastGuard != null && !_lastGuard.Restored)
        {
            try
            {
                _lastGuard.Restore();
                Evidence("SAVE-RESTORE-ONETIME", "ok=True note=OneTimeTearDown 兜底恢复，不构成进程级保证");
            }
            catch (Exception ex)
            {
                Evidence("SAVE-RESTORE-ONETIME", "ok=False error=" + ex.GetType().Name + " message=" + ex.Message
                    + " note=backup/manifest 已保留，需人工核对");
                Debug.LogWarning("[ManagementShellPlayTests] OneTimeTearDown 兜底恢复失败（不掩盖）：" + ex.Message);
            }
        }

        string font = Path.Combine(RepoRoot(), ProtectedFontRelative.Replace('/', Path.DirectorySeparatorChar));
        string fontAfter = File.Exists(font) ? HashFile(font) : "<missing>";
        Evidence("FONT-POST", "sha256=" + fontAfter + " unchanged=" + (fontAfter == _fontBefore));

        var document = LoadBaseline();
        var problems = new List<string>();
        var after = HashBaseline(document, problems);
        int same = after.Count(pair => _baselineBefore != null && _baselineBefore.TryGetValue(pair.Key, out var before)
                                       && before == pair.Value);
        int changed = after.Count(pair => _baselineBefore != null && _baselineBefore.TryGetValue(pair.Key, out var before)
                                         && before != pair.Value);
        Evidence("BASELINE-POST", "total=" + after.Count + " same=" + same + " changed=" + changed
            + " problems=" + problems.Count + (problems.Count == 0 ? " all-match" : " " + string.Join("; ", problems)));

        Assert.AreEqual(0, problems.Count, "测试后 34 项基线出现差异：" + string.Join("; ", problems));
        Assert.AreEqual(_fontBefore, fontAfter, "受保护 TMP 字体在测试后被持久修改");
    }

    [UnitySetUp]
    public IEnumerator EnterTownThroughProductEntry()
    {
        _saveGuard = new SaveSlotGuard();
        _saveGuard.Capture();
        _lastGuard = _saveGuard;

        // UnitySetUp 失败时 NUnit 不保证普通 [TearDown] 一定执行（本轮首例已实测：
        // Capture 后 NewGame 反射歧义，Save-RESTORE 直到 OneTimeTearDown 才发生）。
        // yield 不能放进带 catch 的 try；try/finally 会在 MoveNext 抛出 / 枚举器 Dispose 时执行，
        // 因此只要 fixture 已成功 Capture，就必须在这里用本 runID 的已验备份恢复当前档。
        bool entryCompleted = false;
        try
        {
            Static("VillageRuntimeBridge", "ResetForTests");
            SceneManager.LoadScene("SampleScene");
            yield return null;
            yield return null;

            object player = Static("SaveSystem", "NewGame");
            Assert.IsNotNull(player, "NewGame 必须能创建隔离世界");
            Assert.IsTrue((bool)Static("SaveSystem", "Save", player), "隔离存档主入口 Save 必须成功（失败不忽略）");

            object menu = Find("MainMenuUI");
            Assert.IsNotNull(menu, "MainMenuUI 必须存在");
            Call(menu, "RefreshContinueButton");
            Assert.IsTrue(((Button)Get(menu, "btnContinue")).interactable, "隔离存档写入后 Continue 必须可用");
            // 真实产品入口：MainMenuUI.OnContinue → SaveSystem.Load → TownUI.Initialize。
            Call(menu, "OnContinue");
            yield return null;
            yield return null;

            for (int i = 0; i < 10 && FindOptional("ProductHomeWorldHost") == null; i++) yield return null;
            _town = Find("TownUI");
            Assert.IsNotNull(_town, "TownUI 必须由产品入口进入");
            Assert.IsTrue(((GameObject)Get(_town, "canvasRoot")).activeSelf, "TownCanvas 必须由 TownUI.Initialize 激活");
            _player = Get(_town, "PlayerData");
            Assert.IsNotNull(_player, "TownUI.PlayerData 必须来自真实 Load");
            _host = Find("ProductHomeWorldHost");
            Assert.IsNotNull(_host, "ProductHomeWorldHost 必须由产品世界创建");
            _shell = Get(_host, "Shell");
            Assert.IsNotNull(_shell, "HomeProductShell 必须存在");
            for (int i = 0; i < 10 && !(bool)Get(_shell, "ShellVisible"); i++) yield return null;
            Assert.IsTrue((bool)Get(_shell, "ShellVisible"), "Shell 必须随真实产品入口可见");
            Assert.IsTrue((bool)Get(_host, "WorldVisible"), "World 必须随真实产品入口可见");

            Assert.IsNotNull(Clock(), "Village 时间权威必须存在");
            Call(Clock(), "Pause"); // 确定性：避免世界在用例期间自行推进
            Evidence("ENTRY", "Product entry reached World; clock=Paused; saveGuard=active");
            entryCompleted = true;
        }
        finally
        {
            if (!entryCompleted) RestoreCapturedSaveAfterFailedSetUp();
        }
    }

    /// <summary>
    /// UnitySetUp 已 Capture 后发生任何异常（含反射调用失败、产品入口断言失败）时的唯一清理：
    /// 只用本 fixture 成功捕获的本 runID 备份恢复，并让 SaveSlotGuard 的 manifest / Evidence 留痕。
    /// 恢复自身失败不得掩盖原始 SetUp 异常：记录证据与告警，backup / manifest 保留供人工核对。
    /// </summary>
    private void RestoreCapturedSaveAfterFailedSetUp()
    {
        if (_saveGuard == null) return;
        try
        {
            _saveGuard.Restore();
            Evidence("SAVE-RESTORE-SETUP-FAILURE", "runId=" + _saveGuard.RunId
                + " ok=True note=UnitySetUp 失败后按本 runID 已验备份恢复，不依赖普通 TearDown");
        }
        catch (Exception ex)
        {
            Evidence("SAVE-RESTORE-SETUP-FAILURE", "runId=" + _saveGuard.RunId
                + " ok=False error=" + ex.GetType().Name + " message=" + ex.Message
                + " backup/manifest 已保留，需人工核对");
            Debug.LogWarning("[ManagementShellPlayTests] UnitySetUp 失败后的存档恢复失败（不掩盖原始异常）："
                + ex.GetType().Name + ": " + ex.Message);
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            try
            {
                T("SaveSystem").GetField("ForceSaveFailureForTests", Flags).SetValue(null, null);
            }
            finally
            {
                try
                {
                    if (_saveGuard != null) _saveGuard.Restore();
                }
                finally
                {
                    try { Static("VillageRuntimeBridge", "ResetForTests"); }
                    catch (Exception ex) { Debug.LogWarning("[ManagementShellPlayTests] ResetForTests failed: " + ex.Message); }
                }
            }
        }
        finally
        {
            // 恢复必须排在 ResetForTests 之后：本例以 Return 最终保存前的世界写锁结束，
            // ConfigureActivitySchedule 有 GuardWorldWrite，锁未释放时写入会被产品契约静默拒绝，
            // 旧顺序（先恢复后 Reset）既恢复不了同一实例，即使成功也会被 Reset 再次重建覆盖。
            // 放在最外层 finally：前面的存档兜底恢复 / Reset 即使抛异常，也必须执行本次核对，
            // 且日程断言失败不会短路已完成的 SaveSlotGuard.Restore。
            RestoreActivityScheduleAfterFixture();
        }
    }

    // ==================== 共用产品操作 ====================

    private void OpenManagementThroughNavigation(string what = "底部导航·管理")
    {
        var navigation = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate2");
        Assert.IsNotNull(navigation, "真实 Shell 底部导航必须存在“管理”按钮 Navigate2");
        var button = navigation.GetComponent<Button>();
        Assert.IsNotNull(button, "底部导航“管理”必须是 Button");
        Assert.IsTrue(button.interactable, "世界状态下底部导航“管理”必须可点");
        RealPointerClick((RectTransform)navigation, what);
    }

    private object Management() => Get(_shell, "Management");

    private int TotalPersonCount(object management)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
        var persons = (IEnumerable)Get(snapshot, "Persons");
        return persons.Cast<object>().Count();
    }

    /// <summary>
    /// 管理人口行标题（与 ManagementView.ApplyPopulation 的 authored 显示规则一致）：
    /// DisplayName + " · " +（英雄 / 职业或缺失占位）。
    /// 只用于把“本帧可见业务行”与当前投影人物逐行对照，不替代 PersonId 集合断言。
    /// </summary>
    private static string ExpectedPopulationRowTitle(object person)
    {
        string profession = (string)Get(person, "Profession");
        bool isHero = (bool)Get(person, "IsHero");
        return (string)Get(person, "DisplayName") + " · "
               + (isHero ? "英雄" : (string.IsNullOrEmpty(profession) ? "职业资料暂缺" : profession));
    }

    private static List<string> ExpectedPopulationRowTitles(object management)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
        return ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .Select(ExpectedPopulationRowTitle).ToList();
    }

    private static string PopulationRowTitle(Transform row)
    {
        var view = row.GetComponent(T("ManagementRowView"));
        Assert.IsNotNull(view, "人口行必须绑定 ManagementRowView：" + row.name);
        var label = (TMP_Text)Get(view, "label");
        Assert.IsNotNull(label, "人口行 label 必须绑定：" + row.name);
        return label.text;
    }

    /// <summary>
    /// 人口容器按“逐对象生命周期”分代：activeSelf==true 视为当前代业务行；
    /// activeSelf==false 视为 ManagementView.ClearRows 已停用、本帧末才物理销毁的上一代行。
    /// 不使用 Transform.childCount 作为代际 / 异步 Destroy 是否完成的判据。
    /// </summary>
    private sealed class PopulationRowGenerations
    {
        public List<Transform> Active;
        public List<Transform> Pending;
    }

    private static PopulationRowGenerations ClassifyPopulationRows(RectTransform content)
    {
        var children = content.Cast<Transform>().Where(row => row != null).ToList();
        return new PopulationRowGenerations
        {
            Active = children.Where(row => row.gameObject.activeSelf).ToList(),
            Pending = children.Where(row => !row.gameObject.activeSelf).ToList()
        };
    }

    /// <summary>
    /// 当前代人口行（activeSelf）：数量、身份多重集（含重复计数）必须唯一匹配真实新来源；
    /// Button 必须 authored 可交互，且 IsActive/activeInHierarchy 必须与人口页当前是否显示一致。
    /// </summary>
    private static void AssertCurrentPopulationGeneration(List<Transform> rows, List<string> expectedTitles,
        bool populationPageShown, string what)
    {
        Assert.AreEqual(expectedTitles.Count, rows.Count,
            what + "：当前代人口行数量必须等于真实新来源人数（active=" + rows.Count
            + " expected=" + expectedTitles.Count + "）");
        CollectionAssert.AreEquivalent(expectedTitles, rows.Select(PopulationRowTitle).ToList(),
            what + "：当前代人口行身份多重集必须唯一匹配真实新来源（无旧来源残留、无重复业务行）");
        Assert.AreEqual(rows.Count, rows.Distinct().Count(), what + "：当前代人口行实例不得重复");
        foreach (var row in rows)
        {
            Assert.IsTrue(row.gameObject.activeSelf, what + "：当前代人口行必须 activeSelf：" + row.name);
            Assert.AreEqual(populationPageShown, row.gameObject.activeInHierarchy,
                what + "：当前代人口行的层级可见性必须与人口页是否显示一致：" + row.name);
            var button = row.GetComponent<Button>();
            Assert.IsNotNull(button, what + "：当前代人口行必须是真实 Button：" + row.name);
            Assert.IsTrue(button.interactable, what + "：当前代人口行 Button.interactable 必须为 true：" + row.name);
            Assert.AreEqual(populationPageShown, button.IsActive(),
                what + "：当前代人口行按钮可交互状态必须与人口页是否显示一致：" + row.name);
        }
    }

    /// <summary>
    /// 待销毁代（activeSelf==false）：必须已停用、不可交互，且真实 Raycast 不得命中它们
    /// （停用对象不参与布局 / 射线，这是“旧代未泄漏到业务可见面”的逐对象判据）。
    /// </summary>
    private static void AssertPendingPopulationGeneration(List<Transform> rows, string what)
    {
        foreach (var row in rows)
        {
            Assert.IsTrue(row != null, what + "：待销毁人口行引用不得为 null");
            Assert.IsFalse(row.gameObject.activeSelf, what + "：待销毁人口行必须已停用（不参与布局）：" + row.name);
            Assert.IsFalse(row.gameObject.activeInHierarchy,
                what + "：待销毁人口行不得留在活动层级中：" + row.name);
            var button = row.GetComponent<Button>();
            Assert.IsNotNull(button, what + "：待销毁人口行必须是真实 Button：" + row.name);
            Assert.IsFalse(button.IsActive(), what + "：待销毁人口行按钮必须不可交互：" + row.name);
            var point = ScreenCenter((RectTransform)row);
            var hits = RaycastAllAt(point);
            Assert.IsFalse(hits.Any(hit => hit.gameObject == row.gameObject || hit.gameObject.transform.IsChildOf(row)),
                what + "：待销毁人口行不得参与 Raycast：" + row.name + "\n" + DumpHits(hits));
        }
    }

    private string FirstSnapshotPersonId(object management)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
        object person = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>().FirstOrDefault();
        Assert.IsNotNull(person, "人口快照不得为空");
        return (string)Get(person, "PersonId");
    }

    private static void AssertSerializedObjectReferences(object management, int expectedTotal)
    {
        int total = 0, nulls = 0;
        foreach (var field in management.GetType().GetFields(Flags))
        {
            if (field.IsStatic || !field.IsDefined(typeof(SerializeField), true)) continue;
            var type = field.FieldType;
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                total++;
                if (field.GetValue(management) == null) nulls++;
            }
            else if (type.IsArray && typeof(UnityEngine.Object).IsAssignableFrom(type.GetElementType()))
            {
                var array = field.GetValue(management) as Array;
                if (array == null) { nulls++; continue; }
                foreach (var element in array)
                {
                    total++;
                    if (element == null) nulls++;
                }
            }
        }
        Assert.AreEqual(expectedTotal, total, "ManagementView 序列化对象引用数量必须为 40（新增独立详情入口 1 个）");
        Assert.AreEqual(0, nulls, "ManagementView 序列化对象引用不得为 null");
    }

    // ==================== 用例 1：真实产品入口 → 管理 Prefab / 行 / 过滤 / 人物往返 ====================

    [UnityTest]
    public IEnumerator Management_ProductEntry_ShowsAuthoredPrefabRowsAndPersonRoundTrip()
    {
        Assert.IsTrue(SimulationActive(), "进入 Town 后模拟必须处于 Active（正常浏览不得被本页关闭）");
        string selected = FirstSnapshotPersonIdFromHost();
        Call(Get(_host, "Selection"), "SelectPerson", selected);

        OpenManagementThroughNavigation();
        yield return null;
        yield return null;

        var management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        Assert.AreEqual("ManagementView", ((Component)management).gameObject.name, "Prefab 名称必须保持 ManagementView");
        Assert.AreEqual("Management", Surface(), "真实导航点击后必须进入 Management Surface");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "Management Prefab 实例必须被激活");
        Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "管理必须计入统一模态判定");
        Assert.IsTrue((bool)Get(_shell, "IsPersonModalOpen"), "既有 WorldHost 输入 Guard 别名必须覆盖管理模态");

        var whitebox = FindPath(((Component)_shell).transform, "HomeInternalWhitebox");
        Assert.IsNotNull(whitebox, "既有管理白模节点必须存在（保留给 Cards/History）");
        Assert.IsFalse(whitebox.gameObject.activeSelf, "进入真实 Management 后白模必须退出");

        var tabs = SerializedArray(management, "tabButtons");
        Assert.AreEqual(3, tabs.Length, "固定三页签必须完整绑定");
        foreach (Button tab in tabs)
        {
            Assert.IsNotNull(tab, "页签按钮不得为 null");
            Assert.IsTrue(tab.gameObject.activeInHierarchy, "页签按钮必须可见");
        }
        Assert.AreEqual("Population", Get(management, "ActiveTab").ToString(), "默认页签必须是人口");
        AssertSerializedObjectReferences(management, 40);

        var content = (RectTransform)Get(management, "populationContent");
        int total = TotalPersonCount(management);
        Assert.Greater(total, 0, "真实人口投影必须有数据");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        yield return null;
        Assert.AreEqual(total, content.childCount, "人口行必须按真实人数克隆");
        foreach (Transform row in content)
            Assert.IsTrue(row.gameObject.activeInHierarchy, "克隆行必须可见：" + row.name);

        // 页签：真实点击切换并返回
        ClickChild(management, "ModalPanel/Tab_Workplaces", "页签·职责与场所");
        yield return null;
        Assert.AreEqual("Workplaces", Get(management, "ActiveTab").ToString(), "真实点击页签必须切换");
        Assert.IsTrue(((GameObject)Get(management, "workplacePage")).activeSelf, "职责与场所页必须显示");
        ClickChild(management, "ModalPanel/Tab_Population", "页签·人口");
        yield return null;
        Assert.AreEqual("Population", Get(management, "ActiveTab").ToString(), "页签必须可切回人口");

        // 姓名过滤：真实输入触发人数变化
        var search = (TMP_InputField)Get(management, "nameSearch");
        var summary = (TMP_Text)Get(management, "populationSummary");
        string summaryBefore = summary.text;
        search.text = "不存在的姓名_QA_7f3a";
        yield return null;
        Assert.AreEqual(0, content.childCount, "不匹配的姓名过滤必须清空可见行");
        var emptyHint = (Component)Get(management, "populationEmpty");
        Assert.IsNotNull(emptyHint, "populationEmpty 必须绑定");
        Assert.IsTrue(emptyHint.gameObject.activeSelf, "空结果提示必须出现");
        Assert.AreNotEqual(summaryBefore, summary.text, "过滤后汇总人数文本必须变化");
        search.text = string.Empty;
        yield return null;
        Assert.AreEqual(total, content.childCount, "清空过滤必须恢复全部人口行");
        Assert.IsFalse(emptyHint.gameObject.activeSelf, "非空结果不应显示空提示");

        // 人口行单击：只用既有 Selection 选人并留在管理页；查看详情改由独立 authored 入口。
        string expected = FirstSnapshotPersonId(management);
        object firstPerson = ((IEnumerable)Get(Get(management, "CurrentSnapshot"), "Persons")).Cast<object>().First();
        string expectedName = (string)Get(firstPerson, "DisplayName");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        var rowTransform = content.GetChild(0);
        var rowButton = rowTransform.GetComponent<Button>();
        Assert.IsNotNull(rowButton, "人口行必须是真实 Button");
        RealPointerClick((RectTransform)rowTransform, "人口行（第一行·只选人）");
        yield return null;

        Assert.AreEqual("Management", Surface(), "人口行单击必须留在集中管理页（不得强制跳转人物详情）");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "人口行单击后管理页必须仍可见");
        Assert.IsFalse((bool)Get(Get(_shell, "PersonDetail"), "IsVisible"), "人口行单击不得打开人物详情");
        Assert.AreEqual(expected, SelectionId(), "人口行点击必须只用既有 Selection 选中同一 PersonId");
        Assert.AreEqual(expected, (string)Get(management, "SelectedPersonId"), "管理页必须保持同一人物选择");
        var detailButton = (Button)Get(management, "personDetailButton");
        Assert.IsNotNull(detailButton, "独立“查看人物详情”按钮必须由 authored Prefab 绑定");
        Assert.IsTrue(detailButton.gameObject.activeInHierarchy, "独立详情入口必须可见");
        Assert.IsTrue(detailButton.interactable, "选中人物后独立详情入口必须可点");
        var detailTitle = (TMP_Text)Get(management, "detailTitle");
        Assert.IsNotNull(detailTitle, "右栏详情标题必须绑定");
        Assert.IsTrue(detailTitle.text.Contains(expectedName),
            "右栏必须清楚显示当前选中人物，实际标题：" + detailTitle.text);
        var selectedRow = content.Cast<Transform>()
            .FirstOrDefault(row => row != null && row.gameObject.activeSelf
                                   && PopulationRowTitle(row) == ExpectedPopulationRowTitle(firstPerson));
        Assert.IsNotNull(selectedRow, "选中人物行必须仍可见");
        var selectedBackground = selectedRow.GetComponent<Image>();
        Assert.IsNotNull(selectedBackground, "人口行必须有选中底色 Image");
        Assert.AreEqual(new Color(.28f, .43f, .36f, 1), selectedBackground.color,
            "选中人口行必须有清楚的选中高亮");

        // 独立详情入口 → 既有 Person Surface（同一 PersonId / 同一 Selection）
        RealPointerClick((RectTransform)detailButton.transform, "查看人物详情·独立入口");
        yield return null;

        Assert.AreEqual("Person", Surface(), "独立详情入口必须进入既有 Person Surface");
        Assert.AreEqual(expected, SelectionId(), "独立详情入口必须保持同一 PersonId 的选择");
        var detail = Get(_shell, "PersonDetail");
        Assert.IsNotNull(detail, "既有 PersonDetail 必须存在");
        Assert.IsTrue((bool)Get(detail, "IsVisible"), "PersonDetail 必须显示");
        Assert.AreEqual(expected, (string)Get(detail, "DisplayedPersonId"), "PersonDetail 显示同一 PersonId");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "进入人物详情后管理页必须隐藏");

        // 关闭人物详情 → World，Selection 保留
        var close = (Button)Get(detail, "close");
        Assert.IsNotNull(close, "PersonDetail.Close 必须绑定");
        RealPointerClick((RectTransform)close.transform, "人物详情·关闭");
        yield return null;
        Assert.AreEqual("World", Surface(), "人物详情关闭必须回 World");
        Assert.AreEqual(expected, SelectionId(), "关闭后 Selection 必须保留同一人物");
        Assert.IsFalse((bool)Get(detail, "IsVisible"), "关闭后人物详情必须隐藏");

        // 重新打开管理，仍为同一人
        OpenManagementThroughNavigation("底部导航·管理（重开）");
        yield return null;
        Assert.AreEqual("Management", Surface(), "关闭后必须可重新进入管理");
        Assert.AreEqual(expected, (string)Get(management, "SelectedPersonId"), "重新进入管理必须保持同一人物选择");
        Assert.IsTrue(SimulationActive(), "正常浏览/管理不得 SetSimulationActive(false)");
    }

    private string FirstSnapshotPersonIdFromHost()
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        object person = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>().FirstOrDefault();
        Assert.IsNotNull(person, "世界人物快照不得为空");
        return (string)Get(person, "PersonId");
    }

    // ==================== 用例 12：1920 单分辨率管理布局 / 三页签 / 真实滚动可达 ====================

    private const string LayoutEvidenceRelative = "Development/StudioRuns/R3B-UI-20261025/layout-input-evidence";
    private const string RealScrollEvidenceRelative = "Development/StudioRuns/R3B-UI-20261026/play-evidence";

    private static string LayoutEvidenceDir()
        => Path.Combine(RepoRoot(), LayoutEvidenceRelative.Replace('/', Path.DirectorySeparatorChar));

    private static string RealScrollEvidenceDir()
        => Path.Combine(RepoRoot(), RealScrollEvidenceRelative.Replace('/', Path.DirectorySeparatorChar));

    private static void RealScrollEvidence(string tag, string message)
    {
        string line = DateTime.UtcNow.ToString("O") + " [" + tag + "] " + message;
        Debug.Log("[ManagementShellPlayTests] " + tag + " " + message);
        try
        {
            string dir = RealScrollEvidenceDir();
            Directory.CreateDirectory(dir);
            File.AppendAllText(Path.Combine(dir, "real-scroll-evidence.log"), line + "\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ManagementShellPlayTests] real-scroll evidence write failed: " + ex.Message);
        }
    }

    /// <summary>真实渲染帧末截图；新目录 + UTC 时间戳唯一文件名，绝不覆盖历史截图。</summary>
    private static IEnumerator CaptureRealScrollShot(string tag)
    {
        string dir = RealScrollEvidenceDir();
        Directory.CreateDirectory(dir);
        string stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
        string path = Path.Combine(dir, "management-real-scroll-" + tag + "-" + stamp + ".png");
        for (int i = 2; File.Exists(path); i++)
            path = Path.Combine(dir, "management-real-scroll-" + tag + "-" + stamp + "-" + i + ".png");
        try
        {
            ScreenCapture.CaptureScreenshot(path);
        }
        catch (Exception ex)
        {
            RealScrollEvidence("REAL-SCROLL-SHOT-UNAVAILABLE", "tag=" + tag
                + " error=" + ex.GetType().Name + ":" + ex.Message + " path=" + path);
            yield break;
        }
        if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
        for (int i = 0; i < 4; i++) yield return null;
        bool produced = File.Exists(path) && new FileInfo(path).Length > 0;
        RealScrollEvidence(produced ? "REAL-SCROLL-SHOT" : "REAL-SCROLL-SHOT-UNAVAILABLE",
            "tag=" + tag + " produced=" + produced + " path=" + path
            + (produced ? " bytes=" + new FileInfo(path).Length + " sha256=" + HashFile(path) : "（截图工具不可用，不伪造）"));
    }

    private static void LayoutEvidence(string tag, string message)
    {
        string line = DateTime.UtcNow.ToString("O") + " [" + tag + "] " + message;
        Debug.Log("[ManagementShellPlayTests] " + tag + " " + message);
        try
        {
            string dir = LayoutEvidenceDir();
            Directory.CreateDirectory(dir);
            File.AppendAllText(Path.Combine(dir, "layout-input-evidence.log"), line + "\n");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ManagementShellPlayTests] layout evidence write failed: " + ex.Message);
        }
    }

    /// <summary>把 RectTransform 的真实世界四角换算成当前 Screen 像素矩形（含 Canvas 相机）。</summary>
    private static Rect ScreenRectOf(RectTransform rect)
    {
        var corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        var canvas = rect.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        Vector2 min = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
        Vector2 max = min;
        for (int i = 1; i < 4; i++)
        {
            Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    private static void AssertInsideViewport(RectTransform rect, string what)
    {
        Assert.IsNotNull(rect, what + "：RectTransform 未绑定");
        Assert.IsTrue(rect.gameObject.activeInHierarchy, what + "：控件必须可见");
        Assert.Greater(rect.rect.width, 1f, what + "：控件宽度必须为正");
        Assert.Greater(rect.rect.height, 1f, what + "：控件高度必须为正");
        Rect screenRect = ScreenRectOf(rect);
        const float tolerance = 1.5f;
        Assert.IsTrue(screenRect.xMin >= -tolerance && screenRect.yMin >= -tolerance
                      && screenRect.xMax <= Screen.width + tolerance && screenRect.yMax <= Screen.height + tolerance,
            what + "：控件必须完整位于真实视口内，实际 screenRect=" + screenRect
            + " Screen=" + Screen.width + "x" + Screen.height + "（不以 RectTransform 缩放冒充分辨率）");
    }

    private static void AssertControlHit(RectTransform rect, string what, bool requireClickHandler)
    {
        AssertInsideViewport(rect, what);
        Vector2 point = ScreenCenter(rect);
        var hits = RaycastAllAt(point);
        AssertTopHitBelongs(rect, what, point, hits);
        if (requireClickHandler)
            Assert.IsNotNull(ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject),
                what + "：命中对象父链没有 IPointerClickHandler（真实点击不会触发）\n" + DumpHits(hits));
        LayoutEvidence("HIT", what + " point=" + point + " top=" + hits[0].gameObject.name);
    }

    private static void AssertFieldControl(object view, string fieldName, string what, bool requireClickHandler)
    {
        var component = Get(view, fieldName) as Component;
        Assert.IsNotNull(component, what + "：字段 " + fieldName + " 未绑定");
        AssertControlHit((RectTransform)component.transform, what, requireClickHandler);
    }

    private static void AssertManagementModalBlocksWorld(object management, string what)
    {
        var blocker = FindPath(((Component)management).transform, "ModalBlocker");
        var panel = FindPath(((Component)management).transform, "ModalPanel");
        Assert.IsNotNull(blocker, what + "：ModalBlocker 必须存在");
        Assert.IsNotNull(panel, what + "：ModalPanel 必须存在");
        var blockerRect = (RectTransform)blocker;
        Vector2 point = ScreenPointAt(blockerRect, new Vector2(blockerRect.rect.xMin + 12f, 0f));
        var hits = RaycastAllAt(point);
        Assert.Greater(hits.Count, 0, what + "：管理模态背景点必须可 Raycast");
        var top = hits[0].gameObject;
        Assert.IsTrue(top == blocker.gameObject || top.transform.IsChildOf(blocker) || top.transform.IsChildOf(panel),
            what + "：World 背景点必须被管理模态阻断\n" + DumpHits(hits));
        Assert.IsNull(ExecuteEvents.GetEventHandler<IPointerClickHandler>(top),
            what + "：模态背景不得产生按钮点击处理器\n" + DumpHits(hits));
        LayoutEvidence("MODAL", what + " point=" + point + " top=" + top.name + " world-ray-blocked=True");
    }

    private static void AssertTopHudHit(object shell, string what)
    {
        var pause = FindPath(((Component)shell).transform, "TopHUD/Pause");
        Assert.IsNotNull(pause, what + "：TopHUD/Pause 必须存在");
        Assert.IsNotNull(pause.GetComponent<Button>(), what + "：TopHUD/Pause 必须是 Button");
        AssertControlHit((RectTransform)pause, what, true);
    }

    /// <summary>
    /// 严格断言宿主当前真实 Screen 就是 1920x1080。
    /// 制作人 01a53eb8 已取消 1280：本用例不再请求 / 恢复任何分辨率，
    /// 也不通过 GameView API / 用户 Preferences / RectTransform 缩放 / 截图缩放冒充尺寸。
    /// </summary>
    private static void AssertRealScreenAt1920x1080(string what)
    {
        LayoutEvidence("REAL-SCREEN", what + " Screen=" + Screen.width + "x" + Screen.height
            + " mode=" + Screen.fullScreenMode + " 分辨率切换=none（1280 已由制作人 01a53eb8 取消）");
        Assert.AreEqual(1920, Screen.width, what + "：必须运行在宿主真实 Screen.width=1920，实测=" + Screen.width
            + "（不使用 SetResolution / GameView / RectTransform 缩放改尺寸）");
        Assert.AreEqual(1080, Screen.height, what + "：必须运行在宿主真实 Screen.height=1080，实测=" + Screen.height
            + "（不使用 SetResolution / GameView / RectTransform 缩放改尺寸）");
    }

    /// <summary>
    /// 真实 Canvas 必须跟随当前真实 Screen；只读校验，不改分辨率、不缩放 RectTransform。
    /// </summary>
    private static void AssertCanvasMatchesScreen(object management, string what)
    {
        var canvas = ((Component)management).GetComponentInParent<Canvas>();
        Assert.IsNotNull(canvas, what + "：管理页必须位于真实 Canvas 下");
        Canvas.ForceUpdateCanvases();
        float scale = canvas.scaleFactor;
        Rect pixel = canvas.pixelRect;
        var root = (RectTransform)canvas.transform;
        LayoutEvidence("RESOLUTION", what + " Screen=" + Screen.width + "x" + Screen.height
            + " mode=" + Screen.fullScreenMode
            + " canvasPixelRect=" + pixel.width + "x" + pixel.height + " canvasScaleFactor=" + scale
            + " canvasRootRect=" + root.rect.width + "x" + root.rect.height);
        Assert.IsTrue(Mathf.Abs(pixel.width - Screen.width) <= 1.5f,
            what + "：Canvas 像素宽度必须跟随真实 Screen.width，pixel=" + pixel.width + " Screen=" + Screen.width);
        Assert.IsTrue(Mathf.Abs(pixel.height - Screen.height) <= 1.5f,
            what + "：Canvas 像素高度必须跟随真实 Screen.height，pixel=" + pixel.height + " Screen=" + Screen.height);
        Assert.Greater(scale, 0.01f, what + "：Canvas.scaleFactor 必须为正");
    }

    /// <summary>真实渲染帧末截图；工具不可用只记录能力，不伪造文件。</summary>
    private static IEnumerator CaptureLayoutShot(int width, int height, string tab)
    {
        string dir = LayoutEvidenceDir();
        Directory.CreateDirectory(dir);
        string baseName = "management-" + tab + "-" + width + "x" + height;
        string path = Path.Combine(dir, baseName + ".png");
        // 历史 1920 截图（含失败运行证据）必须保留：同名已存在时不删除、不覆盖，
        // 改写带 UTC 时间戳的新文件名；本用例不清理任何历史证据。
        if (File.Exists(path))
            path = Path.Combine(dir, baseName + "-" + DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'") + ".png");
        try
        {
            ScreenCapture.CaptureScreenshot(path);
        }
        catch (Exception ex)
        {
            LayoutEvidence("SCREENSHOT-UNAVAILABLE", "tab=" + tab + " size=" + width + "x" + height
                + " error=" + ex.GetType().Name + ":" + ex.Message + " path=" + path);
            yield break;
        }
        if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
        for (int i = 0; i < 4; i++) yield return null;
        bool produced = File.Exists(path) && new FileInfo(path).Length > 0;
        LayoutEvidence(produced ? "SCREENSHOT" : "SCREENSHOT-UNAVAILABLE",
            "tab=" + tab + " size=" + width + "x" + height + " produced=" + produced
            + " path=" + path + (produced ? " bytes=" + new FileInfo(path).Length : "（截图工具不可用，不伪造）"));
    }

    private static RectTransform LastActiveRow(params RectTransform[] rowRoots)
    {
        for (int r = rowRoots.Length - 1; r >= 0; r--)
        {
            var root = rowRoots[r];
            if (root == null) continue;
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i) as RectTransform;
                if (child != null && child.gameObject.activeInHierarchy) return child;
            }
        }
        return null;
    }

    private static void AssertRowReachable(RectTransform row, RectTransform viewport, Transform panel, string what)
    {
        AssertInsideViewport(row, what);
        Rect rowRect = ScreenRectOf(row);
        Rect viewRect = ScreenRectOf(viewport);
        const float tolerance = 1.5f;
        Assert.IsTrue(rowRect.yMin >= viewRect.yMin - tolerance && rowRect.yMax <= viewRect.yMax + tolerance
                      && rowRect.xMin >= viewRect.xMin - tolerance && rowRect.xMax <= viewRect.xMax + tolerance,
            what + "：末行必须完整落在滚动视口内 row=" + rowRect + " viewport=" + viewRect);
        Vector2 point = ScreenCenter(row);
        var hits = RaycastAllAt(point);
        AssertTopHitBelongs(row, what, point, hits);
        var handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(hits[0].gameObject);
        Assert.IsNotNull(handler, what + "：末行必须可点击（Raycast + PointerClick handler，不执行点击以免离开管理）\n" + DumpHits(hits));
        Assert.IsTrue(hits[0].gameObject.transform.IsChildOf(panel),
            what + "：末行命中不得穿透管理模态\n" + DumpHits(hits));
        LayoutEvidence("ROW", what + " row=" + row.name + " rowRect=" + rowRect + " viewport=" + viewRect
            + " handler=" + handler.name + "（程序化 Raycast，非物理鼠标）");
    }

    /// <summary>
    /// 内容未超出时如实记录“无需滚动”；超出时只用 EventSystem 的 scroll 事件滚动，
    /// 不直接写 normalizedPosition，并断言位置改变、末行可见且命中正确 UI。
    /// </summary>
    private static IEnumerator AssertScrollReachable(ScrollRect scroll, Transform panel,
        RectTransform[] rowRoots, TMP_Text emptyHint, string what)
    {
        Assert.IsNotNull(scroll, what + "：ScrollRect 未绑定");
        Assert.IsNotNull(scroll.content, what + "：ScrollRect.content 未绑定");
        var content = (RectTransform)scroll.content;
        var viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        yield return null;
        Canvas.ForceUpdateCanvases();
        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        bool overflow = contentHeight > viewportHeight + 1f;
        RectTransform last = LastActiveRow(rowRoots);
        LayoutEvidence("SCROLL", what + " overflow=" + overflow + " contentHeight=" + contentHeight
            + " viewportHeight=" + viewportHeight + " last=" + (last != null ? last.name : "<none>"));
        if (last == null)
        {
            Assert.IsTrue(emptyHint != null && emptyHint.gameObject.activeInHierarchy,
                what + "：没有活动行时必须显示空结果提示");
            LayoutEvidence("SCROLL", what + " empty-hint-visible=True no-scroll-needed（内容不足，如实记录）");
            yield break;
        }
        if (!overflow)
        {
            LayoutEvidence("SCROLL", what + " no-scroll-needed=True（内容未超出视口）");
            AssertRowReachable(last, viewport, panel, what + "·末行（无需滚动）");
            yield break;
        }

        float beforeNorm = scroll.verticalNormalizedPosition;
        Vector2 beforePos = content.anchoredPosition;
        Vector2 point = ScreenCenter(viewport);
        var hits = RaycastAllAt(point);
        Assert.Greater(hits.Count, 0, what + "：滚动视口中心必须可 Raycast");
        var handler = ExecuteEvents.GetEventHandler<IScrollHandler>(hits[0].gameObject);
        Assert.IsNotNull(handler, what + "：滚动视口命中父链没有 IScrollHandler\n" + DumpHits(hits));
        Assert.IsTrue(handler == scroll.gameObject,
            what + "：滚动事件必须由目标 ScrollRect 处理，实际=" + handler.name + "\n" + DumpHits(hits));
        var data = new PointerEventData(EventSystem.current)
        {
            position = point,
            button = PointerEventData.InputButton.Left,
            scrollDelta = new Vector2(0f, -1f)
        };
        int events = 0;
        while (scroll.verticalNormalizedPosition > 0.005f && events < 240)
        {
            data.scrollDelta = new Vector2(0f, -1f);
            Assert.IsTrue(ExecuteEvents.Execute(handler, data, ExecuteEvents.scrollHandler),
                what + "：EventSystem scroll 事件必须被 ScrollRect 处理");
            events++;
            yield return null;
        }
        Canvas.ForceUpdateCanvases();
        yield return null;
        float afterNorm = scroll.verticalNormalizedPosition;
        Vector2 afterPos = content.anchoredPosition;
        Assert.Less(afterNorm, beforeNorm - 0.01f,
            what + "：真实 EventSystem scroll 必须改变 normalizedPosition before=" + beforeNorm
            + " after=" + afterNorm + " events=" + events);
        Assert.Greater(Mathf.Abs(afterPos.y - beforePos.y), 1f,
            what + "：真实 EventSystem scroll 必须改变内容位置 before=" + beforePos + " after=" + afterPos);
        Assert.LessOrEqual(afterNorm, 0.01f,
            what + "：必须能通过真实 scroll 事件滚动到末端 after=" + afterNorm + " events=" + events);
        LayoutEvidence("SCROLL", what + " scrolled=True events=" + events + " norm=" + beforeNorm + "→" + afterNorm
            + " contentY=" + beforePos.y + "→" + afterPos.y + "（程序化 EventSystem scroll，非物理滚轮手感）");
        AssertRowReachable(last, viewport, panel, what + "·末行（真实滚动后）");
    }

    private static bool RectInside(Rect inner, Rect outer, float tolerance = 1.5f)
        => inner.xMin >= outer.xMin - tolerance && inner.xMax <= outer.xMax + tolerance
           && inner.yMin >= outer.yMin - tolerance && inner.yMax <= outer.yMax + tolerance;

    private static bool PointInside(Rect rect, Vector2 point, float tolerance = 1.5f)
        => point.x >= rect.xMin - tolerance && point.x <= rect.xMax + tolerance
           && point.y >= rect.yMin - tolerance && point.y <= rect.yMax + tolerance;

    /// <summary>
    /// 非人口真实业务行标题的 TMP 渲染核对：可见字符、真实 rendered 尺寸、真实字体 preferred 行高、
    /// 与同一行 detail 墨迹不重叠、墨迹落在行内；当行完整位于滚动视口内时，墨迹也不得被视口裁切。
    /// 只读布局事实，不修改标题规则 / 行高 / 视口。
    /// </summary>
    private static void AssertBusinessRowTitleRendered(Transform row, RectTransform viewport, string what)
    {
        Assert.IsNotNull(row, what + "：业务行不得为 null");
        Assert.IsTrue(row.gameObject.activeInHierarchy, what + "：业务行必须可见：" + row.name);
        var view = row.GetComponent(T("ManagementRowView"));
        Assert.IsNotNull(view, what + "：业务行必须绑定 ManagementRowView：" + row.name);
        var label = (TMP_Text)Get(view, "label");
        var detail = (TMP_Text)Get(view, "detail");
        Assert.IsNotNull(label, what + "：业务行 label 未绑定：" + row.name);
        Assert.IsNotNull(detail, what + "：业务行 detail 未绑定：" + row.name);

        label.ForceMeshUpdate(true);
        detail.ForceMeshUpdate(true);
        var labelRect = (RectTransform)label.transform;
        Assert.IsTrue(label.gameObject.activeInHierarchy && label.enabled,
            what + "：标题对象必须 active + enabled：" + row.name);
        Assert.IsFalse(string.IsNullOrWhiteSpace(label.text), what + "：标题文本不得为空：" + row.name);
        Assert.Greater(label.fontSize, 1f, what + "：标题字号必须为正：" + row.name);
        Assert.Greater(label.textInfo.characterCount, 0,
            what + "：TMP 标题必须产生可见字符（characterCount>0）：" + row.name
            + " text=\"" + label.text + "\" rect=" + labelRect.rect);
        int visibleChars = 0;
        for (int i = 0; i < label.textInfo.characterCount; i++)
            if (label.textInfo.characterInfo[i].isVisible) visibleChars++;
        Assert.Greater(visibleChars, 0,
            what + "：TMP 标题必须至少有一个可见字符：" + row.name + " text=\"" + label.text + "\"");
        Assert.IsFalse(label.isTextOverflowing,
            what + "：标题不得被 Ellipsis 截断：" + row.name + " rect=" + labelRect.rect
            + " fontSize=" + label.fontSize + " text=\"" + label.text + "\"");
        Assert.Greater(label.renderedWidth, 1f, what + "：标题必须有真实渲染宽度：" + row.name);
        Assert.Greater(label.renderedHeight, 1f, what + "：标题必须有真实渲染高度：" + row.name);

        float preferredHeight = label.GetPreferredValues(label.text, labelRect.rect.width, 0f).y;
        Assert.GreaterOrEqual(labelRect.rect.height + 1.5f, preferredHeight,
            what + "：标题矩形高度必须容纳真实字体行高：" + row.name
            + " rectHeight=" + labelRect.rect.height + " preferredHeight=" + preferredHeight
            + " text=\"" + label.text + "\"");
        Assert.GreaterOrEqual(labelRect.rect.height + 1.5f, label.renderedHeight,
            what + "：标题渲染高度不得超过矩形高度：" + row.name
            + " renderedHeight=" + label.renderedHeight + " rectHeight=" + labelRect.rect.height);
        Assert.Greater(label.color.a, 0.5f, what + "：标题颜色 alpha 必须可见：" + row.name);
        Assert.Greater(label.canvasRenderer.GetAlpha(), 0.5f,
            what + "：标题 CanvasRenderer alpha 必须可见：" + row.name);
        Assert.IsFalse(label.canvasRenderer.cull, what + "：标题 CanvasRenderer 不得被 cull：" + row.name);

        Rect labelInk = TextScreenRect(label);
        Rect detailInk = TextScreenRect(detail);
        Assert.Greater(labelInk.width, 10f, what + "：标题墨迹宽度必须 >10px：" + row.name + " ink=" + labelInk);
        Assert.Greater(labelInk.height, 5f, what + "：标题墨迹高度必须 >5px：" + row.name + " ink=" + labelInk);
        Assert.IsFalse(labelInk.Overlaps(detailInk),
            what + "：标题墨迹不得与 detail 墨迹重叠：" + row.name
            + " titleInk=" + labelInk + " detailInk=" + detailInk);

        Rect rowScreen = ScreenRectOf((RectTransform)row);
        Assert.IsTrue(RectInside(labelInk, rowScreen, 2f),
            what + "：标题墨迹必须落在所属行内：" + row.name
            + " titleInk=" + labelInk + " row=" + rowScreen);
        bool rowFullyInsideViewport = viewport != null && RectInside(rowScreen, ScreenRectOf(viewport), 2f);
        if (rowFullyInsideViewport)
            Assert.IsTrue(RectInside(labelInk, ScreenRectOf(viewport), 2f),
                what + "：完整位于视口内的业务行，其标题墨迹不得被视口裁切：" + row.name
                + " titleInk=" + labelInk + " viewport=" + ScreenRectOf(viewport));
        RealScrollEvidence("REAL-SCROLL-TITLE", what + " row=" + row.name
            + " text=\"" + label.text + "\" chars=" + label.textInfo.characterCount + " visible=" + visibleChars
            + " rendered=" + label.renderedWidth + "x" + label.renderedHeight
            + " preferredH=" + preferredHeight + " rect=" + labelRect.rect
            + " overlapsDetail=" + labelInk.Overlaps(detailInk)
            + " rowFullyInsideViewport=" + rowFullyInsideViewport
            + "（TMP 可见字符/渲染尺寸/行高/与 detail 墨迹，非仅字符串）");
    }

    /// <summary>
    /// 职责与场所行 detail 的“关键数值真实可读”断言（不是只做字符串包含）：
    ///   1) detail 矩形必须按真实字体行高容纳全部 3 行（否则 Ellipsis 会把首行之后的数值整段截掉）；
    ///   2) 容量 / 绑定 / 实际劳动 / 现场必须排在第 0 行（首行），且每个非空白字符 isVisible=true；
    ///   3) 完整身份（岗位 WorkplaceId）必须排在第 1 行，同样逐字符可见——身份不得为求简洁被删除；
    ///   4) 每个关键字符的真实 TMP 墨迹必须落在所属行矩形内；当该行完整位于滚动视口内时还必须在视口内。
    /// 只读渲染事实，不改文本 / 行高 / 视口 / 字体。
    /// </summary>
    private static void AssertWorkplaceDetailKeyValuesRendered(Transform row, object workplace,
        RectTransform viewport, string what)
    {
        Assert.IsNotNull(row, what + "：业务行不得为 null");
        Assert.IsNotNull(workplace, what + "：业务行必须有对应真实投影行");
        var view = row.GetComponent(T("ManagementRowView"));
        Assert.IsNotNull(view, what + "：业务行必须绑定 ManagementRowView：" + row.name);
        var detail = (TMP_Text)Get(view, "detail");
        Assert.IsNotNull(detail, what + "：业务行 detail 未绑定：" + row.name);
        detail.ForceMeshUpdate(true);
        var detailRect = (RectTransform)detail.transform;

        Assert.IsTrue(detail.gameObject.activeInHierarchy && detail.enabled,
            what + "：detail 对象必须 active + enabled：" + row.name);
        Assert.Greater(detail.fontSize, 1f, what + "：detail 字号必须为正：" + row.name);
        Assert.Greater(detail.textInfo.characterCount, 0,
            what + "：detail 必须产生真实字符（characterCount>0）：" + row.name);
        Assert.Greater(detail.renderedHeight, 1f, what + "：detail 必须有真实渲染高度：" + row.name);
        Assert.Greater(detail.color.a, 0.5f, what + "：detail 颜色 alpha 必须可见：" + row.name);
        Assert.IsFalse(detail.canvasRenderer.cull, what + "：detail CanvasRenderer 不得被 cull：" + row.name);

        float preferredHeight = detail.GetPreferredValues(detail.text, detailRect.rect.width, 0f).y;
        Assert.GreaterOrEqual(detailRect.rect.height + 1.5f, preferredHeight,
            what + "：detail 矩形高度必须容纳全部真实行高（不得用 Ellipsis 截掉关键数值）：" + row.name
            + " rectHeight=" + detailRect.rect.height + " preferredHeight=" + preferredHeight
            + " text=\"" + detail.text.Replace("\n", "\\n") + "\"");
        Assert.IsFalse(detail.isTextOverflowing,
            what + "：detail 不得整体溢出（矩形高度必须够）：" + row.name
            + " rect=" + detailRect.rect + " preferredHeight=" + preferredHeight);

        string workplaceId = (string)Get(workplace, "WorkplaceId");
        int capacity = Convert.ToInt32(Get(workplace, "Capacity"));
        int bound = Convert.ToInt32(Get(workplace, "BoundCount"));
        int labor = Convert.ToInt32(Get(workplace, "LaborCount"));
        int onsite = Convert.ToInt32(Get(workplace, "OnsiteCount"));
        string counters = "绑定 " + bound + "/" + capacity + " · 实际劳动 " + labor + " · 现场 " + onsite;
        string text = detail.text ?? string.Empty;
        Assert.IsTrue(text.StartsWith(counters, StringComparison.Ordinal),
            what + "：detail 首行必须是容量/绑定/实际劳动/现场可比较数值，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsTrue(text.Contains("\n岗位 " + workplaceId),
            what + "：detail 必须保留完整 WorkplaceId 身份行，实际=\"" + text.Replace("\n", "\\n") + "\"");

        Rect rowScreen = ScreenRectOf((RectTransform)row);
        Rect viewRect = viewport != null ? ScreenRectOf(viewport) : rowScreen;
        bool rowFullyInsideViewport = viewport != null && RectInside(rowScreen, viewRect, 2f);
        int checkedChars = AssertDetailTokenRendered(detail, counters, 0, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·容量/绑定/实际劳动/现场");
        checkedChars += AssertDetailTokenRendered(detail, "岗位 " + workplaceId, 1, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·完整WorkplaceId");
        RealScrollEvidence("REAL-SCROLL-DETAIL", what + " row=" + row.name
            + " text=\"" + text.Replace("\n", "\\n") + "\" lines=" + detail.textInfo.lineCount
            + " preferredH=" + preferredHeight + " rect=" + detailRect.rect
            + " checkedChars=" + checkedChars + " rowsInsideViewport=" + rowFullyInsideViewport
            + "（逐字符 isVisible + 字符墨迹落行/视口，非字符串包含）");
    }

    /// <summary>
    /// 施工行业务行“进度 / 工人人数与姓名 / 现场”断言：三行都必须按真实字体行高完整可读，
    /// 不是只做字符串包含：每行每个非空白字符 isVisible=true、行号匹配、字符墨迹落在行内，
    /// 行完整位于滚动视口内时还必须在视口内。
    /// </summary>
    private static void AssertConstructionDetailProgressRendered(Transform row, object construction,
        object snapshot, RectTransform viewport, string what)
    {
        Assert.IsNotNull(row, what + "：业务行不得为 null");
        Assert.IsNotNull(construction, what + "：业务行必须有对应真实投影行");
        var view = row.GetComponent(T("ManagementRowView"));
        Assert.IsNotNull(view, what + "：业务行必须绑定 ManagementRowView：" + row.name);
        var detail = (TMP_Text)Get(view, "detail");
        Assert.IsNotNull(detail, what + "：业务行 detail 未绑定：" + row.name);
        detail.ForceMeshUpdate(true);
        Assert.Greater(detail.textInfo.characterCount, 0,
            what + "：施工行 detail 必须产生真实字符：" + row.name);
        AssertDetailRectFits(detail, what + "：施工行 detail=" + row.name);
        string buildingId = (string)Get(construction, "BuildingInstanceId");
        string progressLabel = (string)Get(construction, "ProgressLabel");
        Assert.IsFalse(string.IsNullOrWhiteSpace(progressLabel), what + "：施工进度标签不得为空");
        int workerCount = Convert.ToInt32(Get(construction, "WorkerCount"));
        var workerIds = ((IEnumerable)Get(construction, "WorkerPersonIds")).Cast<string>().ToList();
        Assert.AreEqual(workerCount, workerIds.Count, what + "：施工行工人人数必须等于真实参与者集合");
        int onsite = Convert.ToInt32(Get(construction, "OnsiteCount"));
        string workerNames = workerIds.Count == 0
            ? "无"
            : string.Join("、", workerIds.Select(id =>
            {
                object person = FindSnapshotPerson(snapshot, id);
                return person == null ? id : (string)Get(person, "DisplayName") + "（" + id + "）";
            }));
        string workerLine = "工人 " + workerCount + " 人：" + workerNames;
        string onsiteLine = "现场 " + onsite + " 人";
        string text = detail.text ?? string.Empty;
        Assert.IsTrue(text.StartsWith("建筑 " + buildingId + " · 进度 " + progressLabel, StringComparison.Ordinal),
            what + "：施工行首行必须可读到真实进度，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsTrue(text.Contains("\n" + workerLine),
            what + "：施工行必须保留真实工人人数与姓名行，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsTrue(text.Contains("\n" + onsiteLine),
            what + "：施工行必须保留真实现场人数行，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Rect rowScreen = ScreenRectOf((RectTransform)row);
        Rect viewRect = viewport != null ? ScreenRectOf(viewport) : rowScreen;
        bool rowFullyInsideViewport = viewport != null && RectInside(rowScreen, viewRect, 2f);
        int checkedChars = AssertDetailTokenRendered(detail, "进度 " + progressLabel, 0, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·进度");
        checkedChars += AssertDetailTokenRendered(detail, workerLine, 1, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·工人人数与姓名");
        checkedChars += AssertDetailTokenRendered(detail, onsiteLine, 2, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·现场人数");
        RealScrollEvidence("REAL-SCROLL-DETAIL", what + " row=" + row.name
            + " text=\"" + text.Replace("\n", "\\n") + "\" lines=" + detail.textInfo.lineCount
            + " checkedChars=" + checkedChars + " rowsInsideViewport=" + rowFullyInsideViewport
            + "（进度/工人姓名与人数/现场逐字符 isVisible + 字符墨迹落行/视口，非字符串包含）");
    }

    /// <summary>
    /// 人口行业务行“职责 / 现时执行 / 临时承诺 / 饥饿”断言：
    /// 真实字体行高必须容纳全部 3 行（否则 Ellipsis 会把后两行整段截掉），
    /// 每项事实按行号逐字符 isVisible 且字符墨迹落在所属行内（行完整在视口内时也在视口内）。
    /// </summary>
    private static void AssertPersonDetailFactsRendered(Transform row, object person, RectTransform viewport, string what)
    {
        Assert.IsNotNull(row, what + "：业务行不得为 null");
        Assert.IsNotNull(person, what + "：业务行必须有对应真实人口投影");
        var view = row.GetComponent(T("ManagementRowView"));
        Assert.IsNotNull(view, what + "：业务行必须绑定 ManagementRowView：" + row.name);
        var detail = (TMP_Text)Get(view, "detail");
        Assert.IsNotNull(detail, what + "：人口行 detail 未绑定：" + row.name);
        detail.ForceMeshUpdate(true);
        Assert.Greater(detail.textInfo.characterCount, 0,
            what + "：人口行 detail 必须产生真实字符：" + row.name);
        AssertDetailRectFits(detail, what + "：人口行 detail=" + row.name);

        string availability = (string)Get(person, "AvailabilityLabel") ?? string.Empty;
        string responsibility = (string)Get(person, "PrimaryResponsibility");
        if (string.IsNullOrEmpty(responsibility)) responsibility = "无主要职责";
        bool legacy = (bool)Get(person, "AssignmentIsLegacy");
        string execution = (string)Get(person, "CurrentExecution");
        if (string.IsNullOrEmpty(execution)) execution = "暂无执行事实";
        string commitment = (string)Get(person, "TemporaryCommitment");
        if (string.IsNullOrEmpty(commitment)) commitment = "无";
        bool hasFacts = (bool)Get(person, "HasNutritionFacts");
        bool hungry = (bool)Get(person, "Hungry");
        int days = Convert.ToInt32(Get(person, "ConsecutiveUnderfedDays"));
        string nutrition = !hasFacts ? "暂无可读饥饿事实"
            : hungry ? "Hungry（连续不足 " + days + " 天）" : "已满足";

        string line0 = availability + " · 职责：" + responsibility
                       + (legacy ? "（既有兼容记录）" : string.Empty);
        string line1 = "执行：" + execution;
        string line2 = "临时承诺：" + commitment + " · 饥饿：" + nutrition;
        string text = detail.text ?? string.Empty;
        Assert.IsTrue(text.StartsWith(line0, StringComparison.Ordinal),
            what + "：人口 detail 首行必须是可用状态与主要职责，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsTrue(text.Contains("\n" + line1),
            what + "：人口 detail 必须保留现时执行事实，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsTrue(text.Contains("\n" + line2),
            what + "：人口 detail 必须保留临时承诺与饥饿事实，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsFalse(string.IsNullOrWhiteSpace(responsibility), what + "：主要职责事实不得为空");
        Assert.IsFalse(string.IsNullOrWhiteSpace(execution), what + "：现时执行事实不得为空");

        Rect rowScreen = ScreenRectOf((RectTransform)row);
        Rect viewRect = viewport != null ? ScreenRectOf(viewport) : rowScreen;
        bool rowFullyInsideViewport = viewport != null && RectInside(rowScreen, viewRect, 2f);
        int checkedChars = AssertDetailTokenRendered(detail, "职责：" + responsibility, 0, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·主要职责");
        checkedChars += AssertDetailTokenRendered(detail, line1, 1, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·现时执行");
        checkedChars += AssertDetailTokenRendered(detail, "临时承诺：" + commitment, 2, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·临时承诺");
        checkedChars += AssertDetailTokenRendered(detail, "饥饿：" + nutrition, 2, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·饥饿");
        RealScrollEvidence("REAL-SCROLL-DETAIL", what + " row=" + row.name
            + " text=\"" + text.Replace("\n", "\\n") + "\" lines=" + detail.textInfo.lineCount
            + " checkedChars=" + checkedChars + " rowsInsideViewport=" + rowFullyInsideViewport
            + "（职责/现时执行/临时承诺/饥饿逐字符 isVisible + 字符墨迹落行/视口，非字符串包含）");
    }

    /// <summary>
    /// 资源行业务行“实例名额 / 实际劳动 / 建筑实例 / 槽位”断言：3 行文本完整可读，
    /// 逐字符核对真实排版与墨迹；实例身份与被劳动人数不是只做字符串包含。
    /// </summary>
    private static void AssertResourceDetailFactsRendered(Transform row, object resource,
        RectTransform viewport, string what)
    {
        Assert.IsNotNull(row, what + "：业务行不得为 null");
        Assert.IsNotNull(resource, what + "：业务行必须有对应真实资源投影");
        var view = row.GetComponent(T("ManagementRowView"));
        Assert.IsNotNull(view, what + "：业务行必须绑定 ManagementRowView：" + row.name);
        var detail = (TMP_Text)Get(view, "detail");
        Assert.IsNotNull(detail, what + "：资源行 detail 未绑定：" + row.name);
        detail.ForceMeshUpdate(true);
        Assert.Greater(detail.textInfo.characterCount, 0,
            what + "：资源行 detail 必须产生真实字符：" + row.name);
        AssertDetailRectFits(detail, what + "：资源行 detail=" + row.name);

        int capacity = Convert.ToInt32(Get(resource, "Capacity"));
        int bound = Convert.ToInt32(Get(resource, "BoundCount"));
        int labor = Convert.ToInt32(Get(resource, "LaborCount"));
        string buildingId = (string)Get(resource, "BuildingInstanceId");
        string slot = (string)Get(resource, "FunctionSlotId");
        Assert.IsFalse(string.IsNullOrWhiteSpace(buildingId), what + "：资源行建筑实例身份不得为空");
        Assert.IsFalse(string.IsNullOrWhiteSpace(slot), what + "：资源行槽位身份不得为空");
        string counters = "名额 绑定 " + bound + "/" + capacity + " · 实际劳动 " + labor;
        string identity = "建筑 " + buildingId + " · 槽位 " + slot;
        string text = detail.text ?? string.Empty;
        Assert.IsTrue(text.StartsWith(counters, StringComparison.Ordinal),
            what + "：资源行首行必须是名额绑定与实际劳动，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Assert.IsTrue(text.Contains("\n" + identity),
            what + "：资源行必须保留建筑实例与槽位身份，实际=\"" + text.Replace("\n", "\\n") + "\"");
        Rect rowScreen = ScreenRectOf((RectTransform)row);
        Rect viewRect = viewport != null ? ScreenRectOf(viewport) : rowScreen;
        bool rowFullyInsideViewport = viewport != null && RectInside(rowScreen, viewRect, 2f);
        int checkedChars = AssertDetailTokenRendered(detail, counters, 0, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·名额/实际劳动");
        checkedChars += AssertDetailTokenRendered(detail, identity, 1, rowScreen,
            rowFullyInsideViewport ? (Rect?)viewRect : null, what + "·建筑实例/槽位");
        RealScrollEvidence("REAL-SCROLL-DETAIL", what + " row=" + row.name
            + " text=\"" + text.Replace("\n", "\\n") + "\" lines=" + detail.textInfo.lineCount
            + " checkedChars=" + checkedChars + " rowsInsideViewport=" + rowFullyInsideViewport
            + "（名额/劳动/实例身份逐字符 isVisible + 字符墨迹落行/视口，非字符串包含）");
    }

    /// <summary>detail 矩形必须按真实字体行高容纳全部行，且不被 Ellipsis 整体截断。</summary>
    private static void AssertDetailRectFits(TMP_Text detail, string what)
    {
        var rect = (RectTransform)detail.transform;
        Assert.IsTrue(detail.gameObject.activeInHierarchy && detail.enabled, what + "：detail 必须 active + enabled");
        Assert.Greater(detail.fontSize, 1f, what + "：detail 字号必须为正");
        Assert.Greater(detail.renderedHeight, 1f, what + "：detail 必须有真实渲染高度");
        Assert.Greater(detail.color.a, 0.5f, what + "：detail 颜色 alpha 必须可见");
        Assert.IsFalse(detail.canvasRenderer.cull, what + "：detail CanvasRenderer 不得被 cull");
        float preferred = detail.GetPreferredValues(detail.text, rect.rect.width, 0f).y;
        Assert.GreaterOrEqual(rect.rect.height + 1.5f, preferred,
            what + "：detail 矩形高度必须容纳全部真实行高（不得用 Ellipsis 截掉必要数值/工人）："
            + " rectHeight=" + rect.rect.height + " preferredHeight=" + preferred
            + " text=\"" + (detail.text ?? string.Empty).Replace("\n", "\\n") + "\"");
        Assert.IsFalse(detail.isTextOverflowing,
            what + "：detail 不得整体溢出（矩形高度必须够）：rect=" + rect.rect + " preferredHeight=" + preferred);
    }

    private static object FindSnapshotPerson(object snapshot, string personId)
    {
        if (snapshot == null || string.IsNullOrEmpty(personId)) return null;
        return ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .FirstOrDefault(p => string.Equals((string)Get(p, "PersonId"), personId, StringComparison.Ordinal));
    }

    /// <summary>逐字符核对 TMP 真实排版：token 必须位于指定行、每个非空白字符 isVisible、字符墨迹落在行/视口内。</summary>
    private static int AssertDetailTokenRendered(TMP_Text detail, string token, int expectedLine,
        Rect rowScreen, Rect? viewportScreen, string what)
    {
        string text = detail.text ?? string.Empty;
        int start = text.IndexOf(token, StringComparison.Ordinal);
        Assert.GreaterOrEqual(start, 0, what + "：detail 必须包含关键文本 \"" + token
            + "\"，实际=\"" + text.Replace("\n", "\\n") + "\"");
        int checkedChars = 0;
        for (int i = start; i < start + token.Length && i < detail.textInfo.characterCount; i++)
        {
            if (char.IsWhiteSpace(text[i])) continue;
            var info = detail.textInfo.characterInfo[i];
            Assert.IsTrue(info.isVisible, what + "：关键字符必须真实可见（isVisible=true）index=" + i
                + " char='" + text[i] + "' token=\"" + token + "\" line=" + info.lineNumber
                + " text=\"" + text.Replace("\n", "\\n") + "\"");
            Assert.AreEqual(expectedLine, info.lineNumber,
                what + "：关键文本必须排在第 " + expectedLine + " 行（首行=可比较数值）token=\"" + token
                + "\" char='" + text[i] + "' line=" + info.lineNumber);
            Rect ink = CharScreenRect(detail, info);
            Assert.Greater(ink.width, 0.5f, what + "：关键字符必须有真实墨迹宽度 index=" + i
                + " char='" + text[i] + "' ink=" + ink);
            Assert.IsTrue(RectInside(ink, rowScreen, 2.5f), what + "：关键字符墨迹必须落在所属行内 index=" + i
                + " char='" + text[i] + "' ink=" + ink + " row=" + rowScreen);
            if (viewportScreen.HasValue)
                Assert.IsTrue(RectInside(ink, viewportScreen.Value, 2.5f),
                    what + "：完整位于视口内的行，其关键字符墨迹不得被视口裁切 index=" + i
                    + " char='" + text[i] + "' ink=" + ink + " viewport=" + viewportScreen.Value);
            checkedChars++;
        }
        Assert.Greater(checkedChars, 0, what + "：关键文本必须至少有一个非空白字符参与墨迹核对 token=\"" + token + "\"");
        return checkedChars;
    }

    /// <summary>单个 TMP 字符的真实墨迹在当前真实 Screen 上的轴对齐矩形（字符局部坐标 → 世界 → 屏幕）。</summary>
    private static Rect CharScreenRect(TMP_Text text, TMP_CharacterInfo info)
    {
        var canvas = text.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        Vector2 a = RectTransformUtility.WorldToScreenPoint(camera, text.transform.TransformPoint(info.bottomLeft));
        Vector2 b = RectTransformUtility.WorldToScreenPoint(camera, text.transform.TransformPoint(info.topRight));
        return Rect.MinMaxRect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }

    /// <summary>
    /// 只用真实 EventSystem 输入把职责与场所列表滚到末端：
    ///   1) 先发一次真实 IScrollHandler scroll（负 scrollDelta，有效指针在视口内），局部滚动；
    ///   2) 再发 beginDrag → 多次 drag（指针始终在视口内，逐步向上）→ endDrag；若仍不在末端则继续下一轮真实拖拽；
    ///   每一步都读取真实 content.anchoredPosition / verticalNormalizedPosition，并断言末行由不可见变可见。
    /// 不直接写 normalizedPosition，不改 viewport / 行高，不克隆假行。
    /// </summary>
    private static IEnumerator RealScrollAndDragToEnd(ScrollRect scroll, Transform panel,
        RectTransform[] rowRoots, string what)
    {
        Assert.IsNotNull(scroll, what + "：ScrollRect 未绑定");
        Assert.IsNotNull(scroll.content, what + "：ScrollRect.content 未绑定");
        var content = (RectTransform)scroll.content;
        var viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        yield return null;
        Canvas.ForceUpdateCanvases();

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        float overflow = contentHeight - viewportHeight;
        Assert.Greater(overflow, 40f,
            what + "：真实业务内容必须溢出视口至少 40px，才能分别验证滚轮与拖拽；"
            + "contentHeight=" + contentHeight + " viewportHeight=" + viewportHeight);
        Rect viewRect = ScreenRectOf(viewport);
        var last = LastActiveRow(rowRoots);
        Assert.IsNotNull(last, what + "：必须存在真实业务末行");
        Rect lastBefore = ScreenRectOf(last);
        Assert.IsFalse(RectInside(lastBefore, viewRect),
            what + "：末行在滚动前必须不可见（不得完整落在视口内）：last=" + last.name
            + " row=" + lastBefore + " viewport=" + viewRect);
        Assert.IsFalse(PointInside(viewRect, ScreenCenter(last)),
            what + "：末行中心在滚动前必须不在视口内：last=" + last.name
            + " center=" + ScreenCenter(last) + " viewport=" + viewRect);
        RealScrollEvidence("REAL-SCROLL-OVERFLOW", what + " overflow=True contentHeight=" + contentHeight
            + " viewportHeight=" + viewportHeight + " overflowPx=" + overflow
            + " last=" + last.name + " lastRectBefore=" + lastBefore + " viewport=" + viewRect
            + " lastVisibleBefore=False（真实行，不克隆 / 不改尺寸）");

        float beforeNorm = scroll.verticalNormalizedPosition;
        Vector2 beforePos = content.anchoredPosition;

        // (1) 真实 EventSystem scroll 事件：滚动轮一小步，证明 normalizedPosition / content 位置真实改变。
        Vector2 point = ScreenCenter(viewport);
        var hits = RaycastAllAt(point);
        Assert.Greater(hits.Count, 0, what + "：滚动视口中心必须可 Raycast");
        var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(hits[0].gameObject);
        Assert.IsNotNull(scrollHandler, what + "：滚动视口命中父链没有 IScrollHandler\n" + DumpHits(hits));
        Assert.IsTrue(scrollHandler == scroll.gameObject,
            what + "：scroll 事件必须由目标 ScrollRect 处理，实际=" + scrollHandler.name + "\n" + DumpHits(hits));
        var scrollData = new PointerEventData(EventSystem.current)
        {
            position = point,
            button = PointerEventData.InputButton.Left,
            scrollDelta = new Vector2(0f, -0.5f)
        };
        Assert.IsTrue(ExecuteEvents.Execute(scrollHandler, scrollData, ExecuteEvents.scrollHandler),
            what + "：EventSystem scroll 事件必须被 ScrollRect 处理");
        yield return null;
        Canvas.ForceUpdateCanvases();
        float afterScrollNorm = scroll.verticalNormalizedPosition;
        Vector2 afterScrollPos = content.anchoredPosition;
        Assert.Less(afterScrollNorm, beforeNorm - 0.0001f,
            what + "：真实 scroll 必须改变 normalizedPosition before=" + beforeNorm + " after=" + afterScrollNorm);
        Assert.Greater(Mathf.Abs(afterScrollPos.y - beforePos.y), 1f,
            what + "：真实 scroll 必须改变内容位置 before=" + beforePos + " after=" + afterScrollPos);
        Assert.Greater(afterScrollNorm, 0.01f,
            what + "：单次滚轮后必须仍未到末端，才能独立验证拖拽；after=" + afterScrollNorm);
        RealScrollEvidence("REAL-SCROLL-WHEEL", what + " beforeNorm=" + beforeNorm + " afterNorm=" + afterScrollNorm
            + " contentY=" + beforePos.y + "→" + afterScrollPos.y + " point=" + point
            + "（真实 EventSystem scroll，非直接赋 normalizedPosition）");

        // (2) 真实 beginDrag / drag / endDrag：每轮指针都从视口内下部向上移动，仍始终位于列表视口内；
        //     内容按合法业务行高自然溢出后一轮 220px 已不足以到末端，因此只要还没到末端就继续下一轮真实拖拽，
        //     不缩小视口、不放大行高、不直接赋 verticalNormalizedPosition。
        float step = Mathf.Min(60f, viewRect.height * 0.1f);
        Vector2 strokeStart = new Vector2(viewRect.center.x, viewRect.yMin + step + 6f);
        Assert.IsTrue(PointInside(viewRect, strokeStart),
            what + "：拖拽起点必须位于列表视口内 start=" + strokeStart + " viewport=" + viewRect);
        int strokes = 0;
        int totalSteps = 0;
        float dragStartNorm = scroll.verticalNormalizedPosition;
        Vector2 dragStartPos = content.anchoredPosition;
        while (scroll.verticalNormalizedPosition > 0.001f && strokes < 12)
        {
            var dragHits = RaycastAllAt(strokeStart);
            Assert.Greater(dragHits.Count, 0, what + "：拖拽起点必须可 Raycast");
            var dragHandler = ExecuteEvents.GetEventHandler<IBeginDragHandler>(dragHits[0].gameObject);
            Assert.IsNotNull(dragHandler, what + "：拖拽起点父链没有 IBeginDragHandler\n" + DumpHits(dragHits));
            Assert.IsTrue(dragHandler == scroll.gameObject,
                what + "：拖拽必须由目标 ScrollRect 处理，实际=" + dragHandler.name + "\n" + DumpHits(dragHits));
            var dragData = new PointerEventData(EventSystem.current)
            {
                position = strokeStart,
                pressPosition = strokeStart,
                button = PointerEventData.InputButton.Left,
                clickCount = 1,
                dragging = true
            };
            Assert.IsTrue(ExecuteEvents.Execute(dragHandler, dragData, ExecuteEvents.beginDragHandler),
                what + "：beginDrag 必须被 ScrollRect 处理");
            for (int i = 0; i < 40 && scroll.verticalNormalizedPosition > 0.001f; i++)
            {
                Vector2 next = new Vector2(strokeStart.x, strokeStart.y + step * (i + 1));
                if (!PointInside(viewRect, next)) break;
                Assert.IsTrue(PointInside(viewRect, next),
                    what + "：拖拽指针必须始终位于列表视口内 next=" + next + " viewport=" + viewRect);
                dragData.delta = new Vector2(0f, step);
                dragData.position = next;
                Assert.IsTrue(ExecuteEvents.Execute(dragHandler, dragData, ExecuteEvents.dragHandler),
                    what + "：drag 必须被 ScrollRect 处理");
                totalSteps++;
                yield return null;
            }
            Assert.IsTrue(ExecuteEvents.Execute(dragHandler, dragData, ExecuteEvents.endDragHandler),
                what + "：endDrag 必须被 ScrollRect 处理");
            dragData.dragging = false;
            strokes++;
            Canvas.ForceUpdateCanvases();
            yield return null;
        }
        float afterDragNorm = scroll.verticalNormalizedPosition;
        Vector2 afterDragPos = content.anchoredPosition;
        Assert.Less(afterDragNorm, afterScrollNorm - 0.01f,
            what + "：真实拖拽必须继续改变 normalizedPosition afterScroll=" + afterScrollNorm
            + " afterDrag=" + afterDragNorm + " strokes=" + strokes);
        Assert.Greater(Mathf.Abs(afterDragPos.y - dragStartPos.y), 1f,
            what + "：真实拖拽必须改变内容位置 beforeDrag=" + dragStartPos + " afterDrag=" + afterDragPos);
        Assert.LessOrEqual(afterDragNorm, 0.01f,
            what + "：真实拖拽必须能到达末端 after=" + afterDragNorm + " strokes=" + strokes
            + " steps=" + totalSteps + " contentHeight=" + contentHeight + " viewportHeight=" + viewportHeight);
        RealScrollEvidence("REAL-SCROLL-DRAG", what + " beginDrag+Drag+endDrag=True strokes=" + strokes
            + " steps=" + totalSteps + " step=" + step + " start=" + strokeStart + " viewport=" + viewRect
            + " norm=" + dragStartNorm + "→" + afterDragNorm
            + " contentY=" + dragStartPos.y + "→" + afterDragPos.y
            + "（每轮指针始终在列表内，非直接赋 normalizedPosition；按合法溢出继续拖，不缩视口）");

        var lastAfter = LastActiveRow(rowRoots);
        Assert.IsNotNull(lastAfter, what + "：滚动后必须仍存在真实业务末行");
        Rect lastAfterRect = ScreenRectOf(lastAfter);
        Assert.IsTrue(RectInside(lastAfterRect, viewRect),
            what + "：末行滚动后必须完整落在视口内：last=" + lastAfter.name
            + " row=" + lastAfterRect + " viewport=" + viewRect);
        Vector2 lastPoint = ScreenCenter(lastAfter);
        Assert.IsTrue(PointInside(viewRect, lastPoint),
            what + "：末行中心滚动后必须在视口内：" + lastPoint + " viewport=" + viewRect);
        var lastHits = RaycastAllAt(lastPoint);
        AssertTopHitBelongs(lastAfter, what + "·末行", lastPoint, lastHits);
        var clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(lastHits[0].gameObject);
        Assert.IsNotNull(clickHandler,
            what + "：末行必须可 PointerClick（Raycast + handler）\n" + DumpHits(lastHits));
        Assert.IsTrue(lastHits[0].gameObject.transform.IsChildOf(panel),
            what + "：末行命中不得穿透管理模态\n" + DumpHits(lastHits));
        RealScrollEvidence("REAL-SCROLL-LAST-ROW", what + " last=" + lastAfter.name
            + " rowAfter=" + lastAfterRect + " viewport=" + viewRect
            + " center=" + lastPoint + " top=" + lastHits[0].gameObject.name
            + " handler=" + clickHandler.name + " visibleAfter=True");
    }

    /// <summary>
    /// 纯展示标题（TMP_Text）的布局/可见性契约：真实可见、完整位于视口内、文本非空，
    /// 字号与矩形尺寸足以按真实字体排版，并且标题区域没有被管理面板外的图层或可点击控件压住。
    /// authored Prefab 中 Title 是 m_RaycastTarget=0、无 Button/IPointerClickHandler 的静态展示文本，
    /// 因此这里不要求 top raycast 命中标题自身；按钮/输入/页签/滚动行仍由 AssertControlHit 等强断言 top-hit + PointerClick。
    /// </summary>
    private static void AssertDisplayTitle(object view, string fieldName, string what)
    {
        var title = Get(view, fieldName) as TMP_Text;
        Assert.IsNotNull(title, what + "：字段 " + fieldName + " 必须绑定展示标题 TMP_Text");
        var rect = (RectTransform)title.transform;
        AssertInsideViewport(rect, what);

        Assert.IsFalse(string.IsNullOrWhiteSpace(title.text), what + "：展示标题文本不得为空");
        Assert.Greater(title.fontSize, 1f, what + "：展示标题字号必须为正");
        Assert.GreaterOrEqual(rect.rect.height, title.fontSize,
            what + "：标题高度至少容纳一行文字 rect=" + rect.rect + " fontSize=" + title.fontSize);
        Assert.GreaterOrEqual(rect.rect.width, title.fontSize * 2f,
            what + "：标题宽度必须足够显示文字 rect=" + rect.rect);
        Vector2 preferred = title.GetPreferredValues(title.text);
        Assert.Greater(preferred.x, 1f, what + "：标题必须能按真实字体排版（preferredWidth>0）");
        Assert.IsTrue(preferred.x <= rect.rect.width + 1.5f && preferred.y <= rect.rect.height + 1.5f,
            what + "：标题尺寸必须容纳文字而不截断 preferred=" + preferred + " rect=" + rect.rect);

        Assert.IsNull(title.GetComponent<Button>(), what + "：纯展示标题不得是 Button");

        var panel = FindPath(((Component)view).transform, "ModalPanel");
        Assert.IsNotNull(panel, what + "：ModalPanel 必须存在");
        Vector2 point = ScreenCenter(rect);
        var hits = RaycastAllAt(point);
        Assert.Greater(hits.Count, 0, what + "：标题中心必须可 Raycast（UI 层未整体关闭）");
        var top = hits[0].gameObject;
        Assert.IsTrue(top == panel.gameObject || top.transform.IsChildOf(panel),
            what + "：标题区域不得被管理面板之外的图层遮挡\n" + DumpHits(hits));
        Assert.IsNull(ExecuteEvents.GetEventHandler<IPointerClickHandler>(top),
            what + "：标题区域不得被可点击控件压住\n" + DumpHits(hits));
        LayoutEvidence("TITLE", what + " text=\"" + title.text + "\" raycastTarget=" + title.raycastTarget
            + " top=" + top.name + "（纯展示标题：不要求自身 top-hit；可见/视口内/文本/尺寸/遮挡已核）");
    }

    /// <summary>
    /// TMP 已生成文字墨迹在真实 Screen 上的轴对齐矩形（局部 textBounds → 世界 → 屏幕）。
    /// 只用于核对“文字确实被排版出来”，不替代 RectTransform 的布局矩形。
    /// </summary>
    private static Rect TextScreenRect(TMP_Text text)
    {
        Bounds bounds = text.textBounds;
        var canvas = text.GetComponentInParent<Canvas>();
        Camera camera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        Vector3 center = text.transform.TransformPoint(bounds.center);
        Vector3 extents = text.transform.TransformVector(bounds.extents);
        Vector2 a = RectTransformUtility.WorldToScreenPoint(camera, center - extents);
        Vector2 b = RectTransformUtility.WorldToScreenPoint(camera, center + extents);
        return Rect.MinMaxRect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }

    /// <summary>
    /// 人口行“标题真正可见”的独立布局断言（2026-10-26 视觉缺陷回归）：
    /// 既有业务断言只比较 TMP.text 字符串与 Raycast，无法发现“文本已赋值但 TMP Ellipsis
    /// 因矩形高度不足把整行截成 0 个可见字符”。这里逐活动行核对真实渲染结果：
    /// 可见字符数、rendered 尺寸、字号所需行高与矩形高度、颜色 alpha / CanvasRenderer alpha / cull、
    /// 标题墨迹与 detail 墨迹不重叠、标题墨迹在行与滚动视口内。
    /// 不改任何业务标题规则；标题字符串仍按 ExpectedPopulationRowTitles 对照。
    /// </summary>
    private static void AssertPopulationRowTitlesRendered(object management, string what)
    {
        var content = (RectTransform)Get(management, "populationContent");
        Assert.IsNotNull(content, what + "：populationContent 未绑定");
        var scroll = (ScrollRect)Get(management, "populationScroll");
        Assert.IsNotNull(scroll, what + "：populationScroll 未绑定");
        var viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
        Canvas.ForceUpdateCanvases();
        var rows = ClassifyPopulationRows(content).Active;
        var expectedTitles = ExpectedPopulationRowTitles(management);
        Assert.Greater(rows.Count, 0, what + "：人口页必须至少有一条活动业务行用于标题可见性核对");
        Assert.AreEqual(expectedTitles.Count, rows.Count,
            what + "：活动人口行数量必须等于真实投影人数（" + rows.Count + " vs " + expectedTitles.Count + "）");
        CollectionAssert.AreEquivalent(expectedTitles, rows.Select(PopulationRowTitle).ToList(),
            what + "：人口行标题字符串仍必须唯一匹配真实投影（保留原业务断言）");

        Rect viewRect = ScreenRectOf(viewport);
        int checkedRows = 0;
        foreach (var row in rows)
        {
            var view = row.GetComponent(T("ManagementRowView"));
            Assert.IsNotNull(view, what + "：人口行必须绑定 ManagementRowView：" + row.name);
            var label = (TMP_Text)Get(view, "label");
            var detail = (TMP_Text)Get(view, "detail");
            Assert.IsNotNull(label, what + "：人口行 label 未绑定：" + row.name);
            Assert.IsNotNull(detail, what + "：人口行 detail 未绑定：" + row.name);

            label.ForceMeshUpdate(true);
            detail.ForceMeshUpdate(true);
            var labelRect = (RectTransform)label.transform;
            var rowRect = (RectTransform)row;

            Assert.IsTrue(label.gameObject.activeInHierarchy && label.enabled,
                what + "：标题对象必须 active + enabled：" + row.name);
            Assert.IsFalse(string.IsNullOrWhiteSpace(label.text), what + "：标题文本不得为空：" + row.name);
            Assert.Greater(label.fontSize, 1f, what + "：标题字号必须为正：" + row.name);
            Assert.Greater(label.textInfo.characterCount, 0,
                what + "：TMP 标题必须产生可见字符（characterCount>0），实际 0 表示被 Ellipsis 整行截断："
                + row.name + " text=\"" + label.text + "\" rect=" + labelRect.rect);
            int visibleChars = 0;
            for (int i = 0; i < label.textInfo.characterCount; i++)
                if (label.textInfo.characterInfo[i].isVisible) visibleChars++;
            Assert.Greater(visibleChars, 0,
                what + "：TMP 标题必须至少有一个可见字符，实际 0：" + row.name + " text=\"" + label.text + "\"");
            Assert.IsFalse(label.isTextOverflowing,
                what + "：标题不得被 Ellipsis 截断（矩形高度必须容纳真实行高）：" + row.name
                + " rect=" + labelRect.rect + " fontSize=" + label.fontSize);
            Assert.Greater(label.renderedWidth, 1f,
                what + "：标题必须有真实渲染宽度 renderedWidth>0：" + row.name);
            Assert.Greater(label.renderedHeight, 1f,
                what + "：标题必须有真实渲染高度 renderedHeight>0：" + row.name);

            float preferredHeight = label.GetPreferredValues(label.text, labelRect.rect.width, 0f).y;
            Assert.GreaterOrEqual(labelRect.rect.height + 1.5f, preferredHeight,
                what + "：标题矩形高度必须容纳真实字体行高：" + row.name
                + " rectHeight=" + labelRect.rect.height + " preferredHeight=" + preferredHeight
                + " fontSize=" + label.fontSize + " text=\"" + label.text + "\"");
            Assert.GreaterOrEqual(labelRect.rect.height + 1.5f, label.renderedHeight,
                what + "：标题渲染高度不得超过矩形高度：" + row.name
                + " renderedHeight=" + label.renderedHeight + " rectHeight=" + labelRect.rect.height);

            Assert.Greater(label.color.a, 0.5f,
                what + "：标题颜色 alpha 必须可见：" + row.name + " color=" + label.color);
            Assert.Greater(label.canvasRenderer.GetAlpha(), 0.5f,
                what + "：标题 CanvasRenderer alpha 必须可见：" + row.name);
            Assert.IsFalse(label.canvasRenderer.cull,
                what + "：标题 CanvasRenderer 不得被 cull：" + row.name);

            Rect labelInk = TextScreenRect(label);
            Rect detailInk = TextScreenRect(detail);
            Assert.Greater(labelInk.width, 10f, what + "：标题墨迹宽度必须 >10px：" + row.name + " ink=" + labelInk);
            Assert.Greater(labelInk.height, 5f, what + "：标题墨迹高度必须 >5px：" + row.name + " ink=" + labelInk);
            Assert.IsFalse(labelInk.Overlaps(detailInk),
                what + "：标题墨迹不得与 detail 墨迹重叠：" + row.name
                + " titleInk=" + labelInk + " detailInk=" + detailInk);

            Rect rowScreen = ScreenRectOf(rowRect);
            const float tolerance = 2f;
            Assert.IsTrue(labelInk.xMin >= rowScreen.xMin - tolerance && labelInk.xMax <= rowScreen.xMax + tolerance
                          && labelInk.yMin >= rowScreen.yMin - tolerance && labelInk.yMax <= rowScreen.yMax + tolerance,
                what + "：标题墨迹必须落在所属行内：" + row.name
                + " titleInk=" + labelInk + " row=" + rowScreen);

            bool rowFullyInsideViewport = rowScreen.yMin >= viewRect.yMin - 1.5f && rowScreen.yMax <= viewRect.yMax + 1.5f
                                          && rowScreen.xMin >= viewRect.xMin - 1.5f && rowScreen.xMax <= viewRect.xMax + 1.5f;
            if (rowFullyInsideViewport)
            {
                Assert.IsTrue(labelInk.xMin >= viewRect.xMin - tolerance && labelInk.xMax <= viewRect.xMax + tolerance
                              && labelInk.yMin >= viewRect.yMin - tolerance && labelInk.yMax <= viewRect.yMax + tolerance,
                    what + "：完整位于滚动视口内的行，其标题墨迹不得被视口裁切：" + row.name
                    + " titleInk=" + labelInk + " viewport=" + viewRect);
                checkedRows++;
            }
            LayoutEvidence("ROW-TITLE-RENDER", what + " row=" + row.name
                + " text=\"" + label.text + "\" chars=" + label.textInfo.characterCount + " visible=" + visibleChars
                + " rendered=" + label.renderedWidth + "x" + label.renderedHeight
                + " preferredH=" + preferredHeight + " rect=" + labelRect.rect
                + " alpha=" + label.color.a + " crAlpha=" + label.canvasRenderer.GetAlpha() + " cull=" + label.canvasRenderer.cull
                + " titleInk=" + labelInk + " detailInk=" + detailInk + " overlapsDetail=" + labelInk.Overlaps(detailInk)
                + " rowFullyInsideViewport=" + rowFullyInsideViewport);
        }
        Assert.Greater(checkedRows, 0, what + "：必须至少有一行完整位于视口内并完成标题墨迹裁切核对");
        LayoutEvidence("ROW-TITLE-VISIBLE", what + " rows=" + rows.Count + " checkedRows=" + checkedRows
            + "（TMP 可见字符/渲染尺寸/行高/alpha/cull/与 detail 不重叠/视口裁切，非仅字符串）");
    }

    private static IEnumerator AssertTabLayout(object management, string tab, string what)
    {
        AssertDisplayTitle(management, "title", what + "·标题");
        AssertFieldControl(management, "closeButton", what + "·关闭", true);
        AssertFieldControl(management, "saveButton", what + "·保存", true);
        foreach (var command in new[] { "assignButton", "removeButton", "addConstructionButton", "removeConstructionButton" })
            AssertFieldControl(management, command, what + "·命令区/" + command, false);

        var panel = FindPath(((Component)management).transform, "ModalPanel");
        Assert.IsNotNull(panel, what + "：ModalPanel 必须存在");
        if (tab == "Population")
        {
            AssertFieldControl(management, "nameSearch", what + "·搜索", true);
            var filters = (Button[])Get(management, "statusFilterButtons");
            Assert.AreEqual(3, filters.Length, what + "：三个状态过滤按钮必须完整绑定");
            foreach (var filter in filters)
                AssertControlHit((RectTransform)filter.transform, what + "·过滤/" + filter.name, true);
            yield return AssertScrollReachable((ScrollRect)Get(management, "populationScroll"), panel,
                new[] { (RectTransform)Get(management, "populationContent") },
                (TMP_Text)Get(management, "populationEmpty"), what + "·人口滚动");
            // 既有滚动/可达断言之后追加“标题真正渲染”核对：不替换、不削弱原断言。
            AssertPopulationRowTitlesRendered(management, what + "·人口行标题渲染");
        }
        else if (tab == "Workplaces")
        {
            yield return AssertScrollReachable((ScrollRect)Get(management, "workplaceScroll"), panel,
                new[]
                {
                    (RectTransform)Get(management, "workplaceContent"),
                    (RectTransform)Get(management, "constructionContent")
                },
                (TMP_Text)Get(management, "workplaceEmpty"), what + "·岗位滚动");
        }
        else
        {
            yield return AssertScrollReachable((ScrollRect)Get(management, "resourceScroll"), panel,
                new[] { (RectTransform)Get(management, "resourceContent") },
                (TMP_Text)Get(management, "resourceEmpty"), what + "·资源滚动");
        }
    }

    // ==================== 用例 12：1920 单分辨率管理布局 / 三页签 / 真实滚动可达 ====================
    // 制作人决定 01a53eb8 已取消 1280：本用例不再请求 / 循环 / 恢复分辨率，
    // 只对宿主当前真实 Screen 1920x1080 做长期布局回归；不用 GameView API / Preferences 改尺寸，
    // 不以 RectTransform 缩放或截图缩放冒充分辨率。最新单例结果待宿主真实运行。

    [UnityTest]
    public IEnumerator ManagementLayout_At1920_KeepsTabsCommandsAndScrollReachable()
    {
        // 先严格确认宿主真实运行尺寸就是 1920x1080；不满足即明确失败，不请求尺寸、不兜底改分辨率。
        AssertRealScreenAt1920x1080("管理布局开始前");
        int width = Screen.width;
        int height = Screen.height;
        LayoutEvidence("LAYOUT-START", "realScreen=" + width + "x" + height + " mode=" + Screen.fullScreenMode
            + " resolutionSwitch=none（1280 双尺寸切换已由制作人 01a53eb8 取消）");

        string selected = FirstSnapshotPersonIdFromHost();
        Call(Get(_host, "Selection"), "SelectPerson", selected);
        OpenManagementThroughNavigation("布局专项·管理 @" + width + "x" + height);
        yield return null;
        yield return null;

        var management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        Assert.AreEqual("Management", Surface(), "真实导航必须进入 Management Surface @" + width + "x" + height);
        Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"),
            "管理必须计入统一模态判定 @" + width + "x" + height);
        AssertCanvasMatchesScreen(management, "管理布局 @" + width + "x" + height);

        var tabs = (Button[])SerializedArray(management, "tabButtons");
        Assert.AreEqual(3, tabs.Length, "固定三页签必须完整绑定 @" + width + "x" + height);
        foreach (var tab in tabs)
            AssertInsideViewport((RectTransform)tab.transform,
                "布局·页签/" + tab.name + " @" + width + "x" + height);

        AssertManagementModalBlocksWorld(management, "管理布局 @" + width + "x" + height);
        AssertTopHudHit(_shell, "管理布局·TopHUD @" + width + "x" + height);

        foreach (var tab in new[] { "Population", "Workplaces", "Resources" })
        {
            ClickChild(management, "ModalPanel/Tab_" + tab, "布局页签·" + tab + " @" + width + "x" + height);
            yield return null;
            Assert.AreEqual(tab, Get(management, "ActiveTab").ToString(),
                "真实点击必须切到页签 " + tab + " @" + width + "x" + height);
            Canvas.ForceUpdateCanvases();
            yield return null;
            yield return CaptureLayoutShot(width, height, tab.ToLowerInvariant());
            yield return AssertTabLayout(management, tab, "管理布局 @" + width + "x" + height + "·" + tab);
        }
        LayoutEvidence("LAYOUT-OK", "管理布局 @" + width + "x" + height
            + "：三页签已逐个程序化点击，标题/搜索/过滤/关闭/保存/命令区视口内且命中正确 UI，"
            + "TopHUD 可命中，World 射线被管理模态阻断，滚动为真实 EventSystem 事件或如实无需滚动");

        // 本用例全程不请求分辨率；结束时再次核验仍是宿主真实 1920x1080（不是恢复，是未改尺寸的契约）。
        AssertRealScreenAt1920x1080("管理布局结束后");
    }

    private void AssertWorldInputsUnchanged(string selected, string clockSpeed, string what)
    {
        Assert.AreEqual(selected, SelectionId(), what + "：滚动 / 行点击不得改变真实世界 Selection");
        Assert.AreEqual(clockSpeed, ClockSpeed(), what + "：滚动 / 行点击不得改变 Clock Speed（Pause 必须保持）");
        Assert.IsTrue(SimulationActive(), what + "：滚动 / 行点击不得关闭模拟");
    }

    // ==================== 用例 13：真实建筑业务行 overflow 滚动（滚轮 + 拖拽）与末行可达 ====================

    /// <summary>
    /// 真实建筑业务行专项（只使用既有产品入口与真实人物，不注入假建筑 / 假人物、不改配置 Authority、
    /// 不缩小 viewport / 放大行、不克隆假行）：
    ///   1) 在合法空位放置 7 座 Farm；前 6 座各由 1 名真实人物通过 TryAddConstructionWorker 加入施工，
    ///      真实 AdvanceTo 14400 后全部 Completed（6 条真实建筑功能岗位行）；
    ///   2) 第 7 座保留 UnderConstruction，并在推进后由真实人物加入施工（1 条真实施工行）；
    ///   3) 打开真实管理页 → 职责与场所，断言 scroll.content 真实 contentHeight > viewportHeight；
    ///   4) 用真实 EventSystem scroll 与 beginDrag/drag/endDrag（指针始终在列表视口内）证明
    ///      content.anchoredPosition / verticalNormalizedPosition 真实改变，末行由不可见变可见；
    ///   5) 末行 Raycast + PointerClick 分别对应真实 WorkplaceId / BuildingInstanceId；
    ///      世界 Selection 与 Clock Pause 不被滚动 / 行选择改变；
    ///   6) 对真实 Workplace 与 Construction 业务行标题做 TMP 可见字符 / rendered / preferred 高度 /
    ///      与 detail 墨迹不重叠核对，并记录实际覆盖类（Resource 未覆盖，不伪造）；
    ///   7) 2026-10-26 职责详情可读性强化：6 条真实岗位行 detail 首行必须是容量 / 绑定 / 实际劳动 / 现场，
    ///      第 2 行是完整 WorkplaceId；逐字符核对 isVisible 与字符墨迹落行（行在视口内时还落视口），
    ///      并核对 1 条真实施工行 detail 首行的真实进度可读——不是只做字符串包含。
    /// 不直接赋 verticalNormalizedPosition 冒充输入，不缩小 viewport、不放大行高。
    /// </summary>
    [UnityTest]
    public IEnumerator ManagementScroll_RealBuildingRows_ReachLastRowWithoutWorldInput()
    {
        IsolateActivityOpportunitiesForFixture("REAL-SCROLL building rows fixture");
        Assert.AreEqual("Paused", ClockSpeed(), "夹具必须从 Clock 暂停开始以保证确定性");

        // 正式默认世界已含真实实例建筑（Farm / Kitchen 岗位 + 住房）；
        // 本用例只追加 7 座 Farm，计数以“开工真实基线 + 新增 6 条完工”为准，不假设空世界。
        object baselineProjection = ReadManagementProjection();
        Assert.IsNotNull(baselineProjection, "开工管理投影必须可读");
        int baselineWorkplaces = ((IEnumerable)Get(baselineProjection, "Workplaces")).Cast<object>().Count();
        int baselineResources = ((IEnumerable)Get(Get(baselineProjection, "Resources"), "Workplaces")).Cast<object>().Count();
        Assert.GreaterOrEqual(baselineWorkplaces, 1, "正式默认世界必须已有真实实例岗位");
        Evidence("REAL-SCROLL", "baseline workplaces=" + baselineWorkplaces + " resources=" + baselineResources
            + " (formal default instance world, not an empty world)");

        var persons = HostPersonIds();
        Assert.GreaterOrEqual(persons.Count, 6, "真实世界至少需要 6 名合法人物才能并行完成 6 座建筑");
        string selected = persons[0];
        Call(Get(_host, "Selection"), "SelectPerson", selected);

        // 合法空位：建造范围 X[-12,12]、Y[-10,8]；Farm 占地 3×2，y=-4 一行相邻 delta=3 不重叠。
        var farmIds = new List<string>();
        foreach (float x in new[] { -10.5f, -7.5f, -4.5f, -1.5f, 1.5f, 4.5f, 7.5f })
            farmIds.Add(PlaceBuilding("building_farm", x));
        Assert.AreEqual(7, farmIds.Count, "本夹具必须通过真实建造入口生成 7 座 Farm");
        Assert.AreEqual(7, farmIds.Distinct().Count(), "每座 Farm 必须是独立 BuildingInstanceId");

        for (int i = 0; i < 6; i++)
        {
            var add = Bridge("TryAddConstructionWorker", farmIds[i], persons[i]);
            Assert.AreEqual("Success", add.ToString(),
                "真实人物 " + persons[i] + " 必须能合法加入 Farm " + farmIds[i] + " 施工");
        }
        Evidence("REAL-SCROLL", "placed=7 batch1=6 participants=[" + string.Join(",", persons.Take(6))
            + "] farms=[" + string.Join(",", farmIds.Take(6)) + "]");

        AdvanceTo(14400);
        Call(_host, "RefreshNow");
        for (int i = 0; i < 6; i++)
            Assert.AreEqual("Completed", BuildingState(farmIds[i]),
                "Farm " + farmIds[i] + " 必须在真实 14400 分钟推进后 Completed");
        Assert.AreEqual("UnderConstruction", BuildingState(farmIds[6]),
            "第 7 座 Farm 必须保留真实施工行（本夹具至少一个 Construction 行）");

        var constructionAdd = Bridge("TryAddConstructionWorker", farmIds[6], persons[0]);
        Assert.AreEqual("Success", constructionAdd.ToString(),
            "推进后真实人物 " + persons[0] + " 必须能合法加入第 7 座 Farm 施工");
        Assert.AreEqual("UnderConstruction", BuildingState(farmIds[6]), "第 7 座 Farm 必须仍在施工");
        object project = ActiveProject();
        Assert.IsNotNull(project, "第 7 座 Farm 加入施工后必须存在真实 Active Project");
        Assert.AreEqual(1, Strings(Get(project, "ParticipantPersonIds")).Count,
            "第 7 座 Farm 的真实施工参与者必须为 1 人");
        Evidence("REAL-SCROLL", "completed=6; construction=" + farmIds[6] + " worker=" + persons[0]
            + " progress=" + Get(project, "Progress") + " labor=" + Get(project, "LaborRemainder"));

        Assert.AreEqual("Paused", ClockSpeed(), "真实建造 / 推进后 Clock 必须仍为 Paused");
        Call(Get(_host, "Selection"), "SelectPerson", selected);
        string selectedBefore = SelectionId();
        string clockBefore = ClockSpeed();

        OpenManagementThroughNavigation("真实建筑业务行滚动专项·管理");
        yield return null;
        yield return null;
        var management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        Assert.AreEqual("Management", Surface(), "真实导航必须进入 Management Surface");
        ShowWorkplacesPage(management);
        yield return null;

        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "Management.CurrentSnapshot 必须已读取");
        var workplaces = ((IEnumerable)Get(snapshot, "Workplaces")).Cast<object>().ToList();
        var constructionRows = ((IEnumerable)Get(snapshot, "Construction")).Cast<object>().ToList();
        Assert.AreEqual(baselineWorkplaces + 6, workplaces.Count,
            "默认实例岗位 + 新增 6 座 Completed Farm 各 1 条真实功能岗位行（不得有测试注入的额外行）");
        Assert.AreEqual(baselineWorkplaces + 6, workplaces.Count(w => Get(w, "Kind").ToString() == "BuildingFunction"),
            "本夹具的岗位行必须全部来自 Completed Building Function");
        Assert.AreEqual(1, constructionRows.Count, "必须恰有 1 条真实施工行");
        Assert.AreEqual(farmIds[6], (string)Get(constructionRows[0], "BuildingInstanceId"),
            "施工行必须对应第 7 座真实 Farm 实例");
        Assert.AreEqual(1, Convert.ToInt32(Get(constructionRows[0], "WorkerCount")),
            "施工行必须显示 1 名真实工人");
        foreach (string farmId in farmIds.Take(6))
        {
            object row = workplaces.FirstOrDefault(w =>
                string.Equals((string)Get(w, "BuildingInstanceId"), farmId, StringComparison.Ordinal));
            Assert.IsNotNull(row, "Completed Farm " + farmId + " 必须有真实岗位行");
            string workplaceId = (string)Get(row, "WorkplaceId");
            Assert.IsTrue(workplaceId.Contains(farmId), "岗位 ID 必须绑定真实建筑实例：" + workplaceId);
        }
        Evidence("REAL-SCROLL", "snapshot workplaces=" + workplaces.Count + " construction=" + constructionRows.Count
            + " constructionBuilding=" + Get(constructionRows[0], "BuildingInstanceId"));

        var scroll = (ScrollRect)Get(management, "workplaceScroll");
        Assert.IsNotNull(scroll, "workplaceScroll 必须绑定");
        var workplaceContent = (RectTransform)Get(management, "workplaceContent");
        var constructionContent = (RectTransform)Get(management, "constructionContent");
        Assert.IsNotNull(workplaceContent, "workplaceContent 必须绑定");
        Assert.IsNotNull(constructionContent, "constructionContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)scroll.content);
        yield return null;
        Canvas.ForceUpdateCanvases();
        Assert.AreEqual(workplaces.Count, workplaceContent.childCount,
            "岗位行数必须等于真实快照岗位数（不是克隆假行）");
        Assert.AreEqual(constructionRows.Count, constructionContent.childCount,
            "施工行数必须等于真实快照施工数");

        var panel = FindPath(((Component)management).transform, "ModalPanel");
        Assert.IsNotNull(panel, "ModalPanel 必须存在");
        yield return RealScrollAndDragToEnd(scroll, panel,
            new[] { workplaceContent, constructionContent }, "真实建筑业务行 overflow 滚动");

        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
        var viewport = scroll.viewport != null ? scroll.viewport : (RectTransform)scroll.transform;
        var workplaceRowsNow = workplaceContent.Cast<Transform>()
            .Where(row => row != null && row.gameObject.activeSelf).ToList();
        var constructionRowsNow = constructionContent.Cast<Transform>()
            .Where(row => row != null && row.gameObject.activeSelf).ToList();
        Assert.AreEqual(workplaces.Count, workplaceRowsNow.Count, "滚动后真实岗位行数必须保持不变");
        Assert.AreEqual(constructionRows.Count, constructionRowsNow.Count, "滚动后真实施工行数必须保持不变");
        Transform lastWorkplaceRow = workplaceRowsNow.Last();
        Transform lastConstructionRow = constructionRowsNow.Last();
        AssertBusinessRowTitleRendered(lastWorkplaceRow, viewport, "岗位业务行标题·末行");
        AssertBusinessRowTitleRendered(lastConstructionRow, viewport, "施工业务行标题·末行");

        // 2026-10-26 职责详情关键数值可读性强化：不只字符串包含。
        // 6 条真实完工岗位行的 detail 首行必须是容量 / 绑定 / 实际劳动 / 现场，第 2 行必须是完整 WorkplaceId，
        // 逐字符核对 isVisible 与字符真实墨迹落行（完整在视口内时还落视口）；1 条真实施工行必须可读到真实进度。
        int workplaceDetailChecked = 0;
        for (int i = 0; i < workplaceRowsNow.Count; i++)
        {
            AssertWorkplaceDetailKeyValuesRendered(workplaceRowsNow[i], workplaces[i], viewport,
                "岗位业务行 detail·第" + (i + 1) + "行");
            workplaceDetailChecked++;
        }
        Assert.AreEqual(workplaces.Count, workplaceDetailChecked,
            "必须逐行核对全部真实岗位行的 detail（不得只抽样一行）");
        Assert.AreEqual(workplaces.Count, workplaces.Count(w => !string.IsNullOrEmpty((string)Get(w, "WorkplaceId"))),
            "全部岗位行的 WorkplaceId 必须非空（身份行不得被删除）");
        AssertConstructionDetailProgressRendered(lastConstructionRow, constructionRows[0], snapshot, viewport,
            "施工业务行 detail·进度/工人/现场");
        AssertWorldInputsUnchanged(selectedBefore, clockBefore, "滚动后");

        yield return CaptureRealScrollShot("workplaces-overflow-last-row");

        object lastWorkplaceModel = workplaces.Last();
        string expectedWorkplaceId = (string)Get(lastWorkplaceModel, "WorkplaceId");
        string expectedWorkplaceBuilding = (string)Get(lastWorkplaceModel, "BuildingInstanceId");
        Assert.IsTrue(expectedWorkplaceId.Contains(expectedWorkplaceBuilding),
            "末岗位的 WorkplaceId 必须对应真实 BuildingInstanceId：" + expectedWorkplaceId);
        RealPointerClick((RectTransform)lastWorkplaceRow, "岗位末行（真实滚动后）");
        yield return null;
        Assert.AreEqual(expectedWorkplaceId, (string)Get(management, "SelectedWorkplaceId"),
            "末岗位 Raycast + PointerClick 必须选中真实 WorkplaceId");
        Assert.AreEqual("Management", Surface(), "岗位行点击不得离开管理页");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "岗位行点击后管理页必须仍可见");
        AssertWorldInputsUnchanged(selectedBefore, clockBefore, "岗位末行点击后");
        Evidence("REAL-SCROLL-CLICK", "workplace=" + expectedWorkplaceId
            + " building=" + expectedWorkplaceBuilding + " selected=True");

        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
        constructionRowsNow = constructionContent.Cast<Transform>()
            .Where(row => row != null && row.gameObject.activeSelf).ToList();
        Assert.AreEqual(constructionRows.Count, constructionRowsNow.Count, "岗位点击后真实施工行数必须保持");
        Transform constructionRowAfter = constructionRowsNow.Last();
        AssertBusinessRowTitleRendered(constructionRowAfter, viewport, "施工业务行标题·点击岗位后");
        RealPointerClick((RectTransform)constructionRowAfter, "施工末行（真实滚动后）");
        yield return null;
        Assert.AreEqual(farmIds[6], (string)Get(management, "SelectedConstructionBuildingId"),
            "施工末行 Raycast + PointerClick 必须对应真实 BuildingInstanceId");
        Assert.AreEqual("Management", Surface(), "施工行点击不得离开管理页");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "施工行点击后管理页必须仍可见");
        AssertWorldInputsUnchanged(selectedBefore, clockBefore, "施工末行点击后");
        Evidence("REAL-SCROLL-CLICK", "construction=" + farmIds[6] + " selected=True;"
            + " worldSelection=" + SelectionId() + " clock=" + ClockSpeed());

        // ===== 资源页：本夹具的 6 座已完工 Farm 必须产生 6 条真实非空资源业务行 =====
        // 不再以“资源页为空、无法构造”作为未覆盖理由：同一真实世界已有 Farm 实例名额与劳动事实。
        ShowResourcesPage(management);
        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
        object refreshedSnapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(refreshedSnapshot, "切换资源页后 CurrentSnapshot 必须已刷新");
        object resources = Get(refreshedSnapshot, "Resources");
        Assert.IsNotNull(resources, "资源投影必须绑定");
        Assert.IsTrue((bool)Get(resources, "HasWorld"), "资源权威必须就绪（不得假称不可读）");
        var resourceModels = ((IEnumerable)Get(resources, "Workplaces")).Cast<object>().ToList();
        Assert.AreEqual(baselineResources + 6, resourceModels.Count,
            "默认实例资源行 + 6 座已完工 Farm 必须产生真实 Farm 资源行（非空业务行，不是空态）");
        var resourceScroll = (ScrollRect)Get(management, "resourceScroll");
        var resourceContent = (RectTransform)Get(management, "resourceContent");
        Assert.IsNotNull(resourceScroll, "resourceScroll 必须绑定");
        Assert.IsNotNull(resourceContent, "resourceContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)resourceScroll.content);
        yield return null;
        Canvas.ForceUpdateCanvases();
        var resourceRowsNow = resourceContent.Cast<Transform>()
            .Where(row => row != null && row.gameObject.activeSelf).ToList();
        Assert.AreEqual(resourceModels.Count, resourceRowsNow.Count,
            "资源行数必须等于真实资源投影行数（不是克隆假行）");
        Assert.AreEqual(resourceModels.Count, resourceRowsNow.Count, "资源页必须存在全部真实活动行");
        var resourceViewport = resourceScroll.viewport != null
            ? resourceScroll.viewport
            : (RectTransform)resourceScroll.transform;
        AssertBusinessRowTitleRendered(resourceRowsNow[0], resourceViewport, "资源业务行标题·首行");
        foreach (var model in resourceModels)
        {
            Assert.IsFalse(string.IsNullOrEmpty((string)Get(model, "BuildingInstanceId")),
                "资源行建筑实例身份不得为空");
            Assert.IsFalse(string.IsNullOrEmpty((string)Get(model, "WorkplaceId")),
                "资源行 WorkplaceId 不得为空");
        }
        for (int i = 0; i < resourceRowsNow.Count; i++)
            AssertResourceDetailFactsRendered(resourceRowsNow[i], resourceModels[i], resourceViewport,
                "资源业务行 detail·第" + (i + 1) + "行");
        yield return RealScrollAndDragToEnd(resourceScroll, panel,
            new[] { resourceContent }, "真实资源业务行 overflow 滚动");
        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
        resourceRowsNow = resourceContent.Cast<Transform>()
            .Where(row => row != null && row.gameObject.activeSelf).ToList();
        Assert.AreEqual(resourceModels.Count, resourceRowsNow.Count, "资源滚动后真实行数必须保持");
        Transform lastResourceRow = resourceRowsNow.Last();
        AssertBusinessRowTitleRendered(lastResourceRow, resourceViewport, "资源业务行标题·末行");
        AssertResourceDetailFactsRendered(lastResourceRow, resourceModels.Last(), resourceViewport,
            "资源业务行 detail·末行滚动后");
        yield return CaptureRealScrollShot("resources-real-rows");
        string expectedResourceWorkplace = (string)Get(resourceModels.Last(), "WorkplaceId");
        RealPointerClick((RectTransform)lastResourceRow, "资源末行（真实滚动后）");
        yield return null;
        Assert.AreEqual(expectedResourceWorkplace, (string)Get(management, "SelectedWorkplaceId"),
            "资源行点击必须选中同一真实 WorkplaceId");
        Assert.AreEqual("Workplaces", Get(management, "ActiveTab").ToString(),
            "资源行点击必须定位到职责与场所的同一行");
        AssertWorldInputsUnchanged(selectedBefore, clockBefore, "资源末行点击后");
        Evidence("REAL-SCROLL-CLICK", "resourceWorkplace=" + expectedResourceWorkplace
            + " rows=" + resourceModels.Count + " selected=True;tab=Workplaces");

        // ===== 人口页：真实人口行的职责 / 现时执行 / 临时承诺 / 饥饿必须完整可读 =====
        ShowPopulationPage(management);
        yield return null;
        Canvas.ForceUpdateCanvases();
        yield return null;
        Canvas.ForceUpdateCanvases();
        object populationSnapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(populationSnapshot, "切换人口页后 CurrentSnapshot 必须已刷新");
        var personModels = ((IEnumerable)Get(populationSnapshot, "Persons")).Cast<object>().ToList();
        Assert.GreaterOrEqual(personModels.Count, 6, "真实人口页必须至少包含夹具中的 6 名真实人物");
        var populationScroll = (ScrollRect)Get(management, "populationScroll");
        var populationContent = (RectTransform)Get(management, "populationContent");
        Assert.IsNotNull(populationScroll, "populationScroll 必须绑定");
        Assert.IsNotNull(populationContent, "populationContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)populationScroll.content);
        yield return null;
        Canvas.ForceUpdateCanvases();
        var populationRowsNow = populationContent.Cast<Transform>()
            .Where(row => row != null && row.gameObject.activeSelf).ToList();
        Assert.AreEqual(personModels.Count, populationRowsNow.Count,
            "人口行数必须等于真实人口投影数（不是克隆假行）");
        var populationViewport = populationScroll.viewport != null
            ? populationScroll.viewport
            : (RectTransform)populationScroll.transform;
        for (int i = 0; i < populationRowsNow.Count; i++)
            AssertPersonDetailFactsRendered(populationRowsNow[i], personModels[i], populationViewport,
                "人口业务行 detail·第" + (i + 1) + "行");
        AssertBusinessRowTitleRendered(populationRowsNow[0], populationViewport, "人口业务行标题·首行");
        yield return CaptureRealScrollShot("population-detail-facts");
        RealScrollEvidence("REAL-SCROLL-COVERAGE",
            "Workplace=True Construction=True Resource=True Population=True"
            + " workplaceDetail=" + workplaces.Count + " constructionDetail=1 workerNames=1 resourceDetail=" + resourceModels.Count
            + " populationDetail=" + personModels.Count
            + "（全部行来自真实世界快照；无伪造行、无假数据）");
    }

    // ==================== 用例 2：模态 Raycast / TopHUD 时钟 / 手动 Pause 持久 ====================

    [UnityTest]
    public IEnumerator ManagementModal_RaycastBlocksWorldAndNavigation_TopHudControlsClock()
    {
        string selected = FirstSnapshotPersonIdFromHost();
        Call(Get(_host, "Selection"), "SelectPerson", selected);
        Assert.AreEqual("Paused", ClockSpeed(), "夹具必须从暂停开始以保证确定性");

        OpenManagementThroughNavigation();
        yield return null;
        var management = Management();
        var blocker = FindPath(((Component)management).transform, "ModalBlocker");
        var panel = FindPath(((Component)management).transform, "ModalPanel");
        Assert.IsNotNull(blocker, "ModalBlocker 必须存在");
        Assert.IsNotNull(panel, "ModalPanel 必须存在");

        // 背景点：ModalBlocker 左侧留白（面板 x=24 之外），应命中真实遮罩。
        var blockerRect = (RectTransform)blocker;
        Vector2 background = ScreenPointAt(blockerRect, new Vector2(blockerRect.rect.xMin + 12f, 0f));
        var backgroundHits = RaycastAllAt(background);
        Assert.Greater(backgroundHits.Count, 0, "管理模态背景点必须可被 Raycast 命中");
        var top = backgroundHits[0].gameObject;
        Assert.IsTrue(top == blocker.gameObject || top.transform.IsChildOf(blocker) || top.transform.IsChildOf(panel),
            "背景点 top hit 必须是 Blocker/面板（不能穿透到 World/Town）\n" + DumpHits(backgroundHits));
        Assert.IsNull(ExecuteEvents.GetEventHandler<IPointerClickHandler>(backgroundHits[0].gameObject),
            "模态背景不得再产生按钮点击处理器\n" + DumpHits(backgroundHits));

        // 世界点击守卫：真实产品入口 HandleWorldClick 不得改动 Selection / Surface。
        Call(_host, "HandleWorldClick", background);
        Assert.AreEqual("Management", Surface(), "管理模态时世界点击不得切换 Surface");
        Assert.AreEqual(selected, SelectionId(), "管理模态时世界点击不得改变 Selection");

        // 底部导航 / 建造：真实 Raycast 覆盖，不得打开建造或改变 Surface。
        var navigate6 = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate6");
        Assert.IsNotNull(navigate6, "底部导航“建造”必须存在");
        var navPoint = ScreenCenter((RectTransform)navigate6);
        var navHits = RaycastAllAt(navPoint);
        Assert.Greater(navHits.Count, 0);
        Assert.IsTrue(navHits[0].gameObject == blocker.gameObject
                      || navHits[0].gameObject.transform.IsChildOf(blocker)
                      || navHits[0].gameObject.transform.IsChildOf(panel),
            "管理模态必须覆盖底部导航（不能穿透）\n" + DumpHits(navHits));
        Assert.IsNull(ExecuteEvents.GetEventHandler<IPointerClickHandler>(navHits[0].gameObject),
            "管理模态时底部导航不得可点\n" + DumpHits(navHits));
        Assert.IsFalse((bool)Get(Get(_shell, "Build"), "IsOpen"), "管理模态时不得打开建造目录");
        Assert.AreEqual("Management", Surface(), "底部导航尝试不得改变 Surface");

        // TopHUD Pause：真实 Raycast 命中真实按钮 + 真实 Button 事件。
        var pause = FindPath(((Component)_shell).transform, "TopHUD/Pause");
        Assert.IsNotNull(pause, "TopHUD Pause 必须存在");
        var pauseButton = pause.GetComponent<Button>();
        Assert.IsNotNull(pauseButton, "TopHUD Pause 必须是 Button");
        Assert.IsTrue(pauseButton.interactable, "管理模态时 TopHUD Pause 不得被禁用");
        RealPointerClick((RectTransform)pause, "TopHUD·暂停/继续");
        yield return null;
        Assert.AreNotEqual("Paused", ClockSpeed(), "真实点击 Pause 后 Clock 必须从暂停恢复");
        Assert.AreEqual("Management", Surface(), "TopHUD 操作不得关闭管理");

        // TopHUD Speed：真实点击必须改变 Clock 速度。
        var speed = FindPath(((Component)_shell).transform, "TopHUD/CurrentSpeed");
        Assert.IsNotNull(speed, "TopHUD CurrentSpeed 必须存在");
        var speedButton = speed.GetComponent<Button>();
        Assert.IsNotNull(speedButton, "TopHUD CurrentSpeed 必须是 Button");
        Assert.IsTrue(speedButton.interactable, "管理模态时 TopHUD Speed 不得被禁用");
        double multiplierBefore = ClockMultiplier();
        RealPointerClick((RectTransform)speed, "TopHUD·速度");
        yield return null;
        Assert.Greater(ClockMultiplier(), multiplierBefore, "真实点击 Speed 后 Clock 速度必须变化");
        Assert.AreEqual("Management", Surface(), "TopHUD 速度操作不得关闭管理");

        // 手动 Pause → 切页 / 关闭 / 重开管理保持 Pause，且模拟仍 Active。
        RealPointerClick((RectTransform)pause, "TopHUD·暂停（手动）");
        yield return null;
        Assert.AreEqual("Paused", ClockSpeed(), "TopHUD 必须能手动暂停");
        ClickChild(management, "ModalPanel/Tab_Resources", "页签·资源");
        yield return null;
        Assert.AreEqual("Paused", ClockSpeed(), "切页不得恢复 Clock");
        Assert.IsTrue(SimulationActive(), "切页不得 SetSimulationActive(false)");

        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "ESC 必须回 World");
        Assert.IsTrue(SimulationActive(), "关闭管理不得 SetSimulationActive(false)");
        Assert.AreEqual("Paused", ClockSpeed(), "关闭管理不得恢复 Clock");

        OpenManagementThroughNavigation("底部导航·管理（Pause 保持验证）");
        yield return null;
        Assert.AreEqual("Management", Surface());
        Assert.AreEqual("Paused", ClockSpeed(), "重开管理必须保持手动 Pause");
        Assert.IsTrue(SimulationActive(), "重开管理不得 SetSimulationActive(false)");
        Assert.AreEqual(selected, SelectionId(), "开关管理不得清除 Selection");
    }

    // ==================== 用例 3：关闭按钮与 ESC 同一 World 返回 / 旧 Town 输入恢复 ====================

    [UnityTest]
    public IEnumerator CloseButtonAndEscape_ReturnWorldKeepSelection_AndRestoreTownInput()
    {
        string selected = FirstSnapshotPersonIdFromHost();
        Call(Get(_host, "Selection"), "SelectPerson", selected);

        var townRoot = (GameObject)Get(_town, "canvasRoot");
        var existingGroup = townRoot.GetComponent<CanvasGroup>();
        bool interactableBefore = existingGroup == null || existingGroup.interactable;
        bool blocksBefore = existingGroup == null || existingGroup.blocksRaycasts;

        OpenManagementThroughNavigation();
        yield return null;
        var management = Management();
        var group = townRoot.GetComponent<CanvasGroup>();
        Assert.IsNotNull(group, "管理模态必须通过既有 TownUI.canvasRoot 的 CanvasGroup 阻断旧 Town 交互");
        Assert.IsFalse(group.interactable, "管理打开时旧 Town 交互必须禁用");
        Assert.IsFalse(group.blocksRaycasts, "管理打开时旧 Town 射线必须禁用");

        // 关闭按钮：真实 Raycast + 真实 Button 事件。
        ClickChild(management, "ModalPanel/Close", "管理·关闭");
        yield return null;
        string surfaceAfterClose = Surface();
        string selectionAfterClose = SelectionId();
        Assert.AreEqual("World", surfaceAfterClose, "管理关闭按钮必须回 World");
        Assert.AreEqual(selected, selectionAfterClose, "管理关闭按钮必须保留选人");
        Assert.AreEqual(interactableBefore, group.interactable, "关闭管理必须恢复旧 Town interactable");
        Assert.AreEqual(blocksBefore, group.blocksRaycasts, "关闭管理必须恢复旧 Town blocksRaycasts");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "关闭后管理页必须隐藏");

        // ESC：同一产品入口，同一 World 返回。
        OpenManagementThroughNavigation("底部导航·管理（ESC 验证）");
        yield return null;
        Assert.IsFalse(group.interactable, "重开管理必须再次阻断旧 Town");
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual(surfaceAfterClose, Surface(), "ESC 与关闭按钮必须返回同一 World Surface");
        Assert.AreEqual(selectionAfterClose, SelectionId(), "ESC 与关闭按钮必须保留同一 Selection");
        Assert.AreEqual(interactableBefore, group.interactable, "ESC 关闭必须恢复旧 Town interactable");
        Assert.AreEqual(blocksBefore, group.blocksRaycasts, "ESC 关闭必须恢复旧 Town blocksRaycasts");
        Assert.IsFalse((bool)Get(Get(_shell, "Build"), "IsOpen"), "ESC 关闭不得打开建造");
        Assert.IsTrue(SimulationActive(), "关闭/ESC 不得 SetSimulationActive(false)");
    }

    // ==================== 用例 3B：合成输入阳性对照与真实 Host 模态阻断开镜 ====================

    /// <summary>
    /// 本用例只替换 Unity legacy Input 的采样数据（Editor-only internal、实例级、finally 清理），
    /// 仍由真实 ProductHomeWorldHost.Update → HomeWorldCamera.ReadInput(overUI,...) 消费；
    /// 不用直接 Pan/Zoom、也不用无输入的 ReadInput(true) 冒充输入门禁。
    /// 模态 / ESC / PointerOverUI / Town CanvasGroup 部分走既有公开回调与真实 Raycast，
    /// 明确是回调机制证据，不是物理鼠标 / 物理键盘证据。
    /// </summary>
    private object CameraControl() => Get(_host, "WorldCamera");

    private static Vector3 CameraPosition(object camera) => ((Component)camera).transform.position;

    private static float CameraSize(object camera) => ((Camera)Get(camera, "ViewCamera")).orthographicSize;

    private static void SetCameraInput(object camera, Vector3 mouse, bool rightHeld, bool rightDown,
        bool middleHeld, bool middleDown, float scrollY)
    {
        Call(camera, "SetInputSampleForTests", mouse, rightHeld, rightDown, middleHeld, middleDown, scrollY);
    }

    private static void ClearCameraInput(object camera)
    {
        Call(camera, "ClearInputSampleForTests");
    }

    /// <summary>
    /// 非模态阳性对照的环境前置：Host.Update 的 overUI 仍用真实 Input.mousePosition 计算；
    /// 若真实指针恰好压在 UI 上，镜头本就不会响应。此时必须显式失败并给出命中证据，
    /// 不能用“相机没动”反推产品阻断开镜。
    /// </summary>
    private void AssertRealPointerOutsideUi(string what)
    {
        Vector2 pointer = Input.mousePosition;
        bool over = (bool)Call(_host, "PointerOverUI", pointer);
        string hits = over ? DumpHits(RaycastAllAt(pointer)) : "（无 UI 命中）";
        Evidence("INPUT-GATE-POINTER", what + " pointer=" + pointer + " overUI=" + over
            + " hits=" + hits.Replace("\n", " | "));
        Assert.IsFalse(over,
            what + "：非模态阳性对照要求真实指针不落在 UI 上（Host.Update 的 overUI 依赖真实 Input.mousePosition）；"
            + "当前环境指针=" + pointer + "。这是环境前置失败，不是产品输入门禁结论。\n" + hits);
    }

    private static string CameraPose(object camera)
        => CameraPosition(camera).ToString("F5") + " size=" + CameraSize(camera).ToString("F5");

    [UnityTest]
    public IEnumerator ManagementModal_InputGate_BlocksCameraAndKeepsEscapeTownState()
    {
        // ---------- 单例宿主 + Editor-only internal 采样缝（不新增 public API） ----------
        Assert.AreEqual(1, UnityEngine.Object.FindObjectsOfType(T("ProductHomeWorldHost"), true).Length,
            "真实产品世界宿主必须单例");
        object camera = CameraControl();
        Assert.IsNotNull(camera, "Host.WorldCamera 必须存在");
        var cameraType = camera.GetType();
        Assert.AreEqual(1, UnityEngine.Object.FindObjectsOfType(cameraType, true).Length,
            "HomeWorldCamera 必须唯一");
        Type[] inputSampleSignature = { typeof(Vector3), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(float) };
        var sampleEntry = ExactMethod(cameraType, "SetInputSampleForTests", Flags,
            "Editor 编译下必须存在 internal SetInputSampleForTests（输入采样缝）", inputSampleSignature);
        Assert.IsTrue(sampleEntry.IsAssembly, "输入采样缝必须是 internal（不得成为 public API）");
        Assert.IsNull(MethodOrNull(cameraType, "SetInputSampleForTests",
                BindingFlags.Public | BindingFlags.Instance, inputSampleSignature),
            "输入采样缝不得暴露为 public");
        var readInput = ExactMethod(cameraType, "ReadInput", BindingFlags.Public | BindingFlags.Instance,
            "HomeWorldCamera.ReadInput 必须保持既有 public 产品入口", typeof(bool), typeof(bool));
        Assert.AreEqual(2, readInput.GetParameters().Length,
            "ReadInput 公共签名不得改变（overUI, suppressRightPan）");
        Assert.AreEqual(typeof(bool), readInput.GetParameters()[0].ParameterType);
        Assert.AreEqual(typeof(bool), readInput.GetParameters()[1].ParameterType);

        string selected = FirstSnapshotPersonIdFromHost();
        Call(Get(_host, "Selection"), "SelectPerson", selected);
        yield return null;
        Assert.AreEqual(selected, SelectionId(), "夹具必须先选中一位真实人物");
        Assert.IsFalse((bool)Get(_shell, "IsPersonModalOpen"), "阳性对照前必须处于非模态 World");

        var townRoot = (GameObject)Get(_town, "canvasRoot");
        var townGroupBefore = townRoot.GetComponent<CanvasGroup>();
        bool interactableBefore = townGroupBefore == null || townGroupBefore.interactable;
        bool blocksBefore = townGroupBefore == null || townGroupBefore.blocksRaycasts;

        try
        {
            // ---------- 阳性对照：真实 Host.Update 帧消费合成 legacy Input 快照 ----------
            AssertRealPointerOutsideUi("非模态阳性对照前");
            Vector3 posBefore = CameraPosition(camera);
            float sizeBefore = CameraSize(camera);
            SetCameraInput(camera, new Vector3(120f, 140f, 0f), true, true, false, false, 0f);
            yield return null; // 首帧仅按下，用于播种 _lastMouse，不产生 Pan
            SetCameraInput(camera, new Vector3(220f, 180f, 0f), true, false, false, false, 1f);
            yield return null;
            yield return null;
            Vector3 posAfter = CameraPosition(camera);
            float sizeAfter = CameraSize(camera);
            Evidence("INPUT-GATE-POSITIVE", "before=" + posBefore + " size=" + sizeBefore
                + " after=" + posAfter + " size=" + sizeAfter);
            Assert.AreNotEqual(posBefore, posAfter,
                "非模态时真实 Host.Update 必须消费合成右键拖动并平移相机（阳性对照）");
            Assert.AreNotEqual(sizeBefore, sizeAfter,
                "非模态时真实 Host.Update 必须消费合成滚轮并改变 orthographicSize（阳性对照）");
            ClearCameraInput(camera);
            yield return null; // 清空后确认生产采样恢复：不再注入时相机保持
            Vector3 idlePos = CameraPosition(camera);
            float idleSize = CameraSize(camera);
            yield return null;
            Assert.AreEqual(idlePos, CameraPosition(camera), "清除测试采样后相机不得再被合成输入移动");
            Assert.AreEqual(idleSize, CameraSize(camera), "清除测试采样后 orthographicSize 必须稳定");

            // ---------- Management 模态：相同输入序列被真实 overUI 门禁阻断 ----------
            OpenManagementThroughNavigation();
            yield return null;
            Assert.AreEqual("Management", Surface(), "必须进入真实管理模态");
            Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "管理模态 SurfaceModalActive 必须为真");
            Assert.IsTrue((bool)Get(_shell, "IsPersonModalOpen"), "IsPersonModalOpen 别名必须同步为真");
            var townGroup = townRoot.GetComponent<CanvasGroup>();
            Assert.IsNotNull(townGroup, "管理模态必须通过既有 TownUI.canvasRoot CanvasGroup 阻断旧 Town");
            Assert.IsFalse(townGroup.interactable, "管理模态旧 Town interactable 必须禁用");
            Assert.IsFalse(townGroup.blocksRaycasts, "管理模态旧 Town blocksRaycasts 必须禁用");

            var management = Management();
            var blocker = FindPath(((Component)management).transform, "ModalBlocker");
            Assert.IsNotNull(blocker, "管理 ModalBlocker 必须存在");
            var blockerRect = (RectTransform)blocker;
            Vector2 background = ScreenPointAt(blockerRect, new Vector2(blockerRect.rect.xMin + 12f, 0f));
            Assert.IsTrue((bool)Call(_host, "PointerOverUI", background), "模态背景点必须被 UI（Blocker）命中");
            Call(_host, "HandleWorldClick", background);
            Assert.AreEqual("Management", Surface(), "模态时世界点击回调不得切换 Surface");
            Assert.AreEqual(selected, SelectionId(), "模态时世界点击回调不得改变 Selection");

            Vector3 modalPos = CameraPosition(camera);
            float modalSize = CameraSize(camera);
            object build = Get(_shell, "Build");
            bool buildOpen = (bool)Get(build, "IsOpen");
            string buildDef = (string)Get(build, "SelectedDefinitionId");
            Vector2 buildCandidate = (Vector2)Get(build, "Candidate");
            string speed = ClockSpeed();
            double multiplier = ClockMultiplier();
            Assert.AreEqual("Paused", speed, "夹具必须从暂停开始以保证 Clock 断言确定");

            SetCameraInput(camera, new Vector3(120f, 140f, 0f), true, true, false, false, 0f);
            yield return null;
            SetCameraInput(camera, new Vector3(220f, 180f, 0f), true, false, false, false, 1f);
            for (int i = 0; i < 5; i++) yield return null;

            Assert.AreEqual(modalPos, CameraPosition(camera), "管理模态下真实帧相机位置不得被合成拖动改变");
            Assert.AreEqual(modalSize, CameraSize(camera), "管理模态下真实帧 orthographicSize 不得被合成滚轮改变");
            Assert.AreEqual(selected, SelectionId(), "管理模态下 Selection 必须保持");
            Assert.AreEqual(buildOpen, (bool)Get(build, "IsOpen"), "管理模态下建造开关必须保持");
            Assert.AreEqual(buildDef, (string)Get(build, "SelectedDefinitionId"), "管理模态下建造候选必须保持");
            Assert.AreEqual(buildCandidate, (Vector2)Get(build, "Candidate"), "管理模态下建造候选位置必须保持");
            Assert.AreEqual(speed, ClockSpeed(), "管理模态下 Clock Speed 不得漂移");
            Assert.AreEqual(multiplier, ClockMultiplier(), "管理模态下 Clock SpeedMultiplier 不得漂移");
            Assert.IsTrue(SimulationActive(), "管理模态下 IsSimulationActive 必须保持");
            Assert.AreEqual("Management", Surface(), "管理模态下 Surface 必须保持");
            Assert.IsFalse(townGroup.interactable, "管理模态下旧 Town 必须持续禁用");
            Assert.IsFalse(townGroup.blocksRaycasts, "管理模态下旧 Town 射线必须持续禁用");
            Evidence("INPUT-GATE-MODAL", "management camera=" + CameraPose(camera)
                + " selection=" + SelectionId() + " buildOpen=" + buildOpen + " speed=" + ClockSpeed());
            ClearCameraInput(camera);

            // ---------- ESC：既有公开回调只关闭当前 Surface，选择/速度不漂移 ----------
            Call(_shell, "HandleEscape");
            yield return null;
            Assert.AreEqual("World", Surface(), "ESC 回调必须只关闭当前管理模态并回 World");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "关闭后 SurfaceModalActive 必须为假");
            Assert.IsFalse((bool)Get(_shell, "IsPersonModalOpen"), "关闭后 IsPersonModalOpen 别名必须同步为假");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "ESC 关闭后旧 Town interactable 必须恢复原值");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "ESC 关闭后旧 Town blocksRaycasts 必须恢复原值");
            Assert.AreEqual(selected, SelectionId(), "ESC 关闭模态不得改变 Selection");
            Assert.AreEqual(speed, ClockSpeed(), "ESC 关闭模态不得改变 Clock Speed");
            Assert.AreEqual(multiplier, ClockMultiplier(), "ESC 关闭模态不得改变 Clock 速度倍率");
            Assert.IsFalse((bool)Get(build, "IsOpen"), "ESC 关闭模态不得打开建造");

            // ---------- Person 模态：真实导航打开，相同输入序列同样被阻断 ----------
            var navigate1 = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate1");
            Assert.IsNotNull(navigate1, "底部导航“人物”必须存在");
            var navigate1Button = navigate1.GetComponent<Button>();
            Assert.IsNotNull(navigate1Button, "底部导航“人物”必须是 Button");
            Assert.IsTrue(navigate1Button.interactable, "World 状态人物导航必须可点");
            RealPointerClick((RectTransform)navigate1, "底部导航·人物（合成输入门禁）");
            yield return null;
            Assert.AreEqual("Person", Surface(), "必须进入真实 Person Surface");
            Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "Person 模态 SurfaceModalActive 必须为真");
            Assert.IsTrue((bool)Get(_shell, "IsPersonModalOpen"), "Person 模态 IsPersonModalOpen 别名必须为真");
            Assert.IsFalse(townGroup.interactable, "Person 模态旧 Town interactable 必须禁用");
            Assert.IsFalse(townGroup.blocksRaycasts, "Person 模态旧 Town blocksRaycasts 必须禁用");

            Vector3 personModalPos = CameraPosition(camera);
            float personModalSize = CameraSize(camera);
            string personSpeed = ClockSpeed();
            double personMultiplier = ClockMultiplier();
            SetCameraInput(camera, new Vector3(120f, 140f, 0f), true, true, false, false, 0f);
            yield return null;
            SetCameraInput(camera, new Vector3(220f, 180f, 0f), true, false, false, false, 1f);
            for (int i = 0; i < 5; i++) yield return null;

            Assert.AreEqual(personModalPos, CameraPosition(camera), "Person 模态下真实帧相机位置不得被合成拖动改变");
            Assert.AreEqual(personModalSize, CameraSize(camera), "Person 模态下真实帧 orthographicSize 不得被合成滚轮改变");
            Assert.AreEqual(selected, SelectionId(), "Person 模态下 Selection 必须保持");
            Assert.AreEqual(personSpeed, ClockSpeed(), "Person 模态下 Clock Speed 不得漂移");
            Assert.AreEqual(personMultiplier, ClockMultiplier(), "Person 模态下 Clock SpeedMultiplier 不得漂移");
            Assert.IsTrue(SimulationActive(), "Person 模态下 IsSimulationActive 必须保持");
            Assert.AreEqual("Person", Surface(), "Person 模态下 Surface 必须保持");
            Evidence("INPUT-GATE-PERSON", "person camera=" + CameraPose(camera)
                + " selection=" + SelectionId() + " speed=" + ClockSpeed());
            ClearCameraInput(camera);

            Call(_shell, "HandleEscape");
            yield return null;
            Assert.AreEqual("World", Surface(), "ESC 回调必须只关闭当前 Person Surface");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "Person 关闭后 SurfaceModalActive 必须为假");
            Assert.IsFalse((bool)Get(_shell, "IsPersonModalOpen"), "Person 关闭后 IsPersonModalOpen 别名必须为假");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "Person 关闭后旧 Town interactable 必须恢复原值");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "Person 关闭后旧 Town blocksRaycasts 必须恢复原值");
            Assert.AreEqual(selected, SelectionId(), "Person 关闭不得改变 Selection");
            Assert.AreEqual(personSpeed, ClockSpeed(), "Person 关闭不得改变 Clock Speed");

            // ---------- 重复开关：Town CanvasGroup 恢复不漂移 ----------
            OpenManagementThroughNavigation("重复开关·管理");
            yield return null;
            Assert.IsFalse(townGroup.interactable, "重复打开管理必须再次禁用旧 Town interactable");
            Assert.IsFalse(townGroup.blocksRaycasts, "重复打开管理必须再次禁用旧 Town blocksRaycasts");
            Call(_shell, "CloseSurface");
            yield return null;
            Assert.AreEqual("World", Surface(), "重复开关后必须回 World");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "重复开关后旧 Town interactable 必须恢复原值");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "重复开关后旧 Town blocksRaycasts 必须恢复原值");
            Assert.AreEqual(selected, SelectionId(), "重复开关不得改变 Selection");
            Assert.AreEqual(personSpeed, ClockSpeed(), "重复开关不得改变 Clock Speed");
            Evidence("INPUT-GATE-REOPEN", "repeatOpenClose=2 townInteractable=" + townGroup.interactable
                + " townBlocksRaycasts=" + townGroup.blocksRaycasts);

            // ---------- 关闭后：同一合成输入再次可改变相机（门禁依赖模态而非永久吞输入） ----------
            AssertRealPointerOutsideUi("关闭模态后再次注入前");
            Vector3 afterClosePos = CameraPosition(camera);
            float afterCloseSize = CameraSize(camera);
            SetCameraInput(camera, new Vector3(60f, 80f, 0f), true, true, false, false, 0f);
            yield return null;
            SetCameraInput(camera, new Vector3(260f, 240f, 0f), true, false, false, false, 1f);
            yield return null;
            yield return null;
            Assert.AreNotEqual(afterClosePos, CameraPosition(camera),
                "关闭模态后同一合成拖动必须再次平移相机（门禁可逆）");
            Assert.AreNotEqual(afterCloseSize, CameraSize(camera),
                "关闭模态后同一合成滚轮必须再次改变 orthographicSize（门禁可逆）");
            Evidence("INPUT-GATE-AFTER-CLOSE", "before=" + afterClosePos + " size=" + afterCloseSize
                + " after=" + CameraPosition(camera) + " size=" + CameraSize(camera));
        }
        finally
        {
            // 输入采样是实例级可逆状态：无论断言是否失败都必须恢复生产 legacy Input 采样。
            ClearCameraInput(camera);
            Evidence("INPUT-GATE-CLEANUP", "camera sample cleared; no physical mouse/keyboard was simulated");
        }
    }

    // ==================== 用例 4：管理真实按钮派工 / 撤职（同类双实例 + 满员前置 + AlreadyAssigned） ====================

    [UnityTest]
    public IEnumerator ManagementWorkplaceCommands_AssignRemoveKeepInstanceIdentityAndRejectWithoutWrite()
    {
        string a = PlaceBuilding("building_farm", 4f);
        string b = PlaceBuilding("building_farm", 8f);
        Assert.AreNotEqual(a, b, "两个同类 Farm 必须是独立建筑实例");
        string p1 = SnapshotPersonIdAt(0), p2 = SnapshotPersonIdAt(1), p3 = SnapshotPersonIdAt(2);
        Evidence("CASE-A", "START a=" + a + " b=" + b + " p1=" + p1 + " p2=" + p2 + " p3=" + p3);

        // 施工准备只用既有公开施工 API；完成后进入管理页做真实按钮命令。
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", a, p1).ToString(), "p1 必须能加入 farm A 施工");
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", b, p2).ToString(), "p2 必须能加入 farm B 施工");
        AdvanceTo(14400);
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(a), "farm A 必须在真实推进后完工");
        Assert.AreEqual("Completed", BuildingState(b), "farm B 必须在真实推进后完工");

        Call(Get(_host, "Selection"), "SelectPerson", p1);
        OpenManagementThroughNavigation("管理·派工/撤职命令");
        yield return null;
        var management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        ShowWorkplacesPage(management);
        yield return null;

        string wa = WorkplaceIdForBuilding(management, a, "farm A");
        string wb = WorkplaceIdForBuilding(management, b, "farm B");
        Assert.AreNotEqual(wa, wb, "两个已完成 Farm 的功能岗位 ID 必须独立");
        Assert.IsTrue(wa.Contains(a) && wb.Contains(b), "岗位 ID 必须绑定各自建筑实例");

        // p1 → A
        SelectForManagement(management, p1);
        yield return null;
        ClickWorkplaceRow(management, wa, "岗位行·farm A");
        yield return null;
        ClickButtonField(management, "assignButton", "派工·p1→farm A");
        yield return null;
        Assert.AreEqual(wa, PersonWorkplace(p1), "真实派工按钮必须写入 farm A 岗位");
        Assert.AreEqual(1, WorkplaceOccupants(wa), "farm A 名额占用必须为 1");
        AssertStatusContains(management, "已绑定");
        Evidence("CASE-A", "p1 assigned wa=" + wa + " occupants=" + WorkplaceOccupants(wa));

        // p2 → A（填满 Capacity=2）
        SelectForManagement(management, p2);
        yield return null;
        ClickWorkplaceRow(management, wa, "岗位行·farm A（p2）");
        yield return null;
        ClickButtonField(management, "assignButton", "派工·p2→farm A");
        yield return null;
        Assert.AreEqual(wa, PersonWorkplace(p2), "p2 必须写入同一 farm A 岗位");
        Assert.AreEqual(2, WorkplaceOccupants(wa), "farm A 应被填满（Capacity=2）");
        Assert.AreEqual(0, WorkplaceOccupants(wb), "farm B 仍必须为空");
        Evidence("CASE-A", "wa full occupants=" + WorkplaceOccupants(wa));

        // 满员：产品按钮必须前置禁用；禁用按钮的真实点击不得写入。
        SelectForManagement(management, p3);
        yield return null;
        ClickWorkplaceRow(management, wa, "岗位行·farm A（满员目标）");
        yield return null;
        var assignButton = (Button)Get(management, "assignButton");
        Assert.IsFalse(assignButton.interactable, "满员目标必须禁派工按钮（CanAssign=false）");
        ClickButtonField(management, "assignButton", "派工·p3→满员 farm A（禁用点击）", false);
        yield return null;
        Assert.IsNull(PersonWorkplace(p3), "满员禁用点击不得写入 p3 职责");
        Assert.AreEqual(2, WorkplaceOccupants(wa), "满员禁用点击不得改变 farm A 占用");
        Evidence("CASE-A", "FULL-AT-UI disabled=true p3-workplace=<none> wa-occupants=2 (CapacityFull 域名诊断未经按钮进入，见报告未测)");

        // p1 撤职：旧名额释放。
        SelectForManagement(management, p1);
        yield return null;
        ClickButtonField(management, "removeButton", "撤职·p1");
        yield return null;
        Assert.IsNull(PersonWorkplace(p1), "撤职按钮必须移除 p1 主要职责");
        Assert.AreEqual(1, WorkplaceOccupants(wa), "撤职必须释放 farm A 名额");
        Assert.AreEqual(0, WorkplaceOccupants(wb), "撤职不得影响 farm B");
        AssertStatusContains(management, "已移除");
        Evidence("CASE-A", "p1 removed; wa occupants=" + WorkplaceOccupants(wa));

        // p1 → B：新目标合法，实例独立。
        ClickWorkplaceRow(management, wb, "岗位行·farm B（新目标）");
        yield return null;
        ClickButtonField(management, "assignButton", "派工·p1→farm B");
        yield return null;
        Assert.AreEqual(wb, PersonWorkplace(p1), "释放后必须能派到另一同类实例 farm B");
        Assert.AreEqual(1, WorkplaceOccupants(wb), "farm B 名额占用必须为 1");
        Assert.AreEqual(1, WorkplaceOccupants(wa), "farm A 仍必须保留 p2 的占用");
        AssertStatusContains(management, "已绑定");

        // AlreadyAssigned：可点但领域拒绝，不写任何岗位。
        ClickWorkplaceRow(management, wa, "岗位行·farm A（已有职责复点）");
        yield return null;
        ClickButtonField(management, "assignButton", "派工·p1→farm A（已有职责）");
        yield return null;
        AssertStatusContains(management, "已有主要职责");
        Assert.AreEqual(wb, PersonWorkplace(p1), "AlreadyAssigned 拒绝不得把 p1 改到 farm A");
        Assert.AreEqual(1, WorkplaceOccupants(wa), "AlreadyAssigned 拒绝不得改变 farm A 占用");
        Assert.AreEqual(1, WorkplaceOccupants(wb), "AlreadyAssigned 拒绝不得改变 farm B 占用");
        Evidence("CASE-A", "END p1=" + PersonWorkplace(p1) + " wa=" + WorkplaceOccupants(wa)
            + " wb=" + WorkplaceOccupants(wb) + " (AlreadyAssigned rejected, no write)");
        Assert.AreEqual("Management", Surface(), "管理命令不得非预期关闭管理页");
    }

    // ==================== 用例 5：施工管理真实按钮（保留 Primary Assignment / 到场才推进） ====================

    [UnityTest]
    public IEnumerator ManagementConstructionCommands_PreservePrimaryAssignmentAndAdvanceOnlyOnsiteLabor()
    {
        // 夹具窗口所有权隔离（既有公开注入点 VillageRuntimeBridge.ConfigureActivitySchedule，未改产品规则）：
        // 本例随后会从当日 0 分钟真实推进到约 816 分钟，生产默认的 420 分钟自主活动机会会随机占用在场人物
        // （真实回执 2026-10-24T14:18:56 CASE-B：6 人中 5 人被 420 活动占用，只剩 1 名自由候选），
        // 使“至少 2 名未被活动占用且无既有职责的自由候选”前置随机失败。这里只排除本例时间跨度内的
        // 活动机会，不关闭全局活动、不改产品活动规则；TearDown 恢复原日程并只读校验。
        IsolateActivityOpportunitiesForFixture("CASE-B construction measurement fixture");

        string a = PlaceBuilding("building_farm", 4f);
        Evidence("CASE-B", "START building=" + a);

        // 生产时间契约（只读）：通勤分钟与上午工作边界来自真实 Schedule，不写死。
        object schedule = Get(StaticField("VillageRuntimeBridge", "IntradayWork"), "Schedule");
        Assert.IsNotNull(schedule, "IntradayWork.Schedule 必须存在");
        long morningStart = Convert.ToInt64(Get(schedule, "MorningStart"));
        long commuteMinutes = Convert.ToInt64(Get(schedule, "CommuteMinutes"));
        Assert.Greater(commuteMinutes, 0L, "真实通勤分钟数必须非零（登记当分钟不能直接到场）");

        // 0 worker 的时间推进不得伪造施工进度（真实 479 分钟、无任何参与者）。
        AdvanceTo(479);
        Call(_host, "RefreshNow");
        Assert.AreEqual("UnderConstruction", BuildingState(a), "0 worker 时建筑必须仍在施工");
        Assert.IsNull(ActiveProject(), "0 worker 时不得存在活动施工项目");

        // 真实活动占用前置（只读证明，不改任何产品状态）：
        // 420 分钟的真实自主活动机会若选中某人，该人持有 Activity claim；生产 RefreshWorkIntent →
        // ArrivedWorkers 会跳过被 claim 的人（自主活动优先于工作到场登记），所以“施工加入成功”
        // 不等于“必登记到场计划”。本用例要求两名施工参与者都未被活动占用，
        // 才能覆盖“登记当前分钟 + Schedule.CommuteMinutes”这一生产契约。
        // 本例已在开始时用既有公开注入点排除本例时间跨度内的活动机会（见方法开头说明），
        // 因此这里的真实 QueryClaim 扫描应稳定返回全部自由候选；断言本身不做任何放松。
        var claims = new StringBuilder();
        var freePersons = new List<string>();
        foreach (string id in HostPersonIds())
        {
            string claim = ActivityClaim(id);
            claims.Append(id + "=" + (claim ?? "<null>") + ";");
            if (claim == null && string.IsNullOrEmpty(PersonWorkplace(id))) freePersons.Add(id);
        }
        Evidence("CASE-B", "activity claims at minute " + Get(Clock(), "MinuteOfDay") + ": " + claims);
        Assert.GreaterOrEqual(freePersons.Count, 2,
            "本夹具需要至少 2 名未被自主活动占用且无既有职责的真实人物；实际 " + freePersons.Count
            + "；claims=" + claims);
        string p1 = freePersons[0], p2 = freePersons[1];
        Evidence("CASE-B", "participants p1=" + PersonDiagnostic(p1) + " p2=" + PersonDiagnostic(p2)
            + "; " + ArrivalDiagnostic());

        Call(Get(_host, "Selection"), "SelectPerson", p1);
        OpenManagementThroughNavigation("管理·施工命令");
        yield return null;
        var management = Management();
        ShowWorkplacesPage(management);
        yield return null;

        var before = FindRow(management, "Construction", "BuildingInstanceId", a);
        Assert.IsNotNull(before, "施工页必须包含真实施工建筑行");
        Assert.AreEqual(0, Convert.ToInt32(Get(before, "Progress")), "0 worker 推进后 Progress 必须仍为 0");
        Assert.AreEqual(0, Convert.ToInt32(Get(before, "WorkerCount")), "0 worker 行必须显示 0 人");
        Evidence("CASE-B", "0-worker advance to 479: progress=0 workers=0");

        // 人物已有 Primary Assignment 的既有合法入口（显式测试兼容岗位；正式默认世界只有实例岗位），
        // 施工加入不得改写它。本夹具不改回“正式默认 0 建筑 / 全量静态岗位”。
        RegisterLegacyFarmWorkplaceForFixture();
        Assert.AreEqual("Success", Bridge("TryAssignPerson", p1, "ws_farm_01").ToString(),
            "施工前必须能为 p1 建立真实 Primary Assignment");
        Assert.AreEqual("ws_farm_01", PersonWorkplace(p1), "前置 Primary Assignment 必须真实写入");

        // 静态兼容岗位（既有静态 Assignment）的最小 UI 事实：显示 + 派工禁用 + 撤职可用。
        // 只点行读取按钮状态，不点撤职，避免破坏本用例“施工保留 Primary Assignment”的前置。
        Call(management, "RefreshNow");
        yield return null;
        object legacyRow = FindRow(management, "Workplaces", "WorkplaceId", "ws_farm_01");
        Assert.IsNotNull(legacyRow, "既有静态 Assignment 必须出现在职责与场所行中（兼容记录）");
        Assert.AreEqual("LegacyCompatibility", Get(legacyRow, "Kind").ToString(), "静态岗位必须标为兼容记录");
        Assert.IsFalse((bool)Get(legacyRow, "CanAssign"), "静态兼容岗位不是正式新增派工目标");
        Assert.AreEqual(1, Convert.ToInt32(Get(legacyRow, "BoundCount")), "静态兼容岗位必须显示真实绑定名额");
        ClickWorkplaceRow(management, "ws_farm_01", "岗位行·静态兼容 ws_farm_01");
        yield return null;
        Assert.IsFalse(((Button)Get(management, "assignButton")).interactable, "静态兼容行选中后派工按钮必须禁用");
        Assert.IsTrue(((Button)Get(management, "removeButton")).interactable, "静态兼容行选中后撤职按钮必须可用");
        Evidence("CASE-B", "legacy row visible=ws_farm_01 kind=LegacyCompatibility bound=1"
            + " assign-disabled=True remove-enabled=True (remove not clicked to keep primary prerequisite)");

        long joinMinuteBeforeP1 = Convert.ToInt64(Get(Clock(), "MinuteOfDay"));
        Evidence("CASE-B", "before p1 add: " + ProjectDiagnostic() + " p1=" + PersonDiagnostic(p1)
            + "; " + ArrivalDiagnostic());
        Assert.IsNull(ActivityClaim(p1),
            "p1 加入施工前不得处于自主活动占用（真实 claim 会跳过到场登记）：" + PersonDiagnostic(p1));
        ClickConstructionRow(management, a, "施工行·farm A");
        yield return null;
        ClickButtonField(management, "addConstructionButton", "加入施工·p1");
        yield return null;
        long p1RegistrationMinute = Convert.ToInt64(Get(Clock(), "MinuteOfDay"));
        Assert.AreEqual(joinMinuteBeforeP1, p1RegistrationMinute, "Clock 暂停时真实加入施工不得自行推进时间");
        var project = ActiveProject();
        Assert.IsNotNull(project, "真实加入施工后必须存在活动项目");
        Assert.IsTrue(Strings(Get(project, "ParticipantPersonIds")).Contains(p1), "p1 必须成为真实施工参与者");
        Assert.AreEqual("ws_farm_01", PersonWorkplace(p1), "加入施工不得改写 p1 的 Primary Assignment");
        Assert.AreEqual(1, WorkplaceOccupants("ws_farm_01"), "岗位占用只反映 Primary Assignment，不等于施工人数");
        Assert.AreEqual(1, Strings(Get(project, "ParticipantPersonIds")).Count, "施工参与者必须为 1");

        // 真实登记时刻是“加入施工的当前分钟”，不是下一个 MorningStart 边界：
        // AddProjectParticipantCore 成功后立即 RefreshWorkIntent，登记 ReadyMinute = 当前分钟 + CommuteMinutes。
        Assert.GreaterOrEqual(p1RegistrationMinute, morningStart - commuteMinutes,
            "生产登记只发生在通勤窗口内（MorningStart - CommuteMinutes 起）");
        Assert.Less(p1RegistrationMinute, morningStart,
            "本夹具在 MorningStart 前加入施工，故到场必须由真实登记时刻推算，不能用 MorningStart+通勤");
        object p1Arrival = WorkArrival(p1);
        Assert.IsNotNull(p1Arrival, "加入施工后 RefreshWorkIntent 必须登记 p1 的真实到场计划");
        Assert.AreEqual("construction:" + a, (string)Get(p1Arrival, "Destination"), "目标登记目的地必须是本施工实例");
        Assert.AreEqual(p1RegistrationMinute + commuteMinutes, Convert.ToInt64(Get(p1Arrival, "ReadyMinute")),
            "生产到场分钟必须等于真实登记时刻 + Schedule.CommuteMinutes");
        Evidence("CASE-B", "p1 joined construction at minute " + p1RegistrationMinute + "; primary=ws_farm_01 unchanged;"
            + " participants=1; arrival=construction:" + a + " ready=" + Get(p1Arrival, "ReadyMinute")
            + "; after=" + ProjectDiagnostic() + "; farmOccupancy=" + WorkplaceOccupants("ws_farm_01")
            + "; " + ArrivalDiagnostic());

        SelectForManagement(management, p2);
        yield return null;
        long joinMinuteBeforeP2 = Convert.ToInt64(Get(Clock(), "MinuteOfDay"));
        Evidence("CASE-B", "before p2 add: " + ProjectDiagnostic() + " p2=" + PersonDiagnostic(p2)
            + "; " + ArrivalDiagnostic());
        Assert.IsNull(ActivityClaim(p2),
            "p2 加入施工前不得处于自主活动占用（真实 claim 会跳过到场登记）：" + PersonDiagnostic(p2));
        ClickConstructionRow(management, a, "施工行·farm A（p2）");
        yield return null;
        ClickButtonField(management, "addConstructionButton", "加入施工·p2");
        yield return null;
        long p2RegistrationMinute = Convert.ToInt64(Get(Clock(), "MinuteOfDay"));
        Assert.AreEqual(joinMinuteBeforeP2, p2RegistrationMinute, "Clock 暂停时真实加入施工不得自行推进时间");
        Assert.AreEqual(p1RegistrationMinute, p2RegistrationMinute, "本夹具两人在同一真实分钟加入施工");
        project = ActiveProject();
        Assert.AreEqual(2, Strings(Get(project, "ParticipantPersonIds")).Count, "p2 加入后施工参与者必须为 2");
        Assert.AreEqual("ws_farm_01", PersonWorkplace(p1), "p2 加入不得改写 p1 的 Primary Assignment");
        object p2Arrival = WorkArrival(p2);
        Assert.IsNotNull(p2Arrival, "p2 加入施工后必须登记真实到场计划；p2=" + PersonDiagnostic(p2)
            + "；" + ProjectDiagnostic() + "；" + ArrivalDiagnostic());
        Assert.AreEqual("construction:" + a, (string)Get(p2Arrival, "Destination"), "p2 目标登记目的地必须是本施工实例");
        Assert.AreEqual(p2RegistrationMinute + commuteMinutes, Convert.ToInt64(Get(p2Arrival, "ReadyMinute")),
            "p2 生产到场分钟必须等于真实登记时刻 + Schedule.CommuteMinutes");
        Evidence("CASE-B", "p2 joined construction at minute " + p2RegistrationMinute
            + "; arrival ready=" + Get(p2Arrival, "ReadyMinute")
            + "; after=" + ProjectDiagnostic() + "; farmOccupancy=" + WorkplaceOccupants("ws_farm_01")
            + "; " + ArrivalDiagnostic());

        // 关闭管理后推进到真实到场边界；再重开管理走按钮撤出/再加入。
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "施工命令后 ESC 必须回 World");

        // 生产契约（AddProjectParticipantCore → RefreshWorkIntent → VillageIntradayWork.Arrived）：
        // 加入施工成功的当前分钟就登记目的地，ReadyMinute = 该真实登记分钟 + Schedule.CommuteMinutes；
        // 480 工作边界不会为“目的地未变”的登记重新计时。本夹具在 479 加入，故 480/481 仍在通勤途中。
        long onsiteMinute = p1RegistrationMinute + commuteMinutes;
        Assert.AreEqual(p2RegistrationMinute + commuteMinutes, onsiteMinute, "同分钟登记的参与者必须有同一边界");
        Assert.Greater(onsiteMinute, 481L, "生产通勤边界必须晚于 481");

        AdvanceTo(480);
        AdvanceTo(481);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "推进后施工项目必须仍活动");
        object arrival = WorkArrival(p1);
        Assert.IsNotNull(arrival, "施工参与者必须已登记真实到场计划（通勤事实）");
        Assert.AreEqual("construction:" + a, (string)Get(arrival, "Destination"), "通勤目的地必须是同一施工实例");
        Assert.AreEqual(onsiteMinute, Convert.ToInt64(Get(arrival, "ReadyMinute")),
            "生产到场分钟必须等于真实登记时刻 + 通勤分钟（480 边界不重新登记）");
        Assert.Greater(onsiteMinute, 481L, "生产通勤边界必须晚于 481");
        Assert.IsNull(PersonPlace(p1), "通勤期间不得伪造到场");
        Assert.AreNotEqual("项目劳动", WorkExecution(p1), "通勤期间不得计为项目劳动");
        Assert.AreEqual(0, Convert.ToInt64(Get(project, "LaborRemainder")), "通勤期间不得产生劳动剩余");
        Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "通勤期间不得推进施工进度");
        Evidence("CASE-B", "commute to " + onsiteMinute + ": place=<null> execution=" + WorkExecution(p1)
            + " labor=0 progress=0 (no contribution before real arrival)");

        // 到达真实到场分钟后必须有真实施工到场与项目劳动，且尚未凭空产生进度。
        AdvanceTo(onsiteMinute);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "到场分钟施工项目必须仍活动");
        Assert.AreEqual("construction:" + a, PersonPlace(p1), "p1 必须在生产到场分钟真实到场施工（非仅参与者计数）");
        Assert.AreEqual("项目劳动", WorkExecution(p1), "p1 必须处于真实项目劳动");
        Assert.AreEqual(0, Convert.ToInt64(Get(project, "LaborRemainder")), "到场当分钟尚未累计劳动");
        Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "到场当分钟不得凭空产生进度");
        Evidence("CASE-B", "onsite minute=" + onsiteMinute + " place=" + PersonPlace(p1) + " labor=0 progress=0");

        // 到场后一个真实工作分钟必须产生劳动剩余（未满一个进度单位前 Progress 仍为 0）。
        AdvanceTo(onsiteMinute + 1);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.AreEqual("construction:" + a, PersonPlace(p1), "真实到场后 p1 必须仍在现场");
        Assert.AreEqual("项目劳动", WorkExecution(p1), "真实到场后 p1 必须仍在项目劳动");
        Assert.Greater(Convert.ToInt64(Get(project, "LaborRemainder")), 0, "真实到场后必须产生劳动剩余量");
        Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "不足一个进度单位前不得推进 Progress");

        // 同一工作日内继续真实劳动，累计满一个进度单位后才允许 Progress 非零（不是多天自动完工）。
        AdvanceTo(796);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "进度出现后施工项目必须仍活动");
        Assert.AreEqual("construction:" + a, PersonPlace(p1), "施工期间 p1 必须保持在真实施工现场");
        Assert.AreEqual("项目劳动", WorkExecution(p1), "施工期间 p1 必须保持真实项目劳动");
        Assert.Greater(Convert.ToInt64(Get(project, "LaborRemainder")), 0, "真实到场后必须产生劳动剩余量");
        Assert.Greater(Convert.ToInt32(Get(project, "Progress")), 0, "真实到场后施工进度必须推进");
        Evidence("CASE-B", "onsite minute=" + Get(Clock(), "MinuteOfDay") + " place=" + PersonPlace(p1)
            + " labor=" + Get(project, "LaborRemainder") + " progress=" + Get(project, "Progress"));

        OpenManagementThroughNavigation("管理·施工命令（到场后）");
        yield return null;
        ShowWorkplacesPage(management);
        yield return null;
        SelectForManagement(management, p1);
        yield return null;
        ClickConstructionRow(management, a, "施工行·farm A（撤出）");
        yield return null;
        ClickButtonField(management, "removeConstructionButton", "撤出施工·p1");
        yield return null;
        long removalMinute = ClockMinuteOfDay();
        project = ActiveProject();
        Assert.IsNotNull(project, "撤出后另一名参与者仍应有活动施工项目");
        Assert.IsFalse(Strings(Get(project, "ParticipantPersonIds")).Contains(p1), "撤出按钮必须移除 p1 施工参与");
        Assert.AreEqual(p2, Strings(Get(project, "ParticipantPersonIds")).Single(), "撤出后必须只剩 p2");
        Assert.AreEqual("ws_farm_01", PersonWorkplace(p1), "撤出施工不得改写 p1 的 Primary Assignment");
        Assert.AreNotEqual("项目劳动", WorkExecution(p1), "撤出当下必须立即改写 p1 的实际 WorkIntent");
        Assert.AreEqual(1, WorkplaceOccupants("ws_farm_01"), "撤出施工不得改变岗位占用");
        // 仍有一名真实在场工人才允许项目继续增长；这里先证明 p2 的现场/劳动事实。
        Assert.AreEqual("construction:" + a, PersonPlace(p2), "撤出后 p2 必须仍在真实施工现场");
        Assert.AreEqual("项目劳动", WorkExecution(p2), "撤出后 p2 必须仍处于真实项目劳动");
        int removalProgress = Convert.ToInt32(Get(project, "Progress"));
        long removalLabor = Convert.ToInt64(Get(project, "LaborRemainder"));
        Evidence("CASE-B", "p1 removed at minute " + removalMinute
            + "; participants=[p2]; primary=ws_farm_01; p1-execution=" + WorkExecution(p1)
            + "; p2-place=" + PersonPlace(p2) + " p2-execution=" + WorkExecution(p2)
            + "; progress=" + removalProgress + " labor=" + removalLabor);

        // 撤出后不立即再加入：先推进一个同日下午的真实工作区间，用“剩余参与者数 + 区间分钟”
        // 从只读真实项目事实推导期望计量。若错误要求“仍有工人时整个项目零增长”，这里会失败；
        // 若撤出者仍被计量，推导出的 LaborRemainder 也不等于 1 人×区间。
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "撤出后 ESC 必须回 World");
        long measureMinute = removalMinute + 4;
        AdvanceTo(measureMinute);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "仅剩 1 名参与者时施工项目必须仍活动");
        Assert.AreEqual(p2, Strings(Get(project, "ParticipantPersonIds")).Single(), "撤出者不得在计量前重新加入");
        AssertProjectInterval(project, removalProgress, removalLabor, measureMinute - removalMinute, 1,
            "撤出后区间 " + removalMinute + "→" + measureMinute + "（仅剩 p2）");
        Assert.AreEqual("construction:" + a, PersonPlace(p2), "测量点 p2 必须仍在现场");
        Assert.AreEqual("项目劳动", WorkExecution(p2), "测量点 p2 必须仍在项目劳动");
        Assert.AreNotEqual("项目劳动", WorkExecution(p1), "撤出者在旧项目上不得继续项目劳动");
        Assert.AreEqual("ws_farm_01", PersonWorkplace(p1), "测量点 p1 Primary Assignment 必须仍不变");
        Evidence("CASE-B", "withdraw-stop measured at " + measureMinute
            + ": interval=" + (measureMinute - removalMinute) + " remaining-workers=1 progress="
            + Get(project, "Progress") + " labor=" + Get(project, "LaborRemainder")
            + " (p2 keeps contributing; p1 stopped project labor)");

        // 合法重新加入：必须是新的通勤登记 → 到场后才恢复贡献，不是立即恢复原劳动。
        OpenManagementThroughNavigation("管理·施工命令（重新加入）");
        yield return null;
        ShowWorkplacesPage(management);
        yield return null;
        SelectForManagement(management, p1);
        yield return null;
        ClickConstructionRow(management, a, "施工行·farm A（重新加入）");
        yield return null;
        int reAddProgress = Convert.ToInt32(Get(project, "Progress"));
        long reAddLabor = Convert.ToInt64(Get(project, "LaborRemainder"));
        ClickButtonField(management, "addConstructionButton", "再加入施工·p1");
        yield return null;
        long reAddMinute = ClockMinuteOfDay();
        Assert.AreEqual(measureMinute, reAddMinute, "Clock 暂停时重新加入不得自行推进时间");
        project = ActiveProject();
        Assert.IsTrue(Strings(Get(project, "ParticipantPersonIds")).Contains(p1), "撤出后必须能合法重新加入施工");
        Assert.AreEqual(2, Strings(Get(project, "ParticipantPersonIds")).Count, "重新加入后施工参与者必须为 2");
        Assert.AreEqual("ws_farm_01", PersonWorkplace(p1), "重新加入施工不得改写 Primary Assignment");
        Assert.AreEqual(reAddProgress, Convert.ToInt32(Get(project, "Progress")), "重新加入本身不得直接产生项目进度");
        Assert.AreEqual(reAddLabor, Convert.ToInt64(Get(project, "LaborRemainder")), "重新加入本身不得直接产生项目劳动");
        object reAddArrival = WorkArrival(p1);
        Assert.IsNotNull(reAddArrival, "重新加入后必须登记新的真实到场计划（不能沿用旧已到场状态）");
        Assert.AreEqual("construction:" + a, (string)Get(reAddArrival, "Destination"), "重新加入必须登记到本施工实例");
        long reAddReadyMinute = Convert.ToInt64(Get(reAddArrival, "ReadyMinute"));
        Assert.AreEqual(reAddMinute + commuteMinutes, reAddReadyMinute,
            "重新加入的生产到场分钟必须是新的登记时刻 + Schedule.CommuteMinutes");
        Assert.AreNotEqual("项目劳动", WorkExecution(p1), "重新加入后、真到场前不得立即计为项目劳动");
        Evidence("CASE-B", "p1 re-added at minute " + reAddMinute + "; arrival=construction:" + a
            + " ready=" + reAddReadyMinute + "; execution=" + WorkExecution(p1)
            + " (no immediate project labor; new commute required)");

        // 通勤区间只由 p2 贡献；到场后的下一分钟 p1 才重新计入（2 人）。
        long commuteMeasure = reAddReadyMinute - reAddMinute;
        AdvanceTo(reAddReadyMinute);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "重新加入通勤期间施工项目必须仍活动");
        AssertProjectInterval(project, reAddProgress, reAddLabor, commuteMeasure, 1,
            "重新加入后通勤区间 " + reAddMinute + "→" + reAddReadyMinute + "（p1 尚未到场）");
        Assert.AreEqual("项目劳动", WorkExecution(p2), "通勤区间内原在场者 p2 必须继续项目劳动");
        Evidence("CASE-B", "re-add commute to " + reAddReadyMinute + ": progress=" + Get(project, "Progress")
            + " labor=" + Get(project, "LaborRemainder") + " (only p2 contributed)");

        int reArrivalProgress = Convert.ToInt32(Get(project, "Progress"));
        long reArrivalLabor = Convert.ToInt64(Get(project, "LaborRemainder"));
        AdvanceTo(reAddReadyMinute + 1);
        Call(_host, "RefreshNow");
        project = ActiveProject();
        Assert.IsNotNull(project, "重新到场后施工项目必须仍活动");
        AssertProjectInterval(project, reArrivalProgress, reArrivalLabor, 1, 2,
            "重新到场后第一分钟 " + reAddReadyMinute + "→" + (reAddReadyMinute + 1) + "（p1 已到场）");
        Assert.AreEqual("项目劳动", WorkExecution(p1), "重新到场后 p1 必须重新处于真实项目劳动");
        Assert.AreEqual("项目劳动", WorkExecution(p2), "重新到场后 p2 必须仍处于真实项目劳动");
        Evidence("CASE-B", "END p1 re-arrived at " + reAddReadyMinute + "; participants=2; primary=ws_farm_01"
            + "; progress=" + Get(project, "Progress") + " labor=" + Get(project, "LaborRemainder")
            + " (2-worker interval derived after real arrival)");
        Assert.AreEqual("Management", Surface(), "施工命令不得非预期关闭管理页");
    }

    // ==================== 用例 7：劳动中撤职（动态岗位真实到场 + 非零证据 + 旧意图清理时序） ====================

    /// <summary>
    /// 真实管理按钮“劳动中撤职”：
    ///   - 前置用真实建造/施工/推进完成动态岗位，从当前 World 快照挑“在村、无 Activity claim、无既有职责”的人物，
    ///     走真实派工按钮 → 真实通勤登记 → 到场 → 非零岗位劳动证据（不造 arrival、不写证据）。
    ///   - 撤职当刻断言：执行标签/绑定/劳动人数立即停止，已结算分钟不追回；
    ///     同时按产品真实行为读取旧 arrival 行（RemovePersonAssignment 不清 WorkIntent），
    ///     明确记录“残留旧意图 ≠ 当前劳动”，并观测它在下一次工作区间完成时被清理。
    ///   - 不宣称“下一时间区间之外也立即清零”，不为通过测试改 Work/Clock 核心契约。
    /// </summary>
    [UnityTest]
    public IEnumerator WorkplaceRemovalDuringLabor_StopsNextIntervalEvidenceAndKeepsSettledMinutes()
    {
        object schedule = Get(StaticField("VillageRuntimeBridge", "IntradayWork"), "Schedule");
        Assert.IsNotNull(schedule, "IntradayWork.Schedule 必须存在");
        long commuteMinutes = Convert.ToInt64(Get(schedule, "CommuteMinutes"));
        long lunch = Convert.ToInt64(Get(schedule, "Lunch"));
        Assert.Greater(commuteMinutes, 0L, "真实通勤分钟数必须非零（派工当刻不能直接到场）");

        // 真实建造 + 真实完工（既有公开施工入口 + 唯一 Clock 推进；不写进度、不造岗位）。
        string farm = PlaceBuilding("building_farm", 4f);
        string builder = SnapshotPersonIdAt(0);
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farm, builder).ToString(),
            "前置：必须能用真实人物加入 farm 施工");
        AdvanceTo(14400);
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(farm), "farm 必须在真实推进后完工");
        Evidence("CASE-D", "START farm=" + farm + " builder=" + builder + " state=Completed");

        // 进入完工后的真实工作区间，再挑合法人物：在村、无 Activity claim、无既有职责。
        AdvanceTo(14400 + 481);
        Call(_host, "RefreshNow");
        string person = null;
        var candidates = new StringBuilder();
        foreach (string id in HostPersonIds())
        {
            string availability = PersonAvailability(id);
            string claim = ActivityClaim(id);
            string primary = PersonWorkplace(id);
            candidates.Append(id + "{available=" + availability + " claim=" + (claim ?? "<null>")
                + " primary=" + (primary ?? "<null>") + "};");
            if (string.Equals(availability, "InVillage", StringComparison.Ordinal)
                && claim == null && string.IsNullOrEmpty(primary))
            {
                person = id;
                break;
            }
        }
        Assert.IsNotNull(person,
            "本夹具需要 1 名在村、无 Activity claim、无既有职责的真实人物；实际：" + candidates);
        Evidence("CASE-D", "candidate=" + PersonDiagnostic(person) + "; all=" + candidates);

        Call(Get(_host, "Selection"), "SelectPerson", person);
        OpenManagementThroughNavigation("管理·劳动中撤职准备");
        yield return null;
        var management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        ShowWorkplacesPage(management);
        yield return null;
        string workplace = WorkplaceIdForBuilding(management, farm, " farm（动态岗位）");
        Assert.IsTrue(workplace.StartsWith("workplace:" + farm + ":", StringComparison.Ordinal),
            "动态岗位 ID 必须绑定真实建筑实例 + 槽位：" + workplace);

        SelectForManagement(management, person);
        yield return null;
        ClickWorkplaceRow(management, workplace, "岗位行·动态 farm");
        yield return null;
        // Clock.CurrentInstant.Minute 是跨日绝对分钟；VillageIntradayWork.Arrived 也以该绝对分钟登记
        // ReadyMinute。CASE-D 在 14400 分钟后运行，不能用 MinuteOfDay 与 arrival 的绝对分钟混比。
        long assignMinute = ClockAbsoluteMinute();
        ClickButtonField(management, "assignButton", "派工·劳动中撤职人物");
        yield return null;
        Assert.AreEqual(assignMinute, ClockAbsoluteMinute(), "Clock 暂停时真实派工不得自行推进时间");
        Assert.AreEqual(workplace, PersonWorkplace(person), "真实派工按钮必须写入动态岗位");
        Assert.AreEqual(1, WorkplaceOccupants(workplace), "动态岗位名额占用必须为 1");
        AssertStatusContains(management, "已绑定");
        long readyMinute = assignMinute + commuteMinutes;
        object assignedArrival = WorkArrival(person);
        Assert.IsNotNull(assignedArrival, "派工后必须登记真实到场计划");
        Assert.AreEqual(workplace, (string)Get(assignedArrival, "Destination"), "到场目的地必须是本次动态岗位");
        Assert.AreEqual(readyMinute, Convert.ToInt64(Get(assignedArrival, "ReadyMinute")),
            "到场分钟必须等于真实派工时刻 + Schedule.CommuteMinutes");
        Assert.AreEqual(0, WorkplaceEvidenceMinutes(person, workplace), "派工当刻不得凭空产生劳动证据");
        Evidence("CASE-D", "assigned at " + assignMinute + " workplace=" + workplace
            + " arrival=" + workplace + "@" + readyMinute + " evidence=0");

        // 真实到场：到场当分钟尚未累计证据；再工作 1 分钟才产生真实非零证据。
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "准备后 ESC 必须回 World");
        AdvanceTo(readyMinute);
        Call(_host, "RefreshNow");
        Assert.AreEqual("岗位劳动", WorkExecution(person), "真实到场分钟必须已进入岗位劳动");
        Assert.AreEqual(0, WorkplaceEvidenceMinutes(person, workplace), "到场当分钟不得凭空产生劳动证据");
        AdvanceTo(readyMinute + 1);
        Call(_host, "RefreshNow");
        long dismissalMinute = ClockAbsoluteMinute();
        Assert.AreEqual(readyMinute + 1, dismissalMinute, "撤职必须发生在真实到场后的下一工作分钟");
        int evidenceBefore = WorkplaceEvidenceMinutes(person, workplace);
        Assert.Greater(evidenceBefore, 0, "撤职前必须具备真实非零岗位劳动证据");
        Assert.AreEqual("岗位劳动", WorkExecution(person), "撤职前人物必须处于真实岗位劳动");
        object arrivalBefore = WorkArrival(person);
        Assert.IsNotNull(arrivalBefore, "撤职前必须存在真实到场登记");
        Assert.AreEqual(workplace, (string)Get(arrivalBefore, "Destination"), "撤职前到场目的地必须是本岗位");
        long readyBefore = Convert.ToInt64(Get(arrivalBefore, "ReadyMinute"));
        Evidence("CASE-D", "labor before dismissal minute=" + dismissalMinute
            + " evidence=" + evidenceBefore + " arrival=" + workplace + "@" + readyBefore
            + " bound=" + WorkplaceOccupants(workplace) + " execution=" + WorkExecution(person));

        // 真实管理按钮撤职（不是直接调 Bridge）。
        OpenManagementThroughNavigation("管理·劳动中撤职");
        yield return null;
        ShowWorkplacesPage(management);
        yield return null;
        SelectForManagement(management, person);
        yield return null;
        ClickWorkplaceRow(management, workplace, "岗位行·动态 farm（撤职）");
        yield return null;
        object rowBefore = FindRow(management, "Workplaces", "WorkplaceId", workplace);
        Assert.IsNotNull(rowBefore, "撤职前岗位行必须存在");
        Assert.AreEqual(1, Convert.ToInt32(Get(rowBefore, "BoundCount")), "撤职前岗位名额绑定必须为 1");
        // 说明：行内 LaborCount 走“现场地点投影”（在工作边界登记），本夹具在 480 边界后才派工，
        // 因此不把 LaborCount 当劳动证据；实际劳动由 IsExecutingWork / 执行标签 / Evidence 分钟证明。
        var removeButton = (Button)Get(management, "removeButton");
        Assert.IsNotNull(removeButton, "撤职按钮必须绑定");
        Assert.IsTrue(removeButton.interactable, "劳动中撤职按钮必须可交互");
        ClickButtonField(management, "removeButton", "劳动中撤职");
        yield return null;

        // 撤职当刻：标签 / 绑定 / 劳动人数立即停止；不追回已结算分钟。
        Assert.IsNull(PersonWorkplace(person), "撤职必须移除主要职责绑定");
        Assert.AreEqual(0, WorkplaceOccupants(workplace), "撤职必须释放岗位名额");
        Assert.AreEqual("空闲", WorkExecution(person), "撤职当刻实际执行标签必须停止旧岗位劳动");
        Assert.IsFalse((bool)Bridge("IsExecutingWork", person), "撤职当刻不得仍判定为正在执行工作");
        AssertStatusContains(management, "已移除");
        object rowAfter = FindRow(management, "Workplaces", "WorkplaceId", workplace);
        Assert.IsNotNull(rowAfter, "撤职后动态岗位行仍应存在（只是无人绑定）");
        Assert.AreEqual(0, Convert.ToInt32(Get(rowAfter, "BoundCount")), "撤职后岗位投影绑定必须为 0");
        Assert.AreEqual(0, Convert.ToInt32(Get(rowAfter, "LaborCount")), "撤职后岗位投影实际劳动人数必须为 0");
        Assert.AreEqual(evidenceBefore, WorkplaceEvidenceMinutes(person, workplace),
            "撤职不得追回已结算的岗位劳动证据（同一分钟不新增也不回滚）");
        Assert.AreEqual("空闲", WorkExecution(person), "读取证据后执行标签仍必须是空闲（不得被旧证据复活）");

        // 旧 arrival 行：RemovePersonAssignment 没有 RefreshWorkIntent/Leave，按真实行为原样记录。
        object arrivalAtRemoval = WorkArrival(person);
        Assert.IsNotNull(arrivalAtRemoval,
            "本实现撤职当刻仍保留旧 arrival 行（残留旧意图）；若产品改为即时清理，必须按新契约评审后更新本断言");
        Assert.AreEqual(workplace, (string)Get(arrivalAtRemoval, "Destination"),
            "残留 arrival 行仍指向撤职前岗位（记录事实，不代表当前意图）");
        Assert.AreEqual(readyBefore, Convert.ToInt64(Get(arrivalAtRemoval, "ReadyMinute")),
            "残留 arrival 行的到场分钟必须保持原值（不是新登记）");
        Assert.IsFalse((bool)Bridge("IsExecutingWork", person),
            "残留 arrival 行不得让撤职者继续被判定为正在执行工作");
        Evidence("CASE-D", "dismissed at minute " + dismissalMinute
            + " bound=0 labor=0 execution=空闲 settled-evidence=" + WorkplaceEvidenceMinutes(person, workplace)
            + " arrival-residual=" + (string)Get(arrivalAtRemoval, "Destination") + "@" + readyBefore
            + " current-intent=false (残留旧意图不等于当前劳动)");

        // 下一次真实工作区间完成时（同日下午 → 午休边界）：旧岗位证据不得再增长，旧 arrival 被清理。
        long dayBase = dismissalMinute - (dismissalMinute % 1440L);
        long nextBoundaryMinute = dayBase + lunch;
        Assert.Greater(nextBoundaryMinute, dismissalMinute, "午休边界必须晚于撤职分钟");
        Call(_shell, "HandleEscape");
        yield return null;
        AdvanceTo(nextBoundaryMinute);
        Call(_host, "RefreshNow");
        Assert.AreEqual(evidenceBefore, WorkplaceEvidenceMinutes(person, workplace),
            "撤职后的下一工作区间不得再从旧岗位累计劳动证据");
        Assert.IsNull(WorkArrival(person), "旧到场意图必须在下一工作区间完成边界被清理");
        Assert.AreEqual("空闲", WorkExecution(person), "撤职后下一区间不得恢复旧岗位劳动");
        Assert.IsFalse((bool)Bridge("IsExecutingWork", person), "撤职后下一区间不得重新判定为工作中");

        OpenManagementThroughNavigation("管理·撤职后计量复核");
        yield return null;
        ShowWorkplacesPage(management);
        yield return null;
        object rowFinal = FindRow(management, "Workplaces", "WorkplaceId", workplace);
        Assert.IsNotNull(rowFinal, "撤职后动态岗位行必须仍可读");
        Assert.AreEqual(0, Convert.ToInt32(Get(rowFinal, "BoundCount")), "复核时岗位绑定必须仍为 0");
        Assert.AreEqual(0, Convert.ToInt32(Get(rowFinal, "LaborCount")), "复核时岗位劳动人数必须仍为 0");
        Assert.AreEqual(evidenceBefore, WorkplaceEvidenceMinutes(person, workplace),
            "复核时已结算证据必须保持不变");
        Evidence("CASE-D", "END boundary=" + nextBoundaryMinute + " arrival="
            + (WorkArrival(person) == null ? "<cleared>" : "<still-present>")
            + " evidence=" + WorkplaceEvidenceMinutes(person, workplace)
            + " (no next-interval growth; settled minute kept; cleanup observed at next interval boundary)");
        Assert.AreEqual("Management", Surface(), "撤职验证期间不得非预期关闭管理页");
    }

    // ==================== 用例 6：显式保存 / Continue / NewGame 连续性 ====================

    /// <summary>
    /// 真实产品入口：管理 Save 按钮 → 强制失败（短暂钩子）→ 恢复成功 Save
    /// → 场景重建后 MainMenu Continue（Load → EnterTown，新 host 引用）
    /// → MainMenu NewGame（HeroSelection 真实卡片/确认按钮，gating 不被绕过）。
    ///
    /// 只比较真实 Runtime Query：具体 PersonId、BuildingInstanceId + slot/WorkplaceId、
    /// Ingredients/Food、施工参与者/Progress/LaborRemainder、真实生产批次/证据与累计执行历史、Clock。
    /// 不把 UI 选择/候选/检索 session 当实现事实，也不把 DTO 存在当通过。
    /// </summary>
    [UnityTest]
    public IEnumerator ExplicitSaveContinueAndNewGame_PreserveFactsWithoutDuplicateSettlementOrOldWorldLeak()
    {
        // ---------------- 0. 本轮隔离世界的初始真实事实（UnitySetUp 已 NewGame + Save + Continue） ----------------
        int initialIngredients = Ingredients();
        int initialFood = Food();
        int initialBatches = ProductionBatchCount();
        int initialEvidence = ProductionEvidenceCount();
        int initialCompleted = AssignmentContributionHistory();
        long initialClock = ClockAbsoluteMinute();
        int authorityInitialIngredients = InitialResourceIngredients();
        int authorityInitialFood = InitialResourceFood();
        Assert.AreEqual(authorityInitialIngredients, initialIngredients,
            "隔离新世界初始 Ingredients 必须等于 VillageResourceState 初始 Authority（不得是旧档/旧世界库存）");
        Assert.AreEqual(authorityInitialFood, initialFood,
            "隔离新世界初始 Food 必须等于 VillageResourceState 初始 Authority（不得是旧档/旧世界库存）");
        Assert.AreEqual(0, initialBatches, "隔离新世界不应预置生产批次");
        Assert.AreEqual(0, initialEvidence, "隔离新世界不应预置生产证据");
        Assert.AreEqual(0, initialCompleted, "隔离新世界不应预置累计执行历史");
        Evidence("CASE-C", "START initial ingredients=" + initialIngredients + " food=" + initialFood
            + " authorityInitial=" + authorityInitialIngredients + "/" + authorityInitialFood
            + " batches=" + initialBatches + " evidence=" + initialEvidence
            + " completedProjects=" + initialCompleted + " clock=" + initialClock);

        // ---------------- 1. 真实建造 + 真实管理按钮建立动态 Completed 岗位与施工参与 ----------------
        string farmB = PlaceBuilding("building_farm", 0f);
        string kitchenC = PlaceBuilding("building_kitchen", 4f);
        Assert.AreNotEqual(farmB, kitchenC, "Farm / Kitchen 必须是两个独立建筑实例");
        string p1 = SnapshotPersonIdAt(0), p2 = SnapshotPersonIdAt(1), p3 = SnapshotPersonIdAt(2),
            p4 = SnapshotPersonIdAt(3), p5 = SnapshotPersonIdAt(4);
        Evidence("CASE-C", "persons p1=" + p1 + " p2=" + p2 + " p3=" + p3 + " p4=" + p4 + " p5=" + p5
            + " farmB=" + farmB + " kitchenC=" + kitchenC);

        // 两栋目标建筑先用真实施工参与者完成（Case A/B 同款公开入口），再建立动态岗位/新施工。
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farmB, p1).ToString(),
            "p1 必须能加入 Farm B 施工");
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", kitchenC, p2).ToString(),
            "p2 必须能加入 Kitchen C 施工");
        AdvanceTo(14400);
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(farmB), "Farm B 必须在真实推进后完工");
        Assert.AreEqual("Completed", BuildingState(kitchenC), "Kitchen C 必须在真实推进后完工");
        Assert.AreEqual(0, AssignmentContributionHistory(),
            "完工当下尚未跨日 Tick，累计执行历史必须仍为 0（不得凭完工伪造历史）");

        // 第三栋保持 UnderConstruction：它是本次保存的真实施工事实。
        string constructionA = PlaceBuilding("building_farm", 8f);
        Assert.AreEqual("UnderConstruction", BuildingState(constructionA),
            "第三次建造必须保持真实施工中（不能推进到完工）");

        Call(Get(_host, "Selection"), "SelectPerson", p4);
        OpenManagementThroughNavigation("管理·保存连续性准备");
        yield return null;
        object management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        ShowWorkplacesPage(management);
        yield return null;

        string wFarm = WorkplaceIdForBuilding(management, farmB, "Farm B");
        string wKitchen = WorkplaceIdForBuilding(management, kitchenC, "Kitchen C");
        Assert.IsTrue(wFarm.StartsWith("workplace:" + farmB + ":", StringComparison.Ordinal),
            "Farm B 岗位 ID 必须绑定具体建筑实例 + 真实槽位：" + wFarm);
        Assert.IsTrue(wKitchen.StartsWith("workplace:" + kitchenC + ":", StringComparison.Ordinal),
            "Kitchen C 岗位 ID 必须绑定具体建筑实例 + 真实槽位：" + wKitchen);
        Assert.AreNotEqual(wFarm, wKitchen, "两栋建筑的动态岗位 ID 必须独立");

        // p4 → 动态 Farm B 岗位（真实行 + 真实派工按钮）
        SelectForManagement(management, p4);
        yield return null;
        ClickWorkplaceRow(management, wFarm, "岗位行·Farm B");
        yield return null;
        ClickButtonField(management, "assignButton", "派工·p4→Farm B");
        yield return null;
        Assert.AreEqual(wFarm, PersonWorkplace(p4), "真实派工按钮必须写入 p4 的动态 Farm B 岗位");
        Assert.AreEqual(1, WorkplaceOccupants(wFarm), "Farm B 真实名额占用必须为 1");

        // p5 → 动态 Kitchen C 岗位
        SelectForManagement(management, p5);
        yield return null;
        ClickWorkplaceRow(management, wKitchen, "岗位行·Kitchen C");
        yield return null;
        ClickButtonField(management, "assignButton", "派工·p5→Kitchen C");
        yield return null;
        Assert.AreEqual(wKitchen, PersonWorkplace(p5), "真实派工按钮必须写入 p5 的动态 Kitchen C 岗位");
        Assert.AreEqual(1, WorkplaceOccupants(wKitchen), "Kitchen C 真实名额占用必须为 1");

        // p1/p2 → 同一 UnderConstruction 实例的真实施工参与（真实行 + 真实按钮；prj_test_01 Capacity=2）
        SelectForManagement(management, p1);
        yield return null;
        ClickConstructionRow(management, constructionA, "施工行·A（p1）");
        yield return null;
        ClickButtonField(management, "addConstructionButton", "加入施工·p1");
        yield return null;
        SelectForManagement(management, p2);
        yield return null;
        ClickConstructionRow(management, constructionA, "施工行·A（p2）");
        yield return null;
        ClickButtonField(management, "addConstructionButton", "加入施工·p2");
        yield return null;

        object project = ActiveProject();
        Assert.IsNotNull(project, "真实施工按钮加入后必须存在活动施工项目");
        CollectionAssert.AreEquivalent(new[] { p1, p2 }, Strings(Get(project, "ParticipantPersonIds")),
            "施工 A 必须记录两名具体真实参与者");
        Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "暂停在登记分钟时不得凭空产生进度");

        // Kitchen 的第一份真实输入：既有公开资源写入口（非反射私有字段、非 DTO 伪造）。
        int resourceInput = 2;
        Call(ResourcesState(), "AddIngredients", resourceInput);
        Evidence("CASE-C", "PREPARE farmB=" + farmB + " kitchenC=" + kitchenC + " constructionA=" + constructionA
            + " wFarm=" + wFarm + " wKitchen=" + wKitchen + " participants=2 ingredientsInput=" + resourceInput);

        // 关闭管理后由真实 Clock 推进跨越一个 Day Tick 到次日午前：真实施工进度、真实生产与执行历史。
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "准备完成后 ESC 必须回 World");
        AdvanceTo(16360);
        Call(_host, "RefreshNow");
        yield return null;

        // ---------------- 2. 捕获保存前的具体事实（全部来自真实 Runtime Query） ----------------
        long savedClock = ClockAbsoluteMinute();
        Assert.AreEqual(16360L, savedClock, "真实推进必须停在 16360 分钟（次日午前）");
        project = ActiveProject();
        Assert.IsNotNull(project, "真实工作后施工项目必须仍活动");
        string projectId = (string)Get(project, "ProjectId");
        int savedProgress = Convert.ToInt32(Get(project, "Progress"));
        long savedLabor = Convert.ToInt64(Get(project, "LaborRemainder"));
        List<string> savedParticipants = Strings(Get(project, "ParticipantPersonIds"));
        Assert.Greater(savedProgress, 0, "真实到场劳动后施工进度必须大于 0");
        CollectionAssert.AreEquivalent(new[] { p1, p2 }, savedParticipants, "施工参与者必须逐项保持");
        int savedIngredients = Ingredients();
        int savedFood = Food();
        Assert.Greater(savedIngredients, 0, "真实 Farm 生产后 Ingredients 必须为非零可辨别值");
        Assert.Greater(savedFood, 0, "真实 Kitchen 生产后 Food 必须为非零可辨别值");
        Assert.AreNotEqual(savedIngredients, savedFood, "Ingredients / Food 必须是可分辨的不同真实值");
        int savedBatches = ProductionBatchCount();
        int savedEvidence = ProductionEvidenceCount();
        int savedCompleted = AssignmentContributionHistory();
        Assert.Greater(savedBatches, 0, "真实 Farm/Kitchen 工作必须留下生产批次");
        Assert.Greater(savedEvidence, 0, "真实岗位劳动必须留下生产证据");
        Assert.Greater(savedCompleted, 0, "真实跨日执行必须留下累计执行历史");
        object p1Arrival = WorkArrival(p1);
        Assert.IsNotNull(p1Arrival, "p1 施工登记必须存在真实到场计划");
        Assert.AreEqual("construction:" + constructionA, (string)Get(p1Arrival, "Destination"),
            "p1 到场目的地必须是本次施工实例");
        long p1ReadyMinute = Convert.ToInt64(Get(p1Arrival, "ReadyMinute"));
        string savePath = CurrentSavePath();
        Assert.IsFalse(string.IsNullOrEmpty(savePath), "SaveSystem.GetSavePath 必须返回真实单槽路径");
        string hashBeforeSuccessSave = File.Exists(savePath) ? HashFile(savePath) : "<absent>";
        object oldSource = StaticField("VillageRuntimeBridge", "SimulationPersonSource");
        object oldTownData = Get(_town, "PlayerData");
        Assert.IsTrue(ReferenceEquals(oldSource, oldTownData), "准备阶段人物源必须仍是当前 Town 的真实 PlayerCollection");
        Evidence("CASE-C", "SAVED-FACTS clock=" + savedClock + " project=" + projectId
            + " progress=" + savedProgress + " labor=" + savedLabor + " participants=" + string.Join(",", savedParticipants)
            + " ingredients=" + savedIngredients + " food=" + savedFood + " batches=" + savedBatches
            + " evidence=" + savedEvidence + " completed=" + savedCompleted
            + " p1Arrival=" + Get(p1Arrival, "Destination") + "@" + p1ReadyMinute
            + " hashBefore=" + hashBeforeSuccessSave);

        // ---------------- 3. 真实管理 Save 按钮：成功文案 + bool 成功路径 + 单槽写盘 ----------------
        OpenManagementThroughNavigation("管理·显式保存（成功）");
        yield return null;
        management = Management();
        Assert.IsNotNull(management);
        ClickButtonField(management, "saveButton", "显式保存·成功");
        yield return null;
        AssertStatusContains(management, "已保存当前世界。");
        Assert.IsTrue((bool)Call(management, "Save"),
            "ManagementView.Save 走真实 SaveSystem bool 回执，成功时必须返回 true");
        yield return null;
        Assert.IsTrue((bool)Static("SaveSystem", "HasSave"), "成功保存后单槽必须可被 Continue 检测");
        string hashAfterSuccessSave = HashFile(savePath);
        Assert.IsFalse(string.IsNullOrEmpty(hashAfterSuccessSave), "成功保存后必须存在真实单槽字节");
        string saveJson = ReadSaveJson();
        StringAssert.Contains("\"SaveVersion\": 1", saveJson, "真实单槽必须是当前 SaveVersion 的可加载 JSON");
        StringAssert.Contains("\"Ingredients\":", saveJson, "真实单槽必须写入资源事实");
        Assert.IsFalse(saveJson.Contains("SelectedWorkplaceId")
                       || saveJson.Contains("SelectedConstructionBuildingId")
                       || saveJson.Contains("nameSearch")
                       || saveJson.Contains("ManagementView")
                       || saveJson.Contains("ActiveTab"),
            "UI Selection / 岗位候选 / 施工候选 / 检索 session 不得进入存档");
        Evidence("CASE-C", "SAVE-SUCCESS bytes=" + new FileInfo(savePath).Length
            + " sha256=" + hashAfterSuccessSave);

        // ---------------- 4. 短暂 Save 失败：明确失败文案、内存不丢、不自动回滚、不写盘 ----------------
        int failIngredients = Ingredients();
        int failFood = Food();
        int failBatches = ProductionBatchCount();
        int failEvidence = ProductionEvidenceCount();
        int failCompleted = AssignmentContributionHistory();
        long failClock = ClockAbsoluteMinute();
        int failProgress = Convert.ToInt32(Get(ActiveProject(), "Progress"));
        var failureHook = T("SaveSystem").GetField("ForceSaveFailureForTests", Flags);
        Assert.IsNotNull(failureHook, "SaveSystem.ForceSaveFailureForTests 测试钩子必须存在");
        try
        {
            LogAssert.Expect(LogType.Warning, new Regex("Save forced to fail"));
            SetForceSaveFailure(() => true);
            ClickButtonField(management, "saveButton", "显式保存·短暂强制失败");
            yield return null;
            AssertStatusContains(management, "未保存");
            AssertStatusContains(management, "内存状态保留");
            Assert.AreEqual(hashAfterSuccessSave, HashFile(savePath),
                "Save 返回 false 时不得写盘（不得伪成功 / 不得部分覆盖）");
            Assert.AreEqual(failIngredients, Ingredients(), "Save 失败不得清空/回滚 Ingredients");
            Assert.AreEqual(failFood, Food(), "Save 失败不得清空/回滚 Food");
            Assert.AreEqual(failBatches, ProductionBatchCount(), "Save 失败不得改写生产批次");
            Assert.AreEqual(failEvidence, ProductionEvidenceCount(), "Save 失败不得改写生产证据");
            Assert.AreEqual(failCompleted, AssignmentContributionHistory(), "Save 失败不得改写执行历史");
            Assert.AreEqual(failProgress, Convert.ToInt32(Get(ActiveProject(), "Progress")),
                "Save 失败不得回滚施工进度");
            Assert.AreEqual(failClock, ClockAbsoluteMinute(), "Save 失败不得推进/回退 Clock");
            Assert.AreEqual(wFarm, PersonWorkplace(p4), "Save 失败不得回滚动态岗位分配");
            Assert.AreEqual(wKitchen, PersonWorkplace(p5), "Save 失败不得回滚动态岗位分配");
            CollectionAssert.AreEquivalent(new[] { p1, p2 },
                Strings(Get(ActiveProject(), "ParticipantPersonIds")), "Save 失败不得回滚施工参与");
            Evidence("CASE-C", "SAVE-FAIL-EXPECTED status=未保存 memory-preserved=true file-unchanged=true"
                + " hash=" + HashFile(savePath) + " clock=" + ClockAbsoluteMinute());
        }
        finally
        {
            SetForceSaveFailure(null);
        }
        // 钩子已复位：再次真实保存成功，保留当前事实而不是回滚。
        ClickButtonField(management, "saveButton", "显式保存·失败后恢复成功");
        yield return null;
        AssertStatusContains(management, "已保存当前世界。");
        string hashRecovered = HashFile(savePath);
        Assert.IsFalse(string.IsNullOrEmpty(hashRecovered), "恢复成功 Save 必须重新写出真实单槽");
        Assert.AreEqual(failIngredients, Ingredients(), "恢复成功 Save 不得改变当前资源事实");
        Assert.AreEqual(failProgress, Convert.ToInt32(Get(ActiveProject(), "Progress")),
            "恢复成功 Save 不得改变当前施工进度");
        Evidence("CASE-C", "SAVE-RECOVERED sha256=" + hashRecovered);

        // ---------------- 5. 真实 MainMenu Continue：场景重建 → Load → EnterTown，换源/新 host ----------------
        int oldHostInstanceId = ((Component)_host).gameObject.GetInstanceID();
        int oldShellInstanceId = ((Component)_shell).gameObject.GetInstanceID();
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return null;
        object menu = Find("MainMenuUI");
        Assert.IsNotNull(menu, "MainMenuUI 必须由真实场景重建");
        Call(menu, "RefreshContinueButton");
        var continueButton = (Button)Get(menu, "btnContinue");
        Assert.IsNotNull(continueButton, "真实 Continue 按钮必须绑定");
        Assert.IsTrue(continueButton.interactable, "真实单槽存在时 Continue 必须可用");
        continueButton.onClick.Invoke(); // 真实产品 Button 事件：OnContinue → SaveSystem.Load → EnterTown
        yield return null;
        yield return null;
        for (int i = 0; i < 10 && FindOptional("ProductHomeWorldHost") == null; i++) yield return null;

        _town = Find("TownUI");
        Assert.IsNotNull(_town, "Continue 后 TownUI 必须由产品入口进入");
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "Continue 后必须重建 ProductHomeWorldHost");
        _shell = Get(_host, "Shell");
        Assert.IsNotNull(_shell, "Continue 后必须重建 HomeProductShell");
        Call(_host, "RefreshNow");
        Assert.AreNotEqual(oldHostInstanceId, ((Component)_host).gameObject.GetInstanceID(),
            "Continue 必须重获新 host 引用，而不是复用旧实例");
        Assert.AreNotEqual(oldShellInstanceId, ((Component)_shell).gameObject.GetInstanceID(),
            "Continue 必须重获新 Shell 引用");
        object loadedSource = StaticField("VillageRuntimeBridge", "SimulationPersonSource");
        Assert.IsFalse(ReferenceEquals(oldSource, loadedSource), "Continue 必须换源到 Load 出的 PlayerCollection");
        Assert.IsTrue(ReferenceEquals(loadedSource, Get(_town, "PlayerData")),
            "TownUI.PlayerData 必须就是真实 Load 出的人物源");
        Assert.AreEqual("Paused", ClockSpeed(), "Load 必须恢复存档中的 Paused，避免自然时间流逝混淆");
        Assert.AreEqual(savedClock, ClockAbsoluteMinute(), "Load 自身不得推进 Clock");
        Assert.AreEqual(hashRecovered, HashFile(savePath), "Load 不得改写单槽存档");
        Evidence("CASE-C", "CONTINUE-ENTRY oldHost=" + oldHostInstanceId
            + " newHost=" + ((Component)_host).gameObject.GetInstanceID()
            + " newShell=" + ((Component)_shell).gameObject.GetInstanceID()
            + " clock=" + ClockAbsoluteMinute() + " speed=" + ClockSpeed());

        // 新 host 上的管理页：候选/检索 session 不得从存档恢复；真实事实必须逐项连续。
        OpenManagementThroughNavigation("管理·Continue 后连续性");
        yield return null;
        management = Management();
        Assert.IsNotNull(management, "新 host 必须重获 ManagementView 引用");
        Assert.IsNull(Get(management, "SelectedWorkplaceId"), "UI 岗位候选不得入档并在 Continue 后恢复");
        Assert.IsNull(Get(management, "SelectedConstructionBuildingId"), "UI 施工候选不得入档并在 Continue 后恢复");
        Assert.AreEqual(string.Empty, ((TMP_InputField)Get(management, "nameSearch")).text,
            "UI 检索 session 不得入档并在 Continue 后恢复");
        Assert.IsNull(SelectionId(), "新 host 的共享 Selection 不得携带旧世界选中人物");

        Assert.AreEqual(wFarm, WorkplaceIdForBuilding(management, farmB, "Continue·Farm B"),
            "动态 Completed 岗位 ID（建筑实例 + slot）必须逐项连续");
        Assert.AreEqual(wKitchen, WorkplaceIdForBuilding(management, kitchenC, "Continue·Kitchen C"),
            "动态 Completed 岗位 ID（建筑实例 + slot）必须逐项连续");
        Assert.AreEqual(wFarm, PersonWorkplace(p4), "p4 的具体动态职责必须连续");
        Assert.AreEqual(wKitchen, PersonWorkplace(p5), "p5 的具体动态职责必须连续");
        Assert.AreEqual(1, WorkplaceOccupants(wFarm), "Farm B 名额占用必须连续");
        Assert.AreEqual(1, WorkplaceOccupants(wKitchen), "Kitchen C 名额占用必须连续");
        Assert.AreEqual("UnderConstruction", BuildingState(constructionA), "UnderConstruction 实例必须仍存在");
        project = ActiveProject();
        Assert.IsNotNull(project, "Continue 后真实施工项目必须仍活动");
        Assert.AreEqual(projectId, (string)Get(project, "ProjectId"), "真实施工项目 ID 必须连续");
        CollectionAssert.AreEquivalent(savedParticipants, Strings(Get(project, "ParticipantPersonIds")),
            "真实施工参与者必须逐项连续");
        Assert.AreEqual(savedProgress, Convert.ToInt32(Get(project, "Progress")), "真实施工进度必须连续");
        Assert.AreEqual(savedLabor, Convert.ToInt64(Get(project, "LaborRemainder")), "劳动剩余必须连续");
        object p1ArrivalAfter = WorkArrival(p1);
        Assert.IsNotNull(p1ArrivalAfter, "Continue 后 p1 到场计划必须连续");
        Assert.AreEqual("construction:" + constructionA, (string)Get(p1ArrivalAfter, "Destination"),
            "Continue 后到场目的地必须连续");
        Assert.AreEqual(p1ReadyMinute, Convert.ToInt64(Get(p1ArrivalAfter, "ReadyMinute")),
            "Continue 后到场分钟必须连续（不得按 Load 当下重新计时）");
        Assert.AreEqual(savedIngredients, Ingredients(), "Continue 后 Ingredients 不得重复结算/增减");
        Assert.AreEqual(savedFood, Food(), "Continue 后 Food 不得重复结算/增减");
        Assert.AreEqual(savedBatches, ProductionBatchCount(), "Continue 不得重复生产批次");
        Assert.AreEqual(savedEvidence, ProductionEvidenceCount(), "Continue 不得重复生产证据");
        Assert.AreEqual(savedCompleted, AssignmentContributionHistory(), "Continue 不得重复结算执行历史");
        Assert.AreEqual(savedClock, ClockAbsoluteMinute(), "Continue 后 Clock 必须与存档一致");
        var constructionRow = FindRow(management, "Construction", "BuildingInstanceId", constructionA);
        Assert.IsNotNull(constructionRow, "Continue 后管理施工行必须仍指向同一实例");
        Assert.AreEqual(savedParticipants.Count, Convert.ToInt32(Get(constructionRow, "WorkerCount")),
            "Continue 后管理施工行工人数必须连续");
        Assert.AreEqual(savedProgress, Convert.ToInt32(Get(constructionRow, "Progress")),
            "Continue 后管理施工行进度必须连续");
        var resourceSnapshot = Get(Get(management, "CurrentSnapshot"), "Resources");
        Assert.AreEqual(savedIngredients, Convert.ToInt32(Get(resourceSnapshot, "Ingredients")),
            "管理资源投影必须来自真实 Authority");
        Assert.AreEqual(savedFood, Convert.ToInt32(Get(resourceSnapshot, "Food")),
            "管理资源投影必须来自真实 Authority");
        Evidence("CASE-C", "CONTINUE-FACTS project=" + projectId + " progress=" + savedProgress
            + " labor=" + savedLabor + " ingredients=" + savedIngredients + " food=" + savedFood
            + " batches=" + savedBatches + " evidence=" + savedEvidence + " completed=" + savedCompleted
            + " clock=" + savedClock + " candidates=null search=empty");

        // ---------------- 6. 真实 MainMenu NewGame：来源替换后无旧世界实例/职责/候选泄露 ----------------
        int configuredRoster = ConfiguredInitialRosterCount();
        Assert.Greater(configuredRoster, 0, "Authority 初始名册必须非空（不写死人数）");
        object previousSource = StaticField("VillageRuntimeBridge", "SimulationPersonSource");
        List<string> oldPersonIds = LegalPersonIdsFromSource(previousSource);
        Assert.IsNotEmpty(oldPersonIds, "旧世界真实人物源必须有可比较的合法 PersonId（含英雄）");
        string[] oldPersons = { p1, p2, p3, p4, p5 };
        string[] oldBuildings = { farmB, kitchenC, constructionA };
        menu = Find("MainMenuUI");
        Assert.IsNotNull(menu, "NewGame 必须走真实 MainMenuUI 产品入口");
        Call(menu, "OnNewGame");
        yield return null;
        object heroSelection = Find("HeroSelectionUI");
        Assert.IsNotNull(heroSelection, "NewGame 必须显示真实 HeroSelectionUI");
        var heroCards = (IList)Get(heroSelection, "_heroCards");
        Assert.Greater(heroCards.Count, 0, "真实 HeroSelection 必须有可选英雄");
        var confirmButton = (Button)Get(heroSelection, "btnConfirm");
        Assert.IsNotNull(confirmButton, "真实 NewGame 确认按钮必须绑定");
        Assert.IsFalse(confirmButton.interactable, "未选择英雄时真实确认按钮必须禁用（gating 不得被绕过）");
        var firstCard = (GameObject)heroCards[0];
        var cardButton = firstCard.GetComponent<Button>();
        Assert.IsNotNull(cardButton, "真实英雄卡必须是 Button");
        Assert.IsTrue(cardButton.interactable, "真实英雄卡必须可点");
        cardButton.onClick.Invoke(); // 真实卡片 Button 事件
        yield return null;
        Assert.IsTrue(confirmButton.interactable, "真实选择英雄后确认按钮必须由产品事件启用");
        confirmButton.onClick.Invoke(); // 真实确认 Button 事件：OnConfirm → DeleteSave/NewGame/Save/EnterTown
        Assert.IsTrue((bool)Static("SaveSystem", "HasSave"),
            "NewGame 确认必须真实写出新世界单槽（产品 Save 失败不得被忽略）");
        Call(Clock(), "Pause"); // NewGame 权威初始时间为 0；立即暂停避免后续帧自然流逝
        Assert.AreEqual(0L, ClockAbsoluteMinute(), "NewGame 必须从真实权威初始时间开始");
        yield return null;
        yield return null;

        _town = Find("TownUI");
        Assert.IsNotNull(_town, "NewGame 确认后必须进入真实 Town");
        _host = Find("ProductHomeWorldHost");
        Assert.IsNotNull(_host, "NewGame 后必须存在产品世界 host");
        _shell = Get(_host, "Shell");
        Assert.IsNotNull(_shell, "NewGame 后必须存在真实 Shell");
        object newSource = StaticField("VillageRuntimeBridge", "SimulationPersonSource");
        Assert.IsFalse(ReferenceEquals(previousSource, newSource), "NewGame 必须替换人物源");
        Assert.IsTrue(ReferenceEquals(newSource, Get(_town, "PlayerData")),
            "NewGame 后 TownUI.PlayerData 必须就是新人物源");
        // 合法 PersonId 集合必须从长期人物真相源独立枚举：角色（含死亡）+ 英雄。
        // 初始名册配置只决定角色人数；NewGame 还必定包含 1 名所选英雄（同为真实 PersonId）。
        var newCharacterIds = PersonIdsOf(newSource, "OwnedCharacters");
        var newHeroIds = PersonIdsOf(newSource, "OwnedHeroes");
        var newPersonIds = LegalPersonIdsFromSource(newSource);
        Assert.AreEqual(configuredRoster, newCharacterIds.Count,
            "NewGame 真实角色数必须等于 Authority 初始名册配置（配置 N 行 → 开局 N 名角色；不写死人数）");
        Assert.Greater(newHeroIds.Count, 0, "HeroSelection 确认后真实人物源必须包含所选英雄");
        Assert.AreEqual(configuredRoster + newHeroIds.Count, newPersonIds.Count,
            "合法人口必须等于配置初始角色 + 真实英雄（英雄也是合法 PersonId，不能只数 OwnedCharacters）");
        Assert.AreEqual(newPersonIds.Count, new HashSet<string>(newPersonIds, StringComparer.Ordinal).Count,
            "NewGame 后真实人物源不得出现重复 PersonId");
        foreach (string oldPersonId in oldPersonIds)
            Assert.IsFalse(newPersonIds.Contains(oldPersonId), "旧世界 PersonId 不得残留到新世界：" + oldPersonId);
        foreach (string oldPerson in oldPersons)
            Assert.IsNull(PersonWorkplace(oldPerson), "旧人物的岗位职责不得泄露到新 Assignments：" + oldPerson);

        Call(_host, "RefreshNow");
        management = Get(_shell, "Management");
        Assert.IsNotNull(management, "NewGame 后必须仍能取得真实 ManagementView");
        Call(management, "RefreshNow");
        yield return null;
        var managementPersonIds = SnapshotPersonIds(management);
        CollectionAssert.AreEquivalent(newPersonIds, managementPersonIds,
            "Management 人口投影必须与真实人物源逐 PersonId 一致（角色+英雄；不得遗漏英雄，也不得残留旧世界 PersonId）");
        Assert.AreEqual(newPersonIds.Count, TotalPersonCount(management),
            "Management 人口行数必须等于真实人物源合法 PersonId 数");
        Assert.IsNull(Get(management, "SelectedPersonId"), "NewGame 后不得保留旧人物选择");
        Assert.IsNull(Get(management, "SelectedWorkplaceId"), "NewGame 后不得保留旧岗位候选");
        Assert.IsNull(Get(management, "SelectedConstructionBuildingId"), "NewGame 后不得保留旧施工候选");
        Assert.AreEqual(string.Empty, ((TMP_InputField)Get(management, "nameSearch")).text,
            "NewGame 后不得保留旧检索 session");
        object newSnapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(newSnapshot, "NewGame 后 Management 必须有真实快照");
        var newWorkplaces = ((IEnumerable)Get(newSnapshot, "Workplaces")).Cast<object>().ToList();
        var newConstruction = ((IEnumerable)Get(newSnapshot, "Construction")).Cast<object>().ToList();
        Assert.IsEmpty(newConstruction, "NewGame 后不得保留旧施工目标（真实 Reset 后应为空）");
        foreach (string oldBuilding in oldBuildings)
        {
            Assert.IsFalse(newWorkplaces.Any(row => string.Equals((string)Get(row, "BuildingInstanceId"),
                    oldBuilding, StringComparison.Ordinal)), "旧建筑实例不得出现在新岗位行：" + oldBuilding);
            Assert.IsFalse(newConstruction.Any(row => string.Equals((string)Get(row, "BuildingInstanceId"),
                    oldBuilding, StringComparison.Ordinal)), "旧建筑实例不得出现在新施工行：" + oldBuilding);
        }
        foreach (object personRow in ((IEnumerable)Get(newSnapshot, "Persons")).Cast<object>())
        {
            string primary = (string)Get(personRow, "PrimaryWorkplaceId");
            Assert.IsFalse(primary == wFarm || primary == wKitchen,
                "旧实例职责不得泄露到新世界人物行：" + primary);
        }
        // NewGame 的初始资源必须独立取自 VillageResourceState 自身初始 Authority（默认 0/0）；
        // 不能把旧世界库存或旧档投影当成正确值。SaveSystem.NewGame 组合层已复用既有 Restore(null)
        // 把 Ingredients / Food 重置到该初始状态，不新增 Authority / DTO / 公共 API。
        int expectedNewIngredients = InitialResourceIngredients();
        int expectedNewFood = InitialResourceFood();
        int newIngredients = Ingredients();
        int newFood = Food();
        Assert.AreNotEqual(savedIngredients, expectedNewIngredients,
            "夹具必须确保旧世界 Ingredients 与初始 Authority 可辨别（否则无法证明无泄漏）");
        Assert.AreNotEqual(savedFood, expectedNewFood,
            "夹具必须确保旧世界 Food 与初始 Authority 可辨别（否则无法证明无泄漏）");
        Assert.AreEqual(expectedNewIngredients, newIngredients,
            "NewGame 的 Ingredients 必须回到 VillageResourceState 初始 Authority，不得沿用旧世界库存");
        Assert.AreEqual(expectedNewFood, newFood,
            "NewGame 的 Food 必须回到 VillageResourceState 初始 Authority，不得沿用旧世界库存");
        Assert.AreEqual(initialBatches, ProductionBatchCount(), "NewGame 不得保留旧世界生产批次");
        Assert.AreEqual(initialEvidence, ProductionEvidenceCount(), "NewGame 不得保留旧世界生产证据");
        Assert.AreEqual(initialCompleted, AssignmentContributionHistory(), "NewGame 不得保留旧世界累计执行历史");
        var newResourceSnapshot = Get(newSnapshot, "Resources");
        Assert.AreEqual(newIngredients, Convert.ToInt32(Get(newResourceSnapshot, "Ingredients")),
            "NewGame 后 Management 资源投影必须与真实 Authority 一致");
        Assert.AreEqual(newFood, Convert.ToInt32(Get(newResourceSnapshot, "Food")),
            "NewGame 后 Management 资源投影必须与真实 Authority 一致");
        var newResourceSummary = (TMP_Text)Get(management, "resourceSummary");
        Assert.IsNotNull(newResourceSummary, "资源汇总必须绑定");
        StringAssert.Contains("食材 " + newIngredients, newResourceSummary.text,
            "Management 资源汇总必须反映真实初始 Ingredients");
        StringAssert.Contains("食物 " + newFood, newResourceSummary.text,
            "Management 资源汇总必须反映真实初始 Food");
        Call(management, "RefreshNow");
        yield return null;
        Assert.AreEqual(newIngredients, Ingredients(), "NewGame 初始化后刷新 Management 不得再增减资源");
        Assert.AreEqual(newFood, Food(), "NewGame 初始化后刷新 Management 不得再增减资源");
        Assert.AreEqual("Management", Surface(), "NewGame 验证期间不得非预期离开管理页");
        Evidence("CASE-C", "NEWGAME roster=" + newPersonIds.Count + " configuredCharacters=" + configuredRoster
            + " sourceCharacters=" + newCharacterIds.Count + " sourceHeroes=" + newHeroIds.Count
            + " sourceIds=" + string.Join(",", newPersonIds)
            + " managementIds=" + string.Join(",", managementPersonIds)
            + " oldLeak=false resources=" + newIngredients + "/" + newFood
            + " authorityInitial=" + expectedNewIngredients + "/" + expectedNewFood
            + " batches=" + ProductionBatchCount() + " evidence=" + ProductionEvidenceCount()
            + " completed=" + AssignmentContributionHistory()
            + " clock=" + ClockAbsoluteMinute()
            + " newIngredientsWereAuthority=" + (newIngredients == expectedNewIngredients)
            + " newFoodWasAuthority=" + (newFood == expectedNewFood));
    }

    // ==================== 用例 9：远征 / Return gating（真实产品出征入口 + 既有公共 Return 相位） ====================

    /// <summary>
    /// 最小代表用例：真实产品出征入口 → 离村人物 → Return 推进（Active 相位 pending）
    /// → 合法结算回村 → 保存/恢复。
    ///
    /// 调用层级（不冒充完整产品链）：
    ///   - 出征：TownUI 的既有产品入口 → ExpeditionUI.ShowExpeditionFromTown（真实准备宿主）
    ///     → 真实 btnConfirm.onClick（OnConfirmExpedition 内部创建 AdventureManager、登记 ExpeditionPersonIds、
    ///       设 SimulationActive=false、显示 Journey）；不是 SetExpeditionPersonIds 冒充。
    ///   - Return：产品确认后 AdventureManager 已就绪；用既有公共 VillageRuntimeBridge.BeginExpeditionReturn(
    ///       1 day, Retreat) 建立一次“返程进行中（Active）”会话以便在相位中间做 gating 断言，
    ///     随后仍由真实产品入口 ExpeditionUI.OnReturnToTown → FinalizeReturn 按相位续跑
    ///       Settlement → Catch-up（真实 Time.TickDay 推进 1 天）→ Carry/Consequence/Clear → Final Return Save。
    ///     elapsed=1 day 是既有 Test/Not Frozen 输入，不是直接 Advance 冒充 Return。
    ///   - 管理 UI 在远征/Return 期间因 SimulationActive=false 被产品隐藏（不可达），
    ///     因此同时断言真实禁用的 UI 绑定状态 + Gateway 防御；不强行点击不可达按钮。
    /// </summary>
    [UnityTest]
    public IEnumerator ExpeditionReturn_KeepsManagementAndSaveGated_UntilLegalProductSettlement()
    {
        Assert.AreEqual(0L, ClockAbsoluteMinute(), "夹具必须从新世界 0 分钟开始");
        Assert.AreEqual("Paused", ClockSpeed(), "夹具必须从玩家 Pause 开始以保证确定性");
        Assert.IsTrue(SimulationActive(), "产品入口进入世界后模拟必须 Active");

        // ---------------- 0. 真实世界基座：Completed Farm A + UnderConstruction Farm B ----------------
        string farmA = PlaceBuilding("building_farm", 4f);
        string firstBuilder = SnapshotPersonIdAt(0);
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farmA, firstBuilder).ToString(),
            "前置：必须能用真实人物加入 Farm A 施工");
        AdvanceTo(14400);
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(farmA), "Farm A 必须在真实推进后完工");
        string farmB = PlaceBuilding("building_farm", 8f);
        Assert.AreEqual("UnderConstruction", BuildingState(farmB), "Farm B 必须保持真实施工中");
        long baseClock = ClockAbsoluteMinute();
        Assert.AreEqual(14400L, baseClock, "基座必须停在真实 14400 分钟");
        Assert.AreEqual(0, ProductionBatchCount(), "基座不得预置生产批次");
        Assert.AreEqual(0, AssignmentContributionHistory(), "基座不得预置执行历史");

        string workplaceA = WorkplaceIdFromProjection(farmA);
        Assert.IsTrue(workplaceA.StartsWith("workplace:" + farmA + ":", StringComparison.Ordinal),
            "Farm A 岗位 ID 必须绑定建筑实例：" + workplaceA);

        // ---------------- 1. 真实产品入口：Home 底部导航“远征” → 准备 → 读取真实出征名单 ----------------
        // 真实补给前置：准备界面在 ShowExpeditionUI / OnCharacterCardClick 内用当时的 Village Food
        // 求值 preflight；因此必须先经既有公开资源写入口配置补给，再走真实导航进入准备。
        // 不在打开后直接改 btnConfirm.interactable（那会绕过真实 UI 更新链）。
        Call(ResourcesState(), "AddFood", 100);
        int foodBeforePrep = Food();
        Assert.GreaterOrEqual(foodBeforePrep, 100, "补给前置必须真实写入 Village Food Authority");

        object expeditionUI = Find("ExpeditionUI");
        Assert.IsNotNull(expeditionUI, "ExpeditionUI 必须存在（既有产品远征入口）");
        ClickChild(_shell, "ProductNavigation/Navigate5", "底部导航·远征");
        yield return null;
        yield return null;
        Assert.IsTrue(((Component)expeditionUI).gameObject.activeSelf, "真实“远征”导航必须打开准备宿主");
        Assert.IsFalse(SimulationActive(), "进入远征准备必须由既有产品入口暂停 Village 模拟");
        Assert.IsFalse(((GameObject)Get(_town, "canvasRoot")).activeSelf,
            "进入远征准备必须由既有产品入口隐藏 Town Canvas");

        object expedition = Get(expeditionUI, "_expedition");
        Assert.IsNotNull(expedition, "真实远征准备必须创建 ExpeditionSystem");
        var selectedCards = (IList)Get(expedition, "SelectedCharacters");
        Assert.IsNotNull(selectedCards, "ExpeditionSystem.SelectedCharacters 必须存在");
        Assert.Greater(selectedCards.Count, 0, "真实远征准备必须自动选中至少一名角色");
        var expectedIds = new List<string>();
        foreach (object card in selectedCards)
        {
            string id = (string)Call(_player, "ResolvePersonIdForCard", card);
            if (!string.IsNullOrEmpty(id) && !expectedIds.Contains(id)) expectedIds.Add(id);
        }
        object selectedHero = Get(expedition, "SelectedHero");
        if (selectedHero != null)
        {
            string heroId = (string)Get(selectedHero, "PersonId");
            if (!string.IsNullOrEmpty(heroId) && !expectedIds.Contains(heroId)) expectedIds.Add(heroId);
        }
        Assert.Greater(expectedIds.Count, 0, "出征名单必须含真实 PersonId");
        string partyPerson = expectedIds[0];
        string constructionPerson = HostPersonIds().FirstOrDefault(id => !expectedIds.Contains(id));
        Assert.IsNotNull(constructionPerson, "夹具需要至少一名不随队离村的真实人物");

        // 真实 Authority 前置：出征人物原有动态职责（Farm A），Return 后必须保持。
        Assert.AreEqual("Success", Bridge("TryAssignPerson", partyPerson, workplaceA).ToString(),
            "出征人物必须能先建立真实动态岗位职责");
        Assert.AreEqual(workplaceA, PersonWorkplace(partyPerson), "前置动态职责必须真实写入");
        Assert.AreEqual(1, WorkplaceOccupants(workplaceA), "前置动态岗位占用必须为 1");

        // 真实施工事实：留村人物参与 Farm B 施工（原有公开施工入口）。
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farmB, constructionPerson).ToString(),
            "留村人物必须能加入 Farm B 施工");
        object projectBefore = ActiveProject();
        Assert.IsNotNull(projectBefore, "加入施工后必须存在真实活动施工项目");
        CollectionAssert.Contains(Strings(Get(projectBefore, "ParticipantPersonIds")), constructionPerson,
            "Farm B 施工参与者必须含留村人物");

        // 真实产品确认按钮（OnConfirmExpedition：登记远征、隐藏 Shell、暂停 Village 模拟）。
        var confirmButton = (Button)Get(expeditionUI, "btnConfirm");
        Assert.IsNotNull(confirmButton, "真实出征确认按钮必须绑定");

        // 逐项核对确认按钮的真实前置（不预设只缺补给）：
        //   队伍选择 = ExpeditionSystem.IsReady；补给 = Village Food + Carry policy + elapsed policy；
        //   世界写锁 = Active Advance / 未完成 Return；成长 pending = GrowthUI 未决选牌。
        // 以上均由真实产品 Authority / 现有 UI 读取，不写按钮、不写私有字段。
        int partySize = selectedCards.Count + (selectedHero != null ? 1 : 0);
        int villageFood = Food();
        int carryPerPerson = Convert.ToInt32(StaticField("VillageRuntimeBridge", "ResourceCarryPerPerson"));
        int dailyRate = Convert.ToInt32(StaticField("VillageRuntimeBridge", "ResourcePerPersonDailyConsumptionRate"));
        int battleDays = Convert.ToInt32(StaticField("LegacyNodeElapsedDaysTestPolicy", "BattleNodeElapsedDays"));
        int requestedCarry = partySize * carryPerPerson;
        int actualCarry = Math.Min(villageFood, requestedCarry);
        int firstStepCost = partySize * battleDays * dailyRate;
        bool expeditionReady = (bool)Get(expedition, "IsReady");
        bool canSaveWorld = (bool)StaticField("VillageRuntimeBridge", "CanSaveWorld");
        bool worldWriteLocked = (bool)StaticField("VillageRuntimeBridge", "IsWorldWriteLocked");
        object activeReturn = StaticField("VillageRuntimeBridge", "ActiveReturnRecord");
        object growthUI = FindOptional("CharacterGrowthUI");
        bool pendingGrowth = growthUI != null && (bool)Get(growthUI, "HasPendingChoice");

        Assert.IsTrue(expeditionReady, "真实队伍选择必须至少含 1 张角色卡（ExpeditionSystem.IsReady）");
        Assert.IsTrue(actualCarry >= firstStepCost,
            "真实补给来源必须足够第一步成本：food=" + villageFood
            + " actualCarry=" + actualCarry + " firstStepCost=" + firstStepCost);
        Assert.IsTrue(canSaveWorld, "确认出征前世界必须不在 WorldBusy / 未完成 Return 锁中");
        Assert.IsFalse(worldWriteLocked, "确认出征前不得存在 Active Advance / 未完成 Return 写锁");
        Assert.IsNull(activeReturn, "确认出征前不得存在未完成的 Return 会话");
        Assert.IsFalse(pendingGrowth, "确认出征前不得有未处理的成长选牌 pending（不得被其它门禁掩盖）");

        Evidence("EXP-RETURN", "confirm-preflight party=" + partySize
            + " cards=" + selectedCards.Count + " hero=" + (selectedHero != null)
            + " isReady=" + expeditionReady + " foodBeforePrep=" + foodBeforePrep + " food=" + villageFood
            + " carryPolicy=" + carryPerPerson + " dailyRate=" + dailyRate + " battleDays=" + battleDays
            + " requestedCarry=" + requestedCarry + " actualCarry=" + actualCarry
            + " firstStepCost=" + firstStepCost + " affordable=" + (actualCarry >= firstStepCost)
            + " worldWriteLocked=" + worldWriteLocked + " canSaveWorld=" + canSaveWorld
            + " returnRecord=" + (activeReturn == null ? "none" : Get(activeReturn, "Phase").ToString())
            + " pendingGrowth=" + pendingGrowth
            + " source=VillageRuntimeBridge.Resources.Food+CarryPolicy+ElapsedPolicy"
            + " button=" + confirmButton.interactable);
        Assert.IsTrue(confirmButton.interactable, "补给足够且队伍/世界前置合法时真实确认按钮必须可用");
        confirmButton.onClick.Invoke();
        yield return null;
        yield return null;
        Assert.IsFalse(((Component)expeditionUI).gameObject.activeSelf, "确认出征后 ExpeditionUI 必须隐藏");

        var registered = Strings(StaticField("VillageRuntimeBridge", "ExpeditionPersonIds"));
        Assert.AreEqual(expectedIds.Count, registered.Count, "Expedition Occupancy 必须恰为真实出征名单");
        foreach (string id in expectedIds) CollectionAssert.Contains(registered, id, "登记必须含真实出征者 " + id);
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "远征离村后 CanSaveWorld 必须为 false");
        Assert.IsFalse(SimulationActive(), "确认出征必须由既有产品入口暂停 Village 模拟（不是内部页规则）");
        Assert.IsFalse((bool)Get(_shell, "ShellVisible"), "远征期间 Home Shell 必须不可见（不得借内部页恢复 Away Simulation）");
        Assert.IsFalse(((GameObject)Get(_town, "canvasRoot")).activeSelf, "远征期间 Town Canvas 必须由产品隐藏");
        Assert.AreEqual("World", Surface(), "远征期间共享路由必须仍停在 World，不得由管理页占据");
        object awayRow = ProjectionPerson(partyPerson);
        Assert.IsNotNull(awayRow, "管理只读投影必须包含出征人物");
        Assert.AreEqual("Expedition", Get(awayRow, "Availability").ToString(),
            "管理投影必须把出征人物标为 Expedition（离村事实）");
        Evidence("EXP-RETURN", "departure clock=" + ClockAbsoluteMinute()
            + " party=" + string.Join(",", expectedIds) + " duty=" + PersonWorkplace(partyPerson)
            + " construction=" + constructionPerson + " project=" + ProjectDiagnostic()
            + " shellVisible=false simulationActive=false canSave=false");

        // ---------------- 2. Return 推进中（Active pending）：管理命令 / 保存必须被 Gate ----------------
        object retreatMode = Enum.Parse(LoadedType("RunEndMode"), "Retreat");
        Assert.IsTrue((bool)StaticByArgs("VillageRuntimeBridge", "BeginExpeditionReturn", 1, retreatMode),
            "既有公共 Return 入口必须能建立一次返程会话（1 day）");
        object returnRecord = StaticField("VillageRuntimeBridge", "ActiveReturnRecord");
        Assert.IsNotNull(returnRecord, "Return 进行中必须存在会话记录");
        Assert.AreEqual("Active", Get(returnRecord, "Phase").ToString(), "Return pending 相位必须为 Active");
        Assert.IsFalse((bool)Get(returnRecord, "Completed"), "Return pending 不得提前 Completed");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "Return pending 期间 CanSaveWorld 必须为 false");

        long gatedClock = ClockAbsoluteMinute();
        string gatedSavePath = CurrentSavePath();
        string gatedSaveHash = File.Exists(gatedSavePath) ? HashFile(gatedSavePath) : "<absent>";
        string gatedDuty = PersonWorkplace(partyPerson);
        int gatedOccupancy = WorkplaceOccupants(workplaceA);
        string gatedProject = ProjectDiagnostic();
        string gatedConstructionDuty = PersonWorkplace(constructionPerson);
        int gatedExpeditionCount = Strings(StaticField("VillageRuntimeBridge", "ExpeditionPersonIds")).Count;

        // person 级：出征者不可被管理命令调度；world 级：留村人物的命令也必须 WorldBusy。
        Assert.AreEqual("PersonNotInVillage",
            CommandOutcome(Static("ManagementCommandGateway", "AssignToWorkplace", partyPerson, workplaceA)),
            "Return pending 期间对远征人物派工必须 PersonNotInVillage");
        Assert.AreEqual("PersonNotInVillage",
            CommandOutcome(Static("ManagementCommandGateway", "RemoveAssignment", partyPerson)),
            "Return pending 期间对远征人物撤职必须 PersonNotInVillage");
        Assert.AreEqual("WorldBusy",
            CommandOutcome(Static("ManagementCommandGateway", "AssignToWorkplace", constructionPerson, workplaceA)),
            "Return pending 期间留村人物派工必须 WorldBusy");
        Assert.AreEqual("WorldBusy",
            CommandOutcome(Static("ManagementCommandGateway", "AddConstructionWorker", farmB, constructionPerson)),
            "Return pending 期间加入施工必须 WorldBusy");
        Assert.AreEqual("WorldBusy",
            CommandOutcome(Static("ManagementCommandGateway", "RemoveConstructionWorker", farmB, constructionPerson)),
            "Return pending 期间撤出施工必须 WorldBusy");
        Assert.AreEqual("WorldBusy",
            CommandOutcome(Static("ManagementCommandGateway", "SaveWorld")),
            "Return pending 期间显式保存必须 WorldBusy");

        Assert.AreEqual(gatedDuty, PersonWorkplace(partyPerson), "被拒命令不得改写远征人物原职责");
        Assert.AreEqual(gatedConstructionDuty, PersonWorkplace(constructionPerson), "被拒命令不得改写留村人物职责");
        Assert.AreEqual(gatedOccupancy, WorkplaceOccupants(workplaceA), "被拒命令不得改写岗位占用");
        Assert.AreEqual(gatedProject, ProjectDiagnostic(), "被拒命令不得改写真实施工事实");
        Assert.AreEqual(gatedExpeditionCount, Strings(StaticField("VillageRuntimeBridge", "ExpeditionPersonIds")).Count,
            "被拒命令不得清除/改写远征占用");
        Assert.AreEqual(gatedClock, ClockAbsoluteMinute(), "被拒命令不得推进 Clock");
        Assert.AreEqual(gatedSaveHash, File.Exists(gatedSavePath) ? HashFile(gatedSavePath) : "<absent>",
            "Return pending 期间保存必须零写盘");

        // 真实管理 UI 绑定：Shell 隐藏不可达，但真实按钮必须处于禁用（不强行点击不可达按钮）。
        object management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        Call(Get(_host, "Selection"), "SelectPerson", partyPerson);
        Call(management, "RefreshNow");
        yield return null;
        Assert.AreEqual(partyPerson, (string)Get(management, "SelectedPersonId"),
            "管理页必须保持同一真实人物选择");
        Assert.IsFalse(((Button)Get(management, "saveButton")).interactable,
            "Return pending 期间真实保存按钮必须禁用");
        Assert.IsFalse(((Button)Get(management, "removeButton")).interactable,
            "Return pending 期间对远征人物真实撤职按钮必须禁用");
        Assert.IsFalse((bool)Call(management, "RemoveSelectedPersonAssignment"),
            "真实撤职 UI 路径必须返回失败");
        AssertStatusContains(management, "远征");
        Assert.IsFalse((bool)Call(management, "Save"), "真实保存 UI 路径必须返回失败");
        AssertStatusContains(management, "未保存");
        AssertStatusContains(management, "世界正在推进");
        Assert.AreEqual(gatedSaveHash, File.Exists(gatedSavePath) ? HashFile(gatedSavePath) : "<absent>",
            "真实保存 UI 失败不得写盘");

        // 表面开关 / ESC：真实 UI 不可达；直接产品回调也不得解锁或替换 Return。
        Call(_shell, "OpenSurface", Enum.Parse(T("HomeSurface"), "Management"));
        Assert.AreEqual("Management", Surface(), "产品路由回调可请求管理表面（用于防御断言）");
        Assert.IsFalse((bool)Get(management, "IsVisible"),
            "Shell 隐藏时管理表面不得实体显示（真实 UI 不可达）");
        Call(_shell, "HandleEscape");
        Assert.AreEqual("World", Surface(), "ESC 必须回到 World，不得绕过 Return gating");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "ESC/表面开关不得提前解锁世界");

        // 手动 Pause：只改 Clock 速度，不得启动第二 Clock、不得推进/解锁 Return。
        Call(_shell, "TogglePause");
        Assert.AreEqual("Normal", ClockSpeed(), "既有手动 Pause 回调必须把 Clock 恢复为 Normal");
        yield return null;
        yield return null;
        yield return null;
        Assert.IsFalse(SimulationActive(), "Return pending 时手动恢复 Clock 不得把 Village 模拟置为 Active");
        Assert.AreEqual(gatedClock, ClockAbsoluteMinute(),
            "Return pending 时手动恢复 Clock 不得启动第二 Clock 推进世界");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "手动 Pause 不得提前解锁保存/命令");
        Assert.AreEqual("Active", Get(StaticField("VillageRuntimeBridge", "ActiveReturnRecord"), "Phase").ToString(),
            "手动 Pause 不得改写 Return 相位");
        Call(_shell, "TogglePause");
        Assert.AreEqual("Paused", ClockSpeed(), "采样后必须回到玩家 Pause");
        Evidence("EXP-RETURN", "pending clock=" + gatedClock + " phase=Active duty=" + gatedDuty
            + " occupancy=" + gatedOccupancy + " project=" + gatedProject
            + " commands=PersonNotInVillage/WorldBusy save=WorldBusy zero-write=true"
            + " shellVisible=false surfaceOpenBlocked=true manualPauseNoUnlock=true");

        // ---------------- 3. 合法产品结算回村：OnReturnToTown → Settlement/Catch-up/Carry/Clear/Final Save ----------------
        Call(expeditionUI, "OnReturnToTown");
        yield return null;
        yield return null;
        for (int i = 0; i < 10 && !(bool)Get(_shell, "ShellVisible"); i++) yield return null;
        Call(_host, "RefreshNow");

        object finalRecord = StaticField("VillageRuntimeBridge", "ActiveReturnRecord");
        Assert.IsNotNull(finalRecord, "合法结算后必须保留 Return 会话终态记录");
        Assert.IsTrue((bool)Get(finalRecord, "Completed"), "最终 Return Save 成功后必须 Completed");
        Assert.AreEqual("Completed", Get(finalRecord, "Phase").ToString(), "Return 终态必须为 Completed");
        Assert.AreEqual(1, Convert.ToInt32(Get(finalRecord, "DaysSimulated")),
            "Return Catch-up 必须逐日推进 1 天（既有 Time 推进器）");
        Assert.AreEqual(0, Strings(StaticField("VillageRuntimeBridge", "ExpeditionPersonIds")).Count,
            "合法结算后 Expedition Occupancy 必须清除");
        Assert.IsTrue((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "合法结算回村后世界必须重新可保存");
        Assert.IsTrue(SimulationActive(), "回村后既有产品入口必须恢复 Village 模拟");
        Assert.AreEqual("Paused", ClockSpeed(), "Return 不得改写玩家手动 Pause");
        Assert.AreEqual(baseClock + 1440L, ClockAbsoluteMinute(),
            "Return Catch-up 必须只推进 1 天（不得重复推进/第二 Clock）");
        Assert.IsTrue((bool)Get(_shell, "ShellVisible"), "回村后 Home Shell 必须恢复可见");
        Assert.AreEqual("World", Surface(), "回村后必须回到 World");
        Assert.AreEqual(workplaceA, PersonWorkplace(partyPerson), "Return 后原动态职责必须保持");
        Assert.AreEqual(1, WorkplaceOccupants(workplaceA), "Return 后岗位占用必须与职责一致");
        Assert.AreEqual("InVillage", PersonAvailability(partyPerson), "Return 后远征人物必须恢复在村可用");
        Assert.AreEqual(0, ProductionBatchCount(), "仅离村人物持有职责时 Return Catch-up 不得重复生产批次");
        Assert.AreEqual(0, AssignmentContributionHistory(), "远征占用期间不得重复结算执行历史");
        object projectAfter = ActiveProject();
        Assert.IsNotNull(projectAfter, "Farm B 施工项目必须仍活动（Test 目标 10，1 天不可完工）");
        CollectionAssert.Contains(Strings(Get(projectAfter, "ParticipantPersonIds")), constructionPerson,
            "Return 后原施工参与者必须保持");
        Assert.AreEqual("UnderConstruction", BuildingState(farmB), "Farm B 必须保持施工中（未重复结算/完成）");
        Assert.IsTrue((bool)Static("SaveSystem", "HasSave"), "最终 Return Save 必须真实落盘");

        // ---------------- 4. 保存可恢复：真实 SaveSystem.Load 后事实逐项一致、无重复生产/历史 ----------------
        long returnClock = ClockAbsoluteMinute();
        int returnIngredients = Ingredients(), returnFood = Food();
        int returnBatches = ProductionBatchCount(), returnEvidence = ProductionEvidenceCount();
        int returnHistory = AssignmentContributionHistory();
        string returnDuty = PersonWorkplace(partyPerson);
        int returnOccupancy = WorkplaceOccupants(workplaceA);
        string returnProject = ProjectDiagnostic();
        string returnFarmB = BuildingState(farmB);
        string returnSaveHash = HashFile(CurrentSavePath());
        Evidence("EXP-RETURN", "returned clock=" + returnClock + " days=1"
            + " ingredients=" + returnIngredients + " food=" + returnFood
            + " batches=" + returnBatches + " evidence=" + returnEvidence + " history=" + returnHistory
            + " duty=" + returnDuty + " occupancy=" + returnOccupancy + " project=" + returnProject
            + " farmB=" + returnFarmB + " save=" + returnSaveHash);

        object loaded = Static("SaveSystem", "Load");
        Assert.IsNotNull(loaded, "最终 Return Save 必须可被真实 Load 恢复");
        Bridge("SetSimulationPersonSource", loaded);
        Call(_host, "RefreshNow");
        yield return null;
        Assert.AreEqual(returnClock, ClockAbsoluteMinute(), "Load 后 Clock 必须与最终 Return Save 一致");
        Assert.AreEqual("Paused", ClockSpeed(), "Load 后必须保持存档中的 Paused");
        Assert.AreEqual(returnIngredients, Ingredients(), "Load 后 Ingredients 必须一致（不得重复结算）");
        Assert.AreEqual(returnFood, Food(), "Load 后 Food 必须一致（不得重复结算）");
        Assert.AreEqual(returnBatches, ProductionBatchCount(), "Load 不得重复生产批次");
        Assert.AreEqual(returnEvidence, ProductionEvidenceCount(), "Load 不得重复生产/工作证据");
        Assert.AreEqual(returnHistory, AssignmentContributionHistory(), "Load 不得重复结算执行历史");
        Assert.AreEqual(returnDuty, PersonWorkplace(partyPerson), "Load 后原动态职责必须一致");
        Assert.AreEqual(returnOccupancy, WorkplaceOccupants(workplaceA), "Load 后岗位占用必须一致");
        Assert.AreEqual(returnProject, ProjectDiagnostic(), "Load 后真实施工事实必须一致");
        Assert.AreEqual(returnFarmB, BuildingState(farmB), "Load 后建筑生命周期必须一致");
        Assert.IsTrue((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "Load 后世界必须处于可保存边界");

        // ---------------- 5. 合法回村后才解锁：真实管理导航 + 按钮可交互 + 显式保存 ----------------
        // SaveSystem.Load 只把世界恢复进既有 Authority，并返回一个新的 PlayerCollection；本用例随后
        // 经产品公共入口 SetSimulationPersonSource 把长期人物真相源重新绑定到该实例（与 Continue 同义）。
        // Shell 对“世界来源切换”的复位（Feed.ResetSession + Router.Close）由它自己的刷新入口消费；
        // 首次真实导航的复位次序已由 HomeProductShell.OpenSurface“先消费复位再提交导航”保证
        // （首次导航回归见用例 10）。本用例仍保留一次显式产品刷新，覆盖“复位先于任何导航被周期刷新消费”的路径。
        object shellSourceBeforeSync = Get(_shell, "_source");
        object shellCanvas = Get(_shell, "_canvas");
        object growthForEntry = FindOptional("CharacterGrowthUI");
        bool growthOpenAtEntry = growthForEntry != null && (bool)Get(growthForEntry, "IsOpen");
        bool growthPendingAtEntry = growthForEntry != null && (bool)Get(growthForEntry, "HasPendingChoice");
        Assert.AreEqual("World", Surface(), "Load 后 Shell 必须仍停在 World（导航前）");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "Load 后管理页不得被提前打开");
        Evidence("EXP-RETURN", "reentry-pre-sync shell=" + ((Component)_shell).GetInstanceID()
            + " host=" + ((Component)_host).GetInstanceID()
            + " management=" + ((Component)management).gameObject.GetInstanceID()
            + " shellSourceObservedLoaded=" + ReferenceEquals(shellSourceBeforeSync, loaded)
            + " bridgeSourceIsLoaded=" + ReferenceEquals(StaticField("VillageRuntimeBridge", "SimulationPersonSource"), loaded)
            + " shellVisible=" + (bool)Get(_shell, "ShellVisible")
            + " canvasEnabled=" + ((Canvas)shellCanvas).enabled
            + " canvasActive=" + ((Canvas)shellCanvas).gameObject.activeInHierarchy
            + " surface=" + Surface()
            + " modal=" + (bool)Get(_shell, "SurfaceModalActive")
            + " growthOpen=" + growthOpenAtEntry + " growthPending=" + growthPendingAtEntry
            + " managementActive=" + (bool)Get(management, "IsVisible"));

        // 若已有合法表面模态（例如成长页），先经产品既有公开关闭路径关闭它，再真实导航；
        // 有未决选牌时不允许绕过（会与产品门禁冲突），直接让用例失败并保留证据。
        if (growthOpenAtEntry)
        {
            Assert.IsFalse(growthPendingAtEntry, "回村后成长页若打开，不得含未决选牌（否则无合法关闭路径）");
            Assert.IsTrue((bool)Call(growthForEntry, "TryHide"), "回村后既有成长模态必须可经真实关闭路径关闭");
            yield return null;
        }
        Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "真实导航前不得残留表面模态");

        Call(_shell, "RefreshNow"); // 产品周期刷新同一入口：只消费来源切换复位，不写 Router / 不激活管理页
        Assert.IsTrue(ReferenceEquals(Get(_shell, "_source"), loaded),
            "Shell 必须先经真实刷新路径观察 SaveSystem.Load 返回的世界来源");
        Assert.AreEqual("World", Surface(), "来源切换复位后 Shell 必须停在 World（导航前）");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "来源切换复位不得提前打开管理页");
        Evidence("EXP-RETURN", "reentry-world-synced shell=" + ((Component)_shell).GetInstanceID()
            + " host=" + ((Component)_host).GetInstanceID()
            + " sourceObservedLoaded=" + ReferenceEquals(Get(_shell, "_source"), loaded)
            + " shellVisible=" + (bool)Get(_shell, "ShellVisible")
            + " canvasEnabled=" + ((Canvas)shellCanvas).enabled
            + " surface=" + Surface()
            + " modal=" + (bool)Get(_shell, "SurfaceModalActive")
            + " managementActive=" + (bool)Get(management, "IsVisible"));

        OpenManagementThroughNavigation("管理·合法返程后解锁");
        yield return null;
        management = Management();
        Assert.AreEqual("Management", Surface(), "真实“管理”导航点击后必须进入 Management Surface");
        Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "真实管理页必须计入统一表面模态");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "合法回村后真实管理页必须可见");
        Evidence("EXP-RETURN", "reentry-nav-committed shell=" + ((Component)_shell).GetInstanceID()
            + " management=" + ((Component)management).gameObject.GetInstanceID()
            + " surface=" + Surface() + " modal=" + (bool)Get(_shell, "SurfaceModalActive")
            + " managementActive=" + (bool)Get(management, "IsVisible")
            + " shellVisible=" + (bool)Get(_shell, "ShellVisible")
            + " canvasEnabled=" + ((Canvas)shellCanvas).enabled
            + " clicked=ProductNavigation/Navigate2 realPointerClick");
        ShowWorkplacesPage(management);
        yield return null;
        SelectForManagement(management, partyPerson);
        yield return null;
        Assert.IsTrue(((Button)Get(management, "saveButton")).interactable,
            "合法回村后真实保存按钮必须重新可用");
        Assert.IsTrue(((Button)Get(management, "removeButton")).interactable,
            "合法回村后原职责人物的真实撤职按钮必须可用");
        ClickButtonField(management, "saveButton", "合法回村后显式保存");
        yield return null;
        AssertStatusContains(management, "已保存当前世界。");
        Assert.AreEqual(returnClock, ClockAbsoluteMinute(), "显式保存不得推进 Clock");
        Assert.AreEqual(returnBatches, ProductionBatchCount(), "显式保存不得重复生产批次");
        Assert.AreEqual(returnHistory, AssignmentContributionHistory(), "显式保存不得重复结算历史");
        Evidence("EXP-RETURN", "END legal return unlocked: clock=" + ClockAbsoluteMinute()
            + " duty=" + PersonWorkplace(partyPerson) + " occupancy=" + WorkplaceOccupants(workplaceA)
            + " project=" + ProjectDiagnostic() + " batches=" + ProductionBatchCount()
            + " history=" + AssignmentContributionHistory() + " loadVerified=true uiUnlocked=true");
    }

    // ==================== 用例 10：来源切换后的首次真实导航不被复位吞掉 ====================

    /// <summary>
    /// 回归（首次导航竞态）：Continue / Return Load 后长期人物来源被替换时，Shell 的既有来源复位
    /// （Feed.ResetSession + Router.Close + 收起关注/成长）必须在用户本次导航提交之前被消费。
    /// 否则 OpenSurface 内部随后的 RefreshNow 会把刚打开的 Surface 复位回 World，
    /// 第一次真实“管理”点击被产品内部复位吞掉（R3-B pilot 遗留；修复前因果见 09ec9d72）。
    ///
    /// 本用例不提前调用 RefreshNow、不等待周期刷新、不写 Router/SetActive/私有字段：
    ///   - 先用真实 Load 替换来源，再经产品周期刷新入口验证“来源复位仍有效”（Cards → World）；
    ///   - 然后再次真实 Load 替换来源，立即用真实 Raycast + Button 事件点击底部“管理”，
    ///     断言管理表面被提交、绑定新来源、旧候选/检索 session 已清；
    ///   - 人口行按“逐对象代际生命周期”核对，不以 childCount 作为异步 Destroy 是否完成的前置：
    ///     当前代 = activeSelf 行（数量与身份多重集必须唯一匹配新来源、Button 可交互），其
    ///     activeInHierarchy / 可交互状态必须与“当前页签是否显示人口页”一致（旧会话页签由产品保留，
    ///     来源替换不改页签，因此本用例不为了同帧人口计数再合成一次不现实的页签点击）；
    ///     ClearRows 先 SetActive(false) 再 Destroy 的待销毁代单列核对“全部 inactive、不可交互、
    ///     不被 Raycast 命中”，并在每个合法帧边界确认上一批待销毁代已物理销毁；
    ///     来源切换后 host 周期刷新清旧 Selection 会触发另一次合法重建（新代替换同一批业务行），
    ///     所以对“新出现的代”只做有界的一帧等待让 Canvas 处理几何，再对当前代做真实命中，
    ///     既不无条件删除断言，也不无限等帧；
    ///   - 页签交互（第二次用户点击）放到下一真实渲染帧：首次导航同一帧内管理页被
    ///     Router.Close→Open 连续关/激活，刚激活的 Graphic 还没有 canvasRenderer.absoluteDepth，
    ///     depth==-1 时 GraphicRaycaster 会跳过它；宿主实测同帧 Canvas.ForceUpdateCanvases() 也不能
    ///     让它可命中，所以本用例只在同帧留证 depth/cull/命中事实，不再重复同帧刷新方案，也不放宽断言；
    ///     页签仍由真实 Raycast + PointerClick 触发，行身份 / 命中在随后真实渲染帧上强断言；
    ///   - 最后仍用真实 host 刷新入口证明“来源切换清旧 Selection”规则未变，且不打断已提交表面。
    /// </summary>
    [UnityTest]
    public IEnumerator FirstNavigationAfterSourceReplacement_CommitsManagementBeforeResetConsumesIt()
    {
        Assert.AreEqual(0L, ClockAbsoluteMinute(), "夹具必须从新世界 0 分钟开始");
        Assert.AreEqual("Paused", ClockSpeed(), "夹具必须从玩家 Pause 开始以保证确定性");
        Assert.IsTrue(SimulationActive(), "产品入口进入世界后模拟必须 Active");
        Assert.IsTrue((bool)Get(_shell, "ShellVisible"), "基线 Shell 必须可见");
        Assert.AreEqual("World", Surface(), "基线必须停在 World");

        object baselineSource = StaticField("VillageRuntimeBridge", "SimulationPersonSource");
        Assert.IsTrue(ReferenceEquals(baselineSource, Get(_shell, "_source")),
            "基线 Shell 必须已消费当前世界来源（否则后续来源切换无法辨别）");

        // 旧世界的真实 UI 会话事实：真实派工建立实例岗位行 + 真实行候选 + 真实检索输入。
        var characterIds = PersonIdsOf(_player, "OwnedCharacters");
        string oldPerson = HostPersonIds().FirstOrDefault(id => characterIds.Contains(id) && string.IsNullOrEmpty(PersonWorkplace(id)));
        Assert.IsNotNull(oldPerson, "夹具需要 1 名无既有主要职责的真实人物，用于建立旧岗位候选");
        string oldWorkplace = FirstInstanceFarmWorkplace(out _);
        Assert.IsFalse(string.IsNullOrEmpty(oldWorkplace), "正式实例世界必须有可派工的 Farm 实例岗位");
        Call(Get(_host, "Selection"), "SelectPerson", oldPerson);
        Assert.AreEqual("Success", Bridge("TryAssignPerson", oldPerson, oldWorkplace).ToString(),
            "前置必须能用既有公开派工入口建立真实实例岗位记录");
        OpenManagementThroughNavigation("首次导航回归·建立旧候选");
        yield return null;
        object management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        var populationContent = (RectTransform)Get(management, "populationContent");
        var populationPage = (GameObject)Get(management, "populationPage");
        var workplacePage = (GameObject)Get(management, "workplacePage");
        var resourcePage = (GameObject)Get(management, "resourcePage");
        Assert.IsNotNull(populationContent, "ManagementView.populationContent 必须绑定");
        Assert.IsNotNull(populationPage, "ManagementView.populationPage 必须绑定");
        Assert.IsNotNull(workplacePage, "ManagementView.workplacePage 必须绑定");
        Assert.IsNotNull(resourcePage, "ManagementView.resourcePage 必须绑定");
        ShowWorkplacesPage(management);
        yield return null;
        ClickWorkplaceRow(management, oldWorkplace, "首次导航回归·旧岗位候选");
        yield return null;
        var search = (TMP_InputField)Get(management, "nameSearch");
        // 旧检索 session 取“真实人物姓名首字”作为非空关键字：过滤后人口列表仍保留真实的旧代人口行，
        // 这样首次导航那一帧才能核对“旧代全部 inactive / 不可交互”，而不是对一个空集合做无意义断言。
        string oldDisplayName = ((IEnumerable)Get(Get(management, "CurrentSnapshot"), "Persons"))
            .Cast<object>()
            .Select(person => ((string)Get(person, "DisplayName") ?? string.Empty).Trim())
            .FirstOrDefault(name => name.Length > 0);
        Assert.IsNotNull(oldDisplayName, "夹具需要至少 1 名有姓名的人物来生成非空旧检索");
        string staleSearch = oldDisplayName.Substring(0, 1);
        search.text = staleSearch;
        yield return null;
        Assert.Greater(populationContent.childCount, 0,
            "非空旧检索必须留下真实的旧代人口行（用于核对旧代停用 / 销毁）");
        Assert.AreEqual(oldWorkplace, (string)Get(management, "SelectedWorkplaceId"),
            "来源切换前必须真实存在待清理的旧岗位候选");
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "旧候选准备后必须回 World");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "旧候选准备后管理页必须隐藏");
        Assert.AreEqual(oldWorkplace, (string)Get(management, "SelectedWorkplaceId"),
            "关闭管理不得提前清除候选（本用例需要在来源切换后验证清理）");
        Assert.AreEqual(staleSearch, ((TMP_InputField)Get(management, "nameSearch")).text,
            "关闭管理不得提前清除检索 session（同上）");

        // 让来源复位有一个真实待关闭的表面：真实底部导航打开“卡牌”白模表面。
        var cardsNavigation = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate3");
        Assert.IsNotNull(cardsNavigation, "真实 Shell 底部导航必须存在“卡牌”按钮 Navigate3");
        var cardsButton = cardsNavigation.GetComponent<Button>();
        Assert.IsNotNull(cardsButton, "底部导航“卡牌”必须是 Button");
        Assert.IsTrue(cardsButton.interactable, "世界状态下底部导航“卡牌”必须可点");
        RealPointerClick((RectTransform)cardsNavigation, "底部导航·卡牌（来源复位有效性）");
        Assert.AreEqual("Cards", Surface(), "真实“卡牌”导航必须进入 Cards 表面");

        // 真实 Load 替换世界来源（与 Continue / Return Load 同义）：
        // 先用产品周期刷新同一入口证明来源复位仍然有效，而不是被本次修复削弱。
        Assert.IsTrue((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"), "Load 前世界必须处于可保存边界");
        object firstLoaded = Static("SaveSystem", "Load");
        Assert.IsNotNull(firstLoaded, "真实 SaveSystem.Load 必须成功");
        Bridge("SetSimulationPersonSource", firstLoaded);
        Assert.IsFalse(ReferenceEquals(firstLoaded, baselineSource), "Load 必须返回新的世界来源实例");
        Call(_shell, "RefreshNow"); // 产品周期刷新同一入口：消费来源复位
        Assert.AreEqual("World", Surface(), "来源复位必须仍把旧表面关闭回 World（复位未被削弱）");
        Assert.IsTrue(ReferenceEquals(Get(_shell, "_source"), firstLoaded),
            "周期刷新入口必须仍能观察并消费 Load 返回的新来源");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "来源复位不得自行打开管理页");
        Assert.IsTrue((bool)Get(_shell, "ShellVisible"), "来源复位不得破坏 Shell 可见门禁");
        Assert.IsTrue(SimulationActive(), "来源复位不得关闭模拟");
        Evidence("FIRST-NAV", "periodic-reset surface=Cards→World shellSourceConsumed=True"
            + " managementHidden=True shellVisible=True simulationActive=True");

        // 第二次真实 Load：来源再次被替换、Shell 尚未消费；随后立即真实点击“管理”，不刷新、不等待帧。
        object secondLoaded = Static("SaveSystem", "Load");
        Assert.IsNotNull(secondLoaded, "第二次真实 Load 必须成功");
        Assert.IsFalse(ReferenceEquals(secondLoaded, firstLoaded), "第二次 Load 必须返回新的世界来源实例");
        Bridge("SetSimulationPersonSource", secondLoaded);
        Assert.IsTrue(ReferenceEquals(StaticField("VillageRuntimeBridge", "SimulationPersonSource"), secondLoaded),
            "Bridge 必须已指向第二次 Load 的新来源");
        Assert.IsFalse(ReferenceEquals(Get(_shell, "_source"), secondLoaded),
            "前置：Shell 必须尚未消费本次来源替换（否则无法证明首次点击竞态）");
        Assert.AreEqual("World", Surface(), "首次真实导航前 Shell 必须仍停在 World");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "首次真实导航前管理页不得被提前打开");
        Assert.IsNull(PersonWorkplace(oldPerson), "Load 必须真实替换世界职责事实（旧派工不得残留）");
        long clockBeforeClick = ClockAbsoluteMinute();
        Evidence("FIRST-NAV", "pre-click shellSourceIsLoaded=" + ReferenceEquals(Get(_shell, "_source"), secondLoaded)
            + " bridgeSourceIsLoaded=True surface=" + Surface() + " clock=" + clockBeforeClick
            + " staleCandidate=" + Get(management, "SelectedWorkplaceId")
            + " staleSearch=" + ((TMP_InputField)Get(management, "nameSearch")).text);

        // 点击前的旧代人口行（当前旧检索过滤后的真实子物体）。保存引用用于核对本次来源重建时
        // “旧代全部 inactive 且不可交互”，以及下一渲染帧的物理销毁。
        var oldGenerationRows = populationContent.Cast<Transform>().Where(row => row != null).ToList();
        Assert.IsNotEmpty(oldGenerationRows, "首次导航前必须存在真实的旧代人口行");
        Assert.AreEqual(populationContent.childCount, oldGenerationRows.Count,
            "旧代人口行集合必须覆盖容器全部子物体（childCount=" + populationContent.childCount + "）");

        // 第一次真实导航点击（真实 Raycast + Button 事件）；中间没有 RefreshNow / 帧等待 / 私有字段写入。
        OpenManagementThroughNavigation("首次导航回归·来源替换后第一次真实点击");

        Assert.AreEqual("Management", Surface(), "来源替换后第一次真实导航必须被提交，不得被来源复位吞掉");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "第一次真实导航后管理页必须可见");
        Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "管理页必须计入统一表面模态");
        Assert.IsTrue((bool)Get(_shell, "ShellVisible"), "首次导航不得破坏 Shell 可见门禁");
        Assert.IsTrue(SimulationActive(), "首次导航不得关闭模拟");
        Assert.AreEqual(clockBeforeClick, ClockAbsoluteMinute(), "首次导航不得推进 Clock");
        Assert.IsTrue(ReferenceEquals(Get(_shell, "_source"), secondLoaded),
            "首次导航必须在提交前消费来源替换复位（_source 必须已是新来源）");
        Assert.IsTrue(ReferenceEquals(Get(management, "_source"), secondLoaded),
            "管理页必须在同一次点击内绑定新世界来源");

        // 旧 UI 候选 / 检索必须由既有来源切换规则清除，而不是带到新世界。
        Assert.IsNull(Get(management, "SelectedWorkplaceId"), "来源切换后旧岗位候选必须清除");
        Assert.IsNull(Get(management, "SelectedConstructionBuildingId"), "来源切换后旧施工候选必须清除");
        Assert.AreEqual(string.Empty, ((TMP_InputField)Get(management, "nameSearch")).text,
            "来源切换后旧检索 session 必须清除");

        // 绑定新来源：管理人口投影必须与真实 Load 出的人物源逐 PersonId 一致。
        var newSourceIds = LegalPersonIdsFromSource(secondLoaded);
        Assert.IsNotEmpty(newSourceIds, "新来源必须包含可比较的合法 PersonId");
        var managementIds = SnapshotPersonIds(management);
        CollectionAssert.AreEquivalent(newSourceIds, managementIds,
            "首次导航后管理人口投影必须逐 PersonId 绑定新世界来源");
        Assert.AreEqual(newSourceIds.Count, TotalPersonCount(management), "首次导航后管理人口行数必须来自新来源");

        // ================= 边界一：首次导航的“同帧”语义（只同步，不点击页签、不跨帧） =================
        // 首次真实点击在同一帧内走完 ProductHomeShell.OpenSurface → SyncSourceReset（Router.Close）
        // → Router.Open(Management) → RefreshNow；刚被激活的 Graphic 还没被 Canvas 批量处理，
        // 而 uGUI 的 GraphicRaycaster 会跳过 depth == -1（= canvasRenderer.absoluteDepth）的 Graphic。
        // 宿主 2026-10-24T14:02:32 那一轮（同帧 ForceUpdateCanvases 补丁上线时的真实采样）为：
        //   raycastBeforeRefresh=0 raycastAfterRefresh=0 tabDepthBeforeRefresh=-1 tabDepthAfterRefresh=-1
        //   tabCull=False populationPageSelf=False activeTab=Workplaces
        // 即同帧再调一次 Canvas.ForceUpdateCanvases() 并不能让页签变得可命中——该“同帧刷新补丁”已删除，
        // 不再重复同一方案。本帧只断言“首次导航同步”的业务事实，页签交互留给下一真实渲染帧。
        var populationTab = (RectTransform)FindPath(((Component)management).transform, "ModalPanel/Tab_Population");
        Assert.IsNotNull(populationTab, "首次导航后 Tab_Population 必须存在");
        var populationTabGraphic = populationTab.GetComponent<Graphic>();
        var populationTabButton = populationTab.GetComponent<Button>();
        string activeTabAtCommit = Get(management, "ActiveTab").ToString();
        bool populationTabActive = activeTabAtCommit == "Population";

        // 页容器必须按产品 ApplyTabs 语义与当前页签一致（旧会话页签被产品保留，来源替换不改页签）。
        Assert.AreEqual(populationTabActive, populationPage.activeSelf,
            "同帧：populationPage.activeSelf 必须与 ActiveTab 一致（activeTab=" + activeTabAtCommit + "）");
        Assert.AreEqual(activeTabAtCommit == "Workplaces", workplacePage.activeSelf,
            "同帧：workplacePage.activeSelf 必须与 ActiveTab 一致（activeTab=" + activeTabAtCommit + "）");
        Assert.AreEqual(activeTabAtCommit == "Resources", resourcePage.activeSelf,
            "同帧：resourcePage.activeSelf 必须与 ActiveTab 一致（activeTab=" + activeTabAtCommit + "）");

        // 来源替换后人口列表会重建。ManagementView.ClearRows 先 SetActive(false) 再 Destroy（销毁在本帧末），
        // 所以被替换的旧代行在本帧仍可引用但必须已 inactive、不可交互；当前代行按 activeSelf 统计身份，
        // 其真实可见性 / 可交互性必须与“当前页签是否显示人口页”一致。
        var expectedTitles = ExpectedPopulationRowTitles(management);
        var sameFrameRows = populationContent.Cast<Transform>().Where(row => row != null).ToList();
        var currentRows = sameFrameRows.Where(row => row.gameObject.activeSelf).ToList();
        var pendingRows = sameFrameRows.Where(row => !row.gameObject.activeSelf).ToList();

        Assert.AreEqual(newSourceIds.Count, currentRows.Count,
            "同帧：当前代人口行必须只来自新来源（childCount=" + populationContent.childCount
            + " inactive=" + pendingRows.Count + "）");
        CollectionAssert.AreEquivalent(expectedTitles, currentRows.Select(PopulationRowTitle).ToList(),
            "同帧：当前代人口行必须逐行对应当前新来源人物（无旧来源身份残留）");
        foreach (var row in currentRows)
        {
            var rowButton = row.GetComponent<Button>();
            Assert.IsNotNull(rowButton, "人口行必须是真实 Button：" + row.name);
            Assert.IsTrue(rowButton.interactable, "当前代人口行按钮必须可交互（interactable）：" + row.name);
            // Selectable.IsActive() == isActiveAndEnabled：页签不显示人口页时，人口行按下产品语义不可交互。
            Assert.AreEqual(populationTabActive, rowButton.IsActive(),
                "当前代人口行按钮的可交互状态必须与当前页签状态一致：" + row.name
                + " activeTab=" + activeTabAtCommit);
        }

        Assert.IsNotEmpty(oldGenerationRows, "首次导航前捕获的旧代人口行必须非空");
        Assert.IsTrue(pendingRows.All(row => oldGenerationRows.Contains(row)),
            "同帧：inactive 子物体只能是本次被替换的旧代人口行");
        Assert.AreEqual(oldGenerationRows.Count, pendingRows.Count,
            "同帧：inactive 子物体数量必须等于被替换的旧代人口行数量（pending=" + pendingRows.Count
            + " old=" + oldGenerationRows.Count + "）");
        foreach (var oldRow in oldGenerationRows)
        {
            Assert.IsTrue(oldRow != null, "同帧：被替换的旧代人口行必须仍可引用（Destroy 排到本帧末）");
            Assert.IsFalse(oldRow.gameObject.activeSelf, "同帧：旧代人口行必须先停用：" + oldRow.name);
            Assert.IsFalse(oldRow.gameObject.activeInHierarchy, "同帧：旧代人口行不得留在层级中：" + oldRow.name);
            var oldButton = oldRow.GetComponent<Button>();
            Assert.IsNotNull(oldButton, "旧代人口行必须是真实 Button：" + oldRow.name);
            Assert.IsFalse(oldButton.IsActive(), "同帧：旧代人口行按钮必须不可交互：" + oldRow.name);
        }

        Evidence("FIRST-NAV", "same-frame-committed surface=" + Surface()
            + " activeTab=" + activeTabAtCommit
            + " populationPageSelf=" + populationPage.activeSelf
            + " workplacePageSelf=" + workplacePage.activeSelf
            + " resourcePageSelf=" + resourcePage.activeSelf
            + " populationContentInHierarchy=" + populationContent.gameObject.activeInHierarchy
            + " rowsSelfActive=" + currentRows.Count
            + " rowsInHierarchy=" + currentRows.Count(row => row.gameObject.activeInHierarchy)
            + " expected=" + newSourceIds.Count
            + " oldPendingInactive=" + pendingRows.Count
            + " tabActiveSelf=" + populationTab.gameObject.activeSelf
            + " tabInHierarchy=" + populationTab.gameObject.activeInHierarchy
            + " tabInteractable=" + (populationTabButton != null && populationTabButton.interactable)
            + " tabRaycastTarget=" + (populationTabGraphic != null && populationTabGraphic.raycastTarget)
            + " tabDepth=" + (populationTabGraphic != null ? populationTabGraphic.depth : int.MinValue)
            + " tabCull=" + (populationTabGraphic != null && populationTabGraphic.canvasRenderer.cull)
            + " tabRaycastHitsSameFrame=" + RaycastAllAt(ScreenCenter(populationTab)).Count
            + "（同帧 Raycast 值只留证，不作断言、不据此放宽任何业务断言）");

        // ================= 边界二：下一真实渲染帧的用户交互（第二次点击 = 页签） =================
        // 首次真实导航之前没有任何 yield / RefreshNow；这里才让出第一帧真实渲染，让 Canvas 处理刚激活的
        // Graphic（这是第二次页签点击之前，而不是首次导航之前）。
        yield return null;

        // 逐对象代际核对（不再把 childCount 当作“异步 Destroy 已完成 / 只剩当前代”的前置条件）：
        //   1) 首次点击前捕获的旧代引用到本帧边界必须已物理销毁；
        //   2) 本帧若存在新一代重建留下的待销毁行，必须全部 inactive、不可交互、不参与 Raycast；
        //   3) 当前代只按 activeSelf 认定，数量与身份多重集必须唯一匹配真实新来源。
        foreach (var oldRow in oldGenerationRows)
            Assert.IsTrue(oldRow == null, "下一渲染帧：被替换的旧代人口行必须已物理销毁");
        var boundaryGeneration = ClassifyPopulationRows(populationContent);
        AssertPendingPopulationGeneration(boundaryGeneration.Pending, "下一渲染帧");
        AssertCurrentPopulationGeneration(boundaryGeneration.Active, expectedTitles,
            populationPage.activeInHierarchy, "下一渲染帧");
        var boundaryFirstRow = (RectTransform)boundaryGeneration.Active[0];

        Evidence("FIRST-NAV", "next-frame-before-tab-click surface=" + Surface()
            + " activeTab=" + Get(management, "ActiveTab")
            + " tabDepth=" + (populationTabGraphic != null ? populationTabGraphic.depth : int.MinValue)
            + " tabCull=" + (populationTabGraphic != null && populationTabGraphic.canvasRenderer.cull)
            + " tabRaycastHits=" + RaycastAllAt(ScreenCenter(populationTab)).Count
            + " childCount=" + populationContent.childCount
            + " activeGeneration=" + boundaryGeneration.Active.Count
            + " pendingGeneration=" + boundaryGeneration.Pending.Count
            + " oldGenerationRefsDestroyed=" + oldGenerationRows.All(row => row == null)
            + " pendingAllInactive=" + boundaryGeneration.Pending.All(row =>
                row != null && !row.gameObject.activeSelf && !row.gameObject.activeInHierarchy));

        // 第二次真实用户点击：真实 Raycast + PointerClick 回人口页签（本帧 Canvas 已处理页签 Graphic）。
        ClickChild(management, "ModalPanel/Tab_Population", "首次导航·下一渲染帧回到人口页签");
        Assert.AreEqual("Population", Get(management, "ActiveTab").ToString(), "回到人口页签后必须显示人口页");
        Assert.IsTrue(populationPage.activeSelf,
            "回到人口页签后 populationPage 必须 active，activeInHierarchy 才能代表业务可见性");
        Assert.IsTrue(populationPage.activeInHierarchy, "回到人口页签后 populationPage 必须 activeInHierarchy");

        // 页签点击不改变人员 key，不触发第三次重建；这里只强制布局几何，不改 Canvas 深度、不改产品状态。
        LayoutRebuilder.ForceRebuildLayoutImmediate(populationContent);

        // 页签点击本身不得替换当前代：当前代必须仍是同一批对象；若有待销毁代，仍须全部 inactive。
        var tabGeneration = ClassifyPopulationRows(populationContent);
        AssertPendingPopulationGeneration(tabGeneration.Pending, "人口页签点击后");
        AssertCurrentPopulationGeneration(tabGeneration.Active, expectedTitles, true, "人口页签点击后");
        Assert.IsTrue(tabGeneration.Active.Contains(boundaryFirstRow),
            "人口页签：首次提交时捕获的人口行必须仍在当前代中（不得被无关重建替换）");
        Assert.IsTrue(oldGenerationRows.All(row => row == null), "人口页签：旧代人口行必须保持已销毁");

        Evidence("FIRST-NAV", "population-tab-clicked surface=" + Surface()
            + " activeTab=" + Get(management, "ActiveTab")
            + " activeGeneration=" + tabGeneration.Active.Count + " expected=" + newSourceIds.Count
            + " pendingGeneration=" + tabGeneration.Pending.Count
            + " oldRowsDestroyed=" + oldGenerationRows.All(row => row == null)
            + " childCount=" + populationContent.childCount);

        // 行命中同样是渲染几何事实：页签点击刚把 PopulationPage 整体激活，其子 Graphic 也要等下一次
        // Canvas 处理后才可被 Raycast。来源切换后 host 周期刷新清旧 Selection 还会触发一次合法重建
        // （同一批业务行换实例），因此按“是否出现新代”做有界等待：每跨一帧都要求上一批待销毁行
        // 已物理销毁、当前待销毁行全部 inactive；最多 3 帧后必须稳定，否则视为持续重建并失败。
        var previousActive = tabGeneration.Active;
        var previousPending = tabGeneration.Pending;
        List<Transform> renderedActive = null;
        List<Transform> renderedPending = null;
        bool generationSettled = false;
        for (int attempt = 0; attempt < 3; attempt++)
        {
            yield return null;
            foreach (var pending in previousPending)
                Assert.IsTrue(pending == null,
                    "渲染帧：上一批待销毁人口行必须已物理销毁（attempt=" + attempt + "）");
            var rendered = ClassifyPopulationRows(populationContent);
            AssertPendingPopulationGeneration(rendered.Pending, "渲染帧");
            AssertCurrentPopulationGeneration(rendered.Active, expectedTitles, true, "渲染帧");
            bool freshlyRebuilt = rendered.Active.Any(row => !previousActive.Contains(row));
            Evidence("FIRST-NAV", "render-frame-generation attempt=" + attempt
                + " surface=" + Surface()
                + " activeTab=" + Get(management, "ActiveTab")
                + " childCount=" + populationContent.childCount
                + " activeGeneration=" + rendered.Active.Count
                + " pendingGeneration=" + rendered.Pending.Count
                + " freshlyRebuilt=" + freshlyRebuilt
                + " previousPendingDestroyed=True");
            previousActive = rendered.Active;
            previousPending = rendered.Pending;
            renderedActive = rendered.Active;
            renderedPending = rendered.Pending;
            if (!freshlyRebuilt) { generationSettled = true; break; }
        }
        Assert.IsTrue(generationSettled,
            "人口行代际必须在限定渲染帧内稳定（不得每帧持续重建）");

        var firstRowAtRender = (RectTransform)renderedActive[0];
        var firstRowPoint = ScreenCenter(firstRowAtRender);
        var firstRowHits = RaycastAllAt(firstRowPoint);
        var firstRowGraphic = firstRowAtRender.GetComponent<Graphic>();
        Evidence("FIRST-NAV", "population-tab-row-hit surface=" + Surface()
            + " point=" + firstRowPoint + " raycastHits=" + firstRowHits.Count
            + " firstRowDepth=" + (firstRowGraphic != null ? firstRowGraphic.depth : int.MinValue)
            + " firstRowCull=" + (firstRowGraphic != null && firstRowGraphic.canvasRenderer.cull)
            + " activeGeneration=" + renderedActive.Count
            + " pendingGeneration=" + (renderedPending != null ? renderedPending.Count : 0));
        AssertTopHitBelongs(firstRowAtRender, "首次导航·人口页签第一行", firstRowPoint, firstRowHits);
        string selectionAfterNavigation = SelectionId();
        Assert.IsTrue(string.IsNullOrEmpty(selectionAfterNavigation) || newSourceIds.Contains(selectionAfterNavigation),
            "首次导航后 Selection 不得引用新来源之外的人物：" + selectionAfterNavigation);

        // 既有“来源切换清旧 Selection”规则仍在 host 侧生效，且不打断已提交的管理表面。
        Call(_host, "RefreshNow");
        Assert.IsNull(SelectionId(), "host 必须仍按既有来源切换规则清除旧 Selection");
        Assert.AreEqual("Management", Surface(), "host 来源复位不得关闭已提交的管理表面");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "host 来源复位后管理页必须仍可见");

        Evidence("FIRST-NAV", "first-click-committed surface=" + Surface()
            + " managementVisible=True shellSourceConsumed=True managementSourceConsumed=True"
            + " staleCandidate=cleared staleSearch=cleared persons=" + managementIds.Count
            + " selectionClearedByHostRule=True clock=" + ClockAbsoluteMinute()
            + " modal=" + (bool)Get(_shell, "SurfaceModalActive")
            + " shellVisible=" + (bool)Get(_shell, "ShellVisible")
            + " simulationActive=" + SimulationActive());
    }

    // ==================== 用例 8：World / Person / Management 三表面持续模拟与事实等价 ====================

    // 同一真实初始世界（W0）与同一绝对目标分钟：14400 = 第 11 天 00:00（刚刚跨过 Day Tick），
    // 16360 = 第 12 天 11:20。区间跨过：午前工作区间、真实生产/工作证据、真实 Day Tick 累计执行历史、
    // 资源结算与 Activities 机会边界。该区间与既有 CASE-C 已证事实一致（非本用例新造数值）。
    private const long EquivalenceBaseMinute = 14400;
    private const long EquivalenceTargetMinute = 16360;

    private sealed class SurfaceRunFacts
    {
        public string Surface;
        public string Clock;
        public string Village;
        public string Work;
        public string Activity;
        public string Social;
        public string Host;
        public string Roles;
        public string Projection;
        public string Resources;
    }

    private readonly Dictionary<string, SurfaceRunFacts> _surfaceRunFacts =
        new Dictionary<string, SurfaceRunFacts>(StringComparer.Ordinal);

    private static object ReadManagementProjection()
    {
        // ManagementReadAdapter.Read 有 Read()/Read(BuildingRegistry) 两个重载，显式取无参只读入口。
        var method = T("ManagementReadAdapter").GetMethod("Read", Flags, null, Type.EmptyTypes, null);
        Assert.IsNotNull(method, "ManagementReadAdapter.Read() 无参只读投影入口必须存在");
        return method.Invoke(null, null);
    }

    /// <summary>
    /// 只改 Clock 的公开采样参数（既有测试同款 Test 用法），用于把“真实帧推进”稳定采样成可观察的整分钟；
    /// 不写 Work / Assignment / Project / Resource / Save 状态，也不改任何生产规则。
    /// </summary>
    private static void SetSecondsPerDay(float secondsPerDay)
    {
        var property = Clock().GetType().GetProperty("SecondsPerDay", Flags);
        Assert.IsNotNull(property, "VillageTime.SecondsPerDay 公开属性必须存在");
        property.SetValue(Clock(), secondsPerDay);
    }

    private static string Nullable(object value) => value == null ? "<null>" : value.ToString();

    private List<string> SnapshotBuildingIds()
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        var ids = new List<string>();
        var buildings = (IEnumerable)Get(snapshot, "Buildings");
        if (buildings != null)
            foreach (object building in buildings) ids.Add((string)Get(building, "InstanceId"));
        ids.Sort(StringComparer.Ordinal);
        return ids;
    }

    private static string TextList(object value)
        => value == null ? "<null>" : string.Join(",", Strings(value));

    private static int TotalEvidenceMinutes()
    {
        object data = IntradayData();
        int total = 0;
        foreach (object row in (IEnumerable)Get(data, "Evidence")) total += Convert.ToInt32(Get(row, "Minutes"));
        return total;
    }

    /// <summary>真实 World 快照事实（人物 / 地点 / 建筑生命周期）；只读当前 ProductHomeSnapshot。</summary>
    private string HostFacts()
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "World Snapshot 必须已建立");
        var persons = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
            .Select(p => string.Join("|", new[]
            {
                (string)Get(p, "PersonId"), Nullable(Get(p, "DisplayName")), Nullable(Get(p, "Profession")),
                Nullable(Get(p, "SimulationPresence")), Nullable(Get(p, "CurrentPlaceId")),
                Nullable(Get(p, "DestinationPlaceId")), Nullable(Get(p, "AssignmentId")),
                Nullable(Get(p, "AssignmentSummary")), Nullable(Get(p, "Availability")),
                Nullable(Get(p, "OccupancySummary")), Nullable(Get(p, "ActivitySummary")),
                Nullable(Get(p, "Away"))
            }))
            .OrderBy(line => line, StringComparer.Ordinal).ToList();
        var places = ((IEnumerable)Get(snapshot, "Places")).Cast<object>()
            .Select(p => string.Join("|", new[]
            {
                (string)Get(p, "PlaceId"), Nullable(Get(p, "Kind")), Nullable(Get(p, "Status")),
                Nullable(Get(p, "Capacity")), TextList(Get(p, "AssignedPersonIds")),
                TextList(Get(p, "OccupantPersonIds"))
            }))
            .OrderBy(line => line, StringComparer.Ordinal).ToList();
        var builder = new StringBuilder();
        builder.AppendLine("persons=[" + string.Join(";", persons) + "]");
        builder.AppendLine("places=[" + string.Join(";", places) + "]");
        foreach (string id in SnapshotBuildingIds())
            builder.AppendLine("building=" + id + "|state=" + BuildingState(id));
        return builder.ToString();
    }

    /// <summary>真实 Work 事实：IntradayWork 权威投影中的工作证据分钟与到场登记（只读 ToSaveData）。</summary>
    private static string WorkFacts()
    {
        object data = IntradayData();
        var evidence = ((IEnumerable)Get(data, "Evidence")).Cast<object>()
            .Select(e => (string)Get(e, "PersonId") + "|" + (string)Get(e, "WorkplaceId") + "|" + Get(e, "Minutes"))
            .OrderBy(line => line, StringComparer.Ordinal).ToList();
        var arrivals = ((IEnumerable)Get(data, "Arrivals")).Cast<object>()
            .Select(a => (string)Get(a, "PersonId") + "|" + Nullable(Get(a, "Destination")) + "|" + Get(a, "ReadyMinute"))
            .OrderBy(line => line, StringComparer.Ordinal).ToList();
        return "evidence=[" + string.Join(";", evidence) + "]\narrivals=[" + string.Join(";", arrivals) + "]";
    }

    /// <summary>真实职责 / 施工 / 现场事实（逐 PersonId、逐建筑实例）。</summary>
    private string RoleFacts()
    {
        var builder = new StringBuilder();
        foreach (string id in HostPersonIds().OrderBy(v => v, StringComparer.Ordinal))
            builder.AppendLine(id + "|primary=" + Nullable(PersonWorkplace(id))
                + "|execution=" + WorkExecution(id)
                + "|executing=" + Bridge("IsExecutingWork", id)
                + "|claim=" + Nullable(ActivityClaim(id))
                + "|place=" + Nullable(PersonPlace(id))
                + "|availability=" + PersonAvailability(id));
        foreach (string building in SnapshotBuildingIds())
            builder.AppendLine("building=" + building + "|state=" + BuildingState(building));
        builder.AppendLine(ProjectDiagnostic());
        return builder.ToString();
    }

    /// <summary>
    /// Management 只读投影（人口 / 职责与场所 / 施工 / 资源）的逐行事实。
    /// 三表面都直接调用同一公开只读 Adapter，因此比较的是同一 Authority 上的读取事实，
    /// 不依赖 UI 会话筛选/排序，也不把“打开页面”当成写操作。
    /// </summary>
    private static string ManagementProjectionFacts(object snapshot)
    {
        Assert.IsNotNull(snapshot, "ManagementReadAdapter.Read() 必须返回投影（不得为 null）");
        var builder = new StringBuilder();
        builder.AppendLine("ready=" + Nullable(Get(snapshot, "Ready")));
        foreach (object row in ((IEnumerable)Get(snapshot, "Persons")).Cast<object>()
                     .OrderBy(r => (string)Get(r, "PersonId"), StringComparer.Ordinal))
            builder.AppendLine("person|" + string.Join("|", new[]
            {
                (string)Get(row, "PersonId"), Nullable(Get(row, "DisplayName")), Nullable(Get(row, "Profession")),
                Nullable(Get(row, "Availability")), Nullable(Get(row, "PrimaryWorkplaceId")),
                Nullable(Get(row, "PrimaryResponsibility")), Nullable(Get(row, "AssignmentIsLegacy")),
                Nullable(Get(row, "CurrentExecution")), Nullable(Get(row, "TemporaryCommitment")),
                Nullable(Get(row, "HasNutritionFacts")), Nullable(Get(row, "Hungry")),
                Nullable(Get(row, "ConsecutiveUnderfedDays"))
            }));
        foreach (object row in ((IEnumerable)Get(snapshot, "Workplaces")).Cast<object>()
                     .OrderBy(r => (string)Get(r, "WorkplaceId"), StringComparer.Ordinal))
            builder.AppendLine("workplace|" + string.Join("|", new[]
            {
                (string)Get(row, "WorkplaceId"), Nullable(Get(row, "Kind")),
                Nullable(Get(row, "BuildingInstanceId")), Nullable(Get(row, "FunctionSlotId")),
                Nullable(Get(row, "FunctionProfileId")), Nullable(Get(row, "Capacity")),
                Nullable(Get(row, "IsRegistered")), TextList(Get(row, "BoundPersonIds")),
                TextList(Get(row, "LaborPersonIds")), TextList(Get(row, "OnsitePersonIds"))
            }));
        foreach (object row in ((IEnumerable)Get(snapshot, "Construction")).Cast<object>()
                     .OrderBy(r => (string)Get(r, "BuildingInstanceId"), StringComparer.Ordinal))
            builder.AppendLine("construction|" + string.Join("|", new[]
            {
                (string)Get(row, "BuildingInstanceId"), Nullable(Get(row, "ProjectId")),
                Nullable(Get(row, "Progress")), Nullable(Get(row, "Target")),
                Nullable(Get(row, "ExecutionPaused")), TextList(Get(row, "WorkerPersonIds")),
                TextList(Get(row, "OnsitePersonIds"))
            }));
        object resources = Get(snapshot, "Resources");
        builder.AppendLine("resources|" + string.Join("|", new[]
        {
            Nullable(Get(resources, "HasWorld")), Nullable(Get(resources, "Ingredients")),
            Nullable(Get(resources, "Food")), TextList(Get(resources, "InVillagePersonIds")),
            TextList(Get(resources, "HungryPersonIds"))
        }));
        foreach (object row in ((IEnumerable)Get(resources, "Workplaces")).Cast<object>()
                     .OrderBy(r => (string)Get(r, "WorkplaceId"), StringComparer.Ordinal))
            builder.AppendLine("resource-workplace|" + string.Join("|", new[]
            {
                (string)Get(row, "WorkplaceId"), Nullable(Get(row, "RoleLabel")),
                Nullable(Get(row, "BuildingInstanceId")), Nullable(Get(row, "Capacity")),
                Nullable(Get(row, "BoundCount")), Nullable(Get(row, "LaborCount"))
            }));
        builder.AppendLine("diagnostics=" + TextList(Get(snapshot, "Diagnostics")));
        return builder.ToString();
    }

    private SurfaceRunFacts CaptureSurfaceFacts(string surface)
    {
        var facts = new SurfaceRunFacts { Surface = surface };
        facts.Clock = ClockAbsoluteMinute() + "|" + ClockSpeed() + "|"
            + ClockMultiplier().ToString("0.############", System.Globalization.CultureInfo.InvariantCulture)
            + "|simulationActive=" + SimulationActive();
        // 真实存档 DTO（只读 BuildSaveData）：Time / IntradayWork / Assignments / Projects / Resources /
        // SocialFacts / World / Buildings / State；不写盘、不改 DTO 结构。
        facts.Village = JsonUtility.ToJson(Bridge("BuildSaveData"));
        facts.Work = WorkFacts();
        facts.Activity = JsonUtility.ToJson(Static("ActivityRuntimeBridge", "BuildSaveData"));
        facts.Social = JsonUtility.ToJson(Static("SocialRuntimeBridge", "BuildSaveData"));
        facts.Host = HostFacts();
        facts.Roles = RoleFacts();
        facts.Projection = ManagementProjectionFacts(ReadManagementProjection());
        facts.Resources = Ingredients() + "/" + Food();
        return facts;
    }

    /// <summary>
    /// 用真实产品 Save/Load 通道回到同一 W0 世界（同一 PersonId / 配置 / 库存 / 职责 / 施工 / Clock）。
    /// 不反射写私有状态、不新建第二 Authority、不手工拼 DTO 覆盖世界。
    /// </summary>
    private void RestoreBaseWorld(string expectedHash, string what)
    {
        Assert.IsTrue(File.Exists(CurrentSavePath()), what + "：W0 单槽快照必须存在");
        Assert.AreEqual(expectedHash, HashFile(CurrentSavePath()), what + "：W0 快照文件在恢复前已被改写");
        Assert.IsTrue((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            what + "：世界必须处于可 Save/Load 边界（无 AdvanceSession / Return 会话）");
        object loaded = Static("SaveSystem", "Load");
        Assert.IsNotNull(loaded, what + "：同一 W0 世界的真实 SaveSystem.Load 必须成功");
        Bridge("SetSimulationPersonSource", loaded);
        SetSecondsPerDay(1440f);
        Call(Get(_shell, "Build"), "Cancel");
        Call(_shell, "CloseSurface");
        Call(_host, "RefreshNow");
        Assert.AreEqual(EquivalenceBaseMinute, ClockAbsoluteMinute(), what + "：恢复后必须回到 W0 的同一绝对分钟");
        Assert.AreEqual("Paused", ClockSpeed(), what + "：W0 是玩家手动 Pause 状态，恢复后必须保持");
        Assert.IsTrue(SimulationActive(), what + "：真实 Load 回到世界后 IsSimulationActive 必须为 true");
        Assert.AreEqual("World", Surface(), what + "：恢复后必须回到 World 表面");
        Evidence("SURFACE-SIM", what + " restored W0 hash=" + expectedHash + " clock=" + ClockAbsoluteMinute()
            + " speed=" + ClockSpeed() + " persons=" + HostPersonIds().Count);
    }

    /// <summary>真实产品入口打开目标表面；World/Person/Management 三个表面逐个打开，不 SetActive 代替。</summary>
    private IEnumerator OpenSurfaceForFacts(string surface, string person, string what)
    {
        Call(Get(_shell, "Build"), "Cancel");
        Call(_shell, "CloseSurface");
        yield return null;
        Assert.AreEqual("World", Surface(), what + "：打开表面前必须先在 World");
        Call(Get(_host, "Selection"), "SelectPerson", person);
        Assert.AreEqual(person, SelectionId(), what + "：必须选中同一真实人物");
        if (surface == "World") yield break;

        if (surface == "Person")
        {
            Call(_shell, "OpenSurface", Enum.Parse(T("HomeSurface"), "Person"));
            yield return null;
            Assert.AreEqual("Person", Surface(), what + "：必须进入真实 Person Surface");
            object detail = Get(_shell, "PersonDetail");
            Assert.IsNotNull(detail, what + "：PersonDetail 必须存在");
            Assert.IsTrue((bool)Get(detail, "IsVisible"), what + "：PersonDetail 必须可见");
            Assert.AreEqual(person, (string)Get(detail, "DisplayedPersonId"),
                what + "：Person Surface 必须显示同一 PersonId");
            Assert.IsTrue((bool)Call(detail, "SelectTab", Enum.Parse(T("PersonDetailTab"), "Life")),
                what + "：真实人物页签切换必须成功（读取真实生活事实）");
            Assert.IsNotEmpty((string)Get(detail, "DisplayedText"),
                what + "：Person Surface 必须显示真实页面文本");
            Assert.AreEqual(person, SelectionId(), what + "：打开 Person Surface 不得改变 Selection");
        }
        else
        {
            OpenManagementThroughNavigation(what + "·底部导航·管理");
            yield return null;
            Assert.AreEqual("Management", Surface(), what + "：必须进入真实 Management Surface");
            object management = Management();
            Assert.IsNotNull(management, what + "：Shell.Management 必须是真实 authored Prefab 实例");
            Call(management, "RefreshNow");
            ShowWorkplacesPage(management);
            yield return null;
            Assert.IsNotNull(Get(management, "CurrentSnapshot"), what + "：Management 必须已读取真实投影");
            Assert.AreEqual(person, SelectionId(), what + "：打开 Management 不得改变 Selection");
        }
        Assert.AreEqual(surface, Surface(), what + "：打开后必须停留在目标表面");
        Assert.IsTrue(SimulationActive(), what + "：表面打开后 IsSimulationActive 必须保持 true");
    }

    /// <summary>
    /// 自然帧持续证明（每个表面独立执行）：
    /// 用既有 TopHUD Pause 同一回调把 Clock 从玩家 Pause 恢复为 Normal，随后只 yield 真实帧，
    /// 断言常驻 VillageTimeDriver 推进 Clock、速度与表面不变、IsSimulationActive 保持；
    /// 并等待 host 真实帧重建 World 快照（不是靠测试直接 Advance 冒充自然持续）。
    /// 采样完成后回到玩家 Pause，并由 RestoreBaseWorld 恢复同一 W0 供确定性区间使用。
    /// </summary>
    private IEnumerator NaturalContinuityRun(string surface, string person, string expectedHash)
    {
        string what = "SURFACE-SIM[自然帧/" + surface + "]";
        RestoreBaseWorld(expectedHash, what);
        yield return OpenSurfaceForFacts(surface, person, what);
        Assert.AreEqual(EquivalenceBaseMinute, ClockAbsoluteMinute(), what + "：自然帧必须从同一 W0 分钟开始");
        Assert.AreEqual("Paused", ClockSpeed(), what + "：自然帧前必须仍是玩家手动 Pause");

        SetSecondsPerDay(72f); // 稳定采样：1 真实秒 ≈ 20 世界分钟，真实帧即可观察到整分钟增量。
        Call(_shell, "TogglePause");
        Assert.AreEqual("Normal", ClockSpeed(), what + "：既有手动 Pause 入口必须把 Clock 从 Paused 恢复为 Normal");

        long before = ClockAbsoluteMinute();
        int frames = 0;
        float deadline = Time.realtimeSinceStartup + 10f;
        while (ClockAbsoluteMinute() == before && Time.realtimeSinceStartup < deadline)
        {
            frames++;
            yield return null;
        }
        long afterFrames = ClockAbsoluteMinute();
        Assert.Greater(afterFrames, before,
            what + "：表面打开期间必须由真实帧推进 Clock（不允许只靠直接 Advance 声称自然持续）；frames=" + frames);
        Assert.AreEqual("Normal", ClockSpeed(), what + "：自然帧推进期间表面不得修改玩家选择的速度");
        Assert.AreEqual(surface, Surface(), what + "：自然帧推进期间必须保持在同一表面");
        Assert.IsTrue(SimulationActive(), what + "：自然帧推进期间 IsSimulationActive 必须保持 true");

        object snapshotBefore = Get(_host, "Snapshot");
        int hostFrames = 0;
        deadline = Time.realtimeSinceStartup + 10f;
        while (ReferenceEquals(Get(_host, "Snapshot"), snapshotBefore) && Time.realtimeSinceStartup < deadline)
        {
            hostFrames++;
            yield return null;
        }
        Assert.IsFalse(ReferenceEquals(Get(_host, "Snapshot"), snapshotBefore),
            what + "：host 必须在真实帧中重建 World 快照；hostFrames=" + hostFrames);
        long afterHost = ClockAbsoluteMinute();
        Assert.GreaterOrEqual(afterHost, afterFrames, what + "：host 帧期间 Clock 不得回退");

        Evidence("SURFACE-SIM", what + " natural frameDelta=" + (afterFrames - before)
            + " minute=" + before + "→" + afterFrames + "→" + afterHost
            + " frames=" + frames + " hostFrames=" + hostFrames
            + " speed=" + ClockSpeed() + " simulationActive=" + SimulationActive()
            + " surface=" + Surface() + " hostSnapshotRebuilt=True");

        Call(_shell, "TogglePause");
        Assert.AreEqual("Paused", ClockSpeed(), what + "：自然帧采样后必须能回到玩家 Pause（稳定采样）");
        SetSecondsPerDay(1440f);
        Call(_shell, "CloseSurface");
        yield return null;
        Assert.AreEqual("World", Surface(), what + "：自然帧采样后必须回 World，以便下次恢复同一 W0");
    }

    /// <summary>
    /// 确定性区间：三表面都从同一 W0、同一 Paused 状态出发，用真实 Clock.AdvanceTo 推进到同一绝对目标分钟
    /// （不混入帧时间抖动），再逐项捕获事实。确定性 Advance 与自然帧是两段分开的证据，不互相冒充。
    /// </summary>
    private IEnumerator EquivalenceRun(string surface, string person, string expectedHash, string workplaceId)
    {
        string what = "SURFACE-SIM[确定性Advance/" + surface + "]";
        RestoreBaseWorld(expectedHash, what);
        yield return OpenSurfaceForFacts(surface, person, what);
        Assert.AreEqual(EquivalenceBaseMinute, ClockAbsoluteMinute(), what + "：确定性推进必须从同一 W0 绝对分钟开始");
        Assert.AreEqual("Paused", ClockSpeed(), what + "：确定性推进前必须是玩家 Pause 状态");

        AdvanceTo(EquivalenceTargetMinute);
        Call(_host, "RefreshNow");

        Assert.AreEqual(EquivalenceTargetMinute, ClockAbsoluteMinute(), what + "：三表面必须到达同一绝对目标分钟");
        Assert.AreEqual("Paused", ClockSpeed(), what + "：确定性推进不得改写玩家手动 Pause");
        Assert.IsTrue(SimulationActive(), what + "：确定性推进后 IsSimulationActive 必须仍为 true");
        Assert.AreEqual(surface, Surface(), what + "：推进过程中不得被表面路由打断");

        // 非平凡性：该目标分钟必须真的跨过工作 / 活动机会 / 资源结算 / 施工边界，否则“等价”是空集比较。
        object project = ActiveProject();
        Assert.IsNotNull(project, what + "：目标分钟必须存在真实施工项目（跨施工边界）");
        Assert.Greater(Convert.ToInt32(Get(project, "Progress")), 0, what + "：真实施工必须产生进度");
        Assert.Greater(ProductionBatchCount(), 0, what + "：必须跨过真实生产批次边界");
        Assert.Greater(ProductionEvidenceCount(), 0, what + "：必须留下真实生产/工作证据");
        Assert.Greater(TotalEvidenceMinutes(), 0, what + "：岗位工作证据分钟必须非零");
        Assert.Greater(AssignmentContributionHistory(), 0, what + "：必须跨过真实 Day Tick 累计执行历史边界");
        Assert.Greater(Ingredients(), 0, what + "：资源必须经过真实结算（Ingredients 非零）");
        Assert.Greater(Food(), 0, what + "：资源必须经过真实结算（Food 非零）");
        Assert.AreEqual(workplaceId, PersonWorkplace(person), what + "：职责事实必须保持真实派工");

        var facts = CaptureSurfaceFacts(surface);
        _surfaceRunFacts[surface] = facts;
        Evidence("SURFACE-SIM", what + " clock=" + facts.Clock + " resources=" + facts.Resources
            + " project=" + ProjectDiagnostic() + " batches=" + ProductionBatchCount()
            + " productionEvidence=" + ProductionEvidenceCount()
            + " evidenceMinutes=" + TotalEvidenceMinutes()
            + " contributionHistory=" + AssignmentContributionHistory()
            + " socialBytes=" + facts.Social.Length + " activityBytes=" + facts.Activity.Length
            + " projectionLines=" + facts.Projection.Split('\n').Length
            + " claims=" + string.Join(",", HostPersonIds().OrderBy(v => v, StringComparer.Ordinal)
                .Select(id => id + "=" + Nullable(ActivityClaim(id)))));
    }

    private static void AssertSurfaceFactEqual(string surface, string name, string expected, string actual)
    {
        if (string.Equals(expected, actual, StringComparison.Ordinal)) return;
        string left = expected ?? "<null>";
        string right = actual ?? "<null>";
        int at = 0;
        while (at < left.Length && at < right.Length && left[at] == right[at]) at++;
        int from = Math.Max(0, at - 200);
        string expectedWindow = left.Substring(from, Math.Min(400, left.Length - from));
        string actualWindow = right.Substring(from, Math.Min(400, right.Length - from));
        Assert.Fail("三表面必须等价，但 " + name + " 在 " + surface + " 与 World 不一致（首个差异 @" + at
            + "；World 全长 " + left.Length + "，实际全长 " + right.Length + "）。\nWorld 片段=" + expectedWindow
            + "\n" + surface + " 片段=" + actualWindow);
    }

    [UnityTest]
    public IEnumerator SurfaceEquivalentSimulation_WorldPersonManagement_ShareSameWorldFactsAndKeepSimulationContinuous()
    {
        _surfaceRunFacts.Clear();

        // ---------------- 0. 同一真实初始世界 W0：真实建造 / 真实派工 / 真实施工 / 真实资源输入 ----------------
        Assert.AreEqual(0L, ClockAbsoluteMinute(), "夹具必须从新世界 0 分钟开始");
        Assert.AreEqual("Paused", ClockSpeed(), "夹具必须从玩家 Pause 开始以保证确定性");
        Assert.IsTrue(SimulationActive(), "产品入口进入世界后模拟必须 Active");

        string farmB = PlaceBuilding("building_farm", 0f);
        string kitchenC = PlaceBuilding("building_kitchen", 4f);
        Assert.AreNotEqual(farmB, kitchenC, "Farm / Kitchen 必须是两个独立建筑实例");
        string p1 = SnapshotPersonIdAt(0), p2 = SnapshotPersonIdAt(1),
            p4 = SnapshotPersonIdAt(3), p5 = SnapshotPersonIdAt(4);
        Assert.AreNotEqual(p4, p5, "派工人物必须是不同 PersonId");

        // 两栋目标建筑用真实施工参与者完成（既有公开施工入口 + 唯一 Clock），再建立动态岗位与第三栋施工。
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farmB, p1).ToString(),
            "p1 必须能加入 Farm B 施工");
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", kitchenC, p2).ToString(),
            "p2 必须能加入 Kitchen C 施工");
        AdvanceTo(EquivalenceBaseMinute);
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(farmB), "Farm B 必须在真实推进后完工");
        Assert.AreEqual("Completed", BuildingState(kitchenC), "Kitchen C 必须在真实推进后完工");

        string constructionA = PlaceBuilding("building_farm", 8f);
        Assert.AreEqual("UnderConstruction", BuildingState(constructionA), "第三次建造必须保持真实施工中");

        Call(Get(_host, "Selection"), "SelectPerson", p4);
        OpenManagementThroughNavigation("SURFACE-SIM·W0 派工准备");
        yield return null;
        object management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        ShowWorkplacesPage(management);
        yield return null;

        string wFarm = WorkplaceIdForBuilding(management, farmB, "Farm B");
        string wKitchen = WorkplaceIdForBuilding(management, kitchenC, "Kitchen C");
        Assert.IsTrue(wFarm.StartsWith("workplace:" + farmB + ":", StringComparison.Ordinal),
            "Farm B 岗位 ID 必须绑定具体建筑实例：" + wFarm);
        Assert.IsTrue(wKitchen.StartsWith("workplace:" + kitchenC + ":", StringComparison.Ordinal),
            "Kitchen C 岗位 ID 必须绑定具体建筑实例：" + wKitchen);

        SelectForManagement(management, p4);
        yield return null;
        ClickWorkplaceRow(management, wFarm, "W0·岗位行 Farm B");
        yield return null;
        ClickButtonField(management, "assignButton", "W0·派工 p4→Farm B");
        yield return null;
        Assert.AreEqual(wFarm, PersonWorkplace(p4), "p4 必须真实派到 Farm B 动态岗位");

        SelectForManagement(management, p5);
        yield return null;
        ClickWorkplaceRow(management, wKitchen, "W0·岗位行 Kitchen C");
        yield return null;
        ClickButtonField(management, "assignButton", "W0·派工 p5→Kitchen C");
        yield return null;
        Assert.AreEqual(wKitchen, PersonWorkplace(p5), "p5 必须真实派到 Kitchen C 动态岗位");

        SelectForManagement(management, p1);
        yield return null;
        ClickConstructionRow(management, constructionA, "W0·施工行 A（p1）");
        yield return null;
        ClickButtonField(management, "addConstructionButton", "W0·加入施工 p1");
        yield return null;
        SelectForManagement(management, p2);
        yield return null;
        ClickConstructionRow(management, constructionA, "W0·施工行 A（p2）");
        yield return null;
        ClickButtonField(management, "addConstructionButton", "W0·加入施工 p2");
        yield return null;

        object project = ActiveProject();
        Assert.IsNotNull(project, "真实施工按钮加入后必须存在活动施工项目");
        CollectionAssert.AreEquivalent(new[] { p1, p2 }, Strings(Get(project, "ParticipantPersonIds")),
            "施工 A 必须记录两名具体真实参与者");
        Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "W0 暂停在登记分钟时不得凭空产生进度");

        Call(ResourcesState(), "AddIngredients", 2); // 既有公开资源写入口（真实 Kitchen 输入）
        Call(_shell, "HandleEscape");
        yield return null;
        Assert.AreEqual("World", Surface(), "W0 准备完成后必须回 World");

        // 真实产品 Save 写入 W0 单槽（SaveSlotGuard 已捕获并在 TearDown 恢复用户原始字节）。
        object player = Get(_town, "PlayerData");
        Assert.IsNotNull(player, "TownUI.PlayerData 必须来自真实 Load");
        Assert.IsTrue((bool)Static("SaveSystem", "Save", player), "W0 快照必须通过真实 SaveSystem.Save 写入单槽");
        string w0Hash = HashFile(CurrentSavePath());
        Assert.AreEqual(EquivalenceBaseMinute, ClockAbsoluteMinute(), "W0 必须保存在同一绝对分钟");
        Assert.AreEqual("Paused", ClockSpeed(), "W0 必须保存在玩家 Pause 状态");
        Evidence("SURFACE-SIM", "W0 saved hash=" + w0Hash + " clock=" + ClockAbsoluteMinute()
            + " farm=" + farmB + " kitchen=" + kitchenC + " construction=" + constructionA
            + " wFarm=" + wFarm + " wKitchen=" + wKitchen
            + " p4=" + p4 + " p5=" + p5 + " participants=" + TextList(Get(project, "ParticipantPersonIds"))
            + " resources=" + Ingredients() + "/" + Food());

        // ---------------- 1. 自然帧持续（三表面各自独立证明） ----------------
        foreach (string surface in new[] { "World", "Person", "Management" })
            yield return NaturalContinuityRun(surface, p4, w0Hash);

        // ---------------- 2. 确定性推进到同一绝对目标分钟（三表面各自从同一 W0 出发） ----------------
        foreach (string surface in new[] { "World", "Person", "Management" })
            yield return EquivalenceRun(surface, p4, w0Hash, wFarm);

        // ---------------- 3. 逐项比较：Clock / Work / Activity / Social / Resource / 职责施工 ----------------
        SurfaceRunFacts baseline = _surfaceRunFacts["World"];
        foreach (string surface in new[] { "Person", "Management" })
        {
            SurfaceRunFacts run = _surfaceRunFacts[surface];
            Assert.AreEqual("World", baseline.Surface, "基线事实必须来自 World 表面");
            AssertSurfaceFactEqual(surface, "Clock（绝对分钟/速度/倍率/IsSimulationActive）", baseline.Clock, run.Clock);
            AssertSurfaceFactEqual(surface,
                "Village BuildSaveData（Time/IntradayWork/Assignments/Projects/Resources/Social/World/Buildings/State）",
                baseline.Village, run.Village);
            AssertSurfaceFactEqual(surface, "Work 证据与到场登记（IntradayWork.ToSaveData）", baseline.Work, run.Work);
            AssertSurfaceFactEqual(surface, "Activity BuildSaveData", baseline.Activity, run.Activity);
            AssertSurfaceFactEqual(surface, "Social BuildSaveData", baseline.Social, run.Social);
            AssertSurfaceFactEqual(surface, "World 快照（人物/地点/建筑生命周期）", baseline.Host, run.Host);
            AssertSurfaceFactEqual(surface, "真实职责/施工/现场事实", baseline.Roles, run.Roles);
            AssertSurfaceFactEqual(surface, "Management 只读投影（人口/职责场所/施工/资源）", baseline.Projection,
                run.Projection);
            AssertSurfaceFactEqual(surface, "资源 Ingredients/Food", baseline.Resources, run.Resources);
        }

        Evidence("SURFACE-SIM", "END three surfaces equivalent at minute " + EquivalenceTargetMinute
            + ": clock=" + baseline.Clock + " resources=" + baseline.Resources
            + " workEvidenceMinutes=" + TotalEvidenceMinutes()
            + " batches=" + ProductionBatchCount() + " contributionHistory=" + AssignmentContributionHistory()
            + " socialBytes=" + baseline.Social.Length + " activityBytes=" + baseline.Activity.Length);
    }

    // ==================== 用例 11：统一事件 pending 的管理门禁（Advance / Return） ====================

    private static object ProjectOfPerson(string personId)
    {
        object projects = StaticField("VillageRuntimeBridge", "Projects");
        if (projects == null) return null;
        return Call(projects, "GetActiveProjectOfPerson", personId);
    }

    private static string ProjectFacts(object project)
    {
        if (project == null) return "<none>";
        return "id=" + Get(project, "ProjectId")
            + " progress=" + Get(project, "Progress")
            + " labor=" + Get(project, "LaborRemainder")
            + " participants=[" + string.Join(",", Strings(Get(project, "ParticipantPersonIds"))) + "]";
    }

    private static int EventOccurrences(string eventId)
    {
        object world = StaticField("EventRuntimeBridge", "World");
        Assert.IsNotNull(world, "EventRuntimeBridge.World 必须存在（事件长期历史）");
        object history = Get(world, "History");
        Assert.IsNotNull(history, "EventWorldState.History 必须存在");
        return Convert.ToInt32(Call(history, "OccurrenceCount", eventId));
    }

    /// <summary>既有 Clock.Advance 产品入口（不改状态，只提交一次真实请求）；返回 Outcome 供门禁断言。</summary>
    private static object AttemptDirectAdvance(long minute)
    {
        object clock = Clock();
        var clockAssembly = clock.GetType().Assembly;
        object target = Activator.CreateInstance(clockAssembly.GetType("WorldInstant", true), minute);
        object origin = Enum.Parse(clockAssembly.GetType("WorldAdvanceOrigin", true), "Direct");
        return Call(clock, "AdvanceTo", target, origin, null, null);
    }

    /// <summary>
    /// 真实调度制造一次 Temporary Commitment：加入 prj_test_01（既有 Test Project），再真实推进 10 天等待完成。
    /// 同一人物第 3 次加入时，产品既有 post-add Recognition 会自行建立 PendingVillageEvent。
    /// </summary>
    private static void CompleteTestProject(string personId, string what)
    {
        Assert.AreEqual("Success", Bridge("TryStartProject", "prj_test_01", new[] { personId }).ToString(),
            what + "：真实加入 prj_test_01 必须成功");
        AdvanceTo(ClockAbsoluteMinute() + 10L * 1440L);
        Assert.IsNull(ProjectOfPerson(personId), what + "：10 天后真实项目必须完成并释放参与者");
    }

    /// <summary>
    /// 统一事件 pending 的管理门禁（真实产品链）：
    ///   - 不反射写 pending：用既有正式 Recognition 触发条件（同一真实人物第 3 次加入 Active Project）
    ///     让 `EventRuntimeBridge.PendingVillageEvent` 由产品自行建立；
    ///   - pending 存在时真实 `Clock.AdvanceTo`（Direct）与既有 Return Catch-up（Return 的同一 AdvanceSession）
    ///     都必须停在 WaitingForInteraction，时钟不前进；
    ///   - 管理表面真实导航 / 产品开关、ESC、手动 Pause 都不能推进或解锁世界；
    ///   - 管理命令与显式保存按既有 `CanSaveWorld`/WorldBusy 规则拒绝且零写盘、零状态改写；
    ///   - 用真实事件选项合法解决后，只完成一次原 Direct 推进 / 一次原 Return Catch-up，
    ///     职责、施工参与者与资源投影与真实 Authority 一致，重复结算被拒绝（AlreadyCaughtUp）。
    /// 只用既有公共入口；不新增产品 API / DTO / WorkClock / 规则，不改 Advance / Return / Event 语义。
    /// </summary>
    [UnityTest]
    public IEnumerator PendingEvent_BlocksAdvanceAndReturn_ManagementCommandsSaveStayGatedUntilLegalChoice()
    {
        const string EventId = "evt_test_village_temp_draft";
        const string LegalOption = "opt_test_village_continue";
        const long Day = 1440L;

        // 夹具隔离：真实活动机会会让被 claim 的参与者跳过工作到场，导致 Test Project 10 天无法完成；
        // 只在本例时间跨度使用既有公开注入点，TearDown 恢复原日程。
        IsolateActivityOpportunitiesForFixture("统一事件 pending 门禁");
        // 测试夹具隔离（不是门禁绕过）：本类入口不重置长期 Event 世界，上一用例可能留下历史 / Pending。
        // 这里用既有公共 Restore(null)（等价“空事件区块”）清空，随后所有 Pending 只由真实 Recognition 触发条件建立。
        Static("EventRuntimeBridge", "Restore", new object[] { null });
        Assert.IsNull(StaticField("EventRuntimeBridge", "PendingVillageEvent"), "夹具起始不得有遗留 Pending");
        Assert.AreEqual(0L, ClockAbsoluteMinute(), "夹具必须从新世界 0 分钟开始");
        Assert.AreEqual("Paused", ClockSpeed(), "夹具必须从玩家 Pause 开始");

        Call(ResourcesState(), "AddFood", 400);
        Call(ResourcesState(), "AddIngredients", 400);

        // ---------------- 0. 真实基座：已完成 Farm A（含职责）+ 40 天真实临时承诺 + 施工中 Farm B ----------------
        string farmA = PlaceBuilding("building_farm", 4f);
        string builder = SnapshotPersonIdAt(1);
        string party = SnapshotPersonIdAt(0);
        string spare1 = SnapshotPersonIdAt(2);
        string spare2 = SnapshotPersonIdAt(3);
        CollectionAssert.AllItemsAreUnique(new[] { party, builder, spare1, spare2 }, "夹具需要 4 名互不相同的真实人物");
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farmA, builder).ToString(),
            "前置：真实人物必须能加入 Farm A 施工");
        AdvanceTo(10L * Day);
        // 读取同步：AdvanceTo 在本帧内同步完成（不经过 Update 的 0.1 秒周期刷新），
        // 若直接读 _host.Snapshot 会得到加入施工后、推进前的旧投影（UnderConstruction），
        // 而真实 BuildingRuntime 已在工作区间完成结算为 Completed。这里只刷新既有产品只读投影，
        // 不改建筑 / 项目 / 劳动状态；与 ExpeditionReturn 用例的读取方式一致。
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(farmA), "Farm A 必须在真实推进后完工");
        string workplaceA = WorkplaceIdFromProjection(farmA);
        Assert.AreEqual("Success", Bridge("TryAssignPerson", party, workplaceA).ToString(),
            "前置：真实人物必须能派到 Farm A 动态岗位");

        CompleteTestProject(spare1, "pending 前置 A1");
        CompleteTestProject(spare1, "pending 前置 A2");
        CompleteTestProject(spare2, "pending 前置 B1");
        CompleteTestProject(spare2, "pending 前置 B2");
        Assert.IsNull(StaticField("EventRuntimeBridge", "PendingVillageEvent"),
            "两次临时承诺不足以触发 Recognition：不得提前建立 Pending");

        string farmB = PlaceBuilding("building_farm", 8f);
        Assert.AreEqual("UnderConstruction", BuildingState(farmB), "Farm B 必须保持真实施工中");
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", farmB, builder).ToString(),
            "前置：真实人物必须能加入 Farm B 施工");

        // ---------------- 1. 既有合法触发条件建立统一事件 Pending（非私有反射） ----------------
        long gatedClock = ClockAbsoluteMinute();
        string gatedDuty = PersonWorkplace(party);
        int gatedOccupancy = WorkplaceOccupants(workplaceA);
        object constructionProject = ProjectOfPerson(builder);
        string gatedConstruction = ProjectFacts(constructionProject);
        int gatedIngredients = Ingredients();
        int gatedFood = Food();
        int gatedBatches = ProductionBatchCount();
        string gatedSavePath = CurrentSavePath();
        string gatedSaveHash = File.Exists(gatedSavePath) ? HashFile(gatedSavePath) : "<absent>";
        Assert.AreEqual(0, EventOccurrences(EventId), "触发前事件历史必须为 0");

        CompleteTestProject3rd(spare1, "pending 触发 A3");
        object pending = StaticField("EventRuntimeBridge", "PendingVillageEvent");
        Assert.IsNotNull(pending, "第 3 次真实临时承诺必须由产品 Recognition 建立 Unified Event Pending");
        Assert.AreEqual(EventId, Get(pending, "EventId").ToString(), "Pending 必须来自既有 Event 配置");
        string eventInstanceId = (string)Get(pending, "EventInstanceId");
        Assert.IsFalse(string.IsNullOrEmpty(eventInstanceId), "Pending 必须有真实 EventInstanceId");
        Assert.AreEqual(0, EventOccurrences(EventId), "Pending 未解决前不得写事件历史");

        // ---------------- 2. pending 阻断真实 Advance：停在 WaitingForInteraction，不推进时钟 ----------------
        long directTarget = gatedClock + 60L;
        object advance = AttemptDirectAdvance(directTarget);
        Assert.AreEqual("WaitingForInteraction", Get(advance, "Outcome").ToString(),
            "统一事件 pending 必须阻断真实 Advance（不得静默推进）");
        object session = Get(Clock(), "AdvanceSession");
        Assert.IsNotNull(session, "WaitingForInteraction 必须保留原 AdvanceSession（不是取消）");
        Assert.AreEqual("WaitingForInteraction", Get(session, "State").ToString(), "原推进必须停在等待交互");
        Assert.AreEqual("Direct", Get(session, "Origin").ToString(), "原推进来源必须保持 Direct");
        Assert.AreEqual(eventInstanceId, (string)Get(session, "BlockingEventInstanceId"),
            "Advance 必须由该真实 Pending 事件阻塞");
        Assert.AreEqual(directTarget, Convert.ToInt64(Get(Get(session, "OriginalTarget"), "Minute")),
            "原推进目标不得被改写");
        Assert.AreEqual(gatedClock, ClockAbsoluteMinute(), "被阻断的推进不得前进 Clock");
        Assert.IsTrue((bool)StaticField("VillageRuntimeBridge", "IsWorldWriteLocked"),
            "等待交互期间世界写锁必须有效");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "等待交互期间 CanSaveWorld 必须为 false");

        // ---------------- 3. 管理页真实导航 / 开关、ESC、手动 Pause 都不绕过门禁 ----------------
        OpenManagementThroughNavigation("pending 等待中·管理导航");
        object management = Management();
        Assert.AreEqual("Management", Surface(), "真实导航必须提交管理表面（门禁不靠不可达实现）");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "真实管理页必须可见");
        Call(Get(_host, "Selection"), "SelectPerson", party);
        Call(management, "RefreshNow");
        Assert.AreEqual(party, (string)Get(management, "SelectedPersonId"), "管理页必须保持真实人物选择");
        Assert.IsFalse(((Button)Get(management, "saveButton")).interactable,
            "pending 等待中真实保存按钮必须禁用");
        Assert.IsFalse(((Button)Get(management, "removeButton")).interactable,
            "pending 等待中真实撤职按钮必须禁用");
        Assert.IsFalse((bool)Call(management, "Save"), "真实保存 UI 路径必须返回失败");
        AssertStatusContains(management, "未保存");
        AssertStatusContains(management, "世界正在推进");
        Assert.IsFalse((bool)Call(management, "RemoveSelectedPersonAssignment"), "真实撤职 UI 路径必须返回失败");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "打开管理页 / 刷新 / 命令 UI 不得解除等待交互");
        Assert.AreEqual(gatedClock, ClockAbsoluteMinute(), "打开管理页不得推进 Clock");

        Call(_shell, "HandleEscape");
        Assert.AreEqual("World", Surface(), "ESC 必须回到 World");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "ESC 后管理页必须关闭");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "ESC 不得解除等待交互");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"), "ESC 不得提前解锁世界");

        // 事件 UI 的首次 Present 必须落在确定的玩家速度上（既有 PauseOnOpen 契约：关闭后恢复“打开前 Speed”）。
        // 旧夹具在此处先 TogglePause→Normal，再依赖 VillageTimeDriver 的真实帧去首次 ShowEventPending，
        // 于是“打开前 Speed”被记成 Normal，关闭时按契约恢复 Normal —— 这不是产品丢失玩家 Pause，
        // 而是夹具把 RestoreAfterClose 用来恢复的采样点（事件打开时刻）留在了 Resume 之后。
        // 这里用既有公开入口先行 Present：打开前速度确定为玩家显式 Pause，随后再验证 Resume 帧不推进世界。
        string speedBeforeEventOpen = ClockSpeed();
        Assert.AreEqual("Paused", speedBeforeEventOpen, "事件打开前必须是玩家显式 Pause");
        Bridge("PresentAdvanceInteraction");
        object openedEventUi = Find("VillageSocialEventUI");
        Assert.IsNotNull(openedEventUi, "既有事件 UI 必须存在");
        Assert.IsTrue(((GameObject)Get(openedEventUi, "root")).activeSelf,
            "Present 后既有事件 UI root 必须真实可见");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "打开事件不得越过等待交互");
        Assert.AreEqual(speedBeforeEventOpen, ClockSpeed(),
            "PauseOnOpen：打开前已是玩家 Pause 时不得重复暂停 / 改速");
        Evidence("EVENT-GATE", "SPEED open paused=" + speedBeforeEventOpen
            + " clock=" + ClockAbsoluteMinute()
            + " session=" + Get(Get(Clock(), "AdvanceSession"), "State"));

        Call(_shell, "TogglePause");
        Assert.AreEqual("Normal", ClockSpeed(), "手动 Resume 必须真实改速（门禁不靠“顶部控制不可达”实现）");
        yield return null;
        yield return null;
        yield return null;
        Assert.AreEqual("Normal", ClockSpeed(), "事件跨真实帧不得改写玩家手动选择的 Normal");
        Assert.AreEqual(gatedClock, ClockAbsoluteMinute(),
            "手动恢复 Clock 不得启动第二 Clock 推进被事件阻塞的世界");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "手动 Pause / Resume 不得解除等待交互");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "手动 Pause / Resume 不得提前解锁命令与保存");
        Assert.IsNotNull(StaticField("EventRuntimeBridge", "PendingVillageEvent"),
            "手动 Pause / Resume 不得清掉待处理事件");
        Assert.IsTrue(((GameObject)Get(openedEventUi, "root")).activeSelf,
            "手动 Resume 不得关闭待处理事件的既有 UI");
        Call(Clock(), "Pause");
        Assert.AreEqual("Paused", ClockSpeed(), "采样后必须回到玩家 Pause");
        Evidence("EVENT-GATE", "SPEED resume=Normal clock=" + ClockAbsoluteMinute()
            + " then playerPause=" + ClockSpeed());

        // ---------------- 4. 命令与显式保存按既有 WorldBusy 规则拒绝，零写盘 / 零状态改写 ----------------
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "AssignToWorkplace", party, workplaceA)),
            "pending 等待中派工必须 WorldBusy");
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "RemoveAssignment", party)),
            "pending 等待中撤职必须 WorldBusy");
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "AddConstructionWorker", farmB, builder)),
            "pending 等待中加入施工必须 WorldBusy");
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "RemoveConstructionWorker", farmB, builder)),
            "pending 等待中撤出施工必须 WorldBusy");
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "SaveWorld")),
            "pending 等待中显式保存必须 WorldBusy");

        Assert.AreEqual(gatedDuty, PersonWorkplace(party), "被拒命令不得改写真实职责");
        Assert.AreEqual(gatedOccupancy, WorkplaceOccupants(workplaceA), "被拒命令不得改写岗位占用");
        Assert.AreEqual(gatedConstruction, ProjectFacts(ProjectOfPerson(builder)), "被拒命令不得改写真实施工事实");
        Assert.AreEqual(gatedIngredients, Ingredients(), "门禁期间资源 Ingredients 必须零变化");
        Assert.AreEqual(gatedFood, Food(), "门禁期间资源 Food 必须零变化");
        Assert.AreEqual(gatedBatches, ProductionBatchCount(), "门禁期间不得产生生产批次");
        Assert.AreEqual(gatedClock, ClockAbsoluteMinute(), "被拒命令 / 保存不得推进 Clock");
        Assert.AreEqual(gatedSaveHash, File.Exists(gatedSavePath) ? HashFile(gatedSavePath) : "<absent>",
            "pending 等待期间保存必须零写盘");

        // ---------------- 5. 合法选择解决事件 → 只完成一次原 Direct 推进 ----------------
        Bridge("PresentAdvanceInteraction");
        object eventUi = Find("VillageSocialEventUI");
        Assert.IsNotNull(eventUi, "既有事件 UI 必须存在");
        Assert.IsTrue(((GameObject)Get(eventUi, "root")).activeSelf,
            "合法选择前既有事件 UI 必须仍处于打开状态（不是重新 Present 出的另一个状态）");
        Assert.AreEqual(speedBeforeEventOpen, ClockSpeed(),
            "二次 Present 不得改写事件打开时记录的速度（关闭恢复的目标不变）");
        Call(eventUi, "OnEventOptionClicked", LegalOption);
        Assert.IsNull(StaticField("EventRuntimeBridge", "PendingVillageEvent"),
            "合法选项必须清除 Pending");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "选择本身不得越过 acknowledgement 直接恢复推进");
        Assert.AreEqual(1, EventOccurrences(EventId), "合法解决必须只写一次事件历史");
        Call(eventUi, "OnCloseClicked");
        Assert.AreEqual("Yielded", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "既有 UI 关闭必须把已处理交互 acknowledge 为 Yielded");
        Bridge("PumpWorldAdvance");
        Assert.IsNull(Get(Clock(), "AdvanceSession"), "原推进必须完成并释放 AdvanceSession");
        Assert.AreEqual(directTarget, ClockAbsoluteMinute(), "原推进必须只完成到原目标（不多推进一分钟）");
        Assert.AreEqual("Paused", ClockSpeed(), "事件关闭后必须保持玩家 Pause");
        Assert.AreEqual(speedBeforeEventOpen, ClockSpeed(),
            "关闭必须恢复事件打开前记录的速度（玩家最后合法速度），而不是改成 Normal");
        Assert.AreEqual(gatedDuty, PersonWorkplace(party), "解决事件后真实职责必须保持一致");
        Assert.AreEqual(gatedOccupancy, WorkplaceOccupants(workplaceA), "解决事件后岗位占用必须保持一致");
        Assert.AreEqual(gatedConstruction, ProjectFacts(ProjectOfPerson(builder)),
            "0:00–1:00 非工作时段不得凭空改写真实施工事实");
        Assert.AreEqual(gatedIngredients, Ingredients(), "解决事件后资源 Ingredients 必须一致");
        Assert.AreEqual(gatedFood, Food(), "解决事件后资源 Food 必须一致");
        Assert.AreEqual(gatedBatches, ProductionBatchCount(), "解决事件后不得重复生产");
        Evidence("EVENT-GATE", "direct advance resumed once clock=" + directTarget
            + " duty=" + PersonWorkplace(party) + " occupancy=" + WorkplaceOccupants(workplaceA)
            + " construction=" + ProjectFacts(ProjectOfPerson(builder))
            + " resources=" + Ingredients() + "/" + Food()
            + " history=" + EventOccurrences(EventId));

        // ---------------- 6. pending 阻断真实 Return Catch-up（同一 AdvanceSession，Origin=Return） ----------------
        long returnStart = ClockAbsoluteMinute();
        int returnIngredients = Ingredients();
        int returnFood = Food();
        int returnBatches = ProductionBatchCount();
        string returnDuty = PersonWorkplace(party);
        int returnOccupancy = WorkplaceOccupants(workplaceA);
        string returnConstruction = ProjectFacts(ProjectOfPerson(builder));

        CompleteTestProject3rd(spare2, "return pending 触发 B3");
        object returnPending = StaticField("EventRuntimeBridge", "PendingVillageEvent");
        Assert.IsNotNull(returnPending, "第 2 个真实触发条件必须建立新的 Pending");
        string returnEventInstanceId = (string)Get(returnPending, "EventInstanceId");
        Assert.AreNotEqual(eventInstanceId, returnEventInstanceId, "Return 用例必须是新的真实事件实例");

        object runEndMode = Enum.Parse(LoadedType("RunEndMode"), "SafeReturn");
        Assert.IsTrue((bool)Static("VillageRuntimeBridge", "BeginExpeditionReturnMinutes", Day, runEndMode),
            "既有公共 Return 入口必须能建立一次真实返程会话");
        Assert.IsTrue((bool)Static("VillageRuntimeBridge", "MarkExpeditionReturnSettled"),
            "既有 Settlement 完成后 Return 相位必须进入 ReturnSettled");
        object catchUp = Bridge("RunReturnCatchUp");
        Assert.AreEqual("WaitingForInteraction", catchUp.ToString(),
            "统一事件 pending 必须阻断既有 Return Catch-up");
        object returnSession = Get(Clock(), "AdvanceSession");
        Assert.IsNotNull(returnSession, "Return 等待交互必须保留原 AdvanceSession");
        Assert.AreEqual("WaitingForInteraction", Get(returnSession, "State").ToString(), "Return 必须停在等待交互");
        Assert.AreEqual("Return", Get(returnSession, "Origin").ToString(), "Return Catch-up 必须保持 Return 来源");
        Assert.AreEqual(returnEventInstanceId, (string)Get(returnSession, "BlockingEventInstanceId"),
            "Return 必须由该真实 Pending 事件阻塞");
        Assert.AreEqual(returnStart + Day, Convert.ToInt64(Get(Get(returnSession, "OriginalTarget"), "Minute")),
            "Return 原目标不得被改写");
        Assert.AreEqual(returnStart, ClockAbsoluteMinute(), "被阻断的 Return 不得前进 Clock");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "Return 未完成前 CanSaveWorld 必须为 false");
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "SaveWorld")),
            "Return 等待中显式保存必须 WorldBusy");
        Assert.AreEqual("WorldBusy", CommandOutcome(Static("ManagementCommandGateway", "AssignToWorkplace", party, workplaceA)),
            "Return 等待中派工必须 WorldBusy");
        Assert.AreEqual(returnIngredients, Ingredients(), "Return 等待中 Ingredients 不得变化");
        Assert.AreEqual(returnFood, Food(), "Return 等待中资源不得变化");
        Assert.AreEqual(returnBatches, ProductionBatchCount(), "Return 等待中不得产生生产批次");
        Assert.AreEqual(returnConstruction, ProjectFacts(ProjectOfPerson(builder)), "Return 等待中施工事实不得变化");
        Assert.AreEqual(gatedSaveHash, HashFile(CurrentSavePath()), "Return 等待中保存必须零写盘");

        Call(_shell, "OpenSurface", Enum.Parse(T("HomeSurface"), "Management"));
        Assert.AreEqual("Management", Surface(), "产品开关可请求管理表面（用于门禁断言）");
        management = Management();
        Assert.IsTrue((bool)Get(management, "IsVisible"), "世界可见时管理表面必须实体显示");
        Call(Get(_host, "Selection"), "SelectPerson", party);
        Call(management, "RefreshNow");
        Assert.IsFalse(((Button)Get(management, "saveButton")).interactable, "Return 等待中保存按钮必须禁用");
        Assert.IsFalse((bool)Call(management, "Save"), "Return 等待中真实保存 UI 必须失败");
        AssertStatusContains(management, "未保存");
        AssertStatusContains(management, "世界正在推进");
        Call(_shell, "HandleEscape");
        Assert.AreEqual("World", Surface(), "ESC 必须回到 World");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "管理开关 / ESC 不得解除 Return 等待交互");

        // ---------------- 7. 合法选择解决事件 → 只完成一次原 Return 结算 ----------------
        Bridge("PresentAdvanceInteraction");
        eventUi = Find("VillageSocialEventUI");
        Assert.IsNotNull(eventUi, "既有事件 UI 必须存在");
        Call(eventUi, "OnEventOptionClicked", LegalOption);
        Assert.IsNull(StaticField("EventRuntimeBridge", "PendingVillageEvent"), "合法选项必须清除 Return Pending");
        Assert.AreEqual(2, EventOccurrences(EventId), "第二次合法解决必须只追加一次事件历史");
        Assert.AreEqual("WaitingForInteraction", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "Return 选择本身不得直接恢复推进");
        Call(eventUi, "OnCloseClicked");
        Assert.AreEqual("Yielded", Get(Get(Clock(), "AdvanceSession"), "State").ToString(),
            "Return 已处理交互必须 acknowledge 为 Yielded");
        catchUp = Bridge("RunReturnCatchUp");
        Assert.AreEqual("Completed", catchUp.ToString(), "原 Return Catch-up 必须只完成一次");
        Assert.AreEqual(returnStart + Day, ClockAbsoluteMinute(), "Return Catch-up 必须只推进 1 天");
        object returnRecord = StaticField("VillageRuntimeBridge", "ActiveReturnRecord");
        Assert.IsNotNull(returnRecord, "Return 会话记录必须存在");
        Assert.AreEqual("CatchUpComplete", Get(returnRecord, "Phase").ToString(), "Return 必须完成 Catch-up 相位");
        Assert.AreEqual(1, Convert.ToInt32(Get(returnRecord, "DaysSimulated")), "Return 只允许模拟 1 天");
        Assert.IsFalse((bool)StaticField("VillageRuntimeBridge", "CanSaveWorld"),
            "Return 完成最终 Save 前仍需保持写锁（本次不提前解锁）");

        // 记录单次结算后的真实事实，再重复调用证明不会重复结算。
        int settledIngredients = Ingredients();
        int settledFood = Food();
        int settledBatches = ProductionBatchCount();
        string settledConstruction = ProjectFacts(ProjectOfPerson(builder));
        Assert.AreEqual(returnDuty, PersonWorkplace(party), "Return 结算后真实职责必须保持一致");
        Assert.AreEqual(returnOccupancy, WorkplaceOccupants(workplaceA), "Return 结算后岗位占用必须一致");
        Assert.AreEqual("UnderConstruction", BuildingState(farmB), "1 天结算不得异常完成 Farm B 施工");
        Assert.IsTrue(Strings(Get(ProjectOfPerson(builder), "ParticipantPersonIds")).Contains(builder),
            "Return 结算后真实施工参与者必须保持");

        catchUp = Bridge("RunReturnCatchUp");
        Assert.AreEqual("AlreadyCaughtUp", catchUp.ToString(), "重复 Return Catch-up 必须被拒绝（不重复结算）");
        Assert.AreEqual(returnStart + Day, ClockAbsoluteMinute(), "重复 Return 不得再推进 Clock");
        Assert.AreEqual(settledIngredients, Ingredients(), "重复 Return 不得重复结算 Ingredients");
        Assert.AreEqual(settledFood, Food(), "重复 Return 不得重复结算 Food");
        Assert.AreEqual(settledBatches, ProductionBatchCount(), "重复 Return 不得重复生产批次");
        Assert.AreEqual(settledConstruction, ProjectFacts(ProjectOfPerson(builder)),
            "重复 Return 不得重复改写施工事实");

        // 管理只读投影必须与真实 Authority 一致（职责 / 施工 / 资源）。
        object projection = ReadManagementProjection();
        Assert.IsNotNull(projection, "管理只读投影必须可读");
        object resourcesRow = Get(projection, "Resources");
        Assert.AreEqual(Ingredients(), Convert.ToInt32(Get(resourcesRow, "Ingredients")),
            "资源投影 Ingredients 必须等于真实 Authority");
        Assert.AreEqual(Food(), Convert.ToInt32(Get(resourcesRow, "Food")),
            "资源投影 Food 必须等于真实 Authority");
        object personRow = ((IEnumerable)Get(projection, "Persons")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "PersonId"), party, StringComparison.Ordinal));
        Assert.IsNotNull(personRow, "管理投影必须包含真实职责人物");
        Assert.AreEqual(PersonWorkplace(party), (string)Get(personRow, "PrimaryWorkplaceId"),
            "职责投影必须等于真实 Assignment");
        object constructionRow = ((IEnumerable)Get(projection, "Construction")).Cast<object>()
            .FirstOrDefault(row => string.Equals((string)Get(row, "BuildingInstanceId"), farmB, StringComparison.Ordinal));
        Assert.IsNotNull(constructionRow, "管理投影必须包含施工中的 Farm B");
        CollectionAssert.AreEquivalent(
            Strings(Get(ProjectOfPerson(builder), "ParticipantPersonIds")),
            Strings(Get(constructionRow, "WorkerPersonIds")),
            "施工投影工人必须等于真实施工参与者");

        Evidence("EVENT-GATE", "END return catchup once clock=" + ClockAbsoluteMinute()
            + " phase=" + Get(returnRecord, "Phase") + " days=" + Get(returnRecord, "DaysSimulated")
            + " history=" + EventOccurrences(EventId)
            + " duty=" + PersonWorkplace(party) + " occupancy=" + WorkplaceOccupants(workplaceA)
            + " construction=" + ProjectFacts(ProjectOfPerson(builder))
            + " resources=" + Ingredients() + "/" + Food()
            + " repeat=AlreadyCaughtUp projection=consistent");
    }

    // ==================== 用例 15：双 FunctionSlot 真实 UI / SaveLoad + 建造态 ESC 回调顺序机制 ====================

    /// <summary>
    /// R3-B 普通可测剩余包（原 14 例之外新增的唯一 Play 用例）：
    ///   1) 复用 StaticConfigRegistry 的 Editor-only internal 测试缝（与第 19 个 Edit 用例同一内存双 slot 口径：
    ///      真实 building_farm 定义 + main(ws_farm_01 / Capacity 2) + aux(ws_test_02 / Capacity 1)），
    ///      不写盘、不改生产配置 / private 字段 / public API / Authority；
    ///   2) 全程真实产品入口：既有公开建造入口 place → 管理页真实“加入施工”按钮 → 真实时钟推进完工 →
    ///      FinalizeConstruction 自动注册两个 Workplace（不造 Completed / assignment / arrival）；
    ///   3) 管理页同一 BuildingInstanceId 下 main / aux 两个独立岗位行，真实点击两行分别派不同人；
    ///      容量 / 绑定 / 劳动（LaborCount）/ 现场（OnsiteCount）/ 逐人 Evidence 必须互不影响；
    ///      满员覆盖两条真实路径：Gateway 命令真实 CapacityFull 诊断 + 原 UI 满员禁用仍保留；
    ///   4) 显式 Save（管理页真实按钮 + bool 回执）→ 真实 MainMenu Continue → SaveSystem.Load 换源 →
    ///      重新进入管理页：两 slot 的职责 / 人物 / 建筑实例与槽位 / 资源 / Clock / 生产计数逐项相等且无重复生产；
    ///   5) 建造态 ESC **回调顺序机制**（不是物理 ESC / 不是 Input.ReadInput）：公开 Build.OpenCatalog 打开候选后
    ///      Shell.HandleEscape 不关闭建造面板、不进入 CloseSurface 分支；公开 Build.Cancel 关闭候选后，
    ///      HandleEscape 才关闭当前（非 World）Surface；
    ///   6) finally 只恢复本次覆盖并复核默认 registry 内容；SaveSlotGuard / 34 项 / 字体保护仍由既有
    ///      OneTimeSetUp / UnitySetUp / TearDown / OneTimeTearDown 链执行，本用例不短路任何一环。
    ///
    /// 边界（不冒称）：程序化 EventSystem 点击不等于物理鼠标；建造 / 管理输入未走 Input.ReadInput 的真实按键路径；
    /// 本用例不证明 Player 编译（双 slot 依赖 Editor-only 测试缝）。
    /// </summary>
    [UnityTest]
    public IEnumerator ManagementTwoSlots_UiSaveLoadAndBuildEscapeCallbacks_KeepIndependentFacts()
    {
        Assert.IsTrue(SimulationActive(), "产品入口进入世界后模拟必须 Active");
        Assert.AreEqual("Paused", ClockSpeed(), "本用例必须从玩家 Pause 开始以保证确定性");
        Assert.AreEqual(0L, ClockAbsoluteMinute(), "本用例必须从真实新世界 0 分钟开始");

        string baselineShape = DescribeRegistry(TryGetBuildingsRegistry(out string baselineDiagnostic));
        Assert.IsFalse(string.IsNullOrEmpty(baselineShape), "默认建筑注册表必须可读：" + baselineDiagnostic);
        object previousOverride = BuildingsOverride();
        Assert.IsNull(previousOverride, "开工前置：不得已有其他用例遗留的 BuildingRegistry 覆盖");
        Evidence("CASE-E", "START baselineRegistry=" + baselineShape.Length + "chars override=<null>");

        string farm = null, wMain = null, wAux = null, workerA = null, workerB = null,
            mainOwner = null, auxOwner = null, lateJoiner = null;
        long savedClock = 0;
        int savedIngredients = 0, savedFood = 0, savedBatches = 0, savedEvidence = 0, savedCompleted = 0;
        try
        {
            // ---------------- 1. 仅内存双 slot 覆盖（Editor-only internal 测试缝，不改生产配置） ----------------
            object twoSlot = RegistryWithAuxSlot(TryGetBuildingsRegistry(out _));
            SetBuildingsOverride(twoSlot);
            Assert.AreSame(twoSlot, BuildingsOverride(), "内存双 slot 覆盖必须真实生效");
            string twoSlotShape = DescribeRegistry(twoSlot);
            Assert.IsTrue(twoSlotShape.StartsWith(baselineShape, StringComparison.Ordinal),
                "覆盖不得改写真实建筑定义目录与前序 FunctionBindings");
            StringAssert.EndsWith("building_farm:aux:Workplace:ws_test_02", twoSlotShape,
                "覆盖必须在真实目录末尾追加 aux 绑定");
            Evidence("CASE-E", "OVERRIDE twoSlot=" + twoSlotShape);

            // 夹具活动窗口隔离（既有公开注入点 ConfigureActivitySchedule；与 CASE-B 同一理由）：
            // 本例需要两名绑定人在下一工作日真实到场劳动，生产默认 420/1020/1200 机会点会以
            // “自主活动优先于工作到场登记”的既有规则随机占人；这里只排除本例时间跨度内的机会点，
            // 不改产品活动规则；TearDown 仍按既有链恢复日程。
            IsolateActivityOpportunitiesForFixture("CASE-E two-slot ui/save fixture");

            // ---------------- 2. 真实 place → 管理页真实施工按钮 → 真实推进完工自动注册 ----------------
            farm = PlaceBuilding("building_farm", 4f);
            Assert.IsFalse(string.IsNullOrEmpty(farm), "公开建造入口必须产生真实建筑实例");
            Assert.AreEqual("UnderConstruction", BuildingState(farm), "新放置建筑必须真实处于施工中");
            workerA = SnapshotPersonIdAt(0);
            workerB = SnapshotPersonIdAt(1);
            mainOwner = SnapshotPersonIdAt(2);
            auxOwner = SnapshotPersonIdAt(3);
            lateJoiner = SnapshotPersonIdAt(4);
            Assert.AreEqual(5, new[] { workerA, workerB, mainOwner, auxOwner, lateJoiner }.Distinct().Count(),
                "本夹具需要 5 名互不相同的真实人物");
            Evidence("CASE-E", "PLACE farm=" + farm + " workerA=" + workerA + " workerB=" + workerB
                + " mainOwner=" + mainOwner + " auxOwner=" + auxOwner + " lateJoiner=" + lateJoiner);

            Call(Get(_host, "Selection"), "SelectPerson", workerA);
            OpenManagementThroughNavigation("管理·双槽施工准备");
            yield return null;
            object management = Management();
            Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
            ShowWorkplacesPage(management);
            yield return null;

            ClickConstructionRow(management, farm, "施工行·双槽 Farm（workerA）");
            yield return null;
            ClickButtonField(management, "addConstructionButton", "加入施工·workerA");
            yield return null;
            SelectForManagement(management, workerB);
            yield return null;
            ClickConstructionRow(management, farm, "施工行·双槽 Farm（workerB）");
            yield return null;
            ClickButtonField(management, "addConstructionButton", "加入施工·workerB");
            yield return null;

            object project = ActiveProject();
            Assert.IsNotNull(project, "真实加入施工后必须存在活动施工项目");
            CollectionAssert.AreEquivalent(new[] { workerA, workerB }, Strings(Get(project, "ParticipantPersonIds")),
                "施工参与者必须来自真实“加入施工”按钮");
            Assert.AreEqual(0, Convert.ToInt32(Get(project, "Progress")), "暂停在登记分钟时不得凭空产生进度");

            Call(_shell, "HandleEscape");
            yield return null;
            Assert.AreEqual("World", Surface(), "施工准备后 ESC 必须回 World");
            AdvanceTo(14400);
            Call(_host, "RefreshNow");
            Assert.AreEqual("Completed", BuildingState(farm), "两名真实工人的真实推进必须让建筑完工");
            Assert.IsNull(ActiveProject(), "完工后不得残留活动施工项目");

            // ---------------- 3. 管理页：同一实例 main / aux 两个独立岗位行（自动注册，不造 assignment） ----------------
            OpenManagementThroughNavigation("管理·双槽岗位");
            yield return null;
            management = Management();
            ShowWorkplacesPage(management);
            yield return null;
            wMain = WorkplaceIdForSlot(management, farm, "main", "双槽 Farm · main");
            wAux = WorkplaceIdForSlot(management, farm, "aux", "双槽 Farm · aux");
            Assert.AreNotEqual(wMain, wAux, "同一建筑实例的两个 FunctionSlot 必须是独立岗位");
            Assert.AreEqual("workplace:" + farm + ":main", wMain, "main 岗位 ID 必须绑定实例 + 真实槽位");
            Assert.AreEqual("workplace:" + farm + ":aux", wAux, "aux 岗位 ID 必须绑定实例 + 真实槽位");
            object mainRow = WorkplaceRowFor(management, farm, "main", "双槽 Farm · main");
            object auxRow = WorkplaceRowFor(management, farm, "aux", "双槽 Farm · aux");
            Assert.IsTrue((bool)Get(mainRow, "IsRegistered"), "完工链必须自动注册 main（不手工造 assignment）");
            Assert.IsTrue((bool)Get(auxRow, "IsRegistered"), "完工链必须自动注册 aux（不手工造 assignment）");
            Assert.AreEqual(2, Convert.ToInt32(Get(mainRow, "Capacity")), "main 容量必须取自真实 profile ws_farm_01");
            Assert.AreEqual(1, Convert.ToInt32(Get(auxRow, "Capacity")), "aux 容量必须取自真实 profile ws_test_02");
            Assert.AreEqual(0, Convert.ToInt32(Get(mainRow, "BoundCount")), "完工当下 main 不得有凭空绑定");
            Assert.AreEqual(0, Convert.ToInt32(Get(auxRow, "BoundCount")), "完工当下 aux 不得有凭空绑定");
            Assert.AreEqual(2, ((IEnumerable)Get(Get(management, "CurrentSnapshot"), "Workplaces")).Cast<object>()
                .Count(row => string.Equals((string)Get(row, "BuildingInstanceId"), farm, StringComparison.Ordinal)),
                "同一实例的两个 FunctionSlot 必须各占一行");
            Evidence("CASE-E", "SLOTS wMain=" + wMain + " wAux=" + wAux
                + " main.capacity=" + Get(mainRow, "Capacity") + " aux.capacity=" + Get(auxRow, "Capacity"));

            // ---------------- 4. 真实点击两行分别派不同人 ----------------
            SelectForManagement(management, mainOwner);
            yield return null;
            ClickWorkplaceRow(management, wMain, "岗位行·main");
            yield return null;
            ClickButtonField(management, "assignButton", "派工·mainOwner→main");
            yield return null;
            Assert.AreEqual(wMain, PersonWorkplace(mainOwner), "真实派工按钮必须写入 mainOwner 的 main 职责");
            Assert.AreEqual(1, WorkplaceOccupants(wMain), "main 真实名额占用必须为 1");
            Assert.AreEqual(0, WorkplaceOccupants(wAux), "main 派工不得影响 aux 名额");

            SelectForManagement(management, auxOwner);
            yield return null;
            ClickWorkplaceRow(management, wAux, "岗位行·aux");
            yield return null;
            ClickButtonField(management, "assignButton", "派工·auxOwner→aux");
            yield return null;
            Assert.AreEqual(wAux, PersonWorkplace(auxOwner), "真实派工按钮必须写入 auxOwner 的 aux 职责");
            Assert.AreEqual(1, WorkplaceOccupants(wAux), "aux 真实名额占用必须为 1");
            Assert.AreEqual(1, WorkplaceOccupants(wMain), "aux 派工不得改变 main 名额");
            AssertStatusContains(management, "已绑定");
            Evidence("CASE-E", "ASSIGN main=" + mainOwner + "→" + wMain + " aux=" + auxOwner + "→" + wAux
                + " main.occupants=" + WorkplaceOccupants(wMain) + " aux.occupants=" + WorkplaceOccupants(wAux));

            // ---------------- 5. 满员：真实 CapacityFull 诊断（aux 满 → main 仍可派） ----------------
            object auxFull = Static("ManagementCommandGateway", "AssignToWorkplace", workerB, wAux);
            Assert.AreEqual("DomainRejected", CommandOutcome(auxFull), "aux 满员必须被领域拒绝：" + auxFull);
            Assert.AreEqual("CapacityFull", (string)Get(auxFull, "DomainDiagnostic"),
                "aux 满员必须返回真实 CapacityFull 领域诊断");
            Assert.IsNull(PersonWorkplace(workerB), "满员拒绝不得写入任何职责");
            Assert.AreEqual(1, WorkplaceOccupants(wAux), "满员拒绝不得改变 aux 占用");
            Assert.AreEqual(1, WorkplaceOccupants(wMain), "aux 满员不得改变 main 名额");

            // main 仍有空位：lateJoiner 经真实 UI 派入 main → 真实填满 Capacity=2。
            SelectForManagement(management, lateJoiner);
            yield return null;
            ClickWorkplaceRow(management, wMain, "岗位行·main（补满）");
            yield return null;
            ClickButtonField(management, "assignButton", "派工·lateJoiner→main");
            yield return null;
            Assert.AreEqual(wMain, PersonWorkplace(lateJoiner), "main 第二名额必须真实写入");
            Assert.AreEqual(2, WorkplaceOccupants(wMain), "main 必须被真实填满（Capacity=2）");
            Assert.AreEqual(1, WorkplaceOccupants(wAux), "main 填满不得改变 aux 名额");

            // main 满员：真实命令诊断 + 原 UI 满员禁用仍保留。
            object mainFull = Static("ManagementCommandGateway", "AssignToWorkplace", workerB, wMain);
            Assert.AreEqual("DomainRejected", CommandOutcome(mainFull), "main 满员必须被领域拒绝：" + mainFull);
            Assert.AreEqual("CapacityFull", (string)Get(mainFull, "DomainDiagnostic"),
                "main 满员必须返回真实 CapacityFull 领域诊断");
            Assert.IsNull(PersonWorkplace(workerB), "main 满员拒绝不得写入");
            Assert.AreEqual(2, WorkplaceOccupants(wMain));
            Assert.AreEqual(1, WorkplaceOccupants(wAux), "main 满员不得改变 aux 名额");
            Evidence("CASE-E", "FULL aux.CapacityFull=True main.CapacityFull=True"
                + " aux.occupants=1 main.occupants=2 no-write=True");

            SelectForManagement(management, workerB);
            yield return null;
            ClickWorkplaceRow(management, wMain, "岗位行·main（满员目标）");
            yield return null;
            var assignButton = (Button)Get(management, "assignButton");
            Assert.IsNotNull(assignButton, "派工按钮必须绑定");
            Assert.IsFalse(assignButton.interactable, "满员目标必须禁派工按钮（CanAssign=false 原规则保留）");
            ClickButtonField(management, "assignButton", "派工·workerB→满员 main（禁用点击）", false);
            yield return null;
            Assert.IsNull(PersonWorkplace(workerB), "满员禁用点击不得写入");
            Assert.AreEqual(2, WorkplaceOccupants(wMain), "满员禁用点击不得改变 main 占用");

            // ---------------- 6. 真实工作时间推进：劳动 / 现场 / Evidence 按 slot 独立 ----------------
            Call(_shell, "HandleEscape");
            yield return null;
            Assert.AreEqual("World", Surface(), "满员核对后 ESC 必须回 World");
            AdvanceTo(16360);
            Call(_host, "RefreshNow");
            yield return null;
            Assert.AreEqual(16360L, ClockAbsoluteMinute(), "真实推进必须停在次日午前 16360 分钟");

            OpenManagementThroughNavigation("管理·双槽劳动核对");
            yield return null;
            management = Management();
            ShowWorkplacesPage(management);
            yield return null;
            mainRow = WorkplaceRowFor(management, farm, "main", "双槽 Farm · main（劳动）");
            auxRow = WorkplaceRowFor(management, farm, "aux", "双槽 Farm · aux（劳动）");
            Assert.AreEqual(2, Convert.ToInt32(Get(mainRow, "BoundCount")), "main 绑定人必须为 2");
            Assert.AreEqual(1, Convert.ToInt32(Get(auxRow, "BoundCount")), "aux 绑定人必须为 1");
            Assert.AreEqual(2, Convert.ToInt32(Get(mainRow, "LaborCount")),
                "main 的两名绑定人必须都在 main 真实劳动（不得把 aux 劳动混入）");
            Assert.AreEqual(1, Convert.ToInt32(Get(auxRow, "LaborCount")),
                "aux 只应有 auxOwner 在 aux 真实劳动（不得把 main 劳动混入）");
            Assert.AreEqual(2, Convert.ToInt32(Get(mainRow, "OnsiteCount")), "main 现场必须是两名绑定人");
            Assert.AreEqual(1, Convert.ToInt32(Get(auxRow, "OnsiteCount")), "aux 现场必须只有 auxOwner");
            Assert.Greater(WorkplaceEvidenceMinutes(mainOwner, wMain), 0, "mainOwner 必须在 main 留下真实劳动证据");
            Assert.Greater(WorkplaceEvidenceMinutes(lateJoiner, wMain), 0, "lateJoiner 必须在 main 留下真实劳动证据");
            Assert.Greater(WorkplaceEvidenceMinutes(auxOwner, wAux), 0, "auxOwner 必须在 aux 留下真实劳动证据");
            Assert.AreEqual(0, WorkplaceEvidenceMinutes(mainOwner, wAux), "main 的劳动不得写成 aux 证据");
            Assert.AreEqual(0, WorkplaceEvidenceMinutes(auxOwner, wMain), "aux 的劳动不得写成 main 证据");
            Assert.AreEqual(wMain, PersonWorkplace(mainOwner), "mainOwner 职责必须仍是 main");
            Assert.AreEqual(wMain, PersonWorkplace(lateJoiner), "lateJoiner 职责必须仍是 main");
            Assert.AreEqual(wAux, PersonWorkplace(auxOwner), "auxOwner 职责必须仍是 aux");
            Evidence("CASE-E", "LABOR main.bound=2 labor=2 onsite=2 aux.bound=1 labor=1 onsite=1"
                + " evidence mainOwner/main=" + WorkplaceEvidenceMinutes(mainOwner, wMain)
                + " lateJoiner/main=" + WorkplaceEvidenceMinutes(lateJoiner, wMain)
                + " auxOwner/aux=" + WorkplaceEvidenceMinutes(auxOwner, wAux)
                + " cross=0 clock=" + ClockAbsoluteMinute());

            // ---------------- 7. 显式 Save（真实按钮 + bool 回执）后记录全部待比较事实 ----------------
            savedClock = ClockAbsoluteMinute();
            savedIngredients = Ingredients();
            savedFood = Food();
            savedBatches = ProductionBatchCount();
            savedEvidence = ProductionEvidenceCount();
            savedCompleted = AssignmentContributionHistory();
            string savePath = CurrentSavePath();
            Assert.IsFalse(string.IsNullOrEmpty(savePath), "SaveSystem.GetSavePath 必须返回真实单槽路径");
            string hashBeforeSave = File.Exists(savePath) ? HashFile(savePath) : "<absent>";
            var resourceBefore = Get(Get(management, "CurrentSnapshot"), "Resources");
            Assert.AreEqual(savedIngredients, Convert.ToInt32(Get(resourceBefore, "Ingredients")),
                "管理资源投影必须等于真实 Ingredients Authority");
            Assert.AreEqual(savedFood, Convert.ToInt32(Get(resourceBefore, "Food")),
                "管理资源投影必须等于真实 Food Authority");

            ClickButtonField(management, "saveButton", "显式保存·双槽");
            yield return null;
            AssertStatusContains(management, "已保存当前世界。");
            Assert.IsTrue((bool)Call(management, "Save"),
                "ManagementView.Save 走真实 SaveSystem bool 回执，成功时必须返回 true");
            Assert.IsTrue((bool)Static("SaveSystem", "HasSave"), "成功保存后单槽必须可被 Continue 检测");
            string hashAfterSave = HashFile(savePath);
            Assert.IsFalse(string.IsNullOrEmpty(hashAfterSave), "成功保存后必须存在真实单槽字节");
            string saveJson = ReadSaveJson();
            StringAssert.Contains("\"SaveVersion\": 1", saveJson, "真实单槽必须是当前 SaveVersion 的可加载 JSON");
            StringAssert.Contains("\"Ingredients\":", saveJson, "真实单槽必须写入资源事实");
            Assert.IsFalse(saveJson.Contains("SelectedWorkplaceId") || saveJson.Contains("SelectedConstructionBuildingId"),
                "UI 岗位 / 施工候选不得进入存档");
            Evidence("CASE-E", "SAVE clock=" + savedClock + " ingredients=" + savedIngredients
                + " food=" + savedFood + " batches=" + savedBatches + " evidence=" + savedEvidence
                + " completed=" + savedCompleted + " hashBefore=" + hashBeforeSave + " hashAfter=" + hashAfterSave);

            // ---------------- 8. 真实 Continue → Load 换源 → 重新进入管理页逐项连续 ----------------
            int oldHostInstanceId = ((Component)_host).gameObject.GetInstanceID();
            SceneManager.LoadScene("SampleScene");
            yield return null;
            yield return null;
            object menu = Find("MainMenuUI");
            Assert.IsNotNull(menu, "MainMenuUI 必须由真实场景重建");
            Call(menu, "RefreshContinueButton");
            var continueButton = (Button)Get(menu, "btnContinue");
            Assert.IsNotNull(continueButton, "真实 Continue 按钮必须绑定");
            Assert.IsTrue(continueButton.interactable, "真实单槽存在时 Continue 必须可用");
            continueButton.onClick.Invoke(); // 真实产品 Button 事件：OnContinue → SaveSystem.Load → EnterTown
            yield return null;
            yield return null;
            for (int i = 0; i < 10 && FindOptional("ProductHomeWorldHost") == null; i++) yield return null;

            _town = Find("TownUI");
            Assert.IsNotNull(_town, "Continue 后 TownUI 必须由产品入口进入");
            _host = Find("ProductHomeWorldHost");
            Assert.IsNotNull(_host, "Continue 后必须重建 ProductHomeWorldHost");
            _shell = Get(_host, "Shell");
            Assert.IsNotNull(_shell, "Continue 后必须重建 HomeProductShell");
            Call(_host, "RefreshNow");
            Assert.AreNotEqual(oldHostInstanceId, ((Component)_host).gameObject.GetInstanceID(),
                "Continue 必须重获新 host 引用，而不是复用旧实例");
            object loadedSource = StaticField("VillageRuntimeBridge", "SimulationPersonSource");
            Assert.IsTrue(ReferenceEquals(loadedSource, Get(_town, "PlayerData")),
                "来源必须同步为真实 Load 出的人物源（TownUI.PlayerData）");
            Assert.AreEqual("Paused", ClockSpeed(), "Load 必须恢复存档中的 Paused");
            Assert.AreEqual(savedClock, ClockAbsoluteMinute(), "Load 自身不得推进 Clock");

            OpenManagementThroughNavigation("管理·双槽 Continue 后");
            yield return null;
            management = Management();
            Assert.IsNotNull(management, "Continue 后必须重获真实 ManagementView");
            Assert.IsNull(Get(management, "SelectedWorkplaceId"), "UI 岗位候选不得入档并在 Continue 后恢复");
            Assert.IsNull(Get(management, "SelectedConstructionBuildingId"), "UI 施工候选不得入档并在 Continue 后恢复");
            ShowWorkplacesPage(management);
            yield return null;

            Assert.AreEqual(wMain, WorkplaceIdForSlot(management, farm, "main", "Continue·main"),
                "main 槽位身份（实例 + slot）必须逐项连续");
            Assert.AreEqual(wAux, WorkplaceIdForSlot(management, farm, "aux", "Continue·aux"),
                "aux 槽位身份（实例 + slot）必须逐项连续");
            mainRow = WorkplaceRowFor(management, farm, "main", "Continue·main");
            auxRow = WorkplaceRowFor(management, farm, "aux", "Continue·aux");
            Assert.AreEqual(farm, (string)Get(mainRow, "BuildingInstanceId"), "main 行必须仍指向同一建筑实例");
            Assert.AreEqual(farm, (string)Get(auxRow, "BuildingInstanceId"), "aux 行必须仍指向同一建筑实例");
            Assert.AreEqual("main", (string)Get(mainRow, "FunctionSlotId"), "main 槽位身份必须连续");
            Assert.AreEqual("aux", (string)Get(auxRow, "FunctionSlotId"), "aux 槽位身份必须连续");
            Assert.AreEqual(2, Convert.ToInt32(Get(mainRow, "Capacity")), "main 容量必须连续");
            Assert.AreEqual(1, Convert.ToInt32(Get(auxRow, "Capacity")), "aux 容量必须连续");
            Assert.IsTrue((bool)Get(mainRow, "IsRegistered") && (bool)Get(auxRow, "IsRegistered"),
                "Load 必须按真实 FunctionBindings 重新注册两个槽位");
            CollectionAssert.AreEquivalent(new[] { mainOwner, lateJoiner }, Strings(Get(mainRow, "BoundPersonIds")),
                "main 绑定人物必须逐项连续（不得串到 aux）");
            CollectionAssert.AreEquivalent(new[] { auxOwner }, Strings(Get(auxRow, "BoundPersonIds")),
                "aux 绑定人物必须逐项连续（不得串到 main）");
            Assert.AreEqual(wMain, PersonWorkplace(mainOwner), "mainOwner 职责必须连续");
            Assert.AreEqual(wMain, PersonWorkplace(lateJoiner), "lateJoiner 职责必须连续");
            Assert.AreEqual(wAux, PersonWorkplace(auxOwner), "auxOwner 职责必须连续");
            Assert.IsNull(PersonWorkplace(workerB), "被满员拒绝的 workerB 不得在 Load 后凭空获得职责");
            Assert.AreEqual(2, WorkplaceOccupants(wMain), "main 名额占用必须连续");
            Assert.AreEqual(1, WorkplaceOccupants(wAux), "aux 名额占用必须连续");
            Assert.AreEqual("Completed", BuildingState(farm), "建筑实例必须在 Load 后仍为 Completed");

            Assert.AreEqual(savedIngredients, Ingredients(), "Continue 后 Ingredients 不得重复结算/增减");
            Assert.AreEqual(savedFood, Food(), "Continue 后 Food 不得重复结算/增减");
            Assert.AreEqual(savedBatches, ProductionBatchCount(), "Continue 不得重复生产批次");
            Assert.AreEqual(savedEvidence, ProductionEvidenceCount(), "Continue 不得重复生产证据");
            Assert.AreEqual(savedCompleted, AssignmentContributionHistory(), "Continue 不得重复结算执行历史");
            Assert.AreEqual(savedClock, ClockAbsoluteMinute(), "Continue 后 Clock 必须与存档一致");
            var resourceAfter = Get(Get(management, "CurrentSnapshot"), "Resources");
            Assert.AreEqual(savedIngredients, Convert.ToInt32(Get(resourceAfter, "Ingredients")),
                "Continue 后管理资源投影必须等于真实 Authority");
            Assert.AreEqual(savedFood, Convert.ToInt32(Get(resourceAfter, "Food")),
                "Continue 后管理资源投影必须等于真实 Authority");
            Call(management, "RefreshNow");
            yield return null;
            Assert.AreEqual(savedIngredients, Ingredients(), "Continue 后刷新管理不得再增减 Ingredients");
            Assert.AreEqual(savedFood, Food(), "Continue 后刷新管理不得再增减 Food");
            Assert.AreEqual(savedBatches, ProductionBatchCount(), "Continue 后刷新管理不得再产生批次");
            Evidence("CASE-E", "CONTINUE main=" + wMain + " aux=" + wAux
                + " bound main=[" + mainOwner + "," + lateJoiner + "] aux=[" + auxOwner + "]"
                + " clock=" + ClockAbsoluteMinute() + " resources=" + Ingredients() + "/" + Food()
                + " batches=" + ProductionBatchCount() + " evidence=" + ProductionEvidenceCount()
                + " completed=" + AssignmentContributionHistory());

            // ---------------- 9. 建造态 ESC：公开入口 + 公开取消的回调 / 取消顺序机制 ----------------
            object build = Get(_shell, "Build");
            Assert.IsNotNull(build, "Shell.Build 必须是真实建造面板");
            Assert.IsFalse((bool)Get(build, "IsOpen"), "前置：建造面板必须关闭");
            Assert.AreEqual("Management", Surface(), "前置：当前必须停在管理表面（用于对照关闭分支）");
            Call(build, "OpenCatalog");
            Assert.IsTrue((bool)Get(build, "IsOpen"), "公开建造入口必须打开建造候选面板");
            Assert.IsTrue((bool)Call(build, "Choose", "building_farm"), "公开 Choose 必须能选中真实建筑定义");
            Assert.IsTrue((bool)Get(build, "GhostVisible"), "选中建造候选后必须显示 ghost 预览");
            // 真实产品顺序：OpenCatalog 自身复用 Shell.CloseSurface 收起管理表面（Surface=World）。
            // 因此这里能证明的是“Build.IsOpen 时 HandleEscape 不进入 CloseSurface 分支”，
            // 不是“某个非 World 表面被保留”（公开入口下无法构造 Build.IsOpen 且 Surface≠World）。
            Assert.AreEqual("World", Surface(), "公开建造入口自身会收起管理表面（产品既有顺序）");
            Assert.IsFalse(((Component)Management()).gameObject.activeSelf,
                "建造打开后管理页必须保持未激活（不得被 ESC 回调重新激活）");

            Call(_shell, "HandleEscape");
            Assert.IsTrue((bool)Get(build, "IsOpen"), "Build.IsOpen 时 Shell.HandleEscape 不得关闭建造面板");
            Assert.AreEqual("building_farm", (string)Get(build, "SelectedDefinitionId"),
                "Build.IsOpen 时 HandleEscape 不得清掉建造候选（取消只走 Build.Cancel）");
            Assert.AreEqual("World", Surface(), "Build.IsOpen 时 HandleEscape 不得进入 CloseSurface 分支");
            Assert.IsFalse(((Component)Management()).gameObject.activeSelf,
                "Build.IsOpen 时 HandleEscape 不得把管理表面重新打开");
            Evidence("CASE-E", "ESC-BUILD buildOpen=True candidate=building_farm ghost=True"
                + " surface=World managementActive=False (回调先判 Build.IsOpen)");

            Call(build, "Cancel"); // 公开取消入口（非反射写私有字段）
            Assert.IsFalse((bool)Get(build, "IsOpen"), "公开 Build.Cancel 必须关闭建造面板");
            Assert.IsNull((string)Get(build, "SelectedDefinitionId"), "Build.Cancel 必须清除建造候选");
            Assert.IsFalse((bool)Get(build, "GhostVisible"), "Build.Cancel 必须隐藏 ghost 预览");

            // 建造关闭后：重新用真实导航打开管理表面，HandleEscape 才必须关闭“当前 Surface”。
            OpenManagementThroughNavigation("管理·建造关闭后的 ESC 对照");
            yield return null;
            Assert.AreEqual("Management", Surface(), "真实导航必须重新打开管理表面");
            Call(_shell, "HandleEscape");
            yield return null;
            Assert.AreEqual("World", Surface(), "建造关闭时 HandleEscape 必须关闭当前管理表面");
            Assert.IsFalse(((Component)Management()).gameObject.activeSelf, "ESC 关闭后管理页必须不再激活");
            Assert.IsFalse((bool)Get(build, "IsOpen"), "ESC 关闭表面不得反向打开建造面板");
            Evidence("CASE-E", "ESC-CLOSE buildOpen=False surface=World managementActive=False"
                + " (公开 Cancel 后 HandleEscape 才关闭当前 Surface)");
        }
        finally
        {
            // 只恢复本次覆盖；不 Reset 生产 Root、不触碰存档 / 字体 / 34 项基线链。
            // SaveSlotGuard 恢复与 ResetForTests 仍由既有 TearDown 的 try/finally 链负责，本处不短路。
            SetBuildingsOverride(previousOverride);
        }

        // ---------------- 10. finally 恢复核对（只读） ----------------
        Assert.IsNull(BuildingsOverride(), "finally 必须清除本次 Editor-only 内存覆盖");
        string restoredShape = DescribeRegistry(TryGetBuildingsRegistry(out string restoredDiagnostic));
        Assert.AreEqual(baselineShape, restoredShape,
            "恢复覆盖后默认建筑注册表内容必须与开工完全相同：" + restoredDiagnostic);
        Assert.IsFalse(restoredShape.Contains("building_farm:aux:Workplace"),
            "默认建筑注册表不得残留 aux 测试绑定");
        Evidence("CASE-E", "END override=<null> defaultRegistryRestored=True farm=" + farm
            + " main=" + wMain + " aux=" + wAux + " clock=" + savedClock
            + " resources=" + savedIngredients + "/" + savedFood);
    }

    // ==================== 用例 16：ESC 逐帧采样 / 真实 LateUpdate 路由 / 建造门禁 ====================

    /// <summary>
    /// 原 R3-B 最后缺口：此前所有 ESC 证据都直接调用公开回调 HandleEscape，绕过了产品真实的
    /// LateUpdate 逐帧键盘轮询，因此“public 回调”不等于“LateUpdate 实际采样路由”。
    /// 本用例沿用 HomeWorldCamera 的 Editor-only internal 实例级输入采样先例，只替换 Shell 的
    /// ESC 采样数据（一次性“一帧按下”，消费后立即回落生产 Input.GetKeyDown）：
    ///   1) 管理模态：注入一次 press → 真实帧执行 Shell.LateUpdate（`if (SampleEscapePressed()) HandleEscape()`）
    ///      → 只关闭当前管理 Surface，Selection / Clock / Speed / IsSimulationActive 不漂移，
    ///      TownUI.canvasRoot CanvasGroup 恢复到管理打开前的取值；关闭后的下一帧不得重复关闭；
    ///   2) 一次消费不残留 + 阴性采样：重开管理后不注入，连续真实帧必须保持管理（不会把上次 press 重放）；
    ///   3) Person 模态：真实底部导航打开，注入一次 press 由真实 LateUpdate 只关闭当前 Person Surface；
    ///   4) 建造门禁：公开 OpenCatalog + 合法候选后先跑“无 ESC 真实帧”，让候选按产品真实路径
    ///      （Host.Update → Build.ReadInput(overUI) → UpdatePointer(Input.mousePosition)）跟随指针稳定，
    ///      再以同指针的无 ESC 帧作阴性对照，随后才注入 press；真实 LateUpdate 走同一 HandleEscape，
    ///      Build.IsOpen 分支不得关闭 Surface / 清候选 / 清 ghost / 放置建筑 / 写世界。
    ///      候选位置断言对齐“指针正常重算”，不把手动 SetCandidate 的值冻结成期望；建造自身的
    ///      物理 ESC 轮询（HomeBuildingPlacement.ReadInput）本用例不注入，本阶段只证明 Shell 路由；
    ///   5) 公开 Build.Cancel 后重开管理，再注入一次 press 才能正常关闭当前 Surface；
    ///   6) finally 只清除本实例注入标记，不短路 SaveSlotGuard / ResetForTests / 活动日程恢复链。
    ///
    /// 边界（不冒称）：注入的是合成“按下”采样，不是 OS 物理 ESC，也不证明 Unity Input.GetKeyDown
    /// 的引擎实现；它证明的是“与生产同一条 LateUpdate 采样 / 路由 / 门禁路径被执行”。
    /// </summary>
    [UnityTest]
    public IEnumerator ManagementEscape_FramePolling_ClosesOnlyCurrentSurfaceAndKeepsBuildGate()
    {
        Assert.AreEqual(1, UnityEngine.Object.FindObjectsOfType(T("ProductHomeWorldHost"), true).Length,
            "真实产品世界宿主必须单例");
        var shellType = _shell.GetType();
        var pressEntry = ExactMethod(shellType, "SetEscapePressForTests", Flags,
            "Editor 编译下必须存在 internal SetEscapePressForTests（一次性 ESC 采样缝）", Type.EmptyTypes);
        Assert.IsTrue(pressEntry.IsAssembly, "ESC 采样缝必须是 internal（不得成为 public API）");
        Assert.AreEqual(0, pressEntry.GetParameters().Length, "ESC 采样缝只允许注入无参的“一帧按下”");
        Assert.IsNull(MethodOrNull(shellType, "SetEscapePressForTests",
                BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes),
            "ESC 采样缝不得暴露为 public");
        var clearEntry = ExactMethod(shellType, "ClearEscapePressForTests", Flags,
            "Editor 编译下必须存在 internal ClearEscapePressForTests", Type.EmptyTypes);
        Assert.IsTrue(clearEntry.IsAssembly, "ESC 注入清理入口必须同为 internal");
        var sampleEntry = ExactMethod(shellType, "SampleEscapePressed", Flags,
            "私有 SampleEscapePressed 必须存在（真实 LateUpdate 的唯一 ESC 采样入口）", Type.EmptyTypes);
        Assert.IsTrue(sampleEntry.IsPrivate, "SampleEscapePressed 必须是 private（不得成为公共 / 测试 API）");
        Assert.AreEqual(typeof(bool), sampleEntry.ReturnType, "SampleEscapePressed 必须返回 bool");
        var lateUpdate = ExactMethod(shellType, "LateUpdate", Flags,
            "Shell.LateUpdate 必须存在（真实帧轮询入口）", Type.EmptyTypes);
        Assert.IsTrue(lateUpdate.IsPrivate, "Shell.LateUpdate 必须保持 Unity 消息方法的私有形态（不是 Update）");
        var handleEscape = ExactMethod(shellType, "HandleEscape", BindingFlags.Public | BindingFlags.Instance,
            "既有 public HandleEscape 必须保留", Type.EmptyTypes);
        Assert.AreEqual(typeof(void), handleEscape.ReturnType, "HandleEscape 公共签名不得改变（void）");
        Assert.AreEqual(0, handleEscape.GetParameters().Length, "HandleEscape 公共签名不得改变（无参）");

        var townRoot = (GameObject)Get(_town, "canvasRoot");
        var townGroupBefore = townRoot.GetComponent<CanvasGroup>();
        bool interactableBefore = townGroupBefore == null || townGroupBefore.interactable;
        bool blocksBefore = townGroupBefore == null || townGroupBefore.blocksRaycasts;
        string selected = FirstSnapshotPersonIdFromHost();
        Call(Get(_host, "Selection"), "SelectPerson", selected);
        long clockBefore = ClockAbsoluteMinute();
        string speedBefore = ClockSpeed();
        double multiplierBefore = ClockMultiplier();
        bool simulationBefore = SimulationActive();
        object build = Get(_shell, "Build");
        Assert.IsNotNull(build, "Shell.Build 必须是真实建造面板");
        Assert.IsFalse((bool)Get(build, "IsOpen"), "前置：建造面板必须关闭");
        Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "前置：World 必须非模态");
        Evidence("ESC-FRAME", "PRE selected=" + selected + " clock=" + clockBefore + " speed=" + speedBefore
            + " multiplier=" + multiplierBefore + " townInteractable=" + interactableBefore
            + " townBlocks=" + blocksBefore + " buildOpen=False");
        try
        {
            // ---------- 1. 管理模态：一次注入 → 真实帧 LateUpdate 关闭 ----------
            OpenManagementThroughNavigation("ESC 帧轮询·管理");
            yield return null;
            yield return null;
            Assert.AreEqual("Management", Surface(), "管理模态必须由真实导航打开");
            Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "管理打开时 SurfaceModalActive 必须为真");
            Assert.IsTrue((bool)Get(_shell, "IsPersonModalOpen"), "既有 IsPersonModalOpen 别名必须同步为真");
            var townGroup = townRoot.GetComponent<CanvasGroup>();
            Assert.IsNotNull(townGroup, "管理模态必须通过既有 TownUI.canvasRoot CanvasGroup 阻断旧 Town");
            Assert.IsFalse(townGroup.interactable, "管理打开时旧 Town interactable 必须禁用");
            Assert.IsFalse(townGroup.blocksRaycasts, "管理打开时旧 Town 射线必须禁用");

            Call(_shell, "SetEscapePressForTests");
            yield return null; // 注入后的真实帧：Shell.LateUpdate 采样并消费一次
            yield return null; // 再跨一帧，确保该帧生命周期完整结束
            Assert.AreEqual("World", Surface(), "真实 LateUpdate 采样到一次 ESC 后必须只关闭当前管理模态");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "ESC 关闭后 SurfaceModalActive 必须为假");
            Assert.IsFalse((bool)Get(_shell, "IsPersonModalOpen"), "ESC 关闭后 IsPersonModalOpen 别名必须为假");
            Assert.IsFalse(((Component)Management()).gameObject.activeSelf, "ESC 关闭后管理页必须不再激活");
            Assert.IsFalse((bool)Get(Management(), "IsVisible"), "ESC 关闭后管理页必须不可见");
            Assert.AreEqual(selected, SelectionId(), "ESC 关闭管理不得改变 Selection");
            Assert.AreEqual(clockBefore, ClockAbsoluteMinute(), "ESC 关闭管理不得推进 / 漂移 Clock");
            Assert.AreEqual(speedBefore, ClockSpeed(), "ESC 关闭管理不得改变 Clock Speed");
            Assert.AreEqual(multiplierBefore, ClockMultiplier(), "ESC 关闭管理不得改变 Clock SpeedMultiplier");
            Assert.AreEqual(simulationBefore, SimulationActive(), "ESC 关闭管理不得改写 IsSimulationActive");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "ESC 关闭后旧 Town interactable 必须恢复原值");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "ESC 关闭后旧 Town blocksRaycasts 必须恢复原值");
            Assert.IsFalse((bool)Get(build, "IsOpen"), "ESC 关闭管理不得打开建造");
            Evidence("ESC-FRAME", "MANAGEMENT-CLOSED surface=" + Surface() + " selection=" + SelectionId()
                + " clock=" + ClockAbsoluteMinute() + " speed=" + ClockSpeed()
                + " townInteractable=" + townGroup.interactable + " townBlocks=" + townGroup.blocksRaycasts);

            // 关闭后的下一帧：不得出现第二次关闭 / 状态回弹。
            yield return null;
            Assert.AreEqual("World", Surface(), "关闭后的下一帧不得重复关闭或改写 Surface");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "关闭后的下一帧不得重新进入模态");
            Assert.AreEqual(selected, SelectionId(), "关闭后的下一帧不得改变 Selection");
            Assert.AreEqual(clockBefore, ClockAbsoluteMinute(), "关闭后的下一帧 Clock 不得漂移");

            // ---------- 2. 一次消费不残留 + 阴性采样（无注入的真实帧保持管理） ----------
            OpenManagementThroughNavigation("ESC 帧轮询·重开管理");
            yield return null;
            yield return null;
            Assert.AreEqual("Management", Surface(), "关闭后必须能用真实导航重新打开管理");
            Assert.IsFalse(townGroup.interactable, "重开管理必须再次阻断旧 Town interactable");
            Assert.IsFalse(townGroup.blocksRaycasts, "重开管理必须再次阻断旧 Town 射线");
            // 这里不注入任何 press：生产 Input.GetKeyDown 在测试环境没有物理 ESC，属于阴性输入。
            for (int i = 0; i < 3; i++) yield return null;
            Assert.AreEqual("Management", Surface(), "没有 ESC 采样的真实帧必须保持管理模态（阴性输入 false）");
            Assert.IsTrue((bool)Get(_shell, "SurfaceModalActive"), "阴性帧内管理模态必须保持");
            Assert.IsFalse(townGroup.interactable, "阴性帧内旧 Town interactable 必须持续禁用");
            Assert.IsFalse(townGroup.blocksRaycasts, "阴性帧内旧 Town 射线必须持续禁用");
            Evidence("ESC-FRAME", "NEGATIVE-FRAMES=3 surface=Management modal=True (no injection; press not replayed)");

            // 第三次注入仍必须只消费一次并正常关闭，证明前面的重开没有被残留注入误关。
            Call(_shell, "SetEscapePressForTests");
            yield return null;
            yield return null;
            Assert.AreEqual("World", Surface(), "重开的管理模态必须被新注入的一次真实帧 ESC 关闭");
            Assert.AreEqual(selected, SelectionId(), "再次关闭管理不得改变 Selection");
            Assert.AreEqual(clockBefore, ClockAbsoluteMinute(), "再次关闭管理不得推进 Clock");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "再次关闭后旧 Town interactable 必须恢复");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "再次关闭后旧 Town blocksRaycasts 必须恢复");

            // ---------- 3. Person 模态：真实导航打开，真实帧 ESC 只关闭当前 Surface ----------
            var navigate1 = FindPath(((Component)_shell).transform, "ProductNavigation/Navigate1");
            Assert.IsNotNull(navigate1, "底部导航“人物”必须存在");
            var navigate1Button = navigate1.GetComponent<Button>();
            Assert.IsNotNull(navigate1Button, "底部导航“人物”必须是 Button");
            Assert.IsTrue(navigate1Button.interactable, "World 状态人物导航必须可点");
            RealPointerClick((RectTransform)navigate1, "底部导航·人物（ESC 帧轮询）");
            yield return null;
            yield return null;
            Assert.AreEqual("Person", Surface(), "真实导航必须进入 Person 模态");
            object detail = Get(_shell, "PersonDetail");
            Assert.IsNotNull(detail, "既有 PersonDetail 必须存在");
            Assert.IsTrue((bool)Get(detail, "IsVisible"), "Person 模态下 PersonDetail 必须可见");
            Assert.IsFalse(townGroup.interactable, "Person 模态旧 Town interactable 必须禁用");
            Assert.IsFalse(townGroup.blocksRaycasts, "Person 模态旧 Town 射线必须禁用");
            long personClock = ClockAbsoluteMinute();
            string personSpeed = ClockSpeed();

            Call(_shell, "SetEscapePressForTests");
            yield return null;
            yield return null;
            Assert.AreEqual("World", Surface(), "真实 LateUpdate 采样也必须只关闭当前 Person 模态");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "Person 关闭后 SurfaceModalActive 必须为假");
            Assert.IsFalse((bool)Get(detail, "IsVisible"), "Person 关闭后 PersonDetail 必须不可见");
            Assert.AreEqual(selected, SelectionId(), "Person 关闭不得改变 Selection");
            Assert.AreEqual(personClock, ClockAbsoluteMinute(), "Person 关闭不得推进 / 漂移 Clock");
            Assert.AreEqual(personSpeed, ClockSpeed(), "Person 关闭不得改变 Clock Speed");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "Person 关闭后旧 Town interactable 必须恢复");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "Person 关闭后旧 Town blocksRaycasts 必须恢复");
            Evidence("ESC-FRAME", "PERSON-CLOSED surface=" + Surface() + " selection=" + SelectionId()
                + " clock=" + ClockAbsoluteMinute() + " townInteractable=" + townGroup.interactable);

            // ---------- 4. 建造门禁：先让候选按真实指针稳定，再用无 ESC 阴性对照验证 ESC 只让路 ----------
            // 产品真实帧路径（本用例不绕过、不禁用）：
            //   ProductHomeWorldHost.Update → if(Build.IsOpen) Build.ReadInput(overUI)
            //   → SelectedDefinitionId!=null && !overUI → UpdatePointer(Input.mousePosition) → SetCandidate(...)
            // 因此合法候选在真实帧里会被“正常指针更新”按当前真实鼠标重算；夹具不冻结候选、
            // 不注入指针、不改 Host/Build 更新，而是：先跑无 ESC 真实帧让候选跟随指针稳定，
            // 捕获同帧指针 / 候选，再以“无 ESC 真实帧”为阴性对照，最后注入一帧 ESC。
            // 说明：HomeBuildingPlacement.ReadInput 自己的物理 ESC / 右键轮询本用例不注入；
            // 本阶段只证明 Shell 的 LateUpdate 采样路由在 Build.IsOpen 时必须让路。
            Call(build, "OpenCatalog");
            Assert.IsTrue((bool)Get(build, "IsOpen"), "公开建造入口必须打开建造候选面板");
            Assert.IsTrue((bool)Call(build, "Choose", "building_farm"), "公开 Choose 必须能选中真实建筑定义");
            Assert.IsTrue((bool)Get(build, "GhostVisible"), "选中建造候选后必须显示 ghost 预览");
            // 正式默认世界已有实例建筑（Farm/Kitchen 在 y=-1.2、住房在 y=4.2）；这里选一个
            // 建造范围内且不与默认实例重叠的公开候选点（y=-6，Farm 占地 3×2），不再假设“空世界”。
            // 该断言只描述“注入 ESC 前的起始夹具”；真实帧会按产品既有指针路径重算候选，
            // 本用例不再要求候选固定在手动值（否则等于强锁候选求绿）。
            Vector2 manualCandidate = new Vector2(0f, -6f);
            Call(build, "SetCandidate", manualCandidate);
            Assert.IsTrue((bool)Get(build, "PreviewValid"), "夹具必须在真实帧前建立合法建造候选（在范围内、无重叠）");
            Assert.AreEqual(manualCandidate, (Vector2)Get(build, "Candidate"),
                "公开 SetCandidate 必须在下一真实帧前生效（起始候选）");
            string buildSurface = Surface();
            Assert.AreEqual("World", buildSurface, "公开建造入口自身收起管理表面（产品既有顺序）");
            string buildDefinition = (string)Get(build, "SelectedDefinitionId");
            Assert.AreEqual("building_farm", buildDefinition, "建造候选定义必须保持真实选择");
            Assert.IsFalse(((Component)Management()).gameObject.activeSelf, "建造打开后管理页必须保持未激活");
            long buildClockBefore = ClockAbsoluteMinute();
            string buildSpeedBefore = ClockSpeed();
            double buildMultiplierBefore = ClockMultiplier();
            bool buildSimulationBefore = SimulationActive();
            int worldBuildingsBefore = WorldBuildingCount();

            // 无 ESC 真实帧（阴性对照）：候选必须跟随当前真实指针按同一产品换算重算并稳定。
            yield return null;
            yield return null;
            Vector3 pointerNegative = Input.mousePosition;
            bool overUiNegative = (bool)Call(_host, "PointerOverUI", (Vector2)pointerNegative);
            string pointerHits = overUiNegative ? DumpHits(RaycastAllAt(pointerNegative)) : "（无 UI 命中）";
            Evidence("ESC-FRAME", "BUILD-POINTER-NEGATIVE pointer=" + pointerNegative + " overUI=" + overUiNegative
                + " manual=" + manualCandidate + " candidate=" + (Vector2)Get(build, "Candidate")
                + " pointerDerived=" + PointerDerivedBuildCandidate(pointerNegative)
                + " hits=" + pointerHits.Replace("\n", " | "));
            Assert.IsFalse(overUiNegative,
                "环境前置：建造打开且未注入 ESC 时，真实指针必须不在 UI 上，否则 Build.ReadInput 会在"
                + " UpdatePointer 之前返回，无法证明“候选跟随指针”。当前指针=" + pointerNegative + "\n" + pointerHits);
            Vector2 candidateNegative = (Vector2)Get(build, "Candidate");
            Vector2 pointerDerivedNegative = PointerDerivedBuildCandidate(pointerNegative);
            Assert.AreEqual(pointerDerivedNegative.x, candidateNegative.x, 0.001f,
                "无 ESC 真实帧的候选 X 必须等于 UpdatePointer(真实指针) 的正常换算（证明候选变化来自指针更新）");
            Assert.AreEqual(pointerDerivedNegative.y, candidateNegative.y, 0.001f,
                "无 ESC 真实帧的候选 Y 必须等于 UpdatePointer(真实指针) 的正常换算（证明候选变化来自指针更新）");
            Assert.AreNotEqual(manualCandidate, pointerDerivedNegative,
                "环境前置：真实指针换算必须与手动候选不同，才能区分“候选被真实帧重算”与“候选未被更新”。"
                + "当前指针=" + pointerNegative + " 换算=" + pointerDerivedNegative);
            Assert.IsTrue((bool)Get(build, "IsOpen"), "无 ESC 真实帧不得关闭建造面板");
            Assert.AreEqual(buildDefinition, (string)Get(build, "SelectedDefinitionId"),
                "无 ESC 真实帧不得清掉建造候选定义");
            Assert.IsTrue((bool)Get(build, "GhostVisible"), "无 ESC 真实帧 ghost 必须保持可见");
            Assert.AreEqual(selected, SelectionId(), "无 ESC 真实帧不得改变 Selection");
            Assert.AreEqual(worldBuildingsBefore, WorldBuildingCount(), "无 ESC 真实帧不得放置建筑 / 写世界");
            Evidence("ESC-FRAME", "BUILD-NEGATIVE buildOpen=True definition=" + buildDefinition + " ghost=True surface="
                + Surface() + " pointer=" + pointerNegative + " overUI=False candidate=" + candidateNegative
                + " buildCount=" + worldBuildingsBefore + " selection=" + SelectionId() + " clock=" + ClockAbsoluteMinute());
            // 再跨一帧仍无 ESC：候选仍必须与同一产品换算一致（不得被冻结、残留注入或 ESC 路径改写）。
            yield return null;
            Vector2 pointerDerivedExtra = PointerDerivedBuildCandidate(Input.mousePosition);
            Vector2 candidateExtra = (Vector2)Get(build, "Candidate");
            Assert.AreEqual(pointerDerivedExtra.x, candidateExtra.x, 0.001f,
                "无 ESC 阴性帧的候选 X 必须持续与真实指针换算一致");
            Assert.AreEqual(pointerDerivedExtra.y, candidateExtra.y, 0.001f,
                "无 ESC 阴性帧的候选 Y 必须持续与真实指针换算一致");

            // 注入一帧 ESC：真实 LateUpdate 消费后，Build.IsOpen 分支只让路；候选仍只由指针路径决定。
            string cameraPoseNegative = CameraPose(Get(_host, "WorldCamera"));
            Call(_shell, "SetEscapePressForTests");
            yield return null;
            yield return null;
            Assert.IsFalse((bool)Get(_shell, "_escapePressPendingForTests"),
                "注入的一次性 ESC 必须已被真实 LateUpdate 采样消费（证明真实帧路由确实执行，而不是没跑到）");
            Vector3 pointerAfterEsc = Input.mousePosition;
            bool overUiAfterEsc = (bool)Call(_host, "PointerOverUI", (Vector2)pointerAfterEsc);
            Vector2 candidateAfterEsc = (Vector2)Get(build, "Candidate");
            Vector2 pointerDerivedAfterEsc = PointerDerivedBuildCandidate(pointerAfterEsc);
            bool pointerStable = pointerAfterEsc == pointerNegative;
            string cameraPoseAfterEsc = CameraPose(Get(_host, "WorldCamera"));
            bool cameraStable = cameraPoseAfterEsc == cameraPoseNegative;
            if (!overUiAfterEsc)
            {
                Assert.AreEqual(pointerDerivedAfterEsc.x, candidateAfterEsc.x, 0.001f,
                    "Build.IsOpen 时 Shell 采样不得改写建造候选位置：真实帧候选必须仍等于 UpdatePointer(真实指针) 的换算");
                Assert.AreEqual(pointerDerivedAfterEsc.y, candidateAfterEsc.y, 0.001f,
                    "Build.IsOpen 时 Shell 采样不得改写建造候选位置：真实帧候选必须仍等于 UpdatePointer(真实指针) 的换算");
            }
            else
            {
                Assert.AreEqual(candidateNegative.x, candidateAfterEsc.x, 0.001f,
                    "指针落入 UI（ReadInput 提前返回）时 Shell 采样也不得改写候选 X");
                Assert.AreEqual(candidateNegative.y, candidateAfterEsc.y, 0.001f,
                    "指针落入 UI（ReadInput 提前返回）时 Shell 采样也不得改写候选 Y");
            }
            if (pointerStable && cameraStable && !overUiAfterEsc)
            {
                Assert.AreEqual(candidateNegative.x, candidateAfterEsc.x, 0.001f,
                    "同一真实指针 / 同一相机下，注入 ESC 的真实帧与无 ESC 阴性帧的候选 X 必须完全一致（ESC 不参与候选）");
                Assert.AreEqual(candidateNegative.y, candidateAfterEsc.y, 0.001f,
                    "同一真实指针 / 同一相机下，注入 ESC 的真实帧与无 ESC 阴性帧的候选 Y 必须完全一致（ESC 不参与候选）");
            }
            Assert.IsTrue((bool)Get(build, "IsOpen"), "Shell 采样在 Build.IsOpen 时不得关闭建造面板");
            Assert.AreEqual(buildDefinition, (string)Get(build, "SelectedDefinitionId"),
                "Build.IsOpen 时 Shell 采样不得清掉建造候选（取消只走 Build.Cancel）");
            Assert.IsTrue((bool)Get(build, "GhostVisible"), "Build.IsOpen 时 ghost 必须保持可见");
            Assert.AreEqual("World", Surface(), "Build.IsOpen 时 Shell 采样不得进入 CloseSurface 分支");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "Build.IsOpen 时 Shell 采样不得重新进入模态");
            Assert.IsFalse(((Component)Management()).gameObject.activeSelf,
                "Build.IsOpen 时 Shell 采样不得重新激活管理页");
            Assert.AreEqual(selected, SelectionId(), "Build.IsOpen 时 Shell 采样不得改变 Selection");
            Assert.AreEqual(buildClockBefore, ClockAbsoluteMinute(), "Build.IsOpen 时 Shell 采样不得推进 Clock");
            Assert.AreEqual(buildSpeedBefore, ClockSpeed(), "Build.IsOpen 时 Shell 采样不得改变 Clock Speed");
            Assert.AreEqual(buildMultiplierBefore, ClockMultiplier(),
                "Build.IsOpen 时 Shell 采样不得改变 Clock SpeedMultiplier");
            Assert.AreEqual(buildSimulationBefore, SimulationActive(),
                "Build.IsOpen 时 Shell 采样不得改写 IsSimulationActive");
            Assert.AreEqual(worldBuildingsBefore, WorldBuildingCount(),
                "Build.IsOpen 时 Shell 采样不得放置建筑 / 写世界（真实建筑实例数必须不变）");
            Evidence("ESC-FRAME", "BUILD-GATE buildOpen=True definition=" + buildDefinition + " ghost=True surface="
                + Surface() + " managementActive=False candidate=" + candidateAfterEsc
                + " pointerDerived=" + pointerDerivedAfterEsc + " pointer=" + pointerAfterEsc
                + " pointerStable=" + pointerStable + " cameraStable=" + cameraStable
                + " camera=" + cameraPoseAfterEsc + " overUI=" + overUiAfterEsc
                + " buildCount=" + WorldBuildingCount() + " selection=" + SelectionId()
                + " clock=" + ClockAbsoluteMinute() + " speed=" + ClockSpeed());

            // ---------- 5. 公开 Cancel 后重开管理，再注入一次才正常关闭当前 Surface ----------
            Call(build, "Cancel");
            Assert.IsFalse((bool)Get(build, "IsOpen"), "公开 Build.Cancel 必须关闭建造面板");
            Assert.IsNull((string)Get(build, "SelectedDefinitionId"), "公开 Build.Cancel 必须清除建造候选");
            Assert.IsFalse((bool)Get(build, "GhostVisible"), "公开 Build.Cancel 必须隐藏 ghost");
            OpenManagementThroughNavigation("ESC 帧轮询·建造关闭后");
            yield return null;
            yield return null;
            Assert.AreEqual("Management", Surface(), "建造关闭后真实导航必须重新打开管理表面");
            Assert.IsFalse(townGroup.interactable, "建造关闭后重开管理必须再次阻断旧 Town");

            Call(_shell, "SetEscapePressForTests");
            yield return null;
            yield return null;
            Assert.AreEqual("World", Surface(), "公开 Cancel 后重开的管理模态必须被一次真实帧 ESC 正常关闭");
            Assert.IsFalse((bool)Get(_shell, "SurfaceModalActive"), "最后一次 ESC 后必须回到非模态 World");
            Assert.IsFalse((bool)Get(build, "IsOpen"), "ESC 关闭管理不得反向打开建造面板");
            Assert.AreEqual(selected, SelectionId(), "最后一次 ESC 不得改变 Selection");
            Assert.AreEqual(clockBefore, ClockAbsoluteMinute(), "最后一次 ESC 不得推进 Clock");
            Assert.AreEqual(speedBefore, ClockSpeed(), "最后一次 ESC 不得改变 Clock Speed");
            Assert.AreEqual(interactableBefore, townGroup.interactable, "最后一次 ESC 后旧 Town interactable 必须恢复");
            Assert.AreEqual(blocksBefore, townGroup.blocksRaycasts, "最后一次 ESC 后旧 Town blocksRaycasts 必须恢复");
            Evidence("ESC-FRAME", "END surface=" + Surface() + " selection=" + SelectionId()
                + " clock=" + ClockAbsoluteMinute() + " speed=" + ClockSpeed()
                + " townInteractable=" + townGroup.interactable + " townBlocks=" + townGroup.blocksRaycasts);
        }
        finally
        {
            // 只清本实例的一次性注入标记；SaveSlotGuard / ResetForTests / 活动日程恢复仍由既有链负责。
            Call(_shell, "ClearEscapePressForTests");
            Evidence("ESC-FRAME", "CLEANUP injection cleared; no OS physical ESC / Unity Input engine internals were simulated");
        }

        Assert.IsFalse((bool)Get(_shell, "_escapePressPendingForTests"), "finally 必须清除未消费的 ESC 注入标记");
        Assert.AreEqual(1, UnityEngine.Object.FindObjectsOfType(T("ProductHomeWorldHost"), true).Length,
            "用例结束宿主仍必须单例");
    }

    /// <summary>
    /// 只读：真实帧 Build.ReadInput(overUI=false) 的候选来源等价换算——与产品同一
    /// HomeWorldCamera.ViewCamera.ScreenToWorldPoint(pointer, z=10) + BuildingWorldMapper.ToLocal。
    /// 仅读指针 / 相机，不写候选、不写世界；用于证明候选变化来自正常 UpdatePointer，
    /// 而不是 Shell 的 ESC 采样 / HandleEscape 路由。
    /// </summary>
    private Vector2 PointerDerivedBuildCandidate(Vector3 pointer)
    {
        object camera = Get(_host, "WorldCamera");
        Assert.IsNotNull(camera, "Host.WorldCamera 必须存在");
        var view = (Camera)Get(camera, "ViewCamera");
        Assert.IsNotNull(view, "Host.WorldCamera.ViewCamera 必须存在");
        Vector3 world = view.ScreenToWorldPoint(new Vector3(pointer.x, pointer.y, 10f));
        return (Vector2)Static("BuildingWorldMapper", "ToLocal", world);
    }

    /// <summary>只读：Host 最新快照中的真实建筑实例数（断言 ESC 帧未放置建筑 / 未写世界）。</summary>
    private int WorldBuildingCount()
    {
        object snapshot = Get(_host, "Snapshot");
        Assert.IsNotNull(snapshot, "Host.Snapshot 必须已生成");
        return ((IEnumerable)Get(snapshot, "Buildings")).Cast<object>().Count();
    }

    /// <summary>
    /// 解析 StaticConfig.Runtime 中的配置类型（PlayMode 测试程序集不静态引用该 assembly；
    /// 与既有 Edit 用例使用同一真实类型，不复制定义）。
    /// </summary>
    private static Type StaticConfigType(string name)
    {
        var type = Type.GetType("RRPG.StaticConfig." + name + ", StaticConfig.Runtime", false);
        Assert.IsNotNull(type, "必须能解析 RRPG.StaticConfig." + name + "（StaticConfig.Runtime）");
        return type;
    }

    /// <summary>只读：经 StaticConfigRegistry.TryGetBuildings 取当前生效的建筑注册表（覆盖优先）。</summary>
    private static object TryGetBuildingsRegistry(out string diagnostic)
    {
        var registryType = StaticConfigType("BuildingRegistry");
        var method = ExactMethod(StaticConfigType("StaticConfigRegistry"), "TryGetBuildings", Flags,
            "StaticConfigRegistry.TryGetBuildings 必须存在",
            registryType.MakeByRefType(), typeof(string).MakeByRefType());
        var args = new object[] { null, null };
        bool ok = (bool)method.Invoke(null, args);
        diagnostic = (string)args[1];
        Assert.IsTrue(ok, "TryGetBuildings 必须成功：" + diagnostic);
        return args[0];
    }

    /// <summary>只读：当前 Editor-only internal 覆盖（null = 无覆盖）。</summary>
    private static object BuildingsOverride()
    {
        var method = ExactMethod(StaticConfigType("StaticConfigRegistry"), "GetBuildingsOverrideForTests", Flags,
            "Editor 编译下必须存在 internal GetBuildingsOverrideForTests", Type.EmptyTypes);
        return method.Invoke(null, null);
    }

    /// <summary>调用 Editor-only internal 测试缝注入 / 清除内存覆盖（不写盘、不改生产 Root）。</summary>
    private static void SetBuildingsOverride(object registry)
    {
        var method = ExactMethod(StaticConfigType("StaticConfigRegistry"), "SetBuildingsOverrideForTests", Flags,
            "Editor 编译下必须存在 internal SetBuildingsOverrideForTests",
            StaticConfigType("BuildingRegistry"));
        method.Invoke(null, new[] { registry });
    }

    /// <summary>
    /// 在真实定义目录末尾追加一条仅内存的 aux FunctionBinding（真实 known profile ws_test_02 / Capacity 1），
    /// 与第 19 个 Edit 用例同一口径；不写盘、不改 Root / 生产配置。
    /// </summary>
    private static object RegistryWithAuxSlot(object baseline)
    {
        Assert.IsNotNull(baseline, "基准建筑注册表不得为 null");
        var registryType = StaticConfigType("BuildingRegistry");
        var bindingType = StaticConfigType("BuildingFunctionBinding");
        var listType = typeof(List<>).MakeGenericType(bindingType);
        var bindings = (IList)Activator.CreateInstance(listType);
        Assert.IsNotNull(bindings, "BuildingFunctionBinding 列表必须可构造");
        foreach (object existing in (IEnumerable)Get(baseline, "FunctionBindings")) bindings.Add(existing);

        object aux = Activator.CreateInstance(bindingType);
        Assert.IsNotNull(aux, "BuildingFunctionBinding 必须可实例化");
        bindingType.GetField("BuildingDefinitionId", Flags).SetValue(aux, "building_farm");
        bindingType.GetField("FunctionSlotId", Flags).SetValue(aux, "aux");
        bindingType.GetField("FunctionType", Flags).SetValue(aux, "Workplace");
        bindingType.GetField("FunctionProfileId", Flags).SetValue(aux, "ws_test_02");
        bindings.Add(aux);

        object registry = Activator.CreateInstance(registryType, new object[] { Get(baseline, "All"), bindings });
        Assert.IsNotNull(registry, "双 slot 内存 BuildingRegistry 必须可构造");
        return registry;
    }

    /// <summary>建筑注册表内容指纹（只读反射；用于覆盖恢复前后默认内容比较，不依赖引用身份）。</summary>
    private static string DescribeRegistry(object registry)
    {
        if (registry == null) return "<null>";
        var definitions = ((IEnumerable)Get(registry, "All")).Cast<object>()
            .Select(d => (string)Get(d, "BuildingDefinitionId") + "/" + (string)Get(d, "DisplayName") + "/"
                         + (string)Get(d, "Category") + "/" + Get(d, "FootprintWidth") + "x" + Get(d, "FootprintDepth"));
        var bindings = ((IEnumerable)Get(registry, "FunctionBindings")).Cast<object>()
            .Select(f => (string)Get(f, "BuildingDefinitionId") + ":" + (string)Get(f, "FunctionSlotId") + ":"
                         + (string)Get(f, "FunctionType") + ":" + (string)Get(f, "FunctionProfileId"));
        return string.Join(";", definitions) + "||" + string.Join(";", bindings);
    }

    /// <summary>只读：按 BuildingInstanceId + FunctionSlotId 取真实功能岗位行（同一实例下 slot 不得按建筑合并）。</summary>
    private static object WorkplaceRowFor(object management, string buildingId, string slot, string what)
    {
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, what + "：Management.CurrentSnapshot 必须已读取");
        object row = ((IEnumerable)Get(snapshot, "Workplaces")).Cast<object>()
            .FirstOrDefault(w => string.Equals((string)Get(w, "BuildingInstanceId"), buildingId, StringComparison.Ordinal)
                                 && string.Equals((string)Get(w, "FunctionSlotId"), slot, StringComparison.Ordinal));
        Assert.IsNotNull(row, what + "：必须存在建筑 " + buildingId + " 槽位 " + slot + " 的真实功能岗位行");
        return row;
    }

    private static string WorkplaceIdForSlot(object management, string buildingId, string slot, string what)
        => (string)Get(WorkplaceRowFor(management, buildingId, slot, what), "WorkplaceId");

    /// <summary>第 3 次真实临时承诺：加入后由产品 post-add Recognition 建立 Unified Event Pending。</summary>
    private static void CompleteTestProject3rd(string personId, string what)
    {
        Assert.AreEqual("Success", Bridge("TryStartProject", "prj_test_01", new[] { personId }).ToString(),
            what + "：第 3 次真实加入 prj_test_01 必须成功（post-add 不回滚）");
        Assert.IsNotNull(ProjectOfPerson(personId), what + "：第 3 次加入后 Active Project 必须保留");
    }

    // ==================== 用例 17：用户完整路径（无选人 → 人口行只选人 → 切页 → 四命令 → 独立详情） ====================

    /// <summary>
    /// 制作人验收路径的真实用户链路（全部走真实产品导航 / EventSystem 指针点击 / authored 控件）：
    ///   1) 世界无选人时打开管理：不跳页、右栏明确提示未选人、独立详情入口禁用；
    ///   2) 真实 EventSystem 点击人口行：只用既有 HomeSelectionContext 选人并留在管理页
    ///      （不进入 Person Surface），行高亮 + 右栏显示同一人物；
    ///   3) 切“职责与场所”页并点击真实岗位行 / 施工行：人物选择保留；
    ///   4) 派工 / 加入施工 / 撤出施工 / 撤职逐项检查按钮 enabled、真实点击（top-hit 由 RealPointerClick
    ///      的 RaycastAll 断言）与真实领域回执；施工加入/撤出期间 Primary Assignment 保持不变；
    ///   5) 独立“查看人物详情”入口正常导航到既有 PersonDetail，关闭后重开管理仍保留同一人物。
    /// 本用例不调用 Selection/私有 helper 代替用户选人：行点击由真实指针点击执行。
    /// 旧“人口行单击强制进入 Person Surface”的期望已在本包修订，原人物往返覆盖仍在第 1 例保留。
    /// </summary>
    [UnityTest]
    public IEnumerator Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry()
    {
        IsolateActivityOpportunitiesForFixture("用户完整路径（无选人→选人→四命令→独立详情）");

        // 夹具：先完成 farm A（真实施工 + 真实推进），再放置仍在施工的 farm B 作为施工目标。
        string completed = PlaceBuilding("building_farm", 4f);
        string builder = SnapshotPersonIdAt(0);
        Assert.AreEqual("Success", Bridge("TryAddConstructionWorker", completed, builder).ToString(),
            "夹具必须能用真实施工入口为 farm A 建立参与者");
        AdvanceTo(14400);
        Call(_host, "RefreshNow");
        Assert.AreEqual("Completed", BuildingState(completed), "farm A 必须在真实推进后完工");
        string constructing = PlaceBuilding("building_farm", 8f);
        Assert.AreEqual("UnderConstruction", BuildingState(constructing), "farm B 必须处于施工中");

        // 1) 世界无选人时打开管理。
        Call(Get(_host, "Selection"), "Clear");
        Call(_host, "RefreshNow");
        Assert.IsNull(SelectionId(), "夹具必须从无选人开始");
        OpenManagementThroughNavigation("用户路径·管理（无选人）");
        yield return null;
        var management = Management();
        Assert.IsNotNull(management, "Shell.Management 必须是真实 authored Prefab 实例");
        Assert.AreEqual("Management", Surface(), "无选人打开管理必须进入 Management Surface");
        Assert.IsNull((string)Get(management, "SelectedPersonId"), "无选人时管理页不得伪造人物选择");
        var detailEntry = (Button)Get(management, "personDetailButton");
        Assert.IsNotNull(detailEntry, "独立详情入口必须绑定");
        var detailEntryLabel = detailEntry.GetComponentInChildren<TMP_Text>();
        Assert.IsNotNull(detailEntryLabel, "独立详情入口必须有 authored 文案");
        Assert.AreEqual("查看人物详情", detailEntryLabel.text, "独立详情入口文案必须明确");
        var detailEntryGraphic = detailEntry.GetComponent<Image>();
        Assert.IsNotNull(detailEntryGraphic, "独立详情入口必须有可命中的 Image");
        Assert.IsTrue(detailEntryGraphic.raycastTarget, "独立详情入口 Image 必须参与 Raycast");
        Assert.IsFalse(detailEntryLabel.raycastTarget, "详情入口文案不得抢走按钮 Raycast");
        Assert.IsFalse(detailEntry.interactable, "无选人时独立详情入口必须禁用");
        var hint = (TMP_Text)Get(management, "actionHint");
        Assert.IsNotNull(hint, "操作区原因提示必须绑定");
        Assert.IsTrue(hint.text.Contains("尚未选择人物"), "无选人时提示必须说明具体原因，实际：" + hint.text);
        var detailTitleBefore = (TMP_Text)Get(management, "detailTitle");
        Assert.IsTrue(detailTitleBefore.text.Contains("未选择人物"), "右栏必须清楚显示未选择人物");

        // 2) 真实 EventSystem 点击人口行：只选人、留在管理页。
        object snapshot = Get(management, "CurrentSnapshot");
        Assert.IsNotNull(snapshot, "管理快照必须已读取");
        var persons = ((IEnumerable)Get(snapshot, "Persons")).Cast<object>().ToList();
        var freeIds = persons.Select(p => (string)Get(p, "PersonId"))
            .Where(id => id != builder && PersonAvailability(id) == "InVillage"
                         && ActivityClaim(id) == null && string.IsNullOrEmpty(PersonWorkplace(id)))
            .ToList();
        Assert.GreaterOrEqual(freeIds.Count, 1,
            "用户路径夹具需要至少 1 名未被活动占用且无既有职责的真实人物");
        string personId = freeIds[0];
        object person = persons.First(p => string.Equals((string)Get(p, "PersonId"), personId, StringComparison.Ordinal));
        string personName = (string)Get(person, "DisplayName");
        int personIndex = persons.FindIndex(p => string.Equals((string)Get(p, "PersonId"), personId, StringComparison.Ordinal));
        var populationContent = (RectTransform)Get(management, "populationContent");
        Assert.IsNotNull(populationContent, "populationContent 必须绑定");
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(populationContent);
        yield return null;
        Canvas.ForceUpdateCanvases();
        var row = populationContent.GetChild(personIndex) as RectTransform;
        Assert.IsNotNull(row, "人口行必须存在");
        Assert.IsTrue(row.gameObject.activeInHierarchy, "人口行必须可见");
        RealPointerClick(row, "用户路径·人口行（真实 EventSystem 点击）");
        yield return null;

        Assert.AreEqual("Management", Surface(), "人口行单击不得跳转页面，必须留在集中管理");
        Assert.IsTrue((bool)Get(management, "IsVisible"), "人口行单击后管理页必须仍可见");
        Assert.IsFalse((bool)Get(Get(_shell, "PersonDetail"), "IsVisible"), "人口行单击不得打开人物详情");
        Assert.AreEqual(personId, SelectionId(), "人口行单击必须用既有 Selection 选中该人物");
        Assert.AreEqual(personId, (string)Get(management, "SelectedPersonId"), "管理页必须保持同一人物选择");
        Assert.IsTrue(detailEntry.interactable, "选中人物后独立详情入口必须可用");
        var detailTitle = (TMP_Text)Get(management, "detailTitle");
        Assert.IsTrue(detailTitle.text.Contains(personName), "右栏必须清楚显示当前选中人物：" + detailTitle.text);
        var detailBody = (TMP_Text)Get(management, "detailBody");
        Assert.IsNotNull(detailBody, "右栏详情正文必须绑定");
        Assert.IsTrue(detailBody.text.Contains(personId), "右栏必须显示人物身份：" + personId);
        var selectedRow = populationContent.Cast<Transform>().FirstOrDefault(r => r != null
            && r.gameObject.activeSelf
            && PopulationRowTitle(r) == ExpectedPopulationRowTitle(person));
        Assert.IsNotNull(selectedRow, "选中人物行必须仍存在");
        var selectedImage = selectedRow.GetComponent<Image>();
        Assert.IsNotNull(selectedImage, "人口行必须有底色 Image");
        Assert.AreEqual(new Color(.28f, .43f, .36f, 1), selectedImage.color, "选中人口行必须有清楚的高亮");
        Evidence("USER-PATH", "row-click selected=" + personId + "; surface=" + Surface()
            + "; detail-open=False; highlight=True; hint=\"" + hint.text + "\"");

        // 5a) 独立详情入口正常导航（在切页前验证）。
        RealPointerClick((RectTransform)detailEntry.transform, "用户路径·查看人物详情（独立入口）");
        yield return null;
        Assert.AreEqual("Person", Surface(), "独立详情入口必须进入既有 Person Surface");
        var personDetail = Get(_shell, "PersonDetail");
        Assert.IsNotNull(personDetail, "既有 PersonDetail 必须存在");
        Assert.IsTrue((bool)Get(personDetail, "IsVisible"), "独立详情入口必须显示 PersonDetail");
        Assert.AreEqual(personId, (string)Get(personDetail, "DisplayedPersonId"), "PersonDetail 必须显示同一人物");
        Assert.IsFalse((bool)Get(management, "IsVisible"), "进入人物详情后管理页必须隐藏");
        var closeDetail = (Button)Get(personDetail, "close");
        Assert.IsNotNull(closeDetail, "PersonDetail.Close 必须绑定");
        RealPointerClick((RectTransform)closeDetail.transform, "用户路径·关闭人物详情");
        yield return null;
        Assert.AreEqual("World", Surface(), "关闭人物详情必须回 World");
        Assert.AreEqual(personId, SelectionId(), "关闭人物详情必须保留同一人物选择");

        // 3) 重开管理 → 切职责页 → 选岗位目标，人物选择保留。
        OpenManagementThroughNavigation("用户路径·管理（重开）");
        yield return null;
        management = Management();
        Assert.AreEqual(personId, (string)Get(management, "SelectedPersonId"), "重开管理必须保留同一人物");
        Assert.AreEqual("InVillage", PersonAvailability(personId), "用户路径夹具要求所选人物在村可命令");
        ShowWorkplacesPage(management);
        yield return null;
        Assert.AreEqual(personId, (string)Get(management, "SelectedPersonId"), "切换职责与场所页不得清除人物");

        string workplace = WorkplaceIdForBuilding(management, completed, "用户路径·farm A 岗位");
        ClickWorkplaceRow(management, workplace, "用户路径·岗位行（farm A）");
        yield return null;
        Assert.AreEqual(personId, (string)Get(management, "SelectedPersonId"), "选岗位目标不得清除人物");
        var assignButton = (Button)Get(management, "assignButton");
        var removeButton = (Button)Get(management, "removeButton");
        Assert.IsTrue(assignButton.interactable, "合法岗位目标下派工按钮必须 enabled");
        Assert.IsFalse(removeButton.interactable, "无主要职责时撤职按钮必须 disabled");
        Assert.IsTrue(hint.text.Contains("没有主要职责"), "撤职禁用必须提示具体原因，实际：" + hint.text);

        // 4a) 派工。
        ClickButtonField(management, "assignButton", "用户路径·派工");
        yield return null;
        Assert.AreEqual(workplace, PersonWorkplace(personId), "真实派工按钮必须写入 farm A 岗位");
        Assert.AreEqual(1, WorkplaceOccupants(workplace), "farm A 名额占用必须为 1");
        AssertStatusContains(management, "已绑定");
        Assert.IsTrue(removeButton.interactable, "派工后撤职按钮必须 enabled");
        Evidence("USER-PATH", "assign ok workplace=" + workplace + " occupants=" + WorkplaceOccupants(workplace));

        // 4b) 加入施工：Primary Assignment 保持。
        ClickConstructionRow(management, constructing, "用户路径·施工行（farm B）");
        yield return null;
        Assert.AreEqual(personId, (string)Get(management, "SelectedPersonId"), "选施工目标不得清除人物");
        var addConstruction = (Button)Get(management, "addConstructionButton");
        var removeConstruction = (Button)Get(management, "removeConstructionButton");
        Assert.IsTrue(addConstruction.interactable, "合法施工目标下加入施工按钮必须 enabled");
        Assert.IsFalse(removeConstruction.interactable, "未参与该施工时撤出施工按钮必须 disabled");
        ClickButtonField(management, "addConstructionButton", "用户路径·加入施工");
        yield return null;
        object project = ActiveProject();
        Assert.IsNotNull(project, "加入施工后必须存在真实活动项目");
        Assert.IsTrue(Strings(Get(project, "ParticipantPersonIds")).Contains(personId),
            "真实加入施工必须写入施工参与者");
        Assert.AreEqual(workplace, PersonWorkplace(personId), "加入施工必须保留 Primary Assignment");
        Assert.AreEqual(1, WorkplaceOccupants(workplace), "加入施工不得改变岗位占用");
        Assert.IsTrue(removeConstruction.interactable, "加入后撤出施工按钮必须 enabled");
        Assert.IsFalse(addConstruction.interactable, "已在施工中时加入施工按钮必须 disabled");
        Assert.IsTrue(hint.text.Contains("已在本施工项目中"), "加入施工禁用必须提示具体原因，实际：" + hint.text);
        Evidence("USER-PATH", "construction-add ok primary=" + PersonWorkplace(personId)
            + " participants=" + string.Join(",", Strings(Get(project, "ParticipantPersonIds"))));

        // 4c) 撤出施工：Primary Assignment 仍保持。
        ClickButtonField(management, "removeConstructionButton", "用户路径·撤出施工");
        yield return null;
        object projectAfter = ActiveProject();
        bool stillParticipant = projectAfter != null
                                && Strings(Get(projectAfter, "ParticipantPersonIds")).Contains(personId);
        Assert.IsFalse(stillParticipant, "真实撤出施工必须移除施工参与者");
        Assert.AreEqual(workplace, PersonWorkplace(personId), "撤出施工必须保留 Primary Assignment");
        Assert.AreEqual(1, WorkplaceOccupants(workplace), "撤出施工不得改变岗位占用");
        Assert.IsTrue(addConstruction.interactable, "撤出后加入施工按钮必须恢复 enabled");
        Assert.IsFalse(removeConstruction.interactable, "撤出后撤出施工按钮必须 disabled");
        Evidence("USER-PATH", "construction-remove ok primary=" + PersonWorkplace(personId)
            + " remainingParticipants=" + (projectAfter == null ? "<project-gone>" : string.Join(",", Strings(Get(projectAfter, "ParticipantPersonIds")))));

        // 4d) 撤职：重新点回岗位行后撤职，真实释放名额。
        ClickWorkplaceRow(management, workplace, "用户路径·岗位行（撤职目标）");
        yield return null;
        Assert.IsTrue(removeButton.interactable, "有主要职责时撤职按钮必须 enabled");
        ClickButtonField(management, "removeButton", "用户路径·撤职");
        yield return null;
        Assert.IsNull(PersonWorkplace(personId), "真实撤职必须移除 Primary Assignment");
        Assert.AreEqual(0, WorkplaceOccupants(workplace), "撤职必须释放岗位名额");
        AssertStatusContains(management, "已移除");
        Assert.AreEqual("Management", Surface(), "四类命令都不得非预期关闭管理页");
        Assert.IsTrue(SimulationActive(), "管理命令不得关闭模拟");
        Evidence("USER-PATH", "remove-assignment ok workplace=<none> occupants=" + WorkplaceOccupants(workplace)
            + "; surface=" + Surface() + "; simulationActive=True");
    }
}
}
