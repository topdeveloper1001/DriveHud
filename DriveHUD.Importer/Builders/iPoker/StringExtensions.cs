using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal static class StringExtensions
    {
        public static Hand GetHandFromString(this string cardsString)
        {
            if (string.IsNullOrWhiteSpace(cardsString))
            {
                throw new ArgumentException("cardsString must not be empty");
            }

            var cards = (from card in cardsString.Split(' ')
                         let rankString = card.Substring(1, card.Length - 1)
                         select new Card
                         {
                             Suit = card.Substring(0, 1),
                             RankString = rankString,
                             Rank = ConvertCardToRank(rankString)
                         }).ToList();

            var hand = new Hand(cards);

            return hand;
        }

        public static int ConvertCardToRank(this string card)
        {
            int rank;

            if (!int.TryParse(card, out rank))
            {
                switch (card)
                {
                    case "J":
                        rank = 11;
                        break;
                    case "Q":
                        rank = 12;
                        break;
                    case "K":
                        rank = 13;
                        break;
                    case "A":
                        rank = 14;
                        break;
                    default:
                        throw new ArgumentException(string.Format("Unknown card - {0}", card), "cards");
                }

            }

            return rank;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}