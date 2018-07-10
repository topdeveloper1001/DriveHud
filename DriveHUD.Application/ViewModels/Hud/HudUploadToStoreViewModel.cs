//-----------------------------------------------------------------------
// <copyright file="HudUploadToStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Common.Wpf.Validation;
using ReactiveUI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudUploadToStoreViewModel : WindowViewModel<HudUploadToStoreViewModel>, IHudUploadToStoreViewModel, INotifyDataErrorInfo
    {
        static HudUploadToStoreViewModel()
        {
            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Description),
                new NonLocalizableString("Description must be not empty"),
                x =>
                {
                    Task.Delay(5000).Wait();
                    return !string.IsNullOrEmpty(x.Description);
                }, true));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Name),
                new NonLocalizableString("Name must be not empty"),
                x =>
                {
                    Task.Delay(2500).Wait();
                    return !string.IsNullOrEmpty(x.Name);
                }, true));

        }

        public override void Configure(object viewModelInfo)
        {
            InitializeCommands();
            OnInitialized();
        }

        #region Properties

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
            }
        }

        private decimal cost;

        public decimal Cost
        {
            get
            {
                return cost;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref cost, value);
            }
        }

        #endregion

        #region Command

        public ReactiveCommand SubmitCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand ResetCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeCommands()
        {
            var canSubmit = this.WhenAny(x => x.HasErrors, x => x.IsValidating, (x1, x2) => !HasErrors && !IsValidating);

            SubmitCommand = ReactiveCommand.Create(() => Upload(), canSubmit);
            CancelCommand = ReactiveCommand.Create(() => OnClosed());
            ResetCommand = ReactiveCommand.Create(() => Reset());
        }

        private void Upload()
        {
            StartAsyncOperation(() =>
            {
                Task.Delay(3000).Wait();

                // exception or just error ?
                // e.g. timeout or some server error
                // duplicate?
                // show error form with Retry or Back button

            },
            () => { });
        }

        private void Reset()
        {
            Name = string.Empty;
        }

        #endregion
    }
}