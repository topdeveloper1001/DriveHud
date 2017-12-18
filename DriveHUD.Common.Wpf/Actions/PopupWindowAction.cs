//-----------------------------------------------------------------------
// <copyright file="PopupWindowAction.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Controls;
using DriveHUD.Common.Wpf.Interactivity;
using Microsoft.Practices.ServiceLocation;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;

namespace DriveHUD.Common.Wpf.Actions
{
    public class PopupWindowAction : PopupAction<Window>
    {
        /// <summary>
        /// Determines if the window has border
        /// </summary>
        public static readonly DependencyProperty HasBorderProperty = DependencyProperty.Register(
                "HasBorder",
                typeof(bool),
                typeof(PopupWindowAction),
                new PropertyMetadata(true, null));

        /// <summary>
        /// Gets or sets if the window has border
        /// </summary>
        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }

        protected override Window CreateWindow()
        {
            var window = new Window
            {
                Owner = Application.Current.MainWindow,
                ShowActivated = true
            };

            return window;
        }

        protected override void ApplyStyles(Window window, INotification notification)
        {
            if (!HasBorder)
            {
                window.AllowsTransparency = true;
                window.WindowStyle = System.Windows.WindowStyle.None;
            }

            if (notification.Title != null)
            {
                window.Title = notification.Title;
            }
        }

        protected override void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        protected override void OnClosed(Window window, Action callback)
        {
            EventHandler handler = null;

            handler = (o, e) =>
            {
                NonTopmostPopup.DisableTopMost = false;
                window.Closed -= handler;
                window.Content = null;

                if (IsSingle && !string.IsNullOrEmpty(ViewName))
                {
                    var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                    windowController.RemoveWindow(ViewName);
                }

                callback?.Invoke();
            };

            window.Closed += handler;
        }

        protected override void Show(Window window)
        {
            NonTopmostPopup.DisableTopMost = true;

            if (IsModal)
            {
                window.ShowDialog();
                return;
            }

            if (IsSingle && !string.IsNullOrEmpty(ViewName))
            {
                var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                windowController.AddWindow(ViewName, window, () => window.Close());
            }

            window.Show();
        }

        protected override void Activate(Window window)
        {
            if (!window.IsVisible)
            {
                window.Show();
                return;
            }

            window.Activate();
        }

        protected override void SetWindowPosition(Window window, double left, double top)
        {
            window.Left = left;
            window.Top = top;
        }
    }
}