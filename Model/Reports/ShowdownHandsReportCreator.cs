using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Data;
using DriveHUD.Entities;
using Model.HandAnalyzers;
using Cards = HandHistories.Objects.Cards;
using Model.Enums;

namespace Model.Reports
{
    public class ShowdownHandsReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            var analyzer = new HandAnalyzer(HandAnalyzer.GetReportAnalyzers());
            var s = statistics.Where(x => !string.IsNullOrEmpty(x.Cards) && Cards.CardGroup.Parse(x.Cards).Count() == 2 && !string.IsNullOrEmpty(x.Board) && Cards.BoardCards.FromCards(x.Board).Count == 5);

            var hands = s.Where(x => !x.IsTourney).GroupBy(x => analyzer.Analyze(Cards.CardGroup.Parse(x.Cards), Cards.BoardCards.FromCards(x.Board))).ToList();

            if(hands == null || hands.Count() == 0)
            {
                return report;
            }

            hands = ShowdownHandsReportRecord.FilterHands(hands).ToList();

            foreach (var group in hands)
            {
                if (group.Key == null)
                    continue;

                ShowdownHandsReportRecord stat = new ShowdownHandsReportRecord();
                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);

                    stat.ShowdownHand = group.Key.GetRank();
                }

                report.Add(stat);
            }

            return report;
        }
    }

    public class TournamentShowdownHandsReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();
            var analyzer = new HandAnalyzer(HandAnalyzer.GetReportAnalyzers());
            var s = statistics.Where(x => !string.IsNullOrEmpty(x.Cards) && Cards.CardGroup.Parse(x.Cards).Count() == 2 && !string.IsNullOrEmpty(x.Board) && Cards.BoardCards.FromCards(x.Board).Count == 5);
            var hands = s.Where(x => x.IsTourney).GroupBy(x => analyzer.Analyze(Cards.CardGroup.Parse(x.Cards), Cards.BoardCards.FromCards(x.Board))).ToList();

            if (hands == null || hands.Count() == 0)
            {
                return report;
            }

            hands = ShowdownHandsReportRecord.FilterHands(hands).ToList();

            foreach (var group in hands)
            {
                if (group.Key == null)
                    continue;

                ShowdownHandsReportRecord stat = new ShowdownHandsReportRecord();
                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);

                    stat.ShowdownHand = group.Key.GetRank();
                }

                report.Add(stat);
            }

            return report;
        }
    }
}
