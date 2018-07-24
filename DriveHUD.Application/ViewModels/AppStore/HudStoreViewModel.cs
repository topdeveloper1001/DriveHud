//-----------------------------------------------------------------------
// <copyright file="HudStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Licensing;
using Microsoft.Practices.ServiceLocation;
using Model.AppStore;
using Model.AppStore.HudStore.ServiceData;
using System;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class HudStoreViewModel : AppStoreBaseViewModel<IHudStoreModel>, IHudStoreViewModel
    {
        public override void Initialize()
        {
            base.Initialize();

            GridColumns = 4;
            GridRows = 2;

            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            var license = licenseService.GetHudStoreLicenseInfo(true);

            var loadInfo = new HudStoreGetHudsRequest
            {
                Serial = license?.Serial
            };

            InitializeModelAsync(() =>
            {
                Model.Load(loadInfo);
            });
        }

        protected override void ModelInitialized(Exception ex)
        {
            base.ModelInitialized(ex);
        }

        protected override void InitializeCommands()
        {
        }

        public override void Refresh(int pageNumber)
        {
            StartAsyncOperation(() => base.Refresh(pageNumber), () => Model.Refresh());      
        }
    }
}