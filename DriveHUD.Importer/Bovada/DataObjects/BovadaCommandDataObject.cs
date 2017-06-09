//-----------------------------------------------------------------------
// <copyright file="BovadaCommandDataObject.cs" company="Ace Poker Solutions">
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
    /// Class represents command of Bovada stream data object
    /// </summary>
    public class BovadaCommandDataObject
    {
        public string pid { get; set; }

        public uint tableNo { get; set; }

        public uint stageNo { get; set; }

        public object account { get; set; }

        public object bet { get; set; }

        public object btn { get; set; }

        public object raise { get; set; }

        public int firstSeat { get; set; }

        public int seat { get; set; }

        public int tableState { get; set; }

        #region Pocket Cards

        public int[] Seat1 { get; set; }

        public int[] Seat2 { get; set; }

        public int[] Seat3 { get; set; }

        public int[] Seat4 { get; set; }

        public int[] Seat5 { get; set; }

        public int[] Seat6 { get; set; }

        public int[] Seat7 { get; set; }

        public int[] Seat8 { get; set; }

        public int[] Seat9 { get; set; }

        #endregion

        #region Community cards

        public int[] bcard { get; set; }

        public object card { get; set; }

        public int pos { get; set; }

        #endregion

        public int state { get; set; }

        public int type { get; set; }

        public object regSeatNo { get; set; }

        public string nickname { get; set; }

        public int maxSeat { get; set; }

        public int buyin { get; set; }

        public int buyinFee { get; set; }

        public object ante { get; set; }

        public int place { get; set; }

        public object dead { get; set; }

        public int rank { get; set; }

        public string prize { get; set; }
    }
}