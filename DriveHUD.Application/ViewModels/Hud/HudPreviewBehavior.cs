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

using DriveHUD.Application.TableConfigurators;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;
using ReactiveUI;
using Telerik.Windows.Controls.Diagrams;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Reactive.Disposables;
using DriveHUD.Application.Controls;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Behavior to display HUD preview
    /// </summary>
    public class HudPreviewBehavior : Behavior<Canvas>
    {
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

            AssociatedObject.Children.Add(toolElement);

            var position = GetToolPosition(toolViewModel);

            Canvas.SetLeft(toolElement, position.X);
            Canvas.SetTop(toolElement, position.Y);
        }

        /// <summary>
        /// Resizes canvas to new size based on content
        /// </summary>
        private void ResizeCanvas()
        {
            if (!AssociatedObject.IsLoaded)
            {
                return;
            }


        }

        private Point GetToolPosition(HudBaseToolViewModel toolViewModel)
        {



            return new Point(0, 0);
        }

        private static void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var canvas = sender as Canvas;

            if (canvas == null)
            {
                return;
            }

            foreach (var child in canvas.Children.OfType<FrameworkElement>())
            {
                var left = Canvas.GetLeft(child);

                if (left + child.ActualWidth > canvas.Width)
                {
                    var scale = (left + child.ActualWidth) / canvas.Width;
                    canvas.Width *= scale;
                    canvas.Height *= scale;
                }
            }
        }
    }
}