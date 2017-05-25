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

using DriveHUD.Application.TableConfigurators.PositionProviders;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Stats;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                case HudDesignerToolType.FourStatBox:
                    return CreateFourStatBoxTool(creationInfo);
                case HudDesignerToolType.GaugeIndicator:
                    return CreateGaugeIndicatorTool(creationInfo);
                case HudDesignerToolType.TiltMeter:
                    return CreateTiltMeterTool(creationInfo);
                case HudDesignerToolType.PlayerProfileIcon:
                    return CreatePlayerIconTool(creationInfo);
                case HudDesignerToolType.Graph:
                    return CreateGraphTool(creationInfo);
                case HudDesignerToolType.TextBox:
                    return CreateTextBoxTool(creationInfo);
                case HudDesignerToolType.BumperStickers:
                    return CreateBumperStickersTool(creationInfo);
                default:
                    throw new NotSupportedException($"{creationInfo.ToolType} isn't supported");
            }
        }

        /// <summary>
        /// Gets the list of <see cref="HudPositionInfo"/> related to base position in designer 
        /// </summary>
        /// <param name="tableType"><see cref="EnumTableType"/> of table</param>
        /// <param name="relativeTableType"><see cref="EnumTableType"/> to which position is set</param>
        /// <param name="position"><see cref="Point"/> position of base element</param>
        /// <returns>The list of <see cref="HudPositionInfo"/></returns>
        public List<HudPositionInfo> GetHudUIPositions(EnumTableType tableType, EnumTableType relativeTableType, Point position)
        {
            var positions = new List<HudPositionInfo>();

            var seats = (int)tableType;
            var relativeTableSeats = (int)relativeTableType;

            var playerLabelPositions = HudDefaultSettings.TablePlayerLabelPositions[seats];
            var relativePlayerLabelPositions = HudDefaultSettings.TablePlayerLabelPositions[relativeTableSeats];

            var relativePlayerLabelPositionX = relativePlayerLabelPositions[0, 0];
            var relativePlayerLabelPositionY = relativePlayerLabelPositions[0, 1];

            var deltaX = position.X - relativePlayerLabelPositionX;
            var deltaY = position.Y - relativePlayerLabelPositionY;

            for (var seat = 0; seat < seats; seat++)
            {
                var playerLabelPositionX = playerLabelPositions[seat, 0];
                var playerLabelPositionY = playerLabelPositions[seat, 1];

                var positionInfo = new HudPositionInfo
                {
                    Position = new Point(playerLabelPositionX + deltaX, playerLabelPositionY + deltaY),
                    Seat = seat + 1
                };

                positions.Add(positionInfo);
            }

            return positions;
        }

        /// <summary>
        /// Creates plain stat box view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Plain stat box view model</returns>
        private HudBaseToolViewModel CreatePlainStatBoxTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. Plain stat box has not been created.");

            var layoutTool = new HudLayoutPlainBoxTool
            {
                Stats = new ReactiveList<StatInfo>(),
                UIPositions = GetHudUIPositions(EnumTableType.HU, EnumTableType.HU, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.PlainStatBoxWidth;
                x.Height = HudDefaultSettings.PlainStatBoxHeight;
            });

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.InitializePositions();

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates 4-stat box view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>4-stat box view model</returns>
        private HudBaseToolViewModel CreateFourStatBoxTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. 4-stat box has not been created.");

            var layoutTool = new HudLayoutFourStatsBoxTool
            {
                Stats = new ReactiveList<StatInfo>(),
                UIPositions = GetHudUIPositions(EnumTableType.HU, EnumTableType.HU, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.FourStatBoxWidth;
                x.Height = HudDefaultSettings.FourStatBoxHeight;
            });

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.InitializePositions();

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates tilt meter view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Tilt meter view model</returns>
        private HudBaseToolViewModel CreateTiltMeterTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. 4-stat box has not been created.");

            var layoutTool = new HudLayoutTiltMeterTool
            {
                UIPositions = GetHudUIPositions(EnumTableType.HU, EnumTableType.HU, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.TiltMeterWidth;
                x.Height = HudDefaultSettings.TiltMeterHeight;
            });

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.InitializePositions();

            if (toolViewModel.Parent != null)
            {
                toolViewModel.Parent.TiltMeter = HudDefaultSettings.TiltMeterDesignerValue;
            }

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates player icon view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Player icon view model</returns>
        private HudBaseToolViewModel CreatePlayerIconTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. 4-stat box has not been created.");

            var layoutTool = new HudLayoutPlayerIconTool
            {
                UIPositions = GetHudUIPositions(EnumTableType.HU, EnumTableType.HU, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.PlayerIconWidth;
                x.Height = HudDefaultSettings.PlayerIconHeight;
            });

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.InitializePositions();

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates text box view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Text box view model</returns>
        private HudBaseToolViewModel CreateTextBoxTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. 4-stat box has not been created.");

            var layoutTool = new HudLayoutTextBoxTool
            {
                UIPositions = GetHudUIPositions(EnumTableType.HU, EnumTableType.HU, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.TextBoxWidth;
                x.Height = HudDefaultSettings.TextBoxHeight;
            });

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.InitializePositions();

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates bumper stickers view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Tilt meter view model</returns>
        private HudBaseToolViewModel CreateBumperStickersTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. Bumper stickers have not been created.");

            var layoutTool = new HudLayoutBumperStickersTool
            {
                UIPositions = GetHudUIPositions(EnumTableType.HU, EnumTableType.HU, creationInfo.Position)
            };

            layoutTool.UIPositions.ForEach(x =>
            {
                x.Width = HudDefaultSettings.BumperStickersWidth;
                x.Height = HudDefaultSettings.BumperStickersHeight;
            });

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.InitializePositions();

            if (toolViewModel.Parent != null)
            {
                toolViewModel.Parent.Stickers = new ObservableCollection<HudBumperStickerType>(creationInfo.Layout.HudBumperStickerTypes.Select(x => x.Clone()));
            }

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates gauge indicator view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Gauge indicator view model</returns>
        private HudBaseToolViewModel CreateGaugeIndicatorTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. Gauge indicator has not been created.");

            var statInfo = creationInfo.Source as StatInfo;

            if (statInfo == null)
            {
                return null;
            }

            statInfo.IsSelected = false;

            var layoutTool = new HudLayoutGaugeIndicator
            {
                BaseStat = statInfo.Clone(),
                Text = statInfo.ToolTip
            };

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.Position = creationInfo.Position;

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Creates graph view model
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/></param>
        /// <returns>Graph view model</returns>
        private HudBaseToolViewModel CreateGraphTool(HudToolCreationInfo creationInfo)
        {
            Check.Require(creationInfo.Layout != null, "Layout isn't defined. Graph has not been created.");

            var statInfo = creationInfo.Source as StatInfo;

            HudLayoutGraphTool layoutTool = null;

            if (statInfo != null)
            {
                statInfo.IsSelected = false;

                var initialStat = statInfo.Stat == Stat.PlayerInfoIcon ?
                    StatInfoHelper.GetStat(Stat.NetWon) :
                    statInfo.Clone();

                layoutTool = new HudLayoutGraphTool
                {
                    BaseStat = statInfo.Clone(),
                    Stats = new ReactiveList<StatInfo> { initialStat }
                };
            }
            else
            {
                var parentTool = creationInfo.Source as HudBaseToolViewModel;

                if (parentTool == null)
                {
                    return null;
                }

                layoutTool = new HudLayoutGraphTool
                {
                    ParentId = parentTool.Tool.Id
                };
            }

            var toolViewModel = layoutTool.CreateViewModel(creationInfo.HudElement);
            toolViewModel.Position = creationInfo.Position;

            creationInfo.Layout.LayoutTools.Add(layoutTool);

            return toolViewModel;
        }

        /// <summary>
        /// Gets positions of tool on HUD for all possible <see cref="EnumPokerSites"/> and <see cref="EnumGameType" />
        /// </summary>
        /// <returns></returns>
        private List<HudPositionsInfo> GetHudPositions(EnumTableType tableType, Point position)
        {
            // existing elements positions will be in default xml files

            // we have predefined position of player labels
            // so we need to calculate offset between UI player label and UI position, then apply those offsets to hud player label

            var hudPositions = new List<HudPositionsInfo>();

            var pokerSites = EntityUtils.GetSupportedPokerSites();

            foreach (var pokerSite in pokerSites)
            {
                foreach (var gameType in Enum.GetValues(typeof(EnumGameType)).OfType<EnumGameType>())
                {
                    var hudPositionsInfo = new HudPositionsInfo
                    {
                        PokerSite = pokerSite,
                        GameType = gameType,
                        HudPositions = GetHudPositionInfo(pokerSite, tableType, position)
                    };

                    hudPositions.Add(hudPositionsInfo);
                }
            }

            return hudPositions;
        }

        /// <summary>
        /// Gets the list of <see cref="HudPositionInfo"/> of element for the specified <see cref="EnumPokerSites"/>, <see cref="EnumTableType"/>
        /// </summary>
        /// <param name="pokerSite"></param>
        /// <param name="tableType"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private List<HudPositionInfo> GetHudPositionInfo(EnumPokerSites pokerSite, EnumTableType tableType, Point position)
        {
            var positionsInfo = new List<HudPositionInfo>();

            // in designer only 2-max and 1st seat are available       
            var labelPositions = HudDefaultSettings.TablePlayerLabelPositions[(int)EnumTableType.HU];

            var labelPositionX = labelPositions[0, 0];
            var labelPositionY = labelPositions[0, 1];

            var offsetX = position.X - labelPositionX;
            var offsetY = position.Y - labelPositionY;

            var positionProvider = ServiceLocator.Current.GetInstance<IPositionProvider>(pokerSite.ToString());

            var seats = (int)tableType;

            var tableTypeSize = (int)tableType;

            if (!positionProvider.Positions.ContainsKey(tableTypeSize))
            {
                return positionsInfo;
            }

            var positions = positionProvider.Positions[tableTypeSize];

            for (var seat = 0; seat < seats; seat++)
            {
                var positionX = positions[seat, 0];
                var positionY = positions[seat, 1];

                var positionInfo = new HudPositionInfo
                {
                    Seat = seat + 1,
                    Position = new Point(positionX + offsetX, positionY + offsetY)
                };

                positionsInfo.Add(positionInfo);
            }

            return positionsInfo;
        }
    }
}