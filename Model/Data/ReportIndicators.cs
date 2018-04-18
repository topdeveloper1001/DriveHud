//-----------------------------------------------------------------------
// <copyright file="ReportIndicators.cs" company="Ace Poker Solutions">
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
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Data
{
    [ProtoContract]
    [ProtoInclude(100, typeof(OpponentReportIndicators))]
    public class ReportIndicators : LightIndicators
    {
        public ReportIndicators() : base()
        {
        }

        public ReportIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
        }

        [ProtoMember(1)]
        public ObservableCollection<ReportHandViewModel> ReportHands { get; protected set; } = new ObservableCollection<ReportHandViewModel>();

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);

            AddStatToStatistic(statistic);
            AddReportHand(statistic);
        }

        public override void Clean()
        {
            base.Clean();
            ReportHands.Clear();
        }

        public virtual void AddIndicator(ReportIndicators indicator)
        {
            if (indicator == null)
            {
                throw new ArgumentNullException(nameof(indicator));
            }

            base.AddIndicator(indicator);

            ReportHands?.AddRange(indicator.ReportHands);
            Statistics?.AddRange(indicator.Statistics);
        }

        protected virtual void AddStatToStatistic(Playerstatistic statistic)
        {
            Statistics.Add(statistic);
        }

        protected virtual void AddReportHand(Playerstatistic statistic)
        {
            var reportHandViewModel = new ReportHandViewModel(statistic);
            ReportHands.Add(reportHandViewModel);
        }
    }
}