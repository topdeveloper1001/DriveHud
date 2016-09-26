using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public bool IsCustomProcessedDataLocationEnabled { get; set; }

        [XmlAttribute]
        public string CustomProcessedDataLocation { get; set; }

        [XmlArray]
        public SiteModel[] SitesModelList { get; set; }

        public SiteSettingsModel()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            IsCustomProcessedDataLocationEnabled = false;
            CustomProcessedDataLocation = StringFormatter.GetAppDataFolderPath();

            var sites = new EnumPokerSites[] { EnumPokerSites.Bovada, EnumPokerSites.BetOnline, EnumPokerSites.TigerGaming, EnumPokerSites.SportsBetting, EnumPokerSites.PokerStars };
            SitesModelList = sites.Select(x => new SiteModel
            {
                PokerSite = x,
                HandHistoryLocationList = new ObservableCollection<string>()
            }).ToArray();
        }

        public override object Clone()
        {
            var model = (SiteSettingsModel)this.MemberwiseClone();
            model.SitesModelList = this.SitesModelList.Select(x => (SiteModel)x.Clone()).ToArray();

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
