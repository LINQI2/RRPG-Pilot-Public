# V17 本地提交后最后核验结果

日期 2026-11-06。**HEAD = `21004a26e716c6bcdae10b8ada2f329de27e4e81`**（本轮 `git rev-parse HEAD` 原文，非模板）。
分支 `ai/rrpg-r3b-population-pilot`；staged=0；dirty=134。
本轮**未 Play、未 Builder、未 Refresh、未 SaveAssets、未改资产/Scene/Save/字体、未 Commit/Push**。

## 1. 门禁

| 项 | 值 |
|---|---|
| Label | `v17-postcommit-01` |
| begin / end | **0 / 0** |
| 字体 | `7b5717b976ba…`（前后一致） |
| records | `20261106T061529Z-…-begin.json` / `20261106T061717Z-…-end.json` |

## 2. 编译：请求一次 ScriptCompilation，域重载已完成

```json
{"content":[{"type":"text","text":"RELOAD_REQUESTED compiling=True"}]}
```

重载后回读：

```
project=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game
version=2022.3.62t9  product=RRPG_AI_Pilot
playing=False  compiling=False  updating=False
scriptCompilationFailed=False
console err=0  warn=523  log=2
assemblies=313
ManagementView@Assembly-CSharp
REDACTED_EMAIL
```

未清 Console；`err=0` 是当前 Console 计数，不代表历史无错误。
**「域重载完成 + scriptCompilationFailed=False」只说明当前 Editor 能编译当前工作区，不等于冷启动重建通过。**

## 3. Prefab 与生成根

- Prefab `Assets/Resources/UI/ManagementView.prefab`：**81 transforms，missingScripts=0**。
- 三条关键 GUID 仍正确解析：
  `4a7dfebd→ManagementView.cs(ManagementView)`、`f37df54f→ManagementRowView.cs(ManagementRowView)`、
  `f9418c72→RRPG_SourceHanSansCN_Regular_TMP.asset`。
- 生成根已加载：`RRPGStaticConfigRoot`，`contentSignature = 78f532b6b58d916d67df6300e26bb0db95fc40ffc02c887c162aa8f7a06168ae`。
- Mirror 目录 **44 个 json**；三份初始化镜像：`InitialWorld.json` `5af02464bbf359c6`(1569B)、
  `InitialPopulation.json` `ae0fe5456266ff66`(1377B)、`InitialBuilding.json` `32abb64be5c5fbe7`(2209B)。

## 4. 签名/NoOp 校验：**未能安全执行，如实报告**

- `ComputeSignature(List<…>, BuildOutcome)` 与 `IsUpToDate(String, Int32, PresentationInput, Payload×4, List<…>, Payload, Payload)`
  均为 **private static**，入参是 Build 内部读取管线的产物。
- 若用反射强行构造，等于复制 Build 的大部分逻辑，且无法保证无副作用；`Build(false)` 在签名不匹配时**仍会写资产**，禁止执行。
- 因此本轮**只证明**：生成根存在、可加载、带签名；三份镜像 SHA 已记录。
  **不证明**签名与当前配置匹配，**不声称** Build 必然 NoOp，更不声称重建通过。

## 5. 状态分层（按工作包第 4 点）

| 命题 | 结论 | 依据 |
|---|---|---|
| 已提交源码/Prefab 闭包完整 | **证据支持** | 12/12 GUID 解析、0 missing script、4 行模板齐全、类型在正确程序集 |
| 当前 Editor 能编译当前工作区 | **证据支持** | `scriptCompilationFailed=False`，err=0 |
| 冷启动 clean clone 全重建 | **未执行** | 未做 fresh checkout + Builder + 全量测试；无证据 |
| 生成配置与 Builder 签名一致 | **未证明** | 纯校验入口不可安全调用 |

## 6. 关于旧管理验收脚本

`ManagementShellPlayTests` 依赖未跟踪的 `R3B-20261022/workspace-baseline.json`：按工作包定性为
**Disposable 历史验收工具**，保留磁盘证据、不进长期回归、**不再据其未提交判定产品阻塞**。
本轮未修改、未删除该测试或基线。

## 7. 副作用与产出

- 一次 ScriptCompilation（依赖的必要操作）；门禁 2 条 record。
- 新增 3 个文件：本文件、`postcommit-state.json`、`postcommit-raw-responses.md`。
- 未覆盖任何旧报告；未进入 G6/G7。

## 8. 未执行项

冷启动 clean clone 重建、Builder、全量/定向测试、Builder 签名一致性证明、Play、产品重验。
