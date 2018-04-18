//-----------------------------------------------------------------------
// <copyright file="ModuleService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using DriveHUD.Common.Infrastructure.Modules;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using Microsoft.Practices.Unity;
using Model;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DriveHUD.Application.Modules
{
    internal class ModuleService : IModuleService
    {
        private IUnityContainer container;

        public void InitializeModules([NotNull] IUnityContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));

            LogProvider.Log.Info("Initializing modules.");

            var modules = CustomModulesFactory.GetModules();

            foreach (var module in modules)
            {
                InitializeModule(module);
            }
        }

        private void InitializeModule([NotNull] CustomModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            LogProvider.Log.Info($"Initializing {module.ModuleName} module.");

            var moduleAssemblyPath = module.ModuleAssembly;

            if (!File.Exists(moduleAssemblyPath))
            {
                moduleAssemblyPath = Path.Combine(StringFormatter.GetModulesFolderPath(), module.ModuleAssembly);

                if (!File.Exists(moduleAssemblyPath))
                {
                    LogProvider.Log.Error($"Module {module.ModuleName} has not been found at '{moduleAssemblyPath}'.");
                    return;
                }
            }

            if (!CertificateHelper.Verify(moduleAssemblyPath))
            {
                LogProvider.Log.Error($"Module {module.ModuleName} isn't valid at '{moduleAssemblyPath}'.");
                return;
            }

            try
            {
                var moduleAssembly = Assembly.LoadFrom(moduleAssemblyPath);

                LogProvider.Log.Info($"Module {module.ModuleName} has been loaded.");

                var moduleInterfaceType = typeof(IDHModule);

                var moduleType = moduleAssembly.GetTypes()
                    .FirstOrDefault(t => t.GetInterfaces().Contains(moduleInterfaceType) && t.GetConstructor(Type.EmptyTypes) != null);

                var moduleObject = Activator.CreateInstance(moduleType) as IDHModule;
                moduleObject.ConfigureContainer(container);

                LogProvider.Log.Info($"Module {module.ModuleName} has been configured.");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Module {module.ModuleName} hasn't been loaded at '{moduleAssemblyPath}'.", e);
            }
        }
    }
}