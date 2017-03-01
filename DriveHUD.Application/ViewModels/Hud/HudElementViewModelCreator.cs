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
using DriveHUD.Common.Log;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Creates HUD element based on elements on table
    /// </summary>
    internal class HudElementViewModelCreator : IHudElementViewModelCreator
    {
        /// <summary>
        /// Create HUD element based on table settings
        /// </summary>
        /// <param name="creationInfo">Creation Info</param>      
        /// <returns>Hud panel element view model</returns>
        public HudElementViewModel Create(HudElementViewModelCreationInfo creationInfo)
        {
            Check.ArgumentNotNull(() => creationInfo);
            Check.ArgumentNotNull(() => creationInfo.HudLayoutInfo);
            Check.ArgumentNotNull(() => creationInfo.HudLayoutInfo.UiPositionsInfo);
            Check.ArgumentNotNull(() => creationInfo.HudLayoutInfo.HudPositionsInfo);
            Check.ArgumentNotNull(() => creationInfo.PreparedHudElements);

            var hudElementViewModel = creationInfo.PreparedHudElements.FirstOrDefault(x => x.Seat == creationInfo.SeatNumber);

            if (hudElementViewModel == null)
            {
                LogProvider.Log.Error(this, $"Could not find data for seat {creationInfo.SeatNumber} in {creationInfo.HudLayoutInfo.Name}. Layout is broken.");
                return null;
            }

            var positionInfo = creationInfo.HudLayoutInfo.HudPositionsInfo.FirstOrDefault(x => x.GameType == creationInfo.GameType && x.PokerSite == creationInfo.PokerSite);

            if (positionInfo == null)
            {
                LogProvider.Log.Error(this, $"Could not find data for position info {creationInfo.SeatNumber} in {creationInfo.HudLayoutInfo.Name}. Layout is broken.");
                return hudElementViewModel;
            }

            var positions = positionInfo.HudPositions.FirstOrDefault(x => x.Seat == creationInfo.SeatNumber);

            if (positions == null)
            {
                LogProvider.Log.Error(this, $"Could not find data for positions {creationInfo.SeatNumber} in {creationInfo.HudLayoutInfo.Name}. Layout is broken.");
                return hudElementViewModel;
            }

            hudElementViewModel.Position = positions.Position;

            var uiPositions = creationInfo.HudLayoutInfo.UiPositionsInfo.FirstOrDefault(x => x.Seat == creationInfo.SeatNumber);

            if (uiPositions == null)
            {
                LogProvider.Log.Error(this, $"Could not find data for ui positions {creationInfo.SeatNumber} in {creationInfo.HudLayoutInfo.Name}. Layout is broken. Default data will be used.");
                return hudElementViewModel;
            }

            hudElementViewModel.Width = uiPositions.Width;
            hudElementViewModel.Height = uiPositions.Height;
            hudElementViewModel.Opacity = creationInfo.HudLayoutInfo.HudOpacity;

            return hudElementViewModel;
        }
    }
}