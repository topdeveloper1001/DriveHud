//-----------------------------------------------------------------------
// <copyright file="ScrollViewerHelper.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class ScrollViewerHelper
    {
        public static DependencyProperty UseMouseWheelForHorizontalScrollProperty = DependencyProperty.RegisterAttached("UseMouseWheelForHorizontalScroll",
            typeof(bool), typeof(ScrollViewerHelper), new PropertyMetadata(OnUseMouseWheelForHorizontalScrollPropertyChanged));

        public static void SetUseMouseWheelForHorizontalScroll(ScrollViewer scrollViewer, object value)
        {
            scrollViewer.SetValue(UseMouseWheelForHorizontalScrollProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollViewer))]
        public static object GetUseMouseWheelForHorizontalScroll(ScrollViewer scrollViewer)
        {
            return scrollViewer.GetValue(UseMouseWheelForHorizontalScrollProperty);
        }

        private static void OnUseMouseWheelForHorizontalScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ScrollViewer scrollViewer))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            }
            else
            {
                scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(sender is ScrollViewer scrollViewer))
            {
                return;
            }

            if (e.Delta < 0)
            {
                scrollViewer.PageRight();
            }
            else
            {
                scrollViewer.PageLeft();
            }

            e.Handled = true;
        }
    }
}