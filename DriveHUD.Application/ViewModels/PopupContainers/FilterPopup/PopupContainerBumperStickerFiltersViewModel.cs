using DriveHUD.Application.ViewModels.Filters;
using DriveHUD.Application.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerBumperStickerFiltersViewModel : PopupContainerBaseFilterViewModel
    {
        #region Fields

        protected override string FilterFileExtension
        {
            // drivehud sticker filter
            get { return ".dsf"; }
        }

        protected override FilterServices FilterService
        {
            get { return FilterServices.Stickers; }
        }

        #endregion

        #region Initialization

        public PopupContainerBumperStickerFiltersViewModel()
        {
            InitializeBindings();
        }

        protected override void InitializeViewModelCollection()
        {
            this.FilterViewCollection = new ObservableCollection<IFilterView>
                (
                    new List<IFilterView>()
                    {
                        new FilterBoardTextureView(FilterModelManager),
                        new FilterHandActionView(FilterModelManager),
                        new FilterHandValueView(FilterModelManager),
                        new FilterQuickView(FilterModelManager),
                        new FilterDateView(FilterModelManager),
                        new FilterHandGridView(FilterModelManager),
                    });
        }

        #endregion

        protected override void Apply_OnClick(object obj)
        {
            throw new NotImplementedException();
        }

    }
}
