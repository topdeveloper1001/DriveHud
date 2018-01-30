//-----------------------------------------------------------------------
// <copyright file="IPokerBaseTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.Base;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Prism.Events;
using System;
using System.IO;
using System.Linq;

namespace DriveHud.Tests.IntegrationTests.Parsers.IPoker
{
    abstract class IPokerBaseTests
    {
        protected const string TestDataFolder = @"..\..\IntegrationTests\Parsers\IPoker\TestData";

        [OneTimeSetUp]
        public void Initialize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        protected HandHistory ParseHandHistory(string handHistoryFile)
        {
            var parser = CreateParser();

            var handHistoryText = File.ReadAllText(handHistoryFile);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.Single();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        protected abstract IHandHistoryParser CreateParser();

        private void ConfigureContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IEventAggregator, EventAggregator>();

            var locator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}
