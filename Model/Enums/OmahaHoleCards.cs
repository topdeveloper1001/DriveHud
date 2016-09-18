using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enums
{
    public enum OmahaHoleCards
    {
        None = 0,
        NoPair,
        OnePair,
        TwoPairs,
        Trips,
        Quads,
        Rainbow,
        AceSuited,
        NoAceSuited,
        DoubleSuited,
        TwoCardWrap,
        ThreeCardWrap,
        FourCardWrap,
        CardStructureAces,
        CardStructureBroadways,
        CardStructureMidHands,
        CardStructureLowCards,
    }
}
