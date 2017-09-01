//-----------------------------------------------------------------------
// <copyright file="PopupAction.cs" company="Ace Poker Solutions">
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
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace DriveHUD.Common.Wpf.Actions
{
    public abstract class PopupAction<T> : TriggerAction<FrameworkElement> where T : ContentControl
    {
        /// <summary>
        /// The content of the child window to display as part of the pop-up.
        /// </summary>
        public static readonly DependencyProperty WindowContentProperty = DependencyProperty.Register(
                "WindowContent",
                typeof(FrameworkElement),
                typeof(PopupAction<T>),
                new PropertyMetadata(null));

        /// <summary>
        /// Determines if the content should be shown in a modal window or not.
        /// </summary>
        public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(
                "IsModal",
                typeof(bool),
                typeof(PopupAction<T>),
                new PropertyMetadata(null));

        /// <summary>
        /// Determines if the content should be initially shown centered over the view that raised the interaction request or not.
        /// </summary>
        public static readonly DependencyProperty CenterOverAssociatedObjectProperty = DependencyProperty.Register(
                "CenterOverAssociatedObject",
                typeof(bool),
                typeof(PopupAction<T>),
                new PropertyMetadata(null));


        // Style of the window
        public static readonly DependencyProperty WindowStyleProperty =
            DependencyProperty.Register("WindowStyle",
                typeof(Style),
                typeof(PopupAction<T>),
                new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the content of the window.
        /// </summary>
        public FrameworkElement WindowContent
        {
            get { return (FrameworkElement)GetValue(WindowContentProperty); }
            set { SetValue(WindowContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the window will be modal or not.
        /// </summary>
        public bool IsModal
        {
            get { return (bool)GetValue(IsModalProperty); }
            set { SetValue(IsModalProperty, value); }
        }

        /// <summary>
        /// Gets or sets if the window will be initially shown centered over the view that raised the interaction request or not.
        /// </summary>
        public bool CenterOverAssociatedObject
        {
            get { return (bool)GetValue(CenterOverAssociatedObjectProperty); }
            set { SetValue(CenterOverAssociatedObjectProperty, value); }
        }

        /// <summary>
        /// Gets or sets style of the window
        /// </summary>
        public Style WindowStyle
        {
            get { return (Style)GetValue(WindowStyleProperty); }
            set { SetValue(WindowStyleProperty, value); }
        }

        /// <summary>
        /// Displays the child window and collects results for <see cref="IInteractionRequest"/>.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;

            if (args == null)
            {
                return;
            }

            // If the WindowContent shouldn't be part of another visual tree.
            if (WindowContent != null && WindowContent.Parent != null)
            {
                return;
            }

            var wrapperWindow = GetWindow(args.Context);

            wrapperWindow.BorderBrush = null;

            // We invoke the callback when the interaction's window is closed.
            var callback = args.Callback;

            OnClosed(wrapperWindow, callback);

            if (CenterOverAssociatedObject && AssociatedObject != null)
            {
                // If we should center the pop-up over the parent window we subscribe to the SizeChanged event
                // so we can change its position after the dimensions are set.
                SizeChangedEventHandler sizeHandler = null;

                sizeHandler = (o, e) =>
                {
                    wrapperWindow.SizeChanged -= sizeHandler;

                    var view = AssociatedObject;
                    var position = view.PointToScreen(new Point(0, 0));

                    var top = position.Y + ((view.ActualHeight - wrapperWindow.ActualHeight) / 2);
                    var left = position.X + ((view.ActualWidth - wrapperWindow.ActualWidth) / 2);

                    SetWindowPosition(wrapperWindow, left, top);
                };

                wrapperWindow.SizeChanged += sizeHandler;
            }
          
            Show(wrapperWindow);
        }

        /// <summary>
        /// Returns the window to display as part of the trigger action.
        /// </summary>
        /// <param name="notification">The notification to be set as a DataContext in the window.</param>
        /// <returns></returns>
        protected virtual T GetWindow(INotification notification)
        {
            if (WindowContent == null)
            {
                throw new NullReferenceException("WindowContent has to be set");
            }

            var wrapperWindow = CreateWindow();

            ApplyStyles(wrapperWindow, notification);

            if (WindowStyle != null)
            {
                wrapperWindow.Style = WindowStyle;
            }

            // If the WindowContent does not have its own DataContext, it will inherit this one.
            wrapperWindow.DataContext = notification;
            wrapperWindow.Tag = "PopupWindow";

            PrepareContentForWindow(notification, wrapperWindow);

            return wrapperWindow;
        }

        /// <summary>
        /// Create window
        /// </summary>
        /// <returns>Window</returns>
        protected abstract T CreateWindow();

        /// <summary>
        /// Apply styles to window
        /// </summary>
        /// <param name="window">Window</param>
        /// <param name="notification">Notification</param>
        protected abstract void ApplyStyles(T window, INotification notification);

        /// <summary>
        /// Checks if the WindowContent or its DataContext implements <see cref="IInteractionRequestAware"/>.
        /// If so, it sets the corresponding value.
        /// Also, if WindowContent does not have a RegionManager attached, it creates a new scoped RegionManager for it.
        /// </summary>
        /// <param name="notification">The notification to be set as a DataContext in the HostWindow.</param>
        /// <param name="wrapperWindow">The HostWindow</param>
        protected virtual void PrepareContentForWindow(INotification notification, T wrapperWindow)
        {
            if (WindowContent == null || wrapperWindow == null)
            {
                return;
            }

            // We set the WindowContent as the content of the window. 
            wrapperWindow.Content = WindowContent;

            // If the WindowContent's DataContext implements IInteractionRequestAware we set the corresponding properties.
            var interactionAware = WindowContent.DataContext as IInteractionRequestAware;

            if (interactionAware == null)
            {
                // If the WindowContent implements IInteractionRequestAware we set the corresponding properties.
                interactionAware = WindowContent as IInteractionRequestAware;

                if (interactionAware == null)
                {
                    if (WindowContent.DataContext is PopupActionNotification)
                    {
                        var dataContext = WindowContent.DataContext as PopupActionNotification;
                        dataContext.OnCloseRaised = () => CloseWindow(wrapperWindow);
                    }

                    return;
                }
            }

            interactionAware.Notification = notification;
            interactionAware.FinishInteraction = () => CloseWindow(wrapperWindow);
        }

        protected abstract void CloseWindow(T window);

        protected abstract void OnClosed(T window, Action callback);

        protected abstract void SetWindowPosition(T window, double left, double top);

        protected abstract void Show(T window);
    }
}