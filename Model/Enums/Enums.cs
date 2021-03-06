﻿//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;

namespace Model.Enums
{
    public enum EnumViewModelType
    {
        NotDefined,
        BaseViewModel,
        MainWindowViewModel,
        HudViewModel,
        DashboardViewModel,
        TournamentViewModel,
        AppsViewModel,
        FilterBoardTextureViewModel,
        FilterHandActionViewModel,
        FilterHandValueViewModel,
        FilterHoleCardsViewModel,
        FilterQuickViewModel,
        FilterStandardViewModel,
        FilteDateViewModel,
        FilterOmahaHandGridViewModel,
        FilterHandGridViewModel,
        SettingsSupportViewModel,
        SettingsCurrencyViewModel,
        SettingsRakeBackViewModel,
        SettingsUpgradeViewModel,
        FilterAdvancedViewModel
    }

    public enum EnumPubSubMessageType
    {
        PopupInteractionRequest,
    }

    public enum EnumPopupInteractionRequestMode
    {
        StatInfoAppearance,
        StatInfoPlayerType,
    }

    [ProtoContract]
    public enum EnumStatInfoValueRangeType
    {
        [ProtoEnum]
        MoreThan,
        [ProtoEnum]
        Between,
        [ProtoEnum]
        LessThan,
    }

    public enum EnumTelerikRadChartSeriesType
    {
        Line,
        Spline,
        Area,
        Spline_Area,
        Step_Line,
        Step_Area,
    }

    public enum ChartTournamentSeriesType
    {
        ROI,
        ITM,
        MoneyWon,
        Luck,
        EV
    }

    public enum ChartCashSeriesWinningType
    {
        Netwon,
        Showdown,
        NonShowdown,
        EV
    }

    public enum ChartCashSeriesValueType
    {
        Currency,
        BB
    }

    public enum ChartTournamentSeriesValueType
    {
        None,
        Currency,
        Chips,
        BB
    }

    public enum CashChartType
    {
        MoneyWon = 0,
        BB100 = 1
    }

    public enum ChartDisplayRange
    {
        None,
        Week,
        Month,
        Year,
        Hands
    }

    public enum TournamentChartFilterType
    {
        All,
        STT,
        MTT
    }

    public enum EnumDashBoardScreen
    {
        Dashboard = 0,
        Tournament = 1,
        HUD = 2
    }

    public enum EnumReports
    {
        None,
        OverAll,
        Position,
        HoleCards,
        Time,
        Stake,
        Session,
        TournamentResults,
        Tournaments,
        TournamentStats,
        TournamentStackSizes,
        TournamentPosition,
        TournamentHoleCards,
        TournamentPokerSite,
        PokerSite,
        TournamentShowdownHands,
        ShowdownHands,
        OpponentAnalysis,
        PopulationAnalysis
    }

    public enum EnumReplayerTableType : byte
    {
        Six = 6,
        Nine = 9,
        Ten = 10
    }

    public enum EnumCurrency
    {
        USD = 0,
        EUR = 1,
        CNY = 2,
        RUB = 3,
        CAN = 4,
        SEK = 5,
        INR = 6
    }

    public enum EnumChipColor
    {
        Black,
        Orange,
        Blue,
        Green,
        Grey,
        Purple,
        Red,
        Yellow
    }

    public enum EnumExportType
    {
        TwoPlusTwo,
        CardsChat,
        PokerStrategy,
        Raw,
        PlainText
    }
}