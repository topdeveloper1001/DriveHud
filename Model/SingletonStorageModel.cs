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
                    var newItems = e.NewItems?.Cast<Playerstatistic>().ToList();

                    var cashItems = newItems.Where(x => !x.IsTourney).AsQueryable().Where(FilterPredicate).ToList();
                    var tournamentItems = newItems.Where(x => x.IsTourney).AsQueryable().Where(FilterPredicate).ToList();

                    if (cashItems != null && cashItems.Any())
                    {
                        FilteredCashPlayerStatistic.AddRange(cashItems);
                    }

                    if (tournamentItems != null && tournamentItems.Any())
                    {
                        FilteredTournamentPlayerStatistic.AddRange(tournamentItems);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    UpdateFilteredStatistics();
                    break;
                case NotifyCollectionChangedAction.Remove:
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
            FilteredCashPlayerStatistic = statistic?.Where(x => !x.IsTourney).AsQueryable().Where(FilterPredicate).ToList();
            FilteredTournamentPlayerStatistic = statistic?.Where(x => x.IsTourney).AsQueryable().Where(FilterPredicate).ToList();
        }

        #endregion

        #region Properties

        private static readonly Lazy<SingletonStorageModel> lazy = new Lazy<SingletonStorageModel>(() => new SingletonStorageModel());
        public static SingletonStorageModel Instance { get { return lazy.Value; } }

        private RangeObservableCollection<Playerstatistic> _statisticCollection;
        private ObservableCollection<IPlayer> _playerCollection;
        private IPlayer _playerSelectedItem;

        private Expression<Func<Playerstatistic, bool>> _filterPredicate = PredicateBuilder.True<Playerstatistic>();

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

        public Expression<Func<Playerstatistic, bool>> FilterPredicate
        {
            get { return _filterPredicate; }
            set
            {
                _filterPredicate = value;
                UpdateFilteredStatistics();
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