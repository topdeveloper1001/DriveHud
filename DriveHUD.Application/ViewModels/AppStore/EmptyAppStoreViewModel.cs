﻿//-----------------------------------------------------------------------
// <copyright file="AppsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Shop;

namespace DriveHUD.Application.ViewModels.AppStore
{
    /// <summary>
    /// Dummy view model, will be replaced with real view models
    /// </summary>
    public class EmptyAppStoreViewModel : AppStoreViewModel<IProductAppStoreModel>
    {
        public override void Initialize()
        {            
        }

        protected override void ModelInitialized()
        {         
        }

        protected override void OnAddToCart(object item)
        {         
        }

        protected override void OnLearnMore(object item)
        {         
        }
    }
}