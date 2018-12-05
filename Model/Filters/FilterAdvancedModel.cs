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

using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Cards;
using Model.Enums;
using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterAdvancedModel : FilterBaseEntity, IFilterModel
    {
        private BuiltFilterModel builtFilterModel;

        public FilterAdvancedModel()
        {
            Name = "Advanced";
            Type = EnumFilterModelType.FilterAdvancedModel;

            SelectedFilters = new ReactiveList<FilterAdvancedItem>();
            SelectedFilters.Changed.Subscribe(x => SelectedFiltersChanged(x));
        }

        #region Properties 

        public EnumFilterModelType Type { get; }

        [XmlIgnore, JsonIgnore]
        public ObservableCollection<FilterAdvancedItem> Filters { get; private set; } = new ObservableCollection<FilterAdvancedItem>();

        [JsonProperty]
        public ReactiveList<FilterAdvancedItem> SelectedFilters { get; private set; }

        #endregion

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();

            foreach (var filter in SelectedFilters)
            {
                if (!FiltersActions.TryGetValue(filter.FilterType, out Func<Playerstatistic, double?, bool> filterAction))
                {
#if DEBUG
                    Console.WriteLine($"Filter {filter.Name} has no action.");
#endif
                    continue;
                }

                predicate = predicate.And(p => filterAction(p, filter.FilterValue));
            }

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
            filterModel.SelectedFilters.Select(x => x.Clone()).ForEach(x => SelectedFilters.Add(x));
        }

        public void ResetFilter()
        {
            SelectedFilters.Clear();
        }

        public void SetBuiltFilterModel(BuiltFilterModel builtFilterModel)
        {
            this.builtFilterModel = builtFilterModel;

            AddFilterItemsToBuiltFilterModel(SelectedFilters);
        }

        private void SelectedFiltersChanged(NotifyCollectionChangedEventArgs args)
        {
            if (builtFilterModel == null)
            {
                return;
            }

            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                var addedItems = args.NewItems?.OfType<FilterAdvancedItem>();

                if (addedItems != null)
                {
                    AddFilterItemsToBuiltFilterModel(addedItems);
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                var removedItems = args.OldItems?.OfType<FilterAdvancedItem>();

                if (removedItems != null)
                {
                    foreach (var removedItem in removedItems)
                    {
                        var itemToRemove = builtFilterModel.FilterSectionCollection
                            .FirstOrDefault(f => f.ItemType == EnumFilterSectionItemType.AdvancedFilterItem &&
                                f.Name == removedItem.ToolTip);

                        if (itemToRemove != null)
                        {
                            builtFilterModel.FilterSectionCollection.Remove(itemToRemove);
                        }
                    }
                }
            }
            else if (args.Action == NotifyCollectionChangedAction.Reset)
            {
                builtFilterModel.FilterSectionCollection.RemoveByCondition(x => x.ItemType == EnumFilterSectionItemType.AdvancedFilterItem);
            }
        }

        private void AddFilterItemsToBuiltFilterModel(IEnumerable<FilterAdvancedItem> addedItems)
        {
            foreach (var addedItem in addedItems)
            {
                if (!builtFilterModel.FilterSectionCollection
                    .Any(f => f.ItemType == EnumFilterSectionItemType.AdvancedFilterItem &&
                        f.Name == addedItem.ToolTip))
                {
                    var filterItem = new FilterSectionItem
                    {
                        ItemType = EnumFilterSectionItemType.AdvancedFilterItem,
                        Name = addedItem.ToolTip,
                        Value = addedItem.ToolTip,
                        IsActive = true
                    };

                    builtFilterModel.FilterSectionCollection.Add(filterItem);
                }
            }
        }

        public override object Clone()
        {
            var clonedFilterAdvancedModel = new FilterAdvancedModel();

            clonedFilterAdvancedModel.Initialize();
            SelectedFilters.ForEach(x => clonedFilterAdvancedModel.SelectedFilters.Add(x.Clone()));

            return clonedFilterAdvancedModel;
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

        private static readonly ReadOnlyDictionary<AdvancedFilterType, Func<Playerstatistic, double?, bool>> FiltersActions =
            new ReadOnlyDictionary<AdvancedFilterType, Func<Playerstatistic, double?, bool>>(new Dictionary<AdvancedFilterType, Func<Playerstatistic, double?, bool>>
            {
                #region Preflop
                { AdvancedFilterType.VPIP , (p, v) => p.Vpiphands > 0 },
                { AdvancedFilterType.PutMoneyinPot, (p, v) => p.Vpiphands > 0 || p.Position.IsBBPosition() || p.Position.IsSBPosition() || p.Position.IsStraddlePosition() },
                { AdvancedFilterType.PFR, (p, v) => p.Pfrhands > 0 },
                { AdvancedFilterType.PFRFalse, (p, v) => p.Pfrhands == 0 },
                { AdvancedFilterType.Did3Bet, (p, v) => p.Didthreebet > 0 },
                { AdvancedFilterType.DidSqueeze, (p, v) => p.Didsqueeze > 0 },
                { AdvancedFilterType.DidColdCall, (p, v) => p.Didcoldcall > 0 },
                { AdvancedFilterType.CouldColdCall, (p, v) => p.Couldcoldcall > 0 },
                { AdvancedFilterType.CouldColdCallFalse, (p, v) => p.Couldcoldcall == 0 },
                { AdvancedFilterType.FacedPreflop3Bet, (p, v) => p.Facedthreebetpreflop > 0 },
                { AdvancedFilterType.FoldedToPreflop3Bet, (p, v) => p.Foldedtothreebetpreflop > 0 },
                { AdvancedFilterType.CalledPreflop3Bet, (p, v) => p.Calledthreebetpreflop > 0 },
                { AdvancedFilterType.RaisedPreflop3Bet, (p, v) => p.Raisedthreebetpreflop > 0 },
                { AdvancedFilterType.FacedPreflop4Bet, (p, v) => p.Facedfourbetpreflop > 0 },
                { AdvancedFilterType.FoldedToPreflop4Bet, (p, v) => p.Foldedtofourbetpreflop > 0 },
                { AdvancedFilterType.CalledPreflop4Bbet, (p, v) => p.Calledfourbetpreflop > 0 },
                { AdvancedFilterType.RaisedPreflop4Bet, (p, v) => p.Raisedfourbetpreflop > 0 },
                { AdvancedFilterType.InBBandStealAttempted, (p, v) => p.Bigblindstealfaced > 0 },
                { AdvancedFilterType.InBBandStealDefended, (p, v) => p.Bigblindstealdefended > 0 },
                { AdvancedFilterType.InBBandStealReraised, (p, v) => p.Bigblindstealreraised > 0 },
                { AdvancedFilterType.InSBandStealAttempted, (p, v) => p.Smallblindstealattempted > 0 },
                { AdvancedFilterType.InSBandStealDefended, (p, v) => p.Smallblindstealdefended > 0 },
                { AdvancedFilterType.InSBandStealReraised, (p, v) => p.Smallblindstealreraised > 0 },
                { AdvancedFilterType.LimpReraised, (p, v) => p.LimpReraised > 0 },
                { AdvancedFilterType.BBsBetPreflopisBiggerThan, (p, v) => p.BetAmountPreflopInBB > v },
                { AdvancedFilterType.BBsBetPreflopisLessThan, (p, v) => p.BetAmountPreflopInBB < v },
                { AdvancedFilterType.BBsCalledPreflopisBiggerThan, (p, v) => p.CallAmountPreflopInBB > v },
                { AdvancedFilterType.BBsCalledPreflopisLessThan, (p, v) => p.CallAmountPreflopInBB < v },
                { AdvancedFilterType.BBsPutInPreflopisBiggerThan, (p, v) => (p.BetAmountPreflopInBB + p.CallAmountPreflopInBB + p.PostAmountPreflopInBB) > v },
                { AdvancedFilterType.BBsPutInPreflopisLessThan, (p, v) => (p.BetAmountPreflopInBB + p.CallAmountPreflopInBB + p.PostAmountPreflopInBB) < v },
                { AdvancedFilterType.PreflopRaiseSizePotisBiggerThan, (p, v) => p.RaiseSizeToPotPreflop > v },
                { AdvancedFilterType.PreflopRaiseSizePotisLessThan, (p, v) => p.RaiseSizeToPotPreflop < v },
                { AdvancedFilterType.PreflopFacingRaiseSizePotisBiggerThan, (p, v) => p.FacingRaiseSizeToPot > v },
                { AdvancedFilterType.PreflopFacingRaiseSizePotisLessThan, (p, v) => p.FacingRaiseSizeToPot < v },
                { AdvancedFilterType.AllinPreflop, (p, v) => p.Allin.Equals(Street.Preflop.ToString()) },
                #endregion

                #region Flop
                { AdvancedFilterType.SawFlop, (p, v) => p.Sawflop > 0 },
                { AdvancedFilterType.SawFlopFalse, (p, v) => p.Sawflop == 0 },
                { AdvancedFilterType.LasttoActionFlop, (p, v) => p.Sawflop > 0 && p.PreflopIP > 0 && p.FlopActions.Length > 0 },
                { AdvancedFilterType.LasttoActionFlopFalse, (p, v) => p.Sawflop > 0 && p.PreflopIP == 0 && p.FlopActions.Length > 0 },
                { AdvancedFilterType.FlopUnopened, (p, v) => p.FlopActions.Equals("X") },
                { AdvancedFilterType.PlayersonFlopisBiggerThan, (p, v) => p.NumberOfPlayersOnFlop > 0 && p.NumberOfPlayersOnFlop > v },
                { AdvancedFilterType.PlayersonFlopisLessThan, (p, v) => p.NumberOfPlayersOnFlop > 0 && p.NumberOfPlayersOnFlop > v },
                { AdvancedFilterType.PlayersonFlopisEqualTo, (p, v) => p.NumberOfPlayersOnFlop > 0 && p.NumberOfPlayersOnFlop == v },
                { AdvancedFilterType.FlopContinuationBetPossible, (p, v) => p.Flopcontinuationbetpossible > 0 },
                { AdvancedFilterType.FlopContinuationBetMade, (p, v) => p.Flopcontinuationbetmade > 0 },
                { AdvancedFilterType.FacingFlopContinuationBet, (p, v) => p.Facingflopcontinuationbet > 0 },
                { AdvancedFilterType.CalledFlopContinuationBet, (p, v) => p.Calledflopcontinuationbet > 0 },
                { AdvancedFilterType.RaisedFlopContinuationBet, (p, v) => p.Raisedflopcontinuationbet > 0 },
                { AdvancedFilterType.FlopWasCheckRaised, (p, v) => p.DidFlopCheckRaise > 0 || p.FacedFlopCheckRaise > 0 },
                { AdvancedFilterType.FlopWasBetInto, (p, v) => p.TotalbetsFlop >= 1 },
                { AdvancedFilterType.FlopWasRaised, (p, v) => p.TotalbetsFlop > 1 },
                { AdvancedFilterType.BBsBetFlopisBiggerThan, (p, v) => p.BetAmountFlopInBB != 0 && p.BetAmountFlopInBB > v },
                { AdvancedFilterType.BBsBetFlopisLessThan, (p, v) => p.BetAmountFlopInBB != 0 && p.BetAmountFlopInBB < v },
                { AdvancedFilterType.BBsCalledFlopisBiggerThan, (p, v) => p.CallAmountFlopInBB != 0 && p.CallAmountFlopInBB > v },
                { AdvancedFilterType.BBsCalledFlopisLessThan, (p, v) => p.CallAmountFlopInBB != 0 && p.CallAmountFlopInBB < v },
                { AdvancedFilterType.BBsPutinFlopisBiggerThan, (p, v) => (p.BetAmountFlopInBB != 0 || p.CallAmountFlopInBB != 0) && (p.BetAmountFlopInBB + p.CallAmountFlopInBB > v) },
                { AdvancedFilterType.BBsPutinFlopisLessThan, (p, v) => (p.BetAmountFlopInBB != 0 || p.CallAmountFlopInBB != 0) && (p.BetAmountFlopInBB + p.CallAmountFlopInBB < v) },
                { AdvancedFilterType.FlopPotSizeinBBsisBiggerThan, (p, v) => p.FlopPotSizeInBB != 0 && p.FlopPotSizeInBB > v },
                { AdvancedFilterType.FlopPotSizeinBBsisLessThan, (p, v) => p.FlopPotSizeInBB != 0 && p.FlopPotSizeInBB < v },
                { AdvancedFilterType.FlopStackPotRatioisBiggerThan, (p, v) => p.FlopStackPotRatio != 0 && p.FlopStackPotRatio > v },
                { AdvancedFilterType.FlopStackPotRatioisLessThan, (p, v) => p.FlopStackPotRatio != 0 && p.FlopStackPotRatio < v },
                { AdvancedFilterType.FlopBetSizePotisBiggerThan, (p, v) => p.FlopBetToPotRatio != 0 && (double)p.FlopBetToPotRatio > v },
                { AdvancedFilterType.FlopBetSizePotisLessThan, (p, v) => p.FlopBetToPotRatio != 0m && (double)p.FlopBetToPotRatio < v },
                { AdvancedFilterType.FlopRaiseSizePotisBiggerThan, (p, v) => p.FlopRaiseSizeToPot != 0 && p.FlopRaiseSizeToPot > v },
                { AdvancedFilterType.FlopRaiseSizePotisLessThan, (p, v) => p.FlopRaiseSizeToPot != 0 && p.FlopRaiseSizeToPot < v },
                { AdvancedFilterType.FlopFacingBetSizePotisBiggerThan, (p, v) => p.FlopFacingBetSizeToPot != 0 && p.FlopFacingBetSizeToPot > v },
                { AdvancedFilterType.FlopFacingBetSizePotisLessThan, (p, v) => p.FlopFacingBetSizeToPot != 0 && p.FlopFacingBetSizeToPot < v },
                { AdvancedFilterType.FlopFacingRaiseSizePotisBiggerThan, (p, v) => p.FlopFacingRaiseSizeToPot != 0 && p.FlopFacingRaiseSizeToPot > v },
                { AdvancedFilterType.FlopFacingRaiseSizePotisLessThan, (p, v) => p.FlopFacingRaiseSizeToPot != 0 && p.FlopFacingRaiseSizeToPot < v },
                { AdvancedFilterType.AllinOnFlop, (p, v) => p.Allin.Equals(Street.Flop.ToString()) },
                { AdvancedFilterType.AllinOnFlopOrEarlier, (p, v) => p.Allin.Equals(Street.Flop.ToString()) || p.Allin.Equals(Street.Preflop.ToString()) },
                #endregion

                #region Turn
                { AdvancedFilterType.SawTurn, (p, v) => p.SawTurn > 0 },
                { AdvancedFilterType.LasttoActonTurn, (p, v) => p.SawTurn > 0 && p.PreflopIP > 0 && p.TurnActions.Length > 0 },
                { AdvancedFilterType.LasttoActonTurnFalse, (p, v) => p.SawTurn > 0 && p.PreflopIP == 0 && p.TurnActions.Length > 0 },
                { AdvancedFilterType.TurnUnopened, (p, v) => p.TurnActions.Equals("X") },
                { AdvancedFilterType.TurnUnopenedFalse, (p, v) => !p.TurnActions.Equals("X") },
                { AdvancedFilterType.PlayersonTurnisBiggerThan, (p, v) => p.NumberOfPlayersOnTurn > 0 && p.NumberOfPlayersOnTurn > v },
                { AdvancedFilterType.PlayersonTurnisLessThan, (p, v) => p.NumberOfPlayersOnTurn > 0 && p.NumberOfPlayersOnTurn > v },
                { AdvancedFilterType.PlayersonTurnisEqualTo, (p, v) => p.NumberOfPlayersOnTurn > 0 && p.NumberOfPlayersOnTurn == v },
                { AdvancedFilterType.TurnContinuationBetPossible, (p, v) => p.Turncontinuationbetpossible > 0 },
                { AdvancedFilterType.TurnContinuationBetMade, (p, v) => p.Turncontinuationbetmade > 0 },
                { AdvancedFilterType.FacingTurnContinuationBet, (p, v) => p.Facingturncontinuationbet > 0 },
                { AdvancedFilterType.FoldedtoTurnContinuationBet, (p, v) => p.Foldedtoturncontinuationbet > 0 },
                { AdvancedFilterType.CalledTurnContinuationBet, (p, v) => p.Calledturncontinuationbet > 0 },
                { AdvancedFilterType.RaisedTurnContinuationBet, (p, v) => p.Raisedturncontinuationbet > 0 },
                { AdvancedFilterType.TurnWasCheckRaised, (p, v) => p.DidTurnCheckRaise > 0 || p.FacedTurnCheckRaise > 0 },
                { AdvancedFilterType.TurnWasBetInto, (p, v) => p.TotalbetsTurn >= 1 },
                { AdvancedFilterType.TurnWasRaised, (p, v) => p.TotalbetsTurn > 1 },
                { AdvancedFilterType.BBsBetTurnisBiggerThan, (p, v) => p.BetAmountTurnInBB != 0 && p.BetAmountTurnInBB > v },
                { AdvancedFilterType.BBsBetTurnisLessThan, (p, v) => p.BetAmountTurnInBB != 0 &&p.BetAmountTurnInBB < v },
                { AdvancedFilterType.BBsCalledTurnisBiggerThan, (p, v) => p.CallAmountTurnInBB != 0 && p.CallAmountTurnInBB > v },
                { AdvancedFilterType.BBsCalledTurnisLessThan, (p, v) => p.CallAmountTurnInBB != 0 && p.CallAmountTurnInBB < v },
                { AdvancedFilterType.BBsPutinTurnisBiggerThan, (p, v) => (p.BetAmountTurnInBB != 0 || p.CallAmountTurnInBB != 0) && (p.BetAmountTurnInBB + p.CallAmountTurnInBB > v) },
                { AdvancedFilterType.BBsPutinTurnisLessThan, (p, v) => (p.BetAmountTurnInBB != 0 || p.CallAmountTurnInBB != 0) && (p.BetAmountTurnInBB + p.CallAmountTurnInBB) < v },
                { AdvancedFilterType.TurnPotSizeinBBsisBiggerThan, (p, v) => p.TurnPotSizeInBB != 0 && p.TurnPotSizeInBB > v },
                { AdvancedFilterType.TurnPotSizeinBBsisLessThan, (p, v) => p.TurnPotSizeInBB != 0 && p.TurnPotSizeInBB < v },
                { AdvancedFilterType.TurnStackPotRatioisBiggerThan, (p, v) => p.TurnStackPotRatio != 0 && p.TurnStackPotRatio > v },
                { AdvancedFilterType.TurnStackPotRatioisLessThan, (p, v) => p.TurnStackPotRatio != 0 && p.TurnStackPotRatio < v },
                { AdvancedFilterType.TurnBetSizePotisBiggerThan, (p, v) => p.TurnBetToPotRatio != 0m && (double)p.TurnBetToPotRatio > v },
                { AdvancedFilterType.TurnBetSizePotisLessThan, (p, v) => p.TurnBetToPotRatio != 0m && (double)p.TurnBetToPotRatio < v },
                { AdvancedFilterType.TurnRaiseSizePotisBiggerThan, (p, v) => p.TurnRaiseSizeToPot != 0 && p.TurnRaiseSizeToPot > v },
                { AdvancedFilterType.TurnRaiseSizePotisLessThan, (p, v) => p.TurnRaiseSizeToPot != 0 && p.TurnRaiseSizeToPot < v },
                { AdvancedFilterType.TurnFacingBetSizePotisBiggerThan, (p, v) => p.TurnFacingBetSizeToPot != 0 && p.TurnFacingBetSizeToPot > v },
                { AdvancedFilterType.TurnFacingBetSizePotisLessThan, (p, v) => p.TurnFacingBetSizeToPot != 0 && p.TurnFacingBetSizeToPot < v },
                { AdvancedFilterType.TurnFacingRaiseSizePotisBiggerThan, (p, v) => p.TurnFacingRaiseSizeToPot != 0 && p.TurnFacingRaiseSizeToPot > v },
                { AdvancedFilterType.TurnFacingRaiseSizePotisLessThan, (p, v) => p.TurnFacingRaiseSizeToPot != 0 && p.TurnFacingRaiseSizeToPot < v },
                { AdvancedFilterType.AllinonTurn, (p, v) => p.Allin.Equals(Street.Turn.ToString()) },
                { AdvancedFilterType.AllinonTurnOrEarlier, (p, v) => p.Allin.Equals(Street.Turn.ToString()) || p.Allin.Equals(Street.Flop.ToString()) || p.Allin.Equals(Street.Preflop.ToString()) },
                #endregion

                #region River
                { AdvancedFilterType.SawRiver, (p, v) => p.SawRiver > 0 },
                { AdvancedFilterType.LasttoActonRiver, (p, v) => p.SawTurn > 0 && p.PreflopIP > 0 && p.RiverActions.Length > 0 },
                { AdvancedFilterType.LasttoActonRiverFalse, (p, v) => p.SawTurn > 0 && p.PreflopIP == 0 && p.RiverActions.Length > 0 },
                { AdvancedFilterType.RiverUnopened, (p, v) => p.RiverActions.Equals("X") },
                { AdvancedFilterType.RiverUnopenedFalse, (p, v) => !p.RiverActions.Equals("X") },
                { AdvancedFilterType.PlayersonRiverisBiggerThan, (p, v) => p.NumberOfPlayersOnRiver > 0 && p.NumberOfPlayersOnRiver > v },
                { AdvancedFilterType.PlayersonRiverisLessThan, (p, v) => p.NumberOfPlayersOnRiver > 0 && p.NumberOfPlayersOnRiver > v },
                { AdvancedFilterType.PlayersonRiverisEqualTo, (p, v) => p.NumberOfPlayersOnRiver > 0 && p.NumberOfPlayersOnRiver == v },
                { AdvancedFilterType.RiverContinuationBetPossible, (p, v) => p.Rivercontinuationbetpossible > 0 },
                { AdvancedFilterType.RiverContinuationBetMade, (p, v) => p.Rivercontinuationbetmade > 0 },
                { AdvancedFilterType.FacingRiverContinuationBet, (p, v) => p.Facingrivercontinuationbet > 0 },
                { AdvancedFilterType.FoldedtoRiverContinuationBet, (p, v) => p.Foldedtorivercontinuationbet > 0 },
                { AdvancedFilterType.CalledRiverContinuationBet, (p, v) => p.Calledrivercontinuationbet > 0 },
                { AdvancedFilterType.RaisedRiverContinuationBet, (p, v) => p.Raisedrivercontinuationbet > 0 },
                { AdvancedFilterType.RiverWasCheckRaised, (p, v) => p.DidRiverCheckRaise > 0 || p.FacedRiverCheckRaise > 0 },
                { AdvancedFilterType.RiverWasBetInto, (p, v) => p.TotalbetsRiver >= 1 },
                { AdvancedFilterType.RiverWasRaised, (p, v) => p.TotalbetsRiver > 1 },
                { AdvancedFilterType.BBsBetRiverisBiggerThan, (p, v) =>  p.BetAmountRiverInBB != 0 && p.BetAmountRiverInBB > v },
                { AdvancedFilterType.BBsBetRiverisLessThan, (p, v) => p.BetAmountRiverInBB != 0 && p.BetAmountRiverInBB < v },
                { AdvancedFilterType.BBsCalledRiverisBiggerThan, (p, v) => p.CallAmountRiverInBB != 0 && p.CallAmountRiverInBB > v },
                { AdvancedFilterType.BBsCalledRiverisLessThan, (p, v) => p.CallAmountRiverInBB != 0 && p.CallAmountRiverInBB < v },
                { AdvancedFilterType.BBsPutinRiverisBiggerThan, (p, v) => (p.BetAmountRiverInBB != 0 || p.CallAmountRiverInBB != 0) && (p.BetAmountRiverInBB + p.CallAmountRiverInBB > v) },
                { AdvancedFilterType.BBsPutinRiverisLessThan, (p, v) => (p.BetAmountRiverInBB != 0 || p.CallAmountRiverInBB != 0) && (p.BetAmountRiverInBB + p.CallAmountRiverInBB < v) },
                { AdvancedFilterType.RiverPotSizeinBBsisBiggerThan, (p, v) => p.RiverPotSizeInBB != 0 && p.RiverPotSizeInBB > v },
                { AdvancedFilterType.RiverPotSizeinBBsisLessThan, (p, v) => p.RiverPotSizeInBB != 0 && p.RiverPotSizeInBB < v },
                { AdvancedFilterType.RiverStackPotRatioisBiggerThan, (p, v) => p.RiverStackPotRatio != 0 && p.RiverStackPotRatio > v },
                { AdvancedFilterType.RiverStackPotRatioisLessThan, (p, v) => p.RiverStackPotRatio != 0 && p.RiverStackPotRatio < v },
                { AdvancedFilterType.RiverBetSizePotisBiggerThan, (p, v) => p.RiverBetToPotRatio != 0m && (double)p.RiverBetToPotRatio > v },
                { AdvancedFilterType.RiverBetSizePotisLessThan, (p, v) => p.RiverBetToPotRatio != 0m && (double)p.RiverBetToPotRatio < v },
                { AdvancedFilterType.RiverRaiseSizePotisBiggerThan, (p, v) => p.RiverRaiseSizeToPot != 0 && p.RiverRaiseSizeToPot > v },
                { AdvancedFilterType.RiverRaiseSizePotisLessThan, (p, v) => p.RiverRaiseSizeToPot != 0 && p.RiverRaiseSizeToPot < v },
                { AdvancedFilterType.RiverFacingBetSizePotisBiggerThan, (p, v) => p.RiverFacingBetSizeToPot != 0 && p.RiverFacingBetSizeToPot > v },
                { AdvancedFilterType.RiverFacingBetSizePotisLessThan, (p, v) => p.RiverFacingBetSizeToPot != 0 && p.RiverFacingBetSizeToPot < v },
                { AdvancedFilterType.RiverFacingRaiseSizePotisBiggerThan, (p, v) => p.RiverFacingRaiseSizeToPot != 0 && p.RiverFacingRaiseSizeToPot > v },
                { AdvancedFilterType.RiverFacingRaiseSizePotisLessThan, (p, v) => p.RiverFacingRaiseSizeToPot != 0 && p.RiverFacingRaiseSizeToPot < v },
                #endregion

                #region Other
                { AdvancedFilterType.SawShowdown, (p, v) => p.Sawshowdown > 0 },
                { AdvancedFilterType.WonHand, (p, v) => p.Wonhand > 0 },
                { AdvancedFilterType.FinalPotSizeinBBsisBiggerThan, (p, v) => p.PotInBB != 0 && p.PotInBB > v },
                { AdvancedFilterType.FinalPotSizeinBBsisLessThan, (p, v) => p.PotInBB != 0 && p.PotInBB < v },
                { AdvancedFilterType.PlayerWonBBsIsBiggerThan, (p, v) => p.TotalWonInBB != 0 && p.TotalWonInBB > v },
                { AdvancedFilterType.PlayerWonBBsIsLessThan, (p, v) => p.TotalWonInBB != 0 && p.TotalWonInBB < v },
                { AdvancedFilterType.PlayerLostBBsIsBiggerThan, (p, v) => p.TotalWonInBB < 0 && Math.Abs(p.TotalWonInBB) > v },
                { AdvancedFilterType.PlayerLostBBsIsLessThan, (p, v) => p.TotalWonInBB < 0 && Math.Abs(p.TotalWonInBB) < v },
                { AdvancedFilterType.PlayerWonOrLostBBsIsBiggerThan, (p, v) => p.TotalWonInBB != 0 && Math.Abs(p.TotalWonInBB) > v },
                { AdvancedFilterType.PlayerWonOrLostBBsIsLessThan, (p, v) => p.TotalWonInBB != 0 && Math.Abs(p.TotalWonInBB) < v },
                { AdvancedFilterType.PlayersSawShowdownIsBiggerThan, (p, v) => p.NumberOfPlayersSawShowdown != 0 && p.NumberOfPlayersSawShowdown > v },
                { AdvancedFilterType.PlayersSawShowdownIsLessThan, (p, v) => p.NumberOfPlayersSawShowdown != 0 && p.NumberOfPlayersSawShowdown < v },
                { AdvancedFilterType.PlayersSawShowdownIsEqualTo, (p, v) => p.NumberOfPlayersSawShowdown != 0 && p.NumberOfPlayersSawShowdown == v },
                { AdvancedFilterType.AllinWinIsBiggerThan, (p, v) => p.Equity != 0 && (double)p.Equity > v },
                { AdvancedFilterType.AllinWinIsLessThan, (p, v) => p.Equity != 0 && (double)p.Equity < v },
                #endregion
            });

        #endregion
    }
}