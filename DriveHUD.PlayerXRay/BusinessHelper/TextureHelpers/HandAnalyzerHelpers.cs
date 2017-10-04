using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.DataTypes;

namespace DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers
{
    public static class HandAnalyzerHelpers
    {
        internal static List<int> GetConnectedCards(int rankNumericValue, List<int> rankNumericValuesList)
        {
            var list = new List<int>(rankNumericValuesList.Where(x => x != rankNumericValue));
            var returnList = new List<int>();
            returnList.Add(rankNumericValue);

            while (returnList.Any(r => list.Any(l => Math.Abs(r - l) == 1)))
            {
                var rank = list.FirstOrDefault(l => returnList.Any(r => Math.Abs(r - l) == 1));
                returnList.Add(rank);
                list.Remove(rank);
            }

            return returnList;
        }


        /// <summary>
        /// Determines if the value of cards are in a sequence
        /// </summary>
        /// <param name="handRanks">Collection of numeric card rank values to compare</param>
        /// <returns>True if the cards are in sequence, false if the hand is null or empty or the values of cards are not in a sequence</returns>
        internal static bool IsStraight(List<int> handRanks)
        {
            bool isStraight = true;
            var boardRanksList = handRanks.ToList();
            boardRanksList.Sort();
            for (int j = 0; j < boardRanksList.Count - 1; j++)
            {
                if (boardRanksList[j + 1] != boardRanksList[j] + 1)
                {
                    isStraight = false;
                    break;
                }
            }
            return isStraight;
        }

        /// <summary>
        /// Determines if the value of cards are in a sequence
        /// </summary>
        /// <param name="hand">Collection of cards to compare</param>
        /// <returns>True if the cards are in sequence, false if the hand is null or empty or the values of cards are not in a sequence</returns>    
        internal static bool IsStraight(IEnumerable<Card> hand, bool checkWheel = false)
        {
            if (hand == null || !hand.Any())
                return false;
            int ace = (int) Card.GetCardRank("A");
            int two = (int) Card.GetCardRank("2"); ;
            List<int> sortedHand = new List<int>();
            if (checkWheel && hand.Any(x => x.Rank == two) && hand.Any(x => x.Rank == ace))
            {
                foreach (var card in hand)
                {
                    if (card.Rank == ace)
                    {
                        sortedHand.Add(1);
                    }
                    else
                    {
                        sortedHand.Add(card.Rank);
                    }
                }
            }
            else
            {
                sortedHand = hand.Select(x => x.Rank).ToList();
            }
            sortedHand.Sort();


            for (int i = 0; i < sortedHand.Count - 1; i++)
            {
                if (sortedHand[i + 1] != sortedHand[i] + 1)
                {
                    return false;
                }
            }

            return true;
        }


        internal static bool HasPair(List<Card> hand, int number)
        {
            // Group the cards by value
            var groups = hand.GroupBy(c => c.Rank);

            // If there is more than two cards that match then the hand is not a pair
            if (groups.Count(c => c.Count() > 2) > 0)
                return false;

            // Returns true if the number of groups specified in the parameters matches the number of groups with a pair
            return groups.Count(c => c.Count() == 2) == number;
        }

        internal static bool IsNoOfKind(List<Card> hand, int number)
        {  
            if (hand == null || !hand.Any())
                return false;

            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number), "Number to compare should be at least 1.");

            bool isNofKind = hand
                .GroupBy(c => c.Rank)
                .Any(g => g.Count() == number);

