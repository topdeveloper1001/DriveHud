//-----------------------------------------------------------------------
// <copyright file="DealerSeat.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Dealer set class
    /// </summary>
    internal class DealerSeat  
    {
        public int SeatNumber { get; set; }

        public DealerSeat()
        {
            this.SeatNumber = -1;
        }
       
        public override string ToString()
        {
            return string.Format("SeatNumber: {0}", SeatNumber);
        }
    }
}