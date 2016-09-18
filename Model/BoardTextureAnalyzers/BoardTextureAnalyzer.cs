//-----------------------------------------------------------------------
// <copyright file="BoardTextureAnalyzer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace Model.BoardTextureAnalyzers
{
    public class BoardTextureAnalyzer
    {
        public static IBoardTextureAnalyzer[] GetDefaultAnalyzers()
        {
            return new IBoardTextureAnalyzer[]
            {
                new HighCardTextureAnalyzer(),
                new NoFlushPossibleTextureAnalyzer(),
                new FlushPossibleTextureAnalyzer(),
                new FourFlushTextureAnalyzer(),
                new FlushOnBoardTextureAnalyzer(),
                new RainbowTextureAnalyzer(),
                new TwoToneTextureAnalyzer(),
                new TwoFlushDrawsTextureAnalyzer(),
                new ThreeToneTextureAnalyzer(),
                new MonotoneTextureAnalyzer(),
                new UncoordinatedTextureAnalyzer(),
                new OneGapperTextureAnalyzer(),
                new TwoGapperTextureAnalyzer(),
                new ThreeConnectedTextureAnalyzer(),
                new FourConnectedTextureAnalyzer(),
                new OpenEndedStraightTextureAnalyzer(),
                new MadeStraightTextureAnalyzer(),
                new OpenEndedBeatNutsTextureAnalyzer(),
                new GutShotBeatNutsTextureAnalyzer(),
                new NoPairTextureAnalyzer(),
                new SinglePairTextureAnalyzer(),
                new TwoPairTextureAnalyzer(),
                new ThreeOfAKindTextureAnalyzer(),
                new FourOfAKindTextureAnalyzer(),
                new FullHouseTextureAnalyzer(),
            };
        }
    }
}