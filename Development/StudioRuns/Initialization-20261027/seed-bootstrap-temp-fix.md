# Initial World Bootstrap 临时文件扩展名与所有权修复

- 阶段：当前阶段 1/1（唯一窄修，不重做调查 / 不重跑整包）
- 运行目录确认：`D:\AI_Agent_Lab\rrpg_pilot\repo`（Pilot 仓库，工作目录工具确认）
- 宿主基线（引用，不在本包复跑）：`compiler-host:60c0c44b6e7d382b1434d342dbb0b2a0645be5ccdd2e49e44bd02d834a04863c` —— Release build 退出 0，Compiler 全测试 276/276 通过、0 skip；`initialworld-seed` 在独立 staging 退出 3，异常为 `ArgumentException: Extension 'seed-tmp' is not supported`
- 本包性质：源码 + 测试 + 报告的窄修；**未运行 Shell / Process / Compiler / tests-run / Unity**，**本包未运行 .NET 测试**，待主开发用固定 Compiler 入口核验

---

## 1. 根因

`InitialWorldWorkbookSeed.Run` 原先使用固定临时路径：

```text
<AuthoringDir>/.RRPG_InitialWorld.xlsx.seed-tmp
```

两个独立缺陷：

1. **扩展名不是 `.xlsx`**：最终扩展名是 `.seed-tmp`。ClosedXML 0.104.2 的 `XLWorkbook.SaveAs(path)` 按扩展名选择包格式，遇到未知扩展名直接抛 `ArgumentException: Extension 'seed-tmp' is not supported`，命令落入 `catch` 返回 3 —— 与宿主 staging 回执完全一致。写入从未开始，因此不是内容或字段问题。
2. **不做所有权判断**：固定名在运行开始时若已存在会被 `File.Delete(temp)` 无条件删除；`finally` 也会按同一固定名再删一次。也就是任何先前存在的同名未知临时文件都会被清掉，违反「只清理由本次创建且拥有的临时文件」。

## 2. 精确文件范围（仅此三项）

| 文件 | 变更 |
|---|---|
| `Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/Core/InitialWorldWorkbookSeed.cs` | 修改：唯一临时名 + `.xlsx` 扩展 + 仅清理自有文件 + 显式无覆盖移动 |
| `Tools/RRPG.ConfigCompiler/tests/RRPG.ConfigCompiler.Tests/InitialWorldWorkbookSeedTests.cs` | 新增：真实文件系统 bootstrap 回归（4 例） |
| `Development/StudioRuns/Initialization-20261027/seed-bootstrap-temp-fix.md` | 新增：本报告 |

未修改：`Program.cs`、`CompilerPaths.cs`、其它 Compiler 源码 / 校验器、其它测试文件、`Development/Config/*`（含现有 README 与工作簿）、默认数值 / 产品配置、Unity 工程、用户档 / 字体 / 基线、Git。`InitialWorldConfigTests.cs` 等既有测试零改动。

## 3. 修复内容（`InitialWorldWorkbookSeed.cs`）

1. **唯一临时文件名**：新增私有 `AllocateOwnedTempPath(authoringDir)`，候选名为
   `.RRPG_InitialWorld.seed-<Guid N>.xlsx`；带 `.` 前缀（`WorkbookDiscovery` 仍会静默跳过隐藏文件），扩展名固定 `.xlsx`；对极小概率碰撞重试 8 次（仍碰撞才抛 `IOException`，不落盘）。
2. **文件所有权**：`Run` 内局部 `string temp = null` 只指向本次分配路径；删除固定名预清理 `File.Delete(temp)` 已移除。`finally` 仅在 `temp != null` 时删除该路径；成功 `File.Move` 后立即置 `temp = null`（所有权转移给目标），因此不会删除目标，也不会碰任何事先存在的临时文件。
3. **原子无覆盖移动**：`File.Move(temp, target, false)`（显式 `overwrite:false`）；移动前保留既有 `File.Exists(target)` 预检查，移动中若目标被并发创建则捕获 `IOException` 且 `File.Exists(target)` 为真时打印拒绝并返回 1（临时文件由 `finally` 清理，属本次所有）。
4. **目标已存在仍拒绝**：入口处的 `File.Exists(target)` 检查与退出码 1 原样保留，不做任何写入。
5. **校验保留**：Open XML SDK 结构校验（`OpenXmlPackageChecker.Validate`，0 issue 才继续）、ClosedXML 重新打开 temp 逐表核对、移动后重新打开目标、`bytes` / `SHA256` 审计输出全部未删减、未放宽。
6. 类级 XML 文档同步更新保护顺序（临时名唯一性与扩展名要求写入注释），玩法 / 默认内容 / 三表字段与行数据未改。

