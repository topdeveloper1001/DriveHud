using DriveHUD.Common.Wpf.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class ColorBrightnessToSolidBrushConverter : MarkupExtensionConverterBase
    {
        public ColorBrightnessToSolidBrushConverter()
        {
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;

            if (color == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var brightness = parameter as float?;

            if (brightness != null)
            {
                color = ColorsHelper.ChangeColorBrightness(color.Value, brightness.Value);
            }

            var brush = new SolidColorBrush(color.Value);

            return brush;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;

            if (brush == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return brush.Color;
        }
    }
}
