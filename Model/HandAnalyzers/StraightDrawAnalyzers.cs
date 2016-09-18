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
    public class StraightDrawTwoCardOpenEndedAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)))
            {
                return false;
            }

            var rankList = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards).Where(x => x.Rank != Cards.Card.PossibleRanksHighCardFirst.First()).Select(x => x.RankNumericValue).Distinct().ToList();
            rankList.Sort();

            List<int> straightDraw = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!straightDraw.Any() || (rankList[i] == straightDraw.Last() + 1))
                {
                    straightDraw.Add(rankList[i]);
                    continue;
                }

                if (straightDraw.Count == 4)
                {
                    if (HandAnalyzerHelpers.IsStraight(straightDraw) && playerCards.All(x => straightDraw.Contains(x.RankNumericValue)))
                    {
                        return true;
                    }
                }

                straightDraw.Clear();
                straightDraw.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawTwoCardOpenEnded;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsOpenEndedStraightDraw(hand.MaskValue, 0UL);
        }
    }

    public class StraightDrawTwoCardDoubleGutShotAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.RankNumericValue).Distinct().ToList();
            if (allCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
            {
                rankList.Add(1);
            }
            rankList.Sort();

            List<int> straightDraw = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!straightDraw.Any() || (rankList[i] == straightDraw.Last() + 1))
                {
                    straightDraw.Add(rankList[i]);
                    continue;
                }

                if (straightDraw.Count == 3)
                {
                    if (HandAnalyzerHelpers.IsStraight(straightDraw))
                    {
                        if (rankList.Any(x => x == straightDraw.Max() + 2) && rankList.Any(x => x == straightDraw.Min() - 2))
                        {
                            straightDraw.Add(straightDraw.Max() + 2);
                            straightDraw.Add(straightDraw.Min() - 2);

                            if (straightDraw.Contains(1))
                            {
                                straightDraw[straightDraw.IndexOf(1)] = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                            }

                            if (playerCards.All(x => straightDraw.Contains(x.RankNumericValue)))
                            {
                                return true;
                            }
                        }
                    }
                }

                straightDraw.Clear();
                straightDraw.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawTwoCardDoubleGutShot;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsStraightDraw(hand.MaskValue, 0UL);
        }
    }

    public class StraightDrawTwoCardGutShotAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.RankNumericValue).Distinct().ToList();
            if (allCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
            {
                rankList.Add(1);
            }
            rankList.Sort();

            List<int> currentSequence = new List<int>();
            List<int> previousSequence = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!currentSequence.Any() || (rankList[i] == currentSequence.Last() + 1))
                {
                    currentSequence.Add(rankList[i]);
                    continue;
                }

                if (currentSequence.Count >= 1 && currentSequence.Count <= 3)
                {
                    if (!previousSequence.Any())
                    {
                        previousSequence = new List<int>(currentSequence);
                    }
                    else if ((previousSequence.Max() == currentSequence.Min() - 2) && (previousSequence.Count + currentSequence.Count >= 4))
                    {
                        if (HandAnalyzerHelpers.IsStraight(currentSequence)
                            && HandAnalyzerHelpers.IsStraight(previousSequence))
                        {
                            if (previousSequence.Contains(1))
                            {
                                previousSequence[previousSequence.IndexOf(1)] = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                            }

                            if (playerCards.All(p => currentSequence.Contains(p.RankNumericValue) || previousSequence.Contains(p.RankNumericValue)))
                            {
                                return true;
                            }
                        }
                    }
                }

                previousSequence = new List<int>(currentSequence);
                currentSequence.Clear();
                currentSequence.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawTwoCardGutShot;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsStraightDraw(hand.MaskValue, 0UL);
        }
    }

    public class StraightDrawOneCardOpenEndedAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)))
            {
                return false;
            }

            var rankList = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards).Where(x => x.Rank != Cards.Card.PossibleRanksHighCardFirst.First()).Select(x => x.RankNumericValue).Distinct().ToList();
            rankList.Sort();

            List<int> straightDraw = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!straightDraw.Any() || (rankList[i] == straightDraw.Last() + 1))
                {
                    straightDraw.Add(rankList[i]);
                    continue;
                }

                if (straightDraw.Count == 4)
                {
                    if (HandAnalyzerHelpers.IsStraight(straightDraw)
                        && playerCards.Any(x => straightDraw.Contains(x.RankNumericValue))
                        && playerCards.Any(x => !straightDraw.Contains(x.RankNumericValue)))
                    {
                        return true;
                    }
                }

                straightDraw.Clear();
                straightDraw.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawOneCardOpenEnded;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsOpenEndedStraightDraw(hand.MaskValue, 0UL);
        }
    }

    public class StraightDrawOneCardDoubleGutShotAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.RankNumericValue).Distinct().ToList();
            if (allCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
            {
                rankList.Add(1);
            }
            rankList.Sort();

            List<int> straightDraw = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!straightDraw.Any() || (rankList[i] == straightDraw.Last() + 1))
                {
                    straightDraw.Add(rankList[i]);
                    continue;
                }

                if (straightDraw.Count == 3)
                {
                    if (HandAnalyzerHelpers.IsStraight(straightDraw))
                    {
                        if (rankList.Any(x => x == straightDraw.Max() + 2) && rankList.Any(x => x == straightDraw.Min() - 2))
                        {
                            straightDraw.Add(straightDraw.Max() + 2);
                            straightDraw.Add(straightDraw.Min() - 2);

                            if (straightDraw.Contains(1))
                            {
                                straightDraw[straightDraw.IndexOf(1)] = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                            }

                            if (playerCards.Any(x => straightDraw.Contains(x.RankNumericValue)) && playerCards.Any(x => !straightDraw.Contains(x.RankNumericValue)))
                            {
                                return true;
                            }
                        }
                    }
                }

                straightDraw.Clear();
                straightDraw.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawOneCardDoubleGutShot;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsOpenEndedStraightDraw(hand.MaskValue, 0UL);
        }
    }


    public class StraightDrawOneCardGutShotAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.RankNumericValue).Distinct().ToList();
            if (allCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
            {
                rankList.Add(1);
            }
            rankList.Sort();

            List<int> currentSequence = new List<int>();
            List<int> previousSequence = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!currentSequence.Any() || (rankList[i] == currentSequence.Last() + 1))
                {
                    currentSequence.Add(rankList[i]);
                    continue;
                }

                if (currentSequence.Count >= 1 && currentSequence.Count <= 3)
                {
                    if (!previousSequence.Any())
                    {
                        previousSequence = new List<int>(currentSequence);
                    }
                    else if ((previousSequence.Max() == currentSequence.Min() - 2) && (previousSequence.Count + currentSequence.Count >= 4))
                    {
                        if (HandAnalyzerHelpers.IsStraight(currentSequence)
                            && HandAnalyzerHelpers.IsStraight(previousSequence))
                        {
                            if (previousSequence.Contains(1))
                            {
                                previousSequence[previousSequence.IndexOf(1)] = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                            }

                            if (playerCards.Any(p => currentSequence.Contains(p.RankNumericValue) || previousSequence.Contains(p.RankNumericValue))
                                && playerCards.Any(p => !currentSequence.Contains(p.RankNumericValue) && !previousSequence.Contains(p.RankNumericValue)))
                            {
                                return true;
                            }
                        }
                    }
                }

                previousSequence = new List<int>(currentSequence);
                currentSequence.Clear();
                currentSequence.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawOneCardGutShot;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return Hand.IsGutShotStraightDraw(hand.MaskValue, 0UL);
        }
    }

    public class StraightDrawTwoCardBackdoorAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (HandAnalyzerHelpers.IsPair(playerCards, 1))
            {
                return false;
            }

            if (playerCards.Any(p => boardCards.Any(b => b.RankNumericValue == p.RankNumericValue)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.RankNumericValue).Distinct().ToList();
            if (allCards.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First()))
            {
                rankList.Add(1);
            }
            rankList.Sort();

            List<int> currentSequence = new List<int>();
            List<int> previousSequence = new List<int>();
            for (int i = 0; i < rankList.Count; i++)
            {
                if (!currentSequence.Any() || (rankList[i] == currentSequence.Last() + 1))
                {
                    currentSequence.Add(rankList[i]);
                    continue;
                }

                if (currentSequence.Count >= 1 && currentSequence.Count <= 3)
                {
                    if (!previousSequence.Any())
                    {
                        previousSequence = new List<int>(currentSequence);
                    }
                    else if (currentSequence.Min() - previousSequence.Max() > 3)
                    {
                        if (previousSequence.Count() == 3)
                        {
                            if (previousSequence.Contains(1))
                            {
                                previousSequence[previousSequence.IndexOf(1)] = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                            }

                            if (playerCards.All(x => previousSequence.Contains(x.RankNumericValue)))
                            {
                                return true;
                            }
                        }
                    }
                    else if ((previousSequence.Count + currentSequence.Count) == 3)
                    {
                        if (HandAnalyzerHelpers.IsStraight(currentSequence)
                            && HandAnalyzerHelpers.IsStraight(previousSequence))
                        {
                            if (previousSequence.Contains(1))
                            {
                                previousSequence[previousSequence.IndexOf(1)] = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
                            }

                            if (playerCards.All(p => currentSequence.Contains(p.RankNumericValue) || previousSequence.Contains(p.RankNumericValue)))
                            {
                                return true;
                            }
                        }
                    }
                }

                previousSequence = new List<int>(currentSequence);
                currentSequence.Clear();
                currentSequence.Add(rankList[i]);
            }

            return false;
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawTwoCardBackdoor;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return true;
        }
    }

    public class StraightDrawNoStraightDrawAnalyzer : IAnalyzer
    {
        public bool Analyze(IEnumerable<Cards.Card> playerCards, BoardCards boardCards)
        {
            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return true;

            return !Hand.IsStraightDraw(string.Join("", playerCards), boardCards.ToString(), string.Empty);
        }

        public ShowdownHands GetRank()
        {
            return ShowdownHands.StraightDrawNoStraightDraw;
        }

        public bool IsValidAnalyzer(Hand hand)
        {
            return !Hand.IsStraightDraw(hand.MaskValue, 0UL);
        }
    }
}
