# TMP 字体资产 +533B 异常：取证与结论（2026-11-05）

范围：`UnityProject/RRPG1.0game/Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset`
性质：**只读取证 + 安全措施**。本轮未修改、未回退、未重建该资产，未执行 `git checkout/restore`。

## 0. 结论（先给结论）

1. **未修复，也不建议按现状修复**：接手时的字节在其他位置不存在副本，Git HEAD 是更旧的构建，回退会丢内容。
2. **未发现内容丢失**：资产当前可正常加载，9 个子资产、648 字符/648 字形闭合、7 张 1024×1024 图集、`isDirty=false`。
3. 变化形态与**动态图集追加字形后被持久化**一致（字符数从 2026-10-23 记录的 646 增至 648），但**没有前字节无法证明**具体差异。
4. 因此：**保留当前文件 + 保留备份 + 记录未解决**。是否接受新哈希为新基线，属制作人决策。

## 1. 当前事实（本轮实测）

| 项 | 值 |
|---|---|
| 大小 | 14,969,377 B |
| SHA256 | `f0f5b901a5ef56a68450803e39510846ccfc38623ab0d3c8654b22f33bcf1681` |
| 文件 mtime (UTC) | 2026-11-05T05:01:47.3537285Z（北京时间 13:01:47） |
| 备份 | `Development/StudioRuns/Initialization-20261105/review-backups/font-after-dsh.asset`（14,969,377B，SHA 同上，已回读比对一致） |

**任务简述中的 “4,968,844 B” 应为笔误**：与本轮全部实测记录（`dsh-resume-01-result.md`、
`reviewer-original-changes.json`、`technical-review.md`）一致的数字是 **14,968,844 B**。
两者相差恰好一位前导“1”，本报告按 14,968,844 处理。

## 2. 变化链（同一文件的四次已知状态）

| 时间 | 大小 | SHA256 | 相对上一状态 |
|---|---:|---|---:|
| 2026-10-22（R3-B 基线） | 14,961,033 | `3c758e16…c404854` | — |
| 2026-10-23 12:30:59Z（已记录事故） | 14,968,490 | `e40b6d77…7556f2d8` | +7,457 |
| 2026-11-01 05:36:25Z（本轮接手前） | 14,968,844 | `9ada5362…b1c22d5` | +354 |
| 2026-11-05 05:01:47Z（本轮观测） | 14,969,377 | `f0f5b901…3bcf1681` | +533 |

前两次变化的取证见 `Development/StudioRuns/R3B-UI-20261023/font-protection-incident-review.md`。
可见这是该资产的**第三次**同类漂移，不是偶发。

## 3. 与 Git HEAD 的关系（为何不能回退）

- HEAD 版本：12,826,076 B，SHA `ba75bbc2fcc8a4f7f08079caf0e84df22591eab55a29315d0e0fd1be16451038`。
- 比当前**小 2,143,301 B**，是更早的石墨构建；它与 10-22 基线、接手前状态**都不相等**。
- 该文件在接手前已是 dirty（`reviewer-original-changes.json` 记录 before=`9ada5362…`）。
- 因此 `git checkout/restore` 会同时覆盖既有未提交成果并丢失约 2.1 MB 字形/图集内容 —— **禁止**。

## 4. 结构核验（运行时读取，非猜测）

通过既有 Unity 通道 `script-execute`（只读）：

```
subAssets=9
names=[TMP_FontAsset:RRPG_SourceHanSansCN_Regular_TMP;
       Material:…Material;
       Texture2D:…Atlas; Atlas 1..Atlas 6]     ← 7 张图集
chars=648  glyphs=648  atlasPopulationMode=Dynamic(1)
atlas=1024x1024  fallbacks=0  isDirty=False  version=1.1.0
```

- 字符表末 16 个字为 `转“”按显›令功既兼供餐导航例员`，其顺序符合“按渲染使用顺序追加”的特征。
- 2026-10-23 的取证报告记录当时为 **646 字符 / 646 字形**；现在 648。两次漂移合计 +2 字形，
  与“渲染时动态补字形、随后被落盘”的**追加型**变化一致；没有任何“字符被删除/替换”的迹象。

## 5. 机制分析（已知 / 推断 / 未知）

**已知**

