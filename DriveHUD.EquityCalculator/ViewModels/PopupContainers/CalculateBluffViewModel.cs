using DriveHUD.Common.Infrastructure.Base;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class CalculateBluffViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields
        private CalculateBluffNotification _notification;
        private int _betValue = 0;
        private int _potValue = 0;
        private int _numberOfPlayers = 0;
        private double _equityValue = 0;
        private double _bluffPercentValue = 0;
        #endregion

        #region Properties
        public Action FinishInteraction
        {
            get; set;
        }

        public INotification Notification
        {
            get
            {
                return this._notification;
            }

            set
            {
                if (value is CalculateBluffNotification)
                {
                    this._notification = value as CalculateBluffNotification;
                    this.EquityValue = this._notification.EquityValue;
                    this.NumberOfPlayers = this._notification.NumberOfPlayers;
                    ResetView();
                    this.OnPropertyChanged(() => this.Notification);
                }
            }
        }

        public int BetValue
        {
            get
            {
                return _betValue;
            }

            set
            {
                SetProperty(ref _betValue, value);
            }
        }

        public int PotValue
        {
            get
            {
                return _potValue;
            }

            set
            {
                SetProperty(ref _potValue, value);
            }
        }

        public double EquityValue
        {
            get
            {
                return _equityValue;
            }

            set
            {
                _equityValue = value;
            }
        }

        public int NumberOfPlayers
        {
            get
            {
                return _numberOfPlayers;
            }

            set
            {
                _numberOfPlayers = value;
            }
        }

        public double BluffPercentValue
        {
            get
            {
                return _bluffPercentValue;
            }

            set
            {
                SetProperty(ref _bluffPercentValue, value);
            }
        }
        #endregion

        #region ICommand
        public ICommand CalculateCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        #endregion

        public CalculateBluffViewModel()
        {
            Init();
        }

        private void Init()
        {
            CalculateCommand = new RelayCommand(CalculateBluff);
            ExitCommand = new RelayCommand(Exit);
        }

        private void ResetView()
        {
            this.BetValue = 0;
            this.PotValue = 0;
            this.BluffPercentValue = 0;
        }

        #region ICommand implementation
        private void Exit(object obj)
        {
            this.FinishInteraction();
        }

        private void CalculateBluff(object obj)
        {
            double P = this.PotValue;
            double L = this.BetValue;
            double H = this.EquityValue / 100;
            double V = 1 - H;
            double W = P + L * (this.NumberOfPlayers - 1);

            double X = ((L * V) - (W * H)) / (P + (L * V) - (W * H));
            if (X > 1)
            {
                X = 1;
            }
            if (X < 0)
            {
                X = 0;
            }
            this.BluffPercentValue = X * 100;
        }
        #endregion
    }
}
