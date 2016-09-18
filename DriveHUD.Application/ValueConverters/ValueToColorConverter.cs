using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Application.ValueConverters
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                if (Decimal.Parse(value.ToString()) > 0)
                    return new SolidColorBrush(Color.FromRgb(37, 145, 83));
                if (Decimal.Parse(value.ToString()) < 0)
                    return new SolidColorBrush(Color.FromRgb(192, 57, 43));
            }
            return new SolidColorBrush(Color.FromRgb(125, 127, 132));
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return null;
        }
    }

    // Does a math equation on the bound value.
    // Use @VALUE in your mathEquation as a substitute for bound value
    // Operator order is parenthesis first, then Left-To-Right (no operator precedence)


    // Does a math equation on the bound value.
    // Use @VALUE in your mathEquation as a substitute for bound value
    // Operator order is parenthesis first, then Left-To-Right (no operator precedence)
}
