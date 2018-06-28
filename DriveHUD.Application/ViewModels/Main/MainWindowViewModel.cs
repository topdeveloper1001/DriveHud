//-----------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.API;
using DriveHUD.Application.HudServices;
using DriveHUD.Application.Licensing;
using DriveHUD.Application.Models;
using DriveHUD.Application.Services;
using DriveHUD.Application.ViewModels.Alias;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Application.ViewModels.Update;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Common.Wpf.Helpers;
using DriveHUD.Common.Wpf.Interactivity;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.BetOnline;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
using Model.Enums;
using Model.Events;
using Model.Filters;
using Model.Interfaces;
using Model.Reports;
using Model.Settings;
using Model.Stats;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using ProtoBuf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        private readonly IDataService dataService;

        private readonly IPlayerStatisticRepository playerStatisticRepository;

        private readonly IEventAggregator eventAggregator;

        private readonly IImporterSessionCacheService importerSessionCacheService;

        private readonly IImporterService importerService;

        private readonly IHudTransmitter hudTransmitter;

        private readonly IFilterModelManagerService filterModelManager;

        private readonly IReportStatusService reportStatusService;

        private bool isAdvancedLoggingEnabled = true;

        private const int ImportFileUpdateDelay = 750;

        private static readonly object playerAddedLock = new object();

        #endregion

        #region Constructor

        internal MainWindowViewModel()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
            reportStatusService = ServiceLocator.Current.GetInstance<IReportStatusService>();

            importerSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();
            importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            importerService.ImportingStopped += OnImportingStopped;

            hudTransmitter = ServiceLocator.Current.GetInstance<IHudTransmitter>();
            filterModelManager = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<RequestEquityCalculatorEvent>().Subscribe(ShowCalculateEquityView);
            eventAggregator.GetEvent<DataImportedEvent>().Subscribe(OnDataImported, ThreadOption.BackgroundThread, false);
            eventAggregator.GetEvent<PlayersAddedEvent>().Subscribe(OnPlayersAdded, ThreadOption.BackgroundThread, false);
            eventAggregator.GetEvent<SettingsUpdatedEvent>().Subscribe(HandleSettingsChangedEvent);
            eventAggregator.GetEvent<UpdateViewRequestedEvent>().Subscribe(UpdateCurrentView);
            eventAggregator.GetEvent<MainNotificationEvent>().Subscribe(RaiseNotification);
            eventAggregator.GetEvent<PokerStarsDetectedEvent>().Subscribe(OnPokerStarsDetected);
            eventAggregator.GetEvent<LoadDataRequestedEvent>().Subscribe(arg => Load());
            eventAggregator.GetEvent<PreImportedDataEvent>().Subscribe(OnPreDataImported, ThreadOption.BackgroundThread, false);
            eventAggregator.GetEvent<TableClosedEvent>().Subscribe(OnTableClosed, ThreadOption.BackgroundThread, false);

            InitializeFilters();
            InitializeData();
            InitializeBindings();

            HudViewModel = new HudViewModel();

            PokerStarsDetectorSingletonService.Instance.Start();

            ConfigureResolutionDependentProperties();

            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.IsAPIEnabled)
            {
                var apiHost = ServiceLocator.Current.GetInstance<IAPIHost>();
                apiHost.StartAPIService();
            }
        }

        private void InitializeData()
        {
            ReportGadgetViewModel = new ReportGadgetViewModel();

            SwitchViewModel(EnumViewModelType.DashboardViewModel);

            StorageModel.StatisticCollection = new RangeObservableCollection<Playerstatistic>();
            StorageModel.PlayerCollection = new ObservableCollection<IPlayer>(dataService.GetPlayersList());
            StorageModel.PlayerCollection.AddRange(dataService.GetAliasesList());
            StorageModel.PropertyChanged += OnStorageModelPropertyChanged;

            ProgressViewModel = new ProgressViewModel();

            CalendarFrom = DateTime.Now;
            CalendarTo = DateTime.Now;

            RadDropDownButtonFilterIsOpen = false;
            RadDropDownButtonFilterKeepOpen = true;

            StorageModel.TryLoadActivePlayer(dataService.GetActivePlayer(), loadHeroIfMissing: true);
        }

        private void InitializeBindings()
        {
            RadioGroupTab_CommandClick = new RelayCommand(new Action<object>(RadioGroupTab_OnClick));

            MenuItemPopupFilter_CommandClick = new RelayCommand(new Action<object>(MenuItemPopupFilter_OnClick));

            ImportFromFileCommand = new DelegateCommand(x => ImportFromFile(), x => !isManualImportingRunning);
            ImportFromDirectoryCommand = new DelegateCommand(x => ImportFromDirectory(), x => !isManualImportingRunning);
            SupportCommand = new RelayCommand(ShowSupportView);
            StartHudCommand = new DelegateCommand(x => StartHud(), x => !IsHudRunning);
            StopHudCommand = new DelegateCommand(x => StopHud(), x => IsHudRunning);
            HideEquityCalculatorCommand = new RelayCommand(HideEquityCalculator);
            SettingsCommand = new RelayCommand(OpenSettingsMenu);
            AliasMenuCommand = new RelayCommand(OpenAliasMenu);
            UpgradeCommand = new RelayCommand(Upgrade);
            PurchaseCommand = new RelayCommand(Purchase);

            PopupSettingsRequest = new InteractionRequest<PopupContainerSettingsViewModelNotification>();
            PopupFiltersRequest = new InteractionRequest<PopupContainerFiltersViewModelNotification>();
            AliasViewRequest = new InteractionRequest<INotification>();
            PopupSupportRequest = new InteractionRequest<INotification>();
            RegistrationViewRequest = new InteractionRequest<INotification>();
            NotificationRequest = new InteractionRequest<INotification>();
            UpdateViewRequest = new InteractionRequest<INotification>();
            NotificationRequest = new InteractionRequest<INotification>();
            SitesSetupViewRequest = new InteractionRequest<INotification>();
            IncorrectlyConfiguredSitesViewRequest = new InteractionRequest<INotification>();

            SortedPlayers = (CollectionView)CollectionViewSource.GetDefaultView(StorageModel.PlayerCollection);
            SortedPlayers.SortDescriptions.Add(new SortDescription(nameof(IPlayer.DecodedName), ListSortDirection.Ascending));
            SortedPlayers.SortDescriptions.Add(new SortDescription(nameof(IPlayer.PokerSite), ListSortDirection.Ascending));
        }

        private void InitializeFilters()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            if (settings.GeneralSettings.IsSaveFiltersOnExit)
            {
                eventAggregator.GetEvent<LoadDefaultFilterRequestedEvent>().Publish(new LoadDefaultFilterRequestedEventArgs());
            }
        }

        private void ConfigureResolutionDependentProperties()
        {
            var screenResolution = Utils.GetScreenResolution();

            if (screenResolution.Width < ResolutionSettings.HighResolutionWidth)
            {
                IsLowResolutionMode = true;
                WindowMinWidth = ResolutionSettings.LowResolutionWidth;
                WindowWidth = ResolutionSettings.LowResolutionWidth;
            }
            else
            {
                WindowMinWidth = ResolutionSettings.HighResolutionWidth;
                WindowWidth = ResolutionSettings.HighResolutionWidth;
            }
        }

        #endregion

        #region Public methods 

        internal void Load()
        {
            var player = StorageModel.PlayerSelectedItem;

            List<Playerstatistic> statistics = new List<Playerstatistic>();

            if (player is PlayerCollectionItem)
            {
                statistics.AddRange(playerStatisticRepository
                    .GetAllPlayerStatistic((StorageModel.PlayerSelectedItem?.Name ?? string.Empty),
                        (short)(StorageModel.PlayerSelectedItem?.PokerSite ?? EnumPokerSites.Unknown)));
            }
            else if (player is AliasCollectionItem)
            {
                (player as AliasCollectionItem).PlayersInAlias
                    .ForEach(pl => statistics.AddRange(playerStatisticRepository
                        .GetAllPlayerStatistic((pl?.Name ?? string.Empty),
                            (short)(pl?.PokerSite ?? EnumPokerSites.Unknown))));
            }

            AddHandTags(statistics);

            if (statistics != null || statistics.Any())
            {
                StorageModel.StatisticCollection.Reset(statistics);
            }
            else
            {
                StorageModel.StatisticCollection.Clear();
            }

            CreatePositionReport();

            UpdateCurrentView(true);
        }

        internal void UpdateHeader()
        {
            RaisePropertyChanged(nameof(AppStartupHeader));
        }

        internal async void RebuildStats()
        {
            IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    LogProvider.Log.Info("Executing statistics rebuild");

                    ProgressViewModel.Progress.Report(new LocalizableString("Progress_RebuildingStatistics"));

                    var playerStatisticReImporter = ServiceLocator.Current.GetInstance<IPlayerStatisticReImporter>();
                    playerStatisticReImporter.InitializeProgress(ProgressViewModel.Progress);
                    playerStatisticReImporter.ReImport();

                    LogProvider.Log.Info("Statistics rebuild has been completed.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Statistics rebuilding failed.", e);
                }
            });

            Load();

            ProgressViewModel.IsActive = false;
            ProgressViewModel.Reset();
            IsEnabled = true;
        }

        internal async void RecoverStats()
        {
            IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    ProgressViewModel.Progress.Report(new LocalizableString("Progress_RecoveringStatistics"));

                    LogProvider.Log.Info("Executing statistics recovering");

                    var playerStatisticReImporter = ServiceLocator.Current.GetInstance<IPlayerStatisticReImporter>();
                    playerStatisticReImporter.Recover();

                    LogProvider.Log.Info("Statistics recovering has been completed.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Statistics recovering failed.", e);
                }
            });

            Load();

            ProgressViewModel.IsActive = false;
            ProgressViewModel.Reset();
            IsEnabled = true;
        }

        internal async void VacuumDatabase()
        {
            IsEnabled = false;

            await Task.Run(() =>
            {
                try
                {
                    ProgressViewModel.Progress.Report(new LocalizableString("Progress_VacuumingDatabase"));

                    LogProvider.Log.Info("Vacuuming database");

                    dataService.VacuumDatabase();

                    LogProvider.Log.Info("Database has been vacuumed.");

                    Load();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Database vacuuming failed.", e);
                }
            });

            ProgressViewModel.IsActive = false;
            ProgressViewModel.Reset();
            IsEnabled = true;
        }

        internal void ShowUpdate()
        {
            var updateViewModel = new UpdateViewModel(App.UpdateApplicationInfo);
            UpdateViewRequest?.Raise(updateViewModel);
        }

        #endregion

        #region Infrastructure

        private void ShowCalculateEquityView(RequestEquityCalculatorEventArgs obj)
        {
            IsShowEquityCalculator = true;
        }

        private void HideEquityCalculator(object obj)
        {
            IsShowEquityCalculator = false;
        }

        private async void StartHud()
        {
            LogProvider.Log.Info(string.Format("Memory before starting auto import: {0:N0}", GC.GetTotalMemory(false)));

            IsHudRunning = true;

            await hudTransmitter.InitializeAsync();

            importerSessionCacheService.Begin();
            importerService.StartImport();

            RefreshCommandsCanExecute();
        }

        private void StopHud()
        {
            importerService.StopImport();
            IsHudRunning = false;
        }

        private void OnImportingStopped(object sender, EventArgs e)
        {
            try
            {
                hudTransmitter.Dispose();

                importerSessionCacheService.End();

                System.Windows.Application.Current?.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        // update data after hud is stopped
                        CreatePositionReport();
                        UpdateCurrentView(true);
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, "Reports has not been updated after HUD stopped.", ex);
                    }
                });

                GC.Collect();
                LogProvider.Log.Info(string.Format("Memory after stopping auto import: {0:N0}", GC.GetTotalMemory(false)));
            }
            finally
            {
                try
                {
                    System.Windows.Application.Current?.Dispatcher.Invoke(() => RefreshCommandsCanExecute());
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        private void HandleSettingsChangedEvent(SettingsUpdatedEventArgs args)
        {
            if (args.IsUpdatePlayersCollection)
            {
                StorageModel.StatisticCollection = new RangeObservableCollection<Playerstatistic>();
                StorageModel.PlayerCollection = new ObservableCollection<IPlayer>(dataService.GetPlayersList());
                StorageModel.PlayerCollection.AddRange(dataService.GetAliasesList());

                StorageModel.TryLoadActivePlayer(dataService.GetActivePlayer(), loadHeroIfMissing: true);
            }

            if (CurrentViewModel.ViewModelType == EnumViewModelType.HudViewModel)
            {
                HudViewModel.RefreshHudTable();
            }
        }

        private void UpdateCurrentView(UpdateViewRequestedEventArgs args)
        {
            if (args != null && args.IsUpdateReportRequested)
            {
                UpdateCurrentView(true);
                ReportGadgetViewModel?.UpdateReport();
                return;
            }

            UpdateCurrentView(false);
        }

        private void UpdateCurrentView(bool forceUpdate)
        {
            if (CurrentViewModel == null)
            {
                return;
            }

            if (forceUpdate)
            {
                reportStatusService.CashUpdated = true;
                reportStatusService.TournamentUpdated = true;
            }

            if (CurrentViewModel == DashboardViewModel)
            {
                DashboardViewModel.Update();
            }
            else if (CurrentViewModel == TournamentViewModel)
            {
                TournamentViewModel.Update();
            }
        }

        private void OnTableClosed(TableClosedEventArgs e)
        {
            try
            {
                if (e == null || e.Handle == 0)
                {
                    return;
                }

                hudTransmitter.CloseTable(e.Handle);

                if (isAdvancedLoggingEnabled)
                {
                    LogProvider.Log.Info(this, $"Close table command has been sent to HUD [handle={e.Handle}]");
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Close table command could not be sent to HUD.", ex);
            }
        }

        private void OnPreDataImported(PreImportedDataEventArgs e)
        {
            try
            {
                if (e == null || e.GameInfo == null || e.GameInfo.WindowHandle == 0)
                {
                    return;
                }

                var gameInfo = e.GameInfo;

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                var activeLayout = hudLayoutsService.GetActiveLayout(gameInfo.PokerSite, gameInfo.TableType, gameInfo.EnumGameType);

                if (activeLayout == null)
                {
                    return;
                }

                var ht = new HudLayout
                {
                    WindowId = gameInfo.WindowHandle,
                    TableType = gameInfo.TableType,
                    PokerSite = gameInfo.PokerSite,
                    GameNumber = gameInfo.GameNumber,
                    GameType = gameInfo.EnumGameType,
                    LayoutName = activeLayout.Name,
                    AvailableLayouts = new List<string>(),
                    PreloadMode = true,
                    PreloadText = e.LoadingText
                };

                byte[] serialized;

                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, ht);
                    serialized = msTestString.ToArray();
                }

                hudTransmitter.Send(serialized);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Preloading data could not be sent to HUD.", ex);
            }
        }

        private void OnDataImported(DataImportedEventArgs e)
        {
            try
            {
                if (e == null || e.GameInfo == null)
                {
                    LogProvider.Log.Error(this, "Fatal error: data can not be imported");
                    return;
                }

                // if no handle available then we don't need to do anything with this data, because hud won't show up
                if (e.GameInfo.WindowHandle == 0)
                {
                    if (e.GameInfo.GameFormat != GameFormat.Zone)
                    {
                        LogProvider.Log.Warn($"No window found for hand #{e.GameInfo.GameNumber}");
                    }

                    return;
                }

                var players = e.Players.Where(x => !string.IsNullOrEmpty(x.PlayerName)).ToList();
                var gameInfo = e.GameInfo;
                var site = e.GameInfo.PokerSite;

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
                var treatAsService = ServiceLocator.Current.GetInstance<ITreatAsService>();

                var treatedTableType = treatAsService.GetTableType(new IntPtr(e.GameInfo.WindowHandle));

                if (!treatedTableType.HasValue)
                {
                    treatedTableType = e.GameInfo.TableType;
                }

                var activeLayout = hudLayoutsService.GetActiveLayout(e.GameInfo.PokerSite, treatedTableType.Value, e.GameInfo.EnumGameType);

                if (activeLayout == null)
                {
                    LogProvider.Log.Error(this, $"Layout has not been found for {e.GameInfo.PokerSite}, {e.GameInfo.TableType}, {e.GameInfo.EnumGameType}");
                    return;
                }

                var playersCacheInfo = gameInfo.GetPlayersCacheInfo();

                // update cache if even we don't need to build HUD
                if (playersCacheInfo != null)
                {
                    playersCacheInfo.ForEach(x => x.GameFormat = gameInfo.GameFormat);

                    var filter = activeLayout.Filter != null ?
                        activeLayout.Filter.Clone() :
                        new HudLayoutFilter();

                    if (!e.DoNotUpdateHud)
                    {
                        playersCacheInfo = playersCacheInfo.Where(x => x.GameNumber == e.GameNumber).ToList();
                    }

                    importerSessionCacheService.AddOrUpdatePlayersStats(playersCacheInfo, gameInfo.Session, filter);
                }

                if (e.DoNotUpdateHud)
                {
                    return;
                }

                if (gameInfo.PokerSite != EnumPokerSites.PokerStars && activeLayout != null)
                {
                    var stickers = activeLayout.HudBumperStickerTypes?.ToDictionary(x => x.Name, x => x.FilterPredicate.Compile());

                    if (stickers.Count > 0)
                    {
                        var playersStickersCacheData = (from player in players
                                                        let playerItem = new PlayerCollectionItem
                                                        {
                                                            PlayerId = player.PlayerId,
                                                            Name = player.PlayerName,
                                                            PokerSite = site
                                                        }
                                                        let lastHandStatistic = importerSessionCacheService.GetPlayersLastHandStatistics(gameInfo.Session, playerItem)
                                                        where lastHandStatistic != null
                                                        select new PlayerStickersCacheData
                                                        {
                                                            Session = gameInfo.Session,
                                                            Player = playerItem,
                                                            Layout = activeLayout.Name,
                                                            Statistic = lastHandStatistic,
                                                            StickerFilters = stickers,
                                                            IsHero = importerSessionCacheService.GetPlayerStats(gameInfo.Session, playerItem)?.IsHero ?? false
                                                        }).ToArray();

                        importerSessionCacheService.AddOrUpdatePlayerStickerStats(playersStickersCacheData, gameInfo.Session);
                    }
                }

                // stats involved into player types and bumper stickers
                var nonToolLayoutStats = activeLayout
                    .HudPlayerTypes
                    .SelectMany(x => x.Stats)
                    .Select(x => x.Stat)
                    .Concat(activeLayout
                        .HudBumperStickerTypes
                        .SelectMany(x => x.Stats)
                        .Select(x => x.Stat))
                    .Concat(new[] { Stat.TotalHands })
                    .Distinct()
                    .ToArray();

                var availableLayouts = hudLayoutsService.GetAvailableLayouts(e.GameInfo.PokerSite, e.GameInfo.TableType, e.GameInfo.EnumGameType);

                var ht = new HudLayout
                {
                    WindowId = gameInfo.WindowHandle,
                    TableType = gameInfo.TableType,
                    PokerSite = gameInfo.PokerSite,
                    GameNumber = gameInfo.GameNumber,
                    GameType = gameInfo.EnumGameType,
                    LayoutName = activeLayout.Name,
                    AvailableLayouts = availableLayouts
                };

                var trackConditionsMeterData = new HudTrackConditionsMeterData();

                foreach (var player in players)
                {
                    var playerCollectionItem = new PlayerCollectionItem
                    {
                        PlayerId = player.PlayerId,
                        Name = player.PlayerName,
                        PokerSite = site
                    };

                    var playerHudContent = new PlayerHudContent
                    {
                        Name = player.PlayerName,
                        SeatNumber = player.SeatNumber
                    };

                    // cache service must return light indicators or hud light indicators
                    var sessionCacheStatistic = importerSessionCacheService.GetPlayerStats(gameInfo.Session, playerCollectionItem);
                    var lastHandStatistic = importerSessionCacheService.GetPlayersLastHandStatistics(gameInfo.Session, playerCollectionItem);

                    var playerData = sessionCacheStatistic.PlayerData;
                    var sessionData = sessionCacheStatistic.SessionPlayerData;

                    // need to do that for track meter tool
                    #region track condition meter

                    trackConditionsMeterData.VPIP += sessionData.VPIP;
                    trackConditionsMeterData.ThreeBet += sessionData.ThreeBet;
                    trackConditionsMeterData.BigBlind = sessionData.Source.BigBlind;

                    if (sessionData.TotalHands > trackConditionsMeterData.TotalHands)
                    {
                        trackConditionsMeterData.TotalHands = sessionData.TotalHands;

                        if (e.GameInfo.GameFormat == GameFormat.MTT || e.GameInfo.GameFormat == GameFormat.SnG)
                        {
                            trackConditionsMeterData.TotalPotSize = sessionData.Source.TotalPotInBB;
                        }
                        else
                        {
                            trackConditionsMeterData.TotalPotSize = sessionData.Source.TotalPot;
                        }
                    }

                    #endregion

                    var hudElementCreator = ServiceLocator.Current.GetInstance<IHudElementViewModelCreator>();

                    var hudElementCreationInfo = new HudElementViewModelCreationInfo
                    {
                        GameType = gameInfo.EnumGameType,
                        HudLayoutInfo = activeLayout,
                        PokerSite = gameInfo.PokerSite,
                        SeatNumber = player.SeatNumber
                    };

                    playerHudContent.HudElement = hudElementCreator.Create(hudElementCreationInfo);

                    if (playerHudContent.HudElement == null)
                    {
                        continue;
                    }

                    playerHudContent.HudElement.TiltMeter = sessionData.TiltMeter;
                    playerHudContent.HudElement.PlayerId = player.PlayerId;
                    playerHudContent.HudElement.PlayerName = !string.IsNullOrWhiteSpace(player.PlayerNick) ?
                        $"{player.PlayerName} / {player.PlayerNick}" : player.PlayerName;
                    playerHudContent.HudElement.PokerSiteId = (short)site;
                    playerHudContent.HudElement.TotalHands = playerData.TotalHands;

                    var playerNotes = dataService.GetPlayerNotes(player.PlayerId);
                    playerHudContent.HudElement.NoteToolTip = NoteBuilder.BuildNote(playerNotes);
                    playerHudContent.HudElement.IsXRayNoteVisible = playerNotes.Any(x => x.IsAutoNote);

                    // configure data for graph tool
                    var sessionStats = sessionData.StatsSessionCollection;

                    var graphTools = playerHudContent.HudElement.Tools.OfType<HudGraphViewModel>().ToArray();

                    foreach (var graphTool in graphTools)
                    {
                        if (graphTool.MainStat == null || sessionStats == null || !sessionStats.ContainsKey(graphTool.MainStat.Stat))
                        {
                            graphTool.StatSessionCollection = new ReactiveList<decimal>();
                            continue;
                        }

                        graphTool.StatSessionCollection = new ReactiveList<decimal>(sessionStats[graphTool.MainStat.Stat]);
                    }

                    var heatMapTools = playerHudContent.HudElement.Tools.OfType<HudHeatMapViewModel>()
                        .Concat(playerHudContent.HudElement.Tools.OfType<HudGaugeIndicatorViewModel>()
                            .SelectMany(x => x.GroupedStats)
                            .SelectMany(x => x.Stats)
                            .Where(x => x.HeatMapViewModel != null)
                            .Select(x => x.HeatMapViewModel))
                        .ToArray();

                    heatMapTools.ForEach(x =>
                    {
                        var heatMapKey = playerData.HeatMaps.Keys
                            .ToArray()
                            .FirstOrDefault(p => p.Stat == x.BaseStat.Stat);

                        x.HeatMap = playerData.HeatMaps[heatMapKey];
                    });

                    var gaugeIndicatorTools = playerHudContent.HudElement.Tools.OfType<HudGaugeIndicatorViewModel>().ToArray();

                    var cardsCollection = sessionData.CardsList;
                    playerHudContent.HudElement.CardsCollection = cardsCollection == null
                        ? new ObservableCollection<string>()
                        : new ObservableCollection<string>(cardsCollection);

                    playerHudContent.HudElement.SessionMoneyWonCollection = sessionStats != null && sessionStats.ContainsKey(Stat.NetWon) ?
                        new ObservableCollection<decimal>(sessionStats[Stat.NetWon]) :
                        new ObservableCollection<decimal>();

                    var activeLayoutHudStats = playerHudContent.HudElement.ToolsStatInfoCollection
                        .Concat(heatMapTools.Select(x => x.BaseStat))
                        .Concat(gaugeIndicatorTools.Select(x => x.BaseStat))
                        .ToList();

                    var extraStats = (from nonToolLayoutStat in nonToolLayoutStats
                                      join activateLayoutHudStat in activeLayoutHudStats on nonToolLayoutStat equals activateLayoutHudStat.Stat into grouped
                                      from extraStat in grouped.DefaultIfEmpty()
                                      where extraStat == null
                                      select new StatInfo
                                      {
                                          Stat = nonToolLayoutStat
                                      }).ToArray();

                    activeLayoutHudStats.AddRange(extraStats);

                    StatsProvider.UpdateStats(activeLayoutHudStats);

                    if (gameInfo.PokerSite == EnumPokerSites.PokerStars)
                    {
                        // remove prohibited for PS stats.
                        activeLayoutHudStats = activeLayoutHudStats.Where(x => x.Stat != Stat.FlopCBetMonotone && x.Stat != Stat.FlopCBetRag).ToList();
                        activeLayoutHudStats.ForEach(x => x.SettingsAppearanceValueRangeCollection = new ObservableCollection<StatInfoOptionValueRange>(
                                x.SettingsAppearanceValueRangeCollection.Skip(Math.Max(0, x.SettingsAppearanceValueRangeCollection.Count() - 3))));
                    }

                    foreach (var statInfo in activeLayoutHudStats)
                    {
                        var propertyName = StatsProvider.GetStatProperyName(statInfo.Stat);

                        if (!string.IsNullOrEmpty(propertyName))
                        {
                            if (playerData.TotalHands < statInfo.MinSample)
                            {
                                statInfo.IsNotVisible = true;
                            }

                            statInfo.AssignStatInfoValues(playerData, propertyName);
                        }
                        else if (!(statInfo is StatInfoBreak) && statInfo.Stat != Stat.PlayerInfoIcon)
                        {
                            continue;
                        }
                    }

                    playerHudContent.HudElement.StatInfoCollection = activeLayoutHudStats;

                    if (gameInfo.PokerSite != EnumPokerSites.PokerStars && lastHandStatistic != null && activeLayout != null)
                    {
                        hudLayoutsService.SetStickers(playerHudContent.HudElement,
                            importerSessionCacheService.GetPlayersStickersStatistics(gameInfo.Session, playerCollectionItem),
                            activeLayout);
                    }

                    ht.ListHUDPlayer.Add(playerHudContent);
                }

                if (gameInfo.PokerSite != EnumPokerSites.PokerStars)
                {
                    var hudElements = ht.ListHUDPlayer.Select(x => x.HudElement).ToArray();
                    hudLayoutsService.SetPlayerTypeIcon(hudElements, activeLayout);
                }

                decimal getDevisionResult(decimal x, decimal y)
                {
                    return y != 0 ? x / y : 0;
                }

                var trackConditionsInfo = new HudTrackConditionsViewModelInfo
                {
                    AveragePot = getDevisionResult(trackConditionsMeterData.TotalPotSize, trackConditionsMeterData.TotalHands),
                    VPIP = getDevisionResult(trackConditionsMeterData.VPIP, players.Count),
                    ThreeBet = getDevisionResult(trackConditionsMeterData.ThreeBet, players.Count),
                    TableType = gameInfo.TableType,
                    BuyInNL = Utils.ConvertBigBlindToNL(trackConditionsMeterData.BigBlind),
                    Position = activeLayout.TrackMeterPositions?
                        .FirstOrDefault(x => x.GameType == gameInfo.EnumGameType && x.PokerSite == gameInfo.PokerSite)?
                        .HudPositions?.FirstOrDefault()?
                        .Position
                };

                ht.HudTrackConditionsMeter = trackConditionsInfo;

                byte[] serialized;

                using (var msTestString = new MemoryStream())
                {
                    Serializer.Serialize(msTestString, ht);
                    serialized = msTestString.ToArray();
                }

                if (isAdvancedLoggingEnabled)
                {
                    LogProvider.Log.Info(this, $"Sending {serialized.Length} bytes to HUD [handle={ht.WindowId}, title={WinApi.GetWindowText(new IntPtr(ht.WindowId))}]");
                }

                hudTransmitter.Send(serialized);

                if (isAdvancedLoggingEnabled)
                {
                    LogProvider.Log.Info(this, $"Data has been sent to HUD [handle={ht.WindowId}]");
                }

                if (string.IsNullOrEmpty(StorageModel.PlayerSelectedItem?.Name) ||
                    string.IsNullOrEmpty(StorageModel.PlayerSelectedItem?.DecodedName) ||
                    StorageModel.PlayerSelectedItem?.PokerSite == EnumPokerSites.Unknown)
                {
                    if (e.Hero == null)
                    {
                        return;
                    }

                    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        StorageModel.PlayerSelectedItem = StorageModel.PlayerCollection
                            .FirstOrDefault(x => x.Name == e.Hero.PlayerName && x.PokerSite == e.GameInfo.PokerSite);
                    });
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Data could not be sent to HUD.", ex);
            }
        }

        private void OnPlayersAdded(PlayersAddedEventArgs e)
        {
            if (e == null || e.AddedPlayers == null || e.AddedPlayers.Length == 0)
            {
                return;
            }

            lock (playerAddedLock)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        foreach (var player in e.AddedPlayers)
                        {
                            StorageModel.PlayerCollection.Add(player);
                        }

                        if (!string.IsNullOrEmpty(StorageModel.PlayerSelectedItem?.Name))
                        {
                            return;
                        }

                        StorageModel.TryLoadHeroPlayer();
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, "Could not update player list.", ex);
                    }
                });
            }
        }

        private async void ImportFromFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var filesToImport = openFileDialog.FileNames.Select(x => new FileInfo(x)).ToArray();

            var fileImporter = ServiceLocator.Current.GetInstance<IFileImporter>();

            IsManualImportingRunning = true;

            await Task.Run(() =>
            {
                fileImporter.Import(filesToImport, ProgressViewModel.Progress);
                Task.Delay(ImportFileUpdateDelay).Wait();
            });

            UpdateCurrentView(true);
            CreatePositionReport();

            ProgressViewModel.IsActive = false;
            IsManualImportingRunning = false;
            ProgressViewModel.Reset();
        }

        private async void ImportFromDirectory()
        {
            var folderDialog = new FolderBrowserDialog();

            var result = folderDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            var directory = new DirectoryInfo(folderDialog.SelectedPath);

            var fileImporter = ServiceLocator.Current.GetInstance<IFileImporter>();

            IsManualImportingRunning = true;

            await Task.Run(() =>
            {
                fileImporter.Import(directory, ProgressViewModel.Progress);
                Task.Delay(ImportFileUpdateDelay).Wait();
            });

            UpdateCurrentView(true);
            CreatePositionReport();

            ProgressViewModel.IsActive = false;
            IsManualImportingRunning = false;
            ProgressViewModel.Reset();
        }

        private void RefreshCommandsCanExecute()
        {
            (ImportFromFileCommand as DelegateCommand)?.InvalidateCanExecute();
            (ImportFromDirectoryCommand as DelegateCommand)?.InvalidateCanExecute();
            (StartHudCommand as DelegateCommand)?.InvalidateCanExecute();
            (StopHudCommand as DelegateCommand)?.InvalidateCanExecute();
        }

        private void ShowSupportView()
        {
            PopupSupportRequest.Raise(new PopupActionNotification() { Title = "DriveHUD Support" });
        }

        private void CreatePositionReport()
        {
            ReportGadgetViewModel?.UpdateReport();
        }

        private void SwitchViewModel(EnumViewModelType viewModelType)
        {
            IsShowEquityCalculator = false;

            if (viewModelType == EnumViewModelType.DashboardViewModel)
            {
                filterModelManager.FilterType = EnumFilterType.Cash;

                // initialize filters            
                eventAggregator.GetEvent<UpdateFilterRequestEvent>()
                    .Publish(new UpdateFilterRequestEventArgs());

                if (DashboardViewModel == null)
                {
                    DashboardViewModel = new DashboardViewModel();
                }

                CurrentViewModel = DashboardViewModel;

                ReportGadgetViewModel.IsShowTournamentData = false;

                DashboardViewModel.Update();
                ReportGadgetViewModel.UpdateReport();
            }
            else if (viewModelType == EnumViewModelType.TournamentViewModel)
            {
                filterModelManager.FilterType = EnumFilterType.Tournament;

                // initialize filters            
                eventAggregator.GetEvent<UpdateFilterRequestEvent>()
                    .Publish(new UpdateFilterRequestEventArgs());

                if (TournamentViewModel == null)
                {
                    TournamentViewModel = new TournamentViewModel();
                }

                CurrentViewModel = TournamentViewModel;
                ReportGadgetViewModel.IsShowTournamentData = true;

                TournamentViewModel.Update();
                ReportGadgetViewModel.UpdateReport();
            }
            else if (viewModelType == EnumViewModelType.HudViewModel)
            {
                if (HudViewModel == null)
                {
                    HudViewModel = new HudViewModel();
                }

                CurrentViewModel = HudViewModel;
            }
            else if (viewModelType == EnumViewModelType.AppsViewModel)
            {
                if (AppsViewModel == null)
                {
                    AppsViewModel = new AppsViewModel();
                }

                CurrentViewModel = AppsViewModel;
            }
        }

        private void AddHandTags(IList<Playerstatistic> statistics)
        {
            if (StorageModel.PlayerSelectedItem == null || StorageModel.PlayerSelectedItem.PokerSites == null)
            {
                return;
            }

            var notes = (from pokerSite in StorageModel.PlayerSelectedItem.PokerSites
                         from note in dataService.GetHandNotes((short)pokerSite)
                         select note).ToArray();

            var statisticsToUpdate = (from note in notes
                                      join statistic in statistics on note.Gamenumber equals statistic.GameNumber
                                      select new { Note = note, Statistic = statistic }).ToArray();

            foreach (var statisticForUpdate in statisticsToUpdate)
            {
                statisticForUpdate.Statistic.HandNote = statisticForUpdate.Note;
            }
        }

        private void OpenSettingsMenu(object obj)
        {
            PubSubMessage pubSubMessage = new PubSubMessage();
            if (obj?.ToString() == "Preferred Seating")
                pubSubMessage.Parameter = "Preferred Seating";

            PopupSettingsRequest_Execute(pubSubMessage);
        }

        private void OpenAliasMenu()
        {
            var model = new AliasViewModel();
            AliasViewRequest.Raise(model);
        }

        private void Upgrade()
        {
            var registrationViewModel = new RegistrationViewModel(true);
            RegistrationViewRequest.Raise(registrationViewModel);

            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

            IsTrial = licenseService.IsTrial;
            IsUpgradable = licenseService.IsUpgradable;
            UpdateHeader();
        }

        private void Purchase()
        {
            Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("SystemSettings_PricingLink"));
        }

        #endregion

        #region Properties

        private DashboardViewModel _dashboardViewModel;
        private TournamentViewModel _tournamentViewModel;
        private HudViewModel _hudViewModel;
        private AppsViewModel _appsViewModel;
        private ReportGadgetViewModel _reportGadgetViewModel;
        private bool _isShowEquityCalculator;

        private IMainTabViewModel currentViewModel;

        private bool _reportGadgetView_IsVisible;

        private ObservableCollection<FilterDropDownModel> _filterTypeList = FilterDropDownModel.GetDefaultFilterTypeList();

        public ObservableCollection<FilterDropDownModel> FilterTypeList
        {
            get { return _filterTypeList; }
            set { _filterTypeList = value; }
        }

        public IProgressViewModel ProgressViewModel { get; private set; }

        public double AppStartupHeight
        {
            get { return SystemParameters.WorkArea.Height; }
        }

        public string AppStartupHeader
        {
            get
            {
                var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
                IEnumerable<string> licenseStrings;

                if (licenseService.LicenseInfos.Any(x => x.IsRegistered && !x.IsTrial))
                {
                    licenseStrings = licenseService.LicenseInfos.Where(x => x.IsRegistered && !x.IsTrial).Select(x => x.License.Edition);
                }
                else
                {
                    licenseStrings = licenseService.LicenseInfos.Where(x => x.IsRegistered).Select(x => x.License.Edition);
                }

                return string.Format(CommonResourceManager.Instance.GetResourceString("Common_ApplicationTitle"), App.Version, string.Join(" & ", licenseStrings));
            }
        }

        private bool _radDropDownButtonFilterKeepOpen { get; set; }

        public bool RadDropDownButtonFilterKeepOpen
        {
            get { return _radDropDownButtonFilterKeepOpen; }
            set { _radDropDownButtonFilterKeepOpen = value; RaisePropertyChanged(); }
        }

        private bool _radDropDownButtonFilterIsOpen { get; set; }

        public bool RadDropDownButtonFilterIsOpen
        {
            get { return _radDropDownButtonFilterIsOpen; }
            set { _radDropDownButtonFilterIsOpen = value; RaisePropertyChanged(); }
        }

        private DateTime _calendarFrom { get; set; }

        public DateTime CalendarFrom
        {
            get { return _calendarFrom; }
            set { _calendarFrom = value; RaisePropertyChanged(); }
        }

        private DateTime _calendarTo { get; set; }

        public DateTime CalendarTo
        {
            get { return _calendarTo; }
            set { _calendarTo = value; RaisePropertyChanged(); }
        }

        public Version Version
        {
            get
            {
                return App.Version;
            }
        }

        private bool isTrial;

        public bool IsTrial
        {
            get
            {
                return isTrial;
            }
            set
            {
                isTrial = value;
                RaisePropertyChanged();
            }
        }

        private bool isUpgradable;

        public bool IsUpgradable
        {
            get
            {
                return isUpgradable;
            }
            set
            {
                isUpgradable = value;
                RaisePropertyChanged();
            }
        }

        private bool isManualImportingRunning = false;

        public bool IsManualImportingRunning
        {
            get
            {
                return isManualImportingRunning;
            }
            private set
            {
                isManualImportingRunning = value;
                RaisePropertyChanged();
                RefreshCommandsCanExecute();
            }
        }

        public DashboardViewModel DashboardViewModel
        {
            get { return _dashboardViewModel; }
            set
            {
                _dashboardViewModel = value;
                RaisePropertyChanged();
            }
        }

        public TournamentViewModel TournamentViewModel
        {
            get { return _tournamentViewModel; }
            set
            {
                _tournamentViewModel = value;
                RaisePropertyChanged();
            }
        }

        public HudViewModel HudViewModel
        {
            get { return _hudViewModel; }
            set
            {
                _hudViewModel = value;
                RaisePropertyChanged();
            }
        }

        public AppsViewModel AppsViewModel
        {
            get { return _appsViewModel; }
            set
            {
                SetProperty(ref _appsViewModel, value);
            }
        }

        public ReportGadgetViewModel ReportGadgetViewModel
        {
            get { return _reportGadgetViewModel; }
            set
            {
                _reportGadgetViewModel = value;
                RaisePropertyChanged();
            }
        }

        public IMainTabViewModel CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                if (currentViewModel != null)
                {
                    currentViewModel.IsActive = false;
                }

                SetProperty(ref currentViewModel, value);

                if (currentViewModel != null)
                {
                    currentViewModel.IsActive = true;
                }

                ReportGadgetView_IsVisible = currentViewModel.ViewModelType == EnumViewModelType.DashboardViewModel ||
                    currentViewModel.ViewModelType == EnumViewModelType.TournamentViewModel;
            }
        }

        public bool ReportGadgetView_IsVisible
        {
            get { return _reportGadgetView_IsVisible; }
            set
            {
                _reportGadgetView_IsVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool IsShowEquityCalculator
        {
            get { return _isShowEquityCalculator; }
            set { SetProperty(ref _isShowEquityCalculator, value); }
        }

        private bool isHudRunning;

        public bool IsHudRunning
        {
            get
            {
                return isHudRunning;
            }
            set
            {
                isHudRunning = value;
                RaisePropertyChanged();
            }
        }

        private bool isLowResolutionMode;

        public bool IsLowResolutionMode
        {
            get
            {
                return isLowResolutionMode;
            }
            private set
            {
                if (isLowResolutionMode != value)
                {
                    isLowResolutionMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int windowMinWidth;

        public int WindowMinWidth
        {
            get
            {
                return windowMinWidth;
            }
            private set
            {
                if (windowMinWidth != value)
                {
                    windowMinWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int windowWidth;

        public int WindowWidth
        {
            get
            {
                return windowWidth;
            }
            set
            {
                if (windowWidth != value)
                {
                    windowWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            private set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CollectionView sortedPlayers;

        public CollectionView SortedPlayers
        {
            get
            {
                return sortedPlayers;
            }
            private set
            {
                if (ReferenceEquals(sortedPlayers, value))
                {
                    return;
                }

                sortedPlayers = value;

                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand UpgradeCommand { get; private set; }

        public ICommand PurchaseCommand { get; private set; }

        public ICommand RadioGroupTab_CommandClick { get; set; }

        private void RadioGroupTab_OnClick(object viewModelType)
        {
            SwitchViewModel((EnumViewModelType)viewModelType);
        }

        public ICommand MenuItemPopupFilter_CommandClick { get; set; }

        private void MenuItemPopupFilter_OnClick(object filterType)
        {
            var type = filterType as FilterDropDownModel;

            if (type == null)
            {
                return;
            }

            if (type.FilterType == EnumFilterDropDown.FilterCreate)
            {
                RadDropDownButtonFilterKeepOpen = false;
                RadDropDownButtonFilterIsOpen = false;
                RadDropDownButtonFilterKeepOpen = true;

                var filterTuple = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString()).FilterTupleCollection.FirstOrDefault();
                PopupFiltersRequestExecute(filterTuple);

                return;
            }

            var enumDateFiterStruct = new EnumDateFiterStruct();

            switch (type.FilterType)
            {
                case EnumFilterDropDown.FilterToday:
                    enumDateFiterStruct.EnumDateRange = EnumDateFiterStruct.EnumDateFiter.Today;
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(enumDateFiterStruct));
                    break;
                case EnumFilterDropDown.FilterThisWeek:
                    enumDateFiterStruct.EnumDateRange = EnumDateFiterStruct.EnumDateFiter.ThisWeek;
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(enumDateFiterStruct));
                    break;
                case EnumFilterDropDown.FilterThisMonth:
                    enumDateFiterStruct.EnumDateRange = EnumDateFiterStruct.EnumDateFiter.ThisMonth;
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(enumDateFiterStruct));
                    break;
                case EnumFilterDropDown.FilterLastMonth:
                    enumDateFiterStruct.EnumDateRange = EnumDateFiterStruct.EnumDateFiter.LastMonth;
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(enumDateFiterStruct));
                    break;
                case EnumFilterDropDown.FilterThisYear:
                    enumDateFiterStruct.EnumDateRange = EnumDateFiterStruct.EnumDateFiter.ThisYear;
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(enumDateFiterStruct));
                    break;
                case EnumFilterDropDown.FilterCustomDateRange:
                    enumDateFiterStruct.EnumDateRange = EnumDateFiterStruct.EnumDateFiter.CustomDateRange;
                    enumDateFiterStruct.DateFrom = CalendarFrom;
                    enumDateFiterStruct.DateTo = CalendarTo;

                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(enumDateFiterStruct));

                    RadDropDownButtonFilterKeepOpen = false;
                    RadDropDownButtonFilterIsOpen = false;
                    RadDropDownButtonFilterKeepOpen = true;
                    break;
                case EnumFilterDropDown.FilterAllStats:
                default:
                    eventAggregator.GetEvent<ResetFiltersEvent>().Publish(new ResetFiltersEventArgs());
                    break;
            }
        }

        public ICommand ImportCommand { get; set; }

        public ICommand ImportFromFileCommand { get; set; }

        public ICommand ImportFromDirectoryCommand { get; set; }

        public ICommand SupportCommand { get; set; }

        public ICommand StartHudCommand { get; set; }

        public ICommand StopHudCommand { get; set; }

        public ICommand CalculateEquityCommand { get; set; }

        public ICommand HideEquityCalculatorCommand { get; set; }

        public ICommand SettingsCommand { get; set; }

        public ICommand AliasMenuCommand { get; set; }

        #endregion

        #region Events

        public void MainWindow_PreviewClosed(object sender, WindowPreviewClosedEventArgs e)
        {
            try
            {
                if (importerService.IsStarted)
                {
                    importerService.StopImport();
                }

                hudTransmitter.Dispose();
                importerSessionCacheService.End();

                if (StorageModel.PlayerSelectedItem != null)
                {
                    dataService.SaveActivePlayer(StorageModel.PlayerSelectedItem.Name, (short?)StorageModel.PlayerSelectedItem.PokerSite);
                }

                // flush betonline cash
                var tournamentsCacheService = ServiceLocator.Current.GetInstance<ITournamentsCacheService>();
                tournamentsCacheService.Flush();

                PokerStarsDetectorSingletonService.Instance.Stop();

                var apiHost = ServiceLocator.Current.GetInstance<IAPIHost>();

                if (apiHost != null && apiHost.IsRunning)
                {
                    apiHost.CloseAPIService();
                }

                if (ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.IsSaveFiltersOnExit)
                {
                    eventAggregator.GetEvent<SaveDefaultFilterRequestedEvent>().Publish(new SaveDefaultFilterRequestedEvetnArgs());
                }

                var windowController = ServiceLocator.Current.GetInstance<IWindowController>();
                windowController.CloseAllWindows();

                if (sender is RadWindow mainWindow)
                {
                    var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
                    var settingsModel = settingsService.GetSettings();

                    var displaySettings = settingsModel.GeneralSettings.DisplaySettings;

                    if (displaySettings == null)
                    {
                        displaySettings = new DisplaySettings();
                        settingsModel.GeneralSettings.DisplaySettings = displaySettings;
                    }

                    displaySettings.Left = mainWindow.Left;
                    displaySettings.Top = mainWindow.Top;
                    displaySettings.Width = mainWindow.ActualWidth;
                    displaySettings.Height = mainWindow.ActualHeight;
                    displaySettings.Maximized = mainWindow.WindowState == WindowState.Maximized;

                    settingsService.SaveSettings(settingsModel);
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, ex);
            }
        }

        private void OnStorageModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ReflectionHelper.GetPath<SingletonStorageModel>(p => p.PlayerSelectedItem))
            {
                Load();
            }
        }

        #endregion

        #region InteractionRequest

        public InteractionRequest<PopupContainerSettingsViewModelNotification> PopupSettingsRequest { get; private set; }

        public InteractionRequest<PopupContainerFiltersViewModelNotification> PopupFiltersRequest { get; private set; }

        public InteractionRequest<INotification> PopupSupportRequest { get; private set; }

        public InteractionRequest<INotification> RegistrationViewRequest { get; private set; }

        public InteractionRequest<INotification> AliasViewRequest { get; private set; }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public InteractionRequest<INotification> UpdateViewRequest { get; private set; }

        public InteractionRequest<INotification> SitesSetupViewRequest { get; private set; }

        public InteractionRequest<INotification> IncorrectlyConfiguredSitesViewRequest { get; private set; }

        private void PopupSettingsRequest_Execute(PubSubMessage pubSubMessage)
        {
            var notification = new PopupContainerSettingsViewModelNotification
            {
                Title = "Settings",
                PubSubMessage = pubSubMessage,
                Parameter = pubSubMessage?.Parameter
            };

            PopupSettingsRequest.Raise(notification,
                returned =>
                {
                    if (returned != null && returned.Confirmed)
                    {
                    }
                    else
                    {
                    }
                });
        }

        private void PopupFiltersRequestExecute(FilterTuple filterTuple)
        {
            var notification = new PopupContainerFiltersViewModelNotification
            {
                Title = "Filters",
                FilterTuple = filterTuple
            };

            PopupFiltersRequest.Raise(notification,
                returned => { });
        }

        private void RaiseNotification(MainNotificationEventArgs obj)
        {
            if (obj == null)
            {
                return;
            }

            var confirmation = new PopupActionNotification
            {
                Title = obj.Title,
                Content = obj.Message,
                HyperLinkText = obj.HyperLink
            };

            NotificationRequest.Raise(confirmation);
        }

        private void OnPokerStarsDetected(PokerStarsDetectedEventArgs obj)
        {
            if (obj.IsDetected)
            {
                HideEquityCalculator(null);
                ReportGadgetViewModel.IsEquityCalculatorEnabled = false;
            }
            else
            {
                ReportGadgetViewModel.IsEquityCalculatorEnabled = true;
            }
        }

        private class HudTrackConditionsMeterData
        {
            public decimal VPIP { get; set; }

            public decimal ThreeBet { get; set; }

            public decimal TotalHands { get; set; }

            public decimal TotalPotSize { get; set; }

            public decimal BigBlind { get; set; }
        }

    }

    #endregion
}