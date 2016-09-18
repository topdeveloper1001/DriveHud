using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Wpf.Helpers
{
    public static class TextMeasurer
    {
        public static double MesureString(string text)
        {
            System.Windows.Controls.TextBlock tb = new System.Windows.Controls.TextBlock() { FontSize = 18 };
            var formattedText = new System.Windows.Media.FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentUICulture,
                System.Windows.FlowDirection.LeftToRight,
                new System.Windows.Media.Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
               tb.FontSize,
                System.Windows.Media.Brushes.Black);
            return formattedText.Width;
        }
    }
}
