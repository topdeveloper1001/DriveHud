﻿//-----------------------------------------------------------------------
// <copyright file="DragAndDropBehavior.cs" company="Ace Poker Solutions">
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
using Model.Stats;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class DragDrop
    {
        public static DataFormat DataFormat { get; } = DataFormats.GetDataFormat("DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop");

        private static DragInfo dragInfo;

        #region Properties

        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop),
              new PropertyMetadata(OnIsDragSourcePropertyChanged));

        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        public static readonly DependencyProperty IsDragTargetProperty = DependencyProperty.RegisterAttached("IsDragTarget", typeof(bool), typeof(DragDrop),
            new PropertyMetadata(OnIsDragTargetPropertyChanged));

        public static bool GetIsDragTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDragTargetProperty);
        }

        public static void SetIsDragTarget(UIElement target, bool value)
        {
            target.SetValue(IsDragTargetProperty, value);
        }

        public static readonly DependencyProperty DragAdornerTemplateProperty = DependencyProperty.RegisterAttached("DragAdornerTemplate", typeof(DataTemplate),
            typeof(DragDrop));

        public static DataTemplate GetDragAdornerTemplate(UIElement target)
        {
            return (DataTemplate)target.GetValue(DragAdornerTemplateProperty);
        }

        public static void SetDragAdornerTemplate(UIElement target, DataTemplate value)
        {
            target.SetValue(DragAdornerTemplateProperty, value);
        }

        public static readonly DependencyProperty DragDropCommandProperty = DependencyProperty.RegisterAttached("DragDropCommand", typeof(ICommand),
            typeof(DragDrop));

        public static ICommand GetDragDropCommand(UIElement target)
        {
            return (ICommand)target.GetValue(DragDropCommandProperty);
        }

        public static void SetDragDropCommand(UIElement target, ICommand value)
        {
            target.SetValue(DragDropCommandProperty, value);
        }

        public static readonly DependencyProperty DragDropDataProperty = DependencyProperty.RegisterAttached("DragDropData", typeof(object),
            typeof(DragDrop));

        public static object GetDragDropData(UIElement target)
        {
            return target.GetValue(DragDropDataProperty);
        }

        public static void SetDragDropData(UIElement target, object value)
        {
            target.SetValue(DragDropDataProperty, value);
        }

        public static readonly DependencyProperty PassClickEventToParentProperty = DependencyProperty.RegisterAttached("PassClickEventToParent", typeof(bool),
            typeof(DragDrop));

        public static bool GetPassClickEventToParent(UIElement target)
        {
            return (bool)target.GetValue(PassClickEventToParentProperty);
        }

        public static void SetPassClickEventToParent(UIElement target, object value)
        {
            target.SetValue(PassClickEventToParentProperty, value);
        }

        private static DragAdorner dragAdorner;

        public static DragAdorner DragAdorner
        {
            get
            {
                return dragAdorner;
            }
            set
            {
                dragAdorner?.Detatch();
                dragAdorner = value;
            }
        }

        #endregion

        private static void OnIsDragSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;

            if (uiElement == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                uiElement.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                uiElement.PreviewMouseMove += OnPreviewMouseMove;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                uiElement.PreviewMouseMove -= OnPreviewMouseMove;
            }
        }

        private static void OnIsDragTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;

            if (uiElement == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                uiElement.AllowDrop = true;
                uiElement.PreviewDragEnter += OnDragEnter;
                uiElement.PreviewDragOver += OnDragOver;
                uiElement.PreviewDragLeave += OnDragLeave;
                uiElement.PreviewDrop += OnDragDrop;
                uiElement.PreviewGiveFeedback += OnDragGiveFeedback;
            }
            else
            {
                uiElement.AllowDrop = false;
                uiElement.PreviewDragEnter -= OnDragEnter;
                uiElement.PreviewDragOver -= OnDragOver;
                uiElement.PreviewDragLeave -= OnDragLeave;
                uiElement.PreviewDrop -= OnDragDrop;
                uiElement.PreviewGiveFeedback -= OnDragGiveFeedback;
            }
        }

        public static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragInfo = new DragInfo(sender, e);

            var uiElement = sender as UIElement;

            if (uiElement == null)
            {
                e.Handled = true;
                return;
            }

            var passClickEventToParent = GetPassClickEventToParent(uiElement);

            if (!passClickEventToParent)
            {
                e.Handled = true;
            }
        }

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement == null || dragInfo == null)
            {
                return;
            }

            var position = e.GetPosition((IInputElement)sender);
            var diff = position - dragInfo.DragStartPosition;

            if (e.LeftButton == MouseButtonState.Pressed && ((Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                (Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)))
            {
                var data = GetDragDropData(dragInfo.VisualSource);           

                var dataObject = new DataObject(DataFormat.Name, data ?? new object());

                try
                {
                    System.Windows.DragDrop.DoDragDrop(uiElement, dataObject, DragDropEffects.Move | DragDropEffects.Copy);
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(typeof(DragDrop), "DragDrop OnPreviewMouseMove failed", ex);
                }
            }
        }

        private static void OnDragGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
        }

        private static void OnDragDrop(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement == null)
            {
                return;
            }

            UpdateTargetDataContext<StatInfo>(sender, x => x.IsSelected = false);

            var dragDropCommand = GetDragDropCommand(uiElement);

            if (dragDropCommand != null)
            {
                var dragDropDataObject = CreateDragDropDataObject(uiElement, e);

                dragDropCommand.Execute(dragDropDataObject);
            }

            DragAdorner = null;
            dragInfo = null;
            e.Handled = true;
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;

            if (uiElement == null)
            {
                return;
            }

            var dragDropCommand = GetDragDropCommand(uiElement);

            var dragDropDataObject = CreateDragDropDataObject(uiElement, e);

            if (dragDropCommand == null || !dragDropCommand.CanExecute(dragDropDataObject))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (DragAdorner == null && dragInfo != null)
            {
                CreateDragAdorner(dragInfo.VisualSource);
            }

            if (DragAdorner != null)
            {
                var adornerPos = e.GetPosition(DragAdorner.AdornedElement);

                DragAdorner.MousePosition = adornerPos;
                DragAdorner.InvalidateVisual();
            }

            UpdateTargetDataContext<StatInfo>(sender, x => x.IsSelected = true);

            e.Handled = true;
        }

        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            OnDragOver(sender, e);
        }

        private static void OnDragLeave(object sender, DragEventArgs e)
        {
            UpdateTargetDataContext<StatInfo>(sender, x => x.IsSelected = false);
            DragAdorner = null;
        }

        private static void CreateDragAdorner(UIElement uiElement)
        {
            var template = GetDragAdornerTemplate(uiElement);

            if (template == null)
            {
                return;
            }

            var contentPresenter = new ContentPresenter();

            if (uiElement is FrameworkElement)
            {
                contentPresenter.Content = (uiElement as FrameworkElement).DataContext;
            }

            contentPresenter.ContentTemplate = template;

            DragAdorner = new DragAdorner(uiElement, contentPresenter);
        }

        private static void UpdateTargetDataContext<T>(object sender, Action<T> action) where T : class
        {
            var frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null && frameworkElement.DataContext != null && frameworkElement.DataContext is T)
            {
                action(frameworkElement.DataContext as T);
            }
        }

        private static DragDropDataObject CreateDragDropDataObject(UIElement element, DragEventArgs e)
        {
            var dragDropDataObject = new DragDropDataObject
            {
                DragEventArgs = e,
                DropData = e.Data.GetData(DataFormat.Name),
                Position = e.GetPosition(element),
                Source = (element as FrameworkElement)?.DataContext
            };

            return dragDropDataObject;
        }
    }
}