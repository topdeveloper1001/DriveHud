//-----------------------------------------------------------------------
// <copyright file="StatInfoBreak.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using System;

namespace Model.Stats
{
    [ProtoContract]
    [Serializable]
    public class StatInfoBreak : StatInfo
    {
        public StatInfoBreak() : base()
        {
            base.Stat = Stat.LineBreak;
        }

        public override Stat Stat
        {
            get
            {
                return base.Stat;
            }
            set
            {
            }
        }

        public override StatInfo Clone()
        {
            return new StatInfoBreak();
        }
    }
}