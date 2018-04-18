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

using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// PlayerStatistic integration tests
    /// </summary>
    [TestFixture]
    class PlayerStatisticTests : BaseDatabaseTest
    {
        #region SetUp

        /// <summary>
        /// Initialize environment for test
        /// </summary>
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Initalize();
        }

        /// <summary>
        /// Freeing resources
        /// </summary>
        [OneTimeTearDown]
        public virtual void TearDown()
        {
            RemoveDatabase();
        }

        [TearDown]
        public virtual void TestTearDown()
        {
            CleanUpDatabase();
        }

        #endregion

        protected override string TestDataFolder
        {
            get
            {
                return @"..\..\IntegrationTests\Importers\TestData\PokerStars\PlayerStatistic";
            }
        }

        [Test]
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

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-SB-PfrOop-2-max.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void PfrOopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.PfrOop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-BTN-Cards.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "AcJh")]
        public void CardsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string expected)
        {
            AssertThatStatIsCalculated(x => x.Cards, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
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
        public void PositionsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string expectedPosition)
        {
            AssertThatStatIsCalculated(x => x.PositionString, fileName, pokerSite, playerName, expectedPosition);
        }

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-FoldedThreeBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedThreeBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedThreeBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Foldedtothreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-DidNotCallThreeBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void ThreeBetCallIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Calledthreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-DidNotCallThreeBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        public void FacedThreeBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facedthreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-Sawshowdown-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-Sawflop-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"DURKADURDUR-Sawflop-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void SawFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Sawflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-Sawshowdown-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-Sawshowdown-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void SawShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Sawshowdown, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-Wonshowdown-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-Wonshowdown-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WonShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Wonshowdown, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-DidWalk-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidWalk-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidWalk-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void NumberOfWalksIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.NumberOfWalks, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-DidFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-DidNotFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-FacedFourBet-4.xml", EnumPokerSites.Ignition, "P9_485136KK", 0)]
        public void DidFourBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Didfourbet, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-FacedFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFourBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFourBet-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"Hero-FacedFourBet-4.xml", EnumPokerSites.Ignition, "Hero", 1)]
        public void FacedFourBetPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facedfourbetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedFourBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToFourBetPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Foldedtofourbetpreflop, fileName, pokerSite, playerName, expected);
        }

        #region Check-raise based stats

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-DidFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidFlopCheckRaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidNotFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFaceFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacedFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedFlopCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedFlopCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToFlopCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedTurnCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnCheckRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacedTurnCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedTurnCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToTurnCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToTurnCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedRiverCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedRiverCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedRiverCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedRiverCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToRiverCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToRiverCheckRaise, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CalledTurnCheckRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CalledTurnCheckRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledTurnCheckRaise, fileName, pokerSite, playerName, expected);
        }

        #endregion

        [Test]
        [TestCase(@"Hero-DidDelayedTurnCBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidDelayedTurnCBet-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidDelayedTurnCBet-3.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-DidDelayedTurnCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void DidDelayedTurnCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidDelayedTurnCBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-CouldDelayedTurnCBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldNotDelayedTurnCBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotDelayedTurnCBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void CouldDelayedTurnCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldDelayedTurnCBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
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

        [Test]
        [TestCase(@"Hero-CouldThreeBetVsSteal-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-CouldThreeBetVsSteal-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldThreeBetVsSteal-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotThreeBetVsSteal-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotThreeBetVsSteal-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldThreeBetVsStealIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldThreeBetVsSteal, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void ButtonstealfacedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealfaced, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void ButtonstealdefendedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealdefended, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void ButtonstealfoldedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealfolded, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-BTN-DefendCORaise-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void ButtonstealreraisedIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Buttonstealreraised, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-DidThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidThreeBetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidThreeBetIp, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-DidThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotThreeBetIP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void PreflopIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.PreflopIP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-MP-DidNotFaceCBet-1.txt", EnumPokerSites.PokerStars, "FastEddie267", 0)]
        public void TurncontinuationbetmadeIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Turncontinuationbetmade, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-MP-DidNotFaceCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacingturncontinuationbetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facingturncontinuationbet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CO-DidNotFaceRiverCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacingrivercontinuationbetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Facingrivercontinuationbet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CheckedRiverAfterBBLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckedRiverAfterBBLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CheckedRiverAfterBBLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedRiverAfterBBLine, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CouldCheckRiverAfterBBLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotCheckRiverAfterBBLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotCheckRiverAfterBBLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotCheckRiverAfterBBLine-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CouldCheckRiverAfterBBLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckRiverAfterBBLine, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidBetRiverOnBXLine-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldBetRiverOnBXLine-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldBetRiverOnBXLine-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]        
        public void DidBetRiverOnBXLineIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidBetRiverOnBXLine, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacingflopcontinuationbetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacingflopcontinuationbetIP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedFlopCBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacingflopcontinuationbetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacingflopcontinuationbetOOP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CalledFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledFlopCBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CalledflopcontinuationbetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledflopcontinuationbetIP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CalledFlopCBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void CalledflopcontinuationbetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledflopcontinuationbetOOP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldToFlopCBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToFlopCBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToFlopcontinuationbetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToFlopcontinuationbetIP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldToFlopCBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToFlopCBetOutPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToFlopcontinuationbetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToFlopcontinuationbetOOP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-FoldToThreeBetOutPosition-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldToThreeBetOutPosition-2.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"DURKADURDUR-FoldToThreeBetOutPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToThreeBetOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToThreeBetOOP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldToThreeBetInPosition-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToThreeBetInPosition-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldToThreeBetInPosition-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldToThreeBetIPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToThreeBetIP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedRiverRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedRaiseRiver, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CalledRiverRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledFacedRaiseRiver, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CouldNotRiverRaise-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotRiverRaise-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldRiverRaiseIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldRaiseRiver, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-StealMade-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-StealMade-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void StealMadeIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.StealMade, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-StealMade-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-StealMade-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNotSteal-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldNotSteal-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void StealPossibleIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.StealPossible, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Peon84-FacingPreflop-Raiser-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.Raiser)]
        [TestCase(@"Peon84-FacingPreflop-Unopened-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.Unopened)]
        [TestCase(@"Peon84-FacingPreflop-Limper-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.Limper)]
        [TestCase(@"Peon84-FacingPreflop-Limper2-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.MultipleLimpers)]
        [TestCase(@"Peon84-FacingPreflop-Raiser-Caller-1.txt", EnumPokerSites.PartyPoker, "Peon84", EnumFacingPreflop.RaiserAndCaller)]
        public void FacingPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, EnumFacingPreflop expected)
        {
            AssertThatStatIsCalculated(x => x.FacingPreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-Did5Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Did5Bet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Did5Bet-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Did5Bet-4.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void Did5BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Did5Bet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-Could5Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-Could5Bet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldNot5Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-Could5Bet-4.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"HeroTest-CouldNot5Bet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void Could5BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.Could5Bet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-CalledCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CalledCheckRaiseVsFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledCheckRaiseVsFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-CalledCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FacedCheckRaiseVsFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedCheckRaiseVsFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-FoldedCheckRaiseVsFlopCBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FoldedCheckRaiseVsFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedCheckRaiseVsFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnBetAfterCheckWhenCheckedFlopAsPfrOOP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckedCalledTurnWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CheckedCalledTurnWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CheckedCalledTurnWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedCalledTurnWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CheckedFoldedToTurnWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CheckedFoldedToTurnWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedFoldedToTurnWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedToTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CalledTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidNotFaceTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FacedTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CalledTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CalledTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CalledTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedToTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-RaisedTurnBetWhenCheckedFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void RaisedTurnBetWhenCheckedFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.RaisedTurnBetWhenCheckedFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CouldCheckRaiseFlopCBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldCheckRaiseFlopCBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldCheckRaiseFlopCBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldCheckRaiseFlopCBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-FlopBetSizeOneHalfOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeOneHalfOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeOneHalfOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Peon384-FlopBetSizeOneQuarterOrLess-1.txt", EnumPokerSites.BetOnline, "Peon384", 1)]
        public void FlopBetSizeOneQuarterOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeOneQuarterOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FlopBetSizeTwoThirdsOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeTwoThirdsOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeTwoThirdsOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FlopBetSizeThreeQuartersOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeThreeQuartersOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeThreeQuartersOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FlopBetSizeOneOrLess-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FlopBetSizeOneOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeOneOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-FlopBetSizeMoreThanOne-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void FlopBetSizeMoreThanOneIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FlopBetSizeMoreThanOne, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
        [TestCase(@"Hero-TurnBetSizeOneHalfOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneHalfOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneHalfOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-TurnBetSizeOneQuarterOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneQuarterOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneQuarterOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-TurnBetSizeOneThirdOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneThirdOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneThirdOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-TurnBetSizeTwoThirdsOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeTwoThirdsOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeTwoThirdsOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-TurnBetSizeThreeQuartersOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeThreeQuartersOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeThreeQuartersOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-TurnBetSizeOneOrLess-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeOneOrLessIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeOneOrLess, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-TurnBetSizeMoreThanOne-1.txt", EnumPokerSites.Ignition, "Hero", 1)]
        public void TurnBetSizeMoreThanOneIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.TurnBetSizeMoreThanOne, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3Bet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3Bet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAfterCalling3BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCalling3Bet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCalling3BetOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAfterCalling3BetOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCalling3BetOpportunity, fileName, pokerSite, playerName, expected);
        }


        [Test]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfr-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAfterCallingPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCallingPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterCallingPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAfterCallingPfrOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterCallingPfrOpportunity, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfr-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfr-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAfterNotCBettingFlopAsPfrIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterNotCBettingFlopAsPfr, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAfterNotCBettingFlopAsPfrOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAfterNotCBettingFlopAsPfrOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAfterNotCBettingFlopAsPfrOpportunity, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAsPF3Bettor-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3Bettor-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void WTSDAsPF3BettorIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAsPF3Bettor, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-WTSDAsPF3BettorOpportunity-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WTSDAsPF3BettorOpportunityIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.WTSDAsPF3BettorOpportunity, fileName, pokerSite, playerName, expected);
        }

        #endregion

        [Test]
        [TestCase(@"DURKADURDUR-DidDelayedTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidDelayedTurnCBetIn3BetPot-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void DidDelayedTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.DidDelayedTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-CouldDelayedTurnCBetIn3BetPot-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void CouldDelayedTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldDelayedTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-FoldToTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"DURKADURDUR-FacedToTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void FoldToTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldToTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedToTurnCBetIn3BetPot-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedToTurnCBetIn3BetPotIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedToTurnCBetIn3BetPot, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
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

        [Test]
        [TestCase(@"HeroTest-FacedDonkBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedDonkBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FacedDonkBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedDonkBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-FoldedToDonkBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FoldedToDonkBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedDonkBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-FacedDonkBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void FoldedToDonkBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FoldedToDonkBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
        [TestCase(@"HeroTest-FacedBetOnTurn-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-3.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-FacedBetOnTurn-4.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void FacedBetOnTurnIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.FacedBetOnTurn, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-CheckedCalledRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedFacedBetOnRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CheckedCalledRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedCalledRiver, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-CheckedFoldedRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedFacedBetOnRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CheckedFoldedRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedFoldedRiver, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-CheckedFacedBetOnRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedCalledRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CheckedFoldedRiver-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        public void CheckedFacedBetOnRiverIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CheckedThenFacedBetOnRiver, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 72.15)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 178)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 2)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-4.txt", EnumPokerSites.PokerStars, "HeroTest", 44)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-Zero-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void RiverCallSizeOnFacingBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected)
        {
            AssertThatStatIsCalculated(x => x.RiverCallSizeOnFacingBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-2.txt", EnumPokerSites.PokerStars, "HeroTest", 269)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-3.txt", EnumPokerSites.PokerStars, "HeroTest", 59)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-4.txt", EnumPokerSites.PokerStars, "HeroTest", 157)]
        [TestCase(@"HeroTest-RiverCallSizeOnFacingBet-Zero-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void RiverWonOnFacingBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, decimal expected)
        {
            AssertThatStatIsCalculated(x => x.RiverWonOnFacingBet, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
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

        [Test]
        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-2.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-DidNotShoveFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 0)]
        public void ShoveFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.ShovedFlopAfter4Bet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-ShovedFlopAfter4Bet-2.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-DidNotShoveFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldNotShoveFlopAfter4Bet-1.xml", EnumPokerSites.BetOnline, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldNotShoveFlopAfter4Bet-2.xml", EnumPokerSites.BetOnline, "HeroTest", 0)]
        public void CouldShoveFlopAfter4BetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldShoveFlopAfter4Bet, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [Test]
        [TestCase(@"HeroTest-CouldBetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldBetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 1)]
        [TestCase(@"HeroTest-CouldNotBetFlopWhenCheckedToSRP-1.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        [TestCase(@"HeroTest-CouldNotBetFlopWhenCheckedToSRP-2.txt", EnumPokerSites.PokerStars, "HeroTest", 0)]
        public void CouldBetFlopWhenCheckedToSRPIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated(x => x.CouldBetFlopWhenCheckedToSRP, fileName, pokerSite, playerName, expected);
        }

        [Test]
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

        [TestCase(@"Hero-CouldRiverBet-1.txt", EnumPokerSites.PokerStars, "Hero", 1)]       
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

        protected virtual void AssertThatStatIsCalculated<T>(Expression<Func<Playerstatistic, T>> expression, string fileName, EnumPokerSites pokerSite, string playerName, T expected, double tolerance = 0.01, [CallerMemberName] string method = "UnknownMethod")
        {
            using (var perfScope = new PerformanceMonitor(method))
            {
                Playerstatistic playerstatistic = null;

                var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
                playerStatisticRepository.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                var getStat = expression.Compile();

                Assert.That(getStat(playerstatistic), Is.EqualTo(expected).Within(tolerance));
            }
        }

        /// <summary>
        /// Gets player statistic from the <see cref="IDataService.Store(Playerstatistic)"/> call for the specified player
        /// </summary>     
        protected virtual bool GetSinglePlayerstatisticFromStoreCall(ref Playerstatistic playerstatistic, Playerstatistic p, string playerName)
        {
            if (p.PlayerName.Equals(playerName))
            {
                playerstatistic = p;
            }

            return true;
        }

        /// <summary>
        /// Gets player statistics from the <see cref="IDataService.Store(Playerstatistic)"/> call for the specified player
        /// </summary>     
        protected virtual bool GetPlayerstatisticCollectionFromStoreCall(ref List<Playerstatistic> playerstatistic, Playerstatistic p, string playerName)
        {
            if (p.PlayerName.Equals(playerName))
            {
                playerstatistic.Add(p);
            }

            return true;
        }
    }
}