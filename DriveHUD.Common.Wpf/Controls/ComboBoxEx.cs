using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Controls
{
    public class ComboBoxEx : ComboBox
    {
        public ComboBoxEx() : base()
        {
        }

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public Style MultiselectListBoxStyle
        {
            get { return (Style)GetValue(MultiselectListBoxStyleProperty); }
            set { SetValue(MultiselectListBoxStyleProperty, value); }
        }

        public double PopupMaxWidth
        {
            get { return (double)GetValue(PopupMaxWidthProperty); }
            set { SetValue(PopupMaxWidthProperty, value); }
        }

        public ControlTemplate ToggleButtonTemplate
        {
            get { return GetValue(ToggleButtonTemplateProperty) as ControlTemplate; }
            set { SetValue(ToggleButtonTemplateProperty, value); }
        }

        public Brush SelectedItemColor
        {
            get { return GetValue(SelectedItemColorProperty) as Brush; }
            set { SetValue(SelectedItemColorProperty, value); }
        }

        public Brush ItemsBackgroundColor
        {
            get { return GetValue(ItemsBackgroundColorProperty) as Brush; }
            set { SetValue(ItemsBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty PopupMaxWidthProperty = DependencyProperty.Register("PopupMaxWidth", typeof(double), typeof(ComboBoxEx), new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty MultiselectListBoxStyleProperty = DependencyProperty.Register("MultiselectListBoxStyle", typeof(Style), typeof(ComboBoxEx), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ComboBoxEx), new PropertyMetadata(SelectionMode.Single));

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(ComboBoxEx), new PropertyMetadata(null));

        private static readonly DependencyProperty SelectedItemColorProperty = DependencyProperty.Register("SelectedItemColor", typeof(Brush), typeof(ComboBoxEx), new PropertyMetadata(null));

        private static readonly DependencyProperty ItemsBackgroundColorProperty = DependencyProperty.Register("ItemsBackgroundColor", typeof(Brush), typeof(ComboBoxEx), new PropertyMetadata(null));

        private static readonly DependencyProperty ToggleButtonTemplateProperty = DependencyProperty.Register("ToggleButtonTemplate", typeof(ControlTemplate), typeof(ComboBoxEx), new PropertyMetadata(null));
    }
}