using DriveHUD.Entities;
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
        public bool IsCustomProcessedDataLocationEnabled { get; set; }
        public string CustomProcessedDataLocation { get; set; }
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
        private EnumPokerSites _pokerSite;
        private ObservableCollection<string> _handHistoryLocationList;

        [XmlAttribute]
        public EnumPokerSites PokerSite
        {
            get
            {
                return _pokerSite;
            }

            set
            {
                _pokerSite = value;
            }
        }

        [XmlArray]
        public ObservableCollection<string> HandHistoryLocationList
        {
            get
            {
                return _handHistoryLocationList;
            }

            set
            {
                _handHistoryLocationList = value;
            }
        }

        public override object Clone()
        {
            var model = (SiteModel)this.MemberwiseClone();
            model.HandHistoryLocationList = new ObservableCollection<string>(this.HandHistoryLocationList);

            return model;
        }
    }
}
