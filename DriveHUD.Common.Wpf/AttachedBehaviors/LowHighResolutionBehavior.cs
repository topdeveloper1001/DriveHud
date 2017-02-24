//-----------------------------------------------------------------------
// <copyright file="RadioButtonResolutionBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    /// <summary>
    /// Behavior allow to set different styles for High/Low resolutions, which is controlled by IsLowResolution property
    /// </summary>
    public class LowHighResolutionBehavior
    {
        #region Attached properties

        public static DependencyProperty LowResolutionStyleProperty = DependencyProperty.RegisterAttached("LowResolutionStyle", typeof(Style), typeof(LowHighResolutionBehavior), new PropertyMetadata());

        public static void SetLowResolutionStyle(FrameworkElement element, object value)
        {
            element.SetValue(LowResolutionStyleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Style GetLowResolutionStyle(FrameworkElement element)
        {
            return (Style)element.GetValue(LowResolutionStyleProperty);
        }

        public static DependencyProperty HighResolutionStyleProperty = DependencyProperty.RegisterAttached("HighResolutionStyle", typeof(Style), typeof(LowHighResolutionBehavior), new PropertyMetadata());

        public static void SetHighResolutionStyle(FrameworkElement element, object value)
        {
            element.SetValue(HighResolutionStyleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Style GetHighResolutionStyle(FrameworkElement element)
        {
            return (Style)element.GetValue(HighResolutionStyleProperty);
        }

        public static DependencyProperty IsLowResolutionProperty = DependencyProperty.RegisterAttached("IsLowResolution", typeof(bool?), typeof(LowHighResolutionBehavior), new PropertyMetadata(null, OnIsLowResolutionChanged));

        public static void SetIsLowResolution(FrameworkElement element, object value)
        {
            element.SetValue(HighResolutionStyleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static object GetIsLowResolution(FrameworkElement element)
        {
            return element.GetValue(HighResolutionStyleProperty);
        }

        #endregion

        private static void OnIsLowResolutionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;

            if (frameworkElement == null)
            {
                return;
            }

            var isLowResolution = e.NewValue as bool?;

            if (!isLowResolution.HasValue)
            {
                return;
            }

            Style frameworkElementStyle = null;

            if (isLowResolution.Value)
            {
                frameworkElementStyle = GetLowResolutionStyle(frameworkElement);
            }
            else
            {
                frameworkElementStyle = GetHighResolutionStyle(frameworkElement);
            }

            frameworkElement.Style = frameworkElementStyle;
        }
    }
}