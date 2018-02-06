//-----------------------------------------------------------------------
// <copyright file="OpponentReportCache.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Data;
using ProtoBuf;
using System.Collections.Generic;

namespace Model.Reports
{
    [ProtoContract]
    public class OpponentReportCache
    {
        [ProtoMember(1)]
        public int[] PlayerIds { get; set; }

        [ProtoMember(2)]
        public List<OpponentReportIndicators> Report { get; set; }
    }
}