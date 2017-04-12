//-----------------------------------------------------------------------
// <copyright file="HudStatsBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Controls;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Behavior to display HUD preview
    /// </summary>
    public class HudPreviewBehavior : Behavior<Canvas>
    {
        private CompositeDisposable Disposables = new CompositeDisposable();

        public static DependencyProperty HudElementProperty = DependencyProperty.Register("HudElement", typeof(HudElementViewModel),
           typeof(HudPreviewBehavior), new PropertyMetadata(null, OnHudElementChanged));

        /// <summary>
        /// Gets or sets <see cref="HudElementViewModel" />
        /// </summary>
        public HudElementViewModel HudElement
        {
            get
            {
                return (HudElementViewModel)GetValue(HudElementProperty);
            }
            set
            {
                SetValue(HudElementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets base width of canvas
        /// </summary>
        public double BaseWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets base width of canvas
        /// </summary>
        public double BaseHeight
        {
            get;
            set;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Width = BaseWidth;
            AssociatedObject.Height = BaseHeight;

            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            if (Disposables != null)
            {
                Disposables.Dispose();
            }

            AssociatedObject.Loaded -= OnLoaded;

            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeCanvas();
        }

        private static void OnHudElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HudPreviewBehavior;
            var hudElement = e.NewValue as HudElementViewModel;

            if (behavior == null || hudElement == null)
            {
                return;
            }

            if (behavior.Disposables != null)
            {
                behavior.Disposables.Dispose();
            }

            behavior.Disposables = new CompositeDisposable();

            if (behavior.AssociatedObject != null && behavior.AssociatedObject.Children != null)
            {
                behavior.AssociatedObject.Children.Clear();
            }

            // create tools
            foreach (var tool in hudElement.Tools)
            {
                behavior.AddTool(tool);
            }

            behavior.ResizeCanvas();
        }

        /// <summary>
        /// Adds the representation of <see cref="HudBaseToolViewModel"/> to the associated <see cref="Canvas"/>
        /// </summary>
        /// <param name="toolViewModel">Tool to add</param>
        private void AddTool(HudBaseToolViewModel toolViewModel)
        {
            if (toolViewModel == null)
            {
                return;
            }

            var toolElement = new HudPlainBox
            {
                DataContext = toolViewModel,
                Width = toolViewModel.Width,
                Height = toolViewModel.Height
            };

            var disposable = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => toolElement.Loaded += h,
                h => toolElement.Loaded -= h).Subscribe(x => ResizeCanvas());

            Disposables.Add(disposable);

            AssociatedObject.Children.Add(toolElement);

            //if (toolViewModel is HudPlainStatBoxViewModel)
            //{
            //    var statBoxViewModel = toolViewModel as HudPlainStatBoxViewModel;

            //    disposable = statBoxViewModel.Stats.Changed.Subscribe(x => ResizeCanvas());
            //    Disposables.Add(disposable);
            //}
        }

        /// <summary>
        /// Resizes canvas to new size based on content and adjusts tools positions
        /// </summary>
        private void ResizeCanvas()
        {
            if (!AssociatedObject.IsLoaded || HudElement == null || HudElement.Tools == null ||
                HudElement.Tools.Count < 1 || AssociatedObject.Children == null ||
                AssociatedObject.Children.OfType<FrameworkElement>().Any(x => !x.IsLoaded))
            {
                return;
            }

            // edges of area occupied by tools 
            double top = double.MaxValue;
            double left = double.MaxValue;
            double right = 0;
            double bottom = 0;

            var toolsElements = AssociatedObject.Children.OfType<FrameworkElement>().ToArray();

            // get edges positions
            foreach (var toolElement in toolsElements)
            {
                var tool = toolElement.DataContext as HudBaseToolViewModel;

                if (tool == null)
                {
                    continue;
                }

                if (tool.Position.X < left)
                {
                    left = tool.Position.X;
                }

                if (tool.Position.Y < top)
                {
                    top = tool.Position.Y;
                }

                if (tool.Position.X + toolElement.ActualWidth > right)
                {
                    right = tool.Position.X + toolElement.ActualWidth;
                }

                if (tool.Position.Y + toolElement.ActualHeight > bottom)
                {
                    bottom = tool.Position.Y + toolElement.ActualHeight;
                }
            }

            var occupiedWidth = right - left;
            var occupiedHeight = bottom - top;

            AssociatedObject.Width = BaseWidth < occupiedWidth ? occupiedWidth : BaseWidth;
            AssociatedObject.Height = BaseHeight < occupiedHeight ? occupiedHeight : BaseHeight;

            // offsets to center elements
            double offsetX = 0;
            double offsetY = 0;

            if (occupiedWidth < AssociatedObject.Width)
            {
                offsetX = (AssociatedObject.Width - occupiedWidth) / 2;
            }

            if (occupiedHeight < AssociatedObject.Height)
            {
                offsetY = (AssociatedObject.Height - occupiedHeight) / 2;
            }

            // center all elements inside canvas
            foreach (var toolElement in toolsElements)
            {
                var tool = toolElement.DataContext as HudBaseToolViewModel;

                if (tool == null)
                {
                    continue;
                }

                Canvas.SetLeft(toolElement, tool.Position.X - left + offsetX);
                Canvas.SetTop(toolElement, tool.Position.Y - top + offsetY);
            }
        }
    }
}