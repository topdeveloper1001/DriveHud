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
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using NSubstitute;
using NUnit.Framework;

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
        [TestCase(@"DURKADURDUR-CO-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR")]
        [TestCase(@"DURKADURDUR-CO-DidColdCallIp-2.txt", EnumPokerSites.PokerStars, "DURKADURDUR")]
        [TestCase(@"DURKADURDUR-BB-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR")]
        [TestCase(@"DURKADURDUR-MP-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR")]
        [TestCase(@"DURKADURDUR-BTN-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR")]
        public void DidColdCallIpIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName)
        {
            using (var perfScope = new PerformanceMonitor("DidColdCallIpIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.DidColdCallIp, Is.EqualTo(1));
            }
        }

        [Test]
        [TestCase(@"DURKADURDUR-CO-DidColdCallIp.txt", EnumPokerSites.PokerStars, "DURKADURDUR")]      
        public void DidColdCallIsCalculated(string fileName, EnumPokerSites pokerSite, string playerName)
        {
            using (var perfScope = new PerformanceMonitor("DidColdCallIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetSinglePlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                FillDatabaseFromSingleFile(fileName, pokerSite);

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");
                Assert.That(playerstatistic.Didcoldcall, Is.EqualTo(1));
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
    }
}