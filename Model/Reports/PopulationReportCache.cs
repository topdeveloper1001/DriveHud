//-----------------------------------------------------------------------
// <copyright file="PopulationReportCache.cs" company="Ace Poker Solutions">
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
using Model.Data;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Model.Reports
{
    [ProtoContract]
    internal class PopulationReportCache
    {
        [ProtoMember(1)]
        public List<PopulationReportIndicators> Report { get; set; }

        [ProtoMember(2)]
        public EnumPokerSites? PokerSite { get; set; }

        [ProtoMember(3)]
        public DateTime StartDate { get; set; }

        [ProtoMember(4)]
        public DateTime EndDate { get; set; }

        [ProtoMember(5)]
        public int PlayersFrom { get; set; }

        [ProtoMember(6)]
        public int PlayersTo { get; set; }
    }
}