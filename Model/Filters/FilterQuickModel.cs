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
                new QuickFilterItem() { Name = "Fold to turn C-Bet", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Foldedtoturncontinuationbet), NoPropertyName = nameof(Playerstatistic.Calledturncontinuationbet) },
                new QuickFilterItem() { Name = "Facing turn C-Bet", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Facingturncontinuationbet), NoPropertyName = nameof(Playerstatistic.Calledturncontinuationbet) },
                new QuickFilterItem() { Name = "3-Bet non premium hand", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), NoPropertyName = nameof(Playerstatistic.Couldthreebet), QuickFilterHandType = QuickFilterHandTypeEnum.NonPremiumHand },
                new QuickFilterItem() { Name = "3-Bet premium hand", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), NoPropertyName = nameof(Playerstatistic.Couldthreebet), QuickFilterHandType = QuickFilterHandTypeEnum.PremiumHand },
                new QuickFilterItem() { Name = "Call 3-Bet w/ non premium hand", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Calledthreebetpreflop), NoPropertyName = nameof(Playerstatistic.Facedthreebetpreflop), QuickFilterHandType = QuickFilterHandTypeEnum.NonPremiumHand },
                new QuickFilterItem() { Name = "Call 3-Bet", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Calledthreebetpreflop), NoPropertyName = nameof(Playerstatistic.Facedthreebetpreflop) },
                new QuickFilterItem() { Name = "3-Bet bluff from Blinds", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), NoPropertyName = nameof(Playerstatistic.Couldthreebet), QuickFilterPosition = Enums.QuickFilterPositionEnum.Blinds, QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "3-Bet bluff from SB", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), NoPropertyName = nameof(Playerstatistic.Couldthreebet), QuickFilterPosition = Enums.QuickFilterPositionEnum.Small, QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "3-Bet bluff from BB", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet), NoPropertyName = nameof(Playerstatistic.Couldthreebet), QuickFilterPosition = Enums.QuickFilterPositionEnum.Big, QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "Squeezed Pre-flop", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didsqueeze), NoPropertyName = nameof(Playerstatistic.Couldsqueeze) },
                new QuickFilterItem() { Name = "Squeezed w/ bluff range", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didsqueeze), NoPropertyName = nameof(Playerstatistic.Couldsqueeze) , QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "4-Bet Bluff", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didfourbet), NoPropertyName = nameof(Playerstatistic.Couldfourbet), QuickFilterHandType = QuickFilterHandTypeEnum.NonPremiumHand },
                new QuickFilterItem() { Name = "4-Bet Bluff from BTN", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didfourbet), NoPropertyName = nameof(Playerstatistic.Couldfourbet), QuickFilterPosition = Enums.QuickFilterPositionEnum.BTN, QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange  },
                new QuickFilterItem() { Name = "4-Bet", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didfourbet) , NoPropertyName = nameof(Playerstatistic.Couldfourbet) },
                new QuickFilterItem() { Name = "Saw Flop HU’s", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.FacedHandsUpOnFlop),  NoPropertyName = nameof(Playerstatistic.FacedMultiWayOnFlop) },
                new QuickFilterItem() { Name = "Saw Flop MW", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.FacedMultiWayOnFlop), NoPropertyName = nameof(Playerstatistic.FacedHandsUpOnFlop) },
                new QuickFilterItem() { Name = "Float Flop", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.PlayedFloatFlop), NoPropertyName = nameof(Playerstatistic.Facingflopcontinuationbet), QuickFilterHandType =  QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "Delayed C-Bet", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidDelayedTurnCBet), NoPropertyName = nameof(Playerstatistic.CouldDelayedTurnCBet) },
                new QuickFilterItem() { Name = "Check Flop w/ TP or Better", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidCheckFlop), NoPropertyName = nameof(Playerstatistic.LimpMade) , QuickFilterHandType = QuickFilterHandTypeEnum.FlopTPOrBetter },
                new QuickFilterItem() { Name = "Cold Call IP", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didcoldcall), NoPropertyName = nameof(Playerstatistic.Couldcoldcall),  QuickFilterPosition = Enums.QuickFilterPositionEnum.IP },
                new QuickFilterItem() { Name = "Cold Call OOP", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didcoldcall), NoPropertyName = nameof(Playerstatistic.Couldcoldcall), QuickFilterPosition = Enums.QuickFilterPositionEnum.OOP },
                new QuickFilterItem() { Name = "Saw Turn", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawTurn) },
                new QuickFilterItem() { Name = "Double Barrel w/ Air", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.TurnContinuationBetWithAirMade), NoPropertyName = nameof(Playerstatistic.Turncontinuationbetpossible), QuickFilterHandType = QuickFilterHandTypeEnum.BluffRange },
                new QuickFilterItem() { Name = "Saw River", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawRiver) },
                new QuickFilterItem() { Name = "Bluffed River", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidBluffedRiver), NoPropertyName = nameof(Playerstatistic.SawRiver) },
                new QuickFilterItem() { Name = "Triple Barrel w/ less than mid pair", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Rivercontinuationbetmade), NoPropertyName = nameof(Playerstatistic.Rivercontinuationbetpossible), QuickFilterHandType = QuickFilterHandTypeEnum.LessThanMidPairOnRiver },
                new QuickFilterItem() { Name = "Flush Draw on Flop", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawflop), QuickFilterHandType = QuickFilterHandTypeEnum.FlushDrawOnFlop },
                new QuickFilterItem() { Name = "Straight Draw on Flop", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawflop), QuickFilterHandType = QuickFilterHandTypeEnum.StraightDrawOnFlop },
                new QuickFilterItem() { Name = "Defend Blinds w/ marginal hand", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Bigblindstealdefended), NoPropertyName = nameof(Playerstatistic.Bigblindstealfaced), QuickFilterHandType = QuickFilterHandTypeEnum.MarginalHand },
                new QuickFilterItem() { Name = "Defend BB", YesPropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Bigblindstealdefended), NoPropertyName = nameof(Playerstatistic.Bigblindstealfaced) }
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
            QuickFilterCollection.ToList().ForEach(x => x.CurrentTriState = EnumTriState.Any);
        }

        #endregion

        #region Restore Defaults

        private void ResetQuickFilterCollectionTo(IEnumerable<QuickFilterItem> quickFilterList)
        {
            foreach (var filter in quickFilterList)
            {
                var cur = QuickFilterCollection.FirstOrDefault(x => x.Name == filter.Name);

                if (cur != null && cur.CurrentTriState != filter.CurrentTriState)
                {
                    cur.CurrentTriState = filter.CurrentTriState;
                }
            }
        }

        #endregion

        #region Predicates

        private Expression<Func<Playerstatistic, bool>> GetQuickFilterPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();

            foreach (var stat in QuickFilterCollection.Where(x => x.CurrentTriState != EnumTriState.Any))
            {
                if (stat.QuickFilterHandType == QuickFilterHandTypeEnum.FlushDrawOnFlop || stat.QuickFilterHandType == QuickFilterHandTypeEnum.StraightDrawOnFlop)
                {
                    predicate = predicate.And(x => Convert.ToBoolean(ReflectionHelper.GetMemberValue(x, stat.YesPropertyName)));

                    var drawPredicate = GetHandPredicate(stat.QuickFilterHandType);

                    if (stat.CurrentTriState == EnumTriState.Off)
                    {
                        drawPredicate = PredicateBuilder.Not(drawPredicate);
                    }

                    predicate = predicate.And(drawPredicate);

                    continue;
                }

                if (!string.IsNullOrWhiteSpace(stat.YesPropertyName))
                {
                    switch (stat.CurrentTriState)
                    {
                        case EnumTriState.On:
                            predicate = predicate.And(x => Convert.ToBoolean(ReflectionHelper.GetMemberValue(x, stat.YesPropertyName)));
                            break;
                        case EnumTriState.Off:
                            predicate = predicate.And(x => !Convert.ToBoolean(ReflectionHelper.GetMemberValue(x, stat.YesPropertyName)));
                            if (!string.IsNullOrWhiteSpace(stat.NoPropertyName))
                            {
                                predicate = predicate.And(x => Convert.ToBoolean(ReflectionHelper.GetMemberValue(x, stat.NoPropertyName)));
                            }
                            break;
                        case EnumTriState.Any:
                            break;
                    }
                }

                if (stat.QuickFilterPosition.HasValue)
                {
                    predicate = predicate.And(GetPositionPredicate(stat.QuickFilterPosition.Value));
                }

                if (stat.QuickFilterHandType != QuickFilterHandTypeEnum.None)
                {
                    predicate = predicate.And(GetHandPredicate(stat.QuickFilterHandType));
                }
            }

            return predicate;
        }

        private static Expression<Func<Playerstatistic, bool>> GetPositionPredicate(Enums.QuickFilterPositionEnum value)
        {
            switch (value)
            {
                case Enums.QuickFilterPositionEnum.Blinds:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Position == EnumPosition.BB || x.Position == EnumPosition.SB);
                case Enums.QuickFilterPositionEnum.Small:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Position == EnumPosition.SB);
                case Enums.QuickFilterPositionEnum.Big:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Position == EnumPosition.BB);
                case Enums.QuickFilterPositionEnum.BTN:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.Position == EnumPosition.BTN);
                case Enums.QuickFilterPositionEnum.IP:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.PreflopIP == 1);
                case Enums.QuickFilterPositionEnum.OOP:
                    return PredicateBuilder.Create<Playerstatistic>(x => x.PreflopOOP == 1);
            }

            return PredicateBuilder.True<Playerstatistic>();
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

        private enum QuickFilterPositionEnum
        {
            Blinds, Small, Big
        }
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
        private string _isPossiblePropertyName;
        private QuickFilterHandTypeEnum _quickFilterHandType;
        private QuickFilterPositionEnum? _quickFilterPosition;

        public string YesPropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        public string NoPropertyName
        {
            get { return _isPossiblePropertyName; }
            set { _isPossiblePropertyName = value; }
        }

        public override EnumTriState CurrentTriState
        {
            get
            {
                return base.CurrentTriState;
            }
            set
            {
                if (value == currentTriState)
                {
                    return;
                }

                currentTriState = value;

                OnPropertyChanged();

                if (OnTriState != null)
                {
                    OnTriState.Invoke();
                }
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

        public QuickFilterPositionEnum? QuickFilterPosition
        {
            get { return _quickFilterPosition; }
            set { _quickFilterPosition = value; }
        }
    }
}
