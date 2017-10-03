//-----------------------------------------------------------------------
// <copyright file="SiteValidationResult.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;

namespace Model.Site
{
    public class SiteValidationResult : ISiteValidationResult
    {
        public SiteValidationResult(EnumPokerSites pokerSite)
        {
            PokerSite = pokerSite;
        }

        public EnumPokerSites PokerSite { get; private set; }

        public bool IsNew { get; set; }

        public bool IsDetected { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsAutoCenter { get; set; }

        public bool FastPokerEnabled { get; set; }

        public bool HasIssue
        {
            get
            {
                return Issues != null && Issues.Count > 0;
            }
        }

        public List<string> Issues { get; set; } = new List<string>();

        public List<string> HandHistoryLocations { get; set; } = new List<string>();
    }
}