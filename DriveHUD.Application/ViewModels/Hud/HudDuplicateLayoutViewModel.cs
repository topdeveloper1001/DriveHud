//-----------------------------------------------------------------------
// <copyright file="HudDuplicateLayoutViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudDuplicateLayoutViewModel : ViewModelBase
    {
        private readonly HudDuplicateLayoutViewModelInfo viewModelInfo;

        public HudDuplicateLayoutViewModel(HudDuplicateLayoutViewModelInfo viewModelInfo)
        {
            Check.Require(viewModelInfo != null);

            this.viewModelInfo = viewModelInfo;

            Initialize();
        }

        #region Properties

        private ObservableCollection<EnumTableType> tableTypes;

        public ObservableCollection<EnumTableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
        }

        private EnumTableType? selectedTableType;

        public EnumTableType? SelectedTableType
        {
            get
            {
                return selectedTableType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedTableType, value);
            }
        }

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

        #endregion

        #region Commands

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }

        #endregion

        private void Initialize()
        {
            tableTypes = new ObservableCollection<EnumTableType>(Enum.GetValues(typeof(EnumTableType)).OfType<EnumTableType>());

            name = viewModelInfo.LayoutName;

            var canSave = this.WhenAny(x => x.Name, x => x.SelectedTableType, (x1, x2) => !string.IsNullOrWhiteSpace(x1.Value) && x2.Value.HasValue);

            SaveCommand = ReactiveCommand.Create(canSave);
            SaveCommand.Subscribe(x =>
            {
                viewModelInfo.Save?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                viewModelInfo.Cancel?.Invoke();
            });

        }
    }
}