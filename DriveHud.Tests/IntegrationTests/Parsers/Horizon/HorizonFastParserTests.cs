//-----------------------------------------------------------------------
// <copyright file="HorizonFastParserTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.Importers.Horizon;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.FastParser.Horizon;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DriveHud.Tests.IntegrationTests.Parsers.Horizon
{
    [TestFixture]
    class HorizonFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\Horizon\TestData";

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

            var parser = new HorizonFastParserImpl();

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
                        }

                        Assert.Fail(e.ToString());
                    }
                }
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 5126023113000128)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", 512716611911000023)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", 5128021934000412)]
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", GameType.NoLimitHoldem)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(gameType));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt")]
        public void IsTournamentTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.IsTournament);
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-Freeroll-Position.txt", 30)]
        public void AnteIsParsedTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 20)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", 20)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", 0.04)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 10)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", 10)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", 0.02)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", Currency.USD)]
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", PokerFormat.CashGame)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(pokerFormat));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", 6)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.SeatType.MaxPlayers, Is.EqualTo(maxPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", EnumPokerSites.Horizon)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Site, Is.EqualTo(pokerSite));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", TableTypeDescription.Regular)]
        public void TableDescriptionIsParsedTest(string handHistoryFile, TableTypeDescription tableDescription)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.TableType.Contains(tableDescription));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", 0.22)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", Currency.USD)]
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Currency, Is.EqualTo(currency));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", 0.03)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", "16611911")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", "16612340")]
        public void TournamentIdIsParsedTest(string handHistoryFile, string tournamentId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(tournamentId.ToString()));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", "$25 Daily Workhorse")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", "Hyper Turbo NLH 3-max - $0.25")]
        public void TournamentNameIsParsedTest(string handHistoryFile, string tournamentName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentName, Is.EqualTo(tournamentName));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", TournamentSpeed.Regular)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", TournamentSpeed.HyperTurbo)]
        public void TournamentSpeedIsParsedTest(string handHistoryFile, TournamentSpeed tournamentSpeed)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Speed, Is.EqualTo(tournamentSpeed));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "AlexTh")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroPlayerName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 3)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.SeatNumber, Is.EqualTo(seatNumber));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-final.txt", true)]
        public void HeroIsLostIsParsedTest(string handHistoryFile, bool isLost)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.IsLost, Is.EqualTo(isLost));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-final.txt", 114)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-Freeroll-Position.txt", 93)]
        public void HeroFinishPositionIsParsedTest(string handHistoryFile, int finishPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.FinishPosition, Is.EqualTo(finishPosition));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-final.txt", 209)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-Freeroll-Position.txt", 307)]
        public void TotalPlayersIsParsedTest(string handHistoryFile, int totalPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TotalPlayers, Is.EqualTo(totalPlayers));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "4s5s")]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "neverwin333", "6h7s")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "H0MMER", "3sAc")]
        public void PlayerHoleCardsIsParsedTest(string handHistoryFile, string playerName, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);
            Assert.IsNotNull(player);
            Assert.IsNotNull(player.HoleCards);
            Assert.That(player.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 900)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", 1.8)]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, decimal startingStack)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.StartingStack, Is.EqualTo(startingStack));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 0)]
        public void HeroWinIsParsedTest(string handHistoryFile, decimal win)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "No Limit Hold'em 10/20 23113")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", "Table 1")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.TableName, Is.EqualTo(tableName));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 5)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Players.Count, Is.EqualTo(playersCount));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 8)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(dealerButtonPosition));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "2018/04/18 12:53:48")]
        public void DateOfHandUtcIsParsedTest(string handHistoryFile, string dateOfHand)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var dateTime = DateTime.Parse(dateOfHand, CultureInfo.InvariantCulture);
            var dateTimeUtc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            Assert.That(handHistory.DateOfHandUtc, Is.EqualTo(dateTimeUtc));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 3, HandActionType.FOLD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 4, HandActionType.CALL)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", 2, HandActionType.BET)]
        public void ActionsAreParsedBaseTest(string handHistoryFile, int numberOfActions, HandActionType handActionType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.That(handHistory.HandActions.Count(x => x.HandActionType == handActionType), Is.EqualTo(numberOfActions));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "neverwin333", -10, HandActionType.SMALL_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "P&R-M-K,NH", -20, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "AlexTh", -20, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "H0MMER", -40, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "AlexTh", 0, HandActionType.FOLD, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "patti40", -40, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "H0MMER", -90, HandActionType.BET, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "neverwin333", -32.25, HandActionType.ALL_IN, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "H0MMER", 392.25, HandActionType.UNCALLED_BET, Street.Turn, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "H0MMER", 109.73, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "neverwin333", 262.92, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-2.txt", "SharkyPirate", -0.02, HandActionType.POSTS, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-2.txt", "SharkyPirate", -0.04, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLN-6-max-5NL.txt", "AlexTh", 0.9, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-Freeroll-Position.txt", "AlexTh", -30, HandActionType.ANTE, Street.Preflop, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-9-max-1.txt", "Cash Game: 23113 [No Limit Hold'em 10/20 23113] - No Limit Holdem Table - 10/20", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-1.txt", "Tournament: 16611911 Buy-In Freeroll : Table 4 - No Limit Holdem - 600/1 200 Ante 100", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-STT-3-max-Hyper.txt", "Tournament: 16612340 Buy-In $0.22 + $0.03 : Table 1 - No Limit Holdem - 15/30", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Horizon\TestData\SingleHands\NLH-MTT-8-max-Freeroll.txt", "Tournament: 16612276 Buy-In Freeroll : Table 19 - No Limit Holdem - 75/150", true)]
        public void ParsedHistoryMatchedWindowTitleTest(string handHistoryFile, string title, bool match)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var parsingResult = new ParsingResult
            {
                Source = handHistory,
                WasImported = true
            };

            ConfigureContainer();

            var HorizonImporter = new HorizonImporterStub();
            var actualMatch = HorizonImporter.Match(title, IntPtr.Zero, parsingResult);

            Assert.That(actualMatch, Is.EqualTo(match));
        }

        private HandHistory ParseHandHistory(string handHistoryFile)
        {
            var parser = new HorizonFastParserImpl();

            var handHistoryText = File.ReadAllText(handHistoryFile);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.Single();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        private void ConfigureContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IEventAggregator, EventAggregator>();

            var dataService = Substitute.For<IDataService>();
            unityContainer.RegisterInstance(dataService);

            var locator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private class HorizonImporterStub : HorizonImporter
        {
            public new bool Match(string title, IntPtr handle, ParsingResult parsingResult)
            {
                return base.Match(title, handle, parsingResult);
            }
        }
    }
}