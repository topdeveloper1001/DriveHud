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

        public void TryLoadHeroPlayer()
        {
            var heroName = CommonResourceManager.Instance.GetResourceString("SystemSettings_HeroName");
            if (PlayerCollection.Contains(heroName))
            {
                PlayerSelectedItem = PlayerCollection.FirstOrDefault(x => x == heroName);
            }
        }

        private void StatisticCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (FilteredPlayerStatistic == null)
            {
                FilteredPlayerStatistic = new List<Playerstatistic>();
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = e.NewItems?.Cast<Playerstatistic>().AsQueryable().Where(FilterPredicate).ToList();
                    if (newItems != null && newItems.Any())
                    {
                        ((List<Playerstatistic>)FilteredPlayerStatistic).AddRange(newItems);
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
            FilteredPlayerStatistic = this.StatisticCollection?.AsQueryable().Where(FilterPredicate).ToList();
        }

        #endregion

        #region Properties

        private static readonly Lazy<SingletonStorageModel> lazy = new Lazy<SingletonStorageModel>(() => new SingletonStorageModel());
        public static SingletonStorageModel Instance { get { return lazy.Value; } }

        private RangeObservableCollection<Playerstatistic> _statisticCollection;
        private ObservableCollection<string> _playerCollection;
        private string _playerSelectedItem;

        private Expression<Func<Playerstatistic, bool>> _filterPredicate = PredicateBuilder.True<Playerstatistic>();

        public RangeObservableCollection<Playerstatistic> StatisticCollection
        {
            get { return _statisticCollection; }
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

        public ObservableCollection<string> PlayerCollection
        {
            get { return _playerCollection; }
            set
            {
                _playerCollection = value;
                OnPropertyChanged();
            }
        }

        public string PlayerSelectedItem
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

        private IList<Playerstatistic> _filteredPlayerStatistic;
        public IList<Playerstatistic> FilteredPlayerStatistic
        {
            set
            {
                _filteredPlayerStatistic = value;
            }
            get
            {
                return _filteredPlayerStatistic;
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
