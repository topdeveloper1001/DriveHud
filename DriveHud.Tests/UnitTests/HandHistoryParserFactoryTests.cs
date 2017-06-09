//-----------------------------------------------------------------------
// <copyright file="HandHistoryParserFactoryTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Parser.Parsers.Factory;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class HandHistoryParserFactoryTests
    {
        private const string testFolder = "UnitTests\\TestData";

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        [TestCase("iPoker.xml", EnumPokerSites.IPoker)]
        [TestCase("PokerStarsHands.txt", EnumPokerSites.PokerStars)]
        [TestCase("PokerStarsSummary.txt", EnumPokerSites.PokerStars)]
        [TestCase("PokerStarsBadHeader.txt", EnumPokerSites.PokerStars)]
        [TestCase("888PokerHands.txt", EnumPokerSites.Poker888)]
        [TestCase("888PokerSummary.txt", EnumPokerSites.Poker888)]
        [TestCase("ACR-SnG2.txt", EnumPokerSites.WinningPokerNetwork)]
        public void GetFullHandHistoryParserReturnsExpectedParser(string fileName, EnumPokerSites expectedSite)
        {
            var file = Path.Combine(testFolder, fileName);

            if (!File.Exists(file))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", file));
            }

            var handText = File.ReadAllText(file);

            var handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            var parser = handHistoryParserFactory.GetFullHandHistoryParser(handText);

            Assert.That(parser.SiteName, Is.EqualTo(expectedSite));
        }      
    }
}