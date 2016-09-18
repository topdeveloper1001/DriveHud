//-----------------------------------------------------------------------
// <copyright file="HandHistoryRecord.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Entities
{
    public partial class HandHistoryRecord
    {
        public virtual int Id { get; set; }

        public virtual Players Player { get; set; }

        public virtual Gametypes GameType { get; set; }

        public virtual DateTime Time { get; set; }

        public virtual string Cards { get; set; }

        public virtual string Line { get; set; }

        public virtual string Board { get; set; }

        public virtual decimal NetWonInCents { get; set; }

        public virtual decimal BBinCents { get; set; }

        public virtual string Pos { get; set; }

        public virtual string FacingPreflop { get; set; }

        public virtual string Action { get; set; }

        public virtual string Allin { get; set; }

        public virtual decimal Equity { get; set; }

        public virtual decimal EquityDiff { get; set; }

        public virtual string PlayerName
        {
            get
            {
                return Player != null ? Player.Playername : string.Empty;
            }
        }

        public virtual decimal BB
        {
            get
            {
                return BBinCents / 100;
            }
        }

        public virtual decimal NetWon
        {
            get
            {
                return NetWonInCents / 100;
            }
        }
    }
}