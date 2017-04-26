//-----------------------------------------------------------------------
// <copyright file="HudBaseNonPopupToolViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Base class for non-popup tools
    /// </summary>
    public abstract class HudBaseNonPopupToolViewModel<T> : HudBaseToolViewModel where T : HudLayoutNonPopupTool
    {
        /// <summary>
        /// Gets the default width of the tool
        /// </summary>
        public abstract double DefaultWidth
        {
            get;
        }

        /// <summary>
        /// Gets the default height of the tool
        /// </summary>
        public abstract double DefaultHeight
        {
            get;
        }

        /// <summary>
        /// Initializes UI position and size for the current <see cref="HudBaseNonPopupToolViewModel"/>
        /// </summary>
        /// <exception cref="DHBusinessException" />
        public override void InitializePositions()
        {
            var tool = Tool as T;

            if (tool == null)
            {
                return;
            }

            var uiPosition = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (uiPosition == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not set UI positions {Parent.Seat}"));
            }

            Width = uiPosition.Width != 0 ? uiPosition.Width : DefaultWidth;
            Height = uiPosition.Height != 0 ? uiPosition.Height : DefaultHeight;

            Opacity = Parent.Opacity;
            Position = uiPosition.Position;
        }

        /// <summary>
        /// Initializes position and size for the current <see cref="HudPlainStatBoxViewModel"/> for the specified <see cref="EnumPokerSites"/> and <see cref="EnumGameType"/>
        /// </summary>
        /// <param name="pokerSite"><see cref="EnumPokerSites"/></param>
        /// <param name="gameType"><see cref="EnumGameType"/></param>
        /// <exception cref="DHBusinessException" />
        public override void InitializePositions(EnumPokerSites pokerSite, EnumGameType gameType)
        {
            var tool = Tool as T;

            if (tool == null)
            {
                return;
            }

            var positionInfo = tool.Positions.FirstOrDefault(x => x.PokerSite == pokerSite && x.GameType == gameType);

            if (positionInfo == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not find data for position info {Parent.Seat}"));
            }

            var positions = positionInfo.HudPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (positions == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not find data for positions {Parent.Seat}"));
            }

            Position = positions.Position;
            Opacity = Parent.Opacity;

            var uiPositions = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            Width = uiPositions != null && uiPositions.Width != 0 ? uiPositions.Width : DefaultWidth;
            Height = uiPositions != null && uiPositions.Height != 0 ? uiPositions.Height : DefaultHeight;
        }

        /// <summary>
        /// Saves <see cref="HudPositionInfo"/> positions for current tool
        /// </summary>
        /// <param name="positions">The list of <see cref="HudPositionInfo"/></param>
        public override void SavePositions(List<HudPositionInfo> positions)
        {
            var tool = Tool as T;

            if (tool == null || positions == null)
            {
                return;
            }

            // Set width and height
            positions.ForEach(x =>
            {
                x.Width = Width;
                x.Height = Height;
            });

            tool.UIPositions = positions;
        }

        /// <summary>
        /// Sets <see cref="HudPositionInfo"/> positions for current tool
        /// </summary>
        /// <param name="positions">The list of <see cref="HudPositionInfo"/></param>
        public override void SetPositions(List<HudPositionInfo> positions)
        {
            var tool = Tool as T;

            if (tool == null || positions == null)
            {
                return;
            }

            var uiPosition = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (uiPosition != null)
            {
                positions.ForEach(x =>
                {
                    x.Width = uiPosition.Width;
                    x.Height = uiPosition.Height;
                });
            }

            tool.UIPositions = positions;
        }
    }
}