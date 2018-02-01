//-----------------------------------------------------------------------
// <copyright file="TournamentViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.ChartData;
using Model.Data;
using Model.Enums;
using Model.Events;
using Model.Filters;
using Model.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class TournamentViewModel : BaseViewModel
    {
        #region Fields

        private bool showLabels = true;
        private bool isShowLabelsEnabled = true;
        private ObservableCollection<ChartSeries> chartSeriesCollection;
        private ChartSeries chartSeriesSelectedItem;
        private ChartDisplayRange chartSeriesDisplayRange = ChartDisplayRange.Month;
        private Bracelet goldenBracelet;
        private Bracelet silverBracelet;
        private Bracelet bronzeBracelet;

        private int totalMTT;
        private int totalSTT;
        private decimal sttWon;
        private decimal mttWon;

        private bool isExpanded = true;

        private readonly IEventAggregator eventAggregator;
        private readonly IFilterModelManagerService filterModelManagerService;

        #endregion

        internal TournamentViewModel()
        {
            InitializeChartSeries();

            GoldenBracelet = new Bracelet { PlaceNumber = 1 };
            SilverBracelet = new Bracelet { PlaceNumber = 2 };
            BronzeBracelet = new Bracelet { PlaceNumber = 3 };

            BraceletTournamentClickCommand = new RelayCommand(BraceletTournamentClick);

            filterModelManagerService = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<TournamentDataUpdatedEvent>().Subscribe(x => Update());
            eventAggregator.GetEvent<BuiltFilterChangedEvent>().Subscribe(x => Update());
        }

        #region Properties

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

        public bool ShowLabels
        {
            get
            {
                return showLabels;
            }
            set
            {
                SetProperty(ref showLabels, value);
            }
        }

        public bool IsShowLabelsEnabled
        {
            get
            {
                return isShowLabelsEnabled;
            }
            set
            {
                SetProperty(ref isShowLabelsEnabled, value);
            }
        }

        public ObservableCollection<ChartSeries> ChartSeriesCollection
        {
            get
            {
                return chartSeriesCollection;
            }
            set
            {
                SetProperty(ref chartSeriesCollection, value);
            }
        }

        public ChartSeries ChartSeriesSelectedItem
        {
            get
            {
                return chartSeriesSelectedItem;
            }
            set
            {
                SetProperty(ref chartSeriesSelectedItem, value);
            }
        }

        public ChartDisplayRange ChartSeriesDisplayRange
        {
            get
            {
                return chartSeriesDisplayRange;
            }
            set
            {
                SetProperty(ref chartSeriesDisplayRange, value);
                SetSerieData(ChartSeriesCollection, ChartSeriesDisplayRange);
            }
        }

        public Bracelet GoldenBracelet
        {
            get
            {
                return goldenBracelet;
            }
            private set
            {
                SetProperty(ref goldenBracelet, value);
            }
        }

        public Bracelet SilverBracelet
        {
            get
            {
                return silverBracelet;
            }
            private set
            {
                SetProperty(ref silverBracelet, value);
            }
        }

        public Bracelet BronzeBracelet
        {
            get
            {
                return bronzeBracelet;
            }
            private set
            {
                SetProperty(ref bronzeBracelet, value);
            }
        }

        public int TotalMTT
        {
            get
            {
                return totalMTT;
            }
            private set
            {
                SetProperty(ref totalMTT, value);
            }
        }

        public int TotalSTT
        {
            get
            {
                return totalSTT;
            }
            private set
            {
                SetProperty(ref totalSTT, value);
            }
        }

        public decimal MTTWon
        {
            get
            {
                return mttWon;
            }
            private set
            {
                SetProperty(ref mttWon, value);
            }
        }

        public decimal STTWon
        {
            get
            {
                return sttWon;
            }
            private set
            {
                SetProperty(ref sttWon, value);
            }
        }

        #endregion

        #region Commands

        public ICommand BraceletTournamentClickCommand { get; private set; }

        #endregion

        internal void Update()
        {
            if (StorageModel.StatisticCollection == null)
            {
                return;
            }

            SetSerieData(ChartSeriesCollection, ChartSeriesDisplayRange);

            var playerTournaments = StorageModel.PlayerSelectedItem != null ?
                ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(StorageModel.PlayerSelectedItem.PlayerIds) :
                new List<Tournaments>();

            MTTWon = 0;
            STTWon = 0;
            TotalMTT = 0;
            TotalSTT = 0;

            var dateFilter = filterModelManagerService.FilterModelCollection?.OfType<FilterDateModel>().FirstOrDefault();

            var filteredTournaments = dateFilter != null ? dateFilter.FilterTournaments(playerTournaments) : playerTournaments;

            foreach (var tournament in filteredTournaments)
            {
                TournamentsTags tag;

                if (Enum.TryParse(tournament.Tourneytagscsv, out tag))
                {
                    switch (tag)
                    {
                        case TournamentsTags.MTT:
                            MTTWon += (tournament.Winningsincents - tournament.Buyinincents - tournament.Rakeincents) / 100m;
                            TotalMTT++;
                            break;
                        case TournamentsTags.STT:
                            STTWon += (tournament.Winningsincents - tournament.Buyinincents - tournament.Rakeincents) / 100m;
                            TotalSTT++;
                            break;
                    }
                }
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                SetBraceletData(GoldenBracelet, filteredTournaments);
                SetBraceletData(SilverBracelet, filteredTournaments);
                SetBraceletData(BronzeBracelet, filteredTournaments);
            });
        }

        private void InitializeChartSeries()
        {
            var blueResource = ChartSerieResourceHelper.GetSeriesBluePalette();
            var yellowResource = ChartSerieResourceHelper.GetSeriesYellowPalette();
            var orangeResource = ChartSerieResourceHelper.GetSerieOrangePalette();
            var greenResource = ChartSerieResourceHelper.GetSerieGreenPalette();

            ObservableCollection<ChartSeries> chartSeriesCollection = new ObservableCollection<ChartSeries>();

            ChartSeries series0 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "ITM%",
                FunctionName = EnumTelerikRadChartFunctionType.ITM,
                Type = EnumTelerikRadChartSeriesType.Area,
                ColorsPalette = blueResource
            };

            ChartSeries series1 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "ROI%",
                FunctionName = EnumTelerikRadChartFunctionType.ROI,
                Type = EnumTelerikRadChartSeriesType.Area,
                ColorsPalette = yellowResource
            };

            ChartSeries series2 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "$",
                FunctionName = EnumTelerikRadChartFunctionType.MoneyWon,
                Type = EnumTelerikRadChartSeriesType.Area,
                ColorsPalette = greenResource,
            };

            ChartSeries series3 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "Luck",
                FunctionName = EnumTelerikRadChartFunctionType.Luck,
                Type = EnumTelerikRadChartSeriesType.Area,
                ColorsPalette = orangeResource
            };

            ChartSeriesCollection = new ObservableCollection<ChartSeries>();
            ChartSeriesCollection.Add(series0);
            ChartSeriesCollection.Add(series1);
            ChartSeriesCollection.Add(series2);
            ChartSeriesCollection.Add(series3);
        }

        private void SetBraceletData(Bracelet bracelet, IEnumerable<Tournaments> playerTournaments)
        {
            bracelet.NumberOfWins = playerTournaments.Where(x => x.Finishposition == bracelet.PlaceNumber).Count();
            bracelet.BraceletItems.Clear();
            foreach (var item in playerTournaments.Where(x => x.Finishposition == bracelet.PlaceNumber))
            {
                HandHistories.Objects.GameDescription.Limit limit =
                    HandHistories.Objects.GameDescription.Limit.FromSmallBlindBigBlind(0, 0, HandHistories.Objects.GameDescription.Currency.USD);//(HandHistories.Objects.GameDescription.Currency)item.CurrencyId);
                bracelet.BraceletItems.Add(new BraceletItem()
                {
                    Id = item.Tourneynumber,
                    AmountString = String.Format("{0} {1:0.##}", limit.GetCurrencySymbol(), item.Winningsincents / 100m)
                });
            }
        }

        private void SetSerieData(IEnumerable<ChartSeries> chartCollection, ChartDisplayRange displayRange)
        {
            var groupedStats = GetGroupedStats(displayRange);

            foreach (var serie in chartCollection)
            {
                List<ChartSeriesItem> itemsList = new List<ChartSeriesItem>();

                foreach (var stat in groupedStats)
                {
                    switch (serie.FunctionName)
                    {
                        case EnumTelerikRadChartFunctionType.ITM:

                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = stat.ITM,
                                Format = "{0:0.##}%",
                                PointColor = serie.ColorsPalette.PointColor,
                                TrackBallColor = serie.ColorsPalette.TrackBallColor,
                                TooltipColor = serie.ColorsPalette.TooltipColor,
                                TooltipForegroundColor = serie.ColorsPalette.TooltipForeground
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.ROI:
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = stat.ROI,
                                Format = "{0:0.##}%",
                                PointColor = serie.ColorsPalette.PointColor,
                                TrackBallColor = serie.ColorsPalette.TrackBallColor,
                                TooltipColor = serie.ColorsPalette.TooltipColor,
                                TooltipForegroundColor = serie.ColorsPalette.TooltipForeground
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.MoneyWon:
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = stat.NetWon,
                                Format = "{0:0.##}$",
                                PointColor = serie.ColorsPalette.PointColor,
                                TrackBallColor = serie.ColorsPalette.TrackBallColor,
                                TooltipColor = serie.ColorsPalette.TooltipColor,
                                TooltipForegroundColor = serie.ColorsPalette.TooltipForeground
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.Luck:
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = 0,
                                Format = "{0:0.##}%",
                                PointColor = serie.ColorsPalette.PointColor,
                                TrackBallColor = serie.ColorsPalette.TrackBallColor,
                                TooltipColor = serie.ColorsPalette.TooltipColor,
                                TooltipForegroundColor = serie.ColorsPalette.TooltipForeground
                            });
                            break;
                    }
                }

                serie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(itemsList.OrderBy(x => x.Date));
            }
        }

        private IEnumerable<TournamentReportRecord> GetGroupedStats(ChartDisplayRange range)
        {
            List<TournamentReportRecord> indicators = null;

            switch (range)
            {
                case ChartDisplayRange.Year:
                    indicators = new List<TournamentReportRecord>(new YearTournamentChartData().Create());
                    break;
                case ChartDisplayRange.Month:
                    indicators = new List<TournamentReportRecord>(new MonthTournamentChartData().Create());
                    break;
                case ChartDisplayRange.Week:
                    indicators = new List<TournamentReportRecord>(new WeekTournamentChartData().Create());
                    break;
            }

            return indicators;
        }

        #region ICommand Implementation

        private void BraceletTournamentClick(object obj)
        {
            var item = obj as BraceletItem;

            if (item == null)
            {
                return;
            }

            eventAggregator.GetEvent<RequestDisplayTournamentHands>().Publish(new RequestDisplayTournamentHandsEvent(item.Id));
        }

        #endregion
    }
}