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
using Model.Data;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// Total playerStatistic integration tests
    /// </summary>
    [TestFixture]
    class TotalPlayerStatisticTests : PlayerStatisticTests
    {
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
        [TestCase(EnumPokerSites.PokerStars, "PLR_2961194VK")]
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


                Assert.That(Math.Round((decimal)100 * playerstatistic.Didthreebet / playerstatistic.Couldthreebet, 1), Is.EqualTo(10.6), nameof(playerstatistic.DidColdCallIp));

                //Assert.That(Math.Round((decimal) 100*playerstatistic.LimpMade/playerstatistic.LimpPossible,1), Is.EqualTo(50), nameof(playerstatistic.DidColdCallIp));  
            }
        }
    }
}