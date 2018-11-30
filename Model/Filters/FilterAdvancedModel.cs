//-----------------------------------------------------------------------
// <copyright file="FilterAdvancedModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterAdvancedModel : FilterBaseEntity, IFilterModel
    {
        public FilterAdvancedModel()
        {
            Name = "Advanced";
            Type = EnumFilterModelType.FilterAdvancedModel;
        }

        #region Properties 

        public EnumFilterModelType Type { get; }

        [XmlIgnore]
        public ObservableCollection<FilterAdvancedItem> Filters { get; set; } = new ObservableCollection<FilterAdvancedItem>();

        public ObservableCollection<FilterAdvancedItem> SelectedFilters { get; set; } = new ObservableCollection<FilterAdvancedItem>();

        #endregion

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();

            return predicate;
        }

        public void Initialize()
        {
            Filters = new ObservableCollection<FilterAdvancedItem>(GetFiltersItems());         
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (!(filter is FilterAdvancedModel filterModel))
            {
                return;
            }

            SelectedFilters.Clear();
            SelectedFilters.AddRange(filterModel.SelectedFilters.Select(x => x.Clone()));
        }

        public void ResetFilter()
        {
            SelectedFilters.Clear();
        }

        #region Initilization helpers 

        public readonly static HashSet<AdvancedFilterType> FiltersWithValueRequired = new HashSet<AdvancedFilterType>
        {
            AdvancedFilterType.BBsBetPreflopisBiggerThan,
            AdvancedFilterType.BBsBetPreflopisLessThan,
            AdvancedFilterType.BBsCalledPreflopisBiggerThan,
            AdvancedFilterType.BBsCalledPreflopisLessThan,
            AdvancedFilterType.BBsPutInPreflopisBiggerThan,
            AdvancedFilterType.BBsPutInPreflopisLessThan,
            AdvancedFilterType.PreflopRaiseSizePotisBiggerThan,
            AdvancedFilterType.PreflopRaiseSizePotisLessThan,
            AdvancedFilterType.PreflopFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.PreflopFacingRaiseSizePotisLessThan,
            AdvancedFilterType.PlayersonFlopisBiggerThan,
            AdvancedFilterType.PlayersonFlopisLessThan,
            AdvancedFilterType.PlayersonFlopisEqualTo,
            AdvancedFilterType.BBsBetFlopisBiggerThan,
            AdvancedFilterType.BBsBetFlopisLessThan,
            AdvancedFilterType.BBsCalledFlopisBiggerThan,
            AdvancedFilterType.BBsCalledFlopisLessThan,
            AdvancedFilterType.BBsPutinFlopisBiggerThan,
            AdvancedFilterType.BBsPutinFlopisLessThan,
            AdvancedFilterType.FlopPotSizeinBBsisBiggerThan,
            AdvancedFilterType.FlopPotSizeinBBsisLessThan,
            AdvancedFilterType.FlopStackPotRatioisBiggerThan,
            AdvancedFilterType.FlopStackPotRatioisLessThan,
            AdvancedFilterType.FlopBetSizePotisBiggerThan,
            AdvancedFilterType.FlopBetSizePotisLessThan,
            AdvancedFilterType.FlopRaiseSizePotisBiggerThan,
            AdvancedFilterType.FlopRaiseSizePotisLessThan,
            AdvancedFilterType.FlopFacingBetSizePotisBiggerThan,
            AdvancedFilterType.FlopFacingBetSizePotisLessThan,
            AdvancedFilterType.FlopFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.FlopFacingRaiseSizePotisLessThan,
            AdvancedFilterType.PlayersonTurnisBiggerThan,
            AdvancedFilterType.PlayersonTurnisLessThan,
            AdvancedFilterType.PlayersonTurnisEqualTo,
            AdvancedFilterType.BBsBetTurnisBiggerThan,
            AdvancedFilterType.BBsBetTurnisLessThan,
            AdvancedFilterType.BBsCalledTurnisBiggerThan,
            AdvancedFilterType.BBsCalledTurnisLessThan,
            AdvancedFilterType.BBsPutinTurnisBiggerThan,
            AdvancedFilterType.BBsPutinTurnisLessThan,
            AdvancedFilterType.TurnPotSizeinBBsisBiggerThan,
            AdvancedFilterType.TurnPotSizeinBBsisLessThan,
            AdvancedFilterType.TurnStackPotRatioisBiggerThan,
            AdvancedFilterType.TurnStackPotRatioisLessThan,
            AdvancedFilterType.TurnBetSizePotisBiggerThan,
            AdvancedFilterType.TurnBetSizePotisLessThan,
            AdvancedFilterType.TurnRaiseSizePotisBiggerThan,
            AdvancedFilterType.TurnRaiseSizePotisLessThan,
            AdvancedFilterType.TurnFacingBetSizePotisBiggerThan,
            AdvancedFilterType.TurnFacingBetSizePotisLessThan,
            AdvancedFilterType.TurnFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.TurnFacingRaiseSizePotisLessThan,
            AdvancedFilterType.PlayersonRiverisBiggerThan,
            AdvancedFilterType.PlayersonRiverisLessThan,
            AdvancedFilterType.PlayersonRiverisEqualTo,
            AdvancedFilterType.BBsBetRiverisBiggerThan,
            AdvancedFilterType.BBsBetRiverisLessThan,
            AdvancedFilterType.BBsCalledRiverisBiggerThan,
            AdvancedFilterType.BBsCalledRiverisLessThan,
            AdvancedFilterType.BBsPutinRiverisBiggerThan,
            AdvancedFilterType.BBsPutinRiverisLessThan,
            AdvancedFilterType.RiverPotSizeinBBsisBiggerThan,
            AdvancedFilterType.RiverPotSizeinBBsisLessThan,
            AdvancedFilterType.RiverStackPotRatioisBiggerThan,
            AdvancedFilterType.RiverStackPotRatioisLessThan,
            AdvancedFilterType.RiverBetSizePotisBiggerThan,
            AdvancedFilterType.RiverBetSizePotisLessThan,
            AdvancedFilterType.RiverRaiseSizePotisBiggerThan,
            AdvancedFilterType.RiverRaiseSizePotisLessThan,
            AdvancedFilterType.RiverFacingBetSizePotisBiggerThan,
            AdvancedFilterType.RiverFacingBetSizePotisLessThan,
            AdvancedFilterType.RiverFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.RiverFacingRaiseSizePotisLessThan,
            AdvancedFilterType.FinalPotSizeinBBsisBiggerThan,
            AdvancedFilterType.FinalPotSizeinBBsisLessThan,
            AdvancedFilterType.PlayerWonBBsIsBiggerThan,
            AdvancedFilterType.PlayerWonBBsIsLessThan,
            AdvancedFilterType.PlayerLostBBsIsBiggerThan,
            AdvancedFilterType.PlayerLostBBsIsLessThan,
            AdvancedFilterType.PlayerWonOrLostBBsIsBiggerThan,
            AdvancedFilterType.PlayerWonOrLostBBsIsLessThan,
            AdvancedFilterType.PlayersSawShowdownIsBiggerThan,
            AdvancedFilterType.PlayersSawShowdownIsLessThan,
            AdvancedFilterType.PlayersSawShowdownIsEqualTo,
            AdvancedFilterType.AllinWinIsBiggerThan,
            AdvancedFilterType.AllinWinIsLessThan
        };

        public readonly static HashSet<AdvancedFilterType> PercentBasedFilters = new HashSet<AdvancedFilterType>
        {
            AdvancedFilterType.PreflopRaiseSizePotisBiggerThan,
            AdvancedFilterType.PreflopRaiseSizePotisLessThan,
            AdvancedFilterType.PreflopFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.PreflopFacingRaiseSizePotisLessThan,
            AdvancedFilterType.FlopBetSizePotisBiggerThan,
            AdvancedFilterType.FlopBetSizePotisLessThan,
            AdvancedFilterType.FlopRaiseSizePotisBiggerThan,
            AdvancedFilterType.FlopRaiseSizePotisLessThan,
            AdvancedFilterType.FlopFacingBetSizePotisBiggerThan,
            AdvancedFilterType.FlopFacingBetSizePotisLessThan,
            AdvancedFilterType.FlopFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.FlopFacingRaiseSizePotisLessThan,
            AdvancedFilterType.TurnBetSizePotisBiggerThan,
            AdvancedFilterType.TurnBetSizePotisLessThan,
            AdvancedFilterType.TurnRaiseSizePotisBiggerThan,
            AdvancedFilterType.TurnRaiseSizePotisLessThan,
            AdvancedFilterType.TurnFacingBetSizePotisBiggerThan,
            AdvancedFilterType.TurnFacingBetSizePotisLessThan,
            AdvancedFilterType.TurnFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.TurnFacingRaiseSizePotisLessThan,
            AdvancedFilterType.RiverBetSizePotisBiggerThan,
            AdvancedFilterType.RiverBetSizePotisLessThan,
            AdvancedFilterType.RiverRaiseSizePotisBiggerThan,
            AdvancedFilterType.RiverRaiseSizePotisLessThan,
            AdvancedFilterType.RiverFacingBetSizePotisBiggerThan,
            AdvancedFilterType.RiverFacingBetSizePotisLessThan,
            AdvancedFilterType.RiverFacingRaiseSizePotisBiggerThan,
            AdvancedFilterType.RiverFacingRaiseSizePotisLessThan
        };

        private static IEnumerable<FilterAdvancedItem> GetFiltersItems()
        {
            return new[]
            {
                // PREFLOP
                Create(FilterAdvancedStageType.PreFlop, "VPIP", AdvancedFilterType.VPIP),
                Create(FilterAdvancedStageType.PreFlop, "Put $ in Pot", AdvancedFilterType.PutMoneyinPot),
                Create(FilterAdvancedStageType.PreFlop, "PFR", AdvancedFilterType.PFR),
                Create(FilterAdvancedStageType.PreFlop, "PFR = False", AdvancedFilterType.PFRFalse),
                Create(FilterAdvancedStageType.PreFlop, "Did 3-bet", AdvancedFilterType.Did3Bet),
                Create(FilterAdvancedStageType.PreFlop, "Did Squeeze", AdvancedFilterType.DidSqueeze),
                Create(FilterAdvancedStageType.PreFlop, "Did Cold Call", AdvancedFilterType.DidColdCall),
                Create(FilterAdvancedStageType.PreFlop, "Could Cold Call", AdvancedFilterType.CouldColdCall),
                Create(FilterAdvancedStageType.PreFlop, "Could Cold Call = False", AdvancedFilterType.CouldColdCallFalse),
                Create(FilterAdvancedStageType.PreFlop, "Faced Preflop 3 bet", AdvancedFilterType.FacedPreflop3Bet),
                Create(FilterAdvancedStageType.PreFlop, "Folded to Preflop 3 bet", AdvancedFilterType.FoldedToPreflop3Bet),
                Create(FilterAdvancedStageType.PreFlop, "Called Preflop 3 bet", AdvancedFilterType.CalledPreflop3Bet),
                Create(FilterAdvancedStageType.PreFlop, "Raised Preflop 3 bet", AdvancedFilterType.RaisedPreflop3Bet),
                Create(FilterAdvancedStageType.PreFlop, "Faced Preflop 4 bet", AdvancedFilterType.FacedPreflop4Bet),
                Create(FilterAdvancedStageType.PreFlop, "Folded to Preflop 4 bet", AdvancedFilterType.FoldedToPreflop4Bet),
                Create(FilterAdvancedStageType.PreFlop, "Called Preflop 4 bet", AdvancedFilterType.CalledPreflop4Bbet),
                Create(FilterAdvancedStageType.PreFlop, "Raised Preflop 4 bet", AdvancedFilterType.RaisedPreflop4Bet),
                Create(FilterAdvancedStageType.PreFlop, "In BB and Steal Attempted", AdvancedFilterType.InBBandStealAttempted),
                Create(FilterAdvancedStageType.PreFlop, "In BB and Steal Defended", AdvancedFilterType.InBBandStealDefended),
                Create(FilterAdvancedStageType.PreFlop, "In BB and Steal Reraised", AdvancedFilterType.InBBandStealReraised),
                Create(FilterAdvancedStageType.PreFlop, "In SB and Steal Attempted", AdvancedFilterType.InSBandStealAttempted),
                Create(FilterAdvancedStageType.PreFlop, "In SB and Steal Defended", AdvancedFilterType.InSBandStealDefended),
                Create(FilterAdvancedStageType.PreFlop, "In SB and Steal Reraised", AdvancedFilterType.InSBandStealReraised),
                Create(FilterAdvancedStageType.PreFlop, "Limp Reraised", AdvancedFilterType.LimpReraised),
                Create(FilterAdvancedStageType.PreFlop, "BBs Bet Preflop is Bigger Than...", AdvancedFilterType.BBsBetPreflopisBiggerThan),
                Create(FilterAdvancedStageType.PreFlop, "BBs Bet Preflop is Less Than...", AdvancedFilterType.BBsBetPreflopisLessThan),
                Create(FilterAdvancedStageType.PreFlop, "BBs Called Preflop is Bigger Than...", AdvancedFilterType.BBsCalledPreflopisBiggerThan),
                Create(FilterAdvancedStageType.PreFlop, "BBs Called Preflop is Less Than...", AdvancedFilterType.BBsCalledPreflopisLessThan),
                Create(FilterAdvancedStageType.PreFlop, "BBs Put In Preflop is Bigger Than...", AdvancedFilterType.BBsPutInPreflopisBiggerThan),
                Create(FilterAdvancedStageType.PreFlop, "BBs Put In Preflop is Less Than...", AdvancedFilterType.BBsPutInPreflopisLessThan),
                Create(FilterAdvancedStageType.PreFlop, "Preflop Raise Size / Pot is Bigger Than...", AdvancedFilterType.PreflopRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.PreFlop, "Preflop Raise Size / Pot is Less Than...", AdvancedFilterType.PreflopRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.PreFlop, "Preflop Facing Raise Size / Pot is Bigger Than...", AdvancedFilterType.PreflopFacingRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.PreFlop, "Preflop Facing Raise Size / Pot is Less Than...", AdvancedFilterType.PreflopFacingRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.PreFlop, "Allin Preflop", AdvancedFilterType.AllinPreflop),
                // FLOP
                Create(FilterAdvancedStageType.Flop, "Saw Flop", AdvancedFilterType.SawFlop),
                Create(FilterAdvancedStageType.Flop, "Saw Flop = False", AdvancedFilterType.SawFlopFalse),
                Create(FilterAdvancedStageType.Flop, "Last to Act on Flop", AdvancedFilterType.LasttoActionFlop),
                Create(FilterAdvancedStageType.Flop, "Last to Act on Flop = False", AdvancedFilterType.LasttoActionFlopFalse),
                Create(FilterAdvancedStageType.Flop, "Flop Unopened", AdvancedFilterType.FlopUnopened),
                Create(FilterAdvancedStageType.Flop, "Players on Flop is Bigger Than...", AdvancedFilterType.PlayersonFlopisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Players on Flop is Less Than...", AdvancedFilterType.PlayersonFlopisLessThan),
                Create(FilterAdvancedStageType.Flop, "Players on Flop is Equal To...", AdvancedFilterType.PlayersonFlopisEqualTo),
                Create(FilterAdvancedStageType.Flop, "Flop Continuation Bet Possible", AdvancedFilterType.FlopContinuationBetPossible),
                Create(FilterAdvancedStageType.Flop, "Flop Continuation Bet Made", AdvancedFilterType.FlopContinuationBetMade),
                Create(FilterAdvancedStageType.Flop, "Facing Flop Continuation Bet", AdvancedFilterType.FacingFlopContinuationBet),
                Create(FilterAdvancedStageType.Flop, "Folded to Flop Continuation Bet", AdvancedFilterType.FoldedtoFlopContinuationBet),
                Create(FilterAdvancedStageType.Flop, "Called Flop Continuation Bet", AdvancedFilterType.CalledFlopContinuationBet),
                Create(FilterAdvancedStageType.Flop, "Raised Flop Continuation Bet", AdvancedFilterType.RaisedFlopContinuationBet),
                Create(FilterAdvancedStageType.Flop, "Flop Bet", AdvancedFilterType.FlopBet),
                Create(FilterAdvancedStageType.Flop, "Flop Bet Fold", AdvancedFilterType.FlopBetFold),
                Create(FilterAdvancedStageType.Flop, "Flop Bet Call", AdvancedFilterType.FlopBetCall),
                Create(FilterAdvancedStageType.Flop, "Flop Bet Raise", AdvancedFilterType.FlopBetRaise),
                Create(FilterAdvancedStageType.Flop, "Flop Raise", AdvancedFilterType.FlopRaise),
                Create(FilterAdvancedStageType.Flop, "Flop Raise Fold", AdvancedFilterType.FlopRaiseFold),
                Create(FilterAdvancedStageType.Flop, "Flop Raise Call", AdvancedFilterType.FlopRaiseCall),
                Create(FilterAdvancedStageType.Flop, "Flop Raise Raise", AdvancedFilterType.FlopRaiseRaise),
                Create(FilterAdvancedStageType.Flop, "Flop Call", AdvancedFilterType.FlopCall),
                Create(FilterAdvancedStageType.Flop, "Flop Call Fold", AdvancedFilterType.FlopCallFold),
                Create(FilterAdvancedStageType.Flop, "Flop Call Call", AdvancedFilterType.FlopCallCall),
                Create(FilterAdvancedStageType.Flop, "Flop Call Raise", AdvancedFilterType.FlopCallRaise),
                Create(FilterAdvancedStageType.Flop, "Flop Check", AdvancedFilterType.FlopCheck),
                Create(FilterAdvancedStageType.Flop, "Flop Check Fold", AdvancedFilterType.FlopCheckFold),
                Create(FilterAdvancedStageType.Flop, "Flop Check Call", AdvancedFilterType.FlopCheckCall),
                Create(FilterAdvancedStageType.Flop, "Flop Check Raise", AdvancedFilterType.FlopCheckRaise),
                Create(FilterAdvancedStageType.Flop, "Flop Fold", AdvancedFilterType.FlopFold),
                Create(FilterAdvancedStageType.Flop, "Flop Was Check Raised", AdvancedFilterType.FlopWasCheckRaised),
                Create(FilterAdvancedStageType.Flop, "Flop Was Bet Into", AdvancedFilterType.FlopWasBetInto),
                Create(FilterAdvancedStageType.Flop, "Flop Was Raised", AdvancedFilterType.FlopWasRaised),
                Create(FilterAdvancedStageType.Flop, "BBs Bet Flop is Bigger Than...", AdvancedFilterType.BBsBetFlopisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "BBs Bet Flop is Less Than...", AdvancedFilterType.BBsBetFlopisLessThan),
                Create(FilterAdvancedStageType.Flop, "BBs Called Flop is Bigger Than...", AdvancedFilterType.BBsCalledFlopisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "BBs Called Flop is Less Than...", AdvancedFilterType.BBsCalledFlopisLessThan),
                Create(FilterAdvancedStageType.Flop, "BBs Put in Flop is Bigger Than...", AdvancedFilterType.BBsPutinFlopisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "BBs Put in Flop is Less Than...", AdvancedFilterType.BBsPutinFlopisLessThan),
                Create(FilterAdvancedStageType.Flop, "Flop Pot Size in BBs is Bigger Than...", AdvancedFilterType.FlopPotSizeinBBsisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Flop Pot Size in BBs is Less Than...", AdvancedFilterType.FlopPotSizeinBBsisLessThan),
                Create(FilterAdvancedStageType.Flop, "Flop Stack Pot Ratio is Bigger Than...", AdvancedFilterType.FlopStackPotRatioisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Flop Stack Pot Ratio is Less Than...", AdvancedFilterType.FlopStackPotRatioisLessThan),
                Create(FilterAdvancedStageType.Flop, "Flop Bet Size / Pot is Bigger Than...", AdvancedFilterType.FlopBetSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Flop Bet Size / Pot is Less Than...", AdvancedFilterType.FlopBetSizePotisLessThan),
                Create(FilterAdvancedStageType.Flop, "Flop Raise Size / Pot is Bigger Than...", AdvancedFilterType.FlopRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Flop Raise Size / Pot is Less Than...", AdvancedFilterType.FlopRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.Flop, "Flop Facing Bet Size / Pot is Bigger Than...", AdvancedFilterType.FlopFacingBetSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Flop Facing Bet Size / Pot is Less Than...", AdvancedFilterType.FlopFacingBetSizePotisLessThan),
                Create(FilterAdvancedStageType.Flop, "Flop Facing Raise Size / Pot is Bigger Than...", AdvancedFilterType.FlopFacingRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Flop, "Flop Facing Raise Size / Pot is Less Than...", AdvancedFilterType.FlopFacingRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.Flop, "Allin on Flop", AdvancedFilterType.AllinOnFlop),
                Create(FilterAdvancedStageType.Flop, "Allin on Flop (or earlier)", AdvancedFilterType.AllinOnFlopOrEarlier),
                // TURN
                Create(FilterAdvancedStageType.Turn, "Saw Turn", AdvancedFilterType.SawTurn),
                Create(FilterAdvancedStageType.Turn, "Last to Act on Turn", AdvancedFilterType.LasttoActonTurn),
                Create(FilterAdvancedStageType.Turn, "Last to Act on Turn = False", AdvancedFilterType.LasttoActonTurnFalse),
                Create(FilterAdvancedStageType.Turn, "Turn Unopened", AdvancedFilterType.TurnUnopened),
                Create(FilterAdvancedStageType.Turn, "Turn Unopened = False", AdvancedFilterType.TurnUnopenedFalse),
                Create(FilterAdvancedStageType.Turn, "Players on Turn is Bigger Than...", AdvancedFilterType.PlayersonTurnisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Players on Turn is Less Than...", AdvancedFilterType.PlayersonTurnisLessThan),
                Create(FilterAdvancedStageType.Turn, "Players on Turn is Equal To...", AdvancedFilterType.PlayersonTurnisEqualTo),
                Create(FilterAdvancedStageType.Turn, "Turn Continuation Bet Possible", AdvancedFilterType.TurnContinuationBetPossible),
                Create(FilterAdvancedStageType.Turn, "Turn Continuation Bet Made", AdvancedFilterType.TurnContinuationBetMade),
                Create(FilterAdvancedStageType.Turn, "Facing Turn Continuation Bet", AdvancedFilterType.FacingTurnContinuationBet),
                Create(FilterAdvancedStageType.Turn, "Folded to Turn Continuation Bet", AdvancedFilterType.FoldedtoTurnContinuationBet),
                Create(FilterAdvancedStageType.Turn, "Called Turn Continuation Bet", AdvancedFilterType.CalledTurnContinuationBet),
                Create(FilterAdvancedStageType.Turn, "Raised Turn Continuation Bet", AdvancedFilterType.RaisedTurnContinuationBet),
                Create(FilterAdvancedStageType.Turn, "Turn Bet", AdvancedFilterType.TurnBet),
                Create(FilterAdvancedStageType.Turn, "Turn Bet Fold", AdvancedFilterType.TurnBetFold),
                Create(FilterAdvancedStageType.Turn, "Turn Bet Call", AdvancedFilterType.TurnBetCall),
                Create(FilterAdvancedStageType.Turn, "Turn Bet Raise", AdvancedFilterType.TurnBetRaise),
                Create(FilterAdvancedStageType.Turn, "Turn Raise", AdvancedFilterType.TurnRaise),
                Create(FilterAdvancedStageType.Turn, "Turn Raise Fold", AdvancedFilterType.TurnRaiseFold),
                Create(FilterAdvancedStageType.Turn, "Turn Raise Call", AdvancedFilterType.TurnRaiseCall),
                Create(FilterAdvancedStageType.Turn, "Turn Raise Raise", AdvancedFilterType.TurnRaiseRaise),
                Create(FilterAdvancedStageType.Turn, "Turn Call", AdvancedFilterType.TurnCall),
                Create(FilterAdvancedStageType.Turn, "Turn Call Fold", AdvancedFilterType.TurnCallFold),
                Create(FilterAdvancedStageType.Turn, "Turn Call Call", AdvancedFilterType.TurnCallCall),
                Create(FilterAdvancedStageType.Turn, "Turn Call Raise", AdvancedFilterType.TurnCallRaise),
                Create(FilterAdvancedStageType.Turn, "Turn Check", AdvancedFilterType.TurnCheck),
                Create(FilterAdvancedStageType.Turn, "Turn Check Fold", AdvancedFilterType.TurnCheckFold),
                Create(FilterAdvancedStageType.Turn, "Turn Check Call", AdvancedFilterType.TurnCheckCall),
                Create(FilterAdvancedStageType.Turn, "Turn Check Raise", AdvancedFilterType.TurnCheckRaise),
                Create(FilterAdvancedStageType.Turn, "Turn Fold", AdvancedFilterType.TurnFold),
                Create(FilterAdvancedStageType.Turn, "Turn Was Check Raised", AdvancedFilterType.TurnWasCheckRaised),
                Create(FilterAdvancedStageType.Turn, "Turn Was Bet Into", AdvancedFilterType.TurnWasBetInto),
                Create(FilterAdvancedStageType.Turn, "Turn Was Raised", AdvancedFilterType.TurnWasRaised),
                Create(FilterAdvancedStageType.Turn, "BBs Bet Turn is Bigger Than...", AdvancedFilterType.BBsBetTurnisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "BBs Bet Turn is Less Than...", AdvancedFilterType.BBsBetTurnisLessThan),
                Create(FilterAdvancedStageType.Turn, "BBs Called Turn is Bigger Than...", AdvancedFilterType.BBsCalledTurnisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "BBs Called Turn is Less Than...", AdvancedFilterType.BBsCalledTurnisLessThan),
                Create(FilterAdvancedStageType.Turn, "BBs Put in Turn is Bigger Than...", AdvancedFilterType.BBsPutinTurnisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "BBs Put in Turn is Less Than...", AdvancedFilterType.BBsPutinTurnisLessThan),
                Create(FilterAdvancedStageType.Turn, "Turn Pot Size in BBs is Bigger Than...", AdvancedFilterType.TurnPotSizeinBBsisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Turn Pot Size in BBs is Less Than...", AdvancedFilterType.TurnPotSizeinBBsisLessThan),
                Create(FilterAdvancedStageType.Turn, "Turn Stack Pot Ratio is Bigger Than...", AdvancedFilterType.TurnStackPotRatioisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Turn Stack Pot Ratio is Less Than...", AdvancedFilterType.TurnStackPotRatioisLessThan),
                Create(FilterAdvancedStageType.Turn, "Turn Bet Size / Pot is Bigger Than...", AdvancedFilterType.TurnBetSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Turn Bet Size / Pot is Less Than...", AdvancedFilterType.TurnBetSizePotisLessThan),
                Create(FilterAdvancedStageType.Turn, "Turn Raise Size / Pot is Bigger Than...", AdvancedFilterType.TurnRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Turn Raise Size / Pot is Less Than...", AdvancedFilterType.TurnRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.Turn, "Turn Facing Bet Size / Pot is Bigger Than...", AdvancedFilterType.TurnFacingBetSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Turn Facing Bet Size / Pot is Less Than...", AdvancedFilterType.TurnFacingBetSizePotisLessThan),
                Create(FilterAdvancedStageType.Turn, "Turn Facing Raise Size / Pot is Bigger Than...", AdvancedFilterType.TurnFacingRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.Turn, "Turn Facing Raise Size / Pot is Less Than...", AdvancedFilterType.TurnFacingRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.Turn, "Allin on Turn", AdvancedFilterType.AllinonTurn),
                Create(FilterAdvancedStageType.Turn, "Allin on Turn (or earlier)", AdvancedFilterType.AllinonTurnOrEarlier),
                // RIVER
                Create(FilterAdvancedStageType.River, "Saw River", AdvancedFilterType.SawRiver),
                Create(FilterAdvancedStageType.River, "Last to Act on River", AdvancedFilterType.LasttoActonRiver),
                Create(FilterAdvancedStageType.River, "Last to Act on River = False", AdvancedFilterType.LasttoActonRiverFalse),
                Create(FilterAdvancedStageType.River, "River Unopened", AdvancedFilterType.RiverUnopened),
                Create(FilterAdvancedStageType.River, "River Unopened = False", AdvancedFilterType.RiverUnopenedFalse),
                Create(FilterAdvancedStageType.River, "Players on River is Bigger Than...", AdvancedFilterType.PlayersonRiverisBiggerThan),
                Create(FilterAdvancedStageType.River, "Players on River is Less Than...", AdvancedFilterType.PlayersonRiverisLessThan),
                Create(FilterAdvancedStageType.River, "Players on River is Equal To...", AdvancedFilterType.PlayersonRiverisEqualTo),
                Create(FilterAdvancedStageType.River, "River Continuation Bet Possible", AdvancedFilterType.RiverContinuationBetPossible),
                Create(FilterAdvancedStageType.River, "River Continuation Bet Made", AdvancedFilterType.RiverContinuationBetMade),
                Create(FilterAdvancedStageType.River, "Facing River Continuation Bet", AdvancedFilterType.FacingRiverContinuationBet),
                Create(FilterAdvancedStageType.River, "Folded to River Continuation Bet", AdvancedFilterType.FoldedtoRiverContinuationBet),
                Create(FilterAdvancedStageType.River, "Called River Continuation Bet", AdvancedFilterType.CalledRiverContinuationBet),
                Create(FilterAdvancedStageType.River, "Raised River Continuation Bet", AdvancedFilterType.RaisedRiverContinuationBet),
                Create(FilterAdvancedStageType.River, "River Bet", AdvancedFilterType.RiverBet),
                Create(FilterAdvancedStageType.River, "River Bet Fold", AdvancedFilterType.RiverBetFold),
                Create(FilterAdvancedStageType.River, "River Bet Call", AdvancedFilterType.RiverBetCall),
                Create(FilterAdvancedStageType.River, "River Bet Raise", AdvancedFilterType.RiverBetRaise),
                Create(FilterAdvancedStageType.River, "River Raise", AdvancedFilterType.RiverRaise),
                Create(FilterAdvancedStageType.River, "River Raise Fold", AdvancedFilterType.RiverRaiseFold),
                Create(FilterAdvancedStageType.River, "River Raise Call", AdvancedFilterType.RiverRaiseCall),
                Create(FilterAdvancedStageType.River, "River Raise Raise", AdvancedFilterType.RiverRaiseRaise),
                Create(FilterAdvancedStageType.River, "River Call", AdvancedFilterType.RiverCall),
                Create(FilterAdvancedStageType.River, "River Call Fold", AdvancedFilterType.RiverCallFold),
                Create(FilterAdvancedStageType.River, "River Call Call", AdvancedFilterType.RiverCallCall),
                Create(FilterAdvancedStageType.River, "River Call Raise", AdvancedFilterType.RiverCallRaise),
                Create(FilterAdvancedStageType.River, "River Check", AdvancedFilterType.RiverCheck),
                Create(FilterAdvancedStageType.River, "River Check Fold", AdvancedFilterType.RiverCheckFold),
                Create(FilterAdvancedStageType.River, "River Check Call", AdvancedFilterType.RiverCheckCall),
                Create(FilterAdvancedStageType.River, "River Check Raise", AdvancedFilterType.RiverCheckRaise),
                Create(FilterAdvancedStageType.River, "River Fold", AdvancedFilterType.RiverFold),
                Create(FilterAdvancedStageType.River, "River Was Check Raised", AdvancedFilterType.RiverWasCheckRaised),
                Create(FilterAdvancedStageType.River, "River Was Bet Into", AdvancedFilterType.RiverWasBetInto),
                Create(FilterAdvancedStageType.River, "River Was Raised", AdvancedFilterType.RiverWasRaised),
                Create(FilterAdvancedStageType.River, "BBs Bet River is Bigger Than...", AdvancedFilterType.BBsBetRiverisBiggerThan),
                Create(FilterAdvancedStageType.River, "BBs Bet River is Less Than...", AdvancedFilterType.BBsBetRiverisLessThan),
                Create(FilterAdvancedStageType.River, "BBs Called River is Bigger Than...", AdvancedFilterType.BBsCalledRiverisBiggerThan),
                Create(FilterAdvancedStageType.River, "BBs Called River is Less Than...", AdvancedFilterType.BBsCalledRiverisLessThan),
                Create(FilterAdvancedStageType.River, "BBs Put in River is Bigger Than...", AdvancedFilterType.BBsPutinRiverisBiggerThan),
                Create(FilterAdvancedStageType.River, "BBs Put in River is Less Than...", AdvancedFilterType.BBsPutinRiverisLessThan),
                Create(FilterAdvancedStageType.River, "River Pot Size in BBs is Bigger Than...", AdvancedFilterType.RiverPotSizeinBBsisBiggerThan),
                Create(FilterAdvancedStageType.River, "River Pot Size in BBs is Less Than...", AdvancedFilterType.RiverPotSizeinBBsisLessThan),
                Create(FilterAdvancedStageType.River, "River Stack Pot Ratio is Bigger Than...", AdvancedFilterType.RiverStackPotRatioisBiggerThan),
                Create(FilterAdvancedStageType.River, "River Stack Pot Ratio is Less Than...", AdvancedFilterType.RiverStackPotRatioisLessThan),
                Create(FilterAdvancedStageType.River, "River Bet Size / Pot is Bigger Than...", AdvancedFilterType.RiverBetSizePotisBiggerThan),
                Create(FilterAdvancedStageType.River, "River Bet Size / Pot is Less Than...", AdvancedFilterType.RiverBetSizePotisLessThan),
                Create(FilterAdvancedStageType.River, "River Raise Size / Pot is Bigger Than...", AdvancedFilterType.RiverRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.River, "River Raise Size / Pot is Less Than...", AdvancedFilterType.RiverRaiseSizePotisLessThan),
                Create(FilterAdvancedStageType.River, "River Facing Bet Size / Pot is Bigger Than...", AdvancedFilterType.RiverFacingBetSizePotisBiggerThan),
                Create(FilterAdvancedStageType.River, "River Facing Bet Size / Pot is Less Than...", AdvancedFilterType.RiverFacingBetSizePotisLessThan),
                Create(FilterAdvancedStageType.River, "River Facing Raise Size / Pot is Bigger Than...", AdvancedFilterType.RiverFacingRaiseSizePotisBiggerThan),
                Create(FilterAdvancedStageType.River, "River Facing Raise Size / Pot is Less Than...", AdvancedFilterType.RiverFacingRaiseSizePotisLessThan),
                // OTHERS
                Create(FilterAdvancedStageType.Other, "Saw Showdown", AdvancedFilterType.SawShowdown),
                Create(FilterAdvancedStageType.Other, "Won Hand", AdvancedFilterType.WonHand),
                Create(FilterAdvancedStageType.Other, "Final Pot Size in BBs is Bigger Than...", AdvancedFilterType.FinalPotSizeinBBsisBiggerThan),
                Create(FilterAdvancedStageType.Other, "Final Pot Size in BBs is Less Than...", AdvancedFilterType.FinalPotSizeinBBsisLessThan),
                Create(FilterAdvancedStageType.Other, "Player Won BBs is Bigger Than...", AdvancedFilterType.PlayerWonBBsIsBiggerThan),
                Create(FilterAdvancedStageType.Other, "Player Won BBs is Less Than...", AdvancedFilterType.PlayerWonBBsIsLessThan),
                Create(FilterAdvancedStageType.Other, "Player Lost BBs is Bigger Than...", AdvancedFilterType.PlayerLostBBsIsBiggerThan),
                Create(FilterAdvancedStageType.Other, "Player Lost BBs is Less Than...", AdvancedFilterType.PlayerLostBBsIsLessThan),
                Create(FilterAdvancedStageType.Other, "Player Won or Lost BBs is Bigger Than...", AdvancedFilterType.PlayerWonOrLostBBsIsBiggerThan),
                Create(FilterAdvancedStageType.Other, "Player Won or Lost BBs is Less Than...", AdvancedFilterType.PlayerWonOrLostBBsIsLessThan),
                Create(FilterAdvancedStageType.Other, "Players Saw Showdown is Bigger Than...", AdvancedFilterType.PlayersSawShowdownIsBiggerThan),
                Create(FilterAdvancedStageType.Other, "Players Saw Showdown is Less Than...", AdvancedFilterType.PlayersSawShowdownIsLessThan),
                Create(FilterAdvancedStageType.Other, "Players Saw Showdown is Equal To...", AdvancedFilterType.PlayersSawShowdownIsEqualTo),
                Create(FilterAdvancedStageType.Other, "Allin Win% is Bigger Than...", AdvancedFilterType.AllinWinIsBiggerThan),
                Create(FilterAdvancedStageType.Other, "Allin Win% is Less Than...", AdvancedFilterType.AllinWinIsLessThan)
            };
        }

        private static FilterAdvancedItem Create(FilterAdvancedStageType stage, string name, AdvancedFilterType filterType)
        {
            return new FilterAdvancedItem(filterType)
            {
                Name = name,
                Stage = stage
            };
        }

        #endregion
    }
}