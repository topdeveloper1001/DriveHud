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
        public HudElementViewModel Create(HudTableDefinition tableDefinition, HudViewModel hudViewModel, int seatNumber, HudType hudType)
        {
            Check.ArgumentNotNull(() => hudViewModel);

            var hudTableViewModel =
                hudViewModel.HudTableViewModels.FirstOrDefault(
                    h =>
                        h.PokerSite == tableDefinition.PokerSite && h.GameType == tableDefinition.GameType &&
                        h.TableType == tableDefinition.TableType);

            var hudElementTemplate = hudTableViewModel?.HudElements.FirstOrDefault(x => x.Seat == seatNumber && x.HudType == hudType);

            var hudElementViewModel = hudElementTemplate?.Clone();

            return hudElementViewModel;
        }
    }
}