using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using DriveHUD.Common.Annotations;
using Model.Enums;
using System.Windows.Media;
using System.Resources;

namespace DriveHUD.ViewModels
{
    public class ChartSerieResourceHelper
    {
        public Color LineColor { get; set; }
        public Color PointColor { get; set; }
        public Color TrackBallColor { get; set; }
        public Color TooltipColor { get; set; }
        public VisualBrush AreaBrush { get; set; }

        public ChartSerieResourceHelper()
        {
        }

        public static ChartSerieResourceHelper GetSeriesBluePalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#34519C"),
                PointColor = (Color)ColorConverter.ConvertFromString("#115576"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#115576"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#34519C"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushBlue")
            };

        }

        public static ChartSerieResourceHelper GetSeriesYellowPalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#FDE40F"),
                PointColor = (Color)ColorConverter.ConvertFromString("#FFF714"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#FFF714"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#FDE40F"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushYellow")

            };

        }

        public static ChartSerieResourceHelper GetSerieOrangePalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#bd5922"),
                PointColor = (Color)ColorConverter.ConvertFromString("#ffdc50"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#ffbf43"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#bd5922"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushOrange")
            };
        }

        public static ChartSerieResourceHelper GetSerieGreenPalette()
        {
            return new ChartSerieResourceHelper()
            {
                LineColor = (Color)ColorConverter.ConvertFromString("#4BA516"),
                PointColor = (Color)ColorConverter.ConvertFromString("#93c940"),
                TrackBallColor = (Color)ColorConverter.ConvertFromString("#92c840"),
                TooltipColor = (Color)ColorConverter.ConvertFromString("#4BA516"),
                AreaBrush = (VisualBrush)System.Windows.Application.Current.FindResource("AreaVisualBrushGreen")
            };
        }
    }

    [Serializable]
    public class ChartSeries : INotifyPropertyChanged
    {

        public ChartSeries()
        {
        }

        #region Properties

        private string _caption;
        private EnumTelerikRadChartFunctionType _functionName;
        private EnumTelerikRadChartSeriesType _type = EnumTelerikRadChartSeriesType.Area;
        private Color _lineColor = new Color();
        private VisualBrush _areaStyle;
        private ObservableCollection<ChartSeriesItem> _itemsCollection;
        private bool _isVisible = true;

        public string Caption
        {
            get { return _caption; }
            set
            {
                if (value == _caption) return;
                _caption = value;
                OnPropertyChanged();
            }
        }

        public EnumTelerikRadChartFunctionType FunctionName
        {
            get { return _functionName; }
            set
            {
                if (value == _functionName) return;
                _functionName = value;
                OnPropertyChanged();
            }
        }

        public EnumTelerikRadChartSeriesType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChartSeriesItem> ItemsCollection
        {
            get { return _itemsCollection; }
            set
            {
                if (value == _itemsCollection) return;
                _itemsCollection = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public Color LineColor
        {
            get
            {
                return _lineColor;
            }

            set
            {
                if (value == LineColor) return;
                _lineColor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush LineColorBrush
        {
            get
            {
                return new SolidColorBrush(LineColor);
            }
        }

        public VisualBrush AreaStyle
        {
            get
            {
                return _areaStyle;
            }

            set
            {
                if (value == _areaStyle) return;
                _areaStyle = value;
                OnPropertyChanged();
            }
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
