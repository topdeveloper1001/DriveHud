//-----------------------------------------------------------------------
// <copyright file="HandPhaseV2.cs" company="Ace Poker Solutions">
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
    /// Hand phases class
    /// </summary>
    internal class HandPhaseV2
    {
        public int HandPhaseID { get; set; }

        public HandPhaseEnum HandPhaseEnum { get; set; }

        public HandPhaseV2()
        {
            this.HandPhaseID = -1;
            this.HandPhaseEnum = HandPhaseEnum.NotEnoughPlayers;
        }

        public override string ToString()
        {
            return string.Format("HandPhaseEnum: {0};  HandPhaseID: {1}", HandPhaseEnum, HandPhaseID);
        }
    }
}