//-----------------------------------------------------------------------
// <copyright file="IPopulationReportService.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;

namespace Model.Reports
{
    /// <summary>
    /// Exposes service to read/update/cache data for population report
    /// </summary>
    public interface IPopulationReportService
    {
        /// <summary>
        /// Gets the population report 
        /// </summary>
        /// <returns>The list of <see cref="Indicators"/></returns>
        IEnumerable<ReportIndicators> GetReport(bool forceRefresh);                       
    }
}