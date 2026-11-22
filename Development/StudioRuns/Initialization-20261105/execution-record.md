# Initial World 本轮执行记录

日期：2026-11-05（北京时间）。Target Branch：`ai/rrpg-r3b-population-pilot`。当前为进行中记录，不是最终验收或 Handoff。

## 授权与保护

制作人已批准 Pilot 范围内的 Initial World 收尾与必要 DSH 调用，暂停后于本次消息明确恢复。保留原保护清单，不修改正式工程/连接器/权限，不 Commit/Push。

既有状态默认统计 69 修改 + 106 未跟踪项；逐文件展开 1209 未跟踪，共 1278 文件。基线清单与 SHA 见本轮 DevelopmentSpec，不以新增执行证据覆盖基线。

## 本次真实执行

| 时间/步骤 | 实际结果 |
|---|---|
| 11:04:16 diagnose | 双方身份匹配，DSH 项目读写、空闲、未知投递 0；只证明连接检查 |
| Compiler 首次测试 | `dotnet test ... --no-restore`；testhost 90 秒未连接，运行中止，0 项实际执行；TRX `test-results/initialworld-20261105.trx`，不是 PASS |
| Compiler validate | `dotnet run --project Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj --no-build -- validate --root D:\AI_Agent_Lab\rrpg_pilot\repo`；退出 0，errors=0、warnings=9（导航 Sheet 跳过），Mirror 未写入。InitialWorld 1 / Population 5 / Building 7 行。此结果在 DSH 配置改动前取得 |
| 恢复 11:12:52 diagnose | 原双方身份和目录匹配，项目读写、空闲、待答 0、未知投递 0 |
| delegate preview | 固定编号 `initialworld-20261105-dispatch01`，目标 `session-2af1b774-439f-416a-9e17-092a43095a66`，Pilot；revision=18，预览未发送 |
| 原会话 status | running=false，seq=26；可见公开回复“收到，未执行任何操作”；结合从未手工发送的本聊天上下文核对，不制造新 ID |
| delegate --send | 同一编号，state=accepted，退出 0；仅接口受理，不等于完成 |
| DSH 后续 status | running=true，seq=116，公开回复确认 Shell 沙箱授予层失败并继续核对 Unity 工具 |
| Compiler 宿主重跑 | 在沙箱外使用同命令与新文件名 `initialworld-20261105-host.trx`；退出 0，280/280 Passed、failed=0、skipped=0；已独立解析 TRX Counters，实际执行 280 项。首次中止记录保留 |

## DSH 真实交付与技术复核

DSH 原会话随后 running=false、seq=167；已读取真实公开回复（seq=157/164）及 `dsh-progress.md`、`dsh-result.md`。只交付环境报告，没有产品/测试/配置修改或 Unity 测试。

复核裁决：

1. Shell 的 `SetNamedSecurityInfoW failed (Win32 5): grantWrite(...)` 只证明 DSH 沙箱准备失败，不能据此推定当前 Windows 用户缺少哪项目录权限。DSH 报告的根因推论未获证实。
2. **不采纳 DSH 建议的 full access 或管理员 ACL 修改**：超出当前授权且无必要；本轮 Codex Shell 已完成 Compiler 280/280 与 validate，可继续承担配置侧操作。不是必须先修 DSH Shell 才能推进所有开发。
3. Temp/UnityLockfile 缺失和旧日志不是实时 Editor 状态 API，不独立证明整个机器没有 Editor。Codex 当前工具列表亦无 Unity 工具，故未取得精确 Pilot Editor、Console、runner、场景/字体 dirty 的现场证据。
4. Codex 的 Windows CIM 进程只读查询在沙箱中报“无法从客户端中访问 CIM 资源”，不把查询失败写成无进程。
5. DSH 对 save-guard 的 grep 只覆盖若干已知状态，不足以独立证明所有 manifest 均恢复成功；本轮没有 Unity 测试，未用该静态检查冒充保护机制验证。
6. `protection-before-doc-sync.json`：独立 SHA256 比对原清单 1278/1278 一致，missing=0、changed=0。其后仅对 5 份 Current 文档加顶部进度，改前原字节备份到 `before-doc-sync/`，历史正文保留。

## 实际改动与当前限制

本轮新增执行规格、工作包、进度/结果报告、真实 Compiler TRX 与保护核验，更新项目状态/交接附记/项目日志/文档地图/DevelopmentSpecs 索引。**没有新增产品 C#、测试源码、XLSX、Mirror、Unity 序列化资产或存档变更。**

Unity/V4、NewGame–派工–生产–Save–Continue、旧档与默认 A 表现仍未验证/未修，不能标记整包完成。需要建立既有 Pilot 的 Editor/Unity 工具连接；不安装或升级 Pipeline（本工程 2022.3/Tuanjie，技能所述 Pipeline 需要 Unity 6，不适用），不修改连接器或认证，不扩大权限。

## 待完成

读取 DSH 现场、V4、默认表现改动及真实回执；独立代码审查与必要验证；核对既有成果保护；更新 Current 文档和可玩入口。当前不宣称 Initial World 完成。

## 环境修复后续执行（2026-11-05 12:49 起）

制作人明确表示问题已解决并要求继续。12:49:28 diagnose 为原双方身份匹配、项目读写、空闲、待答0、未知投递0；status seq193 的维护探针真实返回 PILOT_SHELL_PROBE_OK。公开维护文档16说明原会话Shell已经最小验证，但尚未处理Unity工具接入，不能借此认定Unity恢复。

原 dispatch01 早已 accepted 且返回了环境阻断结果，未重试该发送。为本次明确恢复编写 `dsh-resume-01.md`，使用独立续执行编号 `initialworld-20261105-resume01` 向同一会话发送与原任务关联的增量指令；预览确认原目标和 revision18，send 返回 accepted。此为有已知终态后的新续执行，不绕过未知投递、不重建会话、不更改原记录。

