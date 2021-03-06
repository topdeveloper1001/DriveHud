﻿//-----------------------------------------------------------------------
// <copyright file="PokerStarsFastParserTests.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.PokerStars;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
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

namespace DriveHud.Tests.IntegrationTests.Parsers.PokerStars.TestData
{
    [TestFixture]
    class PokerStarsFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\PokerStars\TestData";

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

            var parser = new PokerStarsFastParserImpl();

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
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 161248135942)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Bad-Header.txt", 163995445015)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max.txt", 35587765)]
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", GameType.NoLimitHoldem)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max.txt", GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(gameType));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt")]
        public void IsTournamentTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.IsTournament);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-ante.txt", 25)]
        public void AnteIsParsedTest(string handHistoryFile, decimal ante)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Ante, Is.EqualTo(ante));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-ante.txt")]
        public void IsAnteTest(string handHistoryFile)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.Limit.IsAnteTable);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 20)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Cash-INR.txt", 50)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 10)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Cash-INR.txt", 25)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", Currency.Chips)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-INR.txt", Currency.Chips)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Zoom-6-max-0.02-0.05.txt", Currency.USD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Cash-INR.txt", Currency.INR)]
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", PokerFormat.Tournament)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(pokerFormat));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max.txt", 7)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.SeatType.MaxPlayers, Is.EqualTo(maxPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", EnumPokerSites.PokerStars)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Site, Is.EqualTo(pokerSite));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", TableTypeDescription.Regular)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Zoom-6-max-0.02-0.05.txt", TableTypeDescription.FastFold)]
        public void TableDescriptionIsParsedTest(string handHistoryFile, TableTypeDescription tableDescription)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.IsTrue(handHistory.GameDescription.TableType.Contains(tableDescription));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 0.44)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-INR.txt", 0.44)]
        public void BuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", Currency.USD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-INR.txt", Currency.INR)]
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 0.06)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-INR.txt", 0.06)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 1724073465)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", 743144019)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", 734703906)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-USD-HUSnG-10-201011.s0rrow.wins.txt", 329052872)]
        public void TournamentIdIsParsedTest(string handHistoryFile, long tournamentId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(tournamentId.ToString()));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", 63)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-9max-MTT-USD-2.50-201305.KO.v1.txt", 2788)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-9max-SC-MTT-201512.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-EUR-SnG-10-201101.Sample.txt", 8)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-MTT-FPP-100-201309.Finished.Super.Satellite.txt", 2)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLSD-MTT-USD-1000-50-201609.reentry.txt", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-USD-HUSnG-10-201011.s0rrow.wins.txt", 1)]
        public void TournamentSummaryPositionIsParsedTest(string handHistoryFile, int position)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.FinishPosition, Is.EqualTo(position));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", 2)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", 348)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-9max-MTT-USD-2.50-201305.KO.v1.txt", 5305)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-9max-SC-MTT-201512.txt", 77)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-EUR-SnG-10-201101.Sample.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-MTT-FPP-100-201309.Finished.Super.Satellite.txt", 36)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLSD-MTT-USD-1000-50-201609.reentry.txt", 104)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-USD-HUSnG-10-201011.s0rrow.wins.txt", 2)]
        public void TournamentSummaryTotalPlayersIsParsedTest(string handHistoryFile, int totalPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TotalPlayers, Is.EqualTo(totalPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", 5.22)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", 200)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLSD-MTT-USD-1000-50-201609.reentry.txt", 8783.11)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-MTT-FPP-100-201309.Finished.Super.Satellite.txt", 0)]
        public void TournamentSummaryWinningIsParsedTest(string handHistoryFile, decimal winning)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Winning, Is.EqualTo(winning));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", 2)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", 100)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLSD-MTT-USD-1000-50-201609.reentry.txt", 1000)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-MTT-FPP-100-201309.Finished.Super.Satellite.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-EUR-SnG-10-201101.Sample.txt", 9.04)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\TS20161206 T1705825174 No Limit Hold'em Freeroll.txt", 0)]
        public void TournamentSummaryBuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", 0.2)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", 9)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLSD-MTT-USD-1000-50-201609.reentry.txt", 50)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-MTT-FPP-100-201309.Finished.Super.Satellite.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-EUR-SnG-10-201101.Sample.txt", 0.96)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\TS20161206 T1705825174 No Limit Hold'em Freeroll.txt", 0)]
        public void TournamentSummaryBuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", "Hero")]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\8-Game-2max-STT-play-100-201306.homeGame.shootout.txt", "Hero")]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLSD-MTT-USD-1000-50-201609.reentry.txt", "fish_san")]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-MTT-FPP-100-201309.Finished.Super.Satellite.txt", "Zhenik 111")]
        public void TournamentSummaryHeroIsParsedTest(string handHistoryFile, string heroName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\Summary\NLHE-4max-MTT-USD-2-201305.shootout.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", false)]
        public void TournamentSummaryIsParsedTest(string handHistoryFile, bool isSummary)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.IsSummary, Is.EqualTo(isSummary));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "Tournament #1724073465")]
        public void TournamentNameIsParsedTest(string handHistoryFile, string tournamentName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentName, Is.EqualTo(tournamentName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "wisg33")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroPlayerName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 5)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.SeatNumber, Is.EqualTo(seatNumber));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", false)]
        public void HeroIsLostIsParsedTest(string handHistoryFile, bool isLost)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.IsLost, Is.EqualTo(isLost));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "AcQs")]
        public void HeroHoleCardsIsParsedTest(string handHistoryFile, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Cards.txt", "DURKADURDUR", "AcJh")]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Cards-2.txt", "ledzep6028", "5d4s")]
        public void PlayerHoleCardsIsParsedTest(string handHistoryFile, string playerName, string holeCards)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var player = handHistory.Players.FirstOrDefault(x => x.PlayerName == playerName);
            Assert.IsNotNull(player);
            Assert.IsNotNull(player.HoleCards);
            Assert.That(player.HoleCards.ToString(), Is.EqualTo(holeCards));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 1500)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Cash-INR.txt", 2082.32)]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, decimal startingStack)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.StartingStack, Is.EqualTo(startingStack));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 0)]
        public void HeroWinIsParsedTest(string handHistoryFile, decimal win)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "1724073465 1")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.TableName, Is.EqualTo(tableName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 9)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Players.Count, Is.EqualTo(playersCount));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 1)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(dealerButtonPosition));
        }      

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", 8, HandActionType.FOLD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT-ante.txt", 4, HandActionType.ANTE)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "wisg33", -60, HandActionType.RAISE, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "leo.criv", -60, HandActionType.CALL, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "leo.criv", 0, HandActionType.CHECK, Street.Flop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "leo.criv", -75, HandActionType.BET, Street.Turn, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "leo.criv", 75, HandActionType.UNCALLED_BET, Street.Turn, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-STT.txt", "leo.criv", 150, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max.txt", "KingFish", 200, HandActionType.WINS, Street.Summary, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max.txt", "KingFish", 4143.75, HandActionType.UNCALLED_BET, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max.txt", "KingFish", -4143.75, HandActionType.ALL_IN, Street.Preflop, 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-9Stack-7-max-straddle.txt", "Devildareman", -200, HandActionType.STRADDLE, Street.Preflop, 1)]
        public void ActionsAreParsedDetailedTest(string handHistoryFile, string playerName, decimal amount, HandActionType handActionType, Street street, int numberOfActions)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var actions = handHistory.HandActions.Where(x => x.Street == street && x.PlayerName.Equals(playerName) && x.HandActionType == handActionType && x.Amount == amount).ToArray();

            Assert.That(actions.Length, Is.EqualTo(numberOfActions));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Tournament\NLH-9-max-MTT-Turbo.txt", "$0.10 NL Hold'em [360 Players, Turbo] - Blinds $20/$40 - Tournament 1735118766 Table 16 - Logged In as Peon347", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\PokerStars\HandHistory\Cash\NLH-Zoom-6-max-0.02-0.05.txt", "Donati - $0.02/$0.05 USD - No Limit Hold'em", true)]
        public void ParsedHistoryMatchedWindowTitleTest(string handHistoryFile, string title, bool match)
        {
            var handHistory = ParseHandHistory(handHistoryFile);

            var parsingResult = new ParsingResult
            {
                Source = handHistory,
                WasImported = true
            };

            ConfigureContainer();

            var pokerStarsImporter = new PokerStarsImporterStub();
            var actualMatch = pokerStarsImporter.Match(title, IntPtr.Zero, parsingResult);

            Assert.That(actualMatch, Is.EqualTo(match));
        }

        private HandHistory ParseHandHistory(string handHistoryFile)
        {
            var parser = new PokerStarsFastParserImpl();

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

        private class PokerStarsImporterStub : PokerStarsImporter
        {
            protected override string[] WindowClassNames => new[] { string.Empty };

            public new bool Match(string title, IntPtr handle, ParsingResult parsingResult)
            {
                return base.Match(title, handle, parsingResult);
            }

            protected override bool TryMatchWindowClass(IntPtr handle, out string outWindowClassName)
            {
                outWindowClassName = string.Empty;
                return true;
            }
        }
    }
}