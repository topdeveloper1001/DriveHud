//-----------------------------------------------------------------------
// <copyright file="WinamaxFastParserTests.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.Winamax;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.Horizon;
using HandHistories.Parser.Parsers.FastParser.Winamax;
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

namespace DriveHud.Tests.IntegrationTests.Parsers.Winamax
{
    class WinamaxFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\Winamax\TestData";

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

            var parser = CreateParser();

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

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\MultipleHands\20180423_Leeds 02_play_holdem_no-limit.txt", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\MultipleHands\20180424_Leeds 03_play_holdem_no-limit.txt", 3)]
        public void SplitUpMultipleHandsTests(string handHistoryFile, int handNumbers)
        {
            var handHistory = File.ReadAllText(handHistoryFile);
            var parser = CreateParser();
            var hands = parser.SplitUpMultipleHands(handHistory).ToArray();

            Assert.That(hands.Length, Is.EqualTo(handNumbers), "Hands numbers doesn't match expected.");
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 127564361524498758)]
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(gameType));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt")]
        public void IsTournamentTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.IsTournament);
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 20)]
        public void AnteIsParsedTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 0.02)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 160)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 0.01)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 80)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", Currency.EURO)]
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Currency, Is.EqualTo(currency));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", PokerFormat.CashGame)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", PokerFormat.Tournament)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(pokerFormat));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 8)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", 8)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.SeatType.MaxPlayers, Is.EqualTo(maxPlayers));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", EnumPokerSites.Winamax)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Site, Is.EqualTo(pokerSite));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", TableTypeDescription.Regular)]
        public void TableDescriptionIsParsedTest(string handHistoryFile, TableTypeDescription tableDescription)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.TableType.Contains(tableDescription));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", 4.5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", 0)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", Currency.EURO)]
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Currency, Is.EqualTo(currency));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", 0.5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", 0)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "232395298")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", "232271925")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", "232395298")]
        public void TournamentIdIsParsedTest(string handHistoryFile, string tournamentId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(tournamentId.ToString()));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "Freeroll")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", "MONSTER STACK")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", "Freeroll")]
        public void TournamentNameIsParsedTest(string handHistoryFile, string tournamentName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentName, Is.EqualTo(tournamentName));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", TournamentSpeed.Regular)]
        public void TournamentSpeedIsParsedTest(string handHistoryFile, TournamentSpeed tournamentSpeed)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Speed, Is.EqualTo(tournamentSpeed));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "Peon84")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "Peon84")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", "Peon84")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", "Peon84")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroPlayerName));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", 7)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.SeatNumber, Is.EqualTo(seatNumber));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll-final.txt", true)]
        public void HeroIsLostIsParsedTest(string handHistoryFile, bool isLost)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.IsLost, Is.EqualTo(isLost));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", 70)]
        public void HeroFinishPositionIsParsedTest(string handHistoryFile, int finishPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.FinishPosition, Is.EqualTo(finishPosition));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\Summary\NLH-MTT-9-max-Freeroll-Summary.txt", 198)]
        public void TotalPlayersIsParsedTest(string handHistoryFile, int totalPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TotalPlayers, Is.EqualTo(totalPlayers));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "Js8s")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "6s5h")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", "JsQd")]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "aniktamama", "9h3d")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "Skaann", "8c3c")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "eric4535", "QcAh")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", "One-day", "3s8s")]
        public void PlayerHoleCardsIsParsedTest(string handHistoryFile, string playerName, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);
            Assert.IsNotNull(player);
            Assert.IsNotNull(player.HoleCards);
            Assert.That(player.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 1.98)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 2700)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", 25004)]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, decimal startingStack)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.StartingStack, Is.EqualTo(startingStack));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 0)]
        public void HeroWinIsParsedTest(string handHistoryFile, decimal win)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "Leeds 02")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "Freeroll(232395298)#002")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-8-max-DeepStack.txt", "MONSTER STACK(232271925)#006")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.TableName, Is.EqualTo(tableName));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 7)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Players.Count, Is.EqualTo(playersCount));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 2)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", 6)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(dealerButtonPosition));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "2018/04/23 15:52:38")]
        public void DateOfHandUtcIsParsedTest(string handHistoryFile, string dateOfHand)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var dateTime = DateTime.Parse(dateOfHand, CultureInfo.InvariantCulture);
            var dateTimeUtc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            Assert.That(handHistory.DateOfHandUtc, Is.EqualTo(dateTimeUtc));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 2, HandActionType.BET)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", 14, HandActionType.CALL)]
        public void ActionsAreParsedBaseTest(string handHistoryFile, int numberOfActions, HandActionType handActionType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            Assert.That(handHistory.HandActions.Count(x => x.HandActionType == handActionType), Is.EqualTo(numberOfActions));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "StevenRex", -0.01, HandActionType.SMALL_BLIND, Street.Preflop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "aniktamama", -0.02, HandActionType.BIG_BLIND, Street.Preflop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "LATINO66", -0.02, HandActionType.CALL, Street.Preflop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "Skaann", -0.16, HandActionType.RAISE, Street.Flop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-9-max-2NL.txt", "Skaann", 3.59, HandActionType.WINS, Street.Summary, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "CARDRICH", -20, HandActionType.ANTE, Street.Preflop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "CARDRICH", -98, HandActionType.BIG_BLIND, Street.Preflop, true, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll-Uncalled.txt", "DEBSON-DEB", -520, HandActionType.RAISE, Street.Preflop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll-Uncalled.txt", "DEBSON-DEB", 440, HandActionType.UNCALLED_BET, Street.Preflop, false, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll-Uncalled.txt", "DEBSON-DEB", 480, HandActionType.WINS, Street.Summary, false, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, bool isAllIn, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount && x.IsAllIn == isAllIn).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [TestCase(@"..\..\IntegrationTests\Parsers\Winamax\TestData\SingleHands\NLH-MTT-9-max-Freeroll.txt", "Freeroll(232395298)#002 / 80-160 (ante 20) NL Holdem / Buy-in: 0", true)]
        public void ParsedHistoryMatchedWindowTitleTest(string handHistoryFile, string title, bool match)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var parsingResult = new ParsingResult
            {
                Source = handHistory,
                WasImported = true
            };

            ConfigureContainer();

            var HorizonImporter = new WinamaxImporterStub();
            var actualMatch = HorizonImporter.Match(title, IntPtr.Zero, parsingResult);

            Assert.That(actualMatch, Is.EqualTo(match));
        }

        private HandHistory ParseHandHistory(string handHistoryFile)
        {
            var parser = CreateParser();

            var handHistoryText = File.ReadAllText(handHistoryFile);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.Single();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        private IHandHistoryParser CreateParser()
        {
            return new WinamaxFastParserImpl();
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

        private class WinamaxImporterStub : WinamaxImporter
        {
            public new bool Match(string title, IntPtr handle, ParsingResult parsingResult)
            {
                return base.Match(title, handle, parsingResult);
            }
        }
    }
}