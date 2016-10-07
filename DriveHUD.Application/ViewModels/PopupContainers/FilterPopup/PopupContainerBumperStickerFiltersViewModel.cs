using DriveHUD.Application.ViewModels.Filters;
using DriveHUD.Application.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using Prism.Interactivity.InteractionRequest;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using Model.Filters;

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
                        new FilterHandGridView(FilterModelManager),
                    });
        }

        #endregion

        #region Methods

        protected override void RestoreDefaultFiltersState()
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.RestoreDefaultState();
            }
        }

        protected override void Apply_OnClick(object obj)
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.UpdateDefaultState();
            }

            Sticker.BuiltFilter = CurrentlyBuiltFilter.Clone();
            CopyFilterModelCollection(FilterModelManager.FilterModelCollection, Sticker.FilterModelCollection);

            // var currentFilter = GetCurrentFilter();
        }

        private void CopyFilterModelCollection(ObservableCollection<IFilterModel> from, ObservableCollection<IFilterModel> to)
        {
            foreach (var filter in to)
            {
                var filterToCopy = from.FirstOrDefault(x => x.GetType() == filter.GetType());
                if (filterToCopy != null)
                {
                    filter.LoadFilter(filterToCopy);
                }
            }
        }

        #endregion

        #region Properties

        private HudBumperStickerType sticker;
        public HudBumperStickerType Sticker
        {
            get { return sticker; }
            set
            {
                SetProperty(ref sticker, value);
            }
        }


        public override INotification Notification
        {
            get { return base.Notification; }
            set
            {
                var notification = value as PopupContainerStickersFiltersViewModelNotification;
                if (notification != null && notification.Sticker != null)
                {
                    this.Sticker = notification.Sticker;
                    CopyFilterModelCollection(Sticker.FilterModelCollection, FilterModelManager.FilterModelCollection);
                }

                base.Notification = value;
            }
        }

        #endregion

    }
}
