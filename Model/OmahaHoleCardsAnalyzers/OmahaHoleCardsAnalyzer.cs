using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.OmahaHoleCardsAnalyzers
{
    public static class OmahaHoleCardsAnalyzer
    {
        public static IOmahaHoleCardsAnalyzer[] GetDefaultOmahaHoleCardsAnalyzers()
        {
            return new IOmahaHoleCardsAnalyzer[]
            {
                new OmahaOnePairAnalyzer(),
                new OmahaNoPairAnalyzer(),
                new OmahaTwoPairsAnalyzer(),
                new OmahaTripsAnalyzer(),
                new OmahaQuadsAnalyzer(),
                new OmahaRainbowAnalyzer(),
                new OmahaAceSuitedAnalyzer(),
                new OmahaNoAceSuitedAnalyzer(),
                new OmahaDoubleSuitedAnalyzer(),
                new OmahaTwoCardWrapAnalyzer(),
                new OmahaThreeCardWrapAnalyzer(),
                new OmahaFourCardWrapAnalyzer(),
                new OmahaHoleCardStructureAcesAnalyzer(),
                new OmahaHoleCardStructureBroadwaysAnalyzer(),
                new OmahaHoleCardStructureMidHandAnalyzer(),
                new OmahaHoleCardStructureLowCardsAnalyzer(),
            };
        }
    }
}
