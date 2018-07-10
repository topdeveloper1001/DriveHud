//-----------------------------------------------------------------------
// <copyright file="HudViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.AttachedBehaviors;
using DriveHUD.Entities;
using HandHistories.Objects.Cards;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.Data;
using Model.Enums;
using Model.Filters;
using Model.Settings;
using Model.Stats;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Represents view model of hud screen
    /// </summary>
    public class HudViewModel : PopupViewModelBase, IMainTabViewModel
    {
        private IHudLayoutsService HudLayoutsService => ServiceLocator.Current.GetInstance<IHudLayoutsService>();

        private ISettingsService SettingsService => ServiceLocator.Current.GetInstance<ISettingsService>();

        private CompositeDisposable designerDisposables = new CompositeDisposable();

        private bool skipOnStatInfoObserveCollectionChanged = false;

        /// <summary>
        /// Initializes a <see cref="HudViewModel"/> instance
        /// </summary>
        public HudViewModel()
        {
            NotificationRequest = new InteractionRequest<PopupBaseNotification>();
            OpenHudUploadToStoreRequest = new InteractionRequest<INotification>();

            InitializeStatInfoCollection();
            InitializeStatInfoCollectionObserved();
            InitializeTableTypes();
            InitializeCommands();
            InitializeObservables();

            CurrentTableType = TableTypes.FirstOrDefault();
        }

        #region Properties

        public EnumViewModelType ViewModelType => EnumViewModelType.HudViewModel;

        public InteractionRequest<PopupBaseNotification> NotificationRequest { get; private set; }

        public InteractionRequest<INotification> OpenHudUploadToStoreRequest { get; private set; }

        private bool currentLayoutIsSwitching;

        private HudLayoutInfoV2 cachedCurrentLayout;

        private EnumTableType currentTableType;

        /// <summary>
        /// Gets or sets the current <see cref="EnumTableType"/> table type 
        /// </summary>
        public EnumTableType CurrentTableType
        {
            get
            {
                return currentTableType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref currentTableType, value);

                if (!currentLayoutIsSwitching)
                {
                    Layouts = new ObservableCollection<HudLayoutInfoV2>(HudLayoutsService.GetAllLayouts(CurrentTableType));
                    CurrentLayout = Layouts.FirstOrDefault();
                }
            }
        }

        private ObservableCollection<HudLayoutInfoV2> layouts;

        /// <summary>
        /// Gets the collection of the <see cref="HudLayoutInfoV2"/> layouts
        /// </summary>
        public ObservableCollection<HudLayoutInfoV2> Layouts
        {
            get
            {
                return layouts;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref layouts, value);
            }
        }

        private HudLayoutInfoV2 currentLayout;

        /// <summary>
        /// Gets or sets the current <see cref="HudLayoutInfoV2"/> layout
        /// </summary>
        public HudLayoutInfoV2 CurrentLayout
        {
            get
            {
                return currentLayout;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref currentLayout, value);

                currentLayoutIsSwitching = true;

                if (currentLayout != null)
                {
                    CurrentTableType = TableTypes.FirstOrDefault(tableType => tableType == currentLayout.TableType);
                }

                currentLayoutIsSwitching = false;

                RefreshData();
            }
        }

        private ObservableCollection<EnumTableType> tableTypes;

        /// <summary>
        /// Gets the collection of <see cref="EnumTableType"/> table types
        /// </summary>
        public ObservableCollection<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref tableTypes, value);
            }
        }

        private string statFilter;

        public string StatFilter
        {
            get
            {
                return statFilter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref statFilter, value);
                ApplyFilterToCollectionView();
            }
        }

        private HashSet<Stat> statInfoCollectionDuplicates = new HashSet<Stat>();

        private List<StatInfo> statInfoCollectionNotDuplicates = new List<StatInfo>();

        private void ApplyFilterToCollectionView()
        {
            statInfoCollectionDuplicates.Clear();
            statInfoCollectionNotDuplicates.Clear();
            StatInfoCollectionView.SortDescriptions.Clear();
            StatInfoCollectionView.GroupDescriptions.Clear();

            if (string.IsNullOrEmpty(statFilter))
            {
                StatInfoCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("StatInfoGroup.Name"));
                StatInfoCollectionView.SortDescriptions.Add(new SortDescription("GroupName", ListSortDirection.Ascending));
            }

            StatInfoCollectionView.SortDescriptions.Add(new SortDescription("Caption", ListSortDirection.Ascending));
            StatInfoCollectionView.Refresh();
        }

        private ReactiveList<StatInfo> statInfoCollection;

        /// <summary>
        /// Gets the collection of the selected <see cref="StatInfo"/> stats
        /// </summary>
        public ReactiveList<StatInfo> StatInfoCollection
        {
            get
            {
                return statInfoCollection;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref statInfoCollection, value);

                var collectionViewSource = (CollectionView)CollectionViewSource.GetDefaultView(statInfoCollection);

                collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription("StatInfoGroup.Name"));

                collectionViewSource.SortDescriptions.Add(new SortDescription("GroupName", ListSortDirection.Ascending));
                collectionViewSource.SortDescriptions.Add(new SortDescription("Caption", ListSortDirection.Ascending));

                collectionViewSource.Filter = (item) =>
                {
                    var stat = item as StatInfo;

                    if (stat == null)
                    {
                        return false;
                    }

                    var filterCondition = true;

                    if (!string.IsNullOrEmpty(statFilter))
                    {
                        filterCondition = stat.Caption.IndexOf(statFilter, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (!statInfoCollectionDuplicates.Contains(stat.Stat))
                        {
                            statInfoCollectionDuplicates.Add(stat.Stat);
                            statInfoCollectionNotDuplicates.Add(stat);
                        }
                        else
                        {
                            filterCondition = filterCondition && statInfoCollectionNotDuplicates.Any(x => ReferenceEquals(x, stat));
                        }
                    }

                    return stat.IsListed && !stat.IsNotVisible && filterCondition;
                };

                var statFiltering = collectionViewSource as ICollectionViewLiveShaping;

                if (statFiltering.CanChangeLiveFiltering)
                {
                    statFiltering.LiveFilteringProperties.Add(nameof(StatInfo.IsNotVisible));
                    statFiltering.IsLiveFiltering = true;
                }

                statFiltering.IsLiveSorting = true;

                StatInfoCollectionView = collectionViewSource;
            }
        }

        private CollectionView statInfoCollectionView;

        /// <summary>
        /// Gets or sets the <see cref="CollectionView"/> of stats
        /// </summary>
        public CollectionView StatInfoCollectionView
        {
            get
            {
                return statInfoCollectionView;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref statInfoCollectionView, value);
            }
        }

        private ObservableCollection<StatInfo> statInfoObserveCollection;

        public ObservableCollection<StatInfo> StatInfoObserveCollection
        {
            get
            {
                return statInfoObserveCollection;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref statInfoObserveCollection, value);
            }
        }

        private StatInfo statInfoObserveSelectedItem;

        public StatInfo StatInfoObserveSelectedItem
        {
            get
            {
                return statInfoObserveSelectedItem;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref statInfoObserveSelectedItem, value);
            }
        }

        private HudElementViewModel previewHudElementViewModel;

        public HudElementViewModel PreviewHudElementViewModel
        {
            get
            {
                return previewHudElementViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref previewHudElementViewModel, value);
            }
        }

        private ReactiveList<HudElementViewModel> hudElements;

        public ReactiveList<HudElementViewModel> HudElements
        {
            get
            {
                return hudElements;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref hudElements, value);
            }
        }

        private bool isInDesignMode;

        public bool IsInDesignMode
        {
            get
            {
                return isInDesignMode;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isInDesignMode, value);
                UpdateStatsCollections();
            }
        }

        private HudElementViewModel designerHudElementViewModel;

        public HudElementViewModel DesignerHudElementViewModel
        {
            get
            {
                return designerHudElementViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref designerHudElementViewModel, value);
            }
        }

        private HudBaseToolViewModel selectedToolViewModel;

        public HudBaseToolViewModel SelectedToolViewModel
        {
            get
            {
                return selectedToolViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedToolViewModel, value);
                this.RaisePropertyChanged(nameof(CanAddStats));
                this.RaisePropertyChanged(nameof(LineBarOrSimpleStatSelectorVisible));
            }
        }

        public bool CanAddStats
        {
            get
            {
                return !IsInDesignMode || (IsInDesignMode && SelectedToolViewModel != null && SelectedToolViewModel is IHudStatsToolViewModel);
            }
        }

        public bool LineBarOrSimpleStatSelectorVisible
        {
            get
            {
                return SelectedToolViewModel != null && SelectedToolViewModel is HudGaugeIndicatorViewModel;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand DataSaveCommand { get; private set; }

        public ReactiveCommand DataDeleteCommand { get; private set; }

        public ReactiveCommand DataImportCommand { get; private set; }

        public ReactiveCommand DataExportCommand { get; private set; }

        public ReactiveCommand SpliterAddCommand { get; private set; }

        public ReactiveCommand SettingsStatInfoCommand { get; private set; }

        public ReactiveCommand PlayerTypeStatsCommand { get; private set; }

        public ReactiveCommand BumperStickersCommand { get; private set; }

        public ReactiveCommand DesignerToolCommand { get; private set; }

        public ReactiveCommand SaveDesignCommand { get; private set; }

        public ReactiveCommand CancelDesignCommand { get; private set; }

        public ICommand AddToolCommand { get; private set; }

        public ReactiveCommand RemoveToolCommand { get; private set; }

        public ReactiveCommand StatClickCommand { get; private set; }

        public ReactiveCommand ToolClickCommand { get; private set; }

        public ReactiveCommand DuplicateCommand { get; private set; }

        public ReactiveCommand OpenHudUploadToStoreCommand { get; private set; }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Initializes <see cref="HudElements"/> using <see cref="CurrentLayout"/>
        /// </summary>
        private void InitializeHudElements()
        {
            if (CurrentLayout == null)
            {
                return;
            }

            // add extension to HudDesignerToolType to select only visible elements (pop ups are hidden)
            var layoutTools = CurrentLayout.LayoutTools.ToArray();

            var seats = (int)CurrentLayout.TableType;

            var hudElementsToAdd = new List<HudElementViewModel>();

            var random = new Random();

            for (var seat = 1; seat <= seats; seat++)
            {
                var hudElement = new HudElementViewModel(layoutTools);
                hudElement.TiltMeter = HudDefaultSettings.TiltMeterDesignerValue;
                hudElement.Seat = seat;
                hudElement.PlayerName = string.Format(HudDefaultSettings.TablePlayerNameFormat, seat);
                hudElement.Opacity = CurrentLayout.Opacity;
                hudElement.Stickers = new ObservableCollection<HudBumperStickerType>(CurrentLayout
                    .HudBumperStickerTypes
                    .Select(x => x.Clone()));

                try
                {
                    hudElement.Tools.ForEach(x =>
                    {
                        x.InitializePositions();

                        if (x is IHudStatsToolViewModel && x is IHudBaseStatToolViewModel)
                        {
                            (x as IHudStatsToolViewModel).Stats.ForEach(s =>
                            {
                                s.CurrentValue = random.Next(0, 100);

                                if (StatsProvider.StatsBases.ContainsKey(s.Stat) && StatsProvider.StatsBases[s.Stat].CreateStatDto != null)
                                {
                                    s.StatDto = new StatDto(0, 0);
                                }
                            });
                        }
                    });
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not set positions on HUD view for {CurrentLayout.Name}", e);
                }

                hudElementsToAdd.Add(hudElement);
            }

            HudElements = new ReactiveList<HudElementViewModel>(hudElementsToAdd);

            InitializePreview();
        }

        /// <summary>
        /// Initializes <see cref="TableTypes"/>
        /// </summary>
        private void InitializeTableTypes()
        {
            TableTypes = new ObservableCollection<EnumTableType>(Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>());
        }

        /// <summary>
        /// Initializes <see cref="StatInfoCollection"/>
        /// </summary>
        private void InitializeStatInfoCollection()
        {
            // Create a list of StatInfo
            StatInfoCollection = new ReactiveList<StatInfo>(StatsProvider.GetAllStats());

            // Initialize stat info
            StatInfoCollection.ForEach(x => x.Initialize());
        }

        /// <summary>
        /// Initializes the collection of <see cref="StatInfo"/>
        /// </summary>
        private void InitializeStatInfoCollectionObserved()
        {
            StatInfoObserveCollection = new ObservableCollection<StatInfo>();
        }

        /// <summary>
        /// Initializes commands
        /// </summary>
        protected override void InitializeCommands()
        {
            base.InitializeCommands();

            var canUseDataCommands = this.WhenAny(x => x.IsInDesignMode, x => !x.Value);

            DataSaveCommand = ReactiveCommand.Create(() => OpenDataSave(), canUseDataCommands);
            DataDeleteCommand = ReactiveCommand.Create(() => DataDelete(), canUseDataCommands);
            DataImportCommand = ReactiveCommand.Create(() => DataImport(), canUseDataCommands);
            DataExportCommand = ReactiveCommand.Create(() => DataExport(), canUseDataCommands);
            DuplicateCommand = ReactiveCommand.Create(() => OpenDuplicate(), canUseDataCommands);
            SpliterAddCommand = ReactiveCommand.Create(() => SpliterAdd());
            SettingsStatInfoCommand = ReactiveCommand.Create<StatInfo>(x => OpenStatsSettings(x));
            PlayerTypeStatsCommand = ReactiveCommand.Create<StatInfo>(x => OpenPlayerTypeStats(x));
            BumperStickersCommand = ReactiveCommand.Create<StatInfo>(x => OpenBumperStickers(x));
            DesignerToolCommand = ReactiveCommand.Create(() => InitializeDesigner());
            CancelDesignCommand = ReactiveCommand.Create(() => CloseDesigner());

            OpenHudUploadToStoreCommand = ReactiveCommand.Create(() =>
            {
                var viewModelInfo = new HudUploadToStoreViewModelInfo();
                var requestInfo = new HudUploadToStoreRequestInfo(viewModelInfo);

                OpenHudUploadToStoreRequest.Raise(requestInfo);
            });

            AddToolCommand = new RelayCommand(x =>
            {
                var dragDropDataObject = x as DragDropDataObject;

                if (dragDropDataObject == null)
                {
                    return;
                }

                if (!IsInDesignMode)
                {
                    InitializeDesigner();
                }

                var toolType = dragDropDataObject.DropData as HudDesignerToolType?;

                if (toolType.HasValue && CanAddTool(toolType.Value))
                {
                    AddTool(toolType.Value, dragDropDataObject.Position, dragDropDataObject.Source);
                }
            }, x =>
            {
                var dragDropDataObject = x as DragDropDataObject;

                if (dragDropDataObject == null)
                {
                    return false;
                }

                var toolType = dragDropDataObject.DropData as HudDesignerToolType?;

                if (!toolType.HasValue)
                {
                    return false;
                }

                // drop on main table
                if (dragDropDataObject.Source is HudViewModel && toolType.Value.IsCommonTool())
                {
                    return true;
                }

                // Gauge indicator can be dropped only on stat info
                if (dragDropDataObject.Source is StatInfo && toolType == HudDesignerToolType.GaugeIndicator)
                {
                    return true;
                }

                // Graph can be dropped only on stat info or on plain box, 4-stat or player icon
                if ((dragDropDataObject.Source is StatInfo || dragDropDataObject.Source is HudPlainStatBoxViewModel ||
                    dragDropDataObject.Source is HudPlayerIconViewModel) && toolType == HudDesignerToolType.Graph)
                {
                    return true;
                }

                // Heat map can be dropped only on supported StatInfo
                if (dragDropDataObject.Source is StatInfo && toolType == HudDesignerToolType.HeatMap)
                {
                    var statInfo = dragDropDataObject.Source as StatInfo;

                    var heatMapStats = StatsProvider.GetHeatMapStats();

                    return heatMapStats.Any(s => s.Stat == statInfo.Stat);
                }

                return false;
            });

            RemoveToolCommand = ReactiveCommand.Create<HudBaseToolViewModel>(x => RemoveTool(x));

            var canUserStatClickCommand = this.WhenAny(x => x.IsInDesignMode, x => x.Value && DesignerHudElementViewModel != null);

            StatClickCommand = ReactiveCommand.Create<StatInfo>(statInfo =>
            {
                if (statInfo == null)
                {
                    return;
                }

                var toolsToShow = DesignerHudElementViewModel.Tools
                    .OfType<IHudBaseStatToolViewModel>()
                    .Where(s => s.BaseStat != null && s.BaseStat.Stat == statInfo.Stat)
                    .OfType<HudBaseToolViewModel>()
                    .ToArray();

                ShowTools(toolsToShow);
            }, canUserStatClickCommand);

            ToolClickCommand = ReactiveCommand.Create<HudBaseToolViewModel>(toolViewModel =>
            {
                if (toolViewModel == null)
                {
                    return;
                }

                var toolsToShow = DesignerHudElementViewModel.Tools
                    .OfType<HudGraphViewModel>()
                    .Where(t => t.ParentToolId != null && t.ParentToolId == toolViewModel.Id)
                    .OfType<HudBaseToolViewModel>()
                    .ToArray();

                ShowTools(toolsToShow);
            }, canUserStatClickCommand);

            SaveDesignCommand = ReactiveCommand.Create(() => SaveDesign());
        }

        /// <summary>
        /// Shows the specified <see cref="HudBaseToolViewModel"/> tools
        /// </summary>
        /// <param name="toolsToShow">Tools to show</param>
        private void ShowTools(HudBaseToolViewModel[] toolsToShow)
        {
            if (toolsToShow.Length < 1)
            {
                return;
            }

            toolsToShow.ForEach(t =>
            {
                t.IsVisible = true;
                t.IsSelected = false;
            });

            toolsToShow.First(t => t.IsSelected = true);
        }

        /// <summary>
        /// Initializes preview
        /// </summary>
        private void InitializePreview()
        {
            var layoutTools = CurrentLayout.LayoutTools.ToArray();

            var random = new Random();

            // set randomized data
            var previewHudElementViewModel = new HudElementViewModel(layoutTools.Select(x => x.Clone()));
            previewHudElementViewModel.Opacity = CurrentLayout.Opacity;
            previewHudElementViewModel.TiltMeter = HudDefaultSettings.TiltMeterDesignerValue;
            previewHudElementViewModel.Stickers = new ObservableCollection<HudBumperStickerType>(CurrentLayout
                .HudBumperStickerTypes
                .Select(x => x.Clone()));

            previewHudElementViewModel.Tools.OfType<IHudStatsToolViewModel>().ForEach(tool =>
            {
                tool.Stats.ForEach(s =>
                {
                    s.CurrentValue = random.Next(0, 100);
                    s.Caption = string.Format(s.Format, s.CurrentValue);

                    if (StatsProvider.StatsBases.ContainsKey(s.Stat) && StatsProvider.StatsBases[s.Stat].CreateStatDto != null)
                    {
                        s.StatDto = new StatDto(0, 0);
                    }
                });
            });

            var cardRanges = Card.GetCardRanges();

            void initializeHeatMapPreview(HudHeatMapViewModel tool)
            {
                tool.BaseStat.CurrentValue = random.Next(0, 100);

                var heatMap = new HeatMapDto
                {
                    OccuredByCardRange = (from cardRange in cardRanges
                                          let occurred = random.Next(0, 100)
                                          select new { CardRange = cardRange, Occured = occurred }).ToDictionary(x => x.CardRange, x => x.Occured)
                };

                tool.HeatMap = heatMap;
            }

            previewHudElementViewModel.Tools
                .OfType<HudHeatMapViewModel>()
                .ForEach(tool => initializeHeatMapPreview(tool));

            previewHudElementViewModel.Tools.OfType<HudGaugeIndicatorViewModel>()
                .SelectMany(x => x.GroupedStats)
                .SelectMany(x => x.Stats)
                .Where(x => x.HeatMapViewModel != null)
                .ForEach(x => initializeHeatMapPreview(x.HeatMapViewModel));

            previewHudElementViewModel.Seat = 1;
            previewHudElementViewModel.PlayerName = string.Format(HudDefaultSettings.TablePlayerNameFormat, previewHudElementViewModel.Seat);

            try
            {
                previewHudElementViewModel.Tools.ForEach(x => x.InitializePositions());
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not set positions on HUD view for {CurrentLayout.Name}", e);
            }

            PreviewHudElementViewModel = previewHudElementViewModel;
        }

        /// <summary>
        /// Initializes observables
        /// </summary>
        private void InitializeObservables()
        {
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => StatInfoObserveCollection.CollectionChanged += h,
                h => StatInfoObserveCollection.CollectionChanged -= h).Subscribe(x =>
                {
                    if (skipOnStatInfoObserveCollectionChanged)
                    {
                        return;
                    }

                    var statsToAdd = x.EventArgs.NewItems?.OfType<StatInfo>().ToList();
                    var statsToRemove = x.EventArgs.OldItems?.OfType<StatInfo>().ToList();

                    AddToolStats(statsToAdd, x.EventArgs.NewStartingIndex);
                    RemoveToolStats(statsToRemove);

                    HideStatsInStatCollection();

                    InitializePreview();
                });

            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => StatInfoCollection.CollectionChanged += h,
                h => StatInfoCollection.CollectionChanged -= h).Subscribe(x =>
                {
                    if (x.EventArgs.Action != NotifyCollectionChangedAction.Add)
                    {
                        return;
                    }

                    var addedStats = x.EventArgs.NewItems?.OfType<StatInfo>().ToArray();

                    if (addedStats == null)
                    {
                        return;
                    }

                    var allStats = StatsProvider.GetAllStats();

                    // remove invalid stats
                    var validStats = (from addedStat in addedStats
                                      join stat in allStats on new
                                      {
                                          Stat = addedStat.Stat,
                                          GroupName = addedStat.GroupName,
                                          StatInfoGroup = (addedStat.StatInfoGroup != null ? addedStat.StatInfoGroup.Name : null)
                                      }
                                      equals new
                                      {
                                          Stat = stat.Stat,
                                          GroupName = stat.GroupName,
                                          StatInfoGroup = stat.StatInfoGroup.Name
                                      }
                                      select addedStat).ToArray();

                    var invalidStats = addedStats.Except(validStats).ToArray();
                    StatInfoCollection.RemoveAll(invalidStats);

                    // remove duplicates
                    var duplicates = (from stat in StatInfoCollection
                                      group stat by new { stat.Stat, stat.GroupName, stat.StatInfoGroup.Name } into grouped
                                      where grouped.Count() > 1
                                      from duplicateStat in grouped
                                      join validStat in validStats on duplicateStat equals validStat
                                      select duplicateStat).ToArray();

                    StatInfoCollection.RemoveAll(duplicates);
                });
        }

        /// <summary>
        /// Add the list of <see cref="StatInfo"/> stats to the specific tool
        /// </summary>
        /// <param name="statsCollection">List of stats</param>
        private void AddToolStats(List<StatInfo> statsCollection, int startingIndex)
        {
            // in design mode we can update only stats in selected tool
            if (statsCollection == null || (isInDesignMode && SelectedToolViewModel == null) || (SelectedToolViewModel != null && !(SelectedToolViewModel is IHudStatsToolViewModel)))
            {
                return;
            }

            if (SelectedToolViewModel != null && SelectedToolViewModel is HudGaugeIndicatorViewModel)
            {
                statsCollection.ForEach(x => x.SetPopupDefaults());
            }

            var statTool = GetToolToModifyStats();

            if (statTool == null)
            {
                return;
            }

            if (startingIndex > statTool.Stats.Count)
            {
                startingIndex = statTool.Stats.Count;
            }

            statTool.Stats.InsertRange(startingIndex, statsCollection);
        }

        private void RemoveToolStats(List<StatInfo> statsCollection)
        {
            // in design mode we can update only stats in selected tool
            if (statsCollection == null || (isInDesignMode && SelectedToolViewModel == null))
            {
                return;
            }

            var statTool = GetToolToModifyStats();
            statTool?.Stats.RemoveRange(statsCollection);

            // if selected tools is base stat tool then we don't delete any tool
            if (SelectedToolViewModel != null && SelectedToolViewModel is IHudBaseStatToolViewModel)
            {
                return;
            }

            // removes all stat based tools 
            foreach (var stat in statsCollection)
            {
                var toolViewModelsToRemove = (from hudElement in HudElements
                                              from toolViewModel in hudElement.Tools.OfType<IHudBaseStatToolViewModel>()
                                              where toolViewModel.BaseStat != null && toolViewModel.BaseStat.Stat == stat.Stat
                                              select new { HudElement = hudElement, ToolViewModel = toolViewModel }).ToArray();

                toolViewModelsToRemove.ForEach(x =>
                {
                    var toolViewModel = x.ToolViewModel as HudBaseToolViewModel;

                    if (toolViewModel != null)
                    {
                        x.HudElement.Tools.Remove(toolViewModel);
                        CurrentLayout.LayoutTools.Remove(toolViewModel.Tool);
                    }
                });
            }
        }

        private IHudStatsToolViewModel GetToolToModifyStats()
        {
            var hudElement = HudElements?.FirstOrDefault();

            if (hudElement == null)
            {
                return null;
            }

            IHudStatsToolViewModel statTool;

            if (SelectedToolViewModel != null)
            {
                statTool = hudElement.Tools
                    .OfType<IHudStatsToolViewModel>()
                    .FirstOrDefault(x => ReferenceEquals(x, SelectedToolViewModel));
            }
            else
            {
                statTool = hudElement.Tools
                   .Where(x => x.ToolType == HudDesignerToolType.PlainStatBox)
                   .OfType<IHudStatsToolViewModel>()
                   .FirstOrDefault();
            }

            return statTool;
        }

        /// <summary>
        /// Opens popup to save data
        /// </summary>
        private void OpenDataSave()
        {
            if (CurrentLayout == null)
            {
                return;
            }

            var hudSelectLayoutViewModelInfo = new HudSelectLayoutViewModelInfo
            {
                LayoutName = CurrentLayout.Name,
                Cancel = ClosePopup,
                Save = DataSave,
                IsSaveAsMode = true,
                TableType = CurrentTableType
            };

            var hudSelectLayoutViewModel = new HudSelectLayoutViewModel(hudSelectLayoutViewModelInfo);
            OpenPopup(hudSelectLayoutViewModel);
        }

        /// <summary>
        /// Saves layout
        /// </summary>
        private void DataSave()
        {
            var hudSelectLayoutViewModel = PopupViewModel as HudSelectLayoutViewModel;

            if (hudSelectLayoutViewModel == null)
            {
                ClosePopup();
                return;
            }

            var hudData = new HudSavedDataInfo
            {
                Name = hudSelectLayoutViewModel.Name,
                Stats = StatInfoObserveCollection,
                LayoutInfo = CurrentLayout
            };

            ClosePopup();

            var savedLayout = HudLayoutsService.SaveAs(hudData);

            if (savedLayout != null && savedLayout.Name != CurrentLayout.Name)
            {
                var existingLayout = Layouts.FirstOrDefault(x => x.Name == savedLayout.Name);

                if (existingLayout != null)
                {
                    Layouts.Remove(existingLayout);
                }

                Layouts.Add(savedLayout);

                var tempCurrentLayout = CurrentLayout;

                CurrentLayout = savedLayout;

                var originalLayout = HudLayoutsService.GetLayout(tempCurrentLayout.Name);

                if (originalLayout != null)
                {
                    var currentLayoutIndex = Layouts.IndexOf(tempCurrentLayout);

                    Layouts.Insert(currentLayoutIndex, originalLayout);
                    Layouts.Remove(tempCurrentLayout);
                }
            }

            var settings = SettingsService.GetSettings();

            if (settings.GeneralSettings.IsHudSavedAtFirstTime)
            {
                var hudSaveFirstTimeNotificationViewModel = new HudSaveFirstTimeNotificationViewModel(ClosePopup);
                OpenPopup(hudSaveFirstTimeNotificationViewModel);

                settings.GeneralSettings.IsHudSavedAtFirstTime = false;
                SettingsService.SaveSettings(settings);
            }
        }

        /// <summary>
        /// Opens popup to duplicate data
        /// </summary>
        private void OpenDuplicate()
        {
            var hudDuplicateLayoutViewModelInfo = new HudDuplicateLayoutViewModelInfo
            {
                LayoutName = CurrentLayout.Name,
                Cancel = ClosePopup,
                Save = DuplicateLayout
            };

            var hudDuplicateLayoutViewModel = new HudDuplicateLayoutViewModel(hudDuplicateLayoutViewModelInfo);
            OpenPopup(hudDuplicateLayoutViewModel);
        }

        /// <summary>
        /// Duplicates the selected layout
        /// </summary>
        private void DuplicateLayout()
        {
            var hudDuplicateLayoutViewModel = PopupViewModel as HudDuplicateLayoutViewModel;

            if (hudDuplicateLayoutViewModel == null || !hudDuplicateLayoutViewModel.SelectedTableType.HasValue)
            {
                ClosePopup();
                return;
            }

            var duplicatedLayout = HudLayoutsService.DuplicateLayout(hudDuplicateLayoutViewModel.SelectedTableType.Value, hudDuplicateLayoutViewModel.Name, CurrentLayout);

            if (duplicatedLayout != null)
            {
                CurrentTableType = duplicatedLayout.TableType;

                var layout = Layouts.FirstOrDefault(x => x.Name == duplicatedLayout.Name);

                if (layout != null)
                {
                    CurrentLayout = layout;
                }
            }

            ClosePopup();
        }

        /// <summary>
        /// Exports layout to file
        /// </summary>
        private void DataExport()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "HUD Layouts (.xml)|*.xml"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                HudLayoutsService.Export(CurrentLayout, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Imports layout from file
        /// </summary>
        private void DataImport()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "HUD Layouts (.xml)|*.xml", Multiselect = true };

            if (openFileDialog.ShowDialog() == true)
            {
                HudLayoutInfoV2 importedLayout = null;

                foreach (var fileName in openFileDialog.FileNames)
                {
                    importedLayout = HudLayoutsService.Import(fileName);
                }

                if (importedLayout == null)
                {
                    return;
                }

                if (CurrentTableType != importedLayout.TableType)
                {
                    CurrentTableType = importedLayout.TableType;
                    CurrentLayout = Layouts.FirstOrDefault(x => x.Name == importedLayout.Name && x.TableType == importedLayout.TableType);
                    return;
                }

                Layouts.Add(importedLayout);
                CurrentLayout = importedLayout;
            }
        }

        /// <summary>
        /// Refreshes stats and hud elements using <see cref="CurrentLayout"/>
        /// </summary>
        private void RefreshData()
        {
            UpdateStatsCollections();
            InitializeHudElements();
        }

        /// <summary>
        /// Deletes layout
        /// </summary>
        private void DataDelete()
        {
            var deleteLayout = false;

            var notification = new PopupBaseNotification()
            {
                Title = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_DeleteHudTitle"),
                CancelButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_DeleteHudCancel"),
                ConfirmButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_DeleteHudYes"),
                Content = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_DeleteHudContent"),
                IsDisplayH1Text = true
            };

            NotificationRequest.Raise(notification,
                  confirmation =>
                  {
                      if (confirmation.Confirmed)
                      {
                          deleteLayout = true;
                      }
                  });

            if (!deleteLayout)
            {
                return;
            }

            if (!HudLayoutsService.Delete(CurrentLayout.Name))
            {
                return;
            }

            Layouts.Remove(CurrentLayout);
            CurrentLayout = Layouts.FirstOrDefault();
        }

        private void UpdateStatsCollections()
        {
            if (CurrentLayout == null)
            {
                return;
            }

            if (StatInfoObserveCollection.Count > 0)
            {
                ReturnStatsToStatCollection();
            }

            AddStatsToSelectedStatCollection();

            HideStatsInStatCollection();
        }

        private void ReturnStatsToStatCollection()
        {
            // Get all chosen stats back to list
            foreach (var statInfo in StatInfoObserveCollection)
            {
                if (statInfo is StatInfoBreak)
                {
                    continue;
                }

                // we need to purge stat info from user changes before we return it to stat collection, 
                // but we can't modify original stat, because it belongs to layout
                var purgedStatInfo = statInfo.Clone();

                if (purgedStatInfo.Stat != Stat.PlayerInfoIcon)
                {
                    purgedStatInfo.Reset();
                    purgedStatInfo.Initialize();
                }

                StatInfoCollection.Add(purgedStatInfo);
            }

            StatInfoObserveCollection.Clear();
        }

        private void AddStatsToSelectedStatCollection()
        {
            skipOnStatInfoObserveCollectionChanged = true;

            var stats = GetSelectedStats();

            var statsToAdd = new List<StatInfo>();

            foreach (var hudStat in stats)
            {
                if (hudStat is StatInfoBreak)
                {
                    statsToAdd.Add(hudStat);
                    continue;
                }

                var existing = StatInfoCollection.FirstOrDefault(x => x.Stat == hudStat.Stat);

                if (existing != null)
                {
                    // we do not recover unexpected stats
                    statsToAdd.Add(hudStat);
                }
            }

            if (statsToAdd.Count > 0)
            {
                StatInfoObserveCollection.AddRange(statsToAdd);
            }

            skipOnStatInfoObserveCollectionChanged = false;
        }

        private IEnumerable<StatInfo> GetSelectedStats()
        {
            if (IsInDesignMode)
            {
                if (SelectedToolViewModel != null && SelectedToolViewModel is IHudStatsToolViewModel)
                {
                    return (SelectedToolViewModel as IHudStatsToolViewModel).Stats.ToArray();
                }

                return new List<StatInfo>();
            }

            return CurrentLayout.LayoutTools.OfType<HudLayoutPlainBoxTool>().SelectMany(x => x.Stats).ToArray();
        }

        private void HideStatsInStatCollection()
        {
            if (SelectedToolViewModel != null && SelectedToolViewModel is IHudStatsToolViewModel &&
                !(SelectedToolViewModel is IHudNonPopupToolViewModel))
            {
                var statsSelectedToolViewModel = SelectedToolViewModel as IHudStatsToolViewModel;
                HideStatsInStatCollection(statsSelectedToolViewModel.Stats);

                return;
            }

            var statsToHide = CurrentLayout.LayoutTools.OfType<HudLayoutPlainBoxTool>().SelectMany(x => x.Stats).ToArray();

            HideStatsInStatCollection(statsToHide);
        }

        private void HideStatsInStatCollection(IEnumerable<StatInfo> stats)
        {
            var statsToHide = (from layoutStat in stats
                               join statInfo in StatInfoCollection on layoutStat.Stat equals statInfo.Stat
                               select statInfo).ToArray();

            StatInfoCollection.Where(x => x.IsNotVisible).ForEach(x => x.IsNotVisible = false);
            statsToHide.ForEach(x => x.IsNotVisible = true);
            stats.ForEach(x => x.IsNotVisible = false);
        }

        /// <summary>
        /// Adds line break stat 
        /// </summary>
        private void SpliterAdd()
        {
            var breakLineStat = new StatInfoBreak();
            StatInfoObserveCollection.Add(breakLineStat);
        }

        /// <summary>
        /// Opens pop-up and initialize stat info settings
        /// </summary>
        /// <param name="selectedStatInfo">Selected stat info</param>
        private void OpenStatsSettings(StatInfo selectedStatInfo)
        {
            if (StatInfoObserveCollection.Count == 0)
            {
                return;
            }

            var opacity = CurrentLayout?.Opacity ?? 0;

            var hudStatSettingsViewModelInfo = new HudStatSettingsViewModelInfo
            {
                SelectedStatInfo = selectedStatInfo,
                SelectedStatInfoCollection = StatInfoObserveCollection,
                SelectedTableTypes = CurrentLayout?.Filter?.TableTypes?.Select(x => (EnumTableType)x).ToArray(),
                DataFreshness = CurrentLayout != null && CurrentLayout.Filter != null ? CurrentLayout.Filter.DataFreshness : 0,
                HudOpacity = opacity,
                Save = SaveStatsSettings,
                Cancel = ClosePopup
            };

            var hudStatSettingsViewModel = new HudStatSettingsViewModel(hudStatSettingsViewModelInfo);

            OpenPopup(hudStatSettingsViewModel);
        }

        /// <summary>
        /// Closes pop-up and save data
        /// </summary>
        private void SaveStatsSettings()
        {
            var hudStatSettings = PopupViewModel as HudStatSettingsViewModel;

            if (hudStatSettings == null)
            {
                return;
            }

            var statInfoToMerge = (from item in hudStatSettings.Items
                                   join statInfo in StatInfoObserveCollection on item.Id equals statInfo.Id
                                   select new { NewItem = item, OldItem = statInfo }).ToArray();

            CurrentLayout.Opacity = hudStatSettings.HudOpacity;

            var filter = new HudLayoutFilter
            {
                DataFreshness = hudStatSettings.DataFreshness.HasValue ? (int)hudStatSettings.DataFreshness.Value : 0,
                TableTypes = hudStatSettings.FilterTableTypes
                    .Where(x => x.IsSelected)
                    .Select(x => (int)x.TableType)
                    .OrderBy(x => x)
                    .ToArray()
            };

            CurrentLayout.Filter = filter.IsDefault ? null : filter;

            foreach (var mergeItem in statInfoToMerge)
            {
                mergeItem.OldItem.Merge(mergeItem.NewItem);

                if (SelectedToolViewModel != null && SelectedToolViewModel is HudGaugeIndicatorViewModel)
                {
                    mergeItem.OldItem.UpdateColor();
                }

                var previewStat = PreviewHudElementViewModel.ToolsStatInfoCollection.FirstOrDefault(x => x.Stat == mergeItem.NewItem.Stat);
                previewStat?.Merge(mergeItem.NewItem);
                previewStat?.UpdateColor();
            }

            HudElements.ForEach(e => e.Opacity = hudStatSettings.HudOpacity);

            InitializePreview();

            ClosePopup();
        }

        /// <summary>
        /// Open pop-up and initialize player types 
        /// </summary>
        /// <param name="selectedStatInfo"></param>
        private void OpenPlayerTypeStats(StatInfo selectedStatInfo)
        {
            if (StatInfoObserveCollection.Count == 0)
            {
                return;
            }

            var hudPlayerTypes = CurrentLayout?.HudPlayerTypes;

            if (hudPlayerTypes == null)
            {
                return;
            }

            var hudPlayerSettingsViewModelInfo = new HudPlayerSettingsViewModelInfo
            {
                PlayerTypes = hudPlayerTypes.Select(x => x.Clone()),
                Save = PlayerTypeSave,
                TableType = CurrentTableType
            };

            var hudPlayerSettingsViewModel = new HudPlayerSettingsViewModel(hudPlayerSettingsViewModelInfo);

            OpenPopup(hudPlayerSettingsViewModel);
        }

        /// <summary>
        /// Open pop-up and initialize bumper stickers
        /// </summary>
        /// <param name="selectedStatInfo"></param>
        private void OpenBumperStickers(StatInfo selectedStatInfo)
        {
            if (StatInfoObserveCollection.Count == 0)
            {
                return;
            }
            var hudBumperStickers = CurrentLayout?.HudBumperStickerTypes;
            if (hudBumperStickers == null)
                return;

            var hudBumperStickersSettingsViewModelInfo = new HudBumperStickersSettingsViewModelInfo
            {
                BumperStickers = hudBumperStickers.Select(x => x.Clone()),
                Save = BumperStickerSave
            };

            var hudBumperStickersViewModel =
                new HudBumperStickersSettingsViewModel(hudBumperStickersSettingsViewModelInfo);

            OpenPopup(hudBumperStickersViewModel);
        }

        /// <summary>
        /// Save player type data from pop-up
        /// </summary>
        private void PlayerTypeSave()
        {
            var hudPlayerSettingsViewModel = PopupViewModel as HudPlayerSettingsViewModel;

            if (hudPlayerSettingsViewModel == null)
            {
                ClosePopup();
                return;
            }

            // merge data to current layout (currently we do not expect new stats or deleted stats)
            var playerTypesToMergeDelete = (from currentPlayerType in CurrentLayout.HudPlayerTypes
                                            join playerType in hudPlayerSettingsViewModel.PlayerTypes on currentPlayerType.Id equals playerType.Id into ptgj
                                            from ptgrouped in ptgj.DefaultIfEmpty()
                                            select new { CurrentPlayerType = currentPlayerType, PlayerType = ptgrouped }).ToArray();

            var playerTypesToMerge = playerTypesToMergeDelete.Where(x => x.PlayerType != null);

            playerTypesToMerge.ForEach(pt =>
            {
                pt.CurrentPlayerType.MergeWith(pt.PlayerType);
            });

            var playerTypesToAdd = (from playerType in hudPlayerSettingsViewModel.PlayerTypes
                                    join currentPlayerType in CurrentLayout.HudPlayerTypes on playerType.Id equals currentPlayerType.Id into ptgj
                                    from ptgrouped in ptgj.DefaultIfEmpty()
                                    where ptgrouped == null
                                    select new { AddedPlayerType = playerType }).ToArray();

            playerTypesToAdd.ForEach(pt =>
            {
                CurrentLayout.HudPlayerTypes.Add(pt.AddedPlayerType);
            });

            var playerTypesToDelete = playerTypesToMergeDelete.Where(x => x.PlayerType == null).Select(x => x.CurrentPlayerType).ToArray();

            playerTypesToDelete.ForEach(pt =>
            {
                CurrentLayout.HudPlayerTypes.Remove(pt);
            });

            ClosePopup();
        }

        /// <summary>
        /// Save bumper sticker data from pop-up
        /// </summary>
        private void BumperStickerSave()
        {
            var hudStickersSettingsViewModel = PopupViewModel as HudBumperStickersSettingsViewModel;

            if (hudStickersSettingsViewModel == null)
            {
                ClosePopup();
                return;
            }

            // merge data to current layout (currently we do not expect new stats or deleted stats)
            var stickersTypesToMerge = (from currentStickerType in CurrentLayout.HudBumperStickerTypes
                                        join stickerType in hudStickersSettingsViewModel.BumperStickers on currentStickerType.Name equals stickerType.Name into stgj
                                        from stgrouped in stgj.DefaultIfEmpty()
                                        where stgrouped != null
                                        select new { CurrentStickerType = currentStickerType, StickerType = stgrouped }).ToArray();

            stickersTypesToMerge.ForEach(st =>
            {
                st.CurrentStickerType.MinSample = st.StickerType.MinSample;
                st.CurrentStickerType.EnableBumperSticker = st.StickerType.EnableBumperSticker;
                st.CurrentStickerType.SelectedColor = st.StickerType.SelectedColor;
                st.CurrentStickerType.Name = st.StickerType.Name;
                st.CurrentStickerType.Description = st.StickerType.Description;

                if (st.StickerType.FilterModelCollection != null)
                {
                    st.CurrentStickerType.FilterModelCollection = new IFilterModelCollection(st.StickerType.FilterModelCollection.Select(x => (IFilterModel)x.Clone()));
                }
                else
                {
                    st.CurrentStickerType.FilterModelCollection = new IFilterModelCollection();
                }

                var statsToMerge = (from currentStat in st.CurrentStickerType.Stats
                                    join stat in st.StickerType.Stats on currentStat.Stat equals stat.Stat into gj
                                    from grouped in gj.DefaultIfEmpty()
                                    where grouped != null
                                    select new { CurrentStat = currentStat, Stat = grouped }).ToArray();

                statsToMerge.ForEach(s =>
                {
                    s.CurrentStat.Low = s.Stat.Low;
                    s.CurrentStat.High = s.Stat.High;
                });
            });

            var stickerTypesToAdd = (from stickerType in hudStickersSettingsViewModel.BumperStickers
                                     join currentStickerType in CurrentLayout.HudBumperStickerTypes on stickerType.Name equals currentStickerType.Name into stgj
                                     from stgrouped in stgj.DefaultIfEmpty()
                                     where stgrouped == null
                                     select new { AddedPlayerType = stickerType }).ToArray();

            stickerTypesToAdd.ForEach(st =>
            {
                CurrentLayout.HudBumperStickerTypes.Add(st.AddedPlayerType);
            });

            CurrentLayout.HudBumperStickerTypes.ForEach(x => x.InitializeFilterPredicate());

            ClosePopup();
        }

        internal void RefreshHudTable()
        {
            this.RaisePropertyChanged(nameof(CurrentTableType));
        }

        private void InitializeDesigner()
        {
            if (IsInDesignMode)
            {
                return;
            }

            if (designerDisposables != null)
            {
                designerDisposables.Dispose();
            }

            designerDisposables = new CompositeDisposable();

            cachedCurrentLayout = CurrentLayout;

            var clonedLayout = CurrentLayout.Clone();
            Layouts.Add(clonedLayout);
            CurrentLayout = clonedLayout;

            DesignerHudElementViewModel = HudElements.FirstOrDefault(x => x.Seat == HudDefaultSettings.DesignerDefaultSeat);

            if (DesignerHudElementViewModel != null)
            {
                // need to update all UI positions, because positions could be changed
                var factory = ServiceLocator.Current.GetInstance<IHudToolFactory>();

                var baseStats = DesignerHudElementViewModel.Tools
                    .OfType<IHudBaseStatToolViewModel>()
                    .Where(x => x.BaseStat != null)
                    .Select(x => x.BaseStat.Stat).ToArray();

                foreach (var tool in DesignerHudElementViewModel.Tools)
                {
                    var uiPositions = factory.GetHudUIPositions(EnumTableType.HU, CurrentTableType, tool.Position);
                    tool.SetPositions(uiPositions);
                    tool.InitializePositions();

                    if (tool is IHudStatsToolViewModel)
                    {
                        (tool as IHudStatsToolViewModel).Stats.ForEach(x =>
                        {
                            if (baseStats.Contains(x.Stat))
                            {
                                x.HasAttachedTools = true;
                            }
                        });
                    }
                }

                HudElements = new ReactiveList<HudElementViewModel>
                {
                    DesignerHudElementViewModel
                };

                DesignerHudElementViewModel.Tools.ChangeTrackingEnabled = true;

                var disposable = DesignerHudElementViewModel.Tools.ItemChanged
                        .Where(x => x.PropertyName.Equals(nameof(HudBaseToolViewModel.IsSelected)))
                        .Subscribe(x =>
                        {
                            if (x.Sender.IsSelected)
                            {
                                SelectedToolViewModel = x.Sender;
                            }
                            else
                            {
                                SelectedToolViewModel = null;
                                InitializePreview();
                            }

                            UpdateStatsCollections();
                        });

                designerDisposables.Add(disposable);
            }

            IsInDesignMode = true;
        }

        /// <summary>
        /// Adds designer tool on table
        /// </summary>
        /// <param name="toolType">Type of tool</param>
        /// <param name="position">Position of tool</param>
        private void AddTool(HudDesignerToolType toolType, Point position, object source)
        {
            if (DesignerHudElementViewModel == null)
            {
                return;
            }

            var factory = ServiceLocator.Current.GetInstance<IHudToolFactory>();

            var creationInfo = new HudToolCreationInfo
            {
                HudElement = DesignerHudElementViewModel,
                Position = position,
                TableType = CurrentTableType,
                ToolType = toolType,
                Layout = CurrentLayout,
                Tools = CurrentLayout.LayoutTools,
                Source = source
            };

            var toolViewModel = factory.CreateTool(creationInfo);

            if (toolViewModel == null)
            {
                return;
            }

            DesignerHudElementViewModel.Tools.Add(toolViewModel);

            toolViewModel.IsSelected = true;

            if ((toolViewModel is IHudBaseStatToolViewModel hudBaseStatToolViewModel) &&
                hudBaseStatToolViewModel.BaseStat != null)
            {
                var statsToUpdate = DesignerHudElementViewModel.Tools
                    .OfType<IHudStatsToolViewModel>()
                    .Where(x => !(x is IHudBaseStatToolViewModel))
                    .SelectMany(x => x.Stats)
                    .Where(x => x.Stat == hudBaseStatToolViewModel.BaseStat.Stat)
                    .ToArray();

                statsToUpdate.ForEach(x => x.HasAttachedTools = true);
            }

            InitializePreview();
        }

        /// <summary>
        /// Check if tool can be added
        /// </summary>
        /// <param name="toolType">Type of tool</param>
        /// <returns>True if tool can be added, otherwise - false</returns>
        private bool CanAddTool(HudDesignerToolType toolType)
        {
            return IsInDesignMode && DesignerHudElementViewModel != null;
        }

        /// <summary>
        /// Removes layout tool from table 
        /// </summary>
        /// <param name="toolViewModel">Tool to remove</param>
        private void RemoveTool(HudBaseToolViewModel toolViewModel)
        {
            if (!IsInDesignMode || DesignerHudElementViewModel == null || CurrentLayout == null || toolViewModel == null)
            {
                return;
            }

            DesignerHudElementViewModel.Tools.Remove(toolViewModel);
            CurrentLayout.LayoutTools.Remove(toolViewModel.Tool);

            InitializePreview();

            // updates all tools which might have stats linked to base stat of deleted tool
            if (toolViewModel is IHudBaseStatToolViewModel)
            {
                var hudGaugeIndicatorViewModel = toolViewModel as IHudBaseStatToolViewModel;

                if (hudGaugeIndicatorViewModel.BaseStat == null)
                {
                    return;
                }

                var remainingBaseStatTools = DesignerHudElementViewModel.Tools
                    .OfType<IHudBaseStatToolViewModel>()
                    .Where(x => x.BaseStat != null && x.BaseStat.Stat == hudGaugeIndicatorViewModel.BaseStat.Stat)
                    .Count();

                if (remainingBaseStatTools > 0)
                {
                    return;
                }

                var statInfos = DesignerHudElementViewModel.Tools
                    .OfType<IHudStatsToolViewModel>()
                    .SelectMany(x => x.Stats)
                    .Where(x => x.Stat == hudGaugeIndicatorViewModel.BaseStat.Stat)
                    .ToArray();

                statInfos.ForEach(x => x.HasAttachedTools = false);

                return;
            }

            var statToolViewModel = toolViewModel as IHudStatsToolViewModel;

            if (statToolViewModel == null)
            {
                return;
            }

            var baseStatToolsToDelete = (from stat in statToolViewModel.Stats
                                         join baseStatTool in DesignerHudElementViewModel.Tools.OfType<IHudBaseStatToolViewModel>().Where(x => x.BaseStat != null)
                                            on stat.Stat equals baseStatTool.BaseStat.Stat
                                         select baseStatTool).ToArray();

            baseStatToolsToDelete.ForEach(x => RemoveTool(x as HudBaseToolViewModel));
        }

        /// <summary>
        /// Saves current design
        /// </summary>
        private void SaveDesign()
        {
            // need to update all UI positions, because positions could be changed
            var factory = ServiceLocator.Current.GetInstance<IHudToolFactory>();

            foreach (var tool in DesignerHudElementViewModel.Tools)
            {
                var uiPositions = factory.GetHudUIPositions(CurrentTableType, EnumTableType.HU, tool.Position);
                tool.SavePositions(uiPositions);
            }

            cachedCurrentLayout.HudPlayerTypes = CurrentLayout.HudPlayerTypes;
            cachedCurrentLayout.HudBumperStickerTypes = CurrentLayout.HudBumperStickerTypes;
            cachedCurrentLayout.LayoutTools = CurrentLayout.LayoutTools;

            CloseDesigner();

            var notification = new PopupBaseNotification()
            {
                Title = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_SaveDesignTitle"),
                CancelButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_SaveDesignNo"),
                ConfirmButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_SaveDesignYes"),
                Content = CommonResourceManager.Instance.GetResourceString("Notifications_HudLayout_SaveDesignContent"),
                IsDisplayH1Text = true
            };

            NotificationRequest.Raise(notification,
                  confirmation =>
                  {
                      if (confirmation.Confirmed)
                      {
                          OpenDataSave();
                      }
                  });
        }

        /// <summary>
        /// Closes designer
        /// </summary>
        private void CloseDesigner()
        {
            DesignerHudElementViewModel.Tools
                .OfType<IHudStatsToolViewModel>()
                .SelectMany(x => x.Stats)
                .ForEach(x =>
                {
                    x.HasAttachedTools = false;
                    x.IsSelected = false;
                });

            DesignerHudElementViewModel = null;

            Layouts.Remove(CurrentLayout);
            CurrentLayout = cachedCurrentLayout;
            cachedCurrentLayout = null;

            SelectedToolViewModel = null;

            IsInDesignMode = false;

            designerDisposables.Dispose();
        }

        #endregion
    }
}