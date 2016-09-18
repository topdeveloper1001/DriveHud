//-----------------------------------------------------------------------
// <copyright file="PlayerRemoved.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Player removed command
    /// </summary>
    internal class PlayerRemoved
    {
        public int SeatNumber { get; set; }

        public bool Previous { get; set; }

        public PlayerRemoved()
        {
            SeatNumber = -1;
        }

        public override string ToString()
        {
            return string.Format("SeatNumber: {0}; Previous: {1}", SeatNumber, Previous);
        }
    }
}