using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(decimal), typeof(Visibility))]
    public class TiltMeterToVisibilityConverter : MarkupExtensionConverterBase
    {
        public TiltMeterToVisibilityConverter()
        {
            HiddenVisibility = Visibility.Hidden;
        }

        public Visibility HiddenVisibility { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tiltMeter = value as decimal?;
            int level = 0;
                               
            if (!tiltMeter.HasValue || !int.TryParse(parameter.ToString(), out level))
            {
                return HiddenVisibility;
            }

            if (tiltMeter.Value < 1)
            {
                return HiddenVisibility;
            }

            bool visible = false;

            switch (level)
            {
                case 1:
                    visible = tiltMeter.Value >= 1;
                    break;
                case 2:
                    visible = tiltMeter.Value >= 21;
                    break;
                case 3:
                    visible = tiltMeter.Value >= 41;
                    break;
                case 4:
                    visible = tiltMeter.Value >= 61;
                    break;
                case 5:
                    visible = tiltMeter.Value >= 81;
                    break;
                default:
                    break;
            }

            return visible ? Visibility.Visible : HiddenVisibility;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}