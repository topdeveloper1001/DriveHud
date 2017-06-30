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

using DriveHUD.Application.TableConfigurators.PositionProviders;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Base class for non-popup tools
    /// </summary>
    [ProtoContract]
    public abstract class HudBaseNonPopupToolViewModel<T> : HudBaseToolViewModel, IHudNonPopupToolViewModel
        where T : HudLayoutNonPopupTool
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
        /// Gets whenever default size must be used 
        /// </summary>
        public virtual bool UseDefaultSizesOnly
        {
            get
            {
                return false;
            }
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
        public override void InitializePositions(EnumPokerSites pokerSite, EnumTableType tableType, EnumGameType gameType)
        {
            var tool = Tool as T;

            if (tool == null)
            {
                return;
            }

            var seats = (int)tableType;
            var currentSeat = Parent.Seat - 1;

            var uiPosition = tool.UIPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (uiPosition == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Could not find UI positions for {pokerSite}, {gameType}, {Parent.Seat}"));
            }

            var positionInfo = tool.Positions.FirstOrDefault(x => x.PokerSite == pokerSite && x.GameType == gameType);

            var hudPositionInfo = positionInfo?.HudPositions.FirstOrDefault(x => x.Seat == Parent.Seat);

            if (hudPositionInfo == null)
            {
                var positionProvider = ServiceLocator.Current.GetInstance<IPositionProvider>(pokerSite.ToString());

                if (!positionProvider.Positions.ContainsKey(seats))
                {
                    throw new DHBusinessException(new NonLocalizableString($"Could not find predefined positions for {pokerSite}, {gameType}, {Parent.Seat}"));
                }

                var playerLabelClientPosition = positionProvider.Positions[seats];

                var playerLabelPosition = HudDefaultSettings.TablePlayerLabelPositions[seats];

                var offsetX = playerLabelClientPosition[currentSeat, 0] - playerLabelPosition[currentSeat, 0];
                var offsetY = playerLabelClientPosition[currentSeat, 1] - playerLabelPosition[currentSeat, 1];

                // do not change position if element is inside or above player label
                if (uiPosition.Position.Y > playerLabelPosition[currentSeat, 1] + HudDefaultSettings.TablePlayerLabelActualHeight)
                {
                    offsetY += positionProvider.PlayerLabelHeight - HudDefaultSettings.TablePlayerLabelHeight;
                }

                hudPositionInfo = new HudPositionInfo
                {
                    Seat = Parent.Seat,
                    Position = new Point(uiPosition.Position.X + offsetX, uiPosition.Position.Y + offsetY)
                };
            }

            Position = hudPositionInfo.Position;
            Opacity = Parent.Opacity;

            Width = uiPosition != null && uiPosition.Width != 0 && !UseDefaultSizesOnly ? uiPosition.Width : DefaultWidth;
            Height = uiPosition != null && uiPosition.Height != 0 && !UseDefaultSizesOnly ? uiPosition.Height : DefaultHeight;
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