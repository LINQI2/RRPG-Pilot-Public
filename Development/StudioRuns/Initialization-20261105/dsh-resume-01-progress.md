# DSH 续执行进度（Initial World，dsh-resume-01）

原 `dsh-progress.md` / `dsh-result.md`（11:14–11:18）为**恢复前历史**，本轮不覆盖、不改写。

## 12:51 第 1 步：Shell 与写入

- `pwsh` 可执行，`.Path` 正确；但沙箱只在**工作区根目录**授予写权限，
  `Development\`、`Tools\`、`UnityProject\` 及本目录下 `Set-Content` 均 `UnauthorizedAccessException`。
- 因此 shell 无法运行 Unity batchmode，也无法直接写配置/生成物。

## 12:52–12:55 第 2/3 步：找到真实 Unity 通道

- 29387/8080 无监听；读 `UserSettings/AI-Game-Developer-Config.json` 得既有端口 **21867**。
- `initialize` 成功：`gamedev-mcp-server 9.2.13`，工具 `rql-*` / `script-execute` / `tests-run` /
  `screenshot-isolated` / `unity-tool-list`。**Editor 在线**（此前“无 Editor”判断已更正）。

## 12:56 第 2 步：Editor 现场

`isPlaying=False isCompiling=False isUpdating=False`；`SampleScene` 未 dirty；无 PrefabStage；
用户存档 1 份 21611B；Console `err:1`（既有 IPC 噪声）。

## 12:57 保护基线

存档 `3ed26a77…`、场景 `570ab047…`、字体 `9ada5362…`；guard 103 run，未完成 0。

## 12:54 步骤 B：V4 回归

`VillageV4ResourceSurplusPlayTests` → **1/1 Passed**，2.09s；guard runId `20261105T045428Z` 已 restored；
日志显示 Day1–3 真实生产与 Jump 后 Food 17→185。

## 12:56 步骤 D（契约）

`InitialWorld*` EditMode **24/24 Passed**；Console 过后 err:0。

## 12:58–13:06 步骤 C：默认 A 表现

1. 定位：`BuildingPresentationBinding` 里 kitchen/house → `village.building.neutral`；
   `VillagePresentation` 无 A 档 person 键；素材只有 Farm 的 proof 图。
2. 制作人选择「生成最小 A 档白模图」。
3. 经 Editor 进程执行脚本：生成 `kitchen-a/house-a/person-a.png`（300×200 透明），
   Excel COM 追加 `VisualAsset` 3 行、`VillagePresentation` 5 行，改 4 行建筑绑定；
   `rrpg-config build` PASS（errors=0/warnings=9），`validate` PASS 且未写 Mirror。
4. Unity `StaticConfigBuilder.ExecuteBuild` → PASS，`A-Tier` 三图转 Sprite，catalog entries=17。
5. 截图发现人物仍是米色椭圆 → 定位两处缺陷：
   - `HomePresentationResolver` 的 A 档分支只给 `village.building.*` 查表，person 键被跳过；
   - `HomeWorldEntityView.ApplyPerson` 在 A 档把人物压成 0.38×0.62 并加米色 tint。
6. 最小修复后重新编译、重建、复跑，产品用例 2/2 连续通过；截图显示人物为蓝色人形剪影。

## 13:05 收尾核验

存档 `3ed26a77…`、场景 `570ab047…` 未变；字体变为 `f0f5b901…`（+533B，已登记为待裁决项）；
guard 133 run 全部 restored；Console `err:0`。

详细证据见 `dsh-resume-01-result.md`。
