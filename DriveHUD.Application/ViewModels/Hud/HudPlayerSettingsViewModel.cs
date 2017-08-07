﻿//-----------------------------------------------------------------------
// <copyright file="HudPlayerSettingsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Images;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudPlayerSettingsViewModel : BaseRangeTypePopupViewModel
    {
        private readonly HudPlayerSettingsViewModelInfo viewModelInfo;
        private readonly IHudLayoutsService hudLayoutService;

        public HudPlayerSettingsViewModel(HudPlayerSettingsViewModelInfo viewModelInfo) : base()
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this.viewModelInfo = viewModelInfo;

            hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            Initialize();
        }

        private void Initialize()
        {
            playerTypes = new ObservableCollection<HudPlayerType>(viewModelInfo.PlayerTypes);
            selectedPlayerType = playerTypes.FirstOrDefault();
        }

        protected override void InitializeCommands()
        {
            base.InitializeCommands();

            LoadCommand = ReactiveCommand.Create();
            LoadCommand.Subscribe(x => Load());

            var canDelete = this.WhenAny(x => x.SelectedPlayerType, x => x.Value != null);

            DeleteCommand = ReactiveCommand.Create(canDelete);
            DeleteCommand.Subscribe(x =>
            {
                if (SelectedPlayerType != null)
                {
                    PlayerTypes.Remove(SelectedPlayerType);
                    SelectedPlayerType = PlayerTypes.FirstOrDefault();
                }
            });

            ResetCommand = ReactiveCommand.Create(canDelete);
            ResetCommand.Subscribe(x =>
            {
                var defaultPlayerTypes = hudLayoutService.CreateDefaultPlayerTypes(viewModelInfo.TableType);

                var defaultPlayerType = defaultPlayerTypes.FirstOrDefault(p => p.Name == SelectedPlayerType.Name);

                if (defaultPlayerType == null)
                {
                    return;
                }

                SelectedPlayerType.StatsToMerge = defaultPlayerType.Stats;
            });
        }

        #region Commands

        public ReactiveCommand<object> LoadCommand { get; private set; }

        public ReactiveCommand<object> ResetCommand { get; private set; }

        public ReactiveCommand<object> DeleteCommand { get; private set; }

        #endregion

        #region Properties 

        private ObservableCollection<HudPlayerType> playerTypes;

        public ObservableCollection<HudPlayerType> PlayerTypes
        {
            get
            {
                return playerTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref playerTypes, value);
            }
        }

        private HudPlayerType selectedPlayerType;

        public HudPlayerType SelectedPlayerType
        {
            get
            {
                return selectedPlayerType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedPlayerType, value);
            }
        }

        #endregion

        #region Validation

        private bool Validate()
        {
            var groupedByName = (from playerType in PlayerTypes
                                 where !string.IsNullOrWhiteSpace(playerType.Name)
                                 group playerType by playerType.Name into grouped
                                 select new { Name = grouped.Key, Count = grouped.Count() });

            var validationResult = PlayerTypes.All(x => !string.IsNullOrWhiteSpace(x.Name)) && groupedByName.All(x => x.Count < 2);

            return validationResult;
        }

        #endregion

        #region Infrastructure

        protected override IObservable<bool> CanSave()
        {
            return this.WhenAny(x => x.SelectedPlayerType.Name, y => !string.IsNullOrWhiteSpace(y.Value) && Validate());
        }

        protected override void Save()
        {
            viewModelInfo.Save?.Invoke();
        }

        private void Load()
        {
            var initialDirectory = hudLayoutService.GetImageDirectory();

            var openFileDialog = new OpenFileDialog
            {
                Filter = "PNG Images (.png)|*.png",
                InitialDirectory = initialDirectory,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var image = ImageHelper.CopyAndProcessImage(openFileDialog.FileName, initialDirectory, 24, 24);

                    if (SelectedPlayerType != null && !string.IsNullOrWhiteSpace(image))
                    {
                        SelectedPlayerType.Image = image;
                        SelectedPlayerType.ImageAlias = Path.GetFileName(image);
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, e);
                }
            }
        }

        protected override void Create()
        {
            var hudPlayerType = new HudPlayerType(true);
            PlayerTypes.Add(hudPlayerType);
            SelectedPlayerType = hudPlayerType;
        }

        #endregion
    }
}