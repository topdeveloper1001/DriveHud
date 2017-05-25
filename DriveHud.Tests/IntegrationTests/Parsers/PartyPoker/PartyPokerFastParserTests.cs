using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Actions;
using DriveHUD.Entities;
using HandHistories.Objects.Players;

namespace DriveHud.Tests.IntegrationTests.Parsers.PartyPoker
{
    class PartyPokerFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\PartyPoker\TestData\";

        [OneTimeSetUp]
        public void Initialize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        public void ParsingDoesNotThrowExceptions(string culture)
        {
            throw new NotImplementedException();
        }

        public void IsCancelledHandTest(string handHistoryFile, bool isCanceled)
        {
            throw new NotImplementedException();
        }

        public void ValidHandTest(string handHistoryFile, bool isValid)
        {
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", "Jc 4h 6s")]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", "Th Ah Ts 3d")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", "")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "9c 5h 6h Jd 8h")]
        public void ParseCommunityCardsTest(string handHistoryFile, string expectedBoard)
        {
            BoardCards expectedCards = BoardCards.FromCards(expectedBoard);
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.CommunityCards, expectedCards);
        }

        public void ParseDateTimeUtcTest(string handHistoryFile, string expectedDateTime)
        {
            throw new NotImplementedException();
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", 3)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", 1)]
        public void ParseDealerPositionTest(string handHistoryFile, int expectedPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.DealerButtonPosition, expectedPosition);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", GameType.FixedLimitHoldem)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", GameType.NoLimitHoldem)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", GameType.PotLimitOmaha)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", GameType.PotLimitOmahaHiLo)]
        public void ParseGameTypeTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.GameDescription.GameType, gameType);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", 13549974403)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", 13551258247)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", 13551973065)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", 13550515602)]
        public void ParseHandIdTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.HandId, handId);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", 13549974403)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", 13551258247)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", 13551973065)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", 13550515602)]
        public void ParseHeroNameTest(string handHistoryFile, string heroName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.Hero.PlayerName, heroName);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", 0.05, 0.1, Currency.USD)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", 0.05, 0.1, Currency.USD)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", 5, 10, Currency.USD)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", 0.05, 0.1, Currency.USD)]
        public void ParseLimitTest(string handHistoryFile, decimal smallBlind, decimal bigBlind, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var limit = handHistory.GameDescription.Limit;

            Assert.AreEqual(limit.SmallBlind, smallBlind);
            Assert.AreEqual(limit.BigBlind, bigBlind);
            Assert.AreEqual(limit.Currency, currency);
        }

        [Test]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4.txt", 0)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", 1)]
        public void ParsePlayersTest(string handHistoryFile, int playersListIndex)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var playerList = GetParsedPlayers(handHistoryFile);
            var expectedPlayers = GetPlayerLists().ElementAt(playersListIndex);

            Assert.AreEqual(expectedPlayers.Count, playerList.Count(), "Player List Count");
            Assert.AreEqual(string.Join(",", expectedPlayers), string.Join(",", playerList));

            Assert.AreEqual(expectedPlayers.Where(x => !x.IsSittingOut).Count(), handHistory.Players.Count, "Parsed HH Player List Count");
            Assert.AreEqual(string.Join(",", expectedPlayers.Where(x => !x.IsSittingOut)), string.Join(",", handHistory.Players));
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", "Rio")]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", "Delhi")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", "Zurich")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "Manila")]
        public void ParseTableNameTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.TableName, tableName);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", "aisberg_2013", -0.05, HandActionType.BET, Street.Flop, 1)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", "addy188", 0, HandActionType.FOLD, Street.Turn, 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", "neohacks", -30, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "ragga22", -0.05, HandActionType.CHECK, Street.Preflop, 1)]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4.txt", "PotatoGun", 7.56, HandActionType.WINS, Street.Summary, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            throw new NotImplementedException();
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "transitions", -490, HandActionType.RAISE, Street.Preflop, true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain5", -1500, HandActionType.RAISE, Street.Preflop, true)]
        public void AllInIsParsed(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, bool isAllin)
        {
            throw new NotImplementedException();
            var handHistory = ParseHandHistory(handHistoryFile);
            var action = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).FirstOrDefault();

            Assert.That(action?.IsAllIn, Is.EqualTo(isAllin));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\6627313 - $10 Freeroll - On Demand\HH20161109 T6627313-G36855145.txt", 10)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", 25)]
        public void ParseAnteTest(string handHistoryFile, decimal ante)
        {
            throw new NotImplementedException();
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", "kachanow")]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", "addy188")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", "SooooLastYr")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "dimatlt633")]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4.txt", "LOLYOU FOLD")]
        public void PlayerIsLostTest(string handHistoryFile, string playerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

            Assert.IsTrue(player.IsLost);
        }

        [Test]
        [TestCase(TestDataFolder + @"MultipleHands/NLHoldem2_4.txt", 1, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/NLHoldem2_4.txt", 1, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/NLHoldem2_4.txt", 1, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/NLHoldem2_4.txt", 1, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/NLHoldem2_4.txt", 1, 0)]
        public void ParseMultipleHandsTest(string handHistoryFile, int numberOfValidHands, int numberOfInvalidHands)
        {
            throw new NotImplementedException();
            var parser = new HandHistories.Parser.Parsers.FastParser.PartyPoker.PartyPokerFastParserImpl();
            int validHands = 0;
            int invalidHands = 0;

            var handHistoryText = System.IO.File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            foreach (var hand in hands)
            {
                try
                {
                    parser.ParseFullHandHistory(hand, true);
                    validHands++;
                }
                catch (HandHistories.Parser.Parsers.Exceptions.InvalidHandException)
                {
                    invalidHands++;
                }
            }

            Assert.AreEqual(numberOfValidHands, validHands);
            Assert.AreEqual(numberOfInvalidHands, invalidHands);
        }


        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\Ante All-In.txt", "artgdev", EnumPosition.EP)]
        public void ParseAllInAnte(string handHistoryFile, string playername, EnumPosition position)
        {
            throw new NotImplementedException();
            //var parsingResult = GetParsingResult(handHistoryFile);

            //var calc = new PlayerStatisticCalculator();
            //var stat = calc.CalculateStatistic(parsingResult, new Players() { Playername = playername, PokersiteId = (short)EnumPokerSites.WinningPokerNetwork, PlayerId = 1 });
            //Assert.That(stat.PositionString, Is.EqualTo(position.ToString()));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\BB All-In.txt", "artgdev")]
        public void ParseBigBlindAllIn(string handHistoryFile, string playername)
        {
            throw new NotImplementedException();
            //var parsingResult = GetParsingResult(handHistoryFile);

            //var calc = new PlayerStatisticCalculator();
            //var stat = calc.CalculateStatistic(parsingResult, new Players() { Playername = playername, PokersiteId = (short)EnumPokerSites.WinningPokerNetwork, PlayerId = 1 });
            //Assert.That(stat.Position, Is.EqualTo(EnumPosition.BB));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\SB All-In.txt", "artgdev")]
        public void ParseSmallBlindAllIn(string handHistoryFile, string playername)
        {
            throw new NotImplementedException();
            //var parsingResult = GetParsingResult(handHistoryFile);

            //var calc = new PlayerStatisticCalculator();
            //var stat = calc.CalculateStatistic(parsingResult, new Players() { Playername = playername, PokersiteId = (short)EnumPokerSites.WinningPokerNetwork, PlayerId = 1 });
            //Assert.That(stat.Position, Is.EqualTo(EnumPosition.SB));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\TournamentHandWithSummary.txt", GameType.PotLimitHoldem, "6626931", 0, 0, TournamentSpeed.Regular)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\TournamentHandWithSummary2.txt", GameType.NoLimitHoldem, "6626931", 1, 0.1, TournamentSpeed.Regular)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\TournamentHandWithSummary3.txt", GameType.NoLimitHoldem, "6626931", 3, 0.3, TournamentSpeed.Regular)]
        public void ParseTournamentSummaryTest(string handHistoryFile, GameType gameType, string tournamentId, decimal tournamentBuyIn, decimal tournamentRake, TournamentSpeed speed)
        {
            throw new NotImplementedException();
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.IsTrue(handHistory.GameDescription.IsTournament);
            Assert.AreEqual(handHistory.GameDescription.Tournament.TournamentId, tournamentId);
            Assert.AreEqual(handHistory.GameDescription.GameType, gameType);
            Assert.AreEqual(handHistory.GameDescription.Tournament.Speed, speed);
            Assert.AreEqual(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, tournamentBuyIn);
            Assert.AreEqual(handHistory.GameDescription.Tournament.BuyIn.Rake, tournamentRake);
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\TournamentHyperTurbo.txt", TournamentSpeed.HyperTurbo)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\HH20170216 T6995792-G39795657.txt", TournamentSpeed.Turbo)]
        public void ParseTournamentSpeedTest(string handHistoryFile, TournamentSpeed speed)
        {
            throw new NotImplementedException();
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(speed, handHistory.GameDescription.Tournament.Speed);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\TournamentHyperTurbo.txt", 525, 1050)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\Ante All-In.txt", 5, 0)]
        public void HeroBetWinIsParsedTest(string handHistoryFile, decimal bet, decimal win)
        {
            throw new NotImplementedException();
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }


        private HandHistories.Objects.Hand.HandHistory ParseHandHistory(string handHistoryFile)
        {
            var handHistoryText = System.IO.File.ReadAllText(handHistoryFile);

            var parserFactory = new HandHistories.Parser.Parsers.Factory.HandHistoryParserFactoryImpl();

            var parser = parserFactory.GetFullHandHistoryParser(handHistoryText);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.First();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        private IEnumerable<Player> GetParsedPlayers(string handHistoryFile)
        {
            var handHistoryText = System.IO.File.ReadAllText(handHistoryFile);

            var parserFactory = new HandHistories.Parser.Parsers.Factory.HandHistoryParserFactoryImpl();

            var parser = parserFactory.GetFullHandHistoryParser(handHistoryText);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.Single();

            var players = parser.ParsePlayers(hand);

            return players;
        }

        private IEnumerable<PlayerList> GetPlayerLists()
        {
            var playerLists = new List<PlayerList>();

            var p1 = new PlayerList()
            {
                new Player("KrookedTyrant", 133.90m, 6),
                new Player("LOLYOU FOLD", 423.45m, 3) { HoleCards = HoleCards.FromCards("6c Ad") },
                new Player("Moose4Life", 551.33m, 4),
                new Player("PotatoGun", 559.66m, 2) { HoleCards = HoleCards.FromCards("Qc Ac") },
                new Player("WhatDayIsIt", 410.67m, 5),
            };

            var p2 = new PlayerList()
            {
                new Player("dimatlt633", 2.06m, 3),
                new Player("ragga22", 17.60m, 1),
            };

            playerLists.Add(p1);
            playerLists.Add(p2);

            return playerLists;
        }
    }
}
