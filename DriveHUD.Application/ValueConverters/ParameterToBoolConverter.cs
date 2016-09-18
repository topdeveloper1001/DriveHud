using System;
using System.Globalization;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class ParameterToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            return (value.ToString() == parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {

            return (bool) value ? Enum.Parse(targetType, parameter.ToString(), true) : false;
        }
    }
}