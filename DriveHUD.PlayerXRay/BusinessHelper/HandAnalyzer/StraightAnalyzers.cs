using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class StraightTwoCardNutAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)) ||
                HandAnalyzerHelpers.HasPair(playerCards, 1))
                return false;


            var sortedBoard = boardCards.OrderBy(x => x.Rank);

            for (int i = sortedBoard.Count() - 1; i >= 2; i--)
            {
                var curCards = sortedBoard.Skip(i - 2);
                if (curCards.Count() >= 3)
                {
                    curCards = curCards.Take(3);
                    var nutStraight = GetNutStraightForCards(curCards);
                    if (nutStraight != null)
                    {
                        var requiredPocketCards = nutStraight.Where(x => !curCards.Any(c => c.Rank == x));
                        if (playerCards.All(x => requiredPocketCards.Contains(x.Rank)))
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

        private IEnumerable<int> GetNutStraightForCards(IEnumerable<DataTypes.Card> boardCards)
        {
            if (boardCards == null || boardCards.Count() != 3)
            {
                throw new ArgumentException("Method requires 3 board cards", "boardCards");
            }

            var boardRanks = boardCards.Select(x => x.Rank).ToList();
            boardRanks.Sort();

            int highestRank = (int)DataTypes.Card.GetCardRank("A");

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
    }

    public class StraightTwoCardAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (playerCards.Any(p => boardCards.Any(b => b.Rank == p.Rank)) || HandAnalyzerHelpers.HasPair(playerCards, 1))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.Rank).ToList();

            if (sortedBoard.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("A"))
                && sortedBoard.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("2")))
            {
                sortedBoard.Insert(0, new DataTypes.Card("Ac"));
            }

            for (int i = sortedBoard.Count() - 1; i >= 2; i--)
            {
                var curCards = sortedBoard.Skip(i - 2).ToList();
                if (curCards.Count() >= 3)
                {
                    curCards.AddRange(playerCards);
                    curCards = curCards.OrderByDescending(x => x.Rank).Take(5).ToList();
                    if (HandAnalyzerHelpers.IsStraight(curCards, true))
                    {
                        return playerCards.All(p => curCards.Any(c => c.Rank == p.Rank));
                    }
                }
            }

            return false;
        }
    }

    public class StraightOneCardNutAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            var ten = (int)DataTypes.Card.GetCardRank("T");
            var ace = (int)DataTypes.Card.GetCardRank("A");
            var sortedBoardRanks = boardCards.Select(x => x.Rank).Where(x => x >= ten && x <= ace).Distinct().ToList();
            sortedBoardRanks.Sort();

            if (sortedBoardRanks.Count() == 4)
            {
                for (int i = ten; i <= ace; i++)
                {
                    if (sortedBoardRanks.Contains(i))
                    {
                        continue;
                    }

                    if (playerCards.Any(x => x.Rank == i))
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
    }

    public class StraightOneCardAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            return playerCards.Any(x => HandAnalyzerHelpers.IsOneCardStraight(x, boardCards));
        }     
    }

    public class StraightOnBoardAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (boardCards == null || boardCards.Count != 5)
            {
                return false;
            }

            if (HandAnalyzerHelpers.IsStraight(boardCards, true))
            {
                if (boardCards.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("A")) &&
                    boardCards.Any(x => x.Rank == (int)DataTypes.Card.GetCardRank("2")))
                {
                    return !playerCards.Any(x => x.Rank == (boardCards
                        .Where(b => b.Rank != (int)DataTypes.Card.GetCardRank("A"))
                        .Max(m => m.Rank) + 1));
                }
                else
                {
                    return playerCards.All(x => x.Rank != boardCards.Max(m => m.Rank) + 1);
                }
            }

            return false;
        }
    }

}
