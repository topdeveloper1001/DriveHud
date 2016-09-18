using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Model.Enums;

namespace DriveHUD.Application.ValueConverters
{
    public class ScreenStateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var state = (EnumDashBoardScreen) value;
            if (state != null)
            {
                switch (parameter as string)
                {
                    case "Dashboard":
                        return state==EnumDashBoardScreen.Dashboard;
                    case "Tournament":
                        return state == EnumDashBoardScreen.Tournament;
                    case "HUD":
                        return state == EnumDashBoardScreen.HUD;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (parameter as string)
            {
                case "Dashboard":
                    return  EnumDashBoardScreen.Dashboard;
                case "Tournament":
                    return   EnumDashBoardScreen.Tournament;
                case "HUD":
                    return  EnumDashBoardScreen.HUD;
            }
            return value;
        }
    }
}
