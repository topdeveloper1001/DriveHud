using HandHistories.Objects.Cards;
using HoldemHand;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Extensions
{
    /// <summary>
    /// Splits cards from single string into array of cards (also in string)
    /// </summary>
    public static class CardHelper
    {
        private static char[] types = { 'c', 's', 'd', 'h' };
        public static List<string> Split(string cards)
        {
            List<string> result = new List<string>();

            string card = string.Empty;
            foreach (var letter in cards)
            {
                card += letter;

                if (types.Contains(letter))
                {
                    result.Add(card);
                    card = string.Empty;
                }
            }

            return result;
        }

        /// <summary>
        /// Splits cards into array of all possible 2 card combinations
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static List<string> SplitTwoCards(string cards)
        {
            var pockets = Split(cards).ToArray();
            var resultList = new List<string>();
            for (int i = 0; i < pockets.Length; i++)
            {
                for (int j = i + 1; j < pockets.Length; j++)
                {
                    resultList.Add(pockets[i] + " " + pockets[j]);
                }
            }

            return resultList;
        }

        /// <summary>
        /// Determines the highest hand based on provided values
        /// </summary>
        /// <param name="pockets">string containing pocket cards</param>
        /// <param name="board">board cards</param>
        /// <returns>Highest hand</returns>
        public static Hand FindBestHand(string holeCards, string board)
        {
            return FindBestHand(SplitTwoCards(holeCards), board);
        }

        /// <summary>
        /// Determines the highest hand based on provided values
        /// </summary>
        /// <param name="pockets">collection of pockets</param>
        /// <param name="board">board cards</param>
        /// <returns>Highest hand</returns>
        public static Hand FindBestHand(IEnumerable<string> pockets, string board)
        {
            var list = new List<Hand>();
            foreach (var item in pockets)
            {
                list.Add(new HoldemHand.Hand(item, board));
            }

            list.Sort((h1, h2) => (h1 > h2) ? 1 : (h1 < h2) ? -1 : 0);

            return list.LastOrDefault();
        }

        public static int GetCardRank(string card)
        {
            if (string.IsNullOrEmpty(card))
            {
                return -1;
            }

            int parsed = 0;
            if (Int32.TryParse(card[0].ToString(), out parsed))
            {
                return parsed;
            }

            switch (card[0].ToString().ToUpper())
            {
                case "T":
                    return 10;
                case "J":
                    return 11;
                case "Q":
                    return 12;
                case "K":
                    return 13;
                case "A":
                    return 14;
            }

            return -1;
        }

        /// <summary>
        /// Determines if board contains enough cards to display specified street
        /// </summary>
        /// <param name="boardCards"></param>
        /// <param name="street"></param>
        /// <returns></returns>
        public static bool IsStreetAvailable(string boardCards, Street street)
        {
            if (string.IsNullOrEmpty(boardCards))
            {
                return false;
            }

            return IsStreetAvailable(BoardCards.FromCards(boardCards), street);
        }

        /// <summary>
        /// Determines if board contains enough cards to display specified street
        /// </summary>
        /// <param name="boardCards"></param>
        /// <param name="street"></param>
        /// <returns></returns>
        public static bool IsStreetAvailable(BoardCards boardCards, Street street)
        {
            if (boardCards == null)
            {
                return false;
            }


            var count = boardCards.Count();

            switch (street)
            {
                case Street.Preflop:
                    return true;
                case Street.Flop:
                    return count > 2;
                case Street.Turn:
                    return count > 3;
                case Street.River:
                    return count > 4;
                default:
                    return false;
            }
        }

        public static int GetCardsAmountForStreet(Street street)
        {
            switch (street)
            {
                case Street.Flop:
                    return 3;
                case Street.Turn:
                    return 4;
                case Street.River:
                    return 5;
            }
            return -1;
        }

        /// <summary>
        /// Combines cards into groups
        /// </summary>
        /// <param name="hands">Collection of hole cards</param>
        /// <returns>List of grouped hands</returns>
        public static List<string> GetHandsFormatted(List<string> hands)
        {
            var cards = new List<char>(new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' });
            var allGroups = new List<List<string>>();
            var handsToRemove = new List<string>();

            for (int i = 0; i < hands.Count; i++)
            {
                if (string.IsNullOrEmpty(hands[i]))
                {
                    continue;
                }

                if (handsToRemove.Contains(hands[i]))
                {
                    continue;
                }

                var group = new List<string>();

                var firstHand = hands[i];

                group.Add(firstHand);

                string lastHand = firstHand;

                bool newCardAdded = true;

                while (newCardAdded)
                {
                    int countBefore = group.Count;

                    foreach (string hand in hands)
                    {
                        if (lastHand[0] == lastHand[1] && hand[0] == hand[1] && hand != firstHand && hand != lastHand)
                        {
                            if (cards.IndexOf(lastHand[0]) == cards.IndexOf(hand[0]) - 1)
                            {
                                group.Add(hand);
                            }
                            else if (cards.IndexOf(firstHand[0]) == cards.IndexOf(hand[0]) + 1)
                            {
                                group.Insert(0, hand);
                            }
                        }
                        else if (hand != firstHand && hand != lastHand && hand.Length > 2 && firstHand.Length > 2 && hand[2] == firstHand[2])
                        {
                            if (hand[0].Equals(lastHand[0]) && cards.IndexOf(lastHand[1]) == cards.IndexOf(hand[1]) - 1)
                            {
                                group.Add(hand);
                            }
                            else if (hand[0].Equals(firstHand[0]) && cards.IndexOf(firstHand[1]) == cards.IndexOf(hand[1]) + 1)
                            {
                                group.Insert(0, hand);
                            }
                        }

                        lastHand = group[group.Count - 1];
                        firstHand = group[0];
                    }

                    newCardAdded = countBefore != group.Count;
                }

                allGroups.Add(group);

                foreach (string hand in group)
                {
                    handsToRemove.Add(hand);
                }
            }

            var res = new List<string>();

            foreach (var group in allGroups)
            {
                if (group.Count > 1)
                {
                    if ((group[0][0] != group[0][1] && cards.IndexOf(group[group.Count - 1][1]) == cards.IndexOf(group[0][0]) - 1)
                        || (group[0][0] == group[0][1] && group[group.Count - 1][0] == 'A'))
                    {
                        res.Add(group[0] + "+");
                    }
                    else
                    {
                        res.Add(group[0] + "-" + group[group.Count - 1]);
                    }

                    continue;
                }

                res.Add(group[0]);
            }

            return res;
        }

        /// <summary>
        /// Uncombines cards from groups
        /// </summary>
        /// <param name="hands">Cards range</param>
        /// <returns>List of hole cards</returns>
        public static List<string> GetHandsUnFormatted(string cards)
        {
            if (string.IsNullOrEmpty(cards))
            {
                return new List<string>();
            }

            if (cards.Length < 3 || (cards.Length == 3 && cards[2] != '+'))
            {
                return new List<string> { cards };
            }

            var cardRanks = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Reverse().ToArray();

            var firstCard = cards.First().ToString();
            var secondCard = string.Empty;
            var suit = string.Empty;

            if (cards.Last() == '+')
            {
                secondCard = cards.Substring(1, cards.Length - 2);

                if (secondCard.Length == 2)
                {
                    suit = secondCard[1].ToString();
                    secondCard = secondCard[0].ToString();
                }

                // pairs (QQ+)
                if (secondCard == firstCard)
                {
                    return cardRanks
                        .SkipWhile(x => x != firstCard)
                        .Select(x => x + x)
                        .ToList();
                }

                // not pair (42s+)
                return cardRanks
                        .SkipWhile(x => x != secondCard)
                        .TakeWhile(x => x != firstCard)
                        .Select(x => firstCard + x + suit)
                        .ToList();
            }

            var cardsFromTo = cards.Split('-');

            if (cardsFromTo.Length != 2)
            {
                return new List<string> { cards };
            }

            var cardsFrom = cardsFromTo[0];
            var cardsTo = cardsFromTo[1];

            if (cardsFrom.Length != cardsTo.Length ||
                cardsFrom.Length != 2 && cardsFrom.Length != 3 ||
                (cardsFrom.Length == 3 && cardsFrom[0] != cardsTo[0]))
            {
                return new List<string> { cards };
            }

            suit = cardsFrom.Length == 3 ? cardsFrom[2].ToString() : string.Empty;
            secondCard = cardsFrom[1].ToString();
            var secondToCard = cardsTo[1].ToString();

            Func<string, string> selector;

            if (cardsFrom.Length == 2)
            {
                selector = x => x + x;
            }
            else
            {
                selector = x => firstCard + x + suit;
            }

            return cardRanks
                       .SkipWhile(x => x != secondCard)
                       .TakeWhile(x => x != secondToCard)
                       .Concat(new[] { secondToCard })
                       .Select(selector)
                       .ToList();
        }
    }
}