# FP-V0.1 正常玩家路径核验：G6/G7

日期2026-11-06。制作人已明确授权调查/必要运行/隔离测试存档；禁止代码玩法修改、用户档覆盖、非必要资产/场景保存、Push/Merge/ACL、扩展工具链。本任务不授权Git写，所有报告只落盘供评审；V17保持Closed。工程唯一 D:/AI_Agent_Lab/rrpg_pilot/repo，当前HEAD223a08a3d1c45cc5498a7b453cd1b9f4675908d4，请真实读取不要复制旧HEAD。

先读 Docs/Roadmap/RRPG_FP-V0.1_Product-Design-and-Evidence-Integration_2026-11-06.md、对应Evidence Index、9/7 Functional Prototype Checkpoint §3七Gate与Current Village§6.8十四项、Expedition/Person/Battle/Resource/Save Current规则；读V17 Local Closure Receipt与最新Handoff。当前规则优先，不把候选变需求。

## 存档/字体/工作区保护：先建立，再运行

1. 读取实际Editor项目、Scene、Play状态。若用户正在Play或其他执行占用，不擅自退出；记录具体状态。按 Development/StudioRuns/Initialization-20261105/font-protection-README.md 的真实v2 begin/end（begin成功才进入；finally end，非0停止，incident保留）。不得Refresh/SaveAssets/SaveScene/Builder，不测字体字模。不恢复/覆盖dirty。
2. SaveSystem.GetSavePath固定单槽，没有路径注入。必须先确认可以安全隔离：在**非Play、没有原游戏运行/写入**时，取得实际用户档路径/是否存在/字节/SHA及.tmp等残留清单；做可审查完整备份并核验。授权允许隔离测试槽，不允许写原档。
3. 可使用**同目录原子改名封存原文件**至唯一命名的 .fp-audit-original-<runid>（禁止覆盖已有任何文件）。原文件本体始终不被写入。原档存在时绝不能在其占槽时调用NewGame/Save；只有封存后槽路径空、备份和renameSHA核对完成，才从正式NewGame建立隔离测试档。若系统不支持安全原子改名或有写入占用，停止写档路径并报告，不另造工具链或谎称已隔离。
4. finally：先安全退出本次测试Play（不保存场景/字体）；把测试槽与本轮.tmp移到本目录的 test-save-evidence 唯一位置，确保原路径空；将封存原文件**原子归位**且禁止覆盖，复核用户原档SHA/字节、原.tmp状态、原目录无测试残留。不是用File.WriteAllBytes覆盖恢复！没有原档时原路径最终仍应不存在。任何异常优先保护原档并明确停点，不能只登记技术协助。
5. 运行前后相关产品/Scene/TMP/历史保护资产hash可核对，必要新证据/门禁records单列。不得把本次测试档当成用户Continue体验。主开发另有全文件基线。

## 正常路径与证据

从正式MainMenu/NewGame开始，记录截图/实际按钮层级和操作、时间、预期/实际、证据。优先真实鼠标/键盘/已有MCP交互；若只能程序化EventSystem点击真实可见按钮则明确记录为程序化正常UI入口，不宣称物理操作。禁止直接调用隐藏/Debug/Test入口替代正常导航，禁止注入金币、人物、HP、卡牌、奖励、时间结算、Node状态、直接强杀敌人、手动构造场景。只能玩家可用速度按钮/时间推进与合法卡牌操作。

G7原链：NewGame→Tutorial Expedition→Return Home→Formal Prep→Expedition→Return→Save/Load→Continue。G6：第一次Return后继续Prep→再次Expedition→第二次Return后仍可继续，核心循环可重复，不人工改数据。

重点尝试：第一次回村→远征准备→第二次远征→战斗/事件→返程结算→村庄变化→再次出征。正常入口首个阻塞先记录并只读追踪原因；**不绕过继续计PASS**。例如NewGame直接Home无Tutorial：记录G7首阻塞，再经已存在可见Home→Prep入口进行明确独立分支验证G6（不是补造Tutorial Return；G6仍缺真实第一次Return前提）。如果某分支继续也遇阻，记录首阻塞后停止该分支，不做测试注入。正常UI打不开不得Invoke私有处理器冒充点击；诊断只读查询与正式路径分别标明。

每一步保存真实工具原回执/状态JSON/必要截图，人物PersonId、建筑/岗位、资源、Supply、选人、Return类型及前后事实按可读范围记录。复用历史E1–E6但注明旧版/注入/Retreat等局限，不作为本轮通过。

## 交付

本目录新增 progress.md（关键进展可尽早写）、result.md、steps.json、gate-matrix.json（G1–7及O1–14逐项：正常实证/历史限定/阻塞/未测）、protection.json（原档封存/归位与SHA、fontgate记录退出码、Scene/TMP等）、raw-responses.md及screenshots。报告区分入口未接/内容未配/功能缺陷/已有功能缺证据/NotFrozen；给首阻塞源码文件与方法/配置依据、影响范围、最小修复顺序和需制作人裁决事项，不实施修复。

有工具限制写具体能力和原因，不安装/修改连接器，不笼统要求主开发接手。若已能到正常战斗，以合法玩法推进；不能因耗时而用伤害注入。完成保护清理及交付后停止。不得宣称FP整体通过，最终由主开发复核。
