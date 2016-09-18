using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Controls
{
    public class ComboBoxEx : ComboBox
    {
        public ComboBoxEx()
            : base()
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

        public static readonly DependencyProperty PopupMaxWidthProperty =
            DependencyProperty.Register("PopupMaxWidth", typeof(double), typeof(ComboBoxEx), new PropertyMetadata(Double.PositiveInfinity));

        public static readonly DependencyProperty MultiselectListBoxStyleProperty =
            DependencyProperty.Register("MultiselectListBoxStyle", typeof(Style), typeof(ComboBoxEx), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ComboBoxEx), new PropertyMetadata(SelectionMode.Single));

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(ComboBoxEx), new PropertyMetadata(null));

        private static DependencyProperty SelectedItemColorProperty
        = DependencyProperty.Register("SelectedItemColor", typeof(Brush), typeof(ComboBoxEx), new PropertyMetadata(null, OnLabelColourChanged));

        private static void OnLabelColourChanged(DependencyObject source,
                                   DependencyPropertyChangedEventArgs e)
        {
            //ComboBoxEx ctl = source as ComboBoxEx;
            //if (ctl != null)
            //{
            //    ctl.LabelGrid.Background = (Brush)e.NewValue;
            //}
        }

        public Brush SelectedItemColor
        {
            get { return GetValue(SelectedItemColorProperty) as Brush; }
            set { SetValue(SelectedItemColorProperty, value); }
        }

        private static DependencyProperty ItemsBackgroundColorProperty
       = DependencyProperty.Register("ItemsBackgroundColor", typeof(Brush), typeof(ComboBoxEx), new PropertyMetadata(null, OnItemsBackgroundColorChanged));

        private static void OnItemsBackgroundColorChanged(DependencyObject source,
                                   DependencyPropertyChangedEventArgs e)
        {
            //ComboBoxEx ctl = source as ComboBoxEx;
            //if (ctl != null)
            //{
            //    ctl.LabelGrid.Background = (Brush)e.NewValue;
            //}
        }

        public Brush ItemsBackgroundColor
        {
            get { return GetValue(ItemsBackgroundColorProperty) as Brush; }
            set { SetValue(ItemsBackgroundColorProperty, value); }
        }

        private static DependencyProperty ToggleButtonTemplateProperty
       = DependencyProperty.Register("ToggleButtonTemplate", typeof(ControlTemplate), typeof(ComboBoxEx), new PropertyMetadata(null, OnToggleButtonTemplateChanged));

        private static void OnToggleButtonTemplateChanged(DependencyObject source,
                                   DependencyPropertyChangedEventArgs e)
        {
            //ComboBoxEx ctl = source as ComboBoxEx;
            //if (ctl != null)
            //{
            //    ctl.LabelGrid.Background = (Brush)e.NewValue;
            //}
        }

        public ControlTemplate ToggleButtonTemplate
        {
            get { return GetValue(ToggleButtonTemplateProperty) as ControlTemplate; }
            set { SetValue(ToggleButtonTemplateProperty, value); }
        }
    }
}
