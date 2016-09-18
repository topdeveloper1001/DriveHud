//-----------------------------------------------------------------------
// <copyright file="HandhistoryrecordMap.cs" company="Ace Poker Solutions">
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
using FluentNHibernate.Mapping;

namespace Model.Mapping
{
    public partial class HandhistoryrecordMap : ClassMap<HandHistoryRecord>
    {
        public HandhistoryrecordMap()
        {
            Table("\"HandRecords\"");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Sequence("\"HandRecords_HandRecordId_seq\"").Column("\"HandRecordId\"");
            Map(x => x.Time).Column("\"HandRecordTimestamp\"").Not.Nullable();
            Map(x => x.Line).Column("\"Line\"");
            Map(x => x.Board).Column("\"Board\"");
            Map(x => x.Cards).Column("\"Cards\"");
            Map(x => x.NetWonInCents).Column("\"NetWon\"").Not.Nullable();
            Map(x => x.Pos).Column("\"Position\"");
            Map(x => x.Action).Column("\"Action\"");
            Map(x => x.Allin).Column("\"AllIn\"");
            Map(x => x.Equity).Column("\"Equity\"");
            Map(x => x.EquityDiff).Column("\"EquityDiff\"");
            Map(x => x.BBinCents).Column("\"BBWon\"").Not.Nullable();
            References(x => x.Player).Column("\"PlayerId\"");
            References(x => x.GameType).Column("\"GameInfoId\"");
        }
    }
}