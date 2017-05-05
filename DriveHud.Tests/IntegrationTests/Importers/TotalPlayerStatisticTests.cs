//-----------------------------------------------------------------------
// <copyright file="TotalPlayerStatisticTests.cs" company="Ace Poker Solutions">
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
using System.IO;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// Total playerStatistic integration tests
    /// </summary>
    [TestFixture]
    class TotalPlayerStatisticTests : BaseDatabaseTest
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
                return @"..\..\IntegrationTests\Importers\TestData\PokerStars\TotalPlayerstatisticTestData";
            }
        }

        /// <summary>
        /// This test is supposed to test all <see cref="Playerstatistic"/> properties after long hh is imported        
        /// </summary>            
        [Test]
        [TestCase(EnumPokerSites.PokerStars, "DURKADURDUR")]
        public void PlayerStatisticIsCalculated(EnumPokerSites pokerSite, string playerName)
        {
            using (var perfScope = new PerformanceMonitor("PlayerStatisticIsCalculated"))
            {
                Playerstatistic playerstatistic = null;

                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                dataService.Store(Arg.Is<Playerstatistic>(x => GetCombinedPlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                var testDataDirectory = new DirectoryInfo(TestDataFolder);

                Assert.True(testDataDirectory.Exists, $"Directory '{TestDataFolder}' has not been found");

                foreach (var file in testDataDirectory.GetFiles())
                {
                    FillDatabaseFromSingleFile(file.Name, pokerSite);
                }

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                // add asserts to validate properties here 
                // current values are for demo purpose, need to import same hh to HM2, then use stats from HM2 as expected values
                Assert.That(playerstatistic.DidColdCallIp, Is.EqualTo(3), nameof(playerstatistic.DidColdCallIp));
                Assert.That(playerstatistic.Vpiphands, Is.EqualTo(35), nameof(playerstatistic.Vpiphands));
            }
        }

        /// <summary>
        /// Gets combined player statistic from the <see cref="IDataService.Store(Playerstatistic)"/> call for the specified player
        /// </summary>     
        protected virtual bool GetCombinedPlayerstatisticFromStoreCall(ref Playerstatistic playerstatistic, Playerstatistic p, string playerName)
        {
            if (p.PlayerName.Equals(playerName))
            {
                if (playerstatistic == null)
                {
                    playerstatistic = p;
                }
                else
                {
                    playerstatistic += p;
                }
            }

            return true;
        }
    }
}