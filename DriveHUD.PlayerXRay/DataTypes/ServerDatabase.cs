#region Usings

using System.ComponentModel;

#endregion

namespace DriveHUD.PlayerXRay.DataTypes
{
    public class ServerDatabase : INotifyPropertyChanged
    {
        #region Delegates

        public delegate void DatabaseSelectedDelegate(ServerDatabase db);

        #endregion

        private bool m_isSelected;
        public string Name { get; set; }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected == value)
                    return;

                m_isSelected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                if (DatabaseSelected != null && value)
                    DatabaseSelected(this);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public event DatabaseSelectedDelegate DatabaseSelected;

        public void Unselect()
        {
            m_isSelected = false;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
        }
    }
}