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

                    current.Value += (stat.NetWon + stat.EVDiff) / stat.BigBlind;
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
                        current.Value += stat.NetWon / stat.BigBlind;
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
                        current.Value += stat.NetWon / stat.BigBlind;
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

                    current.Value += stat.EVDiff / stat.BigBlind;
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
    }
}