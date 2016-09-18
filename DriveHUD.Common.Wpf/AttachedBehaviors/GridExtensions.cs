//-----------------------------------------------------------------------
// <copyright file="GridExtensions.cs" company="Ace Poker Solutions">
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
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class GridExtensions
    {
        #region ColumnItems

        public static DependencyProperty IsBlurEnabledProperty = DependencyProperty.RegisterAttached("IsBlurEnabled", typeof(bool), typeof(GridExtensions), new PropertyMetadata(OnIsBlurEnabledPropertyChanged));

        public static void SetIsBlurEnabled(Grid grid, object value)
        {
            grid.SetValue(IsBlurEnabledProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Grid))]
        public static object GetIsBlurEnabled(Grid grid)
        {
            return grid.GetValue(IsBlurEnabledProperty);
        }

        private static void OnIsBlurEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as Grid;

            if (grid == null)
            {
                return;
            }

            var isBlurEnabled = e.NewValue as bool?;

            if (isBlurEnabled == true)
            {
                grid.Effect = new BlurEffect { Radius = 10 };
                return;
            }

            grid.Effect = null;
        }

        #endregion
    }
}