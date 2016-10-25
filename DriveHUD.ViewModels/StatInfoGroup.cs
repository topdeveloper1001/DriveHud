//-----------------------------------------------------------------------
// <copyright file="StatInfoGroup.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using ReactiveUI;
using ProtoBuf;

namespace DriveHUD.ViewModels
{
    /// <summary>
    /// Class of group of stats
    /// </summary>    
    [Serializable, ProtoContract]
    public class StatInfoGroup : ReactiveObject
    {
        private string name;

        [ProtoMember(1)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}