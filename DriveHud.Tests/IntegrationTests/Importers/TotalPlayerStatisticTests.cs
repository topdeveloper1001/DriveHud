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
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using NSubstitute;
using NUnit.Framework;
using System;
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

                var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
                playerStatisticRepository.Store(Arg.Is<Playerstatistic>(x => GetCombinedPlayerstatisticFromStoreCall(ref playerstatistic, x, playerName)));

                var testDataDirectory = new DirectoryInfo(TestDataFolder);

                Assert.True(testDataDirectory.Exists, $"Directory '{TestDataFolder}' has not been found");

                foreach (var file in testDataDirectory.GetFiles())
                {
                    FillDatabaseFromSingleFile(file.Name, pokerSite);
                }

                Assert.IsNotNull(playerstatistic, $"Player '{playerName}' has not been found");

                Assert.That(playerstatistic.LimpCalled, Is.EqualTo(3), nameof(playerstatistic.LimpCalled));

                Assert.That(playerstatistic.LimpSb, Is.EqualTo(1), nameof(playerstatistic.LimpSb));
                Assert.That(playerstatistic.LimpEp, Is.EqualTo(3), nameof(playerstatistic.LimpEp));
                Assert.That(playerstatistic.LimpMp, Is.EqualTo(1), nameof(playerstatistic.LimpMp));
                Assert.That(playerstatistic.LimpCo, Is.EqualTo(1), nameof(playerstatistic.LimpCo));
                Assert.That(playerstatistic.LimpBtn, Is.EqualTo(0), nameof(playerstatistic.LimpBtn));
                Assert.That(playerstatistic.LimpPossible, Is.EqualTo(11), nameof(playerstatistic.LimpPossible));

                Assert.That(playerstatistic.DidColdCallInSb, Is.EqualTo(0), nameof(playerstatistic.DidColdCallInSb));
                Assert.That(playerstatistic.DidColdCallInBb, Is.EqualTo(0), nameof(playerstatistic.DidColdCallInBb));
                Assert.That(playerstatistic.DidColdCallInEp, Is.EqualTo(3), nameof(playerstatistic.DidColdCallInEp));
                Assert.That(playerstatistic.DidColdCallInMp, Is.EqualTo(1), nameof(playerstatistic.DidColdCallInMp));
                Assert.That(playerstatistic.DidColdCallInCo, Is.EqualTo(0), nameof(playerstatistic.DidColdCallInCo));
                Assert.That(playerstatistic.DidColdCallInBtn, Is.EqualTo(1), nameof(playerstatistic.DidColdCallInBtn));
                Assert.That(playerstatistic.Couldcoldcall, Is.EqualTo(13), nameof(playerstatistic.Couldcoldcall));


                Assert.That(playerstatistic.DidColdCallThreeBet, Is.EqualTo(0), nameof(playerstatistic.DidColdCallThreeBet));
                Assert.That(playerstatistic.DidColdCallFourBet, Is.EqualTo(0), nameof(playerstatistic.DidColdCallFourBet));
                Assert.That(playerstatistic.DidColdCallVsOpenRaiseSb, Is.EqualTo(0), nameof(playerstatistic.DidColdCallVsOpenRaiseSb));
                Assert.That(playerstatistic.DidColdCallVsOpenRaiseCo, Is.EqualTo(0), nameof(playerstatistic.DidColdCallVsOpenRaiseCo));
                Assert.That(playerstatistic.DidColdCallVsOpenRaiseBtn, Is.EqualTo(0), nameof(playerstatistic.DidColdCallVsOpenRaiseBtn));
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

                if (playerstatistic.DidColdCallThreeBet == 1)
                {
                    Console.WriteLine("test");
                }
            }

            return true;
        }
    }
}