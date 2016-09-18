using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(Street), typeof(Visibility))]
    public class StreetToVisibilityConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility returnValue = Visibility.Collapsed;
            if (value == null)
            {
                return returnValue;
            }

            if (parameter == null)
            {
                return returnValue;
            }

            Street valueStreet, parameterStreet;
            if (Enum.TryParse<Street>(value.ToString(), out valueStreet) && Enum.TryParse<Street>(parameter.ToString(), out parameterStreet))
            {
                return valueStreet >= parameterStreet ? Visibility.Visible : Visibility.Collapsed;
            }

            return returnValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
