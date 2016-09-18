//-----------------------------------------------------------------------
// <copyright file="PlayerAction.cs" company="Ace Poker Solutions">
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
    /// Player actions
    /// </summary>
    internal class PlayerAction  
    {
        public int SeatNumber { get; set; }

        public int PlayerID { get; set; }

        public int PlayerActionID { get; set; }

        public PlayerActionEnum PlayerActionEnum { get; set; }

        public PlayerAction()
        {
            this.SeatNumber         = -1;
            this.PlayerID           = -1;
            this.PlayerActionID     = -1;
            this.PlayerActionEnum   = PlayerActionEnum.None;
        }       

        private PlayerActionEnum GetPlayerActionEnum(int p)
        {
            return (PlayerActionEnum)this.PlayerActionID;
        }

        public override string ToString()
        {
            return string.Format("PlayerID: {0}; SeatNumber: {1}; PlayerActionID: {2}; PlayerActionEnum: {3}", 
                PlayerID, SeatNumber, PlayerActionID, PlayerActionEnum);
        }
    }
}