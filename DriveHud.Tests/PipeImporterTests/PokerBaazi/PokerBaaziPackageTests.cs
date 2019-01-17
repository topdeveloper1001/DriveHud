//-----------------------------------------------------------------------
// <copyright file="PokerBaaziPackageTests.cs" company="Ace Poker Solutions">
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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.PipeImporterTests.PokerBaazi
{
    [TestFixture]
    class PokerBaaziPackageTests
    {
        private Dictionary<string, string[]> cachedFiles = new Dictionary<string, string[]>();

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("Packets\\SpectatorResponse.json", PokerBaaziPackageType.StartGameResponse)]
        [TestCase("Packets\\InitResponse.json", PokerBaaziPackageType.InitResponse)]
        [TestCase("Packets\\RoundResponse.json", PokerBaaziPackageType.RoundResponse)]
        [TestCase("Packets\\UserButtonActionResponse.json", PokerBaaziPackageType.UserButtonActionResponse)]
        [TestCase("Packets\\WinnerResponse.json", PokerBaaziPackageType.WinnerResponse)]
        [TestCase("Packets\\StartGameResponse.json", PokerBaaziPackageType.StartGameResponse)]
        [TestCase("Packets\\TournamentDetailsResponse.json", PokerBaaziPackageType.TournamentDetailsResponse)]
        public void ParsePackageTypeTests(string file, PokerBaaziPackageType expectedPackageType)
        {
            file = Path.Combine(PokerBaaziTestsHelper.TestDataFolder, file);

            FileAssert.Exists(file);

            var data = File.ReadAllText(file);

            Assert.IsNotEmpty(data);

            var actualPackageType = PokerBaaziPackage.ParsePackageType(data);

            Assert.That(actualPackageType, Is.EqualTo(expectedPackageType));
        }

        [TestCase("RawData\\RawData-1.txt", 0, true, 100529u, 0L, PokerBaaziPackageType.InitResponse)]
        [TestCase("RawData\\RawData-1.txt", 1, true, 876154u, 0L, PokerBaaziPackageType.RoundResponse)]
        [TestCase("RawData\\RawData-1.txt", 2, false, 0u, 0L, PokerBaaziPackageType.Unknown)]
        [TestCase("RawData\\RawData-1.txt", 3, true, 48710u, 292754L, PokerBaaziPackageType.StartGameResponse)]
        [TestCase("RawData\\RawData-1.txt", 4, true, 0u, 292754L, PokerBaaziPackageType.TournamentDetailsResponse)]
        public void PokerBaaziPackageTryParseTest(string file, int lineNum, bool expectedResult, uint roomId, long tournamentId, PokerBaaziPackageType expectedType)
        {
            var lines = GetFileLines(file);

            Assert.That(lines.Length, Is.GreaterThan(lineNum));

            var data = lines[lineNum];

            Assert.IsNotEmpty(data);

            var result = PokerBaaziPackage.TryParse(data, out PokerBaaziPackage package);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(expectedResult), $"TryParse must return expected result.");
                Assert.That(package.RoomId, Is.EqualTo(roomId), $"RoomId must be equal expected.");
                Assert.That(package.TournamentId, Is.EqualTo(tournamentId), $"TournamentId must be equal expected.");
                Assert.That(package.PackageType, Is.EqualTo(expectedType), $"PackageType must be equal expected.");
            });
        }

        private string[] GetFileLines(string file)
        {
            file = Path.Combine(PokerBaaziTestsHelper.TestDataFolder, file);

            if (!cachedFiles.TryGetValue(file, out string[] lines))
            {
                FileAssert.Exists(file);

                lines = File.ReadAllLines(file);

                cachedFiles.Add(file, lines);
            }

            return lines;
        }
    }
}