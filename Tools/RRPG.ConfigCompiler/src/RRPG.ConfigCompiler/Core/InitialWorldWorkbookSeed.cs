using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ClosedXML.Excel;
using RRPG.ConfigCompiler.Xlsx;

namespace RRPG.ConfigCompiler.Core
{
    /// <summary>
    /// **一次性 Authoring Bootstrap**：创建 <c>Development/Config/RRPG_InitialWorld.xlsx</c>。
    ///
    /// 为什么要单独做成一次性命令：
    ///   - 本领域的正式人工 Authority 仍是 XLSX；Runtime 绝不手写 JSON / ScriptableObject；
    ///   - 在「工作簿尚不存在」的初始化阶段，需要一个确定、可审计、不会覆盖制作人后续编辑的
    ///     首次落盘方式；
    ///   - 因此本命令**只在目标文件不存在时运行**；目标已存在 → 拒绝（退出码 1），
    ///     绝不覆盖、绝不合并、绝不把 XLSX 变成第二份代码内 Authority。
    ///
    /// 保护与自检顺序：
    ///   1. 目标已存在 → 直接失败（不做任何写入）；
    ///   2. 写到同目录隐藏临时文件（WorkbookDiscovery 会跳过 "." 前缀）；临时文件必须是
    ///      **本次运行的唯一文件名**且扩展名为 <c>.xlsx</c>：
    ///        - 扩展名：ClosedXML / OOXML 按扩展名判定包格式，非 .xlsx 会抛
    ///          <c>ArgumentException: Extension '...' is not supported</c>；
    ///        - 唯一性：绝不删除 / 覆盖目录中先前存在的未知临时文件，只清理本次创建并拥有的文件；
    ///   3. Open XML SDK 结构校验（OpenXmlPackageChecker）必须 0 issue；
    ///   4. ClosedXML 重新打开临时文件，逐表核对 sheet 名与数据行数；
    ///   5. 同目录原子无覆盖移动（File.Move 显式 overwrite:false）到目标；
    ///   6. 重新打开目标并计算 bytes / SHA256，打印审计信息。
    ///
    /// 内容全部是 **Test / Not Frozen**：人口名册取自当前工程实际初始名册，
    /// 英雄 / 金币 / 资源取当前实际默认值，建筑布局取当前 Home 展示的 farm / kitchen /
    /// 5 个住房锚点；数量与坐标后续可直接改 XLSX，不需要制作人另开授权。
    /// 默认内容**只包括** farm / kitchen / house，不包含 workshop / 铁匠铺。
    /// </summary>
    public static class InitialWorldWorkbookSeed
    {
        public const string FileName = "RRPG_InitialWorld.xlsx";

        private const string WorldSheet = "InitialWorld";
        private const string PopulationSheet = "InitialPopulation";
        private const string BuildingSheet = "InitialBuilding";
        private const string NavigationSheet = "00_导航";

        private sealed class SeedSheet
        {
            public string Name;
            public string[] Fields;
            public string[] Types;
            public string[] Rules;
            public string[] Headers;
            public string[] Notes;
            public object[][] Rows;
        }

