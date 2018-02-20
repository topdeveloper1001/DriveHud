//-----------------------------------------------------------------------
// <copyright file="CashGraphSettings.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using ProtoBuf;

namespace DriveHUD.Application.ViewModels.Graphs
{
    [ProtoContract]
    public class CashGraphSettings
    {
        [ProtoMember(1)]
        public ChartDisplayRange ChartDisplayRange { get; set; }

        [ProtoMember(2)]
        public bool ShowShowdown { get; set; }

        [ProtoMember(3)]
        public bool ShowNonShowdown { get; set; }

        [ProtoMember(4)]
        public bool ShowEV { get; set; }

        [ProtoMember(5)]
        public ChartCashSeriesValueType ValueType { get; set; }
    }
}