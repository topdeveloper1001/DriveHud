using Model.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Filters
{
    public interface IFilterDataService
    {
        void SaveFilter(ICollection<IFilterModel> filtersCollection, string path);

        ICollection<IFilterModel> LoadFilter(string path);

        void LoadDefaultFilter(Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> _filtersDictionary);

        void SaveDefaultFilter(Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> _filtersDictionary);
    }
}