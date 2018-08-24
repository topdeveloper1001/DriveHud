//-----------------------------------------------------------------------
// <copyright file="TournamentKey.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
    public class TournamentKey
    {
        public TournamentKey(int pokerSiteId, string tournamentNumber)
        {
            PokerSiteId = pokerSiteId;
            TournamentNumber = tournamentNumber;
        }

        public string TournamentNumber { get; private set; }

        public int PokerSiteId { get; private set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = (hash * 31) + PokerSiteId.GetHashCode();

                if (TournamentNumber != null)
                {
                    hash = (hash * 31) + TournamentNumber.GetHashCode();
                }

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TournamentKey);
        }

        private bool Equals(TournamentKey key)
        {
            return key != null && key.PokerSiteId == PokerSiteId && key.TournamentNumber == TournamentNumber;
        }
    }
}