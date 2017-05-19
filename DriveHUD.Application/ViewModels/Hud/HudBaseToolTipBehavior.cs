﻿//-----------------------------------------------------------------------
// <copyright file="HudBaseToolTipBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.ServiceLocation;
using Model.Stats;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Represents base tool tip behavior for <see cref="FrameworkElement"/> based controls
    /// </summary>
    /// <typeparam name="T"><see cref="FrameworkElement"/></typeparam>
    public abstract class HudBaseToolTipBehavior<T> : Behavior<T> where T : FrameworkElement
    {
        private static DependencyProperty HudElementViewModelProperty = DependencyProperty.RegisterAttached("HudElementViewModel", typeof(HudElementViewModel),
            typeof(HudBaseToolTipBehavior<T>), new PropertyMetadata(OnHudElementViewModelChanged));

        public HudElementViewModel HudElementViewModel
        {
            get
            {
                return (HudElementViewModel)GetValue(HudElementViewModelProperty);
            }
            set
            {
                SetValue(HudElementViewModelProperty, value);
            }
        }

        private static void OnHudElementViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HudBaseToolTipBehavior<T>;

            if (behavior == null)
            {
                return;
            }

            behavior.OnHudElementViewModelChanged();
        }

        protected virtual void OnHudElementViewModelChanged()
        {
        }        

        protected virtual void ConfigureSimpleToolTip(FrameworkElement element, object toolTipSource, string path)
        {
            if (toolTipSource == null)
            {
                return;
            }

            var toolTipBinding = new Binding(path)
            {
                Source = toolTipSource,
                Mode = BindingMode.OneWay
            };

            element.SetBinding(FrameworkElement.ToolTipProperty, toolTipBinding);
        }

        protected virtual void ConfigureSimpleToolTip(FrameworkElement element, object toolTipSource)
        {
            var statInfo = toolTipSource as StatInfo;

            if (statInfo != null)
            {
                ConfigureSimpleToolTip(element, toolTipSource, nameof(StatInfo.ToolTip));
            }
        }

        protected virtual HudBaseToolViewModel[] GetToolTipViewModels(object toolTipSource)
        {
            var statInfo = toolTipSource as StatInfo;

            if (statInfo == null || HudElementViewModel == null)
            {
                return new HudBaseToolViewModel[0];
            }

            var toolTipViewModels = HudElementViewModel.Tools
              .OfType<IHudBaseStatToolViewModel>()
              .Where(x => x.BaseStat != null && x.BaseStat.Stat == statInfo.Stat)
              .OfType<HudBaseToolViewModel>()
              .ToArray();

            return toolTipViewModels;
        }

        protected virtual void ConfigureToolTip(FrameworkElement element, object toolTipSource)
        {
            var toolTipViewModels = GetToolTipViewModels(toolTipSource);

            if (toolTipViewModels.Length <= 0)
            {
                ConfigureSimpleToolTip(element, toolTipSource);
                return;
            }

            var hudPanelService = ServiceLocator.Current.GetInstance<IHudPanelService>();

            var frameworkElementFactory = hudPanelService.CreateFrameworkElementFactory(toolTipViewModels.First());

            if (frameworkElementFactory == null)
            {
                return;
            }

            var dataContextBinding = new Binding { Source = toolTipViewModels };
            frameworkElementFactory.SetBinding(FrameworkElement.DataContextProperty, dataContextBinding);

            var template = new DataTemplate();
            template.VisualTree = frameworkElementFactory;
            template.Seal();

            AttachToolTip(element, template);
        }

        protected virtual void AttachToolTip(FrameworkElement element, DataTemplate toolTipContentTemplate)
        {
            var tooltip = new ToolTip();
            tooltip.BorderThickness = new Thickness(0, 0, 0, 0);
            tooltip.Background = new SolidColorBrush(Colors.Transparent);
            tooltip.ContentTemplate = toolTipContentTemplate;

            ToolTipService.SetInitialShowDelay(element, 1);
            ToolTipService.SetShowDuration(element, 60000);
            ToolTipService.SetVerticalOffset(element, -5.0);
            ToolTipService.SetPlacement(element, System.Windows.Controls.Primitives.PlacementMode.Top);
            ToolTipService.SetToolTip(element, tooltip);
        }
    }
}