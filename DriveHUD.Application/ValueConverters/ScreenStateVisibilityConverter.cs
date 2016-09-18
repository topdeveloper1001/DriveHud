using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using Model.Enums;

namespace DriveHUD.Application.ValueConverters
{
    public  class ScreenStateVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = (EnumDashBoardScreen)value;
            if (state != null)
            {
                switch (parameter as string)
                {
                    case "Dashboard":
                        return state == EnumDashBoardScreen.Dashboard?Visibility.Visible:Visibility.Collapsed;
                    case "Tournament":
                        return state == EnumDashBoardScreen.Tournament ? Visibility.Visible : Visibility.Collapsed;
                    case "HUD":
                        return state == EnumDashBoardScreen.HUD ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
