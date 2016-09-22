using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Model.Settings
{
    [Serializable]
    public class PreferredSeatSettingsModel : SettingsBase
    {
        [XmlArray]
        public List<PreferredSeatModel> PrefferedSeats;

        public PreferredSeatSettingsModel()
        {
            PrefferedSeats = new List<PreferredSeatModel>();
        }

        public override object Clone()
        {
            var model = (PreferredSeatSettingsModel)this.MemberwiseClone();
            model.PrefferedSeats = PrefferedSeats.Where(x => x != null).Select(x => (PreferredSeatModel)x.Clone()).ToList();

            return model;
        }
    }

    public class PreferredSeatModel : SettingsBase
    {
        private EnumTableType _tableType;
        private EnumPokerSites _pokerSite;
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