        public static int Run(CompilerPaths paths)
        {
            string target = Path.Combine(paths.AuthoringDir, FileName);
            Console.WriteLine("RRPG Initial World Authoring Bootstrap（一次性；Test / Not Frozen）");
            Console.WriteLine("  authoring : " + CompilerPaths.ToForwardSlash(paths.AuthoringDir));
            Console.WriteLine("  target    : " + CompilerPaths.ToForwardSlash(target));

            if (File.Exists(target))
            {
                Console.Error.WriteLine("拒绝：目标工作簿已存在，本命令绝不覆盖制作人编辑。");
                Console.Error.WriteLine("      如需重建，请先人工备份并移走该文件，再重新运行。");
                return 1;
            }

            Directory.CreateDirectory(paths.AuthoringDir);

            // 只持有本次运行创建的临时文件路径；绝不触碰目录里先前存在的临时文件。
            string temp = null;

            try
            {
                temp = AllocateOwnedTempPath(paths.AuthoringDir);

                using (var workbook = new XLWorkbook())
                {
                    WriteNavigation(workbook);
                    WriteDataSheet(workbook, BuildWorldSheet());
                    WriteDataSheet(workbook, BuildPopulationSheet());
                    WriteDataSheet(workbook, BuildBuildingSheet());
                    workbook.SaveAs(temp);
                }

                List<string> packageIssues = OpenXmlPackageChecker.Validate(temp);
                if (packageIssues.Count > 0)
                {
                    Console.Error.WriteLine("Open XML 结构校验失败（" + packageIssues.Count + " 项），未写入目标：");
                    foreach (string issue in packageIssues) Console.Error.WriteLine("  - " + issue);
                    return 1;
                }

                VerifyWorkbook(temp);

                if (File.Exists(target))
                {
                    Console.Error.WriteLine("拒绝：目标工作簿在写入过程中出现，不覆盖。");
                    return 1;
                }

                try
                {
                    File.Move(temp, target, false); // 原子无覆盖：目标已存在会抛 IOException
                }
                catch (IOException) when (File.Exists(target))
                {
                    Console.Error.WriteLine("拒绝：目标工作簿在写入过程中出现，不覆盖。");
                    return 1;
                }

                temp = null; // 已移动：所有权转移给 target，finally 不再清理

                VerifyWorkbook(target);
                byte[] bytes = File.ReadAllBytes(target);
                Console.WriteLine();
                Console.WriteLine("已创建：" + CompilerPaths.ToForwardSlash(target));
                Console.WriteLine("  bytes    : " + bytes.Length.ToString(CultureInfo.InvariantCulture));
                Console.WriteLine("  sha256   : " + Sha256(bytes));
                Console.WriteLine("  sheets   : " + WorldSheet + " / " + PopulationSheet + " / " + BuildingSheet);
                Console.WriteLine("  next     : rrpg-config validate --root <repo> → build → Unity Builder");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("创建失败：" + ex.GetType().Name + ": " + ex.Message);
                return 3;
            }
            finally
            {
                // 只清理本次创建且仍归本进程所有的临时文件；temp 为 null 时绝不删除任何东西。
                if (temp != null)
                {
                    try { if (File.Exists(temp)) File.Delete(temp); }
                    catch { /* 清理失败不掩盖原始结果 */ }
                }
            }
        }

        /// <summary>
        /// 分配本次运行独占的临时工作簿路径：
        ///   - 与目标同目录，"." 前缀保证 WorkbookDiscovery 静默跳过；
        ///   - 唯一文件名（Guid），不覆盖 / 不删除任何既有临时文件；
        ///   - 扩展名必须 <c>.xlsx</c>，否则 ClosedXML 拒绝构建 OOXML 包。
        /// </summary>
        private static string AllocateOwnedTempPath(string authoringDir)
        {
            string stem = Path.GetFileNameWithoutExtension(FileName);
            for (int attempt = 0; attempt < 8; attempt++)
            {
                string candidate = Path.Combine(
                    authoringDir,
                    "." + stem + ".seed-" + Guid.NewGuid().ToString("N") + ".xlsx");
                if (!File.Exists(candidate)) return candidate;
            }

            throw new IOException("无法分配唯一的临时工作簿文件名（连续碰撞）。");
        }

        private static SeedSheet BuildWorldSheet()
        {
            return new SeedSheet
            {
                Name = WorldSheet,
                Fields = new[] { "ProfileId", "IsDefault", "InitialHeroId", "Gold", "InitialIngredients", "InitialFood" },
                Types = new[] { "id", "bool", "string", "int", "int", "int" },
                Rules = new[] { "", "", "", ">=0", ">=0", ">=0" },
                Headers = new[] { "配置档案ID", "是否默认档案", "初始英雄ID", "初始金币", "初始食材", "初始食物" },
                Notes = new[]
                {
                    "Test / Not Frozen：默认档案身份，可改名但必须恰好一行 IsDefault=TRUE。",
                    "Test / Not Frozen：默认产品入口按这一行读取初始世界。",
                    "Test / Not Frozen：当前实际默认英雄 hero_001（青鹿）；英雄替换时序仍由既有 UI 负责。",
                    "Test / Not Frozen：当前实际默认金币 100，可后续调整。",
                    "Test / Not Frozen：当前实际默认食材 0，可后续调整。",
                    "Test / Not Frozen：当前实际默认食物 0，可后续调整。"
                },
                Rows = new[]
                {
                    new object[] { "iw_default_test", true, "hero_001", 100, 0, 0 }
                }
            };
        }

