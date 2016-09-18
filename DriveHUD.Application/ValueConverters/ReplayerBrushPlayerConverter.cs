using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Application.ValueConverters
{
    public class ReplayerBrushPlayerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;

            var boolValue = (bool) value;

            return boolValue
                ? App.Current.Resources["HudPlayerBrushDisabled"] as VisualBrush
                : App.Current.Resources["HudPlayerBrush"] as VisualBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}