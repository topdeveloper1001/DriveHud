//-----------------------------------------------------------------------
// <copyright file="OmahaHiLoEvaluator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Solvers;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class OmahaHiLoEvaluator : OmahaEvaluator, IPokerEvaluator
    {
        protected override HandWinners GetWinnersInternal()
        {
            var hiComparer = new HiCardsComparer();
            var loComparer = new LoCardsComparer();

            var winners = new HandWinners
            {
                Hi = GetWinnersInternal(hiComparer),
                Lo = GetWinnersInternal(loComparer)
            };

            return winners;
        }
    }
}