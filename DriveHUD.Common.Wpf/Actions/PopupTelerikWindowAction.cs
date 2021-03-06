﻿//-----------------------------------------------------------------------
// <copyright file="PopupTelerikWindowAction.cs" company="Ace Poker Solutions">
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
using Telerik.Windows.Controls;

namespace DriveHUD.Common.Wpf.Actions
{
    public class PopupTelerikWindowAction : PopupAction<RadWindow>
    {
        protected override RadWindow CreateWindow()
        {
            var window = new RadWindow();          
            return window;
        }

        protected override void ApplyStyles(RadWindow window, INotification notification)
        {
            window.Header = notification.Title;
        }

        protected override void CloseWindow(RadWindow window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        protected override void OnClosed(RadWindow window, Action callback)
        {
            EventHandler<WindowClosedEventArgs> handler = null;

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

        protected override void Show(RadWindow window)
        {
            NonTopmostPopup.DisableTopMost = true;

            if (IsModal)
            {
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
                return;
            }

            if (IsSingle && !string.IsNullOrEmpty(ViewName))
            {
                var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                windowController.AddWindow(ViewName, window, () => window.Close());
            }

            window.Show();
            window.BringToFront();
        }

        protected override void Activate(RadWindow window)
        {
            window.BringToFront();
        }

        protected override void SetWindowPosition(RadWindow window, double left, double top)
        {
            window.Left = left;
            window.Top = top;
        }
    }
}