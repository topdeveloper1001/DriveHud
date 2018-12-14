//-----------------------------------------------------------------------
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
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.Hud;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudPlayerSettingsViewModel : BaseRangeTypePopupViewModel
    {
        private readonly HudPlayerSettingsViewModelInfo viewModelInfo;
        private readonly IHudLayoutsService hudLayoutService;
        private readonly IHudPlayerTypeService playerTypeService;

        public HudPlayerSettingsViewModel(HudPlayerSettingsViewModelInfo viewModelInfo) : base()
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this.viewModelInfo = viewModelInfo;

            hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            playerTypeService = ServiceLocator.Current.GetInstance<IHudPlayerTypeService>();

            Initialize();
        }

        private void Initialize()
        {
            PlayerTypes = new ObservableCollection<HudPlayerType>(viewModelInfo.PlayerTypes);
            SelectedPlayerType = playerTypes.FirstOrDefault();
        }

        protected override void InitializeCommands()
        {
            base.InitializeCommands();

            LoadCommand = ReactiveCommand.Create(() => Load());

            var canDelete = this.WhenAny(x => x.SelectedPlayerType, x => x.Value != null);

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedPlayerType != null)
                {
                    PlayerTypes.Remove(SelectedPlayerType);
                    SelectedPlayerType = PlayerTypes.FirstOrDefault();
                }
            }, canDelete);

            ResetCommand = ReactiveCommand.Create(() =>
            {
                var defaultPlayerTypes = playerTypeService.CreateDefaultPlayerTypes(viewModelInfo.TableType);

                var defaultPlayerType = defaultPlayerTypes.FirstOrDefault(p => p.Name == SelectedPlayerType.Name);

                if (defaultPlayerType == null)
                {
                    return;
                }

                SelectedPlayerType.StatsToMerge = defaultPlayerType.Stats;
            }, canDelete);

            ExportCommand = ReactiveCommand.Create(() => Export(new[] { SelectedPlayerType }), canDelete);
            ExportAllCommand = ReactiveCommand.Create(() => Export(playerTypes));
            ImportCommand = ReactiveCommand.Create(() => Import());
        }

        #region Commands

        public ReactiveCommand LoadCommand { get; private set; }

        public ReactiveCommand ResetCommand { get; private set; }

        public ReactiveCommand DeleteCommand { get; private set; }

        public ReactiveCommand ExportCommand { get; private set; }

        public ReactiveCommand ExportAllCommand { get; private set; }

        public ReactiveCommand ImportCommand { get; private set; }

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

        protected override void Create()
        {
            var hudPlayerType = new HudPlayerType(true);
            PlayerTypes.Add(hudPlayerType);
            SelectedPlayerType = hudPlayerType;
        }

        private void Load()
        {
            var initialDirectory = playerTypeService.GetImageDirectory();

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

        private void Import()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = CommonResourceManager.Instance.GetResourceString("SystemSettings_PlayerTypeFileDialogFilter")
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            var importedPlayerTypes = hudLayoutService.ImportPlayerType(openFileDialog.FileName);

            if (importedPlayerTypes == null || importedPlayerTypes.Length == 0)
            {
                return;
            }

            var playerTypesMap = (from importedPlayerType in importedPlayerTypes
                                  join playerType in PlayerTypes on importedPlayerType.Name equals playerType.Name into gj
                                  from grouped in gj.DefaultIfEmpty()
                                  select new { ImportedPlayerType = importedPlayerType, ExistingPlayerType = grouped }).ToArray();

            foreach (var playerTypeMapItem in playerTypesMap)
            {
                if (playerTypeMapItem.ExistingPlayerType == null)
                {
                    PlayerTypes.Add(playerTypeMapItem.ImportedPlayerType);
                    continue;
                }

                playerTypeMapItem.ExistingPlayerType.MergeWith(playerTypeMapItem.ImportedPlayerType);
            }
        }

        private void Export(IEnumerable<HudPlayerType> playerTypes)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = CommonResourceManager.Instance.GetResourceString("SystemSettings_PlayerTypeFileDialogFilter")
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                hudLayoutService.ExportPlayerType(playerTypes.ToArray(), saveFileDialog.FileName);
            }
        }

        #endregion
    }
}