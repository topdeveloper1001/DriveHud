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

using DriveHud.Common.Log;
using DriveHud.Tests.IntegrationTests.Base;
using DriveHud.Tests.UnitTests.Helpers;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
            using (var perfScope = new PerformanceMonitor("DidColdCallIpIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.DidColdCallIp, Is.EqualTo(expected));
            }
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
            using (var perfScope = new PerformanceMonitor("DidColdCallIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.Didcoldcall, Is.EqualTo(expected));
            }
        }

        [Test]
        [TestCase(@"DURKADURDUR-SB-PfrOop-2-max.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void PfrOopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            using (var perfScope = new PerformanceMonitor("PfrOopIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.PfrOop, Is.EqualTo(expected));
            }
        }

        [Test]
        [TestCase(@"DURKADURDUR-BTN-Cards.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "AcJh")]
        public void CardsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string expected)
        {
            using (var perfScope = new PerformanceMonitor("CardsAreImported"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.Cards, Is.EqualTo(expected));
            }
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

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetPlayerstatisticCollectionFromStoreCall(ref playerstatistic, x, playerName)));

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
        [TestCase(@"Hero-Position-EP-1.xml", EnumPokerSites.IPoker, "Peon84", "EP")]
        [TestCase(@"Hero-Position-EP-2.xml", EnumPokerSites.IPoker, "Peon84", "EP")]
        [TestCase(@"Hero-Position-EP-3.xml", EnumPokerSites.IPoker, "Peon_184", "EP")]
        [TestCase(@"Hero-Position-EP-4.txt", EnumPokerSites.PokerStars, "Peon347", "EP")]
        [TestCase(@"Hero-Position-CO-1.xml", EnumPokerSites.IPoker, "Peon_184", "CO")]
        [TestCase(@"Hero-Position-MP-1.xml", EnumPokerSites.IPoker, "Peon84", "MP")]
        [TestCase(@"Hero-Position-MP-2.xml", EnumPokerSites.IPoker, "Peon84", "MP")]
        [TestCase(@"Hero-Position-MP-3.xml", EnumPokerSites.IPoker, "Peon84", "MP")]
        [TestCase(@"DURKADURDUR-CO-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "CO")]
        [TestCase(@"DURKADURDUR-EP-Position.txt", EnumPokerSites.PokerStars, "DURKADURDUR", "EP")]
        public void PositionsAreImported(string fileName, EnumPokerSites pokerSite, string playerName, string expectedPosition)
        {
            using (var perfScope = new PerformanceMonitor("PositionsAreImported"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                Assert.That(playerstatistic.PositionString, Is.EqualTo(expectedPosition));
            }
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
            using (var perfScope = new PerformanceMonitor("PositionsAreImported"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                Assert.That(playerstatistic.IsCutoff, Is.EqualTo(expected));
            }
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedThreeBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedThreeBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedThreeBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            using (var perfScope = new PerformanceMonitor("DidColdCallIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.FoldedtothreebetpreflopVirtual, Is.EqualTo(expected));
            }
        }

        [Test]
        [TestCase(@"Hero-DidNotCallThreeBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void ThreeBetCallIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("ThreeBetCallIsCalculated", x => x.Calledthreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-DidNotCallThreeBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        public void FacedThreeBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("FacedThreeBetIsCalculated", x => x.Facedthreebetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-Sawshowdown-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-Sawflop-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"DURKADURDUR-Sawflop-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void SawFlopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("SawFlopIsCalculated", x => x.Sawflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-Sawshowdown-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"DURKADURDUR-Sawshowdown-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void SawShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("SawShowdownIsCalculated", x => x.Sawshowdown, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-Wonshowdown-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-Wonshowdown-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void WonShowdownIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("WonShowdownIsCalculated", x => x.Wonshowdown, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-DidWalk-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-DidWalk-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        [TestCase(@"DURKADURDUR-DidWalk-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 0)]
        public void NumberOfWalksIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("NumberOfWalksIsCalculated", x => x.NumberOfWalks, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-DidFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-DidNotFourBet-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-DidNotFourBet-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void DidFourBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("DidFourBetIsCalculated", x => x.Didfourbet, fileName, pokerSite, playerName, expected);
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
        public void CouldFourBetIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("CouldFourBetIsCalculated", x => x.Couldfourbet, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FacedFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFourBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FacedFourBet-3.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FacedFourBetPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("FacedFourBetPreflopIsCalculated", x => x.Facedfourbetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"DURKADURDUR-FoldedFourBet-1.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        [TestCase(@"DURKADURDUR-FoldedFourBet-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR", 1)]
        public void FoldedToFourBetPreflopIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("FoldedToFourBetPreflopIsCalculated", x => x.Foldedtofourbetpreflop, fileName, pokerSite, playerName, expected);
        }

        [Test]
        [TestCase(@"Hero-CouldFlopCheckRaise-1.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-2.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-3.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-4.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldFlopCheckRaise-5.xml", EnumPokerSites.IPoker, "Hero", 1)]
        [TestCase(@"Hero-CouldNotFlopCheckRaise-1.xml", EnumPokerSites.IPoker, "Hero", 0)]
        [TestCase(@"Hero-CouldNotFlopCheckRaise-2.xml", EnumPokerSites.IPoker, "Hero", 0)]
        public void CouldFlopCheckRaise(string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            AssertThatStatIsCalculated("CouldFlopCheckRaise", x => x.CouldFlopCheckRaise, fileName, pokerSite, playerName, expected);
        }

        protected virtual void AssertThatStatIsCalculated(string method, Expression<Func<Playerstatistic, int>> expression, string fileName, EnumPokerSites pokerSite, string playerName, int expected)
        {
            using (var perfScope = new PerformanceMonitor(method))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                var getStat = expression.Compile();

                Assert.That(getStat(playerstatistic), Is.EqualTo(expected));
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