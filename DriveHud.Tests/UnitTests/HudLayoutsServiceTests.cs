//-----------------------------------------------------------------------
// <copyright file="HudLayoutServiceTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Data;
using Model.Enums;
using Model.Hud;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class HudLayoutsServiceTests
    {
        private const string HudLayoutTestDataFolder = @"..\..\UnitTests\TestData\HudLayouts";

        private IUnityContainer unityContainer;

        private HudLayoutsService hudLayoutService;

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            ResourceRegistrator.Initialization();

            unityContainer = new UnityContainer();

            var hudPlayerTypeService = Substitute.For<IHudPlayerTypeService>();
            hudPlayerTypeService.GetImageLink(Arg.Any<string>()).Returns(x => ((string)x[0]).Replace(".png", string.Empty));

            unityContainer.RegisterInstance(hudPlayerTypeService);

            var eventAggregator = Substitute.For<IEventAggregator>();
            unityContainer.RegisterInstance(eventAggregator);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);

            hudLayoutService = new HudLayoutsServiceStub();
            hudLayoutService.Import("6maxHUDforDEV.xml");
        }

        [TestCaseSource("SetStickerDataSet")]
        public void TestSetSticker(string layoutName, string stickerName, BumperSticketTestData data, string label)
        {
            var layout = hudLayoutService.GetLayout(layoutName);

            Assert.IsNotNull(layout, $"Layout {layoutName} not found");

            var hudElement = CreateHudElement(layout, EnumGameType.CashHoldem);

            var stickerIndicators = new HudStickerIndicators();

            for (var i = 0; i < 120; i++)
            {
                var stat = new PlayerstatisticTest
                {
                    Flopcontinuationbetmade = i < data.CBet ? 1 : 0,
                    Flopcontinuationbetpossible = i < 100 ? 1 : 0,
                    Pfrhands = i < 100 ? 1 : 0,
                    Facedthreebetpreflop = i < 100 ? 1 : 0,
                    Foldedtothreebetpreflop = i < data.FoldTo3Bet ? 1 : 0,
                    Didthreebet = i < data.S3Bet ? 1 : 0,
                    Couldthreebet = i < 100 ? 1 : 0,
                    Totalhands = 1
                };

                stat.SetDidDoubleBarrel(i < data.DoubleBarrel ? 1 : 0);
                stat.SetCouldDoubleBarrel(i < 100 ? 1 : 0);

                stickerIndicators.AddStatistic(stat);
            }

            Assert.That(stickerIndicators.GetStatValue(Stat.CBet), Is.EqualTo(data.CBet), "CBet is incorrect");
            Assert.That(stickerIndicators.GetStatValue(Stat.DoubleBarrel), Is.EqualTo(data.DoubleBarrel), "DoubleBarrel is incorrect");
            Assert.That(stickerIndicators.GetStatValue(Stat.S3Bet), Is.EqualTo(data.S3Bet), "S3Bet is incorrect");
            Assert.That(stickerIndicators.GetStatValue(Stat.FoldTo3Bet), Is.EqualTo(data.FoldTo3Bet), "FoldTo3Bet is incorrect");

            var stickersStatistic = new Dictionary<string, HudStickerIndicators>
            {
                [stickerName] = stickerIndicators
            };

            hudLayoutService.SetStickers(
                hudElement,
                stickersStatistic,
                layout);

            if (!string.IsNullOrEmpty(label))
            {
                Assert.That(hudElement.Stickers.Count, Is.GreaterThan(0), $"Label {label} not found");
                Assert.That(hudElement.Stickers[0].Label, Is.EqualTo(label));
            }
            else
            {
                Assert.That(hudElement.Stickers.Count, Is.EqualTo(0));
            }
        }

        public static IEnumerable<TestCaseData> SetStickerDataSet()
        {
            yield return new TestCaseData(
                    "DH: 6-max",
                    "One and Done",
                    new BumperSticketTestData
                    {
                        CBet = 60,
                        DoubleBarrel = 0
                    },
                    "OD"
                ).SetName("One and Done set: 6-max, CBet 60, DoubleBarrel 0");

            yield return new TestCaseData(
                   "DH: 6-max",
                   "One and Done",
                   new BumperSticketTestData
                   {
                       CBet = 50,
                       DoubleBarrel = 20
                   },
                   string.Empty
               ).SetName("One and Done not set: 6-max, CBet 50, DoubleBarrel 20");

            yield return new TestCaseData(
                   "DH: 6-max",
                   "One and Done",
                   new BumperSticketTestData
                   {
                       CBet = 55,
                       DoubleBarrel = 45
                   },
                   string.Empty
               ).SetName("One and Done not set: 6-max, CBet 55, DoubleBarrel 45");

            yield return new TestCaseData(
                   "DH: 6-max",
                   "One and Done",
                   new BumperSticketTestData
                   {
                       CBet = 55,
                       DoubleBarrel = 25
                   },
                   "OD"
               ).SetName("One and Done is set: 6-max, CBet 60, DoubleBarrel 25");

            yield return new TestCaseData(
                   "DH: 6-max",
                   "3 For Free",
                   new BumperSticketTestData
                   {
                       S3Bet = 55,
                       FoldTo3Bet = 70
                   },
                   "3FF"
                ).SetName("3 for free set: 6-max, S3Bet 55, FoldTo3Bet 70");
        }

        [TestCaseSource("SetPlayerProfileDataSet")]
        public void TestSetPlayerIcon(string layoutName, PlayerProfileTestData data, string profile)
        {
            var layout = hudLayoutService.GetLayout(layoutName);

            Assert.IsNotNull(layout, $"Layout {layoutName} not found");

            var hudElement = CreateHudElement(layout, EnumGameType.CashHoldem);

            hudElement.SetStatValue(Stat.VPIP, data.VPIP);
            hudElement.SetStatValue(Stat.PFR, data.PFR);
            hudElement.SetStatValue(Stat.AGG, data.AGG);
            hudElement.SetStatValue(Stat.S3Bet, data.S3Bet);
            hudElement.SetStatValue(Stat.AF, data.AF);
            hudElement.SetStatValue(Stat.CBet, data.CBet);
            hudElement.SetStatValue(Stat.FoldToCBet, data.FoldToCBet);
            hudElement.SetStatValue(Stat.FoldTo3Bet, data.FoldTo3Bet);
            hudElement.SetStatValue(Stat.WWSF, data.WWSF);
            hudElement.SetStatValue(Stat.WTSD, data.WTSD);
            hudElement.SetStatValue(Stat.Steal, data.Steal);
            hudElement.SetStatValue(Stat.DonkBet, data.DonkBet);
            hudElement.SetStatValue(Stat.TotalHands, data.TotalHands);

            hudLayoutService.SetPlayerTypeIcon(new[] { hudElement }, layout);

            if (string.IsNullOrEmpty(profile))
            {
                Assert.That(hudElement.PlayerIcon, Is.Null);
            }
            else
            {
                Assert.That(hudElement.PlayerIcon, Is.EqualTo(profile));
            }
        }

        public static IEnumerable<TestCaseData> SetPlayerProfileDataSet()
        {
            yield return new TestCaseData(
                "DH: 6-max",
                new PlayerProfileTestData
                {
                    VPIP = 26.9m,
                    PFR = 19.2m,
                    AF = 1.3m,
                    WTSD = 25m,
                    FoldToCBet = 50m
                },
                "Standard Reg"
            ).SetName("Player Profile: Std Reg (not meet req.)");

            yield return new TestCaseData(
                "DH: 6-max",
                new PlayerProfileTestData
                {
                    VPIP = 45m,
                    PFR = 14m,
                    AF = 2m,
                    S3Bet = 5.6m
                },
                "Whale"
            ).SetName("Player Profile: Whale (not meet req.)");


            yield return new TestCaseData(
                "DH: 6-max",
                new PlayerProfileTestData
                {
                    VPIP = 24.2m,
                    PFR = 18.2m,
                    AF = 2,
                    WTSD = 20,
                    FoldTo3Bet = 0,
                    CBet = 33,
                    WWSF = 60,
                    AGG = 28.57m,
                    Steal = 45.45m,
                    TotalHands = 33
                },
                "Standard Reg"
            ).SetName("Player Profile: Standard Reg (not meet req.)");

            yield return new TestCaseData(
                "DH: BOL 6 max test hud (12/27/18)",
                new PlayerProfileTestData
                {
                    VPIP = 24.2m,
                    PFR = 18.2m,
                    AF = 2,
                    WTSD = 20,
                    FoldTo3Bet = 0,
                    CBet = 33,
                    WWSF = 60,
                    AGG = 28.57m,
                    Steal = 45.45m,
                    TotalHands = 33
                },
                "Fish"
            ).SetName("Player Profile: Fish (not meet req.)");

            yield return new TestCaseData(
                "DH: 6-max",
                new PlayerProfileTestData
                {
                    VPIP = 15m,
                    PFR = 14m,
                    S3Bet = 3m
                },
                "Nit"
            ).SetName("Player Profile: Nit (meet req.)");

            yield return new TestCaseData(
                "DH: 6-max",
                new PlayerProfileTestData
                {
                    VPIP = 27.8m,
                    PFR = 16.7m,
                    AGG = 16.7m,
                    S3Bet = 50m
                },
                "Standard Reg"
            ).SetName("Player Profile: Standard Reg (not meet req.)");

            yield return new TestCaseData(
                "DH: 6-max",
                new PlayerProfileTestData
                {
                    VPIP = 19m,
                    PFR = 19m,
                    AGG = 45m,
                    S3Bet = 5m
                },
                "book"
            ).SetName("Player Profile: Tight Reg (not meet req.)");

            yield return new TestCaseData(
             "DH: 9-max Cash",
             new PlayerProfileTestData
             {
                 VPIP = 27m,
                 PFR = 2m,
                 AGG = 20m,
                 S3Bet = 1.3m,
                 CBet = 82m,
                 TotalHands = 47
             },
             "Fish"
         ).SetName("Player Profile: Fish (not meet req.)");
        }

        #region Infrastructure

        private HudElementViewModel CreateHudElement(HudLayoutInfoV2 layout, EnumGameType enumGameType)
        {
            var hudElementCreator = new HudElementViewModelCreator();

            var hudElementCreationInfo = new HudElementViewModelCreationInfo
            {
                GameType = enumGameType,
                HudLayoutInfo = layout,
                PokerSite = EnumPokerSites.Ignition,
                SeatNumber = 1
            };

            var hudElement = hudElementCreator.Create(hudElementCreationInfo);
            hudElement.PlayerName = "Player";
            hudElement.PlayerId = 1;

            return hudElement;
        }

        #endregion

        private class HudLayoutsServiceStub : HudLayoutsService
        {
            private Dictionary<string, HudLayoutInfoV2> layouts = new Dictionary<string, HudLayoutInfoV2>();

            protected override void Initialize()
            {
                HudLayoutMappings = new HudLayoutMappings();

                var predefinedMappings = GetPredefinedMappings();

                foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
                {
                    foreach (var predefinedPostfix in PredefinedLayoutPostfixes)
                    {
                        var defaultLayoutInfo = GetPredefinedLayout(tableType, predefinedPostfix);

                        if (defaultLayoutInfo == null)
                        {
                            continue;
                        }

                        var predefinedMapping = predefinedMappings.Mappings.FirstOrDefault(x => x.TableType == tableType &&
                                                x.Name == defaultLayoutInfo.Name);

                        if (predefinedMapping != null)
                        {
                            HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                            {
                                TableType = tableType,
                                Name = predefinedMapping.Name,
                                IsDefault = predefinedMapping.IsDefault,
                                PokerSite = predefinedMapping.PokerSite,
                                GameType = predefinedMapping.GameType,
                                IsSelected = predefinedMapping.IsSelected
                            });
                        }
                        else
                        {
                            HudLayoutMappings.Mappings.Add(new HudLayoutMapping
                            {
                                TableType = tableType,
                                Name = defaultLayoutInfo.Name,
                                IsDefault = defaultLayoutInfo.IsDefault
                            });
                        }

                        layouts.Add(defaultLayoutInfo.Name, defaultLayoutInfo);
                    }
                }
            }

            public override HudLayoutInfoV2 GetLayout(string name)
            {
                layouts.TryGetValue(name, out HudLayoutInfoV2 layout);
                return layout;
            }

            public override HudLayoutInfoV2 Import(string path)
            {
                path = Path.Combine(HudLayoutTestDataFolder, path);

                FileAssert.Exists(path);

                return base.Import(path);
            }

            public override void SaveLayoutMappings()
            {
                // do nothing
            }

            protected override string InternalSave(HudLayoutInfoV2 hudLayoutInfo)
            {
                layouts.Add(hudLayoutInfo.Name, hudLayoutInfo);
                return GetLayoutFileName(hudLayoutInfo.Name);
            }
        }

        public class BumperSticketTestData
        {
            public decimal VPIP { get; set; }

            public decimal PFR { get; set; }

            public decimal S3Bet { get; set; }

            public decimal AGG { get; set; }

            public decimal CBet { get; set; }

            public decimal WWSF { get; set; }

            public decimal WTSD { get; set; }

            public decimal FoldTo3Bet { get; set; }

            public decimal DoubleBarrel { get; set; }

            public decimal CheckRaise { get; set; }

            public decimal UO_PFR_EP { get; set; }
        }

        public class PlayerProfileTestData
        {
            public decimal VPIP { get; set; }

            public decimal PFR { get; set; }

            public decimal AGG { get; set; }

            public decimal S3Bet { get; set; }

            public decimal AF { get; set; }

            public decimal CBet { get; set; }

            public decimal FoldToCBet { get; set; }

            public decimal FoldTo3Bet { get; set; }

            public decimal WWSF { get; set; }

            public decimal WTSD { get; set; }

            public decimal Steal { get; set; }

            public decimal DonkBet { get; set; }

            public decimal TotalHands { get; set; } = 120;
        }

        private class PlayerstatisticTest : Playerstatistic
        {
            private int didDoubleBarrel;

            public override int DidDoubleBarrel => didDoubleBarrel;

            public void SetDidDoubleBarrel(int didDoubleBarrel)
            {
                this.didDoubleBarrel = didDoubleBarrel;
            }

            private int couldDoubleBarrel;

            public override int CouldDoubleBarrel => couldDoubleBarrel;

            public void SetCouldDoubleBarrel(int couldDoubleBarrel)
            {
                this.couldDoubleBarrel = couldDoubleBarrel;
            }
        }
    }
}