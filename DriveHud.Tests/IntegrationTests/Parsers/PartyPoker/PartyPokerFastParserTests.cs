﻿using System;
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
using System.Diagnostics;
using System.IO;
using HandHistories.Parser.Parsers.Factory;
using System.Threading;
using System.Globalization;

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

            var succeded = 0;
            var total = 0;

            foreach (var handHistoryFile in handHistoryFiles)
            {
                if (handHistoryFile.FullName.Contains("Tournaments")
                    || handHistoryFile.FullName.Contains("WithInvalid"))
                {
                    continue;
                }

                var handHistory = File.ReadAllText(handHistoryFile.FullName);

                var parserFactory = new HandHistoryParserFactoryImpl();

                var parser = parserFactory.GetFullHandHistoryParser(handHistory);

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
                        }

                        Assert.Fail(e.ToString());
                    }
                }
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", "Jc 4h 6s")]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", "Th Ah Ts 3d")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", "")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "9c 5h 6h Jd 8h")]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", "Kh 7c 2h Ts Tc")]
        public void ParseCommunityCardsTest(string handHistoryFile, string expectedBoard)
        {
            BoardCards expectedCards = BoardCards.FromCards(expectedBoard);
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.CommunityCards, expectedCards);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "2014/01/06 03:05:47Z")]
        [TestCase(TestDataFolder + @"Hands/Tournament157.txt", "2017/05/20 15:33:24Z")]
        public void ParseDateTimeUtcTest(string handHistoryFile, string expectedDateTime)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var dateTime = DateTime.Parse(expectedDateTime, CultureInfo.InvariantCulture).ToUniversalTime();

            Assert.AreEqual(handHistory.DateOfHandUtc, dateTime);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", 3)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", 2)]
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
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", GameType.PotLimitHoldem)]
        [TestCase(TestDataFolder + @"Hands/JackpotSnG.txt", GameType.NoLimitHoldem)]
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
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", 11614201072)]
        [TestCase(TestDataFolder + @"Hands/JabMicroWarmUpPko500Hand.txt", 16847329734)]
        [TestCase(TestDataFolder + @"Hands/NLH-Cash-BB-Dead.txt", 17793933523)]
        [TestCase(TestDataFolder + @"Hands/BountyHunter500Trny.txt", 17957287529)]
        public void ParseHandIdTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.HandId, handId);
        }

        [Test]
        [TestCase(TestDataFolder + @"Hands/HeroBetFolds.txt", "Figarootoo")]
        [TestCase(TestDataFolder + @"Hands/HeroCards.txt", "Peon84")]
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
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", 0.25, 0.5, Currency.USD)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", 0.25, 0.5, Currency.USD)]
        [TestCase(TestDataFolder + @"Hands/JabMicroWarmUpPko500Hand.txt", 500, 1000, Currency.USD)]
        [TestCase(TestDataFolder + @"Hands/JabMicroWarmUpPko500Hand-2.txt", 700, 1400, Currency.USD)]
        [TestCase(TestDataFolder + @"Hands/BountyHunter500Trny.txt", 500, 1000, Currency.USD)]
        [TestCase(TestDataFolder + @"Hands/TournamentBigBlind1000.txt", 500, 1000, Currency.USD)]
        [TestCase(TestDataFolder + @"Hands/TournamentBigBlind1000-2.txt", 600, 1200, Currency.USD)]
        public void ParseLimitTest(string handHistoryFile, decimal smallBlind, decimal bigBlind, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var limit = handHistory.GameDescription.Limit;

            Assert.AreEqual(smallBlind, limit.SmallBlind);
            Assert.AreEqual(bigBlind, limit.BigBlind);
            Assert.AreEqual(currency, limit.Currency);
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
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", "Table  131342 (No DP)")]
        [TestCase(TestDataFolder + @"Hands/Tournament157.txt", "Table #10")]
        [TestCase(TestDataFolder + @"Hands/JackpotSnG.txt", "Table #1")]
        public void ParseTableNameTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.TableName, tableName);
        }

        [Test]
        [TestCase(TestDataFolder + @"GameTypes/FixedLimitHoldem.txt", "aisberg_2013", -0.05, HandActionType.BET, Street.Flop, 1)]
        [TestCase(TestDataFolder + @"GameTypes/NoLimitHoldem.txt", "addy188", 0, HandActionType.FOLD, Street.Turn, 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmaha.txt", "neohacks", -30, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitOmahaHiLo.txt", "ragga22", -0.05, HandActionType.SMALL_BLIND, Street.Preflop, 1)]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4.txt", "PotatoGun", 7.56, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(TestDataFolder + @"GameTypes/PotLimitHoldem.txt", "marlboro man", -0.71, HandActionType.CALL, Street.Flop, 1)]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4_allin.txt", "Moose4Life", 371.87, HandActionType.UNCALLED_BET, Street.Preflop, 1)]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4_allin.txt", "Moose4Life", 265.92, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(TestDataFolder + @"Hands/NLH-Cash-PP-ES-1.txt", "BlackSwam", 0.5, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(TestDataFolder + @"Hands/NLH-Cash-BB-Dead.txt", "MPADAM1980", -0.03, HandActionType.POSTS, Street.Preflop, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions), $"Expected action [{handActionType}, {amount}, {playerName}, {street}] not found");
        }

        [Test]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4_allin.txt", "KrookedTyrant", -116.92, HandActionType.CALL, Street.Preflop, true)]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4_allin2.txt", "FATDAN44", -413.34, HandActionType.RAISE, Street.Preflop, true)]
        public void AllInIsParsed(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, bool isAllin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var action = handHistory
                .HandActions
                .OfType<AllInAction>()
                .Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.SourceActionType == handActionType && x.Amount == amount)
                .FirstOrDefault();

            Assert.That(action?.IsAllIn, Is.EqualTo(isAllin));
        }

        [Test]
        [TestCase(TestDataFolder + @"Hands/HeroBetFolds.txt", 25)]
        [TestCase(TestDataFolder + @"Hands/HeroWins.txt", 25)]
        [TestCase(TestDataFolder + @"Hands/BountyHunter500Trny.txt", 90)]
        public void ParseAnteTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4_allin.txt", "KrookedTyrant")]
        [TestCase(TestDataFolder + @"Hands/NLHoldem2_4_allin2.txt", "bvinnieb")]
        public void PlayerIsLostTest(string handHistoryFile, string playerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

            Assert.IsTrue(player.IsLost);
        }

        [Test]
        [TestCase(TestDataFolder + @"MultipleHands/Edison-2-4-USD-NoLimitHoldem-PartyPokerNJ-5-22-2017.txt", 122, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/Lakewood-2-4-USD-NoLimitHoldem-PartyPokerNJ-5-22-2017.txt", 58, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/Neptune-2-4-USD-NoLimitHoldem-PartyPokerNJ-5-22-2017.txt", 90, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/Orange-2-4-USD-NoLimitHoldem-PartyPokerNJ-5-22-2017.txt", 146, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/Union City-2-4-USD-NoLimitHoldem-PartyPokerNJ-5-22-2017.txt", 466, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/WithInvalid/Kirkcaldy.txt", 3, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/WithInvalid/Rangoon.txt", 3, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/WithInvalid/Supersonic 3_35560068.txt", 13, 0)]
        [TestCase(TestDataFolder + @"Tournaments/Flyweight. $150 Gtd KO (138340269) Table #10.txt", 33, 0)]
        [TestCase(TestDataFolder + @"Tournaments/Jab Micro Warm Up PKO - $500 Gtd (156119820) Table #15.txt", 77, 0)]
        [TestCase(TestDataFolder + @"Tournaments/$0.25 Sit & Go 3-Handed (169131785) Table #1.txt", 27, 0)]
        [TestCase(TestDataFolder + @"MultipleHands/WithInvalid/Barcelona.txt", 47, 1)]
        [TestCase(TestDataFolder + @"MultipleHands/WithInvalid/Corbeil-Essonnes.txt", 46, 1)]
        public void ParseMultipleHandsTest(string handHistoryFile, int numberOfValidHands, int numberOfInvalidHands)
        {
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
        [TestCase(TestDataFolder + @"Hands/Tournament157.txt", GameType.NoLimitHoldem, "138340269", "Flyweight. $150 Gtd KO", 0.55, 0, TournamentSpeed.Regular, 157, 0, "en-US")]
        [TestCase(TestDataFolder + @"Hands/Tournament211.txt", GameType.NoLimitHoldem, "138340262", "Flyweight. $400 Gtd KO", 1.1, 0, TournamentSpeed.Regular, 211, 0, "en-US")]
        [TestCase(TestDataFolder + @"Hands/Tournament2.txt", GameType.NoLimitHoldem, "143085353", "Turbo", 1.1, 0, TournamentSpeed.Turbo, 2, 2.61, "en-US")]
        [TestCase(TestDataFolder + @"Hands/Tournament2.txt", GameType.NoLimitHoldem, "143085353", "Turbo", 1.1, 0, TournamentSpeed.Turbo, 2, 2.61, "nl-NL")]
        [TestCase(TestDataFolder + @"Tournaments/$0.25 Sit & Go 3-Handed (159743601) Table #1.txt", GameType.NoLimitHoldem, "159743600", "$0.25 Sit & Go 3-Handed", 0.25, 0, TournamentSpeed.Regular, 1, 1, "en-US")]
        public void ParseTournamentSummaryTest(string handHistoryFile, GameType gameType, string tournamentId, string tournamentName,
            decimal tournamentBuyIn, decimal tournamentRake, TournamentSpeed speed, int finishPosition, decimal wonAmount, string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.IsTrue(handHistory.GameDescription.IsTournament);
            Assert.AreEqual(tournamentId, handHistory.GameDescription.Tournament.TournamentId, "Tournament id must match.");
            Assert.AreEqual(tournamentName, handHistory.GameDescription.Tournament.TournamentName, "TournamentName must match.");
            Assert.AreEqual(gameType, handHistory.GameDescription.GameType, "GameType must match.");
            Assert.AreEqual(speed, handHistory.GameDescription.Tournament.Speed, "Speed must match.");
            Assert.AreEqual(tournamentBuyIn, handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, "PrizePoolValue must match.");
            Assert.AreEqual(tournamentRake, handHistory.GameDescription.Tournament.BuyIn.Rake, "Rake must match.");
            Assert.AreEqual(finishPosition, handHistory.GameDescription.Tournament.FinishPosition, "FinishPosition must match.");
            Assert.AreEqual(wonAmount, handHistory.GameDescription.Tournament.Winning, "Winning must match.");
        }

        [TestCase(TestDataFolder + @"Hands/BountyHunter500Trny.txt", 0.22)]
        [TestCase(TestDataFolder + @"Hands/JackpotSnG-20181115.txt", 0.25)]
        public void ParseTournamentBuyIn(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(buyin, handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue);
        }

        [Test]
        [TestCase(TestDataFolder + @"Hands/HeroBetFolds.txt", 2108, 0)]
        [TestCase(TestDataFolder + @"Hands/HeroWins.txt", 691, 1632)]
        public void HeroBetWinIsParsedTest(string handHistoryFile, decimal bet, decimal win)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Bet, Is.EqualTo(bet));
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        private HandHistories.Objects.Hand.HandHistory ParseHandHistory(string handHistoryFile)
        {
            var handHistoryText = System.IO.File.ReadAllText(handHistoryFile);

            var parserFactory = new HandHistories.Parser.Parsers.Factory.HandHistoryParserFactoryImpl();

            var parser = parserFactory.GetFullHandHistoryParser(handHistoryText);

            if (parser.SiteName != EnumPokerSites.PartyPoker)
                throw new ArgumentException();

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
