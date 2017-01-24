namespace DriveHud.Tests.UnitTests
{
    using DriveHUD.Application.MigrationService.Migrations;
    using DriveHUD.Application.ViewModels;
    using DriveHUD.Application.ViewModels.Hud;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using Model;

    using NUnit.Framework;

    [TestFixture]
    public class HudMigrationTests
    {
        IUnityContainer container;

        private Migration0014_LayoutsToMultipleFiles migration0014;

        [SetUp]
        public void SetUp()
        {
            container = new UnityContainer();

            container.RegisterType<SingletonStorageModel>();

            container.RegisterType<IHudLayoutsService,HudLayoutsService>(new ContainerControlledLifetimeManager());

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
            migration0014 = new Migration0014_LayoutsToMultipleFiles();
        }

        [Test]
        public void Test1()
        {
            migration0014.Up();
        }
    }
}
