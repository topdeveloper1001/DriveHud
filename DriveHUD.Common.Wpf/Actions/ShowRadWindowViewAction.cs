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
using Microsoft.Practices.ServiceLocation;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using Telerik.Windows.Controls;
using DriveHUD.Common.Wpf.Mvvm;

namespace DriveHUD.Common.Wpf.Actions
{
    public class ShowRadWindowViewAction : ShowViewActionBase<RadWindow>
    {
        protected override void Activate(RadWindow window)
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

            window.BringToFront();
        }

        protected override void CloseWindow(RadWindow window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        protected override RadWindow CreateWindow(INotification context)
        {
            var window = new RadWindow
            {
                Header = context != null ? context.Title : string.Empty,
                Owner = Application.Current.MainWindow
            };

            window.Loaded += (o, e) =>
            {
                var parent = window.Parent as Window;

                if (parent != null)
                {
                    parent.Title = context != null ? context.Title : string.Empty;
                    parent.ShowInTaskbar = false;
                }
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

        protected override void OnClosed(RadWindow window, Action callback)
        {
            EventHandler<WindowClosedEventArgs> handler = null;

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
                    windowController.RemoveWindow(ViewName);
                }

                callback?.Invoke();
            };

            window.Closed += handler;
        }

        protected override void OnClosing(RadWindow window, ICloseableViewModel viewModel)
        {
            EventHandler<WindowPreviewClosedEventArgs> handler = null;

            handler = (s, e) =>
            {
                if (!viewModel.OnClosing())
                {
                    e.Cancel = true;
                    return;
                }

                window.PreviewClosed -= handler;
            };

            window.PreviewClosed += handler;
        }

        protected override void SetWindowPosition(RadWindow window, double left, double top)
        {
            if (window == null)
            {
                return;
            }

            window.Left = left;
            window.Top = top;
        }

        protected override void Show(RadWindow window)
        {
            NonTopmostPopup.DisableTopMost = true;

            if (SingleOnly)
            {
                var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                windowController.AddWindow(ViewName, window, () => window.Close());
            }

            if (IsModal)
            {
                window.ShowDialog();
                return;
            }

            window.Show();
            window.BringToFront();
        }
    }
}