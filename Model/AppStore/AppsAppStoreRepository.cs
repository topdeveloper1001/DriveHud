//-----------------------------------------------------------------------
// <copyright file="AppsAppStoreRepository.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
                WindowIconSource = "pack://application:,,,/images/xray.ico",
                ModuleName = CustomModulesNames.PlayerXRay,
                WindowWidth = 1016,
                WindowHeight = 789
            };

            var pkCatcherApp = new AppStoreModule
            {
                ProductName = "PK HUD Catcher",
                ProductDescription = "The PK HUD catcher is an add-on application that will allow you to capture and run a HUD through DriveHUD. It includes a fully functional 7 day trial. Click Try to enable the trial.",
                ImageLink = "pack://application:,,,/DriveHUD.Common.Resources;component/images/Shop/pk-hud-catcher-logo.png",
                WindowIconSource = "pack://application:,,,/images/pk-icon.ico",
                ModuleName = CustomModulesNames.PKCatcher,
                WindowWidth = 556,
                WindowHeight = 429
            };

            return new[] { playerXRayApp, pkCatcherApp };
        }
    }
}