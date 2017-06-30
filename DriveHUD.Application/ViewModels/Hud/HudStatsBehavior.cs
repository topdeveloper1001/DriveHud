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

using DriveHUD.Common.Linq;
using Model.Enums;
using Model.Stats;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Behavior for <see cref="WrapPanel" /> which display specified stats
    /// </summary>
    public class HudStatsBehavior : HudBaseToolTipBehavior<WrapPanel>
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

        private static DependencyProperty StatDataTemplateProperty = DependencyProperty.RegisterAttached("StatDataTemplate", typeof(DataTemplate), typeof(HudStatsBehavior));

        public DataTemplate StatDataTemplate
        {
            get
            {
                return (DataTemplate)GetValue(StatDataTemplateProperty);
            }
            set
            {
                SetValue(StatDataTemplateProperty, value);
            }
        }


        private static DependencyProperty PlayerIconDataTemplateProperty = DependencyProperty.RegisterAttached("PlayerIconDataTemplate", typeof(DataTemplate), typeof(HudStatsBehavior));

        public DataTemplate PlayerIconDataTemplate
        {
            get
            {
                return (DataTemplate)GetValue(PlayerIconDataTemplateProperty);
            }
            set
            {
                SetValue(PlayerIconDataTemplateProperty, value);
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

        protected override void OnHudElementViewModelChanged()
        {
            base.OnHudElementViewModelChanged();
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
                    var block = CreateStatBlock(statInfo);

                    if (block != null)
                    {
                        panel.Children.Add(block);
                    }

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

        protected virtual FrameworkElement CreateStatBlock(StatInfo statInfo)
        {
            var contentTemplate = statInfo.Stat == Stat.PlayerInfoIcon ? PlayerIconDataTemplate : StatDataTemplate;

            if (contentTemplate == null)
            {
                return null;
            }

            var statBlock = new ContentControl
            {
                Content = statInfo,
                DataContext = statInfo,
                ContentTemplate = contentTemplate
            };

            ConfigureToolTip(statBlock, statInfo);

            return statBlock;
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