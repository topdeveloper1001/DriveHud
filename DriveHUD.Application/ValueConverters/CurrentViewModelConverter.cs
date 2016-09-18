using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using DriveHUD.Application.Models;
using Model.Enums;
using DriveHUD.Common.Infrastructure.Base;

namespace DriveHUD.Application.ValueConverters
{
    public class CurrentViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == ((EnumViewModelType)parameter).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }
}

