using Microsoft.Practices.ServiceLocation;
using Model.Data;
using DriveHUD.Entities;
using Model.Enums;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            string playerName = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(playerName);

            foreach (var group in statistics.Where(x => x.IsTourney).GroupBy(x => GetMRatio(x)))
            {
                MRatioReportRecord stat = new MRatioReportRecord();

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
