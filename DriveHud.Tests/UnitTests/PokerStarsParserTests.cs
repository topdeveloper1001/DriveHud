using NUnit.Framework;
using System;
using System.Linq;
using System.Diagnostics;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture(Category = "PokerStars")]
    public class PokerStarsParserTests
    {
        public void TestParser(string db, bool isTournament)
        {
            var pt4Repository = new PT4Repository("Server=127.0.0.1;Port=5432;Database=" + db + ";User Id=sa;Password=sa12345;");

            var allHands = isTournament ? pt4Repository.GetAllTournamentsHands().ToList() : pt4Repository.GetAllCashHands().ToList();

            Assert.IsTrue(allHands.Count > 0);

            var parser = new PokerStarsFastParserImpl();

            var succeded = 0;

            foreach (var hand in allHands)
            {
                try
                {
                    parser.ParseFullHandHistory(hand.History, true);
                    succeded++;
                }
                catch (Exception e)
                {
                    Assert.Fail(e.ToString());
                }
            }

            Debug.WriteLine("Hands parsed: {0}", allHands.Count);

            Assert.AreEqual(allHands.Count, succeded);
        }

        [Test]
        public void TestCashPT4DB()
        {
            TestParser("PT4_2015_10_08_071733", false);
        }

        [Test]
        public void TestTournamentPT4DB()
        {
            TestParser("PT4_2015_10_08_071733", true);
        }

        [Test]
        public void TestCashJPT()
        {
            TestParser("JPT", false);
        }

        [Test]
        public void TestTournamentJPT()
        {
            TestParser("JPT", true);
        }

        private const string testFolder = "UnitTests\\TestData";

        [Test]
        [TestCase("PokerStarsOmahaHiLo.txt")]
        public void TestPokerStarsOmahaHiLo(string fileName)
        {
            var handText = GetHandText(fileName);

            var parser = new PokerStarsFastParserImpl();
            var handHistory = parser.ParseFullHandHistory(handText);

            Assert.IsNull(handHistory.Exception);
        }

        private string GetHandText(string fileName)
        {
            var file = Path.Combine(testFolder, fileName);

            if (!File.Exists(file))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", file));
            }

            var handText = File.ReadAllText(file);

            return handText;
        }

    }
}