//-----------------------------------------------------------------------
// <copyright file="SettingsRakeBackAddEditViewModel.cs" company="Ace Poker Solutions">
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
    public class SettingsRakeBackAddEditViewModel : WindowViewModelBase
    {
        private SettingsRakeBackViewModelInfo<RakeBackModel> infoViewModel;
        private RakeBackModel settingsModel;

        internal SettingsRakeBackAddEditViewModel(SettingsRakeBackViewModelInfo<RakeBackModel> info)
        {
            InitializeBindings();
            InitializeData(info);
        }

        #region Properties

        public SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        private string rakeBackName;

        public string RakeBackName
        {
            get
            {
                return rakeBackName;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref rakeBackName, value);
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

        private decimal percentage;

        public decimal Percentage
        {
            get
            {
                return percentage;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref percentage, value);
            }
        }

        private DateTime dateBegan;

        public DateTime DateBegan
        {
            get
            {
                return dateBegan;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref dateBegan, value);
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
            var canSaveChanges = this.WhenAny(x => x.RakeBackName, x => x.Player, (x, y) => !string.IsNullOrWhiteSpace(x.Value) && y.Value != null);

            SaveCommand = ReactiveCommand.Create(canSaveChanges);
            SaveCommand.Subscribe(x => SaveChanges());

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x => Cancel());
        }

        private void InitializeData(SettingsRakeBackViewModelInfo<RakeBackModel> info)
        {
            infoViewModel = info;
            settingsModel = infoViewModel?.Model;

            RakeBackName = settingsModel?.RakeBackName ?? string.Empty;
            Player = StorageModel.PlayerCollection.OfType<PlayerCollectionItem>().FirstOrDefault(pl => pl.DecodedName == (settingsModel?.Player ?? string.Empty));
            DateBegan = settingsModel?.DateBegan ?? DateTime.Now;
            Percentage = settingsModel?.Percentage ?? 0m;
        }

        private void SaveChanges()
        {
            bool isAdd = false;
            if (settingsModel == null)
            {
                isAdd = true;
                settingsModel = new RakeBackModel();
            }

            settingsModel.RakeBackName = RakeBackName;
            settingsModel.Player = Player.DecodedName;
            settingsModel.DateBegan = DateBegan;
            settingsModel.Percentage = Percentage;

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