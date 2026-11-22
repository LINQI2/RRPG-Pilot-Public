# 结果：产品体验修复补齐验证

日期 2026-11-05。分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3…`，未 Commit/Push。
**结论：A/B/C 三项的真实专项验证补齐；本包自身验收证据成立，Product Acceptance 仍 Pending。**

## 1. 先补的备份（上轮遗漏项）

`review-before/`（7 份完整字节 + `manifest.json`，含路径/字节/SHA），改前 SHA 前缀：

| 文件 | SHA256 前缀 | 字节 |
|---|---|---|
| ProductHomeSnapshot.cs | `844d1ad4603c` | 6,392 |
| ProductHomeReadAdapter.cs | `9ea50e9ded64` | 10,933 |
| ProductHomeWorldHost.cs | `f5ff39ac316a` | 15,839 |
| ManagementView.cs | `226f44b6aafb` | 44,527 |
| HomeProductShell.cs | `dd5db04b6014` | 22,780 |
| MainMenuUI.cs | `35a9f6637d0d` | 5,440 |
| VillageVisualPrototypePlayTests.cs | `fb4a10382742` | 30,505 |

说明：这补的是**本轮复核前**的字节。上一轮"改前未备份"的历史缺失仍记录在旧报告里，不宣称已修复。

## 2. 本轮最小代码改动（仅 1 处）

`HomeProductShell.SettingsSave`：加 `_lastSaveFrame` 同帧防重入 ——
同一帧内第二次触发（双击 / 同帧两次 Invoke）直接返回，不再发起第二次存档事务。
未改 SaveVersion / DTO / 存档格式；异步期间仍由既有 `_settingsBusy` 兜住。

## 3. 逐例真实结果（8/8 Passed，0 failed，0 skipped）

| 用例 | 时长 | 覆盖点 |
|---|---|---|
| `SettingsOverlayBlocksWorldPointerAndCloseRestores` | 476ms | 设置打开：屏幕点被全屏遮罩命中、`_townInput.blocksRaycasts=false`；关闭后不再由遮罩拦截、`blocksRaycasts` 恢复、`SurfaceModalActive=false` |
| `SettingsSaveIsSingleFlightPerFrameAndStaysOnFailure` | 479ms | 同帧两次点击只 1 次 Save 调用；失败留页并显示原因；跨帧可重试；失败不改人物数据/建筑视图；成功后菜单保持、不跳转 |
| `SettingsSaveFailureStaysAndSuccessReturnsToMainMenu` | 479ms | 保存失败/保存并返回失败均留在 Home；成功后返回真实主菜单 |
| `SaveAndReturnThenContinueKeepsIdentityAssignmentsAndResources` | 522ms | 保存并返回 → 真实 `MainMenuUI.OnContinue` → Town.Initialize → 同一 World 宿主重新激活；人物 ID / Assignment / 建筑 InstanceId / PlayerData 全部一致，无 reseed |
| `ResourceRowIsPlainAndViewWorkplaceNavigatesPrecisely` | 530ms | 资源行普通点击不切页；显式「查看岗位」按完整 WorkplaceId 定位且保留 Person 选择 |
| `AProfileHidesVirtualActivityPlaceButKeepsFactsAndBRoundtrip` | 481ms | A 档虚拟地点无视图；Place/人物/建筑不变；B 仍绘制；A→B→A 往返；Domain 事实不变 |
| `ActivityPlacesCannotReflowRegisteredWorkplaces` | 1137ms | 既有 B 语义回归（未削弱断言） |
| `ProfilesPreserveEntitiesFootprintsSelectionAndDomain` | 496ms | 既有 A/B 领域与布局回归 |

原始响应与逐例耗时：`artifacts/mcp-responses.md`。

## 4. 门禁记录（真实退出码）

| 段 | Label | begin | end | 结果 |
|---|---|---|---|---|
| 1 | `iw-productfix-review01` | 0 | **0（LEGITIMATE_GROWTH_ADOPTED）** | 测试新增「设置」等汉字被动态补入，门禁 v2 判为合法增长并自动采纳 → `e9c04e3d…` |
| 2 | `iw-productfix-review02` | 0 | 0（PASS） | 带截图复跑，字体未变 |

records：`font-protection/records/20261105T104242Z-…`、`…104523Z-…-end-growth.json`、`…104545Z-…`、`…104652Z-…`。
growth 记录：`font-protection/growth/20261105T104522Z-iw-productfix-review01.json`。
**历史 incident 两条未删**。

## 5. Compiler / validate（本轮真实回执）

- `rrpg-config validate --root <repo>` → **PASS**，errors=0、warnings=9（导航 Sheet），Mirror 未写入，exit 0。
- `dotnet test RRPG.ConfigCompiler.Tests --no-restore` → **280 通过 / 0 失败 / 0 跳过**，exit 0。
- 原始日志：`artifacts/compiler-verify.log`。
- 范围说明：本包只改 Unity 侧 C# 与测试，未动 XLSX/Mirror/生成物；上述为真实执行，非沿用旧 280。

## 6. 保护核验

| 对象 | 值 | 结论 |
|---|---|---|
| 用户单槽（本轮最新） | `56a8f2f1…` / 15,518 B | **未被替换成上轮 21,611B 旧值**；测试 guard 捕获并逐字节恢复 |
| `SampleScene.scene` | `570ab047…` | 未变 |
| 字体 | `e9c04e3d…`（review01 合法增长采纳后） | 与基线一致；review02 段 PASS |
| save-guard | 180 个 run，unfinished=0 | 全部 restored |

## 7. 证据文件

- 截图：`screenshots/settings-menu.png`、`screenshots/resources-explicit-view-workplace.png`、`screenshots/a-profile-virtual-place-hidden.png`。
- 原始 MCP 响应：`artifacts/mcp-responses.md`。
- Compiler 日志：`artifacts/compiler-verify.log`。
- 改前字节：`review-before/`（7 文件 + manifest）。

## 8. 未做 / 限制（不冒充）

1. **未**用真实活动调度跑"Tavern 出现/消失"：运行期未自然产生该类过渡，A 项仍以合成 Place 快照 + Domain 不变验证；
   真实 Activity/Presence 读取由既有 `ActivityPlacesCannotReflowRegisteredWorkplaces` 覆盖，但**不把合成快照当作调度验证**。
2. 资源页断言为"完整 WorkplaceId + 候选切页 + Person 保持 + 行视图在 _scrollToLocatedRow 时执行滚动"，
   **未逐像素核对滚动位置**。
3. 保存失败用 `ForceSaveFailureForTests` 接缝注入，未破坏真实用户存档。
4. 物理鼠标输入未验（程序化 `onClick.Invoke()` 与 EventSystem Raycast 属程序化路径）。
5. `HomeProductShell` 的 `_lastSaveFrame` 是"同帧"级防护，不是跨帧节流。

## 9. 状态

- **Implemented**：A / C / B + 同帧防重入。
- **Verified（本轮真实）**：8 例 PlayMode、Compiler 280/280、validate PASS、门禁两段、存档/场景/字体保护。
- **Product Acceptance Pending**：三项体验需制作人实机确认。
- **Git**：未提交、未推送；dirty 187（含本轮新增报告/测试改动）。
