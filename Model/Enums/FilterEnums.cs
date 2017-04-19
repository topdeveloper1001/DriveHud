using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

	public struct EnumDateFiterStruct

	{
		public EnumDateFiter EnumDateRange;//{ get; set; }
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
