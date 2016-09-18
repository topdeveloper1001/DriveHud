using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class StringToImageSourceConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var path = value.ToString();
            var imageUri = new Uri(path, UriKind.RelativeOrAbsolute);
            var imageBitmap = new BitmapImage(imageUri);

            return imageBitmap;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var imageBitmap = value as BitmapImage;

            if (imageBitmap == null)
            {
                return string.Empty;
            }

            return imageBitmap.BaseUri.AbsolutePath;
        }
    }
}