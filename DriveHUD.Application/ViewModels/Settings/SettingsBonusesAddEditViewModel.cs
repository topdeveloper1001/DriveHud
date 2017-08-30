//-----------------------------------------------------------------------
// <copyright file="SettingsBonusesAddEditViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using ReactiveUI;
using System;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsBonusesAddEditViewModel : WindowViewModelBase
    {
        private SettingsRakeBackViewModelInfo<BonusModel> infoViewModel;
        private BonusModel settingsModel;

        internal SettingsBonusesAddEditViewModel(SettingsRakeBackViewModelInfo<BonusModel> info)
        {
            InitializeBindings();
            InitializeData(info);
        }

        #region Properties

        public SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        private string bonusName;

        public string BonusName
        {
            get
            {
                return bonusName;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref bonusName, value);
            }
        }

        private PlayerCollectionItem player;

        public PlayerCollectionItem Player
        {
            get
            {
                return player;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref player, value);
            }
        }

        private decimal amount;

        public decimal Amount
        {
            get
            {
                return amount;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref amount, value);
            }
        }

        private DateTime date;

        public DateTime Date
        {
            get
            {
                return date;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref date, value);
            }
        }

        #endregion

        #region ICommand

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void InitializeBindings()
        {
            var canSaveChanges = this.WhenAny(x => x.BonusName, x => x.Player, (x, y) => !string.IsNullOrWhiteSpace(x.Value) && y.Value != null);

            SaveCommand = ReactiveCommand.Create(canSaveChanges);
            SaveCommand.Subscribe(x => SaveChanges());

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x => Cancel());
        }

        private void InitializeData(SettingsRakeBackViewModelInfo<BonusModel> info)
        {
            infoViewModel = info;
            settingsModel = infoViewModel?.Model;

            BonusName = settingsModel?.BonusName ?? string.Empty;
            Player = StorageModel.PlayerCollection.OfType<PlayerCollectionItem>().FirstOrDefault(pl => pl.DecodedName == (settingsModel?.Player ?? string.Empty));
            Date = settingsModel?.Date ?? DateTime.Now;
            Amount = settingsModel?.Amount ?? 0m;
        }

        private void SaveChanges()
        {
            bool isAdd = false;

            if (settingsModel == null)
            {
                isAdd = true;
                settingsModel = new BonusModel();
            }

            settingsModel.BonusName = BonusName;
            settingsModel.Player = Player.DecodedName;
            settingsModel.Date = Date;
            settingsModel.Amount = Amount;

            if (isAdd)
            {
                infoViewModel?.Add(settingsModel);
            }
            else
            {
                infoViewModel?.Close();
            }
        }

        private void Cancel()
        {
            infoViewModel?.Close();
        }

        #endregion
    }
}