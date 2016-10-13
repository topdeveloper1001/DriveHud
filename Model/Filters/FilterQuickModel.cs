using DriveHUD.Common.Reflection;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Cards;
using HoldemHand;
using Model.Enums;
using Model.Extensions;
using Model.HandAnalyzers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Model.Filters
{
    [Serializable]
    public class FilterQuickModel : FilterBaseEntity, IFilterModel
    {
        public FilterQuickModel()
        {
            this.Name = "Quick Filters";
            this.Type = EnumFilterModelType.FilterQuickModel;
        }

        public void Initialize()
        {
            FilterSectionQuickFilterInitialize();
        }

        #region Methods
        private void FilterSectionQuickFilterInitialize()
        {
            QuickFilterCollection = new ObservableCollection<QuickFilterItem>()
            {
                new QuickFilterItem() { Name = "3-Bet non premium hand", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), QuickFilterHandType = QuickFilterHandTypeEnum.NonPremiumHand },
                new QuickFilterItem() { Name = "3-Bet premium hand", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), QuickFilterHandType = QuickFilterHandTypeEnum.PremiumHand },
                new QuickFilterItem() { Name = "Call 3-Bet w/ non premium hand", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Calledthreebetpreflop), QuickFilterHandType = QuickFilterHandTypeEnum.NonPremiumHand },
                new QuickFilterItem() { Name = "Call 3-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Calledthreebetpreflop) },
                new QuickFilterItem() { Name = "3-Bet bluff from Blinds", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidthreebetBluffInBlinds) },
                new QuickFilterItem() { Name = "3-Bet bluff from SB", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidthreebetBluffInSb) },
                new QuickFilterItem() { Name = "3-Bet bluff from BB", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidthreebetBluffInBb) },
                new QuickFilterItem() { Name = "Squeezed Pre-flop", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didsqueeze) },
                new QuickFilterItem() { Name = "Squeezed w/ bluff range", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didsqueeze), QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "4-Bet Bluff", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidfourbetBluff) },
                new QuickFilterItem() { Name = "4-Bet Bluff from BTN", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidFourBetBluffInBtn) },
                new QuickFilterItem() { Name = "4-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didfourbet) },
                new QuickFilterItem() { Name = "Saw Flop HU’s", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.FacedHandsUpOnFlop) },
                new QuickFilterItem() { Name = "Saw Flop MW", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.FacedMultiWayOnFlop) },
                new QuickFilterItem() { Name = "Float Flop", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.PlayedFloatFlop) },
                new QuickFilterItem() { Name = "Delayed C-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidDelayedTurnCBet) },
                new QuickFilterItem() { Name = "Check Flop w/ TP or Better", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidCheckFlop), QuickFilterHandType = QuickFilterHandTypeEnum.FlopTPOrBetter },
                new QuickFilterItem() { Name = "Cold Call IP", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidColdCallIp) },
                new QuickFilterItem() { Name = "Cold Call OOP", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidColdCallOop) },
                new QuickFilterItem() { Name = "Saw Turn", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawTurn) },
                new QuickFilterItem() { Name = "Double Barrel w/ Air", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.TurnContinuationBetWithAirMade) },
                new QuickFilterItem() { Name = "Saw River", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawRiver) },
                new QuickFilterItem() { Name = "Bluffed River", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidBluffedRiver) },
                new QuickFilterItem() { Name = "Triple Barrel w/ less than mid pair", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Rivercontinuationbetmade), QuickFilterHandType = QuickFilterHandTypeEnum.LessThanMidPairOnRiver },
                new QuickFilterItem() { Name = "Flush Draw on Flop", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawflop), QuickFilterHandType = QuickFilterHandTypeEnum.FlushDrawOnFlop },
                new QuickFilterItem() { Name = "Straight Draw on Flop", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawflop), QuickFilterHandType = QuickFilterHandTypeEnum.StraightDrawOnFlop },
                new QuickFilterItem() { Name = "Defend Blinds w/ marginal hand", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Bigblindstealdefended), QuickFilterHandType = QuickFilterHandTypeEnum.MarginalHand },
                new QuickFilterItem() { Name = "Defend BB", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Bigblindstealdefended) }
            };
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            return GetQuickFilterPredicate();
        }

        public void ResetFilter()
        {
            ResetFastFilterCollection();
        }

        public override object Clone()
        {
            FilterQuickModel model = this.DeepCloneJson();

            return model;
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterQuickModel)
            {
                var filterToLoad = filter as FilterQuickModel;

                ResetQuickFilterCollectionTo(filterToLoad.QuickFilterCollection);
            }
        }
        #endregion

        #region ResetFilters

        public void ResetFastFilterCollection()
        {
            QuickFilterCollection.Where(x => x.TriStateSelectedItem.TriState != EnumTriState.Any).ToList().ForEach(x => x.TriStateSet(EnumTriState.Any));
        }

        #endregion

        #region Restore Defaults

        private void ResetQuickFilterCollectionTo(IEnumerable<QuickFilterItem> quickFilterList)
        {
            foreach (var filter in quickFilterList)
            {
                var cur = QuickFilterCollection.FirstOrDefault(x => x.Name == filter.Name);
                if (cur != null && cur.TriStateSelectedItem.TriState != filter.TriStateSelectedItem.TriState)
                {
                    cur.TriStateSet(filter.TriStateSelectedItem.TriState);
                }
            }
        }

        #endregion

        #region Predicates

        private Expression<Func<Playerstatistic, bool>> GetQuickFilterPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();
            foreach (var stat in QuickFilterCollection.Where(x => x.TriStateSelectedItem.TriState != EnumTriState.Any))
            {
                if (!string.IsNullOrWhiteSpace(stat.PropertyName))
                {
                    switch (stat.TriStateSelectedItem.TriState)
                    {
                        case EnumTriState.On:
                            predicate = predicate.And(x => Convert.ToBoolean(ReflectionHelper.GetMemberValue(x, stat.PropertyName)));
                            break;
                        case EnumTriState.Off:
                            predicate = predicate.And(x => !Convert.ToBoolean(ReflectionHelper.GetMemberValue(x, stat.PropertyName)));
                            break;
                        case EnumTriState.Any:
                            break;
                    }
                }

                if (stat.QuickFilterHandType != QuickFilterHandTypeEnum.None)
                {
                    predicate = predicate.And(GetHandPredicate(stat.QuickFilterHandType));
                }
            }
            return predicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetHandPredicate(QuickFilterHandTypeEnum handType)
        {
            switch (handType)
            {
                case QuickFilterHandTypeEnum.None:
                    break;
                case QuickFilterHandTypeEnum.StraightDrawOnFlop:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, Street.Flop)
                                            && Hand.IsStraightDraw(x.Cards, BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop).ToString(), string.Empty));
                case QuickFilterHandTypeEnum.FlushDrawOnFlop:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, Street.Flop)
                                            && Hand.IsFlushDraw(x.Cards, BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop).ToString(), string.Empty));
                case QuickFilterHandTypeEnum.MarginalHand:
                    return PredicateBuilder.Create<Playerstatistic>(x => HandAnalyzerHelpers.IsMarginalHand(x.Cards));
                case QuickFilterHandTypeEnum.LessThanMidPairOnRiver:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, Street.River)
                                            && new HandAnalyzer(new IAnalyzer[] { new LessThanMidPairAnalyzer() })
                                                .Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.River))
                                                    .GetRank() == ShowdownHands.LessThanMidPair);
                case QuickFilterHandTypeEnum.FlopTPOrBetter:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, Street.Flop)
                                            && new HandAnalyzer(new IAnalyzer[] { new LessThanMidPairAnalyzer() })
                                                .Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop))
                                                    .GetRank() == ShowdownHands.LessThanMidPair);
                case QuickFilterHandTypeEnum.PremiumHand:
                    return PredicateBuilder.Create<Playerstatistic>(x => HandAnalyzerHelpers.IsPremiumHand(x.Cards));
                case QuickFilterHandTypeEnum.NonPremiumHand:
                    return PredicateBuilder.Create<Playerstatistic>(x => !HandAnalyzerHelpers.IsPremiumHand(x.Cards));
                case QuickFilterHandTypeEnum.BluffRange:
                    return PredicateBuilder.Create<Playerstatistic>(x => HandAnalyzerHelpers.IsBluffRange(x.Cards));
                default:
                    break;
            }

            return PredicateBuilder.True<Playerstatistic>();
        }

        #endregion

        #region Properties

        private EnumFilterModelType _type;
        private ObservableCollection<QuickFilterItem> _quickFilterCollection;

        public ObservableCollection<QuickFilterItem> QuickFilterCollection
        {
            get { return _quickFilterCollection; }
            set
            {
                if (value == _quickFilterCollection) return;
                _quickFilterCollection = value;
                OnPropertyChanged(nameof(QuickFilterCollection));
            }
        }

        public EnumFilterModelType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }

    [Serializable]
    public class QuickFilterItem : FilterTriStateBase
    {
        public static Action OnTriState;

        public QuickFilterItem() : this(EnumTriState.Any) { }

        public QuickFilterItem(EnumTriState param = EnumTriState.Any) : base(param)
        {
            QuickFilterHandType = QuickFilterHandTypeEnum.None;
        }

        private string _propertyName;
        private TriStateItem _triStateSelectedItem;
        private QuickFilterHandTypeEnum _quickFilterHandType;

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        public override TriStateItem TriStateSelectedItem
        {
            get { return _triStateSelectedItem; }
            set
            {
                if (value == _triStateSelectedItem) return;
                _triStateSelectedItem = value;
                OnPropertyChanged();

                if (OnTriState != null) OnTriState.Invoke();
            }
        }

        public QuickFilterHandTypeEnum QuickFilterHandType
        {
            get { return _quickFilterHandType; }

            set
            {
                if (value == _quickFilterHandType) return;
                _quickFilterHandType = value;

                OnPropertyChanged(nameof(QuickFilterHandType));
            }
        }
    }
}
