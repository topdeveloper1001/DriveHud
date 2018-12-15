using System;
using System.Globalization;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class SelectedLayoutToCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (values.Length == 2 && values[0].ToString() == values[1].ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}