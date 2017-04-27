//-----------------------------------------------------------------------
// <copyright file="HudFourStatsBoxToolTipBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Stats;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudFourStatsBoxToolTipBehavior : HudBaseToolTipBehavior<FrameworkElement>
    {
        protected override void OnHudElementViewModelChanged()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            var statInfo = AssociatedObject.DataContext as StatInfo;

            if (statInfo == null)
            {
                return;
            }

            ConfigureToolTip(AssociatedObject, statInfo);
        }
    }
}