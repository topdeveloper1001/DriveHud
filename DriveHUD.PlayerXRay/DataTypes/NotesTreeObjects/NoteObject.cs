#region Usings

using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace AcePokerSolutions.DataTypes.NotesTreeObjects
{
    public class NoteObject : INotifyPropertyChanged
    {
        private bool m_isSelected;

        public NoteObject()
        {
            Settings = new NoteSettingsObject();
            DisplayedNote = "Unknown";
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayedNote { get; set; }
        
        public NoteSettingsObject Settings { get; set; }

        [XmlIgnore]
        public bool IsSelected
        {
            set
            {
                if (m_isSelected == value)
                    return;
                m_isSelected = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}