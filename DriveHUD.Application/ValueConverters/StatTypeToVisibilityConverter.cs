using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using DriveHUD.Application.ViewModels;
using DriveHUD.ViewModels;

namespace DriveHUD.Application.ValueConverters
{
    public class StatTypeToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.Equals("StatInfoBreak"))
            {
                if (value != null && value.GetType() == typeof(StatInfoBreak))
                    return Visibility.Visible;
            }
            else
            {
                if (value != null && value.GetType() == typeof(StatInfo))
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