        private static SeedSheet BuildPopulationSheet()
        {
            return new SeedSheet
            {
                Name = PopulationSheet,
                Fields = new[] { "StableIndex", "PersonDefinitionId" },
                Types = new[] { "int", "string" },
                Rules = new[] { "key;>=0", "" },
                Headers = new[] { "稳定序号", "人物定义ID" },
                Notes = new[]
                {
                    "Test / Not Frozen：稳定序号 = 初始创建顺序，只要求唯一且稳定，不代表正式名册数量。",
                    "Test / Not Frozen：PersonDefinitionId 的真实存在性由 Runtime 跨域校验（Compiler 不伪造引用表）。"
                },
                Rows = new[]
                {
                    new object[] { 1, "person_lin_en" },
                    new object[] { 2, "person_su_wan" },
                    new object[] { 3, "person_han_zhao" },
                    new object[] { 4, "person_qiu_shuang" },
                    new object[] { 5, "person_wei_an" }
                }
            };
        }

        private static SeedSheet BuildBuildingSheet()
        {
            return new SeedSheet
            {
                Name = BuildingSheet,
                Fields = new[] { "StableIndex", "BuildingDefinitionId", "WorldX", "WorldY" },
                Types = new[] { "int", "ref:BuildingDefinition", "float", "float" },
                Rules = new[] { "key;>=0", "", "", "" },
                Headers = new[] { "稳定序号", "建筑定义ID", "世界X", "世界Y" },
                Notes = new[]
                {
                    "Test / Not Frozen：稳定序号 = 初始实例分配顺序，只要求唯一且稳定。",
                    "Test / Not Frozen：只允许已存在的 BuildingDefinition；占地 / 重叠由 Runtime Placement 校验。",
                    "Test / Not Frozen：坐标取自当前 Home 展示的农场 / 厨房 / 住房锚点，可后续调整。",
                    "Test / Not Frozen：住房数量与行位置保留当前展示（每名初始人物 1 个住房锚点）。"
                },
                Rows = new[]
                {
                    // 当前 Home 展示的非住房槽位：ws_farm_01 → (-6,-1.2)，ws_kitchen_01 → (-2,-1.2)。
                    new object[] { 1, "building_farm", -6f, -1.2f },
                    new object[] { 2, "building_kitchen", -2f, -1.2f },
                    // 当前 Home 展示的 5 个住房槽位（x = -8.5 + slot*3.4，y = 4.2）。
                    new object[] { 3, "building_house", -8.5f, 4.2f },
                    new object[] { 4, "building_house", -5.1f, 4.2f },
                    new object[] { 5, "building_house", -1.7f, 4.2f },
                    new object[] { 6, "building_house", 1.7f, 4.2f },
                    new object[] { 7, "building_house", 5.1f, 4.2f }
                }
            };
        }

