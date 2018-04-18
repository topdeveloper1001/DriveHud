//-----------------------------------------------------------------------
// <copyright file="ExpectedValuePotItem.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace Model.ExpectedValueSolver
{
    internal class ExpectedValuePotItem
    {
        public string PlayerName { get; set; }

        public decimal Equity { get; set; }

        public decimal ExpectedValue { get; set; }

        public decimal NetWon { get; set; }

        public decimal EVDifference { get; set; }

        public int PotIndex { get; set; }
    }
}