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

using DriveHUD.Common.Linq;
using DriveHUD.Common.Reflection;
using DriveHUD.Importers;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.Enums;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using ReactiveUI;
using System.Reactive.Linq;
using Model.Data;
using DriveHUD.Application.TableConfigurators;
using System.Collections.Specialized;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Resources;
using DriveHUD.Common;
using Model.Site;
using DriveHUD.Common.Log;
using Prism.Interactivity.InteractionRequest;
using Model.Settings;
using Model.Events;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Entities;
using DriveHUD.Application.Controls;
using System.ComponentModel;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Class HUD View Model
    /// </summary>
    public class HudViewModel : PopupViewModelBase
    {
        private IHudLayoutsService hudLayoutsSevice;

        private ISettingsService settingsService
        {
            get { return ServiceLocator.Current.GetInstance<ISettingsService>(); }
        }

        private bool isUpdatingLayout = false;

        public HudViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize view model
        /// </summary>
        private void Initialize()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<PreferredSeatChangedEvent>().Subscribe(OnPreferredSeatChanged);

            hudLayoutsSevice = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            NotificationRequest = new InteractionRequest<INotification>();

            InitializeStatInfoCollection();
            InitializeStatInfoCollectionObserved();

            InitializePlayerCollection();

            gameTypes = Enum.GetValues(typeof(EnumGameType)).Cast<EnumGameType>().Select(x => new EnumGameTypeWrapper(x)).ToList();
            gameType = gameTypes.FirstOrDefault(x => x.GameType == EnumGameType.CashHoldem);

            InitializeTableLayouts();

            HudType = HudType.Default;

            hudViewTypes = new ObservableCollection<HudViewType>(Enum.GetValues(typeof(HudViewType)).Cast<HudViewType>());
            hudViewType = HudViewType.Vertical;

            PreviewHudElementViewModel = new HudElementViewModel { TiltMeter = 100 };

            InitializeCommands();
            InitializeObservables();

            InitializeHudElements();
            InitializeLayouts();
        }

        private void InitializeHudElements()
        {
            HudTableViewModelDictionary = new Dictionary<int, HudTableViewModel>();

            // create HUD elements
            foreach (var tableLayout in tableLayouts.SelectMany(x => x.Items).OfType<HudTableLayoutMenuItem>().Where(x => x.HudTableLayout != null).Select(x => x.HudTableLayout).ToArray())
            {
                foreach (EnumGameType gameType in Enum.GetValues(typeof(EnumGameType)))
                {
                    var hudElements = new ObservableCollection<HudElementViewModel>();

                    foreach (HudType hudType in Enum.GetValues(typeof(HudType)))
                    {
                        var tableConfigurator = ServiceLocator.Current.GetInstance<ITableConfigurator>(TableConfiguratorHelper.GetServiceName(tableLayout.Site, hudType));
                        var hudElementViewModels = tableConfigurator.GenerateElements((int)tableLayout.TableType);

                        hudElements.AddRange(hudElementViewModels);
                    }

                    // generate hud elements                        
                    var hudTableViewModel = new HudTableViewModel
                    {
                        HudElements = hudElements,
                        TableLayout = tableLayout
                    };

                    HudTableViewModelDictionary.Add(GetHash(tableLayout.Site, gameType, tableLayout.TableType), hudTableViewModel);
                }
            }
        }

        private void InitializeLayouts()
        {
            // if no layouts were created then save defaults
            if (hudLayoutsSevice.Layouts.Layouts.Count < 1)
            {
                hudLayoutsSevice.SaveDefaults(HudTableViewModelDictionary);
            }
            else
            {
                var hudTablesToUpdate = (from hudTableViewModel in HudTableViewModelDictionary
                                         join layout in hudLayoutsSevice.Layouts.Layouts on hudTableViewModel.Key equals layout.LayoutId into gj
                                         from grouped in gj.DefaultIfEmpty()
                                         where grouped != null && grouped.IsDefault
                                         select new { HudTableViewModel = hudTableViewModel.Value, Layout = grouped }).ToArray();

                var hudTablesToAdd = (from hudTableViewModel in HudTableViewModelDictionary
                                      join layout in hudLayoutsSevice.Layouts.Layouts on hudTableViewModel.Key equals layout.LayoutId into gj
                                      from grouped in gj.DefaultIfEmpty()
                                      where grouped == null
                                      select hudTableViewModel).ToDictionary(x => x.Key, x => x.Value);

                if (hudTablesToAdd.Count > 0)
                {
                    hudLayoutsSevice.SaveDefaults(hudTablesToAdd);
                }

                hudTablesToUpdate.ForEach(x =>
                {
                    MergeLayouts(x.HudTableViewModel.HudElements, x.Layout);
                });
            }

            UpdateActiveLayout();

            var savedLayouts = hudLayoutsSevice.Layouts.Layouts.Where(x => x.LayoutId == CurrentViewModelHash);

            layouts = new ObservableCollection<HudSavedLayout>(savedLayouts);
        }

        private void InitializeTableLayouts()
        {
            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();

            var configurations = configurationService.GetAll();

            var hudTableLayoutMenuItems = new List<IDropDownMenuItem>();

            foreach (var configuration in configurations)
            {
                var hudTableLayoutMenuItem = new HudTableLayoutMenuItem
                {
                    Header = CommonResourceManager.Instance.GetEnumResource(configuration.Site)
                };

                hudTableLayoutMenuItem.Items = new ObservableCollection<IDropDownMenuItem>(from tableType in configuration.TableTypes
                                                                                           let hudLayout = new HudTableLayout
                                                                                           {
                                                                                               Site = configuration.Site,
                                                                                               TableType = tableType
                                                                                           }
                                                                                           select new HudTableLayoutMenuItem(hudLayout)
                                                                                           {
                                                                                               Parent = hudTableLayoutMenuItem
                                                                                           });

                hudTableLayoutMenuItems.Add(hudTableLayoutMenuItem);
            }


            tableLayouts = new ObservableCollection<IDropDownMenuItem>(hudTableLayoutMenuItems);

            currentTableLayout = hudTableLayoutMenuItems.SelectMany(x => x.Items).OfType<HudTableLayoutMenuItem>().
                                   FirstOrDefault(x => x.HudTableLayout != null && x.HudTableLayout.Site == EnumPokerSites.Bovada && x.HudTableLayout.TableType == EnumTableType.Six);
        }

        /// <summary>
        /// Initialize stat collection
        /// </summary>
        private void InitializeStatInfoCollection()
        {
            // Make a list of StatInfoGroups
            var statInfoGroups = new[]
            {
                new StatInfoGroup { Name = "Most Popular" },
                new StatInfoGroup { Name = "Positional" },
                new StatInfoGroup { Name = "3-Bet" },
                new StatInfoGroup { Name = "4-Bet" },
                new StatInfoGroup { Name = "Flop" },
                new StatInfoGroup { Name = "Turn" },
                new StatInfoGroup { Name = "River" },
                new StatInfoGroup { Name = "Tournament" },
                new StatInfoGroup { Name = "Limp" },
                new StatInfoGroup { Name = "Advanced Stats" },
            };

            // Make a collection of StatInfo
            StatInfoCollection = new ReactiveList<StatInfo>
            {
                new StatInfo { IsListed = false, GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.UO_PFR_EP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.UO_PFR_EP)},

                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.VPIP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.VPIP) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.PFR, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFR)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S3Bet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.AGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.AggPr) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.CBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopCBet)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WTSD, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.WTSD) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WSSD, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.WSSD) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WWSF, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.WSWSF) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.TotalHands, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TotalHands) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldToCBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FoldCBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo3Bet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FoldToThreeBet)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S4Bet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo4Bet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FoldToFourBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FlopAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopAgg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.TurnAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TurnAgg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.RiverAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RiverAgg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.ColdCall, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ColdCall)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.Steal, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.Steal) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldToSteal, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.BlindsFoldSteal) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.Squeeze, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.Squeeze) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.CheckRaise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CheckRaise) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.CBetIP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CBetIP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.CBetOOP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CBetOOP) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S3BetMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInMP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S3BetCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInCO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S3BetBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInBTN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S3BetSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInSB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S3BetBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInBB) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInMP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInCO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInBTN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInSB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInBB) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ColdCallMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ColdCallInMP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ColdCallCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ColdCallInCO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ColdCallBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ColdCallInBTN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ColdCallSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ColdCallInSB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ColdCallBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ColdCallInBB) },


                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetIP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetIP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetOOP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetOOP) },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInMP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInCO) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInBTN) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInSB) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInBB) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetVsSteal, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetVsSteal) },

                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInMP) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInCO) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInBTN) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInSB) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInBB) },

                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.WWSF, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.WSWSF) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FlopCheckRaise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopCheckRaise) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.CBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopCBet)},
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FloatFlop, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FloatFlop)},
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FlopAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopAgg) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.RaiseFlop, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseFlop) },

                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.DelayedTurnCBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidDelayedTurnCBet) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.TurnCheckRaise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TurnCheckRaise) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.RaiseTurn, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseTurn) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.TurnAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TurnAgg) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.TurnSeen, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TurnSeen) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.DoubleBarrel, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TurnCBet) },

                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.RaiseRiver, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseRiver) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.RiverAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RiverAgg) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.RiverSeen, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RiverSeen) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckRiverOnBXLine, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CheckRiverOnBXLine) },

                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.MRatio, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.MRatio) },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.BBs, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.StackInBBs) },

                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.Limp, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimp) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.LimpCall, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimpCall) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.LimpFold, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimpFold) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.LimpReraise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimpReraise) },

                new StatInfo { GroupName = "10", StatInfoGroup = statInfoGroups[9], Stat = Stat.RaiseFrequencyFactor, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseFrequencyFactor) },
                new StatInfo { GroupName = "10", StatInfoGroup = statInfoGroups[9], Stat = Stat.TrueAggression, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TrueAggression) },
                new StatInfo { GroupName = "10", StatInfoGroup = statInfoGroups[9], Stat = Stat.DonkBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DonkBet) },
                new StatInfo { GroupName = "10", StatInfoGroup = statInfoGroups[9], Stat = Stat.DelayedTurnCBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidDelayedTurnCBet) },
            };

            // initialize stat info
            StatInfoCollection.ForEach(x => x.Initialize());
        }

        private void InitializeStatInfoCollectionObserved()
        {
            StatInfoObserveCollection = new StatInfoObservableCollection<StatInfo> { AllowBreak = true, BreakCount = 4 };
        }

        private void InitializePlayerCollection()
        {
            PlayerCollection = new ObservableCollection<PlayerHudContent>();
        }

        protected override void InitializeCommands()
        {
            base.InitializeCommands();

            DataSaveCommand = ReactiveCommand.Create();
            DataSaveCommand.Subscribe(x => OpenDataSave());

            DataDeleteCommand = ReactiveCommand.Create();
            DataDeleteCommand.Subscribe(x => DataDelete());

            DataImportCommand = ReactiveCommand.Create();
            DataImportCommand.Subscribe(x => DataImport());

            DataExportCommand = ReactiveCommand.Create();
            DataExportCommand.Subscribe(x => DataExport());

            SpliterAddCommand = ReactiveCommand.Create();
            SpliterAddCommand.Subscribe(x => SpliterAdd());

            SettingsStatInfoCommand = ReactiveCommand.Create();
            SettingsStatInfoCommand.Subscribe(x => OpenStatsSettings(x as StatInfo));

            PlayerTypeStatsCommand = ReactiveCommand.Create();
            PlayerTypeStatsCommand.Subscribe(x => OpenPlayerTypeStats(x as StatInfo));

            BumperStickersCommand = ReactiveCommand.Create();
            BumperStickersCommand.Subscribe(x => OpenBumperStickers(x as StatInfo));

            SelectHudCommand = ReactiveCommand.Create();
            SelectHudCommand.Subscribe(x => HudType = (HudType == HudType.Plain) ? HudType.Default : HudType.Plain);
        }

        private void InitializeObservables()
        {
            this.ObservableForProperty(x => x.CurrentTableLayout).Select(x => true)
                .Merge(this.ObservableForProperty(x => x.GameType).Select(x => true))
                .Merge(this.ObservableForProperty(x => x.HudType).Select(x => true))
                .Subscribe(x =>
                {
                    var savedLayouts = hudLayoutsSevice.Layouts.Layouts.Where(y => y.LayoutId == CurrentViewModelHash);

                    Layouts.Clear();
                    Layouts.AddRange(savedLayouts);

                    UpdateActiveLayout();

                    RaisePropertyChanged(() => HudType);
                });

            this.ObservableForProperty(x => x.HudViewType).Select(x => true)
                .Subscribe(x =>
                {
                    var isVertical = HudViewType == HudViewType.Vertical;

                    foreach (var hudTableViewModel in hudTableViewModelDictionary.Values)
                    {
                        hudTableViewModel.HudElements.ForEach(h => h.IsVertical = isVertical);
                    }

                    previewHudElementViewModel.IsVertical = isVertical;
                });

            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                                                 h => StatInfoObserveCollection.CollectionChanged += h,
                                                 h => StatInfoObserveCollection.CollectionChanged -= h).
                                                Subscribe(x =>
                                                {
                                                    var hudElements = x.Sender as ObservableCollection<StatInfo>;

                                                    if (hudElements == null)
                                                    {
                                                        return;
                                                    }

                                                    var statsToHide = StatInfoCollection.Where(s => hudElements.Any(h => h != s && h.Stat == s.Stat));
                                                    StatInfoCollection.Where(s => s.IsDuplicateSelected && !statsToHide.Contains(s)).ForEach(s => s.IsDuplicateSelected = false);
                                                    statsToHide.ForEach(s => s.IsDuplicateSelected = true);

                                                    HudTableViewModelCurrent.HudElements.ForEach(o =>
                                                    {
                                                        o.StatInfoCollection.Clear();
                                                        o.StatInfoCollection.AddRange(hudElements);
                                                        o.UpdateMainStats();

                                                    });

                                                    if (PreviewHudElementViewModel != null)
                                                    {
                                                        PreviewHudElementViewModel.StatInfoCollection.Clear();
                                                        PreviewHudElementViewModel.StatInfoCollection.AddRange(hudElements);
                                                        PreviewHudElementViewModel.UpdateMainStats();
                                                    }

                                                    if (!isUpdatingLayout)
                                                    {
                                                        SaveCurrentLayout();
                                                    }
                                                });

        }

        #region Properties

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private bool isStarted;

        private ObservableCollection<PlayerHudContent> playerCollection;

        private ReactiveList<StatInfo> statInfoCollection;
        private CollectionView statInfoCollectionView;

        private StatInfoObservableCollection<StatInfo> statInfoObserveCollection;
        private StatInfo statInfoObserveSelectedItem;

        private Dictionary<int, HudTableViewModel> hudTableViewModelDictionary;

        private HudTableLayoutMenuItem currentTableLayout;

        public HudTableLayoutMenuItem CurrentTableLayout
        {
            get
            {
                return currentTableLayout;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref currentTableLayout, value);
            }
        }

        private ObservableCollection<IDropDownMenuItem> tableLayouts;

        public ObservableCollection<IDropDownMenuItem> TableLayouts
        {
            get
            {
                return tableLayouts;
            }
        }

        public ObservableCollection<PlayerHudContent> PlayerCollection
        {
            get
            {
                return playerCollection;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref playerCollection, value);
            }
        }

        public ReactiveList<StatInfo> StatInfoCollection
        {
            get
            {
                return statInfoCollection;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref statInfoCollection, value);

                CollectionView collectionViewSource = (CollectionView)CollectionViewSource.GetDefaultView(statInfoCollection);

                collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription("StatInfoGroup.Name"));

                collectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription("GroupName", System.ComponentModel.ListSortDirection.Ascending));
                collectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription("Caption", System.ComponentModel.ListSortDirection.Ascending));

                collectionViewSource.Filter = (item) =>
                {
                    var stat = item as StatInfo;
                    if (stat == null)
                        return false;

                    return stat.IsListed && !stat.IsDuplicateSelected;
                };

                var statFiltering = collectionViewSource as ICollectionViewLiveShaping;
                if (statFiltering.CanChangeLiveFiltering)
                {
                    statFiltering.LiveFilteringProperties.Add(nameof(StatInfo.IsDuplicateSelected));
                    statFiltering.IsLiveFiltering = true;
                }

                StatInfoCollectionView = collectionViewSource;
            }
        }

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

        public StatInfoObservableCollection<StatInfo> StatInfoObserveCollection
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

        public Dictionary<int, HudTableViewModel> HudTableViewModelDictionary
        {
            get
            {
                return hudTableViewModelDictionary;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudTableViewModelDictionary, value);
            }
        }

        public HudTableViewModel HudTableViewModelCurrent
        {
            get
            {
                return HudTableViewModelDictionary.ContainsKey(CurrentViewModelHash) ? HudTableViewModelDictionary[CurrentViewModelHash] : null;
            }
            private set
            {
                if (HudTableViewModelDictionary.ContainsKey(CurrentViewModelHash))
                {
                    HudTableViewModelDictionary[CurrentViewModelHash] = value;
                    RaisePropertyChanged(() => HudTableViewModelCurrent);
                }
            }
        }

        public bool IsPreferredSeatingEnabled
        {
            get
            {
                return TableSeatAreaHelpers.GetSeatSetting(CurrentTableLayout.HudTableLayout.TableType, CurrentTableLayout.HudTableLayout.Site).IsPreferredSeatEnabled;
            }
            set
            {
                if (HudTableViewModelCurrent == null)
                {
                    return;
                }

                /* temporary disable for bet online */
                if (HudTableViewModelCurrent.TableLayout.Site == EnumPokerSites.BetOnline)
                {
                    if (value)
                    {
                        DisablePreferredSeatBetOnline();
                        return;
                    }
                }

                UpdateSeatContextMenuState(value);
                UpdateSeatSetting(value);
                RaisePropertyChanged(() => IsPreferredSeatingEnabled);
                if (value)
                {
                    ShowSeatingInstruction();
                }
            }
        }

        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isStarted, value);
            }
        }

        private HudType hudType;

        public HudType HudType
        {
            get
            {
                return CurrentTableLayout?.HudTableLayout.Site == EnumPokerSites.Bovada ? hudType : HudType.Plain;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudType, value);
                this.RaisePropertyChanged(() => SelectedHUD);
            }
        }

        private HudViewType hudViewType;

        public HudViewType HudViewType
        {
            get
            {
                return hudViewType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudViewType, value);
            }
        }

        private ObservableCollection<HudViewType> hudViewTypes;

        public ObservableCollection<HudViewType> HudViewTypes
        {
            get
            {
                return hudViewTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref hudViewTypes, value);
            }
        }

        private EnumGameTypeWrapper gameType;

        public EnumGameTypeWrapper GameType
        {
            get
            {
                return gameType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref gameType, value);
            }
        }

        private List<EnumGameTypeWrapper> gameTypes;

        public List<EnumGameTypeWrapper> GameTypes
        {
            get
            {
                return gameTypes;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref gameTypes, value);
            }
        }

        public string SelectedHUD
        {
            get
            {
                return HudType == HudType.Default ?
                        CommonResourceManager.Instance.GetResourceString("Common_PlainHUD") :
                        CommonResourceManager.Instance.GetResourceString("Common_DriveHUD");
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

        private ObservableCollection<HudSavedLayout> layouts;

        public ObservableCollection<HudSavedLayout> Layouts
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

        private HudSavedLayout currentLayout;

        public HudSavedLayout CurrentLayout
        {
            get
            {
                return currentLayout;
            }
            set
            {
                SaveCurrentLayout();

                this.RaiseAndSetIfChanged(ref currentLayout, value);

                // load data for selected layout
                DataLoad();
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

        public ReactiveCommand<object> SelectHudCommand { get; private set; }

        #endregion

        #region Infrastructure

        IImporterService importerService = ServiceLocator.Current.GetInstance<IImporterService>();

        public void Start()
        {
            importerService.StartImport();

            IsStarted = true;
        }

        public void Stop()
        {
            IsStarted = false;

            importerService.StopImport();

            var tableService = ServiceLocator.Current.GetInstance<IBetOnlineTableService>();
            tableService.Reset();

            HudPainter.ReleaseHook();
        }

        private void OpenDataSave()
        {
            var hudSelectLayoutViewModelInfo = new HudSelectLayoutViewModelInfo
            {
                LayoutId = CurrentViewModelHash,
                Cancel = ClosePopup,
                Save = DataSave,
                IsSaveAsMode = true
            };

            var hudSelectLayoutViewModel = new HudSelectLayoutViewModel(hudSelectLayoutViewModelInfo);

            OpenPopup(hudSelectLayoutViewModel);
        }

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
                LayoutId = CurrentViewModelHash,
                Name = hudSelectLayoutViewModel.Name,
                HudTable = HudTableViewModelCurrent,
                Stats = StatInfoObserveCollection
            };

            var savedLayout = hudLayoutsSevice.SaveAs(hudData);

            if (savedLayout != null && savedLayout != CurrentLayout)
            {
                Layouts.Add(savedLayout);
                CurrentLayout = savedLayout;
            }

            ClosePopup();
        }

        private void DataExport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "HUD Layouts (.xml)|*.xml" };

            if (saveFileDialog.ShowDialog() == true)
            {
                var hudData = new HudSavedDataInfo
                {
                    LayoutId = currentLayout.LayoutId,
                    Name = currentLayout.Name,
                    HudTable = HudTableViewModelDictionary[currentLayout.LayoutId],
                    Stats = StatInfoObserveCollection
                };

                hudLayoutsSevice.Export(hudData, saveFileDialog.FileName);
            }
        }

        private void DataImport()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "HUD Layouts (.xml)|*.xml" };

            if (openFileDialog.ShowDialog() == true)
            {
                var importedLayout = hudLayoutsSevice.Import(openFileDialog.FileName);

                if (importedLayout == null)
                {
                    return;
                }

                if (importedLayout.LayoutId == CurrentViewModelHash)
                {
                    Layouts.Add(importedLayout);
                    CurrentLayout = importedLayout;
                }
            }
        }

        private void OpenDataLoad()
        {
            var viewModelInfo = new HudSelectLayoutViewModelInfo
            {
                LayoutId = CurrentViewModelHash,
                Cancel = ClosePopup,
                Save = DataLoad
            };

            var hudSelectLayoutViewModel = new HudSelectLayoutViewModel(viewModelInfo);

            OpenPopup(hudSelectLayoutViewModel);
        }

        private void DataLoad()
        {
            if (CurrentLayout == null)
            {
                return;
            }

            hudLayoutsSevice.SetLayoutActive(CurrentLayout);

            if (HudTableViewModelCurrent == null)
            {
                return;
            }

            MergeLayouts(HudTableViewModelCurrent.HudElements, CurrentLayout);

            UpdateLayout(CurrentLayout);
        }

        private void DataDelete()
        {
            var loadedLayout = hudLayoutsSevice.GetLayoutByName(CurrentLayout.Name, CurrentViewModelHash);

            HudSavedLayout activeLayout;

            if (hudLayoutsSevice.Delete(loadedLayout, out activeLayout))
            {
                Layouts.Remove(loadedLayout);
            }

            if (activeLayout != null)
            {
                CurrentLayout = activeLayout;
            }
        }

        private void MergeLayouts(IEnumerable<HudElementViewModel> hudElementViewModels, HudSavedLayout layout)
        {
            Check.ArgumentNotNull(() => hudElementViewModels);
            Check.ArgumentNotNull(() => layout);

            var hudElementsToUpdate = (from hudElement in hudElementViewModels
                                       join hudLayoutElement in layout.HudPositions on new { hudElement.Seat, hudElement.HudType } equals new { hudLayoutElement.Seat, hudLayoutElement.HudType } into gj
                                       from grouped in gj.DefaultIfEmpty()
                                       where grouped != null
                                       select new { HudElement = hudElement, HudSavedPosition = grouped }).ToArray();

            hudElementsToUpdate.ForEach(h =>
            {
                h.HudElement.Width = h.HudSavedPosition.Width;
                h.HudElement.Height = h.HudSavedPosition.Height;
                h.HudElement.Position = h.HudSavedPosition.Position;
            });
        }

        private void UpdateActiveLayout()
        {
            var activeLayout = hudLayoutsSevice.GetActiveLayout(CurrentViewModelHash);
            CurrentLayout = activeLayout;
        }

        private void SaveCurrentLayout()
        {
            if (currentLayout == null)
            {
                return;
            }

            // save layout in cache
            var hudData = new HudSavedDataInfo
            {
                LayoutId = currentLayout.LayoutId,
                Name = currentLayout.Name,
                HudTable = HudTableViewModelDictionary[currentLayout.LayoutId],
                Stats = StatInfoObserveCollection
            };

            hudLayoutsSevice.Save(hudData);
        }

        private void UpdateLayout(HudSavedLayout activeLayout)
        {
            if (activeLayout == null)
            {
                return;
            }

            isUpdatingLayout = true;

            // Get all chosen stats back to list
            foreach (var statInfo in StatInfoObserveCollection)
            {
                if (statInfo is StatInfoBreak)
                {
                    continue;
                }

                statInfo.Reset();
                statInfo.Initialize();

                StatInfoCollection.Add(statInfo);
            }

            StatInfoObserveCollection.Clear();

            foreach (var hudStat in activeLayout.HudStats)
            {
                if (hudStat is StatInfoBreak)
                {
                    StatInfoObserveCollection.Add(hudStat);
                    continue;
                }

                var existing = StatInfoCollection.FirstOrDefault(x => x.Stat == hudStat.Stat && x.StatInfoGroup.Name == hudStat.StatInfoGroup.Name);

                if (existing != null)
                {
                    // we do not recover unexpected stats
                    StatInfoObserveCollection.Add(hudStat);
                    StatInfoCollection.Remove(existing);
                }
            }

            isUpdatingLayout = false;
        }

        private void SpliterAdd()
        {
            StatInfoObserveCollection.Add(new StatInfoBreak());
        }

        /// <summary>
        /// Open pop-up and initialize stat info settings
        /// </summary>
        /// <param name="selectedStatInfo">Selected stat info</param>
        private void OpenStatsSettings(StatInfo selectedStatInfo)
        {
            if (StatInfoObserveCollection.Count == 0)
            {
                return;
            }

            var hudStatSettingsViewModelInfo = new HudStatSettingsViewModelInfo
            {
                SelectedStatInfo = selectedStatInfo,
                SelectedStatInfoCollection = StatInfoObserveCollection,
                Save = SaveStatsSettings,
                Cancel = ClosePopup
            };

            var hudStatSettingsViewModel = new HudStatSettingsViewModel(hudStatSettingsViewModelInfo);

            OpenPopup(hudStatSettingsViewModel);
        }

        /// <summary>
        /// Close pop-up and save data
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

            foreach (var mergeItem in statInfoToMerge)
            {
                mergeItem.OldItem.Merge(mergeItem.NewItem);
            }

            ClosePopup();
        }

        /// <summary>
        /// Open pop-up and initialize player types 
        /// </summary>
        /// <param name="selectedStatInfo"></param>
        private void OpenPlayerTypeStats(StatInfo selectedStatInfo)
        {
            if (StatInfoObserveCollection.Count == 0 ||
                CurrentLayout == null || CurrentLayout.HudPlayerTypes == null)
            {
                return;
            }

            var hudPlayerSettingsViewModelInfo = new HudPlayerSettingsViewModelInfo
            {
                PlayerTypes = CurrentLayout.HudPlayerTypes.Select(x => x.Clone()),
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
            if (StatInfoObserveCollection.Count == 0 ||
               CurrentLayout == null || CurrentLayout.HudPlayerTypes == null)
            {
                return;
            }

            var hudBumperStickersSettingsViewModelInfo = new HudBumperStickersSettingsViewModelInfo
            {
                BumperStickers = CurrentLayout.HudBumperStickerTypes.Select(x => x.Clone()),
                Save = BumperStickerSave
            };

            var hudBumperStickersViewModel = new HudBumperStickersSettingsViewModel(hudBumperStickersSettingsViewModelInfo);

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
            var stickersTypesToMerge = (from stickerType in hudStickersSettingsViewModel.BumperStickers
                                        join currentStickerType in CurrentLayout.HudBumperStickerTypes on stickerType.Name equals currentStickerType.Name into stgj
                                        from stgrouped in stgj.DefaultIfEmpty()
                                        where stgrouped != null
                                        select new { CurrentStickerType = stickerType, StickerType = stgrouped }).ToArray();

            stickersTypesToMerge.ForEach(st =>
            {
                st.CurrentStickerType.MinSample = st.StickerType.MinSample;
                st.CurrentStickerType.EnableBumperSticker = st.StickerType.EnableBumperSticker;
                st.CurrentStickerType.SelectedColor = st.StickerType.SelectedColor;
                st.CurrentStickerType.Name = st.StickerType.Name;

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

            ClosePopup();
        }

        private int CurrentViewModelHash
        {
            get { return GetHash(CurrentTableLayout.HudTableLayout.Site, GameType.GameType, CurrentTableLayout.HudTableLayout.TableType); }
        }

        public static int GetHash(HudTableLayout hudTableLayout, EnumGameType gameType)
        {
            if (hudTableLayout != null)
            {
                return GetHash(hudTableLayout.Site, gameType, hudTableLayout.TableType);
            }

            LogProvider.Log.Error("hudTableLayout is not defined. Using defaults values");

            return GetHash(EnumPokerSites.Bovada, gameType, EnumTableType.Six);
        }

        public static int GetHash(EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            unchecked
            {
                int hashCode = (int)2166136261;

                hashCode = (hashCode * 16777619) ^ pokerSite.GetHashCode();
                hashCode = (hashCode * 16777619) ^ gameType.GetHashCode();
                hashCode = (hashCode * 16777619) ^ tableType.GetHashCode();

                return hashCode;
            }
        }

        private void UpdateSeatSetting(bool isPreferredSeatEnabled)
        {
            var pokerSite = CurrentTableLayout.HudTableLayout.Site;
            var tableType = CurrentTableLayout.HudTableLayout.TableType;

            var settings = settingsService.GetSettings();
            var preferredSettings = settings.SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == pokerSite);

            var currentSeatSetting = preferredSettings?.PrefferedSeats?.FirstOrDefault(x => x.TableType == tableType);
            if (currentSeatSetting == null)
            {
                currentSeatSetting = new PreferredSeatModel() { TableType = tableType, IsPreferredSeatEnabled = isPreferredSeatEnabled, PreferredSeat = -1 };
                preferredSettings.PrefferedSeats.Add(currentSeatSetting);
            }

            currentSeatSetting.IsPreferredSeatEnabled = isPreferredSeatEnabled;
            settingsService.SaveSettings(settings);
        }

        private void ShowSeatingInstruction()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                this.NotificationRequest.Raise(
                    new PopupActionNotification
                    {
                        Content = "Right click on seat position at the table, and then select Choose Seat, to select this seat as your preferred seat.",
                        Title = "Preferred Seating",
                    },
                    n => { });
            });
        }

        internal void UpdateSeatContextMenuState()
        {
            var tableSeatSetting = TableSeatAreaHelpers.GetSeatSetting(CurrentTableLayout.HudTableLayout.TableType, CurrentTableLayout.HudTableLayout.Site);
            UpdateSeatContextMenuState(tableSeatSetting.IsPreferredSeatEnabled);
            RaisePropertyChanged(() => IsPreferredSeatingEnabled);
        }

        private void UpdateSeatContextMenuState(bool isEnabled)
        {
            if (HudTableViewModelCurrent != null && HudTableViewModelCurrent.TableSeatAreaCollection != null)
            {
                HudTableViewModelCurrent.TableSeatAreaCollection?.Where(x => x != null).ForEach(x => x.SetContextMenuEnabled(isEnabled));
            }
        }

        private void OnPreferredSeatChanged(PreferredSeatChangedEventArgs obj)
        {
            if (obj == null)
            {
                return;
            }

            foreach (var seat in HudTableViewModelCurrent.TableSeatAreaCollection.Where(x => x.SeatNumber != obj.SeatNumber))
            {
                seat.IsPreferredSeat = false;
            }
        }

        // temporary 
        private void DisablePreferredSeatBetOnline()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                this.NotificationRequest.Raise(
                    new PopupActionNotification
                    {
                        Content = "BetOnline does not currently support preferred seating.",
                        Title = "Preferred Seating",
                    },
                    n => { });

                IsPreferredSeatingEnabled = false;
            });
        }

        internal void RefreshHudTable()
        {
            this.RaisePropertyChanged(nameof(CurrentTableLayout));
        }

        #endregion
    }
}