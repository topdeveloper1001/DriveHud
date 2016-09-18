using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using HoldemHand;
using Model.Enums;
using Cards = HandHistories.Objects.Cards;

namespace Model.HandAnalyzers
{
    public class StraightTwoCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)) || HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.RankNumericValue);

            for (int i = sortedBoard.Count() - 1; i >= 2; i--)
            {
                var curCards = sortedBoard.Skip(i - 2);
                if (curCards.Count() >= 3)
                {
                    curCards = curCards.Take(3);
                    var nutStraight = GetNutStraightForCards(curCards);
                    if (nutStraight != null)
                    {
                        var requiredPocketCards = nutStraight.Where(x => !curCards.Any(c => c.RankNumericValue == x));
                        if (playerCards.All(x => requiredPocketCards.Contains(x.RankNumericValue)))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        private IEnumerable<int> GetNutStraightForCards(IEnumerable<Cards.Card> boardCards)
        {
            if (boardCards == null || boardCards.Count() != 3)
            {
                throw new ArgumentException("Method requires 3 board cards", "boardCards");
            }

            var boardRanks = boardCards.Select(x => x.RankNumericValue).ToList();
            boardRanks.Sort();

            int highestRank = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());

            /* 
               for Ace (14) - 1 iteration;
               for King (13) - 2 iterations
               For Queen (12) & lower - 3 iterations
            */

            int n = highestRank - boardRanks.Max() + 1;
            n = n < 3 ? n : 3;

            for (int i = n - 1; i >= 0; i--)
            {
                List<int> boardRanksList = FillWithClosestUniqueRanks(boardRanks, i);

                boardRanksList.Sort();
                bool isStraight = true;
                for (int j = 0; j < boardRanksList.Count - 1; j++)
                {
                    if (boardRanksList[j + 1] != boardRanksList[j] + 1)
                    {
                        isStraight = false;
                        break;
                    }
                }

                if (isStraight)
                {
                    return boardRanksList;
                }
            }

            return null;
        }

        private static List<int> FillWithClosestUniqueRanks(IEnumerable<int> boardRanks, int firstRankOffset)
        {
            var boardRanksList = new List<int>(boardRanks);
            for (int j = 0; j < 2; j++)
            {
                var cardRank = boardRanks.Max() + firstRankOffset;
                while (boardRanksList.Contains(cardRank))
                {
                    cardRank--;
                }
                boardRanksList.Add(cardRank);
            }

            return boardRanksList;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightTwoCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightTwoCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)) || HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.RankNumericValue).ToList();
            if (sortedBoard.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First())
             && sortedBoard.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.Last()))
            {
                sortedBoard.Insert(0, new Cards.Card("A", "c"));
            }

            for (int i = sortedBoard.Count() - 1; i >= 2; i--)
            {
                var curCards = sortedBoard.Skip(i - 2).ToList();
                if (curCards.Count() >= 3)
                {
                    curCards.AddRange(playerCards);
                    curCards = curCards.OrderByDescending(x => x.RankNumericValue).Take(5).ToList();
                    if (HandAnalyzerHelpers.IsStraight(curCards, true))
                    {
                        return playerCards.All(p => curCards.Any(c => c.RankNumericValue == p.RankNumericValue));
                    }
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightTwoCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightTwoCardBottomAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsStraight(playerCards, true) || playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)) || HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }


            var sortedCards = boardCards.OrderBy(x => x.RankNumericValue).Distinct().ToList();
            if (sortedCards.Min(x => x.RankNumericValue) == Cards.Card.GetRankNumericValue("3"))
            {
                if (HandAnalyzerHelpers.IsStraight(sortedCards.Take(3))
                    && (boardCards.Count == 3 || (boardCards.Count > 3 && !HandAnalyzerHelpers.IsStraight(sortedCards.Take(4)))))
                {
                    if (sortedCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
                    {
                        return false;
                    }

                    return playerCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First())
                        && playerCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.Last());
                }
            }

            List<int> sequence = new List<int>();
            for (int i = sortedCards.Count - 1; i >= 0; i--)
            {
                if (!sequence.Any() || sequence.Min() - sortedCards.ElementAt(i).RankNumericValue == 1)
                {
                    sequence.Add(sortedCards.ElementAt(i).RankNumericValue);
                    if (i != 0)
                        continue;
                }

                if (sequence.Count == 3 && (sequence.Min() > playerCards.Max(x => x.RankNumericValue)))
                {
                    if (HandAnalyzerHelpers.IsStraight(sequence.Union(playerCards.Select(x => x.RankNumericValue))))
                    {
                        return true;
                    }
                }

                sequence.Clear();
                sequence.Add(sortedCards.ElementAt(i).RankNumericValue);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightTwoCardBottom;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightOneCardNutAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var ten = Cards.Card.GetRankNumericValue("T");
            var ace = Cards.Card.GetRankNumericValue("A");
            var sortedBoardRanks = boardCards.Select(x => x.RankNumericValue).Where(x => x >= ten && x <= ace).Distinct().ToList();
            sortedBoardRanks.Sort();

            if (sortedBoardRanks.Count() == 4)
            {
                for (int i = ten; i <= ace; i++)
                {
                    if (sortedBoardRanks.Contains(i))
                    {
                        continue;
                    }

                    if (playerCards.Any(x => x.RankNumericValue == i))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightOneCardNut;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightOneCardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            return playerCards.Any(x => HandAnalyzerHelpers.IsOneCardStraight(x, boardCards));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightOneCard;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightOneCardHighAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            return playerCards.Any(x => HandAnalyzerHelpers.IsOneCardStraight(x, boardCards) && HandAnalyzerHelpers.IsDecentKicker(x));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightOneCardHigh;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightOneCardLowAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            return playerCards.Any(x => HandAnalyzerHelpers.IsOneCardStraight(x, boardCards) && HandAnalyzerHelpers.IsWeakKicker(x));
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightOneCardLow;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }


    public class StraightOneCardBottomAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var sortedCards = boardCards.OrderBy(x => x.RankNumericValue).Distinct().ToList();
            if (sortedCards.Min(x => x.RankNumericValue) == Cards.Card.GetRankNumericValue("2"))
            {
                if (HandAnalyzerHelpers.IsStraight(sortedCards.Take(4))
                    && (sortedCards.Count == 4 || (sortedCards.Count > 4 && !HandAnalyzerHelpers.IsStraight(sortedCards.Take(5)))))
                {
                    if (playerCards.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("6"))
                    || sortedCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
                    {
                        return false;
                    }
                    return playerCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First());
                }
            }

            List<int> sequence = new List<int>();
            for (int i = sortedCards.Count - 1; i >= 0; i--)
            {
                if (!sequence.Any() || (sequence.Min() - sortedCards.ElementAt(i).RankNumericValue) == 1)
                {
                    sequence.Add(sortedCards.ElementAt(i).RankNumericValue);
                    if (i != 0)
                        continue;
                }

                if (sequence.Count == 4 && playerCards.Any(x => (sequence.Min() - x.RankNumericValue) == 1) && !playerCards.Any(x => (x.RankNumericValue - sequence.Max()) == 1))
                {
                    return true;
                }

                sequence.Clear();
                sequence.Add(sortedCards.ElementAt(i).RankNumericValue);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightOneCardBottom;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }

    public class StraightOnBoardAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (boardCards == null || boardCards.Count != 5)
            {
                return false;
            }

            if (HandAnalyzerHelpers.IsStraight(boardCards, true))
            {
                if (boardCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()) && boardCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.Last()))
                {
                    return !playerCards.Any(x => x.RankNumericValue == (boardCards
                                                                       .Where(b => b.Rank != Cards.Card.PossibleRanksHighCardFirst.First())
                                                                       .Max(m => m.RankNumericValue) + 1));
                }
                else
                {
                    return !playerCards.Any(x => x.RankNumericValue == (boardCards.Max(m => m.RankNumericValue) + 1));
                }
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.OnBoardStraight;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return hand.HandTypeValue == Hand.HandTypes.Straight;
        }
    }
}
