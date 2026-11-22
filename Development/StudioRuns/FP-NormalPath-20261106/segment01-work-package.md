# FP正常路径 Segment01：仅 NewGame 到第一次Home，保护完结

本任务延续dsh-work-package.md原授权，不需要制作人另选A/B或批准预算。先前progress的预算理由是执行安排问题，不是产品/权限阻塞。分段执行即可，每段都安全归位；无需一轮内完成G6/G7。严禁因为预算泛称重大阻塞。

唯一目标：本段用**现有正式UI**从MainMenu→NewGame→Hero选择/Confirm→记录实际落点（Tutorial? Home?）。一旦到Home或首个真实阻塞，立即收尾，**本段不继续远征、不进战斗**，通常少量UI操作即可。

保留原工作包所有禁止事项和字体begin/end。若开始时用户Play则不退出、记录实情。先只读确认Pilot项目与isPlaying=false，备份并SHA核验当前真实用户存档；同目录原子改名封存原档（存在则唯一原名 `.fp-audit-original-segment01`，禁止覆盖任何旧文件，封存后槽为空），原文件本体不得写。原.tmp预先存在则先停止而非删改。封存后开始Play，只点击实际可见正式按钮；不用隐藏Handler/直接NewGame/强改数据。

到达正式落点时立即截图并只读记录入口层级、人物ID、界面状态和是否Tutorial；若可见设置菜单正常保存则由正式UI保存本次测试世界用于下一段，否则记录新游戏自身建立的测试槽即可。**不要为了段落继续写用户存档**，用户原档仍在封存路径。

finally退出本次Play（不保存Scene/资产）；把生成的测试槽**移动**到本目录 `segment01-test-save.json`（目标不存在才移，禁止覆盖旧证据），确定原槽空，再把原档**原子归位**，SHA/字节核对一致，目录无测试残留；无原档时槽恢复不存在。字体end退出码非0停止，incident保留。不得以File.WriteAllBytes覆盖恢复用户原档。若封存/原子归位不可用，先停止任何Play/Save，给具体错误，不猜权限、不修ACL。

交付本目录 segment01-result.md / segment01-protection.json / segment01-steps.json / segment01-raw.md / screenshots/。必须完成保护归位再结束。本段只判断NewGame实际去向与G7前缀，后续由主开发另派独立段；不用再次问制作人预算，不假称整G6/G7通过。普通技术问题自行安全处理。
