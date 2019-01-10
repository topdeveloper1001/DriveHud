//-----------------------------------------------------------------------
// <copyright file="PokerBaaziDataManagerTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerBaazi;
using DriveHUD.Importers.PokerBaazi.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.PipeImporterTests.PokerBaazi
{
    [TestFixture]
    class PokerBaaziDataManagerTests
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("Packets\\InitResponse.json")]
        public void DeserializationTest(string file)
        {
            file = Path.Combine(PokerBaaziTestsHelper.TestDataFolder, file);

            FileAssert.Exists(file);

            var data = File.ReadAllText(file);

            Assert.IsNotEmpty(data);

            var actualPackageType = PokerBaaziPackage.ParsePackageType(data);

            var initResponse = JsonConvert.DeserializeObject<PokerBaaziResponse<PokerBaaziInitResponse>>(data);            

            Assert.That(initResponse.ClassObj.MaxPlayers, Is.EqualTo(9));
        }
    }
}