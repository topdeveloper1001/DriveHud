//-----------------------------------------------------------------------
// <copyright file="LayoutMigrator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using DriveHUD.Entities;
using System.Windows;
using DriveHUD.Common.Resources;
using Model.Stats;
using ReactiveUI;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using DriveHUD.Common.Exceptions;

namespace DriveHUD.Application.MigrationService.Migrators
{
    internal class LayoutMigrator : ILayoutMigrator
    {
        private const double uiPositionShiftX = 30;
        private const double ui4StatBoxHorizontalShiftY = 89;
        private const double ui4StatBoxVerticalShiftX = 114;

        private const double uiPlainBoxVertical1RightOrientedShiftX = -70;
        private const double uiPlainBoxVertical1ShiftX = 112;
        private const double uiPlainBoxVerticalShiftY = -8;
        private const double uiPlainBoxVerticalWidth = 101;
        private const double uiPlainBoxShiftY = 26;

        private const double uiTiltMeterShiftY = 62;
        private const double uiTiltMeterRightOrientedShiftX = 132;

        private const double uiPlayerIconShiftY = 62;
        private const double uiPlayerIconRightOrientedShiftX = 104;
        private const double uiPlayerIconShiftX = 13;
        private const double uiBumperStickerShiftY = -10;

        private const double uiExtraShiftY = -97;

        private readonly IEnumerable<StatInfo> availableStats;

        public LayoutMigrator()
        {
            availableStats = StatInfoHelper.GetAllStats();
        }

        public HudLayoutInfoV2 Migrate(HudLayoutInfo layout)
        {
            Check.Require(layout != null, "Layout must be not null");

            var hudLayoutInfoV2 = new HudLayoutInfoV2
            {
                Name = layout.Name,
                IsDefault = layout.IsDefault,
                TableType = layout.TableType,
                Opacity = (double)layout.HudOpacity / 100,
                HudPlayerTypes = layout.HudPlayerTypes.Select(x => x.Clone()).ToList(),
                HudBumperStickerTypes = layout.HudBumperStickerTypes.Select(x => x.Clone()).ToList()
            };

            if (layout.HudViewType == HudViewType.Plain)
            {
                MigratePlainLayout(layout, hudLayoutInfoV2);
            }
            else
            {
                MigrateRichLayout(layout, hudLayoutInfoV2);
            }

            return hudLayoutInfoV2;
        }

        public HudLayoutInfoV2 Migrate(HudSavedLayout layout)
        {
            throw new NotImplementedException();
        }

        private void MigratePlainLayout(HudLayoutInfo layout, HudLayoutInfoV2 layoutInfoV2)
        {
            var hudLayoutPlainBoxTool = new HudLayoutPlainBoxTool();

            hudLayoutPlainBoxTool.UIPositions = (from seat in Enumerable.Range(1, (int)layout.TableType)
                                                 let uiPosition = layout.UiPositionsInfo.FirstOrDefault(x => x.Seat == seat)
                                                 let x = uiPosition != null ? uiPosition.Position.X + uiPositionShiftX : 0
                                                 let y = uiPosition != null ? uiPosition.Position.Y : 0
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = seat,
                                                     Position = new Point(x, y),
                                                     Width = uiPosition != null ? uiPosition.Width : HudDefaultSettings.PlainStatBoxWidth,
                                                     Height = HudDefaultSettings.PlainStatBoxHeight,
                                                 }).ToList();

            hudLayoutPlainBoxTool.Stats = new ReactiveList<StatInfo>(layout.HudStats.Select(x => x.Clone()));

            hudLayoutPlainBoxTool.Positions = (from hudPositionsInfo in layout.HudPositionsInfo
                                               select new HudPositionsInfo
                                               {
                                                   GameType = hudPositionsInfo.GameType,
                                                   PokerSite = hudPositionsInfo.PokerSite,
                                                   HudPositions = (from hudPosition in hudPositionsInfo.HudPositions
                                                                   let offset = GetOffset(hudPositionsInfo.PokerSite, hudPositionsInfo.GameType, layout.TableType, hudPosition.Seat)
                                                                   select new HudPositionInfo
                                                                   {
                                                                       Seat = hudPosition.Seat,
                                                                       Position = new Point(hudPosition.Position.X + offset.X, hudPosition.Position.Y + offset.Y)
                                                                   }).ToList()
                                               }).ToList();

