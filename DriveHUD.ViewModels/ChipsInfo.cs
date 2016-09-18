using DriveHUD.Common.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ChipsInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Color color;
        private decimal count;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public Color ChipsColor
        {
            get { return color; }
            set
            {
                if (value.Equals(color)) return;
                color = value;
                OnPropertyChanged("ChipsColor");
            }
        }

        public decimal Count
        {
            get { return count; }
            set
            {
                if (value.Equals(count)) return;
                count = value;
                OnPropertyChanged("Count");
            }
        }

        public enum Color
        {
            Black,
            Orange,
            Blue,
            Green,
            Grey,
            Purple,
            Red,
            Yellow
        }

        public void Update(ChipsInfo chip)
        {
            ChipsColor = chip.ChipsColor;
            Count = chip.Count;
        }
    }
}
