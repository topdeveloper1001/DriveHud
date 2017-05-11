//-----------------------------------------------------------------------
// <copyright file="NotificationViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.SplashScreen
{
    public class NotificationViewModel : BaseViewModel
    {
        public NotificationViewModel()
        {
            ConfirmCommand = new RelayCommand(() => ConfirmButtonAction?.Invoke());
            CancelCommand = new RelayCommand(() => CancelButtonAction?.Invoke());
        }

        #region Properties

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title == value)
                {
                    return;
                }

                title = value;
                OnPropertyChanged();
            }
        }

        private string message;

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (message == value)
                {
                    return;
                }

                message = value;
                OnPropertyChanged();
            }
        }

        private string confirmButtonText;

        public string ConfirmButtonText
        {
            get
            {
                return confirmButtonText;
            }
            set
            {
                if (confirmButtonText == value)
                {
                    return;
                }

                confirmButtonText = value;
                OnPropertyChanged();
            }
        }

        private Action confirmButtonAction;

        public Action ConfirmButtonAction
        {
            get
            {
                return confirmButtonAction;
            }
            set
            {
                if (confirmButtonAction == value)
                {
                    return;
                }

                confirmButtonAction = value;
                OnPropertyChanged();
            }
        }

        private string cancelButtonText;

        public string CancelButtonText
        {
            get
            {
                return cancelButtonText;
            }
            set
            {
                if (cancelButtonText == value)
                {
                    return;
                }

                cancelButtonText = value;
                OnPropertyChanged();
            }
        }

        private Action cancelButtonAction;

        public Action CancelButtonAction
        {
            get
            {
                return cancelButtonAction;
            }
            set
            {
                if (confirmButtonAction == value)
                {
                    return;
                }

                cancelButtonAction = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand ConfirmCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        #endregion
    }
}