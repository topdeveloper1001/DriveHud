﻿//-----------------------------------------------------------------------
// <copyright file="FilterAdvancedViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using Model.Filters;
using ReactiveUI;
using System.Linq;
using System.Windows.Data;

namespace DriveHUD.Application.ViewModels.Filters
{
    public class FilterAdvancedViewModel : FilterViewModel<FilterAdvancedModel>
    {
        internal FilterAdvancedViewModel(IFilterModelManagerService service) :
            base(EnumViewModelType.FilterAdvancedViewModel, service)
        {
            InitializeCollectionViews();
            InitializeCommands();
        }

        #region Properties

        public CollectionView FiltersCollectionView { get; private set; }

        public CollectionView SelectedFiltersCollectionView { get; private set; }

        #endregion

        #region Commands

        public ReactiveCommand AddToSelectedFiltersCommand { get; private set; }

        public ReactiveCommand RemoveFromSelectedFiltersCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeCollectionViews()
        {
            // get default view return same object for that collection
            FiltersCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(FilterModel.Filters);

            if (FiltersCollectionView.GroupDescriptions.Count == 0)
            {
                FiltersCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FilterAdvancedItem.Stage)));
            }

            SelectedFiltersCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(FilterModel.SelectedFilters);

            if (SelectedFiltersCollectionView.GroupDescriptions.Count == 0)
            {
                SelectedFiltersCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FilterAdvancedItem.Stage)));
            }
        }

        private void InitializeCommands()
        {
            AddToSelectedFiltersCommand = ReactiveCommand.Create(() =>
            {
                var selectedItem = FilterModel.Filters.FirstOrDefault(f => f.IsSelected);

                if (selectedItem != null && !FilterModel.SelectedFilters.Any(f => f.FilterType == selectedItem.FilterType))
                {
                    var filterToAdd = selectedItem.Clone();
                    filterToAdd.IsSelected = false;

                    // if filter requires value
                    if (FilterAdvancedModel.FiltersWithValueRequired.Contains(filterToAdd.FilterType))
                    {
                        // call popup to add filter

                        FilterModel.SelectedFilters.Add(filterToAdd);
                    }
                    else
                    {
                        FilterModel.SelectedFilters.Add(filterToAdd);
                    }
                }
            });

            RemoveFromSelectedFiltersCommand = ReactiveCommand.Create(() =>
            {
                var selectedItem = FilterModel.SelectedFilters.FirstOrDefault(f => f.IsSelected);

                if (selectedItem != null)
                {
                    FilterModel.SelectedFilters.Remove(selectedItem);
                }
            });
        }

        #endregion
    }
}