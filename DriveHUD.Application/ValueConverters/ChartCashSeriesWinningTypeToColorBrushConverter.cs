using Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Application.ValueConverters
{
    public class ChartCashSeriesWinningTypeToColorBrushConverter : DependencyObject, IValueConverter
    {
        public readonly static DependencyProperty ColorBrushChartCashSeriesWinningTypesProperty = DependencyProperty.Register("ColorBrushChartCashSeriesWinningTypes",
            typeof(ColorBrushChartCashSeriesWinningTypeCollection), typeof(ChartCashSeriesWinningTypeToColorBrushConverter),
            new PropertyMetadata(new ColorBrushChartCashSeriesWinningTypeCollection()));

        public ColorBrushChartCashSeriesWinningTypeCollection ColorBrushChartCashSeriesWinningTypes
        {
            get
            {
                return (ColorBrushChartCashSeriesWinningTypeCollection)GetValue(ColorBrushChartCashSeriesWinningTypesProperty);
            }
            set
            {
                SetValue(ColorBrushChartCashSeriesWinningTypesProperty, value);
            }
        }

        public readonly static DependencyProperty DefaultBrushProperty = DependencyProperty.Register("DefaultBrush",
           typeof(SolidColorBrush), typeof(ChartCashSeriesWinningTypeToColorBrushConverter),
           new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public SolidColorBrush DefaultBrush
        {
            get
            {
                return (SolidColorBrush)GetValue(DefaultBrushProperty);
            }
            set
            {
                SetValue(DefaultBrushProperty, value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is ChartCashSeriesWinningType) || ColorBrushChartCashSeriesWinningTypes == null)
            {
                return DefaultBrush;
            }

            var winningType = (ChartCashSeriesWinningType)value;

            var colorBrushWinningType = ColorBrushChartCashSeriesWinningTypes.FirstOrDefault(x => x.WinningType == winningType);

            if (colorBrushWinningType == null)
            {
                return DefaultBrush;
            }

            return colorBrushWinningType.Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class ColorBrushChartCashSeriesWinningType
    {
        public ChartCashSeriesWinningType WinningType { get; set; }

        public SolidColorBrush Brush { get; set; }
    }

    public class ColorBrushChartCashSeriesWinningTypeCollection : ObservableCollection<ColorBrushChartCashSeriesWinningType>
    {
    }
}