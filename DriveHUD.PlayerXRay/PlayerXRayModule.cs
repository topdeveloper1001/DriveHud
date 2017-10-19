//-----------------------------------------------------------------------
// <copyright file="PlayerXRayModule.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Infrastructure.Modules;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.PlayerXRay.Licensing;
using DriveHUD.PlayerXRay.Security;
using DriveHUD.PlayerXRay.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using System;
using System.IO;
using System.Reflection;

namespace DriveHUD.PlayerXRay
{
    public class PlayerXRayModule : IDHModule
    {
        private const string binDirectory = "bin";

        static readonly FileResourceManager resourceManager = new FileResourceManager("XRay_", "DriveHUD.PlayerXRay.Resources.Common", Assembly.GetAssembly(typeof(PlayerXRayMainViewModel)));

        internal bool IsLicenseValid { get; private set; }

        public void ConfigureContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            LogProvider.Log.Info(CustomModulesNames.PlayerXRay,
                string.Format("---------------=============== Initialize PlayerXRay (v.{0}) ===============---------------",
                Assembly.GetAssembly(typeof(PlayerXRayMainViewModel)).GetName().Version));

#if !DEBUG
            ValidateLicenseAssemblies();
#endif

            CommonResourceManager.Instance.RegisterResourceManager(resourceManager);

            container.RegisterType<ILicenseService, LicenseService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPlayerNotesService, PlayerXRayNoteService>(new ContainerControlledLifetimeManager());
            container.RegisterType<INoteProcessingService, NoteProcessingService>();
            container.RegisterType<IModuleEntryViewModel, PlayerXRayMainViewModel>(CustomModulesNames.PlayerXRay);
            container.RegisterType<IViewContainer, PlayerXRayMainView>(CustomModulesNames.PlayerXRay);

            container.RegisterType<ILicenseManager, XRTReg>(LicenseType.Trial.ToString());
            container.RegisterType<ILicenseManager, XRHReg>(LicenseType.Holdem.ToString());
            container.RegisterType<ILicenseManager, XROReg>(LicenseType.Omaha.ToString());
            container.RegisterType<ILicenseManager, XRCReg>(LicenseType.Combo.ToString());

            InitalizeLicenseService();
        }

        private void ValidateLicenseAssemblies()
        {
            var assemblies = new string[] { "DeployLX.Licensing.v5.dll", "XRCReg.dll", "XRReg.dll", "XROReg.dll" };
            var assembliesHashes = new string[] { "c1d67b8e8d38540630872e9d4e44450ce2944700", "41eefbf5455fc80e9f56fa7495f2d1a4e0d30a52", "af7320210803634d0cc182face213af077670f2c", "f157a0fed2ed9707a1e8e84f239068e35e907d7b" };
            var assemblySizes = new int[] { 1032192, 53592, 54616, 53592 };

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyInfo = new FileInfo(assemblies[i]);

                if (!assemblyInfo.Exists)
                {
                    assemblyInfo = new FileInfo(Path.Combine(binDirectory, assemblies[i]));
                }

                var isValid = SecurityUtils.ValidateFileHash(assemblyInfo.FullName, assembliesHashes[i]) && assemblyInfo.Length == assemblySizes[i];

                if (!isValid)
                {
                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Module is invalid");
                    throw new DHInternalException(new NonLocalizableString("PlayerXRay module is invalid, so it cannot be initialized"));
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