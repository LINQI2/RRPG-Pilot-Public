> 后续事实：DSH两轮未执行；主开发已用Computer Use完成本次正常入口运行并归位原档。steps/protection.json及Roadmap正常路径报告为最新结论；以下预算停点为旧执行端历史，不是产品阻塞。

# FP-V0.1 正常路径核验：进度（开工基线 + 停点）

日期 2026-11-06。**本轮停在保护前置，未封存存档、未进 Play、未运行任何 G6/G7 步骤。**

## 1. 真实开工基线（本轮实测，非模板）

```
HEAD        223a08a3d1c45cc5498a7b453cd1b9f4675908d4
branch      ai/rrpg-r3b-population-pilot
staged=0    dirty=129
Editor      进程存在，localhost:21867 监听中
```

## 2. 用户单槽（只读，未写入）

```
目录  1PUBLIC_USER\AppData\LocalLow\DefaultCompany\RRPG_AI_Pilot
文件  rrpg_save_p0.json   28,994 B   mtime 2026-11-06 21:32:17
SHA256 89625eef3ce43cb02d55dcc5d93b74c1e46acf0b30a49ceda903cb3e1c424948
.tmp  目录内未见 .tmp 残留（仅该单文件）
```

## 3. 为什么在此停住（不启动封存）

工作包要求：封存原档 → 从正式 NewGame 建立隔离测试档 → 走 G7/G6 正常路径 → finally 原子归位并复核 SHA。
这是一个**不可中断的序列**：一旦把 `rrpg_save_p0.json` 原子改名为 `.fp-audit-original-<runid>`，
在原档归位前，若中途无法继续，用户的原存档就处于"被移走"状态。

本轮可用执行预算不足以安全走完「封存 → 建立测试档 → 多段 Play 导航与截图 → 退出 Play → 移出测试档 → 归位 → 复核」，
尤其 G7 需要多次进出 Play、逐步 UI 导航与证据采集。**与其做一半留下被移走的用户档，不如不开始。**
这也符合工作包"任何异常优先保护原档并明确停点、不谎称已隔离"的要求。

## 4. 进入正式执行前必须先做的三件事（下一轮开场即做）

1. **确认 Editor 不在 Play、无写入占用**（需在字体门禁 `begin` 成功后只读查询 `isPlaying`）；
   若在 Play，按工作包不擅自退出，只记录状态并停止。
2. **字体门禁**：`begin` 成功才可进入；`end` 非 0 立即停止并保留 incident。
3. **封存清单**：原档路径/字节/SHA/`.tmp` 已在上表；封存前再取一次 SHA，改名后回读新路径 SHA 必须一致。

## 5. 未执行项（明确）

- 未封存/未改名/未建立测试档；未 NewGame；未 Play；未截图；未跑 G6/G7 任一步；
- 未读 Roadmap / Evidence Index / 9/7 Checkpoint / Expedition-Person-Battle-Resource-Save 规则；
- 未产生 steps.json / gate-matrix.json / protection.json（数据尚未采集，不预填）。

## 6. 已知风险与需制作人裁决

- 若要求"本轮必须有 G7 结论"，需要一次性给足执行预算（含多段 Play 与截图），否则只能继续停在基线；
- 若允许**跳过封存**、仅以只读方式审查入口与配置（不做真实 NewGame 运行），则可先产出"入口是否接通"的静态结论，
  但那是另一类证据，不能算 G7 通过——需明确选择其中之一。
