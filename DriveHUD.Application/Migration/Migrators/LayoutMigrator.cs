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

namespace DriveHUD.Application.MigrationService.Migrators
{
    internal class LayoutMigrator : ILayoutMigrator
    {
        private const double uiPositionShiftX = 30;

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
                                                     Height = uiPosition != null ? uiPosition.Height : HudDefaultSettings.PlainStatBoxHeight,
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

            var hudLayoutVPIPGaugeIndicatorTool = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.VPIP },
                Text = "TOTAL",
                HeaderText = "VPIP",
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.VPIP_EP },
                    new StatInfo { Stat = Stat.VPIP_MP },
                    new StatInfo { Stat = Stat.VPIP_CO },
                    new StatInfo { Stat = Stat.VPIP_BN },
                    new StatInfo { Stat = Stat.VPIP_SB },
                    new StatInfo { Stat = Stat.VPIP_BB },
                }
            };

            var hudLayoutColdCallGaugeIndicatorTool = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.VPIP },
                Text = "COLD CALL",
                HeaderText = "Cold Call",
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.ColdCall_EP },
                    new StatInfo { Stat = Stat.ColdCall_MP },
                    new StatInfo { Stat = Stat.ColdCall_CO },
                    new StatInfo { Stat = Stat.ColdCall_BN },
                    new StatInfo { Stat = Stat.ColdCall_SB },
                    new StatInfo { Stat = Stat.ColdCall_BB },
                }
            };

            var hudLayoutPFRGaugeIndicatorTool = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.PFR },
                Text = "UNOPENED",
                HeaderText = "PFR",
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.UO_PFR_EP },
                    new StatInfo { Stat = Stat.UO_PFR_MP },
                    new StatInfo { Stat = Stat.UO_PFR_CO },
                    new StatInfo { Stat = Stat.UO_PFR_BN },
                    new StatInfo { Stat = Stat.UO_PFR_SB },
                    new StatInfo { Stat = Stat.UO_PFR_BB },
                }
            };

            var hudLayout3BetGaugeIndicatorTool = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.S3Bet },
                Text = "TOTAL",
                HeaderText = "3-bet%",
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.ThreeBet_EP },
                    new StatInfo { Stat = Stat.ThreeBet_MP },
                    new StatInfo { Stat = Stat.ThreeBet_CO },
                    new StatInfo { Stat = Stat.ThreeBet_BN },
                    new StatInfo { Stat = Stat.ThreeBet_SB },
                    new StatInfo { Stat = Stat.ThreeBet_BB },
                }
            };

            var hudLayoutAggGaugeIndicatorTool = new HudLayoutGaugeIndicator
            {
                BaseStat = new StatInfo { Stat = Stat.AGG },
                Text = "TOTAL",
                HeaderText = "AGG%",
                Stats = new ReactiveList<StatInfo>
                {
                    new StatInfo { Stat = Stat.FlopAGG },
                    new StatInfo { Stat = Stat.TurnAGG },
                    new StatInfo { Stat = Stat.RiverAGG },
                    new StatInfo { Stat = Stat.RecentAgg }
                }
            };

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

            layoutInfoV2.LayoutTools = new List<HudLayoutTool> { hudLayoutPlainBoxTool, hudLayoutVPIPGaugeIndicatorTool,
                hudLayoutColdCallGaugeIndicatorTool, hudLayoutPFRGaugeIndicatorTool, hudLayout3BetGaugeIndicatorTool, hudLayoutAggGaugeIndicatorTool, hudLayoutBumperStickersTool };

            if (layout.HudStats.Any(x => x.Stat == Stat.PlayerInfoIcon))
            {
                var hudLayoutGraphTool = new HudLayoutGraphTool
                {
                    BaseStat = new StatInfo { Stat = Stat.PlayerInfoIcon },
                    Stats = new ReactiveList<StatInfo> { new StatInfo { Stat = Stat.NetWon } }
                };

                layoutInfoV2.LayoutTools.Add(hudLayoutGraphTool);
            }
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
    }
}