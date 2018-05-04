﻿//-----------------------------------------------------------------------
// <copyright file="RaiseShortcutsOptions.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;

namespace DriveHUD.Importers.PokerMaster.Model
{
    [ProtoContract]
    internal class RaiseShortcutsOptions
    {
        [ProtoMember(1)]
        public bool FixedRatio { get; set; }

        [ProtoMember(2)]
        public int OptionIndex { get; set; }

        [ProtoMember(3)]
        public int RatioUp { get; set; }

        [ProtoMember(4)]
        public int RatioDown { get; set; }

        [ProtoMember(5)]
        public long Customized { get; set; }
    }
}