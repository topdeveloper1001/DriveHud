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
using DriveHUD.Common.Linq;
using DriveHUD.Common.Reflection;
using DriveHUD.ViewModels;
using Model.Enums;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Behavior for <see cref="WrapPanel" /> which display specified stats
    /// </summary>
    public class HudStatsBehavior : Behavior<WrapPanel>
    {
        private static DependencyProperty StatInfoSourceProperty = DependencyProperty.RegisterAttached("StatInfoSource", typeof(ReactiveList<StatInfo>), typeof(HudStatsBehavior), new PropertyMetadata(OnStatInfoSourcePropertyChanged));

        public ReactiveList<StatInfo> StatInfoSource
        {
            get
            {
                return (ReactiveList<StatInfo>)GetValue(StatInfoSourceProperty);
            }
            set
            {
                SetValue(StatInfoSourceProperty, value);
            }
        }

        private static DependencyProperty PlayerIconSourceProperty = DependencyProperty.RegisterAttached("PlayerIconSource", typeof(ImageSource), typeof(HudStatsBehavior), new PropertyMetadata(OnPlayerIconSourcePropertyChanged));

        public ImageSource PlayerIconSource
        {
            get
            {
                return (ImageSource)GetValue(PlayerIconSourceProperty);
            }
            set
            {
                SetValue(PlayerIconSourceProperty, value);
            }
        }

        private static DependencyProperty IsDefaultImageProperty = DependencyProperty.RegisterAttached("IsDefaultImage", typeof(bool), typeof(HudStatsBehavior), new PropertyMetadata(OnIsDefaultImagePropertyChanged));

        public bool IsDefaultImage
        {
            get
            {
                return (bool)GetValue(IsDefaultImageProperty);
            }
            set
            {
                SetValue(IsDefaultImageProperty, value);
            }
        }

        private static DependencyProperty DefaultImageStyleProperty = DependencyProperty.RegisterAttached("DefaultImageStyle", typeof(Style), typeof(HudStatsBehavior));

        public Style DefaultImageStyle
        {
            get
            {
                return (Style)GetValue(DefaultImageStyleProperty);
            }
            set
            {
                SetValue(DefaultImageStyleProperty, value);
            }
        }

        private static DependencyProperty SeparatorProperty = DependencyProperty.RegisterAttached("Separator", typeof(string), typeof(HudStatsBehavior));

        public string Separator
        {
            get
            {
                return (string)GetValue(SeparatorProperty);
            }
            set
            {
                SetValue(SeparatorProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
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

        private static void OnPlayerIconSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HudStatsBehavior;
            var statInfoSource = e.NewValue as ImageSource;

            if (statInfoSource == null || source == null || source.AssociatedObject == null)
            {
                return;
            }

            source.Update();
        }

        private static void OnIsDefaultImagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HudStatsBehavior;
            var statInfoSource = e.NewValue as bool?;

            if (statInfoSource == null || source == null || source.AssociatedObject == null)
            {
                return;
            }

            source.Update();
        }

        private static void OnStatInfoSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HudStatsBehavior;

            var statInfoSource = e.NewValue as ReactiveList<StatInfo>;
            var oldInfoSource = e.OldValue as ReactiveList<StatInfo>;

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

        private void Update()
        {
            if (StatInfoSource == null)
            {
                return;
            }

            var filteredStatInfo = StatInfoSource.Where(x => !x.IsNotVisible).ToList();

            var statInfoGrouped = Split(filteredStatInfo);

            ClearChildrenBindings();
            AssociatedObject.Children.Clear();

            foreach (var statInfoGroup in statInfoGrouped)
            {
                var panel = new WrapPanel();

                panel.Width = double.NaN;
                panel.Height = double.NaN;
                panel.Margin = new Thickness(2, 2, 2, 0);
                panel.Orientation = Orientation.Horizontal;

                foreach (var statInfo in statInfoGroup)
                {
                    FrameworkElement block = null;

                    if (statInfo.Stat == Stat.PlayerInfoIcon)
                    {
                        block = new Image
                        {
                            Width = 16,
                            Height = 16
                        };

                        if (IsDefaultImage)
                        {
                            block.Style = DefaultImageStyle;
                        }
                        else
                        {
                            ((Image)block).Source = PlayerIconSource;
                        }
                    }
                    else
                    {
                        block = CreateStatBlock(statInfo);
                    }

                    panel.Children.Add(block);

                    if (statInfo != statInfoGroup.Last())
                    {
                        var separator = new TextBlock
                        {
                            Text = Separator,
                            Foreground = new SolidColorBrush(Colors.White),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        panel.Children.Add(separator);
                    }
                }

                AssociatedObject.Children.Add(panel);
            }
        }

        private static TextBlock CreateStatBlock(StatInfo statInfo)
        {
            var block = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
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

            // to remove
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