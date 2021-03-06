//-----------------------------------------------------------------------
// <copyright file="PKCatcher.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Infrastructure.Modules;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.PKCatcher.Licensing;
using DriveHUD.PKCatcher.Services;
using DriveHUD.PKCatcher.Settings;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using PKCatcher.Common.Exceptions;
using PKCatcher.Security;
using System;
using System.IO;
using System.Reflection;

namespace DriveHUD.PKCatcher
{
    public class PKCatcherModule : IDHModule
    {
        private const string binDirectory = "bin";

        private static readonly FileResourceManager resourceManager = new FileResourceManager("PKC_", "DriveHUD.PKCatcher.Resources.Common", Assembly.GetAssembly(typeof(PKCatcherModule)));

        internal bool IsLicenseValid { get; private set; }

        public void ConfigureContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            LogProvider.Log.Info(CustomModulesNames.PKCatcher,
                string.Format("---------------=============== Initialize PK HUD Catcher (v.{0}) ===============---------------",
                Assembly.GetAssembly(typeof(PKCatcherModule)).GetName().Version));

#if !DEBUG
            ValidateLicenseAssemblies();
#endif
            CommonResourceManager.Instance.RegisterResourceManager(resourceManager);

            container.RegisterType<ILicenseService, LicenseService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModelContainer, PKCatcherView>(CustomModulesNames.PKCatcher);
            container.RegisterType<IPKCatcherViewModel, PKCatcherViewModel>();
            container.RegisterType<IPKSettingsService, PKSettingsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPKCatcherService, PKCatcherService>(new ContainerControlledLifetimeManager());

            container.RegisterType<ILicenseManager, PKTReg>(LicenseType.PKCTrial.ToString());
            container.RegisterType<ILicenseManager, PKNReg>(LicenseType.PKCNormal.ToString());

            InitalizeLicenseService();
        }

        private void ValidateLicenseAssemblies()
        {
            var assemblies = new string[] { "DeployLX.Licensing.v5.dll", "PKTReg.dll", "PKNReg.dll" };
            var assembliesHashes = new string[] { "c1d67b8e8d38540630872e9d4e44450ce2944700", "223951b8089e4b9952dd8a6cd3922f0a288b9911", "6318616c3ebb085e34ca612d5d8e5a89641f7ea7" };
            var assemblySizes = new int[] { 1032192, 51032, 52056 };

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyInfo = new FileInfo(assemblies[i]);

                if (!assemblyInfo.Exists && assemblies[i].StartsWith("DeployLX", StringComparison.OrdinalIgnoreCase))
                {
                    assemblyInfo = new FileInfo(Path.Combine(binDirectory, assemblies[i]));
                }

                var isValid = SecurityUtils.ValidateFileHash(assemblyInfo.FullName, assembliesHashes[i]) && assemblyInfo.Length == assemblySizes[i];

                if (!isValid)
                {
                    LogProvider.Log.Error(CustomModulesNames.PKCatcher, "Module is invalid");
                    throw new PKInternalException(new NonLocalizableString("PKCatcher module is invalid, so it cannot be initialized"));
                }
            }
        }

        private void InitalizeLicenseService()
        {
            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            IsLicenseValid = licenseService.Validate();
        }
    }
}
