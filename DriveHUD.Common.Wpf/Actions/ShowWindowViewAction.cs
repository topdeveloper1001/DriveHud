//-----------------------------------------------------------------------
// <copyright file="ShowWindowViewAction.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Wpf.Mvvm;
using Microsoft.Practices.ServiceLocation;
using Prism.Interactivity.InteractionRequest;
using System;
using System.ComponentModel;
using System.Windows;

namespace DriveHUD.Common.Wpf.Actions
{
    public class ShowWindowViewAction : ShowViewActionBase<Window>
    {
        protected override void Activate(Window window)
        {
            if (window == null)
            {
                return;
            }

            if (!window.IsVisible)
            {
                window.Show();
                return;
            }

            window.Activate();
        }

        protected override void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        protected override Window CreateWindow(INotification context)
        {
            var window = new Window
            {
                Title = context != null ? context.Title : string.Empty,
                ShowActivated = true
            };

            if (StartupLocation == StartupLocationOption.CenterScreen)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else if (StartupLocation == StartupLocationOption.CenterAssosiated)
            {
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            return window;
        }

        protected override void OnClosed(Window window, Action callback, string viewName)
        {
            EventHandler handler = null;

            handler = (o, e) =>
            {
                NonTopmostPopup.DisableTopMost = false;
                window.Closed -= handler;

                if (window.Content is IViewModelContainer<IDisposable>)
                {
                    ((IViewModelContainer<IDisposable>)window.Content).ViewModel?.Dispose();
                }

                window.Content = null;

                if (SingleOnly)
                {
                    var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                    windowController.RemoveWindow(viewName);
                }

                callback?.Invoke();
            };

            window.Closed += handler;
        }

        protected override void OnClosing(Window window, ICloseableViewModel viewModel)
        {
            CancelEventHandler handler = null;

            handler = (s, e) =>
            {
                if (!viewModel.OnClosing())
                {
                    e.Cancel = true;
                    return;
                }

                window.Closing -= handler;
            };

            window.Closing += handler;
        }

        protected override void SetWindowPosition(Window window, double left, double top)
        {
            if (window == null)
            {
                return;
            }

            window.Left = left;
            window.Top = top;
        }

        protected override void Show(Window window)
        {
            NonTopmostPopup.DisableTopMost = true;

            if (SingleOnly)
            {
                var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                windowController.AddWindow(ViewName, window, () => window.Close());
            }

            if (IsModal)
            {
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
                return;
            }

            window.Show();
        }
    }
}