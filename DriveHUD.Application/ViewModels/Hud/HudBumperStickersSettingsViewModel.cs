//-----------------------------------------------------------------------
// <copyright file="HudBumperStickersSettingsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common;
using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.Enums;
using Model.Filters;
using Model.Hud;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudBumperStickersSettingsViewModel : BaseRangeTypePopupViewModel
    {
        private readonly HudBumperStickersSettingsViewModelInfo viewModelInfo;
        private readonly IHudLayoutsService hudLayoutService;
        private readonly IHudBumperStickerService bumperStickerTypeService;

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

            hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            bumperStickerTypeService = ServiceLocator.Current.GetInstance<IHudBumperStickerService>();

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

            SelectColorCommand = ReactiveCommand.Create<object>(x => SelectColor(x));
            PickerSelectColorCommand = ReactiveCommand.Create(() => IsColorPickerPopupOpened = false);
            FilterCommand = ReactiveCommand.Create(() => PopupFiltersRequestExecute(Service.FilterTupleCollection.FirstOrDefault(f => f.ModelType == EnumFilterModelType.FilterHandGridModel)));
            ButtonFilterModelSectionRemoveCommand = ReactiveCommand.Create<object>(x => ButtonFilterModelSectionRemove(x));

            var canDelete = this.WhenAny(x => x.SelectedBumperSticker, x => x.Value != null);

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedBumperSticker != null)
                {
                    BumperStickers.Remove(SelectedBumperSticker);
                    SelectedBumperSticker = BumperStickers.FirstOrDefault();
                }
            }, canDelete);

            ResetCommand = ReactiveCommand.Create(() =>
            {
                var defaultBumperStickers = bumperStickerTypeService.CreateDefaultBumperStickerTypes();

                var defaultBumperSticker = defaultBumperStickers.FirstOrDefault(p => p.Name == SelectedBumperSticker.Name);

                if (defaultBumperSticker == null)
                {
                    return;
                }

                SelectedBumperSticker.StatsToMerge = defaultBumperSticker.Stats;
            }, canDelete);

            ExportCommand = ReactiveCommand.Create(() => Export(new[] { SelectedBumperSticker }), canDelete);
            ExportAllCommand = ReactiveCommand.Create(() => Export(bumperStickers));
            ImportCommand = ReactiveCommand.Create(() => Import());
        }

        #region Commands

        public ReactiveCommand ButtonFilterModelSectionRemoveCommand { get; private set; }

        public ReactiveCommand SelectColorCommand { get; private set; }

        public ReactiveCommand PickerSelectColorCommand { get; private set; }

        public ReactiveCommand FilterCommand { get; private set; }

        public ReactiveCommand ResetCommand { get; private set; }

        public ReactiveCommand DeleteCommand { get; private set; }

        public ReactiveCommand ExportCommand { get; private set; }

        public ReactiveCommand ExportAllCommand { get; private set; }

        public ReactiveCommand ImportCommand { get; private set; }

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

            SelectedBumperSticker.BuiltFilter = new BuiltFilterModel(FilterServices.Stickers);
            SelectedBumperSticker.BuiltFilter.BindFilterSectionCollection();

            if (SelectedBumperSticker.FilterModelCollection == null || !SelectedBumperSticker.FilterModelCollection.Any())
            {
                SelectedBumperSticker.FilterModelCollection = new IFilterModelCollection(Service.GetFilterModelsList());
            }

            FilterHelpers.CopyFilterModelCollection(SelectedBumperSticker?.FilterModelCollection, Service.FilterModelCollection);
        }

        private void PopupFiltersRequestExecute(FilterTuple filterTuple)
        {
            var notification = new PopupContainerStickersFiltersViewModelNotification
            {
                Title = "Filters",
                FilterTuple = filterTuple,
                Sticker = SelectedBumperSticker
            };

            PopupFiltersRequest.Raise(notification,
                returned =>
                {
                    SelectedBumperSticker.BuiltFilter = new BuiltFilterModel(FilterServices.Stickers);
                    SelectedBumperSticker.BuiltFilter.BindFilterSectionCollection();
                });
        }

        private void Import()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = CommonResourceManager.Instance.GetResourceString("SystemSettings_BumperStickerFileDialogFilter")
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var importedBumperStickerTypes = hudLayoutService.ImportBumperStickerType(openFileDialog.FileName);

            if (importedBumperStickerTypes == null || importedBumperStickerTypes.Length == 0)
            {
                return;
            }

            var bumperStickerTypesMap = (from importedPlayerType in importedBumperStickerTypes
                                         join bumperStickerType in BumperStickers on importedPlayerType.Name equals bumperStickerType.Name into gj
                                         from grouped in gj.DefaultIfEmpty()
                                         select new { ImportedPlayerType = importedPlayerType, ExistingPlayerType = grouped }).ToArray();

            foreach (var bumperStickerTypeMapItem in bumperStickerTypesMap)
            {
                if (bumperStickerTypeMapItem.ExistingPlayerType == null)
                {
                    BumperStickers.Add(bumperStickerTypeMapItem.ImportedPlayerType);
                    continue;
                }

                bumperStickerTypeMapItem.ExistingPlayerType.MergeWith(bumperStickerTypeMapItem.ImportedPlayerType);
            }
        }

        private void Export(IEnumerable<HudBumperStickerType> bumperStickerTypes)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = CommonResourceManager.Instance.GetResourceString("SystemSettings_BumperStickerFileDialogFilter")
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                hudLayoutService.ExportBumperStickerType(bumperStickerTypes.ToArray(), saveFileDialog.FileName);
            }
        }

        #endregion
    }
}