
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

namespace DriveHud.Tests.IntegrationTests.Parsers.TowerTorneos
{
    [TestFixture]
    class TowerTorneosFastParserTest
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData";

        private const string FilePath_Amarillo12 = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\FullHands\TowerTorneosPoker20170602 Amarillo 1-2 No Limit Holdem.txt";
        private const string FilePath_Thornton36 = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\FullHands\TowerTorneosPoker20170602 Thornton 3-6 No Limit Holdem.txt";

        private const string FilePath_Tournament_100_1_Full = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\FullHands\TowerTorneosPoker20170604 Sit & Go 100 + 1 Heads-Up Super Turbo (33663740) No Limit Holdem.txt";
        private const string FilePath_Tournament_100_1_FirstGame = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\SingleHands\TowerTorneosPoker20170604 Sit & Go 100 + 1 First Game.txt";
        private const string FilePath_Tournament_11_1_Full = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\FullHands\TowerTorneosPoker20170604 Sit & Go 11 + 1 10 Seats Rebuy and Add-on  (33663765) No Limit Holdem.txt";
        private const string FilePath_Tournament_11_1_FirstGame = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\SingleHands\TowerTorneosPoker20170604 Sit & Go 11 + 1 First Game.txt";
        private const string FilePath_Tournament_500_30_Full = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\FullHands\TowerTorneosPoker20170604 Sit & Go 500 + 30 Heads-Up Super Turbo (33663813) No Limit Holdem.txt";
        private const string FilePath_Tournament_500_30_FirstGame = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\SingleHands\TowerTorneosPoker20170604 Sit & Go 500 + 30 First Game.txt";
        private const string FilePath_Tournament_500_30_WinGame = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\SingleHands\TowerTorneosPoker20170604 Sit & Go 500 + 30 Win Game.txt";

