//-----------------------------------------------------------------------
// <copyright file="HudPlayerIconToolTipBehavior.cs" company="Ace Poker Solutions">
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
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudPlayerIconToolTipBehavior : HudBaseToolTipBehavior<FrameworkElement>
    {
        protected override void OnHudElementViewModelChanged()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            var playerIconViewModel = AssociatedObject.DataContext as HudPlayerIconViewModel;

            if (playerIconViewModel == null)
            {
                return;
            }

            ConfigureToolTip(AssociatedObject, playerIconViewModel);
        }

        protected override HudBaseToolViewModel[] GetToolTipViewModels(object toolTipSource)
        {
            var playerIconViewModel = toolTipSource as HudPlayerIconViewModel;

            if (playerIconViewModel == null)
            {
                return new HudBaseToolViewModel[0];
            }

            var toolTipViewModels = HudElementViewModel.Tools
               .OfType<HudGraphViewModel>()
               .Where(x => x.ParentToolId != null && x.ParentToolId == playerIconViewModel.Id)
               .OfType<HudBaseToolViewModel>()
               .ToArray();

            return toolTipViewModels;
        }
    }
}