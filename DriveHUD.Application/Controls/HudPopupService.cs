//-----------------------------------------------------------------------
// <copyright file="HudPopupService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Represents a service that provides properties to control the display
    /// and behavior of hud popup
    /// </summary>
    public static class HudPopupService
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="HudPopupService.Popup"/> attached property.
        /// </summary>
        public static readonly DependencyProperty PopupProperty = DependencyProperty.RegisterAttached("Popup", typeof(object), typeof(HudPopupService), new FrameworkPropertyMetadata(null, OnPopupPropertyChanged));

        /// <summary>
        /// Gets the value of the <see cref="HudPopupService.Popup"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's <see cref="HudPopupService.Popup"/> property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static object GetPopup(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return element.GetValue(PopupProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HudPopupService.Popup"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetPopup(DependencyObject element, object value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(PopupProperty, value);
        }

        /// <summary>
        /// Callback for the <see cref="HudPopupService.Popup"/> attached property 
        /// property for an object.
        /// </summary>  
        private static void OnPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as FrameworkElement;
            var popup = e.NewValue as Popup;

            InitializePopup(element, popup);
        }

        /// <summary>
        /// Identifies the <see cref="HudPopupService.InitialShowDelay"/> attached property.
        /// </summary>
        public static readonly DependencyProperty InitialShowDelayProperty = DependencyProperty.RegisterAttached("InitialShowDelay", typeof(int), typeof(HudPopupService));

        /// <summary>
        /// Gets the value of the <see cref="HudPopupService.InitialShowDelay"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's <see cref="HudPopupService.InitialShowDelay"/> property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static int GetInitialShowDelay(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (int)element.GetValue(InitialShowDelayProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HudPopupService.InitialShowDelay"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetInitialShowDelay(DependencyObject element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(InitialShowDelayProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HudPopupService.CloseDelay"/> attached property.
        /// </summary>
        public static readonly DependencyProperty CloseDelayProperty = DependencyProperty.RegisterAttached("CloseDelay", typeof(int), typeof(HudPopupService));

        /// <summary>
        /// Gets the value of the <see cref="HudPopupService.CloseDelay"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's <see cref="HudPopupService.CloseDelay"/> property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static int GetCloseDelay(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (int)element.GetValue(CloseDelayProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HudPopupService.CloseDelay"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetCloseDelay(DependencyObject element, int value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(CloseDelayProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HudPopupService.VerticalOffset"/> attached property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(HudPopupService));

        /// <summary>
        /// Gets the value of the <see cref="HudPopupService.VerticalOffset"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's <see cref="HudPopupService.VerticalOffset"/> property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetVerticalOffset(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (double)element.GetValue(VerticalOffsetProperty);
        }


        /// <summary>
        /// Sets the value of the <see cref="HudPopupService.VerticalOffset"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetVerticalOffset(DependencyObject element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(VerticalOffsetProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HudPopupService.HorizontalOffset"/> attached property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(HudPopupService));

        /// <summary>
        /// Gets the value of the <see cref="HudPopupService.HorizontalOffset"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's <see cref="HudPopupService.HorizontalOffset"/> property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static double GetHorizontalOffset(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HudPopupService.HorizontalOffset"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetHorizontalOffset(DependencyObject element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(HorizontalOffsetProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HudPopupService.Placement"/> attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof(PlacementMode), typeof(HudPopupService));

        /// <summary>
        /// Gets the value of the <see cref="HudPopupService.Placement"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's <see cref="HudPopupService.Placement"/> property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static PlacementMode GetPlacement(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (PlacementMode)element.GetValue(PlacementProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="HudPopupService.Placement"/> attached
        /// property for an object.
        /// </summary>
        /// <param name="element">The object to which the attached property is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValue(PlacementProperty, value);
        }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Initializes the specified <see cref="Popup"/> for the specified <see cref="FrameworkElement"/>
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> for which <see cref="Popup"/> is initialized</param>
        /// <param name="popup">The <see cref="Popup"/> to initialize</param>
        private static void InitializePopup(FrameworkElement element, Popup popup)
        {
            if (element == null || popup == null)
            {
                return;
            }

            var placement = GetPlacement(element);
            var verticalOffset = GetVerticalOffset(element);
            var horizontalOffset = GetHorizontalOffset(element);
            var initialShowDelay = GetInitialShowDelay(element);
            var closeDelay = GetCloseDelay(element);

            popup.PlacementTarget = element;
            popup.Placement = placement;
            popup.PopupAnimation = PopupAnimation.Fade;
            popup.VerticalOffset = verticalOffset;
            popup.HorizontalOffset = horizontalOffset;

            void openTimerTick(object t, EventArgs x) => popup.IsOpen = true;
            void closeTimerTick(object t, EventArgs x) => popup.IsOpen = false;

            var openTimer = new DispatcherTimer();
            openTimer.Interval = TimeSpan.FromMilliseconds(initialShowDelay);
            openTimer.Tick += openTimerTick;

            var closeTimer = new DispatcherTimer();
            closeTimer.Interval = TimeSpan.FromMilliseconds(closeDelay);
            closeTimer.Tick += closeTimerTick;

            void popupMouseEnter(object s, MouseEventArgs a)
            {
                closeTimer.Stop();
            }

            void popupMouseLeave(object s, MouseEventArgs a)
            {
                closeTimer.Start();
            }

            popup.MouseEnter += popupMouseEnter;
            popup.MouseLeave += popupMouseLeave;

            void elementMouseEnter(object s, MouseEventArgs a)
            {
                closeTimer.Stop();
                openTimer.Start();
            }

            void elementMouseLeave(object s, MouseEventArgs a)
            {
                openTimer.Stop();
                closeTimer.Start();
            }

            element.MouseEnter += elementMouseEnter;
            element.MouseLeave += elementMouseLeave;

            element.Unloaded += (s, a) =>
            {
                element.MouseEnter -= elementMouseEnter;
                element.MouseLeave -= elementMouseLeave;
                popup.MouseEnter -= popupMouseEnter;
                popup.MouseLeave -= popupMouseLeave;

                openTimer.Stop();
                closeTimer.Stop();

                openTimer.Tick -= openTimerTick;
                closeTimer.Tick -= closeTimerTick;

                openTimer = null;
                closeTimer = null;
            };
        }

        #endregion
    }
}