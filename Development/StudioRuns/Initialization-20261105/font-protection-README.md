# TMP 字体保护门禁（v2：接受合法 Dynamic Atlas 增长）

制作人 2026-11-05 决策：**接受合法动态字形增长，不做静态字体替换，不做运行时实例隔离。**
门禁从"哈希必须相等"升级为"结构判定"，区分三种结果：

| 结果 | 含义 | 退出码 | 处理 |
|---|---|---|---|
| `PASS` | 与基线逐字节一致 | 0 | 正常放行 |
| `LEGITIMATE_GROWTH` | 仅追加字形/字符的合法增长 | 0 | **自动记录并更新基线** |
| `INCIDENT` | 结构性破坏 | 2 | 停止 Unity 流程，保留证据 |
| `NEEDS_REVIEW` | 无法可靠判断（解码失败/非 Dynamic/格式未知） | 4 | 停止，交人工判断 |

## 合法增长的判定条件（全部满足才通过）

1. 文档块数量不变（`font` + `material` + 7×`texture`），字体块外块长不变。
2. 字符表：旧表是新表的**严格前缀**（Unicode 与 GlyphIndex 都不变）；数量只增不减。
3. 字形表：旧字形记录**逐字节不变**；数量只增不减；字符数 == 字形数。
4. 每个新增字符都能在**新增字形**里找到对应 GlyphIndex。
5. `Material` 块、`m_FaceInfo`、`m_SourceFontFileGUID` 完全不变；`m_AtlasPopulationMode` 仍为 Dynamic(1)。
6. 图集引用：旧引用顺序不变，只允许尾部追加新页；不允许删除/重排。
7. 图集像素：**新增字形矩形（含 12px 栅格化余量）之外必须逐字节不变**；新增矩形内必须有非零像素。
8. 新增矩形不得与任何旧字形矩形重叠（防重排/覆盖）。
9. 任何一项无法判定（解码失败、格式非 Alpha8、解析失败）→ `NEEDS_REVIEW`，不自动接受。

不满足以上任一条件 → `INCIDENT`，不会因为"大小或字符数变了"就放行。

## 文件

| 路径 | 作用 |
|---|---|
| `scripts/font-guard-core.ps1` | 纯函数：解析 + 判定（可被自测直接调用） |
| `scripts/font-guard.ps1` | CLI：capture / verify / begin / end / status / selftest / classify |
| `scripts/font-gate-selftest.ps1` | 自测：历史正样本 + 七类异常样本（全部在临时副本上） |
| `font-protection/baseline.json` | 当前基线 |
| `font-protection/baseline/*.asset` | 基线完整字节备份 |
| `font-protection/growth/` | 每次合法增长记录（新增字符、字形数、页变化、前后哈希） |
| `font-protection/history/` | 被替换下来的旧基线字节（按 SHA 命名） |
| `font-protection/incidents/` | 异常证据（历史 incident **永久保留，不删除**） |
| `font-protection/records/` | begin/end 记录 |

## 用法（不变）

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\font-guard.ps1 begin -Label <名字>
#   ... Unity 流程 ...
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\font-guard.ps1 end   -Label <名字>
```

`begin`/`end` 遇到合法增长会自动采纳并继续（exit 0），遇到 `INCIDENT`/`NEEDS_REVIEW` 立即停止（exit 2/4）。
`status` 只读；`verify` 与 `end` 等价判定。

## 自测

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\font-gate-selftest.ps1
```

期望：`TOTAL=9 PASSED=9 FAILED=0`（2 个历史合法增长 + 7 个异常样本全部拦下）。

## 当前状态

基线：`32b87c67…` / 14,973,886 B（2026-11-05 合法增长采纳，含 656 字符）。
历史 incident 两条保留：`20261105T080341Z-iw-productfix-step03`、`20261105T100237Z-iw-productfix-step05-tests`。
