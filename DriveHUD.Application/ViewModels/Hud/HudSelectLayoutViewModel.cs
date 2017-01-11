//-----------------------------------------------------------------------
// <copyright file="HudSelectLayoutViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Linq;
using ReactiveUI;
using Microsoft.Practices.ServiceLocation;
using System.Reactive.Linq;
using DriveHUD.Common;
using DriveHUD.Common.Resources;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels
{
    public class HudSelectLayoutViewModel : ViewModelBase
    {
        private HudSelectLayoutViewModelInfo viewModelInfo;

        public HudSelectLayoutViewModel(HudSelectLayoutViewModelInfo viewModelInfo)
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this.viewModelInfo = viewModelInfo;

            Initialize();
        }

        private void Initialize()
        {
            ShowInput = viewModelInfo.IsSaveAsMode;

            Name = $"{viewModelInfo.LayoutName} Copy";

            var canSave = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));

            SaveCommand = ReactiveCommand.Create(canSave);
            SaveCommand.Subscribe(x =>
            {
                if (viewModelInfo.Save != null)
                {
                    viewModelInfo.Save();
                }
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                if (viewModelInfo.Cancel != null)
                {
                    viewModelInfo.Cancel();
                }
            });
        }

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }


        public string Header
        {
            get
            {
                return viewModelInfo.IsDeleteMode ?
                            CommonResourceManager.Instance.GetResourceString("Common_HudLayout_DeleteLabel") :
                            CommonResourceManager.Instance.GetResourceString("Common_HudLayout_SelectLabel");
            }
        }

        private bool showInput;

        public bool ShowInput
        {
            get
            {
                return showInput;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref showInput, value);
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
    }
}