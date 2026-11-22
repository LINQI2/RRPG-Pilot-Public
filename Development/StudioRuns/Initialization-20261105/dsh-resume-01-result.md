# DSH 续执行结果（Initial World，dsh-resume-01）

- 日期：2026-11-05；分支 `ai/rrpg-r3b-population-pilot`（未切换、未 Commit/Push）。
- 本轮在原 `dsh-result.md`（恢复前历史，保留不动）之后执行 A–D。
- 结论：**B（V4）与 D 的契约回归真实通过；C（默认 A 表现）已落地并被截图验证；A 档人物另有一处真实 bug 被修复。**
  仍需制作人肉眼验收，不宣称整包通过。

## 1. 实际改动（含前值/新值与备份）

| 路径 | 原因 | 前 SHA256 | 后 SHA256 |
|---|---|---|---|
| `Development/Config/RRPG_VisualAssets.xlsx` | 追加 3 行 A 档视觉资源 | `6641911b6223bf19a652f75b1d80facf4495abb1fbef0348cf56055acfb0d5ed` | `f5cb3518bad866330f89a6e59d7916900c99772efc97ff92eb05926a8efc4c6a` |
| `Development/Config/RRPG_VillagePresentation.xlsx` | 追加 5 行表现键 + 改 4 行建筑绑定 | `0ee4d28cfcf8f8e45094ab29aec8456d23ef8d56d7271384091ddd3594ea2ba9` | `847ffa6bc06f46d2a6012ee1caa5d9a6f417e2c785a948f3c9b74261c9e99300` |
| `Development/SourceAssets/Village/A-Tier/kitchen-a.png` | 新增 A 档白模图 | 无（新建） | `70eeccff19a9c1ee04f162904db82cffa1d793f7c515a6a2542a6a3dabe08c9e` |
| 同上 `house-a.png` | 同上 | 无 | `f9a36fe8b0f8ad6408f6a324ed6758f86b9e3bdf168b3c34357496ec18cd6112` |
| 同上 `person-a.png` | 同上（第二版强化剪影） | 无 | `8fad00bc3a88ec81863835ab6ae0d1b18be23576e35958a483fe3c1391eac90f` |
| `UnityProject/.../Village/ProductHome/HomePresentationResolver.cs` | 修 A 档人物表现键不被查表 | 见 §2.3 说明（原行内容见下） | `fdd5fa1231978ab1168c6fd2b34af7380e0e57f7f12099b570f9c84955b9eea5` |
| `UnityProject/.../Village/ProductHome/HomeWorldEntityView.cs` | A 档人物按 Sprite 等比显示 | 见 §2.3 | `868cbe1a9c5a2bb952a357718226b632523c65bbc300c7da90869cda0a5d393c` |

备份：两个 xlsx 的**原始字节**在 `Development/StudioRuns/Initialization-20261105/before-a-tier/`
（`RRPG_VillagePresentation.xlsx` 5458B、`RRPG_VisualAssets.xlsx` 7674B，SHA 即上表“前”值）。
原 `dsh-progress.md` / `dsh-result.md` 未改动；未删除任何历史证据。

正规生成物（由 Compiler / Unity Builder 写入，非手改）：
- `Development/Generated/StaticConfig/` 的 `VisualAsset.json`、`VillagePresentation.json`、
  `BuildingPresentationBinding.json`、`AssetManifest.json` 由 Compiler build 更新。
- `UnityProject/.../Assets/Generated/StaticConfig/Assets/Village/A-Tier/*.png(+.meta)` 与
  `Assets/Generated/StaticConfig/Resources/RRPGStaticConfigRoot.asset` 等数据库由 Unity Builder 重建。

新增脚本与日志：`Development/StudioRuns/Initialization-20261105/scripts/dsh-a-tier.ps1`、
`scripts/dsh-a-tier-person2.ps1`、`artifacts/a-tier-build.log`、`artifacts/a-tier-person-v2.log`。

## 2. Unity 连接与工具实际执行证据

### 2.1 通道（不是 Unity MCP 客户端工具，而是既有 HTTP 通道）

- 本会话工具面仍**只有 `codex_bridge`**，没有 Unity MCP 客户端工具。
- 但工程内既有配置 `UnityProject/RRPG1.0game/UserSettings/AI-Game-Developer-Config.json` 指向
  **`http://localhost:21867`**（streamableHttp）。实测 `initialize` 返回
  `serverInfo: gamedev-mcp-server 9.2.13`，工具集 `rql-query / rql-mutate / rql-schema /
  script-execute / tests-run / screenshot-isolated / unity-tool-list`。
  本轮全部 Unity 操作经该**已配置通道**完成，未安装/升级 Package，未改认证或连接器。

### 2.2 Editor 现场（`script-execute` 实取，非目录推定）

