//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// PlayerStatistic integration tests
    /// </summary>
    [TestFixture]
    class PlayerStatisticTests : PlayerStatisticBaseTests
    {
        protected override string TestDataFolder
        {
            get
            {
                return @"..\..\IntegrationTests\Importers\TestData\PokerStars\PlayerStatistic";
            }
        }

        [TestCase(@"General-HandHumber-1.xml", EnumPokerSites.BetOnline, "RiverGod37", 315575540L)]
        [TestCase(@"General-HandHumber-2.xml", EnumPokerSites.BetOnline, "ChiTownMCA", 366526206)]
        public void GameNumberIsStored(string fileName, EnumPokerSites pokerSite, string playerName, long expected)
        {
            AssertThatStatIsCalculated(x => x.GameNumber, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CO-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CO-DidColdCallIp-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CO-DidColdCallIp-AllIn.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BB-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-MP-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-SB-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-SB-DidColdCallIp-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidColdCallIpIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidColdCallIp, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CO-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CO-DidColdCallIp-AllIn.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DidNotColdCall.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DidNotColdCall-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BB-DidColdCall.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BB-DidColdCall-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-MP-DidColdCall.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CO-DidColdCall.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DidColdCall.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void DidColdCallIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Didcoldcall, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-SB-PfrOop-2-max.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void PfrOopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.PfrOop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-BTN-Cards.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "AcJh")]
        public void CardsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string expected)
        {
            AssertThatStatIsCalculated(x => x.Cards, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Positions.xml", EnumPokerSites.IPoker, "Peon84", "BTN", 3)]
        [TestCase(@"Hero-Positions.xml", EnumPokerSites.IPoker, "Peon84", "CO", 1)]
        [TestCase(@"Hero-Positions.xml", EnumPokerSites.IPoker, "Peon84", "MP", 4)]
        [TestCase(@"Hero-Positions.xml", EnumPokerSites.IPoker, "Peon84", "EP", 7)]
        [TestCase(@"Hero-Positions.xml", EnumPokerSites.IPoker, "Peon84", "SB", 2)]
        [TestCase(@"Hero-Positions.xml", EnumPokerSites.IPoker, "Peon84", "BB", 3)]
        public void AllPositionsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string position, int expectedHandsAmount)
        {
            using (var perfScope = new PerformanceMonitor("AllPositionsAreImported"))
            {
                var playerstatistic = new List<Playerstatistic>();

                var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
                playerStatisticRepository.Store(Arg.Is<Playerstatistic>(x => GetPlayerstatisticCollectionFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.True(playerstatistic.Count > 0, $"Player '{playerName}' has not been found");

                var playerstatisticByPosition = playerstatistic
                    .GroupBy(x => x.PositionString)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                Assert.True(playerstatisticByPosition.ContainsKey(position), $"Hands on {position} has not been found");

                Assert.That(playerstatisticByPosition[position].Length, Is.EqualTo(expectedHandsAmount), $"Hands on {position} doesn't match expected");
            }
        }

        [TestCase(@"Hero-Position-EP-1.xml", EnumPokerSites.BetOnline, "Peon84", "EP")]
        [TestCase(@"Hero-Position-EP-2.xml", EnumPokerSites.BetOnline, "Peon84", "EP")]
        [TestCase(@"Hero-Position-EP-3.xml", EnumPokerSites.IPoker, "Peon_184", "EP")]
        [TestCase(@"Hero-Position-EP-4.txt", EnumPokerSites.PokerStars, "Peon347", "EP")]
        [TestCase(@"Hero-Position-EP-5.xml", EnumPokerSites.Ignition, "Hero", "EP")]
        [TestCase(@"Hero-Position-EP-6.xml", EnumPokerSites.Ignition, "Hero", "EP")]
        [TestCase(@"Hero-Position-EP-7.xml", EnumPokerSites.BetOnline, "HeroTest", "EP")]
        [TestCase(@"Hero-Position-CO-1.xml", EnumPokerSites.IPoker, "Peon_184", "CO")]
        [TestCase(@"Hero-Position-CO-2.xml", EnumPokerSites.BetOnline, "Hero", "CO")]
        [TestCase(@"Hero-Position-MP-1.xml", EnumPokerSites.BetOnline, "Peon84", "MP")]
        [TestCase(@"Hero-Position-MP-2.xml", EnumPokerSites.BetOnline, "Peon84", "MP")]
        [TestCase(@"Hero-Position-MP-3.xml", EnumPokerSites.BetOnline, "Peon84", "MP")]
        [TestCase(@"Hero-Position-MP-4.xml", EnumPokerSites.Ignition, "Hero", "MP")]
        [TestCase(@"DURKADURDUR-CO-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "CO")]
        [TestCase(@"DURKADURDUR-EP-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "EP")]
        [TestCase(@"DURKADURDUR-SB-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "BTN")]
        [TestCase(@"AsX4-SB-Position.txt", EnumPokerSites.Winamax, "as x 4", "SB")]
        [TestCase(@"Ginac808-EP-Position.xml", EnumPokerSites.BetOnline, "ginac808", "EP")]
        [TestCase(@"Ginac808-MP-Position.xml", EnumPokerSites.BetOnline, "ginac808", "MP")]
        [TestCase(@"AlexTh-CO-Position.txt", EnumPokerSites.Horizon, "AlexTh", "CO")]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2097148", "BTN")]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2047673", "EP")]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2129138", "EP")]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2016412", "MP")]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2089715", "MP")]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2118877", "CO")]
        public void PositionsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string expectedPosition)
        {
            AssertThatStatIsCalculated(x => x.PositionString, fileName, pokerSite, playerName, expectedPosition);
        }

        [TestCase(@"Hero-Position-EP-1.xml", EnumPokerSites.IPoker, "Peon84", false)]
        [TestCase(@"Hero-Position-EP-2.xml", EnumPokerSites.IPoker, "Peon84", false)]
        [TestCase(@"Hero-Position-MP-1.xml", EnumPokerSites.IPoker, "Peon84", false)]
        [TestCase(@"Hero-Position-MP-2.xml", EnumPokerSites.IPoker, "Peon84", false)]
        [TestCase(@"Hero-Position-MP-3.xml", EnumPokerSites.IPoker, "Peon84", false)]
        [TestCase(@"DURKADURDUR-CO-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", true)]
        [TestCase(@"DURKADURDUR-EP-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", false)]
        public void IsCutoffImported(string fileName, EnumPokerSites pokerSite, string playerName, bool expected)
        {
            AssertThatStatIsCalculated(x => x.IsCutoff, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FoldedThreeBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedThreeBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedThreeBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Foldedtothreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-DidNotCallThreeBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void ThreeBetCallIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Calledthreebetpreflop, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"Hero-DidNotCallThreeBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        public void FacedThreeBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facedthreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Sawshowdown-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-Sawflop-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"DURKADURDUR-Sawflop-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void SawFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Sawflop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Sawshowdown-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-Sawshowdown-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void SawShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Sawshowdown, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-Wonshowdown-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-Wonshowdown-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WonShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Wonshowdown, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidWalk-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidWalk-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidWalk-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void NumberOfWalksIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.NumberOfWalks, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-DidFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-DidNotFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-FacedFourBet-4.xml", EnumPokerSites.Ignition, "P9_485136KK", 0)]
        public void DidFourBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Didfourbet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-3.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"DURKADURDUR-CouldNotFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"Hero-CouldFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-3.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-4.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-5.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-FacedFourBet-4.xml", EnumPokerSites.Ignition, "P9_485136KK", 0)]
        [TestCase(@"DURKADURDUR-CouldNot4Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldFourBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Couldfourbet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-3.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"DURKADURDUR-CouldNotFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"Hero-CouldFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-3.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-4.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFourBet-5.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-FacedFourBet-4.xml", EnumPokerSites.Ignition, "P9_485136KK", 0)]
        [TestCase(@"DURKADURDUR-CouldNot4Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldfourbetpreflopVirtualIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldfourbetpreflopVirtual, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FacedFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFourBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFourBet-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"Hero-FacedFourBet-4.xml", EnumPokerSites.Ignition, "Hero", 1)]
        public void FacedFourBetPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facedfourbetpreflop, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FoldedFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedFourBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToFourBetPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Foldedtofourbetpreflop, fileName, pokerSite, playerName, expected);
        }

        #region Check-raise based stats


        [TestCase(@"Hero-CouldFlopCheckRaise-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-3.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-4.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-5.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldNotFlopCheckRaise-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFlopCheckRaise-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void CouldFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-DidFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckRaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidNotFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FacedFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFaceFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacedFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FoldedFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FacedTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedTurnCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacedTurnCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedTurnCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FoldedTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToTurnCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToTurnCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FacedRiverCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedRiverCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedRiverCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-FoldedRiverCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToRiverCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToRiverCheckRaise, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-CalledTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CalledTurnCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledTurnCheckRaise, fileName, pokerSite, playerName, expected);
        }

        #endregion

        [TestCase(@"Hero-DidDelayedTurnCBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-DidDelayedTurnCBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-DidDelayedTurnCBet-3.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-DidDelayedTurnCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void DidDelayedTurnCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidDelayedTurnCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldDelayedTurnCBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldNotDelayedTurnCBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotDelayedTurnCBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void CouldDelayedTurnCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldDelayedTurnCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-Equity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 88.41, 0.01)]
        [TestCase(@"DURKADURDUR-Equity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 4.54, 0.01)]
        [TestCase(@"Omaha-HiLo-Equity-1.txt", EnumPokerSites.PokerStars, "deadman426", 23, 3)]
        [TestCase(@"Omaha-HiLo-Equity-1.txt", EnumPokerSites.PokerStars, "tmacnich", 43, 3)]
        [TestCase(@"Omaha-HiLo-Equity-1.txt", EnumPokerSites.PokerStars, "pitervper777", 33, 3)]
        [TestCase(@"Omaha-HiLo-Equity-2.txt", EnumPokerSites.AmericasCardroom, "Mooseslayer", 17, 3)]
        [TestCase(@"Omaha-HiLo-Equity-2.txt", EnumPokerSites.AmericasCardroom, "zc13expert", 24, 3)]
        [TestCase(@"Omaha-HiLo-Equity-2.txt", EnumPokerSites.AmericasCardroom, "SinmanJr", 25, 3)]
        [TestCase(@"Holdem-Equity-1.txt", EnumPokerSites.PartyPoker, "ktm85888", 9.5, 0.03)]
        [TestCase(@"Holdem-Equity-1.txt", EnumPokerSites.PartyPoker, "Griffindorgirl", 28.57, 0.01)]
        [TestCase(@"Holdem-Equity-1.txt", EnumPokerSites.PartyPoker, "pistike88", 61.9, 0.01)]
        [TestCase(@"Holdem-Equity-2.xml", EnumPokerSites.Ignition, "Hero", 17.74, 0.01)]
        [TestCase(@"Holdem-Equity-2.xml", EnumPokerSites.Ignition, "P2_121759IM", 64.52, 0.01)]
        [TestCase(@"Holdem-Equity-2.xml", EnumPokerSites.Ignition, "P8_400154CF", 17.74, 0.01)]

        public void EquityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected, double tolerance)
        {
            AssertThatStatIsCalculated(x => x.Equity * 100, fileName, pokerSite, playerName, expected, tolerance);
        }

        [TestCase(@"Hero-ExpectedValue-1.xml", EnumPokerSites.Ignition, "Hero", -754, 0.01)]
        [TestCase(@"Hero-ExpectedValue-2.xml", EnumPokerSites.Ignition, "Hero", -3421, 0.01)]
        [TestCase(@"Hero-ExpectedValue-3.xml", EnumPokerSites.Ignition, "Hero", 887, 0.01)]
        [TestCase(@"DURKADURDUR-ExpectedValue-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 7728, 0.01)]
        [TestCase(@"DURKADURDUR-ExpectedValue-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", -14786, 0.2)]
        [TestCase(@"Peon84-ExpectedValue-1.txt", EnumPokerSites.PartyPoker, "Peon84", 1316758, 0.01)]

        public void EVDiffIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected, double tolerance)
        {
            var expectedEVDiff = expected / 100m;
            AssertThatStatIsCalculated(x => x.EVDiff, fileName, pokerSite, playerName, expectedEVDiff, tolerance);
        }

        [TestCase(@"Hero-CouldThreeBetVsSteal-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-CouldThreeBetVsSteal-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldThreeBetVsSteal-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotThreeBetVsSteal-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotThreeBetVsSteal-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldThreeBetVsStealIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldThreeBetVsSteal, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"AlexTh-CO-Position.txt", EnumPokerSites.Horizon, "AlexTh", false)]
        public void IsBigBlindIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, bool expected)
        {
            AssertThatStatIsCalculated(x => x.IsBigBlind, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"Holdem6Plus-StealSituation-1.xml", EnumPokerSites.PokerKing, "2129138", 1)]
        public void ButtonstealfacedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealfaced, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void ButtonstealdefendedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealdefended, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"Holdem6Plus-StealSituation-1.xml", EnumPokerSites.PokerKing, "2129138", 1)]
        public void ButtonstealfoldedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealfolded, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void ButtonstealreraisedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealreraised, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidThreeBetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidThreeBetIp, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"Holdem6Plus-General-1.xml", EnumPokerSites.PokerKing, "2097148", 1)]

        public void PreflopIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.PreflopIP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-MP-DidNotFaceCBet-1.txt", EnumPokerSites.PokerStars, "FastEddie267", 0)]
        public void TurncontinuationbetmadeIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Turncontinuationbetmade, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-MP-DidNotFaceCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"Hero-FacingTurnCBet-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacingTurnCBet-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FacingturncontinuationbetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facingturncontinuationbet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CO-DidNotFaceRiverCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacingrivercontinuationbetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facingrivercontinuationbet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CheckedRiverAfterBBLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckedRiverAfterBBLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CheckedRiverAfterBBLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedRiverAfterBBLine, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CouldCheckRiverAfterBBLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotCheckRiverAfterBBLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotCheckRiverAfterBBLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotCheckRiverAfterBBLine-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldCheckRiverAfterBBLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckRiverAfterBBLine, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidBetRiverOnBXLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidBetRiverOnBXLine, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldBetRiverOnBXLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldBetRiverOnBXLine, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacingflopcontinuationbetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacingflopcontinuationbetIP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FacedFlopCBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacingflopcontinuationbetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacingflopcontinuationbetOOP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CalledFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledFlopCBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CalledflopcontinuationbetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledflopcontinuationbetIP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CalledFlopCBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CalledflopcontinuationbetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledflopcontinuationbetOOP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FoldToFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToFlopCBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToFlopcontinuationbetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToFlopcontinuationbetIP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FoldToFlopCBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToFlopcontinuationbetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToFlopcontinuationbetOOP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldToThreeBetOutPosition-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldToThreeBetOutPosition-2.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"DURKADURDUR-FoldToThreeBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToThreeBetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToThreeBetOOP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FoldToThreeBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToThreeBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToThreeBetInPosition-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToThreeBetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToThreeBetIP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FacedRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedRiverRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedRaiseRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CalledRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CalledRiverRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledFacedRaiseRiver, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-CouldNotRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldRiverRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldRaiseRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-StealMade-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-StealMade-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"Holdem6Plus-StealSituation-1.xml", EnumPokerSites.PokerKing, "2118877", 1)]
        public void StealMadeIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.StealMade, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-StealMade-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-StealMade-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotSteal-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotSteal-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"Hero-CouldNotSteal-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void StealPossibleIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.StealPossible, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Peon84-FacingPreflop-Raiser-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.Raiser)]
        [TestCase(@"Peon84-FacingPreflop-Unopened-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.Unopened)]
        [TestCase(@"Peon84-FacingPreflop-Limper-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.Limper)]
        [TestCase(@"Peon84-FacingPreflop-Limper2-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.MultipleLimpers)]
        [TestCase(@"Peon84-FacingPreflop-Raiser-Caller-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.RaiserAndCaller)]
        public void FacingPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, EnumFacingPreflop expected)
        {
            AssertThatStatIsCalculated(x => x.FacingPreflop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-Did5Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Did5Bet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Did5Bet-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Did5Bet-4.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void Did5BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Did5Bet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-Could5Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Could5Bet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNot5Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-Could5Bet-4.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"HeroTest-CouldNot5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void Could5BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Could5Bet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-CalledCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CalledCheckRaiseVsFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledCheckRaiseVsFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-CalledCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FacedCheckRaiseVsFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedCheckRaiseVsFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldedCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FoldedCheckRaiseVsFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedCheckRaiseVsFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidNotFaceTurnBetAfterCheckWhenCheckedFlopAsPfrOOP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckedCalledTurnWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-CheckedCalledTurnWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CheckedCalledTurnWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedCalledTurnWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckedFoldedToTurnWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CheckedFoldedToTurnWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedFoldedToTurnWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FoldedToTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacedTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CalledTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CalledTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FoldedToTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-RaisedTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void RaisedTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.RaisedTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CouldCheckRaiseFlopCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldCheckRaiseFlopCBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldCheckRaiseFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckRaiseFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CheckRaisedFlopCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckRaisedFlopCBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckRaisedFlopCBet-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldCheckRaiseFlopCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldCheckRaiseFlopCBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CheckRaisedFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckRaisedFlopCBet, fileName, pokerSite, playerName, expected);
        }

        #region FlopBetSize tests

        [TestCase(@"DURKADURDUR-FlopBetSizeOneHalfOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeOneHalfOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeOneHalfOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Peon384-FlopBetSizeOneQuarterOrLess-1.txt", EnumPokerSites.BetOnline, "Peon384", 1)]
        public void FlopBetSizeOneQuarterOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeOneQuarterOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FlopBetSizeTwoThirdsOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeTwoThirdsOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeTwoThirdsOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FlopBetSizeThreeQuartersOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeThreeQuartersOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeThreeQuartersOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FlopBetSizeOneOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeOneOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeOneOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FlopBetSizeMoreThanOne-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void FlopBetSizeMoreThanOneIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeMoreThanOne, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FlopBetSizeMoreThanOne-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"DURKADURDUR-FlopBetSizeOneOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FlopBetSizeThreeQuartersOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"Peon384-FlopBetSizeOneQuarterOrLess-1.txt", EnumPokerSites.BetOnline, "Peon384", 1)]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void DidFlopBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidFlopBet, fileName, pokerSite, playerName, expected);
        }

        #endregion

        #region TurnBetSize tests

        [TestCase(@"Hero-TurnBetSizeOneHalfOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneHalfOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneHalfOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeOneQuarterOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneQuarterOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneQuarterOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeOneThirdOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneThirdOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneThirdOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeTwoThirdsOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeTwoThirdsOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeTwoThirdsOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeThreeQuartersOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeThreeQuartersOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeThreeQuartersOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeOneOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneOrLess, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeMoreThanOne-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeMoreThanOneIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeMoreThanOne, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TurnBetSizeOneHalfOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-TurnBetSizeOneQuarterOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-TurnBetSizeOneThirdOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-TurnBetSizeTwoThirdsOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-TurnBetSizeThreeQuartersOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-TurnBetSizeOneOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-TurnBetSizeMoreThanOne-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void DidTurnBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidTurnBet, fileName, pokerSite, playerName, expected);
        }

        #endregion

        #region WTSD after stats


        [TestCase(@"DURKADURDUR-WTSDAfterCalling3Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3Bet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAfterCalling3BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCalling3Bet, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAfterCalling3BetOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCalling3BetOpportunity, fileName, pokerSite, playerName, expected);
        }



        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfr-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAfterCallingPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCallingPfr, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAfterCallingPfrOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCallingPfrOpportunity, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfr-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAfterNotCBettingFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterNotCBettingFlopAsPfr, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAfterNotCBettingFlopAsPfrOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterNotCBettingFlopAsPfrOpportunity, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-WTSDAsPF3Bettor-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3Bettor-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAsPF3BettorIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAsPF3Bettor, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAsPF3BettorOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAsPF3BettorOpportunity, fileName, pokerSite, playerName, expected);
        }

        #endregion

        [TestCase(@"DURKADURDUR-DidDelayedTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidDelayedTurnCBetIn3BetPot-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidDelayedTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidDelayedTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldDelayedTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldDelayedTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldToTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"DURKADURDUR-FacedToTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FoldToTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-FacedToTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"HeroTest-FacedTurnCBetIn3BetPot-1.xml", EnumPokerSites.BetOnline, "Hero", 1)]
        public void FacedToTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedToTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-DidFlopCheckBehind-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckBehind-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckBehind-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFlopCheckBehind-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldFlopCheckBehind-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldFlopCheckBehind-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidFlopCheckBehindIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidFlopCheckBehind, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"DURKADURDUR-CouldFlopCheckBehind-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldFlopCheckBehind-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldFlopCheckBehind-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotFlopCheckBehind-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotFlopCheckBehind-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidFlopCheckBehind-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckBehind-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckBehind-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldFlopCheckBehindIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldFlopCheckBehind, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FacedDonkBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedDonkBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FacedDonkBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedDonkBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldedToDonkBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedToDonkBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedDonkBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-FacedDonkBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void FoldedToDonkBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToDonkBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldedTurn-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTurn-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTurn-3.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTurn-4.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-FacedBetOnTurn-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-FacedBetOnTurn-3.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-FacedBetOnTurn-4.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FoldedTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedTurn, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FacedBetOnTurn-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-3.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-4.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FacedBetOnTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedBetOnTurn, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-CheckedCalledRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedFacedBetOnRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CheckedCalledRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedCalledRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-CheckedFoldedRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedFacedBetOnRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CheckedFoldedRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedFoldedRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-CheckedFacedBetOnRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedCalledRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedFoldedRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void CheckedFacedBetOnRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedThenFacedBetOnRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 72.15)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 178)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 2)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-4.txt", EnumPokerSites.PokerStars, "HeroTest", 44)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-Zero-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void RiverCallSizeOnFacingBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected)
        {
            AssertThatStatIsCalculated(x => x.RiverCallSizeOnFacingBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 269)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 59)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-4.txt", EnumPokerSites.PokerStars, "HeroTest", 157)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-Zero-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void RiverWonOnFacingBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected)
        {
            AssertThatStatIsCalculated(x => x.RiverWonOnFacingBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldedTo5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTo5Bet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTo5Bet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-Faced5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-Faced5Bet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-Faced5Bet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-Faced5Bet-4.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void FoldedTo5BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedTo5Bet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-FoldedTo5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTo5Bet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedTo5Bet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-Faced5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-Faced5Bet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-Faced5Bet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-Faced5Bet-4.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-DidNotFace5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-DidNotFace5Bet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void Faced5BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Faced5Bet, fileName, pokerSite, playerName, expected);
        }


        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-2.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-DidNotShoveFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 0)]
        public void ShoveFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.ShovedFlopAfter4Bet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-2.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-DidNotShoveFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldNotShoveFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldNotShoveFlopAfter4Bet-2.xml", EnumPokerSites.BetOnline, "HeroTest", 0)]
        public void CouldShoveFlopAfter4BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldShoveFlopAfter4Bet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-BetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-BetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldBetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldBetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldNotBetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldNotBetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void BetFlopWhenCheckedToSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.BetFlopWhenCheckedToSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-CouldBetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldBetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldNotBetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldNotBetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CouldBetFlopWhenCheckedToSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldBetFlopWhenCheckedToSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"HeroTest-UO-PFR-BN-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-Not-UO-PFR-BN-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-Not-UO-PFR-BN-3.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void UOPFRBNIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.UO_PFR_BN, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-NetWon-1.txt", EnumPokerSites.Poker888, "Hero", 300000)]
        public void NetWonIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Totalamountwonincents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldRiverBet-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldRiverBet-2.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldNotRiverBet-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-CouldNotRiverBet-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-CouldNotRiverBet-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CouldRiverBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldRiverBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldTurnBet-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldTurnBet-2.txt", EnumPokerSites.Winamax, "Hero", 1)]
        public void CouldTurnBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldTurnBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedFlop-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacedBetOnFlop-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FoldedFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedFlop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedFlop-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacedBetOnFlop-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void FacedBetOnFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedBetOnFlop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-TotalWonAmountOnRiverCall-1.xml", EnumPokerSites.Ignition, "Hero", 922)]
        public void TotalWonAmountOnRiverCallIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TotalWonAmountOnRiverCall, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldProbeBetTurn-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldProbeBetTurn-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-CouldProbeBetTurn-False-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CouldProbeBetTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldProbeBetTurn, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldProbeBetRiver-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldProbeBetRiver-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CouldProbeBetRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldProbeBetRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-DoubleBarrelSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-DoubleBarrelSRP-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void DoubleBarrelSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DoubleBarrelSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FacedProbeBetTurn-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacedProbeBetTurn-True-2.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacedProbeBetTurn-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FacedProbeBetTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedProbeBetTurn, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedToProbeBetTurn-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FoldedToProbeBetTurn-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FoldedToProbeBetTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToProbeBetTurn, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FacedProbeBetRiver-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void FacedProbeBetRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedProbeBetRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedToProbeBetRiver-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FoldedToProbeBetRiver-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FoldedToProbeBetRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToProbeBetRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToTurnBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToTurnBetIPSRP-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CheckFlopAsPFRAndFoldToTurnBetIPSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckFlopAsPFRAndFoldToTurnBetIPSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToTurnBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToTurnBetIPSRP-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void CouldCheckFlopAsPFRAndFoldToTurnBetIPSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToTurnBetOOPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP-True-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CheckFlopAsPFRAndFoldToTurnBetOOPSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckFlopAsPFRAndFoldToTurnBetOOPSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP-True-2.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToRiverBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void CheckFlopAsPFRAndFoldToRiverBetIPSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckFlopAsPFRAndFoldToRiverBetIPSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CheckFlopAsPFRAndFoldToRiverBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-CouldCheckFlopAsPFRAndFoldToRiverBetIPSRP-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void CouldCheckFlopAsPFRAndFoldToRiverBetIPSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckFlopAsPFRAndFoldToRiverBetIPSRP, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldNot-ColdCall4Bet.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void ColdCall4BetInBBIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidColdCall4BetInBB, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldNot-ColdCall3Bet.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-DidNotColdCall3Bet-1.xml", EnumPokerSites.Ignition, "Hero", 0)]
        [TestCase(@"Hero-DidNotColdCall3Bet-2.xml", EnumPokerSites.Ignition, "Hero", 0)]
        [TestCase(@"Hero-DidNotColdCall3Bet-1.xml", EnumPokerSites.Ignition, "P3_649467OW", 1)]
        [TestCase(@"Hero-DidNotColdCall3Bet-2.xml", EnumPokerSites.Ignition, "P3_149165VF", 1)]
        public void ColdCall3BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidColdCallThreeBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-CouldColdCall3Bet-1.xml", EnumPokerSites.Ignition, "Hero", 1)]
        [TestCase(@"Hero-CouldColdCall3Bet-2.xml", EnumPokerSites.Ignition, "Hero", 1)]
        public void CouldColdCall3BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldColdCallThreeBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FacedDelayedCBet-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacedDelayedCBet-True-2.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FacedDelayedCBet-False-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-FoldedToDelayedCBet-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FoldedToDelayedCBet-True-2.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void FacedDelayedCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedDelayedCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedToDelayedCBet-True-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        [TestCase(@"Hero-FoldedToDelayedCBet-True-2.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void FoldedToDelayedCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToDelayedCBet, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FirstRaiserPosition-1.txt", EnumPokerSites.PokerStars, "Hero", EnumPosition.Undefined)]
        public void FirstRaiserPositionIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, EnumPosition expected)
        {
            AssertThatStatIsCalculated(x => x.FirstRaiserPosition, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-ThreeBettorPosition-1.txt", EnumPokerSites.PokerStars, "Hero", EnumPosition.MP1)]
        public void ThreeBettorPositionIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, EnumPosition expected)
        {
            AssertThatStatIsCalculated(x => x.ThreeBettorPosition, fileName, pokerSite, playerName, expected);
        }

        #region Pot based stat tests

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 2000)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 600)]
        public void BetAmountPreflopInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.BetAmountPreflopInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 500)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CallAmountPreflopInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CallAmountPreflopInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 100)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void PostAmountPreflopInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.PostAmountPreflopInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 222.22)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 200)]
        public void RaiseSizeToPotPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.RaiseSizeToPotPreflop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 200)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 200)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FacingRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.FacingRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        public void NumberOfPlayersOnFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.NumberOfPlayersOnFlop, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 2700)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 1100)]
        public void BetAmountFlopInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.BetAmountFlopInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 800)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 15600)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CallAmountFlopInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CallAmountFlopInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 1400)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 4300)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 1500)]
        public void FlopPotSizeInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopPotSizeInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 19.29)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 4.96)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 25.78)]
        public void FlopStackPotRatioIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.FlopStackPotRatio, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-4.txt", EnumPokerSites.PokerStars, "Hero", 113.79)]
        public void FlopRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.FlopRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 57.1428)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FlopFacingBetSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.FlopFacingBetSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 261.428)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void FlopFacingRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.FlopFacingRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-4.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void NumberOfPlayersOnTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.NumberOfPlayersOnTurn, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 2800)]
        public void BetAmountTurnInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.BetAmountTurnInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 2500)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void CallAmountTurnInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CallAmountTurnInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 3000)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 40900)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 3700)]
        [TestCase(@"Hero-Pot-Calcs-4.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void TurnPotSizeInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnPotSizeInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 8.73)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0.074)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 10.15)]
        public void TurnStackPotRatioIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.TurnStackPotRatio, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void TurnRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 83.33)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "bluffinmyway", 75.67)]
        public void TurnFacingBetSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.TurnFacingBetSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void TurnFacingRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnFacingRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-4.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void NumberOfPlayersOnRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.NumberOfPlayersOnRiver, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "bluffinmyway", 5900)]
        public void BetAmountRiverInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.BetAmountRiverInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 5900)]
        public void CallAmountRiverInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CallAmountRiverInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 8000)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 40900)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 9300)]
        [TestCase(@"Hero-Pot-Calcs-4.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void RiverPotSizeInCentsIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.RiverPotSizeInCents, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 2.96)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0.074)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 3.73)]
        public void RiverStackPotRatioIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.RiverStackPotRatio, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void RiverRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.RiverRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 63.44)]
        public void RiverFacingBetSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, double expected)
        {
            AssertThatStatIsCalculated(x => x.RiverFacingBetSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 0)]
        public void RiverFacingRaiseSizeToPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.RiverFacingRaiseSizeToPot, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-Pot-Calcs-1.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-2.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        [TestCase(@"Hero-Pot-Calcs-3.txt", EnumPokerSites.PokerStars, "Hero", 2)]
        public void NumberOfPlayersSawShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.NumberOfPlayersSawShowdown, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedTo3BetInBTNvs3BetSB-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void FoldTo3BetInBTNvs3BetSBIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldTo3BetInBTNvs3BetSB, fileName, pokerSite, playerName, expected);
        }

        [TestCase(@"Hero-FoldedTo3BetInBTNvs3BetSB-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]
        public void CouldFoldTo3BetInBTNvs3BetSBIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldFoldTo3BetInBTNvs3BetSB, fileName, pokerSite, playerName, expected);
        }

        #endregion
    }
}