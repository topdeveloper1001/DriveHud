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
            foreach(var item in pockets)
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

            var count = BoardCards.FromCards(boardCards).Count();

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
            List<char> cards = new List<char>(new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' });

            int c = 0;
            List<List<string>> allGroups = new List<List<string>>();


            List<string> handsToRemove = new List<string>();
            for (int i = 0; i < hands.Count; i++)
            {
                if (hands[i].Equals("")) continue;
                if (handsToRemove.Contains(hands[i])) continue;
                List<string> group = new List<string>();
                string firstHand = hands[i];
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

            List<string> res = new List<string>();
            foreach (List<string> group in allGroups)
            {
                if (group.Count > 1)
                {
                    if ((group[0][0] != group[0][1] && cards.IndexOf(group[group.Count - 1][1]) == cards.IndexOf(group[0][0]) - 1)
                        || (group[0][0] == group[0][1] && group[group.Count - 1][0] == 'A'))
                        res.Add(group[0] + "+");
                    else res.Add(group[0] + "-" + group[group.Count - 1]);
                }
                else res.Add(group[0]);
            }
            return res;
        }
    }
}