```
projectPath=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game
unityVersion=2022.3.62t9  productName=RRPG_AI_Pilot
isPlaying=False isCompiling=False isUpdating=False
activeScene=Assets/Scenes/SampleScene.scene  sceneDirty=False  sceneCount=1  prefabStage=False
persistentDataPath=1PUBLIC_USER/AppData/LocalLow/DefaultCompany/RRPG_AI_Pilot
saveDirFiles=1 [rrpg_save_p0.json 21611B]
```

Console 基线唯一 error 经反射取出为
`[1] The message header is corrupted and for security reasons connection will be terminated.`
（既有 IPC 噪声，非产品错误）。改动前后 `err` 计数均为 0（该条已被 Console 清理）。

### 2.3 发现并修复的真实缺陷

A 档下人物始终显示同一个中位标记。根因在 `HomePresentationResolver.TryResolve`：

```csharp
else if ((PrototypeEnabled || presentationKey.StartsWith("village.building.")) && !TryBuildingAsset(...))
```

条件只让 `village.building.*` 查表，`village.person.*` 直接跳过，`assetId` 保持默认
`va_village_neutral_marker`。因此**即使配置里有人物表现键也不会生效**。
修复：条件追加 `|| presentationKey.StartsWith("village.person.")`（未动 place 分支，保持最小面）。

`HomeWorldEntityView.ApplyPerson` 另有 A 档分支把人物固定为 `0.38×0.62` 压扁 + 米色 tint，
新增 `!_prototype` 分支：按解析到的 Sprite 等比缩放（高度 1.05 世界单位）并恢复白色 tint；
不改身份、碰撞体与布局。

### 2.4 配置生成链命令与结果（真实回执）

经 Editor 进程（非沙箱 token）执行，避免沙箱只允许根目录写入的限制：

```
rrpg-config.exe build    --root D:\AI_Agent_Lab\rrpg_pilot\repo   → exit 0，result=PASS errors=0 warnings=9
rrpg-config.exe validate --root D:\AI_Agent_Lab\rrpg_pilot\repo   → exit 0，result=PASS errors=0 warnings=9，
                                                                     Mirror 未写入（validate 只读）
```

build 明确记录 `updated Generated/VillagePresentation.json`、`Generated/VisualAsset.json`、
`Generated/BuildingPresentationBinding.json`、`Generated/AssetManifest.json`；其余 40 个镜像 unchanged。

Unity Builder（`StaticConfigBuilder.ExecuteBuild`，在运行中的 Editor 内执行）：
`result=PASS noOp=false`，逐条记录
`managed asset(updated): Assets/Generated/StaticConfig/Assets/Village/A-Tier/{house,kitchen,person}-a.png`、
`visual asset catalog: entries=17`、`updated Resources/RRPGStaticConfigRoot.asset`。

解析回读（运行时 API）：

```
village.building.kitchen.basic → va_village_kitchen_a (sprite=kitchen-a 300x200)
village.building.house.basic   → va_village_house_a   (sprite=house-a 300x200)
village.person.onsite/.away/.traveling → va_village_person_a (sprite=person-a 300x200)
bindKitchen=village.building.kitchen.basic  bindHouse=village.building.house.basic
```

## 3. 测试执行（全部真实运行，逐项结果）

| # | 测试 | 通道/参数 | 结果 | 时长 |
|---|---|---|---|---|
| 1 | `VillageV4ResourceSurplusPlayTests`（整类 1 例） | `tests-run` PlayMode，class 过滤 | **1/1 Passed，0 failed，0 skipped** | 2.09s |
| 2 | 初始化/接线 EditMode（`InitialWorld*` 3 个 Fixture） | `tests-run` EditMode，query 过滤 | **24/24 Passed，0 failed，0 skipped** | 1.78s |
| 3 | `InitializationProductPlayTests` 2 例（产品入口 + Kitchen 真 UI 派工/加工） | `tests-run` PlayMode，query 过滤 | **2/2 Passed**，共跑 4 次均通过 | 3.4–3.9s |

- 无 0 项运行、无 skip、无失败、无中止。
- V4 用例内的 `SaveSlotGuard` runId `20261105T045428Z`（12:54:28–12:54:30），
  `sourceSha256=3ed26a77…`、`readbackVerified=true`、`restored=true`。
- EditMode 用例名与结果见运行输出，例如
  `NewGameIsSingleShotAndRepeatableWithoutDuplicateInstances`、
  `ContinuePreservesInstanceIdentityPlacementAssignmentAndResources`、
  `LegacySaveWithoutBuildingsDoesNotRebuildInitialWorldAndKeepsStaticAssignments` 均 Passed。

## 4. NewGame → 派工 → 生产 → Save → Continue 证据

V4 用例执行日志（`Temp/mcp-server/ai-editor-logs.txt`，12:54:30）：

