//-----------------------------------------------------------------------
// <copyright file="ISiteConfiguration.cs" company="Ace Poker Solutions">
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
using Model.Settings;
using System;
using System.Collections.Generic;

namespace Model.Site
{
    /// <summary>
    /// Interface of site configuration
    /// </summary>
    public interface ISiteConfiguration
    {
        #region Predefined properties

        EnumPokerSites Site { get; }

        IEnumerable<EnumTableType> TableTypes { get; }

        bool IsHandHistoryLocationRequired { get; }

        bool IsPrefferedSeatsAllowed { get; }

        bool IsAutoCenterAllowed { get; }

        string LogoSource { get; }

        #endregion

        #region Editable properties

        Dictionary<int, int> PreferredSeats { get; }

        string HeroName { get; set; }

        TimeSpan TimeZoneOffset { get; set; }

        #endregion

        #region Public methods

        string[] GetHandHistoryFolders();

        ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel);

        #endregion
    }
}