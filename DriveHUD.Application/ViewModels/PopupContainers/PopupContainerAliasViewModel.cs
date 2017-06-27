using DriveHUD.Application.ViewModels.Alias;
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
    public class PopupContainerAliasViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Constructor

        public PopupContainerAliasViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            OkCommand = new RelayCommand(Ok);
        }

        private void InitializeViewModel()
        {
            if (SelectedViewModel == null)
            {
                SelectedViewModel = new AliasViewModel();
            }
        }

        #endregion

        #region Properties

        private PopupContainerAliasViewModelNotification _notification;
        private object _selectedViewModel;

        public object SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { SetProperty(ref _selectedViewModel, value); }
        }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this._notification; }
            set
            {
                if (value is PopupContainerAliasViewModelNotification)
                {
                    this._notification = value as PopupContainerAliasViewModelNotification;
                    this.OnPropertyChanged(() => this.Notification);

                    InitializeViewModel();
                }
            }
        }

        #endregion

        #region Methods

        private void Ok(object obj)
        {
            FinishInteraction.Invoke();
        }

        #endregion

        #region Commands

        public ICommand OkCommand { get; set; }

        #endregion
    }
}
