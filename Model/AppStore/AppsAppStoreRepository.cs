﻿//-----------------------------------------------------------------------
// <copyright file="AppsAppStoreRepository.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;

namespace Model.AppStore
{
    internal class AppsAppStoreRepository : IAppsAppStoreRepository
    {
        public IEnumerable<AppStoreProduct> GetAllProducts()
        {
            var playerXRayApp = new AppStoreModule
            {
                ProductName = "Player X-Ray",
                ProductDescription = "An automated note taking application that will allow you to see right through your opponents strategies in order to make the best possible adjustments to their game.",
                ImageLink = "pack://application:,,,/DriveHUD.Common.Resources;component/images/Shop/player-xray-logo.png",
                ModuleName = CustomModulesNames.PlayerXRay
            };

            return new[] { playerXRayApp };
        }
    }
}