        private const string FilePath_Tournament_100_1_Summary = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\Summary\TowerTorneosPoker20170604 Sit & Go 100 + 1 Heads-Up Super Turbo (33663740) No Limit Holdem.txt - Summary.txt";
        private const string FilePath_Tournament_11_1_Summary = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\Summary\TowerTorneosPoker20170604 Sit & Go 11 + 1 10 Seats Rebuy and Add-on  (33663765) No Limit Holdem.txt - Summary.txt";
        private const string FilePath_Tournament_500_30_Summary = @"..\..\IntegrationTests\Parsers\TowerTorneos\TestData\Summary\TowerTorneosPoker20170604 Sit & Go 500 + 30 Heads-Up Super Turbo (33663813) No Limit Holdem.txt - Summary.txt";
        
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
        [TestCase(FilePath_Amarillo12, new long[] { 476760690 })]
        [TestCase(FilePath_Thornton36, new long[]
        {
            441498230,
            441498242,
            441498247,
            441498254,
            441498263,
            441498270,
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
        [TestCase(FilePath_Amarillo12, new GameType[]
        {
            GameType.NoLimitHoldem
        })]
        [TestCase(FilePath_Thornton36, new GameType[] 
        {
            GameType.NoLimitHoldem,
            GameType.NoLimitHoldem,
            GameType.NoLimitHoldem,
            GameType.NoLimitHoldem,
            GameType.NoLimitHoldem,
            GameType.NoLimitHoldem,
        })]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType[] gameTypes)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            if (gameTypes.Length != hands.Count())
                throw new InvalidDataException(hands.Count() > gameTypes.Length
                    ? "Too many hands."
                    : "Too many arguments.");

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.GameType == gameTypes[i])
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full)]
        [TestCase(FilePath_Tournament_11_1_Full)]
        [TestCase(FilePath_Tournament_500_30_Full)]
        [TestCase(FilePath_Tournament_100_1_FirstGame)]
        [TestCase(FilePath_Tournament_11_1_FirstGame)]
        [TestCase(FilePath_Tournament_500_30_FirstGame)]
        [TestCase(FilePath_Amarillo12, false)]
        [TestCase(FilePath_Thornton36, false)]
        public void IsTournamentTest(string handHistoryFile, bool mustBeTournamet = true)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            List<bool> allIsTournaments = new List<bool>();

            for (int i = 0; i < hands.Count(); i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.IsTournament)
                    allIsTournaments.Add(true);
                else
                    allIsTournaments.Add(false);
            }

            Assert.True(mustBeTournamet ? !allIsTournaments.Contains(false) : !allIsTournaments.Contains(true));
        }

        //[Test]
        //[TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 30)]
        //public void AnteIsParsedTest(string handHistoryFile, double[] ante)
        //{
        //    throw new NotImplementedException();

        //    var parser = new Poker888FastParserImpl();

        //    string rawHands = File.ReadAllText(handHistoryFile);
        //    var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

        //    var succeded = 0;
        //    var total = hands.Count();

        //    for (int i = 0; i < total; i++)
        //    {
        //        HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

        //        if (hand.GameDescription.Limit.Ante == (decimal)ante[i])
        //            succeded++;
        //    }

        //    Assert.AreEqual(total, succeded);

        //    Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        //}

        //[Test]
        //[TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt")]
        //public void IsAnteTest(string handHistoryFile)
        //{
        //    throw new NotImplementedException();

        //    var parser = new Poker888FastParserImpl();

        //    string rawHands = File.ReadAllText(handHistoryFile);
        //    var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

        //    var succeded = 0;
        //    var total = hands.Count();

        //    for (int i = 0; i < total; i++)
        //    {
        //        HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

        //        if (hand.GameDescription.Limit.IsAnteTable)
        //            succeded++;
        //    }

        //    Assert.AreEqual(total, succeded);

        //    Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        //}

        [Test]
        [TestCase(FilePath_Amarillo12, 2)]
        [TestCase(FilePath_Thornton36, 6)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Limit.BigBlind == (decimal)bigBlind)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Amarillo12, 1)]
        [TestCase(FilePath_Thornton36, 3)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var parser = new Poker888FastParserImpl();

            string rawHands = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(rawHands).ToArray();

            var succeded = 0;
            var total = hands.Count();

            for (int i = 0; i < total; i++)
            {
                HandHistory hand = parser.ParseFullHandHistory(hands[i], true);

                if (hand.GameDescription.Limit.SmallBlind == (decimal)smallBlind)
                    succeded++;
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(FilePath_Amarillo12, Currency.PlayMoney)]
        [TestCase(FilePath_Thornton36, Currency.PlayMoney)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", PokerFormat.Tournament)]
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
        [TestCase(FilePath_Amarillo12, 10)]
        [TestCase(FilePath_Thornton36, 6)]
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
        [TestCase(FilePath_Amarillo12, EnumPokerSites.Poker888)]
        [TestCase(FilePath_Thornton36, EnumPokerSites.Poker888)]
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
        [TestCase(FilePath_Amarillo12, TableTypeDescription.Regular)]
        [TestCase(FilePath_Thornton36, TableTypeDescription.Regular)]
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
        [TestCase(FilePath_Tournament_100_1_Full, 100)]
        [TestCase(FilePath_Tournament_11_1_Full, 11)]
        [TestCase(FilePath_Tournament_500_30_Full, 500)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.PrizePoolValue, buyin);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, Currency.PlayMoney)]
        [TestCase(FilePath_Tournament_11_1_Full, Currency.PlayMoney)]
        [TestCase(FilePath_Tournament_500_30_Full, Currency.PlayMoney)]
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.Currency, currency);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, 1)]
        [TestCase(FilePath_Tournament_11_1_Full, 1)]
        [TestCase(FilePath_Tournament_500_30_Full, 30)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.Rake, rake);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, 33663740)]
        [TestCase(FilePath_Tournament_11_1_Full, 33663765)]
        [TestCase(FilePath_Tournament_500_30_Full, 33663813)]
        public void TournamentIdIsParsedTest(string handHistoryFile, long tournamentId)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.TournamentId, tournamentId.ToString());
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, 2)]
        [TestCase(FilePath_Tournament_11_1_Summary, 2)]
        [TestCase(FilePath_Tournament_500_30_Summary, 2)]
        public void TournamentSummaryPositionIsParsedTest(string handHistoryFile, int position)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.FinishPosition, position);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, 2)]
        [TestCase(FilePath_Tournament_11_1_Summary, 10)]
        [TestCase(FilePath_Tournament_500_30_Summary, 2)]
        public void TournamentSummaryTotalPlayersIsParsedTest(string handHistoryFile, int totalPlayers)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.TotalPlayers, totalPlayers);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, 0)]
        [TestCase(FilePath_Tournament_11_1_Summary, 49.50)]
        [TestCase(FilePath_Tournament_500_30_Summary, 0)]
        public void TournamentSummaryWinningIsParsedTest(string handHistoryFile, decimal winning)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.Winning, winning);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, 0)]
        [TestCase(FilePath_Tournament_11_1_Summary, 0)]
        [TestCase(FilePath_Tournament_500_30_Summary, 0)]
        public void TournamentSummaryRebuyIsParsedTest(string handHistoryFile, decimal rebuy)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.Rebuy, rebuy);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, 100)]
        [TestCase(FilePath_Tournament_11_1_Summary, 11)]
        [TestCase(FilePath_Tournament_500_30_Summary, 500)]
        public void TournamentSummaryBuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.PrizePoolValue, buyin);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, 1)]
        [TestCase(FilePath_Tournament_11_1_Summary, 1)]
        [TestCase(FilePath_Tournament_500_30_Summary, 30)]
        public void TournamentSummaryBuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.BuyIn.Rake, rake);
        }


        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, "kherson_it")]
        [TestCase(FilePath_Tournament_11_1_Summary, "kherson_it")]
        [TestCase(FilePath_Tournament_500_30_Summary, "kherson_it")]
        public void TournamentSummaryHeroIsParsedTest(string handHistoryFile, string heroName)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.Hero.PlayerName, heroName);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Summary, true)]
        [TestCase(FilePath_Tournament_11_1_Summary, true)]
        [TestCase(FilePath_Tournament_500_30_Summary, true)]
        [TestCase(FilePath_Tournament_100_1_Full, false)]
        [TestCase(FilePath_Tournament_11_1_Full, false)]
        [TestCase(FilePath_Tournament_500_30_Full, false)]
        public void TournamentSummaryIsParsedTest(string handHistoryFile, bool isSummary)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.IsSummary, isSummary);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, "Tournament #33663740")]
        [TestCase(FilePath_Tournament_11_1_Full, "Tournament #33663765")]
        [TestCase(FilePath_Tournament_500_30_Full, "Tournament #33663813")]
        public void TournamentNameIsParsedTest(string handHistoryFile, string tournamentName)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.GameDescription.Tournament.TournamentName, tournamentName);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, "kherson_it")]
        [TestCase(FilePath_Tournament_11_1_Full, "kherson_it")]
        [TestCase(FilePath_Tournament_500_30_Full, "kherson_it")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.Hero.PlayerName, heroPlayerName);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, 4)]
        [TestCase(FilePath_Tournament_11_1_Full, 2)]
        [TestCase(FilePath_Tournament_500_30_Full, 9)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.Hero.SeatNumber, seatNumber);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_Full, "2d4d")]
        [TestCase(FilePath_Tournament_11_1_Full, "8c9d")]
        [TestCase(FilePath_Tournament_500_30_Full, "Qh4c")]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string holeCards)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.Hero.HoleCards.ToString(), holeCards);
        }

        [Test]
        [TestCase(FilePath_Amarillo12, new double[] { 200 })]
        [TestCase(FilePath_Thornton36, new double[] { 600, 390, 378, 372, 366, 360 })]
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
        [TestCase(FilePath_Tournament_500_30_FirstGame, 0)]
        [TestCase(FilePath_Tournament_500_30_WinGame, 80)]
        public void HeroWinIsParsedTest(string handHistoryFile, decimal win)
        {
            var parser = new Poker888FastParserImpl();

            string rawHand = File.ReadAllText(handHistoryFile);
            HandHistory hand = parser.ParseFullHandHistory(rawHand, true);

            Assert.AreEqual(win, hand.Hero.Win);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_FirstGame, "#1")]
        [TestCase(FilePath_Tournament_100_1_Full, "#1")]
        [TestCase(FilePath_Tournament_11_1_FirstGame, "#1")]
        [TestCase(FilePath_Tournament_11_1_Full, "#1")]
        [TestCase(FilePath_Tournament_500_30_FirstGame, "#1")]
        [TestCase(FilePath_Tournament_500_30_Full, "#1")]
        [TestCase(FilePath_Tournament_500_30_WinGame, "#1")]
        [TestCase(FilePath_Amarillo12, "Amarillo")]
        [TestCase(FilePath_Thornton36, "Thornton")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.TableName, tableName);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_FirstGame, 2)]
        [TestCase(FilePath_Tournament_100_1_Full, 2)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, 10)]
        [TestCase(FilePath_Tournament_11_1_Full, 10)]
        [TestCase(FilePath_Tournament_500_30_FirstGame, 2)]
        [TestCase(FilePath_Tournament_500_30_Full, 2)]
        [TestCase(FilePath_Tournament_500_30_WinGame, 2)]
        [TestCase(FilePath_Amarillo12, 5)]
        [TestCase(FilePath_Thornton36, 5)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.Players.Count, playersCount);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_FirstGame, 4)]
        [TestCase(FilePath_Tournament_100_1_Full, 4)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, 10)]
        [TestCase(FilePath_Tournament_11_1_Full, 10)]
        [TestCase(FilePath_Tournament_500_30_FirstGame, 9)]
        [TestCase(FilePath_Tournament_500_30_Full, 9)]
        [TestCase(FilePath_Tournament_500_30_WinGame, 4)]
        [TestCase(FilePath_Amarillo12, 9)]
        [TestCase(FilePath_Thornton36, 9)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var hand = ParseFirstGame(handHistoryFile);

            Assert.AreEqual(hand.DealerButtonPosition, dealerButtonPosition);
        }       

        [Test]
        [TestCase(FilePath_Tournament_100_1_FirstGame, HandActionType.CALL, 2)]
        [TestCase(FilePath_Tournament_100_1_FirstGame, HandActionType.CHECK, 3)]
        [TestCase(FilePath_Tournament_100_1_Full, HandActionType.CALL, 2)]
        [TestCase(FilePath_Tournament_100_1_Full, HandActionType.CHECK, 3)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, HandActionType.CALL, 6)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, HandActionType.CHECK, 2)]
        [TestCase(FilePath_Tournament_11_1_Full, HandActionType.CALL, 6)]
        [TestCase(FilePath_Tournament_11_1_Full, HandActionType.CHECK, 2)]
        [TestCase(FilePath_Tournament_11_1_Full, HandActionType.BET, 1)]
        [TestCase(FilePath_Tournament_11_1_Full, HandActionType.FOLD, 7)]
        [TestCase(FilePath_Tournament_11_1_Full, HandActionType.SHOW, 3)]
        [TestCase(FilePath_Tournament_500_30_FirstGame, HandActionType.CALL, 3)]
        [TestCase(FilePath_Tournament_500_30_FirstGame, HandActionType.CHECK, 1)]
        [TestCase(FilePath_Tournament_500_30_Full, HandActionType.BET, 3)]
        [TestCase(FilePath_Tournament_500_30_Full, HandActionType.FOLD, 1)]
        [TestCase(FilePath_Tournament_500_30_Full, HandActionType.SHOW, 0)]
        [TestCase(FilePath_Tournament_500_30_WinGame, HandActionType.CALL, 2)]
        [TestCase(FilePath_Tournament_500_30_WinGame, HandActionType.CHECK, 0)]
        [TestCase(FilePath_Tournament_500_30_WinGame, HandActionType.BET, 1)]
        [TestCase(FilePath_Tournament_500_30_WinGame, HandActionType.FOLD, 1)]
        [TestCase(FilePath_Tournament_500_30_WinGame, HandActionType.SHOW, 0)]
        [TestCase(FilePath_Amarillo12, HandActionType.CALL, 4)]
        [TestCase(FilePath_Amarillo12, HandActionType.CHECK, 1)]
        [TestCase(FilePath_Amarillo12, HandActionType.BET, 1)]
        [TestCase(FilePath_Amarillo12, HandActionType.FOLD, 2)]
        [TestCase(FilePath_Amarillo12, HandActionType.SHOW, 3)]
        [TestCase(FilePath_Thornton36, HandActionType.CALL, 7)]
        [TestCase(FilePath_Thornton36, HandActionType.CHECK, 9)]
        [TestCase(FilePath_Thornton36, HandActionType.BET, 2)]
        [TestCase(FilePath_Thornton36, HandActionType.FOLD, 3)]
        [TestCase(FilePath_Thornton36, HandActionType.SHOW, 1)]
        public void ActionsAreParsedBaseTest(string handHistoryFile, HandActionType handActionType, int numberOfActions)
        {
            var hand = ParseFirstGame(handHistoryFile);

            var actions = hand.HandActions.Where(ha => ha.HandActionType == handActionType);

            Assert.AreEqual(actions.Count(), numberOfActions);
        }

        [Test]
        [TestCase(FilePath_Tournament_100_1_FirstGame, "kherson_it", -10, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_100_1_FirstGame, "OSVALDOJN30", 80, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(FilePath_Tournament_100_1_Full, "kherson_it", 0, HandActionType.CHECK, Street.Turn, 1)]
        [TestCase(FilePath_Tournament_100_1_Full, "OSVALDOJN30", 0, HandActionType.CHECK, Street.Turn, 1)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, "Ivory.1944", -980, HandActionType.ALL_IN, Street.Flop, 1)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, "kherson_it", 0, HandActionType.CHECK, Street.Flop, 1)]
        [TestCase(FilePath_Tournament_11_1_Full, "den.33", -20, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_11_1_FirstGame, "kherson_it", 0, HandActionType.CHECK, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_11_1_Full, "Gilmar1608", -130, HandActionType.BET, Street.Flop, 1)]
        [TestCase(FilePath_Tournament_11_1_Full, "vladimirpluc", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_11_1_Full, "Erasmonn", 0, HandActionType.SHOW, Street.Showdown, 1)]
        [TestCase(FilePath_Tournament_500_30_FirstGame, "kherson_it", -20, HandActionType.CALL, Street.Turn, 1)]
        [TestCase(FilePath_Tournament_500_30_FirstGame, "o.bender1975", 0, HandActionType.CHECK, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_500_30_Full, "o.bender1975", -20, HandActionType.BET, Street.River, 1)]
        [TestCase(FilePath_Tournament_500_30_Full, "kherson_it", 0, HandActionType.FOLD, Street.River, 1)]
        [TestCase(FilePath_Tournament_500_30_WinGame, "o.bender1975", -10, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_500_30_WinGame, "o.bender1975", -20, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Tournament_500_30_WinGame, "kherson_it", -20, HandActionType.BET, Street.Flop, 1)]
        [TestCase(FilePath_Tournament_500_30_WinGame, "o.bender1975", 0, HandActionType.FOLD, Street.Flop, 1)]
        [TestCase(FilePath_Amarillo12, "mouseyyy77", -2, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Amarillo12, "Buzila14", -113, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Amarillo12, "kherson_it", 0, HandActionType.CHECK, Street.Flop, 1)]
        [TestCase(FilePath_Amarillo12, "Buzila14", -85, HandActionType.BET, Street.Flop, 1)]
        [TestCase(FilePath_Amarillo12, "AleksMordas5", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(FilePath_Amarillo12, "kherson_it", 170, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(FilePath_Amarillo12, "sanyi7966", 344, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(FilePath_Thornton36, "Duck515", -6, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(FilePath_Thornton36, "olivella14", 0, HandActionType.CHECK, Street.Flop, 1)]
        [TestCase(FilePath_Thornton36, "kherson_it", -66, HandActionType.BET, Street.Turn, 1)]
        [TestCase(FilePath_Thornton36, "olivella14", 0, HandActionType.FOLD, Street.River, 1)]
        [TestCase(FilePath_Thornton36, "josefonsec", 500, HandActionType.WINS, Street.Summary, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var hand = ParseFirstGame(handHistoryFile);

            var actions = hand.HandActions.Where(ha => ha.Street == street
                && ha.PlayerName == playerName
                && ha.HandActionType == handActionType
                && ha.Amount == amount);

            Assert.AreEqual(actions.Count(), numberOfActions);
        }

        private HandHistory ParseFirstGame(string handHistoryFile)
        {
            var parser = new Poker888FastParserImpl();
            
            string rawHand = File.ReadAllText(handHistoryFile);

            return parser.ParseFullHandHistory(parser.SplitUpMultipleHands(rawHand).FirstOrDefault());
        }
    }
}
