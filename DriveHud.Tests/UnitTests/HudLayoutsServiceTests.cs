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
using System.Linq;


namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class HudLayoutsServiceTests
    {
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
            hudElement.SetStatValue(Stat.AF, data.AF);
            hudElement.SetStatValue(Stat.WTSD, data.WTSD);
            hudElement.SetStatValue(Stat.FoldToCBet, data.FoldToCBet);
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
                ).SetName("Player Profile: Std Reg if not meet req.");
        }

        //[Test]
        //public void TestSetStickerODAndWTYAreSet()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    var stat = new Playerstatistic();

        //    stat.Flopcontinuationbetmade = 60;
        //    stat.Flopcontinuationbetpossible = 100;

        //    stat.Turncontinuationbetmade = 20;
        //    stat.Turncontinuationbetpossible = 100;

        //    stat.UO_PFR_EP = 21;
        //    stat.PositionUnoppened = new PositionalStat() { EP = 100 };

        //    stat.Totalhands = 20;

        //    hudElements[0].SetStatValue(Stat.CBet, nameof(Indicators.FlopCBet), 60);
        //    hudElements[0].SetStatValue(Stat.DoubleBarrel, nameof(Indicators.TurnCBet), 20);
        //    hudElements[0].SetStatValue(Stat.UO_PFR_EP, nameof(Indicators.UO_PFR_EP), 21);

        //    hudLayoutService.SetStickers(hudElements.FirstOrDefault(), new Dictionary<string, Playerstatistic>() { { "One and Done", stat }, { "Way Too Early", stat } }, LayoutName);

        //    Assert.That(hudElements[0].Stickers[0].Label, Is.EqualTo("OD"));
        //    Assert.That(hudElements[0].Stickers[1].Label, Is.EqualTo("WTE"));
        //}

        //[Test]
        //public void TestSetPlayerTypeIconNitIsSet()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    hudElements[0].SetStatValue(Stat.VPIP, 15);
        //    hudElements[0].SetStatValue(Stat.PFR, 14);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 3);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Nit.png"));
        //}

        //[Test]
        //public void TestSetPlayerTypeIconBothPlayersAreNit()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    hudElements[0].SetStatValue(Stat.VPIP, 15);
        //    hudElements[0].SetStatValue(Stat.PFR, 14);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 3);

        //    hudElements[1].SetStatValue(Stat.VPIP, 9);
        //    hudElements[1].SetStatValue(Stat.PFR, 8);
        //    hudElements[1].SetStatValue(Stat.S3Bet, 2);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Nit.png"));
        //    Assert.That(hudElements[1].PlayerIcon, Is.EqualTo("Nit.png"));
        //}

        //[Test]
        //public void TestSetPlayerTypeExtraRatioIconStandardReg()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    hudElements[0].SetStatValue(Stat.VPIP, 27.8m);
        //    hudElements[0].SetStatValue(Stat.PFR, 16.7m);
        //    hudElements[0].SetStatValue(Stat.AGG, 16.7m);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 50m);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Standard Reg.png"));
        //}

        //[Test]
        //public void TestSetPlayerTypeIconStandardAndTightAreSet()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    hudElements[0].SetStatValue(Stat.VPIP, 23);
        //    hudElements[0].SetStatValue(Stat.PFR, 18);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 5);
        //    hudElements[0].SetStatValue(Stat.AGG, 43);

        //    hudElements[1].SetStatValue(Stat.VPIP, 19);
        //    hudElements[1].SetStatValue(Stat.PFR, 19);
        //    hudElements[1].SetStatValue(Stat.S3Bet, 5);
        //    hudElements[1].SetStatValue(Stat.AGG, 45);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Standard Reg.png"));
        //    Assert.That(hudElements[1].PlayerIconToolTip, Is.EqualTo("Tight Reg"));
        //}

        //[Test]
        //public void TestSetPlayerTypeIconWhenEnablePlayerProfileIsFalse()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    var hudSavedLayout = hudLayoutService.Layouts.Layouts.FirstOrDefault(x => x.LayoutId == LayoutId);

        //    hudSavedLayout.HudPlayerTypes.ForEach(x => x.EnablePlayerProfile = false);

        //    hudElements[0].SetStatValue(Stat.VPIP, 15);
        //    hudElements[0].SetStatValue(Stat.PFR, 14);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 3);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.Null);
        //    Assert.That(hudElements[0].PlayerIconToolTip, Is.Null);
        //}

        //[Test]
        //public void TestSetPlayerTypeIconWhenDisplayPlayerIconIsFalse()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    var hudSavedLayout = hudLayoutService.Layouts.Layouts.FirstOrDefault(x => x.LayoutId == LayoutId);

        //    var playerProfileType = hudSavedLayout.HudPlayerTypes.FirstOrDefault(x => x.Name.Equals("Nit"));

        //    playerProfileType.DisplayPlayerIcon = false;

        //    hudElements[0].SetStatValue(Stat.VPIP, 15);
        //    hudElements[0].SetStatValue(Stat.PFR, 14);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 3);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.Null);
        //    Assert.That(hudElements[0].PlayerIconToolTip, Is.EqualTo("Nit"));
        //}

        //[Test]
        //public void TestSetPlayerTypeIconWhenTableIs2Max()
        //{
        //    var hudLayoutService = CreateHudLayoutsService(EnumTableType.HU);
        //    var hudElements = CreateHudElement();

        //    var hudSavedLayout = hudLayoutService.Layouts.Layouts.FirstOrDefault(x => x.LayoutId == LayoutId);

        //    var playerProfileType = hudSavedLayout.HudPlayerTypes.FirstOrDefault(x => x.Name.Equals("Nit"));

        //    playerProfileType.DisplayPlayerIcon = false;

        //    hudElements[0].SetStatValue(Stat.VPIP, 15);
        //    hudElements[0].SetStatValue(Stat.PFR, 14);
        //    hudElements[0].SetStatValue(Stat.S3Bet, 3);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.Null);
        //    Assert.That(hudElements[0].PlayerIconToolTip, Is.EqualTo("Nit"));
        //}

        //[Test]
        //public void TestSetPlayerTypeIconNutballIsSet()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    hudElements[0].SetStatValue(Stat.VPIP, 41);
        //    hudElements[0].SetStatValue(Stat.PFR, 23);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Nutball.png"));
        //}

        //[Test]
        //public void TestSetPlayerIconNitIfNoneMatchRequirements()
        //{
        //    var hudLayoutService = CreateHudLayoutsService();
        //    var hudElements = CreateHudElement();

        //    hudElements[0].SetStatValue(Stat.VPIP, 15);
        //    hudElements[0].SetStatValue(Stat.PFR, 14);

        //    hudElements[1].SetStatValue(Stat.VPIP, 9);
        //    hudElements[1].SetStatValue(Stat.PFR, 8);

        //    hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

        //    Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Fish.png"));
        //    Assert.That(hudElements[1].PlayerIcon, Is.EqualTo("Nit.png"));
        //}

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