//-----------------------------------------------------------------------
// <copyright file="HudToolFactory.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Defines factory to create new hud tool
    /// </summary>
    internal class HudToolFactory : IHudToolFactory
    {
        /// <summary>
        /// Creates <see cref="HudBaseToolViewModel"/> based on the specified <see cref="HudToolCreationInfo"/>
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/> to create new hud tool</param>
        /// <returns>Created <see cref="HudBaseToolViewModel"/></returns>
        public HudBaseToolViewModel CreateTool(HudToolCreationInfo creationInfo)
        {
            Check.ArgumentNotNull(() => creationInfo);

            switch (creationInfo.ToolType)
            {
                case HudDesignerToolType.PlainStatBox:
                    return CreatePlainStatBoxTool(creationInfo);
                default:
                    throw new NotSupportedException($"{creationInfo.ToolType} isn't supported");
            }
        }

        private HudBaseToolViewModel CreatePlainStatBoxTool(HudToolCreationInfo creationInfo)
        {
            var layoutTool = new HudLayoutPlainBoxTool
            {
                Stats = new ReactiveList<StatInfo>(),
                Positions = GetHudPositions(),
                UIPositions = GetHudUIPositions(creationInfo.TableType, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.PlainStatBoxWidth;
                x.Height = HudDefaultSettings.PlainStatBoxHeight;
            });

            return layoutTool.CreateViewModel(creationInfo.HudElement);
        }

        private List<HudPositionsInfo> GetHudPositions()
        {
            return new List<HudPositionsInfo>();
        }

        private List<HudPositionInfo> GetHudUIPositions(EnumTableType tableType, Point position)
        {
            var positions = new List<HudPositionInfo>();

            var seats = (int)tableType;

            var playerLabelPositions = HudDefaultSettings.TablePlayerLabelPositions[seats];

            var playerLabelPositionX = playerLabelPositions[0, 0];
            var playerLabelPositionY = playerLabelPositions[0, 1];

            var deltaX = position.X - playerLabelPositionX;
            var deltaY = position.Y - playerLabelPositionY;

            for (var seat = 0; seat < seats; seat++)
            {
                playerLabelPositionX = playerLabelPositions[seat, 0];
                playerLabelPositionY = playerLabelPositions[seat, 1];

                var positionInfo = new HudPositionInfo
                {
                    Position = new Point(playerLabelPositionX + deltaX, playerLabelPositionY + deltaY),
                };

                positions.Add(positionInfo);
            }

            return positions;
        }
    }
}