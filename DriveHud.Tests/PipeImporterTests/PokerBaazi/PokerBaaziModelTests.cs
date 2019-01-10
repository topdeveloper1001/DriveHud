//-----------------------------------------------------------------------
// <copyright file="PokerBaaziModelTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerBaazi.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.PipeImporterTests.PokerBaazi
{
    [TestFixture]
    class PokerBaaziModelTests
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("Packets\\InitResponse.json")]
        public void InitResponseDeserializationTest(string file)
        {
            file = Path.Combine(PokerBaaziTestsHelper.TestDataFolder, file);

            FileAssert.Exists(file);

            var data = File.ReadAllText(file);

            Assert.IsNotEmpty(data);

            var actualPackageType = PokerBaaziPackage.ParsePackageType(data);

            var initResponse = JsonConvert.DeserializeObject<PokerBaaziResponse<PokerBaaziInitResponse>>(data);

            Assert.Multiple(() =>
            {
                Assert.That(initResponse.ClassObj.MaxPlayers, Is.EqualTo(9));
                Assert.That(initResponse.ClassObj.TournamentName, Is.EqualTo("Dazzling Deuces"));
                Assert.That(initResponse.ClassObj.RoomId, Is.EqualTo(100529));
                Assert.That(initResponse.ClassObj.SmallBlind, Is.EqualTo(1));
                Assert.That(initResponse.ClassObj.BigBlind, Is.EqualTo(2));
                Assert.That(initResponse.ClassObj.UserId, Is.EqualTo(375337));
                Assert.That(initResponse.ClassObj.TournamentId, Is.EqualTo(231044));
                Assert.That(initResponse.ClassObj.Straddle, Is.EqualTo(false));
            });
        }
    }
}