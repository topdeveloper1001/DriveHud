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
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.AttachedBehaviors;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
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
    public class HudViewModel : PopupViewModelBase
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
            NotificationRequest = new InteractionRequest<INotification>();

            InitializeStatInfoCollection();
            InitializeStatInfoCollectionObserved();
            InitializeTableTypes();
            InitializeCommands();
            InitializeObservables();

            CurrentTableType = TableTypes.FirstOrDefault();
        }

        #region Properties

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

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

                    return stat.IsListed && !stat.IsNotVisible;
                };

                var statFiltering = collectionViewSource as ICollectionViewLiveShaping;

                if (statFiltering.CanChangeLiveFiltering)
                {
                    statFiltering.LiveFilteringProperties.Add(nameof(StatInfo.IsNotVisible));
                    statFiltering.IsLiveFiltering = true;
                }

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
            }
        }

        public bool CanAddStats
        {
            get
            {
                return !IsInDesignMode || (IsInDesignMode && SelectedToolViewModel != null && SelectedToolViewModel is IHudStatsToolViewModel);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> DataSaveCommand { get; private set; }

        public ReactiveCommand<object> DataDeleteCommand { get; private set; }

        public ReactiveCommand<object> DataImportCommand { get; private set; }

        public ReactiveCommand<object> DataExportCommand { get; private set; }

        public ReactiveCommand<object> SpliterAddCommand { get; private set; }

        public ReactiveCommand<object> SettingsStatInfoCommand { get; private set; }

        public ReactiveCommand<object> PlayerTypeStatsCommand { get; private set; }

        public ReactiveCommand<object> BumperStickersCommand { get; private set; }

        public ReactiveCommand<object> DesignerToolCommand { get; private set; }

        public ReactiveCommand<object> SaveDesignCommand { get; private set; }

        public ReactiveCommand<object> CancelDesignCommand { get; private set; }

        public ICommand AddToolCommand { get; private set; }

        public ReactiveCommand<object> RemoveToolCommand { get; private set; }

        public ReactiveCommand<object> StatClickCommand { get; private set; }

        public ReactiveCommand<object> ToolClickCommand { get; private set; }

        public ReactiveCommand<object> DuplicateCommand { get; private set; }

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
            StatInfoCollection = new ReactiveList<StatInfo>(StatInfoHelper.GetAllStats());

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

            DataSaveCommand = ReactiveCommand.Create(canUseDataCommands);
            DataSaveCommand.Subscribe(x => OpenDataSave());

            DataDeleteCommand = ReactiveCommand.Create(canUseDataCommands);
            DataDeleteCommand.Subscribe(x => DataDelete());

            DataImportCommand = ReactiveCommand.Create(canUseDataCommands);
            DataImportCommand.Subscribe(x => DataImport());

            DataExportCommand = ReactiveCommand.Create(canUseDataCommands);
            DataExportCommand.Subscribe(x => DataExport());

            DuplicateCommand = ReactiveCommand.Create(canUseDataCommands);
            DuplicateCommand.Subscribe(x => { });

            SpliterAddCommand = ReactiveCommand.Create();
            SpliterAddCommand.Subscribe(x => SpliterAdd());

            SettingsStatInfoCommand = ReactiveCommand.Create();
            SettingsStatInfoCommand.Subscribe(x => OpenStatsSettings(x as StatInfo));

            PlayerTypeStatsCommand = ReactiveCommand.Create();
            PlayerTypeStatsCommand.Subscribe(x => OpenPlayerTypeStats(x as StatInfo));

            BumperStickersCommand = ReactiveCommand.Create();
            BumperStickersCommand.Subscribe(x => OpenBumperStickers(x as StatInfo));

            DesignerToolCommand = ReactiveCommand.Create();
            DesignerToolCommand.Subscribe(x => InitializeDesigner());

            CancelDesignCommand = ReactiveCommand.Create();
            CancelDesignCommand.Subscribe(x => CloseDesigner());

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

                if (dragDropDataObject.Source is HudFourStatsBoxViewModel || dragDropDataObject.Source is HudPlainStatBoxViewModel)
                {
                    var statInfoList = dragDropDataObject.DragEventArgs.Data.GetData("Model.Stats.StatInfo") as List<object>;

                    var statInfo = statInfoList.OfType<StatInfo>().FirstOrDefault();

                    if (statInfo == null)
                    {
                        return;
                    }

                    var targetToolViewModel = dragDropDataObject.Source as HudBaseToolViewModel;

                    if (targetToolViewModel == null)
                    {
                        return;
                    }

                    targetToolViewModel = DesignerHudElementViewModel.Tools.FirstOrDefault(t => t.Id == targetToolViewModel.Id);

                    SelectedToolViewModel = targetToolViewModel;

                    UpdateStatsCollections();

                    StatInfoObserveCollection.Add(statInfo);

                    return;
                }

                var dataObject = dragDropDataObject.DragEventArgs.Data.GetData(DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.DataFormat.Name);

                var toolType = dataObject as HudDesignerToolType?;

                if (toolType.HasValue && CanAddTool(toolType.Value))
                {
                    AddTool(toolType.Value, dragDropDataObject.Position, dragDropDataObject.Source);
                }
            }, x =>
            {
                var dragEventArgs = x as DragEventArgs;

                if (dragEventArgs == null)
                {
                    return false;
                }

                var statInfoList = dragEventArgs.Data.GetData("Model.Stats.StatInfo") as List<object>;

                var statInfo = statInfoList?.OfType<StatInfo>().FirstOrDefault();

                if (statInfo == null)
                {
                    return false;
                }

                var dragTarget = dragEventArgs.Source as FrameworkElement;

                if (dragTarget == null || dragTarget.DataContext == null)
                {
                    return false;
                }

                return dragTarget.DataContext is HudFourStatsBoxViewModel || dragTarget.DataContext is HudPlainStatBoxViewModel;
            });

            RemoveToolCommand = ReactiveCommand.Create();
            RemoveToolCommand.Subscribe(x =>
            {
                var toolToRemove = x as HudBaseToolViewModel;

                if (toolToRemove == null)
                {
                    return;
                }

                RemoveTool(toolToRemove);
            });

            var canUserStatClickCommand = this.WhenAny(x => x.IsInDesignMode, x => x.Value && DesignerHudElementViewModel != null);

            StatClickCommand = ReactiveCommand.Create(canUserStatClickCommand);
            StatClickCommand.Subscribe(x =>
            {
                var statInfo = x as StatInfo;

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
            });

            ToolClickCommand = ReactiveCommand.Create(canUserStatClickCommand);
            ToolClickCommand.Subscribe(x =>
            {
                var toolViewModel = x as HudBaseToolViewModel;

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
            });

            SaveDesignCommand = ReactiveCommand.Create();
            SaveDesignCommand.Subscribe(x => SaveDesign());
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
                });
            });

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

                    InitializePreview();
                });
        }

        /// <summary>
        /// Add the list of <see cref="StatInfo"/> stats to the specific tool
        /// </summary>
        /// <param name="statsCollection">List of stats</param>
        private void AddToolStats(List<StatInfo> statsCollection, int startingIndex)
        {
            // in design mode we can update only stats in selected tool
            if (statsCollection == null || (isInDesignMode && SelectedToolViewModel == null) || !(SelectedToolViewModel is IHudStatsToolViewModel))
            {
                return;
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

            var statTool = hudElement.Tools.OfType<IHudStatsToolViewModel>()
                            .FirstOrDefault(x => (SelectedToolViewModel != null && ReferenceEquals(x, SelectedToolViewModel)) ||
                                SelectedToolViewModel == null);

            return statTool;
        }

        /// <summary>
        /// Opens popup to save data
        /// </summary>
        private void OpenDataSave()
        {
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
                Layouts.Add(savedLayout);
                CurrentLayout = savedLayout;
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
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "HUD Layouts (.xml)|*.xml" };

            if (openFileDialog.ShowDialog() == true)
            {
                var importedLayout = HudLayoutsService.Import(openFileDialog.FileName);

                if (importedLayout == null)
                {
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

                var existing = StatInfoCollection.FirstOrDefault(x => x.Stat == hudStat.Stat && x.StatInfoGroup?.Name == hudStat.StatInfoGroup?.Name);

                if (existing != null)
                {
                    // we do not recover unexpected stats
                    statsToAdd.Add(hudStat);
                    StatInfoCollection.Remove(existing);
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
            if (SelectedToolViewModel != null && SelectedToolViewModel is IHudStatsToolViewModel)
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
        }

        /// <summary>
        /// Adds line break stat 
        /// </summary>
        private void SpliterAdd()
        {
            StatInfoObserveCollection.Add(new StatInfoBreak());
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

            foreach (var mergeItem in statInfoToMerge)
            {
                mergeItem.OldItem.Merge(mergeItem.NewItem);

                var previewStat = PreviewHudElementViewModel.StatInfoCollection.FirstOrDefault(x => x.Stat == mergeItem.NewItem.Stat);
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
                return;
            var hudPlayerTypes = CurrentLayout?.HudPlayerTypes;
            if (hudPlayerTypes == null)
                return;
            var hudPlayerSettingsViewModelInfo = new HudPlayerSettingsViewModelInfo
            {
                PlayerTypes = hudPlayerTypes.Select(x => x.Clone()),
                Save = PlayerTypeSave
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
            var playerTypesToMerge = (from currentPlayerType in CurrentLayout.HudPlayerTypes
                                      join playerType in hudPlayerSettingsViewModel.PlayerTypes on currentPlayerType.Name equals playerType.Name into ptgj
                                      from ptgrouped in ptgj.DefaultIfEmpty()
                                      where ptgrouped != null
                                      select new { CurrentPlayerType = currentPlayerType, PlayerType = ptgrouped }).ToArray();

            playerTypesToMerge.ForEach(pt =>
            {
                pt.CurrentPlayerType.MinSample = pt.PlayerType.MinSample;
                pt.CurrentPlayerType.EnablePlayerProfile = pt.PlayerType.EnablePlayerProfile;
                pt.CurrentPlayerType.DisplayPlayerIcon = pt.PlayerType.DisplayPlayerIcon;

                var statsToMerge = (from currentStat in pt.CurrentPlayerType.Stats
                                    join stat in pt.PlayerType.Stats on currentStat.Stat equals stat.Stat into gj
                                    from grouped in gj.DefaultIfEmpty()
                                    where grouped != null
                                    select new { CurrentStat = currentStat, Stat = grouped }).ToArray();

                statsToMerge.ForEach(s =>
                {
                    s.CurrentStat.Low = s.Stat.Low;
                    s.CurrentStat.High = s.Stat.High;
                });
            });

            var playerTypesToAdd = (from playerType in hudPlayerSettingsViewModel.PlayerTypes
                                    join currentPlayerType in CurrentLayout.HudPlayerTypes on playerType.Name equals currentPlayerType.Name into ptgj
                                    from ptgrouped in ptgj.DefaultIfEmpty()
                                    where ptgrouped == null
                                    select new { AddedPlayerType = playerType }).ToArray();

            playerTypesToAdd.ForEach(pt =>
            {
                CurrentLayout.HudPlayerTypes.Add(pt.AddedPlayerType);
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
                Source = source
            };

            var toolViewModel = factory.CreateTool(creationInfo);

            if (toolViewModel == null)
            {
                return;
            }

            DesignerHudElementViewModel.Tools.Add(toolViewModel);

            toolViewModel.IsSelected = true;

            if (toolViewModel is IHudBaseStatToolViewModel)
            {
                var hudBaseStatToolViewModel = toolViewModel as IHudBaseStatToolViewModel;

                if (hudBaseStatToolViewModel.BaseStat != null)
                {
                    var statsToUpdate = DesignerHudElementViewModel.Tools
                        .OfType<IHudStatsToolViewModel>()
                        .Where(x => !(x is IHudBaseStatToolViewModel))
                        .SelectMany(x => x.Stats)
                        .Where(x => x.Stat == hudBaseStatToolViewModel.BaseStat.Stat)
                        .ToArray();

                    statsToUpdate.ForEach(x => x.HasAttachedTools = true);
                }
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