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

using DriveHUD.Application.Licensing;
using DriveHUD.Application.Security;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Windows.Controls;
using System.Net;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using DriveHUD.Common.Utils;
using Prism.Interactivity.InteractionRequest;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Log;
using System.Diagnostics;
using System.Collections.ObjectModel;
using DriveHUD.Importers;
using DriveHUD.Common.Wpf.Actions;
using System.Reactive.Linq;

namespace DriveHUD.Application.ViewModels.Registration
{
    public class RegistrationViewModel : ViewModelBase, INotification, IInteractionRequestAware
    {
        private ILicenseService licenseService;

        private RegistrationState state;

        private string serverUrl = string.Empty;
        private DeployLXLicensingServer server;

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public RegistrationViewModel(bool showRegister)
        {
            server = new DeployLXLicensingServer();
            server.CookieContainer = new CookieContainer();


            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

            NotificationRequest = new InteractionRequest<INotification>();

            title = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_Title");

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

            if (!licenseService.IsRegistered)
            {
                if (licenseService.IsTrialExpired)
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

            if (licenseService.IsExpiringSoon || licenseService.IsExpired)
            {
                InitializeExpiringSoon();
                return;
            }
        }

        private void InitializeCommands()
        {
            var canSend = this.WhenAny(x1 => x1.Email, x2 => x2.CaptchaText, (x1, x2) => Utils.IsValidEmail(x1.Value) && (IsCaptchaVisible && !string.IsNullOrWhiteSpace(x2.Value) || !IsCaptchaVisible));

            SendCommand = ReactiveCommand.Create(canSend);
            SendCommand.Subscribe(x => Send());

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x => Cancel());

            TrialCommand = ReactiveCommand.Create();
            TrialCommand.Subscribe(x => InitializeTrial());

            BackCommand = ReactiveCommand.Create();
            BackCommand.Subscribe(x => Back());

            OKCommand = ReactiveCommand.Create();
            OKCommand.Subscribe(x => Cancel());

            RegisterCommand = ReactiveCommand.Create();
            RegisterCommand.Subscribe(x => InitializeRegister(false));

            ActivateCommand = ReactiveCommand.Create();
            ActivateCommand.Subscribe(x => Register());

            BuyCommand = ReactiveCommand.Create();
            BuyCommand.Subscribe(x => Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("Common_BuyLink")));
        }

