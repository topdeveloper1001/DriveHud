
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.FastParser._888;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DriveHud.Tests.IntegrationTests.Parsers.WSPONet
{
    [TestFixture]
    class WSPONetFastParserTest
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData";

        private const string FilePath_Tournament_SuperTurboNoLimits = @"..\..\IntegrationTests\Parsers\WSPO\TestData\FullHands\FullTournament.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game1 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 1.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game2 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 2.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game3 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 3.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game4 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 4.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game5 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 5.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game6 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 6.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game7 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 7.txt";
        private const string FilePath_Tournament_SuperTurboNoLimits_Game8 = @"..\..\IntegrationTests\Parsers\WSPO\TestData\SingleHands\Tornament Game 8.txt";

        private const string FilePath_Tournament_SitAndGo_20_2_FullHand = @"..\..\IntegrationTests\Parsers\WSPO\TestData\FullHands\WSOP.com-NJ20170604 Sit & Go 20 + 2 6 Seats (353029) No Limit Holdem.txt";
        private const string FilePath_Tournament_SitAndGo_20_2_Summary = @"..\..\IntegrationTests\Parsers\WSPO\TestData\Summary\WSOP.com-NJ20170604 Sit & Go 20 + 2 6 Seats (353029) No Limit Holdem.txt - Summary.txt";

        [OneTimeSetUp]
        public void Initialize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        [TestCase("en-US")]
        [TestCase("hu-HU")]
        [TestCase("ru-RU")]
        [TestCase("en-CA")]
        [TestCase("fr-CA")]
        [TestCase("zh-CN")]
        [TestCase("zh-HK")]
        [TestCase("zh-SG")]
        public void ParsingDoesNotThrowExceptions(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            var testDataDirectoryInfo = new DirectoryInfo(TestDataFolder);

            var handHistoryFiles = testDataDirectoryInfo.GetFiles("*.txt", SearchOption.AllDirectories);

            var parser = new Poker888FastParserImpl();

            var succeded = 0;
            var total = 0;

            foreach (var handHistoryFile in handHistoryFiles)
            {
                var handHistory = File.ReadAllText(handHistoryFile.FullName);

                var hands = parser.SplitUpMultipleHands(handHistory).ToArray();

                total += hands.Length;

                var hash = new HashSet<string>();

                foreach (var hand in hands)
                {
                    try
                    {
                        parser.ParseFullHandHistory(hand, true);
                        succeded++;
                    }
                    catch (Exception e)
                    {
                        if (!hash.Contains(handHistoryFile.FullName))
                        {
                            Debug.WriteLine(handHistoryFile.FullName);
                            Debug.WriteLine(hand);
                        }

                        Assert.Fail(e.ToString());
                    }
                }
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new long[]
        {
            439270100,
            439270101,
            439270102,
            439270103,
            439270104,
            439270105,
            439270106,
            439270107,
            439270108,
            439270109,
            439270110,
            439270111,
            439270112,
            439270113,
            439270114,
            439270115,
        })]
        public void HandIdIsParsedTest(string handHistoryFile, long[] handsId)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            if (handsId.Length != hands.Count())
                throw new InvalidDataException(hands.Count() > handsId.Length
                    ? "Too many hands."
                    : "Too many ids.");

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.HandId == handsId[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.GameType == gameType)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits)]
        public void IsTournamentTest(string handHistoryFile)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.IsTournament)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, false)]
        public void IsAnteTest(string handHistoryFile, bool isAnte = true)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Limit.IsAnteTable == isAnte)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new double[] { 50, 50, 50, 50, 50, 50, 50, 50, 50, 50, 60, 60, 60, 60, 60, 60 })]
        public void BigBlindIsParsedTest(string handHistoryFile, double[] bigBlinds)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Limit.BigBlind == (decimal)bigBlinds[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new double[] { 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 30, 30, 30, 30, 30, 30 })]
        public void SmallBlindIsParsedTest(string handHistoryFile, double[] smallBlinds)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Limit.SmallBlind == (decimal)smallBlinds[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, Currency.PlayMoney)]
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Limit.Currency == currency)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, PokerFormat.Tournament)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.PokerFormat == pokerFormat)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, 2)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.SeatType.MaxPlayers == maxPlayers)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, EnumPokerSites.Poker888)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Site == pokerSite)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, TableTypeDescription.Regular)]
        public void TableDescriptionIsParsedTest(string handHistoryFile, TableTypeDescription tableDescription)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);
                var tableTypeDescription = hand.GameDescription.TableType.FirstOrDefault();

                if (tableTypeDescription == tableDescription)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, 10)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Tournament.BuyIn.PrizePoolValue == buyin)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, Currency.PlayMoney)]
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Tournament.BuyIn.Currency == currency)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, 1)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Tournament.BuyIn.Rake == rake)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, 350804)]
        public void TournamentIdIsParsedTest(string handHistoryFile, long tournamentId)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Tournament.TournamentId == tournamentId.ToString())
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 6)]
        public void TournamentSummaryPositionIsParsedTest(string handHistoryFile, int position)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.FinishPosition, position);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 6)]
        public void TournamentSummaryTotalPlayersIsParsedTest(string handHistoryFile, int totalPlayers)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.TotalPlayers, totalPlayers);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 0)]
        public void TournamentSummaryWinningIsParsedTest(string handHistoryFile, decimal winning)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.Winning, winning);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 0)]
        public void TournamentSummaryRebuyIsParsedTest(string handHistoryFile, decimal rebuy)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.Rebuy, rebuy);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 0)]
        public void TournamentSummaryAddonIsParsedTest(string handHistoryFile, decimal addon)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.Addon, addon);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 20)]
        public void TournamentSummaryBuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.PrizePoolValue, buyin);
        }

        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, 2)]
        public void TournamentSummaryBuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.Rake, rake);
        }


        [Test]
        [TestCase(FilePath_Tournament_SitAndGo_20_2_Summary, "AlexPoker92")]
        public void TournamentSummaryHeroIsParsedTest(string handHistoryFile, string heroName)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.Hero.PlayerName, heroName);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, false)]
        public void TournamentSummaryIsParsedTest(string handHistoryFile, bool isSummary)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Tournament.IsSummary == isSummary)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, "Tournament #350804")]
        public void TournamentNameIsParsedTest(string handHistoryFile, string tournamentName)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Tournament.TournamentName == tournamentName)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, "AlexPoker92")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Hero.PlayerName == heroPlayerName)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, 4)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Hero.SeatNumber == seatNumber)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, false)]
        public void HeroIsLostIsParsedTest(string handHistoryFile, bool isLost)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Hero.IsLost == isLost)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new string[]
            {
                "6dKh",
                "6h8s",
                "2cKc",
                "4d5s",
                "QcQs",
                "3c7c",
                "7hKd",
                "8h8s",
                "QdKc",
                "5c8c",
                "3dKs",
                "JcQd",
                "QhTs",
                "5s6h",
                "3s4h",
                "7sAh",
            })]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string[] holeCards)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Hero.HoleCards.ToString() == holeCards[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new double[] { 1500, 1550, 1575, 1525, 1550, 1600, 1625, 1575, 1600, 1650, 1675, 1615, 1645, 1705, 1735, 1795, })]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, double[] startingStack)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Hero.StartingStack == (decimal)startingStack[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new double[] { 100, 75, 0, 75, 100, 75, 0, 75, 100, 75, 0, 90, 120, 90, 120, 90 })]
        public void HeroWinIsParsedTest(string handHistoryFile, double[] win)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Hero.Win == (decimal)win[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, "#1")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.TableName == tableName)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, 2)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.Players.Count == playersCount)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits, new int[] { 4, 9, 4, 9, 4, 9, 4, 9, 4, 9, 4, 9, 4, 9, 4, 9 })]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int[] dealerButtonPosition)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.DealerButtonPosition == dealerButtonPosition[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }        

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, 1, HandActionType.SMALL_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, 1, HandActionType.BIG_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, 1, HandActionType.CALL)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, 7, HandActionType.CHECK)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game2, 1, HandActionType.SMALL_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game2, 1, HandActionType.BIG_BLIND)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game3, 1, HandActionType.SMALL_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game3, 1, HandActionType.BIG_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game3, 1, HandActionType.CALL)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game3, 7, HandActionType.CHECK)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game4, 1, HandActionType.FOLD)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game5, 1, HandActionType.RAISE)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game6, 1, HandActionType.SMALL_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game6, 1, HandActionType.BIG_BLIND)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game7, 1, HandActionType.SHOW)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game7, 1, HandActionType.MUCKS)]

        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game8, 1, HandActionType.SMALL_BLIND)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game8, 1, HandActionType.BIG_BLIND)]
        public void ActionsAreParsedBaseTest(string handHistoryFile, int numberOfActions, HandActionType handActionType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var actions = handHistory.HandActions.Where(act => act.HandActionType == handActionType);

            Assert.That(actions.Count(), Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, "AlexPoker92", -25, HandActionType.SMALL_BLIND, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, "Confl1ct10n", -50, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, "AlexPoker92", -25, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_SuperTurboNoLimits_Game1, "AlexPoker92", 100, HandActionType.WINS, Street.Summary, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        private HandHistory ParseHandHistory(string handHistoryFile)
        {
            var parser = new Poker888FastParserImpl();

            var handHistoryText = File.ReadAllText(handHistoryFile);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.Single();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        private HandHistory ParseFirstGame(string handHistoryFile)
        {
            var parser = new Poker888FastParserImpl();

            string rawHand = File.ReadAllText(handHistoryFile);

            return parser.ParseFullHandHistory(parser.SplitUpMultipleHands(rawHand).FirstOrDefault());
        }
    }
}
