//-----------------------------------------------------------------------
// <copyright file="SingletonStorageModel.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Model
{
    public sealed class SingletonStorageModel : BindableBase
    {
        private static readonly ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();

        public SingletonStorageModel()
        {
            statisticCollection = new RangeObservableCollection<Playerstatistic>();
            statisticCollection.CollectionChanged += OnStatisticCollectionCollectionChanged;
        }

        #region Properties

        private static readonly Lazy<SingletonStorageModel> lazyInstance = new Lazy<SingletonStorageModel>(() => new SingletonStorageModel());

        public static SingletonStorageModel Instance => lazyInstance.Value;

        private readonly RangeObservableCollection<Playerstatistic> statisticCollection;
        private List<Playerstatistic> filteredCashPlayerStatistic;
        private List<Playerstatistic> filteredTournamentPlayerStatistic;

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

                using (syncLock.Write())
                {
                    filteredCashPlayerStatistic = statisticCollection
                        .Where(x => !x.IsTourney)
                        .AsQueryable()
                        .Where(CashFilterPredicate)
                        .ToList();
                }

                RaisePropertyChanged();
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

                using (syncLock.Write())
                {
                    filteredTournamentPlayerStatistic = statisticCollection
                        .Where(x => x.IsTourney)
                        .AsQueryable()
                        .Where(TournamentFilterPredicate)
                        .ToList();
                }

                RaisePropertyChanged();
            }
        }

        private ObservableCollection<IPlayer> playerCollection;

        public ObservableCollection<IPlayer> PlayerCollection
        {
            get
            {
                return playerCollection;
            }
            set
            {
                SetProperty(ref playerCollection, value);
            }
        }

        private IPlayer playerSelectedItem;

        public IPlayer PlayerSelectedItem
        {
            get
            {
                return playerSelectedItem;
            }
            set
            {
                SetProperty(ref playerSelectedItem, value);
                RaisePropertyChanged("Indicators");
            }
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

        public void AddStatistic(IEnumerable<Playerstatistic> playerstatistic)
        {
            try
            {
                syncLock.EnterWriteLock();
                statisticCollection.AddRange(playerstatistic);
            }
            finally
            {
                syncLock.ExitWriteLock();
            }
        }

        public List<Playerstatistic> GetStatisticCollection()
        {
            try
            {
                syncLock.EnterReadLock();
                return statisticCollection.ToList();
            }
            finally
            {
                syncLock.ExitReadLock();
            }
        }

        public Playerstatistic FindStatistic(Func<Playerstatistic, bool> predicate)
        {
            try
            {
                syncLock.EnterReadLock();
                return statisticCollection.FirstOrDefault(predicate);
            }
            finally
            {
                syncLock.ExitReadLock();
            }
        }

        public int RemoveStatistic(Func<Playerstatistic, bool> condition)
        {
            try
            {
                syncLock.EnterWriteLock();
                return statisticCollection.RemoveByCondition(condition);
            }
            finally
            {
                syncLock.EnterWriteLock();
            }
        }

        public List<Playerstatistic> GetFilteredCashPlayerStatistic()
        {
            try
            {
                syncLock.EnterReadLock();
                return filteredCashPlayerStatistic?.ToList();
            }
            finally
            {
                syncLock.ExitReadLock();
            }
        }

        public List<Playerstatistic> GetFilteredTournamentPlayerStatistic()
        {
            try
            {
                syncLock.EnterReadLock();
                return filteredTournamentPlayerStatistic?.ToList();
            }
            finally
            {
                syncLock.ExitReadLock();
            }
        }

        public void SetStatisticCollection(IEnumerable<Playerstatistic> statistic)
        {
            try
            {
                syncLock.EnterWriteLock();
                statisticCollection.Reset(statistic);
                UpdateFilteredStatistics();
            }
            finally
            {
                syncLock.ExitWriteLock();
            }
        }

        public void ResetStatisticCollection()
        {
            try
            {
                syncLock.EnterWriteLock();
                statisticCollection.Clear();
                UpdateFilteredStatistics();
            }
            finally
            {
                syncLock.ExitWriteLock();
            }
        }

        private void OnStatisticCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (filteredCashPlayerStatistic == null)
            {
                filteredCashPlayerStatistic = new List<Playerstatistic>();
            }

            if (filteredTournamentPlayerStatistic == null)
            {
                filteredTournamentPlayerStatistic = new List<Playerstatistic>();
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
                        filteredCashPlayerStatistic.AddRange(newCashItems);
                    }

                    if (newTournamentItems != null && newTournamentItems.Any())
                    {
                        filteredTournamentPlayerStatistic.AddRange(newTournamentItems);
                    }

                    if (oldItems != null)
                    {
                        var oldCashItems = oldItems.Where(x => !x.IsTourney).AsQueryable().Where(CashFilterPredicate).ToList();
                        var oldTournamentItems = oldItems.Where(x => x.IsTourney).AsQueryable().Where(TournamentFilterPredicate).ToList();

                        filteredCashPlayerStatistic.RemoveRange(oldCashItems);
                        filteredTournamentPlayerStatistic.RemoveRange(oldTournamentItems);
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
            filteredCashPlayerStatistic = statisticCollection.Where(x => !x.IsTourney).AsQueryable().Where(CashFilterPredicate).ToList();
            filteredTournamentPlayerStatistic = statisticCollection.Where(x => x.IsTourney).AsQueryable().Where(TournamentFilterPredicate).ToList();
        }

        #endregion      
    }
}