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
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
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
using Model.Site;
using Prism.Interactivity.InteractionRequest;
using Model.Settings;
using Model.Events;
using DriveHUD.Entities;
using System.ComponentModel;
using Model.Filters;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common;
using Microsoft.Win32;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Class HUD View Model
    /// </summary>
    public class HudViewModel : PopupViewModelBase
    {
        private IHudLayoutsService _hudLayoutsSevice;
        private ISettingsService SettingsService => ServiceLocator.Current.GetInstance<ISettingsService>();

        private ObservableCollection<HudLayoutInfo> _layouts;

        public ObservableCollection<HudLayoutInfo> Layouts
        {
            get
            {
                return _layouts;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _layouts, value);
            }
        }

        private HudLayoutInfo _currentLayout;

        public HudLayoutInfo CurrentLayout
        {
            get { return _currentLayout; }
            set
            {
                if (CurrentLayout != null && _currentLayout != value)
                {
                    CurrentLayout.HudStats.Clear();
                    CurrentLayout.HudStats.AddRange(StatInfoObserveCollection.Select(s => s.Clone()));
                }

                _currentLayoutSwitching = true;

                this.RaiseAndSetIfChanged(ref _currentLayout, value);

                if (_currentLayout != null)
                {
                    previewHudElementViewModel.HudViewType = HudViewType;
                    CurrentTableType = TableTypes.FirstOrDefault(t => t.TableType == _currentLayout.TableType);
                }

                var playerIcon = StatInfoCollectionView.SourceCollection.OfType<StatInfo>().FirstOrDefault(x=>x.Stat == Stat.Blank);
                if (playerIcon != null)
                {
                    playerIcon.IsVisible = HudViewType == HudViewType.Plain;
                }
                playerIcon = StatInfoObserveCollection.FirstOrDefault(x => x.Stat == Stat.Blank);
                if (playerIcon != null)
                {
                    playerIcon.IsVisible = HudViewType == HudViewType.Plain;
                }

                // load data for selected layout
                RaisePropertyChanged(() => HudViewType);
                _currentLayoutSwitching = false;
                DataLoad(true);
            }
        }

        #region Commands

        public ReactiveCommand<object> DataSaveCommand { get; private set; }

        public ReactiveCommand<object> DataDeleteCommand { get; private set; }

        public ReactiveCommand<object> DataImportCommand { get; private set; }

        public ReactiveCommand<object> DataExportCommand { get; private set; }

        public ReactiveCommand<object> SpliterAddCommand { get; private set; }

        public ReactiveCommand<object> SettingsStatInfoCommand { get; private set; }

        public ReactiveCommand<object> PlayerTypeStatsCommand { get; private set; }

        public ReactiveCommand<object> BumperStickersCommand { get; private set; }

        #endregion

        public HudViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize view model
        /// </summary>
        private void Initialize()
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<PreferredSeatChangedEvent>().Subscribe(OnPreferredSeatChanged);
            eventAggregator.GetEvent<UpdateHudEvent>().Subscribe(OnUpdateHudRaised);

            _hudLayoutsSevice = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            NotificationRequest = new InteractionRequest<INotification>();

            InitializeStatInfoCollection();
            InitializeStatInfoCollectionObserved();

            InitializePlayerCollection();

            InitializeTableLayouts();

            hudViewTypes = new ObservableCollection<HudViewType>(Enum.GetValues(typeof(HudViewType)).Cast<HudViewType>());

            var settings = SettingsService.GetSettings();

            PreviewHudElementViewModel = new HudElementViewModel { TiltMeter = 100, Opacity = 100 };
            PreviewHudElementViewModel.HudViewType = (HudViewType)settings.GeneralSettings.HudViewMode;

            InitializeCommands();
            InitializeObservables();

            InitializeHudElements();

            CurrentTableType = TableTypes.FirstOrDefault();
        }

        private void InitializeHudElements()
        {
            HudTableViewModels = new List<HudTableViewModel>();

            foreach (EnumTableType tableType in Enum.GetValues(typeof(EnumTableType)))
            {
                // generate hud table view model
                var hudTableViewModel = new HudTableViewModel
                {
                    TableType = tableType
                };

                HudTableViewModels.Add(hudTableViewModel);

                // initialize hud elements
                InitializeHudTableHudElements(hudTableViewModel, tableType);
            }
        }

        private void InitializeHudTableHudElements(HudTableViewModel hudTableViewModel, EnumTableType tableType)
        {
            if (hudTableViewModel == null)
            {
                return;
            }

            var hudElements = new ObservableCollection<HudElementViewModel>();

            var tableConfigurator = ServiceLocator.Current.GetInstance<ITableConfigurator>(HudViewType.ToString());

            var hudElementViewModels = tableConfigurator.GenerateElements((int)tableType).ToArray();
            hudElements.AddRange(hudElementViewModels);

            hudTableViewModel.HudElements = hudElements;
        }

        private void InitializeTableLayouts()
        {
            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            _configurations = configurationService.GetAll();
            TableTypes =
                new ObservableCollection<EnumTableTypeWrapper>(
                    Enum.GetValues(typeof(EnumTableType))
                        .OfType<EnumTableType>()
                        .Select(t => new EnumTableTypeWrapper(t)));
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
                new StatInfoGroup { Name = "Continuation Bet" },
                new StatInfoGroup { Name = "Limp" },
                new StatInfoGroup { Name = "Advanced Stats" },
            };

            // Make a collection of StatInfo
            StatInfoCollection = new ReactiveList<StatInfo>
            {
                new StatInfo { IsListed = false, GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.UO_PFR_EP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.UO_PFR_EP)},

                new StatInfo {GroupName = "1", StatInfoGroup = statInfoGroups[0], Caption = "Player Info Icon",Stat = Stat.Blank, IsVisible = true},

                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.VPIP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.VPIP) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.PFR, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFR)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S3Bet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.AF, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.Agg) },
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
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInEP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFRInEP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFRInMP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFRInCO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFRInBTN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFRInSB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.PFRInBB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.BTNDefendCORaise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.BTNDefendCORaise) },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetIP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetIP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetOOP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetOOP) },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInMP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInCO) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInBTN) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInSB) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetInBB) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetVsSteal, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.ThreeBetVsSteal) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.CBetInThreeBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetInThreeBetPot) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldToCBetFromThreeBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FoldFlopCBetFromThreeBetPot) },

                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetMP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInMP) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetCO, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInCO) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBTN, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInBTN) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetSB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInSB) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBB, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FourBetInBB) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.CBetInFourBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetInFourBetPot) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.FoldToCBetFromFourBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FoldFlopCBetFromFourBetPot) },

                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.WWSF, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.WSWSF) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FlopCheckRaise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopCheckRaise) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.CBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopCBet)},
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FloatFlop, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FloatFlop)},
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FlopAGG, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopAgg) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.RaiseFlop, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseFlop) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.CheckFoldFlopPfrOop, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CheckFoldFlopPfrOop) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.CheckFoldFlop3BetOop, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CheckFoldFlop3BetOop) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.BetFoldFlopPfrRaiser, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.BetFoldFlopPfrRaiser) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.BetFlopCalled3BetPreflopIp, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.BetFlopCalled3BetPreflopIp) },

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

                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.CBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FlopCBet) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FoldToCBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.FoldCBet) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.CBetIP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CBetIP) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.CBetOOP, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.CBetOOP) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.CBetInThreeBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetInThreeBetPot) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.CBetInFourBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetInFourBetPot) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FlopCBetVsOneOpp, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetVsOneOpp) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FlopCBetVsTwoOpp, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetVsTwoOpp) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FlopCBetMW, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetMW) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FlopCBetMonotone, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetMonotone) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FlopCBetRag, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FlopCBetRag) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FoldToCBetFromThreeBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FoldFlopCBetFromThreeBetPot) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.FoldToCBetFromFourBetPot, PropertyName = ReflectionHelper.GetPath<Indicators>(x=> x.FoldFlopCBetFromFourBetPot) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.RaiseCBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseCBet) },

                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.Limp, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimp) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.LimpCall, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimpCall) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.LimpFold, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimpFold) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.LimpReraise, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidLimpReraise) },

                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseFrequencyFactor, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.RaiseFrequencyFactor) },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.TrueAggression, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.TrueAggression) },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.DonkBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DonkBet) },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.DelayedTurnCBet, PropertyName = ReflectionHelper.GetPath<Indicators>(x => x.DidDelayedTurnCBet) },
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
        }

        private void InitializeObservables()
        {
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => StatInfoObserveCollection.CollectionChanged += h,
                h => StatInfoObserveCollection.CollectionChanged -= h).Subscribe(x =>
            {
                var hudElements = x.Sender as ObservableCollection<StatInfo>;

                if (hudElements == null)
                {
                    return;
                }

                var statsToHide = StatInfoCollection.Where(s => hudElements.Any(h => h != s && h.Stat == s.Stat));
                StatInfoCollection.Where(s => s.IsDuplicateSelected && !statsToHide.Contains(s))
                    .ForEach(s => s.IsDuplicateSelected = false);
                statsToHide.ForEach(s => s.IsDuplicateSelected = true);

                CurrentHudTableViewModel.HudElements.ForEach(o =>
                {
                    o.StatInfoCollection.Clear();
                    o.StatInfoCollection.AddRange(hudElements);
                    o.UpdateMainStats();

                });
                if (PreviewHudElementViewModel != null)
                {
                    PreviewHudElementViewModel.StatInfoCollection.Clear();

                    Random r = new Random();

                    for (int i = 0; i < hudElements.Count; i++)
                    {
                        if (hudElements[i] is StatInfoBreak)
                        {
                            PreviewHudElementViewModel.StatInfoCollection.Add((hudElements[i] as StatInfoBreak).Clone());
                            continue;
                        }

                        var stat = hudElements[i].Clone();
                        stat.CurrentValue = r.Next(0, 100);
                        stat.Caption = string.Format(stat.Format, stat.CurrentValue);
                        PreviewHudElementViewModel.StatInfoCollection.Add(stat);
                    }

                    PreviewHudElementViewModel.UpdateMainStats();
                }
            });

        }

        #region Properties

        public InteractionRequest<INotification> NotificationRequest { get; private set; }
        public event EventHandler TableUpdated;

        private ObservableCollection<PlayerHudContent> playerCollection;

        private ReactiveList<StatInfo> statInfoCollection;
        private CollectionView statInfoCollectionView;

        private StatInfoObservableCollection<StatInfo> statInfoObserveCollection;
        private StatInfo statInfoObserveSelectedItem;

        private EnumTableTypeWrapper _currentTableType;

        public EnumTableTypeWrapper CurrentTableType
        {
            get
            {
                return _currentTableType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _currentTableType, value);

                if (!_currentLayoutSwitching && CurrentTableType != null)
                {
                    Layouts = new ObservableCollection<HudLayoutInfo>(_hudLayoutsSevice.GetAllLayouts(CurrentTableType.TableType));
                    CurrentLayout = Layouts.FirstOrDefault();
                }
            }
        }

        public ObservableCollection<EnumTableTypeWrapper> TableTypes
        {
            get
            {
                return _tableTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref _tableTypes, value);
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

        public List<HudTableViewModel> HudTableViewModels
        {
            get { return _hudLayoutsSevice.HudTableViewModels; }
            set { _hudLayoutsSevice.HudTableViewModels = value; }
        }

        public HudTableViewModel CurrentHudTableViewModel { get; private set; }

        public HudViewType HudViewType => _currentLayout?.HudViewType ?? HudViewType.Plain;

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

        private IEnumerable<ISiteConfiguration> _configurations;
        private ObservableCollection<EnumTableTypeWrapper> _tableTypes;
        private bool _currentLayoutSwitching;

        #endregion

        #region Infrastructure

        private void OpenDataSave()
        {
            var hudSelectLayoutViewModelInfo = new HudSelectLayoutViewModelInfo
            {
                LayoutName = CurrentLayout.Name,
                Cancel = ClosePopup,
                Save = DataSave,
                IsSaveAsMode = true,
                TableType = CurrentTableType.TableType
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
                Name = hudSelectLayoutViewModel.Name,
                HudTable = CurrentHudTableViewModel,
                Stats = StatInfoObserveCollection,
                LayoutInfo = CurrentLayout
            };
            ClosePopup();
            var savedLayout = _hudLayoutsSevice.SaveAs(hudData);

            if (savedLayout != null && savedLayout.Name != CurrentLayout.Name)
            {
                Layouts.Add(savedLayout);
                CurrentLayout = savedLayout;
            }
        }

        private void DataExport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "HUD Layouts (.xml)|*.xml" };

            if (saveFileDialog.ShowDialog() == true)
                _hudLayoutsSevice.Export(CurrentLayout, saveFileDialog.FileName);
        }

        private void DataImport()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "HUD Layouts (.xml)|*.xml" };

            if (openFileDialog.ShowDialog() == true)
            {
                var importedLayout = _hudLayoutsSevice.Import(openFileDialog.FileName);

                if (importedLayout == null)
                    return;
                Layouts.Add(importedLayout);
                CurrentLayout = importedLayout;
            }
        }

        private void DataLoad(bool disableLayoutSave)
        {
            if (CurrentLayout == null)
            {
                return;
            }

            if (CurrentHudTableViewModel != null && !disableLayoutSave)
            {
                SaveLayoutChanged(CurrentLayout);
            }

            CurrentHudTableViewModel = HudTableViewModels.FirstOrDefault(h => h.TableType == CurrentTableType.TableType);

            if (CurrentHudTableViewModel == null)
            {
                return;
            }

            InitializeHudTableHudElements(CurrentHudTableViewModel, CurrentTableType.TableType);

            CurrentHudTableViewModel.HudViewType = HudViewType;
            CurrentHudTableViewModel.HudElements.ForEach(h => h.HudViewType = HudViewType);
            CurrentHudTableViewModel.Opacity = ((double)CurrentLayout.HudOpacity) / 100;
            CurrentHudTableViewModel.HudElements.ForEach(e => e.Opacity = CurrentHudTableViewModel.Opacity);
            //MergeLayouts(CurrentHudTableViewModel.HudElements, CurrentLayout);
            UpdateLayout(CurrentLayout);

            TableUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void MergeLayouts(IEnumerable<HudElementViewModel> hudElementViewModels, HudLayoutInfo layout)
        {
            Check.ArgumentNotNull(() => hudElementViewModels);
            Check.ArgumentNotNull(() => layout);

            foreach (var hudElementViewModel in hudElementViewModels)
            {
                var userDefinedPosition = layout.UiPositionsInfo.FirstOrDefault(p => p.Seat == hudElementViewModel.Seat);

                if (userDefinedPosition == null)
                {
                    continue;
                }

                hudElementViewModel.Width = userDefinedPosition.Width;
                hudElementViewModel.Height = userDefinedPosition.Height;
                hudElementViewModel.Position = userDefinedPosition.Position;
            }
        }

        private void DataDelete()
        {
            if (!_hudLayoutsSevice.Delete(CurrentLayout.Name))
            {
                return;
            }

            Layouts.Remove(CurrentLayout);
            UpdateActiveLayout();
        }

        public void UpdateActiveLayout()
        {
            CurrentLayout = Layouts.FirstOrDefault();
        }

        private void SaveLayoutChanged(HudLayoutInfo layoutToSave)
        {
            if (layoutToSave == null)
                return;
            layoutToSave.HudStats = StatInfoObserveCollection.Select(x =>
            {
                var statInfoBreak = x as StatInfoBreak;

                return statInfoBreak != null ? statInfoBreak.Clone() : x.Clone();
            }).ToList();
        }

        private void UpdateLayout(HudLayoutInfo layout)
        {
            if (layout == null)
                return;

            // Get all chosen stats back to list
            foreach (var statInfo in StatInfoObserveCollection)
            {
                if (statInfo is StatInfoBreak)
                {
                    continue;
                }

                if (statInfo.Stat == Stat.Blank)
                {
                    statInfo.IsVisible = HudViewType == HudViewType.Plain;
                    StatInfoCollection.Add(statInfo);
                }
                else
                {
                    statInfo.Reset();
                    statInfo.Initialize();
                }

                StatInfoCollection.Add(statInfo);
            }

            StatInfoObserveCollection.Clear();

            foreach (var hudStat in layout.HudStats)
            {
                if (hudStat is StatInfoBreak || hudStat.Stat == Stat.Blank)
                {
                    StatInfoObserveCollection.Add(hudStat);
                    continue;
                }

                var existing =
                    StatInfoCollection.FirstOrDefault(
                        x => x.Stat == hudStat.Stat && x.StatInfoGroup?.Name == hudStat.StatInfoGroup.Name);

                if (existing != null)
                {
                    // we do not recover unexpected stats
                    StatInfoObserveCollection.Add(hudStat);
                    StatInfoCollection.Remove(existing);
                }
            }
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
            var opacity = CurrentLayout?.HudOpacity ?? 0;
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
            CurrentLayout.HudOpacity = hudStatSettings.HudOpacity;
            foreach (var mergeItem in statInfoToMerge)
            {
                mergeItem.OldItem.Merge(mergeItem.NewItem);

                var previewStat = PreviewHudElementViewModel.StatInfoCollection.FirstOrDefault(x => x.Stat == mergeItem.NewItem.Stat);
                previewStat?.Merge(mergeItem.NewItem);
                previewStat?.UpdateColor();
            }
            CurrentHudTableViewModel.Opacity = ((double)CurrentLayout.HudOpacity) / 100;
            CurrentHudTableViewModel.HudElements.ForEach(e => e.Opacity = CurrentHudTableViewModel.Opacity);

            if (CurrentLayout.IsDefault)
            {
                foreach (var table in HudTableViewModels.Where(t => t.TableType == CurrentTableType.TableType))
                {
                    table.HudElements.ForEach(e => e.Opacity = CurrentHudTableViewModel.Opacity);
                    table.Opacity = CurrentHudTableViewModel.Opacity;
                }
            }

            TableUpdated?.Invoke(this, EventArgs.Empty);
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

            //_hudLayoutsSevice.SaveBumperStickers(_currentLayoutInfo);

            ClosePopup();
        }

        private void OnPreferredSeatChanged(PreferredSeatChangedEventArgs obj)
        {
            if (obj == null)
            {
                return;
            }

            foreach (var seat in CurrentHudTableViewModel.TableSeatAreaCollection.Where(x => x.SeatNumber != obj.SeatNumber))
            {
                seat.IsPreferredSeat = false;
            }
        }

        internal void RefreshHudTable()
        {
            this.RaisePropertyChanged(nameof(CurrentTableType));
        }


        private void OnUpdateHudRaised(UpdateHudEventArgs obj)
        {
            if (obj == null)
                return;

            CurrentHudTableViewModel.HudElements.ForEach(x =>
            {
                x.Height = obj.Height;
                x.Width = obj.Width;
            });
        }

        #endregion
    }
}