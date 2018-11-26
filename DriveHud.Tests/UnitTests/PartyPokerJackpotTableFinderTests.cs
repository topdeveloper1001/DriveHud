//-----------------------------------------------------------------------
// <copyright file="PartyPokerJackpotTableFinderTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PartyPoker;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class PartyPokerJackpotTableFinderTests
    {
        private const string testFolder = "../../UnitTests/TestData/PartyPoker/HandHistory/UserName/20181115";

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("190730272", "$0.25 Sit & Go 3-Handed (190730272) Table #1.txt", "5209023")]
        [TestCase("190735059", "$0.25 Sit & Go 3-Handed (190735059) Table #1.txt", "5208910")]
        public void FindTableIdTest(string tournamentId, string fileName, string expectedTableId)
        {
            var file = Path.Combine(testFolder, fileName);

            var fileInfo = new FileInfo(file);

            Assert.IsTrue(fileInfo.Exists, $"{file} must exists.");

            var actualTableId = PartyPokerJackpotTableFinder.FindTableId(tournamentId, fileInfo.FullName);
            Assert.That(actualTableId, Is.EqualTo(expectedTableId));
        }
    }
}