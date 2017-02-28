//-----------------------------------------------------------------------
// <copyright file="MainScrollBarBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class MainScrollBarBehavior : Behavior<ScrollViewer>
    {
        public double ThresholdHeight
        {
            get { return (double)GetValue(ThresholdHeightProperty); }
            set { SetValue(ThresholdHeightProperty, value); }
        }

        public static readonly DependencyProperty ThresholdHeightProperty =
            DependencyProperty.Register("ThresholdHeight", typeof(double), typeof(MainScrollBarBehavior), new PropertyMetadata(500.0));

        public FrameworkElement ContentContainer
        {
            get { return (FrameworkElement)GetValue(ContentContainerProperty); }
            set { SetValue(ContentContainerProperty, value); }
        }

        public static readonly DependencyProperty ContentContainerProperty =
            DependencyProperty.Register("ContentContainer", typeof(FrameworkElement), typeof(MainScrollBarBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnContentContainerChanged)));

        private static void OnContentContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MainScrollBarBehavior;
            if (behavior == null)
            {
                return;
            }

            behavior.ContentContainer.SizeChanged += behavior.ContentContainer_SizeChanged;
        }

        internal void ContentContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.AssociatedObject.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled)
            {
                Update();
            }
        }

        private void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource == this.AssociatedObject)
            {
                Update();
                e.Handled = true;
            }
        }

        private void Update()
        {
            if (ContentContainer == null || 0 >= ContentContainer.ActualHeight)
            {
                return;
            }

            if ((0 >= this.AssociatedObject.ActualHeight))
            {
                // The attached ScrollViewer was probably not laid out yet, or has zero size.
                this.AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                return;
            }

            var scrollBarVisibility = this.AssociatedObject.VerticalScrollBarVisibility;

            if (this.AssociatedObject.ScrollableHeight == 0)
            {
                if (scrollBarVisibility == ScrollBarVisibility.Auto
                    && ContentContainer.ActualHeight >= ThresholdHeight)
                {
                    this.AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    ContentContainer.MaxHeight = Double.PositiveInfinity;
                    return;
                }

                if (scrollBarVisibility == ScrollBarVisibility.Disabled
                    && ContentContainer.ActualHeight < ThresholdHeight)
                {
                    ContentContainer.MaxHeight = ThresholdHeight;
                    this.AssociatedObject.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                    return;
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged;
        }

        protected override void OnDetaching()
        {
            if (this.ContentContainer != null)
            {
                this.ContentContainer.SizeChanged -= ContentContainer_SizeChanged;
            }

            this.AssociatedObject.ScrollChanged -= AssociatedObject_ScrollChanged;
            base.OnDetaching();
        }
    }
}
