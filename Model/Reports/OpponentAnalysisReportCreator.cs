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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Model.Reports
{
    public class OpponentAnalysisReportCreator : CashBaseReportCreator<ReportIndicators>
    {
        public override ObservableCollection<ReportIndicators> Create(List<Playerstatistic> statistics, CancellationToken cancellationToken, bool forceRefresh = false)
        {
            var opponentReportService = ServiceLocator.Current.GetInstance<IOpponentReportService>();

            var report = opponentReportService.GetReport(cancellationToken);

            return report != null ?
                new ObservableCollection<ReportIndicators>(report) :
                new ObservableCollection<ReportIndicators>();
        }

        protected override List<ReportIndicators> CombineChunkedIndicators(BlockingCollection<ReportIndicators> chunkedIndicators, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<ReportIndicators> chunkedIndicators, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}