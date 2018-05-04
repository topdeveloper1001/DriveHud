using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Actions;
using Model;
using Model.Settings;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsSupportViewModel : SettingsViewModel<ISettingsBase>
    {
        internal SettingsSupportViewModel(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            NotificationRequest = new InteractionRequest<INotification>();

            SendMessageCommand = new DelegateCommand(SendMessage, CanSend);
            ShowOnlineManualCommand = new RelayCommand(ShowOnlineManual);
            ShowKnowledgeBaseCommand = new RelayCommand(ShowKnowledgeBase);
            ShowSupportForumsCommand = new RelayCommand(ShowSupportForums);

            AttachLog = true;
            IsSending = false;
        }

        #region Properties

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private string _userName;
        private string _userEmail;
        private string _userMessage;
        private bool _attachLog;
        private bool _sendAdvancedLog;
        private bool _isSending;

        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
                SendMessageCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                SetProperty(ref _userEmail, value);
                SendMessageCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserMessage
        {
            get { return _userMessage; }
            set
            {
                SetProperty(ref _userMessage, value);
                SendMessageCommand.RaiseCanExecuteChanged();
            }
        }

        public bool AttachLog
        {
            get { return _attachLog; }
            set
            {
                SetProperty(ref _attachLog, value);
            }
        }

        public bool SendAdvancedLog
        {
            get { return _sendAdvancedLog; }
            set
            {
                SetProperty(ref _sendAdvancedLog, value);
            }
        }

        public bool IsSending
        {
            get { return _isSending; }
            set
            {
                SetProperty(ref _isSending, value);
            }
        }

        #endregion

        #region ICommand

        public DelegateCommand SendMessageCommand { get; set; }
        public ICommand ShowOnlineManualCommand { get; set; }
        public ICommand ShowKnowledgeBaseCommand { get; set; }
        public ICommand ShowSupportForumsCommand { get; set; }

        #endregion

        #region ICommand Implementation

        private async void SendMessage()
        {
            try
            {
                IsSending = true;
                var appFolder = string.Empty;
                var dataFolder = string.Empty;
                if (AttachLog)
                {
                    appFolder = AppDomain.CurrentDomain.BaseDirectory;
                    dataFolder = StringFormatter.GetAppDataFolderPath();
                }
                using (var message = SmtpClientHelper.ComposeSupportEmail(UserName, UserEmail, UserMessage, appFolder, dataFolder, SendAdvancedLog))
                using (var client = SmtpClientHelper.GetSmtpClient())
                {
                    await client.SendMailAsync(message);
                }

                RaiseMessage("Support", "The message has been sent.");
            }
            catch (Exception ex)
            {
                RaiseMessage("Support", "Failed to send message.");
                LogProvider.Log.Error(ex);
            }

            IsSending = false;
        }

        private void ShowOnlineManual(object obj)
        {
            Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("SystemSettings_OnlineManual"));
        }

        private void ShowKnowledgeBase(object obj)
        {
            Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("SystemSettings_KnowledgeBase"));
        }

        private void ShowSupportForums(object obj)
        {
            Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("SystemSettings_ForumsLink"));
        }

        private bool CanSend()
        {
            bool canSend = Utils.IsValidEmail(UserEmail) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserMessage);
            return canSend;
        }

        private void RaiseMessage(string title, string content)
        {
            this.NotificationRequest.Raise(
                 new PopupActionNotification
                 {
                     Content = content,
                     Title = title,
                 });
        }

        #endregion
    }
}
