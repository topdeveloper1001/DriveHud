//-----------------------------------------------------------------------
// <copyright file="PKCatcherViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.PKCatcher.Events;
using DriveHUD.PKCatcher.Licensing;
using DriveHUD.PKCatcher.Settings;
using DriveHUD.PKCatcher.ViewModels;
using DriveHUD.PKCatcher.Views;
using Microsoft.Practices.ServiceLocation;
using Model;
using Prism.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DriveHUD.PKCatcher
{
    public class PKCatcherViewModel : WindowViewModelBase<PKCatcherViewModel>, IPKCatcherViewModel
    {
        private readonly IEventAggregator eventAggregator;

        private readonly SubscriptionToken raisePopupSubscriptionToken;

        private readonly IPKSettingsService settingsService;

        public PKCatcherViewModel()
        {
            settingsService = ServiceLocator.Current.GetInstance<IPKSettingsService>();
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            raisePopupSubscriptionToken = eventAggregator.GetEvent<RaisePopupEvent>().Subscribe(RaisePopup, false);
        }

        public override void Configure(object viewModelInfo)
        {
            UpgradeCommand = ReactiveCommand.Create(() => Upgrade());
            ManualCommand = ReactiveCommand.Create(() => BrowserHelper.OpenLinkInBrowser(CommonResourceManager.Instance.GetResourceString("PKC_MainView_ManualLink")));
            TutorialCommand = ReactiveCommand.Create(() => BrowserHelper.OpenLinkInBrowser(CommonResourceManager.Instance.GetResourceString("PKC_MainView_TutorialLink")));
            PurchaseCommand = ReactiveCommand.Create(() => BrowserHelper.OpenLinkInBrowser(CommonResourceManager.Instance.GetResourceString("PKC_BuyLink")));
            GenerateLogCommand = ReactiveCommand.Create(() => GenerateNetLog());

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
        
        private void GenerateNetLog()
        {
            StartAsyncOperation(() =>
            {
                var processStartInfo = new ProcessStartInfo("netstat", "-ano")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(processStartInfo);
                var result = process.StandardOutput.ReadToEnd();

                process.WaitForExit(5000);

                File.WriteAllText("Logs\\pk-netstat.log", result);
            }, ex =>
            {
                string message;
                string title;

                if (ex != null)
                {
                    LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Failed to create PK netstat log.", ex);
                    message = CommonResourceManager.Instance.GetResourceString("PKC_GenerateLogView_FailMessage");
                    title = CommonResourceManager.Instance.GetResourceString("PKC_GenerateLogView_FailTitle");
                }
                else
                {
                    message = CommonResourceManager.Instance.GetResourceString("PKC_GenerateLogView_SuccessMessage");
                    title = CommonResourceManager.Instance.GetResourceString("PKC_GenerateLogView_SuccessTitle");
                }

                var generateLogViewModel = new GenerateLogViewModel
                {
                    Message = message
                };

                var popupEventArgs = new RaisePopupEventArgs()
                {
                    Title = title,
                    Content = new GenerateLogView(generateLogViewModel)
                };

                RaisePopup(popupEventArgs);
            });
        }

        #region Properties        

        private Assembly Assembly
        {
            get
            {
                return Assembly.GetAssembly(typeof(PKCatcherViewModel));
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
                    return CommonResourceManager.Instance.GetResourceString("PKC_LicenseType_None");
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
                SaveSettings();
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

        public ReactiveCommand GenerateLogCommand { get; private set; }

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
                Title = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_Title"),
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