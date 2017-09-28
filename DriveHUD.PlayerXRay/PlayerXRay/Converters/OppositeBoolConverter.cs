using System;
using System.Globalization;
using System.Windows.Data;

namespace AcePokerSolutions.PlayerXRay.Converters
{
    public class OppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(bool) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(bool) value);
        }

        #endregion
    }
}
