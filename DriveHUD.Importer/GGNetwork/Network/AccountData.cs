//-----------------------------------------------------------------------
// <copyright file="AccountData.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.Importers.GGNetwork.Network
{
    public class AccountData
    {
        public AccountInfo AccountInfo { get; set; }

        public IList<object> Avatars { get; set; }

        public DateTime CurrentServerTime { get; set; }

        public string ReconnectKey { get; set; }

        public int Result { get; set; }
    }
}