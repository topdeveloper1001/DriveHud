//-----------------------------------------------------------------------
// <copyright file="ExportFunctionsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Ifrastructure;
using DriveHUD.Entities;
using HandHistories.Parser.Parsers.Factory;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    class ExportFunctionsTests
    {
        private const string testFolder = "UnitTests\\TestData";

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        [TestCase("ExportTest-Forum-Source-MTT.xml", "ExportTest-Forum-Result-MTT.txt", EnumPokerSites.Ignition)]
        [TestCase("ExportTest-Forum-Source-Zone.xml", "ExportTest-Forum-Result-Zone.txt", EnumPokerSites.Ignition)]
        [TestCase("ExportTest-Forum-Source-CashWithPost.xml", "ExportTest-Forum-Result-CashWithPost.txt", EnumPokerSites.Ignition)]
        [TestCase("ExportTest-Forum-Source-Straddle.xml", "ExportTest-Forum-Result-Straddle.txt", EnumPokerSites.PokerMaster)]
        [TestCase("ExportTest-Forum-Source-Omaha.txt", "ExportTest-Forum-Result-Omaha.txt", EnumPokerSites.WinningPokerNetwork)]
        public void HandHistoryIsConvertedIntoForumFormat(string sourceFileName, string expectedResultFileName, EnumPokerSites site)
        {
            var sourceFile = Path.Combine(testFolder, sourceFileName);
            var expectedResultFile = Path.Combine(testFolder, expectedResultFileName);

            if (!File.Exists(sourceFile))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", sourceFile));
            }

            if (!File.Exists(expectedResultFile))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", expectedResultFile));
            }

            var handHistoryText = File.ReadAllText(sourceFile);

            var factory = new HandHistoryParserFactoryImpl();

            var parser = factory.GetFullHandHistoryParser(site);

            var handHistory = parser.ParseFullHandHistory(handHistoryText);

            var actualHandHistoryForumText = ExportFunctions.ConvertHHToForumFormat(handHistory);
            var expectedHandHistoryForumText = File.ReadAllText(expectedResultFile);

            Assert.That(actualHandHistoryForumText, Is.EqualTo(expectedHandHistoryForumText));
        }
    }
}