using System;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class BoolToTextConverter_OnOff : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null || System.Convert.ToBoolean(value) == false) ? "Off" : "On";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
