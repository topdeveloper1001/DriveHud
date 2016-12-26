using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.DBMigration
{
    public class Bootstrapper
    {
        public IUnityContainer UnityContainer { get; private set; }

        public void Run()
        {
            ResourceRegistrator.Initialization();

            UnityContainer = new UnityContainer();

            UnityContainer.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));

            var locator = new UnityServiceLocator(UnityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}
