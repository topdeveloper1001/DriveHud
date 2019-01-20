//-----------------------------------------------------------------------
// <copyright file="PKModelTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerKing.Model;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.TcpImportersTests.PKTests
{
    [TestFixture]
    class PKModelTests
    {
        private const string TestFolder = @"..\..\TcpImportersTests\PKTests\TestData\ModelData";

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, TestFolder);
        }

        [TestCase("PKLoginResponse.xml", 105772u, "2b9a55c70c8c8e7300f24b0c2c540d1e")]
        public void PKLoginResponseTryParseTest(string fileName, uint userId, string userToken)
        {
            var fileText = File.ReadAllText(fileName);

            var result = PKLoginResponse.TryParse(fileText, out PKLoginResponse loginResponse);

            Assert.IsTrue(result, "TryParse result must be true");
            Assert.IsNotNull(loginResponse);

            Assert.Multiple(() =>
            {
                Assert.That(loginResponse.UserId, Is.EqualTo(userId));
                Assert.That(loginResponse.UserToken, Is.EqualTo(userToken));
            });
        }
    }
}