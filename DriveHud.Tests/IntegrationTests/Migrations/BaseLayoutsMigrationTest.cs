//-----------------------------------------------------------------------
// <copyright file="LayoutsMigrationTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Application;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Resources;
using Microsoft.Practices.Unity;

namespace DriveHud.Tests.IntegrationTests.Migrations
{
    class BaseLayoutsMigrationTest : BaseIntegrationTest
    {
        private const string testDataFolder = @"..\..\IntegrationTests\Migrations\TestData";

        protected override string TestDataFolder
        {
            get
            {
                return testDataFolder;
            }
        }

        protected override void InitializeContainer(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<IHudLayoutsService, HudLayoutsService>();
            UnityServicesBootstrapper.ConfigureContainer(unityContainer);
            ResourceRegistrator.Initialization();
        }
    }
}