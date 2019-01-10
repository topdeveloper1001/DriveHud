//-----------------------------------------------------------------------
// <copyright file="PokerBaaziHandBuilder.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.Hand;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziHandBuilder : IPokerBaaziHandBuilder
    {
        public PokerBaaziInitResponse GetInitResponse(uint roomId)
        {
            throw new NotImplementedException();
        }

        public bool TryBuild(PokerBaaziPackage package, out HandHistory handHistory)
        {
            throw new NotImplementedException();
        }
    }
}