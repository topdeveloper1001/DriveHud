//-----------------------------------------------------------------------
// <copyright file="IPokerConfiguration.cs" company="Ace Poker Solutions">
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
using System.Linq;

namespace Model.Site
{
    public class IPokerConfiguration : ISiteConfiguration
    {
        public IPokerConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Three,
                EnumTableType.Four,
                EnumTableType.Six,
                EnumTableType.Nine,
                EnumTableType.Ten
            };
        }

        public string HeroName
        {
            get; set;
        }

        public bool IsHandHistoryLocationRequired
        {
            get
            {
                return true;
            }
        }

        public bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }

        public string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/ipoker_logo.png";
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

        public EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.IPoker;
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

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }

        public string[] GetHandHistoryFolders()
        {
            return new string[0];
        }

        public ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            if (siteModel == null)
            {
                return null;
            }

            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                HandHistoryLocations = GetHandHistoryFolders().ToList(),
                IsDetected = false,
                IsEnabled = siteModel.Enabled
            };

            return validationResult;
        }
    }
}