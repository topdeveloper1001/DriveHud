//-----------------------------------------------------------------------
// <copyright file="AliasPlayer.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Entities
{
    public partial class AliasPlayer
    {
        public virtual int AliasId { get; set; }

        public virtual int PlayersId { get; set; }

        public override int GetHashCode()
        {
            return AliasId;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AliasPlayer;

            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return AliasId == other.AliasId &&
                PlayersId == other.PlayersId;
        }
    }
}