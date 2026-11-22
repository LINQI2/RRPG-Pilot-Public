# 附记：验收返修的安全刷新、编译发现与 Prefab 绑定核验（2026-10-28，阶段 1/1）

> 本文件只补充“上一份返修报告未做 Unity 侧核验”的缺口；不改写 `person-selection-command-repair.md`，不预填任何测试通过结论。

## 1. 本层执行边界

- 只在本任务授权目录 `D:\AI_Agent_Lab\rrpg_pilot\repo` 内读写；只连接 Pilot 工程 `UnityProject/RRPG1.0game`。
- 未运行 `tests-run`、未启动/忙等原生 TestRunner、未调用 `rql-mutate`、未执行 SaveAssets / SaveChanges / SaveDirtyScenes / SetDirty / ClearDirty / PrefabStage / Shell / Git / Prefs / 启停 Editor。
- 未做字体测量、未实例化 UI、未做 TMP 渲染或 `GetPreferredValues` 探针；未读写正式工程、认证目录或其他工作区；未创建额外付费子 Agent。
- 本层没有修改产品源码、Prefab、测试源码或两份返修报告；只新增本附记。未发现需要“最小编译/绑定修复”的缺陷。

## 2. 前置实测（只读）

| 项 | 实测值 |
| --- | --- |
| Editor 身份 | `2022.3.62t9`，`dataPath=D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game/Assets` |
| 运行/编译 | `isPlaying=False`、`isPlayingOrWillChangePlaymode=False`、`isCompiling=False`、`scriptCompilationFailed=False` |
| 原生 TestRunner | `TestRunnerApi.IsRunActive()=False`，本层未启动运行 |
| 场景 / PrefabStage | `SampleScene dirty=False roots=5`、`PrefabStage=null`；`scopeActive=False` |
| 受保护字体（磁盘） | `14961033B / 3c758e16d62ef8bd36331a6ad26fc00bc165606f6b6bcdfe24da44108c404854` |
| 字体 9 子资产 IsDirty | `dirty=0`（只读 `EditorUtility.IsDirty`；未测量字模） |
| 34 项 workspace-baseline | `baselineTotal=34 mismatch=0 missing=0`，逐文件 bytes+SHA256 一致 |
| 用户存档（只读实际值） | `1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot/rrpg_save_p0.json`：**`13438B / b3831d4dc52f81f4cb74beee6f47c5f89242e705fa62e1c9aac1e31ec6d4b4d3`**，`.tmp=False`，mtime `2026-10-27T08:22:32.3195198Z`。此值为“当前实际档”，**不是**旧 `13405 / 4b2b56f6…` 回滚目标 |
| 既有存档 guard | `R3B-SaveProtection-20261024`：`dirs=293`（`restored=292` + `protective-snapshot=1`），`unfinished=0`；下一宿主运行由测试内 `SaveSlotGuard` 按当前实际字节捕获/恢复 |

## 3. 报告副本 SHA（原字节复制，旧失败保留）

| 文件 | 字节 | SHA256 |
| --- | --- | --- |
| `R3B-UI-20261028/person-selection-command-repair.md` | 6033 | `da52af93b74017783305686688228cd23f325f764162733531afde006bffdc65` |
| `R3B-UI-20261027/management-selection-acceptance-fix.md` | 6033 | 同上 |

只读回读 `byteIdentical=True`；两份文件本轮均未改写，10-27 目录内旧失败记录原样保留。

## 4. 安全刷新与编译发现

- 执行一次 `AssetDatabase.Refresh()`（默认选项）：返回后 `isCompiling=False`、`scriptCompilationFailed=False`；刷新前后字体磁盘 `3c758e16…c404854`、9 子资产 `dirty=0` 不变。
- `Library/ScriptAssemblies/Assembly-CSharp.dll`：`650240B / 676028ab8290f509aa09f531de42d4c73708a0c355c25759a198e4bd6f0c443d`，mtime `2026-10-27T10:18:50Z`，晚于 `ManagementView.cs`（`42941B / e02de745…b3c`，mtime `08:48:40Z`），说明新增 `SelectPersonRow` / `OpenSelectedPersonDetail` / `personDetailButton` 已随当前源码成功编译。
- `BattleStage1.PlayModeTests.dll`：`575488B / a9e333362a4bf9cc99e73b53da6d91d2881ddf75fefedf82078ffb52052ca378`，mtime `2026-10-27T10:18:50Z`，晚于测试源码（`495525B / 24e47dcc…0812`，mtime `08:50:01Z`）。
- 发现口径（只读）：已编译测试程序集内 `UnityTestAttribute` 方法 `17` 个；RQL `tests | where fullName contains "ManagementShellPlayTests"` 同样返回 **17 条 PlayMode**，含新第 17 例 `Management_UserPath_SelectRowStayOpenThenCommandsAndIndependentDetailEntry`；原 16 例方法名全部保留。本层未执行这些用例。

## 5. Prefab 序列化绑定（静态 YAML 核验，未实例化/未渲染）

- 对象：`Assets/Resources/UI/ManagementView.prefab`，`290093B / f750ed83…977b`。
- `ManagementView` 组件序列化对象引用：**`40` 个**、`{fileID: 0}` **0 个**、无法解析的 fileID **0 个**；其中 `tabButtons=3`、`statusFilterButtons=3`，其余 34 个标量引用全部非空。
- `personDetailButton` → `{fileID: 1450098202734151205}`：该组件挂在 `OpenPersonDetailButton`（`1450098202734151201`）上，`m_TargetGraphic` 指向同对象 `Image`（`…204`）；`Image m_RaycastTarget: 1`；子 `Label` 的 TMP `m_RaycastTarget: 0`、文本为“查看人物详情”。
- `DetailPanel` 子列表含 `{fileID: 1450098202734151202}`；`DetailTitle` 宽度为 `306`；新按钮矩形 `(x:328, y:-14, w:174, h:40)`。新增 fileID 段 `1450098202734151201…209` 各自只有唯一 `&` 定义。
- 未加载 Prefab 资产、未进入 PrefabStage、未实例化、未触发 TMP 测量；因此不存在作用域外 UI 渲染或字体脏化。

## 6. 待宿主复核（本层不预填）

- 当前 `testResults` 仍是上一轮 **16 条**（全 Passed，最后证据行 `2026-10-27T07:07:53Z`），**不是** 17 例回执；本层未运行、未改写。
- 第 1 例修订与第 17 例的通过/失败、真实按钮 top-hit、领域事实与独立详情导航，均须由后续宿主对整类 `ManagementShellPlayTests` **串行**运行的真实 NUnit 回执确认。
- 本层只读/工具侧异常披露：一次发现用 snippet 因引用 `NUnit.Framework.UnityTestAttribute` 写法错误被工具拒绝编译（2 条 Console Error），随即改用已编译程序集反射 + RQL `tests` 只读发现；该错误不涉及产品程序集，`scriptCompilationFailed=False`。未清除 Console 以保留真实痕迹。
