//-----------------------------------------------------------------------
// <copyright file="RakebackAppStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using Model.AppStore;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class RakebackAppStoreViewModel : AppStoreBaseViewModel<IRakebackAppStoreModel>
    {
        public override void Initialize()
        {
            base.Initialize();

            GridColumns = 2;
            GridRows = 2;

            InitializeModelAsync(() =>
            {
                Model.Load(null);
            });
        }

        public ReactiveCommand SignUpCommand { get; private set; }

        protected override void InitializeCommands()
        {
            SignUpCommand = ReactiveCommand.Create<AppStoreRakeback>(x =>
            {
                if (string.IsNullOrWhiteSpace(x.SignUpLink))
                {
                    return;
                }

                BrowserHelper.OpenLinkInBrowser(x.SignUpLink);
            });
        }
    }
}