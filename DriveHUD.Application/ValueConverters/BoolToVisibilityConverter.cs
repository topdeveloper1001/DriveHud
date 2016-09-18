using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using Telerik.Charting;

namespace DriveHUD.Application.ValueConverters
{
   public class BoolToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility returnValue = Visibility.Visible;

            if (value == null) return returnValue;

            bool _value = (bool)value;
            string _parameter = parameter == null ? string.Empty : parameter.ToString();

            if (_value) returnValue = _parameter == "Inverse" ? Visibility.Collapsed : Visibility.Visible;
            if (!_value) returnValue = _parameter == "Inverse" ? Visibility.Visible : Visibility.Collapsed;

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
