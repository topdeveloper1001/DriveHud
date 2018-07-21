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
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Common.Wpf.Validation;
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
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudUploadToStoreViewModel : WindowViewModel<HudUploadToStoreViewModel,
        IHudStoreUploadModel>, IHudUploadToStoreViewModel, INotifyDataErrorInfo
    {
        private const int MaxImages = 8;

        private CompositeDisposable selectableDataSubscription;

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
                new NonLocalizableString("Common_HudUploadToStoreView_AtLeastOneImageMustBeSpecified"),
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
            BusyStatus = HudUploadToStoreBusyStatus.Loading;

            IsSubmitButtonVisible = true;
            IsResetButtonVisible = true;
            IsCancelButtonVisible = true;

            InitializeModelAsync(() => Model.Load());
            Initialize();

            for (var i = 0; i < 1; i++)
            {
                images.Add(new HudUploadToStoreImage
                {
                    Caption = "Caption 1",
                    Path = @"d:\hud-screenshot-1.png"
                });
            }

            images.Add(new HudUploadToStoreImage
            {
                Caption = "Caption 2",
                Path = @"d:\hud-screenshot-2.png"
            });

            Name = "Test layout for testing only";
            Description = "Test layout for testing only";
        }

        #region Properties

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

        #endregion

        #region Command

        public ReactiveCommand SubmitCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand ResetCommand { get; private set; }

        public ReactiveCommand AddImageCommand { get; private set; }

        public ReactiveCommand RemoveImageCommand { get; private set; }

        public ReactiveCommand BackCommand { get; private set; }

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

            BackCommand = ReactiveCommand.Create(() => Message = string.Empty);
        }

        protected override void ModelInitialized()
        {
            base.ModelInitialized();
            InitializeSelectableData();
        }

        private void InitializeSelectableData()
        {
            selectableDataSubscription?.Dispose();
            selectableDataSubscription = new CompositeDisposable();

            var allItemName = CommonResourceManager.Instance.GetResourceString("Common_HudUploadToStoreView_All");

            GameVariants = InitializeSelectableData(Model.GameVariants, () => new GameVariant { Name = allItemName }, nameof(GameVariants), true);
            GameTypes = InitializeSelectableData(Model.GameTypes, () => new GameType { Name = allItemName }, nameof(GameTypes), true);
            TableTypes = InitializeSelectableData(Model.TableTypes, () => new TableType { Name = allItemName }, nameof(TableTypes), true);
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

        // Uploads data to server
        private void Upload()
        {
            BusyStatus = HudUploadToStoreBusyStatus.Submitting;

            StartAsyncOperation(() =>
            {
                var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

                // trial can't be used to upload data
                if (licenseService.IsTrial)
                {
                    throw new DHBusinessException(new NonLocalizableString("Trial license isn't eligible to upload HUD to the store."));
                }

                var registerdLicenses = licenseService.LicenseInfos.Where(x => x.IsRegistered).ToArray();

                var license = registerdLicenses.FirstOrDefault(x => x.LicenseType == LicenseType.Combo) ??
                    registerdLicenses.FirstOrDefault(x => x.LicenseType == LicenseType.Holdem) ??
                    registerdLicenses.FirstOrDefault(x => x.LicenseType == LicenseType.Omaha);

                if (license == null)
                {
                    throw new DHBusinessException(new NonLocalizableString("Couldn't find license to upload HUD to the store."));
                }

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
                    Serial = license.Serial
                };

                Model.Upload(uploadInfo);
            },
            ex =>
            {
                if (true)
                // if (ex != null)
                {
                    if (ex != null)
                        LogProvider.Log.Error(this, "Fail to upload HUD on the HUD store.", ex);


                    IsSubmitButtonVisible = false;
                    IsResetButtonVisible = false;
                    IsRetryButtonVisible = true;
                    IsBackButtonVisible = true;
                    IsCancelButtonVisible = true;
                    IsCloseButtonVisible = false;

                    Message = LocalizableString.ToString("Common_HudUploadToStoreView_UploadingFailed", ex != null ? ex.Message : "No errors.");

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
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            if (!IsImage(openFileDialog.FileName))
            {
                return;
            }

            var image = new HudUploadToStoreImage
            {
                Path = openFileDialog.FileName
            };

            images.Add(image);
        }

        private static bool IsImage(string path)
        {
            try
            {
                var image = new BitmapImage(new Uri(path));
                return true;
            }
            catch
            {
                LogProvider.Log.Error($"Couldn't add {path} as an image to the data for uploading to HUD store.");
            }

            return false;
        }

        private void RemoveImage()
        {
            var imagesToRemove = images.Where(x => x.IsSelected).ToArray();
            imagesToRemove.ForEach(x => images.Remove(x));
        }

        #endregion
    }
}