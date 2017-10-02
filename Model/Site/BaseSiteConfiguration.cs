//-----------------------------------------------------------------------
// <copyright file="BaseSiteConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
    /// Base site configuration class
    /// </summary>
    public abstract class BaseSiteConfiguration : ISiteConfiguration
    {
        public virtual bool FastPokerAllowed
        {
            get
            {
                return false;
            }
        }

        public virtual string FastPokerModeName
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual string HeroName
        {
            get;
            set;
        }

        public virtual bool IsAutoCenterAllowed
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsHandHistoryLocationRequired
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsPrefferedSeatsAllowed
        {
            get
            {
                return false;
            }
        }

        public abstract string LogoSource
        {
            get;
        }

        public virtual Dictionary<int, int> PreferredSeats
        {
            get
            {
                return new Dictionary<int, int>();
            }
        }

        public abstract EnumPokerSites Site
        {
            get;
        }

        public abstract IEnumerable<EnumTableType> TableTypes
        {
            get;
        }

        public virtual TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public abstract string[] GetHandHistoryFolders();

        public abstract ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel);
    }
}