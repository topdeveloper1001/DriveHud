//-----------------------------------------------------------------------
// <copyright file="HandIdInfo.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class HandIdInfo
    {
        public int PlayType { get; set; }

        public int GameType { get; set; }

        public int ModeType { get; set; }

        public int SequenceId { get; set; }

        public object HandId { get; set; }

        public string StringHandId { get; set; }
    }
}