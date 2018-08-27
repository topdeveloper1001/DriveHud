﻿//-----------------------------------------------------------------------
// <copyright file="FilterStandardModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Model.Filters
{
    [Serializable]
    public class FilterStandardModel : FilterBaseEntity, IFilterModel
    {
        #region Constructor

        public FilterStandardModel(int playerCountMinAvailable = 2, int playerCountMinSelectedItem = 2, int playerCountMaxAvailable = 10, int playerCountMaxSelectedItem = 10)
        {
            Name = "Standard Filters";

            Type = EnumFilterModelType.FilterStandardModel;

            PlayerCountMinAvailable = playerCountMinAvailable;
            PlayerCountMaxAvailable = playerCountMaxAvailable;

            FilterSectionPlayersBetweenCollectionInitialize();
        }

        public void Initialize()
        {
            FilterSectionStatCollectionInitialize();
            FilterSectionStakeLevelCollectionInitialize();
            FilterSectionBuyinCollectionInitialize();
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
            StatCollection = new ObservableCollection<StatItem>
            {
                new StatItem { Name = "VPIP", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Vpiphands ) },
                new StatItem { Name = "PFR", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Pfrhands ) },
                new StatItem { Name = "3-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didthreebet) },
                new StatItem { Name = "Called 3-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Calledthreebetpreflop) },
                new StatItem { Name = "4-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didfourbet) },
                new StatItem { Name = "C-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Flopcontinuationbetmade) },
                new StatItem { Name = "Fold to C-Bet", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Foldedtoflopcontinuationbet) },
                new StatItem { Name = "Check-Raise", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidCheckRaise) },
                new StatItem { Name = "Squeezed", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Didsqueeze) },
                new StatItem { Name = "Relative Position", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.IsRelativePosition) },
                new StatItem { Name = "Saw Flop", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawflop) },
                new StatItem { Name = "Saw Turn", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawTurn) },
                new StatItem { Name = "Saw River", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.SawRiver) },
                new StatItem { Name = "Saw Showdown", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.Sawshowdown) },
                new StatItem { Name = "Open Raised", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.DidOpenRaise) },
                new StatItem { Name = "Relative 3-Bet POS", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.IsRelative3BetPosition) },
                new StatItem { Name = "Steal", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.StealMade) },
                new StatItem { Name = "Raised Limpers", PropertyName = ReflectionHelper.GetPath<Playerstatistic>(o => o.IsRaisedLimpers) }
            };
        }

        public void FilterSectionStakeLevelCollectionInitialize()
        {
            StakeLevelCollection = new ObservableCollection<StakeLevelItem>();
        }

        public void FilterSectionBuyinCollectionInitialize()
        {
            BuyinCollection = new ObservableCollection<BuyinItem>();
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
                        new CurrencyItem() { Name = "EUR", Value = Currency.EURO, IsChecked = true }
                    }
                );
        }

        public void PlayerCountListSet()
        {
            if (PlayerCountMinAvailable == 0 || PlayerCountMaxAvailable == 0)
            {
                return;
            }

            var minCount = (PlayerCountMaxSelectedItem ?? PlayerCountMaxAvailable) - 2;

            PlayerCountMinList = (from v in Enumerable.Range(PlayerCountMinAvailable, minCount) select v).ToList();
            PlayerCountMinSelectedItem = _playerCountMinList.Contains(PlayerCountMinSelectedItem ?? -1) ? PlayerCountMinSelectedItem : PlayerCountMinList.ElementAt(0);

            var maxCount = this.PlayerCountMaxAvailable - (PlayerCountMinSelectedItem ?? PlayerCountMinAvailable);

            PlayerCountMaxList = (from v in Enumerable.Range((PlayerCountMinSelectedItem ?? PlayerCountMinAvailable) + 1, maxCount) select v).ToList();
            PlayerCountMaxSelectedItem = _playerCountMaxList.Contains(PlayerCountMaxSelectedItem ?? -1) ? PlayerCountMaxSelectedItem : PlayerCountMaxList.ElementAt(0);

            OnPlayersBetweenChanged?.Invoke();
        }

        public void FilterSectionTableRingCollectionInitialize()
        {
            Table6MaxCollection = new ObservableCollection<TableRingItem>()
            {
                new TableRingItem() { IsChecked = true, Seat = 5, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.BTN, Name = "BTN" },
                new TableRingItem() { IsChecked = true, Seat = 6, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.SB, Name = "SB" },
                new TableRingItem() { IsChecked = true, Seat = 1, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.BB, Name = "BB" },
                new TableRingItem() { IsChecked = true, Seat = 2, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.UTG, Name = "UTG" },
                new TableRingItem() { IsChecked = true, Seat = 3, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.MP1, Name = "MP1" },
                new TableRingItem() { IsChecked = true, Seat = 4, TableType = EnumTableType.Six, PlayerPosition = EnumPosition.CO, Name = "CO" },
            };

            TableFullRingCollection = new ObservableCollection<TableRingItem>()
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
                var limit = Limit.FromSmallBlindBigBlind(gameType.Smallblindincents / 100m,
                    gameType.Bigblindincents / 100m,
                    (Currency)gameType.CurrencytypeId);

                var isFastFold = ((TableTypeDescription)gameType.TableType & TableTypeDescription.FastFold) == TableTypeDescription.FastFold;

                var gtString = String.Format("{0}{1}{2}{3}",
                    limit.GetCurrencySymbol(),
                    Math.Abs(limit.BigBlind),
                    GameTypeUtils.GetShortName((GameType)gameType.PokergametypeId),
                    isFastFold ? " Fast" : string.Empty);

                var stakeLevelItem = new StakeLevelItem
                {
                    Name = gtString,
                    StakeLimit = limit,
                    IsChecked = true,
                    PokergametypeId = gameType.PokergametypeId,
                    TableType = gameType.TableType
                };

                gameTypesList.Add(stakeLevelItem);

                if (!StakeLevelCollection.Any(x => x.Name == gtString))
                {
                    StakeLevelCollection.Add(stakeLevelItem);

                    if (!isModified)
                    {
                        isModified = true;
                    }
                }
            }

            var removeList = StakeLevelCollection.Where(x => !gameTypesList.Any(g => x.Name == g.Name)).ToList();

            foreach (var limit in removeList)
            {
                StakeLevelCollection.Remove(limit);

                if (!isModified)
                {
                    isModified = true;
                }
            }

            if (isModified)
            {
                StakeLevelCollection = new ObservableCollection<StakeLevelItem>(StakeLevelCollection.OrderBy(x => x.StakeLimit.BigBlind));
            }
        }

        public void UpdateFilterSectionBuyinCollection(IList<Tournaments> tournaments)
        {
            BuyinCollection.Clear();

            var tournamentsGroupedByBuyin = tournaments
                .GroupBy(x => x.Buyinincents)
                .Select(x => new { Buyin = x.Key, Tournaments = x.Select(t => t.BuildKey()).Distinct().ToArray() })
                .ToArray();

            var buyinItems = new List<BuyinItem>();

            foreach (var tournamentGroupedByBuyin in tournamentsGroupedByBuyin)
            {
                var buyin = tournamentGroupedByBuyin.Buyin / 100m;

                var buyinItem = new BuyinItem
                {
                    Name = $"${buyin}",
                    Buyin = buyin,
                    TournamentKeys = new HashSet<TournamentKey>(tournamentGroupedByBuyin.Tournaments),
                    IsChecked = true
                };

                buyinItems.Add(buyinItem);

                if (!BuyinCollection.Any(x => x.Buyin == buyinItem.Buyin))
                {
                    BuyinCollection.Add(buyinItem);
                }
            }

            var buyinItemsToRemove = (from buyin in BuyinCollection
                                      join buyinItem in buyinItems on buyin.Buyin equals buyinItem.Buyin into gj
                                      from buyinItem in gj.DefaultIfEmpty()
                                      where buyinItem == null
                                      select buyin).ToArray();

            buyinItemsToRemove.ForEach(x => BuyinCollection.Remove(x));
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
                .And(GetBuyinPredicate())
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
            ResetBuyinCollection();
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
                ResetFilterBuyinTo(filterToLoad.BuyinCollection);
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
                    Table6MaxCollection.ToList().ForEach(x => x.IsChecked = true);
                    break;

                case EnumTableType.Nine:
                    TableFullRingCollection.ToList().ForEach(x => x.IsChecked = true);
                    break;
            }
        }

        public void ResetStakeLevelCollection()
        {
            StakeLevelCollection.ToList().ForEach(x => x.IsChecked = true);
        }

        public void ResetBuyinCollection()
        {
            BuyinCollection.ToList().ForEach(x => x.IsChecked = true);
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
            StatCollection.ToList().ForEach(x => x.CurrentTriState = EnumTriState.Any);
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
                    cur.CurrentTriState = stat.CurrentTriState;
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

        private void ResetFilterBuyinTo(IEnumerable<BuyinItem> buyinItems)
        {
            if (buyinItems == null)
            {
                return;
            }

            foreach (var buyinItem in buyinItems)
            {
                var cur = BuyinCollection.FirstOrDefault(x => x.Buyin == buyinItem.Buyin);

                if (cur != null)
                {
                    cur.IsChecked = buyinItem.IsChecked;
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
                    curCollection = Table6MaxCollection;
                    break;

                case EnumTableType.Nine:
                default:
                    curCollection = TableFullRingCollection;
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
            PlayerCountMinSelectedItem = minSelected;
            PlayerCountMaxSelectedItem = maxSelected;
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
                                            && (x.TableTypeDescription & state.TableType) == x.TableTypeDescription
                                            && x.PokergametypeId == state.PokergametypeId));
            }

            return predicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetBuyinPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();

            var uncheckedBuyins = BuyinCollection.Where(x => !x.IsChecked).ToArray();

            foreach (var buyin in uncheckedBuyins)
            {
                predicate = predicate.And(x => !x.IsTourney || !buyin.TournamentKeys.Contains(new TournamentKey(x.PokersiteId, x.TournamentId)));
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

            curPredicate = curPredicate.Or(x => x.CurrencyId == (short)Currency.All);

            return curPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetPreflopActionPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();
            var uncheckedActions = PreFlopActionCollection.Where(x => !x.IsChecked);

            if (uncheckedActions.Any())
            {
                predicate = predicate.And(x => x.FacingPreflop != EnumFacingPreflop.None);
            }

            foreach (var action in uncheckedActions)
            {
                predicate = predicate.And(x => x.FacingPreflop != action.FacingPreflop);
            }

            return predicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetStatsPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();

            foreach (var stat in StatCollection.Where(x => x.CurrentTriState != EnumTriState.Any))
            {
                var empty = new Playerstatistic();
                var propValue = ReflectionHelper.GetMemberValue(empty, stat.PropertyName);

                switch (stat.CurrentTriState)
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
        private ObservableCollection<BuyinItem> _buyinCollection;
        private ObservableCollection<CurrencyItem> _currencyCollection;
        private ObservableCollection<StatItem> _statCollection;
        private ObservableCollection<TableRingItem> _table6maxCollection;
        private ObservableCollection<TableRingItem> _tableFullRingCollection;

        public EnumFilterModelType Type
        {
            get { return _type; }
            set
            {
                SetProperty(ref _type, value);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
            }
        }

        public int PlayerCountMinAvailable
        {
            get { return _playerCountMinAvailable; }
            set
            {
                SetProperty(ref _playerCountMinAvailable, value);
            }
        }

        public List<int> PlayerCountMinList
        {
            get { return _playerCountMinList; }
            set
            {
                if (value != null && _playerCountMinList != null && value.SequenceEqual(_playerCountMinList))
                {
                    return;
                }

                SetProperty(ref _playerCountMinList, value);

                PlayerCountListSet();
            }
        }

        public int? PlayerCountMinSelectedItem
        {
            get { return _playerCountMinSelectedItem; }
            set
            {
                if (_playerCountMinSelectedItem == value)
                {
                    return;
                }

                SetProperty(ref _playerCountMinSelectedItem, value);
                PlayerCountListSet();
            }
        }

        public int PlayerCountMaxAvailable
        {
            get { return _playerCountMaxAvailable; }
            set
            {
                SetProperty(ref _playerCountMaxAvailable, value);
            }
        }

        public List<int> PlayerCountMaxList
        {
            get { return _playerCountMaxList; }
            set
            {
                if (value != null && _playerCountMaxList != null && value.SequenceEqual(_playerCountMaxList))
                {
                    return;
                }

                SetProperty(ref _playerCountMaxList, value);
                PlayerCountListSet();
            }
        }

        public int? PlayerCountMaxSelectedItem
        {
            get { return _playerCountMaxSelectedItem; }
            set
            {
                if (_playerCountMaxSelectedItem == value)
                {
                    return;
                }

                SetProperty(ref _playerCountMaxSelectedItem, value);
                PlayerCountListSet();
            }
        }

        public ObservableCollection<PreFlopActionItem> PreFlopActionCollection
        {
            get { return _preFlopActionCollection; }
            set
            {
                SetProperty(ref _preFlopActionCollection, value);
            }
        }

        public ObservableCollection<StakeLevelItem> StakeLevelCollection
        {
            get { return _stakeLevelCollection; }
            set
            {
                SetProperty(ref _stakeLevelCollection, value);
            }
        }

        public ObservableCollection<BuyinItem> BuyinCollection
        {
            get
            {
                return _buyinCollection;
            }
            set
            {
                SetProperty(ref _buyinCollection, value);
            }
        }

        public ObservableCollection<CurrencyItem> CurrencyCollection
        {
            get { return _currencyCollection; }
            set
            {
                SetProperty(ref _currencyCollection, value);
            }
        }
        public ObservableCollection<StatItem> StatCollection
        {
            get { return _statCollection; }
            set
            {
                SetProperty(ref _statCollection, value);
            }
        }

        public ObservableCollection<TableRingItem> Table6MaxCollection
        {
            get { return _table6maxCollection; }
            set
            {
                SetProperty(ref _table6maxCollection, value);
            }
        }

        public ObservableCollection<TableRingItem> TableFullRingCollection
        {
            get { return _tableFullRingCollection; }
            set
            {
                SetProperty(ref _tableFullRingCollection, value);
            }
        }

        #endregion
    }

    [Serializable]
    public class FilterSectionItem : FilterBaseEntity
    {
        private EnumFilterSectionItemType _itemType = EnumFilterSectionItemType.FilterSectionNone;
        private string _value = string.Empty;

        public EnumFilterSectionItemType ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                SetProperty(ref _itemType, value);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetProperty(ref _value, value);
            }
        }
    }

    [Serializable]
    public class StakeLevelItem : FilterBaseEntity
    {
        public static Action OnIsChecked;
        private bool isChecked;
        private Limit stakeLimit;
        private short pokergametypeId;

        public Limit StakeLimit
        {
            get
            {
                return stakeLimit;
            }
            set
            {
                SetProperty(ref stakeLimit, value);
            }
        }

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                SetProperty(ref isChecked, value);
                OnIsChecked?.Invoke();
            }
        }

        public short PokergametypeId
        {
            get
            {
                return pokergametypeId;
            }

            set
            {
                SetProperty(ref pokergametypeId, value);
            }
        }

        private uint tableType;

        public uint TableType
        {
            get
            {
                return tableType;
            }
            set
            {
                SetProperty(ref tableType, value);
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
                RaisePropertyChanged();

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
                RaisePropertyChanged();

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
                RaisePropertyChanged();
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                RaisePropertyChanged();

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

        public string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
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

                RaisePropertyChanged();

                if (OnTriState != null)
                {
                    OnTriState.Invoke();
                }
            }
        }
    }

    [Serializable]
    public class BuyinItem : FilterBaseEntity
    {
        public static Action OnIsChecked;

        private bool isChecked;

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                SetProperty(ref isChecked, value);
                OnIsChecked?.Invoke();
            }
        }

        private decimal buyin;

        public decimal Buyin
        {
            get
            {
                return buyin;
            }
            set
            {
                SetProperty(ref buyin, value);
            }
        }

        private HashSet<TournamentKey> tournamentKeys;

        public HashSet<TournamentKey> TournamentKeys
        {
            get
            {
                return tournamentKeys;
            }
            set
            {
                SetProperty(ref tournamentKeys, value);
            }
        }
    }
}