using Model.Enums;
using Model.Filters;
using Prism.Commands;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterHandValueViewModel : FilterViewModel<FilterHandValueModel>
    {
        internal FilterHandValueViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterHandValueViewModel, service)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            this.ButtonFilterModelStatItemSwap_CommandClick = new DelegateCommand<object>(this.ButtonFilterModelStatItemSwap_OnClick);
        }

        private void FilterModelStatItemSwap(FastFilterItem param)
        {
            param.TriStateSwap();
        }

        #region Commands
        public ICommand ButtonFilterModelStatItemSwap_CommandClick { get; set; }
        private void ButtonFilterModelStatItemSwap_OnClick(object param)
        {
            FilterModelStatItemSwap((FastFilterItem)param);
        }
        #endregion
    }
}
