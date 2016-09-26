//-----------------------------------------------------------------------
// <copyright file="BovadaConfiguration.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Model.Enums;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System.Linq;
using DriveHUD.Entities;

namespace Model.Site
{
    public class BovadaConfiguration : ISiteConfiguration
    {
        private const string heroName = "Hero";

        public BovadaConfiguration()
        {
            tableTypes = new EnumTableType[]
            {
                EnumTableType.HU,
                EnumTableType.Six,
                EnumTableType.Nine
            };

            HeroName = heroName;
        }

        public EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.Bovada;
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

        public virtual Dictionary<int, int> PreferredSeats
        {
            get
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().SiteSettings.SitesModelList.FirstOrDefault(x=> x.PokerSite == Site)?.PrefferedSeats;
                var filteredSeatsList = preferredSeats?.Where(x => x.IsPreferredSeatEnabled && x.PreferredSeat != -1);
                var seatsDictonary = new Dictionary<int, int>();
                foreach(var preferredSeat in filteredSeatsList)
                {
                    seatsDictonary.Add((int)preferredSeat.TableType, preferredSeat.PreferredSeat);
                }

                return seatsDictonary;
            }
        }

        public TimeSpan TimeZoneOffset
        {
            get;
            set;
        }
    }
}