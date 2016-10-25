using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Filters
{
    public interface IFilterModelManagerService
    {
        ObservableCollection<FilterTuple> FilterTupleCollection { get; set; }
        ObservableCollection<IFilterModel> FilterModelCollection { get; set; }

        /// <summary>
        /// Determines what filter models collection will be returned when <see cref="FilterModelCollection"/> is called.
        /// </summary>
        /// <param name="filterType"><see cref="EnumFilterType.Cash"/> for cash filters, <see cref="EnumFilterType.Tournament"/>
        /// for tournament filters. Cash is default</param>
        void SetFilterType(EnumFilterType filterType);

        /// <summary>
        /// Copies data from specified in <see cref="SetFilterType(EnumFilterType)"/> collection into all other available collections
        /// </summary>
        void SpreadFilter();

        /// <summary>
        /// Gets dictionary containing all available filter model collections
        /// </summary>
        /// <returns></returns>
        Dictionary<EnumFilterType, ObservableCollection<IFilterModel>> GetFilterModelDictionary();

        /// <summary>
        /// Getnerates the new default filter model list
        /// </summary>
        /// <returns></returns>
        ObservableCollection<IFilterModel> GetFilterModelsList();
    }
}
