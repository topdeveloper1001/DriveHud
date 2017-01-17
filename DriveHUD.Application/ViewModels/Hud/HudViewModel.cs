//-----------------------------------------------------------------------
// <copyright file="HudViewModel.cs" company="Ace Poker Solutions">
// Copyright � 2015 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Common.Resources;
using Model.Site;
using Prism.Interactivity.InteractionRequest;
using Model.Settings;
using Model.Events;
using DriveHUD.Common.Wpf.Actions;
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
        public EnumPokerSites DefaultPokerSite = EnumPokerSites.Poker888;
        public EnumGameType DefaultGameType = EnumGameType.CashHoldem;

        private IHudLayoutsService _hudLayoutsSevice;
        private ISettingsService SettingsService => ServiceLocator.Current.GetInstance<ISettingsService>();

        private HudTableDefinition CurrentTableDefinition { get; set; }
            

        private HudTableDefinedProperties CurrentLayoutTableDefinedProperties
            =>
            CurrentLayout?.HudTableDefinedProperties.FirstOrDefault(
                p => p.HudTableDefinition.Equals(CurrentTableDefinition));



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

            HudType = HudType.Plain;

            hudViewTypes = new ObservableCollection<HudViewType>(Enum.GetValues(typeof(HudViewType)).Cast<HudViewType>());

            var settings = SettingsService.GetSettings();

            hudViewType = HudViewType.Plain;
            lastDHHudViewType = (HudViewType)settings.GeneralSettings.HudViewMode;

            PreviewHudElementViewModel = new HudElementViewModel { TiltMeter = 100 };
            PreviewHudElementViewModel.HudViewType = (HudViewType)settings.GeneralSettings.HudViewMode;

            InitializeCommands();
            InitializeObservables();

            InitializeHudElements();
            InitializeLayouts();
        }

        private void InitializeHudElements()
        {
            HudTableViewModels = new List<HudTableViewModel>();

            // create HUD elements
            foreach (var pokerSite in _configurations.Select(c => c.Site))
            {
                foreach (var tableType in GetSiteTableTypes(pokerSite))
                {
                    foreach (EnumGameType gType in Enum.GetValues(typeof(EnumGameType)))
                    {
                        var hudElements = new ObservableCollection<HudElementViewModel>();

                        foreach (HudType hType in Enum.GetValues(typeof(HudType)))
                        {
                            var tableConfigurator =
                                ServiceLocator.Current.GetInstance<ITableConfigurator>(
                                    TableConfiguratorHelper.GetServiceName(
                                        pokerSite,
                                        hType));
                            var hudElementViewModels = tableConfigurator.GenerateElements((int) tableType).ToArray();

                            hudElementViewModels.ForEach(x => x.HudViewType = HudViewType);

                            hudElements.AddRange(hudElementViewModels);
                        }

                        // generate hud elements                        
                        var hudTableViewModel = new HudTableViewModel
                        {
                            HudElements = hudElements,
                            PokerSite = pokerSite,
                            TableType = tableType,
                            GameType = gType
                        };
                        HudTableViewModels.Add(hudTableViewModel);
                    }
                }
            }
        }

        private void InitializeLayouts()
        {
            // if no layouts were created then save defaults
            //if (hudLayoutsSevice.Layouts.Count < 1)
            //{
            //    hudLayoutsSevice.SaveDefaults(HudTableViewModelDictionary);
            //}
            //else
            //{
            //    var hudTablesToUpdate = (from hudTableViewModel in HudTableViewModelDictionary
            //                             join layout in hudLayoutsSevice.Layouts on hudTableViewModel.Key equals layout.LayoutId into gj
            //                             from grouped in gj.DefaultIfEmpty()
            //                             where grouped != null && grouped.IsDefault
            //                             select new { HudTableViewModel = hudTableViewModel.Value, Layout = grouped }).ToArray();

            //    var hudTablesToAdd = (from hudTableViewModel in HudTableViewModelDictionary
            //                          join layout in hudLayoutsSevice.Layouts on hudTableViewModel.Key equals layout.LayoutId into gj
            //                          from grouped in gj.DefaultIfEmpty()
            //                          where grouped == null
            //                          select hudTableViewModel).ToDictionary(x => x.Key, x => x.Value);

            //    if (hudTablesToAdd.Count > 0)
            //    {
            //        hudLayoutsSevice.SaveDefaults(hudTablesToAdd);
            //    }

            //    hudTablesToUpdate.ForEach(x =>
            //    {
            //        MergeLayouts(x.HudTableViewModel.HudElements, x.Layout);
            //    });
            //}

            //UpdateActiveLayout();

            _layouts = new ObservableCollection<HudLayoutInfo>(_hudLayoutsSevice.Layouts);
        }

        private void InitializeTableLayouts()
        {
            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            _configurations = configurationService.GetAll();
            CurrentPokerSite = null;
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

            SelectHudCommand = ReactiveCommand.Create();
            SelectHudCommand.Subscribe(x => SwitchHudType());

            SelectSiteAndGameTypeCommand = ReactiveCommand.Create();
            SelectSiteAndGameTypeCommand.Subscribe(x => SelectSiteAndGameType());
        }

        private void SelectSiteAndGameType()
        {
            var hudSelectSiteGameTypeViewModelInfo = new HudSelectSiteGameTypeViewModelInfo
            {
                PokerSite = CurrentPokerSite,
                GameType = CurrentGameType,
                Cancel = ClosePopup,
                Save = SetSiteGameType,
            };

            var hudSelectLayoutViewModel = new HudSelectSiteGameTypeViewModel(hudSelectSiteGameTypeViewModelInfo);

            OpenPopup(hudSelectLayoutViewModel);
        }

        private void SwitchHudType()
        {
            if (HudType == HudType.Plain)
            {
                HudType = HudType.Default;
                HudViewType = lastDHHudViewType;
            }
            else
            {
                HudType = HudType.Plain;
                HudViewType = HudViewType.Plain;
            }
        }

        private void InitializeObservables()
        {
            this.ObservableForProperty(x => x.HudViewType).Select(x => true)
                .Subscribe(x =>
                {
                    var settings = SettingsService.GetSettings();
                    settings.GeneralSettings.HudViewMode = (int)HudViewType;
                    SettingsService.SaveSettings(settings);

                    if (HudViewType == HudViewType.Plain)
                    {
                        HudType = HudType.Plain;
                        return;
                    }
                    else
                    {
                        HudType = HudType.Default;
                    }

                    CurrentHudTableViewModel.HudElements.ForEach(h => h.HudViewType = HudViewType);
                    previewHudElementViewModel.HudViewType = HudViewType;               
                });

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
        public event EventHandler TableUpdate;

        private bool isStarted;

        private ObservableCollection<PlayerHudContent> playerCollection;

        private ReactiveList<StatInfo> statInfoCollection;
        private CollectionView statInfoCollectionView;

        private StatInfoObservableCollection<StatInfo> statInfoObserveCollection;
        private StatInfo statInfoObserveSelectedItem;

        private EnumTableTypeWrapper _currentTableType;

        public EnumTableTypeWrapper CurrentTableType
        {
            get { return _currentTableType; }
            set
            {
                if (_currentTableType == value)
                    return;
                _currentTableType = value;
                this.RaisePropertyChanged(nameof(CurrentTableType));
                if (!_currentLayoutSwitching && CurrentTableType != null)
                    DataLoad(false);
            }
        }

        public ObservableCollection<EnumTableTypeWrapper> TableTypes
        {
            get { return _tableTypes; }
            private set
            {
                if (_tableTypes == value)
                    return;
                _tableTypes = value;
                this.RaisePropertyChanged(nameof(TableTypes));
                CurrentTableType = _tableTypes.FirstOrDefault();
            }
        }

        private EnumPokerSites? _currentPokerSite;

        public EnumPokerSites? CurrentPokerSite
        {
            get { return _currentPokerSite; }
            set
            {
                if (_currentPokerSite == value)
                    return;
                _currentPokerSite = value;
                if (_currentPokerSite == null)
                    TableTypes =
                        new ObservableCollection<EnumTableTypeWrapper>(
                            Enum.GetValues(typeof(EnumTableType))
                                .OfType<EnumTableType>()
                                .Select(t => new EnumTableTypeWrapper(t)));
                else
                    TableTypes =
                        new ObservableCollection<EnumTableTypeWrapper>(
                            GetSiteTableTypes(_currentPokerSite.Value).Select(t => new EnumTableTypeWrapper(t)));
                this.RaisePropertyChanged(nameof(CurrentPokerSite));
                if (CurrentPokerSite != EnumPokerSites.Ignition)
                    HudViewType = HudViewType.Plain;
                if (_currentLayoutSwitching || CurrentTableType == null)
                    return;
                DataLoad(false);
            }
        }

        private EnumGameType? _currentGameType;

        public EnumGameType? CurrentGameType
        {
            get { return _currentGameType; }
            set
            {
                if (_currentGameType == value)
                    return;
                _currentGameType = value;
                this.RaisePropertyChanged(nameof(CurrentGameType));
                if (!_currentLayoutSwitching && CurrentTableType != null)
                    DataLoad(false);
            }
        }

        private IEnumerable<EnumTableType> GetSiteTableTypes(EnumPokerSites pokerSite)
        {
            var configuration = _configurations.FirstOrDefault(c => c.Site == pokerSite);
            return configuration == null ? new List<EnumTableType>() : configuration.TableTypes;
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
            get { return CurrentPokerSite == EnumPokerSites.Ignition ? hudType : HudType.Plain; }
            set
            {
                this.RaiseAndSetIfChanged(ref hudType, value);
                this.RaisePropertyChanged(() => SelectedHUD);
                if (!_currentLayoutSwitching && CurrentTableType != null)
                    DataLoad(false);
            }
        }

        private HudViewType lastDHHudViewType;

        private HudViewType hudViewType;

        public HudViewType HudViewType
        {
            get
            {
                return hudViewType;
            }
            set
            {
                if (value == HudViewType.Plain)
                {
                    lastDHHudViewType = HudViewType;
                }

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
        private IEnumerable<ISiteConfiguration> _configurations;
        private ObservableCollection<EnumTableTypeWrapper> _tableTypes;
        private bool _currentLayoutSwitching;

        public HudLayoutInfo CurrentLayout
        {
            get { return _currentLayout; }
            set
            {
                if (CurrentLayout != null && CurrentTableDefinition != null && _currentLayout != value)
                {
                    var targetProps = GetTargetProperties(CurrentLayout, CurrentTableDefinition);
                    if (targetProps != null)
                    {
                        targetProps.HudStats.Clear();
                        targetProps.HudStats.AddRange(StatInfoObserveCollection.Select(s => s.Clone()));
                    }
                }
                this.RaiseAndSetIfChanged(ref _currentLayout, value);
                // load data for selected layout
                _currentLayoutSwitching = true;
                if (CurrentLayout != null &&
                    CurrentLayout.HudTableDefinedProperties.All(p => p.HudTableDefinition.PokerSite != CurrentPokerSite))
                    CurrentPokerSite =
                        CurrentLayout.HudTableDefinedProperties.FirstOrDefault()?.HudTableDefinition.PokerSite;
                if (CurrentLayout != null &&
                    CurrentLayout.HudTableDefinedProperties.All(p => p.HudTableDefinition.GameType != CurrentGameType))
                    CurrentGameType =
                        CurrentLayout.HudTableDefinedProperties.FirstOrDefault()?.HudTableDefinition.GameType;
                _currentLayoutSwitching = false;
                DataLoad(true);
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

        public ReactiveCommand<object> SelectSiteAndGameTypeCommand { get; private set; }

        #endregion

        #region Infrastructure
    
        private void OpenDataSave()
        {
            var layoutName = CurrentLayout.Name;
            
            var hudSelectLayoutViewModelInfo = new HudSelectLayoutViewModelInfo
            {
                LayoutName = layoutName,
  
                Cancel = ClosePopup,
                Save = DataSave,
                IsSaveAsMode = true
            };

            var hudSelectLayoutViewModel = new HudSelectLayoutViewModel(hudSelectLayoutViewModelInfo);

            OpenPopup(hudSelectLayoutViewModel);
        }

        private void SetSiteGameType()
        {
            var hudSelectSiteViewModel = PopupViewModel as HudSelectSiteGameTypeViewModel;
            if (hudSelectSiteViewModel == null)
            {
                ClosePopup();
                return;
            }
            CurrentPokerSite = hudSelectSiteViewModel.SelectedPokerSite?.PokerSite;
            CurrentGameType = hudSelectSiteViewModel.SelectedGameType?.GameType;
            ClosePopup();
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
                LayoutInfo = CurrentLayout,
                TableDefinition = CurrentTableDefinition
            };

            var savedLayout = _hudLayoutsSevice.SaveAs(hudData);

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
                return;
            var pokerSite = CurrentPokerSite ?? DefaultPokerSite;
            var gameType = CurrentGameType ?? DefaultGameType;
            if (CurrentHudTableViewModel != null && !disableLayoutSave)
                SaveLayoutChanged(CurrentLayout, CurrentTableDefinition);
            CurrentTableDefinition = new HudTableDefinition
            {
                PokerSite = CurrentPokerSite,
                GameType = CurrentGameType,
                TableType = CurrentTableType.TableType
            };
            CurrentHudTableViewModel =
                HudTableViewModels.FirstOrDefault(
                    h => h.PokerSite == pokerSite && h.TableType == CurrentTableType.TableType && h.GameType == gameType);
            if (CurrentHudTableViewModel == null)
                return;
            var targetHudProperties = GetTargetProperties(CurrentLayout, CurrentTableDefinition);
            if (targetHudProperties != null)
                CurrentHudTableViewModel.Opacity = ((double) targetHudProperties.HudOpacity)/100;
            MergeLayouts(CurrentTableDefinition, CurrentHudTableViewModel.HudElements, CurrentLayout);
            UpdateLayout(CurrentLayout);
            TableUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void DataDelete()
        {
            if (!_hudLayoutsSevice.Delete(CurrentLayout))
                return;
            Layouts.Remove(CurrentLayout);
            UpdateActiveLayout();
        }

        private HudTableDefinedProperties GetTargetProperties(HudLayoutInfo layout, HudTableDefinition tableDefinition)
        {
            var result =
                layout.HudTableDefinedProperties.FirstOrDefault(p => p.HudTableDefinition.Equals(tableDefinition));
            if (result == null && !_currentLayoutSwitching)
            {
                result =
                    layout.HudTableDefinedProperties.FirstOrDefault(
                        p =>
                            (p.HudTableDefinition.GameType == null ||
                             p.HudTableDefinition.GameType == tableDefinition.GameType) &&
                            (p.HudTableDefinition.PokerSite == null ||
                             p.HudTableDefinition.PokerSite == tableDefinition.PokerSite) &&
                            (p.HudTableDefinition.TableType == tableDefinition.TableType));
            }
            return result;
        }

        private void MergeLayouts(HudTableDefinition tableDefinition,
            IEnumerable<HudElementViewModel> hudElementViewModels, HudLayoutInfo layout)
        {
            Check.ArgumentNotNull(() => hudElementViewModels);
            Check.ArgumentNotNull(() => layout);
            if (!layout.IsDefault)
            {
                foreach (var layoutHudTableDefinedProperty in layout.HudTableDefinedProperties)
                {
                    layoutHudTableDefinedProperty.HudTableDefinition.GameType = tableDefinition.GameType;
                    layoutHudTableDefinedProperty.HudTableDefinition.PokerSite = tableDefinition.PokerSite;
                }
            }
            var targetHudProperties = GetTargetProperties(layout, tableDefinition);
            if (targetHudProperties == null)
                return;
            foreach (var hudElementViewModel in hudElementViewModels)
            {
                var userDefinedPosition =
                    targetHudProperties.HudPositions.FirstOrDefault(
                        p => p.HudType == hudElementViewModel.HudType && p.Seat == hudElementViewModel.Seat);
                if (userDefinedPosition == null)
                    continue;
                hudElementViewModel.Width = userDefinedPosition.Width;
                hudElementViewModel.Height = userDefinedPosition.Height;
                hudElementViewModel.Position = userDefinedPosition.Position;
            }
        }

        public void UpdateActiveLayout()
        {
            CurrentLayout = Layouts.FirstOrDefault();
        }

        private void SaveLayoutChanged(HudLayoutInfo layoutToSave, HudTableDefinition tableDefinition)
        {
            if (layoutToSave == null)
                return;
            var targetProps = GetTargetProperties(layoutToSave, tableDefinition);
            if (targetProps == null)
            {
                targetProps = new HudTableDefinedProperties { HudTableDefinition = tableDefinition };
                layoutToSave.HudTableDefinedProperties.Add(targetProps);
            }
            targetProps.HudStats = StatInfoObserveCollection.Select(x =>
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
                    continue;
                //statInfo.Reset();
                statInfo.Initialize();
                StatInfoCollection.Add(statInfo);
            }
            var targetProps = GetTargetProperties(layout, CurrentTableDefinition);
            StatInfoObserveCollection.Clear();
            if (targetProps == null)
                return;
            foreach (var hudStat in targetProps.HudStats)
            {
                if (hudStat is StatInfoBreak)
                {
                    StatInfoObserveCollection.Add(hudStat);
                    continue;
                }

                var existing =
                    StatInfoCollection.FirstOrDefault(
                        x => x.Stat == hudStat.Stat && x.StatInfoGroup.Name == hudStat.StatInfoGroup.Name);

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
            var targetProperties = GetTargetProperties(CurrentLayout, CurrentTableDefinition);
            var opacity = targetProperties?.HudOpacity ?? 0;
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
            var targetProperties = GetTargetProperties(CurrentLayout, CurrentTableDefinition);
            targetProperties.HudOpacity = hudStatSettings.HudOpacity;
            foreach (var mergeItem in statInfoToMerge)
            {
                mergeItem.OldItem.Merge(mergeItem.NewItem);

                var previewStat = PreviewHudElementViewModel.StatInfoCollection.FirstOrDefault(x => x.Stat == mergeItem.NewItem.Stat);
                previewStat?.Merge(mergeItem.NewItem);
                previewStat?.UpdateColor();
            }
            var targetHudProperties = GetTargetProperties(CurrentLayout, CurrentTableDefinition);
            CurrentHudTableViewModel.Opacity = ((double)targetHudProperties.HudOpacity) / 100;
            MergeLayouts(CurrentTableDefinition, CurrentHudTableViewModel.HudElements, CurrentLayout);
            TableUpdate?.Invoke(this, EventArgs.Empty);
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
            var hudPlayerTypes = CurrentLayoutTableDefinedProperties?.HudPlayerTypes;
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
            var hudBumperStickers = CurrentLayoutTableDefinedProperties?.HudBumperStickerTypes;
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
            var playerTypesToMerge = (from currentPlayerType in CurrentLayoutTableDefinedProperties.HudPlayerTypes
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
                                    join currentPlayerType in CurrentLayoutTableDefinedProperties.HudPlayerTypes on playerType.Name equals currentPlayerType.Name into ptgj
                                    from ptgrouped in ptgj.DefaultIfEmpty()
                                    where ptgrouped == null
                                    select new { AddedPlayerType = playerType }).ToArray();

            playerTypesToAdd.ForEach(pt =>
            {
                CurrentLayoutTableDefinedProperties.HudPlayerTypes.Add(pt.AddedPlayerType);
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
            var stickersTypesToMerge = (from currentStickerType in CurrentLayoutTableDefinedProperties.HudBumperStickerTypes
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
                                     join currentStickerType in CurrentLayoutTableDefinedProperties.HudBumperStickerTypes on stickerType.Name equals currentStickerType.Name into stgj
                                     from stgrouped in stgj.DefaultIfEmpty()
                                     where stgrouped == null
                                     select new { AddedPlayerType = stickerType }).ToArray();

            stickerTypesToAdd.ForEach(st =>
            {
                CurrentLayoutTableDefinedProperties.HudBumperStickerTypes.Add(st.AddedPlayerType);
            });

            CurrentLayoutTableDefinedProperties.HudBumperStickerTypes.ForEach(x => x.InitializeFilterPredicate());

            _hudLayoutsSevice.SaveBumperStickers(CurrentLayout, CurrentTableDefinition);

            ClosePopup();
        }

        private void UpdateSeatSetting(bool isPreferredSeatEnabled)
        {
            var settings = SettingsService.GetSettings();
            var preferredSettings =
                settings.SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == CurrentPokerSite);

            var currentSeatSetting =
                preferredSettings?.PrefferedSeats?.FirstOrDefault(x => x.TableType == CurrentTableType.TableType);
            if (currentSeatSetting == null)
            {
                currentSeatSetting = new PreferredSeatModel()
                {
                    TableType = CurrentTableType.TableType,
                    IsPreferredSeatEnabled = isPreferredSeatEnabled,
                    PreferredSeat = -1
                };
                preferredSettings.PrefferedSeats.Add(currentSeatSetting);
            }

            currentSeatSetting.IsPreferredSeatEnabled = isPreferredSeatEnabled;
            SettingsService.SaveSettings(settings);
        }

        private void ShowSeatingInstruction()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate
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
            var pokerSite = CurrentPokerSite.HasValue ? CurrentPokerSite.Value : DefaultPokerSite;
            var tableSeatSetting = TableSeatAreaHelpers.GetSeatSetting(CurrentTableType.TableType, pokerSite);
            UpdateSeatContextMenuState(tableSeatSetting.IsPreferredSeatEnabled);
        }

        private void UpdateSeatContextMenuState(bool isEnabled)
        {
            CurrentHudTableViewModel?.TableSeatAreaCollection?.Where(x => x != null).ForEach(x => x.SetContextMenuEnabled(isEnabled));
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