//-----------------------------------------------------------------------
// <copyright file="GGNConfiguration.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;

namespace Model.Site
{
    public class GGNConfiguration : BaseSiteConfiguration, ISiteConfiguration
    {
        public GGNConfiguration()
        {
            tableTypes = new EnumTableType[]
            {
                EnumTableType.Six,
                EnumTableType.Nine
            };
        }

        public override string LogoSource
        {
            get
            {
                return string.Empty;
            }
        }

        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.GGN;
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

        public override string[] GetHandHistoryFolders()
        {
            return new string[0];
        }

        public override ISiteValidationResult ValidateSiteConfiguration(SiteModel siteModel)
        {
            var validationResult = new SiteValidationResult(Site)
            {
                IsNew = !siteModel.Configured,
                IsDetected = DetectSite(),
                IsEnabled = siteModel.Enabled,
            };

            return validationResult;
        }

        /// <summary>
        /// Detects whenever ggn poker client is installed
        /// </summary>
        /// <returns>True if installed, otherwise - false</returns>
        private bool DetectSite()
        {
            return true;
        }
    }
}