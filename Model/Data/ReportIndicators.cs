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

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        [ProtoMember(2)]
        protected decimal rakeback;

        public virtual decimal Rakeback
        {
            get
            {
                return rakeback;
            }
        }

        private Dictionary<int, Tuple<DateTime, decimal>[]> rakebacksByPlayer;

        private bool ignoreRakeback = false;

        public override void AddStatistic(Playerstatistic statistic)
        {
            if (rakebacksByPlayer == null && !ignoreRakeback)
            {
                try
                {
                    var rakebackSettings = (RakeBackSettingsModel)ServiceLocator.Current.GetInstance<ISettingsService>()
                        .GetSettings()
                        .RakeBackSettings.Clone();

                    rakebacksByPlayer = rakebackSettings.RakeBackList
                        .GroupBy(x => x.PlayerId)
                        .ToDictionary(x => x.Key,
                            x => x.OrderBy(p => p.DateBegan).Select(p => Tuple.Create(p.DateBegan, p.Percentage)).ToArray());
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Failed to set rakebacks for report indicator.", e);
                    ignoreRakeback = true;
                }
            }

            base.AddStatistic(statistic);

            AddStatToStatistic(statistic);
            AddReportHand(statistic);

            if (rakebacksByPlayer == null ||
                statistic.Totalrakeincents == 0 ||
                rakebacksByPlayer.Count == 0 ||
                !rakebacksByPlayer.TryGetValue(statistic.PlayerId, out Tuple<DateTime, decimal>[] rakebacks))
            {
                return;
            }

            var rakebackItem = rakebacks.LastOrDefault(x => x.Item1 <= statistic.Time);

            if (rakebackItem == null)
            {
                return;
            }

            rakeback += rakebackItem.Item2 * statistic.Totalrakeincents / 10000m;
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

            rakeback += indicator.rakeback;
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