//-----------------------------------------------------------------------
// <copyright file="ReplayerHelpers.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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
using Model.Reports;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Replayer
{
    public static class ReplayerHelpers
    {
        /// <summary>
        /// Selects last 30 hands that meet requirements ( Pot > 6 BB; is VPIP) from the same session as specified statistic 
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IEnumerable<ReplayerDataModel> CreateSessionHandsList(IEnumerable<Playerstatistic> statistics, Playerstatistic current)
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var bbFilter = settings.GeneralSettings.ReplayerBBFilter;

            IEnumerable<Playerstatistic> potStat = new List<Playerstatistic>();

            if (statistics != null && statistics.Count() > 0)
            {
                if (current.IsTourney)
                {
                    potStat = statistics.Where(x => x.TournamentId == current.TournamentId).OrderByDescending(x => x.Time).Take(30);
                }
                else
                {
                    var session = new SessionsReportCreator().Create(statistics.ToList()).Where(x => x.Statistics.Any(s => s.GameNumber == current.GameNumber));

                    if (session != null && session.Count() > 0)
                    {
                        potStat = session.FirstOrDefault().Statistics.OrderByDescending(x => x.Time);
                    }
                }
            }

            var result = potStat.Where(x => Math.Abs(x.NetWon) > (bbFilter * x.BigBlind) && x.Vpiphands > 0).Select(x => new ReplayerDataModel(x));

            return result;
        }
    }
}