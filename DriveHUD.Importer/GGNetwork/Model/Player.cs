//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class Player
    {
        public string Id { get; set; }

        public int Sequence { get; set; }

        public string NickName { get; set; }

        public string CountryCode { get; set; }

        public int AvatarId { get; set; }

        public int SeatIndex { get; set; }

        public int PositionType { get; set; }

        public bool IsSittingOut { get; set; }

        public IList<HoleCard> HoleCards { get; set; }

        public IList<RabbitCard> RabbitCards { get; set; }

        public bool IsWinner { get; set; }

        public bool IsShowCard { get; set; }

        public IList<object> ShowCardsIndex { get; set; }

        public bool IsDealtPlayer { get; set; }

        public bool IsFolded { get; set; }

        public int InitialBalance { get; set; }

        public int InitialBounty { get; set; }

        public int FinalBounty { get; set; }

        public int RebuyAmount { get; set; }

        public int RemoveAmount { get; set; }

        public int PostedAnte { get; set; }

        public int PostedSmallBlind { get; set; }

        public int PostedBigBlind { get; set; }

        public int PostedStraddle { get; set; }

        public int JackpotAmount { get; set; }

        public IList<object> JackpotHand { get; set; }

        public int ContributedPot { get; set; }

        public int TotalEarnedAmountIncludedRake { get; set; }

        public int JackpotTicketTemplateId { get; set; }

        public int JackpotTicketValue { get; set; }

        public string AvatarType { get; set; }

        public string ImageUrl { get; set; }

        public int TotalEarnedAmount { get; set; }

        public bool IsAmbassador { get; set; }
    }
}