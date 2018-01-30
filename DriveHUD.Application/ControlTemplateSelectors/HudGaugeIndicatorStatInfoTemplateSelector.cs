//-----------------------------------------------------------------------
// <copyright file="HudGaugeIndicatorStatInfoTemplateSelector.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using System.Windows;
using System.Windows.Controls;

namespace DriveHUD.Application.ControlTemplateSelectors
{
    public class HudGaugeIndicatorStatInfoTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var group = item as HudGaugeIndicatorStatsGroupViewModel;

            if (group == null)
            {
                return LineBarStatInfoTemplate;
            }

            switch (group.GroupType)
            {
                case HudGaugeIndicatorStatGroupType.BreakLine:
                    return StatInfoBreakTemplate;
                case HudGaugeIndicatorStatGroupType.LineBar:
                    return LineBarStatInfoTemplate;
                case HudGaugeIndicatorStatGroupType.Text:
                    return StatInfoTemplate;
            }

            return LineBarStatInfoTemplate;
        }

        public DataTemplate StatInfoTemplate { get; set; }

        public DataTemplate LineBarStatInfoTemplate { get; set; }

        public DataTemplate StatInfoBreakTemplate { get; set; }
    }
}