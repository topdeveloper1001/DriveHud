using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enums
{
    public enum BoardTextures
    {
        None = 0,
        HighCard,
        NoFlushPossible,
        FlushPossible,
        FourFlush,
        FlushOnBoard,
        Rainbow,
        TwoTone,
        TwoFlushDraws,
        ThreeTone,
        Monotone,
        Uncoordinated,
        OneGapper,
        TwoGapper,
        ThreeConnected,
        FourConnected,
        OpenEndedStraight,
        MadeStraight,
        OpenEndedBeatNuts,
        GutShotBeatNuts,
        NoPair,
        SinglePair,
        TwoPair,
        ThreeOfAKind,
        FourOfAKind,
        FullHouse,
        ExactCards,
    }
}
