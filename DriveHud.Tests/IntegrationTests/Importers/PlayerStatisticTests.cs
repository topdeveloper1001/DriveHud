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
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
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