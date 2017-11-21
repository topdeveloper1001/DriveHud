//-----------------------------------------------------------------------
// <copyright file="Action.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class Action
    {
        public int PlayerIndex { get; set; }

        public int TableActionType { get; set; }

        public int TurnActionType { get; set; }

        public int AfterAction { get; set; }

        public int ActionAmount { get; set; }

        public bool IsAllIn { get; set; }

        public double SpentTotalTimeBank { get; set; }

        public int MessageType { get; set; }

        public int BubbleCategory { get; set; }

        public int BubbleIndex { get; set; }

        public bool IsOnStraddle { get; set; }

        public bool IsTimeOver { get; set; }
    }
}