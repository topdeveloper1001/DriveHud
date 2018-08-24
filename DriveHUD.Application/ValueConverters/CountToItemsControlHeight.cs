using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class CountToItemsControlHeight : IValueConverter
    {        
        public double RowHeight { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int count) || !(parameter is int itemsPerRow))
            {
                return RowHeight;
            }

            return Math.Ceiling((double)count / itemsPerRow) * RowHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}