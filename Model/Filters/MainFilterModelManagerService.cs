//-----------------------------------------------------------------------
// <copyright file="MainFilterModelManagerService.cs" company="Ace Poker Solutions">
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
using System.Linq;

namespace Model.Filters
{
    internal class MainFilterModelManagerService : BindableBase, IFilterModelManagerService
    {
        public MainFilterModelManagerService()
        {
            Initialize();
        }

        private void Initialize()
        {
            enumFilterType = EnumFilterType.Cash;

            FilterTupleCollection = new ReadOnlyObservableCollection<FilterTuple>(new ObservableCollection<FilterTuple>
            (
                new List<FilterTuple>()
                {
                    new FilterTuple() { Name = "Standard Filters", ModelType = EnumFilterModelType.FilterStandardModel, ViewModelType = EnumViewModelType.FilterStandardViewModel },
                    new FilterTuple() { Name = "Hole Cards", ModelType = EnumFilterModelType.FilterHandGridModel, ViewModelType = EnumViewModelType.FilterHandGridViewModel, },
                    new FilterTuple() { Name = "Hand Value", ModelType = EnumFilterModelType.FilterHandValueModel, ViewModelType = EnumViewModelType.FilterHandValueViewModel, },
                    new FilterTuple() { Name = "Board Texture", ModelType = EnumFilterModelType.FilterBoardTextureModel, ViewModelType = EnumViewModelType.FilterBoardTextureViewModel },
                    new FilterTuple() { Name = "Hand Action", ModelType = EnumFilterModelType.FilterHandActionModel, ViewModelType = EnumViewModelType.FilterHandActionViewModel, },
                    new FilterTuple() { Name = "Advanced", ModelType = EnumFilterModelType.FilterAdvancedModel, ViewModelType = EnumViewModelType.FilterAdvancedViewModel, },
                    new FilterTuple() { Name = "Quick Filters", ModelType = EnumFilterModelType.FilterQuickModel, ViewModelType = EnumViewModelType.FilterQuickViewModel, },
                }
            ));

            filterModelCollections = new Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>>()
            {
                { EnumFilterType.Cash, GetFilterModelsList() },
                { EnumFilterType.Tournament, GetFilterModelsList() }
            };
        }

        public ReadOnlyObservableCollection<IFilterModel> GetFilterModelsList()
        {
            var list = new ReadOnlyObservableCollection<IFilterModel>(new ObservableCollection<IFilterModel>
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
                    new FilterAdvancedModel { Id = Guid.Parse("1EC8735E-F8A2-46BB-8687-9211DA7A97A4") }
            });

            list.ForEach(x => x.Initialize());

            return list;
        }

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
     
        public void SpreadFilter()
        {
            EnumFilterType fromType = enumFilterType;

            for (int i = 0; i < filterModelCollections.Count; i++)
            {
                var key = filterModelCollections.ElementAt(i).Key;

                if (key == fromType)
                {
                    continue;
                }

                var filterCollection = filterModelCollections[key];
                var fromCollection = filterModelCollections[fromType];

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

        public Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> GetFilterModelDictionary()
        {
            return filterModelCollections;
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

        #region Properties

        private EnumFilterType enumFilterType;

        public Dictionary<EnumFilterType, ReadOnlyObservableCollection<IFilterModel>> filterModelCollections;

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

        public ReadOnlyObservableCollection<IFilterModel> FilterModelCollection
        {
            get
            {
                return filterModelCollections[enumFilterType];
            }
            set
            {
                if (!filterModelCollections.ContainsKey(enumFilterType))
                {
                    return;
                }

                filterModelCollections[enumFilterType] = value;

                RaisePropertyChanged();
            }
        }

        #endregion      
    }
}