using System;
using System.Collections.Generic;

using Model.Reports;

namespace DriveHUD.Application.ReportsLayout
{
    public static class ReportManager
    {
        private static Dictionary<Model.Enums.EnumReports, Tuple<ReportLayoutCreator, IReportCreator>> reports = new Dictionary<Model.Enums.EnumReports, Tuple<ReportLayoutCreator, IReportCreator>>();

        static ReportManager()
        {
            /*  Dashboard Tab */
            Register<HoleCardsLayoutCreator, HoleCardsReportCreator>(Model.Enums.EnumReports.HoleCards);
            Register<OverAllReportLayoutCreator, OverAllReportCreator>(Model.Enums.EnumReports.OverAll);
            Register<PositionLayoutCreator, PositionReportCreator>(Model.Enums.EnumReports.Position);
            Register<SessionsLayoutCreator, SessionsReportCreator>(Model.Enums.EnumReports.Session);
            Register<StakesLayoutCreator, StakesReportCreator>(Model.Enums.EnumReports.Stake);
            Register<TimeLayoutCreator, TimeReportCreator>(Model.Enums.EnumReports.Time);
            Register<ShowdownHandsLayoutCreator, ShowdownHandsReportCreator>(Model.Enums.EnumReports.ShowdownHands);
            Register<PokerSiteLayoutCreator, PokerSiteReportCreator>(Model.Enums.EnumReports.PokerSite);
            Register<OpponentAnalysisLayoutCreator, OpponentAnalysisReportCreator>(Model.Enums.EnumReports.OpponentAnalysis);
            Register<PopulationAnalysisLayoutCreator, PopulationAnalysisReportCreator>(Model.Enums.EnumReports.PopulationAnalysis);
            /*  Tournaments Tab */
            Register<TournamentsResultLayoutCreator, TournamentOverAllReportCreator>(Model.Enums.EnumReports.TournamentResults);
            Register<TournamentsLayoutCreator, TournamentReportCreator>(Model.Enums.EnumReports.Tournaments);
            Register<TournamentStatsLayoutCreator, TournamentOverAllReportCreator>(Model.Enums.EnumReports.TournamentStats);
            Register<TournamentStackSizeLayoutCreator, TournamentStackSizeReportCreator>(Model.Enums.EnumReports.TournamentStackSizes);
            Register<PositionLayoutCreator, TournamentPositionReportCreator>(Model.Enums.EnumReports.TournamentPosition);
            Register<HoleCardsLayoutCreator, TournamentHoleCardsReportCreator>(Model.Enums.EnumReports.TournamentHoleCards);
            Register<ShowdownHandsLayoutCreator, TournamentShowdownHandsReportCreator>(Model.Enums.EnumReports.TournamentShowdownHands);
            Register<TournamentPokerSiteLayoutCreator, TournamentPokerSiteReportCreator>(Model.Enums.EnumReports.TournamentPokerSite);
        }

        private static void Register<TLayout, TCreator>(Model.Enums.EnumReports reportType)
            where TLayout : ReportLayoutCreator, new() where TCreator : IReportCreator, new()
        {
            if (reports.ContainsKey(reportType))
                return;

            reports.Add(reportType, new Tuple<ReportLayoutCreator, IReportCreator>(new TLayout(), new TCreator()));
        }

        public static IReportCreator GetReportCreator(Model.Enums.EnumReports reportType)
        {
            if (!reports.ContainsKey(reportType))
                return null;

            return reports[reportType].Item2;
        }

        public static ReportLayoutCreator GetReportLayout(Model.Enums.EnumReports reportType)
        {
            if (!reports.ContainsKey(reportType))
                return null;

            return reports[reportType].Item1;
        }
    }
}