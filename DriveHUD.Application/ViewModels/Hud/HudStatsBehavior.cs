﻿//-----------------------------------------------------------------------
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

using DriveHUD.Application.ValueConverters;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Reflection;
using DriveHUD.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System;
using Telerik.Windows.Controls;
using DriveHUD.Application.Controls;
using System.Diagnostics;
using System.Windows.Documents;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudStatsBehavior : Behavior<WrapPanel>
    {
        private const double VERTICAL_PANEL_GAP = 32;
        private const double TOTAL_PANEL_WIDTH = 144;

        private static readonly DependencyProperty IsVerticalProperty = DependencyProperty.Register("IsVertical", typeof(bool), typeof(HudStatsBehavior), new PropertyMetadata(false));

        public bool IsVertical
        {
            get { return (bool)GetValue(IsVerticalProperty); }
            set { SetValue(IsVerticalProperty, value); }
        }

        private static readonly DependencyProperty IsMainStatsPanelEnabledProperty = DependencyProperty.RegisterAttached("IsMainStatsPanelEnabled", typeof(bool), typeof(HudStatsBehavior), new PropertyMetadata(true));

        public bool IsMainStatsPanelEnabled
        {
            get { return (bool)GetValue(IsMainStatsPanelEnabledProperty); }
            set
            {
                SetValue(IsMainStatsPanelEnabledProperty, value);
            }
        }

        private static DependencyProperty StatInfoSourceProperty = DependencyProperty.RegisterAttached("StatInfoSource", typeof(ObservableCollection<StatInfo>), typeof(HudStatsBehavior), new PropertyMetadata(OnStatInfoSourcePropertyChanged));

        public ObservableCollection<StatInfo> StatInfoSource
        {
            get
            {
                return (ObservableCollection<StatInfo>)GetValue(StatInfoSourceProperty);
            }
            set
            {
                SetValue(StatInfoSourceProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        }

        private void AssociatedObject_SizeChanged(object sender, RoutedEventArgs e)
        {
            if (IsVertical)
            {
                ConfigureVerticalPanel();
            }
        }

        protected override void OnDetaching()
        {
            Cleanup();
            base.OnDetaching();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            Cleanup();
        }

        private static void OnStatInfoSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HudStatsBehavior;

            var statInfoSource = e.NewValue as ObservableCollection<StatInfo>;
            var oldInfoSource = e.OldValue as ObservableCollection<StatInfo>;

            if (statInfoSource == null || source == null || source.AssociatedObject == null)
            {
                return;
            }

            if (oldInfoSource != null)
            {
                oldInfoSource.CollectionChanged -= source.StatInfoSource_CollectionChanged;
            }

            statInfoSource.CollectionChanged -= source.StatInfoSource_CollectionChanged;
            statInfoSource.CollectionChanged += source.StatInfoSource_CollectionChanged;

            source.Update();
        }

        internal void StatInfoSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();
        }

        internal void Update()
        {
            var filteredStatInfo = new List<StatInfo>();

            var counter = 0;

            if (StatInfoSource == null)
            {
                return;
            }

            foreach (var statInfo in StatInfoSource)
            {
                if (statInfo.IsNotVisible || (statInfo is StatInfoBreak && counter < 4 && IsMainStatsPanelEnabled))
                {
                    continue;
                }

                if (counter > 3 || !IsMainStatsPanelEnabled)
                {
                    filteredStatInfo.Add(statInfo);
                }

                counter++;
            }

            var statInfoGrouped = Split(filteredStatInfo);

            ClearChildrenBindings();
            AssociatedObject.Children.Clear();

            foreach (var statInfoGroup in statInfoGrouped)
            {
                var panel = new WrapPanel();

                panel.Width = double.NaN;
                panel.Height = double.NaN;
                panel.Margin = new Thickness(2, 2, 2, 0);
                panel.Orientation = IsVertical ? Orientation.Vertical : Orientation.Horizontal;

                foreach (var statInfo in statInfoGroup)
                {
                    TextBlock block = CreateStatBlock(statInfo);

                    if (IsVertical)
                    {
                        Border border = new Border();
                        border.Child = block;

                        if (statInfo != statInfoGroup.Last())
                        {
                            border.BorderThickness = new Thickness(0, 0, 0, 1);
                            border.Margin = new Thickness(0, 0, 5, 0);
                            border.BorderBrush = Brushes.White;
                        }

                        panel.Children.Add(border);
                    }
                    else
                    {
                        panel.Children.Add(block);

                        if (statInfo != statInfoGroup.Last())
                        {
                            TextBlock separator = new TextBlock { Text = " | ", Foreground = new SolidColorBrush(Colors.White), VerticalAlignment = VerticalAlignment.Center };
                            panel.Children.Add(separator);
                        }
                    }
                }

                AssociatedObject.Children.Add(panel);
            }
        }

        private void ConfigureVerticalPanel()
        {
            var vm = (AssociatedObject.DataContext as HudElementViewModel);
            var border = AssociatedObject.ParentOfType<Border>();

            if (vm == null || border == null)
            {
                return;
            }

            var topMargin = border.MinHeight < border.ActualHeight ? border.MinHeight - border.ActualHeight : 0d;

            if (vm.IsRightOriented)
            {
                var width = border.ActualWidth;
                if (width > VERTICAL_PANEL_GAP)
                {
                    var margin = VERTICAL_PANEL_GAP - width;
                    border.Margin = new Thickness(margin, topMargin, Math.Abs(margin), Math.Abs(topMargin));
                }
            }
            else
            {
                var margin = TOTAL_PANEL_WIDTH - VERTICAL_PANEL_GAP;

                border.Margin = new Thickness(margin, topMargin, -margin, Math.Abs(topMargin));
            }
        }

        private static TextBlock CreateStatBlock(StatInfo statInfo)
        {
            var block = new TextBlock
            {
                TextWrapping = System.Windows.TextWrapping.Wrap,
                Foreground = new SolidColorBrush(statInfo.CurrentColor),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var label = new Run();
            var caption = new Run();

            label.SetBinding(Run.TextProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.Label)) { Source = statInfo });
            caption.SetBinding(Run.TextProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.Caption)) { Source = statInfo });

            block.Inlines.Add(label);
            block.Inlines.Add(caption);

            block.SetBinding(TextBlock.ForegroundProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.CurrentColor)) { Source = statInfo, Converter = new ValueConverters.ColorToBrushConverter() });
            block.SetBinding(TextBlock.FontFamilyProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.SettingsAppearanceFontFamily)) { Source = statInfo });
            block.SetBinding(TextBlock.FontSizeProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.SettingsAppearanceFontSize)) { Source = statInfo });
            block.SetBinding(TextBlock.FontWeightProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.SettingsAppearanceFontBold)) { Source = statInfo });
            block.SetBinding(TextBlock.FontStyleProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.SettingsAppearanceFontItalic)) { Source = statInfo });
            block.SetBinding(TextBlock.TextDecorationsProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.SettingsAppearanceFontUnderline)) { Source = statInfo });

            if (statInfo.IsStatInfoToolTipAvailable)
            {
                FrameworkElementFactory fef = new FrameworkElementFactory(typeof(HudStatToolTip));
                var datacontextBinding = new Binding() { Source = statInfo };
                fef.SetBinding(FrameworkElement.DataContextProperty, datacontextBinding);

                var template = new DataTemplate();
                template.VisualTree = fef;
                template.Seal();

                var tooltip = new ToolTip();
                tooltip.BorderThickness = new Thickness(0, 0, 0, 0);
                tooltip.Background = new SolidColorBrush(Colors.Transparent);
                tooltip.ContentTemplate = template;

                ToolTipService.SetInitialShowDelay(block, 1);
                ToolTipService.SetShowDuration(block, 60000);
                ToolTipService.SetVerticalOffset(block, -5.0);
                ToolTipService.SetPlacement(block, System.Windows.Controls.Primitives.PlacementMode.Top);
                ToolTipService.SetToolTip(block, tooltip);
            }
            else
            {
                block.SetBinding(TextBlock.ToolTipProperty, new Binding(ReflectionHelper.GetPath<StatInfo>(o => o.ToolTip)) { Source = statInfo });
            }

            return block;
        }

        private List<List<StatInfo>> Split(IList<StatInfo> source)
        {
            var result = new List<List<StatInfo>>();

            int i = 0;

            result.Add(new List<StatInfo>());

            foreach (var item in source)
            {
                if (item is StatInfoBreak)
                {
                    result.Add(new List<StatInfo>());
                    i++;
                    continue;
                }

                result[i].Add(item);

                if (result[i].Count == 4 && IsVertical)
                {
                    result.Add(new List<StatInfo>());
                    i++;
                }
            }

            return result;
        }

        #region Cleanup

        private bool isCleanedup = false;

        private void Cleanup()
        {

            if (!isCleanedup)
            {
                ClearChildrenBindings();

                if (StatInfoSource != null)
                {
                    StatInfoSource.CollectionChanged -= StatInfoSource_CollectionChanged;
                }

                AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
                AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;

                isCleanedup = true;
            }
        }

        private void ClearChildrenBindings()
        {
            AssociatedObject.Children.ForEach(x => ClearBindings(x));
        }

        private void ClearBindings(object elem)
        {
            if ((elem as DependencyObject) != null)
            {
                BindingOperations.ClearAllBindings(elem as DependencyObject);
            }
        }

        #endregion
    }
}