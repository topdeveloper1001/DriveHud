using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Application.ValueConverters
{
    public class DecimalToPotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                if (value.ToString() != "0")
                {
                    if(parameter != null && !string.IsNullOrEmpty(parameter.ToString()))
                    {
                        return String.Format(parameter.ToString(), value);
                    }
                } 
                else
                {
                    return string.Empty;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return null;
        }
    }
}
