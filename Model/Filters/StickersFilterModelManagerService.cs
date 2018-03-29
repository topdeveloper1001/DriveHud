//-----------------------------------------------------------------------
// <copyright file="StickersFilterModelManagerService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using Model.Enums;
using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Filters
{
    internal class StickersFilterModelManagerService : BindableBase, IFilterModelManagerService
    {
        public StickersFilterModelManagerService()
        {
            FilterTupleCollection = new ReadOnlyObservableCollection<FilterTuple>(new ObservableCollection<FilterTuple>
                {
                    new FilterTuple { Name = "Hole Cards", ModelType = EnumFilterModelType.FilterHandGridModel, ViewModelType = EnumViewModelType.FilterHandGridViewModel, },
                    new FilterTuple { Name = "Hand Value", ModelType = EnumFilterModelType.FilterHandValueModel, ViewModelType = EnumViewModelType.FilterHandValueViewModel, },
                    new FilterTuple { Name = "Board Texture", ModelType = EnumFilterModelType.FilterBoardTextureModel, ViewModelType = EnumViewModelType.FilterBoardTextureViewModel },
                    new FilterTuple { Name = "Hand Action", ModelType = EnumFilterModelType.FilterHandActionModel, ViewModelType = EnumViewModelType.FilterHandActionViewModel, },
                    new FilterTuple { Name = "Quick Filters", ModelType = EnumFilterModelType.FilterQuickModel, ViewModelType = EnumViewModelType.FilterQuickViewModel, },
                });

            FilterModelCollection = GetFilterModelsList();
        }

        public ReadOnlyObservableCollection<IFilterModel> GetFilterModelsList()
        {
            var list = new ReadOnlyObservableCollection<IFilterModel>(new ObservableCollection<IFilterModel>
            {
                new FilterHoleCardsModel { Id = Guid.Parse("{064EA6DE-BCD0-40EE-9EB8-31BB1B97873C}") },
                new FilterHandValueModel { Id = Guid.Parse("{A5665500-35E9-40FF-AEFE-27FDD6826437}") },
                new FilterBoardTextureModel { Id = Guid.Parse("{6138140D-D950-4AB9-B2B1-00380FAFC179}") },
                new FilterHandActionModel { Id = Guid.Parse("{01C28033-1A53-40AD-B10B-B6F85DB4AC92}") },
                new FilterQuickModel { Id = Guid.Parse("{D8AE4CE4-7FFE-4D68-B99E-4DCED4EE8812}") },
                new FilterOmahaHandGridModel { Id = Guid.Parse("{ABFBCA7F-8DB7-437F-8DEE-A26C5EADF6B3}") },
                new FilterHandGridModel { Id = Guid.Parse("{95C3B90B-A7C3-4988-9F39-16CE09399D61}"), Name = "Hand Grid" },
            });

            list.ForEach(x => x.Initialize());

            return list;
        }

        private ReadOnlyObservableCollection<IFilterModel> filterModelCollection;

        public ReadOnlyObservableCollection<IFilterModel> FilterModelCollection
        {
            get
            {
                return filterModelCollection;
            }
            set
            {
                SetProperty(ref filterModelCollection, value);
            }
        }

        private ReadOnlyObservableCollection<FilterTuple> filterTupleCollection;

        public ReadOnlyObservableCollection<FilterTuple> FilterTupleCollection
        {
            get
            {
                return filterTupleCollection;
            }
            set
            {
                SetProperty(ref filterTupleCollection, value);

            }
        }

        private EnumFilterType enumFilterType;

        public EnumFilterType FilterType
        {
            get
            {
                return enumFilterType;
            }
            set
            {
                SetProperty(ref enumFilterType, value);
            }
        }

        #region Not used

        public Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> GetFilterModelDictionary()
        {
            throw new NotImplementedException();
        }      

        public void SpreadFilter()
        {
            throw new NotImplementedException();
        }

        public int GetFiltersHashCode()
        {
            unchecked
            {
                var hashcode = 23;

                FilterModelCollection.ForEach(model =>
                {
                    var modelHashCode = JsonConvert.SerializeObject(model);
                    hashcode = (hashcode * 31) + modelHashCode.GetHashCode();
                });

                return hashcode;
            }
        }

        #endregion
    }
}