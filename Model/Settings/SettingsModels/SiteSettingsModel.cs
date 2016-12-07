using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var sites = new EnumPokerSites[] {
                EnumPokerSites.Ignition,
                EnumPokerSites.BetOnline,
                EnumPokerSites.TigerGaming,
                EnumPokerSites.SportsBetting,
                EnumPokerSites.PokerStars,
                EnumPokerSites.Poker888,
                EnumPokerSites.AmericasCardroom,
                EnumPokerSites.BlackChipPoker,
            };

            SitesModelList = sites.Select(x => new SiteModel
            {
                PokerSite = x,
                HandHistoryLocationList = new ObservableCollection<string>()
            }).ToArray();
        }

        public override object Clone()
        {
            var model = new SiteSettingsModel();
            model.IsProcessedDataLocationEnabled = this.IsProcessedDataLocationEnabled;
            model.ProcessedDataLocation = this.ProcessedDataLocation;

            for (int i = 0; i < model.SitesModelList.Count(); i++)
            {
                var siteModel = this.SitesModelList.FirstOrDefault(x => x.PokerSite == model.SitesModelList[i].PokerSite);
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

        [XmlArray]
        public ObservableCollection<string> HandHistoryLocationList { get; set; }

        [XmlArray]
        public List<PreferredSeatModel> PrefferedSeats { get; set; }

        public override object Clone()
        {
            var model = (SiteModel)this.MemberwiseClone();
            model.HandHistoryLocationList = new ObservableCollection<string>(this.HandHistoryLocationList);
            model.PrefferedSeats = PrefferedSeats.Where(x => x != null).Select(x => (PreferredSeatModel)x.Clone()).ToList();

            return model;
        }
    }

    [Serializable]
    public class PreferredSeatModel : SettingsBase
    {
        private EnumTableType _tableType;
        private bool _isPreferredSeatEnabled = false;
        private int _preferredSeat = -1;

        [XmlAttribute]
        public EnumTableType TableType
        {
            get
            {
                return _tableType;
            }

            set
            {
                _tableType = value;
            }
        }

        [XmlAttribute]
        public bool IsPreferredSeatEnabled
        {
            get
            {
                return _isPreferredSeatEnabled;
            }

            set
            {
                _isPreferredSeatEnabled = value;
            }
        }

        [XmlAttribute]
        public int PreferredSeat
        {
            get
            {
                return _preferredSeat;
            }

            set
            {
                _preferredSeat = value;
            }
        }
    }
}
