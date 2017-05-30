//-----------------------------------------------------------------------
// <copyright file="StatInfoBase.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using DriveHUD.Entities;
using Model.Data;
using Model.Enums;
using ProtoBuf;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Model.Stats
{
    [ProtoContract]
    [ProtoInclude(100, typeof(StatInfo))]
    public class StatInfoBase : INotifyPropertyChanged
    {
        private Stat stat;

        [ProtoMember(1)]
        public virtual Stat Stat
        {
            get
            {
                return stat;
            }
            set
            {
                if (value == stat)
                {
                    return;
                }

                stat = value;

                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public virtual Expression<Func<Playerstatistic, StatDto>> GetStatDtoExpression { get; set; }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}