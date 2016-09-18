﻿//-----------------------------------------------------------------------
// <copyright file="PlayerAdded.cs" company="Ace Poker Solutions">
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
    /// Player added command
    /// </summary>
    internal class PlayerAdded
    {
        public int SeatNumber { get; set; }

        public int PlayerID { get; set; }

        public bool IsHero { get; set; }

        public bool Previous { get; set; }

        public PlayerAdded()
        {
            SeatNumber = -1;
            PlayerID   = -1;
            IsHero     = false;
        }
      
        public override string ToString()
        {
            return string.Format("PlayerID: {0}; SeatNumber: {1}; IsHero: {2}; Previous: {3}", PlayerID, SeatNumber, IsHero, Previous);
        }
    }
}