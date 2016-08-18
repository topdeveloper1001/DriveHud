using DriveHUD.Common;
using Microsoft.Practices.ServiceLocation;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public class HudBumperStickersSettingsViewModel : BaseRangeTypePopupViewModel
    {
        private readonly HudBumperStickersSettingsViewModelInfo viewModelInfo;
        private readonly IHudLayoutsService hudLayoutService;

        public HudBumperStickersSettingsViewModel(HudBumperStickersSettingsViewModelInfo viewModelInfo) : base()
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this.viewModelInfo = viewModelInfo;

            hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            Initialize();
        }

        private void Initialize()
        {
            bumperStickers = new ObservableCollection<HudBumperStickerType>(viewModelInfo.BumperStickers);
            selectedBumperSticker = bumperStickers.FirstOrDefault();
        }

        #region Properties

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

        protected override void Load()
        {
        }

        protected override void Save()
        {
            viewModelInfo.Save?.Invoke();
        }
        #endregion
    }
}
