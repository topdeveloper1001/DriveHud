//-----------------------------------------------------------------------
// <copyright file="GGNUtilsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.GGNetwork;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class GGNUtilsTests
    {
        [Test]
        [TestCase("Bounty Hunters $2.10 - Table 11", "Bounty Hunters 2,100 - Table 11")]
        [TestCase("Bounty Hunters $31.50 - Table 2", "Bounty Hunters 31,500 - Table 2")]
        public void ReplaceMoneyWithChipsInTitleTest(string tableName, string expected)
        {
            var actual = GGNUtils.ReplaceMoneyWithChipsInTitle(tableName);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("Chinese Zodiac #10 9Max $0 Freeroll - Table 41", "Chinese Zodiac Rooster - Table 41")]
        [TestCase("Chinese Zodiac #12 9Max $0 Freeroll - Table 41", "Chinese Zodiac Pig - Table 41")]
        [TestCase("Chinese Zodiac #10 9Max $0 Freeroll", "Chinese Zodiac Rooster")]
        public void PurgeTournamentNameTest(string tournamentName, string expected)
        {
            var actual = GGNUtils.PurgeTournamentName(tournamentName);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}