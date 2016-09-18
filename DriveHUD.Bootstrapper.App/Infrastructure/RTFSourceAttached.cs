using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DriveHUD.Bootstrapper.App.Infrastructure
{
    public class RTFSourceAttached : DependencyObject
    {
        public static string GetRTFSource(DependencyObject obj)
        {
            return (string)obj.GetValue(RTFSourceProperty);
        }

        public static void SetRTFSource(DependencyObject obj, string value)
        {
            obj.SetValue(RTFSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for RTFSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RTFSourceProperty =
            DependencyProperty.RegisterAttached("RTFSource", typeof(string), typeof(RTFSourceAttached), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, propertyChangedCallback: OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(!(d is RichTextBox) || e.NewValue == null || string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                return;
            }

            var rtb = d as RichTextBox;
            var filepath = e.NewValue.ToString();
            if (System.IO.File.Exists(filepath))
            {
                var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                using (var fileStream = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate))
                {
                    textRange.Load(fileStream, System.Windows.DataFormats.Rtf);
                }
            }

        }
    }
}
