//-----------------------------------------------------------------------
// <copyright file="RangeCardsEnums.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace Model.Enums
{
    public enum RangeCardSuit
    {
        None = -1, Clubs = 0, Diamonds = 1, Hearts = 2, Spades = 3
    }

    public enum RangeCardRank
    {
        None = -1, Two = 0, Three = 1, Four = 2, Five = 3, Six = 4, Seven = 5, Eight = 6,
        Nine = 7, Ten = 8, Jack = 9, Queen = 10, King = 11, Ace = 12
    }

    public enum RangeSelectorItemType
    {
        Default,
        Pair,
        Suited,
        OffSuited
    }

    public enum Likelihood
    {
        Definitely = 100,
        Likely = 75,
        NotVeryLikely = 50,
        Rarely = 25,
        Custom,
        None = -1
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
                case RangeCardSuit.None:
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

        public static RangeCardRank StringToRank(this RangeCardRank rank, string value)
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
}