//-----------------------------------------------------------------------
// <copyright file="ClassTemplateSelector.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.TableConfigurators;
using DriveHUD.Entities;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;

namespace DriveHUD.Application.ControlTemplateSelectors
{
    public class HudPanelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var radItem = (item as RadDiagramItem);

            if (radItem == null)
            {
                return null;
            }

            if (radItem.DataContext is ITableSeatArea)
            {
                return null;
            }

            var classViewModel = radItem.DataContext as HudPlayerViewModel;

            if (classViewModel == null)
            {
                var hudType = radItem.Tag as HudViewType?;

                if (hudType == null)
                {
                    return null;
                }

                if (!hudType.HasValue)
                {
                    return PlayerPlaceTemplate;
                }

                if (hudType == HudViewType.Plain)
                {
                    return PlainHudTemplate;
                }

                return RichHudTemplate;
            }

            return PlayerPlaceTemplate;
        }

        public DataTemplate PlayerPlaceTemplate { get; set; }

        public DataTemplate PlainHudTemplate { get; set; }

        public DataTemplate RichHudTemplate { get; set; }
    }
}