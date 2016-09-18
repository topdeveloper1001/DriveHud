using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Filters
{
    public interface IFilterDataService
    {
        void SaveFilter(ICollection<IFilterModel> filtersCollection, string path);

        ICollection<IFilterModel> LoadFilter(string path);

        void LoadDefaultFilter(Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> _filtersDictionary);

        void SaveDefaultFilter(Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> _filtersDictionary);
    }
}
