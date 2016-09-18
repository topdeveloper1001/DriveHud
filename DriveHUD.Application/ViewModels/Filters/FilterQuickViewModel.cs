using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterQuickViewModel : FilterViewModel<FilterQuickModel>
    {
        internal FilterQuickViewModel() : base(EnumViewModelType.FilterQuickViewModel)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.ButtonFilterModelStatItemSwap_CommandClick = new RelayCommand(this.ButtonFilterModelStatItemSwap_OnClick);
        }

        private void FilterModelStatItemSwap(QuickFilterItem param)
        {
            param.TriStateSwap();
        }

        #region Commands
        public ICommand ButtonFilterModelStatItemSwap_CommandClick { get; set; }
        private void ButtonFilterModelStatItemSwap_OnClick(object param)
        {
            FilterModelStatItemSwap((QuickFilterItem)param);
        }
        #endregion
    }
}
