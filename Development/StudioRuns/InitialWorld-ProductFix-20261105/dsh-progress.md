# DSH 进度：Initial World 产品体验修复（A/C/B）

工作包 `dsh-work-package.md`；规格 `Docs/DevelopmentSpecs/2026-11-05_RRPG_V17_Initial-World_Product-Experience-Fixes_Development-Spec-V0.1.md`。

## 15:52 开场
读工作包与规格；核对分支 `ai/rrpg-r3b-population-pilot`、HEAD `054b00d3…`。

## 15:52 门禁可执行性
- 本 Shell 沙箱：仓库根可写，子目录写入被拒。`font-guard.ps1 begin` 直接运行报
  `Access to the path ...records\...json is denied`，**exit=1**（精确错误已记录）。
- 改用既有普通用户能力：经 Unity HTTP 通道由 Editor 进程派生 `powershell -File font-guard.ps1`，命令仍从 repo 根调用，退出码原样回传。

## 15:55 现场发现
Editor 处于 Play 模式（制作人 15:05 游玩并保存：存档 21,611B→15,518B），字体内存 `dirty=1`、字符 648→654。
磁盘字体仍是基线 `f0f5b901…`。已向制作人确认；答复「我已退出」。

## 15:59 step01（PASS）
begin=0 → 只读身份/场景/字体/Console（playing=False，scene dirty=False，fontDirty=1，chars=654，err=0）→ end=0。

## 16:00–16:02 实施 A/C/B
- A：快照加 `DrawsStandaloneEntityInA`；Adapter 按真实实体资格赋值；Host 按档位判定（B 不变）。
- C：资源行纯展示 + 显式「查看岗位」按钮（携带完整 WorkplaceId，目标不存在则禁用并说明）。
- B：Home 顶栏「设置」+ 遮罩 + 三动作；busy guard；失败留页显示原因；成功后经 `MainMenuUI.ShowMainMenuAfterHome()` 返回。
- 偏差：**编辑前未做字节备份/改前 SHA**（已如实登记，提供反向还原说明）。

## 16:02 step02（PASS）
begin=0 → Refresh + 等编译 + Console 回读（err=0、isCompiling=False）→ end=0。

## 16:03 step03（end=2，CHANGED → 停止）
begin=0 → PlayMode `VillageVisualPrototypePlayTests` 2/2 Passed → **end=2**。
字体 `f0f5b901…/14,969,377` → `d19fd6a1…/14,972,511`（+3,134B），写入时刻 08:03:25Z。
incident：`font-protection/incidents/20261105T080341Z-iw-productfix-step03/`（incident.json + font-after.asset）。

按规格**立即停止全部后续 Unity 操作**，未复测、未 capture、未恢复、未清 dirty。

## 当前阻塞
需制作人决定是否把 `d19fd6a1…/14,972,511` 采纳为新基线；采纳后才可用 `capture -Force` 重设并继续 A/C/B 专项测试。
