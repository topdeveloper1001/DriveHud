//-----------------------------------------------------------------------
// <copyright file="ChartSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using Model.Data;
using Model.Enums;
using System.Collections.Generic;

namespace DriveHUD.ViewModels
{
    public class ChartSeriesProvider
    {
        public static IEnumerable<ChartSeries> CreateMoneyWonChartSeries()
        {
            var winningsChartCollection = new List<ChartSeries>();

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NetWonSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Netwon,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                Format = "{0:0.##}$",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    current.Value += stat.NetWon;
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NonShowdownSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.NonShowdown,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                Format = "{0:0.##}",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    if (stat.Sawshowdown == 0)
                    {
                        current.Value += stat.NetWon;
                    }
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_ShowdownSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Showdown,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                Format = "{0:0.##}",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    if (stat.Sawshowdown == 1)
                    {
                        current.Value += stat.NetWon;
                    }
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_EVSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.EV,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                Format = "{0:0.##}$",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    current.Value += stat.NetWon + stat.EVDiff;
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NetWonSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Netwon,
                ChartCashSeriesValueType = ChartCashSeriesValueType.BB,
                Format = "{0:0}$",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    current.Value += stat.BigBlind != 0 ? stat.NetWon / stat.BigBlind : 0;
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NonShowdownSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.NonShowdown,
                ChartCashSeriesValueType = ChartCashSeriesValueType.BB,
                Format = "{0:0.##}",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    if (stat.Sawshowdown == 0)
                    {
                        current.Value += stat.BigBlind != 0 ? stat.NetWon / stat.BigBlind : 0;
                    }
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_ShowdownSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Showdown,
                ChartCashSeriesValueType = ChartCashSeriesValueType.BB,
                Format = "{0:0.##}",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    if (stat.Sawshowdown == 1)
                    {
                        current.Value += stat.BigBlind != 0 ? stat.NetWon / stat.BigBlind : 0;
                    }
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_EVSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.EV,
                ChartCashSeriesValueType = ChartCashSeriesValueType.BB,
                Format = "{0:0}$",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    current.Value += stat.BigBlind != 0 ? (stat.NetWon + stat.EVDiff) / stat.BigBlind : 0;
                }
            });

            return winningsChartCollection;
        }

        public static IEnumerable<ChartSeries> CreateBB100ChartSeries()
        {
            var bb100ChartCollection = new List<ChartSeries>();

            var bb100Indicator = new LightIndicators();

            bb100ChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NetWonSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Netwon,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                Format = "{0:0.##}$",
                UpdateChartSeriesItem = (current, previous, stat, index) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }
                    else
                    {
                        bb100Indicator = new LightIndicators();
                    }

                    bb100Indicator.AddStatistic(stat);

                    current.Value = bb100Indicator.BB;
                }
            });

            return bb100ChartCollection;
        }

        public static IEnumerable<TournamentChartSeries> CreateTournamentChartSeries()
        {
            var tournamentChartCollection = new List<TournamentChartSeries>();

            var blueResource = ChartSerieResourceHelper.GetSeriesBluePalette();
            var yellowResource = ChartSerieResourceHelper.GetSeriesYellowPalette();
            var orangeResource = ChartSerieResourceHelper.GetSerieOrangePalette();
            var greenResource = ChartSerieResourceHelper.GetSerieGreenPalette();

            tournamentChartCollection.Add(new TournamentChartSeries()
            {
                IsVisible = true,
                Caption = "ITM%",
                Format = "{0:0.##}%",
                SeriesType = ChartTournamentSeriesType.ITM,
                ColorsPalette = blueResource,
                UpdateChartSeriesItem = (current, previous, tournament) =>
                {
                    current.Value = tournament.ITM;
                }
            });

            tournamentChartCollection.Add(new TournamentChartSeries()
            {
                IsVisible = true,
                Caption = "ROI%",
                Format = "{0:0.##}%",
                SeriesType = ChartTournamentSeriesType.ROI,
                ColorsPalette = yellowResource,
                UpdateChartSeriesItem = (current, previous, tournament) =>
                {
                    current.Value = tournament.ROI;
                }
            });

            tournamentChartCollection.Add(new TournamentChartSeries()
            {
                IsVisible = true,
                Caption = "$",
                Format = "{0:0.##}$",
                SeriesType = ChartTournamentSeriesType.MoneyWon,
                ColorsPalette = greenResource,
                UpdateChartSeriesItem = (current, previous, tournament) =>
                {
                    current.Value = tournament.NetWon;
                }
            });

            tournamentChartCollection.Add(new TournamentChartSeries()
            {
                IsVisible = true,
                Caption = "Luck",
                Format = "{0:0.##}%",
                SeriesType = ChartTournamentSeriesType.Luck,
                ColorsPalette = orangeResource,
                UpdateChartSeriesItem = (current, previous, tournament) =>
                {
                    current.Value = 0;
                }
            });

            return tournamentChartCollection;
        }
    }
}