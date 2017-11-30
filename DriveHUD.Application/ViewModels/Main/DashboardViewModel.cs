using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Model;
using Model.Enums;

using DriveHUD.ViewModels;
using DriveHUD.Common.Infrastructure.Base;
using System.Collections.Generic;
using Prism.Events;
using Model.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Windows.Input;
using Model.Data;
using Model.Reports;
using System.Diagnostics;
using Model.ChartData;

namespace DriveHUD.Application.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<ChartSeries> _firstChartCollection = new ObservableCollection<ChartSeries>();
        private ObservableCollection<ChartSeries> _secondChartCollection = new ObservableCollection<ChartSeries>();
        private EnumTelerikRadChartDisplayRange _firstChartDisplayRange = EnumTelerikRadChartDisplayRange.Month;
        private EnumTelerikRadChartDisplayRange _secondChartDisplayRange = EnumTelerikRadChartDisplayRange.Month;
        private LightIndicators _indicatorCollection;
        private bool _isExpanded = true;
        #endregion

        #region Properties

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public ObservableCollection<ChartSeries> FirstChartCollection
        {
            get
            {
                return _firstChartCollection;
            }

            set
            {
                _firstChartCollection = value;
            }
        }

        public ObservableCollection<ChartSeries> SecondChartCollection
        {
            get
            {
                return _secondChartCollection;
            }

            set
            {
                _secondChartCollection = value;
            }
        }

        public EnumTelerikRadChartDisplayRange FirstChartDisplayRange
        {
            get { return _firstChartDisplayRange; }
            set
            {
                if (value != _firstChartDisplayRange)
                    SetProperty(ref _firstChartDisplayRange, value);

                SetSerieData(FirstChartCollection, ChartSerieResourceHelper.GetSerieGreenPalette(), FirstChartDisplayRange);
            }
        }

        public EnumTelerikRadChartDisplayRange SecondChartDisplayRange
        {
            get { return _secondChartDisplayRange; }
            set
            {
                if (value != _secondChartDisplayRange)
                    SetProperty(ref _secondChartDisplayRange, value);

                SetSerieData(SecondChartCollection, ChartSerieResourceHelper.GetSerieOrangePalette(), SecondChartDisplayRange);
            }
        }

        public LightIndicators IndicatorCollection
        {
            get { return _indicatorCollection; }
            set
            {
                SetProperty(ref _indicatorCollection, value);
            }
        }
        #endregion

        internal DashboardViewModel(SynchronizationContext _synchronizationContext)
        {
            synchronizationContext = _synchronizationContext;

            Init();
        }

        private void Init()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<BuiltFilterChangedEvent>().Subscribe(UpdateFilteredData);

            InitCharts();
        }

        private void SetSerieData(IEnumerable<ChartSeries> chartCollection, ChartSerieResourceHelper resource, EnumTelerikRadChartDisplayRange displayRange)
        {
            foreach (var serie in chartCollection)
            {
                List<ChartSeriesItem> itemsList = new List<ChartSeriesItem>();

                var groupedResults = GetGroupedStats(displayRange);

                foreach (var stat in groupedResults)
                {
                    switch (serie.FunctionName)
                    {
                        case EnumTelerikRadChartFunctionType.MoneyWon:
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Item1,
                                Value = stat.Item2,
                                ValueText = string.Format("{0:0.##}$", stat.Item2),
                                PointColor = resource.PointColor,
                                TrackBallColor = resource.TrackBallColor,
                                TooltipColor = resource.TooltipColor
                            });
                            break;
                        case EnumTelerikRadChartFunctionType.BB:
                            itemsList.Add(new ChartSeriesItem()
                            {
                                Date = stat.Item1,
                                Value = stat.Item3,
                                ValueText = string.Format("{0:0.##}", stat.Item3),
                                PointColor = resource.PointColor,
                                TrackBallColor = resource.TrackBallColor,
                                TooltipColor = resource.TooltipColor
                            });
                            break;
                    }
                }

                serie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(itemsList.OrderBy(x => x.Date));
            }
        }

        internal void UpdateFilteredData(BuiltFilterChangedEventArgs args)
        {
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

            SetSerieData(SecondChartCollection, ChartSerieResourceHelper.GetSerieOrangePalette(), SecondChartDisplayRange);
            SetSerieData(FirstChartCollection, ChartSerieResourceHelper.GetSerieGreenPalette(), FirstChartDisplayRange);
        }

        private void InitCharts()
        {
            ChartSerieResourceHelper resource = ChartSerieResourceHelper.GetSerieGreenPalette();
            ChartSeries series0 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "Money Won",
                FunctionName = EnumTelerikRadChartFunctionType.MoneyWon,
                Type = EnumTelerikRadChartSeriesType.Area,
                LineColor = resource.LineColor,
                AreaStyle = resource.AreaBrush
            };

            resource = ChartSerieResourceHelper.GetSerieOrangePalette();
            ChartSeries series1 = new ChartSeries()
            {
                IsVisible = true,
                Caption = "bb/100",
                FunctionName = EnumTelerikRadChartFunctionType.BB,
                Type = EnumTelerikRadChartSeriesType.Area,
                LineColor = resource.LineColor,
                AreaStyle = resource.AreaBrush
            };

            FirstChartCollection.Clear();
            FirstChartCollection.Add(series0);
            SecondChartCollection.Clear();
            SecondChartCollection.Add(series1);
        }

        private IEnumerable<Tuple<DateTime, decimal, decimal>> GetGroupedStats(EnumTelerikRadChartDisplayRange range)
        {
            switch (range)
            {
                case EnumTelerikRadChartDisplayRange.Year:
                    return new YearChartData().Create(StorageModel.StatisticCollection.ToList().Where(x => !x.IsTourney).ToList());
                case EnumTelerikRadChartDisplayRange.Month:
                    return new MonthChartData().Create(StorageModel.StatisticCollection.ToList().Where(x => !x.IsTourney).ToList());
                case EnumTelerikRadChartDisplayRange.Week:
                    return new WeekChartData().Create(StorageModel.StatisticCollection.ToList().Where(x => !x.IsTourney).ToList());
            }

            return null;
        }
    }
}