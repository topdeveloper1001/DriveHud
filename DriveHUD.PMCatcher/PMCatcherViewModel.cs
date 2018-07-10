//-----------------------------------------------------------------------
// <copyright file="PMCatcherViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.PMCatcher.Events;
using DriveHUD.PMCatcher.Licensing;
using DriveHUD.PMCatcher.Settings;
using DriveHUD.PMCatcher.ViewModels;
using DriveHUD.PMCatcher.Views;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DriveHUD.PMCatcher
{
    public class PMCatcherViewModel : LightWindowViewModel, IPMCatcherViewModel
    {
        private readonly IEventAggregator eventAggregator;

        private readonly SubscriptionToken raisePopupSubscriptionToken;

        private readonly IPMSettingsService settingsService;

        public PMCatcherViewModel()
        {
            settingsService = ServiceLocator.Current.GetInstance<IPMSettingsService>();
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            raisePopupSubscriptionToken = eventAggregator.GetEvent<RaisePopupEvent>().Subscribe(RaisePopup, false);
        }

        public override void Configure(object viewModelInfo)
        {
            UpgradeCommand = ReactiveCommand.Create(() => Upgrade());
            ManualCommand = ReactiveCommand.Create(() => BrowserHelper.OpenLinkInBrowser(CommonResourceManager.Instance.GetResourceString("PMC_MainView_ManualLink")));
            TutorialCommand = ReactiveCommand.Create(() => BrowserHelper.OpenLinkInBrowser(CommonResourceManager.Instance.GetResourceString("PMC_MainView_TutorialLink")));
            PurchaseCommand = ReactiveCommand.Create(() => BrowserHelper.OpenLinkInBrowser(CommonResourceManager.Instance.GetResourceString("PMC_BuyLink")));

            OnInitialized();

            var settingsModel = settingsService.GetSettings();

            Enabled = settingsModel.Enabled;
            AdvancedLogging = settingsModel.IsAdvancedLoggingEnabled;

            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

            if (licenseService.IsTrial ||
                     (licenseService.IsRegistered && licenseService.IsExpiringSoon) ||
                     !licenseService.IsRegistered)
            {
                RaiseRegistrationPopup(false);
                return;
            }

            Initialize(licenseService);
        }

        #region Properties        

        private Assembly Assembly
        {
            get
            {
                return Assembly.GetAssembly(typeof(PMCatcherViewModel));
            }
        }

        public Version Version
        {
            get
            {
                return Assembly.GetName().Version;
            }
        }

        public DateTime BuildDate
        {
            get
            {
                try
                {
                    var fileInfo = new FileInfo(Assembly.Location);
                    return fileInfo.LastWriteTimeUtc;
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }

        private string popupTitle;

        public string PopupTitle
        {
            get
            {
                return popupTitle;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref popupTitle, value);
            }
        }

        private object popupContent;

        public object PopupContent
        {
            get
            {
                return popupContent;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref popupContent, value);
            }
        }

        private bool popupIsOpen;

        public bool PopupIsOpen
        {
            get
            {
                return popupIsOpen;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref popupIsOpen, value);
            }

        }

        public string LicenseType
        {
            get
            {
                var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

                IEnumerable<string> licenseStrings;

                if (licenseService.LicenseInfos.Any(x => x.IsRegistered && !x.IsTrial))
                {
                    licenseStrings = licenseService.LicenseInfos.Where(x => x.IsRegistered && !x.IsTrial).Select(x => x.License.Edition);
                }
                else
                {
                    licenseStrings = licenseService.LicenseInfos.Where(x => x.IsRegistered).Select(x => x.License.Edition);
                }

                if (licenseStrings == null || licenseStrings.Count() == 0)
                {
                    return CommonResourceManager.Instance.GetResourceString("PMC_LicenseType_None");
                }

                return string.Join(" & ", licenseStrings);
            }
        }

        private bool isTrial;

        public bool IsTrial
        {
            get
            {
                return isTrial;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isTrial, value);
            }
        }

        private bool isUpgradable;

        public bool IsUpgradable
        {
            get
            {
                return isUpgradable;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isUpgradable, value);
            }
        }

        private bool enabled;

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref enabled, value);
            }
        }

        private bool advancedLogging;

        public bool AdvancedLogging
        {
            get
            {
                return advancedLogging;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref advancedLogging, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand UpgradeCommand { get; private set; }

        public ReactiveCommand ManualCommand { get; private set; }

        public ReactiveCommand TutorialCommand { get; private set; }

        public ReactiveCommand PurchaseCommand { get; private set; }

        #endregion

        #region IDisposable implementation

        protected override void Disposing()
        {
            eventAggregator.GetEvent<RaisePopupEvent>().Unsubscribe(raisePopupSubscriptionToken);
            base.Disposing();
        }

        #endregion

        private void Initialize(ILicenseService licenseService)
        {
            IsUpgradable = licenseService.IsUpgradable;
            IsTrial = licenseService.IsTrial;
        }

        private void SaveSettings()
        {
            var settingsModel = settingsService.GetSettings();

            settingsModel.Enabled = Enabled;
            settingsModel.IsAdvancedLoggingEnabled = AdvancedLogging;

            settingsService.SaveSettings(settingsModel);
        }

        private void Upgrade()
        {
            RaiseRegistrationPopup(true);
        }

        private void RaiseRegistrationPopup(bool showRegister)
        {
            var registrationViewModel = new RegistrationViewModel(showRegister)
            {
                Callback = () =>
                {
                    var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

                    Initialize(licenseService);

                    this.RaisePropertyChanged(nameof(LicenseType));
                }
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = CommonResourceManager.Instance.GetResourceString("PMC_RegistrationView_Title"),
                Content = new RegistrationView(registrationViewModel)
            };

            RaisePopup(popupEventArgs);
        }

        private void RaisePopup(RaisePopupEventArgs e)
        {
            var containerView = e.Content as IPopupContainerView;

            if (containerView != null && containerView.ViewModel != null)
            {
                containerView.ViewModel.FinishInteraction = () => ClosePopup();
            }

            PopupTitle = e.Title;
            PopupContent = containerView;
            PopupIsOpen = true;
        }

        private void ClosePopup()
        {
            PopupIsOpen = false;
            PopupContent = null;
            PopupTitle = null;
        }
    }
}