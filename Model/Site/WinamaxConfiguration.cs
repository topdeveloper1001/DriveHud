//-----------------------------------------------------------------------
// <copyright file="WinamaxConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Site
{
    public class WinamaxConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        private readonly string[] possibleFolders = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Winamax Poker", "accounts")
        };

        private readonly string[] subFolders = new[]
        {
            "history",
        };

        public WinamaxConfiguration()
        {
            prefferedSeat = new Dictionary<int, int>();

            tableTypes = new EnumTableType[]
            {              
                EnumTableType.Four                
            };
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.Winamax;
            }
        }

        private readonly IEnumerable<EnumTableType> tableTypes;

        public override IEnumerable<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        private readonly Dictionary<int, int> prefferedSeat;

        public override Dictionary<int, int> PreferredSeats
        {
            get
            {
                return prefferedSeat;
            }
        }

        public override bool IsHandHistoryLocationRequired
        {
            get
            {
                return true;
            }
        }

        public override bool IsPrefferedSeatsAllowed
        {
            get
            {
                return true;
            }
        }
        public override string LogoSource
        {
            get
            {
                return "/DriveHUD.Common.Resources;Component/images/SiteLogos/Winamax_logo.png";
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            var handHistoryFolders = new List<string>();

            foreach (var folder in possibleFolders)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                try
                {

                    var hhDirs = Directory.EnumerateDirectories(folder, "*", SearchOption.AllDirectories)
                        .Where(x => subFolders.Any(sf => x.EndsWith(sf, StringComparison.OrdinalIgnoreCase)))
                        .ToArray();

                    handHistoryFolders.AddRange(hhDirs);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Couldn't get hand history locations for {Site} at {folder}", e);
                }
            }

            return handHistoryFolders.Distinct().ToArray();
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            if (siteModel == null)
            {
                return null;
            }

            var handHistoriesLocation = GetHandHistoryFolders();

            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = handHistoriesLocation.Length > 0,
                IsEnabled = siteModel.Enabled,
                HandHistoryLocations = handHistoriesLocation.ToList()
            };

            return validationResult;
        }
    }
}