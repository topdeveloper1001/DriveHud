using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using System.ComponentModel;
using DriveHUD.Common.Annotations;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using DriveHUD.Common.Linq;

namespace Model.Filters
{
    public class MainFilterModelManagerService : IFilterModelManagerService, INotifyPropertyChanged
    {
        public MainFilterModelManagerService()
        {
            Initialize();
        }

        private void Initialize()
        {
            _enumFilterType = EnumFilterType.Cash;

            this.FilterTupleCollection = new ObservableCollection<FilterTuple>
            (
                new List<FilterTuple>()
                {
                    new FilterTuple() { Name = "Standard Filters", ModelType = EnumFilterModelType.FilterStandardModel, ViewModelType = EnumViewModelType.FilterStandardViewModel },
                    new FilterTuple() { Name = "Hole Cards", ModelType = EnumFilterModelType.FilterHandGridModel, ViewModelType = EnumViewModelType.FilterHandGridViewModel, },
                    new FilterTuple() { Name = "Hand Value", ModelType = EnumFilterModelType.FilterHandValueModel, ViewModelType = EnumViewModelType.FilterHandValueViewModel, },
                    new FilterTuple() { Name = "Board Texture", ModelType = EnumFilterModelType.FilterBoardTextureModel, ViewModelType = EnumViewModelType.FilterBoardTextureViewModel },
                    new FilterTuple() { Name = "Hand Action", ModelType = EnumFilterModelType.FilterHandActionModel, ViewModelType = EnumViewModelType.FilterHandActionViewModel, },
                    new FilterTuple() { Name = "Quick Filters", ModelType = EnumFilterModelType.FilterQuickModel, ViewModelType = EnumViewModelType.FilterQuickViewModel, },
                }
            );

            this._filterModelCollections = new Dictionary<EnumFilterType, ObservableCollection<IFilterModel>>()
            {
                { EnumFilterType.Cash, GetFilterModelsList() },
                { EnumFilterType.Tournament, GetFilterModelsList() }
            };
        }

        public ObservableCollection<IFilterModel> GetFilterModelsList()
        {
            var list = new ObservableCollection<IFilterModel>
            {
                    new FilterStandardModel() { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Name = "Standard Filters" },
                    new FilterHoleCardsModel() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Hole Cards" },
                    new FilterHandValueModel() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Hand Value" },
                    new FilterBoardTextureModel() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Board Texture" },
                    new FilterHandActionModel() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Hand Action" },
                    new FilterQuickModel() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Quick Filters" },
                    new FilterDateModel() { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Date Filter" },
                    new FilterOmahaHandGridModel() { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Omaha Hand Grid" },
                    new FilterHandGridModel() { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Hand Grid" },
            };

            list.ForEach(x => x.Initialize());

            return list;
        }

        public void SetFilterType(EnumFilterType filterType)
        {
            switch (filterType)
            {
                case EnumFilterType.Tournament:
                    _enumFilterType = EnumFilterType.Tournament;
                    break;
                case EnumFilterType.Cash:
                default:
                    _enumFilterType = EnumFilterType.Cash;
                    break;
            }
        }

        public void SpreadFilter()
        {
            EnumFilterType fromType = _enumFilterType;

            for (int i = 0; i < _filterModelCollections.Count; i++)
            {
                var key = _filterModelCollections.ElementAt(i).Key;
                if (key == fromType)
                {
                    continue;
                }

                var filterCollection = _filterModelCollections[key];
                var fromCollection = _filterModelCollections[fromType];

                for (int j = 0; j < filterCollection.Count; j++)
                {
                    var currentFilter = filterCollection[j];
                    var fromFilter = fromCollection.FirstOrDefault(x => x.Type == currentFilter.Type);
                    if (fromFilter != null)
                    {
                        currentFilter.LoadFilter(fromFilter);
                    }
                }
            }
        }

        public Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> GetFilterModelDictionary()
        {
            return _filterModelCollections;
        }

        #region Properties
        private EnumFilterType _enumFilterType;

        public Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> _filterModelCollections;

        private ObservableCollection<FilterTuple> _filterTupleCollection;

        public ObservableCollection<FilterTuple> FilterTupleCollection
        {
            get { return _filterTupleCollection; }
            set
            {
                _filterTupleCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IFilterModel> FilterModelCollection
        {
            get { return _filterModelCollections[_enumFilterType]; }
            set
            {
                if (!_filterModelCollections.ContainsKey(_enumFilterType))
                {
                    return;
                }
                _filterModelCollections[_enumFilterType] = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