- 该字体是 `m_AtlasPopulationMode: 1 (Dynamic)`，缺字时由 TMP 在运行时补进图集并置 dirty。
- 2026-10-23 事故的怀疑路径是 `GrowthDataInitializer` 每次域重载后 `AssetDatabase.SaveAssets()`；
  **该路径现已不存在**：当前代码第 100 行明确注释“禁止全局 SaveAssets：动态 TMP 字体…”，
  只调用 `AssetDatabase.Refresh()`。
- 工程内唯一涉及字体写入 API 的脚本是 `Assets/Editor/StudioTestResourceScope.cs`
  （快照→恢复→校验，不新增字形），且全工程没有任何测试引用它 → **该保护机制尚未接线**。
- 本轮时间线：13:00:27 产品 PlayMode 用例渲染中文姓名/UI（会触发动态补字）→ 13:01:1x 修改 C# 后
  `AssetDatabase.Refresh(ForceUpdate)` → 13:01:47 字体 mtime 前进 → 13:01:53 下一轮测试 FONT-PRE 读到新哈希。

**推断（中等置信，未证明）**

- 写入发生在“Play 渲染补字 → 编辑器刷新/重载期间落盘 dirty 资产”这一区间，属编辑器自身资产管线行为，
  而非测试代码或人工显式写字体。

**未知（不猜）**

- 13:01:47 那次写入的**具体触发者**：无逐写日志，Editor 主日志不在授权范围。
- 533 字节的**具体字段差异**：无前字节，无法做字节级 diff。

## 6. 数据丢失风险评估

| 风险 | 判断 | 依据 |
|---|---|---|
| 字符/字形丢失 | **未发现** | 648 字符 = 648 字形闭合；相对 10-23 的 646 为净增 |
| 图集像素损坏 | **未发现** | 7 张 1024×1024 图集引用完好，资产加载无报错 |
| 引用/子资产断裂 | **未发现** | 9 个子资产映射稳定（Material + 7 Atlas + 字体本体） |
| 序列化截断 | **未发现** | 文件可被 TMP 1.1.0 正常解析，`isDirty=false` |
| 精确还原能力 | **已丢失** | 14,968,844B 的副本在 `D:\AI_Agent_Lab` 全盘检索 0 命中 |

## 7. 为什么不修

- 修复 = 写回 14,968,844B，但**该字节不存在**；用 HEAD（12.8MB）或 10-23 版本（14,961,033B）替换都属于
  “用另一份非基线字节覆盖当前成果”，会引入新污染并丢失约 2.1 MB 的既有字形/图集增量。
- 手工删 YAML 片段以“凑回”大小属于无依据改写，风险高于收益。
- 现有保护机制（`StudioTestResourceScope`）具备快照/恢复/校验能力，但未接入测试；
  在它未接线前做任何写入都没有兜底。

## 8. 建议（需制作人/主开发决策）

1. **不回退**，保留当前文件与备份；等待制作人决定是否把 `f0f5b901… / 14,969,377` 接受为新保护基线。
2. **接线保护**：把 `StudioTestResourceScope`（已存在于 `Assets/Editor/`，当前 untracked）接入
   PlayMode 入口，或至少在测试前后做一次 SHA 门禁，避免第四次无感漂移。
3. **长期修复**：如需彻底消除，应由主开发在 Editor 关闭状态下，用源 OTF + 受控设置重新生成该字体资产，
   并记录生成参数；但那会产生第四份哈希，须在明确授权下进行。

## 9. 状态更新（2026-11-05，制作人确认后）

制作人已确认采用**门禁方案**，建议 1、2 已落地：

- 现行字体**已采纳为新基线**（`font-protection/baseline.json`，SHA `f0f5b901…` / 14,969,377 B），
  附完整字节备份；历史三态仅作溯源，不作为可恢复版本。
- 门禁 `scripts/font-guard.ps1`（capture/verify/begin/end/status/selftest）已实现并自检通过：
  单字节翻转被捕获（exit 2 + incident 证据），未改动时 exit 0。
- 真实流程窗口验证：`Refresh + StaticConfigBuilder` 与 `PlayMode 产品 2/2` 均 begin→end PASS。
- 策略固定为：**不自动恢复、不做 Git 覆盖、不静默接受变化**；发现漂移立即停止并取证。

仍未改变的事实：**写入成因未定位**，因此本条从「未解决」变为「**受控 + 可检测**」，
整包状态仍为 Partial，不因门禁上线而宣称完成。详见 `font-protection-README.md`。
