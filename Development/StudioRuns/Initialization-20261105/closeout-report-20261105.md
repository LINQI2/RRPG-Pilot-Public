# Initial World 技术收尾报告（2026-11-05 第二轮）

分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3e38fe2e2e142bfdb2502de90f7991030`，未 Commit / 未 Push。
本报告是**技术收尾**，不是产品验收，不宣布 Work Package Closed。

## 1. 字体异常

| 项 | 结论 |
|---|---|
| 原因判断 | **未证明**。形态与「TMP 动态图集补字形后被编辑器落盘」一致（字符数 646→648），但无前字节无法做字节级 diff；13:01:47 那次写入的触发者无日志可查 |
| 是否修复 | **否**，且不应在无副本情况下修 |
| 修复前后 SHA256 | 未修改：`9ada5362…`（14,968,844B，接手前）→ 仍为 `f0f5b901…`（14,969,377B，当前） |
| 备份位置 | `review-backups/font-after-dsh.asset`（14,969,377B，SHA 与当前一致，已回读） |
| 数据丢失风险 | **未发现**：9 子资产、648 字符=648 字形、7×1024 图集、`isDirty=false`、加载正常。**但精确还原能力已丢失**（14,968,844B 在本地 0 命中） |

补充事实：Git HEAD 版本仅 12,826,076B（`ba75bbc2…`），比当前小约 2.1MB，回退会覆盖既有成果并丢内容 →
`git checkout/restore` 被明确排除。完整取证：`font-asset-investigation-20261105.md`。

## 2. 功能与测试

**最终确认的实现范围**（本轮未新增开发）：
- A 档白模：`Development/SourceAssets/Village/A-Tier/{kitchen,house,person}-a.png`；
  `RRPG_VisualAssets.xlsx` +3 行、`RRPG_VillagePresentation.xlsx` +5 键并改 4 行建筑绑定。
- `HomePresentationResolver.cs`：A 档人物表现键参与查表；`HomeWorldEntityView.cs`：A 档人物等比显示、静止白色。
- 生成链：Compiler → `VisualAsset/VillagePresentation/BuildingPresentationBinding/AssetManifest.json`
  → Unity `StaticConfigBuilder` → 数据库与托管 PNG。未手改 Mirror / YAML。
- 默认初始化配置实测（`InitialBuilding.json`）：`building_farm=1`、`building_kitchen=1`、`building_house=5`，共 7 行。

**实际执行并复核回执的测试**：

| 测试 | 结果 | 复核方式 |
|---|---|---|
| Compiler 全量 | **280/280 Passed**，0 failed/skipped/notExecuted | 直接解析 `test-results/initialworld-20261105-host.trx` Counters |
| Compiler 首次中止 | total=0 / outcome=Failed，**如实保留** | 解析 `initialworld-20261105.trx` |
| Config validate（改后） | **PASS**，errors=0、warnings=9 | `reviewer-validate.log` 结果行 |
| Unity Builder | **PASS**，noOp=false | `.build-state/builder.stage.txt` |
| 初始化 EditMode | **24/24 Passed**，0 failed | 统计 `reviewer-test-results.txt` 的 24 条 Passed |
| DSH 产品 PlayMode | **2/2 Passed** | 本轮亲跑的 4 次运行输出 |
| 主开发独立 V4+视觉 | **3/3 Passed**，2.7028398s | `reviewer-play-v4-visual.txt` |
| 存档 guard | **136/136 restored**，0 bad | `reviewer-guard-final.json` + 本轮重扫 |
| 用户存档 / SampleScene | 字节一致 | `3ed26a77…`(21611B) / `570ab047…`(846770B) |

**未执行（不得写成 PASS）**：全量 EditMode（除上述 24 例）、全量 PlayMode、管理 17 例、物理输入 / IME / DPI /
Player 构建、A/B 观感的制作人验收。

## 3. 项目文档

本轮修改（均只**新增顶部收尾横幅 / 追加条目**，不改历史正文）：

| 文件 | 更新内容 |
|---|---|
| `Docs/项目状态快照.md` | 顶部新增 2026-11-05 收尾横幅：默认 1/1/5、字体结论、回执、Partial 状态 |
| `Docs/项目日志.md` | 同上横幅（含“全量与管理 17 例未重跑”声明） |
| `Docs/开发验收交接.md` | 同上横幅（明确“技术收尾 ≠ 产品验收”） |
| `Docs/文档地图.md` | 同上横幅 + 指向字体取证报告的新引用 |
| `Docs/DevelopmentSpecs/README.md` | 同上横幅 + 说明本轮不新增 DevelopmentSpec |
| `Development/StudioRuns/Initialization-20261105/font-asset-investigation-20261105.md` | 新增：字体完整取证 |
| `Development/StudioRuns/Initialization-20261105/closeout-report-20261105.md` | 新增：本报告 |

引用链核验：5 份文档的 4 类相对链接（`../Development/...`、`../../Development/...`、同目录 Spec）目标文件均存在。

## 4. Git

| 项 | 值 |
|---|---|
| 分支 | `ai/rrpg-r3b-population-pilot` |
| HEAD | `054b00d3e38fe2e2e142bfdb2502de90f7991030` |
| 未提交条目 | 185（与开工时相同；新增文件落在既有未跟踪目录内，不改变计数） |
| 本轮新增修改 | 2 个新文件（字体取证、本报告）+ 5 份文档顶部横幅 |
| 是否保持原有工作区 | **是**：未 reset / clean / checkout / restore / add / commit / push；既有 dirty 文件未被覆盖 |

## 5. 遗留事项

**已解决**
- 默认初始化数量 1/1/5 经配置实测确认，并写入状态文档（制作人决策已记录）。
- 字体异常完成取证与备份，明确“不修”的理由与证据。
- 回执逐份复核（含 TRX 直接解析），排除文档中的过期“待验证”表述。

**尚未解决**
- 字体 +533B 的**触发者与逐字节差异**未证明；`StudioTestResourceScope` 保护机制已存在但**未接线到测试**，
  同类漂移可能第四次发生。
- 通用 Place 仍走 `neutral`；Farm 仍用带 TECH PROOF 文字的原图。
- 物理输入 / IME / DPI / Player 构建未验。

**需要制作人判断**
- 是否把 `f0f5b901… / 14,969,377` 接受为新的字体保护基线（接受 = 需同时决定是否补日志门禁）。
- A 档观感（5 房 + 带烟囱厨房 + 蓝色人物）是否达到“可识别”标准。
- 是否授权把 `StudioTestResourceScope` 接入 PlayMode 入口（属新的小范围开发）。

**后续建议**
1. 先接线字体保护门禁，再跑后续 PlayMode，避免继续无感漂移。
2. 若确需恢复字体，应在 Editor 关闭状态下用源 OTF + 受控参数重生成，并单独授权。

## 收尾状态

**技术收尾完成（Documented / Verification PARTIAL）**：
实现与验证证据闭合、文档已同步、Git 未提交；
**字体保护异常是唯一未闭合的保护项，因此整包保持 Partial，不得标记 Completed 或 Product Accepted。**
