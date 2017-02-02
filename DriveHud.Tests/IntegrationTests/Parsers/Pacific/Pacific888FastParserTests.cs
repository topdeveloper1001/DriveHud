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
using System.Threading;

namespace DriveHud.Tests.IntegrationTests.Parsers.Pacific.TestData
{
    [TestFixture]
    class Pacific888FastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\Pacific\TestData";

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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 841896912)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 445357327)]        
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", GameType.NoLimitHoldem)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", GameType.NoLimitHoldem)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 0.02)]
        //[TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - port.txt", 0.02)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 125)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 0.01)]
        //[TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - port.txt", 0.01)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", Currency.USD)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NLH - 3 max - Play Money.txt", Currency.PlayMoney)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", Currency.USD)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NLH - 3 max - Play Money.txt", 3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NLH - 4 max - Real Money.txt", 4)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NLH - 5 max - Real Money.txt", 5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NLH - 8 max - Real Money.txt", 8)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 9)]
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
        public void BuyInCurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithRake.txt", 0.05)]
        public void BuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 89426293)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", 89426293)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", 34471039)]
        public void TournamentIdIsParsedTest(string handHistoryFile, long tournamentId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(tournamentId.ToString()));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", 30)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", 7)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-6max-SNG-0.10-201206.summary.txt", 3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-FREE-MTT-$75Freeroll20120217-Summary.txt", 71)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-Freeroll-MTT-201205.russian.txt", 440)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-MTT-EUR-0.01-201206.freeroll.buyin.summary.txt", 93)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-PM-MTT-10-20120221-Summary.txt", 39)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 59)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3-201111.real.money.txt", 46)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-5-201111.cashed.txt", 7)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-STT-20-201205.euro.style.txt", 3)]
        public void TournamentSummaryPositionIsParsedTest(string handHistoryFile, int position)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.FinishPosition, Is.EqualTo(position));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", 283)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", 254)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-6max-SNG-0.10-201206.summary.txt", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-FREE-MTT-$75Freeroll20120217-Summary.txt", 3854)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-Freeroll-MTT-201205.russian.txt", 2323)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-MTT-EUR-0.01-201206.freeroll.buyin.summary.txt", 664)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-PM-MTT-10-20120221-Summary.txt", 175)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 468)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3-201111.real.money.txt", 315)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-5-201111.cashed.txt", 254)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-STT-20-201205.euro.style.txt", 6)]
        public void TournamentSummaryTotalPlayersIsParsedTest(string handHistoryFile, int totalPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.TotalPlayers, Is.EqualTo(totalPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", 50.8)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-6max-SNG-0.10-201206.summary.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-FREE-MTT-$75Freeroll20120217-Summary.txt", 0.3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-Freeroll-MTT-201205.russian.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-MTT-EUR-0.01-201206.freeroll.buyin.summary.txt", 1.20)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-PM-MTT-10-20120221-Summary.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3-201111.real.money.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-5-201111.cashed.txt", 50.80)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-STT-20-201205.euro.style.txt", 0)]
        public void TournamentSummaryWinningIsParsedTest(string handHistoryFile, decimal winning)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Winning, Is.EqualTo(winning));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 3.15)]
        public void TournamentSummaryRebuyIsParsedTest(string handHistoryFile, decimal rebuy)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Rebuy, Is.EqualTo(rebuy));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 3.15)]
        public void TournamentSummaryAddonIsParsedTest(string handHistoryFile, decimal addon)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.Addon, Is.EqualTo(addon));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", 0.01)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", 5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-6max-SNG-0.10-201206.summary.txt", 0.09)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-FREE-MTT-$75Freeroll20120217-Summary.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-Freeroll-MTT-201205.russian.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-MTT-EUR-0.01-201206.freeroll.buyin.summary.txt", 0.01)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-PM-MTT-10-20120221-Summary.txt", 10)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 3.15)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3-201111.real.money.txt", 3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-5-201111.cashed.txt", 5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-STT-20-201205.euro.style.txt", 20)]
        public void TournamentSummaryBuyInPrizePoolIsParsedTest(string handHistoryFile, decimal buyin)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue, Is.EqualTo(buyin));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", 0.5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-6max-SNG-0.10-201206.summary.txt", 0.01)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-FREE-MTT-$75Freeroll20120217-Summary.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-Freeroll-MTT-201205.russian.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-MTT-EUR-0.01-201206.freeroll.buyin.summary.txt", 00)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-PM-MTT-10-20120221-Summary.txt", 1)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3.15-201111.rebuys.addons.txt", 0.35)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-3-201111.real.money.txt", 0.3)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-MTT-5-201111.cashed.txt", 0.5)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\NLHE-USD-STT-20-201205.euro.style.txt", 1)]
        public void TournamentSummaryBuyInRakeIsParsedTest(string handHistoryFile, decimal rake)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.BuyIn.Rake, Is.EqualTo(rake));
        }


        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", "Peon_84")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament Win - Summary.txt", "Hero")]
        public void TournamentSummaryHeroIsParsedTest(string handHistoryFile, string heroName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\Summary\Tournament - Summary.txt", true)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", false)]
        public void TournamentSummaryIsParsedTest(string handHistoryFile, bool isSummary)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Tournament.IsSummary, Is.EqualTo(isSummary));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "Tournament #89426293")]
        public void TournamentNameIsParsedTest(string handHistoryFile, string tournamentName)
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", false)]
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 2)]
        public void HeroStartingStackIsParsedTest(string handHistoryFile, decimal startingStack)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.StartingStack, Is.EqualTo(startingStack));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 0)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 0.06)]
        public void HeroWinIsParsedTest(string handHistoryFile, decimal win)
        {        
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.Win, Is.EqualTo(win));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "#2")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", "Amersfoort")]
        public void TableNameIsParsedTest(string handHistoryFile, string tableName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.TableName, Is.EqualTo(tableName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 7)]
        public void PlayersCountIsParsedTest(string handHistoryFile, int playersCount)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Players.Count, Is.EqualTo(playersCount));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", 6)]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", 9)]
        public void DealerButtonPositionIsParsedTest(string handHistoryFile, int dealerButtonPosition)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(dealerButtonPosition));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "2016/10/26 18:59:58")]
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\NHL - 9 max - 0.01 - 0.02 - rus.txt", "2016/11/22 23:36:47")]
        public void DateOfHandUtcIsParsedTest(string handHistoryFile, string dateOfHand)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            var dateTimeUtc = DateTime.Parse(dateOfHand, CultureInfo.InvariantCulture);
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
        [TestCase(@"..\..\IntegrationTests\Parsers\Pacific\SingleHands\TournamentWithAnte.txt", "leandro5678", -750, HandActionType.RAISE, Street.Preflop, 1)]
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