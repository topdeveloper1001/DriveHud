using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.DataTypes;

namespace DriveHUD.PlayerXRay.DataTypes
{
    public class Card
    {
        public Card(string name)
        {
            if (name.Length == 2)
            {
                CardName = name;
            }

        }

        public string CardName { get; set; }
        public static List<CardRank> CardRankList = Enum.GetValues(typeof(CardRank)).Cast<CardRank>().OrderByDescending(x => x).ToList();
        public CardRank CardValue => GetCardRank(CardName[0]);
        public RangeCardSuit CardSuit => GetRangeCardSuit(CardName[1]);
        public int Rank => (int)CardValue;

        public static CardRank GetCardRank(char c)
        {
            if (c == '2')
                return CardRank.Two;
            if (c == '3')
                return CardRank.Three;
            if (c == '4')
                return CardRank.Four;
            if (c == '5')
                return CardRank.Five;
            if (c == '6')
                return CardRank.Six;
            if (c == '7')
                return CardRank.Seven;
            if (c == '8')
                return CardRank.Eight;
            if (c == '9')
                return CardRank.Nine;
            if (c == 'T' || c == 't')
                return CardRank.Ten;
            if (c == 'J' || c == 'j')
                return CardRank.Jack;
            if (c == 'Q' || c == 'q')
                return CardRank.Queen;
            if (c == 'K' || c == 'k')
                return CardRank.King;
            if (c == 'A' || c == 'a')
                return CardRank.Ace;

            return CardRank.None;
        }

        public static string GetCardRankString(CardRank rank)
        {
            switch (rank)
            {
                case CardRank.Two:
                    return "2";
                case CardRank.Three:
                    return "3";
                case CardRank.Four:
                    return "4";
                case CardRank.Five:
                    return "5";
                case CardRank.Six:
                    return "6";
                case CardRank.Seven:
                    return "7";
                case CardRank.Eight:
                    return "8";
                case CardRank.Nine:
                    return "9";
                case CardRank.Ten:
                    return "T";
                case CardRank.Jack:
                    return "J";
                case CardRank.Queen:
                    return "Q";
                case CardRank.King:
                    return "K";
                case CardRank.Ace:
                    return "A";
                default:
                    return  "";
            }
        }

        public static CardRank GetCardRank(string c)
        {
            return GetCardRank(c[0]);
            //if (c.Equals("2"))
            //    return CardRank.Two;
            //if (c.Equals("3"))
            //    return CardRank.Three;
            //if (c.Equals("4"))
            //    return CardRank.Four;
            //if (c.Equals("5"))
            //    return CardRank.Five;
            //if (c.Equals("6"))
            //    return CardRank.Six;
            //if (c.Equals("7"))
            //    return CardRank.Seven;
            //if (c.Equals("8"))
            //    return CardRank.Eight;
            //if (c.Equals("9"))
            //    return CardRank.Nine;
            //if (c.Equals("10"))
            //    return CardRank.Ten;
            //if (c.Equals("J") || c.Equals("j"))
            //    return CardRank.Jack;
            //if (c.Equals("Q") || c.Equals("q"))
            //    return CardRank.Queen;
            //if (c.Equals("K") || c.Equals("k"))
            //    return CardRank.King;
            //if (c.Equals("A") || c.Equals("a"))
            //    return CardRank.Ace;

            //return CardRank.None;
        }

        private RangeCardSuit GetRangeCardSuit(char c)
        {
            if (c == 's' || c == 'S')
                return RangeCardSuit.Spades;

            if (c == 'c' || c == 'C')
                return RangeCardSuit.Clubs;

            if (c == 'h' || c == 'H')
                return RangeCardSuit.Hearts;

            if (c == 'd' || c == 'D')
                return RangeCardSuit.Diamonds;

            return RangeCardSuit.None;
        }
    }
}
