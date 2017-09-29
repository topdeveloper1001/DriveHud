//-----------------------------------------------------------------------
// <copyright file="ISiteValidationResult.cs" company="Ace Poker Solutions">
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
    /// <summary>
    /// Represents the result of the validation of the site
    /// </summary>
    public interface ISiteValidationResult
    {
        EnumPokerSites PokerSite { get; }

        bool IsNew { get; set; }

        bool HasIssue { get; }

        bool IsDetected { get; set; }

        bool IsEnabled { get; }

        bool IsAutoCenter { get; set; }

        bool FastPokerEnabled { get; set; }

        List<string> Issues { get; set; }

        List<string> HandHistoryLocations { get; set; }
    }
}