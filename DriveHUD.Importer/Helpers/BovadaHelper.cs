//-----------------------------------------------------------------------
// <copyright file="BovadaHelper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.Importers.Bovada;

namespace DriveHUD.Importers.Helpers
{
    /// <summary>
    /// Helper for bovada importer
    /// </summary>
    internal class BovadaHelper
    {
        public static EnumGameType ConvertToEnumGameType(GameType gameType, GameFormat gameFormat)
        {
            if (gameFormat == GameFormat.SnG)
            {
                if (gameType == GameType.Holdem)
                {
                    return EnumGameType.SNGHoldem;
                }
                else
                {
                    return EnumGameType.SNGOmaha;
                }
            }

            if (gameFormat == GameFormat.MTT)
            {
                if (gameType == GameType.Holdem)
                {
                    return EnumGameType.MTTHoldem;
                }
                else
                {
                    return EnumGameType.MTTOmaha;
                }
            }

            if (gameType == GameType.Omaha || gameType == GameType.OmahaHiLo)
            {
                return EnumGameType.CashOmaha;
            }

            return EnumGameType.CashHoldem;
        }

        internal static bool IsZonePoker(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }

            return tableName.ToUpper().Contains("ZONE POKER");
        }
    }
}