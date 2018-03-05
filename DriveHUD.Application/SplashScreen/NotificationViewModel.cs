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
using System.Windows.Input;

namespace DriveHUD.Application.SplashScreen
{
    public class NotificationViewModel : BaseViewModel
    {
        public NotificationViewModel()
        {
            Button1Command = new RelayCommand(() => Button1Action?.Invoke());
            Button2Command = new RelayCommand(() => Button2Action?.Invoke());
            Button3Command = new RelayCommand(() => Button3Action?.Invoke());
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
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        private string button1Text;

        public string Button1Text
        {
            get
            {
                return button1Text;
            }
            set
            {
                if (button1Text == value)
                {
                    return;
                }

                button1Text = value;
                RaisePropertyChanged();
            }
        }

        private Action button1Action;

        public Action Button1Action
        {
            get
            {
                return button1Action;
            }
            set
            {
                if (button1Action == value)
                {
                    return;
                }

                button1Action = value;
                RaisePropertyChanged();
            }
        }

        private string button2Text;

        public string Button2Text
        {
            get
            {
                return button2Text;
            }
            set
            {
                if (button2Text == value)
                {
                    return;
                }

                button2Text = value;
                RaisePropertyChanged();
            }
        }

        private Action button2Action;

        public Action Button2Action
        {
            get
            {
                return button2Action;
            }
            set
            {
                if (button2Action == value)
                {
                    return;
                }

                button2Action = value;
                RaisePropertyChanged();
            }
        }

        private string button3Text;

        public string Button3Text
        {
            get
            {
                return button3Text;
            }
            set
            {
                if (button3Text == value)
                {
                    return;
                }

                button3Text = value;
                RaisePropertyChanged();
            }
        }

        private Action button3Action;

        public Action Button3Action
        {
            get
            {
                return button3Action;
            }
            set
            {
                if (button3Action == value)
                {
                    return;
                }

                button3Action = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand Button1Command { get; private set; }

        public ICommand Button2Command { get; private set; }

        public ICommand Button3Command { get; private set; }

        #endregion
    }
}