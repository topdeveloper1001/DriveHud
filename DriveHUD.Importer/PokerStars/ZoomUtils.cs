//-----------------------------------------------------------------------
// <copyright file="ZoomUtils.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System;
using System.Linq;

namespace DriveHUD.Importers.PokerStars
{
    /// <summary>
    /// Utilities for Zoom mode of PS
    /// </summary>
    internal class ZoomUtils
    {
        public static GameFormat? ParseGameFormatFromTitle(string title)
        {
            var tableEndStartIndex = title.IndexOf("#", StringComparison.OrdinalIgnoreCase) - 1;

            if (tableEndStartIndex <= 0)
            {
                tableEndStartIndex = title.IndexOf("-", StringComparison.OrdinalIgnoreCase) - 1;

                if (tableEndStartIndex <= 0)
                {
                    return null;
                }
            }

            var tableName = title.Substring(0, tableEndStartIndex);

            if (IsZoomTable(tableName))
            {
                return GameFormat.Zoom;
            }
            else if (!title.Contains("Tournament"))
            {
                return GameFormat.Cash;
            }

            return null;
        }

        public static bool IsZoomTable(string tableName)
        {
            return ZoomTables.Contains(tableName);
        }

        private static readonly string[] ZoomTables = new[] { "McNaught", "Borrelllly", "Halley", "Lovejoy", "Hyakutake", "Donati", "Lynx", "Hartley", "Aludra", "Devanssay",
            "Eulalia", "Nansen", "Amundsen", "Whirlpool", "Hydra", "Thyestes", "Arp", "Baade", "Aquarius Dwarf", "Serpens Caput", "Triangulum", "Gotha", "Aenaa", "Diotima",
            "Lambda Velorum", "Humason", "Centaurus", "Dorado", "Lupus", "Coma Berenices", "Cassiopeia", "Perseus", "C Carinae", "Alpha Reticuli (CAP)", "Chi Sagittarii",
            "Sirius", "Omicron Capricorni", "Beta Tucanae (CAP)", "Delta Antilae", "Theta Cancri", "Chi Draconis", "Sigam Aquilae (CAP)", "Iota Apodis", "Zeta Phoenicis",
            "Delta Boötis", "Gamma Delphini (CAP)", "Phi Piscium", "Tau Hydrae", "Adhara", "Iota Cancri (CAP)", "Deneb el Okab", "Lambda Arietis", "Cetus", "Mira (CAP)",
            "Crux", "Rho Capricorni", "Gamma Crateris", "Alpha Crucis (CAP)", "Norma", "Canes Venatici", "Draco", "Amália", "Eusébio", "Pessoa", "Cervantes", "Velazquez",
            "Gaudi", "Dali", "Goya", "Picasso", "Clubs", "Spades", "Hears", "Diamonds", "Turn", "River", "Portland", "Los Angeles", "Houston", "New York", "Las Vegas",
            "Boston", "Boulder", "Washington", "Dallas", "New Orleans", "Miami", "Antares", "Atena", "Fenice", "Pegaso", "Cigno", "Shun", "Sirio", "Cronos" };
    }
}