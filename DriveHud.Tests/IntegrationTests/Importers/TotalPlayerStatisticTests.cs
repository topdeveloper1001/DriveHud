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

using System;
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
        [TestCase(EnumPokerSites.PokerStars, "PLR_2527293KW")]
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
                Assert.That(Math.Round((decimal)100*playerstatistic.LimpCalled/playerstatistic.LimpFaced,1), Is.EqualTo(100));
                Assert.That(Math.Round((decimal)100*playerstatistic.LimpSb/playerstatistic.LimpPossible,1), Is.EqualTo(9.1));
                Assert.That(Math.Round((decimal)100*playerstatistic.LimpEp/playerstatistic.LimpPossible,1), Is.EqualTo(36.4));
                Assert.That(Math.Round((decimal)100*playerstatistic.LimpMp/playerstatistic.LimpPossible,1), Is.EqualTo(9.1));
                Assert.That(Math.Round((decimal)100*playerstatistic.LimpCo/playerstatistic.LimpPossible,1), Is.EqualTo(0));
                Assert.That(Math.Round((decimal)100*playerstatistic.LimpBtn/playerstatistic.LimpPossible,1), Is.EqualTo(0));

                Assert.That(Math.Round((decimal)100 * playerstatistic.DidColdCallInSb / playerstatistic.Couldcoldcall, 1), Is.EqualTo(0));
                Assert.That(Math.Round((decimal)100 * playerstatistic.DidColdCallInBb / playerstatistic.Couldcoldcall, 1), Is.EqualTo(0));
                Assert.That(Math.Round((decimal)100 * playerstatistic.DidColdCallInEp / playerstatistic.Couldcoldcall, 1), Is.EqualTo(23.1));
                Assert.That(Math.Round((decimal)100 * playerstatistic.DidColdCallInMp / playerstatistic.Couldcoldcall, 1), Is.EqualTo(7.7));
                Assert.That(Math.Round((decimal)100 * playerstatistic.DidColdCallInCo / playerstatistic.Couldcoldcall, 1), Is.EqualTo(0));
                Assert.That(Math.Round((decimal)100 * playerstatistic.DidColdCallInBtn / playerstatistic.Couldcoldcall, 1), Is.EqualTo(7.7));
                
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