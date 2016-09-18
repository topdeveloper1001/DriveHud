//-----------------------------------------------------------------------
// <copyright file="PlayerLastActionAndStack.cs" company="Ace Poker Solutions">
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
    internal class PlayerLastActionAndStack
    {
        public PlayerActionEnum Action { get; set; }

        public decimal StackValue { get; set; }

        public PlayerLastActionAndStack()
        {
            this.Action = PlayerActionEnum.None;
            this.StackValue = 0.0m;
        }

        public PlayerLastActionAndStack(PlayerActionEnum pa, decimal stack)
        {
            this.Action = pa;
            this.StackValue = stack;
        }
    }
}