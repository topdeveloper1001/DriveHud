using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Filters;
using Model.Interfaces;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterStandardViewModel : FilterViewModel<FilterStandardModel>
    {
        internal FilterStandardViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterStandardViewModel, service)
        {
            InitializeBindings();
        }

        public override void InitializeFilterModel()
        {
            var player = StorageModel.PlayerSelectedItem;

            var gameTypes = player != null ?
                ServiceLocator.Current.GetInstance<IDataService>().GetPlayerGameTypes(player.PlayerIds) :
                new List<Gametypes>();

            FilterModel = (FilterStandardModel)FilterModelManager.FilterModelCollection.FirstOrDefault(x => x.GetType().Equals(typeof(FilterStandardModel)));
            FilterModel.UpdateFilterSectionStakeLevelCollection(gameTypes);

            FilterModelClone = (FilterStandardModel)FilterModel.Clone();
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