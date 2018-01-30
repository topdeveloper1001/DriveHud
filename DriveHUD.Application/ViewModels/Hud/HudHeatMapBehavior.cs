//-----------------------------------------------------------------------
// <copyright file="HudHeatMapBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Cards;
using Model.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudHeatMapBehavior : Behavior<UniformGrid>
    {
        #region Dependency properties

        private static DependencyProperty RangeBlockDataTemplateProperty = DependencyProperty.RegisterAttached("RangeBlockDataTemplate", typeof(DataTemplate),
            typeof(HudHeatMapBehavior), new PropertyMetadata(OnRangeBlockDataTemplateChanged));

        public DataTemplate RangeBlockDataTemplate
        {
            get
            {
                return (DataTemplate)GetValue(RangeBlockDataTemplateProperty);
            }
            set
            {
                SetValue(RangeBlockDataTemplateProperty, value);
            }
        }

        private static DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", typeof(HeatMapDto),
            typeof(HudHeatMapBehavior), new PropertyMetadata(OnItemsSourceChanged));

        public HeatMapDto ItemsSource
        {
            get
            {
                return (HeatMapDto)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        #endregion

        private Dictionary<string, FrameworkElement> rangeBlocks = null;

        protected override void OnAttached()
        {
            base.OnAttached();

            CreateRangeBlocks();
        }

        private void CreateRangeBlocks()
        {
            if (RangeBlockDataTemplate == null || AssociatedObject == null || AssociatedObject.Children == null)
            {
                return;
            }

            rangeBlocks = new Dictionary<string, FrameworkElement>();

            var cardRanges = Card.GetCardRanges();

            AssociatedObject.Columns = AssociatedObject.Rows = Card.PossibleRanksHighCardFirst.Length;

            foreach (var cardRange in cardRanges)
            {
                var content = new HeatMapStatDtoViewModel(cardRange);

                var rangeBlock = new ContentControl
                {
                    DataContext = content,
                    Content = content,
                    ContentTemplate = RangeBlockDataTemplate
                };

                rangeBlocks.Add(cardRange, rangeBlock);
                AssociatedObject.Children.Add(rangeBlock);
            }
        }

        private static void OnRangeBlockDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HudHeatMapBehavior;

            var rangeDataTemplate = e.NewValue as DataTemplate;

            if (source == null || rangeDataTemplate == null || source.AssociatedObject == null)
            {
                return;
            }

            source.CreateRangeBlocks();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as HudHeatMapBehavior;

            var heatMaps = e.NewValue as HeatMapDto;

            if (source == null || heatMaps == null || source.AssociatedObject == null)
            {
                return;
            }

            var maxOccured = heatMaps.OccuredByCardRange.Count > 0 ? heatMaps.OccuredByCardRange.Max(x => x.Value) : 0;

            foreach (var heatMap in heatMaps.OccuredByCardRange)
            {
                if (!source.rangeBlocks.ContainsKey(heatMap.Key))
                {
                    continue;
                }

                var heatMapStatDtoViewModel = source.rangeBlocks[heatMap.Key].DataContext as HeatMapStatDtoViewModel;

                if (heatMapStatDtoViewModel == null)
                {
                    continue;
                }

                heatMapStatDtoViewModel.Occurred = heatMap.Value;
                heatMapStatDtoViewModel.MaxOccurred = maxOccured;
            }
        }
    }
}