        private static void WriteNavigation(XLWorkbook workbook)
        {
            var sheet = workbook.AddWorksheet(NavigationSheet);
            string[,] lines =
            {
                { "RRPG_InitialWorld", "正式 NewGame 初始世界配置（Authoring）" },
                { "Authority", "本工作簿是唯一人工 Authority；Runtime 不手写 JSON / SO。" },
                { "链", "XLSX → RRPG.ConfigCompiler → Development/Generated/StaticConfig/*.json → Unity Builder → StaticConfigRegistry → InitialWorldConfigurationCatalog" },
                { "三张表", "InitialWorld（profile/资源/英雄）/ InitialPopulation（稳定序号/人物定义）/ InitialBuilding（稳定序号/建筑定义/坐标）" },
                { "三表齐全", "必须同时存在；只写一部分是 Hard Error，不会被静默当成 0 建筑。" },
                { "Test / Not Frozen", "本工作簿所有数值、人口、住房数量与坐标都是测试默认，后续可直接修改，无需制作人另定。" },
                { "默认建筑", "只包括 building_farm / building_kitchen / building_house；不包括 workshop / 铁匠铺。" },
                { "跨域", "PersonDefinition 不在同一 Compiler 语料里：Compiler 只校验形状，Runtime 校验真实引用。" },
                { "重建", "本文件由一次性 bootstrap 生成；之后请直接编辑本工作簿，不要再次运行 bootstrap（命令会拒绝覆盖）。" },
                { "生成", "改完运行 RRPG_检查配置.cmd / RRPG_一键导表.cmd（Compiler build → Unity Builder）。" }
            };

            for (int r = 0; r < lines.GetLength(0); r++)
            {
                sheet.Cell(r + 1, 1).Value = lines[r, 0];
                sheet.Cell(r + 1, 2).Value = lines[r, 1];
            }
            sheet.Column(1).Width = 22;
            sheet.Column(2).Width = 120;
        }

        private static void WriteDataSheet(XLWorkbook workbook, SeedSheet seed)
        {
            var sheet = workbook.AddWorksheet(seed.Name);
            sheet.Cell(1, 1).Value = RawSheet.AnnotationMachineFieldLabel;
            sheet.Cell(2, 1).Value = RawSheet.AnnotationTypeLabel;
            sheet.Cell(3, 1).Value = RawSheet.AnnotationRuleLabel;
            sheet.Cell(4, 1).Value = "中文说明";
            for (int r = 0; r < seed.Notes.Length; r++) sheet.Cell(5 + r, 1).Value = seed.Notes[r];

            for (int i = 0; i < seed.Fields.Length; i++)
            {
                int column = i + 2; // B 列起
                sheet.Cell(1, column).Value = seed.Fields[i];
                sheet.Cell(2, column).Value = seed.Types[i];
                sheet.Cell(3, column).Value = seed.Rules[i];
                sheet.Cell(4, column).Value = seed.Headers[i];
            }

            for (int r = 0; r < seed.Rows.Length; r++)
            {
                object[] row = seed.Rows[r];
                for (int c = 0; c < row.Length; c++)
                {
                    SetValue(sheet.Cell(5 + r, c + 2), row[c]);
                }
            }

            sheet.Column(1).Width = 50;
            for (int i = 0; i < seed.Fields.Length; i++) sheet.Column(i + 2).Width = 22;
        }

        private static void SetValue(IXLCell cell, object value)
        {
            if (value == null) return;
            if (value is string s) cell.Value = s;
            else if (value is int i) cell.Value = i;
            else if (value is long l) cell.Value = l;
            else if (value is float f) cell.Value = (double)f;
            else if (value is double d) cell.Value = d;
            else if (value is bool b) cell.Value = b;
            else cell.Value = value.ToString();
        }

        private static void VerifyWorkbook(string path)
        {
            using (var workbook = new XLWorkbook(path))
            {
                string[] expected = { WorldSheet, PopulationSheet, BuildingSheet };
                foreach (string name in expected)
                {
                    IXLWorksheet sheet;
                    try { sheet = workbook.Worksheet(name); }
                    catch (Exception ex) { throw new InvalidDataException("重新打开后缺少 Sheet：" + name, ex); }

                    int dataRows = 0;
                    for (int r = RawSheet.FirstDataRow; r <= sheet.LastRowUsed().RowNumber(); r++)
                    {
                        bool any = false;
                        for (int c = 2; c <= sheet.LastColumnUsed().ColumnNumber(); c++)
                        {
                            if (WorkbookReader.CellText(sheet.Cell(r, c)).Length > 0) { any = true; break; }
                        }
                        if (any) dataRows++;
                    }

                    if (dataRows == 0)
                        throw new InvalidDataException("Sheet 没有数据行：" + name);
                }
            }
        }

        private static string Sha256(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}
