using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DriveHUD.PlayerXRay.BusinessHelper.TextureAnalyzers.Straight;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class StraightDrawTwoCardOpenEndedAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)))
            {
                return false;
            }

            var rankList = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards).Where(x => x.Rank != (int)DataTypes.Card.GetCardRank("A")).Select(x => x.Rank).Distinct().ToList();
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
                    if (HandAnalyzerHelpers.IsStraight(straightDraw) && playerCards.All(x => straightDraw.Contains(x.Rank)))
                    {
                        return true;
                    }
                }

                straightDraw.Clear();
                straightDraw.Add(rankList[i]);
            }

            return false;
        }
    }

    public class StraightDrawTwoCardGutShotAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.Rank).Distinct().ToList();
            if (allCards.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("A")))
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
                                previousSequence[previousSequence.IndexOf(1)] = (int)DataTypes.Card.GetCardRank("A");
                            }

                            if (playerCards.All(p => currentSequence.Contains(p.Rank) || previousSequence.Contains(p.Rank)))
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
    }

    public class StraightDrawOneCardOpenEndedAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)))
            {
                return false;
            }

            var rankList = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards).Where(x => x.Rank != (int)DataTypes.Card.GetCardRank("A")).Select(x => x.Rank).Distinct().ToList();
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
                        && playerCards.Any(x => straightDraw.Contains(x.Rank))
                        && playerCards.Any(x => !straightDraw.Contains(x.Rank)))
                    {
                        return true;
                    }
                }

                straightDraw.Clear();
                straightDraw.Add(rankList[i]);
            }

            return false;
        }
    }


    public class StraightDrawOneCardGutShotAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.Rank).Distinct().ToList();
            if (allCards.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("A")))
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
                                previousSequence[previousSequence.IndexOf(1)] = (int)DataTypes.Card.GetCardRank("A");
                            }

                            if (playerCards.Any(p => currentSequence.Contains(p.Rank) || previousSequence.Contains(p.Rank))
                                && playerCards.Any(p => !currentSequence.Contains(p.Rank) && !previousSequence.Contains(p.Rank)))
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
    }

    public class StraightDrawTwoCardBackdoorAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (HandAnalyzerHelpers.HasPair(playerCards, 1))
            {
                return false;
            }

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)))
            {
                return false;
            }

            var allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);
            var rankList = allCards.Select(x => x.Rank).Distinct().ToList();
            if (allCards.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("A")))
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
                                previousSequence[previousSequence.IndexOf(1)] = (int)DataTypes.Card.GetCardRank("A");
                            }

                            if (playerCards.All(x => previousSequence.Contains(x.Rank)))
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
                                previousSequence[previousSequence.IndexOf(1)] = (int)DataTypes.Card.GetCardRank("A");
                            }

                            if (playerCards.All(p => currentSequence.Contains(p.Rank) || previousSequence.Contains(p.Rank)))
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
    }

    public class StraightDrawNoStraightDrawAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (boardCards == null || boardCards.Count() + playerCards.Count < 4)
                return false;

            List<DataTypes.Card> allCards = HandAnalyzerHelpers.CombineCardLists(playerCards, boardCards);

            //List<DataTypes.Card> orderedCardsByRanks = allCards.GroupBy(x => x.Rank).Select(x => x.First()).ToList(); 



            foreach (CardRank cardRank in DataTypes.Card.CardRankList)
            {
                if (cardRank == CardRank.None)
                    continue;

                List<DataTypes.Card> checkStraight = new List<DataTypes.Card>();
                checkStraight.AddRange(allCards);
                checkStraight.Add(new DataTypes.Card(DataTypes.Card.GetCardRankString(cardRank) + "c"));
                List<DataTypes.Card> orderedCheckList = checkStraight.GroupBy(x => x.Rank).Select(x => x.First()).OrderBy(x => x.Rank).ToList();

                if (CheckIfHasStraight(orderedCheckList))
                    return false;
            }

            return true;
        }

        private bool CheckIfHasStraight(List<DataTypes.Card> cards)
        {
            if (cards.Count < 5)
                return false;

            //bool flag = false;
            for (int i = 0; i < cards.Count - 4; i++)
            {
                if (HandAnalyzerHelpers.IsStraight(cards.GetRange(i, 5), true) ||
                    HandAnalyzerHelpers.IsStraight(cards.GetRange(i, 5), false))
                {
                    return true;
                    //flag = true;
                    //break;
                }
            }
            //if (flag)
            //    return false;

            return false;
        }
    }
}
