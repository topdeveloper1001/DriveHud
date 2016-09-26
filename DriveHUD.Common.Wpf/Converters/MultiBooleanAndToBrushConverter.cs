using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public class MultiBooleanAndToBrushConverter : MarkupExtensionMultiConverterBase
    {
        public MultiBooleanAndToBrushConverter()
        {
            TrueColor = new SolidColorBrush(Colors.Green);
            FalseColor = new SolidColorBrush(Colors.Red);
        }

        public SolidColorBrush TrueColor { get; set; }
        public SolidColorBrush FalseColor { get; set; }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = values.All(c => c is bool && (bool)c);

            return result ? TrueColor : FalseColor;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
