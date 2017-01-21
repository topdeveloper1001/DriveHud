using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public enum EnumTelerikRadChartFunctionType
    {
        ROI, ITM, MoneyWon, Luck, ThreeBet, PFR, BB
    }


    public enum EnumTelerikRadChartDisplayRange
    {
        Day,
        Week,
        Month,
        Year
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
        OpponentAnalysis
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
        SEK = 5
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
        Raw
    }
}