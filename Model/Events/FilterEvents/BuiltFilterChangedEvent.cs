//-----------------------------------------------------------------------
// <copyright file="BuiltFilterChangedEventArgs.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Enums;
using Model.Filters;
using Prism.Events;
using System;
using System.Linq.Expressions;

namespace Model.Events
{
    public class BuiltFilterChangedEventArgs : EventArgs
    {
        public BuiltFilterModel BuiltFilter { get; set; }

        public Expression<Func<Playerstatistic, bool>> Predicate { get; set; }

        public EnumFilterType[] AffectedFilter { get; set; }

        public BuiltFilterChangedEventArgs(BuiltFilterModel builtFilter, Expression<Func<Playerstatistic, bool>> predicate)
        {
            BuiltFilter = builtFilter;
            Predicate = predicate;
        }
    }

    public class BuiltFilterChangedEvent : PubSubEvent<BuiltFilterChangedEventArgs>
    {
    }

    public class BuiltFilterRefreshEvent : PubSubEvent<BuiltFilterChangedEventArgs>
    {
    }
}