using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;
using Model.Extensions;
using Cards = HandHistories.Objects.Cards;
using Model.HandAnalyzers;

namespace Model.BoardTextureAnalyzers
{
    public class UncoordinatedTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet).OrderBy(x => x.RankNumericValue).ToList();
            if (board.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.First())
                && board.Any(x => x.Rank == Cards.Card.PossibleRanksHighCardFirst.Last()))
            {
                return false;
            }

            for (int i = 1; i < board.Count; i++)
            {
                if (Math.Abs(board[i - 1].RankNumericValue - board[i].RankNumericValue) == 1)
                {
                    return false;
                }
            }

            return true;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.Uncoordinated;
        }
    }

    public class OneGapperTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet).OrderBy(x => x.RankNumericValue).ToList();
            if (board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("A"))
                && board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("3")))
            {
                return true;
            }

            for (int i = 1; i < board.Count; i++)
            {
                if (Math.Abs(board[i - 1].RankNumericValue - board[i].RankNumericValue) == 2)
                {
                    return true;
                }
            }

            return false;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.OneGapper;
        }
    }

    public class TwoGapperTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet).OrderBy(x => x.RankNumericValue).ToList();
            if (board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("A"))
                && board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("4")))
            {
                return true;
            }

            for (int i = 1; i < board.Count; i++)
            {
                if (Math.Abs(board[i - 1].RankNumericValue - board[i].RankNumericValue) == 3)
                {
                    return true;
                }
            }

            return false;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.TwoGapper;
        }
    }

    public class ThreeConnectedTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet).OrderBy(x => x.RankNumericValue).ToList();
            var orderedRanks = board.Select(x => x.RankNumericValue).ToList();
            if (board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("A"))
                && board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("2")))
            {
                orderedRanks.Insert(0, 1);
            }

            int count = 0;
            for (int i = 1; i < orderedRanks.Count; i++)
            {
                if (Math.Abs(orderedRanks[i - 1] - orderedRanks[i]) == 1)
                {
                    count++;
                }
                else if (count == 2)
                {
                    return true;
                }
                else
                {
                    count = 0;
                }
            }

            return count == 2;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.ThreeConnected;
        }
    }

    public class FourConnectedTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet).OrderBy(x => x.RankNumericValue).ToList();
            var orderedRanks = board.Select(x => x.RankNumericValue).ToList();
            if (board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("A"))
                && board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("2")))
            {
                orderedRanks.Insert(0, 1);
            }

            int count = 0;
            for (int i = 1; i < orderedRanks.Count; i++)
            {
                if (Math.Abs(orderedRanks[i - 1] - orderedRanks[i]) == 1)
                {
                    count++;
                }
                else if (count == 3)
                {
                    return true;
                }
                else
                {
                    count = 0;
                }
            }

            return count == 3;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.FourConnected;
        }
    }

    public class OpenEndedStraightTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            var boardTextureItem = boardTexture as StraightBoardTextureItem;
            if (boardTextureItem == null || !CardHelper.IsStreetAvailable(boardCards.ToString(), boardTextureItem.TargetStreet))
            {
                return false;
            }

            int numberOfOpenEndedStraights = 0;

            var orderedRanks = BoardTextureAnalyzerHelpers.GetOrderedBoardNumericRanks(boardCards, boardTextureItem);
            if (orderedRanks.Count < 2)
            {
                return false;
            }

            int firstRank = orderedRanks.Max() + 2;
            while (firstRank > orderedRanks.Min() - 2)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                var secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank - 1);
                if (secondRank <= 0)
                {
                    break;
                }

                var connectedList = new List<int>(orderedRanks);
                connectedList.Add(secondRank);
                connectedList = HandAnalyzerHelpers.GetConnectedCards(firstRank, connectedList);

                if (connectedList.Count > 3)
                {
                    numberOfOpenEndedStraights += HandAnalyzerHelpers.IsStraight(connectedList.OrderByDescending(x => x).Take(4)) ? 1 : 0;
                }
                firstRank--;
            }

            return BoardTextureAnalyzerHelpers.CheckEquality(boardTextureItem.SelectedSign.Key, numberOfOpenEndedStraights, boardTextureItem.SelectedNumber);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.OpenEndedStraight;
        }
    }

    public class MadeStraightTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            var boardTextureItem = boardTexture as StraightBoardTextureItem;
            if (boardTextureItem == null || !CardHelper.IsStreetAvailable(boardCards.ToString(), boardTextureItem.TargetStreet))
            {
                return false;
            }

            int numberOfOpenEndedStraights = 0;

            var orderedRanks = BoardTextureAnalyzerHelpers.GetOrderedBoardNumericRanks(boardCards, boardTextureItem);
            if (orderedRanks.Count < 2)
            {
                return false;
            }

            int firstRank = orderedRanks.Max() + 2;
            while (firstRank > orderedRanks.Min() - 2)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                var secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank - 1);
                if (secondRank <= 0)
                {
                    break;
                }

                var connectedList = new List<int>(orderedRanks);
                connectedList.Add(secondRank);
                connectedList = HandAnalyzerHelpers.GetConnectedCards(firstRank, connectedList);

                if (connectedList.Count > 4)
                {
                    numberOfOpenEndedStraights += HandAnalyzerHelpers.IsStraight(connectedList.OrderByDescending(x => x).Take(5)) ? 1 : 0;
                }
                firstRank--;
            }

            return BoardTextureAnalyzerHelpers.CheckEquality(boardTextureItem.SelectedSign.Key, numberOfOpenEndedStraights, boardTextureItem.SelectedNumber);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.MadeStraight;
        }
    }

    public class OpenEndedBeatNutsTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            var boardTextureItem = boardTexture as StraightBoardTextureItem;
            if (boardTextureItem == null || !CardHelper.IsStreetAvailable(boardCards.ToString(), boardTextureItem.TargetStreet))
            {
                return false;
            }

            int numberOfOpenEndedStraights = 0;

            var orderedRanks = BoardTextureAnalyzerHelpers.GetOrderedBoardNumericRanks(boardCards, boardTextureItem);
            if (orderedRanks.Count < 2)
            {
                return false;
            }

            int firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, orderedRanks.Max() + 2);
            int initFirstRank = firstRank;
            int nutRank = 0;
            bool isNutFound = false;
            while (firstRank > orderedRanks.Min() - 2)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                if (isNutFound && firstRank <= nutRank)
                {
                    break;
                }

                var secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank - 1);
                if (secondRank <= 0)
                {
                    break;
                }

                var connectedList = new List<int>(orderedRanks);
                connectedList.Add(secondRank);
                connectedList = HandAnalyzerHelpers.GetConnectedCards(firstRank, connectedList);

                if (connectedList.Count > (isNutFound ? 3 : 4))
                {
                    var hand = connectedList.OrderByDescending(x => x).Take(isNutFound ? 4 : 5);
                    bool isStraight = HandAnalyzerHelpers.IsStraight(hand);
                    if (isStraight)
                    {
                        if (!isNutFound)
                        {
                            isNutFound = true;
                            firstRank = initFirstRank;
                            nutRank = hand.Max();
                            continue;
                        }
                        else
                        {
                            numberOfOpenEndedStraights++;
                        }
                    }
                }
                firstRank--;
            }

            return BoardTextureAnalyzerHelpers.CheckEquality(boardTextureItem.SelectedSign.Key, numberOfOpenEndedStraights, boardTextureItem.SelectedNumber);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.OpenEndedBeatNuts;
        }
    }

    public class GutShotBeatNutsTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            var boardTextureItem = boardTexture as StraightBoardTextureItem;
            if (boardTextureItem == null || !CardHelper.IsStreetAvailable(boardCards.ToString(), boardTextureItem.TargetStreet))
            {
                return false;
            }

            int numberOfOpenEndedStraights = 0;

            var orderedRanks = BoardTextureAnalyzerHelpers.GetOrderedBoardNumericRanks(boardCards, boardTextureItem);
            if (orderedRanks.Count < 2)
            {
                return false;
            }

            int nutRank = GetNutHighCard(orderedRanks);
            if(nutRank <= 0)
            {
                return false;
            }

            int firstRank = orderedRanks.Max() + 3;
            int secondRank = firstRank - 1;
            while (firstRank > nutRank)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, secondRank);

                if (firstRank == secondRank)
                {
                    secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, secondRank - 1);
                }

                if (secondRank <= 0 || secondRank < orderedRanks.Min() - 3 )
                {
                    firstRank--;
                    secondRank = firstRank;
                    continue;
                }

                var tempList = new List<int>(orderedRanks);
                tempList.Add(firstRank);
                tempList.Add(secondRank);
                tempList = tempList.Where(x => x <= firstRank).ToList();
                if (tempList.Count > 3)
                {
                    var hand = tempList.OrderByDescending(x => x).Take(4);
                    var isGutShot = hand.Max() - hand.Min() == 4;
                    if (isGutShot)
                    {
                        numberOfOpenEndedStraights++;
                    }
                }
                secondRank--;
            }

            return BoardTextureAnalyzerHelpers.CheckEquality(boardTextureItem.SelectedSign.Key, numberOfOpenEndedStraights, boardTextureItem.SelectedNumber);
        }

        private int GetNutHighCard(List<int> orderedRanks)
        {
            int firstRank = orderedRanks.Max() + 2;
            while (firstRank > orderedRanks.Min() - 2)
            {
                firstRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank);
                var secondRank = BoardTextureAnalyzerHelpers.GetHighestUniqueRankNumberForList(orderedRanks, firstRank - 1);
                if (secondRank <= 0)
                {
                    break;
                }

                var connectedList = new List<int>(orderedRanks);
                connectedList.Add(secondRank);
                connectedList = HandAnalyzerHelpers.GetConnectedCards(firstRank, connectedList);

                if (connectedList.Count > 4)
                {
                    var hand = connectedList.OrderByDescending(x => x).Take(5);
                    if (HandAnalyzerHelpers.IsStraight(hand))
                    {
                        return hand.Max();
                    }
                }
                firstRank--;
            }

            return -1;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.GutShotBeatNuts;
        }
    }
}