```
初始：Ingredients=0 Food=0 在村 Person=0
Day1: InVillage=6 执行事实=2 | Farm→Ing=12 Kitchen→Food=10(耗Ing=10) | Food消费=5(未满足3) | Ing 0→1 Food 0→5
Day2: InVillage=6 执行事实=2 | Farm→Ing=12 Kitchen→Food=12 | Food消费=6(足额6) | Ing 1→1 Food 5→11
Day3: InVillage=6 执行事实=2 | Farm→Ing=12 Kitchen→Food=12 | Food消费=6(足额6) | Ing 1→1 Food 11→17
Jump To Next Month：Food 17 → 185（Ingredients=1）
```

同用例随后完成真实出征（Carry 扣减对账）、Journey 可见性、`RequestRetreatFromProgression` 收尾。

产品入口用例证据日志（`Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-play-evidence.log`）：

```
[SAVE] sha256=9ed4a38… buildings=building_instance_1;…
[CONTINUE] sameBuildings=True buildings=…;…（实例 ID 与顺序不变）
[END] productEntry=True managementAssign=True labor=True saveContinue=True noReseed=True
      housingNoWorkplace=True workshopRejected=True
[SAVE-RESTORE] runId=20261105T050155Z existed=True sha256=3ed26a77… matches-before=True
[SCHEDULE-RESTORE] sameInstance=True
```

## 5. 保护核验（测试前 → 全部完成后）

| 对象 | 测试前 SHA256 | 全部完成后 SHA256 | 结论 |
|---|---|---|---|
| 用户存档 `rrpg_save_p0.json` | `3ed26a77…e3f6`（21611B） | `3ed26a77…e3f6`（21611B） | **一致**，无 .tmp 残留 |
| `SampleScene.scene` | `570ab047…c72c`（846770B） | `570ab047…c72c`（846770B） | **一致**（mtime 仍为 10-23） |
| `RRPG_SourceHanSansCN_Regular_TMP.asset` | `9ada5362…2d5`（14968844B） | `f0f5b901…681`（14969377B） | **不一致，见下** |

save-guard：测试前 103 个 run，全部完成后 133 个 run，逐条读 `manifest.json`，
**unfinished=0**（全部 restored）。测试结束时代码内 `[FONT-POST] unchanged=True`。

**字体异常（必须如实登记）**：13:00:28 的 FONT-PRE/POST 仍是 `9ada5362…`；13:01:53 的 FONT-PRE
已变成 `f0f5b901…`（+533B），此后未再变化。时间落在本会话修改 C# 后执行
`AssetDatabase.Refresh(ForceUpdate)` 与下一轮测试之间，最可能由 TMP 资产重新序列化导致，
而非测试写入失败（该轮自身 pre/post 一致）。**未做语义比对**，建议 Codex 用 git diff 核对或从版本库回退。

## 6. 默认 A 表现证据

- 截图：`Development/StudioRuns/Initialization-20261027/play-evidence/initialization-product-save-continue-20261105T050547Z.png`
  （1920×1080，sha256 `45d93b25…`；同一路径下 `…045027Z`、`…045156Z`、`…045351Z`、`…045449Z` 为修复过程留证）。
- 画面内容（默认 A，"R2 对照：A 白模" 仍高亮）：5 座住房=陶土屋顶小屋，厨房=赭色屋顶+烟囱，
  农田=`FARM A / TECH PROOF` 绿色田垄，5 名在村人物=蓝色人形剪影+浅色头部，全部互不相同。
- 制作人体验入口：Editor Play → 进入村庄 Home 画面即可看到；右下角 `R2 对照：A 白模` 按钮保持 A 为默认，
  切 B 仍走原 R2 原型，A/B 不共用同一套图。

## 7. 状态分层

- **Implemented**：A 档 3 张白模图 + 视觉资源/表现键/建筑绑定配置；A 档人物查表缺陷与显示尺寸修复。
- **Verified**：V4 1/1、初始化 EditMode 24/24、产品 PlayMode 2/2（多次复跑一致）、Compiler build+validate PASS、
  Unity Builder PASS、A 档截图四类实体可区分、存档与场景逐字节未变、guard 0 未完成。
- **Product Acceptance Pending**：A 档观感是否达到制作人“可识别”标准；B 档对照未被破坏需肉眼确认。
- **Deferred**：A 档 place 类实体（`village.place.*`）仍走中位标记，本轮未动；字体字节差异待 Codex 裁决。

## 8. 未解决与需人工介入

1. **字体资产字节变化**（+533B，`f0f5b901…`）：需 Codex 用版本库核对/回退；本会话不擅自回写。
2. **A 档 place 表现**：`village.place.Home/Workplace/…` 仍无 A 键，非建筑承载的 place 仍是中位标记。
   若制作人认为这仍影响“可识别”，需再授权一轮（同样走 XLSX → Compiler → Mirror）。
3. 本会话无 Unity MCP 客户端工具，全部经 HTTP 通道；若后续需要 `console-get-logs` 等未注册工具，
   仍需该通道或由主开发执行。
