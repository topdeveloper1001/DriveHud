//-----------------------------------------------------------------------
// <copyright file="FilterAdvancedModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Enums;

namespace Model.Filters
{
    [Serializable]
    public class FilterAdvancedModel : FilterBaseEntity, IFilterModel
    {
        public FilterAdvancedModel()
        {
            Name = "Advanced";
            Type = EnumFilterModelType.FilterAdvancedModel;
        }

        #region Properties 

        public EnumFilterModelType Type { get; }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();

            return predicate;
        }

        public void Initialize()
        {
        }

        public void LoadFilter(IFilterModel filter)
        {
        }

        public void ResetFilter()
        {
        }

        #endregion
    }
}