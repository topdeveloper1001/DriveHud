//-----------------------------------------------------------------------
// <copyright file="PlayerUtils.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.GGNetwork.Model;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.GGNetwork
{
    internal class PlayerUtils
    {
        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pots"></param>
        /// <returns></returns>
        public static bool IsWinsMainPot(Player player, IList<Pot> pots)
        {
            if (player == null)
            {
                return false;
            }

            if (!player.IsWinner)
            {
                return false;
            }

            var mainPotAmount = pots.Max(p => p.Amount);

            foreach (var pot in pots)
            {
                if (pot.Amount != mainPotAmount)
                {
                    continue;
                }

                if (pot.Winners.Contains(player.Id))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pots"></param>
        /// <returns></returns>
        public static bool IsWinsSidePot(Player player, IList<Pot> pots)
        {
            if (player == null)
            {
                return false;
            }

            if (!player.IsWinner)
            {
                return false;
            }

            if (pots.Count <= 1)
            {
                return false;
            }

            int mainPotAmount = pots.Max(p => p.Amount);

            foreach (var pot in pots)
            {
                if (pot.Amount == mainPotAmount)
                {
                    continue;
                }

                if (pot.Winners.Contains(player.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}