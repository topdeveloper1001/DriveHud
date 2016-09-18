using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Settings;
using System.Windows.Input;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Utils;
using System.Diagnostics;
using Prism.Commands;
using DriveHUD.Common.Resources;
using System.Net.Mail;
using System.Windows;
using System.IO;
using DriveHUD.Common.Log;

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
            SendMessageCommand = new DelegateCommand(SendMessage, CanSend);
            ShowOnlineManualCommand = new RelayCommand(ShowOnlineManual);
            ShowKnowledgeBaseCommand = new RelayCommand(ShowKnowledgeBase);
            ShowSupportForumsCommand = new RelayCommand(ShowSupportForums);

            AttachLog = true;
            IsSending = false;
        }

        #region Properties

        private string _userName;
        private string _userEmail;
        private string _userMessage;
        private bool _attachLog;
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

                using (var message = SmtpClientHelper.ComposeSupportEmail(UserName, UserEmail, UserMessage, AttachLog))
                using (var client = SmtpClientHelper.GetSmtpClient())
                {
                    await client.SendMailAsync(message);
                }

                MessageBox.Show("The message has been sent.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send message");
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

        #endregion
    }
}
