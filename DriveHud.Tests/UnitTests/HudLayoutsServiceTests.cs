//-----------------------------------------------------------------------
// <copyright file="HudLayoutServiceTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Reflection;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model.Data;
using Model.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DriveHud.Tests.UnitTests
{
    //[TestFixture]
    //public class HudLayoutsServiceTests
    //{
    //    private const string LayoutName = "default";

    //    [Test]
    //    public void TestSetStickerOneAndDoneIsSet()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();
    //        var stat = new Playerstatistic();

    //        stat.Flopcontinuationbetmade = 60;
    //        stat.Flopcontinuationbetpossible = 100;

    //        stat.Turncontinuationbetmade = 20;
    //        stat.Turncontinuationbetpossible = 100;

    //        stat.Totalhands = 20;

    //        hudElements[0].SetStatValue(Stat.CBet, nameof(Indicators.FlopCBet), 90);
    //        hudElements[0].SetStatValue(Stat.DoubleBarrel, nameof(Indicators.TurnCBet), 90);

    //        hudLayoutService.SetStickers(hudElements.FirstOrDefault(), new Dictionary<string, Playerstatistic>() { { "One and Done", stat } }, LayoutName);

    //        Assert.That(hudElements[0].Stickers[0].Label, Is.EqualTo("OD"));
    //    }

    //    [Test]
    //    public void TestSetStickerODAndWTYAreSet()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        var stat = new Playerstatistic();

    //        stat.Flopcontinuationbetmade = 60;
    //        stat.Flopcontinuationbetpossible = 100;

    //        stat.Turncontinuationbetmade = 20;
    //        stat.Turncontinuationbetpossible = 100;

    //        stat.UO_PFR_EP = 21;
    //        stat.PositionUnoppened = new PositionalStat() { EP = 100 };

    //        stat.Totalhands = 20;

    //        hudElements[0].SetStatValue(Stat.CBet, nameof(Indicators.FlopCBet), 60);
    //        hudElements[0].SetStatValue(Stat.DoubleBarrel, nameof(Indicators.TurnCBet), 20);
    //        hudElements[0].SetStatValue(Stat.UO_PFR_EP, nameof(Indicators.UO_PFR_EP), 21);

    //        hudLayoutService.SetStickers(hudElements.FirstOrDefault(), new Dictionary<string, Playerstatistic>() { { "One and Done", stat }, { "Way Too Early", stat } }, LayoutName);

    //        Assert.That(hudElements[0].Stickers[0].Label, Is.EqualTo("OD"));
    //        Assert.That(hudElements[0].Stickers[1].Label, Is.EqualTo("WTE"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconNitIsSet()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        hudElements[0].SetStatValue(Stat.VPIP, 15);
    //        hudElements[0].SetStatValue(Stat.PFR, 14);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 3);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Nit.png"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconBothPlayersAreNit()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        hudElements[0].SetStatValue(Stat.VPIP, 15);
    //        hudElements[0].SetStatValue(Stat.PFR, 14);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 3);

    //        hudElements[1].SetStatValue(Stat.VPIP, 9);
    //        hudElements[1].SetStatValue(Stat.PFR, 8);
    //        hudElements[1].SetStatValue(Stat.S3Bet, 2);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Nit.png"));
    //        Assert.That(hudElements[1].PlayerIcon, Is.EqualTo("Nit.png"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeExtraRatioIconStandardReg()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        hudElements[0].SetStatValue(Stat.VPIP, 27.8m);
    //        hudElements[0].SetStatValue(Stat.PFR, 16.7m);
    //        hudElements[0].SetStatValue(Stat.AGG, 16.7m);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 50m);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Standard Reg.png"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconStandardAndTightAreSet()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        hudElements[0].SetStatValue(Stat.VPIP, 23);
    //        hudElements[0].SetStatValue(Stat.PFR, 18);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 5);
    //        hudElements[0].SetStatValue(Stat.AGG, 43);

    //        hudElements[1].SetStatValue(Stat.VPIP, 19);
    //        hudElements[1].SetStatValue(Stat.PFR, 19);
    //        hudElements[1].SetStatValue(Stat.S3Bet, 5);
    //        hudElements[1].SetStatValue(Stat.AGG, 45);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Standard Reg.png"));
    //        Assert.That(hudElements[1].PlayerIconToolTip, Is.EqualTo("Tight Reg"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconWhenEnablePlayerProfileIsFalse()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        var hudSavedLayout = hudLayoutService.Layouts.Layouts.FirstOrDefault(x => x.LayoutId == LayoutId);

    //        hudSavedLayout.HudPlayerTypes.ForEach(x => x.EnablePlayerProfile = false);

    //        hudElements[0].SetStatValue(Stat.VPIP, 15);
    //        hudElements[0].SetStatValue(Stat.PFR, 14);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 3);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.Null);
    //        Assert.That(hudElements[0].PlayerIconToolTip, Is.Null);
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconWhenDisplayPlayerIconIsFalse()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        var hudSavedLayout = hudLayoutService.Layouts.Layouts.FirstOrDefault(x => x.LayoutId == LayoutId);

    //        var playerProfileType = hudSavedLayout.HudPlayerTypes.FirstOrDefault(x => x.Name.Equals("Nit"));

    //        playerProfileType.DisplayPlayerIcon = false;

    //        hudElements[0].SetStatValue(Stat.VPIP, 15);
    //        hudElements[0].SetStatValue(Stat.PFR, 14);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 3);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.Null);
    //        Assert.That(hudElements[0].PlayerIconToolTip, Is.EqualTo("Nit"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconWhenTableIs2Max()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService(EnumTableType.HU);
    //        var hudElements = CreateHudElements();

    //        var hudSavedLayout = hudLayoutService.Layouts.Layouts.FirstOrDefault(x => x.LayoutId == LayoutId);

    //        var playerProfileType = hudSavedLayout.HudPlayerTypes.FirstOrDefault(x => x.Name.Equals("Nit"));

    //        playerProfileType.DisplayPlayerIcon = false;

    //        hudElements[0].SetStatValue(Stat.VPIP, 15);
    //        hudElements[0].SetStatValue(Stat.PFR, 14);
    //        hudElements[0].SetStatValue(Stat.S3Bet, 3);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.Null);
    //        Assert.That(hudElements[0].PlayerIconToolTip, Is.EqualTo("Nit"));
    //    }

    //    [Test]
    //    public void TestSetPlayerTypeIconNutballIsSet()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        hudElements[0].SetStatValue(Stat.VPIP, 41);
    //        hudElements[0].SetStatValue(Stat.PFR, 23);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Nutball.png"));
    //    }

    //    [Test]
    //    public void TestSetPlayerIconNitIfNoneMatchRequirements()
    //    {
    //        var hudLayoutService = CreateHudLayoutsService();
    //        var hudElements = CreateHudElements();

    //        hudElements[0].SetStatValue(Stat.VPIP, 15);
    //        hudElements[0].SetStatValue(Stat.PFR, 14);

    //        hudElements[1].SetStatValue(Stat.VPIP, 9);
    //        hudElements[1].SetStatValue(Stat.PFR, 8);

    //        hudLayoutService.SetPlayerTypeIcon(hudElements, LayoutName);

    //        Assert.That(hudElements[0].PlayerIcon, Is.EqualTo("Fish.png"));
    //        Assert.That(hudElements[1].PlayerIcon, Is.EqualTo("Nit.png"));
    //    }

    //    #region Infrastructure

    //    private HudLayoutsService CreateHudLayoutsService(EnumTableType tableType = EnumTableType.Six)
    //    {
    //        var hudLayoutService = new HudLayoutsServiceStub();

    //        var hudTables = new Dictionary<int, HudTableViewModel>();
    //        //  hudTables.Add(LayoutId, new HudTableViewModel { HudElements = new ObservableCollection<HudElementViewModel>(), TableType = tableType });

    //        hudLayoutService.SaveDefaults(hudTables);

    //        return hudLayoutService;
    //    }

    //    private HudElementViewModel[] CreateHudElements(int max = 6, int totalHands = 25)
    //    {
    //        var hudElements = (from seat in Enumerable.Range(1, max)
    //                           select new HudElementViewModel(GetAllStatInfos())
    //                           {
    //                               Seat = seat,
    //                           }).ToArray();

    //        hudElements.ForEach(x => x.SetStatValue(Stat.TotalHands, totalHands));

    //        return hudElements;
    //    }

    //    private IEnumerable<StatInfo> GetAllStatInfos()
    //    {
    //        var stats = new List<StatInfo>
    //        {
    //            new StatInfo { Stat = Stat.VPIP, Caption = "VPIP%" },
    //            new StatInfo { Stat = Stat.PFR, Caption = "PFR%"},
    //            new StatInfo { Stat = Stat.S3Bet, Caption = "3-bet%" },
    //            new StatInfo { Stat = Stat.AGG, Caption = "AGG%" },
    //            new StatInfo { Stat = Stat.CBet, Caption = "C-bet%"},
    //            new StatInfo { Stat = Stat.WTSD, Caption = "WTSD" },
    //            new StatInfo { Stat = Stat.WSSD, Caption = "W$SD" },
    //            new StatInfo { Stat = Stat.WWSF, Caption = "WWSF" },
    //            new StatInfo { Stat = Stat.TotalHands, Caption = "Total Hands", Format = "{0:0}" },
    //            new StatInfo { Stat = Stat.FoldToCBet, Caption = "Fold to C-Bet%" },
    //            new StatInfo { Stat = Stat.FoldTo3Bet, Caption = "Fold to 3-bet%"},
    //            new StatInfo { Stat = Stat.S4Bet, Caption = "4-bet%" },
    //            new StatInfo { Stat = Stat.FoldTo4Bet, Caption = "Fold to 4-bet%" },
    //            new StatInfo { Stat = Stat.FlopAGG, Caption = "Flop AGG%" },
    //            new StatInfo { Stat = Stat.TurnAGG, Caption = "Turn AGG%" },
    //            new StatInfo { Stat = Stat.RiverAGG, Caption = "River AGG%" },
    //            new StatInfo { Stat = Stat.ColdCall, Caption = "Cold Call%"},
    //            new StatInfo { Stat = Stat.Steal, Caption = "Steal%" },
    //            new StatInfo { Stat = Stat.FoldToSteal, Caption = "Fold to Steal%" },
    //            new StatInfo { Stat = Stat.Squeeze, Caption = "Squeeze%" },
    //            new StatInfo { Stat = Stat.CheckRaise, Caption = "Check-Raise%" },
    //            new StatInfo { Stat = Stat.CBetIP, Caption = "C-Bet IP%" },
    //            new StatInfo { Stat = Stat.CBetOOP, Caption = "C-Bet OOP%" },
    //            new StatInfo { Stat = Stat.S3BetMP, Caption = "3-Bet MP%" },
    //            new StatInfo { Stat = Stat.S3BetCO, Caption = "3-Bet CO%" },
    //            new StatInfo { Stat = Stat.S3BetBTN, Caption = "3-Bet BTN%" },
    //            new StatInfo { Stat = Stat.S3BetSB, Caption = "3-Bet SB%" },
    //            new StatInfo { Stat = Stat.S3BetBB, Caption = "3-Bet BB%" },
    //            new StatInfo { Stat = Stat.S4BetMP, Caption = "4-Bet MP%" },
    //            new StatInfo { Stat = Stat.S4BetCO, Caption = "4-Bet CO%" },
    //            new StatInfo { Stat = Stat.S4BetBTN, Caption = "4-Bet BTN%" },
    //            new StatInfo { Stat = Stat.S4BetSB, Caption = "4-Bet SB%" },
    //            new StatInfo { Stat = Stat.S4BetBB, Caption = "4-Bet BB%" },
    //            new StatInfo { Stat = Stat.ColdCallMP, Caption = "Cold Call MP%" },
    //            new StatInfo { Stat = Stat.ColdCallCO, Caption = "Cold Call CO%" },
    //            new StatInfo { Stat = Stat.ColdCallBTN, Caption = "Cold Call BTN%" },
    //            new StatInfo { Stat = Stat.ColdCallSB, Caption = "Cold Call SB%" },
    //            new StatInfo { Stat = Stat.ColdCallBB, Caption = "Cold Call BB%" },
    //            new StatInfo { Stat = Stat.S3BetIP, Caption = "3-BET IP%" },
    //            new StatInfo { Stat = Stat.S3BetOOP, Caption = "3-BET OOP%" },
    //            new StatInfo { Stat = Stat.DoubleBarrel, Caption = "Double Barrel" },
    //            new StatInfo { Stat = Stat.UO_PFR_EP, Caption = "UO PFR EP" }
    //        };

    //        stats.ForEach(x => x.Initialize());

    //        return stats;
    //    }

    //    #endregion

    //    private class HudLayoutsServiceStub : HudLayoutsService
    //    {
    //        protected override void Initialize()
    //        {
    //            HudLayoutMappings = new HudLayoutMappings();
    //        }

    //        public override string GetImageLink(string image)
    //        {
    //            return image;
    //        }
    //    }
    //}
}