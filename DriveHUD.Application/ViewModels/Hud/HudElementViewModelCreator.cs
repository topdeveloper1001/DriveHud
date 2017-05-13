﻿//-----------------------------------------------------------------------
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
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Defines methods to create <see cref="HudElementViewModel"/>
    /// </summary>
    internal class HudElementViewModelCreator : IHudElementViewModelCreator
    {
        /// <summary>
        /// Creates <see cref="HudElementViewModel"/> based on the specific <see cref="HudElementViewModelCreationInfo"/>
        /// </summary>
        /// <param name="creationInfo">Creation Info</param>      
        /// <returns><see cref="HudElementViewModel"/></returns>
        public HudElementViewModel Create(HudElementViewModelCreationInfo creationInfo)
        {
            Check.ArgumentNotNull(() => creationInfo);
            Check.Require(creationInfo.HudLayoutInfo != null, "HudLayoutInfo must be set.");

            var hudElementViewModel = new HudElementViewModel(creationInfo.HudLayoutInfo.LayoutTools.Select(x => x.Clone()));
            hudElementViewModel.Seat = creationInfo.SeatNumber;
            hudElementViewModel.Opacity = creationInfo.HudLayoutInfo.Opacity;

            try
            {
                hudElementViewModel.Tools.ForEach(x => x.InitializePositions(creationInfo.PokerSite, creationInfo.HudLayoutInfo.TableType, creationInfo.GameType));
            }
            catch (DHBusinessException e)
            {
                LogProvider.Log.Error(this, $"Could not configure positions for {creationInfo.HudLayoutInfo.Name}", e);
            }

            return hudElementViewModel;
        }
    }
}