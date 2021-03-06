﻿//-----------------------------------------------------------------------
// <copyright file="PKImporterHelper.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.PokerKing
{
    internal class PKImporterHelper
    {
        public static bool IsPortMatch(int port)
        {
            return port == 31001 || port == 38001 || IsFastFoldPort(port);
        }

        public static bool IsFastFoldPort(int port)
        {
            return fasfFoldPorts.Contains(port);
        }

        private static readonly HashSet<int> fasfFoldPorts = new HashSet<int>(Enumerable.Range(0, 10).Select(x => 35001 + x * 100));
    }
}