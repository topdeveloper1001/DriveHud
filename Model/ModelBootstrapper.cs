﻿//-----------------------------------------------------------------------
// <copyright file="ModelBootstrapper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.Unity;
using Model.AppStore;
using Model.AppStore.HudStore;
using Model.Enums;
using Model.Export;
using Model.Filters;
using Model.Hud;
using Model.Reports;
using Model.Solvers;
using System;

namespace Model
{
    /// <summary> 
    /// Model bootstrapper to allow us to make all interfaces and all classes internal (need for obfuscation)
    /// </summary>
    public static class ModelBootstrapper
    {
        public static void ConfigureContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.RegisterType<IReportStatusService, ReportStatusService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPlayerStatisticRepository, PlayerStatisticRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFilterDataService, FilterDataService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));
            container.RegisterType<IFilterModelManagerService, MainFilterModelManagerService>(FilterServices.Main.ToString(), new ContainerControlledLifetimeManager());
            container.RegisterType<IFilterModelManagerService, StickersFilterModelManagerService>(FilterServices.Stickers.ToString(), new ContainerControlledLifetimeManager());
            container.RegisterType<ISessionFactoryService, SessionFactoryService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IProductAppStoreRepository, ProductAppStoreRepository>();
            container.RegisterType<IProductAppStoreModel, ProductAppStoreModel>();
            container.RegisterType<ITrainingAppStoreRepository, TrainingAppStoreRepository>();
            container.RegisterType<ITrainingAppStoreModel, TrainingAppStoreModel>();
            container.RegisterType<IAppsAppStoreRepository, AppsAppStoreRepository>();
            container.RegisterType<IAppsAppStoreModel, AppsAppStoreModel>();
            container.RegisterType<IRakebackAppStoreRepository, RakebackAppStoreRepository>();
            container.RegisterType<IRakebackAppStoreModel, RakebackAppStoreModel>();
            container.RegisterType<IOpponentReportService, OpponentReportService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPopulationReportService, PopulationReportService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IHudPlayerTypeService, HudPlayerTypeService>();
            container.RegisterType<IHudBumperStickerService, HudBumperStickerService>();
            container.RegisterType<IEquitySolver, EquitySolver>();
            container.RegisterType<IHudStoreUploadModel, HudStoreUploadModel>();
            container.RegisterType<IHudStoreWebService, HudStoreWebService>();
            container.RegisterType<IHudStoreModel, HudStoreModel>();
            container.RegisterType<IPlayerStatisticCalculator, PlayerStatisticCalculator>(PlayerStatisticCalculatorServiceNames.Base);
            container.RegisterType<IPlayerStatisticCalculator, PKShortPlayerStatisticCalculator>(PlayerStatisticCalculatorServiceNames.PKShortDeck);
            container.RegisterType<IHandExportService, HandExportService>();
            container.RegisterType<IHandNoteCacheService, HandNoteCacheService>(new ContainerControlledLifetimeManager());

            #region Export services

            container.RegisterType<IHandExportPreparingService, CommonHandExportPreparingService>(HandExportPreparingServiceProvider.Common);
            container.RegisterType<IHandExportPreparingService, WPNHandExportPreparingService>(HandExportPreparingServiceProvider.WPN);
            container.RegisterType<IHandExportPreparingService, DHBasedHandExportPreparingService>(HandExportPreparingServiceProvider.DriveHUD);
            container.RegisterType<IHandExportPreparingService, IPokerHandExportPreparingService>(HandExportPreparingServiceProvider.CustomIPoker);

            #endregion
        }
    }
}