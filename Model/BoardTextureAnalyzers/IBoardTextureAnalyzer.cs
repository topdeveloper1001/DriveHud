using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.BoardTextureAnalyzers
{
    public interface IBoardTextureAnalyzer
    {
        BoardTextures GetRank();

        bool Analyze(BoardCards boardCards, BoardTextureItem boardTexture);
    }
}
