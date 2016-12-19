//-----------------------------------------------------------------------
// <copyright file="Stack.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Globalization;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Stack command
    /// </summary>
    internal class Stack
    {
        public int SeatNumber { get; set; }

        public int PlayerID { get; set; }

        public decimal StackValue { get; set; }

        public Stack()
        {
            SeatNumber = -1;
            PlayerID = -1;
            StackValue = 0.0m;
        }

        public Stack(int seatNumber, int playerID, decimal stackValue)
        {
            SeatNumber = seatNumber;
            PlayerID = playerID;
            StackValue = stackValue;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SeatNumber: {0}; PlayerID: {1}; StackValue: {2}", SeatNumber, PlayerID, StackValue);
        }
    }
}