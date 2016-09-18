using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace DriveHUD.Application.ValueConverters
{
    public class IntToCardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = value as int?;
            if (intValue == null)
                return Binding.DoNothing;

            if (intValue < 0 || intValue > 52)
                return Binding.DoNothing;

            var control = parameter as Control;
            if (control == null)
                return Binding.DoNothing;

            return new ImageBrush(
                new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(control),
                    string.Format("/DriveHUD.Common.Resources;component/images/cards/{0}.png", intValue))));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}