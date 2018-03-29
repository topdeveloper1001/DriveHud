using DriveHUD.Application.ViewModels.Filters;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Application.Views;
using Model.Enums;
using Model.Filters;
using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        protected override void ApplyFilters(object obj)
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.UpdateDefaultState();
            }

            Sticker.BuiltFilter = CurrentlyBuiltFilter.Clone();
            FilterHelpers.CopyFilterModelCollection(FilterModelManager.FilterModelCollection, Sticker.FilterModelCollection);
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
                }

                base.Notification = value;
            }
        }

        #endregion

    }
}
