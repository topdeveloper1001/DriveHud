using DriveHUD.Common.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DriveHUD.ViewModels
{
    [Serializable]
    public class ChartSeriesItem : INotifyPropertyChanged
    {
        public ChartSeriesItem()
        {
        }

        #region Properties

        private decimal _value;
        private string _valueText;
        private DateTime _date;
        private Color _pointColor = new Color();
        private Color _trackBallColor = new Color();
        private Color _tooltipColor = new Color();

        public decimal Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public string ValueText
        {
            get { return _valueText; }
            set
            {
                if (value == _valueText) return;
                _valueText = value;
                OnPropertyChanged();
            }
        }


        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value == _date) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public Color PointColor
        {
            get
            {
                return _pointColor;
            }

            set
            {
                if (value == _pointColor) return;
                _pointColor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush PointColorBrush
        {
            get
            {
                return new SolidColorBrush(PointColor);
            }
        }

        public Color TrackBallColor
        {
            get
            {
                return _trackBallColor;
            }

            set
            {
                if (value == _trackBallColor) return;
                _trackBallColor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush TrackBallColorBrush
        {
            get { return new SolidColorBrush(TrackBallColor); }
        }

        public Color TooltipColor
        {
            get
            {
                return _tooltipColor;
            }

            set
            {
                if (value == _tooltipColor) return;
                _tooltipColor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush TooltipColorBrush
        {
            get { return new SolidColorBrush(TooltipColor); }
        }


        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
