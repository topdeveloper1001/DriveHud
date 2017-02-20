//-----------------------------------------------------------------------
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

            container.RegisterType<ISessionFactoryService, SessionFactoryService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IProductAppStoreRepository, ProductAppStoreRepository>();
            container.RegisterType<IProductAppStoreModel, ProductAppStoreModel>();
        }
    }
}