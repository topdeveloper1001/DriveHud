//-----------------------------------------------------------------------
// <copyright file="SingletonStorageModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Model
{
    public sealed class SingletonStorageModel : INotifyPropertyChanged
    {
        #region fields

        #endregion

        #region Constructor

        public SingletonStorageModel()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Select player with name specified if exists
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="loadHeroIfMissing">True if need to select HERO in case when player with specified name does not exist</param>
        public void TryLoadActivePlayer(IPlayer player, bool loadHeroIfMissing)
        {
            var playerSelectedItem = PlayerCollection.FirstOrDefault(x => x.PlayerId == player.PlayerId && x.GetType() == player.GetType());

            if (playerSelectedItem != null)
            {
                PlayerSelectedItem = playerSelectedItem;
                return;
            }

            if (loadHeroIfMissing)
            {
                TryLoadHeroPlayer();
            }
        }

        public void TryLoadHeroPlayer()
        {
            var heroName = CommonResourceManager.Instance.GetResourceString(ResourceStrings.HeroName);

            var playerCollectionItems = PlayerCollection.OfType<PlayerCollectionItem>().ToArray();

            if (playerCollectionItems.Any(x => x.Name == heroName))
            {
                PlayerSelectedItem = playerCollectionItems
                    .Where(x => x.Name == heroName)
                    .OrderBy(x => x.PokerSite).FirstOrDefault();
            }
        }

        private void StatisticCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (FilteredCashPlayerStatistic == null)
            {
                FilteredCashPlayerStatistic = new List<Playerstatistic>();
            }

            if (FilteredTournamentPlayerStatistic == null)
            {
                FilteredTournamentPlayerStatistic = new List<Playerstatistic>();
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    var newItems = e.NewItems?.OfType<Playerstatistic>().ToList();
                    var oldItems = e.OldItems?.OfType<Playerstatistic>().ToList();

                    var newCashItems = newItems?.Where(x => !x.IsTourney).AsQueryable().Where(CashFilterPredicate).ToList();
                    var newTournamentItems = newItems?.Where(x => x.IsTourney).AsQueryable().Where(TournamentFilterPredicate).ToList();

                    if (newCashItems != null && newCashItems.Any())
                    {
                        FilteredCashPlayerStatistic.AddRange(newCashItems);
                    }

                    if (newTournamentItems != null && newTournamentItems.Any())
                    {
                        FilteredTournamentPlayerStatistic.AddRange(newTournamentItems);
                    }

                    if (oldItems != null)
                    {
                        var oldCashItems = oldItems.Where(x => !x.IsTourney).AsQueryable().Where(CashFilterPredicate).ToList();
                        var oldTournamentItems = oldItems.Where(x => x.IsTourney).AsQueryable().Where(TournamentFilterPredicate).ToList();

                        FilteredCashPlayerStatistic.RemoveRange(oldCashItems);
                        FilteredTournamentPlayerStatistic.RemoveRange(oldTournamentItems);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                default:
                    UpdateFilteredStatistics();
                    break;
            }
        }

        private void UpdateFilteredStatistics()
        {
            var statistic = StatisticCollection?.ToList();
            FilteredCashPlayerStatistic = statistic?.Where(x => !x.IsTourney).AsQueryable().Where(CashFilterPredicate).ToList();
            FilteredTournamentPlayerStatistic = statistic?.Where(x => x.IsTourney).AsQueryable().Where(TournamentFilterPredicate).ToList();
        }

        #endregion

        #region Properties

        private static readonly Lazy<SingletonStorageModel> lazy = new Lazy<SingletonStorageModel>(() => new SingletonStorageModel());
        public static SingletonStorageModel Instance { get { return lazy.Value; } }

        private RangeObservableCollection<Playerstatistic> _statisticCollection;
        private ObservableCollection<IPlayer> _playerCollection;
        private IPlayer _playerSelectedItem;

        public RangeObservableCollection<Playerstatistic> StatisticCollection
        {
            get
            {
                return _statisticCollection;
            }
            set
            {
                if (StatisticCollection != null)
                {
                    StatisticCollection.CollectionChanged -= StatisticCollection_CollectionChanged;
                }

                _statisticCollection = value;

                if (StatisticCollection != null)
                {
                    StatisticCollection.CollectionChanged += StatisticCollection_CollectionChanged;
                }

                OnPropertyChanged();
            }
        }

        public ObservableCollection<IPlayer> PlayerCollection
        {
            get { return _playerCollection; }
            set
            {
                _playerCollection = value;
                OnPropertyChanged();
            }
        }

        public IPlayer PlayerSelectedItem
        {
            get { return _playerSelectedItem; }
            set
            {
                if (_playerSelectedItem == value) return;

                _playerSelectedItem = value;

                OnPropertyChanged();
                OnPropertyChanged("Indicators");
            }
        }

        private Expression<Func<Playerstatistic, bool>> cashFilterPredicate = PredicateBuilder.True<Playerstatistic>();

        public Expression<Func<Playerstatistic, bool>> CashFilterPredicate
        {
            get
            {
                return cashFilterPredicate;
            }
            set
            {
                cashFilterPredicate = value;

                FilteredCashPlayerStatistic = StatisticCollection?
                    .ToList()
                    .Where(x => !x.IsTourney)
                    .AsQueryable()
                    .Where(CashFilterPredicate)
                    .ToList();

                OnPropertyChanged();
            }
        }

        private Expression<Func<Playerstatistic, bool>> tournamentFilterPredicate = PredicateBuilder.True<Playerstatistic>();

        public Expression<Func<Playerstatistic, bool>> TournamentFilterPredicate
        {
            get
            {
                return tournamentFilterPredicate;
            }
            set
            {
                tournamentFilterPredicate = value;

                FilteredTournamentPlayerStatistic = StatisticCollection?
                    .ToList()
                    .Where(x => x.IsTourney)
                    .AsQueryable()
                    .Where(TournamentFilterPredicate)
                    .ToList();

                OnPropertyChanged();
            }
        }

        private List<Playerstatistic> filteredCashPlayerStatistic;

        public List<Playerstatistic> FilteredCashPlayerStatistic
        {
            get
            {
                return filteredCashPlayerStatistic;
            }
            set
            {
                if (ReferenceEquals(filteredCashPlayerStatistic, value))
                {
                    return;
                }

                filteredCashPlayerStatistic = value;
                OnPropertyChanged();
            }
        }

        private List<Playerstatistic> filteredTournamentPlayerStatistic;

        public List<Playerstatistic> FilteredTournamentPlayerStatistic
        {
            get
            {
                return filteredTournamentPlayerStatistic;
            }
            set
            {
                if (ReferenceEquals(filteredTournamentPlayerStatistic, value))
                {
                    return;
                }

                filteredTournamentPlayerStatistic = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}