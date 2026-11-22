using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ClosedXML.Excel;
using RRPG.ConfigCompiler.Core;
using RRPG.ConfigCompiler.Xlsx;
using Xunit;

namespace RRPG.ConfigCompiler.Tests
{
    /// <summary>
    /// 一次性 Authoring Bootstrap（<see cref="InitialWorldWorkbookSeed"/>）的**真实文件系统**回归。
    ///
    /// 背景：seed 曾在独立 staging 以退出码 3 失败：
    ///   <c>ArgumentException: Extension 'seed-tmp' is not supported</c>
    /// 根因是临时文件用了固定名 <c>.RRPG_InitialWorld.xlsx.seed-tmp</c>，扩展名不是 .xlsx。
    ///
    /// 本测试只验证可观察的文件系统契约（不依赖 Unity、不访问正式 Development/Config）：
    ///   - 首次运行成功创建目标工作簿，且不残留任何本命令拥有的临时文件；
    ///   - 重复运行被拒绝（退出码 1），目标字节完全不变；
    ///   - 目录中先前存在的未知临时文件（含旧固定名）不被删除 / 覆盖。
    /// </summary>
    public class InitialWorldWorkbookSeedTests
    {
        private static string TargetPath(TestRoot root)
        {
            return Path.Combine(root.Paths.AuthoringDir, InitialWorldWorkbookSeed.FileName);
        }

        private static string[] FilesInAuthoringDir(TestRoot root)
        {
            return Directory.Exists(root.Paths.AuthoringDir)
                ? Directory.GetFiles(root.Paths.AuthoringDir)
                : Array.Empty<string>();
        }

        private static string Sha256(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        /// <summary>旧实现使用的固定临时文件名（扩展名不是 .xlsx；用于证明既有未知文件不被触碰）。</summary>
        private const string LegacyTempName = ".RRPG_InitialWorld.xlsx.seed-tmp";

        [Fact]
        public void FirstRunCreatesWorkbookWithoutLeavingOwnedTempFiles()
        {
            using var root = new TestRoot();

            int exit = InitialWorldWorkbookSeed.Run(root.Paths);

            Assert.Equal(0, exit);
            string target = TargetPath(root);
            Assert.True(File.Exists(target), "首次运行必须创建目标工作簿");

            // 目标本身是合法 OOXML 包，且三张数据表都有内容（与 seed 自检同口径）。
            Assert.Empty(OpenXmlPackageChecker.Validate(target));
            using (var workbook = new XLWorkbook(target))
            {
                foreach (string sheetName in new[] { "InitialWorld", "InitialPopulation", "InitialBuilding" })
                {
                    IXLWorksheet sheet = workbook.Worksheet(sheetName);
                    Assert.True(sheet.LastRowUsed().RowNumber() >= RawSheet.FirstDataRow,
                        "Sheet 没有数据行：" + sheetName);
                }

                // 说明列布局：A 列是制作人说明，业务字段从 B 列（col 2）开始。
                Assert.Equal("iw_default_test", workbook.Worksheet("InitialWorld").Cell(5, 2).GetString());
                // InitialPopulation：col2=StableIndex，col3=PersonDefinitionId。
                Assert.Equal("person_lin_en", workbook.Worksheet("InitialPopulation").Cell(5, 3).GetString());
                // InitialBuilding：col2=StableIndex，col3=BuildingDefinitionId。
                Assert.Equal("building_farm", workbook.Worksheet("InitialBuilding").Cell(5, 3).GetString());
            }

            // 本命令只应留下目标文件：临时文件（唯一名 / 旧固定名）都必须清理干净。
            string[] files = FilesInAuthoringDir(root).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
            Assert.Equal(new[] { InitialWorldWorkbookSeed.FileName }, files);
        }

        [Fact]
        public void SecondRunIsRejectedAndTargetBytesStayIdentical()
        {
            using var root = new TestRoot();

            Assert.Equal(0, InitialWorldWorkbookSeed.Run(root.Paths));
            string target = TargetPath(root);
            byte[] before = File.ReadAllBytes(target);
            string hashBefore = Sha256(before);

            int second = InitialWorldWorkbookSeed.Run(root.Paths);

            Assert.Equal(1, second);
            Assert.True(File.Exists(target), "拒绝重复运行时目标必须仍存在");
            byte[] after = File.ReadAllBytes(target);
            Assert.Equal(before, after);
            Assert.Equal(hashBefore, Sha256(after));

            // 被拒绝的运行不得留下任何临时文件。
            string[] files = FilesInAuthoringDir(root).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
            Assert.Equal(new[] { InitialWorldWorkbookSeed.FileName }, files);
        }

        [Fact]
        public void UnknownExistingTempFilesAreNeitherDeletedNorOverwritten()
        {
            using var root = new TestRoot();
            Directory.CreateDirectory(root.Paths.AuthoringDir);

            // 模拟目录里先前存在、但不属于本次运行的临时 / 未知文件：
            //   - 旧实现会无条件删除的固定名；
            //   - 另一个任意未知文件，用来证明本命令不做通配清理。
            string legacyTemp = Path.Combine(root.Paths.AuthoringDir, LegacyTempName);
            string foreignTemp = Path.Combine(root.Paths.AuthoringDir, ".RRPG_InitialWorld.seed-foreign.xlsx");
            byte[] legacyBytes = new byte[] { 1, 2, 3, 4, 5 };
            byte[] foreignBytes = new byte[] { 9, 8, 7 };
            File.WriteAllBytes(legacyTemp, legacyBytes);
            File.WriteAllBytes(foreignTemp, foreignBytes);

            int exit = InitialWorldWorkbookSeed.Run(root.Paths);

            Assert.Equal(0, exit);
            Assert.True(File.Exists(TargetPath(root)), "首次运行必须创建目标工作簿");

            Assert.True(File.Exists(legacyTemp), "未知既有临时文件不得被删除：" + LegacyTempName);
            Assert.Equal(legacyBytes, File.ReadAllBytes(legacyTemp));
            Assert.True(File.Exists(foreignTemp), "未知既有临时文件不得被删除：" + Path.GetFileName(foreignTemp));
            Assert.Equal(foreignBytes, File.ReadAllBytes(foreignTemp));

            // 除目标与两个预先存在的文件外，不得新增任何残留。
            var expected = new List<string>
            {
                InitialWorldWorkbookSeed.FileName,
                LegacyTempName,
                Path.GetFileName(foreignTemp)
            };
            string[] files = FilesInAuthoringDir(root).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
            Assert.Equal(expected.OrderBy(n => n, StringComparer.Ordinal).ToArray(), files);
        }

        [Fact]
        public void PreExistingTargetIsRejectedWithoutWritingOrTouchingTempFiles()
        {
            using var root = new TestRoot();
            Directory.CreateDirectory(root.Paths.AuthoringDir);
            string target = TargetPath(root);
            byte[] original = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0xAA, 0xBB };
            File.WriteAllBytes(target, original);

            int exit = InitialWorldWorkbookSeed.Run(root.Paths);

            Assert.Equal(1, exit);
            Assert.Equal(original, File.ReadAllBytes(target));
            Assert.Equal(new[] { InitialWorldWorkbookSeed.FileName },
                FilesInAuthoringDir(root).Select(Path.GetFileName).ToArray());
        }
    }
}