## 4. 新增测试（`InitialWorldWorkbookSeedTests.cs`，xUnit，现有 Compiler 测试工程模式）

全部使用现有 `TestRoot`（系统临时目录 + 一次性根，不触碰正式 `Development/Config`），不依赖 Unity、不访问网络。

| 用例 | 覆盖契约 |
|---|---|
| `FirstRunCreatesWorkbookWithoutLeavingOwnedTempFiles` | 首次运行返回 0；目标存在；`OpenXmlPackageChecker.Validate(target)` 为空；重新打开核对 `InitialWorld` / `InitialPopulation` / `InitialBuilding` 有数据行且 B5 / C5 值正确；Authoring 目录最终只有目标文件（无任何自有临时残留） |
| `SecondRunIsRejectedAndTargetBytesStayIdentical` | 第二次运行返回 1；目标存在；`byte[]` 逐字节相等且 SHA256 相同；目录仍只有目标文件 |
| `UnknownExistingTempFilesAreNeitherDeletedNorOverwritten` | 预置旧固定名 `.RRPG_InitialWorld.xlsx.seed-tmp` 与任意 `.RRPG_InitialWorld.seed-foreign.xlsx`；运行返回 0 且生成目标；两个既有文件仍存在、字节不变；目录无额外残留 |
| `PreExistingTargetIsRejectedWithoutWritingOrTouchingTempFiles` | 目标先存在（任意字节）时返回 1；目标字节不变；不产生任何临时文件 |

用例直接调用公开入口 `InitialWorldWorkbookSeed.Run(CompilerPaths)`，即真实 CLI 命令的同一代码路径（`Program.cs` 的 `initialworld-seed` 分发未改），不是复制实现或 mock 文件系统。

## 5. 静态复核（本包实际执行的唯一验证形式）

- 逐行重读修改后的 `InitialWorldWorkbookSeed.cs`（含 `AllocateOwnedTempPath`、`try/catch/finally`、`File.Move(..., false)`），确认：入口拒绝路径不写盘；`temp` 仅在本次分配后非空；成功移动后置空；`finally` 无通配 / 固定名删除。
- 仓库范围 grep `seed-tmp|\.seed-|File.Delete|File.Move|SaveAs(`：seed 源码只剩自有临时文件的 `File.Delete(temp)` 与 `File.Move(temp, target, false)`；旧固定名只出现在新测试的负例常量与文档注释中；`OutputCommitter` 等其它模块未受影响（未改动）。
- 公开 API 核对：`InitialWorldWorkbookSeed.Run` 为 `public static int`、`OpenXmlPackageChecker.Validate(string) -> List<string>`、`RawSheet.FirstDataRow` 为 `public const int`、`TestRoot` / `TestKit` 为测试工程既有类型，均可在新测试中直接引用。
- 列位置核对：seed 使用「A 列说明 + B 列起业务字段」布局，故 `InitialPopulation` 的 `PersonDefinitionId` 在 col 3、`InitialBuilding` 的 `BuildingDefinitionId` 在 col 3，测试断言与实现一致。
- 边界核对：本包未写 `Development/Config` 或 `Development/Generated`，未生成 `.xlsx` / `.json` / `.asset`，未改默认值、用户档、字体或基线，未操作 Git。

## 6. 未运行边界与待主开发执行

- 本包**未运行**：Shell / Process / `dotnet` / Compiler / `tests-run` / Unity；因此新增 4 例与既有 276 例的**实际结果未知**，本报告不声称测试通过。
- 未计算改动文件的字节哈希（无 Shell），仅保留上述静态复核证据。
- 待主开发在固定 Compiler 入口（同一 `compiler-host:60c0c44b…` 受控宿主）依次执行并以其回执为准：

```text
dotnet build Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj -c Release
dotnet test  Tools/RRPG.ConfigCompiler/tests/RRPG.ConfigCompiler.Tests/RRPG.ConfigCompiler.Tests.csproj -c Release --filter FullyQualifiedName~InitialWorldWorkbookSeedTests
:: 建议随后跑整包既有 Compiler 测试，确认 276 例基线未回退
dotnet run --project Tools/RRPG.ConfigCompiler/src/RRPG.ConfigCompiler/RRPG.ConfigCompiler.csproj -c Release -- initialworld-seed --root <独立 staging root>
```

- 预期：4 例通过；`initialworld-seed` 首次在 staging 退出 0 并生成 `Development/Config/RRPG_InitialWorld.xlsx`；重复执行退出 1 且目标字节不变。若 `OpenXmlPackageChecker` 对 ClosedXML 产物报 issue，属既有校验门的真实结果，不得为通过而放宽，应回报主开发。
- 本报告与实现需由主开发专业核验；不代制作人批准，也不表示初始化整包完成。Studio 阶段自动接续后续验证门禁。
