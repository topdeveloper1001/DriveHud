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
using DriveHUD.Common.Wpf.Mvvm;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Progress view model
    /// </summary>
    public class ProgressViewModel : ViewModelBase, IProgressViewModel
    {
        private CancellationTokenSource cancellationTokerSource;

        private DHProgress progress;

        public ProgressViewModel()
        {
            cancellationTokerSource = new CancellationTokenSource();

            progress = new DHProgress(cancellationTokerSource.Token);
            progress.ProgressChanged += OnProgressChanged;

            StopCommand = ReactiveCommand.Create();
            StopCommand.Subscribe(x => Stop());
        }

        public void Reset()
        {
            if (progress != null)
            {
                cancellationTokerSource = new CancellationTokenSource();
                progress.Reset(cancellationTokerSource.Token);
            }
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

        #region Commands

        public ReactiveCommand<object> StopCommand { get; private set; }

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

        private void Stop()
        {
            if (cancellationTokerSource != null && !cancellationTokerSource.IsCancellationRequested)
            {
                cancellationTokerSource.Cancel();
            }
        }

        #endregion
    }
}