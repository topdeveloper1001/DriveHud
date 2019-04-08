//-----------------------------------------------------------------------
// <copyright file="ExportFunctionsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Ifrastructure;
using DriveHUD.Entities;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Data;
using Model.Enums;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    class ExportFunctionsTests : BaseIndicatorsTests
    {
        private const string testFolder = "UnitTests\\TestData";

        protected override void ConfigureUnityContainer(IUnityContainer container)
        {
            base.ConfigureUnityContainer(container);

            var playerStatisticRepository = Substitute.For<IPlayerStatisticRepository>();
            container.RegisterInstance<IPlayerStatisticRepository>(playerStatisticRepository);
        }

        [TestCaseSource("TestDataCase", Category = "ExportFunctions.ConvertHHToForumFormatV2")]
        public void HandHistoryIsConvertedIntoForumFormatV2(string sourceFileName, string expectedResultFileName, EnumExportType exportType,
            EnumPokerSites site, TestIndicators[] testIndicators)
        {
            var pr = Process.GetProcesses();

            var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();

            playerStatisticRepository
                .GetPlayersIndicators<ExportIndicators>(Arg.Any<string[]>(), Arg.Any<short?>())
                .Returns(testIndicators?.ToDictionary(x => x.PlayerName, x => (ExportIndicators)x));

            var sourceFile = Path.Combine(testFolder, sourceFileName);
            var expectedResultFile = Path.Combine(testFolder, expectedResultFileName);

            if (!File.Exists(sourceFile))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", sourceFile));
            }

            if (!File.Exists(expectedResultFile))
            {
                throw new Exception(string.Format("Test file '{0}' doesn't exist", expectedResultFile));
            }

            var handHistoryText = File.ReadAllText(sourceFile);

            var factory = new HandHistoryParserFactoryImpl();

            var parser = factory.GetFullHandHistoryParser(site);

            var handHistory = parser.ParseFullHandHistory(handHistoryText);

            var actualHandHistoryForumText = ExportFunctions.ConvertHHToForumFormat(handHistory, exportType, true);
            actualHandHistoryForumText = ReplaceHeader(actualHandHistoryForumText);

            var expectedHandHistoryForumText = File.ReadAllText(expectedResultFile);

            Assert.That(actualHandHistoryForumText, Is.EqualTo(expectedHandHistoryForumText));
        }

        private static IEnumerable<TestCaseData> TestDataCase()
        {
            yield return new TestCaseData(
                "ExportTest-Forum-Source-RaiseTo.xml",
                "ExportTest-Forum-Result-RaiseTo-2Plus2.txt",
                EnumExportType.TwoPlusTwo,
                EnumPokerSites.Ignition,
                new[]
                {
                    new TestIndicators
                    {
                        PlayerName = "P1_120608NK",
                        VPIP = 31.2m,
                        PFR = 23.3m,
                        ThreeBet = 9m,
                        AggPr = 23.5m,
                        TotalHands = 223000
                    },
                    new TestIndicators
                    {
                        PlayerName = "P2_647958KF",
                        VPIP = 41.2m,
                        PFR = 13.3m,
                        ThreeBet = 3m,
                        AggPr = 33.5m,
                        TotalHands = 123000
                    },
                    new TestIndicators
                    {
                        PlayerName = "Hero",
                        VPIP = 24.64m,
                        PFR = 15.87m,
                        ThreeBet = 7.5m,
                        AggPr = 28.5m,
                        TotalHands = 455000
                    },
                    new TestIndicators
                    {
                        PlayerName = "P4_415406RG",
                        VPIP = 52.64m,
                        PFR = 10.87m,
                        ThreeBet = 1.45m,
                        AggPr = 45.23m,
                        TotalHands = 5500
                    },
                    new TestIndicators
                    {
                        PlayerName = "P7_248456BT",
                        VPIP = 4.64m,
                        PFR = 3.87m,
                        ThreeBet = 0m,
                        AggPr = 15m,
                        TotalHands = 100
                    },
                    new TestIndicators
                    {
                        PlayerName = "P8_128380BY",
                        VPIP = 19m,
                        PFR = 9.87m,
                        ThreeBet = 0.55m,
                        AggPr = 18.34m,
                        TotalHands = 4170
                    },
                    new TestIndicators
                    {
                        PlayerName = "P9_901887II",
                        VPIP = 32m,
                        PFR = 12m,
                        ThreeBet = 1m,
                        AggPr = 15.5m,
                        TotalHands = 5000
                    }
                }
            );
            yield return new TestCaseData(
                "ExportTest-Forum-Source-RaiseTo.xml",
                "ExportTest-Forum-Result-RaiseTo-CardsChat.txt",
                EnumExportType.CardsChat,
                EnumPokerSites.Ignition,
                new[]
                {
                    new TestIndicators
                    {
                        PlayerName = "P1_120608NK",
                        VPIP = 31.2m,
                        PFR = 23.3m,
                        ThreeBet = 9m,
                        AggPr = 23.5m,
                        TotalHands = 223000
                    },
                    new TestIndicators
                    {
                        PlayerName = "P2_647958KF",
                        VPIP = 41.2m,
                        PFR = 13.3m,
                        ThreeBet = 3m,
                        AggPr = 33.5m,
                        TotalHands = 123000
                    },
                    new TestIndicators
                    {
                        PlayerName = "Hero",
                        VPIP = 24.64m,
                        PFR = 15.87m,
                        ThreeBet = 7.5m,
                        AggPr = 28.5m,
                        TotalHands = 455000
                    },
                    new TestIndicators
                    {
                        PlayerName = "P4_415406RG",
                        VPIP = 52.64m,
                        PFR = 10.87m,
                        ThreeBet = 1.45m,
                        AggPr = 45.23m,
                        TotalHands = 5500
                    },
                    new TestIndicators
                    {
                        PlayerName = "P7_248456BT",
                        VPIP = 4.64m,
                        PFR = 3.87m,
                        ThreeBet = 0m,
                        AggPr = 15m,
                        TotalHands = 100
                    },
                    new TestIndicators
                    {
                        PlayerName = "P8_128380BY",
                        VPIP = 19m,
                        PFR = 9.87m,
                        ThreeBet = 0.55m,
                        AggPr = 18.34m,
                        TotalHands = 4170
                    },
                    new TestIndicators
                    {
                        PlayerName = "P9_901887II",
                        VPIP = 32m,
                        PFR = 12m,
                        ThreeBet = 1m,
                        AggPr = 15.5m,
                        TotalHands = 5000
                    }
                }
            );
            yield return new TestCaseData(
               "ExportTest-Forum-Source-RaiseTo.xml",
               "ExportTest-Forum-Result-RaiseTo-Plain.txt",
               EnumExportType.PlainText,
               EnumPokerSites.Ignition,
               new[]
               {
                    new TestIndicators
                    {
                        PlayerName = "P1_120608NK",
                        VPIP = 31.2m,
                        PFR = 23.3m,
                        ThreeBet = 9m,
                        AggPr = 23.5m,
                        TotalHands = 223000
                    },
                    new TestIndicators
                    {
                        PlayerName = "P2_647958KF",
                        VPIP = 41.2m,
                        PFR = 13.3m,
                        ThreeBet = 3m,
                        AggPr = 33.5m,
                        TotalHands = 123000
                    },
                    new TestIndicators
                    {
                        PlayerName = "Hero",
                        VPIP = 24.64m,
                        PFR = 15.87m,
                        ThreeBet = 7.5m,
                        AggPr = 28.5m,
                        TotalHands = 455000
                    },
                    new TestIndicators
                    {
                        PlayerName = "P4_415406RG",
                        VPIP = 52.64m,
                        PFR = 10.87m,
                        ThreeBet = 1.45m,
                        AggPr = 45.23m,
                        TotalHands = 5500
                    },
                    new TestIndicators
                    {
                        PlayerName = "P7_248456BT",
                        VPIP = 4.64m,
                        PFR = 3.87m,
                        ThreeBet = 0m,
                        AggPr = 15m,
                        TotalHands = 100
                    },
                    new TestIndicators
                    {
                        PlayerName = "P8_128380BY",
                        VPIP = 19m,
                        PFR = 9.87m,
                        ThreeBet = 0.55m,
                        AggPr = 18.34m,
                        TotalHands = 4170
                    },
                    new TestIndicators
                    {
                        PlayerName = "P9_901887II",
                        VPIP = 32m,
                        PFR = 12m,
                        ThreeBet = 1m,
                        AggPr = 15.5m,
                        TotalHands = 5000
                    }
               }
            );
            yield return new TestCaseData(
               "ExportTest-Forum-Source-MTT.xml",
               "ExportTest-Forum-Result-MTT.txt",
               EnumExportType.TwoPlusTwo,
               EnumPokerSites.Ignition,
               null
            );
            yield return new TestCaseData(
              "ExportTest-Forum-Source-Zone.xml",
              "ExportTest-Forum-Result-Zone.txt",
              EnumExportType.TwoPlusTwo,
              EnumPokerSites.Ignition,
              null
           );
            yield return new TestCaseData(
               "ExportTest-Forum-Source-CashWithPost.xml",
               "ExportTest-Forum-Result-CashWithPost.txt",
               EnumExportType.TwoPlusTwo,
               EnumPokerSites.Ignition,
               null
            );
            yield return new TestCaseData(
               "ExportTest-Forum-Source-Straddle.xml",
               "ExportTest-Forum-Result-Straddle.txt",
               EnumExportType.TwoPlusTwo,
               EnumPokerSites.PokerMaster,
               null
            );
            yield return new TestCaseData(
             "ExportTest-Forum-Source-Omaha.txt",
             "ExportTest-Forum-Result-Omaha.txt",
             EnumExportType.TwoPlusTwo,
             EnumPokerSites.WinningPokerNetwork,
             null
            );
            yield return new TestCaseData(
             "ExportTest-Forum-Source-Cash.xml",
             "ExportTest-Forum-Result-Cash.txt",
             EnumExportType.TwoPlusTwo,
             EnumPokerSites.BetOnline,
             null
            );
            yield return new TestCaseData(
            "ExportTest-Forum-Source-Many-MP.xml",
            "ExportTest-Forum-Result-Many-MP.txt",
            EnumExportType.TwoPlusTwo,
            EnumPokerSites.Ignition,
            null
           );
        }

        private static string ReplaceHeader(string text)
        {
            return text
                .Replace("Hand History driven straight to this forum with DriveHUD [url=http://drivehud.com/?t=hh]Poker HUD[/url] and Database Software",
                "Hand History driven straight to this forum with DriveHUD [url=http://drivehud.com/?t=hh]Poker Tracking[/url] Software")
                .Replace("Hand History driven straight to this forum with DriveHUD Poker HUD and Database Software",
                "Hand History driven straight to this forum with DriveHUD Poker Tracking Software");
        }

        public abstract class TestIndicatorsAbstract : ExportIndicators
        {
            public sealed override decimal VPIP => VPIPGetter;

            protected abstract decimal VPIPGetter { get; }

            public sealed override decimal PFR => PFRGetter;

            protected abstract decimal PFRGetter { get; }

            public sealed override decimal ThreeBet => ThreeBetGetter;

            protected abstract decimal ThreeBetGetter { get; }

            public sealed override decimal AggPr => AggPrGetter;

            protected abstract decimal AggPrGetter { get; }

            public sealed override decimal TotalHands => TotalHandsGetter;

            protected abstract decimal TotalHandsGetter { get; }
        }

        public class TestIndicators : TestIndicatorsAbstract
        {
            public string PlayerName { get; set; }

            public new decimal VPIP { get; set; }

            protected sealed override decimal VPIPGetter { get { return VPIP; } }

            public new decimal PFR { get; set; }

            protected sealed override decimal PFRGetter { get { return PFR; } }

            public new decimal ThreeBet { get; set; }

            protected sealed override decimal ThreeBetGetter { get { return ThreeBet; } }

            public new decimal AggPr { get; set; }

            protected sealed override decimal AggPrGetter { get { return AggPr; } }

            public new int TotalHands { get; set; }

            protected sealed override decimal TotalHandsGetter { get { return TotalHands; } }
        }
    }
}