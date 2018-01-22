//-----------------------------------------------------------------------
// <copyright file="IPokerFastParserTests.cs" company="Ace Poker Solutions">
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
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser.IPoker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DriveHud.Tests.IntegrationTests.Parsers.IPoker
{
    [TestFixture]
    class IPokerFastParserTests : IPokerBaseTests
    {
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
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var cultureInfo = new CultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = cultureInfo;

            var testDataDirectoryInfo = new DirectoryInfo(TestDataFolder);

            var handHistoryFiles = testDataDirectoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);

            var parser = new IPokerFastParserImpl();

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
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 7668185353)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-WildTwister.xml", 7680274162)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-1.xml", 3531068283)]
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", GameType.NoLimitHoldem)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-1.xml", GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(gameType));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 0.02)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 50)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-1.xml", 0.05)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 0.01)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 25)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-1.xml", 0.02)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", Currency.EURO)]
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", PokerFormat.CashGame)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", PokerFormat.Tournament)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-9-max-SNG-Turbo.xml", PokerFormat.Tournament)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-1.xml", PokerFormat.CashGame)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(pokerFormat));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-DON.xml", TournamentSpeed.SuperTurbo)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-9-max-SNG-Turbo.xml", TournamentSpeed.Turbo)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", TournamentSpeed.Regular)]
        public void TournamentSpeedIsParsedTest(string handHistoryFile, TournamentSpeed tournamentSpeed)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Speed, Is.EqualTo(tournamentSpeed));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-DON-won.xml", 1)]
        public void TournamentFinishPositionIsParsedTest(string handHistoryFile, int place)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.FinishPosition, Is.EqualTo(place));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-DON-won.xml", 0.18)]
        public void TournamentWinningIsParsedTest(string handHistoryFile, decimal winning)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Winning, Is.EqualTo(winning));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-DON.xml", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-9-max-SNG-Turbo.xml", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-9-max-SeatType-Test.xml", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-WildTwister.xml", 3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-9-max-cash.xml", 9)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.SeatType.MaxPlayers, Is.EqualTo(maxPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", EnumPokerSites.IPoker)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Site, Is.EqualTo(pokerSite));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", "Peon384")]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-2.xml", "Hero")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroPlayerName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 8)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.SeatNumber, Is.EqualTo(seatNumber));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml")]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-9-max-SNG-Turbo.xml")]
        public void IsTournamentTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.IsTournament);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 5)]
        public void AnteIsParsedTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml")]
        public void IsAnteTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.Limit.IsAnteTable);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 0.91)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-WildTwister.xml", 0.47)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", Currency.USD)]
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 0.09)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-WildTwister.xml", 0.03)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 909153222)]
        public void TournamentIdIsParsedTest(string handHistoryFile, long tournamentId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(tournamentId.ToString()));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", "5h8d")]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-2.xml", "9s9d")]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-2.xml", "papernet", "5hJs")]
        public void PlayerHoleCardsIsParsedTest(string handHistoryFile, string playerName, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);

            Assert.IsNotNull(player, $"{playerName} has not been found");

            Assert.That(player.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-2.xml", "4sKd3c6d7c")]
        public void CommunityCardsAreParsedTest(string handHistoryFile, string communityCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.CommunityCards.ToString(), Is.EqualTo(communityCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 10000)]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, decimal startingStack)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.StartingStack, Is.EqualTo(startingStack));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 4)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-zone-2.xml", 6)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Players.Count, Is.EqualTo(playersCount));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 1)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(dealerButtonPosition));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 3, HandActionType.FOLD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", 4, HandActionType.ANTE)]
        public void ActionsAreParsedBaseTest(string handHistoryFile, int numberOfActions, HandActionType handActionType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var groupedAnteActions = (from handAction in handHistory.HandActions
                                      where handAction.HandActionType == handActionType
                                      group handAction by new { handAction.HandActionType, handAction.PlayerName } into grouped
                                      select grouped.Key).ToArray();

            Assert.That(groupedAnteActions.Length, Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-MTT.xml", "chalky608", -150, HandActionType.RAISE, Street.Preflop, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ipoker-old-format-1.xml", "IPokerTest", 0.23)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ipoker-new-format-1.xml", "IPokerTest", 0.16)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ipoker-new-format-2.xml", "IPokerTest", 2.75)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\ipoker-new-format-3.xml", "FAT1Raccoon", 0.08)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", "Peon384", 0.19)]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max-DON-won.xml", "Peon384", 3205)]
        public void WinActionsAreParsed(string handHistoryFile, string playerName, decimal amount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var winAction = handHistory.HandActions
                .FirstOrDefault(x => x.Street == Street.Summary && x.PlayerName.Equals(playerName) && x.HandActionType == HandActionType.WINS);

            Assert.IsNotNull(winAction, $"WIN action hasn't been found for {playerName}");
            Assert.That(winAction.Amount, Is.EqualTo(amount), $"WIN amount doesn't match expected for {playerName}");
        }

        protected override IHandHistoryParser CreateParser()
        {
            return new IPokerFastParserImpl();
        }
    }
}