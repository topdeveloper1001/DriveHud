//-----------------------------------------------------------------------
// <copyright file="WinningPokerNetworkFileBasedImporterTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.Importers.WinningPokerNetwork;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class WinningPokerNetworkFileBasedImporterTests
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            var unityContainer = new UnityContainer();

            var eventAggregator = Substitute.For<IEventAggregator>();
            unityContainer.RegisterInstance(eventAggregator);

            var dataService = Substitute.For<IDataService>();
            unityContainer.RegisterInstance(dataService);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestCase("WHH20190104 TJPTG10354204T1.txt", "10354204")]
        [TestCase("HH20181128 T9231156-G61246348.txt", "9231156")]
        public void TestGetTournamentNumberFromFile(string fileName, string expectedTournamentNumber)
        {
            var importer = new WinningPokerNetworkFileBasedImporterStub();

            var actualTournamentNumber = importer.GetTournamentNumberFromFile(fileName);

            Assert.That(actualTournamentNumber, Is.EqualTo(expectedTournamentNumber));
        }

        private class WinningPokerNetworkFileBasedImporterStub : WinningPokerNetworkFileBasedImporter
        {
            public WinningPokerNetworkFileBasedImporterStub()
            {
            }

            protected override string[] ProcessNames => throw new NotImplementedException();

            protected override EnumPokerSites Site => throw new NotImplementedException();

            public new string GetTournamentNumberFromFile(string fileName)
            {
                return base.GetTournamentNumberFromFile(fileName);
            }
        }
    }
}