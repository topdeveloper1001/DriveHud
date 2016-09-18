using Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public class MRatioToColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is EnumMRatio)
            {
                switch ((EnumMRatio)value)
                {
                    case EnumMRatio.RedZone:
                        return new SolidColorBrush(Colors.Red);
                    case EnumMRatio.OrangeZone:
                        return new SolidColorBrush(Colors.Orange);
                    case EnumMRatio.YellowZone:
                        return new SolidColorBrush(Colors.Yellow);
                    case EnumMRatio.GreenZone:
                        return new SolidColorBrush(Colors.Green);
                    case EnumMRatio.BlueZone:
                        return new SolidColorBrush(Colors.Blue);
                    case EnumMRatio.PurpleZone:
                        return new SolidColorBrush(Colors.Purple);
                }
            }
            return new SolidColorBrush();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MRatioToTextConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumMRatio)
            {
                switch ((EnumMRatio)value)
                {
                    case EnumMRatio.RedZone:
                        return "\u2264 5";
                    case EnumMRatio.OrangeZone:
                        return "6-10";
                    case EnumMRatio.YellowZone:
                        return "10-20";
                    case EnumMRatio.GreenZone:
                        return "20-40";
                    case EnumMRatio.BlueZone:
                        return "40-60";
                    case EnumMRatio.PurpleZone:
                        return "\u2265 60";
                }
            }
            return string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
