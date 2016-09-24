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
    [ValueConversion(typeof(object), typeof(SolidColorBrush))]
    public class ParameterBooleanToBrushConverter : MarkupExtensionConverterBase
    {
        public ParameterBooleanToBrushConverter()
        {
            TrueColor = new SolidColorBrush(Colors.Green);
            FalseColor = new SolidColorBrush(Colors.Red);
        }

        public SolidColorBrush TrueColor { get; set; }
        public SolidColorBrush FalseColor { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;

            if (value != null)
            {
                result = (value.ToString() == parameter.ToString());
            }

            return result ? TrueColor : FalseColor;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as SolidColorBrush) == TrueColor)
            {
                return true;
            }

            return false;
        }
    }
}
