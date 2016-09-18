using System;
using System.Globalization;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class ValueToEquityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,object parameter, CultureInfo culture)
        {
            if (value as decimal? == 0)
                return string.Empty;
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return null;
        }
    }
}