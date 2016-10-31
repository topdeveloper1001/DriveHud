//-----------------------------------------------------------------------
// <copyright file="Pacific888FastParserTests.cs" company="Ace Poker Solutions">
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
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.FastParser._888;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.IntegrationTests.Parsers.Pacific.TestData
{
    [TestFixture]
    class Pacific888FastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\Pacific\TestData\Tournament";

        //[Test]
        public void ParsingDoesNotThrowExceptions()
        {
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
                        }

                        Assert.Fail(e.ToString());
                    }
                }
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 841896912)]
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(gameType));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt")]
        public void IsTournamentTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.IsTournament);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 30)]
        public void AnteIsParsedTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt")]
        public void IsAnteTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.Limit.IsAnteTable);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 250)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 125)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", Currency.USD)]
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", PokerFormat.Tournament)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(PokerFormat.Tournament));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 9)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.SeatType.MaxPlayers, Is.EqualTo(maxPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", EnumPokerSites.Poker888)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Site, Is.EqualTo(EnumPokerSites.Poker888));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", TableTypeDescription.Regular)]
        public void TableDescriptionIsParsedTest(string handHistoryFile, TableTypeDescription tableDescription)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var tableTypeDescription = handHistory.GameDescription.TableType.FirstOrDefault();
            Assert.That(tableTypeDescription, Is.EqualTo(tableDescription));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 0.01)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithRake.txt", 0.45)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", Currency.USD)]
        public void BuyInIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithRake.txt", 0.05)]
        public void RakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 89426293)]
        public void TournmanetIdIsParsedTest(string handHistoryFile, long tournamentId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(tournamentId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Tournament #89426293")]
        public void TournmanetNameIsParsedTest(string handHistoryFile, string tournamentName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentName, Is.EqualTo(tournamentName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Peon_84")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroPlayerName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 4)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.SeatNumber, Is.EqualTo(seatNumber));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", true)]
        public void HeroIsLostIsParsedTest(string handHistoryFile, bool isLost)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.IsLost, Is.EqualTo(isLost));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "5cQd")]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 1000)]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, decimal startingStack)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.StartingStack, Is.EqualTo(startingStack));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 0)]
        public void HeroWinIsParsedTest(string handHistoryFile, decimal win)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Table #2")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.TableName, Is.EqualTo(tableName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 6)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Players.Count, Is.EqualTo(playersCount));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 6)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(dealerButtonPosition));
        }

        // need to decide how to handle dates
        //[Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "2016/10/26 19:59:58")]
        public void DateOfHandUtcIsParsedTest(string handHistoryFile, string dateOfHand)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var dateTime = DateTime.Parse(dateOfHand, CultureInfo.InvariantCulture);
            var dateTimeUtc = dateTime.ToUniversalTime();
            Assert.That(handHistory.DateOfHandUtc, Is.EqualTo(dateTimeUtc));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 6, HandActionType.ANTE)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 4, HandActionType.FOLD)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "leandro5678", -750, HandActionType.RAISE, Street.Preflop,  1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Bakermansam", -250, HandActionType.BIG_BLIND, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Bakermansam", -500, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Bakermansam", -220, HandActionType.ALL_IN, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "leandro5678", -220, HandActionType.ALL_IN, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "leandro5678", 2120, HandActionType.WINS, Street.Summary, 1)]
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
    }
}