续执行要求保留原报告、新写 resume 进度和结果，明确不采纳 Full Access/ACL 建议，已过 Compiler 不重复跑；从工具和保护面实测开始，优先原 V4 未完成专项。

### 独立复核增量

- DSH 进程查询最初漏报 Editor，Codex 宿主 CIM 后确认原 Pilot PID25628，且其两个 AssetImportWorker 子进程存在。基于漏报曾启动本轮重复 Pilot PID26976（仅指向 Pilot 和本轮专用日志）；正常关闭未生效，核对其唯一日志参数/工程路径后仅结束该自有重复进程，原Editor/worker未动。未保存场景或修改原用户实例。
- 实际既有 MCP 通道为 localhost:21867/mcp，DSH 已通过真实 API 确认 Pilot 2022.3.62t9、SampleScene dirty=false。旧29387端口不作当前状态依据。
- Codex 已使用同一既有通道的只读 `tools/list` 和 `rql-query` 独立查询。`reviewer-test-results.txt` 中24条：基础10、配置管线5、产品接线9，全部Passed，message/stackTrace均null。没有额外运行测试，也未并行写世界。保存 `reviewer-mcp-tools.txt` 为本轮实际工具schema。
- `protection-resume-observation.json`：恢复后比对最初1278文件，仍只有本轮已记录的5份Current文档不同，其余既有文件相同；未将历史报告中的旧存档/字体哈希当成本轮基线。
- DSH公开回复seq579报告制作人已在原生会话选择“生成A档白模图”；后续按该实际制作人选择实施，主开发仍须核对最终素材、绑定和真实结果，不以消息宣称替代文件与测试证据。

### 本轮交付复核结果

原DSH续执行已明确结束（running=false，seq952），主开发随后接管复核，无并行工程写入。DSH产品Play2例日志、V4原guard、当前生成配置和截图均实际读取。主开发最终独立V4+视觉3/3、0 skipped；最新validate PASS；三图源/Manifest/Unity生成字节一致。最终Editor空闲、场景dirty=false、字体9子资产dirty=0；136 guard均restored。用户单槽与场景字节保持本轮捕获值。

**保护异常未解决**：原字体14968844B/9ada5362…变成14969377B/f0f5b901…（+533B），不能用Git覆盖原dirty成果，未找到对应前字节。当前副本保留，无凭空恢复或手改YAML。故不标记整包完成或最终Handoff。详情、真实证据路径、可玩入口和Deferred见 `technical-review.md`；本轮新增/修改清单见 `delivery-files.json`。Current五份文档仅更新本轮顶部附记，历史正文保留。

## 字体保护门禁（2026-11-05 制作人确认方案后）

制作人确认采用「TMP 字体保护门禁」：**现行字体作为新基线，不恢复历史版本**；门禁只做核验与取证，
**不自动恢复、不做 Git 覆盖、不静默接受变化**。

### 基线（已落盘）

- 文件：`UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`
- SHA256 `f0f5b901a5ef56a68450803e39510846ccfc38623ab0d3c8654b22f33bcf1681`，14,969,377 B，
  mtime `2026-11-05T05:01:47.3537285Z`
- 完整字节备份 + 清单：`font-protection/baseline.json`、
  `font-protection/baseline/RRPG_SourceHanSansCN_Regular_TMP.asset`（大小与 SHA 回读一致）
- 历史三态（10-22 / 10-23 / 11-01）以只读形式登记在 `baseline.json.previousStates`，仅作溯源，不作为可恢复版本。

### 工具（复用现有 PowerShell 证据链，未引入新框架）

`scripts/font-guard.ps1`，命令：`capture` / `verify` / `begin` / `end` / `status` / `selftest`。
- `verify`：比对大小+SHA；不一致 → **写 incident（含变化前后哈希/大小/mtime + 当前字节副本）并 exit 2**，不恢复。
- `begin`/`end`：在可能触发序列化的流程前后各核验一次；`begin` 若发现已漂移则直接拒绝继续（exit 2）。
- `status`：只读报告；`selftest`：仅在临时副本上模拟变化，绝不触碰真实字体。

### 自检（要求 7）

`font-guard.ps1 selftest` 真实结果：`{"result":"PASS","captureExit":0,"unchangedExit":0,"changedExit":2,"incidents":1}`
—— 未改动时 exit 0；**单字节翻转被捕获，exit 2 且生成 incident 证据**。

### 真实流程门禁（要求 3）

| 窗口 | 覆盖动作 | 结果 |
|---|---|---|
| `w1-refresh-builder` | `AssetDatabase.Refresh(ForceUpdate)` + `StaticConfigBuilder.ExecuteBuild` | begin OK → end **PASS**（字体哈希未变） |
| `w2-playmode-product` | PlayMode 产品 2 例（真 UI 派工/加工 + Save/Continue） | begin OK → 测试 **2/2 Passed** → end **PASS** |
| `w3-v4-regression` | PlayMode `VillageV4ResourceSurplusPlayTests`（1 例） | begin OK → 测试 **1/1 Passed** → end **PASS** |

记录文件：`font-protection/records/20261105T064935Z-w1-refresh-builder-begin.json`、
`…064943Z-…-end.json`、`…064947Z-w2-playmode-product-begin.json`、`…065014Z-…-end.json`。

### 对状态的影响

字体异常从「未解决」变为「**已用新基线 + 门禁受控**」，但**写入成因仍未定位**，
故整包状态仍为 **Partial / Product Acceptance Pending / 未 Commit 未 Push**。
后续任何 Unity 流程应先 `begin`、后 `end`；`end` 非 0 时必须停止后续测试并保留 incident。
