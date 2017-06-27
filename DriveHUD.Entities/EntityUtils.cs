//-----------------------------------------------------------------------
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
                .Where(p => p != EnumPokerSites.Unknown && p != EnumPokerSites.IPoker && p != EnumPokerSites.Bovada)
                .ToArray();

            return supportedPokerSites;
        }
    }
}