//-----------------------------------------------------------------------
// <copyright file="ProgressViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Progress;
using ReactiveUI;
using System.Globalization;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Progress view model
    /// </summary>
    public class ProgressViewModel : ViewModelBase, IProgressViewModel
    {
        private DHProgress progress;

        public ProgressViewModel()
        {
            progress = new DHProgress();
            progress.ProgressChanged += OnProgressChanged;
        }

        #region Properties        

        private decimal minimum;

        public decimal Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref minimum, value);
            }
        }

        private decimal maximum;

        public decimal Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref maximum, value);
            }
        }

        private decimal progressValue;

        public decimal ProgressValue
        {
            get
            {
                return progressValue;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref progressValue, value);
            }
        }

        private bool isActive;

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isActive, value);
            }
        }

        private string text;

        public string Text
        {
            get
            {
                return text;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref text, value);
            }
        }

        public IDHProgress Progress
        {
            get
            {
                return progress;
            }
        }

        #endregion

        #region Infrastructure

        private void OnProgressChanged(object sender, ProgressItem e)
        {
            if (e == null)
            {
                return;
            }

            if (!IsActive)
            {
                IsActive = true;
            }

            Text = e.Message.ToString(CultureInfo.CurrentUICulture);
            ProgressValue = e.ProgressValue;
        }

        #endregion
    }
}