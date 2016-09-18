using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DriveHUD.Application.ValueConverters
{
    public class ColorToChipsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value.ToString();
            if (strValue == null)
                return Binding.DoNothing;

            var control = parameter as Control;
            if (control == null)
                return Binding.DoNothing;

            return new ImageBrush(
                new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(control),
                    string.Format("/DriveHUD.Common.Resources;component/images/Chips/{0}.png", strValue))));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
