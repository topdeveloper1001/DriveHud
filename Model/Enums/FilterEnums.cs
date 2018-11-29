//-----------------------------------------------------------------------
// <copyright file="FilterEnums.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace Model.Enums
{
    public enum EnumTriState
    {
        Any = 0,
        On = 1,
        Off = 2,
    }

    public enum EnumEquality
    {
        GreaterThan,
        LessThan,
        EqualTo
    }

    public enum EnumFilterDropDown
    {
        FilterCreate = 0,
        FilterToday = 1,
        FilterThisWeek = 2,
        FilterThisMonth = 3,
        FilterLastMonth = 4,
        FilterThisYear = 5,
        FilterCustomDateRange = 6,
        FilterAllStats = 7
    }


    public enum EnumFilterModelType
    {
        FilterBoardTextureModel,
        FilterHandActionModel,
        FilterHandValueModel,
        FilterHoleCards,
        FilterOmahaHandGrid,
        FilterQuickModel,
        FilterStandardModel,
        FilterDateModel,
        FilterHandGridModel,
        FilterAdvancedModel
    }

    public enum EnumFilterSectionItemType
    {
        FilterSectionNone = 0,
        StakeLevel,
        PreFlopAction,
        Currency,
        Stat,
        TableSixRing,
        TableNineRing,
        PlayersBetween,
        HoleCards,
        HandValueFastFilter,
        FlopHandValue,
        TurnHandValue,
        RiverHandValue,
        Date,
        FlopBoardCardItem,
        TurnBoardCardItem,
        RiverBoardCardItem,
        FlopBoardTextureItem,
        TurnBoardTextureItem,
        RiverBoardTextureItem,
        PreflopHandActionItem,
        FlopHandActionItem,
        TurnHandActionItem,
        RiverHandActionItem,
        OmahaHandGridItem,
        QuickFilterItem,
        Buyin
    }

    public struct EnumDateFiterStruct

    {
        public EnumDateFiter EnumDateRange { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public enum EnumDateFiter
        {
            None,
            Custom,
            Today,
            ThisWeek,
            ThisMonth,
            LastMonth,
            ThisYear,
            CustomDateRange
        }
    }

    public enum EnumHandValuesFastFilterType
    {
        TPTK,
        TPGK,
        Overpair,
        SecondPair,
        ThirdPair,
        BottomPair,
        BottomSet,
        AnyFlushDraw,
        AnyStraightDraw,
        LowFlush,
        BottomStraight,
        SetSecondOrLower,
    }

    public enum QuickFilterHandTypeEnum
    {
        None,
        StraightDrawOnFlop,
        FlushDrawOnFlop,
        MarginalHand,
        LessThanMidPairOnRiver,
        FlopTPOrBetter,
        PremiumHand,
        NonPremiumHand,
        BluffRange,
    }

    public enum QuickFilterPositionEnum
    {
        Any, BTN, Blinds, Small, Big, IP, OOP
    }


    public enum EnumFilterType
    {
        Cash,
        Tournament,
    }

    public enum FilterServices
    {
        Main,
        Stickers
    }

}
