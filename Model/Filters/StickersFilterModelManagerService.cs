using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using DriveHUD.Common.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DriveHUD.Common.Linq;

namespace Model.Filters
{
    public class StickersFilterModelManagerService : IFilterModelManagerService
    {
        public StickersFilterModelManagerService()
        {
            this.FilterTupleCollection = new ObservableCollection<FilterTuple>(
                new List<FilterTuple>()
                {
                    new FilterTuple() { Name = "Hole Cards", ModelType = EnumFilterModelType.FilterHandGridModel, ViewModelType = EnumViewModelType.FilterHandGridViewModel, },
                    new FilterTuple() { Name = "Hand Value", ModelType = EnumFilterModelType.FilterHandValueModel, ViewModelType = EnumViewModelType.FilterHandValueViewModel, },
                    new FilterTuple() { Name = "Board Texture", ModelType = EnumFilterModelType.FilterBoardTextureModel, ViewModelType = EnumViewModelType.FilterBoardTextureViewModel },
                    new FilterTuple() { Name = "Hand Action", ModelType = EnumFilterModelType.FilterHandActionModel, ViewModelType = EnumViewModelType.FilterHandActionViewModel, },
                    new FilterTuple() { Name = "Quick Filters", ModelType = EnumFilterModelType.FilterQuickModel, ViewModelType = EnumViewModelType.FilterQuickViewModel, },
                });

            this.FilterModelCollection = GetFilterModelsList();
        }

        public ObservableCollection<IFilterModel> GetFilterModelsList()
        {
            var list = new ObservableCollection<IFilterModel>
            {
                new FilterHoleCardsModel() { Id = Guid.Parse("{064EA6DE-BCD0-40EE-9EB8-31BB1B97873C}"), Name = "Hole Cards" },
                new FilterHandValueModel() { Id = Guid.Parse("{A5665500-35E9-40FF-AEFE-27FDD6826437}"), Name = "Hand Value" },
                new FilterBoardTextureModel() { Id = Guid.Parse("{6138140D-D950-4AB9-B2B1-00380FAFC179}"), Name = "Board Texture" },
                new FilterHandActionModel() { Id = Guid.Parse("{01C28033-1A53-40AD-B10B-B6F85DB4AC92}"), Name = "Hand Action" },
                new FilterQuickModel() { Id = Guid.Parse("{D8AE4CE4-7FFE-4D68-B99E-4DCED4EE8812}"), Name = "Quick Filters" },
                new FilterOmahaHandGridModel() { Id = Guid.Parse("{ABFBCA7F-8DB7-437F-8DEE-A26C5EADF6B3}"), Name = "Omaha Hand Grid" },
                new FilterHandGridModel() { Id = Guid.Parse("{95C3B90B-A7C3-4988-9F39-16CE09399D61}"), Name = "Hand Grid" },
            };

            list.ForEach(x => x.Initialize());

            return list;
        }

        private ObservableCollection<IFilterModel> _filterModelCollection;
        public ObservableCollection<IFilterModel> FilterModelCollection
        {
            get { return _filterModelCollection; }
            set
            {
                _filterModelCollection = value;
                OnPropertyChanged(nameof(FilterModelCollection));
            }
        }

        private ObservableCollection<FilterTuple> _filterTupleCollection;
        public ObservableCollection<FilterTuple> FilterTupleCollection
        {
            get { return _filterTupleCollection; }
            set
            {
                _filterTupleCollection = value;
                OnPropertyChanged(nameof(FilterTupleCollection));
            }
        }

        #region Not used

        public Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> GetFilterModelDictionary()
        {
            throw new NotImplementedException();
        }

        public void SetFilterType(EnumFilterType filterType)
        {
            throw new NotImplementedException();
        }

        public void SpreadFilter()
        {
            throw new NotImplementedException();
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
