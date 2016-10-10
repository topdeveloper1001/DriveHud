using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Model.Enums;
using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Utils;
using System.Linq.Expressions;

namespace Model.Filters
{
    [Serializable]
    public class FilterStandardModel : FilterBaseEntity, IFilterModel
    {
        #region Constructor

        public FilterStandardModel(int playerCountMinAvailable = 2, int playerCountMinSelectedItem = 2, int playerCountMaxAvailable = 10, int playerCountMaxSelectedItem = 10)
        {
            this.Name = "Standard Filters";
            this.Type = EnumFilterModelType.FilterStandardModel;

            this.PlayerCountMinAvailable = playerCountMinAvailable;
            this.PlayerCountMaxAvailable = playerCountMaxAvailable;

            FilterSectionPlayersBetweenCollectionInitialize();
        }

        public void Initialize()
        {
            FilterSectionStatCollectionInitialize();
            FilterSectionStakeLevelCollectionInitialize();
            FilterSectionPreFlopActionCollectionInitialize();
            FilterSectionCurrencyCollectionInitialize();
            FilterSectionTableRingCollectionInitialize();
        }

        #endregion

        #region Actions
        public static Action OnPlayersBetweenChanged;
        #endregion

        #region Methods

        public void FilterSectionStatCollectionInitialize()
        {
            ////////////////////////////////
            // Initialize StatItemCollection
            this.StatCollection = new ObservableCollection<StatItem>
                (
                    new List<StatItem>()
                    {
                        new StatItem() { Name = "VPIP", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Vpiphands ) },
                        new StatItem() { Name = "PFR", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Pfrhands ) },
                        new StatItem() { Name = "3-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet) },
                        new StatItem() { Name = "Called 3-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Calledthreebetpreflop) },
                        new StatItem() { Name = "4-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didfourbet) },
                        new StatItem() { Name = "C-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Flopcontinuationbetmade) },
                        new StatItem() { Name = "Fold to C-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Foldedtoflopcontinuationbet) },
                        new StatItem() { Name = "Check-Raise", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidCheckRaise) },
                        new StatItem() { Name = "Squeezed", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didsqueeze) },
                        new StatItem() { Name = "Relative Position", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.IsRelativePosition) },
                        new StatItem() { Name = "Saw Flop", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawflop) },
                        new StatItem() { Name = "Saw Turn", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawTurn) },
                        new StatItem() { Name = "Saw River", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawRiver) },
                        new StatItem() { Name = "Saw Showdown", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawshowdown) },
                        new StatItem() { Name = "Open Raised", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidOpenRaise) },
                        new StatItem() { Name = "Relative 3-Bet POS", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.IsRelative3BetPosition) },
                        new StatItem() { Name = "Steal", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.StealMade) },
                        new StatItem() { Name = "Raised Limpers", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.IsRaisedLimpers) },
                    }
                );
        }

        public void FilterSectionStakeLevelCollectionInitialize()
        {
            //////////////////////////////////
            // Initialize StakeLevelCollection
            this.StakeLevelCollection = new ObservableCollection<StakeLevelItem>();
        }

        public void FilterSectionPreFlopActionCollectionInitialize()
        {
            /////////////////////////////////////
            // Initialize PreFlopActionCollection
            this.PreFlopActionCollection = new ObservableCollection<PreFlopActionItem>
                (
                    new List<PreFlopActionItem>()
                    {
                        new PreFlopActionItem() { Name = CommonResourceManager.Instance.GetResourceString("Common_Unopened"), FacingPreflop = EnumFacingPreflop.Unopened, IsChecked = true },
                        new PreFlopActionItem() { Name = CommonResourceManager.Instance.GetResourceString("Common_Limper"),  FacingPreflop = EnumFacingPreflop.Limper, IsChecked = true },
                        new PreFlopActionItem() { Name = String.Format("2+ {0}", CommonResourceManager.Instance.GetResourceString("Common_Limpers")), FacingPreflop = EnumFacingPreflop.MultipleLimpers, IsChecked = true },
                        new PreFlopActionItem() { Name = CommonResourceManager.Instance.GetResourceString("Common_Raiser"), FacingPreflop = EnumFacingPreflop.Raiser,  IsChecked = true },
                        new PreFlopActionItem() { Name = String.Format("{0} + {1}", CommonResourceManager.Instance.GetResourceString("Common_Raiser"), CommonResourceManager.Instance.GetResourceString("Common_Caller")), FacingPreflop = EnumFacingPreflop.RaiserAndCaller, IsChecked = true },
                        new PreFlopActionItem() { Name = String.Format("2+ {0}", CommonResourceManager.Instance.GetResourceString("Common_Callers")), FacingPreflop = EnumFacingPreflop.MultipleCallers, IsChecked = true },
                        new PreFlopActionItem() { Name = CommonResourceManager.Instance.GetResourceString("Common_3Bet"), FacingPreflop = EnumFacingPreflop.ThreeBet, IsChecked = true},
                        new PreFlopActionItem() { Name = CommonResourceManager.Instance.GetResourceString("Common_4Bet"), FacingPreflop = EnumFacingPreflop.FourBet, IsChecked = true },
                        new PreFlopActionItem() { Name = CommonResourceManager.Instance.GetResourceString("Common_5Bet"), FacingPreflop = EnumFacingPreflop.FiveBet, IsChecked = true }
                    }
                );
        }

        public void FilterSectionCurrencyCollectionInitialize()
        {
            ////////////////////////////////
            // Initialize CurrencyCollection
            this.CurrencyCollection = new ObservableCollection<CurrencyItem>
                (
                    new List<CurrencyItem>()
                    {
                        new CurrencyItem() { Name = "USD", Value = Currency.USD, IsChecked = true },
                        new CurrencyItem() { Name = "CAD", Value = Currency.CAD, IsChecked = true },
                        new CurrencyItem() { Name = "YUAN", Value = Currency.YUAN, IsChecked = true },
                        new CurrencyItem() { Name = "EUR", Value = Currency.EURO, IsChecked = true },
                    }
                );
        }

        public void PlayerCountListSet()
        {
            if (this.PlayerCountMinAvailable == 0 || this.PlayerCountMaxAvailable == 0) return;

            int minCount = (this.PlayerCountMaxSelectedItem ?? this.PlayerCountMaxAvailable) - 2;
            this.PlayerCountMinList = (from v in Enumerable.Range(this.PlayerCountMinAvailable, minCount) select v).ToList();
            this.PlayerCountMinSelectedItem = this._playerCountMinList.Contains(this.PlayerCountMinSelectedItem ?? -1) ? this.PlayerCountMinSelectedItem : this.PlayerCountMinList.ElementAt(0);

            int maxCount = this.PlayerCountMaxAvailable - (this.PlayerCountMinSelectedItem ?? this.PlayerCountMinAvailable);
            this.PlayerCountMaxList = (from v in Enumerable.Range((this.PlayerCountMinSelectedItem ?? this.PlayerCountMinAvailable) + 1, maxCount) select v).ToList();
            this.PlayerCountMaxSelectedItem = this._playerCountMaxList.Contains(this.PlayerCountMaxSelectedItem ?? -1) ? this.PlayerCountMaxSelectedItem : this.PlayerCountMaxList.ElementAt(0);


            if (OnPlayersBetweenChanged != null) OnPlayersBetweenChanged.Invoke();
        }

        public void FilterSectionTableRingCollectionInitialize()
        {
            ////////////////////////////////
            // Initialize TableRingCollections
            this.Table6MaxCollection = new ObservableCollection<TableRingItem>()
            {
                new TableRingItem() { IsChecked = true, Seat = 5, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.BTN, Name = "BTN" },
                new TableRingItem() { IsChecked = true, Seat = 6, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.SB, Name = "SB" },
                new TableRingItem() { IsChecked = true, Seat = 1, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.BB, Name = "BB" },
                new TableRingItem() { IsChecked = true, Seat = 2, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.UTG, Name = "UTG" },
                new TableRingItem() { IsChecked = true, Seat = 3, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.MP1, Name = "MP1" },
                new TableRingItem() { IsChecked = true, Seat = 4, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.CO, Name = "CO" },
            };

            this.TableFullRingCollection = new ObservableCollection<TableRingItem>()
            {
                new TableRingItem() { IsChecked = true, Seat = 6, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.BTN, Name = "BTN" },
                new TableRingItem() { IsChecked = true, Seat = 7, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.SB, Name = "SB" },
                new TableRingItem() { IsChecked = true, Seat = 8, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.BB, Name = "BB" },
                new TableRingItem() { IsChecked = true, Seat = 9, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.UTG, Name = "UTG" },
                new TableRingItem() { IsChecked = true, Seat = 1, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.UTG_1, Name = "UTG+1" },
                new TableRingItem() { IsChecked = true, Seat = 2, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.UTG_2, Name = "UTG+2" },
                new TableRingItem() { IsChecked = true, Seat = 3, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.MP1, Name = "MP1" },
                new TableRingItem() { IsChecked = true, Seat = 4, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.MP2, Name = "MP2" },
                new TableRingItem() { IsChecked = true, Seat = 5, TableType = EnumTableType.Nine, PlayerPosition = EnumPosition.CO, Name = "CO" },
            };
        }

        public void FilterSectionPlayersBetweenCollectionInitialize()
        {
            _playerCountMinSelectedItem = this.PlayerCountMinAvailable;
            _playerCountMaxSelectedItem = this.PlayerCountMaxAvailable;

            PlayerCountListSet();
        }

        public void UpdateFilterSectionStakeLevelCollection(IList<Gametypes> gameTypes)
        {
            bool isModified = false;
            List<StakeLevelItem> gameTypesList = new List<StakeLevelItem>();
            foreach (var gameType in gameTypes.Where(x => !x.Istourney))
            {
                var limit = Limit.FromSmallBlindBigBlind(gameType.Smallblindincents / 100m, gameType.Bigblindincents / 100m, (Currency)gameType.CurrencytypeId);
                var gtString = String.Format("{0}{1}{2}", limit.GetCurrencySymbol(), Math.Abs(limit.BigBlind), GameTypeUtils.GetShortName((GameType)gameType.PokergametypeId));
                var stakeLevelItem = new StakeLevelItem() { Name = gtString, StakeLimit = limit, IsChecked = true, PokergametypeId = gameType.PokergametypeId };
                gameTypesList.Add(stakeLevelItem);
                if (!StakeLevelCollection.Any(x => x.Name == gtString))
                {
                    StakeLevelCollection.Add(stakeLevelItem);
                    if (!isModified)
                        isModified = true;
                }
            }

            var removeList = StakeLevelCollection.Where(x => !gameTypesList.Any(g => x.Name == g.Name)).ToList();
            foreach (var limit in removeList)
            {
                StakeLevelCollection.Remove(limit);
                if (!isModified)
                    isModified = true;
            }

            if (isModified)
            {
                StakeLevelCollection = new ObservableCollection<StakeLevelItem>(StakeLevelCollection.OrderBy(x => x.StakeLimit.BigBlind));
            }
        }

        public override object Clone()
        {
            FilterStandardModel model = this.DeepCloneJson();
            model.PlayerCountMaxSelectedItem = PlayerCountMaxSelectedItem;
            model.PlayerCountMinSelectedItem = PlayerCountMinSelectedItem;

            return model;
        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            return PredicateBuilder.Create(GetStakeLevelPredicate())
                .And(GetPlayersBetweenPredicate())
                .And(GetTableRingPredicate())
                .And(GetCurrencyPredicate())
                .And(GetPreflopActionPredicate())
                .And(GetStatsPredicate());
        }

        public void ResetFilter()
        {
            ResetTableRingCollection();
            ResetStakeLevelCollection();
            ResetCurrencyCollection();
            ResetPreFlopActionCollection();
            ResetStatCollection();
            FilterSectionPlayersBetweenCollectionInitialize();
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterStandardModel)
            {
                var filterToLoad = filter as FilterStandardModel;

                ResetFilterStatTo(filterToLoad.StatCollection);
                ResetFilterStakeLevelTo(filterToLoad.StakeLevelCollection);
                ResetFilterPreFlopActionTo(filterToLoad.PreFlopActionCollection);
                ResetFilterCurrencyTo(filterToLoad.CurrencyCollection);
                ResetFilterTableRingTo(filterToLoad.Table6MaxCollection, EnumTableType.Six);
                ResetFilterTableRingTo(filterToLoad.TableFullRingCollection, EnumTableType.Nine);
                ResetFilterPlayersBetweenTo(filterToLoad.PlayerCountMinSelectedItem.Value, filterToLoad.PlayerCountMaxSelectedItem.Value);
            }
        }

        #endregion

        #region Reset Filter
        public void ResetTableRingCollection()
        {
            ResetTableRingCollection(EnumTableType.Six);
            ResetTableRingCollection(EnumTableType.Nine);
        }

        public void ResetTableRingCollection(EnumTableType tableType)
        {
            switch (tableType)
            {
                case EnumTableType.Six:
                    this.Table6MaxCollection.ToList().ForEach(x => x.IsChecked = true);
                    break;

                case EnumTableType.Nine:
                    this.TableFullRingCollection.ToList().ForEach(x => x.IsChecked = true);
                    break;
            }
        }

        public void ResetStakeLevelCollection()
        {
            StakeLevelCollection.ToList().ForEach(x => x.IsChecked = true);
        }

        public void ResetCurrencyCollection()
        {
            CurrencyCollection.ToList().ForEach(x => x.IsChecked = true);
        }

        public void ResetPreFlopActionCollection()
        {
            PreFlopActionCollection.ToList().ForEach(x => x.IsChecked = true);
        }

        public void ResetStatCollection()
        {
            StatCollection.ToList().ForEach(x => x.TriStateSet(EnumTriState.Any));
        }
        #endregion

        #region Restore Defaults
        private void ResetFilterStatTo(IEnumerable<StatItem> statList)
        {
            foreach (var stat in statList)
            {
                var cur = StatCollection.FirstOrDefault(x => x.PropertyName == stat.PropertyName);
                if (cur != null)
                {
                    cur.TriStateSet(stat.TriStateSelectedItem.TriState);
                }
            }
        }

        private void ResetFilterStakeLevelTo(IEnumerable<StakeLevelItem> stakeLevelList)
        {
            foreach (var stakeItem in stakeLevelList)
            {
                var cur = StakeLevelCollection.FirstOrDefault(x => x.Name == stakeItem.Name);
                if (cur != null)
                {
                    cur.IsChecked = stakeItem.IsChecked;
                }
            }
        }

        private void ResetFilterPreFlopActionTo(IEnumerable<PreFlopActionItem> preflopActionList)
        {
            foreach (var preflop in preflopActionList)
            {
                var cur = PreFlopActionCollection.FirstOrDefault(x => x.FacingPreflop == preflop.FacingPreflop);
                if (cur != null)
                {
                    cur.IsChecked = preflop.IsChecked;
                }
            }
        }

        private void ResetFilterCurrencyTo(IEnumerable<CurrencyItem> currencyList)
        {
            foreach (var currency in currencyList)
            {
                var cur = CurrencyCollection.FirstOrDefault(x => x.Value == currency.Value);
                if (cur != null)
                {
                    cur.IsChecked = currency.IsChecked;
                }
            }
        }

        private void ResetFilterTableRingTo(IEnumerable<TableRingItem> tableRingList, EnumTableType tableType)
        {
            IList<TableRingItem> curCollection;
            switch (tableType)
            {
                case EnumTableType.Six:
                    curCollection = this.Table6MaxCollection;
                    break;

                case EnumTableType.Nine:
                default:
                    curCollection = this.TableFullRingCollection;
                    break;
            }

            foreach (var item in tableRingList)
            {
                var cur = curCollection.FirstOrDefault(x => x.Seat == item.Seat);
                if (cur != null)
                {
                    cur.IsChecked = item.IsChecked;
                }
            }

        }

        private void ResetFilterPlayersBetweenTo(int minSelected, int maxSelected)
        {
            this.PlayerCountMinSelectedItem = minSelected;
            this.PlayerCountMaxSelectedItem = maxSelected;
        }
        #endregion

        #region Predicates
        private Expression<Func<Playerstatistic, bool>> GetStakeLevelPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();
            var uncheckedStakes = StakeLevelCollection.Where(x => !x.IsChecked);
            foreach (var state in uncheckedStakes)
            {
                predicate = predicate.And(x => !(x.BigBlind == state.StakeLimit.BigBlind
                                            && x.CurrencyId == (short)state.StakeLimit.Currency
                                            && x.PokergametypeId == state.PokergametypeId));
            }

            return predicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetPlayersBetweenPredicate()
        {
            return PredicateBuilder.Create<Playerstatistic>(x => x.Numberofplayers >= PlayerCountMinSelectedItem)
                .And(x => x.Numberofplayers <= PlayerCountMaxSelectedItem);
        }

        private Expression<Func<Playerstatistic, bool>> GetTableRingPredicate()
        {
            var sixRingUnchecked = Table6MaxCollection.Where(x => !x.IsChecked);
            var fullRingUnchecked = TableFullRingCollection.Where(x => !x.IsChecked);

            var sixRingPredicate = PredicateBuilder.True<Playerstatistic>();
            sixRingPredicate = sixRingPredicate.And(x => x.Numberofplayers <= 6);
            foreach (var tableItem in sixRingUnchecked)
            {
                sixRingPredicate = sixRingPredicate.And(x => x.Position != tableItem.PlayerPosition);
            }

            var fullRingPredicate = PredicateBuilder.True<Playerstatistic>();
            fullRingPredicate = fullRingPredicate.And(x => x.Numberofplayers > 6);
            foreach (var tableItem in fullRingUnchecked)
            {
                fullRingPredicate = fullRingPredicate.And(x => x.Position != tableItem.PlayerPosition);
            }

            return sixRingPredicate.Or(fullRingPredicate);
        }

        private Expression<Func<Playerstatistic, bool>> GetCurrencyPredicate()
        {
            var curPredicate = PredicateBuilder.False<Playerstatistic>();
            var checkedCurrency = CurrencyCollection.Where(x => x.IsChecked);
            foreach (var state in checkedCurrency)
            {
                curPredicate = curPredicate.Or(x => x.CurrencyId == (short)state.Value);
            }
            return curPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetPreflopActionPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();
            var uncheckedActions = PreFlopActionCollection.Where(x => !x.IsChecked);
            foreach (var action in uncheckedActions)
            {
                predicate = predicate.And(x => x.FacingPreflop != action.FacingPreflop);
            }

            return predicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetStatsPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();
            foreach (var stat in StatCollection.Where(x => x.TriStateSelectedItem.TriState != EnumTriState.Any))
            {
                Playerstatistic empty = new Playerstatistic();
                var propValue = ReflectionHelper.GetMemberValue(empty, stat.PropertyName);

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
            return predicate;
        }
        #endregion

        #region Properties

        private EnumFilterModelType _type;
        private string _description = "Filter description";

        private int _playerCountMinAvailable;
        private List<int> _playerCountMinList;
        private int? _playerCountMinSelectedItem;

        private int _playerCountMaxAvailable;
        private List<int> _playerCountMaxList;
        private int? _playerCountMaxSelectedItem;

        private ObservableCollection<PreFlopActionItem> _preFlopActionCollection;
        private ObservableCollection<StakeLevelItem> _stakeLevelCollection;
        private ObservableCollection<CurrencyItem> _currencyCollection;
        private ObservableCollection<StatItem> _statCollection;
        private ObservableCollection<TableRingItem> _table6maxCollection;
        private ObservableCollection<TableRingItem> _tableFullRingCollection;

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

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public int PlayerCountMinAvailable
        {
            get { return _playerCountMinAvailable; }
            set
            {
                if (value == _playerCountMinAvailable) return;
                _playerCountMinAvailable = value;
                OnPropertyChanged();
            }
        }

        public List<int> PlayerCountMinList
        {
            get { return _playerCountMinList; }
            set
            {
                if (value != null && _playerCountMinList != null && value.SequenceEqual(_playerCountMinList)) return;
                _playerCountMinList = value;
                OnPropertyChanged();

                PlayerCountListSet();
            }
        }

        public int? PlayerCountMinSelectedItem
        {
            get { return _playerCountMinSelectedItem; }
            set
            {
                if (value == _playerCountMinSelectedItem) return;
                _playerCountMinSelectedItem = value;
                OnPropertyChanged();

                PlayerCountListSet();
            }
        }

        public int PlayerCountMaxAvailable
        {
            get { return _playerCountMaxAvailable; }
            set
            {
                if (value == _playerCountMaxAvailable) return;
                _playerCountMaxAvailable = value;
                OnPropertyChanged();
            }
        }

        public List<int> PlayerCountMaxList
        {
            get { return _playerCountMaxList; }
            set
            {
                if (value != null && _playerCountMaxList != null && value.SequenceEqual(_playerCountMaxList)) return;
                _playerCountMaxList = value;
                OnPropertyChanged();

                PlayerCountListSet();
            }
        }

        public int? PlayerCountMaxSelectedItem
        {
            get { return _playerCountMaxSelectedItem; }
            set
            {
                if (value == _playerCountMaxSelectedItem) return;
                _playerCountMaxSelectedItem = value;
                OnPropertyChanged();

                PlayerCountListSet();
            }
        }

        public ObservableCollection<PreFlopActionItem> PreFlopActionCollection
        {
            get { return _preFlopActionCollection; }
            set
            {
                if (value == _preFlopActionCollection) return;
                _preFlopActionCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StakeLevelItem> StakeLevelCollection
        {
            get { return _stakeLevelCollection; }
            set
            {
                if (value == _stakeLevelCollection) return;
                _stakeLevelCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CurrencyItem> CurrencyCollection
        {
            get { return _currencyCollection; }
            set
            {
                if (value == _currencyCollection) return;
                _currencyCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<StatItem> StatCollection
        {
            get { return _statCollection; }
            set
            {
                if (value == _statCollection) return;
                _statCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TableRingItem> Table6MaxCollection
        {
            get { return _table6maxCollection; }
            set
            {
                if (value == _table6maxCollection) return;
                _table6maxCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TableRingItem> TableFullRingCollection
        {
            get { return _tableFullRingCollection; }
            set
            {
                if (value == _tableFullRingCollection) return;
                _tableFullRingCollection = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }

    [Serializable]
    public class FilterSectionItem : FilterBaseEntity
    {
        private EnumFilterSectionItemType _itemType = EnumFilterSectionItemType.FilterSectionNone;
        private string value = string.Empty;

        public EnumFilterSectionItemType ItemType
        {
            get { return _itemType; }

            set
            {
                if (value == _itemType) return;
                _itemType = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }

    [Serializable]
    public class StakeLevelItem : FilterBaseEntity
    {
        public static Action OnIsChecked;
        private bool _isChecked;
        private Limit _stakeLimit;
        private short _pokergametypeId;

        public Limit StakeLimit
        {
            get { return _stakeLimit; }
            set
            {
                if (value == _stakeLimit) return;
                _stakeLimit = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnIsChecked != null) OnIsChecked.Invoke();
            }
        }

        public short PokergametypeId
        {
            get
            {
                return _pokergametypeId;
            }

            set
            {
                _pokergametypeId = value;
            }
        }
    }

    [Serializable]
    public class PreFlopActionItem : FilterBaseEntity
    {
        public static Action OnIsChecked;
        private bool _isChecked;

        private EnumFacingPreflop _facingPreflop;

        public EnumFacingPreflop FacingPreflop
        {
            get { return _facingPreflop; }
            set { _facingPreflop = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnIsChecked != null) OnIsChecked.Invoke();
            }
        }
    }

    [Serializable]
    public class TableRingItem : FilterBaseEntity
    {
        public static Action OnIsChecked;
        private bool _isChecked;
        private EnumTableType _tableType = EnumTableType.Six;
        private EnumPosition _playerPosition = EnumPosition.Undefined;
        private int _seat = 1;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnIsChecked != null) OnIsChecked.Invoke();
            }
        }

        public EnumTableType TableType
        {
            get
            {
                return _tableType;
            }

            set
            {
                _tableType = value;
            }
        }

        public int Seat
        {
            get
            {
                return _seat;
            }

            set
            {
                _seat = value;
            }
        }

        public EnumPosition PlayerPosition
        {
            get
            {
                return _playerPosition;
            }

            set
            {
                _playerPosition = value;
            }
        }
    }

    [Serializable]
    public class CurrencyItem : FilterBaseEntity
    {
        public static Action OnIsChecked;
        private bool _isChecked;

        private Currency _value;

        public Currency Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnIsChecked != null) OnIsChecked.Invoke();
            }
        }
    }

    [Serializable]
    public class StatItem : FilterTriStateBase
    {
        public static Action OnTriState;

        public StatItem() : this(EnumTriState.Any) { }

        public StatItem(EnumTriState param = EnumTriState.Any) : base(param)
        {
        }

        private string _propertyName;
        private TriStateItem _triStateSelectedItem;

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
    }

    [Serializable]
    public class TriStateItem : FilterBaseEntity
    {
        private EnumTriState _triState;
        private Color _color;

        public EnumTriState TriState
        {
            get { return _triState; }
            set
            {
                if (value == _triState) return;
                _triState = value;
                OnPropertyChanged();
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                if (value == _color) return;
                _color = value;
                OnPropertyChanged();
            }
        }
    }
}

