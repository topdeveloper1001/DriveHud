using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Filters;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DriveHUD.Application.ViewModels
{
    public class HudBumperStickersSettingsViewModel : BaseRangeTypePopupViewModel
    {
        private readonly HudBumperStickersSettingsViewModelInfo viewModelInfo;

        private IFilterModelManagerService Service
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Stickers.ToString());
            }
        }

        public HudBumperStickersSettingsViewModel(HudBumperStickersSettingsViewModelInfo viewModelInfo) : base()
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this.viewModelInfo = viewModelInfo;

            Initialize();
        }

        private void Initialize()
        {
            PopupFiltersRequest = new InteractionRequest<PopupContainerFiltersViewModelNotification>();

            BumperStickers = new ObservableCollection<HudBumperStickerType>(viewModelInfo.BumperStickers);
            SelectedBumperSticker = bumperStickers.FirstOrDefault();
        }

        protected override void InitializeCommands()
        {
            base.InitializeCommands();

            SelectColorCommand = ReactiveCommand.Create();
            SelectColorCommand.Subscribe(x => SelectColor(x));

            PickerSelectColorCommand = ReactiveCommand.Create();
            PickerSelectColorCommand.Subscribe(x => IsColorPickerPopupOpened = false);

            FilterCommand = ReactiveCommand.Create();
            FilterCommand.Subscribe(x => PopupFiltersRequestExecute(Service.FilterTupleCollection.FirstOrDefault(f => f.ModelType == EnumFilterModelType.FilterHandGridModel)));

            ButtonFilterModelSectionRemoveCommand = ReactiveCommand.Create();
            ButtonFilterModelSectionRemoveCommand.Subscribe(x => ButtonFilterModelSectionRemove(x));
        }

        #region Commands

        public ReactiveCommand<object> ButtonFilterModelSectionRemoveCommand { get; private set; }

        public ReactiveCommand<object> SelectColorCommand { get; private set; }

        public ReactiveCommand<object> PickerSelectColorCommand { get; private set; }

        public ReactiveCommand<object> FilterCommand { get; private set; }

        #endregion

        #region Properties

        public InteractionRequest<PopupContainerFiltersViewModelNotification> PopupFiltersRequest { get; private set; }

        private bool isColorPickerPopupOpened;

        public bool IsColorPickerPopupOpened
        {
            get
            {
                return isColorPickerPopupOpened;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isColorPickerPopupOpened, value);
            }
        }

        private ObservableCollection<HudBumperStickerType> bumperStickers;

        public ObservableCollection<HudBumperStickerType> BumperStickers
        {
            get
            {
                return bumperStickers;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref bumperStickers, value);
            }
        }

        private HudBumperStickerType selectedBumperSticker;

        public HudBumperStickerType SelectedBumperSticker
        {
            get
            {
                return selectedBumperSticker;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedBumperSticker, value);
                InitializeSelectedBumperSticker();
            }
        }

        #endregion

        #region Validation

        private bool Validate()
        {
            var groupedByName = (from bumperSticker in BumperStickers
                                 where !string.IsNullOrWhiteSpace(bumperSticker.Name)
                                 group bumperSticker by bumperSticker.Name into grouped
                                 select new { Name = grouped.Key, Count = grouped.Count() });

            var validationResult = BumperStickers.All(x => !string.IsNullOrWhiteSpace(x.Name)) && groupedByName.All(x => x.Count < 2);

            return validationResult;
        }

        #endregion

        #region Infrastructure

        protected override IObservable<bool> CanSave()
        {
            return this.WhenAny(x => x.SelectedBumperSticker.Name, y => !string.IsNullOrWhiteSpace(y.Value) && Validate());
        }

        protected override void Create()
        {
            var hudBumperStickerType = new HudBumperStickerType(true);
            BumperStickers.Add(hudBumperStickerType);
            SelectedBumperSticker = hudBumperStickerType;
        }

        protected override void Save()
        {
            viewModelInfo.Save?.Invoke();
        }

        private void SelectColor(object statInfoValueRange)
        {
            IsColorPickerPopupOpened = true;
        }

        private void ButtonFilterModelSectionRemove(object x)
        {
            if (SelectedBumperSticker == null || SelectedBumperSticker.BuiltFilter == null)
            {
                return;
            }

            if (x is FilterSectionItem)
            {
                SelectedBumperSticker.BuiltFilter.RemoveBuiltFilterItem(x as FilterSectionItem);
                FilterHelpers.CopyFilterModelCollection(Service.FilterModelCollection, SelectedBumperSticker?.FilterModelCollection);
            }
        }

        private void InitializeSelectedBumperSticker()
        {
            if (SelectedBumperSticker == null)
                return;

            SelectedBumperSticker.BuiltFilter = new BuiltFilterModel(Model.Enums.FilterServices.Stickers);
            SelectedBumperSticker.BuiltFilter.BindFilterSectionCollection();

            if (SelectedBumperSticker.FilterModelCollection == null || !SelectedBumperSticker.FilterModelCollection.Any())
            {
                SelectedBumperSticker.FilterModelCollection = new IFilterModelCollection(Service.GetFilterModelsList());
            }

            FilterHelpers.CopyFilterModelCollection(SelectedBumperSticker?.FilterModelCollection, Service.FilterModelCollection);
        }

        private void PopupFiltersRequestExecute(FilterTuple filterTuple)
        {
            PopupContainerStickersFiltersViewModelNotification notification = new PopupContainerStickersFiltersViewModelNotification();

            notification.Title = "Filters";
            notification.FilterTuple = filterTuple;
            notification.Sticker = SelectedBumperSticker;

            this.PopupFiltersRequest.Raise(notification,
                returned =>
                {
                    SelectedBumperSticker.BuiltFilter = new BuiltFilterModel(Model.Enums.FilterServices.Stickers);
                    SelectedBumperSticker.BuiltFilter.BindFilterSectionCollection();
                });
        }


        #endregion
    }
}
