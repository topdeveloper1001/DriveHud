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

using DriveHUD.Application.HudServices;
using DriveHUD.Application.Licensing;
using DriveHUD.Application.Models;
using DriveHUD.Application.Services;
using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Common.Wpf.Helpers;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.BetOnline;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Model.Events;
using Model.Filters;
using Model.Interfaces;
using Model.Settings;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Telerik.Windows.Controls;
using DriveHUD.API;
using DriveHUD.Application.Views.Hud;
using DriveHUD.Application.ViewModels.Layouts;

namespace DriveHUD.Application.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        private IDataService dataService;

        private IEventAggregator eventAggregator;

        private IImporterSessionCacheService importerSessionCacheService;

        private IImporterService importerService;

        private IHudTransmitter hudTransmitter;

        private IFilterModelManagerService filterModelManager;

        private IAPIHost apiHost;

        private bool isAdvancedLoggingEnabled = false;

        private bool isSetPlayerIdMessageShown = false;

        #endregion

        #region Constructor

        internal MainWindowViewModel(SynchronizationContext _synchronizationContext)
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();

            importerSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();
            importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            importerService.ImportingStopped += OnImportingStopped;

            hudTransmitter = ServiceLocator.Current.GetInstance<IHudTransmitter>();
            filterModelManager = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());
            synchronizationContext = _synchronizationContext;

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<RequestEquityCalculatorEvent>().Subscribe(ShowCalculateEquityView);
            eventAggregator.GetEvent<DataImportedEvent>().Subscribe(OnDataImported, ThreadOption.BackgroundThread, false);
            eventAggregator.GetEvent<SettingsUpdatedEvent>().Subscribe(HandleSettingsChangedEvent);
            eventAggregator.GetEvent<UpdateViewRequestedEvent>().Subscribe(UpdateCurrentView);
            eventAggregator.GetEvent<MainNotificationEvent>().Subscribe(RaiseNotification);
            eventAggregator.GetEvent<PokerStarsDetectedEvent>().Subscribe(OnPokerStarsDetected);

            InitializeFilters();
            InitializeData();
            InitializeBindings();

            HudViewModel = new HudViewModel();
            filterModelManager.SetFilterType(EnumFilterType.Cash);

            PokerStarsDetectorSingletonService.Instance.Start();

            ConfigureResolutionDependentProperties();

            apiHost = ServiceLocator.Current.GetInstance<IAPIHost>();
            apiHost.StartAPIService();
        }

        private void InitializeData()
        {
            ReportGadgetViewModel = new ReportGadgetViewModel();

            SwitchViewModel(EnumViewModelType.DashboardViewModel);

            StorageModel.StatisticCollection = new RangeObservableCollection<Playerstatistic>();
            StorageModel.PlayerCollection = new ObservableCollection<PlayerCollectionItem>(dataService.GetPlayersList());
            StorageModel.PropertyChanged += StorageModel_PropertyChanged;

            ProgressViewModel = new ProgressViewModel();

            StorageModel.TryLoadActivePlayer(dataService.GetActivePlayer(), loadHeroIfMissing: true);
        }

        private void InitializeBindings()
        {
            RadioGroupTab_CommandClick = new RelayCommand(new Action<object>(RadioGroupTab_OnClick));

            MenuItemPopupFilter_CommandClick = new RelayCommand(new Action<object>(MenuItemPopupFilter_OnClick));

            PurgeCommand = new RelayCommand(Purge);
            ImportFromFileCommand = new DelegateCommand(x => ImportFromFile(), x => !isManualImportingRunning);
            ImportFromDirectoryCommand = new DelegateCommand(x => ImportFromDirectory(), x => !isManualImportingRunning);
            SupportCommand = new RelayCommand(ShowSupportView);
            StartHudCommand = new DelegateCommand(x => StartHud(), x => !IsHudRunning);
            StopHudCommand = new DelegateCommand(x => StopHud(), x => IsHudRunning);
            HideEquityCalculatorCommand = new RelayCommand(HideEquityCalculator);
            SettingsCommand = new RelayCommand(OpenSettingsMenu);
            UpgradeCommand = new RelayCommand(Upgrade);
            PurchaseCommand = new RelayCommand(Purchase);

            PopupSettingsRequest = new InteractionRequest<PopupContainerSettingsViewModelNotification>();
            PopupFiltersRequest = new InteractionRequest<PopupContainerFiltersViewModelNotification>();
            PopupSupportRequest = new InteractionRequest<INotification>();
            RegistrationViewRequest = new InteractionRequest<INotification>();
            NotificationRequest = new InteractionRequest<INotification>();
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

        #region Methods

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
            hudTransmitter.Dispose();

            importerSessionCacheService.End();

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                // update data after hud is stopped
                CreatePositionReport();
                UpdateCurrentView();

                RefreshCommandsCanExecute();
            });

            GC.Collect();
            LogProvider.Log.Info(string.Format("Memory after stopping auto import: {0:N0}", GC.GetTotalMemory(false)));
        }

        internal void Load()
        {
            var statistics = dataService.GetPlayerStatisticFromFile(StorageModel.PlayerSelectedItem.Name, (short)StorageModel.PlayerSelectedItem.PokerSite);
            AddHandTags(statistics);

            if (statistics != null)
            {
                StorageModel.StatisticCollection.Reset(statistics);
            }
            else
            {
                StorageModel.StatisticCollection.Clear();
            }

            CreatePositionReport();

            UpdateCurrentView();
        }

        private void HandleSettingsChangedEvent(SettingsUpdatedEventArgs args)
        {
            if (args.IsUpdatePlayersCollection)
            {
                StorageModel.StatisticCollection = new RangeObservableCollection<Playerstatistic>();
                StorageModel.PlayerCollection = new ObservableCollection<PlayerCollectionItem>(dataService.GetPlayersList());
                StorageModel.TryLoadActivePlayer(dataService.GetActivePlayer(), loadHeroIfMissing: true);
            }

            if (CurrentViewModelType == EnumViewModelType.HudViewModel)
            {
                HudViewModel.RefreshHudTable();
            }
        }

        private void UpdateCurrentView(UpdateViewRequestedEventArgs args)
        {
            UpdateCurrentView();

            if (args != null && args.IsUpdateReportRequested)
            {
                ReportGadgetViewModel?.UpdateReport();
            }
        }

        private void UpdateCurrentView()
        {
            if (CurrentViewModel == null)
            {
                return;
            }
            else if (CurrentViewModel == DashboardViewModel)
            {
                DashboardViewModel.Update();
            }
            else if (CurrentViewModel == TournamentViewModel)
            {
                TournamentViewModel.Update();
            }
        }

        private void RefreshData(GameInfo gameInfo = null)
        {
            UpdatePlayerList(gameInfo);

            if (string.IsNullOrEmpty(StorageModel.PlayerSelectedItem.Name))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    StorageModel.TryLoadHeroPlayer();
                });
                return;
            }

            if (gameInfo == null)
            {
                UpdateCurrentView();
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

                // need to update UI info
                RefreshData(e.GameInfo);

                // if no handle available then we don't need to do anything with this data, because hud won't show up
                if (e.GameInfo.WindowHandle == 0)
                {
                    LogProvider.Log.Warn($"No window found for hand #{e.GameInfo.GameNumber}");
                    return;
                }

                var report = ReportGadgetViewModel.ReportCollection.FirstOrDefault();

                if (report != null)
                {
                    ReportGadgetViewModel.ReportSelectedItem = report;
                }

                var players = e.Players;
                var gameInfo = e.GameInfo;
                var maxSeats = (int)gameInfo.TableType;
                var site = e.GameInfo.PokerSite;

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
                var activeLayout = hudLayoutsService.GetActiveLayout(e.GameInfo.PokerSite, e.GameInfo.TableType, e.GameInfo.EnumGameType);

                if (activeLayout == null)
                {
                    LogProvider.Log.Error(this, $"Layout has not been found for {e.GameInfo.PokerSite}, {e.GameInfo.TableType}, {e.GameInfo.EnumGameType}");
                    return;
                }

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

                for (int seatNumber = 1; seatNumber <= maxSeats; seatNumber++)
                {
                    var player = players.FirstOrDefault(x => x.SeatNumber == seatNumber);

                    if (player == null || string.IsNullOrEmpty(player.PlayerName))
                    {
                        continue;
                    }

                    var playerCollectionItem = new PlayerCollectionItem
                    {
                        PlayerId = player.PlayerId,
                        Name = player.PlayerName,
                        PokerSite = site
                    };

                    var playerHudContent = new PlayerHudContent
                    {
                        Name = player.PlayerName,
                        SeatNumber = seatNumber
                    };

                    // cache service must return light indicators or hud light indicators
                    var sessionCacheStatistic = importerSessionCacheService.GetPlayerStats(gameInfo.Session, playerCollectionItem);
                    var lastHandStatistic = importerSessionCacheService.GetPlayersLastHandStatistics(gameInfo.Session, playerCollectionItem);

                    var item = sessionCacheStatistic.PlayerData;
                    var sessionData = sessionCacheStatistic.SessionPlayerData;

                    // need to do that for track meter tool
                    #region track condition meter

                    trackConditionsMeterData.VPIP += sessionData.VPIP;
                    trackConditionsMeterData.ThreeBet += sessionData.ThreeBet;

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
                        SeatNumber = seatNumber
                    };

                    playerHudContent.HudElement = hudElementCreator.Create(hudElementCreationInfo);

                    if (playerHudContent.HudElement == null)
                    {
                        continue;
                    }

                    playerHudContent.HudElement.TiltMeter = sessionData.TiltMeter;
                    playerHudContent.HudElement.PlayerName = player.PlayerName;
                    playerHudContent.HudElement.PokerSiteId = (short)site;
                    playerHudContent.HudElement.NoteToolTip = dataService.GetPlayerNote(player.PlayerName, (short)site)?.Note ?? string.Empty;
                    playerHudContent.HudElement.TotalHands = item.TotalHands;

                    var sessionMoney = sessionData.MoneyWonCollection;
                    playerHudContent.HudElement.SessionMoneyWonCollection = sessionData.MoneyWonCollection == null
                        ? new ObservableCollection<decimal>()
                        : new ObservableCollection<decimal>(sessionMoney);

                    var cardsCollection = sessionData.CardsList;
                    playerHudContent.HudElement.CardsCollection = cardsCollection == null
                        ? new ObservableCollection<string>()
                        : new ObservableCollection<string>(cardsCollection);

                    var activeLayoutHudStats = playerHudContent.HudElement.StatInfoCollection;

                    if (gameInfo.PokerSite == EnumPokerSites.PokerStars)
                    {
                        // remove prohibited for PS stats.
                        activeLayoutHudStats = activeLayoutHudStats.Where(x => x.Stat != Stat.FlopCBetMonotone && x.Stat != Stat.FlopCBetRag).ToArray();
                        activeLayoutHudStats.ForEach(x => x.SettingsAppearanceValueRangeCollection = new ObservableCollection<StatInfoOptionValueRange>(
                                x.SettingsAppearanceValueRangeCollection.Skip(Math.Max(0, x.SettingsAppearanceValueRangeCollection.Count() - 3))));
                    }

                    foreach (var statInfo in activeLayoutHudStats)
                    {
                        if (!string.IsNullOrEmpty(statInfo.PropertyName))
                        {
                            if (item.TotalHands < statInfo.MinSample)
                            {
                                statInfo.IsNotVisible = true;
                            }

                            statInfo.AssignStatInfoValues(item);
                        }
                        else if (!(statInfo is StatInfoBreak) && statInfo.Stat != Stat.PlayerInfoIcon)
                        {
                            continue;
                        }

                        #region tool tip (commented)

                        // temporary
                        //var tooltipCollection = StatInfoToolTip.GetToolTipCollection(statInfo.Stat);

                        //if (tooltipCollection != null)
                        //{
                        //    foreach (var tooltip in tooltipCollection)
                        //    {
                        //        tooltip.CategoryStat.AssignStatInfoValues(item);

                        //        foreach (var stat in tooltip.StatsCollection)
                        //        {
                        //            stat.AssignStatInfoValues(item);
                        //        }

                        //        if (tooltip.CardsList == null)
                        //        {
                        //            continue;
                        //        }

                        //        var listObj = ReflectionHelper.GetPropertyValue(sessionData, tooltip.CardsList.PropertyName) as IEnumerable<string>;

                        //        if (listObj != null)
                        //        {
                        //            tooltip.CardsList.Cards = new ObservableCollection<string>(listObj);
                        //        }
                        //    }

                        //    statInfo.StatInfoToolTipCollection = tooltipCollection;
                        //}

                        #endregion                        
                    }

                    if (gameInfo.PokerSite != EnumPokerSites.PokerStars && lastHandStatistic != null)
                    {
                        var stickers = hudLayoutsService.GetValidStickers(lastHandStatistic, activeLayout.Name);

                        if (stickers.Any())
                        {
                            importerSessionCacheService.AddOrUpdatePlayerStickerStats(gameInfo.Session, playerCollectionItem, stickers.ToDictionary(x => x, x => lastHandStatistic));
                        }

                        hudLayoutsService.SetStickers(playerHudContent.HudElement,
                            importerSessionCacheService.GetPlayersStickersStatistics(gameInfo.Session,
                                playerCollectionItem), activeLayout.Name);
                    }

                    ht.ListHUDPlayer.Add(playerHudContent);
                }

                if (gameInfo.PokerSite != EnumPokerSites.PokerStars)
                {
                    var hudElements = ht.ListHUDPlayer.Select(x => x.HudElement).ToArray();
                    hudLayoutsService.SetPlayerTypeIcon(hudElements, activeLayout.Name);
                }

                Func<decimal, decimal, decimal> getDevisionResult = (x, y) =>
                {
                    return y != 0 ? x / y : 0;
                };

                var trackConditionsInfo = new HudTrackConditionsViewModelInfo
                {
                    AveragePot = getDevisionResult(trackConditionsMeterData.TotalPotSize, trackConditionsMeterData.TotalHands),
                    VPIP = getDevisionResult(trackConditionsMeterData.VPIP, players.Count),
                    ThreeBet = getDevisionResult(trackConditionsMeterData.ThreeBet, players.Count),
                    TableType = gameInfo.TableType,
                    BuyInNL = Utils.ConvertBigBlindToNL(trackConditionsMeterData.BigBlind)
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

                if (!isSetPlayerIdMessageShown && (string.IsNullOrEmpty(StorageModel.PlayerSelectedItem.Name) ||
                    string.IsNullOrEmpty(StorageModel.PlayerSelectedItem.DecodedName) ||
                    StorageModel.PlayerSelectedItem.PokerSite == EnumPokerSites.Unknown))
                {
                    isSetPlayerIdMessageShown = true;
                    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate
                   {
                       this.NotificationRequest.Raise(
                           new PopupActionNotification
                           {
                               Content = "Please, set Player ID in order to see data.",
                               Title = "DriveHUD",
                           }, n => { });
                   });
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Importing failed", ex);
            }
        }

        internal async void ImportFromFile()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Multiselect = true;

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
                RefreshData();
            });

            ProgressViewModel.IsActive = false;
            IsManualImportingRunning = false;
            ProgressViewModel.Reset();

            CreatePositionReport();
        }

        internal async void ImportFromDirectory()
        {
            var folderDialog = new FolderBrowserDialog();

            var result = folderDialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            var _validExtensions = new HashSet<string> { ".txt", ".xml" };
            var filesToImport = Directory.EnumerateFiles(folderDialog.SelectedPath, "*.*", SearchOption.AllDirectories)
                .Where(f => _validExtensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f => new FileInfo(f)).ToArray();

            if (filesToImport.Length == 0)
            {
                return;
            }

            var fileImporter = ServiceLocator.Current.GetInstance<IFileImporter>();

            IsManualImportingRunning = true;

            await Task.Factory.StartNew(() =>
            {
                fileImporter.Import(filesToImport, ProgressViewModel.Progress);
                RefreshData();
            });

            ProgressViewModel.IsActive = false;
            IsManualImportingRunning = false;
            ProgressViewModel.Reset();

            CreatePositionReport();
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

        internal async void Purge()
        {
            ProgressViewModel.IsActive = true;

            await Task.Factory.StartNew(() => dataService.Purge());

            ProgressViewModel.IsActive = false;
            StorageModel.PlayerCollection.Clear();

            Load();
        }

        internal void CreatePositionReport()
        {
            ReportGadgetViewModel.UpdateReport();
        }

        private void UpdatePlayerList(GameInfo gameInfo)
        {
            var updatedPlayers = gameInfo != null && gameInfo.AddedPlayers != null ? gameInfo.AddedPlayers : dataService.GetPlayersList();

            foreach (var player in updatedPlayers)
            {
                App.Current.Dispatcher.Invoke(() => StorageModel.PlayerCollection.Add(player));
            }
        }

        private void SwitchViewModel(EnumViewModelType viewModelType)
        {
            IsShowEquityCalculator = false;
            if (viewModelType == EnumViewModelType.DashboardViewModel)
            {
                if (this.DashboardViewModel == null)
                {
                    this.DashboardViewModel = new DashboardViewModel(synchronizationContext)
                    {
                        Type = EnumViewModelType.DashboardViewModel,
                    };
                }

                this.CurrentViewModel = this.DashboardViewModel;
                this.ReportGadgetViewModel.IsShowTournamentData = false;
                UpdateCurrentView();

                filterModelManager.SetFilterType(EnumFilterType.Cash);
                eventAggregator.GetEvent<UpdateFilterRequestEvent>().Publish(new UpdateFilterRequestEventArgs());
            }
            if (viewModelType == EnumViewModelType.TournamentViewModel)
            {
                if (this.TournamentViewModel == null)
                {
                    this.TournamentViewModel = new TournamentViewModel()
                    {
                        Type = EnumViewModelType.TournamentViewModel,
                    };
                }

                this.CurrentViewModel = this.TournamentViewModel;
                this.ReportGadgetViewModel.IsShowTournamentData = true;
                UpdateCurrentView();

                filterModelManager.SetFilterType(EnumFilterType.Tournament);
                eventAggregator.GetEvent<UpdateFilterRequestEvent>().Publish(new UpdateFilterRequestEventArgs());
            }
            if (viewModelType == EnumViewModelType.HudViewModel)
            {
                if (this.HudViewModel == null)
                {
                    this.HudViewModel = new HudViewModel();
                }

                filterModelManager.SetFilterType(EnumFilterType.Cash);
                this.CurrentViewModel = this.HudViewModel;
            }
            if (viewModelType == EnumViewModelType.AppsViewModel)
            {
                if (this.AppsViewModel == null)
                {
                    this.AppsViewModel = new AppsViewModel();
                }

                this.CurrentViewModel = this.AppsViewModel;
            }
        }

        private void AddHandTags(IList<Playerstatistic> statistics)
        {
            var notes = dataService.GetHandNotes((short)StorageModel.PlayerSelectedItem.PokerSite);

            var statisticsForUpdate = (from note in notes
                                       join statistic in statistics on note.Gamenumber equals statistic.GameNumber
                                       select new { Note = note, Statistic = statistic }).ToArray();

            foreach (var statisticForUpdate in statisticsForUpdate)
            {
                statisticForUpdate.Statistic.HandNote = statisticForUpdate.Note;
            }
        }

        private void OpenSettingsMenu(object obj)
        {
            PopupSettingsRequest_Execute(new PubSubMessage());
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

        internal void UpdateHeader()
        {
            OnPropertyChanged(nameof(AppStartupHeader));
        }

        #endregion

        #region Properties

        private DashboardViewModel _dashboardViewModel;
        private TournamentViewModel _tournamentViewModel;
        private HudViewModel _hudViewModel;
        private AppsViewModel _appsViewModel;
        private ReportGadgetViewModel _reportGadgetViewModel;
        private bool _isShowEquityCalculator;

        private object _currentViewModel;
        private EnumViewModelType _currentViewModelType;

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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
                RefreshCommandsCanExecute();
            }
        }

        public DashboardViewModel DashboardViewModel
        {
            get { return _dashboardViewModel; }
            set
            {
                _dashboardViewModel = value;
                OnPropertyChanged();
            }
        }

        public TournamentViewModel TournamentViewModel
        {
            get { return _tournamentViewModel; }
            set
            {
                _tournamentViewModel = value;
                OnPropertyChanged();
            }
        }

        public HudViewModel HudViewModel
        {
            get { return _hudViewModel; }
            set
            {
                _hudViewModel = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();

                if (this.DashboardViewModel != null) this.DashboardViewModel.IsActive = value.GetType().Equals(typeof(DashboardViewModel));
                if (this.TournamentViewModel != null) this.TournamentViewModel.IsActive = value.GetType().Equals(typeof(TournamentViewModel));
                if (this.HudViewModel != null) this.HudViewModel.IsActive = value.GetType().Equals(typeof(HudViewModel));
                if (this.AppsViewModel != null) this.AppsViewModel.IsActive = value.GetType().Equals(typeof(AppsViewModel));

                this.ReportGadgetView_IsVisible = !value.GetType().Equals(typeof(HudViewModel))
                    && !value.GetType().Equals(typeof(AppsViewModel));

                if (value.GetType().Equals(typeof(DashboardViewModel)))
                {
                    _currentViewModelType = EnumViewModelType.DashboardViewModel;
                }

                if (value.GetType().Equals(typeof(TournamentViewModel)))
                {
                    _currentViewModelType = EnumViewModelType.TournamentViewModel;
                }

                if (value.GetType().Equals(typeof(HudViewModel)))
                {
                    _currentViewModelType = EnumViewModelType.HudViewModel;
                }

                if (value.GetType().Equals(typeof(AppsViewModel)))
                {
                    _currentViewModelType = EnumViewModelType.AppsViewModel;
                }
            }
        }

        public EnumViewModelType CurrentViewModelType
        {
            get { return _currentViewModelType; }
            set
            {
                _currentViewModelType = value;
                OnPropertyChanged();
            }
        }

        public bool ReportGadgetView_IsVisible
        {
            get { return _reportGadgetView_IsVisible; }
            set
            {
                _reportGadgetView_IsVisible = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
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
                var filterTuple = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString()).FilterTupleCollection.FirstOrDefault();
                PopupFiltersRequestExecute(filterTuple);
                return;
            }

            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            switch (type.FilterType)
            {
                case EnumFilterDropDown.FilterToday:
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(EnumDateFiter.Today));
                    break;
                case EnumFilterDropDown.FilterThisWeek:
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(EnumDateFiter.ThisWeek));
                    break;
                case EnumFilterDropDown.FilterThisMonth:
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(EnumDateFiter.ThisMonth));
                    break;
                case EnumFilterDropDown.FilterLastMonth:
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(EnumDateFiter.LastMonth));
                    break;
                case EnumFilterDropDown.FilterThisYear:
                    eventAggregator.GetEvent<DateFilterChangedEvent>().Publish(new DateFilterChangedEventArgs(EnumDateFiter.ThisYear));
                    break;
                case EnumFilterDropDown.FilterAllStats:
                default:
                    eventAggregator.GetEvent<ResetFiltersEvent>().Publish(new ResetFiltersEventArgs());
                    break;
            }
        }

        public ICommand PurgeCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand ImportFromFileCommand { get; set; }
        public ICommand ImportFromDirectoryCommand { get; set; }
        public ICommand SupportCommand { get; set; }
        public ICommand StartHudCommand { get; set; }
        public ICommand StopHudCommand { get; set; }
        public ICommand CalculateEquityCommand { get; set; }
        public ICommand HideEquityCalculatorCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        #endregion

        #region Events

        public void MainWindow_PreviewClosed(object sender, WindowPreviewClosedEventArgs e)
        {
            if (importerService.IsStarted)
            {
                importerService.StopImport();
            }

            hudTransmitter.Dispose();
            importerSessionCacheService.End();

            dataService.SaveActivePlayer(StorageModel.PlayerSelectedItem.Name, (short)StorageModel.PlayerSelectedItem.PokerSite);

            // flush betonline cash
            var tournamentsCacheService = ServiceLocator.Current.GetInstance<ITournamentsCacheService>();
            tournamentsCacheService.Flush();

            PokerStarsDetectorSingletonService.Instance.Stop();

            apiHost.CloseAPIService();

            if (ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.IsSaveFiltersOnExit)
            {
                eventAggregator.GetEvent<SaveDefaultFilterRequestedEvent>().Publish(new SaveDefaultFilterRequestedEvetnArgs());
            }
        }

        private void StorageModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private void PopupSettingsRequest_Execute(PubSubMessage pubSubMessage)
        {
            PopupContainerSettingsViewModelNotification notification = new PopupContainerSettingsViewModelNotification();

            notification.Title = "Settings";
            notification.PubSubMessage = pubSubMessage;

            this.PopupSettingsRequest.Raise(notification,
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
            PopupContainerFiltersViewModelNotification notification = new PopupContainerFiltersViewModelNotification();

            notification.Title = "Filters";
            notification.FilterTuple = filterTuple;

            this.PopupFiltersRequest.Raise(notification,
                returned => { });
        }

        private void RaiseNotification(MainNotificationEventArgs obj)
        {
            if (obj == null)
            {
                return;
            }

            PopupActionNotification confirmation = new PopupActionNotification();
            confirmation.Title = obj.Title;
            confirmation.Content = obj.Message;
            confirmation.HyperLinkText = obj.HyperLink;

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