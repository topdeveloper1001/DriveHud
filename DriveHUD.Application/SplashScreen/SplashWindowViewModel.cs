//-----------------------------------------------------------------------
// <copyright file="SplashWindowViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Progress;
using System.Globalization;

namespace DriveHUD.Application.SplashScreen
{
    public class SplashWindowViewModel : BaseViewModel
    {
        private DHProgress progress;

        public SplashWindowViewModel()
        {
            progress = new DHProgress();
            progress.ProgressChanged += OnProgressChanged;
        }

        private string status;

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public IDHProgress Progress
        {
            get
            {
                return progress;
            }
        }

        private void OnProgressChanged(object sender, ProgressItem e)
        {
            if (e == null)
            {
                return;
            }

            Status = e.Message.ToString(CultureInfo.CurrentUICulture);
        }   

        private NotificationWindow notificationWindow;

        public void ShowNotification(BaseViewModel viewModel)
        {
            notificationWindow = new NotificationWindow();
            notificationWindow.DataContext = viewModel;
            notificationWindow.ShowDialog();
        }

        public void CloseNotification()
        {
            notificationWindow?.Close();
        }
    }
}