using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Filters;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application.ViewModels
{
    public class FilterStandardViewModel : FilterViewModel<FilterStandardModel>
    {
        internal FilterStandardViewModel() : base(EnumViewModelType.FilterStandardViewModel)
        {
            InitializeBindings();
        }

        public override void InitializeFilterModel()
        {
            var player = StorageModel.PlayerSelectedItem;
            var gameTypes = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerGameTypes(player);

            this.FilterModel = (FilterStandardModel)FilterModelManager.FilterModelCollection.Where(x => x.GetType().Equals(typeof(FilterStandardModel))).FirstOrDefault();
            this.FilterModel.UpdateFilterSectionStakeLevelCollection(gameTypes);

            this.FilterModelClone = (FilterStandardModel)this.FilterModel.Clone();
        }

        private void InitializeBindings()
        {
            this.ButtonFilterModelStatItemSwap_CommandClick = new DelegateCommand<object>(this.ButtonFilterModelStatItemSwap_OnClick);
        }

        private void FilterModelStatItemSwap(StatItem param)
        {
            param.TriStateSwap();
        }

        #region Commands
        public ICommand ButtonFilterModelStatItemSwap_CommandClick { get; set; }
        private void ButtonFilterModelStatItemSwap_OnClick(object param)
        {
            FilterModelStatItemSwap((StatItem)param);
        }
        #endregion
    }
}