        private void InitializeCaptcha()
        {
            byte[] imageData = new byte[0];

            try
            {
                imageData = server.CreateSession();
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                // no response from server
                if (response == null)
                {
                    var errorMessage = "License service isn't responding";

                    LogProvider.Log.Error(this, errorMessage, ex);
                    throw new LicenseNoConnectionException(errorMessage, ex);
                }

                if (response != null && response.StatusCode == HttpStatusCode.Found)
                {
                    serverUrl = new Uri(new Uri(server.Url), response.Headers["Location"]).AbsoluteUri;
                    server.Url = serverUrl;
                    imageData = server.CreateSession();
                }
            }

            LogProvider.Log.Info($"Trying to register trial. Cookies in container: {server.CookieContainer.Count}");

            var image = new BitmapImage();

            try
            {
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = mem;
                    image.EndInit();
                }

                image.Freeze();

                CaptchaImage = image;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't get captcha image", e);
            }
        }

        /// <summary>
        /// Configure view to show greeting message
        /// </summary>
        private void InitializeGreeting()
        {
            state = RegistrationState.Greeting;

            TextMessage = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_GreetingText");

            IsSerialVisible = false;
            IsEmailVisible = false;
            IsCaptchaVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = true;
            IsRegisterButtonVisible = true;
            IsBuyButtonVisible = true;
            IsRenewButtonVisible = false;
            IsBackButtonVisible = false;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = true;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        /// <summary>
        /// Configure view to show greeting message
        /// </summary>
        private void InitializeTrialActive()
        {
            state = RegistrationState.TrialActive;

            TextMessage = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_GreetingText");

            IsSerialVisible = false;
            IsEmailVisible = false;
            IsCaptchaVisible = false;

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
        }

        /// <summary>
        /// Configure view to show trial expired dialog
        /// </summary>
        private void InitializeTrialExpired()
        {
            state = RegistrationState.TrialExpired;

            TextMessage = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_TrialExpiredText");

            IsSerialVisible = false;
            IsEmailVisible = false;
            IsCaptchaVisible = false;

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
        }

        /// <summary>
        /// Configure view to show trial form
        /// </summary>
        private void InitializeTrial()
        {
            state = RegistrationState.Trial;

            try
            {
                InitializeCaptcha();
            }
            catch (LicenseNoConnectionException)
            {
                InitializeMessage("Common_RegistrationView_NoConnection");
                return;
            }

            TextMessage = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_TrialRegisterText");

            IsSerialVisible = false;
            IsEmailVisible = true;
            IsCaptchaVisible = true;

            IsSendButtonVisible = true;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsOKButtonVisible = false;
            IsBackButtonVisible = true;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        /// <summary>
        /// Configure view to show register form
        /// </summary>
        private void InitializeRegister(bool calledFromMain)
        {
            state = RegistrationState.Registration;

            TextMessage = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_RegisterText");

            IsSerialVisible = true;
            IsEmailVisible = true;
            IsCaptchaVisible = false;

            IsSendButtonVisible = true;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsOKButtonVisible = false;
            IsBackButtonVisible = !calledFromMain;
            IsCancelButtonVisible = calledFromMain;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        private void InitializeMessage(string messageKey)
        {
            state = RegistrationState.Message;

            TextMessage = CommonResourceManager.Instance.GetResourceString(messageKey);

            IsSerialVisible = false;
            IsEmailVisible = false;
            IsCaptchaVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsBackButtonVisible = true;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = false;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        private void InitializeActivation()
        {
            state = RegistrationState.Activation;

            TextMessage = CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_CouldNotBeActivated");

            IsSerialVisible = false;
            IsEmailVisible = false;
            IsCaptchaVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = false;
            IsBuyButtonVisible = false;
            IsRenewButtonVisible = false;
            IsBackButtonVisible = true;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = false;
            IsActivateButtonVisible = true;

            IsTrialProgressBarVisible = false;
            IsLicenseDaysLeftVisible = false;
        }

        /// <summary>
        /// Configure view to show expiring soon message
        /// </summary>
        private void InitializeExpiringSoon()
        {
            state = RegistrationState.ExpiringSoon;

            TextMessage = licenseService.IsExpired ?
                            CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_ExpiringSoonText") :
                            CommonResourceManager.Instance.GetResourceString("Common_RegistrationView_ExpiringSoonText");

            IsSerialVisible = false;
            IsEmailVisible = false;
            IsCaptchaVisible = false;

            IsSendButtonVisible = false;
            IsTrialButtonVisible = false;
            IsRegisterButtonVisible = true;
            IsBuyButtonVisible = true;
            IsRenewButtonVisible = true;
            IsBackButtonVisible = false;
            IsOKButtonVisible = false;
            IsCancelButtonVisible = true;
            IsActivateButtonVisible = false;

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

        private BitmapImage captchaImage;

        public BitmapImage CaptchaImage
        {
            get
            {
                return captchaImage;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref captchaImage, value);
            }
        }

        private string captchaText;

        public string CaptchaText
        {
            get
            {
                return captchaText;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref captchaText, value);
            }
        }

        private bool isCaptchaVisible;

        public bool IsCaptchaVisible
        {
            get
            {
                return isCaptchaVisible;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isCaptchaVisible, value);
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

        private ObservableCollection<LicenseInfoViewModel> licenses;

        public ObservableCollection<LicenseInfoViewModel> Licenses
        {
            get
            {
                return licenses;
            }
        }

        #endregion

        #region Implementation of INotification

        private object content;

        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref title, value);
            }
        }

        #endregion

        #region Implementation of IInteractionRequestAware

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

        private INotification notification;

        public INotification Notification
        {
            get
            {
                return notification;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref notification, value);
            }
        }

        #endregion

        #endregion

        #region Commands        

        public ReactiveCommand<object> CancelCommand { get; private set; }

        public ReactiveCommand<object> SendCommand { get; private set; }

        public ReactiveCommand<object> TrialCommand { get; private set; }

        public ReactiveCommand<object> BuyCommand { get; private set; }

        public ReactiveCommand<object> RegisterCommand { get; private set; }

        public ReactiveCommand<object> RenewCommand { get; private set; }

        public ReactiveCommand<object> OKCommand { get; private set; }

        public ReactiveCommand<object> ActivateCommand { get; private set; }

        public ReactiveCommand<object> BackCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void Cancel()
        {
            if (!licenseService.IsRegistered)
            {
#if !DEBUG
                System.Windows.Application.Current.Shutdown();
#endif
            }

            if (FinishInteraction != null)
            {
                FinishInteraction();
            }
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
                trialSerial = server.RegisterTrial(CaptchaText, Email);
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response != null && response.StatusCode == HttpStatusCode.Found)
                {
                    serverUrl = new Uri(new Uri(server.Url), response.Headers["Location"]).AbsoluteUri;
                    server.Url = serverUrl;
                    trialSerial = server.RegisterTrial(CaptchaText, Email);
                }
            }

            if (string.IsNullOrWhiteSpace(trialSerial))
            {
                try
                {
                    InitializeCaptcha();
                }
                catch (LicenseNoConnectionException)
                {
                    InitializeMessage("Common_RegistrationView_NoConnection");
                    return;
                }

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
                InitializeMessage("Common_RegistrationView_InvalidSerial");
                return false;
            }
            catch (LicenseCouldNotActivateException)
            {
                InitializeActivation();
                return false;
            }
            catch (LicenseExpiredException)
            {
                InitializeMessage("Common_RegistrationView_ExpiredText");
                return false;
            }
            catch
            {
                InitializeMessage("Common_RegistrationView_InvalidSerial");
                return false;
            }

            var sessionsService = ServiceLocator.Current.GetInstance<ISessionService>();
            sessionsService.Initialize();

            this.NotificationRequest.Raise(
                 new PopupActionNotification
                 {
                     Content = "Thank you. DriveHUD has been successfully registered!",
                     Title = Title,
                 },
                 n => { });

            return true;
        }

        private void Register()
        {
            if (!Register(Serial, Email))
            {
                return;
            }

            Cancel();
        }

        private void Back()
        {
            Serial = string.Empty;
            Email = string.Empty;
            CaptchaText = string.Empty;

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

