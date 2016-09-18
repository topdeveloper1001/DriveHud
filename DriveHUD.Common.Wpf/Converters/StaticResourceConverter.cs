using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    public class StaticResourceConverter : MarkupExtensionMultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FrameworkElement targetObject = values[0] as FrameworkElement;

            if (targetObject == null)
            {
                return DependencyProperty.UnsetValue;
            }
            return targetObject.TryFindResource(values[1]);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
