using DriveHUD.Application.Views;
using DriveHUD.Common.Infrastructure.Base;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public class FilterHandGridViewModel : FilterViewModel<FilterHandGridModel>
    {
        private FilterHoleCardsView _filterHoleCards;

        public FilterHoleCardsView FilterHoleCards
        {
            get { return _filterHoleCards; }
            set
            {
                SetProperty(ref _filterHoleCards, value);
            }
        }

        private FilterOmahaHandGridView _filterOmahaHandGrid;

        public FilterOmahaHandGridView FilterOmahaHandGrid
        {
            get { return _filterOmahaHandGrid; }
            set
            {
                SetProperty(ref _filterOmahaHandGrid, value);
            }
        }

        private object _selectedViewModel;

        public object SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                SetProperty(ref _selectedViewModel, value);
            }
        }

        internal FilterHandGridViewModel() : base(Model.Enums.EnumViewModelType.FilterHandGridViewModel)
        {
            FilterHoleCards = new FilterHoleCardsView();
            FilterOmahaHandGrid = new FilterOmahaHandGridView();

            SwitchView();
        }

        public override void RestoreDefaultState()
        {
            FilterHoleCards?.ViewModel?.RestoreDefaultState();
            FilterOmahaHandGrid?.ViewModel?.RestoreDefaultState();
        }

        public override void UpdateDefaultState()
        {
            FilterHoleCards?.ViewModel?.UpdateDefaultState();
            FilterOmahaHandGrid?.ViewModel?.UpdateDefaultState();
        }

        public override void InitializeFilterModel()
        {
            FilterHoleCards?.ViewModel?.InitializeFilterModel();
            FilterOmahaHandGrid?.ViewModel?.InitializeFilterModel();
        }

        internal void SwitchView(bool isHoldem = true)
        {
            if (isHoldem)
            {
                SelectedViewModel = FilterHoleCards;
            }
            else
            {
                SelectedViewModel = FilterOmahaHandGrid;
            }
        }
    }
}
