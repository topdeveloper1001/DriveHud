using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Infrastructure.Base;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerBaseNotificationViewModel : BaseViewModel, IInteractionRequestAware
    {
        public PopupContainerBaseNotificationViewModel()
        {
            ConfirmCommand = new RelayCommand(DoConfirm);
            CancelCommand = new RelayCommand(DoCancel);
        }

        #region Properties

        private bool _isDisplayH1Text;
        public bool IsDisplayH1Text
        {
            get { return _isDisplayH1Text; }
            set { SetProperty(ref _isDisplayH1Text, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                SetProperty(ref _content, value);
            }
        }

        private string _confirmButtonCaption;
        public string ConfirmButtonCaption
        {
            get { return _confirmButtonCaption; }
            set
            {
                SetProperty(ref _confirmButtonCaption, value);
            }
        }

        private string _cancelButtonCaption;
        public string CancelButtonCaption
        {
            get { return _cancelButtonCaption; }
            set
            {
                SetProperty(ref _cancelButtonCaption, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void DoConfirm(object obj)
        {
            _notification.Confirmed = true;
            FinishInteraction.Invoke();
        }

        private void DoCancel(object obj)
        {
            _notification.Confirmed = false;
            FinishInteraction.Invoke();
        }

        #endregion

        #region IInteractionRequestAware

        public Action FinishInteraction { get; set; }

        private PopupBaseNotification _notification;
        public INotification Notification
        {
            get
            {
                return _notification;
            }

            set
            {
                _notification = value as PopupBaseNotification;
                if(_notification != null)
                {
                  //  Title = _notification.Title;
                    Content = _notification.Content.ToString();
                    ConfirmButtonCaption = _notification.ConfirmButtonCaption;
                    CancelButtonCaption = _notification.CancelButtonCaption;
                    IsDisplayH1Text = _notification.IsDisplayH1Text;

                    _notification.Confirmed = false;
                }
            }
        }

        #endregion
    }
}
