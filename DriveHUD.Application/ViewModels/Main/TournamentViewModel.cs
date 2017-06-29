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
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.ChartData;
using Model.Data;
using Model.Enums;
using Model.Events;
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
        private bool _showLabels = true;
        private bool _isShowLabelsEnabled = true;
        private ObservableCollection<ChartSeries> _chartSeriesCollection;
        private ChartSeries _chartSeriesSelectedItem;
        private EnumTelerikRadChartDisplayRange _chartSeriesDisplayRange = EnumTelerikRadChartDisplayRange.Month;
        private Bracelet _goldenBracelet = new Bracelet();
        private Bracelet _silverBracelet = new Bracelet();
        private Bracelet _bronzeBracelet = new Bracelet();

        private int _totalMTT;
        private int _totalSTT;
        private decimal _sttWon;
        private decimal _mttWon;

        private bool _isExpanded = true;
        #endregion

        #region Properties
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public bool ShowLabels
        {
            get { return this._showLabels; }
            set
            {
                if (value == _showLabels) return;
                _showLabels = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowLabelsEnabled
        {
            get { return this._isShowLabelsEnabled; }
            set
            {
                if (this._isShowLabelsEnabled != value)
                {
                    this._isShowLabelsEnabled = value;
                    this.OnPropertyChanged("IsShowLabelsEnabled");
                }
            }
        }

        public ObservableCollection<ChartSeries> ChartSeriesCollection
        {
            get { return this._chartSeriesCollection; }
            set
            {
                if (value == _chartSeriesCollection) return;
                _chartSeriesCollection = value;
                OnPropertyChanged();
            }
        }

        public ChartSeries ChartSeriesSelectedItem
        {
            get { return this._chartSeriesSelectedItem; }
            set
            {
                if (value == _chartSeriesSelectedItem) return;
                _chartSeriesSelectedItem = value;
                OnPropertyChanged();
            }
        }

        public EnumTelerikRadChartDisplayRange ChartSeriesDisplayRange
        {
            get
            {
                return _chartSeriesDisplayRange;
            }

            set
            {
                if (value != _chartSeriesDisplayRange)
                    SetProperty(ref _chartSeriesDisplayRange, value);

                SetSerieData(ChartSeriesCollection, ChartSeriesDisplayRange);
            }
        }

        public Bracelet GoldenBracelet
        {
            get
            {
                return _goldenBracelet;
            }

            set
            {
                SetProperty(ref _goldenBracelet, value);
            }
        }

        public Bracelet SilverBracelet
        {
            get
            {
                return _silverBracelet;
            }

            set
            {
                SetProperty(ref _silverBracelet, value);
            }
        }

        public Bracelet BronzeBracelet
        {
            get
            {
                return _bronzeBracelet;
            }

            set
            {
                SetProperty(ref _bronzeBracelet, value);
            }
        }

        public int TotalMTT
        {
            get
            {
                return _totalMTT;
            }

            set
            {
                SetProperty(ref _totalMTT, value);
            }
        }

        public int TotalSTT
        {
            get
            {
                return _totalSTT;
            }

            set
            {
                SetProperty(ref _totalSTT, value);
            }
        }

        public decimal MTTWon
        {
            get { return _mttWon; }
            set { SetProperty(ref _mttWon, value); }
        }

        public decimal STTWon
        {
            get { return _sttWon; }
            set { SetProperty(ref _sttWon, value); }
        }

        #endregion

        #region ICommand
        public ICommand BraceletTournamentClickCommand { get; set; }
        #endregion

        internal TournamentViewModel()
        {
            InitializeChartSeries();

            GoldenBracelet = new Bracelet() { PlaceNumber = 1 };
            GoldenBracelet.BraceletItems = new ObservableCollection<BraceletItem>();
            SilverBracelet = new Bracelet() { PlaceNumber = 2 };
            SilverBracelet.BraceletItems = new ObservableCollection<BraceletItem>();
            BronzeBracelet = new Bracelet() { PlaceNumber = 3 };
            BronzeBracelet.BraceletItems = new ObservableCollection<BraceletItem>();

            BraceletTournamentClickCommand = new RelayCommand(BraceletTournamentClick);

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<TournamentDataUpdatedEvent>().Subscribe(Update);
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
                LineColor = blueResource.LineColor,
                AreaStyle = blueResource.AreaBrush
            };

            ChartSeries series1 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "ROI%",
                FunctionName = EnumTelerikRadChartFunctionType.ROI,
                Type = EnumTelerikRadChartSeriesType.Area,
                LineColor = yellowResource.LineColor,
                AreaStyle = yellowResource.AreaBrush
            };

            ChartSeries series2 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "$",
                FunctionName = EnumTelerikRadChartFunctionType.MoneyWon,
                Type = EnumTelerikRadChartSeriesType.Area,
                LineColor = greenResource.LineColor,
                AreaStyle = greenResource.AreaBrush
            };

            ChartSeries series3 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "Luck",
                FunctionName = EnumTelerikRadChartFunctionType.Luck,
                Type = EnumTelerikRadChartSeriesType.Area,
                LineColor = orangeResource.LineColor,
                AreaStyle = orangeResource.AreaBrush
            };

            ChartSeriesCollection = new ObservableCollection<ChartSeries>();
            ChartSeriesCollection.Add(series0);
            ChartSeriesCollection.Add(series1);
            ChartSeriesCollection.Add(series2);
            ChartSeriesCollection.Add(series3);
        }

        private void Update(TournamentDataUpdatedEventArgs args)
        {
            Update();
        }

        internal void Update()
        {
            if (this.StorageModel.StatisticCollection == null)
                return;

            SetSerieData(ChartSeriesCollection, ChartSeriesDisplayRange);

            var playerTournaments = StorageModel.PlayerSelectedItem != null ?
                ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(StorageModel.PlayerSelectedItem.PlayerIds) :
                new List<Tournaments>();

            MTTWon = 0;
            STTWon = 0;
            TotalMTT = 0;
            TotalSTT = 0;

            foreach (var tournament in playerTournaments)
            {
                TournamentsTags tag;

                if (Enum.TryParse(tournament.Tourneytagscsv, out tag))
                {
                    switch (tag)
                    {
                        case TournamentsTags.MTT:
                            if (tournament.Finishposition == 1)
                            {
                                MTTWon += tournament.Winningsincents / 100m;
                            }
                            TotalMTT++;
                            break;
                        case TournamentsTags.STT:
                            if (tournament.Finishposition == 1)
                            {
                                STTWon += tournament.Winningsincents / 100m;
                            }
                            TotalSTT++;
                            break;
                    }
                }
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                SetBraceletData(GoldenBracelet, playerTournaments);
                SetBraceletData(SilverBracelet, playerTournaments);
                SetBraceletData(BronzeBracelet, playerTournaments);
            });

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

        private void SetSerieData(IEnumerable<ChartSeries> chartCollection, EnumTelerikRadChartDisplayRange displayRange)
        {
            foreach (var serie in chartCollection)
            {
                List<ChartSeriesItem> itemsList = new List<ChartSeriesItem>();
                foreach (var stat in GetGroupedStats(displayRange))
                {
                    switch (serie.FunctionName)
                    {
                        case EnumTelerikRadChartFunctionType.ITM:
                            var blueResource = ChartSerieResourceHelper.GetSeriesBluePalette();
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = stat.ITM,
                                ValueText = string.Format("{0:0.##}%", stat.ITM),
                                PointColor = blueResource.PointColor,
                                TrackBallColor = blueResource.TrackBallColor,
                                TooltipColor = blueResource.TooltipColor
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.ROI:
                            var yellowResource = ChartSerieResourceHelper.GetSeriesYellowPalette();
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = stat.ROI,
                                ValueText = string.Format("{0:0.##}%", stat.ROI),
                                PointColor = yellowResource.PointColor,
                                TrackBallColor = yellowResource.TrackBallColor,
                                TooltipColor = yellowResource.TooltipColor
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.MoneyWon:
                            var greenResource = ChartSerieResourceHelper.GetSerieGreenPalette();
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = stat.Won,
                                ValueText = string.Format("{0:0.##}$", stat.Won),
                                PointColor = greenResource.PointColor,
                                TrackBallColor = greenResource.TrackBallColor,
                                TooltipColor = greenResource.TooltipColor
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.Luck:
                            var orangeResource = ChartSerieResourceHelper.GetSerieOrangePalette();
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Started,
                                Value = 0,
                                ValueText = string.Format("{0:0.##}%", 0),
                                PointColor = orangeResource.PointColor,
                                TrackBallColor = orangeResource.TrackBallColor,
                                TooltipColor = orangeResource.TooltipColor
                            });
                            break;
                    }
                }
                serie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(itemsList.OrderBy(x => x.Date));
            }
        }

        private IEnumerable<TournamentReportRecord> GetGroupedStats(EnumTelerikRadChartDisplayRange range)
        {
            List<TournamentReportRecord> indicators = null;
            switch (range)
            {
                case EnumTelerikRadChartDisplayRange.Year:
                    indicators = new List<TournamentReportRecord>(new YearTournamentChartData().Create());
                    break;
                case EnumTelerikRadChartDisplayRange.Month:
                    indicators = new List<TournamentReportRecord>(new MonthTournamentChartData().Create());
                    break;
                case EnumTelerikRadChartDisplayRange.Week:
                    indicators = new List<TournamentReportRecord>(new WeekTournamentChartData().Create());
                    break;
            }

            return indicators;
        }

        #region ICommand Implementation
        private void BraceletTournamentClick(object obj)
        {
            if (obj != null)
            {
                if (obj is BraceletItem)
                {
                    var item = obj as BraceletItem;
                    ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RequestDisplayTournamentHands>().Publish(new RequestDisplayTournamentHandsEvent(item.Id));
                }
            }
        }
        #endregion
    }
}
