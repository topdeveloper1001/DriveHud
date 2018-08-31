//-----------------------------------------------------------------------
// <copyright file="RegistrationViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.PKCatcher.Licensing;
using Microsoft.Practices.ServiceLocation;
using Model;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;

namespace DriveHUD.PKCatcher.ViewModels
{
    public class RegistrationViewModel : ReactiveObject, IPopupInteractionAware
    {
        private ILicenseService licenseService;

        private RegistrationState state;

        private string serverUrl = string.Empty;
        private DeployLXLicensingServer server;

        public RegistrationViewModel(bool showRegister)
        {
            server = new DeployLXLicensingServer
            {
                CookieContainer = new CookieContainer()
            };

            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

            InitializeCommands();

            if (showRegister)
            {
                InitializeRegister(true);
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            licenses = new ObservableCollection<LicenseInfoViewModel>(licenseService.LicenseInfos.Where(x => x.IsRegistered && !x.IsTrial).Select(x => new LicenseInfoViewModel(x)));

            if (licenseService.IsRegistered && licenseService.IsExpiringSoon)
            {
                InitializeExpiringSoon();
                return;
            }

            if (!licenseService.IsRegistered)
            {
                if (licenseService.IsExpired)
                {
                    InitializeExpiringSoon();
                }
                else if (licenseService.IsTrialExpired)
                {
                    InitializeTrialExpired();
                }
                else
                {
                    InitializeGreeting();
                }

                return;
            }

            if (licenseService.IsTrial)
            {
                InitializeTrialActive();
                return;
            }
        }

        private void InitializeCommands()
        {
            var canSend = this.WhenAny(x => x.Email, x => Utils.IsValidEmail(x.Value));

            SendCommand = ReactiveCommand.Create(() => Send(), canSend);
            CancelCommand = ReactiveCommand.Create(() => Cancel());
            TrialCommand = ReactiveCommand.Create(() => InitializeTrial());
            BackCommand = ReactiveCommand.Create(() => Back());
            OKCommand = ReactiveCommand.Create(() => Cancel());
            RegisterCommand = ReactiveCommand.Create(() => InitializeRegister(false));
            ActivateCommand = ReactiveCommand.Create(() => Register());
            BuyCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("PKC_BuyLink"));
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Could not open buy link", ex);
                }
            });

            RenewCommand = ReactiveCommand.Create(() =>
            {
                try
                {
                    var serialToRenew = licenseService.LicenseInfos
                        .Where(l => !string.IsNullOrEmpty(l.Serial) && !l.IsTrial)
                        .OrderBy(l => l.ExpiryDate).FirstOrDefault()?
                        .Serial;

                    if (!string.IsNullOrWhiteSpace(serialToRenew))
                    {
                        Process.Start(BrowserHelper.GetDefaultBrowserPath(), string.Format(CommonResourceManager.Instance.GetResourceString("SystemSettings_RenewLicenseLink"), serialToRenew));
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Could not open renew link", ex);
                }
            });

            ResetLicensesCommand = ReactiveCommand.Create(()=>
            {
                try
                {
                    LogProvider.Log.Info(CustomModulesNames.PKCatcher, "Resetting licenses");

                    licenseService.ResetLicenses();

                    LogProvider.Log.Info(CustomModulesNames.PKCatcher, "Licenses has been reset");

                    InitializeMessage("PKC_RegistrationView_ResetLicensesSuccess");
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Could not reset licenses", ex);
                    InitializeMessage("PKC_RegistrationView_ResetLicensesFailed");
                }
            });
        }

        /// <summary>
        /// Configure view to show greeting message
        /// </summary>
        private void InitializeGreeting()
        {
            state = RegistrationState.Greeting;

            TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_GreetingText");

            IsSerialVisible = false;
            IsEmailVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = true;
            IsRegisterButtonVisible = true;
            IsBuyButtonVisible = true;
            IsRenewButtonVisible = false;
            IsBackButtonVisible = false;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
            IsResetLicensesButtonVisible = false;
        }

        /// <summary>
        /// Configure view to show greeting message
        /// </summary>
        private void InitializeTrialActive()
        {
            state = RegistrationState.TrialActive;

            TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_GreetingText");

            IsSerialVisible = false;
            IsEmailVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = true;
            IsBuyButtonVisible = true;
            IsRenewButtonVisible = false;
            IsOKButtonVisible = true;
            IsBackButtonVisible = false;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = true;
            IsLicenseDaysLeftVisible = false;
            IsResetLicensesButtonVisible = true;
        }

        /// <summary>
        /// Configure view to show trial expired dialog
        /// </summary>
        private void InitializeTrialExpired()
        {
            state = RegistrationState.TrialExpired;

            TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_TrialExpiredText");

            IsSerialVisible = false;
            IsEmailVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = true;
            IsBuyButtonVisible = true;
            IsRenewButtonVisible = false;
            IsOKButtonVisible = false;
            IsBackButtonVisible = false;
            IsCancelButtonVisible = true;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = true;
            IsLicenseDaysLeftVisible = false;
            IsResetLicensesButtonVisible = true;
        }

        /// <summary>
        /// Configure view to show trial form
        /// </summary>
        private void InitializeTrial()
        {
            state = RegistrationState.Trial;

            TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_TrialRegisterText");

            IsSerialVisible = false;
            IsEmailVisible = true;

            IsSendButtonVisible = true;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsOKButtonVisible = false;
            IsBackButtonVisible = true;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = false;
            IsResetLicensesButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        /// <summary>
        /// Configure view to show register form
        /// </summary>
        private void InitializeRegister(bool calledFromMain)
        {
            state = RegistrationState.Registration;

            TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_RegisterText");

            IsSerialVisible = true;
            IsEmailVisible = true;

            IsSendButtonVisible = true;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsOKButtonVisible = false;
            IsBackButtonVisible = !calledFromMain;
            IsCancelButtonVisible = calledFromMain;
            IsActivateButtonVisible = false;
            IsResetLicensesButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        private void InitializeMessage(string messageKey, bool isOk = false, params object[] args)
        {
            state = RegistrationState.Message;

            TextMessage = string.Format(CommonResourceManager.Instance.GetResourceString(messageKey), args);

            IsSerialVisible = false;
            IsEmailVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsBackButtonVisible = !isOk;
            IsOKButtonVisible = isOk;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = false;
            IsResetLicensesButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        private void InitializeActivation()
        {
            state = RegistrationState.Activation;

            TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_CouldNotBeActivated");

            IsSerialVisible = false;
            IsEmailVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsBackButtonVisible = true;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = true;
            IsResetLicensesButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        /// <summary>
        /// Configure view to show expiring soon message
        /// </summary>
        private void InitializeExpiringSoon()
        {
            state = RegistrationState.ExpiringSoon;

            if (licenseService.IsExpired)
            {
                var splitter = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_ExpiredSerialSplitter");

                var expiredLicenses = string.Join(splitter, licenseService.LicenseInfos
                    .Where(x => !x.IsTrial && x.IsExpired && !string.IsNullOrEmpty(x.Serial))
                    .Select(x => x.Serial));

                TextMessage = string.Format(CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_ExpiredText"), expiredLicenses);
            }
            else
            {
                TextMessage = CommonResourceManager.Instance.GetResourceString("PKC_RegistrationView_ExpiringSoonText");
            }

            IsSerialVisible = false;
            IsEmailVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = true;
            IsBuyButtonVisible = true;
            IsRenewButtonVisible = true;
            IsBackButtonVisible = false;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = true;
            IsActivateButtonVisible = false;
            IsResetLicensesButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = true;
        }

        #region Properties

        private string serial;

        public string Serial
        {
            get
            {
                return serial;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref serial, value);
            }
        }

        private bool isSerialVisible;

        public bool IsSerialVisible
        {
            get
            {
                return isSerialVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isSerialVisible, value);
            }
        }

        private bool isEmailVisible;

        public bool IsEmailVisible
        {
            get
            {
                return isEmailVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isEmailVisible, value);
            }
        }

        private string email;

        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref email, value);
            }
        }

        private string textMessage;

        public string TextMessage
        {
            get
            {
                return textMessage;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref textMessage, value);
            }
        }

        #region Buttons visibility

        private bool isSendButtonVisible;

        public bool IsSendButtonVisible
        {
            get
            {
                return isSendButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isSendButtonVisible, value);
            }
        }

        private bool isTrialButtonVisible;

        public bool IsTrialButtonVisible
        {
            get
            {
                return isTrialButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isTrialButtonVisible, value);
            }
        }

        private bool isBuyButtonVisible;

        public bool IsBuyButtonVisible
        {
            get
            {
                return isBuyButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isBuyButtonVisible, value);
            }
        }

        private bool isRegisterButtonVisible;

        public bool IsRegisterButtonVisible
        {
            get
            {
                return isRegisterButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isRegisterButtonVisible, value);
            }
        }

        private bool isRenewButtonVisible;

        public bool IsRenewButtonVisible
        {
            get
            {
                return isRenewButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isRenewButtonVisible, value);
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

        private bool isTrialProgressBarVisible;

        public bool IsTrialProgressBarVisible
        {
            get
            {
                return isTrialProgressBarVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isTrialProgressBarVisible, value);
            }
        }

        private bool isOKButtonVisible;

        public bool IsOKButtonVisible
        {
            get
            {
                return isOKButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isOKButtonVisible, value);
            }
        }

        private bool isActivateButtonVisible;

        public bool IsActivateButtonVisible
        {
            get
            {
                return isActivateButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isActivateButtonVisible, value);
            }
        }

        public int TrialDaysLeft
        {
            get
            {
                var licenseInfo = licenseService.LicenseInfos.FirstOrDefault(x => x.IsTrial);
                return licenseInfo != null ? (int)licenseInfo.TimeRemaining.TotalDays : 0;
            }
        }

        private bool isLicenseDaysLeftVisible;

        public bool IsLicenseDaysLeftVisible
        {
            get
            {
                return isLicenseDaysLeftVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isLicenseDaysLeftVisible, value);
            }
        }

        private bool isResetLicensesButtonVisible;

        public bool IsResetLicensesButtonVisible
        {
            get
            {
                return isResetLicensesButtonVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isResetLicensesButtonVisible, value);
            }
        }

        private ObservableCollection<LicenseInfoViewModel> licenses;

        public ObservableCollection<LicenseInfoViewModel> Licenses
        {
            get
            {
                return licenses;
            }
        }

        private Action callback;

        public Action Callback
        {
            get
            {
                return callback;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref callback, value);
            }
        }

        #endregion      

        #region Implementation of IPopupInteractionAware

        private Action finishInteraction;

        public Action FinishInteraction
        {
            get
            {
                return finishInteraction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref finishInteraction, value);
            }
        }

        #endregion

        #endregion

        #region Commands        

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand SendCommand { get; private set; }

        public ReactiveCommand TrialCommand { get; private set; }

        public ReactiveCommand BuyCommand { get; private set; }

        public ReactiveCommand RegisterCommand { get; private set; }

        public ReactiveCommand RenewCommand { get; private set; }

        public ReactiveCommand OKCommand { get; private set; }

        public ReactiveCommand ActivateCommand { get; private set; }

        public ReactiveCommand BackCommand { get; private set; }

        public ReactiveCommand ResetLicensesCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void Cancel()
        {
            if (!licenseService.IsRegistered)
            {
                InitializeGreeting();
                return;
            }

            Callback?.Invoke();
            FinishInteraction?.Invoke();
        }

        private void Send()
        {
            switch (state)
            {
                case RegistrationState.Trial:
                    RegisterTrial();
                    break;
                case RegistrationState.Registration:
                    Register();
                    break;
                default:
                    Cancel();
                    break;
            }
        }

        private void RegisterTrial()
        {
            if (!string.IsNullOrWhiteSpace(serverUrl))
            {
                server.Url = serverUrl;
            }

            var trialSerial = string.Empty;

            try
            {
                var encryptedEmail = licenseService.EncryptEmail(Email);

                try
                {
                    trialSerial = server.RegisterTrial(encryptedEmail);
                }
                catch (WebException ex)
                {
                    var response = (HttpWebResponse)ex.Response;

                    if (response == null)
                    {
                        var errorMessage = "License service isn't responding";

                        LogProvider.Log.Error(CustomModulesNames.PKCatcher, errorMessage, ex);
                        throw new LicenseNoConnectionException(errorMessage, ex);
                    }

                    if (response != null && response.StatusCode == HttpStatusCode.Found)
                    {
                        serverUrl = new Uri(new Uri(server.Url), response.Headers["Location"]).AbsoluteUri;
                        server.Url = serverUrl;
                        trialSerial = server.RegisterTrial(encryptedEmail);
                    }
                }

                if (string.IsNullOrWhiteSpace(trialSerial))
                {
                    LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Couldn't register trial. Server error.");
                    InitializeMessage("PKC_RegistrationView_TrialRegisterError");
                    return;
                }
            }
            catch (LicenseNoConnectionException)
            {
                InitializeMessage("PKC_RegistrationView_NoConnection");
                return;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Couldn't register trial", e);
                InitializeMessage("PKC_RegistrationView_TrialRegisterError");
                return;
            }

            if (!Register(trialSerial, Email))
            {
                return;
            }

            Cancel();
        }

        private bool Register(string serial, string email)
        {
            try
            {
                licenseService.Register(serial, email);
            }
            catch (LicenseInvalidSerialException)
            {
                InitializeMessage("PKC_RegistrationView_InvalidSerial");
                return false;
            }
            catch (LicenseCouldNotActivateException)
            {
                InitializeActivation();
                return false;
            }
            catch (LicenseExpiredException)
            {
                var licenseType = licenseService.GetTypeFromSerial(serial);

                if (licenseType == LicenseType.PKCTrial)
                {
                    InitializeMessage("PKC_RegistrationView_TrialExpiredText");
                }
                else
                {
                    InitializeMessage("PKC_RegistrationView_ExpiredText", false, serial);
                    IsRenewButtonVisible = true;
                }

                return false;
            }
            catch
            {
                InitializeMessage("PKC_RegistrationView_InvalidSerial");
                return false;
            }

            return true;
        }

        private void Register()
        {
            if (!Register(Serial, Email))
            {
                return;
            }

            InitializeMessage("PKC_RegistrationView_Success", true);
        }

        private void Back()
        {
            Serial = string.Empty;
            Email = string.Empty;

            if (licenseService.IsTrial)
            {
                InitializeTrialActive();
                return;
            }

            if (licenseService.IsTrialExpired)
            {
                InitializeTrialExpired();
                return;
            }

            if (licenseService.IsExpiringSoon || licenseService.IsExpired)
            {
                InitializeExpiringSoon();
                return;
            }

            InitializeGreeting();
        }

        #endregion

        #region Registration state

        private enum RegistrationState
        {
            Greeting,
            Trial,
            TrialActive,
            TrialExpired,
            Registration,
            Message,
            Activation,
            ExpiringSoon
        }

        #endregion
    }
}