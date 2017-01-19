//-----------------------------------------------------------------------
// <copyright file="HudElementViewModelCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using Model.Enums;
using System.Linq;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Creates HUD element based on elements on table
    /// </summary>
    internal class HudElementViewModelCreator : IHudElementViewModelCreator
    {
        /// <summary>
        /// Create HUD element based on table settings
        /// </summary>
        /// <param name="tableDefinition">Table key</param>
        /// <param name="hudViewModel">Hud view model</param>
        /// <param name="seatNumber">Seat number</param>
        /// <param name="hudType">Hud type</param>
        /// <returns>Hud panel element view model</returns>
        public HudElementViewModel Create(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType,
            HudViewModel hudViewModel, int seatNumber, HudType hudType)
        {
            Check.ArgumentNotNull(() => hudViewModel);

            var hudTableViewModel = hudViewModel.HudTableViewModels.FirstOrDefault(h => h.TableType == tableType);


            var hudElementTemplate = hudType == HudType.Plain
                ? hudTableViewModel?.HudElements.FirstOrDefault(
                    x => x.Seat == seatNumber && x.HudViewType == HudViewType.Plain)
                : hudTableViewModel?.HudElements.FirstOrDefault(
                    x => x.Seat == seatNumber && x.HudViewType != HudViewType.Plain);

            var hudElementViewModel = hudElementTemplate?.Clone();
            hudElementViewModel.HudViewType = pokerSite == Entities.EnumPokerSites.Bodog ||
                                              pokerSite == Entities.EnumPokerSites.Bovada ||
                                              pokerSite == Entities.EnumPokerSites.Ignition
                ? hudViewModel.HudViewType
                : Entities.HudViewType.Plain;

            return hudElementViewModel;
        }
    }
}