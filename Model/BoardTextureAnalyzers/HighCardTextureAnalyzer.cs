using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;
using Model.HandAnalyzers;
using Model.Extensions;

namespace Model.BoardTextureAnalyzers
{
    public class HighCardTextureAnalyzer : IBoardTextureAnalyzer
    {
        public bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture)
        {
            if(!CardHelper.IsStreetAvailable(boardCards.ToString(), boardTexture.TargetStreet) || !(boardTexture is HighCardBoardTextureItem))
            {
                return false;
            }

            var highCardItem = boardTexture as HighCardBoardTextureItem;
            var board = boardCards.GetBoardOnStreet(highCardItem.TargetStreet);

            return board.Aggregate((i1, i2) => i1.RankNumericValue > i2.RankNumericValue ? i1 : i2).Rank == highCardItem.SelectedRank;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.HighCard;
        }
    }
}
