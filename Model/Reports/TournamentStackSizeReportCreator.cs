﻿//-----------------------------------------------------------------------
// <copyright file="TournamentStackSizeReportCreator.cs" company="Ace Poker Solutions">
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
using Model.Enums;
using Model.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    public class TournamentStackSizeReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player.Name, (short)player.PokerSite);

            foreach (var group in statistics.Where(x => x.IsTourney).GroupBy(x => GetMRatio(x)).ToArray())
            {
                var stat = new MRatioReportRecord();

                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                }

                stat.MRatio = group.Key;
                report.Add(stat);
            }

            return report;
        }

        private EnumMRatio GetMRatio(Playerstatistic stat)
        {
            var mRatioValue = PlayerStatisticCalculator.CalculateMRatio(stat);

            EnumMRatio mRatio;

            if (mRatioValue <= 5)
            {
                mRatio = EnumMRatio.RedZone;
            }
            else if (mRatioValue < 10)
            {
                mRatio = EnumMRatio.OrangeZone;
            }
            else if (mRatioValue < 20)
            {
                mRatio = EnumMRatio.YellowZone;
            }
            else if (mRatioValue < 40)
            {
                mRatio = EnumMRatio.GreenZone;
            }
            else if (mRatioValue < 60)
            {
                mRatio = EnumMRatio.BlueZone;
            }
            else
            {
                mRatio = EnumMRatio.PurpleZone;
            }

            return mRatio;
        }
    }
}