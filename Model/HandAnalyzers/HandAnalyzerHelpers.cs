using System;
using System.Collections.Generic;
using System.Linq;
using HandHistories.Objects.Cards;
using System.Text;
using System.Threading.Tasks;
using Cards = HandHistories.Objects.Cards;
using Model.Enums;
using System.Diagnostics;

namespace Model.HandAnalyzers
{
    internal static class HandAnalyzerHelpers
    {
        /// <summary>
        /// Determines if the same suit is in every card in the hand
        /// </summary>
        /// <param name="hand">Collection of cards to compare</param>
        /// <returns>True if every card contains the same suit, false if the hand is null or empty or every card in the hand does not contain the same suit</returns>
        internal static bool IsFlush(IEnumerable<HandHistories.Objects.Cards.Card> hand)
        {
            if (hand == null || !hand.Any())
                return false;

            bool flush = hand.GroupBy(c => c.Suit).Count() == 1;
            return flush;
        }

        internal static bool IsFlushDraw(IEnumerable<Cards.Card> playerCards, BoardCards boardCards, int amountOfPlayersCardsInDraw, bool isBackdoor = false)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var suitGroups = allCards.GroupBy(x => x.Suit);
            var drawGroup = suitGroups.FirstOrDefault(x => x.Count() == (isBackdoor ? 3 : 4));

