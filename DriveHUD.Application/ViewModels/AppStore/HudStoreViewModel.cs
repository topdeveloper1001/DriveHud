//-----------------------------------------------------------------------
// <copyright file="HudStoreViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Model.AppStore;
using Model.AppStore.HudStore;
using Model.AppStore.HudStore.Model;
using Model.AppStore.HudStore.ServiceData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class HudStoreViewModel : AppStoreBaseViewModel<IHudStoreModel>, IHudStoreViewModel
    {
        public override void Initialize()
        {
            base.Initialize();

            items = new ReactiveList<HudStoreItemViewModel>();

            filter = new ReadOnlyObservableCollection<HudStoreFilter>(
                new ObservableCollection<HudStoreFilter>(Enum.GetValues(typeof(HudStoreFilter)).OfType<HudStoreFilter>()));

            sorting = new ReadOnlyObservableCollection<HudStoreSorting>(
                new ObservableCollection<HudStoreSorting>(Enum.GetValues(typeof(HudStoreSorting)).OfType<HudStoreSorting>()));

            GridColumns = 4;
            GridRows = 1;

            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            var license = licenseService.GetHudStoreLicenseInfo(true);

            var loadInfo = new HudStoreGetHudsRequest
            {
                Serial = license?.Serial
            };

            Model.ObservableForProperty(x => x.SelectedFilter).Select(x => true)
                .Merge(Model.ObservableForProperty(x => x.SelectedSorting).Select(x => true))
                .Subscribe(x =>
                {
                    StartAsyncOperation(() => Model.Refresh(1, ProductsPerPage), ex => OnUpdated());
                });

            InitializeModelAsync(() =>
            {
                Model.Load(loadInfo);
            });
        }

        public override void Refresh(int pageNumber)
        {
            StartAsyncOperation(() => base.Refresh(pageNumber), ex =>
            {
                Model.Refresh();

                Items.Clear();

                Model.Items.Select(x => new HudStoreItemViewModel(x)).ForEach(x => Items.Add(x));

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                var importedLayoutsIds = new HashSet<int>(hudLayoutsService.HudLayoutMappings.Mappings
                    .Where(x => x.LayoutId != 0)
                    .Select(x => x.LayoutId)
                    .Distinct());

                // Mark layouts as imported if layout with same id is in the list of huds 
                Items.ForEach(item => item.IsImported = importedLayoutsIds.Contains(item.Item.LayoutId));
            });
        }
    
        #region Properties

        private ReadOnlyObservableCollection<HudStoreFilter> filter;

        public ReadOnlyObservableCollection<HudStoreFilter> Filter
        {
            get
            {
                return filter;
            }
        }

        private ReadOnlyObservableCollection<HudStoreSorting> sorting;

        public ReadOnlyObservableCollection<HudStoreSorting> Sorting
        {
            get
            {
                return sorting;
            }
        }

        private ReactiveList<HudStoreItemViewModel> items;

        public ReactiveList<HudStoreItemViewModel> Items
        {
            get
            {
                return items;
            }
        }

        private bool isPopupOpened;

        public bool IsPopupOpened
        {
            get
            {
                return isPopupOpened;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isPopupOpened, value);
            }
        }

        private HudStoreItemViewModel popupItem;

        public HudStoreItemViewModel PopupItem
        {
            get
            {
                return popupItem;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref popupItem, value);
            }
        }

        public HudStoreImageItem PopupImage
        {
            get
            {
                return PopupItem != null && PopupItem.Images != null &&
                    PopupImageIndex < PopupItem.Images.Count ? PopupItem.Images[PopupImageIndex].Item : null;
            }
        }

        private int popupImageIndex;

        public int PopupImageIndex
        {
            get
            {
                return popupImageIndex;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref popupImageIndex, value);
                this.RaisePropertyChanged(nameof(IsPreviousButtonVisible));
                this.RaisePropertyChanged(nameof(IsNextButtonVisible));
                this.RaisePropertyChanged(nameof(PopupImage));
            }
        }

        public bool IsPreviousButtonVisible
        {
            get
            {
                return popupImageIndex > 0;
            }
        }

        public bool IsNextButtonVisible
        {
            get
            {
                return PopupItem != null && PopupItem.Images != null &&
                    popupImageIndex < PopupItem.Images.Count - 1;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand InstallCommand { get; private set; }

        public ReactiveCommand ExpandImageCommand { get; private set; }

        public ReactiveCommand ClosePopupCommand { get; private set; }

        public ReactiveCommand PreviousImageCommand { get; private set; }

        public ReactiveCommand NextImageCommand { get; private set; }

        #endregion

        protected override void InitializeCommands()
        {
            InstallCommand = ReactiveCommand.Create<HudStoreItemViewModel>(x => Install(x));

            ExpandImageCommand = ReactiveCommand.Create<HudStoreItemViewModel>(x =>
            {
                PopupItem = x;
                PopupImageIndex = 0;
                IsPopupOpened = true;
            });

            ClosePopupCommand = ReactiveCommand.Create<HudStoreItemViewModel>(x =>
            {
                IsPopupOpened = false;
            });

            PreviousImageCommand = ReactiveCommand.Create(() =>
            {
                if (IsPreviousButtonVisible)
                {
                    PopupImageIndex--;
                }
            });

            NextImageCommand = ReactiveCommand.Create(() =>
            {
                if (IsNextButtonVisible)
                {
                    PopupImageIndex++;
                }
            });
        }

        private void Install(HudStoreItemViewModel hudStoreItem)
        {
            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            var license = licenseService.GetHudStoreLicenseInfo(true);

            Stream layoutStream = null;

            StartAsyncOperation(() => layoutStream = Model.DownloadLayout(hudStoreItem.Item.LayoutId, license?.Serial), (Exception ex) =>
            {
                if (ex != null)
                {
                    LogProvider.Log.Error(this, "Failed to install HUD from the HUD store.", ex);

                    RaiseNotification(LocalizableString.ToString("Common_HudStoreViewModel_InstallingFailedTitle"),
                        LocalizableString.ToString("Common_HudStoreViewModel_InstallingFailedContent", ex.Message));

                    return;
                }

                if (layoutStream == null)
                {
                    LogProvider.Log.Error(this, "Failed to install HUD from the HUD store. Layout was empty.", ex);

                    RaiseNotification(LocalizableString.ToString("Common_HudStoreViewModel_InstallingFailedTitle"),
                        LocalizableString.ToString("Common_HudStoreViewModel_InstallingFailedContent"));

                    return;
                }

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                var importedLayout = hudLayoutsService.Import(layoutStream, hudStoreItem.Item.LayoutId);

                if (importedLayout != null)
                {
                    LogProvider.Log.Info($"Successfully installed HUD: {hudStoreItem.Item.Name} [{hudStoreItem.Item.LayoutId}]");

                    RaiseNotification(LocalizableString.ToString("Common_HudStoreViewModel_InstallingSucceededTitle"),
                       LocalizableString.ToString("Common_HudStoreViewModel_InstallingSucceededContent"));

                    hudStoreItem.IsImported = true;

                    return;
                }

                LogProvider.Log.Error(this, "Failed to install HUD from the HUD store. Internal error.", ex);

                RaiseNotification(LocalizableString.ToString("Common_HudStoreViewModel_InstallingFailedTitle"),
                       LocalizableString.ToString("Common_HudStoreViewModel_InstallingFailedInternalErrorContent"));
            });
        }

        private void RaiseNotification(string title, string text)
        {
            var notification = new PopupBaseNotification()
            {
                Title = title,
                ConfirmButtonCaption = LocalizableString.ToString("Common_HudStoreViewModel_OK"),
                CancelButtonCaption = null,
                Content = text,
            };

            NotificationRequest.Raise(notification);
        }
    }
}