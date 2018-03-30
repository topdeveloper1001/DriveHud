﻿//-----------------------------------------------------------------------
// <copyright file="CashBaseReportCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker CashGroupingReportCreator. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Data;

namespace Model.Reports
{
    public abstract class CashGroupingReportCreator<T, GroupType> : GroupingReportCreator<T, GroupType>
         where T : ReportIndicators
    {
        public override bool IsTournament
        {
            get
            {
                return false;
            }
        }
    }
}