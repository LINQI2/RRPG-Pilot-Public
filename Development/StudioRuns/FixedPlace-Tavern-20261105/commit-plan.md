# Tavern 提交清单与最小完整依赖（供授权后立即执行）

日期 2026-11-05。分支 `ai/rrpg-r3b-population-pilot`，HEAD `054b00d3…`，暂存区空，dirty 195。

## 1. 只属于 Tavern 的新增文件（可独立提交，12 个）

```
Development/SourceAssets/Village/A-Tier/tavern-a.png (+ .meta 若已生成)
UnityProject/RRPG1.0game/Assets/Tests/PlayMode/Village/TavernFixedPlacePlayTests.cs
Development/StudioRuns/FixedPlace-Tavern-20261105/artifacts/tavern-build.log
Development/StudioRuns/FixedPlace-Tavern-20261105/before/RRPG_VillagePresentation.xlsx
Development/StudioRuns/FixedPlace-Tavern-20261105/before/RRPG_VisualAssets.xlsx
Development/StudioRuns/FixedPlace-Tavern-20261105/closeout-handoff.md
Development/StudioRuns/FixedPlace-Tavern-20261105/dsh-result.md
Development/StudioRuns/FixedPlace-Tavern-20261105/screenshots/a-fixed-tavern.png
Development/StudioRuns/FixedPlace-Tavern-20261105/scripts/*.ps1   (5 个)
```

这些文件提交后**单独不构成可运行变更**：测试引用 `IsFixedPlace` 与 `village.place.fixed.Tavern`，
两者都在下面的共享文件里。

## 2. 共享代码文件（必须一起提交，且含其他包内容）

| 文件 | 改动行 | 其中 Tavern | 其中 Initial World(A/C/B) |
|---|---:|---:|---|
| `ProductHomeReadAdapter.cs` | +141 −9 | `ApplyFixedPlaces`（约 35 行）+ 固定场所补全调用 | `DrawsStandaloneEntityInA` 赋值与注释（约 100 行） |
| `ProductHomeSnapshot.cs` | +26 −2 | `IsFixedPlace` 属性 + 构造参数 | `DrawsStandaloneEntityInA` |
| `HomeWorldEntityView.cs` | +20 −2 | `ApplyPlace` 固定场所白色分支 | A 档人物等比缩放 |
| `ProductHomeWorldHost.cs` | +14 −3 | `ShouldDrawPlace` 的 `IsFixedPlace` 短路 | A 档原判定 |
| `HomePresentationResolver.cs` | +3 −1 | 固定场所档位键 + `village.place.` 查表 | `village.person.` 查表 |

**无法只取 Tavern 部分**：`ApplyFixedPlaces` 使用的新构造签名本身来自 A 包改动；
只提交 Tavern 侧会编译失败。

## 3. 配置源与生成物（二进制，无法拆分）

| 文件 | 结论 |
|---|---|
| `Development/Config/RRPG_VillagePresentation.xlsx` | 二进制；同时含 A 包 5 个键与 Tavern 2 个键 |
| `Development/Config/RRPG_VisualAssets.xlsx` | 二进制；同时含 A 档 3 个资源与 `va_village_tavern_a` |
| `Development/Generated/StaticConfig/{VillagePresentation,VisualAsset,AssetManifest}.json` | 由上面两本生成，同样两包并存 |

`git diff --numstat` 对两本 XLSX 输出 `-  -`（binary），**不能按 hunk 暂存**。

## 4. 共享测试与文档

- `VillageVisualPrototypePlayTests.cs`（M）：A/C/B 的 3 个用例 + 本轮把合成地点改名 `VirtualSpot`。
- `Docs/项目状态快照.md`、`Docs/项目日志.md`、`Docs/文档地图.md`、`Docs/DevelopmentSpecs/README.md`、
  检查点 / V17 Spec / Product Home GameSpec / Building World GameSpec / `07 Village` / `当前设计状态`：
  顶部同时含 Initial World、字体门禁、Tavern 多条横幅。

## 5. 三种可执行方案与代价

| 方案 | 提交内容 | 代价 |
|---|---|---|
| **A. 一次性合并提交（推荐）** | 上述 1+2+3+4+文档，一条 commit，消息写明两包范围 | 一条 commit 覆盖两包归属；需授权 |
| B. 先提 A/C/B，再提 Tavern | 两条 commit，第二次只含 Tavern 新增文件 + 已无共享改动的代码 | 需先为 A/C/B 授权提交；两次都要门禁复核 |
| C. Tavern commit 附带 A 包代码/配置行 | 一条 commit，消息注明"含 Initial World 配置行 N 处" | 归属混写，review 时易误判 |

**结论：不存在"纯 Tavern、且可运行"的独立提交。** 最小完整依赖 = §1 + §2 + §3（§4 文档可选）。
执行命令（以 A 为例，逐路径，不用 `add .`/`-A`）：

```powershell
git add <§1 的 12 个路径> <§2 的 5 个路径> <§3 的 5 个路径> <Docs 的 10 个路径>
git commit -m "feat(village): Tavern as Fixed World Place + Initial World product fixes"
```

## 6. 需要的许可

一句话即可：**「按 A 合并提交」** 或 **「按 B 先提 A/C/B」**。得到后我立即执行、给出 SHA 与提交后 `git status` 核验。
在许可之前保持 `Product Accepted / Closeout Prepared / Commit Pending`，不做任何暂存。
