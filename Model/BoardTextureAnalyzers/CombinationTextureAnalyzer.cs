using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;
using Model.Extensions;
using Model.HandAnalyzers;

namespace Model.BoardTextureAnalyzers
{
    public class NoPairTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet);
            var lastCard = board.Last();
            return !board.Take(board.Count - 1).Any(x => x.Rank == lastCard.Rank);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.NoPair;
        }
    }

    public class SinglePairTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet);
            var lastCard = board.Last();
            return board.Take(board.Count - 1).GroupBy(x => x.Rank).All(x => x.Count() == 1) && board.Take(board.Count - 1).Any(x => x.Rank == lastCard.Rank);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.SinglePair;
        }
    }

    public class TwoPairTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet);
            var lastCard = board.Last();
            var grouppedBoard = board.Take(board.Count - 1).GroupBy(x => x.Rank);
            if (grouppedBoard.Count(x => x.Count() > 1) > 1)
            {
                return false;
            }
            var pair = grouppedBoard.FirstOrDefault(x => x.Count() == 2);
            if (pair == null || pair.Key == lastCard.Rank)
            {
                return false;
            }

            return board.Take(board.Count - 1).Any(x => x.Rank == lastCard.Rank);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.TwoPair;
        }
    }

    public class ThreeOfAKindTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet);
            var lastCard = board.Last();
            var grouppedBoard = board.Take(board.Count - 1).GroupBy(x => x.Rank);
            if (grouppedBoard.Count(x => x.Count() > 1) > 1)
            {
                return false;
            }

            var pair = grouppedBoard.FirstOrDefault(x => x.Count() == 2);
            if (pair == null)
            {
                return false;
            }

            return pair.Key == lastCard.Rank;
        }


        public BoardTextures GetRank()
        {
            return BoardTextures.ThreeOfAKind;
        }
    }

    public class FourOfAKindTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            var board = boardCards.GetBoardOnStreet(boardTexture.TargetStreet);
            var lastCard = board.Last();
            var grouppedBoard = board.Take(board.Count - 1).GroupBy(x => x.Rank);
            if (grouppedBoard.Count(x => x.Count() > 1) > 1)
            {
                return false;
            }

            var trips = grouppedBoard.FirstOrDefault(x => x.Count() == 3);
            if (trips == null)
            {
                return false;
            }

            return trips.Key == lastCard.Rank;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.FourOfAKind;
        }
    }

    public class FullHouseTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsNofKind(boardCards, 2) && HandAnalyzerHelpers.IsNofKind(boardCards, 3);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.FullHouse;
        }
    }
}
