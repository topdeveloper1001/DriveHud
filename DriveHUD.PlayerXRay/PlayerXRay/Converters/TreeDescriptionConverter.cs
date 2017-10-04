using System;
using System.Windows.Data;

namespace DriveHUD.PlayerXRay.Converters
{
    public class TreeDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            return string.IsNullOrEmpty(value.ToString()) ? null : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
