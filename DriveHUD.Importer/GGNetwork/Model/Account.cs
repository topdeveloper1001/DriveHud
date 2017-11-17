//-----------------------------------------------------------------------
// <copyright file="Account.cs" company="Ace Poker Solutions">
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
    public class Account
    {
        public string UserId { get; set; }

        public string NickName { get; set; }

        public bool IsNickNameExists { get; set; }

        public string Username { get; set; }

        public string GpId { get; set; }

        public bool IsSuperUser { get; set; }

        public bool IsAmbassador { get; set; }

        public bool IsVIP { get; set; }

        public int AvatarId { get; set; }

        public string CountryCode { get; set; }

        public string BrandId { get; set; }

        public bool IsAccountLocked { get; set; }

        public bool IsImageLocked { get; set; }

        public string AvatarType { get; set; }

        public string ImageUrl { get; set; }

        public int DefaultWalletId { get; set; }

        public string WalletType { get; set; }

        public string CurrencyId { get; set; }

        public int Scale { get; set; }

        public bool IsAgreeTerms { get; set; }

        public List<object> RoomWallets { get; set; }

        public int Status { get; set; }

        public bool IsReviewingKyc { get; set; }

        public int TourneyWalletId { get; set; }

        public bool FishBuffetOptIn { get; set; }

        public FishBuffetRank FishBuffetRank { get; set; }
    }
}