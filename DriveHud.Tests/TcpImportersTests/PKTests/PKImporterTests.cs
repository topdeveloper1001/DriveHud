//-----------------------------------------------------------------------
// <copyright file="PKImporterTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.PokerKing;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System.Collections.Concurrent;
using System.Text;

namespace DriveHud.Tests.TcpImportersTests.PKTests
{
    [TestFixture]
    class PKImporterTests
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            var unityContainer = new UnityContainer();

            var pkCatcherService = Substitute.For<IPKCatcherService>();
            unityContainer.RegisterInstance(pkCatcherService);

            var eventAggregator = Substitute.For<IEventAggregator>();
            unityContainer.RegisterInstance(eventAggregator);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestCase("Form item: \"{\"gameid\":2, \"mode\":1, \"token\":\"5dc38b194d15784246cc6b889081e3c4\", \"uid\":2008947}", "5dc38b194d15784246cc6b889081e3c4", 2008947u)]
        public void TestProcessLoginPortPacket(string json, string token, uint userId)
        {
            var importer = new PKImporterStub();

            var bytes = Encoding.UTF8.GetBytes(json);

            var capturedPacket = new CapturedPacket
            {
                Bytes = bytes
            };

            importer.ProcessLoginPortPacket(capturedPacket);

            var result = importer.UserTokens.TryGetValue(userId, out string userToken);

            Assert.IsTrue(result, $"User token {userId} not found");
            Assert.That(userToken, Is.EqualTo(token), $"User token {userId} must match");
        }

        private class PKImporterStub : PKImporter
        {
            public new void ProcessLoginPortPacket(CapturedPacket packet)
            {
                base.ProcessLoginPortPacket(packet);
            }

            public ConcurrentDictionary<uint, string> UserTokens { get { return userTokens; } }
        }
    }
}