//-----------------------------------------------------------------------
// <copyright file="BetOnlineConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Settings;
using System;
using System.Collections.Generic;

namespace Model.Site
{
    public class BetOnlineConfiguration : ISiteConfiguration
    {
        private readonly string[] registryKeys = new[] { "BetOnline 0" };

        private const string heroName = "Hero";

        public BetOnlineConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Eight,
                EnumTableType.Nine,
                EnumTableType.Ten
            };

            HeroName = heroName;
        }

        public virtual EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.BetOnline;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        public string HeroName
        {
            get;
            set;
        }

        public bool IsHandHistoryLocationRequired
        {
            get
            {
                return false;
            }
        }

        public bool IsPrefferedSeatsAllowed
        {
            get
            {
                return false;
            }
        }

        public bool IsAutoCenterAllowed
        {
            get
            {
                return false;
            }
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public virtual string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/betonline_logo.png";
            }
        }

        public string[] GetHandHistoryFolders()
        {
            return new string[] { };
        }

        protected virtual string[] RegistryKeys
        {
            get
            {
                return registryKeys;
            }
        }

        public ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = RegistryUtils.UninstallRegistryKeysExist(RegistryKeys),
                IsEnabled = siteModel.Enabled,
            };

            return validationResult;
        }
    }
}