//-----------------------------------------------------------------------
// <copyright file="ZoneHandAdjusterTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Builders.iPoker;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    internal class ZoneHandAdjusterTests
    {
        private const string TestDataFolder = @"..\..\UnitTests\TestData\ZoneHandAdjusterData";

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("Zone-Hand-Source-1.xml", "Zone-Hand-Result-1.xml")]
        public void AdjustHandTest(string sourceFile, string expectedResultFile)
        {
            var source = File.ReadAllText(GetFilePath(sourceFile));
            var expected = File.ReadAllText(GetFilePath(expectedResultFile));

            var zoneHandAdjuster = new ZoneHandAdjuster();
            var actual = zoneHandAdjuster.Adjust(source);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private static string GetFilePath(string file)
        {
            return Path.Combine(TestDataFolder, file);
        }
    }
}