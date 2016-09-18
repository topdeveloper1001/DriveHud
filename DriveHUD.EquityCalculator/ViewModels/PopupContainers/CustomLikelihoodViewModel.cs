using System;
using DriveHUD.Common.Infrastructure.Base;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Input;
using Model.Notifications;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class CustomLikelihoodViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields
        private CustomLikelihoodNotification _notification;
        private int _likelihood;
        #endregion

        #region Properties
        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                if (value is CustomLikelihoodNotification)
                {
                    this._notification = value as CustomLikelihoodNotification;
                    this._notification.Likelihood = -1;
                    this.Likelihood = 0;
                }
            }
        }

        public int Likelihood
        {
            get { return _likelihood; }
            set { SetProperty(ref _likelihood, value); }
        }
        #endregion

        #region ICommand
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        #endregion

        public CustomLikelihoodViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            ExitCommand = new RelayCommand(Exit);
        }

        #region ICommand Implementation
        private void Exit(object obj)
        {
            this.FinishInteraction();
        }

        private void Save(object obj)
        {
            this._notification.Likelihood = Likelihood;
            this.FinishInteraction();
        }
        #endregion
    }
}
