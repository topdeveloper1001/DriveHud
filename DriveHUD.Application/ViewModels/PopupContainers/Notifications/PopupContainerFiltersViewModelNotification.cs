//-----------------------------------------------------------------------
// <copyright file="PopupContainerFiltersViewModelNotification.cs" company="Ace Poker Solutions">
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
using Model.Filters;
using Prism.Interactivity.InteractionRequest;

namespace DriveHUD.Application.ViewModels.PopupContainers.Notifications
{
    public class PopupContainerFiltersViewModelNotification : Confirmation
    {
        public PopupContainerFiltersViewModelNotification()
        {
        }

        public PopupContainerFiltersViewModelNotification(object items)
            : this()
        {
        }

        public FilterTuple FilterTuple { get; set; }
    }

    public class PopupContainerStickersFiltersViewModelNotification : PopupContainerFiltersViewModelNotification
    {
        public HudBumperStickerType Sticker { get; set; }

        public PopupContainerStickersFiltersViewModelNotification()
        {
        }
    }
}