using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.OmahaHoleCardsAnalyzers
{
    public interface IOmahaHoleCardsAnalyzer
    {
        OmahaHoleCards GetRank();

        bool Analyze(IEnumerable<HandHistories.Objects.Cards.Card> cards, OmahaHandGridItem handGridItem);
    }
}
