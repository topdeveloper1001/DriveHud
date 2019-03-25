//-----------------------------------------------------------------------
// <copyright file="Adda52HandBuilderTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.Adda52;
using DriveHUD.Importers.Adda52.Model;
using DriveHUD.Importers.AndroidBase;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.QualityTools.Testing.Fakes;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Fakes;
using System.IO;
using System.Text;

namespace DriveHud.Tests.ProxyImporterTests.Adda52Tests
{
    [TestFixture]
    class Adda52HandBuilderTests
    {
        private const string SourceJsonFile = "Source.json";
        private const string ExpectedResultFile = "Result.xml";
        private static readonly DateTime handDate = new DateTime(2018, 9, 12, 0, 5, 4, DateTimeKind.Utc);

        private const string TestDataFolder = @"ProxyImporterTests\Adda52Tests\TestData\HandsRawData";

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<IPackageBuilder<Adda52Package>, Adda52PackageBuilder>();

            var eventAggregator = Substitute.For<IEventAggregator>();
            unityContainer.RegisterInstance(eventAggregator);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestCase("regular-6-max-no-hero-1")]
        [TestCase("regular-6-max-no-hero")]
        [TestCase("regular-9-max-hero")]
        [TestCase("regular-9-max-no-hero-1")]
        [TestCase("mtt-9-max-no-hero-1")]
        [TestCase("stt-6-max-no-hero-1")]
        public void TryBuildTest(string testFolder)
        {
            HandHistory actual = null;

            using (ShimsContext.Create())
            {
                ShimDateTime.UtcNowGet = () => handDate;

                var packages = ReadPackages(testFolder);

                CollectionAssert.IsNotEmpty(packages, $"Packages collection must be not empty for {testFolder}");

                var handBuilder = new Adda52HandBuilder();
                
                foreach (var package in packages)
                {
                    if (handBuilder.TryBuild(package, out actual))
                    {
                        break;
                    }
                }
            }

            Assert.IsNotNull(actual, $"Actual HandHistory must be not null for {testFolder}");

            var expected = ReadExpectedHandHistory(testFolder);

            Assert.IsNotNull(expected, $"Expected HandHistory must be not null for {testFolder}");

            AssertionUtils.AssertHandHistory(actual, expected);
        }

        private IEnumerable<Adda52JsonPackage> ReadPackages(string testFolder)
        {
            var packages = new List<Adda52JsonPackage>();

            var sourceJsonFile = Path.Combine(TestDataFolder, testFolder, SourceJsonFile);

            FileAssert.Exists(sourceJsonFile);

            var sourceJson = File.ReadAllText(sourceJsonFile);

            var testObject = JsonConvert.DeserializeObject<Adda52TestSourceObject>(sourceJson);

            foreach (var packet in testObject.Packages)
            {
                var json = JsonConvert.SerializeObject(packet);                

                if (Adda52JsonPackage.TryParse(json, out Adda52JsonPackage package))
                {
                    packages.Add(package);
                }
            }

            return packages;
        }

        private HandHistory ReadExpectedHandHistory(string folder)
        {
            var xmlFile = Path.Combine(TestDataFolder, folder, ExpectedResultFile);

            FileAssert.Exists(xmlFile);

            try
            {
                var handHistoryText = File.ReadAllText(xmlFile);
                return SerializationHelper.DeserializeObject<HandHistory>(handHistoryText);
            }
            catch (Exception e)
            {
                Assert.Fail($"{ExpectedResultFile} in {folder} has not been deserialized: {e}");
            }

            return null;
        }

        private class Adda52TestSourceObject
        {
            public IEnumerable<object> Packages { get; set; }
        }
    }
}