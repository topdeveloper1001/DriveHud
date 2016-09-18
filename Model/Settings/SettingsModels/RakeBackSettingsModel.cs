using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Settings
{
    [Serializable]
    public class RakeBackSettingsModel : SettingsBase
    {
        [XmlArray]
        public List<RakeBackModel> RakeBackList { get; set; }

        [XmlArray]
        public List<BonusModel> BonusesList { get; set; }

        public RakeBackSettingsModel()
        {
            RakeBackList = new List<RakeBackModel>();
            BonusesList = new List<BonusModel>();
        }

        public override object Clone()
        {
            var model = (RakeBackSettingsModel)this.MemberwiseClone();
            model.RakeBackList = RakeBackList.Where(x => x != null).Select(x => (RakeBackModel)x.Clone()).ToList();
            model.BonusesList = BonusesList.Where(x => x != null).Select(x => (BonusModel)x.Clone()).ToList();

            return model;
        }
    }

    [Serializable]
    public class RakeBackModel : SettingsBase
    {
        private string _rakeBackName;
        private string _player;
        private decimal _percentage;
        private DateTime _dateBegan;

        [XmlAttribute]
        public DateTime DateBegan
        {
            get { return _dateBegan; }
            set
            {
                if (_dateBegan == value) return;
                _dateBegan = value;
                OnPropertyChanged(nameof(DateBegan));
            }
        }

        [XmlAttribute]
        public decimal Percentage
        {
            get { return _percentage; }
            set
            {
                if (_percentage == value) return;
                _percentage = value;
                OnPropertyChanged(nameof(Percentage));
            }
        }

        [XmlAttribute]
        public string Player
        {
            get { return _player; }
            set
            {
                if (_player == value) return;
                _player = value;
                OnPropertyChanged(nameof(Player));
            }
        }

        [XmlText]
        public string RakeBackName
        {
            get { return _rakeBackName; }
            set
            {
                if (_rakeBackName == value) return;
                _rakeBackName = value;
                OnPropertyChanged(nameof(RakeBackName));
            }
        }
    }

    [Serializable]
    public class BonusModel : SettingsBase
    {
        private string _bonusName;
        private string _player;
        private decimal _amount;
        private DateTime _date;

        [XmlAttribute]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value) return;
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        [XmlAttribute]
        public string Player
        {
            get { return _player; }
            set
            {
                if (_player == value) return;
                _player = value;
                OnPropertyChanged(nameof(Player));
            }
        }

        [XmlText]
        public string BonusName
        {
            get { return _bonusName; }
            set
            {
                if (_bonusName == value) return;
                _bonusName = value;
                OnPropertyChanged(nameof(BonusName));
            }
        }

        [XmlAttribute]
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (_amount == value) return;
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
    }

}
