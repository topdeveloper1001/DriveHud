using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using Model.Settings;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Model;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsSendLogViewModel : BaseViewModel
    {
        public static Action OnClose;
        internal SettingsSendLogViewModel() 
        {
            Initialize();
        }

        private void Initialize()
        {
            SendMessageCommand = new DelegateCommand(SendMessage, CanSend);

            IsSending = false;
        }

        #region Properties

        private string _userName;
        private string _userEmail;
        private string _userMessage;
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

        #endregion

        #region ICommand Implementation

        private async void SendMessage()
        {
            try
            {
                IsSending = true;

                using (var message = SmtpClientHelper.ComposeSupportEmail(UserName, UserEmail, UserMessage, AppDomain.CurrentDomain.BaseDirectory, StringFormatter.GetAppDataFolderPath()))
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
            finally
            {
                OnClose?.Invoke();
            }

            IsSending = false;
        }

        private bool CanSend()
        {
            bool canSend = Utils.IsValidEmail(UserEmail) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(UserMessage);
            return canSend;
        }

        #endregion

    }
}
