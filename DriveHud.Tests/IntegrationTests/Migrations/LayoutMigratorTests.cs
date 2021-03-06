﻿//-----------------------------------------------------------------------
// <copyright file="LayoutMigratorTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.MigrationService.Migrators;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Extensions;
using DriveHUD.Entities;
using Model.Enums;
using Model.Hud;
using Model.Stats;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHud.Tests.IntegrationTests.Migrations
{
    [TestFixture]
    class LayoutMigratorTests : BaseLayoutsMigrationTest
    {
        private const string LayoutsV1Folder = "Layouts v.1";

        private const string LayoutsV2Folder = "Layouts v.2";

        private Dictionary<string, MigrationV2Result> migratedLayoutsV2;

        [OneTimeSetUp]
        public void LoadAndMigrateLayoutsV2()
        {
            Initalize();

            var layoutsV2Folder = Path.Combine(TestDataFolder, LayoutsV2Folder);

            Assert.IsTrue(Directory.Exists(layoutsV2Folder), $"Directory '{layoutsV2Folder}' doesn't exist");

            var layoutsV2DirectoryInfo = new DirectoryInfo(layoutsV2Folder);

            var layoutFiles = layoutsV2DirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            migratedLayoutsV2 = new Dictionary<string, MigrationV2Result>();

            var migrator = new LayoutMigrator();

            try
            {
                foreach (var file in layoutFiles)
                {
                    var hudLayoutInfo = ReadLayoutInfo(file.FullName);
                    var hudLayoutInfoV2 = migrator.Migrate(hudLayoutInfo);

                    var migrationV2Result = new MigrationV2Result
                    {
                        Source = hudLayoutInfo,
                        Result = hudLayoutInfoV2
                    };

                    migratedLayoutsV2.Add(file.FullName, migrationV2Result);
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Migration failed: {e}");
            }

            Assert.That(migratedLayoutsV2.Count, Is.EqualTo(layoutFiles.Length));
        }

        [OneTimeTearDown]
        public void SaveMigratedLayouts()
        {
            var migratedLayoutsDirectory = new DirectoryInfo("MigratedLayouts");

            if (migratedLayoutsDirectory.Exists)
            {
                migratedLayoutsDirectory.Delete(true);
            }

            migratedLayoutsDirectory.Create();

            foreach (var migratedLayoutV2 in migratedLayoutsV2)
            {
                var sourceLayoutFile = new FileInfo(migratedLayoutV2.Key);
                var directoryName = sourceLayoutFile.Directory.Name;

                var layoutFileName = $"{migratedLayoutV2.Value.Result.Name.RemoveInvalidFileNameChars()}.xml";
                var layoutFilePath = Path.Combine(migratedLayoutsDirectory.FullName, directoryName, layoutFileName);

                var layoutFileInfo = new FileInfo(layoutFilePath);

                if (!layoutFileInfo.Directory.Exists)
                {
                    layoutFileInfo.Directory.Create();
                }

                using (var fs = File.Open(layoutFileInfo.FullName, FileMode.Create))
                {
                    var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfoV2));
                    xmlSerializer.Serialize(fs, migratedLayoutV2.Value.Result);
                }
            }
        }

        [Test]
        public void LayoutV2OpacityIsMigrated()
        {
            Assert.Multiple(() =>
            {
                foreach (var layoutFile in migratedLayoutsV2.Keys)
                {
                    var migrationResult = GetMigrationV2Result(layoutFile);
                    var expectedOpacity = (double)migrationResult.Source.HudOpacity / 100;
                    Assert.That(migrationResult.Result.Opacity, Is.EqualTo(expectedOpacity), layoutFile);
                }
            });
        }

        [Test]
        public void LayoutV2NameIsMigrated()
        {
            Assert.Multiple(() =>
            {
                foreach (var layoutFile in migratedLayoutsV2.Keys)
                {
                    var migrationResult = GetMigrationV2Result(layoutFile);
                    Assert.That(migrationResult.Result.Name, Is.EqualTo(migrationResult.Source.Name));
                }
            });
        }

        [Test]
        public void LayoutV2TableTypeIsMigrated()
        {
            Assert.Multiple(() =>
            {
                foreach (var layoutFile in migratedLayoutsV2.Keys)
                {
                    var migrationResult = GetMigrationV2Result(layoutFile);
                    Assert.That(migrationResult.Result.TableType, Is.EqualTo(migrationResult.Source.TableType));
                }
            });
        }

        [Test]
        public void LayoutV2IsDefaultIsMigrated()
        {
            Assert.Multiple(() =>
            {
                foreach (var layoutFile in migratedLayoutsV2.Keys)
                {
                    var migrationResult = GetMigrationV2Result(layoutFile);
                    Assert.That(migrationResult.Result.IsDefault, Is.EqualTo(migrationResult.Source.IsDefault));
                }
            });
        }

        [Test]
        public void LayoutV2PlayerTypesAreMigrated()
        {
            Assert.Multiple(() =>
            {
                foreach (var layoutFile in migratedLayoutsV2.Keys)
                {
                    var migrationResult = GetMigrationV2Result(layoutFile);

                    Assert.That(migrationResult.Result.HudPlayerTypes.Count, Is.EqualTo(migrationResult.Source.HudPlayerTypes.Count));

                    foreach (var hudPlayerType in migrationResult.Source.HudPlayerTypes)
                    {
                        var foundPlayerType = migrationResult.Result.HudPlayerTypes.FirstOrDefault(x => ComparePlayerType(x, hudPlayerType));

                        Assert.IsNotNull(foundPlayerType, $"{hudPlayerType.Name} doesn't match");
                    }
                }
            });
        }

        [Test]
        public void LayoutV2BumperStickersAreMigrated()
        {
            Assert.Multiple(() =>
            {
                foreach (var layoutFile in migratedLayoutsV2.Keys)
                {
                    var migrationResult = GetMigrationV2Result(layoutFile);

                    Assert.That(migrationResult.Source.HudBumperStickerTypes.Count, Is.EqualTo(migrationResult.Result.HudBumperStickerTypes.Count));

                    foreach (var hudBumperStickerTypes in migrationResult.Source.HudBumperStickerTypes)
                    {
                        var migratedBumperSticketType = migrationResult.Result.HudBumperStickerTypes.FirstOrDefault(x => x.Name == hudBumperStickerTypes.Name &&
                            x.Label == hudBumperStickerTypes.Label && x.Description == hudBumperStickerTypes.Description && x.ToolTip == hudBumperStickerTypes.ToolTip &&
                            x.EnableBumperSticker == hudBumperStickerTypes.EnableBumperSticker && x.MinSample == hudBumperStickerTypes.MinSample &&
                            x.SelectedColor == hudBumperStickerTypes.SelectedColor && x.Stats.Count == hudBumperStickerTypes.Stats.Count);

                        Assert.IsNotNull(migratedBumperSticketType);
                    }
                }
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 1, 440, 156, 144, double.NaN)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 2, 654, 300, 144, double.NaN)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 3, 440, 447, 144, double.NaN)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 4, 282, 447, 144, double.NaN)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 5, 90, 300, 144, double.NaN)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 6, 282, 156, 144, double.NaN)]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 1, 403, 174, 142, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-horiz-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 2, 624, 267, 142, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-horiz-s2)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 1, 405, 51, 101, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 2, 619, 195, 101, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-vert1-s2)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 4, 364, 342, 101, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-vert1-s4)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 5, 172, 195, 101, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-vert1-s5)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", 1, 410, 174, 142, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-vert2-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", 5, 60, 318, 142, double.NaN, TestName = "LayoutV2ToolsPlainBoxUiPositionsAreMigrated(6-max-vert2-s6)")]
        public void LayoutV2ToolsPlainBoxUiPositionsAreMigrated(string file, int seat, double x, double y, double width, double height)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools
                .Where(p => p.ToolType == HudDesignerToolType.PlainStatBox)
                .Cast<HudLayoutPlainBoxTool>()
                .FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.UIPositions.Count, Is.EqualTo((int)migrationResult.Source.TableType));

                var uiPosition = layoutTool.UIPositions.FirstOrDefault(p => p.Seat == seat);

                Assert.IsNotNull(uiPosition, "UiPosition not found in migrated layout");

                Assert.That(uiPosition.Position.X, Is.EqualTo(x), "X");
                Assert.That(uiPosition.Position.Y, Is.EqualTo(y), "Y");
                Assert.That(uiPosition.Width, Is.EqualTo(width), "Width");
                Assert.That(uiPosition.Height, Is.EqualTo(height), "Height");
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml")]
        public void LayoutV2ToolsPlainBoxStatsAreMigrated(string file)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutPlainBoxTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Stats, "Tool stats not found");

            Assert.That(layoutTool.Stats.Count, Is.EqualTo(migrationResult.Source.HudStats.Count), "Stats.Count");

            Assert.Multiple(() =>
            {
                for (var i = 0; i < migrationResult.Source.HudStats.Count; i++)
                {
                    var expectedStat = migrationResult.Source.HudStats[i];
                    var actualStat = layoutTool.Stats[i];

                    AssertThatStatsAreEqual(actualStat, expectedStat);
                }
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\DH 2-max.xml")]
        public void LayoutV2ToolsPlainBoxStatsWithoutBreakersAreMigrated(string file)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutPlainBoxTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Stats, "Tool stats not found");

            migrationResult.Source.HudStats.Insert(4, new StatInfoBreak());

            Assert.That(layoutTool.Stats.Count, Is.EqualTo(migrationResult.Source.HudStats.Count), "Stats.Count");

            Assert.Multiple(() =>
            {
                for (var i = 0; i < migrationResult.Source.HudStats.Count; i++)
                {
                    var expectedStat = migrationResult.Source.HudStats[i];
                    var actualStat = layoutTool.Stats[i];

                    AssertThatStatsAreEqual(actualStat, expectedStat);
                }
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 338, 115)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 639, 202)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 3, 639, 388)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 4, 342, 467)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 5, 47, 389)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 6, 38, 200)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.BetOnline, EnumGameType.CashHoldem, 1, 340, 119)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.BetOnline, EnumGameType.CashHoldem, 2, 630, 188)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.PokerStars, EnumGameType.CashHoldem, 1, 334, 96)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Poker888, EnumGameType.CashHoldem, 2, 648, 248)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.AmericasCardroom, EnumGameType.CashHoldem, 6, 43, 252)]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 451, 133, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-horiz-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 632, 204, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-horiz-s2)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 329, 10, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 626, 96, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-vert1-s2)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 4, 446, 364, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-vert1-s4)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 5, 148, 284, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-vert1-s5)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 334, 133, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-vert2-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 5, 36, 407, TestName = "LayoutV2ToolsPlainBoxPositionsAreMigrated(6-max-vert2-s6)")]
        public void LayoutV2ToolsPlainBoxPositionsAreMigrated(string file, EnumPokerSites pokerSite, EnumGameType gameType, int seat, double x, double y)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools
                .Where(p => p.ToolType == HudDesignerToolType.PlainStatBox)
                .Cast<HudLayoutPlainBoxTool>()
                .FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Positions, "Tool positions not found in migrated layout");

            var positions = layoutTool.Positions.FirstOrDefault(p => p.PokerSite == pokerSite && p.GameType == gameType);

            Assert.IsNotNull(positions, "Tool positions for specified site, game type not found in migrated layout");

            var seatPosition = positions.HudPositions.FirstOrDefault(p => p.Seat == seat);

            Assert.IsNotNull(seatPosition, "Tool positions for specified seat not found in migrated layout");

            Assert.That(seatPosition.Position.X, Is.EqualTo(x));
            Assert.That(seatPosition.Position.Y, Is.EqualTo(y));
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 1, 403, 148, 142, 19, false, TestName = "LayoutV2Tool4StatBoxUiPositionsAreMigrated(9-max-horizontal-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 3, 624, 348, 142, 19, false, TestName = "LayoutV2Tool4StatBoxUiPositionsAreMigrated(9-max-horizontal-s3")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 6, 208, 439, 142, 19, false, TestName = "LayoutV2Tool4StatBoxUiPositionsAreMigrated(9-max-horizontal-s6")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 1, 524, 59, 30, 63, true, TestName = "LayoutV2Tool4StatBoxUiPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 6, 252, 59, 30, 63, true, TestName = "LayoutV2Tool4StatBoxUiPositionsAreMigrated(6-max-vert1-s6)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", 1, 524, 59, 30, 63, true, TestName = "LayoutV2Tool4StatBoxUiPositionsAreMigrated(6-max-vert2-s1)")]
        public void LayoutV2Tool4StatBoxUiPositionsAreMigrated(string file, int seat, double x, double y, double width, double height, bool isVertical)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutFourStatsBoxTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.IsVertical, Is.EqualTo(isVertical));

                Assert.That(layoutTool.UIPositions.Count, Is.EqualTo((int)migrationResult.Source.TableType));

                var uiPosition = layoutTool.UIPositions.FirstOrDefault(p => p.Seat == seat);

                Assert.IsNotNull(uiPosition, "UiPosition not found in migrated layout");

                Assert.That(uiPosition.Position.X, Is.EqualTo(x));
                Assert.That(uiPosition.Position.Y, Is.EqualTo(y));
                Assert.That(uiPosition.Width, Is.EqualTo(width));
                Assert.That(uiPosition.Height, Is.EqualTo(height));
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml")]
        public void LayoutV2Tools4StatBoxStatsAreMigrated(string file)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutFourStatsBoxTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Stats, "Tool stats not found");

            Assert.That(layoutTool.Stats.Count, Is.LessThanOrEqualTo(4), "Stats.Count");

            Assert.Multiple(() =>
            {
                for (var i = 0; i < migrationResult.Source.HudStats.Count; i++)
                {
                    var expectedStat = migrationResult.Source.HudStats[i];
                    var actualStat = layoutTool.Stats[i];

                    AssertThatStatsAreEqual(actualStat, expectedStat);

                    if (i == 3)
                    {
                        break;
                    }
                }
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 451, 107)]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 632, 178)]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 9, 218, 107)]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 448, 18)]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 4, 334, 372)]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 5, 36, 292)]
        public void LayoutV2Tools4StatBoxPositionsAreMigrated(string file, EnumPokerSites pokerSite, EnumGameType gameType, int seat, double x, double y)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutFourStatsBoxTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Positions, "Tool positions not found in migrated layout");

            var positions = layoutTool.Positions.FirstOrDefault(p => p.PokerSite == pokerSite && p.GameType == gameType);

            Assert.IsNotNull(positions, "Tool positions for specified site, game type not found in migrated layout");

            var seatPosition = positions.HudPositions.FirstOrDefault(p => p.Seat == seat);

            Assert.IsNotNull(seatPosition, "Tool positions for specified seat not found in migrated layout");

            Assert.That(seatPosition.Position.X, Is.EqualTo(x));
            Assert.That(seatPosition.Position.Y, Is.EqualTo(y));
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "TOTAL", "VPIP", Stat.VPIP_EP, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "TOTAL", "VPIP", Stat.VPIP_MP, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "TOTAL", "VPIP", Stat.VPIP_CO, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "TOTAL", "VPIP", Stat.VPIP_BN, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "TOTAL", "VPIP", Stat.VPIP_SB, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "TOTAL", "VPIP", Stat.VPIP_BB, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "COLD CALL", "Cold Call", Stat.ColdCall_EP, 1)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "COLD CALL", "Cold Call", Stat.ColdCall_MP, 1)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "COLD CALL", "Cold Call", Stat.ColdCall_CO, 1)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "COLD CALL", "Cold Call", Stat.ColdCall_BN, 1)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "COLD CALL", "Cold Call", Stat.ColdCall_SB, 1)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.VPIP, false, "COLD CALL", "Cold Call", Stat.ColdCall_BB, 1)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.PFR, false, "UNOPENED", "PFR", Stat.UO_PFR_EP, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.PFR, false, "UNOPENED", "PFR", Stat.UO_PFR_MP, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.PFR, false, "UNOPENED", "PFR", Stat.UO_PFR_CO, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.PFR, false, "UNOPENED", "PFR", Stat.UO_PFR_BN, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.PFR, false, "UNOPENED", "PFR", Stat.UO_PFR_SB, 0)]        
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.S3Bet, false, "TOTAL", "3-bet%", Stat.ThreeBet_EP, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.S3Bet, false, "TOTAL", "3-bet%", Stat.ThreeBet_MP, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.S3Bet, false, "TOTAL", "3-bet%", Stat.ThreeBet_CO, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.S3Bet, false, "TOTAL", "3-bet%", Stat.ThreeBet_BN, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.S3Bet, false, "TOTAL", "3-bet%", Stat.ThreeBet_SB, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.S3Bet, false, "TOTAL", "3-bet%", Stat.ThreeBet_BB, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.AGG, false, "TOTAL", "AGG%", Stat.FlopAGG, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.AGG, false, "TOTAL", "AGG%", Stat.TurnAGG, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.AGG, false, "TOTAL", "AGG%", Stat.RiverAGG, 0)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.AGG, false, "TOTAL", "AGG%", Stat.RecentAgg, 0)]
        public void LayoutV2ToolsHudIndicatorsAreMigrated(string file, Stat baseStat, bool isVertical, string text, string headerText, Stat stat, int index)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTools = migrationResult.Result.LayoutTools
                .OfType<HudLayoutGaugeIndicator>()
                .Where(x => x.BaseStat.Stat == baseStat)
                .ToArray();

            Assert.True(layoutTools.Length >= index + 1, "Tool not found in migrated layout");

            var layoutTool = layoutTools[index];

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.BaseStat.Stat, Is.EqualTo(baseStat), nameof(HudLayoutGaugeIndicator.BaseStat));
                Assert.That(layoutTool.IsVertical, Is.EqualTo(isVertical), nameof(HudLayoutGaugeIndicator.IsVertical));
                Assert.That(layoutTool.Text, Is.EqualTo(text), nameof(HudLayoutGaugeIndicator.Text));
                Assert.That(layoutTool.HeaderText, Is.EqualTo(headerText), nameof(HudLayoutGaugeIndicator.HeaderText));

                Assert.IsNotNull(layoutTool.Stats, nameof(HudLayoutGaugeIndicator.Stats));

                var statInfo = layoutTool.Stats.FirstOrDefault(x => x.Stat == stat);

                Assert.IsNotNull(statInfo, nameof(HudLayoutGaugeIndicator.Stats));
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", Stat.PlayerInfoIcon, false, Stat.NetWon)]
        public void LayoutV2ToolsHudGraphsAreMigrated(string file, Stat baseStat, bool isVertical, Stat stat)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools
                .OfType<HudLayoutGraphTool>()
                .FirstOrDefault(x => x.BaseStat.Stat == baseStat);

            Assert.IsNotNull(layoutTool, "Tools not found in migrated layout");

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.BaseStat.Stat, Is.EqualTo(baseStat), nameof(HudLayoutGraphTool.BaseStat));
                Assert.That(layoutTool.IsVertical, Is.EqualTo(isVertical), nameof(HudLayoutGraphTool.IsVertical));

                Assert.IsNotNull(layoutTool.Stats, nameof(HudLayoutGraphTool.Stats));

                var statInfo = layoutTool.Stats.FirstOrDefault(x => x.Stat == stat);

                Assert.IsNotNull(statInfo, nameof(HudLayoutGraphTool.Stats));
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 1, 440, 143, 100, 13)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 2, 654, 287, 100, 13)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 3, 440, 434, 100, 13)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 4, 282, 434, 100, 13)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 5, 90, 287, 100, 13)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", 6, 282, 143, 100, 13)]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 1, 403, 164, 100, 13, TestName = "LayoutV2ToolsBumperStickersUiPositionsAreMigrated(9-max-horizontal-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 2, 624, 257, 100, 13, TestName = "LayoutV2ToolsBumperStickersUiPositionsAreMigrated(9-max-horizontal-s6")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 1, 405, 41, 100, 13, TestName = "LayoutV2ToolsBumperStickersUiPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 5, 172, 185, 100, 13, TestName = "LayoutV2ToolsBumperStickersUiPositionsAreMigrated(6-max-vert1-s6)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", 1, 410, 164, 100, 13, TestName = "LayoutV2ToolsBumperStickersUiPositionsAreMigrated(6-max-vert2-s2)")]
        public void LayoutV2ToolsBumperStickersUiPositionsAreMigrated(string file, int seat, double x, double y, double width, double height)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutBumperStickersTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.UIPositions.Count, Is.EqualTo((int)migrationResult.Source.TableType));

                var uiPosition = layoutTool.UIPositions.FirstOrDefault(p => p.Seat == seat);

                Assert.IsNotNull(uiPosition, "UiPosition not found in migrated layout");

                Assert.That(uiPosition.Position.X, Is.EqualTo(x));
                Assert.That(uiPosition.Position.Y, Is.EqualTo(y));
                Assert.That(uiPosition.Width, Is.EqualTo(width));
                Assert.That(uiPosition.Height, Is.EqualTo(height));
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 338, 102)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 639, 189)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 3, 639, 375)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 4, 342, 454)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 5, 47, 376)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 6, 38, 187)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.BetOnline, EnumGameType.CashHoldem, 1, 340, 106)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.BetOnline, EnumGameType.CashHoldem, 2, 630, 175)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.PokerStars, EnumGameType.CashHoldem, 1, 334, 83)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.Poker888, EnumGameType.CashHoldem, 2, 648, 235)]
        [TestCase(@"Layouts v.2\Layouts1\ACRsngHYPer 6-max.xml", EnumPokerSites.AmericasCardroom, EnumGameType.CashHoldem, 6, 43, 239)]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 451, 123, TestName = "LayoutV2ToolsBumperStickersPositionsAreMigrated(6-max-horiz-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 632, 194, TestName = "LayoutV2ToolsBumperStickersPositionsAreMigrated(6-max-horiz-s2)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 329, 0, TestName = "LayoutV2ToolsBumperStickersPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 5, 148, 274, TestName = "LayoutV2ToolsBumperStickersPositionsAreMigrated(6-max-vert1-s5)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 334, 123, TestName = "LayoutV2ToolsBumperStickersPositionsAreMigrated(6-max-vert2-s1)")]
        public void LayoutV2ToolsBumperStickersPositionsAreMigrated(string file, EnumPokerSites pokerSite, EnumGameType gameType, int seat, double x, double y)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutBumperStickersTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Positions, "Tool positions not found in migrated layout");

            var positions = layoutTool.Positions.FirstOrDefault(p => p.PokerSite == pokerSite && p.GameType == gameType);

            Assert.IsNotNull(positions, "Tool positions for specified site, game type not found in migrated layout");

            var seatPosition = positions.HudPositions.FirstOrDefault(p => p.Seat == seat);

            Assert.IsNotNull(seatPosition, "Tool positions for specified seat not found in migrated layout");

            Assert.That(seatPosition.Position.X, Is.EqualTo(x));
            Assert.That(seatPosition.Position.Y, Is.EqualTo(y));
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 1, 535, 121, 13, 27, TestName = "LayoutV2ToolsTiltMeterUiPositionsAreMigrated(9-max-horizontal-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 6, 208, 412, 13, 27, TestName = "LayoutV2ToolsTiltMeterUiPositionsAreMigrated(9-max-horizontal-s6")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 1, 542, 121, 13, 27, TestName = "LayoutV2ToolsTiltMeterUiPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 6, 252, 121, 13, 27, TestName = "LayoutV2ToolsTiltMeterUiPositionsAreMigrated(6-max-vert1-s6)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", 2, 756, 265, 13, 27, TestName = "LayoutV2ToolsTiltMeterUiPositionsAreMigrated(6-max-vert2-s2)")]
        public void LayoutV2ToolsTiltMeterUiPositionsAreMigrated(string file, int seat, double x, double y, double width, double height)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutTiltMeterTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.UIPositions.Count, Is.EqualTo((int)migrationResult.Source.TableType));

                var uiPosition = layoutTool.UIPositions.FirstOrDefault(p => p.Seat == seat);

                Assert.IsNotNull(uiPosition, "UiPosition not found in migrated layout");

                Assert.That(uiPosition.Position.X, Is.EqualTo(x));
                Assert.That(uiPosition.Position.Y, Is.EqualTo(y));
                Assert.That(uiPosition.Width, Is.EqualTo(width));
                Assert.That(uiPosition.Height, Is.EqualTo(height));
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 583, 80, TestName = "LayoutV2ToolsToolsTiltMeterPositionsAreMigrated(9-max-horizontal-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 6, 148, 419, TestName = "LayoutV2ToolsToolsTiltMeterPositionsAreMigrated(9-max-horizontal-s6")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 466, 80, TestName = "LayoutV2ToolsToolsTiltMeterPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 6, 36, 166, TestName = "LayoutV2ToolsToolsTiltMeterPositionsAreMigrated(6-max-vert1-s6)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 763, 166, TestName = "LayoutV2ToolsToolsTiltMeterPositionsAreMigrated(6-max-vert2-s2)")]
        public void LayoutV2ToolsToolsTiltMeterPositionsAreMigrated(string file, EnumPokerSites pokerSite, EnumGameType gameType, int seat, double x, double y)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutTiltMeterTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Positions, "Tool positions not found in migrated layout");

            var positions = layoutTool.Positions.FirstOrDefault(p => p.PokerSite == pokerSite && p.GameType == gameType);

            Assert.IsNotNull(positions, "Tool positions for specified site, game type not found in migrated layout");

            var seatPosition = positions.HudPositions.FirstOrDefault(p => p.Seat == seat);

            Assert.IsNotNull(seatPosition, "Tool positions for specified seat not found in migrated layout");

            Assert.That(seatPosition.Position.X, Is.EqualTo(x));
            Assert.That(seatPosition.Position.Y, Is.EqualTo(y));
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 1, 507, 121, 28, 27, TestName = "LayoutV2ToolsPlayerIconUiPositionsAreMigrated(9-max-horizontal-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", 6, 221, 412, 28, 27, TestName = "LayoutV2ToolsPlayerIconUiPositionsAreMigrated(9-max-horizontal-s6")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 1, 514, 121, 28, 27, TestName = "LayoutV2ToolsPlayerIconUiPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", 6, 265, 121, 28, 27, TestName = "LayoutV2ToolsPlayerIconUiPositionsAreMigrated(6-max-vert1-s6)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", 2, 728, 265, 28, 27, TestName = "LayoutV2ToolsPlayerIconUiPositionsAreMigrated(6-max-vert2-s2)")]
        public void LayoutV2ToolsPlayerIconUiPositionsAreMigrated(string file, int seat, double x, double y, double width, double height)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutPlayerIconTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");

            Assert.Multiple(() =>
            {
                Assert.That(layoutTool.UIPositions.Count, Is.EqualTo((int)migrationResult.Source.TableType));

                var uiPosition = layoutTool.UIPositions.FirstOrDefault(p => p.Seat == seat);

                Assert.IsNotNull(uiPosition, "UiPosition not found in migrated layout");

                Assert.That(uiPosition.Position.X, Is.EqualTo(x));
                Assert.That(uiPosition.Position.Y, Is.EqualTo(y));
                Assert.That(uiPosition.Width, Is.EqualTo(width));
                Assert.That(uiPosition.Height, Is.EqualTo(height));
            });
        }

        [Test]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 555, 80, TestName = "LayoutV2ToolsToolsPlayerIconPositionsAreMigrated(9-max-horizontal-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH Harrington M-Zone 9-max Horizontal.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 6, 161, 419, TestName = "LayoutV2ToolsToolsPlayerIconPositionsAreMigrated(9-max-horizontal-s6")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 1, 438, 80, TestName = "LayoutV2ToolsToolsPlayerIconPositionsAreMigrated(6-max-vert1-s1)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_1 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 6, 49, 166, TestName = "LayoutV2ToolsToolsPlayerIconPositionsAreMigrated(6-max-vert1-s6)")]
        [TestCase(@"Layouts v.2\Layouts2\DH 6-max Vertical_2 - IgnitionBodog.xml", EnumPokerSites.Ignition, EnumGameType.CashHoldem, 2, 735, 166, TestName = "LayoutV2ToolsToolsPlayerIconPositionsAreMigrated(6-max-vert2-s2)")]
        public void LayoutV2ToolsToolsPlayerIconPositionsAreMigrated(string file, EnumPokerSites pokerSite, EnumGameType gameType, int seat, double x, double y)
        {
            var migrationResult = GetMigrationV2Result(file);

            Assert.IsNotNull(migrationResult.Result.LayoutTools, "Tools not found in migrated layout");

            var layoutTool = migrationResult.Result.LayoutTools.OfType<HudLayoutPlayerIconTool>().FirstOrDefault();

            Assert.IsNotNull(layoutTool, "Tool not found in migrated layout");
            Assert.IsNotNull(layoutTool.Positions, "Tool positions not found in migrated layout");

            var positions = layoutTool.Positions.FirstOrDefault(p => p.PokerSite == pokerSite && p.GameType == gameType);

            Assert.IsNotNull(positions, "Tool positions for specified site, game type not found in migrated layout");

            var seatPosition = positions.HudPositions.FirstOrDefault(p => p.Seat == seat);

            Assert.IsNotNull(seatPosition, "Tool positions for specified seat not found in migrated layout");

            Assert.That(seatPosition.Position.X, Is.EqualTo(x));
            Assert.That(seatPosition.Position.Y, Is.EqualTo(y));
        }

        private void AssertThatStatsAreEqual(StatInfo actual, StatInfo expected)
        {
            Assert.That(actual.GetType(), Is.EqualTo(expected.GetType()), "Type");
            Assert.That(actual.Stat, Is.EqualTo(expected.Stat), nameof(StatInfo.Stat));
            Assert.That(actual.GroupName, Is.EqualTo(expected.GroupName), nameof(StatInfo.GroupName));
            Assert.That(actual.CurrentColor, Is.EqualTo(expected.CurrentColor), nameof(StatInfo.CurrentColor));
            Assert.That(actual.SettingsAppearance_IsChecked, Is.EqualTo(expected.SettingsAppearance_IsChecked), nameof(StatInfo.SettingsAppearance_IsChecked));
            Assert.That(actual.SettingsPlayerType_IsChecked, Is.EqualTo(expected.SettingsPlayerType_IsChecked), nameof(StatInfo.SettingsPlayerType_IsChecked));
            Assert.That(actual.SettingsAppearanceFontSource, Is.EqualTo(expected.SettingsAppearanceFontSource), nameof(StatInfo.SettingsAppearanceFontSource));
            Assert.That(actual.SettingsAppearanceFontSize, Is.EqualTo(expected.SettingsAppearanceFontSize), nameof(StatInfo.SettingsAppearanceFontSize));
            Assert.That(actual.SettingsAppearanceFontBold_IsChecked, Is.EqualTo(expected.SettingsAppearanceFontBold_IsChecked), nameof(StatInfo.SettingsAppearanceFontBold_IsChecked));
            Assert.That(actual.SettingsAppearanceFontItalic_IsChecked, Is.EqualTo(expected.SettingsAppearanceFontItalic_IsChecked), nameof(StatInfo.SettingsAppearanceFontItalic_IsChecked));
            Assert.That(actual.SettingsAppearanceFontUnderline_IsChecked, Is.EqualTo(expected.SettingsAppearanceFontUnderline_IsChecked), nameof(StatInfo.SettingsAppearanceFontUnderline_IsChecked));

            Assert.That(actual.SettingsAppearanceValueRangeCollection.Count, Is.EqualTo(expected.SettingsAppearanceValueRangeCollection.Count), nameof(StatInfo.SettingsAppearanceValueRangeCollection));

            for (var i = 0; i < actual.SettingsAppearanceValueRangeCollection.Count; i++)
            {
                var actualValueRange = actual.SettingsAppearanceValueRangeCollection[i];
                var expectedValueRange = expected.SettingsAppearanceValueRangeCollection[i];

                Assert.That(actualValueRange.SortOrder, Is.EqualTo(expectedValueRange.SortOrder), nameof(StatInfoOptionValueRange.SortOrder));
                Assert.That(actualValueRange.Value, Is.EqualTo(expectedValueRange.Value), nameof(StatInfoOptionValueRange.Value));
                Assert.That(actualValueRange.ValueRangeType, Is.EqualTo(expectedValueRange.ValueRangeType), nameof(StatInfoOptionValueRange.ValueRangeType));
                Assert.That(actualValueRange.Color, Is.EqualTo(expectedValueRange.Color), nameof(StatInfoOptionValueRange.Color));
            }
        }

        /// <summary>
        /// Gets migration result for the specified file path
        /// </summary>
        /// <param name="layoutFile"></param>
        /// <returns></returns>
        private MigrationV2Result GetMigrationV2Result(string layoutFile)
        {
            var layoutFileFullName = Path.Combine(TestDataFolder, layoutFile);

            var layoutFileInfo = new FileInfo(layoutFileFullName);

            Assert.IsTrue(migratedLayoutsV2.ContainsKey(layoutFileInfo.FullName), $"Key '{layoutFileFullName}' doesn't exist in migration results. Please check test data.");

            var migrationResult = migratedLayoutsV2[layoutFileInfo.FullName];

            return migrationResult;
        }

        /// <summary>
        /// Reads layout v1 from the specified file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private HudLayoutInfo ReadLayoutInfo(string file)
        {
            Assert.IsTrue(File.Exists(file), $"File '{file}' not found");

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xmlSerializer = new XmlSerializer(typeof(HudLayoutInfo));
                var hudLayoutInfo = xmlSerializer.Deserialize(fs) as HudLayoutInfo;
                return hudLayoutInfo;
            }
        }

        private bool ComparePlayerType(HudPlayerType hudPlayerType1, HudPlayerType hudPlayerType2)
        {
            var result = hudPlayerType1.Name == hudPlayerType2.Name && hudPlayerType1.ImageAlias == hudPlayerType2.ImageAlias &&
                hudPlayerType1.EnablePlayerProfile == hudPlayerType2.EnablePlayerProfile && hudPlayerType1.DisplayPlayerIcon == hudPlayerType2.DisplayPlayerIcon &&
                hudPlayerType1.MinSample == hudPlayerType2.MinSample;

            if ((hudPlayerType2.Stats != null && hudPlayerType1.Stats != null && hudPlayerType2.Stats.Count != hudPlayerType1.Stats.Count) ||
                (hudPlayerType2.Stats == null && hudPlayerType1.Stats != null) || (hudPlayerType2.Stats != null && hudPlayerType1.Stats == null))
            {
                return false;
            }
            else if (hudPlayerType2.Stats == null && hudPlayerType1.Stats == null)
            {
                return result;
            }

            var orderedStatList = hudPlayerType2.Stats.OrderBy(x => x.Stat).ToArray();
            var hudPlayerTypeOrderedStatList = hudPlayerType1.Stats.OrderBy(x => x.Stat).ToArray();

            for (var i = 0; i < orderedStatList.Length; i++)
            {
                result &= orderedStatList[i].Equals(hudPlayerTypeOrderedStatList[i]);
            }

            return result;
        }

        private class MigrationV2Result
        {
            public HudLayoutInfo Source { get; set; }

            public HudLayoutInfoV2 Result { get; set; }
        }
    }
}