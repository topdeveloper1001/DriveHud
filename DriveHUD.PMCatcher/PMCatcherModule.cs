//-----------------------------------------------------------------------
// <copyright file="PMCatcherModule.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.PMCatcher.Licensing;
using DriveHUD.PMCatcher.Services;
using DriveHUD.PMCatcher.Settings;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using PMCatcher.Common.Exceptions;
using PMCatcher.Security;
using System;
using System.IO;
using System.Reflection;

namespace DriveHUD.PMCatcher
{
    public class PMCatcherModule : IDHModule
    {
        private const string binDirectory = "bin";

        private static readonly FileResourceManager resourceManager = new FileResourceManager("PMC_", "DriveHUD.PMCatcher.Resources.Common", Assembly.GetAssembly(typeof(PMCatcherModule)));

        internal bool IsLicenseValid { get; private set; }

        public void ConfigureContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            LogProvider.Log.Info(CustomModulesNames.PMCatcher,
                string.Format("---------------=============== Initialize PM HUD Catcher (v.{0}) ===============---------------",
                Assembly.GetAssembly(typeof(PMCatcherModule)).GetName().Version));

#if !DEBUG
            ValidateLicenseAssemblies();
#endif
            CommonResourceManager.Instance.RegisterResourceManager(resourceManager);

            container.RegisterType<ILicenseService, LicenseService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IViewModelContainer, PMCatcherView>(CustomModulesNames.PMCatcher);
            container.RegisterType<IPMCatcherViewModel, PMCatcherViewModel>();
            container.RegisterType<IPMSettingsService, PMSettingsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPMCatcherService, PMCatcherService>(new ContainerControlledLifetimeManager());

            container.RegisterType<ILicenseManager, PMTReg>(LicenseType.PMCTrial.ToString());
            container.RegisterType<ILicenseManager, PMHReg>(LicenseType.PMCHoldem.ToString());
            container.RegisterType<ILicenseManager, PMOReg>(LicenseType.PMCOmaha.ToString());
            container.RegisterType<ILicenseManager, PMCReg>(LicenseType.PMCCombo.ToString());

            InitalizeLicenseService();
        }

        private void ValidateLicenseAssemblies()
        {
            var assemblies = new string[] { "DeployLX.Licensing.v5.dll", "PMTReg.dll", "PMCReg.dll", "PMHReg.dll", "PMOReg.dll" };
            var assembliesHashes = new string[] { "c1d67b8e8d38540630872e9d4e44450ce2944700", "5cb016092ac8801635c8e512a0dd53a807cb1afd", "458f07ccf7263d3d2a7311c1181596ba7b200116", "e5fcdb720730f610d4e1210212bd51bd85bc6ed0", "1c04138ca4f51cf6ff62678b43b4d37d4284f4a8" };
            var assemblySizes = new int[] { 1032192, 44032, 42496, 42496, 43008 };

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
                    LogProvider.Log.Error(CustomModulesNames.PMCatcher, "Module is invalid");
                    throw new PMInternalException(new NonLocalizableString("PMCatcher module is invalid, so it cannot be initialized"));
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