//-----------------------------------------------------------------------
// <copyright file="UnityServicesBootstrapper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.TableConfigurators.PositionProviders;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Entities;
using Microsoft.Practices.Unity;
using System;

namespace DriveHUD.Application
{
    /// <summary>
    /// Importer bootstrapper to allow us to make all interfaces and all classes internal
    /// </summary>
    public static class UnityServicesBootstrapper
    {
        public static void ConfigureContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            // Panel services
            container.RegisterType<IHudPanelService, HudPanelService>();
            container.RegisterType<IHudPanelService, HudPanelService>(EnumPokerSites.Ignition.ToString());
            container.RegisterType<IHudPanelService, HudPanelService>(EnumPokerSites.Bodog.ToString());
            container.RegisterType<IHudPanelService, BetOnlineHudPanelService>(EnumPokerSites.BetOnline.ToString());
            container.RegisterType<IHudPanelService, BetOnlineHudPanelService>(EnumPokerSites.SportsBetting.ToString());
            container.RegisterType<IHudPanelService, BetOnlineHudPanelService>(EnumPokerSites.TigerGaming.ToString());
            container.RegisterType<IHudPanelService, PokerStarsHudPanelService>(EnumPokerSites.PokerStars.ToString());
            container.RegisterType<IHudPanelService, Poker888HudPanelService>(EnumPokerSites.Poker888.ToString());
            container.RegisterType<IHudPanelService, WinningPokerNetworkHudPanelService>(EnumPokerSites.AmericasCardroom.ToString());
            container.RegisterType<IHudPanelService, WinningPokerNetworkHudPanelService>(EnumPokerSites.BlackChipPoker.ToString());
            container.RegisterType<IHudPanelService, WinningPokerNetworkHudPanelService>(EnumPokerSites.TruePoker.ToString());
            container.RegisterType<IHudPanelService, WinningPokerNetworkHudPanelService>(EnumPokerSites.YaPoker.ToString());
            container.RegisterType<IHudPanelService, WinningPokerNetworkHudPanelService>(EnumPokerSites.WinningPokerNetwork.ToString());

            // Position Providers
            container.RegisterType<IPositionProvider, CommonPositionProvider>(EnumPokerSites.Unknown.ToString());
            container.RegisterType<IPositionProvider, CommonPositionProvider>(EnumPokerSites.BetOnline.ToString());
            container.RegisterType<IPositionProvider, CommonPositionProvider>(EnumPokerSites.TigerGaming.ToString());
            container.RegisterType<IPositionProvider, CommonPositionProvider>(EnumPokerSites.SportsBetting.ToString());
            container.RegisterType<IPositionProvider, IgnitionPositionProvider>(EnumPokerSites.Ignition.ToString());
            container.RegisterType<IPositionProvider, IgnitionPositionProvider>(EnumPokerSites.Bodog.ToString());
            container.RegisterType<IPositionProvider, Poker888PositionProvider>(EnumPokerSites.Poker888.ToString());
            container.RegisterType<IPositionProvider, PokerStarsPositionProvider>(EnumPokerSites.PokerStars.ToString());
            container.RegisterType<IPositionProvider, WinningPositionProvider>(EnumPokerSites.WinningPokerNetwork.ToString());
            container.RegisterType<IPositionProvider, WinningPositionProvider>(EnumPokerSites.AmericasCardroom.ToString());
            container.RegisterType<IPositionProvider, WinningPositionProvider>(EnumPokerSites.BlackChipPoker.ToString());
            container.RegisterType<IPositionProvider, WinningPositionProvider>(EnumPokerSites.TruePoker.ToString());
            container.RegisterType<IPositionProvider, WinningPositionProvider>(EnumPokerSites.YaPoker.ToString());
        }
    }
}