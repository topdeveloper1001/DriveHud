using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Entities;
using DriveHUD.Importers;
using HandHistories.Objects.Players;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Enums;
using NUnit.Framework;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DriveHud.Tests.FunctionalTests.Hud
{
    [TestFixture, RequiresSTA]
    public class HudPositionTests
    {
        IUnityContainer container;

        [SetUp]
        public void SetUp()
        {
            container = new UnityContainer();

            container.RegisterType<SingletonStorageModel>();

            container.RegisterType<IHudElementViewModelCreator, HudElementViewModelCreator>();
            container.RegisterType<IHudPanelService, BovadaHudPanelService>();
            container.RegisterType<IHudLayoutsService, HudLayoutsServiceStub>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImporterService, ImporterService>();
            container.RegisterType<ITableConfigurator, BovadaRichTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.Ignition, HudType.Default));
            container.RegisterType<ITableConfigurator, BovadaTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.Ignition, HudType.Plain));
            container.RegisterType<ITableConfigurator, CommonTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.Unknown, HudType.Plain));
            container.RegisterType<IEventAggregator, EventAggregator>();

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        //[Test]
        //public async void BuildHudTest()
        //{
        //    var bovadaProcess = Process.GetProcessesByName("BovadaWindow").FirstOrDefault();

        //    Assert.That(bovadaProcess, Is.Not.Null);

        //    var maxSeats = 9;

        //    var hudLayoutsService = container.Resolve<IHudLayoutsService>();

        //    var ht = new HudLayout
        //    {
        //        WindowId = bovadaProcess.MainWindowHandle.ToInt32(),
        //        HudType = HudType.Default,
        //        TableType = (EnumTableType)maxSeats
        //    };

        //    var hudViewModel = new HudViewModel();

        //    hudViewModel.CurrentTableLayout = new HudTableLayout
        //    {
        //        TableType = (EnumTableType)maxSeats,
        //        Site = EnumPokerSites.Bovada
        //    };

        //    var players = new PlayerList();

        //    for (var i = 1; i <= maxSeats; i++)
        //    {
        //        players.Add(new Player(string.Format("P{0}", i), 100, i));
        //    }

        //    var tableKey = HudViewModel.GetHash(hudViewModel.CurrentTableLayout, EnumGameType.CashHoldem);

        //    for (int i = 1; i <= maxSeats; i++)
        //    {
        //        var playerName = string.Empty;
        //        var seatNumber = 0;

        //        foreach (var player in players)
        //        {
        //            if (player.SeatNumber == i)
        //            {
        //                if (!string.IsNullOrEmpty(player.PlayerName))
        //                {
        //                    playerName = player.PlayerName;
        //                    seatNumber = player.SeatNumber;
        //                }

        //                break;
        //            }
        //        }

        //        if (string.IsNullOrEmpty(playerName))
        //        {
        //            continue;
        //        }

        //        var playerHudContent = new PlayerHudContent
        //        {
        //            Name = playerName,
        //            SeatNumber = seatNumber
        //        };

        //        var hudElementCreator = container.Resolve<IHudElementViewModelCreator>();

        //        playerHudContent.HudElement = hudElementCreator.Create(tableKey, hudViewModel, seatNumber, HudType.Plain);

        //        if (playerHudContent.HudElement == null)
        //        {
        //            continue;
        //        }

        //        //playerHudContent.HudElement.StatInfoCollection.Add(statInfo);

        //    }

        //    var hudElements = ht.ListHUDPlayer.Select(x => x.HudElement).ToArray();
        //    hudLayoutsService.SetPlayerTypeIcon(hudElements, tableKey);

        //    ht.TableHud = hudViewModel.HudTableViewModelDictionary[tableKey];

        //    HudPainter.UpdateHud(ht);

        //    await Task.Delay(10000);

        //    HudPainter.ReleaseHook();
        //}

        private class HudLayoutsServiceStub : HudLayoutsService
        {
            protected override void Initialize()
            {
                hudLayouts = new HudSavedLayouts();
            }

            public override string GetImageLink(string image)
            {
                return image;
            }
        }
    }
}
