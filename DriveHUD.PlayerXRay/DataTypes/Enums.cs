using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace DriveHUD.PlayerXRay.DataTypes
{
    public enum RangeCardSuit
    {
        None = -1, Clubs = 0, Diamonds = 1, Hearts = 2, Spades = 3
    }

    //old ranking system
    public enum RangeCardRank
    {
        None = -1, Two = 0, Three = 1, Four = 2, Five = 3, Six = 4, Seven = 5, Eight = 6,
        Nine = 7, Ten = 8, Jack = 9, Queen = 10, King = 11, Ace = 12
    }

    //new ranking system
    public enum CardRank
    {
        None = -1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8,
        Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14
    }

    public enum RangeSelectorItemType
    {
        Default,
        Pair,
        Suited,
        OffSuited
    }

    #region Suit/Rank To String extensions

    public static class RangeCardEnumExtensions
    {
        public static string ToSuitString(this RangeCardSuit suit)
        {
            switch (suit)
            {
                case RangeCardSuit.Clubs:
                    return "c";
                case RangeCardSuit.Diamonds:
                    return "d";
                case RangeCardSuit.Hearts:
                    return "h";
                case RangeCardSuit.Spades:
                    return "s";
                default:
                    return "x";
            }
        }

        public static RangeCardSuit StringToSuit(this RangeCardSuit suit, string value)
        {
            RangeCardSuit result = RangeCardSuit.None;
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "c":
                        result = RangeCardSuit.Clubs;
                        break;
                    case "d":
                        result = RangeCardSuit.Diamonds;
                        break;
                    case "h":
                        result = RangeCardSuit.Hearts;
                        break;
                    case "s":
                        result = RangeCardSuit.Spades;
                        break;
                }
            }
            return result;
        }

        public static string ToRankString(this RangeCardRank rank)
        {
            switch (rank)
            {
                case RangeCardRank.Two:
                    return "2";
                case RangeCardRank.Three:
                    return "3";
                case RangeCardRank.Four:
                    return "4";
                case RangeCardRank.Five:
                    return "5";
                case RangeCardRank.Six:
                    return "6";
                case RangeCardRank.Seven:
                    return "7";
                case RangeCardRank.Eight:
                    return "8";
                case RangeCardRank.Nine:
                    return "9";
                case RangeCardRank.Ten:
                    return "T";
                case RangeCardRank.Jack:
                    return "J";
                case RangeCardRank.Queen:
                    return "Q";
                case RangeCardRank.King:
                    return "K";
                case RangeCardRank.Ace:
                    return "A";
                case RangeCardRank.None:
                default:
                    return "X";
            }
        }

        public static RangeCardRank StringToRank(this RangeCardRank rank1, string value)
        {
            RangeCardRank result = RangeCardRank.None;
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "2":
                        result = RangeCardRank.Two;
                        break;
                    case "3":
                        result = RangeCardRank.Three;
                        break;
                    case "4":
                        result = RangeCardRank.Four;
                        break;
                    case "5":
                        result = RangeCardRank.Five;
                        break;
                    case "6":
                        result = RangeCardRank.Six;
                        break;
                    case "7":
                        result = RangeCardRank.Seven;
                        break;
                    case "8":
                        result = RangeCardRank.Eight;
                        break;
                    case "9":
                        result = RangeCardRank.Nine;
                        break;
                    case "T":
                        result = RangeCardRank.Ten;
                        break;
                    case "J":
                        result = RangeCardRank.Jack;
                        break;
                    case "Q":
                        result = RangeCardRank.Queen;
                        break;
                    case "K":
                        result = RangeCardRank.King;
                        break;
                    case "A":
                        result = RangeCardRank.Ace;
                        break;
                }
            }
            return result;
        }
    }

    #endregion

    public static class RangeSelectorItemTypeEnumExtensions
    {
        public static string ToRangeSelectorItemString(this RangeSelectorItemType value)
        {
            switch (value)
            {
                case RangeSelectorItemType.OffSuited:
                    return "o";
                case RangeSelectorItemType.Suited:
                    return "s";
                case RangeSelectorItemType.Pair:
                    return string.Empty;
                case RangeSelectorItemType.Default:
                default:
                    return string.Empty;
            }
        }

        public static RangeSelectorItemType StringToRangeItemType(this RangeSelectorItemType value, string s)
        {
            if (!string.IsNullOrEmpty(s) && s.Length == 1)
            {
                switch (s)
                {
                    case "o":
                        return RangeSelectorItemType.OffSuited;
                    case "s":
                        return RangeSelectorItemType.Suited;
                }
            }

            return RangeSelectorItemType.Pair;
        }
    }



    public enum GameType : byte
    {
        Unknown = 0,
        [Description("NL")]
        NoLimitHoldem = 1,
        [Description("L")]
        FixedLimitHoldem = 2,
        [Description("PL")]
        PotLimitOmaha = 3,
        [Description("PL")]
        PotLimitHoldem = 4,
        [Description("PL")]
        PotLimitOmahaHiLo = 5,
        [Description("NL")]
        CapNoLimitHoldem = 6,
        [Description("PL")]
        CapPotLimitOmaha = 7,
        [Description("PL")]
        FiveCardPotLimitOmaha = 8,
        [Description("PL")]
        FiveCardPotLimitOmahaHiLo = 9,
        [Description("NL")]
        NoLimitOmaha = 10,
        [Description("NL")]
        NoLimitOmahaHiLo = 11,
        [Description("L")]
        FixedLimitOmaha = 12,
        [Description("L")]
        FixedLimitOmahaHiLo = 13,
        [Description("L")]
        SpreadLimitHoldem = 14,         //todo ask about game type s=description. Filter only holdem games.
        Any = 31,
    }  
    
    public enum ClientType
    {
        [XmlEnum("0")]
        DriveHUD,
        [XmlEnum("1")]
        HoldemManager,
        [XmlEnum("2")]
        PokerTracker
    }

    public enum NoteStageType
    {
        [XmlEnum("0")]
        PreFlop,
        [XmlEnum("1")]
        Flop,
        [XmlEnum("2")]
        Turn,
        [XmlEnum("3")]
        River,
        [XmlEnum("4")]
        Other
    }

    public enum PlayerTypeEnum
    {
        [XmlEnum("0")]
        Tag,
        [XmlEnum("1")]
        Fish,
        [XmlEnum("2")]
        Whale,
        [XmlEnum("3")]
        Gambler,
        [XmlEnum("4")]
        Lag,
        [XmlEnum("5")]
        Rock,
        [XmlEnum("6")]
        Nit
    }

    public enum TableTypeEnum
    {
        [XmlEnum("0")]
        NoLimit,
        [XmlEnum("1")]
        PotLimit,
        [XmlEnum("2")]
        Limit
    }

    public enum TableSizeEnum
    {
        [XmlEnum("2")]
        HeadsUp = 2,
        [XmlEnum("4")]
        Players34 = 4,
        [XmlEnum("6")]
        Players56 = 6,
        [XmlEnum("10")]
        Player710 = 10
    }

    public enum ActionTypeEnum
    {
        [XmlEnum("0")]
        Any,
        [XmlEnum("1")]
        Bet,
        [XmlEnum("2")]
        Check,
        [XmlEnum("3")]
        Call,
        [XmlEnum("4")]
        Raise,
        [XmlEnum("5")]
        Fold
    }

    public enum FilterEnum
    {
        //PREFLOP

        VPIP = 0,
        PutMoneyinPot = 1,
        PFR = 2,
        PFRFalse = 302,
        Did3Bet = 3,
        DidSqueeze = 4,
        DidColdCall = 5,
        CouldColdCall = 303,
        CouldColdCallFalse = 304,
        FacedPreflop3Bet = 6,
        FoldedToPreflop3Bet = 7,
        CalledPreflop3Bet = 8,
        RaisedPreflop3Bet = 9,
        FacedPreflop4Bet = 10,
        FoldedToPreflop4Bet = 11,
        CalledPreflop4Bbet = 12,
        RaisedPreflop4Bet = 13,
        InBBandStealAttempted = 14,
        InBBandStealDefended = 15,
        InBBandStealReraised = 16,
        InSBandStealAttempted = 17,
        InSBandStealDefended = 18,
        InSBandStealReraised = 19,
        LimpReraised = 20,
        BBsBetPreflopisBiggerThan = 21,
        BBsBetPreflopisLessThan = 22,
        BBsCalledPreflopisBiggerThan = 23,
        BBsCalledPreflopisLessThan = 24,
        BBsPutInPreflopisBiggerThan = 25,
        BBsPutInPreflopisLessThan = 26,
        PreflopRaiseSizePotisBiggerThan = 27,
        PreflopRaiseSizePotisLessThan = 28,
        PreflopFacingRaiseSizePotisBiggerThan = 29,
        PreflopFacingRaiseSizePotisLessThan = 30,
        AllinPreflop = 31,

        //FLOP

        SawFlop = 32,
        SawFlopFalse = 308,
        LasttoActionFlop = 33,
        LasttoActionFlopFalse = 305,
        FlopUnopened = 34,
        PlayersonFlopisBiggerThan = 35,
        PlayersonFlopisLessThan = 36,
        PlayersonFlopisEqualTo = 37,
        FlopContinuationBetPossible = 38,
        FlopContinuationBetMade = 39,
        FacingFlopContinuationBet = 40,
        FoldedtoFlopContinuationBet = 41,
        CalledFlopContinuationBet = 42,
        RaisedFlopContinuationBet = 43,
        FlopBet = 44,
        FlopBetFold = 45,
        FlopBetCall = 46,
        FlopBetRaise = 47,
        FlopRaise = 48,
        FlopRaiseFold = 49,
        FlopRaiseCall = 50,
        FlopRaiseRaise = 51,
        FlopCall = 52,
        FlopCallFold = 53,
        FlopCallCall = 54,
        FlopCallRaise = 55,
        FlopCheck = 56,
        FlopCheckFold = 57,
        FlopCheckCall = 58,
        FlopCheckRaise = 59,
        FlopFold = 60,
        FlopWasCheckRaised = 61,
        FlopWasBetInto = 62,
        FlopWasRaised = 63,
        BBsBetFlopisBiggerThan = 64,
        BBsBetFlopisLessThan = 65,
        BBsCalledFlopisBiggerThan = 66,
        BBsCalledFlopisLessThan = 67,
        BBsPutinFlopisBiggerThan = 68,
        BBsPutinFlopisLessThan = 69,
        FlopPotSizeinBBsisBiggerThan = 70,
        FlopPotSizeinBBsisLessThan = 71,
        FlopStackPotRatioisBiggerThan = 72,
        FlopStackPotRatioisLessThan = 73,
        FlopBetSizePotisBiggerThan = 74,
        FlopBetSizePotisLessThan = 75,
        FlopRaiseSizePotisBiggerThan = 76,
        FlopRaiseSizePotisLessThan = 77,
        FlopFacingBetSizePotisBiggerThan = 78,
        FlopFacingBetSizePotisLessThan = 79,
        FlopFacingRaiseSizePotisBiggerThan = 80,
        FlopFacingRaiseSizePotisLessThan = 81,
        AllinOnFlop = 82,
        AllinOnFlopOrEarlier = 83,

        //TURN

        SawTurn = 84,
        LasttoActonTurn = 85,
        LasttoActonTurnFalse = 306,
        TurnUnopened = 86,
        TurnUnopenedFalse = 300,
        PlayersonTurnisBiggerThan = 87,
        PlayersonTurnisLessThan = 88,
        PlayersonTurnisEqualTo = 89,
        TurnContinuationBetPossible = 90,
        TurnContinuationBetMade = 91,
        FacingTurnContinuationBet = 92,
        FoldedtoTurnContinuationBet = 93,
        CalledTurnContinuationBet = 94,
        RaisedTurnContinuationBet = 95,
        TurnBet = 96,
        TurnBetFold = 97,
        TurnBetCall = 98,
        TurnBetRaise = 99,
        TurnRaise = 100,
        TurnRaiseFold = 101,
        TurnRaiseCall = 102,
        TurnRaiseRaise = 103,
        TurnCall = 104,
        TurnCallFold = 105,
        TurnCallCall = 106,
        TurnCallRaise = 107,
        TurnCheck = 108,
        TurnCheckFold = 109,
        TurnCheckCall = 110,
        TurnCheckRaise = 111,
        TurnFold = 112,
        TurnWasCheckRaised = 113,
        TurnWasBetInto = 114,
        TurnWasRaised = 115,
        BBsBetTurnisBiggerThan = 116,
        BBsBetTurnisLessThan = 117,
        BBsCalledTurnisBiggerThan = 118,
        BBsCalledTurnisLessThan = 119,
        BBsPutinTurnisBiggerThan = 120,
        BBsPutinTurnisLessThan = 121,
        TurnPotSizeinBBsisBiggerThan = 123,
        TurnPotSizeinBBsisLessThan = 124,
        TurnStackPotRatioisBiggerThan = 125,
        TurnStackPotRatioisLessThan = 126,
        TurnBetSizePotisBiggerThan = 127,
        TurnBetSizePotisLessThan = 128,
        TurnRaiseSizePotisBiggerThan = 129,
        TurnRaiseSizePotisLessThan = 130,
        TurnFacingBetSizePotisBiggerThan = 131,
        TurnFacingBetSizePotisLessThan = 132,
        TurnFacingRaiseSizePotisBiggerThan = 133,
        TurnFacingRaiseSizePotisLessThan = 134,
        AllinonTurn = 135,
        AllinonTurnOrEarlier = 136,

        //RIVER

        SawRiver = 137,
        LasttoActonRiver = 138,
        LasttoActonRiverFalse = 307,
        RiverUnopened = 139,
        RiverUnopenedFalse = 301,
        PlayersonRiverisBiggerThan = 140,
        PlayersonRiverisLessThan = 141,
        PlayersonRiverisEqualTo = 142,
        RiverContinuationBetPossible = 143,
        RiverContinuationBetMade = 144,
        FacingRiverContinuationBet = 145,
        FoldedtoRiverContinuationBet = 146,
        CalledRiverContinuationBet = 147,
        RaisedRiverContinuationBet = 148,
        RiverBet = 149,
        RiverBetFold = 150,
        RiverBetCall = 151,
        RiverBetRaise = 152,
        RiverRaise = 153,
        RiverRaiseFold = 154,
        RiverRaiseCall = 155,
        RiverRaiseRaise = 156,
        RiverCall = 157,
        RiverCallFold = 158,
        RiverCallCall = 159,
        RiverCallRaise = 160,
        RiverCheck = 161,
        RiverCheckFold = 162,
        RiverCheckCall = 163,
        RiverCheckRaise = 164,
        RiverFold = 165,
        RiverWasCheckRaised = 166,
        RiverWasBetInto = 167,
        RiverWasRaised = 168,
        BBsBetRiverisBiggerThan = 169,
        BBsBetRiverisLessThan = 170,
        BBsCalledRiverisBiggerThan = 171,
        BBsCalledRiverisLessThan = 172,
        BBsPutinRiverisBiggerThan = 173,
        BBsPutinRiverisLessThan = 174,
        RiverPotSizeinBBsisBiggerThan = 175,
        RiverPotSizeinBBsisLessThan = 176,
        RiverStackPotRatioisBiggerThan = 177,
        RiverStackPotRatioisLessThan = 178,
        RiverBetSizePotisBiggerThan = 179,
        RiverBetSizePotisLessThan = 180,
        RiverRaiseSizePotisBiggerThan = 181,
        RiverRaiseSizePotisLessThan = 182,
        RiverFacingBetSizePotisBiggerThan = 183,
        RiverFacingBetSizePotisLessThan = 184,
        RiverFacingRaiseSizePotisBiggerThan = 185,
        RiverFacingRaiseSizePotisLessThan = 186,

        //OTHER

        SawShowdown = 187,
        WonHand = 188,
        FinalPotSizeinBBsisBiggerThan = 189,
        FinalPotSizeinBBsisLessThan = 190,
        PlayerWonBBsIsBiggerThan = 191,
        PlayerWonBBsIsLessThan = 192,
        PlayerLostBBsIsBiggerThan = 193,
        PlayerLostBBsIsLessThan = 194,
        PlayerWonOrLostBBsIsBiggerThan = 195,
        PlayerWonOrLostBBsIsLessThan = 196,
        PlayersSawShowdownIsBiggerThan = 197,
        PlayersSawShowdownIsLessThan = 198,
        PlayersSawShowdownIsEqualTo = 199,
        AllinWinIsBiggerThan = 200,
        AllinWinIsLessThan = 201
    }

    public enum HandValueEnum
    {
        HighCardTwoOvercards = 10,
        HighCardOneOvercard = 11,
        HighCardNoOvercards = 0,
        OnePairPocketPairOverpair = 20,
        OnePairPocketPairSecondPair = 21,
        OnePairPocketPairLowPair = 22,
        OnePairTopPairTopKicker = 23,
        OnePairTopPairGoodKicker = 24,
        OnePairTopPairWeakKicker = 25,
        OnePairSecondPairAceKicker = 26,
        OnePairSecondPairNonAceKicker = 27,
        OnePairBottomPairAceKicker = 28,
        OnePairBottomPairNonAceKicker = 29,
        OnePairPairedBoardTwoOvercards = 30,
        OnePairPairedBoardOneOvercard = 31,
        OnePairPairedBoardNoOvercards = 32,
        TwoPairBothCardsPairedTopTwoPair = 40,
        TwoPairBothCardsPairedTopPairPlusPair = 41,
        TwoPairBothCardsPairedMiddlePlusBottom = 42,
        TwoPairPairPlusPairedBoardTopPair = 43,
        TwoPairPairPlusPairedBoardSecondPair = 44,
        TwoPairPairPlusPairedBoardBottomPair = 45,
        TwoPairPocketPairPlusPairedBoardOverpair = 46,
        TwoPairPocketPairPlusPairedBoardGreaterPairOnBoard = 47,
        TwoPairPocketPairPlusPairedBoardLowerPairOnBoard = 48,
        TwoPaironBoard = 49,
        ThreeofaKindSetHighSet = 60,
        ThreeofaKindSetSecondSet = 61,
        ThreeofaKindSetLowSet = 62,
        ThreeofaKindTripsHighTripsHighKicker = 63,
        ThreeofaKindTripsHighTripsLowKicker = 64,
        ThreeofaKindTripsSecondTripsHighKicker = 65,
        ThreeofaKindTripsSecondTripsLowKicker = 66,
        ThreeofaKindTripsLowTripsHighKicker = 67,
        ThreeofaKindTripsLowTripsLowKicker = 68,
        TripsOnBoard = 69,
        StraightTwoCardNutStraight = 80,
        StraightTwoCardStraight = 81,
        StraightOneCardNutStraight = 82,
        StraightOneCardStraight = 83,
        StraightStraightOnBoard = 84,
        Flush3FlushCardsNutFlush = 100,
        Flush3FlushCardsHighFlush = 101,
        Flush3FlushCardsLowFlush = 102,
        Flush4FlushCardsNutFlush = 103,
        Flush4FlushCardsHighFlush = 104,
        Flush4FlushCardsLowFlush = 105,
        FlushOnBoardNutFlush = 106,
        FlushOnBoardHighFlush = 107,
        FlushOnBoardLowFlush = 108,
        FullHouse2CardsPocketPairnoTripsonBoard = 120,
        FullHouse2CardsPocketPairPlusTripsonBoard = 121,
        FullHouse2CardsNoPocketsnoTripsonBoard = 122,
        FullHouseLess2CardsTripsonBoard = 123,
        FullHouseLess2CardsFillTopPairnoTrips = 124,
        FullHouseLess2CardsFillBottomPairnoTrips = 125,
        FullHouseonBoard = 126,
        FourofaKindWithPocketPair = 140,
        FourofaKindWithoutPocketPair = 141,
        FourofaKindonBoard = 142,
        StraightFlush2Cards = 160,
        StraightFlushonBoard = 162
    }

    public enum HandValueFlushDrawEnum
    {
        TwoCardNutFlushDraw = 1,
        TwoCardHighFlushDraw = 2,
        TwoCardLowFlushDraw = 3,
        OneCardNutFlushDraw = 4,
        OneCardHighFlushDraw = 5,
        OneCardLowFlushDraw = 6,
        TwoCardNutBackdoorFlushDraw = 7,
        TwoCardBackdoorFlushDraw = 8,
        OneCardNutBackdoorFlushDraw = 9,
        NoFlushDraw = 0,
    }

    public enum HandValueStraightDraw
    {
        TwoCardOpenEndedStraightDraw = 1,
        TwoCardGutshotDraw = 2,
        OneCardOpenEndedStraightDraw = 3,
        OneCardGutshotDraw = 4,
        TwoCardBackdoorStraightDraw = 5,
        NoStraightDraw = 0
    }

    public enum CompareEnum
    {
        [XmlEnum("0")]
        EqualTo,
        [XmlEnum("1")]
        GreaterThan,
        [XmlEnum("2")]
        LessThan
    }

    public enum FlopFlushCardsEnum
    {
        [XmlEnum("1")]
        Rainbow = 1,
        [XmlEnum("2")]
        TwoOfOneSuit = 2,
        [XmlEnum("3")]
        ThreeOfOneSuit = 3
    }

    public enum TurnFlushCardsEnum
    {
        [XmlEnum("1")]
        Rainbow = 1,
        [XmlEnum("2")]
        TwoOfOneSuit = 2,
        [XmlEnum("0")]
        TwoOfTwoSuits = 0,
        [XmlEnum("3")]
        ThreeOfOneSuit = 3,
        [XmlEnum("4")]
        FourOfOneSuit = 4
    }

    public enum RiverFlushCardsEnum
    {
        [XmlEnum("0")]
        NoFlush,
        [XmlEnum("3")]
        ThreeCardsOneSuit = 3,
        [XmlEnum("4")]
        FourCardsOneSuit = 4,
        [XmlEnum("5")]
        FiveCardsOneSuit = 5
    }

    public enum QueryHmPositionEnum
    {
        Button = 5,
        Middle = 3,
        BigBlind = 1,
        Cutoff = 4,
        Early = 2,
        SmallBlind = 0
    }

    public class Enums
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}