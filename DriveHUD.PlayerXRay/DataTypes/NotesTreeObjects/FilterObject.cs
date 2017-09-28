#region Usings

using System;
using System.ComponentModel;
using System.Xml.Serialization;

#endregion

namespace AcePokerSolutions.DataTypes.NotesTreeObjects
{
    [Serializable]
    public class FilterObject : INotifyPropertyChanged
    {
        private bool m_selected;

        public FilterObject()
        {
            Value = null;
        }

        public string Description { get; set; }
        public int Tag { get; set; }
        public double? Value { get; set; }

        [XmlIgnore]
        public bool Selected
        {
            get { return m_selected; }
            set
            {
                if (m_selected == value)
                    return;

                m_selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected"));
            }
        }

        public NoteStageType Stage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}