            var hudLayoutBumperStickersTool = new HudLayoutBumperStickersTool
            {
                UIPositions = (from position in hudLayoutPlainBoxTool.UIPositions
                               select new HudPositionInfo
                               {
                                   Seat = position.Seat,
                                   Position = new Point(position.Position.X, position.Position.Y - HudDefaultSettings.BumperStickersHeight),
                                   Width = HudDefaultSettings.BumperStickersWidth,
                                   Height = HudDefaultSettings.BumperStickersHeight
                               }).ToList(),
                Positions = (from positionsInfo in hudLayoutPlainBoxTool.Positions
                             select new HudPositionsInfo
                             {
                                 PokerSite = positionsInfo.PokerSite,
                                 GameType = positionsInfo.GameType,
                                 HudPositions = (from position in positionsInfo.HudPositions
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = position.Seat,
                                                     Position = new Point(position.Position.X, position.Position.Y - HudDefaultSettings.BumperStickersHeight)
                                                 }).ToList()
                             }).ToList()
            };


            layoutInfoV2.LayoutTools = new List<HudLayoutTool> { hudLayoutPlainBoxTool, hudLayoutBumperStickersTool };

            MigratePopups(layout, layoutInfoV2);
        }

        private void MigrateRichLayout(HudLayoutInfo layout, HudLayoutInfoV2 layoutInfoV2)
        {
            // 4-stat box
            var hudLayout4StatBoxTool = new HudLayoutFourStatsBoxTool
            {
                IsVertical = layout.HudViewType != HudViewType.Horizontal,
                UIPositions = (from seat in Enumerable.Range(1, (int)layout.TableType)
                               let uiPosition = layout.UiPositionsInfo.FirstOrDefault(x => x.Seat == seat)
                               let shiftX = layout.HudViewType != HudViewType.Horizontal && IsRightOriented((int)layout.TableType, seat) ? ui4StatBoxVerticalShiftX : 0
                               let shiftY = layout.HudViewType == HudViewType.Horizontal ? ui4StatBoxHorizontalShiftY : 0
                               let x = uiPosition != null ? uiPosition.Position.X + shiftX : 0
                               let y = uiPosition != null ? uiPosition.Position.Y + shiftY : 0
                               let width = layout.HudViewType != HudViewType.Horizontal ? HudDefaultSettings.FourStatVerticalBoxWidth : HudDefaultSettings.FourStatBoxWidth
                               let height = layout.HudViewType != HudViewType.Horizontal ? HudDefaultSettings.FourStatVerticalBoxHeight : HudDefaultSettings.FourStatBoxHeight
                               select new HudPositionInfo
                               {
                                   Seat = seat,
                                   Position = new Point(x, y + uiExtraShiftY),
                                   Width = width,
                                   Height = height,
                               }).ToList(),
                Stats = new ReactiveList<StatInfo>(layout.HudStats
                    .Where(x => x.Stat != Stat.PlayerInfoIcon && x.Stat != Stat.LineBreak)
                    .Take(4)
                    .Select(x => x.Clone())),
                Positions = (from hudPositionsInfo in layout.HudPositionsInfo
                             select new HudPositionsInfo
                             {
                                 GameType = hudPositionsInfo.GameType,
                                 PokerSite = hudPositionsInfo.PokerSite,
                                 HudPositions = (from hudPosition in hudPositionsInfo.HudPositions
                                                 let shiftX = layout.HudViewType != HudViewType.Horizontal && IsRightOriented((int)layout.TableType, hudPosition.Seat) ? ui4StatBoxVerticalShiftX : 0
                                                 let shiftY = layout.HudViewType == HudViewType.Horizontal ? ui4StatBoxHorizontalShiftY : 0
                                                 let offset = GetOffset(hudPositionsInfo.PokerSite, hudPositionsInfo.GameType, layout.TableType, hudPosition.Seat)
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = hudPosition.Seat,
                                                     Position = new Point(hudPosition.Position.X + offset.X + shiftX, hudPosition.Position.Y + offset.Y + shiftY)
                                                 }).ToList()
                             }).ToList()
            };

            // plain box
            var hudLayoutPlainBoxTool = new HudLayoutPlainBoxTool
            {
                UIPositions = (from seat in Enumerable.Range(1, (int)layout.TableType)
                               let uiPosition = layout.UiPositionsInfo.FirstOrDefault(x => x.Seat == seat)
                               let shiftX = layout.HudViewType == HudViewType.Vertical_1 ?
                                                (IsRightOriented((int)layout.TableType, seat) ? uiPlainBoxVertical1RightOrientedShiftX : uiPlainBoxVertical1ShiftX) : 0
                               let shiftY = layout.HudViewType == HudViewType.Vertical_1 ? uiPlainBoxVerticalShiftY : uiPlainBoxShiftY + ui4StatBoxHorizontalShiftY
                               let x = uiPosition != null ? uiPosition.Position.X + shiftX : 0
                               let y = uiPosition != null ? uiPosition.Position.Y + shiftY : 0
                               let width = layout.HudViewType == HudViewType.Vertical_1 ? uiPlainBoxVerticalWidth : HudDefaultSettings.FourStatBoxWidth
                               let height = HudDefaultSettings.PlainStatBoxHeight
                               select new HudPositionInfo
                               {
                                   Seat = seat,
                                   Position = new Point(x, y + uiExtraShiftY),
                                   Width = width,
                                   Height = height
                               }).ToList(),

                Stats = new ReactiveList<StatInfo>(layout.HudStats
                    .Where(x => !hudLayout4StatBoxTool.Stats.Any(p => p.Stat == x.Stat))
                    .Select(x => x.Clone())),

                Positions = (from hudPositionsInfo in layout.HudPositionsInfo
                             select new HudPositionsInfo
                             {
                                 GameType = hudPositionsInfo.GameType,
                                 PokerSite = hudPositionsInfo.PokerSite,
                                 HudPositions = (from hudPosition in hudPositionsInfo.HudPositions
                                                 let shiftX = layout.HudViewType == HudViewType.Vertical_1 ?
                                                        (IsRightOriented((int)layout.TableType, hudPosition.Seat) ? uiPlainBoxVertical1RightOrientedShiftX : uiPlainBoxVertical1ShiftX) : 0
                                                 let shiftY = layout.HudViewType == HudViewType.Vertical_1 ? uiPlainBoxVerticalShiftY : uiPlainBoxShiftY + ui4StatBoxHorizontalShiftY
                                                 let offset = GetOffset(hudPositionsInfo.PokerSite, hudPositionsInfo.GameType, layout.TableType, hudPosition.Seat)
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = hudPosition.Seat,
                                                     Position = new Point(hudPosition.Position.X + offset.X + shiftX, hudPosition.Position.Y + offset.Y + shiftY)
                                                 }).ToList()
                             }).ToList()
            };

            // tilt meter
            var hudLayoutTiltMeterTool = new HudLayoutTiltMeterTool
            {
                UIPositions = (from seat in Enumerable.Range(1, (int)layout.TableType)
                               let uiPosition = layout.UiPositionsInfo.FirstOrDefault(x => x.Seat == seat)
                               let shiftX = IsRightOriented((int)layout.TableType, seat) ? uiTiltMeterRightOrientedShiftX : 0
                               let shiftY = uiTiltMeterShiftY
                               let x = uiPosition != null ? uiPosition.Position.X + shiftX : 0
                               let y = uiPosition != null ? uiPosition.Position.Y + shiftY : 0
                               let width = HudDefaultSettings.TiltMeterWidth
                               let height = HudDefaultSettings.TiltMeterHeight
                               select new HudPositionInfo
                               {
                                   Seat = seat,
                                   Position = new Point(x, y + uiExtraShiftY),
                                   Width = width,
                                   Height = height
                               }).ToList(),

                Positions = (from hudPositionsInfo in layout.HudPositionsInfo
                             select new HudPositionsInfo
                             {
                                 GameType = hudPositionsInfo.GameType,
                                 PokerSite = hudPositionsInfo.PokerSite,
                                 HudPositions = (from hudPosition in hudPositionsInfo.HudPositions
                                                 let shiftX = IsRightOriented((int)layout.TableType, hudPosition.Seat) ? uiTiltMeterRightOrientedShiftX : 0
                                                 let shiftY = uiTiltMeterShiftY
                                                 let offset = GetOffset(hudPositionsInfo.PokerSite, hudPositionsInfo.GameType, layout.TableType, hudPosition.Seat)
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = hudPosition.Seat,
                                                     Position = new Point(hudPosition.Position.X + offset.X + shiftX, hudPosition.Position.Y + offset.Y + shiftY)
                                                 }).ToList()
                             }).ToList()
            };

            // player icon
            var hudLayoutPlayerIconTool = new HudLayoutPlayerIconTool
            {
                UIPositions = (from seat in Enumerable.Range(1, (int)layout.TableType)
                               let uiPosition = layout.UiPositionsInfo.FirstOrDefault(x => x.Seat == seat)
                               let shiftX = IsRightOriented((int)layout.TableType, seat) ? uiPlayerIconRightOrientedShiftX : uiPlayerIconShiftX
                               let shiftY = uiPlayerIconShiftY
                               let x = uiPosition != null ? uiPosition.Position.X + shiftX : 0
                               let y = uiPosition != null ? uiPosition.Position.Y + shiftY : 0
                               let width = HudDefaultSettings.PlayerIconWidth
                               let height = HudDefaultSettings.PlayerIconHeight
                               select new HudPositionInfo
                               {
                                   Seat = seat,
                                   Position = new Point(x, y + uiExtraShiftY),
                                   Width = width,
                                   Height = height
                               }).ToList(),

                Positions = (from hudPositionsInfo in layout.HudPositionsInfo
                             select new HudPositionsInfo
                             {
                                 GameType = hudPositionsInfo.GameType,
                                 PokerSite = hudPositionsInfo.PokerSite,
                                 HudPositions = (from hudPosition in hudPositionsInfo.HudPositions
                                                 let shiftX = IsRightOriented((int)layout.TableType, hudPosition.Seat) ? uiPlayerIconRightOrientedShiftX : uiPlayerIconShiftX
                                                 let shiftY = uiPlayerIconShiftY
                                                 let offset = GetOffset(hudPositionsInfo.PokerSite, hudPositionsInfo.GameType, layout.TableType, hudPosition.Seat)
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = hudPosition.Seat,
                                                     Position = new Point(hudPosition.Position.X + offset.X + shiftX, hudPosition.Position.Y + offset.Y + shiftY)
                                                 }).ToList()
                             }).ToList()
            };

            var hudLayoutGraphTool = new HudLayoutGraphTool
            {
                ParentId = hudLayoutPlayerIconTool.Id,
                Stats = new ReactiveList<StatInfo> { CreateStatInfo(Stat.NetWon) }
            };

            // bumper stickers
            var hudLayoutBumperStickersTool = new HudLayoutBumperStickersTool
            {
                UIPositions = (from seat in Enumerable.Range(1, (int)layout.TableType)
                               let uiPosition = layout.UiPositionsInfo.FirstOrDefault(x => x.Seat == seat)
                               let shiftX = layout.HudViewType == HudViewType.Vertical_1 ?
                                                (IsRightOriented((int)layout.TableType, seat) ? uiPlainBoxVertical1RightOrientedShiftX : uiPlainBoxVertical1ShiftX) : 0
                               let shiftY = layout.HudViewType == HudViewType.Vertical_1 ? uiPlainBoxVerticalShiftY : uiPlainBoxShiftY + ui4StatBoxHorizontalShiftY
                               let x = uiPosition != null ? uiPosition.Position.X + shiftX : 0
                               let y = (uiPosition != null ? uiPosition.Position.Y + shiftY : 0) + uiBumperStickerShiftY
                               let width = HudDefaultSettings.BumperStickersWidth
                               let height = HudDefaultSettings.BumperStickersHeight
                               select new HudPositionInfo
                               {
                                   Seat = seat,
                                   Position = new Point(x, y + uiExtraShiftY),
                                   Width = width,
                                   Height = height
                               }).ToList(),

                Positions = (from hudPositionsInfo in layout.HudPositionsInfo
                             select new HudPositionsInfo
                             {
                                 GameType = hudPositionsInfo.GameType,
                                 PokerSite = hudPositionsInfo.PokerSite,
                                 HudPositions = (from hudPosition in hudPositionsInfo.HudPositions
                                                 let shiftX = layout.HudViewType == HudViewType.Vertical_1 ?
                                                        (IsRightOriented((int)layout.TableType, hudPosition.Seat) ? uiPlainBoxVertical1RightOrientedShiftX : uiPlainBoxVertical1ShiftX) : 0
                                                 let shiftY = layout.HudViewType == HudViewType.Vertical_1 ? uiPlainBoxVerticalShiftY : uiPlainBoxShiftY + ui4StatBoxHorizontalShiftY
                                                 let offset = GetOffset(hudPositionsInfo.PokerSite, hudPositionsInfo.GameType, layout.TableType, hudPosition.Seat)
                                                 select new HudPositionInfo
                                                 {
                                                     Seat = hudPosition.Seat,
                                                     Position = new Point(hudPosition.Position.X + offset.X + shiftX, hudPosition.Position.Y + offset.Y + shiftY + uiBumperStickerShiftY)
                                                 }).ToList()
                             }).ToList()
            };

            layoutInfoV2.LayoutTools = new List<HudLayoutTool> { hudLayout4StatBoxTool, hudLayoutPlainBoxTool, hudLayoutTiltMeterTool, hudLayoutPlayerIconTool, hudLayoutGraphTool, hudLayoutBumperStickersTool };

            MigratePopups(layout, layoutInfoV2);
        }

        private void MigratePopups(HudLayoutInfo layout, HudLayoutInfoV2 layoutInfoV2)
        {
            if (layout.HudStats.Any(x => x.Stat == Stat.VPIP))
            {
                var hudLayoutVPIPGaugeIndicatorTool = new HudLayoutGaugeIndicator
                {
                    BaseStat = CreateStatInfo(Stat.VPIP),
                    Text = "TOTAL",
                    HeaderText = "VPIP",
                    Stats = new ReactiveList<StatInfo>
                    {
                        CreateStatInfo(Stat.VPIP_EP),
                        CreateStatInfo(Stat.VPIP_MP),
                        CreateStatInfo(Stat.VPIP_CO),
                        CreateStatInfo(Stat.VPIP_BN),
                        CreateStatInfo(Stat.VPIP_SB),
                        CreateStatInfo(Stat.VPIP_BB),
                    }
                };

                var hudLayoutColdCallGaugeIndicatorTool = new HudLayoutGaugeIndicator
                {
                    BaseStat = CreateStatInfo(Stat.VPIP),
                    Text = "COLD CALL",
                    HeaderText = "Cold Call",
                    Stats = new ReactiveList<StatInfo>
                    {
                        CreateStatInfo(Stat.ColdCall_EP),
                        CreateStatInfo(Stat.ColdCall_MP),
                        CreateStatInfo(Stat.ColdCall_CO),
                        CreateStatInfo(Stat.ColdCall_BN),
                        CreateStatInfo(Stat.ColdCall_SB),
                        CreateStatInfo(Stat.ColdCall_BB),
                    }
                };

                layoutInfoV2.LayoutTools.Add(hudLayoutVPIPGaugeIndicatorTool);
                layoutInfoV2.LayoutTools.Add(hudLayoutColdCallGaugeIndicatorTool);
            }

            if (layout.HudStats.Any(x => x.Stat == Stat.PFR))
            {
                var hudLayoutPFRGaugeIndicatorTool = new HudLayoutGaugeIndicator
                {
                    BaseStat = CreateStatInfo(Stat.PFR),
                    Text = "UNOPENED",
                    HeaderText = "PFR",
                    Stats = new ReactiveList<StatInfo>
                    {
                        CreateStatInfo(Stat.UO_PFR_EP),
                        CreateStatInfo(Stat.UO_PFR_MP),
                        CreateStatInfo(Stat.UO_PFR_CO),
                        CreateStatInfo(Stat.UO_PFR_BN),
                        CreateStatInfo(Stat.UO_PFR_SB),
                        CreateStatInfo(Stat.UO_PFR_BB),
                    }
                };

                layoutInfoV2.LayoutTools.Add(hudLayoutPFRGaugeIndicatorTool);
            }

            if (layout.HudStats.Any(x => x.Stat == Stat.S3Bet))
            {
                var hudLayout3BetGaugeIndicatorTool = new HudLayoutGaugeIndicator
                {
                    BaseStat = CreateStatInfo(Stat.S3Bet),
                    Text = "TOTAL",
                    HeaderText = "3-bet%",
                    Stats = new ReactiveList<StatInfo>
                    {
                        CreateStatInfo(Stat.ThreeBet_EP),
                        CreateStatInfo(Stat.ThreeBet_MP),
                        CreateStatInfo(Stat.ThreeBet_CO),
                        CreateStatInfo(Stat.ThreeBet_BN),
                        CreateStatInfo(Stat.ThreeBet_SB),
                        CreateStatInfo(Stat.ThreeBet_BB),
                    }
                };

                layoutInfoV2.LayoutTools.Add(hudLayout3BetGaugeIndicatorTool);
            }

            if (layout.HudStats.Any(x => x.Stat == Stat.AGG))
            {
                var hudLayoutAggGaugeIndicatorTool = new HudLayoutGaugeIndicator
                {
                    BaseStat = CreateStatInfo(Stat.AGG),
                    Text = "TOTAL",
                    HeaderText = "AGG%",
                    Stats = new ReactiveList<StatInfo>
                    {
                        CreateStatInfo(Stat.FlopAGG),
                        CreateStatInfo(Stat.TurnAGG),
                        CreateStatInfo(Stat.RiverAGG),
                        CreateStatInfo(Stat.RecentAgg)
                    }
                };

                layoutInfoV2.LayoutTools.Add(hudLayoutAggGaugeIndicatorTool);
            }

            if (layout.HudStats.Any(x => x.Stat == Stat.PlayerInfoIcon))
            {
                var hudLayoutGraphTool = new HudLayoutGraphTool
                {
                    BaseStat = CreateStatInfo(Stat.PlayerInfoIcon),
                    Stats = new ReactiveList<StatInfo> { CreateStatInfo(Stat.NetWon) }
                };

                layoutInfoV2.LayoutTools.Add(hudLayoutGraphTool);
            }
        }

        private StatInfo CreateStatInfo(Stat stat)
        {
            var statInfo = availableStats.FirstOrDefault(x => x.Stat == stat);

            if (statInfo == null)
            {
                throw new DHBusinessException(new NonLocalizableString($"Stat {stat} has not been found."));
            }

            return statInfo;
        }

        private Point GetOffset(EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType, int seat)
        {
            if (pokerSite == EnumPokerSites.Ignition || pokerSite == EnumPokerSites.Bovada || pokerSite == EnumPokerSites.Bodog)
            {
                return new Point(0, -27);
            }

            var hudPanelService = ServiceLocator.Current.GetInstance<IHudPanelService>(pokerSite.ToString());

            var shifts = hudPanelService.GetPositionShift(tableType, seat);
            return shifts;
        }

        private static bool IsRightOriented(int seats, int seat)
        {
            seat = seat - 1;
            return (seats > 6 && seat < 5) || (seats < 7 && seats > 2 && seat < 3) || (seats < 3 && seat < 1);
        }
    }
}