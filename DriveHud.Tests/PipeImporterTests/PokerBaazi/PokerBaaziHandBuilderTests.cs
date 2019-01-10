//-----------------------------------------------------------------------
// <copyright file="PokerBaaziHandBuilderTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.PokerBaazi;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.Hand;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.PipeImporterTests.PokerBaazi
{
    [TestFixture]
    class PokerBaaziHandBuilderTests
    {
        private const string SourceJsonFile = "Source.json";
        private const string ExpectedResultFile = "Result.xml";

        private const string TestDataFolder = "HandsRawData";

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCase("mtt-9-max-no-hero-1")]
        public void TryBuildTest(string testFolder)
        {
            var packages = ReadPackages(testFolder);

            CollectionAssert.IsNotEmpty(packages, $"Packages collection must be not empty for {testFolder}");

            var handBuilder = new PokerBaaziHandBuilder();

            HandHistory actual = null;

            foreach (var package in packages)
            {
                if (handBuilder.TryBuild(package, out actual))
                {
                    break;
                }
            }

            Assert.IsNotNull(actual, $"Actual HandHistory must be not null for {testFolder}");

            var expected = ReadExpectedHandHistory(testFolder);

            Assert.IsNotNull(expected, $"Expected HandHistory must be not null for {testFolder}");

            AssertionUtils.AssertHandHistory(actual, expected);
        }

        private IEnumerable<PokerBaaziPackage> ReadPackages(string testFolder)
        {
            var packages = new List<PokerBaaziPackage>();

            var sourceJsonFile = Path.Combine(PokerBaaziTestsHelper.TestDataFolder, TestDataFolder, testFolder, SourceJsonFile);

            FileAssert.Exists(sourceJsonFile);

            var sourceJson = File.ReadAllText(sourceJsonFile);

            var testObject = JsonConvert.DeserializeObject<PokerBaaziTestSourceObject>(sourceJson);

            foreach (var packet in testObject.Packets)
            {
                var json = JsonConvert.SerializeObject(packet);

                var package = new PokerBaaziPackage
                {
                    PackageType = PokerBaaziPackage.ParsePackageType(json),
                    JsonData = json
                };

                packages.Add(package);
            }

            return packages;
        }

        private HandHistory ReadExpectedHandHistory(string testFolder)
        {
            var xmlFile = Path.Combine(TestDataFolder, TestDataFolder, testFolder, ExpectedResultFile);

            FileAssert.Exists(xmlFile);

            try
            {
                var handHistoryText = File.ReadAllText(xmlFile);
                return SerializationHelper.DeserializeObject<HandHistory>(handHistoryText);
            }
            catch (Exception e)
            {
                Assert.Fail($"{ExpectedResultFile} in {testFolder} has not been deserialized: {e}");
            }

            return null;
        }

        private class PokerBaaziTestSourceObject
        {
            public IEnumerable<object> Packets { get; set; }
        }
    }
}