using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using DriveHUD.Entities;

namespace DriveHUD.Application.ValueConverters
{
    public class FilterCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] != null && values[0] != DependencyProperty.UnsetValue && values[1] != null)
            {
                var range = ((KeyValuePair<int, string>)values[1]).Key;
                var list = ((List<Playerstatistic>)values[0]).OrderByDescending(x => x.Time).Take(range);
                return list;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
