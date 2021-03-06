﻿//-----------------------------------------------------------------------
// <copyright file="IgnitionInfoDataManagerTests.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.UnitTests.Helpers;
using DriveHUD.Importers.Bovada;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class IgnitionInfoDataManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        [TestCase("ign-info.log", 1205u, "Zone Poker - Patriots", GameType.Holdem, GameLimit.NL, 6, true)]
        [TestCase("ign-info.log", 1206u, "Zone Poker - Bills", GameType.Holdem, GameLimit.NL, 9, true)]
        [TestCase("ign-info.log", 1218u, "Zone Poker - Giants", GameType.Omaha, GameLimit.PL, 6, true)]
        [TestCase("ign-info.log", 1223u, "Zone Poker - Packers", GameType.OmahaHiLo, GameLimit.NL, 6, true)]
        public void ZoneDataIsRead(string fileName, uint expectedGameId, string expectedName, GameType expectedGameType,
            GameLimit expectedLimit, int expectedSeats, bool isZone)
        {
            var infoData = IgnitionTestsHelper.PrepareInfoTestData(GetTestDataFilePath(fileName));

            // create mock for event aggregator
            var eventAggregator = Substitute.For<IEventAggregator>();

            var infoDataManager = new IgnitionInfoDataManagerStub(eventAggregator);

            foreach (var data in infoData)
            {
                infoDataManager.ProcessData(data);
            }

            var tableData = infoDataManager.GetTableData(expectedGameId);

            Assert.IsNotNull(tableData, "TableData must be found");
            Assert.That(tableData.TableName, Is.EqualTo(expectedName));
            Assert.That(tableData.GameType, Is.EqualTo(expectedGameType));
            Assert.That(tableData.GameLimit, Is.EqualTo(expectedLimit));
            Assert.That(tableData.TableSize, Is.EqualTo(expectedSeats));
            Assert.That(tableData.IsZone, Is.EqualTo(isZone));
        }      

        private static string GetTestDataFilePath(string name)
        {
            return string.Format("UnitTests\\TestData\\Ignition\\{0}", name);
        }       
    }
}