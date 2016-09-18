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
    public class RainbowTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).All(x => x.Count() == 1);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.Rainbow;
        }
    }

    public class TwoFlushDrawsTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).Count(x=> x.Count() == 2) == 2;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.TwoFlushDraws;
        }
    }

    public class TwoToneTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).Count() == 2;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.TwoTone;
        }
    }

    public class ThreeToneTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).Count() == 3;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.ThreeTone;
        }
    }

    public class MonotoneTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return HandAnalyzerHelpers.IsFlush(boardCards.GetBoardOnStreet(boardTexture.TargetStreet));
        }

        public virtual BoardTextures GetRank()
        {
            return BoardTextures.Monotone;
        }
    }

    public class NoFlushPossibleTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return !boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).Any(x => x.Count() > 2);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.NoFlushPossible;
        }
    }

    public class FlushPossibleTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).Any(x => x.Count() == 3);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.FlushPossible;
        }
    }

    public class FourFlushTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if (!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet))
            {
                return false;
            }

            return boardCards.GetBoardOnStreet(boardTexture.TargetStreet).GroupBy(x => x.Suit).Any(x => x.Count() == 4);
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.FourFlush;
        }
    }

    public class FlushOnBoardTextureAnalyzer : MonotoneTextureAnalyzer
    {
        public override BoardTextures GetRank()
        {
            return BoardTextures.FlushOnBoard;
        }
    }
}
