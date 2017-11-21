//-----------------------------------------------------------------------
// <copyright file="GGNSerializationTests.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.GGNetwork;
using DriveHUD.Importers.GGNetwork.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class GGNSerializationTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("TournamentsInfo.json", 1049)]
        public void TournamentsInfoCanBeDeserialized(string file, int expected)
        {
            var tournamentsInfoJson = File.ReadAllText(GetTestDataFilePath(file));

            TournamentInfoResponse tournamentsInformationResponse = null;

            Assert.DoesNotThrow(() =>
            {
                tournamentsInformationResponse = JsonConvert.DeserializeObject<TournamentInfoResponse>(tournamentsInfoJson);
            });

            Assert.IsNotNull(tournamentsInformationResponse, "Tournaments info must be not null");

            Assert.That(tournamentsInformationResponse.Tournaments.Count, Is.EqualTo(expected));
        }

        [TestCase("MultipleHandHistoriesInfo.json", 3)]
        public void MultipleHandHistoriesCanBeDeserialized(string file, int expected)
        {
            var handHistoriesInfoJson = File.ReadAllText(GetTestDataFilePath(file));

            HandHistoriesInformation handHistoriesInformation = null;

            Assert.DoesNotThrow(() =>
            {
                handHistoriesInformation = JsonConvert.DeserializeObject<HandHistoriesInformation>(handHistoriesInfoJson);
            });

            Assert.IsNotNull(handHistoriesInformation, "HandHistoriesInformation must be not null");

            Assert.That(handHistoriesInformation.Histories.Count, Is.EqualTo(expected));
        }

        [TestCase("SingleHandHistoryInfo.json")]
        public void SingleHandHistoryCanBeDeserialized(string file)
        {
            var handHistoriesInfoJson = File.ReadAllText(GetTestDataFilePath(file));

            HandHistoryInformation handHistoryInformation = null;

            Assert.DoesNotThrow(() =>
            {
                handHistoryInformation = JsonConvert.DeserializeObject<HandHistoryInformation>(handHistoriesInfoJson);
            });

            Assert.IsNotNull(handHistoryInformation, "HandHistoryInformation must be not null");
            Assert.IsNotNull(handHistoryInformation.HandHistory, "HandHistoryInformation.HandHistory must be not null");
        }

        private static string GetTestDataFilePath(string name)
        {
            return string.Format("UnitTests\\TestData\\GGNJsonData\\{0}", name);
        }
    }
}