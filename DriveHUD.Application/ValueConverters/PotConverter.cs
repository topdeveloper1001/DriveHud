using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;

using Color = System.Windows.Media.Color;

namespace DriveHUD.Application.ValueConverters
{
    public class PotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal pot = (decimal)value;

            if (pot < 10)
                return pot.ToString("N2");

            if (pot < 100)
                return pot.ToString("N1");

            if (pot < 1000)
                return pot.ToString("N0");

            if (pot < 10000)
                return string.Format("{0}k", Math.Truncate(pot / 100) / 10);

            var result = Math.Round(pot / 1000);
            if (result < 1000)
                return string.Format("{0}k", result);

            return string.Format("{0}M", Math.Truncate(result / 100) / 10);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PotColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal pot = (decimal)value;

            if (pot >= System.Convert.ToDecimal(50))
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            else if (pot >= System.Convert.ToDecimal(20))
                return new SolidColorBrush(Color.FromRgb(255, 128, 0));
            else
                return new SolidColorBrush(Color.FromRgb(0, 204, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}