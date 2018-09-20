//-----------------------------------------------------------------------
// <copyright file="Adda52TableServiceTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.Importers.Adda52;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Settings;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DriveHud.Tests.ProxyImporterTests.Adda52Tests
{
    [TestFixture]
    class Adda52TableServiceTests
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var unityContainer = new UnityContainer();

            var eventAggregator = Substitute.For<IEventAggregator>();
            unityContainer.RegisterInstance(eventAggregator);

            unityContainer.RegisterType<ISettingsService, SettingsServiceStub>();
          
            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        [TestCase("Pune Pockets - 39 | VIP | Blinds : 1/2", true)]
        [TestCase("PLO Hi/Lo - 71 | VIP | Blinds : 1/2", true)]
        [TestCase("Freeroll Trips - 06 | Freeroll | Blinds : 25/50", true)]
        [TestCase("SNG 4 Player Prize 40 - 59 | Blinds : 15/30 | Ante : 0", true)]
        [TestCase("Super 200 - 05 | Blinds : 2000/4000 | Ante : 400", true)]
        [TestCase("Play Online Poker! Adda52.com", false)]
        [TestCase("SNG 6 Player Prize 6000 - 83", false)]
        [TestCase("Super 200 || Tournament is Running for last 02:18:26", false)]
        public void MatchTests(string title, bool expected)
        {
            var adda52TableService = new Adda52TableServiceStub();
            var actual = adda52TableService.Match(title);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test, TestCaseSource("GetWindowTestData")]
        public void GetWindowTest(HandHistory handHistory, Dictionary<int, string> openedTables, int expectedHandle)
        {
            var adda52TableService = new Adda52TableServiceStub();
            adda52TableService.InitializeOpenedTables(openedTables);

            var actualHandle = adda52TableService.GetWindow(handHistory).ToInt32();

            Assert.That(actualHandle, Is.EqualTo(expectedHandle));
        }

        /// <summary>
        /// Tests if Adda52 process can be found (requires to run adda52 on test system)
        /// </summary>   
        // [Test]
        public void GetClientProcessTests()
        {
            var adda52TableService = new Adda52TableServiceStub();

            var process = adda52TableService.GetPokerClientProcess();

            Assert.IsNotNull(process);
        }

        #region Data

        public static IEnumerable<TestCaseData> GetWindowTestData
        {
            get
            {
                yield return new TestCaseData(
                    new HandHistory
                    {
                        TableName = "RING9#3192339",
                        GameDescription = new GameDescriptor(
                            PokerFormat.CashGame,
                            EnumPokerSites.Adda52,
                            GameType.NoLimitHoldem,
                            Limit.FromSmallBlindBigBlind(1, 2, Currency.INR),
                            TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                            SeatType.FromMaxPlayers(9), null)
                        {
                            CashBuyInHigh = 80
                        }
                    }, GetWindowTestDataOpenedTables(), 1)
                    .SetName("CashRegular | Pune Pockets 39 | Blinds 1/2");

                yield return new TestCaseData(
                   new HandHistory
                   {
                       TableName = "RING9#3354639",
                       GameDescription = new GameDescriptor(
                           PokerFormat.CashGame,
                           EnumPokerSites.Adda52,
                           GameType.NoLimitHoldem,
                           Limit.FromSmallBlindBigBlind(25, 50, Currency.PlayMoney),
                           TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                           SeatType.FromMaxPlayers(9), null)
                       {
                           CashBuyInHigh = 3000
                       }
                   }, GetWindowTestDataOpenedTables(), 2)
                   .SetName("CashFreeroll | Freeroll Trips 39 | Blinds 25/50");

                yield return new TestCaseData(
                  new HandHistory
                  {
                      TableName = "RING6#3354671",
                      GameDescription = new GameDescriptor(
                          PokerFormat.CashGame,
                          EnumPokerSites.Adda52,
                          GameType.PotLimitOmahaHiLo,
                          Limit.FromSmallBlindBigBlind(1, 2, Currency.INR),
                          TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                          SeatType.FromMaxPlayers(6), null)
                      {
                          CashBuyInHigh = 80
                      }
                  }, GetWindowTestDataOpenedTables(), 3)
                  .SetName("CashRegular | PLO Hi/Lo - 71 | Blinds 1/2");

                yield return new TestCaseData(
                   new HandHistory
                   {
                       TableName = "RING6#3354635",
                       GameDescription = new GameDescriptor(
                           PokerFormat.CashGame,
                           EnumPokerSites.Adda52,
                           GameType.NoLimitHoldem,
                           Limit.FromSmallBlindBigBlind(25, 50, Currency.INR),
                           TableType.FromTableTypeDescriptions(TableTypeDescription.Speed),
                           SeatType.FromMaxPlayers(6), null)
                       {
                           CashBuyInHigh = 10000
                       }
                   }, GetWindowTestDataOpenedTables(), 6)
                   .SetName("CashRegular | Delhi Deuce - 35 | Blinds 25/50");

                yield return new TestCaseData(
                    new HandHistory
                    {
                        TableName = "STT4#3354659",
                        GameDescription = new GameDescriptor(
                            PokerFormat.Tournament,
                            EnumPokerSites.Adda52,
                            GameType.NoLimitHoldem,
                            Limit.FromSmallBlindBigBlind(15, 30, Currency.Chips),
                            TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                            SeatType.FromMaxPlayers(4),
                            new TournamentDescriptor
                            {
                                BuyIn = Buyin.FromBuyinRake(10, 1, Currency.INR),
                                TournamentsTags = TournamentsTags.STT
                            })
                    }, GetWindowTestDataOpenedTables(), 4)
                    .SetName("STT | SNG 4 Player Prize 40 - 59");

                yield return new TestCaseData(
                   new HandHistory
                   {
                       TableName = "MTT8#3244605",
                       GameDescription = new GameDescriptor(
                           PokerFormat.Tournament,
                           EnumPokerSites.Adda52,
                           GameType.NoLimitHoldem,
                           Limit.FromSmallBlindBigBlind(2000, 4000, Currency.Chips, true, 400),
                           TableType.FromTableTypeDescriptions(TableTypeDescription.Regular),
                           SeatType.FromMaxPlayers(8),
                           new TournamentDescriptor
                           {
                               BuyIn = Buyin.FromBuyinRake(182, 18, Currency.INR),
                               TournamentsTags = TournamentsTags.MTT,
                               TournamentName = "Super 200"
                           })
                   }, GetWindowTestDataOpenedTables(), 5)
                   .SetName("MTT | Super 200 - 05");
            }
        }

        public static Dictionary<int, string> GetWindowTestDataOpenedTables()
        {
            return new Dictionary<int, string>
            {
                [1] = "Pune Pockets - 39 | VIP | Blinds : 1/2",
                [2] = "Freeroll Trips - 39 | Freeroll | Blinds : 25 / 50",
                [3] = "PLO Hi/Lo - 71 | VIP | Blinds : 1/2",
                [4] = "SNG 4 Player Prize 40 - 59 | Blinds : 15/30 | Ante : 0",
                [5] = "Super 200 - 05 | Blinds : 2000/4000 | Ante : 400",
                [6] = "Delhi Deuce - 35 | VIP | Blinds : 25/50"
            };
        }

        #endregion

        #region Helpers

        private class Adda52TableServiceStub : Adda52TableService
        {
            public void InitializeOpenedTables(Dictionary<int, string> tables)
            {
                openedTables = tables.ToDictionary(x => new IntPtr(x.Key), x => x.Value);
            }

            public new bool Match(string title)
            {
                return base.Match(title);
            }

            public new Process GetPokerClientProcess()
            {
                return base.GetPokerClientProcess();
            }
        }

        protected class SettingsServiceStub : ISettingsService
        {
            public SettingsModel GetSettings()
            {
                var settingsModel = new SettingsModel();
                settingsModel.GeneralSettings.TimeZoneOffset = 0;
                return settingsModel;
            }

            public void SaveSettings(SettingsModel settings)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}