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
        /// <param name="tableKey">Table key</param>
        /// <param name="hudViewModel">Hud view model</param>
        /// <param name="seatNumber">Seat number</param>
        /// <param name="hudType">Hud type</param>
        /// <returns>Hud panel element view model</returns>
        public HudElementViewModel Create(int tableKey, HudViewModel hudViewModel, int seatNumber, HudType hudType)
        {
            Check.ArgumentNotNull(() => hudViewModel);

            if (!hudViewModel.HudTableViewModelDictionary.ContainsKey(tableKey))
            {
                return null;
            }

            var hudTableViewModel = hudViewModel.HudTableViewModelDictionary[tableKey];

            var hudElementTemplate = hudTableViewModel.HudElements.FirstOrDefault(x => x.Seat == seatNumber && x.HudType == hudType);

            if (hudElementTemplate == null)
            {
                return null;
            }

            var hudElementViewModel = hudElementTemplate.Clone();

            return hudElementViewModel;
        }
    }
}