            if (drawGroup == null || suitGroups.Any(x => x.Count() > drawGroup.Count()) || playerCards.Count(x => drawGroup.Any(d => d.CardStringValue == x.CardStringValue)) != amountOfPlayersCardsInDraw)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the value of cards are in a sequence
        /// </summary>
        /// <param name="hand">Collection of cards to compare</param>
        /// <returns>True if the cards are in sequence, false if the hand is null or empty or the values of cards are not in a sequence</returns>
        internal static bool IsStraight(IEnumerable<HandHistories.Objects.Cards.Card> hand, bool checkWheel = false)
        {
            if (hand == null || !hand.Any())
                return false;
            string ace = Cards.Card.PossibleRanksHighCardFirst[0];
            string two = Cards.Card.PossibleRanksHighCardFirst.Last();
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
                        sortedHand.Add(card.RankNumericValue);
                    }
                }
            }
            else
            {
                sortedHand = hand.Select(x => x.RankNumericValue).ToList();
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

        /// <summary>
        /// Determines if the value of cards are in a sequence
        /// </summary>
        /// <param name="handRanks">Collection of numeric card rank values to compare</param>
        /// <returns>True if the cards are in sequence, false if the hand is null or empty or the values of cards are not in a sequence</returns>
        internal static bool IsStraight(IEnumerable<int> handRanks)
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
        /// Determines if it's possible to create straight using specified card
        /// </summary>
        /// <param name="playerCard"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        internal static bool IsOneCardStraight(Cards.Card playerCard, IEnumerable<Cards.Card> boardCards)
        {
            if (boardCards == null || boardCards.Count() < 4)
            {
                return false;
            }
            var sortedBoardRanks = boardCards.Select(x => x.RankNumericValue).Distinct().ToList();
            sortedBoardRanks.Sort();
            if (sortedBoardRanks.Contains(Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First())))
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
                    list.Add(playerCard.RankNumericValue);

                    if (HandAnalyzerHelpers.IsStraight(list) && !boardCards.Any(x => x.RankNumericValue == playerCard.RankNumericValue))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if the hand has a specified number of the same card.
        /// </summary>
        /// <param name="hand">Collection of cards to compare</param>
        /// <param name="number">Number of cards that should have the same value</param>
        ///     IEnumerable&lt;Card&gt; hand = new[]
        ///         {
        ///             new Card(CardValueType.Ace, SuitType.Club),
        ///             new Card(CardValueType.Ace, SuitType.Spade),
        ///             new Card(CardValueType.Ace, SuitType.Heart),
        ///             new Card(CardValueType.Jack, SuitType.Spade),
        ///             new Card(CardValueType.Ten, SuitType.Spade)
        ///         };
        /// 
        ///     hand.IsNofKind(3);      // Returns true because there are three aces.
        ///     hand.IsNofKind(4);      // Returns false because there is not a four of a kind;
        /// <example>
        /// </example>
        /// <returns>True if the there is the specfied number of cards with the same value</returns>
        internal static bool IsNofKind(IEnumerable<HandHistories.Objects.Cards.Card> hand, int number)
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

        /// <summary>
        /// Determines if the specified number of pairs is in the hand
        /// </summary>
        /// <param name="hand">Collection of cards to compare</param>
        /// <param name="number">Number of pairs in the hand</param>
        ///     IEnumerable&lt;Card&gt; hand = new[]
        ///         {
        ///             new Card(CardValueType.Ace, SuitType.Club),
        ///             new Card(CardValueType.Ace, SuitType.Spade),
        ///             new Card(CardValueType.Jack, SuitType.Heart),
        ///             new Card(CardValueType.Jack, SuitType.Spade),
        ///             new Card(CardValueType.Ten, SuitType.Spade)
        ///         };
        /// 
        ///     hand.IsPair(2);      // Returns true because there are two pairs in the hand.
        ///     hand.IsPair(1);      // Returns false because there are two pairs in the hand, not one;
        /// <example>
        /// </example>
        /// <returns>True if the there is the specfied number of cards with the same value</returns>
        internal static bool IsPair(IEnumerable<HandHistories.Objects.Cards.Card> hand, int number)
        {
            // Group the cards by value
            var groups = hand.GroupBy(c => c.Rank);

            // If there is more than two cards that match then the hand is not a pair
            if (groups.Count(c => c.Count() > 2) > 0)
                return false;

            // Returns true if the number of groups specified in the parameters matches the number of groups with a pair
            return groups.Count(c => c.Count() == 2) == number;
        }

        /// <summary>
        /// Tries to find royal flush in hand
        /// </summary>
        /// <param name="hand">Collection of card to find royal flush in</param>
        /// <returns>Roayl flush hand, or null if input collection doesn't contain it</returns>
        internal static IEnumerable<HandHistories.Objects.Cards.Card> GetRoyalFlushCards(IEnumerable<HandHistories.Objects.Cards.Card> hand)
        {
            var flushCards = hand.GroupBy(x => x.Suit).FirstOrDefault(x => x.Count() >= 5);
            string aceRank = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.First();

            if (flushCards == null || !flushCards.Any(x => x.Rank == aceRank))
            {
                return null;
            }

            var flushCardsList = new List<HandHistories.Objects.Cards.Card>(flushCards);
            flushCardsList = flushCardsList.OrderByDescending(x => x.RankNumericValue).ToList();

            var streetCards = flushCardsList.Take(5);
            if (HandAnalyzerHelpers.IsStraight(streetCards))
            {
                return streetCards;
            }

            return null;
        }

        /// <summary>
        /// Returns highest card from first collection that doesn't have pair in the second collection or null if there are no such card.
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        internal static Cards.Card GetUnpairedCard(IEnumerable<Cards.Card> playerCards, IEnumerable<Cards.Card> boardCards)
        {
            return playerCards.OrderByDescending(x => x.RankNumericValue).FirstOrDefault(p => !boardCards.Any(b => b.RankNumericValue == p.RankNumericValue));
        }

        /// <summary>
        /// Returns the highest possible card rank that is not in the provided collection
        /// </summary>
        /// <param name="hand">Collection of cards</param>
        /// <returns>Highest unpaired card rank</returns>
        internal static string GetTopKickerRank(IEnumerable<HandHistories.Objects.Cards.Card> hand)
        {
            return Cards.Card.PossibleRanksHighCardFirst.First(r => !hand.Any(b => b.Rank == r));
        }

        /// <summary>
        /// Determines if specific hole card is top kicker
        /// </summary>
        /// <param name="unpairedCard">hole card</param>
        /// <param name="board">Collection of board cards</param>
        /// <returns></returns>
        internal static bool IsTopKicker(Cards.Card unpairedCard, IEnumerable<Cards.Card> board)
        {
            return unpairedCard.Rank == GetTopKickerRank(board);
        }

        /// <summary>
        /// Determines if specific hole card is decent kicker
        /// </summary>
        /// <param name="card">hole card</param>
        /// <param name="board">Collection of board cards</param>
        /// <returns></returns>
        internal static bool IsDecentKicker(Cards.Card card)
        {
            return card.RankNumericValue > Cards.Card.GetRankNumericValue("T");
        }

        /// <summary>
        /// Determines if specific hole card is weak kicker
        /// </summary>
        /// <param name="card">hole card</param>
        /// <param name="board">Collection of board cards</param>
        /// <returns></returns>
        internal static bool IsWeakKicker(Cards.Card card)
        {
            return card.RankNumericValue <= Cards.Card.GetRankNumericValue("T");
        }

        internal static bool IsNoOvercards(IEnumerable<Cards.Card> playerCards, IEnumerable<Cards.Card> boardCards)
        {
            return !playerCards.Any(p => boardCards.Any(b => b.RankNumericValue < p.RankNumericValue));
        }

        internal static bool IsOneOvercard(IEnumerable<Cards.Card> playerCards, IEnumerable<Cards.Card> boardCards)
        {
            return playerCards.Any(p => boardCards.Any(b => b.RankNumericValue > p.RankNumericValue)) && playerCards.Any(p => !boardCards.Any(b => b.RankNumericValue >= p.RankNumericValue));
        }

        internal static bool IsTwoOvercards(IEnumerable<Cards.Card> playerCards, IEnumerable<Cards.Card> boardCards)
        {
            return !playerCards.Any(p => boardCards.Any(b => b.RankNumericValue >= p.RankNumericValue));
        }

        internal static IList<Cards.Card> CombineCardLists(IEnumerable<Cards.Card> playerCards, IEnumerable<Cards.Card> boardCards)
        {
            var allCards = new List<Cards.Card>(playerCards);
            allCards.AddRange(boardCards);
            return allCards;
        }

        internal static bool IsBottomStraight(ShowdownHands hand)
        {
            return hand == ShowdownHands.StraightOneCardBottom || hand == ShowdownHands.StraightTwoCardBottom;
        }

        internal static bool IsSetSecondOrLower(ShowdownHands hand)
        {
            return hand > ShowdownHands.ThreeOfAKindBottomSet && hand < ShowdownHands.ThreeOfAKindTopSet;
        }

        internal static List<int> GetOrderedNumericRanksList(IEnumerable<HandHistories.Objects.Cards.Card> cards)
        {
            var orderedRanks = cards.Select(x => x.RankNumericValue).Distinct().ToList();
            if (cards.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("A")))
            {
                orderedRanks.Insert(0, 1);
            }
            orderedRanks.Sort();
            return orderedRanks;
        }

        internal static List<int> GetConnectedCards(int rankNumericValue, List<int> rankNumericValuesList)
        {
            var list = new List<int>(rankNumericValuesList.Where(x=> x != rankNumericValue));
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
        /// Determines if hand is withing AQ+/TT+ range
        /// </summary>
        /// <param name="playerCards"></param>
        /// <returns></returns>
        internal static bool IsPremiumHand(IEnumerable<Cards.Card> playerCards)
        {
            return IsPremiumHand(string.Join("", playerCards.Select(x => x.CardStringValue)));
        }

        /// <summary>
        /// Determines if hand is withing AQ+/TT+ range
        /// </summary>
        /// <param name="playerHand"></param>
        /// <returns></returns>
        internal static bool IsPremiumHand(string playerHand)
        {
            if(string.IsNullOrWhiteSpace(playerHand))
            {
                return false;
            }

            return HoldemHand.PocketHands.Query("AA, KK, QQ, TT, AKs, AKo, AQs, AQo").Contains(HoldemHand.Hand.ParseHand(playerHand));
        }

        /// <summary>
        /// Determines if hand is out of AT+/99+ range
        /// </summary>
        /// <param name="playerCards"></param>
        /// <returns></returns>
        internal static bool IsBluffRange(IEnumerable<Cards.Card> playerCards)
        {
            return IsBluffRange(string.Join("", playerCards.Select(x => x.CardStringValue)));
        }

        /// <summary>
        /// Determines if hand is out of AT+/99+ range
        /// </summary>
        /// <param name="playerHand"></param>
        /// <returns></returns>
        internal static bool IsBluffRange(string playerHand)
        {
            if (string.IsNullOrWhiteSpace(playerHand))
            {
                return false;
            }

            return !HoldemHand.PocketHands.Query("AA, KK, QQ, TT, 99, AKs, AKo, AQs, AQo, AJs, AJo, ATs, ATo").Contains(HoldemHand.Hand.ParseHand(playerHand));
        }

        /// <summary>
        /// Determines if hand is out of 22+/A9o+/A2s+/KTo+/KTs+/QTo+/QTs+/JTs/JTo range
        /// </summary>
        /// <param name="playerCards"></param>
        /// <returns></returns>
        internal static bool IsMarginalHand(IEnumerable<Cards.Card> playerCards)
        {
            return IsMarginalHand(string.Join("", playerCards.Select(x => x.CardStringValue)));
        }

        /// <summary>
        /// Determines if hand is out of 22+/A9o+/A2s+/KTo+/KTs+/QTo+/QTs+/JTs/JTo range
        /// </summary>
        /// <param name="playerHand"></param>
        /// <returns></returns>
        internal static bool IsMarginalHand(string playerHand)
        {
            if (string.IsNullOrWhiteSpace(playerHand))
            {
                return false;
            }
            // 22+/A9o+/A2s+/KTo+/KTs+/QTo+/QTs+/JTs/JTo
            return !HoldemHand.PocketHands.Query("AA, KK, QQ, TT, 99, 88, 77, 66, 55, 44, 33, 22, A9o, ATo, AJo, AQo, AKo, A2s, A3s, A4s, A5s, A6s, A7s, A8s, A9s, ATs, AJs, AQs, AKs, KTo, KJo, KQo, KTs, KJs, KQs, QTo, QJo, QTs, QJs, JTs, JTo").Contains(HoldemHand.Hand.ParseHand(playerHand));
        }
    }
}
