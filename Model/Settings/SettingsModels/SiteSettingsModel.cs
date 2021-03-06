﻿//-----------------------------------------------------------------------
// <copyright file="SiteSettingsModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Model.Settings
{
    [Serializable]
    public class SiteSettingsModel : SettingsBase
    {
        [XmlAttribute]
        public bool IsProcessedDataLocationEnabled { get; set; }

        [XmlAttribute]
        public string ProcessedDataLocation { get; set; }

        [XmlArray]
        public SiteModel[] SitesModelList { get; set; }

        public SiteSettingsModel()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsProcessedDataLocationEnabled = true;
            ProcessedDataLocation = Path.Combine(StringFormatter.GetAppDataFolderPath(), "ProcessedData");

            var sites = new[]
            {
                EnumPokerSites.Ignition,
                EnumPokerSites.BetOnline,
                EnumPokerSites.TigerGaming,
                EnumPokerSites.SportsBetting,
                EnumPokerSites.SpartanPoker,
                EnumPokerSites.PokerStars,
                EnumPokerSites.Poker888,
                EnumPokerSites.AmericasCardroom,
                EnumPokerSites.BlackChipPoker,
                EnumPokerSites.TruePoker,
                EnumPokerSites.YaPoker,
                EnumPokerSites.WinningPokerNetwork,
                EnumPokerSites.PartyPoker,
                EnumPokerSites.IPoker,
                EnumPokerSites.Horizon,
                EnumPokerSites.Winamax,
                EnumPokerSites.Adda52,
                EnumPokerSites.PokerBaazi
            };

            SitesModelList = sites.Select(x => new SiteModel
            {
                PokerSite = x,
                HandHistoryLocationList = new ObservableCollection<string>()
            }).ToArray();
        }

        public override object Clone()
        {
            var model = new SiteSettingsModel
            {
                IsProcessedDataLocationEnabled = IsProcessedDataLocationEnabled,
                ProcessedDataLocation = ProcessedDataLocation
            };

            for (int i = 0; i < model.SitesModelList.Count(); i++)
            {
                var siteModel = SitesModelList.FirstOrDefault(x => x.PokerSite == model.SitesModelList[i].PokerSite);

                if (siteModel != null)
                {
                    model.SitesModelList[i] = (SiteModel)siteModel.Clone();
                }
            }

            return model;
        }
    }

    [Serializable]
    public class SiteModel : SettingsBase
    {
        public SiteModel()
        {
            PrefferedSeats = new List<PreferredSeatModel>();
        }

        [XmlAttribute]
        public EnumPokerSites PokerSite { get; set; }

        private bool enabled;

        [XmlAttribute]
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled == value)
                {
                    return;
                }

                enabled = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public bool Configured { get; set; }

        [XmlArray]
        public ObservableCollection<string> HandHistoryLocationList { get; set; }

        [XmlArray]
        public List<PreferredSeatModel> PrefferedSeats { get; set; }

        [XmlAttribute]
        public bool IsAutoCenter { get; set; }

        [XmlAttribute]
        public bool FastPokerEnabled { get; set; }

        [XmlAttribute]
        public string HeroName { get; set; }

        public override object Clone()
        {
            var model = (SiteModel)MemberwiseClone();
            model.HandHistoryLocationList = new ObservableCollection<string>(HandHistoryLocationList);
            model.PrefferedSeats = PrefferedSeats.Where(x => x != null).Select(x => (PreferredSeatModel)x.Clone()).ToList();

            return model;
        }
    }

    [Serializable]
    public class PreferredSeatModel : SettingsBase
    {
        [XmlAttribute]
        public EnumTableType TableType { get; set; }

        [XmlAttribute]
        public bool IsPreferredSeatEnabled { get; set; } = false;

        [XmlAttribute]
        public int PreferredSeat { get; set; } = -1;
    }
}