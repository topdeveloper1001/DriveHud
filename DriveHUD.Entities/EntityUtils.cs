﻿//-----------------------------------------------------------------------
// <copyright file="EntityUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Entities
{
    /// <summary>
    /// Defines methods to help to work with entities
    /// </summary>
    public class EntityUtils
    {
        /// <summary>
        /// Gets the list of the supported <see cref="EnumPokerSites"/>
        /// </summary>
        /// <returns>The list of the supported <see cref="EnumPokerSites"/></returns>
        public static IEnumerable<EnumPokerSites> GetSupportedPokerSites()
        {
            var supportedPokerSites = Enum.GetValues(typeof(EnumPokerSites))
                .OfType<EnumPokerSites>()
                .Where(p => p != EnumPokerSites.Unknown && p != EnumPokerSites.Bovada)
                .ToArray();

            return supportedPokerSites;
        }

        /// <summary>
        /// Gets the dictionary of poker networks
        /// </summary>
        /// <returns></returns>
        public static Dictionary<EnumPokerNetworks, EnumPokerSites[]> GetNetworkSites()
        {
            var networksDictionary = new Dictionary<EnumPokerNetworks, EnumPokerSites[]>
            {
                [EnumPokerNetworks.Ignition] = new[] { EnumPokerSites.Ignition, EnumPokerSites.Bodog, EnumPokerSites.Bovada },
                [EnumPokerNetworks.Chico] = new[] { EnumPokerSites.BetOnline, EnumPokerSites.SportsBetting, EnumPokerSites.TigerGaming },
                [EnumPokerNetworks.PokerStars] = new[] { EnumPokerSites.PokerStars },
                [EnumPokerNetworks.PartyPoker] = new[] { EnumPokerSites.PartyPoker },
                [EnumPokerNetworks.WPN] = new[] { EnumPokerSites.AmericasCardroom, EnumPokerSites.BlackChipPoker, EnumPokerSites.TruePoker, EnumPokerSites.YaPoker, EnumPokerSites.WinningPokerNetwork },
                [EnumPokerNetworks.Poker888] = new[] { EnumPokerSites.Poker888 },
                [EnumPokerNetworks.IPoker] = new[] { EnumPokerSites.IPoker },
                [EnumPokerNetworks.GGN] = new[] { EnumPokerSites.GGN },
                [EnumPokerNetworks.Horizon] = new[] { EnumPokerSites.Horizon },
                [EnumPokerNetworks.Winamax] = new[] { EnumPokerSites.Winamax },
                [EnumPokerNetworks.Adda52] = new[] { EnumPokerSites.Adda52 },
                [EnumPokerNetworks.QuadnetIndia] = new[] { EnumPokerSites.SpartanPoker },
                [EnumPokerNetworks.BaaziNetworks] = new[] { EnumPokerSites.PokerBaazi }
            };

            return networksDictionary;
        }

        /// <summary>
        /// Gets the network of the specified site
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public static EnumPokerNetworks GetSiteNetwork(EnumPokerSites site)
        {
            var networks = GetNetworkSites();

            foreach (var network in networks.Keys)
            {
                if (networks[network].Contains(site))
                {
                    return network;
                }
            }

            return EnumPokerNetworks.Unknown;
        }
    }
}