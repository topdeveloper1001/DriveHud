using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
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

        [Test]
        [TestCase("iPoker.xml", EnumPokerSites.IPoker)]
        [TestCase("PokerStarsHands.txt", EnumPokerSites.PokerStars)]
        [TestCase("888PokerHands.txt", EnumPokerSites.Poker888)]
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