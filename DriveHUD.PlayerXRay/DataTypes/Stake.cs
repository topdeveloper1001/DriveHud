#region Usings

using System.ComponentModel;

#endregion

namespace DriveHUD.PlayerXRay.DataTypes
{
    public class Stake : INotifyPropertyChanged
    {
        private bool m_isSelected;
        private string m_name;

        public TableTypeEnum TableType { get; set; }
        public TableSizeEnum TableSize { get; set; }

        public int ID { get; set; }
        public decimal StakeValue { get; set; }  

        public string Name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                ParseValues();
            }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void ParseValues()
        {
            if (Name.Contains("NL"))
                TableType = TableTypeEnum.NoLimit;
            else                                              //todo check how many TableType we need
                TableType = Name.Contains("PL") ? TableTypeEnum.PotLimit : TableTypeEnum.Limit;
     
            if (Name.Contains("6 max"))
                TableSize = TableSizeEnum.Players56;
            else if (Name.Contains("FR"))
                TableSize = TableSizeEnum.Player710;
            else if (Name.Contains("short"))
                TableSize = TableSizeEnum.Players34;
            else
                TableSize = TableSizeEnum.HeadsUp;
        }
    }
}