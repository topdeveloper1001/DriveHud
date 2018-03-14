//-----------------------------------------------------------------------
// <copyright file="IFilterModelManagerService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Filters
{
    public interface IFilterModelManagerService
    {
        ReadOnlyObservableCollection<FilterTuple> FilterTupleCollection { get; set; }

        ReadOnlyObservableCollection<IFilterModel> FilterModelCollection { get; set; }

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
        Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> GetFilterModelDictionary();

        /// <summary>
        /// Getnerates the new default filter model list
        /// </summary>
        /// <returns></returns>
        ReadOnlyObservableCollection<IFilterModel> GetFilterModelsList();

        /// <summary>
        /// Gets the hash code based on all filters for the current filter type
        /// </summary>
        /// <returns>Hash code</returns>
        int GetFiltersHashCode();
    }
}