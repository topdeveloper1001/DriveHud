using System;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class CurrentViewModelMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values[0].ToString() == values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new[] { value, value };
        }
    }
}
