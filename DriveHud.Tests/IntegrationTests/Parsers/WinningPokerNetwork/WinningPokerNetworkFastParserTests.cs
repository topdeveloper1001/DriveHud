//-----------------------------------------------------------------------
// <copyright file="WinningPokerNetworkFastParserTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Parsers.FastParser.Winning;
using Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DriveHud.Tests.IntegrationTests.Parsers.WinningPokerNetwork
{
    [TestFixture]
    class WinningPokerNetworkFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData";

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
                if (handHistoryFile.FullName.Contains("Unfinished") ||
                    handHistoryFile.FullName.Contains("InValid") ||
                    handHistoryFile.FullName.Contains("5-10 PM Hold`em 5-10 6-max - 3 (Hold'em) - 2016-11-08.txt") ||
                    handHistoryFile.FullName.Contains("HH20161109 T6627313-G36855146.txt"))
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
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\CancelledHand.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\ValidHand_1.txt", false)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\ValidHand_2.txt", false)]
        public void IsCancelledHandTest(string handHistoryFile, bool isCanceled)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Cancelled, Is.EqualTo(isCanceled));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\CancelledHand.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\ValidHand_1.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\ValidHand_2.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\InValidHand_1.txt", false)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\ValidHandTests\InValidHand_2.txt", false)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\SNG2-9-max.txt", true)]
        public void ValidHandTest(string handHistoryFile, bool isValid)
        {
            if (isValid)
            {
                Assert.DoesNotThrow(() => ParseHandHistory(handHistoryFile));
            }
            else
            {
                Assert.Throws<InvalidHandException>(() => ParseHandHistory(handHistoryFile));
            }
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-NoBoard.txt", "")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-Flop.txt", "Qs 6h Kh")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-Turn.txt", "Ah 5h Js 5d")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-River.txt", "Th 7c Ks 8d 8h")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Ts 7c 5c 3d Qc")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", "4h 6c 9h 4d 7s")]
        public void ParseCommunityCardsTest(string handHistoryFile, string expectedBoard)
        {
            BoardCards expectedCards = BoardCards.FromCards(expectedBoard);
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.CommunityCards, expectedCards);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-5-10-PM.txt", "2016/11/8 14:44:41")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "2017/05/08 08:32:30")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", "2017/05/08 09:06:37")]
        public void ParseDateTimeUtcTest(string handHistoryFile, string expectedDateTime)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var dateTime = DateTime.Parse(expectedDateTime, CultureInfo.InvariantCulture);

            Assert.AreEqual(handHistory.DateOfHandUtc, dateTime);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-NoBoard.txt", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-Flop.txt", 5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", 4)]
        public void ParseDealerPositionTest(string handHistoryFile, int expectedPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.DealerButtonPosition, expectedPosition);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-NoBoard.txt", GameType.NoLimitHoldem)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO-0.50-1-USD-Flop.txt", GameType.PotLimitOmaha)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO8-25-50-USD-Turn.txt", GameType.PotLimitOmahaHiLo)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", GameType.NoLimitHoldem)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", GameType.NoLimitHoldem)]
        public void ParseGameTypeTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.GameDescription.GameType, gameType);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-NoBoard.txt", 764153469)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO-0.50-1-USD-Flop.txt", 765368106)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO8-25-50-USD-Turn.txt", 763785106)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", 12908866)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", 12909872)]
        public void ParseHandIdTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.HandId, handId);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-5-10-PlayMoney-River-Hero.txt", "Hero")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Peon_84")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", "Peon_84")]
        public void ParseHeroNameTest(string handHistoryFile, string heroName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.Hero.PlayerName, heroName);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-NoBoard.txt", 0.05, 0.1, Currency.USD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO-0.50-1-USD-Flop.txt", 0.5, 1, Currency.USD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO8-25-50-USD-Turn.txt", 25, 50, Currency.USD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", 10, 20, Currency.All)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", 100, 200, Currency.All)]
        public void ParseLimitTest(string handHistoryFile, decimal smallBlind, decimal bigBlind, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var limit = handHistory.GameDescription.Limit;

            Assert.AreEqual(limit.SmallBlind, smallBlind);
            Assert.AreEqual(limit.BigBlind, bigBlind);
            Assert.AreEqual(limit.Currency, currency);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Players\SittingOut.txt", 0)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "$10 Freeroll - On Demand, Table 26")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO-0.50-1-USD-Flop.txt", "(PRR) Ki-Lin - 2")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "1")]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", "1")]
        public void ParseTableNameTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(handHistory.TableName, tableName);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "BlueJeanie", -20, HandActionType.SMALL_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "undertgun", -40, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "transitions", -490, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "1DeadPhish", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "Hero", -1000, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "transitions", 1040, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-Turn.txt", "01123581321", -0.20, HandActionType.BET, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-0.05-0.10-USD-Turn.txt", "needmorefppp", -0.20, HandActionType.CALL, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain5", -1500, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain6", -1500, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain1", -10, HandActionType.SMALL_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain2", -20, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithBets.txt", "Villain9", -300, HandActionType.BET, Street.Turn, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO8-AllIn-UncalledBet.txt", "Granny_Annie", 2696, HandActionType.UNCALLED_BET, Street.Preflop, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\NLH-Tournament-All-In-Uncolled-Bet.txt", "transitions", -490, HandActionType.RAISE, Street.Preflop, true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain5", -1500, HandActionType.RAISE, Street.Preflop, true)]
        public void AllInIsParsed(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, bool isAllin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var action = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).FirstOrDefault();

            Assert.That(action?.IsAllIn, Is.EqualTo(isAllin));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\6627313 - $10 Freeroll - On Demand\HH20161109 T6627313-G36855145.txt", 10)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-WithAnte.txt", 25)]
        public void ParseAnteTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2.txt", "Villain5", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-HeroLost.txt", "Peon_84", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\SnG2\ACR-SnG2-HeroLost.txt", "Peon_84", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\PLO8-AllIn-UncalledBet.txt", "Granny_Annie", false)]
        public void PlayerIsLostTest(string handHistoryFile, string playerName, bool expected)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

            Assert.That(player.IsLost, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Cash\FixedLimitHoldem\5-10 PM Hold`em 5-10 6-max - 3 (Hold'em) - 2016-11-08.txt", 2, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Cash\FixedLimitHoldem\5-10 PM Hold`em 5-10 6-max (Hold'em) - 2016-11-09.txt", 15, 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Cash\FixedLimitHoldem\5-10 PM Hold`em 5-10 6-max (Hold'em) - 2016-11-09 - Unfinished.txt", 14, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\6627313 - $10 Freeroll - On Demand\HH20161109 T6627313-G36855119.txt", 28, 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\6627313 - $10 Freeroll - On Demand\HH20161109 T6627313-G36855145.txt", 7, 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\6627313 - $10 Freeroll - On Demand\HH20161109 T6627313-G36855146.txt", 1, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\6627313 - $10 Freeroll - On Demand\HH20161109 T6627313-G36855120.txt", 75, 0)]
        public void ParseMultipleHandsTest(string handHistoryFile, int numberOfValidHands, int numberOfInvalidHands)
        {
            var parser = new WinningPokerNetworkFastParserImpl();
            int validHands = 0;
            int invalidHands = 0;

            var handHistoryText = File.ReadAllText(handHistoryFile);
            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            foreach (var hand in hands)
            {
                try
                {
                    parser.ParseFullHandHistory(hand, true);
                    validHands++;
                }
                catch (InvalidHandException)
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
            var parsingResult = GetParsingResult(handHistoryFile);

            var calc = new PlayerStatisticCalculator();
            var stat = calc.CalculateStatistic(parsingResult, new Players() { Playername = playername, PokersiteId = (short)EnumPokerSites.WinningPokerNetwork, PlayerId = 1 }, null);
            Assert.That(stat.PositionString, Is.EqualTo(position.ToString()));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\BB All-In.txt", "artgdev")]
        public void ParseBigBlindAllIn(string handHistoryFile, string playername)
        {
            var parsingResult = GetParsingResult(handHistoryFile);

            var calc = new PlayerStatisticCalculator();
            var stat = calc.CalculateStatistic(parsingResult, new Players() { Playername = playername, PokersiteId = (short)EnumPokerSites.WinningPokerNetwork, PlayerId = 1 }, null);
            Assert.That(stat.Position, Is.EqualTo(EnumPosition.BB));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\SB All-In.txt", "artgdev")]
        public void ParseSmallBlindAllIn(string handHistoryFile, string playername)
        {
            var parsingResult = GetParsingResult(handHistoryFile);

            var calc = new PlayerStatisticCalculator();
            var stat = calc.CalculateStatistic(parsingResult, new Players() { Playername = playername, PokersiteId = (short)EnumPokerSites.WinningPokerNetwork, PlayerId = 1 }, null);
            Assert.That(stat.Position, Is.EqualTo(EnumPosition.SB));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\TournamentHandWithSummary.txt", GameType.PotLimitHoldem, "6626931", 0, 0, TournamentSpeed.Regular)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\TournamentHandWithSummary2.txt", GameType.NoLimitHoldem, "6626931", 1, 0.1, TournamentSpeed.Regular)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\Tournament\TournamentHandWithSummary3.txt", GameType.NoLimitHoldem, "6626931", 3, 0.3, TournamentSpeed.Regular)]
        public void ParseTournamentSummaryTest(string handHistoryFile, GameType gameType, string tournamentId, decimal tournamentBuyIn, decimal tournamentRake, TournamentSpeed speed)
        {
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
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.AreEqual(speed, handHistory.GameDescription.Tournament.Speed);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\TournamentHyperTurbo.txt", 525, 1050)]
        [TestCase(@"..\..\IntegrationTests\Parsers\WinningPokerNetwork\TestData\SingleHands\Ante All-In.txt", 5, 0)]
        public void HeroBetWinIsParsedTest(string handHistoryFile, decimal bet, decimal win)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        private ParsingResult GetParsingResult(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var pokersiteId = (short)EnumPokerSites.WinningPokerNetwork;
            var hh = new Handhistory
            {
                Gamenumber = handHistory.HandId,
                GametypeId = (int)handHistory.GameDescription.GameType,
                Handtimestamp = handHistory.DateOfHandUtc,
                HandhistoryVal = handHistory.FullHandHistoryText,
                PokersiteId = pokersiteId,
                Tourneynumber = string.Empty
            };

            var gameType = new Gametypes
            {
                Anteincents = Utils.ConvertToCents(handHistory.GameDescription.Limit.Ante),
                Bigblindincents = Utils.ConvertToCents(handHistory.GameDescription.Limit.BigBlind),
                CurrencytypeId = (short)handHistory.GameDescription.Limit.Currency,
                Istourney = handHistory.GameDescription.IsTournament,
                PokergametypeId = (short)(handHistory.GameDescription.GameType),
                Smallblindincents = Utils.ConvertToCents(handHistory.GameDescription.Limit.SmallBlind),
                Tablesize = (short)handHistory.GameDescription.SeatType.MaxPlayers
            };

            var players = handHistory.Players.Select(player => new Players
            {
                Playername = player.PlayerName,
                PokersiteId = pokersiteId
            }).ToList();

            return new ParsingResult
            {
                HandHistory = hh,
                Players = players,
                GameType = gameType,
                Source = handHistory
            };
        }

        private HandHistory ParseHandHistory(string handHistoryFile)
        {
            var handHistoryText = File.ReadAllText(handHistoryFile);

            var parserFactory = new HandHistoryParserFactoryImpl();

            var parser = parserFactory.GetFullHandHistoryParser(handHistoryText);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.First();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        private IEnumerable<Player> GetParsedPlayers(string handHistoryFile)
        {
            var handHistoryText = File.ReadAllText(handHistoryFile);

            var parserFactory = new HandHistoryParserFactoryImpl();

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
                new Player("Sgtgeo", 724.19m, 1),
                new Player("Cx Agent", 854m, 3) { HoleCards = HoleCards.FromCards("Th 4s") },
                new Player("JB 1226 NYC", 10250.13m, 4),
                new Player("Hero", 603m, 5) { IsSittingOut = true },
                new Player("Lobo125", 817.92m, 6),
            };

            playerLists.Add(p1);

            return playerLists;
        }
    }
}
