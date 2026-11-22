# Unity MCP 重连验证

日期：2026-11-06

- 原因：Codex 的 `unity_ai_game_developer` 配置仍指向旧端口 `29387`，Editor 当前服务使用 `21867`。
- 已执行：通过 `codex mcp add` 将该服务地址更新为 `http://localhost:21867/mcp`，保留 HTTP 传输。
- 验证方式：直接请求本机 MCP，完成 initialize、tools/list 和真实 tools/call。当前聊天的原生工具列表尚未刷新，实际调用经本机 HTTP 完成。
- 已确认工程：`D:/AI_Agent_Lab/rrpg_pilot/repo/UnityProject/RRPG1.0game`，版本 `2022.3.62t9`。
- 活动场景：`Assets/Scenes/SampleScene.scene`，未脏，5 个根对象。
- Editor：未运行、未暂停、未编译、未更新。
- Console 当前条目：4 条 Log、755 条 Warning；分类结果中没有 Error/Exception。最近 12 条均为 LiberationSans SDF 缺字警告。
- 本次没有修改场景、游戏资产或玩法配置；未断开其他 MCP 客户端。

原始验证结果在同级上层 `Evidence` 中：`unity-live-project.json`、`unity-live-editor.json`、`unity-live-scenes.json`、`unity-live-console.json`、`unity-live-console-summary.sse`。

后续美术工作：从 Home 实机样板开始校准统一斜向网格、建筑落点、人物脚底锚点和遮挡排序；验证通过后再批量替换。
