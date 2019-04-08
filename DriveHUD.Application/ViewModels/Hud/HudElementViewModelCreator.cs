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

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Defines methods to create <see cref="HudElementViewModel"/>
    /// </summary>
    internal class HudElementViewModelCreator : IHudElementViewModelCreator
    {
        private static readonly HudDesignerToolType[] PSDeniedTools = new HudDesignerToolType[] { HudDesignerToolType.PlayerProfileIcon, HudDesignerToolType.HeatMap, HudDesignerToolType.BumperStickers };

        /// <summary>
        /// Creates <see cref="HudElementViewModel"/> based on the specific <see cref="HudElementViewModelCreationInfo"/>
        /// </summary>
        /// <param name="creationInfo">Creation Info</param>      
        /// <returns><see cref="HudElementViewModel"/></returns>
        public HudElementViewModel Create(HudElementViewModelCreationInfo creationInfo)
        {
            Check.ArgumentNotNull(() => creationInfo);
            Check.Require(creationInfo.HudLayoutInfo != null, "HudLayoutInfo must be set.");

            var layoutTools = GetHudLayoutTools(creationInfo);

            var hudElementViewModel = new HudElementViewModel(layoutTools)
            {
                Seat = creationInfo.SeatNumber,
                Opacity = creationInfo.HudLayoutInfo.Opacity
            };

            try
            {
                hudElementViewModel.Tools.ForEach(x =>
                {
                    x.InitializePositions(creationInfo.PokerSite, creationInfo.HudLayoutInfo.TableType, creationInfo.GameType);
                    ApplyRestrictions(x, creationInfo);
                });
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not configure positions for seat #{creationInfo.SeatNumber} using {creationInfo.HudLayoutInfo.Name}, {(int)creationInfo.HudLayoutInfo.TableType}-max", e);
            }

            return hudElementViewModel;
        }

        /// <summary>
        /// Gets the array of <see cref="HudLayoutTool"/> for the specified layout
        /// </summary>
        /// <param name="creationInfo"><see cref="HudElementViewModelCreationInfo"/> to get the array of <see cref="HudLayoutTool"/></param>        
        private IEnumerable<HudLayoutTool> GetHudLayoutTools(HudElementViewModelCreationInfo creationInfo)
        {
            var layoutTools = creationInfo.HudLayoutInfo.LayoutTools.Select(x => x.Clone()).ToArray();

            if (creationInfo.PokerSite == EnumPokerSites.PokerStars)
            {
                layoutTools = layoutTools.Where(x => !PSDeniedTools.Contains(x.ToolType)).ToArray();
            }

            return layoutTools;
        }

        /// <summary>
        /// Applies restrictions to the specified <see cref="HudBaseToolViewModel"/> for the specified <see cref="HudElementViewModelCreationInfo"/> 
        /// </summary>
        /// <param name="toolViewModel"></param>
        /// <param name="creationInfo"></param>
        private void ApplyRestrictions(HudBaseToolViewModel toolViewModel, HudElementViewModelCreationInfo creationInfo)
        {
            if (creationInfo == null || creationInfo.PokerSite != EnumPokerSites.PokerStars)
            {
                return;
            }

            if (toolViewModel is HudGaugeIndicatorViewModel)
            {
                (toolViewModel as HudGaugeIndicatorViewModel).IsGraphIndicatorsDisabled = true;
            }
        }
    }
}