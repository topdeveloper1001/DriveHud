//-----------------------------------------------------------------------
// <copyright file="DashboardViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Main;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Resources;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Enums;
using Model.Events;
using Prism.Events;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Properties

        private bool isExpanded = true;

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                SetProperty(ref isExpanded, value);
            }
        }

        private CashGraphViewModel winningsGraphViewModel;

        public CashGraphViewModel WinningsGraphViewModel
        {
            get
            {
                return winningsGraphViewModel;
            }
            private set
            {
                SetProperty(ref winningsGraphViewModel, value);
            }
        }

        private CashGraphViewModel bb100GraphViewModel;

        public CashGraphViewModel BB100GraphViewModel
        {
            get
            {
                return bb100GraphViewModel;
            }
            private set
            {
                SetProperty(ref bb100GraphViewModel, value);
            }
        }

        private LightIndicators indicatorCollection;

        public LightIndicators IndicatorCollection
        {
            get
            {
                return indicatorCollection;
            }
            set
            {
                SetProperty(ref indicatorCollection, value);
            }
        }

        #endregion

        internal DashboardViewModel()
        {
            ServiceLocator.Current
                .GetInstance<IEventAggregator>()
                .GetEvent<BuiltFilterChangedEvent>()
                .Subscribe(UpdateFilteredData);

            InitializeWinningsChart();
            InitializeBB100Chart();
        }

        internal void UpdateFilteredData(BuiltFilterChangedEventArgs args)
        {
            if (!IsActive)
            {
                return;
            }

            if (IndicatorCollection == null)
            {
                IndicatorCollection = new LightIndicators();
            }
            else
            {
                IndicatorCollection.Clean();
            }

            if (StorageModel.FilteredPlayerStatistic == null)
            {
                return;
            }

            var statistics = StorageModel.FilteredPlayerStatistic.Where(x => !x.IsTourney).ToList();

            IndicatorCollection.UpdateSource(statistics);
            OnPropertyChanged(() => IndicatorCollection);
        }

        internal void Update()
        {
            if (StorageModel.StatisticCollection == null)
            {
                return;
            }

            UpdateFilteredData(null);

            WinningsGraphViewModel?.Update();
            BB100GraphViewModel?.Update();
        }

        private void InitializeWinningsChart()
        {
            var winningsChartCollection = new List<ChartSeries>();

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NetWonSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Netwon,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                ColorsPalette = ChartSerieResourceHelper.GetSerieGreenPalette(),
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
                ColorsPalette = ChartSerieResourceHelper.GetSerieRedPalette(),
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
                ColorsPalette = ChartSerieResourceHelper.GetSeriesYellowPalette(),
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
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NetWonSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Netwon,
                ChartCashSeriesValueType = ChartCashSeriesValueType.BB,
                ColorsPalette = ChartSerieResourceHelper.GetSerieGreenPalette(),
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

                    current.Value += stat.NetWon / stat.BigBlind;
                }
            });

            winningsChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NonShowdownSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.NonShowdown,
                ChartCashSeriesValueType = ChartCashSeriesValueType.BB,
                ColorsPalette = ChartSerieResourceHelper.GetSerieRedPalette(),
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
                ColorsPalette = ChartSerieResourceHelper.GetSeriesYellowPalette(),
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

            WinningsGraphViewModel = new CashGraphViewModel(winningsChartCollection);
        }

        private void InitializeBB100Chart()
        {
            var bb100ChartCollection = new List<ChartSeries>();

            var bb100Indicator = new LightIndicators();

            bb100ChartCollection.Add(new ChartSeries
            {
                Caption = CommonResourceManager.Instance.GetResourceString("Common_Chart_NetWonSeries"),
                ChartCashSeriesWinningType = ChartCashSeriesWinningType.Netwon,
                ChartCashSeriesValueType = ChartCashSeriesValueType.Currency,
                ColorsPalette = ChartSerieResourceHelper.GetSerieOrangePalette(),
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

            BB100GraphViewModel = new CashGraphViewModel(bb100ChartCollection, ChartDisplayRange.Month);
        }
    }
}