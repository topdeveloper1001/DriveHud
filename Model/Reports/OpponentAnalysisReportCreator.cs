//-----------------------------------------------------------------------
// <copyright file="OpponentAnalysisReportCreator.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Reports
{
    public class OpponentAnalysisReportCreator : CashBaseReportCreator
    {
        public override ObservableCollection<ReportIndicators> Create(IList<Playerstatistic> statistics, bool forceRefresh = false)
        {
            var opponentReportService = ServiceLocator.Current.GetInstance<IOpponentReportService>();

            var report = opponentReportService.GetReport();

            return report != null ?
                new ObservableCollection<ReportIndicators>(report) :
                new ObservableCollection<ReportIndicators>();
        }
    }
}