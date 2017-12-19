//-----------------------------------------------------------------------
// <copyright file="ExternalImporterUtilsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.ExternalImporter;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class ExternalImporterUtilsTests
    {
        [Test]
        [TestCase("Bounty Hunters $2.10 : Buy-in $1.10 - Blinds 100 / 200 - Table 3", "Bounty Hunters $2.10 - Table 3", true)]
        [TestCase("Bounty Hunters 2,100 : Buy-in 1,100 - Blinds 100 / 200 - Table 3", "Bounty Hunters $2.10 - Table 3", true)]
        [TestCase("悬赏金猎人 $2.10 : 重購 $1.10 - 盲註 50 / 100 - 牌桌 3", "Bounty Hunters $2.10 - Table 3", true)]
        [TestCase("中國十二生肖 虎 : 免費賽 - 盲註 2500 / 5000 - 牌桌 14", "Chinese Zodiac Tiger - Table 14", true)]
        public void TournamentTableMatchTest(string title, string tableName, bool expected)
        {
            var actual = ExternalImporterUtils.IsTournamentTableMatch(title, tableName);
            Assert.That(actual, Is.EqualTo(expected));
        }        
    }
}