            return isNofKind;
        }

        internal static bool IsNoOvercards(IEnumerable<Card> playerCards, IEnumerable<Card> boardCards)
        {
            return !playerCards.Any(p => boardCards.Any(b => b.Rank < p.Rank));
        }

        internal static bool IsOneOvercard(IEnumerable<Card> playerCards, IEnumerable<Card> boardCards)
        {
            return playerCards.Any(p => boardCards.Any(b => b.Rank > p.Rank)) && playerCards.Any(p => !boardCards.Any(b => b.Rank >= p.Rank));
        }

        internal static bool IsTwoOvercards(IEnumerable<Card> playerCards, IEnumerable<Card> boardCards)
        {
            return !playerCards.Any(p => boardCards.Any(b => b.Rank >= p.Rank));
        }

        internal static List<Card> CombineCardLists(IEnumerable<Card> playerCards, IEnumerable<Card> boardCards)
        {
            var allCards = new List<Card>(playerCards);
            allCards.AddRange(boardCards);
            return allCards;
        }

        internal static Card GetUnpairedCard(IEnumerable<Card> playerCards, IEnumerable<Card> boardCards)
        {
            return playerCards.OrderByDescending(x => x.Rank).FirstOrDefault(p => boardCards.All(b => b.Rank != p.Rank));
        }

        /// <summary>
        /// Returns the highest possible card rank that is not in the provided collection
        /// </summary>
        /// <param name="hand">Collection of cards</param>
        /// <returns>Highest unpaired card rank</returns>
        internal static int GetTopKickerRank(IEnumerable<Card> hand)
        {
            return (int) Card.CardRankList.First(r => hand.All(h => h.CardValue != r));
        }

        /// <summary>
        /// Determines if specific hole card is top kicker
        /// </summary>
        /// <param name="unpairedCard">hole card</param>
        /// <param name="board">Collection of board cards</param>
        /// <returns></returns>
        internal static bool IsTopKicker(Card unpairedCard, IEnumerable<Card> board)
        {
            return unpairedCard.Rank == GetTopKickerRank(board);
        }

        /// <summary>
        /// Determines if specific hole card is decent kicker
        /// </summary>
        /// <param name="card">hole card</param>
        /// <param name="board">Collection of board cards</param>
        /// <returns></returns>
        internal static bool IsDecentKicker(Card card)
        {
            return card.CardValue > Card.GetCardRank("T");
        }

        /// <summary>
        /// Determines if specific hole card is weak kicker
        /// </summary>
        /// <param name="card">hole card</param>
        /// <param name="board">Collection of board cards</param>
        /// <returns></returns>
        internal static bool IsWeakKicker(Card card)
        {
            return card.CardValue <= Card.GetCardRank("T");
        }

        internal static bool IsFlush(IEnumerable<Card> hand)
        {
            if (hand == null || !hand.Any())
                return false;

            bool flush = hand.GroupBy(c => c.CardSuit).Count() == 1;
            return flush;
        }

        /// <summary>
        /// Determines if it's possible to create straight using specified card
        /// </summary>
        /// <param name="playerCard"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        internal static bool IsOneCardStraight(Card playerCard, IEnumerable<Card> boardCards)
        {
            if (boardCards == null || boardCards.Count() < 4)
            {
                return false;
            }
            var sortedBoardRanks = boardCards.Select(x => x.Rank).Distinct().ToList();
            sortedBoardRanks.Sort();
            if (sortedBoardRanks.Contains((int) Card.GetCardRank("A")))
            {
                sortedBoardRanks.Insert(0, 1);
            }

            if (sortedBoardRanks.Count < 4)
            {
                return false;
            }

            for (int i = 0; i <= sortedBoardRanks.Count - 4; i++)
            {
                var curCards = sortedBoardRanks.Skip(i);
                if (curCards.Count() >= 4)
                {
                    var list = new List<int>(curCards.Take(4));
                    list.Add(playerCard.Rank);

                    if (HandAnalyzerHelpers.IsStraight(list) && !boardCards.Any(x => x.Rank == playerCard.Rank))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool IsFlushDraw(IEnumerable<Card> playerCards, IEnumerable<Card> boardCards, int amountOfPlayersCardsInDraw, bool isBackdoor = false)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var suitGroups = allCards.GroupBy(x => x.CardSuit);
            var drawGroup = suitGroups.FirstOrDefault(x => x.Count() == (isBackdoor ? 3 : 4));

            if (drawGroup == null || suitGroups.Any(x => x.Count() > drawGroup.Count()) || playerCards.Count(x => drawGroup.Any(d => d.CardSuit == x.CardSuit)) != amountOfPlayersCardsInDraw)
            {
                return false;
            }

            return true;
        }

    }
}
