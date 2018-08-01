//-----------------------------------------------------------------------
// <copyright file="HudUploadToStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Licensing;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Common.Wpf.Validation;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.AppStore.HudStore;
using Model.AppStore.HudStore.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudUploadToStoreViewModel : WindowViewModel<HudUploadToStoreViewModel,
        IHudStoreUploadModel>, IHudUploadToStoreViewModel, INotifyDataErrorInfo
    {
        private const int MaxImages = 8;

        private CompositeDisposable selectableDataSubscription;

        private HudLayoutInfoV2 hudLayout;

        static HudUploadToStoreViewModel()
        {
            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Name),
                new LocalizableString("Common_HudUploadToStoreView_NameMustBeNotEmpty"),
                x => !string.IsNullOrEmpty(x.Name)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Name),
                new LocalizableString("Common_HudUploadToStoreView_NameMustBeLongerThan10"),
                x => x.Name != null && x.Name.Length >= 10 && x.Name.Length <= 50));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
               nameof(Name),
               new LocalizableString("Common_HudUploadToStoreView_NameIsAlreadyInUse"),
               x => string.IsNullOrEmpty(x.Name) || x.Model != null && x.Model.LayoutsNamesInUse != null && !x.Model.LayoutsNamesInUse.Contains(x.Name)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Description),
                new LocalizableString("Common_HudUploadToStoreView_DescriptionMustBeNotEmpty"),
                x => !string.IsNullOrEmpty(x.Description)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Description),
                new LocalizableString("Common_HudUploadToStoreView_DescriptionMustBeNotLongerThan1000"),
                x => !string.IsNullOrEmpty(x.Description) && x.Description.Length < 1001));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Cost),
                new LocalizableString("Common_HudUploadToStoreView_CostMustBeNotNegative"),
                x => x.Cost >= 0));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(GameVariants),
                new LocalizableString("Common_HudUploadToStoreView_GameVariantMustBeSelected"),
                x => x.GameVariants != null && x.GameVariants.Any(p => p.IsSelected)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(GameTypes),
                new LocalizableString("Common_HudUploadToStoreView_GameTypeMustBeSelected"),
                x => x.GameTypes != null && x.GameTypes.Any(p => p.IsSelected)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(TableTypes),
                new LocalizableString("Common_HudUploadToStoreView_TableTypeMustBeSelected"),
                x => x.TableTypes != null && x.TableTypes.Any(p => p.IsSelected)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Images),
                new LocalizableString("Common_HudUploadToStoreView_AtLeastOneImageMustBeSpecified"),
                x => x.Images.Count != 0));
        }

        public HudUploadToStoreViewModel()
        {
            images = new ReactiveList<HudUploadToStoreImage>
            {
                ChangeTrackingEnabled = true
            };

            images.Changed.Subscribe(x => ApplyRules(nameof(Images)));
        }

        public override void Configure(object viewModelInfo)
        {
            if (!(viewModelInfo is HudUploadToStoreViewModelInfo hudUploadToStoreViewModelInfo))
            {
                throw new ArgumentException(nameof(viewModelInfo));
            }

            hudLayout = hudUploadToStoreViewModelInfo.Layout;

            InitializeModelAsync(() => Model.Load());
        }

        #region Properties

        private IHudLayoutsService HudLayoutsService => ServiceLocator.Current.GetInstance<IHudLayoutsService>();

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
            }
        }

        private decimal cost;

        public decimal Cost
        {
            get
            {
                return cost;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref cost, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<GameVariant>> gameVariants;

        public ReactiveList<SelectableItemViewModel<GameVariant>> GameVariants
        {
            get
            {
                return gameVariants;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref gameVariants, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<GameType>> gameTypes;

        public ReactiveList<SelectableItemViewModel<GameType>> GameTypes
        {
            get
            {
                return gameTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref gameTypes, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<TableType>> tableTypes;

        public ReactiveList<SelectableItemViewModel<TableType>> TableTypes
        {
            get
            {
                return tableTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref tableTypes, value);
            }
        }

        private readonly ReactiveList<HudUploadToStoreImage> images;

        public ReactiveList<HudUploadToStoreImage> Images
        {
            get
            {
                return images;
            }
        }

        private HudUploadToStoreBusyStatus busyStatus;

        public HudUploadToStoreBusyStatus BusyStatus
        {
            get
            {
                return busyStatus;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref busyStatus, value);
            }
        }

        private string message;

        public string Message
        {
            get
            {
                return message;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref message, value);
                this.RaisePropertyChanged(nameof(IsLayoutSelectionVisible));
                this.RaisePropertyChanged(nameof(IsUploadFormVisible));
            }
        }

        private bool isSubmitButtonVisible;

        public bool IsSubmitButtonVisible
        {
            get
            {
                return isSubmitButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isSubmitButtonVisible, value);
            }
        }

        private bool isResetButtonVisible;

        public bool IsResetButtonVisible
        {
            get
            {
                return isResetButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isResetButtonVisible, value);
            }
        }

        private bool isRetryButtonVisible;

        public bool IsRetryButtonVisible
        {
            get
            {
                return isRetryButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isRetryButtonVisible, value);
            }
        }

        private bool isBackButtonVisible;

        public bool IsBackButtonVisible
        {
            get
            {
                return isBackButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isBackButtonVisible, value);
            }
        }

        private bool isCancelButtonVisible;

        public bool IsCancelButtonVisible
        {
            get
            {
                return isCancelButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isCancelButtonVisible, value);
            }
        }

        private bool isCloseButtonVisible;

        public bool IsCloseButtonVisible
        {
            get
            {
                return isCloseButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isCloseButtonVisible, value);
            }
        }

        private bool isSelectLayoutButtonVisible;

        public bool IsSelectLayoutButtonVisible
        {
            get
            {
                return isSelectLayoutButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isSelectLayoutButtonVisible, value);
            }
        }

        private EnumTableType currentLayoutTableType;

        public EnumTableType CurrentLayoutTableType
        {
            get
            {
                return currentLayoutTableType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref currentLayoutTableType, value);
                Layouts = new ReactiveList<HudLayoutInfoV2>(HudLayoutsService.GetAllLayouts(CurrentLayoutTableType));
                CurrentLayout = Layouts.FirstOrDefault();
            }
        }

        private ReactiveList<EnumTableType> layoutTableTypes;

        public ReactiveList<EnumTableType> LayoutTableTypes
        {
            get
            {
                return layoutTableTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref layoutTableTypes, value);
            }
        }

        private ReactiveList<HudLayoutInfoV2> layouts;

        public ReactiveList<HudLayoutInfoV2> Layouts
        {
            get
            {
                return layouts;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref layouts, value);
            }
        }

        private HudLayoutInfoV2 currentLayout;

        public HudLayoutInfoV2 CurrentLayout
        {
            get
            {
                return currentLayout;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref currentLayout, value);
            }
        }

        public bool IsLayoutSelectionVisible
        {
            get
            {
                return string.IsNullOrEmpty(Message) && hudLayout == null;
            }
        }

        public bool IsUploadFormVisible
        {
            get
            {
                return string.IsNullOrEmpty(Message) && !IsLayoutSelectionVisible;
            }
        }

        #endregion

        #region Command

        public ReactiveCommand SubmitCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand ResetCommand { get; private set; }

        public ReactiveCommand AddImageCommand { get; private set; }

        public ReactiveCommand RemoveImageCommand { get; private set; }

        public ReactiveCommand BackCommand { get; private set; }

        public ReactiveCommand SelectLayoutCommand { get; private set; }

        #endregion

        #region Infrastructure

        protected override void InitializeCommands()
        {
            var canSubmit = Observable.CombineLatest(
                this.WhenAny(x => x.HasErrors, x => x.IsValidating, (x1, x2) => !HasErrors && !IsValidating),
                images.Changed.Select(x => true).Merge(images.ItemChanged.Select(x => true)).Select(x => images.All(p => !p.HasErrors && !p.IsValidating)),
                (x1, x2) => x1 && x2);

            SubmitCommand = ReactiveCommand.Create(() => Upload(), canSubmit);

            CancelCommand = ReactiveCommand.Create(() => OnClosed());
            ResetCommand = ReactiveCommand.Create(() => Reset());

            var canAddImage = images.Changed.Select(x => images.Count <= MaxImages).StartWith(true);
            AddImageCommand = ReactiveCommand.Create(() => AddImage(), canAddImage);

            var canRemoveImage = images.ItemChanged.Select(x => images.Any(p => p.IsSelected)).StartWith(false);
            RemoveImageCommand = ReactiveCommand.Create(() => RemoveImage(), canRemoveImage);

            BackCommand = ReactiveCommand.Create(() =>
            {
                Message = string.Empty;
                IsRetryButtonVisible = false;
                IsSubmitButtonVisible = true;
            });

            var canSelect = this.WhenAny(x => x.CurrentLayout, x => x.Value != null);

            SelectLayoutCommand = ReactiveCommand.Create(() =>
            {
                hudLayout = CurrentLayout;

                IsSelectLayoutButtonVisible = false;
                IsSubmitButtonVisible = true;
                IsResetButtonVisible = true;

                SelectDefaultTableType();

                Name = hudLayout.Name;

                ApplyRules();

                this.RaisePropertyChanged(nameof(IsLayoutSelectionVisible));
                this.RaisePropertyChanged(nameof(IsUploadFormVisible));
            }, canSelect);
        }

        protected override void ModelInitialized(Exception ex)
        {
            Initialize();

            if (ex != null)
            {
                LogProvider.Log.Error(this, "Could not initialize model.", ex);
                Message = LocalizableString.ToString("Common_HudUploadToStoreView_ModelNotLoaded", ex.Message);
                IsCloseButtonVisible = true;
                return;
            }

            IsCancelButtonVisible = true;

            if (hudLayout != null)
            {
                IsSubmitButtonVisible = true;
                IsResetButtonVisible = true;
            }

            Name = hudLayout?.Name;

            base.ModelInitialized(ex);
            InitializeSelectableData();
        }

        private void InitializeSelectableData()
        {
            selectableDataSubscription?.Dispose();
            selectableDataSubscription = new CompositeDisposable();

            var allItemName = CommonResourceManager.Instance.GetResourceString("Common_HudUploadToStoreView_All");

            GameVariants = InitializeSelectableData(Model.GameVariants, () => new GameVariant { Name = allItemName }, nameof(GameVariants), true);
            GameTypes = InitializeSelectableData(Model.GameTypes, () => new GameType { Name = allItemName }, nameof(GameTypes), true);
            TableTypes = InitializeSelectableData(Model.TableTypes, () => new TableType { Name = allItemName }, nameof(TableTypes), false);

            if (hudLayout == null)
            {
                IsSelectLayoutButtonVisible = true;
                LayoutTableTypes = new ReactiveList<EnumTableType>(Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>());
                CurrentLayoutTableType = LayoutTableTypes.FirstOrDefault();
                return;
            }

            SelectDefaultTableType();
        }

        private void SelectDefaultTableType()
        {
            var tableTypeToSelect = TableTypes.FirstOrDefault(x => x.Item.MaxPlayers == (int)hudLayout.TableType);

            if (tableTypeToSelect != null)
            {
                tableTypeToSelect.IsSelected = true;
            }
        }

        private ReactiveList<SelectableItemViewModel<T>> InitializeSelectableData<T>(IEnumerable<T> source, Func<T> creator, string propertyName, bool isAllSelected = false)
        {
            var allItem = new SelectableItemViewModel<T>(creator()) { IsAll = true };

            var data = new ReactiveList<SelectableItemViewModel<T>>(source.Select(x => new SelectableItemViewModel<T>(x)))
            {
                ChangeTrackingEnabled = true
            };

            data.Insert(0, allItem);

            var ignoreSelectionChanged = false;

            selectableDataSubscription.Add(
                allItem.Changed
                    .Where(x => x.PropertyName == nameof(SelectableItemViewModel<T>.IsSelected))
                    .Subscribe(x =>
                    {
                        if (!ignoreSelectionChanged)
                        {
                            data.ChangeTrackingEnabled = false;
                            data.Where(p => !ReferenceEquals(allItem, p)).ForEach(p => p.IsSelected = allItem.IsSelected);
                            data.ChangeTrackingEnabled = true;
                            ApplyRules(propertyName);
                        }
                    }));

            selectableDataSubscription.Add(
                data.ItemChanged
                    .Where(x => !ReferenceEquals(allItem, x.Sender))
                    .Subscribe(x =>
                    {
                        if (!x.Sender.IsSelected && allItem.IsSelected)
                        {
                            ignoreSelectionChanged = true;
                            allItem.IsSelected = false;
                            ignoreSelectionChanged = false;
                        }
                        else if (!allItem.IsSelected &&
                            x.Sender.IsSelected &&
                            data.Where(p => !ReferenceEquals(allItem, p)).All(p => p.IsSelected))
                        {
                            ignoreSelectionChanged = true;
                            allItem.IsSelected = true;
                            ignoreSelectionChanged = false;
                        }

                        ApplyRules(propertyName);
                    }));

            if (isAllSelected)
            {
                allItem.IsSelected = true;
            }

            return data;
        }

        private void Upload()
        {
            BusyStatus = HudUploadToStoreBusyStatus.Submitting;

            StartAsyncOperation(() =>
            {
                var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

                var license = licenseService.GetHudStoreLicenseInfo(false);

                if (license == null)
                {
                    throw new DHBusinessException(new NonLocalizableString("Couldn't find license to upload HUD to the store."));
                }

                // update layout name in xml
                hudLayout.Name = Name;

                var serializedLayout = SerializationHelper.SerializeObject(hudLayout);

                var uploadInfo = new HudStoreUploadInfo
                {
                    Name = Name,
                    Description = Description,
                    GameVariants = GameVariants.Where(x => x.IsSelected && !x.IsAll).Select(x => x.Item).ToArray(),
                    GameTypes = GameTypes.Where(x => x.IsSelected && !x.IsAll).Select(x => x.Item).ToArray(),
                    TableTypes = TableTypes.Where(x => x.IsSelected && !x.IsAll).Select(x => x.Item).ToArray(),
                    Cost = Cost,
                    Images = Images.Select(x => new HudStoreUploadImageInfo
                    {
                        Caption = x.Caption,
                        Path = x.Path
                    }).ToArray(),
                    Serial = license.Serial,
                    HudLayout = serializedLayout
                };

                Model.Upload(uploadInfo);
            },
            ex =>
            {
                if (ex != null)
                {
                    LogProvider.Log.Error(this, "Failed to upload HUD on the HUD store.", ex);

                    IsSubmitButtonVisible = false;
                    IsResetButtonVisible = false;
                    IsRetryButtonVisible = true;
                    IsBackButtonVisible = true;
                    IsCancelButtonVisible = true;
                    IsCloseButtonVisible = false;

                    Message = LocalizableString.ToString("Common_HudUploadToStoreView_UploadingFailed", ex.Message);

                    return;
                }

                IsSubmitButtonVisible = false;
                IsResetButtonVisible = false;
                IsRetryButtonVisible = false;
                IsBackButtonVisible = false;
                IsCancelButtonVisible = false;
                IsCloseButtonVisible = true;

                Message = CommonResourceManager.Instance.GetResourceString("Common_HudUploadToStoreView_UploadingSucceed");
            });
        }

        /// <summary>
        /// Resets form
        /// </summary>
        private void Reset()
        {
            Name = string.Empty;
            Description = string.Empty;
            Cost = 0;
            GameVariants.ForEach(x => x.IsSelected = false);
            GameTypes.ForEach(x => x.IsSelected = false);
            TableTypes.ForEach(x => x.IsSelected = false);
            Images.Clear();
        }

        private void AddImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = CommonResourceManager.Instance.GetResourceString("Common_HudUploadToStoreView_SelectImageDialogTitle"),
                Filter = "Image files (*.jpg, *.jpeg, *.gif, *.png, *.bmp) | *.jpg; *.jpeg; *.gif; *.png; *.bmp"
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (!TryGetImage(openFileDialog.FileName, out BitmapImage bitmapImage))
            {
                LogProvider.Log.Error($"Couldn't add {openFileDialog.FileName} as an image to the data for uploading to HUD store.");
                return;
            }

            var image = new HudUploadToStoreImage
            {               
                ImageSize = new Size(bitmapImage.Width, bitmapImage.Height),
                Path = openFileDialog.FileName
            };

            images.Add(image);
        }

        private static bool TryGetImage(string path, out BitmapImage image)
        {
            try
            {
                image = new BitmapImage(new Uri(path));
                return true;
            }
            catch
            {
                image = null;
                return false;
            }
        }

        private void RemoveImage()
        {
            var imagesToRemove = images.Where(x => x.IsSelected).ToArray();
            imagesToRemove.ForEach(x => images.Remove(x));
        }

        #endregion
    }
}