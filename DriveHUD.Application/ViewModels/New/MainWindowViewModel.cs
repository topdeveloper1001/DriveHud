using DriveHUD.Application.Models;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Importers;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
using DriveHUD.Entities;
using Model.Enums;
using Model.Events;
using Model.Filters;
using Model.Interfaces;
using Model.Reports;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using DriveHUD.Application.ViewModels.Hud;
using Model.Settings;
using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Application.Licensing;
using System.Diagnostics;
using System.Windows.Threading;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Common.Log;
using DriveHUD.Common.Wpf.Actions;
using Newtonsoft.Json;
using ProtoBuf;
using System.Collections.Concurrent;
using DriveHUD.Application.HudServices;
using DriveHUD.Common.WinApi;
using Model.Site;

namespace DriveHUD.Application.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Fields

        private IDataService dataService;

        private IEventAggregator eventAggregator;

        private IImporterSessionCacheService importerSessionCacheService;

        private IFilterModelManagerService filterModelManager;

        #endregion

        #region Constructor

        internal MainWindowViewModel(SynchronizationContext _synchronizationContext)
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();

            importerSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();
            filterModelManager = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());

            synchronizationContext = _synchronizationContext;

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<RequestEquityCalculatorEvent>().Subscribe(ShowCalculateEquityView);
            eventAggregator.GetEvent<DataImportedEvent>().Subscribe(OnDataImported, ThreadOption.BackgroundThread, false);
            eventAggregator.GetEvent<SettingsUpdatedEvent>().Subscribe(HandleSettingsChangedEvent);
            eventAggregator.GetEvent<UpdateViewRequestedEvent>().Subscribe(UpdateCurrentView);
            eventAggregator.GetEvent<MainNotificationEvent>().Subscribe(RaiseNotification);

            InitializeFilters();
            InitializeData();
            InitializeBindings();

            HudViewModel = new HudViewModel();
            filterModelManager.SetFilterType(EnumFilterType.Cash);
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
            ImportFromFileCommand = new RelayCommand(ImportFromFile);
            ImportFromDirectoryCommand = new RelayCommand(ImportFromDirectory);
            SupportCommand = new RelayCommand(ShowSupportView);
            StartHudCommand = new RelayCommand(() => StartHud(false));
            StopHudCommand = new RelayCommand(() => StopHud(false));
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

        internal void StartHud(bool switchViewModel = true)
        {
            LogProvider.Log.Info(string.Format("Memory before starting auto import: {0:N0}", GC.GetTotalMemory(false)));

            int workerThreads;
            int completionPortThreads;

            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);

            LogProvider.Log.Info($"Threads info: workerThreads: {workerThreads} completionPortThreads: {completionPortThreads} ");

            if (switchViewModel)
            {
                SwitchViewModel(EnumViewModelType.HudViewModel);
            }

            HudViewModel.Start();

            importerSessionCacheService.Begin();
        }

        internal void StopHud(bool switchViewModel = false)
        {
            if (switchViewModel)
            {
                SwitchViewModel(EnumViewModelType.HudViewModel);
            }
            HudViewModel.Stop();

            importerSessionCacheService.End();

            // update data after hud is stopped
            CreatePositionReport();
            UpdateCurrentView();

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

        private void UpdateCurrentView(EventArgs args)
        {
            UpdateCurrentView();
            ReportGadgetViewModel?.UpdateReport();
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

        private void RefreshData()
        {
            UpdatePlayerList();

            if (string.IsNullOrEmpty(StorageModel.PlayerSelectedItem.Name))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    StorageModel.TryLoadHeroPlayer();
                });
                return;
            }

            UpdateCurrentView();
        }

        private void OnDataImported(DataImportedEventArgs e)
        {
            try
            {
                var sw = new Stopwatch();

                sw.Start();

                RefreshData();

                var refreshTime = sw.ElapsedMilliseconds;

                Debug.WriteLine("RefreshData {0} ms", refreshTime);

                sw.Restart();

                var report = ReportGadgetViewModel.ReportCollection.FirstOrDefault();

                if (report != null)
                {
                    ReportGadgetViewModel.ReportSelectedItem = report;
                }

                var players = e.Players;
                var gameInfo = e.GameInfo;
                var maxSeats = (int)gameInfo.TableType;
                var site = e.GameInfo.PokerSite;

                var ht = new HudLayout
                {
                    WindowId = gameInfo.WindowHandle,
                    HudType = site == EnumPokerSites.Ignition ? HudViewModel.HudType : HudType.Plain,
                    TableType = gameInfo.TableType
                };

                var tableKey = HudViewModel.GetHash(site, gameInfo.EnumGameType, gameInfo.TableType);

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                var trackConditionsMeterData = new HudTrackConditionsMeterData();

                for (int i = 1; i <= maxSeats; i++)
                {
                    var playerName = string.Empty;
                    var playerCollectionItem = new PlayerCollectionItem();
                    var seatNumber = 0;

                    foreach (var player in players)
                    {
                        if (player.SeatNumber == i)
                        {
                            if (!string.IsNullOrEmpty(player.PlayerName))
                            {
                                playerName = player.PlayerName;
                                seatNumber = player.SeatNumber;
                                playerCollectionItem = new PlayerCollectionItem { Name = player.PlayerName, PokerSite = site };
                            }

                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(playerName))
                    {
                        continue;
                    }

                    var playerHudContent = new PlayerHudContent
                    {
                        Name = playerName,
                        SeatNumber = seatNumber
                    };

                    var statisticCollection = importerSessionCacheService.GetPlayerStats(gameInfo.Session, playerCollectionItem);
                    var lastHandStatistic = importerSessionCacheService.GetPlayersLastHandStatistics(gameInfo.Session, playerCollectionItem);
                    var sessionStatisticCollection = statisticCollection.Where(x => !string.IsNullOrWhiteSpace(x.SessionCode) && x.SessionCode == gameInfo.Session);

                    var item = new HudIndicators(statisticCollection);
                    var sessionData = new HudIndicators(sessionStatisticCollection);

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

                    var hudElementCreator = ServiceLocator.Current.GetInstance<IHudElementViewModelCreator>();

                    playerHudContent.HudElement = hudElementCreator.Create(tableKey, HudViewModel, seatNumber, ht.HudType);

                    if (playerHudContent.HudElement == null)
                    {
                        continue;
                    }

                    playerHudContent.HudElement.TiltMeter = sessionData.TiltMeter;
                    playerHudContent.HudElement.PlayerName = playerName;
                    playerHudContent.HudElement.PokerSiteId = (short)site;
                    playerHudContent.HudElement.IsNoteIconVisible = !string.IsNullOrWhiteSpace(dataService.GetPlayerNote(playerName, (short)site)?.Note ?? string.Empty);
                    playerHudContent.HudElement.TotalHands = item.TotalHands;

                    var sessionMoney = sessionStatisticCollection.SingleOrDefault(x => x.MoneyWonCollection != null)?.MoneyWonCollection;
                    playerHudContent.HudElement.SessionMoneyWonCollection = sessionMoney == null
                        ? new ObservableCollection<decimal>()
                        : new ObservableCollection<decimal>(sessionMoney);

                    var cardsCollection = sessionStatisticCollection.SingleOrDefault(x => x.CardsList != null)?.CardsList;
                    playerHudContent.HudElement.CardsCollection = cardsCollection == null
                        ? new ObservableCollection<string>()
                        : new ObservableCollection<string>(cardsCollection);

                    var doNotAddPlayer = false;

                    var activeLayout = hudLayoutsService.GetActiveLayout(tableKey);

                    if (activeLayout == null)
                    {
                        LogProvider.Log.Error(this, "Could not find active layout");
                        return;
                    }

                    // create new array to prevent Collection was modified exception
                    var activeLayoutHudStats = activeLayout.HudStats.ToArray();

                    var statsExceptActive = HudViewModel.StatInfoCollection.Concat(HudViewModel.StatInfoObserveCollection)
                                            .Except(activeLayoutHudStats, new LambdaComparer<StatInfo>((x, y) => x.Stat == y.Stat)).Select(x =>
                                            {
                                                var statInfoBreak = x as StatInfoBreak;

                                                if (statInfoBreak != null)
                                                {
                                                    return statInfoBreak.Clone();
                                                }

                                                return x.Clone();
                                            }).ToArray();

                    statsExceptActive.ForEach(x => x.IsNotVisible = true);

                    var allStats = activeLayoutHudStats.Select(x =>
                    {
                        var statInfoBreak = x as StatInfoBreak;

                        if (statInfoBreak != null)
                        {
                            return statInfoBreak.Clone();
                        }

                        return x.Clone();
                    }).Concat(statsExceptActive);

                    foreach (var statInfo in allStats)
                    {
                        if (!string.IsNullOrEmpty(statInfo.PropertyName))
                        {
                            statInfo.AssignStatInfoValues(item);
                        }
                        else if (!(statInfo is StatInfoBreak))
                        {
                            doNotAddPlayer = true;
                        }

                        // temporary
                        var tooltipCollection = StatInfoToolTip.GetToolTipCollection(statInfo.Stat);

                        if (tooltipCollection != null)
                        {
                            foreach (var tooltip in tooltipCollection)
                            {
                                tooltip.CategoryStat.AssignStatInfoValues(item);

                                foreach (var stat in tooltip.StatsCollection)
                                {
                                    stat.AssignStatInfoValues(item);
                                }

                                if (tooltip.CardsList == null)
                                {
                                    continue;
                                }

                                var listObj = ReflectionHelper.GetPropertyValue(sessionData, tooltip.CardsList.PropertyName) as IEnumerable<string>;

                                if (listObj != null)
                                {
                                    tooltip.CardsList.Cards = new ObservableCollection<string>(listObj);
                                }
                            }
                            statInfo.StatInfoToolTipCollection = tooltipCollection;
                        }

                        playerHudContent.HudElement.StatInfoCollection.Add(statInfo);
                    }

                    if (lastHandStatistic != null)
                    {
                        var stickers = hudLayoutsService.GetValidStickers(lastHandStatistic, tableKey);

                        if (stickers.Any())
                        {
                            importerSessionCacheService.AddOrUpdatePlayerStickerStats(gameInfo.Session, playerCollectionItem, stickers.ToDictionary(x => x, x => lastHandStatistic));
                        }

                        hudLayoutsService.SetStickers(playerHudContent.HudElement, importerSessionCacheService.GetPlayersStickersStatistics(gameInfo.Session, playerCollectionItem), tableKey);
                    }

                    if (!doNotAddPlayer)
                    {
                        ht.ListHUDPlayer.Add(playerHudContent);
                    }
                }

                sw.Stop();

                Debug.WriteLine("Hand has been imported for {0} ms", sw.ElapsedMilliseconds + refreshTime);

                if (HudViewModel.HudTableViewModelDictionary.ContainsKey(tableKey))
                {
                    var hudElements = ht.ListHUDPlayer.Select(x => x.HudElement).ToArray();
                    hudLayoutsService.SetPlayerTypeIcon(hudElements, tableKey);

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

                    ht.TableHud = HudViewModel.HudTableViewModelDictionary[tableKey].Clone();

                    byte[] serialized;

                    using (var msTestString = new MemoryStream())
                    {
                        Serializer.Serialize(msTestString, ht);
                        serialized = msTestString.ToArray();
                    }

                    var settingsModel = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

                    if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                    {
                        LogProvider.Log.Info(this, $"Sending {serialized.Length} bytes to HUD [handle={ht.WindowId}, title={WinApi.GetWindowText(new IntPtr(ht.WindowId))}]");
                    }

                    var hudTransmitter = ServiceLocator.Current.GetInstance<IHudTransmitter>();
                    hudTransmitter.Send(serialized);

                    if (settingsModel.GeneralSettings.IsAdvancedLoggingEnabled)
                    {
                        LogProvider.Log.Info(this, $"Data has been sent to HUD [handle={ht.WindowId}]");
                    }
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

            await Task.Run(() =>
            {
                fileImporter.Import(filesToImport, ProgressViewModel.Progress);
                RefreshData();
            });

            ProgressViewModel.IsActive = false;

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

            await Task.Factory.StartNew(() =>
            {
                fileImporter.Import(filesToImport, ProgressViewModel.Progress);
                RefreshData();
            });

            ProgressViewModel.IsActive = false;

            CreatePositionReport();
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

        private void UpdatePlayerList()
        {
            var updatedPlayers = dataService.GetPlayersList();

            foreach (var player in updatedPlayers)
            {
                if (StorageModel.PlayerCollection.Contains(player))
                {
                    continue;
                }

                var playerCopy = player;

                App.Current.Dispatcher.Invoke(() => StorageModel.PlayerCollection.Add(playerCopy));
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
            HudViewModel.Stop();
            importerSessionCacheService.End();
            dataService.SaveActivePlayer(StorageModel.PlayerSelectedItem.Name, (short)StorageModel.PlayerSelectedItem.PokerSite);

            // flush betonline cash
            var tournamentsCacheService = ServiceLocator.Current.GetInstance<ITournamentsCacheService>();
            tournamentsCacheService.Flush();

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