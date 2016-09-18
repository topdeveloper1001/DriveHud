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
    public class ZeroNonZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility returnValue = (value == null || System.Convert.ToDouble(value) == 0) ? Visibility.Collapsed : Visibility.Visible;

            if (parameter != null && parameter.ToString() == "Inverse")
            {
                returnValue = returnValue == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
